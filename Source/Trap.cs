using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Trap {
		/* Places a trap. All traps are untyped until discovered. */
		public static void place_trap(Cave c, int y, int x)
		{
			Misc.assert(Cave.cave_in_bounds(c, y, x));
			Misc.assert(Cave.cave_isempty(c, y, x));

			/* Place an invisible trap */
			Cave.cave_set_feat(c, y, x, Cave.FEAT_INVIS);
		}
	}
}
