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
			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			Monster.Monster m_ptr;

			/* No monster */
			if (m_idx <= 0) return (false);

			/* Get monster */
			m_ptr = Cave.cave_monster(Cave.cave, m_idx);

			/* Monster must be alive */
			if (m_ptr.r_idx == 0) return (false);

			/* Monster must be visible */
			if (!m_ptr.ml) return (false);
	
			/* Player must be aware this is a monster */
			if (m_ptr.unaware) return (false);

			/* Monster must be projectable */
			if (!Cave.projectable(py, px, m_ptr.fy, m_ptr.fx, Spell.PROJECT_NONE))
			    return (false);

			/* Hack -- no targeting hallucinations */
			if (Misc.p_ptr.timed[(int)Timed_Effect.IMAGE] != 0) return (false);

			/* Assume okay */
			return (true);
		}

		/*
		 * Set the target to a monster (or nobody)
		 */
		public static void set_monster(int m_idx)
		{
			/* Acceptable target */
			if ((m_idx > 0) && able(m_idx))
			{
				Monster.Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);

				/* Save target info */
				target_set = true;
				target_who = (ushort)m_idx;
				target_y = m_ptr.fy;
				target_x = m_ptr.fx;
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

		/*
		 * Sorting hook -- comp function -- by "distance to player"
		 *
		 * We use "u" and "v" to point to arrays of "x" and "y" positions,
		 * and sort the arrays by double-distance to the player.
		 */
		static int cmp_distance(Loc a, Loc b)
		{
			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;
			int da, db, kx, ky;

			/* Absolute distance components */
			kx = a.x; kx -= px; kx = Math.Abs(kx);
			ky = a.y; ky -= py; ky = Math.Abs(ky);

			/* Approximate Double Distance to the first point */
			da = ((kx > ky) ? (kx + kx + ky) : (ky + ky + kx));

			/* Absolute distance components */
			kx = b.x; kx -= px; kx = Math.Abs(kx);
			ky = b.y; ky -= py; ky = Math.Abs(ky);

			/* Approximate Double Distance to the first point */
			db = ((kx > ky) ? (kx + kx + ky) : (ky + ky + kx));

			/* Compare the distances */
			if (da < db)
			    return -1;
			if (da > db)
			    return 1;
			return 0;
		}

		/**
		 * Obtains the location the player currently targets.
		 *
		 * Both `col` and `row` must point somewhere, and on function termination,
		 * contain the X and Y locations respectively.
		 */
		public static void get(out short col, out short row)
		{
			//assert(col);
			//assert(row);

			col = target_x;
			row = target_y;
		}


		/*
		 * Hack -- determine if a given location is "interesting"
		 */
		static bool set_interactive_accept(int y, int x)
		{
			Object.Object o_ptr;


			/* Player grids are always interesting */
			if (Cave.cave.m_idx[y][x] < 0) return (true);


			/* Handle hallucination */
			if (Misc.p_ptr.timed[(int)Timed_Effect.IMAGE] != 0) return (false);


			/* Visible monsters */
			if (Cave.cave.m_idx[y][x] > 0)
			{
				Monster.Monster m_ptr = Cave.cave_monster(Cave.cave, Cave.cave.m_idx[y][x]);

				/* Visible monsters */
				if (m_ptr.ml && !m_ptr.unaware) return (true);
			}

			/* Scan all objects in the grid */
			for (o_ptr = Object.Object.get_first_object(y, x); o_ptr != null; o_ptr = Object.Object.get_next_object(o_ptr))
			{
				/* Memorized object */
				if (o_ptr.marked != 0 && !Squelch.item_ok(o_ptr)) return (true);
			}

			/* Interesting memorized features */
			if ((Cave.cave.info[y][x] & (Cave.CAVE_MARK)) != 0)
			{
				/* Notice glyphs */
				if (Cave.cave.feat[y][x] == Cave.FEAT_GLYPH) return (true);

				/* Notice doors */
				if (Cave.cave.feat[y][x] == Cave.FEAT_OPEN) return (true);
				if (Cave.cave.feat[y][x] == Cave.FEAT_BROKEN) return (true);

				/* Notice stairs */
				if (Cave.cave.feat[y][x] == Cave.FEAT_LESS) return (true);
				if (Cave.cave.feat[y][x] == Cave.FEAT_MORE) return (true);

				/* Notice shops */
				if ((Cave.cave.feat[y][x] >= Cave.FEAT_SHOP_HEAD) &&
					(Cave.cave.feat[y][x] <= Cave.FEAT_SHOP_TAIL)) return (true);

				/* Notice traps */
				if (Cave.cave_isknowntrap(Cave.cave, y, x)) return true;

				/* Notice doors */
				if (Cave.cave_iscloseddoor(Cave.cave, y, x)) return true;

				/* Notice rubble */
				if (Cave.cave.feat[y][x] == Cave.FEAT_RUBBLE) return (true);

				/* Notice veins with treasure */
				if (Cave.cave.feat[y][x] == Cave.FEAT_MAGMA_K) return (true);
				if (Cave.cave.feat[y][x] == Cave.FEAT_QUARTZ_K) return (true);
			}

			/* Nope */
			return (false);
		}

		/*
		 * Return a target set of target_able monsters.
		 */
		static List<Loc> set_interactive_prepare(int mode)
		{
			int y, x;
			List<Loc> targets = new List<Loc>();// point_set_new(TS_INITIAL_SIZE);

			/* Scan the current panel */
			for (y = Term.instance.offset_y; y < Term.instance.offset_y + Misc.SCREEN_HGT; y++)
			{
			    for (x = Term.instance.offset_x; x < Term.instance.offset_x + Misc.SCREEN_WID; x++)
			    {
			        /* Check bounds */
			        if (!Cave.cave.in_bounds_fully(y, x)) continue;

			        /* Require "interesting" contents */
			        if (!set_interactive_accept(y, x)) continue;

			        /* Special mode */
			        if ((mode & (KILL)) != 0)
			        {
			            /* Must contain a monster */
			            if (!(Cave.cave.m_idx[y][x] > 0)) continue;

			            /* Must be a targettable monster */
			            if (!able(Cave.cave.m_idx[y][x])) continue;
			        }

			        /* Save the location */
					targets.Add(new Loc(x, y));
			    }
			}

			targets.Sort(cmp_distance);

			//sort(targets.pts, point_set_size(targets), sizeof(*(targets.pts)), cmp_distance);
			return targets;
		}

		public static bool set_closest(int mode)
		{
			int y, x, m_idx;
			Monster.Monster m_ptr;
			//char m_name[80];
			string m_name;
			bool visibility;
			List<Loc> targets;

			/* Cancel old target */
			set_monster(0);

			/* Get ready to do targetting */
			targets = set_interactive_prepare(mode);

			/* If nothing was prepared, then return */
			if (targets.Count() < 1)
			{
			    Utilities.msg("No Available Target.");
			    //point_set_dispose(targets);
			    return false;
			}

			/* Find the first monster in the queue */
			y = targets[0].y;
			x = targets[0].x;
			m_idx = Cave.cave.m_idx[y][x];
	
			/* Target the monster, if possible */
			if ((m_idx <= 0) || !able(m_idx))
			{
			    Utilities.msg("No Available Target.");
			    //point_set_dispose(targets);
			    return false;
			}

			/* Target the monster */
			m_ptr = Cave.cave_monster(Cave.cave, m_idx);
			m_name = m_ptr.monster_desc(0x00);
			if((mode & QUIET) == 0) {
				string temp = Char.ToUpper(m_name[0]) + m_name.Substring(1);
				Utilities.msg("{0} is targeted.", temp);
			}
			Term.fresh();

			/* Set up target information */
			Cave.monster_race_track(m_ptr.r_idx);
			Cave.health_track(Misc.p_ptr, Cave.cave.m_idx[y][x]);
			set_monster(m_idx);

			/* Visual cue */
			visibility = Term.get_cursor();
			Term.set_cursor(true);
			Cave.move_cursor_relative(y, x);
			Term.redraw_section(x, y, x, y);

			/* TODO: what's an appropriate amount of time to spend highlighting */
			Term.xtra(TERM_XTRA.DELAY, 150);
			Term.set_cursor(visibility);

			//point_set_dispose(targets);
			return true;
		}

	}
}
