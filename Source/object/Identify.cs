﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Object {
	partial class Object {

		/* Whether to learn egos and flavors with less than complete information */
		const bool EASY_LEARN = true;

		/** Time last item was wielded */
		static int object_last_wield = 0;
		/*
		 * Sense the inventory
		 */
		public static void sense_inventory()
		{
			int i;
	
			//char o_name[80];
			string o_name = null;
			uint rate;
	
	
			/* No ID when confused in a bad state */
			if (Misc.p_ptr.timed[(int)Timed_Effect.CONFUSED] != 0) return;


			/* Notice some things after a while */
			if (Misc.turn >= (object_last_wield + 3000))
			{
				object_notice_after_time();
				object_last_wield = 0;
			}


			/* Get improvement rate */
			if (Misc.p_ptr.player_has(Misc.PF.PSEUDO_ID_IMPROV.value))
			    rate = (uint)(Misc.p_ptr.Class.sense_base / (Misc.p_ptr.lev * Misc.p_ptr.lev + Misc.p_ptr.Class.sense_div));
			else
			    rate = (uint)(Misc.p_ptr.Class.sense_base / (Misc.p_ptr.lev + Misc.p_ptr.Class.sense_div));

			if (!Random.one_in_((int)rate)) return;


			/* Check everything */
			for (i = 0; i < Misc.ALL_INVEN_TOTAL; i++)
			{
			    string text = null;

			    Object o_ptr = Misc.p_ptr.inventory[i];
			    obj_pseudo_t feel;
			    bool cursed;

			    bool okay = false;

			    /* Skip empty slots */
			    if (o_ptr.kind == null) continue;

			    /* Valid "tval" codes */
			    switch (o_ptr.tval)
			    {
			        case TVal.TV_SHOT:
			        case TVal.TV_ARROW:
			        case TVal.TV_BOLT:
			        case TVal.TV_BOW:
			        case TVal.TV_DIGGING:
			        case TVal.TV_HAFTED:
			        case TVal.TV_POLEARM:
			        case TVal.TV_SWORD:
			        case TVal.TV_BOOTS:
			        case TVal.TV_GLOVES:
			        case TVal.TV_HELM:
			        case TVal.TV_CROWN:
			        case TVal.TV_SHIELD:
			        case TVal.TV_CLOAK:
			        case TVal.TV_SOFT_ARMOR:
			        case TVal.TV_HARD_ARMOR:
			        case TVal.TV_DRAG_ARMOR:
			        {
			            okay = true;
			            break;
			        }
			    }
		
			    /* Skip non-sense machines */
			    if (!okay) continue;
		
			    /* It is known, no information needed */
			    if (o_ptr.is_known()) continue;
		
		
			    /* It has already been sensed, do not sense it again */
			    if (o_ptr.was_sensed())
			    {
			        /* Small chance of wielded, sensed items getting complete ID */
					if(o_ptr.artifact == null && (i >= Misc.INVEN_WIELD) && Random.one_in_(1000)) {
						throw new NotImplementedException();
						//do_ident_item(i, o_ptr);
					}

			        continue;
			    }

			    /* Occasional failure on inventory items */
			    if ((i < Misc.INVEN_WIELD) && Random.one_in_(5)) continue;


			    /* Sense the object */
			    o_ptr.notice_sensing();
			    cursed = o_ptr.notice_curses();

			    /* Get the feeling */
			    feel = o_ptr.pseudo();

			    /* Stop everything */
			    Cave.disturb(Misc.p_ptr, 0, 0);

			    if (cursed)
			        text = "cursed";
			    else
			        text = Misc.inscrip_text[(int)feel];

			    o_name = o_ptr.object_desc(Detail.BASE);

			    /* Average pseudo-ID means full ID */
			    if (feel == obj_pseudo_t.INSCRIP_AVERAGE)
			    {
			        o_ptr.notice_everything();

			        Utilities.msgt(Message_Type.MSG_PSEUDOID,
			                "You feel the %s (%c) %s %s average...",
			                o_name, index_to_label(i),((i >=
			                Misc.INVEN_WIELD) ? "you are using" : "in your pack"),
			                ((o_ptr.number == 1) ? "is" : "are"));
			    }
			    else
			    {
			        if (i >= Misc.INVEN_WIELD)
			        {
			            Utilities.msgt(Message_Type.MSG_PSEUDOID, "You feel the %s (%c) you are %s %s %s...",
			                           o_name, index_to_label(i), describe_use(i),
			                           ((o_ptr.number == 1) ? "is" : "are"),
			                                       text);
			        }
			        else
			        {
			            Utilities.msgt(Message_Type.MSG_PSEUDOID, "You feel the %s (%c) in your pack %s %s...",
			                           o_name, index_to_label(i),
			                           ((o_ptr.number == 1) ? "is" : "are"),
			                                       text);
			        }
			    }


			    /* Set squelch flag as appropriate */
			    if (i < Misc.INVEN_WIELD)
			        Misc.p_ptr.notice |= Misc.PN_SQUELCH;
		
		
			    /* Combine / Reorder the pack (later) */
			    Misc.p_ptr.notice |= (Misc.PN_COMBINE | Misc.PN_REORDER | Misc.PN_SORT_QUIVER);
		
			    /* Redraw stuff */
			    Misc.p_ptr.redraw |= (Misc.PR_INVEN | Misc.PR_EQUIP);
			}
		}

		/**
		 * Notice things about an object that would be noticed in time.
		 */
		static void object_notice_after_time()
		{
			int i;
			int flag;

			Object o_ptr;
			string o_name;//[80];

			Bitflag f = new Bitflag(Object_Flag.SIZE);
			Bitflag timed_mask = new Bitflag(Object_Flag.SIZE);

			Object_Flag.create_mask(timed_mask, true, Object_Flag.object_flag_id.TIMED);

			/* Check every item the player is wearing */
			for (i = Misc.INVEN_WIELD; i < Misc.ALL_INVEN_TOTAL; i++)
			{
				o_ptr = Misc.p_ptr.inventory[i];

				if (o_ptr.kind == null || o_ptr.is_known()) continue;

				/* Check for timed notice flags */
				o_name = o_ptr.object_desc(Detail.BASE);
				o_ptr.object_flags(ref f);
				f.inter(timed_mask);

				for (flag = f.next(Bitflag.FLAG_START); flag != Bitflag.FLAG_null; flag = f.next(flag + 1))
				{
					if (!o_ptr.known_flags.has(flag))
					{
						/* Message */
						Object_Flag.flag_message(flag, o_name);

						/* Notice the flag */
						o_ptr.notice_flag(flag);

						if (o_ptr.is_jewelry() && (o_ptr.effect() == null || o_ptr.effect_is_known()))
						{
							/* XXX this is a small hack, but jewelry with anything noticeable really is obvious */
							/* XXX except, wait until learn activation if that is only clue */
							o_ptr.flavor_aware();
							o_ptr.check_for_ident();
						}
					}
					else
					{
						/* Notice the flag is absent */
						o_ptr.notice_flag(flag);
					}
				}

				/* XXX Is this necessary? */
				o_ptr.check_for_ident();
			}
		}

		/*
		 * Notice a single flag - returns true if anything new was learned
		 */
		public bool notice_flag(int flag)
		{
			if (!known_flags.has(flag))
			{
				known_flags.on(flag);
				/* XXX Eddie don't want infinite recursion if object_check_for_ident sets more flags,
				 * but maybe this will interfere with savefile repair
				 */
				check_for_ident();
				Game_Event.signal(Game_Event.Event_Type.INVENTORY);
				Game_Event.signal(Game_Event.Event_Type.EQUIPMENT);
				return true;
			}

			return false;
		}


		/*
		 * Mark an object as sensed.
		 */
		void notice_sensing()
		{
			if (was_sensed())
				return;

			if (artifact != null) {
				artifact.seen = artifact.everseen = true;
				ident |= IDENT_NAME;
			}

			notice_curses();
			if (add_ident_flags(IDENT_SENSE))
				check_for_ident();
		}

		/**
		 * Notice curses on an object.
		 *
		 * \param o_ptr is the object to notice curses on
		 */
		bool notice_curses()
		{
			Bitflag f = new Bitflag(Object_Flag.SIZE);
			Bitflag f2 = new Bitflag(Object_Flag.SIZE);

			object_flags(ref f);

			/* Gather whatever curse flags there are to know */
			Object_Flag.create_mask(f2, false, Object_Flag.object_flag_type.CURSE);

			/* Remove everything except the curse flags */
			f.inter(f2);

			/* give knowledge of which curses are present */
			notice_flags(f);

			check_for_ident();

			Misc.p_ptr.notice |= Misc.PN_SQUELCH;

			return !f.is_empty();
		}

		/*
		 * Checks for additional knowledge implied by what the player already knows.
		 *
		 * \param o_ptr is the object to check
		 *
		 * returns whether it calls object_notice_everyting
		 */
		bool check_for_ident()
		{
			Bitflag flags = new Bitflag(Object_Flag.SIZE);
			Bitflag known_flags = new Bitflag(Object_Flag.SIZE);
			Bitflag f2 = new Bitflag(Object_Flag.SIZE);
	
			object_flags(ref flags);
			object_flags_known(ref known_flags);

			/* Some flags are irrelevant or never learned or too hard to learn */
			Object_Flag.create_mask(f2, false,	Object_Flag.object_flag_type.INT, 
												Object_Flag.object_flag_type.IGNORE, 
												Object_Flag.object_flag_type.HATES);

			flags.diff(f2);
			known_flags.diff(f2);

			if (!flags.is_equal(known_flags)) return false;

			/* If we know attack bonuses, and defence bonuses, and effect, then
			 * we effectively know everything, so mark as such */
			if ((attack_plusses_are_visible() || (was_sensed() && to_h == 0 && to_d == 0)) &&
			    (defence_plusses_are_visible() || (was_sensed() && to_a == 0)) &&
			    (effect_is_known() || effect() == null))
			{
			    /* In addition to knowing the pval flags, it is necessary to know the pvals to know everything */
			    int i;
			    for (i = 0; i < num_pvals; i++)
			        if (!this_pval_is_visible(i))
			            break;
			    if (i == num_pvals) {
			        notice_everything();
			        return true;
			    }
			}

			/* We still know all the flags, so we still know if it's an ego */
			if (ego != null)
			{
			    /* require worn status so you don't learn launcher of accuracy or gloves of slaying before wield */
			    if (was_worn())
			        notice_ego();
			}

			return false;
		}

		/*
		 * Notice the ego on an ego item.
		 */
		public void notice_ego()
		{
			throw new NotImplementedException();
			//bitflag learned_flags[OF_SIZE];
			//bitflag xtra_flags[OF_SIZE];

			//if (!o_ptr.ego)
			//    return;


			///* XXX Eddie print a message on notice ego if not already noticed? */
			///* XXX Eddie should we do something about everseen of egos here? */

			///* Learn ego flags */
			//of_union(o_ptr.known_flags, o_ptr.ego.flags);

			///* Learn all flags except random abilities */
			//of_setall(learned_flags);

			//switch (o_ptr.ego.xtra)
			//{
			//    case OBJECT_XTRA_TYPE_NONE:
			//        break;
			//    case OBJECT_XTRA_TYPE_SUSTAIN:
			//        create_mask(xtra_flags, false, OFT_SUST, OFT_MAX);
			//        of_diff(learned_flags, xtra_flags);
			//        break;
			//    case OBJECT_XTRA_TYPE_RESIST:
			//        create_mask(xtra_flags, false, OFT_HRES, OFT_MAX);
			//        of_diff(learned_flags, xtra_flags);
			//        break;
			//    case OBJECT_XTRA_TYPE_POWER:
			//        create_mask(xtra_flags, false, OFT_MISC, OFT_PROT, OFT_MAX);
			//        of_diff(learned_flags, xtra_flags);
			//        break;
			//    default:
			//        assert(0);
			//}

			//of_union(o_ptr.known_flags, learned_flags);

			//if (object_add_ident_flags(o_ptr, IDENT_NAME))
			//{
			//    /* if you know the ego, you know which it is of excellent or splendid */
			//    object_notice_sensing(o_ptr);

			//    object_check_for_ident(o_ptr);
			//}
		}

		/*
		 * Notice a set of flags - returns true if anything new was learned
		 */
		bool notice_flags(Bitflag flags)
		{
			throw new NotImplementedException();
			//if (!of_is_subset(o_ptr.known_flags, flags))
			//{
			//    of_union(o_ptr.known_flags, flags);
			//    /* XXX Eddie don't want infinite recursion if object_check_for_ident sets more flags,
			//     * but maybe this will interfere with savefile repair
			//     */
			//    object_check_for_ident(o_ptr);
			//    event_signal(EVENT_INVENTORY);
			//    event_signal(EVENT_EQUIPMENT);

			//    return true;
			//}

			//return false;
		}

		public void notice_attack_plusses()
		{
			Misc.assert(kind != null);

			if (attack_plusses_are_visible())
			    return;

			if (add_ident_flags(IDENT_ATTACK))
			    check_for_ident();


			if (wield_slot() == Misc.INVEN_WIELD)
			{
			    //char o_name[80];
				string o_name = object_desc(Detail.BASE);
			    Utilities.msgt(Message_Type.MSG_PSEUDOID,
			            "You know more about the %s you are using.",
			            o_name);
			}
			else if ((to_d != 0 || to_h != 0) &&
			        !((tval == TVal.TV_HARD_ARMOR || tval == TVal.TV_SOFT_ARMOR) && (to_h < 0)))
			{
			    //char o_name[80];
				string o_name = object_desc(Detail.BASE);
			    Utilities.msgt(Message_Type.MSG_PSEUDOID, "Your {0} glows.", o_name);
			}

			Misc.p_ptr.update |= (Misc.PU_BONUS);
			Game_Event.signal(Game_Event.Event_Type.INVENTORY);
			Game_Event.signal(Game_Event.Event_Type.EQUIPMENT);
		}

		/**
		 * Notice a given special flag on wielded items.
		 *
		 * \param flag is the flag to notice
		 */
		public static void wieldeds_notice_flag(Player.Player p, int flag)
		{
			int i;

			/* Sanity check */
			if (flag == 0) return;

			/* XXX Eddie need different naming conventions for starting wieldeds at INVEN_WIELD vs INVEN_WIELD+2 */
			for (i = Misc.INVEN_WIELD; i < Misc.ALL_INVEN_TOTAL; i++)
			{
			    Object o_ptr = p.inventory[i];
			    Bitflag f = new Bitflag(Object_Flag.SIZE);

			    if (o_ptr.kind == null) continue;

			    o_ptr.object_flags(ref f);

			    if (f.has(flag) && !o_ptr.known_flags.has(flag))
			    {
			        //char o_name[80];
					string o_name = o_ptr.object_desc(Detail.BASE);

			        /* Notice the flag */
			        o_ptr.notice_flag(flag);

			        /* XXX Eddie should this go before noticing the flag to avoid learning twice? */
			        if (EASY_LEARN && o_ptr.is_jewelry())
			        {
			            /* XXX Eddie EASY_LEARN Possible concern: gets =teleportation just from +2 speed */
			            o_ptr.flavor_aware();
			            o_ptr.check_for_ident();
			        }

			        /* Message */
			        Object_Flag.flag_message(flag, o_name);
			    }
			    else
			    {
			        /* Notice that flag is absent */
			        o_ptr.notice_flag(flag);
			    }

			    /* XXX Eddie should not need this, should be done in noticing, but will remove later */
			    o_ptr.check_for_ident();

			}

			return;
		}


		/**
		 * Notice things which happen on attacking.
		 */
		public static void wieldeds_notice_on_attack()
		/* Does not apply to weapon or bow which should be done separately */
		{
			int i;

			for (i = Misc.INVEN_WIELD + 2; i < Misc.INVEN_TOTAL; i++)
			    if (Misc.p_ptr.inventory[i].kind != null)
			        Misc.p_ptr.inventory[i].notice_attack_plusses();

			/* XXX Eddie print message? */
			/* XXX Eddie do we need to do more about ammo? */

			return;
		}

		/*
		 * \returns whether it is possible an object has a high resist given the
		 *          player's current knowledge
		 */
		public bool high_resist_is_possible()
		{
			throw new NotImplementedException();
			//bitflag flags[OF_SIZE], f2[OF_SIZE];

			///* Actual object flags */
			//object_flags(o_ptr, flags);

			///* Add player's uncertainty */
			//of_comp_union(flags, o_ptr.known_flags);

			///* Check for possible high resist */
			//create_mask(f2, false, OFT_HRES, OFT_MAX);
			//if (of_is_inter(flags, f2))
			//    return true;
			//else
			//    return false;
		}

		/**
		 * Notice things which happen on defending.
		 */
		public static void object_notice_on_defend(Player.Player p)
		{
			int i;

			for (i = Misc.INVEN_WIELD; i < Misc.INVEN_TOTAL; i++)
				if (p.inventory[i].kind != null)
					p.inventory[i].notice_defence_plusses(p);

			Game_Event.signal(Game_Event.Event_Type.INVENTORY);
			Game_Event.signal(Game_Event.Event_Type.EQUIPMENT);
		}

		void notice_defence_plusses(Player.Player p)
		{
			Misc.assert(kind != null);

			if (defence_plusses_are_visible())
				return;

			if (add_ident_flags(IDENT_DEFENCE))
				check_for_ident();

			if (ac != 0 || to_a != 0)
			{
				//char o_name[80];
				string o_name = object_desc(Detail.BASE);
				Utilities.msgt(Message_Type.MSG_PSEUDOID, "You know more about the {0} you are wearing.", o_name);
			}

			p.update |= (Misc.PU_BONUS);
			Game_Event.signal(Game_Event.Event_Type.INVENTORY);
			Game_Event.signal(Game_Event.Event_Type.EQUIPMENT);
		}

		/*
		 * Notice stuff when firing or throwing objects.
		 *
		 */
		/* XXX Eddie perhaps some stuff from do_cmd_fire and do_cmd_throw should be moved here */
		public void notice_on_firing()
		{
			if (add_ident_flags(IDENT_FIRED))
				check_for_ident();
		}

		/*
		 * Determine whether a weapon or missile weapon is obviously {excellent} when
		 * worn.
		 *
		 * XXX Eddie should messages be adhoc all over the place?  perhaps the main
		 * loop should check for change in inventory/wieldeds and all messages be
		 * printed from one place
		 */
		public void notice_on_wield()
		{
			Bitflag f = new Bitflag(Object_Flag.SIZE);
			Bitflag f2 = new Bitflag(Object_Flag.SIZE);
			Bitflag obvious_mask = new Bitflag(Object_Flag.SIZE);
			bool obvious = false;

			Object_Flag.create_mask(obvious_mask, true, Object_Flag.object_flag_id.WIELD);

			/* Save time of wield for later */
			object_last_wield = Misc.turn;

			/* Only deal with un-ID'd items */
			if (is_known()) return;

			/* Wear it */
			flavor_tried();
			if (add_ident_flags(IDENT_WORN))
			    check_for_ident();

			/* CC: may wish to be more subtle about this once we have ego lights
			 * with multiple pvals */
			if (is_light() && ego != null)
			    notice_ego();

			if (flavor_is_aware() && easy_know())
			{
			    notice_everything();
			    return;
			}

			/* Automatically sense artifacts upon wield */
			sense_artifact();

			/* Note artifacts when found */
			if (artifact != null)
			    History.add_artifact(artifact, is_known(), true);

			/* special case FA, needed at least for mages wielding gloves */
			if (FA_would_be_obvious())
			    obvious_mask.on(Object_Flag.FREE_ACT.value);

			/* Extract the flags */
			object_flags(ref f);

			/* Find obvious things (disregarding curses) - why do we remove the curses?? */
			Object_Flag.create_mask(f2, false, Object_Flag.object_flag_type.CURSE);
			obvious_mask.diff(f2);
			if (f.is_inter(obvious_mask)) obvious = true;
			Object_Flag.create_mask(obvious_mask, true, Object_Flag.object_flag_id.WIELD);

			/* Notice any obvious brands or slays */
			Slay.object_notice_slays(this, obvious_mask);

			/* Learn about obvious flags */
			known_flags.union(obvious_mask);

			/* XXX Eddie should these next NOT call object_check_for_ident due to worries about repairing? */

			/* XXX Eddie this is a small hack, but jewelry with anything noticeable really is obvious */
			/* XXX Eddie learn =soulkeeping vs =bodykeeping when notice sustain_str */
			if (is_jewelry())
			{
			    /* Learn the flavor of jewelry with obvious flags */
			    if (EASY_LEARN && obvious)
			        flavor_aware();

			    /* Learn all flags on any aware non-artifact jewelry */
			    if (flavor_is_aware() && artifact == null)
			        know_all_flags();
			}

			check_for_ident();

			if (!obvious) return;

			/* XXX Eddie need to add stealth here, also need to assert/double-check everything is covered */
			/* CC: also need to add FA! */
			if (f.has(Object_Flag.STR.value))
			    Utilities.msg("You feel %s!", pval[which_pval(
			        Object_Flag.STR.value)] > 0 ? "stronger" : "weaker");
			if (f.has(Object_Flag.INT.value))
			    Utilities.msg("You feel %s!", pval[which_pval(
			        Object_Flag.INT.value)] > 0 ? "smarter" : "more stupid");
			if (f.has(Object_Flag.WIS.value))
			    Utilities.msg("You feel %s!", pval[which_pval(
			        Object_Flag.WIS.value)] > 0 ? "wiser" : "more naive");
			if (f.has(Object_Flag.DEX.value))
			    Utilities.msg("You feel %s!", pval[which_pval(
			        Object_Flag.DEX.value)] > 0 ? "more dextrous" : "clumsier");
			if (f.has(Object_Flag.CON.value))
			    Utilities.msg("You feel %s!", pval[which_pval(
			        Object_Flag.CON.value)] > 0 ? "healthier" : "sicklier");
			if (f.has(Object_Flag.CHR.value))
			    Utilities.msg("You feel %s!", pval[which_pval(
			        Object_Flag.CHR.value)] > 0 ? "cuter" : "uglier");
			if (f.has(Object_Flag.SPEED.value))
			    Utilities.msg("You feel strangely %s.", pval[which_pval(
			        Object_Flag.SPEED.value)] > 0 ? "quick" : "sluggish");
			if (f.has(Object_Flag.BLOWS.value))
			    Utilities.msg("Your weapon %s in your hands.",
			        pval[which_pval(Object_Flag.BLOWS.value)] > 0 ?
			            "tingles" : "aches");
			if (f.has(Object_Flag.SHOTS.value))
			    Utilities.msg("Your bow %s in your hands.",
			        pval[which_pval(Object_Flag.SHOTS.value)] > 0 ?
			            "tingles" : "aches");
			if (f.has(Object_Flag.INFRA.value))
			    Utilities.msg("Your eyes tingle.");
			if (f.has(Object_Flag.LIGHT.value))
			    Utilities.msg("It glows!");
			if (f.has(Object_Flag.TELEPATHY.value))
			    Utilities.msg("Your mind feels strangely sharper!");

			/* WARNING -- masking f by obvious mask -- this should be at the end of this function */
			/* CC: I think this can safely go, but just in case ... */
			/*flags_mask(f, OF_SIZE, OF_OBVIOUS_MASK, FLAG_END); */

			/* Remember the flags */
			notice_sensing();

			/* XXX Eddie should we check_for_ident here? */
		}

		/**
		 * Mark an object's flavour as tried.
		 *
		 * \param o_ptr is the object whose flavour should be marked
		 */
		public void flavor_tried()
		{
			Misc.assert(kind != null);

			kind.tried = true;
		}

		/*
		 * Sense artifacts
		 */
		public void sense_artifact()
		{
			if (artifact != null)
				notice_sensing();
			else
				ident |= IDENT_NOTART;
		}

		/**
		 * Notice the "effect" from activating an object.
		 *
		 * \param o_ptr is the object to become aware of
		 */
		public void notice_effect()
		{
			if (add_ident_flags(IDENT_EFFECT))
				check_for_ident();

			/* noticing an effect gains awareness */
			if (!flavor_is_aware())
				flavor_aware();
		}

	}
}
