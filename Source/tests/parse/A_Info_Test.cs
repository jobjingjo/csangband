using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class A_Info_Test : UnitTest {
		static Parser p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "parse/a-info";

			suite.SetSetup(delegate() {
				p = Init.init_parse_a();
			});
			suite.SetTeardown(delegate() {
				p.Destroy();
			});

			suite.AddTest("n0", test_n0);
			suite.AddTest("badtval0", test_badtval0);
			suite.AddTest("badtval1", test_badtval1);
			suite.AddTest("badsval0", test_badsval);
			suite.AddTest("badsval1", test_badsval1);
			suite.AddTest("i0", test_i0);
			suite.AddTest("w0", test_w0);
			suite.AddTest("a0", test_a0);
			suite.AddTest("a1", test_a1);
			suite.AddTest("a2", test_a2);
			suite.AddTest("p0", test_p0);
			suite.AddTest("f0", test_f0);
			suite.AddTest("e0", test_e0);
			suite.AddTest("m0", test_m0);
			suite.AddTest("d0", test_d0);
			suite.AddTest("l0", test_l0);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_n0() {
			Fail("Write Later");
			/*
			enum parser_error r = parser_parse(state, "N:3:of Thrain");
			struct artifact *a;

			eq(r, PARSE_ERROR_NONE);
			a = parser_priv(state);
			require(a);
			eq(a.aidx, 3);
			require(streq(a.name, "of Thrain"));
			ok;*/
		}

		public static void test_badtval0() {
			Fail("Write Later");
			/*
			enum parser_error r = parser_parse(state, "I:badtval:6:3");
			eq(r, PARSE_ERROR_UNRECOGNISED_TVAL);
			ok;*/
		}

		public static void test_badtval1() {
			Fail("Write Later");
			/*
			enum parser_error r = parser_parse(state, "I:-1:6:3");
			eq(r, PARSE_ERROR_UNRECOGNISED_TVAL);
			ok;*/
		}

		/* Causes segfault: lookup_sval() requires z_info/k_info */
		public static void test_badsval() {
			Fail("Write Later");
			/*
			errr r = parser_parse(state, "I:light:badsval:3");
			eq(r, PARSE_ERROR_UNRECOGNISED_SVAL);
			ok;*/
		}

		public static void test_badsval1() {
			Fail("Write Later");
			/*
			enum parser_error r = parser_parse(state, "I:light:-2:3");
			eq(r, PARSE_ERROR_UNRECOGNISED_SVAL);
			ok;*/
		}

		public static void test_i0() {
			Fail("Write Later");
			/*
			enum parser_error r = parser_parse(state, "I:light:6");
			struct artifact *a;

			eq(r, PARSE_ERROR_NONE);
			a = parser_priv(state);
			require(a);
			eq(a.tval, TV_LIGHT);
			eq(a.sval, 6);
			ok;*/
		}

		public static void test_w0() {
			Fail("Write Later");
			/*
			enum parser_error r = parser_parse(state, "W:3:5:8:200");
			struct artifact *a;

			eq(r, PARSE_ERROR_NONE);
			a = parser_priv(state);
			require(a);
			eq(a.level, 3);
			eq(a.rarity, 5);
			eq(a.weight, 8);
			eq(a.cost, 200);
			ok;*/
		}

		public static void test_a0() {
			Fail("Write Later");
			/*
			enum parser_error r = parser_parse(state, "A:3:5");
			eq(r, PARSE_ERROR_GENERIC);
			ok;*/
		}

		public static void test_a1() {
			Fail("Write Later");
			/*
			enum parser_error r = parser_parse(state, "A:3:5 to 300");
			eq(r, PARSE_ERROR_OUT_OF_BOUNDS);
			ok;*/
		}

		public static void test_a2() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "A:3:5 to 10");
			struct artifact *a;

			eq(r, PARSE_ERROR_NONE);
			a = parser_priv(state);
			require(a);
			eq(a.alloc_prob, 3);
			eq(a.alloc_min, 5);
			eq(a.alloc_max, 10);
			ok;*/
		}

		public static void test_p0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "P:3:4d5:8:2:1");
			struct artifact *a;

			eq(r, PARSE_ERROR_NONE);
			a = parser_priv(state);
			require(a);
			eq(a.ac, 3);
			eq(a.dd, 4);
			eq(a.ds, 5);
			eq(a.to_h, 8);
			eq(a.to_d, 2);
			eq(a.to_a, 1);
			ok;*/
		}

		public static void test_f0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "F:SEE_INVIS | HOLD_LIFE");
			struct artifact *a;

			eq(r, PARSE_ERROR_NONE);
			a = parser_priv(state);
			require(a);
			require(a.flags);
			ok;*/
		}

		public static void test_l0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "L:17:STR | CON");
			struct artifact *a;

			eq(r, PARSE_ERROR_NONE);
			a = parser_priv(state);
			eq(a.pval[0], 17);
			require(a.pval_flags[0]);
			ok;*/
		}

		public static void test_e0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "E:DETECT_ALL:20+d30");
			struct artifact *a;

			eq(r, PARSE_ERROR_NONE);
			a = parser_priv(state);
			require(a);
			require(a.effect);
			eq(a.time.base, 20);
			eq(a.time.sides, 30);
			ok;*/
		}

		public static void test_m0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "M:foo");
			struct artifact *a;

			eq(r, 0);
			r = parser_parse(state, "M:bar");
			eq(r, 0);
			a = parser_priv(state);
			require(a);
			require(streq(a.effect_msg, "foobar"));
			ok;*/
		}

		public static void test_d0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "D:baz");
			struct artifact *a;

			eq(r, 0);
			r = parser_parse(state, "D: quxx");
			eq(r, 0);
			a = parser_priv(state);
			require(a);
			require(streq(a.text, "baz quxx"));
			ok;*/
		}
	}
}
