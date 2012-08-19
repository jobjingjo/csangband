using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class History_Test : UnitTest {
		static Player.History_Chart ca;
		static Player.History_Chart cb;
		static Player.History_Chart cc;

		static Player.History_Entry ea0;
		static Player.History_Entry ea1;
		static Player.History_Entry eb0;
		static Player.History_Entry eb1;
		static Player.History_Entry ec0;
		static Player.History_Entry ec1;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "player/history";

			suite.SetSetup(delegate() {
				//INIT
				ca = new Player.History_Chart();
				cb = new Player.History_Chart();
				cc = new Player.History_Chart();

				ea0 = new Player.History_Entry();
				ea1 = new Player.History_Entry();
				eb0 = new Player.History_Entry();
				eb1 = new Player.History_Entry();
				ec0 = new Player.History_Entry();
				ec1 = new Player.History_Entry();

				//HOOK & SET
				ca.entries = ea0;
				cb.entries = eb0;
				cc.entries = ec0;

				ea0.next = ea1;
				ea0.succ = cb;
				ea0.roll = 50;
				ea0.bonus = 0;
				ea0.text = "A0";

				ea1.next = null;
				ea1.succ = cc;
				ea1.roll = 100;
				ea1.bonus = 0;
				ea1.text = "A1";

				eb0.next = eb1;
				eb0.succ = cc;
				eb0.roll = 50;
				eb0.bonus = 0;
				eb0.text = "B0";

				eb1.next = null;
				eb1.succ = null;
				eb1.roll = 100;
				eb1.bonus = 0;
				eb1.text = "B1";

				ec0.next = ec1;
				ec0.succ = null;
				ec0.roll = 50;
				ec0.bonus = 0;
				ec0.text = "C0";

				ec1.next = null;
				ec1.succ = null;
				ec1.roll = 100;
				ec1.bonus = 0;
				ec1.text = "C1";
			});
			suite.NoTeardown();


			suite.AddTest("0", test_0);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_0() {
			for(int i = 0; i < 100; i++) {
				short s;
				string h = ca.get_history(out s);
				Require(h != null);
				Equal(h[0], 'A');
				Require(Char.IsDigit(h[1]));
				Require(h[2] == 'B' || h[2] == 'C');
				Require(Char.IsDigit(h[3]));
				if(h[2] == 'B' && h.Length >= 5) {
					Require(h[4] == 'C');
					Require(Char.IsDigit(h[5]));
				}
			}

			Ok();
		}
	}
}
