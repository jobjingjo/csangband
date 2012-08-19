using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class E_Info_test : UnitTest {
		static Parser p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "parse/e-info";

			suite.SetSetup(delegate() {
				p = Init.init_parse_e();
			});
			suite.SetTeardown(delegate() {
				p.Destroy();
			});

			suite.AddTest("order", test_order);
			suite.AddTest("n0", test_n0);
			suite.AddTest("w0", test_w0);
			suite.AddTest("x0", test_x0);
			suite.AddTest("t0", test_t0);
			suite.AddTest("t1", test_t1);
			suite.AddTest("c0", test_c0);
			suite.AddTest("m0", test_m0);
			suite.AddTest("f0", test_f0);
			suite.AddTest("d0", test_d0);
			suite.AddTest("l0", test_l0);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_order() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "X:3:4");
			eq(r, PARSE_ERROR_MISSING_RECORD_HEADER);
			ok;*/
		}

		public static void test_n0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "N:5:of Resist Lightning");
			struct ego_item *e;

			eq(r, PARSE_ERROR_NONE);
			e = parser_priv(state);
			require(e);
			eq(e.eidx, 5);
			require(streq(e.name, "of Resist Lightning"));
			ok;*/
		}

		public static void test_w0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "W:2:4:6:8");
			struct ego_item *e;

			eq(r, PARSE_ERROR_NONE);
			e = parser_priv(state);
			require(e);
			eq(e.level, 2);
			eq(e.rarity, 4);
			eq(e.cost, 8);
			return PARSE_ERROR_NONE;*/
		}

		public static void test_x0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "X:5:1");
			struct ego_item *e;

			eq(r, PARSE_ERROR_NONE);
			e = parser_priv(state);
			require(e);
			eq(e.rating, 5);
			eq(e.xtra, 1);
			ok;*/
		}

		public static void test_t0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "T:22:2:13");
			struct ego_item *e;

			eq(r, PARSE_ERROR_NONE);
			e = parser_priv(state);
			require(e);
			eq(e.tval[0], 22);
			eq(e.min_sval[0], 2);
			eq(e.max_sval[0], 13);
			ok;*/
		}

		/* Broken: lookup_sval() requires k_info, z_info */
		public static void test_t1() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "T:sword:dagger:scimitar");
			struct ego_item *e;

			eq(r, PARSE_ERROR_NONE);
			e = parser_priv(state);
			require(e);
			eq(e.tval[1], TV_SWORD);
			eq(e.min_sval[1], SV_DAGGER);
			eq(e.max_sval[1], SV_SCIMITAR);
			ok;*/
		}

		public static void test_c0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "C:1d2:3d4:5d6");
			struct ego_item *e;

			eq(r, PARSE_ERROR_NONE);
			e = parser_priv(state);
			require(e);
			eq(e.to_h.dice, 1);
			eq(e.to_h.sides, 2);
			eq(e.to_d.dice, 3);
			eq(e.to_d.sides, 4);
			eq(e.to_a.dice, 5);
			eq(e.to_a.sides, 6);
			ok;*/
		}

		public static void test_l0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "L:1+2d3M4:5:STR | INT");
			struct ego_item *e;

			eq(r, PARSE_ERROR_NONE);
			e = parser_priv(state);
			require(e);
			eq(e.pval[0].base, 1);
			eq(e.pval[0].dice, 2);
			eq(e.pval[0].sides, 3);
			eq(e.pval[0].m_bonus, 4);
			eq(e.min_pval[0], 5);
			require(e.pval_flags[0]);
			ok;*/
		}

		public static void test_m0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "M:10:13:4");
			struct ego_item *e;

			eq(r, PARSE_ERROR_NONE);
			e = parser_priv(state);
			require(e);
			eq(e.min_to_h, 10);
			eq(e.min_to_d, 13);
			eq(e.min_to_a, 4);
			ok;*/
		}

		public static void test_f0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "F:SEE_INVIS");
			struct ego_item *e;

			eq(r, PARSE_ERROR_NONE);
			e = parser_priv(state);
			require(e);
			require(e.flags);
			ok;*/
		}

		public static void test_d0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "D:foo");
			struct ego_item *e;
			eq(r, PARSE_ERROR_NONE);
			r = parser_parse(state, "D: bar");
			eq(r, PARSE_ERROR_NONE);

			e = parser_priv(state);
			require(e);
			require(streq(e.text, "foo bar"));
			ok;*/
		}
	}
}
