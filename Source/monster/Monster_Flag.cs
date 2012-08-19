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

		public static Monster_Flag NONE = new  Monster_Flag(0,        "");
		public static Monster_Flag UNIQUE = new  Monster_Flag(1,      "");
		public static Monster_Flag QUESTOR= new  Monster_Flag(2,     "");
		public static Monster_Flag MALE=  new Monster_Flag(3,        "");
		public static Monster_Flag FEMALE = new Monster_Flag(4,      "");
		public static Monster_Flag CHAR_CLEAR=  new Monster_Flag(5,  "");
		public static Monster_Flag ATTR_RAND = new Monster_Flag(6,	"");
		public static Monster_Flag ATTR_CLEAR = new Monster_Flag(7,  "");
		public static Monster_Flag ATTR_MULTI = new Monster_Flag(8,  "");
		public static Monster_Flag FORCE_DEPTH=  new Monster_Flag(9, "");
		public static Monster_Flag UNAWARE= new Monster_Flag(10,		"");
		public static Monster_Flag FORCE_SLEEP = new Monster_Flag(11, "");
		public static Monster_Flag FORCE_EXTRA= new  Monster_Flag(12, "");
		public static Monster_Flag FRIEND = new Monster_Flag(13,      "");
		public static Monster_Flag FRIENDS = new Monster_Flag(14,     "");
		public static Monster_Flag ESCORT=  new Monster_Flag(15,      "");
		public static Monster_Flag ESCORTS=  new Monster_Flag(16,     "");
		public static Monster_Flag NEVER_BLOW=  new Monster_Flag(17,  "");
		public static Monster_Flag NEVER_MOVE = new Monster_Flag(18,  "");
		public static Monster_Flag RAND_25 = new Monster_Flag(19,     "");
		public static Monster_Flag RAND_50 = new Monster_Flag(20,    "");
		public static Monster_Flag ONLY_GOLD= new  Monster_Flag(21,   "");
		public static Monster_Flag ONLY_ITEM=  new Monster_Flag(22,   "");
		public static Monster_Flag DROP_40 = new Monster_Flag(23,     "");
		public static Monster_Flag DROP_60 = new Monster_Flag(24,     "");
		public static Monster_Flag DROP_1 = new Monster_Flag(25,      "");
		public static Monster_Flag DROP_2 = new Monster_Flag(26,      "");
		public static Monster_Flag DROP_3 = new Monster_Flag(27,      "");
		public static Monster_Flag DROP_4 = new Monster_Flag(28,      "");
		public static Monster_Flag DROP_GOOD = new Monster_Flag(29,   "");
		public static Monster_Flag DROP_GREAT=  new Monster_Flag(30,  "");
		public static Monster_Flag DROP_20=  new Monster_Flag(31,     "");
		public static Monster_Flag XXX10 = new Monster_Flag(32,       "");
		public static Monster_Flag STUPID = new Monster_Flag(33,      "");
		public static Monster_Flag SMART = new Monster_Flag(34,       "");
		public static Monster_Flag HAS_LIGHT= new Monster_Flag(35,   "");
		public static Monster_Flag ATTR_FLICKER= new  Monster_Flag(36,"");
		public static Monster_Flag INVISIBLE = new Monster_Flag(37,   "");
		public static Monster_Flag COLD_BLOOD = new Monster_Flag(38,  "");
		public static Monster_Flag EMPTY_MIND=  new Monster_Flag(39,  "");
		public static Monster_Flag WEIRD_MIND = new Monster_Flag(40,  "");
		public static Monster_Flag MULTIPLY = new Monster_Flag(41,    "");
		public static Monster_Flag REGENERATE = new Monster_Flag(42,  "");
		public static Monster_Flag XXX1=  new Monster_Flag(43,        "");
		public static Monster_Flag XXX2 = new Monster_Flag(44,        "");
		public static Monster_Flag POWEMonster_FlagUL = new Monster_Flag(45,    "");
		public static Monster_Flag XXX3 = new Monster_Flag(46,        "");
		public static Monster_Flag XXX4 = new Monster_Flag(47,        "");
		public static Monster_Flag XXX5=  new Monster_Flag(48,        "");
		public static Monster_Flag OPEN_DOOR= new  Monster_Flag(49,   "");
		public static Monster_Flag BASH_DOOR = new Monster_Flag(50,   "");
		public static Monster_Flag PASS_WALL = new Monster_Flag(51,   "");
		public static Monster_Flag KILL_WALL = new Monster_Flag(52,   "");
		public static Monster_Flag MOVE_BODY = new Monster_Flag(53,   "");
		public static Monster_Flag KILL_BODY = new Monster_Flag(54,   "");
		public static Monster_Flag TAKE_ITEM = new Monster_Flag(55,   "");
		public static Monster_Flag KILL_ITEM = new Monster_Flag(56,   "");
		public static Monster_Flag BRAIN_1=  new Monster_Flag(57,     "");
		public static Monster_Flag BRAIN_2 = new Monster_Flag(58,     "");
		public static Monster_Flag BRAIN_3= new  Monster_Flag(59,     "");
		public static Monster_Flag BRAIN_4= new  Monster_Flag(60,     "");
		public static Monster_Flag BRAIN_5= new  Monster_Flag(61,     "");
		public static Monster_Flag BRAIN_6=  new Monster_Flag(62,     "");
		public static Monster_Flag BRAIN_7= new  Monster_Flag(63,     "");
		public static Monster_Flag BRAIN_8= new Monster_Flag(64,     "");
		public static Monster_Flag ORC = new Monster_Flag(65,         "");
		public static Monster_Flag TROLL = new Monster_Flag(66,       "");
		public static Monster_Flag GIANT = new Monster_Flag(67,       "");
		public static Monster_Flag DRAGON = new Monster_Flag(68,      "");
		public static Monster_Flag DEMON = new Monster_Flag(69,       "");
		public static Monster_Flag UNDEAD=  new Monster_Flag(70,      "");
		public static Monster_Flag EVIL = new Monster_Flag(71,        "");
		public static Monster_Flag ANIMAL=  new Monster_Flag(72,      "");
		public static Monster_Flag METAL = new Monster_Flag(73,       "");
		public static Monster_Flag XXX6 = new Monster_Flag(74,        "");
		public static Monster_Flag NONLIVING=  new Monster_Flag(75,	"");
		public static Monster_Flag XXX7 = new Monster_Flag(76,		"");
		public static Monster_Flag HURT_LIGHT=  new Monster_Flag(77,  "");
		public static Monster_Flag HURT_ROCK=  new Monster_Flag(78,   "");
		public static Monster_Flag HURT_FIRE=  new Monster_Flag(79,   "");
		public static Monster_Flag HURT_COLD=  new Monster_Flag(80,   "");
		public static Monster_Flag IM_ACID=  new Monster_Flag(81,     "");
		public static Monster_Flag IM_ELEC = new Monster_Flag(82,     "");
		public static Monster_Flag IM_FIRE = new Monster_Flag(83,     "");
		public static Monster_Flag IM_COLD = new Monster_Flag(84,     "");
		public static Monster_Flag IM_POIS = new Monster_Flag(85,     "");
		public static Monster_Flag XXX8 = new Monster_Flag(86,		"");
		public static Monster_Flag RES_NETH = new Monster_Flag(87,    "");
		public static Monster_Flag IM_WATER = new Monster_Flag(88,    "");
		public static Monster_Flag RES_PLAS = new Monster_Flag(89,    "");
		public static Monster_Flag RES_NEXUS = new Monster_Flag(90,   "");
		public static Monster_Flag RES_DISE = new Monster_Flag(91,    "");
		public static Monster_Flag XXX9 = new Monster_Flag(92,		"");
		public static Monster_Flag NO_FEAR = new Monster_Flag(93,     "");
		public static Monster_Flag NO_STUN = new Monster_Flag(94,     "");
		public static Monster_Flag NO_CONF = new Monster_Flag(95,     "");
		public static Monster_Flag NO_SLEEP = new Monster_Flag(96,    "");
		public static Monster_Flag MAX = new Monster_Flag(97,			"ERROR: Monster_Flag.MAX SHOULD NEVER BE SHOWN TO THE PLAYER");

		public static int SIZE{
			get {
				return Bitflag.FLAG_SIZE(MAX.value);
			}
		}
		public const int BYTES	 = 		   32; /* savefile bytes, i.e. 256 flags */
	}
}
