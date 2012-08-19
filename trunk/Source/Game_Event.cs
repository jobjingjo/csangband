using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Game_Event {

		//All this was unioned together
		public Loc point;
		
		public string text;

		public bool flag;

		//Enable these two later
		//I think these two are located in Birther.cs, look there before enabling these...
		public class birthstage_info
		{
			public bool reset;
			public string hint;
			public int n_choices;
			public int initial_choice;
			public string[] choices;
			public string[] helptexts;
			public object xtra;
		} 
		
		public birthstage_info birthstage;

  		public class birthstats_info
		{
			public int[] stats;
			public int remaining;
		}
		public birthstats_info birthstats;
		//End union


		//all instances of user input parameter was void*
		/* 
		 * A function called when a game event occurs - these are registered to be
		 * called by event_add_handler or event_add_handler_set, and deregistered
		 * when they should no longer be called through event_remove_handler or
		 * event_remove_handler_set.
		 */
		public delegate void Handler(Event_Type type, Game_Event data, object user);

		class event_handler_entry
		{
			public event_handler_entry next;
			public Handler fn;
			public object user;
		}

		//it was an array of pointers... this might be sufficient
		static event_handler_entry[] event_handlers = new event_handler_entry[N_GAME_EVENTS];

		static void dispatch(Event_Type type, Game_Event data)
		{
			event_handler_entry t = event_handlers[(int)type];

			/* 
			 * Send the word out to all interested event handlers.
			 */
			while (t != null)
			{
				/* Call the handler with the relevant data */
				t.fn(type, data, t.user);
				t = t.next;
			}
		}

		public static void add_handler(Event_Type type, Handler fn, object user)
		{
			event_handler_entry e;

			Misc.assert(fn != null);

			/* Make a new entry */
			e = new event_handler_entry();
			e.fn = fn;
			e.user = user;

			/* Add it to the head of the appropriate list */
			e.next = event_handlers[(int)type];
			event_handlers[(int)type] = e;
		}

		public static void remove_handler(Event_Type type, Handler fn, object user)
		{
			event_handler_entry prev = null;
			event_handler_entry curr = event_handlers[(int)type];

			/* Look for the entry in the list */
			while (curr != null)
			{
			    /* Check if this is the entry we want to remove */
			    if (curr.fn == fn && curr.user == user)
			    {
			        if (prev == null)
			        {
			            event_handlers[(int)type] = curr.next;
			        }
			        else
			        {
			            prev.next = curr.next;
			        }

			        //mem_free(curr);
			        return;
			    }

			    prev = curr;
			    curr = curr.next;
			}
		}

		public static void remove_all_handlers()
		{
			throw new NotImplementedException();
			//int type;
			//struct event_handler_entry *handler, *next;

			//for (type = 0; type < N_GAME_EVENTS; type++) {
			//    handler = event_handlers[type];
			//    while (handler) {
			//        next = handler.next;
			//        mem_free(handler);
			//        handler = next;
			//    }
			//    event_handlers[type] = null;
			//}
		}

		//n_types was a size_t
		public static void add_handler_set(Event_Type[] type, Handler fn, object user)
		{
			for (int i = 0; i < type.Length; i++)
			    add_handler(type[i], fn, user);
		}

		//n_types was a size_t
		public static void remove_handler_set(Event_Type type, int n_types, Handler fn, object user)
		{
			throw new NotImplementedException();
			//size_t i;

			//for (i = 0; i < n_types; i++)
			//    event_remove_handler(type[i], fn, user);
		}




		public static void signal(Event_Type type)
		{
			dispatch(type, null);
		}

		public static void signal_flag(Event_Type type, bool flag)
		{
			Game_Event data = new Game_Event();
			data.flag = flag;

			dispatch(type, data);
		}


		public static void signal_point(Event_Type type, int x, int y)
		{
			Game_Event data = new Game_Event();
			data.point = new Loc(x, y);
			//data.point.x = x;
			//data.point.y = y;

			Game_Event.dispatch(type, data);
		}


		public static void signal_string(Event_Type type, string s)
		{
			Game_Event data = new Game_Event();
			data.text = s;

			dispatch(type, data);
		}

		//stats is size 6
		public static void signal_birthpoints(int[] stats, int remaining)
		{
			Game_Event data = new Game_Event();
			data.birthstats = new birthstats_info();

			data.birthstats.stats = stats;
			data.birthstats.remaining = remaining;

			dispatch(Event_Type.BIRTHPOINTS, data);
		}


		/* The various events we can send signals about. */
		public enum Event_Type
		{
			MAP = 0,		/* Some part of the map has changed. */

			STATS,  		/* One or more of the stats. */
			HP,	   	/* HP or MaxHP. */
			MANA,		/* Mana or MaxMana. */
			AC,		/* Armour Class. */
			EXPERIENCE,	/* Experience or MaxExperience. */
			PLAYERLEVEL,	/* Player's level has changed */
			PLAYERTITLE,	/* Player's title has changed */
			GOLD,		/* Player's gold amount. */
			MONSTERHEALTH,	/* Observed monster's health level. */
			DUNGEONLEVEL,	/* Dungeon depth */
			PLAYERSPEED,	/* Player's speed */
			RACE_CLASS,	/* Race or Class */
			STUDYSTATUS,	/* "Study" availability */
			STATUS,		/* Status */
			DETECTIONSTATUS,	/* Trap detection status */
			STATE,		/* The three 'R's: Resting, Repeating and
						   Searching */
			MOUSEBUTTONS,     /* Displayed mouse buttons need changing */

			PLAYERMOVED,
			SEEFLOOR,         /* When the player would "see" floor objects */

			INVENTORY,
			EQUIPMENT,
			ITEMLIST,
			MONSTERLIST,
			MONSTERTARGET,
			OBJECTTARGET,
			MESSAGE,
			
			INITSTATUS,	/* New status message for initialisation */
			BIRTHPOINTS,	/* Change in the birth points */

			/* Changing of the game state/context. */
			ENTER_INIT,
			LEAVE_INIT,
			ENTER_BIRTH,
			LEAVE_BIRTH,
			ENTER_GAME,
			LEAVE_GAME,
			ENTER_STORE,
			LEAVE_STORE,
			ENTER_DEATH,
			LEAVE_DEATH,

			END  /* Can be sent at the end of a series of events */
		}

		public const int N_GAME_EVENTS = (int)Event_Type.END + 1;
	}
}
