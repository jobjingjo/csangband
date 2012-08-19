using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Player {
	class Player_Magic {
		public Player_Magic() {
			for(int i = 0; i < info.Length; i++) {
				info[i] = new Magic_Type();
			}
		}
		/*
		 * Information about the player's "magic"
		 *
		 * Note that a player with a "spell_book" of "zero" is illiterate.
		 */
		public Magic_Type[] info = new Magic_Type[(int)Misc.PY_MAX_SPELLS];	/* The available spells */
	}
}
