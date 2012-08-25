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









