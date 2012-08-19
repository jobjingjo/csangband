using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Flavor_Test : UnitTest {
		static Parser p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "parse/flavor";

			suite.SetSetup(delegate() {
				p = Init.init_parse_flavor();
			});
			suite.SetTeardown(delegate() {
				p.Destroy();
			});


			suite.AddTest("n0", test_n0);
			suite.AddTest("n1", test_n1);
			suite.AddTest("g0", test_g0);
			suite.AddTest("d0", test_d0);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_n0() {
			Fail("Write Later");
			/*
			enum parser_error r = parser_parse(state, "N:1:3:5");
			struct flavor *f;

			eq(r, PARSE_ERROR_NONE);
			f = parser_priv(state);
			require(f);
			eq(f.fidx, 1);
			eq(f.tval, 3);
			eq(f.sval, 5);
			ok;*/
		}

		public static void test_n1() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "N:2:light");
			struct flavor *f;

			eq(r, PARSE_ERROR_NONE);
			f = parser_priv(state);
			require(f);
			eq(f.fidx, 2);
			eq(f.tval, TV_LIGHT);
			eq(f.sval, SV_UNKNOWN);
			ok;*/
		}

		public static void test_g0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "G:!:y");
			struct flavor *f;

			eq(r, PARSE_ERROR_NONE);
			f = parser_priv(state);
			require(f);
			eq(f.fidx, 2);
			eq(f.d_char, '!');
			eq(f.d_attr, TERM_YELLOW);
			ok;*/
		}

		public static void test_d0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "D:foo");
			struct flavor *f;

			eq(r, PARSE_ERROR_NONE);
			r = parser_parse(state, "D: bar");
			eq(r, PARSE_ERROR_NONE);
			f = parser_priv(state);
			require(f);
			require(streq(f.text, "foo bar"));
			eq(f.fidx, 2);
			ok;*/
		}
	}
}
