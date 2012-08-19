using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Grouper {
		/**
		 * Defines a (value, name) pairing.  Variable names used are historical.
		 */
		public int tval;
		public string name;

		public Grouper(int val, string name) {
			this.tval = val;
			this.name = name;
		}
	}
}
