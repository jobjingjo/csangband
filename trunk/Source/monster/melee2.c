/*
 * File: melee2.c
 * Purpose: Monster AI routines
 *
 * Copyright (c) 1997 Ben Harrison, David Reeve Sward, Keldon Jones.
 *
 * This work is free software; you can redistribute it and/or modify it
 * under the terms of either:
 *
 * a) the GNU General Public License as published by the Free Software
 *    Foundation, version 2, or
 *
 * b) the "Angband licence":
 *    This software may be copied and distributed for educational, research,
 *    and not for profit purposes provided that this copyright and statement
 *    are included in all such copies.  Other copyrights may also apply.
 */

#include "angband.h"
#include "attack.h"
#include "cave.h"
#include "monster/mon-make.h"
#include "monster/mon-spell.h"
#include "monster/mon-timed.h"
#include "monster/mon-util.h"
#include "object/slays.h"
#include "object/tvalsval.h"
#include "spells.h"
#include "squelch.h"

/*
 * Determine if a bolt will arrive, checking that no monsters are in the way
 */
#define clean_shot(Y1, X1, Y2, X2) \
	projectable(Y1, X1, Y2, X2, PROJECT_STOP)


/*
 * And now for Intelligent monster attacks (including spells).
 *
 * Give monsters more intelligent attack/spell selection based on
 * observations of previous attacks on the player, and/or by allowing
 * the monster to "cheat" and know the player status.
 *
 * Maintain an idea of the player status, and use that information
 * to occasionally eliminate "ineffective" spell attacks.  We could
 * also eliminate ineffective normal attacks, but there is no reason
 * for the monster to do this, since he gains no benefit.
 * Note that MINDLESS monsters are not allowed to use this code.
 * And non-INTELLIGENT monsters only use it partially effectively.
 *
 * Actually learn what the player resists, and use that information
 * to remove attacks or spells before using them. 
 *
 * This has the added advantage that attacks and spells are related.
 * The "smart_learn" option means that the monster "learns" the flags
 * that should be set, and "smart_cheat" means that he "knows" them.
 * So "smart_cheat" means that the "smart" field is always up to date,
 * while "smart_learn" means that the "smart" field is slowly learned.
 * Both of them have the same effect on the "choose spell" routine.
 */

/*
 * Remove the "bad" spells from a spell list
 */
static void remove_bad_spells(int m_idx, bitflag f[RSF_SIZE])
{
	monster_type *m_ptr = cave_monster(cave, m_idx);
	monster_race *r_ptr = &r_info[m_ptr.r_idx];

	bitflag f2[RSF_SIZE], ai_flags[OF_SIZE];

	size_t i;	
	u32b smart = 0L;

	/* Stupid monsters act randomly */
	if (rf_has(r_ptr.flags, RF_STUPID)) return;

	/* Take working copy of spell flags */
	rsf_copy(f2, f);

	/* Don't heal if full */
	if (m_ptr.hp >= m_ptr.maxhp) rsf_off(f2, RSF_HEAL);
	
	/* Don't haste if hasted with time remaining */
	if (m_ptr.m_timed[MON_TMD_FAST] > 10) rsf_off(f2, RSF_HASTE);

	/* Don't teleport to if the player is already next to us */
	if (m_ptr.cdis == 1) rsf_off(f2, RSF_TELE_TO);

	/* Update acquired knowledge */
	of_wipe(ai_flags);
	if (OPT(birth_ai_learn))
	{
		/* Occasionally forget player status */
		if (one_in_(100))
			of_wipe(m_ptr.known_pflags);

		/* Use the memorized flags */
		smart = m_ptr.smart;
		of_copy(ai_flags, m_ptr.known_pflags);
	}

	/* Cheat if requested */
	if (OPT(birth_ai_cheat)) {
		for (i = 0; i < OF_MAX; i++)
			if (check_state(p_ptr, i, p_ptr.state.flags))
				of_on(ai_flags, i);
		if (!p_ptr.msp) smart |= SM_IMM_MANA;
	}

	/* Cancel out certain flags based on knowledge */
	if (!of_is_empty(ai_flags))
		unset_spells(f2, ai_flags, r_ptr);

	if (smart & SM_IMM_MANA && randint0(100) <
			50 * (rf_has(r_ptr.flags, RF_SMART) ? 2 : 1))
		rsf_off(f2, RSF_DRAIN_MANA);

	/* use working copy of spell flags */
	rsf_copy(f, f2);
}


/*
 * Determine if there is a space near the selected spot in which
 * a summoned creature can appear
 */
static bool summon_possible(int y1, int x1)
{
	int y, x;

	/* Start at the location, and check 2 grids in each dir */
	for (y = y1 - 2; y <= y1 + 2; y++)
	{
		for (x = x1 - 2; x <= x1 + 2; x++)
		{
			/* Ignore illegal locations */
			if (!in_bounds(y, x)) continue;

			/* Only check a circular area */
			if (distance(y1, x1, y, x) > 2) continue;

			/* Hack: no summon on glyph of warding */
			if (cave.feat[y][x] == FEAT_GLYPH) continue;

			/* Require empty floor grid in line of sight */
			if (cave_empty_bold(y, x) && los(y1, x1, y, x))
			{
				return (true);
			}
		}
	}

	return false;
}


/*
 * Offsets for the spell indices
 */
#define BASE2_LOG_HACK_FRAGMENT(A,B) ((((A)>>(B)) & 1)*(B))

/** Unfortunately, this macro only works for an isolated bitflag 
 * (exactly one bit set, all others reset).  It also only works for at most 
 * 32 bits. 
 */
#define BASE2_LOG_HACK(A)	\
	(BASE2_LOG_HACK_FRAGMENT(A,31)+BASE2_LOG_HACK_FRAGMENT(A,30)+ \
	 BASE2_LOG_HACK_FRAGMENT(A,29)+BASE2_LOG_HACK_FRAGMENT(A,28)+ \
	 BASE2_LOG_HACK_FRAGMENT(A,27)+BASE2_LOG_HACK_FRAGMENT(A,26)+ \
	 BASE2_LOG_HACK_FRAGMENT(A,25)+BASE2_LOG_HACK_FRAGMENT(A,24)+ \
	 BASE2_LOG_HACK_FRAGMENT(A,23)+BASE2_LOG_HACK_FRAGMENT(A,22)+ \
	 BASE2_LOG_HACK_FRAGMENT(A,21)+BASE2_LOG_HACK_FRAGMENT(A,20)+ \
	 BASE2_LOG_HACK_FRAGMENT(A,19)+BASE2_LOG_HACK_FRAGMENT(A,18)+ \
	 BASE2_LOG_HACK_FRAGMENT(A,17)+BASE2_LOG_HACK_FRAGMENT(A,16)+ \
	 BASE2_LOG_HACK_FRAGMENT(A,15)+BASE2_LOG_HACK_FRAGMENT(A,14)+ \
	 BASE2_LOG_HACK_FRAGMENT(A,13)+BASE2_LOG_HACK_FRAGMENT(A,12)+ \
	 BASE2_LOG_HACK_FRAGMENT(A,11)+BASE2_LOG_HACK_FRAGMENT(A,10)+ \
	 BASE2_LOG_HACK_FRAGMENT(A,9)+BASE2_LOG_HACK_FRAGMENT(A,8)+ \
	 BASE2_LOG_HACK_FRAGMENT(A,7)+BASE2_LOG_HACK_FRAGMENT(A,6)+ \
	 BASE2_LOG_HACK_FRAGMENT(A,5)+BASE2_LOG_HACK_FRAGMENT(A,4)+ \
	 BASE2_LOG_HACK_FRAGMENT(A,3)+BASE2_LOG_HACK_FRAGMENT(A,2)+ \
	 BASE2_LOG_HACK_FRAGMENT(A,1))


/*
 * Have a monster choose a spell to cast.
 *
 * Note that the monster's spell list has already had "useless" spells
 * (bolts that won't hit the player, summons without room, etc.) removed.
 * Perhaps that should be done by this function.
 *
 * Stupid monsters will just pick a spell randomly.  Smart monsters
 * will choose more "intelligently".
 *
 * This function could be an efficiency bottleneck.
 */
static int choose_attack_spell(int m_idx, bitflag f[RSF_SIZE])
{
	monster_type *m_ptr = cave_monster(cave, m_idx);
	monster_race *r_ptr = &r_info[m_ptr.r_idx];

	int num = 0;
	byte spells[RSF_MAX];

	int i, py = p_ptr.py, px = p_ptr.px;

	bool has_escape, has_attack, has_summon, has_tactic;
	bool has_annoy, has_haste, has_heal;


	/* Smart monsters restrict their spell choices. */
	if (OPT(birth_ai_smart) && !rf_has(r_ptr.flags, RF_STUPID))
	{
		/* What have we got? */
		has_escape = test_spells(f, RST_ESCAPE);
		has_attack = test_spells(f, RST_ATTACK | RST_BOLT | RST_BALL | RST_BREATH);
		has_summon = test_spells(f, RST_SUMMON);
		has_tactic = test_spells(f, RST_TACTIC);
		has_annoy = test_spells(f, RST_ANNOY);
		has_haste = test_spells(f, RST_HASTE);
		has_heal = test_spells(f, RST_HEAL);

		/*** Try to pick an appropriate spell type ***/

		/* Hurt badly or afraid, attempt to flee */
		if (has_escape && ((m_ptr.hp < m_ptr.maxhp / 4) || m_ptr.m_timed[MON_TMD_FEAR]))
		{
			/* Choose escape spell */
			set_spells(f, RST_ESCAPE);
		}

		/* Still hurt badly, couldn't flee, attempt to heal */
		else if (has_heal && m_ptr.hp < m_ptr.maxhp / 4)
		{
			/* Choose heal spell */
			set_spells(f, RST_HEAL);
		}

		/* Player is close and we have attack spells, blink away */
		else if (has_tactic && (distance(py, px, m_ptr.fy, m_ptr.fx) < 4) &&
		         has_attack && (randint0(100) < 75))
		{
			/* Choose tactical spell */
			set_spells(f, RST_TACTIC);
		}

		/* We're hurt (not badly), try to heal */
		else if (has_heal && (m_ptr.hp < m_ptr.maxhp * 3 / 4) &&
		         (randint0(100) < 60))
		{
			/* Choose heal spell */
			set_spells(f, RST_HEAL);
		}

		/* Summon if possible (sometimes) */
		else if (has_summon && (randint0(100) < 50))
		{
			/* Choose summon spell */
			set_spells(f, RST_SUMMON);
		}

		/* Attack spell (most of the time) */
		else if (has_attack && (randint0(100) < 85))
		{
			/* Choose attack spell */
			set_spells(f, RST_ATTACK | RST_BOLT | RST_BALL | RST_BREATH);
		}

		/* Try another tactical spell (sometimes) */
		else if (has_tactic && (randint0(100) < 50))
		{
			/* Choose tactic spell */
			set_spells(f, RST_TACTIC);
		}

		/* Haste self if we aren't already somewhat hasted (rarely) */
		else if (has_haste && (randint0(100) < (20 + r_ptr.speed - m_ptr.mspeed)))
		{
			/* Choose haste spell */
			set_spells(f, RST_HASTE);
		}

		/* Annoy player (most of the time) */
		else if (has_annoy && (randint0(100) < 85))
		{
			/* Choose annoyance spell */
			set_spells(f, RST_ANNOY);
		}

		/* Else choose no spell */
		else
		{
			rsf_wipe(f);
		}

		/* Anything left? */
		if (rsf_is_empty(f)) return (FLAG_END);
	}

	/* Extract all spells: "innate", "normal", "bizarre" */
	for (i = FLAG_START, num = 0; i < RSF_MAX; i++)
	{
		if (rsf_has(f, i)) spells[num++] = i;
	}

	/* Paranoia */
	if (num == 0) return 0;

	/* Pick at random */
	return (spells[randint0(num)]);
}



/*
 * Returns whether a given monster will try to run from the player.
 *
 * Monsters will attempt to avoid very powerful players.  See below.
 *
 * Because this function is called so often, little details are important
 * for efficiency.  Like not using "mod" or "div" when possible.  And
 * attempting to check the conditions in an optimal order.  Note that
 * "(x << 2) == (x * 4)" if "x" has enough bits to hold the result.
 *
 * Note that this function is responsible for about one to five percent
 * of the processor use in normal conditions...
 */
static int mon_will_run(int m_idx)
{
	monster_type *m_ptr = cave_monster(cave, m_idx);

	monster_race *r_ptr = &r_info[m_ptr.r_idx];

	u16b p_lev, m_lev;
	u16b p_chp, p_mhp;
	u16b m_chp, m_mhp;
	u32b p_val, m_val;

	/* Keep monsters from running too far away */
	if (m_ptr.cdis > MAX_SIGHT + 5) return (false);

	/* All "afraid" monsters will run away */
	if (m_ptr.m_timed[MON_TMD_FEAR]) return (true);

	/* Nearby monsters will not become terrified */
	if (m_ptr.cdis <= 5) return (false);

	/* Examine player power (level) */
	p_lev = p_ptr.lev;

	/* Examine monster power (level plus morale) */
	m_lev = r_ptr.level + (m_idx & 0x08) + 25;

	/* Optimize extreme cases below */
	if (m_lev > p_lev + 4) return (false);
	if (m_lev + 4 <= p_lev) return (true);

	/* Examine player health */
	p_chp = p_ptr.chp;
	p_mhp = p_ptr.mhp;

	/* Examine monster health */
	m_chp = m_ptr.hp;
	m_mhp = m_ptr.maxhp;

	/* Prepare to optimize the calculation */
	p_val = (p_lev * p_mhp) + (p_chp << 2);	/* div p_mhp */
	m_val = (m_lev * m_mhp) + (m_chp << 2);	/* div m_mhp */

	/* Strong players scare strong monsters */
	if (p_val * m_mhp > m_val * p_mhp) return (true);

	/* Assume no terror */
	return (false);
}




/*
 * Choose the "best" direction for "flowing"
 *
 * Note that ghosts and rock-eaters are never allowed to "flow",
 * since they should move directly towards the player.
 *
 * Prefer "non-diagonal" directions, but twiddle them a little
 * to angle slightly towards the player's actual location.
 *
 * Allow very perceptive monsters to track old "spoor" left by
 * previous locations occupied by the player.  This will tend
 * to have monsters end up either near the player or on a grid
 * recently occupied by the player (and left via "teleport").
 *
 * Note that if "smell" is turned on, all monsters get vicious.
 *
 * Also note that teleporting away from a location will cause
 * the monsters who were chasing you to converge on that location
 * as long as you are still near enough to "annoy" them without
 * being close enough to chase directly.  I have no idea what will
 * happen if you combine "smell" with low "aaf" values.
 */
static bool get_moves_aux(struct cave *c, int m_idx, int *yp, int *xp)
{
	int py = p_ptr.py;
	int px = p_ptr.px;

	int i, y, x, y1, x1;

	int when = 0;
	int cost = 999;

	monster_type *m_ptr = cave_monster(cave, m_idx);
	monster_race *r_ptr = &r_info[m_ptr.r_idx];

	/* Monster can go through rocks */
	if (flags_test(r_ptr.flags, RF_SIZE, RF_PASS_WALL, RF_KILL_WALL, FLAG_END)) return (false);

	/* Monster location */
	y1 = m_ptr.fy;
	x1 = m_ptr.fx;

	/* The player is not currently near the monster grid */
	if (c.when[y1][x1] < c.when[py][px])
	{
		/* The player has never been near the monster grid */
		if (c.when[y1][x1] == 0) return (false);

		/* The monster is not allowed to track the player */
		if (!OPT(birth_ai_smell)) return (false);
	}

	/* Monster is too far away to notice the player */
	if (c.cost[y1][x1] > MONSTER_FLOW_DEPTH) return (false);
	if (c.cost[y1][x1] > r_ptr.aaf) return (false);

	/* Hack -- Player can see us, run towards him */
	if (player_has_los_bold(y1, x1)) return (false);

	/* Check nearby grids, diagonals first */
	for (i = 7; i >= 0; i--)
	{
		/* Get the location */
		y = y1 + ddy_ddd[i];
		x = x1 + ddx_ddd[i];

		/* Ignore illegal locations */
		if (c.when[y][x] == 0) continue;

		/* Ignore ancient locations */
		if (c.when[y][x] < when) continue;

		/* Ignore distant locations */
		if (c.cost[y][x] > cost) continue;

		/* Save the cost and time */
		when = c.when[y][x];
		cost = c.cost[y][x];

		/* Hack -- Save the "twiddled" location */
		(*yp) = py + 16 * ddy_ddd[i];
		(*xp) = px + 16 * ddx_ddd[i];
	}

	/* No legal move (?) */
	if (!when) return (false);

	/* Success */
	return (true);
}

/*
 * Provide a location to flee to, but give the player a wide berth.
 *
 * A monster may wish to flee to a location that is behind the player,
 * but instead of heading directly for it, the monster should "swerve"
 * around the player so that he has a smaller chance of getting hit.
 */
static bool get_fear_moves_aux(struct cave *c, int m_idx, int *yp, int *xp)
{
	int y, x, y1, x1, fy, fx, py, px, gy = 0, gx = 0;
	int when = 0, score = -1;
	int i;

	monster_type *m_ptr = cave_monster(cave, m_idx);
	monster_race *r_ptr = &r_info[m_ptr.r_idx];

	/* Player location */
	py = p_ptr.py;
	px = p_ptr.px;

	/* Monster location */
	fy = m_ptr.fy;
	fx = m_ptr.fx;

	/* Desired destination */
	y1 = fy - (*yp);
	x1 = fx - (*xp);

	/* The player is not currently near the monster grid */
	if (c.when[fy][fx] < c.when[py][px])
	{
		/* No reason to attempt flowing */
		return (false);
	}

	/* Monster is too far away to use flow information */
	if (c.cost[fy][fx] > MONSTER_FLOW_DEPTH) return (false);
	if (c.cost[fy][fx] > r_ptr.aaf) return (false);

	/* Check nearby grids, diagonals first */
	for (i = 7; i >= 0; i--)
	{
		int dis, s;

		/* Get the location */
		y = fy + ddy_ddd[i];
		x = fx + ddx_ddd[i];

		/* Ignore illegal locations */
		if (c.when[y][x] == 0) continue;

		/* Ignore ancient locations */
		if (c.when[y][x] < when) continue;

		/* Calculate distance of this grid from our destination */
		dis = distance(y, x, y1, x1);

		/* Score this grid */
		s = 5000 / (dis + 3) - 500 / (c.cost[y][x] + 1);

		/* No negative scores */
		if (s < 0) s = 0;

		/* Ignore lower scores */
		if (s < score) continue;

		/* Save the score and time */
		when = c.when[y][x];
		score = s;

		/* Save the location */
		gy = y;
		gx = x;
	}

	/* No legal move (?) */
	if (!when) return (false);

	/* Find deltas */
	(*yp) = fy - gy;
	(*xp) = fx - gx;

	/* Success */
	return (true);
}



/*
 * Hack -- Precompute a bunch of calls to distance() in find_safety() and
 * find_hiding().
 *
 * The pair of arrays dist_offsets_y[n] and dist_offsets_x[n] contain the
 * offsets of all the locations with a distance of n from a central point,
 * with an offset of (0,0) indicating no more offsets at this distance.
 *
 * This is, of course, fairly unreadable, but it eliminates multiple loops
 * from the previous version.
 *
 * It is probably better to replace these arrays with code to compute
 * the relevant arrays, even if the storage is pre-allocated in hard
 * coded sizes.  At the very least, code should be included which is
 * able to generate and dump these arrays (ala "los()").  XXX XXX XXX
 *
 * Also, the storage needs could be reduced by using char.  XXX XXX XXX
 *
 * These arrays could be combined into two big arrays, using sub-arrays
 * to hold the offsets and lengths of each portion of the sub-arrays, and
 * this could perhaps also be used somehow in the "look" code.  XXX XXX XXX
 */


static const int d_off_y_0[] =
{ 0 };

static const int d_off_x_0[] =
{ 0 };


static const int d_off_y_1[] =
{ -1, -1, -1, 0, 0, 1, 1, 1, 0 };

static const int d_off_x_1[] =
{ -1, 0, 1, -1, 1, -1, 0, 1, 0 };


static const int d_off_y_2[] =
{ -1, -1, -2, -2, -2, 0, 0, 1, 1, 2, 2, 2, 0 };

static const int d_off_x_2[] =
{ -2, 2, -1, 0, 1, -2, 2, -2, 2, -1, 0, 1, 0 };


static const int d_off_y_3[] =
{ -1, -1, -2, -2, -3, -3, -3, 0, 0, 1, 1, 2, 2,
  3, 3, 3, 0 };

static const int d_off_x_3[] =
{ -3, 3, -2, 2, -1, 0, 1, -3, 3, -3, 3, -2, 2,
  -1, 0, 1, 0 };


static const int d_off_y_4[] =
{ -1, -1, -2, -2, -3, -3, -3, -3, -4, -4, -4, 0,
  0, 1, 1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 0 };

static const int d_off_x_4[] =
{ -4, 4, -3, 3, -2, -3, 2, 3, -1, 0, 1, -4, 4,
  -4, 4, -3, 3, -2, -3, 2, 3, -1, 0, 1, 0 };


static const int d_off_y_5[] =
{ -1, -1, -2, -2, -3, -3, -4, -4, -4, -4, -5, -5,
  -5, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 4, 4, 5, 5,
  5, 0 };

static const int d_off_x_5[] =
{ -5, 5, -4, 4, -4, 4, -2, -3, 2, 3, -1, 0, 1,
  -5, 5, -5, 5, -4, 4, -4, 4, -2, -3, 2, 3, -1,
  0, 1, 0 };


static const int d_off_y_6[] =
{ -1, -1, -2, -2, -3, -3, -4, -4, -5, -5, -5, -5,
  -6, -6, -6, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5,
  5, 5, 6, 6, 6, 0 };

static const int d_off_x_6[] =
{ -6, 6, -5, 5, -5, 5, -4, 4, -2, -3, 2, 3, -1,
  0, 1, -6, 6, -6, 6, -5, 5, -5, 5, -4, 4, -2,
  -3, 2, 3, -1, 0, 1, 0 };


static const int d_off_y_7[] =
{ -1, -1, -2, -2, -3, -3, -4, -4, -5, -5, -5, -5,
  -6, -6, -6, -6, -7, -7, -7, 0, 0, 1, 1, 2, 2, 3,
  3, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 7, 0 };

static const int d_off_x_7[] =
{ -7, 7, -6, 6, -6, 6, -5, 5, -4, -5, 4, 5, -2,
  -3, 2, 3, -1, 0, 1, -7, 7, -7, 7, -6, 6, -6,
  6, -5, 5, -4, -5, 4, 5, -2, -3, 2, 3, -1, 0,
  1, 0 };


static const int d_off_y_8[] =
{ -1, -1, -2, -2, -3, -3, -4, -4, -5, -5, -6, -6,
  -6, -6, -7, -7, -7, -7, -8, -8, -8, 0, 0, 1, 1,
  2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7,
  8, 8, 8, 0 };

static const int d_off_x_8[] =
{ -8, 8, -7, 7, -7, 7, -6, 6, -6, 6, -4, -5, 4,
  5, -2, -3, 2, 3, -1, 0, 1, -8, 8, -8, 8, -7,
  7, -7, 7, -6, 6, -6, 6, -4, -5, 4, 5, -2, -3,
  2, 3, -1, 0, 1, 0 };


static const int d_off_y_9[] =
{ -1, -1, -2, -2, -3, -3, -4, -4, -5, -5, -6, -6,
  -7, -7, -7, -7, -8, -8, -8, -8, -9, -9, -9, 0,
  0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 7,
  7, 8, 8, 8, 8, 9, 9, 9, 0 };

static const int d_off_x_9[] =
{ -9, 9, -8, 8, -8, 8, -7, 7, -7, 7, -6, 6, -4,
  -5, 4, 5, -2, -3, 2, 3, -1, 0, 1, -9, 9, -9,
  9, -8, 8, -8, 8, -7, 7, -7, 7, -6, 6, -4, -5,
  4, 5, -2, -3, 2, 3, -1, 0, 1, 0 };


static const int *dist_offsets_y[10] =
{
	d_off_y_0, d_off_y_1, d_off_y_2, d_off_y_3, d_off_y_4,
	d_off_y_5, d_off_y_6, d_off_y_7, d_off_y_8, d_off_y_9
};

static const int *dist_offsets_x[10] =
{
	d_off_x_0, d_off_x_1, d_off_x_2, d_off_x_3, d_off_x_4,
	d_off_x_5, d_off_x_6, d_off_x_7, d_off_x_8, d_off_x_9
};


/*
 * Choose a "safe" location near a monster for it to run toward.
 *
 * A location is "safe" if it can be reached quickly and the player
 * is not able to fire into it (it isn't a "clean shot").  So, this will
 * cause monsters to "duck" behind walls.  Hopefully, monsters will also
 * try to run towards corridor openings if they are in a room.
 *
 * This function may take lots of CPU time if lots of monsters are fleeing.
 *
 * Return true if a safe location is available.
 */
static bool find_safety(struct cave *c, int m_idx, int *yp, int *xp)
{
	monster_type *m_ptr = cave_monster(cave, m_idx);

	int fy = m_ptr.fy;
	int fx = m_ptr.fx;

	int py = p_ptr.py;
	int px = p_ptr.px;

	int i, y, x, dy, dx, d, dis;
	int gy = 0, gx = 0, gdis = 0;

	const int *y_offsets;
	const int *x_offsets;

	/* Start with adjacent locations, spread further */
	for (d = 1; d < 10; d++)
	{
		/* Get the lists of points with a distance d from (fx, fy) */
		y_offsets = dist_offsets_y[d];
		x_offsets = dist_offsets_x[d];

		/* Check the locations */
		for (i = 0, dx = x_offsets[0], dy = y_offsets[0];
		     dx != 0 || dy != 0;
		     i++, dx = x_offsets[i], dy = y_offsets[i])
		{
			y = fy + dy;
			x = fx + dx;

			/* Skip illegal locations */
			if (!in_bounds_fully(y, x)) continue;

			/* Skip locations in a wall */
			if (!cave_floor_bold(y, x)) continue;

			/* Ignore grids very far from the player */
			if (c.when[y][x] < c.when[py][px]) continue;

			/* Ignore too-distant grids */
			if (c.cost[y][x] > c.cost[fy][fx] + 2 * d) continue;

			/* Check for absence of shot (more or less) */
			if (!player_has_los_bold(y,x))
			{
				/* Calculate distance from player */
				dis = distance(y, x, py, px);

				/* Remember if further than previous */
				if (dis > gdis)
				{
					gy = y;
					gx = x;
					gdis = dis;
				}
			}
		}

		/* Check for success */
		if (gdis > 0)
		{
			/* Good location */
			(*yp) = fy - gy;
			(*xp) = fx - gx;

			/* Found safe place */
			return (true);
		}
	}

	/* No safe place */
	return (false);
}




/*
 * Choose a good hiding place near a monster for it to run toward.
 *
 * Pack monsters will use this to "ambush" the player and lure him out
 * of corridors into open space so they can swarm him.
 *
 * Return true if a good location is available.
 */
static bool find_hiding(int m_idx, int *yp, int *xp)
{
	monster_type *m_ptr = cave_monster(cave, m_idx);

	int fy = m_ptr.fy;
	int fx = m_ptr.fx;

	int py = p_ptr.py;
	int px = p_ptr.px;

	int i, y, x, dy, dx, d, dis;
	int gy = 0, gx = 0, gdis = 999, min;

	const int *y_offsets, *x_offsets;

	/* Closest distance to get */
	min = distance(py, px, fy, fx) * 3 / 4 + 2;

	/* Start with adjacent locations, spread further */
	for (d = 1; d < 10; d++)
	{
		/* Get the lists of points with a distance d from (fx, fy) */
		y_offsets = dist_offsets_y[d];
		x_offsets = dist_offsets_x[d];

		/* Check the locations */
		for (i = 0, dx = x_offsets[0], dy = y_offsets[0];
		     dx != 0 || dy != 0;
		     i++, dx = x_offsets[i], dy = y_offsets[i])
		{
			y = fy + dy;
			x = fx + dx;

			/* Skip illegal locations */
			if (!in_bounds_fully(y, x)) continue;

			/* Skip occupied locations */
			if (!cave_empty_bold(y, x)) continue;

			/* Check for hidden, available grid */
			if (!player_has_los_bold(y, x) && (clean_shot(fy, fx, y, x)))
			{
				/* Calculate distance from player */
				dis = distance(y, x, py, px);

				/* Remember if closer than previous */
				if (dis < gdis && dis >= min)
				{
					gy = y;
					gx = x;
					gdis = dis;
				}
			}
		}

		/* Check for success */
		if (gdis < 999)
		{
			/* Good location */
			(*yp) = fy - gy;
			(*xp) = fx - gx;

			/* Found good place */
			return (true);
		}
	}

	/* No good place */
	return (false);
}


/*
 * Hack -- compare the "strength" of two monsters XXX XXX XXX
 */
static int compare_monsters(const monster_type *m_ptr, const monster_type *n_ptr)
{
	monster_race *r_ptr;

	u32b mexp1, mexp2;

	/* Race 1 */
	r_ptr = &r_info[m_ptr.r_idx];

	/* Extract mexp */
	mexp1 = r_ptr.mexp;

	/* Race 2 */
	r_ptr = &r_info[n_ptr.r_idx];

	/* Extract mexp */
	mexp2 = r_ptr.mexp;

	/* Compare */
	if (mexp1 < mexp2) return (-1);
	if (mexp1 > mexp2) return (1);

	/* Assume equal */
	return (0);
}

/*
 * Critical blow.  All hits that do 95% of total possible damage,
 * and which also do at least 20 damage, or, sometimes, N damage.
 * This is used only to determine "cuts" and "stuns".
 */
static int monster_critical(int dice, int sides, int dam)
{
	int max = 0;
	int total = dice * sides;

	/* Must do at least 95% of perfect */
	if (dam < total * 19 / 20) return (0);

	/* Weak blows rarely work */
	if ((dam < 20) && (randint0(100) >= dam)) return (0);

	/* Perfect damage */
	if (dam == total) max++;

	/* Super-charge */
	if (dam >= 20)
	{
		while (randint0(100) < 2) max++;
	}

	/* Critical damage */
	if (dam > 45) return (6 + max);
	if (dam > 33) return (5 + max);
	if (dam > 25) return (4 + max);
	if (dam > 18) return (3 + max);
	if (dam > 11) return (2 + max);
	return (1 + max);
}

/*
 * Determine if a monster attack against the player succeeds.
 */
bool check_hit(struct player *p, int power, int level)
{
	int chance, ac;

	/* Calculate the "attack quality" */
	chance = (power + (level * 3));

	/* Total armor */
	ac = p.state.ac + p.state.to_a;

	/* if the monster checks vs ac, the player learns ac bonuses */
	/* XXX Eddie should you only learn +ac on miss, -ac on hit?  who knows */
	object_notice_on_defend(p);

	/* Check if the player was hit */
	return test_hit(chance, ac, true);
}


#define MAX_DESC_INSULT 8


/*
 * Hack -- possible "insult" messages
 */
static const char *desc_insult[MAX_DESC_INSULT] =
{
	"insults you!",
	"insults your mother!",
	"gives you the finger!",
	"humiliates you!",
	"defiles you!",
	"dances around you!",
	"makes obscene gestures!",
	"moons you!!!"
};


#define MAX_DESC_MOAN 8


/*
 * Hack -- possible "insult" messages
 */
static const char *desc_moan[MAX_DESC_MOAN] =
{
	"wants his mushrooms back.",
	"tells you to get off his land.",
	"looks for his dogs. ",
	"says 'Did you kill my Fang?' ",
	"asks 'Do you want to buy any mushrooms?' ",
	"seems sad about something.",
	"asks if you have seen his dogs.",
	"mumbles something about mushrooms."
};










/* Test functions */
bool (*testfn_make_attack_normal)(struct monster *m, struct player *p) = make_attack_normal;
