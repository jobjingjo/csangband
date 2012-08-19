using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Owner_Test : UnitTest {
		static Store s0;
		static Store s1;
		static Parser p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "parse/owner";

			suite.SetSetup(delegate() {
				throw new NotImplementedException();
				/*s0.next = &s1;
				s1.next = null;
				s0.sidx = 0;
				s1.sidx = 1;
				*state = store_owner_parser_new(&s0);*/
			});
			suite.SetTeardown(delegate() {
				p.Destroy();
			});

			suite.AddTest("n0", test_n0);
			suite.AddTest("s0", test_s0);
			suite.AddTest("s1", test_s1);
			suite.AddTest("n1", test_n1);
			suite.AddTest("s2", test_s2);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_n0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "N:0");

			eq(r, PARSE_ERROR_NONE);
			require(parser_priv(state));
			ok;*/
		}

		public static void test_s0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "S:5000:Foo");

			eq(r, PARSE_ERROR_NONE);
			eq(s0.owners.max_cost, 5000);
			require(streq(s0.owners.name, "Foo"));
			ok;*/
		}

		public static void test_s1() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "S:10000:Bar");

			eq(r, PARSE_ERROR_NONE);
			eq(s0.owners.max_cost, 10000);
			require(streq(s0.owners.name, "Bar"));
			ok;*/
		}

		public static void test_n1() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "N:1");

			eq(r, PARSE_ERROR_NONE);
			require(parser_priv(state));
			ok;*/
		}

		public static void test_s2() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "S:15000:Baz");

			eq(r, PARSE_ERROR_NONE);
			eq(s1.owners.max_cost, 15000);
			require(streq(s1.owners.name, "Baz"));
			ok;*/
		}
	}
}
