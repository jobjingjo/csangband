using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CSAngband.Monster;

namespace CSAngband.Object {
	partial class Object {
		/**
		 * Constants for the power algorithm:
		 * - fudge factor for extra damage from rings etc. (used if extra blows)
		 * - assumed damage for off-weapon brands
		 * - base power for jewelry
		 * - base power for armour items (for halving acid damage)
		 * - power per point of damage
		 * - power per point of +to_hit
		 * - power per point of base AC
		 * - power per point of +to_ac
		 * (these four are all halved in the algorithm)
		 * - assumed max blows
		 * - inhibiting values for +blows/might/shots/immunities (max is one less)
		 */
		const int NONWEAP_DAMAGE   	=	15; /* fudge to boost extra blows */
		const int WEAP_DAMAGE			=	12; /* and for off-weapon combat flags */
		const int BASE_JEWELRY_POWER	=	 4;
		const int BASE_ARMOUR_POWER		=	1;
		const int BASE_LIGHT_POWER		=	6; /* for rad-2; doubled for rad-3 */
		const int DAMAGE_POWER			=	5; /* i.e. 2.5 */
		const int TO_HIT_POWER			=	3; /* i.e. 1.5 */
		const int BASE_AC_POWER			=	 2; /* i.e. 1 */
		const int TO_AC_POWER         =     2; /* i.e. 1 */
		const int MAX_BLOWS           =     5;
		const int INHIBIT_IMMUNITIES   =    4;

		/**
		 * Define a set of constants for dealing with launchers and ammo:
		 * - the assumed average damage of ammo (for rating launchers)
		 * (the current values assume normal (non-seeker) ammo enchanted to +9)
		 * - the assumed bonus on launchers (for rating ego ammo)
		 * - twice the assumed multiplier (for rating any ammo)
		 * N.B. Ammo tvals are assumed to be consecutive! We access this array using
		 * (o_ptr.tval - TV_SHOT) for ammo, and 
		 * (o_ptr.sval / 10) for launchers
		 */
		class archery_t {
			public archery_t(int a, int b, int c, int d ){
				ammo_tval = a;
				ammo_dam = b;
				launch_dam = c;
				launch_mult = d;
			}

			public int ammo_tval;
			public int ammo_dam;
			public int launch_dam;
			public int launch_mult;
		} 
		
		static archery_t[] archery = new archery_t[]{
			 new archery_t(TVal.TV_SHOT, 10, 9, 4),
			 new archery_t(TVal.TV_ARROW, 12, 9, 5),
			 new archery_t(TVal.TV_BOLT, 14, 9, 7)
		};

		/**
		 * Set the weightings of flag types:
		 * - factor for power increment for multiple flags
		 * - additional power bonus for a "full set" of these flags
		 * - number of these flags which constitute a "full set"
		 * - whether value is damage-dependent
		 */
		class set {
			public set (Object_Flag.object_flag_type a, int b, int c, int d, bool e, int f, string g){
				type = (int)a;
				factor = b;
				bonus = c;
				size = d;
				dam_dep = e;
				count = f;
				desc = g;
			}
			public int type;
			public int factor;
			public int bonus;
			public int size;
			public bool dam_dep;
			public int count;
			public string desc;
		} 
		static set [] sets = new set[]{
			new set( Object_Flag.object_flag_type.SUST, 1, 10, 5, false, 0, "sustains" ),
			new set(Object_Flag.object_flag_type.SLAY, 1, 10, 8, true,  0, "normal slays" ),
			new set( Object_Flag.object_flag_type.BRAND, 2, 20, 5, true,  0, "brands" ),
			new set( Object_Flag.object_flag_type.KILL, 3, 20, 3, true,  0, "x5 slays" ),
			new set(Object_Flag.object_flag_type.IMM,  6, INHIBIT_POWER, 4, false, 0, "immunities" ),
			new set( Object_Flag.object_flag_type.LRES, 1, 10, 4, false, 0, "low resists" ),
			new set( Object_Flag.object_flag_type.HRES, 2, 10, 9, false, 0, "high resists" ),
			new set( Object_Flag.object_flag_type.PROT, 3, 15, 4, false, 0, "protections" ),
			new set( Object_Flag.object_flag_type.MISC, 1, 25, 8, false, 0, "misc abilities" )
		};

		/**
		 * Boost ratings for combinations of ability bonuses
		 * We go up to +24 here - anything higher is inhibited
		 * N.B. Not all stats count equally towards this total
		 */
		static short[] ability_power = new short[25]
			{0, 0, 0, 0, 0, 0, 0, 2, 4, 6, 8,
			12, 16, 20, 24, 30, 36, 42, 48, 56, 64,
			74, 84, 96, 110};

		/**
		 * Calculate the rating for a given slay combination
		 */
		int slay_power(bool verbose, StreamWriter log_file, bool known)
		{
			Bitflag s_index = new Bitflag(Object_Flag.SIZE);
			Bitflag f = new Bitflag(Object_Flag.SIZE);
			Bitflag f2 = new Bitflag(Object_Flag.SIZE);
			int sv = 0; //uint
			int i, j;
			int mult;
			Slay best_s_ptr = null;
			Monster_Race r_ptr;
			Monster.Monster m_ptr;
			//monster_type monster_type_body;
			string[] desc = new string[Slay.MAX.value];// = { 0 }, *
			string[] brand = new string[Slay.MAX.value];// = { 0 };
			int[] s_mult = new int[Slay.MAX.value];// = { 0 };

			if (known)
			    object_flags(ref f);
			else
			    object_flags_known(ref f);

			/* Combine the slay bytes into an index value, return if there are none */
			s_index.copy(f);
			Object_Flag.create_mask(f2, false, Object_Flag.object_flag_type.SLAY, 
				Object_Flag.object_flag_type.KILL, Object_Flag.object_flag_type.BRAND);

			if (!s_index.is_inter(f2))
			    return Eval.tot_mon_power;
			else
			    s_index.inter(f2);

			throw new NotImplementedException();

			///* Look in the cache to see if we know this one yet */
			//sv = check_slay_cache(s_index);

			///* If it's cached (or there are no slays), return the value */
			//if (sv != 0)	{
			//    file_putf(log_file, "Slay cache hit\n");
			//    return sv;
			//}

			///*
			// * Otherwise we need to calculate the expected average multiplier
			// * for this combination (multiplied by the total number of
			// * monsters, which we'll divide out later).
			// */
			//for (i = 0; i < z_info.r_max; i++)	{
			//    best_s_ptr = null;
			//    mult = 1;
			//    r_ptr = &r_info[i];
			//    m_ptr = &monster_type_body;
			//    m_ptr.r_idx = i;

			//    /* Find the best multiplier against this monster */
			//    improve_attack_modifier((object_type *)o_ptr, m_ptr, &best_s_ptr,
			//            false, !known);
			//    if (best_s_ptr)
			//        mult = best_s_ptr.mult;

			//    /* Add the multiple to sv */
			//    sv += mult * r_ptr.scaled_power;
			//}

			///*
			// * To get the expected damage for this weapon, multiply the
			// * average damage from base dice by sv, and divide by the
			// * total number of monsters.
			// */
			//if (verbose) {
			//    /* Write info about the slay combination and multiplier */
			//    file_putf(log_file, "Slay multiplier for: ");

			//    j = list_slays(s_index, s_index, desc, brand, s_mult, false);

			//    for (i = 0; i < j; i++) {
			//        if (brand[i]) {
			//            file_putf(log_file, brand[i]);
			//        } else {
			//            file_putf(log_file, desc[i]);
			//        }
			//        file_putf(log_file, "x%d ", s_mult[i]); 
			//    }
			//    file_putf(log_file, "\nsv is: %d\n", sv);
			//    file_putf(log_file, " and t_m_p is: %d \n", tot_mon_power);
			//    file_putf(log_file, "times 1000 is: %d\n", (1000 * sv) / tot_mon_power);
			//}

			///* Add to the cache */
			//if (fill_slay_cache(s_index, sv))
			//    file_putf(log_file, "Added to slay cache\n");

			//return sv;
		}

		/*
		 * Calculate the multiplier we'll get with a given bow type.
		 * Note that this relies on the multiplier being the 2nd digit of the bow's
		 * sval. We assume that sval has already been checked for legitimacy before
		 * we get here.
		 */
		static int bow_multiplier(int sval)
		{
			int mult = 0;
			mult = sval - 10 * (sval / 10);
			return mult;
		}

		/*
		 * Evaluate the object's overall power level.
		 */
		public int object_power(bool verbose, StreamWriter log_file,	bool known)
		{
			int p = 0, q = 0, slay_pwr = 0, dice_pwr = 0;
			int i, j;
			int extra_stat_bonus = 0, mult = 1, num_slays = 0, k = 1;
			Bitflag flags = new Bitflag(Object_Flag.SIZE);
			Bitflag mask = new Bitflag(Object_Flag.SIZE);

			/* Zero the flag counts */
			for (i = 0; i < sets.Length; i++)
			    sets[i].count = 0;

			/* Extract the flags */
			if (known) {
			//    log_file.Write("Object is deemed knwon\n");
			    object_flags(ref flags);
			} else {
			//    log_file.Write("Object may not be fully known\n");
			    object_flags_known(ref flags);
			}

			/* Log the flags in human-readable form */
			//if (verbose)
			    //log_flags(flags, log_file); //meh

			/* Get the slay power and number of slay/brand types */
			Object_Flag.create_mask(mask, false, Object_Flag.object_flag_type.SLAY, Object_Flag.object_flag_type.KILL, 
			    Object_Flag.object_flag_type.BRAND);
			num_slays = Slay.list_slays(flags, mask, null, null, null, true);
			if (num_slays != 0)
			    slay_pwr = slay_power(verbose, log_file, known);

			/* Start with any damage boost from the item itself */
			p += (to_d * DAMAGE_POWER / 2);
			//file_putf(log_file, "Adding power from to_dam, total is %d\n", p);

			/* Add damage from dice for any wieldable weapon or ammo */
			if (wield_slot() == Misc.INVEN_WIELD || is_ammo()) {
			    dice_pwr = (dd * (ds + 1) * DAMAGE_POWER / 4);
			//    file_putf(log_file, "Adding %d power for dam dice\n", dice_pwr);
			/* Add 2nd lot of damage power for nonweapons */
			} else if (wield_slot() != Misc.INVEN_BOW) {
			    p += (to_d * DAMAGE_POWER);
			//    file_putf(log_file, "Adding power from nonweap to_dam, total is %d\n", p);
			    /* Add power boost for nonweapons with combat flags */
			    if (num_slays != 0 || flags.has(Object_Flag.BLOWS.value) || flags.has(Object_Flag.SHOTS.value) || flags.has(Object_Flag.MIGHT.value)) {
			        dice_pwr = (WEAP_DAMAGE * DAMAGE_POWER);
			//        file_putf(log_file, "Adding %d power for nonweap combat flags\n", dice_pwr);
			    }
			}
			p += dice_pwr;

			/* Add ammo damage for launchers, get multiplier and rescale */
			if (wield_slot() == Misc.INVEN_BOW) {
			    p += (archery[sval / 10].ammo_dam * DAMAGE_POWER / 2);
			//    file_putf(log_file, "Adding power from ammo, total is %d\n", p);

			    mult = bow_multiplier(sval);
			//    file_putf(log_file, "Base mult for this weapon is %d\n", mult);
			}

			/* Add launcher bonus for ego ammo, multiply for launcher and rescale */
			if (is_ammo()) {
			    if (ego != null)
			        p += (archery[tval - TVal.TV_SHOT].launch_dam * DAMAGE_POWER / 2);
			    p = p * archery[tval - TVal.TV_SHOT].launch_mult / (2 * MAX_BLOWS);
			//    file_putf(log_file, "After multiplying ammo and rescaling, power is %d\n", p);
			}

			/* Add power for extra blows */
			if (flags.has(Object_Flag.BLOWS.value)) {
			    j = which_pval(Object_Flag.BLOWS.value);
			    if (known || this_pval_is_visible(j)) {
			        if (pval[j] >= INHIBIT_BLOWS) {
			            p += INHIBIT_POWER;
			//            file_putf(log_file, "INHIBITING - too many extra blows - quitting\n");
			            return p;
			        } else {
			            p = p * (MAX_BLOWS + pval[j]) / MAX_BLOWS;
			            /* Add boost for assumed off-weapon damage */
			            p += (NONWEAP_DAMAGE * pval[j] * DAMAGE_POWER / 2);
			//            file_putf(log_file, "Adding power for extra blows, total is %d\n", p);
			        }
			    }
			}

			/* Add power for extra shots - note that we cannot handle negative shots */
			if (flags.has(Object_Flag.SHOTS.value)) {
			    j = which_pval(Object_Flag.SHOTS.value);
			    if (known || this_pval_is_visible(j)) {
			        if (pval[j] >= INHIBIT_SHOTS) {
			            p += INHIBIT_POWER;
			//            file_putf(log_file, "INHIBITING - too many extra shots - quitting\n");
			            return p;
			        } else if (pval[j] > 0) {
			            p = (p * (1 + pval[j]));
			//            file_putf(log_file, "Extra shots: multiplying power by 1 + %d, total is %d\n", o_ptr.pval[j], p);
			        }
			    }
			}

			/* Add power for extra might */
			if (flags.has(Object_Flag.MIGHT.value)) {
			    j = which_pval(Object_Flag.MIGHT.value);
			    if (known || this_pval_is_visible(j)) {
			        if (pval[j] >= INHIBIT_MIGHT) {
			            p += INHIBIT_POWER;
			            mult = 1;	/* don't overflow */
			//            file_putf(log_file, "INHIBITING - too much extra might - quitting\n");
			            return p;
			        } else
			            mult += pval[j];
			//        file_putf(log_file, "Mult after extra might is %d\n", mult);
			    }
			}
			p *= mult;
			//file_putf(log_file, "After multiplying power for might, total is %d\n", p);

			/* Apply the correct slay multiplier */
			if (slay_pwr != 0) {
			    p += (dice_pwr * (slay_pwr / 100)) / (Eval.tot_mon_power / 100);
			//    file_putf(log_file, "Adjusted for slay power, total is %d\n", p);
			}

			/* Melee weapons assume MAX_BLOWS per turn, so we must divide by MAX_BLOWS
			 * to get equal ratings for launchers. */
			if (wield_slot() == Misc.INVEN_BOW) {
			    p /= MAX_BLOWS;
			//    file_putf(log_file, "Rescaling bow power, total is %d\n", p);
			}

			/* Add power for +to_hit */
			p += (to_h * TO_HIT_POWER / 2);
			//file_putf(log_file, "Adding power for to hit, total is %d\n", p);

			/* Add power for base AC and adjust for weight */
			if (ac != 0) {
			    p += BASE_ARMOUR_POWER;
			    q += (ac * BASE_AC_POWER / 2);
			//    file_putf(log_file, "Adding %d power for base AC value\n", q);

			    /* Add power for AC per unit weight */
			    if (weight > 0) {
			        i = 750 * (ac + to_a) / weight;

			        /* Avoid overpricing Elven Cloaks */
			        if (i > 450) i = 450;

			        q *= i;
			        q /= 100;

			    /* Weightless (ethereal) armour items get fixed boost */
			    } else
			        q *= 5;
			    p += q;
			//    file_putf(log_file, "Adding power for AC per unit weight, now %d\n", p);
			}
			/* Add power for +to_ac */
			p += (to_a * TO_AC_POWER / 2);
			//file_putf(log_file, "Adding power for to_ac of %d, total is %d\n", o_ptr.to_a, p);
			if (to_a > HIGH_TO_AC) {
			    p += ((to_a - (HIGH_TO_AC - 1)) * TO_AC_POWER);
			//    file_putf(log_file, "Adding power for high to_ac value, total is %d\n", p);
			}
			if (to_a > VERYHIGH_TO_AC) {
			    p += ((to_a - (VERYHIGH_TO_AC -1)) * TO_AC_POWER * 2);
			//    file_putf(log_file, "Adding power for very high to_ac value, total is %d\n", p);
			}
			if (to_a >= INHIBIT_AC) {
			    p += INHIBIT_POWER;
			//    file_putf(log_file, "INHIBITING: AC bonus too high\n");
			}

			/* Add power for light sources by radius XXX Hack - rewrite calc_torch! */
			if (wield_slot() == Misc.INVEN_LIGHT) {
			    p += BASE_LIGHT_POWER;

			    /* Artifact lights have larger radius so add more */
			    if (artifact != null)
			        p += BASE_LIGHT_POWER;

			//    file_putf(log_file, "Adding power for light radius, total is %d\n", p);
			}

			/* Add base power for jewelry */
			if (is_jewelry()) {
			    p += BASE_JEWELRY_POWER;
			//    file_putf(log_file, "Adding power for jewelry, total is %d\n", p);
			}

			/* Add power for non-derived flags (derived flags have flag_power 0) */
			for (i = flags.next(Bitflag.FLAG_START); i != Bitflag.FLAG_null; i = flags.next(i + 1)) {
			    if (Object_Flag.flag_uses_pval(i)) {
			        j = which_pval(i);
			        if (known || this_pval_is_visible(j)) {
			            k = pval[j];
			            extra_stat_bonus += (k * Object_Flag.pval_mult(i));
			        }
			    } else
			        k = 1;

			    if (Object_Flag.flag_power(i) != 0) {
			        p += (k * Object_Flag.flag_power(i) * Object_Flag.slot_mult(i, wield_slot()));
			        //file_putf(log_file, "Adding power for %s, total is %d\n", flag_name(i), p);
			    }

			    /* Track combinations of flag types - note we ignore SUST_CHR */
			    for (j = 0; j < sets.Length; j++)
			        if ((sets[j].type == Object_Flag.obj_flag_type(i)) && (i != Object_Flag.SUST_CHR.value))
			            sets[j].count++;
			}

			/* Add extra power term if there are a lot of ability bonuses */
			if (extra_stat_bonus > 249) {
			//    file_putf(log_file, "Inhibiting!  (Total ability bonus of %d is too high)\n", extra_stat_bonus);
			    p += INHIBIT_POWER;
			} else {
			    p += ability_power[extra_stat_bonus / 10];
			//    file_putf(log_file, "Adding power for pval total of %d, total is %d\n", extra_stat_bonus, p);
			}

			/* Add extra power for multiple flags of the same type */
			for (i = 0; i < sets.Length; i++) {
			    if (sets[i].count > 1) {
			        q = (sets[i].factor * sets[i].count * sets[i].count);
			        /* Scale damage-dependent set bonuses by damage dice power */
			        if (sets[i].dam_dep)
			            q = q * dice_pwr / (DAMAGE_POWER * 5);
			        p += q;
			//        file_putf(log_file, "Adding power for multiple %s, total is %d\n", sets[i].desc, p);
			    }

			    /* Add bonus if item has a full set of these flags */
			    if (sets[i].count == sets[i].size) {
			        p += sets[i].bonus;
			//        file_putf(log_file, "Adding power for full set of %s, total is %d\n", sets[i].desc, p);
			    }
			}

			/* add power for effect */
			if (known || effect_is_known())	{
			    if (artifact != null && artifact.effect != null) {
			        p += artifact.effect.effect_power();
			//        file_putf(log_file, "Adding power for artifact activation, total is %d\n", p);
			    } else {
			        p += kind.effect.effect_power();
			//        file_putf(log_file, "Adding power for item activation, total is %d\n", p);
			    }
			}

			//file_putf(log_file, "FINAL POWER IS %d\n", p);

			return p;
		}
	}
}
