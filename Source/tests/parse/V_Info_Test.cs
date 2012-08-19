using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class V_Info_Test : UnitTest {
		static Parser p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "parse/v-info";

			suite.SetSetup(delegate() {
				p = Init.init_parse_v();
			});
			suite.SetTeardown(delegate() {
				p.Destroy();
			});

			suite.AddTest("n0", test_n0);
			suite.AddTest("x0", test_x0);
			suite.AddTest("d0", test_d0);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_n0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "N:1:round");
			struct vault *v;

			eq(r, PARSE_ERROR_NONE);
			v = parser_priv(state);
			require(v);
			eq(v.vidx, 1);
			require(streq(v.name, "round"));
			ok;*/
		}

		public static void test_x0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "X:6:5:12:20");
			struct vault *v;

			eq(r, PARSE_ERROR_NONE);
			v = parser_priv(state);
			require(v);
			eq(v.typ, 6);
			eq(v.rat, 5);
			eq(v.hgt, 12);
			eq(v.wid, 20);
			ok;*/
		}

		public static void test_d0() {
			Fail("Write Later");
			/*enum parser_error r0 = parser_parse(state, "D:  %%  ");
			enum parser_error r1 = parser_parse(state, "D: %  % ");
			struct vault *v;

			eq(r0, PARSE_ERROR_NONE);
			eq(r1, PARSE_ERROR_NONE);
			v = parser_priv(state);
			require(v);
			require(streq(v.text, "  %%   %  % "));
			ok;*/
		}
	}
}
