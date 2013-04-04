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
		public static Info[] history_list;

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

		void LIMITLOW(ref int a, int b) {
			if (a < b) a = b;
		}

		void LIMITHI(ref int a, int b) {
			if (a > b) a = b;
		}


		/*
		 * Return the number of history entries.
		 */
		public static int history_get_num(){
			return history_ctr;
		}


		/*
		 * Mark artifact number `id` as known.
		 */
		public static bool history_know_artifact(Artifact artifact)
		{
			throw new NotImplementedException();
			//size_t i = history_ctr;
			//assert(artifact);

			//while (i--) {
			//    if (history_list[i].a_idx == artifact.aidx) {
			//        history_list[i].type = HISTORY_ARTIFACT_KNOWN;
			//        return true;
			//    }
			//}

			//return false;
		}


		/*
		 * Returns true if the artifact denoted by a_idx is KNOWN in the history log.
		 */
		public static bool history_is_artifact_known(Artifact artifact)
		{
			throw new NotImplementedException();
			//size_t i = history_ctr;
			//assert(artifact);

			//while (i--) {
			//    if (history_list[i].type & HISTORY_ARTIFACT_KNOWN &&
			//            history_list[i].a_idx == artifact.aidx)
			//        return true;
			//}

			//return false;
		}


		/*
		 * Returns true if the artifact denoted by a_idx is an active entry in
		 * the history log (i.e. is not marked HISTORY_ARTIFACT_LOST).  This permits
		 * proper handling of the case where the player loses an artifact but (in
		 * preserve mode) finds it again later.
		 */
		public static bool history_is_artifact_logged(Artifact artifact)
		{
			throw new NotImplementedException();
			//size_t i = history_ctr;
			//assert(artifact);

			//while (i--) {
			//    /* Don't count ARTIFACT_LOST entries; then we can handle
			//     * re-finding previously lost artifacts in preserve mode  */
			//    if (history_list[i].type & HISTORY_ARTIFACT_LOST)
			//        continue;

			//    if (history_list[i].a_idx == artifact.aidx)
			//        return true;
			//}

			//return false;
		}


		/*
		 * Convert all ARTIFACT_UNKNOWN history items to HISTORY_ARTIFACT_KNOWN.
		 * Use only after player retirement/death for the final character dump.
		 */
		public static void history_unmask_unknown()
		{
			throw new NotImplementedException();
			//size_t i = history_ctr;

			//while (i--)
			//{
			//    if (history_list[i].type & HISTORY_ARTIFACT_UNKNOWN)
			//    {
			//        history_list[i].type &= ~(HISTORY_ARTIFACT_UNKNOWN);
			//        history_list[i].type |= HISTORY_ARTIFACT_KNOWN;
			//    }
			//}
		}


		/*
		 * Used to determine whether the history entry is visible in the listing or not.
		 * Returns true if the item is masked -- that is, if it is invisible
		 *
		 * All artifacts are now sensed on pickup, so nothing is now invisible. The
		 * KNOWN / UNKNOWN distinction is if we had fully identified it or not
		 */
		public static bool history_masked(int i)
		{
			return false;
		}

		/*
		 * Finds the index of the last printable (non-masked) item in the history list.
		 */
		public static int last_printable_item()
		{
			throw new NotImplementedException();
			//size_t i = history_ctr;

			//while (i--)
			//{
			//    if (!history_masked(i))
			//        break;
			//}

			//return i;
		}

		public static void print_history_header()
		{
			throw new NotImplementedException();
			//char buf[80];

			///* Print the header (character name and title) */
			//strnfmt(buf, sizeof(buf), "%s the %s %s",
			//        op_ptr.full_name,
			//        p_ptr.race.name,
			//        p_ptr.class.name);

			//c_put_str(TERM_WHITE, buf, 0, 0);
			//c_put_str(TERM_WHITE, "============================================================", 1, 0);
			//c_put_str(TERM_WHITE, "                   CHAR.  ", 2, 0);
			//c_put_str(TERM_WHITE, "|   TURN  | DEPTH |LEVEL| EVENT", 3, 0);
			//c_put_str(TERM_WHITE, "============================================================", 4, 0);
		}


		/* Handles all of the display functionality for the history list. */
		public static void history_display()
		{
			throw new NotImplementedException();
			//int row, wid, hgt, page_size;
			//char buf[90];
			//static size_t first_item = 0;
			//size_t max_item = last_printable_item();
			//size_t i;

			//Term_get_size(&wid, &hgt);

			///* Six lines provide space for the header and footer */
			//page_size = hgt - 6;

			//screen_save();

			//while (1)
			//{
			//    struct keypress ch;

			//    Term_clear();

			//    /* Print everything to screen */
			//    print_history_header();

			//    row = 0;
			//    for (i = first_item; row <= page_size && i < history_ctr; i++)
			//    {
			//        /* Skip messages about artifacts not yet IDed. */
			//        if (history_masked(i))
			//            continue;

			//        strnfmt(buf, sizeof(buf), "%10d%7d\'%5d   %s",
			//            history_list[i].turn,
			//            history_list[i].dlev * 50,
			//            history_list[i].clev,
			//            history_list[i].event);

			//        if (history_list[i].type & HISTORY_ARTIFACT_LOST)
			//            my_strcat(buf, " (LOST)", sizeof(buf));

			//        /* Size of header = 5 lines */
			//        prt(buf, row + 5, 0);
			//        row++;
			//    }
			//    prt("[Arrow keys scroll, p for previous page, n for next page, ESC to exit.]", hgt - 1, 0);

			//    ch = inkey();

			//    /* XXXmacro we should have a generic "key . scroll" function */
			//    if (ch.code == 'n')
			//    {
			//        size_t scroll_to = first_item + page_size;

			//        while (history_masked(scroll_to) && scroll_to < history_ctr - 1)
			//            scroll_to++;

			//        first_item = (scroll_to < max_item ? scroll_to : max_item);
			//    }
			//    else if (ch.code == 'p')
			//    {
			//        int scroll_to = first_item - page_size;

			//        while (history_masked(scroll_to) && scroll_to > 0)
			//            scroll_to--;

			//        first_item = (scroll_to >= 0 ? scroll_to : 0);
			//    }
			//    else if (ch.code == ARROW_DOWN)
			//    {
			//        size_t scroll_to = first_item + 1;

			//        while (history_masked(scroll_to) && scroll_to < history_ctr - 1)
			//            scroll_to++;

			//        first_item = (scroll_to < max_item ? scroll_to : max_item);
			//    }
			//    else if (ch.code == ARROW_UP)
			//    {
			//        int scroll_to = first_item - 1;

			//        while (history_masked(scroll_to) && scroll_to > 0)
			//            scroll_to--;

			//        first_item = (scroll_to >= 0 ? scroll_to : 0);
			//    }
			//    else if (ch.code == ESCAPE)
			//        break;
			//}

			//screen_load();

			//return;
		}


		/* Dump character history to a file, which we assume is already open. */
		//void dump_history(ang_file *file)
		//{
		//    throw new NotImplementedException();
		//    //size_t i;
		//    //char buf[90];

		//    ///* We use either ascii or system-specific encoding */
		//    //int encoding = OPT(xchars_to_file) ? SYSTEM_SPECIFIC : ASCII;

		//    //    file_putf(file, "============================================================\n");
		//    //    file_putf(file, "                   CHAR.\n");
		//    //    file_putf(file, "|   TURN  | DEPTH |LEVEL| EVENT\n");
		//    //    file_putf(file, "============================================================\n");

		//    //for (i = 0; i < (last_printable_item() + 1); i++)
		//    //{
		//    //    /* Skip not-yet-IDd artifacts */
		//    //    if (history_masked(i)) continue;

		//    //            strnfmt(buf, sizeof(buf), "%10d%7d\'%5d   %s",
		//    //                            history_list[i].turn,
		//    //                            history_list[i].dlev * 50,
		//    //                            history_list[i].clev,
		//    //                            history_list[i].event);

		//    //            if (history_list[i].type & HISTORY_ARTIFACT_LOST)
		//    //                            my_strcat(buf, " (LOST)", sizeof(buf));

		//    //    x_file_putf(file, encoding, "%s", buf);
		//    //    file_put(file, "\n");
		//    //}

		//    //return;
		//}

	}
}
