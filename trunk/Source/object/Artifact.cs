using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Object {
	/**
	 * Information about artifacts.
	 *
	 * Note that ::cur_num is written to the savefile.
	 *
	 * TODO: Fix this max_num/cur_num crap and just have a big boolean array of
	 * which artifacts have been created and haven't, so this can become read-only.
	 */
	class Artifact {
		public Artifact() {
			 for (int i = 0; i < Misc.MAX_PVAL; i++){
				 pval_flags[i] = new Bitflag(Object_Flag.SIZE);
			 }
		}
		public string Name;
		public string Text;

		public UInt32 aidx;

		public Artifact Next;

		public byte tval;    /**< General artifact type (see TV_ macros) */
		public byte sval;    /**< Artifact sub-type (see SV_ macros) */
		public Int16[] pval = new Int16[(int)Misc.MAX_PVALS];    /**< Power for any flags which need it */
		public byte num_pvals;/**< Number of pvals in use on this item */

		public Int16 to_h;    /**< Bonus to hit */
		public Int16 to_d;    /**< Bonus to damage */
		public Int16 to_a;    /**< Bonus to armor */
		public Int16 ac;      /**< Base armor */

		public byte dd;      /**< Base damage dice */
		public byte ds;      /**< Base damage sides */

		public Int16 weight;  /**< Weight in 1/10lbs */

		public Int32 cost;    /**< Artifact (pseudo-)worth */

		public Bitflag flags = new Bitflag(Object_Flag.SIZE);		/**< Flags */
		public Bitflag[] pval_flags = new Bitflag[Misc.MAX_PVAL];	/**< pval flags */

		public byte level;   /** Difficulty level for activation */
		public byte rarity;  /** Unused */
		public byte alloc_prob; /** Chance of being generated (i.e. rarity) */
		public byte alloc_min;  /** Minimum depth (can appear earlier) */
		public byte alloc_max;  /** Maximum depth (will NEVER appear deeper) */

		public bool created;	/**< Whether this artifact has been created */
		public bool seen;	/**< Whether this artifact has been seen this game */
		public bool everseen;	/**< Whether this artifact has ever been seen  */

		public Effect effect;     /**< Artifact activation (see effects.c) */
		public string effect_msg;

		public random_value time;  /**< Recharge time (if appropriate) */

		public string gen_name(string[][] words) {
			throw new NotImplementedException();
			/*char buf[BUFLEN];
			char word[MAX_NAME_LEN + 1];
			randname_make(RANDNAME_TOLKIEN, MIN_NAME_LEN, MAX_NAME_LEN, word, sizeof(word), words);
			word[0] = toupper((unsigned char)word[0]);
			if (one_in_(3))
				strnfmt(buf, sizeof(buf), "'%s'", word);
			else
				strnfmt(buf, sizeof(buf), "of %s", word);
			if (a.aidx == ART_POWER)
				strnfmt(buf, sizeof(buf), "of Power (The One Ring)");
			if (a.aidx == ART_GROND)
				strnfmt(buf, sizeof(buf), "'Grond'");
			if (a.aidx == ART_MORGOTH)
				strnfmt(buf, sizeof(buf), "of Morgoth");
			return string_make(buf);*/
		}

		/**
		 * Return the a_idx of the artifact with the given name
		 */
		public static int lookup_artifact_name(string name)
		{
			int i;
			int a_idx = -1;
	
			/* Look for it */
			for (i = 1; i < Misc.z_info.a_max; i++)
			{
			    Artifact a_ptr = Misc.a_info[i];
				if(a_ptr == null) {
					continue;
				}

			    /* Test for equality */
			    if (a_ptr.Name != null && name == a_ptr.Name)
			        return i;
		
			    /* Test for close matches */
			    if (a_ptr.Name != null && (a_ptr.Name.ToLower() == name.ToLower()) && a_idx == -1)
			        a_idx = i;
			} 

			/* Return our best match */
			return a_idx;
		}
	}
}
