using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Monster {
	/** Structures **/
	/*
	 * A stacked monster message entry
	 */
	class Monster_Race_Message {
		public short mon_race;		/* The race of the monster */
		public byte mon_flags;		/* Flags: 0x01 means hidden monster, 0x02 means offscreen monster */
		public int msg_code;		/* The coded message */
		public byte mon_count;		/* How many monsters triggered this message */
		public bool delay;			/* Should this message be put off to the end */
	}

	class Monster_Message_History {
		public int monster_idx;		/* The monster */
		public int message_code;	/* The coded message */
	}

	/** Constants **/

	/* The codified monster messages */
	public enum MON_MSG {
		NONE = 0,

		/* project_m */
		DIE,
		DESTROYED,
		RESIST_A_LOT,
		HIT_HARD,
		RESIST,
		IMMUNE,
		RESIST_SOMEWHAT,
		UNAFFECTED,
		SPAWN,
		HEALTHIER,
		FALL_ASLEEP,
		WAKES_UP,
		CRINGE_LIGHT,
		SHRIVEL_LIGHT,
		LOSE_SKIN,
		DISSOLVE,
		CATCH_FIRE,
		BADLY_FROZEN,
		SHUDDER,
		CHANGE,
		DISAPPEAR,
		MORE_DAZED,
		DAZED,
		NOT_DAZED,
		MORE_CONFUSED,
		CONFUSED,
		NOT_CONFUSED,
		MORE_SLOWED,
		SLOWED,
		NOT_SLOWED,
		MORE_HASTED,
		HASTED,
		NOT_HASTED,
		MORE_AFRAID,
		FLEE_IN_TERROR,
		NOT_AFRAID,
		MORIA_DEATH,
		DISENTEGRATES,
		FREEZE_SHATTER,
		MANA_DRAIN,
		BRIEF_PUZZLE,
		MAINTAIN_SHAPE,

		/* message_pain */
		UNHARMED,
		MON_MSG_95,
		MON_MSG_75,
		MON_MSG_50,
		MON_MSG_35,
		MON_MSG_20,
		MON_MSG_10,
		MON_MSG_0,

		/* Always leave this at the end */
		MAX
	};

	class Monster_Message {
		public static Monster_Race_Message[] mon_msg;
		public static Monster_Message_History[] mon_message_hist;

		/*
		 * Stack a codified message for the given monster race. You must supply
		 * the description of some monster of this race. You can also supply
		 * different monster descriptions for the same race.
		 * Return true on success.
		 */
		public static bool add_monster_message(string mon_name, int m_idx, int msg_code, bool delay) {
			throw new NotImplementedException();
			//int i;
			//byte mon_flags = 0;

			//monster_type *m_ptr = cave_monster(cave, m_idx);
			//int r_idx = m_ptr.r_idx;

			//if (redundant_monster_message(m_idx, msg_code)) return (false);

			///* Paranoia */
			//if (!mon_name || !mon_name[0]) mon_name = "it";

			///* Save the "hidden" mark, if present */
			//if (strstr(mon_name, "(hidden)")) mon_flags |= 0x01;

			///* Save the "offscreen" mark, if present */
			//if (strstr(mon_name, "(offscreen)")) mon_flags |= 0x02;

			///* Monster is invisible or out of LOS */
			//if (streq(mon_name, "it") || streq(mon_name, "something"))
			//   mon_flags |= 0x04;

			///* Query if the message is already stored */
			//for (i = 0; i < size_mon_msg; i++)
			//{
			//    /* We found the race and the message code */
			//    if ((mon_msg[i].mon_race == r_idx) &&
			//        (mon_msg[i].mon_flags == mon_flags) &&
			//        (mon_msg[i].msg_code == msg_code))
			//    {
			//        /* Can we increment the counter? */
			//        if (mon_msg[i].mon_count < MAX_UCHAR)
			//        {
			//            /* Stack the message */
			//            ++(mon_msg[i].mon_count);
			//        }

			//        /* Success */
			//        return (true);
			//    }
			//}

			///* The message isn't stored. Check free space */
			//if (size_mon_msg >= MAX_STORED_MON_MSG) return (false);

			///* Assign the message data to the free slot */
			//mon_msg[i].mon_race = r_idx;
			//mon_msg[i].mon_flags = mon_flags;
			//mon_msg[i].msg_code = msg_code;
			//mon_msg[i].delay = delay;
			///* Just this monster so far */
			//mon_msg[i].mon_count = 1;

			///* One more entry */
			//++size_mon_msg;

			//p_ptr.notice |= PN_MON_MESSAGE;

			///* record which monster had this message stored */
			//if (size_mon_hist >= MAX_STORED_MON_CODES) return (true);
			//mon_message_hist[size_mon_hist].monster_idx = m_idx;
			//mon_message_hist[size_mon_hist].message_code = msg_code;
			//size_mon_hist++;

			///* Success */
			//return (true);
		}
	}
}
