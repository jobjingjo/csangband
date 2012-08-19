using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class ParseStore_Test : UnitTest {
		static Parser p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "parse/store";

			suite.SetSetup(delegate() {
				p = Store.store_parser_new();
			});
			suite.SetTeardown(delegate() {
				p.Destroy();
			});


			suite.AddTest("s0", test_s0);
			suite.AddTest("i0", test_i0);

			UnitTest_Main.AddSuite(suite);
		}

		
		public static void test_s0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "S:2:33");
			struct store *s;

			eq(r, PARSE_ERROR_NONE);
			s = parser_priv(state);
			require(s);
			eq(s.sidx, 1);
			eq(s.table_size, 33);
			ok;*/
		}

		/* Causes segfault: lookup_name() requires z_info/k_info */
		public static void test_i0() {
			Fail("This test aparently always causes segfaults?");
			/*
			enum parser_error r = parser_parse(state, "I:2:3:5");
			struct store *s;

			eq(r, PARSE_ERROR_NONE);
			s = parser_priv(state);
			require(s);
			require(s.table[0]);
			require(s.table[1]);
			ok;*/
		}
	}
}
