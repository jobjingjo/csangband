/*
 * File: identify.c
 * Purpose: Object identification and knowledge routines
 *
 * Copyright (c) 1997 Ben Harrison, James E. Wilson, Robert A. Koeneke
 * Copyright (c) 2009 Brian Bull
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
#include "game-event.h"
#include "history.h"
#include "object/slays.h"
#include "object/tvalsval.h"
#include "object/pval.h"
#include "spells.h"
#include "squelch.h"

/*** Knowledge accessor functions ***/

/**
 * \returns whether the object is known to be an artifact
 */
bool object_is_known_artifact(const object_type *o_ptr)
{
	return (o_ptr.ident & IDENT_INDESTRUCT) ||
			(o_ptr.artifact && object_was_sensed(o_ptr));
}

/**
 * \returns whether the object is known to be cursed
 */
bool object_is_known_cursed(const object_type *o_ptr)
{
	bitflag f[OF_SIZE], f2[OF_SIZE];

	object_flags_known(o_ptr, f);

	/* Gather whatever curse flags there are to know */
	create_mask(f2, false, OFT_CURSE, OFT_MAX);

	return of_is_inter(f, f2);
}

/**
 * \returns whether the object is known to be blessed
 */
bool object_is_known_blessed(const object_type *o_ptr)
{
	bitflag f[OF_SIZE];

	object_flags_known(o_ptr, f);

	return (of_has(f, OF_BLESSED)) ? true : false;
}







/**
 * Check whether an object has IDENT_KNOWN but should not
 */
bool object_is_not_known_consistently(const object_type *o_ptr)
{
	if (easy_know(o_ptr))
		return false;
	if (!(o_ptr.ident & IDENT_KNOWN))
		return true;
	if ((o_ptr.ident & IDENTS_SET_BY_IDENTIFY) != IDENTS_SET_BY_IDENTIFY)
		return true;
	if (o_ptr.ident & IDENT_EMPTY)
		return true;
	else if (o_ptr.artifact &&
			!(o_ptr.artifact.seen || o_ptr.artifact.everseen))
		return true;

	if (!of_is_full(o_ptr.known_flags))
		return true;

	return false;
}


/**
 * Notice that an object is indestructible.
 */
void object_notice_indestructible(object_type *o_ptr)
{
	if (object_add_ident_flags(o_ptr, IDENT_INDESTRUCT))
		object_check_for_ident(o_ptr);
}












