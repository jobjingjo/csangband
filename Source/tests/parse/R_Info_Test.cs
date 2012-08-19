using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class R_Info_Test : UnitTest {
		static Parser p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "parse/r-info";

			suite.SetSetup(delegate() {
				p = Init.init_parse_r();
			});
			suite.SetTeardown(delegate() {
				p.Destroy();
			});

			suite.AddTest("n0", test_n0 );
			suite.AddTest("c0", test_c0 );
			suite.AddTest("t0", test_t0 );
			suite.AddTest("i0", test_i0 );
			suite.AddTest("w0", test_w0 );
			suite.AddTest("b0", test_b0 );
			suite.AddTest("b1", test_b1 );
			suite.AddTest("f0", test_f0 );
			suite.AddTest("d0", test_d0 );
			suite.AddTest("s0", test_s0 );

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_n0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "N:544:Carcharoth, the Jaws of Thirst");
			struct monster_race *mr;

			eq(r, PARSE_ERROR_NONE);
			mr = parser_priv(state);
			require(mr);
			eq(mr.ridx, 544);
			require(streq(mr.name, "Carcharoth, the Jaws of Thirst"));
			ok;*/
		}

		public static void test_t0() {
			Fail("Write Later");
			/*enum parser_error r;
			struct monster_race *mr;

			rb_info = &test_rb_info;
			r = parser_parse(state, "T:townsfolk");
			eq(r, PARSE_ERROR_NONE);
			mr = parser_priv(state);
			require(mr);
			require(streq(mr.base.name, "townsfolk"));
			ok;*/
		}

		public static void test_c0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "C:v");
			struct monster_race *mr;

			eq(r, PARSE_ERROR_NONE);
			mr = parser_priv(state);
			require(mr);
			eq(mr.d_attr, TERM_VIOLET);
			ok;*/
		}

		public static void test_i0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "I:7:500:80:22:3");
			struct monster_race *mr;

			eq(r, PARSE_ERROR_NONE);
			mr = parser_priv(state);
			require(mr);
			eq(mr.speed, 7);
			eq(mr.avg_hp, 500);
			eq(mr.aaf, 80);
			eq(mr.ac, 22);
			eq(mr.sleep, 3);
			ok;*/
		}

		public static void test_w0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "W:42:11:27:4");
			struct monster_race *mr;

			eq(r, PARSE_ERROR_NONE);
			mr = parser_priv(state);
			require(mr);
			eq(mr.level, 42);
			eq(mr.rarity, 11);
			eq(mr.power, 27);
			eq(mr.mexp, 4);
			ok;*/
		}

		public static void test_b0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "B:CLAW:FIRE:9d12");
			struct monster_race *mr;

			eq(r, PARSE_ERROR_NONE);
			mr = parser_priv(state);
			require(mr);
			require(mr.blow[0].method);
			require(mr.blow[0].effect);
			eq(mr.blow[0].d_dice, 9);
			eq(mr.blow[0].d_side, 12);
			ok;*/
		}

		public static void test_b1() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "B:BITE:FIRE:6d8");
			struct monster_race *mr;

			eq(r, PARSE_ERROR_NONE);
			mr = parser_priv(state);
			require(mr);
			require(mr.blow[1].method);
			require(mr.blow[1].effect);
			eq(mr.blow[1].d_dice, 6);
			eq(mr.blow[1].d_side, 8);
			ok;*/
		}

		public static void test_f0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "F:UNIQUE | MALE");
			struct monster_race *mr;

			eq(r, PARSE_ERROR_NONE);
			mr = parser_priv(state);
			require(mr);
			require(mr.flags);
			ok;*/
		}

		public static void test_d0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "D:foo bar ");
			enum parser_error s = parser_parse(state, "D: baz");
			struct monster_race *mr;

			eq(r, PARSE_ERROR_NONE);
			eq(s, PARSE_ERROR_NONE);
			mr = parser_priv(state);
			require(mr);
			require(streq(mr.text, "foo bar  baz"));
			ok;*/
		}

		public static void test_s0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "S:1_IN_4 | BR_DARK | S_HOUND");
			struct monster_race *mr;

			eq(r, PARSE_ERROR_NONE);
			mr = parser_priv(state);
			require(mr);
			eq(mr.freq_spell, 25);
			eq(mr.freq_innate, 25);
			require(mr.spell_flags);
			ok;*/
		}
	}
}
