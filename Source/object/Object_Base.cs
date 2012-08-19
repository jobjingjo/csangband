using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Object {
	/**
	 * Information about object types, like rods, wands, etc.
	 */
	class Object_Base {
		public string Name;

		public int tval;
		public Object_Base Next;

		public Bitflag flags = new Bitflag(Object_Flag.SIZE);

		public int break_perc;

		//Copies values from 'from'... a copy constructor basically.
		public void Copy(Object_Base from) {
			this.Name = from.Name;
			this.tval = from.tval;
			//this.Next = from.Next; //I don't think we need that... you can't set it in a config
			this.flags.copy(from.flags);
			this.break_perc = from.break_perc;
		}
	}
}
