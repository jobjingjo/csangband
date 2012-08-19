using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Player {
	class Player_Class {
		public Player_Class() {
			spells = new Player_Magic();
		}

		public Player_Class next;
		public string Name;
		public UInt32 cidx;

		public string[] title = new string[10];    /* Titles - offset */

		public Int16[] c_adj = new Int16[(int)Stat.Max]; /* Class stat modifier */

		public Int16[] c_skills = new Int16[(int)Skill.MAX];	/* class skills */
		public Int16[] x_skills = new Int16[(int)Skill.MAX];	/* extra skills */

		public Int16 c_mhp;        /* Class hit-dice adjustment */
		public Int16 c_exp;        /* Class experience factor */
	
		public Bitflag pflags = new Bitflag(Misc.PF_SIZE); /* Class (player) flags */

		public UInt16 max_attacks;  /* Maximum possible attacks */
		public UInt16 min_weight;   /* Minimum weapon weight for calculations */
		public UInt16 att_multiply; /* Multiplier for attack calculations */

		public byte spell_book;   /* Tval of spell books (if any) */
		public UInt16 spell_stat;   /* Stat for spells (if any) */
		public UInt16 spell_first;  /* Level of first spell */
		public UInt16 spell_weight; /* Weight that hurts spells */

		public UInt32 sense_base;   /* Base pseudo-id value */
		public UInt16 sense_div;    /* Pseudo-id divisor */

		public Start_Item start_items; /**< The starting inventory */
	
		public Player_Magic spells; /* Magic spells */

		public static Player_Class player_id2class(int id)
		{
			Player_Class c = null;
			for (c = Misc.classes; c != null; c = c.next)
				if (c.cidx == id)
					break;
			return c;
		}
	}
}
