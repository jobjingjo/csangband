/*
 * File: player/calcs.c
 * Purpose: Player status calculation, signalling ui events based on status
 *          changes.
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
#include "cave.h"
#include "files.h"
#include "game-event.h"
#include "monster/mon-msg.h"
#include "monster/mon-util.h"
#include "object/tvalsval.h"
#include "object/pval.h"
#include "spells.h"
#include "squelch.h"

/*
 * Computes weight remaining before burdened.
 */
int weight_remaining(void)
{
	int i;

	/* Weight limit based only on strength */
	i = 60 * adj_str_wgt[p_ptr.state.stat_ind[A_STR]] - p_ptr.total_weight - 1;

	/* Return the result */
	return (i);
}

/*
 * Handle "p_ptr.update" and "p_ptr.redraw"
 */
void handle_stuff(struct player *p)
{
	if (p.update) update_stuff(p);
	if (p.redraw) redraw_stuff(p);
}

