/*
 * File: store.c
 * Purpose: Store stocking and UI
 *
 * Copyright (c) 1997 Robert A. Koeneke, James E. Wilson, Ben Harrison
 * Copyright (c) 2007 Andrew Sidwell, who rewrote a fair portion
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
#include "game-event.h"
#include "history.h"
#include "init.h"
#include "object/inventory.h"
#include "object/tvalsval.h"
#include "object/object.h"
#include "spells.h"
#include "squelch.h"
#include "target.h"
#include "textui.h"
#include "ui-menu.h"
#include "z-debug.h"

/*** Constants and definitions ***/






/*** Utilities ***/


/* Randomly select one of the entries in an array */
#define ONE_OF(x)	x[randint0(N_ELEMENTS(x))]



/*** Flavour text stuff ***/

/*
 * Messages for reacting to purchase prices.
 */
static const char *comment_worthless[] =
{
	"Arrgghh!",
	"You bastard!",
	"You hear someone sobbing...",
	"The shopkeeper howls in agony!",
	"The shopkeeper wails in anguish!",
	"The shopkeeper beats his head against the counter."
};

static const char *comment_bad[] =
{
	"Damn!",
	"You fiend!",
	"The shopkeeper curses at you.",
	"The shopkeeper glares at you."
};

static const char *comment_accept[] =
{
	"Okay.",
	"Fine.",
	"Accepted!",
	"Agreed!",
	"Done!",
	"Taken!"
};

static const char *comment_good[] =
{
	"Cool!",
	"You've made my day!",
	"The shopkeeper sniggers.",
	"The shopkeeper giggles.",
	"The shopkeeper laughs loudly."
};

static const char *comment_great[] =
{
	"Yipee!",
	"I think I'll retire!",
	"The shopkeeper jumps for joy.",
	"The shopkeeper smiles gleefully.",
	"Wow.  I'm going to name my new villa in your honour."
};

/*
 * Get rid of stores at cleanup. Gets rid of everything.
 */
void free_stores(void)
{
	struct owner *o;
	struct owner *next;
	int i;

	/* Free the store inventories */
	for (i = 0; i < MAX_STORES; i++)
	{
		/* Get the store */
		struct store *store = &stores[i];

		/* Free the store inventory */
		mem_free(store.stock);
		mem_free(store.table);

		for (o = store.owners; o; o = next) {
			next = o.next;
			string_free(o.name);
			mem_free(o);
		}
	}
	mem_free(stores);
}





/*
 * Let a shop-keeper React to a purchase
 *
 * We paid "price", it was worth "value", and we thought it was worth "guess"
 */
static void purchase_analyze(s32b price, s32b value, s32b guess)
{
	/* Item was worthless, but we bought it */
	if ((value <= 0) && (price > value))
		msgt(MSG_STORE1, "%s", ONE_OF(comment_worthless));

	/* Item was cheaper than we thought, and we paid more than necessary */
	else if ((value < guess) && (price > value))
		msgt(MSG_STORE2, "%s", ONE_OF(comment_bad));

	/* Item was a good bargain, and we got away with it */
	else if ((value > guess) && (value < (4 * guess)) && (price < value))
		msgt(MSG_STORE3, "%s", ONE_OF(comment_good));

	/* Item was a great bargain, and we got away with it */
	else if ((value > guess) && (price < value))
		msgt(MSG_STORE4, "%s", ONE_OF(comment_great));
}




/*** Check if a store will buy an object ***/

/*
 * Determine if the current store will purchase the given object
 *
 * Note that a shop-keeper must refuse to buy "worthless" objects
 */
static bool store_will_buy(struct store *store, const object_type *o_ptr)
{
	/* Switch on the store */
	switch (store.sidx)
	{
		/* General Store */
		case STORE_GENERAL:
		{
			/* Accept lights (inc. oil), spikes and food */
			if (o_ptr.tval == TV_LIGHT || o_ptr.tval == TV_FOOD ||
					o_ptr.tval == TV_FLASK || o_ptr.tval == TV_SPIKE) break;
			else return false;
		}

		/* Armoury */
		case STORE_ARMOR:
		{
			/* Analyze the type */
			switch (o_ptr.tval)
			{
				case TV_BOOTS:
				case TV_GLOVES:
				case TV_CROWN:
				case TV_HELM:
				case TV_SHIELD:
				case TV_CLOAK:
				case TV_SOFT_ARMOR:
				case TV_HARD_ARMOR:
				case TV_DRAG_ARMOR:
					break;

				default:
					return (false);
			}
			break;
		}

		/* Weapon Shop */
		case STORE_WEAPON:
		{
			/* Analyze the type */
			switch (o_ptr.tval)
			{
				case TV_SHOT:
				case TV_BOLT:
				case TV_ARROW:
				case TV_BOW:
				case TV_DIGGING:
				case TV_HAFTED:
				case TV_POLEARM:
				case TV_SWORD:
					break;

				default:
					return (false);
			}
			break;
		}

		/* Temple */
		case STORE_TEMPLE:
		{
			/* Analyze the type */
			switch (o_ptr.tval)
			{
				case TV_PRAYER_BOOK:
				case TV_SCROLL:
				case TV_POTION:
				case TV_HAFTED:
					break;

				case TV_POLEARM:
				case TV_SWORD:
				case TV_DIGGING:
				{
					/* Known blessed blades are accepted too */
					if (object_is_known_blessed(o_ptr)) break;
				}

				default:
					return (false);
			}
			break;
		}

		/* Alchemist */
		case STORE_ALCHEMY:
		{
			/* Analyze the type */
			switch (o_ptr.tval)
			{
				case TV_SCROLL:
				case TV_POTION:
					break;

				default:
					return (false);
			}
			break;
		}

		/* Magic Shop */
		case STORE_MAGIC:
		{
			/* Analyze the type */
			switch (o_ptr.tval)
			{
				case TV_MAGIC_BOOK:
				case TV_AMULET:
				case TV_RING:
				case TV_STAFF:
				case TV_WAND:
				case TV_ROD:
				case TV_SCROLL:
				case TV_POTION:
					break;

				default:
					return (false);
			}
			break;
		}

		/* Home */
		case STORE_HOME:
		{
			return true;
		}
	}

	/* Ignore "worthless" items */
	if (object_value(o_ptr, 1, false) <= 0) return (false);

	/* Assume okay */
	return (true);
}

/*** Basics: pricing, generation, etc. ***/

/*
 * Check to see if the shop will be carrying too many objects
 *
 * Note that the shop, just like a player, will not accept things
 * it cannot hold.  Before, one could "nuke" objects this way, by
 * adding them to a pile which was already full.
 */
static bool store_check_num(struct store *store, const object_type *o_ptr)
{
	int i;
	object_type *j_ptr;

	/* Free space is always usable */
	if (store.stock_num < store.stock_size) return true;

	/* The "home" acts like the player */
	if (store.sidx == STORE_HOME)
	{
		/* Check all the objects */
		for (i = 0; i < store.stock_num; i++)
		{
			/* Get the existing object */
			j_ptr = &store.stock[i];

			/* Can the new object be combined with the old one? */
			if (object_similar(j_ptr, o_ptr, OSTACK_PACK))
				return (true);
		}
	}

	/* Normal stores do special stuff */
	else
	{
		/* Check all the objects */
		for (i = 0; i < store.stock_num; i++)
		{
			/* Get the existing object */
			j_ptr = &store.stock[i];

			/* Can the new object be combined with the old one? */
			if (object_similar(j_ptr, o_ptr, OSTACK_STORE))
				return (true);
		}
	}

	/* But there was no room at the inn... */
	return (false);
}



/*
 * Add an object to the inventory of the Home.
 *
 * In all cases, return the slot (or -1) where the object was placed.
 *
 * Note that this is a hacked up version of "inven_carry()".
 *
 * Also note that it may not correctly "adapt" to "knowledge" becoming
 * known: the player may have to pick stuff up and drop it again.
 */
static int home_carry(object_type *o_ptr)
{
	int i, slot;
	u32b value, j_value;
	object_type *j_ptr;

	struct store *store = &stores[STORE_HOME];

	/* Check each existing object (try to combine) */
	for (slot = 0; slot < store.stock_num; slot++)
	{
		/* Get the existing object */
		j_ptr = &store.stock[slot];

		/* The home acts just like the player */
		if (object_similar(j_ptr, o_ptr, OSTACK_PACK))
		{
			/* Save the new number of items */
			object_absorb(j_ptr, o_ptr);

			/* All done */
			return (slot);
		}
	}

	/* No space? */
	if (store.stock_num >= store.stock_size) return (-1);

	/* Determine the "value" of the object */
	value = object_value(o_ptr, 1, false);

	/* Check existing slots to see if we must "slide" */
	for (slot = 0; slot < store.stock_num; slot++)
	{
		/* Get that object */
		j_ptr = &store.stock[slot];

		/* Hack -- readable books always come first */
		if ((o_ptr.tval == p_ptr.class.spell_book) &&
		    (j_ptr.tval != p_ptr.class.spell_book)) break;
		if ((j_ptr.tval == p_ptr.class.spell_book) &&
		    (o_ptr.tval != p_ptr.class.spell_book)) continue;

		/* Objects sort by decreasing type */
		if (o_ptr.tval > j_ptr.tval) break;
		if (o_ptr.tval < j_ptr.tval) continue;

		/* Can happen in the home */
		if (!object_flavor_is_aware(o_ptr)) continue;
		if (!object_flavor_is_aware(j_ptr)) break;

		/* Objects sort by increasing sval */
		if (o_ptr.sval < j_ptr.sval) break;
		if (o_ptr.sval > j_ptr.sval) continue;

		/* Objects in the home can be unknown */
		if (!object_is_known(o_ptr)) continue;
		if (!object_is_known(j_ptr)) break;

		/* Objects sort by decreasing value */
		j_value = object_value(j_ptr, 1, false);
		if (value > j_value) break;
		if (value < j_value) continue;
	}

	/* Slide the others up */
	for (i = store.stock_num; i > slot; i--)
	{
		/* Hack -- slide the objects */
		object_copy(&store.stock[i], &store.stock[i-1]);
	}

	/* More stuff now */
	store.stock_num++;

	/* Hack -- Insert the new object */
	object_copy(&store.stock[slot], o_ptr);

	/* Return the location */
	return (slot);
}


/*** Display code ***/











/*** Higher-level code ***/






/*
 * Determine if the current store will purchase the given object
 */
static bool store_will_buy_tester(const object_type *o_ptr)
{
	struct store *store = current_store();
	if (store)
		return store_will_buy(store, o_ptr);

	return false;
}












