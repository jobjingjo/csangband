using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Monster {
	/*
	 * Monster "lore" information
	 *
	 * Note that these fields are related to the "monster recall" and can
	 * be scrapped if space becomes an issue, resulting in less "complete"
	 * monster recall (no knowledge of spells, etc). XXX XXX XXX
	 */
	class Monster_Lore {
		public Int16 sights;			/* Count sightings of this monster */
		Int16 deaths;			/* Count deaths from this monster */

		public Int16 pkills;			/* Count monsters killed in this life */
		public Int16 tkills;			/* Count monsters killed in all lives */

		public byte wake;				/* Number of times woken up (?) */
		public byte ignore;			/* Number of times ignored (?) */

		byte drop_gold;			/* Max number of gold dropped at once */
		byte drop_item;			/* Max number of item dropped at once */

		byte cast_innate;		/* Max number of innate spells seen */
		byte cast_spell;		/* Max number of other spells seen */

		byte[] blows = new byte[Monster_Blow.MONSTER_BLOW_MAX]; /* Number of times each blow type was seen */

		public Bitflag flags = new Bitflag(Monster_Flag.SIZE); /* Observed racial flags - a 1 indicates
								 * the flag (or lack thereof) is known to
								 * the player */
		public Bitflag spell_flags = new Bitflag(Monster_Spell_Flag.SIZE);  /* Observed racial spell flags */
	}
}
