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