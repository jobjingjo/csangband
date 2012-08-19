using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Data_Panel {
		public ConsoleColor color;
		public string label;
		public string fmt;	/* printf format argument */
		public Type_Union[] value = new Type_Union[Misc.MAX_FMT];	/* (short) arugment list */
	}
}
