using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Quest {
		/*
		 * Structure for the "quests"
		 *
		 * Hack -- currently, only the "level" parameter is set, with the
		 * semantics that "one (QUEST) monster of that level" must be killed,
		 * and then the "level" is reset to zero, meaning "all done".  Later,
		 * we should allow quests like "kill 100 fire hounds", and note that
		 * the "quest level" is then the level past which progress is forbidden
		 * until the quest is complete.  Note that the "QUESTOR" flag then could
		 * become a more general "never out of depth" flag for monsters.
		 */
		public byte level;		/* Dungeon level */
		public int r_idx;		/* Monster race */

		public int cur_num;	/* Number killed (unused) */
		public int max_num;	/* Number required (unused) */
	}
}
