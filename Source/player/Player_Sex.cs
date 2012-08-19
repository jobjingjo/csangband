using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Player {
	class Player_Sex {
		public Player_Sex(string title, string winner) {
			this.Title = title;
			this.Winner = winner;
		}
		public string Title;		/* Type of sex */
		public string Winner;		/* Name of winner */
	}
}
