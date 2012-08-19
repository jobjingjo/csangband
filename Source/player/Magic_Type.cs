using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Player {
	/*
	 * A structure to hold class-dependent information on spells.
	 */
	class Magic_Type {
		public byte slevel;		/* Required level (to learn) */
		public byte smana;			/* Required mana (to cast) */
		public byte sfail;			/* Minimum chance of failure */
		public byte sexp;			/* Encoded experience bonus */
	}
}
