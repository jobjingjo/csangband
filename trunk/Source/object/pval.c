/*
 * File: pval.c
 * Purpose: Functions for handling object pvals.
 *
 * Copyright (c) 2011 Chris Carr
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
#include "object/pval.h"
#include "object/tvalsval.h"

/**
 * Obtain the pval_flags for an item which are known to the player
 */
void object_pval_flags_known(const object_type *o_ptr,
	bitflag flags[MAX_PVALS][OF_SIZE])
{
    int i, flag;

    object_pval_flags(o_ptr, flags);

    for (i = 0; i < MAX_PVALS; i++)
        of_inter(flags[i], o_ptr.known_flags);

	/* Kind and ego pval_flags may have shifted pvals so we iterate */
	if (object_flavor_is_aware(o_ptr))
	    for (i = 0; i < MAX_PVALS; i++)
			for (flag = of_next(o_ptr.kind.pval_flags[i], FLAG_START);
					flag != FLAG_END; flag = of_next(o_ptr.kind.pval_flags[i],
					flag + 1))
				of_on(flags[which_pval(o_ptr, flag)], flag);

	if (o_ptr.ego && easy_know(o_ptr))
	    for (i = 0; i < MAX_PVALS; i++)
			for (flag = of_next(o_ptr.ego.pval_flags[i], FLAG_START);
					flag != FLAG_END; flag = of_next(o_ptr.ego.pval_flags[i],
					flag + 1))
				of_on(flags[which_pval(o_ptr, flag)], flag);
}


