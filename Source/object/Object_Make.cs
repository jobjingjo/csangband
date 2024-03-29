﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Object {
	partial class Object {

		/* Define a value for minima which will be ignored. */
		const int NO_MINIMUM = 255;

		/*
		 * The chance of inflating the requested object level (1/x).
		 * Lower values yield better objects more often.
		 */
		const int GREAT_OBJ = 20;

		/*
		 * There is a 1/20 (5%) chance that ego-items with an inflated base-level are
		 * generated when an object is turned into an ego-item (see make_ego_item()
		 * in object2.c). As above, lower values yield better ego-items more often.
		 */
		const int GREAT_EGO =  20;

		/**
		 * Applying magic to an object, which includes creating ego-items, and applying
		 * random bonuses,
		 *
		 * The `good` argument forces the item to be at least `good`, and the `great`
		 * argument does likewise.  Setting `allow_artifacts` to true allows artifacts
		 * to be created here.
		 *
		 * If `good` or `great` are not set, then the `lev` argument controls the
		 * quality of item.
		 *
		 * Returns 0 if a normal object, 1 if a good object, 2 if an ego item, 3 if an
		 * artifact.
		 */
		public short apply_magic(int lev, bool allow_artifacts, bool good, bool great)
		{
			int i;
			short power = 0;

			/* Chance of being `good` and `great` */
			int good_chance = (lev + 2) * 3;
			int great_chance = Math.Min(lev / 4 + lev, 50);

			/* Roll for "good" */
			if (good || (Random.randint0(100) < good_chance)) {
			    power = 1;

			    /* Roll for "great" */
			    if (great || (Random.randint0(100) < great_chance))
			        power = 2;
			}

			/* Roll for artifact creation */
			if (allow_artifacts) {
			    int rolls = 0;

			    /* Get one roll if excellent */
			    if (power >= 2) rolls = 1;

			    /* Get four rolls if forced great */
			    if (great) rolls = 4;

			    /* Roll for artifacts if allowed */
			    for (i = 0; i < rolls; i++)
			        if (make_artifact()) return 3;
			}

			/* Try to make an ego item */
			if (power == 2)
			    make_ego_item(lev);

			/* Apply magic */
			switch (tval)
			{
			    case TVal.TV_DIGGING:
			    case TVal.TV_HAFTED:
			    case TVal.TV_POLEARM:
			    case TVal.TV_SWORD:
			    case TVal.TV_BOW:
			    case TVal.TV_SHOT:
			    case TVal.TV_ARROW:
			    case TVal.TV_BOLT:
			        apply_magic_weapon(lev, power);
			        break;

			    case TVal.TV_DRAG_ARMOR:
			    case TVal.TV_HARD_ARMOR:
			    case TVal.TV_SOFT_ARMOR:
			    case TVal.TV_SHIELD:
			    case TVal.TV_HELM:
			    case TVal.TV_CROWN:
			    case TVal.TV_CLOAK:
			    case TVal.TV_GLOVES:
			    case TVal.TV_BOOTS:
			        apply_magic_armour(lev, power);
			        break;

			    case TVal.TV_RING:
			        if (sval == (int)SVal.sval_ring.SV_RING_SPEED) {
			            /* Super-charge the ring */
			            while (Random.one_in_(2))
			                pval[which_pval(Object_Flag.SPEED.value)]++;
			        }
			        break;

			    case TVal.TV_CHEST:
			        /* Hack -- skip ruined chests */
			        if (kind.level <= 0) break;

			        /* Hack -- pick a "difficulty" */
			        pval[Misc.DEFAULT_PVAL] = (short)Random.randint1(kind.level);

			        /* Never exceed "difficulty" of 55 to 59 */
			        if (pval[Misc.DEFAULT_PVAL] > 55)
			            pval[Misc.DEFAULT_PVAL] = (short)(55 + Random.randint0(5));

			        break;
			}

			/* Apply minima from ego items if necessary */
			ego_apply_minima();

			return power;
		}



		/*
		 * Attempt to change an object into an artifact.  If the object is already
		 * set to be an artifact, use that, or otherwise use a suitable randomly-
		 * selected artifact.
		 *
		 * This routine should only be called by "apply_magic()"
		 *
		 * Note -- see "make_artifact_special()" and "apply_magic()"
		 */
		bool make_artifact()
		{
			throw new NotImplementedException();
			//artifact_type *a_ptr;
			//int i;
			//bool art_ok = true;

			///* Make sure birth no artifacts isn't set */
			//if (OPT(birth_no_artifacts)) art_ok = false;

			///* Special handling of Grond/Morgoth */
			//if (o_ptr.artifact)
			//{
			//    switch (o_ptr.artifact.aidx)
			//    {
			//        case ART_GROND:
			//        case ART_MORGOTH:
			//            art_ok = true;
			//    }
			//}

			//if (!art_ok) return (false);

			///* No artifacts in the town */
			//if (!p_ptr.depth) return (false);

			///* Paranoia -- no "plural" artifacts */
			//if (o_ptr.number != 1) return (false);

			///* Check the artifact list (skip the "specials") */
			//for (i = ART_MIN_NORMAL; !o_ptr.artifact && i < z_info.a_max; i++) {
			//    a_ptr = &a_info[i];

			//    /* Skip "empty" items */
			//    if (!a_ptr.name) continue;

			//    /* Cannot make an artifact twice */
			//    if (a_ptr.created) continue;

			//    /* Must have the correct fields */
			//    if (a_ptr.tval != o_ptr.tval) continue;
			//    if (a_ptr.sval != o_ptr.sval) continue;

			//    /* XXX XXX Enforce minimum "depth" (loosely) */
			//    if (a_ptr.alloc_min > p_ptr.depth)
			//    {
			//        /* Get the "out-of-depth factor" */
			//        int d = (a_ptr.alloc_min - p_ptr.depth) * 2;

			//        /* Roll for out-of-depth creation */
			//        if (randint0(d) != 0) continue;
			//    }

			//    /* Enforce maximum depth (strictly) */
			//    if (a_ptr.alloc_max < p_ptr.depth) continue;

			//    /* We must make the "rarity roll" */
			//    if (randint1(100) > a_ptr.alloc_prob) continue;

			//    /* Mark the item as an artifact */
			//    o_ptr.artifact = a_ptr;
			//}

			//if (o_ptr.artifact) {
			//    copy_artifact_data(o_ptr, o_ptr.artifact);
			//    o_ptr.artifact.created = 1;
			//    return true;
			//}

			//return false;
		}

		/**
		 * Try to find an ego-item for an object, setting o_ptr.ego if successful and
		 * applying various bonuses.
		 */
		void make_ego_item(int level)
		{
			/* Cannot further improve artifacts or ego items */
			if (artifact != null || ego != null) return;

			/* Occasionally boost the generation level of an item */
			if (level > 0 && Random.one_in_(GREAT_EGO))
			    level = (int)(1 + (level * Misc.MAX_DEPTH / Random.randint1(Misc.MAX_DEPTH)));

			/* Try to get a legal ego type for this item */
			ego = ego_find_random(level);

			/* Actually apply the ego template to the item */
			if (ego != null)
			    ego_apply_magic(level);

			return;
		}

		/**
		 * Select an ego-item that fits the object's tval and sval.
		 */
		Ego_Item ego_find_random(int level)
		{
			int i, j;
			long total = 0L;

			/* XXX alloc_ego_table &c should be static to this file */
			CSAngband.Init.alloc_entry[] table = CSAngband.Init.alloc_ego_table;
			Ego_Item ego;

			/* Go through all possible ego items and find oens which fit this item */
			for (i = 0; i < CSAngband.Init.alloc_ego_size; i++) {
			    /* Reset any previous probability of this type being picked */
			    table[i].prob3 = 0;

			    if (level < table[i].level)
			        continue;

			    /* Access the ego item */
			    ego = Misc.e_info[table[i].index];

			    /* XXX Ignore cursed items for now */
			    if (Object_Flag.cursed_p(ego.flags)) continue;

			    /* Test if this is a legal ego-item type for this object */
			    for (j = 0; j < Misc.EGO_TVALS_MAX; j++) {
			        /* Require identical base type */
			        if (tval == ego.tval[j] &&
			                sval >= ego.min_sval[j] &&
			                sval <= ego.max_sval[j]) {
			            table[i].prob3 = table[i].prob2;
			            break;
			        }
			    }

			    /* Total */
			    total += table[i].prob3;
			}

			if (total != 0) {
			    long value = Random.randint0((int)total);
			    for (i = 0; i < CSAngband.Init.alloc_ego_size; i++) {
			        /* Found the entry */
			        if (value < table[i].prob3) break;

			        /* Decrement */
			        value = value - table[i].prob3;
			    }

			    return Misc.e_info[table[i].index];
			}

			return null;
		}


		/**
		 * Apply generation magic to an ego-item.
		 */
		void ego_apply_magic(int level)
		{
			int i, flag, x;

			Bitflag flags = new Bitflag(Object_Flag.SIZE);
			Bitflag newf = new Bitflag(Object_Flag.SIZE);
			object_flags(ref flags);

			/* Extra powers */
			if (ego.xtra == Object_Flag.OBJECT_XTRA_TYPE_SUSTAIN)
			    Object_Flag.create_mask(newf, false, Object_Flag.object_flag_type.SUST);
			else if (ego.xtra == Object_Flag.OBJECT_XTRA_TYPE_RESIST)
			    Object_Flag.create_mask(newf, false, Object_Flag.object_flag_type.HRES);
			else if (ego.xtra == Object_Flag.OBJECT_XTRA_TYPE_POWER)
			    Object_Flag.create_mask(newf, false, Object_Flag.object_flag_type.PROT, Object_Flag.object_flag_type.MISC);

			if (ego.xtra != 0)
			    this.flags.on(get_new_attr(flags, newf));

			/* Apply extra ego bonuses */
			to_h += (short)Random.randcalc(ego.to_h, level, aspect.RANDOMISE);
			to_d += (short)Random.randcalc(ego.to_d, level, aspect.RANDOMISE);
			to_a += (short)Random.randcalc(ego.to_a, level, aspect.RANDOMISE);

			/* Apply pvals */
			for (i = 0; i < ego.num_pvals; i++) {
			    flags.copy(ego.pval_flags[i]);
			    x = Random.randcalc(ego.pval[i], level, aspect.RANDOMISE);
			    for (flag = flags.next(Bitflag.FLAG_START); flag != Bitflag.FLAG_null; flag = flags.next(flag + 1))
			        add_pval(x, flag);
			}

			/* Apply flags */
			this.flags.union(ego.flags);

			return;
		}

		/**
		 * This is a safe way to choose a random new flag to add to an object.
		 * It takes the existing flags and an array of new flags,
		 * and returns an entry from newf, or 0 if there are no
		 * new flags available.
		 */
		static int get_new_attr(Bitflag flags, Bitflag newf)
		{
			int options = 0, flag = 0;

			for (int i = newf.next(Bitflag.FLAG_START); i != Bitflag.FLAG_null; i = newf.next(i + 1))
			{
				/* skip this one if the flag is already present */
				if (flags.has(i)) continue;

				/* each time we find a new possible option, we have a 1-in-N chance of
				 * choosing it and an (N-1)-in-N chance of keeping a previous one */
				if (Random.one_in_(++options)) flag = i;
			}

			return flag;
		}


		/**
		 * Apply minimum standards for ego-items.
		 */
		void ego_apply_minima()
		{
			if (ego == null) return;

			if (ego.min_to_h != NO_MINIMUM && to_h < ego.min_to_h)
			    to_h = ego.min_to_h;
			if (ego.min_to_d != NO_MINIMUM && to_d < ego.min_to_d)
			    to_d = ego.min_to_d;
			if (ego.min_to_a != NO_MINIMUM && to_a < ego.min_to_a)
			    to_a = ego.min_to_a;

			ego_min_pvals();
		}

		/**
		 * Apply minimum pvals to an ego item. Note that 0 is treated as meaning
		 * "do not apply a minimum to this pval", so it leaves negative pvals alone.
		 */
		//Nick: Prepare your surf board for this one...
		public void ego_min_pvals()
		{
			int i, j, flag;

			if (ego == null) return;

			for(i = 0; i < num_pvals; i++) {
				for(j = 0; j < ego.num_pvals; j++) {
					for(flag = ego.pval_flags[j].next(Bitflag.FLAG_START);
								flag != Bitflag.FLAG_null; flag = ego.pval_flags[j].next(flag + 1)) {


						if(!flags.has(flag) || (ego.min_pval[j] != NO_MINIMUM && pval_flags[i].has(flag) &&
								pval[i] < ego.min_pval[j])) {

							add_pval(ego.min_pval[j] - pval[i], flag);
						}
					}
				}
			}
		}

		/*** Apply magic to an item ***/

		/*
		 * Apply magic to a weapon.
		 */
		public void apply_magic_weapon(int level, int power)
		{
			if (power <= 0)
			    return;

			to_h += (short)(Random.randint1(5) + Random.m_bonus(5, level));
			to_d += (short)(Random.randint1(5) + Random.m_bonus(5, level));

			if (power > 1) {
			    to_h += Random.m_bonus(10, level);
			    to_d += Random.m_bonus(10, level);

			    if (wield_slot() == Misc.INVEN_WIELD || is_ammo()) {
			        /* Super-charge the damage dice */
			        while ((dd * ds > 0) && Random.one_in_((int)(10L * dd * ds)))
			            dd++;

			        /* But not too high */
			        if (dd > 9) dd = 9;
			    }
			}
		}


		/*
		 * Apply magic to armour
		 */
		void apply_magic_armour(int level, int power)
		{
			if (power <= 0)
			    return;

			to_a += (short)(Random.randint1(5) + Random.m_bonus(5, level));
			if (power > 1)
			    to_a += Random.m_bonus(10, level);
		}

		/*** Make a gold item ***/

		/* The largest possible average gold drop at max depth with biggest spread */
		static int MAX_GOLD_DROP = (3 * Misc.MAX_DEPTH + 30);

		/*
		 * Make a money object
		 */
		public static void make_gold(ref Object j_ptr, int lev, int coin_type)
		{
			int sval;

			/* This average is 20 at dlev0, 105 at dlev40, 220 at dlev100. */
			/* Follows the formula: y=2x+20 */
			int avg = 2*lev + 20;
			int spread = lev + 10;
			int value = Random.rand_spread(avg, spread);

			/* Increase the range to infinite, moving the average to 110% */
			while (Random.one_in_(100) && value * 10 <= short.MaxValue)
				value *= 10;

			/* Pick a treasure variety scaled by level, or force a type */
			if (coin_type != (int)SVal.sval_gold.SV_GOLD_ANY)
				sval = coin_type;
			else
				sval = (((value * 100) / MAX_GOLD_DROP) * (int)SVal.sval_gold.SV_GOLD_MAX) / 100;

			/* Do not create illegal treasure types */
			if (sval >= (int)SVal.sval_gold.SV_GOLD_MAX) sval = (int)SVal.sval_gold.SV_GOLD_MAX - 1;

			/* Prepare a gold object */
			j_ptr.prep(Object_Kind.lookup_kind(TVal.TV_GOLD, sval), lev, aspect.RANDOMISE);

			/* If we're playing with no_selling, increase the value */
			if (Option.birth_no_selling.value && Misc.p_ptr.depth != 0)
				value = value * Math.Min(5, (int)Misc.p_ptr.depth);

			/* Cap gold at max short (or alternatively make pvals s32b) */
			if (value > short.MaxValue)
				value = short.MaxValue;

			j_ptr.pval[Misc.DEFAULT_PVAL] = (short)value;
		}

		/*
		 * Attempt to make an object (normal or good/great)
		 *
		 * This routine plays nasty games to generate the "special artifacts".
		 * We assume that the given object has been "wiped". You can optionally
		 * receive the object's value in value if you pass a non-null pointer.
		 *
		 * Returns the whether or not creation worked.
		 */
		public static bool make_object(Cave c, ref Object j_ptr, int lev, bool good, bool great, ref int value)
		{
			int Base;
			Object_Kind kind;

			/* Try to make a special artifact */
			if (Random.one_in_(good ? 10 : 1000)) {
			    if (make_artifact_special(ref j_ptr, lev)) {
			        if (value != 0) value = j_ptr.value_real(1, false, true);
			        return true;
			    }

			    /* If we failed to make an artifact, the player gets a great item */
			    good = great = true;
			}

			/* Base level for the object */
			Base = (good ? (lev + 10) : lev);

			/* Get the object, prep it and apply magic */
			kind = get_obj_num(Base, good || great);
			if (kind == null) return false;
			j_ptr.prep(kind, lev, aspect.RANDOMISE);
			j_ptr.apply_magic(lev, true, good, great);

			/* Generate multiple items */
			if (kind.gen_mult_prob >= Random.randint1(100))
			    j_ptr.number = (byte)Random.randcalc(kind.stack_size, lev, aspect.RANDOMISE);

			if(value != 0) value = j_ptr.value_real(j_ptr.number, false, true);

			/* Return value, increased for uncursed out-of-depth objects */
			if (!Object_Flag.cursed_p(j_ptr.flags) && (kind.alloc_min > c.depth)) {
			    if (value != 0) value = (kind.alloc_min - c.depth) * (value / 5);
			}

			return true;
		}

		/*
		 * Mega-Hack -- Attempt to create one of the "Special Objects".
		 *
		 * We are only called from "make_object()"
		 *
		 * Note -- see "make_artifact()" and "apply_magic()".
		 *
		 * We *prefer* to create the special artifacts in order, but this is
		 * normally outweighed by the "rarity" rolls for those artifacts.
		 */
		static bool make_artifact_special(ref Object o_ptr, int level)
		{
			int i;
			Object_Kind kind;

			/* No artifacts, do nothing */
			if (Option.birth_no_artifacts.value) return false;

			/* No artifacts in the town */
			if (Misc.p_ptr.depth == 0) return false;

			/* Check the special artifacts */
			for (i = 0; i < Misc.ART_MIN_NORMAL; ++i) {
			    Artifact a_ptr = Misc.a_info[i];

			    /* Skip "empty" artifacts */
			    if (a_ptr == null) continue;

			    /* Cannot make an artifact twice */
			    if (a_ptr.created) continue;

			    /* Enforce minimum "depth" (loosely) */
			    if (a_ptr.alloc_min > Misc.p_ptr.depth) {
			        /* Get the "out-of-depth factor" */
			        int d = (a_ptr.alloc_min - Misc.p_ptr.depth) * 2;

			        /* Roll for out-of-depth creation */
			        if (Random.randint0(d) != 0) continue;
			    }

			    /* Enforce maximum depth (strictly) */
			    if (a_ptr.alloc_max < Misc.p_ptr.depth) continue;

			    /* Artifact "rarity roll" */
			    if (Random.randint1(100) > a_ptr.alloc_prob) continue;

			    /* Find the base object */
			    kind = Object_Kind.lookup_kind(a_ptr.tval, a_ptr.sval);

			    /* Make sure the kind was found */
			    if (kind == null) continue;

			    /* Enforce minimum "object" level (loosely) */
			    if (kind.level > level) {
			        /* Get the "out-of-depth factor" */
			        int d = (kind.level - level) * 5;

			        /* Roll for out-of-depth creation */
			        if (Random.randint0(d) != 0) continue;
			    }

			    /* Assign the template */
			    o_ptr.prep(kind, a_ptr.alloc_min, aspect.RANDOMISE);

			    /* Mark the item as an artifact */
			    o_ptr.artifact = a_ptr;

			    /* Copy across all the data from the artifact struct */
			    o_ptr.copy_artifact_data(a_ptr);

			    /* Mark the artifact as "created" */
			    a_ptr.created = true;

			    /* Success */
			    return true;
			}

			/* Failure */
			return false;
		}

		/**
		 * Copy artifact data to a normal object, and set various slightly hacky
		 * globals.
		 */
		public void copy_artifact_data(Artifact a_ptr)
		{
			int i;

			/* Extract the data */
			for(i = 0; i < a_ptr.num_pvals; i++) {
				if(a_ptr.pval[i] != 0) {
					pval[i] = a_ptr.pval[i];
					pval_flags[i].copy(a_ptr.pval_flags[i]);
				}
			}
			num_pvals = a_ptr.num_pvals;
			ac = a_ptr.ac;
			dd = a_ptr.dd;
			ds = a_ptr.ds;
			to_a = a_ptr.to_a;
			to_h = a_ptr.to_h;
			to_d = a_ptr.to_d;
			weight = a_ptr.weight;
			flags.union(a_ptr.flags);
		}
	}
}
