using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Monster;
using CSAngband.Object;

namespace CSAngband.Player {
	partial class Player {
		//A reference to itself for all to see.
		//I assume p_ptr is a global pointer to player
		//p_ptr occurs too many times to find it's definition
		//If p_ptr is not a global pointer to the player, then this is wrong.
		private static Player m_instance;
		public static Player instance {
			get {
				if(m_instance == null) {
					m_instance = new Player();
				}

				return m_instance;
			}
			set {
				m_instance = value;
			}
		}

		//Various things
		public static bool character_generated;	/* The character exists */
		public static bool character_dungeon;	/* The character has a dungeon */
		public static bool character_saved;		/* The character was just saved to a savefile */

		//TODO: Make some sort of algorithm for this
		/*
		 * Base experience levels, may be adjusted up for race and/or class
		 */
		static public long[] player_exp =
		{
			10,			25,			45,			70,			100,		140,		200,
			280,		380,		500,		650,		850,		1100,		1400,		
			1800,		2300,		2900,		3600,		4400,		5400,		6800,
			8400,		10200,		12500,		17500,		25000,		35000L,		50000L,
			75000L,		100000L,	150000L,	200000L,	275000L,	350000L,	450000L,
			550000L,	700000L,	850000L,	1000000L,	1250000L,	1500000L,	1800000L,
			2100000L,	2400000L,	2700000L,	3000000L,	3500000L,	4000000L,	4500000L,
			5000000L
		};

		public Int16 py;			/* Player location */
		public Int16 px;			/* Player location */

		public byte psex;			/* Sex index */
		public byte oops;			/* Unused */

		public Player_Sex Sex;
		public Player_Race Race;
		public Player_Class Class;

		public byte hitdie;		/* Hit dice (sides) */
		public byte expfact;		/* Experience factor */

		public Int16 age;			/* Characters age */
		public Int16 ht;			/* Height */
		public Int16 wt;			/* Weight */
		public Int16 sc;			/* Social Class */

		public Int32 au;			/* Current Gold */

		public Int16 max_depth;		/* Max depth */
		public Int16 depth;			/* Cur depth */

		public Int16 max_lev;		/* Max level */
		public Int16 lev;			/* Cur level */

		public Int32 max_exp;		/* Max experience */
		public Int32 exp;			/* Cur experience */
		public UInt16 exp_frac;		/* Cur exp frac (times 2^16) */

		public Int16 mhp;			/* Max hit pts */
		public Int16 chp;			/* Cur hit pts */
		public UInt16 chp_frac;		/* Cur hit frac (times 2^16) */

		public Int16 msp;			/* Max mana pts */
		public Int16 csp;			/* Cur mana pts */
		public UInt16 csp_frac;		/* Cur mana frac (times 2^16) */

		public Int16[] stat_max = new Int16[(int)Stat.Max];	/* Current "maximal" stat values */
		public Int16[] stat_cur = new Int16[(int)Stat.Max];	/* Current "natural" stat values */

		public Int16[] timed = new Int16[(int)Timed_Effect.MAX];	/* Timed effects */

		public Int16 word_recall;	/* Word of recall counter */

		public Int16 energy;		/* Current energy */

		public Int16 food;			/* Current nutrition */

		public byte confusing;		/* Glowing hands */
		public byte searching;		/* Currently searching */
		public byte unignoring;	/* Unignoring */

		public int[] spell_flags = new int[Misc.PY_MAX_SPELLS]; /* Spell flags */

		public byte[] spell_order = new byte[Misc.PY_MAX_SPELLS];	/* Spell order */

		public Int16[] player_hp = new Int16[Misc.PY_MAX_LEVEL];	/* HP Array */

		public string died_from;		/* Cause of death */ //Max length = 80?
		public string history;

		public UInt16 total_winner;		/* Total winner */
		public UInt16 panic_save;		/* Panic save */

		public UInt16 noscore;			/* Cheating flags */

		public bool is_dead;			/* Player is dead */

		public bool wizard;			/* Player is in wizard mode */


		/*** Temporary fields ***/

		public bool playing;			/* true if player is playing */
		public bool leaving;			/* true if player is leaving */
		public bool autosave;          /* true if autosave is pending */
		public bool randarts;			/* true if randarts have been loaded/generated */

		public bool create_up_stair;	/* Create up stair on next level */
		public bool create_down_stair;	/* Create down stair on next level */

		public Int32 total_weight;		/* Total weight being carried */

		public Int16 inven_cnt;			/* Number of items in inventory */
		public Int16 equip_cnt;			/* Number of items in equipment */

		public Int16 health_who;		/* Health bar trackee */

		public Int16 monster_race_idx;	/* Monster race trackee */

		public Int16 object_idx;    /* Object trackee */
		public Int16 object_kind_idx;	/* Object kind trackee */

		public Int16 energy_use;		/* Energy use this turn */

		public Int16 resting;			/* Resting counter */
		public Int16 running;			/* Running counter */
		public bool running_withpathfind;      /* Are we using the pathfinder ? */
		public bool running_firststep;  /* Is this our first step running? */

		public Int16 run_cur_dir;		/* Direction we are running */
		public Int16 run_old_dir;		/* Direction we came from */
		public bool run_unused;		/* Unused (padding field) */
		public bool run_open_area;		/* Looking for an open area */
		public bool run_break_right;	/* Looking for a break (right) */
		public bool run_break_left;	/* Looking for a break (left) */

		public Int16 command_arg;		/* Gives argument of current command 
						   (generally a repeat count) */
		public Int16 command_wrk;		/* Used by the UI to decide whether
						   to start off showing equipment or
						   inventory listings when offering
						   a choice.  See obj/obj-ui.c*/

		public Int16 new_spells;		/* Number of spells available */

		public bool cumber_armor;	/* Mana draining armor */
		public bool cumber_glove;	/* Mana draining gloves */

		public Int16 cur_light;		/* Radius of light (if any) */

		public long notice;		/* Bit flags for pending "special" actions to 
								   carry out after the current "action", 
								   such as reordering inventory, squelching, 
								   etc. */
		public UInt32 update;		/* Bit flags for recalculations needed after
					   this "action", such as HP, or visible area */
		public UInt32 redraw;	        /* Bit flags for things that /have/ changed,
									   and just need to be redrawn by the UI,
									   such as HP, Speed, etc.*/

		public UInt32 total_energy;	/* Total energy used (including resting) */
		public UInt32 resting_turn;	/* Number of player turns spent resting */

		/* Generation fields (for quick start) */
		public Int32 au_birth;          /* Birth gold when option birth_money is false */
		public Int16[] stat_birth = new Int16[(int)Stat.Max]; /* Birth "natural" stat values */
		public Int16 ht_birth;          /* Birth Height */
		public Int16 wt_birth;          /* Birth Weight */
		public Int16 sc_birth;		/* Birth social class */

		/* Variable and calculatable player state */
		public Player_State state;

		/* "cached" quiver statistics*/
		public UInt16 quiver_size;
		public UInt16 quiver_slots;
		public UInt16 quiver_remainder;

		public Object.Object[] inventory;

		public Player() {
			instance = this;
			bool keep_randarts = false;

			inventory = null;

			/* Preserve p_ptr.randarts so that players can use loaded randarts even
			 * if they create a completely different character */
			if (randarts)
				keep_randarts = true;

			/* Wipe the player */
			//(void)WIPE(p, struct player); //Already have a clean slate

			randarts = keep_randarts;

			//Enable below else later

			/* Start with no artifacts made yet */
			for (int i = 0; Misc.z_info != null && i < Misc.z_info.a_max; i++)
			{
				Artifact a_ptr = Misc.a_info[i];
				if(a_ptr == null)
					continue;
				a_ptr.created = false;
				a_ptr.seen = false;
			}


			/* Start with no quests */
			for (int i = 0; Misc.q_list != null && i < Misc.MAX_Q_IDX; i++)
			{
				if(Misc.q_list[i] == null)
					Misc.q_list[i] = new Quest();
				Misc.q_list[i].level = 0;
			}

			if (Misc.q_list != null) {
				Misc.q_list[0].level = 99;
				Misc.q_list[1].level = 100;
			}

			for (int i = 1; Misc.z_info != null && i < Misc.z_info.k_max; i++) {
				Object.Object_Kind k_ptr = Misc.k_info[i];
				if(k_ptr == null)
					continue;
				k_ptr.tried = false;
				k_ptr.aware = false;
			}

			for (int i = 1; Misc.z_info != null && i < Misc.z_info.r_max; i++)
			{
				Monster_Race r_ptr = Misc.r_info[i];
				Monster_Lore l_ptr = Misc.l_list[i];
				if(r_ptr == null || l_ptr == null)
					continue;
				r_ptr.cur_num = 0;
				r_ptr.max_num = 100;
				if (r_ptr.flags.has(Monster_Flag.UNIQUE.value))
					r_ptr.max_num = 1;
				l_ptr.pkills = 0;
			}


			/* Hack -- no ghosts */
			if (Misc.z_info != null && Misc.r_info[Misc.z_info.r_max-1] != null)
				Misc.r_info[Misc.z_info.r_max-1].max_num = 0;


			/* Always start with a well fed player (this is surely in the wrong fn) */
			food = Misc.PY_FOOD_FULL - 1;


			/* None of the spells have been learned yet */
			for (int i = 0; i < Misc.PY_MAX_SPELLS; i++)
				spell_order[i] = 99;

			inventory = new Object.Object[Misc.ALL_INVEN_TOTAL];
			for(int i = 0; i < Misc.ALL_INVEN_TOTAL; i++) {
				inventory[i] = new Object.Object();
			}
			
			/* First turn. */
			Misc.turn = 1;
			total_energy = 0;
			resting_turn = 0;
			/* XXX default race/class */
			Race = Misc.races;
			Class = Misc.classes;
		}

		public bool stat_inc(Stat istat) {
			int stat = (int)istat;
			int v = stat_cur[stat];

			if(v >= 18 + 100)
				return false;
			if(v < 18) {
				stat_cur[stat]++;
			} else if(v < 18 + 90) {
				int gain = (((18 + 100) - v) / 2 + 3) / 2;
				if(gain < 1)
					gain = 1;
				stat_cur[stat] += (short)(Random.randint1((int)gain) + gain / 2);
				if(stat_cur[stat] > 18 + 99)
					stat_cur[stat] = 18 + 99;
			} else {
				stat_cur[stat] = 18 + 100;
			}

			if(stat_cur[stat] > stat_max[stat])
				stat_max[stat] = stat_cur[stat];

			update |= (int)Misc.PU_BONUS;
			return true;
		}

		public bool stat_dec(Stat istat, bool permanent) {
			int stat = (int)istat;
			int cur, max;
			bool res = false;

			cur = stat_cur[stat];
			max = stat_max[stat];

			if(cur > 18 + 10)
				cur -= 10;
			else if(cur > 18)
				cur = 18;
			else if(cur > 3)
				cur -= 1;

			res = (cur != stat_cur[stat]);

			if(permanent) {
				if(max > 18 + 10)
					max -= 10;
				else if(max > 18)
					max = 18;
				else if(max > 3)
					max -= 1;

				res = (max != stat_max[stat]);
			}

			if(res) {
				stat_cur[stat] = (short)cur;
				stat_max[stat] = (short)max;
				update |= (int)(Misc.PU_BONUS);
				redraw |= (int)(Misc.PR_STATS);
			}

			return res;
		}

		public void exp_gain(Int32 amount){
			exp += amount;
			if (exp < max_exp)
				max_exp += amount / 10;
			adjust_level(true);
		}
		
		public void exp_lose(Int32 amount, bool permanent){
			if (exp < amount)
				amount = exp;
			exp -= amount;
			if (permanent)
				max_exp -= amount;
			adjust_level(true);
		}

		//TODO: Enable once we have options
		
		public ConsoleColor hp_attr(){
			if(chp >= mhp)
				return ConsoleColor.Green;
			else if(chp > (mhp * Player_Other.instance.hitpoint_warn) / 10)
				return ConsoleColor.Yellow;
			else
				return ConsoleColor.Red;
		}
		
		public ConsoleColor sp_attr(){
			ConsoleColor attr;
	
			if (csp >= msp)
				attr = ConsoleColor.Green;
			else if (csp > (msp * Player_Other.instance.hitpoint_warn) / 10)
				attr = ConsoleColor.Yellow;
			else
				attr = ConsoleColor.Red;
	
			return attr;
		}

		private void adjust_level(bool verbose)
		{
			if (exp < 0)
				exp = 0;

			if (max_exp < 0)
				max_exp = 0;

			if (exp > Misc.PY_MAX_EXP)
				exp = (int)Misc.PY_MAX_EXP;

			if (max_exp > Misc.PY_MAX_EXP)
				max_exp = (int)Misc.PY_MAX_EXP;

			if (exp > max_exp)
				max_exp = exp;

			redraw |= (int)Misc.PR_EXP;

			//TODO: enable
			//handle_stuff(p);

			while ((lev > 1) &&
				   (exp < (player_exp[lev-2] *
								  expfact / 100L)))
				lev--;


			while ((lev < Misc.PY_MAX_LEVEL) &&
				   (exp >= (player_exp[lev-1] *
								   expfact / 100L)))
			{
				string buf; //Max char = 80?

				lev++;

				/* Save the highest level */
				if (lev > max_lev)
					max_lev = lev;

				if (verbose)
				{
					//TODO: Enable once we have history
					/* Log level updates */
					buf = "Reached level " + lev;
					//history_add(buf, HISTORY_GAIN_LEVEL, 0);

					/* Message */
					//msgt(MSG_LEVEL, "Welcome to level %d.",	p.lev);
				}

				/* Add to social class */
				sc += (short)Random.randint1(2);
				if (sc > 150)
					sc = 150;

				//TODO: Enable once we have spells2.c
				/*
				do_res_stat(Stat.Str);
				do_res_stat(Stat.Int);
				do_res_stat(Stat.Wis);
				do_res_stat(Stat.Dex);
				do_res_stat(Stat.Con);
				do_res_stat(Stat.Chr);
				 * */
			}

			while((max_lev < Misc.PY_MAX_LEVEL) && (max_exp >= (player_exp[max_lev - 1] * expfact / 100L))) {
				max_lev++;
			}

			update |= (int)(Misc.PU_BONUS | Misc.PU_HP | Misc.PU_MANA | Misc.PU_SPELLS);
			redraw |= (int)(Misc.PR_LEV | Misc.PR_TITLE | Misc.PR_EXP | Misc.PR_STATS);

			//TODO: enable
			//handle_stuff(p);
		}

		/*
		 * This fleshes out a full player based on the choices currently made,
		 * and so is called whenever things like race or class are chosen.
		 */
		public void generate(Player_Sex s, Player_Race r, Player_Class c)
		{
			if (s == null) s = Misc.sex_info[psex];
			if (c == null) c = Class;
			if (r == null) r = Race;

			Sex = s;
			Class = c;
			Race = r;

			// Level 1
			max_lev = lev = 1;

			// Experience factor
			expfact = (byte)(Race.r_exp + Class.c_exp);

			// Hitdice
			hitdie = (byte)(Race.r_mhp + Class.c_mhp);

			// Initial hitpoints
			mhp = hitdie;

			// Pre-calculate level 1 hitdice
			player_hp[0] = hitdie;

			// Roll for age/height/weight
			get_ahw();

			//HACK - get_history relies on a pointer, so we have to pack and unpack SC...
			if(Race.history != null) {
				history = Race.history.get_history(out sc);
			} else {
				history = "";
			}

			sc_birth = sc;
		}

		
		/*
		 * Computes character's age, height, and weight
		 */
		void get_ahw()
		{
			/* Calculate the age */
			age = (short)(Race.b_age + Random.randint1(Race.m_age));

			/* Calculate the height/weight for males */
			if (psex == Misc.SEX_MALE)
			{
				ht = ht_birth = Random.Rand_normal(Race.m_b_ht, Race.m_m_ht);
				wt = wt_birth = Random.Rand_normal(Race.m_b_wt, Race.m_m_wt);
			}

			/* Calculate the height/weight for females */
			else if (psex == Misc.SEX_FEMALE)
			{
				ht = ht_birth = Random.Rand_normal(Race.f_b_ht, Race.f_m_ht);
				wt = wt_birth = Random.Rand_normal(Race.f_b_wt, Race.f_m_wt);
			}
		}

		/* Is the player capable of casting a spell? */
		public static bool can_cast()
		{
			if (Misc.p_ptr.Class.spell_book == 0)
			{
				Utilities.msg("You cannot pray or produce magics.");
				return false;
			}

			if (Misc.p_ptr.timed[(int)Timed_Effect.BLIND] != 0 || Cave.no_light())
			{
				Utilities.msg("You cannot see!");
				return false;
			}

			if (Misc.p_ptr.timed[(int)Timed_Effect.CONFUSED] != 0)
			{
				Utilities.msg("You are too confused!");
				return false;
			}

			return true;
		}

		/* Is the player capable of studying? */
		public static bool can_study()
		{
			if (!can_cast())
				return false;

			if (Misc.p_ptr.new_spells == 0)
			{
				string p = ((Misc.p_ptr.Class.spell_book == TVal.TV_MAGIC_BOOK) ? "spell" : "prayer");
				Utilities.msg("You cannot learn any new {0}s!", p);
				return false;
			}

			return true;
		}

		/* Determine if the player can read scrolls. */
		public static bool can_read()
		{
			if (Misc.p_ptr.timed[(int)Timed_Effect.BLIND] != 0)
			{
				Utilities.msg("You can't see anything.");
				return false;
			}

			if (Cave.no_light())
			{
				Utilities.msg("You have no light to read by.");
				return false;
			}

			if (Misc.p_ptr.timed[(int)Timed_Effect.CONFUSED] != 0)
			{
				Utilities.msg("You are too confused to read!");
				return false;
			}

			if (Misc.p_ptr.timed[(int)Timed_Effect.AMNESIA] != 0)
			{
				Utilities.msg("You can't remember how to read!");
				return false;
			}

			return true;
		}

		/* Determine if the player can fire with the bow */
		static public bool can_fire()
		{
			Object.Object o_ptr = Misc.p_ptr.inventory[Misc.INVEN_BOW];

			//Require a usable launcher
			if (o_ptr.tval == 0 || Misc.p_ptr.state.ammo_tval == 0)
			{
				Utilities.msg("You have nothing to fire with.");
				return false;
			}

			return true;
		}

		/*
		 * Apply confusion, if needed, to a direction
		 *
		 * Display a message and return true if direction changes.
		 */
		//dp was an int*, I just made it an int
		public bool confuse_dir(ref int dp, bool too)
		{
			int dir = dp;

			if (timed[(int)Timed_Effect.CONFUSED] != 0)
				if ((dir == 5) || (Random.randint0(100) < 75))
					// Random direction
					dir = Misc.ddd[Random.randint0(8)];

			if (dp != dir) {
				if (too)
					Utilities.msg("You are too confused.");
				else
					Utilities.msg("You are confused.");

				dp = dir;
				return true;
			}

			return false;
		}

		/**
		 * Clear the timed effect `idx`.  Mention this if `notify` is true.
		 */
		public bool clear_timed(Timed_Effect idx, bool notify)
		{
			return set_timed(idx, 0, notify);
		}

		/*
		 * Set a timed event (except timed resists, cutting and stunning).
		 */
		public bool set_timed(Timed_Effect idx, int v, bool notify)
		{
			/* Hack -- Force good values */
			v = (v > 10000) ? 10000 : (v < 0) ? 0 : v;
			if ((idx < 0) || (idx > Timed_Effect.MAX)) return false;

			/* No change */
			if (timed[(int)idx] == v) return false;

			/* Hack -- call other functions */
			if (idx == Timed_Effect.STUN) return set_stun(this, v);
			else if (idx == Timed_Effect.CUT) return set_cut(this, v);

			/* Don't mention effects which already match the player state. */
			if (idx == Timed_Effect.OPP_ACID && check_state(Object_Flag.IM_ACID, state.flags))
			    notify = false;
			else if (idx == Timed_Effect.OPP_ELEC && check_state(Object_Flag.IM_ELEC, state.flags))
			    notify = false;
			else if (idx == Timed_Effect.OPP_FIRE && check_state(Object_Flag.IM_FIRE, state.flags))
			    notify = false;
			else if (idx == Timed_Effect.OPP_COLD && check_state(Object_Flag.IM_COLD, state.flags))
			    notify = false;
			else if (idx == Timed_Effect.OPP_CONF && state.flags.has(Object_Flag.RES_CONFU.value))
			    notify = false;

			/* Find the effect */
			timed_effect effect = effects[(int)idx];

			/* Turning off, always mention */
			if (v == 0)
			{
			    Utilities.msgt(Message_Type.MSG_RECOVER, "{0}", effect.on_end);
			    notify = true;
			}

			/* Turning on, always mention */
			else if (timed[(int)idx] == 0)
			{
			    Utilities.msgt(effect.msg, "{0}", effect.on_begin);
			    notify = true;
			}

			else if (notify)
			{
			    /* Decrementing */
			    if (timed[(int)idx] > v && effect.on_decrease != null)
			        Utilities.msgt(effect.msg, "{0}", effect.on_decrease);

			    /* Incrementing */
			    else if (v > timed[(int)idx] && effect.on_increase != null)
			        Utilities.msgt(effect.msg, "{0}", effect.on_increase);
			}

			/* Use the value */
			timed[(int)idx] = (short)v;

			/* Sort out the sprint effect */
			if (idx == Timed_Effect.SPRINT && v == 0)
			    inc_timed(Timed_Effect.SLOW, 100, true, false);

			/* Nothing to notice */
			if (!notify) return false;

			/* Disturb */
			if (Option.disturb_state.value) Cave.disturb(this, 0, 0);

			/* Update the visuals, as appropriate. */
			update |= effect.flag_update;
			redraw |= (Misc.PR_STATUS | effect.flag_redraw);

			/* Handle stuff */
			handle_stuff();

			/* Result */
			return true;
		}


		/*
		 * Obtain the "flags" for the player as if he was an item
		 */
		public static void player_flags(ref Bitflag f)
		{
			if(f == null) {
			    f = new Bitflag(Object.Object_Flag.SIZE);
			}
			/* Add racial flags */
			f.copy(instance.Race.flags);

			/* Some classes become immune to fear at a certain plevel */
			if (instance.player_has(Misc.PF.BRAVERY_30.value) && instance.lev >= 30)
			    f.on(Object.Object_Flag.RES_FEAR.value);
		}

		/**
		 * Determine whether an object flag or its timed equivalent are set in the
		 * passed-in flags (which probably come from a state structure). This assumes
		 * that there are no p_ptr.timed effects which can be active yet unknown to
		 * the player.
		 *
		 * \param p player to act upon
		 * \param flag is the object flag for which we are checking.
		 * \param f is the set of flags we're checking
		 */
		public bool check_state(Object.Object_Flag flag, Bitflag f)
		{
			Object_Flag of_ptr = Object_Flag.list[flag.value];

			/* Sanity check */
			if (flag == null) return false;

			if (f.has(flag.value) || (of_ptr.timed != (Timed_Effect)0 && timed[(int)of_ptr.timed] > 0))
			    return true;

			return false;
		}


		public bool player_has(int flag){
			return (Race.pflags.has(flag) || Class.pflags.has(flag));
		}

	}
}
