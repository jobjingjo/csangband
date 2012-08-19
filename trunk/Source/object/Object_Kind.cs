using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * Information about object kinds, including player knowledge.
 *
 * TODO: split out the user-changeable bits into a separate struct so this
 * one can be read-only.
 */
namespace CSAngband.Object
{
	class Object_Kind
	{
		public Object_Kind() {
			for(int i = 0; i < Misc.MAX_PVALS; i++) {
				pval_flags[i] = new Bitflag(Object_Flag.SIZE);
			}
			to_h = new random_value();
			to_d = new random_value();
			to_a = new random_value();

			for(int i = 0; i < pval.Length; i++) {
				pval[i] = new random_value();
			}

			effect = Effect.XXX;
		}
		public string Name;
		public string Text;

		public Object_Base Base;

		public Object_Kind next;
		public UInt32 kidx;

		public byte tval;         /**< General object type (see TV_ macros) */
		public byte sval;         /**< Object sub-type (see SV_ macros) */
		public random_value[] pval = new random_value[(int)Misc.MAX_PVALS]; /**< Power for any flags which need it */
		public byte num_pvals;	   /**< Number of pvals in use on this item */

		public random_value to_h; /**< Bonus to hit */
		public random_value to_d; /**< Bonus to damage */
		public random_value to_a; /**< Bonus to armor */
		public Int16 ac;           /**< Base armor */

		public byte dd;           /**< Damage dice */
		public byte ds;           /**< Damage sides */
		public Int16 weight;       /**< Weight, in 1/10lbs */

		public Int32 cost;         /**< Object base cost */

		public Bitflag flags = new Bitflag(Object_Flag.SIZE);			/**< Flags */
		public Bitflag[] pval_flags = new Bitflag[Misc.MAX_PVALS];	/**< pval flags */

		public ConsoleColor d_attr;       /**< Default object attribute */
		public char d_char;       /**< Default object character */

		public byte alloc_prob;   /**< Allocation: commonness */
		public byte alloc_min;    /**< Highest normal dungeon level */
		public byte alloc_max;    /**< Lowest normal dungeon level */
		public byte level;        /**< Level (difficulty of activation) */

		public Effect effect;         /**< Effect this item produces (effects.c) */
		public random_value time;   /**< Recharge time (rods/activation) */
		public random_value charge; /**< Number of charges (staves/wands) */

		public byte gen_mult_prob;      /**< Probability of generating more than one */
		public random_value stack_size; /**< Number to generate */

		public Flavor flavor;         /**< Special object flavor (or zero) */


		/** Game-dependent **/

		public ConsoleColor x_attr;   /**< Desired object attribute (set by user/pref file) */
		public char x_char;   /**< Desired object character (set by user/pref file) */

		/** Also saved in savefile **/

		public Quark note; /**< Autoinscription quark number */

		public bool aware;    /**< Set if player is aware of the kind's effects */
		public bool tried;    /**< Set if kind has been tried */

		public byte squelch;  /**< Squelch settings */
		public bool everseen; /**< Set if kind has ever been seen (to despoilify squelch menus) */

		public Spell spells;

		//was in obj-util.c
		/**
		 * Return the object kind with the given `tval` and `sval`, or null.
		 */
		public static Object_Kind lookup_kind(int tval, int sval)
		{
			/* Look for it */
			for (int k = 0; k < Misc.z_info.k_max; k++)
			{
			    Object_Kind kind = Misc.k_info[k];
				if(kind == null)
					continue;
			    if (kind.tval == tval && kind.sval == sval)
			        return kind;
			}

			/* Failure */
			Utilities.msg("No object: {0}:{1} ({2})", tval, sval, TVal.find_name(tval));
			return null;
		}

		public static Object_Kind objkind_get(int tval, int sval) {
			return lookup_kind(tval, sval);
		}
	}
}
