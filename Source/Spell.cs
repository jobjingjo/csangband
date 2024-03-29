﻿using System;
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

		/* TODO: these descriptions are somewhat wrong/misleading */
		/*
		 * Bit flags for the "project()" function
		 *
		 *   NONE: No flags
		 *   JUMP: Jump directly to the target location (this is a hack)
		 *   BEAM: Work as a beam weapon (affect every grid passed through)
		 *   THRU: Continue "through" the target (used for "bolts"/"beams")
		 *   STOP: Stop as soon as we hit a monster (used for "bolts")
		 *   GRID: Affect each grid in the "blast area" in some way
		 *   ITEM: Affect each object in the "blast area" in some way
		 *   KILL: Affect each monster in the "blast area" in some way
		 *   HIDE: Hack -- disable "visual" feedback from projection
		 *   AWARE: Effects are already obvious to the player
		 */
		public const int PROJECT_NONE = 0x000;
		public const int PROJECT_JUMP = 0x001;
		public const int PROJECT_BEAM = 0x002;
		public const int PROJECT_THRU = 0x004;
		public const int PROJECT_STOP = 0x008;
		public const int PROJECT_GRID = 0x010;
		public const int PROJECT_ITEM = 0x020;
		public const int PROJECT_KILL = 0x040;
		public const int PROJECT_HIDE = 0x080;
		public const int PROJECT_AWARE = 0x100;

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
			GF gf_ptr = type;
			int i, denom;

			if (resist == 3) /* immune */
			    return 0;

			/* Hack - acid damage is halved by armour, holy orb is halved */
			if ((type == GF.ACID && minus_ac(p)) || type == GF.HOLY_ORB)
			    dam = (dam + 1) / 2;

			if (resist == -1) /* vulnerable */
			    return (dam * 4 / 3);

			/* Variable resists vary the denominator, so we need to invert the logic
			 * of dam_aspect. (m_bonus is unused) */
			switch (dam_aspect) {
			    case aspect.MINIMISE:
			        denom = Random.randcalc(gf_ptr.denom, 0, aspect.MAXIMISE);
			        break;
			    case aspect.MAXIMISE:
			        denom = Random.randcalc(gf_ptr.denom, 0, aspect.MINIMISE);
			        break;
			    default:
			        denom = Random.randcalc(gf_ptr.denom, 0, dam_aspect);
					break;
			}

			for (i = resist; i > 0; i--)
			    if (denom != 0)
			        dam = dam * gf_ptr.num / denom;

			return dam;
		}

		/*
		 * Acid has hit the player, attempt to affect some armor.
		 *
		 * Note that the "base armor" of an object never changes.
		 *
		 * If any armor is damaged (or resists), the player takes less damage.
		 */
		static bool minus_ac(Player.Player p)
		{
			Object.Object o_ptr = null;

			Bitflag f = new Bitflag(Object.Object_Flag.SIZE);

			//char o_name[80];
			string o_name;

			/* Avoid crash during monster power calculations */
			if (p.inventory == null) return false;

			/* Pick a (possibly empty) inventory slot */
			switch (Random.randint1(6))
			{
				case 1: o_ptr = p.inventory[Misc.INVEN_BODY]; break;
				case 2: o_ptr = p.inventory[Misc.INVEN_ARM]; break;
				case 3: o_ptr = p.inventory[Misc.INVEN_OUTER]; break;
				case 4: o_ptr = p.inventory[Misc.INVEN_HANDS]; break;
				case 5: o_ptr = p.inventory[Misc.INVEN_HEAD]; break;
				case 6: o_ptr = p.inventory[Misc.INVEN_FEET]; break;
				//default: Misc.assert(0); //Nick: DA FUQ is this doing here C???
			}

			/* Nothing to damage */
			if (o_ptr.kind == null) return (false);

			/* No damage left to be done */
			if (o_ptr.ac + o_ptr.to_a <= 0) return (false);

			/* Describe */
			o_name = o_ptr.object_desc(Object.Object.Detail.BASE);
			//object_desc(o_name, sizeof(o_name), o_ptr, ODESC_BASE);

			/* Extract the flags */
			o_ptr.object_flags(ref f);

			/* Object resists */
			if (f.has(Object.Object_Flag.IGNORE_ACID.value))
			{
				Utilities.msg("Your %s is unaffected!", o_name);

				return (true);
			}

			/* Message */
			Utilities.msg("Your %s is damaged!", o_name);

			/* Damage the item */
			o_ptr.to_a--;

			p.update |= Misc.PU_BONUS;
			p.redraw |= (Misc.PR_EQUIP);

			/* Item was damaged */
			return (true);
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
			o_name = o_ptr.object_desc(Object.Object.Detail.PREFIX | Object.Object.Detail.FULL);

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
			int old_chp = p.chp;

			int warning = (p.mhp * Player.Player_Other.instance.hitpoint_warn / 10);


			/* Paranoia */
			if (p.is_dead) return;


			/* Disturb */
			Cave.disturb(p, 1, 0);

			/* Mega-Hack -- Apply "invulnerability" */
			if (p.timed[(int)Timed_Effect.INVULN] != 0 && (dam < 9000)) return;

			/* Hurt the player */
			p.chp -= (short)dam;

			/* Display the hitpoints */
			p.redraw |= (Misc.PR_HP);

			/* Dead player */
			if (p.chp < 0)
			{
			    /* Hack -- Note death */
			    Utilities.msgt(Message_Type.MSG_DEATH, "You die.");
			    Utilities.message_flush();

			    /* Note cause of death */
				p.died_from = kb_str;

			    /* No longer a winner */
			    p.total_winner = 0;

			    /* Note death */
			    p.is_dead = true;

			    /* Leaving */
			    p.leaving = true;

			    /* Dead */
			    return;
			}

			/* Hitpoint warning */
			if (p.chp < warning)
			{
			    /* Hack -- bell on first notice */
			    if (old_chp > warning)
			    {
			        Utilities.bell("Low hitpoint warning!");
			    }

			    /* Message */
			    Utilities.msgt(Message_Type.MSG_HITPOINT_WARN, "*** LOW HITPOINT WARNING! ***");
			    Utilities.message_flush();
			}
		}


		/**
		 * Check for resistance to a GF_ attack type. Return codes:
		 * -1 = vulnerability
		 * 0 = no resistance (or resistance plus vulnerability)
		 * 1 = single resistance or opposition (or double resist plus vulnerability)
		 * 2 = double resistance (including opposition)
		 * 3 = total immunity
		 *
		 * \param type is the attack type we are trying to resist
		 * \param flags is the set of flags we're checking
		 * \param real is whether this is a real attack
		 */
		public static int check_for_resist(Player.Player p, GF type, Bitflag flags, bool real)
		{
			//GF gf_ptr = &gf_table[type];
			GF gf_ptr = type;

			int result = 0;

			if (gf_ptr.vuln != Object.Object_Flag.NONE && flags.has(gf_ptr.vuln.value))
			    result--;

			/* If it's not a real attack, we don't check timed status explicitly */
			if (real && (int)gf_ptr.opp != 0 && p.timed[(int)gf_ptr.opp] != 0)
			    result++;

			if (gf_ptr.resist != Object.Object_Flag.NONE && flags.has(gf_ptr.resist.value))
			    result++;

			if (gf_ptr.immunity != Object.Object_Flag.NONE && flags.has(gf_ptr.immunity.value))
			    result = 3;

			/* Notice flags, if it's a real attack */
			if (real && gf_ptr.immunity.value != 0)
			    Object.Object.wieldeds_notice_flag(p, gf_ptr.immunity.value);
			if (real && gf_ptr.resist.value  != 0)
			    Object.Object.wieldeds_notice_flag(p, gf_ptr.resist.value);
			if (real && gf_ptr.vuln.value != 0)
			    Object.Object.wieldeds_notice_flag(p, gf_ptr.vuln.value);

			return result;
		}

		/*
		 * Increase players hit points, notice effects
		 */
		public static bool hp_player(int num)
		{
			/* Healing needed */
			if (Misc.p_ptr.chp < Misc.p_ptr.mhp)
			{
				/* Gain hitpoints */
				Misc.p_ptr.chp += (short)num;

				/* Enforce maximum */
				if (Misc.p_ptr.chp >= Misc.p_ptr.mhp)
				{
					Misc.p_ptr.chp = Misc.p_ptr.mhp;
					Misc.p_ptr.chp_frac = 0;
				}

				/* Redraw */
				Misc.p_ptr.redraw |= (Misc.PR_HP);

				/* Print a nice message */
				if (num < 5)
					Utilities.msg("You feel a little better.");
				else if (num < 15)
					Utilities.msg("You feel better.");
				else if (num < 35)
					Utilities.msg("You feel much better.");
				else
					Utilities.msg("You feel very good.");

				/* Notice */
				return (true);
			}

			/* Ignore */
			return (false);
		}


		/*
		 * Teleport the player to a location up to "dis" grids away.
		 *
		 * If no such spaces are readily available, the distance may increase.
		 * Try very hard to move the player at least a quarter that distance.
		 */
		public static void teleport_player(int dis)
		{
			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			int d, i, min, y, x;

			bool look = true;


			/* Initialize */
			y = py;
			x = px;

			/* Minimum distance */
			min = dis / 2;

			/* Look until done */
			while (look)
			{
				/* Verify max distance */
				if (dis > 200) dis = 200;

				/* Try several locations */
				for (i = 0; i < 500; i++)
				{
					/* Pick a (possibly illegal) location */
					while (true)
					{
						y = Random.rand_spread(py, dis);
						x = Random.rand_spread(px, dis);
						d = Cave.distance(py, px, y, x);
						if ((d >= min) && (d <= dis)) break;
					}

					/* Ignore illegal locations */
					if (!Cave.cave.in_bounds_fully(y, x)) continue;

					/* Require "naked" floor space */
					if (!Cave.cave_naked_bold(y, x)) continue;

					/* No teleporting into vaults and such */
					if ((Cave.cave.info[y][x] & (Cave.CAVE_ICKY)) != 0) continue;

					/* This grid looks good */
					look = false;

					/* Stop looking */
					break;
				}

				/* Increase the maximum distance */
				dis = dis * 2;

				/* Decrease the minimum distance */
				min = min / 2;
			}

			/* Sound */
			//sound(MSG_TELEPORT); //Nick: Todo: Enable sound

			/* Move player */
			Monster.Monster.monster_swap(py, px, y, x);

			/* Handle stuff XXX XXX XXX */
			Misc.p_ptr.handle_stuff();
		}

	}
}
