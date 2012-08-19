using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class S_Info_Test : UnitTest {
		static Parser p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "parse/s-info";

			suite.SetSetup(delegate() {
				p = Init.init_parse_s();
			});
			suite.SetTeardown(delegate() {
				p.Destroy();
			});

			suite.AddTest("n0", test_n0 );
			suite.AddTest( "i0", test_i0);
			suite.AddTest("d0", test_d0);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_n0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "N:1:Detect Monsters");
			struct spell *s;

			eq(r, PARSE_ERROR_NONE);
			s = parser_priv(state);
			require(s);
			eq(s.sidx, 1);
			require(streq(s.name, "Detect Monsters"));
			ok;*/
		}

		public static void test_i0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "I:90:0:1");
			struct spell *s;

			eq(r, PARSE_ERROR_NONE);
			s = parser_priv(state);
			require(s);
			eq(s.tval, 90);
			eq(s.sval, 0);
			eq(s.snum, 1);
			ok;*/
		}

		public static void test_d0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "D:Teleports you randomly.");
			struct spell *s;

			eq(r, PARSE_ERROR_NONE);
			s = parser_priv(state);
			require(s);
			require(streq(s.text, "Teleports you randomly."));
			ok;*/
		}
	}
}
