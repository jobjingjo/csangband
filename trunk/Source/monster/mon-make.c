/*
 * File: mon-make.c
 * Purpose: Monster creation / placement code.
 *
 * Copyright (c) 1997-2007 Ben Harrison, James E. Wilson, Robert A. Koeneke
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
#include "history.h"
#include "target.h"
#include "monster/mon-lore.h"
#include "monster/mon-make.h"
#include "monster/mon-timed.h"
#include "monster/mon-util.h"
#include "object/tvalsval.h"


/*
 * Delete the monster, if any, at a given location
 */
void delete_monster(int y, int x)
{
	/* Paranoia */
	if (!in_bounds(y, x)) return;

	/* Delete the monster (if any) */
	if (cave.m_idx[y][x] > 0) delete_monster_idx(cave.m_idx[y][x]);
}


/*
 * Apply a "monster restriction function" to the "monster allocation table"
 */
void get_mon_num_prep(void)
{
	int i;

	/* Scan the allocation table */
	for (i = 0; i < alloc_race_size; i++)
	{
		/* Get the entry */
		alloc_entry *entry = &alloc_race_table[i];

		/* Accept monsters which pass the restriction, if any */
		if (!get_mon_num_hook || (*get_mon_num_hook)(entry.index))
		{
			/* Accept this monster */
			entry.prob2 = entry.prob1;
		}

		/* Do not use this monster */
		else
		{
			/* Decline this monster */
			entry.prob2 = 0;
		}
	}

	/* Success */
	return;
}

/*
 * Maximum size of a group of monsters
 */
#define GROUP_MAX	25

/*
 * Pick a monster group size. Used for monsters with the FRIENDS
 * flag and monsters with the ESCORT/ESCORTS flags.
 */
static int group_size_1(int r_idx)
{
	monster_race *r_ptr = &r_info[r_idx];

	int total, extra = 0;

	/* Pick a group size */
	total = randint1(13);

	/* Hard monsters, small groups */
	if (r_ptr.level > p_ptr.depth)
	{
		extra = r_ptr.level - p_ptr.depth;
		extra = 0 - randint1(extra);
	}

	/* Easy monsters, large groups */
	else if (r_ptr.level < p_ptr.depth)
	{
		extra = p_ptr.depth - r_ptr.level;
		extra = randint1(extra);
	}

	/* Modify the group size */
	total += extra;

	/* Minimum size */
	if (total < 1) total = 1;

	/* Maximum size */
	if (total > GROUP_MAX) total = GROUP_MAX;

	return total;
}
		
/*
 * Pick a monster group size. Used for monsters with the FRIEND
 * flag.
 */
static int group_size_2(int r_idx)
{
	monster_race *r_ptr = &r_info[r_idx];

	int total, extra = 0;

	/* Start small */
	total = 1;

	/* Easy monsters, large groups */
	if (r_ptr.level < p_ptr.depth)
	{
		extra = 2 * (p_ptr.depth - r_ptr.level);
		extra = randint1(extra);
	}

	/* Modify the group size */
	total += extra;

	/* Maximum size */
	if (total > GROUP_MAX) total = GROUP_MAX;

	return total;
}

		
/*
 * Attempt to place a "group" of monsters around the given location
 */
static bool place_new_monster_group(struct cave *c, int y, int x, int r_idx,
	bool slp, int total, byte origin)
{
	int n, i;

	int hack_n;

	byte hack_y[GROUP_MAX];
	byte hack_x[GROUP_MAX];

	/* Start on the monster */
	hack_n = 1;
	hack_x[0] = x;
	hack_y[0] = y;

	/* Puddle monsters, breadth first, up to total */
	for (n = 0; (n < hack_n) && (hack_n < total); n++) {
		/* Grab the location */
		int hx = hack_x[n];
		int hy = hack_y[n];

		/* Check each direction, up to total */
		for (i = 0; (i < 8) && (hack_n < total); i++) {
			int mx = hx + ddx_ddd[i];
			int my = hy + ddy_ddd[i];

			/* Walls and Monsters block flow */
			if (!cave_empty_bold(my, mx)) continue;

			/* Attempt to place another monster */
			if (place_new_monster_one(my, mx, r_idx, slp, origin)) {
				/* Add it to the "hack" set */
				hack_y[hack_n] = my;
				hack_x[hack_n] = mx;
				hack_n++;
			}
		}
	}

	/* Success */
	return (true);
}


/*
 * Hack -- help pick an escort type
 */
static int place_monster_idx = 0;

/*
 * Hack -- help pick an escort type
 */
static bool place_monster_okay(int r_idx)
{
	monster_race *r_ptr = &r_info[place_monster_idx];

	monster_race *z_ptr = &r_info[r_idx];

	/* Require similar "race" */
	if (z_ptr.d_char != r_ptr.d_char) return (false);

	/* Skip more advanced monsters */
	if (z_ptr.level > r_ptr.level) return (false);

	/* Skip unique monsters */
	if (rf_has(z_ptr.flags, RF_UNIQUE)) return (false);

	/* Paranoia -- Skip identical monsters */
	if (place_monster_idx == r_idx) return (false);

	/* Okay */
	return (true);
}

/*
 * Create magical stairs after finishing a quest monster.
 */
static void build_quest_stairs(int y, int x)
{
	int ny, nx;


	/* Stagger around */
	while (!cave_valid_bold(y, x))
	{
		int d = 1;

		/* Pick a location */
		scatter(&ny, &nx, y, x, d, 0);

		/* Stagger */
		y = ny; x = nx;
	}

	/* Destroy any objects */
	delete_object(y, x);

	/* Explain the staircase */
	msg("A magical staircase appears...");

	/* Create stairs down */
	cave_set_feat(cave, y, x, FEAT_MORE);

	/* Update the visuals */
	p_ptr.update |= (PU_UPDATE_VIEW | PU_MONSTERS);

	/* Fully update the flow */
	p_ptr.update |= (PU_FORGET_FLOW | PU_UPDATE_FLOW);
}


/**
 * Handle the "death" of a monster.
 *
 * Disperse treasures carried by the monster centered at the monster location.
 * Note that objects dropped may disappear in crowded rooms.
 *
 * Check for "Quest" completion when a quest monster is killed.
 *
 * Note that only the player can induce "monster_death()" on Uniques.
 * Thus (for now) all Quest monsters should be Uniques.
 */
void monster_death(int m_idx, bool stats)
{
	int i, y, x;
	int dump_item = 0;
	int dump_gold = 0;
	int total = 0;
	s16b this_o_idx, next_o_idx = 0;

	monster_type *m_ptr = cave_monster(cave, m_idx);
	monster_race *r_ptr = &r_info[m_ptr.r_idx];

	bool visible = (m_ptr.ml || rf_has(r_ptr.flags, RF_UNIQUE));

	object_type *i_ptr;
	object_type object_type_body;

	/* Get the location */
	y = m_ptr.fy;
	x = m_ptr.fx;
	
	/* Delete any mimicked objects */
	if (m_ptr.mimicked_o_idx > 0)
		delete_object_idx(m_ptr.mimicked_o_idx);

	/* Drop objects being carried */
	for (this_o_idx = m_ptr.hold_o_idx; this_o_idx; this_o_idx = next_o_idx) {
		object_type *o_ptr;

		/* Get the object */
		o_ptr = object_byid(this_o_idx);

		/* Line up the next object */
		next_o_idx = o_ptr.next_o_idx;

		/* Paranoia */
		o_ptr.held_m_idx = 0;

		/* Get local object, copy it and delete the original */
		i_ptr = &object_type_body;
		object_copy(i_ptr, o_ptr);
		delete_object_idx(this_o_idx);

		/* Count it and drop it - refactor once origin is a bitflag */
		if (!stats) {
			if ((i_ptr.tval == TV_GOLD) && (i_ptr.origin != ORIGIN_STOLEN))
				dump_gold++;
			else if ((i_ptr.tval != TV_GOLD) && ((i_ptr.origin == ORIGIN_DROP)
					|| (i_ptr.origin == ORIGIN_DROP_PIT)
					|| (i_ptr.origin == ORIGIN_DROP_VAULT)
					|| (i_ptr.origin == ORIGIN_DROP_SUMMON)
					|| (i_ptr.origin == ORIGIN_DROP_SPECIAL)
					|| (i_ptr.origin == ORIGIN_DROP_BREED)
					|| (i_ptr.origin == ORIGIN_DROP_POLY)
					|| (i_ptr.origin == ORIGIN_DROP_WIZARD)))
				dump_item++;
		}

		/* Change origin if monster is invisible, unless we're in stats mode */
		if (!visible && !stats)
			i_ptr.origin = ORIGIN_DROP_UNKNOWN;

		drop_near(cave, i_ptr, 0, y, x, true);
	}

	/* Forget objects */
	m_ptr.hold_o_idx = 0;

	/* Take note of any dropped treasure */
	if (visible && (dump_item || dump_gold))
		lore_treasure(m_idx, dump_item, dump_gold);

	/* Update monster list window */
	p_ptr.redraw |= PR_MONLIST;

	/* Only process "Quest Monsters" */
	if (!rf_has(r_ptr.flags, RF_QUESTOR)) return;

	/* Mark quests as complete */
	for (i = 0; i < MAX_Q_IDX; i++)	{
		/* Note completed quests */
		if (q_list[i].level == r_ptr.level) q_list[i].level = 0;

		/* Count incomplete quests */
		if (q_list[i].level) total++;
	}

	/* Build magical stairs */
	build_quest_stairs(y, x);

	/* Nothing left, game over... */
	if (total == 0) {
		p_ptr.total_winner = true;
		p_ptr.redraw |= (PR_TITLE);
		msg("*** CONGRATULATIONS ***");
		msg("You have won the game!");
		msg("You may retire (commit suicide) when you are ready.");
	}
}


/**
 * Decrease a monster's hit points and handle monster death.
 *
 * We return true if the monster has been killed (and deleted).
 *
 * We announce monster death (using an optional "death message"
 * if given, and a otherwise a generic killed/destroyed message).
 *
 * Only "physical attacks" can induce the "You have slain" message.
 * Missile and Spell attacks will induce the "dies" message, or
 * various "specialized" messages.  Note that "You have destroyed"
 * and "is destroyed" are synonyms for "You have slain" and "dies".
 *
 * Invisible monsters induce a special "You have killed it." message.
 *
 * Hack -- we "delay" fear messages by passing around a "fear" flag.
 *
 * Consider decreasing monster experience over time, say, by using
 * "(m_exp * m_lev * (m_lev)) / (p_lev * (m_lev + n_killed))" instead
 * of simply "(m_exp * m_lev) / (p_lev)", to make the first monster
 * worth more than subsequent monsters.  This would also need to
 * induce changes in the monster recall code.  XXX XXX XXX
 **/
bool mon_take_hit(int m_idx, int dam, bool *fear, const char *note)
{
	monster_type *m_ptr = cave_monster(cave, m_idx);

	monster_race *r_ptr = &r_info[m_ptr.r_idx];

	monster_lore *l_ptr = &l_list[m_ptr.r_idx];

	s32b div, new_exp, new_exp_frac;


	/* Redraw (later) if needed */
	if (p_ptr.health_who == m_idx) p_ptr.redraw |= (PR_HEALTH);

	/* Wake it up */
	mon_clear_timed(m_idx, MON_TMD_SLEEP, MON_TMD_FLG_NOMESSAGE, false);

	/* Become aware of its presence */
	if (m_ptr.unaware)
		become_aware(m_idx);

	/* Hurt it */
	m_ptr.hp -= dam;

	/* It is dead now */
	if (m_ptr.hp < 0)
	{
		char m_name[80];
		char buf[80];

		/* Assume normal death sound */
		int soundfx = MSG_KILL;

		/* Play a special sound if the monster was unique */
		if (rf_has(r_ptr.flags, RF_UNIQUE))
		{
			if (r_ptr.base == lookup_monster_base("Morgoth"))
				soundfx = MSG_KILL_KING;
			else
				soundfx = MSG_KILL_UNIQUE;
		}

		/* Extract monster name */
		monster_desc(m_name, sizeof(m_name), m_ptr, 0);

		/* Death by Missile/Spell attack */
		if (note)
		{
			/* Hack -- allow message suppression */
			if (strlen(note) <= 1)
			{
				/* Be silent */
			}

			else msgt(soundfx, "%^s%s", m_name, note);
		}

		/* Death by physical attack -- invisible monster */
		else if (!m_ptr.ml)
		{
			msgt(soundfx, "You have killed %s.", m_name);
		}

		/* Death by Physical attack -- non-living monster */
		else if (monster_is_unusual(r_ptr))
		{
			msgt(soundfx, "You have destroyed %s.", m_name);
		}

		/* Death by Physical attack -- living monster */
		else
		{
			msgt(soundfx, "You have slain %s.", m_name);
		}

		/* Player level */
		div = p_ptr.lev;

		/* Give some experience for the kill */
		new_exp = ((long)r_ptr.mexp * r_ptr.level) / div;

		/* Handle fractional experience */
		new_exp_frac = ((((long)r_ptr.mexp * r_ptr.level) % div)
		                * 0x10000L / div) + p_ptr.exp_frac;

		/* Keep track of experience */
		if (new_exp_frac >= 0x10000L)
		{
			new_exp++;
			p_ptr.exp_frac = (u16b)(new_exp_frac - 0x10000L);
		}
		else
		{
			p_ptr.exp_frac = (u16b)new_exp_frac;
		}

		/* When the player kills a Unique, it stays dead */
		if (rf_has(r_ptr.flags, RF_UNIQUE))
		{
			char unique_name[80];
			r_ptr.max_num = 0;

			/* This gets the correct name if we slay an invisible unique and don't have See Invisible. */
			monster_desc(unique_name, sizeof(unique_name), m_ptr, MDESC_SHOW | MDESC_IND2);

			/* Log the slaying of a unique */
			strnfmt(buf, sizeof(buf), "Killed %s", unique_name);
			history_add(buf, HISTORY_SLAY_UNIQUE, 0);
		}

		/* Gain experience */
		player_exp_gain(p_ptr, new_exp);

		/* Generate treasure */
		monster_death(m_idx, false);

		/* Recall even invisible uniques or winners */
		if (m_ptr.ml || rf_has(r_ptr.flags, RF_UNIQUE))
		{
			/* Count kills this life */
			if (l_ptr.pkills < MAX_SHORT) l_ptr.pkills++;

			/* Count kills in all lives */
			if (l_ptr.tkills < MAX_SHORT) l_ptr.tkills++;

			/* Hack -- Auto-recall */
			monster_race_track(m_ptr.r_idx);
		}

		/* Delete the monster */
		delete_monster_idx(m_idx);

		/* Not afraid */
		(*fear) = false;

		/* Monster is dead */
		return (true);
	}


	/* Mega-Hack -- Pain cancels fear */
	if (!(*fear) && m_ptr.m_timed[MON_TMD_FEAR] && (dam > 0))
	{
		int tmp = randint1(dam);

		/* Cure a little fear */
		if (tmp < m_ptr.m_timed[MON_TMD_FEAR])
			/* Reduce fear */
			mon_dec_timed(m_idx, MON_TMD_FEAR, tmp, MON_TMD_FLG_NOMESSAGE,
				false);

		/* Cure all the fear */
		else
		{
			/* Cure fear */
			mon_clear_timed(m_idx, MON_TMD_FEAR, MON_TMD_FLG_NOMESSAGE, false);

			/* No more fear */
			(*fear) = false;
		}
	}

	/* Sometimes a monster gets scared by damage */
	if (!m_ptr.m_timed[MON_TMD_FEAR] && !rf_has(r_ptr.flags, RF_NO_FEAR) && dam > 0)
	{
		int percentage;

		/* Percentage of fully healthy */
		percentage = (100L * m_ptr.hp) / m_ptr.maxhp;

		/*
		 * Run (sometimes) if at 10% or less of max hit points,
		 * or (usually) when hit for half its current hit points
		 */
		if ((randint1(10) >= percentage) ||
		    ((dam >= m_ptr.hp) && (randint0(100) < 80)))
		{
			int timer = randint1(10) + (((dam >= m_ptr.hp) && (percentage > 7)) ?
	                   20 : ((11 - percentage) * 5));

			/* Hack -- note fear */
			(*fear) = true;

			mon_inc_timed(m_idx, MON_TMD_FEAR, timer,
					MON_TMD_FLG_NOMESSAGE | MON_TMD_FLG_NOFAIL, false);
		}
	}


	/* Not dead yet */
	return (false);
}

