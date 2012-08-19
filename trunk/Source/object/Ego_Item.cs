using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Object
{
	class Ego_Item
	{
		public Ego_Item() {
			for(int i = 0; i < Misc.MAX_PVALS; i++) {
				pval_flags[i] = new Bitflag(Object_Flag.SIZE);
			}

			to_h = new random_value();
			to_d = new random_value();
			to_a = new random_value();
		}

		public Ego_Item next;

		public string name;
		public string text;

		public UInt32 eidx;

		public Int32 cost;			/* Ego-item "cost" */

		public Bitflag flags = new Bitflag(Object_Flag.SIZE);		/**< Flags */
		public Bitflag[] pval_flags = new Bitflag[Misc.MAX_PVALS];	/**< pval flags */

		public byte level;		/* Minimum level */
		public byte rarity;		/* Object rarity */
		public byte rating;		/* Level rating boost */
		public byte alloc_prob; 	/** Chance of being generated (i.e. rarity) */
		public byte alloc_min;  	/** Minimum depth (can appear earlier) */
		public byte alloc_max;  	/** Maximum depth (will NEVER appear deeper) */

		public int tval_at = 0;
		public byte[] tval = new byte[Misc.EGO_TVALS_MAX]; 	/* Legal tval */
		public byte[] min_sval = new byte[Misc.EGO_TVALS_MAX];	/* Minimum legal sval */
		public byte[] max_sval = new byte[Misc.EGO_TVALS_MAX];	/* Maximum legal sval */

		public random_value to_h;     		/* Extra to-hit bonus */
		public random_value to_d; 		/* Extra to-dam bonus */
		public random_value to_a; 		/* Extra to-ac bonus */
		public random_value[] pval = new random_value[Misc.MAX_PVALS]; 	/* Extra pval bonus */
		public byte num_pvals;			/* Number of pvals used */

		public byte min_to_h;			/* Minimum to-hit value */
		public byte min_to_d;			/* Minimum to-dam value */
		public byte min_to_a;			/* Minimum to-ac value */
		public byte[] min_pval = new byte[Misc.MAX_PVALS];	/* Minimum pval */

		public byte xtra;			/* Extra sustain/resist/power */

		public bool everseen;			/* Do not spoil squelch menus */
	}
}
