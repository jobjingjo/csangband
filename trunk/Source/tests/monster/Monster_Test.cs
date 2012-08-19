using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Monster_Test : UnitTest {
		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "monster/monster";

			suite.SetSetup(delegate() {
				UnitTest_Utils.read_edit_files();
			});
			suite.NoTeardown();


			suite.AddTest("match_monster_bases", test_match_monster_bases);

			UnitTest_Main.AddSuite(suite);
		}

		/* Regression test for #1409 */
		public static void test_match_monster_bases() {
			Monster.Monster_Base Base;

			/* Scruffy little dog */
			Base = Misc.r_info[3].Base;
			Require(Base.match_monster_bases("canine"));
			Require(Base.match_monster_bases("zephyr hound", "canine"));
			Require(!Base.match_monster_bases("angel"));
			Require(!Base.match_monster_bases("lich", "vampire", "wraith"));

			/* Morgoth */
			Base = Misc.r_info[547].Base;
			Require(!Base.match_monster_bases("canine"));
			Require(!Base.match_monster_bases("lich", "vampire", "wraith"));
			Require(Base.match_monster_bases("person", "Morgoth"));
			Require(Base.match_monster_bases("Morgoth"));

			Ok();
		}
	}
}
