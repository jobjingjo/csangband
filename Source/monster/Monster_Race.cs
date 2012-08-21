using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Monster {
	/*
	 * Monster "race" information, including racial memories
	 *
	 * Note that "d_attr" and "d_char" are used for MORE than "visual" stuff.
	 *
	 * Note that "x_attr" and "x_char" are used ONLY for "visual" stuff.
	 *
	 * Note that "cur_num" (and "max_num") represent the number of monsters
	 * of the given race currently on (and allowed on) the current level.
	 * This information yields the "dead" flag for Unique monsters.
	 *
	 * Note that "max_num" is reset when a new player is created.
	 * Note that "cur_num" is reset when a new level is created.
	 *
	 * Maybe "x_attr", "x_char", "cur_num", and "max_num" should
	 * be moved out of this array since they are not read from
	 * "monster.txt".
	 */
	class Monster_Race {
		public Monster_Race() {
			for (int i = 0; i < blow.Length; i++){
				blow[i] = new Monster_Blow();
			}
		}

		public Monster_Race Next;

		public UInt32 ridx;

		public string Name;
		public string Text;

		public Monster_Base Base;

		public UInt16 avg_hp;				/* Average HP for this creature */

		public Int16 ac;				/* Armour Class */

		public Int16 sleep;				/* Inactive counter (base) */
		public byte aaf;				/* Area affect radius (1-100) */
		public byte speed;				/* Speed (normally 110) */

		public Int32 mexp;				/* Exp value for kill */

		public long power;				/* Monster power */
		public long scaled_power;		/* Monster power scaled by level */

		public Int16 highest_threat;	/* Monster highest threat */
	
		/*AMF:DEBUG*/			/**/
		public long melee_dam;			/**/
		public long spell_dam;			/**/
		public long hp;				/**/
		/*END AMF:DEBUG*/		/**/

		public byte freq_innate;		/* Innate spell frequency */
		public byte freq_spell;		/* Other spell frequency */

		public Bitflag flags = new Bitflag(Monster_Flag.SIZE);         /* Flags */
		public Bitflag spell_flags = new Bitflag(Monster_Spell_Flag.SIZE);  /* Spell flags */

		public Monster_Blow[] blow = new Monster_Blow[Monster_Blow.MONSTER_BLOW_MAX]; /* Up to four blows per round */

		public byte level;				/* Level of creature */
		public byte rarity;			/* Rarity of creature */

		public ConsoleColor d_attr;			/* Default monster attribute */
		public char d_char;			/* Default monster character */

		public ConsoleColor x_attr;			/* Desired monster attribute */
		public char x_char;			/* Desired monster character */

		public byte max_num;			/* Maximum population allowed per level */
		public byte cur_num;			/* Monster population on current level */

		public Monster_Drop drops;
	
		public Monster_Mimic mimic_kinds;

		/*
		 * Some monster types are different.
		 */
		public bool monster_is_unusual(){
			return flags.test(Monster_Flag.DEMON.value, Monster_Flag.UNDEAD.value, Monster_Flag.STUPID.value, 
				Monster_Flag.NONLIVING.value);
		}

		public bool monster_is_nonliving(){
			return flags.test(Monster_Flag.DEMON.value, Monster_Flag.UNDEAD.value, Monster_Flag.NONLIVING.value);
		}
	}
}
