using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Player;
using CSAngband.Object;

namespace CSAngband {
	class UIBirth {
		/*
		 * Overview
		 * ========
		 * This file implements the user interface side of the birth process
		 * for the classic terminal-based UI of Angband.
		 *
		 * It models birth as a series of steps which must be carried out in 
		 * a specified order, with the option of stepping backwards to revisit
		 * past choices.
		 *
		 * It starts when we receive the EVENT_ENTER_BIRTH event from the game,
		 * and ends when we receive the EVENT_LEAVE_BIRTH event.  In between,
		 * we will repeatedly be asked to supply a game command, which change
		 * the state of the character being rolled.  Once the player is happy
		 * with their character, we send the CMD_ACCEPT_CHARACTER command.
		 */


		/* A local-to-this-file global to hold the most important bit of state
		   between calls to the game proper.  Probably not strictly necessary,
		   but reduces complexity a bit. */
		enum birth_stage {
			BIRTH_BACK = -1,
			BIRTH_RESET = 0,
			BIRTH_QUICKSTART,
			BIRTH_SEX_CHOICE,
			BIRTH_RACE_CHOICE,
			BIRTH_CLASS_CHOICE,
			BIRTH_ROLLER_CHOICE,
			BIRTH_POINTBASED,
			BIRTH_ROLLER,
			BIRTH_NAME_CHOICE,
			BIRTH_FINAL_CONFIRM,
			BIRTH_COMPLETE
		};


		enum birth_questions {
			BQ_METHOD = 0,
			BQ_SEX,
			BQ_RACE,
			BQ_CLASS,
			BQ_ROLLER,
			MAX_BIRTH_QUESTIONS
		};

		enum birth_rollers {
			BR_POINTBASED = 0,
			BR_NORMAL,
			MAX_BIRTH_ROLLERS
		};


		//static void point_based_start();
		static bool quickstart_allowed = false;

		/* ------------------------------------------------------------------------
		 * Quickstart? screen.
		 * ------------------------------------------------------------------------ */
		static birth_stage get_quickstart_command() {
			throw new NotImplementedException();
			//const char *prompt = "['Y' to use this character, 'N' to start afresh, 'C' to change name]";

			//enum birth_stage next = BIRTH_QUICKSTART;

			///* Prompt for it */
			//prt("New character based on previous one:", 0, 0);
			//prt(prompt, Term.hgt - 1, Term.wid / 2 - strlen(prompt) / 2);

			///* Buttons */
			//button_kill_all();
			//button_add("[Y]", 'y');
			//button_add("[N]", 'n');
			//button_add("[C]", 'c');
			//redraw_stuff(p_ptr);

			//do
			//{
			//    /* Get a key */
			//    struct keypress ke = inkey();

			//    if (ke.code == 'N' || ke.code == 'n')
			//    {
			//        cmd_insert(CMD_BIRTH_RESET);
			//        next = BIRTH_SEX_CHOICE;
			//    }
			//    else if (ke.code == KTRL('X'))
			//    {
			//        cmd_insert(CMD_QUIT);
			//        next = BIRTH_COMPLETE;
			//    }
			//    else if (ke.code == 'C' || ke.code == 'c')
			//    {
			//        next = BIRTH_NAME_CHOICE;
			//    }
			//    else if (ke.code == 'Y' || ke.code == 'y')
			//    {
			//        cmd_insert(CMD_ACCEPT_CHARACTER);
			//        next = BIRTH_COMPLETE;
			//    }
			//} while (next == BIRTH_QUICKSTART);

			///* Buttons */
			//button_kill_all();
			//redraw_stuff(p_ptr);

			///* Clear prompt */
			//clear_from(23);

			//return next;
		}

		/* ------------------------------------------------------------------------
		 * The various "menu" bits of the birth process - namely choice of sex,
		 * race, class, and roller type.
		 * ------------------------------------------------------------------------ */

		/* The various menus */
		static Menu_Type sex_menu, race_menu, class_menu, roller_menu;

		/* Locations of the menus, etc. on the screen */
		const int HEADER_ROW = 1;
		const int QUESTION_ROW = 7;
		const int TABLE_ROW = 9;

		const int QUESTION_COL = 2;
		const int SEX_COL = 2;
		const int RACE_COL = 14;
		const int RACE_AUX_COL = 29;
		const int CLASS_COL = 29;
		const int CLASS_AUX_COL = 43;
		const int ROLLER_COL = 43;

		const int MENU_ROWS = TABLE_ROW + 14;

		/* upper left column and row, width, and lower column */
		static Region gender_region = new Region(SEX_COL, TABLE_ROW, 14, MENU_ROWS);
		static Region race_region = new Region(RACE_COL, TABLE_ROW, 14, MENU_ROWS);
		static Region class_region = new Region(CLASS_COL, TABLE_ROW, 14, MENU_ROWS);
		static Region roller_region = new Region(ROLLER_COL, TABLE_ROW, 28, MENU_ROWS);

		/* We use different menu "browse functions" to display the help text
		   sometimes supplied with the menu items - currently just the list
		   of bonuses, etc, corresponding to each race and class. */
		delegate void browse_f(int oid, object db, Region l);

		/* We have one of these structures for each menu we display - it holds
		   the useful information for the menu - text of the menu items, "help"
		   text, current (or default) selection, and whether random selection
		   is allowed. */
		class birthmenu_data {
			public string[] items;
			public string hint;
			public bool allow_random;
		};

		/* A custom "display" function for our menus that simply displays the
		   text from our stored data in a different colour if it's currently
		   selected. */
		static void birthmenu_display(Menu_Type menu, int oid, bool cursor, int row, int col, int width) {
			birthmenu_data data = menu.menu_data as birthmenu_data;

			ConsoleColor attr = Menu_Type.curs_attrs[(int)Menu_Type.CURS.KNOWN][cursor?1:0];
			Utilities.c_put_str(attr, data.items[oid], row, col);
		}

		/* Our custom menu iterator, only really needed to allow us to override
		   the default handling of "commands" in the standard iterators (hence
		   only defining the display and handler parts). */
		static Menu_Type.menu_iter birth_iter = new Menu_Type.menu_iter(null, null, birthmenu_display, null, null);

		static void skill_help(short[] skills, int mhp, int exp, int infra) {
			Utilities.text_out_e("Hit/Shoot/Throw: {0}/{1}/{2}\n", skills[(int)Skill.TO_HIT_MELEE], skills[(int)Skill.TO_HIT_BOW], skills[(int)Skill.TO_HIT_THROW]);
			Utilities.text_out_e("Hit die: {0}   XP mod: {1}%\n", mhp, exp);
			Utilities.text_out_e("Disarm: {0}   Devices: {1}\n", skills[(int)Skill.DISARM], skills[(int)Skill.DEVICE]);
			Utilities.text_out_e("Save:   {0}   Stealth: {1}\n", skills[(int)Skill.SAVE], skills[(int)Skill.STEALTH]);
			if (infra >= 0)
			    Utilities.text_out_e("Infravision:  {0} ft\n", infra * 10);
			Utilities.text_out_e("Digging:      {0}\n", skills[(int)Skill.DIGGING]);
			Utilities.text_out_e("Search:       {0}/{1}", skills[(int)Skill.SEARCH], skills[(int)Skill.SEARCH_FREQUENCY]);
			if (infra < 0)
			    Utilities.text_out_e("\n");
		}

		static string get_flag_desc(int flag) {
			if (flag == Object_Flag.SUST_STR.value)
				return "Sustains strength";
			else if (flag == Object_Flag.SUST_DEX.value)
				return "Sustains dexterity";
			else if (flag == Object_Flag.SUST_CON.value)
				return "Sustains constitution";
			else if (flag == Object_Flag.RES_POIS.value)
				return "Resists poison";
			else if (flag == Object_Flag.RES_LIGHT.value)
				return "Resists light damage";
			else if (flag == Object_Flag.RES_DARK.value)
				return "Resists darkness damage";
			else if (flag == Object_Flag.RES_BLIND.value)
				return "Resists blindness";
			else if (flag == Object_Flag.HOLD_LIFE.value)
				return "Sustains experience";
			else if (flag == Object_Flag.FREE_ACT.value)
				return "Resists paralysis";
			else if (flag == Object_Flag.REGEN.value)
				return "Regenerates quickly";
			else if (flag == Object_Flag.SEE_INVIS.value)
				return "Sees invisible creatures";
			else
				return "Undocumented flag";
		}

		static string get_pflag_desc(int flag) {
			throw new NotImplementedException();
			//switch (flag)
			//{
			//    case PF_EXTRA_SHOT: return "Gains extra shots with bow";
			//    case PF_BRAVERY_30: return "Gains immunity to fear";
			//    case PF_BLESS_WEAPON: return "Prefers blunt/blessed weapons";
			//    case PF_CUMBER_GLOVE: return null;
			//    case PF_ZERO_FAIL: return "Advanced spellcasting";
			//    case PF_BEAM: return null;
			//    case PF_CHOOSE_SPELLS: return null;
			//    case PF_PSEUDO_ID_IMPROV: return null;
			//    case PF_KNOW_MUSHROOM: return "Identifies mushrooms";
			//    case PF_KNOW_ZAPPER: return "Identifies magic devices";
			//    case PF_SEE_ORE: return "Senses ore/minerals";
			//    default: return "Undocumented pflag";
			//}
		}

		static void race_help(int i, object db, Region l) {
			int j;
			int k;
			Player_Race r = Player_Race.player_id2race(i);
			int len = ((int)Stat.Max + 1) / 2;

			int n_flags = 0;
			int flag_space = 3;

			if (r == null) return;

			/* Output to the screen */
			Misc.text_out_hook = Utilities.text_out_to_screen;

			/* Indent output */
			Misc.text_out_indent = RACE_AUX_COL;
			Term.gotoxy(RACE_AUX_COL, TABLE_ROW);

			for (j = 0; j < len; j++)
			{  
			    string name1 = Stat_Names.Reduced[j];
			    string name2 = Stat_Names.Reduced[j + len];

			    int adj1 = r.r_adj[j];
			    int adj2 = r.r_adj[j + len];

			    Utilities.text_out_e("%s%+3d  %s%+3d\n", name1, adj1, name2, adj2);
			}

			Utilities.text_out_e("\n");
			skill_help(r.r_skills, r.r_mhp, r.r_exp, r.infra);
			Utilities.text_out_e("\n");

			for (k = 0; k < Object_Flag.MAX.value; k++)
			{
			    if (n_flags >= flag_space) break;
			    if (!r.flags.has(k)) continue;
			    Utilities.text_out_e("\n%s", get_flag_desc(k));
			    n_flags++;
			}

			for (k = 0; k < Misc.PF.MAX.value; k++)
			{
			    if (n_flags >= flag_space) break;
			    if (!r.pflags.has(k)) continue;
			    Utilities.text_out_e("\n%s", get_pflag_desc(k));
			    n_flags++;
			}

			while(n_flags < flag_space)
			{
			    Utilities.text_out_e("\n");
			    n_flags++;
			}

			/* Reset text_out() indentation */
			Misc.text_out_indent = 0;
		}

		static void class_help(int i, object db, Region l) {
			int j;
			int k;
			Player_Class c = Player_Class.player_id2class(i);
			int len = (((int)Stat.Max) + 1) / 2;

			int n_flags = 0;
			int flag_space = 5;

			if (c == null) return;

			/* Output to the screen */
			Misc.text_out_hook = Utilities.text_out_to_screen;

			/* Indent output */
			Misc.text_out_indent = CLASS_AUX_COL;
			Term.gotoxy(CLASS_AUX_COL, TABLE_ROW);

			for (j = 0; j < len; j++)
			{  
			    string name1 = Stat_Names.Reduced[j];
			    string name2 = Stat_Names.Reduced[j + len];

			    int adj1 = c.c_adj[j] + Player.Player.instance.Race.r_adj[j];
			    int adj2 = c.c_adj[j + len] + Player.Player.instance.Race.r_adj[j + len];

			    Utilities.text_out_e("%s%+3d  %s%+3d\n", name1, adj1, name2, adj2);
			}

			Utilities.text_out_e("\n");

			skill_help(c.c_skills, c.c_mhp, c.c_exp, -1);

			if (c.spell_book == TVal.TV_MAGIC_BOOK) {
			    Utilities.text_out_e("\nLearns arcane magic");
			} else if (c.spell_book == TVal.TV_PRAYER_BOOK) {
			    Utilities.text_out_e("\nLearns divine magic");
			}

			for (k = 0; k < Misc.PF.MAX.value; k++)
			{
			    string s;
			    if (n_flags >= flag_space) break;
			    if (!c.pflags.has(k)) continue;
			    s = get_pflag_desc(k);
			    if (s.Length == 0) continue;
			    Utilities.text_out_e("\n%s", s);
			    n_flags++;
			}

			while(n_flags < flag_space)
			{
			    Utilities.text_out_e("\n");
			    n_flags++;
			}

			/* Reset text_out() indentation */
			Misc.text_out_indent = 0;
		}

		/* Set up one of our menus ready to display choices for a birth question.
		   This is slightly involved. */
		static void init_birth_menu(ref Menu_Type menu, int n_choices, int initial_choice, Region reg, bool allow_random, Menu_Type.browse_hook_func aux) {
			birthmenu_data menu_data;

			/* Initialise a basic menu */
			menu = new Menu_Type(Menu_Type.skin_id.SCROLL, birth_iter);

			/* A couple of behavioural flags - we want selections letters in
			   lower case and a double tap to act as a selection. */
			menu.selections = TextUI.lower_case;
			menu.flags = (int)Menu_Type.menu_type_flags.MN_DBL_TAP;

			/* Copy across the game's suggested initial selection, etc. */
			menu.cursor = initial_choice;

			/* Allocate sufficient space for our own bits of menu information. */
			menu_data = new birthmenu_data();

			/* Allocate space for an array of menu item texts and help texts
			   (where applicable) */
			menu_data.items = new string[n_choices];
			menu_data.allow_random = allow_random;

			/* Set private data */
			menu.priv(n_choices, menu_data);

			/* Set up the "browse" hook to display help text (where applicable). */
			menu.browse_hook = aux;

			/* Lay out the menu appropriately */
			menu.layout(reg);
		}



		static void setup_menus() {
			int i, n;
			Player_Class c;
			Player_Race r;

			string [] roller_choices = new string[(int)birth_rollers.MAX_BIRTH_ROLLERS]{ 
			    "Point-based", 
			    "Standard roller" 
			};

			birthmenu_data mdata;

			/* Sex menu fairly straightforward */
			init_birth_menu(ref sex_menu, Misc.MAX_SEXES, Player.Player.instance.psex, gender_region, true, null);
			mdata = sex_menu.menu_data as birthmenu_data;
			for (i = 0; i < Misc.MAX_SEXES; i++)
			    mdata.items[i] = Misc.sex_info[i].Title;
			mdata.hint = "Your 'sex' does not have any significant gameplay effects.";

			n = 0;
			for (r = Misc.races; r != null; r = r.Next) n++;
			/* Race menu more complicated. */
			init_birth_menu(ref race_menu, n, (int)(Player.Player.instance.Race != null ? Player.Player.instance.Race.ridx : 0),
			                race_region, true, race_help);
			mdata = race_menu.menu_data as birthmenu_data;

			for (i = 0, r = Misc.races; r != null; r = r.Next, i++)
			    mdata.items[r.ridx] = r.Name;
			mdata.hint = "Your 'race' determines various intrinsic factors and bonuses.";

			n = 0;
			for (c = Misc.classes; c != null; c = c.next) n++;
			/* Class menu similar to race. */
			init_birth_menu(ref class_menu, n, (int)(Player.Player.instance.Class == null ? Player.Player.instance.Class.cidx : 0),
			                class_region, true, class_help);
			mdata = class_menu.menu_data as birthmenu_data;

			for (i = 0, c = Misc.classes; c != null; c = c.next, i++)
			    mdata.items[c.cidx] = c.Name;
			mdata.hint = "Your 'class' determines various intrinsic abilities and bonuses";

			/* Roller menu straightforward again */
			init_birth_menu(ref roller_menu, (int)birth_rollers.MAX_BIRTH_ROLLERS, 0, roller_region, false, null);
			mdata = roller_menu.menu_data as birthmenu_data;
			for (i = 0; i < (int)birth_rollers.MAX_BIRTH_ROLLERS; i++)
			    mdata.items[i] = roller_choices[i];
			mdata.hint = "Your choice of character generation.  Point-based is recommended.";
		}

		/* Cleans up our stored menu info when we've finished with it. */
		static void free_birth_menu(Menu_Type menu) {
			//We're good, let garbage collection handle it
			//struct birthmenu_data *data = menu.menu_data;

			//if (data)
			//{
			//    mem_free(data.items);
			//    mem_free(data);
			//}
		}

		static void free_birth_menus() {
			/* We don't need these any more. */
			free_birth_menu(sex_menu);
			free_birth_menu(race_menu);
			free_birth_menu(class_menu);
			free_birth_menu(roller_menu);
		}

		/*
		 * Clear the previous question
		 */
		static void clear_question() {
			int i;

			for (i = QUESTION_ROW; i < TABLE_ROW; i++)
			{
			    /* Clear line, position cursor */
			    Term.erase(0, i, 255);
			}
		}


		const string BIRTH_MENU_HELPTEXT =
			"{lightblue}Please select your character from the menu below:{/}\n\n\n" +
			"Use the {lightgreen}movement keys{/} to scroll the menu, \n" +
			"{lightgreen}Enter{/} to select the current menu item, '{lightgreen}*{/}' \n" +
			"for a random menu item, '{lightgreen}ESC{/}' to step back through the \n" +
			"birth process, '{lightgreen}={/}' for the birth options, '{lightgreen}?{/} \n" +
			"for help, or '{lightgreen}Ctrl-X{/}' to quit.\n";

		/* Show the birth instructions on an otherwise blank screen */
		static void print_menu_instructions() {
			/* Clear screen */
			Term.clear();

			/* Output to the screen */
			Misc.text_out_hook = Utilities.text_out_to_screen;

			/* Indent output */
			Misc.text_out_indent = QUESTION_COL;
			Term.gotoxy(QUESTION_COL, HEADER_ROW);

			/* Display some helpful information */
			Utilities.text_out_e(BIRTH_MENU_HELPTEXT);

			/* Reset text_out() indentation */
			Misc.text_out_indent = 0;
		}

		/* Allow the user to select from the current menu, and return the 
		   corresponding command to the game.  Some actions are handled entirely
		   by the UI (displaying help text, for instance). */
		static birth_stage menu_question(birth_stage current, Menu_Type current_menu, Command_Code choice_command) {
			birthmenu_data menu_data = current_menu.menu_data as birthmenu_data;

			birth_stage next = birth_stage.BIRTH_RESET;

			/* Print the question currently being asked. */
			clear_question();
			Term.putstr(QUESTION_COL, QUESTION_ROW, -1, ConsoleColor.Yellow, menu_data.hint);

			current_menu.cmd_keys = "?=*\x18";	 /* ?, =, *, <ctl-X> */

			while (next == birth_stage.BIRTH_RESET)
			{
			    /* Display the menu, wait for a selection of some sort to be made. */
			    ui_event cx = current_menu.select(ui_event_type.EVT_KBRD, false);

			    /* As all the menus are displayed in "hierarchical" style, we allow
			       use of "back" (left arrow key or equivalent) to step back in 
			       the proces as well as "escape". */
			    if (cx.type == ui_event_type.EVT_ESCAPE)
			    {
			        next = birth_stage.BIRTH_BACK;
			    }
			    else if (cx.type == ui_event_type.EVT_SELECT)
			    {
			        if (current == birth_stage.BIRTH_ROLLER_CHOICE)
			        {
						Game_Command.insert(Command_Code.FINALIZE_OPTIONS);

			            if (current_menu.cursor != 0)
			            {
			                /* Do a first roll of the stats */
							Game_Command.insert(Command_Code.ROLL_STATS);
			                next = current + 2;
			            }
			            else
			            {
			                /* 
			                 * Make sure we've got a point-based char to play with. 
			                 * We call point_based_start here to make sure we get
			                 * an update on the points totals before trying to
			                 * display the screen.  The call to CMD_RESET_STATS
			                 * forces a rebuying of the stats to give us up-to-date
			                 * totals.  This is, it should go without saying, a hack.
			                 */
			                point_based_start();
							Game_Command.insert(Command_Code.RESET_STATS);
							Game_Command.get_top().set_arg_choice(0, 1);
			                next = current + 1;
			            }
			        }
			        else
			        {
						Game_Command.insert(choice_command);
			            Game_Command.get_top().set_arg_choice(0, current_menu.cursor);
			            next = current + 1;
			        }
			    }
			    else if (cx.type == ui_event_type.EVT_KBRD)
			    {
			        /* '*' chooses an option at random from those the game's provided. */
			        if (cx.key.code == (keycode_t)'*' && menu_data.allow_random) 
			        {
			            current_menu.cursor = Random.randint0(current_menu.count);
						Game_Command.insert(choice_command);
						Game_Command.get_top().set_arg_choice(0, current_menu.cursor);
			            current_menu.refresh(false);
			            next = current + 1;
			        }
			        else if (cx.key.code == (keycode_t)'=') 
			        {
			            Do_Command.options_birth();
			            next = current;
			        }
			        else if (cx.key.code == (keycode_t)UIEvent.KTRL('X')) 
			        {
						Game_Command.insert(Command_Code.QUIT);
			            next = birth_stage.BIRTH_COMPLETE;
			        }
			        else if (cx.key.code == (keycode_t)'?')
			        {
			            Do_Command.help();
			        }
			    }
			}

			return next;
		}

		/* ------------------------------------------------------------------------
		 * The rolling bit of the roller.
		 * ------------------------------------------------------------------------ */
		const int ROLLERCOL = 42;

		static birth_stage roller_command(bool first_call) {
			throw new NotImplementedException();
			//char prompt[80] = "";
			//size_t promptlen = 0;

			//struct keypress ch;

			//enum birth_stage next = BIRTH_ROLLER;

			///* Used to keep track of whether we've rolled a character before or not. */
			//static bool prev_roll = false;

			///* Display the player - a bit cheaty, but never mind. */
			//display_player(0);

			//if (first_call)
			//    prev_roll = false;

			///* Add buttons */
			//button_add("[ESC]", ESCAPE);
			//button_add("[Enter]", '\r');
			//button_add("[r]", 'r');
			//if (prev_roll) button_add("[p]", 'p');
			//clear_from(Term.hgt - 2);
			//redraw_stuff(p_ptr);

			///* Prepare a prompt (must squeeze everything in) */
			//strnfcat(prompt, sizeof (prompt), &promptlen, "['r' to reroll");
			//if (prev_roll) 
			//    strnfcat(prompt, sizeof(prompt), &promptlen, ", 'p' for prev");
			//strnfcat(prompt, sizeof (prompt), &promptlen, " or 'Enter' to accept]");

			///* Prompt for it */
			//prt(prompt, Term.hgt - 1, Term.wid / 2 - promptlen / 2);

			///* Prompt and get a command */
			//ch = inkey();

			//if (ch.code == ESCAPE) 
			//{
			//    button_kill('r');
			//    button_kill('p');

			//    next = BIRTH_BACK;
			//}

			///* 'Enter' accepts the roll */
			//if ((ch.code == '\r') || (ch.code == '\n')) 
			//{
			//    next = BIRTH_NAME_CHOICE;
			//}

			///* Reroll this character */
			//else if ((ch.code == ' ') || (ch.code == 'r'))
			//{
			//    cmd_insert(CMD_ROLL_STATS);
			//    prev_roll = true;
			//}

			///* Previous character */
			//else if (prev_roll && (ch.code == 'p'))
			//{
			//    cmd_insert(CMD_PREV_STATS);
			//}

			///* Quit */
			//else if (ch.code == KTRL('X')) 
			//{
			//    cmd_insert(CMD_QUIT);
			//    next = BIRTH_COMPLETE;
			//}

			///* Help XXX */
			//else if (ch.code == '?')
			//{
			//    do_cmd_help();
			//}

			///* Nothing handled directly here */
			//else
			//{
			//    bell("Illegal roller command!");
			//}

			///* Kill buttons */
			//button_kill(ESCAPE);
			//button_kill('\r');
			//button_kill('r');
			//button_kill('p');
			//redraw_stuff(p_ptr);

			//return next;
		}

		/* ------------------------------------------------------------------------
		 * Point-based stat allocation.
		 * ------------------------------------------------------------------------ */

		/* The locations of the "costs" area on the birth screen. */
		const int COSTS_ROW = 2;
		const int COSTS_COL = (42 + 32);
		const int TOTAL_COL = (42 + 19);

		/* This is called whenever a stat changes.  We take the easy road, and just
		   redisplay them all using the standard function. */
		static void point_based_stats(Game_Event.Event_Type type, Game_Event data, object user) {
			Files.display_player_stat_info();
		}

		/* This is called whenever any of the other miscellaneous stat-dependent things
		   changed.  We are hooked into changes in the amount of gold in this case,
		   but redisplay everything because it's easier. */
		static void point_based_misc(Game_Event.Event_Type type, Game_Event data, object user) {
			Files.display_player_xtra_info();
		}


		/* This is called whenever the points totals are changed (in birth.c), so
		   that we can update our display of how many points have been spent and
		   are available. */
		static void point_based_points(Game_Event.Event_Type type, Game_Event data, object user) {
			int sum = 0;
			int[] stats = data.birthstats.stats;

			/* Display the costs header */
			Utilities.put_str("Cost", COSTS_ROW - 1, COSTS_COL);

			/* Display the costs */
			for (int i = 0; i < (int)Stat.Max; i++)
			{
			    /* Display cost */
			    Utilities.put_str(stats[i].ToString("D4"), COSTS_ROW + i, COSTS_COL);
			    sum += stats[i];
			}

			Utilities.put_str("Total Cost: " + sum.ToString("D2") + "/" + 
				(data.birthstats.remaining + sum).ToString("D2"), COSTS_ROW + (int)Stat.Max, TOTAL_COL);
		}

		static void point_based_start() {
			string prompt = "[up/down to move, left/right to modify, 'r' to reset, 'Enter' to accept]";

			/* Clear */
			Term.clear();

			/* Display the player */
			Files.display_player_xtra_info();
			Files.display_player_stat_info();

			Utilities.prt(prompt, Term.instance.hgt - 1, Term.instance.wid / 2 - prompt.Length / 2);

			/* Register handlers for various events - cheat a bit because we redraw
			   the lot at once rather than each bit at a time. */
			Game_Event.add_handler(Game_Event.Event_Type.BIRTHPOINTS, point_based_points, null);	
			Game_Event.add_handler(Game_Event.Event_Type.STATS, point_based_stats, null);	
			Game_Event.add_handler(Game_Event.Event_Type.GOLD, point_based_misc, null);	
		}

		static void point_based_stop() {
			Game_Event.remove_handler(Game_Event.Event_Type.BIRTHPOINTS, point_based_points, null);
			Game_Event.remove_handler(Game_Event.Event_Type.STATS, point_based_stats, null);
			Game_Event.remove_handler(Game_Event.Event_Type.GOLD, point_based_misc, null);
		}

		static int stat = 0;
		static birth_stage point_based_command() {
				birth_stage next = birth_stage.BIRTH_POINTBASED;

				/*	point_based_display();*/
			    /* Place cursor just after cost of current stat */
				/* Draw the Selection Cursor */
				new Region(COSTS_COL + 4, COSTS_ROW, 1, 6).erase();
				Utilities.put_str("<", COSTS_ROW + stat, COSTS_COL + 4);

			    /* Get key */
			    keypress ch = Utilities.inkey();

			    if (ch.code == (keycode_t)UIEvent.KTRL('X')) 
			    {
					Game_Command.insert(Command_Code.QUIT);
			        next = birth_stage.BIRTH_COMPLETE;
			    }

			    /* Go back a step, or back to the start of this step */
			    else if (ch.code == keycode_t.ESCAPE) 
			    {
			        next = birth_stage.BIRTH_BACK;
			    }

			    else if (ch.code == (keycode_t)'r' || ch.code == (keycode_t)'R') 
			    {
					Game_Command.insert(Command_Code.RESET_STATS);
					Game_Command.get_top().set_arg_choice(0, 0);
			    }

			    /* Done */
			    else if ((ch.code == (keycode_t)'\r') || (ch.code == (keycode_t)'\n')) 
			    {
			        next = birth_stage.BIRTH_NAME_CHOICE;
			    }
			    else
			    {
			        int dir = Utilities.target_dir(ch);

			        /* Prev stat, looping round to the bottom when going off the top */
			        if (dir == 8)
			            stat = (stat + (int)Stat.Max - 1) % (int)Stat.Max;

			        /* Next stat, looping round to the top when going off the bottom */
			        if (dir == 2)
			            stat = (stat + 1) % (int)Stat.Max;

			        /* Decrease stat (if possible) */
			        if (dir == 4)
			        {
						Game_Command.insert(Command_Code.SELL_STAT);
						Game_Command.get_top().set_arg_choice(0, stat);
			        }

			        /* Increase stat (if possible) */
			        if (dir == 6)
			        {
						Game_Command.insert(Command_Code.BUY_STAT);
						Game_Command.get_top().set_arg_choice(0, stat);
			        }

			    }

			    return next;
		}

		/* ------------------------------------------------------------------------
		 * Asking for the player's chosen name.
		 * ------------------------------------------------------------------------ */
		static birth_stage get_name_command() {
			birth_stage next;
			string name = "";

			if (Utilities.get_name(ref name, 32))
			{
				Game_Command.insert(Command_Code.NAME_CHOICE);
				Game_Command.get_top().set_arg_choice(0, name);
			    next = birth_stage.BIRTH_FINAL_CONFIRM;
			}
			else
			{
			    next = birth_stage.BIRTH_BACK;
			}

			return next;
		}

		/* ------------------------------------------------------------------------
		 * Final confirmation of character.
		 * ------------------------------------------------------------------------ */
		static birth_stage get_confirm_command() {
			Player.Player p_ptr = Player.Player.instance;

			string prompt = "['ESC' to step back, 'S' to start over, or any other key to continue]";
			keypress ke;

			birth_stage next;

			/* Prompt for it */
			Utilities.prt(prompt, Term.instance.hgt - 1, Term.instance.wid / 2 - (prompt.Length / 2));

			/* Buttons */
			Button.button_kill_all();
			Button.button_add("[Continue]", 'q');
			Button.button_add("[ESC]", (char)keycode_t.ESCAPE);
			Button.button_add("[S]", 'S');
			p_ptr.redraw_stuff();

			/* Get a key */
			ke = Utilities.inkey();

			/* Start over */
			if (ke.code == (keycode_t)'S' || ke.code == (keycode_t)'s')
			{
			    next = birth_stage.BIRTH_RESET;
			}
			else if (ke.code == (keycode_t)UIEvent.KTRL('X'))
			{
				Game_Command.insert(Command_Code.QUIT);
			    next = birth_stage.BIRTH_COMPLETE;
			}
			else if (ke.code == keycode_t.ESCAPE)
			{
			    next = birth_stage.BIRTH_BACK;
			}
			else
			{
				Game_Command.insert(Command_Code.ACCEPT_CHARACTER);
			    next = birth_stage.BIRTH_COMPLETE;
			}

			/* Buttons */
			Button.button_kill_all();
			p_ptr.redraw_stuff();

			/* Clear prompt */
			Utilities.clear_from(23);

			return next;
		}



		/* ------------------------------------------------------------------------
		 * Things that relate to the world outside this file: receiving game events
		 * and being asked for game commands.
		 * ------------------------------------------------------------------------ */

		/*
		 * This is called when we receive a request for a command in the birth 
		 * process.

		 * The birth process continues until we send a final character confirmation
		 * command (or quit), so this is effectively called in a loop by the main
		 * game.
		 *
		 * We're imposing a step-based system onto the main game here, so we need
		 * to keep track of where we're up to, where each step moves on to, etc.
		 */
		static birth_stage current_stage = birth_stage.BIRTH_RESET;
		static birth_stage prev;
		static birth_stage roller = birth_stage.BIRTH_RESET;
		public static int get_birth_command(bool wait) {
			birth_stage next = current_stage;

			switch (current_stage)
			{
			    case birth_stage.BIRTH_RESET:
			    {
					Game_Command.insert(Command_Code.BIRTH_RESET);

			        roller = birth_stage.BIRTH_RESET;

			        if (quickstart_allowed)
			            next = birth_stage.BIRTH_QUICKSTART;
			        else
			            next = birth_stage.BIRTH_SEX_CHOICE;

			        break;
			    }

			    case birth_stage.BIRTH_QUICKSTART:
			    {
			        Files.display_player(0);
			        next = get_quickstart_command();
			        break;
			    }

			    case birth_stage.BIRTH_SEX_CHOICE:
			    case birth_stage.BIRTH_CLASS_CHOICE:
			    case birth_stage.BIRTH_RACE_CHOICE:
			    case birth_stage.BIRTH_ROLLER_CHOICE:
			    {
			        Menu_Type menu = sex_menu;
			        Command_Code command = Command_Code.CHOOSE_SEX;

			        Term.clear();
			        print_menu_instructions();

			        if (current_stage > birth_stage.BIRTH_SEX_CHOICE)
			        {
			            sex_menu.refresh(false);
			            menu = race_menu;
			            command = Command_Code.CHOOSE_RACE;
			        }

			        if (current_stage > birth_stage.BIRTH_RACE_CHOICE)
			        {
			            race_menu.refresh(false);
			            menu = class_menu;
			            command = Command_Code.CHOOSE_CLASS;
			        }

			        if (current_stage > birth_stage.BIRTH_CLASS_CHOICE)
			        {
			            class_menu.refresh(false);
			            menu = roller_menu;
			            command = Command_Code.NULL;
			        }

			        next = menu_question(current_stage, menu, command);

			        if (next == birth_stage.BIRTH_BACK)
			            next = current_stage - 1;

			        /* Make sure that the character gets reset before quickstarting */
			        if (next == birth_stage.BIRTH_QUICKSTART) 
			            next = birth_stage.BIRTH_RESET;

			        break;
			    }

			    case birth_stage.BIRTH_POINTBASED:
			    {
			        roller = birth_stage.BIRTH_POINTBASED;

			        if (prev > birth_stage.BIRTH_POINTBASED)
			            point_based_start();

			        next = point_based_command();

			        if (next == birth_stage.BIRTH_BACK)
			            next = birth_stage.BIRTH_ROLLER_CHOICE;

			        if (next != birth_stage.BIRTH_POINTBASED)
			            point_based_stop();

			        break;
			    }

			    case birth_stage.BIRTH_ROLLER:
			    {
			        roller = birth_stage.BIRTH_ROLLER;
			        next = roller_command(prev < birth_stage.BIRTH_ROLLER);
			        if (next == birth_stage.BIRTH_BACK)
			            next = birth_stage.BIRTH_ROLLER_CHOICE;

			        break;
			    }

			    case birth_stage.BIRTH_NAME_CHOICE:
			    {
			        if (prev < birth_stage.BIRTH_NAME_CHOICE)
			            Files.display_player(0);

			        next = get_name_command();
			        if (next == birth_stage.BIRTH_BACK)
			            next = roller;

			        break;
			    }

			    case birth_stage.BIRTH_FINAL_CONFIRM:
			    {
			        if (prev < birth_stage.BIRTH_FINAL_CONFIRM)
			            Files.display_player(0);

			        next = get_confirm_command();
			        if (next == birth_stage.BIRTH_BACK)
			            next = birth_stage.BIRTH_NAME_CHOICE;

			        break;
			    }
			}

			prev = current_stage;
			current_stage = next;

			return 0;
		}

		/*
		 * Called when we enter the birth mode - so we set up handlers, command hooks,
		 * etc, here.
		 */
		static void ui_enter_birthscreen(Game_Event.Event_Type type, Game_Event data, object user) {
			///* Set the ugly static global that tells us if quickstart's available. */
			quickstart_allowed = data.flag;

			setup_menus();
		}

		static void ui_leave_birthscreen(Game_Event.Event_Type type, Game_Event data, object user) {
			free_birth_menus();
		}

		public static void ui_init_birthstate_handlers() {
			Game_Event.add_handler(Game_Event.Event_Type.ENTER_BIRTH, ui_enter_birthscreen, null);
			Game_Event.add_handler(Game_Event.Event_Type.LEAVE_BIRTH, ui_leave_birthscreen, null);
		}
	}
}
