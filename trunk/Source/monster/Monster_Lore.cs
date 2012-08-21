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
		public Int16 deaths;			/* Count deaths from this monster */

		public Int16 pkills;			/* Count monsters killed in this life */
		public Int16 tkills;			/* Count monsters killed in all lives */

		public byte wake;				/* Number of times woken up (?) */
		public byte ignore;			/* Number of times ignored (?) */

		byte drop_gold;			/* Max number of gold dropped at once */
		byte drop_item;			/* Max number of item dropped at once */

		public byte cast_innate;		/* Max number of innate spells seen */
		public byte cast_spell;		/* Max number of other spells seen */

		public byte[] blows = new byte[Monster_Blow.MONSTER_BLOW_MAX]; /* Number of times each blow type was seen */

		public Bitflag flags = new Bitflag(Monster_Flag.SIZE); /* Observed racial flags - a 1 indicates
								 * the flag (or lack thereof) is known to
								 * the player */
		public Bitflag spell_flags = new Bitflag(Monster_Spell_Flag.SIZE);  /* Observed racial spell flags */

		/*
		 * Take note that the given monster just dropped some treasure
		 *
		 * Note that learning the "GOOD"/"GREAT" flags gives information
		 * about the treasure (even when the monster is killed for the first
		 * time, such as uniques, and the treasure has not been examined yet).
		 *
		 * This "indirect" method is used to prevent the player from learning
		 * exactly how much treasure a monster can drop from observing only
		 * a single example of a drop.  This method actually observes how much
		 * gold and items are dropped, and remembers that information to be
		 * described later by the monster recall code.
		 */
		public static void lore_treasure(int m_idx, int num_item, int num_gold)
		{
			throw new NotImplementedException();
			//monster_type *m_ptr = cave_monster(cave, m_idx);
			//monster_lore *l_ptr = &l_list[m_ptr.r_idx];


			///* Note the number of things dropped */
			//if (num_item > l_ptr.drop_item) l_ptr.drop_item = num_item;
			//if (num_gold > l_ptr.drop_gold) l_ptr.drop_gold = num_gold;

			///* Learn about drop quality */
			//rf_on(l_ptr.flags, RF_DROP_GOOD);
			//rf_on(l_ptr.flags, RF_DROP_GREAT);

			///* Update monster recall window */
			//if (p_ptr.monster_race_idx == m_ptr.r_idx)
			//{
			//    /* Window stuff */
			//    p_ptr.redraw |= (PR_MONSTER);
			//}
		}
	}
}
