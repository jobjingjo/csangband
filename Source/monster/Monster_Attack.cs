﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBM = CSAngband.Monster.Monster_Blow.RBM;
using RBE = CSAngband.Monster.Monster_Blow.RBE;

namespace CSAngband.Monster {
	partial class Monster {
		/*
		 * Determine if a monster attack against the player succeeds.
		 */
		bool check_hit(Player.Player p, int power, int level)
		{
			int chance, ac;

			/* Calculate the "attack quality" */
			chance = (power + (level * 3));

			/* Total armor */
			ac = p.state.ac + p.state.to_a;

			/* if the monster checks vs ac, the player learns ac bonuses */
			/* XXX Eddie should you only learn +ac on miss, -ac on hit?  who knows */
			throw new NotImplementedException();
			//object_notice_on_defend(p);

			///* Check if the player was hit */
			//return test_hit(chance, ac, true);
		}

		/*
		 * Attack the player via physical attacks.
		 */
		//TODO: Submit this function to the Guiness book of world records for "Longest fucking function"
		//TODO: Refactor this. Refactor this so fucking hard.
		public bool make_attack_normal(Player.Player p)
		{
			Monster_Race r_ptr = Misc.r_info[r_idx];

			Monster_Lore l_ptr = Misc.l_list[r_idx];

			int i, k, tmp;

			short gold;

			string o_name;


			// Not allowed to attack
			if (Race.flags.has(Monster_Flag.NEVER_BLOW.value)) return false;

			// Total armor
			int ac = p.state.ac + p.state.to_a;

			// Extract the effective monster level
			int rlev = ((Race.level >= 1) ? Race.level : 1);


			// Get the monster name (or "it")
			string m_name = monster_desc(0);

			// Get the "died from" information (i.e. "a kobold")
			string ddesc = monster_desc(Desc.SHOW | Desc.IND2);

			// Assume no blink
			bool blinked = false;

			// Scan through all blows
			for (int ap_cnt = 0; ap_cnt < Monster_Blow.MONSTER_BLOW_MAX; ap_cnt++)
			{
			    bool visible = false;
			    bool obvious = false;
			    bool do_break = false;

			    int power = 0;
			    int damage = 0;

			    string act = null;

			    // Extract the attack infomation
			    RBE effect = Race.blow[ap_cnt].effect;
			    RBM method = Race.blow[ap_cnt].method;
			    int d_dice = Race.blow[ap_cnt].d_dice;
			    int d_side = Race.blow[ap_cnt].d_side;


			    // Hack -- no more attacks
			    if (method == RBM.NONE) break;

			    // Handle "leaving"
			    if (p.leaving) break;

			    // Extract visibility (before blink)
			    if (ml) visible = true;

			    // Extract visibility from carrying light
			    if (Race.flags.has(Monster_Flag.HAS_LIGHT.value)) visible = true;

			    power = effect.power;

			    // Monster hits player
			    if (effect == null || check_hit(p, power, rlev))
			    {
			        // Always disturbing
			        Cave.disturb(p, 1, 0);

			        // Hack -- Apply "protection from evil"
			        if (p.timed[(int)Timed_Effect.PROTEVIL] > 0)
			        {
			            // Learn about the evil flag
			            if (ml)
			            {
			                l_ptr.flags.on(Monster_Flag.EVIL.value);
			            }

			            if (r_ptr.flags.has(Monster_Flag.EVIL.value) && p.lev >= rlev && Random.randint0(100) + p.lev > 50)
			            {
			                // Message
			                Utilities.msg("%^s is repelled.", m_name);

			                // Hack -- Next attack
			                continue;
			            }
			        }


			        // Assume no cut or stun
			        bool do_cut = method.do_cut;
			        bool do_stun = method.do_stun;

			        // Assume no sound
			        Message_Type sound_msg = method.sound_msg;

			        act = method.action;

			        // Describe the attack method
			        if (method == RBM.INSULT){
						throw new NotImplementedException();
			            //act = desc_insult[Random.randint0(Misc.MAX_DESC_INSULT)];
			        } else if (method == RBM.MOAN){
						throw new NotImplementedException();
			            //act = desc_moan[Random.randint0(Misc.MAX_DESC_MOAN)];
			        }

			        // Message
			        if (act != null)
			            Utilities.msgt(sound_msg, "%^s %s", m_name, act);


			        // Hack -- assume all attacks are obvious
			        obvious = true;

			        // Roll out the damage
			        if (d_dice > 0 && d_side > 0)
			            damage = Random.damroll(d_dice, d_side);
			        else
			            damage = 0;

			        // Apply appropriate damage
					if (effect.value == 0){
			            // Hack -- Assume obvious
			            obvious = true;

			            // Hack -- No damage
			            damage = 0;

			            break;
			        } else if (effect.value == RBE.HURT.value)
			        {
						throw new NotImplementedException();
						//// Obvious
						//obvious = true;

						//// Hack -- Player armor reduces total damage
						//damage -= (damage * ((ac < 240) ? ac : 240) / 400);

						//// Take damage
						//take_hit(p, damage, ddesc);

						//break;
			        } else if ( effect.value == RBE.POISON.value)
			        {
						throw new NotImplementedException();
						//damage = adjust_dam(p, GF_POIS, damage, RANDOMISE,
						//    check_for_resist(p, GF_POIS, p.state.flags, true));

						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Take "poison" effect
						//if (player_inc_timed(p, TMD_POISONED, randint1(rlev) + 5, true, true))
						//    obvious = true;

						//// Learn about the player
						//monster_learn_resists(m_ptr, p, GF_POIS);

						//break;
			        } else if (effect.value == RBE.UN_BONUS.value)
			        {
						throw new NotImplementedException();
						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Allow complete resist
						//if (!check_state(p, OF_RES_DISEN, p.state.flags))
						//{
						//    // Apply disenchantment
						//    if (apply_disenchant(0)) obvious = true;
						//}

						//// Learn about the player
						//monster_learn_resists(m_ptr, p, GF_DISEN);

						//break;
			        } else if (effect.value == RBE.UN_POWER.value)
			        {
						throw new NotImplementedException();
						//int unpower = 0, newcharge;

						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Find an item
						//for (k = 0; k < 10; k++)
						//{
						//    // Pick an item
						//    i = randint0(INVEN_PACK);

						//    // Obtain the item
						//    o_ptr = &p.inventory[i];

						//    // Skip non-objects
						//    if (!o_ptr.kind) continue;

						//    // Drain charged wands/staves
						//    if ((o_ptr.tval == TV_STAFF) ||
						//        (o_ptr.tval == TV_WAND))
						//    {
						//        // Charged?
						//        if (o_ptr.pval[DEFAULT_PVAL])
						//        {
						//            // Get number of charge to drain
						//            unpower = (rlev / (o_ptr.kind.level + 2)) + 1;

						//            // Get new charge value, don't allow negative
						//            newcharge = MAX((o_ptr.pval[DEFAULT_PVAL]
						//                    - unpower),0);
								
						//            // Remove the charges
						//            o_ptr.pval[DEFAULT_PVAL] = newcharge;
						//        }
						//    }

						//    if (unpower)
						//    {
						//        int heal = rlev * unpower;

						//        msg("Energy drains from your pack!");

						//        obvious = true;

						//        // Don't heal more than max hp
						//        heal = MIN(heal, maxhp - hp);

						//        // Heal
						//        hp += heal;

						//        // Redraw (later) if needed
						//        if (cave_monster(cave, p.health_who) == m_ptr)
						//            p.redraw |= (PR_HEALTH);

						//        // Combine / Reorder the pack
						//        p.notice |= (PN_COMBINE | PN_REORDER);

						//        // Redraw stuff
						//        p.redraw |= (PR_INVEN);

						//        // Affect only a single inventory slot
						//        break;
						//    }
						//}

						//break;
			        } else if (effect.value == RBE.EAT_GOLD.value)
			        {
						throw new NotImplementedException();
						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Obvious
						//obvious = true;

						//// Saving throw (unless paralyzed) based on dex and level
						//if (!p.timed[TMD_PARALYZED] &&
						//    (randint0(100) < (adj_dex_safe[p.state.stat_ind[A_DEX]] +
						//                        p.lev)))
						//{
						//    // Saving throw message
						//    msg("You quickly protect your money pouch!");

						//    // Occasional blink anyway
						//    if (randint0(3)) blinked = true;
						//}

						//// Eat gold
						//else {
						//    gold = (p.au / 10) + randint1(25);
						//    if (gold < 2) gold = 2;
						//    if (gold > 5000) gold = (p.au / 20) + randint1(3000);
						//    if (gold > p.au) gold = p.au;
						//    p.au -= gold;
						//    if (gold <= 0) {
						//        msg("Nothing was stolen.");
						//        break;
						//    }
						//    // Let the player know they were robbed
						//    msg("Your purse feels lighter.");
						//    if (p.au)
						//        msg("%ld coins were stolen!", (long)gold);
						//    else
						//        msg("All of your coins were stolen!");

						//    // While we have gold, put it in objects
						//    while (gold > 0) {
						//        int amt;

						//        // Create a new temporary object
						//        object_type o;
						//        object_wipe(&o);
						//        object_prep(&o, objkind_get(TV_GOLD, SV_GOLD), 0, MINIMISE);

						//        // Amount of gold to put in this object
						//        amt = gold > MAX_PVAL ? MAX_PVAL : gold;
						//        o.pval[DEFAULT_PVAL] = amt;
						//        gold -= amt;

						//        // Set origin to stolen, so it is not confused with
						//        // dropped treasure in monster_death
						//        o.origin = ORIGIN_STOLEN;

						//        // Give the gold to the monster
						//        monster_carry(m_ptr, &o);
						//    }

						//    // Redraw gold
						//    p.redraw |= (PR_GOLD);

						//    // Blink away
						//    blinked = true;
						//}

						//break;
			        } else if (effect.value == RBE.EAT_ITEM.value)
			        {
						throw new NotImplementedException();
						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Saving throw (unless paralyzed) based on dex and level
						//if (!p.timed[TMD_PARALYZED] &&
						//    (randint0(100) < (adj_dex_safe[p.state.stat_ind[A_DEX]] +
						//                        p.lev)))
						//{
						//    // Saving throw message
						//    msg("You grab hold of your backpack!");

						//    // Occasional "blink" anyway
						//    blinked = true;

						//    // Obvious 
						//    obvious = true;

						//    // Done 
						//    break;
						//}

						//// Find an item 
						//for (k = 0; k < 10; k++)
						//{
						//    object_type *i_ptr;
						//    object_type object_type_body;

						//    // Pick an item 
						//    i = randint0(INVEN_PACK);

						//    // Obtain the item 
						//    o_ptr = &p.inventory[i];

						//    // Skip non-objects 
						//    if (!o_ptr.kind) continue;

						//    // Skip artifacts 
						//    if (o_ptr.artifact) continue;

						//    // Get a description 
						//    object_desc(o_name, sizeof(o_name), o_ptr, ODESC_FULL);

						//    // Message 
						//    msg("%sour %s (%c) was stolen!",
						//                ((o_ptr.number > 1) ? "One of y" : "Y"),
						//                o_name, index_to_label(i));

						//    // Get local object 
						//    i_ptr = &object_type_body;

						//    // Obtain local object 
						//    object_copy(i_ptr, o_ptr);

						//    // Modify number
						//    i_ptr.number = 1;

						//    // Hack -- If a rod, staff, or wand, allocate total
						//    // maximum timeouts or charges between those
						//    // stolen and those missed. -LM-
						//    distribute_charges(o_ptr, i_ptr, 1);

						//    // Carry the object
						//    (void)monster_carry(m_ptr, i_ptr);

						//    // Steal the items
						//    inven_item_increase(i, -1);
						//    inven_item_optimize(i);

						//    // Obvious
						//    obvious = true;

						//    // Blink away
						//    blinked = true;

						//    // Done
						//    break;
						//}

						//break;
			        } else if (effect.value == RBE.EAT_FOOD.value)
			        {
						throw new NotImplementedException();
						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Steal some food
						//for (k = 0; k < 10; k++)
						//{
						//    // Pick an item from the pack
						//    i = randint0(INVEN_PACK);

						//    // Get the item
						//    o_ptr = &p.inventory[i];

						//    // Skip non-objects
						//    if (!o_ptr.kind) continue;

						//    // Skip non-food objects
						//    if (o_ptr.tval != TV_FOOD) continue;

						//    // Get a description
						//    object_desc(o_name, sizeof(o_name), o_ptr,
						//                ODESC_PREFIX | ODESC_BASE);

						//    // Message
						//    msg("%sour %s (%c) was eaten!",
						//                ((o_ptr.number > 1) ? "One of y" : "Y"),
						//                o_name, index_to_label(i));

						//    // Steal the items
						//    inven_item_increase(i, -1);
						//    inven_item_optimize(i);

						//    // Obvious
						//    obvious = true;

						//    // Done
						//    break;
						//}

						//break;
			        } else if (effect.value == RBE.EAT_LIGHT.value)
			        {
						throw new NotImplementedException();
						//bitflag f[OF_SIZE];

						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Get the light, and its flags
						//o_ptr = &p.inventory[INVEN_LIGHT];
						//object_flags(o_ptr, f);

						//// Drain fuel where applicable
						//if (!of_has(f, OF_NO_FUEL) && (o_ptr.timeout > 0))
						//{
						//    // Reduce fuel
						//    o_ptr.timeout -= (250 + randint1(250));
						//    if (o_ptr.timeout < 1) o_ptr.timeout = 1;

						//    // Notice
						//    if (!p.timed[TMD_BLIND])
						//    {
						//        msg("Your light dims.");
						//        obvious = true;
						//    }

						//    // Redraw stuff
						//    p.redraw |= (PR_EQUIP);
						//}

						//break;
			        } else if (effect.value == RBE.ACID.value)
			        {
						throw new NotImplementedException();
						//// Obvious
						//obvious = true;

						//// Message
						//msg("You are covered in acid!");

						//// Special damage
						//damage = adjust_dam(p, GF_ACID, damage, RANDOMISE, 
						//    check_for_resist(p, GF_ACID, p.state.flags, true));
						//if (damage) {
						//    take_hit(p, damage, ddesc);
						//    inven_damage(p, GF_ACID, MIN(damage * 5, 300));
						//}

						//// Learn about the player
						//monster_learn_resists(m_ptr, p, GF_ACID);

						//break;
			        } else if (effect.value == RBE.ELEC.value)
			        {
						throw new NotImplementedException();
						//// Obvious
						//obvious = true;

						//// Message
						//msg("You are struck by electricity!");

						//// Take damage (special)
						//damage = adjust_dam(p, GF_ELEC, damage, RANDOMISE,
						//    check_for_resist(p, GF_ELEC, p.state.flags, true));
						//if (damage) {
						//    take_hit(p, damage, ddesc);
						//    inven_damage(p, GF_ELEC, MIN(damage * 5, 300));
						//}

						//// Learn about the player
						//monster_learn_resists(m_ptr, p, GF_ELEC);

						//break;
			        } else if (effect.value == RBE.FIRE.value)
			        {
						throw new NotImplementedException();
						//// Obvious
						//obvious = true;

						//// Message
						//msg("You are enveloped in flames!");

						//// Take damage (special)
						//damage = adjust_dam(p, GF_FIRE, damage, RANDOMISE,
						//    check_for_resist(p, GF_FIRE, p.state.flags, true));
						//if (damage) {
						//    take_hit(p, damage, ddesc);
						//    inven_damage(p, GF_FIRE, MIN(damage * 5, 300));
						//}

						//// Learn about the player
						//monster_learn_resists(m_ptr, p, GF_FIRE);

						//break;
			        } else if (effect.value == RBE.COLD.value)
			        {
						throw new NotImplementedException();
						//// Obvious
						//obvious = true;

						//// Message
						//msg("You are covered with frost!");

						//// Take damage (special)
						//damage = adjust_dam(p, GF_COLD, damage, RANDOMISE,
						//    check_for_resist(p, GF_COLD, p.state.flags, true));
						//if (damage) {
						//    take_hit(p, damage, ddesc);
						//    inven_damage(p, GF_COLD, MIN(damage * 5, 300));
						//}

						//// Learn about the player
						//monster_learn_resists(m_ptr, p, GF_COLD);

						//break;
			        } else if (effect.value == RBE.BLIND.value)
			        {
						throw new NotImplementedException();
						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Increase "blind"
						//if (player_inc_timed(p, TMD_BLIND, 10 + randint1(rlev), true, true))
						//    obvious = true;

						//// Learn about the player
						//update_smart_learn(m_ptr, p, OF_RES_BLIND);

						//break;
			        } else if (effect.value == RBE.CONFUSE.value)
			        {
						throw new NotImplementedException();
						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Increase "confused"
						//if (player_inc_timed(p, TMD_CONFUSED, 3 + randint1(rlev), true, true))
						//    obvious = true;

						//// Learn about the player
						//update_smart_learn(m_ptr, p, OF_RES_CONFU);

						//break;
			        } else if (effect.value == RBE.TERRIFY.value)
			        {
						throw new NotImplementedException();
						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Increase "afraid"
						//if (randint0(100) < p.state.skills[SKILL_SAVE])
						//{
						//    msg("You stand your ground!");
						//    obvious = true;
						//}
						//else
						//{
						//    if (player_inc_timed(p, TMD_AFRAID, 3 + randint1(rlev), true,
						//            true))
						//        obvious = true;
						//}

						//// Learn about the player
						//update_smart_learn(m_ptr, p, OF_RES_FEAR);

						//break;
			        } else if(effect.value == RBE.PARALYZE.value)
			        {
						throw new NotImplementedException();
						//// Hack -- Prevent perma-paralysis via damage
						//if (p.timed[TMD_PARALYZED] && (damage < 1)) damage = 1;

						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Increase "paralyzed"
						//if (randint0(100) < p.state.skills[SKILL_SAVE])
						//{
						//    msg("You resist the effects!");
						//    obvious = true;
						//}
						//else
						//{
						//    if (player_inc_timed(p, TMD_PARALYZED, 3 + randint1(rlev), true,
						//            true))
						//        obvious = true;
						//}

						//// Learn about the player
						//update_smart_learn(m_ptr, p, OF_FREE_ACT);

						//break;
			        } else if (effect.value == RBE.LOSE_STR.value)
			        {
						throw new NotImplementedException();
						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Damage (stat)
						//if (do_dec_stat(A_STR, false)) obvious = true;

						//break;
			        } else if (effect.value == RBE.LOSE_INT.value)
			        {
						throw new NotImplementedException();
						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Damage (stat)
						//if (do_dec_stat(A_INT, false)) obvious = true;

						//break;
			        } else if (effect.value == RBE.LOSE_WIS.value)
			        {
						throw new NotImplementedException();
						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Damage (stat)
						//if (do_dec_stat(A_WIS, false)) obvious = true;

						//break;
			        } else if (effect.value == RBE.LOSE_DEX.value)
			        {
						throw new NotImplementedException();
						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Damage (stat)
						//if (do_dec_stat(A_DEX, false)) obvious = true;

						//break;
			        } else if (effect.value == RBE.LOSE_CON.value)
			        {
						throw new NotImplementedException();
						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Damage (stat)
						//if (do_dec_stat(A_CON, false)) obvious = true;

						//break;
			        } else if (effect.value == RBE.LOSE_CHR.value)
			        {
						throw new NotImplementedException();
						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Damage (stat)
						//if (do_dec_stat(A_CHR, false)) obvious = true;

						//break;
			        } else if (effect.value == RBE.LOSE_ALL.value)
			        {
						throw new NotImplementedException();
						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Damage (stats)
						//if (do_dec_stat(A_STR, false)) obvious = true;
						//if (do_dec_stat(A_DEX, false)) obvious = true;
						//if (do_dec_stat(A_CON, false)) obvious = true;
						//if (do_dec_stat(A_INT, false)) obvious = true;
						//if (do_dec_stat(A_WIS, false)) obvious = true;
						//if (do_dec_stat(A_CHR, false)) obvious = true;

						//break;
			        } else if (effect.value == RBE.SHATTER.value)
			        {
						throw new NotImplementedException();
						//// Obvious
						//obvious = true;

						//// Hack -- Reduce damage based on the player armor class
						//damage -= (damage * ((ac < 240) ? ac : 240) / 400);

						//// Take damage
						//take_hit(p, damage, ddesc);

						//// Radius 8 earthquake centered at the monster
						//if (damage > 23)
						//{
						//    int px_old = p.px;
						//    int py_old = p.py;

						//    earthquake(fy, fx, 8);

						//    // Stop the blows if the player is pushed away
						//    if ((px_old != p.px) ||
						//        (py_old != p.py))
						//        do_break = true;
						//}
						//break;
			        } else if (effect.value == RBE.EXP_10.value)
			        {
						throw new NotImplementedException();
						//// Obvious
						//obvious = true;

						//// Take damage
						//take_hit(p, damage, ddesc);
						//update_smart_learn(m_ptr, p_ptr, OF_HOLD_LIFE);

						//if (check_state(p, OF_HOLD_LIFE, p.state.flags) && (randint0(100) < 95))
						//{
						//    msg("You keep hold of your life force!");
						//}
						//else
						//{
						//    s32b d = damroll(10, 6) + (p.exp/100) * MON_DRAIN_LIFE;
						//    if (check_state(p, OF_HOLD_LIFE, p.state.flags))
						//    {
						//        msg("You feel your life slipping away!");
						//        player_exp_lose(p, d / 10, false);
						//    }
						//    else
						//    {
						//        msg("You feel your life draining away!");
						//        player_exp_lose(p, d, false);
						//    }
						//}

						//break;
			        } else if (effect.value == RBE.EXP_20.value)
			        {
						throw new NotImplementedException();
						//// Obvious
						//obvious = true;

						//// Take damage
						//take_hit(p, damage, ddesc);
						//update_smart_learn(m_ptr, p_ptr, OF_HOLD_LIFE);

						//if (check_state(p, OF_HOLD_LIFE, p.state.flags) && (randint0(100) < 90))
						//{
						//    msg("You keep hold of your life force!");
						//}
						//else
						//{
						//    s32b d = damroll(20, 6) + (p.exp / 100) * MON_DRAIN_LIFE;

						//    if (check_state(p, OF_HOLD_LIFE, p.state.flags))
						//    {
						//        msg("You feel your life slipping away!");
						//        player_exp_lose(p, d / 10, false);
						//    }
						//    else
						//    {
						//        msg("You feel your life draining away!");
						//        player_exp_lose(p, d, false);
						//    }
						//}
						//break;
			        } else if (effect.value == RBE.EXP_40.value)
			        {
						throw new NotImplementedException();
						//// Obvious
						//obvious = true;

						//// Take damage
						//take_hit(p, damage, ddesc);
						//update_smart_learn(m_ptr, p_ptr, OF_HOLD_LIFE);

						//if (check_state(p, OF_HOLD_LIFE, p.state.flags) && (randint0(100) < 75))
						//{
						//    msg("You keep hold of your life force!");
						//}
						//else
						//{
						//    s32b d = damroll(40, 6) + (p.exp / 100) * MON_DRAIN_LIFE;

						//    if (check_state(p, OF_HOLD_LIFE, p.state.flags))
						//    {
						//        msg("You feel your life slipping away!");
						//        player_exp_lose(p, d / 10, false);
						//    }
						//    else
						//    {
						//        msg("You feel your life draining away!");
						//        player_exp_lose(p, d, false);
						//    }
						//}
						//break;
			        } else if (effect.value == RBE.EXP_80.value)
			        {
						throw new NotImplementedException();
						//// Obvious
						//obvious = true;

						//// Take damage
						//take_hit(p, damage, ddesc);
						//update_smart_learn(m_ptr, p_ptr, OF_HOLD_LIFE);

						//if (check_state(p, OF_HOLD_LIFE, p.state.flags) && (randint0(100) < 50))
						//{
						//    msg("You keep hold of your life force!");
						//}
						//else
						//{
						//    s32b d = damroll(80, 6) + (p.exp / 100) * MON_DRAIN_LIFE;

						//    if (check_state(p, OF_HOLD_LIFE, p.state.flags))
						//    {
						//        msg("You feel your life slipping away!");
						//        player_exp_lose(p, d / 10, false);
						//    }
						//    else
						//    {
						//        msg("You feel your life draining away!");
						//        player_exp_lose(p, d, false);
						//    }
						//}
						//break;
			        } else if (effect.value == RBE.HALLU.value)
			        {
						throw new NotImplementedException();
						//// Take damage 
						//take_hit(p, damage, ddesc);

						//// Increase "image"
						//if (player_inc_timed(p, TMD_IMAGE, 3 + randint1(rlev / 2), true, true))
						//    obvious = true;

						//// Learn about the player
						//monster_learn_resists(m_ptr, p, GF_CHAOS);

						//break;
			        } else {
						throw new NotImplementedException();
					}


			        // Hack -- only one of cut or stun
			        if (do_cut && do_stun)
			        {
			            // Cancel cut
			            if (Random.randint0(100) < 50)
			            {
			                do_cut = false;
			            }

			            // Cancel stun
			            else
			            {
			                do_stun = false;
			            }
			        }

			        // Handle cut
			        if (do_cut)
			        {
						throw new NotImplementedException();
						//int k;

						//// Critical hit (zero if non-critical)
						//tmp = monster_critical(d_dice, d_side, damage);

						//// Roll for damage
						//switch (tmp)
						//{
						//    case 0: k = 0; break;
						//    case 1: k = randint1(5); break;
						//    case 2: k = randint1(5) + 5; break;
						//    case 3: k = randint1(20) + 20; break;
						//    case 4: k = randint1(50) + 50; break;
						//    case 5: k = randint1(100) + 100; break;
						//    case 6: k = 300; break;
						//    default: k = 500; break;
						//}

						//// Apply the cut
						//if (k) (void)player_inc_timed(p, TMD_CUT, k, true, true);
			        }

			        // Handle stun
			        if (do_stun)
			        {
						throw new NotImplementedException();
						//int k;

						//// Critical hit (zero if non-critical)
						//tmp = monster_critical(d_dice, d_side, damage);

						//// Roll for damage
						//switch (tmp)
						//{
						//    case 0: k = 0; break;
						//    case 1: k = randint1(5); break;
						//    case 2: k = randint1(10) + 10; break;
						//    case 3: k = randint1(20) + 20; break;
						//    case 4: k = randint1(30) + 30; break;
						//    case 5: k = randint1(40) + 40; break;
						//    case 6: k = 100; break;
						//    default: k = 200; break;
						//}

						//// Apply the stun
						//if (k) (void)player_inc_timed(p, TMD_STUN, k, true, true);
			        }
			    }

			    // Monster missed player
			    else
			    {
					throw new NotImplementedException();
					//// Analyze failed attacks
					//switch (method)
					//{
					//    case RBM_HIT:
					//    case RBM_TOUCH:
					//    case RBM_PUNCH:
					//    case RBM_KICK:
					//    case RBM_CLAW:
					//    case RBM_BITE:
					//    case RBM_STING:
					//    case RBM_BUTT:
					//    case RBM_CRUSH:
					//    case RBM_ENGULF:

					//    // Visible monsters
					//    if (ml)
					//    {
					//        // Disturbing
					//        disturb(p, 1, 0);

					//        // Message
					//        msg("%^s misses you.", m_name);
					//    }

					//    break;
					//}
			    }


			    // Analyze "visible" monsters only
			    if (visible)
			    {
			        // Count "obvious" attacks (and ones that cause damage)
			        if (obvious || damage != 0 || (l_ptr.blows[ap_cnt] > 10))
			        {
			            // Count attacks of this type
			            if (l_ptr.blows[ap_cnt] < byte.MaxValue)
			            {
			                l_ptr.blows[ap_cnt]++;
			            }
			        }
			    }

			    // Skip the other blows if necessary
			    if (do_break) break;
			}


			// Blink away
			if (blinked)
			{
			    Utilities.msg("There is a puff of smoke!");
				throw new NotImplementedException();
			    //teleport_away(m_ptr, MAX_SIGHT * 2 + 5);
			}


			// Always notice cause of death
			if (p.is_dead && (l_ptr.deaths < short.MaxValue))
			{
			    l_ptr.deaths++;
			}


			// Assume we attacked
			// Nick: Because, based on the length of this function, 
			// literally a million other things could have happened
			return true;
		}

	}
}
