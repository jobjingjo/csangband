using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;

namespace CSAngband {
	class History {
		/* History message types */
		public const int PLAYER_BIRTH     = 0x0001;	/* Player was born */
		public const int ARTIFACT_UNKNOWN = 0x0002;	/* Player found but not IDd an artifact */
		public const int ARTIFACT_KNOWN   = 0x0004;	/* Player has IDed an artifact */
		public const int ARTIFACT_LOST    = 0x0008;	/* Player had an artifact and lost it */
		public const int PLAYER_DEATH     = 0x0010;	/* Player has been slain */
		public const int SLAY_UNIQUE      = 0x0020;	/* Player has slain a unique monster */
		public const int USER_INPUT       = 0x0040;	/* User-added note */
		public const int SAVEFILE_IMPORT  = 0x0080;	/* Added when an older version savefile is imported */
		public const int GAIN_LEVEL       = 0x0100;	/* Player gained a level */
		public const int GENERIC          = 0x0200;	/* Anything else not covered here (unused) */

		public class Info
		{
			public ushort type;			/* Kind of history item */
			public short dlev;			/* Dungeon level when this item was recorded */
			public short clev;			/* Character level when this item was recorded */
			public byte a_idx;			/* Artifact this item relates to */
			public int turn;			/* Turn this item was recorded on */
			public string evt;	/* The text of the item */ //size=80
		};

		/*
		 * Number of slots available at birth in the player history list.  Defaults to
		 * 10 and will expand automatically as new history entries are added, up the
		 * the maximum defined value.
		 */
		const int HISTORY_BIRTH_SIZE = 10;
		const int HISTORY_MAX = 5000;



		/* The historical list for the character */
		static Info[] history_list;

		/* Index of first writable entry */
		static int history_ctr;

		/* Current size of history list */
		static int history_size;

		/*
		 * Initialise an empty history list.
		 */
		static void init(int entries)
		{
			history_ctr = 0;
			history_size = entries;
			history_list = new Info[history_size];
		}

		/*
		 * Clear any existing history.
		 */
		public static void clear()
		{
			if (history_list == null) return;

			history_list = null;
			history_ctr = 0;
			history_size = 0;
		}

		/*
		 * Add an entry with text `event` to the history list, with type `type`
		 * ("HISTORY_xxx" in defines.h), and artifact number `id` (0 for everything
		 * else).
		 *
		 * Return true on success.
		 */
		public static bool add_full(ushort type, Artifact artifact, short dlev, short clev, int turn, string text)
		{
			/* Allocate the history list if needed */
			if (history_list == null)
			    init(HISTORY_BIRTH_SIZE);

			/* Expand the history list if appropriate */
			else if ((history_ctr == history_size) && !set_num(history_size + 10))
			    return false;

			/* History list exists and is not full.  Add an entry at the current counter location. */
			history_list[history_ctr] = new Info();
			history_list[history_ctr].type = type;
			history_list[history_ctr].dlev = dlev;
			history_list[history_ctr].clev = clev;
			history_list[history_ctr].a_idx = (byte)(artifact != null ? artifact.aidx : 0);
			history_list[history_ctr].turn = turn;
			history_list[history_ctr].evt = text;

			history_ctr++;

			return true;
		}

		/*
		 * Set the number of history items.
		 */
		static bool set_num(int num)
		{
			throw new NotImplementedException();
			//history_info *new_list;

			//if (num > HISTORY_MAX)
			//    num = HISTORY_MAX;

			//if (num < history_size)  return false;
			//if (num == history_size) return false;

			///* Allocate new memory, copy across */
			///* XXX Should use mem_realloc() */
			//new_list = C_ZNEW(num, history_info);
			//C_COPY(new_list, history_list, history_ctr, history_info);
			//FREE(history_list);

			//history_list = new_list;
			//history_size = num;

			//return true;
		}


		/*
		 * Add an entry with text `event` to the history list, with type `type`
		 * ("HISTORY_xxx" in defines.h), and artifact number `id` (0 for everything
		 * else).
		 *
		 * Returne true on success.
		 */
		public static bool add(string evt, ushort type, Artifact artifact)
		{
			Player.Player p_ptr = Player.Player.instance;
			return add_full(type, artifact, p_ptr.depth, p_ptr.lev, Misc.turn, evt);
		}

		/*
		 * Adding artifacts to the history list is trickier than other operations.
		 * This is a wrapper function that gets some of the logic out of places
		 * where it really doesn't belong.  Call this to add an artifact to the history
		 * list or make the history entry visible--history_add_artifact will make that
		 * determination depending on what object_is_known returns for the artifact.
		 */
		public static bool add_artifact(Artifact artifact, bool known, bool found)
		{
			throw new NotImplementedException();
			//object_type object_type_body;
			//object_type *o_ptr = &object_type_body;

			//char o_name[80];
			//char buf[80];
			//u16b type;

			//assert(artifact);

			///* Make fake artifact for description purposes */
			//object_wipe(o_ptr);
			//make_fake_artifact(o_ptr, artifact);
			//object_desc(o_name, sizeof(o_name), o_ptr,
			//            ODESC_PREFIX | ODESC_BASE | ODESC_SPOIL);
			//strnfmt(buf, sizeof(buf), (found)?"Found %s":"Missed %s", o_name);

			///* Known objects gets different treatment */
			//if (known) {
			//    /* Try revealing any existing artifact, otherwise log it */
			//    if (history_is_artifact_logged(artifact))
			//        history_know_artifact(artifact);
			//    else
			//        history_add(buf, HISTORY_ARTIFACT_KNOWN, artifact);
			//} else {
			//    if (!history_is_artifact_logged(artifact)) {
			//        type = HISTORY_ARTIFACT_UNKNOWN |
			//                (found ? 0 : HISTORY_ARTIFACT_LOST);
			//        history_add(buf, type, artifact);
			//    } else {
			//        return false;
			//    }
			//}

			//return true;
		}

		/*
		 * Mark artifact number `id` as lost forever, either due to leaving it on a
		 * level, or due to a store purging its inventory after the player sold it.
		 */
		public static bool lose_artifact(Artifact artifact)
		{
			throw new NotImplementedException();
			//size_t i = history_ctr;
			//assert(artifact);

			//while (i--) {
			//    if (history_list[i].a_idx == artifact.aidx) {
			//        history_list[i].type |= HISTORY_ARTIFACT_LOST;
			//        return true;
			//    }
			//}

			///* If we lost an artifact that didn't previously have a history, then we missed it */
			//history_add_artifact(artifact, false, false);

			//return false;
		}

	}
}
