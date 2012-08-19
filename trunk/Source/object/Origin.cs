using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Object {
	public enum Origin{
		NONE = 0,
		FLOOR,			/* found on the dungeon floor */
		DROP,			/* normal monster drops */
		CHEST,
		DROP_SPECIAL,	/* from monsters in special rooms */
		DROP_PIT,		/* from monsters in pits/nests */
		DROP_VAULT,		/* from monsters in vaults */
		SPECIAL,			/* on the floor of a special room */
		PIT,				/* on the floor of a pit/nest */
		VAULT,			/* on the floor of a vault */
		LABYRINTH,		/* on the floor of a labyrinth */
		CAVERN,			/* on the floor of a cavern */
		RUBBLE,			/* found under rubble */
		MIXED,			/* stack with mixed origins */
		STATS,			/* ^ only the above are considered by main-stats */
		ACQUIRE,			/* called forth by scroll */
		DROP_BREED,		/* from breeders */
		DROP_SUMMON,		/* from combat summons */
		STORE,			/* something you bought */
		STOLEN,			/* stolen by monster (used only for gold) */
		BIRTH,			/* objects created at character birth */
		DROP_UNKNOWN,	/* drops from unseen foes */
		CHEAT,			/* created by wizard mode */
		DROP_POLY,		/* from polymorphees */
		DROP_WIZARD,		/* from wizard mode summons */

		MAX
	};
}
