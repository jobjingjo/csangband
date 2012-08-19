using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;

namespace CSAngband {
	//TODO: Figure out what a "squelch" is and what this class "represents" about it.
	class Squelch {
		/*
		 * Used for mapping the values below to names.
		 */
		class quality_name
		{
			public quality_name(squelch_type_t e, string s){
				enum_val = (int)e;
				name = s;
			}

			public quality_name(quality_squelch e, string s){
				enum_val = (int) e;
				name = s;
			}

			public int enum_val;
			public string name;
		}

		/*
		 * List of kinds of item, for pseudo-id squelch.
		 */
		public enum squelch_type_t
		{
			TYPE_WEAPON_POINTY,
			TYPE_WEAPON_BLUNT,
			TYPE_SHOOTER,
			TYPE_MISSILE_SLING,
			TYPE_MISSILE_BOW,
			TYPE_MISSILE_XBOW,
			TYPE_ARMOR_ROBE,
			TYPE_ARMOR_BODY,
			TYPE_ARMOR_CLOAK,
			TYPE_ARMOR_ELVEN_CLOAK,
			TYPE_ARMOR_SHIELD,
			TYPE_ARMOR_HEAD,
			TYPE_ARMOR_HANDS,
			TYPE_ARMOR_FEET,
			TYPE_DIGGER,
			TYPE_RING,
			TYPE_AMULET,
			TYPE_LIGHT,

			TYPE_MAX
		}

		/*
		 * The different kinds of quality squelch
		 */
		enum quality_squelch
		{
			SQUELCH_NONE,
			SQUELCH_BAD,
			SQUELCH_AVERAGE,
			SQUELCH_GOOD,
			SQUELCH_EXCELLENT_NO_HI,
			SQUELCH_EXCELLENT_NO_SPL,
			SQUELCH_ALL,

			SQUELCH_MAX
		}


		/*
		 * Squelch flags
		 */
		const int SQUELCH_IF_AWARE =	0x01;
		const int SQUELCH_IF_UNAWARE =	0x02;

		/*
		 * Initialise the squelch package (currently just asserts). //LIES!!!
		 */
		public static void Init()
		{ //There was nothing here to beginwith...
		}

		class quality_squelc_struct
		{
			public quality_squelc_struct(squelch_type_t s, int t, int min, int max){
				squelch_type = s;
				tval = t;
				min_sval = min;
				max_sval = max;
			}

			public squelch_type_t squelch_type;
			public int tval;
			public int min_sval;
			public int max_sval;
		}

		static quality_squelc_struct[] quality_mapping = new quality_squelc_struct[]
		{
			new quality_squelc_struct( squelch_type_t.TYPE_WEAPON_POINTY,	TVal.TV_SWORD,		0,			SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_WEAPON_POINTY,	TVal.TV_POLEARM,	0,			SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_WEAPON_BLUNT,	TVal.TV_HAFTED,		0,			SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_SHOOTER,			TVal.TV_BOW,		0,			SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_MISSILE_SLING,	TVal.TV_SHOT,		0,			SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_MISSILE_BOW,		TVal.TV_ARROW,		0,			SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_MISSILE_XBOW,	TVal.TV_BOLT,		0,			SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_ARMOR_ROBE,		TVal.TV_SOFT_ARMOR,	
				(int)SVal.sval_soft_armour.SV_ROBE,(int)SVal.sval_soft_armour.SV_ROBE ),
			new quality_squelc_struct( squelch_type_t.TYPE_ARMOR_BODY,		TVal.TV_DRAG_ARMOR,	0,			SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_ARMOR_BODY,		TVal.TV_HARD_ARMOR,	0,			SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_ARMOR_BODY,		TVal.TV_SOFT_ARMOR,	0,			SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_ARMOR_CLOAK,		TVal.TV_CLOAK,		
				(int)SVal.sval_cloak.SV_CLOAK,	(int)SVal.sval_cloak.SV_FUR_CLOAK ),
			new quality_squelc_struct( squelch_type_t.TYPE_ARMOR_CLOAK,		TVal.TV_CLOAK,		
				(int)SVal.sval_cloak.SV_ETHEREAL_CLOAK, (int)SVal.sval_cloak.SV_ETHEREAL_CLOAK ),
		/* XXX Eddie need to assert SV_CLOAK < SV_FUR_CLOAK < SV_ELVEN_CLOAK */
			new quality_squelc_struct( squelch_type_t.TYPE_ARMOR_ELVEN_CLOAK, TVal.TV_CLOAK,	
				(int)SVal.sval_cloak.SV_ELVEN_CLOAK, (int)SVal.sval_cloak.SV_ELVEN_CLOAK ),
			new quality_squelc_struct( squelch_type_t.TYPE_ARMOR_SHIELD,	TVal.TV_SHIELD,		0,		SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_ARMOR_HEAD,		TVal.TV_HELM,		0,		SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_ARMOR_HEAD,		TVal.TV_CROWN,		0,		SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_ARMOR_HANDS,		TVal.TV_GLOVES,		0,		SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_ARMOR_FEET,		TVal.TV_BOOTS,		0,		SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_DIGGER,			TVal.TV_DIGGING,	0,		SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_RING,			TVal.TV_RING,		0,		SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_AMULET,			TVal.TV_AMULET,		0,		SVal.SV_UNKNOWN ),
			new quality_squelc_struct( squelch_type_t.TYPE_LIGHT, 			TVal.TV_LIGHT, 		0,		SVal.SV_UNKNOWN )
		};



		static quality_name[] quality_choices = new quality_name[(int)squelch_type_t.TYPE_MAX]
		{
			new quality_name( squelch_type_t.TYPE_WEAPON_POINTY,		"Pointy Melee Weapons" ),
			new quality_name( squelch_type_t.TYPE_WEAPON_BLUNT,			"Blunt Melee Weapons" ),
			new quality_name( squelch_type_t.TYPE_SHOOTER,				"Missile weapons" ),
			new quality_name( squelch_type_t.TYPE_MISSILE_SLING,		"Shots and Pebbles" ),
			new quality_name( squelch_type_t.TYPE_MISSILE_BOW,			"Arrows" ),
			new quality_name( squelch_type_t.TYPE_MISSILE_XBOW,			"Bolts" ),
			new quality_name( squelch_type_t.TYPE_ARMOR_ROBE,			"Robes" ),
			new quality_name( squelch_type_t.TYPE_ARMOR_BODY,			"Body Armor" ),
			new quality_name( squelch_type_t.TYPE_ARMOR_CLOAK,			"Cloaks" ),
			new quality_name( squelch_type_t.TYPE_ARMOR_ELVEN_CLOAK,	"Elven Cloaks" ),
			new quality_name( squelch_type_t.TYPE_ARMOR_SHIELD,			"Shields" ),
			new quality_name( squelch_type_t.TYPE_ARMOR_HEAD,			"Headgear" ),
			new quality_name( squelch_type_t.TYPE_ARMOR_HANDS,			"Handgear" ),
			new quality_name( squelch_type_t.TYPE_ARMOR_FEET,			"Footgear" ),
			new quality_name( squelch_type_t.TYPE_DIGGER,				"Diggers" ),
			new quality_name( squelch_type_t.TYPE_RING,					"Rings" ),
			new quality_name( squelch_type_t.TYPE_AMULET,				"Amulets" ),
			new quality_name( squelch_type_t.TYPE_LIGHT, 				"Lights" )
		};

		/*
		 * The names for the various kinds of quality
		 */
		static quality_name[] quality_values = new quality_name[]
		{
			new quality_name( quality_squelch.SQUELCH_NONE,				"no squelch" ),
			new quality_name( quality_squelch.SQUELCH_BAD,				"bad" ),
			new quality_name( quality_squelch.SQUELCH_AVERAGE,			"average" ),
			new quality_name( quality_squelch.SQUELCH_GOOD,				"good" ),
			new quality_name( quality_squelch.SQUELCH_EXCELLENT_NO_HI,	"excellent with no high resists" ),
			new quality_name( quality_squelch.SQUELCH_EXCELLENT_NO_SPL,	"excellent but not splendid" ),
			new quality_name( quality_squelch.SQUELCH_ALL,				"non-artifact" ),
		};

		static byte[] squelch_level = new byte[(int)squelch_type_t.TYPE_MAX];
		const int squelch_size = (int)squelch_type_t.TYPE_MAX;


		/*** Autoinscription stuff ***/

		public static string get_autoinscription(Object_Kind kind)
		{
			return (kind != null) && (kind.note != null) ? kind.note.ToString() : null;
		}

		/* Put the autoinscription on an object */
		public static int apply_autoinscription(Object.Object o_ptr)
		{
			string o_name = ""; //80
			string note = get_autoinscription(o_ptr.kind);

			/* Don't inscribe unaware objects */
			if (note != null || !o_ptr.flavor_is_aware())
			    return 0;

			/* Don't re-inscribe if it's already inscribed */
			if (o_ptr.note != null)
			    return 0;

			/* Get an object description */
			Object_Desc.object_desc(ref o_name, 80, o_ptr, Object_Desc.Detail.PREFIX | Object_Desc.Detail.FULL);

			if(note == null)
				o_ptr.note = Quark.Add(note);
			else
				o_ptr.note = null;

			Utilities.msg("You autoinscribe {0}.", o_name);

			return 1;
		}


		public static int remove_autoinscription(short kind)
		{
			throw new NotImplementedException();
			//struct object_kind *k = objkind_byid(kind);
			//if (!k || !k.note)
			//    return 0;
			//k.note = 0;
			//return 1;
		}


		public static int add_autoinscription(short kind, string inscription)
		{
			throw new NotImplementedException();
			//struct object_kind *k = objkind_byid(kind);
			//if (!k)
			//    return 0;
			//if (!inscription)
			//    return remove_autoinscription(kind);
			//k.note = quark_add(inscription);
			//return 1;
		}


		public static void autoinscribe_ground()
		{
			throw new NotImplementedException();
			//int py = p_ptr.py;
			//int px = p_ptr.px;
			//s16b this_o_idx, next_o_idx = 0;

			///* Scan the pile of objects */
			//for (this_o_idx = cave.o_idx[py][px]; this_o_idx; this_o_idx = next_o_idx)
			//{
			//    /* Get the next object */
			//    next_o_idx = object_byid(this_o_idx).next_o_idx;

			//    /* Apply an autoinscription */
			//    apply_autoinscription(object_byid(this_o_idx));
			//}
		}

		public static void autoinscribe_pack()
		{
			throw new NotImplementedException();
			//int i;

			///* Cycle through the inventory */
			//for (i = INVEN_PACK; i >= 0; i--)
			//{
			//    /* Skip empty items */
			//    if (!p_ptr.inventory[i].kind) continue;

			//    /* Apply the inscription */
			//    apply_autoinscription(&p_ptr.inventory[i]);
			//}

			//return;
		}




		/*** Squelch code ***/

		/*
		 * Squelch the flavor of an object
		 */
		public static void object_squelch_flavor_of(Object.Object o_ptr)
		{
			throw new NotImplementedException();
			//if (object_flavor_is_aware(o_ptr))
			//    o_ptr.kind.squelch |= SQUELCH_IF_AWARE;
			//else
			//    o_ptr.kind.squelch |= SQUELCH_IF_UNAWARE;
		}


		/*
		 * Find the squelch type of the object, or TYPE_MAX if none
		 */
		public static squelch_type_t type_of(Object.Object o_ptr)
		{
			/* Find the appropriate squelch group */
			for (int i = 0; i < quality_mapping.Length; i++)
			{
			    if ((quality_mapping[i].tval == o_ptr.tval) &&
			        (quality_mapping[i].min_sval <= o_ptr.sval) &&
			        (quality_mapping[i].max_sval >= o_ptr.sval))
			        return quality_mapping[i].squelch_type;
			}

			return squelch_type_t.TYPE_MAX;
		}

		/**
		 * Small helper function to see how an object trait compares to the one
		 * in its base type.
		 *
		 * If the base type provides a positive bonus, we'll use that. Otherwise, we'll
		 * use zero (players don't consider an item with a positive bonus to be bad
		 * even if the base kind has a higher positive bonus).
		 */
		static int cmp_object_trait(int bonus, random_value Base)
		{
			throw new NotImplementedException();
			//int amt = randcalc(base, 0, MINIMISE);
			//if (amt > 0) amt = 0;
			//return CMP(bonus, amt);
		}

		/**
		 * Small helper function to see if an item seems good, bad or average based on
		 * to_h, to_d and to_a.
		 *
		 * The sign of the return value announces if the object is bad (negative),
		 * good (positive) or average (zero).
		 */
		static int is_object_good(Object.Object o_ptr)
		{
			throw new NotImplementedException();
			//int good = 0;
			//good += 4 * cmp_object_trait(o_ptr.to_d, o_ptr.kind.to_d);
			//good += 2 * cmp_object_trait(o_ptr.to_h, o_ptr.kind.to_h);
			//good += 1 * cmp_object_trait(o_ptr.to_a, o_ptr.kind.to_a);
			//return good;
		}


		/*
		 * Determine the squelch level of an object, which is similar to its pseudo.
		 *
		 * The main point is when the value is undetermined given current info,
		 * return the maximum possible value.
		 */
		public static byte squelch_level_of(Object.Object o_ptr)
		{
			throw new NotImplementedException();
			//byte value;
			//bitflag f[OF_SIZE], f2[OF_SIZE];
			//int i;

			//object_flags_known(o_ptr, f);

			///* Deal with jewelry specially. */
			//if (object_is_jewelry(o_ptr))
			//{
			//    /* CC: average jewelry has at least one known positive pval */
			//    for (i = 0; i < o_ptr.num_pvals; i++)
			//        if ((object_this_pval_is_visible(o_ptr, i)) && (o_ptr.pval[i] > 0))
			//            return SQUELCH_AVERAGE;

			//    if ((o_ptr.to_h > 0) || (o_ptr.to_d > 0) || (o_ptr.to_a > 0))
			//        return SQUELCH_AVERAGE;
			//    if ((object_attack_plusses_are_visible(o_ptr) &&
			//            ((o_ptr.to_h < 0) || (o_ptr.to_d < 0))) ||
			//            (object_defence_plusses_are_visible(o_ptr) && o_ptr.to_a < 0))
			//        return SQUELCH_BAD;

			//    return SQUELCH_AVERAGE;
			//}

			///* And lights */
			//if (o_ptr.tval == TV_LIGHT)
			//{
			//    create_mask(f2, true, OFID_WIELD, OFT_MAX);
			//    if (of_is_inter(f, f2))
			//        return SQUELCH_ALL;
			//    if ((o_ptr.to_h > 0) || (o_ptr.to_d > 0) || (o_ptr.to_a > 0))
			//        return SQUELCH_GOOD;
			//    if ((o_ptr.to_h < 0) || (o_ptr.to_d < 0) || (o_ptr.to_a < 0))
			//        return SQUELCH_BAD;

			//    return SQUELCH_AVERAGE;
			//}

			///* CC: we need to redefine "bad" with multiple pvals
			// * At the moment we use "all pvals known and negative" */
			//for (i = 0; i < o_ptr.num_pvals; i++) {
			//    if (!object_this_pval_is_visible(o_ptr, i) ||
			//        (o_ptr.pval[i] > 0))
			//        break;

			//    if (i == (o_ptr.num_pvals - 1))
			//        return SQUELCH_BAD;
			//}

			//if (object_was_sensed(o_ptr)) {
			//    obj_pseudo_t pseudo = object_pseudo(o_ptr);

			//    switch (pseudo) {
			//        case INSCRIP_AVERAGE: {
			//            value = SQUELCH_AVERAGE;
			//            break;
			//        }

			//        case INSCRIP_EXCELLENT: {
			//            /* have to assume splendid until you have tested it */
			//            if (object_was_worn(o_ptr)) {
			//                if (object_high_resist_is_possible(o_ptr))
			//                    value = SQUELCH_EXCELLENT_NO_SPL;
			//                else
			//                    value = SQUELCH_EXCELLENT_NO_HI;
			//            } else {
			//                value = SQUELCH_ALL;
			//            }
			//            break;
			//        }

			//        case INSCRIP_SPLENDID:
			//            value = SQUELCH_ALL;
			//            break;
			//        case INSCRIP_null:
			//        case INSCRIP_SPECIAL:
			//            value = SQUELCH_MAX;
			//            break;

			//        /* This is the interesting case */
			//        case INSCRIP_STRANGE:
			//        case INSCRIP_MAGICAL: {
			//            value = SQUELCH_GOOD;

			//            if ((object_attack_plusses_are_visible(o_ptr) ||
			//                    randcalc_valid(o_ptr.kind.to_h, o_ptr.to_h) ||
			//                    randcalc_valid(o_ptr.kind.to_d, o_ptr.to_d)) &&
			//                    (object_defence_plusses_are_visible(o_ptr) ||
			//                    randcalc_valid(o_ptr.kind.to_a, o_ptr.to_a))) {
			//                int isgood = is_object_good(o_ptr);
			//                if (isgood > 0) {
			//                    value = SQUELCH_GOOD;
			//                } else if (isgood < 0) {
			//                    value = SQUELCH_BAD;
			//                } else {
			//                    value = SQUELCH_AVERAGE;
			//                }
			//            }
			//            break;
			//        }

			//        default:
			//            /* do not handle any other possible pseudo values */
			//            assert(0);
			//    }
			//}
			//else
			//{
			//    if (object_was_worn(o_ptr))
			//        value = SQUELCH_EXCELLENT_NO_SPL; /* object would be sensed if it were splendid */
			//    else if (object_is_known_not_artifact(o_ptr))
			//        value = SQUELCH_ALL;
			//    else
			//        value = SQUELCH_MAX;
			//}

			//return value;
		}

		/*
		 * Remove any squelching of a particular flavor
		 */
		public static void kind_squelch_clear(Object_Kind k_ptr)
		{
			throw new NotImplementedException();
			//k_ptr.squelch = 0;
			//p_ptr.notice |= PN_SQUELCH;
		}

		public static bool kind_is_squelched_aware(Object_Kind k_ptr)
		{
			return (k_ptr.squelch & SQUELCH_IF_AWARE) != 0 ? true : false;
		}

		public static bool kind_is_squelched_unaware(Object_Kind k_ptr)
		{
			throw new NotImplementedException();
			//return (k_ptr.squelch & SQUELCH_IF_UNAWARE) ? true : false;
		}

		public static void kind_squelch_when_aware(Object_Kind k_ptr)
		{
			throw new NotImplementedException();
			//k_ptr.squelch |= SQUELCH_IF_AWARE;
			//p_ptr.notice |= PN_SQUELCH;
		}

		public static void kind_squelch_when_unaware(Object_Kind k_ptr)
		{
			throw new NotImplementedException();
			//k_ptr.squelch |= SQUELCH_IF_UNAWARE;
			//p_ptr.notice |= PN_SQUELCH;
		}


		/*
		 * Determines if an object is eligible for squelching.
		 */
		public static bool item_ok(Object.Object o_ptr)
		{
			byte type;

			if (Misc.p_ptr.unignoring != 0)
			    return false;

			/* Don't squelch artifacts unless marked to be squelched */
			if (o_ptr.artifact != null || o_ptr.check_for_inscrip("!k") || o_ptr.check_for_inscrip("!*"))
			    return false;

			/* Do squelch individual objects that marked ignore */
			if (o_ptr.ignore)
			    return true;

			/* Auto-squelch dead chests */
			if (o_ptr.tval == TVal.TV_CHEST && o_ptr.pval[Misc.DEFAULT_PVAL] == 0)
			    return true;

			/* Do squelching by kind */
			if (o_ptr.flavor_is_aware() ?
			     kind_is_squelched_aware(o_ptr.kind) :
			     kind_is_squelched_unaware(o_ptr.kind))
			    return true;

			type = (byte)type_of(o_ptr);
			if (type == (int)squelch_type_t.TYPE_MAX)
			    return false;

			/* Squelch items known not to be special */
			if (o_ptr.is_known_not_artifact() && squelch_level[type] == (byte)quality_squelch.SQUELCH_ALL)
			    return true;

			/* Get result based on the feeling and the squelch_level */
			if (squelch_level_of(o_ptr) <= squelch_level[type])
			    return true;
			else
			    return false;
		}

		/*
		 * Drop all {squelch}able items.
		 */
		public static void drop()
		{
			int n;

			/* Scan through the slots backwards */
			for (n = Misc.INVEN_PACK - 1; n >= 0; n--)
			{
			    Object.Object o_ptr = Misc.p_ptr.inventory[n];

			    /* Skip non-objects and unsquelchable objects */
			    if (o_ptr.kind == null) continue;
			    if (!item_ok(o_ptr)) continue;

			    /* Check for !d (no drop) inscription */
			    if (!o_ptr.check_for_inscrip("!d") && !o_ptr.check_for_inscrip("!*"))
			    {
			        /* We're allowed to drop it. */
			        Object.Object.inven_drop(n, o_ptr.number);
			    }
			}

			/* Combine/reorder the pack */
			Misc.p_ptr.notice |= (Misc.PN_COMBINE | Misc.PN_REORDER);
		}


		/*
		 * Reset the player's squelch choices for a new game.
		 */
		public static void birth_init()
		{
			int i;

			/* Reset squelch bits */
			for(i = 0; i < Misc.z_info.k_max; i++) {
				if(Misc.k_info[i] == null)
					continue;
				Misc.k_info[i].squelch = 0;
			}

			/* Clear the squelch bytes */
			for (i = 0; i < (int)squelch_type_t.TYPE_MAX; i++)
			    squelch_level[i] =  (int)quality_squelch.SQUELCH_NONE;
		}


	}
}
