using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	static class Attack {
		class attack_result {
			bool success;
			int dmg;
			UInt32 msg_type;
			string hit_verb;
		};

		//public static int breakage_chance(const object_type *o_ptr, bool hit_target);
		public static bool test_hit(int chance, int ac, int vis) {
			throw new NotImplementedException();
			return false;
		}

		/**
		 * Attack the monster at the given location with a single blow.
		 */
		static bool py_attack_real(int y, int x, ref bool fear) {
			throw new NotImplementedException();
			///* Information about the target of the attack */
			//monster_type *m_ptr = cave_monster(cave, cave.m_idx[y][x]);
			//monster_race *r_ptr = &r_info[m_ptr.r_idx];
			//char m_name[80];
			//bool stop = false;

			///* The weapon used */
			//object_type *o_ptr = &p_ptr.inventory[INVEN_WIELD];

			///* Information about the attack */
			//int bonus = p_ptr.state.to_h + o_ptr.to_h;
			//int chance = p_ptr.state.skills[SKILL_TO_HIT_MELEE] + bonus * BTH_PLUS_ADJ;
			//bool do_quake = false;
			//bool success = false;

			///* Default to punching for one damage */
			//const char *hit_verb = "punch";
			//int dmg = 1;
			//u32b msg_type = MSG_HIT;

			///* Extract monster name (or "it") */
			//monster_desc(m_name, sizeof(m_name), m_ptr, 0);

			///* Auto-Recall if possible and visible */
			//if (m_ptr.ml) monster_race_track(m_ptr.r_idx);

			///* Track a new monster */
			//if (m_ptr.ml) health_track(p_ptr, cave.m_idx[y][x]);

			///* Handle player fear (only for invisible monsters) */
			//if (check_state(p_ptr, OF_AFRAID, p_ptr.state.flags)) {
			//    msgt(MSG_AFRAID, "You are too afraid to attack %s!", m_name);
			//    return false;
			//}

			///* Disturb the monster */
			//mon_clear_timed(cave.m_idx[y][x], MON_TMD_SLEEP, MON_TMD_FLG_NOMESSAGE,
			//    false);

			///* See if the player hit */
			//success = test_hit(chance, r_ptr.ac, m_ptr.ml);

			///* If a miss, skip this hit */
			//if (!success) {
			//    msgt(MSG_MISS, "You miss %s.", m_name);
			//    return false;
			//}

			///* Handle normal weapon */
			//if (o_ptr.kind) {
			//    int i;
			//    const struct slay *best_s_ptr = null;

			//    hit_verb = "hit";

			//    /* Get the best attack from all slays or
			//     * brands on all non-launcher equipment */
			//    for (i = INVEN_LEFT; i < INVEN_TOTAL; i++) {
			//        struct object *obj = &p_ptr.inventory[i];
			//        if (obj.kind)
			//            improve_attack_modifier(obj, m_ptr, &best_s_ptr, true, false);
			//    }

			//    improve_attack_modifier(o_ptr, m_ptr, &best_s_ptr, true, false);
			//    if (best_s_ptr != null)
			//        hit_verb = best_s_ptr.melee_verb;

			//    dmg = damroll(o_ptr.dd, o_ptr.ds);
			//    dmg *= (best_s_ptr == null) ? 1 : best_s_ptr.mult;

			//    dmg += o_ptr.to_d;
			//    dmg = critical_norm(o_ptr.weight, o_ptr.to_h, dmg, &msg_type);

			//    /* Learn by use for the weapon */
			//    object_notice_attack_plusses(o_ptr);

			//    if (check_state(p_ptr, OF_IMPACT, p_ptr.state.flags) && dmg > 50) {
			//        do_quake = true;
			//        wieldeds_notice_flag(p_ptr, OF_IMPACT);
			//    }
			//}

			///* Learn by use for other equipped items */
			//wieldeds_notice_on_attack();

			///* Apply the player damage bonuses */
			//dmg += p_ptr.state.to_d;

			///* No negative damage */
			//if (dmg <= 0) dmg = 0;

			///* Tell the player what happened */
			//if (dmg <= 0)
			//    msgt(MSG_MISS, "You fail to harm %s.", m_name);
			//else if (msg_type == MSG_HIT)
			//    msgt(MSG_HIT, "You %s %s.", hit_verb, m_name);
			//else if (msg_type == MSG_HIT_GOOD)
			//    msgt(MSG_HIT_GOOD, "You %s %s. %s", hit_verb, m_name, "It was a good hit!");
			//else if (msg_type == MSG_HIT_GREAT)
			//    msgt(MSG_HIT_GREAT, "You %s %s. %s", hit_verb, m_name, "It was a great hit!");
			//else if (msg_type == MSG_HIT_SUPERB)
			//    msgt(MSG_HIT_SUPERB, "You %s %s. %s", hit_verb, m_name, "It was a superb hit!");
			//else if (msg_type == MSG_HIT_HI_GREAT)
			//    msgt(MSG_HIT_HI_GREAT, "You %s %s. %s", hit_verb, m_name, "It was a *GREAT* hit!");
			//else if (msg_type == MSG_HIT_HI_SUPERB)
			//    msgt(MSG_HIT_HI_SUPERB, "You %s %s. %s", hit_verb, m_name, "It was a *SUPERB* hit!");

			///* Complex message */
			//if (p_ptr.wizard)
			//    msg("You do %d (out of %d) damage.", dmg, m_ptr.hp);

			///* Confusion attack */
			//if (p_ptr.confusing) {
			//    p_ptr.confusing = false;
			//    msg("Your hands stop glowing.");

			//    mon_inc_timed(cave.m_idx[y][x], MON_TMD_CONF,
			//            (10 + randint0(p_ptr.lev) / 10), MON_TMD_FLG_NOTIFY, false);
			//}

			///* Damage, check for fear and death */
			//stop = mon_take_hit(cave.m_idx[y][x], dmg, fear, null);

			//if (stop)
			//    (*fear) = false;

			///* Apply earthquake brand */
			//if (do_quake) {
			//    earthquake(p_ptr.py, p_ptr.px, 10);
			//    if (cave.m_idx[y][x] == 0) stop = true;
			//}

			//return stop;
		}


		/**
		 * Attack the monster at the given location
		 *
		 * We get blows until energy drops below that required for another blow, or
		 * until the target monster dies. Each blow is handled by py_attack_real().
		 * We don't allow @ to spend more than 100 energy in one go, to avoid slower
		 * monsters getting double moves.
		 */
		public static void py_attack(int y, int x) {
			int blow_energy = 10000 / Misc.p_ptr.state.num_blows;
			int blows = 0;
			bool fear = false;
			Monster.Monster m_ptr = Cave.cave_monster(Cave.cave, Cave.cave.m_idx[y][x]);
	
			/* disturb the player */
			Cave.disturb(Misc.p_ptr, 0,0);

			/* Initialize the energy used */
			Misc.p_ptr.energy_use = 0;

			/* Attack until energy runs out or enemy dies. We limit energy use to 100
			 * to avoid giving monsters a possible double move. */
			while (Misc.p_ptr.energy >= blow_energy * (blows + 1)) {
				bool stop = py_attack_real(y, x, ref fear);
				Misc.p_ptr.energy_use += (short)blow_energy;
				if (stop || Misc.p_ptr.energy_use + blow_energy > 100) break;
				blows++;
			}
	
			/* Hack - delay fear messages */
			if (fear && m_ptr.ml) {
				//char m_name[80];
				string m_name = m_ptr.monster_desc(0);
				throw new NotImplementedException();
				//add_monster_message(m_name, cave.m_idx[y][x], MON_MSG_FLEE_IN_TERROR, true);
			}
		}

		/**
		 * ranged_attack is a function pointer, used to execute a kind of attack.
		 *
		 * This allows us to abstract details of throwing, shooting, etc. out while
		 * keeping the core projectile tracking, monster cleanup, and display code
		 * in common.
		 */
		//Make this a delegate
		//public static typedef struct attack_result (*ranged_attack) (object_type *o_ptr, int y, int x);
	}
}
