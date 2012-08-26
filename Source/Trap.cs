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

		/*
		 * Handle player hitting a real trap
		 */
		public static void hit_trap(int y, int x)
		{
			bool ident;
			Feature trap = Misc.f_info[Cave.cave.feat[y][x]];

			/* Disturb the player */
			Cave.disturb(Misc.p_ptr, 0, 0);

			/* Run the effect */
			trap.effect.effect_do(out ident, false, 0, 0, 0);
		}

		/*
		 * Hack -- instantiate a trap
		 *
		 * XXX XXX XXX This routine should be redone to reflect trap "level".
		 * That is, it does not make sense to have spiked pits at 50 feet.
		 * Actually, it is not this routine, but the "trap instantiation"
		 * code, which should also check for "trap doors" on quest levels.
		 */
		public static void pick_trap(int y, int x)
		{
			int feat;

			int[] min_level =
			{
				2,		/* Trap door */
				2,		/* Open pit */
				2,		/* Spiked pit */
				2,		/* Poison pit */
				3,		/* Summoning rune */
				1,		/* Teleport rune */
				2,		/* Fire rune */
				2,		/* Acid rune */
				2,		/* Slow rune */
				6,		/* Strength dart */
				6,		/* Dexterity dart */
				6,		/* Constitution dart */
				2,		/* Gas blind */
				1,		/* Gas confuse */
				2,		/* Gas poison */
				2,		/* Gas sleep */
			};

			/* Paranoia */
			if (Cave.cave.feat[y][x] != Cave.FEAT_INVIS) return;

			/* Pick a trap */
			while (true)
			{
				/* Hack -- pick a trap */
				feat = Cave.FEAT_TRAP_HEAD + Random.randint0(16);

				/* Check against minimum depth */
				if (min_level[feat - Cave.FEAT_TRAP_HEAD] > Misc.p_ptr.depth) continue;

				/* Hack -- no trap doors on quest levels */
				if ((feat == Cave.FEAT_TRAP_HEAD + 0x00) && Cave.is_quest(Misc.p_ptr.depth)) continue;

				/* Hack -- no trap doors on the deepest level */
				if ((feat == Cave.FEAT_TRAP_HEAD + 0x00) && (Misc.p_ptr.depth >= Misc.MAX_DEPTH-1)) continue;

				/* Done */
				break;
			}

			/* Activate the trap */
			Cave.cave_set_feat(Cave.cave, y, x, feat);
		}
	}
}
