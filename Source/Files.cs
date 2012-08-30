using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Player;
using CSAngband.Object;

namespace CSAngband {
	class Files {
		public static string savefile;

		static Region[] boundaries = new Region[]
		{
					   /* x   y     width, rows */
			new Region(   0,  0,		0,	  0 ),
			new Region(   1,  1,	   40,	  8 ), /* Name, Class, ... */
			new Region(   1,  9,	   22,	  9 ), /* Cur Exp, Max Exp, ... */
			new Region(   26, 9,	   17,	  9 ), /* AC, melee, ... */
			new Region(   48, 9,	   24,	  8 ), /* skills */
			new Region(   21, 2,	   18,	  5 )  /* Age, ht, wt, ... */
		};
		/*
		 * Process the player name and extract a clean "base name".
		 *
		 * If "sf" is true, then we initialize "savefile" based on player name.
		 *
		 * Some platforms (Windows, Macintosh, Amiga) leave the "savefile" empty
		 * when a new character is created, and then when the character is done
		 * being created, they call this function to choose a new savefile name.
		 *
		 * This also now handles the turning on and off of the automatic
		 * sequential numbering of character names with Roman numerals.  
		 */
		public static void process_player_name(bool sf) {
			/* Process the player name */
			//Nick: Let's just assume it's acceptable
			//for (int i = 0; i < Player.Player_Other.instance.full_name.Length; i++)
			//{
			//    char c = Player.Player_Other.instance.full_name[i];

			//    /* No control characters */
			//    if (iscntrl((unsigned char)c))
			//    {
			//        /* Illegal characters */
			//        quit_fmt("Illegal control char (0x%02X) in player name", c);
			//    }

			//    /* Convert all non-alphanumeric symbols */
			//    if (!isalpha((unsigned char)c) && !isdigit((unsigned char)c)) c = '_';

			//    /* Build "base_name" */
			//    op_ptr.base_name[i] = c;
			//}

			//#if defined(WINDOWS)

			//    /* Max length */
			//    if (i > 8) i = 8;

			//#endif

			/* Terminate */
			//op_ptr.base_name[i] = '\0';

			/* Require a "base" name */
			if(Player_Other.instance.base_name == null || Player_Other.instance.base_name.Length == 0) {
				Player_Other.instance.base_name = "PLAYER";
			}


			/* Pick savefile name if needed */
			if(sf) {
				//char temp[128];

				//#if defined(SET_UID)
				//        /* Rename the savefile, using the player_uid and base_name */
				//        strnfmt(temp, sizeof(temp), "%d.%s", player_uid, op_ptr.base_name);
				//#else
				/* Rename the savefile, using the base name */
				//strnfmt(temp, sizeof(temp), "%s", op_ptr.base_name);
				//#endif

				/* Build the filename */
				savefile = Misc.path_build(Misc.ANGBAND_DIR_SAVE, Player_Other.instance.base_name);
			}
		}

		/*
		 * Display the character on the screen (two different modes)
		 *
		 * The top two lines, and the bottom line (or two) are left blank.
		 *
		 * Mode 0 = standard display with skills/history
		 * Mode 1 = special display with equipment flags
		 */
		public static void display_player(int mode) {
			/* Erase screen */
			Utilities.clear_from(0);

			/* When not playing, do not display in subwindows */
			if (Term.instance != Misc.angband_term[0] && !Player.Player.instance.playing) return;

			/* Stat info */
			display_player_stat_info();

			if (mode != 0)
			{
			    Data_Panel[] data = new Data_Panel[Misc.MAX_PANEL];
			    int rows = get_panel(1, ref data);

			    display_panel(data, rows, true, boundaries[1]);

			    /* Stat/Sustain flags */
			    display_player_sust_info();

			    /* Other flags */
			    display_player_flag_info();
			}

			/* Standard */
			else
			{
			    /* Extra info */
			    display_player_xtra_info();
			}
		}

		public static void display_player_xtra_info() {
			int[] panels = new int[] { 1, 2, 3, 4, 5 };
			bool[] left_adj = new bool[] { true, false, false, false, false };
			Data_Panel[] data = new Data_Panel[Misc.MAX_PANEL];
			for(int i = 0; i < Misc.MAX_PANEL; i++) {
				data[i] = new Data_Panel();
			}

			for(int i = 0; i < panels.Length; i++) {
				int oid = panels[i];
				int rows = get_panel(oid, ref data);

				/* Hack:  Don't show 'Level' in the name, class ...  panel */
				if(oid == 1)
					rows -= 1;

				display_panel(data, rows, left_adj[i], boundaries[oid]);
			}

			/* Indent output by 1 character, and wrap at column 72 */
			Misc.text_out_wrap = 72;
			Misc.text_out_indent = 1;

			/* History */
			Term.gotoxy(Misc.text_out_indent, 19);
			Utilities.text_out_to_screen(ConsoleColor.White, Player.Player.instance.history);

			/* Reset text_out() vars */
			Misc.text_out_wrap = 0;
			Misc.text_out_indent = 0;

			return;
		}

		/*
		 * Special display, part 2b
		 */
		public static void display_player_stat_info() {
			/* Row */
			int row = 2;

			/* Column */
			int col = 42;

			/* Player Pointer */
			Player.Player p_ptr = Player.Player.instance;

			/* Print out the labels for the columns */
			Utilities.c_put_str(ConsoleColor.White, "  Self", row - 1, col + 5);
			Utilities.c_put_str(ConsoleColor.White, " RB", row - 1, col + 12);
			Utilities.c_put_str(ConsoleColor.White, " CB", row - 1, col + 16);
			Utilities.c_put_str(ConsoleColor.White, " EB", row - 1, col + 20);
			Utilities.c_put_str(ConsoleColor.White, "  Best", row - 1, col + 24);

			/* Display the stats */
			for(int i = 0; i < (int)Stat.Max; i++) {
				/* Reduced */
				if(p_ptr.stat_cur[i] < p_ptr.stat_max[i]) {
					/* Use lowercase stat name */
					Utilities.put_str(Stat_Names.Reduced[i], row + i, col);
				}

				/* Normal */
				else {
					/* Assume uppercase stat name */
					Utilities.put_str(Stat_Names.Normal[i], row + i, col);
				}

				/* Indicate natural maximum */
				if(p_ptr.stat_max[i] == 18 + 100) {
					Utilities.put_str("!", row + i, col + 3);
				}

				string buf;

				/* Internal "natural" maximum value */
				cnv_stat(p_ptr.stat_max[i], out buf);
				Utilities.c_put_str(ConsoleColor.Green, buf, row + i, col + 5);

				/* Race Bonus */
				buf = p_ptr.Race.r_adj[i].ToString().PadLeft(3, ' ');
				Utilities.c_put_str(ConsoleColor.Cyan, buf, row + i, col + 12);

				/* Class Bonus */
				buf = p_ptr.Class.c_adj[i].ToString().PadLeft(3, ' ');
				Utilities.c_put_str(ConsoleColor.Cyan, buf, row + i, col + 16);

				/* Equipment Bonus */
				buf = p_ptr.state.stat_add[i].ToString().PadLeft(3, ' ');
				Utilities.c_put_str(ConsoleColor.Cyan, buf, row + i, col + 20);

				/* Resulting "modified" maximum value */
				cnv_stat(p_ptr.state.stat_top[i], out buf);
				Utilities.c_put_str(ConsoleColor.Green, buf, row + i, col + 24);

				/* Only display stat_use if there has been draining */
				if(p_ptr.stat_cur[i] < p_ptr.stat_max[i]) {
					cnv_stat(p_ptr.state.stat_use[i], out buf);
					Utilities.c_put_str(ConsoleColor.Yellow, buf, row + i, col + 31);
				}
			}
		}

		/*
		 * Converts stat num into a six-char (right justified) string
		 */
		public static void cnv_stat(int val, out string out_val) {
			/* Above 18 */
			if(val > 18) {
				int bonus = (val - 18);

				if(bonus >= 220)
					out_val = "18/***";
				else if(bonus >= 100)
					out_val = "18/" + bonus.ToString(); //"%03"
				else if(bonus >= 10)
					out_val = "18/0" + bonus.ToString(); //%02
				else
					out_val = "18/00" + bonus.ToString();
			}

			/* From 3 to 18 */
			else {
				if(val >= 10)
					out_val = "    " + val.ToString();
				else
					out_val = "     " + val.ToString();
			}
		}

		/* data_panel array element initializer, for ansi compliance */
		static void P_I(ConsoleColor col, string lab, string format, Type_Union val1,
			Type_Union val2, Data_Panel[] panel, ref int i) {
			panel[i].color = col;
			panel[i].label = lab;
			panel[i].fmt = format;
			panel[i].value[0] = val1;
			panel[i].value[1] = val2;
			i++;
		}

		static string show_title() {
			if(Player.Player.instance.wizard)
				return "[=-WIZARD-=]";
			else if(Player.Player.instance.total_winner != 0 || Player.Player.instance.lev > Misc.PY_MAX_LEVEL)
				return "***WINNER***";
			else
				return Player.Player.instance.Class.title[(Player.Player.instance.lev - 1) / 5];
		}

		static ConsoleColor max_color(int val, int max)
		{
			return val < max ? ConsoleColor.Yellow : ConsoleColor.Green;
		}

		static string show_adv_exp()
		{
			Player.Player p_ptr = Player.Player.instance;
			if (p_ptr.lev < Misc.PY_MAX_LEVEL)
			{
				long advance = (Player.Player.player_exp[p_ptr.lev - 1] * p_ptr.expfact / 100L);
				return advance.ToString();
			}
			else {
				return "********";
			}
		}

		static string show_depth()
		{
			Player.Player p_ptr = Player.Player.instance;

			if (p_ptr.max_depth == 0) return "Town";

			return (p_ptr.max_depth * 50) + "' (" + p_ptr.max_depth + ")";
		}

		static string show_speed()
		{
			Player.Player p_ptr = Player.Player.instance;
			int tmp = p_ptr.state.speed;
			if (p_ptr.timed[(int)Timed_Effect.FAST] > 0) tmp -= 10;
			if (p_ptr.timed[(int)Timed_Effect.SLOW] > 0) tmp += 10;
			if (p_ptr.searching != 0) tmp += 10;
			if (tmp == 110) return "Normal";
			return "" + (tmp - 110);
		}

		static string show_melee_weapon(Object.Object o_ptr)
		{
			Player.Player p_ptr = Player.Player.instance;
			int hit = p_ptr.state.dis_to_h;
			int dam = p_ptr.state.dis_to_d;

			if (o_ptr.attack_plusses_are_visible())
			{
				hit += o_ptr.to_h;
				dam += o_ptr.to_d;
			}

			return "(" + hit + "," + dam + ")";
		}

		static string show_missile_weapon(Object.Object o_ptr)
		{
			Player.Player p_ptr = Player.Player.instance;
			int hit = p_ptr.state.dis_to_h;
			int dam = 0;

			if (o_ptr.attack_plusses_are_visible())
			{
				hit += o_ptr.to_h;
				dam += o_ptr.to_d;
			}
			return "(" + hit + "," + dam + ")";;
		}

		class temp_skills {
			public temp_skills(string n, Skill s, int d) {
				name = n;
				skill = s;
				div = d;
			}

			public string name;
			public Skill skill;
			public int div;
		};

		/* colours for table items */
		static ConsoleColor[] colour_table = new ConsoleColor[]
		{
			ConsoleColor.Red, ConsoleColor.Red, ConsoleColor.Red, ConsoleColor.Red, ConsoleColor.Magenta,
			ConsoleColor.Yellow, ConsoleColor.Yellow, ConsoleColor.DarkGreen, ConsoleColor.DarkGreen, ConsoleColor.Green,
			ConsoleColor.Cyan
		};

		static string show_status()
		{
			Player.Player p_ptr = Player.Player.instance;
			int sc = p_ptr.sc;
			sc /= 10;

			switch (sc)
			{
				case 0:
				case 1:
					return "Pariah";

				case 2:
					return "Outcast";

				case 3:
				case 4:
					return "Unknown";

				case 5:
					return "Known";

				case 6:
				/* Maximum status by birth 75 = 7 */
				case 7:
					return "Liked";

				case 8:
					return "Well-liked";

				case 9:
				case 10:
					return "Respected";

				case 11:
				case 12:
					return "Role model";

				case 13:
					return "Feared";

				case 14:
				case 15:
					return "Lordly";
			}

			return sc.ToString();
			;
		}

		/*
		 * Returns a "rating" of x depending on y, and sets "attr" to the
		 * corresponding "attribute".
		 */
		static string likert(int x, int y, ref ConsoleColor attr)
		{
			/* Paranoia */
			if (y <= 0) y = 1;

			/* Negative value */
			if (x < 0)
			{
				attr = ConsoleColor.Red;
				return ("Very Bad");
			}

			/* Analyze the value */
			switch ((x / y))
			{
				case 0:
				case 1:
				{
					attr = ConsoleColor.Red;
					return ("Bad");
				}
				case 2:
				{
					attr = ConsoleColor.Red;
					return ("Poor");
				}
				case 3:
				case 4:
				{
					attr = ConsoleColor.Yellow;
					return ("Fair");
				}
				case 5:
				{
					attr = ConsoleColor.Yellow;
					return ("Good");
				}
				case 6:
				{
					attr = ConsoleColor.Yellow;
					return ("Very Good");
				}
				case 7:
				case 8:
				{
					attr = ConsoleColor.Green;
					return ("Excellent");
				}
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				{
					attr = ConsoleColor.Green;
					return ("Superb");
				}
				case 14:
				case 15:
				case 16:
				case 17:
				{
					attr = ConsoleColor.Green;
					return ("Heroic");
				}
				default:
				{
					attr = ConsoleColor.Green;
					return ("Legendary");
				}
			}
		}

		//size = panel.Length
		static int get_panel(int oid, ref Data_Panel[] panel) {
			int size = panel.Length;
			int ret = panel.Length;
			Type_Union END = new Type_Union();
			Player.Player p_ptr = Player.Player.instance;
			Player_Other op_ptr = Player_Other.instance;
			switch(oid) {
				case 1: {
						int i = 0;
						Misc.assert(size >= (uint)boundaries[1].page_rows);
						ret = boundaries[1].page_rows;
						P_I(ConsoleColor.Cyan, "Name", "{0}", new Type_Union(op_ptr.full_name), END, panel, ref i);
						P_I(ConsoleColor.Cyan, "Sex", "{0}", new Type_Union(p_ptr.Sex.Title), END, panel, ref i);
						P_I(ConsoleColor.Cyan, "Race", "{0}", new Type_Union(p_ptr.Race.Name), END, panel, ref i);
						P_I(ConsoleColor.Cyan, "Class", "{0}", new Type_Union(p_ptr.Class.Name), END, panel, ref i);
						P_I(ConsoleColor.Cyan, "Title", "{0}", new Type_Union(show_title()), END, panel, ref i);
						P_I(ConsoleColor.Cyan, "HP", "{0}/{1}", new Type_Union(p_ptr.chp), new Type_Union(p_ptr.mhp), panel, ref i);
						P_I(ConsoleColor.Cyan, "SP", "{0}/{1}", new Type_Union(p_ptr.csp), new Type_Union(p_ptr.msp), panel, ref i);
						P_I(ConsoleColor.Cyan, "Level", "{0}", new Type_Union(p_ptr.lev), END, panel, ref i);
						Misc.assert(i == boundaries[1].page_rows);
						return ret;
					}
				case 2: {
						int i = 0;
						Misc.assert(ret >= boundaries[2].page_rows);
						ret = boundaries[2].page_rows;
						P_I(max_color(p_ptr.lev, p_ptr.max_lev), "Level", "{0}", new Type_Union(p_ptr.lev), END, panel, ref i);
						P_I(max_color(p_ptr.exp, p_ptr.max_exp), "Cur Exp", "{0}", new Type_Union(p_ptr.exp), END, panel, ref i);
						P_I(ConsoleColor.Green, "Max Exp", "{0}", new Type_Union(p_ptr.max_exp), END, panel, ref i);
						P_I(ConsoleColor.Green, "Adv Exp", "{0}", new Type_Union(show_adv_exp()), END, panel, ref i);
						P_I(ConsoleColor.Green, "MaxDepth", "{0}", new Type_Union(show_depth()), END, panel, ref i);
						P_I(ConsoleColor.Green, "Game Turns", "{0}", new Type_Union(Misc.turn), END, panel, ref i);
						P_I(ConsoleColor.Green, "Standard Turns", "{0}", new Type_Union((int)(p_ptr.total_energy / 100)), END, panel, ref i);
						P_I(ConsoleColor.Green, "Resting Turns", "{0}", new Type_Union((int)(p_ptr.resting_turn)), END, panel, ref i);
						P_I(ConsoleColor.Green, "Gold", "{0}", new Type_Union(p_ptr.au), END, panel, ref i);
						Misc.assert(i == boundaries[2].page_rows);
						return ret;
					}
				case 3: {
						int i = 0;
						Misc.assert(ret >= boundaries[3].page_rows);
						ret = boundaries[3].page_rows;
						P_I(ConsoleColor.Cyan, "Armor", "[{0},%+y]",	new Type_Union(p_ptr.state.dis_ac), 
							new Type_Union(p_ptr.state.dis_to_a)  , panel, ref i);
						P_I(ConsoleColor.Cyan, "Fight", "(%+y,%+y)",	new Type_Union(p_ptr.state.dis_to_h), 
							new Type_Union(p_ptr.state.dis_to_d)  , panel, ref i);
						P_I(ConsoleColor.Cyan, "Melee", "{0}",		new Type_Union(show_melee_weapon(p_ptr.inventory[Misc.INVEN_WIELD])), END  , panel, ref i);
						P_I(ConsoleColor.Cyan, "Shoot", "{0}",		new Type_Union(show_missile_weapon(p_ptr.inventory[Misc.INVEN_BOW])), END , panel, ref i );
						P_I(ConsoleColor.Cyan, "Blows", "{0}.%y/turn",	new Type_Union(p_ptr.state.num_blows / 100), 
							new Type_Union((p_ptr.state.num_blows / 10) % 10) , panel, ref i);
						P_I(ConsoleColor.Cyan, "Shots", "{0}/turn",	new Type_Union(p_ptr.state.num_shots), END, panel, ref i );
						P_I(ConsoleColor.Cyan, "Infra", "{0} ft",	new Type_Union(p_ptr.state.see_infra * 10), END , panel, ref i );
						P_I(ConsoleColor.Cyan, "Speed", "{0}",		new Type_Union(show_speed()), END, panel, ref i );
						P_I(ConsoleColor.Cyan, "Burden","%.1y lbs",	new Type_Union(p_ptr.total_weight/10.0f), END, panel, ref i  );
						Misc.assert(i == boundaries[3].page_rows);
						return ret;
					}
				case 4: {
						
					
						temp_skills[] skills =
						{
						    new temp_skills( "Saving Throw", Skill.SAVE, 6 ),
						    new temp_skills( "Stealth", Skill.STEALTH, 1 ),
						    new temp_skills( "Fighting", Skill.TO_HIT_MELEE, 12 ),
						    new temp_skills( "Shooting", Skill.TO_HIT_BOW, 12 ),
						    new temp_skills( "Disarming", Skill.DISARM, 8 ),
						    new temp_skills( "Magic Device", Skill.DEVICE, 6 ),
						    new temp_skills( "Perception", Skill.SEARCH_FREQUENCY, 6 ),
						    new temp_skills( "Searching", Skill.SEARCH, 6 )
						};
						int i;
						Misc.assert(skills.Length == boundaries[4].page_rows);
						ret = skills.Length;
						if (ret > size) ret = size;
						for (i = 0; i < ret; i++)
						{
						    short skill = p_ptr.state.skills[(int)skills[i].skill];
						    panel[i].color = ConsoleColor.Cyan;
						    panel[i].label = skills[i].name;
						    if (skills[i].skill == Skill.SAVE || skills[i].skill == Skill.SEARCH)
						    {
						        if (skill < 0) skill = 0;
						        if (skill > 100) skill = 100;
						        panel[i].fmt = "{0}%";
						        panel[i].value[0] = new Type_Union(skill);
						        panel[i].color = colour_table[skill / 10];
						    }
						    else if (skills[i].skill == Skill.DEVICE)
						    {
						        panel[i].fmt = "{0}";
						        panel[i].value[0] = new Type_Union(skill);
						        panel[i].color = colour_table[skill / 13];
						    }
						    else if (skills[i].skill == Skill.SEARCH_FREQUENCY)
						    {
						        if (skill <= 0) skill = 1;
						        if (skill >= 50)
						        {
						            panel[i].fmt = "1 in 1";
						            panel[i].color = colour_table[10];
						        }
						        else
						        {
						            /* convert to % chance of searching */
						            skill = (short)(50 - skill);
						            panel[i].fmt = "1 in {0}";
						            panel[i].value[0] = new Type_Union(skill);
						            panel[i].color = colour_table[(100 - skill*2) / 10];
						        }
						    }
						    else if (skills[i].skill == Skill.DISARM)
						    {
						        /* assume disarming a dungeon trap */
						        skill -= 5;
						        if (skill > 100) skill = 100;
						        if (skill < 2) skill = 2;
						        panel[i].fmt = "{0}%";
						        panel[i].value[0] = new Type_Union(skill);
						        panel[i].color = colour_table[skill / 10];
						    }
						    else
						    {
						        panel[i].fmt = "{0}";
								//last argument for likert was "panel[i].color"...
								ConsoleColor c = ConsoleColor.DarkMagenta; //Random color...
						        panel[i].value[0] = new Type_Union(likert(skill, skills[i].div, ref c));
						    }
						}
						return ret;
					}
				case 5: {
						    int i = 0;
						    Misc.assert(ret >= boundaries[5].page_rows);
						    ret = boundaries[5].page_rows;
						    P_I(ConsoleColor.Cyan, "Age",			"{0}",	new Type_Union(p_ptr.age), END , panel, ref i  );
						    P_I(ConsoleColor.Cyan, "Height",		"{0}",	new Type_Union(p_ptr.ht), END  , panel, ref i  );
						    P_I(ConsoleColor.Cyan, "Weight",		"{0}",	new Type_Union(p_ptr.wt), END  , panel, ref i  );
						    P_I(ConsoleColor.Cyan, "Social",		"{0}",	new Type_Union(show_status()), END  , panel, ref i  );
						    P_I(ConsoleColor.Cyan, "Maximize",	"{0}",	new Type_Union(Option.birth_maximize.value ? 'Y' : 'N'), END, panel, ref i  );
							//#if 0
							//    /* Preserve mode deleted */
							//    P_I(ConsoleColor.Cyan, "Preserve",	"{0}",	c2u(birth_preserve ? 'Y' : 'N'), END);
							//#endif
						    Misc.assert(i == boundaries[5].page_rows);
						    return ret;
					}
			}
			/* hopefully not reached */
			return 0;
		}

		static void display_panel(Data_Panel[] panel, int count, bool left_adj, Region bounds) {
			int i;
			string buffer;
			int col = bounds.col;
			int row = bounds.row;
			int w = bounds.width;
			int offset = 0;

			bounds.erase();

			if (left_adj)
			{
			    for (i = 0; i < count; i++)
			    {
			        int len = panel[i].label.Length;
			        if (offset < len) offset = len;
			    }
			    offset += 2;
			}

			for (i = 0; i < count; i++, row++)
			{
			    int len;
			    if (panel[i].label.Length > 0) continue;
			    Term.putstr(col, row, panel[i].label.Length, ConsoleColor.White, panel[i].label);

				buffer = String.Format(panel[i].fmt, panel[i].value[0].value, panel[i].value[1].value);

			    len = buffer.Length;
			    len = len < w - offset ? len : w - offset - 1;
			    if (left_adj)
			        Term.putstr(col+offset, row, len, panel[i].color, buffer);
			    else
			        Term.putstr(col+w-len, row, len, panel[i].color, buffer);
			}
		}

		static void display_player_flag_info()
		{
			int i;
			for (i = 0; i < 4; i++)
			{
				//was previously:
				//display_resistance_panel(player_flag_table+i*RES_ROWS), RES_ROWS+3, &resist_region[i]);
				display_resistance_panel(player_flag_table[i], RES_ROWS, resist_region[i]);
			}
		}


		/*
		 * Special display, part 2c
		 *
		 * How to print out the modifications and sustains.
		 * Positive mods with no sustain will be light green.
		 * Positive mods with a sustain will be dark green.
		 * Sustains (with no modification) will be a dark green 's'.
		 * Negative mods (from a curse) will be red.
		 * Huge mods (>9), like from MICoMorgoth, will be a '*'
		 * No mod, no sustain, will be a slate '.'
		 */
		static void display_player_sust_info()
		{
			int j, stat;

			Player.Player p_ptr = Player.Player.instance;

			Bitflag f = new Bitflag(Object_Flag.SIZE);

			Object_Flag[] stat_flags = new Object_Flag[(int)Stat.Max];
			Object_Flag[] sustain_flags = new Object_Flag[(int)Stat.Max];

			ConsoleColor a;
			char c;


			/* Row */
			int row = 2;

			/* Column */
			int col = 26;

			/* Build the stat flags tables */
			stat_flags[(int)Stat.Str] = Object_Flag.STR;
			stat_flags[(int)Stat.Int] = Object_Flag.INT;
			stat_flags[(int)Stat.Wis] = Object_Flag.WIS;
			stat_flags[(int)Stat.Dex] = Object_Flag.DEX;
			stat_flags[(int)Stat.Con] = Object_Flag.CON;
			stat_flags[(int)Stat.Chr] = Object_Flag.CHR;
			sustain_flags[(int)Stat.Str] = Object_Flag.SUST_STR;
			sustain_flags[(int)Stat.Int] = Object_Flag.SUST_INT;
			sustain_flags[(int)Stat.Wis] = Object_Flag.SUST_WIS;
			sustain_flags[(int)Stat.Dex] = Object_Flag.SUST_DEX;
			sustain_flags[(int)Stat.Con] = Object_Flag.SUST_CON;
			sustain_flags[(int)Stat.Chr] = Object_Flag.SUST_CHR;

			/* Header */
			Utilities.c_put_str(ConsoleColor.White, "abcdefghijkl@", row-1, col);

			/* Process equipment */
			for (int i = Misc.INVEN_WIELD; i < Misc.INVEN_TOTAL; ++i)
			{
				/* Get the object */
				Object.Object o_ptr = p_ptr.inventory[i];

				if (o_ptr.kind == null) {
					col++;
					continue;
				}

				/* Get the "known" flags */
				o_ptr.object_flags_known(ref f);

				/* Initialize color based of sign of pval. */
				for (stat = 0; stat < (int)Stat.Max; stat++)
				{
					/* Default */
					a = ConsoleColor.Gray;
					c = '.';

					/* Boost */
					if (f.has(stat_flags[stat].value))
					{
						/* Default */
						c = '*';

						/* Work out which pval we're talking about */
						j = o_ptr.which_pval(stat_flags[stat].value);

						/* Good */
						if (o_ptr.pval[j] > 0)
						{
							/* Good */
							a = ConsoleColor.Green;

							/* Label boost */
							if (o_ptr.pval[j] < 10)
								c = (char)Basic.I2D((char)o_ptr.pval[j]);
						}

						/* Bad */
						if (o_ptr.pval[j] < 0)
						{
							/* Bad */
							a = ConsoleColor.Red;

							/* Label boost */
							if (o_ptr.pval[j] > -10)
								c = (char)Basic.I2D((char)-(o_ptr.pval[j]));
						}
					}

					/* Sustain */
					if (f.has(sustain_flags[stat].value))
					{
						/* Dark green */
						a = ConsoleColor.DarkGreen;

						/* Convert '.' to 's' */
						if (c == '.') c = 's';
					}

					if ((c == '.') && o_ptr.kind != null && !o_ptr.object_flag_is_known(sustain_flags[stat].value))
						c = '?';

					/* Dump proper character */
					Term.putch(col, row+stat, a, c);
				}

				/* Advance */
				col++;
			}

			/* Player flags */
			Player.Player.player_flags(ref f);

			/* Check stats */
			for (stat = 0; stat < (int)Stat.Max; ++stat)
			{
				/* Default */
				a = ConsoleColor.Gray;
				c = '.';

				/* Sustain */
				if (f.has(sustain_flags[stat].value))
				{
					/* Dark green "s" */
					a = ConsoleColor.DarkGreen;
					c = 's';
				}

				/* Dump */
				Term.putch(col, row+stat, a, c);
			}

			/* Column */
			col = 26;

			/* Footer */
			Utilities.c_put_str(ConsoleColor.White, "abcdefghijkl@", row+6, col);

			/* Equippy */
			display_player_equippy(row+7, col);
		}

		/*
		 * Equippy chars
		 */
		static void display_player_equippy(int y, int x)
		{
			Player.Player p_ptr = Player.Player.instance;

			ConsoleColor a;
			char c;

			Object.Object o_ptr;


			/* Dump equippy chars */
			for (int i = Misc.INVEN_WIELD; i < Misc.INVEN_TOTAL; ++i)
			{
				/* Object */
				o_ptr = p_ptr.inventory[i];

				/* Skip empty objects */
				if (o_ptr.kind == null) continue;

				/* Get attr/char for display */
				a = o_ptr.object_attr();
				c = o_ptr.object_char();

				/* Dump */
				if ((Term.tile_width == 1) && (Term.tile_height == 1))
				{
					Term.putch(x+i-Misc.INVEN_WIELD, y, a, c);
				}
			}
		}


		/*
		 * List of resistances and abilities to display
		 */
		public const int RES_ROWS = 9;
		class player_flag_record
		{
			public player_flag_record(string n, Object_Flag r, Object_Flag i, Object_Flag v){
				name = n;
				res_flag = r;
				im_flag = i;
				vuln_flag = v;
			}

			public string name;		/* Name of resistance/ability */
			public Object_Flag res_flag;			/* resistance flag bit */
			public Object_Flag im_flag;			/* corresponding immunity bit, if any */
			public Object_Flag vuln_flag;			/* corresponding vulnerability flag, if any */
		};

		static player_flag_record[][] player_flag_table = new player_flag_record[4][]
		{
			new player_flag_record[RES_ROWS]
			{new player_flag_record( "rAcid",	Object_Flag.RES_ACID,    Object_Flag.IM_ACID, Object_Flag.VULN_ACID ),
			new player_flag_record( "rElec",	Object_Flag.RES_ELEC,    Object_Flag.IM_ELEC, Object_Flag.VULN_ELEC ),
			new player_flag_record( "rFire",	Object_Flag.RES_FIRE,    Object_Flag.IM_FIRE, Object_Flag.VULN_FIRE ),
			new player_flag_record( "rCold",	Object_Flag.RES_COLD,    Object_Flag.IM_COLD, Object_Flag.VULN_COLD ),
			new player_flag_record( "rPois",	Object_Flag.RES_POIS,    Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "rLite",	Object_Flag.RES_LIGHT,   Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "rDark",	Object_Flag.RES_DARK,    Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "Sound",	Object_Flag.RES_SOUND,   Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "Shard",	Object_Flag.RES_SHARD,   Object_Flag.NONE,   Object_Flag.NONE )},
			new player_flag_record[RES_ROWS]
			{new player_flag_record( "Nexus",	Object_Flag.RES_NEXUS,   Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "Nethr",	Object_Flag.RES_NETHR,   Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "Chaos",	Object_Flag.RES_CHAOS,   Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "Disen",	Object_Flag.RES_DISEN,   Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "Feath",	Object_Flag.FEATHER,     Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "pFear",	Object_Flag.RES_FEAR,    Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "pBlnd",	Object_Flag.RES_BLIND,   Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "pConf",	Object_Flag.RES_CONFU,   Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "pStun",	Object_Flag.RES_STUN,	 Object_Flag.NONE,   Object_Flag.NONE )},
			new player_flag_record[RES_ROWS]
			{new player_flag_record( "Light",	Object_Flag.LIGHT,       Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "Regen",	Object_Flag.REGEN,       Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "  ESP",	Object_Flag.TELEPATHY,   Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "Invis",	Object_Flag.SEE_INVIS,   Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "FrAct",	Object_Flag.FREE_ACT,    Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "HLife",	Object_Flag.HOLD_LIFE,   Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "Stea.",	Object_Flag.STEALTH,     Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "Sear.",	Object_Flag.SEARCH,      Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "Infra",	Object_Flag.INFRA,       Object_Flag.NONE,   Object_Flag.NONE )},
			new player_flag_record[RES_ROWS]
			{new player_flag_record( "Tunn.",	Object_Flag.TUNNEL,      Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "Speed",	Object_Flag.SPEED,       Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "Blows",	Object_Flag.BLOWS,       Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "Shots",	Object_Flag.SHOTS,       Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "Might",	Object_Flag.MIGHT,       Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "S.Dig",	Object_Flag.SLOW_DIGEST, Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "ImpHP",	Object_Flag.IMPAIR_HP,   Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( " Fear",	Object_Flag.AFRAID,      Object_Flag.NONE,   Object_Flag.NONE ),
			new player_flag_record( "Aggrv",	Object_Flag.AGGRAVATE,   Object_Flag.NONE,   Object_Flag.NONE )}
		};

		const int RES_COLS = (5 + 2 + Misc.INVEN_TOTAL - Misc.INVEN_WIELD);
		static Region[] resist_region = new Region[] {
			new Region( 0*(RES_COLS+1), 10, RES_COLS, RES_ROWS+2 ),
			new Region( 1*(RES_COLS+1), 10, RES_COLS, RES_ROWS+2 ),
			new Region( 2*(RES_COLS+1), 10, RES_COLS, RES_ROWS+2 ),
			new Region( 3*(RES_COLS+1), 10, RES_COLS, RES_ROWS+2 )
		};

		static void display_resistance_panel(player_flag_record[] resists, int size, Region bounds) 
		{
			Player.Player p_ptr = Player.Player.instance;
			int col = bounds.col;
			int row = bounds.row;
			Term.putstr(col, row++, RES_COLS, ConsoleColor.White, "      abcdefghijkl@");
			for (int i = 0; i < size-3; i++, row++)
			{
				ConsoleColor name_attr = ConsoleColor.White;
				Term.gotoxy(col+6, row);
				/* repeated extraction of flags is inefficient but more natural */
				for (int j = Misc.INVEN_WIELD; j <= Misc.INVEN_TOTAL; j++)
				{
					Object.Object o_ptr = p_ptr.inventory[j];
					Bitflag f = new Bitflag(Object_Flag.SIZE);

					ConsoleColor[] alternatingcols = new ConsoleColor[]{ ConsoleColor.Gray, ConsoleColor.DarkGray };
					ConsoleColor attr = alternatingcols[j % 2]; /* alternating columns */
					char sym = '.';

					bool res, imm, vuln;

					/* Wipe flagset */
					f.wipe();

					if (j < Misc.INVEN_TOTAL && o_ptr.kind != null)
					{
						o_ptr.object_flags_known(ref f);
					}
					else if (j == Misc.INVEN_TOTAL)
					{
						Player.Player.player_flags(ref f);

						/* If the race has innate infravision/digging, force the corresponding flag
						   here.  If we set it in player_flags(), then all callers of that
						   function will think the infravision is caused by equipment. */
						if (p_ptr.Race.infra > 0)
							f.on(Object_Flag.INFRA.value);
						if (p_ptr.Race.r_skills[(int)Skill.DIGGING] > 0)
							f.on(Object_Flag.TUNNEL.value);
					}

					res = f.has(resists[i].res_flag.value);
					imm = f.has(resists[i].im_flag.value);
					vuln = f.has(resists[i].vuln_flag.value);

					if (imm) name_attr = ConsoleColor.DarkGreen;
					else if (res && name_attr == ConsoleColor.White) name_attr = ConsoleColor.Cyan;

					if (vuln) sym = '-';
					else if (imm) sym = '*';
					else if (res) sym = '+';
					else if ((j < Misc.INVEN_TOTAL) && o_ptr.kind != null && 
						!o_ptr.object_flag_is_known(resists[i].res_flag.value)) sym = '?';
					Term.addch(attr, sym);
				}
				Term.putstr(col, row, 6, name_attr, resists[i].name.ToString());
			}
			Term.putstr(col, row++, RES_COLS, ConsoleColor.White, "      abcdefghijkl@");
			/* Equippy */
			display_player_equippy(row++, col+6);
		}

		/*
		 * Close up the current game (player may or may not be dead)
		 *
		 * Note that the savefile is not saved until the tombstone is
		 * actually displayed and the player has a chance to examine
		 * the inventory and such.  This allows cheating if the game
		 * is equipped with a "quit without save" method.  XXX XXX XXX
		 */
		public static void close_game()
		{
			/* Handle stuff */
			Misc.p_ptr.handle_stuff();

			/* Flush the messages */
			Utilities.message_flush();

			/* Flush the input */
			Utilities.flush();


			/* No suspending now */
			Signals.ignore_tstp();


			/* Hack -- Increase "icky" depth */
			Misc.character_icky++;


			/* Handle death */
			if (Misc.p_ptr.is_dead)
			{
				Death.screen();
			}

			/* Still alive */
			else
			{
				/* Save the game */
				save_game();

				if (Term.instance.mapped_flag)
				{
					keypress ch;

					Utilities.prt("Press Return (or Escape).", 0, 40);
					ch = Utilities.inkey();
					if (ch.code != keycode_t.ESCAPE)
						Score.predict_score();
				}
			}


			/* Hack -- Decrease "icky" depth */
			Misc.character_icky--;


			/* Allow suspending now */
			Signals.handle_tstp();
		}

		/*
		 * Save the game
		 */
		public static void save_game()
		{
			/* Disturb the player */
			Cave.disturb(Misc.p_ptr, 1, 0);

			/* Clear messages */
			Utilities.message_flush();

			/* Handle stuff */
			Misc.p_ptr.handle_stuff();

			/* Message */
			Utilities.prt("Saving game...", 0, 0);

			/* Refresh */
			Term.fresh();

			/* The player is not dead */
			Misc.p_ptr.died_from = "(saved)";

			/* Forbid suspend */
			Signals.ignore_tstp();

			/* Save the player */
			if (Savefile.savefile_save(savefile))
			    Utilities.prt("Saving game... done.", 0, 0);
			else
			    Utilities.prt("Saving game... failed!", 0, 0);

			/* Allow suspend again */
			Signals.handle_tstp();

			/* Refresh */
			Term.fresh();

			/* Note that the player is not dead */
			Misc.p_ptr.died_from = "(alive and well)";
		}
	}
}
