using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests
{
	class ObjectAttack_Test : UnitTest
	{
		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "object/attack";

			suite.NoSetup();
			suite.NoTeardown();


			suite.AddTest("breakage-chance", test_breakage_chance);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_breakage_chance() {
			Object.Object obj = new Object.Object();
			int c;

			obj.prep(UnitTest_Data.Longsword(), 1, aspect.AVERAGE);
			c = obj.breakage_chance(true);
			Equal(c, 50);
			c = obj.breakage_chance(false);
			Equal(c, 25);
			obj.artifact = UnitTest_Data.Artifact_Sword();
			c = obj.breakage_chance(true);
			Equal(c, 0);
			c = obj.breakage_chance(false);
			Equal(c, 0);
			Ok();
		}
	}
}
