using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class C_Info_Test : UnitTest {
		static Parser p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "parse/c-info";

			suite.SetSetup(delegate() {
				p = Init.init_parse_c();
			});
			suite.SetTeardown(delegate() {
				p.Destroy();
			});

			suite.AddTest("n0", test_n0 );
			suite.AddTest("s0", test_s0 );
			suite.AddTest("c0", test_c0 );
			suite.AddTest("x0", test_x0 );
			suite.AddTest("i0", test_i0 );
			suite.AddTest("a0", test_a0 );
			suite.AddTest("m0", test_m0 );
			suite.AddTest("b0", test_b0 );
			suite.AddTest("t0", test_t0 );
			suite.AddTest("e0", test_e0 );
			suite.AddTest("f0", test_f0 );

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_n0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "N:4:Ranger");
			struct player_class *c;

			eq(r, PARSE_ERROR_NONE);
			c = parser_priv(state);
			require(c);
			eq(c.cidx, 4);
			require(streq(c.name, "Ranger"));
			ok;*/
		}

		public static void test_s0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "S:3:-3:2:-2:1:-1");
			struct player_class *c;

			eq(r, PARSE_ERROR_NONE);
			c = parser_priv(state);
			require(c);
			eq(c.c_adj[A_STR], 3);
			eq(c.c_adj[A_INT], -3);
			eq(c.c_adj[A_WIS], 2);
			eq(c.c_adj[A_DEX], -2);
			eq(c.c_adj[A_CON], 1);
			eq(c.c_adj[A_CHR], -1);
			ok;*/
		}

		public static void test_c0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "C:30:32:28:3:24:16:56:72:72:0");
			struct player_class *c;

			eq(r, PARSE_ERROR_NONE);
			c = parser_priv(state);
			require(c);
			eq(c.c_skills[SKILL_DISARM], 30);
			eq(c.c_skills[SKILL_DEVICE], 32);
			eq(c.c_skills[SKILL_SAVE], 28);
			eq(c.c_skills[SKILL_STEALTH], 3);
			eq(c.c_skills[SKILL_SEARCH], 24);
			eq(c.c_skills[SKILL_SEARCH_FREQUENCY], 16);
			eq(c.c_skills[SKILL_TO_HIT_MELEE], 56);
			eq(c.c_skills[SKILL_TO_HIT_BOW], 72);
			eq(c.c_skills[SKILL_TO_HIT_THROW], 72);
			eq(c.c_skills[SKILL_DIGGING], 0);
			ok;*/
		}

		public static void test_x0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "X:8:10:10:0:0:0:30:45:45:0");
			struct player_class *c;

			eq(r, PARSE_ERROR_NONE);
			c = parser_priv(state);
			require(c);
			eq(c.x_skills[SKILL_DISARM], 8);
			eq(c.x_skills[SKILL_DEVICE], 10);
			eq(c.x_skills[SKILL_SAVE], 10);
			eq(c.x_skills[SKILL_STEALTH], 0);
			eq(c.x_skills[SKILL_SEARCH], 0);
			eq(c.x_skills[SKILL_SEARCH_FREQUENCY], 0);
			eq(c.x_skills[SKILL_TO_HIT_MELEE], 30);
			eq(c.x_skills[SKILL_TO_HIT_BOW], 45);
			eq(c.x_skills[SKILL_TO_HIT_THROW], 45);
			eq(c.x_skills[SKILL_DIGGING], 0);
			ok;*/
		}

		public static void test_i0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "I:4:30:20000:40");
			struct player_class *c;

			eq(r, PARSE_ERROR_NONE);
			c = parser_priv(state);
			require(c);
			eq(c.c_mhp, 4);
			eq(c.c_exp, 30);
			eq(c.sense_base, 20000);
			eq(c.sense_div, 40);
			ok;*/
		}

		public static void test_a0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "A:5:35:4");
			struct player_class *c;

			eq(r, PARSE_ERROR_NONE);
			c = parser_priv(state);
			require(c);
			eq(c.max_attacks, 5);
			eq(c.min_weight, 35);
			eq(c.att_multiply, 4);
			ok;*/
		}

		public static void test_m0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "M:90:1:3:400");
			struct player_class *c;

			eq(r, PARSE_ERROR_NONE);
			c = parser_priv(state);
			require(c);
			eq(c.spell_book, 90);
			eq(c.spell_stat, 1);
			eq(c.spell_first, 3);
			eq(c.spell_weight, 400);
			ok;*/
		}

		public static void test_b0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "B:8:23:25:90:3");
			struct player_class *c;

			eq(r, PARSE_ERROR_NONE);
			c = parser_priv(state);
			require(c);
			eq(c.spells.info[8].slevel, 23);
			eq(c.spells.info[8].smana, 25);
			eq(c.spells.info[8].sfail, 90);
			eq(c.spells.info[8].sexp, 3);
			ok;*/
		}

		public static void test_t0() {
			Fail("Write Later");
			/*enum parser_error r0 = parser_parse(state, "T:Runner");
			enum parser_error r1 = parser_parse(state, "T:Strider");
			struct player_class *c;

			eq(r0, PARSE_ERROR_NONE);
			eq(r1, PARSE_ERROR_NONE);
			c = parser_priv(state);
			require(c);
			require(streq(c.title[0], "Runner"));
			require(streq(c.title[1], "Strider"));
			ok;*/
		}

		/* Causes segfault: lookup_sval() requires z_info/k_info */
		public static void test_e0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "E:magic book:2:2:5");
			struct player_class *c;

			eq(r, PARSE_ERROR_NONE);
			c = parser_priv(state);
			require(c);
			ptreq(c.start_items[0].kind, objkind_get(TV_MAGIC_BOOK, 2));
			eq(c.start_items[0].min, 2);
			eq(c.start_items[0].max, 5);
			ok;*/
		}

		public static void test_f0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "F:CUMBER_GLOVE | CHOOSE_SPELLS");
			struct player_class *c;

			eq(r, PARSE_ERROR_NONE);
			c = parser_priv(state);
			require(c);
			require(c.pflags);
			ok;*/
		}
	}
}
