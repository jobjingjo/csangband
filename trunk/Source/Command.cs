using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Command {
		/* cmd0.c */

		public static Menu_Type command_menu;
		public static Menu_Type.menu_iter command_menu_iter = new Menu_Type.menu_iter(
			null,
			null,
			cmd_list_entry,
			cmd_list_action,
			null
		);

		static bool cmd_list_action(Menu_Type m, ui_event mevent, int oid)
		{
			if (mevent.type == ui_event_type.EVT_SELECT)
			    return cmd_menu(ref Command_List.all[oid], m.menu_data);
			else
			    return false;
		}

		/*
		 * Display a list of commands.
		 */
		static bool cmd_menu(ref Command_List list, object selection_p)
		{
			Menu_Type menu;
			Menu_Type.menu_iter commands_menu = new Menu_Type.menu_iter( null, null, cmd_sub_entry, null, null );
			Region area = new Region(23, 4, 37, 13);

			ui_event evt;
			//Command_Info selection = selection_p as Command_Info;

			/* Set up the menu */
			menu = new Menu_Type(Menu_Type.skin_id.SCROLL, commands_menu);
			menu.priv(list.list.Length, list.list);
			menu.layout(area);

			/* Set up the screen */
			Utilities.screen_save();
			Utilities.window_make(21, 3, 62, 17);

			/* Select an entry */
			evt = menu.select(0, true);

			/* Load de screen */
			Utilities.screen_load();

			if (evt.type == ui_event_type.EVT_SELECT)
				selection_p = list.list[menu.cursor]; //This was originally selection as above

			return false;
		}

		/* Display an entry on a command menu */
		static void cmd_sub_entry(Menu_Type menu, int oid, bool cursor, int row, int col, int width)
		{
			ConsoleColor attr = (cursor ? ConsoleColor.Cyan : ConsoleColor.White);
			Command_Info[] commands = menu.menu_data as Command_Info[];

			//(void)width;

			/* Write the description */
			Term.putstr(col, row, -1, attr, commands[oid].desc);

			/* Include keypress */
			Term.addch(attr, ' ');
			Term.addch(attr, '(');

			/* KTRL()ing a control character does not alter it at all */
			if (UIEvent.KTRL(commands[oid].key) == commands[oid].key)
			{
			    Term.addch(attr, '^');
			    Term.addch(attr, UIEvent.UN_KTRL((keycode_t)commands[oid].key));
			}
			else
			{
			    Term.addch(attr, commands[oid].key);
			}

			Term.addch(attr, ')');
		}


		static void cmd_list_entry(Menu_Type menu, int oid, bool cursor, int row, int col, int width)
		{
			ConsoleColor attr = (cursor ? ConsoleColor.Cyan : ConsoleColor.White);
			Term.putstr(col, row, -1, attr, Command_List.all[oid].name);
		}

		/* List indexed by char */
		public static Command_Info[] converted_list = new Command_Info[Char.MaxValue+1];
		
		/*
		 * Initialise the command list.
		 */
		public static void Init()
		{
			/*for(int i = 0; i < converted_list.Length; i++) {
				converted_list[i] = new Command_Info();
			}*/

			// Go through all generic commands
			for (int j = 0; j < Command_List.all.Length; j++)
			{
				Command_Info[] commands = Command_List.all[j].list;

				// Fill everything in 
				for(int i = 0; i < Command_List.all[j].list.Length; i++) {
					converted_list[commands[i].key] = commands[i];
				}
			}
		}

		public static char lookup_key(Command_Code lookup_cmd)
		{
			for (int i = 0; i < converted_list.Length; i++) {
				Command_Info cmd = converted_list[i];

				if (cmd != null && cmd.cmd == lookup_cmd)
					return cmd.key;
			}

			return '\0';
		}

		public static Command_Code lookup(char key)
		{
			throw new NotImplementedException();
			/*
			if (!converted_list[key])
				return CMD_null;

			return converted_list[key].cmd;*/
		}


		//TODO: Find a better place for the feeling stuff
		/*
		 * Array of feeling strings for object feelings.
		 * Keep strings at 36 or less characters to keep the
		 * combined feeling on one row.
		 */
		static string [] obj_feeling_text=
		{
			"Looks like any other level.",
			"you sense an item of wondrous power!",
			"there are superb treasures here.",
			"there are excellent treasures here.",
			"there are very good treasures here.",
			"there are good treasures here.",
			"there may be something worthwhile here.",
			"there may not be much interesting here.",
			"there aren't many treasures here.",
			"there are only scraps of junk here.",
			"there are naught but cobwebs here."
		};

		/*
		 * Array of feeling strings for monster feelings.
		 * Keep strings at 36 or less characters to keep the
		 * combined feeling on one row.
		 */
		static string[] mon_feeling_text=
		{
			/* first string is just a place holder to 
			 * maintain symmetry with obj_feeling.
			 */
			"You are still uncertain about this place",
			"Omens of death haunt this place",
			"This place seems murderous",
			"This place seems terribly dangerous",
			"You feel anxious about this place",
			"You feel nervous about this place",
			"This place does not seem too risky",
			"This place seems reasonably safe",
			"This seems a tame, sheltered place",
			"This seems a quiet, peaceful place"
		};

		/*
		 * Display the feeling.  Players always get a monster feeling.
		 * Object feelings are delayed until the player has explored some
		 * of the level.
		 */

		public static void display_feeling(bool obj_only)
		{
			int obj_feeling = Cave.cave.feeling / 10;
			int mon_feeling = Cave.cave.feeling - (10 * obj_feeling);
			//string join;

			/* Don't show feelings for cold-hearted characters */
			if (Option.birth_no_feelings.value) return;

			/* No useful feeling in town */
			if (Misc.p_ptr.depth == 0) {
				Utilities.msg("Looks like a typical town.");
				return;
			}

			throw new NotImplementedException();
	
			///* Display only the object feeling when it's first discovered. */
			//if (obj_only){
			//    msg("You feel that %s", obj_feeling_text[obj_feeling]);
			//    return;
			//}
	
			///* Players automatically get a monster feeling. */
			//if (cave.feeling_squares < FEELING1){
			//    msg("%s.", mon_feeling_text[mon_feeling]);
			//    return;
			//}
	
			///* Verify the feelings */
			//if (obj_feeling >= N_ELEMENTS(obj_feeling_text))
			//    obj_feeling = N_ELEMENTS(obj_feeling_text) - 1;

			//if (mon_feeling >= N_ELEMENTS(mon_feeling_text))
			//    mon_feeling = N_ELEMENTS(mon_feeling_text) - 1;

			///* Decide the conjunction */
			//if ((mon_feeling <= 5 && obj_feeling > 6) ||
			//        (mon_feeling > 5 && obj_feeling <= 6))
			//    join = ", yet";
			//else
			//    join = ", and";

			///* Display the feeling */
			//msg("%s%s %s", mon_feeling_text[mon_feeling], join,
			//    obj_feeling_text[obj_feeling]);
		}

		/**
		 * Check no currently worn items are stopping the action 'c'
		 */
		public static bool key_confirm_command(char c)
		{
			int i;

			/* Hack -- Scan equipment */
			for (i = Misc.INVEN_WIELD; i < Misc.INVEN_TOTAL; i++)
			{
			    string verify_inscrip = "^" + c;
			    int n;

			    Object.Object o_ptr = Misc.p_ptr.inventory[i];
			    if (o_ptr.kind == null) continue;

			    /* Set up string to look for, e.g. "^d" */
			    //verify_inscrip[1] = c;

			    /* Verify command */
			    n = (o_ptr.check_for_inscrip("^*")?1:0) + (o_ptr.check_for_inscrip(verify_inscrip)?1:0);
			    while (n-- != 0)
			    {
			        if (!Utilities.get_check("Are you sure? "))
			            return false;
			    }
			}

			return true;
		}

		/*
		 * Move player in the given direction.
		 *
		 * This routine should only be called when energy has been expended.
		 *
		 * Note that this routine handles monsters in the destination grid,
		 * and also handles attempting to move into walls/doors/rubble/etc.
		 */
		public static void move_player(int dir, bool disarm)
		{
			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			int y = py + Misc.ddy[dir];
			int x = px + Misc.ddx[dir];

			int m_idx = Cave.cave.m_idx[y][x];

			/* Attack monsters */
			if (m_idx > 0) {
			    /* Mimics surprise the player */
			    if (Monster.Monster.is_mimicking(m_idx)) {
					throw new NotImplementedException();
					//become_aware(m_idx);

					///* Mimic wakes up */
					//mon_clear_timed(m_idx, MON_TMD_SLEEP, MON_TMD_FLG_NOMESSAGE, false);

			    } else {
			        Attack.py_attack(y, x);
			    }
			}

			/* Optionally alter traps/doors on movement */
			else if (disarm && (Cave.cave.info[y][x] & Cave.CAVE_MARK) != 0 &&
			        (Cave.cave_isknowntrap(Cave.cave, y, x) ||
			        Cave.cave_iscloseddoor(Cave.cave, y, x)))
			{
				throw new NotImplementedException();
				///* Auto-repeat if not already repeating */
				//if (cmd_get_nrepeats() == 0)
				//    cmd_set_repeat(99);

				//do_cmd_alter_aux(dir);
			}

			/* Cannot walk through walls */
			else if (!Cave.cave_floor_bold(y, x))
			{
			    /* Disturb the player */
			    Cave.disturb(Misc.p_ptr, 0, 0);

			    /* Notice unknown obstacles */
			    if ((Cave.cave.info[y][x] & Cave.CAVE_MARK) == 0)
			    {
			        /* Rubble */
			        if (Cave.cave.feat[y][x] == Cave.FEAT_RUBBLE)
			        {
			            Utilities.msgt(Message_Type.MSG_HITWALL, "You feel a pile of rubble blocking your way.");
			            Cave.cave.info[y][x] |= (Cave.CAVE_MARK);
			            Cave.cave_light_spot(Cave.cave, y, x);
			        }

			        /* Closed door */
			        else if (Cave.cave.feat[y][x] < Cave.FEAT_SECRET)
			        {
			            Utilities.msgt(Message_Type.MSG_HITWALL, "You feel a door blocking your way.");
			            Cave.cave.info[y][x] |= (Cave.CAVE_MARK);
			            Cave.cave_light_spot(Cave.cave, y, x);
			        }

			        /* Wall (or secret door) */
			        else
			        {
			            Utilities.msgt(Message_Type.MSG_HITWALL, "You feel a wall blocking your way.");
			            Cave.cave.info[y][x] |= (Cave.CAVE_MARK);
			            Cave.cave_light_spot(Cave.cave, y, x);
			        }
			    }

			    /* Mention known obstacles */
			    else
			    {
			        if (Cave.cave.feat[y][x] == Cave.FEAT_RUBBLE)
			            Utilities.msgt(Message_Type.MSG_HITWALL, "There is a pile of rubble blocking your way.");
			        else if (Cave.cave.feat[y][x] < Cave.FEAT_SECRET)
			            Utilities.msgt(Message_Type.MSG_HITWALL, "There is a door blocking your way.");
			        else
			            Utilities.msgt(Message_Type.MSG_HITWALL, "There is a wall blocking your way.");
			    }
			}

			/* Normal movement */
			else
			{
			    /* See if trap detection status will change */
			    bool old_dtrap = ((Cave.cave.info2[py][px] & (Cave.CAVE2_DTRAP)) != 0);
			    bool new_dtrap = ((Cave.cave.info2[y][x] & (Cave.CAVE2_DTRAP)) != 0);

			    /* Note the change in the detect status */
			    if (old_dtrap != new_dtrap)
			        Misc.p_ptr.redraw |= (Misc.PR_DTRAP);

			    /* Disturb player if the player is about to leave the area */
			    if (Option.disturb_detect.value && Misc.p_ptr.running != 0 && 
			        !Misc.p_ptr.running_firststep && old_dtrap && !new_dtrap)
			    {
			        Cave.disturb(Misc.p_ptr, 0, 0);
			        return;
			    }

			    /* Move player */
				Monster.Monster.monster_swap(py, px, y, x);
  
				/* New location */
				y = py = Misc.p_ptr.py;
				x = px = Misc.p_ptr.px;

				/* Searching */
				if(Misc.p_ptr.searching != 0 || (Misc.p_ptr.state.skills[(int)Skill.SEARCH_FREQUENCY] >= 50) ||
						Random.one_in_(50 - Misc.p_ptr.state.skills[(int)Skill.SEARCH_FREQUENCY])) {
					search(false);
				}

				/* Handle "store doors" */
				if ((Cave.cave.feat[Misc.p_ptr.py][Misc.p_ptr.px] >= Cave.FEAT_SHOP_HEAD) &&
				    (Cave.cave.feat[Misc.p_ptr.py][Misc.p_ptr.px] <= Cave.FEAT_SHOP_TAIL))
				{
				    /* Disturb */
				    Cave.disturb(Misc.p_ptr, 0, 0);
					Game_Command.insert(Command_Code.ENTER_STORE);
				}

				/* All other grids (including traps) */
				else
				{
				    /* Handle objects (later) */
				    Misc.p_ptr.notice |= (Misc.PN_PICKUP);
				}


				/* Discover invisible traps */
				if (Cave.cave.feat[y][x] == Cave.FEAT_INVIS)
				{
				    /* Disturb */
				    Cave.disturb(Misc.p_ptr, 0, 0);

				    /* Message */
				    Utilities.msg("You found a trap!");

					throw new NotImplementedException();

					///* Pick a trap */
					//pick_trap(y, x);

					///* Hit the trap */
					//hit_trap(y, x);
				}

				/* Set off an visible trap */
				else if (Cave.cave_isknowntrap(Cave.cave, y, x))
				{
				    /* Disturb */
				    Cave.disturb(Misc.p_ptr, 0, 0);

					throw new NotImplementedException();
				    /* Hit the trap */
				    //hit_trap(y, x);
				}
			}

			Misc.p_ptr.running_firststep = false;
		}

		public static int do_autopickup()
		{
			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			short this_o_idx, next_o_idx = 0;

			Object.Object o_ptr;

			/* Objects picked up.  Used to determine time cost of command. */
			byte objs_picked_up = 0;

			int floor_num = 0;
			int[] floor_list = new int[Misc.MAX_FLOOR_STACK + 1];

			/* Nothing to pick up -- return */
			if (Cave.cave.o_idx[py][px] == 0) return (0);

			throw new NotImplementedException();

			///* Always pickup gold, effortlessly */
			//py_pickup_gold();


			///* Scan the remaining objects */
			//for (this_o_idx = cave.o_idx[py][px]; this_o_idx; this_o_idx = next_o_idx)
			//{
			//    /* Get the object and the next object */
			//    o_ptr = object_byid(this_o_idx);
			//    next_o_idx = o_ptr.next_o_idx;

			//    /* Ignore all hidden objects and non-objects */
			//    if (squelch_item_ok(o_ptr) || !o_ptr.kind) continue;

			//    /* XXX Hack -- Enforce limit */
			//    if (floor_num >= N_ELEMENTS(floor_list)) break;


			//    /* Hack -- disturb */
			//    disturb(p_ptr, 0, 0);


			//    /* Automatically pick up items into the backpack */
			//    if (auto_pickup_okay(o_ptr))
			//    {
			//        /* Pick up the object with message */
			//        py_pickup_aux(this_o_idx, true);
			//        objs_picked_up++;

			//        continue;
			//    }


			//    /* Tally objects and store them in an array. */

			//    /* Remember this object index */
			//    floor_list[floor_num] = this_o_idx;

			//    /* Count non-gold objects that remain on the floor. */
			//    floor_num++;
			//}

			//return objs_picked_up;
		}

		/*
		 * Search for hidden things.  Returns true if a search was attempted, returns
		 * false when the player has a 0% chance of finding anything.  Prints messages
		 * for negative confirmation when verbose mode is requested.
		 */
		public static bool search(bool verbose)
		{
			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			int y, x, chance;

			bool found = false;

			Object.Object o_ptr;


			/* Start with base search ability */
			chance = Misc.p_ptr.state.skills[(int)Skill.SEARCH];

			/* Penalize various conditions */
			if (Misc.p_ptr.timed[(int)Timed_Effect.BLIND] != 0 || Cave.no_light()) chance = chance / 10;
			if (Misc.p_ptr.timed[(int)Timed_Effect.CONFUSED] != 0 || Misc.p_ptr.timed[(int)Timed_Effect.IMAGE] != 0) chance = chance / 10;

			/* Prevent fruitless searches */
			if (chance <= 0)
			{
				if (verbose)
				{
					Utilities.msg("You can't make out your surroundings well enough to search.");

					/* Cancel repeat */
					Cave.disturb(Misc.p_ptr, 0, 0);
				}

				return false;
			}

			/* Search the nearby grids, which are always in bounds */
			for (y = (py - 1); y <= (py + 1); y++)
			{
				for (x = (px - 1); x <= (px + 1); x++)
				{
					/* Sometimes, notice things */
					if (Random.randint0(100) < chance)
					{
						/* Invisible trap */
						if (Cave.cave.feat[y][x] == Cave.FEAT_INVIS)
						{
							found = true;
							throw new NotImplementedException();
							///* Pick a trap */
							//pick_trap(y, x);

							///* Message */
							//Utilities.msg("You have found a trap.");

							///* Disturb */
							//Cave.disturb(Misc.p_ptr, 0, 0);
						}

						/* Secret door */
						if (Cave.cave.feat[y][x] == Cave.FEAT_SECRET)
						{
							found = true;

							/* Message */
							Utilities.msg("You have found a secret door.");

							throw new NotImplementedException();
							///* Pick a door */
							//place_closed_door(cave, y, x);

							///* Disturb */
							//disturb(p_ptr, 0, 0);
						}

						/* Scan all objects in the grid */
						for (o_ptr = Object.Object.get_first_object(y, x); o_ptr != null; o_ptr = Object.Object.get_next_object(o_ptr))
						{
							throw new NotImplementedException();
							///* Skip non-chests */
							//if (o_ptr.tval != TV_CHEST) continue;

							///* Skip disarmed chests */
							//if (o_ptr.pval[DEFAULT_PVAL] <= 0) continue;

							///* Skip non-trapped chests */
							//if (!chest_traps[o_ptr.pval[DEFAULT_PVAL]]) continue;

							///* Identify once */
							//if (!object_is_known(o_ptr))
							//{
							//    found = true;

							//    /* Message */
							//    msg("You have discovered a trap on the chest!");

							//    /* Know the trap */
							//    object_notice_everything(o_ptr);

							//    /* Notice it */
							//    disturb(p_ptr, 0, 0);
							//}
						}
					}
				}
			}

			if (verbose && !found)
			{
				if (chance >= 100)
					Utilities.msg("There are no secrets here.");
				else
					Utilities.msg("You found nothing.");
			}

			return true;
		}

	}
}
