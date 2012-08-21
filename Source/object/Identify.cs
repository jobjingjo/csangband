using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Object {
	partial class Object {
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
				throw new NotImplementedException();
				//object_notice_after_time();
				//object_last_wield = 0;
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

			    Object_Desc.object_desc(ref o_name, 80, o_ptr, Object_Desc.Detail.BASE);

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
			throw new NotImplementedException();
			//bitflag flags[OF_SIZE], known_flags[OF_SIZE], f2[OF_SIZE];
	
			//object_flags(o_ptr, flags);
			//object_flags_known(o_ptr, known_flags);

			///* Some flags are irrelevant or never learned or too hard to learn */
			//create_mask(f2, false, OFT_INT, OFT_IGNORE, OFT_HATES, OFT_MAX);

			//of_diff(flags, f2);
			//of_diff(known_flags, f2);

			//if (!of_is_equal(flags, known_flags)) return false;

			///* If we know attack bonuses, and defence bonuses, and effect, then
			// * we effectively know everything, so mark as such */
			//if ((object_attack_plusses_are_visible(o_ptr) || (object_was_sensed(o_ptr) && o_ptr.to_h == 0 && o_ptr.to_d == 0)) &&
			//    (object_defence_plusses_are_visible(o_ptr) || (object_was_sensed(o_ptr) && o_ptr.to_a == 0)) &&
			//    (object_effect_is_known(o_ptr) || !object_effect(o_ptr)))
			//{
			//    /* In addition to knowing the pval flags, it is necessary to know the pvals to know everything */
			//    int i;
			//    for (i = 0; i < o_ptr.num_pvals; i++)
			//        if (!object_this_pval_is_visible(o_ptr, i))
			//            break;
			//    if (i == o_ptr.num_pvals) {
			//        object_notice_everything(o_ptr);
			//        return true;
			//    }
			//}

			///* We still know all the flags, so we still know if it's an ego */
			//if (o_ptr.ego)
			//{
			//    /* require worn status so you don't learn launcher of accuracy or gloves of slaying before wield */
			//    if (object_was_worn(o_ptr))
			//        object_notice_ego(o_ptr);
			//}

			//return false;
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
			throw new NotImplementedException();
			//assert(o_ptr && o_ptr.kind);

			//if (object_attack_plusses_are_visible(o_ptr))
			//    return;

			//if (object_add_ident_flags(o_ptr, IDENT_ATTACK))
			//    object_check_for_ident(o_ptr);


			//if (wield_slot(o_ptr) == INVEN_WIELD)
			//{
			//    char o_name[80];

			//    object_desc(o_name, sizeof(o_name), o_ptr, ODESC_BASE);
			//    msgt(MSG_PSEUDOID,
			//            "You know more about the %s you are using.",
			//            o_name);
			//}
			//else if ((o_ptr.to_d || o_ptr.to_h) &&
			//        !((o_ptr.tval == TV_HARD_ARMOR || o_ptr.tval == TV_SOFT_ARMOR) && (o_ptr.to_h < 0)))
			//{
			//    char o_name[80];

			//    object_desc(o_name, sizeof(o_name), o_ptr, ODESC_BASE);
			//    msgt(MSG_PSEUDOID, "Your %s glows.", o_name);
			//}

			//p_ptr.update |= (PU_BONUS);
			//event_signal(EVENT_INVENTORY);
			//event_signal(EVENT_EQUIPMENT);
		}

		/**
		 * Notice a given special flag on wielded items.
		 *
		 * \param flag is the flag to notice
		 */
		public static void wieldeds_notice_flag(Player.Player p, int flag)
		{
			throw new NotImplementedException();
			//int i;

			///* Sanity check */
			//if (!flag) return;

			///* XXX Eddie need different naming conventions for starting wieldeds at INVEN_WIELD vs INVEN_WIELD+2 */
			//for (i = INVEN_WIELD; i < ALL_INVEN_TOTAL; i++)
			//{
			//    object_type *o_ptr = &p.inventory[i];
			//    bitflag f[OF_SIZE];

			//    if (!o_ptr.kind) continue;

			//    object_flags(o_ptr, f);

			//    if (of_has(f, flag) && !of_has(o_ptr.known_flags, flag))
			//    {
			//        char o_name[80];
			//        object_desc(o_name, sizeof(o_name), o_ptr, ODESC_BASE);

			//        /* Notice the flag */
			//        object_notice_flag(o_ptr, flag);

			//        /* XXX Eddie should this go before noticing the flag to avoid learning twice? */
			//        if (EASY_LEARN && object_is_jewelry(o_ptr))
			//        {
			//            /* XXX Eddie EASY_LEARN Possible concern: gets =teleportation just from +2 speed */
			//            object_flavor_aware(o_ptr);
			//            object_check_for_ident(o_ptr);
			//        }

			//        /* Message */
			//        flag_message(flag, o_name);
			//    }
			//    else
			//    {
			//        /* Notice that flag is absent */
			//        object_notice_flag(o_ptr, flag);
			//    }

			//    /* XXX Eddie should not need this, should be done in noticing, but will remove later */
			//    object_check_for_ident(o_ptr);

			//}

			//return;
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
	}
}
