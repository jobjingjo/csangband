using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Monster;
using CSAngband.Object;

namespace CSAngband {
	static class Attack {
		class attack_result {
			bool success;
			int dmg;
			UInt32 msg_type;
			string hit_verb;
		};

		//public static int breakage_chance(const object_type *o_ptr, bool hit_target);

		/**
		 * Determine if the player "hits" a monster.
		 */
		public static bool test_hit(int chance, int ac, int vis) {
			int k = Random.randint0(100);

			/* There is an automatic 12% chance to hit,
			 * and 5% chance to miss.
			 */
			if (k < 17) return k < 12;

			/* Penalize invisible targets */
			if (vis == 0) chance /= 2;

			/* Starting a bit higher up on the scale */
			if (chance < 9) chance = 9;

			/* Power competes against armor */
			return Random.randint0(chance) >= (ac * 2 / 3);
		}

		/**
		 * Attack the monster at the given location with a single blow.
		 */
		static bool py_attack_real(int y, int x, ref bool fear) {
			/* Information about the target of the attack */
			Monster.Monster m_ptr = Cave.cave_monster(Cave.cave, Cave.cave.m_idx[y][x]);
			Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];
			//char m_name[80];
			string m_name;
			bool stop = false;

			/* The weapon used */
			Object.Object o_ptr = Misc.p_ptr.inventory[Misc.INVEN_WIELD];

			/* Information about the attack */
			int bonus = Misc.p_ptr.state.to_h + o_ptr.to_h;
			int chance = Misc.p_ptr.state.skills[(int)Skill.TO_HIT_MELEE] + bonus * Misc.BTH_PLUS_ADJ;
			bool do_quake = false;
			bool success = false;

			/* Default to punching for one damage */
			string hit_verb = "punch";
			int dmg = 1;
			Message_Type msg_type = Message_Type.MSG_HIT;

			/* Extract monster name (or "it") */
			m_name = m_ptr.monster_desc(0);

			/* Auto-Recall if possible and visible */
			if (m_ptr.ml) Cave.monster_race_track(m_ptr.r_idx);

			/* Track a new monster */
			if (m_ptr.ml) Cave.health_track(Misc.p_ptr, Cave.cave.m_idx[y][x]);

			/* Handle player fear (only for invisible monsters) */
			if (Misc.p_ptr.check_state(Object_Flag.AFRAID, Misc.p_ptr.state.flags)) {
			    Utilities.msgt(Message_Type.MSG_AFRAID, "You are too afraid to attack %s!", m_name);
			    return false;
			}

			/* Disturb the monster */
			Monster.Monster.mon_clear_timed(Cave.cave.m_idx[y][x], (int)Misc.MON_TMD.SLEEP, Misc.MON_TMD_FLG_NOMESSAGE,false);

			/* See if the player hit */
			success = test_hit(chance, (int)r_ptr.ac, m_ptr.ml?1:0);

			/* If a miss, skip this hit */
			if (!success) {
			    Utilities.msgt(Message_Type.MSG_MISS, "You miss {0}.", m_name);
			    return false;
			}

			/* Handle normal weapon */
			if (o_ptr.kind != null) {
			    int i;
			    Slay best_s_ptr = null;

			    hit_verb = "hit";

			    /* Get the best attack from all slays or
			     * brands on all non-launcher equipment */
			    for (i = Misc.INVEN_LEFT; i < Misc.INVEN_TOTAL; i++) {
			        Object.Object obj = Misc.p_ptr.inventory[i];
			        if (obj.kind != null)
			            Slay.improve_attack_modifier(obj, m_ptr, ref best_s_ptr, true, false);
			    }

			    Slay.improve_attack_modifier(o_ptr, m_ptr, ref best_s_ptr, true, false);
			    if (best_s_ptr != null)
			        hit_verb = best_s_ptr.melee_verb;

			    dmg = Random.damroll(o_ptr.dd, o_ptr.ds);
			    dmg *= (best_s_ptr == null) ? 1 : best_s_ptr.mult;

			    dmg += o_ptr.to_d;
			    dmg = critical_norm(o_ptr.weight, o_ptr.to_h, dmg, ref msg_type);

			    /* Learn by use for the weapon */
			    o_ptr.notice_attack_plusses();

			    if (Misc.p_ptr.check_state(Object_Flag.IMPACT, Misc.p_ptr.state.flags) && dmg > 50) {
			        do_quake = true;
			        Object.Object.wieldeds_notice_flag(Misc.p_ptr, Object_Flag.IMPACT.value);
			    }
			}

			/* Learn by use for other equipped items */
			Object.Object.wieldeds_notice_on_attack();

			/* Apply the player damage bonuses */
			dmg += Misc.p_ptr.state.to_d;

			/* No negative damage */
			if (dmg <= 0) dmg = 0;

			/* Tell the player what happened */
			if (dmg <= 0)
			    Utilities.msgt(Message_Type.MSG_MISS, "You fail to harm {0}.", m_name);
			else if (msg_type == Message_Type.MSG_HIT)
			    Utilities.msgt(Message_Type.MSG_HIT, "You {0} {1}.", hit_verb, m_name);
			else if (msg_type == Message_Type.MSG_HIT_GOOD)
			    Utilities.msgt(Message_Type.MSG_HIT_GOOD, "You {0} {1}. {2}", hit_verb, m_name, "It was a good hit!");
			else if (msg_type == Message_Type.MSG_HIT_GREAT)
			    Utilities.msgt(Message_Type.MSG_HIT_GREAT, "You {0} {1}. {2}", hit_verb, m_name, "It was a great hit!");
			else if (msg_type == Message_Type.MSG_HIT_SUPERB)
			    Utilities.msgt(Message_Type.MSG_HIT_SUPERB, "You {0} {1}. {2}", hit_verb, m_name, "It was a superb hit!");
			else if (msg_type == Message_Type.MSG_HIT_HI_GREAT)
			    Utilities.msgt(Message_Type.MSG_HIT_HI_GREAT, "You {0} {1}. {2}", hit_verb, m_name, "It was a *GREAT* hit!");
			else if (msg_type == Message_Type.MSG_HIT_HI_SUPERB)
			    Utilities.msgt(Message_Type.MSG_HIT_HI_SUPERB, "You {0} {1}. {2}", hit_verb, m_name, "It was a *SUPERB* hit!");

			/* Complex message */
			if (Misc.p_ptr.wizard)
			    Utilities.msg("You do {0} (out of {1}) damage.", dmg, m_ptr.hp);

			/* Confusion attack */
			if (Misc.p_ptr.confusing != 0) {
				Misc.p_ptr.confusing = 0;//false;
			    Utilities.msg("Your hands stop glowing.");

				Monster.Monster.mon_inc_timed(Cave.cave.m_idx[y][x], (int)Misc.MON_TMD.CONF,
				        (10 + Random.randint0(Misc.p_ptr.lev) / 10), Misc.MON_TMD_FLG_NOTIFY, false);
			}

			/* Damage, check for fear and death */
			stop = Monster.Monster_Make.mon_take_hit(Cave.cave.m_idx[y][x], dmg, ref fear, null);

			if (stop)
			    fear = false;

			/* Apply earthquake brand */
			if (do_quake) {
				throw new NotImplementedException();
				//earthquake(Misc.p_ptr.py, Misc.p_ptr.px, 10);
				//if (Cave.cave.m_idx[y][x] == 0) stop = true;
			}

			return stop;
		}

		/**
		 * Determine damage for critical hits from melee.
		 *
		 * Factor in weapon weight, total plusses, player level.
		 */
		static int critical_norm(int weight, int plus, int dam, ref Message_Type msg_type) {
			throw new NotImplementedException();
			//int chance = weight + (p_ptr.state.to_h + plus) * 5 + p_ptr.lev * 3;
			//int power = weight + randint1(650);

			//if (randint1(5000) > chance) {
			//    *msg_type = MSG_HIT;
			//    return dam;

			//} else if (power < 400) {
			//    *msg_type = MSG_HIT_GOOD;
			//    return 2 * dam + 5;

			//} else if (power < 700) {
			//    *msg_type = MSG_HIT_GREAT;
			//    return 2 * dam + 10;

			//} else if (power < 900) {
			//    *msg_type = MSG_HIT_SUPERB;
			//    return 3 * dam + 15;

			//} else if (power < 1300) {
			//    *msg_type = MSG_HIT_HI_GREAT;
			//    return 3 * dam + 20;

			//} else {
			//    *msg_type = MSG_HIT_HI_SUPERB;
			//    return 4 * dam + 20;
			//}
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
				Monster_Message.add_monster_message(m_name, Cave.cave.m_idx[y][x], (int)MON_MSG.FLEE_IN_TERROR, true);
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
