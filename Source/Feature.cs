using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Feature {
		/**
		 * Information about terrain features.
		 *
		 * At the moment this isn't very much, but eventually a primitive flag-based
		 * information system will be used here.
		 */
		public string name;
		public int fidx;

		public Feature next;

		public byte mimic;    /**< Feature to mimic */
		public byte priority; /**< Display priority */

		public byte locked;   /**< How locked is it? */
		public byte jammed;   /**< How jammed is it? */
		public byte shopnum;  /**< Which shop does it take you to? */
		public byte dig;      /**< How hard is it to dig through? */

		public Effect effect;   /**< Effect on entry to grid */
		public UInt32 flags;    /**< Terrain flags */

		public ConsoleColor d_attr;   /**< Default feature attribute */
		public char d_char;   /**< Default feature character */

		public ConsoleColor[] x_attr = new ConsoleColor[3];   /**< Desired feature attribute (set by user/pref file) */
		public char[] x_char = new char[3];   /**< Desired feature character (set by user/pref file) */
	}
}
