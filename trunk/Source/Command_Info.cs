using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Command_Info {
		/*
		 * Holds a generic command - if cmd is set to other than CMD_null 
		 * it simply pushes that command to the game, otherwise the hook 
		 * function will be called.
		 */
		public Command_Info() {
			desc = "";
			key = '\0';
			cmd = Command_Code.NULL;
		}
		public Command_Info(string desc, char key, Command_Code cmd, hook_func hook = null, prereq_func prereq = null) {
			this.desc = desc;
			this.key = key;
			this.cmd = cmd;
			if(hook != null) {
				this.hook = hook;
			}
			if(prereq != null) {
				this.prereq = prereq;
			}
		}
		public string desc;
		public char key;
		public Command_Code cmd;

		public delegate void hook_func();
		public hook_func hook;

		public delegate bool prereq_func();
		public prereq_func prereq;

		/*
		 * This file contains (several) big lists of commands, so that they can be
		 * easily maniuplated for e.g. help displays, or if a port wants to provide a
		 * native menu containing a command list.
		 *
		 * Consider a two-paned layout for the command menus. XXX
		 *
		 * This file still needs some clearing up. XXX
		 */

		/*** Handling bits ***/
		public static Command_Info[] cmd_item = new Command_Info[]
		{
			new Command_Info("Inscribe an object",		'{', Command_Code.INSCRIBE),
			new Command_Info("Uninscribe an object",	'}', Command_Code.UNINSCRIBE),
			new Command_Info("Wear/wield an item",		'w', Command_Code.WIELD),
			new Command_Info("Take off/unwield an item",'t', Command_Code.TAKEOFF),
			new Command_Info("Examine an item",			'I', Command_Code.NULL, TextUI.obj_examine ),
			new Command_Info("Drop an item",			'd', Command_Code.DROP),
			new Command_Info("Fire your missile weapon",'f', Command_Code.FIRE, null, Player.Player.can_fire ),
			new Command_Info("Use a staff",				'u', Command_Code.USE_STAFF),
			new Command_Info("Aim a wand",				'a', Command_Code.USE_WAND),
			new Command_Info("Zap a rod",				'z', Command_Code.USE_ROD),
			new Command_Info("Activate an object",		'A', Command_Code.ACTIVATE),
			new Command_Info("Eat some food",			'E', Command_Code.EAT),
			new Command_Info("Quaff a potion",			'q', Command_Code.QUAFF),
			new Command_Info("Read a scroll",			'r', Command_Code.READ_SCROLL, null, Player.Player.can_read ),
			new Command_Info("Fuel your light source",	'F', Command_Code.REFILL),
		};

		/* General actions */
		public static Command_Info[] cmd_action = new Command_Info[]
		{
			new Command_Info("Search for traps/doors",		's', Command_Code.SEARCH,	null),
			new Command_Info("Disarm a trap or chest",		'D', Command_Code.DISARM,	null),
			new Command_Info("Rest for a while",			'R', Command_Code.NULL,		TextUI.cmd_rest),
			new Command_Info("Look around",					'l', Command_Code.NULL,		Do_Command.look),
			new Command_Info("Target monster or location",	'*', Command_Code.NULL,		Do_Command.target),
			new Command_Info("Target closest monster",		'\'',Command_Code.NULL,		Do_Command.target_closest),
			new Command_Info("Dig a tunnel",				'T', Command_Code.TUNNEL,	null),
			new Command_Info("Go up staircase",				'<', Command_Code.GO_UP,	null),
			new Command_Info("Go down staircase",			'>', Command_Code.GO_DOWN,	null),
			new Command_Info("Toggle search mode",			'S', Command_Code.TOGGLE_SEARCH, null),
			new Command_Info("Open a door or a chest",		'o', Command_Code.OPEN,		null),
			new Command_Info("Close a door",				'c', Command_Code.CLOSE,	null),
			new Command_Info("Jam a door shut",				'j', Command_Code.JAM,		null),
			new Command_Info("Bash a door open",			'B', Command_Code.BASH,		null),
			new Command_Info("Fire at nearest target",		'h', Command_Code.NULL,		TextUI.cmd_fire_at_nearest),
			new Command_Info("Throw an item",				'v', Command_Code.THROW,	TextUI.cmd_throw),
			new Command_Info("Walk into a trap",			'W', Command_Code.JUMP,		null),
		};

		/* Item management commands */
		public static Command_Info[] cmd_item_manage = new Command_Info[]
		{
			new Command_Info("Display equipment listing", 'e', Command_Code.NULL, Do_Command.equip),
			new Command_Info("Display inventory listing", 'i', Command_Code.NULL, Do_Command.inven),
			new Command_Info("Pick up objects",           'g', Command_Code.PICKUP, null),
			new Command_Info("Destroy an item",           'k', Command_Code.DESTROY, TextUI.cmd_destroy)	
		};

		/* Information access commands */
		public static Command_Info[] cmd_info = new Command_Info[]
		{
			new Command_Info("Browse a book",				'b', Command_Code.BROWSE_SPELL, TextUI.spell_browse, null),
			new Command_Info("Gain new spells",				'G', Command_Code.STUDY_BOOK, TextUI.obj_study, Player.Player.can_study),
			new Command_Info("Cast a spell",				'm', Command_Code.CAST, TextUI.obj_cast, Player.Player.can_cast ),
			new Command_Info("Cast a spell",				'p', Command_Code.CAST, TextUI.obj_cast, Player.Player.can_cast ),
			new Command_Info("Full dungeon map",            'M', Command_Code.NULL, Do_Command.view_map ),
			new Command_Info("Toggle ignoring of items",    'K', Command_Code.NULL, TextUI.cmd_toggle_ignore ),
			new Command_Info("Display visible item list",   ']', Command_Code.NULL, Do_Command.itemlist ),
			new Command_Info("Display visible monster list",'[', Command_Code.NULL, Do_Command.monlist ),
			new Command_Info("Locate player on map",        'L', Command_Code.NULL, Do_Command.locate ),
			new Command_Info("Help",                        '?', Command_Code.NULL, Do_Command.help ),
			new Command_Info("Identify symbol",             '/', Command_Code.NULL, Do_Command.query_symbol ),
			new Command_Info("Character description",       'C', Command_Code.NULL, Do_Command.change_name ),
			new Command_Info("Check knowledge",             '~', Command_Code.NULL, TextUI.browse_knowledge ),
			new Command_Info("Repeat level feeling",   UIEvent.KTRL('F'),Command_Code.NULL, Do_Command.feeling ),
			new Command_Info("Show previous message",  UIEvent.KTRL('O'),Command_Code.NULL, Do_Command.message_one ),
			new Command_Info("Show previous messages", UIEvent.KTRL('P'),Command_Code.NULL, Do_Command.messages )
		};

		/* Utility/assorted commands */
		public static Command_Info[] cmd_util = new Command_Info[]
		{
			new Command_Info("Interact with options",        '=', Command_Code.NULL, Do_Command.xxx_options ),

			new Command_Info("Save and don't quit",  UIEvent.KTRL('S'), Command_Code.SAVE, null ),
			new Command_Info("Save and quit",        UIEvent.KTRL('X'), Command_Code.QUIT, null ),
			new Command_Info("Quit (commit suicide)",      'Q', Command_Code.NULL, TextUI.cmd_suicide ),
			new Command_Info("Redraw the screen",    UIEvent.KTRL('R'), Command_Code.NULL, Do_Command.redraw ),

			new Command_Info("Load \"screen dump\"",       '(', Command_Code.NULL, Do_Command.load_screen ),
			new Command_Info("Save \"screen dump\"",       ')', Command_Code.NULL, Do_Command.save_screen )
		};

		/*
		 * Flip "inven" and "equip" in any sub-windows
		 */
		//TOO: Find a better place for this!!!
		public static void toggle_inven_equip()
		{
			throw new NotImplementedException();
			//term *old = Term;
			//int i;

			///* Change the actual setting */
			//flip_inven = !flip_inven;

			///* Redraw any subwindows showing the inventory/equipment lists */
			//for (i = 0; i < ANGBAND_TERM_MAX; i++)
			//{
			//    Term_activate(angband_term[i]); 

			//    if (op_ptr.window_flag[i] & PW_INVEN)
			//    {
			//        if (!flip_inven)
			//            show_inven(OLIST_WINDOW | OLIST_WEIGHT | OLIST_QUIVER);
			//        else
			//            show_equip(OLIST_WINDOW | OLIST_WEIGHT);
			
			//        Term_fresh();
			//    }
			//    else if (op_ptr.window_flag[i] & PW_EQUIP)
			//    {
			//        if (!flip_inven)
			//            show_equip(OLIST_WINDOW | OLIST_WEIGHT);
			//        else
			//            show_inven(OLIST_WINDOW | OLIST_WEIGHT | OLIST_QUIVER);
			
			//        Term_fresh();
			//    }
			//}

			//Term_activate(old);
		}

		/* Commands that shouldn't be shown to the user */ 
		public static Command_Info[] cmd_hidden= new Command_Info[]
		{
			new Command_Info("Take notes",               ':', Command_Code.NULL, Do_Command.note ),
			new Command_Info("Version info",             'V', Command_Code.NULL, Do_Command.version ),
			new Command_Info("Load a single pref line",  '"', Command_Code.NULL, Do_Command.pref ),
			new Command_Info("Enter a store",            '_', Command_Code.ENTER_STORE, null ),
			new Command_Info("Toggle windows",     UIEvent.KTRL('E'), Command_Code.NULL, toggle_inven_equip ), /* XXX */
			new Command_Info("Alter a grid",             '+', Command_Code.ALTER, null ),
			new Command_Info("Walk",                     ';', Command_Code.WALK, null ),
			new Command_Info("Start running",            '.', Command_Code.RUN, null ),
			new Command_Info("Stand still",              ',', Command_Code.HOLD, null ),
			new Command_Info("Center map",              UIEvent.KTRL('L'), Command_Code.NULL, Do_Command.center_map ),

			new Command_Info("Toggle wizard mode",  UIEvent.KTRL('W'), Command_Code.NULL, Do_Command.wizard ),
			new Command_Info("Repeat previous command",  UIEvent.KTRL('V'), Command_Code.REPEAT, null ),
			new Command_Info("Do autopickup",          UIEvent.KTRL('G'), Command_Code.AUTOPICKUP, null ),

		/*#ifdef ALLOW_DEBUG
			"Debug mode commands", KTRL('A'), CMD_null, do_cmd_try_debug },
		#endif
		#ifdef ALLOW_BORG
			"Borg commands",       KTRL('Z'), CMD_null, do_cmd_try_borg }
		#endif*/
		};
	}
}
