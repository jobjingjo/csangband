using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Monster;
using CSAngband.Object;

namespace CSAngband {
	static class Attack {
		public class attack_result {
			public attack_result(bool success, int dmg, Message_Type message_type, string verb){
				this.success = success;
				this.dmg = dmg;
				this.msg_type = message_type;
				this.hit_verb = verb;
			}
			public bool success;
			public int dmg;
			public Message_Type msg_type;
			public string hit_verb;
		};

		//public static int breakage_chance(const object_type *o_ptr, bool hit_target);

		/**
		 * Determine if the player "hits" a monster.
		 */
		public static bool test_hit(int chance, int ac, bool vis) {
			int k = Random.randint0(100);

			/* There is an automatic 12% chance to hit,
			 * and 5% chance to miss.
			 */
			if (k < 17) return k < 12;

			/* Penalize invisible targets */
			if (vis == false) chance /= 2;

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
			    Utilities.msgt(Message_Type.MSG_AFRAID, "You are too afraid to attack {0}!", m_name);
			    return false;
			}

			/* Disturb the monster */
			Monster.Monster.mon_clear_timed(Cave.cave.m_idx[y][x], (int)Misc.MON_TMD.SLEEP, Misc.MON_TMD_FLG_NOMESSAGE,false);

			/* See if the player hit */
			success = test_hit(chance, (int)r_ptr.ac, m_ptr.ml);

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

				Monster.Monster.mon_inc_timed(Cave.cave.m_idx[y][x], Misc.MON_TMD.CONF,
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
			int chance = weight + (Misc.p_ptr.state.to_h + plus) * 5 + Misc.p_ptr.lev * 3;
			int power = weight + Random.randint1(650);

			if (Random.randint1(5000) > chance) {
			    msg_type = Message_Type.MSG_HIT;
			    return dam;

			} else if (power < 400) {
			    msg_type = Message_Type.MSG_HIT_GOOD;
			    return 2 * dam + 5;

			} else if (power < 700) {
			    msg_type = Message_Type.MSG_HIT_GREAT;
			    return 2 * dam + 10;

			} else if (power < 900) {
			    msg_type = Message_Type.MSG_HIT_SUPERB;
			    return 3 * dam + 15;

			} else if (power < 1300) {
			    msg_type = Message_Type.MSG_HIT_HI_GREAT;
			    return 3 * dam + 20;

			} else {
			    msg_type = Message_Type.MSG_HIT_HI_SUPERB;
			    return 4 * dam + 20;
			}
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
				Monster_Message.add_monster_message(m_name, Cave.cave.m_idx[y][x], MON_MSG.FLEE_IN_TERROR, true);
			}
		}

		/**
		 * ranged_attack is a function pointer, used to execute a kind of attack.
		 *
		 * This allows us to abstract details of throwing, shooting, etc. out while
		 * keeping the core projectile tracking, monster cleanup, and display code
		 * in common.
		 */
		public delegate attack_result ranged_attack(Object.Object o_ptr, int y, int x);
		
		/**
		 * Helper function used with ranged_helper by do_cmd_fire.
		 */
		public static attack_result make_ranged_shot(Object.Object o_ptr, int y, int x) {
			attack_result result = new attack_result(false, 0, 0, "hit");

			Object.Object j_ptr = Misc.p_ptr.inventory[Misc.INVEN_BOW];

			Monster.Monster m_ptr = Cave.cave_monster(Cave.cave, Cave.cave.m_idx[y][x]);
			Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			int bonus = Misc.p_ptr.state.to_h + o_ptr.to_h + j_ptr.to_h;
			int chance = Misc.p_ptr.state.skills[(int)Skill.TO_HIT_BOW] + bonus * Misc.BTH_PLUS_ADJ;
			int chance2 = chance - Cave.distance(Misc.p_ptr.py, Misc.p_ptr.px, y, x);

			int multiplier = Misc.p_ptr.state.ammo_mult;
			Slay best_s_ptr = null;

			/* Did we hit it (penalize distance travelled) */
			if (!test_hit(chance2, r_ptr.ac, m_ptr.ml)) return result;

			result.success = true;

			Slay.improve_attack_modifier(o_ptr, m_ptr, ref best_s_ptr, true, false);
			Slay.improve_attack_modifier(j_ptr, m_ptr, ref best_s_ptr, true, false);

			/* If we have a slay, modify the multiplier appropriately */
			if (best_s_ptr != null) {
			    result.hit_verb = best_s_ptr.range_verb;
			    multiplier += best_s_ptr.mult;
			}

			/* Apply damage: multiplier, slays, criticals, bonuses */
			result.dmg = Random.damroll(o_ptr.dd, o_ptr.ds);
			result.dmg += o_ptr.to_d + j_ptr.to_d;
			result.dmg *= multiplier;
			result.dmg = critical_shot(o_ptr.weight, o_ptr.to_h, result.dmg, ref result.msg_type);

			Misc.p_ptr.inventory[Misc.INVEN_BOW].notice_attack_plusses();

			return result;
		}

		/**
		 * Determine damage for critical hits from shooting.
		 *
		 * Factor in item weight, total plusses, and player level.
		 */
		static int critical_shot(int weight, int plus, int dam, ref Message_Type msg_type) {
			int chance = weight + (Misc.p_ptr.state.to_h + plus) * 4 + Misc.p_ptr.lev * 2;
			int power = weight + Random.randint1(500);

			if (Random.randint1(5000) > chance) {
				msg_type = Message_Type.MSG_SHOOT_HIT;
				return dam;

			} else if (power < 500) {
				msg_type = Message_Type.MSG_HIT_GOOD;
				return 2 * dam + 5;

			} else if (power < 1000) {
				msg_type = Message_Type.MSG_HIT_GREAT;
				return 2 * dam + 10;

			} else {
				msg_type = Message_Type.MSG_HIT_SUPERB;
				return 3 * dam + 15;
			}
		}

		/**
		 * This is a helper function used by do_cmd_throw and do_cmd_fire.
		 *
		 * It abstracts out the projectile path, display code, identify and clean up
		 * logic, while using the 'attack' parameter to do work particular to each
		 * kind of attack.
		 */
		public static void ranged_helper(int item, int dir, int range, int shots, ranged_attack attack) {
			/* Get the ammo */
			Object.Object o_ptr = Object.Object.object_from_item_idx(item);

			int i, j;
			ConsoleColor missile_attr = o_ptr.object_attr();
			char missile_char = o_ptr.object_char();

			//object_type object_type_body;
			Object.Object i_ptr = new Object.Object();//&object_type_body;

			//char o_name[80];
			string o_name;

			int path_n;
			List<ushort> path_g = new List<ushort>();//[256];

			int msec = Player.Player_Other.instance.delay_factor;

			/* Start at the player */
			int x = Misc.p_ptr.px;
			int y = Misc.p_ptr.py;

			/* Predict the "target" location */
			short ty = (short)(y + 99 * Misc.ddy[dir]);
			short tx = (short)(x + 99 * Misc.ddx[dir]);

			bool hit_target = false;

			/* Check for target validity */
			if ((dir == 5) && Target.okay()) {
				int taim;
				//char msg[80];
				string msg;
				Target.get(out tx, out ty);
				taim = Cave.distance(y, x, ty, tx);
				if (taim > range) {
					msg = String.Format("Target out of range by {0} squares. Fire anyway? ", taim - range);
					if (!Utilities.get_check(msg)) return;
				}
			}

			/* Sound */
			//sound(MSG_SHOOT); //later

			o_ptr.notice_on_firing();

			/* Describe the object */
			o_name = o_ptr.object_desc(Object.Object.Detail.FULL | Object.Object.Detail.SINGULAR);

			/* Actually "fire" the object -- Take a partial turn */
			Misc.p_ptr.energy_use = (short)(100 / shots);

			/* Calculate the path */
			path_n = Cave.project_path(out path_g, range, y, x, ty, tx, 0);

			/* Hack -- Handle stuff */
			Misc.p_ptr.handle_stuff();

			/* Start at the player */
			x = Misc.p_ptr.px;
			y = Misc.p_ptr.py;

			/* Project along the path */
			for (i = 0; i < path_n; ++i) {
				int ny = Cave.GRID_Y(path_g[i]);
				int nx = Cave.GRID_X(path_g[i]);

				/* Hack -- Stop before hitting walls */
				if (!Cave.cave_floor_bold(ny, nx)) break;

				/* Advance */
				x = nx;
				y = ny;

				/* Only do visuals if the player can "see" the missile */
				if (Cave.player_can_see_bold(y, x)) {
					Cave.print_rel(missile_char, missile_attr, y, x);
					Cave.move_cursor_relative(y, x);

					Term.fresh();
					if (Misc.p_ptr.redraw != 0) Misc.p_ptr.redraw_stuff();

					Term.xtra(TERM_XTRA.DELAY, msec);
					Cave.cave_light_spot(Cave.cave, y, x);

					Term.fresh();
					if (Misc.p_ptr.redraw != 0) Misc.p_ptr.redraw_stuff();
				} else {
					/* Delay anyway for consistency */
					Term.xtra(TERM_XTRA.DELAY, msec);
				}

				/* Handle monster */
				if (Cave.cave.m_idx[y][x] > 0) break;
			}

			/* Try the attack on the monster at (x, y) if any */
			if (Cave.cave.m_idx[y][x] > 0) {
				Monster.Monster m_ptr = Cave.cave_monster(Cave.cave, Cave.cave.m_idx[y][x]);
				Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];
				bool visible = m_ptr.ml;

				bool fear = false;
				//char m_name[80];
				string m_name;
				string note_dies = r_ptr.monster_is_unusual() ? " is destroyed." : " dies.";

				attack_result result = attack(o_ptr, y, x);
				int dmg = result.dmg;
				Message_Type msg_type = result.msg_type;
				string hit_verb = result.hit_verb;

				if (result.success) {
					hit_target = true;

					/* Get "the monster" or "it" */
					m_name = m_ptr.monster_desc(0);
		
					o_ptr.notice_attack_plusses();
		
					/* No negative damage; change verb if no damage done */
					if (dmg <= 0) {
						dmg = 0;
						hit_verb = "fail to harm";
					}
		
					if (!visible) {
						/* Invisible monster */
						Utilities.msgt(Message_Type.MSG_SHOOT_HIT, "The {0} finds a mark.", o_name);
					} else {
						/* Visible monster */
						if ((Message_Type)msg_type == Message_Type.MSG_SHOOT_HIT)
							Utilities.msgt(Message_Type.MSG_SHOOT_HIT, "The {0} {1} {2}.", o_name, hit_verb, m_name);
						else if ((Message_Type)msg_type == Message_Type.MSG_HIT_GOOD) {
							Utilities.msgt(Message_Type.MSG_HIT_GOOD, "The {0} {1} {2}. {3}", o_name, hit_verb, m_name, 
								"It was a good hit!");
						} else if ((Message_Type)msg_type == Message_Type.MSG_HIT_GREAT) {
							Utilities.msgt(Message_Type.MSG_HIT_GREAT, "The {0} {1} {2}. {3}", o_name, hit_verb, m_name,
								 "It was a great hit!");
						} else if ((Message_Type)msg_type == Message_Type.MSG_HIT_SUPERB) {
							Utilities.msgt(Message_Type.MSG_HIT_SUPERB, "The {0} {1} {2}. {3}", o_name, hit_verb, m_name,
								 "It was a superb hit!");
						}
		
						/* Track this monster */
						if (m_ptr.ml) Cave.monster_race_track(m_ptr.r_idx);
						if (m_ptr.ml) Cave.health_track(Misc.p_ptr, Cave.cave.m_idx[y][x]);
					}
		
					/* Complex message */
					if (Misc.p_ptr.wizard)
						Utilities.msg("You do {0} (out of {1}) damage.", dmg, m_ptr.hp);
		
					/* Hit the monster, check for death */
					if (!Monster_Make.mon_take_hit(Cave.cave.m_idx[y][x], dmg, ref fear, note_dies)) {
						Monster_Message.message_pain(Cave.cave.m_idx[y][x], dmg);
						if (fear && m_ptr.ml)
							Monster_Message.add_monster_message(m_name, Cave.cave.m_idx[y][x], MON_MSG.FLEE_IN_TERROR, true);
					}
				}
			}

			/* Obtain a local object */
			i_ptr = o_ptr.copy();
			i_ptr.split(o_ptr, 1);

			/* See if the ammunition broke or not */
			j = i_ptr.breakage_chance(hit_target);

			/* Drop (or break) near that location */
			Object.Object.drop_near(Cave.cave, i_ptr, j, y, x, true);

			if (item >= 0) {
				/* The ammo is from the inventory */
				Object.Object.inven_item_increase(item, -1);
				Object.Object.inven_item_describe(item);
				Object.Object.inven_item_optimize(item);
			} else {
				/* The ammo is from the floor */
				Object.Object.floor_item_increase(0 - item, -1);
				Object.Object.floor_item_optimize(0 - item);
			}
		}
	}
}
