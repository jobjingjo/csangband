/*
 * File: xtra2.c
 * Purpose: Targetting, sorting, panel update
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
#include "cmds.h"
#include "keymap.h"
#include "history.h"
#include "object/tvalsval.h"
#include "pathfind.h"
#include "spells.h"
#include "target.h"

/* Private function that is shared by verify_panel() and center_panel() */
void verify_panel_int(bool centered);


/*
 * Perform the minimum "whole panel" adjustment to ensure that the given
 * location is contained inside the current panel, and return true if any
 * such adjustment was performed.
 */
bool adjust_panel(int y, int x)
{
	bool changed = false;

	int j;

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

		screen_hgt = (j == 0) ? SCREEN_HGT : t.hgt;
		screen_wid = (j == 0) ? SCREEN_WID : t.wid;

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
 * Change the current panel to the panel lying in the given direction.
 *
 * Return true if the panel was changed.
 */
bool change_panel(int dir)
{
	bool changed = false;
	int j;

	/* Scan windows */
	for (j = 0; j < ANGBAND_TERM_MAX; j++)
	{
		int screen_hgt, screen_wid;
		int wx, wy;

		term *t = angband_term[j];

		/* No window */
		if (!t) continue;

		/* No relevant flags */
		if ((j > 0) && !(op_ptr.window_flag[j] & PW_MAP)) continue;

		screen_hgt = (j == 0) ? SCREEN_HGT : t.hgt;
		screen_wid = (j == 0) ? SCREEN_WID : t.wid;

		/* Shift by half a panel */
		wy = t.offset_y + ddy[dir] * screen_hgt / 2;
		wx = t.offset_x + ddx[dir] * screen_wid / 2;

		/* Use "modify_panel" */
		if (modify_panel(t, wy, wx)) changed = true;
	}

	return (changed);
}


/*
 * Given a "source" and "target" location, extract a "direction",
 * which will move one step from the "source" towards the "target".
 *
 * Note that we use "diagonal" motion whenever possible.
 *
 * We return "5" if no motion is needed.
 */
int motion_dir(int y1, int x1, int y2, int x2)
{
	/* No movement required */
	if ((y1 == y2) && (x1 == x2)) return (5);

	/* South or North */
	if (x1 == x2) return ((y1 < y2) ? 2 : 8);

	/* East or West */
	if (y1 == y2) return ((x1 < x2) ? 6 : 4);

	/* South-east or South-west */
	if (y1 < y2) return ((x1 < x2) ? 3 : 1);

	/* North-east or North-west */
	if (y1 > y2) return ((x1 < x2) ? 9 : 7);

	/* Paranoia */
	return (5);
}




/*
 * Get an "aiming direction" (1,2,3,4,6,7,8,9 or 5) from the user.
 *
 * Return true if a direction was chosen, otherwise return false.
 *
 * The direction "5" is special, and means "use current target".
 *
 * This function tracks and uses the "global direction", and uses
 * that as the "desired direction", if it is set.
 *
 * Note that "Force Target", if set, will pre-empt user interaction,
 * if there is a usable target already set.
 */
bool get_aim_dir(int *dp)
{
	/* Global direction */
	int dir = 0;
	
	ui_event ke;

	const char *p;

	/* Initialize */
	(*dp) = 0;

	/* Hack -- auto-target if requested */
	if (OPT(use_old_target) && target_okay() && !dir) dir = 5;

	/* Ask until satisfied */
	while (!dir)
	{
		/* Choose a prompt */
		if (!target_okay())
			p = "Direction ('*' or <click> to target, \"'\" for closest, Escape to cancel)? ";
		else
			p = "Direction ('5' for target, '*' or <click> to re-target, Escape to cancel)? ";

		/* Get a command (or Cancel) */
		if (!get_com_ex(p, &ke)) break;

		if (ke.type == EVT_MOUSE)
		{
			if (target_set_interactive(TARGET_KILL, KEY_GRID_X(ke), KEY_GRID_Y(ke)))
				dir = 5;
		}
		else if (ke.type == EVT_KBRD)
		{
			if (ke.key.code == '*')
			{
				/* Set new target, use target if legal */
				if (target_set_interactive(TARGET_KILL, -1, -1))
					dir = 5;
			}
			else if (ke.key.code == '\'')
			{
				/* Set to closest target */
				if (target_set_closest(TARGET_KILL))
					dir = 5;
			}
			else if (ke.key.code == 't' || ke.key.code == '5' ||
					ke.key.code == '0' || ke.key.code == '.')
			{
				if (target_okay())
					dir = 5;
			}
			else
			{
				/* Possible direction */
				int keypresses_handled = 0;
				
				while (ke.key.code != 0)
				{
					int this_dir;
					
					/* XXX Ideally show and move the cursor here to indicate 
					   the currently "Pending" direction. XXX */
					this_dir = target_dir(ke.key);
					
					if (this_dir)
						dir = dir_transitions[dir][this_dir];
					else
						break;
					
					if (lazymove_delay == 0 || ++keypresses_handled > 1)
						break;
				
					/* See if there's a second keypress within the defined
					   period of time. */
					inkey_scan = lazymove_delay; 
					ke = inkey_ex();
				}
			}
		}

		/* Error */
		if (!dir) bell("Illegal aim direction!");
	}

	/* No direction */
	if (!dir) return (false);

	/* Save direction */
	(*dp) = dir;

	/* A "valid" direction was entered */
	return (true);
}







