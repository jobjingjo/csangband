/*
 * File: cmd-obj.c
 * Purpose: Handle objects in various ways
 *
 * Copyright (c) 1997 Ben Harrison, James E. Wilson, Robert A. Koeneke
 * Copyright (c) 2007-9 Andrew Sidwell, Chris Carr, Ed Graham, Erik Osheim
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
#include "cmds.h"
#include "effects.h"
#include "game-cmd.h"
#include "object/inventory.h"
#include "object/tvalsval.h"
#include "spells.h"
#include "target.h"

/*** Utility bits and bobs ***/




typedef enum {
	ART_TAG_NONE,
	ART_TAG_NAME,
	ART_TAG_KIND,
	ART_TAG_VERB,
	ART_TAG_VERB_IS
} art_tag_t;

static art_tag_t art_tag_lookup(const char *tag)
{
	if (strncmp(tag, "name", 4) == 0)
		return ART_TAG_NAME;
	else if (strncmp(tag, "kind", 4) == 0)
		return ART_TAG_KIND;
	else if (strncmp(tag, "s", 1) == 0)
		return ART_TAG_VERB;
	else if (strncmp(tag, "is", 2) == 0)
		return ART_TAG_VERB_IS;
	else
		return ART_TAG_NONE;
}

/*
 * Print an artifact activation message.
 *
 * In order to support randarts, with scrambled names, we re-write
 * the message to replace instances of {name} with the artifact name
 * and instances of {kind} with the type of object.
 *
 * This code deals with plural and singular forms of verbs correctly
 * when encountering {s}, though in fact both names and kinds are
 * always singular in the current code (gloves are "Set of" and boots
 * are "Pair of")
 */
static void activation_message(object_type *o_ptr, const char *message)
{
	char buf[1024] = "\0";
	const char *next;
	const char *s;
	const char *tag;
	const char *in_cursor;
	size_t end = 0;

	in_cursor = message;

	next = strchr(in_cursor, '{');
	while (next)
	{
		/* Copy the text leading up to this { */
		strnfcat(buf, 1024, &end, "%.*s", next - in_cursor, in_cursor); 

		s = next + 1;
		while (*s && isalpha((unsigned char) *s)) s++;

		if (*s == '}')		/* Valid tag */
		{
			tag = next + 1; /* Start the tag after the { */
			in_cursor = s + 1;

			switch(art_tag_lookup(tag))
			{
			case ART_TAG_NAME:
				end += object_desc(buf, 1024, o_ptr, ODESC_PREFIX | ODESC_BASE); 
				break;
			case ART_TAG_KIND:
				object_kind_name(&buf[end], 1024-end, o_ptr.kind, true);
				end += strlen(&buf[end]);
				break;
			case ART_TAG_VERB:
				strnfcat(buf, 1024, &end, "s");
				break;
			case ART_TAG_VERB_IS:
				if((end > 2) && (buf[end-2] == 's'))
					strnfcat(buf, 1024, &end, "are");
				else
					strnfcat(buf, 1024, &end, "is");
			default:
				break;
			}
		}
		else    /* An invalid tag, skip it */
		{
			in_cursor = next + 1;
		}

		next = strchr(in_cursor, '{');
	}
	strnfcat(buf, 1024, &end, in_cursor);

	msg("%s", buf);
}

