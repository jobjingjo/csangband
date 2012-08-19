/*
 * File: history.c
 * Purpose: Character auto-history creation, management, and display
 *
 * Copyright (c) 2007 J.D. White
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
#include "history.h"

#define LIMITLOW(a, b) if (a < b) a = b;
#define LIMITHI(a, b) if (a > b) a = b;


/*
 * Return the number of history entries.
 */
size_t history_get_num(void)
{
	return history_ctr;
}


/*
 * Mark artifact number `id` as known.
 */
static bool history_know_artifact(struct artifact *artifact)
{
	size_t i = history_ctr;
	assert(artifact);

	while (i--) {
		if (history_list[i].a_idx == artifact.aidx) {
			history_list[i].type = HISTORY_ARTIFACT_KNOWN;
			return true;
		}
	}

	return false;
}


/*
 * Returns true if the artifact denoted by a_idx is KNOWN in the history log.
 */
bool history_is_artifact_known(struct artifact *artifact)
{
	size_t i = history_ctr;
	assert(artifact);

	while (i--) {
		if (history_list[i].type & HISTORY_ARTIFACT_KNOWN &&
				history_list[i].a_idx == artifact.aidx)
			return true;
	}

	return false;
}


/*
 * Returns true if the artifact denoted by a_idx is an active entry in
 * the history log (i.e. is not marked HISTORY_ARTIFACT_LOST).  This permits
 * proper handling of the case where the player loses an artifact but (in
 * preserve mode) finds it again later.
 */
static bool history_is_artifact_logged(struct artifact *artifact)
{
	size_t i = history_ctr;
	assert(artifact);

	while (i--) {
		/* Don't count ARTIFACT_LOST entries; then we can handle
		 * re-finding previously lost artifacts in preserve mode  */
		if (history_list[i].type & HISTORY_ARTIFACT_LOST)
			continue;

		if (history_list[i].a_idx == artifact.aidx)
			return true;
	}

	return false;
}


/*
 * Convert all ARTIFACT_UNKNOWN history items to HISTORY_ARTIFACT_KNOWN.
 * Use only after player retirement/death for the final character dump.
 */
void history_unmask_unknown(void)
{
	size_t i = history_ctr;

	while (i--)
	{
		if (history_list[i].type & HISTORY_ARTIFACT_UNKNOWN)
		{
			history_list[i].type &= ~(HISTORY_ARTIFACT_UNKNOWN);
			history_list[i].type |= HISTORY_ARTIFACT_KNOWN;
		}
	}
}


/*
 * Used to determine whether the history entry is visible in the listing or not.
 * Returns true if the item is masked -- that is, if it is invisible
 *
 * All artifacts are now sensed on pickup, so nothing is now invisible. The
 * KNOWN / UNKNOWN distinction is if we had fully identified it or not
 */
static bool history_masked(size_t i)
{
	return false;
}

/*
 * Finds the index of the last printable (non-masked) item in the history list.
 */
static size_t last_printable_item(void)
{
	size_t i = history_ctr;

	while (i--)
	{
		if (!history_masked(i))
			break;
	}

	return i;
}

static void print_history_header(void)
{
	char buf[80];

	/* Print the header (character name and title) */
	strnfmt(buf, sizeof(buf), "%s the %s %s",
	        op_ptr.full_name,
	        p_ptr.race.name,
	        p_ptr.class.name);

	c_put_str(TERM_WHITE, buf, 0, 0);
	c_put_str(TERM_WHITE, "============================================================", 1, 0);
	c_put_str(TERM_WHITE, "                   CHAR.  ", 2, 0);
	c_put_str(TERM_WHITE, "|   TURN  | DEPTH |LEVEL| EVENT", 3, 0);
	c_put_str(TERM_WHITE, "============================================================", 4, 0);
}


/* Handles all of the display functionality for the history list. */
void history_display(void)
{
	int row, wid, hgt, page_size;
	char buf[90];
	static size_t first_item = 0;
	size_t max_item = last_printable_item();
	size_t i;

	Term_get_size(&wid, &hgt);

	/* Six lines provide space for the header and footer */
	page_size = hgt - 6;

	screen_save();

	while (1)
	{
		struct keypress ch;

		Term_clear();

		/* Print everything to screen */
		print_history_header();

		row = 0;
		for (i = first_item; row <= page_size && i < history_ctr; i++)
		{
			/* Skip messages about artifacts not yet IDed. */
			if (history_masked(i))
				continue;

			strnfmt(buf, sizeof(buf), "%10d%7d\'%5d   %s",
				history_list[i].turn,
				history_list[i].dlev * 50,
				history_list[i].clev,
				history_list[i].event);

			if (history_list[i].type & HISTORY_ARTIFACT_LOST)
				my_strcat(buf, " (LOST)", sizeof(buf));

			/* Size of header = 5 lines */
			prt(buf, row + 5, 0);
			row++;
		}
		prt("[Arrow keys scroll, p for previous page, n for next page, ESC to exit.]", hgt - 1, 0);

		ch = inkey();

		/* XXXmacro we should have a generic "key . scroll" function */
		if (ch.code == 'n')
		{
			size_t scroll_to = first_item + page_size;

			while (history_masked(scroll_to) && scroll_to < history_ctr - 1)
				scroll_to++;

			first_item = (scroll_to < max_item ? scroll_to : max_item);
		}
		else if (ch.code == 'p')
		{
			int scroll_to = first_item - page_size;

			while (history_masked(scroll_to) && scroll_to > 0)
				scroll_to--;

			first_item = (scroll_to >= 0 ? scroll_to : 0);
		}
		else if (ch.code == ARROW_DOWN)
		{
			size_t scroll_to = first_item + 1;

			while (history_masked(scroll_to) && scroll_to < history_ctr - 1)
				scroll_to++;

			first_item = (scroll_to < max_item ? scroll_to : max_item);
		}
		else if (ch.code == ARROW_UP)
		{
			int scroll_to = first_item - 1;

			while (history_masked(scroll_to) && scroll_to > 0)
				scroll_to--;

			first_item = (scroll_to >= 0 ? scroll_to : 0);
		}
		else if (ch.code == ESCAPE)
			break;
	}

	screen_load();

	return;
}


/* Dump character history to a file, which we assume is already open. */
void dump_history(ang_file *file)
{
	size_t i;
	char buf[90];

	/* We use either ascii or system-specific encoding */
 	int encoding = OPT(xchars_to_file) ? SYSTEM_SPECIFIC : ASCII;

        file_putf(file, "============================================================\n");
        file_putf(file, "                   CHAR.\n");
        file_putf(file, "|   TURN  | DEPTH |LEVEL| EVENT\n");
        file_putf(file, "============================================================\n");

	for (i = 0; i < (last_printable_item() + 1); i++)
	{
		/* Skip not-yet-IDd artifacts */
		if (history_masked(i)) continue;

                strnfmt(buf, sizeof(buf), "%10d%7d\'%5d   %s",
                                history_list[i].turn,
                                history_list[i].dlev * 50,
                                history_list[i].clev,
                                history_list[i].event);

                if (history_list[i].type & HISTORY_ARTIFACT_LOST)
                                my_strcat(buf, " (LOST)", sizeof(buf));

		x_file_putf(file, encoding, "%s", buf);
		file_put(file, "\n");
	}

	return;
}
