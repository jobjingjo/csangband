using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class P_Info_Test : UnitTest {
		static Parser p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "parse/p-info";

			suite.SetSetup(delegate() {
				p = Init.init_parse_p();
			});
			suite.SetTeardown(delegate() {
				p.Destroy();
			});

			suite.AddTest("n0", test_n0);
			suite.AddTest("s0", test_s0);
			suite.AddTest("r0", test_r0);
			suite.AddTest("x0", test_x0);
			suite.AddTest("i0", test_i0);
			suite.AddTest("h0", test_h0);
			suite.AddTest("w0", test_w0);
			suite.AddTest("f0", test_f0);
			suite.AddTest("y0", test_y0);
			suite.AddTest("c0", test_c0);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_n0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "N:1:Half-Elf");
			struct player_race *pr;

			eq(r, PARSE_ERROR_NONE);
			pr = parser_priv(state);
			require(pr);
			eq(pr.ridx, 1);
			require(streq(pr.name, "Half-Elf"));
			ok;*/
		}

		public static void test_s0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "S:1:-1:2:-2:3:-3");
			struct player_race *pr;

			eq(r, PARSE_ERROR_NONE);
			pr = parser_priv(state);
			require(pr);
			eq(pr.r_adj[A_STR], 1);
			eq(pr.r_adj[A_INT], -1);
			eq(pr.r_adj[A_WIS], 2);
			eq(pr.r_adj[A_DEX], -2);
			eq(pr.r_adj[A_CON], 3);
			eq(pr.r_adj[A_CHR], -3);
			ok;*/
		}

		public static void test_r0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "R:1:3:5:7:9:2:4:6:8:10");
			struct player_race *pr;

			eq(r, PARSE_ERROR_NONE);
			pr = parser_priv(state);
			require(pr);
			eq(pr.r_skills[SKILL_DISARM], 1);
			eq(pr.r_skills[SKILL_DEVICE], 3);
			eq(pr.r_skills[SKILL_SAVE], 5);
			eq(pr.r_skills[SKILL_STEALTH], 7);
			eq(pr.r_skills[SKILL_SEARCH], 9);
			eq(pr.r_skills[SKILL_SEARCH_FREQUENCY], 2);
			eq(pr.r_skills[SKILL_TO_HIT_MELEE], 4);
			eq(pr.r_skills[SKILL_TO_HIT_BOW], 6);
			eq(pr.r_skills[SKILL_TO_HIT_THROW], 8);
			eq(pr.r_skills[SKILL_DIGGING], 10);
			ok;*/
		}

		public static void test_x0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "X:10:20:80");
			struct player_race *pr;

			eq(r, PARSE_ERROR_NONE);
			pr = parser_priv(state);
			require(pr);
			eq(pr.r_mhp, 10);
			eq(pr.r_exp, 20);
			eq(pr.infra, 80);
			ok;*/
		}

		public static void test_i0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "I:0:10:3");
			struct player_race *pr;

			eq(r, PARSE_ERROR_NONE);
			pr = parser_priv(state);
			require(pr);
			ptreq(pr.history, null);
			eq(pr.b_age, 10);
			eq(pr.m_age, 3);
			ok;*/
		}

		public static void test_h0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "H:10:2:11:3");
			struct player_race *pr;

			eq(r, PARSE_ERROR_NONE);
			pr = parser_priv(state);
			require(pr);
			eq(pr.m_b_ht, 10);
			eq(pr.m_m_ht, 2);
			eq(pr.f_b_ht, 11);
			eq(pr.f_m_ht, 3);
			ok;*/
		}

		public static void test_w0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "W:80:10:75:7");
			struct player_race *pr;

			eq(r, PARSE_ERROR_NONE);
			pr = parser_priv(state);
			require(pr);
			eq(pr.m_b_wt, 80);
			eq(pr.m_m_wt, 10);
			eq(pr.f_b_wt, 75);
			eq(pr.f_m_wt, 7);
			ok;*/
		}

		public static void test_f0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "F:SUST_DEX");
			struct player_race *pr;

			eq(r, PARSE_ERROR_NONE);
			pr = parser_priv(state);
			require(pr);
			require(pr.flags);
			ok;*/
		}

		public static void test_y0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "Y:KNOW_ZAPPER");
			struct player_race *pr;

			eq(r, PARSE_ERROR_NONE);
			pr = parser_priv(state);
			require(pr);
			require(pr.pflags);
			ok;*/
		}

		public static void test_c0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "C:1|3|5");
			struct player_race *pr;

			eq(r, PARSE_ERROR_NONE);
			pr = parser_priv(state);
			require(pr);
			eq(pr.choice, (1 << 5) | (1 << 3) | (1 << 1));
			ok;*/
		}
	}
}
