using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	public class Loc {
		public Loc() {
			this.x = 0;
			this.y = 0;
		}
		public Loc(int x, int y) {
			this.x = x;
			this.y = y;
		}
		public int x;
		public int y;
	}
}
