using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Map {
		/* ------------------------------------------------------------------------
		 * Map redraw.
		 * ------------------------------------------------------------------------ */
		//#if 0
		//static void trace_map_updates(game_event_type type, game_event_data *data, void *user)
		//{
		//    if (data.point.x == -1 && data.point.y == -1)
		//    {
		//        printf("Redraw whole map\n");
		//    }
		//    else
		//    {
		//        printf("Redraw (%i, %i)\n", data.point.x, data.point.y);
		//    }
		//}
		//#endif

		public static void update_maps(Game_Event.Event_Type type, Game_Event data, object user)
		{
		    Term t = user as Term;

		    /* This signals a whole-map redraw. */
		    if (data.point.x == -1 && data.point.y == -1)
		    {
		        Cave.prt_map();
		    }
		    /* Single point to be redrawn */
		    else
		    {
		        Grid_Data g = new Grid_Data();
		        ConsoleColor a = ConsoleColor.White, ta = ConsoleColor.White;
		        char c = ' ', tc = ' ';
		
		        int ky, kx;
		        int vy, vx;
		
		        /* Location relative to panel */
		        ky = data.point.y - t.offset_y;
		        kx = data.point.x - t.offset_x;

		        if (t == Misc.angband_term[0])
		        {
		            /* Verify location */
		            if ((ky < 0) || (ky >= Misc.SCREEN_HGT)) return;
			
		            /* Verify location */
		            if ((kx < 0) || (kx >= Misc.SCREEN_WID)) return;
			
		            /* Location in window */
		            vy = ky + Misc.ROW_MAP;
		            vx = kx + Misc.COL_MAP;

		              if (Term.tile_width > 1)
		              {
		                  vx += (Term.tile_width - 1) * kx;
		              }
		              if (Term.tile_height > 1)
		              {
		                  vy += (Term.tile_height - 1) * ky;
		              }
		        }
		        else
		        {
		            if (Term.tile_width > 1)
		            {
		                    kx += (Term.tile_width - 1) * kx;
		            }
		            if (Term.tile_height > 1)
		            {
		                    ky += (Term.tile_height - 1) * ky;
		            }
			
		            /* Verify location */
		            if ((ky < 0) || (ky >= t.hgt)) return;
		            if ((kx < 0) || (kx >= t.wid)) return;
			
		            /* Location in window */
		            vy = ky;
		            vx = kx;
		        }

		
		        /* Redraw the grid spot */
		        Cave.map_info(data.point.y, data.point.x, ref g);
		        Cave.grid_data_as_text(ref g, ref a, ref c, ref ta, ref tc);
		        t.queue_char(vx, vy, a, c, ta, tc);
				//#if 0
				//        /* Plot 'spot' updates in light green to make them visible */
				//        Term_queue_char(t, vx, vy, TERM_L_GREEN, c, ta, tc);
				//#endif
		
		        if ((Term.tile_width > 1) || (Term.tile_height > 1))
		        {
		                t.big_queue_char(vx, vy, a, c, ConsoleColor.White, ' ');
		        }
		    }
		}
	}
}
