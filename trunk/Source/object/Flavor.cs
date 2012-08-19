using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Object {
	class Flavor {
		public string text;
		public Flavor next;
		public UInt32 fidx;

		public byte tval;      /* Associated object type */
		public byte sval;      /* Associated object sub-type */

		public ConsoleColor d_attr;    /* Default flavor attribute */
		public char d_char;    /* Default flavor character */

		public ConsoleColor x_attr;    /* Desired flavor attribute */
		public char x_char;    /* Desired flavor character */


		/* Maximum number of scroll titles generated */
		const int MAX_TITLES =   50;

		/*
		 * Hold the titles of scrolls, 6 to 14 characters each, plus quotes.
		 */
		static string[] scroll_adj = new string[MAX_TITLES];

		/*
		 * Prepare the "variable" part of the "k_info" array.
		 *
		 * The "color"/"metal"/"type" of an item is its "flavor".
		 * For the most part, flavors are assigned randomly each game.
		 *
		 * Initialize descriptions for the "colored" objects, including:
		 * Rings, Amulets, Staffs, Wands, Rods, Food, Potions, Scrolls.
		 *
		 * The first 4 entries for potions are fixed (Water, Apple Juice,
		 * Slime Mold Juice, Unused Potion).
		 *
		 * Scroll titles are always between 6 and 14 letters long.  This is
		 * ensured because every title is composed of whole words, where every
		 * word is from 2 to 8 letters long, and that no scroll is finished
		 * until it attempts to grow beyond 15 letters.  The first time this
		 * can happen is when the current title has 6 letters and the new word
		 * has 8 letters, which would result in a 6 letter scroll title.
		 *
		 * Hack -- make sure everything stays the same for each saved game
		 * This is accomplished by the use of a saved "random seed", as in
		 * "town_gen()".  Since no other functions are called while the special
		 * seed is in effect, so this function is pretty "safe".
		 */
		public static void init()
		{
			int i, j;

			/* Hack -- Use the "simple" RNG */
			Random.Rand_quick = true;

			/* Hack -- Induce consistant flavors */
			Random.Rand_value = (uint)Misc.seed_flavor;

			flavor_assign_fixed();

			flavor_assign_random(TVal.TV_RING);
			flavor_assign_random(TVal.TV_AMULET);
			flavor_assign_random(TVal.TV_STAFF);
			flavor_assign_random(TVal.TV_WAND);
			flavor_assign_random(TVal.TV_ROD);
			flavor_assign_random(TVal.TV_FOOD);
			flavor_assign_random(TVal.TV_POTION);

			/* Scrolls (random titles, always white) */
			for (i = 0; i < MAX_TITLES; i++)
			{
				string buf = "\""; //26
				string end = "";
				int titlelen = 0;
				int wordlen;
				bool okay = true;

				wordlen = RandomName.randname_make(RandomName.randname_type.RANDNAME_SCROLL, 2, 8, ref end, 24, RandomName.name_sections);

				while (titlelen + wordlen < (int)(18 - 3)) //was scroll_adj[0].Length - 3, but we no longer have to worry about the null terminator
				{
					buf += end + " ";
				    //end[wordlen] = ' ';
				    titlelen += wordlen + 1;
				    //end += wordlen + 1;
					end = "";
				    wordlen = RandomName.randname_make(RandomName.randname_type.RANDNAME_SCROLL, 2, 8, ref end, 24 - titlelen, RandomName.name_sections);
				}
				buf += end + "\"";
				//buf[titlelen] = '"';
				//buf[titlelen+1] = '\0';

				/* Check the scroll name hasn't already been generated */
				for (j = 0; j < i; j++)
				{
				    if (buf == scroll_adj[j])
				    {
				        okay = false;
				        break;
				    }
				}

				if (okay)
				{
				    scroll_adj[i] = buf;
				}
				else
				{
				    /* Have another go at making a name */
				    i--;
				}
			}
			flavor_assign_random(TVal.TV_SCROLL);

			/* Hack -- Use the "complex" RNG */
			Random.Rand_quick = false;

			/* Analyze every object */
			for (i = 1; i < Misc.z_info.k_max; i++)
			{
				Object_Kind k_ptr = Misc.k_info[i];

				/* Skip "empty" objects */
				if (k_ptr == null || k_ptr.Name == null || k_ptr.Name.Length == 0) continue;

				/* No flavor yields aware */
				if (k_ptr.flavor == null) k_ptr.aware = true;
			}
		}

		
		static void flavor_assign_fixed()
		{
			for (Flavor f = Misc.flavors; f != null; f = f.next) {
				if (f.sval == SVal.SV_UNKNOWN)
					continue;

				for (int i = 0; i < Misc.z_info.k_max; i++) {
					Object_Kind k = Misc.k_info[i];
					if(k == null)
						continue;
					if (k.tval == f.tval && k.sval == f.sval)
						k.flavor = f;
				}
			}
		}


		static void flavor_assign_random(byte tval)
		{
			int flavor_count = 0;
			int choice;

			/* Count the random flavors for the given tval */
			for (Flavor f = Misc.flavors; f != null; f = f.next)
				if (f.tval == tval && f.sval == SVal.SV_UNKNOWN)
					flavor_count++;

			for (int i = 0; i < Misc.z_info.k_max; i++) {
				if(Misc.k_info[i] == null)
					continue;
				if (Misc.k_info[i].tval != tval || Misc.k_info[i].flavor != null)
					continue;

				/* HACK - Ordinary food is "boring" */
				if ((tval == TVal.TV_FOOD) && (Misc.k_info[i].sval < SVal.SV_FOOD_MIN_SHROOM))
					continue;

				if (flavor_count == 0)
					Utilities.quit("Not enough flavors for tval " + tval + ".");

				choice = Random.randint0(flavor_count);
	
				for (Flavor f = Misc.flavors; f != null; f = f.next) {
					if (f.tval != tval || f.sval != SVal.SV_UNKNOWN)
						continue;

					if (choice == 0) {
						Misc.k_info[i].flavor = f;
						f.sval = Misc.k_info[i].sval;
						if (tval == TVal.TV_SCROLL)
							f.text = scroll_adj[Misc.k_info[i].sval];
						flavor_count--;
						break;
					}

					choice--;
				}
			}
		}
	}
}
