using System;
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
	}
}
