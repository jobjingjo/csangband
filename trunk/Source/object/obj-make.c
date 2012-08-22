/*
 * File: obj-make.c
 * Purpose: Object generation functions.
 *
 * Copyright (c) 1987-2007 Angband contributors
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
#include "object/tvalsval.h"
#include "object/pval.h"

/*** Make an artifact ***/

/**
 * Copy artifact data to a normal object, and set various slightly hacky
 * globals.
 */
void copy_artifact_data(object_type *o_ptr, const artifact_type *a_ptr)
{
	int i;

	/* Extract the data */
	for (i = 0; i < a_ptr.num_pvals; i++)
		if (a_ptr.pval[i]) {
			o_ptr.pval[i] = a_ptr.pval[i];
			of_copy(o_ptr.pval_flags[i], a_ptr.pval_flags[i]);
		}
	o_ptr.num_pvals = a_ptr.num_pvals;
	o_ptr.ac = a_ptr.ac;
	o_ptr.dd = a_ptr.dd;
	o_ptr.ds = a_ptr.ds;
	o_ptr.to_a = a_ptr.to_a;
	o_ptr.to_h = a_ptr.to_h;
	o_ptr.to_d = a_ptr.to_d;
	o_ptr.weight = a_ptr.weight;
	of_union(o_ptr.flags, a_ptr.flags);
}






/*** Generate a random object ***/



