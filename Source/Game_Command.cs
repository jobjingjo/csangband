using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	public enum Direction {
		UNKNOWN = 0,
		NW = 1,
		N = 2,
		NE = 3,
		W = 4,
		TARGET = 5,
		NONE = 5,
		E = 6,
		SW = 7,
		S = 8,
		SE = 9,
	};

	public enum cmd_context {
		CMD_INIT,
		CMD_BIRTH,
		CMD_GAME,
		CMD_STORE,
		CMD_DEATH
	}

	/*
	* All valid game commands.  Not all implemented yet.
	*/
	public enum Command_Code {
		NULL = 0,	/* A "do nothing" command so that there's something
							   UIs can use as a "no command yet" sentinel. */
		/* 
		 * Splash screen commands 
		 */
		LOADFILE,
		NEWGAME,

		/* 
		 * Birth commands 
		 */
		BIRTH_RESET,
		CHOOSE_SEX,
		CHOOSE_RACE,
		CHOOSE_CLASS,
		FINALIZE_OPTIONS,
		BUY_STAT,
		SELL_STAT,
		RESET_STATS,
		ROLL_STATS,
		PREV_STATS,
		NAME_CHOICE,
		ACCEPT_CHARACTER,

		/* 
		 * The main game commands
		 */
		GO_UP,
		GO_DOWN,
		SEARCH,
		TOGGLE_SEARCH,
		WALK,
		JUMP,
		PATHFIND,

		INSCRIBE,
		UNINSCRIBE,
		TAKEOFF,
		WIELD,
		DROP,
		BROWSE_SPELL,
		STUDY_SPELL,
		STUDY_BOOK,
		CAST, /* Casting a spell /or/ praying. */
		USE_STAFF,
		USE_WAND,
		USE_ROD,
		ACTIVATE,
		EAT,
		QUAFF,
		READ_SCROLL,
		REFILL,
		FIRE,
		THROW,
		PICKUP,
		AUTOPICKUP,
		DESTROY,
		/*	CMD_SQUELCH_TYPE, -- might be a command, might have another interface */
		DISARM,
		REST,
		/*	CMD_TARGET, -- possibly should be a UI-level thing */
		TUNNEL,
		OPEN,
		CLOSE,
		JAM,
		BASH,
		RUN,
		HOLD,
		ENTER_STORE,
		ALTER,

		/* Store commands */
		SELL,
		BUY,
		STASH,
		RETRIEVE,

		/* Hors categorie Commands */
		SUICIDE,
		SAVE,

		/*	CMD_OPTIONS, -- probably won't be a command in this sense*/
		QUIT,
		HELP,
		REPEAT
	}

	public class cmd_arg
	{
		public string text;

		public int value;
	
		public Loc point;
	}

	public enum cmd_arg_type
	{
		arg_NONE = 0,
		arg_STRING = 0x01,
		arg_CHOICE = 0x02,
		arg_NUMBER = 0x04,
		arg_ITEM = 0x08,
		arg_DIRECTION = 0x10,
		arg_TARGET = 0x20,
		arg_POINT = 0x40
	};

	/*
	 * The game_command type is used to return details of the command the
	 * game should carry out.
	 *
	 * 'command' should always have a valid cmd_code value, the other entries
	 * may or may not be significant depending on the command being returned.
	 *
	 * NOTE: This is prone to change quite a bit while things are shaken out.
	 */
	class Game_Command {

		/* Item selector type (everything required for get_item()) */
		class item_selector_type
		{
			public item_selector_type(Command_Code a, string b, string c, Misc.item_tester_hook_func d, int e) {
				command = a;
				prompt = b;
				noop = c;
				filter = d;
				mode = e;
			}

			public Command_Code command;
			public string prompt;
			public string noop;

			//This delegate is now renamed and in Misc
			//public delegate bool filter_func(Object.Object o_ptr);
			public Misc.item_tester_hook_func filter;
			public int mode;
		};

		/** List of requirements for various commands' objects */
		static item_selector_type[] item_selector =
		{
			new item_selector_type( Command_Code.INSCRIBE, "Inscribe which item? ",
			  "You have nothing to inscribe.",
			  null, (Misc.USE_EQUIP | Misc.USE_INVEN | Misc.USE_FLOOR | Misc.IS_HARMLESS) ),

			new item_selector_type( Command_Code.UNINSCRIBE, "Un-inscribe which item? ",
			  "You have nothing to un-inscribe.",
			  Object.Object.obj_has_inscrip, (Misc.USE_EQUIP | Misc.USE_INVEN | Misc.USE_FLOOR) ),

			new item_selector_type( Command_Code.WIELD, "Wear/wield which item? ",
			  "You have nothing you can wear or wield.",
			  Object.Object.obj_can_wear, (Misc.USE_INVEN | Misc.USE_FLOOR) ),

			new item_selector_type( Command_Code.TAKEOFF, "Take off which item? ",
			  "You are not wearing anything you can take off.",
			  Object.Object.obj_can_takeoff, Misc.USE_EQUIP ),

			new item_selector_type( Command_Code.DROP, "Drop which item? ",
			  "You have nothing to drop.",
			  null, (Misc.USE_EQUIP | Misc.USE_INVEN) ),

			new item_selector_type( Command_Code.FIRE, "Fire which item? ",
			  "You have nothing to fire.",
			  Object.Object.obj_can_fire, (Misc.USE_INVEN | Misc.USE_EQUIP | Misc.USE_FLOOR | Misc.QUIVER_TAGS) ),

			new item_selector_type( Command_Code.USE_STAFF, "Use which staff? ",
			  "You have no staff to use.",
			  Object.Object.obj_is_staff, (Misc.USE_INVEN | Misc.USE_FLOOR | Misc.SHOW_FAIL) ),

			new item_selector_type( Command_Code.USE_WAND, "Aim which wand? ",
			  "You have no wand to aim.",
			  Object.Object.obj_is_wand, (Misc.USE_INVEN | Misc.USE_FLOOR | Misc.SHOW_FAIL) ),

			new item_selector_type( Command_Code.USE_ROD, "Zap which rod? ",
			  "You have no charged rods to zap.",
			  Object.Object.obj_is_rod, (Misc.USE_INVEN | Misc.USE_FLOOR | Misc.SHOW_FAIL) ),

			new item_selector_type( Command_Code.ACTIVATE, "Activate which item? ",
			  "You have nothing to activate.",
			  Object.Object.obj_is_activatable, (Misc.USE_EQUIP | Misc.SHOW_FAIL) ),

			new item_selector_type( Command_Code.EAT, "Eat which item? ",
			  "You have nothing to eat.",
			  Object.Object.obj_is_food, (Misc.USE_INVEN | Misc.USE_FLOOR) ),

			new item_selector_type( Command_Code.QUAFF, "Quaff which potion? ",
			  "You have no potions to quaff.",
			  Object.Object.obj_is_potion, (Misc.USE_INVEN | Misc.USE_FLOOR) ),

			new item_selector_type( Command_Code.READ_SCROLL, "Read which scroll? ",
			  "You have no scrolls to read.",
			  Object.Object.obj_is_scroll, (Misc.USE_INVEN | Misc.USE_FLOOR) ),

			new item_selector_type( Command_Code.REFILL, "Refuel with what fuel source? ",
			  "You have nothing to refuel with.",
			  Object.Object.obj_can_refill, (Misc.USE_INVEN | Misc.USE_FLOOR) ),
		};


		/* 
		 * Command handlers will take a pointer to the command structure
		 * so that they can access any arguments supplied.
		 */
		public delegate void cmd_handler_fn(Command_Code code, cmd_arg[] args);
		/* Maximum number of arguments a command needs to take. */
		public const int CMD_MAX_ARGS = 2;

		public Game_Command(Command_Code cmd, cmd_arg_type[] args, cmd_handler_fn func, bool r, int ar) {
			this.command = cmd;
			for(int i = 0; args != null && i < args.Length; i++) {
				this.arg_type[i] = args[i];
			}
			this.fn = func;
			this.repeat_allowed = r;
			this.nrepeats = ar;
		}

		public Game_Command() {
			this.command = Command_Code.NULL;
			this.arg_type = new cmd_arg_type[]{cmd_arg_type.arg_NONE, cmd_arg_type.arg_NONE};
			this.fn = null;
			this.repeat_allowed = false;
			this.nrepeats = 0;
		}
		/*
		 * A function called by the game to get a command from the UI.
		 */
		public delegate int cmd_get_hook_func(cmd_context c, bool wait);
		public static cmd_get_hook_func cmd_get_hook;


		/* A valid command code. */
		public Command_Code command;

		/* Number of times to attempt to repeat command. */
		int nrepeats;

		/* Arguments to the command */
		public cmd_arg[] arg = new cmd_arg[CMD_MAX_ARGS];

		/* Whether an argument was passed or not */
		bool[] arg_present = new bool[CMD_MAX_ARGS];

		/* Types of the arguments passed */
		cmd_arg_type[] arg_type = new cmd_arg_type[CMD_MAX_ARGS];
		
		cmd_handler_fn fn;
		bool repeat_allowed;
			
		
		static Game_Command[] game_cmds = new Game_Command[]
		{
			new Game_Command(Command_Code.LOADFILE, new cmd_arg_type[]{ cmd_arg_type.arg_NONE }, null, false, 0 ),
			new Game_Command(Command_Code.NEWGAME, new cmd_arg_type[]{ cmd_arg_type.arg_NONE }, null, false, 0 ),

			new Game_Command(Command_Code.BIRTH_RESET, new cmd_arg_type[]{ cmd_arg_type.arg_NONE }, null, false, 0 ),
			new Game_Command(Command_Code.CHOOSE_SEX, new cmd_arg_type[]{ cmd_arg_type.arg_CHOICE }, null, false, 0 ),
			new Game_Command(Command_Code.CHOOSE_RACE, new cmd_arg_type[]{ cmd_arg_type.arg_CHOICE }, null, false, 0 ),
			new Game_Command(Command_Code.CHOOSE_CLASS, new cmd_arg_type[]{ cmd_arg_type.arg_CHOICE }, null, false, 0 ),
 			new Game_Command(Command_Code.FINALIZE_OPTIONS, new cmd_arg_type[]{ cmd_arg_type.arg_CHOICE }, null, false, 0 ),
			new Game_Command(Command_Code.BUY_STAT, new cmd_arg_type[]{ cmd_arg_type.arg_CHOICE }, null, false, 0 ),
			new Game_Command(Command_Code.SELL_STAT, new cmd_arg_type[]{ cmd_arg_type.arg_CHOICE }, null, false, 0 ),
			new Game_Command(Command_Code.RESET_STATS, new cmd_arg_type[]{ cmd_arg_type.arg_CHOICE }, null, false, 0 ),
			new Game_Command(Command_Code.ROLL_STATS, new cmd_arg_type[]{ cmd_arg_type.arg_NONE }, null, false, 0 ),
			new Game_Command(Command_Code.PREV_STATS, new cmd_arg_type[]{ cmd_arg_type.arg_NONE }, null, false, 0 ),
			new Game_Command(Command_Code.NAME_CHOICE, new cmd_arg_type[]{ cmd_arg_type.arg_STRING }, null, false, 0 ),
			new Game_Command(Command_Code.ACCEPT_CHARACTER, new cmd_arg_type[]{ cmd_arg_type.arg_NONE }, null, false, 0 ),

			new Game_Command(Command_Code.GO_UP, new cmd_arg_type[]{ cmd_arg_type.arg_NONE }, Do_Command.go_up, false, 0 ),
			new Game_Command(Command_Code.GO_DOWN, new cmd_arg_type[]{ cmd_arg_type.arg_NONE }, Do_Command.go_down, false, 0 ),
			new Game_Command(Command_Code.SEARCH, new cmd_arg_type[]{ cmd_arg_type.arg_NONE }, Do_Command.search, true, 10 ),
			new Game_Command(Command_Code.TOGGLE_SEARCH,new cmd_arg_type[] { cmd_arg_type.arg_NONE }, Do_Command.toggle_search, false, 0 ),
			new Game_Command(Command_Code.WALK, new cmd_arg_type[]{ cmd_arg_type.arg_DIRECTION }, Do_Command.walk, true, 0 ),
			new Game_Command(Command_Code.RUN,new cmd_arg_type[] { cmd_arg_type.arg_DIRECTION }, Do_Command.run, false, 0 ),
			new Game_Command(Command_Code.JUMP,new cmd_arg_type[] { cmd_arg_type.arg_DIRECTION }, Do_Command.jump, false, 0 ),
			new Game_Command(Command_Code.OPEN,new cmd_arg_type[] { cmd_arg_type.arg_DIRECTION }, Do_Command.open, true, 99 ),
			new Game_Command(Command_Code.CLOSE,new cmd_arg_type[] { cmd_arg_type.arg_DIRECTION }, Do_Command.close, true, 99 ),
			new Game_Command(Command_Code.TUNNEL,new cmd_arg_type[] { cmd_arg_type.arg_DIRECTION }, Do_Command.tunnel, true, 99 ),
			new Game_Command(Command_Code.HOLD,new cmd_arg_type[] { cmd_arg_type.arg_NONE }, Do_Command.hold, true, 0 ),
			new Game_Command(Command_Code.DISARM, new cmd_arg_type[]{ cmd_arg_type.arg_DIRECTION }, Do_Command.disarm, true, 99 ),
			new Game_Command(Command_Code.BASH,new cmd_arg_type[] { cmd_arg_type.arg_DIRECTION }, Do_Command.bash, true, 99 ),
			new Game_Command(Command_Code.ALTER, new cmd_arg_type[]{ cmd_arg_type.arg_DIRECTION }, Do_Command.alter, true, 99 ),
			new Game_Command(Command_Code.JAM, new cmd_arg_type[]{ cmd_arg_type.arg_DIRECTION }, Do_Command.spike, false, 0 ),
			new Game_Command(Command_Code.REST, new cmd_arg_type[]{ cmd_arg_type.arg_CHOICE }, Do_Command.rest, false, 0 ),
			new Game_Command(Command_Code.PATHFIND,new cmd_arg_type[] { cmd_arg_type.arg_POINT }, Do_Command.pathfind, false, 0 ),
			new Game_Command(Command_Code.PICKUP, new cmd_arg_type[]{ cmd_arg_type.arg_ITEM }, Do_Command.pickup, false, 0 ),
			new Game_Command(Command_Code.AUTOPICKUP, new cmd_arg_type[]{ cmd_arg_type.arg_NONE }, Do_Command.autopickup, false, 0 ),
			new Game_Command(Command_Code.WIELD, new cmd_arg_type[]{ cmd_arg_type.arg_ITEM, cmd_arg_type.arg_NUMBER }, Do_Command.wield, false, 0 ),
			new Game_Command(Command_Code.TAKEOFF,new cmd_arg_type[] { cmd_arg_type.arg_ITEM }, Do_Command.takeoff, false, 0 ),
			new Game_Command(Command_Code.DROP,new cmd_arg_type[] { cmd_arg_type.arg_ITEM, cmd_arg_type.arg_NUMBER }, Do_Command.drop, false, 0 ),
			new Game_Command(Command_Code.UNINSCRIBE, new cmd_arg_type[]{ cmd_arg_type.arg_ITEM }, Do_Command.uninscribe, false, 0 ),
			new Game_Command(Command_Code.EAT,new cmd_arg_type[] { cmd_arg_type.arg_ITEM }, Do_Command.use, false, 0 ),
			new Game_Command(Command_Code.QUAFF, new cmd_arg_type[]{ cmd_arg_type.arg_ITEM, cmd_arg_type.arg_TARGET }, Do_Command.use, false, 0 ),
			new Game_Command(Command_Code.USE_ROD, new cmd_arg_type[]{ cmd_arg_type.arg_ITEM, cmd_arg_type.arg_TARGET }, Do_Command.use, false, 0 ),
			new Game_Command(Command_Code.USE_STAFF,new cmd_arg_type[] { cmd_arg_type.arg_ITEM }, Do_Command.use, false, 0 ),
			new Game_Command(Command_Code.USE_WAND,new cmd_arg_type[] { cmd_arg_type.arg_ITEM, cmd_arg_type.arg_TARGET }, Do_Command.use, false, 0 ),
			new Game_Command(Command_Code.READ_SCROLL,new cmd_arg_type[] { cmd_arg_type.arg_ITEM, cmd_arg_type.arg_TARGET }, Do_Command.use, false, 0 ),
			new Game_Command(Command_Code.ACTIVATE,new cmd_arg_type[] { cmd_arg_type.arg_ITEM, cmd_arg_type.arg_TARGET }, Do_Command.use, false, 0 ),
			new Game_Command(Command_Code.REFILL,new cmd_arg_type[] { cmd_arg_type.arg_ITEM }, Do_Command.refill, false, 0 ),
			new Game_Command(Command_Code.FIRE, new cmd_arg_type[]{ cmd_arg_type.arg_ITEM, cmd_arg_type.arg_TARGET }, Do_Command.fire, false, 0 ),
			new Game_Command(Command_Code.THROW, new cmd_arg_type[]{ cmd_arg_type.arg_ITEM, cmd_arg_type.arg_TARGET }, Do_Command.Throw, false, 0 ),
			new Game_Command(Command_Code.DESTROY, new cmd_arg_type[]{ cmd_arg_type.arg_ITEM }, Do_Command.destroy, false, 0 ),
			new Game_Command(Command_Code.ENTER_STORE,new cmd_arg_type[] { cmd_arg_type.arg_NONE }, Do_Command.store, false, 0 ),
			new Game_Command(Command_Code.INSCRIBE,new cmd_arg_type[] { cmd_arg_type.arg_ITEM, cmd_arg_type.arg_STRING }, Do_Command.inscribe, false, 0 ),
			new Game_Command(Command_Code.STUDY_SPELL, new cmd_arg_type[]{ cmd_arg_type.arg_CHOICE }, Do_Command.study_spell, false, 0 ),
			new Game_Command(Command_Code.STUDY_BOOK,new cmd_arg_type[] { cmd_arg_type.arg_ITEM }, Do_Command.study_book, false, 0 ),
			new Game_Command(Command_Code.CAST, new cmd_arg_type[]{ cmd_arg_type.arg_CHOICE, cmd_arg_type.arg_TARGET }, Do_Command.cast, false, 0 ),
			new Game_Command(Command_Code.SELL, new cmd_arg_type[]{ cmd_arg_type.arg_ITEM, cmd_arg_type.arg_NUMBER }, Do_Command.sell, false, 0 ),
			new Game_Command(Command_Code.STASH, new cmd_arg_type[]{ cmd_arg_type.arg_ITEM, cmd_arg_type.arg_NUMBER }, Do_Command.stash, false, 0 ),
			new Game_Command(Command_Code.BUY, new cmd_arg_type[]{ cmd_arg_type.arg_CHOICE, cmd_arg_type.arg_NUMBER }, Do_Command.buy, false, 0 ),
			new Game_Command(Command_Code.RETRIEVE, new cmd_arg_type[]{ cmd_arg_type.arg_CHOICE, cmd_arg_type.arg_NUMBER }, Do_Command.retrieve, false, 0 ),
			new Game_Command(Command_Code.SUICIDE, new cmd_arg_type[]{ cmd_arg_type.arg_NONE }, Do_Command.suicide, false, 0 ),
			new Game_Command(Command_Code.SAVE, new cmd_arg_type[]{ cmd_arg_type.arg_NONE }, Do_Command.save_game, false, 0 ),
			new Game_Command(Command_Code.QUIT,new cmd_arg_type[] { cmd_arg_type.arg_NONE }, Do_Command.quit, false, 0 ),
			new Game_Command(Command_Code.HELP,new cmd_arg_type[] { cmd_arg_type.arg_NONE }, null, false, 0 ),
			new Game_Command(Command_Code.REPEAT, new cmd_arg_type[]{ cmd_arg_type.arg_NONE }, null, false, 0 )
		};

		static bool repeating = false;
		static bool repeat_prev_allowed = false;

		const int CMD_QUEUE_SIZE = 20;
		static int prev_cmd_idx(int idx){
			return ((idx + CMD_QUEUE_SIZE - 1) % CMD_QUEUE_SIZE);
		}

		static int cmd_head = 0;
		static int cmd_tail = 0;
		static Game_Command[] cmd_queue = new Game_Command[CMD_QUEUE_SIZE];

		/*
		 * Get the next game command, with 'wait' indicating whether we
		 * are prepared to wait for a command or require a quick return with
		 * no command.
		 */
		public static Game_Command get(cmd_context c, ref Game_Command cmd, bool wait)
		{
			/* If we're repeating, just pull the last command again. */
			if (repeating)
			{
			    cmd = cmd_queue[prev_cmd_idx(cmd_tail)];
			    return cmd;
			}

			/* If there are no commands queued, ask the UI for one. */
			if (cmd_head == cmd_tail) 
			    cmd_get_hook(c, wait);

			/* If we have a command ready, set it and return success. */
			if (cmd_head != cmd_tail)
			{
			    cmd = cmd_queue[cmd_tail++];
			    if (cmd_tail == CMD_QUEUE_SIZE) cmd_tail = 0;

			    return cmd;
			}

			/* Failure to get a command. */
			return null;
		}

		/* 
		 * Inserts a command in the queue to be carried out. 
		 */
		public static int insert(Command_Code c)
		{
			return insert_repeated(c, 0);
		}

		/*
		 * Inserts a command in the queue to be carried out, with the given
		 * number of repeats.
		 */
		public static int insert_repeated(Command_Code c, int nrepeats)
		{
			Game_Command cmd = new Game_Command();

			if (cmd_idx(c) == -1)
				return 1;

			cmd.command = c;
			cmd.nrepeats = nrepeats;

			return cmd_insert_s(ref cmd);
		}

		/* Return the index of the given command in the command array. */
		static int cmd_idx(Command_Code code)
		{
			for (int i = 0; i < game_cmds.Length; i++)
			{
				if (game_cmds[i].command == code)
					return i;
			}

			return -1;
		}

		/*
		 * Insert the given command into the command queue.
		 */
		static int cmd_insert_s(ref Game_Command cmd)
		{
			/* If queue full, return error */
			if (cmd_head + 1 == cmd_tail) return 1;
			if (cmd_head + 1 == CMD_QUEUE_SIZE && cmd_tail == 0) return 1;

			/* Insert command into queue. */
			if (cmd.command != Command_Code.REPEAT)
			{
				cmd_queue[cmd_head] = cmd;
			}
			else
			{
				int cmd_prev = cmd_head - 1;

				//was previously just "repeat_prev_allowed"
				if (!repeat_prev_allowed) return 1;

				/* If we're repeating a command, we duplicate the previous command 
				   in the next command "slot". */
				if (cmd_prev < 0) cmd_prev = CMD_QUEUE_SIZE - 1;
		
				if (cmd_queue[cmd_prev].command != Command_Code.NULL)
					cmd_queue[cmd_head] = cmd_queue[cmd_prev];
			}

			/* Advance point in queue, wrapping around at the end */
			cmd_head++;
			if (cmd_head == CMD_QUEUE_SIZE) cmd_head = 0;

			return 0;	
		}

		public void set_arg_choice(int n, int choice)
		{
			int idx = cmd_idx(command);

			Misc.assert(n <= CMD_MAX_ARGS);
			Misc.assert((game_cmds[idx].arg_type[n] & cmd_arg_type.arg_CHOICE) != 0);

			arg[n] = new cmd_arg();
			arg[n].value = choice;
			arg_type[n] = cmd_arg_type.arg_CHOICE;
			arg_present[n] = true;
		}

		public void set_arg_number(int n, int num)
		{
			int idx = cmd_idx(command);

			Misc.assert(n <= CMD_MAX_ARGS);
			Misc.assert((game_cmds[idx].arg_type[n] & cmd_arg_type.arg_NUMBER) != 0);

			arg[n] = new cmd_arg();
			arg[n].value = num;
			arg_type[n] = cmd_arg_type.arg_NUMBER;
			arg_present[n] = true;
		}

		public void set_arg_choice(int n, string choice)
		{
			int idx = cmd_idx(command);

			Misc.assert(n <= CMD_MAX_ARGS);
			Misc.assert((game_cmds[idx].arg_type[n] & cmd_arg_type.arg_STRING) != 0);

			arg[n] = new cmd_arg();
			arg[n].text = choice;
			arg_type[n] = cmd_arg_type.arg_STRING;
			arg_present[n] = true;
		}

		public static Game_Command get_top()
		{
			return cmd_queue[prev_cmd_idx(cmd_head)];
		}

		/*
		 * Do not allow the current command to be repeated by the user using the
		 * "repeat last command" command.
		 */
		public static void disable_repeat()
		{
			repeat_prev_allowed = false;
		}

		/* 
		 * Returns the number of repeats left for the current command.
		 * i.e. zero if not repeating.
		 */
		public static int get_nrepeats()
		{
			Game_Command cmd = cmd_queue[prev_cmd_idx(cmd_tail)];
			return cmd.nrepeats;
		}

		/* 
		 * Remove any pending repeats from the current command. 
		 */
		public static void cancel_repeat()
		{
			Game_Command cmd = cmd_queue[prev_cmd_idx(cmd_tail)];

			if (cmd.nrepeats != 0 || repeating)
			{
				/* Cancel */
				cmd.nrepeats = 0;
				repeating = false;
		
				/* Redraw the state (later) */
				Misc.p_ptr.redraw |= (Misc.PR_STATE);
			}
		}

		/* 
		 * Request a game command from the uI and carry out whatever actions
		 * go along with it.
		 */
		public static void process_command(cmd_context ctx, bool no_request)
		{
			Game_Command cmd = new Game_Command();

			/* Reset so that when selecting items, we look in the default location */
			Misc.p_ptr.command_wrk = 0;

			/* If we've got a command to process, do it. */
			if (get(ctx, ref cmd, !no_request) != null) //Was ==null...
			{
				int oldrepeats = cmd.nrepeats;
				int idx = cmd_idx(cmd.command);
				int i;

				if (idx == -1) return;

				for (i = 0; i < item_selector.Length; i++)
				{
					item_selector_type itms = item_selector[i];

					if (itms.command != cmd.command)
						continue;

					if (!cmd.arg_present[0])
					{
						int item = 0;

						Misc.item_tester_hook = itms.filter;
						if (!Object.Object.get_item(ref item, itms.prompt, itms.noop, cmd.command, itms.mode))
						    return;

						cmd.set_arg_item(0, item);
					}
				}

				/* XXX avoid dead objects from being re-used on repeat.
				 * this needs to be expanded into a general safety-check
				 * on args */
				if ((game_cmds[idx].arg_type[0] == cmd_arg_type.arg_ITEM) && cmd.arg_present[0]) {
					Object.Object o_ptr = Object.Object.object_from_item_idx(cmd.arg[0].value);
					if (o_ptr.kind == null)
					    return;
				}

				/* Do some sanity checking on those arguments that might have 
				   been declared as "unknown", such as directions and targets. */
				switch (cmd.command)
				{
					case Command_Code.INSCRIBE:
					{
						throw new NotImplementedException();
						//char o_name[80];
						//char tmp[80] = "";

						//object_type *o_ptr = object_from_item_idx(cmd.arg[0].item);
			
						//object_desc(o_name, sizeof(o_name), o_ptr, ODESC_PREFIX | ODESC_FULL);
						//msg("Inscribing %s.", o_name);
						//message_flush();
			
						///* Use old inscription */
						//if (o_ptr.note)
						//    strnfmt(tmp, sizeof(tmp), "%s", quark_str(o_ptr.note));
			
						///* Get a new inscription (possibly empty) */
						//if (!get_string("Inscription: ", tmp, sizeof(tmp)))
						//    return;

						//cmd_set_arg_string(cmd, 1, tmp);
						//break;
					}

					case Command_Code.OPEN:
					{
						throw new NotImplementedException();
						//if (OPT(easy_open) && (!cmd.arg_present[0] ||
						//        cmd.arg[0].direction == DIR_UNKNOWN))
						//{
						//    int y, x;
						//    int n_closed_doors, n_locked_chests;
			
						//    n_closed_doors = count_feats(&y, &x, cave_iscloseddoor, false);
						//    n_locked_chests = count_chests(&y, &x, false);
			
						//    if (n_closed_doors + n_locked_chests == 1)
						//        cmd_set_arg_direction(cmd, 0, coords_to_dir(y, x));
						//}

						//goto get_dir;
					}

					case Command_Code.CLOSE:
					{
						throw new NotImplementedException();
						//if (OPT(easy_open) && (!cmd.arg_present[0] ||
						//        cmd.arg[0].direction == DIR_UNKNOWN))
						//{
						//    int y, x;
			
						//    /* Count open doors */
						//    if (count_feats(&y, &x, cave_isopendoor, false) == 1)
						//        cmd_set_arg_direction(cmd, 0, coords_to_dir(y, x));
						//}

						//goto get_dir;
					}

					case Command_Code.DISARM:
					{
						throw new NotImplementedException();
						//if (OPT(easy_open) && (!cmd.arg_present[0] ||
						//        cmd.arg[0].direction == DIR_UNKNOWN))
						//{
						//    int y, x;
						//    int n_visible_traps, n_trapped_chests;
			
						//    n_visible_traps = count_feats(&y, &x, cave_isknowntrap, true);
						//    n_trapped_chests = count_chests(&y, &x, true);

						//    if (n_visible_traps + n_trapped_chests == 1)
						//        cmd_set_arg_direction(cmd, 0, coords_to_dir(y, x));
						//}

						//goto get_dir;
					}

					case Command_Code.TUNNEL:
					case Command_Code.WALK:
					case Command_Code.RUN:
					case Command_Code.JUMP:
					case Command_Code.BASH:
					case Command_Code.ALTER:
					case Command_Code.JAM:
					{
					get_dir:
						/* Direction hasn't been specified, so we ask for one. */
						if (!cmd.arg_present[0] || cmd.arg[0].value == (int)Direction.UNKNOWN)
						{
						    int dir;
						    if (!Xtra2.get_rep_dir(out dir))
						        return;

							
						    cmd.set_arg_direction(0, dir);
						}
				
						break;
					}

					case Command_Code.DROP:
					{
						throw new NotImplementedException();
						//if (!cmd.arg_present[1])
						//{
						//    object_type *o_ptr = object_from_item_idx(cmd.arg[0].item);
						//    int amt = get_quantity(null, o_ptr.number);
						//    if (amt <= 0)
						//        return;

						//    cmd_set_arg_number(cmd, 1, amt);
						//}

						//break;
					}
			
					/* 
					 * These take an item number and a  "target" as arguments, 
					 * though a target isn't always actually needed, so we'll 
					 * only prompt for it via callback if the item being used needs it.
					 */
					case Command_Code.USE_WAND:
					case Command_Code.USE_ROD:
					case Command_Code.QUAFF:
					case Command_Code.ACTIVATE:
					case Command_Code.READ_SCROLL:
					case Command_Code.FIRE:
					case Command_Code.THROW:
					{
						bool get_target = false;
						Object.Object o_ptr = Object.Object.object_from_item_idx(cmd.arg[0].value);

						/* If we couldn't resolve the item, then abort this */
						if (o_ptr.kind == null) break;

						/* Thrown objects always need an aim, others might, depending
						 * on the object */
						if (o_ptr.needs_aim() || cmd.command == Command_Code.THROW)
						{
							if(!cmd.arg_present[1]) {
								get_target = true;
							} else if(cmd.arg[1].value == (int)Direction.UNKNOWN) {
								get_target = true;
							} else if(cmd.arg[1].value == (int)Direction.TARGET && !Target.okay()) {
								get_target = true;
							}
						}

						cmd.arg[1] = new cmd_arg();
						cmd.arg[1].value = 0;

						if (get_target && !Xtra2.get_aim_dir(ref cmd.arg[1].value))
						        return;

						Misc.p_ptr.confuse_dir(ref cmd.arg[1].value, false);
						cmd.arg_present[1] = true;

						break;
					}
			
					/* This takes a choice and a direction. */
					case Command_Code.CAST:
					{
						throw new NotImplementedException();
						//bool get_target = false;

						//if (spell_needs_aim(Misc.p_ptr.Class.spell_book, cmd.arg[0].choice))
						//{
						//    if (!cmd.arg_present[1])
						//        get_target = true;

						//    if (cmd.arg[1].direction == DIR_UNKNOWN)
						//        get_target = true;

						//    if (cmd.arg[1].direction == DIR_TARGET && !target_okay())
						//        get_target = true;
						//}

						//if (get_target && !get_aim_dir(&cmd.arg[1].direction))
						//        return;

						//player_confuse_dir(p_ptr, &cmd.arg[1].direction, false);
						//cmd.arg_present[1] = true;
				
						//break;
					}

					case Command_Code.WIELD:
					{
						throw new NotImplementedException();
						//object_type *o_ptr = object_from_item_idx(cmd.arg[0].choice);
						//int slot = wield_slot(o_ptr);
			
						///* Usually if the slot is taken we'll just replace the item in the slot,
						// * but in some cases we need to ask the user which slot they actually
						// * want to replace */
						//if (p_ptr.inventory[slot].kind)
						//{
						//    if (o_ptr.tval == TV_RING)
						//    {
						//        const char *q = "Replace which ring? ";
						//        const char *s = "Error in obj_wield, please report";
						//        item_tester_hook = obj_is_ring;
						//        if (!get_item(&slot, q, s, CMD_WIELD, USE_EQUIP)) return;
						//    }
			
						//    if (obj_is_ammo(o_ptr) && !object_similar(&p_ptr.inventory[slot],
						//        o_ptr, OSTACK_QUIVER))
						//    {
						//        const char *q = "Replace which ammunition? ";
						//        const char *s = "Error in obj_wield, please report";
						//        item_tester_hook = obj_is_ammo;
						//        if (!get_item(&slot, q, s, CMD_WIELD, USE_EQUIP)) return;
						//    }
						//}

						///* Set relevant slot */
						//cmd_set_arg_number(cmd, 1, slot);

						//break;
					}

					default: 
					{
						/* I can see the point of the compiler warning, but still... */
						break;
					}
				}

				/* Command repetition */
				if (game_cmds[idx].repeat_allowed)
				{
					/* Auto-repeat only if there isn't already a repeat length. */
					if (game_cmds[idx].nrepeats > 0 && cmd.nrepeats == 0)
					    Game_Command.set_repeat(game_cmds[idx].nrepeats);
				}
				else
				{
					cmd.nrepeats = 0;
					repeating = false;
				}

				/* 
				 * The command gets to unset this if it isn't appropriate for
				 * the user to repeat it.
				 */
				repeat_prev_allowed = true;

				if (game_cmds[idx].fn != null)
					game_cmds[idx].fn(cmd.command, cmd.arg);

				/* If the command hasn't changed nrepeats, count this execution. */
				if (cmd.nrepeats > 0 && oldrepeats == Game_Command.get_nrepeats())
					Game_Command.set_repeat(oldrepeats - 1);
			}
		}

		public void set_arg_direction(int n, int dir)
		{
			int idx = cmd_idx(command);

			Misc.assert(n <= CMD_MAX_ARGS);
			Misc.assert((game_cmds[idx].arg_type[n] & cmd_arg_type.arg_DIRECTION) != 0);

			arg[n] = new cmd_arg();
			arg[n].value = dir;
			arg_type[n] = cmd_arg_type.arg_DIRECTION;
			arg_present[n] = true;
		}
		
		/* 
			* Update the number of repeats pending for the current command. 
			*/
		public static void set_repeat(int nrepeats)
		{
			Game_Command cmd = cmd_queue[prev_cmd_idx(cmd_tail)];

			cmd.nrepeats = nrepeats;
			if (nrepeats != 0) repeating = true;
			else repeating = false;

			/* Redraw the state (later) */
			Misc.p_ptr.redraw |= (Misc.PR_STATE);
		}


		public void set_arg_item(int n, int item)
		{
			int idx = cmd_idx(command);

			Misc.assert(n <= CMD_MAX_ARGS);
			Misc.assert((game_cmds[idx].arg_type[n] & cmd_arg_type.arg_ITEM) != 0);

			arg[n] = new cmd_arg();
			arg[n].value = item;
			arg_type[n] = cmd_arg_type.arg_ITEM;
			arg_present[n] = true;
		}

	}
}
