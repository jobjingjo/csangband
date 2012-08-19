using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	/*
	 * Indexes of the various "stats" (hard-coded by savefiles, etc).
	 */
	public enum Stat : int
	{
		Str = 0,
		Int,
		Wis,
		Dex,
		Con,
		Chr,

		Max
	};

	public class Stat_Names {
		/*
		 * Abbreviations of healthy stats
		 */		
		public static string[] Normal = new string[]
		{
			"STR: ", "INT: ", "WIS: ", "DEX: ", "CON: ", "CHR: "
		};

		/*
		 * Abbreviations of damaged stats
		 */
		public static string [] Reduced = new string[]
		{
			"Str: ", "Int: ", "Wis: ", "Dex: ", "Con: ", "Chr: "
		};
	}
}
