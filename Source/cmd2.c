/*
 * File: cmd2.c
 * Purpose: Chest and door opening/closing, disarming, running, resting, &c.
 *
 * Copyright (c) 1997 Ben Harrison, James E. Wilson, Robert A. Koeneke
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
#include "cmds.h"
#include "files.h"
#include "game-cmd.h"
#include "game-event.h"
#include "generate.h"
#include "monster/mon-timed.h"
#include "monster/mon-util.h"
#include "monster/monster.h"
#include "object/tvalsval.h"
#include "spells.h"
#include "squelch.h"
#include "trap.h"

/*
 * Determine if a grid contains a chest
 */
static s16b chest_check(int y, int x)
{
	s16b this_o_idx, next_o_idx = 0;


	/* Scan all objects in the grid */
	for (this_o_idx = cave.o_idx[y][x]; this_o_idx; this_o_idx = next_o_idx)
	{
		object_type *o_ptr;

		/* Get the object */
		o_ptr = object_byid(this_o_idx);

		/* Get the next object */
		next_o_idx = o_ptr.next_o_idx;

		/* Skip unknown chests XXX XXX */
		/* if (!o_ptr.marked) continue; */

		/* Check for chest */
		if (o_ptr.tval == TV_CHEST) return (this_o_idx);
	}

	/* No chest */
	return (0);
}


/*
 * Allocate objects upon opening a chest
 *
 * Disperse treasures from the given chest, centered at (x,y).
 *
 * Small chests often contain "gold", while Large chests always contain
 * items.  Wooden chests contain 2 items, Iron chests contain 4 items,
 * and Steel chests contain 6 items.  The "value" of the items in a
 * chest is based on the level on which the chest is generated.
 */
static void chest_death(int y, int x, s16b o_idx)
{
	int number, value;

	bool tiny;

	object_type *o_ptr;

	object_type *i_ptr;
	object_type object_type_body;


	/* Get the chest */
	o_ptr = object_byid(o_idx);

	/* Small chests often hold "gold" */
	tiny = (o_ptr.sval < SV_CHEST_MIN_LARGE);

	/* Determine how much to drop (see above) */
	number = (o_ptr.sval % SV_CHEST_MIN_LARGE) * 2;

	/* Zero pval means empty chest */
	if (!o_ptr.pval[DEFAULT_PVAL]) number = 0;

	/* Determine the "value" of the items */
	value = o_ptr.origin_depth - 10 + 2 * o_ptr.sval;
	if (value < 1)
		value = 1;

	/* Drop some objects (non-chests) */
	for (; number > 0; --number)
	{
		/* Get local object */
		i_ptr = &object_type_body;

		/* Wipe the object */
		object_wipe(i_ptr);

		/* Small chests often drop gold */
		if (tiny && (randint0(100) < 75))
			make_gold(i_ptr, value, SV_GOLD_ANY);

		/* Otherwise drop an item, as long as it isn't a chest */
		else {
			if (!make_object(cave, i_ptr, value, false, false, null)) continue;
			if (i_ptr.tval == TV_CHEST) continue;
		}

		/* Record origin */
		i_ptr.origin = ORIGIN_CHEST;
		i_ptr.origin_depth = o_ptr.origin_depth;

		/* Drop it in the dungeon */
		drop_near(cave, i_ptr, 0, y, x, true);
	}

	/* Empty */
	o_ptr.pval[DEFAULT_PVAL] = 0;

	/* Known */
	object_notice_everything(o_ptr);
}


/*
 * Chests have traps too.
 *
 * Exploding chest destroys contents (and traps).
 * Note that the chest itself is never destroyed.
 */
static void chest_trap(int y, int x, s16b o_idx)
{
	int i, trap;

	object_type *o_ptr = object_byid(o_idx);


	/* Ignore disarmed chests */
	if (o_ptr.pval[DEFAULT_PVAL] <= 0) return;

	/* Obtain the traps */
	trap = chest_traps[o_ptr.pval[DEFAULT_PVAL]];

	/* Lose strength */
	if (trap & (CHEST_LOSE_STR))
	{
		msg("A small needle has pricked you!");
		take_hit(p_ptr, damroll(1, 4), "a poison needle");
		(void)do_dec_stat(A_STR, false);
	}

	/* Lose constitution */
	if (trap & (CHEST_LOSE_CON))
	{
		msg("A small needle has pricked you!");
		take_hit(p_ptr, damroll(1, 4), "a poison needle");
		(void)do_dec_stat(A_CON, false);
	}

	/* Poison */
	if (trap & (CHEST_POISON))
	{
		msg("A puff of green gas surrounds you!");
		(void)player_inc_timed(p_ptr, TMD_POISONED, 10 + randint1(20), true, true);
	}

	/* Paralyze */
	if (trap & (CHEST_PARALYZE))
	{
		msg("A puff of yellow gas surrounds you!");
		(void)player_inc_timed(p_ptr, TMD_PARALYZED, 10 + randint1(20), true, true);
	}

	/* Summon monsters */
	if (trap & (CHEST_SUMMON))
	{
		int num = 2 + randint1(3);
		msg("You are enveloped in a cloud of smoke!");
		sound(MSG_SUM_MONSTER);
		for (i = 0; i < num; i++)
		{
			(void)summon_specific(y, x, p_ptr.depth, 0, 1);
		}
	}

	/* Explode */
	if (trap & (CHEST_EXPLODE))
	{
		msg("There is a sudden explosion!");
		msg("Everything inside the chest is destroyed!");
		o_ptr.pval[DEFAULT_PVAL] = 0;
		take_hit(p_ptr, damroll(5, 8), "an exploding chest");
	}
}

/*
 * Return the number of doors/traps around (or under) the character.
 */
int count_feats(int *y, int *x, bool (*test)(struct cave *cave, int y, int x), bool under)
{
	int d;
	int xx, yy;
	int count = 0; /* Count how many matches */

	/* Check around (and under) the character */
	for (d = 0; d < 9; d++)
	{
		/* if not searching under player continue */
		if ((d == 8) && !under) continue;

		/* Extract adjacent (legal) location */
		yy = p_ptr.py + ddy_ddd[d];
		xx = p_ptr.px + ddx_ddd[d];

		/* Paranoia */
		if (!in_bounds_fully(yy, xx)) continue;

		/* Must have knowledge */
		if (!(cave.info[yy][xx] & (CAVE_MARK))) continue;

		/* Not looking for this feature */
		if (!((*test)(cave, yy, xx))) continue;

		/* Count it */
		++count;

		/* Remember the location of the last door found */
		*y = yy;
		*x = xx;
	}

	/* All done */
	return count;
}


/*
 * Return the number of chests around (or under) the character.
 * If requested, count only trapped chests.
 */
int count_chests(int *y, int *x, bool trapped)
{
	int d, count, o_idx;

	object_type *o_ptr;

	/* Count how many matches */
	count = 0;

	/* Check around (and under) the character */
	for (d = 0; d < 9; d++)
	{
		/* Extract adjacent (legal) location */
		int yy = p_ptr.py + ddy_ddd[d];
		int xx = p_ptr.px + ddx_ddd[d];

		/* No (visible) chest is there */
		if ((o_idx = chest_check(yy, xx)) == 0) continue;

		/* Grab the object */
		o_ptr = object_byid(o_idx);

		/* Already open */
		if (o_ptr.pval[DEFAULT_PVAL] == 0) continue;

		/* No (known) traps here */
		if (trapped &&
		    (!object_is_known(o_ptr) ||
		     (o_ptr.pval[DEFAULT_PVAL] < 0) ||
		     !chest_traps[o_ptr.pval[DEFAULT_PVAL]]))
		{
			continue;
		}

		/* Count it */
		++count;

		/* Remember the location of the last chest found */
		*y = yy;
		*x = xx;
	}

	/* All done */
	return count;
}


/*
 * Extract a "direction" which will move one step from the player location
 * towards the given "target" location (or "5" if no motion necessary).
 */
int coords_to_dir(int y, int x)
{
	return (motion_dir(p_ptr.py, p_ptr.px, y, x));
}





/*
 * Tunnel through wall.  Assumes valid location.
 *
 * Note that it is impossible to "extend" rooms past their
 * outer walls (which are actually part of the room).
 *
 * Attempting to do so will produce floor grids which are not part
 * of the room, and whose "illumination" status do not change with
 * the rest of the room.
 */
static bool twall(int y, int x)
{
	/* Paranoia -- Require a wall or door or some such */
	if (cave_floor_bold(y, x)) return (false);

	/* Sound */
	sound(MSG_DIG);

	/* Forget the wall */
	cave.info[y][x] &= ~(CAVE_MARK);

	/* Remove the feature */
	cave_set_feat(cave, y, x, FEAT_FLOOR);

	/* Update the visuals */
	p_ptr.update |= (PU_UPDATE_VIEW | PU_MONSTERS);

	/* Fully update the flow */
	p_ptr.update |= (PU_FORGET_FLOW | PU_UPDATE_FLOW);

	/* Result */
	return (true);
}


/*
 * Find the index of some "spikes", if possible.
 *
 * XXX XXX XXX Let user choose a pile of spikes, perhaps?
 */
static bool get_spike(int *ip)
{
	int i;

	/* Check every item in the pack */
	for (i = 0; i < INVEN_PACK; i++)
	{
		object_type *o_ptr = &p_ptr.inventory[i];

		/* Skip non-objects */
		if (!o_ptr.kind) continue;

		/* Check the "tval" code */
		if (o_ptr.tval == TV_SPIKE)
		{
			/* Save the spike index */
			(*ip) = i;

			/* Success */
			return (true);
		}
	}

	/* Oops */
	return (false);
}
