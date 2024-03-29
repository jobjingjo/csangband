/*
 * File: ui.c
 * Purpose: Generic ui functions
 *
 * Copyright (c) 2007 Pete Mack and others.
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
#include "z-textblock.h"

/*** Text display ***/

static void display_area(const char *text, const byte *attrs,
		size_t *line_starts, size_t *line_lengths,
		size_t n_lines,
		region area, size_t line_from)
{
	size_t i, j;

	n_lines = MIN(n_lines, (size_t) area.page_rows);

	for (i = 0; i < n_lines; i++) {
		Term_erase(area.col, area.row + i, area.width);
		for (j = 0; j < line_lengths[line_from + i]; j++) {
			Term_putch(area.col + j, area.row + i,
					attrs[line_starts[line_from + i] + j],
					text[line_starts[line_from + i] + j]);
		}
	}
}

void textui_textblock_place(textblock *tb, region orig_area, const char *header)
{
	const char *text = textblock_text(tb);
	const byte *attrs = textblock_attrs(tb);

	/* xxx on resize this should be recalculated */
	region area = region_calculate(orig_area);

	size_t *line_starts = null, *line_lengths = null;
	size_t n_lines;

	n_lines = textblock_calculate_lines(tb,
			&line_starts, &line_lengths, area.width);

	area.page_rows--;

	if (n_lines > (size_t) area.page_rows)
		n_lines = area.page_rows;

	c_prt(TERM_L_BLUE, header, area.row, area.col);
	area.row++;

	display_area(text, attrs, line_starts, line_lengths, n_lines, area, 0);

	mem_free(line_starts);
	mem_free(line_lengths);
}





