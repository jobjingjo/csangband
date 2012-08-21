using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;

namespace CSAngband {
	class TextUI {
		static Menu_Type knowledge_menu;
		static int[] obj_group_order = null;

		/* Standard menu orderings */
		public const string lower_case = "abcdefghijklmnopqrstuvwxyz";
		public const string upper_case = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public const string all_letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
		
		/*
		 * Display a list of command types, allowing the user to select one.
		 */
		static char action_menu_choose()
		{
			throw new NotImplementedException();
			//region area = { 21, 5, 37, 6 };

			//struct cmd_info chosen_command = { 0 };

			//if (!command_menu)
			//    command_menu = menu_new(MN_SKIN_SCROLL, &command_menu_iter);

			//menu_setpriv(command_menu, N_ELEMENTS(cmds_all) - 1, &chosen_command);
			//menu_layout(command_menu, &area);

			///* Set up the screen */
			//screen_save();
			//window_make(19, 4, 58, 11);

			//menu_select(command_menu, 0, true);

			//screen_load();

			//return chosen_command.key;
		}

				/*** Input processing ***/


		/**
		 * Get a command count, with the '0' key.
		 */
		static int get_count()
		{
			throw new NotImplementedException();
			//int count = 0;

			//while (1)
			//{
			//    struct keypress ke;

			//    prt(format("Count: %d", count), 0, 0);

			//    ke = inkey();
			//    if (ke.code == ESCAPE)
			//        return -1;

			//    /* Simple editing (delete or backspace) */
			//    else if (ke.code == 0x7F || ke.code == KTRL('H'))
			//        count = count / 10;

			//    /* Actual numeric data */
			//    else if (isdigit((unsigned char) ke.code))
			//    {
			//        count = count * 10 + D2I(ke.code);

			//        if (count >= 9999)
			//        {
			//            bell("Invalid repeat count!");
			//            count = 9999;
			//        }
			//    }

			//    /* Anything non-numeric passes straight to command input */
			//    else
			//    {
			//        /* XXX nasty hardcoding of action menu key */
			//        if (ke.code != '\n' && ke.code != '\r')
			//            Term_keypress(ke.code, ke.mods);

			//        break;
			//    }
			//}

			//return count;
		}



		/*
		 * Hack -- special buffer to hold the action of the current keymap
		 */
		static keypress[] request_command_buffer = new keypress[256];


		/*
		 * Request a command from the user.
		 *
		 * Note that "caret" ("^") is treated specially, and is used to
		 * allow manual input of control characters.  This can be used
		 * on many machines to request repeated tunneling (Ctrl-H) and
		 * on the Macintosh to request "Control-Caret".
		 *
		 * Note that "backslash" is treated specially, and is used to bypass any
		 * keymap entry for the following character.  This is useful for macros.
		 */
		static ui_event get_command()
		{
			int mode = (int)(Option.rogue_like_commands.value ? Keymap.Mode.ROGUE : Keymap.Mode.ORIG);

			keypress[] tmp = new keypress[2]{ new keypress(), new keypress()};

			ui_event ke = new ui_event();
			//ui_event ret = ke;

			keypress[] act = null;



			/* Get command */
			while (true)
			{
			    /* Hack -- no flush needed */
			    Term.msg_flag = false;

			    /* Activate "command mode" */
			    Utilities.inkey_flag = true;

			    /* Get a command */
			    ke = Utilities.inkey_ex();

			    if (ke.type == ui_event_type.EVT_KBRD) {
			        bool keymap_ok = true;
			        switch ((char)ke.key.code) {
			            case '0': {
			                int count = TextUI.get_count();

							throw new NotImplementedException();
							//if (count == -1 || !get_com_ex("Command: ", &ke))
							//    continue;
							//else
							//    p_ptr.command_arg = count;
							//break;
			            }

			            case '\\': {
			                /* Allow keymaps to be bypassed */
							throw new NotImplementedException();
							//(void)get_com_ex("Command: ", &ke);
							//keymap_ok = false;
							//break;
			            }

			            case '^': {
							throw new NotImplementedException();
							///* Allow "control chars" to be entered */
							//if (get_com("Control: ", &ke.key))
							//    ke.key.code = KTRL(ke.key.code);
							//break;
			            }
			        }

			        /* Find any relevant keymap */
					if(keymap_ok) {
						act = Keymap.find(mode, ke.key);
						//if (act == null) {
						//    ret = ke;
						//}
					} 
			    }

			    /* Erase the message line */
			    Utilities.prt("", 0, 0);

			    if (ke.type == ui_event_type.EVT_BUTTON)
			    {
			        /* Buttons are always specified in standard keyset */
			        act = tmp;
			        tmp[0] = ke.key;
			    }

			    /* Apply keymap if not inside a keymap already */
			    if (ke.key.code != (keycode_t)0 && act != null && Utilities.inkey_next == null)
			    {
					//int n = 0;
					//while (n < act.Length && act[n] != null)//act[n].type
					//    n++;

					///* Make room for the terminator */
					//n += 1;

			        /* Install the keymap */
			        for (int q = 0; q < act.Length; q++){ //It used to check for q < n instead
						request_command_buffer[q] = act[q];
						//memcpy(request_command_buffer, act, n);
					}

			        /* Start using the buffer */
			        Utilities.inkey_next = new List<keypress>(request_command_buffer);

			        /* Continue */
					//ret.type = ke.type;
					//ret.mouse = ke.mouse;
					//ret.key = act[0];
			        continue;
			    }

			    /* Done */
			    break;
			}

			return ke;
		}


		/**
		 * Handle a textui mouseclick.
		 */
		static void process_click(ui_event e)
		{
			throw new NotImplementedException();
		//    int x, y;

		//    if (!OPT(mouse_movement)) return;

		//    y = KEY_GRID_Y(e);
		//    x = KEY_GRID_X(e);

		//    /* Check for a valid location */
		//    if (!in_bounds_fully(y, x)) return;

		//    /* XXX show context menu here */
		//    if ((p_ptr.py == y) && (p_ptr.px == x))
		//        textui_cmd_rest();

		//    else /* if (e.mousebutton == 1) */
		//    {
		//        if (p_ptr.timed[TMD_CONFUSED])
		//        {
		//            cmd_insert(CMD_WALK);
		//        }
		//        else
		//        {
		//            cmd_insert(CMD_PATHFIND);
		//            cmd_set_arg_point(cmd_get_top(), 0, y, x);
		//        }
		//    }

		//#if 0
		//    else if (e.mousebutton == 2)
		//    {
		//        target_set_location(y, x);
		//        msg_print("Target set.");
		//    }
		//#endif
		}

		/**
		 * Process a textui keypress.
		 */
		static bool process_key(keypress kp)
		{
			Command_Info cmd;

			/* XXXmacro this needs rewriting */
			char c = (char)kp.code;

			if (c == '\n' || c == '\r')
			    c = TextUI.action_menu_choose();

			if (c == '\0' || c == (char)keycode_t.ESCAPE || c == ' ' || c == '\a')
			    return true;

			cmd = Command.converted_list[c];
			if (cmd == null) return false;

			if (Command.key_confirm_command(c) && (cmd.prereq == null || cmd.prereq())) {
			    if (cmd.hook != null)
			        cmd.hook();
			    else if (cmd.cmd != Command_Code.NULL)
					Game_Command.insert_repeated(cmd.cmd, Misc.p_ptr.command_arg);
			}

			return true;
		}


		/**
		 * Parse and execute the current command
		 * Give "Warning" on illegal commands.
		 */
		public static void process_command(bool no_request)
		{
			bool done = true;
			ui_event e;

			/* Reset argument before getting command */
			Misc.p_ptr.command_arg = 0;
			e = TextUI.get_command();

			if (e.type == ui_event_type.EVT_RESIZE)
			    Do_Command.redraw();

			else if (e.type == ui_event_type.EVT_MOUSE)
			    TextUI.process_click(e);

			else if (e.type == ui_event_type.EVT_KBRD)
			    done = TextUI.process_key(e.key);

			if (!done)
			    Do_Command.unknown();
		}

		/*
		 * Display known objects
		 */
		public static void browse_object_knowledge(string name, int row)
		{
			throw new NotImplementedException();
			//group_funcs kind_f = {TV_GOLD, false, kind_name, o_cmp_tval, obj2gid, 0};
			//member_funcs obj_f = {display_object, desc_obj_fake, o_xchar, o_xattr, o_xtra_prompt, o_xtra_act, 0};

			//int *objects;
			//int o_count = 0;
			//int i;
			//object_kind *kind;

			//objects = C_ZNEW(z_info.k_max, int);

			//for (i = 0; i < z_info.k_max; i++)
			//{
			//    kind = &k_info[i];
			//    /* It's in the list if we've ever seen it, or it has a flavour,
			//     * and either it's not one of the special artifacts, or if it is,
			//     * we're not aware of it yet. This way the flavour appears in the list
			//     * until it is found.
			//     */
			//    if ((kind.everseen || kind.flavor || OPT(cheat_xtra)) &&
			//            (!of_has(kind.flags, OF_INSTA_ART) ||
			//             !artifact_is_known(get_artifact_from_kind(kind))))
			//    {
			//        int c = obj_group_order[k_info[i].tval];
			//        if (c >= 0) objects[o_count++] = i;
			//    }
			//}

			//display_knowledge("known objects", objects, o_count, kind_f, obj_f, "Squelch  Inscribed          Sym");

			//FREE(objects);
		}

		/*
		 * Definition of the "player knowledge" menu.
		 */
		static Menu_Action[] knowledge_actions = new Menu_Action[]
		{
			new Menu_Action( 0, (char)0, "Display object knowledge",   			TextUI.browse_object_knowledge	),
			new Menu_Action( 0, (char)0, "Display artifact knowledge", 			Do_Command.knowledge_artifacts	),
			new Menu_Action( 0, (char)0, "Display ego item knowledge", 			Do_Command.knowledge_ego_items	),
			new Menu_Action( 0, (char)0, "Display monster knowledge",  			Do_Command.knowledge_monsters	),
			new Menu_Action( 0, (char)0, "Display feature knowledge",  			Do_Command.knowledge_features	),
			new Menu_Action( 0, (char)0, "Display contents of general store",	Do_Command.knowledge_store		),
			new Menu_Action( 0, (char)0, "Display contents of armourer",		Do_Command.knowledge_store		),
			new Menu_Action( 0, (char)0, "Display contents of weaponsmith",		Do_Command.knowledge_store		),
			new Menu_Action( 0, (char)0, "Display contents of temple",   		Do_Command.knowledge_store		),
			new Menu_Action( 0, (char)0, "Display contents of alchemist",		Do_Command.knowledge_store		),
			new Menu_Action( 0, (char)0, "Display contents of magic shop",		Do_Command.knowledge_store		),
			new Menu_Action( 0, (char)0, "Display contents of black market",	Do_Command.knowledge_store		),
			new Menu_Action( 0, (char)0, "Display contents of home",   			Do_Command.knowledge_store		),
			new Menu_Action( 0, (char)0, "Display hall of fame",       			Do_Command.knowledge_scores		),
			new Menu_Action( 0, (char)0, "Display character history",			Do_Command.knowledge_history	),
		};

		public static void knowledge_init()
		{
			/* Initialize the menus */
			knowledge_menu = new Menu_Type(Menu_Type.skin_id.SCROLL, Menu_Type.menu_find_iter(Menu_Type.menu_iter_id.ACTIONS));
			knowledge_menu.priv(knowledge_actions.Length, knowledge_actions);

			knowledge_menu.title = "Display current knowledge";
			knowledge_menu.selections = lower_case;

			/* initialize other static variables */
			if (obj_group_order == null)
			{
			    int i;
			    int gid = -1;

			    obj_group_order = new int[Object.TVal.TV_GOLD + 1];
			    Utilities.atexit(cleanup_cmds);

			    /* Allow for missing values */
			    for (i = 0; i <= Object.TVal.TV_GOLD; i++)
			        obj_group_order[i] = -1;

			    for (i = 0; 0 != object_text_order[i].tval; i++)
			    {
			        if (object_text_order[i].name != null) gid = i;
			        obj_group_order[object_text_order[i].tval] = gid;
			    }
			}
		}




		/*
		 * Display the "player knowledge" menu.
		 */
		public static void browse_knowledge()
		{
			throw new NotImplementedException();
			//int i;
			//region knowledge_region = { 0, 0, -1, 18 };

			///* Grey out menu items that won't display anything */
			//if (collect_known_artifacts(null, 0) > 0)
			//    knowledge_actions[1].flags = 0;
			//else
			//    knowledge_actions[1].flags = MN_ACT_GRAYED;

			//knowledge_actions[2].flags = MN_ACT_GRAYED;
			//for (i = 0; i < z_info.e_max; i++)
			//{
			//    if (e_info[i].everseen || OPT(cheat_xtra))
			//    {
			//        knowledge_actions[2].flags = 0;
			//        break;
			//    }
			//}

			//if (count_known_monsters() > 0)
			//    knowledge_actions[3].flags = 0;
			//else
			//    knowledge_actions[3].flags = MN_ACT_GRAYED;

			//screen_save();
			//menu_layout(&knowledge_menu, &knowledge_region);

			//clear_from(0);
			//menu_select(&knowledge_menu, 0, false);

			//screen_load();
		}

		public enum CMD_Destroy_Return_Values
		{
			IGNORE_THIS_ITEM,
			UNIGNORE_THIS_ITEM,
			IGNORE_THIS_FLAVOR,
			UNIGNORE_THIS_FLAVOR,
			IGNORE_THIS_QUALITY
		};

		public static /*CMD_Destroy_Return_Values*/ void cmd_destroy()
		{
			throw new NotImplementedException();
			//int item;
			//object_type *o_ptr;

			//char out_val[160];

			//menu_type *m;
			//region r;
			//int selected;

			///* Get an item */
			//const char *q = "Ignore which item? ";
			//const char *s = "You have nothing to ignore.";
			//if (!get_item(&item, q, s, CMD_DESTROY, USE_INVEN | USE_EQUIP | USE_FLOOR))
			//    return;

			//o_ptr = object_from_item_idx(item);

			//m = menu_dynamic_new();
			//m.selections = lower_case;

			///* Basic ignore option */
			//if (!o_ptr.ignore) {
			//    menu_dynamic_add(m, "This item only", IGNORE_THIS_ITEM);
			//} else {
			//    menu_dynamic_add(m, "Unignore this item", UNIGNORE_THIS_ITEM);
			//}

			///* Flavour-aware squelch */
			//if (squelch_tval(o_ptr.tval) &&
			//        (!o_ptr.artifact || !object_flavor_is_aware(o_ptr))) {
			//    bool squelched = kind_is_squelched_aware(o_ptr.kind) ||
			//            kind_is_squelched_unaware(o_ptr.kind);

			//    char tmp[70];
			//    object_desc(tmp, sizeof(tmp), o_ptr, ODESC_BASE | ODESC_PLURAL);
			//    if (!squelched) {
			//        strnfmt(out_val, sizeof out_val, "All %s", tmp);
			//        menu_dynamic_add(m, out_val, IGNORE_THIS_FLAVOR);
			//    } else {
			//        strnfmt(out_val, sizeof out_val, "Unignore all %s", tmp);
			//        menu_dynamic_add(m, out_val, UNIGNORE_THIS_FLAVOR);
			//    }
			//}

			///* Quality squelching */
			//if (object_was_sensed(o_ptr) || object_was_worn(o_ptr) ||
			//        object_is_known_not_artifact(o_ptr)) {
			//    byte value = squelch_level_of(o_ptr);
			//    int type = squelch_type_of(o_ptr);

			//    if (object_is_jewelry(o_ptr) &&
			//                squelch_level_of(o_ptr) != SQUELCH_BAD)
			//        value = SQUELCH_MAX;

			//    if (value != SQUELCH_MAX && type != TYPE_MAX) {
			//        strnfmt(out_val, sizeof out_val, "All %s %s",
			//                quality_values[value].name, quality_choices[type].name);

			//        menu_dynamic_add(m, out_val, IGNORE_THIS_QUALITY);
			//    }
			//}

			///* work out display region */
			//r.width = menu_dynamic_longest_entry(m) + 3 + 2; /* +3 for tag, 2 for pad */
			//r.col = 80 - r.width;
			//r.row = 1;
			//r.page_rows = m.count;

			//screen_save();
			//menu_layout(m, &r);
			//region_erase_bordered(&r);

			//prt("(Enter to select, ESC) Ignore:", 0, 0);
			//selected = menu_dynamic_select(m);

			//screen_load();

			//if (selected == IGNORE_THIS_ITEM) {
			//    cmd_insert(CMD_DESTROY);
			//    cmd_set_arg_item(cmd_get_top(), 0, item);
			//} else if (selected == UNIGNORE_THIS_ITEM) {
			//    o_ptr.ignore = false;
			//} else if (selected == IGNORE_THIS_FLAVOR) {
			//    object_squelch_flavor_of(o_ptr);
			//} else if (selected == UNIGNORE_THIS_FLAVOR) {
			//    kind_squelch_clear(o_ptr.kind);
			//} else if (selected == IGNORE_THIS_QUALITY) {
			//    byte value = squelch_level_of(o_ptr);
			//    int type = squelch_type_of(o_ptr);

			//    squelch_level[type] = value;
			//}

			//p_ptr.notice |= PN_SQUELCH;

			//menu_dynamic_free(m);
		}

		public static void cmd_toggle_ignore()
		{
			throw new NotImplementedException();
			//p_ptr.unignoring = !p_ptr.unignoring;
			//p_ptr.notice |= PN_SQUELCH;
			//do_cmd_redraw();
		}

		/**
		 * Front-end 'throw' command.
		 */
		public static void cmd_throw() {
			throw new NotImplementedException();
			//int item, dir;
			//const char *q, *s;

			///* Get an item */
			//q = "Throw which item? ";
			//s = "You have nothing to throw.";
			//if (!get_item(&item, q, s, CMD_THROW, (USE_EQUIP | USE_INVEN | USE_FLOOR))) return;

			//if (item >= INVEN_WIELD && item < QUIVER_START) {
			//    msg("You cannot throw wielded items.");
			//    return;
			//}

			///* Get a direction (or cancel) */
			//if (!get_aim_dir(&dir)) return;

			//cmd_insert(CMD_THROW);
			//cmd_set_arg_item(cmd_get_top(), 0, item);
			//cmd_set_arg_target(cmd_get_top(), 1, dir);
		}


		/**
		 * Front-end command which fires at the nearest target with default ammo.
		 */
		public static void cmd_fire_at_nearest() {
			throw new NotImplementedException();
			///* the direction '5' means 'use the target' */
			//int i, dir = 5, item = -1;

			///* Require a usable launcher */
			//if (!p_ptr.inventory[INVEN_BOW].tval || !p_ptr.state.ammo_tval) {
			//    msg("You have nothing to fire with.");
			//    return;
			//}

			///* Find first eligible ammo in the quiver */
			//for (i = QUIVER_START; i < QUIVER_END; i++) {
			//    if (p_ptr.inventory[i].tval != p_ptr.state.ammo_tval) continue;
			//    item = i;
			//    break;
			//}

			///* Require usable ammo */
			//if (item < 0) {
			//    msg("You have no ammunition in the quiver to fire");
			//    return;
			//}

			///* Require foe */
			//if (!target_set_closest(TARGET_KILL | TARGET_QUIET)) return;

			///* Fire! */
			//cmd_insert(CMD_FIRE);
			//cmd_set_arg_item(cmd_get_top(), 0, item);
			//cmd_set_arg_target(cmd_get_top(), 1, dir);
		}

		public static void cmd_suicide()
		{
			throw new NotImplementedException();
			///* Flush input */
			//flush();

			///* Verify Retirement */
			//if (p_ptr.total_winner)
			//{
			//    /* Verify */
			//    if (!get_check("Do you want to retire? ")) return;
			//}

			///* Verify Suicide */
			//else
			//{
			//    struct keypress ch;

			//    /* Verify */
			//    if (!get_check("Do you really want to commit suicide? ")) return;

			//    /* Special Verification for suicide */
			//    prt("Please verify SUICIDE by typing the '@' sign: ", 0, 0);
			//    flush();
			//    ch = inkey();
			//    prt("", 0, 0);
			//    if (ch.code != '@') return;
			//}

			//cmd_insert(CMD_SUICIDE);
		}

		public static void cmd_rest()
		{
			throw new NotImplementedException();
			///* Prompt for time if needed */
			//if (p_ptr.command_arg <= 0)
			//{
			//    const char *p = "Rest (0-9999, '!' for HP or SP, '*' for HP and SP, '&' as needed): ";

			//    char out_val[5] = "& ";

			//    /* Ask for duration */
			//    if (!get_string(p, out_val, sizeof(out_val))) return;

			//    /* Rest until done */
			//    if (out_val[0] == '&')
			//    {
			//        cmd_insert(CMD_REST);
			//        cmd_set_arg_choice(cmd_get_top(), 0, REST_COMPLETE);
			//    }

			//    /* Rest a lot */
			//    else if (out_val[0] == '*')
			//    {
			//        cmd_insert(CMD_REST);
			//        cmd_set_arg_choice(cmd_get_top(), 0, REST_ALL_POINTS);
			//    }

			//    /* Rest until HP or SP filled */
			//    else if (out_val[0] == '!')
			//    {
			//        cmd_insert(CMD_REST);
			//        cmd_set_arg_choice(cmd_get_top(), 0, REST_SOME_POINTS);
			//    }
		
			//    /* Rest some */
			//    else
			//    {
			//        int turns = atoi(out_val);
			//        if (turns <= 0) return;
			//        if (turns > 9999) turns = 9999;
			
			//        cmd_insert(CMD_REST);
			//        cmd_set_arg_choice(cmd_get_top(), 0, turns);
			//    }
			//}
		}

		/**
		 * Browse a given book.
		 */
		public static void book_browse(Object.Object o_ptr)
		{
			throw new NotImplementedException();
			//menu_type *m;
			//const char *noun = (p_ptr.class.spell_book == TV_MAGIC_BOOK ?
			//        "spell" : "prayer");

			//m = spell_menu_new(o_ptr, spell_okay_to_browse);
			//if (m) {
			//    spell_menu_browse(m, noun);
			//    spell_menu_destroy(m);
			//} else {
			//    msg("You cannot browse that.");
			//}
		}

		/**
		 * Browse the given book.
		 */
		public static void spell_browse()
		{
			throw new NotImplementedException();
			//int item;

			//item_tester_hook = obj_can_browse;
			//if (!get_item(&item, "Browse which book? ",
			//        "You have no books that you can read.",
			//        CMD_BROWSE_SPELL, (USE_INVEN | USE_FLOOR | IS_HARMLESS)))
			//    return;

			///* Track the object kind */
			//track_object(item);
			//handle_stuff(p_ptr);

			//textui_book_browse(object_from_item_idx(item));
		}

		/**
		 * Study a book to gain a new spell
		 */
		public static void obj_study()
		{
			throw new NotImplementedException();
			//int item;

			//item_tester_hook = obj_can_study;
			//if (!get_item(&item, "Study which book? ",
			//        "You have no books that you can read.",
			//        CMD_STUDY_BOOK, (USE_INVEN | USE_FLOOR)))
			//    return;

			//track_object(item);
			//handle_stuff(p_ptr);

			//if (player_has(PF_CHOOSE_SPELLS)) {
			//    int spell = get_spell(object_from_item_idx(item),
			//            "study", spell_okay_to_study);
			//    if (spell >= 0) {
			//        cmd_insert(CMD_STUDY_SPELL);
			//        cmd_set_arg_choice(cmd_get_top(), 0, spell);
			//    }
			//} else {
			//    cmd_insert(CMD_STUDY_BOOK);
			//    cmd_set_arg_item(cmd_get_top(), 0, item);
			//}
		}

		/**
		 * Cast a spell from a book.
		 */
		public static void obj_cast()
		{
			throw new NotImplementedException();
		//    int item;
		//    int spell;

		//    const char *verb = ((p_ptr.class.spell_book == TV_MAGIC_BOOK) ? "cast" : "recite");

		//    item_tester_hook = obj_can_cast_from;
		//    if (!get_item(&item, "Cast from which book? ",
		//            "You have no books that you can read.",
		//            CMD_CAST, (USE_INVEN | USE_FLOOR)))
		//        return;

		//    /* Track the object kind */
		//    track_object(item);

		//    /* Ask for a spell */
		//    spell = get_spell(object_from_item_idx(item), verb, spell_okay_to_cast);
		//    if (spell >= 0) {
		//        cmd_insert(CMD_CAST);
		//        cmd_set_arg_choice(cmd_get_top(), 0, spell);
		//    }
		}

		/* Command3.c */
		/* Examine an object */
		public static void obj_examine() {
			throw new NotImplementedException();
			/*
			char header[120];

			textblock *tb;
			region area = { 0, 0, 0, 0 };

			object_type *o_ptr;
			int item;

			// Select item 
			if (!get_item(&item, "Examine which item?", "You have nothing to examine.",
					CMD_null, (USE_EQUIP | USE_INVEN | USE_FLOOR | IS_HARMLESS)))
				return;

			// Track object for object recall
			track_object(item);

			// Display info
			o_ptr = object_from_item_idx(item);
			tb = object_info(o_ptr, OINFO_NONE);
			object_desc(header, sizeof(header), o_ptr, ODESC_PREFIX | ODESC_FULL);

			textui_textblock_show(tb, area, format("%^s", header));
			textblock_free(tb);*/
		}

		public static int get_cmd(cmd_context context, bool wait)
		{
			if (context == cmd_context.CMD_BIRTH)
			    return UIBirth.get_birth_command(wait);
			else if (context == cmd_context.CMD_GAME)
			    TextUI.process_command(!wait);

			/* If we've reached here, we haven't got a command. */
			return 1;
		}

		/* Keep macro counts happy. */
		static void cleanup_cmds() {
			throw new NotImplementedException();
			//mem_free(obj_group_order);
		}

		static Grouper[] object_text_order =
		{
			new Grouper(TVal.TV_RING,			"Ring"			),
			new Grouper(TVal.TV_AMULET,			"Amulet"		),
			new Grouper(TVal.TV_POTION,			"Potion"		),
			new Grouper(TVal.TV_SCROLL,			"Scroll"		),
			new Grouper(TVal.TV_WAND,			"Wand"			),
			new Grouper(TVal.TV_STAFF,			"Staff"			),
			new Grouper(TVal.TV_ROD,			"Rod"			),
			new Grouper(TVal.TV_FOOD,			"Food"			),
			new Grouper(TVal.TV_PRAYER_BOOK,	"Priest Book"	),
			new Grouper(TVal.TV_MAGIC_BOOK,		"Magic Book"	),
			new Grouper(TVal.TV_LIGHT,			"Light"			),
			new Grouper(TVal.TV_FLASK,			"Flask"			),
			new Grouper(TVal.TV_SWORD,			"Sword"			),
			new Grouper(TVal.TV_POLEARM,		"Polearm"		),
			new Grouper(TVal.TV_HAFTED,			"Hafted Weapon" ),
			new Grouper(TVal.TV_BOW,			"Bow"			),
			new Grouper(TVal.TV_ARROW,			"Ammunition"	),
			new Grouper(TVal.TV_BOLT,			null			),
			new Grouper(TVal.TV_SHOT,			null			),
			new Grouper(TVal.TV_SHIELD,			"Shield"		),
			new Grouper(TVal.TV_CROWN,			"Crown"			),
			new Grouper(TVal.TV_HELM,			"Helm"			),
			new Grouper(TVal.TV_GLOVES,			"Gloves"		),
			new Grouper(TVal.TV_BOOTS,			"Boots"			),
			new Grouper(TVal.TV_CLOAK,			"Cloak"			),
			new Grouper(TVal.TV_DRAG_ARMOR,		"Dragon Scale Mail" ),
			new Grouper(TVal.TV_HARD_ARMOR,		"Hard Armor"	),
			new Grouper(TVal.TV_SOFT_ARMOR,		"Soft Armor"	),
			new Grouper(TVal.TV_SPIKE,			"Spike"			),
			new Grouper(TVal.TV_DIGGING,		"Digger"		),
			new Grouper(TVal.TV_JUNK,			"Junk"			),
			new Grouper(0,						null			)
		};
	}
}
