using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	static class Misc {
		public static Monster.Monster_Race[] r_info;
		public static Maxima z_info = new Maxima();
		public static Feature[] f_info;
		public static Object.Object_Base[] kb_info;
		public static Object.Object_Kind[] k_info;
		public static Object.Artifact[] a_info;
		public static Object.Ego_Item[] e_info;
		public static Monster.Monster_Base rb_info;
		public static Monster.Monster_Pain[] pain_messages;
		public static Player.Player_Race races;
		public static Player.Player_Class classes;
		public static Object.Flavor flavors;
		public static Vault vaults;
		public static Object.Object_Kind objkinds;
		public static Spell[] s_info;
		public static Hint hints;
		public static Pit_Profile[] pit_info;
		public static Player.History_Chart histories;

		/*
		 * Hack -- Maximum number of quests
		 */
		public const int MAX_Q_IDX = 4;
		public static Quest[] q_list;

		public static string[] f_info_flags = new string[]
		{
			"PWALK",
			"PPASS",
			"MWALK",
			"MPASS",
			"LOOK",
			"DIG",
			"DOOR",
			"EXIT_UP",
			"EXIT_DOWN",
			"PERM",
			"TRAP",
			"SHOP",
			"HIDDEN",
			"BORING"
		};

		public static Monster.Monster_Lore[] l_list;

		public static Store[] stores;

		/*
		 * Maximum number of player "sex" types (see "table.c", etc)
		 */
		public const int MAX_SEXES = 3;
		/*
		 * Player Sexes
		 *
		 *	Title,
		 *	Winner
		 */
		public static Player.Player_Sex[] sex_info = new Player.Player_Sex[MAX_SEXES]
		{
			new Player.Player_Sex("Female",	"Queen"),
			new Player.Player_Sex("Male",	"King"),
			new Player.Player_Sex("Neuter",	"Regent")
		};


		/*
		 * Player sex constants (hard-coded by save-files, arrays, etc)
		 */
		public static int SEX_FEMALE = 0;
		public static int SEX_MALE	=	1;
		

		/*
		 * Maximum number of players spells
		 */
		public const int PY_MAX_SPELLS = 64;

		/*
		 * Player constants
		 */
		public const long PY_MAX_EXP =	99999999L;	/* Maximum exp */
		public const long PY_MAX_GOLD =	999999999L; /* Maximum gold */
		public const int	 PY_MAX_LEVEL =	50;			/* Maximum level */

		/**
		 * Maximum number of pvals on objects
		 *
		 * Note: all pvals other than DEFAULT_PVAL are assumed to be associated with
		 * flags, and any non-flag uses of pval (e.g. chest quality, gold quantity)
		 * are assumed to use DEFAULT_PVAL.
		 */
		public const int MAX_PVALS = 3;
		public const int DEFAULT_PVAL = 0;
		
		/*
		 * An item's pval (for charges, amount of gold, etc) is limited to s16b
		 */
		public const int MAX_PVAL = 32767;

		/*
		 * Bit flags for the "p_ptr.update" variable
		 */
		public const uint PU_BONUS =		0x00000001;	/* Calculate bonuses */
		public const uint PU_TORCH =		0x00000002;	/* Calculate torch radius */
		/* xxx (many) */
		public const uint PU_HP =			0x00000010;	/* Calculate chp and mhp */
		public const uint PU_MANA =			0x00000020;	/* Calculate csp and msp */
		public const uint PU_SPELLS =		0x00000040;	/* Calculate spells */
		/* xxx (many) */
		public const uint PU_FORGET_VIEW =	0x00010000;	/* Forget field of view */
		public const uint PU_UPDATE_VIEW =	0x00020000;	/* Update field of view */
		/* xxx (many) */
		public const uint PU_FORGET_FLOW =	0x00100000;	/* Forget flow data */
		public const uint PU_UPDATE_FLOW =	0x00200000;	/* Update flow data */
		/* xxx (many) */
		public const uint PU_MONSTERS =		0x10000000;	/* Update monsters */
		public const uint PU_DISTANCE =		0x20000000;	/* Update distances */
		/* xxx */
		public const uint PU_PANEL = 0x80000000;	/* Update panel */


		/*
		 * Player "food" crucial values
		 */
		public const int PY_FOOD_UPPER = 20000;  /* Upper limit on food counter */
		public const int PY_FOOD_MAX	= 15000;	/* Food value (Bloated) */
		public const int PY_FOOD_FULL =	10000;	/* Food value (Normal) */
		public const int PY_FOOD_ALERT =	2000;	/* Food value (Hungry) */
		public const int PY_FOOD_WEAK =	1000;	/* Food value (Weak) */
		public const int PY_FOOD_FAINT =	500;	/* Food value (Fainting) */
		public const int PY_FOOD_STARVE =	100;	/* Food value (Starving) */

		/*
		 * Player regeneration constants
		 */
		public const int PY_REGEN_NORMAL=		197;	/* Regen factor*2^16 when full */
		public const int PY_REGEN_WEAK=		98;	/* Regen factor*2^16 when weak */
		public const int PY_REGEN_FAINT=		33;	/* Regen factor*2^16 when fainting */
		public const int PY_REGEN_HPBASE=		1442;	/* Min amount hp regen*2^16 */
		public const int PY_REGEN_MNBASE	=	524;	/* Min amount mana regen*2^16 */

		/*
		 * Flags for player_type.spell_flags[]
		 */
		public const int PY_SPELL_LEARNED=    0x01; /* Spell has been learned */
		public const int PY_SPELL_WORKED  =   0x02; /* Spell has been successfully tried */
		public const int PY_SPELL_FORGOTTEN = 0x04; /* Spell has been forgotten */


		public class PF
		{
			public static List<PF> list = new List<PF>();

			public string name;
			public string text;
			public int value;

			public PF(string name, int value, string text){
				this.name = name;
				this.text = text;
				this.value = value;

				list.Add(this);
			}
			public static PF NONE = new PF("NONE", 0,             "");
			public static PF EXTRA_SHOT = new PF("EXTRA_SHOT", 1,       "receive extra shots with tension bows at levels 20 and 40");
			public static PF BRAVERY_30 = new PF("BRAVERY_30", 2,       "become immune to fear at level 30");
			public static PF BLESS_WEAPON = new PF("BLESS_WEAPON", 3,     "may only wield blessed or hafted weapons");
			public static PF CUMBER_GLOVE = new PF("CUMBER_GLOVE", 4,     "have difficulty using magic with covered hands");
			public static PF ZERO_FAIL = new PF("ZERO_FAIL", 5,        "may obtain a perfect success rate with magic");
			public static PF BEAM = new PF("BEAM", 6,             "frequently turn bolt spells into beams");
			public static PF CHOOSE_SPELLS = new PF("CHOOSE_SPELLS", 7,    "may choose their own spells to study");
			public static PF PSEUDO_ID_IMPROV = new PF("PSEUDO_ID_IMPROV", 8, "get better at psudo id with experience");
			public static PF KNOW_MUSHROOM = new PF("KNOW_MUSHROOM", 9,    "easily recognize mushrooms");
			public static PF KNOW_ZAPPER = new PF("KNOW_ZAPPER", 10,      "easily recognize magic devices");
			public static PF SEE_ORE = new PF("SEE_ORE", 11,          "can sense ore in the walls");
			public static PF MAX = new PF("MAX", 12, "ERROR: PF.MAX SHOULD NEVER BE SHOWN TO THE PLAYER");
		};

		public static int PF_SIZE{
			get {
				return Bitflag.FLAG_SIZE(PF.MAX.value);
			}
		}

		/*
		 * Maximum number of "normal" pack slots, and the index of the "overflow"
		 * slot, which can hold an item, but only temporarily, since it causes the
		 * pack to "overflow", dropping the "last" item onto the ground.  Since this
		 * value is used as an actual slot, it must be less than "INVEN_WIELD" (below).
		 * Note that "INVEN_PACK" is probably hard-coded by its use in savefiles, and
		 * by the fact that the screen can only show 23 items plus a one-line prompt.
		 */
		public const int INVEN_PACK     =   23;

		/*
		 * Like the previous but takes into account the (variably full quiver).
		 */
		public static int INVEN_MAX_PACK{
			get {
				return (INVEN_PACK - Player.Player.instance.quiver_slots);
			}
		}

		/*
		 * Indexes used for various "equipment" slots (hard-coded by savefiles, etc).
		 */
		public const int INVEN_WIELD=	24;
		public const int INVEN_BOW   =    25;
		public const int INVEN_LEFT   =   26;
		public const int INVEN_RIGHT    = 27;
		public const int INVEN_NECK   =   28;
		public const int INVEN_LIGHT  =   29;
		public const int INVEN_BODY  =    30;
		public const int INVEN_OUTER  =   31;
		public const int INVEN_ARM    =   32;
		public const int INVEN_HEAD   =   33;
		public const int INVEN_HANDS  =   34;
		public const int INVEN_FEET   =   35;

		/*
		 * Total number of inventory slots (hard-coded).
		 */
		public const int INVEN_TOTAL	=36;


		/*
		 *Quiver
		 */
		public const int QUIVER_START =	37;
		public const int QUIVER_SIZE  =	10;
		public const int QUIVER_END  = 	47;

		public const int ALL_INVEN_TOTAL= 47;
		/* Since no item index can have this value, use it to mean
		 * "no object", so that 0 can be a valid index. */
		public const int NO_OBJECT	=	(ALL_INVEN_TOTAL+1);


		/*
		 * A "stack" of items is limited to less than 100 items (hard-coded).
		 */
		public const int MAX_STACK_SIZE =100;

		/*
		 * Maximum number of objects allowed in a single dungeon grid.
		 *
		 * The main-screen has a minimum size of 24 rows, so we can always
		 * display 23 objects + 1 header line.
		 */
		public const int MAX_FLOOR_STACK		=	23;

		/*
		 * Bit flags for the "p_ptr.notice" variable
		 */
		public const long PN_COMBINE    =  0x00000001L;    /* Combine the pack */
		public const long PN_REORDER    =  0x00000002L;    /* Reorder the pack */
		public const long PN_AUTOINSCRIBE =0x00000004L;    /* Autoinscribe items */
		public const long PN_PICKUP     =  0x00000008L;    /* Pick stuff up */
		public const long PN_SQUELCH     = 0x00000010L;    /* Squelch stuff */
		public const long PN_SORT_QUIVER = 0x00000020L;    /* Sort the quiver */
		public const long PN_MON_MESSAGE=	0x00000040L;	   /* flush monster pain messages */
		/* xxx (many) */


		/*
		 * Bit flags for the "p_ptr.redraw" variable
		 */
		public const uint PR_MISC		=	0x00000001;	/* Display Race/Class */
		public const uint PR_TITLE	=	0x00000002;	/* Display Title */
		public const uint PR_LEV		=	0x00000004;	/* Display Level */
		public const uint PR_EXP		=	0x00000008;	/* Display Experience */
		public const uint PR_STATS	=	0x00000010;	/* Display Stats */
		public const uint PR_ARMOR	=	0x00000020;	/* Display Armor */
		public const uint PR_HP		=	0x00000040;	/* Display Hitpoints */
		public const uint PR_MANA	=		0x00000080;	/* Display Mana */
		public const uint PR_GOLD	=		0x00000100;	/* Display Gold */

		public const uint PR_HEALTH	=	0x00000800;	/* Display Health Bar */
		public const uint PR_SPEED	=	0x00001000;	/* Display Extra (Speed) */
		public const uint PR_STUDY	=	0x00002000;	/* Display Extra (Study) */
		public const uint PR_DEPTH	=	0x00004000;	/* Display Depth */
		public const uint PR_STATUS	=	0x00008000;
		public const uint PR_DTRAP	=	0x00010000; /* Trap detection indicator */
		public const uint PR_STATE	=	0x00020000;	/* Display Extra (State) */
		public const uint PR_MAP		=	0x00040000;	/* Redraw whole map */

		public const uint PR_INVEN	=	0x00080000; /* Display inven/equip */
		public const uint PR_EQUIP	=	0x00100000; /* Display equip/inven */
		public const uint PR_MESSAGE	=	0x00200000; /* Display messages */
		public const uint PR_MONSTER	=	0x00400000; /* Display monster recall */
		public const uint PR_OBJECT	=	0x00800000; /* Display object recall */
		public const uint PR_MONLIST	=	0x01000000; /* Display monster list */
		public const uint PR_BUTTONS   =   0x02000000; /* Display mouse buttons */
		public const uint PR_ITEMLIST  =   0x04000000; /* Display item list */

		/* Display Basic Info */
		public const long PR_BASIC =
			(PR_MISC | PR_TITLE | PR_STATS | PR_LEV |
			 PR_EXP | PR_GOLD | PR_ARMOR | PR_HP |
			 PR_MANA | PR_DEPTH | PR_HEALTH | PR_SPEED);

		/* Display Extra Info */
		public const long PR_EXTRA = (PR_STATUS | PR_STATE | PR_STUDY);


		/*
		 * Bit flags for the "p_ptr.window" variable.
		 */
		public const long PW_INVEN      =      0x00000001L; /* Display inven/equip */
		public const long PW_EQUIP      =      0x00000002L; /* Display equip/inven */
		public const long PW_PLAYER_0   =      0x00000004L; /* Display player (basic) */
		public const long PW_PLAYER_1    =     0x00000008L; /* Display player (extra) */
		public const long PW_PLAYER_2   =      0x00000010L; /* Display player (compact) */
		public const long PW_MAP        =      0x00000020L; /* Display dungeon map */
		public const long PW_MESSAGE    =      0x00000040L; /* Display messages */
		public const long PW_OVERHEAD   =      0x00000080L; /* Display overhead view */
		public const long PW_MONSTER    =      0x00000100L; /* Display monster recall */
		public const long PW_OBJECT      =     0x00000200L; /* Display object recall */
		public const long PW_MONLIST    =      0x00000400L; /* Display monster list */
		public const long PW_STATUS    =       0x00000800L; /* Display status */
		public const long PW_ITEMLIST    =     0x00001000L; /* Display item list */
		/* xxx */
		public const long PW_BORG_1      =     0x00004000L; /* Display borg messages */
		public const long PW_BORG_2      =     0x00008000L; /* Display borg status */


		public const int PW_MAX_FLAGS	=	16;


		/*
		 * Number of tval/min-sval/max-sval slots per ego_item
		 */
		public const int EGO_TVALS_MAX = 3;

		/*
		 * Monster Timed Effects
		 */
		public enum MON_TMD
		{
			SLEEP = 0,
			STUN,
			CONF,
			FEAR,
			SLOW,
			FAST,

			MAX
		};

		//Angband Strings
		public static string ANGBAND_SYS;
		public static string ANGBAND_GRAF;

		public static string ANGBAND_DIR_APEX;
		public static string ANGBAND_DIR_EDIT;
		public static string ANGBAND_DIR_FILE;
		public static string ANGBAND_DIR_HELP;
		public static string ANGBAND_DIR_INFO;
		public static string ANGBAND_DIR_SAVE;
		public static string ANGBAND_DIR_PREF;
		public static string ANGBAND_DIR_USER;
		public static string ANGBAND_DIR_XTRA;

		public static string ANGBAND_DIR_XTRA_FONT;
		public static string ANGBAND_DIR_XTRA_GRAF;
		public static string ANGBAND_DIR_XTRA_SOUND;
		public static string ANGBAND_DIR_XTRA_ICON;

		public static void assert(bool value){
			if (value == false){
				
				throw new Exception("Assert failed");
			}
		}
		public static void assert(bool value, string Message){
			if (value == false){
				throw new Exception(Message);
			}
		}

		public delegate void sound_hook_func(int i);
		public static sound_hook_func sound_hook;

		/*
		 * Maximum amount of Angband windows.
		 */
		public static int ANGBAND_TERM_MAX = 8;

		/*
		 * The array[ANGBAND_TERM_MAX] of window pointers
		 */
		public static Term[] angband_term = new Term[ANGBAND_TERM_MAX];

		/*
		 * More maximum values
		 */
		public static int  MAX_SIGHT=	20;	/* Maximum view distance */
		public static int MAX_RANGE = 20;	/* Maximum range (spells, etc) */

		//THIS WAS IN z-file.c
		//....we need to find a place to put this
		//TODO: find a place to put this
		/*
		 * Create a new path string by appending a 'leaf' to 'base'.
		 *
		 * On Unixes, we convert a tidle at the beginning of a basename to mean the
		 * directory, complicating things a little, but better now than later.
		 *
		 * Remember to free the return value.
		 */
		//NOTE: I modified this function to basically return Base + / + leaf
		public static string path_build(string Base, string leaf)
		{
			if(leaf[0] == '/' || Base.Last() == '/') {
				return Base + leaf;
			} else {
				return Base + "/" + leaf;
			}
		//    int cur_len = 0;
		//    buf[0] = '\0';

		//    if (!leaf || !leaf[0])
		//    {
		//        if (base && base[0])
		//            path_process(buf, len, &cur_len, base);

		//        return cur_len;
		//    }


		//    /*
		//     * If the leafname starts with the seperator,
		//     *   or with the tilde (on Unix),
		//     *   or there's no base path,
		//     * We use the leafname only.
		//     */
		//#if defined(SET_UID) || defined(USE_PRIVATE_PATHS)
		//    if ((!base || !base[0]) || prefix(leaf, PATH_SEP) || leaf[0] == '~')
		//#else
		//    if ((!base || !base[0]) || prefix(leaf, PATH_SEP))
		//#endif
		//    {
		//        path_process(buf, len, &cur_len, leaf);
		//        return cur_len;
		//    }


		//    /* There is both a relative leafname and a base path from which it is relative */
		//    path_process(buf, len, &cur_len, base);
		//    strnfcat(buf, len, &cur_len, "%s", PATH_SEP);
		//    path_process(buf, len, &cur_len, leaf);

		//    return cur_len;
		}

		public static short character_icky;		/* Depth of the game in special mode */
		public static short character_xtra;		/* Depth of the game in startup mode */

		public static Term term_screen {
			get {
				return angband_term[0];
			}
		}

		public static uint SCAN_INSTANT = uint.MaxValue;
		public static uint SCAN_OFF = 0;
		public static uint SCAN_MACRO = 45;

		/*
		 * Maximum dungeon level.  The player can never reach this level
		 * in the dungeon, and this value is used for various calculations
		 * involving object and monster creation.  It must be at least 100.
		 * Setting it below 128 may prevent the creation of some objects.
		 */
		public static int MAX_DEPTH = 128;

		/*
		 * Run-time arguments
		 */
		public static bool arg_wizard;			/* Command arg -- Request wizard mode */
		public static int arg_graphics;			/* Command arg -- Request graphics mode */
		public static bool arg_graphics_nice;			/* Command arg -- Request nice graphics mode */
		public static bool arg_rebalance;			/* Command arg -- Rebalance monsters */

		
		/*
		 * This table allows quick conversion from "speed" to "energy"
		 * The basic function WAS ((S>=110) ? (S-110) : (100 / (120-S)))
		 * Note that table access is *much* quicker than computation.
		 *
		 * Note that the table has been changed at high speeds.  From
		 * "Slow (-40)" to "Fast (+30)" is pretty much unchanged, but
		 * at speeds above "Fast (+30)", one approaches an asymptotic
		 * effective limit of 50 energy per turn.  This means that it
		 * is relatively easy to reach "Fast (+30)" and get about 40
		 * energy per turn, but then speed becomes very "expensive",
		 * and you must get all the way to "Fast (+50)" to reach the
		 * point of getting 45 energy per turn.  After that point,
		 * furthur increases in speed are more or less pointless,
		 * except to balance out heavy inventory.
		 *
		 * Note that currently the fastest monster is "Fast (+30)".
		 *
		 * It should be possible to lower the energy threshhold from
		 * 100 units to 50 units, though this may interact badly with
		 * the (compiled out) small random energy boost code.  It may
		 * also tend to cause more "clumping" at high speeds.
		 */
		public static byte[] extract_energy = new byte[200]
		{
			/* Slow */     1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
			/* Slow */     1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
			/* Slow */     1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
			/* Slow */     1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
			/* Slow */     1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
			/* Slow */     1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
			/* S-50 */     1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
			/* S-40 */     2,  2,  2,  2,  2,  2,  2,  2,  2,  2,
			/* S-30 */     2,  2,  2,  2,  2,  2,  2,  3,  3,  3,
			/* S-20 */     3,  3,  3,  3,  3,  4,  4,  4,  4,  4,
			/* S-10 */     5,  5,  5,  5,  6,  6,  7,  7,  8,  9,
			/* Norm */    10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
			/* F+10 */    20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
			/* F+20 */    30, 31, 32, 33, 34, 35, 36, 36, 37, 37,
			/* F+30 */    38, 38, 39, 39, 40, 40, 40, 41, 41, 41,
			/* F+40 */    42, 42, 42, 43, 43, 43, 44, 44, 44, 44,
			/* F+50 */    45, 45, 45, 45, 45, 46, 46, 46, 46, 46,
			/* F+60 */    47, 47, 47, 47, 47, 48, 48, 48, 48, 48,
			/* F+70 */    49, 49, 49, 49, 49, 49, 49, 49, 49, 49,
			/* Fast */    49, 49, 49, 49, 49, 49, 49, 49, 49, 49,
		};

		/**
		 * Maximum number of rvals (monster templates) that a pit can specify.
		 */
		public const int MAX_RVALS = 4;


		/*
		 * Maximum size of the "view" array (see "cave.c")
		 * Note that the "view radius" will NEVER exceed 20, and even if the "view"
		 * was octagonal, we would never require more than 1520 entries in the array.
		 */
		public const int VIEW_MAX = 1536;

		/*
		 * Maximum size of the "temp" array (see "cave.c")
		 * Note that we must be as large as "VIEW_MAX" for proper functioning
		 * of the "update_view()" function, and we must also be as large as the
		 * largest illuminatable room, but no room is larger than 800 grids.  We
		 * must also be large enough to allow "good enough" use as a circular queue,
		 * to calculate monster flow, but note that the flow code is "paranoid".
		 */
		public const int TEMP_MAX = 1536;
		/*
		 * Arrays[TEMP_MAX] used for various things
		 */
		public static ushort[] temp_g;

		/* Maxinum number of stacked monster messages */
		public const int MAX_STORED_MON_MSG		= 200;
		public const int MAX_STORED_MON_CODES	= 400;

		/*
		 * Called with an array of the new flags for all the subwindows, in order
		 * to set them to the new values, with a chance to perform housekeeping.
		 */
		public static void subwindows_set_flags(uint[] new_flags, int n_subwindows)
		{
			for (int j = 0; j < n_subwindows; j++)
			{
				/* Dead window */
				if (Misc.angband_term[j] == null) continue;

				/* Ignore non-changes */
				if (Player.Player_Other.instance.window_flag[j] != new_flags[j])
				{
					throw new NotImplementedException(); //because fuck if I know what this means...
					//subwindow_set_flags(j, new_flags[j]);
				}
			}
		}

		/* player_type.noscore flags */
		public const int NOSCORE_WIZARD =		0x0002;
		public const int NOSCORE_DEBUG	=	0x0008;
		public const int NOSCORE_BORG = 0x0010;

		public static int seed_randart;		/* Hack -- consistent random artifacts */
		public static int seed_flavor;		/* Hack -- consistent object colors */
		public static int seed_town;			/* Hack -- consistent town layout */

		/*
		 * Hack -- function hook to output (colored) text to the
		 * screen or to a file.
		 */
		public delegate void text_out_hook_func(ConsoleColor a, string str);
		public static text_out_hook_func text_out_hook;

		/*
		 * Hack -- Where to wrap the text when using text_out().  Use the default
		 * value (for example the screen width) when 'text_out_wrap' is 0.
		 */
		public static int text_out_wrap = 0;


		/*
		 * Hack -- Indentation for the text when using text_out().
		 */
		public static int text_out_indent = 0;

		/*
		 * Hack -- Padding after wrapping
		 */
		public static int text_out_pad = 0;

		/*
		 * The range of possible indexes into tables based upon stats.
		 * Currently things range from 3 to 18/220 = 40.
		 */
		public const int STAT_RANGE = 38;

		public static int turn = 0;

		public const int MAX_FMT = 2;

		public const int MAX_PANEL = 12;

		/*
		 * Global arrays for converting "keypad direction" into "offsets".
		 */
		public static short[] ddx = new short[10]{ 0, -1, 0, 1, -1, 0, 1, -1, 0, 1 };

		public static short[] ddy = new short[10]{ 0, 1, 1, 1, 0, 0, 0, -1, -1, -1 };

		/*
		 * Refueling constants
		 */
		public const int FUEL_TORCH       =         5000;  /* Maximum amount of fuel in a torch */
		public const int FUEL_LAMP       =         15000;  /* Maximum amount of fuel in a lantern */
		public const int DEFAULT_TORCH  =     FUEL_TORCH;  /* Default amount of fuel in a torch */
		public const int DEFAULT_LAMP  = (FUEL_LAMP / 2);  /* Default amount of fuel in a lantern */

		//Fuck it, everyone kept wanting this.
		//Eventually this should be removed/moved
		public static Player.Player p_ptr {
			get {
				return Player.Player.instance;
			}
		}

		public static string[] inscrip_text =
		{
			null,
			"strange",
			"average",
			"magical",
			"splendid",
			"excellent",
			"special",
			"unknown"
		};

		/*
		 * Specify color for inventory item text display (by tval)
		 * Be sure to use "index & 0x7F" to avoid illegal access
		 */
		public static ConsoleColor[] tval_to_attr = new ConsoleColor[128];

		
		public static int use_graphics = 0;		/* The "graphics" mode is enabled */

		public static short o_max = 1;			/* Number of allocated objects */

		
		public static short o_cnt = 0;			/* Number of live objects */

		public static short num_repro;			/* Current reproducer count */

		/*
		 * Global arrays for optimizing "ddx[ddd[i]]" and "ddy[ddd[i]]".
		 */
		public static short[] ddx_ddd= new short[9]
		{ 0, 0, 1, -1, 1, -1, 1, -1, 0 };

		public static short[] ddy_ddd= new short[9]
		{ 1, -1, 0, 0, 1, 1, -1, -1, 0 };

		/*
		 * Global array for looping through the "keypad directions".
		 */
		public static short[] ddd = new short[9]
		{ 2, 8, 6, 4, 3, 1, 9, 7, 5 };

		/**
		 * Number of text rows in each map screen, regardless of tile size
		 */
		public static int SCREEN_ROWS{
			get {
				return (Term.instance.hgt - ROW_MAP - 1);
			}
		}

		public const int ROW_MAP	=		1;
		public const int COL_MAP		=	13;

		/**
		 * Number of grids in each screen (vertically)
		 */
		public static int SCREEN_HGT {
			get {
				return ((int)(SCREEN_ROWS / Term.tile_height));
			}
		}

		/**
		 * Number of grids in each screen (horizontally)
		 */
		public static int SCREEN_WID {
			get {
				return ((int)((Term.instance.wid - COL_MAP - 1) / Term.tile_width));
			}
		}

		/* The fixed amount of energy a player should have at the start of a new level */
		public const int INITIAL_DUNGEON_ENERGY = 100;

		/*
		 * Maximum flow depth when using "MONSTER_FLOW"
		 */
		public const int MONSTER_FLOW_DEPTH = 32;

		/** Macros **/

		/* Flags for the monster timed functions */
		public const int MON_TMD_FLG_NOTIFY		=0x01; /* Give notification */
		public const int MON_TMD_MON_SOURCE		=0x02; /* Monster is causing the damage */
		public const int MON_TMD_FLG_NOMESSAGE	=0x04; /* Never show a message */
		public const int MON_TMD_FLG_NOFAIL = 0x08; /* Never fail */

		/*
		 * Available graphic modes
		 */
		public const int GRAPHICS_NONE          = 0;
		public const int GRAPHICS_ORIGINAL      = 1;
		public const int GRAPHICS_ADAM_BOLT     = 2;
		public const int GRAPHICS_DAVID_GERVAIS = 3;
		public const int GRAPHICS_PSEUDO        = 4;
		public const int GRAPHICS_NOMAD         = 5;

		public const int MAX_ITEMLIST = 2560;
	
		/* 
		 * Special values for the number of turns to rest, these need to be
		 * negative numbers, as postive numbers are taken to be a turncount,
		 * and zero means "not resting". 
		 */
		public enum REST
		{
			COMPLETE = -2,
			ALL_POINTS = -1,
			SOME_POINTS = -3
		};

		/*
		 * Bit flags for the "get_item" function
		 */
		public const int USE_EQUIP   =  0x01;	/* Allow equip items */
		public const int USE_INVEN   =  0x02;	/* Allow inven items */
		public const int USE_FLOOR   =  0x04;	/* Allow floor items */
		public const int CAN_SQUELCH =  0x08;	/* Allow selection of all squelched items */
		public const int IS_HARMLESS =  0x10;	/* Ignore generic warning inscriptions */
		public const int SHOW_PRICES =  0x20;	/* Show item prices in item lists */
		public const int SHOW_FAIL   =  0x40; 	/* Show device failure in item lists */
		public const int QUIVER_TAGS =  0x80;	/* 0-9 are quiver slots when selecting */

		/*
		 * Convert a "key event" into a "location" (Y)
		 */
		public static int KEY_GRID_Y(ui_event K){
			return ((int) (((K.mouse.y - ROW_MAP) / Term.tile_height) + Term.instance.offset_y));
		}

		/*
		 * Convert a "key event" into a "location" (X)
		 */
		public static int KEY_GRID_X(ui_event K){
			return ((int)(((K.mouse.x - COL_MAP) / Term.tile_width) + Term.instance.offset_x));
		}

		/* Delay in centiseconds before moving to allow another keypress */
		/* Zero means normal instant movement. */
		public static ushort lazymove_delay = 0;

		/* Number of days passed on the current dungeon trip -
		  - used for determining store updates on return to town */
		public static ushort daycount = 0;

		/*
		 * There is a 1/160 chance per round of creating a new monster
		 */
		public const int MAX_M_ALLOC_CHANCE = 160;

		/*
		 * A monster can only "multiply" (reproduce) if there are fewer than 100
		 * monsters on the level capable of such spontaneous reproduction.  This
		 * is a hack which prevents the "mon_list[]" array from exploding due to
		 * reproducing monsters.  Messy, but necessary.
		 */
		public const int MAX_REPRO = 100;


		public const int BREAK_GLYPH = 550;	/* Rune of protection resistance */

		public const int BTH_PLUS_ADJ    =	3; 	/* Adjust BTH per plus-to-hit */
		public const int MON_MULT_ADJ	=	8; 	/* High value slows multiplication */
		public const int MON_DRAIN_LIFE = 2;	/* Percent of player exp drained per hit */

		/*
		 * Flag to override which store is selected if in a knowledge menu
		 */
		public static STORE store_knowledge = STORE.NONE;

		/*
		 * Here is a "hook" used during calls to "get_item()" and
		 * "show_inven()" and "show_equip()", and the choice window routines.
		 */
		public delegate bool item_tester_hook_func(Object.Object o);
		public static item_tester_hook_func item_tester_hook;

		/*
		 * Total Hack -- allow all items to be listed (even empty ones)
		 * This is only used by "do_cmd_inven_e()" and is cleared there.
		 */
		public static bool item_tester_full;

		/*
		 * Here is a "pseudo-hook" used during calls to "get_item()" and
		 * "show_inven()" and "show_equip()", and the choice window routines.
		 */
		public static byte item_tester_tval;

		/*
		 * Normal levels get at least 14 monsters
		 */
		public const int MIN_M_ALLOC_LEVEL = 14;

		/*
		 * Hack -- first "normal" artifact in the artifact list.  All of
		 * the artifacts with indexes from 1 to 15 are "special" (lights,
		 * rings, amulets), and the ones from 16 to 127 are "normal".
		 */
		public const int ART_MIN_NORMAL = 16;

		/*
		 * Return "s" (or not) depending on whether n is singular.
		 */
		public static string PLURAL(bool n){
			return (n ? "" : "s");
		}
	}
}
