using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Monster;

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


		public const int TS_INITIAL_SIZE = 20;

		/*
		 * Height of the help screen; any higher than 4 will overlap the health
		 * bar which we want to keep in targeting mode.
		 */
		public const int HELP_HEIGHT = 3;

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
			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			int path_n;
			List<ushort> path_g = new List<ushort>();//[256];

			int i, d, m, t, bd;
			int wid, hgt, help_prompt_loc;

			bool done = false;
			bool flag = true;
			bool help = false;

			keypress query;

			/* These are used for displaying the path to the target */
			char[] path_char = new char[Misc.MAX_RANGE];
			ConsoleColor[] path_attr = new ConsoleColor[Misc.MAX_RANGE];
			List<Loc> targets;

			/* If we haven't been given an initial location, start on the
			   player. */
			if (x == -1 || y == -1)
			{
			    x = Misc.p_ptr.px;
			    y = Misc.p_ptr.py;
			}
			/* If we /have/ been given an initial location, make sure we
			   honour it by going into "free targetting" mode. */
			else
			{
			    flag = false;
			}

			/* Cancel target */
			Target.set_monster(0);

			/* Cancel tracking */
			/* health_track(0); */

			/* Calculate the window location for the help prompt */
			Term.get_size(out wid, out hgt);
			help_prompt_loc = hgt - 1;
	
			/* Display the help prompt */
			Utilities.prt("Press '?' for help.", help_prompt_loc, 0);

			/* Prepare the target set */
			targets = Target.set_interactive_prepare(mode);

			/* Start near the player */
			m = 0;

			/* Interact */
			while (!done) {
			    bool path_drawn = false;
		
			    /* Interesting grids */
			    if (flag && targets.Count() != 0)
			    {
			        y = targets[m].y;
			        x = targets[m].x;

			        /* Adjust panel if needed */
			        if (adjust_panel_help(y, x, help)) Misc.p_ptr.handle_stuff();
		
			        /* Update help */
			        if (help) {
			            bool good_target = (Cave.cave.m_idx[y][x] > 0) &&
			                Target.able(Cave.cave.m_idx[y][x]);
			            Target.display_help(good_target, !(flag && targets.Count() != 0));
			        }

			        /* Find the path. */
			        path_n = Cave.project_path(out path_g, Misc.MAX_RANGE, py, px, y, x, Spell.PROJECT_THRU);

			        /* Draw the path in "target" mode. If there is one */
			        if ((mode & (KILL)) != 0)
			            path_drawn = draw_path(path_g, path_char, path_attr, py, px) != 0;

			        /* Describe and Prompt */
			        query = Target.set_interactive_aux(y, x, mode);

			        /* Remove the path */
			        if (path_drawn) load_path(path_g, path_char, path_attr);

			        /* Cancel tracking */
			        /* health_track(0); */

			        /* Assume no "direction" */
			        d = 0;


			        /* Analyze */
			        switch ((char)query.code)
			        {
			            case (char)keycode_t.ESCAPE:
			            case 'q':
			            {
			                done = true;
			                break;
			            }

			            case ' ':
			            case '*':
			            case '+':
			            {
			                if (++m == targets.Count())
			                    m = 0;

			                break;
			            }

			            case '-':
			            {
			                if (m-- == 0)
			                    m = targets.Count() - 1;

			                break;
			            }

			            case 'p':
			            {
			                /* Recenter around player */
			                Xtra2.verify_panel();

			                /* Handle stuff */
			                Misc.p_ptr.handle_stuff();

			                y = Misc.p_ptr.py;
			                x = Misc.p_ptr.px;
							flag = false;
							break;
			            }

			            case 'o':
			            {
			                flag = false;
			                break;
			            }

			            case 'm':
			            {
			                break;
			            }

			            case 't':
			            case '5':
			            case '0':
			            case '.':
			            {
			                int m_idx = Cave.cave.m_idx[y][x];

			                if ((m_idx > 0) && Target.able(m_idx))
			                {
			                    Cave.health_track(Misc.p_ptr, m_idx);
			                    Target.set_monster(m_idx);
			                    done = true;
			                }
			                else
			                {
			                    Utilities.bell("Illegal target!");
			                }
			                break;
			            }

			            case 'g':
			            {
							Game_Command.insert(Command_Code.PATHFIND);
							Game_Command.get_top().set_arg_point(0, y, x);
			                done = true;
			                break;
			            }
				
			            case '?':
			            {
			                help = !help;
					
			                /* Redraw main window */
			                Misc.p_ptr.redraw |= (uint)(Misc.PR_BASIC | Misc.PR_EXTRA | Misc.PR_MAP | Misc.PR_EQUIP);
			                Term.clear();
			                Misc.p_ptr.handle_stuff();
			                if (!help)
			                    Utilities.prt("Press '?' for help.", help_prompt_loc, 0);
					
			                break;
			            }

			            default:
			            {
			                /* Extract direction */
			                d = Utilities.target_dir(query);

			                /* Oops */
			                if (d == 0) Utilities.bell("Illegal command for target mode!");

			                break;
			            }
			        }

			        /* Hack -- move around */
			        if (d != 0)
			        {
			            int old_y = targets[m].y;
			            int old_x = targets[m].x;

			            /* Find a new monster */
			            i = Target.pick(old_y, old_x, Misc.ddy[d], Misc.ddx[d], targets);

			            /* Scroll to find interesting grid */
			            if (i < 0)
			            {
			                int old_wy = Term.instance.offset_y;
			                int old_wx = Term.instance.offset_x;

			                /* Change if legal */
			                if (Xtra2.change_panel(d))
			                {
			                    /* Recalculate interesting grids */
			                    //point_set_dispose(targets);
			                    targets = Target.set_interactive_prepare(mode);

			                    /* Find a new monster */
			                    i = Target.pick(old_y, old_x, Misc.ddy[d], Misc.ddx[d], targets);

			                    /* Restore panel if needed */
			                    if ((i < 0) && Xtra2.modify_panel(Term.instance, old_wy, old_wx))
			                    {
			                        /* Recalculate interesting grids */
			                        //point_set_dispose(targets);
			                        targets = Target.set_interactive_prepare(mode);
			                    }

			                    /* Handle stuff */
			                    Misc.p_ptr.handle_stuff();
			                }
			            }

			            /* Use interesting grid if found */
			            if (i >= 0) m = i;
			        }
			    }

			    /* Arbitrary grids */
			    else
			    {
			        /* Update help */
			        if (help) 
			        {
			            bool good_target = ((Cave.cave.m_idx[y][x] > 0) && Target.able(Cave.cave.m_idx[y][x]));
			            Target.display_help(good_target, !(flag && targets.Count() != 0));
			        }

			        /* Find the path. */
			        path_n = Cave.project_path(out path_g, Misc.MAX_RANGE, py, px, y, x, Spell.PROJECT_THRU);

			        /* Draw the path in "target" mode. If there is one */
			        if ((mode & (KILL)) != 0)
			            path_drawn = draw_path(path_g, path_char, path_attr, py, px) != 0;

			        /* Describe and Prompt (enable "TARGET_LOOK") */
			        query = Target.set_interactive_aux(y, x, mode | LOOK);

			        /* Remove the path */
			        if (path_drawn)  load_path(path_g, path_char, path_attr);

			        /* Cancel tracking */
			        /* health_track(0); */

			        /* Assume no direction */
			        d = 0;

			        /* Analyze the keypress */
			        switch ((char)query.code)
			        {
			            case (char)keycode_t.ESCAPE:
			            case 'q':
			            {
			                done = true;
			                break;
			            }

			            case ' ':
			            case '*':
			            case '+':
			            case '-':
			            {
			                break;
			            }

			            case 'p':
			            {
			                /* Recenter around player */
			                Xtra2.verify_panel();

			                /* Handle stuff */
			                Misc.p_ptr.handle_stuff();

			                y = Misc.p_ptr.py;
			                x = Misc.p_ptr.px;
							break;
			            }

			            case 'o':
			            {
			                break;
			            }

			            case 'm':
			            {
			                flag = true;

			                m = 0;
			                bd = 999;

			                /* Pick a nearby monster */
			                for (i = 0; i < targets.Count(); i++)
			                {
			                    t = Cave.distance(y, x, targets[i].y, targets[i].x);

			                    /* Pick closest */
			                    if (t < bd)
			                    {
			                        m = i;
			                        bd = t;
			                    }
			                }

			                /* Nothing interesting */
			                if (bd == 999) flag = false;

			                break;
			            }

			            case 't':
			            case '5':
			            case '0':
			            case '.':
			            {
							Target.set_location(y, x);
			                done = true;
			                break;
			            }

			            case 'g':
			            {
							Game_Command.insert(Command_Code.PATHFIND);
							Game_Command.get_top().set_arg_point(0, y, x);
			                done = true;
			                break;
			            }

			            case '?':
			            {
			                help = !help;
					
			                /* Redraw main window */
			                Misc.p_ptr.redraw |= (uint)(Misc.PR_BASIC | Misc.PR_EXTRA | Misc.PR_MAP | Misc.PR_EQUIP);
			                Term.clear();
			                Misc.p_ptr.handle_stuff();
			                if (!help)
			                    Utilities.prt("Press '?' for help.", help_prompt_loc, 0);
					
			                break;
			            }

			            default:
			            {
			                /* Extract a direction */
			                d = Utilities.target_dir(query);

			                /* Oops */
			                if (d == 0) Utilities.bell("Illegal command for target mode!");

			                break;
			            }
			        }

			        /* Handle "direction" */
			        if (d != 0)
			        {
			            int dungeon_hgt = (Misc.p_ptr.depth == 0) ? Cave.TOWN_HGT : Cave.DUNGEON_HGT;
			            int dungeon_wid = (Misc.p_ptr.depth == 0) ? Cave.TOWN_WID : Cave.DUNGEON_WID;

			            /* Move */
			            x += Misc.ddx[d];
			            y += Misc.ddy[d];

			            /* Slide into legality */
			            if (x >= dungeon_wid - 1) x--;
			            else if (x <= 0) x++;

			            /* Slide into legality */
			            if (y >= dungeon_hgt - 1) y--;
			            else if (y <= 0) y++;

			            /* Adjust panel if needed */
			            if (adjust_panel_help(y, x, help))
			            {
			                /* Handle stuff */
			                Misc.p_ptr.handle_stuff();

			                /* Recalculate interesting grids */
			                //point_set_dispose(targets);
			                targets = Target.set_interactive_prepare(mode);
			            }
			        }
			    }
			}

			/* Forget */
			//point_set_dispose(targets);

			/* Redraw as necessary */
			if (help)
			{
			    Misc.p_ptr.redraw |= (uint)(Misc.PR_BASIC | Misc.PR_EXTRA | Misc.PR_MAP | Misc.PR_EQUIP);
			    Term.clear();
			}
			else
			{
			    Utilities.prt("", 0, 0);
			    Utilities.prt("", help_prompt_loc, 0);
			    Misc.p_ptr.redraw |= (Misc.PR_DEPTH | Misc.PR_STATUS);
			}

			/* Recenter around player */
			Xtra2.verify_panel();

			/* Handle stuff */
			Misc.p_ptr.handle_stuff();

			/* Failure to set target */
			if (!target_set) return (false);

			/* Success */
			return (true);
		}

		/*
		 * Hack -- help "select" a location (see below)
		 */
		static short pick(int y1, int x1, int dy, int dx, List<Loc> targets)
		{
			int i, v;

			int x2, y2, x3, y3, x4, y4;

			int b_i = -1, b_v = 9999;


			/* Scan the locations */
			for (i = 0; i < targets.Count(); i++)
			{
				/* Point 2 */
				x2 = targets[i].x;
				y2 = targets[i].y;

				/* Directed distance */
				x3 = (x2 - x1);
				y3 = (y2 - y1);

				/* Verify quadrant */
				if (dx != 0 && (x3 * dx <= 0)) continue;
				if (dy != 0 && (y3 * dy <= 0)) continue;

				/* Absolute distance */
				x4 = Math.Abs(x3);
				y4 = Math.Abs(y3);

				/* Verify quadrant */
				if (dy != 0 && dx == 0 && (x4 > y4)) continue;
				if (dx != 0 && dy == 0 && (y4 > x4)) continue;

				/* Approximate Double Distance */
				v = ((x4 > y4) ? (x4 + x4 + y4) : (y4 + y4 + x4));

				/* Penalize location XXX XXX XXX */

				/* Track best */
				if ((b_i >= 0) && (v >= b_v)) continue;

				/* Track best */
				b_i = i; b_v = v;
			}

			/* Result */
			return (short)(b_i);
		}

		/*
		 * Monster health description
		 */
		static string look_mon_desc(int m_idx)
		{
			string buf;
			Monster.Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);
			Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			bool living = true;


			/* Determine if the monster is "living" (vs "undead") */
			if (r_ptr.monster_is_unusual()) living = false;


			/* Healthy monsters */
			if (m_ptr.hp >= m_ptr.maxhp)
			{
				/* No damage */
				buf = living ? "unhurt" : "undamaged";
			}
			else
			{
				/* Calculate a health "percentage" */
				int perc = (int)(100L * m_ptr.hp / m_ptr.maxhp);

				if(perc >= 60)
					buf = living ? "somewhat wounded" : "somewhat damaged";
				else if(perc >= 25)
					buf = living ? "wounded" : "damaged";
				else if(perc >= 10)
					buf = living ? "badly wounded" : "badly damaged";
				else
					buf = living ? "almost dead" : "almost destroyed";
			}

			if (m_ptr.m_timed[(int)Misc.MON_TMD.SLEEP] != 0) buf += ", asleep";
			if (m_ptr.m_timed[(int)Misc.MON_TMD.CONF] != 0) buf += ", confused";
			if (m_ptr.m_timed[(int)Misc.MON_TMD.FEAR] != 0) buf += ", afraid";
			if (m_ptr.m_timed[(int)Misc.MON_TMD.STUN] != 0) buf += ", stunned";

			return buf;
		}

		/*
		 * Describe a location relative to the player position.
		 * e.g. "12 S 35 W" or "0 N, 33 E" or "0 N, 0 E"
		 */
		static string coords_desc(int y, int x)
		{
			string east_or_west;
			string north_or_south;

			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			if (y > py)
				north_or_south = "S";
			else
				north_or_south = "N";

			if (x < px)
				east_or_west = "W";
			else
				east_or_west = "E";

			string buf = String.Format("{0} {1}, {2} {3}", Math.Abs(y-py), north_or_south, 
				Math.Abs(x-px), east_or_west);

			return buf;
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
		static keypress set_interactive_aux(int y, int x, int mode)
		{
			short this_o_idx = 0, next_o_idx = 0;

			string s1, s2, s3;

			bool boring;

			int feat;

			int[] floor_list = new int[Misc.MAX_FLOOR_STACK];
			int floor_num;

			keypress query = new keypress();

			string out_val = "";//new char[256];

			string coords;//new char[20];

			/* Describe the square location */
			coords = coords_desc(y, x);

			/* Repeat forever */
			while (true)
			{
			    /* Paranoia */
			    query.code = (keycode_t)' ';

			    /* Assume boring */
			    boring = true;

			    /* Default */
			    s1 = "You see ";
			    s2 = "";
			    s3 = "";


			    /* The player */
			    if (Cave.cave.m_idx[y][x] < 0)
			    {
			        /* Description */
			        s1 = "You are ";

			        /* Preposition */
			        s2 = "on ";
			    }

			    /* Hallucination messes things up */
			    if (Misc.p_ptr.timed[(int)Timed_Effect.IMAGE] != 0)
			    {
					throw new NotImplementedException();
					//const char *name = "something strange";

					///* Display a message */
					//if (p_ptr.wizard)
					//    strnfmt(out_val, sizeof(out_val), "%s%s%s%s, %s (%d:%d).",
					//            s1, s2, s3, name, coords, y, x);
					//else
					//    strnfmt(out_val, sizeof(out_val), "%s%s%s%s, %s.",
					//            s1, s2, s3, name, coords);

					//prt(out_val, 0, 0);
					//move_cursor_relative(y, x);
					//query = inkey();

					///* Stop on everything but "return" */
					//if (query.code == '\n' || query.code == '\r')
					//    continue;

					//return query;
			    }

			    /* Actual monsters */
			    if (Cave.cave.m_idx[y][x] > 0)
			    {
			        Monster.Monster m_ptr = Cave.cave_monster(Cave.cave, Cave.cave.m_idx[y][x]);
			        Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			        /* Visible */
			        if (m_ptr.ml && !m_ptr.unaware)
			        {
			            bool recall = false;

			            //char m_name[80];
						string m_name;

			            /* Not boring */
			            boring = false;

			            /* Get the monster name ("a kobold") */
			            m_name = m_ptr.monster_desc(Monster.Monster.Desc.IND2);

			            /* Hack -- track this monster race */
			            Cave.monster_race_track(m_ptr.r_idx);

			            /* Hack -- health bar for this monster */
			            Cave.health_track(Misc.p_ptr, Cave.cave.m_idx[y][x]);

			            /* Hack -- handle stuff */
			            Misc.p_ptr.handle_stuff();

			            /* Interact */
			            while (true)
			            {
			                /* Recall */
			                if (recall)
			                {
			                    /* Save screen */
			                    Utilities.screen_save();

			                    /* Recall on screen */
			                    Monster_Lore.screen_roff(m_ptr.r_idx);

			                    /* Command */
			                    query = Utilities.inkey();

			                    /* Load screen */
			                    Utilities.screen_load();
			                }

			                /* Normal */
			                else
			                {
			                    //char buf[80];
								string buf;

			                    /* Describe the monster */
			                    buf = look_mon_desc(Cave.cave.m_idx[y][x]);

			                    /* Describe, and prompt for recall */
			                    if (Misc.p_ptr.wizard)
			                    {
									out_val = String.Format("{0}{1}{2}{3} ({4}), {5} ({6}:{7}).",
			                                s1, s2, s3, m_name, buf, coords, y, x);
			                    }
			                    else
			                    {
									out_val = String.Format("{0}{1}{2}{3} ({4}), {5}.",
			                                s1, s2, s3, m_name, buf, coords);
			                    }

			                    Utilities.prt(out_val, 0, 0);

			                    /* Place cursor */
			                    Cave.move_cursor_relative(y, x);

			                    /* Command */
			                    query = Utilities.inkey();
			                }

			                /* Normal commands */
			                if (query.code == (keycode_t)'r')
			                    recall = !recall;
			                else
			                    break;
			            }

			            /* Stop on everything but "return"/"space" */
			            if ((char)query.code != '\n' && (char)query.code != '\r' && (char)query.code != ' ')
			                break;

			            /* Sometimes stop at "space" key */
			            if (((char)query.code == ' ') && (mode & (LOOK)) == 0) break;

			            /* Take account of gender */
			            if (r_ptr.flags.has(Monster_Flag.FEMALE.value)) s1 = "She is ";
			            else if (r_ptr.flags.has(Monster_Flag.MALE.value)) s1 = "He is ";
			            else s1 = "It is ";

			            /* Use a verb */
			            s2 = "carrying ";

			            /* Scan all objects being carried */
			            for (this_o_idx = m_ptr.hold_o_idx; this_o_idx != 0; this_o_idx = next_o_idx)
			            {
							string o_name;
							//char o_name[80];

			                Object.Object o_ptr;

			                /* Get the object */
			                o_ptr = Object.Object.byid(this_o_idx);

			                /* Get the next object */
			                next_o_idx = o_ptr.next_o_idx;

			                /* Obtain an object description */
			                o_name = o_ptr.object_desc(Object.Object.Detail.PREFIX | Object.Object.Detail.FULL);

			                /* Describe the object */
			                if (Misc.p_ptr.wizard)
			                {
			                    out_val = String.Format("{0}{1}{2}{3}, {4} ({5}:{6}).",
			                            s1, s2, s3, o_name, coords, y, x);
			                }
			                /* Disabled since monsters now carry their drops
			                else
			                {
			                    strnfmt(out_val, sizeof(out_val),
			                            "%s%s%s%s, %s.", s1, s2, s3, o_name, coords);
			                } */

			                Utilities.prt(out_val, 0, 0);
			                Cave.move_cursor_relative(y, x);
			                query = Utilities.inkey();

			                /* Stop on everything but "return"/"space" */
			                if (((char)query.code != '\n') && ((char)query.code != '\r') && ((char)query.code != ' ')) break;

			                /* Sometimes stop at "space" key */
			                if (((char)query.code == ' ') && (mode & (LOOK)) == 0) break;

			                /* Change the intro */
			                s2 = "also carrying ";
			            }

			            /* Double break */
			            if (this_o_idx != 0) break;

			            /* Use a preposition */
			            s2 = "on ";
			        }
			    }

			    /* Assume not floored */
			    floor_num = Object.Object.scan_floor(floor_list, floor_list.Length, y, x, 0x02);

			    /* Scan all marked objects in the grid */
			    if ((floor_num > 0) && ((Misc.p_ptr.timed[(int)Timed_Effect.BLIND] == 0) || (y == Misc.p_ptr.py && x == Misc.p_ptr.px)))
			    {
			        /* Not boring */
			        boring = false;

			        Cave.track_object(-floor_list[0]);
			        Misc.p_ptr.handle_stuff();

			        /* If there is more than one item... */
			        if (floor_num > 1) while (true)
			        {
			            /* Describe the pile */
			            if (Misc.p_ptr.wizard)
			            {
							out_val = String.Format("{0}{1}{2}a pile of {3} objects, {4} ({5}:{6}).",
			                        s1, s2, s3, floor_num, coords, y, x);
			            }
			            else
			            {
							out_val = String.Format("{0}{1}{2}a pile of {3} objects, {4}.",
			                        s1, s2, s3, floor_num, coords);
			            }

			            Utilities.prt(out_val, 0, 0);
			            Cave.move_cursor_relative(y, x);
			            query = Utilities.inkey();

			            /* Display objects */
			            if ((char)query.code == 'r')
			            {
			                int rdone = 0;
			                int pos;
			                while (rdone == 0)
			                {
			                    /* Save screen */
			                    Utilities.screen_save();

			                    /* Display */
			                    Object.Object.show_floor(floor_list, floor_num, 
									Object.Object.olist_detail_t.OLIST_WEIGHT | Object.Object.olist_detail_t.OLIST_GOLD);

			                    /* Describe the pile */
			                    Utilities.prt(out_val, 0, 0);
			                    query = Utilities.inkey();

			                    /* Load screen */
			                    Utilities.screen_load();

			                    pos = (char)query.code - 'a';
			                    if (0 <= pos && pos < floor_num)
			                    {
			                        Cave.track_object(-floor_list[pos]);
			                        Misc.p_ptr.handle_stuff();
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

			            //char o_name[80];
						string o_name;

			            /* Get the single object in the list */
			            Object.Object o_ptr = Object.Object.byid((short)floor_list[0]);

			            /* Not boring */
			            boring = false;

			            /* Obtain an object description */
						o_name = o_ptr.object_desc(Object.Object.Detail.PREFIX | Object.Object.Detail.FULL);

			            /* Describe the object */
			            if (Misc.p_ptr.wizard)
			            {
							out_val = String.Format("{0}{1}{2}{3}, {4} ({5}:{6}).",
			                        s1, s2, s3, o_name, coords, y, x);
			            }
			            else
			            {
							out_val = String.Format("{0}{1}{2}{3}, {4}.", s1, s2, s3, o_name, coords);
			            }

			            Utilities.prt(out_val, 0, 0);
			            Cave.move_cursor_relative(y, x);
			            query = Utilities.inkey();

			            /* Stop on everything but "return"/"space" */
			            if (((char)query.code != '\n') && ((char)query.code != '\r') && ((char)query.code != ' ')) break;

			            /* Sometimes stop at "space" key */
			            if (((char)query.code == ' ') && (mode & (LOOK)) == 0) break;

			            /* Change the intro */
			            s1 = "It is ";

			            /* Plurals */
			            if (o_ptr.number != 1) s1 = "They are ";

			            /* Preposition */
			            s2 = "on ";
			        }

			    }

			    /* Double break */
			    if (this_o_idx != 0) break;


			    /* Feature (apply "mimic") */
			    feat = Misc.f_info[Cave.cave.feat[y][x]].mimic;

			    /* Require knowledge about grid, or ability to see grid */
			    if ((Cave.cave.info[y][x] & (Cave.CAVE_MARK)) == 0 && !Cave.player_can_see_bold(y,x))
			    {
			        /* Forget feature */
			        feat = Cave.FEAT_NONE;
			    }

			    /* Terrain feature if needed */
			    if (boring || (feat > Cave.FEAT_INVIS))
			    {
			        string name = Misc.f_info[feat].name;

			        /* Hack -- handle unknown grids */
			        if (feat == Cave.FEAT_NONE) name = "unknown grid";

			        /* Pick a prefix */
			        if (s2 != null && (feat >= Cave.FEAT_DOOR_HEAD)) s2 = "in ";

			        /* Pick proper indefinite article */
			        s3 = ("aeiouAEIOU".Contains(name[0])) ? "an " : "a ";

			        /* Hack -- special introduction for store doors */
			        if ((feat >= Cave.FEAT_SHOP_HEAD) && (feat <= Cave.FEAT_SHOP_TAIL))
			        {
			            s3 = "the entrance to the ";
			        }

			        /* Display a message */
			        if (Misc.p_ptr.wizard)
			        {
			            out_val = String.Format("{0}{1}{2}{3}, {4} ({5}:{6}).", s1, s2, s3, name, coords, y, x);
			        }
			        else
			        {
						out_val = String.Format("{0}{1}{2}{3}, {4}.", s1, s2, s3, name, coords);
			        }

			        Utilities.prt(out_val, 0, 0);
			        Cave.move_cursor_relative(y, x);
			        query = Utilities.inkey();

			        /* Stop on everything but "return"/"space" */
			        if (((char)query.code != '\n') && ((char)query.code != '\r') && ((char)query.code != ' ')) break;
			    }

			    /* Stop on everything but "return" */
			    if (((char)query.code != '\n') && ((char)query.code != '\r')) break;
			}

			/* Keep going */
			return (query);
		}


		/**
		 * Load the attr/char at each point along "path" which is on screen from
		 * "a" and "c". This was saved in draw_path().
		 */
		static void load_path(List<ushort> path_g, char[] c, ConsoleColor[] a) {
			int i;
			for (i = 0; i < path_g.Count(); i++) {
				int y = Cave.GRID_Y(path_g[i]);
				int x = Cave.GRID_X(path_g[i]);

				if (!Term.panel_contains((uint)y, (uint)x)) continue;
				Cave.move_cursor_relative(y, x);
				Term.addch(a[i], c[i]);
			}

			Term.fresh();
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
		static int draw_path(List<ushort> path_g, char[] c, ConsoleColor[] a, int y1, int x1)
		{
			int i;
			bool on_screen;

			/* No path, so do nothing. */
			if (path_g.Count() < 1) return 0;

			/* The starting square is never drawn, but notice if it is being
			 * displayed. In theory, it could be the last such square.
			 */
			on_screen = Term.panel_contains((uint)y1, (uint)x1);

			/* Draw the path. */
			for (i = 0; i < path_g.Count(); i++) {
			    ConsoleColor colour;

			    /* Find the co-ordinates on the level. */
			    int y = Cave.GRID_Y(path_g[i]);
			    int x = Cave.GRID_X(path_g[i]);

			    /*
			     * As path[] is a straight line and the screen is oblong,
			     * there is only section of path[] on-screen.
			     * If the square being drawn is visible, this is part of it.
			     * If none of it has been drawn, continue until some of it
			     * is found or the last square is reached.
			     * If some of it has been drawn, finish now as there are no
			     * more visible squares to draw.
			     */
			     if (Term.panel_contains((uint)y,(uint)x)) on_screen = true;
			     else if (on_screen) break;
			     else continue;

			    /* Find the position on-screen */
			    Cave.move_cursor_relative(y,x);

			    /* This square is being overwritten, so save the original. */
			    Term.what(Term.instance.scr.cx, Term.instance.scr.cy, ref a[i], ref c[i]);

			    /* Choose a colour. */
			    if (Cave.cave.m_idx[y][x] != 0 && Cave.cave_monster(Cave.cave, Cave.cave.m_idx[y][x]).ml) {
			        /* Visible monsters are red. */
			        Monster.Monster m_ptr = Cave.cave_monster(Cave.cave, Cave.cave.m_idx[y][x]);
			        Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			        /*mimics act as objects*/
			        if (r_ptr.flags.has(Monster_Flag.UNAWARE.value)) 
			            colour = ConsoleColor.Yellow;
			        else
			            colour = ConsoleColor.Red;
			    }

			    else if (Cave.cave.o_idx[y][x] != 0 && Object.Object.byid(Cave.cave.o_idx[y][x]).marked != 0)
			        /* Known objects are yellow. */
			        colour = ConsoleColor.Yellow;

			    else if (!Cave.cave_floor_bold(y,x) &&
			             ((Cave.cave.info[y][x] & (Cave.CAVE_MARK)) != 0 || Cave.player_can_see_bold(y,x)))
			        /* Known walls are blue. */
			        colour = ConsoleColor.Blue;

			    else if ((Cave.cave.info[y][x] & (Cave.CAVE_MARK)) == 0 && !Cave.player_can_see_bold(y,x))
			        /* Unknown squares are grey. */
			        colour = ConsoleColor.Gray;

			    else
			        /* Unoccupied squares are white. */
			        colour = ConsoleColor.White;

			    /* Draw the path segment */
			    Term.addch(colour, '*');
			}
			return i;
		}

		/*
		 * Display targeting help at the bottom of the screen.
		 */
		static void display_help(bool monster, bool free)
		{
			/* Determine help location */
			int wid, hgt, help_loc;
			Term.get_size(out wid, out hgt);
			help_loc = hgt - HELP_HEIGHT;
	
			/* Clear */
			Utilities.clear_from(help_loc);

			/* Prepare help hooks */
			Misc.text_out_hook = Utilities.text_out_to_screen;
			Misc.text_out_indent = 1;
			Term.gotoxy(1, help_loc);

			/* Display help */
			Utilities.text_out_c(ConsoleColor.Green, "<dir>");
			Utilities.text_out(" and ");
			Utilities.text_out_c(ConsoleColor.Green, "<click>");
			Utilities.text_out(" look around. '");
			Utilities.text_out_c(ConsoleColor.Green, "g");
			Utilities.text_out(" moves to the selection. '");
			Utilities.text_out_c(ConsoleColor.Green, "p");
			Utilities.text_out("' selects the player. '");
			Utilities.text_out_c(ConsoleColor.Green, "q");
			Utilities.text_out("' exits. '");
			Utilities.text_out_c(ConsoleColor.Green, "r");
			Utilities.text_out("' displays details. '");

			if (free)
			{
				Utilities.text_out_c(ConsoleColor.Green, "m");
				Utilities.text_out("' restricts to interesting places. ");
			}
			else
			{
				Utilities.text_out_c(ConsoleColor.Green, "+");
				Utilities.text_out("' and '");
				Utilities.text_out_c(ConsoleColor.Green, "-");
				Utilities.text_out("' cycle through interesting places. '");
				Utilities.text_out_c(ConsoleColor.Green, "o");
				Utilities.text_out("' allows free selection. ");
			}
	
			if (monster || free)
			{
				Utilities.text_out("'");
				Utilities.text_out_c(ConsoleColor.Green, "t");
				Utilities.text_out("' targets the current selection.");
			}

			/* Reset */
			Misc.text_out_indent = 0;
		}

		/*
		 * Set the target to a location
		 */
		public static void set_location(int y, int x)
		{
			/* Legal target */
			if (Cave.cave.in_bounds_fully(y, x))
			{
				/* Save target info */
				target_set = true;
				target_who = 0;
				target_y = (short)y;
				target_x = (short)x;
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
		 * Perform the minimum "whole panel" adjustment to ensure that the given
		 * location is contained inside the current panel, and return true if any
		 * such adjustment was performed. Optionally accounts for the targeting
		 * help window.
		 */
		static bool adjust_panel_help(int y, int x, bool help)
		{
			bool changed = false;

			int j;

			int screen_hgt_main = help ? (Term.instance.hgt - Misc.ROW_MAP - 3) 
									   : (Term.instance.hgt - Misc.ROW_MAP - 1);

			/* Scan windows */
			for (j = 0; j < Misc.ANGBAND_TERM_MAX; j++)
			{
				int wx, wy;
				int screen_hgt, screen_wid;

				Term t = Misc.angband_term[j];

				/* No window */
				if (t == null) continue;

				/* No relevant flags */
				if ((j > 0) && (Player.Player_Other.instance.window_flag[j] & Misc.PW_MAP) == 0) continue;

				wy = t.offset_y;
				wx = t.offset_x;

				screen_hgt = (j == 0) ? screen_hgt_main : t.hgt;
				screen_wid = (j == 0) ? (Term.instance.wid - Misc.COL_MAP - 1) : t.wid;

				/* Bigtile panels need adjustment */
				screen_wid = screen_wid / Term.tile_width;
				screen_hgt = screen_hgt / Term.tile_height;

				/* Adjust as needed */
				while (y >= wy + screen_hgt) wy += screen_hgt / 2;
				while (y < wy) wy -= screen_hgt / 2;

				/* Adjust as needed */
				while (x >= wx + screen_wid) wx += screen_wid / 2;
				while (x < wx) wx -= screen_wid / 2;

				/* Use "modify_panel" */
				if (Xtra2.modify_panel(t, wy, wx)) changed = true;
			}

			return (changed);
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
