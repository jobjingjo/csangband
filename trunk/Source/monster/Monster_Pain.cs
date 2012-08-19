using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Monster {
	/*
	 * Monster pain messages.
	 */
	class Monster_Pain {
		public string[] Messages = new string[7];
		public int pain_idx;
	
		public Monster_Pain Next;
	}
}
