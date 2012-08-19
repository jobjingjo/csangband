using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Object {
	//Split from TValSVal
	class SVal {
		/* The "sval" codes for TV_SHOT/TV_ARROW/TV_BOLT */
		public const int SV_AMMO_LIGHT = 0;	/* pebbles */
		public const int SV_AMMO_NORMAL = 1;	/* shots, arrows, bolts */
		public const int SV_AMMO_HEAVY = 2;	/* seeker arrows and bolts */
		public const int SV_AMMO_SILVER = 3;	/* silver arrows and bolts */

		/* The "sval" codes for TV_BOW (note information in "sval") */
		public const int SV_SLING = 2;	/* (x2) */
		public const int SV_SHORT_BOW = 12;	/* (x2) */
		public const int SV_LONG_BOW = 13;	/* (x3) */
		public const int SV_LIGHT_XBOW = 23;	/* (x3) */
		public const int SV_HEAVY_XBOW = 24;	/* (x4) */

		/* The sval codes for TV_LIGHT */
		public const int SV_LIGHT_TORCH = 0;
		public const int SV_LIGHT_LANTERN = 1;
		public const int SV_LIGHT_GALADRIEL = 4;
		public const int SV_LIGHT_ELENDIL = 5;
		public const int SV_LIGHT_THRAIN = 6;
		public const int SV_LIGHT_PALANTIR = 7;

		/* Hacky defines */
		public const int SV_SCROLL_PHASE_DOOR = 1;
		public const int SV_SCROLL_WORD_OF_RECALL = 29;
		public const int SV_SCROLL_RUNE_OF_PROTECTION = 38;


		public enum sval_digging /* tval 20 */
		{
			SV_SHOVEL = 1,
			SV_PICK = 2,
			SV_MATTOCK = 3
		};

		public enum sval_hafted /* tval 21 */
		{
			SV_WHIP = 1,
			SV_BALL_AND_CHAIN = 2,
			SV_MORNING_STAR = 3,
			SV_FLAIL = 4,
			SV_TWO_HANDED_GREAT_FLAIL = 5,
			SV_MACE = 10,
			SV_LEAD_FILLED_MACE = 11,
			SV_QUARTERSTAFF = 12,
			SV_WAR_HAMMER = 13,
			SV_MAUL = 14,
			SV_GREAT_HAMMER = 15,
			SV_MACE_OF_DISRUPTION = 20,
			SV_GROND = 50
		};

		public enum sval_polearm /* tval 22 */
		{
			SV_SPEAR = 1,
			SV_AWL_PIKE = 2,
			SV_TRIDENT = 3,
			SV_HALBERD = 4,
			SV_PIKE = 5,
			SV_BEAKED_AXE = 10,
			SV_BROAD_AXE = 11,
			SV_BATTLE_AXE = 12,
			SV_LOCHABER_AXE = 13,
			SV_GREAT_AXE = 14,
			SV_SCYTHE = 20,
			SV_GLAIVE = 21,
			SV_LANCE = 22,
			SV_SCYTHE_OF_SLICING = 23,
			SV_LUCERNE_HAMMER = 24
		};

		public enum sval_sword /* tval 23 */
		{
			SV_DAGGER = 1,
			SV_MAIN_GAUCHE = 2,
			SV_RAPIER = 3,
			SV_SHORT_SWORD = 4,
			SV_CUTLASS = 5,
			SV_TULWAR = 6,
			SV_SCIMITAR = 7,
			SV_LONG_SWORD = 8,
			SV_BROAD_SWORD = 9,
			SV_BASTARD_SWORD = 10,
			SV_KATANA = 11,
			SV_ZWEIHANDER = 12,
			SV_EXECUTIONERS_SWORD = 13,
			SV_BLADE_OF_CHAOS = 14
		};

		public enum sval_boots /* tval 30 */
		{
			SV_PAIR_OF_LEATHER_SANDALS = 1,
			SV_PAIR_OF_LEATHER_BOOTS = 2,
			SV_PAIR_OF_IRON_SHOD_BOOTS = 3,
			SV_PAIR_OF_STEEL_SHOD_BOOTS = 4,
			SV_PAIR_OF_MITHRIL_SHOD_BOOTS = 5,
			SV_PAIR_OF_ETHEREAL_SLIPPERS = 6
		};

		public enum sval_gloves /* tval 31 */
		{
			SV_SET_OF_LEATHER_GLOVES = 1,
			SV_SET_OF_GAUNTLETS = 2,
			SV_SET_OF_MITHRIL_GAUNTLETS = 3,
			SV_SET_OF_CAESTUS = 4,
			SV_SET_OF_ALCHEMISTS_GLOVES = 5
		};

		public enum sval_helm /* tval 32 */
		{
			SV_HARD_LEATHER_CAP = 2,
			SV_METAL_CAP = 3,
			SV_IRON_HELM = 5,
			SV_STEEL_HELM = 6
		};

		public enum sval_crown /* tval 33 */
		{
			SV_IRON_CROWN = 10,
			SV_GOLDEN_CROWN = 11,
			SV_JEWEL_ENCRUSTED_CROWN = 12,
			SV_MORGOTH = 50
		};

		public enum sval_shield /* tval 34 */
		{
		/*	SV_BUCKLER = 1, */
			SV_WICKER_SHIELD = 2,
			SV_SMALL_METAL_SHIELD = 3,
			SV_LEATHER_SHIELD = 4,
			SV_LARGE_METAL_SHIELD = 5,
			SV_MITHRIL_SHIELD = 10
		};

		public enum sval_cloak /* tval 35 */
		{
			SV_CLOAK = 1,
			SV_FUR_CLOAK = 2,
			SV_ELVEN_CLOAK = 3,
			SV_ETHEREAL_CLOAK = 4
		};

		public enum sval_soft_armour /* tval 36 */
		{
			SV_ROBE = 2,
			SV_SOFT_LEATHER_ARMOUR = 4,
			SV_STUDDED_LEATHER_ARMOUR = 7,
			SV_HARD_LEATHER_ARMOUR = 6,
			SV_LEATHER_SCALE_MAIL = 11
		};

		public enum sval_hard_armour /* tval 37 */
		{
			SV_METAL_SCALE_MAIL = 1,
			SV_CHAIN_MAIL = 2,
			SV_AUGMENTED_CHAIN_MAIL = 3,
			SV_BAR_CHAIN_MAIL = 4,
			SV_METAL_BRIGANDINE_ARMOUR = 5,
			SV_PARTIAL_PLATE_ARMOUR = 6,
			SV_METAL_LAMELLAR_ARMOUR = 7,
			SV_FULL_PLATE_ARMOUR = 8,
			SV_RIBBED_PLATE_ARMOUR = 9,
			SV_MITHRIL_CHAIN_MAIL = 10,
			SV_MITHRIL_PLATE_MAIL = 11,
			SV_ADAMANTITE_PLATE_MAIL = 12
		};

		public enum sval_dragon_armour /* tval 38 */
		{
			SV_DRAGON_BLACK = 1,
			SV_DRAGON_BLUE	= 2,
			SV_DRAGON_WHITE = 3,
			SV_DRAGON_RED = 4,
			SV_DRAGON_GREEN = 5,
			SV_DRAGON_MULTIHUED = 6,
			SV_DRAGON_SHINING = 10,
			SV_DRAGON_LAW = 12,
			SV_DRAGON_BRONZE = 14,
			SV_DRAGON_GOLD = 16,
			SV_DRAGON_CHAOS = 18,
			SV_DRAGON_BALANCE = 20,
			SV_DRAGON_POWER = 30
		};

		public enum sval_amulet /* tval 40 */
		{
			SV_AMULET_WISDOM = 1,
			SV_AMULET_CHARISMA = 2,
			SV_AMULET_RESIST_LIGHTNING = 3,
			SV_AMULET_RESIST_ACID = 4,
			SV_AMULET_RESISTANCE = 5,
			SV_AMULET_SUSTENANCE = 6,
			SV_AMULET_THE_MAGI = 7,
			SV_AMULET_ESP = 8,
			SV_AMULET_DEVOTION = 9,
			SV_AMULET_WEAPONMASTERY = 10,
			SV_AMULET_TRICKERY = 11,
			SV_AMULET_REGENERATION = 15,
			SV_AMULET_INFRAVISION = 16,
			SV_AMULET_SEARCHING = 17,
			SV_AMULET_TELEPORTATION = 18,
			SV_AMULET_SLOW_DIGESTION = 19,
			SV_AMULET_ADORNMENT = 20,
			SV_AMULET_INERTIA = 21,
			SV_AMULET_CARLAMMAS = 50,
			SV_AMULET_INGWE = 51,
			SV_AMULET_DWARVES = 52,
			SV_AMULET_ELESSAR = 53,
			SV_AMULET_EVENSTAR = 54
		};

		public enum sval_ring /* tval 45 */
		{
			SV_RING_STRENGTH = 1,
			SV_RING_INTELLIGENCE = 2,
			SV_RING_DEXTERITY = 3,
			SV_RING_CONSTITUTION = 4,
			SV_RING_SPEED = 5,
			SV_RING_SEARCHING = 6,
			SV_RING_BODYKEEPING = 9,
			SV_RING_SOULKEEPING = 10,
			SV_RING_SUSTAIN_CHARISMA = 11,
			SV_RING_RESIST_POISON = 12,
			SV_RING_RESIST_FIRE = 13,
			SV_RING_RESIST_COLD = 14,
			SV_RING_LIGHT = 16,
			SV_RING_DARK = 17,
			SV_RING_FLAMES = 18,
			SV_RING_ACID = 19,
			SV_RING_ICE = 20,
			SV_RING_LIGHTNING = 21,
			SV_RING_DAMAGE = 23,
			SV_RING_ACCURACY = 24,
			SV_RING_SLAYING = 25,
			SV_RING_PROTECTION = 26,
			SV_RING_TELEPORTATION = 27,
			SV_RING_RECKLESS_ATTACKS = 28,
			SV_RING_OPEN_WOUNDS = 29,
			SV_RING_OF_ESCAPING = 30,
			SV_RING_OF_THE_MOUSE = 31,
			SV_RING_OF_THE_DOG = 32,
			SV_RING_SLOW_DIGESTION = 34,
			SV_RING_FEATHER_FALLING = 35,
			SV_RING_FREE_ACTION = 36,
			SV_RING_SEE_INVISIBLE = 37,
			SV_RING_DELVING = 38,

			/* Artifact rings */
			SV_RING_BARAHIR = 50,
			SV_RING_TULKAS = 51,
			SV_RING_NARYA = 52,
			SV_RING_NENYA = 53,
			SV_RING_VILYA = 54,
			SV_RING_POWER = 55
		};

		public enum sval_food /* tval 80 */
		{
			SV_FOOD_RATION = 1,
			SV_FOOD_SLIME_MOLD = 2,
			SV_FOOD_WAYBREAD = 3
		};

		public enum sval_gold /* tval 100 */
		{
			SV_GOLD_ANY = -1,
			SV_COPPER = 0,
			SV_SILVER = 1,
			SV_GARNETS = 2,
			SV_GOLD = 3,
			SV_OPALS = 4,
			SV_SAPPHIRES = 5,
			SV_RUBIES = 6,
			SV_DIAMONDS = 7,
			SV_EMERALDS = 8,
			SV_MITHRIL = 9,
			SV_ADAMANTITE = 10,

			SV_GOLD_MAX
		};


		/*
		 * Special "sval" limit -- first "normal" food
		 */
		public const int SV_FOOD_MIN_SHROOM = 5;

		/*
		 * Special "sval" limit -- first "large" chest
		 */
		public const int SV_CHEST_MIN_LARGE = 4;

		/*
		 * Special "sval" limit -- first "good" magic/prayer book
		 */
		public const int SV_BOOK_MIN_GOOD = 4;

		/*
		 * Special "sval" value -- unknown "sval"
		 */
		public const int SV_UNKNOWN = 255;

		/**
		 * Return the numeric sval of the object kind with the given `tval` and name `name`.
		 */
		public static int lookup_sval(int tval, string name)
		{
			uint r;
			if (uint.TryParse(name, out r))
				return (int)r;

			/* Look for it */
			for (int k = 1; k < Misc.z_info.k_max; k++)
			{
				Object_Kind k_ptr = Misc.k_info[k];
				//Skip empty entries
				if(k_ptr == null)
					continue;
				string nm = k_ptr.Name;

				if (name == null) continue;

				//Basically, if name starts with &, skip the first two chars
				if (nm[0] == '&' && name.Length >= 2)
					nm = nm.Substring(2);

				//ORIGINAL
				//if (*nm == '&' && *(nm+1))
				//    nm += 2;

				/* Found a match */
				if (k_ptr.tval == tval && name == nm)
					return k_ptr.sval;
			}

			return -1;
		}
	}
}
