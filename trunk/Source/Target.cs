using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Target {
		/*
		 * Bit flags for the "target_set" function
		 *
		 *	KILL: Target monsters
		 *	LOOK: Describe grid fully
		 *	XTRA: Currently unused flag (NOT USED)
		 *	GRID: Select from all grids (NOT USED)
		 * QUIET: Prevent targeting messages.
		 */
		public const int KILL =  0x01;
		public const int LOOK =  0x02;
		public const int XTRA =  0x04;
		public const int GRID =  0x08;
		public const int QUIET = 0x10;

		/*** File-wide variables ***/

		/* Is the target set? */
		static bool target_set;

		/* Current monster being tracked, or 0 */
		static ushort target_who;

		/* Target location */
		static short target_x, target_y;

		/*
		 * Determine is a monster makes a reasonable target
		 *
		 * The concept of "targetting" was stolen from "Morgul" (?)
		 *
		 * The player can target any location, or any "target-able" monster.
		 *
		 * Currently, a monster is "target_able" if it is visible, and if
		 * the player can hit it with a projection, and the player is not
		 * hallucinating.  This allows use of "use closest target" macros.
		 *
		 * Future versions may restrict the ability to target "trappers"
		 * and "mimics", but the semantics is a little bit weird.
		 */
		public static bool able(int m_idx)
		{
			throw new NotImplementedException();
			//int py = p_ptr.py;
			//int px = p_ptr.px;

			//monster_type *m_ptr;

			///* No monster */
			//if (m_idx <= 0) return (false);

			///* Get monster */
			//m_ptr = cave_monster(cave, m_idx);

			///* Monster must be alive */
			//if (!m_ptr.r_idx) return (false);

			///* Monster must be visible */
			//if (!m_ptr.ml) return (false);
	
			///* Player must be aware this is a monster */
			//if (m_ptr.unaware) return (false);

			///* Monster must be projectable */
			//if (!projectable(py, px, m_ptr.fy, m_ptr.fx, PROJECT_NONE))
			//    return (false);

			///* Hack -- no targeting hallucinations */
			//if (p_ptr.timed[TMD_IMAGE]) return (false);

			///* Assume okay */
			//return (true);
		}

		/*
		 * Set the target to a monster (or nobody)
		 */
		public static void set_monster(int m_idx)
		{
			/* Acceptable target */
			if ((m_idx > 0) && able(m_idx))
			{
				throw new NotImplementedException();
				//monster_type *m_ptr = cave_monster(cave, m_idx);

				///* Save target info */
				//target_set = true;
				//target_who = m_idx;
				//target_y = m_ptr.fy;
				//target_x = m_ptr.fx;
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

		/**
		 * Returns the currently targeted monster index.
		 */
		public static short get_monster()
		{
			return (short)target_who;
		}

		/*
		 * Handle "target" and "look".
		 *
		 * Note that this code can be called from "get_aim_dir()".
		 *
		 * Currently, when "flag" is true, that is, when
		 * "interesting" grids are being used, and a directional key is used, we
		 * only scroll by a single panel, in the direction requested, and check
		 * for any interesting grids on that panel.  The "correct" solution would
		 * actually involve scanning a larger set of grids, including ones in
		 * panels which are adjacent to the one currently scanned, but this is
		 * overkill for this function.  XXX XXX
		 *
		 * Hack -- targetting/observing an "outer border grid" may induce
		 * problems, so this is not currently allowed.
		 *
		 * The player can use the direction keys to move among "interesting"
		 * grids in a heuristic manner, or the "space", "+", and "-" keys to
		 * move through the "interesting" grids in a sequential manner, or
		 * can enter "location" mode, and use the direction keys to move one
		 * grid at a time in any direction.  The "t" (set target) command will
		 * only target a monster (as opposed to a location) if the monster is
		 * target_able and the "interesting" mode is being used.
		 *
		 * The current grid is described using the "look" method above, and
		 * a new command may be entered at any time, but note that if the
		 * "TARGET_LOOK" bit flag is set (or if we are in "location" mode,
		 * where "space" has no obvious meaning) then "space" will scan
		 * through the description of the current grid until done, instead
		 * of immediately jumping to the next "interesting" grid.  This
		 * allows the "target" command to retain its old semantics.
		 *
		 * The "*", "+", and "-" keys may always be used to jump immediately
		 * to the next (or previous) interesting grid, in the proper mode.
		 *
		 * The "return" key may always be used to scan through a complete
		 * grid description (forever).
		 *
		 * This command will cancel any old target, even if used from
		 * inside the "look" command.
		 *
		 *
		 * 'mode' is one of TARGET_LOOK or TARGET_KILL.
		 * 'x' and 'y' are the initial position of the target to be highlighted,
		 * or -1 if no location is specified.
		 * Returns true if a target has been successfully set, false otherwise.
		 */
		public static bool set_interactive(int mode, int x, int y)
		{
			throw new NotImplementedException();
			//int py = p_ptr.py;
			//int px = p_ptr.px;

			//int path_n;
			//u16b path_g[256];

			//int i, d, m, t, bd;
			//int wid, hgt, help_prompt_loc;

			//bool done = false;
			//bool flag = true;
			//bool help = false;

			//struct keypress query;

			///* These are used for displaying the path to the target */
			//char path_char[MAX_RANGE];
			//byte path_attr[MAX_RANGE];
			//struct point_set *targets;

			///* If we haven't been given an initial location, start on the
			//   player. */
			//if (x == -1 || y == -1)
			//{
			//    x = p_ptr.px;
			//    y = p_ptr.py;
			//}
			///* If we /have/ been given an initial location, make sure we
			//   honour it by going into "free targetting" mode. */
			//else
			//{
			//    flag = false;
			//}

			///* Cancel target */
			//target_set_monster(0);

			///* Cancel tracking */
			///* health_track(0); */

			///* Calculate the window location for the help prompt */
			//Term_get_size(&wid, &hgt);
			//help_prompt_loc = hgt - 1;
	
			///* Display the help prompt */
			//prt("Press '?' for help.", help_prompt_loc, 0);

			///* Prepare the target set */
			//targets = target_set_interactive_prepare(mode);

			///* Start near the player */
			//m = 0;

			///* Interact */
			//while (!done) {
			//    bool path_drawn = false;
		
			//    /* Interesting grids */
			//    if (flag && point_set_size(targets))
			//    {
			//        y = targets.pts[m].y;
			//        x = targets.pts[m].x;

			//        /* Adjust panel if needed */
			//        if (adjust_panel_help(y, x, help)) handle_stuff(p_ptr);
		
			//        /* Update help */
			//        if (help) {
			//            bool good_target = (cave.m_idx[y][x] > 0) &&
			//                target_able(cave.m_idx[y][x]);
			//            target_display_help(good_target, !(flag && point_set_size(targets)));
			//        }

			//        /* Find the path. */
			//        path_n = project_path(path_g, MAX_RANGE, py, px, y, x, PROJECT_THRU);

			//        /* Draw the path in "target" mode. If there is one */
			//        if (mode & (TARGET_KILL))
			//            path_drawn = draw_path(path_n, path_g, path_char, path_attr, py, px);

			//        /* Describe and Prompt */
			//        query = target_set_interactive_aux(y, x, mode);

			//        /* Remove the path */
			//        if (path_drawn) load_path(path_n, path_g, path_char, path_attr);

			//        /* Cancel tracking */
			//        /* health_track(0); */

			//        /* Assume no "direction" */
			//        d = 0;


			//        /* Analyze */
			//        switch (query.code)
			//        {
			//            case ESCAPE:
			//            case 'q':
			//            {
			//                done = true;
			//                break;
			//            }

			//            case ' ':
			//            case '*':
			//            case '+':
			//            {
			//                if (++m == point_set_size(targets))
			//                    m = 0;

			//                break;
			//            }

			//            case '-':
			//            {
			//                if (m-- == 0)
			//                    m = point_set_size(targets) - 1;

			//                break;
			//            }

			//            case 'p':
			//            {
			//                /* Recenter around player */
			//                verify_panel();

			//                /* Handle stuff */
			//                handle_stuff(p_ptr);

			//                y = p_ptr.py;
			//                x = p_ptr.px;
			//            }

			//            case 'o':
			//            {
			//                flag = false;
			//                break;
			//            }

			//            case 'm':
			//            {
			//                break;
			//            }

			//            case 't':
			//            case '5':
			//            case '0':
			//            case '.':
			//            {
			//                int m_idx = cave.m_idx[y][x];

			//                if ((m_idx > 0) && target_able(m_idx))
			//                {
			//                    health_track(p_ptr, m_idx);
			//                    target_set_monster(m_idx);
			//                    done = true;
			//                }
			//                else
			//                {
			//                    bell("Illegal target!");
			//                }
			//                break;
			//            }

			//            case 'g':
			//            {
			//                cmd_insert(CMD_PATHFIND);
			//                cmd_set_arg_point(cmd_get_top(), 0, y, x);
			//                done = true;
			//                break;
			//            }
				
			//            case '?':
			//            {
			//                help = !help;
					
			//                /* Redraw main window */
			//                p_ptr.redraw |= (PR_BASIC | PR_EXTRA | PR_MAP | PR_EQUIP);
			//                Term_clear();
			//                handle_stuff(p_ptr);
			//                if (!help)
			//                    prt("Press '?' for help.", help_prompt_loc, 0);
					
			//                break;
			//            }

			//            default:
			//            {
			//                /* Extract direction */
			//                d = target_dir(query);

			//                /* Oops */
			//                if (!d) bell("Illegal command for target mode!");

			//                break;
			//            }
			//        }

			//        /* Hack -- move around */
			//        if (d)
			//        {
			//            int old_y = targets.pts[m].y;
			//            int old_x = targets.pts[m].x;

			//            /* Find a new monster */
			//            i = target_pick(old_y, old_x, ddy[d], ddx[d], targets);

			//            /* Scroll to find interesting grid */
			//            if (i < 0)
			//            {
			//                int old_wy = Term.offset_y;
			//                int old_wx = Term.offset_x;

			//                /* Change if legal */
			//                if (change_panel(d))
			//                {
			//                    /* Recalculate interesting grids */
			//                    point_set_dispose(targets);
			//                    targets = target_set_interactive_prepare(mode);

			//                    /* Find a new monster */
			//                    i = target_pick(old_y, old_x, ddy[d], ddx[d], targets);

			//                    /* Restore panel if needed */
			//                    if ((i < 0) && modify_panel(Term, old_wy, old_wx))
			//                    {
			//                        /* Recalculate interesting grids */
			//                        point_set_dispose(targets);
			//                        targets = target_set_interactive_prepare(mode);
			//                    }

			//                    /* Handle stuff */
			//                    handle_stuff(p_ptr);
			//                }
			//            }

			//            /* Use interesting grid if found */
			//            if (i >= 0) m = i;
			//        }
			//    }

			//    /* Arbitrary grids */
			//    else
			//    {
			//        /* Update help */
			//        if (help) 
			//        {
			//            bool good_target = ((cave.m_idx[y][x] > 0) && target_able(cave.m_idx[y][x]));
			//            target_display_help(good_target, !(flag && point_set_size(targets)));
			//        }

			//        /* Find the path. */
			//        path_n = project_path(path_g, MAX_RANGE, py, px, y, x, PROJECT_THRU);

			//        /* Draw the path in "target" mode. If there is one */
			//        if (mode & (TARGET_KILL))
			//            path_drawn = draw_path (path_n, path_g, path_char, path_attr, py, px);

			//        /* Describe and Prompt (enable "TARGET_LOOK") */
			//        query = target_set_interactive_aux(y, x, mode | TARGET_LOOK);

			//        /* Remove the path */
			//        if (path_drawn)  load_path(path_n, path_g, path_char, path_attr);

			//        /* Cancel tracking */
			//        /* health_track(0); */

			//        /* Assume no direction */
			//        d = 0;

			//        /* Analyze the keypress */
			//        switch (query.code)
			//        {
			//            case ESCAPE:
			//            case 'q':
			//            {
			//                done = true;
			//                break;
			//            }

			//            case ' ':
			//            case '*':
			//            case '+':
			//            case '-':
			//            {
			//                break;
			//            }

			//            case 'p':
			//            {
			//                /* Recenter around player */
			//                verify_panel();

			//                /* Handle stuff */
			//                handle_stuff(p_ptr);

			//                y = p_ptr.py;
			//                x = p_ptr.px;
			//            }

			//            case 'o':
			//            {
			//                break;
			//            }

			//            case 'm':
			//            {
			//                flag = true;

			//                m = 0;
			//                bd = 999;

			//                /* Pick a nearby monster */
			//                for (i = 0; i < point_set_size(targets); i++)
			//                {
			//                    t = distance(y, x, targets.pts[i].y, targets.pts[i].x);

			//                    /* Pick closest */
			//                    if (t < bd)
			//                    {
			//                        m = i;
			//                        bd = t;
			//                    }
			//                }

			//                /* Nothing interesting */
			//                if (bd == 999) flag = false;

			//                break;
			//            }

			//            case 't':
			//            case '5':
			//            case '0':
			//            case '.':
			//            {
			//                target_set_location(y, x);
			//                done = true;
			//                break;
			//            }

			//            case 'g':
			//            {
			//                cmd_insert(CMD_PATHFIND);
			//                cmd_set_arg_point(cmd_get_top(), 0, y, x);
			//                done = true;
			//                break;
			//            }

			//            case '?':
			//            {
			//                help = !help;
					
			//                /* Redraw main window */
			//                p_ptr.redraw |= (PR_BASIC | PR_EXTRA | PR_MAP | PR_EQUIP);
			//                Term_clear();
			//                handle_stuff(p_ptr);
			//                if (!help)
			//                    prt("Press '?' for help.", help_prompt_loc, 0);
					
			//                break;
			//            }

			//            default:
			//            {
			//                /* Extract a direction */
			//                d = target_dir(query);

			//                /* Oops */
			//                if (!d) bell("Illegal command for target mode!");

			//                break;
			//            }
			//        }

			//        /* Handle "direction" */
			//        if (d)
			//        {
			//            int dungeon_hgt = (p_ptr.depth == 0) ? TOWN_HGT : DUNGEON_HGT;
			//            int dungeon_wid = (p_ptr.depth == 0) ? TOWN_WID : DUNGEON_WID;

			//            /* Move */
			//            x += ddx[d];
			//            y += ddy[d];

			//            /* Slide into legality */
			//            if (x >= dungeon_wid - 1) x--;
			//            else if (x <= 0) x++;

			//            /* Slide into legality */
			//            if (y >= dungeon_hgt - 1) y--;
			//            else if (y <= 0) y++;

			//            /* Adjust panel if needed */
			//            if (adjust_panel_help(y, x, help))
			//            {
			//                /* Handle stuff */
			//                handle_stuff(p_ptr);

			//                /* Recalculate interesting grids */
			//                point_set_dispose(targets);
			//                targets = target_set_interactive_prepare(mode);
			//            }
			//        }
			//    }
			//}

			///* Forget */
			//point_set_dispose(targets);

			///* Redraw as necessary */
			//if (help)
			//{
			//    p_ptr.redraw |= (PR_BASIC | PR_EXTRA | PR_MAP | PR_EQUIP);
			//    Term_clear();
			//}
			//else
			//{
			//    prt("", 0, 0);
			//    prt("", help_prompt_loc, 0);
			//    p_ptr.redraw |= (PR_DEPTH | PR_STATUS);
			//}

			///* Recenter around player */
			//verify_panel();

			///* Handle stuff */
			//handle_stuff(p_ptr);

			///* Failure to set target */
			//if (!target_set) return (false);

			///* Success */
			//return (true);
		}

		/*
		 * Update (if necessary) and verify (if possible) the target.
		 *
		 * We return true if the target is "okay" and false otherwise.
		 */
		public static bool okay()
		{
			/* No target */
			if (!target_set) return (false);

			/* Accept "location" targets */
			if (target_who == 0) return (true);

			/* Check "monster" targets */
			if (target_who > 0)
			{
				int m_idx = target_who;

				/* Accept reasonable targets */
				if (able(m_idx))
				{
					Monster.Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);

					/* Get the monster location */
					target_y = m_ptr.fy;
					target_x = m_ptr.fx;

					/* Good target */
					return (true);
				}
			}

			/* Assume no target */
			return (false);
		}

		public static bool set_closest(int mode)
		{
			throw new NotImplementedException();
			//int y, x, m_idx;
			//monster_type *m_ptr;
			//char m_name[80];
			//bool visibility;
			//struct point_set *targets;

			///* Cancel old target */
			//target_set_monster(0);

			///* Get ready to do targetting */
			//targets = target_set_interactive_prepare(mode);

			///* If nothing was prepared, then return */
			//if (point_set_size(targets) < 1)
			//{
			//    msg("No Available Target.");
			//    point_set_dispose(targets);
			//    return false;
			//}

			///* Find the first monster in the queue */
			//y = targets.pts[0].y;
			//x = targets.pts[0].x;
			//m_idx = cave.m_idx[y][x];
	
			///* Target the monster, if possible */
			//if ((m_idx <= 0) || !target_able(m_idx))
			//{
			//    msg("No Available Target.");
			//    point_set_dispose(targets);
			//    return false;
			//}

			///* Target the monster */
			//m_ptr = cave_monster(cave, m_idx);
			//monster_desc(m_name, sizeof(m_name), m_ptr, 0x00);
			//if (!(mode & TARGET_QUIET))
			//    msg("%^s is targeted.", m_name);
			//Term_fresh();

			///* Set up target information */
			//monster_race_track(m_ptr.r_idx);
			//health_track(p_ptr, cave.m_idx[y][x]);
			//target_set_monster(m_idx);

			///* Visual cue */
			//Term_get_cursor(&visibility);
			//(void)Term_set_cursor(true);
			//move_cursor_relative(y, x);
			//Term_redraw_section(x, y, x, y);

			///* TODO: what's an appropriate amount of time to spend highlighting */
			//Term_xtra(TERM_XTRA_DELAY, 150);
			//(void)Term_set_cursor(visibility);

			//point_set_dispose(targets);
			//return true;
		}

	}
}
