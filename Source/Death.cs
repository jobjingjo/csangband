using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Death {
		/*
		 * Hack - save the time of death
		 */
		//static time_t death_time = (time_t)0;
		static int death_time = 0;



		/*
		 * Write formatted string `fmt` on line `y`, centred between points x1 and x2.
		 */
		static void put_str_centred(int y, int x1, int x2, string fmt, params object[] vals)
		{
			throw new NotImplementedException();
			//va_list vp;
			//char *tmp;
			//size_t len;
			//int x;

			///* Format into the (growable) tmp */
			//va_start(vp, fmt);
			//tmp = vformat(fmt, vp);
			//va_end(vp);

			///* Centre now */
			//len = strlen(tmp);
			//x = x1 + ((x2-x1)/2 - len/2);

			//put_str(tmp, y, x);
		}


		/*
		 * Display the tombstone
		 */
		static void print_tomb()
		{
			throw new NotImplementedException();
			//ang_file *fp;
			//char buf[1024];
			//int line = 0;


			//Term_clear();

			///* Open the death file */
			//path_build(buf, sizeof(buf), ANGBAND_DIR_FILE, "dead.txt");
			//fp = file_open(buf, MODE_READ, -1);

			//if (fp)
			//{
			//    while (file_getl(fp, buf, sizeof(buf)))
			//        put_str(buf, line++, 0);

			//    file_close(fp);
			//}

			//line = 7;

			//put_str_centred(line++, 8, 8+31, "%s", op_ptr.full_name);
			//put_str_centred(line++, 8, 8+31, "the");
			//if (p_ptr.total_winner)
			//    put_str_centred(line++, 8, 8+31, "Magnificent");
			//else
			//    put_str_centred(line++, 8, 8+31, "%s", p_ptr.class.title[(p_ptr.lev - 1) / 5]);

			//line++;

			//put_str_centred(line++, 8, 8+31, "%s", p_ptr.class.name);
			//put_str_centred(line++, 8, 8+31, "Level: %d", (int)p_ptr.lev);
			//put_str_centred(line++, 8, 8+31, "Exp: %d", (int)p_ptr.exp);
			//put_str_centred(line++, 8, 8+31, "AU: %d", (int)p_ptr.au);
			//put_str_centred(line++, 8, 8+31, "Killed on Level %d", p_ptr.depth);
			//put_str_centred(line++, 8, 8+31, "by %s.", p_ptr.died_from);

			//line++;

			//put_str_centred(line++, 8, 8+31, "by %-.24s", ctime(&death_time));
		}


		/*
		 * Know inventory and home items upon death
		 */
		static void death_knowledge()
		{
			throw new NotImplementedException();
			//struct store *st_ptr = &stores[STORE_HOME];
			//object_type *o_ptr;

			//int i;

			//for (i = 0; i < ALL_INVEN_TOTAL; i++)
			//{
			//    o_ptr = &p_ptr.inventory[i];
			//    if (!o_ptr.kind) continue;

			//    object_flavor_aware(o_ptr);
			//    object_notice_everything(o_ptr);
			//}

			//for (i = 0; i < st_ptr.stock_num; i++)
			//{
			//    o_ptr = &st_ptr.stock[i];
			//    if (!o_ptr.kind) continue;

			//    object_flavor_aware(o_ptr);
			//    object_notice_everything(o_ptr);
			//}

			//history_unmask_unknown();

			///* Hack -- Recalculate bonuses */
			//p_ptr.update |= (PU_BONUS);
			//handle_stuff(p_ptr);
		}



		/*
		 * Display the winner crown
		 */
		static void display_winner()
		{
			throw new NotImplementedException();
			//char buf[1024];
			//ang_file *fp;

			//int wid, hgt;
			//int i = 2;
			//int width = 0;


			//path_build(buf, sizeof(buf), ANGBAND_DIR_FILE, "crown.txt");
			//fp = file_open(buf, MODE_READ, -1);

			//Term_clear();
			//Term_get_size(&wid, &hgt);

			//if (fp)
			//{
			//    /* Get us the first line of file, which tells us how long the */
			//    /* longest line is */
			//    file_getl(fp, buf, sizeof(buf));
			//    sscanf(buf, "%d", &width);
			//    if (!width) width = 25;

			//    /* Dump the file to the screen */
			//    while (file_getl(fp, buf, sizeof(buf)))
			//        put_str(buf, i++, (wid/2) - (width/2));

			//    file_close(fp);
			//}

			//put_str_centred(i, 0, wid, "All Hail the Mighty %s!", p_ptr.sex.winner);

			//flush();
			//pause_line(Term);
		}


		/*
		 * Menu command: dump character dump to file.
		 */
		static void death_file(string title, int row)
		{
			throw new NotImplementedException();
			//char buf[1024];
			//char ftmp[80];

			//strnfmt(ftmp, sizeof(ftmp), "%s.txt", op_ptr.base_name);

			//if (get_file(ftmp, buf, sizeof buf))
			//{
			//    errr err;

			//    /* Dump a character file */
			//    screen_save();
			//    err = file_character(buf, false);
			//    screen_load();

			//    /* Check result */
			//    if (err)
			//        msg("Character dump failed!");
			//    else
			//        msg("Character dump successful.");

			//    /* Flush messages */
			//    message_flush();
			//}
		}

		/*
		 * Menu command: view character dump and inventory.
		 */
		static void death_info(string title, int row)
		{
			throw new NotImplementedException();
			//int i, j, k;
			//object_type *o_ptr;
			//struct store *st_ptr = &stores[STORE_HOME];


			//screen_save();

			///* Display player */
			//display_player(0);

			///* Prompt for inventory */
			//prt("Hit any key to see more information: ", 0, 0);

			///* Allow abort at this point */
			//(void)anykey();


			///* Show equipment and inventory */

			///* Equipment -- if any */
			//if (p_ptr.equip_cnt)
			//{
			//    Term_clear();
			//    item_tester_full = true;
			//    show_equip(OLIST_WEIGHT);
			//    prt("You are using: -more-", 0, 0);
			//    (void)anykey();
			//}

			///* Inventory -- if any */
			//if (p_ptr.inven_cnt)
			//{
			//    Term_clear();
			//    item_tester_full = true;
			//    show_inven(OLIST_WEIGHT);
			//    prt("You are carrying: -more-", 0, 0);
			//    (void)anykey();
			//}



			///* Home -- if anything there */
			//if (st_ptr.stock_num)
			//{
			//    /* Display contents of the home */
			//    for (k = 0, i = 0; i < st_ptr.stock_num; k++)
			//    {
			//        /* Clear screen */
			//        Term_clear();

			//        /* Show 12 items */
			//        for (j = 0; (j < 12) && (i < st_ptr.stock_num); j++, i++)
			//        {
			//            byte attr;

			//            char o_name[80];
			//            char tmp_val[80];

			//            /* Get the object */
			//            o_ptr = &st_ptr.stock[i];

			//            /* Print header, clear line */
			//            strnfmt(tmp_val, sizeof(tmp_val), "%c) ", I2A(j));
			//            prt(tmp_val, j+2, 4);

			//            /* Get the object description */
			//            object_desc(o_name, sizeof(o_name), o_ptr,
			//                        ODESC_PREFIX | ODESC_FULL);

			//            /* Get the inventory color */
			//            attr = tval_to_attr[o_ptr.tval % N_ELEMENTS(tval_to_attr)];

			//            /* Display the object */
			//            c_put_str(attr, o_name, j+2, 7);
			//        }

			//        /* Caption */
			//        prt(format("Your home contains (page %d): -more-", k+1), 0, 0);

			//        /* Wait for it */
			//        (void)anykey();
			//    }
			//}

			//screen_load();
		}

		/*
		 * Menu command: peruse pre-death messages.
		 */
		static void death_messages(string title, int row)
		{
			//screen_save();
			//do_cmd_messages();
			//screen_load();
		}

		/*
		 * Menu command: see top twenty scores.
		 */
		static void death_scores(string title, int row)
		{
			//screen_save();
			//show_scores();
			//screen_load();
		}

		/*
		 * Menu command: examine items in the inventory.
		 */
		static void death_examine(string title, int row)
		{
			throw new NotImplementedException();
			//int item;
			//const char *q, *s;

			///* Get an item */
			//q = "Examine which item? ";
			//s = "You have nothing to examine.";

			//while (get_item(&item, q, s, 0, (USE_INVEN | USE_EQUIP | IS_HARMLESS)))
			//{
			//    char header[120];

			//    textblock *tb;
			//    region area = { 0, 0, 0, 0 };

			//    object_type *o_ptr = &p_ptr.inventory[item];

			//    tb = object_info(o_ptr, OINFO_FULL);
			//    object_desc(header, sizeof(header), o_ptr, ODESC_PREFIX | ODESC_FULL);

			//    textui_textblock_show(tb, area, format("%^s", header));
			//    textblock_free(tb);
			//}
		}


		/*
		 * Menu command: view character history.
		 */
		static void death_history(string title, int row)
		{
			throw new NotImplementedException();
			//history_display();
		}

		/*
		 * Menu command: allow spoiler generation (mainly for randarts).
		 */
		static void death_spoilers(string title, int row)
		{
			throw new NotImplementedException();
			//do_cmd_spoilers();
		}

		/* Menu command: toggle birth_keep_randarts option. */
		static void death_randarts(string title, int row)
		{
			throw new NotImplementedException();
			//if (p_ptr.randarts)
			//    option_set(option_name(OPT_birth_keep_randarts),
			//        get_check("Keep randarts for next game? "));
			//else
			//    msg("You are not playing with randarts!");
		}


		/*
		 * Menu structures for the death menu. Note that Quit must always be the
		 * last option, due to a hard-coded check in death_screen
		 */
		static Menu_Type death_menu;
		static Menu_Action[] death_actions =
		{
			new Menu_Action( 0, 'i', "Information",   death_info      ),
			new Menu_Action( 0, 'm', "Messages",      death_messages  ),
			new Menu_Action( 0, 'f', "File dump",     death_file      ),
			new Menu_Action( 0, 'v', "View scores",   death_scores    ),
			new Menu_Action( 0, 'x', "Examine items", death_examine   ),
			new Menu_Action( 0, 'h', "History",       death_history   ),
			new Menu_Action( 0, 's', "Spoilers",      death_spoilers  ),
			new Menu_Action( 0, 'r', "Keep randarts", death_randarts  ),
			new Menu_Action( 0, 'q', "Quit",          null            ),
		};



		/*
		 * Handle character death
		 */
		public static void screen()
		{
			throw new NotImplementedException();
			//bool done = false;
			//const region area = { 51, 2, 0, N_ELEMENTS(death_actions) };

			///* Retire in the town in a good state */
			//if (p_ptr.total_winner)
			//{
			//    p_ptr.depth = 0;
			//    my_strcpy(p_ptr.died_from, "Ripe Old Age", sizeof(p_ptr.died_from));
			//    p_ptr.exp = p_ptr.max_exp;
			//    p_ptr.lev = p_ptr.max_lev;
			//    p_ptr.au += 10000000L;

			//    display_winner();
			//}

			///* Get time of death */
			//(void)time(&death_time);
			//print_tomb();
			//death_knowledge();
			//enter_score(&death_time);

			///* Flush all input and output */
			//flush();
			//message_flush();

			///* Display and use the death menu */
			//if (!death_menu)
			//{
			//    death_menu = menu_new_action(death_actions,
			//            N_ELEMENTS(death_actions));

			//    death_menu.flags = MN_CASELESS_TAGS;
			//}

			//menu_layout(death_menu, &area);

			//while (!done)
			//{
			//    ui_event e = menu_select(death_menu, EVT_KBRD, false);
			//    if (e.type == EVT_KBRD)
			//    {
			//        if (e.key.code == KTRL('X')) break;
			//    }
			//    else if (e.type == EVT_SELECT)
			//    {
			//        done = get_check("Do you want to quit? ");
			//    }
			//}

			///* Save dead player */
			//if (!savefile_save(savefile))
			//{
			//    msg("death save failed!");
			//    message_flush();
			//}
		}

	}
}
