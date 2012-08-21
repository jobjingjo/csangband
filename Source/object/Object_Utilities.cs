using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Object {
	partial class Object {
		/*
		 * Delete all the items when player leaves the level
		 *
		 * Note -- we do NOT visually reflect these (irrelevant) changes
		 *
		 * Hack -- we clear the "cave.o_idx[y][x]" field for every grid,
		 * and the "m_ptr.next_o_idx" field for every monster, since
		 * we know we are clearing every object.  Technically, we only
		 * clear those fields for grids/monsters containing objects,
		 * and we clear it once for every such object.
		 */
		public static void wipe_o_list(Cave c)
		{
			int i;

			/* Delete the existing objects */
			for (i = 1; i < Misc.o_max; i++)
			{
				Object o_ptr = Object.byid((short)i);
				if (o_ptr.kind == null) continue;

				/* Preserve artifacts or mark them as lost in the history */
				if (o_ptr.artifact != null) {
					throw new NotImplementedException();
					///* Preserve if dungeon creation failed, or preserve mode, or items
					// * carried by monsters, and only artifacts not seen */
					//if ((!character_dungeon || !OPT(birth_no_preserve) ||
					//        o_ptr.held_m_idx) && !object_was_sensed(o_ptr))
					//    o_ptr.artifact.created = false;
					//else
					//    history_lose_artifact(o_ptr.artifact);
				}

				/* Monster */
				if (o_ptr.held_m_idx != 0)
				{
					Monster.Monster m_ptr;

					/* Monster */
					m_ptr = Cave.cave_monster(Cave.cave, o_ptr.held_m_idx);

					/* Hack -- see above */
					m_ptr.hold_o_idx = 0;
				}

				/* Dungeon */
				else
				{
					/* Get the location */
					int y = o_ptr.iy;
					int x = o_ptr.ix;

					/* Hack -- see above */
					c.o_idx[y][x] = 0;
				}

				/* Wipe the object */
				o_ptr.WIPE();
			}

			/* Reset "o_max" */
			Misc.o_max = 1;

			/* Reset "o_cnt" */
			Misc.o_cnt = 0;
		}

		/*
		 * Get and return the index of a "free" object.
		 *
		 * This routine should almost never fail, but in case it does,
		 * we must be sure to handle "failure" of this routine.
		 */
		public static short o_pop()
		{
			int i;


			/* Initial allocation */
			if (Misc.o_max < Misc.z_info.o_max)
			{
				/* Get next space */
				i = Misc.o_max;

				/* Expand object array */
				Misc.o_max++;

				/* Count objects */
				Misc.o_cnt++;

				/* Use this object */
				return ((short)i);
			}


			/* Recycle dead objects */
			for (i = 1; i < Misc.o_max; i++)
			{
				Object o_ptr = Object.byid((short)i);
				if (o_ptr.kind != null) continue;

				/* Count objects */
				Misc.o_cnt++;

				/* Use this object */
				return ((short)i);
			}


			/* Warn the player (except during dungeon creation) */
			if (Player.Player.character_dungeon) Utilities.msg("Too many objects!");

			/* Oops */
			return (0);
		}

		/*
		 * Get the first object at a dungeon location
		 * or null if there isn't one.
		 */
		public static Object get_first_object(int y, int x)
		{
			short o_idx = Cave.cave.o_idx[y][x];

			if (o_idx != 0)
				return Object.byid(o_idx);

			/* No object */
			return (null);
		}


		/*
		 * Get the next object in a stack or null if there isn't one.
		 */
		public static Object get_next_object(Object o_ptr)
		{
			if (o_ptr.next_o_idx != 0)
				return Object.byid(o_ptr.next_o_idx);

			/* No more objects */
			return null;
		}

		/*
		 * Drop (some of) a non-cursed inventory/equipment item
		 *
		 * The object will be dropped "near" the current location
		 */
		public static void inven_drop(int item, int amt)
		{
			throw new NotImplementedException();
			//int py = p_ptr.py;
			//int px = p_ptr.px;

			//object_type *o_ptr;

			//object_type *i_ptr;
			//object_type object_type_body;

			//char o_name[80];


			///* Get the original object */
			//o_ptr = &p_ptr.inventory[item];

			///* Error check */
			//if (amt <= 0) return;

			///* Not too many */
			//if (amt > o_ptr.number) amt = o_ptr.number;


			///* Take off equipment */
			//if (item >= INVEN_WIELD)
			//{
			//    /* Take off first */
			//    item = inven_takeoff(item, amt);

			//    /* Get the original object */
			//    o_ptr = &p_ptr.inventory[item];
			//}

			///* Stop tracking items no longer in the inventory */
			//if (tracked_object_is(item) && amt == o_ptr.number)
			//{
			//    track_object(NO_OBJECT);
			//}

			//i_ptr = &object_type_body;

			//object_copy(i_ptr, o_ptr);
			//object_split(i_ptr, o_ptr, amt);

			///* Describe local object */
			//object_desc(o_name, sizeof(o_name), i_ptr, ODESC_PREFIX | ODESC_FULL);

			///* Message */
			//msg("You drop %s (%c).", o_name, index_to_label(item));

			///* Drop it near the player */
			//drop_near(cave, i_ptr, 0, py, px, false);

			///* Modify, Describe, Optimize */
			//inven_item_increase(item, -amt);
			//inven_item_describe(item);
			//inven_item_optimize(item);
		}

		/*
		 * Combine items in the pack
		 * Also "pick up" any gold in the inventory by accident
		 *
		 * Note special handling of the "overflow" slot
		 */
		public static void combine_pack()
		{
			int i, j, k;

			Object o_ptr;
			Object j_ptr;

			bool flag = false;


			/* Combine the pack (backwards) */
			for (i = Misc.INVEN_PACK; i > 0; i--)
			{
			    bool slide = false;

			    /* Get the item */
			    o_ptr = Misc.p_ptr.inventory[i];

			    /* Skip empty items */
			    if (o_ptr.kind == null) continue;

			    /* Absorb gold */
			    if (o_ptr.tval == TVal.TV_GOLD)
			    {
			        /* Count the gold */
			        slide = true;
			        Misc.p_ptr.au += o_ptr.pval[Misc.DEFAULT_PVAL];
			    }

			    /* Scan the items above that item */
			    else for (j = 0; j < i; j++)
			    {
			        /* Get the item */
			        j_ptr = Misc.p_ptr.inventory[j];

			        /* Skip empty items */
			        if (j_ptr.kind == null) continue;

			        /* Can we drop "o_ptr" onto "j_ptr"? */
			        if (j_ptr.similar(o_ptr, object_stack_t.OSTACK_PACK))
			        {
			            /* Take note */
			            flag = slide = true;

			            /* Add together the item counts */
			            j_ptr.absorb(o_ptr);

			            break;
			        }
			    }


			    /* Compact the inventory */
			    if (slide)
			    {
			        /* One object is gone */
			        Misc.p_ptr.inven_cnt--;

			        /* Slide everything down */
			        for (k = i; k < Misc.INVEN_PACK; k++)
			        {
			            /* Hack -- slide object */
						Misc.p_ptr.inventory[k] = Misc.p_ptr.inventory[k + 1];

						throw new NotImplementedException();
						///* Update object_idx if necessary */
						//if (tracked_object_is(k+1))
						//{
						//    track_object(k);
						//}
			        }

			        /* Hack -- wipe hole */
					Misc.p_ptr.inventory[k] = new Object();
			        //object_wipe(&p_ptr.inventory[k]);

			        /* Redraw stuff */
			        Misc.p_ptr.redraw |= (Misc.PR_INVEN);
			    }
			}

			/* Message */
			if (flag)
			{
			    Utilities.msg("You combine some items in your pack.");

			    /* Stop "repeat last command" from working. */
			    Game_Command.disable_repeat();
			}
		}


		/*
		 * Reorder items in the pack
		 *
		 * Note special handling of the "overflow" slot
		 */
		public static void reorder_pack()
		{
			int i, j, k;

			int o_value;
			int j_value;

			Object o_ptr;
			Object j_ptr;

			Object i_ptr;
			//object_type object_type_body;

			bool flag = false;


			/* Re-order the pack (forwards) */
			for (i = 0; i < Misc.INVEN_PACK; i++)
			{
			    /* Get the item */
			    o_ptr = Misc.p_ptr.inventory[i];

			    /* Skip empty slots */
			    if (o_ptr.kind == null) continue;

			    /* Get the "value" of the item */
			    o_value = o_ptr.kind.cost;

			    /* Scan every occupied slot */
			    for (j = 0; j < Misc.INVEN_PACK; j++)
			    {
			        /* Get the item already there */
			        j_ptr = Misc.p_ptr.inventory[j];

			        /* Use empty slots */
			        if (j_ptr.kind == null) break;

			        /* Hack -- readable books always come first */
			        if ((o_ptr.tval == Misc.p_ptr.Class.spell_book) &&
			            (j_ptr.tval != Misc.p_ptr.Class.spell_book)) break;
			        if ((j_ptr.tval == Misc.p_ptr.Class.spell_book) &&
			            (o_ptr.tval != Misc.p_ptr.Class.spell_book)) continue;

			        /* Objects sort by decreasing type */
			        if (o_ptr.tval > j_ptr.tval) break;
			        if (o_ptr.tval < j_ptr.tval) continue;

			        /* Non-aware (flavored) items always come last */
			        if (!o_ptr.flavor_is_aware()) continue;
			        if (!j_ptr.flavor_is_aware()) break;

			        /* Objects sort by increasing sval */
			        if (o_ptr.sval < j_ptr.sval) break;
			        if (o_ptr.sval > j_ptr.sval) continue;

			        /* Unidentified objects always come last */
			        if (!o_ptr.is_known()) continue;
			        if (!j_ptr.is_known()) break;

			        /* Lights sort by decreasing fuel */
			        if (o_ptr.tval == TVal.TV_LIGHT)
			        {
			            if (o_ptr.pval[Misc.DEFAULT_PVAL] > j_ptr.pval[Misc.DEFAULT_PVAL]) break;
			            if (o_ptr.pval[Misc.DEFAULT_PVAL] < j_ptr.pval[Misc.DEFAULT_PVAL]) continue;
			        }

			        /* Determine the "value" of the pack item */
			        j_value = j_ptr.kind.cost;

			        /* Objects sort by decreasing value */
			        if (o_value > j_value) break;
			        if (o_value < j_value) continue;
			    }

			    /* Never move down */
			    if (j >= i) continue;

			    /* Take note */
			    flag = true;

			    /* Get local object */
			    //i_ptr = &object_type_body;

			    /* Save a copy of the moving item */
			    //object_copy(i_ptr, &p_ptr.inventory[i]);
				i_ptr = Misc.p_ptr.inventory[i];

			    /* Slide the objects */
			    for (k = i; k > j; k--)
			    {
			        /* Slide the item */
			        //object_copy(&p_ptr.inventory[k], &p_ptr.inventory[k-1]);
					Misc.p_ptr.inventory[k] = Misc.p_ptr.inventory[k - 1];
					throw new NotImplementedException();
					///* Update object_idx if necessary */
					//if (tracked_object_is(k-1))
					//{
					//    track_object(k);
					//}
			    }

			    /* Insert the moving item */
			    Misc.p_ptr.inventory[j] = i_ptr;

				throw new NotImplementedException();
				///* Update object_idx if necessary */
				//if (tracked_object_is(i))
				//{
				//    track_object(j);
				//}

				///* Redraw stuff */
				//Misc.p_ptr.redraw |= (Misc.PR_INVEN);
			}

			if (flag) 
			{
			    Utilities.msg("You reorder some items in your pack.");

			    /* Stop "repeat last command" from working. */
			    Game_Command.disable_repeat();
			}
		}

		/**
		 * Compare ammunition from slots (0-9); used for sorting.
		 *
		 * \returns -1 if slot1 should come first, 1 if slot2 should come first, or 0.
		 */
		static int compare_ammo(int slot1, int slot2)
		{
			/* Right now there is no sorting criteria */
			return 0;
		}

		/*
		 * Returns whether the pack is holding the more than the maximum number of
		 * items. The max size is INVEN_MAX_PACK, which is a macro since quiver size
		 * affects slots available. If this is true, calling pack_overflow() will
		 * trigger a pack overflow.
		 */
		static bool pack_is_overfull()
		{
			return Misc.p_ptr.inventory[Misc.INVEN_MAX_PACK].kind != null ? true : false;
		}

		/*
		 * Overflow an item from the pack, if it is overfull.
		 */
		public static void pack_overflow()
		{
			int item = Misc.INVEN_MAX_PACK;
			string o_name;//[80];
			Object o_ptr;

			if (!pack_is_overfull()) return;

			throw new NotImplementedException();
			///* Get the slot to be dropped */
			//o_ptr = &p_ptr.inventory[item];

			///* Disturbing */
			//disturb(p_ptr, 0, 0);

			///* Warning */
			//msg("Your pack overflows!");

			///* Describe */
			//object_desc(o_name, sizeof(o_name), o_ptr, ODESC_PREFIX | ODESC_FULL);

			///* Message */
			//msg("You drop %s (%c).", o_name, index_to_label(item));

			///* Drop it (carefully) near the player */
			//drop_near(cave, o_ptr, 0, p_ptr.py, p_ptr.px, false);

			///* Modify, Describe, Optimize */
			//inven_item_increase(item, -255);
			//inven_item_describe(item);
			//inven_item_optimize(item);

			///* Notice stuff (if needed) */
			//if (p_ptr.notice) notice_stuff(p_ptr);

			///* Update stuff (if needed) */
			//if (p_ptr.update) update_stuff(p_ptr);

			///* Redraw stuff (if needed) */
			//if (p_ptr.redraw) redraw_stuff(p_ptr);
		}

		/* Basic tval testers */
		public static bool obj_is_staff(Object o_ptr)  { return o_ptr.tval == TVal.TV_STAFF; }
		public static bool obj_is_wand(Object o_ptr)   { return o_ptr.tval == TVal.TV_WAND; }
		public static bool obj_is_rod(Object o_ptr)    { return o_ptr.tval == TVal.TV_ROD; }
		public static bool obj_is_potion(Object o_ptr) { return o_ptr.tval == TVal.TV_POTION; }
		public static bool obj_is_scroll(Object o_ptr) { return o_ptr.tval == TVal.TV_SCROLL; }
		public static bool obj_is_food(Object o_ptr)   { return o_ptr.tval == TVal.TV_FOOD; }
		public static bool obj_is_light(Object o_ptr)   { return o_ptr.tval == TVal.TV_LIGHT; }
		public static bool obj_is_ring(Object o_ptr)   { return o_ptr.tval == TVal.TV_RING; }


		/* Determine if an object has charges */
		public static bool obj_has_charges(Object o_ptr)
		{
			if (o_ptr.tval != TVal.TV_WAND && o_ptr.tval != TVal.TV_STAFF) return false;

			if (o_ptr.pval[Misc.DEFAULT_PVAL] <= 0) return false;

			return true;
		}

		/* Determine if an object is zappable */
		public static bool obj_can_zap(Object o_ptr)
		{
			throw new NotImplementedException();
			///* Any rods not charging? */
			//if (o_ptr.tval == TVal.TV_ROD && number_charging(o_ptr) < o_ptr.number)
			//    return true;

			//return false;
		}

		/* Determine if an object is activatable */
		public static bool obj_is_activatable(Object o_ptr)
		{
			throw new NotImplementedException();
			//return object_effect(o_ptr) ? true : false;
		}

		/* Determine if an object can be activated now */
		public static bool obj_can_activate(Object o_ptr)
		{
			throw new NotImplementedException();
			//if (obj_is_activatable(o_ptr))
			//{
			//    /* Check the recharge */
			//    if (!o_ptr.timeout) return true;
			//}

			//return false;
		}

		public static bool obj_can_refill(Object o_ptr)
		{
			throw new NotImplementedException();
			//bitflag f[OF_SIZE];
			//Object j_ptr = &p_ptr.inventory[INVEN_LIGHT];

			///* Get flags */
			//object_flags(o_ptr, f);

			//if (j_ptr.sval == SV_LIGHT_LANTERN)
			//{
			//    /* Flasks of oil are okay */
			//    if (o_ptr.tval == TV_FLASK) return (true);
			//}

			///* Non-empty, non-everburning sources are okay */
			//if ((o_ptr.tval == TV_LIGHT) &&
			//    (o_ptr.sval == j_ptr.sval) &&
			//    (o_ptr.timeout > 0) &&
			//    !of_has(f, OF_NO_FUEL))
			//{
			//    return (true);
			//}

			///* Assume not okay */
			//return (false);
		}


		public static bool obj_can_browse(Object o_ptr)
		{
			return o_ptr.tval == Misc.p_ptr.Class.spell_book;
		}

		public static bool obj_can_cast_from(Object o_ptr)
		{
			throw new NotImplementedException();
			//return obj_can_browse(o_ptr) &&
			//        spell_book_count_spells(o_ptr, spell_okay_to_cast) > 0;
		}

		public static bool obj_can_study(Object o_ptr)
		{
			throw new NotImplementedException();
			//return obj_can_browse(o_ptr) &&
			//        spell_book_count_spells(o_ptr, spell_okay_to_study) > 0;
		}


		/* Can only take off non-cursed items */
		public static bool obj_can_takeoff(Object o_ptr)
		{
			throw new NotImplementedException();
			//return !cursed_p((bitflag *)o_ptr.flags);
		}

		/* Can only put on wieldable items */
		public static bool obj_can_wear(Object o_ptr)
		{
			throw new NotImplementedException();
			//return (wield_slot(o_ptr) >= INVEN_WIELD);
		}

		/* Can only fire an item with the right tval */
		public static bool obj_can_fire(Object o_ptr)
		{
			return o_ptr.tval == Misc.p_ptr.state.ammo_tval;
		}

		/* Can has inscrip pls */
		public static bool obj_has_inscrip(Object o_ptr)
		{
			throw new NotImplementedException();
			//return (o_ptr.note ? true : false);
		}

		public int number_charging()
		{
			int charge_time, num_charging;
			random_value timeout;

			/* Artifacts have a special timeout */	
			if (artifact != null)
			    timeout = artifact.time;
			else
			    timeout = kind.time;

			if(timeout == null) {
				timeout = new random_value();
			}

			charge_time = Random.randcalc(timeout, 0, aspect.AVERAGE);

			/* Item has no timeout */
			if (charge_time <= 0) return 0;

			/* No items are charging */
			if (this.timeout <= 0) return 0;

			/* Calculate number charging based on timeout */
			num_charging = (this.timeout + charge_time - 1) / charge_time;

			/* Number charging cannot exceed stack size */
			if (num_charging > number) num_charging = number;

			return num_charging;
		}


		public bool recharge_timeout()
		{
			throw new NotImplementedException();
			//int charging_before, charging_after;

			///* Find the number of charging items */
			//charging_before = number_charging(o_ptr);

			///* Nothing to charge */	
			//if (charging_before == 0)
			//    return false;

			///* Decrease the timeout */
			//o_ptr.timeout -= MIN(charging_before, o_ptr.timeout);

			///* Find the new number of charging items */
			//charging_after = number_charging(o_ptr);

			///* Return true if at least 1 item obtained a charge */
			//if (charging_after < charging_before)
			//    return true;
			//else
			//    return false;
		}

		/*
		 * Get the indexes of objects at a given floor location. -TNB-
		 *
		 * Return the number of object indexes acquired.
		 *
		 * Valid flags are any combination of the bits:
		 *   0x01 -- Verify item tester
		 *   0x02 -- Marked/visible items only
		 *   0x04 -- Only the top item
		 */
		public static int scan_floor(int[] items, int max_size, int y, int x, int mode)
		{
			int this_o_idx, next_o_idx;

			int num = 0;
	
			/* Sanity */
			if (!Cave.in_bounds(y, x)) return 0;

			/* Scan all objects in the grid */
			for (this_o_idx = Cave.cave.o_idx[y][x]; this_o_idx != 0; this_o_idx = next_o_idx)
			{
			    Object o_ptr;

			    /* XXX Hack -- Enforce limit */
			    if (num >= max_size) break;


			    /* Get the object */
			    o_ptr = Object.byid((short)this_o_idx);

			    /* Get the next object */
			    next_o_idx = o_ptr.next_o_idx;

			    /* Item tester */
			    if ((mode & 0x01) != 0 && !o_ptr.item_tester_okay()) continue;

			    /* Marked */
			    if ((mode & 0x02) != 0 && (o_ptr.marked == 0 || Squelch.item_ok(o_ptr)))
			        continue;

			    /* Accept this item */
			    items[num++] = this_o_idx;

			    /* Only one */
			    if ((mode & 0x04) != 0) break;
			}

			return num;
		}

		/*
		 * Check an item against the item tester info
		 */
		public bool item_tester_okay()
		{
			throw new NotImplementedException();
			///* Hack -- allow listing empty slots */
			//if (item_tester_full) return (true);

			///* Require an item */
			//if (!o_ptr.kind) return (false);

			///* Hack -- ignore "gold" */
			//if (o_ptr.tval == TV_GOLD) return (false);

			///* Check the tval */
			//if (item_tester_tval)
			//{
			//    if (item_tester_tval != o_ptr.tval) return (false);
			//}

			///* Check the hook */
			//if (item_tester_hook)
			//{
			//    if (!(*item_tester_hook)(o_ptr)) return (false);
			//}

			///* Assume okay */
			//return (true);
		}

		/*
		 * Let an object fall to the ground at or near a location.
		 *
		 * The initial location is assumed to be "in_bounds_fully()".
		 *
		 * This function takes a parameter "chance".  This is the percentage
		 * chance that the item will "disappear" instead of drop.  If the object
		 * has been thrown, then this is the chance of disappearance on contact.
		 *
		 * This function will produce a description of a drop event under the player
		 * when "verbose" is true.
		 *
		 * We check several locations to see if we can find a location at which
		 * the object can combine, stack, or be placed.  Artifacts will try very
		 * hard to be placed, including "teleporting" to a useful grid if needed.
		 */
		public static void drop_near(Cave c, Object j_ptr, int chance, int y, int x, bool verbose)
		{
			throw new NotImplementedException();
			//int i, k, n, d, s;

			//int bs, bn;
			//int by, bx;
			//int dy, dx;
			//int ty, tx;

			//object_type *o_ptr;

			//char o_name[80];

			//bool flag = false;

			//bool plural = false;


			///* Extract plural */
			//if (j_ptr.number != 1) plural = true;

			///* Describe object */
			//object_desc(o_name, sizeof(o_name), j_ptr, ODESC_BASE);


			///* Handle normal "breakage" */
			//if (!j_ptr.artifact && (randint0(100) < chance))
			//{
			//    /* Message */
			//    msg("The %s break%s.", o_name, PLURAL(plural));

			//    /* Failure */
			//    return;
			//}


			///* Score */
			//bs = -1;

			///* Picker */
			//bn = 0;

			///* Default */
			//by = y;
			//bx = x;

			///* Scan local grids */
			//for (dy = -3; dy <= 3; dy++)
			//{
			//    /* Scan local grids */
			//    for (dx = -3; dx <= 3; dx++)
			//    {
			//        bool comb = false;

			//        /* Calculate actual distance */
			//        d = (dy * dy) + (dx * dx);

			//        /* Ignore distant grids */
			//        if (d > 10) continue;

			//        /* Location */
			//        ty = y + dy;
			//        tx = x + dx;

			//        /* Skip illegal grids */
			//        if (!in_bounds_fully(ty, tx)) continue;

			//        /* Require line of sight */
			//        if (!los(y, x, ty, tx)) continue;

			//        /* Require floor space */
			//        if (cave.feat[ty][tx] != FEAT_FLOOR) continue;

			//        /* No objects */
			//        k = 0;
			//        n = 0;

			//        /* Scan objects in that grid */
			//        for (o_ptr = get_first_object(ty, tx); o_ptr;
			//                o_ptr = get_next_object(o_ptr))
			//        {
			//            /* Check for possible combination */
			//            if (object_similar(o_ptr, j_ptr, OSTACK_FLOOR))
			//                comb = true;

			//            /* Count objects */
			//            if (!squelch_item_ok(o_ptr))
			//                k++;
			//            else
			//                n++;
			//        }

			//        /* Add new object */
			//        if (!comb) k++;

			//        /* Option -- disallow stacking */
			//        if (OPT(birth_no_stacking) && (k > 1)) continue;
			
			//        /* Paranoia? */
			//        if ((k + n) > MAX_FLOOR_STACK &&
			//                !floor_get_idx_oldest_squelched(ty, tx)) continue;

			//        /* Calculate score */
			//        s = 1000 - (d + k * 5);

			//        /* Skip bad values */
			//        if (s < bs) continue;

			//        /* New best value */
			//        if (s > bs) bn = 0;

			//        /* Apply the randomizer to equivalent values */
			//        if ((++bn >= 2) && (randint0(bn) != 0)) continue;

			//        /* Keep score */
			//        bs = s;

			//        /* Track it */
			//        by = ty;
			//        bx = tx;

			//        /* Okay */
			//        flag = true;
			//    }
			//}


			///* Handle lack of space */
			//if (!flag && !j_ptr.artifact)
			//{
			//    /* Message */
			//    msg("The %s disappear%s.", o_name, PLURAL(plural));

			//    /* Debug */
			//    if (p_ptr.wizard) msg("Breakage (no floor space).");

			//    /* Failure */
			//    return;
			//}


			///* Find a grid */
			//for (i = 0; !flag; i++)
			//{
			//    /* Bounce around */
			//    if (i < 1000)
			//    {
			//        ty = rand_spread(by, 1);
			//        tx = rand_spread(bx, 1);
			//    }

			//    /* Random locations */
			//    else
			//    {
			//        ty = randint0(c.height);
			//        tx = randint0(c.width);
			//    }

			//    /* Require floor space */
			//    if (cave.feat[ty][tx] != FEAT_FLOOR) continue;

			//    /* Bounce to that location */
			//    by = ty;
			//    bx = tx;

			//    /* Require floor space */
			//    if (!cave_clean_bold(by, bx)) continue;

			//    /* Okay */
			//    flag = true;
			//}


			///* Give it to the floor */
			//if (!floor_carry(c, by, bx, j_ptr))
			//{
			//    /* Message */
			//    msg("The %s disappear%s.", o_name, PLURAL(plural));

			//    /* Debug */
			//    if (p_ptr.wizard) msg("Breakage (too many objects).");

			//    if (j_ptr.artifact) j_ptr.artifact.created = false;

			//    /* Failure */
			//    return;
			//}


			///* Sound */
			//sound(MSG_DROP);

			///* Message when an object falls under the player */
			//if (verbose && (cave.m_idx[by][bx] < 0) && !squelch_item_ok(j_ptr))
			//{
			//    msg("You feel something roll beneath your feet.");
			//}
		}
	}
}
