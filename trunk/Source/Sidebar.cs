using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Sidebar {
		/* ------------------------------------------------------------------------
		 * Sidebar display functions
		 * ------------------------------------------------------------------------ */

		/*
		 * Print character info at given row, column in a 13 char field
		 */
		static void prt_field(string info, int row, int col)
		{
			/* Dump 13 spaces to clear */
			Utilities.c_put_str(ConsoleColor.White, "             ", row, col);

			/* Dump the info itself */
			Utilities.c_put_str(ConsoleColor.Cyan, info, row, col);
		}


		/*
		 * Print character stat in given row, column
		 */
		static void prt_stat(Stat stat, int row, int col)
		{
			string tmp; //32

			/* Display "injured" stat */
			if (Misc.p_ptr.stat_cur[(int)stat] < Misc.p_ptr.stat_max[(int)stat])
			{
			    Utilities.put_str(Stat_Names.Reduced[(int)stat], row, col);
			    Files.cnv_stat(Misc.p_ptr.state.stat_use[(int)stat], out tmp);
			    Utilities.c_put_str(ConsoleColor.Yellow, tmp, row, col + 6);
			}

			/* Display "healthy" stat */
			else
			{
			    Utilities.put_str(Stat_Names.Normal[(int)stat], row, col);
			    Files.cnv_stat(Misc.p_ptr.state.stat_use[(int)stat], out tmp);
			    Utilities.c_put_str(ConsoleColor.Green, tmp, row, col + 6);
			}

			/* Indicate natural maximum */
			if (Misc.p_ptr.stat_max[(int)stat] == 18+100)
			{
			    Utilities.put_str("!", row, col + 3);
			}
		}


		/*
		 * Prints "title", including "wizard" or "winner" as needed.
		 */
		static void prt_title(int row, int col)
		{
			string p;

			/* Wizard */
			if (Misc.p_ptr.wizard)
			{
			    p = "[=-WIZARD-=]";
			}

			/* Winner */
			else if (Misc.p_ptr.total_winner != 0 || (Misc.p_ptr.lev > Misc.PY_MAX_LEVEL))
			{
			    p = "***WINNER***";
			}

			/* Normal */
			else
			{
			    p = Misc.p_ptr.Class.title[(Misc.p_ptr.lev - 1) / 5];
			}

			prt_field(p, row, col);
		}


		/*
		 * Prints level
		 */
		static void prt_level(int row, int col)
		{
			string tmp;//char tmp[32];

			tmp = Misc.p_ptr.lev.ToString().PadLeft(6, ' ');
			//strnfmt(tmp, sizeof(tmp), "%6d", p_ptr.lev);

			if (Misc.p_ptr.lev >= Misc.p_ptr.max_lev)
			{
			    Utilities.put_str("LEVEL ", row, col);
			    Utilities.c_put_str(ConsoleColor.Green, tmp, row, col + 6);
			}
			else
			{
			    Utilities.put_str("Level ", row, col);
			    Utilities.c_put_str(ConsoleColor.Yellow, tmp, row, col + 6);
			}
		}


		/*
		 * Display the experience
		 */
		static void prt_exp(int row, int col)
		{
			//char out_val[32];
			string out_val;
			bool lev50 = (Misc.p_ptr.lev == 50); //Nick: 50 is definitely a magic number. Is this max level?

			long xp = (long)Misc.p_ptr.exp;


			/* Calculate XP for next level */
			if (!lev50)
			    xp = (long)(Player.Player.player_exp[Misc.p_ptr.lev - 1] * Misc.p_ptr.expfact / 100L) - Misc.p_ptr.exp;

			/* Format XP */
			out_val = xp.ToString().PadLeft(8, ' ');
			//strnfmt(out_val, sizeof(out_val), "%8ld", (long)xp);


			if (Misc.p_ptr.exp >= Misc.p_ptr.max_exp)
			{
			    Utilities.put_str((lev50 ? "EXP" : "NXT"), row, col);
			    Utilities.c_put_str(ConsoleColor.Green, out_val, row, col + 4);
			}
			else
			{
			    Utilities.put_str((lev50 ? "Exp" : "Nxt"), row, col);
			    Utilities.c_put_str(ConsoleColor.Yellow, out_val, row, col + 4);
			}
		}


		/*
		 * Prints current gold
		 */
		static void prt_gold(int row, int col)
		{
			//char tmp[32];
			string tmp;

			Utilities.put_str("AU ", row, col);
			tmp = Misc.p_ptr.au.ToString().PadLeft(9, ' ');
			//strnfmt(tmp, sizeof(tmp), "%9ld", (long)p_ptr.au);
			Utilities.c_put_str(ConsoleColor.Green, tmp, row, col + 3);
		}


		/*
		 * Equippy chars
		 */
		static void prt_equippy(int row, int col)
		{
			int i;

			ConsoleColor a;
			char c;

			Object.Object o_ptr;

			/* No equippy chars in bigtile mode */
			if (Term.tile_width > 1 || Term.tile_height > 1) return;

			/* Dump equippy chars */
			for (i = Misc.INVEN_WIELD; i < Misc.INVEN_TOTAL; i++) {
			    /* Object */
			    o_ptr = Misc.p_ptr.inventory[i];

			    if (o_ptr.kind != null) {
			        c = o_ptr.object_char();
			        a = o_ptr.object_attr();
			    } else {
			        c = ' ';
			        a = ConsoleColor.White;
			    }

			    /* Dump */
			    Term.putch(col + i - Misc.INVEN_WIELD, row, a, c);
			}
		}


		/*
		 * Prints current AC
		 */
		static void prt_ac(int row, int col)
		{
			string tmp;//char tmp[32];

			Utilities.put_str("Cur AC ", row, col);
			tmp = (Misc.p_ptr.state.dis_ac + Misc.p_ptr.state.dis_to_a).ToString().PadLeft(5, ' ');
			//strnfmt(tmp, sizeof(tmp), "%5d", p_ptr.state.dis_ac + p_ptr.state.dis_to_a);
			Utilities.c_put_str(ConsoleColor.Green, tmp, row, col + 7);
		}

		/*
		 * Prints Cur hit points
		 */
		static void prt_hp(int row, int col)
		{
			//char cur_hp[32], max_hp[32];
			string cur_hp, max_hp;
			ConsoleColor color = Misc.p_ptr.hp_attr();

			Utilities.put_str("HP ", row, col);

			max_hp = Misc.p_ptr.mhp.ToString().PadLeft(4, ' ');
			cur_hp = Misc.p_ptr.chp.ToString().PadLeft(4, ' ');
	
			Utilities.c_put_str(color, cur_hp, row, col + 3);
			Utilities.c_put_str(ConsoleColor.White, "/", row, col + 7);
			Utilities.c_put_str(ConsoleColor.Green, max_hp, row, col + 8);
		}

		/*
		 * Prints players max/cur spell points
		 */
		static void prt_sp(int row, int col)
		{
			//char cur_sp[32], max_sp[32];
			string cur_sp, max_sp;
			ConsoleColor color = Misc.p_ptr.sp_attr();

			/* Do not show mana unless we have some */
			if (Misc.p_ptr.msp == 0) return;

			Utilities.put_str("SP ", row, col);

			max_sp = Misc.p_ptr.msp.ToString().PadLeft(4, ' ');
			cur_sp = Misc.p_ptr.csp.ToString().PadLeft(4, ' ');

			/* Show mana */
			Utilities.c_put_str(color, cur_sp, row, col + 3);
			Utilities.c_put_str(ConsoleColor.White, "/", row, col + 7);
			Utilities.c_put_str(ConsoleColor.Green, max_sp, row, col + 8);
		}

		/*
		 * Calculate the monster bar color separately, for ports.
		 */
		static ConsoleColor monster_health_attr()
		{
			ConsoleColor attr = ConsoleColor.White;
	
			/* Not tracking */
			if (Misc.p_ptr.health_who == 0)
			    attr = ConsoleColor.Gray;//TERM_DARK;

			/* Tracking an unseen, hallucinatory, or dead monster */
			else if ((!Cave.cave_monster(Cave.cave, Misc.p_ptr.health_who).ml) ||
			        (Misc.p_ptr.timed[(int)Timed_Effect.IMAGE]) != 0 ||
			        (Cave.cave_monster(Cave.cave, Misc.p_ptr.health_who).hp < 0))
			{
			    /* The monster health is "unknown" */
			    attr = ConsoleColor.White;
			}
	
			else
			{
			    Monster.Monster mon = Cave.cave_monster(Cave.cave, Misc.p_ptr.health_who);
			    int pct;

			    /* Default to almost dead */
			    attr = ConsoleColor.Red;

			    /* Extract the "percent" of health */
			    pct = (int)(100L * mon.hp / mon.maxhp);

			    /* Badly wounded */
			    if (pct >= 10) attr = ConsoleColor.Red;

			    /* Wounded */
			    if (pct >= 25) attr = ConsoleColor.DarkYellow;

			    /* Somewhat Wounded */
			    if (pct >= 60) attr = ConsoleColor.Yellow;

			    /* Healthy */
			    if (pct >= 100) attr = ConsoleColor.Green;

			    /* Afraid */
			    if (mon.m_timed[(int)Misc.MON_TMD.FEAR] != 0) attr = ConsoleColor.Magenta;

			    /* Confused */
			    if (mon.m_timed[(int)Misc.MON_TMD.CONF] != 0) attr = ConsoleColor.DarkRed; //UMBER

			    /* Stunned */
			    if (mon.m_timed[(int)Misc.MON_TMD.STUN] != 0) attr = ConsoleColor.Blue; //Light Blue

			    /* Asleep */
			    if (mon.m_timed[(int)Misc.MON_TMD.SLEEP] != 0) attr = ConsoleColor.DarkBlue; //Blue
			}
	
			return attr;
		}

		/*
		 * Redraw the "monster health bar"
		 *
		 * The "monster health bar" provides visual feedback on the "health"
		 * of the monster currently being "tracked".  There are several ways
		 * to "track" a monster, including targetting it, attacking it, and
		 * affecting it (and nobody else) with a ranged attack.  When nothing
		 * is being tracked, we clear the health bar.  If the monster being
		 * tracked is not currently visible, a special health bar is shown.
		 */
		static void prt_health(int row, int col)
		{
			ConsoleColor attr = monster_health_attr();
			Monster.Monster mon;
	
			/* Not tracking */
			if (Misc.p_ptr.health_who == 0)
			{
			    /* Erase the health bar */
			    Term.erase(col, row, 12);
			    return;
			}

			mon = Cave.cave_monster(Cave.cave, Misc.p_ptr.health_who);

			/* Tracking an unseen, hallucinatory, or dead monster */
			if ((!mon.ml) || /* Unseen */
			        (Misc.p_ptr.timed[(int)Timed_Effect.IMAGE] != 0) || /* Hallucination */
			        (mon.hp < 0)) /* Dead (?) */
			{
			    /* The monster health is "unknown" */
			    Term.putstr(col, row, 12, attr, "[----------]");
			}

			/* Tracking a visible monster */
			else
			{
			    int pct, len;

			    Monster.Monster m_ptr = Cave.cave_monster(Cave.cave, Misc.p_ptr.health_who);

			    /* Extract the "percent" of health */
			    pct = (int)(100L * m_ptr.hp / m_ptr.maxhp);

			    /* Convert percent into "health" */
			    len = (pct < 10) ? 1 : (pct < 90) ? (pct / 10 + 1) : 10;

			    /* Default to "unknown" */
			    Term.putstr(col, row, 12, ConsoleColor.White, "[----------]");

			    /* Dump the current "health" (use '*' symbols) */
			    Term.putstr(col + 1, row, len, attr, "**********");
			}
		}


		/*
		 * Prints the speed of a character.
		 */
		static void prt_speed(int row, int col)
		{
			int i = Misc.p_ptr.state.speed;

			ConsoleColor attr = ConsoleColor.White;
			string type = null;
			//char buf[32] = "";
			string buf = "";

			/* Hack -- Visually "undo" the Search Mode Slowdown */
			if (Misc.p_ptr.searching != 0) i += 10;

			/* Fast */
			if (i > 110)
			{
			    attr = ConsoleColor.Green;
			    type = "Fast";
			}

			/* Slow */
			else if (i < 110)
			{
			    attr = ConsoleColor.DarkRed; //UMBER
			    type = "Slow";
			}

			if(type != null)
				buf = type + "(" + (i - 110).ToString() + ")";
			    //strnfmt(buf, sizeof(buf), "%s (%+d)", type, (i - 110));

			/* Display the speed */
			//c_put_str(attr, format("%-10s", buf), row, col);
			Utilities.c_put_str(attr, buf, row, col);
		}


		/*
		 * Prints depth in stat area
		 */
		static void prt_depth(int row, int col)
		{
			//char depths[32];
			string depths;
			
			if (Misc.p_ptr.depth == 0)
			{
				depths = "Town";
			    //my_strcpy(depths, "Town", sizeof(depths));
			}
			else
			{
				depths = (Misc.p_ptr.depth * 50).ToString() + "' (L" + Misc.p_ptr.depth + ")";
			    /*strnfmt(depths, sizeof(depths), "%d' (L%d)",
			            p_ptr.depth * 50, p_ptr.depth);*/
			}

			/* Right-Adjust the "depth", and clear old values */
			Utilities.put_str(depths, row, col);
			//put_str(format("%-13s", depths), row, col);
		}




		/* Some simple wrapper functions */
		static void prt_str(int row, int col) { prt_stat(Stat.Str, row, col); }
		static void prt_dex(int row, int col) { prt_stat(Stat.Dex, row, col); }
		static void prt_wis(int row, int col) { prt_stat(Stat.Wis, row, col); }
		static void prt_int(int row, int col) { prt_stat(Stat.Int, row, col); }
		static void prt_con(int row, int col) { prt_stat(Stat.Con, row, col); }
		static void prt_chr(int row, int col) { prt_stat(Stat.Chr, row, col); }
		static void prt_race(int row, int col) { prt_field(Misc.p_ptr.Race.Name, row, col); }
		static void prt_class(int row, int col) { prt_field(Misc.p_ptr.Class.Name, row, col); }


		/*
		 * Struct of sidebar handlers.
		 */
		class side_handler_t
		{
			public side_handler_t(hook_func a, int b, Game_Event.Event_Type c){
				hook = a;
				priority = b;
				type = c;
			}

			public delegate void hook_func(int r, int c);
			public hook_func hook;	 /* int row, int col */
			public int priority;		 /* 1 is most important (always displayed) */
			public Game_Event.Event_Type type;	 /* PR_* flag this corresponds to */
		} 
	
		static side_handler_t[] side_handlers =
		{
			new side_handler_t( prt_race,    19, Game_Event.Event_Type.RACE_CLASS ),
			new side_handler_t( prt_title,   18, Game_Event.Event_Type.PLAYERTITLE ),
			new side_handler_t( prt_class,   22, Game_Event.Event_Type.RACE_CLASS ),
			new side_handler_t( prt_level,   10, Game_Event.Event_Type.PLAYERLEVEL ),
			new side_handler_t( prt_exp,     16, Game_Event.Event_Type.EXPERIENCE ),
			new side_handler_t( prt_gold,    11, Game_Event.Event_Type.GOLD ),
			new side_handler_t( prt_equippy, 17, Game_Event.Event_Type.EQUIPMENT ),
			new side_handler_t( prt_str,      6, Game_Event.Event_Type.STATS ),
			new side_handler_t( prt_int,      5, Game_Event.Event_Type.STATS ),
			new side_handler_t( prt_wis,      4, Game_Event.Event_Type.STATS ),
			new side_handler_t( prt_dex,      3, Game_Event.Event_Type.STATS ),
			new side_handler_t( prt_con,      2, Game_Event.Event_Type.STATS ),
			new side_handler_t( prt_chr,      1, Game_Event.Event_Type.STATS ),
			new side_handler_t( null,        15, 0 ),
			new side_handler_t( prt_ac,       7, Game_Event.Event_Type.AC ),
			new side_handler_t( prt_hp,       8, Game_Event.Event_Type.HP ),
			new side_handler_t( prt_sp,       9, Game_Event.Event_Type.MANA ),
			new side_handler_t( null,        21, 0 ),
			new side_handler_t( prt_health,  12, Game_Event.Event_Type.MONSTERHEALTH ),
			new side_handler_t( null,        20, 0 ),
			new side_handler_t( null,        22, 0 ),
			new side_handler_t( prt_speed,   13, Game_Event.Event_Type.PLAYERSPEED ), /* Slow (-NN) / Fast (+NN) */
			new side_handler_t( prt_depth,   14, Game_Event.Event_Type.DUNGEONLEVEL ), /* Lev NNN / NNNN ft */
		};


		/*
		 * This prints the sidebar, using a clever method which means that it will only
		 * print as much as can be displayed on <24-line screens.
		 *
		 * Each row is given a priority; the least important higher numbers and the most
		 * important lower numbers.  As the screen gets smaller, the rows start to
		 * disappear in the order of lowest to highest importance.
		 */
		public static void update_sidebar(Game_Event.Event_Type type, Game_Event data, object user)
		{
			int x, y, row;
			int max_priority;
			int i;


			Term.get_size(out x, out y);

			/* Keep the top and bottom lines clear. */
			max_priority = y - 2;

			/* Display list entries */
			for (i = 0, row = 1; i < side_handlers.Length; i++)
			{
				side_handler_t hnd = side_handlers[i];
				int priority = hnd.priority;
				bool from_bottom = false;

				/* Negative means print from bottom */
				if (priority < 0)
				{
					priority = -priority;
					from_bottom = true;
				}

				/* If this is high enough priority, display it */
				if (priority <= max_priority)
				{
					if (hnd.type == type && hnd.hook != null)
					{
						if (from_bottom)
							hnd.hook(Term.instance.hgt - (side_handlers.Length - i), 0);
						else
							hnd.hook(row, 0);
					}

					/* Increment for next time */
					row++;
				}
			}
		}

		public static void hp_colour_change(Game_Event.Event_Type type, Game_Event data, object user)
		{
			/*
			 * hack:  redraw player, since the player's color
			 * now indicates approximate health.  Note that
			 * using this command when graphics mode is on
			 * causes the character to be a black square.
			 */
			if ((Option.hp_changes_color.value) && (Misc.arg_graphics == Misc.GRAPHICS_NONE))
			{
			    Cave.cave_light_spot(Cave.cave, Misc.p_ptr.py, Misc.p_ptr.px);
			}
		}
	}
}
