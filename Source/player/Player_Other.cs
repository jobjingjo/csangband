using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Player {
	/*
	 * Some more player information
	 *
	 * This information is retained across player lives
	 */
	class Player_Other {
		public Player_Other() {
			full_name = "";
		}

		///*
		// * The player other record (static)
		// */
		//static player_other player_other_body;

		///*
		// * Pointer to the player other record
		// */
		//player_other *op_ptr = &player_other_body;
		public static Player_Other instance = new Player_Other(); //voala! Singleton!


		//Both have max length 32
		public string full_name;		/* Full name */
		public string base_name;		/* Base name */
	
		public bool[] opt = new bool[Option.MAX];		/* Options */
	
		public uint[] window_flag = new uint[Misc.ANGBAND_TERM_MAX];	/* Window flags */
	
		public byte hitpoint_warn;		/* Hitpoint warning (0 to 9) */
	
		public byte delay_factor;		/* Delay factor (0 to 9) */
	
		public byte name_suffix;		/* numeric suffix for player name */
	}
}
