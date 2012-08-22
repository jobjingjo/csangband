using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CSAngband.Object
{
	/*
	 * Object information, for a specific object.
	 *
	 * Note that a "discount" on an item is permanent and never goes away.
	 *
	 * Note that inscriptions are now handled via the "quark_str()" function
	 * applied to the "note" field, which will return null if "note" is zero.
	 *
	 * Note that "object" records are "copied" on a fairly regular basis,
	 * and care must be taken when handling such objects.
	 *
	 * Note that "object flags" must now be derived from the object kind,
	 * the artifact and ego-item indexes, and the two "xtra" fields.
	 *
	 * Each cave grid points to one (or zero) objects via the "o_idx"
	 * field (above).  Each object then points to one (or zero) objects
	 * via the "next_o_idx" field, forming a singly linked list, which
	 * in game terms, represents a "stack" of objects in the same grid.
	 *
	 * Each monster points to one (or zero) objects via the "hold_o_idx"
	 * field (below).  Each object then points to one (or zero) objects
	 * via the "next_o_idx" field, forming a singly linked list, which
	 * in game terms, represents a pile of objects held by the monster.
	 *
	 * The "held_m_idx" field is used to indicate which monster, if any,
	 * is holding the object.  Objects being held have "ix=0" and "iy=0".
	 */
	partial class Object
	{
		/* Object origin kinds */
		public static Object[] o_list;

		
		public Object(){}
		
		/**
		 * Wipe an object clean and make it a standard object of the specified kind.
		 * Was previous "object_prep", is now a constructor for Object
		 */
		public void prep(Object_Kind k, int lev, aspect rand_aspect)
		{
			int flag, x;
			Bitflag flags = new Bitflag(Object_Flag.SIZE);

			// Assign the kind and copy across data
			this.kind = k;
			this.tval = k.tval;
			this.sval = k.sval;
			this.ac = k.ac;
			this.dd = k.dd;
			this.ds = k.ds;
			this.weight = k.weight;

			// Default number
			this.number = 1;

			for(int i = 0; i < pval_flags.Length; i++) {
				pval_flags[i] = new Bitflag(Object_Flag.SIZE);
			}

			// Apply pvals and then copy flags
			for (int i = 0; i < k.num_pvals; i++) {
				flags.copy(k.pval_flags[i]);
				flags.copy(k.pval_flags[i]);
				x = Random.randcalc(k.pval[i], lev, rand_aspect);
				for (flag = flags.next(Bitflag.FLAG_START); flag != Bitflag.FLAG_null; flag = flags.next(flag + 1))
					add_pval(x, flag);
			}
			if(k.Base != null) {
				flags.copy(k.Base.flags);
			}
			flags.union(k.flags);

			// Assign charges (wands/staves only)
			if (tval == TVal.TV_WAND || tval == TVal.TV_STAFF)
				pval[Misc.DEFAULT_PVAL] = (short)Random.randcalc(k.charge, lev, rand_aspect);

			// Assign flagless pval for food or oil
			if (tval == TVal.TV_FOOD || tval == TVal.TV_POTION || tval == TVal.TV_FLASK)
				pval[Misc.DEFAULT_PVAL] = (short)Random.randcalc(k.pval[Misc.DEFAULT_PVAL], lev, rand_aspect);

			// Default fuel for lamps
			if (tval == TVal.TV_LIGHT) {
				if(sval == SVal.SV_LIGHT_TORCH)
					timeout = Misc.DEFAULT_TORCH;
				else if(sval == SVal.SV_LIGHT_LANTERN)
					timeout = Misc.DEFAULT_LAMP;
			}

			// Default magic
			to_h = (short)Random.randcalc(k.to_h, lev, rand_aspect);
			to_d = (short)Random.randcalc(k.to_d, lev, rand_aspect);
			to_a = (short)Random.randcalc(k.to_a, lev, rand_aspect);
		}

		public Object_Kind kind;
		public Ego_Item ego;
		public Artifact artifact;

		public byte iy;			/* Y-position on map, or zero */
		public byte ix;			/* X-position on map, or zero */

		public byte tval;			/* Item type (from kind) */
		public byte sval;			/* Item sub-type (from kind) */

		public Int16[] pval = new Int16[(int)Misc.MAX_PVALS];		/* Item extra-parameter */
		public byte num_pvals;			/* Number of pvals in use */

		public Int16 weight;			/* Item weight */

		public Bitflag flags = new Bitflag(Object_Flag.SIZE);		/**< Flags */
		public Bitflag known_flags = new Bitflag(Object_Flag.SIZE);	/**< Player-known flags */
		public Bitflag[] pval_flags = new Bitflag[Misc.MAX_PVALS];	/**< pval flags */
		public short ident;			/* Special flags */

		public Int16 ac;			/* Normal AC */
		public Int16 to_a;			/* Plusses to AC */
		public Int16 to_h;			/* Plusses to hit */
		public Int16 to_d;			/* Plusses to damage */

		public byte dd, ds;		/* Damage dice/sides */

		public Int16 timeout;		/* Timeout Counter */

		public byte number;		/* Number of items */
		public byte marked;		/* Object is marked */
		public bool ignore;		/* Object is ignored */

		public Int16 next_o_idx;	/* Next object in stack (if any) */
		public Int16 held_m_idx;	/* Monster holding us (if any) */
		public Int16 mimicking_m_idx; /* Monster mimicking us (if any) */

		public Origin origin;        /* How this item was found */
		public byte origin_depth;  /* What depth the item was found at */
		public Int16 origin_xtra;   /* Extra information about origin */

		public Quark note; /* Inscription index */

		public Object copy() {
			return (Object)this.MemberwiseClone();
		}

		/**
		 * Returns percent chance of an object breaking after throwing or shooting.
		 *
		 * Artifacts will never break.
		 *
		 * Beyond that, each item kind has a percent chance to break (0-100). When the
		 * object hits its target this chance is used.
		 *
		 * When an object misses it also has a chance to break. This is determined by
		 * squaring the normaly breakage probability. So an item that breaks 100% of
		 * the time on hit will also break 100% of the time on a miss, whereas a 50%
		 * hit-breakage chance gives a 25% miss-breakage chance, and a 10% hit breakage
		 * chance gives a 1% miss-breakage chance.
		 */
		public int breakage_chance(bool hit_target) {
			int perc = kind.Base.break_perc;

			if (artifact != null) return 0;
			if (!hit_target) return (perc * perc) / 100;
			return perc;
		}

		/*
		 * Some constants used in randart generation and power calculation
		 * - thresholds for limiting to_hit, to_dam and to_ac
		 * - fudge factor for rescaling ammo cost
		 * (a stack of this many equals a weapon of the same damage output)
		 */
		public const int INHIBIT_POWER =    20000;
		public const int INHIBIT_BLOWS =        3;
		public const int INHIBIT_MIGHT    =       4;
		public const int INHIBIT_SHOTS     =      3;
		public const int HIGH_TO_AC         =    26;
		public const int VERYHIGH_TO_AC      =   36;
		public const int INHIBIT_AC           =  56;
		public const int HIGH_TO_HIT           = 16;
		public const int VERYHIGH_TO_HIT        =26;
		public const int HIGH_TO_DAM           = 16;
		public const int VERYHIGH_TO_DAM      =  26;
		public const int AMMO_RESCALER   =      20; // this value is also used for torches

		public static void Init()
		{
			o_list = new Object[Misc.z_info.o_max];
		}

		/** Arrays holding an index of objects to generate for a given level */
		static uint[] obj_total = new uint[Misc.MAX_DEPTH];
		static byte[] obj_alloc;

		static uint[] obj_total_great = new uint[Misc.MAX_DEPTH];
		static byte[] obj_alloc_great;

		/* Don't worry about probabilities for anything past dlev100 */
		const int MAX_O_DEPTH	=	100;

		/*
		 * Hack -- determine if a template is "good".
		 *
		 * Note that this test only applies to the object *kind*, so it is
		 * possible to choose a kind which is "good", and then later cause
		 * the actual object to be cursed.  We do explicitly forbid objects
		 * which are known to be boring or which start out somewhat damaged.
		 */
		static bool kind_is_good(Object_Kind kind)
		{
			/* Analyze the item type */
			switch (kind.tval)
			{
				/* Armor -- Good unless damaged */
				case TVal.TV_HARD_ARMOR:
				case TVal.TV_SOFT_ARMOR:
				case TVal.TV_DRAG_ARMOR:
				case TVal.TV_SHIELD:
				case TVal.TV_CLOAK:
				case TVal.TV_BOOTS:
				case TVal.TV_GLOVES:
				case TVal.TV_HELM:
				case TVal.TV_CROWN:
				{
					if (Random.randcalc(kind.to_a, 0, aspect.MINIMISE) < 0) return (false);
					return (true);
				}

				/* Weapons -- Good unless damaged */
				case TVal.TV_BOW:
					return true; //HACK, it seems as if bows do no damage!!!
				case TVal.TV_SWORD:
				case TVal.TV_HAFTED:
				case TVal.TV_POLEARM:
				case TVal.TV_DIGGING:
				{
					if (Random.randcalc(kind.to_h, 0, aspect.MINIMISE) < 0) return (false);
					if (Random.randcalc(kind.to_d, 0, aspect.MINIMISE) < 0) return (false);
					return (true);
				}

				/* Ammo -- Arrows/Bolts are good */
				case TVal.TV_BOLT:
				case TVal.TV_ARROW:
				{
					return (true);
				}

				/* Books -- High level books are good */
				case TVal.TV_MAGIC_BOOK:
				case TVal.TV_PRAYER_BOOK:
				{
					if (kind.sval >= SVal.SV_BOOK_MIN_GOOD) return (true);
					return (false);
				}

				/* Rings -- Rings of Speed are good */
				case TVal.TV_RING:
				{
					if (kind.sval == (byte)SVal.sval_ring.SV_RING_SPEED) return (true);
					return (false);
				}

				/* Amulets -- Amulets of the Magi are good */
				case TVal.TV_AMULET:
				{
					if (kind.sval == (byte)SVal.sval_amulet.SV_AMULET_THE_MAGI) return (true);
					if (kind.sval == (byte)SVal.sval_amulet.SV_AMULET_DEVOTION) return (true);
					if (kind.sval == (byte)SVal.sval_amulet.SV_AMULET_WEAPONMASTERY) return (true);
					if (kind.sval == (byte)SVal.sval_amulet.SV_AMULET_TRICKERY) return (true);
					return (false);
				}
			}

			/* Assume not good */
			return (false);
		}
		/*
		 * Using k_info[], init rarity data for the entire dungeon.
		 */
		public static bool init_obj_alloc()
		{
			int k_max = Misc.z_info.k_max;
			int item, lev;


			/* Free obj_allocs if allocated */
			//FREE(obj_alloc);

			/* Allocate and wipe */
			obj_alloc = new byte[(MAX_O_DEPTH + 1) * k_max];
			obj_alloc_great = new byte[(MAX_O_DEPTH + 1) * k_max];

			/* Wipe the totals */
			obj_total.Initialize();
			obj_total_great.Initialize();


			/* Init allocation data */
			for (item = 1; item < k_max; item++)
			{
				Object_Kind kind = Misc.k_info[item];
				if(kind == null)
					continue;

				int min = kind.alloc_min;
				int max = kind.alloc_max;

				/* If an item doesn't have a rarity, move on */
				if (kind.alloc_prob == 0) continue;

				/* Go through all the dungeon levels */
				for (lev = 0; lev <= MAX_O_DEPTH; lev++)
				{
					int rarity = kind.alloc_prob;

					/* Save the probability in the standard table */
					if ((lev < min) || (lev > max)) rarity = 0;
					obj_total[lev] += (uint)rarity;
					obj_alloc[(lev * k_max) + item] = (byte)rarity;

					/* Save the probability in the "great" table if relevant */
					if (!kind_is_good(kind)) rarity = 0;
					obj_total_great[lev] += (uint)rarity;
					obj_alloc_great[(lev * k_max) + item] = (byte)rarity;
				}
			}

			return true;
		}


		/*
		 * Free object allocation info.
		 */
		public static void free_obj_alloc()
		{
			//nothing to do here
			//FREE(obj_alloc);
			//FREE(obj_alloc_great);
		}


		/**
		 * \returns whether an object counts as "known" due to EASY_KNOW status
		 */
		bool easy_know()
		{
			if (kind.aware && kind.flags.has(Object_Flag.EASY_KNOW.value))
				return true;
			else
				return false;
		}

		/*
		 * Obtain the flags for an item which are known to the player
		 */
		public void object_flags_known(ref Bitflag flags)
		{
			object_flags(ref flags);

			flags.inter(known_flags);

			if (flavor_is_aware())
			    flags.union(kind.flags);

			if (ego != null && easy_know())
			    flags.union(ego.flags);
		}

		/*
		 * Obtain the flags for an item
		 */
		public void object_flags(ref Bitflag flags)
		{
			flags.wipe();

			if (kind == null)
			    return;

			flags.copy(this.flags);
		}

		/**
		 * Return the pval_flags for an item
		 */
		public void object_pval_flags(out Bitflag[] flags)
		{
			flags = new Bitflag[Misc.MAX_PVALS];

			if (this.kind == null)
				return;

			for (int i = 0; i < Misc.MAX_PVALS; i++) {
				flags[i] = new Bitflag(Object_Flag.SIZE);
				flags[i].copy(pval_flags[i]);
			}
		}

		/**
		 * Return the pval which governs a particular flag.
		 * We assume that we are only called if this pval and flag exist.
		 **/
		public int which_pval(int flag)
		{
			int i;
			Bitflag[] f;

			object_pval_flags(out f);

			for (i = 0; i < Misc.MAX_PVALS; i++) {
			    if (f[i].has(flag))
			        return i;
			}
			
			//XXX XXX XXX THIS IS SO BAD!!! FIGURE OUT WHY WE FAIL!!!
			//Todo: Figure out why we fail
			//We keep failing... I am just returning here...
			/*Utilities.msg("flag is {0}.", flag);
			Utilities.msg("kidx is {0}.", kind.kidx);
			Utilities.pause_line(Term.instance);
			Misc.assert(false);*/
			return 0;
		}

		/**
		 * \returns whether both the object is both an ego and the player knows it is
		 */
		public bool ego_is_visible()
		{
			if (ego == null)
				return false;

			if (tval == TVal.TV_LIGHT)
				return true;

			if ((ident & IDENT_NAME) != 0 || (ident & IDENT_STORE) != 0)
				return true;
			else
				return false;
		}

		/**
		 * \returns whether any ego or artifact name is available to the player
		 */
		public bool name_is_visible()
		{
			return ((ident & IDENT_NAME) != 0) ? true : false;
		}

		/**
		 * \returns whether an object should be treated as fully known (e.g. ID'd)
		 */
		public bool is_known()
		{
			return ((ident & IDENT_KNOWN) != 0) || easy_know() || ((ident & IDENT_STORE) != 0);
		}

		/**
		 * \returns whether the object's defence bonuses are known
		 */
		public bool defence_plusses_are_visible()
		{
			/* Bonuses have been revealed or for sale */
			if ((ident & IDENT_DEFENCE) != 0 || (ident & IDENT_STORE) != 0)
			    return true;

			/* Aware jewelry with non-variable bonuses */
			if (is_jewelry() && flavor_is_aware())
			{
			    if (!Random.randcalc_varies(kind.to_a))
			        return true;
			}

			return false;
		}

		/* ID flags */
		public const int IDENT_SENSE  =   0x0001;  /* Has been "sensed" */
		public const int IDENT_WORN   =   0x0002;  /* Has been tried on */
		public const int IDENT_EMPTY  =   0x0004;  /* Is known to be empty */
		public const int IDENT_KNOWN  =   0x0008;  /* Fully known */
		public const int IDENT_STORE  =   0x0010;  /* Item is in the inventory of a store */
		public const int IDENT_ATTACK =   0x0020;  /* Know combat dice/ac/bonuses */
		public const int IDENT_DEFENCE=   0x0040;  /* Know AC/etc bonuses */
		public const int IDENT_EFFECT =   0x0080;  /* Know item activation/effect */
		/* xxx */
		public const int IDENT_INDESTRUCT=0x0200; /* Tried to destroy it and failed */
		public const int IDENT_NAME   =   0x0400;  /* Know the name of ego or artifact if there is one */
		public const int IDENT_FIRED  =   0x0800;  /* Has been used as a missile */
		public const int IDENT_NOTART =   0x1000;  /* Item is known not to be an artifact */
		public const int IDENT_FAKE   =   0x2000;  /* Item is a fake, for displaying knowledge */

		public const int IDENTS_SET_BY_IDENTIFY = ( IDENT_KNOWN | IDENT_ATTACK | IDENT_DEFENCE | IDENT_SENSE | IDENT_EFFECT | IDENT_WORN | IDENT_FIRED | IDENT_NAME );
		/*
		 * Rings and Amulets
		 */
		public bool is_jewelry(){
			return ((tval == TVal.TV_RING) || (tval == TVal.TV_AMULET));
		}

		/**
		 * \returns whether the player is aware of the object's flavour
		 */
		public bool flavor_is_aware()
		{
			Misc.assert(kind != null);
			return kind.aware;
		}

		/**
		 * \returns whether the object's attack plusses are known
		 */
		public bool attack_plusses_are_visible()
		{
			/* Bonuses have been revealed or for sale */
			if (((ident & IDENT_ATTACK) != 0) || ((ident & IDENT_STORE) != 0))
			    return true;

			/* Aware jewelry with non-variable bonuses */
			if (is_jewelry() && flavor_is_aware())
			{
			    if (!Random.randcalc_varies(kind.to_h) && !Random.randcalc_varies(kind.to_d))
			        return true;
			}

			return false;
		}

		/*
		 * \returns whether the player knows whether an object has a given flag
		 */
		public bool object_flag_is_known(int flag)
		{
			throw new NotImplementedException();
			//if (easy_know(o_ptr) ||	(o_ptr.ident & IDENT_STORE) ||	of_has(o_ptr.known_flags, flag))
			//    return true;

			//return false;
		}

		/*
		 * Return the "attr" for a given item.
		 * Use "flavor" if available.
		 * Default to user definitions.
		 */
		public ConsoleColor object_attr(){
			throw new NotImplementedException();
			//(object_kind_attr((T).kind))
		}

		/*
		 * Return the "char" for a given item.
		 * Use "flavor" if available.
		 * Default to user definitions.
		 */
		public char object_char() {
			throw new NotImplementedException();
			//#define object_char(T) \
			//    (object_kind_char((T).kind))
		}

		/*
		 * Return the "attr" for a given item kind.
		 * Use "flavor" if available.
		 * Default to user definitions.
		 */
		public static ConsoleColor object_kind_attr(Object_Kind k){
			throw new NotImplementedException();
			//return (use_flavor_glyph((kind)) ? 
			// (kind.flavor.x_attr) :
			// (kind.x_attr))
		}

		/*
		 * Return the "char" for a given item kind.
		 * Use "flavor" if available.
		 * Default to user definitions.
		 */
		public static char object_kind_char(Object_Kind k){
			throw new NotImplementedException();
			//#define object_kind_char(kind) \
			//(use_flavor_glyph(kind) ? \
			// ((kind).flavor.x_char) : \
			// ((kind).x_char))
		}
		
		/**
		 * Add a pval to an object, rearranging flags as necessary. Returns true
		 * if the number of pvals is now different, false if it is the same.
		 *
		 * \param o_ptr is the object we're adjusting
		 * \param pval is the pval we are adding
		 * \param flag is the flag to which we are adding the pval
		 */
		bool add_pval(int pval, int flag)
		{
			Bitflag f = new Bitflag(Object_Flag.SIZE);
			int a = -1, best_pval;

			/* Sanity check (we may be called with 0 - see ticket #1451) */
			if (pval == 0) return false;

			Object_Flag.create_mask(f, false, Object_Flag.object_flag_type.PVAL, Object_Flag.object_flag_type.STAT);

			if (flags.has(flag)) {
			    /* See if any other flags are associated with this pval */
			    a = which_pval(flag);
			    f.off(flag);
				f.inter(pval_flags[a]);
			    if (f.is_empty()) { /* Safe to increment and finish */
			        this.pval[a] += (short)pval;
			        if (this.pval[a] == 0) { /* Remove the flag */
						flags.off(flag);
						pval_flags[a].off(flag);
			        }
			        return (object_dedup_pvals());
			    }
			}

			/* So it doesn't have the flag, or it does but that pval also has others */

			/* Create a new pval if we can */
			if (this.num_pvals < Misc.MAX_PVALS) {
			    this.pval[this.num_pvals] = (short)pval;
			    this.pval_flags[this.num_pvals].on(flag);
			    if (a != -1) { /* then we need to move the flag to the new pval */
			        this.pval[this.num_pvals] += this.pval[a];
			        this.pval_flags[a].off(flag);
			    } else /* We need to add it to object_flags */
			        this.flags.on(flag);
			    this.num_pvals++; /* We do this last because pvals start from zero */
			    /* We invert the logic because we've already added a pval */
			    return (!object_dedup_pvals());
			} else { /* we use the closest existing pval */
			    best_pval = object_closest_pval((pval + (a == -1 ? 0 : this.pval[a])));
			    if (best_pval != a) { /* turn on the flag on the new pval */
			        this.pval_flags[best_pval].on(flag);
					if(a != -1) /* turn it off on its old pval */
						this.pval_flags[a].off(flag);
					else /* add it to object_flags */
						this.flags.on(flag);
			    }
			    return false; /* We haven't changed any pvals, so no need to de-dup */
			}
		}

		/**
		 * Return the pval of an object which is closest to value. Returns -1 if
		 * o_ptr has no pvals.
		 */
		int object_closest_pval(int value)
		{
			int i, best_diff, best_pval = 0;

			if (num_pvals == 0)
				return -1; /* Or should we stop wimping around and just assert(0) ? */

			best_diff = value;

			for(i = 0; i < num_pvals; i++) {
				if(Math.Abs(pval[i] - value) < best_diff) {
					best_diff = Math.Abs(pval[i] - value);
					best_pval = i;
				}
			}

			return best_pval;
		}

		/**
		 * Combine two pvals of the same value on an object. Returns true if changes
		 * were made, i.e. o_ptr.num_pvals has decreased by one.
		 */
		bool object_dedup_pvals()
		{
			int i, j, k;

			/* Abort if there can be no duplicates */
			if (num_pvals < 2)
			    return false;

			/* Find the first pair of pvals which have the same value */
			for (i = 0; i < num_pvals; i++) {
			    for (j = i + 1; j < num_pvals; j++) {
			        if (pval[i] == pval[j]) {
			            /* Nuke the j pval and its flags, combining them with i's */
			            pval_flags[i].union(pval_flags[j]);
			            pval_flags[j].wipe();
			            pval[j] = 0;
			            /* Move any remaining pvals down one to fill the void */
			            for (k = j + 1; k < num_pvals; k++) {
			                pval_flags[k - 1].copy(pval_flags[k]);
							pval_flags[k].wipe();
			                pval[k - 1] = pval[k];
			                pval[k] = 0;
			            }
			            /* We now have one fewer pval */
			            num_pvals--;
			            return true;
			        }
			    }
			}

			/* No two pvals are identical */
			return false;
		}

		/**
		 * Mark an object's flavour as as one the player is aware of.
		 *
		 * \param o_ptr is the object whose flavour should be marked as aware
		 */
		public void flavor_aware()
		{
			Player.Player p_ptr = Player.Player.instance;

			if (kind.aware) return;
			
			kind.aware = true;

			/* Fix squelch/autoinscribe */
			p_ptr.notice |= (uint)Misc.PN_SQUELCH;
			Squelch.apply_autoinscription(this);

			for (int i = 1; i < Misc.z_info.o_max; i++)
			{
			    Object floor_o_ptr = Object.byid((short)i);
				if(floor_o_ptr == null)
					continue;

			    /* Some objects change tile on awareness */
			    /* So update display for all floor objects of this kind */
			    if (floor_o_ptr.held_m_idx == 0 && floor_o_ptr.kind == kind)
			        Cave.cave_light_spot(Cave.cave, floor_o_ptr.iy, floor_o_ptr.ix);
			}
		}

		/*
		 * Sets a some IDENT_ flags on an object.
		 *
		 * \param o_ptr is the object to check
		 * \param flags are the ident flags to be added
		 *
		 * \returns whether o_ptr.ident changed
		 */
		bool add_ident_flags(int flags)
		{
			if ((ident & flags) != flags)
			{
				ident |= (short)flags;
				return true;
			}

			return false;
		}

		/**
		 * Make the player aware of all of an object's flags.
		 *
		 * \param o_ptr is the object to mark
		 */
		void know_all_flags()
		{
			known_flags.setall();
		}

		/**
		 * Mark as object as fully known, a.k.a identified. 
		 *
		 * \param o_ptr is the object to mark as identified
		 */
		public void notice_everything()
		{
			/* The object is "empty" */
			ident &= ~(IDENT_EMPTY);

			/* Mark as known */
			flavor_aware();
			add_ident_flags(IDENTS_SET_BY_IDENTIFY);

			/* Artifact has now been seen */
			if (artifact != null && (ident & IDENT_FAKE) == 0)
			{
			    artifact.seen = artifact.everseen = true;
			    History.add_artifact(artifact, true, true);
			}

			/* Know all flags there are to be known */
			know_all_flags();
		}

		/*
		 * Check if we have space for an item in the pack without overflow
		 */
		public bool inven_carry_okay()
		{
			/* Empty slot? */
			if (Player.Player.instance.inven_cnt < Misc.INVEN_MAX_PACK) return true;

			/* Check if it can stack */
			if (inven_stack_okay()) return true;

			/* Nope */
			return false;
		}

		/*
		 * Check to see if an item is stackable in the inventory
		 */
		public bool inven_stack_okay()
		{
			throw new NotImplementedException();
			///* Similar slot? */
			//int j;

			///* If our pack is full and we're adding too many missiles, there won't be
			// * enough room in the quiver, so don't check it. */
			//int limit;

			//if (!pack_is_full())
			//    /* The pack has more room */
			//    limit = ALL_INVEN_TOTAL;
			//else if (p_ptr.quiver_remainder == 0)
			//    /* Quiver already maxed out */
			//    limit = INVEN_PACK;
			//else if (p_ptr.quiver_remainder + o_ptr.number > 99)
			//    /* Too much new ammo */
			//    limit = INVEN_PACK;
			//else
			//    limit = ALL_INVEN_TOTAL;

			//for (j = 0; j < limit; j++)
			//{
			//    object_type *j_ptr = &p_ptr.inventory[j];

			//    /* Skip equipped items and non-objects */
			//    if (j >= INVEN_PACK && j < QUIVER_START) continue;
			//    if (!j_ptr.kind) continue;

			//    /* Check if the two items can be combined */
			//    if (object_similar(j_ptr, o_ptr, OSTACK_PACK)) return (true);
			//}
			//return (false);
		}

		/**
		 * Save the size of the quiver.
		 */
		public static void save_quiver_size(Player.Player p)
		{
			int i, count = 0;
			for (i = Misc.QUIVER_START; i < Misc.QUIVER_END; i++)
				if (p.inventory[i].kind != null)
					count += p.inventory[i].number;

			p.quiver_size = (ushort)count;
			p.quiver_slots = (ushort)((count + 98) / 99);
			p.quiver_remainder = (ushort)(count % 99);
		}

		/*
		 * Add an item to the players inventory, and return the slot used.
		 *
		 * If the new item can combine with an existing item in the inventory,
		 * it will do so, using "object_similar()" and "object_absorb()", else,
		 * the item will be placed into the "proper" location in the inventory.
		 *
		 * This function can be used to "over-fill" the player's pack, but only
		 * once, and such an action must trigger the "overflow" code immediately.
		 * Note that when the pack is being "over-filled", the new item must be
		 * placed into the "overflow" slot, and the "overflow" must take place
		 * before the pack is reordered, but (optionally) after the pack is
		 * combined.  This may be tricky.  See "dungeon.c" for info.
		 *
		 * Note that this code must remove any location/stack information
		 * from the object once it is placed into the inventory.
		 */
		public short inven_carry(Player.Player p)
		{
			int i, j, k;
			int n = -1;

			Object j_ptr;

			/* Apply an autoinscription */
			Squelch.apply_autoinscription(this);

			/* Check for combining */
			for (j = 0; j < Misc.INVEN_PACK; j++)
			{
			    j_ptr = p.inventory[j];
			    if (j_ptr.kind == null) continue;

			    /* Hack -- track last item */
			    n = j;

			    /* Check if the two items can be combined */
			    if (j_ptr.similar(this, object_stack_t.OSTACK_PACK))
			    {
			        /* Combine the items */
			        j_ptr.absorb(this);

			        /* Increase the weight */
			        p.total_weight += (this.number * this.weight);

			        /* Recalculate bonuses */
			        p.update |= (Misc.PU_BONUS);

			        /* Redraw stuff */
			        p.redraw |= (Misc.PR_INVEN);

			        /* Save quiver size */
			        save_quiver_size(p);

			        /* Success */
			        return (short)(j);
			    }
			}


			/* Paranoia */
			if (p.inven_cnt > Misc.INVEN_MAX_PACK) return (-1);


			/* Find an empty slot */
			for (j = 0; j <= Misc.INVEN_MAX_PACK; j++)
			{
			    j_ptr = p.inventory[j];
			    if (j_ptr.kind == null) break;
			}

			/* Use that slot */
			i = j;

			/* Reorder the pack */
			if (i < Misc.INVEN_MAX_PACK)
			{
			    short o_value, j_value;

			    /* Get the "value" of the item */
			    o_value = (short)kind.cost;

			    /* Scan every occupied slot */
			    for (j = 0; j < Misc.INVEN_MAX_PACK; j++)
			    {
			        j_ptr = p.inventory[j];
			        if (j_ptr.kind == null) break;

			        /* Hack -- readable books always come first */
			        if ((tval == Misc.p_ptr.Class.spell_book) &&
			            (j_ptr.tval != Misc.p_ptr.Class.spell_book)) break;
			        if ((j_ptr.tval == Misc.p_ptr.Class.spell_book) &&
			            (tval != Misc.p_ptr.Class.spell_book)) continue;

			        /* Objects sort by decreasing type */
			        if (tval > j_ptr.tval) break;
			        if (tval < j_ptr.tval) continue;

			        /* Non-aware (flavored) items always come last */
			        if (!flavor_is_aware()) continue;
			        if (!j_ptr.flavor_is_aware()) break;

			        /* Objects sort by increasing sval */
			        if (sval < j_ptr.sval) break;
			        if (sval > j_ptr.sval) continue;

			        /* Unidentified objects always come last */
			        if (!is_known()) continue;
			        if (!j_ptr.is_known()) break;

			        /* Lights sort by decreasing fuel */
			        if (tval == TVal.TV_LIGHT)
			        {
			            if (pval[Misc.DEFAULT_PVAL] > j_ptr.pval[Misc.DEFAULT_PVAL]) break;
			            if (pval[Misc.DEFAULT_PVAL] < j_ptr.pval[Misc.DEFAULT_PVAL]) continue;
			        }

			        /* Determine the "value" of the pack item */
			        j_value = (short)j_ptr.kind.cost;

			        /* Objects sort by decreasing value */
			        if (o_value > j_value) break;
			        if (o_value < j_value) continue;
			    }

			    /* Use that slot */
			    i = j;

			    /* Slide objects */
			    for (k = n; k >= i; k--)
			    {
			        /* Hack -- Slide the item */
					p.inventory[k+1] = p.inventory[k];

			        /* Update object_idx if necessary */
			        if (Cave.tracked_object_is(k))
			        {
			            Cave.track_object(k+1);
			        }
			    }

			    /* Wipe the empty slot */
			    p.inventory[i] = new Object();
			}

			p.inventory[i] = this;

			j_ptr = p.inventory[i];
			j_ptr.next_o_idx = 0;
			j_ptr.held_m_idx = 0;
			j_ptr.iy = j_ptr.ix = 0;
			j_ptr.marked = 0; //false

			p.total_weight += (j_ptr.number * j_ptr.weight);
			p.inven_cnt++;
			p.update |= (Misc.PU_BONUS);
			p.notice |= (uint)(Misc.PN_COMBINE | Misc.PN_REORDER);
			p.redraw |= (Misc.PR_INVEN);

			/* Hobbits ID mushrooms on pickup, gnomes ID wands and staffs on pickup */
			if (!j_ptr.is_known())
			{
				if (Misc.p_ptr.player_has(Misc.PF.KNOW_MUSHROOM.value) && j_ptr.tval == TVal.TV_FOOD)
				{
				    Spell.do_ident_item(i, j_ptr);
				    Utilities.msg("Mushrooms for breakfast!");
				}

				if (Misc.p_ptr.player_has(Misc.PF.KNOW_ZAPPER.value) &&
				    (j_ptr.tval == TVal.TV_WAND || j_ptr.tval == TVal.TV_STAFF))
				{
				    Spell.do_ident_item(i, j_ptr);
				}
			}

			/* Save quiver size */
			save_quiver_size(p);

			/* Return the slot */
			return (short)(i);
		}

		/*
		 * Return the "value" of an "unknown" item
		 * Make a guess at the value of non-aware items
		 */
		int value_base()
		{
			throw new NotImplementedException();
			///* Use template cost for aware objects */
			//if (object_flavor_is_aware(o_ptr) || o_ptr.ident & IDENT_STORE)
			//    return o_ptr.kind.cost;

			///* Analyze the type */
			//switch (o_ptr.tval)
			//{
			//    case TV_FOOD:
			//        return 5;
			//    case TV_POTION:
			//    case TV_SCROLL:
			//        return 20;
			//    case TV_RING:
			//    case TV_AMULET:
			//        return 45;
			//    case TV_WAND:
			//        return 50;
			//    case TV_STAFF:
			//        return 70;
			//    case TV_ROD:
			//        return 90;
			//}

			//return 0;
		}


		/*
		 * Return the "real" price of a "known" item, not including discounts.
		 *
		 * Wand and staffs get cost for each charge.
		 *
		 * Wearable items (weapons, launchers, jewelry, lights, armour) and ammo
		 * are priced according to their power rating. All ammo, and normal (non-ego)
		 * torches are scaled down by AMMO_RESCALER to reflect their impermanence.
		 */
		public int value_real(int qty, bool verbose, bool known)
		{
			int value, total_value;

			int power;
			int a = 1;
			int b = 1;

			if (wearable_p())
			{
			    string buf; //[1024];
			    FileStream log_file = null;

				//if(verbose) {
					buf = Misc.path_build(Misc.ANGBAND_DIR_USER, "pricing.log");
					log_file = File.Open(buf, FileMode.Append);
					//if (!log_file)
					//{
					//    msg("Error - can't open pricing.log for writing.");
					//        exit(1);
					//}
				//}

				StreamWriter sr = new StreamWriter(log_file);

				sr.Write("object is " + kind.Name + "\n");
				power = object_power(verbose, sr, known);
				value = ((power == 0)?0:power>0?1:-1) * ((a * power * power) + (b * power));

				if ((tval == TVal.TV_SHOT) || (tval == TVal.TV_ARROW) ||
				    (tval == TVal.TV_BOLT) || ((tval == TVal.TV_LIGHT)
				    && (sval == SVal.SV_LIGHT_TORCH) && ego == null) )
				{
				    value = value / AMMO_RESCALER;
				    if (value < 1) value = 1;
				}

				sr.Write("a is " + a + " and b is " + b + "\n");
				sr.Write("value is " + value + "\n");
				total_value = value * qty;

				//if (verbose)
				//{
				//    if (!file_close(log_file))
				//    {
				//        msg("Error - can't close pricing.log file.");
				//        exit(1);
				//    }
				//}
				if (total_value < 0) total_value = 0;

				log_file.Close();

				return (total_value);
			}

			/* Hack -- "worthless" items */
			if (kind.cost == 0) return (0);

			/* Base cost */
			value = kind.cost;

			/* Analyze the item type and quantity*/
			switch (tval)
			{
			    /* Wands/Staffs */
			    case TVal.TV_WAND:
			    case TVal.TV_STAFF:
			    {
			        total_value = value * qty;

			        /* Calculate number of charges, rounded up */
			        int charges = pval[Misc.DEFAULT_PVAL] * qty / number;
			        if ((pval[Misc.DEFAULT_PVAL] * qty) % number != 0)
			            charges++;

			        /* Pay extra for charges, depending on standard number of charges */
			        total_value += value * charges / 20;

			        /* Done */
			        break;
			    }

			    default:
			    {
			        total_value = value * qty;
			        break;
			    }
			}

			/* No negative value */
			if (total_value < 0) total_value = 0;

			/* Return the value */
			return (total_value);
		}


		/*
		 * Return the price of an item including plusses (and charges).
		 *
		 * This function returns the "value" of the given item (qty one).
		 *
		 * Never notice "unknown" bonuses or properties, including "curses",
		 * since that would give the player information he did not have.
		 *
		 * Note that discounted items stay discounted forever.
		 */
		public int value(int qty, bool verbose)
		{
			int value = 0;


			if (is_known())
			{
			    if (Object_Flag.cursed_p(flags)) return (0);

			    value = value_real(qty, verbose, true);
			}
			else if (wearable_p())
			{
				throw new NotImplementedException();
				//object_type object_type_body;
				//object_type *j_ptr = &object_type_body;

				///* Hack -- Felt cursed items */
				//if (object_was_sensed(o_ptr) && cursed_p((bitflag *)o_ptr.flags))
				//    return (0L);

				//memcpy(j_ptr, o_ptr, sizeof(object_type));

				///* give j_ptr only the flags known to be in o_ptr */
				//object_flags_known(o_ptr, j_ptr.flags);

				//if (!object_attack_plusses_are_visible(o_ptr))
				//    j_ptr.to_h = j_ptr.to_d = 0;
				//if (!object_defence_plusses_are_visible(o_ptr))
				//    j_ptr.to_a = 0;

				//value = object_value_real(j_ptr, qty, verbose, false);
			}
			else value = value_base() * qty;


			/* Return the final value */
			return (value);
		}


		/*
		 * Hack -- determine if an item is "wearable" (or a missile)
		 */
		bool wearable_p()
		{
			/* Valid "tval" codes */
			switch (tval)
			{
				case TVal.TV_SHOT:
				case TVal.TV_ARROW:
				case TVal.TV_BOLT:
				case TVal.TV_BOW:
				case TVal.TV_DIGGING:
				case TVal.TV_HAFTED:
				case TVal.TV_POLEARM:
				case TVal.TV_SWORD:
				case TVal.TV_BOOTS:
				case TVal.TV_GLOVES:
				case TVal.TV_HELM:
				case TVal.TV_CROWN:
				case TVal.TV_SHIELD:
				case TVal.TV_CLOAK:
				case TVal.TV_SOFT_ARMOR:
				case TVal.TV_HARD_ARMOR:
				case TVal.TV_DRAG_ARMOR:
				case TVal.TV_LIGHT:
				case TVal.TV_AMULET:
				case TVal.TV_RING: return (true);
			}

			/* Nope */
			return (false);
		}

		public static Object byid(short oidx)
		{
			Misc.assert(oidx >= 0);
			Misc.assert(oidx <= Misc.z_info.o_max);
			return o_list[oidx];
		}

		public static void set_byid(short oidx, Object o)
		{
			Misc.assert(oidx >= 0);
			Misc.assert(oidx <= Misc.z_info.o_max);
			o_list[oidx] = o;
		}

		/**
		 * \returns whether a specific pval is known to the player
		 */
		public bool this_pval_is_visible(int pval)
		{
			throw new NotImplementedException();
			//bitflag f[MAX_PVALS][OF_SIZE], f2[OF_SIZE];

			//assert(o_ptr.kind);

			//if (o_ptr.ident & IDENT_STORE)
			//    return true;

			///* Aware jewelry with non-variable pval */
			//if (object_is_jewelry(o_ptr) && object_flavor_is_aware(o_ptr)) {
			//    if (!randcalc_varies(o_ptr.kind.pval[pval]))
			//        return true;
			//}

			//if (object_was_worn(o_ptr)) {
			//    object_pval_flags_known(o_ptr, f);

			//    /* Create the mask for pval-related flags */
			//    create_mask(f2, false, OFT_STAT, OFT_PVAL, OFT_MAX);

			//    if (of_is_inter(f[pval], f2))
			//        return true;
			//}

			//return false;
		}

		/**
		 * \returns whether the object has been sensed with pseudo-ID
		 */
		public bool was_sensed()
		{
			return (ident & IDENT_SENSE) != 0 ? true : false;
		}

		/**
		 * \returns whether the object has been worn/wielded
		 */
		public bool was_worn()
		{
			return (ident & IDENT_WORN) != 0 ? true : false;
		}

		/**
		 * Determine which equipment slot (if any) an item likes. The slot might (or
		 * might not) be open, but it is a slot which the object could be equipped in.
		 *
		 * For items where multiple slots could work (e.g. ammo or rings), the function
		 * will try to a return a stackable slot first (only for ammo), then an open
		 * slot if possible, and finally a used (but valid) slot if necessary.
		 */
		public short wield_slot()
		{
			/* Slot for equipment */
			switch (tval)
			{
				case TVal.TV_DIGGING:
				case TVal.TV_HAFTED:
				case TVal.TV_POLEARM:
				case TVal.TV_SWORD: return (Misc.INVEN_WIELD);

				case TVal.TV_BOW: return (Misc.INVEN_BOW);

				case TVal.TV_RING:
					return (short)((Misc.p_ptr.inventory[Misc.INVEN_RIGHT].kind != null) ? Misc.INVEN_LEFT : Misc.INVEN_RIGHT);

				case TVal.TV_AMULET: return (Misc.INVEN_NECK);

				case TVal.TV_LIGHT: return (Misc.INVEN_LIGHT);

				case TVal.TV_DRAG_ARMOR:
				case TVal.TV_HARD_ARMOR:
				case TVal.TV_SOFT_ARMOR: return (Misc.INVEN_BODY);

				case TVal.TV_CLOAK: return (Misc.INVEN_OUTER);

				case TVal.TV_SHIELD: return (Misc.INVEN_ARM);

				case TVal.TV_CROWN:
				case TVal.TV_HELM: return (Misc.INVEN_HEAD);

				case TVal.TV_GLOVES: return (Misc.INVEN_HANDS);

				case TVal.TV_BOOTS: return (Misc.INVEN_FEET);

				case TVal.TV_BOLT:
				case TVal.TV_ARROW:
				case TVal.TV_SHOT:
					return wield_slot_ammo();
			}

			/* No slot available */
			return (-1);
		}


		/**
		 * Used by wield_slot() to find an appopriate slot for ammo. See wield_slot()
		 * for information on what this returns.
		 */
		short wield_slot_ammo()
		{
			short i, open = 0;

			/* If the ammo is inscribed with a slot number, we'll try to put it in */
			/* that slot, if possible. */
			i = (short)get_inscribed_ammo_slot();
			if (i != 0 && Misc.p_ptr.inventory[i].kind == null) return i;

			for (i = (short)Misc.QUIVER_START; i < (short)Misc.QUIVER_END; i++)
			{
				if (Misc.p_ptr.inventory[i].kind == null)
				{
					/* Save the open slot if we haven't found one already */
					if (open == 0) open = i;
					continue;
				}

				/* If ammo is cursed we can't stack it */
				if (Object_Flag.cursed_p(Misc.p_ptr.inventory[i].flags)) continue;

				/* If they are stackable, we'll use this slot for sure */
				if (Misc.p_ptr.inventory[i].similar(this, object_stack_t.OSTACK_QUIVER)) return i;
			}

			/* If not absorbed, return an open slot (or QUIVER_START if no room) */
			return (short)((open != 0) ? open : Misc.QUIVER_START);
		}

		/**
		 * Pseudo-ID markers.
		 */
		public enum obj_pseudo_t
		{
			INSCRIP_null = 0,            /*!< No pseudo-ID status */
			INSCRIP_STRANGE = 1,         /*!< Item that has mixed combat bonuses */
			INSCRIP_AVERAGE = 2,         /*!< Item with no interesting features */
			INSCRIP_MAGICAL = 3,         /*!< Item with combat bonuses */
			INSCRIP_SPLENDID = 4,        /*!< Obviously good item */
			INSCRIP_EXCELLENT = 5,       /*!< Ego-item */
			INSCRIP_SPECIAL = 6,         /*!< Artifact */
			INSCRIP_UNKNOWN = 7,

			INSCRIP_MAX                  /*!< Maximum number of pseudo-ID markers */
		}

		public bool FA_would_be_obvious()
		{
			if (Misc.p_ptr.player_has(Misc.PF.CUMBER_GLOVE.value) && wield_slot() == Misc.INVEN_HANDS) {
				Bitflag flags = new Bitflag(Object_Flag.SIZE);
				object_flags(ref flags);

				if (!flags.has(Object_Flag.DEX.value) && !flags.has(Object_Flag.SPELLS_OK.value))
					return true;
			}

			return false;
		}

		/*
		 * Given an object, return a short identifier which gives some idea of what
		 * the item is.
		 */
		public obj_pseudo_t pseudo()
		{
			Bitflag flags = new Bitflag(Object_Flag.SIZE);
			Bitflag f2 = new Bitflag(Object_Flag.SIZE);

			/* Get the known and obvious flags on the object,
			 * not including curses or properties of the kind.
			 */
			object_flags_known(ref flags);
			Object_Flag.create_mask(f2, true, Object_Flag.object_flag_id.WIELD);

			/* FA on gloves is obvious to mage casters */
			if (FA_would_be_obvious())
				f2.on(Object_Flag.FREE_ACT.value);

			/* Now we remove the non-obvious known flags */
			flags.inter(f2);

			/* Now we remove the cursed flags and the kind flags */
			Object_Flag.create_mask(f2, false, Object_Flag.object_flag_type.CURSE);
			flags.diff(f2);
			flags.diff(kind.flags);

			if ((ident & IDENT_INDESTRUCT) != 0)
				return obj_pseudo_t.INSCRIP_SPECIAL;
			if ((was_sensed() || was_worn()) && artifact != null)
				return obj_pseudo_t.INSCRIP_SPECIAL;

			/* jewelry does not pseudo */
			if (is_jewelry())
				return obj_pseudo_t.INSCRIP_null;

			/* XXX Eddie should also check for flags with pvals where the pval exceeds
			 * the base pval for things like picks of digging, though for now acid brand gets those
			 */
			if (!flags.is_empty())
				return obj_pseudo_t.INSCRIP_SPLENDID;

			if (!is_known() && !was_sensed())
				return obj_pseudo_t.INSCRIP_null;

			if (ego != null)
			{
				/* uncursed bad egos are not excellent */
				if (ego.flags.is_inter(f2))
					return obj_pseudo_t.INSCRIP_STRANGE; /* XXX Eddie need something worse */
				else
					return obj_pseudo_t.INSCRIP_EXCELLENT;
			}

			if (to_a == Random.randcalc(kind.to_a, 0, aspect.MINIMISE) &&
				to_h == Random.randcalc(kind.to_h, 0, aspect.MINIMISE) &&
				 to_d == Random.randcalc(kind.to_d, 0, aspect.MINIMISE))
				return obj_pseudo_t.INSCRIP_AVERAGE;

			if (to_a >= Random.randcalc(kind.to_a, 0, aspect.MINIMISE) &&
				to_h >= Random.randcalc(kind.to_h, 0, aspect.MINIMISE) &&
				to_d >= Random.randcalc(kind.to_d, 0, aspect.MINIMISE))
				return obj_pseudo_t.INSCRIP_MAGICAL;

			if (to_a <= Random.randcalc(kind.to_a, 0, aspect.MINIMISE) &&
				to_h <= Random.randcalc(kind.to_h, 0, aspect.MINIMISE) &&
				to_d <= Random.randcalc(kind.to_d, 0, aspect.MINIMISE))
				return obj_pseudo_t.INSCRIP_MAGICAL;

			return obj_pseudo_t.INSCRIP_STRANGE;
		}

		/**
		 * \returns whether the object is known to not be an artifact
		 */
		public bool is_known_not_artifact()
		{
			if ((ident & IDENT_NOTART) != 0)
				return true;

			return false;
		}

		/*
		 * Looks if "inscrip" is present on the given object.
		 */
		//If this needs to be int, revert to values in comments
		public bool check_for_inscrip(string inscrip)
		{
			if (note == null) return false; //0

			string s = note.ToString();

			if(s != null && s.Contains(inscrip))
				return true;//s.IndexOf(inscrip);
			else
				return false;//s.Length;
		}


		/**
		 * \returns whether the object has been fired/thrown
		 */
		public bool was_fired()
		{
			return ((ident & IDENT_FIRED) != 0) ? true : false;
		}

		/**
		 * \returns whether the player has tried to use other objects of the same kind
		 */
		public bool flavor_was_tried()
		{
			Misc.assert(kind != null); //This might not always be true...?
			return kind.tried;
		}


		/**
		 * Modes for stacking by object_similar()
		 */
		public enum object_stack_t
		{
			OSTACK_NONE    = 0x00, /* No options (this does NOT mean no stacking) */
			OSTACK_STORE   = 0x01, /* Store stacking */
			OSTACK_PACK    = 0x02, /* Inventory and home */
			OSTACK_LIST    = 0x04, /* Object list */
			OSTACK_MONSTER = 0x08, /* Monster carrying objects */
			OSTACK_FLOOR   = 0x10, /* Floor stacking */
			OSTACK_QUIVER  = 0x20  /* Quiver */
		} ;
		/*
		 * Determine if an item can "absorb" a second item
		 *
		 * See "object_absorb()" for the actual "absorption" code.
		 *
		 * If permitted, we allow weapons/armor to stack, if "known".
		 *
		 * Missiles will combine if both stacks have the same "known" status.
		 * This is done to make unidentified stacks of missiles useful.
		 *
		 * Food, potions, scrolls, and "easy know" items always stack.
		 *
		 * Chests, and activatable items, except rods, never stack (for various
		 * reasons).
		 */
		public bool similar(Object j_ptr, object_stack_t mode)
		{
			int i;
			int total = number + j_ptr.number;

			/* Check against stacking limit - except in stores which absorb anyway */
			if ((mode & object_stack_t.OSTACK_STORE) == 0 && (total >= Misc.MAX_STACK_SIZE)) return false;

			/* Hack -- identical items cannot be stacked */
			if (this == j_ptr) return false;

			/* Require identical object kinds */
			if (kind != j_ptr.kind) return false;

			/* Different flags don't stack */
			if (!flags.is_equal(j_ptr.flags)) return false;

			/* Artifacts never stack */
			if (artifact != null || j_ptr.artifact != null) return false;

			/* Analyze the items */
			switch (tval)
			{
			    /* Chests never stack */
			    case TVal.TV_CHEST:
			    {
			        /* Never okay */
			        return false;
			    }

			    /* Food, potions, scrolls and rods all stack nicely */
			    case TVal.TV_FOOD:
			    case TVal.TV_POTION:
			    case TVal.TV_SCROLL:
			    case TVal.TV_ROD:
			    {
			        /* Since the kinds are identical, either both will be
			        aware or both will be unaware */
			        break;
			    }

			    /* Gold, staves and wands stack most of the time */
			    case TVal.TV_STAFF:
			    case TVal.TV_WAND:
			    case TVal.TV_GOLD:
			    {
			        /* Too much gold or too many charges */
			        if (pval[Misc.DEFAULT_PVAL] + j_ptr.pval[Misc.DEFAULT_PVAL] > Misc.MAX_PVAL)
			            return false;

			        /* ... otherwise ok */
			        else break;
			    }

			    /* Weapons, ammo, armour, jewelry, lights */
			    case TVal.TV_BOW:
			    case TVal.TV_DIGGING:
			    case TVal.TV_HAFTED:
			    case TVal.TV_POLEARM:
			    case TVal.TV_SWORD:
			    case TVal.TV_BOOTS:
			    case TVal.TV_GLOVES:
			    case TVal.TV_HELM:
			    case TVal.TV_CROWN:
			    case TVal.TV_SHIELD:
			    case TVal.TV_CLOAK:
			    case TVal.TV_SOFT_ARMOR:
			    case TVal.TV_HARD_ARMOR:
			    case TVal.TV_DRAG_ARMOR:
			    case TVal.TV_RING:
			    case TVal.TV_AMULET:
			    case TVal.TV_LIGHT:
			    case TVal.TV_BOLT:
			    case TVal.TV_ARROW:
			    case TVal.TV_SHOT:
			    {
			        /* Require identical values */
			        if (ac != j_ptr.ac) return false;
			        if (dd != j_ptr.dd) return false;
			        if (ds != j_ptr.ds) return false;

			        /* Require identical bonuses */
			        if (to_h != j_ptr.to_h) return false;
			        if (to_d != j_ptr.to_d) return false;
			        if (to_a != j_ptr.to_a) return false;

			        /* Require all identical pvals */
			        for (i = 0; i < Misc.MAX_PVALS; i++)
			            if (pval[i] != j_ptr.pval[i])
			                return (false);

			        /* Require identical ego-item types */
			        if (ego != j_ptr.ego) return (false);

			        /* Hack - Never stack recharging wearables ... */
			        if ((timeout != 0 || j_ptr.timeout != 0) && tval != TVal.TV_LIGHT) return false;

			        /* ... and lights must have same amount of fuel */
			        else if ((timeout != j_ptr.timeout) && tval == TVal.TV_LIGHT) return false;

			        /* Prevent unIDd items stacking in the object list */
			        if ((mode & object_stack_t.OSTACK_LIST) == 0 && (ident & j_ptr.ident & IDENT_KNOWN) == 0) return false;

			        /* Probably okay */
			        break;
			    }

			    /* Anything else */
			    default:
			    {
			        /* Probably okay */
			        break;
			    }
			}

			/* Require compatible inscriptions */
			if (note != null && j_ptr.note != null && (note != j_ptr.note))
			    return false;

			/* They must be similar enough */
			return (true);
		}

		/*
		 * Allow one item to "absorb" another, assuming they are similar.
		 *
		 * The blending of the "note" field assumes that either (1) one has an
		 * inscription and the other does not, or (2) neither has an inscription.
		 * In both these cases, we can simply use the existing note, unless the
		 * blending object has a note, in which case we use that note.
		 *
		 * The blending of the "discount" field assumes that either (1) one is a
		 * special inscription and one is nothing, or (2) one is a discount and
		 * one is a smaller discount, or (3) one is a discount and one is nothing,
		 * or (4) both are nothing.  In all of these cases, we can simply use the
		 * "maximum" of the two "discount" fields.
		 *
		 * These assumptions are enforced by the "object_similar()" code.
		 */
		public void absorb(Object j_ptr)
		{	
			throw new NotImplementedException();
			//int total = o_ptr.number + j_ptr.number;

			///* Add together the item counts */
			//o_ptr.number = ((total < MAX_STACK_SIZE) ? total : (MAX_STACK_SIZE - 1));

			///* Blend all knowledge */
			//o_ptr.ident |= (j_ptr.ident & ~IDENT_EMPTY);
			//of_union(o_ptr.known_flags, j_ptr.known_flags);

			///* Merge inscriptions */
			//if (j_ptr.note)
			//    o_ptr.note = j_ptr.note;

			///* Combine timeouts for rod stacking */
			//if (o_ptr.tval == TV_ROD)
			//    o_ptr.timeout += j_ptr.timeout;

			///* Combine pvals for wands and staves */
			//if (o_ptr.tval == TV_WAND || o_ptr.tval == TV_STAFF ||
			//        o_ptr.tval == TV_GOLD)
			//{
			//    int total = o_ptr.pval[DEFAULT_PVAL] + j_ptr.pval[DEFAULT_PVAL];
			//    o_ptr.pval[DEFAULT_PVAL] = total >= MAX_PVAL ? MAX_PVAL : total;
			//}

			///* Combine origin data as best we can */
			//if (o_ptr.origin != j_ptr.origin ||
			//        o_ptr.origin_depth != j_ptr.origin_depth ||
			//        o_ptr.origin_xtra != j_ptr.origin_xtra) {
			//    int act = 2;

			//    if (o_ptr.origin_xtra && j_ptr.origin_xtra) {
			//        monster_race *r_ptr = &r_info[o_ptr.origin_xtra];
			//        monster_race *s_ptr = &r_info[j_ptr.origin_xtra];

			//        bool r_uniq = rf_has(r_ptr.flags, RF_UNIQUE) ? true : false;
			//        bool s_uniq = rf_has(s_ptr.flags, RF_UNIQUE) ? true : false;

			//        if (r_uniq && !s_uniq) act = 0;
			//        else if (s_uniq && !r_uniq) act = 1;
			//        else act = 2;
			//    }

			//    switch (act)
			//    {
			//        /* Overwrite with j_ptr */
			//        case 1:
			//        {
			//            o_ptr.origin = j_ptr.origin;
			//            o_ptr.origin_depth = j_ptr.origin_depth;
			//            o_ptr.origin_xtra = j_ptr.origin_xtra;
			//        }

			//        /* Set as "mixed" */
			//        case 2:
			//        {
			//            o_ptr.origin = ORIGIN_MIXED;
			//        }
			//    }
			//}
		}

		/*
		 * Return a string describing how a given item is being worn.
		 * Currently, only used for items in the equipment, not inventory.
		 */
		public static string describe_use(int i)
		{
			string p;

			switch (i)
			{
				case Misc.INVEN_WIELD: p = "attacking monsters with"; break;
				case Misc.INVEN_BOW:   p = "shooting missiles with"; break;
				case Misc.INVEN_LEFT:  p = "wearing on your left hand"; break;
				case Misc.INVEN_RIGHT: p = "wearing on your right hand"; break;
				case Misc.INVEN_NECK:  p = "wearing around your neck"; break;
				case Misc.INVEN_LIGHT: p = "using to light the way"; break;
				case Misc.INVEN_BODY:  p = "wearing on your body"; break;
				case Misc.INVEN_OUTER: p = "wearing on your back"; break;
				case Misc.INVEN_ARM:   p = "wearing on your arm"; break;
				case Misc.INVEN_HEAD:  p = "wearing on your head"; break;
				case Misc.INVEN_HANDS: p = "wearing on your hands"; break;
				case Misc.INVEN_FEET:  p = "wearing on your feet"; break;
				default:          p = "carrying in your pack"; break;
			}

			/* Hack -- Heavy weapon */
			if (i == Misc.INVEN_WIELD)
			{
				Object o_ptr;
				o_ptr = Misc.p_ptr.inventory[i];
				if (Player.Player.adj_str_hold[Misc.p_ptr.state.stat_ind[(int)Stat.Str]] < o_ptr.weight / 10)
				{
					p = "just lifting";
				}
			}

			/* Hack -- Heavy bow */
			if (i == Misc.INVEN_BOW)
			{
				Object o_ptr;
				o_ptr = Misc.p_ptr.inventory[i];
				if (Player.Player.adj_str_hold[Misc.p_ptr.state.stat_ind[(int)Stat.Str]] < o_ptr.weight / 10)
				{
					p = "just holding";
				}
			}

			/* Return the result */
			return p;
		}

		/*
		 * Convert an inventory index into a one character label.
		 *
		 * Note that the label does NOT distinguish inven/equip.
		 */
		public static char index_to_label(int i)
		{
			/* Indexes for "inven" are easy */
			if (i < Misc.INVEN_WIELD) return (Basic.I2A(i));

			/* Indexes for "equip" are offset */
			return (Basic.I2A(i - Misc.INVEN_WIELD));
		}

		/**
		 * Determine whether an object is ammo
		 *
		 * \param o_ptr is the object to check
		 */
		public bool is_ammo()
		{
			switch (tval)
			{
				case TVal.TV_SHOT:
				case TVal.TV_ARROW:
				case TVal.TV_BOLT:
					return true;
				default:
					return false;
			}
		}

		/**
		 * \returns whether the player is aware of the object's effect when used
		 */
		public bool effect_is_known()
		{
			Misc.assert(kind != null);
			return (easy_know() || (ident & IDENT_EFFECT) != 0
				|| (flavor_is_aware() && kind.effect != null)
				|| (ident & IDENT_STORE) != 0) ? true : false;
		}


		/*
		 * Increase the "number" of an item in the inventory
		 */
		public static void inven_item_increase(int item, int num)
		{
			Object o_ptr = Misc.p_ptr.inventory[item];

			/* Apply */
			num += o_ptr.number;

			/* Bounds check */
			if (num > 255) num = 255;
			else if (num < 0) num = 0;

			/* Un-apply */
			num -= o_ptr.number;

			/* Change the number and weight */
			if (num != 0)
			{
				/* Add the number */
				o_ptr.number += (byte)num;

				/* Add the weight */
				Misc.p_ptr.total_weight += (num * o_ptr.weight);

				/* Recalculate bonuses */
				Misc.p_ptr.update |= (Misc.PU_BONUS);

				/* Recalculate mana XXX */
				Misc.p_ptr.update |= (Misc.PU_MANA);

				/* Combine the pack */
				Misc.p_ptr.notice |= (int)(Misc.PN_COMBINE);

				/* Redraw stuff */
				Misc.p_ptr.redraw |= (Misc.PR_INVEN | Misc.PR_EQUIP);
			}
		}

		/*
		 * Erase an inventory slot if it has no more items
		 */
		public static void inven_item_optimize(int item)
		{
			Object o_ptr = Misc.p_ptr.inventory[item];
			int i, j, slot, limit;

			/* Save a possibly new quiver size */
			if (item >= Misc.QUIVER_START) save_quiver_size(Misc.p_ptr);

			/* Only optimize real items which are empty */
			if (o_ptr.kind == null || o_ptr.number != 0) return;

			/* Stop tracking erased item if necessary */
			if (Cave.tracked_object_is(item))
			{
				Cave.track_object(Misc.NO_OBJECT);
			}

			/* Items in the pack are treated differently from other items */
			if (item < Misc.INVEN_WIELD)
			{
				Misc.p_ptr.inven_cnt--;
				Misc.p_ptr.redraw |= Misc.PR_INVEN;
				limit = Misc.INVEN_MAX_PACK;
			}

			/* Items in the quiver and equipped items are (mostly) treated similarly */
			else
			{
				Misc.p_ptr.equip_cnt--;
				Misc.p_ptr.redraw |= Misc.PR_EQUIP;
				limit = item >= Misc.QUIVER_START ? Misc.QUIVER_END : 0;
			}

			/* If the item is equipped (but not in the quiver), there is no need to */
			/* slide other items. Bonuses and such will need to be recalculated */
			if (limit == 0)
			{
				/* Erase the empty slot */
				//object_wipe(&);
				Misc.p_ptr.inventory[item] = new Object();
		
				/* Recalculate stuff */
				Misc.p_ptr.update |= (Misc.PU_BONUS);
				Misc.p_ptr.update |= (Misc.PU_TORCH);
				Misc.p_ptr.update |= (Misc.PU_MANA);
		
				return;
			}

			/* Slide everything down */
			for (j = item, i = item + 1; i < limit; i++)
			{
				if (limit == Misc.QUIVER_END && Misc.p_ptr.inventory[i].kind != null)
				{
					/* If we have an item with an inscribed location (and it's in */
					/* that location) then we won't move it. */
					slot = Misc.p_ptr.inventory[i].get_inscribed_ammo_slot();
					if (slot != 0 && slot == i)
						continue;
				}
				Misc.p_ptr.inventory[j] = Misc.p_ptr.inventory[i]; //Might have to swap these two...
				//COPY(&Misc.p_ptr.inventory[j], &p_ptr.inventory[i], object_type);

				/* Update object_idx if necessary */
				if (Cave.tracked_object_is(i))
				{
					Cave.track_object(j);
				}

				j = i;
			}

			/* Reorder the quiver if necessary */
			if (item >= Misc.QUIVER_START) sort_quiver();

			/* Wipe the left-over object on the end */
			Misc.p_ptr.inventory[j] = new Object();
			//object_wipe(&Misc.p_ptr.inventory[j]);

			/* Inventory has changed, so disable repeat command */ 
			Game_Command.disable_repeat();
		}

		int get_inscribed_ammo_slot()
		{
			char s;
			if (note == null) return 0;
			s = note.ToString()[note.ToString().IndexOf('f') + 1];
			//s = strchr(quark_str(o_ptr.note), 'f');
			if (s == 0 || s < '0' || s > '9') return 0;

			return Misc.QUIVER_START + (s - '0');
		}

		/**
		 * Sorts the quiver--ammunition inscribed with @fN prefers to end up in quiver
		 * slot N.
		 */
		public static void sort_quiver()
		{
			/* Ammo slots go from 0-9; these indices correspond to the range of
			 * (QUIVER_START) - (QUIVER_END-1) in inventory[].
			 */
			bool[] locked = new bool[Misc.QUIVER_SIZE]{false, false, false, false, false,
			                            false, false, false, false, false};
			int[] desired = new int[Misc.QUIVER_SIZE]{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
			int i, j, k;
			Object o_ptr;

			/* Here we figure out which slots have inscribed ammo, and whether that
			 * ammo is already in the slot it "wants" to be in or not.
			 */
			for (i=0; i < Misc.QUIVER_SIZE; i++)
			{
			    j = Misc.QUIVER_START + i;
			    o_ptr = Misc.p_ptr.inventory[j];

			    /* Skip this slot if it doesn't have ammo */
			    if (o_ptr.kind == null) continue;

			    /* Figure out which slot this ammo prefers, if any */
			    k = o_ptr.get_inscribed_ammo_slot();
			    if (k == null) continue;

			    k -= Misc.QUIVER_START;
			    if (k == i) locked[i] = true;
			    if (desired[k] < 0) desired[k] = i;
			}

			/* For items which had a preference that was not fulfilled, we will swap
			 * them into the slot as long as it isn't already locked.
			 */
			for (i=0; i < Misc.QUIVER_SIZE; i++)
			{
			    if (locked[i] || desired[i] < 0) continue;

			    /* item in slot 'desired[i]' desires to be in slot 'i' */
				throw new NotImplementedException();
				//swap_quiver_slots(desired[i], i);
				//locked[i] = true;
			}

			/* Now we need to compact ammo which isn't in a preferrred slot towards the
			 * "front" of the quiver */
			for (i=0; i < Misc.QUIVER_SIZE; i++)
			{
			    /* If the slot isn't empty, skip it */
			    if (Misc.p_ptr.inventory[Misc.QUIVER_START + i].kind != null) continue;

			    /* Start from the end and find an unlocked item to put here. */
			    for (j=Misc.QUIVER_SIZE - 1; j > i; j--)
			    {
			        if (Misc.p_ptr.inventory[Misc.QUIVER_START + j].kind == null || locked[j]) continue;
					throw new NotImplementedException();
					//swap_quiver_slots(i, j);
					//break;
			    }
			}

			/* Now we will sort all other ammo using a simple insertion sort */
			for (i=0; i < Misc.QUIVER_SIZE; i++)
			{
			    k = i;
				if(!locked[k])
					for(j = i + 1; j < Misc.QUIVER_SIZE; j++)
						if(!locked[j] && compare_ammo(k, j) > 0)
							throw new NotImplementedException();
			                //swap_quiver_slots(j, k);
			}
		}

		/*
		 * Choose an object kind given a dungeon level to choose it for.
		 */
		public static Object_Kind get_obj_num(int level, bool good)
		{
			/* This is the base index into obj_alloc for this dlev */
			int ind, item;
			int value;

			/* Occasional level boost */
			if ((level > 0) && Random.one_in_(GREAT_OBJ))
			{
			    /* What a bizarre calculation */
			    level = (int)(1 + (level * MAX_O_DEPTH / Random.randint1(MAX_O_DEPTH)));
			}

			/* Paranoia */
			level = Math.Min(level, MAX_O_DEPTH);
			level = Math.Max(level, 0);

			/* Pick an object */
			ind = level * Misc.z_info.k_max;

			if (!good)
			{
			    value = Random.randint0((int)obj_total[level]);
			    for (item = 1; item < Misc.z_info.k_max; item++)
			    {
			        /* Found it */
			        if (value < obj_alloc[ind + item]) break;

			        /* Decrement */
			        value -= obj_alloc[ind + item];
			    }
			}
			else
			{
			    value = Random.randint0((int)obj_total_great[level]);
			    for (item = 1; item < Misc.z_info.k_max; item++)
			    {
			        /* Found it */
			        if (value < obj_alloc_great[ind + item]) break;

			        /* Decrement */
			        value -= obj_alloc_great[ind + item];
			    }
			}


			/* Return the item index */
			return objkind_byid(item);
		}

		static Object_Kind objkind_byid(int kidx) {
			if (kidx < 1 || kidx > Misc.z_info.k_max)
				return null;
			return Misc.k_info[kidx];
		}

		public void WIPE() {
			kind = null;
			number = 0;
		}

		/*
		 * Delete a dungeon object
		 *
		 * Handle "stacks" of objects correctly.
		 */
		public static void delete_object_idx(int o_idx)
		{
			throw new NotImplementedException();
			//object_type *j_ptr;

			///* Excise */
			//excise_object_idx(o_idx);

			///* Object */
			//j_ptr = object_byid(o_idx);

			///* Dungeon floor */
			//if (!(j_ptr.held_m_idx))
			//{
			//    int y, x;

			//    /* Location */
			//    y = j_ptr.iy;
			//    x = j_ptr.ix;

			//    cave_light_spot(cave, y, x);
			//}
	
			///* Delete the mimicking monster if necessary */
			//if (j_ptr.mimicking_m_idx) {
			//    monster_type *m_ptr;
		
			//    m_ptr = cave_monster(cave, j_ptr.mimicking_m_idx);
		
			//    /* Clear the mimicry */
			//    m_ptr.mimicked_o_idx = 0;
		
			//    delete_monster_idx(j_ptr.mimicking_m_idx);
			//}

			///* Wipe the object */
			//object_wipe(j_ptr);

			///* Count objects */
			//o_cnt--;

			///* Stop tracking deleted objects if necessary */
			//if (tracked_object_is(0 - o_idx))
			//{
			//    track_object(NO_OBJECT);
			//}
		}


	}
}
