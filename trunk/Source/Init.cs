using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Player;
using CSAngband.Monster;
using CSAngband.Object;
using System.IO;

namespace CSAngband {
	partial class Init {
		/*
		 * Find the default paths to all of our important sub-directories.
		 *
		 * All of the sub-directories should, by default, be located inside
		 * the main directory, whose location is very system dependant and is 
		 * set by the ANGBAND_PATH environment variable, if it exists. (On multi-
		 * user systems such as Linux this is not the default - see config.h for
		 * more info.)
		 *
		 * This function takes a writable buffers, initially containing the
		 * "path" to the "config", "lib" and "data" directories, for example, 
		 * "/etc/angband/", "/usr/share/angband" and "/var/games/angband" -
		 * or a system dependant string, for example, ":lib:".  The buffer
		 * must be large enough to contain at least 32 more characters.
		 *
		 * Various command line options may allow some of the important
		 * directories to be changed to user-specified directories, most
		 * importantly, the "apex" and "user" and "save" directories,
		 * but this is done after this function, see "main.c".
		 *
		 * In general, the initial path should end in the appropriate "PATH_SEP"
		 * string.  All of the "sub-directory" paths (created below or supplied
		 * by the user) will NOT end in the "PATH_SEP" string, see the special
		 * "path_build()" function in "util.c" for more information.
		 *
		 * Hack -- first we free all the strings, since this is known
		 * to succeed even if the strings have not been allocated yet,
		 * as long as the variables start out as "null".  This allows
		 * this function to be called multiple times, for example, to
		 * try several base "path" values until a good one is found.
		 */
		public static void file_paths(string configpath, string libpath, string datapath)
		{
			/*#ifdef PRIVATE_USER_PATH
				char buf[1024];
			#endif /* PRIVATE_USER_PATH */

			/*** Free everything ***/
			/* Free the sub-paths */
			Misc.ANGBAND_DIR_APEX = "";
			Misc.ANGBAND_DIR_EDIT = "";
			Misc.ANGBAND_DIR_FILE = "";
			Misc.ANGBAND_DIR_HELP = "";
			Misc.ANGBAND_DIR_INFO = "";
			Misc.ANGBAND_DIR_SAVE = "";
			Misc.ANGBAND_DIR_PREF = "";
			Misc.ANGBAND_DIR_USER = "";
			Misc.ANGBAND_DIR_XTRA = "";

			Misc.ANGBAND_DIR_XTRA_FONT = "";
			Misc.ANGBAND_DIR_XTRA_GRAF = "";
			Misc.ANGBAND_DIR_XTRA_SOUND = "";
			Misc.ANGBAND_DIR_XTRA_ICON = "";

			/*** Prepare the paths ***/

			/* Build path names */
			Misc.ANGBAND_DIR_EDIT = configpath + "edit";
			Misc.ANGBAND_DIR_FILE = libpath + "file";
			Misc.ANGBAND_DIR_HELP = libpath + "help";
			Misc.ANGBAND_DIR_INFO = libpath + "info";
			Misc.ANGBAND_DIR_PREF = configpath + "pref";
			Misc.ANGBAND_DIR_XTRA = libpath + "xtra";

			/* Build xtra/ paths */
			Misc.ANGBAND_DIR_XTRA_FONT = Misc.ANGBAND_DIR_XTRA + Config.PATH_SEP + "font";
			Misc.ANGBAND_DIR_XTRA_GRAF = Misc.ANGBAND_DIR_XTRA + Config.PATH_SEP + "graf";
			Misc.ANGBAND_DIR_XTRA_SOUND = Misc.ANGBAND_DIR_XTRA + Config.PATH_SEP + "sound";
			Misc.ANGBAND_DIR_XTRA_ICON = Misc.ANGBAND_DIR_XTRA + Config.PATH_SEP + "icon";

		/*#ifdef PRIVATE_USER_PATH

			// Build the path to the user specific directory
			if (strncmp(ANGBAND_SYS, "test", 4) == 0)
				path_build(buf, sizeof(buf), PRIVATE_USER_PATH, "Test");
			else
				path_build(buf, sizeof(buf), PRIVATE_USER_PATH, VERSION_NAME);
			ANGBAND_DIR_USER = string_make(buf);

			path_build(buf, sizeof(buf), ANGBAND_DIR_USER, "scores");
			ANGBAND_DIR_APEX = string_make(buf);

			path_build(buf, sizeof(buf), ANGBAND_DIR_USER, "save");
			ANGBAND_DIR_SAVE = string_make(buf);

		#else*/

			/* Build pathnames */
			Misc.ANGBAND_DIR_USER = datapath + "user";
			Misc.ANGBAND_DIR_APEX = datapath + "apex";
			Misc.ANGBAND_DIR_SAVE = datapath + "save";

		//#endif /* PRIVATE_USER_PATH */
		}

		/*
		 * Initialise just the internal arrays.
		 * This should be callable by the test suite, without relying on input, or
		 * anything to do with a user or savefiles.
		 *
		 * Assumption: Paths are set up correctly before calling this function.
		 */
		public static void arrays()
		{
			/* Initialize size info */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing array sizes...");
			if (z_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize sizes");

			/* Initialize feature info */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (features)");
			if (f_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize features");

			/* Initialize object base info */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (object bases)");
			if (kb_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize object bases");

			/* Initialize object info */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (objects)");
			if (k_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize objects");

			/* Initialize ego-item info */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (ego-items)");
			if (e_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize ego-items");

			/* Initialize artifact info */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (artifacts)");
			if (a_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize artifacts");

			/* Initialize monster pain messages */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (pain messages)");
			if (mp_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize monster pain messages");

			/* Initialize monster-base info */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (monster bases)");
			if (rb_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize monster bases");
	
			/* Initialize monster info */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (monsters)");
			if (r_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize monsters");

			/* Initialize monster pits */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (monster pits)");
			if (pit_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize monster pits");

			/* Initialize feature info */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (vaults)");
			if (v_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize vaults");

			/* Initialize history info */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (histories)");
			if (h_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize histories");

			/* Initialize race info */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (races)");
			if (p_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize races");

			/* Initialize class info */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (classes)");
			if (c_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize classes");

			/* Initialize flavor info */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (flavors)");
			if (flavor_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize flavors");

			/* Initialize spell info */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (spells)");
			if (s_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize spells");

			/* Initialize hint text */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (hints)");
			if (hints_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Cannot initialize hints");

			/* Initialise store stocking data */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (store stocks)");
			Store.Init();

			/* Initialise random name data */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (random names)");
			if (names_parser.run_parser() != Parser.Error.NONE) Utilities.quit("Can't parse names");

			/* Initialize some other arrays */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (other)");
			if (init_other() != Parser.Error.NONE) Utilities.quit("Cannot initialize other stuff");

			/* Initialize some other arrays */
			Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initializing arrays... (alloc)");
			if (init_alloc() != Parser.Error.NONE) Utilities.quit("Cannot initialize alloc stuff");
		}

		

		/*
		 * Initialize some other arrays
		 */
		static Parser.Error init_other()
		{
			/*** Prepare the various "bizarre" arrays ***/

			/* Initialize the "quark" package */
			Quark.Init();

			/* Initialize squelch things */
			Squelch.Init();
			TextUI.knowledge_init();

			/* Initialize the "message" package */
			Message.Init();

			/*** Prepare grid arrays ***/

			/* Array of grids */
			Misc.temp_g = new ushort[Misc.TEMP_MAX];

			Cave.instance = new Cave();

			/* Array of stacked monster messages */
			Monster_Message.mon_msg = new Monster_Race_Message[Misc.MAX_STORED_MON_MSG];
			Monster_Message.mon_message_hist = new Monster_Message_History[Misc.MAX_STORED_MON_CODES];

			/*** Prepare "vinfo" array ***/

			/* Used by "update_view()" */
			Cave.vinfo_init();


			/*** Prepare entity arrays ***/

			/* Objects */
			Object.Object.Init();

			/*** Prepare lore array ***/

			/* Lore */
			Misc.l_list = new Monster_Lore[Misc.z_info.r_max];


			/*** Prepare mouse buttons ***/
			Button.Init(Button.add_text, Button.kill_text);


			/*** Prepare quest array ***/

			/* Quests */
			Misc.q_list = new Quest[Misc.MAX_Q_IDX];


			/*** Prepare the inventory ***/

			/* Allocate it */
			Player.Player.instance.inventory = new Object.Object[Misc.ALL_INVEN_TOTAL];
			for(int i = 0; i < Player.Player.instance.inventory.Count(); i++) {
				Player.Player.instance.inventory[i] = new Object.Object();
			}


			/*** Prepare the options ***/
			Option.set_defaults();

			/* Initialize the window flags */
			for (int i = 0; i < Misc.ANGBAND_TERM_MAX; i++)
			{
			    /* Assume no flags */
			    Player_Other.instance.window_flag[i] = (uint)0L; //God damn it, who wrote that?!?! -werror should be on!!!
			}


			/*** Pre-allocate space for the "format()" buffer ***/ //Nick: Not needed in C#
			/* Hack -- Just call the "format()" function */
			//format("I wish you could swim, like dolphins can swim...");

			/* Success */
			return (0);
		}

		/*
		 * An entry for the object/monster allocation functions
		 *
		 * Pass 1 is determined from allocation information
		 * Pass 2 is determined from allocation restriction
		 * Pass 3 is determined from allocation calculation
		 */
		public class alloc_entry
		{
			public alloc_entry() {
			}
			public short index;		/* The actual index */

			public byte level;		/* Base dungeon level */
			public byte prob1;		/* Probability, pass 1 */
			public byte prob2;		/* Probability, pass 2 */
			public byte prob3;		/* Probability, pass 3 */

			public ushort total;		/* Unused for now */
		};

		/*
		 * The size of the "alloc_ego_table"
		 */
		public static short alloc_ego_size;

		/*
		 * The array[alloc_ego_size] of entries in the "ego allocator table"
		 */
		public static alloc_entry[] alloc_ego_table;


		/*
		 * The size of "alloc_race_table" (at most z_info.r_max)
		 */
		public static short alloc_race_size;

		/*
		 * The array[alloc_race_size] of entries in the "race allocator table"
		 */
		public static alloc_entry[] alloc_race_table;

		/*
		 * Initialize some other arrays
		 */
		static Parser.Error init_alloc()
		{
			int i;

			Monster_Race r_ptr;

			Ego_Item e_ptr;

			alloc_entry[] table;

			ushort[] num = new ushort[Misc.MAX_DEPTH]; //SHOULD NEVER BE NEGATIVE!!!

			ushort[] aux = new ushort[Misc.MAX_DEPTH];


			/*** Initialize object allocation info ***/
			Object.Object.init_obj_alloc();

			/*** Analyze monster allocation info ***/

			/* Clear the "aux" array */
			//aux.Initialize();

			/* Clear the "num" array */
			//num.Initialize();

			/* Size of "alloc_race_table" */
			alloc_race_size = 0;

			/* Scan the monsters (not the ghost) */
			for (i = 1; i < Misc.z_info.r_max - 1; i++)
			{
			    /* Get the i'th race */
			    r_ptr = Misc.r_info[i];
				if(r_ptr == null)
					continue;

			    /* Legal monsters */
			    if (r_ptr.rarity != 0)
			    {
			        /* Count the entries */
			        alloc_race_size++;

			        /* Group by level */
			        num[r_ptr.level]++;
			    }
			}

			/* Collect the level indexes */
			for (i = 1; i < Misc.MAX_DEPTH; i++)
			{
			    /* Group by level */
			    num[i] += num[i-1];
			}

			/* Paranoia */
			if (num[0] == 0) Utilities.quit("No town monsters!");


			/*** Initialize monster allocation info ***/

			/* Allocate the alloc_race_table */
			alloc_race_table = new alloc_entry[alloc_race_size];
			for(int q = 0; q < alloc_race_table.Length; q++) {
				alloc_race_table[q] = new alloc_entry();
			}

			/* Get the table entry */
			table = alloc_race_table;

			/* Scan the monsters (not the ghost) */
			for (i = 1; i < Misc.z_info.r_max - 1; i++)
			{
			    /* Get the i'th race */
			    r_ptr = Misc.r_info[i];
				if(r_ptr == null)
					continue;

			    /* Count valid pairs */
			    if (r_ptr.rarity != 0)
			    {
			        int p, x, y, z;

			        /* Extract the base level */
			        x = r_ptr.level;

			        /* Extract the base probability */
			        p = (100 / r_ptr.rarity);

			        /* Skip entries preceding our locale */
			        y = (x > 0) ? num[x-1] : 0;

			        /* Skip previous entries at this locale */
			        z = y + aux[x];

			        /* Load the entry */
			        table[z].index = (short)i;
			        table[z].level = (byte)x;
			        table[z].prob1 = (byte)p;
			        table[z].prob2 = (byte)p;
			        table[z].prob3 = (byte)p;

			        /* Another entry complete for this locale */
			        aux[x]++;
			    }
			}

			/*** Analyze ego_item allocation info ***/

			/* Clear the "aux" array */
			/* Clear the "num" array */
			for(int q = 0; q < aux.Length; q++) {
				aux[q] = 0;
				num[q] = 0;
			}

			/* Size of "alloc_ego_table" */
			alloc_ego_size = 0;

			/* Scan the ego items */
			for (i = 1; i < Misc.z_info.e_max; i++)
			{
			    /* Get the i'th ego item */
			    e_ptr = Misc.e_info[i];
				if(e_ptr == null)
					continue;

			    /* Legal items */
			    if (e_ptr.rarity != 0)
			    {
			        /* Count the entries */
			        alloc_ego_size++;

			        /* Group by level */
			        num[e_ptr.level]++;
			    }
			}

			/* Collect the level indexes */
			for (i = 1; i < Misc.MAX_DEPTH; i++)
			{
			    /* Group by level */
			    num[i] += num[i-1];
			}

			/*** Initialize ego-item allocation info ***/

			/* Allocate the alloc_ego_table */
			alloc_ego_table = new alloc_entry[alloc_ego_size];
			for(int q = 0; q < alloc_ego_table.Length; q++) {
				alloc_ego_table[q] = new alloc_entry();
			}

			/* Get the table entry */
			table = alloc_ego_table;

			/* Scan the ego-items */
			for (i = 1; i < Misc.z_info.e_max; i++)
			{
			    /* Get the i'th ego item */
			    e_ptr = Misc.e_info[i];
				if(e_ptr == null)
					continue;

			    /* Count valid pairs */
			    if (e_ptr.rarity != 0)
			    {
			        int p, x, y, z;

			        /* Extract the base level */
			        x = e_ptr.level;

			        /* Extract the base probability */
			        p = (100 / e_ptr.rarity);

			        /* Skip entries preceding our locale */
			        y = (x > 0) ? num[x-1] : 0;

			        /* Skip previous entries at this locale */
			        z = y + aux[x];

			        /* Load the entry */
			        table[z].index = (short)i;
			        table[z].level = (byte)x;
			        table[z].prob1 = (byte)p;
			        table[z].prob2 = (byte)p;
			        table[z].prob3 = (byte)p;

			        /* Another entry complete for this locale */
			        aux[x]++;
			    }
			}


			/* Success */
			return (0);
		}

		/*
		 * Hack -- main Angband initialization entry point
		 *
		 * Verify some files, display the "news.txt" file, create
		 * the high score file, initialize all internal arrays, and
		 * load the basic "user pref files".
		 *
		 * Be very careful to keep track of the order in which things
		 * are initialized, in particular, the only thing *known* to
		 * be available when this function is called is the "z-term.c"
		 * package, and that may not be fully initialized until the
		 * end of this function, when the default "user pref files"
		 * are loaded and "Term_xtra(TERM_XTRA_REACT,0)" is called.
		 *
		 * Note that this function attempts to verify the "news" file,
		 * and the game aborts (cleanly) on failure, since without the
		 * "news" file, it is likely that the "lib" folder has not been
		 * correctly located.  Otherwise, the news file is displayed for
		 * the user.
		 *
		 * Note that this function attempts to verify (or create) the
		 * "high score" file, and the game aborts (cleanly) on failure,
		 * since one of the most common "extraction" failures involves
		 * failing to extract all sub-directories (even empty ones), such
		 * as by failing to use the "-d" option of "pkunzip", or failing
		 * to use the "save empty directories" option with "Compact Pro".
		 * This error will often be caught by the "high score" creation
		 * code below, since the "lib/apex" directory, being empty in the
		 * standard distributions, is most likely to be "lost", making it
		 * impossible to create the high score file.
		 *
		 * Note that various things are initialized by this function,
		 * including everything that was once done by "init_some_arrays".
		 *
		 * This initialization involves the parsing of special files
		 * in the "lib/edit" directories.
		 *
		 * Note that the "template" files are initialized first, since they
		 * often contain errors.  This means that macros and message recall
		 * and things like that are not available until after they are done.
		 *
		 * We load the default "user pref files" here in case any "color"
		 * changes are needed before character creation.
		 *
		 * Note that the "graf-xxx.prf" file must be loaded separately,
		 * if needed, in the first (?) pass through "TERM_XTRA_REACT".
		 */
		public static bool init_angband()
		{
		    Game_Event.signal(Game_Event.Event_Type.ENTER_INIT);

		    /*** Initialize some arrays ***/
		    Init.arrays();

		    /*** Load default user pref files ***/

		    /* Initialize feature info */
		    Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Loading basic user pref file...");

		    /* Process that file */
		    Prefs.process_pref_file("pref.prf", false, false);

		    /* Done */
		    Game_Event.signal_string(Game_Event.Event_Type.INITSTATUS, "Initialization complete");

		    /* Sneakily init command list */
		    Command.Init();

			//#ifdef ALLOW_BORG /* apw */
			//    /* Allow the screensaver to do its work  */
			//    if (screensaver)
			//    {
			//        event_signal(EVENT_LEAVE_INIT);
			//        return !file_exists(savefile);
			//    }
			//#endif /* ALLOW_BORG */

		    /* Ask for a "command" until we get one we like. */
		    while (true)
		    {
		        Game_Command command_req = new Game_Command();

		        if (Game_Command.get(cmd_context.CMD_INIT, ref command_req, true) == null)
		            continue;
		        else if (command_req.command == Command_Code.QUAFF)
		            Utilities.quit();
		        else if (command_req.command == Command_Code.NEWGAME)
		        {
		            Game_Event.signal(Game_Event.Event_Type.LEAVE_INIT);
		            return true;
		        }
		        else if (command_req.command == Command_Code.LOADFILE)
		        {
		            Game_Event.signal(Game_Event.Event_Type.LEAVE_INIT);
		            return false;
		        }
		    }
		}

		public static void init_display()
		{
			Game_Event.add_handler(Game_Event.Event_Type.ENTER_INIT, ui_enter_init, null);
			Game_Event.add_handler(Game_Event.Event_Type.LEAVE_INIT, ui_leave_init, null);

			Game_Event.add_handler(Game_Event.Event_Type.ENTER_GAME, ui_enter_game, null);
			Game_Event.add_handler(Game_Event.Event_Type.LEAVE_GAME, ui_leave_game, null);

			UIBirth.ui_init_birthstate_handlers();
		}

		/*
		 * Hack -- take notes on line 23
		 */
		static void splashscreen_note(Game_Event.Event_Type type, Game_Event data, object user)
		{
			Term.erase(0, 23, 255);
			Term.putstr(20, 23, -1, ConsoleColor.White, "[" + data.text + "]");
			Term.fresh();
		}

		static void show_splashscreen(Game_Event.Event_Type type, Game_Event data, object user)
		{
			//ang_file *fp;

			//char buf[1024];

			/*** Verify the "news" file ***/

			string buf = Misc.path_build(Misc.ANGBAND_DIR_FILE, "news.txt");
			if (!File.Exists(buf))
			{
				//char why[1024];

				/* Crash and burn */
				string why = "Cannot access the '" + buf + "' file!";
				
				throw new NotImplementedException(); //<-- Uncomment below and remove this
				//init_angband_aux(why);
			}


			/*** Display the "news" file ***/

			Term.clear();

			/* Open the News file */
			buf = Misc.path_build(Misc.ANGBAND_DIR_FILE, "news.txt"); //Didn't we just do this?
			FileStream fp = File.OpenRead(buf);
			StreamReader sr = new StreamReader(fp);

			Misc.text_out_hook = Utilities.text_out_to_screen;

			/* Dump */
			//if (fp)
			//{
				/* Dump the file to the screen */
				while (!sr.EndOfStream)
				{
					buf = sr.ReadLine();
					//We are skipping the version stuff, we will do that later I gues...
					//No big deal if we skip it, right?
					//string version_marker = strstr(buf, "$VERSION");
					//if (version_marker)
					//{
					//    ptrdiff_t pos = version_marker - buf;
					//    strnfmt(version_marker, sizeof(buf) - pos, "%-8s", buildver);
					//}

					Utilities.text_out_e("{0}", buf);
					Utilities.text_out("\n");
				}

				fp.Close();
			//}

			/* Flush it */
			Term.fresh();
		}

		/* ------------------------------------------------------------------------
		 * Initialising
		 * ------------------------------------------------------------------------ */
		static void ui_enter_init(Game_Event.Event_Type type, Game_Event data, object user)
		{
			show_splashscreen(type, data, user);

			/* Set up our splashscreen handlers */
			Game_Event.add_handler(Game_Event.Event_Type.INITSTATUS, splashscreen_note, null);
		}

		static void ui_leave_init(Game_Event.Event_Type type, Game_Event data, object user)
		{
			/* Remove our splashscreen handlers */
			Game_Event.remove_handler(Game_Event.Event_Type.INITSTATUS, splashscreen_note, null);
		}


		/* 
		 * There are a few functions installed to be triggered by several 
		 * of the basic player events.  For convenience, these have been grouped 
		 * in this list.
		 */
		static Game_Event.Event_Type[] player_events =
		{
			Game_Event.Event_Type.RACE_CLASS,
			Game_Event.Event_Type.PLAYERTITLE,
			Game_Event.Event_Type.EXPERIENCE,
			Game_Event.Event_Type.PLAYERLEVEL,
			Game_Event.Event_Type.GOLD,
			Game_Event.Event_Type.EQUIPMENT,  /* For equippy chars */
			Game_Event.Event_Type.STATS,
			Game_Event.Event_Type.HP,
			Game_Event.Event_Type.MANA,
			Game_Event.Event_Type.AC,

			Game_Event.Event_Type.MONSTERHEALTH,

			Game_Event.Event_Type.PLAYERSPEED,
			Game_Event.Event_Type.DUNGEONLEVEL,
		};

		static Game_Event.Event_Type[] statusline_events =
		{
			Game_Event.Event_Type.STUDYSTATUS,
			Game_Event.Event_Type.STATUS,
			Game_Event.Event_Type.DETECTIONSTATUS,
			Game_Event.Event_Type.STATE,
			Game_Event.Event_Type.MOUSEBUTTONS
		};

		static void ui_enter_game(Game_Event.Event_Type type, Game_Event data, object user)
		{
		    /* Because of the "flexible" sidebar, all these things trigger
		       the same function. */
		    Game_Event.add_handler_set(player_events, Sidebar.update_sidebar, null);

		    /* The flexible statusbar has similar requirements, so is
		       also trigger by a large set of events. */
		    Game_Event.add_handler_set(statusline_events, Statusline.update_statusline, null);

		    /* Player HP can optionally change the colour of the '@' now. */
		    Game_Event.add_handler(Game_Event.Event_Type.HP, Sidebar.hp_colour_change, null);

		    /* Simplest way to keep the map up to date - will do for now */
		    Game_Event.add_handler(Game_Event.Event_Type.MAP, Map.update_maps, Misc.angband_term[0]);
			//#if 0
			//    Game_Event.event_add_handler(EVENT_MAP, trace_map_updates, angband_term[0]);
			//#endif
		    /* Check if the panel should shift when the player's moved */
		    Game_Event.add_handler(Game_Event.Event_Type.PLAYERMOVED, Floor.check_panel, null);
		    Game_Event.add_handler(Game_Event.Event_Type.SEEFLOOR, Floor.see_floor_items, null);
		}

		static void ui_leave_game(Game_Event.Event_Type type, Game_Event data, object user)
		{
			throw new NotImplementedException();
		//    /* Because of the "flexible" sidebar, all these things trigger
		//       the same function. */
		//    event_remove_handler_set(player_events, N_ELEMENTS(player_events),
		//                  update_sidebar, null);

		//    /* The flexible statusbar has similar requirements, so is
		//       also trigger by a large set of events. */
		//    event_remove_handler_set(statusline_events, N_ELEMENTS(statusline_events),
		//                  update_statusline, null);

		//    /* Player HP can optionally change the colour of the '@' now. */
		//    event_remove_handler(EVENT_HP, hp_colour_change, null);

		//    /* Simplest way to keep the map up to date - will do for now */
		//    event_remove_handler(EVENT_MAP, update_maps, angband_term[0]);
		//#if 0
		//    event_remove_handler(EVENT_MAP, trace_map_updates, angband_term[0]);
		//#endif
		//    /* Check if the panel should shift when the player's moved */
		//    event_remove_handler(EVENT_PLAYERMOVED, check_panel, null);
		//    event_remove_handler(EVENT_SEEFLOOR, see_floor_items, null);
		}

		public static void cleanup_angband()
		{
			throw new NotImplementedException();
			///* Free the macros */
			//keymap_free();

			///* Free the allocation tables */
			//free_obj_alloc();
			//FREE(alloc_ego_table);
			//FREE(alloc_race_table);

			//event_remove_all_handlers();

			///* Free the stores */
			//if (stores) free_stores();

			///* Free the quest list */
			//FREE(q_list);

			//button_free();
			//FREE(p_ptr.inventory);

			///* Free the lore, monster, and object lists */
			//FREE(l_list);
			//objects_destroy();

			///* Free the temp array */
			//FREE(temp_g);

			//cave_free(cave);

			///* Free the stacked monster messages */
			//FREE(mon_msg);
			//FREE(mon_message_hist);

			///* Free the messages */
			//messages_free();

			///* Free the history */
			//history_clear();

			///* Free the "quarks" */
			//quarks_free();

			//cleanup_parser(&k_parser);
			//cleanup_parser(&kb_parser);
			//cleanup_parser(&a_parser);
			//cleanup_parser(&names_parser);
			//cleanup_parser(&r_parser);
			//cleanup_parser(&rb_parser);
			//cleanup_parser(&f_parser);
			//cleanup_parser(&e_parser);
			//cleanup_parser(&p_parser);
			//cleanup_parser(&c_parser);
			//cleanup_parser(&v_parser);
			//cleanup_parser(&h_parser);
			//cleanup_parser(&flavor_parser);
			//cleanup_parser(&s_parser);
			//cleanup_parser(&hints_parser);
			//cleanup_parser(&mp_parser);
			//cleanup_parser(&pit_parser);
			//cleanup_parser(&z_parser);

			///* Free the format() buffer */
			//vformat_kill();

			///* Free the directories */
			//string_free(ANGBAND_DIR_APEX);
			//string_free(ANGBAND_DIR_EDIT);
			//string_free(ANGBAND_DIR_FILE);
			//string_free(ANGBAND_DIR_HELP);
			//string_free(ANGBAND_DIR_INFO);
			//string_free(ANGBAND_DIR_SAVE);
			//string_free(ANGBAND_DIR_PREF);
			//string_free(ANGBAND_DIR_USER);
			//string_free(ANGBAND_DIR_XTRA);

			//string_free(ANGBAND_DIR_XTRA_FONT);
			//string_free(ANGBAND_DIR_XTRA_GRAF);
			//string_free(ANGBAND_DIR_XTRA_SOUND);
			//string_free(ANGBAND_DIR_XTRA_ICON);
		}

		/*
		 * Reset the "visual" lists
		 *
		 * This involves resetting various things to their "default" state.
		 *
		 * If the "prefs" flag is true, then we will also load the appropriate
		 * "user pref file" based on the current setting of the "use_graphics"
		 * flag.  This is useful for switching "graphics" on/off.
		 */
		/* XXX this does not belong here */
		//Todo: Figure out where it goes. At a glance, init seems nice.
		//Nick: Was previously in "obj-util.c"... lulz
		public static void reset_visuals(bool load_prefs)
		{
			int i;
			Flavor f;

			/* Extract default attr/char code for features */
			for (i = 0; i < Misc.z_info.f_max; i++)
			{
				int j;
				Feature f_ptr = Misc.f_info[i];

				/* Assume we will use the underlying values */
				for (j = 0; j < (int)Grid_Data.grid_light_level.FEAT_LIGHTING_MAX; j++)
				{
					f_ptr.x_attr[j] = f_ptr.d_attr;
					f_ptr.x_char[j] = f_ptr.d_char;
				}
			}

			/* Extract default attr/char code for objects */
			for (i = 0; i < Misc.z_info.k_max; i++)
			{
				Object.Object_Kind k_ptr = Misc.k_info[i];
				if(k_ptr == null)
					continue;

				/* Default attr/char */
				k_ptr.x_attr = k_ptr.d_attr;
				k_ptr.x_char = k_ptr.d_char;
			}

			/* Extract default attr/char code for monsters */
			for (i = 0; i < Misc.z_info.r_max; i++)
			{
				Monster.Monster_Race r_ptr = Misc.r_info[i];
				if(r_ptr == null)
					continue;

				/* Default attr/char */
				r_ptr.x_attr = r_ptr.d_attr;
				r_ptr.x_char = r_ptr.d_char;
			}

			/* Extract default attr/char code for flavors */
			for (f = Misc.flavors; f != null; f = f.next)
			{
				f.x_attr = f.d_attr;
				f.x_char = f.d_char;
			}

			/* Extract attr/chars for inventory objects (by tval) */
			for (i = 0; i < Misc.tval_to_attr.Length; i++)
			{
				/* Default to white */
				Misc.tval_to_attr[i] = ConsoleColor.White;
			}

			if (!load_prefs)
				return;



			/* Graphic symbols */
			if (Misc.use_graphics != 0)
				Prefs.process_pref_file("graf.prf", false, false);

			/* Normal symbols */
			else
				Prefs.process_pref_file("font.prf", false, false);

			//#ifdef ALLOW_BORG_GRAPHICS
			//    /* Initialize the translation table for the borg */
			//    init_translate_visuals();
			//#endif /* ALLOW_BORG_GRAPHICS */
		}
	}
}
