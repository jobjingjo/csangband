/*
 * File: ui-menu.c
 * Purpose: Generic menu interaction functions
 *
 * Copyright (c) 2007 Pete Mack
 * Copyright (c) 2010 Andi Sidwell
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
#include "ui-event.h"
#include "ui-menu.h"

/* forward declarations */
static bool menu_calc_size(menu_type *menu);
static bool is_valid_row(menu_type *menu, int cursor);


/* Display an event, with possible preference overrides */
static void display_action_aux(menu_action *act, byte color, int row, int col, int wid)
{
	/* TODO: add preference support */
	/* TODO: wizard mode should show more data */
	Term_erase(col, row, wid);

	if (act.name)
		Term_putstr(col, row, wid, color, act.name);
}


/* ================== GENERIC HELPER FUNCTIONS ============== */


/* ================== MENU ACCESSORS ================ */


void menu_set_filter(menu_type *menu, const int filter_list[], int n)
{
	menu.filter_list = filter_list;
	menu.filter_count = n;

	menu_ensure_cursor_valid(menu);
}

void menu_release_filter(menu_type *menu)
{
	menu.filter_list = null;
	menu.filter_count = 0;

	menu_ensure_cursor_valid(menu);

}

/* ======================== MENU INITIALIZATION ==================== */

menu_type *menu_new_action(menu_action *acts, size_t n)
{
	menu_type *m = menu_new(MN_SKIN_SCROLL, menu_find_iter(MN_ITER_ACTIONS));
	menu_setpriv(m, n, acts);
	return m;
}


/*** Dynamic menu handling ***/

struct menu_entry {
	char *text;
	int value;

	struct menu_entry *next;
};

static void dynamic_display(menu_type *m, int oid, bool cursor,
		int row, int col, int width)
{
	struct menu_entry *entry;
	byte color = curs_attrs[CURS_KNOWN][0 != cursor];

	for (entry = menu_priv(m); oid; oid--) {
		entry = entry.next;
		assert(entry);
	}

	Term_putstr(col, row, width, color, entry.text);
}

static const menu_iter dynamic_iter = {
	null,	/* tag */
	null,	/* valid */
	dynamic_display,
	null,	/* handler */
	null	/* resize */
};

menu_type *menu_dynamic_new(void)
{
	menu_type *m = menu_new(MN_SKIN_SCROLL, &dynamic_iter);
	menu_setpriv(m, 0, null);
	return m;
}

void menu_dynamic_add(menu_type *m, const char *text, int value)
{
	struct menu_entry *head = menu_priv(m);
	struct menu_entry *new = mem_zalloc(sizeof *new);

	assert(m.row_funcs == &dynamic_iter);

	new.text = string_make(text);
	new.value = value;

	if (head) {
		struct menu_entry *tail = head;
		while (1) {
			if (tail.next)
				tail = tail.next;
			else
				break;
		}

		tail.next = new;
		menu_setpriv(m, m.count + 1, head);
	} else {
		menu_setpriv(m, m.count + 1, new);
	}
}

size_t menu_dynamic_longest_entry(menu_type *m)
{
	size_t biggest = 0;
	size_t current;

	struct menu_entry *entry;

	for (entry = menu_priv(m); entry; entry = entry.next) {
		current = strlen(entry.text);
		if (current > biggest)
			biggest = current;
	}

	return biggest;
}

int menu_dynamic_select(menu_type *m)
{
	ui_event e = menu_select(m, 0, true);
	struct menu_entry *entry;
	int cursor = m.cursor;

	if (e.type == EVT_ESCAPE)
		return -1;

	for (entry = menu_priv(m); cursor; cursor--) {
		entry = entry.next;
		assert(entry);
	}	

	return entry.value;
}

void menu_dynamic_free(menu_type *m)
{
	struct menu_entry *entry = menu_priv(m);
	if (entry) {
		struct menu_entry *next = entry.next;
		string_free(entry.text);
		mem_free(entry);
		entry = next;
	}
	mem_free(m);
}

