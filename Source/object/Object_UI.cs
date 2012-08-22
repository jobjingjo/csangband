using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Object {
	partial class Object {
		/*
		 * Let the user select an item, save its "index"
		 *
		 * Return true only if an acceptable item was chosen by the user.
		 *
		 * The selected item must satisfy the "item_tester_hook()" function,
		 * if that hook is set, and the "item_tester_tval", if that value is set.
		 *
		 * All "item_tester" restrictions are cleared before this function returns.
		 *
		 * The user is allowed to choose acceptable items from the equipment,
		 * inventory, or floor, respectively, if the proper flag was given,
		 * and there are any acceptable items in that location.
		 *
		 * The equipment or inventory are displayed (even if no acceptable
		 * items are in that location) if the proper flag was given.
		 *
		 * If there are no acceptable items available anywhere, and "str" is
		 * not null, then it will be used as the text of a warning message
		 * before the function returns.
		 *
		 * Note that the user must press "-" to specify the item on the floor,
		 * and there is no way to "examine" the item on the floor, while the
		 * use of "capital" letters will "examine" an inventory/equipment item,
		 * and prompt for its use.
		 *
		 * If a legal item is selected from the inventory, we save it in "cp"
		 * directly (0 to 35), and return true.
		 *
		 * If a legal item is selected from the floor, we save it in "cp" as
		 * a negative (-1 to -511), and return true.
		 *
		 * If no item is available, we do nothing to "cp", and we display a
		 * warning message, using "str" if available, and return false.
		 *
		 * If no item is selected, we do nothing to "cp", and return false.
		 *
		 * Global "p_ptr.command_wrk" is used to choose between equip/inven/floor
		 * listings.  It is equal to USE_INVEN or USE_EQUIP or USE_FLOOR, except
		 * when this function is first called, when it is equal to zero, which will
		 * cause it to be set to USE_INVEN.
		 *
		 * We always erase the prompt when we are done, leaving a blank line,
		 * or a warning message, if appropriate, if no items are available.
		 *
		 * Note that only "acceptable" floor objects get indexes, so between two
		 * commands, the indexes of floor objects may change.  XXX XXX XXX
		 */
		public static bool get_item(ref int cp, string pmt, string str, Command_Code cmd, int mode)
		{
		    int py = Misc.p_ptr.py;
		    int px = Misc.p_ptr.px;
		    char cmdkey = Command.lookup_key(cmd);

		    keypress which;

		    int j, k = 0;

		    int i1, i2;
		    int e1, e2;
		    int f1, f2;

		    bool done, item;

		    bool oops = false;

		    bool use_inven = ((mode & Misc.USE_INVEN) != 0 ? true : false);
		    bool use_equip = ((mode & Misc.USE_EQUIP) != 0 ? true : false);
		    bool use_floor = ((mode & Misc.USE_FLOOR) != 0 ? true : false);
		    bool use_quiver = ((mode & Misc.QUIVER_TAGS) != 0 ? true : false);
		    bool is_harmless = ((mode & Misc.IS_HARMLESS) != 0 ? true : false);
		    bool quiver_tags = ((mode & Misc.QUIVER_TAGS) != 0 ? true : false);

		    olist_detail_t olist_mode = 0;

		    bool allow_inven = false;
		    bool allow_equip = false;
		    bool allow_floor = false;

		    bool toggle = false;

		    string tmp_val;//[160];
		    string out_val;//[160];

		    int[] floor_list = new int[Misc.MAX_FLOOR_STACK];
		    int floor_num;

		    bool show_list = true;


		    /* Object list display modes */
		    if ((mode & Misc.SHOW_FAIL) != 0)
		        olist_mode |= (olist_detail_t.OLIST_FAIL);
		    else
		        olist_mode |= (olist_detail_t.OLIST_WEIGHT);
		    if ((mode & Misc.SHOW_PRICES) != 0)
		        olist_mode |= (olist_detail_t.OLIST_PRICE);

		    /* Paranoia XXX XXX XXX */
		    Utilities.message_flush();


		    /* Not done */
		    done = false;

		    /* No item selected */
		    item = false;


		    /* Full inventory */
		    i1 = 0;
		    i2 = Misc.INVEN_PACK - 1;

		    /* Forbid inventory */
		    if (!use_inven) i2 = -1;

		    /* Restrict inventory indexes */
		    while ((i1 <= i2) && (!Object.get_item_okay(i1))) i1++;
		    while ((i1 <= i2) && (!get_item_okay(i2))) i2--;

		    /* Accept inventory */
		    if (i1 <= i2) allow_inven = true;


		    /* Full equipment */
		    e1 = Misc.INVEN_WIELD;
		    e2 = Misc.ALL_INVEN_TOTAL - 1;

		    /* Forbid equipment */
		    if (!use_equip) e2 = -1;

		    /* Restrict equipment indexes */
		    while ((e1 <= e2) && (!get_item_okay(e1))) e1++;
		    while ((e1 <= e2) && (!get_item_okay(e2))) e2--;

		    /* Accept equipment */
		    if (e1 <= e2) allow_equip = true;


		    /* Scan all non-gold objects in the grid */
		    floor_num = scan_floor(floor_list, floor_list.Length, py, px, 0x03);

		    /* Full floor */
		    f1 = 0;
		    f2 = floor_num - 1;

		    /* Forbid floor */
		    if (!use_floor) f2 = -1;

		    /* Restrict floor indexes */
		    while ((f1 <= f2) && (!get_item_okay(0 - floor_list[f1]))) f1++;
		    while ((f1 <= f2) && (!get_item_okay(0 - floor_list[f2]))) f2--;

		    /* Accept floor */
		    if (f1 <= f2) allow_floor = true;


		    /* Require at least one legal choice */
		    if (!allow_inven && !allow_equip && !allow_floor)
		    {
		        /* Oops */
		        oops = true;
		        done = true;
		    }

		    /* Analyze choices */
		    else
		    {
		        /* Hack -- Start on equipment if requested */
		        if ((Misc.p_ptr.command_wrk == Misc.USE_EQUIP) && use_equip)
		            Misc.p_ptr.command_wrk = Misc.USE_EQUIP;

		        /* If we are using the quiver then start on equipment */
		        else if (use_quiver)
		            Misc.p_ptr.command_wrk = Misc.USE_EQUIP;

		        /* Use inventory if allowed */
		        else if (use_inven)
		            Misc.p_ptr.command_wrk = Misc.USE_INVEN;

		        /* Use equipment if allowed */
		        else if (use_equip)
		            Misc.p_ptr.command_wrk = Misc.USE_EQUIP;

		        /* Use floor if allowed */
		        else if (use_floor)
		            Misc.p_ptr.command_wrk = Misc.USE_FLOOR;

		        /* Hack -- Use (empty) inventory */
		        else
		            Misc.p_ptr.command_wrk = Misc.USE_INVEN;
		    }


		    /* Start out in "display" mode */
		    if (show_list)
		    {
		        /* Save screen */
		        Utilities.screen_save();
		    }


		    /* Repeat until done */
		    while (!done)
		    {
		        int ni = 0;
		        int ne = 0;

		        /* Scan windows */
		        for (j = 0; j < Misc.ANGBAND_TERM_MAX; j++)
		        {
		            /* Unused */
		            if (Misc.angband_term[j] == null) continue;

		            /* Count windows displaying inven */
		            if ((Player.Player_Other.instance.window_flag[j] & (Misc.PW_INVEN)) != 0) ni++;

		            /* Count windows displaying equip */
		            if ((Player.Player_Other.instance.window_flag[j] & (Misc.PW_EQUIP)) != 0) ne++;
		        }

		        /* Toggle if needed */
		        if (((Misc.p_ptr.command_wrk == Misc.USE_EQUIP) && ni != 0 && ne == 0) ||
		            ((Misc.p_ptr.command_wrk == Misc.USE_INVEN) && ni == 0 && ne != 0))
		        {
		            /* Toggle */
		            Command_Info.toggle_inven_equip();

		            /* Track toggles */
		            toggle = !toggle;
		        }

		        /* Redraw */
		        Misc.p_ptr.redraw |= (Misc.PR_INVEN | Misc.PR_EQUIP);

		        /* Redraw windows */
		        Misc.p_ptr.redraw_stuff();

		        /* Viewing inventory */
		        if (Misc.p_ptr.command_wrk == Misc.USE_INVEN)
		        {
		            /* Redraw if needed */
		            if (show_list) show_inven(olist_mode);

		            /* Begin the prompt */
					out_val = "Inven:";

		            /* List choices */
		            if (i1 <= i2)
		            {
		                /* Build the prompt */
						tmp_val = String.Format(" {0}-{1},", index_to_label(i1), index_to_label(i2));

		                /* Append */
						out_val += tmp_val;
		            }

		            /* Indicate ability to "view" */
		            if (!show_list)
		            {
						out_val += " * to see,";
						Button.button_add("[*]", '*');
		            }

		            /* Indicate legality of "toggle" */
		            if (use_equip)
		            {
						out_val += " / for Equip,";
		                Button.button_add("[/]", '/');
		            }

		            /* Indicate legality of the "floor" */
		            if (allow_floor)
		            {
						out_val += " - for floor,";
		                Button.button_add("[-]", '-');
		            }
		        }

		        /* Viewing equipment */
		        else if (Misc.p_ptr.command_wrk == Misc.USE_EQUIP)
		        {
		            /* Redraw if needed */
		            if (show_list) show_equip(olist_mode);

		            /* Begin the prompt */
					out_val = "Equip:";

		            /* List choices */
		            if (e1 <= e2)
		            {
		                /* Build the prompt */
						tmp_val = String.Format(" {0}-{1},", index_to_label(e1), index_to_label(e2));

		                /* Append */
						out_val += tmp_val;
		            }

		            /* Indicate ability to "view" */
		            if (!show_list)
		            {
						out_val += " * to see,";
		                Button.button_add("[*]", '*');
		            }

		            /* Indicate legality of "toggle" */
		            if (use_inven)
		            {
						out_val += " / for Inven,";
		                Button.button_add("[/]", '/');
		            }

		            /* Indicate legality of the "floor" */
		            if (allow_floor)
		            {
						out_val += " - for floor,";
		                Button.button_add("[!]", '!');
		            }
		        }

		        /* Viewing floor */
		        else
		        {
		            /* Redraw if needed */
		            if (show_list) show_floor(floor_list, floor_num, olist_mode);

		            /* Begin the prompt */
					out_val = "Floor:";

		            /* List choices */
		            if (f1 <= f2)
		            {
		                /* Build the prompt */
						tmp_val = String.Format(" {0}-{1}", Basic.I2A(f1), Basic.I2A(f2));

		                /* Append */
						out_val += tmp_val;
		            }

		            /* Indicate ability to "view" */
		            if (!show_list)
		            {
						out_val += " * to see,";
		                Button.button_add("[*]", '*');
		            }

		            /* Append */
		            if (use_inven)
		            {
						out_val += " / for Inven,";
		                Button.button_add("[/]", '/');
		            }

		            /* Append */
		            else if (use_equip)
		            {
						out_val += " / for Equip,";
		                Button.button_add("[/]", '/');
		            }
		        }

		        Misc.p_ptr.redraw_stuff();

		        /* Finish the prompt */
				out_val += " ESC";

		        /* Build the prompt */
				tmp_val = String.Format("({0}) {1}", out_val, pmt);

		        /* Show the prompt */
		        Utilities.prt(tmp_val, 0, 0);


		        /* Get a key */
		        which = Utilities.inkey();

		        /* Parse it */
		        switch (which.code)
		        {
		            case keycode_t.ESCAPE:
		            {
		                done = true;
		                break;
		            }

		            case (keycode_t)'/':
		            {
		                /* Toggle to inventory */
		                if (use_inven && (Misc.p_ptr.command_wrk != Misc.USE_INVEN))
		                {
		                    Misc.p_ptr.command_wrk = Misc.USE_INVEN;
		                }

		                /* Toggle to equipment */
		                else if (use_equip && (Misc.p_ptr.command_wrk != Misc.USE_EQUIP))
		                {
		                    Misc.p_ptr.command_wrk = Misc.USE_EQUIP;
		                }

		                /* No toggle allowed */
		                else
		                {
		                    Utilities.bell("Cannot switch item selector!");
		                    break;
		                }


		                /* Hack -- Fix screen */
		                if (show_list)
		                {
		                    /* Load screen */
		                    Utilities.screen_load();

		                    /* Save screen */
		                    Utilities.screen_save();
		                }

		                /* Need to redraw */
		                break;
		            }

		            case (keycode_t)'-':
		            {
		                /* Paranoia */
		                if (!allow_floor)
		                {
		                    Utilities.bell("Cannot select floor!");
		                    break;
		                }

		                /* There is only one item */
		                if (floor_num == 1)
		                {
		                    /* Auto-select */
		                    if (Misc.p_ptr.command_wrk == (Misc.USE_FLOOR))
		                    {
		                        /* Special index */
		                        k = 0 - floor_list[0];

		                        /* Allow player to "refuse" certain actions */
		                        if (!get_item_allow(k, cmdkey, is_harmless))
		                        {
		                            done = true;
		                            break;
		                        }

		                        /* Accept that choice */
		                        cp = k;
		                        item = true;
		                        done = true;

		                        break;
		                    }
		                }

		                /* Hack -- Fix screen */
		                if (show_list)
		                {
		                    /* Load screen */
		                    Utilities.screen_load();

		                    /* Save screen */
		                    Utilities.screen_save();
		                }

		                Misc.p_ptr.command_wrk = (Misc.USE_FLOOR);

						//#if 0
						//                /* Check each legal object */
						//                for (i = 0; i < floor_num; ++i)
						//                {
						//                    /* Special index */
						//                    k = 0 - floor_list[i];

						//                    /* Skip non-okay objects */
						//                    if (!get_item_okay(k)) continue;

						//                    /* Allow player to "refuse" certain actions */
						//                    if (!get_item_allow(k, cmdkey, is_harmless)) continue;

						//                    /* Accept that choice */
						//                    (*cp) = k;
						//                    item = true;
						//                    done = true;
						//                    break;
						//                }
						//#endif

		                break;
		            }

		            case (keycode_t)'0':
		            case (keycode_t)'1': case (keycode_t)'2': case (keycode_t)'3':
		            case (keycode_t)'4': case (keycode_t)'5': case (keycode_t)'6':
		            case (keycode_t)'7': case (keycode_t)'8': case (keycode_t)'9':
		            {
		                /* Look up the tag */
		                if (get_tag(ref k, (char)which.code, cmd, quiver_tags) == 0)
		                {
		                    Utilities.bell("Illegal object choice (tag)!");
		                    break;
		                }

		                /* Hack -- Validate the item */
		                if ((k < Misc.INVEN_WIELD) ? !allow_inven : !allow_equip)
		                {
		                    Utilities.bell("Illegal object choice (tag)!");
		                    break;
		                }

		                /* Validate the item */
		                if (!get_item_okay(k))
		                {
		                    Utilities.bell("Illegal object choice (tag)!");
		                    break;
		                }

		                /* Allow player to "refuse" certain actions */
		                if (!get_item_allow(k, cmdkey, is_harmless))
		                {
		                    done = true;
		                    break;
		                }

		                /* Accept that choice */
		                cp = k;
		                item = true;
		                done = true;
		                break;
		            }

		            case (keycode_t)'\n':
		            case (keycode_t)'\r':
		            {
		                /* Choose "default" inventory item */
		                if (Misc.p_ptr.command_wrk == Misc.USE_INVEN)
		                {
		                    if (i1 != i2)
		                    {
		                        Utilities.bell("Illegal object choice (default)!");
		                        break;
		                    }

		                    k = i1;
		                }

		                /* Choose the "default" slot (0) of the quiver */
		                else if (quiver_tags)
		                    k = e1;

		                /* Choose "default" equipment item */
		                else if (Misc.p_ptr.command_wrk == Misc.USE_EQUIP)
		                {
		                    if (e1 != e2)
		                    {
		                        Utilities.bell("Illegal object choice (default)!");
		                        break;
		                    }

		                    k = e1;
		                }

		                /* Choose "default" floor item */
		                else
		                {
		                    if (f1 != f2)
		                    {
		                        Utilities.bell("Illegal object choice (default)!");
		                        break;
		                    }

		                    k = 0 - floor_list[f1];
		                }

		                /* Validate the item */
		                if (!get_item_okay(k))
		                {
		                    Utilities.bell("Illegal object choice (default)!");
		                    break;
		                }

		                /* Allow player to "refuse" certain actions */
		                if (!get_item_allow(k, cmdkey, is_harmless))
		                {
		                    done = true;
		                    break;
		                }

		                /* Accept that choice */
		                cp = k;
		                item = true;
		                done = true;
		                break;
		            }

		            default:
		            {
		                bool verify;

		                /* Note verify */
		                verify = (Char.IsUpper((char)which.code) ? true : false);

		                /* Lowercase */
		                which.code = (keycode_t)Char.ToLower((char)which.code);

		                /* Convert letter to inventory index */
		                if (Misc.p_ptr.command_wrk == Misc.USE_INVEN)
		                {
		                    k = label_to_inven((char)which.code);

		                    if (k < 0)
		                    {
		                        Utilities.bell("Illegal object choice (inven)!");
		                        break;
		                    }
		                }

		                /* Convert letter to equipment index */
		                else if (Misc.p_ptr.command_wrk == Misc.USE_EQUIP)
		                {
		                    k = label_to_equip((char)which.code);

		                    if (k < 0)
		                    {
		                        Utilities.bell("Illegal object choice (equip)!");
		                        break;
		                    }
		                }

		                /* Convert letter to floor index */
		                else
		                {
		                    k = (Char.IsLower((char)which.code) ? Basic.A2I((char)which.code) : -1);

		                    if (k < 0 || k >= floor_num)
		                    {
		                        Utilities.bell("Illegal object choice (floor)!");
		                        break;
		                    }

		                    /* Special index */
		                    k = 0 - floor_list[k];
		                }

		                /* Validate the item */
		                if (!get_item_okay(k))
		                {
		                    Utilities.bell("Illegal object choice (normal)!");
		                    break;
		                }

		                /* Verify the item */
		                if (verify && !verify_item("Try", k))
		                {
		                    done = true;
		                    break;
		                }

		                /* Allow player to "refuse" certain actions */
		                if (!get_item_allow(k, cmdkey, is_harmless))
		                {
		                    done = true;
		                    break;
		                }

		                /* Accept that choice */
		                cp = k;
		                item = true;
		                done = true;
		                break;
		            }
		        }
		    }


		    /* Fix the screen if necessary */
		    if (show_list)
		    {
		        /* Load screen */
		        Utilities.screen_load();

		        /* Hack -- Cancel "display" */
		        show_list = false;
		    }


		    /* Kill buttons */
		    Button.button_kill('*');
		    Button.button_kill('/');
		    Button.button_kill('-');
		    Button.button_kill('!');
		    Misc.p_ptr.redraw_stuff();
 
		    /* Forget the item_tester_tval restriction */
		    Misc.item_tester_tval = 0;

		    /* Forget the item_tester_hook restriction */
		    Misc.item_tester_hook = null;


		    /* Toggle again if needed */
		    if (toggle) Command_Info.toggle_inven_equip();

		    /* Update */
		    Misc.p_ptr.redraw |= (Misc.PR_INVEN | Misc.PR_EQUIP);
		    Misc.p_ptr.redraw_stuff();


		    /* Clear the prompt line */
		    Utilities.prt("", 0, 0);

		    /* Warning if needed */
		    if (oops && str != null) Utilities.msg("{0}", str);

		    /* Result */
		    return (item);
		}

		/**
		 * Modes for item lists in "show_inven()"  "show_equip()" and "show_floor()"
		 */
		public enum olist_detail_t
		{
			OLIST_NONE   = 0x00,   /* No options */
   			OLIST_WINDOW = 0x01,   /* Display list in a sub-term (left-align) */
   			OLIST_QUIVER = 0x02,   /* Display quiver lines */
   			OLIST_GOLD   = 0x04,   /* Include gold in the list */
			OLIST_WEIGHT = 0x08,   /* Show item weight */
			OLIST_PRICE  = 0x10,   /* Show item price */
			OLIST_FAIL   = 0x20    /* Show device failure */

		}

		/*
		 * Display the inventory.  Builds a list of objects and passes them
		 * off to show_obj_list() for display.  Mode flags documented in
		 * object.h
		 */
		public static void show_inven(olist_detail_t mode)
		{
			int i, last_slot = -1;
			int diff = Player.Player.weight_remaining();

			Object o_ptr;

			int num_obj = 0;
			string[] labels = new string[50]; //up to 80 long each
			Object[] objects = new Object[50];

			bool in_term = (mode & olist_detail_t.OLIST_WINDOW) != 0 ? true : false;

			/* Include burden for term windows */
			if (in_term)
			{
				labels[num_obj] = String.Format("Burden {0}.{1} lb ({2}.{3} lb {4}) ",
			            Misc.p_ptr.total_weight / 10, Misc.p_ptr.total_weight % 10,
			            Math.Abs(diff) / 10, Math.Abs(diff) % 10,
			            (diff < 0 ? "overweight" : "remaining"));

			    objects[num_obj] = null;
			    num_obj++;
			}

			/* Find the last occupied inventory slot */
			for (i = 0; i < Misc.INVEN_PACK; i++)
			{
			    o_ptr = Misc.p_ptr.inventory[i];
			    if (o_ptr.kind != null) last_slot = i;
			}

			/* Build the object list */
			for (i = 0; i <= last_slot; i++)
			{
			    o_ptr = Misc.p_ptr.inventory[i];

			    /* Acceptable items get a label */
			    if (o_ptr.item_tester_okay())
			        labels[num_obj] = index_to_label(i).ToString() + ") ";

			    /* Unacceptable items are still displayed in term windows */
			    else if (in_term)
					labels[num_obj] = "   ";

			    /* Unacceptable items are skipped in the main window */
			    else continue;

			    /* Save the object */
			    objects[num_obj] = o_ptr;
			    num_obj++;
			}

			/* Display the object list */
			if (in_term)
			    /* Term window starts with a burden header */
			    show_obj_list(num_obj, 1, labels, objects, mode);
			else
			    show_obj_list(num_obj, 0, labels, objects, mode);
		}

		/*
		 * Display a list of objects.  Each object may be prefixed with a label.
		 * Used by show_inven(), show_equip(), and show_floor().  Mode flags are
		 * documented in object.h
		 */
		static void show_obj_list(int num_obj, int num_head, string[] labels, Object[] objects, olist_detail_t mode)
		{
			int i, row = 0, col = 0;
			ConsoleColor attr;
			int max_len = 0;
			int ex_width = 0, ex_offset, ex_offset_ctr;

			Object o_ptr;
			string[] o_name = new string[50]; //max length 80
			string tmp_val; //80
	
			bool in_term;
	
			in_term = (mode & olist_detail_t.OLIST_WINDOW) != 0 ? true : false;

			if (in_term) max_len = 40;

			/* Calculate name offset and max name length */
			for (i = 0; i < num_obj; i++)
			{
				o_ptr = objects[i];

				/* null objects are used to skip lines, or display only a label */		
				if (o_ptr == null || o_ptr.kind == null)
				{
					if (i < num_head)
						o_name[i] = "";
					else
						o_name[i] = "(nothing)";
				}
				else
					o_name[i] = o_ptr.object_desc(Detail.PREFIX | Detail.FULL);

				/* Max length of label + object name */
				max_len = Math.Max(max_len, labels[i].Length + o_name[i].Length);
			}

			/* Take the quiver message into consideration */
			if ((mode & olist_detail_t.OLIST_QUIVER) != 0 && Misc.p_ptr.quiver_slots > 0)
				max_len = Math.Max(max_len, 24);

			/* Width of extra fields */
			if ((mode & olist_detail_t.OLIST_WEIGHT) != 0) ex_width += 9;
			if ((mode & olist_detail_t.OLIST_PRICE) != 0) ex_width += 9;
			if ((mode & olist_detail_t.OLIST_FAIL) != 0) ex_width += 10;

			/* Determine beginning row and column */
			if (in_term)
			{
				/* Term window */
				row = 0;
				col = 0;
			}
			else
			{
				/* Main window */
				row = 1;
				col = Term.instance.wid - 1 - max_len - ex_width;

				if (col < 3) col = 0;
			}

			/* Column offset of the first extra field */
			ex_offset = Math.Min(max_len, (Term.instance.wid - 1 - ex_width - col));

			/* Output the list */
			for (i = 0; i < num_obj; i++)
			{
				o_ptr = objects[i];
		
				/* Clear the line */
				Utilities.prt("", row + i, Math.Max(col - 2, 0));

				/* If we have no label then we won't display anything */
				if (labels[i].Length == 0) continue;

				/* Print the label */
				Utilities.put_str(labels[i], row + i, col);

				/* Limit object name */
				if (labels[i].Length + o_name[i].Length > ex_offset)
				{
					int truncate = ex_offset - labels[i].Length;
			
					if (truncate < 0) truncate = 0;
					if (truncate > o_name[i].Length - 1) truncate = o_name[i].Length - 1;

					o_name[i] = o_name[i].Substring(0, truncate);
					//o_name[i][truncate] = '\0';
				}
		
				/* Item kind determines the color of the output */
				if (o_ptr != null && o_ptr.kind != null)
					attr = Misc.tval_to_attr[o_ptr.tval % Misc.tval_to_attr.Length];
				else
					attr = ConsoleColor.Gray;

				/* Object name */
				Utilities.c_put_str(attr, o_name[i], row + i, col + labels[i].Length);

				/* If we don't have an object, we can skip the rest of the output */
				if (o_ptr == null || o_ptr.kind == null) continue;

				/* Extra fields */
				ex_offset_ctr = ex_offset;
		
				if ((mode & olist_detail_t.OLIST_PRICE) != 0)
				{
					throw new NotImplementedException();
					//int price = price_item(o_ptr, true, o_ptr.number);
					//strnfmt(tmp_val, sizeof(tmp_val), "%6d au", price);
					//put_str(tmp_val, row + i, col + ex_offset_ctr);
					//ex_offset_ctr += 9;
				}

				if ((mode & olist_detail_t.OLIST_FAIL) != 0)
				{
					throw new NotImplementedException();
					//int fail = (9 + get_use_device_chance(o_ptr)) / 10;
					//if (object_effect_is_known(o_ptr))
					//    strnfmt(tmp_val, sizeof(tmp_val), "%4d%% fail", fail);
					//else
					//    my_strcpy(tmp_val, "    ? fail", sizeof(tmp_val));
					//put_str(tmp_val, row + i, col + ex_offset_ctr);
					//ex_offset_ctr += 10;
				}

				if ((mode & olist_detail_t.OLIST_WEIGHT) != 0)
				{
					int weight = o_ptr.weight * o_ptr.number;
					//tmp_val = String.Format("%4d.%1d lb", weight / 10, weight % 10);
					tmp_val = String.Format("{0}.{1} lb", weight / 10, weight % 10);
					Utilities.put_str(tmp_val, row + i, col + ex_offset_ctr);
					ex_offset_ctr += 9;
				}
			}

			/* For the inventory: print the quiver count */
			if ((mode & olist_detail_t.OLIST_QUIVER) != 0)
			{
				int count, j;

				/* Quiver may take multiple lines */
				for(j = 0; j < Misc.p_ptr.quiver_slots; j++, i++)
				{
				    string fmt = "in Quiver: {0} missile{1}";
				    char letter = index_to_label(in_term ? i - 1 : i);

				    /* Number of missiles in this "slot" */
				    if (j == Misc.p_ptr.quiver_slots - 1 && Misc.p_ptr.quiver_remainder > 0)
				        count = Misc.p_ptr.quiver_remainder;
				    else
				        count = 99;

				    /* Clear the line */
				    Utilities.prt("", row + i, Math.Max(col - 2, 0));

				    /* Print the (disabled) label */
				    tmp_val = String.Format("{0}) ", letter);
				    Utilities.c_put_str(ConsoleColor.Gray, tmp_val, row + i, col);

				    /* Print the count */
				    tmp_val = String.Format(fmt, count, count == 1 ? "" : "s");
				    Utilities.c_put_str(ConsoleColor.DarkYellow, tmp_val, row + i, col + 3); //Umber
				}
			}

			/* Clear term windows */
			if (in_term)
			{
				for (; i < Term.instance.hgt; i++)
				{
					Utilities.prt("", row + i, Math.Max(col - 2, 0));
				}
			}
	
			/* Print a drop shadow for the main window if necessary */
			else if (i > 0 && row + i < 24)
			{
				Utilities.prt("", row + i, Math.Max(col - 2, 0));
			}
		}

		/*
		 * Display the equipment.  Builds a list of objects and passes them
		 * off to show_obj_list() for display.  Mode flags documented in
		 * object.h
		 */
		public static void show_equip(olist_detail_t mode)
		{
			throw new NotImplementedException();
		   // int i, last_slot = 0;

		   // object_type *o_ptr;

		   //int num_obj = 0;
		   //char labels[50][80];
		   //object_type *objects[50];

		   // char tmp_val[80];

		   //bool in_term = (mode & OLIST_WINDOW) ? true : false;

		   // /* Find the last equipment slot to display */
		   // for (i = INVEN_WIELD; i < ALL_INVEN_TOTAL; i++)
		   // {
		   //     o_ptr = &p_ptr.inventory[i];
		   //     if (i < INVEN_TOTAL || o_ptr.kind) last_slot = i;
		   // }

		   // /* Build the object list */
		   // for (i = INVEN_WIELD; i <= last_slot; i++)
		   // {
		   //     o_ptr = &p_ptr.inventory[i];

		   //     /* May need a blank line to separate the quiver */
		   //     if (i == INVEN_TOTAL)
		   //     {
		   //         int j;
		   //         bool need_spacer = false;
			
		   //         /* Scan the rest of the items for acceptable entries */
		   //         for (j = i; j < last_slot; j++)
		   //         {
		   //             o_ptr = &p_ptr.inventory[j];
		   //             if (item_tester_okay(o_ptr)) need_spacer = true;
		   //         }

		   //         /* Add a spacer between equipment and quiver */
		   //         if (num_obj > 0 && need_spacer)
		   //         {
		   //             my_strcpy(labels[num_obj], "", sizeof(labels[num_obj]));
		   //             objects[num_obj] = null;
		   //             num_obj++;
		   //         }

		   //         continue;
		   //     }

		   //     /* Acceptable items get a label */
		   //     if (item_tester_okay(o_ptr))
		   //         strnfmt(labels[num_obj], sizeof(labels[num_obj]), "%c) ", index_to_label(i));

		   //     /* Unacceptable items are still displayed in term windows */
		   //     else if (in_term)
		   //         my_strcpy(labels[num_obj], "   ", sizeof(labels[num_obj]));

		   //     /* Unacceptable items are skipped in the main window */
		   //     else continue;

		   //     /* Show full slot labels */
		   //     strnfmt(tmp_val, sizeof(tmp_val), "%-14s: ", mention_use(i));
		   //     my_strcat(labels[num_obj], tmp_val, sizeof(labels[num_obj]));

		   //     /* Save the object */
		   //     objects[num_obj] = o_ptr;
		   //     num_obj++;
		   // }

		   // /* Display the object list */
		   // show_obj_list(num_obj, 0, labels, objects, mode);
		}

		/*
		 * Display the floor.  Builds a list of objects and passes them
		 * off to show_obj_list() for display.  Mode flags documented in
		 * object.h
		 */
		public static void show_floor(int[] floor_list, int floor_num, olist_detail_t mode)
		{
			throw new NotImplementedException();
		   // int i;

		   // object_type *o_ptr;

		   //int num_obj = 0;
		   //char labels[50][80];
		   //object_type *objects[50];

		   // if (floor_num > MAX_FLOOR_STACK) floor_num = MAX_FLOOR_STACK;

		   // /* Build the object list */
		   // for (i = 0; i < floor_num; i++)
		   // {
		   //     o_ptr = object_byid(floor_list[i]);

		   //     /* Tester always skips gold. When gold should be displayed,
		   //      * only test items that are not gold.
		   //      */
		   //     if ((o_ptr.tval != TV_GOLD || !(mode & OLIST_GOLD)) &&
		   //         !item_tester_okay(o_ptr))
		   //         continue;

		   //     strnfmt(labels[num_obj], sizeof(labels[num_obj]),
		   //             "%c) ", index_to_label(i));

		   //     /* Save the object */
		   //     objects[num_obj] = o_ptr;
		   //     num_obj++;
		   // }

		   // /* Display the object list */
		   // show_obj_list(num_obj, 0, labels, objects, mode);
		}

		/*
		 * Hack -- allow user to "prevent" certain choices.
		 *
		 * The item can be negative to mean "item on floor".
		 */
		static bool get_item_allow(int item, char ch, bool is_harmless)
		{
			throw new NotImplementedException();
			//object_type *o_ptr;
			//char verify_inscrip[] = "!*";

			//unsigned n;

			///* Inventory or floor */
			//if (item >= 0)
			//    o_ptr = &p_ptr.inventory[item];
			//else
			//    o_ptr = object_byid(0 - item);

			///* Check for a "prevention" inscription */
			//verify_inscrip[1] = ch;

			///* Find both sets of inscriptions, add together, and prompt that number of times */
			//n = check_for_inscrip(o_ptr, verify_inscrip);

			//if (!is_harmless)
			//    n += check_for_inscrip(o_ptr, "!*");

			//while (n--)
			//{
			//    if (!verify_item("Really try", item))
			//        return (false);
			//}

			///* Allow it */
			//return (true);
		}

		/*
		 * Find the "first" inventory object with the given "tag".
		 *
		 * A "tag" is a char "n" appearing as "@n" anywhere in the
		 * inscription of an object.
		 *
		 * Also, the tag "@xn" will work as well, where "n" is a tag-char,
		 * and "x" is the action that tag will work for.
		 */
		static int get_tag(ref int cp, char tag, Command_Code cmd, bool quiver_tags)
		{
			throw new NotImplementedException();
			//int i;
			//const char *s;

			///* (f)ire is handled differently from all others, due to the quiver */
			//if (quiver_tags)
			//{
			//    i = QUIVER_START + tag - '0';
			//    if (p_ptr.inventory[i].kind)
			//    {
			//        *cp = i;
			//        return (true);
			//    }
			//    return (false);
			//}

			///* Check every object */
			//for (i = 0; i < ALL_INVEN_TOTAL; ++i)
			//{
			//    object_type *o_ptr = &p_ptr.inventory[i];

			//    /* Skip non-objects */
			//    if (!o_ptr.kind) continue;

			//    /* Skip empty inscriptions */
			//    if (!o_ptr.note) continue;

			//    /* Find a '@' */
			//    s = strchr(quark_str(o_ptr.note), '@');

			//    /* Process all tags */
			//    while (s)
			//    {
			//        /* Check the normal tags */
			//        if (s[1] == tag)
			//        {
			//            /* Save the actual inventory ID */
			//            *cp = i;

			//            /* Success */
			//            return (true);
			//        }

			//        /* Check the special tags */
			//        if ((cmd_lookup(s[1]) == cmd) && (s[2] == tag))
			//        {
			//            /* Save the actual inventory ID */
			//            *cp = i;

			//            /* Success */
			//            return (true);
			//        }

			//        /* Find another '@' */
			//        s = strchr(s + 1, '@');
			//    }
			//}

			///* No such tag */
			//return (false);
		}

		/*
		 * Verify the choice of an item.
		 *
		 * The item can be negative to mean "item on floor".
		 */
		public static bool verify_item(string prompt, int item)
		{
			throw new NotImplementedException();
			//char o_name[80];

			//char out_val[160];

			//object_type *o_ptr;

			///* Inventory */
			//if (item >= 0)
			//{
			//    o_ptr = &p_ptr.inventory[item];
			//}

			///* Floor */
			//else
			//{
			//    o_ptr = object_byid(0 - item);
			//}

			///* Describe */
			//object_desc(o_name, sizeof(o_name), o_ptr, ODESC_PREFIX | ODESC_FULL);

			///* Prompt */
			//strnfmt(out_val, sizeof(out_val), "%s %s? ", prompt, o_name);

			///* Query */
			//return (get_check(out_val));
		}

	}
}
