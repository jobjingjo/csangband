﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Player {
	partial class Player {
		/*
		 * Set "p_ptr.food", notice observable changes
		 *
		 * The "p_ptr.food" variable can get as large as 20000, allowing the
		 * addition of the most "filling" item, Elvish Waybread, which adds
		 * 7500 food units, without overflowing the 32767 maximum limit.
		 *
		 * Perhaps we should disturb the player with various messages,
		 * especially messages about hunger status changes.  XXX XXX XXX
		 *
		 * Digestion of food is handled in "dungeon.c", in which, normally,
		 * the player digests about 20 food units per 100 game turns, more
		 * when "fast", more when "regenerating", less with "slow digestion",
		 * but when the player is "gorged", he digests 100 food units per 10
		 * game turns, or a full 1000 food units per 100 game turns.
		 *
		 * Note that the player's speed is reduced by 10 units while gorged,
		 * so if the player eats a single food ration (5000 food units) when
		 * full (15000 food units), he will be gorged for (5000/100)*10 = 500
		 * game turns, or 500/(100/5) = 25 player turns (if nothing else is
		 * affecting the player speed).
		 */
		//formerly player_set_food
		public bool set_food(int v)
		{
			int old_aux, new_aux;

			bool notice = false;

			/* Hack -- Force good values */
			v = Math.Min(v, Misc.PY_FOOD_UPPER);
			v = Math.Max(v, 0);

			/* Fainting / Starving */
			if (food < Misc.PY_FOOD_FAINT)
			{
			    old_aux = 0;
			}

			/* Weak */
			else if (food < Misc.PY_FOOD_WEAK)
			{
			    old_aux = 1;
			}

			/* Hungry */
			else if (food < Misc.PY_FOOD_ALERT)
			{
			    old_aux = 2;
			}

			/* Normal */
			else if (food < Misc.PY_FOOD_FULL)
			{
			    old_aux = 3;
			}

			/* Full */
			else if (food < Misc.PY_FOOD_MAX)
			{
			    old_aux = 4;
			}

			/* Gorged */
			else
			{
			    old_aux = 5;
			}

			/* Fainting / Starving */
			if (v < Misc.PY_FOOD_FAINT)
			{
			    new_aux = 0;
			}

			/* Weak */
			else if (v < Misc.PY_FOOD_WEAK)
			{
			    new_aux = 1;
			}

			/* Hungry */
			else if (v < Misc.PY_FOOD_ALERT)
			{
			    new_aux = 2;
			}

			/* Normal */
			else if (v < Misc.PY_FOOD_FULL)
			{
			    new_aux = 3;
			}

			/* Full */
			else if (v < Misc.PY_FOOD_MAX)
			{
			    new_aux = 4;
			}

			/* Gorged */
			else
			{
			    new_aux = 5;
			}

			/* Food increase */
			if (new_aux > old_aux)
			{
			    /* Describe the state */
			    switch (new_aux)
			    {
			        /* Weak */
			        case 1:
			        {
			            Utilities.msg("You are still weak.");
			            break;
			        }

			        /* Hungry */
			        case 2:
			        {
			            Utilities.msg("You are still hungry.");
			            break;
			        }

			        /* Normal */
			        case 3:
			        {
			            Utilities.msg("You are no longer hungry.");
			            break;
			        }

			        /* Full */
			        case 4:
			        {
			            Utilities.msg("You are full!");
			            break;
			        }

			        /* Bloated */
			        case 5:
			        {
			            Utilities.msg("You have gorged yourself!");
			            break;
			        }
			    }

			    /* Change */
			    notice = true;
			}

			/* Food decrease */
			else if (new_aux < old_aux)
			{
			    /* Describe the state */
			    switch (new_aux)
			    {
			        /* Fainting / Starving */
			        case 0:
			        {
			            Utilities.msgt(Message_Type.MSG_NOTICE, "You are getting faint from hunger!");
			            break;
			        }

			        /* Weak */
			        case 1:
			        {
			            Utilities.msgt(Message_Type.MSG_NOTICE, "You are getting weak from hunger!");
			            break;
			        }

			        /* Hungry */
			        case 2:
			        {
			            Utilities.msgt(Message_Type.MSG_HUNGRY, "You are getting hungry.");
			            break;
			        }

			        /* Normal */
			        case 3:
			        {
			            Utilities.msgt(Message_Type.MSG_NOTICE, "You are no longer full.");
			            break;
			        }

			        /* Full */
			        case 4:
			        {
			            Utilities.msgt(Message_Type.MSG_NOTICE, "You are no longer gorged.");
			            break;
			        }
			    }

			    /* Change */
			    notice = true;
			}

			/* Use the value */
			food = (short)v;

			/* Nothing to notice */
			if (!notice) return (false);

			/* Disturb */
			if (Option.disturb_state.value) Cave.disturb(Misc.p_ptr, 0, 0);

			/* Recalculate bonuses */
			update |= (Misc.PU_BONUS);

			/* Redraw hunger */
			redraw |= (Misc.PR_STATUS);

			/* Handle stuff */
			Misc.p_ptr.handle_stuff();

			/* Result */
			return (true);
		}

		/**
		 * Increase the timed effect `idx` by `v`.  Mention this if `notify` is true.
		 * Check for resistance to the effect if `check` is true.
		 */
		//formerly player_inc_timed
		public bool inc_timed(Timed_Effect idx, int v, bool notify, bool check)
		{
			GF effect;

			/* Check we have a valid effect */
			if ((idx < 0) || (idx > Timed_Effect.MAX)) return false;

			/* Find the effect */
			effect = GF.list[(int)idx];

			/* Check that @ can be affected by this effect */
			if (check) {
			    Object.Object.wieldeds_notice_flag(Misc.p_ptr, effect.resist.value);
			    if (check_state(effect.resist, state.flags)) return false;
			}

			/* Paralysis should be non-cumulative */
			if (idx == Timed_Effect.PARALYZED && timed[(int)Timed_Effect.PARALYZED] > 0)
			    return false;

			/* Set v */
			v = v + timed[(int)idx];

			return set_timed(idx, v, notify);
		}

		/**
		 * Decrease the timed effect `idx` by `v`.  Mention this if `notify` is true.
		 */
		public bool dec_timed(Timed_Effect idx, int v, bool notify)
		{
			/* Check we have a valid effect */
			if ((idx < 0) || (idx > Timed_Effect.MAX)) return false;

			/* Set v */
			v = timed[(int)idx] - v;

			return set_timed(idx, v, notify);
		}

		//typedef struct
		//{
		//  const char *on_begin;
		//  const char *on_end;
		//  const char *on_increase;
		//  const char *on_decrease;
		//  u32b flag_redraw, flag_update;
		//  int msg;
		//  int resist;
		//} timed_effect;

		//static timed_effect effects[] =
		//{
		//    { "You feel yourself moving faster!", "You feel yourself slow down.",
		//            null, null,
		//            0, PU_BONUS, MSG_SPEED, 0 },
		//    { "You feel yourself moving slower!", "You feel yourself speed up.",
		//            null, null,
		//            0, PU_BONUS, MSG_SLOW, OF_FREE_ACT },
		//    { "You are blind.", "You blink and your eyes clear.",
		//            null, null,
		//            PR_MAP, PU_FORGET_VIEW | PU_UPDATE_VIEW | PU_MONSTERS, MSG_BLIND,
		//            OF_RES_BLIND },
		//    { "You are paralysed!", "You can move again.",
		//            null, null,
		//            0, 0, MSG_PARALYZED, OF_FREE_ACT },
		//    { "You are confused!", "You are no longer confused.",
		//            "You are more confused!", "You feel a little less confused.",
		//            0, 0, MSG_CONFUSED, OF_RES_CONFU },
		//    { "You are terrified!", "You feel bolder now.",
		//            "You are more scared!", "You feel a little less scared.",
		//            0, PU_BONUS, MSG_AFRAID, OF_RES_FEAR },
		//    { "You feel drugged!", "You can see clearly again.",
		//            "You feel more drugged!", "You feel less drugged.",
		//            PR_MAP | PR_MONLIST | PR_ITEMLIST, 0, MSG_DRUGGED, OF_RES_CHAOS },
		//    { "You are poisoned!", "You are no longer poisoned.",
		//            "You are more poisoned!", "You are less poisoned.",
		//            0, 0, MSG_POISONED, OF_RES_POIS },
		//    { null, null, null, null, 0, 0, 0, 0 },  /* TMD_CUT -- handled seperately */
		//    { null, null, null, null, 0, 0, 0, OF_RES_STUN },  /* TMD_STUN -- handled seperately */
		//    { "You feel safe from evil!", "You no longer feel safe from evil.",
		//            "You feel even safer from evil!", "You feel less safe from evil.",
		//            0, 0, MSG_PROT_EVIL, 0 },
		//    { "You feel invulnerable!", "You feel vulnerable once more.",
		//            null, null,
		//            0, PU_BONUS, MSG_INVULN, 0 },
		//    { "You feel like a hero!", "You no longer feel heroic.",
		//            "You feel more like a hero!", "You feel less heroic.",
		//            0, PU_BONUS, MSG_HERO, 0 },
		//    { "You feel like a killing machine!", "You no longer feel berserk.",
		//            "You feel even more berserk!", "You feel less berserk.",
		//            0, PU_BONUS, MSG_BERSERK, 0 },
		//    { "A mystic shield forms around your body!", "Your mystic shield crumbles away.",
		//            "The mystic shield strengthens.", "The mystic shield weakens.",
		//            0, PU_BONUS, MSG_SHIELD, 0 },
		//    { "You feel righteous!", "The prayer has expired.",
		//            "You feel more righteous!", "You feel less righteous.",
		//            0, PU_BONUS, MSG_BLESSED, 0 },
		//    { "Your eyes feel very sensitive!", "Your eyes no longer feel so sensitive.",
		//            "Your eyes feel more sensitive!", "Your eyes feel less sensitive.",
		//            0, (PU_BONUS | PU_MONSTERS), MSG_SEE_INVIS, 0 },
		//    { "Your eyes begin to tingle!", "Your eyes stop tingling.",
		//            "Your eyes' tingling intensifies.", "Your eyes tingle less.",
		//            0, (PU_BONUS | PU_MONSTERS), MSG_INFRARED, 0 },
		//    { "You feel resistant to acid!", "You are no longer resistant to acid.",
		//            "You feel more resistant to acid!", "You feel less resistant to acid.",
		//            PR_STATUS, 0, MSG_RES_ACID, OF_VULN_ACID },
		//    { "You feel resistant to electricity!", "You are no longer resistant to electricity.",
		//            "You feel more resistant to electricity!", "You feel less resistant to electricity.",
		//            PR_STATUS, 0, MSG_RES_ELEC, OF_VULN_ELEC },
		//    { "You feel resistant to fire!", "You are no longer resistant to fire.",
		//            "You feel more resistant to fire!", "You feel less resistant to fire.",
		//            PR_STATUS, 0, MSG_RES_FIRE, OF_VULN_FIRE },
		//    { "You feel resistant to cold!", "You are no longer resistant to cold.",
		//            "You feel more resistant to cold!", "You feel less resistant to cold.",
		//            PR_STATUS, 0, MSG_RES_COLD, OF_VULN_COLD },
		//    { "You feel resistant to poison!", "You are no longer resistant to poison.",
		//            "You feel more resistant to poison!", "You feel less resistant to poison.",
		//            0, 0, MSG_RES_POIS, 0 },
		//    { "You feel resistant to confusion!", "You are no longer resistant to confusion.",
		//            "You feel more resistant to confusion!", "You feel less resistant to confusion.",
		//            PR_STATUS, PU_BONUS, 0, 0 },
		//    { "You feel your memories fade.", "Your memories come flooding back.",
		//            null, null,
		//            0, 0, MSG_GENERIC, 0 },
		//    { "Your mind expands.", "Your horizons are once more limited.",
		//            "Your mind expands further.", null,
		//            0, PU_BONUS, MSG_GENERIC, 0 },
		//    { "Your skin turns to stone.", "A fleshy shade returns to your skin.",
		//            null, null,
		//            0, PU_BONUS, MSG_GENERIC, 0 },
		//    { "You feel the need to run away, and fast!", "The urge to run dissipates.",
		//            null, null,
		//            0, PU_BONUS, MSG_AFRAID, 0 },
		//    { "You start sprinting.", "You suddenly stop sprinting.",
		//            null, null,
		//            0, PU_BONUS, MSG_SPEED, 0 },
		//    { "You feel bold.", "You no longer feel bold.",
		//            "You feel even bolder!", "You feel less bold.",
		//            0, PU_BONUS, MSG_BOLD, 0 },
		//};

		/*
		 * The "stun" and "cut" statuses need to be handled by special functions of
		 * their own, as they are more complex than the ones handled by the generic
		 * code.
		 */

		/*
		 * Set "p_ptr.timed[TMD_STUN]", notice observable changes
		 *
		 * Note the special code to only notice "range" changes.
		 */
		static bool set_stun(Player p, int v)
		{
			throw new NotImplementedException();
			//int old_aux, new_aux;

			//bool notice = false;

			///* Hack -- Force good values */
			//v = (v > 10000) ? 10000 : (v < 0) ? 0 : v;

			///* Knocked out */
			//if (p.timed[TMD_STUN] > 100)
			//{
			//    old_aux = 3;
			//}

			///* Heavy stun */
			//else if (p.timed[TMD_STUN] > 50)
			//{
			//    old_aux = 2;
			//}

			///* Stun */
			//else if (p.timed[TMD_STUN] > 0)
			//{
			//    old_aux = 1;
			//}

			///* None */
			//else
			//{
			//    old_aux = 0;
			//}

			///* Knocked out */
			//if (v > 100)
			//{
			//    new_aux = 3;
			//}

			///* Heavy stun */
			//else if (v > 50)
			//{
			//    new_aux = 2;
			//}

			///* Stun */
			//else if (v > 0)
			//{
			//    new_aux = 1;
			//}

			///* None */
			//else
			//{
			//    new_aux = 0;
			//}

			///* Increase cut */
			//if (new_aux > old_aux)
			//{
			//    /* Describe the state */
			//    switch (new_aux)
			//    {
			//        /* Stun */
			//        case 1:
			//        {
			//            msgt(MSG_STUN, "You have been stunned.");
			//            break;
			//        }

			//        /* Heavy stun */
			//        case 2:
			//        {
			//            msgt(MSG_STUN, "You have been heavily stunned.");
			//            break;
			//        }

			//        /* Knocked out */
			//        case 3:
			//        {
			//            msgt(MSG_STUN, "You have been knocked out.");
			//            break;
			//        }
			//    }

			//    /* Notice */
			//    notice = true;
			//}

			///* Decrease cut */
			//else if (new_aux < old_aux)
			//{
			//    /* Describe the state */
			//    switch (new_aux)
			//    {
			//        /* None */
			//        case 0:
			//        {
			//            msgt(MSG_RECOVER, "You are no longer stunned.");
			//            if (OPT(disturb_state)) disturb(p_ptr, 0, 0);
			//            break;
			//        }
			//    }

			//    /* Notice */
			//    notice = true;
			//}

			///* Use the value */
			//p.timed[TMD_STUN] = v;

			///* No change */
			//if (!notice) return (false);

			///* Disturb */
			//if (OPT(disturb_state)) disturb(p_ptr, 0, 0);

			///* Recalculate bonuses */
			//p.update |= (PU_BONUS);

			///* Redraw the "stun" */
			//p.redraw |= (PR_STATUS);

			///* Handle stuff */
			//handle_stuff(p_ptr);

			///* Result */
			//return (true);
		}


		/*
		 * Set "p_ptr.timed[TMD_CUT]", notice observable changes
		 *
		 * Note the special code to only notice "range" changes.
		 */
		static bool set_cut(Player p, int v)
		{
			throw new NotImplementedException();
			//int old_aux, new_aux;

			//bool notice = false;

			///* Hack -- Force good values */
			//v = (v > 10000) ? 10000 : (v < 0) ? 0 : v;

			///* Mortal wound */
			//if (p.timed[TMD_CUT] > 1000)
			//{
			//    old_aux = 7;
			//}

			///* Deep gash */
			//else if (p.timed[TMD_CUT] > 200)
			//{
			//    old_aux = 6;
			//}

			///* Severe cut */
			//else if (p.timed[TMD_CUT] > 100)
			//{
			//    old_aux = 5;
			//}

			///* Nasty cut */
			//else if (p.timed[TMD_CUT] > 50)
			//{
			//    old_aux = 4;
			//}

			///* Bad cut */
			//else if (p.timed[TMD_CUT] > 25)
			//{
			//    old_aux = 3;
			//}

			///* Light cut */
			//else if (p.timed[TMD_CUT] > 10)
			//{
			//    old_aux = 2;
			//}

			///* Graze */
			//else if (p.timed[TMD_CUT] > 0)
			//{
			//    old_aux = 1;
			//}

			///* None */
			//else
			//{
			//    old_aux = 0;
			//}

			///* Mortal wound */
			//if (v > 1000)
			//{
			//    new_aux = 7;
			//}

			///* Deep gash */
			//else if (v > 200)
			//{
			//    new_aux = 6;
			//}

			///* Severe cut */
			//else if (v > 100)
			//{
			//    new_aux = 5;
			//}

			///* Nasty cut */
			//else if (v > 50)
			//{
			//    new_aux = 4;
			//}

			///* Bad cut */
			//else if (v > 25)
			//{
			//    new_aux = 3;
			//}

			///* Light cut */
			//else if (v > 10)
			//{
			//    new_aux = 2;
			//}

			///* Graze */
			//else if (v > 0)
			//{
			//    new_aux = 1;
			//}

			///* None */
			//else
			//{
			//    new_aux = 0;
			//}

			///* Increase cut */
			//if (new_aux > old_aux)
			//{
			//    /* Describe the state */
			//    switch (new_aux)
			//    {
			//        /* Graze */
			//        case 1:
			//        {
			//            msgt(MSG_CUT, "You have been given a graze.");
			//            break;
			//        }

			//        /* Light cut */
			//        case 2:
			//        {
			//            msgt(MSG_CUT, "You have been given a light cut.");
			//            break;
			//        }

			//        /* Bad cut */
			//        case 3:
			//        {
			//            msgt(MSG_CUT, "You have been given a bad cut.");
			//            break;
			//        }

			//        /* Nasty cut */
			//        case 4:
			//        {
			//            msgt(MSG_CUT, "You have been given a nasty cut.");
			//            break;
			//        }

			//        /* Severe cut */
			//        case 5:
			//        {
			//            msgt(MSG_CUT, "You have been given a severe cut.");
			//            break;
			//        }

			//        /* Deep gash */
			//        case 6:
			//        {
			//            msgt(MSG_CUT, "You have been given a deep gash.");
			//            break;
			//        }

			//        /* Mortal wound */
			//        case 7:
			//        {
			//            msgt(MSG_CUT, "You have been given a mortal wound.");
			//            break;
			//        }
			//    }

			//    /* Notice */
			//    notice = true;
			//}

			///* Decrease cut */
			//else if (new_aux < old_aux)
			//{
			//    /* Describe the state */
			//    switch (new_aux)
			//    {
			//        /* None */
			//        case 0:
			//        {
			//            msgt(MSG_RECOVER, "You are no longer bleeding.");
			//            if (OPT(disturb_state)) disturb(p_ptr, 0, 0);
			//            break;
			//        }
			//    }

			//    /* Notice */
			//    notice = true;
			//}

			///* Use the value */
			//p.timed[TMD_CUT] = v;

			///* No change */
			//if (!notice) return (false);

			///* Disturb */
			//if (OPT(disturb_state)) disturb(p_ptr, 0, 0);

			///* Recalculate bonuses */
			//p.update |= (PU_BONUS);

			///* Redraw the "cut" */
			//p.redraw |= (PR_STATUS);

			///* Handle stuff */
			//handle_stuff(p_ptr);

			///* Result */
			//return (true);
		}
	}
}
