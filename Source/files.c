/*
 * File: files.c
 * Purpose: Various file-related activities, poorly organised
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
#include "buildid.h"
#include "cave.h"
#include "cmds.h"
#include "files.h"
#include "game-cmd.h"
#include "history.h"
#include "object/tvalsval.h"
#include "object/pval.h"
#include "option.h"
#include "savefile.h"
#include "ui-menu.h"

/*
 * Hack -- Dump a character description file
 *
 * XXX XXX XXX Allow the "full" flag to dump additional info,
 * and trigger its usage from various places in the code.
 */
errr file_character(const char *path, bool full)
{
	int i, x, y;

	byte a;
	char c;

	ang_file *fp;

	struct store *st_ptr = &stores[STORE_HOME];

	char o_name[80];

	byte (*old_xchar_hook)(byte c) = Term.xchar_hook;

	char buf[1024];

	/* We use either ascii or system-specific encoding */
 	int encoding = OPT(xchars_to_file) ? SYSTEM_SPECIFIC : ASCII;

	/* Unused parameter */
	(void)full;


	/* Open the file for writing */
	fp = file_open(path, MODE_WRITE, FTYPE_TEXT);
	if (!fp) return (-1);

	/* Display the requested encoding -- ASCII or system-specific */
 	if (!OPT(xchars_to_file)) Term.xchar_hook = null;

	/* Begin dump */
	file_putf(fp, "  [%s Character Dump]\n\n", buildid);


	/* Display player */
	display_player(0);

	/* Dump part of the screen */
	for (y = 1; y < 23; y++)
	{
		/* Dump each row */
		for (x = 0; x < 79; x++)
		{
			/* Get the attr/char */
			(void)(Term_what(x, y, &a, &c));

			/* Dump it */
			buf[x] = c;
		}

		/* Back up over spaces */
		while ((x > 0) && (buf[x-1] == ' ')) --x;

		/* Terminate */
		buf[x] = '\0';

		/* End the row */
		x_file_putf(fp, encoding, "%s\n", buf);
	}

	/* Skip a line */
	file_putf(fp, "\n");

	/* Display player */
	display_player(1);

	/* Dump part of the screen */
	for (y = 11; y < 20; y++)
	{
		/* Dump each row */
		for (x = 0; x < 39; x++)
		{
			/* Get the attr/char */
			(void)(Term_what(x, y, &a, &c));

			/* Dump it */
			buf[x] = c;
		}

		/* Back up over spaces */
		while ((x > 0) && (buf[x-1] == ' ')) --x;

		/* Terminate */
		buf[x] = '\0';

		/* End the row */
		x_file_putf(fp, encoding, "%s\n", buf);
	}

	/* Skip a line */
	file_putf(fp, "\n");

	/* Dump part of the screen */
	for (y = 11; y < 20; y++)
	{
		/* Dump each row */
		for (x = 0; x < 39; x++)
		{
			/* Get the attr/char */
			(void)(Term_what(x + 40, y, &a, &c));

			/* Dump it */
			buf[x] = c;
		}

		/* Back up over spaces */
		while ((x > 0) && (buf[x-1] == ' ')) --x;

		/* Terminate */
		buf[x] = '\0';

		/* End the row */
		x_file_putf(fp, encoding, "%s\n", buf);
	}

	/* Skip some lines */
	file_putf(fp, "\n\n");


	/* If dead, dump last messages -- Prfnoff */
	if (p_ptr.is_dead)
	{
		i = messages_num();
		if (i > 15) i = 15;
		file_putf(fp, "  [Last Messages]\n\n");
		while (i-- > 0)
		{
			x_file_putf(fp, encoding, "> %s\n", message_str((s16b)i));
		}
		x_file_putf(fp, encoding, "\nKilled by %s.\n\n", p_ptr.died_from);
	}


	/* Dump the equipment */
	file_putf(fp, "  [Character Equipment]\n\n");
	for (i = INVEN_WIELD; i < ALL_INVEN_TOTAL; i++)
	{
		if (i == INVEN_TOTAL)
		{
			file_putf(fp, "\n\n  [Character Quiver]\n\n");
			continue;
		}
		object_desc(o_name, sizeof(o_name), &p_ptr.inventory[i],
				ODESC_PREFIX | ODESC_FULL);

		x_file_putf(fp, encoding, "%c) %s\n", index_to_label(i), o_name);
		if (p_ptr.inventory[i].kind)
			object_info_chardump(fp, &p_ptr.inventory[i], 5, 72);
	}

	/* Dump the inventory */
	file_putf(fp, "\n\n  [Character Inventory]\n\n");
	for (i = 0; i < INVEN_PACK; i++)
	{
		if (!p_ptr.inventory[i].kind) break;

		object_desc(o_name, sizeof(o_name), &p_ptr.inventory[i],
					ODESC_PREFIX | ODESC_FULL);

		x_file_putf(fp, encoding, "%c) %s\n", index_to_label(i), o_name);
		object_info_chardump(fp, &p_ptr.inventory[i], 5, 72);
	}
	file_putf(fp, "\n\n");


	/* Dump the Home -- if anything there */
	if (st_ptr.stock_num)
	{
		/* Header */
		file_putf(fp, "  [Home Inventory]\n\n");

		/* Dump all available items */
		for (i = 0; i < st_ptr.stock_num; i++)
		{
			object_desc(o_name, sizeof(o_name), &st_ptr.stock[i],
						ODESC_PREFIX | ODESC_FULL);
			x_file_putf(fp, encoding, "%c) %s\n", I2A(i), o_name);

			object_info_chardump(fp, &st_ptr.stock[i], 5, 72);
		}

		/* Add an empty line */
		file_putf(fp, "\n\n");
	}

	/* Dump character history */
	dump_history(fp);
	file_putf(fp, "\n\n");

	/* Dump options */
	file_putf(fp, "  [Options]\n\n");

	/* Dump options */
	for (i = OPT_BIRTH; i < OPT_BIRTH + N_OPTS_BIRTH; i++)
	{
		if (option_name(i))
		{
			file_putf(fp, "%-45s: %s (%s)\n",
			        option_desc(i),
			        op_ptr.opt[i] ? "yes" : "no ",
			        option_name(i));
		}
	}

	/* Skip some lines */
	file_putf(fp, "\n\n");

	/* Return to standard display */
 	Term.xchar_hook = old_xchar_hook;

	file_close(fp);


	/* Success */
	return (0);
}


/*
 * Make a string lower case.
 */
static void string_lower(char *buf)
{
	char *s;

	/* Lowercase the string */
	for (s = buf; *s != 0; s++) *s = tolower((unsigned char)*s);
}


/*
 * Recursive file perusal.
 *
 * Return false on "?", otherwise true.
 *
 * This function could be made much more efficient with the use of "seek"
 * functionality, especially when moving backwards through a file, or
 * forwards through a file by less than a page at a time.  XXX XXX XXX
 */
bool show_file(const char *name, const char *what, int line, int mode)
{
	int i, k, n;

	struct keypress ch;

	/* Number of "real" lines passed by */
	int next = 0;

	/* Number of "real" lines in the file */
	int size;

	/* Backup value for "line" */
	int back = 0;

	/* This screen has sub-screens */
	bool menu = false;

	/* Case sensitive search */
	bool case_sensitive = false;

	/* Current help file */
	ang_file *fff = null;

	/* Find this string (if any) */
	char *find = null;

	/* Jump to this tag */
	const char *tag = null;

	/* Hold a string to find */
	char finder[80] = "";

	/* Hold a string to show */
	char shower[80] = "";

	/* Filename */
	char filename[1024];

	/* Describe this thing */
	char caption[128] = "";

	/* Path buffer */
	char path[1024];

	/* General buffer */
	char buf[1024];

	/* Lower case version of the buffer, for searching */
	char lc_buf[1024];

	/* Sub-menu information */
	char hook[26][32];

	int wid, hgt;



	/* Wipe the hooks */
	for (i = 0; i < 26; i++) hook[i][0] = '\0';

	/* Get size */
	Term_get_size(&wid, &hgt);

	/* Copy the filename */
	my_strcpy(filename, name, sizeof(filename));

	n = strlen(filename);

	/* Extract the tag from the filename */
	for (i = 0; i < n; i++)
	{
		if (filename[i] == '#')
		{
			filename[i] = '\0';
			tag = filename + i + 1;
			break;
		}
	}

	/* Redirect the name */
	name = filename;


	/* Hack XXX XXX XXX */
	if (what)
	{
		my_strcpy(caption, what, sizeof(caption));

		my_strcpy(path, name, sizeof(path));
		fff = file_open(path, MODE_READ, -1);
	}

	/* Look in "help" */
	if (!fff)
	{
		strnfmt(caption, sizeof(caption), "Help file '%s'", name);

		path_build(path, sizeof(path), ANGBAND_DIR_HELP, name);
		fff = file_open(path, MODE_READ, -1);
	}

	/* Look in "info" */
	if (!fff)
	{
		strnfmt(caption, sizeof(caption), "Info file '%s'", name);

		path_build(path, sizeof(path), ANGBAND_DIR_INFO, name);
		fff = file_open(path, MODE_READ, -1);
	}

	/* Oops */
	if (!fff)
	{
		/* Message */
		msg("Cannot open '%s'.", name);
		message_flush();

		/* Oops */
		return (true);
	}


	/* Pre-Parse the file */
	while (true)
	{
		/* Read a line or stop */
		if (!file_getl(fff, buf, sizeof(buf))) break;

		/* XXX Parse "menu" items */
		if (prefix(buf, "***** "))
		{
			char b1 = '[', b2 = ']';

			/* Notice "menu" requests */
			if ((buf[6] == b1) && isalpha((unsigned char)buf[7]) &&
			    (buf[8] == b2) && (buf[9] == ' '))
			{
				/* This is a menu file */
				menu = true;

				/* Extract the menu item */
				k = A2I(buf[7]);

				/* Store the menu item (if valid) */
				if ((k >= 0) && (k < 26))
					my_strcpy(hook[k], buf + 10, sizeof(hook[0]));
			}
			/* Notice "tag" requests */
			else if (buf[6] == '<')
			{
				if (tag)
				{
					/* Remove the closing '>' of the tag */
					buf[strlen(buf) - 1] = '\0';

					/* Compare with the requested tag */
					if (streq(buf + 7, tag))
					{
						/* Remember the tagged line */
						line = next;
					}
				}
			}

			/* Skip this */
			continue;
		}

		/* Count the "real" lines */
		next++;
	}

	/* Save the number of "real" lines */
	size = next;



	/* Display the file */
	while (true)
	{
		/* Clear screen */
		Term_clear();


		/* Restrict the visible range */
		if (line > (size - (hgt - 4))) line = size - (hgt - 4);
		if (line < 0) line = 0;


		/* Re-open the file if needed */
		if (next > line)
		{
			/* Close it */
			file_close(fff);

			/* Hack -- Re-Open the file */
			fff = file_open(path, MODE_READ, -1);
			if (!fff) return (true);

			/* File has been restarted */
			next = 0;
		}


		/* Goto the selected line */
		while (next < line)
		{
			/* Get a line */
			if (!file_getl(fff, buf, sizeof(buf))) break;

			/* Skip tags/links */
			if (prefix(buf, "***** ")) continue;

			/* Count the lines */
			next++;
		}


		/* Dump the next lines of the file */
		for (i = 0; i < hgt - 4; )
		{
			/* Hack -- track the "first" line */
			if (!i) line = next;

			/* Get a line of the file or stop */
			if (!file_getl(fff, buf, sizeof(buf))) break;

			/* Hack -- skip "special" lines */
			if (prefix(buf, "***** ")) continue;

			/* Count the "real" lines */
			next++;

			/* Make a copy of the current line for searching */
			my_strcpy(lc_buf, buf, sizeof(lc_buf));

			/* Make the line lower case */
			if (!case_sensitive) string_lower(lc_buf);

			/* Hack -- keep searching */
			if (find && !i && !strstr(lc_buf, find)) continue;

			/* Hack -- stop searching */
			find = null;

			/* Dump the line */
			Term_putstr(0, i+2, -1, TERM_WHITE, buf);

			/* Highlight "shower" */
			if (shower[0])
			{
				const char *str = lc_buf;

				/* Display matches */
				while ((str = strstr(str, shower)) != null)
				{
					int len = strlen(shower);

					/* Display the match */
					Term_putstr(str-lc_buf, i+2, len, TERM_YELLOW, &buf[str-lc_buf]);

					/* Advance */
					str += len;
				}
			}

			/* Count the printed lines */
			i++;
		}

		/* Hack -- failed search */
		if (find)
		{
			bell("Search string not found!");
			line = back;
			find = null;
			continue;
		}


		/* Show a general "title" */
		prt(format("[%s, %s, Line %d-%d/%d]", buildid,
		           caption, line, line + hgt - 4, size), 0, 0);


		/* Prompt -- menu screen */
		if (menu)
		{
			/* Wait for it */
			prt("[Press a Number, or ESC to exit.]", hgt - 1, 0);
		}

		/* Prompt -- small files */
		else if (size <= hgt - 4)
		{
			/* Wait for it */
			prt("[Press ESC to exit.]", hgt - 1, 0);
		}

		/* Prompt -- large files */
		else
		{
			/* Wait for it */
			prt("[Press Space to advance, or ESC to exit.]", hgt - 1, 0);
		}

		/* Get a keypress */
		ch = inkey();

		/* Exit the help */
		if (ch.code == '?') break;

		/* Toggle case sensitive on/off */
		if (ch.code == '!')
		{
			case_sensitive = !case_sensitive;
		}

		/* Try showing */
		if (ch.code == '&')
		{
			/* Get "shower" */
			prt("Show: ", hgt - 1, 0);
			(void)askfor_aux(shower, sizeof(shower), null);

			/* Make the "shower" lowercase */
			if (!case_sensitive) string_lower(shower);
		}

		/* Try finding */
		if (ch.code == '/')
		{
			/* Get "finder" */
			prt("Find: ", hgt - 1, 0);
			if (askfor_aux(finder, sizeof(finder), null))
			{
				/* Find it */
				find = finder;
				back = line;
				line = line + 1;

				/* Make the "finder" lowercase */
				if (!case_sensitive) string_lower(finder);

				/* Show it */
				my_strcpy(shower, finder, sizeof(shower));
			}
		}

		/* Go to a specific line */
		if (ch.code == '#')
		{
			char tmp[80] = "0";

			prt("Goto Line: ", hgt - 1, 0);
			if (askfor_aux(tmp, sizeof(tmp), null))
				line = atoi(tmp);
		}

		/* Go to a specific file */
		if (ch.code == '%')
		{
			char ftmp[80] = "help.hlp";

			prt("Goto File: ", hgt - 1, 0);
			if (askfor_aux(ftmp, sizeof(ftmp), null))
			{
				if (!show_file(ftmp, null, 0, mode))
					ch.code = ESCAPE;
			}
		}

		switch (ch.code) {
			/* up a line */
			case ARROW_UP:
			case '8': line--; break;

			/* up a page */
			case KC_PGUP:
			case '9':
			case '-': line -= (hgt - 4); break;

			/* home */
			case KC_HOME:
			case '7': line = 0; break;

			/* down a line */
			case ARROW_DOWN:
			case '2':
			case '\n':
			case '\r': line++; break;

			/* down a page */
			case KC_PGDOWN:
			case '3':
			case ' ': line += hgt - 4; break;

			/* end */
			case KC_END:
			case '1': line = size; break;
		}

		/* Recurse on letters */
		if (menu && isalpha((unsigned char)ch.code))
		{
			/* Extract the requested menu item */
			k = A2I(ch.code);

			/* Verify the menu item */
			if ((k >= 0) && (k <= 25) && hook[k][0])
			{
				/* Recurse on that file */
				if (!show_file(hook[k], null, 0, mode)) ch.code = ESCAPE;
			}
		}

		/* Exit on escape */
		if (ch.code == ESCAPE) break;
	}

	/* Close the file */
	file_close(fff);

	/* Done */
	return (ch.code != '?');
}





static void write_html_escape_char(ang_file *fp, char c)
{
	switch (c)
	{
		case '<':
			file_putf(fp, "&lt;");
			break;
		case '>':
			file_putf(fp, "&gt;");
			break;
		case '&':
			file_putf(fp, "&amp;");
			break;
		default:
			file_putf(fp, "%c", c);
			break;
	}
}


/* Take an html screenshot */
void html_screenshot(const char *name, int mode)
{
	int y, x;
	int wid, hgt;

	byte a = TERM_WHITE;
	byte oa = TERM_WHITE;
	char c = ' ';

	const char *new_color_fmt = (mode == 0) ?
					"<font color=\"#%02X%02X%02X\">"
				 	: "[COLOR=\"#%02X%02X%02X\"]";
	const char *change_color_fmt = (mode == 0) ?
					"</font><font color=\"#%02X%02X%02X\">"
					: "[/COLOR][COLOR=\"#%02X%02X%02X\"]";
	const char *close_color_fmt = mode ==  0 ? "</font>" : "[/COLOR]";

	ang_file *fp;
	char buf[1024];


	path_build(buf, sizeof(buf), ANGBAND_DIR_USER, name);
	fp = file_open(buf, MODE_WRITE, FTYPE_TEXT);

	/* Oops */
	if (!fp)
	{
		plog_fmt("Cannot write the '%s' file!", buf);
		return;
	}

	/* Retrieve current screen size */
	Term_get_size(&wid, &hgt);

	if (mode == 0)
	{
		file_putf(fp, "<!DOCTYPE html><html><head>\n");
		file_putf(fp, "  <meta='generator' content='%s'>\n", buildid);
		file_putf(fp, "  <title>%s</title>\n", name);
		file_putf(fp, "</head>\n\n");
		file_putf(fp, "<body style='color: #fff; background: #000;'>\n");
		file_putf(fp, "<pre>\n");
	}
	else 
	{
		file_putf(fp, "[CODE][TT][BC=black][COLOR=white]\n");
	}

	/* Dump the screen */
	for (y = 0; y < hgt; y++)
	{
		for (x = 0; x < wid; x++)
		{
			/* Get the attr/char */
			(void)(Term_what(x, y, &a, &c));

			/* Color change */
			if (oa != a && c != ' ')
			{
				/* From the default white to another color */
				if (oa == TERM_WHITE)
				{
					file_putf(fp, new_color_fmt,
					        angband_color_table[a][1],
					        angband_color_table[a][2],
					        angband_color_table[a][3]);
				}

				/* From another color to the default white */
				else if (a == TERM_WHITE)
				{
					file_putf(fp, close_color_fmt);
				}

				/* Change colors */
				else
				{
					file_putf(fp, change_color_fmt,
					        angband_color_table[a][1],
					        angband_color_table[a][2],
					        angband_color_table[a][3]);
				}

				/* Remember the last color */
				oa = a;
			}

			/* Write the character and escape special HTML characters */
			if (mode == 0) write_html_escape_char(fp, c);
			else file_putf(fp, "%c", c);
		}

		/* End the row */
		file_putf(fp, "\n");
	}

	/* Close the last font-color tag if necessary */
	if (oa != TERM_WHITE) file_putf(fp, close_color_fmt);

	if (mode == 0)
	{
		file_putf(fp, "</pre>\n");
		file_putf(fp, "</body>\n");
		file_putf(fp, "</html>\n");
	}
	else 
	{
		file_putf(fp, "[/COLOR][/BC][/TT][/CODE]\n");
	}

	/* Close it */
	file_close(fp);
}
