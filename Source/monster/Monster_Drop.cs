using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Monster {
	/* Information about specified monster drops */ 
	class Monster_Drop {
		public Monster_Drop Next;
		public Object.Object_Kind kind;
		public Object.Artifact artifact;
		public UInt32 percent_chance;
		public UInt32 min;
		public UInt32 max;
	}
}
