using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Birth_Test : UnitTest {
		static Player.Player p;
		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "player/birth";

			suite.SetSetup(delegate() {
				p = new Player.Player();
			});
			suite.NoTeardown();


			suite.AddTest("generate0", test_generate0);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_generate0() {
			p.generate(UnitTest_Data.Player_Sex(), UnitTest_Data.Race(), UnitTest_Data.Class());
			Equal(p.lev, 1);
			Equal(p.mhp, 19);
			//Comparing these means that the info didn't really change... we need to check things more specifically
			/*Equal(p.Sex, UnitTest_Data.Player_Sex());
			Equal(p.Race, UnitTest_Data.Race());
			Equal(p.Class, UnitTest_Data.Class());*/
			Ok();
		}
	}
}
