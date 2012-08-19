using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Trivial_Test : UnitTest {
		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "trivial";

			suite.NoSetup();
			suite.NoTeardown();


			suite.AddTest("OK", test_empty);
			suite.AddTest("Require", test_require);
			suite.AddTest("Equal", test_equal);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_empty() {
			Ok();
		}

		public static void test_require() {
			Require(1 == 1);
			Ok();
		}

		public static void test_equal() {
			Equal(7, 7);
			Equal("Mike", "Mike");
			Ok();
		}
	}
}
