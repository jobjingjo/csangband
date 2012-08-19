using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Statusline {
		/* ------------------------------------------------------------------------
		 * Status line display functions
		 * ------------------------------------------------------------------------ */

		/* Simple macro to initialise structs */
		//#define S(s)		s, sizeof(s)

		/*
		 * Struct to describe different timed effects
		 */
		class state_info
		{
			public state_info(int a, string b, ConsoleColor d){
				value = a;
				str = b;
				attr = d;
			}
			public int value;
			public string str;
			public ConsoleColor attr;
		};

		/* TMD_CUT descriptions */
		static state_info[] cut_data =
		{
			new state_info( 1000, "Mortal wound", ConsoleColor.Red ),
			new state_info(  200, "Deep gash",    ConsoleColor.DarkRed ),
			new state_info(  100, "Severe cut",   ConsoleColor.DarkRed ),
			new state_info(   50, "Nasty cut",    ConsoleColor.DarkYellow ),
			new state_info(   25, "Bad cut",      ConsoleColor.DarkYellow ),
			new state_info(   10, "Light cut",    ConsoleColor.Yellow ),
			new state_info(    0, "Graze",        ConsoleColor.Yellow ),
		};

		/* TMD_STUN descriptions */
		static state_info[] stun_data =
		{
			new state_info(   100, "Knocked out", ConsoleColor.Red ),
			new state_info(    50, "Heavy stun",  ConsoleColor.DarkYellow ),
			new state_info(     0, "Stun",        ConsoleColor.DarkYellow ),
		};

		/* p_ptr.hunger descriptions */
		static state_info[] hunger_data =
		{
			new state_info( Misc.PY_FOOD_FAINT, "Faint",    ConsoleColor.Red ),
			new state_info( Misc.PY_FOOD_WEAK,  "Weak",     ConsoleColor.DarkYellow ),
			new state_info( Misc.PY_FOOD_ALERT, "Hungry",   ConsoleColor.Yellow ),
			new state_info( Misc.PY_FOOD_FULL,  "",         ConsoleColor.Green ),
			new state_info( Misc.PY_FOOD_MAX,   "Full",     ConsoleColor.Green ),
			new state_info( Misc.PY_FOOD_UPPER, "Gorged",   ConsoleColor.DarkGreen ),
		};

		/* For the various TMD_* effects */
		static state_info[] effects =
		{
			new state_info( (int)Timed_Effect.BLIND,     "Blind",      ConsoleColor.DarkYellow ),
			new state_info( (int)Timed_Effect.PARALYZED, "Paralyzed!", ConsoleColor.Red ),
			new state_info( (int)Timed_Effect.CONFUSED,  "Confused",   ConsoleColor.DarkYellow ),
			new state_info( (int)Timed_Effect.AFRAID,    "Afraid",     ConsoleColor.DarkYellow ),
			new state_info( (int)Timed_Effect.TERROR,    "Terror",     ConsoleColor.Red ),
			new state_info( (int)Timed_Effect.IMAGE,     "Halluc",     ConsoleColor.DarkYellow ),
			new state_info( (int)Timed_Effect.POISONED,  "Poisoned",   ConsoleColor.DarkYellow ),
			new state_info( (int)Timed_Effect.PROTEVIL,  "ProtEvil",   ConsoleColor.Green ),
			new state_info( (int)Timed_Effect.SPRINT,    "Sprint",     ConsoleColor.Green ),
			new state_info( (int)Timed_Effect.TELEPATHY, "ESP",        ConsoleColor.Cyan ),
			new state_info( (int)Timed_Effect.INVULN,    "Invuln",     ConsoleColor.Green ),
			new state_info( (int)Timed_Effect.HERO,      "Hero",       ConsoleColor.Green ),
			new state_info( (int)Timed_Effect.SHERO,     "Berserk",    ConsoleColor.Green ),
			new state_info( (int)Timed_Effect.BOLD,      "Bold",       ConsoleColor.Green ),
			new state_info( (int)Timed_Effect.STONESKIN, "Stone",      ConsoleColor.Green ),
			new state_info( (int)Timed_Effect.SHIELD,    "Shield",     ConsoleColor.Green ),
			new state_info( (int)Timed_Effect.BLESSED,   "Blssd",      ConsoleColor.Green ),
			new state_info( (int)Timed_Effect.SINVIS,    "SInvis",     ConsoleColor.Green ),
			new state_info( (int)Timed_Effect.SINFRA,    "Infra",      ConsoleColor.Green ),
			new state_info( (int)Timed_Effect.OPP_ACID,  "RAcid",      ConsoleColor.Gray ),
			new state_info( (int)Timed_Effect.OPP_ELEC,  "RElec",      ConsoleColor.Blue ),
			new state_info( (int)Timed_Effect.OPP_FIRE,  "RFire",      ConsoleColor.Red ),
			new state_info( (int)Timed_Effect.OPP_COLD,  "RCold",      ConsoleColor.White ),
			new state_info( (int)Timed_Effect.OPP_POIS,  "RPois",      ConsoleColor.DarkGreen ),
			new state_info( (int)Timed_Effect.OPP_CONF,  "RConf",      ConsoleColor.DarkMagenta ),
			new state_info( (int)Timed_Effect.AMNESIA,   "Amnesiac",   ConsoleColor.DarkYellow ),
		};

		static int PRINT_STATE(string sym, state_info[] data, int index, int row, int col)
		{
			for (int i = 0; i < data.Length; i++) 
			{
				bool val = false;
				if (sym == ">") val = index > data[i].value;
				if (sym == "<") val = index < data[i].value;

				if (val)
				{
					if (data[i].str.Length > 0)
					{
						Utilities.c_put_str(data[i].attr, data[i].str, row, col);
						return data[i].str.Length;
					}
					else
					{
						continue; //return 0; //Was originally return, if wonky things, revert
					}
				}
			}

			return 0;
		}


		/*
		 * Print recall status.
		 */
		static int prt_recall(int row, int col)
		{
			if (Misc.p_ptr.word_recall != 0)
			{
				Utilities.c_put_str(ConsoleColor.White, "Recall", row, col);
				return "Recall".Length;
			}

			return 0;
		}


		/*
		 * Print cut indicator.
		 */
		static int prt_cut(int row, int col)
		{
			PRINT_STATE(">", cut_data, Misc.p_ptr.timed[(int)Timed_Effect.CUT], row, col);
			return 0;
		}


		/*
		 * Print stun indicator.
		 */
		static int prt_stun(int row, int col)
		{
			PRINT_STATE(">", stun_data, Misc.p_ptr.timed[(int)Timed_Effect.STUN], row, col);
			return 0;
		}


		/*
		 * Prints status of hunger
		 */
		static int prt_hunger(int row, int col)
		{
			PRINT_STATE("<", hunger_data, Misc.p_ptr.food, row, col);
			return 0;
		}



		/*
		 * Prints Searching, Resting, or 'count' status
		 * Display is always exactly 10 characters wide (see below)
		 *
		 * This function was a major bottleneck when resting, so a lot of
		 * the text formatting code was optimized in place below.
		 */
		static int prt_state(int row, int col)
		{
			ConsoleColor attr = ConsoleColor.White;

			string text = ""; //16


			/* Resting */
			if (Misc.p_ptr.resting != 0)
			{
				int i;
				int n = Misc.p_ptr.resting;

				/* Start with "Rest" */
				text = "Rest ";

				/* Extensive (timed) rest */
				if (n >= 1000)
				{
					i = n / 100;
					
					if (i >= 10)
					{
						int q = i / 10;
						if (q >= 10)
						{
							text += Basic.I2D(q / 10);
							//text[5] = Basic.I2D(q / 10);
						}
						else {
							text += " ";
						}
						text += Basic.I2D(q % 10);
						
					} else {
						text += " ";
					}
					
					text += Basic.I2D(i % 10) + "00";
					//text[9] = '0';
					//text[8] = '0';
					//text[7] = I2D(i % 10);
				}

				/* Long (timed) rest */
				else if (n >= 100)
				{
					text += "  ";
					i = n;
					text += Basic.I2D(i / 100);
					text += Basic.I2D((i / 10) % 10);
					text += Basic.I2D(i % 10);
					//text[9] = Basic.I2D(i % 10);
					//i = i / 10;
					//text[8] = Basic.I2D(i % 10);
					//text[7] = Basic.I2D(i / 10);
				}

				/* Medium (timed) rest */
				else if (n >= 10)
				{
					text += "   ";
					i = n;
					text += Basic.I2D(i/10);
					text += Basic.I2D(i%10);
					//text[9] = I2D(i % 10);
					//text[8] = I2D(i / 10);
				}

				/* Short (timed) rest */
				else if (n > 0)
				{
					text += "    ";
					i = n;
					text += Basic.I2D(i);
					//text[9] = I2D(i);
				}

				/* Rest until healed */
				else if (n == -1)
				{
					text += "*****";
					//text[5] = text[6] = text[7] = text[8] = text[9] = '*';
				}

				/* Rest until done */
				else if (n == -2)
				{
					text += "&&&&&";
					//text[5] = text[6] = text[7] = text[8] = text[9] = '&';
				}
		
				/* Rest until HP or SP filled */
				else if (n == -3)
				{
					text += "!!!!!";
					//text[5] = text[6] = text[7] = text[8] = text[9] = '!';
				}

			}

			/* Repeating */
			else if (Game_Command.get_nrepeats() != 0)
			{
				int nrepeats = Game_Command.get_nrepeats();

				if (nrepeats > 999)
					text = "Rep. " + nrepeats / 100 + "00";
				else
					text = "Repeat " + nrepeats;
			}

			/* Searching */
			else if (Misc.p_ptr.searching != 0)
			{
				text = "Searching ";
			}

			/* Display the info (or blanks) */
			Utilities.c_put_str(attr, text, row, col);

			return text.Length;
		}


		/*
		 * Prints trap detection status
		 */
		static int prt_dtrap(int row, int col)
		{
			short info = Cave.cave.info2[Misc.p_ptr.py][Misc.p_ptr.px];
			/* The player is in a trap-detected grid */
			if ((info & (Cave.CAVE2_DTRAP)) != 0)
			{
			    /* The player is on the border */
			    if (Cave.dtrap_edge(Misc.p_ptr.py, Misc.p_ptr.px))
			        Utilities.c_put_str(ConsoleColor.Yellow, "DTrap", row, col);
			    else
			        Utilities.c_put_str(ConsoleColor.Green, "DTrap", row, col);

			    return 5;
			}

			return 0;
		}



		/*
		 * Print whether a character is studying or not.
		 */
		static int prt_study(int row, int col)
		{
			if (Misc.p_ptr.new_spells != 0)
			{
			    string text = "Study (" + Misc.p_ptr.new_spells.ToString() + ")";
			    Utilities.put_str(text, row, col);
			    return text.Length;
			}

			return 0;
		}



		/*
		 * Print all timed effects.
		 */
		static int prt_tmd(int row, int col)
		{
			int len = 0;

			for (int i = 0; i < effects.Length; i++)
			{
			    if (Misc.p_ptr.timed[effects[i].value] != 0)
			    {
			        Utilities.c_put_str(effects[i].attr, effects[i].str, row, col + len);
			        len += effects[i].str.Length;
			    }
			}

			return len;
		}

		/**
		 * Print "unignoring" status
		 */
		static int prt_unignore(int row, int col)
		{
			if (Misc.p_ptr.unignoring != 0) {
			    string str = "Unignoring";
			    Utilities.put_str(str, row, col);
			    return str.Length;
			}

			return 0;
		}

		/*
		 * Print mouse buttons
		 */
		static int prt_buttons(int row, int col)
		{
			if(Option.mouse_buttons.value)
				throw new NotFiniteNumberException();
			    //return button_print(row, col);

			return 0;
		}


		/* Useful typedef */
		delegate int status_f(int row, int col);

		static status_f[] status_handlers =
		{ prt_buttons, prt_unignore, prt_recall, prt_state, prt_cut, prt_stun,
		  prt_hunger, prt_study, prt_tmd, prt_dtrap };


		/*
		 * Print the status line.
		 */
		public static void update_statusline(Game_Event.Event_Type type, Game_Event data, object user)
		{
			int row = Term.instance.hgt - 1;
			int col = 13;
			int i;

			/* Clear the remainder of the line */
			Utilities.prt("", row, col);

			/* Display those which need redrawing */
			for (i = 0; i < status_handlers.Length; i++)
				col += status_handlers[i](row, col);
		}

	}
}
