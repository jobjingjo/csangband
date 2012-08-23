using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Monster;
using CSAngband.Object;
using CSAngband.Player;

namespace CSAngband {
	partial class Cave {
		/*
		 * Special cave grid flags
		 */
		public const byte CAVE_MARK	=	0x01; 	/* memorized feature */
		public const byte CAVE_GLOW	=	0x02; 	/* self-illuminating */
		public const byte CAVE_ICKY	=	0x04; 	/* part of a vault */
		public const byte CAVE_ROOM	=	0x08; 	/* part of a room */
		public const byte CAVE_SEEN	=	0x10; 	/* seen flag */
		public const byte CAVE_VIEW	=	0x20; 	/* view flag */
		public const byte CAVE_TEMP	=	0x40; 	/* temp flag */
		public const byte CAVE_WALL	=	0x80; 	/* wall flag */

		public const byte CAVE2_DTRAP	=	0x01;	/* trap detected grid */
		public const byte CAVE2_FEEL	=	0x02;	/* hidden points to trigger feelings*/

		/*** Feature Indexes (see "lib/edit/terrain.txt") ***/

		/* Nothing */
		public const int FEAT_NONE =0x00;

		/* Various */
		public const int FEAT_FLOOR= 0x01;
		public const int FEAT_INVIS= 0x02;
		public const int FEAT_GLYPH =0x03;
		public const int FEAT_OPEN =0x04;
		public const int FEAT_BROKEN =0x05;
		public const int FEAT_LESS =0x06;
		public const int FEAT_MORE= 0x07;

		/* Shops */
		public const int FEAT_SHOP_HEAD= 0x08;
		public const int FEAT_SHOP_TAIL =0x0F;

		/* Traps */
		public const int FEAT_TRAP_HEAD= 0x10;
		public const int FEAT_TRAP_TAIL= 0x1F;

		/* Doors */
		public const int FEAT_DOOR_HEAD= 0x20;
		public const int FEAT_DOOR_TAIL= 0x2F;

		/* Secret door */
		public const int FEAT_SECRET= 0x30;

		/* Rubble */
		public const int FEAT_RUBBLE= 0x31;

		/* Mineral seams */
		public const int FEAT_MAGMA= 0x32;
		public const int FEAT_QUARTZ =0x33;
		public const int FEAT_MAGMA_H= 0x34;
		public const int FEAT_QUARTZ_H =0x35;
		public const int FEAT_MAGMA_K= 0x36;
		public const int FEAT_QUARTZ_K =0x37;

		/* Walls */
		public const int FEAT_WALL_EXTRA= 0x38;
		public const int FEAT_WALL_INNER= 0x39;
		public const int FEAT_WALL_OUTER= 0x3A;
		public const int FEAT_WALL_SOLID= 0x3B;
		public const int FEAT_PERM_EXTRA= 0x3C;
		public const int FEAT_PERM_INNER =0x3D;
		public const int FEAT_PERM_OUTER =0x3E;
		public const int FEAT_PERM_SOLID= 0x3F;
			
		public static Cave cave = null;
		/*
		 * Number of grids in each dungeon (vertically)
		 * Must be a multiple of SCREEN_HGT
		 * Must be less or equal to 256
		 */
		public const int DUNGEON_HGT = 66;

		/*
		 * Number of grids in each dungeon (horizontally)
		 * Must be a multiple of SCREEN_WID
		 * Must be less or equal to 256
		 */
		public const int DUNGEON_WID = 198;

		//Aparently this instance reference was temporary while he refactored. 
		//We should figure out how to remove it.
		public static Cave instance;

		public int created_at;
		public int depth;

		public byte feeling;
		public uint obj_rating;
		public uint mon_rating;
		public bool good_item;

		public int height;
		public int width;
	
		public ushort feeling_squares; /* Keep track of how many feeling squares the player has visited */

		public short[][] info; //The two infos were byte, but fuck the ~ symbol in C#.
		public short[][] info2;//If wonky happens, maybe try out char instead?
		public byte[][] feat;
		public byte[][] cost;
		public byte[][] when;
		public short[][]m_idx;
		public short[][]o_idx;

		public Monster.Monster[] monsters;
		public int mon_max;
		public int mon_cnt;

		/*
		 * Determines if a map location is "meaningful"
		 */
		public static bool in_bounds(int Y, int X){
			return (((Y) < (DUNGEON_HGT)) &&
			 ((X) < (DUNGEON_WID)));
		}

		/*
		 * Determine if a "legal" grid is an "empty" floor grid
		 *
		 * Line 1 -- forbid doors, rubble, seams, walls
		 * Line 2 -- forbid player/monsters
		 */
		public static bool cave_empty_bold(int Y, int X){
			return (cave_floor_bold(Y,X) &&
			 (cave.m_idx[Y][X] == 0));
		}

		/*
		 * Determine if a "legal" grid is a "floor" grid
		 *
		 * Line 1 -- forbid doors, rubble, seams, walls
		 *
		 * Note the use of the new "CAVE_WALL" flag.
		 */
		public static bool cave_floor_bold(int Y, int X){
			return ((cave.info[Y][X] & (CAVE_WALL)) == 0);
		}

		/*
		 * Determine if a "legal" grid is within "los" of the player
		 *
		 * Note the use of comparison to zero to force a "boolean" result
		 */
		public static bool player_has_los_bold(int Y, int X) {
			return ((cave.info[Y][X] & (CAVE_VIEW)) != 0);
		}

		/*
		 * Determine if a "legal" grid can be "seen" by the player
		 *
		 * Note the use of comparison to zero to force a "boolean" result
		 */
		public static bool player_can_see_bold(int Y, int X){
			return ((cave.info[Y][X] & (CAVE_SEEN)) != 0);
		}

		/**
		 * cave_predicate is a function pointer which tests a given square to
		 * see if the predicate in question is true.
		 */
		public delegate bool cave_predicate_func(Cave c, int y, int x);
		public cave_predicate_func cave_predicate;

		public Cave() {
			info = new short[DUNGEON_HGT][];
			info2 = new short[DUNGEON_HGT][];
			feat = new byte[DUNGEON_HGT][];
			cost = new byte[DUNGEON_HGT][];
			when = new byte[DUNGEON_HGT][];
			m_idx = new short[DUNGEON_HGT][];
			o_idx = new short[DUNGEON_HGT][];

			//Note: I might be able to just do a square array... but then you can't ref individual rows
			//Look into this...
			for(int i = 0; i < DUNGEON_HGT; i++){
				info[i] = new short[256];
				info2[i] = new short[256];
				feat[i] = new byte[DUNGEON_WID];
				cost[i] = new byte[DUNGEON_WID];
				when[i] = new byte[DUNGEON_WID];
				m_idx[i] = new short[DUNGEON_WID];
				o_idx[i] = new short[DUNGEON_WID];
			}
			

			monsters = new Monster.Monster[Misc.z_info.m_max];
			mon_max = 1;

			created_at = 1;
		}

		~Cave() {
			/*mem_free(c.info);
			mem_free(c.info2);
			mem_free(c.feat);
			mem_free(c.cost);
			mem_free(c.when);
			mem_free(c.m_idx);
			mem_free(c.o_idx);
			mem_free(c.monsters);
			mem_free(c);*/
		}

		//Helper function used for sorting... if it is used only once, it should be an anonymous delegate...
		static int cmp_longs(long a, long b)
		{
		    long x = a;
		    long y = b;

		    if (x < y)
		        return -1;
		    if (x > y)
		        return 1;

		    return 0;
		}

		/*
		 * Save a slope
		 */
		static void vinfo_init_aux(vinfo_hack hack, int y, int x, long m)
		{
		    int i;

		    /* Handle "legal" slopes */
		    if ((m > 0) && (m <= SCALE))
		    {
		        /* Look for that slope */
		        for (i = 0; i < hack.num_slopes; i++)
		        {
		            if (hack.slopes[i] == m) break;
		        }

		        /* New slope */
		        if (i == hack.num_slopes)
		        {
		            /* Paranoia */
		            if (hack.num_slopes >= VINFO_MAX_SLOPES)
		            {
		                Utilities.quit("Too many slopes (" + VINFO_MAX_SLOPES +")!");
		            }

		            /* Save the slope, and advance */
		            hack.slopes[hack.num_slopes++] = m;
		        }
		    }

		    /* Track slope range */
		    if (hack.slopes_min[y,x] > m) hack.slopes_min[y,x] = m;
		    if (hack.slopes_max[y,x] < m) hack.slopes_max[y,x] = m;
		}

		/*
		 * Convert a "location" (Y,X) into a "grid" (G)
		 */
		static int GRID(int Y, int X){
			return (256 * (Y) + (X));
		}

		/*
		 * Convert a "grid" (G) into a "location" (Y)
		 */
		static int GRID_Y(int G){
			return ((int)((G) / 256U));
		}

		/*
		 * Convert a "grid" (G) into a "location" (X)
		 */
		static int GRID_X(int G) {
			return ((int)((G) % 256U));
		}

		/*
		 * Initialize the "vinfo" array
		 *
		 * Full Octagon (radius 20), Grids=1149
		 *
		 * Quadrant (south east), Grids=308, Slopes=251
		 *
		 * Octant (east then south), Grids=161, Slopes=126
		 *
		 * This function assumes that VINFO_MAX_GRIDS and VINFO_MAX_SLOPES
		 * have the correct values, which can be derived by setting them to
		 * a number which is too high, running this function, and using the
		 * error messages to obtain the correct values.
		 */
		public static int vinfo_init()
		{
			int i, g;
			long m;	

			int num_grids = 0;

			int queue_head = 0;
			int queue_tail = 0;
			vinfo_type[] queue = new vinfo_type[VINFO_MAX_GRIDS*2];

			//Nick: initialize v_info here, fill it with new stuffs to play with instead of nulls
			for (int n = 0; n < vinfo.Count(); n++){
				vinfo[n] = new vinfo_type();
			}

			/* Make hack */
			vinfo_hack hack = new vinfo_hack();

			/* Analyze grids */
			for (int y = 0; y <= Misc.MAX_SIGHT; ++y)
			{
				//Triangular array? Weird... Triangle is on top right corner
			    for (int x = y; x <= Misc.MAX_SIGHT; ++x)
			    {
			        /* Skip grids which are out of sight range */
			        if (distance(0, 0, y, x) > Misc.MAX_SIGHT) continue;

			        /* Default slope range */
			        hack.slopes_min[y,x] = 999999999;
			        hack.slopes_max[y,x] = 0;

			        /* Paranoia */
			        if (num_grids >= VINFO_MAX_GRIDS)
			        {
						Utilities.quit("Too many grids (" + num_grids + " >= " + VINFO_MAX_GRIDS + ")!");
			        }

			        /* Count grids */
			        num_grids++;

			        /* Slope to the top right corner */
			        m = SCALE * (1000L * y - 500) / (1000L * x + 500);

			        /* Handle "legal" slopes */
			        vinfo_init_aux(hack, y, x, m);

			        /* Slope to top left corner */
			        m = SCALE * (1000L * y - 500) / (1000L * x - 500);

			        /* Handle "legal" slopes */
			        vinfo_init_aux(hack, y, x, m);

			        /* Slope to bottom right corner */
			        m = SCALE * (1000L * y + 500) / (1000L * x + 500);

			        /* Handle "legal" slopes */
			        vinfo_init_aux(hack, y, x, m);

			        /* Slope to bottom left corner */
			        m = SCALE * (1000L * y + 500) / (1000L * x - 500);

			        /* Handle "legal" slopes */
			        vinfo_init_aux(hack, y, x, m);
			    }
			}


			/* Enforce maximal efficiency */
			if (num_grids < VINFO_MAX_GRIDS)
			{
				Utilities.quit("Too few grids (" + num_grids + " < " + VINFO_MAX_GRIDS + ")!");
			}

			/* Enforce maximal efficiency */
			if (hack.num_slopes < VINFO_MAX_SLOPES)
			{
				Utilities.quit("Too few slopes (" + hack.num_slopes + " < " + VINFO_MAX_SLOPES + ")!");
			}

			Utilities.sort(hack.slopes, cmp_longs);

			/* Enqueue player grid */
			queue[queue_tail++] = vinfo[0];

			/* Process queue */
			while (queue_head < queue_tail)
			{
			    int e;


			    /* Index */
			    e = queue_head++;

			    /* Main Grid */
			    g = vinfo[e].grid[0];

			    /* Location */
			    int y = GRID_Y(g);
			    int x = GRID_X(g);


			    /* Compute grid offsets */
			    vinfo[e].grid[0] = (short)GRID(+y,+x);
			    vinfo[e].grid[1] = (short)GRID(+x,+y);
			    vinfo[e].grid[2] = (short)GRID(+x,-y);
			    vinfo[e].grid[3] = (short)GRID(+y,-x);
			    vinfo[e].grid[4] = (short)GRID(-y,-x);
			    vinfo[e].grid[5] = (short)GRID(-x,-y);
			    vinfo[e].grid[6] = (short)GRID(-x,+y);
			    vinfo[e].grid[7] = (short)GRID(-y,+x);


			    /* Analyze slopes */
			    for (i = 0; i < hack.num_slopes; ++i)
			    {
			        m = hack.slopes[i];

			        /* Memorize intersection slopes (for non-player-grids) */
			        if ((e > 0) && (hack.slopes_min[y,x] < m) && (m < hack.slopes_max[y,x]))
			        {
			            switch (i / 32)
			            {
			                case 3: vinfo[e].bits_3 |= (uint)(1L << (i % 32)); break;
			                case 2: vinfo[e].bits_2 |= (uint)(1L << (i % 32)); break;
			                case 1: vinfo[e].bits_1 |= (uint)(1L << (i % 32)); break;
			                case 0: vinfo[e].bits_0 |= (uint)(1L << (i % 32)); break;
			            }
			        }
			    }


			    /* Default */
			    vinfo[e].next_0 = vinfo[0];

			    /* Grid next child */
			    if (distance(0, 0, y, x+1) <= Misc.MAX_SIGHT)
			    {
			        g = GRID(y,x+1);

			        if (queue[queue_tail-1].grid[0] != g)
			        {
			            vinfo[queue_tail].grid[0] = (short)g;
			            queue[queue_tail] = vinfo[queue_tail];
			            queue_tail++;
			        }

			        vinfo[e].next_0 = vinfo[queue_tail - 1];
			    }


			    /* Default */
			    vinfo[e].next_1 = vinfo[0];

			    /* Grid diag child */
			    if (distance(0, 0, y+1, x+1) <= Misc.MAX_SIGHT)
			    {
			        g = GRID(y+1,x+1);

			        if (queue[queue_tail-1].grid[0] != g)
			        {
			            vinfo[queue_tail].grid[0] = (short)g;
			            queue[queue_tail] = vinfo[queue_tail];
			            queue_tail++;
			        }

			        vinfo[e].next_1 = vinfo[queue_tail - 1];
			    }


			    /* Hack -- main diagonal has special children */
			    if (y == x) vinfo[e].next_0 = vinfo[e].next_1;


			    /* Extra values */
			    vinfo[e].y = (byte)y;
			    vinfo[e].x = (byte)x;
			    vinfo[e].d = (byte)((y > x) ? (y + x/2) : (x + y/2));
			    vinfo[e].r = (byte)((y==0) ? x : (x==0) ? y : (y == x) ? y : 0);
			}


			/* Verify maximal bits XXX XXX XXX */
			if (((vinfo[1].bits_3 | vinfo[2].bits_3) != VINFO_BITS_3) ||
			    ((vinfo[1].bits_2 | vinfo[2].bits_2) != VINFO_BITS_2) ||
			    ((vinfo[1].bits_1 | vinfo[2].bits_1) != VINFO_BITS_1) ||
			    ((vinfo[1].bits_0 | vinfo[2].bits_0) != VINFO_BITS_0))
			{
			    Utilities.quit("Incorrect bit masks!");
			}


			/* Kill hack */
			//FREE(hack); //lolc


			/* Success */
			return (0);
		}



		/*
		 * Forget the "CAVE_VIEW" grids, redrawing as needed
		 */
		public static void forget_view()
		{
			int i, g;

			int fast_view_n = view_n;
			ushort[] fast_view_g = view_g;

			/* XXX: this is moronic. It's not 'fast'. */
			//byte *fast_cave_info = &cave.info[0][0];
			//Stopped using the pointer, so this code might be buggy...

			/* None to forget */
			if (fast_view_n == 0) return;

			/* Clear them all */
			for (i = 0; i < fast_view_n; i++)
			{
			    int y, x;

			    /* Grid */
			    g = fast_view_g[i];

			    /* Location */
			    y = GRID_Y(g);
			    x = GRID_X(g);

			    /* Clear "CAVE_VIEW" and "CAVE_SEEN" flags */
			    //fast_cave_info[g] &= ~(CAVE_VIEW | CAVE_SEEN);
				cave.info[y][x] &= ~(CAVE_VIEW | CAVE_SEEN);

			    /* Clear "CAVE_LIGHT" flag */
			    /* fast_cave.info[g] &= ~(CAVE_LIGHT); */

			    /* Redraw */
			    cave_light_spot(cave, y, x);
			}

			/* None left */
			fast_view_n = 0;


			/* Save 'view_n' */
			view_n = fast_view_n;
		}



		/*
		 * Calculate the complete field of view using a new algorithm
		 *
		 * If "view_g" and "temp_g" were global pointers to arrays of grids, as
		 * opposed to actual arrays of grids, then we could be more efficient by
		 * using "pointer swapping".
		 *
		 * Note the following idiom, which is used in the function below.
		 * This idiom processes each "octant" of the field of view, in a
		 * clockwise manner, starting with the east strip, south side,
		 * and for each octant, allows a simple calculation to set "g"
		 * equal to the proper grids, relative to "pg", in the octant.
		 *
		 *   for (o2 = 0; o2 < 8; o2++)
		 *   ...
		 *         g = pg + p.grid[o2];
		 *   ...
		 *
		 *
		 * Normally, vision along the major axes is more likely than vision
		 * along the diagonal axes, so we check the bits corresponding to
		 * the lines of sight near the major axes first.
		 *
		 * We use the "temp_g" array (and the "CAVE_TEMP" flag) to keep track of
		 * which grids were previously marked "CAVE_SEEN", since only those grids
		 * whose "CAVE_SEEN" value changes during this routine must be redrawn.
		 *
		 * This function is now responsible for maintaining the "CAVE_SEEN"
		 * flags as well as the "CAVE_VIEW" flags, which is good, because
		 * the only grids which normally need to be memorized and/or redrawn
		 * are the ones whose "CAVE_SEEN" flag changes during this routine.
		 *
		 * Basically, this function divides the "octagon of view" into octants of
		 * grids (where grids on the main axes and diagonal axes are "shared" by
		 * two octants), and processes each octant one at a time, processing each
		 * octant one grid at a time, processing only those grids which "might" be
		 * viewable, and setting the "CAVE_VIEW" flag for each grid for which there
		 * is an (unobstructed) line of sight from the center of the player grid to
		 * any internal point in the grid (and collecting these "CAVE_VIEW" grids
		 * into the "view_g" array), and setting the "CAVE_SEEN" flag for the grid
		 * if, in addition, the grid is "illuminated" in some way.
		 *
		 * This function relies on a theorem (suggested and proven by Mat Hostetter)
		 * which states that in each octant of a field of view, a given grid will
		 * be "intersected" by one or more unobstructed "lines of sight" from the
		 * center of the player grid if and only if it is "intersected" by at least
		 * one such unobstructed "line of sight" which passes directly through some
		 * corner of some grid in the octant which is not shared by any other octant.
		 * The proof is based on the fact that there are at least three significant
		 * lines of sight involving any non-shared grid in any octant, one which
		 * intersects the grid and passes though the corner of the grid closest to
		 * the player, and two which "brush" the grid, passing through the "outer"
		 * corners of the grid, and that any line of sight which intersects a grid
		 * without passing through the corner of a grid in the octant can be "slid"
		 * slowly towards the corner of the grid closest to the player, until it
		 * either reaches it or until it brushes the corner of another grid which
		 * is closer to the player, and in either case, the existanc of a suitable
		 * line of sight is thus demonstrated.
		 *
		 * It turns out that in each octant of the radius 20 "octagon of view",
		 * there are 161 grids (with 128 not shared by any other octant), and there
		 * are exactly 126 distinct "lines of sight" passing from the center of the
		 * player grid through any corner of any non-shared grid in the octant.  To
		 * determine if a grid is "viewable" by the player, therefore, you need to
		 * simply show that one of these 126 lines of sight intersects the grid but
		 * does not intersect any wall grid closer to the player.  So we simply use
		 * a bit vector with 126 bits to represent the set of interesting lines of
		 * sight which have not yet been obstructed by wall grids, and then we scan
		 * all the grids in the octant, moving outwards from the player grid.  For
		 * each grid, if any of the lines of sight which intersect that grid have not
		 * yet been obstructed, then the grid is viewable.  Furthermore, if the grid
		 * is a wall grid, then all of the lines of sight which intersect the grid
		 * should be marked as obstructed for future reference.  Also, we only need
		 * to check those grids for whom at least one of the "parents" was a viewable
		 * non-wall grid, where the parents include the two grids touching the grid
		 * but closer to the player grid (one adjacent, and one diagonal).  For the
		 * bit vector, we simply use 4 32-bit integers.  All of the static values
		 * which are needed by this function are stored in the large "vinfo" array
		 * (above), which is machine generated by another program.  XXX XXX XXX
		 *
		 * Hack -- The queue must be able to hold more than VINFO_MAX_GRIDS grids
		 * because the grids at the edge of the field of view use "grid zero" as
		 * their children, and the queue must be able to hold several of these
		 * special grids.  Because the actual number of required grids is bizarre,
		 * we simply allocate twice as many as we would normally need.  XXX XXX XXX
		 */
		public static void update_view()
		{
			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			int pg = GRID(py,px);

			int i, j, k, g, o2;

			int radius;

			int fast_view_n = view_n;
			ushort[] fast_view_g = view_g;

			int fast_temp_n = 0;
			ushort[] fast_temp_g = Misc.temp_g;

			/* XXX: also moronic. Optimizers exist. */
			//byte *fast_cave_info = &cave.info[0][0];
			//lolz, Nick: I am omitting this

			short info;


			/*** Step 0 -- Begin ***/

			/* Save the old "view" grids for later */
			for (i = 0; i < fast_view_n; i++)
			{
			    /* Grid */
			    g = fast_view_g[i];

				int x = GRID_X(g);
				int y = GRID_Y(g);


			    /* Get grid info */
			    //info = fast_cave_info[g];
				info = cave.info[y][x];

			    /* Save "CAVE_SEEN" grids */
			    if ((info & (CAVE_SEEN)) != 0)
			    {
			        /* Set "CAVE_TEMP" flag */
			        info |= (CAVE_TEMP);

					//NICK This is a hack, if we have seen it, we mark it...
					info |= (CAVE_MARK);

			        /* Save grid for later */
			        fast_temp_g[fast_temp_n++] = (ushort)g;
			    }

			    /* Clear "CAVE_VIEW" and "CAVE_SEEN" flags */
			    info &= ~(CAVE_VIEW | CAVE_SEEN);

			    /* Clear "CAVE_LIGHT" flag */
			    /* info &= ~(CAVE_LIGHT); */

			    /* Save cave info */
			    //fast_cave_info[g] = info;
				cave.info[y][x] = info;
			}

			/* Reset the "view" array */
			fast_view_n = 0;

			/* Extract "radius" value */
			radius = Misc.p_ptr.cur_light;

			/* Handle real light */
			if (radius > 0) ++radius;

			/* Scan monster list and add monster lights */
			for (k = 1; k < Misc.z_info.m_max; k++)
			{
			    /* Check the k'th monster */
			    Monster.Monster m_ptr = cave_monster(cave, k);
				if(m_ptr == null)
					continue; //TODO: See why there were null monsters in the cave?
			    Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			    /* Access the location */
			    int fx = m_ptr.fx;
			    int fy = m_ptr.fy;

			    bool in_los = los(Misc.p_ptr.py, Misc.p_ptr.px, fy, fx);

			    /* Skip dead monsters */
			    if (m_ptr.r_idx == 0) continue;

			    /* Skip monsters not carrying light */
			    if (!r_ptr.flags.has(Monster_Flag.HAS_LIGHT.value)) continue;

			    /* Light a 3x3 box centered on the monster */
			    for (i = -1; i <= 1; i++)
			    {
			        for (j = -1; j <= 1; j++)
			        {
			            int sy = fy + i;
			            int sx = fx + j;
				
			            /* If the monster isn't visible we can only light open tiles */
			            if (!in_los && !cave_floor_bold(sy, sx))
			                continue;

			            /* If the tile is too far away we won't light it */
			            if (distance(Misc.p_ptr.py, Misc.p_ptr.px, sy, sx) > Misc.MAX_SIGHT)
			                continue;
				
			            /* If the tile itself isn't in LOS, don't light it */
			            if (!los(Misc.p_ptr.py, Misc.p_ptr.px, sy, sx))
			                continue;
				
			            g = GRID(sy, sx);

			            /* Mark the square lit and seen */
			            //fast_cave_info[g] |= (CAVE_VIEW | CAVE_SEEN);
						cave.info[sy][sx] |= (CAVE_VIEW | CAVE_SEEN);
				
			            /* Save in array */
			            fast_view_g[fast_view_n++] = (ushort)g;
			        }
			    }
			}


			/*** Step 1 -- player grid ***/

			/* Player grid */
			g = pg;

			/* Get grid info */
			//info = fast_cave_info[g];
			int tx = GRID_X(g);
			int ty = GRID_Y(g);

			info = cave.info[ty][tx];

			/* Assume viewable */
			info |= (CAVE_VIEW);

			/* Torch-lit grid */
			if (0 < radius)
			{
			    /* Mark as "CAVE_SEEN" */
			    info |= (CAVE_SEEN);

			    /* Mark as "CAVE_LIGHT" */
			    /* info |= (CAVE_LIGHT); */
			}

			/* Perma-lit grid */
			else if ((info & (CAVE_GLOW)) != 0)
			{
			    /* Mark as "CAVE_SEEN" */
			    info |= (CAVE_SEEN);
			}

			/* Save cave info */
			//fast_cave_info[g] = info;
			cave.info[GRID_Y(g)][GRID_X(g)] = info;

			/* Save in array */
			fast_view_g[fast_view_n++] = (ushort)g;


			/*** Step 2 -- octants ***/

			/* Scan each octant */
			for (o2 = 0; o2 < 8; o2++)
			{
			    vinfo_type p;

			    /* Last added */
			    vinfo_type last = vinfo[0];

			    /* Grid queue */
			    int queue_head = 0;
			    int queue_tail = 0;
			    vinfo_type[] queue = new vinfo_type[VINFO_MAX_GRIDS*2];
				for(int q = 0; q < queue.Length; q++) {
					queue[q] = new vinfo_type(); //Gotta do this to avoid null references...
				}

			    /* Slope bit vector */
			    uint bits0 = (uint)VINFO_BITS_0;
			    uint bits1 = (uint)VINFO_BITS_1;
			    uint bits2 = (uint)VINFO_BITS_2;
			    uint bits3 = (uint)VINFO_BITS_3;

			    /* Reset queue */
			    queue_head = queue_tail = 0;

			    /* Initial grids */
			    queue[queue_tail++] = vinfo[1];
			    queue[queue_tail++] = vinfo[2];

			    /* Process queue */
			    while (queue_head < queue_tail)
			    {
			        /* Dequeue next grid */
			        p = queue[queue_head++];

			        /* Check bits */
			        if ((bits0 & (p.bits_0)) != 0 ||
			            (bits1 & (p.bits_1)) != 0 ||
			            (bits2 & (p.bits_2)) != 0 ||
			            (bits3 & (p.bits_3)) != 0)
			        {
			            /* Extract grid value XXX XXX XXX */
			            g = pg + p.grid[o2];

			            /* Get grid info */
			            //info = fast_cave_info[g];
						info = cave.info[GRID_Y(g)][GRID_X(g)];

			            /* Handle wall */
			            if ((info & (CAVE_WALL)) != 0)
			            {
			                /* Clear bits */
			                bits0 &= ~(p.bits_0);
			                bits1 &= ~(p.bits_1);
			                bits2 &= ~(p.bits_2);
			                bits3 &= ~(p.bits_3);

			                /* Newly viewable wall */
			                if ((info & (CAVE_VIEW)) == 0)
			                {
			                    /* Mark as viewable */
			                    info |= (CAVE_VIEW);

			                    /* Torch-lit grids */
			                    if (p.d < radius)
			                    {
			                        /* Mark as "CAVE_SEEN" */
			                        info |= (CAVE_SEEN);

			                        /* Mark as "CAVE_LIGHT" */
			                        /* info |= (CAVE_LIGHT); */
			                    }

			                    /* Perma-lit grids */
			                    else if ((info & (CAVE_GLOW)) != 0)
			                    {
			                        int y = GRID_Y(g);
			                        int x = GRID_X(g);

			                        /* Hack -- move towards player */
			                        int yy = (y < py) ? (y + 1) : (y > py) ? (y - 1) : y;
			                        int xx = (x < px) ? (x + 1) : (x > px) ? (x - 1) : x;

			                        /* Check for "simple" illumination */
			                        if ((cave.info[yy][xx] & (CAVE_GLOW)) != 0)
			                        {
			                            /* Mark as seen */
			                            info |= (CAVE_SEEN);
			                        }
			                    }

			                    /* Save cave info */
			                    //fast_cave_info[g] = info;
								cave.info[GRID_Y(g)][GRID_X(g)] = info;

			                    /* Save in array */
			                    fast_view_g[fast_view_n++] = (ushort)g;
			                }
			            }

			            /* Handle non-wall */
			            else
			            {
			                /* Enqueue child */
			                if (last != p.next_0)
			                {
			                    queue[queue_tail++] = last = p.next_0;
			                }

			                /* Enqueue child */
			                if (last != p.next_1)
			                {
			                    queue[queue_tail++] = last = p.next_1;
			                }

			                /* Newly viewable non-wall */
			                if ((info & (CAVE_VIEW)) == 0)
			                {
			                    /* Mark as "viewable" */
			                    info |= (CAVE_VIEW);

			                    /* Torch-lit grids */
			                    if (p.d < radius)
			                    {
			                        /* Mark as "CAVE_SEEN" */
			                        info |= (CAVE_SEEN);

			                        /* Mark as "CAVE_LIGHT" */
			                        /* info |= (CAVE_LIGHT); */
			                    }

			                    /* Perma-lit grids */
			                    else if ((info & (CAVE_GLOW)) != 0)
			                    {
			                        /* Mark as "CAVE_SEEN" */
			                        info |= (CAVE_SEEN);
			                    }

			                    /* Save cave info */
			                    //fast_cave_info[g] = info;
								cave.info[GRID_Y(g)][GRID_X(g)] = info;

			                    /* Save in array */
			                    fast_view_g[fast_view_n++] = (ushort)g;
			                }
			            }
			        }
			    }
			}


			/*** Step 3 -- Complete the algorithm ***/

			/* Handle blindness */
			if (Misc.p_ptr.timed[(int)Timed_Effect.BLIND] != 0)
			{
			    /* Process "new" grids */
			    for (i = 0; i < fast_view_n; i++)
			    {
			        /* Grid */
			        g = fast_view_g[i];

			        /* Grid cannot be "CAVE_SEEN" */
			        //fast_cave_info[g] &= ~(CAVE_SEEN);
					cave.info[GRID_Y(g)][GRID_X(g)] &= ~(CAVE_SEEN);
			    }
			}

			/* Process "new" grids */
			for (i = 0; i < fast_view_n; i++)
			{
			    /* Grid */
			    g = fast_view_g[i];

			    /* Get grid info */
			    //info = fast_cave_info[g];
				info = cave.info[GRID_Y(g)][GRID_X(g)];

			    /* Was not "CAVE_SEEN", is now "CAVE_SEEN" */
			    if (((info & (CAVE_SEEN)) != 0) && ((info & (CAVE_TEMP)) == 0))
			    {
			        int y, x;

			        /* Location */
			        y = GRID_Y(g);
			        x = GRID_X(g);
			
			        /* Handle feeling squares */
			        if ((cave.info2[y][x] & CAVE2_FEEL) != 0)
			        {
			            cave.feeling_squares++;
				
			            /* Erase the square so you can't 'resee' it */
			            cave.info2[y][x] &= ~(CAVE2_FEEL);
			
			            /* Display feeling if necessary */
			            if (cave.feeling_squares == FEELING1)
			                Command.display_feeling(true);
		
			        }
			
			        cave_note_spot(cave, y, x);
			        cave_light_spot(cave, y, x);
			    }
			}

			/* Process "old" grids */
			for (i = 0; i < fast_temp_n; i++)
			{
			    /* Grid */
			    g = fast_temp_g[i];

			    /* Get grid info */
			    //info = fast_cave_info[g];
				info = cave.info[GRID_Y(g)][GRID_X(g)];

			    /* Clear "CAVE_TEMP" flag */
			    info &= ~(CAVE_TEMP);

			    /* Save cave info */
			    //fast_cave_info[g] = info;
				cave.info[GRID_Y(g)][GRID_X(g)] = info;

			    /* Was "CAVE_SEEN", is now not "CAVE_SEEN" */
			    if ((info & (CAVE_SEEN)) == 0)
			    {
			        int y, x;

			        /* Location */
			        y = GRID_Y(g);
			        x = GRID_X(g);

			        /* Redraw */
			        cave_light_spot(cave, y, x);
			    }
			}


			/* Save 'view_n' */
			view_n = fast_view_n;

			//Nick: We might want to save these too? If wonky, comment out below
			view_g = fast_view_g;
			Misc.temp_g = fast_temp_g;
		}




		/*
		 * Size of the circular queue used by "update_flow()"
		 */
		public const int FLOW_MAX = 2048;

		/*
		 * Hack -- provide some "speed" for the "flow" code
		 * This entry is the "current index" for the "when" field
		 * Note that a "when" value of "zero" means "not used".
		 *
		 * Note that the "cost" indexes from 1 to 127 are for
		 * "old" data, and from 128 to 255 are for "new" data.
		 *
		 * This means that as long as the player does not "teleport",
		 * then any monster up to 128 + MONSTER_FLOW_DEPTH will be
		 * able to track down the player, and in general, will be
		 * able to track down either the player or a position recently
		 * occupied by the player.
		 */
		static int flow_save = 0;



		/*
		 * Hack -- forget the "flow" information
		 */
		public static void cave_forget_flow(Cave c)
		{
			int x, y;

			/* Nothing to forget */
			if (flow_save == 0) return;

			/* Check the entire dungeon */
			for (y = 0; y < DUNGEON_HGT; y++)
			{
			    for (x = 0; x < DUNGEON_WID; x++)
			    {
			        /* Forget the old data */
			        c.cost[y][x] = 0;
			        c.when[y][x] = 0;
			    }
			}

			/* Start over */
			flow_save = 0;
		}


		/*
		 * Hack -- fill in the "cost" field of every grid that the player can
		 * "reach" with the number of steps needed to reach that grid.  This
		 * also yields the "distance" of the player from every grid.
		 *
		 * In addition, mark the "when" of the grids that can reach the player
		 * with the incremented value of "flow_save".
		 *
		 * Hack -- use the local "flow_y" and "flow_x" arrays as a "circular
		 * queue" of cave grids.
		 *
		 * We do not need a priority queue because the cost from grid to grid
		 * is always "one" (even along diagonals) and we process them in order.
		 */
		public static void cave_update_flow(Cave c)
		{
			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			int ty, tx;

			int y, x;

			int n, d;

			int flow_n;

			int flow_tail = 0;
			int flow_head = 0;

			byte[] flow_y = new byte[FLOW_MAX];
			byte[] flow_x = new byte[FLOW_MAX];


			/*** Cycle the flow ***/

			/* Cycle the flow */
			if (flow_save++ == 255)
			{
			    /* Cycle the flow */
			    for (y = 0; y < DUNGEON_HGT; y++)
			    {
			        for (x = 0; x < DUNGEON_WID; x++)
			        {
			            int w = c.when[y][x];
			            c.when[y][x] = (byte)((w >= 128) ? (w - 128) : 0);
			        }
			    }

			    /* Restart */
			    flow_save = 128;
			}

			/* Local variable */
			flow_n = flow_save;


			/*** Player Grid ***/

			/* Save the time-stamp */
			c.when[py][px] = (byte)flow_n;

			/* Save the flow cost */
			c.cost[py][px] = 0;

			/* Enqueue that entry */
			flow_y[flow_head] = (byte)py;
			flow_x[flow_head] = (byte)px;

			/* Advance the queue */
			++flow_tail;


			/*** Process Queue ***/

			/* Now process the queue */
			while (flow_head != flow_tail)
			{
			    /* Extract the next entry */
			    ty = flow_y[flow_head];
			    tx = flow_x[flow_head];

			    /* Forget that entry (with wrap) */
			    if (++flow_head == FLOW_MAX) flow_head = 0;

			    /* Child cost */
			    n = c.cost[ty][tx] + 1;

			    /* Hack -- Limit flow depth */
			    if (n == Misc.MONSTER_FLOW_DEPTH) continue;

			    /* Add the "children" */
			    for (d = 0; d < 8; d++)
			    {
			        int old_head = flow_tail;

			        /* Child location */
			        y = ty + Misc.ddy_ddd[d];
			        x = tx + Misc.ddx_ddd[d];

			        /* Ignore "pre-stamped" entries */
			        if (c.when[y][x] == flow_n) continue;

			        /* Ignore "walls" and "rubble" */
			        if (c.feat[y][x] >= FEAT_RUBBLE) continue;

			        /* Save the time-stamp */
			        c.when[y][x] = (byte)flow_n;

			        /* Save the flow cost */
			        c.cost[y][x] = (byte)n;

			        /* Enqueue that entry */
			        flow_y[flow_tail] = (byte)y;
			        flow_x[flow_tail] = (byte)x;

			        /* Advance the queue */
			        if (++flow_tail == FLOW_MAX) flow_tail = 0;

			        /* Hack -- Overflow by forgetting new entry */
			        if (flow_tail == flow_head) flow_tail = old_head;
			    }
			}
		}




		/*
		 * Light up the dungeon using "claravoyance"
		 *
		 * This function "illuminates" every grid in the dungeon, memorizes all
		 * "objects", and memorizes all grids as with magic mapping.
		 */
		public static void wiz_light()
		{
			throw new NotImplementedException();
			//int i, y, x;


			///* Memorize objects */
			//for (i = 1; i < o_max; i++)
			//{
			//    object_type *o_ptr = object_byid(i);

			//    /* Skip dead objects */
			//    if (!o_ptr.kind) continue;

			//    /* Skip held objects */
			//    if (o_ptr.held_m_idx) continue;

			//    /* Memorize */
			//    o_ptr.marked = true;
			//}

			///* Scan all normal grids */
			//for (y = 1; y < DUNGEON_HGT-1; y++)
			//{
			//    /* Scan all normal grids */
			//    for (x = 1; x < DUNGEON_WID-1; x++)
			//    {
			//        /* Process all non-walls */
			//        if (cave.feat[y][x] < FEAT_SECRET)
			//        {
			//            /* Scan all neighbors */
			//            for (i = 0; i < 9; i++)
			//            {
			//                int yy = y + ddy_ddd[i];
			//                int xx = x + ddx_ddd[i];

			//                /* Perma-light the grid */
			//                cave.info[yy][xx] |= (CAVE_GLOW);

			//                /* Memorize normal features */
			//                if (cave.feat[yy][xx] > FEAT_INVIS)
			//                    cave.info[yy][xx] |= (CAVE_MARK);
			//            }
			//        }
			//    }
			//}

			///* Fully update the visuals */
			//p_ptr.update |= (PU_FORGET_VIEW | PU_UPDATE_VIEW | PU_MONSTERS);

			///* Redraw whole map, monster list */
			//p_ptr.redraw |= (PR_MAP | PR_MONLIST | PR_ITEMLIST);
		}


		/*
		 * Forget the dungeon map (ala "Thinking of Maud...").
		 */
		public static void wiz_dark()
		{
			throw new NotImplementedException();
			//int i, y, x;


			///* Forget every grid */
			//for (y = 0; y < DUNGEON_HGT; y++)
			//{
			//    for (x = 0; x < DUNGEON_WID; x++)
			//    {
			//        /* Process the grid */
			//        cave.info[y][x] &= ~(CAVE_MARK);
			//        cave.info2[y][x] &= ~(CAVE2_DTRAP);
			//    }
			//}

			///* Forget all objects */
			//for (i = 1; i < o_max; i++)
			//{
			//    object_type *o_ptr = object_byid(i);

			//    /* Skip dead objects */
			//    if (!o_ptr.kind) continue;

			//    /* Skip held objects */
			//    if (o_ptr.held_m_idx) continue;

			//    /* Forget the object */
			//    o_ptr.marked = false;
			//}

			///* Fully update the visuals */
			//p_ptr.update |= (PU_FORGET_VIEW | PU_UPDATE_VIEW | PU_MONSTERS);

			///* Redraw map, monster list */
			//p_ptr.redraw |= (PR_MAP | PR_MONLIST | PR_ITEMLIST);
		}



		/*
		 * Light or Darken the town
		 */
		public static void cave_illuminate(Cave c, bool daytime)
		{
			int y, x, i;


			/* Apply light or darkness */
			for (y = 0; y < c.height; y++)
			{
			    for (x = 0; x < c.width; x++)
			    {
			        /* Interesting grids */
			        if (c.feat[y][x] > FEAT_INVIS)
			        {
			            /* Illuminate the grid */
			            c.info[y][x] |= (CAVE_GLOW);

			            /* Memorize the grid */
			            c.info[y][x] |= (CAVE_MARK);
			        }

			        /* Boring grids (light) */
			        else if (daytime)
			        {
			            /* Illuminate the grid */
			            c.info[y][x] |= (CAVE_GLOW);

			            /* Memorize grids */
			            c.info[y][x] |= (CAVE_MARK);
			        }

			        /* Boring grids (dark) */
			        else
			        {
			            /* Darken the grid */
			            c.info[y][x] &= ~(CAVE_GLOW);

			            /* Forget grids */
			            c.info[y][x] &= ~(CAVE_MARK);
			        }
			    }
			}


			/* Handle shop doorways */
			for (y = 0; y < c.height; y++)
			{
			    for (x = 0; x < c.width; x++)
			    {
			        /* Track shop doorways */
			        if ((c.feat[y][x] >= FEAT_SHOP_HEAD) &&
			            (c.feat[y][x] <= FEAT_SHOP_TAIL))
			        {
			            for (i = 0; i < 8; i++)
			            {
			                int yy = y + Misc.ddy_ddd[i];
			                int xx = x + Misc.ddx_ddd[i];

			                /* Illuminate the grid */
			                c.info[yy][xx] |= (CAVE_GLOW);

			                /* Memorize grids */
			                c.info[yy][xx] |= (CAVE_MARK);
			            }
			        }
			    }
			}


			/* Fully update the visuals */
			Misc.p_ptr.update |= (Misc.PU_FORGET_VIEW | Misc.PU_UPDATE_VIEW | Misc.PU_MONSTERS);

			/* Redraw map, monster list */
			Misc.p_ptr.redraw |= (Misc.PR_MAP | Misc.PR_MONLIST | Misc.PR_ITEMLIST);
		}

		public static void cave_set_feat(Cave c, int y, int x, int feat)
		{
			Misc.assert(c != null);
			Misc.assert(y >= 0 && y < DUNGEON_HGT);
			Misc.assert(x >= 0 && x < DUNGEON_WID);
			/* XXX: Check against c.height and c.width instead, once everywhere
			 * honors those... */

			c.feat[y][x] = (byte)feat;

			if (feat >= FEAT_DOOR_HEAD)
			    c.info[y][x] |= CAVE_WALL;
			else
			    c.info[y][x] &= ~CAVE_WALL;

			if (Player.Player.character_dungeon) {
			    cave_note_spot(c, y, x);
			    cave_light_spot(c, y, x);
			}
		}

		public static bool cave_in_bounds(Cave c, int y, int x)
		{
			return x >= 0 && x < c.width && y >= 0 && y < c.height;
		}

		public bool in_bounds_fully(int y, int x)
		{
			return x > 0 && x < width - 1 && y > 0 && y < height - 1;
		}

		/*
		 * Determine the path taken by a projection.
		 *
		 * The projection will always start from the grid (y1,x1), and will travel
		 * towards the grid (y2,x2), touching one grid per unit of distance along
		 * the major axis, and stopping when it enters the destination grid or a
		 * wall grid, or has travelled the maximum legal distance of "range".
		 *
		 * Note that "distance" in this function (as in the "update_view()" code)
		 * is defined as "MAX(dy,dx) + MIN(dy,dx)/2", which means that the player
		 * actually has an "octagon of projection" not a "circle of projection".
		 *
		 * The path grids are saved into the grid array pointed to by "gp", and
		 * there should be room for at least "range" grids in "gp".  Note that
		 * due to the way in which distance is calculated, this function normally
		 * uses fewer than "range" grids for the projection path, so the result
		 * of this function should never be compared directly to "range".  Note
		 * that the initial grid (y1,x1) is never saved into the grid array, not
		 * even if the initial grid is also the final grid.  XXX XXX XXX
		 *
		 * The "flg" flags can be used to modify the behavior of this function.
		 *
		 * In particular, the "PROJECT_STOP" and "PROJECT_THRU" flags have the same
		 * semantics as they do for the "project" function, namely, that the path
		 * will stop as soon as it hits a monster, or that the path will continue
		 * through the destination grid, respectively.
		 *
		 * The "PROJECT_JUMP" flag, which for the "project()" function means to
		 * start at a special grid (which makes no sense in this function), means
		 * that the path should be "angled" slightly if needed to avoid any wall
		 * grids, allowing the player to "target" any grid which is in "view".
		 * This flag is non-trivial and has not yet been implemented, but could
		 * perhaps make use of the "vinfo" array (above).  XXX XXX XXX
		 *
		 * This function returns the number of grids (if any) in the path.  This
		 * function will return zero if and only if (y1,x1) and (y2,x2) are equal.
		 *
		 * This algorithm is similar to, but slightly different from, the one used
		 * by "update_view_los()", and very different from the one used by "los()".
		 */
		//gp was u16b *
		public static int project_path(ushort[] gp, int range, int y1, int x1, int y2, int x2, int flg)
		{
			throw new NotImplementedException();
			//int y, x;

			//int n = 0;
			//int k = 0;

			///* Absolute */
			//int ay, ax;

			///* Offsets */
			//int sy, sx;

			///* Fractions */
			//int frac;

			///* Scale factors */
			//int full, half;

			///* Slope */
			//int m;


			///* No path necessary (or allowed) */
			//if ((x1 == x2) && (y1 == y2)) return (0);


			///* Analyze "dy" */
			//if (y2 < y1)
			//{
			//    ay = (y1 - y2);
			//    sy = -1;
			//}
			//else
			//{
			//    ay = (y2 - y1);
			//    sy = 1;
			//}

			///* Analyze "dx" */
			//if (x2 < x1)
			//{
			//    ax = (x1 - x2);
			//    sx = -1;
			//}
			//else
			//{
			//    ax = (x2 - x1);
			//    sx = 1;
			//}


			///* Number of "units" in one "half" grid */
			//half = (ay * ax);

			///* Number of "units" in one "full" grid */
			//full = half << 1;


			///* Vertical */
			//if (ay > ax)
			//{
			//    /* Start at tile edge */
			//    frac = ax * ax;

			//    /* Let m = ((dx/dy) * full) = (dx * dx * 2) = (frac * 2) */
			//    m = frac << 1;

			//    /* Start */
			//    y = y1 + sy;
			//    x = x1;

			//    /* Create the projection path */
			//    while (1)
			//    {
			//        /* Save grid */
			//        gp[n++] = GRID(y,x);

			//        /* Hack -- Check maximum range */
			//        if ((n + (k >> 1)) >= range) break;

			//        /* Sometimes stop at destination grid */
			//        if (!(flg & (PROJECT_THRU)))
			//        {
			//            if ((x == x2) && (y == y2)) break;
			//        }

			//        /* Always stop at non-initial wall grids */
			//        if ((n > 0) && !cave_floor_bold(y, x)) break;

			//        /* Sometimes stop at non-initial monsters/players */
			//        if (flg & (PROJECT_STOP))
			//        {
			//            if ((n > 0) && (cave.m_idx[y][x] != 0)) break;
			//        }

			//        /* Slant */
			//        if (m)
			//        {
			//            /* Advance (X) part 1 */
			//            frac += m;

			//            /* Horizontal change */
			//            if (frac >= half)
			//            {
			//                /* Advance (X) part 2 */
			//                x += sx;

			//                /* Advance (X) part 3 */
			//                frac -= full;

			//                /* Track distance */
			//                k++;
			//            }
			//        }

			//        /* Advance (Y) */
			//        y += sy;
			//    }
			//}

			///* Horizontal */
			//else if (ax > ay)
			//{
			//    /* Start at tile edge */
			//    frac = ay * ay;

			//    /* Let m = ((dy/dx) * full) = (dy * dy * 2) = (frac * 2) */
			//    m = frac << 1;

			//    /* Start */
			//    y = y1;
			//    x = x1 + sx;

			//    /* Create the projection path */
			//    while (1)
			//    {
			//        /* Save grid */
			//        gp[n++] = GRID(y,x);

			//        /* Hack -- Check maximum range */
			//        if ((n + (k >> 1)) >= range) break;

			//        /* Sometimes stop at destination grid */
			//        if (!(flg & (PROJECT_THRU)))
			//        {
			//            if ((x == x2) && (y == y2)) break;
			//        }

			//        /* Always stop at non-initial wall grids */
			//        if ((n > 0) && !cave_floor_bold(y, x)) break;

			//        /* Sometimes stop at non-initial monsters/players */
			//        if (flg & (PROJECT_STOP))
			//        {
			//            if ((n > 0) && (cave.m_idx[y][x] != 0)) break;
			//        }

			//        /* Slant */
			//        if (m)
			//        {
			//            /* Advance (Y) part 1 */
			//            frac += m;

			//            /* Vertical change */
			//            if (frac >= half)
			//            {
			//                /* Advance (Y) part 2 */
			//                y += sy;

			//                /* Advance (Y) part 3 */
			//                frac -= full;

			//                /* Track distance */
			//                k++;
			//            }
			//        }

			//        /* Advance (X) */
			//        x += sx;
			//    }
			//}

			///* Diagonal */
			//else
			//{
			//    /* Start */
			//    y = y1 + sy;
			//    x = x1 + sx;

			//    /* Create the projection path */
			//    while (1)
			//    {
			//        /* Save grid */
			//        gp[n++] = GRID(y,x);

			//        /* Hack -- Check maximum range */
			//        if ((n + (n >> 1)) >= range) break;

			//        /* Sometimes stop at destination grid */
			//        if (!(flg & (PROJECT_THRU)))
			//        {
			//            if ((x == x2) && (y == y2)) break;
			//        }

			//        /* Always stop at non-initial wall grids */
			//        if ((n > 0) && !cave_floor_bold(y, x)) break;

			//        /* Sometimes stop at non-initial monsters/players */
			//        if (flg & (PROJECT_STOP))
			//        {
			//            if ((n > 0) && (cave.m_idx[y][x] != 0)) break;
			//        }

			//        /* Advance (Y) */
			//        y += sy;

			//        /* Advance (X) */
			//        x += sx;
			//    }
			//}


			///* Length */
			//return (n);
		}


		/*
		 * Determine if a bolt spell cast from (y1,x1) to (y2,x2) will arrive
		 * at the final destination, assuming that no monster gets in the way,
		 * using the "project_path()" function to check the projection path.
		 *
		 * Note that no grid is ever "projectable()" from itself.
		 *
		 * This function is used to determine if the player can (easily) target
		 * a given grid, and if a monster can target the player.
		 */
		public static bool projectable(int y1, int x1, int y2, int x2, int flg)
		{
			throw new NotImplementedException();
			//int y, x;

			//int grid_n = 0;
			//u16b grid_g[512];

			///* Check the projection path */
			//grid_n = project_path(grid_g, MAX_RANGE, y1, x1, y2, x2, flg);

			///* No grid is ever projectable from itself */
			//if (!grid_n) return (false);

			///* Final grid */
			//y = GRID_Y(grid_g[grid_n-1]);
			//x = GRID_X(grid_g[grid_n-1]);

			///* May not end in a wall grid */
			//if (!cave_floor_bold(y, x)) return (false);

			///* May not end in an unrequested grid */
			//if ((y != y2) || (x != x2)) return (false);

			///* Assume okay */
			//return (true);
		}



		/*
		 * Standard "find me a location" function
		 *
		 * Obtains a legal location within the given distance of the initial
		 * location, and with "los()" from the source to destination location.
		 *
		 * This function is often called from inside a loop which searches for
		 * locations while increasing the "d" distance.
		 *
		 * Currently the "m" parameter is unused.
		 */
		//yp and xp were both pointers
		public static void scatter(int[] yp, int[] xp, int y, int x, int d, int m)
		{
			throw new NotImplementedException();
			//int nx, ny;


			///* Unused parameter */
			//(void)m;

			///* Pick a location */
			//while (true)
			//{
			//    /* Pick a new location */
			//    ny = rand_spread(y, d);
			//    nx = rand_spread(x, d);

			//    /* Ignore annoying locations */
			//    if (!in_bounds_fully(ny, nx)) continue;

			//    /* Ignore "excessively distant" locations */
			//    if ((d > 1) && (distance(y, x, ny, nx) > d)) continue;

			//    /* Require "line of sight" */
			//    if (los(y, x, ny, nx)) break;
			//}

			///* Save the location */
			//(*yp) = ny;
			//(*xp) = nx;
		}

		public static void health_track(Player.Player p, int m_idx)
		{
			p.health_who = (short)m_idx;
			p.redraw |= Misc.PR_HEALTH;
		}

		/*
		 * Hack -- track the given monster race
		 */
		public static void monster_race_track(int r_idx)
		{
			/* Save this monster ID */
			Misc.p_ptr.monster_race_idx = (short)r_idx;

			/* Window stuff */
			Misc.p_ptr.redraw |= (Misc.PR_MONSTER);
		}



		/*
		 * Hack -- track the given object kind
		 */
		public static void track_object(int item)
		{
			Misc.p_ptr.object_idx = (short)item;
			Misc.p_ptr.object_kind_idx = Misc.NO_OBJECT;
			Misc.p_ptr.redraw |= (Misc.PR_OBJECT);
		}

		public static void track_object_kind(int k_idx)
		{
			Misc.p_ptr.object_idx = Misc.NO_OBJECT;
			Misc.p_ptr.object_kind_idx = (short)k_idx;
			Misc.p_ptr.redraw |= (Misc.PR_OBJECT);
		}

		public static bool tracked_object_is(int item)
		{
			return (Misc.p_ptr.object_idx == item);
		}

		/*
		 * Something has happened to disturb the player.
		 *
		 * The first arg indicates a major disturbance, which affects search.
		 *
		 * The second arg is currently unused, but could induce output flush.
		 *
		 * All disturbance cancels repeated commands, resting, and running.
		 */
		public static void disturb(Player.Player p, int stop_search, int unused_flag)
		{
			/* Unused parameter */
			//(void)unused_flag;

			/* Cancel repeated commands */
			Game_Command.cancel_repeat();

			/* Cancel Resting */
			if (p.resting != 0) {
			    p.resting = 0;
			    p.redraw |= Misc.PR_STATE;
			}

			/* Cancel running */
			if (p.running != 0) {
			    Misc.p_ptr.running = 0;

			    /* Check for new panel if appropriate */
			    if (Option.center_player.value) Xtra2.verify_panel();
			    p.update |= Misc.PU_TORCH;
			}

			/* Cancel searching if requested */
			if (stop_search != 0 && p.searching != 0)
			{
			    p.searching = 0; //false
			    p.update |= Misc.PU_BONUS;
			    p.redraw |= Misc.PR_STATE;
			}

			/* Flush input */
			Utilities.flush();
		}

		public static bool is_quest(int level)
		{
			int i;

			/* Town is never a quest */
			if (level == 0 || Misc.q_list == null) return false;

			for (i = 0; i < Misc.MAX_Q_IDX; i++)
			    if (Misc.q_list[i].level == level)
			        return true;

			return false;
		}


		static int view_n;
		static ushort[] view_g = new ushort[Misc.VIEW_MAX];

		/*
		 * Approximate distance between two points.
		 *
		 * When either the X or Y component dwarfs the other component,
		 * this function is almost perfect, and otherwise, it tends to
		 * over-estimate about one grid per fifteen grids of distance.
		 *
		 * Algorithm: hypot(dy,dx) = max(dy,dx) + min(dy,dx) / 2
		 */
		public static int distance(int y1, int x1, int y2, int x2)
		{
			//TODO: Make a more accurate distance
			/* Find the absolute y/x distance components */
			int ay = Math.Abs(y2 - y1);
			int ax = Math.Abs(x2 - x1);

			/* Approximate the distance */
			return ay > ax ? ay + (ax>>1) : ax + (ay>>1);
		}


		/*
		 * A simple, fast, integer-based line-of-sight algorithm.  By Joseph Hall,
		 * 4116 Brewster Drive, Raleigh NC 27606.  Email to jnh@ecemwl.ncsu.edu.
		 *
		 * This function returns true if a "line of sight" can be traced from the
		 * center of the grid (x1,y1) to the center of the grid (x2,y2), with all
		 * of the grids along this path (except for the endpoints) being non-wall
		 * grids.  Actually, the "chess knight move" situation is handled by some
		 * special case code which allows the grid diagonally next to the player
		 * to be obstructed, because this yields better gameplay semantics.  This
		 * algorithm is totally reflexive, except for "knight move" situations.
		 *
		 * Because this function uses (short) ints for all calculations, overflow
		 * may occur if dx and dy exceed 90.
		 *
		 * Once all the degenerate cases are eliminated, we determine the "slope"
		 * ("m"), and we use special "fixed point" mathematics in which we use a
		 * special "fractional component" for one of the two location components
		 * ("qy" or "qx"), which, along with the slope itself, are "scaled" by a
		 * scale factor equal to "abs(dy*dx*2)" to keep the math simple.  Then we
		 * simply travel from start to finish along the longer axis, starting at
		 * the border between the first and second tiles (where the y offset is
		 * thus half the slope), using slope and the fractional component to see
		 * when motion along the shorter axis is necessary.  Since we assume that
		 * vision is not blocked by "brushing" the corner of any grid, we must do
		 * some special checks to avoid testing grids which are "brushed" but not
		 * actually "entered".
		 *
		 * Angband three different "line of sight" type concepts, including this
		 * function (which is used almost nowhere), the "project()" method (which
		 * is used for determining the paths of projectables and spells and such),
		 * and the "update_view()" concept (which is used to determine which grids
		 * are "viewable" by the player, which is used for many things, such as
		 * determining which grids are illuminated by the player's torch, and which
		 * grids and monsters can be "seen" by the player, etc).
		 */
		public static bool los(int y1, int x1, int y2, int x2)
		{
			/* Delta */
			int dx, dy;

			/* Absolute */
			int ax, ay;

			/* Signs */
			int sx, sy;

			/* Fractions */
			int qx, qy;

			/* Scanners */
			int tx, ty;

			/* Scale factors */
			int f1, f2;

			/* Slope, or 1/Slope, of LOS */
			int m;


			/* Extract the offset */
			dy = y2 - y1;
			dx = x2 - x1;

			/* Extract the absolute offset */
			ay = Math.Abs(dy);
			ax = Math.Abs(dx);


			/* Handle adjacent (or identical) grids */
			if ((ax < 2) && (ay < 2)) return (true);


			/* Directly South/North */
			if (dx == 0)
			{
			    /* South -- check for walls */
			    if (dy > 0)
			    {
			        for (ty = y1 + 1; ty < y2; ty++)
			        {
			            if (!cave_floor_bold(ty, x1)) return (false);
			        }
			    }

			    /* North -- check for walls */
			    else
			    {
			        for (ty = y1 - 1; ty > y2; ty--)
			        {
			            if (!cave_floor_bold(ty, x1)) return (false);
			        }
			    }

			    /* Assume los */
			    return (true);
			}

			/* Directly East/West */
			if (dy == 0)
			{
			    /* East -- check for walls */
			    if (dx > 0)
			    {
			        for (tx = x1 + 1; tx < x2; tx++)
			        {
			            if (!cave_floor_bold(y1, tx)) return (false);
			        }
			    }

			    /* West -- check for walls */
			    else
			    {
			        for (tx = x1 - 1; tx > x2; tx--)
			        {
			            if (!cave_floor_bold(y1, tx)) return (false);
			        }
			    }

			    /* Assume los */
			    return (true);
			}


			/* Extract some signs */
			sx = (dx < 0) ? -1 : 1;
			sy = (dy < 0) ? -1 : 1;


			/* Vertical "knights" */
			if (ax == 1)
			{
			    if (ay == 2)
			    {
			        if (cave_floor_bold(y1 + sy, x1)) return (true);
			    }
			}

			/* Horizontal "knights" */
			else if (ay == 1)
			{
			    if (ax == 2)
			    {
			        if (cave_floor_bold(y1, x1 + sx)) return (true);
			    }
			}


			/* Calculate scale factor div 2 */
			f2 = (ax * ay);

			/* Calculate scale factor */
			f1 = f2 << 1;


			/* Travel horizontally */
			if (ax >= ay)
			{
			    /* Let m = dy / dx * 2 * (dy * dx) = 2 * dy * dy */
			    qy = ay * ay;
			    m = qy << 1;

			    tx = x1 + sx;

			    /* Consider the special case where slope == 1. */
			    if (qy == f2)
			    {
			        ty = y1 + sy;
			        qy -= f1;
			    }
			    else
			    {
			        ty = y1;
			    }

			    /* Note (below) the case (qy == f2), where */
			    /* the LOS exactly meets the corner of a tile. */
			    while ((x2 - tx) != 0)
			    {
			        if (!cave_floor_bold(ty, tx)) return (false);

			        qy += m;

			        if (qy < f2)
			        {
			            tx += sx;
			        }
			        else if (qy > f2)
			        {
			            ty += sy;
			            if (!cave_floor_bold(ty, tx)) return (false);
			            qy -= f1;
			            tx += sx;
			        }
			        else
			        {
			            ty += sy;
			            qy -= f1;
			            tx += sx;
			        }
			    }
			}

			/* Travel vertically */
			else
			{
			    /* Let m = dx / dy * 2 * (dx * dy) = 2 * dx * dx */
			    qx = ax * ax;
			    m = qx << 1;

			    ty = y1 + sy;

			    if (qx == f2)
			    {
			        tx = x1 + sx;
			        qx -= f1;
			    }
			    else
			    {
			        tx = x1;
			    }

			    /* Note (below) the case (qx == f2), where */
			    /* the LOS exactly meets the corner of a tile. */
			    while ((y2 - ty) != 0)
			    {
			        if (!cave_floor_bold(ty, tx)) return (false);

			        qx += m;

			        if (qx < f2)
			        {
			            ty += sy;
			        }
			        else if (qx > f2)
			        {
			            tx += sx;
			            if (!cave_floor_bold(ty, tx)) return (false);
			            qx -= f1;
			            ty += sy;
			        }
			        else
			        {
			            tx += sx;
			            qx -= f1;
			            ty += sy;
			        }
			    }
			}

			/* Assume los */
			return (true);
		}




		/*
		 * Returns true if the player's grid is dark
		 */
		public static bool no_light()
		{
			return (!player_can_see_bold(Misc.p_ptr.py, Misc.p_ptr.px));
		}




		/*
		 * Determine if a given location may be "destroyed"
		 *
		 * Used by destruction spells, and for placing stairs, etc.
		 */
		public static bool cave_valid_bold(int y, int x)
		{
			throw new NotImplementedException();
			//object_type *o_ptr;

			///* Forbid perma-grids */
			//if (cave_perma_bold(y, x)) return (false);

			///* Check objects */
			//for (o_ptr = get_first_object(y, x); o_ptr; o_ptr = get_next_object(o_ptr))
			//{
			//    /* Forbid artifact grids */
			//    if (o_ptr.artifact) return (false);
			//}

			///* Accept */
			//return (true);
		}


		/*
		 * Hack -- Hallucinatory monster
		 */
		//a was byte*, c was char*
		static void hallucinatory_monster(byte[] a, char[] c)
		{
			throw new NotImplementedException();
			//while (1)
			//{
			//    /* Select a random monster */
			//    monster_race *r_ptr = &r_info[randint0(Misc.z_info.r_max)];
		
			//    /* Skip non-entries */
			//    if (!r_ptr.name) continue;
		
			//    /* Retrieve attr/char */
			//    *a = r_ptr.x_attr;
			//    *c = r_ptr.x_char;
			//    return;
			//}
		}


		/*
		 * Hack -- Hallucinatory object
		 */
		//a was byte*, c was char*
		static void hallucinatory_object(ref ConsoleColor a, ref char c)
		{
			throw new NotImplementedException();
			//while (1)
			//{
			//    /* Select a random object */
			//    object_kind *k_ptr = &k_info[randint0(Misc.z_info.k_max - 1) + 1];

			//    /* Skip non-entries */
			//    if (!k_ptr.name) continue;
		
			//    /* Retrieve attr/char (HACK - without flavors) */
			//    *a = k_ptr.x_attr;
			//    *c = k_ptr.x_char;
		
			//    /* HACK - Skip empty entries */
			//    if (*a == 0 || *c == 0) continue;

			//    return;
			//}
		}




		/*
		 * Translate text colours.
		 *
		 * This translates a color based on the attribute. We use this to set terrain to
		 * be lighter or darker, make metallic monsters shimmer, highlight text under the
		 * mouse, and reduce the colours on mono colour or 16 colour terms to the correct
		 * colour space.
		 *
		 * TODO: Honour the attribute for the term (full color, mono, 16 color) but ensure
		 * that e.g. the lighter version of yellow becomes white in a 16 color term, but
		 * light yellow in a full colour term.
		 */
		public static byte get_color(byte a, int attr, int n)
		{
			throw new NotImplementedException();
			///* Accept any graphical attr (high bit set) */
			//if (a & (0x80)) return (a);

			///* TODO: Honour the attribute for the term (full color, mono, 16 color) */
			//if (!attr) return(a);

			///* Translate the color N times */
			//while (n > 0)
			//{
			//    a = color_table[a].color_translate[attr];
			//    n--;
			//}
	
			///* Return the modified color */
			//return (a);
		}


		/* 
		 * Checks if a square is at the (inner) edge of a trap detect area 
		 */ 
		public static bool dtrap_edge(int y, int x) 
		{ 
			/* Check if the square is a dtrap in the first place */ 
			if ((cave.info2[y][x] & CAVE2_DTRAP) == 0) return false; 

			/* Check for non-dtrap adjacent grids */ 
			if (cave.in_bounds_fully(y + 1, x    ) &&
			        ((cave.info2[y + 1][x    ] & CAVE2_DTRAP) == 0)) return true; 
			if (cave.in_bounds_fully(y    , x + 1) &&
			        ((cave.info2[y    ][x + 1] & CAVE2_DTRAP) == 0)) return true; 
			if (cave.in_bounds_fully(y - 1, x    ) &&
			        ((cave.info2[y - 1][x    ] & CAVE2_DTRAP) == 0)) return true; 
			if (cave.in_bounds_fully(y    , x - 1) &&
			        ((cave.info2[y    ][x - 1] & CAVE2_DTRAP) == 0)) return true; 

			return false; 
		}


		static bool feat_is_known_trap(int feat) {
			throw new NotImplementedException();
			//return feat >= FEAT_TRAP_HEAD && feat <= FEAT_TRAP_TAIL;
		}

		static bool feat_is_treasure(int feat) {
			throw new NotImplementedException();
			//return feat == FEAT_MAGMA_K || feat == FEAT_QUARTZ_K;
		}


		/**
		 * Apply text lighting effects
		 */
		static void grid_get_text(Grid_Data g, ref ConsoleColor a, ref char c)
		{
		    /* Trap detect edge, but don't colour traps themselves, or treasure */
		    if (g.trapborder && !feat_is_known_trap((int)g.f_idx) && !feat_is_treasure((int)g.f_idx))
		    {
		        if (g.in_view)
		            a = ConsoleColor.Green;
		        else
		            a = ConsoleColor.DarkGreen;
		    }
		    else if (g.f_idx == FEAT_FLOOR)
		    {
		        if (g.lighting == Grid_Data.grid_light_level.FEAT_LIGHTING_BRIGHT) {
		            if (a == ConsoleColor.White)
		                a = ConsoleColor.Yellow;
		        } else if (g.lighting == Grid_Data.grid_light_level.FEAT_LIGHTING_DARK) {
		            if (a == ConsoleColor.White)
		                a = ConsoleColor.DarkGray; //Light dark...??? Maybe make this black...
		        }
		    }
		    else if (g.f_idx > FEAT_INVIS)
		    {
		        if (g.lighting == Grid_Data.grid_light_level.FEAT_LIGHTING_DARK) {
		            if (a == ConsoleColor.White)
		                a = ConsoleColor.Gray; //SLATE
		        }
		    }
		}


		/*
		 * This function takes a pointer to a grid info struct describing the 
		 * contents of a grid location (as obtained through the function map_info)
		 * and fills in the character and attr pairs for display.
		 *
		 * ap and cp are filled with the attr/char pair for the monster, object or 
		 * floor tile that is at the "top" of the grid (monsters covering objects, 
		 * which cover floor, assuming all are present).
		 *
		 * tap and tcp are filled with the attr/char pair for the floor, regardless
		 * of what is on it.  This can be used by graphical displays with
		 * transparency to place an object onto a floor tile, is desired.
		 *
		 * Any lighting effects are also applied to these pairs, clear monsters allow
		 * the underlying colour or feature to show through (ATTR_CLEAR and
		 * CHAR_CLEAR), multi-hued colour-changing (ATTR_MULTI) is applied, and so on.
		 * Technically, the flag "CHAR_MULTI" is supposed to indicate that a monster 
		 * looks strange when examined, but this flag is currently ignored.
		 *
		 * NOTES:
		 * This is called pretty frequently, whenever a grid on the map display
		 * needs updating, so don't overcomplicate it.
		 *
		 * The "zero" entry in the feature/object/monster arrays are
		 * used to provide "special" attr/char codes, with "monster zero" being
		 * used for the player attr/char, "object zero" being used for the "pile"
		 * attr/char, and "feature zero" being used for the "darkness" attr/char.
		 *
		 * TODO:
		 * The transformations for tile colors, or brightness for the 16x16
		 * tiles should be handled differently.  One possibility would be to
		 * extend feature_type with attr/char definitions for the different states.
		 * This will probably be done outside of the current text.graphics mappings
		 * though.
		 */
		public static void grid_data_as_text(ref Grid_Data g, ref ConsoleColor ap, ref char cp, ref ConsoleColor tap, ref char tcp)
		{
		    Feature f_ptr = Misc.f_info[g.f_idx];

		    ConsoleColor a = f_ptr.x_attr[(int)g.lighting];
		    char c = f_ptr.x_char[(int)g.lighting];

		    /* Check for trap detection boundaries */
		    if (Misc.use_graphics == Misc.GRAPHICS_NONE || Misc.use_graphics == Misc.GRAPHICS_PSEUDO)
		        grid_get_text(g, ref a, ref c);

		    /* Save the terrain info for the transparency effects */
		    tap = a;
		    tcp = c;


		    /* If there's an object, deal with that. */
		    if (g.first_kind != null)
		    {
		        if (g.hallucinate) {
		            /* Just pick a random object to display. */
		            hallucinatory_object(ref a, ref c);
		        } else if (g.multiple_objects) {
		            /* Get the "pile" feature instead */
		            a = Object.Object.object_kind_attr(Misc.k_info[0]);
		            c = Object.Object.object_kind_char(Misc.k_info[0]);
		        } else {
		            /* Normal attr and char */
		            a = Object.Object.object_kind_attr(g.first_kind);
		            c = Object.Object.object_kind_char(g.first_kind);
		        }			
		    }

		    /* If there's a monster */
		    if (g.m_idx > 0 && !Monster.Monster.is_mimicking((int)g.m_idx))
		    {
		        if (g.hallucinate)
		        {
					throw new NotImplementedException();
		            /* Just pick a random monster to display. */
		            //hallucinatory_monster(&a, &c);
		        }
		        else
		        {
		            Monster.Monster m_ptr = cave_monster(cave, (int)g.m_idx);
		            Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];
				
		            ConsoleColor da;
		            char dc;
			
		            /* Desired attr & char */
		            da = r_ptr.x_attr;
		            dc = r_ptr.x_char;

		            /* Special attr/char codes */
		            if (((int)da & 0x80) != 0 && (dc & 0x80) != 0)
		            {
		                /* Use attr */
		                a = da;
				
		                /* Use char */
		                c = dc;
		            }
			
		            /* Turn uniques purple if desired (violet, actually) */
		            else if (Option.purple_uniques.value && r_ptr.flags.has(Monster_Flag.UNIQUE.value))
		            {
		                /* Use (light) violet attr */
		                a = ConsoleColor.Magenta;

		                /* Use char */
		                c = dc;
		            }

		            /* Multi-hued monster */
		            else if (r_ptr.flags.has(Monster_Flag.ATTR_MULTI.value) ||
		                     r_ptr.flags.has(Monster_Flag.ATTR_FLICKER.value) ||
		                     r_ptr.flags.has(Monster_Flag.ATTR_RAND.value))
		            {
		                /* Multi-hued attr */
		                a = ((int)m_ptr.attr) != 0 ? m_ptr.attr : da;
				
		                /* Normal char */
		                c = dc;
		            }
			
		            /* Normal monster (not "clear" in any way) */
		            else if (!r_ptr.flags.test(Monster_Flag.ATTR_CLEAR.value, Monster_Flag.CHAR_CLEAR.value))
		            {
		                /* Use attr */
		                a = da;

		                /* Desired attr & char */
		                da = r_ptr.x_attr;
		                dc = r_ptr.x_char;
				
		                /* Use char */
		                c = dc;
		            }
			
		            /* Hack -- Bizarre grid under monster */
		            else if (((int)a & 0x80) != 0 || (c & 0x80) != 0)
		            {
		                /* Use attr */
		                a = da;
				
		                /* Use char */
		                c = dc;
		            }
			
		            /* Normal char, Clear attr, monster */
		            else if (!r_ptr.flags.has(Monster_Flag.CHAR_CLEAR.value))
		            {
		                /* Normal char */
		                c = dc;
		            }
				
		            /* Normal attr, Clear char, monster */
		            else if (!r_ptr.flags.has(Monster_Flag.ATTR_CLEAR.value))
		            {
		                /* Normal attr */
		                a = da;
		            }

		            /* Store the drawing attr so we can use it elsewhere */
		            m_ptr.attr = a;
		        }
		    }

		    /* Handle "player" */
		    else if (g.is_player)
		    {
		        Monster_Race r_ptr = Misc.r_info[0]; //Is 0 supposed to be player??

		        /* Get the "player" attr */
		        a = r_ptr.x_attr;
		        if ((Option.hp_changes_color.value) && ((int)a & 0x80) == 0){
		            switch(Misc.p_ptr.chp * 10 / Misc.p_ptr.mhp) {
		                case 10:
		                case  9: {
		                    a = ConsoleColor.White;
		                    break;
		                }
		                case  8:
		                case  7: {
		                    a = ConsoleColor.Yellow;
		                    break;
		                }
		                case  6:
		                case  5: {
		                    a = ConsoleColor.DarkYellow;
		                    break;
		                }
		                case  4:
		                case  3: {
		                    a = ConsoleColor.Red;
		                    break;
		                }
		                case  2:
		                case  1:
		                case  0: {
		                    a = ConsoleColor.Red;
		                    break;
		                }
		                default: {
		                    a = ConsoleColor.White;
		                    break;
		                }
		            }
		        }

		        /* Get the "player" char */
		        c = r_ptr.x_char;
		    }

		    /* Result */
		    ap = a;
		    cp = c;	
		}




		/*
		 * This function takes a grid location (x, y) and extracts information the
		 * player is allowed to know about it, filling in the grid_data structure
		 * passed in 'g'.
		 *
		 * The information filled in is as follows:
		 *  - g.f_idx is filled in with the terrain's feature type, or FEAT_NONE
		 *    if the player doesn't know anything about the grid.  The function
		 *    makes use of the "mimic" field in terrain in order to allow one
		 *    feature to look like another (hiding secret doors, invisible traps,
		 *    etc).  This will return the terrain type the player "Knows" about,
		 *    not necessarily the real terrain.
		 *  - g.m_idx is set to the monster index, or 0 if there is none (or the
		 *    player doesn't know it).
		 *  - g.first_kind is set to the object_kind of the first object in a grid
		 *    that the player knows about, or null for no objects.
		 *  - g.muliple_objects is true if there is more than one object in the
		 *    grid that the player knows and cares about (to facilitate any special
		 *    floor stack symbol that might be used).
		 *  - g.in_view is true if the player can currently see the grid - this can
		 *    be used to indicate field-of-view, such as through the OPT(view_bright_light)
		 *    option.
		 *  - g.lighting is set to indicate the lighting level for the grid:
		 *    FEAT_LIGHTING_DARK for unlit grids, FEAT_LIGHTING_LIT for those lit by the player's
		 *    light source, and FEAT_LIGHTING_BRIGHT for inherently light grids (lit rooms, etc).
		 *    Note that lighting is always FEAT_LIGHTING_BRIGHT for known "interesting" grids
		 *    like walls.
		 *  - g.is_player is true if the player is on the given grid.
		 *  - g.hallucinate is true if the player is hallucinating something "strange"
		 *    for this grid - this should pick a random monster to show if the m_idx
		 *    is non-zero, and a random object if first_kind is non-zero.
		 *       
		 * NOTES:
		 * This is called pretty frequently, whenever a grid on the map display
		 * needs updating, so don't overcomplicate it.
		 *
		 * Terrain is remembered separately from objects and monsters, so can be
		 * shown even when the player can't "see" it.  This leads to things like
		 * doors out of the player's view still change from closed to open and so on.
		 *
		 * TODO:
		 * Hallucination is currently disabled (it was a display-level hack before,
		 * and we need it to be a knowledge-level hack).  The idea is that objects
		 * may turn into different objects, monsters into different monsters, and
		 * terrain may be objects, monsters, or stay the same.
		 */
		public static void map_info(int y, int x, ref Grid_Data g)
		{
		    Object.Object o_ptr;
		    short info;

		    Misc.assert(x < DUNGEON_WID);
		    Misc.assert(y < DUNGEON_HGT);

		    info = Cave.cave.info[y][x];
	
		    /* Default "clear" values, others will be set later where appropriate. */
		    g.first_kind = null;
		    g.multiple_objects = false;
		    g.lighting = Grid_Data.grid_light_level.FEAT_LIGHTING_DARK;

		    g.f_idx = cave.feat[y][x];
		    if (Misc.f_info[g.f_idx].mimic != 0)
		        g.f_idx = Misc.f_info[g.f_idx].mimic;

		    g.in_view = (info & CAVE_SEEN) != 0 ? true : false;
		    g.is_player = (cave.m_idx[y][x] < 0) ? true : false;
		    g.m_idx = (uint)((g.is_player) ? 0 : cave.m_idx[y][x]);
		    g.hallucinate = Misc.p_ptr.timed[(int)Timed_Effect.IMAGE] != 0 ? true : false;
		    g.trapborder = (dtrap_edge(y, x)) ? true : false;

		    if (g.in_view)
		    {
		        g.lighting = Grid_Data.grid_light_level.FEAT_LIGHTING_LIT;

		        if ((info & CAVE_GLOW) == 0 && Option.view_yellow_light.value)
		            g.lighting = Grid_Data.grid_light_level.FEAT_LIGHTING_BRIGHT;
		    }
		    else if ((info & CAVE_MARK) == 0)
		    {
		        g.f_idx = FEAT_NONE;
		    }


		    /* Objects */
		    for (o_ptr = Object.Object.get_first_object(y, x); o_ptr != null; o_ptr = Object.Object.get_next_object(o_ptr))
		    {
		        /* Memorized objects */
		        if (o_ptr.marked != 0 && !Squelch.item_ok(o_ptr))
		        {
		            /* First item found */
		            if (g.first_kind == null)
		            {
		                g.first_kind = o_ptr.kind;
		            }
		            else
		            {
		                g.multiple_objects = true;

		                /* And we know all we need to know. */
		                break;
		            }
		        }
		    }

		    /* Monsters */
		    if (g.m_idx > 0)
		    {
		        /* If the monster isn't "visible", make sure we don't list it.*/
		        Monster.Monster m_ptr = cave_monster(Cave.cave, (int)g.m_idx);
		        if (!m_ptr.ml) g.m_idx = 0;

		    }

		    /* Rare random hallucination on non-outer walls */
		    if (g.hallucinate && g.m_idx == 0 && g.first_kind == null)
		    {
		        if (Random.one_in_(128) && g.f_idx < FEAT_PERM_SOLID)
		            g.m_idx = 1;
		        else if (Random.one_in_(128) && g.f_idx < FEAT_PERM_SOLID)
		            /* if hallucinating, we just need first_kind to not be null */
		            g.first_kind = Misc.k_info[0];
		        else
		            g.hallucinate = false;
		    }

		    Misc.assert(g.f_idx <= FEAT_PERM_SOLID);
		    if (!g.hallucinate)
		        Misc.assert((int)g.m_idx < cave.mon_max);
		    /* All other g fields are 'flags', mostly booleans. */
		}



		/*
		 * Move the cursor to a given map location.
		 */
		static void move_cursor_relative_map(int y, int x)
		{
			int ky, kx;

			Term old;

			int j;

			/* Scan windows */
			for (j = 0; j < Misc.ANGBAND_TERM_MAX; j++)
			{
			    Term t = Misc.angband_term[j];

			    /* No window */
			    if (t == null) continue;

			    /* No relevant flags */
			    if ((Player_Other.instance.window_flag[j] & (Misc.PW_MAP)) == 0) continue;

			    /* Location relative to panel */
			    ky = y - t.offset_y;

			    if (Term.tile_height > 1)
			    {
			            ky = Term.tile_height * ky;
			    }

			    /* Verify location */
			    if ((ky < 0) || (ky >= t.hgt)) continue;

			    /* Location relative to panel */
			    kx = x - t.offset_x;

			    if (Term.tile_width > 1)
			    {
			            kx = Term.tile_width * kx;
			    }

			    /* Verify location */
			    if ((kx < 0) || (kx >= t.wid)) continue;

			    /* Go there */
				//Nick: This seems like a MAJOR hack!!! Fix this!!!
			    old = Term.instance;
			    t.activate();
			    Term.gotoxy(kx, ky);
			    old.activate();
			}
		}


		/*
		 * Move the cursor to a given map location.
		 *
		 * The main screen will always be at least 24x80 in size.
		 */
		public static void move_cursor_relative(int y, int x)
		{
			int ky, kx;
			int vy, vx;

			/* Move the cursor on map sub-windows */
			move_cursor_relative_map(y, x);

			/* Location relative to panel */
			ky = y - Term.instance.offset_y;

			/* Verify location */
			if ((ky < 0) || (ky >= Misc.SCREEN_HGT)) return;

			/* Location relative to panel */
			kx = x - Term.instance.offset_x;

			/* Verify location */
			if ((kx < 0) || (kx >= Misc.SCREEN_WID)) return;

			/* Location in window */
			vy = ky + Misc.ROW_MAP;

			/* Location in window */
			vx = kx + Misc.COL_MAP;

			if (Term.tile_width > 1)
			{
			        vx += (Term.tile_width - 1) * kx;
			}
			if (Term.tile_height > 1)
			{
			        vy += (Term.tile_height - 1) * ky;
			}

			/* Go there */
			Term.gotoxy(vx, vy);
		}



		/*
		 * Display an attr/char pair at the given map location
		 *
		 * Note the inline use of "panel_contains()" for efficiency.
		 *
		 * Note the use of "Term_queue_char()" for efficiency.
		 */
		static void print_rel_map(char c, byte a, int y, int x)
		{
			throw new NotImplementedException();
			//int ky, kx;

			//int j;

			///* Scan windows */
			//for (j = 0; j < ANGBAND_TERM_MAX; j++)
			//{
			//    term *t = angband_term[j];

			//    /* No window */
			//    if (!t) continue;

			//    /* No relevant flags */
			//    if (!(op_ptr.window_flag[j] & (PW_MAP))) continue;

			//    /* Location relative to panel */
			//    ky = y - t.offset_y;

			//    if (tile_height > 1)
			//    {
			//            ky = tile_height * ky;
			//        if (ky + 1 >= t.hgt) continue;
			//    }

			//    /* Verify location */
			//    if ((ky < 0) || (ky >= t.hgt)) continue;

			//    /* Location relative to panel */
			//    kx = x - t.offset_x;

			//    if (tile_width > 1)
			//    {
			//            kx = tile_width * kx;
			//        if (kx + 1 >= t.wid) continue;
			//    }

			//    /* Verify location */
			//    if ((kx < 0) || (kx >= t.wid)) continue;

			//    /* Hack -- Queue it */
			//    Term_queue_char(t, kx, ky, a, c, 0, 0);

			//    if ((tile_width > 1) || (tile_height > 1))
			//            Term_big_queue_char(Term, kx, ky, a, c, 0, 0);
			//}
		}



		/*
		 * Display an attr/char pair at the given map location
		 *
		 * Note the inline use of "panel_contains()" for efficiency.
		 *
		 * Note the use of "Term_queue_char()" for efficiency.
		 *
		 * The main screen will always be at least 24x80 in size.
		 */
		public static void print_rel(char c, byte a, int y, int x)
		{
			throw new NotImplementedException();
			//int ky, kx;
			//int vy, vx;

			///* Print on map sub-windows */
			//print_rel_map(c, a, y, x);

			///* Location relative to panel */
			//ky = y - Term.offset_y;

			///* Verify location */
			//if ((ky < 0) || (ky >= SCREEN_HGT)) return;

			///* Location relative to panel */
			//kx = x - Term.offset_x;

			///* Verify location */
			//if ((kx < 0) || (kx >= SCREEN_WID)) return;

			///* Get right position */
			//vx = COL_MAP + (tile_width * kx);
			//vy = ROW_MAP + (tile_height * ky);

			///* Hack -- Queue it */
			//Term_queue_char(Term, vx, vy, a, c, 0, 0);

			//if ((tile_width > 1) || (tile_height > 1))
			//        Term_big_queue_char(Term, vx, vy, a, c, 0, 0);
		}




		/*
		 * Memorize interesting viewable object/features in the given grid
		 *
		 * This function should only be called on "legal" grids.
		 *
		 * This function will memorize the object and/or feature in the given grid,
		 * if they are (1) see-able and (2) interesting.  Note that all objects are
		 * interesting, all terrain features except floors (and invisible traps) are
		 * interesting, and floors (and invisible traps) are interesting sometimes
		 * (depending on various options involving the illumination of floor grids).
		 *
		 * The automatic memorization of all objects and non-floor terrain features
		 * as soon as they are displayed allows incredible amounts of optimization
		 * in various places, especially "map_info()" and this function itself.
		 *
		 * Note that the memorization of objects is completely separate from the
		 * memorization of terrain features, preventing annoying floor memorization
		 * when a detected object is picked up from a dark floor, and object
		 * memorization when an object is dropped into a floor grid which is
		 * memorized but out-of-sight.
		 *
		 * This function should be called every time the "memorization" of a grid
		 * (or the object in a grid) is called into question, such as when an object
		 * is created in a grid, when a terrain feature "changes" from "floor" to
		 * "non-floor", and when any grid becomes "see-able" for any reason.
		 *
		 * This function is called primarily from the "update_view()" function, for
		 * each grid which becomes newly "see-able".
		 */
		public static void cave_note_spot(Cave c, int y, int x)
		{
			Object.Object o_ptr;

			/* Require "seen" flag */
			if ((c.info[y][x] & CAVE_SEEN) == 0)
			    return;

			for (o_ptr = Object.Object.get_first_object(y, x); o_ptr != null; o_ptr = Object.Object.get_next_object(o_ptr))
			    o_ptr.marked = 1; //true

			if ((c.info[y][x] & CAVE_MARK) == 0)
			    return;

			/* Memorize this grid */
			cave.info[y][x] |= (CAVE_MARK);
		}



		/*
		 * Redraw (on the screen) a given map location
		 *
		 * This function should only be called on "legal" grids.
		 */
		public static void cave_light_spot(Cave c, int y, int x)
		{
			Game_Event.signal_point(Game_Event.Event_Type.MAP, x, y);
		}


		static void prt_map_aux()
		{
			ConsoleColor a = ConsoleColor.White;
			char c = '\0';
			ConsoleColor ta = ConsoleColor.White;
			char tc = '\0';
			Grid_Data g = new Grid_Data();

			int y, x;
			int vy, vx;
			int ty, tx;

			int j;

			/* Scan windows */
			for (j = 0; j < Misc.ANGBAND_TERM_MAX; j++)
			{
			    Term t = Misc.angband_term[j];

			    /* No window */
			    if (t == null) continue;

			    /* No relevant flags */
			    if ((Player_Other.instance.window_flag[j] & (Misc.PW_MAP)) == 0) continue;

			    /* Assume screen */
			    ty = t.offset_y + (t.hgt / Term.tile_height);
			    tx = t.offset_x + (t.wid / Term.tile_width);

			    /* Dump the map */
			    for (y = t.offset_y, vy = 0; y < ty; vy++, y++)
			    {
			            if (vy + Term.tile_height - 1 >= t.hgt) continue;
			        for (x = t.offset_x, vx = 0; x < tx; vx++, x++)
			        {
			            /* Check bounds */
			            if (!in_bounds(y, x)) continue;

			            if (vx + Term.tile_width - 1 >= t.wid) continue;

			            /* Determine what is there */
			            Cave.map_info(y, x, ref g);
			            Cave.grid_data_as_text(ref g, ref a, ref c, ref ta, ref tc);
			            t.queue_char(vx, vy, a, c, ta, tc);

						if((Term.tile_width > 1) || (Term.tile_height > 1))
							throw new NotImplementedException();
			                //t.big_queue_char(vx, vy, 255, -1, 0, 0);
			        }
			    }
			}
		}



		/*
		 * Redraw (on the screen) the current map panel
		 *
		 * Note the inline use of "light_spot()" for efficiency.
		 *
		 * The main screen will always be at least 24x80 in size.
		 */
		public static void prt_map()
		{
			ConsoleColor a = ConsoleColor.White;
			char c = '\0';
			ConsoleColor ta = ConsoleColor.White;
			char tc = '\0';
			Grid_Data g = new Grid_Data();

			int y, x;
			int vy, vx;
			int ty, tx;

			/* Redraw map sub-windows */
			prt_map_aux();

			/* Assume screen */
			ty = Term.instance.offset_y + Misc.SCREEN_HGT;
			tx = Term.instance.offset_x + Misc.SCREEN_WID;

			/* Dump the map */
			for (y = Term.instance.offset_y, vy = Misc.ROW_MAP; y < ty; vy++, y++)
			{
			    for (x = Term.instance.offset_x, vx = Misc.COL_MAP; x < tx; vx++, x++)
			    {
			        /* Check bounds */
			        if (!in_bounds(y, x)) continue;

			        /* Determine what is there */
			        Cave.map_info(y, x, ref g);
			        Cave.grid_data_as_text(ref g, ref a, ref c, ref ta, ref tc);

			        /* Hack -- Queue it */
			        Term.instance.queue_char(vx, vy, a, c, ta, tc);

			        if ((Term.tile_width > 1) || (Term.tile_height > 1))
			        {
			            Term.instance.big_queue_char(vx, vy, a, c, ConsoleColor.White, ' ');
	      
			            if (Term.tile_width > 1)
			            {
							vx += Term.tile_width - 1;
			            }
			        }
			    }
      
			    if (Term.tile_height > 1)
					vy += Term.tile_height - 1;
      
			}
		}


		/*
		 * Display a "small-scale" map of the dungeon in the active Term.
		 *
		 * Note that this function must "disable" the special lighting effects so
		 * that the "priority" function will work.
		 *
		 * Note the use of a specialized "priority" function to allow this function
		 * to work with any graphic attr/char mappings, and the attempts to optimize
		 * this function where possible.
		 *
		 * If "cy" and "cx" are not null, then returns the screen location at which
		 * the player was displayed, so the cursor can be moved to that location,
		 * and restricts the horizontal map size to SCREEN_WID.  Otherwise, nothing
		 * is returned (obviously), and no restrictions are enforced.
		 */
		//cy and cx were int*
		public static void display_map(int[] cy, int[] cx)
		{
			throw new NotImplementedException();
			//int py = p_ptr.py;
			//int px = p_ptr.px;

			//int map_hgt, map_wid;
			//int dungeon_hgt, dungeon_wid;
			//int row, col;

			//int x, y;
			//grid_data g;

			//byte ta;
			//char tc;

			//byte tp;

			///* Large array on the stack */
			//byte mp[DUNGEON_HGT][DUNGEON_WID];

			//monster_race *r_ptr = &r_info[0];

			///* Desired map height */
			//map_hgt = Term.hgt - 2;
			//map_wid = Term.wid - 2;

			//dungeon_hgt = (p_ptr.depth == 0) ? TOWN_HGT : DUNGEON_HGT;
			//dungeon_wid = (p_ptr.depth == 0) ? TOWN_WID : DUNGEON_WID;

			///* Prevent accidents */
			//if (map_hgt > dungeon_hgt) map_hgt = dungeon_hgt;
			//if (map_wid > dungeon_wid) map_wid = dungeon_wid;

			///* Prevent accidents */
			//if ((map_wid < 1) || (map_hgt < 1)) return;


			///* Nothing here */
			//ta = TERM_WHITE;
			//tc = ' ';

			///* Clear the priorities */
			//for (y = 0; y < map_hgt; ++y)
			//{
			//    for (x = 0; x < map_wid; ++x)
			//    {
			//        /* No priority */
			//        mp[y][x] = 0;
			//    }
			//}


			///* Draw a box around the edge of the term */
			//window_make(0, 0, map_wid + 1, map_hgt + 1);

			///* Analyze the actual map */
			//for (y = 0; y < dungeon_hgt; y++)
			//{
			//    for (x = 0; x < dungeon_wid; x++)
			//    {
			//        row = (y * map_hgt / dungeon_hgt);
			//        col = (x * map_wid / dungeon_wid);

			//        if (tile_width > 1)
			//            col = col - (col % tile_width);
			//        if (tile_height > 1)
			//            row = row - (row % tile_height);

			//        /* Get the attr/char at that map location */
			//        map_info(y, x, &g);

			//        /* Get the priority of that attr/char */
			//        tp = f_info[g.f_idx].priority;

			//        /* Save "best" */
			//        if (mp[row][col] < tp)
			//        {
			//            /* Hack - make every grid on the map lit */
			//            g.lighting = FEAT_LIGHTING_BRIGHT;
			//            grid_data_as_text(&g, &ta, &tc, &ta, &tc);

			//            /* Add the character */
			//            Term_putch(col + 1, row + 1, ta, tc);

			//            if ((tile_width > 1) || (tile_height > 1))
			//                Term_big_putch(col + 1, row + 1, ta, tc);

			//            /* Save priority */
			//            mp[row][col] = tp;
			//        }
			//    }
			//}

			///*** Display the player ***/

			///* Player location */
			//row = (py * map_hgt / dungeon_hgt);
			//col = (px * map_wid / dungeon_wid);

			//if (tile_width > 1)
			//    col = col - (col % tile_width);
			//if (tile_height > 1)
			//    row = row - (row % tile_height);

			///* Get the "player" tile */
			//ta = r_ptr.x_attr;
			//tc = r_ptr.x_char;

			///* Draw the player */
			//Term_putch(col + 1, row + 1, ta, tc);

			//if ((tile_width > 1) || (tile_height > 1))
			//    Term_big_putch(col + 1, row + 1, ta, tc);
  
			///* Return player location */
			//if (cy != null) (*cy) = row + 1;
			//if (cx != null) (*cx) = col + 1;
		}


		/*
		 * Some comments on the dungeon related data structures and functions...
		 *
		 * Angband is primarily a dungeon exploration game, and it should come as
		 * no surprise that the internal representation of the dungeon has evolved
		 * over time in much the same way as the game itself, to provide semantic
		 * changes to the game itself, to make the code simpler to understand, and
		 * to make the executable itself faster or more efficient in various ways.
		 *
		 * There are a variety of dungeon related data structures, and associated
		 * functions, which store information about the dungeon, and provide methods
		 * by which this information can be accessed or modified.
		 *
		 * Some of this information applies to the dungeon as a whole, such as the
		 * list of unique monsters which are still alive.  Some of this information
		 * only applies to the current dungeon level, such as the current depth, or
		 * the list of monsters currently inhabiting the level.  And some of the
		 * information only applies to a single grid of the current dungeon level,
		 * such as whether the grid is illuminated, or whether the grid contains a
		 * monster, or whether the grid can be seen by the player.  If Angband was
		 * to be turned into a multi-player game, some of the information currently
		 * associated with the dungeon should really be associated with the player,
		 * such as whether a given grid is viewable by a given player.
		 *
		 * One of the major bottlenecks in ancient versions of Angband was in the
		 * calculation of "line of sight" from the player to various grids, such
		 * as those containing monsters, using the relatively expensive "los()"
		 * function.  This was such a nasty bottleneck that a lot of silly things
		 * were done to reduce the dependancy on "line of sight", for example, you
		 * could not "see" any grids in a lit room until you actually entered the
		 * room, at which point every grid in the room became "illuminated" and
		 * all of the grids in the room were "memorized" forever.  Other major
		 * bottlenecks involved the determination of whether a grid was lit by the
		 * player's torch, and whether a grid blocked the player's line of sight.
		 * These bottlenecks led to the development of special new functions to
		 * optimize issues involved with "line of sight" and "torch lit grids".
		 * These optimizations led to entirely new additions to the game, such as
		 * the ability to display the player's entire field of view using different
		 * colors than were used for the "memorized" portions of the dungeon, and
		 * the ability to memorize dark floor grids, but to indicate by the way in
		 * which they are displayed that they are not actually illuminated.  And
		 * of course many of them simply made the game itself faster or more fun.
		 * Also, over time, the definition of "line of sight" has been relaxed to
		 * allow the player to see a wider "field of view", which is slightly more
		 * realistic, and only slightly more expensive to maintain.
		 *
		 * Currently, a lot of the information about the dungeon is stored in ways
		 * that make it very efficient to access or modify the information, while
		 * still attempting to be relatively conservative about memory usage, even
		 * if this means that some information is stored in multiple places, or in
		 * ways which require the use of special code idioms.  For example, each
		 * monster record in the monster array contains the location of the monster,
		 * and each cave grid has an index into the monster array, or a zero if no
		 * monster is in the grid.  This allows the monster code to efficiently see
		 * where the monster is located, while allowing the dungeon code to quickly
		 * determine not only if a monster is present in a given grid, but also to
		 * find out which monster.  The extra space used to store the information
		 * twice is inconsequential compared to the speed increase.
		 *
		 * Some of the information about the dungeon is used by functions which can
		 * constitute the "critical efficiency path" of the game itself, and so the
		 * way in which they are stored and accessed has been optimized in order to
		 * optimize the game itself.  For example, the "update_view()" function was
		 * originally created to speed up the game itself (when the player was not
		 * running), but then it took on extra responsibility as the provider of the
		 * new "special effects lighting code", and became one of the most important
		 * bottlenecks when the player was running.  So many rounds of optimization
		 * were performed on both the function itself, and the data structures which
		 * it uses, resulting eventually in a function which not only made the game
		 * faster than before, but which was responsible for even more calculations
		 * (including the determination of which grids are "viewable" by the player,
		 * which grids are illuminated by the player's torch, and which grids can be
		 * "seen" in some way by the player), as well as for providing the guts of
		 * the special effects lighting code, and for the efficient redisplay of any
		 * grids whose visual representation may have changed.
		 *
		 * Several pieces of information about each cave grid are stored in various
		 * two dimensional arrays, with one unit of information for each grid in the
		 * dungeon.  Some of these arrays have been intentionally expanded by a small
		 * factor to make the two dimensional array accesses faster by allowing the
		 * use of shifting instead of multiplication.
		 *
		 * Several pieces of information about each cave grid are stored in the
		 * "cave.info" array, which is a special two dimensional array of bytes,
		 * one for each cave grid, each containing eight separate "flags" which
		 * describe some property of the cave grid.  These flags can be checked and
		 * modified extremely quickly, especially when special idioms are used to
		 * force the compiler to keep a local register pointing to the base of the
		 * array.  Special location offset macros can be used to minimize the number
		 * of computations which must be performed at runtime.  Note that using a
		 * byte for each flag set may be slightly more efficient than using a larger
		 * unit, so if another flag (or two) is needed later, and it must be fast,
		 * then the two existing flags which do not have to be fast should be moved
		 * out into some other data structure and the new flags should take their
		 * place.  This may require a few minor changes in the savefile code.
		 *
		 * The "CAVE_ROOM" flag is saved in the savefile and is used to determine
		 * which grids are part of "rooms", and thus which grids are affected by
		 * "illumination" spells.  This flag does not have to be very fast.
		 *
		 * The "CAVE_ICKY" flag is saved in the savefile and is used to determine
		 * which grids are part of "vaults", and thus which grids cannot serve as
		 * the destinations of player teleportation.  This flag does not have to
		 * be very fast.
		 *
		 * The "CAVE_MARK" flag is saved in the savefile and is used to determine
		 * which grids have been "memorized" by the player.  This flag is used by
		 * the "map_info()" function to determine if a grid should be displayed.
		 * This flag is used in a few other places to determine if the player can
		 * "know" about a given grid.  This flag must be very fast.
		 *
		 * The "CAVE_GLOW" flag is saved in the savefile and is used to determine
		 * which grids are "permanently illuminated".  This flag is used by the
		 * "update_view()" function to help determine which viewable flags may
		 * be "seen" by the player.  This flag is used by the "map_info" function
		 * to determine if a grid is only lit by the player's torch.  This flag
		 * has special semantics for wall grids (see "update_view()").  This flag
		 * must be very fast.
		 *
		 * The "CAVE_WALL" flag is used to determine which grids block the player's
		 * line of sight.  This flag is used by the "update_view()" function to
		 * determine which grids block line of sight, and to help determine which
		 * grids can be "seen" by the player.  This flag must be very fast.
		 *
		 * The "CAVE_VIEW" flag is used to determine which grids are currently in
		 * line of sight of the player.  This flag is set by (and used by) the
		 * "update_view()" function.  This flag is used by any code which needs to
		 * know if the player can "view" a given grid.  This flag is used by the
		 * "map_info()" function for some optional special lighting effects.  The
		 * "player_has_los_bold()" macro wraps an abstraction around this flag, but
		 * certain code idioms are much more efficient.  This flag is used to check
		 * if a modification to a terrain feature might affect the player's field of
		 * view.  This flag is used to see if certain monsters are "visible" to the
		 * player.  This flag is used to allow any monster in the player's field of
		 * view to "sense" the presence of the player.  This flag must be very fast.
		 *
		 * The "CAVE_SEEN" flag is used to determine which grids are currently in
		 * line of sight of the player and also illuminated in some way.  This flag
		 * is set by the "update_view()" function, using computations based on the
		 * "CAVE_VIEW" and "CAVE_WALL" and "CAVE_GLOW" flags of various grids.  This
		 * flag is used by any code which needs to know if the player can "see" a
		 * given grid.  This flag is used by the "map_info()" function both to see
		 * if a given "boring" grid can be seen by the player, and for some optional
		 * special lighting effects.  The "player_can_see_bold()" macro wraps an
		 * abstraction around this flag, but certain code idioms are much more
		 * efficient.  This flag is used to see if certain monsters are "visible" to
		 * the player.  This flag is never set for a grid unless "CAVE_VIEW" is also
		 * set for the grid.  Whenever the "CAVE_WALL" or "CAVE_GLOW" flag changes
		 * for a grid which has the "CAVE_VIEW" flag set, the "CAVE_SEEN" flag must
		 * be recalculated.  The simplest way to do this is to call "forget_view()"
		 * and "update_view()" whenever the "CAVE_WALL" or "CAVE_GLOW" flags change
		 * for a grid which has "CAVE_VIEW" set.  This flag must be very fast.
		 *
		 * The "CAVE_TEMP" flag is used for a variety of temporary purposes.  This
		 * flag is used to determine if the "CAVE_SEEN" flag for a grid has changed
		 * during the "update_view()" function.  This flag is used to "spread" light
		 * or darkness through a room.  This flag is used by the "monster flow code".
		 * This flag must always be cleared by any code which sets it, often, this
		 * can be optimized by the use of the special "temp_g" array.  This flag must
		 * be very fast.
		 *
		 * Note that the "CAVE_MARK" flag is used for many reasons, some of which
		 * are strictly for optimization purposes.  The "CAVE_MARK" flag means that
		 * even if the player cannot "see" the grid, he "knows" about the terrain in
		 * that grid.  This is used to "memorize" grids when they are first "seen" by
		 * the player, and to allow certain grids to be "detected" by certain magic.
		 * Note that most grids are always memorized when they are first "seen", but
		 * "boring" grids (floor grids) are only memorized if the "OPT(view_torch_grids)"
		 * option is set, or if the "OPT(view_perma_grids)" option is set, and the grid
		 * in question has the "CAVE_GLOW" flag set.
		 *
		 * Objects are "memorized" in a different way, using a special "marked" flag
		 * on the object itself, which is set when an object is observed or detected.
		 * This allows objects to be "memorized" independant of the terrain features.
		 *
		 * The "update_view()" function is an extremely important function.  It is
		 * called only when the player moves, significant terrain changes, or the
		 * player's blindness or torch radius changes.  Note that when the player
		 * is resting, or performing any repeated actions (like digging, disarming,
		 * farming, etc), there is no need to call the "update_view()" function, so
		 * even if it was not very efficient, this would really only matter when the
		 * player was "running" through the dungeon.  It sets the "CAVE_VIEW" flag
		 * on every cave grid in the player's field of view, and maintains an array
		 * of all such grids in the global "view_g" array.  It also checks the torch
		 * radius of the player, and sets the "CAVE_SEEN" flag for every grid which
		 * is in the "field of view" of the player and which is also "illuminated",
		 * either by the players torch (if any) or by any permanent light source.
		 * It could use and help maintain information about multiple light sources,
		 * which would be helpful in a multi-player version of Angband.
		 *
		 * The "update_view()" function maintains the special "view_g" array, which
		 * contains exactly those grids which have the "CAVE_VIEW" flag set.  This
		 * array is used by "update_view()" to (only) memorize grids which become
		 * newly "seen", and to (only) redraw grids whose "seen" value changes, which
		 * allows the use of some interesting (and very efficient) "special lighting
		 * effects".  In addition, this array could be used elsewhere to quickly scan
		 * through all the grids which are in the player's field of view.
		 *
		 * Note that the "update_view()" function allows, among other things, a room
		 * to be "partially" seen as the player approaches it, with a growing cone
		 * of floor appearing as the player gets closer to the door.  Also, by not
		 * turning on the "memorize perma-lit grids" option, the player will only
		 * "see" those floor grids which are actually in line of sight.  And best
		 * of all, you can now activate the special lighting effects to indicate
		 * which grids are actually in the player's field of view by using dimmer
		 * colors for grids which are not in the player's field of view, and/or to
		 * indicate which grids are illuminated only by the player's torch by using
		 * the color yellow for those grids.
		 *
		 * The old "update_view()" algorithm uses the special "CAVE_EASY" flag as a
		 * temporary internal flag to mark those grids which are not only in view,
		 * but which are also "easily" in line of sight of the player.  This flag
		 * is actually just the "CAVE_SEEN" flag, and the "update_view()" function
		 * makes sure to clear it for all old "CAVE_SEEN" grids, and then use it in
		 * the algorithm as "CAVE_EASY", and then clear it for all "CAVE_EASY" grids,
		 * and then reset it as appropriate for all new "CAVE_SEEN" grids.  This is
		 * kind of messy, but it works.  The old algorithm may disappear eventually.
		 *
		 * The new "update_view()" algorithm uses a faster and more mathematically
		 * correct algorithm, assisted by a large machine generated static array, to
		 * determine the "CAVE_VIEW" and "CAVE_SEEN" flags simultaneously.  See below.
		 *
		 * It seems as though slight modifications to the "update_view()" functions
		 * would allow us to determine "reverse" line-of-sight as well as "normal"
		 * line-of-sight", which would allow monsters to have a more "correct" way
		 * to determine if they can "see" the player, since right now, they "cheat"
		 * somewhat and assume that if the player has "line of sight" to them, then
		 * they can "pretend" that they have "line of sight" to the player.  But if
		 * such a change was attempted, the monsters would actually start to exhibit
		 * some undesirable behavior, such as "freezing" near the entrances to long
		 * hallways containing the player, and code would have to be added to make
		 * the monsters move around even if the player was not detectable, and to
		 * "remember" where the player was last seen, to avoid looking stupid.
		 *
		 * Note that the "CAVE_GLOW" flag means that a grid is permanently lit in
		 * some way.  However, for the player to "see" the grid, as determined by
		 * the "CAVE_SEEN" flag, the player must not be blind, the grid must have
		 * the "CAVE_VIEW" flag set, and if the grid is a "wall" grid, and it is
		 * not lit by the player's torch, then it must touch a grid which does not
		 * have the "CAVE_WALL" flag set, but which does have both the "CAVE_GLOW"
		 * and "CAVE_VIEW" flags set.  This last part about wall grids is induced
		 * by the semantics of "CAVE_GLOW" as applied to wall grids, and checking
		 * the technical requirements can be very expensive, especially since the
		 * grid may be touching some "illegal" grids.  Luckily, it is more or less
		 * correct to restrict the "touching" grids from the eight "possible" grids
		 * to the (at most) three grids which are touching the grid, and which are
		 * closer to the player than the grid itself, which eliminates more than
		 * half of the work, including all of the potentially "illegal" grids, if
		 * at most one of the three grids is a "diagonal" grid.  In addition, in
		 * almost every situation, it is possible to ignore the "CAVE_VIEW" flag
		 * on these three "touching" grids, for a variety of technical reasons.
		 * Finally, note that in most situations, it is only necessary to check
		 * a single "touching" grid, in fact, the grid which is strictly closest
		 * to the player of all the touching grids, and in fact, it is normally
		 * only necessary to check the "CAVE_GLOW" flag of that grid, again, for
		 * various technical reasons.  However, one of the situations which does
		 * not work with this last reduction is the very common one in which the
		 * player approaches an illuminated room from a dark hallway, in which the
		 * two wall grids which form the "entrance" to the room would not be marked
		 * as "CAVE_SEEN", since of the three "touching" grids nearer to the player
		 * than each wall grid, only the farthest of these grids is itself marked
		 * "CAVE_GLOW".
		 *
		 *
		 * Here are some pictures of the legal "light source" radius values, in
		 * which the numbers indicate the "order" in which the grids could have
		 * been calculated, if desired.  Note that the code will work with larger
		 * radiuses, though currently yields such a radius, and the game would
		 * become slower in some situations if it did.
		 *
		 *       Rad=0     Rad=1      Rad=2        Rad=3
		 *      No-Light Torch,etc   Lantern     Artifacts
		 *
		 *                                          333
		 *                             333         43334
		 *                  212       32123       3321233
		 *         @        1@1       31@13       331@133
		 *                  212       32123       3321233
		 *                             333         43334
		 *                                          333
		 *
		 *
		 * Here is an illustration of the two different "update_view()" algorithms,
		 * in which the grids marked "%" are pillars, and the grids marked "?" are
		 * not in line of sight of the player.
		 *
		 *
		 *                    Sample situation
		 *
		 *                  #####################
		 *                  ############.%.%.%.%#
		 *                  #...@..#####........#
		 *                  #............%.%.%.%#
		 *                  #......#####........#
		 *                  ############........#
		 *                  #####################
		 *
		 *
		 *          New Algorithm             Old Algorithm
		 *
		 *      ########?????????????    ########?????????????
		 *      #...@..#?????????????    #...@..#?????????????
		 *      #...........?????????    #.........???????????
		 *      #......#####.....????    #......####??????????
		 *      ########?????????...#    ########?????????????
		 *
		 *      ########?????????????    ########?????????????
		 *      #.@....#?????????????    #.@....#?????????????
		 *      #............%???????    #...........?????????
		 *      #......#####........?    #......#####?????????
		 *      ########??????????..#    ########?????????????
		 *
		 *      ########?????????????    ########?????%???????
		 *      #......#####........#    #......#####..???????
		 *      #.@..........%???????    #.@..........%???????
		 *      #......#####........#    #......#####..???????
		 *      ########?????????????    ########?????????????
		 *
		 *      ########??????????..#    ########?????????????
		 *      #......#####........?    #......#####?????????
		 *      #............%???????    #...........?????????
		 *      #.@....#?????????????    #.@....#?????????????
		 *      ########?????????????    ########?????????????
		 *
		 *      ########?????????%???    ########?????????????
		 *      #......#####.....????    #......####??????????
		 *      #...........?????????    #.........???????????
		 *      #...@..#?????????????    #...@..#?????????????
		 *      ########?????????????    ########?????????????
		 */




		/*
		 * Maximum number of grids in a single octant
		 */
		public const int VINFO_MAX_GRIDS = 161;


		/*
		 * Maximum number of slopes in a single octant
		 */
		public const int VINFO_MAX_SLOPES = 126;


		/*
		 * Mask of bits used in a single octant
		 */
		public const long VINFO_BITS_3 = 0x3FFFFFFF;
		public const long VINFO_BITS_2 = 0xFFFFFFFF;
		public const long VINFO_BITS_1 = 0xFFFFFFFF;
		public const long VINFO_BITS_0 = 0xFFFFFFFF;


		/*
		 * The 'vinfo_type' structure
		 */
		class vinfo_type
		{
			public short[] grid = new short[8];

			public uint bits_3;
			public uint bits_2;
			public uint bits_1;
			public uint bits_0;

			public vinfo_type next_0;
			public vinfo_type next_1;

			public byte y;
			public byte x;
			public byte d;
			public byte r;
		};



		/*
		 * The array of "vinfo" objects, initialized by "vinfo_init()"
		 */
		static vinfo_type[] vinfo = new vinfo_type[VINFO_MAX_GRIDS];




		/*
		 * Slope scale factor
		 */
		public const long SCALE = 100000L;


		/*
		 * The actual slopes (for reference)
		 */

		/* Bit :     Slope   Grids */
		/* --- :     -----   ----- */
		/*   0 :      2439      21 */
		/*   1 :      2564      21 */
		/*   2 :      2702      21 */
		/*   3 :      2857      21 */
		/*   4 :      3030      21 */
		/*   5 :      3225      21 */
		/*   6 :      3448      21 */
		/*   7 :      3703      21 */
		/*   8 :      4000      21 */
		/*   9 :      4347      21 */
		/*  10 :      4761      21 */
		/*  11 :      5263      21 */
		/*  12 :      5882      21 */
		/*  13 :      6666      21 */
		/*  14 :      7317      22 */
		/*  15 :      7692      20 */
		/*  16 :      8108      21 */
		/*  17 :      8571      21 */
		/*  18 :      9090      20 */
		/*  19 :      9677      21 */
		/*  20 :     10344      21 */
		/*  21 :     11111      20 */
		/*  22 :     12000      21 */
		/*  23 :     12820      22 */
		/*  24 :     13043      22 */
		/*  25 :     13513      22 */
		/*  26 :     14285      20 */
		/*  27 :     15151      22 */
		/*  28 :     15789      22 */
		/*  29 :     16129      22 */
		/*  30 :     17241      22 */
		/*  31 :     17647      22 */
		/*  32 :     17948      23 */
		/*  33 :     18518      22 */
		/*  34 :     18918      22 */
		/*  35 :     20000      19 */
		/*  36 :     21212      22 */
		/*  37 :     21739      22 */
		/*  38 :     22580      22 */
		/*  39 :     23076      22 */
		/*  40 :     23809      22 */
		/*  41 :     24137      22 */
		/*  42 :     24324      23 */
		/*  43 :     25714      23 */
		/*  44 :     25925      23 */
		/*  45 :     26315      23 */
		/*  46 :     27272      22 */
		/*  47 :     28000      23 */
		/*  48 :     29032      23 */
		/*  49 :     29411      23 */
		/*  50 :     29729      24 */
		/*  51 :     30434      23 */
		/*  52 :     31034      23 */
		/*  53 :     31428      23 */
		/*  54 :     33333      18 */
		/*  55 :     35483      23 */
		/*  56 :     36000      23 */
		/*  57 :     36842      23 */
		/*  58 :     37142      24 */
		/*  59 :     37931      24 */
		/*  60 :     38461      24 */
		/*  61 :     39130      24 */
		/*  62 :     39393      24 */
		/*  63 :     40740      24 */
		/*  64 :     41176      24 */
		/*  65 :     41935      24 */
		/*  66 :     42857      23 */
		/*  67 :     44000      24 */
		/*  68 :     44827      24 */
		/*  69 :     45454      23 */
		/*  70 :     46666      24 */
		/*  71 :     47368      24 */
		/*  72 :     47826      24 */
		/*  73 :     48148      24 */
		/*  74 :     48387      24 */
		/*  75 :     51515      25 */
		/*  76 :     51724      25 */
		/*  77 :     52000      25 */
		/*  78 :     52380      25 */
		/*  79 :     52941      25 */
		/*  80 :     53846      25 */
		/*  81 :     54838      25 */
		/*  82 :     55555      24 */
		/*  83 :     56521      25 */
		/*  84 :     57575      26 */
		/*  85 :     57894      25 */
		/*  86 :     58620      25 */
		/*  87 :     60000      23 */
		/*  88 :     61290      25 */
		/*  89 :     61904      25 */
		/*  90 :     62962      25 */
		/*  91 :     63636      25 */
		/*  92 :     64705      25 */
		/*  93 :     65217      25 */
		/*  94 :     65517      25 */
		/*  95 :     67741      26 */
		/*  96 :     68000      26 */
		/*  97 :     68421      26 */
		/*  98 :     69230      26 */
		/*  99 :     70370      26 */
		/* 100 :     71428      25 */
		/* 101 :     72413      26 */
		/* 102 :     73333      26 */
		/* 103 :     73913      26 */
		/* 104 :     74193      27 */
		/* 105 :     76000      26 */
		/* 106 :     76470      26 */
		/* 107 :     77777      25 */
		/* 108 :     78947      26 */
		/* 109 :     79310      26 */
		/* 110 :     80952      26 */
		/* 111 :     81818      26 */
		/* 112 :     82608      26 */
		/* 113 :     84000      26 */
		/* 114 :     84615      26 */
		/* 115 :     85185      26 */
		/* 116 :     86206      27 */
		/* 117 :     86666      27 */
		/* 118 :     88235      27 */
		/* 119 :     89473      27 */
		/* 120 :     90476      27 */
		/* 121 :     91304      27 */
		/* 122 :     92000      27 */
		/* 123 :     92592      27 */
		/* 124 :     93103      28 */
		/* 125 :    100000      13 */


		/*
		 * Temporary data used by "vinfo_init()"
		 *
		 *	- Number of grids
		 *
		 *	- Number of slopes
		 *
		 *	- Slope values
		 *
		 *	- Slope range per grid
		 */
		class vinfo_hack {
			public int num_slopes;

			public long[] slopes = new long[VINFO_MAX_SLOPES];

			//Were jagged arrays. Square arrays seem to work fine.
			public long[,] slopes_min = new long[Misc.MAX_SIGHT+1, Misc.MAX_SIGHT+1];
			public long[,] slopes_max = new long[Misc.MAX_SIGHT+1, Misc.MAX_SIGHT+1];
		};

		/**
		 * FEATURE PREDICATES
		 *
		 * These functions are used to figure out what kind of square something is,
		 * via c.feat[y][x]. All direct testing of c.feat[y][x] should be rewritten
		 * in terms of these functions.
		 *
		 * It's often better to use feature behavior predicates (written in terms of
		 * these functions) instead of these functions directly. For instance,
		 * cave_isrock() will return false for a secret door, even though it will
		 * behave like a rock wall until the player determines it's a door.
		 *
		 * Use functions like cave_isdiggable, cave_iswall, etc. in these cases.
		 */

		/**
		 * true if the square is normal open floor.
		 */
		public static bool cave_isfloor(Cave c, int y, int x) {
			return c.feat[y][x] == FEAT_FLOOR;
		}

		/**
		 * true if the square is a normal granite rock wall.
		 *
		 * FEAT_WALL_SOLID is the normal feature type. The others are weird byproducts
		 * of cave generation (and should be avoided).
		 */
		public static bool cave_isrock(Cave c, int y, int x) {
			switch (c.feat[y][x]) {
			    case FEAT_WALL_EXTRA:
			    case FEAT_WALL_INNER:
			    case FEAT_WALL_OUTER:
			    case FEAT_WALL_SOLID: return true;
			    default: return false;
			}
		}

		/**
		 * true if the square is a permanent wall.
		 *
		 * FEAT_PERM_SOLID is the normal feature type. The others are weird byproducts
		 * of cave generation (and should be avoided).
		 */
		public static bool cave_isperm(Cave c, int y, int x) {
			switch (c.feat[y][x]) {
			    case FEAT_PERM_EXTRA:
			    case FEAT_PERM_INNER:
			    case FEAT_PERM_OUTER:
			    case FEAT_PERM_SOLID: return true;
			    default: return false;
			}
		}

		/**
		 * true if the square is a magma wall.
		 */
		public static bool cave_ismagma(Cave c, int y, int x) {
			switch (c.feat[y][x]) {
			    case FEAT_MAGMA:
			    case FEAT_MAGMA_H:
			    case FEAT_MAGMA_K: return true;
			    default: return false;
			}
		}

		/**
		 * true if the square is a quartz wall.
		 */
		public static bool cave_isquartz(Cave c, int y, int x) {
			switch (c.feat[y][x]) {
			    case FEAT_QUARTZ:
			    case FEAT_QUARTZ_H:
			    case FEAT_QUARTZ_K: return true;
			    default: return false;
			}
		}

		/**
		 * true if the square is a mineral wall (magma/quartz).
		 */
		public static bool cave_ismineral(Cave c, int y, int x) {
			return cave_isrock(c, y, x) || cave_ismagma(c, y, x) || cave_isquartz(c, y, x);
		}

		/**
		 * true if the square is rubble.
		 */
		public static bool cave_isrubble(Cave c, int y, int x) {
			return c.feat[y][x] == FEAT_RUBBLE;
		}

		/**
		 * true if the square is a hidden secret door.
		 *
		 * These squares appear as if they were granite--when detected a secret door
		 * is replaced by a closed door.
		 */
		public static bool cave_issecretdoor(Cave c, int y, int x) {
			return c.feat[y][x] == FEAT_SECRET;
		}

		/**
		 * true if the square is an open door.
		 */
		public static bool cave_isopendoor(Cave c, int y, int x) {
			return c.feat[y][x] == FEAT_OPEN;
		}

		/**
		 * true if the square is a closed door (possibly locked or jammed).
		 */
		public static bool cave_iscloseddoor(Cave c, int y, int x) {
			int feat = c.feat[y][x];
			return feat >= FEAT_DOOR_HEAD && feat <= FEAT_DOOR_TAIL;
		}

		/**
		 * true if the square is a closed, locked door.
		 */
		public static bool cave_islockeddoor(Cave c, int y, int x) {
			int feat = c.feat[y][x];
			return feat >= FEAT_DOOR_HEAD + 0x01 && feat <= FEAT_DOOR_TAIL;
		}

		/**
		 * true if the square is a closed, jammed door.
		 */
		public static bool cave_isjammeddoor(Cave c, int y, int x) {
			int feat = c.feat[y][x];
			return feat >= FEAT_DOOR_HEAD + 0x08 && feat <= FEAT_DOOR_TAIL;
		}

		/**
		 * true if the square is a door.
		 *
		 * This includes open, closed, and hidden doors.
		 */
		public static bool cave_isdoor(Cave c, int y, int x) {
			return (cave_isopendoor(c, y, x) ||
			        cave_issecretdoor(c, y, x) ||
			        cave_iscloseddoor(cave, y, x));
		}

		/**
		 * true if the square is an unknown trap (it will appear as a floor tile).
		 */
		public static bool cave_issecrettrap(Cave c, int y, int x) {
			return c.feat[y][x] == FEAT_INVIS;
		}

		/**
		 * true if the square is a known trap.
		 */
		public static bool cave_isknowntrap(Cave c, int y, int x) {
			int feat = c.feat[y][x];
			return feat >= FEAT_TRAP_HEAD && feat <= FEAT_TRAP_TAIL;
		}

		/**
		 * true if the square contains a trap, known or unknown.
		 */
		public static bool cave_istrap(Cave c, int y, int x) {
			return cave_issecrettrap(cave, y, x) || cave_isknowntrap(cave, y, x);
		}



		/**
		 * SQUARE BEHAVIOR PREDICATES
		 *
		 * These functions define how a given square behaves, e.g. whether it is
		 * passable by the player, whether it is diggable, contains items, etc.
		 *
		 * These functions use the FEATURE PREDICATES (as well as c.info) to make
		 * the determination.
		 */

		/**
		 * true if the square is open (a floor square not occupied by a monster).
		 */
		public static bool cave_isopen(Cave c, int y, int x) {
			return cave_isfloor(c, y, x) && (c.m_idx[y][x] == 0);
		}

		/**
		 * true if the square is empty (an open square without any items).
		 */
		public static bool cave_isempty(Cave c, int y, int x) {
			return cave_isopen(c, y, x) && (c.o_idx[y][x] == 0);
		}

		/**
		 * true if the square is a floor square without items.
		 */
		public static bool cave_canputitem(Cave c, int y, int x) {
			return cave_isfloor(c, y, x) && c.o_idx[y][x] == 0;
		}

		/**
		 * true if the square can be dug: this includes rubble and non-permanent walls.
		 */
		public static bool cave_isdiggable(Cave c, int y, int x) {
			return (cave_ismineral(c, y, x) ||
			        cave_issecretdoor(c, y, x) || 
			        cave_isrubble(c, y, x));
		}

		/**
		 * true if the square is passable by the player.
		 *
		 * This function is the logical negation of cave_iswall().
		 */
		public static bool cave_ispassable(Cave c, int y, int x) {
			return (c.info[y][x] & CAVE_WALL) == 0;
		}

		/**
		 * true if the square is a wall square (impedes the player).
		 *
		 * This function is the logical negation of cave_ispassable().
		 */
		public static bool cave_iswall(Cave c, int y, int x) {
			return ((c.info[y][x] & CAVE_WALL) != 0);
		}

		/**
		 * true if the square is a permanent wall or one of the "stronger" walls.
		 *
		 * The stronger walls are granite, magma and quartz. This excludes things like
		 * secret doors and rubble.
		 */
		public static bool cave_isstrongwall(Cave c, int y, int x) {
			return cave_ismineral(c, y, x) || cave_isperm(c, y, x);
		}

		/**
		 * true if the square is part of a vault.
		 *
		 * This doesn't say what kind of square it is, just that it is part of a vault.
		 */
		public static bool cave_isvault(Cave c, int y, int x) {
			return (c.info[y][x] & CAVE_ICKY) != 0;
		}

		/**
		 * true if the square is part of a room.
		 */
		public static bool cave_isroom(Cave c, int y, int x) {
			return (c.info[y][x] & CAVE_ROOM) != 0;
		}

		/**
		 * true if cave square is a feeling trigger square 
		 */
		public static bool cave_isfeel(Cave c, int y, int x){
			return (c.info2[y][x] & CAVE2_FEEL) != 0;
		}

		/**
		 * Get a monster on the current level by its index.
		 */
		public static Monster.Monster cave_monster(Cave c, int idx) {
			return c.monsters[idx];
		}

		/**
		 * Set a monster on the current level by its index.
		 */
		public static void cave_monster_set(Cave c, int idx, Monster.Monster m) {
			c.monsters[idx] = m;
		}

		/**
		 * The maximum number of monsters allowed in the level.
		 */
		public static int cave_monster_max(Cave c) {
			return c.mon_max;
		}

		/**
		 * The current number of monsters present on the level.
		 */
		public static int cave_monster_count(Cave c) {
			return c.mon_cnt;
		}

		/**
		 * Add visible treasure to a mineral square.
		 */
		public static void upgrade_mineral(Cave c, int y, int x) {
			switch (c.feat[y][x]) {
			    case FEAT_MAGMA: cave_set_feat(c, y, x, FEAT_MAGMA_K); break;
			    case FEAT_QUARTZ: cave_set_feat(c, y, x, FEAT_QUARTZ_K); break;
			}
		}
	}
}
