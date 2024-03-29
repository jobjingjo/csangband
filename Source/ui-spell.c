/*
 * File: ui-spell.c
 * Purpose: Spell UI handing
 *
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

#include "cave.h"
#include "object/tvalsval.h"
#include "game-cmd.h"
#include "spells.h"

#include "ui.h"
#include "ui-menu.h"


/**
 * Spell menu data struct
 */
struct spell_menu_data {
	int spells[PY_MAX_SPELLS];
	int n_spells;

	bool browse;
	bool (*is_valid)(int spell);

	int selected_spell;
};


/**
 * Is item oid valid?
 */
static int spell_menu_valid(menu_type *m, int oid)
{
	struct spell_menu_data *d = menu_priv(m);
	int *spells = d.spells;

	return d.is_valid(spells[oid]);
}

/**
 * Display a row of the spell menu
 */
static void spell_menu_display(menu_type *m, int oid, bool cursor,
		int row, int col, int wid)
{
	struct spell_menu_data *d = menu_priv(m);
	int spell = d.spells[oid];
	const magic_type *s_ptr = &p_ptr.class.spells.info[spell];

	char help[30];
	char out[80];

	int attr;
	const char *illegible = null;
	const char *comment = null;

	if (s_ptr.slevel >= 99) {
		illegible = "(illegible)";
		attr = TERM_L_DARK;
	} else if (p_ptr.spell_flags[spell] & PY_SPELL_FORGOTTEN) {
		comment = " forgotten";
		attr = TERM_YELLOW;
	} else if (p_ptr.spell_flags[spell] & PY_SPELL_LEARNED) {
		if (p_ptr.spell_flags[spell] & PY_SPELL_WORKED) {
			/* Get extra info */
			get_spell_info(p_ptr.class.spell_book, spell, help, sizeof(help));
			comment = help;
			attr = TERM_WHITE;
		} else {
			comment = " untried";
			attr = TERM_L_GREEN;
		}
	} else if (s_ptr.slevel <= p_ptr.lev) {
		comment = " unknown";
		attr = TERM_L_BLUE;
	} else {
		comment = " difficult";
		attr = TERM_RED;
	}

	/* Dump the spell --(-- */
	strnfmt(out, sizeof(out), "%-30s%2d %4d %3d%%%s",
			get_spell_name(p_ptr.class.spell_book, spell),
			s_ptr.slevel, s_ptr.smana, spell_chance(spell), comment);
	c_prt(attr, illegible ? illegible : out, row, col);
}

/**
 * Handle an event on a menu row.
 */
static bool spell_menu_handler(menu_type *m, const ui_event *e, int oid)
{
	struct spell_menu_data *d = menu_priv(m);

	if (e.type == EVT_SELECT) {
		d.selected_spell = d.spells[oid];
		return d.browse ? true : false;
	}

	return true;
}

/**
 * Show spell long description when browsing
 */
static void spell_menu_browser(int oid, void *data, const region *loc)
{
	struct spell_menu_data *d = data;
	int spell = d.spells[oid];

	/* Redirect output to the screen */
	text_out_hook = text_out_to_screen;
	text_out_wrap = 0;
	text_out_indent = loc.col - 1;
	text_out_pad = 1;

	Term_gotoxy(loc.col, loc.row + loc.page_rows);
	text_out("\n%s\n", s_info[(p_ptr.class.spell_book == TV_MAGIC_BOOK) ? spell : spell + PY_MAX_SPELLS].text);

	/* XXX */
	text_out_pad = 0;
	text_out_indent = 0;
}

static const menu_iter spell_menu_iter = {
	null,	/* get_tag = null, just use lowercase selections */
	spell_menu_valid,
	spell_menu_display,
	spell_menu_handler,
	null	/* no resize hook */
};

/** Create and initialise a spell menu, given an object and a validity hook */
static menu_type *spell_menu_new(const object_type *o_ptr,
		bool (*is_valid)(int spell))
{
	menu_type *m = menu_new(MN_SKIN_SCROLL, &spell_menu_iter);
	struct spell_menu_data *d = mem_alloc(sizeof *d);

	region loc = { -60, 1, 60, -99 };

	/* collect spells from object */
	d.n_spells = spell_collect_from_book(o_ptr, d.spells);
	if (d.n_spells == 0 || !spell_okay_list(is_valid, d.spells, d.n_spells))
	{
		mem_free(m);
		mem_free(d);
		return null;
	}

	/* copy across private data */
	d.is_valid = is_valid;
	d.selected_spell = -1;
	d.browse = false;

	menu_setpriv(m, d.n_spells, d);

	/* set flags */
	m.header = "Name                             Lv Mana Fail Info";
	m.flags = MN_CASELESS_TAGS;
	m.selections = lower_case;
	m.browse_hook = spell_menu_browser;

	/* set size */
	loc.page_rows = d.n_spells + 1;
	menu_layout(m, &loc);

	return m;
}

/** Clean up a spell menu instance */
static void spell_menu_destroy(menu_type *m)
{
	struct spell_menu_data *d = menu_priv(m);
	mem_free(d);
	mem_free(m);
}

/**
 * Run the spell menu to select a spell.
 */
static int spell_menu_select(menu_type *m, const char *noun, const char *verb)
{
	struct spell_menu_data *d = menu_priv(m);

	screen_save();

	region_erase_bordered(&m.active);
	prt(format("%^s which %s? ", verb, noun), 0, 0);

	menu_select(m, 0, true);

	screen_load();

	return d.selected_spell;
}

/**
 * Run the spell menu, without selections.
 */
static void spell_menu_browse(menu_type *m, const char *noun)
{
	struct spell_menu_data *d = menu_priv(m);

	screen_save();

	region_erase_bordered(&m.active);
	prt(format("Browsing %ss.  Press Escape to exit.", noun), 0, 0);

	d.browse = true;
	menu_select(m, 0, true);

	screen_load();
}


/**
 * Interactively select a spell.
 *
 * Returns the spell selected, or -1.
 */
static int get_spell(const object_type *o_ptr, const char *verb,
		bool (*spell_test)(int spell))
{
	menu_type *m;
	const char *noun = (p_ptr.class.spell_book == TV_MAGIC_BOOK ?
			"spell" : "prayer");

	m = spell_menu_new(o_ptr, spell_test);
	if (m) {
		int spell = spell_menu_select(m, noun, verb);
		spell_menu_destroy(m);
		return spell;
	}

	return -1;
}