using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Xtra3 {
		/* Return a random hint from the global hints list */
		public static string random_hint()
		{
			Hint v = null, r = null;
			int n;
			for (v = Misc.hints, n = 1; v != null; v = v.next, n++)
			    if (Random.one_in_(n))
			        r = v;
			return r.hint;
		}
	}
}
