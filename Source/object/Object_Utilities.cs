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
				if (o_ptr == null || o_ptr.kind == null) continue;

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

		public bool is_staff()  { return tval == TVal.TV_STAFF; }
		public bool is_wand()   { return tval == TVal.TV_WAND; }
		public bool is_rod()    { return tval == TVal.TV_ROD; }
		public bool is_potion() { return tval == TVal.TV_POTION; }
		public bool is_scroll() { return tval == TVal.TV_SCROLL; }
		public bool is_food()   { return tval == TVal.TV_FOOD; }
		public bool is_light()	{ return tval == TVal.TV_LIGHT; }
		public bool is_ring()   { return tval == TVal.TV_RING; }


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
		public bool can_zap() { return obj_can_zap(this);}

		/* Determine if an object is activatable */
		public static bool obj_is_activatable(Object o_ptr)
		{
			throw new NotImplementedException();
			//return object_effect(o_ptr) ? true : false;
		}
		public bool is_activatable() { return obj_is_activatable(this); }

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
			return (o_ptr.wield_slot() >= Misc.INVEN_WIELD);
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
			int charging_before, charging_after;

			/* Find the number of charging items */
			charging_before = number_charging();

			/* Nothing to charge */	
			if (charging_before == 0)
			    return false;

			/* Decrease the timeout */
			timeout -= (short)Math.Min(charging_before, timeout);

			/* Find the new number of charging items */
			charging_after = number_charging();

			/* Return true if at least 1 item obtained a charge */
			if (charging_after < charging_before)
			    return true;
			else
			    return false;
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
				if (o_ptr == null) break;

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
			/* Hack -- allow listing empty slots */
			if (Misc.item_tester_full) return (true);

			/* Require an item */
			if (kind == null) return (false);

			/* Hack -- ignore "gold" */
			if (tval == TVal.TV_GOLD) return (false);

			/* Check the tval */
			if (Misc.item_tester_tval != 0)
			{
			    if (Misc.item_tester_tval != tval) return (false);
			}

			/* Check the hook */
			if (Misc.item_tester_hook != null)
			{
			    if (!Misc.item_tester_hook(this)) return (false);
			}

			/* Assume okay */
			return (true);
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
			int i, k, n, d, s;

			int bs, bn;
			int by, bx;
			int dy, dx;
			int ty, tx;

			Object o_ptr;

			//char o_name[80];
			string o_name;

			bool flag = false;

			bool plural = false;


			/* Extract plural */
			if (j_ptr.number != 1) plural = true;

			/* Describe object */
			o_name = j_ptr.object_desc(Detail.BASE);


			/* Handle normal "breakage" */
			if (j_ptr.artifact == null && (Random.randint0(100) < chance))
			{
			    /* Message */
			    Utilities.msg("The {0} break{1}.", o_name, Misc.PLURAL(plural));

			    /* Failure */
			    return;
			}


			/* Score */
			bs = -1;

			/* Picker */
			bn = 0;

			/* Default */
			by = y;
			bx = x;

			/* Scan local grids */
			for (dy = -3; dy <= 3; dy++)
			{
			    /* Scan local grids */
			    for (dx = -3; dx <= 3; dx++)
			    {
			        bool comb = false;

			        /* Calculate actual distance */
			        d = (dy * dy) + (dx * dx);

			        /* Ignore distant grids */
			        if (d > 10) continue;

			        /* Location */
			        ty = y + dy;
			        tx = x + dx;

			        /* Skip illegal grids */
			        if (!Cave.cave.in_bounds_fully(ty, tx)) continue;

			        /* Require line of sight */
			        if (!Cave.los(y, x, ty, tx)) continue;

			        /* Require floor space */
			        if (Cave.cave.feat[ty][tx] != Cave.FEAT_FLOOR) continue;

			        /* No objects */
			        k = 0;
			        n = 0;

			        /* Scan objects in that grid */
			        for (o_ptr = get_first_object(ty, tx); o_ptr != null;
			                o_ptr = get_next_object(o_ptr))
			        {
			            /* Check for possible combination */
			            if (o_ptr.similar(j_ptr, object_stack_t.OSTACK_FLOOR))
			                comb = true;

			            /* Count objects */
			            if (!Squelch.item_ok(o_ptr))
			                k++;
			            else
			                n++;
			        }

			        /* Add new object */
			        if (!comb) k++;

			        /* Option -- disallow stacking */
			        if (Option.birth_no_stacking.value && (k > 1)) continue;
			
			        /* Paranoia? */
			        if ((k + n) > Misc.MAX_FLOOR_STACK && floor_get_idx_oldest_squelched(ty, tx) == 0) continue;

			        /* Calculate score */
			        s = 1000 - (d + k * 5);

			        /* Skip bad values */
			        if (s < bs) continue;

			        /* New best value */
			        if (s > bs) bn = 0;

			        /* Apply the randomizer to equivalent values */
			        if ((++bn >= 2) && (Random.randint0(bn) != 0)) continue;

			        /* Keep score */
			        bs = s;

			        /* Track it */
			        by = ty;
			        bx = tx;

			        /* Okay */
			        flag = true;
			    }
			}


			/* Handle lack of space */
			if (!flag && j_ptr.artifact == null)
			{
			    /* Message */
			    Utilities.msg("The {0} disappear{1}.", o_name, Misc.PLURAL(plural));

			    /* Debug */
			    if (Misc.p_ptr.wizard) Utilities.msg("Breakage (no floor space).");

			    /* Failure */
			    return;
			}


			/* Find a grid */
			for (i = 0; !flag; i++)
			{
			    /* Bounce around */
			    if (i < 1000)
			    {
			        ty = Random.rand_spread(by, 1);
			        tx = Random.rand_spread(bx, 1);
			    }

			    /* Random locations */
			    else
			    {
			        ty = Random.randint0(c.height);
			        tx = Random.randint0(c.width);
			    }

			    /* Require floor space */
			    if (Cave.cave.feat[ty][tx] != Cave.FEAT_FLOOR) continue;

			    /* Bounce to that location */
			    by = ty;
			    bx = tx;

			    /* Require floor space */
			    if (!Cave.cave_clean_bold(by, bx)) continue;

			    /* Okay */
			    flag = true;
			}


			/* Give it to the floor */
			if (floor_carry(c, by, bx, j_ptr) == 0)
			{
			    /* Message */
			    Utilities.msg("The {0} disappear{1}.", o_name, Misc.PLURAL(plural));

			    /* Debug */
			    if (Misc.p_ptr.wizard) Utilities.msg("Breakage (too many objects).");

			    if (j_ptr.artifact != null) j_ptr.artifact.created = false;

			    /* Failure */
			    return;
			}


			/* Sound */
			//sound(MSG_DROP); //Nick: Add this later

			/* Message when an object falls under the player */
			if (verbose && (Cave.cave.m_idx[by][bx] < 0) && !Squelch.item_ok(j_ptr))
			{
			    Utilities.msg("You feel something roll beneath your feet.");
			}
		}

		/*
		 * Let the floor carry an object, deleting old squelched items if necessary
		 */
		public static short floor_carry(Cave c, int y, int x, Object j_ptr)
		{
			int n = 0;

			short o_idx;

			short this_o_idx, next_o_idx = 0;


			/* Scan objects in that grid for combination */
			for (this_o_idx = c.o_idx[y][x]; this_o_idx != 0; this_o_idx = next_o_idx)
			{
				Object o_ptr = byid(this_o_idx);
				if(o_ptr == null)
					continue;

				/* Get the next object */
				next_o_idx = o_ptr.next_o_idx;

				/* Check for combination */
				if (o_ptr.similar(j_ptr, object_stack_t.OSTACK_FLOOR))
				{
					/* Combine the items */
					o_ptr.absorb(j_ptr);

					/* Result */
					return (this_o_idx);
				}

				/* Count objects */
				n++;
			}

			/* Option -- disallow stacking */
			if (Option.birth_no_stacking.value && n != 0) return (0);

			/* The stack is already too large */
			if (n >= Misc.MAX_FLOOR_STACK)
			{
				throw new NotImplementedException();
				///* Squelch the oldest squelched object */
				//short squelch_idx = floor_get_idx_oldest_squelched(y, x);

				//if (squelch_idx)
				//    delete_object_idx(squelch_idx);
				//else
				//    return 0;
			}


			/* Make an object */
			o_idx = o_pop();

			/* Success */
			if (o_idx != 0)
			{
				Object o_ptr;

				/* Get the object */
				o_ptr = byid(o_idx);

				/* Structure Copy */
				o_ptr = j_ptr.copy();

				/* Location */
				o_ptr.iy = (byte)y;
				o_ptr.ix = (byte)x;

				/* Forget monster */
				o_ptr.held_m_idx = 0;

				/* Link the object to the pile */
				o_ptr.next_o_idx = c.o_idx[y][x];

				/* Link the floor to the object */
				c.o_idx[y][x] = o_idx;

				Cave.cave_note_spot(c, y, x);
				Cave.cave_light_spot(c, y, x);
			}

			/* Result */
			return (o_idx);
		}

		/*
		 * Verify the "okayness" of a given item.
		 *
		 * The item can be negative to mean "item on floor".
		 */
		public static bool get_item_okay(int item)
		{
			/* Verify the item */
			return (object_from_item_idx(item).item_tester_okay());
		}

		/*
		 * Convert a label into the index of an item in the "inven".
		 *
		 * Return "-1" if the label does not indicate a real item.
		 */
		public static short label_to_inven(char c)
		{
			int i;

			/* Convert */
			i = (Char.IsLower((char)c) ? Basic.A2I(c) : -1);

			/* Verify the index */
			if ((i < 0) || (i > Misc.INVEN_PACK)) return (-1);

			/* Empty slots can never be chosen */
			if (Misc.p_ptr.inventory[i].kind == null) return (-1);

			/* Return the index */
			return ((short)i);
		}


		/*
		 * Convert a label into the index of a item in the "equip".
		 *
		 * Return "-1" if the label does not indicate a real item.
		 */
		public static short label_to_equip(char c)
		{
			int i;

			/* Convert */
			i = (Char.IsLower((char)c) ? Basic.A2I(c) : -1) + Misc.INVEN_WIELD;

			/* Verify the index */
			if ((i < Misc.INVEN_WIELD) || (i >= Misc.ALL_INVEN_TOTAL)) return (-1);
			if (i == Misc.INVEN_TOTAL) return (-1);

			/* Empty slots can never be chosen */
			if (Misc.p_ptr.inventory[i].kind == null) return (-1);

			/* Return the index */
			return ((short)i);
		}

		/* Get an o_ptr from an item number */
		public static Object object_from_item_idx(int item)
		{
			if (item >= 0)
				return Misc.p_ptr.inventory[item];
			else
				return byid((short)(0 - item));
		}

		/*
		 * Return an object's effect.
		 */
		public Effect effect()
		{
			if (artifact != null)
				return artifact.effect;
			else
				return kind.effect;
		}

		/*
		 * Does the given object need to be aimed?
		 */ 
		public bool needs_aim()
		{
			Effect effect = this.effect();

			/* If the effect needs aiming, or if the object type needs
			   aiming, this object needs aiming. */
			return effect.aim || tval == TVal.TV_BOLT ||
					tval == TVal.TV_SHOT || tval == TVal.TV_ARROW ||
					tval == TVal.TV_WAND ||
					(tval == TVal.TV_ROD && !flavor_is_aware());
		}

		/* 
		 * Check if the given item is available for the player to use. 
		 *
		 * 'mode' defines which areas we should look at, a la scan_items().
		 */
		public static bool item_is_available(int item, Misc.item_tester_hook_func tester, int mode)
		{
			int[] item_list = new int[Misc.ALL_INVEN_TOTAL + Misc.MAX_FLOOR_STACK];
			int item_num;
			int i;

			Misc.item_tester_hook = tester;
			Misc.item_tester_tval = 0;
			item_num = scan_items(item_list, item_list.Length, mode);

			for (i = 0; i < item_num; i++)
			{
			    if (item_list[i] == item)
			        return true;
			}

			return false;
		}

		/*
		 * Get a list of "valid" item indexes.
		 *
		 * Fills item_list[] with items that are "okay" as defined by the
		 * current item_tester_hook, etc.  mode determines what combination of
		 * inventory, equipment and player's floor location should be used
		 * when drawing up the list.
		 *
		 * Returns the number of items placed into the list.
		 *
		 * Maximum space that can be used is [INVEN_TOTAL + MAX_FLOOR_STACK],
		 * though practically speaking much smaller numbers are likely.
		 */
		public static int scan_items(int[] item_list, int item_list_max, int mode)
		{
			bool use_inven = ((mode & Misc.USE_INVEN) != 0 ? true : false);
			bool use_equip = ((mode & Misc.USE_EQUIP) != 0 ? true : false);
			bool use_floor = ((mode & Misc.USE_FLOOR) != 0 ? true : false);

			int[] floor_list = new int[Misc.MAX_FLOOR_STACK];
			int floor_num;

			int i;
			int item_list_num = 0;

			if (use_inven)
			{
				for (i = 0; i < Misc.INVEN_PACK && item_list_num < item_list_max; i++)
				{
					if (get_item_okay(i))
						item_list[item_list_num++] = i;
				}
			}

			if (use_equip)
			{
				for (i = Misc.INVEN_WIELD; i < Misc.ALL_INVEN_TOTAL && item_list_num < item_list_max; i++)
				{
					if (get_item_okay(i))
						item_list[item_list_num++] = i;
				}
			}

			/* Scan all non-gold objects in the grid */
			if (use_floor)
			{
				floor_num = scan_floor(floor_list, floor_list.Length, Misc.p_ptr.py, Misc.p_ptr.px, 0x03);

				for (i = 0; i < floor_num && item_list_num < item_list_max; i++)
				{
					if (get_item_okay(-floor_list[i]))
						item_list[item_list_num++] = -floor_list[i];
				}
			}

			/* Forget the item_tester_tval and item_tester_hook  restrictions */
			Misc.item_tester_tval = 0;
			Misc.item_tester_hook = null;

			return item_list_num;
		}

		/*
		 * Return a string mentioning how a given item is carried
		 */
		public static string mention_use(int slot)
		{
			switch (slot)
			{
				case Misc.INVEN_WIELD:
				{
					if (Player.Player.adj_str_hold[Misc.p_ptr.state.stat_ind[(int)Stat.Str]] < Misc.p_ptr.inventory[slot].weight / 10)
						return "Just lifting";
					else
						return "Wielding";
				}

				case Misc.INVEN_BOW:
				{
					if (Player.Player.adj_str_hold[Misc.p_ptr.state.stat_ind[(int)Stat.Str]] < Misc.p_ptr.inventory[slot].weight / 10)
						return "Just holding";
					else
						return "Shooting";
				}

				case Misc.INVEN_LEFT:  return "On left hand";
				case Misc.INVEN_RIGHT: return "On right hand";
				case Misc.INVEN_NECK:  return "Around neck";
				case Misc.INVEN_LIGHT: return "Light source";
				case Misc.INVEN_BODY:  return "On body";
				case Misc.INVEN_OUTER: return "About body";
				case Misc.INVEN_ARM:   return "On arm";
				case Misc.INVEN_HEAD:  return "On head";
				case Misc.INVEN_HANDS: return "On hands";
				case Misc.INVEN_FEET:  return "On feet";

				case Misc.QUIVER_START + 0: return "In quiver [f0]";
				case Misc.QUIVER_START + 1: return "In quiver [f1]";
				case Misc.QUIVER_START + 2: return "In quiver [f2]";
				case Misc.QUIVER_START + 3: return "In quiver [f3]";
				case Misc.QUIVER_START + 4: return "In quiver [f4]";
				case Misc.QUIVER_START + 5: return "In quiver [f5]";
				case Misc.QUIVER_START + 6: return "In quiver [f6]";
				case Misc.QUIVER_START + 7: return "In quiver [f7]";
				case Misc.QUIVER_START + 8: return "In quiver [f8]";
				case Misc.QUIVER_START + 9: return "In quiver [f9]";
			}

			/*if (slot >= QUIVER_START && slot < QUIVER_END)
				return "In quiver";*/

			return "In pack";
		}

		/*
		 * Excise a dungeon object from any stacks
		 */
		public static void excise_object_idx(int o_idx)
		{
			Object j_ptr;

			short this_o_idx, next_o_idx = 0;

			short prev_o_idx = 0;


			/* Object */
			j_ptr = byid((short)o_idx);

			/* Monster */
			if (j_ptr.held_m_idx != 0)
			{
			    Monster.Monster m_ptr;

			    /* Monster */
			    m_ptr = Cave.cave_monster(Cave.cave, j_ptr.held_m_idx);

			    /* Scan all objects in the grid */
			    for (this_o_idx = m_ptr.hold_o_idx; this_o_idx != 0; this_o_idx = next_o_idx)
			    {
			        Object o_ptr;

			        /* Get the object */
			        o_ptr = byid(this_o_idx);

			        /* Get the next object */
			        next_o_idx = o_ptr.next_o_idx;

			        /* Done */
			        if (this_o_idx == o_idx)
			        {
			            /* No previous */
			            if (prev_o_idx == 0)
			            {
			                /* Remove from list */
			                m_ptr.hold_o_idx = next_o_idx;
			            }

			            /* Real previous */
			            else
			            {
			                Object i_ptr;

			                /* Previous object */
			                i_ptr = byid(prev_o_idx);

			                /* Remove from list */
			                i_ptr.next_o_idx = next_o_idx;
			            }

			            /* Forget next pointer */
			            o_ptr.next_o_idx = 0;

			            /* Done */
			            break;
			        }

			        /* Save prev_o_idx */
			        prev_o_idx = this_o_idx;
			    }
			}

			/* Dungeon */
			else
			{
			    int y = j_ptr.iy;
			    int x = j_ptr.ix;

			    /* Scan all objects in the grid */
			    for (this_o_idx = Cave.cave.o_idx[y][x]; this_o_idx != 0; this_o_idx = next_o_idx)
			    {
			        Object o_ptr;

			        /* Get the object */
			        o_ptr = byid(this_o_idx);

			        /* Get the next object */
			        next_o_idx = o_ptr.next_o_idx;

			        /* Done */
			        if (this_o_idx == o_idx)
			        {
			            /* No previous */
			            if (prev_o_idx == 0)
			            {
			                /* Remove from list */
			                Cave.cave.o_idx[y][x] = next_o_idx;
			            }

			            /* Real previous */
			            else
			            {
			                Object i_ptr;

			                /* Previous object */
			                i_ptr = byid(prev_o_idx);

			                /* Remove from list */
			                i_ptr.next_o_idx = next_o_idx;
			            }

			            /* Forget next pointer */
			            o_ptr.next_o_idx = 0;

			            /* Done */
			            break;
			        }

			        /* Save prev_o_idx */
			        prev_o_idx = this_o_idx;
			    }
			}
		}

		
		/*
		 * Prepare an object `dst` representing `amt` objects,  based on an existing 
		 * object `src` representing at least `amt` objects.
		 *
		 * Takes care of the charge redistribution concerns of stacked items.
		 */
		public static void copy_amt(ref Object dst, Object src, int amt)
		{
			//this is the dest
			int charge_time = Random.randcalc(src.kind.time, 0, aspect.AVERAGE), max_time;

			/* Get a copy of the object */
			dst = src.copy();

			/* Modify quantity */
			dst.number = (byte)amt;
			dst.note = src.note;

			/* 
			 * If the item has charges/timeouts, set them to the correct level 
			 * too. We split off the same amount as distribute_charges.
			 */
			if (src.tval == TVal.TV_WAND || src.tval == TVal.TV_STAFF)
			{
			    dst.pval[Misc.DEFAULT_PVAL] = (short)(src.pval[Misc.DEFAULT_PVAL] * amt / src.number);
			}

			if (src.tval == TVal.TV_ROD)
			{
			    max_time = charge_time * amt;

			    if (src.timeout > max_time)
			        dst.timeout = (short)max_time;
			    else
			        dst.timeout = src.timeout;
			}
		}

		/**
		 * Split off 'at' items from 'src' into 'dest'.
		 */
		public void split(Object src, int amt)
		{
			/* Distribute charges of wands, staves, or rods */
			src.distribute_charges(this, amt);

			/* Modify quantity */
			number = (byte)amt;
			if (src.note != null)
				note = src.note;
		}

		/*
		 * Distribute charges of rods, staves, or wands.
		 *
		 * o_ptr = source item
		 * q_ptr = target item, must be of the same type as o_ptr
		 * amt   = number of items that are transfered
		 */
		public void distribute_charges(Object o_ptr, int amt)
		{
			int charge_time = Random.randcalc(o_ptr.kind.time, 0, aspect.AVERAGE), max_time;

			/*
			 * Hack -- If rods, staves, or wands are dropped, the total maximum
			 * timeout or charges need to be allocated between the two stacks.
			 * If all the items are being dropped, it makes for a neater message
			 * to leave the original stack's pval alone. -LM-
			 */
			if ((o_ptr.tval == TVal.TV_WAND) ||
			    (o_ptr.tval == TVal.TV_STAFF))
			{
			    pval[Misc.DEFAULT_PVAL] = (short)(o_ptr.pval[Misc.DEFAULT_PVAL] * amt / o_ptr.number);

			    if (amt < o_ptr.number)
			        o_ptr.pval[Misc.DEFAULT_PVAL] -= pval[Misc.DEFAULT_PVAL];
			}

			/*
			 * Hack -- Rods also need to have their timeouts distributed.
			 *
			 * The dropped stack will accept all time remaining to charge up to
			 * its maximum.
			 */
			if (o_ptr.tval == TVal.TV_ROD)
			{
			    max_time = charge_time * amt;

			    if (o_ptr.timeout > max_time)
			        timeout = (short)max_time;
			    else
			        timeout = o_ptr.timeout;

			    if (amt < o_ptr.number)
			        o_ptr.timeout -= timeout;
			}
		}

		/*
		 * Describe an item in the inventory. Note: only called when an item is 
		 * dropped, used, or otherwise deleted from the inventory
		 */
		public static void inven_item_describe(int item)
		{
			Object o_ptr = Misc.p_ptr.inventory[item];

			//char o_name[80];
			string o_name;

			if (o_ptr.artifact != null && (o_ptr.is_known() || o_ptr.name_is_visible()))
			{
			    /* Get a description */
				o_name = o_ptr.object_desc(Detail.FULL | Detail.SINGULAR);

			    /* Print a message */
			    Utilities.msg("You no longer have the {0} ({1}).", o_name, index_to_label(item));
			}
			else
			{
			    /* Get a description */
				o_name = o_ptr.object_desc(Detail.PREFIX | Detail.FULL);

			    /* Print a message */
			    Utilities.msg("You have {0} ({1}).", o_name, index_to_label(item));
			}
		}

		/*
		 * Increase the "number" of an item on the floor
		 */
		public static void floor_item_increase(int item, int num)
		{
			throw new NotImplementedException();
			//object_type *o_ptr = object_byid(item);

			///* Apply */
			//num += o_ptr.number;

			///* Bounds check */
			//if (num > 255) num = 255;
			//else if (num < 0) num = 0;

			///* Un-apply */
			//num -= o_ptr.number;

			///* Change the number */
			//o_ptr.number += num;
		}

		/*
		 * Describe an item in the inventory.
		 */
		public static void floor_item_describe(int item)
		{
			//object_type *o_ptr = object_byid(item);

			//char o_name[80];

			///* Get a description */
			//object_desc(o_name, sizeof(o_name), o_ptr, ODESC_PREFIX | ODESC_FULL);

			///* Print a message */
			//msg("You see %s.", o_name);
		}

		
		/*
		 * Optimize an item on the floor (destroy "empty" items)
		 */
		public static void floor_item_optimize(int item)
		{
			throw new NotImplementedException();
			//object_type *o_ptr = object_byid(item);

			///* Paranoia -- be sure it exists */
			//if (!o_ptr.kind) return;

			///* Only optimize empty items */
			//if (o_ptr.number) return;

			///* Delete the object */
			//delete_object_idx(item);
		}

		/**
		 * Find and return the index to the oldest object on the given grid marked as
		 * "squelch".
		 */
		static short floor_get_idx_oldest_squelched(int y, int x)
		{
			throw new NotImplementedException();
			//s16b squelch_idx = 0;
			//s16b this_o_idx;

			//object_type *o_ptr = null;

			//for (this_o_idx = cave.o_idx[y][x]; this_o_idx; this_o_idx = o_ptr.next_o_idx)
			//{
			//    o_ptr = object_byid(this_o_idx);

			//    if (squelch_item_ok(o_ptr))
			//        squelch_idx = this_o_idx;
			//}

			//return squelch_idx;
		}

		/*
		 * \returns whether item this will fit in slot 'slot'
		 */
		public bool slot_can_wield_item(int slot)
		{
			if (tval == TVal.TV_RING)
				return (slot == Misc.INVEN_LEFT || slot == Misc.INVEN_RIGHT) ? true : false;
			else if (is_ammo())
				return (slot >= Misc.QUIVER_START && slot < Misc.QUIVER_END) ? true : false;
			else
				return (wield_slot() == slot) ? true : false;
		}


		/*
		 * Shifts ammo at or above the item slot towards the end of the quiver, making
		 * room for a new piece of ammo.
		 */
		public static void open_quiver_slot(int slot)
		{
			int i, pref;
			int dest = Misc.QUIVER_END - 1;

			/* This should only be used on ammunition */
			if (slot < Misc.QUIVER_START) return;

			/* Quiver is full */
			if (Misc.p_ptr.inventory[Misc.QUIVER_END - 1].kind != null) return;

			/* Find the first open quiver slot */
			while (Misc.p_ptr.inventory[dest].kind != null) dest++;

			/* Swap things with the space one higher (essentially moving the open space
			 * towards our goal slot. */
			for (i = dest - 1; i >= slot; i--)
			{
			    /* If we have an item with an inscribed location (and it's in */
			    /* that location) then we won't move it. */
			    pref = Misc.p_ptr.inventory[i].get_inscribed_ammo_slot();
			    if (i != slot && pref != 0 && pref == i) continue;

			    /* Update object_idx if necessary */
			    if (Cave.tracked_object_is(i))
			    {
			        Cave.track_object(dest);
			    }

			    /* Copy the item up and wipe the old slot */
				Misc.p_ptr.inventory[dest] = Misc.p_ptr.inventory[i];
			    dest = i;
			    Misc.p_ptr.inventory[dest] = new Object();
			}
		}

		/*
		 * Take off (some of) a non-cursed equipment item
		 *
		 * Note that only one item at a time can be wielded per slot.
		 *
		 * Note that taking off an item when "full" may cause that item
		 * to fall to the ground.
		 *
		 * Return the inventory slot into which the item is placed.
		 */
		public static short inven_takeoff(int item, int amt)
		{
			int slot;

			Object o_ptr;

			Object i_ptr;
			//object_type object_type_body;

			string act;

			//char o_name[80];
			string o_name;

			bool track_removed_item = false;


			/* Get the item to take off */
			o_ptr = Misc.p_ptr.inventory[item];

			/* Paranoia */
			if (amt <= 0) return (-1);

			/* Verify */
			if (amt > o_ptr.number) amt = o_ptr.number;

			/* Get local object */
			i_ptr = new Object();//&object_type_body;

			/* Obtain a local object */
			i_ptr = o_ptr.copy();
			//object_copy(i_ptr, o_ptr);

			/* Modify quantity */
			i_ptr.number = (byte)amt;

			/* Describe the object */
			o_name = i_ptr.object_desc(Detail.PREFIX | Detail.FULL);

			/* Took off weapon */
			if (item == Misc.INVEN_WIELD)
			{
			    act = "You were wielding";
			}

			/* Took off bow */
			else if (item == Misc.INVEN_BOW)
			{
			    act = "You were holding";
			}

			/* Took off light */
			else if (item == Misc.INVEN_LIGHT)
			{
			    act = "You were holding";
			}

			/* Took off something */
			else
			{
			    act = "You were wearing";
			}

			/* Update object_idx if necessary, after optimization */
			if (Cave.tracked_object_is(item))
			{
			    track_removed_item = true;
			}

			/* Modify, Optimize */
			inven_item_increase(item, -amt);
			inven_item_optimize(item);

			/* Carry the object */
			slot = i_ptr.inven_carry(Misc.p_ptr);

			/* Track removed item if necessary */
			if (track_removed_item)
			{
			    Cave.track_object(slot);
			}

			/* Message */
			Utilities.msgt(Message_Type.MSG_WIELD, "{0} {1} ({2}).", act, o_name, index_to_label(slot));

			Misc.p_ptr.notice |= Misc.PN_SQUELCH;

			/* Return slot */
			return (short)(slot);
		}


		/*
		 * Deletes all objects at given location
		 */
		public static void delete_object(int y, int x)
		{
			throw new NotImplementedException();
			//short this_o_idx, next_o_idx = 0;

			///* Paranoia */
			//if (!in_bounds(y, x)) return;

			///* Scan all objects in the grid */
			//for (this_o_idx = cave.o_idx[y][x]; this_o_idx; this_o_idx = next_o_idx) {
			//    object_type *o_ptr;

			//    /* Get the object */
			//    o_ptr = object_byid(this_o_idx);

			//    /* Get the next object */
			//    next_o_idx = o_ptr.next_o_idx;

			//    /* Preserve unseen artifacts */
			//    if (o_ptr.artifact && !object_was_sensed(o_ptr))
			//        o_ptr.artifact.created = false;

			//    /* Delete the mimicking monster if necessary */
			//    if (o_ptr.mimicking_m_idx) {
			//        monster_type *m_ptr;
			
			//        m_ptr = cave_monster(cave, o_ptr.mimicking_m_idx);
			
			//        /* Clear the mimicry */
			//        m_ptr.mimicked_o_idx = 0;
			
			//        delete_monster_idx(o_ptr.mimicking_m_idx);
			//    }

			//    /* Wipe the object */
			//    object_wipe(o_ptr);

			//    /* Count objects */
			//    o_cnt--;
			//}

			///* Objects are gone */
			//cave.o_idx[y][x] = 0;

			///* Visual update */
			//cave_light_spot(cave, y, x);
		}



		/*
		 * Move an object from index i1 to index i2 in the object list
		 */
		static void compact_objects_aux(int i1, int i2)
		{
			int i;

			Object o_ptr;


			/* Do nothing */
			if (i1 == i2) return;


			/* Repair objects */
			for (i = 1; i < Misc.o_max; i++) {
				/* Get the object */
				o_ptr = Object.byid((short)i);

				/* Skip "dead" objects */
				if (o_ptr == null || o_ptr.kind == null) continue;

				/* Repair "next" pointers */
				if (o_ptr.next_o_idx == i1) {
					/* Repair */
					o_ptr.next_o_idx = (short)i2;
				}
			}


			/* Get the object */
			o_ptr = Object.byid((short)i1);


			/* Monster */
			if (o_ptr.held_m_idx != 0) {
				Monster.Monster m_ptr;

				/* Get the monster */
				m_ptr = Cave.cave_monster(Cave.cave, o_ptr.held_m_idx);

				/* Repair monster */
				if (m_ptr.hold_o_idx == i1) {
					/* Repair */
					m_ptr.hold_o_idx = (short)i2;
				}
			}

			/* Dungeon */
			else {
				int y, x;

				/* Get location */
				y = o_ptr.iy;
				x = o_ptr.ix;

				/* Repair grid */
				if (Cave.cave.o_idx[y][x] == i1) {
					/* Repair */
					Cave.cave.o_idx[y][x] = (short)i2;
				}

				/* Mimic */
				if (o_ptr.mimicking_m_idx != 0) {
					Monster.Monster m_ptr;

					/* Get the monster */
					m_ptr = Cave.cave_monster(Cave.cave, o_ptr.mimicking_m_idx);

					/* Repair monster */
					if (m_ptr.mimicked_o_idx == i1) {
						/* Repair */
						m_ptr.mimicked_o_idx = (short)i2;
					}
				}
			}


			/* Hack -- move object */
			//Object.byid(i2).copy(Object.byid(i1));
			o_list[i2] = o_list[i1];

			/* Hack -- wipe hole */
			//object_wipe(o_ptr);
		}


		/*
		 * Compact and reorder the object list
		 *
		 * This function can be very dangerous, use with caution!
		 *
		 * When compacting objects, we first destroy gold, on the basis that by the
		 * time item compaction becomes an issue, the player really won't care.
		 * We also nuke items marked as squelch.
		 *
		 * When compacting other objects, we base the saving throw on a combination of
		 * object level, distance from player, and current "desperation".
		 *
		 * After compacting, we "reorder" the objects into a more compact order, and we
		 * reset the allocation info, and the "live" array.
		 */
		public static void compact_objects(int size)
		{
			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			int i, y, x, cnt;

			int cur_lev, cur_dis, chance;


			/* Reorder objects when not passed a size */
			if (size == 0) {
				/* Excise dead objects (backwards!) */
				for (i = Misc.o_max - 1; i >= 1; i--) {
					Object o_ptr = Object.byid((short)i);
					if (o_ptr != null && o_ptr.kind != null) continue;

					/* Move last object into open hole */
					compact_objects_aux(Misc.o_max - 1, i);

					/* Compress "o_max" */
					Misc.o_max--;
				}

				return;
			}


			/* Message */
			Utilities.msg("Compacting objects...");

			/*** Try destroying objects ***/

			/* First do gold */
			for (i = 1; (i < Misc.o_max) && (size != 0); i++) {
				Object o_ptr = Object.byid((short)i);

				/* Nuke gold or squelched items */
				if (o_ptr.tval == TVal.TV_GOLD || Squelch.item_ok(o_ptr)) {
					delete_object_idx(i);
					size--;
				}
			}


			/* Compact at least 'size' objects */
			for (cnt = 1; size != 0; cnt++) {
				/* Get more vicious each iteration */
				cur_lev = 5 * cnt;

				/* Get closer each iteration */
				cur_dis = 5 * (20 - cnt);

				/* Examine the objects */
				for (i = 1; (i < Misc.o_max) && (size != 0); i++) {
					Object o_ptr = Object.byid((short)i);
					if (o_ptr == null || o_ptr.kind == null) continue;

					/* Hack -- High level objects start out "immune" */
					if (o_ptr.kind.level > cur_lev && o_ptr.kind.squelch == null)
						continue;

					/* Monster */
					if (o_ptr.held_m_idx != 0) {
						Monster.Monster m_ptr;

						/* Get the monster */
						m_ptr = Cave.cave_monster(Cave.cave, o_ptr.held_m_idx);

						/* Get the location */
						y = m_ptr.fy;
						x = m_ptr.fx;

						/* Monsters protect their objects */
						if ((Random.randint0(100) < 90) && o_ptr.kind.squelch == null)
							continue;
					}

					/* Mimicked items */
					else if (o_ptr.mimicking_m_idx != 0) {
						/* Get the location */
						y = o_ptr.iy;
						x = o_ptr.ix;

						/* Mimicked items try hard not to be compacted */
						if (Random.randint0(100) < 90)
							continue;
					}

					/* Dungeon */
					else {
						/* Get the location */
						y = o_ptr.iy;
						x = o_ptr.ix;
					}

					/* Nearby objects start out "immune" */
					if ((cur_dis > 0) && (Cave.distance(py, px, y, x) < cur_dis) && o_ptr.kind.squelch == null)
						continue;

					/* Saving throw */
					chance = 90;


					/* Hack -- only compact artifacts in emergencies */
					if (o_ptr.artifact != null && (cnt < 1000)) chance = 100;

					/* Apply the saving throw */
					if (Random.randint0(100) < chance) continue;

					/* Delete the object */
					delete_object_idx(i);
					size--;
				}
			}


			/* Reorder objects */
			compact_objects(0);
		}



		/*
		 * Determine if a weapon is 'blessed'
		 */
		public static bool is_blessed(Object o_ptr)
		{
			throw new NotImplementedException();
			//Bitflag f = new Bitflag(Object_Flag.SIZE);

			///* Get the flags */
			//object_flags(o_ptr, f);

			///* Is the object blessed? */
			//return (of_has(f, OF_BLESSED) ? true : false);
		}







		/**
		 * This will push objects off a square.
		 * 
		 * The methodology is to load all objects on the square into a queue. Replace
		 * the previous square with a type that does not allow for objects. Drop the
		 * objects. Last, put the square back to its original type.
		 */
		public static void push_object(int y, int x)
		{
			throw new NotImplementedException();
			///* Save the original terrain feature */
			//int feat_old = cave.feat[y][x];

			//object_type *o_ptr;
   
			//struct queue *queue = q_new(MAX_FLOOR_STACK);

			///* Push all objects on the square into the queue */
			//for (o_ptr = get_first_object(y, x); o_ptr; o_ptr = get_next_object(o_ptr))
			//    q_push_ptr(queue, o_ptr);

			///* Set feature to an open door */
			//cave_set_feat(cave, y, x, FEAT_OPEN);
	
			///* Drop objects back onto the floor */
			//while (q_len(queue) > 0)
			//{
			//    /* Take object from the queue */
			//    o_ptr = q_pop_ptr(queue);
	
			//    /* Drop the object */
			//    drop_near(cave, o_ptr, 0, y, x, false);
			//}
	
			///* Delete original objects */
			//delete_object(y, x);
	
			///* Reset cave feature */
			//cave_set_feat(cave, y, x, feat_old);
	
			//q_free(queue);
		}

		/*
		 * Scatter some "great" objects near the player
		 */
		public static void acquirement(int y1, int x1, int level, int num, bool great)
		{
			throw new NotImplementedException();
			//object_type *i_ptr;
			//object_type object_type_body;

			///* Acquirement */
			//while (num--)
			//{
			//    /* Get local object */
			//    i_ptr = &object_type_body;

			//    /* Wipe the object */
			//    object_wipe(i_ptr);

			//    /* Make a good (or great) object (if possible) */
			//    if (!make_object(cave, i_ptr, level, true, great, null)) continue;

			//    i_ptr.origin = ORIGIN_ACQUIRE;
			//    i_ptr.origin_depth = p_ptr.depth;

			//    /* Drop the object */
			//    drop_near(cave, i_ptr, 0, y1, x1, true);
			//}
		}


		/*
		 * Describe the charges on an item in the inventory.
		 */
		public static void inven_item_charges(int item)
		{
			throw new NotImplementedException();
			//object_type *o_ptr = &p_ptr.inventory[item];

			///* Require staff/wand */
			//if ((o_ptr.tval != TV_STAFF) && (o_ptr.tval != TV_WAND)) return;

			///* Require known item */
			//if (!object_is_known(o_ptr)) return;

			///* Print a message */
			//msg("You have %d charge%s remaining.", o_ptr.pval[DEFAULT_PVAL],
			//    (o_ptr.pval[DEFAULT_PVAL] != 1) ? "s" : "");
		}




		/**
		 * Swap ammunition between quiver slots (0-9).
		 */
		public static void swap_quiver_slots(int slot1, int slot2)
		{
			throw new NotImplementedException();
			//int i = slot1 + QUIVER_START;
			//int j = slot2 + QUIVER_START;
			//object_type o;

			//object_copy(&o, &p_ptr.inventory[i]);
			//object_copy(&p_ptr.inventory[i], &p_ptr.inventory[j]);
			//object_copy(&p_ptr.inventory[j], &o);

			///* Update object_idx if necessary */
			//if (tracked_object_is(i))
			//{
			//    track_object(j);
			//}

			//if (tracked_object_is(j))
			//{
			//    track_object(i);
			//}
		}

		/*
		 * Describe the charges on an item on the floor.
		 */
		public static void floor_item_charges(int item)
		{
			throw new NotImplementedException();
			//object_type *o_ptr = object_byid(item);

			///* Require staff/wand */
			//if ((o_ptr.tval != TV_STAFF) && (o_ptr.tval != TV_WAND)) return;

			///* Require known item */
			//if (!object_is_known(o_ptr)) return;

			///* Print a message */
			//msg("There %s %d charge%s remaining.",
			//    (o_ptr.pval[DEFAULT_PVAL] != 1) ? "are" : "is",
			//     o_ptr.pval[DEFAULT_PVAL],
			//    (o_ptr.pval[DEFAULT_PVAL] != 1) ? "s" : "");
		}


		/*
		 *Returns the number of times in 1000 that @ will FAIL
		 * - thanks to Ed Graham for the formula
		 */
		public static int get_use_device_chance(Object o_ptr)
		{
			throw new NotImplementedException();
			//int lev, fail, numerator, denominator;

			//int skill = p_ptr.state.skills[SKILL_DEVICE];

			//int skill_min = 10;
			//int skill_max = 141;
			//int diff_min  = 1;
			//int diff_max  = 100;

			///* Extract the item level, which is the difficulty rating */
			//if (o_ptr.artifact)
			//    lev = o_ptr.artifact.level;
			//else
			//    lev = o_ptr.kind.level;

			///* TODO: maybe use something a little less convoluted? */
			//numerator   = (skill - lev) - (skill_max - diff_min);
			//denominator = (lev - skill) - (diff_max - skill_min);

			///* Make sure that we don't divide by zero */
			//if (denominator == 0) denominator = numerator > 0 ? 1 : -1;

			//fail = (100 * numerator) / denominator;

			///* Ensure failure rate is between 1% and 75% */
			//if (fail > 750) fail = 750;
			//if (fail < 10) fail = 10;

			//return fail;
		}

		public static void reduce_charges(Object o_ptr, int amt)
		{
			throw new NotImplementedException();
			///*
			// * Hack -- If rods or wand are destroyed, the total maximum timeout or
			// * charges of the stack needs to be reduced, unless all the items are
			// * being destroyed. -LM-
			// */
			//if (((o_ptr.tval == TV_WAND) ||
			//     (o_ptr.tval == TV_STAFF)) &&
			//    (amt < o_ptr.number))
			//{
			//    o_ptr.pval[DEFAULT_PVAL] -= o_ptr.pval[DEFAULT_PVAL] * amt / o_ptr.number;
			//}

			//if ((o_ptr.tval == TV_ROD) &&
			//    (amt < o_ptr.number))
			//{
			//    o_ptr.timeout -= o_ptr.timeout * amt / o_ptr.number;
			//}
		}

		/*** Object kind lookup functions ***/


		/**
		 * Return the k_idx of the object kind with the given `tval` and name `name`.
		 */
		public static int lookup_name(int tval, string name)
		{
			throw new NotImplementedException();
			//int k;

			///* Look for it */
			//for (k = 1; k < z_info.k_max; k++)
			//{
			//    object_kind *k_ptr = &k_info[k];
			//    const char *nm = k_ptr.name;

			//    if (!nm) continue;

			//    if (*nm == '&' && *(nm+1))
			//        nm += 2;

			//    /* Found a match */
			//    if (k_ptr.tval == tval && !strcmp(name, nm))
			//        return k;
			//}

			//msg("No object (\"%s\",\"%s\")", tval_find_name(tval), name);
			//return -1;
		}


		/**
		 * Sort comparator for objects using only tval and sval.
		 * -1 if o1 should be first
		 *  1 if o2 should be first
		 *  0 if it doesn't matter
		 */
		public static int compare_types(Object o1, Object o2)
		{
			throw new NotImplementedException();
			//if (o1.tval == o2.tval)
			//    return CMP(o1.sval, o2.sval);
			//else
			//    return CMP(o1.tval, o2.tval);
		}

		/**
		 * Sort comparator for objects
		 * -1 if o1 should be first
		 *  1 if o2 should be first
		 *  0 if it doesn't matter
		 *
		 * The sort order is designed with the "list items" command in mind.
		 */
		public static int compare_items(Object o1, Object o2)
		{
			throw new NotImplementedException();
			///* known artifacts will sort first */
			//if (object_is_known_artifact(o1) && object_is_known_artifact(o2))
			//    return compare_types(o1, o2);
			//if (object_is_known_artifact(o1)) return -1;
			//if (object_is_known_artifact(o2)) return 1;

			///* unknown objects will sort next */
			//if (!object_flavor_is_aware(o1) && !object_flavor_is_aware(o2))
			//    return compare_types(o1, o2);
			//if (!object_flavor_is_aware(o1)) return -1;
			//if (!object_flavor_is_aware(o2)) return 1;

			///* if only one of them is worthless, the other comes first */
			//if (o1.kind.cost == 0 && o2.kind.cost != 0) return 1;
			//if (o1.kind.cost != 0 && o2.kind.cost == 0) return -1;

			///* otherwise, just compare tvals and svals */
			///* NOTE: arguably there could be a better order than this */
			//return compare_types(o1, o2);
		}


		/**
		 * Helper to draw the Object Recall subwindow; this actually does the work.
		 */
		public static void display_object_recall(Object o_ptr)
		{
			throw new NotImplementedException();
			//char header[120];

			//textblock *tb = object_info(o_ptr, OINFO_NONE);
			//object_desc(header, sizeof(header), o_ptr, ODESC_PREFIX | ODESC_FULL);

			//clear_from(0);
			//textui_textblock_place(tb, SCREEN_REGION, header);
			//textblock_free(tb);
		}


		/**
		 * This draws the Object Recall subwindow when displaying a particular object
		 * (e.g. a helmet in the backpack, or a scroll on the ground)
		 */
		public static void display_object_idx_recall(short item)
		{
			throw new NotImplementedException();
			//object_type *o_ptr = object_from_item_idx(item);
			//display_object_recall(o_ptr);
		}


		/**
		 * This draws the Object Recall subwindow when displaying a recalled item kind
		 * (e.g. a generic ring of acid or a generic blade of chaos)
		 */
		public static void display_object_kind_recall(short k_idx)
		{
			throw new NotImplementedException();
			//object_type object = { 0 };
			//object_prep(&object, &k_info[k_idx], 0, EXTREMIFY);
			//if (k_info[k_idx].aware)
			//    object.ident |= IDENT_STORE;

			//display_object_recall(&object);
		}

		/*
		 * Display visible items, similar to display_monlist
		 */
		public static void display_itemlist()
		{
			throw new NotImplementedException();
			//int max;
			//int mx, my;
			//unsigned num;
			//int line = 1, x = 0;
			//int cur_x;
			//unsigned i;
			//unsigned disp_count = 0;
			//byte a;
			//char c;

			//object_type *types[MAX_ITEMLIST];
			//int counts[MAX_ITEMLIST];
			//int dx[MAX_ITEMLIST], dy[MAX_ITEMLIST];
			//unsigned counter = 0;

			//int dungeon_hgt = p_ptr.depth == 0 ? TOWN_HGT : DUNGEON_HGT;
			//int dungeon_wid = p_ptr.depth == 0 ? TOWN_WID : DUNGEON_WID;

			//byte attr;
			//char buf[80];

			//int floor_list[MAX_FLOOR_STACK];

			///* Clear the term if in a subwindow, set x otherwise */
			//if (Term != angband_term[0])
			//{
			//    clear_from(0);
			//    max = Term.hgt - 1;
			//}
			//else
			//{
			//    x = 13;
			//    max = Term.hgt - 2;
			//}

			///* Look at each square of the dungeon for items */
			//for (my = 0; my < dungeon_hgt; my++)
			//{
			//    for (mx = 0; mx < dungeon_wid; mx++)
			//    {
			//        num = scan_floor(floor_list, MAX_FLOOR_STACK, my, mx, 0x02);

			//        /* Iterate over all the items found on this square */
			//        for (i = 0; i < num; i++)
			//        {
			//            object_type *o_ptr = object_byid(floor_list[i]);
			//            unsigned j;

			//            /* Skip gold/squelched */
			//            if (o_ptr.tval == TV_GOLD || squelch_item_ok(o_ptr))
			//                continue;

			//            /* See if we've already seen a similar item; if so, just add */
			//            /* to its count */
			//            for (j = 0; j < counter; j++)
			//            {
			//                if (object_similar(o_ptr, types[j],
			//                    OSTACK_LIST))
			//                {
			//                    counts[j] += o_ptr.number;
			//                    if ((my - p_ptr.py) * (my - p_ptr.py) + (mx - p_ptr.px) * (mx - p_ptr.px) < dy[j] * dy[j] + dx[j] * dx[j])
			//                    {
			//                        dy[j] = my - p_ptr.py;
			//                        dx[j] = mx - p_ptr.px;
			//                    }
			//                    break;
			//                }
			//            }

			//            /* We saw a new item. So insert it at the end of the list and */
			//            /* then sort it forward using compare_items(). The types list */
			//            /* is always kept sorted. */
			//            if (j == counter)
			//            {
			//                types[counter] = o_ptr;
			//                counts[counter] = o_ptr.number;
			//                dy[counter] = my - p_ptr.py;
			//                dx[counter] = mx - p_ptr.px;

			//                while (j > 0 && compare_items(types[j - 1], types[j]) > 0)
			//                {
			//                    object_type *tmp_o = types[j - 1];
			//                    int tmpcount;
			//                    int tmpdx = dx[j-1];
			//                    int tmpdy = dy[j-1];

			//                    types[j - 1] = types[j];
			//                    types[j] = tmp_o;
			//                    dx[j-1] = dx[j];
			//                    dx[j] = tmpdx;
			//                    dy[j-1] = dy[j];
			//                    dy[j] = tmpdy;
			//                    tmpcount = counts[j - 1];
			//                    counts[j - 1] = counts[j];
			//                    counts[j] = tmpcount;
			//                    j--;
			//                }
			//                counter++;
			//            }
			//        }
			//    }
			//}

			///* Note no visible items */
			//if (!counter)
			//{
			//    /* Clear display and print note */
			//    c_prt(TERM_SLATE, "You see no items.", 0, 0);
			//    if (Term == angband_term[0])
			//        Term_addstr(-1, TERM_WHITE, "  (Press any key to continue.)");

			//    /* Done */
			//    return;
			//}
			//else
			//{
			//    /* Reprint Message */
			//    prt(format("You can see %d item%s:",
			//               counter, (counter > 1 ? "s" : "")), 0, 0);
			//}

			//for (i = 0; i < counter; i++)
			//{
			//    /* o_name will hold the object_desc() name for the object. */
			//    /* o_desc will also need to put a (x4) behind it. */
			//    /* can there be more than 999 stackable items on a level? */
			//    char o_name[80];
			//    char o_desc[86];

			//    object_type *o_ptr = types[i];

			//    /* We shouldn't list coins or squelched items */
			//    if (o_ptr.tval == TV_GOLD || squelch_item_ok(o_ptr))
			//        continue;

			//    object_desc(o_name, sizeof(o_name), o_ptr, ODESC_FULL);
			//    if (counts[i] > 1)
			//        strnfmt(o_desc, sizeof(o_desc), "%s (x%d) %d %c, %d %c", o_name, counts[i],
			//            (dy[i] > 0) ? dy[i] : -dy[i], (dy[i] > 0) ? 'S' : 'N',
			//            (dx[i] > 0) ? dx[i] : -dx[i], (dx[i] > 0) ? 'E' : 'W');
			//    else
			//        strnfmt(o_desc, sizeof(o_desc), "%s  %d %c %d %c", o_name,
			//            (dy[i] > 0) ? dy[i] : -dy[i], (dy[i] > 0) ? 'S' : 'N',
			//            (dx[i] > 0) ? dx[i] : -dx[i], (dx[i] > 0) ? 'E' : 'W');

			//    /* Reset position */
			//    cur_x = x;

			//    /* See if we need to scroll or not */
			//    if (Term == angband_term[0] && (line == max) && disp_count != counter)
			//    {
			//        prt("-- more --", line, x);
			//        anykey();

			//        /* Clear the screen */
			//        for (line = 1; line <= max; line++)
			//            prt("", line, x);

			//        /* Reprint Message */
			//        prt(format("You can see %d item%s:",
			//                   counter, (counter > 1 ? "s" : "")), 0, 0);

			//        /* Reset */
			//        line = 1;
			//    }
			//    else if (line == max)
			//    {
			//        continue;
			//    }

			//    /* Note that the number of items actually displayed */
			//    disp_count++;

			//    if (o_ptr.artifact && object_is_known(o_ptr))
			//        /* known artifact */
			//        attr = TERM_VIOLET;
			//    else if (!object_flavor_is_aware(o_ptr))
			//        /* unaware of kind */
			//        attr = TERM_RED;
			//    else if (o_ptr.kind.cost == 0)
			//        /* worthless */
			//        attr = TERM_SLATE;
			//    else
			//        /* default */
			//        attr = TERM_WHITE;

			//    a = object_kind_attr(o_ptr.kind);
			//    c = object_kind_char(o_ptr.kind);

			//    /* Display the pict */
			//    if ((tile_width == 1) && (tile_height == 1))
			//    {
			//            Term_putch(cur_x++, line, a, c);
			//        Term_putch(cur_x++, line, TERM_WHITE, ' ');
			//    }

			//    /* Print and bump line counter */
			//    c_prt(attr, o_desc, line, cur_x);
			//    line++;
			//}

			//if (disp_count != counter)
			//{
			//    /* Print "and others" message if we've run out of space */
			//    strnfmt(buf, sizeof buf, "  ...and %d others.", counter - disp_count);
			//    c_prt(TERM_WHITE, buf, line, x);
			//}
			//else
			//{
			//    /* Otherwise clear a line at the end, for main-term display */
			//    prt("", line, x);
			//}

			//if (Term == angband_term[0])
			//    Term_addstr(-1, TERM_WHITE, "  (Press any key to continue.)");
		}


		/*** Generic utility functions ***/

		/*
		 * Returns whether the pack is holding the maximum number of items. The max
		 * size is INVEN_MAX_PACK, which is a macro since quiver size affects slots
		 * available.
		 */
		public static bool pack_is_full()
		{
			return Misc.p_ptr.inventory[Misc.INVEN_MAX_PACK - 1].kind != null;
		}

		public static void objects_destroy()
		{
			throw new NotImplementedException();
			//mem_free(o_list);
		}

	}
}
