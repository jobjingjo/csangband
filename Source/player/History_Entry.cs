using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Player {
	class History_Entry {
		public History_Entry next;
		public History_Chart succ;
		public int isucc;
		public int roll;
		public int bonus;
		public string text;
	}
}
