using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	/*
	* Extra data to associate with each "window"
	*
	* Each "window" is represented by a "term_data" structure, which
	* contains a "term" structure, which contains a pointer (t.data)
	* back to the term_data structure.
	*/
	class Term_Data {
		public Term_Data() {
			t = new Term();
		}

		public Term t;

		/* Other fields if needed XXX XXX XXX */
	}
}
