using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class F_Info_Test : UnitTest {
		static Parser p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "parse/f-info";

			suite.SetSetup(delegate() {
				p = Init.init_parse_f();
			});
			suite.SetTeardown(delegate() {
				p.Destroy();
			});

			suite.AddTest("n0", test_n0);
			suite.AddTest("g0", test_g0);
			suite.AddTest("m0", test_m0);
			suite.AddTest("p0", test_p0);
			suite.AddTest("f0", test_f0);
			suite.AddTest("x0", test_x0);
			suite.AddTest("e0", test_e0);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_n0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "N:3:Test Feature");
			struct feature *f;

			eq(r, PARSE_ERROR_NONE);
			f = parser_priv(state);
			require(f);
			require(streq(f.name, "Test Feature"));
			eq(f.fidx, 3);
			eq(f.mimic, 3);
			ok;*/
		}

		public static void test_g0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "G:::red");
			struct feature *f;

			eq(r, PARSE_ERROR_NONE);
			f = parser_priv(state);
			require(f);
			eq(f.d_char, ':');
			eq(f.d_attr, TERM_RED);
			ok;*/
		}

		public static void test_m0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "M:11");
			struct feature *f;

			eq(r, PARSE_ERROR_NONE);
			f = parser_priv(state);
			require(f);
			eq(f.mimic, 11);
			ok;*/
		}

		public static void test_p0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "P:2");
			struct feature *f;

			eq(r, PARSE_ERROR_NONE);
			f = parser_priv(state);
			require(f);
			eq(f.priority, 2);
			ok;*/
		}

		public static void test_f0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "F:MWALK | LOOK");
			struct feature *f;

			eq(r, PARSE_ERROR_NONE);
			f = parser_priv(state);
			require(f);
			require(f.flags);
			ok;*/
		}

		public static void test_x0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "X:3:5:9:2");
			struct feature *f;

			eq(r, PARSE_ERROR_NONE);
			f = parser_priv(state);
			require(f);
			eq(f.locked, 3);
			eq(f.jammed, 5);
			eq(f.shopnum, 9);
			eq(f.dig, 2);
			ok;*/
		}

		public static void test_e0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "E:TRAP_PIT");
			struct feature *f;

			eq(r, PARSE_ERROR_NONE);
			f = parser_priv(state);
			require(f);
			require(f.effect);
			ok;*/
		}
	}
}
