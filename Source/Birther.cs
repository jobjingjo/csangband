using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Player;
using CSAngband.Object;

namespace CSAngband {
	class Birther {
		/*
		 * Overview
		 * ========
		 * This file contains the game-mechanical part of the birth process.
		 * To follow the code, start at player_birth towards the bottom of
		 * the file - that is the only external entry point to the functions
		 * defined here.
		 *
		 * Player (in the Angband sense of character) birth is modelled as a
		 * a series of commands from the UI to the game to manipulate the
		 * character and corresponding events to inform the UI of the outcomes
		 * of these changes.
		 *
		 * The current aim of this section is that after any birth command
		 * is carried out, the character should be left in a playable state.
		 * In particular, this means that if a savefile is supplied, the
		 * character will be set up according to the "quickstart" rules until
		 * another race or class is chosen, or until the stats are reset by
		 * the UI.
		 *
		 * Once the UI signals that the player is happy with the character, the
		 * game does housekeeping to ensure the character is ready to start the
		 * game (clearing the history log, making sure options are set, etc)
		 * before returning control to the game proper.
		 */


		/* 
		 * Maximum amount of starting equipment, and starting gold
		 */
		const int STARTING_GOLD = 600;


		/*
		 * Forward declare
		 */
		//typedef struct Birther /*lovely*/ Birther; /*sometimes we think she's a dream*/

		/*
		 * A structure to hold "rolled" information, and any
		 * other useful state for the birth process.
		 *
		 * XXX Demand Obama's birth certificate
		 */
		byte sex;
		Player_Race Race;
		Player_Class Class;

		short age;
		short wt;
		short ht;
		short sc;

		int au;

		short[] stat = new short[(int)Stat.Max];

		string history;



		/*
		 * Save the currently rolled data into the supplied 'player'.
		 */
		void save_roller_data()
		{
			throw new NotImplementedException();
			//int i;

			///* Save the data */
			//player.sex = Player.Player.instance.psex;
			//player.race = Player.Player.instance.race;
			//player.class = Player.Player.instance.class;
			//player.age = Player.Player.instance.age;
			//player.wt = Player.Player.instance.wt_birth;
			//player.ht = Player.Player.instance.ht_birth;
			//player.sc = Player.Player.instance.sc_birth;
			//player.au = Player.Player.instance.au_birth;

			///* Save the stats */
			//for (i = 0; i < A_MAX; i++)
			//    player.stat[i] = Player.Player.instance.stat_birth[i];

			//player.history = Player.Player.instance.history;
		}


		/*
		 * Load stored player data from 'player' as the currently rolled data,
		 * optionally placing the current data in 'prev_player' (if 'prev_player'
		 * is non-null).
		 *
		 * It is perfectly legal to specify the same "Birther" for both 'player'
		 * and 'prev_player'.
		 */
		void load_roller_data(Birther prev_player)
		{
			throw new NotImplementedException();
			//int i;

			// /* The initialisation is just paranoia - structure assignment is
			//    (perhaps) not strictly defined to work with uninitialised parts
			//    of structures. */
			//Birther temp = new Birther();

			///*** Save the current data if we'll need it later ***/
			//if (prev_player) save_roller_data(&temp);

			///*** Load the previous data ***/

			///* Load the data */
			//Player.Player.instance.psex = player.sex;
			//Player.Player.instance.race = player.race;
			//Player.Player.instance.class = player.class;
			//Player.Player.instance.age = player.age;
			//Player.Player.instance.wt = Player.Player.instance.wt_birth = player.wt;
			//Player.Player.instance.ht = Player.Player.instance.ht_birth = player.ht;
			//Player.Player.instance.sc = Player.Player.instance.sc_birth = player.sc;
			//Player.Player.instance.au_birth = player.au;
			//Player.Player.instance.au = STARTING_GOLD;

			///* Load the stats */
			//for (i = 0; i < A_MAX; i++)
			//{
			//    Player.Player.instance.stat_max[i] = Player.Player.instance.stat_cur[i] = Player.Player.instance.stat_birth[i] = player.stat[i];
			//}

			///* Load the history */
			//Player.Player.instance.history = player.history;

			///*** Save the current data if the caller is interested in it. ***/
			//if (prev_player) *prev_player = temp;
		}


		/*
		 * Adjust a stat by an amount.
		 *
		 * This just uses "modify_stat_value()" unless "maximize" mode is false,
		 * and a positive bonus is being applied, in which case, a special hack
		 * is used.
		 */
		static int adjust_stat(int value, int amount)
		{
			throw new NotImplementedException();
			///* Negative amounts or maximize mode */
			//if ((amount < 0) || OPT(birth_maximize))
			//{
			//    return (modify_stat_value(value, amount));
			//}

			///* Special hack */
			//else
			//{
			//    int i;

			//    /* Apply reward */
			//    for (i = 0; i < amount; i++)
			//    {
			//        if (value < 18)
			//        {
			//            value++;
			//        }
			//        else if (value < 18+70)
			//        {
			//            value += randint1(15) + 5;
			//        }
			//        else if (value < 18+90)
			//        {
			//            value += randint1(6) + 2;
			//        }
			//        else if (value < 18+100)
			//        {
			//            value++;
			//        }
			//    }
			//}

			///* Return the result */
			//return (value);
		}




		/*
		 * Roll for a characters stats
		 *
		 * For efficiency, we include a chunk of "calc_bonuses()".
		 */
		static void get_stats(int[] stat_use)
		{
			throw new NotImplementedException();
			//int i, j;

			//int bonus;

			//int dice[18];


			///* Roll and verify some stats */
			//while (true)
			//{
			//    /* Roll some dice */
			//    for (j = i = 0; i < 18; i++)
			//    {
			//        /* Roll the dice */
			//        dice[i] = randint1(3 + i % 3);

			//        /* Collect the maximum */
			//        j += dice[i];
			//    }

			//    /* Verify totals */
			//    if ((j > 42) && (j < 54)) break;
			//}

			///* Roll the stats */
			//for (i = 0; i < A_MAX; i++)
			//{
			//    /* Extract 5 + 1d3 + 1d4 + 1d5 */
			//    j = 5 + dice[3*i] + dice[3*i+1] + dice[3*i+2];

			//    /* Save that value */
			//    Player.Player.instance.stat_max[i] = j;

			//    /* Obtain a "bonus" for "race" and "class" */
			//    bonus = Player.Player.instance.race.r_adj[i] + Player.Player.instance.class.c_adj[i];

			//    /* Variable stat maxes */
			//    if (OPT(birth_maximize))
			//    {
			//        /* Start fully healed */
			//        Player.Player.instance.stat_cur[i] = Player.Player.instance.stat_max[i];

			//        /* Efficiency -- Apply the racial/class bonuses */
			//        stat_use[i] = modify_stat_value(Player.Player.instance.stat_max[i], bonus);
			//    }

			//    /* Fixed stat maxes */
			//    else
			//    {
			//        /* Apply the bonus to the stat (somewhat randomly) */
			//        stat_use[i] = adjust_stat(Player.Player.instance.stat_max[i], bonus);

			//        /* Save the resulting stat maximum */
			//        Player.Player.instance.stat_cur[i] = Player.Player.instance.stat_max[i] = stat_use[i];
			//    }

			//    Player.Player.instance.stat_birth[i] = Player.Player.instance.stat_max[i];
			//}
		}


		static void roll_hp()
		{
			int i, j, min_value, max_value;

			/* Minimum hitpoints at highest level */
			min_value = (Misc.PY_MAX_LEVEL * (Player.Player.instance.hitdie - 1) * 3) / 8;
			min_value += Misc.PY_MAX_LEVEL;

			/* Maximum hitpoints at highest level */
			max_value = (Misc.PY_MAX_LEVEL * (Player.Player.instance.hitdie - 1) * 5) / 8;
			max_value += Misc.PY_MAX_LEVEL;

			/* Roll out the hitpoints */
			while (true)
			{
			    /* Roll the hitpoint values */
			    for (i = 1; i < Misc.PY_MAX_LEVEL; i++)
			    {
			        j = (int)Random.randint1(Player.Player.instance.hitdie);
			        Player.Player.instance.player_hp[i] = (short)(Player.Player.instance.player_hp[i-1] + j);
			    }

			    /* XXX Could also require acceptable "mid-level" hitpoints */

			    /* Require "valid" hitpoints at highest level */
			    if (Player.Player.instance.player_hp[Misc.PY_MAX_LEVEL-1] < min_value) continue;
			    if (Player.Player.instance.player_hp[Misc.PY_MAX_LEVEL-1] > max_value) continue;

			    /* Acceptable */
			    break;
			}
		}


		static void get_bonuses()
		{
			/* Calculate the bonuses and hitpoints */
			Player.Player.instance.update |= (uint)(Misc.PU_BONUS | Misc.PU_HP);

			/* Update stuff */
			Player.Player.instance.update_stuff();

			/* Fully healed */
			Player.Player.instance.chp = Player.Player.instance.mhp;

			/* Fully rested */
			Player.Player.instance.csp = Player.Player.instance.msp;
		}



		/*
		 * Get the player's starting money
		 */
		static void get_money()
		{
			//Nick: This was commented out before I got here...
			/*if (OPT(birth_money))
		    {
		        Player.Player.instance.au_birth = 200;
		        Player.Player.instance.au = 500;
		    }
		    else
		    {*/
		        Player.Player.instance.au = Player.Player.instance.au_birth = STARTING_GOLD;
		}


		/**
		 * Try to wield everything wieldable in the inventory.
		 */
		static void wield_all(Player.Player p)
		{
			Object.Object o_ptr;
			Object.Object i_ptr;
			//Object.Object object_type_body;

			int slot;
			int item;
			int num;
			bool is_ammo;

			/* Scan through the slots backwards */
			for (item = Misc.INVEN_PACK - 1; item >= 0; item--)
			{
			    o_ptr = p.inventory[item];
			    is_ammo = o_ptr.is_ammo();

			    /* Skip non-objects */
			    if (o_ptr.kind == null) continue;

			    /* Make sure we can wield it */
			    slot = o_ptr.wield_slot();
			    if (slot < Misc.INVEN_WIELD) continue;

			    i_ptr = p.inventory[slot];
			    if (i_ptr.kind != null && (!is_ammo ||(is_ammo && !o_ptr.similar(i_ptr, Object.Object.object_stack_t.OSTACK_PACK))))
			        continue;

			    /* Figure out how much of the item we'll be wielding */
			    num = is_ammo ? o_ptr.number : 1;

			    /* Get local object */
				i_ptr = new Object.Object();
				i_ptr = o_ptr.copy();
				//This entire bit was uber shadey... Rewrote above
				////i_ptr = object_type_body;
				//p.inventory[slot] = o_ptr; //object_copy(i_ptr, o_ptr); //This might not work...
				//i_ptr = o_ptr;  //If wonky equips happen, check here

			    /* Modify quantity */
			    i_ptr.number = (byte)num;

				/* Decrease the item (from the pack) */
				Object.Object.inven_item_increase(item, -num);
				Object.Object.inven_item_optimize(item);

				/* Get the wield slot */
				//o_ptr = p.inventory[slot];

				/* Wear the new stuff */
				//object_copy(o_ptr, i_ptr);
				p.inventory[slot] = i_ptr;

				/* Increase the weight */
				p.total_weight += i_ptr.weight * i_ptr.number;

				/* Increment the equip counter by hand */
				p.equip_cnt++;
			}

			Object.Object.save_quiver_size(p);

			return;
		}


		/*
		 * Init players with some belongings
		 *
		 * Having an item identifies it and makes the player "aware" of its purpose.
		 */
		static void player_outfit(Player.Player p)
		{
			//Object.Object object_type_body = new Object.Object();

			/* Give the player starting equipment */
			for (Start_Item si = Player.Player.instance.Class.start_items; si != null; si = si.next)
			{
			    /* Get local object */
			    Object.Object i_ptr = new Object.Object();

			    /* Prepare the item */
			    i_ptr.prep(si.kind, 0, aspect.MINIMISE);
			    i_ptr.number = (byte)Random.rand_range(si.min, si.max);
			    i_ptr.origin = Origin.BIRTH;

			    i_ptr.flavor_aware();
			    i_ptr.notice_everything();

			    i_ptr.inven_carry(p);
			    si.kind.everseen = true;

			    /* Deduct the cost of the item from starting cash */
			    p.au -= i_ptr.value(i_ptr.number, false);
			}

			/* Sanity check */
			if (p.au < 0)
			    p.au = 0;

			/* Now try wielding everything */
			wield_all(p);
		}

		/*
		 * Modify a stat value by a "modifier", return new value
		 *
		 * Stats go up: 3,4,...,17,18,18/10,18/20,...,18/220
		 * Or even: 18/13, 18/23, 18/33, ..., 18/220
		 *
		 * Stats go down: 18/220, 18/210,..., 18/10, 18, 17, ..., 3
		 * Or even: 18/13, 18/03, 18, 17, ..., 3
		 */
		public static short modify_stat_value(int value, int amount)
		{
			int i;

			/* Reward */
			if (amount > 0)
			{
				/* Apply each point */
				for (i = 0; i < amount; i++)
				{
					/* One point at a time */
					if (value < 18) value++;

					/* Ten "points" at a time */
					else value += 10;
				}
			}

			/* Penalty */
			else if (amount < 0)
			{
				/* Apply each point */
				for (i = 0; i < (0 - amount); i++)
				{
					/* Ten points at a time */
					if (value >= 18+10) value -= 10;

					/* Hack -- prevent weirdness */
					else if (value > 18) value = 18;

					/* One point at a time */
					else if (value > 3) value--;
				}
			}

			/* Return new value */
			return (short)(value);
		}

		/*
		 * Cost of each "point" of a stat.
		 */
		static int[] birth_stat_costs = new int[18 + 1] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 2, 4 };

		/* It was feasible to get base 17 in 3 stats with the autoroller */
		const int MAX_BIRTH_POINTS = 24; /* 3 * (1+1+1+1+1+1+2) */

		static void recalculate_stats(short[] stats, int points_left)
		{
			/* Process stats */
			for (int i = 0; i < (int)Stat.Max; i++)
			{
			    /* Variable stat maxes */
			    if (Option.birth_maximize.value)
			    {
			        /* Reset stats */
			        Player.Player.instance.stat_cur[i] = Player.Player.instance.stat_max[i] =
			            Player.Player.instance.stat_birth[i] = stats[i];
			    }

			    /* Fixed stat maxes */
			    else
			    {
					int bonus = 0;
			        /* Obtain a "bonus" for "race" and "class" */
					if (Player.Player.instance.Race != null || Player.Player.instance.Class != null)
						bonus = Player.Player.instance.Race.r_adj[i] + Player.Player.instance.Class.c_adj[i];

			        /* Apply the racial/class bonuses */
			        Player.Player.instance.stat_cur[i] = Player.Player.instance.stat_max[i] = 
			            Player.Player.instance.stat_birth[i] =
			            modify_stat_value(stats[i], bonus);
			    }
			}

			/* Gold is inversely proportional to cost */
			Player.Player.instance.au_birth = STARTING_GOLD + (50 * points_left);

			/* Update bonuses, hp, etc. */
			get_bonuses();

			/* Tell the UI about all this stuff that's changed. */
			Game_Event.signal(Game_Event.Event_Type.GOLD);
			Game_Event.signal(Game_Event.Event_Type.AC);
			Game_Event.signal(Game_Event.Event_Type.HP);
			Game_Event.signal(Game_Event.Event_Type.STATS);
		}

		static void reset_stats(short[] stats, int[] points_spent, ref int points_left)
		{
			/* Calculate and signal initial stats and points totals. */
			points_left = MAX_BIRTH_POINTS;

			for (int i = 0; i < (int)Stat.Max; i++)
			{
			    /* Initial stats are all 10 and costs are zero */
			    stats[i] = 10;
			    points_spent[i] = 0;
			}

			/* Use the new "birth stat" values to work out the "other"
			   stat values (i.e. after modifiers) and tell the UI things have 
			   changed. */
			recalculate_stats(stats, points_left);
			Game_Event.signal_birthpoints(points_spent, points_left);	
		}

		static bool buy_stat(Stat choice, short[] stats, int[] points_spent, ref int points_left)
		{
			/* Must be a valid stat, and have a "base" of below 18 to be adjusted */
			if (!(choice >= Stat.Max || choice < 0) &&	(stats[(int)choice] < 18))
			{
			    /* Get the cost of buying the extra point (beyond what
			       it has already cost to get this far). */
			    int stat_cost = birth_stat_costs[stats[(int)choice] + 1];

			    if (stat_cost <= points_left)
			    {
			        stats[(int)choice]++;
			        points_spent[(int)choice] += stat_cost;
			        points_left -= stat_cost;

			        /* Tell the UI the new points situation. */
					Game_Event.signal_birthpoints(points_spent, points_left);

			        /* Recalculate everything that's changed because
			           the stat has changed, and inform the UI. */
			        recalculate_stats(stats, points_left);

			        return true;
			    }
			}

			/* Didn't adjust stat. */
			return false;
		}


		static bool sell_stat(Stat choice, short[] stats, int[] points_spent, ref int points_left)
		{
			/* Must be a valid stat, and we can't "sell" stats below the base of 10. */
			if (!(choice >= Stat.Max || choice < 0) && (stats[(int)choice] > 10))
			{
			    int stat_cost = birth_stat_costs[stats[(int)choice]];

			    stats[(int)choice]--;
			    points_spent[(int)choice] -= stat_cost;
			    points_left += stat_cost;

			    /* Tell the UI the new points situation. */
			    Game_Event.signal_birthpoints(points_spent, points_left);

			    /* Recalculate everything that's changed because
			       the stat has changed, and inform the UI. */
			    recalculate_stats(stats, points_left);

			    return true;
			}

			/* Didn't adjust stat. */
			return false;
		}


		/*
		 * This picks some reasonable starting values for stats based on the
		 * current race/class combo, etc.  For now I'm disregarding concerns
		 * about role-playing, etc, and using the simple outline from
		 * http://angband.oook.cz/forum/showpost.php?p=17588&postcount=6:
		 *
		 * 0. buy base STR 17
		 * 1. if possible buy adj DEX of 18/10
		 * 2. spend up to half remaining points on each of spell-stat and con, 
		 *    but only up to max base of 16 unless a pure class 
		 *    [mage or priest or warrior]
		 * 3. If there are any points left, spend as much as possible in order 
		 *    on DEX, non-spell-stat, CHR. 
		 */
		static void generate_stats(short [] stats, int [] points_spent, ref int points_left)
		{
			int step = 0;
			bool[] maxed = new bool[(int)Stat.Max];
			for(int i = 0; i < maxed.Length; i++) {
				maxed[i] = false;
			}
			bool pure = false;

			/* Determine whether the class is "pure" */
			if (Player.Player.instance.Class.spell_stat == 0 || Player.Player.instance.Class.max_attacks < 5)
			{
			    pure = true;
			}

			while (points_left > 0 && step >= 0)
			{
			    switch (step)
			    {
			        /* Buy base STR 17 */
			        case 0:				
			        {
			            if (!maxed[(int)Stat.Str] && stats[(int)Stat.Str] < 17)
			            {
			                if (!buy_stat(Stat.Str, stats, points_spent, ref points_left))
			                    maxed[(int)Stat.Str] = true;
			            }
			            else
			            {
			                step++;
			            }

			            break;
			        }

			        /* Try and buy adj DEX of 18/10 */
			        case 1:
			        {
			            if (!maxed[(int)Stat.Dex] && Player.Player.instance.state.stat_top[(int)Stat.Dex] < 18+10)
			            {
			                if (!buy_stat(Stat.Dex, stats, points_spent, ref points_left))
			                    maxed[(int)Stat.Dex] = true;
			            }
			            else
			            {
			                step++;
			            }

			            break;
			        }

			        /* If we can't get 18/10 dex, sell it back. */
			        case 2:
			        {
			            if (Player.Player.instance.state.stat_top[(int)Stat.Dex] < 18+10)
			            {
			                while (stats[(int)Stat.Dex] > 10)
			                    sell_stat(Stat.Dex, stats, points_spent, ref points_left);

			                maxed[(int)Stat.Dex] = false;
			            }
				
			            step++;
						break;
			        }

			        /* 
			         * Spend up to half remaining points on each of spell-stat and 
			         * con, but only up to max base of 16 unless a pure class 
			         * [mage or priest or warrior]
			         */
			        case 3:
			        {
			            int points_trigger = points_left / 2;

			            if (Player.Player.instance.Class.spell_stat != 0)
			            {
			                while (!maxed[Player.Player.instance.Class.spell_stat] &&
			                       (pure || stats[Player.Player.instance.Class.spell_stat] < 16) &&
			                       points_spent[Player.Player.instance.Class.spell_stat] < points_trigger)
			                {						
			                    if (!buy_stat((Stat)Player.Player.instance.Class.spell_stat, stats, points_spent, ref points_left))
			                    {
			                        maxed[Player.Player.instance.Class.spell_stat] = true;
			                    }

			                    if (points_spent[Player.Player.instance.Class.spell_stat] > points_trigger)
			                    {
			                        sell_stat((Stat)Player.Player.instance.Class.spell_stat, stats, points_spent, ref points_left);
			                        maxed[Player.Player.instance.Class.spell_stat] = true;
			                    }
			                }
			            }

			            while (!maxed[(int)Stat.Con] && (pure || stats[(int)Stat.Con] < 16) && points_spent[(int)Stat.Con] < points_trigger)
			            {						
			                if (!buy_stat(Stat.Con, stats, points_spent, ref points_left))
			                {
			                    maxed[(int)Stat.Con] = true;
			                }
					
			                if (points_spent[(int)Stat.Con] > points_trigger)
			                {
			                    sell_stat(Stat.Con, stats, points_spent, ref points_left);
			                    maxed[(int)Stat.Con] = true;
			                }
			            }
				
			            step++;
			            break;
			        }

			        /* 
			         * If there are any points left, spend as much as possible in 
			         * order on DEX, non-spell-stat, CHR. 
			         */
			        case 4:
			        {				
			            int next_stat;

			            if (!maxed[(int)Stat.Dex])
			            {
			                next_stat = (int)Stat.Dex;
			            }
			            else if (!maxed[(int)Stat.Int] && Player.Player.instance.Class.spell_stat != (int)Stat.Wis)
			            {
			                next_stat = (int)Stat.Int;
			            }
			            else if (!maxed[(int)Stat.Wis] && Player.Player.instance.Class.spell_stat != (int)Stat.Wis)
			            {
			                next_stat = (int)Stat.Wis;
			            }
			            else if (!maxed[(int)Stat.Chr])
			            {
			                next_stat = (int)Stat.Chr;
			            }
			            else
			            {
			                step++;
			                break;
			            }

			            /* Buy until we can't buy any more. */
			            while (buy_stat((Stat)next_stat, stats, points_spent, ref points_left));
			            maxed[next_stat] = true;

			            break;
			        }

			        default:
			        {
			            step = -1;
			            break;
			        }
			    }
			}
		}




		/* Reset everything back to how it would be on loading the game. */
		static void do_birth_reset(bool use_quickstart, Birther quickstart_prev)
		{
			/* If there's quickstart data, we use it to set default
			   character choices. */
			if (use_quickstart && quickstart_prev != null)
			    quickstart_prev.load_roller_data(null);

			Player.Player.instance.generate(null, null, null);

			/* Update stats with bonuses, etc. */
			get_bonuses();
		}


		/*
		 * Create a new character.
		 *
		 * Note that we may be called with "junk" leftover in the various
		 * fields, so we must be sure to clear them first.
		 */
		public static void player_birth(bool quickstart_allowed)
		{
			Game_Command blank = new Game_Command(Command_Code.NULL, null, null, false, 0);
			Game_Command cmd = blank;

			short[] stats = new short[(int)Stat.Max];
			int[] points_spent = new int[(int)Stat.Max];
			int points_left = 0;
			string buf;
			int success;

			bool rolled_stats = false;

			/*
			 * The last character displayed, to allow the user to flick between two.
			 * We rely on prev.age being zero to determine whether there is a stored
			 * character or not, so initialise it here.
			 */
			Birther prev = new Birther();

			/*
			 * If quickstart is allowed, we store the old character in this,
			 * to allow for it to be reloaded if we step back that far in the
			 * birth process.
			 */
			Birther quickstart_prev = new Birther();

			/*
			 * If there's a quickstart character, store it for later use.
			 * If not, default to whatever the first of the choices is.
			 */
			if(quickstart_allowed) {
				quickstart_prev.save_roller_data();
			} else {
				Player.Player.instance.psex = 0;
				/* XXX default race/class */
				Player.Player.instance.Class = Misc.classes;
				Player.Player.instance.Race = Misc.races;
				Player.Player.instance.generate(null, null, null);
			}

			/* Handle incrementing name suffix */
			buf = Utilities.find_roman_suffix_start(Player_Other.instance.full_name);

			if (buf != null)
			{
			    /* Try to increment the roman suffix */
			    success = Utilities.int_to_roman((Utilities.roman_to_int(buf) + 1), buf);
			
			    if (success == 0) Utilities.msg("Sorry, could not deal with suffix");
			}
	

			/* We're ready to start the interactive birth process. */
			Game_Event.signal_flag(Game_Event.Event_Type.ENTER_BIRTH, quickstart_allowed);

			/* 
			 * Loop around until the UI tells us we have an acceptable character.
			 * Note that it is possible to quit from inside this loop.
			 */
			while (cmd.command != Command_Code.ACCEPT_CHARACTER)
			{
			    /* Grab a command from the queue - we're happy to wait for it. */
			    if (Game_Command.get(cmd_context.CMD_BIRTH, ref cmd, true) == null) continue;

			    if (cmd.command == Command_Code.BIRTH_RESET)
			    {
					Player.Player.instance.generate(null, null, null);
					reset_stats(stats, points_spent, ref points_left);
					do_birth_reset(quickstart_allowed, quickstart_prev);
					rolled_stats = false;
			    }
			    else if (cmd.command == Command_Code.CHOOSE_SEX)
			    {
					Player.Player.instance.psex = (byte)cmd.arg[0].value; 
					Player.Player.instance.generate(null, null, null);
			    }
			    else if (cmd.command == Command_Code.CHOOSE_RACE)
			    {
					Player.Player.instance.generate(null, Player_Race.player_id2race(cmd.arg[0].value), null);

					reset_stats(stats, points_spent, ref points_left);
					generate_stats(stats, points_spent, ref points_left);
					rolled_stats = false;
			    }
			    else if (cmd.command == Command_Code.CHOOSE_CLASS)
			    {
					Player.Player.instance.generate(null, null, Player_Class.player_id2class(cmd.arg[0].value));

					reset_stats(stats, points_spent, ref points_left);
					generate_stats(stats, points_spent, ref points_left);
					rolled_stats = false;
			    }
			    else if (cmd.command == Command_Code.FINALIZE_OPTIONS)
			    {
					/* Reset score options from cheat options */
					for (int i = Option.CHEAT; i < Option.CHEAT + Option.N_OPTS_CHEAT; i++)
					{
					    Player_Other.instance.opt[Option.SCORE + (i - Option.CHEAT)] =
					        Player_Other.instance.opt[i];
					}
			    }
			    else if (cmd.command == Command_Code.BUY_STAT)
			    {
					/* .choice is the stat to buy */
					if (!rolled_stats)
					    buy_stat((Stat)cmd.arg[0].value, stats, points_spent, ref points_left);
			    }
			    else if (cmd.command == Command_Code.SELL_STAT)
			    {
					/* .choice is the stat to sell */
					if (!rolled_stats)
					    sell_stat((Stat)cmd.arg[0].value, stats, points_spent, ref points_left);
			    }
			    else if (cmd.command == Command_Code.RESET_STATS)
			    {
					/* .choice is whether to regen stats */
					reset_stats(stats, points_spent, ref points_left);

					if (cmd.arg[0].value == 1)
					    generate_stats(stats, points_spent, ref points_left);

					rolled_stats = false;
			    }
			    else if (cmd.command == Command_Code.ROLL_STATS)
			    {
					throw new NotImplementedException();
					//int i;

					//save_roller_data(&prev);

					///* Get a new character */
					//get_stats(stats);

					///* Update stats with bonuses, etc. */
					//get_bonuses();

					///* There's no real need to do this here, but it's tradition. */
					//get_ahw(Player.Player.instance);
					//Player.Player.instance.history = get_history(Player.Player.instance.race.history, &Player.Player.instance.sc);
					//Player.Player.instance.sc_birth = Player.Player.instance.sc;

					//event_signal(EVENT_GOLD);
					//event_signal(EVENT_AC);
					//event_signal(EVENT_HP);
					//event_signal(EVENT_STATS);

					///* Give the UI some dummy info about the points situation. */
					//points_left = 0;
					//for (i = 0; i < A_MAX; i++)
					//{
					//    points_spent[i] = 0;
					//}

					//event_signal_birthpoints(points_spent, points_left);

					///* Lock out buying and selling of stats based on rolled stats. */
					//rolled_stats = true;
			    }
			    else if (cmd.command == Command_Code.PREV_STATS)
			    {
					throw new NotImplementedException();
					///* Only switch to the stored "previous"
					//   character if we've actually got one to load. */
					//if (prev.age)
					//{
					//    load_roller_data(&prev, &prev);
					//    get_bonuses();
					//}

					//event_signal(EVENT_GOLD);
					//event_signal(EVENT_AC);
					//event_signal(EVENT_HP);
					//event_signal(EVENT_STATS);
			    }
			    else if (cmd.command == Command_Code.NAME_CHOICE)
			    {
					/* Set player name */
					Player_Other.instance.full_name = cmd.arg[0].text;

					//string_free((void *) cmd.arg[0].string);

					/* Don't change savefile name.  If the UI wants it changed, they can do it. XXX (Good idea?) */
					Files.process_player_name(false);
			    }
			    /* Various not-specific-to-birth commands. */
			    else if (cmd.command == Command_Code.HELP)
			    {
					throw new NotImplementedException();
					//char buf[80];

					//strnfmt(buf, sizeof(buf), "birth.txt");
					//screen_save();
					//show_file(buf, null, 0, 0);
					//screen_load();
			    }
			    else if (cmd.command == Command_Code.QUIT)
			    {
			        Utilities.quit();
			    }
			}

			roll_hp();

			Squelch.birth_init();

			/* Clear old messages, add new starting message */
			History.clear();
			History.add("Began the quest to destroy Morgoth.", History.PLAYER_BIRTH, null);

			/* Reset message prompt (i.e. no extraneous -more-s) */
			Term.msg_flag = true;

			/* Note player birth in the message recall */
			Message.add(" ", Message_Type.MSG_GENERIC);
			Message.add("  ", Message_Type.MSG_GENERIC);
			Message.add("====================", Message_Type.MSG_GENERIC);
			Message.add("  ", Message_Type.MSG_GENERIC);
			Message.add(" ", Message_Type.MSG_GENERIC);

			/* Give the player some money */
			get_money();

			/* Outfit the player, if they can sell the stuff */
			if (!Option.birth_no_selling.value) player_outfit(Player.Player.instance);

			/* Initialise the stores */
			Store.reset();

			/* Now we're really done.. */
			Game_Event.signal(Game_Event.Event_Type.LEAVE_BIRTH);
		}

	}
}
