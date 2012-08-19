using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Player {
	class History_Chart {
		public History_Chart next;
		public History_Entry entries;
		public UInt32 idx;

		/*
		 * Get the racial history, and social class, using the "history charts".
		 */
		public string get_history(out Int16 sc)
		{
			int roll, social_class;
			History_Entry entry;
			string res = "";

			social_class = (int)Random.randint1(4);

			History_Chart chart = this;

			while (chart != null) {
				roll = (int)Random.randint1(100);
				for (entry = chart.entries; ; entry = entry.next)
					if (roll <= entry.roll)
						break;
				Misc.assert(entry != null);

				res += entry.text;
				social_class += entry.bonus - 50;
				chart = entry.succ;
			}

			if (social_class > 75)
				social_class = 75;
			else if (social_class < 1)
				social_class = 1;

			sc = (short)social_class;
			
			return res;
		}
	}

	/* Histories are a graph of charts; each chart contains a set of individual
	 * entries for that chart, and each entry contains a text description and a
	 * successor chart to move history generation to.
	 * For example:
	 * 	chart 1 {
	 * 		entry {
	 * 			desc "You are the illegitimate and unacknowledged child";
	 * 			next 2;
	 * 		};
	 * 		entry {
	 * 			desc "You are the illegitimate but acknowledged child";
	 * 			next 2;
	 * 		};
	 * 		entry {
	 * 			desc "You are one of several children";
	 * 			next 3;
	 * 		};
	 * 	};
	 *
	 * History generation works by walking the graph from the starting chart for
	 * each race, picking a random entry (with weighted probability) each time.
	 */
}
