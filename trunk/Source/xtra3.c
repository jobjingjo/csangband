/*
 * File: xtra3.c
 * Purpose: Handles the setting up updating, and cleaning up of the various
 *          things that are displayed by the game.
 *
 * Copyright (c) 1997 Ben Harrison, James E. Wilson, Robert A. Koeneke
 * Copyright (c) 2007 Antony Sidwell
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
#include "buildid.h"
#include "button.h"
#include "cave.h"
#include "files.h"
#include "game-event.h"
#include "game-cmd.h"
#include "monster/mon-lore.h"
#include "monster/mon-util.h"
#include "monster/monster.h"
#include "object/tvalsval.h"
#include "textui.h"
#include "ui-birth.h"


/* ------------------------------------------------------------------------
 * Subwindow displays
 * ------------------------------------------------------------------------ */

/* 
 * true when we're supposed to display the equipment in the inventory 
 * window, or vice-versa.
 */
static bool flip_inven;

static void update_inven_subwindow(game_event_type type, game_event_data *data,
				       void *user)
{
	term *old = Term;
	term *inv_term = user;

	/* Activate */
	Term_activate(inv_term);

	if (!flip_inven)
		show_inven(OLIST_WINDOW | OLIST_WEIGHT | OLIST_QUIVER);
	else
		show_equip(OLIST_WINDOW | OLIST_WEIGHT);

	Term_fresh();
	
	/* Restore */
	Term_activate(old);
}

static void update_equip_subwindow(game_event_type type, game_event_data *data,
				   void *user)
{
	term *old = Term;
	term *inv_term = user;

	/* Activate */
	Term_activate(inv_term);

	if (!flip_inven)
		show_equip(OLIST_WINDOW | OLIST_WEIGHT);
	else
		show_inven(OLIST_WINDOW | OLIST_WEIGHT | OLIST_QUIVER);

	Term_fresh();
	
	/* Restore */
	Term_activate(old);
}

static void update_itemlist_subwindow(game_event_type type, game_event_data *data, void *user)
{
	term *old = Term;
	term *inv_term = user;

	/* Activate */
	Term_activate(inv_term);

	display_itemlist();
	Term_fresh();
	
	/* Restore */
	Term_activate(old);
}

static void update_monlist_subwindow(game_event_type type, game_event_data *data, void *user)
{
	term *old = Term;
	term *inv_term = user;

	/* Activate */
	Term_activate(inv_term);

	display_monlist();
	Term_fresh();
	
	/* Restore */
	Term_activate(old);
}


static void update_monster_subwindow(game_event_type type, game_event_data *data, void *user)
{
	term *old = Term;
	term *inv_term = user;

	/* Activate */
	Term_activate(inv_term);

	/* Display monster race info */
	if (p_ptr.monster_race_idx)
		display_roff(p_ptr.monster_race_idx);

	Term_fresh();
	
	/* Restore */
	Term_activate(old);
}


static void update_object_subwindow(game_event_type type, game_event_data *data, void *user)
{
	term *old = Term;
	term *inv_term = user;
	
	/* Activate */
	Term_activate(inv_term);
	
	if (p_ptr.object_idx != NO_OBJECT)
		display_object_idx_recall(p_ptr.object_idx);
	else if(p_ptr.object_kind_idx != NO_OBJECT)
		display_object_kind_recall(p_ptr.object_kind_idx);
	Term_fresh();
	
	/* Restore */
	Term_activate(old);
}


static void update_messages_subwindow(game_event_type type, game_event_data *data, void *user)
{
	term *old = Term;
	term *inv_term = user;

	int i;
	int w, h;
	int x, y;

	const char *msg;

	/* Activate */
	Term_activate(inv_term);

	/* Get size */
	Term_get_size(&w, &h);

	/* Dump messages */
	for (i = 0; i < h; i++)
	{
		byte color = message_color(i);
		u16b count = message_count(i);
		const char *str = message_str(i);

		if (count == 1)
			msg = str;
		else
			msg = format("%s <%dx>", str, count);

		Term_putstr(0, (h - 1) - i, -1, color, msg);


		/* Cursor */
		Term_locate(&x, &y);

		/* Clear to end of line */
		Term_erase(x, y, 255);
	}

	Term_fresh();
	
	/* Restore */
	Term_activate(old);
}

static struct minimap_flags
{
	int win_idx;
	bool needs_redraw;
} minimap_data[ANGBAND_TERM_MAX];

static void update_minimap_subwindow(game_event_type type,
	game_event_data *data, void *user)
{
	struct minimap_flags *flags = user;

	if (type == EVENT_END) {
		term *old = Term;
		term *t = angband_term[flags.win_idx];
		
		/* Activate */
		Term_activate(t);

		/* If whole-map redraw, clear window first. */
		if (flags.needs_redraw)
			Term_clear();

		/* Redraw map */
		display_map(null, null);
		Term_fresh();
		
		/* Restore */
		Term_activate(old);

		flags.needs_redraw = false;
	}
}


/*
 * Hack -- display player in sub-windows (mode 0)
 */
static void update_player0_subwindow(game_event_type type, game_event_data *data, void *user)
{
	term *old = Term;
	term *inv_term = user;

	/* Activate */
	Term_activate(inv_term);

	/* Display flags */
	display_player(0);

	Term_fresh();
	
	/* Restore */
	Term_activate(old);
}

/*
 * Hack -- display player in sub-windows (mode 1)
 */
static void update_player1_subwindow(game_event_type type, game_event_data *data, void *user)
{
	term *old = Term;
	term *inv_term = user;

	/* Activate */
	Term_activate(inv_term);

	/* Display flags */
	display_player(1);

	Term_fresh();
	
	/* Restore */
	Term_activate(old);
}


/*
 * Display the left-hand-side of the main term, in more compact fashion.
 */
static void update_player_compact_subwindow(game_event_type type, game_event_data *data, void *user)
{
	int row = 0;
	int col = 0;
	int i;

	term *old = Term;
	term *inv_term = user;

	/* Activate */
	Term_activate(inv_term);

	/* Race and Class */
	prt_field(p_ptr.race.name, row++, col);
	prt_field(p_ptr.class.name, row++, col);

	/* Title */
	prt_title(row++, col);

	/* Level/Experience */
	prt_level(row++, col);
	prt_exp(row++, col);

	/* Gold */
	prt_gold(row++, col);

	/* Equippy chars */
	prt_equippy(row++, col);

	/* All Stats */
	for (i = 0; i < A_MAX; i++) prt_stat(i, row++, col);

	/* Empty row */
	row++;

	/* Armor */
	prt_ac(row++, col);

	/* Hitpoints */
	prt_hp(row++, col);

	/* Spellpoints */
	prt_sp(row++, col);

	/* Monster health */
	prt_health(row++, col);

	Term_fresh();
	
	/* Restore */
	Term_activate(old);
}


static void flush_subwindow(game_event_type type, game_event_data *data, void *user)
{
	term *old = Term;
	term *t = user;

	/* Activate */
	Term_activate(t);

	Term_fresh();
	
	/* Restore */
	Term_activate(old);
}


static void subwindow_flag_changed(int win_idx, u32b flag, bool new_state)
{
	void (*register_or_deregister)(game_event_type type, game_event_handler *fn, void *user);
	void (*set_register_or_deregister)(game_event_type *type, size_t n_events, game_event_handler *fn, void *user);

	/* Decide whether to register or deregister an evenrt handler */
	if (new_state == false)
	{
		register_or_deregister = event_remove_handler;
		set_register_or_deregister = event_remove_handler_set;
	}
	else
	{
		register_or_deregister = event_add_handler;
		set_register_or_deregister = event_add_handler_set;
	}

	switch (flag)
	{
		case PW_INVEN:
		{
			register_or_deregister(EVENT_INVENTORY,
					       update_inven_subwindow,
					       angband_term[win_idx]);
			break;
		}

		case PW_EQUIP:
		{
			register_or_deregister(EVENT_EQUIPMENT,
					       update_equip_subwindow,
					       angband_term[win_idx]);
			break;
		}

		case PW_PLAYER_0:
		{
			set_register_or_deregister(player_events, 
						   N_ELEMENTS(player_events),
						   update_player0_subwindow,
						   angband_term[win_idx]);
			break;
		}

		case PW_PLAYER_1:
		{
			set_register_or_deregister(player_events, 
						   N_ELEMENTS(player_events),
						   update_player1_subwindow,
						   angband_term[win_idx]);
			break;
		}

		case PW_PLAYER_2:
		{
			set_register_or_deregister(player_events, 
						   N_ELEMENTS(player_events),
						   update_player_compact_subwindow,
						   angband_term[win_idx]);
			break;
		}

		case PW_MAP:
		{
			register_or_deregister(EVENT_MAP,
					       update_maps,
					       angband_term[win_idx]);

			register_or_deregister(EVENT_END,
					       flush_subwindow,
					       angband_term[win_idx]);
			break;
		}


		case PW_MESSAGE:
		{
			register_or_deregister(EVENT_MESSAGE,
					       update_messages_subwindow,
					       angband_term[win_idx]);
			break;
		}

		case PW_OVERHEAD:
		{
			minimap_data[win_idx].win_idx = win_idx;

			register_or_deregister(EVENT_MAP,
					       update_minimap_subwindow,
					       &minimap_data[win_idx]);

			register_or_deregister(EVENT_END,
					       update_minimap_subwindow,
					       &minimap_data[win_idx]);
			break;
		}

		case PW_MONSTER:
		{
			register_or_deregister(EVENT_MONSTERTARGET,
					       update_monster_subwindow,
					       angband_term[win_idx]);
			break;
		}

		case PW_OBJECT:
		{
			register_or_deregister(EVENT_OBJECTTARGET,
						   update_object_subwindow,
						   angband_term[win_idx]);
			break;
		}

		case PW_MONLIST:
		{
			register_or_deregister(EVENT_MONSTERLIST,
					       update_monlist_subwindow,
					       angband_term[win_idx]);
			break;
		}

		case PW_ITEMLIST:
		{
			register_or_deregister(EVENT_ITEMLIST,
						   update_itemlist_subwindow,
						   angband_term[win_idx]);
			break;
	}
}
}


/*
 * Set the flags for one Term, calling "subwindow_flag_changed" with each flag that
 * has changed setting so that it can do any housekeeping to do with 
 * siaplying hte new thing or no longer displaying the old one.
 */
static void subwindow_set_flags(int win_idx, u32b new_flags)
{
	term *old = Term;
	int i;

	/* Deal with the changed flags by seeing what's changed */
	for (i = 0; i < 32; i++)
	{
		/* Only process valid flags */
		if (window_flag_desc[i])
		{
			if ((new_flags & (1L << i)) != (op_ptr.window_flag[win_idx] & (1L << i)))
			{
				subwindow_flag_changed(win_idx, (1L << i), (new_flags & (1L << i)) != 0);
			}
		}
	}

	/* Store the new flags */
	op_ptr.window_flag[win_idx] = new_flags;
	
	/* Activate */
	Term_activate(angband_term[win_idx]);
	
	/* Erase */
	Term_clear();
	
	/* Refresh */
	Term_fresh();
			
	/* Restore */
	Term_activate(old);
}

/* ------------------------------------------------------------------------
 * Showing and updating the splash screen.
 * ------------------------------------------------------------------------ */
/*
 * Hack -- Explain a broken "lib" folder and quit (see below).
 */
static void init_angband_aux(const char *why)
{
	quit_fmt("%s\n\n%s", why,
	         "The 'lib' directory is probably missing or broken.\n"
	         "Perhaps the archive was not extracted correctly.\n"
	         "See the 'readme.txt' file for more information.");
}

extern game_event_handler ui_enter_birthscreen;

/* Return a random hint from the global hints list */
char* random_hint(void)
{
	struct hint *v, *r = null;
	int n;
	for (v = hints, n = 1; v; v = v.next, n++)
		if (one_in_(n))
			r = v;
	return r.hint;
}
