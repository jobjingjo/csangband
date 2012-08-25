/*
 * File: target.c
 * Purpose: Targetting code
 *
 * Copyright (c) 1997-2007 Angband contributors
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
#include "game-cmd.h"
#include "monster/mon-lore.h"
#include "monster/mon-util.h"
#include "monster/monster.h"
#include "squelch.h"
#include "target.h"

/*
 * Height of the help screen; any higher than 4 will overlap the health
 * bar which we want to keep in targeting mode.
 */
#define HELP_HEIGHT 3



#define TS_INITIAL_SIZE	20

/*** Functions ***/

/*
 * Monster health description
 */
static void look_mon_desc(char *buf, size_t max, int m_idx)
{
	monster_type *m_ptr = cave_monster(cave, m_idx);
	monster_race *r_ptr = &r_info[m_ptr.r_idx];

	bool living = true;


	/* Determine if the monster is "living" (vs "undead") */
	if (monster_is_unusual(r_ptr)) living = false;


	/* Healthy monsters */
	if (m_ptr.hp >= m_ptr.maxhp)
	{
		/* No damage */
		my_strcpy(buf, (living ? "unhurt" : "undamaged"), max);
	}
	else
	{
		/* Calculate a health "percentage" */
		int perc = 100L * m_ptr.hp / m_ptr.maxhp;

		if (perc >= 60)
			my_strcpy(buf, (living ? "somewhat wounded" : "somewhat damaged"), max);
		else if (perc >= 25)
			my_strcpy(buf, (living ? "wounded" : "damaged"), max);
		else if (perc >= 10)
			my_strcpy(buf, (living ? "badly wounded" : "badly damaged"), max);
		else
			my_strcpy(buf, (living ? "almost dead" : "almost destroyed"), max);
	}

	if (m_ptr.m_timed[MON_TMD_SLEEP]) my_strcat(buf, ", asleep", max);
	if (m_ptr.m_timed[MON_TMD_CONF]) my_strcat(buf, ", confused", max);
	if (m_ptr.m_timed[MON_TMD_FEAR]) my_strcat(buf, ", afraid", max);
	if (m_ptr.m_timed[MON_TMD_STUN]) my_strcat(buf, ", stunned", max);
}







/*
 * Set the target to a location
 */
void target_set_location(int y, int x)
{
	/* Legal target */
	if (in_bounds_fully(y, x))
	{
		/* Save target info */
		target_set = true;
		target_who = 0;
		target_y = y;
		target_x = x;
	}

	/* Clear target */
	else
	{
		/* Reset target info */
		target_set = false;
		target_who = 0;
		target_y = 0;
		target_x = 0;
	}
}





/*
 * Hack -- help "select" a location (see below)
 */
static s16b target_pick(int y1, int x1, int dy, int dx, struct point_set *targets)
{
	int i, v;

	int x2, y2, x3, y3, x4, y4;

	int b_i = -1, b_v = 9999;


	/* Scan the locations */
	for (i = 0; i < point_set_size(targets); i++)
	{
		/* Point 2 */
		x2 = targets.pts[i].x;
		y2 = targets.pts[i].y;

		/* Directed distance */
		x3 = (x2 - x1);
		y3 = (y2 - y1);

		/* Verify quadrant */
		if (dx && (x3 * dx <= 0)) continue;
		if (dy && (y3 * dy <= 0)) continue;

		/* Absolute distance */
		x4 = ABS(x3);
		y4 = ABS(y3);

		/* Verify quadrant */
		if (dy && !dx && (x4 > y4)) continue;
		if (dx && !dy && (y4 > x4)) continue;

		/* Approximate Double Distance */
		v = ((x4 > y4) ? (x4 + x4 + y4) : (y4 + y4 + x4));

		/* Penalize location XXX XXX XXX */

		/* Track best */
		if ((b_i >= 0) && (v >= b_v)) continue;

		/* Track best */
		b_i = i; b_v = v;
	}

	/* Result */
	return (b_i);
}






/*
 * Perform the minimum "whole panel" adjustment to ensure that the given
 * location is contained inside the current panel, and return true if any
 * such adjustment was performed. Optionally accounts for the targeting
 * help window.
 */
static bool adjust_panel_help(int y, int x, bool help)
{
	bool changed = false;

	int j;

	int screen_hgt_main = help ? (Term.hgt - ROW_MAP - 3) 
							   : (Term.hgt - ROW_MAP - 1);

	/* Scan windows */
	for (j = 0; j < ANGBAND_TERM_MAX; j++)
	{
		int wx, wy;
		int screen_hgt, screen_wid;

		term *t = angband_term[j];

		/* No window */
		if (!t) continue;

		/* No relevant flags */
		if ((j > 0) && !(op_ptr.window_flag[j] & PW_MAP)) continue;

		wy = t.offset_y;
		wx = t.offset_x;

		screen_hgt = (j == 0) ? screen_hgt_main : t.hgt;
		screen_wid = (j == 0) ? (Term.wid - COL_MAP - 1) : t.wid;

		/* Bigtile panels need adjustment */
		screen_wid = screen_wid / tile_width;
		screen_hgt = screen_hgt / tile_height;

		/* Adjust as needed */
		while (y >= wy + screen_hgt) wy += screen_hgt / 2;
		while (y < wy) wy -= screen_hgt / 2;

		/* Adjust as needed */
		while (x >= wx + screen_wid) wx += screen_wid / 2;
		while (x < wx) wx -= screen_wid / 2;

		/* Use "modify_panel" */
		if (modify_panel(t, wy, wx)) changed = true;
	}

	return (changed);
}


/*
 * Describe a location relative to the player position.
 * e.g. "12 S 35 W" or "0 N, 33 E" or "0 N, 0 E"
 */
static void coords_desc(char *buf, int size, int y, int x)
{
	const char *east_or_west;
	const char *north_or_south;

	int py = p_ptr.py;
	int px = p_ptr.px;

	if (y > py)
		north_or_south = "S";
	else
		north_or_south = "N";

	if (x < px)
		east_or_west = "W";
	else
		east_or_west = "E";

	strnfmt(buf, size, "%d %s, %d %s",
		ABS(y-py), north_or_south, ABS(x-px), east_or_west);
}

/*
 * Display targeting help at the bottom of the screen.
 */
static void target_display_help(bool monster, bool free)
{
	/* Determine help location */
	int wid, hgt, help_loc;
	Term_get_size(&wid, &hgt);
	help_loc = hgt - HELP_HEIGHT;
	
	/* Clear */
	clear_from(help_loc);

	/* Prepare help hooks */
	text_out_hook = text_out_to_screen;
	text_out_indent = 1;
	Term_gotoxy(1, help_loc);

	/* Display help */
	text_out_c(TERM_L_GREEN, "<dir>");
	text_out(" and ");
	text_out_c(TERM_L_GREEN, "<click>");
	text_out(" look around. '");
	text_out_c(TERM_L_GREEN, "g");
	text_out(" moves to the selection. '");
	text_out_c(TERM_L_GREEN, "p");
	text_out("' selects the player. '");
	text_out_c(TERM_L_GREEN, "q");
	text_out("' exits. '");
	text_out_c(TERM_L_GREEN, "r");
	text_out("' displays details. '");

	if (free)
	{
		text_out_c(TERM_L_GREEN, "m");
		text_out("' restricts to interesting places. ");
	}
	else
	{
		text_out_c(TERM_L_GREEN, "+");
		text_out("' and '");
		text_out_c(TERM_L_GREEN, "-");
		text_out("' cycle through interesting places. '");
		text_out_c(TERM_L_GREEN, "o");
		text_out("' allows free selection. ");
	}
	
	if (monster || free)
	{
		text_out("'");
		text_out_c(TERM_L_GREEN, "t");
		text_out("' targets the current selection.");
	}

	/* Reset */
	text_out_indent = 0;
}

/*
 * Examine a grid, return a keypress.
 *
 * The "mode" argument contains the "TARGET_LOOK" bit flag, which
 * indicates that the "space" key should scan through the contents
 * of the grid, instead of simply returning immediately.  This lets
 * the "look" command get complete information, without making the
 * "target" command annoying.
 *
 * The "info" argument contains the "commands" which should be shown
 * inside the "[xxx]" text.  This string must never be empty, or grids
 * containing monsters will be displayed with an extra comma.
 *
 * Note that if a monster is in the grid, we update both the monster
 * recall info and the health bar info to track that monster.
 *
 * This function correctly handles multiple objects per grid, and objects
 * and terrain features in the same grid, though the latter never happens.
 *
 * This function must handle blindness/hallucination.
 */
static struct keypress target_set_interactive_aux(int y, int x, int mode)
{
	s16b this_o_idx = 0, next_o_idx = 0;

	const char *s1, *s2, *s3;

	bool boring;

	int feat;

	int floor_list[MAX_FLOOR_STACK];
	int floor_num;

	struct keypress query;

	char out_val[256];

	char coords[20];

	/* Describe the square location */
	coords_desc(coords, sizeof(coords), y, x);

	/* Repeat forever */
	while (1)
	{
		/* Paranoia */
		query.code = ' ';

		/* Assume boring */
		boring = true;

		/* Default */
		s1 = "You see ";
		s2 = "";
		s3 = "";


		/* The player */
		if (cave.m_idx[y][x] < 0)
		{
			/* Description */
			s1 = "You are ";

			/* Preposition */
			s2 = "on ";
		}

		/* Hallucination messes things up */
		if (p_ptr.timed[TMD_IMAGE])
		{
			const char *name = "something strange";

			/* Display a message */
			if (p_ptr.wizard)
				strnfmt(out_val, sizeof(out_val), "%s%s%s%s, %s (%d:%d).",
						s1, s2, s3, name, coords, y, x);
			else
				strnfmt(out_val, sizeof(out_val), "%s%s%s%s, %s.",
						s1, s2, s3, name, coords);

			prt(out_val, 0, 0);
			move_cursor_relative(y, x);
			query = inkey();

			/* Stop on everything but "return" */
			if (query.code == '\n' || query.code == '\r')
				continue;

			return query;
		}

		/* Actual monsters */
		if (cave.m_idx[y][x] > 0)
		{
			monster_type *m_ptr = cave_monster(cave, cave.m_idx[y][x]);
			monster_race *r_ptr = &r_info[m_ptr.r_idx];

			/* Visible */
			if (m_ptr.ml && !m_ptr.unaware)
			{
				bool recall = false;

				char m_name[80];

				/* Not boring */
				boring = false;

				/* Get the monster name ("a kobold") */
				monster_desc(m_name, sizeof(m_name), m_ptr, MDESC_IND2);

				/* Hack -- track this monster race */
				monster_race_track(m_ptr.r_idx);

				/* Hack -- health bar for this monster */
				health_track(p_ptr, cave.m_idx[y][x]);

				/* Hack -- handle stuff */
				handle_stuff(p_ptr);

				/* Interact */
				while (1)
				{
					/* Recall */
					if (recall)
					{
						/* Save screen */
						screen_save();

						/* Recall on screen */
						screen_roff(m_ptr.r_idx);

						/* Command */
						query = inkey();

						/* Load screen */
						screen_load();
					}

					/* Normal */
					else
					{
						char buf[80];

						/* Describe the monster */
						look_mon_desc(buf, sizeof(buf), cave.m_idx[y][x]);

						/* Describe, and prompt for recall */
						if (p_ptr.wizard)
						{
							strnfmt(out_val, sizeof(out_val),
									"%s%s%s%s (%s), %s (%d:%d).",
									s1, s2, s3, m_name, buf, coords, y, x);
						}
						else
						{
							strnfmt(out_val, sizeof(out_val),
									"%s%s%s%s (%s), %s.",
									s1, s2, s3, m_name, buf, coords);
						}

						prt(out_val, 0, 0);

						/* Place cursor */
						move_cursor_relative(y, x);

						/* Command */
						query = inkey();
					}

					/* Normal commands */
					if (query.code == 'r')
						recall = !recall;
					else
						break;
				}

				/* Stop on everything but "return"/"space" */
				if (query.code != '\n' && query.code != '\r' && query.code != ' ')
					break;

				/* Sometimes stop at "space" key */
				if ((query.code == ' ') && !(mode & (TARGET_LOOK))) break;

				/* Take account of gender */
				if (rf_has(r_ptr.flags, RF_FEMALE)) s1 = "She is ";
				else if (rf_has(r_ptr.flags, RF_MALE)) s1 = "He is ";
				else s1 = "It is ";

				/* Use a verb */
				s2 = "carrying ";

				/* Scan all objects being carried */
				for (this_o_idx = m_ptr.hold_o_idx; this_o_idx; this_o_idx = next_o_idx)
				{
					char o_name[80];

					object_type *o_ptr;

					/* Get the object */
					o_ptr = object_byid(this_o_idx);

					/* Get the next object */
					next_o_idx = o_ptr.next_o_idx;

					/* Obtain an object description */
					object_desc(o_name, sizeof(o_name), o_ptr,
								ODESC_PREFIX | ODESC_FULL);

					/* Describe the object */
					if (p_ptr.wizard)
					{
						strnfmt(out_val, sizeof(out_val),
								"%s%s%s%s, %s (%d:%d).",
								s1, s2, s3, o_name, coords, y, x);
					}
					/* Disabled since monsters now carry their drops
					else
					{
						strnfmt(out_val, sizeof(out_val),
								"%s%s%s%s, %s.", s1, s2, s3, o_name, coords);
					} */

					prt(out_val, 0, 0);
					move_cursor_relative(y, x);
					query = inkey();

					/* Stop on everything but "return"/"space" */
					if ((query.code != '\n') && (query.code != '\r') && (query.code != ' ')) break;

					/* Sometimes stop at "space" key */
					if ((query.code == ' ') && !(mode & (TARGET_LOOK))) break;

					/* Change the intro */
					s2 = "also carrying ";
				}

				/* Double break */
				if (this_o_idx) break;

				/* Use a preposition */
				s2 = "on ";
			}
		}

		/* Assume not floored */
		floor_num = scan_floor(floor_list, N_ELEMENTS(floor_list), y, x, 0x02);

		/* Scan all marked objects in the grid */
		if ((floor_num > 0) &&
		    (!(p_ptr.timed[TMD_BLIND]) || (y == p_ptr.py && x == p_ptr.px)))
		{
			/* Not boring */
			boring = false;

			track_object(-floor_list[0]);
			handle_stuff(p_ptr);

			/* If there is more than one item... */
			if (floor_num > 1) while (1)
			{
				/* Describe the pile */
				if (p_ptr.wizard)
				{
					strnfmt(out_val, sizeof(out_val),
							"%s%s%sa pile of %d objects, %s (%d:%d).",
							s1, s2, s3, floor_num, coords, y, x);
				}
				else
				{
					strnfmt(out_val, sizeof(out_val),
							"%s%s%sa pile of %d objects, %s.",
							s1, s2, s3, floor_num, coords);
				}

				prt(out_val, 0, 0);
				move_cursor_relative(y, x);
				query = inkey();

				/* Display objects */
				if (query.code == 'r')
				{
					int rdone = 0;
					int pos;
					while (!rdone)
					{
						/* Save screen */
						screen_save();

						/* Display */
						show_floor(floor_list, floor_num, (OLIST_WEIGHT | OLIST_GOLD));

						/* Describe the pile */
						prt(out_val, 0, 0);
						query = inkey();

						/* Load screen */
						screen_load();

						pos = query.code - 'a';
						if (0 <= pos && pos < floor_num)
						{
							track_object(-floor_list[pos]);
							handle_stuff(p_ptr);
							continue;
						}
						rdone = 1;
					}

					/* Now that the user's done with the display loop, let's */
					/* the outer loop over again */
					continue;
				}

				/* Done */
				break;
			}
			/* Only one object to display */
			else
			{

				char o_name[80];

				/* Get the single object in the list */
				object_type *o_ptr = object_byid(floor_list[0]);

				/* Not boring */
				boring = false;

				/* Obtain an object description */
				object_desc(o_name, sizeof(o_name), o_ptr,
							ODESC_PREFIX | ODESC_FULL);

				/* Describe the object */
				if (p_ptr.wizard)
				{
					strnfmt(out_val, sizeof(out_val),
							"%s%s%s%s, %s (%d:%d).",
							s1, s2, s3, o_name, coords, y, x);
				}
				else
				{
					strnfmt(out_val, sizeof(out_val),
							"%s%s%s%s, %s.", s1, s2, s3, o_name, coords);
				}

				prt(out_val, 0, 0);
				move_cursor_relative(y, x);
				query = inkey();

				/* Stop on everything but "return"/"space" */
				if ((query.code != '\n') && (query.code != '\r') && (query.code != ' ')) break;

				/* Sometimes stop at "space" key */
				if ((query.code == ' ') && !(mode & (TARGET_LOOK))) break;

				/* Change the intro */
				s1 = "It is ";

				/* Plurals */
				if (o_ptr.number != 1) s1 = "They are ";

				/* Preposition */
				s2 = "on ";
			}

		}

		/* Double break */
		if (this_o_idx) break;


		/* Feature (apply "mimic") */
		feat = f_info[cave.feat[y][x]].mimic;

		/* Require knowledge about grid, or ability to see grid */
		if (!(cave.info[y][x] & (CAVE_MARK)) && !player_can_see_bold(y,x))
		{
			/* Forget feature */
			feat = FEAT_NONE;
		}

		/* Terrain feature if needed */
		if (boring || (feat > FEAT_INVIS))
		{
			const char *name = f_info[feat].name;

			/* Hack -- handle unknown grids */
			if (feat == FEAT_NONE) name = "unknown grid";

			/* Pick a prefix */
			if (*s2 && (feat >= FEAT_DOOR_HEAD)) s2 = "in ";

			/* Pick proper indefinite article */
			s3 = (is_a_vowel(name[0])) ? "an " : "a ";

			/* Hack -- special introduction for store doors */
			if ((feat >= FEAT_SHOP_HEAD) && (feat <= FEAT_SHOP_TAIL))
			{
				s3 = "the entrance to the ";
			}

			/* Display a message */
			if (p_ptr.wizard)
			{
				strnfmt(out_val, sizeof(out_val),
						"%s%s%s%s, %s (%d:%d).", s1, s2, s3, name, coords, y, x);
			}
			else
			{
				strnfmt(out_val, sizeof(out_val),
						"%s%s%s%s, %s.", s1, s2, s3, name, coords);
			}

			prt(out_val, 0, 0);
			move_cursor_relative(y, x);
			query = inkey();

			/* Stop on everything but "return"/"space" */
			if ((query.code != '\n') && (query.code != '\r') && (query.code != ' ')) break;
		}

		/* Stop on everything but "return" */
		if ((query.code != '\n') && (query.code != '\r')) break;
	}

	/* Keep going */
	return (query);
}





/**
 * Draw a visible path over the squares between (x1,y1) and (x2,y2).
 *
 * The path consists of "*", which are white except where there is a
 * monster, object or feature in the grid.
 *
 * This routine has (at least) three weaknesses:
 * - remembered objects/walls which are no longer present are not shown,
 * - squares which (e.g.) the player has walked through in the dark are
 *   treated as unknown space.
 * - walls which appear strange due to hallucination aren't treated correctly.
 *
 * The first two result from information being lost from the dungeon arrays,
 * which requires changes elsewhere
 */
static int draw_path(u16b path_n, u16b *path_g, char *c, byte *a, int y1, int x1)
{
	int i;
	bool on_screen;

	/* No path, so do nothing. */
	if (path_n < 1) return 0;

	/* The starting square is never drawn, but notice if it is being
     * displayed. In theory, it could be the last such square.
     */
	on_screen = panel_contains(y1, x1);

	/* Draw the path. */
	for (i = 0; i < path_n; i++) {
		byte colour;

		/* Find the co-ordinates on the level. */
		int y = GRID_Y(path_g[i]);
		int x = GRID_X(path_g[i]);

		/*
		 * As path[] is a straight line and the screen is oblong,
		 * there is only section of path[] on-screen.
		 * If the square being drawn is visible, this is part of it.
		 * If none of it has been drawn, continue until some of it
		 * is found or the last square is reached.
		 * If some of it has been drawn, finish now as there are no
		 * more visible squares to draw.
		 */
		 if (panel_contains(y,x)) on_screen = true;
		 else if (on_screen) break;
		 else continue;

	 	/* Find the position on-screen */
		move_cursor_relative(y,x);

		/* This square is being overwritten, so save the original. */
		Term_what(Term.scr.cx, Term.scr.cy, a+i, c+i);

		/* Choose a colour. */
		if (cave.m_idx[y][x] && cave_monster(cave, cave.m_idx[y][x]).ml) {
			/* Visible monsters are red. */
			monster_type *m_ptr = cave_monster(cave, cave.m_idx[y][x]);
			monster_race *r_ptr = &r_info[m_ptr.r_idx];

			/*mimics act as objects*/
			if (rf_has(r_ptr.flags, RF_UNAWARE)) 
				colour = TERM_YELLOW;
			else
				colour = TERM_L_RED;
		}

		else if (cave.o_idx[y][x] && object_byid(cave.o_idx[y][x]).marked)
			/* Known objects are yellow. */
			colour = TERM_YELLOW;

		else if (!cave_floor_bold(y,x) &&
				 ((cave.info[y][x] & (CAVE_MARK)) || player_can_see_bold(y,x)))
			/* Known walls are blue. */
			colour = TERM_BLUE;

		else if (!(cave.info[y][x] & (CAVE_MARK)) && !player_can_see_bold(y,x))
			/* Unknown squares are grey. */
			colour = TERM_L_DARK;

		else
			/* Unoccupied squares are white. */
			colour = TERM_WHITE;

		/* Draw the path segment */
		(void)Term_addch(colour, '*');
	}
	return i;
}


/**
 * Load the attr/char at each point along "path" which is on screen from
 * "a" and "c". This was saved in draw_path().
 */
static void load_path(u16b path_n, u16b *path_g, char *c, byte *a) {
	int i;
	for (i = 0; i < path_n; i++) {
		int y = GRID_Y(path_g[i]);
		int x = GRID_X(path_g[i]);

		if (!panel_contains(y, x)) continue;
		move_cursor_relative(y, x);
		Term_addch(a[i], c[i]);
	}

	Term_fresh();
}

