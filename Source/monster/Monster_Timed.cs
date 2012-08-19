﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Monster {
	partial class Monster {

		class mon_timed_effect{
			public mon_timed_effect(int a, int b, int c, uint d, int e){
				message_begin = a;
				message_end = b;
				message_increase = c;
				flag_resist = d;
				max_timer = e;
			}

			public int message_begin;
			public int message_end;
			public int message_increase;
			public uint flag_resist;
			public int max_timer;
		}

		/*
		 * Monster timed effects.
		 * '0' means no message.
		 */
		static mon_timed_effect[] effects = new mon_timed_effect[]
		{
			new mon_timed_effect( (int)MON_MSG.FALL_ASLEEP, (int)MON_MSG.WAKES_UP, 
				0, (uint)Monster_Flag.NO_SLEEP.value, 10000 ),
			new mon_timed_effect( (int)MON_MSG.DAZED,		(int)MON_MSG.NOT_DAZED, 
				(int)MON_MSG.MORE_DAZED, (uint)Monster_Flag.NO_STUN.value, 200 ),
			new mon_timed_effect( (int)MON_MSG.CONFUSED,	(int)MON_MSG.NOT_CONFUSED, 
				(int)MON_MSG.MORE_CONFUSED, (uint)Monster_Flag.NO_CONF.value, 200 ),
			new mon_timed_effect( (int)MON_MSG.FLEE_IN_TERROR, (int)MON_MSG.NOT_AFRAID, 
				(int)MON_MSG.MORE_AFRAID, (uint)Monster_Flag.NO_FEAR.value, 10000 ),
			new mon_timed_effect( (int)MON_MSG.SLOWED,		(int)MON_MSG.NOT_SLOWED, 
				(int)MON_MSG.MORE_SLOWED, (uint)0L, 50 ),
			new mon_timed_effect( (int)MON_MSG.HASTED,		(int)MON_MSG.NOT_HASTED, 
				(int)MON_MSG.MORE_HASTED, (uint)0L, 50 ),
		};


		/**
		 * Decreases the timed effect `ef_idx` by `timer`.
		 *
		 * Calculates the new timer, then passes that to mon_set_timed().
		 * If a timer would be set to a negative number, it is set to 0 instead.
		 * Note that decreasing a timed effect should never fail.
		 *
		 * Returns true if the monster's timer changed.
		 */
		static bool mon_dec_timed(int m_idx, int ef_idx, int timer, ushort flag, bool id)
		{
			Monster m_ptr;

			Misc.assert(ef_idx >= 0 && ef_idx < (int)Misc.MON_TMD.MAX);

			Misc.assert(m_idx > 0);
			m_ptr = Cave.cave_monster(Cave.cave, m_idx);

			Misc.assert(timer > 0);

			/* Decreasing is never resisted */
			flag |= Misc.MON_TMD_FLG_NOFAIL;

			/* New counter amount */
			timer = m_ptr.m_timed[ef_idx] - timer;
			if (timer < 0)
			    timer = 0;

			return mon_set_timed(m_ptr, ef_idx, timer, flag, id);
		}

		/**
		 * Attempts to set the timer of the given monster effect to `timer`.
		 *
		 * Checks to see if the monster resists the effect, using mon_resist_effect().
		 * If not, the effect is set to `timer` turns. If `timer` is 0, or if the
		 * effect timer was 0, or if MON_TMD_FLG_NOTIFY is set in `flag`, then a
		 * message is printed, unless MON_TMD_FLG_NOMESSAGE is set in `flag`.
		 *
		 * Set a timed monster event to 'v'.  Give messages if the right flags are set.
		 * Check if the monster is able to resist the spell.  Mark the lore.
		 * Returns true if the monster was affected.
		 * Return false if the monster was unaffected.
		 */
		static bool mon_set_timed(Monster m_ptr, int ef_idx, int timer, ushort flag, bool id)
		{
			mon_timed_effect effect;

			int m_note = 0;
			int resisted;
			int old_timer;

			Misc.assert(ef_idx >= 0 && ef_idx < (int)Misc.MON_TMD.MAX);
			effect = effects[ef_idx];

			Misc.assert(m_ptr != null);
			old_timer = m_ptr.m_timed[ef_idx];

			/* Ignore dead monsters */
			if (m_ptr.r_idx == 0) return false;

			/* No change */
			if (old_timer == timer) return false;

			if (timer == 0) {
				/* Turning off, usually mention */
				m_note = effect.message_end;
				flag |= Misc.MON_TMD_FLG_NOTIFY;
			} else if (old_timer == 0) {
				/* Turning on, usually mention */
				flag |= Misc.MON_TMD_FLG_NOTIFY;
				m_note = effect.message_begin;
			} else if (timer > old_timer) {
				/* Different message for increases, but don't automatically mention. */
				m_note = effect.message_increase;
			}

			/* Determine if the monster resisted or not */
			resisted = mon_resist_effect(m_ptr, ef_idx, timer, flag)?1:0;

			if (resisted != 0)
				m_note = (int)MON_MSG.UNAFFECTED;
			else
				m_ptr.m_timed[ef_idx] = (short)timer;

			if (Misc.p_ptr.health_who == m_ptr.midx) Misc.p_ptr.redraw |= (Misc.PR_HEALTH);

			/* Update the visuals, as appropriate. */
			Misc.p_ptr.redraw |= (Misc.PR_MONLIST);

			/* Print a message if there is one, if the effect allows for it, and if
			 * either the monster is visible, or we're trying to ID something */
			if (m_note != 0 && (m_ptr.ml || id) && (flag & Misc.MON_TMD_FLG_NOMESSAGE) == 0 && (flag & Misc.MON_TMD_FLG_NOTIFY) != 0) {
				//char m_name[80];
				string m_name;

				m_name = m_ptr.monster_desc((Desc)0x04);
				Monster_Message.add_monster_message(m_name, m_ptr.midx, m_note, true);
			}

			return resisted == 0;
		}

		/**
		 * Determines whether the given monster successfully resists the given effect.
		 *
		 * If MON_TMD_FLG_NOFAIL is set in `flag`, this returns false.
		 * Then we determine if the monster resists the effect for some racial
		 * reason. For example, the monster might have the NO_SLEEP flag, in which
		 * case it always resists sleep. Or if it breathes chaos, it always resists
		 * confusion. If the given monster doesn't resist for any of these reasons,
		 * then it makes a saving throw. If MON_TMD_MON_SOURCE is set in `flag`,
		 * indicating that another monster caused this effect, then the chance of
		 * success on the saving throw just depends on the monster's native depth.
		 * Otherwise, the chance of success decreases as `timer` increases.
		 *
		 * Also marks the lore for any appropriate resists.
		 */
		static bool mon_resist_effect(Monster m_ptr, int ef_idx, int timer, ushort flag)
		{
			mon_timed_effect effect;
			int resist_chance;
			Monster_Race r_ptr;
			Monster_Lore l_ptr;

			Misc.assert(ef_idx >= 0 && ef_idx < (int)Misc.MON_TMD.MAX);
			effect = effects[ef_idx];

			Misc.assert(m_ptr != null);
			r_ptr = Misc.r_info[m_ptr.r_idx];
			l_ptr = Misc.l_list[m_ptr.r_idx];
	
			/* Hasting never fails */
			if (ef_idx == (int)Misc.MON_TMD.FAST) return (false);
	
			/* Some effects are marked to never fail */
			if ((flag & Misc.MON_TMD_FLG_NOFAIL) != 0) return (false);

			/* A sleeping monster resists further sleeping */
			if (ef_idx == (int)Misc.MON_TMD.SLEEP && m_ptr.m_timed[ef_idx] != 0) return (true);

			/* If the monster resists innately, learn about it */
			if (r_ptr.flags.has((int)effect.flag_resist)) {
			    if (m_ptr.ml)
			        l_ptr.flags.on((int)effect.flag_resist);

			    return (true);
			}

			/* Monsters with specific breaths resist stunning*/
			if (ef_idx == (int)Misc.MON_TMD.STUN && (r_ptr.spell_flags.has(Monster_Spell_Flag.BR_SOUN.value) ||
			        r_ptr.spell_flags.has(Monster_Spell_Flag.BR_WALL.value)))
			{
			    /* Add the lore */
			    if (m_ptr.ml)
			    {
			        if (r_ptr.spell_flags.has(Monster_Spell_Flag.BR_SOUN.value))
			            l_ptr.spell_flags.on(Monster_Spell_Flag.BR_SOUN.value);
			        if (r_ptr.spell_flags.has(Monster_Spell_Flag.BR_WALL.value))
			            l_ptr.spell_flags.on(Monster_Spell_Flag.BR_WALL.value);
			    }

			    return (true);
			}

			/* Monsters with specific breaths resist confusion */
			if ((ef_idx == (int)Misc.MON_TMD.CONF) &&
			    ((r_ptr.spell_flags.has(Monster_Spell_Flag.BR_CHAO.value)) ||
			     (r_ptr.spell_flags.has(Monster_Spell_Flag.BR_CONF.value))) )
			{
			    /* Add the lore */
			    if (m_ptr.ml)
			    {
			        if (r_ptr.spell_flags.has(Monster_Spell_Flag.BR_CHAO.value))
			            l_ptr.spell_flags.on(Monster_Spell_Flag.BR_CHAO.value);
			        if (r_ptr.spell_flags.has(Monster_Spell_Flag.BR_CONF.value))
			            l_ptr.spell_flags.on(Monster_Spell_Flag.BR_CONF.value);
			    }

			    return (true);
			}

			/* Inertia breathers resist slowing */
			if (ef_idx == (int)Misc.MON_TMD.SLOW && r_ptr.spell_flags.has(Monster_Spell_Flag.BR_INER.value))
			{
			    l_ptr.spell_flags.on(Monster_Spell_Flag.BR_INER.value);
			    return (true);
			}

			/* Calculate the chance of the monster making its saving throw. */
			if (ef_idx == (int)Misc.MON_TMD.SLEEP)
			    timer /= 25; /* Hack - sleep uses much bigger numbers */

			if ((flag & (int)Misc.MON_TMD_MON_SOURCE) != 0)
			    resist_chance = r_ptr.level;
			else
			    resist_chance = r_ptr.level + 40 - (timer / 2);

			if (Random.randint0(100) < resist_chance) return (true);

			/* Uniques are doubly hard to affect */
			if (r_ptr.flags.has(Monster_Flag.UNIQUE.value))
			    if (Random.randint0(100) < resist_chance) return (true);

			return (false);
		}

		/**
		 * Clears the timed effect `ef_idx`.
		 *
		 * Returns true if the monster's timer was changed.
		 */
		static bool mon_clear_timed(int m_idx, int ef_idx, ushort flag, bool id)
		{
			Monster m_ptr;

			Misc.assert(ef_idx >= 0 && ef_idx < (int)Misc.MON_TMD.MAX);

			Misc.assert(m_idx > 0);
			m_ptr = Cave.cave_monster(Cave.cave, m_idx);

			if (m_ptr.m_timed[ef_idx] == 0) return false;

			/* Clearing never fails */
			flag |= Misc.MON_TMD_FLG_NOFAIL;

			return mon_set_timed(m_ptr, ef_idx, 0, flag, id);
		}
	}
}
