using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Lookup_Test : UnitTest {
		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "command/lookup";

			suite.SetSetup(delegate() {
				Command.Init();
			});
			suite.NoTeardown();


			suite.AddTest("cmd_lookup", test_cmd_lookup);

			UnitTest_Main.AddSuite(suite);
		}

		/* Regression test for #1330 */
		public static void test_cmd_lookup() {
			Require(Command.lookup('Z') == Command_Code.NULL);
			Require(Command.lookup('{') == Command_Code.INSCRIBE);
			Require(Command.lookup('u') == Command_Code.USE_STAFF);
			Require(Command.lookup('T') == Command_Code.TUNNEL);
			Require(Command.lookup('g') == Command_Code.PICKUP);
			Require(Command.lookup('G') == Command_Code.STUDY_BOOK);
			Require(Command.lookup(UIEvent.KTRL('S')) == Command_Code.SAVE);
			Require(Command.lookup('+') == Command_Code.ALTER);
	
			Ok();
		}
	}
}
