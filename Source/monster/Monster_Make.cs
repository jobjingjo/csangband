using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;

namespace CSAngband.Monster {
	class Monster_Make {
		/*
		 * There is a 1/50 (2%) chance of inflating the requested monster level
		 * during the creation of a monsters (see "get_mon_num()" in "monster.c").
		 * Lower values yield harder monsters more often.
		 */
		public const int  NASTY_MON  =  25;        /* 1/chance of inflated monster level */
		public const int MON_OOD_MAX = 10;     /* maximum out-of-depth amount */

		/**
		 * Calculate hp for a monster. This function assumes that the Rand_normal
		 * function has limits of +/- 4x std_dev. If that changes, this function
		 * will become inaccurate.
		 *
		 * \param r_ptr is the race of the monster in question.
		 * \param hp_aspect is the hp calc we want (min, max, avg, random).
		 */
		public static int mon_hp(Monster_Race r_ptr, aspect hp_aspect)
		{
				int std_dev = (((r_ptr.avg_hp * 10) / 8) + 5) / 10;

				if (r_ptr.avg_hp > 1) std_dev++;

				switch (hp_aspect) {
					case aspect.MINIMISE:
						return (r_ptr.avg_hp - (4 * std_dev));
					case aspect.MAXIMISE:
					case aspect.EXTREMIFY:
						return (r_ptr.avg_hp + (4 * std_dev));
					case aspect.AVERAGE:
						return r_ptr.avg_hp;
					default:
						return Random.Rand_normal(r_ptr.avg_hp, std_dev);
				}
		}

		/*
		 * Attempt to allocate a random monster in the dungeon.
		 *
		 * Place the monster at least "dis" distance from the player.
		 *
		 * Use "slp" to choose the initial "sleep" status
		 *
		 * Use "depth" for the monster level
		 */
		public static bool pick_and_place_distant_monster(Cave c, Loc loc, int dis, bool slp, int depth)
		{
			int py = loc.y;
			int px = loc.x;

			int y = 0, x = 0;
			int	attempts_left = 10000;

			Misc.assert(c != null);

			/* Find a legal, distant, unoccupied, space */
			while (--attempts_left != 0)
			{
				/* Pick a location */
				y = Random.randint0(c.height);
				x = Random.randint0(c.width);

				/* Require "naked" floor grid */
				if (!Cave.cave_isempty(c, y, x)) continue;

				/* Accept far away grids */
				if (Cave.distance(y, x, py, px) > dis) break;
			}

			if (attempts_left == 0)
			{
				if (Option.cheat_xtra.value || Option.cheat_hear.value)
				{
					Utilities.msg("Warning! Could not allocate a new monster.");
				}

				return false;
			}

			/* Attempt to place the monster, allow groups */
			if (pick_and_place_monster(c, y, x, depth, slp, true, Object.Origin.DROP)) return (true);

			/* Nope */
			return (false);
		}

		/*
		 * Pick a monster race, make a new monster of that race, then place
		 * it in the dungeon.
		 */
		public static bool pick_and_place_monster(Cave c, int y, int x, int depth, bool slp, bool grp, Object.Origin origin)
		{
			int r_idx;

			/* Pick a monster race */
			r_idx = get_mon_num(depth);

			/* Handle failure */
			if (r_idx == 0) return (false);

			/* Attempt to place the monster */
			return (place_new_monster(c, y, x, r_idx, slp, grp, origin));
		}

		/*
		 * Choose a monster race that seems "appropriate" to the given level
		 *
		 * This function uses the "prob2" field of the "monster allocation table",
		 * and various local information, to calculate the "prob3" field of the
		 * same table, which is then used to choose an "appropriate" monster, in
		 * a relatively efficient manner.
		 *
		 * Note that "town" monsters will *only* be created in the town, and
		 * "normal" monsters will *never* be created in the town, unless the
		 * "level" is "modified", for example, by polymorph or summoning.
		 *
		 * There is a small chance (1/50) of "boosting" the given depth by
		 * a small amount (up to four levels), except in the town.
		 *
		 * It is (slightly) more likely to acquire a monster of the given level
		 * than one of a lower level.  This is done by choosing several monsters
		 * appropriate to the given level and keeping the "hardest" one.
		 *
		 * Note that if no monsters are "appropriate", then this function will
		 * fail, and return zero, but this should *almost* never happen.
		 */
		public static short get_mon_num(int level)
		{
			int i, j, p;

			int r_idx;

			long value, total;

			Monster_Race r_ptr;

			Init.alloc_entry[] table = Init.alloc_race_table;

			/* Occasionally produce a nastier monster in the dungeon */
			if (level > 0 && Random.one_in_(NASTY_MON))
				level += Math.Min(level / 4 + 2, MON_OOD_MAX);

			/* Reset total */
			total = 0L;

			/* Process probabilities */
			for (i = 0; i < Init.alloc_race_size; i++)
			{
				/* Monsters are sorted by depth */
				if (table[i].level > level) break;

				/* Default */
				table[i].prob3 = 0;

				/* Hack -- No town monsters in dungeon */
				if ((level > 0) && (table[i].level <= 0)) continue;

				/* Get the "r_idx" of the chosen monster */
				r_idx = table[i].index;

				/* Get the actual race */
				r_ptr = Misc.r_info[r_idx];

				/* Hack -- "unique" monsters must be "unique" */
				if (r_ptr.flags.has(Monster_Flag.UNIQUE.value) &&
					r_ptr.cur_num >= r_ptr.max_num)
				{
					continue;
				}

				/* Depth Monsters never appear out of depth */
				if (r_ptr.flags.has(Monster_Flag.FORCE_DEPTH.value) && r_ptr.level > Misc.p_ptr.depth)
				{
					continue;
				}

				/* Accept */
				table[i].prob3 = table[i].prob2;

				/* Total */
				total += table[i].prob3;
			}

			/* No legal monsters */
			if (total <= 0) return (0);


			/* Pick a monster */
			value = Random.randint0((int)total);

			/* Find the monster */
			for (i = 0; i < Init.alloc_race_size; i++)
			{
				/* Found the entry */
				if (value < table[i].prob3) break;

				/* Decrement */
				value = value - table[i].prob3;
			}


			/* Power boost */
			p = Random.randint0(100);

			/* Try for a "harder" monster once (50%) or twice (10%) */
			if (p < 60)
			{
				/* Save old */
				j = i;

				/* Pick a monster */
				value = Random.randint0((int)total);

				/* Find the monster */
				for (i = 0; i < Init.alloc_race_size; i++)
				{
					/* Found the entry */
					if (value < table[i].prob3) break;

					/* Decrement */
					value = value - table[i].prob3;
				}

				/* Keep the "best" one */
				if (table[i].level < table[j].level) i = j;
			}

			/* Try for a "harder" monster twice (10%) */
			if (p < 10)
			{
				/* Save old */
				j = i;

				/* Pick a monster */
				value = Random.randint0((int)total);

				/* Find the monster */
				for (i = 0; i < Init.alloc_race_size; i++)
				{
					/* Found the entry */
					if (value < table[i].prob3) break;

					/* Decrement */
					value = value - table[i].prob3;
				}

				/* Keep the "best" one */
				if (table[i].level < table[j].level) i = j;
			}


			/* Result */
			return (table[i].index);
		}

		/*
		 * Attempt to place a monster of the given race at the given location.
		 *
		 * To give the player a sporting chance, any monster that appears in
		 * line-of-sight and is extremely dangerous can be marked as
		 * "FORCE_SLEEP", which will cause them to be placed with low energy,
		 * which often (but not always) lets the player move before they do.
		 *
		 * This routine refuses to place out-of-depth "FORCE_DEPTH" monsters.
		 *
		 * XXX XXX XXX Use special "here" and "dead" flags for unique monsters,
		 * remove old "cur_num" and "max_num" fields.
		 *
		 * XXX XXX XXX Actually, do something similar for artifacts, to simplify
		 * the "preserve" mode, and to make the "what artifacts" flag more useful.
		 *
		 * This is the only function which may place a monster in the dungeon,
		 * except for the savefile loading code.
		 */
		static bool place_new_monster_one(int y, int x, int r_idx, bool slp, byte origin)
		{
			int i;

			Monster_Race r_ptr;
			Monster n_ptr;
			//Monster monster_type_body;

			string name;

			/* Paranoia */
			if (!Cave.in_bounds(y, x)) return (false);

			/* Require empty space */
			if (!Cave.cave_empty_bold(y, x)) return (false);

			/* No creation on glyph of warding */
			if (Cave.cave.feat[y][x] == Cave.FEAT_GLYPH) return (false);

			/* Paranoia */
			if (r_idx == 0) return (false);

			/* Race */
			r_ptr = Misc.r_info[r_idx];
			if (r_ptr == null) return (false);

			/* Paranoia */
			if (r_ptr.Name == null) return (false);

			name = r_ptr.Name;

			/* "unique" monsters must be "unique" */
			if (r_ptr.flags.has(Monster_Flag.UNIQUE.value) && r_ptr.cur_num >= r_ptr.max_num)
				return (false);

			/* Depth monsters may NOT be created out of depth */
			if (r_ptr.flags.has(Monster_Flag.FORCE_DEPTH.value) && Misc.p_ptr.depth < r_ptr.level)
				return (false);

			/* Add to level feeling */
			Cave.cave.mon_rating += (uint)(r_ptr.power / 20);

			/* Check out-of-depth-ness */
			if (r_ptr.level > Misc.p_ptr.depth) {
				if (r_ptr.flags.has(Monster_Flag.UNIQUE.value)) { /* OOD unique */
					if (Option.cheat_hear.value)
						Utilities.msg("Deep Unique (%s).", name);
				} else { /* Normal monsters but OOD */
					if (Option.cheat_hear.value)
						Utilities.msg("Deep Monster (%s).", name);
				}
				/* Boost rating by power per 10 levels OOD */
				Cave.cave.mon_rating += (uint)((r_ptr.level - Misc.p_ptr.depth) * r_ptr.power / 200);
			}
			/* Note uniques for cheaters */
			else if (r_ptr.flags.has(Monster_Flag.UNIQUE.value) && Option.cheat_hear.value)
				Utilities.msg("Unique (%s).", name);

			/* Get local monster */
			n_ptr = new Monster();//&monster_type_body;

			/* Clean out the monster */
			n_ptr.WIPE();
			//(void)WIPE(n_ptr, monster_type);

			/* Save the race */
			n_ptr.r_idx = (short)r_idx;

			/* Enforce sleeping if needed */
			if (slp && r_ptr.sleep != 0) {
				int val = r_ptr.sleep;
				n_ptr.m_timed[(int)Misc.MON_TMD.SLEEP] = (short)((val * 2) + Random.randint1(val * 10));
			}

			/* Uniques get a fixed amount of HP */
			if (r_ptr.flags.has(Monster_Flag.UNIQUE.value))
				n_ptr.maxhp = (short)r_ptr.avg_hp;
			else {
				n_ptr.maxhp = (short)mon_hp(r_ptr, aspect.RANDOMISE);
				n_ptr.maxhp = (short)Math.Max((int)n_ptr.maxhp, 1);
			}

			/* And start out fully healthy */
			n_ptr.hp = n_ptr.maxhp;

			/* Extract the monster base speed */
			n_ptr.mspeed = r_ptr.speed;

			/* Hack -- small racial variety */
			if (!r_ptr.flags.has(Monster_Flag.UNIQUE.value)) {
				/* Allow some small variation per monster */
				i = Misc.extract_energy[r_ptr.speed] / 10;
				if (i != 0) n_ptr.mspeed += (byte)Random.rand_spread(0, i);
			}

			/* Give a random starting energy */
			n_ptr.energy = (byte)Random.randint0(50);

			/* Force monster to wait for player */
			if (r_ptr.flags.has(Monster_Flag.FORCE_SLEEP.value))
				n_ptr.mflag |= (Monster_Flag.MFLAG_NICE);

			/* Radiate light? */
			if (r_ptr.flags.has(Monster_Flag.HAS_LIGHT.value))
				Misc.p_ptr.update |= Misc.PU_UPDATE_VIEW;
	
			/* Is this obviously a monster? (Mimics etc. aren't) */
			if (r_ptr.flags.has(Monster_Flag.UNAWARE.value)) 
				n_ptr.unaware = true;
			else
				n_ptr.unaware = false;

			/* Set the color if necessary */
			if (r_ptr.flags.has(Monster_Flag.ATTR_RAND.value))
				n_ptr.attr = Utilities.num_to_attr(Random.randint1(Enum.GetValues(typeof(ConsoleColor)).Length - 1));

			/* Place the monster in the dungeon */
			if (place_monster(y, x, n_ptr, origin) == 0)
				return (false);

			/* Success */
			return (true);
		}

		/**
		 * Place a copy of a monster in the dungeon XXX XXX
		 */
		public static short place_monster(int y, int x, Monster n_ptr, byte origin)
		{
			short m_idx;

			Monster_Race r_ptr;

			/* Paranoia XXX XXX */
			if (Cave.cave.m_idx[y][x] != 0) return 0;

			/* Get a new record */
			m_idx = mon_pop();

			if (m_idx == 0) return 0;
			n_ptr.midx = m_idx;

			/* Make a new monster */
			Cave.cave.m_idx[y][x] = m_idx;

			/* Get the new monster */
			//m_ptr = Cave.cave_monster(Cave.cave, m_idx);

			/* Copy the monster XXX */
			//m_ptr.COPY(n_ptr);

			//Nick: Set the new monster... Much better than the above two steps...
			Cave.cave_monster_set(Cave.cave, m_idx, n_ptr);

			/* Location */
			n_ptr.fy = (byte)y;
			n_ptr.fx = (byte)x;

			/* Update the monster */
			Monster.update_mon(m_idx, true);

			/* Get the new race */
			r_ptr = Misc.r_info[n_ptr.r_idx];

			/* Hack -- Count the number of "reproducers" */
			if (r_ptr.flags.has(Monster_Flag.MULTIPLY.value)) Misc.num_repro++;

			/* Count racial occurances */
			r_ptr.cur_num++;

			/* Create the monster's drop, if any */
			if(origin != 0) {
				mon_create_drop(m_idx, origin);
			}

			/* Make mimics start mimicking */
			if (origin != 0 && r_ptr.mimic_kinds != null) {
				throw new NotImplementedException();
				//object_type *i_ptr;
				//object_type object_type_body;
				//object_kind *kind = r_ptr.mimic_kinds.kind;
				//struct monster_mimic *mimic_kind;
				//int i = 1;
		
				///* Pick a random object kind to mimic */
				//for (mimic_kind = r_ptr.mimic_kinds; mimic_kind; mimic_kind = mimic_kind.next, i++) {
				//    if (one_in_(i)) kind = mimic_kind.kind;
				//}

				//i_ptr = &object_type_body;

				//if (kind.tval == TV_GOLD) {
				//    make_gold(i_ptr, p_ptr.depth, kind.sval);
				//} else {
				//    object_prep(i_ptr, kind, r_ptr.level, RANDOMISE);
				//    apply_magic(i_ptr, r_ptr.level, true, false, false);
				//    i_ptr.number = 1;
				//}

				//i_ptr.mimicking_m_idx = m_idx;
				//m_ptr.mimicked_o_idx = floor_carry(cave, y, x, i_ptr);
			}

			/* Result */
			return m_idx;
		}

		/**
		 * Create a specific monster's drop, including any specified drops.
		 *
		 * Returns true if anything is created, false if nothing is.
		 */
		static bool mon_create_drop(int m_idx, byte origin)
		{
			Monster_Drop drop;

			Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);
			Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			bool great = (r_ptr.flags.has(Monster_Flag.DROP_GREAT.value)) ? true : false;
			bool good = (r_ptr.flags.has(Monster_Flag.DROP_GOOD.value) ? true : false) || great;
			bool any = false;
			bool gold_ok = (!r_ptr.flags.has(Monster_Flag.ONLY_ITEM.value));
			bool item_ok = (!r_ptr.flags.has(Monster_Flag.ONLY_GOLD.value));

			int number = 0, level, j;
			int force_coin = get_coin_type(r_ptr);

			Object.Object i_ptr;
			//object_type object_type_body;

			/* Determine how much we can drop */
			if (r_ptr.flags.has(Monster_Flag.DROP_20.value) && Random.randint0(100) < 20) number++;
			if (r_ptr.flags.has(Monster_Flag.DROP_40.value) && Random.randint0(100) < 40) number++;
			if (r_ptr.flags.has(Monster_Flag.DROP_60.value) && Random.randint0(100) < 60) number++;
			if (r_ptr.flags.has(Monster_Flag.DROP_4.value)) number += Random.rand_range(2, 6);
			if (r_ptr.flags.has(Monster_Flag.DROP_3.value)) number += Random.rand_range(2, 4);
			if (r_ptr.flags.has(Monster_Flag.DROP_2.value)) number += Random.rand_range(1, 3);
			if (r_ptr.flags.has(Monster_Flag.DROP_1.value)) number++;

			/* Take the best of average of monster level and current depth,
			   and monster level - to reward fighting OOD monsters */
			level = Math.Max((r_ptr.level + Misc.p_ptr.depth) / 2, r_ptr.level);

			/* Specified drops */
			for (drop = r_ptr.drops; drop != null; drop = drop.Next) {
				if (Random.randint0(100) >= drop.percent_chance)
					continue;

				//i_ptr = &object_type_body;
				i_ptr = new Object.Object();
				if (drop.artifact != null) {
					throw new NotImplementedException();
					//object_prep(i_ptr, objkind_get(drop.artifact.tval,
					//    drop.artifact.sval), level, RANDOMISE);
					//i_ptr.artifact = drop.artifact;
					//copy_artifact_data(i_ptr, i_ptr.artifact);
					//i_ptr.artifact.created = 1;
				} else {
					i_ptr.prep(drop.kind, level, aspect.RANDOMISE);
					i_ptr.apply_magic(level, true, good, great);
				}

				i_ptr.origin = (Object.Origin)origin;
				i_ptr.origin_depth = (byte)Misc.p_ptr.depth;
				i_ptr.origin_xtra = m_ptr.r_idx;
				i_ptr.number = (byte)(Random.randint0((int)(drop.max - drop.min)) + drop.min);
				if (m_ptr.carry(i_ptr) != 0)
					any = true;
			}

			/* Make some objects */
			for (j = 0; j < number; j++) {
				//i_ptr = &object_type_body;
				//object_wipe(i_ptr);

				i_ptr = new Object.Object();

				if (gold_ok && (!item_ok || (Random.randint0(100) < 50))) {
				    Object.Object.make_gold(ref i_ptr, level, force_coin);
				} else {
					int q = 0;
				    if (!Object.Object.make_object(Cave.cave, ref i_ptr, level, good, great, ref q)) continue;
				}

				i_ptr.origin = (Origin)origin;
				i_ptr.origin_depth = (byte)Misc.p_ptr.depth;
				i_ptr.origin_xtra = m_ptr.r_idx;
				if (m_ptr.carry(i_ptr) != 0)
				    any = true;
			}

			return any;
		}

		/**
		 * Return the coin type of a monster race, based on the monster being
		 * killed.
		 */
		static int get_coin_type(Monster_Race r_ptr)
		{
			string name = r_ptr.Name;

			if (!r_ptr.flags.has(Monster_Flag.METAL.value)) return (int)SVal.sval_gold.SV_GOLD_ANY;

			/* Look for textual clues */
			if (name.Contains("copper "))	return (int)SVal.sval_gold.SV_COPPER;
			if (name.Contains("silver "))	return (int)SVal.sval_gold.SV_SILVER;
			if (name.Contains("gold "))		return (int)SVal.sval_gold.SV_GOLD;
			if (name.Contains("mithril "))	return (int)SVal.sval_gold.SV_MITHRIL;
			if (name.Contains("adamantite "))	return (int)SVal.sval_gold.SV_ADAMANTITE;

			/* Assume nothing */
			return (int)SVal.sval_gold.SV_GOLD_ANY;
		}

		/*
		 * Get and return the index of a "free" monster.
		 *
		 * This routine should almost never fail, but it *can* happen.
		 */
		static short mon_pop()
		{
			int i;


			/* Normal allocation */
			if (Cave.cave_monster_max(Cave.cave) < Misc.z_info.m_max)
			{
				/* Get the next hole */
				i = Cave.cave_monster_max(Cave.cave);

				/* Expand the array */
				Cave.cave.mon_max++;

				/* Count monsters */
				Cave.cave.mon_cnt++;

				/* Return the index */
				return ((short)i);
			}


			/* Recycle dead monsters */
			for (i = 1; i < Cave.cave_monster_max(Cave.cave); i++)
			{
				Monster m_ptr;

				/* Get the monster */
				m_ptr = Cave.cave_monster(Cave.cave, i);

				/* Skip live monsters */
				if (m_ptr.r_idx != 0) continue;

				/* Count monsters */
				Cave.cave.mon_cnt++;

				/* Use this monster */
				return ((short)i);
			}


			/* Warn the player (except during dungeon creation) */
			if (Player.Player.character_dungeon) Utilities.msg("Too many monsters!");

			/* Try not to crash */
			return (0);
		}

		/*
		 * Attempt to place a monster of the given race at the given location
		 *
		 * Note that certain monsters are now marked as requiring "friends".
		 * These monsters, if successfully placed, and if the "grp" parameter
		 * is true, will be surrounded by a "group" of identical monsters.
		 *
		 * Note that certain monsters are now marked as requiring an "escort",
		 * which is a collection of monsters with similar "race" but lower level.
		 *
		 * Some monsters induce a fake "group" flag on their escorts.
		 *
		 * Note the "bizarre" use of non-recursion to prevent annoying output
		 * when running a code profiler.
		 *
		 * Note the use of the new "monster allocation table" code to restrict
		 * the "get_mon_num()" function to "legal" escort types.
		 */
		public static bool place_new_monster(Cave c, int y, int x, int r_idx, bool slp, bool grp, Object.Origin origin)
		{
			//int i;

			Monster_Race r_ptr = Misc.r_info[r_idx];

			Misc.assert(c != null);

			/* Place one monster, or fail */
			if (!place_new_monster_one(y, x, r_idx, slp, (byte)origin)) return (false);

			/* Require the "group" flag */
			if (!grp) return (true);

			/* Friends for certain monsters */
			if (r_ptr.flags.has(Monster_Flag.FRIEND.value)) {
				throw new NotImplementedException();
				//int total = group_size_2(r_idx);
		
				///* Attempt to place a group */
				//(void)place_new_monster_group(c, y, x, r_idx, slp, total, origin);
			}

			/* Friends for certain monsters */
			if (r_ptr.flags.has(Monster_Flag.FRIENDS.value)) {
				int total = group_size_1(r_idx);
		
				/* Attempt to place a group */
				place_new_monster_group(c, y, x, r_idx, slp, total, (byte)origin);
			}

			/* Escorts for certain monsters */
			if (r_ptr.flags.has(Monster_Flag.ESCORT.value))
			{
				throw new NotImplementedException();
				///* Try to place several "escorts" */
				//for (i = 0; i < 50; i++)
				//{
				//    int nx, ny, z, d = 3;

				//    /* Pick a location */
				//    scatter(&ny, &nx, y, x, d, 0);

				//    /* Require empty grids */
				//    if (!cave_empty_bold(ny, nx)) continue;

				//    /* Set the escort index */
				//    place_monster_idx = r_idx;

				//    /* Set the escort hook */
				//    get_mon_num_hook = place_monster_okay;

				//    /* Prepare allocation table */
				//    get_mon_num_prep();

				//    /* Pick a random race */
				//    z = get_mon_num(r_ptr.level);

				//    /* Remove restriction */
				//    get_mon_num_hook = null;

				//    /* Prepare allocation table */
				//    get_mon_num_prep();

				//    /* Handle failure */
				//    if (!z) break;

				//    /* Place a single escort */
				//    (void)place_new_monster_one(ny, nx, z, slp, origin);

				//    /* Place a "group" of escorts if needed */
				//    if (rf_has(r_info[z].flags, RF_FRIEND)) {
				//        int total = group_size_2(r_idx);
				
				//        /* Attempt to place a group */
				//        (void)place_new_monster_group(c, y, x, r_idx, slp, total, origin);
				//    }
			
				//    if (rf_has(r_info[z].flags, RF_FRIENDS) || rf_has(r_ptr.flags, RF_ESCORTS)) {
				//        int total = group_size_1(r_idx);
				
				//        /* Place a group of monsters */
				//        (void)place_new_monster_group(c, ny, nx, z, slp, total, origin);
				//    }
				//}
			}

			/* Success */
			return (true);
		}

		/*
		 * Attempt to place a "group" of monsters around the given location
		 */
		static bool place_new_monster_group(Cave c, int y, int x, int r_idx, bool slp, int total, byte origin)
		{
			int n, i;

			int hack_n;

			byte[] hack_y = new byte[GROUP_MAX];
			byte[] hack_x = new byte[GROUP_MAX];

			/* Start on the monster */
			hack_n = 1;
			hack_x[0] = (byte)x;
			hack_y[0] = (byte)y;

			/* Puddle monsters, breadth first, up to total */
			for (n = 0; (n < hack_n) && (hack_n < total); n++) {
				/* Grab the location */
				int hx = hack_x[n];
				int hy = hack_y[n];

				/* Check each direction, up to total */
				for (i = 0; (i < 8) && (hack_n < total); i++) {
					int mx = hx + Misc.ddx_ddd[i];
					int my = hy + Misc.ddy_ddd[i];

					/* Walls and Monsters block flow */
					if (!Cave.cave_empty_bold(my, mx)) continue;

					/* Attempt to place another monster */
					if (place_new_monster_one(my, mx, r_idx, slp, origin)) {
						/* Add it to the "hack" set */
						hack_y[hack_n] = (byte)my;
						hack_x[hack_n] = (byte)mx;
						hack_n++;
					}
				}
			}

			/* Success */
			return (true);
		}

		/*
		 * Maximum size of a group of monsters
		 */
		public const int GROUP_MAX = 25;

		/*
		 * Pick a monster group size. Used for monsters with the FRIENDS
		 * flag and monsters with the ESCORT/ESCORTS flags.
		 */
		static int group_size_1(int r_idx)
		{
			Monster_Race r_ptr = Misc.r_info[r_idx];

			int total, extra = 0;

			/* Pick a group size */
			total = Random.randint1(13);

			/* Hard monsters, small groups */
			if (r_ptr.level > Misc.p_ptr.depth)
			{
				extra = r_ptr.level - Misc.p_ptr.depth;
				extra = 0 - Random.randint1(extra);
			}

			/* Easy monsters, large groups */
			else if (r_ptr.level < Misc.p_ptr.depth)
			{
				extra = Misc.p_ptr.depth - r_ptr.level;
				extra = Random.randint1(extra);
			}

			/* Modify the group size */
			total += extra;

			/* Minimum size */
			if (total < 1) total = 1;

			/* Maximum size */
			if (total > GROUP_MAX) total = GROUP_MAX;

			return total;
		}

		/*
		 * Delete/Remove all the monsters when the player leaves the level
		 *
		 * This is an efficient method of simulating multiple calls to the
		 * "delete_monster()" function, with no visual effects.
		 */
		public static void wipe_mon_list(Cave c, Player.Player p)
		{
			int i;

			/* Delete all the monsters */
			for (i = Cave.cave_monster_max(Cave.cave) - 1; i >= 1; i--)
			{
				Monster m_ptr = Cave.cave_monster(Cave.cave, i);

				Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

				/* Skip dead monsters */
				if (m_ptr.r_idx == 0) continue;

				/* Hack -- Reduce the racial counter */
				r_ptr.cur_num--;

				/* Monster is gone */
				c.m_idx[m_ptr.fy][m_ptr.fx] = 0;

				/* Wipe the Monster */
				m_ptr.WIPE();
			}

			/* Reset "cave.mon_max" */
			Cave.cave.mon_max = 1;

			/* Reset "mon_cnt" */
			Cave.cave.mon_cnt = 0;

			/* Hack -- reset "reproducer" count */
			Misc.num_repro = 0;

			/* Hack -- no more target */
			Target.set_monster(0);

			/* Hack -- no more tracking */
			Cave.health_track(p, 0);
		}

		// XXX Nick: I think there is a better place to put this
		public static void player_place(Cave c, Player.Player p, int y, int x)
		{
			Misc.assert(c.m_idx[y][x] == 0);

			/* Save player location */
			p.py = (short)y;
			p.px = (short)x;

			/* Mark cave grid */
			c.m_idx[y][x] = -1;
		}

		/*
		 * Move an object from index i1 to index i2 in the object list
		 */
		public static void compact_monsters_aux(int i1, int i2)
		{
			int y, x;

			Monster m_ptr;

			short this_o_idx, next_o_idx = 0;


			/* Do nothing */
			if (i1 == i2) return;


			/* Old monster */
			m_ptr = Cave.cave_monster(Cave.cave, i1);

			/* Location */
			y = m_ptr.fy;
			x = m_ptr.fx;

			/* Update the cave */
			Cave.cave.m_idx[y][x] = (short)i2;
	
			/* Update midx */
			m_ptr.midx = i2;

			/* Repair objects being carried by monster */
			for (this_o_idx = m_ptr.hold_o_idx; this_o_idx != 0; this_o_idx = next_o_idx)
			{
				Object.Object o_ptr;

				/* Get the object */
				o_ptr = Object.Object.byid(this_o_idx);

				/* Get the next object */
				next_o_idx = o_ptr.next_o_idx;

				/* Reset monster pointer */
				o_ptr.held_m_idx = (short)i2;
			}
	
			/* Move mimicked objects */
			if (m_ptr.mimicked_o_idx > 0) {

				Object.Object o_ptr;

				/* Get the object */
				o_ptr = Object.Object.byid(m_ptr.mimicked_o_idx);

				/* Reset monster pointer */
				o_ptr.mimicking_m_idx = (short)i2;
			}

			/* Hack -- Update the target */
			if (Target.get_monster() == i1) Target.set_monster(i2);

			/* Hack -- Update the health bar */
			if (Misc.p_ptr.health_who == i1) Misc.p_ptr.health_who = (short)i2;

			/* Hack -- move monster */
			Cave.cave_monster_set(Cave.cave, i2, Cave.cave_monster(Cave.cave, i1));

			/* Hack -- wipe hole */
			Cave.cave_monster(Cave.cave, i1).WIPE();
			//(void)WIPE(cave_monster(cave, i1), monster_type);
		}

		/*
		 * Delete a monster by index.
		 *
		 * When a monster is deleted, all of its objects are deleted.
		 */
		public static void delete_monster_idx(int i)
		{
			int x, y;

			Monster m_ptr = Cave.cave_monster(Cave.cave, i);
			Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			short this_o_idx, next_o_idx = 0;

			/* Get location */
			y = m_ptr.fy;
			x = m_ptr.fx;

			/* Hack -- Reduce the racial counter */
			r_ptr.cur_num--;

			/* Hack -- count the number of "reproducers" */
			if (r_ptr.flags.has(Monster_Flag.MULTIPLY.value)) Misc.num_repro--;

			/* Hack -- remove target monster */
			if (Target.get_monster() == i) Target.set_monster(0);

			/* Hack -- remove tracked monster */
			if (Misc.p_ptr.health_who == i) Cave.health_track(Misc.p_ptr, 0);

			/* Monster is gone */
			Cave.cave.m_idx[y][x] = 0;

			/* Delete objects */
			for (this_o_idx = m_ptr.hold_o_idx; this_o_idx != 0; this_o_idx = next_o_idx)
			{
				Object.Object o_ptr;

				/* Get the object */
				o_ptr = Object.Object.byid(this_o_idx);

				/* Get the next object */
				next_o_idx = o_ptr.next_o_idx;

				/* Preserve unseen artifacts (we assume they were created as this
				 * monster's drop) - this will cause unintended behaviour in preserve
				 * off mode if monsters can pick up artifacts */
				if (o_ptr.artifact != null && !o_ptr.was_sensed())
					o_ptr.artifact.created = false;

				/* Clear held_m_idx now to avoid wasting time in delete_object_idx */
				o_ptr.held_m_idx = 0;

				/* Delete the object */
				Object.Object.delete_object_idx(this_o_idx);
			}

			/* Delete mimicked objects */
			if (m_ptr.mimicked_o_idx > 0)
				Object.Object.delete_object_idx(m_ptr.mimicked_o_idx);

			/* Wipe the Monster */
			m_ptr.WIPE();
			//(void)WIPE(m_ptr, monster_type);

			/* Count monsters */
			Cave.cave.mon_cnt--;

			/* Visual update */
			Cave.cave_light_spot(Cave.cave, y, x);
		}

		/*
		 * Compact and Reorder the monster list
		 *
		 * This function can be very dangerous, use with caution!
		 *
		 * When actually "compacting" monsters, we base the saving throw
		 * on a combination of monster level, distance from player, and
		 * current "desperation".
		 *
		 * After "compacting" (if needed), we "reorder" the monsters into a more
		 * compact order, and we reset the allocation info, and the "live" array.
		 */
		public static void compact_monsters(int size)
		{
			int i, num, cnt;

			int cur_lev, cur_dis, chance;


			/* Message (only if compacting) */
			if (size != 0) Utilities.msg("Compacting monsters...");


			/* Compact at least 'size' objects */
			for (num = 0, cnt = 1; num < size; cnt++)
			{
				/* Get more vicious each iteration */
				cur_lev = 5 * cnt;

				/* Get closer each iteration */
				cur_dis = 5 * (20 - cnt);

				/* Check all the monsters */
				for (i = 1; i < Cave.cave_monster_max(Cave.cave); i++)
				{
					Monster m_ptr = Cave.cave_monster(Cave.cave, i);

					Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

					/* Paranoia -- skip "dead" monsters */
					if (m_ptr.r_idx == 0) continue;

					/* Hack -- High level monsters start out "immune" */
					if (r_ptr.level > cur_lev) continue;

					/* Ignore nearby monsters */
					if ((cur_dis > 0) && (m_ptr.cdis < cur_dis)) continue;

					/* Saving throw chance */
					chance = 90;

					/* Only compact "Quest" Monsters in emergencies */
					if (r_ptr.flags.has(Monster_Flag.QUESTOR.value) && (cnt < 1000)) chance = 100;

					/* Try not to compact Unique Monsters */
					if (r_ptr.flags.has(Monster_Flag.UNIQUE.value)) chance = 99;

					/* All monsters get a saving throw */
					if (Random.randint0(100) < chance) continue;

					/* Delete the monster */
					delete_monster_idx(i);

					/* Count the monster */
					num++;
				}
			}


			/* Excise dead monsters (backwards!) */
			for (i = Cave.cave_monster_max(Cave.cave) - 1; i >= 1; i--)
			{
				/* Get the i'th monster */
				Monster m_ptr = Cave.cave_monster(Cave.cave, i);

				/* Skip real monsters */
				if (m_ptr.r_idx != 0) continue;

				/* Move last monster into open hole */
				compact_monsters_aux(Cave.cave_monster_max(Cave.cave) - 1, i);

				/* Compress "cave.mon_max" */
				Cave.cave.mon_max--;
			}
		}

		/**
		 * Decrease a monster's hit points and handle monster death.
		 *
		 * We return true if the monster has been killed (and deleted).
		 *
		 * We announce monster death (using an optional "death message"
		 * if given, and a otherwise a generic killed/destroyed message).
		 *
		 * Only "physical attacks" can induce the "You have slain" message.
		 * Missile and Spell attacks will induce the "dies" message, or
		 * various "specialized" messages.  Note that "You have destroyed"
		 * and "is destroyed" are synonyms for "You have slain" and "dies".
		 *
		 * Invisible monsters induce a special "You have killed it." message.
		 *
		 * Hack -- we "delay" fear messages by passing around a "fear" flag.
		 *
		 * Consider decreasing monster experience over time, say, by using
		 * "(m_exp * m_lev * (m_lev)) / (p_lev * (m_lev + n_killed))" instead
		 * of simply "(m_exp * m_lev) / (p_lev)", to make the first monster
		 * worth more than subsequent monsters.  This would also need to
		 * induce changes in the monster recall code.  XXX XXX XXX
		 **/
		public static bool mon_take_hit(int m_idx, int dam, ref bool fear, string note)
		{
			Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);

			Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			Monster_Lore l_ptr = Misc.l_list[m_ptr.r_idx];

			int div, new_exp, new_exp_frac;


			/* Redraw (later) if needed */
			if (Misc.p_ptr.health_who == m_idx) Misc.p_ptr.redraw |= (Misc.PR_HEALTH);

			/* Wake it up */
			Monster.mon_clear_timed(m_idx, (int)Misc.MON_TMD.SLEEP, Misc.MON_TMD_FLG_NOMESSAGE, false);

			/* Become aware of its presence */
			if (m_ptr.unaware)
			    Monster.become_aware(m_idx);

			/* Hurt it */
			m_ptr.hp -= (short)dam;

			/* It is dead now */
			if (m_ptr.hp < 0)
			{
			    //char m_name[80];
			    //char buf[80];
				string m_name;
				string buf;

			    /* Assume normal death sound */
			    Message_Type soundfx = Message_Type.MSG_KILL;

			    /* Play a special sound if the monster was unique */
			    if (r_ptr.flags.has(Monster_Flag.UNIQUE.value))
			    {
			        if (r_ptr.Base == Monster.lookup_monster_base("Morgoth"))
			            soundfx = Message_Type.MSG_KILL_KING;
			        else
			            soundfx = Message_Type.MSG_KILL_UNIQUE;
			    }

			    /* Extract monster name */
			    m_name = m_ptr.monster_desc(0);

			    /* Death by Missile/Spell attack */
			    if (note != null)
			    {
			        /* Hack -- allow message suppression */
			        if (note.Length <= 1)
			        {
			            /* Be silent */
			        }

			        else Utilities.msgt(soundfx, "%^s%s", m_name, note);
			    }

			    /* Death by physical attack -- invisible monster */
			    else if (!m_ptr.ml)
			    {
			        Utilities.msgt(soundfx, "You have killed {0}.", m_name);
			    }

			    /* Death by Physical attack -- non-living monster */
			    else if (r_ptr.monster_is_unusual())
			    {
			        Utilities.msgt(soundfx, "You have destroyed {0}.", m_name);
			    }

			    /* Death by Physical attack -- living monster */
			    else
			    {
			        Utilities.msgt(soundfx, "You have slain {0}.", m_name);
			    }

			    /* Player level */
			    div = Misc.p_ptr.lev;

			    /* Give some experience for the kill */
			    new_exp = (int)(((long)r_ptr.mexp * r_ptr.level) / div);

			    /* Handle fractional experience */
			    new_exp_frac = (int)(((((long)r_ptr.mexp * r_ptr.level) % div) * 0x10000L / div) + Misc.p_ptr.exp_frac);

			    /* Keep track of experience */
			    if (new_exp_frac >= 0x10000L)
			    {
			        new_exp++;
			        Misc.p_ptr.exp_frac = (ushort)(new_exp_frac - 0x10000L);
			    }
			    else
			    {
			        Misc.p_ptr.exp_frac = (ushort)new_exp_frac;
			    }

			    /* When the player kills a Unique, it stays dead */
			    if (r_ptr.flags.has(Monster_Flag.UNIQUE.value))
			    {
			        //char unique_name[80];
					string unique_name;
			        r_ptr.max_num = 0;

			        /* This gets the correct name if we slay an invisible unique and don't have See Invisible. */
			        unique_name = m_ptr.monster_desc(Monster.Desc.SHOW | Monster.Desc.IND2);

			        /* Log the slaying of a unique */
					buf = "Killed " + unique_name;
			        //strnfmt(buf, sizeof(buf), "Killed %s", unique_name);
			        History.add(buf, History.SLAY_UNIQUE, null);
			    }

			    /* Gain experience */
			    Misc.p_ptr.exp_gain(new_exp);

			    /* Generate treasure */
			    monster_death(m_idx, false);

			    /* Recall even invisible uniques or winners */
			    if (m_ptr.ml || r_ptr.flags.has(Monster_Flag.UNIQUE.value))
			    {
			        /* Count kills this life */
			        if (l_ptr.pkills < short.MaxValue) l_ptr.pkills++;

			        /* Count kills in all lives */
			        if (l_ptr.tkills < short.MaxValue) l_ptr.tkills++;

			        /* Hack -- Auto-recall */
			        Cave.monster_race_track(m_ptr.r_idx);
			    }

			    /* Delete the monster */
			    delete_monster_idx(m_idx);

			    /* Not afraid */
			    fear = false;

			    /* Monster is dead */
			    return (true);
			}


			/* Mega-Hack -- Pain cancels fear */
			if (!fear && m_ptr.m_timed[(int)Misc.MON_TMD.FEAR] != 0 && (dam > 0))
			{
			    int tmp = Random.randint1(dam);

			    /* Cure a little fear */
			    if (tmp < m_ptr.m_timed[(int)Misc.MON_TMD.FEAR])
			        /* Reduce fear */
			        Monster.mon_dec_timed(m_idx, (int)Misc.MON_TMD.FEAR, tmp, Misc.MON_TMD_FLG_NOMESSAGE, false);

			    /* Cure all the fear */
			    else
			    {
			        /* Cure fear */
			        Monster.mon_clear_timed(m_idx, (int)Misc.MON_TMD.FEAR, Misc.MON_TMD_FLG_NOMESSAGE, false);

			        /* No more fear */
			        fear = false;
			    }
			}

			/* Sometimes a monster gets scared by damage */
			if (m_ptr.m_timed[(int)Misc.MON_TMD.FEAR] == 0 && !r_ptr.flags.has(Monster_Flag.NO_FEAR.value) && dam > 0)
			{
			    int percentage;

			    /* Percentage of fully healthy */
			    percentage = (int)((100L * m_ptr.hp) / m_ptr.maxhp);

			    /*
			     * Run (sometimes) if at 10% or less of max hit points,
			     * or (usually) when hit for half its current hit points
			     */
			    if ((Random.randint1(10) >= percentage) ||
			        ((dam >= m_ptr.hp) && (Random.randint0(100) < 80)))
			    {
			        int timer = Random.randint1(10) + (((dam >= m_ptr.hp) && (percentage > 7)) ?
			                   20 : ((11 - percentage) * 5));

			        /* Hack -- note fear */
			        fear = true;

			        Monster.mon_inc_timed(m_idx, (int)Misc.MON_TMD.FEAR, timer, Misc.MON_TMD_FLG_NOMESSAGE | Misc.MON_TMD_FLG_NOFAIL, false);
			    }
			}


			/* Not dead yet */
			return (false);
		}

		/**
		 * Handle the "death" of a monster.
		 *
		 * Disperse treasures carried by the monster centered at the monster location.
		 * Note that objects dropped may disappear in crowded rooms.
		 *
		 * Check for "Quest" completion when a quest monster is killed.
		 *
		 * Note that only the player can induce "monster_death()" on Uniques.
		 * Thus (for now) all Quest monsters should be Uniques.
		 */
		public static void monster_death(int m_idx, bool stats)
		{
			int i, y, x;
			int dump_item = 0;
			int dump_gold = 0;
			int total = 0;
			short this_o_idx, next_o_idx = 0;

			Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);
			Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			bool visible = (m_ptr.ml || r_ptr.flags.has(Monster_Flag.UNIQUE.value));

			Object.Object i_ptr;
			//object_type object_type_body;

			/* Get the location */
			y = m_ptr.fy;
			x = m_ptr.fx;
	
			/* Delete any mimicked objects */
			if (m_ptr.mimicked_o_idx > 0)
			    Object.Object.delete_object_idx(m_ptr.mimicked_o_idx);

			/* Drop objects being carried */
			for (this_o_idx = m_ptr.hold_o_idx; this_o_idx != 0; this_o_idx = next_o_idx) {
			    Object.Object o_ptr;

			    /* Get the object */
			    o_ptr = Object.Object.byid(this_o_idx);

			    /* Line up the next object */
			    next_o_idx = o_ptr.next_o_idx;

			    /* Paranoia */
			    o_ptr.held_m_idx = 0;

			    /* Get local object, copy it and delete the original */
			    //i_ptr = &object_type_body;
			    //object_copy(i_ptr, o_ptr);
				i_ptr = o_ptr.copy();
			    Object.Object.delete_object_idx(this_o_idx);

			    /* Count it and drop it - refactor once origin is a bitflag */
			    if (!stats) {
			        if ((i_ptr.tval == TVal.TV_GOLD) && (i_ptr.origin != Origin.STOLEN))
			            dump_gold++;
			        else if ((i_ptr.tval != TVal.TV_GOLD) && ((i_ptr.origin == Origin.DROP)
			                || (i_ptr.origin == Origin.DROP_PIT)
			                || (i_ptr.origin == Origin.DROP_VAULT)
			                || (i_ptr.origin == Origin.DROP_SUMMON)
			                || (i_ptr.origin == Origin.DROP_SPECIAL)
			                || (i_ptr.origin == Origin.DROP_BREED)
			                || (i_ptr.origin == Origin.DROP_POLY)
			                || (i_ptr.origin == Origin.DROP_WIZARD)))
			            dump_item++;
			    }

			    /* Change origin if monster is invisible, unless we're in stats mode */
			    if (!visible && !stats)
			        i_ptr.origin = Origin.DROP_UNKNOWN;

			    Object.Object.drop_near(Cave.cave, i_ptr, 0, y, x, true);
			}

			/* Forget objects */
			m_ptr.hold_o_idx = 0;

			/* Take note of any dropped treasure */
			if (visible && (dump_item != 0 || dump_gold != 0))
			    Monster_Lore.lore_treasure(m_idx, dump_item, dump_gold);

			/* Update monster list window */
			Misc.p_ptr.redraw |= Misc.PR_MONLIST;

			/* Only process "Quest Monsters" */
			if (!r_ptr.flags.has(Monster_Flag.QUESTOR.value)) return;

			/* Mark quests as complete */
			for (i = 0; i < Misc.MAX_Q_IDX; i++)	{
			    /* Note completed quests */
			    if (Misc.q_list[i].level == r_ptr.level) Misc.q_list[i].level = 0;

			    /* Count incomplete quests */
			    if (Misc.q_list[i].level != 0) total++;
			}

			/* Build magical stairs */
			build_quest_stairs(y, x);

			/* Nothing left, game over... */
			if (total == 0) {
			    Misc.p_ptr.total_winner = 1;//true
			    Misc.p_ptr.redraw |= (Misc.PR_TITLE);
			    Utilities.msg("*** CONGRATULATIONS ***");
			    Utilities.msg("You have won the game!");
			    Utilities.msg("You may retire (commit suicide) when you are ready.");
			}
		}

		/*
		 * Create magical stairs after finishing a quest monster.
		 */
		static void build_quest_stairs(int y, int x)
		{
			throw new NotImplementedException();
			//int ny, nx;


			///* Stagger around */
			//while (!cave_valid_bold(y, x))
			//{
			//    int d = 1;

			//    /* Pick a location */
			//    scatter(&ny, &nx, y, x, d, 0);

			//    /* Stagger */
			//    y = ny; x = nx;
			//}

			///* Destroy any objects */
			//delete_object(y, x);

			///* Explain the staircase */
			//msg("A magical staircase appears...");

			///* Create stairs down */
			//cave_set_feat(cave, y, x, FEAT_MORE);

			///* Update the visuals */
			//p_ptr.update |= (PU_UPDATE_VIEW | PU_MONSTERS);

			///* Fully update the flow */
			//p_ptr.update |= (PU_FORGET_FLOW | PU_UPDATE_FLOW);
		}
	}
}
