using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Monster;

namespace CSAngband.Object {
	//This is going to be one of those big lists...
	class Slay {
		//aka slay_table
		public static List<Slay> list = new List<Slay>();
		private static int counter = 0;

		public Slay(string name, Object_Flag object_flag, Monster_Flag monster_flag, Monster_Flag resist_flag,
					int mult, string range_verb, string melee_verb, string active_verb, string desc, string brand){
			this.value = counter++;

			this.name = name;
			this.object_flag = object_flag;
			this.monster_flag = monster_flag;
			this.resist_flag = resist_flag;
			this.mult = mult;
			this.range_verb = range_verb;
			this.melee_verb = melee_verb;
			this.active_verb = active_verb;
			this.desc = desc;
			this.brand = brand;
		}

		int value;					/* Numerical index */

		string name;
		Object_Flag object_flag;			/* Object flag for the slay */
		Monster_Flag monster_flag;			/* Which monster flag(s) make it vulnerable */
		Monster_Flag resist_flag;			/* Which monster flag(s) make it resist */
		public int mult;					/* Slay multiplier */
		string range_verb;		/* attack verb for ranged hits */
		public string melee_verb; 	/* attack verb for melee hits */
		string active_verb; 	/* verb for when the object is active */
		string desc;			/* description of vulnerable creatures */
		string brand;			/* name of brand */

		/*
		 * Entries in this table should be in ascending order of multiplier, to
		 * ensure that the highest one takes precedence. Structure is name,
		 * object flag, vulnerable flag, monster resist flag, multiplier, ranged verb,
		 * melee verb, verb describing what the thing does when it is active,
		 * description of affected creatures, brand
		 */

		public static Slay XXX =	new Slay("XXX" , 		Object_Flag.NONE,		 Monster_Flag.NONE,   Monster_Flag.NONE,	0,	null,	    		null,      			null,         		null, 										null);
		public static Slay ANIMAL2 =new Slay("ANIMAL2" ,	Object_Flag.SLAY_ANIMAL, Monster_Flag.ANIMAL, Monster_Flag.NONE,	2,	"pierces",  		"smite",   			"glows",      		"animals", 									null);
		public static Slay EVIL2 =	new Slay("EVIL2" ,		Object_Flag.SLAY_EVIL,   Monster_Flag.EVIL,   Monster_Flag.NONE,	2,	"pierces",  		"smite",   			"glows",      		"evil creatures", 							null);
		public static Slay ACID2 =	new Slay("ACID2" ,		Object_Flag.BRAND_FIZZ,  Monster_Flag.NONE,   Monster_Flag.IM_ACID,	2,	"corrodes",			"corrode", 			"fizzes",   		"creatures not resistant to acid", 			"weak acid");
		public static Slay ELEC2 =	new Slay("ELEC2" ,		Object_Flag.BRAND_BUZZ,  Monster_Flag.NONE,   Monster_Flag.IM_ELEC,	2,	"zaps",     		"zap",     			"buzzes",			"creatures not resistant to electricity", 	"weak lightning");
		public static Slay FIRE2 =	new Slay("FIRE2" ,		Object_Flag.BRAND_WARM,  Monster_Flag.NONE,   Monster_Flag.IM_FIRE,	2,	"singes",			"singe",			"grows warm", 		"creatures not resistant to fire", 			"weak flames");
		public static Slay COLD2 =	new Slay("COLD2" ,		Object_Flag.BRAND_COOL,  Monster_Flag.NONE,   Monster_Flag.IM_COLD,	2,	"chills" , 			"chill",  			"grows cool", 		"creatures not resistant to cold", 			"weak frost");
		public static Slay POISON2 =new Slay("POISON2" ,	Object_Flag.BRAND_ICKY,  Monster_Flag.NONE,   Monster_Flag.IM_POIS,	2,	"sickens", 			"sicken",  			"glows green",  	"creatures not resistant to poison", 		"weak venom");
		public static Slay UNDEAD3 =new Slay("UNDEAD3" ,	Object_Flag.SLAY_UNDEAD, Monster_Flag.UNDEAD, Monster_Flag.NONE,	3,	"pierces",  		"smite",   			"glows",      		"undead", 									null);
		public static Slay DEMON3 = new Slay("DEMON3" ,		Object_Flag.SLAY_DEMON,  Monster_Flag.DEMON,  Monster_Flag.NONE,	3,	"pierces",  		"smite",   			"glows",      		"demons", 									null);
		public static Slay ORC3 =	new Slay("ORC3" ,		Object_Flag.SLAY_ORC,    Monster_Flag.ORC,    Monster_Flag.NONE,	3,	"pierces",  		"smite",   			"glows",      		"orcs", 									null);
		public static Slay TROLL3 = new Slay("TROLL3" ,		Object_Flag.SLAY_TROLL,  Monster_Flag.TROLL,  Monster_Flag.NONE,	3,	"pierces",  		"smite",   			"glows",      		"trolls", 									null);
		public static Slay GIANT3 = new Slay("GIANT3" ,		Object_Flag.SLAY_GIANT,  Monster_Flag.GIANT,  Monster_Flag.NONE,	3,	"pierces",  		"smite",   			"glows",      		"giants", 									null);
		public static Slay DRAGON3 =new Slay("DRAGON3" ,	Object_Flag.SLAY_DRAGON, Monster_Flag.DRAGON, Monster_Flag.NONE,	3,	"pierces",  		"smite",   			"glows",      		"dragons", 									null);
		public static Slay ACID3 =	new Slay("ACID3" ,		Object_Flag.BRAND_ACID,  Monster_Flag.NONE,   Monster_Flag.IM_ACID,	3,	"dissolves", 		"dissolve", 		"spits",      		"creatures not resistant to acid", 			"acid");
		public static Slay ELEC3 =	new Slay("ELEC3" ,		Object_Flag.BRAND_ELEC,  Monster_Flag.NONE,   Monster_Flag.IM_ELEC,	3,	"shocks",     		"shock",     		"crackles",   		"creatures not resistant to electricity", 	"lightning");
		public static Slay FIRE3 =	new Slay("FIRE3" ,		Object_Flag.BRAND_FIRE,  Monster_Flag.NONE,   Monster_Flag.IM_FIRE,	3,	"burns",    		"burn",    			"flares",     		"creatures not resistant to fire", 			"flames");
		public static Slay COLD3 =	new Slay("COLD3" ,		Object_Flag.BRAND_COLD,  Monster_Flag.NONE,   Monster_Flag.IM_COLD,	3,	"freezes" , 		"freeze",  			"grows cold", 		"creatures not resistant to cold", 			"frost");
		public static Slay POISON3 =new Slay("POISON3" ,	Object_Flag.BRAND_POIS,  Monster_Flag.NONE,   Monster_Flag.IM_POIS,	3,	"poisons",  		"poison",  			"seethes",    		"creatures not resistant to poison", 		"venom");
		public static Slay DRAGON5 =new Slay("DRAGON5" , 	Object_Flag.KILL_DRAGON, Monster_Flag.DRAGON, Monster_Flag.NONE,	5,	"deeply pierces", 	"fiercely smite", 	"glows brightly", 	"dragons", 									null);
		public static Slay DEMON5 = new Slay("DEMON5" , 	Object_Flag.KILL_DEMON,  Monster_Flag.DEMON,  Monster_Flag.NONE,	5,	"deeply pierces", 	"fiercely smite", 	"glows brightly", 	"demons", 									null);
		public static Slay UNDEAD5 =new Slay("UNDEAD5" , 	Object_Flag.KILL_UNDEAD, Monster_Flag.UNDEAD, Monster_Flag.NONE,	5,	"deeply pierces", 	"fiercely smite", 	"glows brightly", 	"undead", 									null);


		/*
		 * Slay cache. Used for looking up slay values in obj-power.c
		 */
		class flag_cache {
				public Bitflag flags = new Bitflag(Object_Flag.SIZE);   	/* Combination of slays and brands */
				public int value;            		/* Value of this combination */
		}

		/**
		 * Cache of slay values (for object_power)
		 */
		//was a pointer, is probably an array
		static flag_cache[] slay_cache;

		/**
		 * Create a cache of slay combinations found on ego items, and the values of
		 * these combinations. This is to speed up slay_power(), which will be called
		 * many times for ego items during the game.
		 *
		 * \param items is the set of ego types from which we are extracting slay
		 * combinations
		 */
		public static int create_slay_cache(Ego_Item[] items)
		{
			int count = 0;
			Bitflag cacheme = new Bitflag(Object_Flag.SIZE);
			Bitflag slay_mask = new Bitflag(Object_Flag.SIZE);

			/* Build the slay mask */
			Object_Flag.create_mask(slay_mask, false, Object_Flag.object_flag_type.SLAY, 
				Object_Flag.object_flag_type.KILL, Object_Flag.object_flag_type.BRAND);

			/* Calculate necessary size of slay_cache */
			Bitflag[] dupcheck = new Bitflag[Misc.z_info.e_max];

			for (int i = 0; i < Misc.z_info.e_max; i++) {
			    dupcheck[i] = new Bitflag(Object_Flag.SIZE);
			    Ego_Item e_ptr = items[i];

				//Some items are null... I guess we should just skip them...?
				//TODO: Find out why they are null, and see if we actually should just skip them...
				if(e_ptr == null) {
					continue;
				}

			    /* Find the slay flags on this ego */
			    cacheme.copy(e_ptr.flags);
				cacheme.inter(slay_mask);

			    /* Only consider non-empty combinations of slay flags */
			    if (!cacheme.is_empty()) {
			        /* Skip previously scanned combinations */
			        for (int j = 0; j < i; j++)
			            if (cacheme.is_equal(dupcheck[j])) continue;

			        /* msg("Found a new slay combo on an ego item"); */
			        count++;
			        dupcheck[i].copy(cacheme);
			    }
			}

			/* Allocate slay_cache with an extra empty element for an iteration stop */
			slay_cache = new flag_cache[count + 1];
			count = 0;

			/* Populate the slay_cache */
			for (int i = 0; i < Misc.z_info.e_max; i++) {
			    if (!dupcheck[i].is_empty()) {
					slay_cache[count] = new flag_cache();
					slay_cache[count].flags.copy(dupcheck[i]);
			        slay_cache[count].value = 0;
			        count++;
			        /*msg("Cached a slay combination");*/
			    }
			}

			//From a time without garbage collection...
			/*for (i = 0; i < z_info.e_max; i++)
			    FREE(dupcheck[i]);
			FREE(dupcheck);*/

			/* Success */
			return 0;
		}


		/**
		 * Remove slays which are duplicates, i.e. they have exactly the same "monster
		 * flag" and the same "resist flag". The one with highest multiplier is kept.
		 *
		 * \param flags is the flagset from which to remove duplicates.
		 * count is the number of dups removed.
		 */
		public static int dedup_slays(ref Bitflag flags) {
			int i, j;
			int count = 0;

			for (i = 0; i < list.Count(); i++) {
			    Slay s_ptr = list[i];
			    if (flags.has(s_ptr.object_flag.value)) {
			        for (j = i + 1; j < list.Count(); j++) {
			            Slay t_ptr = list[j];
			            if (flags.has(t_ptr.object_flag.value) &&
			                    (t_ptr.monster_flag == s_ptr.monster_flag) &&
			                    (t_ptr.resist_flag == s_ptr.resist_flag) &&
			                    (t_ptr.mult != s_ptr.mult)) {
			                count++;
			                if (t_ptr.mult > s_ptr.mult)
			                    flags.off(s_ptr.object_flag.value);
			                else
			                    flags.off(t_ptr.object_flag.value);
			            }
			        }
			    }
			}

			return count;
		}


		/**
		 * Get a random slay (or brand).
		 * We use randint1 because the first entry in slay_table is null.
		 *
		 * \param mask is the set of slays from which we are choosing.
		 */
		//mask.size = Object_Flag.SIZE
		public static Slay random_slay(Bitflag mask)
		{
			throw new NotImplementedException();
			//const struct slay *s_ptr;
			//do {
			//    s_ptr = &slay_table[randint1(SL_MAX - 1)];
			//} while (!Object_Flag.has(mask, s_ptr.object_flag));

			//return s_ptr;
		}


		/**
		 * Match slays in flags against a chosen flag mask
		 *
		 * count is the number of matches
		 * \param flags is the flagset to analyse for matches
		 * \param mask is the flagset against which to test
		 * \param desc[] is the array of descriptions of matching slays - can be null
		 * \param brand[] is the array of descriptions of brands - can be null
		 * \param mult[] is the array of multipliers of those slays - can be null
		 * \param dedup is whether or not to remove duplicates
		 *
		 * desc[], brand[] and mult[] must be >= SL_MAX in size
		 */
		//Bitflags were size Object_Flag.SIZE, might be an out param
		public static int list_slays(Bitflag flags, Bitflag mask, string[] desc, string[] brand,int[] mult, bool dedup)
		{
			int i, count = 0;
			Bitflag f = new Bitflag(Object_Flag.SIZE);

			/* We are only interested in the flags specified in mask */
			f.copy(flags);
			f.inter(mask);

			/* Remove "duplicate" flags if desired */
			if (dedup) dedup_slays(ref f);

			/* Collect slays */
			for (i = 0; i < list.Count(); i++) {
			    Slay s_ptr = list[i];
			    if (f.has(s_ptr.object_flag.value)) {
			        if (mult != null)
			            mult[count] = s_ptr.mult;
			        if (brand != null)
			            brand[count] = s_ptr.brand;
			        if (desc != null)
			            desc[count] = s_ptr.desc;
			        count++;
			    }
			}

			return count;
		}


		/**
		 * Notice any slays on a particular object which are in mask.
		 *
		 * \param o_ptr is the object on which we are noticing slays
		 * \param mask is the flagset within which we are noticing them
		 */
		//mask was size: Object_Flag.SIZE. It might not even be an array, but just a bitflag with that size...
		//It might end up being an out parameter...
		public static void object_notice_slays(Object o_ptr, Bitflag[] mask)
		{
			throw new NotImplementedException();
			//bool learned;
			//bitflag f[Object_Flag.SIZE];
			//char o_name[40];
			//int i;

			///* We are only interested in the flags specified in mask */
			//object_flags(o_ptr, f);
			//Object_Flag.inter(f, mask);

			///* if you learn a slay, learn the ego and print a message */
			//for (i = 0; i < SL_MAX; i++) {
			//    const struct slay *s_ptr = &slay_table[i];
			//    if (Object_Flag.has(f, s_ptr.object_flag)) {
			//        learned = object_notice_flag(o_ptr, s_ptr.object_flag);
			//        if (EASY_LEARN && learned) {
			//            object_notice_ego(o_ptr);
			//            object_desc(o_name, sizeof(o_name), o_ptr, ODESC_BASE);
			//            msg("Your %s %s!", o_name, s_ptr.active_verb);
			//        }
			//    }
			//}

			//object_check_for_ident(o_ptr);
		}


		/**
		 * Extract the multiplier from a given object hitting a given monster.
		 *
		 * \param o_ptr is the object being used to attack
		 * \param m_ptr is the monster being attacked
		 * \param best_s_ptr is the best applicable slay_table entry, or null if no
		 *  slay already known
		 * \param real is whether this is a real attack (where we update lore) or a
		 *  simulation (where we don't)
		 * \param known_only is whether we are using all the object flags, or only
		 * the ones we *already* know about
		 */
		//Best_s_ptr was slay**
		public static void improve_attack_modifier(Object o_ptr, Monster.Monster m_ptr, 
			ref Slay best_s_ptr, bool real, bool known_only)
		{
			throw new NotImplementedException();
			//monster_race *r_ptr = &r_info[m_ptr.r_idx];
			//monster_lore *l_ptr = &l_list[m_ptr.r_idx];
			//bitflag f[Object_Flag.SIZE], known_f[Object_Flag.SIZE], note_f[Object_Flag.SIZE];
			//int i;

			//object_flags(o_ptr, f);
			//object_flags_known(o_ptr, known_f);

			//for (i = 0; i < SL_MAX; i++) {
			//    const struct slay *s_ptr = &slay_table[i];
			//    if ((known_only && !Object_Flag.has(known_f, s_ptr.object_flag)) ||
			//            (!known_only && !Object_Flag.has(f, s_ptr.object_flag))) continue;

			//    /* In a real attack, learn about monster resistance or slay match if:
			//     * EITHER the slay flag on the object is known,
			//     * OR the monster is vulnerable to the slay/brand
			//     */
			//    if (real && (Object_Flag.has(known_f, s_ptr.object_flag) || (s_ptr.monster_flag
			//            && Monster_Flag.has(r_ptr.flags,	s_ptr.monster_flag)) ||
			//            (s_ptr.resist_flag && !Monster_Flag.has(r_ptr.flags,
			//            s_ptr.resist_flag)))) {

			//        /* notice any brand or slay that would affect monster */
			//        Object_Flag.wipe(note_f);
			//        Object_Flag.on(note_f, s_ptr.object_flag);
			//        object_notice_slays(o_ptr, note_f);

			//        if (m_ptr.ml && s_ptr.monster_flag)
			//            Monster_Flag.on(l_ptr.flags, s_ptr.monster_flag);

			//        if (m_ptr.ml && s_ptr.resist_flag)
			//            Monster_Flag.on(l_ptr.flags, s_ptr.resist_flag);
			//    }

			//    /* If the monster doesn't resist or the slay flag matches */
			//    if ((s_ptr.brand && !Monster_Flag.has(r_ptr.flags, s_ptr.resist_flag)) ||
			//            (s_ptr.monster_flag && Monster_Flag.has(r_ptr.flags,
			//            s_ptr.monster_flag))) {

			//        /* compare multipliers to determine best attack */
			//        if ((*best_s_ptr == null) || ((*best_s_ptr).mult < s_ptr.mult))
			//            *best_s_ptr = s_ptr;
			//    }
			//}
		}


		/**
		 * React to slays which hurt a monster
		 * 
		 * \param obj_flags is the set of flags we're testing for slays
		 * \param mon_flags is the set of flags we're adjusting as a result
		 */
		//both are probably arrays
		public static void react_to_slay(Bitflag obj_flags, Bitflag mon_flags)
		{
			throw new NotImplementedException();
			//int i;
			//for (i = 0; i < SL_MAX; i++) {
			//    const struct slay *s_ptr = &slay_table[i];
			//    if (Object_Flag.has(obj_flags, s_ptr.object_flag) && s_ptr.monster_flag)
			//        Monster_Flag.on(mon_flags, s_ptr.monster_flag);
			//}
		}


		/**
		 * Check the slay cache for a combination of slays and return a slay value
		 * 
		 * \param index is the set of slay flags to look for
		 */
		//index might be an array
		public static int check_slay_cache(Bitflag index)
		{
			throw new NotImplementedException();
			//int i;

			//for (i = 0; !Object_Flag.is_empty(slay_cache[i].flags); i++)
			//    if (Object_Flag.is_equal(index, slay_cache[i].flags)) break;

			//return slay_cache[i].value;
		}


		/**
		 * Fill in a value in the slay cache. Return true if a change is made.
		 *
		 * \param index is the set of slay flags whose value we are adding
		 * \param value is the value of the slay flags in index
		 */
		//index might be an array
		public static bool fill_slay_cache(Bitflag index, int value)
		{
			throw new NotImplementedException();
			//int i;

			//for (i = 0; !Object_Flag.is_empty(slay_cache[i].flags); i++) {
			//    if (Object_Flag.is_equal(index, slay_cache[i].flags)) {
			//        slay_cache[i].value = value;
			//        return true;
			//    }
			//}

			//return false;
		}

		public static void free_slay_cache()
		{
			throw new NotImplementedException();
			//mem_free(slay_cache);
		}
	}
}
