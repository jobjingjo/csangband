using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Pathfind {
		/*
		 * Hack -- allow quick "cycling" through the legal directions
		 */
		static byte[] cycle =
		{ 1, 2, 3, 6, 9, 8, 7, 4, 1, 2, 3, 6, 9, 8, 7, 4, 1 };


		/*
		 * Hack -- map each direction into the "middle" of the "cycle[]" array
		 */
		static byte[] chome =
		{ 0, 8, 9, 10, 7, 0, 11, 6, 5, 4 };

		/* Compute the direction (in the angband 123456789 sense) from a point to a
		 * point. We decide to use diagonals if dx and dy are within a factor of two of
		 * each other; otherwise we choose a cardinal direction. */
		public static Direction direction_to(Loc from, Loc to)
		{
			int adx = Math.Abs(to.x - from.x);
			int ady = Math.Abs(to.y - from.y);
			int dx = to.x - from.x;
			int dy = to.y - from.y;

			if (dx == 0 && dy == 0)
				return Direction.NONE;

			if (dx >= 0 && dy >= 0)
			{
				if (adx < ady * 2 && ady < adx * 2)
					return Direction.NE;
				else if (adx > ady)
					return Direction.E;
				else
					return Direction.N;
			}
			else if (dx > 0 && dy < 0)
			{
				if (adx < ady * 2 && ady < adx * 2)
					return Direction.SE;
				else if (adx > ady)
					return Direction.E;
				else
					return Direction.S;
			}
			else if (dx < 0 && dy > 0)
			{
				if (adx < ady * 2 && ady < adx * 2)
					return Direction.NW;
				else if (adx > ady)
					return Direction.W;
				else
					return Direction.N;
			}
			else if (dx <= 0 && dy <= 0)
			{
				if (adx < ady * 2 && ady < adx * 2)
					return Direction.SW;
				else if (adx > ady)
					return Direction.W;
				else
					return Direction.S;
			}

			Misc.assert(false);
			return Direction.UNKNOWN;
		}

		/****** Running code ******/
		/*
		 * Basically, once you start running, you keep moving until something
		 * interesting happens.  In an enclosed space, you run straight, but
		 * you follow corners as needed (i.e. hallways).  In an open space,
		 * you run straight, but you stop before entering an enclosed space
		 * (i.e. a room with a doorway).  In a semi-open space (with walls on
		 * one side only), you run straight, but you stop before entering an
		 * enclosed space or an open space (i.e. running along side a wall).
		 *
		 * All discussions below refer to what the player can see, that is,
		 * an unknown wall is just like a normal floor.  This means that we
		 * must be careful when dealing with "illegal" grids.
		 *
		 * No assumptions are made about the layout of the dungeon, so this
		 * algorithm works in hallways, rooms, town, destroyed areas, etc.
		 *
		 * In the diagrams below, the player has just arrived in the grid
		 * marked as '@', and he has just come from a grid marked as 'o',
		 * and he is about to enter the grid marked as 'x'.
		 *
		 * Running while confused is not allowed, and so running into a wall
		 * is only possible when the wall is not seen by the player.  This
		 * will take a turn and stop the running.
		 *
		 * Several conditions are tracked by the running variables.
		 *
		 *   p_ptr.run_open_area (in the open on at least one side)
		 *   p_ptr.run_break_left (wall on the left, stop if it opens)
		 *   p_ptr.run_break_right (wall on the right, stop if it opens)
		 *
		 * When running begins, these conditions are initialized by examining
		 * the grids adjacent to the requested destination grid (marked 'x'),
		 * two on each side (marked 'L' and 'R').  If either one of the two
		 * grids on a given side is a wall, then that side is considered to
		 * be "closed".  Both sides enclosed yields a hallway.
		 *
		 *    LL                     @L
		 *    @x      (normal)       RxL   (diagonal)
		 *    RR      (east)          R    (south-east)
		 *
		 * In the diagram below, in which the player is running east along a
		 * hallway, he will stop as indicated before attempting to enter the
		 * intersection (marked 'x').  Starting a new run in any direction
		 * will begin a new hallway run.
		 *
		 * #.#
		 * ##.##
		 * o@x..
		 * ##.##
		 * #.#
		 *
		 * Note that a minor hack is inserted to make the angled corridor
		 * entry (with one side blocked near and the other side blocked
		 * further away from the runner) work correctly. The runner moves
		 * diagonally, but then saves the previous direction as being
		 * straight into the gap. Otherwise, the tail end of the other
		 * entry would be perceived as an alternative on the next move.
		 *
		 * In the diagram below, the player is running east down a hallway,
		 * and will stop in the grid (marked '1') before the intersection.
		 * Continuing the run to the south-east would result in a long run
		 * stopping at the end of the hallway (marked '2').
		 *
		 * ##################
		 * o@x       1
		 * ########### ######
		 * #2          #
		 * #############
		 *
		 * After each step, the surroundings are examined to determine if
		 * the running should stop, and to determine if the running should
		 * change direction.  We examine the new current player location
		 * (at which the runner has just arrived) and the direction from
		 * which the runner is considered to have come.
		 *
		 * Moving one grid in some direction places you adjacent to three
		 * or five new grids (for straight and diagonal moves respectively)
		 * to which you were not previously adjacent (marked as '!').
		 *
		 *   ...!              ...
		 *   .o@!  (normal)    .o.!  (diagonal)
		 *   ...!  (east)      ..@!  (south east)
		 *                      !!!
		 *
		 * If any of the newly adjacent grids are "interesting" (monsters,
		 * objects, some terrain features) then running stops.
		 *
		 * If any of the newly adjacent grids seem to be open, and you are
		 * looking for a break on that side, then running stops.
		 *
		 * If any of the newly adjacent grids do not seem to be open, and
		 * you are in an open area, and the non-open side was previously
		 * entirely open, then running stops.
		 *
		 * If you are in a hallway, then the algorithm must determine if
		 * the running should continue, turn, or stop.  If only one of the
		 * newly adjacent grids appears to be open, then running continues
		 * in that direction, turning if necessary.  If there are more than
		 * two possible choices, then running stops.  If there are exactly
		 * two possible choices, separated by a grid which does not seem
		 * to be open, then running stops.  Otherwise, as shown below, the
		 * player has probably reached a "corner".
		 *
		 *    ###             o##
		 *    o@x  (normal)   #@!   (diagonal)
		 *    ##!  (east)     ##x   (south east)
		 *
		 * In this situation, there will be two newly adjacent open grids,
		 * one touching the player on a diagonal, and one directly adjacent.
		 * We must consider the two "option" grids further out (marked '?').
		 * We assign "option" to the straight-on grid, and "option2" to the
		 * diagonal grid.
		 *
		 *    ###s
		 *    o@x?   (may be incorrect diagram!)
		 *    ##!?
		 *
		 * If both "option" grids are closed, then there is no reason to enter
		 * the corner, and so we can cut the corner, by moving into the other
		 * grid (diagonally).  If we choose not to cut the corner, then we may
		 * go straight, but we pretend that we got there by moving diagonally.
		 * Below, we avoid the obvious grid (marked 'x') and cut the corner
		 * instead (marked 'n').
		 *
		 *    ###:               o##
		 *    o@x#   (normal)    #@n    (maybe?)
		 *    ##n#   (east)      ##x#
		 *                       ####
		 *
		 * If one of the "option" grids is open, then we may have a choice, so
		 * we check to see whether it is a potential corner or an intersection
		 * (or room entrance).  If the grid two spaces straight ahead, and the
		 * space marked with 's' are both open, then it is a potential corner
		 * and we enter it if requested.  Otherwise, we stop, because it is
		 * not a corner, and is instead an intersection or a room entrance.
		 *
		 *    ###
		 *    o@x
		 *    ##!#
		 *
		 * I do not think this documentation is correct.
		 */


		/* Maximum size around the player to consider in the pathfinder */
		const int MAX_PF_RADIUS = 50;

		/* Maximum distance to consider in the pathfinder */
		const int MAX_PF_LENGTH = 250;


		//Was jagged array.
		static int[,] terrain = new int[MAX_PF_RADIUS, MAX_PF_RADIUS];
		static char[] pf_result = new char[MAX_PF_LENGTH];
		static int pf_result_index;

		/*
		 * Initialize the running algorithm for a new direction.
		 *
		 * Diagonal Corridor -- allow diaginal entry into corridors.
		 *
		 * Blunt Corridor -- If there is a wall two spaces ahead and
		 * we seem to be in a corridor, then force a turn into the side
		 * corridor, must be moving straight into a corridor here. (?)
		 *
		 * Diagonal Corridor    Blunt Corridor (?)
		 *       # #                  #
		 *       #x#                 @x#
		 *       @p.                  p
		 */
		static void run_init(int dir)
		{
			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			int i, row, col;

			bool deepleft, deepright;
			bool shortleft, shortright;

			/* Mark that we're starting a run */
			Misc.p_ptr.running_firststep = true;

			/* Save the direction */
			Misc.p_ptr.run_cur_dir = (short)dir;

			/* Assume running straight */
			Misc.p_ptr.run_old_dir = (short)dir;

			/* Assume looking for open area */
			Misc.p_ptr.run_open_area = true;

			/* Assume not looking for breaks */
			Misc.p_ptr.run_break_right = false;
			Misc.p_ptr.run_break_left = false;

			/* Assume no nearby walls */
			deepleft = deepright = false;
			shortright = shortleft = false;

			/* Find the destination grid */
			row = py + Misc.ddy[dir];
			col = px + Misc.ddx[dir];

			/* Extract cycle index */
			i = chome[dir];

			/* Check for nearby wall */
			if (see_wall(cycle[i+1], py, px))
			{
				Misc.p_ptr.run_break_left = true;
				shortleft = true;
			}

			/* Check for distant wall */
			else if (see_wall(cycle[i+1], row, col))
			{
				Misc.p_ptr.run_break_left = true;
				deepleft = true;
			}

			/* Check for nearby wall */
			if (see_wall(cycle[i-1], py, px))
			{
				Misc.p_ptr.run_break_right = true;
				shortright = true;
			}

			/* Check for distant wall */
			else if (see_wall(cycle[i-1], row, col))
			{
				Misc.p_ptr.run_break_right = true;
				deepright = true;
			}

			/* Looking for a break */
			if (Misc.p_ptr.run_break_left && Misc.p_ptr.run_break_right)
			{
				/* Not looking for open area */
				Misc.p_ptr.run_open_area = false;

				/* Hack -- allow angled corridor entry */
				if ((dir & 0x01) != 0)
				{
					if (deepleft && !deepright)
					{
						Misc.p_ptr.run_old_dir = cycle[i - 1];
					}
					else if (deepright && !deepleft)
					{
						Misc.p_ptr.run_old_dir = cycle[i + 1];
					}
				}

				/* Hack -- allow blunt corridor entry */
				else if (see_wall(cycle[i], row, col))
				{
					if (shortleft && !shortright)
					{
						Misc.p_ptr.run_old_dir = cycle[i - 2];
					}
					else if (shortright && !shortleft)
					{
						Misc.p_ptr.run_old_dir = cycle[i + 2];
					}
				}
			}
		}

		/*
		 * Hack -- Check for a "known wall" (see below)
		 */
		static bool see_wall(int dir, int y, int x)
		{
			/* Get the new location */
			y += Misc.ddy[dir];
			x += Misc.ddx[dir];

			/* Illegal grids are not known walls XXX XXX XXX */
			if (!Cave.in_bounds(y, x)) return (false);

			/* Non-wall grids are not known walls */
			if (Cave.cave.feat[y][x] < Cave.FEAT_SECRET) return (false);

			/* Unknown walls are not known walls */
			if ((Cave.cave.info[y][x] & (Cave.CAVE_MARK)) == 0) return (false);

			/* Default */
			return (true);
		}


		/*
		 * Update the current "run" path
		 *
		 * Return true if the running should be stopped
		 */
		static bool run_test()
		{
			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			int prev_dir;
			int new_dir;

			int row, col;
			int i, max;
			bool inv;
			int option, option2;


			/* No options yet */
			option = 0;
			option2 = 0;

			/* Where we came from */
			prev_dir = Misc.p_ptr.run_old_dir;


			/* Range of newly adjacent grids */
			max = (prev_dir & 0x01) + 1;


			/* Look at every newly adjacent square. */
			for (i = -max; i <= max; i++)
			{
				Object.Object o_ptr;


				/* New direction */
				new_dir = cycle[chome[prev_dir] + i];

				/* New location */
				row = py + Misc.ddy[new_dir];
				col = px + Misc.ddx[new_dir];


				/* Visible monsters abort running */
				if (Cave.cave.m_idx[row][col] > 0)
				{
					Monster.Monster m_ptr = Cave.cave_monster(Cave.cave, Cave.cave.m_idx[row][col]);

					/* Visible monster */
					if (m_ptr.ml) return (true);
				}

				/* Visible objects abort running */
				for (o_ptr = Object.Object.get_first_object(row, col); o_ptr != null; o_ptr = Object.Object.get_next_object(o_ptr))
				{
					/* Visible object */
					if (o_ptr.marked != 0 && !Squelch.item_ok(o_ptr)) return (true);
				}


				/* Assume unknown */
				inv = true;

				/* Check memorized grids */
				if ((Cave.cave.info[row][col] & (Cave.CAVE_MARK)) != 0)
				{
					bool notice = true;

					/* Examine the terrain */
					switch (Cave.cave.feat[row][col])
					{
						/* Floors */
						case Cave.FEAT_FLOOR:

						/* Invis traps */
						case Cave.FEAT_INVIS:

						/* Secret doors */
						case Cave.FEAT_SECRET:

						/* Normal veins */
						case Cave.FEAT_MAGMA:
						case Cave.FEAT_QUARTZ:

						/* Hidden treasure */
						case Cave.FEAT_MAGMA_H:
						case Cave.FEAT_QUARTZ_H:

						/* Walls */
						case Cave.FEAT_WALL_EXTRA:
						case Cave.FEAT_WALL_INNER:
						case Cave.FEAT_WALL_OUTER:
						case Cave.FEAT_WALL_SOLID:
						case Cave.FEAT_PERM_EXTRA:
						case Cave.FEAT_PERM_INNER:
						case Cave.FEAT_PERM_OUTER:
						case Cave.FEAT_PERM_SOLID:
						{
							/* Ignore */
							notice = false;

							/* Done */
							break;
						}
					}

					/* Interesting feature */
					if (notice) return (true);

					/* The grid is "visible" */
					inv = false;
				}

				/* Analyze unknown grids and floors */
				if (inv || Cave.cave_floor_bold(row, col))
				{
					/* Looking for open area */
					if (Misc.p_ptr.run_open_area)
					{
						/* Nothing */
					}

					/* The first new direction. */
					else if (option == 0)
					{
						option = new_dir;
					}

					/* Three new directions. Stop running. */
					else if (option2 != 0)
					{
						return (true);
					}

					/* Two non-adjacent new directions.  Stop running. */
					else if (option != cycle[chome[prev_dir] + i - 1])
					{
						return (true);
					}

					/* Two new (adjacent) directions (case 1) */
					else if ((new_dir & 0x01) != 0)
					{
						option2 = new_dir;
					}

					/* Two new (adjacent) directions (case 2) */
					else
					{
						option2 = option;
						option = new_dir;
					}
				}

				/* Obstacle, while looking for open area */
				else
				{
					if (Misc.p_ptr.run_open_area)
					{
						if (i < 0)
						{
							/* Break to the right */
							Misc.p_ptr.run_break_right = true;
						}

						else if (i > 0)
						{
							/* Break to the left */
							Misc.p_ptr.run_break_left = true;
						}
					}
				}
			}


			/* Look at every soon to be newly adjacent square. */
			for (i = -max; i <= max; i++)
			{		
				/* New direction */
				new_dir = cycle[chome[prev_dir] + i];
		
				/* New location */
				row = py + Misc.ddy[prev_dir] + Misc.ddy[new_dir];
				col = px + Misc.ddx[prev_dir] + Misc.ddx[new_dir];
		
				/* HACK: Ugh. Sometimes we come up with illegal bounds. This will
				 * treat the symptom but not the disease. */
				if (row >= Cave.DUNGEON_HGT || col >= Cave.DUNGEON_WID) continue;
				if (row < 0 || col < 0) continue;

				/* Visible monsters abort running */
				if (Cave.cave.m_idx[row][col] > 0)
				{
					Monster.Monster m_ptr = Cave.cave_monster(Cave.cave, Cave.cave.m_idx[row][col]);
			
					/* Visible monster */
					if (m_ptr.ml) return (true);			
				}
			}

			/* Looking for open area */
			if (Misc.p_ptr.run_open_area)
			{
				/* Hack -- look again */
				for (i = -max; i < 0; i++)
				{
					new_dir = cycle[chome[prev_dir] + i];

					row = py + Misc.ddy[new_dir];
					col = px + Misc.ddx[new_dir];

					/* Unknown grid or non-wall */
					/* Was: cave_floor_bold(row, col) */
					if ((Cave.cave.info[row][col] & (Cave.CAVE_MARK)) == 0 ||
						(Cave.cave.feat[row][col] < Cave.FEAT_SECRET))
					{
						/* Looking to break right */
						if (Misc.p_ptr.run_break_right)
						{
							return (true);
						}
					}

					/* Obstacle */
					else
					{
						/* Looking to break left */
						if (Misc.p_ptr.run_break_left)
						{
							return (true);
						}
					}
				}

				/* Hack -- look again */
				for (i = max; i > 0; i--)
				{
					new_dir = cycle[chome[prev_dir] + i];

					row = py + Misc.ddy[new_dir];
					col = px + Misc.ddx[new_dir];

					/* Unknown grid or non-wall */
					/* Was: cave_floor_bold(row, col) */
					if ((Cave.cave.info[row][col] & (Cave.CAVE_MARK)) == 0 ||
						(Cave.cave.feat[row][col] < Cave.FEAT_SECRET))
					{
						/* Looking to break left */
						if (Misc.p_ptr.run_break_left)
						{
							return (true);
						}
					}

					/* Obstacle */
					else
					{
						/* Looking to break right */
						if (Misc.p_ptr.run_break_right)
						{
							return (true);
						}
					}
				}
			}


			/* Not looking for open area */
			else
			{
				/* No options */
				if (option == 0)
				{
					return (true);
				}

				/* One option */
				else if (option2 == 0)
				{
					/* Primary option */
					Misc.p_ptr.run_cur_dir = (short)option;

					/* No other options */
					Misc.p_ptr.run_old_dir = (short)option;
				}

				/* Two options, examining corners */
				else
				{
					/* Primary option */
					Misc.p_ptr.run_cur_dir = (short)option;

					/* Hack -- allow curving */
					Misc.p_ptr.run_old_dir = (short)option2;
				}
			}


			/* About to hit a known wall, stop */
			if (see_wall(Misc.p_ptr.run_cur_dir, py, px))
			{
				return (true);
			}


			/* Failure */
			return (false);
		}


		/*
		 * Take one step along the current "run" path
		 *
		 * Called with a real direction to begin a new run, and with zero
		 * to continue a run in progress.
		 */
		public static void run_step(int dir)
		{
			int x, y;

			/* Start run */
			if (dir != 0)
			{
			    /* Initialize */
			    run_init(dir);

			    /* Hack -- Set the run counter */
			    Misc.p_ptr.running = (short)(Misc.p_ptr.command_arg != 0 ? Misc.p_ptr.command_arg : 1000);

			    /* Calculate torch radius */
			    Misc.p_ptr.update |= (Misc.PU_TORCH);
			}

			/* Continue run */
			else
			{
			    if (!Misc.p_ptr.running_withpathfind)
			    {
			        /* Update run */
			        if (run_test())
			        {
			            /* Disturb */
			            Cave.disturb(Misc.p_ptr, 0, 0);
	
			            /* Done */
			            return;
			        }
			    }
			    else
			    {
			        /* Abort if we have finished */
			        if (pf_result_index < 0)
			        {
			            Cave.disturb(Misc.p_ptr, 0, 0);
			            Misc.p_ptr.running_withpathfind = false;
			            return;
			        }

			        /* Abort if we would hit a wall */
			        else if (pf_result_index == 0)
			        {
			            /* Get next step */
			            y = Misc.p_ptr.py + Misc.ddy[pf_result[pf_result_index] - '0'];
			            x = Misc.p_ptr.px + Misc.ddx[pf_result[pf_result_index] - '0'];

			            /* Known wall */
			            if ((Cave.cave.info[y][x] & (Cave.CAVE_MARK)) != 0 && !Cave.cave_floor_bold(y, x))
			            {
			                Cave.disturb(Misc.p_ptr, 0,0);
			                Misc.p_ptr.running_withpathfind = false;
			                return;
			            }
			        }

			        /*
			         * Hack -- walking stick lookahead.
			         *
			         * If the player has computed a path that is going to end up in a wall,
			         * we notice this and convert to a normal run. This allows us to click
			         * on unknown areas to explore the map.
			         *
			         * We have to look ahead two, otherwise we don't know which is the last
			         * direction moved and don't initialise the run properly.
			         */
			        else if (pf_result_index > 0)
			        {
			            /* Get next step */
			            y = Misc.p_ptr.py + Misc.ddy[pf_result[pf_result_index] - '0'];
			            x = Misc.p_ptr.px + Misc.ddx[pf_result[pf_result_index] - '0'];

			            /* Known wall */
			            if ((Cave.cave.info[y][x] & (Cave.CAVE_MARK)) != 0 && !Cave.cave_floor_bold(y, x))
			            {
			                Cave.disturb(Misc.p_ptr, 0,0);
			                Misc.p_ptr.running_withpathfind = false;
			                return;
			            }

			            /* Get step after */
			            y = y + Misc.ddy[pf_result[pf_result_index-1] - '0'];
			            x = x + Misc.ddx[pf_result[pf_result_index-1] - '0'];

			            /* Known wall */
			            if ((Cave.cave.info[y][x] & (Cave.CAVE_MARK)) != 0 && !Cave.cave_floor_bold(y, x))
			            {
			                Misc.p_ptr.running_withpathfind = false;

			                run_init(pf_result[pf_result_index] - '0');
			            }
			        }

			        Misc.p_ptr.run_cur_dir = (short)(pf_result[pf_result_index--] - '0');
			    }
			}


			/* Decrease counter */
			Misc.p_ptr.running--;

			/* Take time */
			Misc.p_ptr.energy_use = 100;

			/* Move the player */
			Command.move_player(Misc.p_ptr.run_cur_dir, true);
		}
	}
}
