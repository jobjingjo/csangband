using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	/*
	 * Skill indexes
	 */
	public enum Skill : int
	{
		DISARM = 0,			/* Skill: Disarming */
		DEVICE,				/* Skill: Magic Devices */
		SAVE,				/* Skill: Saving throw */
		STEALTH,			/* Skill: Stealth factor */
		SEARCH,				/* Skill: Searching ability */
		SEARCH_FREQUENCY,	/* Skill: Searching frequency */
		TO_HIT_MELEE,		/* Skill: To hit (normal) */
		TO_HIT_BOW,			/* Skill: To hit (shooting) */
		TO_HIT_THROW,		/* Skill: To hit (throwing) */
		DIGGING,			/* Skill: Digging */

		MAX
	}
}
