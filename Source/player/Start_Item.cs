using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Player {
	class Start_Item {
		public Object.Object_Kind kind;
		public byte min;	/* Minimum starting amount */
		public byte max;	/* Maximum starting amount */

		public Start_Item next;
	}
}
