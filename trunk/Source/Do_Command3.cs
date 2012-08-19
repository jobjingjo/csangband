using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	partial class Do_Command {
		/*
		 * Display inventory
		 */
		public static void inven()
		{
			throw new NotImplementedException();
			/*ui_event e;
			int diff = weight_remaining();

			// Hack -- Start in "inventory" mode
			p_ptr.command_wrk = (USE_INVEN);

			// Save screen
			screen_save();

			// Hack -- show empty slots
			item_tester_full = true;

			// Display the inventory
			show_inven(OLIST_WEIGHT | OLIST_QUIVER);

			// Hack -- hide empty slots
			item_tester_full = false;

			// Prompt for a command
			prt(format("(Inventory) Burden %d.%d lb (%d.%d lb %s). Command: ",
						p_ptr.total_weight / 10, p_ptr.total_weight % 10,
						abs(diff) / 10, abs(diff) % 10,
						(diff < 0 ? "overweight" : "remaining")),
				0, 0);

			// Get a new command
			e = inkey_ex();
			if (!(e.type == EVT_KBRD && e.key.code == ESCAPE))
				Term_event_push(&e);

			// Load screen
			screen_load();*/
		}


		/*
		 * Display equipment
		 */
		public static void equip()
		{
			throw new NotImplementedException();
			//ui_event e;

			///* Hack -- Start in "equipment" mode */
			//p_ptr.command_wrk = (USE_EQUIP);

			///* Save screen */
			//screen_save();

			///* Hack -- show empty slots */
			//item_tester_full = true;

			///* Display the equipment */
			//show_equip(OLIST_WEIGHT);

			///* Hack -- undo the hack above */
			//item_tester_full = false;

			///* Prompt for a command */
			//prt("(Equipment) Command: ", 0, 0);

			///* Get a new command */
			//e = inkey_ex();
			//if (!(e.type == EVT_KBRD && e.key.code == ESCAPE))
			//    Term_event_push(&e);

			///* Load screen */
			//screen_load();
		}

		/*
		 * Target command
		 */
		public static void target()
		{
			throw new NotImplementedException();
			//if (target_set_interactive(TARGET_KILL, -1, -1))
			//    msg("Target Selected.");
			//else
			//    msg("Target Aborted.");
		}


		public static void target_closest()
		{
			throw new NotImplementedException();
			//target_set_closest(TARGET_KILL);
		}


		/*
		 * Look command
		 */
		public static void look()
		{
			throw new NotImplementedException();
			///* Look around */
			//if (target_set_interactive(TARGET_LOOK, -1, -1))
			//{
			//    msg("Target Selected.");
			//}
		}



		/*
		 * Allow the player to examine other sectors on the map
		 */
		public static void locate()
		{
			throw new NotImplementedException();
			//int dir, y1, x1, y2, x2;

			//char tmp_val[80];

			//char out_val[160];


			///* Start at current panel */
			//y1 = Term.offset_y;
			//x1 = Term.offset_x;

			///* Show panels until done */
			//while (1)
			//{
			//    /* Get the current panel */
			//    y2 = Term.offset_y;
			//    x2 = Term.offset_x;
		
			//    /* Describe the location */
			//    if ((y2 == y1) && (x2 == x1))
			//    {
			//        tmp_val[0] = '\0';
			//    }
			//    else
			//    {
			//        strnfmt(tmp_val, sizeof(tmp_val), "%s%s of",
			//                ((y2 < y1) ? " north" : (y2 > y1) ? " south" : ""),
			//                ((x2 < x1) ? " west" : (x2 > x1) ? " east" : ""));
			//    }

			//    /* Prepare to ask which way to look */
			//    strnfmt(out_val, sizeof(out_val),
			//            "Map sector [%d,%d], which is%s your sector.  Direction?",
			//            (y2 / PANEL_HGT), (x2 / PANEL_WID), tmp_val);

			//    /* More detail */
			//    if (OPT(center_player))
			//    {
			//        strnfmt(out_val, sizeof(out_val),
			//                "Map sector [%d(%02d),%d(%02d)], which is%s your sector.  Direction?",
			//                (y2 / PANEL_HGT), (y2 % PANEL_HGT),
			//                (x2 / PANEL_WID), (x2 % PANEL_WID), tmp_val);
			//    }

			//    /* Assume no direction */
			//    dir = 0;

			//    /* Get a direction */
			//    while (!dir)
			//    {
			//        struct keypress command;

			//        /* Get a command (or Cancel) */
			//        if (!get_com(out_val, &command)) break;

			//        /* Extract direction */
			//        dir = target_dir(command);

			//        /* Error */
			//        if (!dir) bell("Illegal direction for locate!");
			//    }

			//    /* No direction */
			//    if (!dir) break;

			//    /* Apply the motion */
			//    change_panel(dir);

			//    /* Handle stuff */
			//    handle_stuff(p_ptr);
			//}

			///* Verify panel */
			//verify_panel();
		}

		//A and B were both const void *
		private static int cmp_mexp(object a, object b)
		{
			throw new NotImplementedException();
			//u16b ia = *(const u16b *)a;
			//u16b ib = *(const u16b *)b;
			//if (r_info[ia].mexp < r_info[ib].mexp)
			//    return -1;
			//if (r_info[ia].mexp > r_info[ib].mexp)
			//    return 1;
			//return (a < b ? -1 : (a > b ? 1 : 0));
		}

		//a and b were const void *s
		private static int cmp_level(object a, object b)
		{
			throw new NotImplementedException();
			//u16b ia = *(const u16b *)a;
			//u16b ib = *(const u16b *)b;
			//if (r_info[ia].level < r_info[ib].level)
			//    return -1;
			//if (r_info[ia].level > r_info[ib].level)
			//    return 1;
			//return cmp_mexp(a, b);
		}

		//a and b were const void *
		private static int cmp_tkill(object a, object b)
		{
			throw new NotImplementedException();
			//u16b ia = *(const u16b *)a;
			//u16b ib = *(const u16b *)b;
			//if (l_list[ia].tkills < l_list[ib].tkills)
			//    return -1;
			//if (l_list[ia].tkills > l_list[ib].tkills)
			//    return 1;
			//return cmp_level(a, b);
		}

		//a and b were const void *
		private static int cmp_pkill(object a, object b)
		{
			throw new NotImplementedException();
			//u16b ia = *(const u16b *)a;
			//u16b ib = *(const u16b *)b;
			//if (l_list[ia].pkills < l_list[ib].pkills)
			//    return -1;
			//if (l_list[ia].pkills > l_list[ib].pkills)
			//    return 1;
			//return cmp_tkill(a, b);
		}

		//a and be were const void *
		private static int cmp_monsters(object a, object b)
		{
			throw new NotImplementedException();
			//return cmp_level(a, b);
		}

		/*
		 * Search the monster, item, and feature types to find the
		 * meaning for the given symbol.
		 *
		 * Note: We currently search items first, then features, then
		 * monsters, and we return the first hit for a symbol.
		 * This is to prevent mimics and lurkers from matching
		 * a symbol instead of the item or feature it is mimicking.
		 *
		 * Todo: concatenate all matches into buf. This will be much
		 * easier once we can loop through item tvals instead of items
		 * (see note below.)
		 *
		 * Todo: Should this take the user's pref files into account?
		 */
		private static void lookup_symbol(keypress sym, string buf)
		{
			throw new NotImplementedException();
			//int i;
			//monster_base *race;

			///* Look through items */
			///* Note: We currently look through all items, and grab the tval when we find a match.
			//It would make more sense to loop through tvals, but then we need to associate
			//a display character with each tval. */
			//for (i = 1; i < z_info.k_max; i++) {
			//    if (k_info[i].d_char == (char)sym.code) {
			//        strnfmt(buf, max, "%c - %s.", (char)sym.code, tval_find_name(k_info[i].tval));
			//        return;
			//    }
			//}

			///* Look through features */
			///* Note: We need a better way of doing this. Currently '#' matches secret door,
			//and '^' matches trap door (instead of the more generic "trap"). */
			//for (i = 1; i < z_info.f_max; i++) {
			//    if (f_info[i].d_char == (char)sym.code) {
			//        strnfmt(buf, max, "%c - %s.", (char)sym.code, f_info[i].name);
			//        return;
			//    }
			//}
	
			///* Look through monster templates */
			//for (race = rb_info; race; race = race.next){
			//    if ((char)sym.code == race.d_char) {
			//        strnfmt(buf, max, "%c - %s.", (char)sym.code, race.text);
			//        return;
			//    }
			//}

			///* No matches */
			//strnfmt(buf, max, "%c - %s.", (char)sym.code, "Unknown Symbol");
	
			//return;
		}

		/*
		 * Identify a character, allow recall of monsters
		 *
		 * Several "special" responses recall "multiple" monsters:
		 *   ^A (all monsters)
		 *   ^U (all unique monsters)
		 *   ^N (all non-unique monsters)
		 *
		 * The responses may be sorted in several ways, see below.
		 *
		 * Note that the player ghosts are ignored, since they do not exist.
		 */
		public static void query_symbol()
		{
			throw new NotImplementedException();
			//int i, n, r_idx;
			//char buf[128];

			//struct keypress sym;
			//struct keypress query;

			//bool all = false;
			//bool uniq = false;
			//bool norm = false;

			//bool recall = false;

			//u16b *who;

			///* Get a character, or abort */
			//if (!get_com("Enter character to be identified, or control+[ANU]: ", &sym))
			//    return;

			///* Describe */
			//if (sym.code == KTRL('A'))
			//{
			//    all = true;
			//    my_strcpy(buf, "Full monster list.", sizeof(buf));
			//}
			//else if (sym.code == KTRL('U'))
			//{
			//    all = uniq = true;
			//    my_strcpy(buf, "Unique monster list.", sizeof(buf));
			//}
			//else if (sym.code == KTRL('N'))
			//{
			//    all = norm = true;
			//    my_strcpy(buf, "Non-unique monster list.", sizeof(buf));
			//}
			//else
			//{
			//    lookup_symbol(sym, buf, sizeof(buf));
			//}

			///* Display the result */
			//prt(buf, 0, 0);

			///* Allocate the "who" array */
			//who = C_ZNEW(z_info.r_max, u16b);

			///* Collect matching monsters */
			//for (n = 0, i = 1; i < z_info.r_max - 1; i++)
			//{
			//    monster_race *r_ptr = &r_info[i];
			//    monster_lore *l_ptr = &l_list[i];

			//    /* Nothing to recall */
			//    if (!OPT(cheat_know) && !l_ptr.sights) continue;

			//    /* Require non-unique monsters if needed */
			//    if (norm && rf_has(r_ptr.flags, RF_UNIQUE)) continue;

			//    /* Require unique monsters if needed */
			//    if (uniq && !rf_has(r_ptr.flags, RF_UNIQUE)) continue;

			//    /* Collect "appropriate" monsters */
			//    if (all || (r_ptr.d_char == (char)sym.code)) who[n++] = i;
			//}

			///* Nothing to recall */
			//if (!n)
			//{
			//    /* XXX XXX Free the "who" array */
			//    FREE(who);

			//    return;
			//}

			///* Buttons */
			//button_add("[y]", 'y');
			//button_add("[k]", 'k');
			///* Don't collide with the repeat button */
			//button_add("[n]", 'q'); 
			//redraw_stuff(p_ptr);

			///* Prompt */
			//put_str("Recall details? (y/k/n): ", 0, 40);

			///* Query */
			//query = inkey();

			///* Restore */
			//prt(buf, 0, 0);

			///* Buttons */
			//button_kill('y');
			//button_kill('k');
			//button_kill('q');
			//redraw_stuff(p_ptr);

			///* Interpret the response */
			//if (query.code == 'k')
			//{
			//    /* Sort by kills (and level) */
			//    sort(who, n, sizeof(*who), cmp_pkill);
			//}
			//else if (query.code == 'y' || query.code == 'p')
			//{
			//    /* Sort by level; accept 'p' as legacy */
			//    sort(who, n, sizeof(*who), cmp_level);
			//}
			//else
			//{
			//    /* Any unsupported response is "nope, no history please" */
	
			//    /* XXX XXX Free the "who" array */
			//    FREE(who);

			//    return;
			//}

			///* Start at the end */
			//i = n - 1;

			///* Button */
			//button_add("[r]", 'r');
			//button_add("[-]", '-');
			//button_add("[+]", '+');
			//redraw_stuff(p_ptr);

			///* Scan the monster memory */
			//while (1)
			//{
			//    /* Extract a race */
			//    r_idx = who[i];

			//    /* Hack -- Auto-recall */
			//    monster_race_track(r_idx);

			//    /* Hack -- Handle stuff */
			//    handle_stuff(p_ptr);

			//    /* Hack -- Begin the prompt */
			//    roff_top(r_idx);

			//    /* Hack -- Complete the prompt */
			//    Term_addstr(-1, TERM_WHITE, " [(r)ecall, ESC]");

			//    /* Interact */
			//    while (1)
			//    {
			//        /* Recall */
			//        if (recall)
			//        {
			//            /* Save screen */
			//            screen_save();

			//            /* Recall on screen */
			//            screen_roff(who[i]);

			//            /* Hack -- Complete the prompt (again) */
			//            Term_addstr(-1, TERM_WHITE, " [(r)ecall, ESC]");
			//        }

			//        /* Command */
			//        query = inkey();

			//        /* Unrecall */
			//        if (recall)
			//        {
			//            /* Load screen */
			//            screen_load();
			//        }

			//        /* Normal commands */
			//        if (query.code != 'r') break;

			//        /* Toggle recall */
			//        recall = !recall;
			//    }

			//    /* Stop scanning */
			//    if (query.code == ESCAPE) break;

			//    /* Move to "prev" monster */
			//    if (query.code == '-')
			//    {
			//        if (++i == n)
			//            i = 0;
			//    }

			//    /* Move to "next" monster */
			//    else
			//    {
			//        if (i-- == 0)
			//            i = n - 1;
			//    }
			//}

			///* Button */
			//button_kill('r');
			//button_kill('-');
			//button_kill('+');
			//redraw_stuff(p_ptr);

			///* Re-display the identity */
			//prt(buf, 0, 0);

			///* Free the "who" array */
			//FREE(who);
		}

		/* Centers the map on the player */
		public static void center_map()
		{
			throw new NotImplementedException();
			//center_panel();
		}

		/*
		 * Peruse the On-Line-Help
		 */
		public static void help()
		{
			throw new NotImplementedException();
			///* Save screen */
			//screen_save();

			///* Peruse the main help file */
			//(void)show_file("help.hlp", null, 0, 0);

			///* Load screen */
			//screen_load();
		}
	}
}
