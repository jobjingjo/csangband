using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband{
	class GUID {
		public UInt32 ID;

		public static bool Equal(GUID a, GUID b)
		{
			return a.ID == b.ID;
		}

		public bool Equals(GUID other) {
			return this.ID == other.ID;
		}
	}
}
