using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Option {
		static int Counter = 0;
		public static Option[] options = new Option[MAX];

		public Option(string name, string description, bool normal){
			this.name = name;
			this.description = description;
			this.normal = normal;

			this.index = Counter++;
			options[index] = this;
		}
		public string name;
		public string description;
		public bool normal;
		public bool value {
			get {
				return Player.Player_Other.instance.opt[index];
			}
			set {
				Player.Player_Other.instance.opt[index] = value;
			}
		}
		
		public int index;
		
		/*
		 * Option indexes (hard-coded by savefiles)
		 */

		
		public static Option rogue_like_commands = new Option (  "rogue_like_commands", "Use the roguelike command keyset",            false  );  /* 0 */
		public static Option use_sound = new Option (  "use_sound",           "Use sound",                                   false  );  /* 1 */
		public static Option XXX_2 = new Option (  null,                  null,                                          false  );  /* 2 */
		public static Option use_old_target = new Option (  "use_old_target",      "Use old target by default",                   false  );  /* 3 */
		public static Option pickup_always = new Option (  "pickup_always",       "Always pickup items",                         false  );  /* 4 */
		public static Option pickup_inven = new Option (  "pickup_inven",        "Always pickup items matching inventory",      true  );   /* 5 */
		public static Option show_flavors = new Option (  "show_flavors",        "Show flavors in object descriptions",         false  );   /* 6 */
		public static Option disturb_move = new Option (  "disturb_move",        "Disturb whenever any monster moves",          false  );  /* 7 */
		public static Option disturb_near = new Option (  "disturb_near",        "Disturb whenever viewable monster moves",     true  );   /* 8 */
		public static Option disturb_detect = new Option (  "disturb_detect",      "Disturb whenever leaving trap detected area", true  );   /* 9 */
		public static Option disturb_state = new Option (  "disturb_state",       "Disturb whenever player state changes",       true  );   /* 10 */
		public static Option XXX_11 = new Option (  null,                  null,                                          false  );  /* 11 */
		public static Option XXX_12 = new Option (  null,                  null,                                          false  );  /* 12 */
		public static Option view_yellow_light = new Option (  "view_yellow_light",   "Color: Illuminate torchlight in yellow",      false  );  /* 13 */
		public static Option easy_open = new Option (  "easy_open",           "Open/disarm/close without direction",         true  );  /* 14 */
		public static Option animate_flicker = new Option (  "animate_flicker",     "Color: Shimmer multi-colored things",         false  );  /* 15 */
		public static Option center_player = new Option (  "center_player",       "Center map continuously",                     false  );  /* 16 */
		public static Option purple_uniques = new Option (  "purple_uniques",      "Color: Show unique monsters in purple",       false  );  /* 17 */
		public static Option xchars_to_file = new Option (  "xchars_to_file",      "Allow accents in output files",               false  );  /* 18 */
		public static Option auto_more = new Option (  "auto_more",           "Automatically clear '-more-' prompts",        false  );  /* 19 */
		public static Option hp_changes_color = new Option (  "hp_changes_color",    "Color: Player color indicates % hit points",  false  );  /* 20 */
		public static Option mouse_movement = new Option (  "mouse_movement",      "Allow mouse clicks to move the player",       false  );  /* 21 */
		public static Option mouse_buttons = new Option (  "mouse_buttons",       "Show mouse status line buttons",              false  );  /* 22 */
		public static Option notify_recharge = new Option (  "notify_recharge",     "Notify on object recharge",                   false  );  /* 23 */
		public static Option XXX_24 = new Option (  null,                  null,                                          false  );  /* 24 */
		public static Option XXX_25 = new Option (  null,                  null,                                          false  );  /* 25 */
		public static Option XXX_26 = new Option (  null,                  null,                                          false  );  /* 26 */
		public static Option XXX_27 = new Option (  null,                  null,                                          false  );  /* 27 */
		public static Option XXX_28 = new Option (  null,                  null,                                          false  );  /* 28 */
		public static Option XXX_29 = new Option (  null,                  null,                                          false  );  /* 29 */
		public static Option XXX_30 = new Option (  null,                  null,                                          false  );  /* 30 */
		public static Option cheat_hear = new Option (  "cheat_hear",          "Cheat: Peek into monster creation",           false  );  /* 31 */
		public static Option cheat_room = new Option (  "cheat_room",          "Cheat: Peek into dungeon creation",           false  );  /* 32 */
		public static Option cheat_xtra = new Option (  "cheat_xtra",          "Cheat: Peek into something else",             false  );  /* 33 */
		public static Option cheat_know = new Option (  "cheat_know",          "Cheat: Know complete monster info",           false  );  /* 34 */
		public static Option cheat_live = new Option (  "cheat_live",          "Cheat: Allow player to avoid death",          false  );  /* 35 */
		public static Option XXX_36 = new Option (  null,                  null,                                          false  );  /* 36 */
		public static Option XXX_37 = new Option (  null,                  null,                                          false  );  /* 37 */
		public static Option XXX_38 = new Option (  null,                  null,                                          false  );  /* 38 */
		public static Option XXX_39 = new Option (  null,                  null,                                          false  );  /* 39 */
		public static Option XXX_40 = new Option (  null,                  null,                                          false  );  /* 40 */
		public static Option score_hear = new Option (  "score_hear",          "Score: Peek into monster creation",           false  );  /* 41 */
		public static Option score_room = new Option (  "score_room",          "Score: Peek into dungeon creation",           false  );  /* 42 */
		public static Option score_xtra = new Option (  "score_xtra",          "Score: Peek into something else",             false  );  /* 43 */
		public static Option score_know = new Option (  "score_know",          "Score: Know complete monster info",           false  );  /* 44 */
		public static Option score_live = new Option (  "score_live",          "Score: Allow player to avoid death",          false  );  /* 45 */
		public static Option XXX_46 = new Option (  null,                  null,                                          false  );  /* 46 */
		public static Option XXX_47 = new Option (  null,                  null,                                          false  );  /* 47 */
		public static Option XXX_48 = new Option (  null,                  null,                                          false  );  /* 48 */
		public static Option XXX_49 = new Option (  null,                  null,                                          false  );  /* 49 */
		public static Option birth_maximize = new Option (  "birth_maximize",      "Maximise effect of race/class bonuses",       true  );   /* 50 */
		public static Option birth_randarts = new Option (  "birth_randarts",      "Randomise the artifacts (except a very few)", false  );  /* 51 */
		public static Option birth_ironman = new Option (  "birth_ironman",       "Restrict the use of stairs/recall",           false  );  /* 52 */
		public static Option birth_no_stores = new Option (  "birth_no_stores",     "Restrict the use of stores/home",             false  );  /* 53 */
		public static Option birth_no_artifacts = new Option (  "birth_no_artifacts",  "Restrict creation of artifacts",              false  );  /* 54 */
		public static Option birth_no_stacking = new Option (  "birth_no_stacking",   "Don't stack objects on the floor",            false  );  /* 55 */
		public static Option birth_no_preserve = new Option (  "birth_no_preserve",   "Lose artifacts when leaving level",           false  );  /* 56 */
		public static Option birth_no_stairs = new Option (  "birth_no_stairs",     "Don't generate connected stairs",             false  );  /* 57 */
		public static Option birth_no_feelings = new Option (  "birth_no_feelings",   "Don't show level feelings",                   false  );  /* 58 */
		public static Option birth_no_selling = new Option (  "birth_no_selling",    "Items always sell for 0 gold",                false  );  /* 59 */
		public static Option birth_keep_randarts = new Option (  "birth_keep_randarts", "Use previous set of randarts",                true  );   /* 60 */
		public static Option birth_ai_smell = new Option (  "birth_ai_smell",      "Monsters chase recent locations",             true  );   /* 61 */
		public static Option birth_ai_packs = new Option (  "birth_ai_packs",      "Monsters act smarter in groups",              true  );   /* 62 */
		public static Option birth_ai_learn = new Option (  "birth_ai_learn",      "Monsters learn from their mistakes",          false  );  /* 63 */
		public static Option birth_ai_cheat = new Option (  "birth_ai_cheat",      "Monsters exploit player's weaknesses",        false  );  /* 64 */
		public static Option birth_ai_smart = new Option (  "birth_ai_smart",      "Monsters behave more intelligently (broken)", false  );  /* 65 */
		public static Option XXX_66 = new Option (  null,                  null,                                          false  );  /* 66 */
		public static Option XXX_67 = new Option (  null,                  null,                                          false  );  /* 67 */
		public static Option XXX_68 = new Option (  null,                  null,                                          false  );  /* 68 */
		public static Option XXX_69 = new Option (  null,                  null,                                          false  );  /* 69 */
		/*
		 * Option screen interface
		 */
		static Option[][] option_page = new Option[PAGE_MAX][]
		{
			/* Interface */
			new Option[PAGE_PER]{
				rogue_like_commands,
				use_old_target,
				pickup_always,
				pickup_inven,
				easy_open,
				center_player,
				view_yellow_light,
				hp_changes_color,
				animate_flicker,
				purple_uniques,
				show_flavors,
				mouse_movement,
				mouse_buttons,
				use_sound,
				xchars_to_file,
				null,
			},

			/* Warning */
			new Option[PAGE_PER]{
				disturb_move,
				disturb_near,
				disturb_detect,
				disturb_state,
				auto_more,
				notify_recharge,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
			},

			/* Birth/Difficulty */
			new Option[PAGE_PER]{
				birth_maximize,
				birth_randarts,
				birth_keep_randarts,
				birth_ai_smell,
				birth_ai_packs,
				birth_ai_learn,
				birth_ai_cheat,
				birth_ai_smart,
				birth_ironman,
				birth_no_stores,
				birth_no_artifacts,
				birth_no_stacking,
				birth_no_preserve,
				birth_no_stairs,
				birth_no_feelings,
				birth_no_selling,
			},

			/* Cheat */
			new Option[PAGE_PER]{
				cheat_hear,
				cheat_room,
				cheat_xtra,
				cheat_know,
				cheat_live,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
			}
		};


		/*** Functions ***/

		/** Given an option index, return its name */
		public static  string option_name(int opt)
		{
			throw new NotImplementedException();
			//if (opt >= MAX) return null;
			//return options[opt].name;
		}

		/** Given an option index, return its description */
		public static  string option_desc(int opt)
		{
			throw new NotImplementedException();
			//if (opt >= MAX) return null;
			//return options[opt].description;
		}

		/** Set an an option, return true if successful */
		/* Setup functions */
		public static  bool option_set(string name, bool on)
		{
			throw new NotImplementedException();
			//size_t opt;
			//for (opt = 0; opt < MAX; opt++)
			//{
			//    if (!options[opt].name || !streq(options[opt].name, name))
			//        continue;

			//    op_ptr.opt[opt] = on;
			//    if (on && option_is_cheat(opt)) {
			//        op_ptr.opt[opt + (SCORE - CHEAT)] = true;
			//    }

			//    return true;
			//}

			//return false;
		}

		/** Reset options to defaults */
		public static void set_defaults()
		{
			//size_t opt;
			//for (opt = 0; opt < MAX; opt++)
			//    op_ptr.opt[opt] = options[opt].normal;
		}

		//#if 0 /* unused so far but may be useful in future */
		//static bool option_is_birth(int opt) { return opt >= BIRTH && opt < (BIRTH + N_OPTS_BIRTH); }
		//static bool option_is_score(int opt) { return opt >= SCORE && opt < (SCORE + N_OPTS_CHEAT); }
		//#endif
		static bool option_is_cheat(int opt) { return opt >= CHEAT && opt < (CHEAT + N_OPTS_CHEAT); }

		/*** Option display definitions ***/

		/*
		 * Information for "do_cmd_options()".
		 */
		public const int PAGE_MAX		=		4;
		public const int PAGE_PER	=		16;

		public const int PAGE_BIRTH	=		2;

		/* The option data structures */
		//extern const int option_page[PAGE_MAX][PAGE_PER];

		/*** Option definitions ***/

		/*
		 * Option indexes (offsets)
		 *
		 * These values are hard-coded by savefiles (and various pieces of code).  Ick.
		 */
		public const int CHEAT		=			30;
		public const int SCORE		=			40;
		public const int BIRTH		=			50;

		//#define NONE					89
		public const int MAX =	90;

		public const int N_OPTS_CHEAT	=			6;
		public const int N_OPTS_BIRTH = 16;


		//#define OPT(opt_name)	op_ptr.opt[##opt_name]
	}
}
