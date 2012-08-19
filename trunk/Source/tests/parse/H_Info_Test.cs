using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class H_Info_Test : UnitTest {
		static Parser p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "parse/h-info";

			suite.SetSetup(delegate() {
				p = Init.init_parse_h();
			});
			suite.SetTeardown(delegate() {
				p.Destroy();
			});


			suite.AddTest("n0", test_n0);
			suite.AddTest("d0", test_d0);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_n0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "N:1:3:5:2");
			struct history_chart *c;
			struct history_entry *e;

			eq(r, PARSE_ERROR_NONE);
			c = parser_priv(state);
			require(c);
			e = c.entries;
			require(e);
			eq(c.idx, 1);
			ptreq(e.next, null);
			ptreq(e.roll, 5);
			ptreq(e.bonus, 2);
			eq(e.isucc, 3);
			ok;*/
		}

		public static void test_d0() {
			Fail("Write Later");
			/*enum parser_error r = parser_parse(state, "D:hello there");
			struct history_chart *h;

			eq(r, PARSE_ERROR_NONE);
			h = parser_priv(state);
			require(h);
			require(streq(h.entries.text, "hello there"));
			ok;*/
		}
	}
}
