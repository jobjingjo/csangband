/*
 * File: obj-ui.c
 * Purpose: Mainly object descriptions and generic UI functions
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
#include "button.h"
#include "tvalsval.h"
#include "cmds.h"


/*
 * Display the equipment.  Builds a list of objects and passes them
 * off to show_obj_list() for display.  Mode flags documented in
 * object.h
 */
void show_equip(olist_detail_t mode)
{
	int i, last_slot = 0;

	object_type *o_ptr;

   int num_obj = 0;
   char labels[50][80];
   object_type *objects[50];

	char tmp_val[80];

   bool in_term = (mode & OLIST_WINDOW) ? true : false;

	/* Find the last equipment slot to display */
	for (i = INVEN_WIELD; i < ALL_INVEN_TOTAL; i++)
	{
		o_ptr = &p_ptr.inventory[i];
		if (i < INVEN_TOTAL || o_ptr.kind) last_slot = i;
	}

	/* Build the object list */
	for (i = INVEN_WIELD; i <= last_slot; i++)
	{
		o_ptr = &p_ptr.inventory[i];

		/* May need a blank line to separate the quiver */
		if (i == INVEN_TOTAL)
		{
			int j;
			bool need_spacer = false;
			
			/* Scan the rest of the items for acceptable entries */
			for (j = i; j < last_slot; j++)
			{
				o_ptr = &p_ptr.inventory[j];
				if (item_tester_okay(o_ptr)) need_spacer = true;
			}

			/* Add a spacer between equipment and quiver */
			if (num_obj > 0 && need_spacer)
			{
				my_strcpy(labels[num_obj], "", sizeof(labels[num_obj]));
				objects[num_obj] = null;
				num_obj++;
			}

			continue;
		}

		/* Acceptable items get a label */
		if (item_tester_okay(o_ptr))
			strnfmt(labels[num_obj], sizeof(labels[num_obj]), "%c) ", index_to_label(i));

		/* Unacceptable items are still displayed in term windows */
		else if (in_term)
			my_strcpy(labels[num_obj], "   ", sizeof(labels[num_obj]));

		/* Unacceptable items are skipped in the main window */
		else continue;

		/* Show full slot labels */
		strnfmt(tmp_val, sizeof(tmp_val), "%-14s: ", mention_use(i));
		my_strcat(labels[num_obj], tmp_val, sizeof(labels[num_obj]));

		/* Save the object */
		objects[num_obj] = o_ptr;
		num_obj++;
	}

	/* Display the object list */
	show_obj_list(num_obj, 0, labels, objects, mode);
}


/*
 * Display the floor.  Builds a list of objects and passes them
 * off to show_obj_list() for display.  Mode flags documented in
 * object.h
 */
void show_floor(const int *floor_list, int floor_num, olist_detail_t mode)
{
	int i;

	object_type *o_ptr;

   int num_obj = 0;
   char labels[50][80];
   object_type *objects[50];

	if (floor_num > MAX_FLOOR_STACK) floor_num = MAX_FLOOR_STACK;

	/* Build the object list */
	for (i = 0; i < floor_num; i++)
	{
		o_ptr = object_byid(floor_list[i]);

		/* Tester always skips gold. When gold should be displayed,
		 * only test items that are not gold.
		 */
		if ((o_ptr.tval != TV_GOLD || !(mode & OLIST_GOLD)) &&
		    !item_tester_okay(o_ptr))
			continue;

		strnfmt(labels[num_obj], sizeof(labels[num_obj]),
		        "%c) ", index_to_label(i));

		/* Save the object */
		objects[num_obj] = o_ptr;
		num_obj++;
	}

	/* Display the object list */
	show_obj_list(num_obj, 0, labels, objects, mode);
}


/*
 * Verify the choice of an item.
 *
 * The item can be negative to mean "item on floor".
 */
bool verify_item(const char *prompt, int item)
{
	char o_name[80];

	char out_val[160];

	object_type *o_ptr;

	/* Inventory */
	if (item >= 0)
	{
		o_ptr = &p_ptr.inventory[item];
	}

	/* Floor */
	else
	{
		o_ptr = object_byid(0 - item);
	}

	/* Describe */
	object_desc(o_name, sizeof(o_name), o_ptr, ODESC_PREFIX | ODESC_FULL);

	/* Prompt */
	strnfmt(out_val, sizeof(out_val), "%s %s? ", prompt, o_name);

	/* Query */
	return (get_check(out_val));
}


/*
 * Hack -- allow user to "prevent" certain choices.
 *
 * The item can be negative to mean "item on floor".
 */
static bool get_item_allow(int item, unsigned char ch, bool is_harmless)
{
	object_type *o_ptr;
	char verify_inscrip[] = "!*";

	unsigned n;

	/* Inventory or floor */
	if (item >= 0)
		o_ptr = &p_ptr.inventory[item];
	else
		o_ptr = object_byid(0 - item);

	/* Check for a "prevention" inscription */
	verify_inscrip[1] = ch;

	/* Find both sets of inscriptions, add together, and prompt that number of times */
	n = check_for_inscrip(o_ptr, verify_inscrip);

	if (!is_harmless)
		n += check_for_inscrip(o_ptr, "!*");

	while (n--)
	{
		if (!verify_item("Really try", item))
			return (false);
	}

	/* Allow it */
	return (true);
}



/*
 * Find the "first" inventory object with the given "tag".
 *
 * A "tag" is a char "n" appearing as "@n" anywhere in the
 * inscription of an object.
 *
 * Also, the tag "@xn" will work as well, where "n" is a tag-char,
 * and "x" is the action that tag will work for.
 */
static int get_tag(int *cp, char tag, cmd_code cmd, bool quiver_tags)
{
	int i;
	const char *s;

	/* (f)ire is handled differently from all others, due to the quiver */
	if (quiver_tags)
	{
		i = QUIVER_START + tag - '0';
		if (p_ptr.inventory[i].kind)
		{
			*cp = i;
			return (true);
		}
		return (false);
	}

	/* Check every object */
	for (i = 0; i < ALL_INVEN_TOTAL; ++i)
	{
		object_type *o_ptr = &p_ptr.inventory[i];

		/* Skip non-objects */
		if (!o_ptr.kind) continue;

		/* Skip empty inscriptions */
		if (!o_ptr.note) continue;

		/* Find a '@' */
		s = strchr(quark_str(o_ptr.note), '@');

		/* Process all tags */
		while (s)
		{
			/* Check the normal tags */
			if (s[1] == tag)
			{
				/* Save the actual inventory ID */
				*cp = i;

				/* Success */
				return (true);
			}

			/* Check the special tags */
			if ((cmd_lookup(s[1]) == cmd) && (s[2] == tag))
			{
				/* Save the actual inventory ID */
				*cp = i;

				/* Success */
				return (true);
			}

			/* Find another '@' */
			s = strchr(s + 1, '@');
		}
	}

	/* No such tag */
	return (false);
}



