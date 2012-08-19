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

/*
 * Check to see if the player can use a rod/wand/staff/activatable object.
 */
static int check_devices(object_type *o_ptr)
{
	int fail;
	const char *action;
	const char *what = null;

	/* Get the right string */
	switch (o_ptr.tval)
	{
		case TV_ROD:   action = "zap the rod";   break;
		case TV_WAND:  action = "use the wand";  what = "wand";  break;
		case TV_STAFF: action = "use the staff"; what = "staff"; break;
		default:       action = "activate it";  break;
	}

	/* Figure out how hard the item is to use */
	fail = get_use_device_chance(o_ptr);

	/* Roll for usage */
	if (randint1(1000) < fail)
	{
		flush();
		msg("You failed to %s properly.", action);
		return false;
	}

	/* Notice empty staffs */
	if (what && o_ptr.pval[DEFAULT_PVAL] <= 0)
	{
		flush();
		msg("The %s has no charges left.", what);
		o_ptr.ident |= (IDENT_EMPTY);
		return false;
	}

	return true;
}


/*
 * Return the chance of an effect beaming, given a tval.
 */
static int beam_chance(int tval)
{
	switch (tval)
	{
		case TV_WAND: return 20;
		case TV_ROD:  return 10;
	}

	return 0;
}


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






/*** Taking off/putting on ***/




/*
 * Wield or wear a single item from the pack or floor
 */
void wield_item(object_type *o_ptr, int item, int slot)
{
	object_type object_type_body;
	object_type *i_ptr = &object_type_body;

	const char *fmt;
	char o_name[80];

	bool combined_ammo = false;
	bool track_wielded_item = false;
	int num = 1;

	/* If we are stacking ammo in the quiver */
	if (obj_is_ammo(o_ptr))
	{
		num = o_ptr.number;
		combined_ammo = object_similar(o_ptr, &p_ptr.inventory[slot],
			OSTACK_QUIVER);
	}

	/* Take a turn */
	p_ptr.energy_use = 100;

	/* Obtain local object */
	object_copy(i_ptr, o_ptr);

	/* Modify quantity */
	i_ptr.number = num;

	/* Update object_idx if necessary, once object is in slot */
	if (tracked_object_is(item))
	{
		track_wielded_item = true;
	}

	/* Decrease the item (from the pack) */
	if (item >= 0)
	{
		inven_item_increase(item, -num);
		inven_item_optimize(item);
	}

	/* Decrease the item (from the floor) */
	else
	{
		floor_item_increase(0 - item, -num);
		floor_item_optimize(0 - item);
	}

	/* Get the wield slot */
	o_ptr = &p_ptr.inventory[slot];

	if (combined_ammo)
	{
		/* Add the new ammo to the already-quiver-ed ammo */
		object_absorb(o_ptr, i_ptr);
	}
	else 
	{
		/* Take off existing item */
		if (o_ptr.kind)
			(void)inven_takeoff(slot, 255);

		/* If we are wielding ammo we may need to "open" the slot by shifting
		 * later ammo up the quiver; this is because we already called the
		 * inven_item_optimize() function. */
		if (slot >= QUIVER_START)
			open_quiver_slot(slot);
	
		/* Wear the new stuff */
		object_copy(o_ptr, i_ptr);

		/* Increment the equip counter by hand */
		p_ptr.equip_cnt++;
	}

	/* Increase the weight */
	p_ptr.total_weight += i_ptr.weight * num;

	/* Track object if necessary */
	if (track_wielded_item)
	{
		track_object(slot);
	}

	/* Do any ID-on-wield */
	object_notice_on_wield(o_ptr);

	/* Where is the item now */
	if (slot == INVEN_WIELD)
		fmt = "You are wielding %s (%c).";
	else if (slot == INVEN_BOW)
		fmt = "You are shooting with %s (%c).";
	else if (slot == INVEN_LIGHT)
		fmt = "Your light source is %s (%c).";
	else if (combined_ammo)
		fmt = "You combine %s in your quiver (%c).";
	else if (slot >= QUIVER_START && slot < QUIVER_END)
		fmt = "You add %s to your quiver (%c).";
	else
		fmt = "You are wearing %s (%c).";

	/* Describe the result */
	object_desc(o_name, sizeof(o_name), o_ptr, ODESC_PREFIX | ODESC_FULL);

	/* Message */
	msgt(MSG_WIELD, fmt, o_name, index_to_label(slot));

	/* Cursed! */
	if (cursed_p(o_ptr.flags))
	{
		/* Warn the player */
		msgt(MSG_CURSED, "Oops! It feels deathly cold!");

		/* Sense the object */
		object_notice_curses(o_ptr);
	}

	/* Save quiver size */
	save_quiver_size(p_ptr);

	/* See if we have to overflow the pack */
	pack_overflow();

	/* Recalculate bonuses, torch, mana */
	p_ptr.notice |= PN_SORT_QUIVER;
	p_ptr.update |= (PU_BONUS | PU_TORCH | PU_MANA);
	p_ptr.redraw |= (PR_INVEN | PR_EQUIP);
}


