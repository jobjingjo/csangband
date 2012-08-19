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
	}
}
