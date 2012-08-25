using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Monster {
	class Monster_Flag {

		/*** Monster flags ***/

		/*
		 * Special Monster Flags (all temporary)
		 */
		public const int MFLAG_VIEW	= 0x01;	/* Monster is in line of sight */
		/* xxx */
		public const int MFLAG_NICE	= 0x20;	/* Monster is still being nice */
		public const int MFLAG_SHOW	= 0x40;	/* Monster is recently memorized */
		public const int MFLAG_MARK = 0x80;	/* Monster is currently memorized */


		public static List<Monster_Flag> list = new List<Monster_Flag>();
		/*
		 * Monster property and ability flags (race flags)
		 */
		public int value;
		public string desc;
		public Monster_Flag(int value, string text){
			this.value = value;
			this.desc = text;

			list.Add(this);
		}

		public static Monster_Flag NONE = new  Monster_Flag(0,			"NONE");
		public static Monster_Flag UNIQUE = new  Monster_Flag(1,		"UNIQUE");
		public static Monster_Flag QUESTOR = new Monster_Flag(2, "QUESTOR");
		public static Monster_Flag MALE = new Monster_Flag(3, "MALE");
		public static Monster_Flag FEMALE = new Monster_Flag(4, "FEMALE");
		public static Monster_Flag CHAR_CLEAR = new Monster_Flag(5, "CHAR_CLEAR");
		public static Monster_Flag ATTR_RAND = new Monster_Flag(6, "ATTR_RAND");
		public static Monster_Flag ATTR_CLEAR = new Monster_Flag(7, "ATTR_CLEAR");
		public static Monster_Flag ATTR_MULTI = new Monster_Flag(8, "ATTR_MULTI");
		public static Monster_Flag FORCE_DEPTH = new Monster_Flag(9, "FORCE_DEPTH");
		public static Monster_Flag UNAWARE = new Monster_Flag(10, "UNAWARE");
		public static Monster_Flag FORCE_SLEEP = new Monster_Flag(11, "FORCE_SLEEP");
		public static Monster_Flag FORCE_EXTRA = new Monster_Flag(12, "FORCE_EXTRA");
		public static Monster_Flag FRIEND = new Monster_Flag(13, "FRIEND");
		public static Monster_Flag FRIENDS = new Monster_Flag(14, "FRIENDS");
		public static Monster_Flag ESCORT = new Monster_Flag(15, "ESCORT");
		public static Monster_Flag ESCORTS = new Monster_Flag(16, "ESCORTS");
		public static Monster_Flag NEVER_BLOW = new Monster_Flag(17, "NEVER_BLOW");
		public static Monster_Flag NEVER_MOVE = new Monster_Flag(18, "NEVER_MOVE");
		public static Monster_Flag RAND_25 = new Monster_Flag(19, "RAND_25");
		public static Monster_Flag RAND_50 = new Monster_Flag(20, "RAND_50");
		public static Monster_Flag ONLY_GOLD = new Monster_Flag(21, "ONLY_GOLD");
		public static Monster_Flag ONLY_ITEM = new Monster_Flag(22, "ONLY_ITEM");
		public static Monster_Flag DROP_40 = new Monster_Flag(23, "DROP_40");
		public static Monster_Flag DROP_60 = new Monster_Flag(24, "DROP_60");
		public static Monster_Flag DROP_1 = new Monster_Flag(25, "DROP_1");
		public static Monster_Flag DROP_2 = new Monster_Flag(26, "DROP_2");
		public static Monster_Flag DROP_3 = new Monster_Flag(27, "DROP_3");
		public static Monster_Flag DROP_4 = new Monster_Flag(28, "DROP_4");
		public static Monster_Flag DROP_GOOD = new Monster_Flag(29, "DROP_GOOD");
		public static Monster_Flag DROP_GREAT = new Monster_Flag(30, "DROP_GREAT");
		public static Monster_Flag DROP_20 = new Monster_Flag(31, "DROP_20");
		public static Monster_Flag XXX10 = new Monster_Flag(32, "XXX10");
		public static Monster_Flag STUPID = new Monster_Flag(33, "STUPID");
		public static Monster_Flag SMART = new Monster_Flag(34, "SMART");
		public static Monster_Flag HAS_LIGHT = new Monster_Flag(35, "HAS_LIGHT");
		public static Monster_Flag ATTR_FLICKER = new Monster_Flag(36, "ATTR_FLICKER");
		public static Monster_Flag INVISIBLE = new Monster_Flag(37, "INVISIBLE");
		public static Monster_Flag COLD_BLOOD = new Monster_Flag(38, "COLD_BLOOD");
		public static Monster_Flag EMPTY_MIND = new Monster_Flag(39, "EMPTY_MIND");
		public static Monster_Flag WEIRD_MIND = new Monster_Flag(40, "WEIRD_MIND");
		public static Monster_Flag MULTIPLY = new Monster_Flag(41, "MULTIPLY");
		public static Monster_Flag REGENERATE = new Monster_Flag(42, "REGENERATE");
		public static Monster_Flag XXX1 = new Monster_Flag(43, "XXX1");
		public static Monster_Flag XXX2 = new Monster_Flag(44, "XXX2");
		public static Monster_Flag POWERFUL = new Monster_Flag(45, "POWERFUL");
		public static Monster_Flag XXX3 = new Monster_Flag(46, "XXX3");
		public static Monster_Flag XXX4 = new Monster_Flag(47, "XXX4");
		public static Monster_Flag XXX5 = new Monster_Flag(48, "XXX5");
		public static Monster_Flag OPEN_DOOR = new Monster_Flag(49, "OPEN_DOOR");
		public static Monster_Flag BASH_DOOR = new Monster_Flag(50, "BASH_DOOR");
		public static Monster_Flag PASS_WALL = new Monster_Flag(51, "PASS_WALL");
		public static Monster_Flag KILL_WALL = new Monster_Flag(52, "KILL_WALL");
		public static Monster_Flag MOVE_BODY = new Monster_Flag(53, "MOVE_BODY");
		public static Monster_Flag KILL_BODY = new Monster_Flag(54, "KILL_BODY");
		public static Monster_Flag TAKE_ITEM = new Monster_Flag(55, "TAKE_ITEM");
		public static Monster_Flag KILL_ITEM = new Monster_Flag(56, "KILL_ITEM");
		public static Monster_Flag BRAIN_1 = new Monster_Flag(57, "BRAIN_1");
		public static Monster_Flag BRAIN_2 = new Monster_Flag(58, "BRAIN_2");
		public static Monster_Flag BRAIN_3 = new Monster_Flag(59, "BRAIN_3");
		public static Monster_Flag BRAIN_4 = new Monster_Flag(60, "BRAIN_4");
		public static Monster_Flag BRAIN_5 = new Monster_Flag(61, "BRAIN_5");
		public static Monster_Flag BRAIN_6 = new Monster_Flag(62, "BRAIN_6");
		public static Monster_Flag BRAIN_7 = new Monster_Flag(63, "BRAIN_7");
		public static Monster_Flag BRAIN_8 = new Monster_Flag(64, "BRAIN_8");
		public static Monster_Flag ORC = new Monster_Flag(65, "ORC");
		public static Monster_Flag TROLL = new Monster_Flag(66, "TROLL");
		public static Monster_Flag GIANT = new Monster_Flag(67, "GIANT");
		public static Monster_Flag DRAGON = new Monster_Flag(68, "DRAGON");
		public static Monster_Flag DEMON = new Monster_Flag(69, "DEMON");
		public static Monster_Flag UNDEAD = new Monster_Flag(70, "UNDEAD");
		public static Monster_Flag EVIL = new Monster_Flag(71, "EVIL");
		public static Monster_Flag ANIMAL = new Monster_Flag(72, "ANIMAL");
		public static Monster_Flag METAL = new Monster_Flag(73, "METAL");
		public static Monster_Flag XXX6 = new Monster_Flag(74, "XXX6");
		public static Monster_Flag NONLIVING = new Monster_Flag(75, "NONLIVING");
		public static Monster_Flag XXX7 = new Monster_Flag(76, "XXX7");
		public static Monster_Flag HURT_LIGHT = new Monster_Flag(77, "HURT_LIGHT");
		public static Monster_Flag HURT_ROCK = new Monster_Flag(78, "HURT_ROCK");
		public static Monster_Flag HURT_FIRE = new Monster_Flag(79, "HURT_FIRE");
		public static Monster_Flag HURT_COLD = new Monster_Flag(80, "HURT_COLD");
		public static Monster_Flag IM_ACID = new Monster_Flag(81, "IM_ACID");
		public static Monster_Flag IM_ELEC = new Monster_Flag(82, "IM_ELEC");
		public static Monster_Flag IM_FIRE = new Monster_Flag(83, "IM_FIRE");
		public static Monster_Flag IM_COLD = new Monster_Flag(84, "IM_COLD");
		public static Monster_Flag IM_POIS = new Monster_Flag(85, "IM_POIS");
		public static Monster_Flag XXX8 = new Monster_Flag(86, "XXX8");
		public static Monster_Flag RES_NETH = new Monster_Flag(87, "RES_NETH");
		public static Monster_Flag IM_WATER = new Monster_Flag(88, "IM_WATER");
		public static Monster_Flag RES_PLAS = new Monster_Flag(89, "RES_PLAS");
		public static Monster_Flag RES_NEXUS = new Monster_Flag(90, "RES_NEXUS");
		public static Monster_Flag RES_DISE = new Monster_Flag(91, "RES_DISE");
		public static Monster_Flag XXX9 = new Monster_Flag(92, "XXX9");
		public static Monster_Flag NO_FEAR = new Monster_Flag(93, "NO_FEAR");
		public static Monster_Flag NO_STUN = new Monster_Flag(94, "NO_STUN");
		public static Monster_Flag NO_CONF = new Monster_Flag(95, "NO_CONF");
		public static Monster_Flag NO_SLEEP = new Monster_Flag(96, "NO_SLEEP");
		public static Monster_Flag MAX = new Monster_Flag(97,			"ERROR: Monster_Flag.MAX SHOULD NEVER BE SHOWN TO THE PLAYER");

		public static int SIZE{
			get {
				return Bitflag.FLAG_SIZE(MAX.value);
			}
		}
		public const int BYTES	 = 		   32; /* savefile bytes, i.e. 256 flags */
	}
}
