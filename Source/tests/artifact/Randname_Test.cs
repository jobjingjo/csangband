using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Randname_Test : UnitTest {
		static int NAMES_TRIES = 100;

		static string[] names = new string[]{
			"aaaaaa",
			"bbbbbb",
			"cccccc",
			"dddddd",
			"eeeeee",
			"ffffff",
			"gggggg",
			"hhhhhh",
			"iiiiii",
			"jjjjjj"
		};

		static string[][] p = new string[][] { names, names };

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "artifact/randname";

			suite.NoSetup();
			suite.NoTeardown();


			suite.AddTest("names", test_names);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_names() {
			Object.Artifact a = new Object.Artifact();
			string n;

			a.aidx = 1;
			for (int i = 0; i < NAMES_TRIES; i++) {
				n = a.gen_name(p);
				if(n.Contains('\'')) {
					if(n.IndexOf('\'') == n.LastIndexOf('\'')) {
						Fail("A ' was opened and never closed");
					}
				} else {
					if(!n.Contains("of ")) {
						Fail("The unique name did not contain 'of' or any single quotes");
					}
				}
			}

			Ok();
		}
	}
}
