using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Pathfind {
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

		/*
		 * Take one step along the current "run" path
		 *
		 * Called with a real direction to begin a new run, and with zero
		 * to continue a run in progress.
		 */
		public static void run_step(int dir)
		{
			//int x, y;

			///* Start run */
			//if (dir)
			//{
			//    /* Initialize */
			//    run_init(dir);

			//    /* Hack -- Set the run counter */
			//    p_ptr.running = (p_ptr.command_arg ? p_ptr.command_arg : 1000);

			//    /* Calculate torch radius */
			//    p_ptr.update |= (PU_TORCH);
			//}

			///* Continue run */
			//else
			//{
			//    if (!p_ptr.running_withpathfind)
			//    {
			//        /* Update run */
			//        if (run_test())
			//        {
			//            /* Disturb */
			//            disturb(p_ptr, 0, 0);
	
			//            /* Done */
			//            return;
			//        }
			//    }
			//    else
			//    {
			//        /* Abort if we have finished */
			//        if (pf_result_index < 0)
			//        {
			//            disturb(p_ptr, 0, 0);
			//            p_ptr.running_withpathfind = false;
			//            return;
			//        }

			//        /* Abort if we would hit a wall */
			//        else if (pf_result_index == 0)
			//        {
			//            /* Get next step */
			//            y = p_ptr.py + ddy[pf_result[pf_result_index] - '0'];
			//            x = p_ptr.px + ddx[pf_result[pf_result_index] - '0'];

			//            /* Known wall */
			//            if ((cave.info[y][x] & (CAVE_MARK)) && !cave_floor_bold(y, x))
			//            {
			//                disturb(p_ptr, 0,0);
			//                p_ptr.running_withpathfind = false;
			//                return;
			//            }
			//        }

			//        /*
			//         * Hack -- walking stick lookahead.
			//         *
			//         * If the player has computed a path that is going to end up in a wall,
			//         * we notice this and convert to a normal run. This allows us to click
			//         * on unknown areas to explore the map.
			//         *
			//         * We have to look ahead two, otherwise we don't know which is the last
			//         * direction moved and don't initialise the run properly.
			//         */
			//        else if (pf_result_index > 0)
			//        {
			//            /* Get next step */
			//            y = p_ptr.py + ddy[pf_result[pf_result_index] - '0'];
			//            x = p_ptr.px + ddx[pf_result[pf_result_index] - '0'];

			//            /* Known wall */
			//            if ((cave.info[y][x] & (CAVE_MARK)) && !cave_floor_bold(y, x))
			//            {
			//                disturb(p_ptr, 0,0);
			//                p_ptr.running_withpathfind = false;
			//                return;
			//            }

			//            /* Get step after */
			//            y = y + ddy[pf_result[pf_result_index-1] - '0'];
			//            x = x + ddx[pf_result[pf_result_index-1] - '0'];

			//            /* Known wall */
			//            if ((cave.info[y][x] & (CAVE_MARK)) && !cave_floor_bold(y, x))
			//            {
			//                p_ptr.running_withpathfind = false;

			//                run_init(pf_result[pf_result_index] - '0');
			//            }
			//        }

			//        p_ptr.run_cur_dir = pf_result[pf_result_index--] - '0';
			//    }
			//}


			///* Decrease counter */
			//p_ptr.running--;

			///* Take time */
			//p_ptr.energy_use = 100;

			///* Move the player */
			//move_player(p_ptr.run_cur_dir, true);
		}
	}
}
