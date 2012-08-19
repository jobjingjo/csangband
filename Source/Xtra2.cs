using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Player;

namespace CSAngband {
	class Xtra2 {
		/*
		 * Verify the current panel (relative to the player location).
		 *
		 * By default, when the player gets "too close" to the edge of the current
		 * panel, the map scrolls one panel in that direction so that the player
		 * is no longer so close to the edge.
		 *
		 * The "OPT(center_player)" option allows the current panel to always be centered
		 * around the player, which is very expensive, and also has some interesting
		 * gameplay ramifications.
		 */
		public static void verify_panel()
		{
			verify_panel_int(Option.center_player.value);
		}

		public static void center_panel()
		{
			verify_panel_int(true);
		}

		public static void verify_panel_int(bool centered)
		{
			int wy, wx;
			int screen_hgt, screen_wid;

			int panel_wid, panel_hgt;

			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			int j;

			/* Scan windows */
			for (j = 0; j < Misc.ANGBAND_TERM_MAX; j++)
			{
				Term t = Misc.angband_term[j];

				/* No window */
				if (t == null) continue;

				/* No relevant flags */
				if ((j > 0) && (Player_Other.instance.window_flag[j] & (Misc.PW_MAP)) == 0) continue;

				wy = t.offset_y;
				wx = t.offset_x;

				screen_hgt = (j == 0) ? Misc.SCREEN_HGT : t.hgt;
				screen_wid = (j == 0) ? Misc.SCREEN_WID : t.wid;

				panel_wid = screen_wid / 2;
				panel_hgt = screen_hgt / 2;


				/* Scroll screen vertically when off-center */
				if (centered && Misc.p_ptr.running == 0 && (py != wy + panel_hgt))
					wy = py - panel_hgt;

				/* Scroll screen vertically when 3 grids from top/bottom edge */
				else if ((py < wy + 3) || (py >= wy + screen_hgt - 3))
					wy = py - panel_hgt;


				/* Scroll screen horizontally when off-center */
				if (centered && Misc.p_ptr.running == 0 && (px != wx + panel_wid))
					wx = px - panel_wid;

				/* Scroll screen horizontally when 3 grids from left/right edge */
				else if ((px < wx + 3) || (px >= wx + screen_wid - 3))
					wx = px - panel_wid;


				/* Scroll if needed */
				modify_panel(t, wy, wx);
			}
		}

		/*
		 * Modify the current panel to the given coordinates, adjusting only to
		 * ensure the coordinates are legal, and return true if anything done.
		 *
		 * The town should never be scrolled around.
		 *
		 * Note that monsters are no longer affected in any way by panel changes.
		 *
		 * As a total hack, whenever the current panel changes, we assume that
		 * the "overhead view" window should be updated.
		 */
		public static bool modify_panel(Term t, int wy, int wx)
		{
			int dungeon_hgt = (Misc.p_ptr.depth == 0) ? Cave.TOWN_HGT : Cave.DUNGEON_HGT;
			int dungeon_wid = (Misc.p_ptr.depth == 0) ? Cave.TOWN_WID : Cave.DUNGEON_WID;

			/* Verify wy, adjust if needed */
			if (wy > dungeon_hgt - Misc.SCREEN_HGT) wy = dungeon_hgt - Misc.SCREEN_HGT;
			if (wy < 0) wy = 0;

			/* Verify wx, adjust if needed */
			if (wx > dungeon_wid - Misc.SCREEN_WID) wx = dungeon_wid - Misc.SCREEN_WID;
			if (wx < 0) wx = 0;

			/* React to changes */
			if ((t.offset_y != wy) || (t.offset_x != wx))
			{
				/* Save wy, wx */
				t.offset_y = (byte)wy;
				t.offset_x = (byte)wx;

				/* Redraw map */
				Misc.p_ptr.redraw |= (Misc.PR_MAP);

				/* Redraw for big graphics */
				if ((Term.tile_width > 1) || (Term.tile_height > 1)) Misc.p_ptr.redraw_stuff();
      
				/* Changed */
				return (true);
			}

			/* No change */
			return (false);
		}

		static int[][] dir_transitions = new int[10][]
		{
			/* 0. */ new int [10]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
			/* 1. */ new int [10]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
			/* 2. */ new int [10]{ 0, 0, 2, 0, 1, 0, 3, 0, 5, 0 }, 
			/* 3. */ new int [10]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
			/* 4. */ new int [10]{ 0, 0, 1, 0, 4, 0, 5, 0, 7, 0 }, 
			/* 5. */ new int [10]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
			/* 6. */ new int [10]{ 0, 0, 3, 0, 5, 0, 6, 0, 9, 0 }, 
			/* 7. */ new int [10]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
			/* 8. */ new int [10]{ 0, 0, 5, 0, 7, 0, 9, 0, 8, 0 }, 
			/* 9. */ new int [10]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		};

		/*
		 * Request a "movement" direction (1,2,3,4,6,7,8,9) from the user.
		 *
		 * Return true if a direction was chosen, otherwise return false.
		 *
		 * This function should be used for all "repeatable" commands, such as
		 * run, walk, open, close, bash, disarm, spike, tunnel, etc, as well
		 * as all commands which must reference a grid adjacent to the player,
		 * and which may not reference the grid under the player.
		 *
		 * Directions "5" and "0" are illegal and will not be accepted.
		 *
		 * This function tracks and uses the "global direction", and uses
		 * that as the "desired direction", if it is set.
		 */
		public static bool get_rep_dir(out int dp)
		{
			int dir = 0;

			ui_event ke;

			/* Initialize */
			dp = 0;

			/* Get a direction */
			while (dir == 0)
			{
				/* Paranoia XXX XXX XXX */
				Utilities.message_flush();

				/* Get first keypress - the first test is to avoid displaying the
				   prompt for direction if there's already a keypress queued up
				   and waiting - this just avoids a flickering prompt if there is
				   a "lazy" movement delay. */
				Utilities.inkey_scan = Misc.SCAN_INSTANT;
				ke = Utilities.inkey_ex();
				Utilities.inkey_scan = Misc.SCAN_OFF;

				if (ke.type == ui_event_type.EVT_NONE || (ke.type == ui_event_type.EVT_KBRD && Utilities.target_dir(ke.key) == 0))
				{
				    Utilities.prt("Direction or <click> (Escape to cancel)? ", 0, 0);
				    ke = Utilities.inkey_ex();
				}

				/* Check mouse coordinates */
				if (ke.type == ui_event_type.EVT_MOUSE)
				{
				    /*if (ke.button) */
				    {
				        int y = Misc.KEY_GRID_Y(ke);
				        int x = Misc.KEY_GRID_X(ke);
				        Loc from = new Loc(Misc.p_ptr.px, Misc.p_ptr.py);
				        Loc to = new Loc(x, y);

						throw new NotImplementedException();
				        //dir = pathfind_direction_to(from, to);
				    }
				}

				/* Get other keypresses until a direction is chosen. */
				else if (ke.type == ui_event_type.EVT_KBRD)
				{
				    int keypresses_handled = 0;

				    while (ke.type == ui_event_type.EVT_KBRD && ke.key.code != 0)
				    {
				        int this_dir;

				        if (ke.key.code == keycode_t.ESCAPE) 
				        {
				            /* Clear the prompt */
				            Utilities.prt("", 0, 0);

				            return (false);
				        }

				        /* XXX Ideally show and move the cursor here to indicate 
				           the currently "Pending" direction. XXX */
				        this_dir = Utilities.target_dir(ke.key);

				        if (this_dir != 0)
				            dir = dir_transitions[dir][this_dir];

				        if (Misc.lazymove_delay == 0 || ++keypresses_handled > 1)
				            break;

				        Utilities.inkey_scan = Misc.lazymove_delay; 
				        ke = Utilities.inkey_ex();
				    }

				    /* 5 is equivalent to "escape" */
				    if (dir == 5)
				    {
				        /* Clear the prompt */
				        Utilities.prt("", 0, 0);

				        return (false);
				    }
				}

				/* Oops */
				if (dir == 0) Utilities.bell("Illegal repeatable direction!");
			}

			/* Clear the prompt */
			Utilities.prt("", 0, 0);

			/* Save direction */
			dp = dir;

			/* Success */
			return (true);
		}
	}
}
