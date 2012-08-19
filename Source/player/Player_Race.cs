using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Player {
	class Player_Race {
		public Player_Race Next;
		public string Name;
	
		public UInt32 ridx;

		public Int16[] r_adj = new Int16[(int)Stat.Max];	/* Racial stat bonuses */
	
		public Int16[] r_skills = new Int16[(int)Skill.MAX];	/* racial skills */

		public byte r_mhp;			/* Race hit-dice modifier */
		public byte r_exp;			/* Race experience factor */

		public byte b_age;			/* base age */
		public byte m_age;			/* mod age */

		public byte m_b_ht;		/* base height (males) */
		public byte m_m_ht;		/* mod height (males) */
		public byte m_b_wt;		/* base weight (males) */
		public byte m_m_wt;		/* mod weight (males) */

		public byte f_b_ht;		/* base height (females) */
		public byte f_m_ht;		/* mod height (females) */
		public byte f_b_wt;		/* base weight (females) */
		public byte f_m_wt;		/* mod weight (females) */

		public byte infra;			/* Infra-vision	range */

		public byte choice;		/* Legal class choices */
		public History_Chart history;
	
		public Bitflag flags = new Bitflag(Object.Object_Flag.SIZE);   /* Racial (object) flags */
		public Bitflag pflags= new Bitflag(Misc.PF_SIZE);  /* Racial (player) flags */

		public static Player_Race player_id2race(int id)
		{
			Player_Race r = null;
			for (r = Misc.races; r != null; r = r.Next)
				if (r.ridx == id)
					break;
			return r;
		}

	}
}
