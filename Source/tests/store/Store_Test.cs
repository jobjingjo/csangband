using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Store_Test : UnitTest {
		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "store/store";

			suite.SetSetup(delegate() {
				UnitTest_Utils.read_edit_files();
			});
			suite.NoTeardown();


			suite.AddTest("Enough items in Armoury", test_enough_armor);
			suite.AddTest("Enough items in Weaponsmith", test_enough_weapons);
			suite.AddTest("Enough items in Temple", test_enough_temple);
			suite.AddTest("Enough items in Alchemists", test_enough_alchemy);
			suite.AddTest("Enough items in Magicians", test_enough_magic);

			UnitTest_Main.AddSuite(suite);
		}

		static int number_distinct_items(STORE which)
		{
			throw new NotImplementedException();
			/*Store s = Misc.stores[(int)which];
			u32b *kinds = mem_zalloc(sizeof(u32b) * s.table_num);
			u32b kind;
			int number_kinds = 0;

			// Loop over available stock entries, counting unique kinds
			for (int i = 0; i < s.table_num; i++)
			{
				unsigned int j;

				kind = s.table[i].kidx;
				// Loop over existing found kinds, and skip out early if it's
				// been seen already
				for (j = 0; j < i; j++)
				{
					if (kinds[j] == kind)
						break;
				}
				// If we've run off the end of the loop, we've not seen it before,
				// so count it
				if (j == i) {
					kinds[j] = kind;
					number_kinds++;
				}
			}
			return number_kinds;*/
		}


		public static void test_enough_armor() {
			Require(number_distinct_items(STORE.ARMOR) >= Store.MAX_KEEP);
			Ok();
		}

		public static void test_enough_weapons() {
			Require(number_distinct_items(STORE.WEAPON) >= Store.MAX_KEEP);
			Ok();
		}

		public static void test_enough_temple() {
			Require(number_distinct_items(STORE.TEMPLE) >= Store.MAX_KEEP);
			Ok();
		}

		public static void test_enough_alchemy() {
			Require(number_distinct_items(STORE.ALCHEMY) >= Store.MAX_KEEP);
			Ok();
		}

		public static void test_enough_magic() {
			Require(number_distinct_items(STORE.MAGIC) >= Store.MAX_KEEP);
			Ok();
		}
	}
}
