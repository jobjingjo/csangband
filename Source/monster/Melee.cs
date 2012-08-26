using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;

namespace CSAngband.Monster {
	partial class Monster {
		/*
		 * Determine if a bolt will arrive, checking that no monsters are in the way
		 */
		public static bool clean_shot(int Y1, int X1, int Y2, int X2){
			return Cave.projectable(Y1, X1, Y2, X2, Spell.PROJECT_STOP);
		}

		/*
		 * Remove the "bad" spells from a spell list
		 */
		static void remove_bad_spells(int m_idx, Bitflag f)
		{
			Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);
			Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			Bitflag f2 = new Bitflag(Monster_Spell_Flag.SIZE);
			Bitflag ai_flags = new Bitflag(Object_Flag.SIZE);

			int i;	
			uint smart = 0;

			/* Stupid monsters act randomly */
			if (r_ptr.flags.has(Monster_Flag.STUPID.value)) return;

			/* Take working copy of spell flags */
			f2.copy(f);

			/* Don't heal if full */
			if (m_ptr.hp >= m_ptr.maxhp) f2.off(Monster_Spell_Flag.HEAL.value);
	
			/* Don't haste if hasted with time remaining */
			if (m_ptr.m_timed[(int)Misc.MON_TMD.FAST] > 10) f2.off(Monster_Spell_Flag.HASTE.value);

			/* Don't teleport to if the player is already next to us */
			if (m_ptr.cdis == 1) f2.off(Monster_Spell_Flag.TELE_TO.value);

			/* Update acquired knowledge */
			ai_flags.wipe();
			if (Option.birth_ai_learn.value)
			{
			    /* Occasionally forget player status */
			    if (Random.one_in_(100))
			        m_ptr.known_pflags.wipe();

			    /* Use the memorized flags */
			    smart = m_ptr.smart;
			    ai_flags.copy(m_ptr.known_pflags);
			}

			/* Cheat if requested */
			if (Option.birth_ai_cheat.value) {
			    for (i = 0; i < Object_Flag.MAX.value; i++)
			        if (Misc.p_ptr.check_state(Object_Flag.list[i], Misc.p_ptr.state.flags))
			            ai_flags.on(i);
			    if (Misc.p_ptr.msp == 0) smart |= Misc.SM_IMM_MANA;
			}

			/* Cancel out certain flags based on knowledge */
			if(!ai_flags.is_empty()) {
				throw new NotImplementedException();
				//unset_spells(f2, ai_flags, r_ptr);
			}

			if ((smart & Misc.SM_IMM_MANA) != 0 && Random.randint0(100) < 50 * (r_ptr.flags.has(Monster_Flag.SMART.value) ? 2 : 1))
			    f2.off(Monster_Spell_Flag.DRAIN_MANA.value);

			/* use working copy of spell flags */
			f.copy(f2);
		}


		/*
		 * Determine if there is a space near the selected spot in which
		 * a summoned creature can appear
		 */
		static bool summon_possible(int y1, int x1)
		{
			int y, x;

			/* Start at the location, and check 2 grids in each dir */
			for (y = y1 - 2; y <= y1 + 2; y++)
			{
				for (x = x1 - 2; x <= x1 + 2; x++)
				{
					/* Ignore illegal locations */
					if (!Cave.in_bounds(y, x)) continue;

					/* Only check a circular area */
					if (Cave.distance(y1, x1, y, x) > 2) continue;

					/* Hack: no summon on glyph of warding */
					if (Cave.cave.feat[y][x] == Cave.FEAT_GLYPH) continue;

					/* Require empty floor grid in line of sight */
					if (Cave.cave_empty_bold(y, x) && Cave.los(y1, x1, y, x))
					{
						return (true);
					}
				}
			}

			return false;
		}

		/*
		 * Have a monster choose a spell to cast.
		 *
		 * Note that the monster's spell list has already had "useless" spells
		 * (bolts that won't hit the player, summons without room, etc.) removed.
		 * Perhaps that should be done by this function.
		 *
		 * Stupid monsters will just pick a spell randomly.  Smart monsters
		 * will choose more "intelligently".
		 *
		 * This function could be an efficiency bottleneck.
		 */
		static int choose_attack_spell(int m_idx, Bitflag f)
		{
			throw new NotImplementedException();
			//monster_type *m_ptr = cave_monster(cave, m_idx);
			//monster_race *r_ptr = &r_info[m_ptr.r_idx];

			//int num = 0;
			//byte spells[RSF_MAX];

			//int i, py = p_ptr.py, px = p_ptr.px;

			//bool has_escape, has_attack, has_summon, has_tactic;
			//bool has_annoy, has_haste, has_heal;


			///* Smart monsters restrict their spell choices. */
			//if (OPT(birth_ai_smart) && !rf_has(r_ptr.flags, RF_STUPID))
			//{
			//    /* What have we got? */
			//    has_escape = test_spells(f, RST_ESCAPE);
			//    has_attack = test_spells(f, RST_ATTACK | RST_BOLT | RST_BALL | RST_BREATH);
			//    has_summon = test_spells(f, RST_SUMMON);
			//    has_tactic = test_spells(f, RST_TACTIC);
			//    has_annoy = test_spells(f, RST_ANNOY);
			//    has_haste = test_spells(f, RST_HASTE);
			//    has_heal = test_spells(f, RST_HEAL);

			//    /*** Try to pick an appropriate spell type ***/

			//    /* Hurt badly or afraid, attempt to flee */
			//    if (has_escape && ((m_ptr.hp < m_ptr.maxhp / 4) || m_ptr.m_timed[MON_TMD_FEAR]))
			//    {
			//        /* Choose escape spell */
			//        set_spells(f, RST_ESCAPE);
			//    }

			//    /* Still hurt badly, couldn't flee, attempt to heal */
			//    else if (has_heal && m_ptr.hp < m_ptr.maxhp / 4)
			//    {
			//        /* Choose heal spell */
			//        set_spells(f, RST_HEAL);
			//    }

			//    /* Player is close and we have attack spells, blink away */
			//    else if (has_tactic && (distance(py, px, m_ptr.fy, m_ptr.fx) < 4) &&
			//             has_attack && (randint0(100) < 75))
			//    {
			//        /* Choose tactical spell */
			//        set_spells(f, RST_TACTIC);
			//    }

			//    /* We're hurt (not badly), try to heal */
			//    else if (has_heal && (m_ptr.hp < m_ptr.maxhp * 3 / 4) &&
			//             (randint0(100) < 60))
			//    {
			//        /* Choose heal spell */
			//        set_spells(f, RST_HEAL);
			//    }

			//    /* Summon if possible (sometimes) */
			//    else if (has_summon && (randint0(100) < 50))
			//    {
			//        /* Choose summon spell */
			//        set_spells(f, RST_SUMMON);
			//    }

			//    /* Attack spell (most of the time) */
			//    else if (has_attack && (randint0(100) < 85))
			//    {
			//        /* Choose attack spell */
			//        set_spells(f, RST_ATTACK | RST_BOLT | RST_BALL | RST_BREATH);
			//    }

			//    /* Try another tactical spell (sometimes) */
			//    else if (has_tactic && (randint0(100) < 50))
			//    {
			//        /* Choose tactic spell */
			//        set_spells(f, RST_TACTIC);
			//    }

			//    /* Haste self if we aren't already somewhat hasted (rarely) */
			//    else if (has_haste && (randint0(100) < (20 + r_ptr.speed - m_ptr.mspeed)))
			//    {
			//        /* Choose haste spell */
			//        set_spells(f, RST_HASTE);
			//    }

			//    /* Annoy player (most of the time) */
			//    else if (has_annoy && (randint0(100) < 85))
			//    {
			//        /* Choose annoyance spell */
			//        set_spells(f, RST_ANNOY);
			//    }

			//    /* Else choose no spell */
			//    else
			//    {
			//        rsf_wipe(f);
			//    }

			//    /* Anything left? */
			//    if (rsf_is_empty(f)) return (FLAG_END);
			//}

			///* Extract all spells: "innate", "normal", "bizarre" */
			//for (i = FLAG_START, num = 0; i < RSF_MAX; i++)
			//{
			//    if (rsf_has(f, i)) spells[num++] = i;
			//}

			///* Paranoia */
			//if (num == 0) return 0;

			///* Pick at random */
			//return (spells[randint0(num)]);
		}
	}
}
