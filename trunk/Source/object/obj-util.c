/*
 * File: object2.c
 * Purpose: Object list maintenance and other object utilities
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
#include "effects.h"
#include "game-cmd.h"
#include "generate.h"
#include "history.h"
#include "monster/mon-make.h"
#include "object/inventory.h"
#include "object/tvalsval.h"
#include "prefs.h"
#include "randname.h"
#include "spells.h"
#include "squelch.h"
#include "z-queue.h"

#ifdef ALLOW_BORG_GRAPHICS
extern void init_translate_visuals(void);
#endif /* ALLOW_BORG_GRAPHICS */

/*
 * \returns whether item o_ptr will fit in slot 'slot'
 */
bool slot_can_wield_item(int slot, const object_type *o_ptr)
{
	if (o_ptr.tval == TV_RING)
		return (slot == INVEN_LEFT || slot == INVEN_RIGHT) ? true : false;
	else if (obj_is_ammo(o_ptr))
		return (slot >= QUIVER_START && slot < QUIVER_END) ? true : false;
	else
		return (wield_slot(o_ptr) == slot) ? true : false;
}







/*
 * Deletes all objects at given location
 */
void delete_object(int y, int x)
{
	s16b this_o_idx, next_o_idx = 0;

	/* Paranoia */
	if (!in_bounds(y, x)) return;

	/* Scan all objects in the grid */
	for (this_o_idx = cave.o_idx[y][x]; this_o_idx; this_o_idx = next_o_idx) {
		object_type *o_ptr;

		/* Get the object */
		o_ptr = object_byid(this_o_idx);

		/* Get the next object */
		next_o_idx = o_ptr.next_o_idx;

		/* Preserve unseen artifacts */
		if (o_ptr.artifact && !object_was_sensed(o_ptr))
			o_ptr.artifact.created = false;

		/* Delete the mimicking monster if necessary */
		if (o_ptr.mimicking_m_idx) {
			monster_type *m_ptr;
			
			m_ptr = cave_monster(cave, o_ptr.mimicking_m_idx);
			
			/* Clear the mimicry */
			m_ptr.mimicked_o_idx = 0;
			
			delete_monster_idx(o_ptr.mimicking_m_idx);
		}

		/* Wipe the object */
		object_wipe(o_ptr);

		/* Count objects */
		o_cnt--;
	}

	/* Objects are gone */
	cave.o_idx[y][x] = 0;

	/* Visual update */
	cave_light_spot(cave, y, x);
}



/*
 * Move an object from index i1 to index i2 in the object list
 */
static void compact_objects_aux(int i1, int i2)
{
	int i;

	object_type *o_ptr;


	/* Do nothing */
	if (i1 == i2) return;


	/* Repair objects */
	for (i = 1; i < o_max; i++)
	{
		/* Get the object */
		o_ptr = object_byid(i);

		/* Skip "dead" objects */
		if (!o_ptr.kind) continue;

		/* Repair "next" pointers */
		if (o_ptr.next_o_idx == i1)
		{
			/* Repair */
			o_ptr.next_o_idx = i2;
		}
	}


	/* Get the object */
	o_ptr = object_byid(i1);


	/* Monster */
	if (o_ptr.held_m_idx)
	{
		monster_type *m_ptr;

		/* Get the monster */
		m_ptr = cave_monster(cave, o_ptr.held_m_idx);

		/* Repair monster */
		if (m_ptr.hold_o_idx == i1)
		{
			/* Repair */
			m_ptr.hold_o_idx = i2;
		}
	}

	/* Dungeon */
	else
	{
		int y, x;

		/* Get location */
		y = o_ptr.iy;
		x = o_ptr.ix;

		/* Repair grid */
		if (cave.o_idx[y][x] == i1)
		{
			/* Repair */
			cave.o_idx[y][x] = i2;
		}

		/* Mimic */
		if (o_ptr.mimicking_m_idx)
		{
			monster_type *m_ptr;

			/* Get the monster */
			m_ptr = cave_monster(cave, o_ptr.mimicking_m_idx);

			/* Repair monster */
			if (m_ptr.mimicked_o_idx == i1)
			{
				/* Repair */
				m_ptr.mimicked_o_idx = i2;
			}
		}
	}


	/* Hack -- move object */
	COPY(object_byid(i2), object_byid(i1), object_type);

	/* Hack -- wipe hole */
	object_wipe(o_ptr);
}


/*
 * Compact and reorder the object list
 *
 * This function can be very dangerous, use with caution!
 *
 * When compacting objects, we first destroy gold, on the basis that by the
 * time item compaction becomes an issue, the player really won't care.
 * We also nuke items marked as squelch.
 *
 * When compacting other objects, we base the saving throw on a combination of
 * object level, distance from player, and current "desperation".
 *
 * After compacting, we "reorder" the objects into a more compact order, and we
 * reset the allocation info, and the "live" array.
 */
void compact_objects(int size)
{
	int py = p_ptr.py;
	int px = p_ptr.px;

	int i, y, x, cnt;

	int cur_lev, cur_dis, chance;


	/* Reorder objects when not passed a size */
	if (!size)
	{
		/* Excise dead objects (backwards!) */
		for (i = o_max - 1; i >= 1; i--)
		{
			object_type *o_ptr = object_byid(i);
			if (o_ptr.kind) continue;

			/* Move last object into open hole */
			compact_objects_aux(o_max - 1, i);

			/* Compress "o_max" */
			o_max--;
		}

		return;
	}


	/* Message */
	msg("Compacting objects...");

	/*** Try destroying objects ***/

	/* First do gold */
	for (i = 1; (i < o_max) && (size); i++)
	{
		object_type *o_ptr = object_byid(i);

		/* Nuke gold or squelched items */
		if (o_ptr.tval == TV_GOLD || squelch_item_ok(o_ptr))
		{
			delete_object_idx(i);
			size--;
		}
	}


	/* Compact at least 'size' objects */
	for (cnt = 1; size; cnt++)
	{
		/* Get more vicious each iteration */
		cur_lev = 5 * cnt;

		/* Get closer each iteration */
		cur_dis = 5 * (20 - cnt);

		/* Examine the objects */
		for (i = 1; (i < o_max) && (size); i++)
		{
			object_type *o_ptr = object_byid(i);
			if (!o_ptr.kind) continue;

			/* Hack -- High level objects start out "immune" */
			if (o_ptr.kind.level > cur_lev && !o_ptr.kind.squelch)
				continue;

			/* Monster */
			if (o_ptr.held_m_idx)
			{
				monster_type *m_ptr;

				/* Get the monster */
				m_ptr = cave_monster(cave, o_ptr.held_m_idx);

				/* Get the location */
				y = m_ptr.fy;
				x = m_ptr.fx;

				/* Monsters protect their objects */
				if ((randint0(100) < 90) && !o_ptr.kind.squelch)
					continue;
			}

			/* Mimicked items */
			else if (o_ptr.mimicking_m_idx)
			{
				/* Get the location */
				y = o_ptr.iy;
				x = o_ptr.ix;

				/* Mimicked items try hard not to be compacted */
				if (randint0(100) < 90)
					continue;
			}
			
			/* Dungeon */
			else
			{
				/* Get the location */
				y = o_ptr.iy;
				x = o_ptr.ix;
			}

			/* Nearby objects start out "immune" */
			if ((cur_dis > 0) && (distance(py, px, y, x) < cur_dis) && !o_ptr.kind.squelch)
				continue;

			/* Saving throw */
			chance = 90;


			/* Hack -- only compact artifacts in emergencies */
			if (o_ptr.artifact && (cnt < 1000)) chance = 100;

			/* Apply the saving throw */
			if (randint0(100) < chance) continue;

			/* Delete the object */
			delete_object_idx(i);
			size--;
		}
	}


	/* Reorder objects */
	compact_objects(0);
}



/*
 * Determine if a weapon is 'blessed'
 */
bool is_blessed(const object_type *o_ptr)
{
	bitflag f[OF_SIZE];

	/* Get the flags */
	object_flags(o_ptr, f);

	/* Is the object blessed? */
	return (of_has(f, OF_BLESSED) ? true : false);
}


/**
 * Split off 'at' items from 'src' into 'dest'.
 */
void object_split(struct object *dest, struct object *src, int amt)
{
	/* Distribute charges of wands, staves, or rods */
	distribute_charges(src, dest, amt);

	/* Modify quantity */
	dest.number = amt;
	if (src.note)
		dest.note = src.note;
}

/**
 * Find and return the index to the oldest object on the given grid marked as
 * "squelch".
 */
static s16b floor_get_idx_oldest_squelched(int y, int x)
{
	s16b squelch_idx = 0;
	s16b this_o_idx;

	object_type *o_ptr = null;

	for (this_o_idx = cave.o_idx[y][x]; this_o_idx; this_o_idx = o_ptr.next_o_idx)
	{
		o_ptr = object_byid(this_o_idx);

		if (squelch_item_ok(o_ptr))
			squelch_idx = this_o_idx;
	}

	return squelch_idx;
}


/**
 * This will push objects off a square.
 * 
 * The methodology is to load all objects on the square into a queue. Replace
 * the previous square with a type that does not allow for objects. Drop the
 * objects. Last, put the square back to its original type.
 */
void push_object(int y, int x)
{
	/* Save the original terrain feature */
	int feat_old = cave.feat[y][x];

	object_type *o_ptr;
   
	struct queue *queue = q_new(MAX_FLOOR_STACK);

	/* Push all objects on the square into the queue */
	for (o_ptr = get_first_object(y, x); o_ptr; o_ptr = get_next_object(o_ptr))
		q_push_ptr(queue, o_ptr);

	/* Set feature to an open door */
	cave_set_feat(cave, y, x, FEAT_OPEN);
	
	/* Drop objects back onto the floor */
	while (q_len(queue) > 0)
	{
		/* Take object from the queue */
		o_ptr = q_pop_ptr(queue);
	
		/* Drop the object */
		drop_near(cave, o_ptr, 0, y, x, false);
	}
	
	/* Delete original objects */
	delete_object(y, x);
	
	/* Reset cave feature */
	cave_set_feat(cave, y, x, feat_old);
	
	q_free(queue);
}

/*
 * Scatter some "great" objects near the player
 */
void acquirement(int y1, int x1, int level, int num, bool great)
{
	object_type *i_ptr;
	object_type object_type_body;

	/* Acquirement */
	while (num--)
	{
		/* Get local object */
		i_ptr = &object_type_body;

		/* Wipe the object */
		object_wipe(i_ptr);

		/* Make a good (or great) object (if possible) */
		if (!make_object(cave, i_ptr, level, true, great, null)) continue;

		i_ptr.origin = ORIGIN_ACQUIRE;
		i_ptr.origin_depth = p_ptr.depth;

		/* Drop the object */
		drop_near(cave, i_ptr, 0, y1, x1, true);
	}
}


/*
 * Describe the charges on an item in the inventory.
 */
void inven_item_charges(int item)
{
	object_type *o_ptr = &p_ptr.inventory[item];

	/* Require staff/wand */
	if ((o_ptr.tval != TV_STAFF) && (o_ptr.tval != TV_WAND)) return;

	/* Require known item */
	if (!object_is_known(o_ptr)) return;

	/* Print a message */
	msg("You have %d charge%s remaining.", o_ptr.pval[DEFAULT_PVAL],
	    (o_ptr.pval[DEFAULT_PVAL] != 1) ? "s" : "");
}


/*
 * Describe an item in the inventory. Note: only called when an item is 
 * dropped, used, or otherwise deleted from the inventory
 */
void inven_item_describe(int item)
{
	object_type *o_ptr = &p_ptr.inventory[item];

	char o_name[80];

	if (o_ptr.artifact && 
		(object_is_known(o_ptr) || object_name_is_visible(o_ptr)))
	{
		/* Get a description */
		object_desc(o_name, sizeof(o_name), o_ptr, ODESC_FULL | ODESC_SINGULAR);

		/* Print a message */
		msg("You no longer have the %s (%c).", o_name, index_to_label(item));
	}
	else
	{
		/* Get a description */
		object_desc(o_name, sizeof(o_name), o_ptr, ODESC_PREFIX | ODESC_FULL);

		/* Print a message */
		msg("You have %s (%c).", o_name, index_to_label(item));
	}
}

/**
 * Swap ammunition between quiver slots (0-9).
 */
void swap_quiver_slots(int slot1, int slot2)
{
	int i = slot1 + QUIVER_START;
	int j = slot2 + QUIVER_START;
	object_type o;

	object_copy(&o, &p_ptr.inventory[i]);
	object_copy(&p_ptr.inventory[i], &p_ptr.inventory[j]);
	object_copy(&p_ptr.inventory[j], &o);

	/* Update object_idx if necessary */
	if (tracked_object_is(i))
	{
		track_object(j);
	}

	if (tracked_object_is(j))
	{
		track_object(i);
	}
}

/*
 * Shifts ammo at or above the item slot towards the end of the quiver, making
 * room for a new piece of ammo.
 */
void open_quiver_slot(int slot)
{
	int i, pref;
	int dest = QUIVER_END - 1;

	/* This should only be used on ammunition */
	if (slot < QUIVER_START) return;

	/* Quiver is full */
	if (p_ptr.inventory[QUIVER_END - 1].kind) return;

	/* Find the first open quiver slot */
	while (p_ptr.inventory[dest].kind) dest++;

	/* Swap things with the space one higher (essentially moving the open space
	 * towards our goal slot. */
	for (i = dest - 1; i >= slot; i--)
	{
		/* If we have an item with an inscribed location (and it's in */
		/* that location) then we won't move it. */
		pref = get_inscribed_ammo_slot(&p_ptr.inventory[i]);
		if (i != slot && pref && pref == i) continue;

		/* Update object_idx if necessary */
		if (tracked_object_is(i))
		{
			track_object(dest);
		}

		/* Copy the item up and wipe the old slot */
		COPY(&p_ptr.inventory[dest], &p_ptr.inventory[i], object_type);
		dest = i;
		object_wipe(&p_ptr.inventory[dest]);


	}
}


/*
 * Describe the charges on an item on the floor.
 */
void floor_item_charges(int item)
{
	object_type *o_ptr = object_byid(item);

	/* Require staff/wand */
	if ((o_ptr.tval != TV_STAFF) && (o_ptr.tval != TV_WAND)) return;

	/* Require known item */
	if (!object_is_known(o_ptr)) return;

	/* Print a message */
	msg("There %s %d charge%s remaining.",
	    (o_ptr.pval[DEFAULT_PVAL] != 1) ? "are" : "is",
	     o_ptr.pval[DEFAULT_PVAL],
	    (o_ptr.pval[DEFAULT_PVAL] != 1) ? "s" : "");
}



/*
 * Describe an item in the inventory.
 */
void floor_item_describe(int item)
{
	object_type *o_ptr = object_byid(item);

	char o_name[80];

	/* Get a description */
	object_desc(o_name, sizeof(o_name), o_ptr, ODESC_PREFIX | ODESC_FULL);

	/* Print a message */
	msg("You see %s.", o_name);
}


/*
 * Increase the "number" of an item on the floor
 */
void floor_item_increase(int item, int num)
{
	object_type *o_ptr = object_byid(item);

	/* Apply */
	num += o_ptr.number;

	/* Bounds check */
	if (num > 255) num = 255;
	else if (num < 0) num = 0;

	/* Un-apply */
	num -= o_ptr.number;

	/* Change the number */
	o_ptr.number += num;
}


/*
 * Optimize an item on the floor (destroy "empty" items)
 */
void floor_item_optimize(int item)
{
	object_type *o_ptr = object_byid(item);

	/* Paranoia -- be sure it exists */
	if (!o_ptr.kind) return;

	/* Only optimize empty items */
	if (o_ptr.number) return;

	/* Delete the object */
	delete_object_idx(item);
}


/*
 * Take off (some of) a non-cursed equipment item
 *
 * Note that only one item at a time can be wielded per slot.
 *
 * Note that taking off an item when "full" may cause that item
 * to fall to the ground.
 *
 * Return the inventory slot into which the item is placed.
 */
s16b inven_takeoff(int item, int amt)
{
	int slot;

	object_type *o_ptr;

	object_type *i_ptr;
	object_type object_type_body;

	const char *act;

	char o_name[80];

	bool track_removed_item = false;


	/* Get the item to take off */
	o_ptr = &p_ptr.inventory[item];

	/* Paranoia */
	if (amt <= 0) return (-1);

	/* Verify */
	if (amt > o_ptr.number) amt = o_ptr.number;

	/* Get local object */
	i_ptr = &object_type_body;

	/* Obtain a local object */
	object_copy(i_ptr, o_ptr);

	/* Modify quantity */
	i_ptr.number = amt;

	/* Describe the object */
	object_desc(o_name, sizeof(o_name), i_ptr, ODESC_PREFIX | ODESC_FULL);

	/* Took off weapon */
	if (item == INVEN_WIELD)
	{
		act = "You were wielding";
	}

	/* Took off bow */
	else if (item == INVEN_BOW)
	{
		act = "You were holding";
	}

	/* Took off light */
	else if (item == INVEN_LIGHT)
	{
		act = "You were holding";
	}

	/* Took off something */
	else
	{
		act = "You were wearing";
	}

	/* Update object_idx if necessary, after optimization */
	if (tracked_object_is(item))
	{
		track_removed_item = true;
	}

	/* Modify, Optimize */
	inven_item_increase(item, -amt);
	inven_item_optimize(item);

	/* Carry the object */
	slot = inven_carry(p_ptr, i_ptr);

	/* Track removed item if necessary */
	if (track_removed_item)
	{
		track_object(slot);
	}

	/* Message */
	msgt(MSG_WIELD, "%s %s (%c).", act, o_name, index_to_label(slot));

	p_ptr.notice |= PN_SQUELCH;

	/* Return slot */
	return (slot);
}


/*
 *Returns the number of times in 1000 that @ will FAIL
 * - thanks to Ed Graham for the formula
 */
int get_use_device_chance(const object_type *o_ptr)
{
	int lev, fail, numerator, denominator;

	int skill = p_ptr.state.skills[SKILL_DEVICE];

	int skill_min = 10;
	int skill_max = 141;
	int diff_min  = 1;
	int diff_max  = 100;

	/* Extract the item level, which is the difficulty rating */
	if (o_ptr.artifact)
		lev = o_ptr.artifact.level;
	else
		lev = o_ptr.kind.level;

	/* TODO: maybe use something a little less convoluted? */
	numerator   = (skill - lev) - (skill_max - diff_min);
	denominator = (lev - skill) - (diff_max - skill_min);

	/* Make sure that we don't divide by zero */
	if (denominator == 0) denominator = numerator > 0 ? 1 : -1;

	fail = (100 * numerator) / denominator;

	/* Ensure failure rate is between 1% and 75% */
	if (fail > 750) fail = 750;
	if (fail < 10) fail = 10;

	return fail;
}


/*
 * Distribute charges of rods, staves, or wands.
 *
 * o_ptr = source item
 * q_ptr = target item, must be of the same type as o_ptr
 * amt   = number of items that are transfered
 */
void distribute_charges(object_type *o_ptr, object_type *q_ptr, int amt)
{
	int charge_time = randcalc(o_ptr.kind.time, 0, AVERAGE), max_time;

	/*
	 * Hack -- If rods, staves, or wands are dropped, the total maximum
	 * timeout or charges need to be allocated between the two stacks.
	 * If all the items are being dropped, it makes for a neater message
	 * to leave the original stack's pval alone. -LM-
	 */
	if ((o_ptr.tval == TV_WAND) ||
	    (o_ptr.tval == TV_STAFF))
	{
		q_ptr.pval[DEFAULT_PVAL] = o_ptr.pval[DEFAULT_PVAL] * amt / o_ptr.number;

		if (amt < o_ptr.number)
			o_ptr.pval[DEFAULT_PVAL] -= q_ptr.pval[DEFAULT_PVAL];
	}

	/*
	 * Hack -- Rods also need to have their timeouts distributed.
	 *
	 * The dropped stack will accept all time remaining to charge up to
	 * its maximum.
	 */
	if (o_ptr.tval == TV_ROD)
	{
		max_time = charge_time * amt;

		if (o_ptr.timeout > max_time)
			q_ptr.timeout = max_time;
		else
			q_ptr.timeout = o_ptr.timeout;

		if (amt < o_ptr.number)
			o_ptr.timeout -= q_ptr.timeout;
	}
}


void reduce_charges(object_type *o_ptr, int amt)
{
	/*
	 * Hack -- If rods or wand are destroyed, the total maximum timeout or
	 * charges of the stack needs to be reduced, unless all the items are
	 * being destroyed. -LM-
	 */
	if (((o_ptr.tval == TV_WAND) ||
	     (o_ptr.tval == TV_STAFF)) &&
	    (amt < o_ptr.number))
	{
		o_ptr.pval[DEFAULT_PVAL] -= o_ptr.pval[DEFAULT_PVAL] * amt / o_ptr.number;
	}

	if ((o_ptr.tval == TV_ROD) &&
	    (amt < o_ptr.number))
	{
		o_ptr.timeout -= o_ptr.timeout * amt / o_ptr.number;
	}
}

/*** Object kind lookup functions ***/


/**
 * Return the k_idx of the object kind with the given `tval` and name `name`.
 */
int lookup_name(int tval, const char *name)
{
	int k;

	/* Look for it */
	for (k = 1; k < z_info.k_max; k++)
	{
		object_kind *k_ptr = &k_info[k];
		const char *nm = k_ptr.name;

		if (!nm) continue;

		if (*nm == '&' && *(nm+1))
			nm += 2;

		/* Found a match */
		if (k_ptr.tval == tval && !strcmp(name, nm))
			return k;
	}

	msg("No object (\"%s\",\"%s\")", tval_find_name(tval), name);
	return -1;
}


/**
 * Sort comparator for objects using only tval and sval.
 * -1 if o1 should be first
 *  1 if o2 should be first
 *  0 if it doesn't matter
 */
static int compare_types(const object_type *o1, const object_type *o2)
{
	if (o1.tval == o2.tval)
		return CMP(o1.sval, o2.sval);
	else
		return CMP(o1.tval, o2.tval);
}

/**
 * Sort comparator for objects
 * -1 if o1 should be first
 *  1 if o2 should be first
 *  0 if it doesn't matter
 *
 * The sort order is designed with the "list items" command in mind.
 */
static int compare_items(const object_type *o1, const object_type *o2)
{
	/* known artifacts will sort first */
	if (object_is_known_artifact(o1) && object_is_known_artifact(o2))
		return compare_types(o1, o2);
	if (object_is_known_artifact(o1)) return -1;
	if (object_is_known_artifact(o2)) return 1;

	/* unknown objects will sort next */
	if (!object_flavor_is_aware(o1) && !object_flavor_is_aware(o2))
		return compare_types(o1, o2);
	if (!object_flavor_is_aware(o1)) return -1;
	if (!object_flavor_is_aware(o2)) return 1;

	/* if only one of them is worthless, the other comes first */
	if (o1.kind.cost == 0 && o2.kind.cost != 0) return 1;
	if (o1.kind.cost != 0 && o2.kind.cost == 0) return -1;

	/* otherwise, just compare tvals and svals */
	/* NOTE: arguably there could be a better order than this */
	return compare_types(o1, o2);
}


/**
 * Helper to draw the Object Recall subwindow; this actually does the work.
 */
void display_object_recall(object_type *o_ptr)
{
	char header[120];

	textblock *tb = object_info(o_ptr, OINFO_NONE);
	object_desc(header, sizeof(header), o_ptr, ODESC_PREFIX | ODESC_FULL);

	clear_from(0);
	textui_textblock_place(tb, SCREEN_REGION, header);
	textblock_free(tb);
}


/**
 * This draws the Object Recall subwindow when displaying a particular object
 * (e.g. a helmet in the backpack, or a scroll on the ground)
 */
void display_object_idx_recall(s16b item)
{
	object_type *o_ptr = object_from_item_idx(item);
	display_object_recall(o_ptr);
}


/**
 * This draws the Object Recall subwindow when displaying a recalled item kind
 * (e.g. a generic ring of acid or a generic blade of chaos)
 */
void display_object_kind_recall(s16b k_idx)
{
	object_type object = { 0 };
	object_prep(&object, &k_info[k_idx], 0, EXTREMIFY);
	if (k_info[k_idx].aware)
		object.ident |= IDENT_STORE;

	display_object_recall(&object);
}

/*
 * Display visible items, similar to display_monlist
 */
void display_itemlist(void)
{
	int max;
	int mx, my;
	unsigned num;
	int line = 1, x = 0;
	int cur_x;
	unsigned i;
	unsigned disp_count = 0;
	byte a;
	char c;

	object_type *types[MAX_ITEMLIST];
	int counts[MAX_ITEMLIST];
	int dx[MAX_ITEMLIST], dy[MAX_ITEMLIST];
	unsigned counter = 0;

	int dungeon_hgt = p_ptr.depth == 0 ? TOWN_HGT : DUNGEON_HGT;
	int dungeon_wid = p_ptr.depth == 0 ? TOWN_WID : DUNGEON_WID;

	byte attr;
	char buf[80];

	int floor_list[MAX_FLOOR_STACK];

	/* Clear the term if in a subwindow, set x otherwise */
	if (Term != angband_term[0])
	{
		clear_from(0);
		max = Term.hgt - 1;
	}
	else
	{
		x = 13;
		max = Term.hgt - 2;
	}

	/* Look at each square of the dungeon for items */
	for (my = 0; my < dungeon_hgt; my++)
	{
		for (mx = 0; mx < dungeon_wid; mx++)
		{
			num = scan_floor(floor_list, MAX_FLOOR_STACK, my, mx, 0x02);

			/* Iterate over all the items found on this square */
			for (i = 0; i < num; i++)
			{
				object_type *o_ptr = object_byid(floor_list[i]);
				unsigned j;

				/* Skip gold/squelched */
				if (o_ptr.tval == TV_GOLD || squelch_item_ok(o_ptr))
					continue;

				/* See if we've already seen a similar item; if so, just add */
				/* to its count */
				for (j = 0; j < counter; j++)
				{
					if (object_similar(o_ptr, types[j],
						OSTACK_LIST))
					{
						counts[j] += o_ptr.number;
						if ((my - p_ptr.py) * (my - p_ptr.py) + (mx - p_ptr.px) * (mx - p_ptr.px) < dy[j] * dy[j] + dx[j] * dx[j])
						{
							dy[j] = my - p_ptr.py;
							dx[j] = mx - p_ptr.px;
						}
						break;
					}
				}

				/* We saw a new item. So insert it at the end of the list and */
				/* then sort it forward using compare_items(). The types list */
				/* is always kept sorted. */
				if (j == counter)
				{
					types[counter] = o_ptr;
					counts[counter] = o_ptr.number;
					dy[counter] = my - p_ptr.py;
					dx[counter] = mx - p_ptr.px;

					while (j > 0 && compare_items(types[j - 1], types[j]) > 0)
					{
						object_type *tmp_o = types[j - 1];
						int tmpcount;
						int tmpdx = dx[j-1];
						int tmpdy = dy[j-1];

						types[j - 1] = types[j];
						types[j] = tmp_o;
						dx[j-1] = dx[j];
						dx[j] = tmpdx;
						dy[j-1] = dy[j];
						dy[j] = tmpdy;
						tmpcount = counts[j - 1];
						counts[j - 1] = counts[j];
						counts[j] = tmpcount;
						j--;
					}
					counter++;
				}
			}
		}
	}

	/* Note no visible items */
	if (!counter)
	{
		/* Clear display and print note */
		c_prt(TERM_SLATE, "You see no items.", 0, 0);
		if (Term == angband_term[0])
			Term_addstr(-1, TERM_WHITE, "  (Press any key to continue.)");

		/* Done */
		return;
	}
	else
	{
		/* Reprint Message */
		prt(format("You can see %d item%s:",
				   counter, (counter > 1 ? "s" : "")), 0, 0);
	}

	for (i = 0; i < counter; i++)
	{
		/* o_name will hold the object_desc() name for the object. */
		/* o_desc will also need to put a (x4) behind it. */
		/* can there be more than 999 stackable items on a level? */
		char o_name[80];
		char o_desc[86];

		object_type *o_ptr = types[i];

		/* We shouldn't list coins or squelched items */
		if (o_ptr.tval == TV_GOLD || squelch_item_ok(o_ptr))
			continue;

		object_desc(o_name, sizeof(o_name), o_ptr, ODESC_FULL);
		if (counts[i] > 1)
			strnfmt(o_desc, sizeof(o_desc), "%s (x%d) %d %c, %d %c", o_name, counts[i],
				(dy[i] > 0) ? dy[i] : -dy[i], (dy[i] > 0) ? 'S' : 'N',
				(dx[i] > 0) ? dx[i] : -dx[i], (dx[i] > 0) ? 'E' : 'W');
		else
			strnfmt(o_desc, sizeof(o_desc), "%s  %d %c %d %c", o_name,
				(dy[i] > 0) ? dy[i] : -dy[i], (dy[i] > 0) ? 'S' : 'N',
				(dx[i] > 0) ? dx[i] : -dx[i], (dx[i] > 0) ? 'E' : 'W');

		/* Reset position */
		cur_x = x;

		/* See if we need to scroll or not */
		if (Term == angband_term[0] && (line == max) && disp_count != counter)
		{
			prt("-- more --", line, x);
			anykey();

			/* Clear the screen */
			for (line = 1; line <= max; line++)
				prt("", line, x);

			/* Reprint Message */
			prt(format("You can see %d item%s:",
					   counter, (counter > 1 ? "s" : "")), 0, 0);

			/* Reset */
			line = 1;
		}
		else if (line == max)
		{
			continue;
		}

		/* Note that the number of items actually displayed */
		disp_count++;

		if (o_ptr.artifact && object_is_known(o_ptr))
			/* known artifact */
			attr = TERM_VIOLET;
		else if (!object_flavor_is_aware(o_ptr))
			/* unaware of kind */
			attr = TERM_RED;
		else if (o_ptr.kind.cost == 0)
			/* worthless */
			attr = TERM_SLATE;
		else
			/* default */
			attr = TERM_WHITE;

		a = object_kind_attr(o_ptr.kind);
		c = object_kind_char(o_ptr.kind);

		/* Display the pict */
		if ((tile_width == 1) && (tile_height == 1))
		{
		        Term_putch(cur_x++, line, a, c);
			Term_putch(cur_x++, line, TERM_WHITE, ' ');
		}

		/* Print and bump line counter */
		c_prt(attr, o_desc, line, cur_x);
		line++;
	}

	if (disp_count != counter)
	{
		/* Print "and others" message if we've run out of space */
		strnfmt(buf, sizeof buf, "  ...and %d others.", counter - disp_count);
		c_prt(TERM_WHITE, buf, line, x);
	}
	else
	{
		/* Otherwise clear a line at the end, for main-term display */
		prt("", line, x);
	}

	if (Term == angband_term[0])
		Term_addstr(-1, TERM_WHITE, "  (Press any key to continue.)");
}


/*** Generic utility functions ***/




/*
 * Get a list of "valid" item indexes.
 *
 * Fills item_list[] with items that are "okay" as defined by the
 * current item_tester_hook, etc.  mode determines what combination of
 * inventory, equipment and player's floor location should be used
 * when drawing up the list.
 *
 * Returns the number of items placed into the list.
 *
 * Maximum space that can be used is [INVEN_TOTAL + MAX_FLOOR_STACK],
 * though practically speaking much smaller numbers are likely.
 */
int scan_items(int *item_list, size_t item_list_max, int mode)
{
	bool use_inven = ((mode & USE_INVEN) ? true : false);
	bool use_equip = ((mode & USE_EQUIP) ? true : false);
	bool use_floor = ((mode & USE_FLOOR) ? true : false);

	int floor_list[MAX_FLOOR_STACK];
	int floor_num;

	int i;
	size_t item_list_num = 0;

	if (use_inven)
	{
		for (i = 0; i < INVEN_PACK && item_list_num < item_list_max; i++)
		{
			if (get_item_okay(i))
				item_list[item_list_num++] = i;
		}
	}

	if (use_equip)
	{
		for (i = INVEN_WIELD; i < ALL_INVEN_TOTAL && item_list_num < item_list_max; i++)
		{
			if (get_item_okay(i))
				item_list[item_list_num++] = i;
		}
	}

	/* Scan all non-gold objects in the grid */
	if (use_floor)
	{
		floor_num = scan_floor(floor_list, N_ELEMENTS(floor_list), p_ptr.py, p_ptr.px, 0x03);

		for (i = 0; i < floor_num && item_list_num < item_list_max; i++)
		{
			if (get_item_okay(-floor_list[i]))
				item_list[item_list_num++] = -floor_list[i];
		}
	}

	/* Forget the item_tester_tval and item_tester_hook  restrictions */
	item_tester_tval = 0;
	item_tester_hook = null;

	return item_list_num;
}




/*
 * Returns whether the pack is holding the maximum number of items. The max
 * size is INVEN_MAX_PACK, which is a macro since quiver size affects slots
 * available.
 */
bool pack_is_full(void)
{
	return p_ptr.inventory[INVEN_MAX_PACK - 1].kind ? true : false;
}

void objects_destroy(void)
{
	mem_free(o_list);
}
