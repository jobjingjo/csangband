/*
 * File: prefs.c
 * Purpose: Pref file handling code
 *
 * Copyright (c) 2003 Takeshi Mogami, Robert Ruehlmann
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
#include "keymap.h"
#include "prefs.h"
#include "squelch.h"
#include "spells.h"


/*** Pref file saving code ***/

/*
 * Header and footer marker string for pref file dumps
 */
static const char *dump_separator = "#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#";


/*
 * Remove old lines from pref files
 *
 * If you are using setgid, make sure privileges were raised prior
 * to calling this.
 */
static void remove_old_dump(const char *cur_fname, const char *mark)
{
	bool between_marks = false;
	bool changed = false;

	char buf[1024];

	char start_line[1024];
	char end_line[1024];

	char new_fname[1024];

	ang_file *new_file;
	ang_file *cur_file;


	/* Format up some filenames */
	strnfmt(new_fname, sizeof(new_fname), "%s.new", cur_fname);

	/* Work out what we expect to find */
	strnfmt(start_line, sizeof(start_line), "%s begin %s",
			dump_separator, mark);
	strnfmt(end_line,   sizeof(end_line),   "%s end %s",
			dump_separator, mark);



	/* Open current file */
	cur_file = file_open(cur_fname, MODE_READ, -1);
	if (!cur_file) return;

	/* Open new file */
	new_file = file_open(new_fname, MODE_WRITE, FTYPE_TEXT);
	if (!new_file)
	{
		msg("Failed to create file %s", new_fname);
		return;
	}

	/* Loop for every line */
	while (file_getl(cur_file, buf, sizeof(buf)))
	{
		/* If we find the start line, turn on */
		if (!strcmp(buf, start_line))
		{
			between_marks = true;
		}

		/* If we find the finish line, turn off */
		else if (!strcmp(buf, end_line))
		{
			between_marks = false;
			changed = true;
		}

		if (!between_marks)
		{
			/* Copy orginal line */
			file_putf(new_file, "%s\n", buf);
		}
	}

	/* Close files */
	file_close(cur_file);
	file_close(new_file);

	/* If there are changes, move things around */
	if (changed)
	{
		char old_fname[1024];
		strnfmt(old_fname, sizeof(old_fname), "%s.old", cur_fname);

		if (file_move(cur_fname, old_fname))
		{
			file_move(new_fname, cur_fname);
			file_delete(old_fname);
		}
	}

	/* Otherwise just destroy the new file */
	else
	{
		file_delete(new_fname);
	}
}


/*
 * Output the header of a pref-file dump
 */
static void pref_header(ang_file *fff, const char *mark)
{
	/* Start of dump */
	file_putf(fff, "%s begin %s\n", dump_separator, mark);

	file_putf(fff, "# *Warning!*  The lines below are an automatic dump.\n");
	file_putf(fff, "# Don't edit them; changes will be deleted and replaced automatically.\n");
}

/*
 * Output the footer of a pref-file dump
 */
static void pref_footer(ang_file *fff, const char *mark)
{
	file_putf(fff, "# *Warning!*  The lines above are an automatic dump.\n");
	file_putf(fff, "# Don't edit them; changes will be deleted and replaced automatically.\n");

	/* End of dump */
	file_putf(fff, "%s end %s\n", dump_separator, mark);
}


/*
 * Write all current options to a user preference file.
 */
void option_dump(ang_file *fff)
{
	int i, j;

	/* Dump options (skip cheat, adult, score) */
	for (i = 0; i < OPT_CHEAT; i++)
	{
		const char *name = option_name(i);
		if (!name) continue;

		/* Comment */
		file_putf(fff, "# Option '%s'\n", option_desc(i));

		/* Dump the option */
		if (op_ptr.opt[i])
			file_putf(fff, "Y:%s\n", name);
		else
			file_putf(fff, "X:%s\n", name);

		/* Skip a line */
		file_putf(fff, "\n");
	}

	/* Dump window flags */
	for (i = 1; i < ANGBAND_TERM_MAX; i++)
	{
		/* Require a real window */
		if (!angband_term[i]) continue;

		/* Check each flag */
		for (j = 0; j < (int)N_ELEMENTS(window_flag_desc); j++)
		{
			/* Require a real flag */
			if (!window_flag_desc[j]) continue;

			/* Comment */
			file_putf(fff, "# Window '%s', Flag '%s'\n",
				angband_term_name[i], window_flag_desc[j]);

			/* Dump the flag */
			if (op_ptr.window_flag[i] & (1L << j))
				file_putf(fff, "W:%d:%d:1\n", i, j);
			else
				file_putf(fff, "W:%d:%d:0\n", i, j);

			/* Skip a line */
			file_putf(fff, "\n");
		}
	}

	keymap_dump(fff);
}




/* Dump monsters */
void dump_monsters(ang_file *fff)
{
	int i;

	for (i = 0; i < z_info.r_max; i++)
	{
		monster_race *r_ptr = &r_info[i];
		byte attr = r_ptr.x_attr;
		byte chr = r_ptr.x_char;

		/* Skip non-entries */
		if (!r_ptr.name) continue;

		file_putf(fff, "# Monster: %s\n", r_ptr.name);
		file_putf(fff, "R:%d:%d:%d\n", i, attr, chr);
	}
}

/* Dump objects */
void dump_objects(ang_file *fff)
{
	int i;

	file_putf(fff, "# Objects\n");

	for (i = 1; i < z_info.k_max; i++)
	{
		object_kind *k_ptr = &k_info[i];
		const char *name = k_ptr.name;

		if (!name) continue;
		if (name[0] == '&' && name[1] == ' ')
			name += 2;

		file_putf(fff, "K:%s:%s:%d:%d\n", tval_find_name(k_ptr.tval),
				name, k_ptr.x_attr, k_ptr.x_char);
	}
}

/* Dump features */
void dump_features(ang_file *fff)
{
	int i;

	for (i = 0; i < z_info.f_max; i++)
	{
		feature_type *f_ptr = &f_info[i];
		size_t j;

		/* Skip non-entries */
		if (!f_ptr.name) continue;

		/* Skip mimic entries -- except invisible trap */
		if ((f_ptr.mimic != i) && (i != FEAT_INVIS)) continue;

		file_putf(fff, "# Terrain: %s\n", f_ptr.name);
		for (j = 0; j < FEAT_LIGHTING_MAX; j++)
		{
			byte attr = f_ptr.x_attr[j];
			byte chr = f_ptr.x_char[j];

			const char *light = null;
			if (j == FEAT_LIGHTING_BRIGHT)
				light = "bright";
			else if (j == FEAT_LIGHTING_LIT)
				light = "lit";
			else if (j == FEAT_LIGHTING_DARK)
				light = "dark";

			assert(light);

			file_putf(fff, "F:%d:%s:%d:%d\n", i, light, attr, chr);
		}
	}
}

/* Dump flavors */
void dump_flavors(ang_file *fff)
{
	struct flavor *f;

	for (f = flavors; f; f = f.next) {
		byte attr = f.x_attr;
		byte chr = f.x_char;

		file_putf(fff, "# Item flavor: %s\n", f.text);
		file_putf(fff, "L:%d:%d:%d\n\n", f.fidx, attr, chr);
	}
}

/* Dump colors */
void dump_colors(ang_file *fff)
{
	int i;

	for (i = 0; i < MAX_COLORS; i++)
	{
		int kv = angband_color_table[i][0];
		int rv = angband_color_table[i][1];
		int gv = angband_color_table[i][2];
		int bv = angband_color_table[i][3];

		const char *name = "unknown";

		/* Skip non-entries */
		if (!kv && !rv && !gv && !bv) continue;

		/* Extract the color name */
		if (i < BASIC_COLORS) name = color_table[i].name;

		file_putf(fff, "# Color: %s\n", name);
		file_putf(fff, "V:%d:%d:%d:%d:%d\n\n", i, kv, rv, gv, bv);
	}
}




/**
 * Save a set of preferences to file, overwriting any old preferences with the
 * same title.
 *
 * \param path is the filename to dump to
 * \param dump is a pointer to the function that does the writing to file
 * \param title is the name of this set of preferences
 *
 * \returns true on success, false otherwise.
 */
bool prefs_save(const char *path, void (*dump)(ang_file *), const char *title)
{
	ang_file *fff;

	safe_setuid_grab();

	/* Remove old keymaps */
	remove_old_dump(path, title);

	fff = file_open(path, MODE_APPEND, FTYPE_TEXT);
	if (!fff) {
		safe_setuid_drop();
		return false;
	}

	/* Append the header */
	pref_header(fff, title);
	file_putf(fff, "\n\n");
	file_putf(fff, "# %s definitions\n\n", strstr(title, " "));

	dump(fff);

	file_putf(fff, "\n\n\n");
	pref_footer(fff, title);
	file_close(fff);

	safe_setuid_drop();

	return true;
}

errr process_pref_file_command(const char *s)
{
	struct parser *p = init_parse_prefs(true);
	errr e = parser_parse(p, s);
	mem_free(parser_priv(p));
	parser_destroy(p);
	return e;
}


static void print_error(const char *name, struct parser *p) {
	struct parser_state s;
	parser_getstate(p, &s);
	msg("Parse error in %s line %d column %d: %s: %s", name,
	           s.line, s.col, s.msg, parser_error_str[s.error]);
	message_flush();
}
