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


/*
 * Mega-Hack -- Attempt to create one of the "Special Objects".
 *
 * We are only called from "make_object()"
 *
 * Note -- see "make_artifact()" and "apply_magic()".
 *
 * We *prefer* to create the special artifacts in order, but this is
 * normally outweighed by the "rarity" rolls for those artifacts.
 */
static bool make_artifact_special(object_type *o_ptr, int level)
{
	int i;
	object_kind *kind;

	/* No artifacts, do nothing */
	if (OPT(birth_no_artifacts)) return false;

	/* No artifacts in the town */
	if (!p_ptr.depth) return false;

	/* Check the special artifacts */
	for (i = 0; i < ART_MIN_NORMAL; ++i) {
		artifact_type *a_ptr = &a_info[i];

		/* Skip "empty" artifacts */
		if (!a_ptr.name) continue;

		/* Cannot make an artifact twice */
		if (a_ptr.created) continue;

		/* Enforce minimum "depth" (loosely) */
		if (a_ptr.alloc_min > p_ptr.depth) {
			/* Get the "out-of-depth factor" */
			int d = (a_ptr.alloc_min - p_ptr.depth) * 2;

			/* Roll for out-of-depth creation */
			if (randint0(d) != 0) continue;
		}

		/* Enforce maximum depth (strictly) */
		if (a_ptr.alloc_max < p_ptr.depth) continue;

		/* Artifact "rarity roll" */
		if (randint1(100) > a_ptr.alloc_prob) continue;

		/* Find the base object */
		kind = lookup_kind(a_ptr.tval, a_ptr.sval);

		/* Make sure the kind was found */
		if (!kind) continue;

		/* Enforce minimum "object" level (loosely) */
		if (kind.level > level) {
			/* Get the "out-of-depth factor" */
			int d = (kind.level - level) * 5;

			/* Roll for out-of-depth creation */
			if (randint0(d) != 0) continue;
		}

		/* Assign the template */
		object_prep(o_ptr, kind, a_ptr.alloc_min, RANDOMISE);

		/* Mark the item as an artifact */
		o_ptr.artifact = a_ptr;

		/* Copy across all the data from the artifact struct */
		copy_artifact_data(o_ptr, a_ptr);

		/* Mark the artifact as "created" */
		a_ptr.created = 1;

		/* Success */
		return true;
	}

	/* Failure */
	return false;
}



/*** Generate a random object ***/


/*
 * Attempt to make an object (normal or good/great)
 *
 * This routine plays nasty games to generate the "special artifacts".
 * We assume that the given object has been "wiped". You can optionally
 * receive the object's value in value if you pass a non-null pointer.
 *
 * Returns the whether or not creation worked.
 */
bool make_object(struct cave *c, object_type *j_ptr, int lev, bool good, bool great, s32b *value)
{
	int base;
	object_kind *kind;

	/* Try to make a special artifact */
	if (one_in_(good ? 10 : 1000)) {
		if (make_artifact_special(j_ptr, lev)) {
			if (value) *value = object_value_real(j_ptr, 1, false, true);
			return true;
		}

		/* If we failed to make an artifact, the player gets a great item */
		good = great = true;
	}

	/* Base level for the object */
	base = (good ? (lev + 10) : lev);

	/* Get the object, prep it and apply magic */
	kind = get_obj_num(base, good || great);
	if (!kind) return false;
	object_prep(j_ptr, kind, lev, RANDOMISE);
	apply_magic(j_ptr, lev, true, good, great);

	/* Generate multiple items */
	if (kind.gen_mult_prob >= randint1(100))
		j_ptr.number = randcalc(kind.stack_size, lev, RANDOMISE);

	if(value) *value = object_value_real(j_ptr, j_ptr.number, false, true);

	/* Return value, increased for uncursed out-of-depth objects */
	if (!cursed_p(j_ptr.flags) && (kind.alloc_min > c.depth)) {
		if (value) *value = (kind.alloc_min - c.depth) * (*value / 5);
	}

	return true;
}


/*** Make a gold item ***/

/* The largest possible average gold drop at max depth with biggest spread */
#define MAX_GOLD_DROP     (3 * MAX_DEPTH + 30)

/*
 * Make a money object
 */
void make_gold(object_type *j_ptr, int lev, int coin_type)
{
	int sval;

	/* This average is 20 at dlev0, 105 at dlev40, 220 at dlev100. */
	/* Follows the formula: y=2x+20 */
	s32b avg = 2*lev + 20;
	s32b spread = lev + 10;
	s32b value = rand_spread(avg, spread);

	/* Increase the range to infinite, moving the average to 110% */
	while (one_in_(100) && value * 10 <= MAX_SHORT)
		value *= 10;

	/* Pick a treasure variety scaled by level, or force a type */
	if (coin_type != SV_GOLD_ANY)
		sval = coin_type;
	else
		sval = (((value * 100) / MAX_GOLD_DROP) * SV_GOLD_MAX) / 100;

	/* Do not create illegal treasure types */
	if (sval >= SV_GOLD_MAX) sval = SV_GOLD_MAX - 1;

	/* Prepare a gold object */
	object_prep(j_ptr, lookup_kind(TV_GOLD, sval), lev, RANDOMISE);

	/* If we're playing with no_selling, increase the value */
	if (OPT(birth_no_selling) && p_ptr.depth)
		value = value * MIN(5, p_ptr.depth);

	/* Cap gold at max short (or alternatively make pvals s32b) */
	if (value > MAX_SHORT)
		value = MAX_SHORT;

	j_ptr.pval[DEFAULT_PVAL] = value;
}
