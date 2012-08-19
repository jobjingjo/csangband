using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;

namespace CSAngband.Player {
	partial class Player {
		/*
		 * Handle "update"
		 */
		public void update_stuff()
		{
			/* Update stuff */
			if (update == 0) return;


			if ((update & (Misc.PU_BONUS)) != 0)
			{
			    update &= ~(Misc.PU_BONUS);
			    update_bonuses();
			}

			if ((update & (Misc.PU_TORCH)) != 0)
			{
			    update &= ~(Misc.PU_TORCH);
			    calc_torch();
			}

			if ((update & (Misc.PU_HP)) != 0)
			{
			    update &= ~(Misc.PU_HP);
			    calc_hitpoints();
			}

			if ((update & (Misc.PU_MANA)) != 0)
			{
			    update &= ~(Misc.PU_MANA);
			    calc_mana();
			}

			if ((update & (Misc.PU_SPELLS)) != 0)
			{
			    update &= (uint)~(Misc.PU_SPELLS);
			    calc_spells();
			}


			/* Character is not ready yet, no screen updates */
			if (!character_generated) return;


			/* Character is in "icky" mode, no screen updates */
			if (Misc.character_icky != 0) return;


			//Nick: I feel like everything below this point should be a function call to cave.
			if ((update & (Misc.PU_FORGET_VIEW)) != 0)
			{
				update &= ~(Misc.PU_FORGET_VIEW);
				Cave.forget_view();
			}

			if ((update & (Misc.PU_UPDATE_VIEW)) != 0)
			{
				update &= ~(Misc.PU_UPDATE_VIEW);
				Cave.update_view();
			}


			if ((update & (Misc.PU_FORGET_FLOW)) != 0)
			{
				update &= ~(Misc.PU_FORGET_FLOW);
				Cave.cave_forget_flow(Cave.cave);
			}

			if ((update & (Misc.PU_UPDATE_FLOW)) != 0)
			{
				update &= ~(Misc.PU_UPDATE_FLOW);
				Cave.cave_update_flow(Cave.cave);
			}


			if ((update & (Misc.PU_DISTANCE)) != 0)
			{
				update &= ~(Misc.PU_DISTANCE);
				update &= ~(Misc.PU_MONSTERS);
				Monster.Monster.update_monsters(true);
			}

			if ((update & (Misc.PU_MONSTERS)) != 0)
			{
				update &= ~(Misc.PU_MONSTERS);
				Monster.Monster.update_monsters(false);
			}


			if ((update & (Misc.PU_PANEL)) != 0)
			{
				update &= ~(Misc.PU_PANEL);
				Game_Event.signal(Game_Event.Event_Type.PLAYERMOVED);
			}
		}

		static string get_spell_name(int tval, int spell)
		{
			if (tval == TVal.TV_MAGIC_BOOK)
				return Misc.s_info[spell].name;
			else
				return Misc.s_info[spell + Misc.PY_MAX_SPELLS].name;
		}

		/*
		 * Calculate number of spells player should have, and forget,
		 * or remember, spells until that number is properly reflected.
		 *
		 * Note that this function induces various "status" messages,
		 * which must be bypasses until the character is created.
		 */
		static void calc_spells()
		{
			int i, j, k, levels;
			int num_allowed, num_known;
			int percent_spells;

			Magic_Type s_ptr;

			short old_spells;

			string p = ((instance.Class.spell_book == TVal.TV_MAGIC_BOOK) ? "spell" : "prayer");

			/* Hack -- must be literate */
			if (instance.Class.spell_book == 0) return;

			/* Hack -- wait for creation */
			if (!character_generated) return;

			/* Hack -- handle "xtra" mode */
			if (Misc.character_xtra != 0) return;

			/* Save the new_spells value */
			old_spells = instance.new_spells;

			/* Determine the number of spells allowed */
			levels = instance.lev - instance.Class.spell_first + 1;

			/* Hack -- no negative spells */
			if (levels < 0) levels = 0;

			/* Number of 1/100 spells per level */
			percent_spells = adj_mag_study[instance.state.stat_ind[instance.Class.spell_stat]];

			/* Extract total allowed spells (rounded up) */
			num_allowed = (((percent_spells * levels) + 50) / 100);

			/* Assume none known */
			num_known = 0;

			/* Count the number of spells we know */
			for (j = 0; j < Misc.PY_MAX_SPELLS; j++)
			{
			    /* Count known spells */
			    if ((instance.spell_flags[j] & Misc.PY_SPELL_LEARNED) != 0)
			    {
			        num_known++;
			    }
			}

			/* See how many spells we must forget or may learn */
			instance.new_spells = (short)(num_allowed - num_known);

			/* Forget spells which are too hard */
			for (i = Misc.PY_MAX_SPELLS - 1; i >= 0; i--)
			{
			    /* Get the spell */
			    j = instance.spell_order[i];

			    /* Skip non-spells */
			    if (j >= 99) continue;

			    /* Get the spell */
			    s_ptr = instance.Class.spells.info[j];

			    /* Skip spells we are allowed to know */
			    if (s_ptr.slevel <= instance.lev) continue;

			    /* Is it known? */
			    if ((instance.spell_flags[j] & Misc.PY_SPELL_LEARNED) != 0)
			    {
			        /* Mark as forgotten */
			        instance.spell_flags[j] |= (char)Misc.PY_SPELL_FORGOTTEN;

			        /* No longer known */
			        instance.spell_flags[j] &= (~(Misc.PY_SPELL_LEARNED));

			        /* Message */
			        Utilities.msg("You have forgotten the %s of %s.", p,
			                   get_spell_name(instance.Class.spell_book, j));

			        /* One more can be learned */
			        instance.new_spells++;
			    }
			}

			/* Forget spells if we know too many spells */
			for (i = Misc.PY_MAX_SPELLS - 1; i >= 0; i--)
			{
			    /* Stop when possible */
			    if (instance.new_spells >= 0) break;

			    /* Get the (i+1)th spell learned */
			    j = instance.spell_order[i];

			    /* Skip unknown spells */
			    if (j >= 99) continue;

			    /* Forget it (if learned) */
			    if ((instance.spell_flags[j] & Misc.PY_SPELL_LEARNED) != 0)
			    {
			        /* Mark as forgotten */
			        instance.spell_flags[j] |= Misc.PY_SPELL_FORGOTTEN;

			        /* No longer known */
			        instance.spell_flags[j] &= ~Misc.PY_SPELL_LEARNED;

			        /* Message */
			        Utilities.msg("You have forgotten the %s of %s.", p,
			                   get_spell_name(instance.Class.spell_book, j));

			        /* One more can be learned */
			        instance.new_spells++;
			    }
			}

			/* Check for spells to remember */
			for (i = 0; i < Misc.PY_MAX_SPELLS; i++)
			{
			    /* None left to remember */
			    if (instance.new_spells <= 0) break;

			    /* Get the next spell we learned */
			    j = instance.spell_order[i];

			    /* Skip unknown spells */
			    if (j >= 99) break;

			    /* Get the spell */
			    s_ptr = instance.Class.spells.info[j];

			    /* Skip spells we cannot remember */
			    if (s_ptr.slevel > instance.lev) continue;

			    /* First set of spells */
			    if ((instance.spell_flags[j] & Misc.PY_SPELL_FORGOTTEN) != 0)
			    {
			        /* No longer forgotten */
			        instance.spell_flags[j] &= ~Misc.PY_SPELL_FORGOTTEN;

			        /* Known once more */
			        instance.spell_flags[j] |= Misc.PY_SPELL_LEARNED;

			        /* Message */
			        Utilities.msg("You have remembered the %s of %s.",
			                   p, get_spell_name(instance.Class.spell_book, j));

			        /* One less can be learned */
			        instance.new_spells--;
			    }
			}

			/* Assume no spells available */
			k = 0;

			/* Count spells that can be learned */
			for (j = 0; j < Misc.PY_MAX_SPELLS; j++)
			{
			    /* Get the spell */
			    s_ptr = instance.Class.spells.info[j];

			    /* Skip spells we cannot remember or don't exist */
			    if (s_ptr.slevel > instance.lev || s_ptr.slevel == 0) continue;

			    /* Skip spells we already know */
			    if ((instance.spell_flags[j] & Misc.PY_SPELL_LEARNED) != 0)
			    {
			        continue;
			    }

			    /* Count it */
			    k++;
			}

			/* Cannot learn more spells than exist */
			if (instance.new_spells > k) instance.new_spells = (short)k;

			/* Spell count changed */
			if (old_spells != instance.new_spells)
			{
			    /* Message if needed */
			    if (instance.new_spells > 0)
			    {
			        /* Message */
			        Utilities.msg("You can learn %d more %s%s.",
			                   instance.new_spells, p,
			                   (instance.new_spells != 1) ? "s" : "");
			    }

			    /* Redraw Study Status */
			    instance.redraw |= (Misc.PR_STUDY | Misc.PR_OBJECT);
			}
		}


		/*
		 * Calculate maximum mana.  You do not need to know any spells.
		 * Note that mana is lowered by heavy (or inappropriate) armor.
		 *
		 * This function induces status messages.
		 */
		static void calc_mana()
		{
			int msp, levels, cur_wgt, max_wgt;

			Object.Object o_ptr;

			bool old_cumber_glove = instance.cumber_glove;
			bool old_cumber_armor = instance.cumber_armor;

			/* Hack -- Must be literate */
			if (instance.Class.spell_book == 0)
			{
			    instance.msp = 0;
			    instance.csp = 0;
			    instance.csp_frac = 0;
			    return;
			}

			/* Extract "effective" player level */
			levels = (instance.lev - instance.Class.spell_first) + 1;
			if (levels > 0)
			{
			    msp = 1;
			    msp += (short)(adj_mag_mana[instance.state.stat_ind[instance.Class.spell_stat]] * levels / 100);
			}
			else
			{
			    levels = 0;
			    msp = 0;
			}

			/* Process gloves for those disturbed by them */
			if (instance.player_has(Misc.PF.CUMBER_GLOVE.value))
			{
			    Bitflag f = new Bitflag(Object_Flag.SIZE);

			    /* Assume player is not encumbered by gloves */
			    instance.cumber_glove = false;

			    /* Get the gloves */
			    o_ptr = instance.inventory[Misc.INVEN_HANDS];

			    /* Examine the gloves */
			    o_ptr.object_flags(ref f);

			    /* Normal gloves hurt mage-type spells */
			    if (o_ptr.kind != null && !f.has(Object_Flag.FREE_ACT.value) && 
					!f.has(Object_Flag.SPELLS_OK.value) && !(f.has(Object_Flag.DEX.value) && 
					(o_ptr.pval[o_ptr.which_pval(Object_Flag.DEX.value)] > 0)))
			    {
			        /* Encumbered */
			        instance.cumber_glove = true;

			        /* Reduce mana */
			        msp = (3 * msp) / 4;
			    }
			}

			/* Assume player not encumbered by armor */
			instance.cumber_armor = false;

			/* Weigh the armor */
			cur_wgt = 0;
			cur_wgt += instance.inventory[Misc.INVEN_BODY].weight;
			cur_wgt += instance.inventory[Misc.INVEN_HEAD].weight;
			cur_wgt += instance.inventory[Misc.INVEN_ARM].weight;
			cur_wgt += instance.inventory[Misc.INVEN_OUTER].weight;
			cur_wgt += instance.inventory[Misc.INVEN_HANDS].weight;
			cur_wgt += instance.inventory[Misc.INVEN_FEET].weight;

			/* Determine the weight allowance */
			max_wgt = instance.Class.spell_weight;

			/* Heavy armor penalizes mana */
			if (((cur_wgt - max_wgt) / 10) > 0)
			{
			    /* Encumbered */
			    instance.cumber_armor = true;

			    /* Reduce mana */
			    msp -= ((cur_wgt - max_wgt) / 10);
			}

			/* Mana can never be negative */
			if (msp < 0) msp = 0;

			/* Maximum mana has changed */
			if (msp != instance.msp)
			{
			    /* Save new limit */
			    instance.msp = (short)msp;

			    /* Enforce new limit */
			    if (instance.csp >= msp)
			    {
			        instance.csp = (short)msp;
			        instance.csp_frac = 0;
			    }

			    /* Display mana later */
			    instance.redraw |= (Misc.PR_MANA);
			}

			/* Hack -- handle "xtra" mode */
			if (Misc.character_xtra != 0) return;

			/* Take note when "glove state" changes */
			if (old_cumber_glove != instance.cumber_glove)
			{
			    /* Message */
			    if (instance.cumber_glove)
			    {
			        Utilities.msg("Your covered hands feel unsuitable for spellcasting.");
			    }
			    else
			    {
			        Utilities.msg("Your hands feel more suitable for spellcasting.");
			    }
			}

			/* Take note when "armor state" changes */
			if (old_cumber_armor != instance.cumber_armor)
			{
			    /* Message */
			    if (instance.cumber_armor)
			    {
			        Utilities.msg("The weight of your armor encumbers your movement.");
			    }
			    else
			    {
			        Utilities.msg("You feel able to move more freely.");
			    }
			}
		}


		/*
		 * Calculate the players (maximal) hit points
		 *
		 * Adjust current hitpoints if necessary
		 */
		void calc_hitpoints()
		{
			long bonus;
			int new_max;

			/* Get "1/100th hitpoint bonus per level" value */
			bonus = adj_con_mhp[state.stat_ind[(int)Stat.Con]];

			/* Calculate hitpoints */
			if(lev == 0) {
				new_max = 1; //You be a newb
			} else {
				new_max = (int)(player_hp[lev - 1] + (bonus * lev / 100));
			}

			/* Always have at least one hitpoint per level */
			if (new_max < lev + 1) mhp = (short)(lev + 1);

			/* New maximum hitpoints */
			if (mhp != new_max)
			{
				/* Save new limit */
				mhp = (short)new_max;

				/* Enforce new limit */
				if (chp >= new_max)
				{
					chp = mhp;
					chp_frac = 0;
				}

				/* Display hitpoints (later) */
				redraw |= (Misc.PR_HP);
			}
		}


		/*
		 * Calculate and set the current light radius.
		 *
		 * The brightest wielded object counts as the light source; radii do not add
		 * up anymore.
		 *
		 * Note that a cursed light source no longer emits light.
		 */
		void calc_torch()
		{
			int i;

			short old_light = cur_light;
			bool burn_light = true;

			short new_light = 0;
			int extra_light = 0;

			/* Ascertain lightness if in the town */
			if (depth == 0 && ((Misc.turn % (10L * Cave.TOWN_DAWN)) < ((10L * Cave.TOWN_DAWN) / 2)))
			    burn_light = false;

			/* Examine all wielded objects, use the brightest */
			for (i = Misc.INVEN_WIELD; i < Misc.INVEN_TOTAL; i++)
			{
			    Bitflag f = new Bitflag(Object_Flag.SIZE);

			    int amt = 0;
			    Object.Object o_ptr = inventory[i];

			    /* Skip empty slots */
			    if (o_ptr.kind == null) continue;

			    /* Extract the flags */
			    o_ptr.object_flags(ref f);

			    /* Cursed objects emit no light */
			    if (f.has(Object_Flag.LIGHT_CURSE.value))
			        amt = 0;

			    /* Examine actual lights */
			    else if (o_ptr.tval == TVal.TV_LIGHT)
			    {
			        int flag_inc = f.has(Object_Flag.LIGHT.value) ? 1 : 0;

			        /* Artifact lights provide permanent bright light */
			        if (o_ptr.artifact != null)
			            amt = 3 + flag_inc;

			        /* Non-artifact lights and those without fuel provide no light */
			        else if (!burn_light || o_ptr.timeout == 0)
			            amt = 0;

			        /* All lit lights provide at least radius 2 light */
			        else
			        {
			            amt = 2 + flag_inc;

			            /* Torches below half fuel provide less light */
			            if (o_ptr.sval == SVal.SV_LIGHT_TORCH && o_ptr.timeout < (Misc.FUEL_TORCH / 4)) //Nick: This looks like 1/4th...
			                amt--;
			        }
			    }

			    else
			    {
			        /* LIGHT flag on an non-cursed non-lights always increases radius */
			        if (f.has(Object_Flag.LIGHT.value)) extra_light++;
			    }

			    /* Alter cur_light if reasonable */
			    if (new_light < amt)
			        new_light = (short)amt;
			}

			/* Add bonus from LIGHT flags */
			new_light += (short)extra_light;

			/* Limit light */
			new_light = (short)Math.Min((int)new_light, 5);
			new_light = (short)Math.Max((int)new_light, 0);

			/* Notice changes in the "light radius" */
			if (old_light != new_light)
			{
			    /* Update the visuals */
			    cur_light = new_light;
			    update |= (Misc.PU_UPDATE_VIEW | Misc.PU_MONSTERS);
			}
		}

		/*
		 * Calculate the blows a player would get.
		 *
		 * \param o_ptr is the object for which we are calculating blows
		 * \param state is the player state for which we are calculating blows
		 * \param extra_blows is the number of +blows available from this object and
		 * this state
		 *
		 * N.B. state.num_blows is now 100x the number of blows.
		 */
		public int calc_blows(Object.Object obj, Player_State state, int extra_blows)
		{
			int blows;
			int str_index, dex_index;
			int div;
			int blow_energy;

			if(obj == null)
				return 0;

			/* Enforce a minimum "weight" (tenth pounds) */
			div = ((obj.weight < Class.min_weight) ? (int)Class.min_weight :
			    obj.weight);

			/* Get the strength vs weight */
			str_index = adj_str_blow[state.stat_ind[(int)Stat.Str]] *
			        Class.att_multiply / div;

			/* Maximal value */
			if (str_index > 11) str_index = 11;

			/* Index by dexterity */
			dex_index = Math.Min((int)adj_dex_blow[state.stat_ind[(int)Stat.Dex]], 11);

			/* Use the blows table to get energy per blow */
			blow_energy = blows_table[str_index, dex_index];

			blows = Math.Min((10000 / blow_energy), (100 * Class.max_attacks));

			/* Require at least one blow */
			return Math.Max(blows + (100 * extra_blows), 100);
		}

		/*
		 * Calculate the players current "state", taking into account
		 * not only race/class intrinsics, but also objects being worn
		 * and temporary spell effects.
		 *
		 * See also calc_mana() and calc_hitpoints().
		 *
		 * Take note of the new "speed code", in particular, a very strong
		 * player will start slowing down as soon as he reaches 150 pounds,
		 * but not until he reaches 450 pounds will he be half as fast as
		 * a normal kobold.  This both hurts and helps the player, hurts
		 * because in the old days a player could just avoid 300 pounds,
		 * and helps because now carrying 300 pounds is not very painful.
		 *
		 * The "weapon" and "bow" do *not* add to the bonuses to hit or to
		 * damage, since that would affect non-combat things.  These values
		 * are actually added in later, at the appropriate place.
		 *
		 * If id_only is true, calc_bonuses() will only use the known
		 * information of objects; thus it returns what the player _knows_
		 * the character state to be.
		 */
		void calc_bonuses(Object.Object[] inventory, ref Player_State state, bool id_only)
		{
			Player p_ptr = Player.instance;
			int i, j, hold;

			int extra_blows = 0;
			int extra_shots = 0;
			int extra_might = 0;

			Object.Object o_ptr;

			Bitflag f = new Bitflag(Object_Flag.SIZE);
			Bitflag collect_f = new Bitflag(Object_Flag.SIZE);

			/*** Reset ***/
			state = new Player_State();
			//memset(state, 0, sizeof *state);

			/* Set various defaults */
			state.speed = 110;
			state.num_blows = 100;


			/*** Extract race/class info ***/

			/* Base infravision (purely racial) */
			if(p_ptr.Race == null)
				return;
			state.see_infra = p_ptr.Race.infra;

			/* Base skills */
			for (i = 0; i < (int)Skill.MAX; i++)
			    state.skills[i] = (short)(p_ptr.Race.r_skills[i] + p_ptr.Class.c_skills[i]);


			/*** Analyze player ***/

			/* Extract the player flags */
			Player.player_flags(ref collect_f);


			/*** Analyze equipment ***/

			/* Scan the equipment */
			for (i = Misc.INVEN_WIELD; i < Misc.INVEN_TOTAL; i++)
			{
			    o_ptr = inventory[i];

			    /* Skip non-objects */
			    if (o_ptr ==  null || o_ptr.kind == null) continue;

			    /* Extract the item flags */
			    if (id_only)
			        o_ptr.object_flags_known(ref f);
			    else
			        o_ptr.object_flags(ref f);

			    collect_f.union(f);

			    /* Affect stats */
			    if (f.has(Object_Flag.STR.value)) state.stat_add[(int)Stat.Str] +=
			        o_ptr.pval[o_ptr.which_pval(Object_Flag.STR.value)];
			    if (f.has(Object_Flag.INT.value)) state.stat_add[(int)Stat.Int] +=
			        o_ptr.pval[o_ptr.which_pval(Object_Flag.INT.value)];
			    if (f.has(Object_Flag.WIS.value)) state.stat_add[(int)Stat.Wis] +=
			        o_ptr.pval[o_ptr.which_pval(Object_Flag.WIS.value)];
			    if (f.has(Object_Flag.DEX.value)) state.stat_add[(int)Stat.Dex] +=
			        o_ptr.pval[o_ptr.which_pval(Object_Flag.DEX.value)];
			    if (f.has(Object_Flag.CON.value)) state.stat_add[(int)Stat.Con] +=
			        o_ptr.pval[o_ptr.which_pval(Object_Flag.CON.value)];
			    if (f.has(Object_Flag.CHR.value)) state.stat_add[(int)Stat.Chr] +=
			        o_ptr.pval[o_ptr.which_pval(Object_Flag.CHR.value)];

			    /* Affect stealth */
			    if (f.has(Object_Flag.STEALTH.value)) state.skills[(int)Skill.STEALTH] +=
			        o_ptr.pval[o_ptr.which_pval(Object_Flag.STEALTH.value)];

			    /* Affect searching ability (factor of five) */
			    if (f.has(Object_Flag.SEARCH.value)) state.skills[(int)Skill.SEARCH] +=
			        (short)(o_ptr.pval[o_ptr.which_pval(Object_Flag.SEARCH.value)] * 5);

			    /* Affect searching frequency (factor of five) */
			    if (f.has(Object_Flag.SEARCH.value)) state.skills[(int)Skill.SEARCH_FREQUENCY] += 
					(short)(o_ptr.pval[o_ptr.which_pval(Object_Flag.SEARCH.value)] * 5);

			    /* Affect infravision */
			    if (f.has(Object_Flag.INFRA.value)) state.see_infra +=
			        o_ptr.pval[o_ptr.which_pval(Object_Flag.INFRA.value)];

			    /* Affect digging (factor of 20) */
			    if (f.has(Object_Flag.TUNNEL.value)) state.skills[(int)Skill.DIGGING] +=
			        (short)(o_ptr.pval[o_ptr.which_pval(Object_Flag.TUNNEL.value)] * 20);

			    /* Affect speed */
			    if (f.has(Object_Flag.SPEED.value)) state.speed +=
			        o_ptr.pval[o_ptr.which_pval(Object_Flag.SPEED.value)];

			    /* Affect blows */
			    if (f.has(Object_Flag.BLOWS.value)) extra_blows +=
			        o_ptr.pval[o_ptr.which_pval(Object_Flag.BLOWS.value)];

			    /* Affect shots */
			    if (f.has(Object_Flag.SHOTS.value)) extra_shots +=
			        o_ptr.pval[o_ptr.which_pval(Object_Flag.SHOTS.value)];

			    /* Affect Might */
			    if (f.has(Object_Flag.MIGHT.value)) extra_might +=
			        o_ptr.pval[o_ptr.which_pval(Object_Flag.MIGHT.value)];

			    /* Modify the base armor class */
			    state.ac += o_ptr.ac;

			    /* The base armor class is always known */
			    state.dis_ac += o_ptr.ac;

			    /* Apply the bonuses to armor class */
			    if (!id_only || o_ptr.is_known())
			        state.to_a += o_ptr.to_a;

			    /* Apply the mental bonuses to armor class, if known */
			    if (o_ptr.defence_plusses_are_visible())
			        state.dis_to_a += o_ptr.to_a;

			    /* Hack -- do not apply "weapon" bonuses */
			    if (i == Misc.INVEN_WIELD) continue;

			    /* Hack -- do not apply "bow" bonuses */
			    if (i == Misc.INVEN_BOW) continue;

			    /* Apply the bonuses to hit/damage */
			    if (!id_only || o_ptr.is_known())
			    {
			        state.to_h += o_ptr.to_h;
			        state.to_d += o_ptr.to_d;
			    }

			    /* Apply the mental bonuses tp hit/damage, if known */
			    if (o_ptr.attack_plusses_are_visible())
			    {
			        state.dis_to_h += o_ptr.to_h;
			        state.dis_to_d += o_ptr.to_d;
			    }
			}


			/*** Update all flags ***/

			for (i = 0; i < Object_Flag.MAX.value; i++)
			    if (collect_f.has(i))
			        state.flags.on(i);


			/*** Handle stats ***/

			/* Calculate stats */
			for (i = 0; i < (int)Stat.Max; i++)
			{
			    int add, top, use, ind;

			    /* Extract modifier */
			    add = state.stat_add[i];

			    /* Maximize mode */
			    if (Option.birth_maximize.value)
			    {
			        /* Modify the stats for race/class */
			        add += (p_ptr.Race.r_adj[i] + p_ptr.Class.c_adj[i]);
			    }

			    /* Extract the new "stat_top" value for the stat */
			    top = Birther.modify_stat_value(p_ptr.stat_max[i], add);

			    /* Save the new value */
			    state.stat_top[i] = (short)top;

			    /* Extract the new "stat_use" value for the stat */
			    use = Birther.modify_stat_value(p_ptr.stat_cur[i], add);

			    /* Save the new value */
			    state.stat_use[i] = (short)use;

			    /* Values: n/a */
			    if (use <= 3) ind = 0;

			    /* Values: 3, 4, ..., 18 */
			    else if (use <= 18) ind = (use - 3);

			    /* Ranges: 18/00-18/09, ..., 18/210-18/219 */
			    else if (use <= 18+219) ind = (15 + (use - 18) / 10);

			    /* Range: 18/220+ */
			    else ind = (37);

			    Misc.assert((0 <= ind) && (ind < Misc.STAT_RANGE));

			    /* Save the new index */
			    state.stat_ind[i] = (short)ind;
			}


			/*** Temporary flags ***/

			/* Apply temporary "stun" */
			if (p_ptr.timed[(int)Timed_Effect.STUN] > 50)
			{
			    state.to_h -= 20;
			    state.dis_to_h -= 20;
			    state.to_d -= 20;
			    state.dis_to_d -= 20;
			    state.skills[(int)Skill.DEVICE] = (short)(state.skills[(int)Skill.DEVICE] * 8 / 10);
			}
			else if (p_ptr.timed[(int)Timed_Effect.STUN] != 0)
			{
			    state.to_h -= 5;
			    state.dis_to_h -= 5;
			    state.to_d -= 5;
			    state.dis_to_d -= 5;
			    state.skills[(int)Skill.DEVICE] = (short)(state.skills[(int)Skill.DEVICE] * 9 / 10);
			}

			/* Invulnerability */
			if (p_ptr.timed[(int)Timed_Effect.INVULN] != 0)
			{
			    state.to_a += 100;
			    state.dis_to_a += 100;
			}

			/* Temporary blessing */
			if (p_ptr.timed[(int)Timed_Effect.BLESSED] != 0)
			{
			    state.to_a += 5;
			    state.dis_to_a += 5;
			    state.to_h += 10;
			    state.dis_to_h += 10;
			    state.skills[(int)Skill.DEVICE] = (short) (state.skills[(int)Skill.DEVICE] * 105 / 100);
			}

			/* Temporary shield */
			if (p_ptr.timed[(int)Timed_Effect.SHIELD] != 0)
			{
			    state.to_a += 50;
			    state.dis_to_a += 50;
			}

			/* Temporary stoneskin */
			if (p_ptr.timed[(int)Timed_Effect.STONESKIN] != 0)
			{
			    state.to_a += 40;
			    state.dis_to_a += 40;
			    state.speed -= 5;
			}

			/* Temporary "Hero" */
			if (p_ptr.timed[(int)Timed_Effect.HERO] != 0)
			{
			    state.to_h += 12;
			    state.dis_to_h += 12;
			    state.skills[(int)Skill.DEVICE] = (short)(state.skills[(int)Skill.DEVICE] * 105 / 100);
			}

			/* Temporary "Berserk" */
			if (p_ptr.timed[(int)Timed_Effect.SHERO] != 0)
			{
			    state.to_h += 24;
			    state.dis_to_h += 24;
			    state.to_a -= 10;
			    state.dis_to_a -= 10;
			    state.skills[(int)Skill.DEVICE] = (short)(state.skills[(int)Skill.DEVICE] * 9 / 10);
			}

			/* Temporary "fast" */
			if (p_ptr.timed[(int)Timed_Effect.FAST] != 0 || p_ptr.timed[(int)Timed_Effect.SPRINT] != 0)
			    state.speed += 10;

			/* Temporary "slow" */
			if (p_ptr.timed[(int)Timed_Effect.SLOW] != 0)
			    state.speed -= 10;

			/* Temporary infravision boost */
			if (p_ptr.timed[(int)Timed_Effect.SINFRA] != 0)
			    state.see_infra += 5;

			/* Terror - this is necessary because TMD_AFRAID already occupies the 
			 * of_ptr.timed slot for Object_Flag.AFRAID */
			if (p_ptr.timed[(int)Timed_Effect.TERROR] > p_ptr.timed[(int)Timed_Effect.AFRAID])
			    p_ptr.timed[(int)Timed_Effect.AFRAID] = p_ptr.timed[(int)Timed_Effect.TERROR];

			if (p_ptr.timed[(int)Timed_Effect.TERROR] != 0)
			    state.speed += 5;

			/* Fear can come from item flags too */
			if (p_ptr.check_state(Object_Flag.AFRAID, p_ptr.state.flags))
			{
			    state.to_h -= 20;
			    state.dis_to_h -= 20;
			    state.to_a += 8;
			    state.dis_to_a += 8;
			    state.skills[(int)Skill.DEVICE] = (short)(state.skills[(int)Skill.DEVICE] * 95 / 100);
			}

			/* Confusion */
			if (p_ptr.timed[(int)Timed_Effect.CONFUSED] != 0)
			    state.skills[(int)Skill.DEVICE] = (short)(state.skills[(int)Skill.DEVICE] * 75 / 100);

			/* Amnesia */
			if (p_ptr.timed[(int)Timed_Effect.AMNESIA] != 0)
			    state.skills[(int)Skill.DEVICE] = (short)(state.skills[(int)Skill.DEVICE] * 8 / 10);

			/* Poison */
			if (p_ptr.timed[(int)Timed_Effect.POISONED] != 0)
			    state.skills[(int)Skill.DEVICE] = (short)(state.skills[(int)Skill.DEVICE] * 95 / 100);

			/* Hallucination */
			if (p_ptr.timed[(int)Timed_Effect.IMAGE] != 0)
			    state.skills[(int)Skill.DEVICE] = (short)(state.skills[(int)Skill.DEVICE] * 8 / 10);

			/*** Analyze weight ***/

			/* Extract the current weight (in tenth pounds) */
			j = p_ptr.total_weight;

			/* Extract the "weight limit" (in tenth pounds) */
			i = state.weight_limit();

			/* Apply "encumbrance" from weight */
			if (j > i / 2) state.speed -= (short)((j - (i / 2)) / (i / 10));

			/* Bloating slows the player down (a little) */
			if (p_ptr.food >= Misc.PY_FOOD_MAX) state.speed -= 10;

			/* Searching slows the player down */
			if (p_ptr.searching != 0) state.speed -= 10;

			/* Sanity check on extreme speeds */
			if (state.speed < 0) state.speed = 0;
			if (state.speed > 199) state.speed = 199;

			/*** Apply modifier bonuses ***/

			/* Actual Modifier Bonuses (Un-inflate stat bonuses) */
			state.to_a += (short)((int)(adj_dex_ta[state.stat_ind[(int)Stat.Dex]]) - 128);
			state.to_d += (short)((int)(adj_str_td[state.stat_ind[(int)Stat.Str]]) - 128);
			state.to_h += (short)((int)(adj_dex_th[state.stat_ind[(int)Stat.Dex]]) - 128);
			state.to_h += (short)((int)(adj_str_th[state.stat_ind[(int)Stat.Str]]) - 128);

			/* Displayed Modifier Bonuses (Un-inflate stat bonuses) */
			state.dis_to_a += (short)((int)(adj_dex_ta[state.stat_ind[(int)Stat.Dex]]) - 128);
			state.dis_to_d += (short)((int)(adj_str_td[state.stat_ind[(int)Stat.Str]]) - 128);
			state.dis_to_h += (short)((int)(adj_dex_th[state.stat_ind[(int)Stat.Dex]]) - 128);
			state.dis_to_h += (short)((int)(adj_str_th[state.stat_ind[(int)Stat.Str]]) - 128);


			/*** Modify skills ***/

			/* Affect Skill -- stealth (bonus one) */
			state.skills[(int)Skill.STEALTH] += 1;

			/* Affect Skill -- disarming (DEX and INT) */
			state.skills[(int)Skill.DISARM] += adj_dex_dis[state.stat_ind[(int)Stat.Dex]];
			state.skills[(int)Skill.DISARM] += adj_int_dis[state.stat_ind[(int)Stat.Int]];

			/* Affect Skill -- magic devices (INT) */
			state.skills[(int)Skill.DEVICE] += adj_int_dev[state.stat_ind[(int)Stat.Int]];

			/* Affect Skill -- saving throw (WIS) */
			state.skills[(int)Skill.SAVE] += adj_wis_sav[state.stat_ind[(int)Stat.Wis]];

			/* Affect Skill -- digging (STR) */
			state.skills[(int)Skill.DIGGING] += adj_str_dig[state.stat_ind[(int)Stat.Str]];

			/* Affect Skills (Level, by Class) */
			for (i = 0; i < (int)Skill.MAX; i++)
			    state.skills[i] += (short)(p_ptr.Class.x_skills[i] * p_ptr.lev / 10);

			/* Limit Skill -- digging from 1 up */
			if (state.skills[(int)Skill.DIGGING] < 1) state.skills[(int)Skill.DIGGING] = 1;

			/* Limit Skill -- stealth from 0 to 30 */
			if (state.skills[(int)Skill.STEALTH] > 30) state.skills[(int)Skill.STEALTH] = 30;
			if (state.skills[(int)Skill.STEALTH] < 0) state.skills[(int)Skill.STEALTH] = 0;

			/* Apply Skill -- Extract noise from stealth */
			state.noise = (uint)(1L << (30 - state.skills[(int)Skill.STEALTH]));

			/* Obtain the "hold" value */
			hold = adj_str_hold[state.stat_ind[(int)Stat.Str]];


			/*** Analyze current bow ***/

			/* Examine the "current bow" */
			o_ptr = inventory[Misc.INVEN_BOW];

			/* Assume not heavy */
			state.heavy_shoot = false;

			/* It is hard to hold a heavy bow */
			if (hold < o_ptr.weight / 10)
			{
			    /* Hard to wield a heavy bow */
			    state.to_h += (short)(2 * (hold - o_ptr.weight / 10));
			    state.dis_to_h += (short)(2 * (hold - o_ptr.weight / 10));

			    /* Heavy Bow */
			    state.heavy_shoot = true;
			}

			/* Analyze launcher */
			if (o_ptr.kind != null)
			{
			    /* Get to shoot */
			    state.num_shots = 1;

			    /* Analyze the launcher */
			    switch (o_ptr.sval)
			    {
			        /* Sling and ammo */
			        case SVal.SV_SLING:
			        {
			            state.ammo_tval = TVal.TV_SHOT;
			            state.ammo_mult = 2;
			            break;
			        }

			        /* Short Bow and Arrow */
			        case SVal.SV_SHORT_BOW:
			        {
			            state.ammo_tval = TVal.TV_ARROW;
			            state.ammo_mult = 2;
			            break;
			        }

			        /* Long Bow and Arrow */
			        case SVal.SV_LONG_BOW:
			        {
			            state.ammo_tval = TVal.TV_ARROW;
			            state.ammo_mult = 3;
			            break;
			        }

			        /* Light Crossbow and Bolt */
			        case SVal.SV_LIGHT_XBOW:
			        {
			            state.ammo_tval = TVal.TV_BOLT;
			            state.ammo_mult = 3;
			            break;
			        }

			        /* Heavy Crossbow and Bolt */
			        case SVal.SV_HEAVY_XBOW:
			        {
			            state.ammo_tval = TVal.TV_BOLT;
			            state.ammo_mult = 4;
			            break;
			        }
			    }

			    /* Apply special flags */
			    if (o_ptr.kind != null && !state.heavy_shoot)
			    {
			        /* Extra shots */
			        state.num_shots += (short)extra_shots;

			        /* Extra might */
			        state.ammo_mult += (byte)extra_might;

			        /* Hack -- Rangers love Bows */
			        if (player_has(Misc.PF.EXTRA_SHOT.value) && (state.ammo_tval == TVal.TV_ARROW))
			        {
			            /* Extra shot at level 20 */
			            if (p_ptr.lev >= 20) state.num_shots++;

			            /* Extra shot at level 40 */
			            if (p_ptr.lev >= 40) state.num_shots++;
			        }
			    }

			    /* Require at least one shot */
			    if (state.num_shots < 1) state.num_shots = 1;
			}


			/*** Analyze weapon ***/

			/* Examine the "current weapon" */
			o_ptr = inventory[Misc.INVEN_WIELD];

			/* Assume not heavy */
			state.heavy_wield = false;

			/* It is hard to hold a heavy weapon */
			if (hold < o_ptr.weight / 10)
			{
			    /* Hard to wield a heavy weapon */
			    state.to_h += (short)(2 * (hold - o_ptr.weight / 10));
			    state.dis_to_h += (short)(2 * (hold - o_ptr.weight / 10));

			    /* Heavy weapon */
			    state.heavy_wield = true;
			}

			/* Non-object means barehanded attacks */
			if (o_ptr.kind == null)
			    Misc.assert(o_ptr.weight == 0);

			/* Normal weapons */
			if (!state.heavy_wield)
			{
			    /* Calculate number of blows */
			    state.num_blows = (short)calc_blows(o_ptr, state, extra_blows);

			    /* Boost digging skill by weapon weight */
				state.skills[(int)Skill.DIGGING] += (short)(o_ptr.weight / 10);
			}

			/* Assume okay */
			state.icky_wield = false;

			/* Priest weapon penalty for non-blessed edged weapons */
			if (player_has(Misc.PF.BLESS_WEAPON.value) && !p_ptr.check_state(Object_Flag.BLESSED, p_ptr.state.flags) &&
			    ((o_ptr.tval == TVal.TV_SWORD) || (o_ptr.tval == TVal.TV_POLEARM)))
			{
			    /* Reduce the real bonuses */
			    state.to_h -= 2;
			    state.to_d -= 2;

			    /* Reduce the mental bonuses */
			    state.dis_to_h -= 2;
			    state.dis_to_d -= 2;

			    /* Icky weapon */
			    state.icky_wield = true;
			}

			return;
		}

		/*
		 * Calculate bonuses, and print various things on changes.
		 */
		void update_bonuses()
		{
			int i;

			if(state == null) {
				state = new Player_State();
			}
			Player_State old = new Player_State(state);


			/*** Calculate bonuses ***/

			calc_bonuses(inventory, ref state, false);


			/*** Notice changes ***/

			/* Analyze stats */
			for (i = 0; i < (int)Stat.Max; i++)
			{
			    /* Notice changes */
			    if (state.stat_top[i] != old.stat_top[i])
			    {
			        /* Redisplay the stats later */
			        redraw |= (Misc.PR_STATS);
			    }

			    /* Notice changes */
			    if (state.stat_use[i] != old.stat_use[i])
			    {
			        /* Redisplay the stats later */
			        redraw |= (Misc.PR_STATS);
			    }

			    /* Notice changes */
			    if (state.stat_ind[i] != old.stat_ind[i])
			    {
			        /* Change in CON affects Hitpoints */
			        if (i == (int)Stat.Con)
			        {
			            update |= (Misc.PU_HP);
			        }

			        /* Change in INT may affect Mana/Spells */
			        else if (i == (int)Stat.Int)
			        {
			            if (Class.spell_stat == (int)Stat.Int)
			            {
			                update |= (Misc.PU_MANA | Misc.PU_SPELLS);
			            }
			        }

			        /* Change in WIS may affect Mana/Spells */
			        else if (i == (int)Stat.Wis)
			        {
			            if (Class.spell_stat == (int)Stat.Wis)
			            {
			                update |= (Misc.PU_MANA | Misc.PU_SPELLS);
			            }
			        }
			    }
			}


			/* Hack -- Telepathy Change */
			if (state.flags.has(Object_Flag.TELEPATHY.value) != old.flags.has(Object_Flag.TELEPATHY.value))
			{
			    /* Update monster visibility */
			    update |= (Misc.PU_MONSTERS);
			}

			/* Hack -- See Invis Change */
			if (state.flags.has(Object_Flag.SEE_INVIS.value) != old.flags.has(Object_Flag.SEE_INVIS.value))
			{
			    /* Update monster visibility */
			    update |= (Misc.PU_MONSTERS);
			}

			/* Redraw speed (if needed) */
			if (state.speed != old.speed)
			{
			    /* Redraw speed */
			    redraw |= (Misc.PR_SPEED);
			}

			/* Redraw armor (if needed) */
			if ((state.dis_ac != old.dis_ac) || (state.dis_to_a != old.dis_to_a))
			{
			    /* Redraw */
			    redraw |= (Misc.PR_ARMOR);
			}

			/* Hack -- handle "xtra" mode */
			if (Misc.character_xtra != 0) return;

			/* Take note when "heavy bow" changes */
			if (old.heavy_shoot != state.heavy_shoot)
			{
			    /* Message */
			    if (state.heavy_shoot)
			        Utilities.msg("You have trouble wielding such a heavy bow.");
			    else if (inventory[Misc.INVEN_BOW].kind != null)
			        Utilities.msg("You have no trouble wielding your bow.");
			    else
			        Utilities.msg("You feel relieved to put down your heavy bow.");
			}

			/* Take note when "heavy weapon" changes */
			if (old.heavy_wield != state.heavy_wield)
			{
			    /* Message */
			    if (state.heavy_wield)
			        Utilities.msg("You have trouble wielding such a heavy weapon.");
			    else if (inventory[Misc.INVEN_WIELD].kind != null)
			        Utilities.msg("You have no trouble wielding your weapon.");
			    else
			        Utilities.msg("You feel relieved to put down your heavy weapon.");	
			}

			/* Take note when "illegal weapon" changes */
			if (old.icky_wield != state.icky_wield)
			{
			    /* Message */
			    if (state.icky_wield)
			        Utilities.msg("You do not feel comfortable with your weapon.");
			    else if (inventory[Misc.INVEN_WIELD].kind != null)
			        Utilities.msg("You feel comfortable with your weapon.");
			    else
			        Utilities.msg("You feel more comfortable after removing your weapon.");
			}
		}

		class flag_event_trigger
		{
			public flag_event_trigger(uint f, Game_Event.Event_Type e){
				flag = f;
				evt =e;
			}
			public uint flag;
			public Game_Event.Event_Type evt;
		};



		/*
		 * Events triggered by the various flags.
		 */
		static flag_event_trigger[] redraw_events = new flag_event_trigger[]
		{
			new flag_event_trigger( Misc.PR_MISC,    Game_Event.Event_Type.RACE_CLASS ),
			new flag_event_trigger( Misc.PR_TITLE,   Game_Event.Event_Type.PLAYERTITLE ),
			new flag_event_trigger( Misc.PR_LEV,     Game_Event.Event_Type.PLAYERLEVEL ),
			new flag_event_trigger( Misc.PR_EXP,     Game_Event.Event_Type.EXPERIENCE ),
			new flag_event_trigger( Misc.PR_STATS,   Game_Event.Event_Type.STATS ),
			new flag_event_trigger( Misc.PR_ARMOR,   Game_Event.Event_Type.AC ),
			new flag_event_trigger( Misc.PR_HP,      Game_Event.Event_Type.HP ),
			new flag_event_trigger( Misc.PR_MANA,    Game_Event.Event_Type.MANA ),
			new flag_event_trigger( Misc.PR_GOLD,    Game_Event.Event_Type.GOLD ),
			new flag_event_trigger( Misc.PR_HEALTH,  Game_Event.Event_Type.MONSTERHEALTH ),
			new flag_event_trigger( Misc.PR_DEPTH,   Game_Event.Event_Type.DUNGEONLEVEL ),
			new flag_event_trigger( Misc.PR_SPEED,   Game_Event.Event_Type.PLAYERSPEED ),
			new flag_event_trigger( Misc.PR_STATE,   Game_Event.Event_Type.STATE ),
			new flag_event_trigger( Misc.PR_STATUS,  Game_Event.Event_Type.STATUS ),
			new flag_event_trigger( Misc.PR_STUDY,   Game_Event.Event_Type.STUDYSTATUS ),
			new flag_event_trigger( Misc.PR_DTRAP,   Game_Event.Event_Type.DETECTIONSTATUS ),
			new flag_event_trigger( Misc.PR_BUTTONS, Game_Event.Event_Type.MOUSEBUTTONS ),

			new flag_event_trigger( Misc.PR_INVEN,   Game_Event.Event_Type.INVENTORY ),
			new flag_event_trigger( Misc.PR_EQUIP,   Game_Event.Event_Type.EQUIPMENT ),
			new flag_event_trigger( Misc.PR_MONLIST, Game_Event.Event_Type.MONSTERLIST ),
			new flag_event_trigger( Misc.PR_ITEMLIST, Game_Event.Event_Type.ITEMLIST ),
			new flag_event_trigger( Misc.PR_MONSTER, Game_Event.Event_Type.MONSTERTARGET ),
			new flag_event_trigger( Misc.PR_OBJECT, Game_Event.Event_Type.OBJECTTARGET ),
			new flag_event_trigger( Misc.PR_MESSAGE, Game_Event.Event_Type.MESSAGE )
		};

		/*
		 * Handle "p_ptr.redraw"
		 */
		public void redraw_stuff()
		{
			/* Redraw stuff */
			if (redraw == 0) return;

			/* Character is not ready yet, no screen updates */
			if (!character_generated) return;

			/* Character is in "icky" mode, no screen updates */
			if (Misc.character_icky != 0) return;

			/* For each listed flag, send the appropriate signal to the UI */
			for (int i = 0; i < redraw_events.Length; i++)
			{
				flag_event_trigger hnd = redraw_events[i];

				if (redraw != 0 & hnd.flag != 0)
					Game_Event.signal(hnd.evt);
			}

			/* Then the ones that require parameters to be supplied. */
			if ((redraw & Misc.PR_MAP) != 0)
			{
				/* Mark the whole map to be redrawn */
				Game_Event.signal_point(Game_Event.Event_Type.MAP, -1, -1);
			}

			redraw = 0;

			/*
			 * Do any plotting, etc. delayed from earlier - this set of updates
			 * is over.
			 */
			Game_Event.signal(Game_Event.Event_Type.END);
		}

		/*** Generic "deal with" functions ***/

		/*
		 * Handle "p_ptr.notice"
		 */
		public void notice_stuff()
		{
			/* Notice stuff */
			if (notice == 0) return;


			/* Deal with autoinscribe stuff */
			if ((notice & Misc.PN_AUTOINSCRIBE) != 0)
			{
				notice &= (long)(~(Misc.PN_AUTOINSCRIBE));
				throw new NotImplementedException();
				//autoinscribe_pack();
				//autoinscribe_ground();
			}

			/* Deal with squelch stuff */
			if ((notice & Misc.PN_SQUELCH) != 0)
			{
				notice &= ~(Misc.PN_SQUELCH);
				Squelch.drop();
			}

			/* Combine the pack */
			if ((notice & Misc.PN_COMBINE) != 0)
			{
				notice &= ~(Misc.PN_COMBINE);
				Object.Object.combine_pack();
			}

			/* Reorder the pack */
			if ((notice & Misc.PN_REORDER) != 0)
			{
				notice &= ~(Misc.PN_REORDER);
				Object.Object.reorder_pack();
			}

			/* Sort the quiver */
			if ((notice & Misc.PN_SORT_QUIVER)!=0)
			{
				notice &= ~(Misc.PN_SORT_QUIVER);
				Object.Object.sort_quiver();
			}

			/* Dump the monster messages */
			if ((notice & Misc.PN_MON_MESSAGE)!=0)
			{
				notice &= ~(Misc.PN_MON_MESSAGE);
				throw new NotImplementedException();
				/* Make sure this comes after all of the monster messages */
				//flush_all_monster_messages();
			}
		}

		/*
		 * Handle "p_ptr.update" and "p_ptr.redraw"
		 */
		public void handle_stuff()
		{
			if (update != 0) update_stuff();
			if (redraw != 0) redraw_stuff();
		}
	}
}
