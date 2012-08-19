using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	/*
	 * And here's the structure for the "fixed" spell information
	 */
	class Spell {
		public Spell next;
		public UInt32 sidx;
		public string name;
		public string text;

		public byte realm;			/* 0 = mage; 1 = priest */
		public byte tval;			/* Item type for book this spell is in */
		public byte sval;			/* Item sub-type for book (= book number) */
		public byte snum;			/* Position of spell within book */

		public byte spell_index;	/* Index into player_magic array */

		/**
		 * Adjust damage according to resistance or vulnerability.
		 *
		 * \param type is the attack type we are checking.
		 * \param dam is the unadjusted damage.
		 * \param dam_aspect is the calc we want (min, avg, max, random).
		 * \param resist is the degree of resistance (-1 = vuln, 3 = immune).
		 */
		public static int adjust_dam(Player.Player p, GF type, int dam, aspect dam_aspect, int resist)
		{
			throw new NotImplementedException();
			//const struct gf_type *gf_ptr = &gf_table[type];
			//int i, denom;

			//if (resist == 3) /* immune */
			//    return 0;

			///* Hack - acid damage is halved by armour, holy orb is halved */
			//if ((type == GF_ACID && minus_ac(p)) || type == GF_HOLY_ORB)
			//    dam = (dam + 1) / 2;

			//if (resist == -1) /* vulnerable */
			//    return (dam * 4 / 3);

			///* Variable resists vary the denominator, so we need to invert the logic
			// * of dam_aspect. (m_bonus is unused) */
			//switch (dam_aspect) {
			//    case MINIMISE:
			//        denom = randcalc(gf_ptr.denom, 0, MAXIMISE);
			//        break;
			//    case MAXIMISE:
			//        denom = randcalc(gf_ptr.denom, 0, MINIMISE);
			//        break;
			//    default:
			//        denom = randcalc(gf_ptr.denom, 0, dam_aspect);
			//}

			//for (i = resist; i > 0; i--)
			//    if (denom)
			//        dam = dam * gf_ptr.num / denom;

			//return dam;
		}

		/*
		 * Identify an item.
		 *
		 * `item` is used to print the slot occupied by an object in equip/inven.
		 * Any negative value assigned to "item" can be used for specifying an object
		 * on the floor.
		 */
		public static void do_ident_item(int item, Object.Object o_ptr)
		{
			string o_name = "";//80

			Message_Type msg_type = (Message_Type)0;
			int i;
			bool bad = true;

			/* Identify it */
			o_ptr.flavor_aware();
			o_ptr.notice_everything();

			/* Apply an autoinscription, if necessary */
			Squelch.apply_autoinscription(o_ptr);

			/* Set squelch flag */
			Misc.p_ptr.notice |= (int)Misc.PN_SQUELCH;

			/* Recalculate bonuses */
			Misc.p_ptr.update |= (Misc.PU_BONUS);

			/* Combine / Reorder the pack (later) */
			Misc.p_ptr.notice |= (int)(Misc.PN_COMBINE | Misc.PN_REORDER | Misc.PN_SORT_QUIVER);

			/* Window stuff */
			Misc.p_ptr.redraw |= (Misc.PR_INVEN | Misc.PR_EQUIP);

			/* Description */
			Object.Object_Desc.object_desc(ref o_name, 80, o_ptr, Object.Object_Desc.Detail.PREFIX | Object.Object_Desc.Detail.FULL);

			/* Determine the message type. */
			/* CC: we need to think more carefully about how we define "bad" with
			 * multiple pvals - currently using "all nonzero pvals < 0" */
			for (i = 0; i < o_ptr.num_pvals; i++)
				if (o_ptr.pval[i] > 0)
					bad = false;

			if (bad)
				msg_type = Message_Type.MSG_IDENT_BAD;
			else if (o_ptr.artifact != null)
				msg_type = Message_Type.MSG_IDENT_ART;
			else if (o_ptr.ego != null)
				msg_type = Message_Type.MSG_IDENT_EGO;
			else
				msg_type = Message_Type.MSG_GENERIC;

			/* Log artifacts to the history list. */
			if (o_ptr.artifact != null)
				History.add_artifact(o_ptr.artifact, true, true);

			/* Describe */
			if (item >= Misc.INVEN_WIELD)
			{
				Utilities.msgt(msg_type, "{0}: {1} ({2}).", Object.Object.describe_use(item), o_name, Object.Object.index_to_label(item));
				//Utilities.msgt(msg_type, "%^s: %s (%c).", describe_use(item), o_name, index_to_label(item));
			}
			else if (item >= 0)
			{
				Utilities.msgt(msg_type, "In your pack: {0} ({1}).", o_name, Object.Object.index_to_label(item));
				//Utilities.msgt(msg_type, "In your pack: %s (%c).", o_name, index_to_label(item));
			}
			else
			{
				Utilities.msgt(msg_type, "On the ground: {0}.", o_name);
			}
		}


		/*
		 * Decreases players hit points and sets death flag if necessary
		 *
		 * Invulnerability needs to be changed into a "shield" XXX XXX XXX
		 *
		 * Hack -- this function allows the user to save (or quit) the game
		 * when he dies, since the "You die." message is shown before setting
		 * the player to "dead".
		 */
		public static void take_hit(Player.Player p, int dam, string kb_str)
		{
			throw new NotImplementedException();
			//int old_chp = p.chp;

			//int warning = (p.mhp * op_ptr.hitpoint_warn / 10);


			///* Paranoia */
			//if (p.is_dead) return;


			///* Disturb */
			//disturb(p, 1, 0);

			///* Mega-Hack -- Apply "invulnerability" */
			//if (p.timed[TMD_INVULN] && (dam < 9000)) return;

			///* Hurt the player */
			//p.chp -= dam;

			///* Display the hitpoints */
			//p.redraw |= (PR_HP);

			///* Dead player */
			//if (p.chp < 0)
			//{
			//    /* Hack -- Note death */
			//    msgt(MSG_DEATH, "You die.");
			//    message_flush();

			//    /* Note cause of death */
			//    my_strcpy(p.died_from, kb_str, sizeof(p.died_from));

			//    /* No longer a winner */
			//    p.total_winner = false;

			//    /* Note death */
			//    p.is_dead = true;

			//    /* Leaving */
			//    p.leaving = true;

			//    /* Dead */
			//    return;
			//}

			///* Hitpoint warning */
			//if (p.chp < warning)
			//{
			//    /* Hack -- bell on first notice */
			//    if (old_chp > warning)
			//    {
			//        bell("Low hitpoint warning!");
			//    }

			//    /* Message */
			//    msgt(MSG_HITPOINT_WARN, "*** LOW HITPOINT WARNING! ***");
			//    message_flush();
			//}
		}

	}
}
