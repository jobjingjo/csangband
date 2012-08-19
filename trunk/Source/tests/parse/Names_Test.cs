using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Names_Test : UnitTest {
		static Parser p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "parse/names";

			suite.SetSetup(delegate() {
				p = Init.init_parse_names();
			});
			suite.SetTeardown(delegate() {
				p.Destroy();
			});


			suite.AddTest("n0", test_n0);
			suite.AddTest("d0", test_d0);
			suite.AddTest("n1", test_n1);
			suite.AddTest("d1", test_d1);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_n0() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "N:1");
			eq(r, 0);
			ok;*/
		}

		public static void test_d0() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "D:foo");
			eq(r, 0);
			r = parser_parse(state, "D:bar");
			eq(r, 0);
			ok;*/
		}

		public static void test_n1() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "N:2");
			eq(r, 0);
			ok;*/
		}

		public static void test_d1() {
			Fail("Write Later");
			/*
			errr r = parser_parse(state, "D:baz");
			eq(r, 0);
			r = parser_parse(state, "D:quxx");
			eq(r, 0);
			ok;*/
		}
	}
}
