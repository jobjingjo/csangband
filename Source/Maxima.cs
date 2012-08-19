using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	/**
	 * Information about maximal indices of certain arrays.
	 *
	 * These are actually not the maxima, but the maxima plus one, because of
	 * 0-based indexing issues.
	 */
	class Maxima {
		public UInt16 f_max;       /**< Maximum number of terrain features */
		public UInt16 k_max;       /**< Maximum number of object base kinds */
		public UInt16 a_max;       /**< Maximum number of artifact kinds */
		public UInt16 e_max;       /**< Maximum number of ego-item kinds */
		public UInt16 r_max;       /**< Maximum number of monster races */
		public UInt16 mp_max;	  /**< Maximum number of monster pain message sets */
		public UInt16 s_max;       /**< Maximum number of magic spells */
		public UInt16 pit_max;	  /**< Maximum number of monster pit types */

		public UInt16 o_max;       /**< Maximum number of objects on a given level */
		public UInt16 m_max;       /**< Maximum number of monsters on a given level */
	}
}
