using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Menu_Action {
		/**
		 * Flags for menu_actions.
		 */
		public const uint MN_ACT_GRAYED		=   0x0001; /* Allows selection but no action */
		public const uint MN_ACT_HIDDEN		=	0x0002; /* Row is hidden, but may be selected via tag */
		public Menu_Action(int flags, char tag, string name, action_func action) {
			this.flags = flags;
			this.tag = tag;
			this.name = name;
			this.action = action;
		}

		public int flags;
		public char tag;
		public string name;

		public delegate void action_func(string s, int row);
		public action_func action;
	}
}
