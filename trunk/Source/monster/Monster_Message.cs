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
		static ushort size_mon_hist = 0;
		static ushort size_mon_msg = 0;

		public static Monster_Race_Message[] mon_msg;
		public static Monster_Message_History[] mon_message_hist;

		/*
		 * Stack a codified message for the given monster race. You must supply
		 * the description of some monster of this race. You can also supply
		 * different monster descriptions for the same race.
		 * Return true on success.
		 */
		public static bool add_monster_message(string mon_name, int m_idx, int msg_code, bool delay) {
			int i;
			byte mon_flags = 0;

			Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);
			int r_idx = m_ptr.r_idx;

			if (redundant_monster_message(m_idx, msg_code)) return (false);

			/* Paranoia */
			if (mon_name == null || mon_name.Length == 0) mon_name = "it";

			/* Save the "hidden" mark, if present */
			if (mon_name.Contains("(hidden)")) mon_flags |= 0x01;

			/* Save the "offscreen" mark, if present */
			if (mon_name.Contains("(offscreen)")) mon_flags |= 0x02;

			/* Monster is invisible or out of LOS */
			if ((mon_name == "it") || mon_name == "something")
			   mon_flags |= 0x04;

			/* Query if the message is already stored */
			for (i = 0; i < size_mon_msg; i++)
			{
			    /* We found the race and the message code */
			    if ((mon_msg[i].mon_race == r_idx) &&
			        (mon_msg[i].mon_flags == mon_flags) &&
			        (mon_msg[i].msg_code == msg_code))
			    {
			        /* Can we increment the counter? */
			        if (mon_msg[i].mon_count < byte.MaxValue)
			        {
			            /* Stack the message */
			            ++(mon_msg[i].mon_count);
			        }

			        /* Success */
			        return (true);
			    }
			}

			/* The message isn't stored. Check free space */
			if (size_mon_msg >= Misc.MAX_STORED_MON_MSG) return (false);

			/* Assign the message data to the free slot */
			mon_msg[i] = new Monster_Race_Message();
			mon_msg[i].mon_race = (short)r_idx;
			mon_msg[i].mon_flags = mon_flags;
			mon_msg[i].msg_code = msg_code;
			mon_msg[i].delay = delay;
			/* Just this monster so far */
			mon_msg[i].mon_count = 1;

			/* One more entry */
			++size_mon_msg;

			Misc.p_ptr.notice |= Misc.PN_MON_MESSAGE;

			/* record which monster had this message stored */
			if (size_mon_hist >= Misc.MAX_STORED_MON_CODES) return (true);
			mon_message_hist[size_mon_hist] = new Monster_Message_History();
			mon_message_hist[size_mon_hist].monster_idx = m_idx;
			mon_message_hist[size_mon_hist].message_code = msg_code;
			size_mon_hist++;

			/* Success */
			return (true);
		}

		public static void flush_all_monster_messages()
		{
			/* Flush regular messages, then delayed messages */
			flush_monster_messages(false);
			flush_monster_messages(true);

			/* Delete all the stacked messages and history */
		   size_mon_msg = 0;
		   size_mon_hist = 0;
		}

		/*
		 * Show and delete the stacked monster messages.
		 * Some messages are delayed so that they show up after everything else.
		 * This is to avoid things like "The snaga dies. The snaga runs in fear!"
		 * So we only flush messages matching the delay parameter.
		 */
		static void flush_monster_messages(bool delay)
		{
			throw new NotImplementedException();
		   //int i, r_idx, count;
		   //monster_race *r_ptr;
		   //char buf[512];
		   //char *action;
		   //bool action_only;

		   ///* We use either ascii or system-specific encoding */
		   //int encoding = (OPT(xchars_to_file)) ? SYSTEM_SPECIFIC : ASCII;

		   ///* Show every message */
		   //for (i = 0; i < size_mon_msg; i++)
		   //{
		   //    if (mon_msg[i].delay != delay) continue;
   
		   //    /* Cache the monster count */
		   //    count = mon_msg[i].mon_count;
 
		   //    /* Paranoia */
		   //    if (count < 1) continue;

		   //    /* Start with an empty string */
		   //    buf[0] = '\0';

		   //    /* Cache the race index */
		   //    r_idx = mon_msg[i].mon_race;
           
		   //    /* Get the proper message action */
		   //    action = get_mon_msg_action(mon_msg[i].msg_code, (count > 1),
		   //             &r_info[r_idx]);

		   //    /* Is it a regular race? */
		   //    if (r_idx > 0)
		   //    {
		   //        /* Get the race */
		   //        r_ptr = &r_info[r_idx];
		   //    }
		   //    /* It's the special mark for non-visible monsters */
		   //    else
		   //    {
		   //        /* No race */
		   //        r_ptr = null;
		   //    }

		   //    /* Monster is marked as invisible */
		   //    if(mon_msg[i].mon_flags & 0x04) r_ptr = null;
	   
		   //    /* Special message? */
		   //    action_only = (*action == '~');

		   //    /* Format the proper message for visible monsters */
		   //    if (r_ptr && !action_only)
		   //    {
		   //        char race_name[80];
 
		   //        /* Get the race name */
		   //        my_strcpy(race_name, r_ptr.name, sizeof(race_name));

		   //        /* Uniques */
		   //        if (rf_has(r_ptr.flags, RF_UNIQUE))
		   //        {
		   //            /* Just copy the race name */
		   //            my_strcpy(buf, (r_ptr.name), sizeof(buf));
		   //        }
		   //        /* We have more than one monster */
		   //        else if (count > 1)
		   //        {
		   //            /* Get the plural of the race name */
		   //            plural_aux(race_name, sizeof(race_name));

		   //            /* Put the count and the race name together */
		   //            strnfmt(buf, sizeof(buf), "%d %s", count, race_name);
		   //        }
		   //        /* Normal lonely monsters */
		   //        else
		   //        {
		   //            /* Just add a slight flavor */
		   //            strnfmt(buf, sizeof(buf), "the %s", race_name);
		   //        }

		   //    }
		   //    /* Format the message for non-viewable monsters if necessary */
		   //    else if (!r_ptr && !action_only)
		   //    {
		   //        if (count > 1)
		   //        {
		   //            /* Show the counter */
		   //            strnfmt(buf, sizeof(buf), "%d monsters", count);
		   //        }
		   //        else
		   //        {
		   //            /* Just one non-visible monster */
		   //            my_strcpy(buf, "it", sizeof(buf));
		   //        }
		   //    }

		   //    /* Special message. Nuke the mark */
		   //    if (action_only)
		   //    {   
		   //        ++action;
		   //    }
		   //    /* Regular message */
		   //    else
		   //    {
		   //        /* Add special mark. Monster is offscreen */
		   //        if (mon_msg[i].mon_flags & 0x02) my_strcat(buf, " (offscreen)", sizeof(buf));
        
		   //        /* Add the separator */
		   //        my_strcat(buf, " ", sizeof(buf));
		   //    }

		   //    /* Append the action to the message */
		   //    my_strcat(buf, action, sizeof(buf));
       
		   //    /* Translate to accented characters */
		   //    /* Translate the note to the desired encoding */
		   //    xstr_trans(buf, encoding);

		   //    /* Capitalize the message */
		   //    *buf = my_toupper((unsigned char)*buf);

		   //    /* Hack - play sound for fear message */
		   //    if (mon_msg[i].msg_code == MON_MSG_FLEE_IN_TERROR)
		   //         sound(MSG_FLEE);
	   
		   //    /* Show the message */
		   //    msg(buf);
		   //}
		}


		/*
		 * Tracks which monster has had which pain message stored, so redundant
		 * messages don't happen due to monster attacks hitting other monsters.
		 * Returns true if the message is redundant.
		 */
		static bool redundant_monster_message(int m_idx, int msg_code)
		{
		   int i;

		   /* No messages yet */
		   if (size_mon_hist == 0) return false;

		   for (i = 0; i < size_mon_hist; i++)
		   {
			   /* Not the same monster */
			   if (m_idx != mon_message_hist[i].monster_idx) continue;

			   /* Not the same code */
			   if (msg_code != mon_message_hist[i].message_code) continue;

			   /* We have a match. */
			   return (true);
		   }

		   return (false);
		}

	}
}
