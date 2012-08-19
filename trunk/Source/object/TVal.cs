using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Object {
	//Split from TValSVal
	class TVal {
		/*** Object "tval" and "sval" codes ***/

		/*
		 * PS: to regenerate the inside of an sval enum, do this:
		 * $ grep --context 2 I:<TVAL> object.txt | grep ^[NI] |
		 *   perl -pe 'y/[a-z]\- /[A-Z]__/' | perl -pe 's/(&_|~|')//g' | cut -d: -f3 |
		 *   perl -00 -pe 's/([^\n]*)\n([^\n]*)\n/\tSV_\1\ =\ \2,\n/g'
		 */

		/*
		 * The values for the "tval" field of various objects.
		 *
		 * This value is the primary means by which items are sorted in the
		 * player inventory, followed by "sval" and "cost".
		 *
		 * Note that a "BOW" with tval = 19 and sval S = 10*N+P takes a missile
		 * weapon with tval = 16+N, and does (xP) damage when so combined.  This
		 * fact is not actually used in the source, but it kind of interesting.
		 *
		 * Note that as of 2.7.8, the "item flags" apply to all items, though
		 * only armor and weapons and a few other items use any of these flags.
		 */

		public const int TV_null = 0;
		public const int TV_SKELETON = 1;	/* Skeletons ('s') */
		public const int TV_BOTTLE = 2;	/* Empty bottles ('!') */
		public const int TV_JUNK = 3;	/* Sticks, Pottery, etc ('~') */
		public const int TV_SPIKE = 5;	/* Spikes ('~') */
		public const int TV_CHEST = 7;	/* Chests ('~') */
		public const int TV_SHOT = 16;	/* Ammo for slings */
		public const int TV_ARROW = 17;	/* Ammo for bows */
		public const int TV_BOLT = 18;	/* Ammo for x-bows */
		public const int TV_BOW = 19;	/* Slings/Bows/Xbows */
		public const int TV_DIGGING = 20;	/* Shovels/Picks */
		public const int TV_HAFTED = 21;	/* Priest Weapons */
		public const int TV_POLEARM = 22;	/* Axes and Pikes */
		public const int TV_SWORD = 23;	/* Edged Weapons */
		public const int TV_BOOTS = 30;	/* Boots */
		public const int TV_GLOVES = 31;	/* Gloves */
		public const int TV_HELM = 32;	/* Helms */
		public const int TV_CROWN = 33;	/* Crowns */
		public const int TV_SHIELD = 34;	/* Shields */
		public const int TV_CLOAK = 35;	/* Cloaks */
		public const int TV_SOFT_ARMOR = 36;	/* Soft Armor */
		public const int TV_HARD_ARMOR = 37;	/* Hard Armor */
		public const int TV_DRAG_ARMOR = 38;	/* Dragon Scale Mail */
		public const int TV_LIGHT = 39;	/* Lights (including Specials) */
		public const int TV_AMULET = 40;	/* Amulets (including Specials) */
		public const int TV_RING = 45;	/* Rings (including Specials) */
		public const int TV_STAFF = 55;
		public const int TV_WAND = 65;
		public const int TV_ROD = 66;
		public const int TV_SCROLL = 70;
		public const int TV_POTION = 75;
		public const int TV_FLASK = 77;
		public const int TV_FOOD = 80;
		public const int TV_MAGIC_BOOK = 90;
		public const int TV_PRAYER_BOOK = 91;
		public const int TV_GOLD = 100;	/* Gold can only be picked up by players */
		public const int TV_MAX = 101;


		/*** Textual<.numeric conversion ***/

		/**
		 * List of { tval, name } pairs.
		 */
		public static Grouper[] tval_names = new Grouper[]
		{
			new Grouper( TV_SKELETON,    "skeleton" ),
			new Grouper( TV_BOTTLE,      "bottle" ),
			new Grouper( TV_JUNK,        "junk" ),
			new Grouper( TV_SPIKE,       "spike" ),
			new Grouper( TV_CHEST,       "chest" ),
			new Grouper( TV_SHOT,        "shot" ),
			new Grouper( TV_ARROW,       "arrow" ),
			new Grouper( TV_BOLT,        "bolt" ),
			new Grouper( TV_BOW,         "bow" ),
			new Grouper( TV_DIGGING,     "digger" ),
			new Grouper( TV_HAFTED,      "hafted" ),
			new Grouper( TV_POLEARM,     "polearm" ),
			new Grouper( TV_SWORD,       "sword" ),
			new Grouper( TV_BOOTS,       "boots" ),
			new Grouper( TV_GLOVES,      "gloves" ),
			new Grouper( TV_HELM,        "helm" ),
			new Grouper( TV_CROWN,       "crown" ),
			new Grouper( TV_SHIELD,      "shield" ),
			new Grouper( TV_CLOAK,       "cloak" ),
			new Grouper( TV_SOFT_ARMOR,  "soft armor" ),
			new Grouper( TV_SOFT_ARMOR,  "soft armour" ),
			new Grouper( TV_HARD_ARMOR,  "hard armor" ),
			new Grouper( TV_HARD_ARMOR,  "hard armour" ),
			new Grouper( TV_DRAG_ARMOR,  "dragon armor" ),
			new Grouper( TV_DRAG_ARMOR,  "dragon armour" ),
			new Grouper( TV_LIGHT,       "light" ),
			new Grouper( TV_AMULET,      "amulet" ),
			new Grouper( TV_RING,        "ring" ),
			new Grouper( TV_STAFF,       "staff" ),
			new Grouper( TV_WAND,        "wand" ),
			new Grouper( TV_ROD,         "rod" ),
			new Grouper( TV_SCROLL,      "scroll" ),
			new Grouper( TV_POTION,      "potion" ),
			new Grouper( TV_FLASK,       "flask" ),
			new Grouper( TV_FOOD,        "food" ),
			new Grouper( TV_MAGIC_BOOK,  "magic book" ),
			new Grouper( TV_PRAYER_BOOK, "prayer book" ),
			new Grouper( TV_GOLD,        "gold" )
		};
		/**
		 * Returns the numeric equivalent tval of the textual tval `name`.
		 */
		public static int find_idx(string name)
		{
			uint r;

			if (uint.TryParse(name, out r))
				return (int)r;

			for (int i = 0; i < tval_names.Length; i++)
			{
				if (name == tval_names[i].name)
					return tval_names[i].tval;
			}

			return -1;
		}

		
		/**
		 * Returns the textual equivalent tval of the numeric tval `name`.
		 */
		public static string find_name(int tval)
		{
			for (int i = 0; i < tval_names.Length; i++)
			{
				if (tval == tval_names[i].tval)
					return tval_names[i].name;
			}

			return "unknown";
		}
	}
}
