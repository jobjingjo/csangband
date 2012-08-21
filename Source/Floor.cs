using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;

namespace CSAngband {
	class Floor {
		/* ------------------------------------------------------------------------
		 * Temporary (hopefully) hackish solutions.
		 * ------------------------------------------------------------------------ */
		public static void check_panel(Game_Event.Event_Type type, Game_Event data, object user)
		{
			Xtra2.verify_panel();
		}

		public static void see_floor_items(Game_Event.Event_Type type, Game_Event data, object user)
		{
			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			int floor_num = 0;
			int[] floor_list = new int[Misc.MAX_FLOOR_STACK + 1];
			bool blind = ((Misc.p_ptr.timed[(int)Timed_Effect.BLIND] != 0) || (Cave.no_light()));

			string p = "see";
			int can_pickup = 0;
			int i;

			/* Scan all marked objects in the grid */
			floor_num = Object.Object.scan_floor(floor_list, floor_list.Length, py, px, 0x03);
			if (floor_num == 0) return;

			for (i = 0; i < floor_num; i++)
			    can_pickup += Object.Object.byid((short)floor_list[i]).inven_carry_okay()?1:0;
	
			/* One object */
			if (floor_num == 1)
			{
			    /* Get the object */
			    Object.Object o_ptr = Object.Object.byid((short)floor_list[0]);
			    //char o_name[80];
				string o_name = null;

			    if (can_pickup == 0)
			        p = "have no room for";
			    else if (blind)
			        p = "feel";

			    /* Describe the object.  Less detail if blind. */
			    if (blind)
			        o_name = o_ptr.object_desc(Object.Object.Detail.PREFIX | Object.Object.Detail.BASE);
			    else
			        o_name = o_ptr.object_desc(Object.Object.Detail.PREFIX | Object.Object.Detail.FULL);

			    /* Message */
			    Utilities.message_flush();
			    Utilities.msg("You {0} {1}.", p, o_name);
			}
			else
			{
			    ui_event e;

			    if (can_pickup == 0)	p = "have no room for the following objects";
			    else if (blind)     p = "feel something on the floor";

				throw new NotImplementedException();
			    /* Display objects on the floor */
				//screen_save();
				//show_floor(floor_list, floor_num, (OLIST_WEIGHT));
				//prt(format("You %s: ", p), 0, 0);

				///* Wait for it.  Use key as next command. */
				//e = inkey_ex();
				//Term_event_push(&e);

				///* Restore screen */
				//screen_load();
			}
		}
	}
}
