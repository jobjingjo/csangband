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


static bool store_get_check(const char *prompt)
{
	struct keypress ch;

	/* Prompt for it */
	prt(prompt, 0, 0);

	/* Get an answer */
	ch = inkey();

	/* Erase the prompt */
	prt("", 0, 0);

	if (ch.code == ESCAPE) return (false);
	if (strchr("Nn", ch.code)) return (false);

	/* Success */
	return (true);
}


/*
 * Return the quantity of a given item in the pack (include quiver).
 */
static int find_inven(const object_type *o_ptr)
{
	int i, j;
	int num = 0;

	/* Similar slot? */
	for (j = 0; j < QUIVER_END; j++)
	{
		object_type *j_ptr = &p_ptr.inventory[j];

		/* Check only the inventory and the quiver */
		if (j >= INVEN_WIELD && j < QUIVER_START) continue;

		/* Require identical object types */
		if (o_ptr.kind != j_ptr.kind) continue;

		/* Analyze the items */
		switch (o_ptr.tval)
		{
			/* Chests */
			case TV_CHEST:
			{
				/* Never okay */
				return 0;
			}

			/* Food and Potions and Scrolls */
			case TV_FOOD:
			case TV_POTION:
			case TV_SCROLL:
			{
				/* Assume okay */
				break;
			}

			/* Staves and Wands */
			case TV_STAFF:
			case TV_WAND:
			{
				/* Assume okay */
				break;
			}

			/* Rods */
			case TV_ROD:
			{
				/* Assume okay */
				break;
			}

			/* Weapons and Armor */
			case TV_BOW:
			case TV_DIGGING:
			case TV_HAFTED:
			case TV_POLEARM:
			case TV_SWORD:
			case TV_BOOTS:
			case TV_GLOVES:
			case TV_HELM:
			case TV_CROWN:
			case TV_SHIELD:
			case TV_CLOAK:
			case TV_SOFT_ARMOR:
			case TV_HARD_ARMOR:
			case TV_DRAG_ARMOR:
			{
				/* Fall through */
			}

			/* Rings, Amulets, Lights */
			case TV_RING:
			case TV_AMULET:
			case TV_LIGHT:
			{
				/* Require both items to be known */
				if (!object_is_known(o_ptr) || !object_is_known(j_ptr)) continue;

				/* Fall through */
			}

			/* Missiles */
			case TV_BOLT:
			case TV_ARROW:
			case TV_SHOT:
			{
				/* Require identical knowledge of both items */
				if (object_is_known(o_ptr) != object_is_known(j_ptr)) continue;

				/* Require identical "bonuses" */
				if (o_ptr.to_h != j_ptr.to_h) continue;
				if (o_ptr.to_d != j_ptr.to_d) continue;
				if (o_ptr.to_a != j_ptr.to_a) continue;

				/* Require identical "pval" codes */
				for (i = 0; i < MAX_PVALS; i++)
					if (o_ptr.pval[i] != j_ptr.pval[i])
						continue;

				if (o_ptr.num_pvals != j_ptr.num_pvals)
					continue;

				/* Require identical "artifact" names */
				if (o_ptr.artifact != j_ptr.artifact) continue;

				/* Require identical "ego-item" names */
				if (o_ptr.ego != j_ptr.ego) continue;

				/* Lights must have same amount of fuel */
				else if (o_ptr.timeout != j_ptr.timeout && o_ptr.tval == TV_LIGHT)
					continue;

				/* Require identical "values" */
				if (o_ptr.ac != j_ptr.ac) continue;
				if (o_ptr.dd != j_ptr.dd) continue;
				if (o_ptr.ds != j_ptr.ds) continue;

				/* Probably okay */
				break;
			}

			/* Various */
			default:
			{
				/* Require knowledge */
				if (!object_is_known(o_ptr) || !object_is_known(j_ptr)) continue;

				/* Probably okay */
				break;
			}
		}


		/* Different flags */
		if (!of_is_equal(o_ptr.flags, j_ptr.flags))
			continue;

		/* They match, so add up */
		num += j_ptr.number;
	}

	return num;
}


/*
 * Buy an object from a store
 */
static bool store_purchase(int item)
{
	int amt, num;

	object_type *o_ptr;

	object_type object_type_body;
	object_type *i_ptr = &object_type_body;

	char o_name[80];

	s32b price;

	struct store *store = current_store();

	if (!store) {
		msg("You cannot purchase items when not in a store.");
		return false;
	}

	/* Get the actual object */
	o_ptr = &store.stock[item];
	if (item < 0) return false;

	/* Clear all current messages */
	msg_flag = false;
	prt("", 0, 0);

	if (store.sidx == STORE_HOME) {
		amt = o_ptr.number;
	} else {
		/* Price of one */
		price = price_item(o_ptr, false, 1);

		/* Check if the player can afford any at all */
		if ((u32b)p_ptr.au < (u32b)price)
		{
			/* Tell the user */
			msg("You do not have enough gold for this item.");

			/* Abort now */
			return false;
		}

		/* Work out how many the player can afford */
		amt = p_ptr.au / price;
		if (amt > o_ptr.number) amt = o_ptr.number;
		
		/* Double check for wands/staves */
		if ((p_ptr.au >= price_item(o_ptr, false, amt+1)) && (amt < o_ptr.number))
			amt++;

	}

	/* Find the number of this item in the inventory */
	if (!object_flavor_is_aware(o_ptr))
		num = 0;
	else
		num = find_inven(o_ptr);

	strnfmt(o_name, sizeof o_name, "%s how many%s? (max %d) ",
	        (store.sidx == STORE_HOME) ? "Take" : "Buy",
	        num ? format(" (you have %d)", num) : "", amt);

	/* Get a quantity */
	amt = get_quantity(o_name, amt);

	/* Allow user abort */
	if (amt <= 0) return false;

	/* Get desired object */
	object_copy_amt(i_ptr, o_ptr, amt);

	/* Ensure we have room */
	if (!inven_carry_okay(i_ptr))
	{
		msg("You cannot carry that many items.");
		return false;
	}

	/* Describe the object (fully) */
	object_desc(o_name, sizeof(o_name), i_ptr, ODESC_PREFIX | ODESC_FULL);

	/* Attempt to buy it */
	if (store.sidx != STORE_HOME)
	{
		u32b price;
		bool response;

		/* Extract the price for the entire stack */
		price = price_item(i_ptr, false, i_ptr.number);

		screen_save();

		/* Show price */
		prt(format("Price: %d", price), 1, 0);

		/* Confirm purchase */
		response = store_get_check(format("Buy %s? [ESC, any other key to accept]", o_name));
		screen_load();

		/* Negative response, so give up */
		if (!response) return false;

		cmd_insert(CMD_BUY);
		cmd_set_arg_choice(cmd_get_top(), 0, item);
		cmd_set_arg_number(cmd_get_top(), 1, amt);
	}

	/* Home is much easier */
	else
	{
		cmd_insert(CMD_RETRIEVE);
		cmd_set_arg_choice(cmd_get_top(), 0, item);
		cmd_set_arg_number(cmd_get_top(), 1, amt);
	}

	/* Not kicked out */
	return true;
}



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

/*
 * Sell an object, or drop if it we're in the home.
 */
static bool store_sell(void)
{
	int amt;
	int item;
	int get_mode = USE_EQUIP | USE_INVEN | USE_FLOOR;

	object_type *o_ptr;
	object_type object_type_body;
	object_type *i_ptr = &object_type_body;

	char o_name[120];


	const char *reject = "You have nothing that I want. ";
	const char *prompt = "Sell which item? ";

	struct store *store = current_store();

	if (!store) {
		msg("You cannot sell items when not in a store.");
		return false;
	}

	/* Clear all current messages */
	msg_flag = false;
	prt("", 0, 0);

	if (store.sidx == STORE_HOME) {
		prompt = "Drop which item? ";
	} else {
		item_tester_hook = store_will_buy_tester;
		get_mode |= SHOW_PRICES;
	}

	/* Get an item */
	p_ptr.command_wrk = USE_INVEN;

	if (!get_item(&item, prompt, reject, CMD_DROP, get_mode))
		return false;

	/* Get the item */
	o_ptr = object_from_item_idx(item);

	/* Hack -- Cannot remove cursed objects */
	if ((item >= INVEN_WIELD) && cursed_p(o_ptr.flags))
	{
		/* Oops */
		msg("Hmmm, it seems to be cursed.");

		/* Nope */
		return false;
	}

	/* Get a quantity */
	amt = get_quantity(null, o_ptr.number);

	/* Allow user abort */
	if (amt <= 0) return false;

	/* Get a copy of the object representing the number being sold */
	object_copy_amt(i_ptr, object_from_item_idx(item), amt);

	if (!store_check_num(store, i_ptr))
	{
		if (store.sidx == STORE_HOME)
			msg("Your home is full.");

		else
			msg("I have not the room in my store to keep it.");

		return false;
	}

	/* Get a full description */
	object_desc(o_name, sizeof(o_name), i_ptr, ODESC_PREFIX | ODESC_FULL);

	/* Real store */
	if (store.sidx != STORE_HOME)
	{
		/* Extract the value of the items */
		u32b price = price_item(i_ptr, true, amt);

		screen_save();

		/* Show price */
		prt(format("Price: %d", price), 1, 0);

		/* Confirm sale */
		if (!store_get_check(format("Sell %s? [ESC, any other key to accept]", o_name)))
		{
			screen_load();
			return false;
		}

		screen_load();

		cmd_insert(CMD_SELL);
		cmd_set_arg_item(cmd_get_top(), 0, item);
		cmd_set_arg_number(cmd_get_top(), 1, amt);
	}

	/* Player is at home */
	else
	{
		cmd_insert(CMD_STASH);
		cmd_set_arg_item(cmd_get_top(), 0, item);
		cmd_set_arg_number(cmd_get_top(), 1, amt);
	}

	return true;
}


/*
 * Examine an item in a store
 */
static void store_examine(int item)
{
	struct store *store = current_store();
	object_type *o_ptr;

	char header[120];

	textblock *tb;
	region area = { 0, 0, 0, 0 };

	if (item < 0) return;

	/* Get the actual object */
	o_ptr = &store.stock[item];

	/* Show full info in most stores, but normal info in player home */
	tb = object_info(o_ptr, (store.sidx != STORE_HOME) ? OINFO_FULL : OINFO_NONE);
	object_desc(header, sizeof(header), o_ptr, ODESC_PREFIX | ODESC_FULL);

	textui_textblock_show(tb, area, header);
	textblock_free(tb);

	/* Hack -- Browse book, then prompt for a command */
	if (o_ptr.tval == p_ptr.class.spell_book)
		textui_book_browse(o_ptr);
}



/*
 * Process a command in a store
 *
 * Note that we must allow the use of a few "special" commands in the stores
 * which are not allowed in the dungeon, and we must disable some commands
 * which are allowed in the dungeon but not in the stores, to prevent chaos.
 */
static bool store_process_command_key(struct keypress kp)
{
	int cmd = 0;

	/* Process the keycode */
	switch (kp.code) {
		case 'T': /* roguelike */
		case 't': cmd = CMD_TAKEOFF; break;

		case KTRL('D'): /* roguelike */
		case 'k': textui_cmd_destroy(); break;

		case 'P': /* roguelike */
		case 'b': textui_spell_browse(); break;

		case '~': textui_browse_knowledge(); break;
		case 'I': textui_obj_examine(); break;
		case 'w': cmd = CMD_WIELD; break;
		case '{': cmd = CMD_INSCRIBE; break;
		case '}': cmd = CMD_UNINSCRIBE; break;

		case 'e': do_cmd_equip(); break;
		case 'i': do_cmd_inven(); break;
		case KTRL('E'): toggle_inven_equip(); break;
		case 'C': do_cmd_change_name(); break;
		case KTRL('P'): do_cmd_messages(); break;
		case ')': do_cmd_save_screen(); break;

		default: return false;
	}

	if (cmd)
		cmd_insert_repeated(cmd, 0);

	return true;
}



