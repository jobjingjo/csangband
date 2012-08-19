using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class K_Info_Test : UnitTest {
		static Parser p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "parse/k-info";

			suite.SetSetup(delegate() {
				p = Init.init_parse_k();
			});
			suite.SetTeardown(delegate() {
				p.Destroy();
			});

			suite.AddTest("n0", test_n0);
			suite.AddTest("g0", test_g0);
			suite.AddTest("g1", test_g1);
			suite.AddTest("i0", test_i0);
			suite.AddTest("i1", test_i1);
			suite.AddTest("w0", test_w0);
			suite.AddTest("a0", test_a0);
			suite.AddTest("p0", test_p0);
			suite.AddTest("c0", test_c0);
			suite.AddTest("m0", test_m0);
			suite.AddTest("f0", test_f0);
			suite.AddTest("e0", test_e0);
			suite.AddTest("d0", test_d0);
			suite.AddTest("l0", test_l0);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_n0() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "N:3:Test Object Kind");
			struct object_kind *k;

			eq(r, 0);
			k = parser_priv(state);
			require(k);
			eq(k.kidx, 3);
			require(streq(k.name, "Test Object Kind"));
			ok;*/
		}

		public static void test_g0() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "G:~:red");
			struct object_kind *k;

			eq(r, 0);
			k = parser_priv(state);
			require(k);
			eq(k.d_char, '~');
			eq(k.d_attr, TERM_RED);
			ok;*/
		}

		public static void test_g1() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "G:!:W");
			struct object_kind *k;

			eq(r, 0);
			k = parser_priv(state);
			require(k);
			eq(k.d_char, '!');
			eq(k.d_attr, TERM_L_WHITE);
			ok;*/
		}

		public static void test_i0() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "I:4:2");
			struct object_kind *k;

			eq(r, 0);
			k = parser_priv(state);
			require(k);
			eq(k.tval, 4);
			eq(k.sval, 2);
			ok;*/
		}

		public static void test_i1() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "I:food:2");
			struct object_kind *k;

			eq(r, 0);
			k = parser_priv(state);
			require(k);
			eq(k.tval, TV_FOOD);
			eq(k.sval, 2);
			ok;*/
		}

		public static void test_w0() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "W:10:0:5:120");
			struct object_kind *k;

			eq(r, 0);
			k = parser_priv(state);
			require(k);
			eq(k.level, 10);
			eq(k.weight, 5);
			eq(k.cost, 120);
			ok;*/
		}

		public static void test_a0() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "A:3:4 to 6");
			struct object_kind *k;

			eq(r, 0);
			k = parser_priv(state);
			require(k);
			eq(k.alloc_prob, 3);
			eq(k.alloc_min, 4);
			eq(k.alloc_max, 6);
			ok;*/
		}

		public static void test_p0() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "P:3:4d8:1d4:2d5:7d6");
			struct object_kind *k;

			eq(r, 0);
			k = parser_priv(state);
			require(k);
			eq(k.ac, 3);
			eq(k.dd, 4);
			eq(k.ds, 8);
			eq(k.to_h.dice, 1);
			eq(k.to_h.sides, 4);
			eq(k.to_d.dice, 2);
			eq(k.to_d.sides, 5);
			eq(k.to_a.dice, 7);
			eq(k.to_a.sides, 6);
			ok;*/
		}

		public static void test_c0() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "C:2d8");
			struct object_kind *k;

			eq(r, 0);
			k = parser_priv(state);
			require(k);
			eq(k.charge.dice, 2);
			eq(k.charge.sides, 8);
			ok;*/
		}

		public static void test_m0() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "M:4:3d6");
			struct object_kind *k;

			eq(r, 0);
			k = parser_priv(state);
			require(k);
			eq(k.gen_mult_prob, 4);
			eq(k.stack_size.dice, 3);
			eq(k.stack_size.sides, 6);
			ok;*/
		}

		public static void test_f0() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "F:STR");
			struct object_kind *k;

			eq(r, 0);
			k = parser_priv(state);
			require(k);
			require(k.flags);
			ok;*/
		}

		public static void test_l0() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "L:1+2d3M4:STR | INT");
			struct object_kind *k;

			eq(r, 0);
			k = parser_priv(state);
			require(k);
			eq(k.pval[0].base, 1);
			eq(k.pval[0].dice, 2);
			eq(k.pval[0].sides, 3);
			eq(k.pval[0].m_bonus, 4);
			require(k.pval_flags[0]);
			ok;*/
		}

		public static void test_e0() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "E:POISON:4d5");
			struct object_kind *k;

			eq(r, 0);
			k = parser_priv(state);
			require(k);
			require(k.effect);
			eq(k.time.dice, 4);
			eq(k.time.sides, 5);
			ok;*/
		}

		public static void test_d0() {
			Fail("Write Later");
			/*errr r = parser_parse(state, "D:foo bar");
			struct object_kind *k;

			eq(r, 0);
			k = parser_priv(state);
			require(k);
			require(k.text);
			require(streq(k.text, "foo bar"));
			r = parser_parse(state, "D: baz");
			eq(r, 0);
			ptreq(k, parser_priv(state));
			require(streq(k.text, "foo bar baz"));
			ok;*/
		}
	}
}
