using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Player_Test : UnitTest {
		static CSAngband.Player.Player p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "player";

			suite.SetSetup(delegate() {
				p = new CSAngband.Player.Player();
			});

			suite.NoTeardown();


			suite.AddTest("Increment Stat", test_stat_inc);
			suite.AddTest("Decrement Stat", test_stat_dec);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_stat_inc() {
			bool v;

			p.stat_cur[(int)Stat.Str] = 18 + 101;
			v = p.stat_inc(Stat.Str);
			if(v) {
				Fail("Strength was incorrectly incremented");
			}

			p.stat_cur[(int)Stat.Str] = 15;
			p.stat_inc(Stat.Str);
			if(p.stat_cur[(int)Stat.Str] != 16) {
				Fail("Strength was not incremented");
			}

			p.stat_inc(Stat.Str);
			if(p.stat_cur[(int)Stat.Str] != 17) {
				Fail("Strength was not incremented");
			}

			p.stat_inc(Stat.Str);
			if(p.stat_cur[(int)Stat.Str] != 18) {
				Fail("Strength was not incremented");
			}


			p.stat_inc(Stat.Str);
			if(p.stat_cur[(int)Stat.Str] <= 18) {
				Fail("Strength was not incremented");
			}
			
			Ok();
		}

		public static void test_stat_dec() {
			bool v;

			p.stat_cur[(int)Stat.Str] = 3;
			p.stat_max[(int)Stat.Str] = 3;
			v = p.stat_dec(Stat.Str, true);
			if(v == true) {
				Fail("Strength was incorrectly decremented");
			}

			p.stat_cur[(int)Stat.Str] = 15;
			p.stat_max[(int)Stat.Str] = 15;
			p.stat_dec((int)Stat.Str, false);
			if(p.stat_cur[(int)Stat.Str] != 14) {
				Fail("Strength was not decremented");
			}
			if(p.stat_max[(int)Stat.Str] != 15) {
				Fail("Strength was incorrectly decremented");
			}

			p.stat_dec((int)Stat.Str, true);
			if(p.stat_cur[(int)Stat.Str] != 13) {
				Fail("Strength was not decremented");
			}
			if(p.stat_max[(int)Stat.Str] != 14) {
				Fail("Strength was incorrectly decremented");
			}

			p.stat_cur[(int)Stat.Str] = 18+13;
			p.stat_max[(int)Stat.Str] = 18+13;
			p.stat_dec(Stat.Str, false);
			if(p.stat_cur[(int)Stat.Str] != 18 + 03) {
				Fail("Strength was not decremented");
			}
			if(p.stat_max[(int)Stat.Str] != 18 + 13) {
				Fail("Strength was incorrectly decremented");
			}

			p.stat_max[(int)Stat.Str] = 18 + 03;
			p.stat_dec(Stat.Str, true);
			if(p.stat_cur[(int)Stat.Str] != 18) {
				Fail("Strength was not decremented");
			}
			if(p.stat_max[(int)Stat.Str] != 18) {
				Fail("Strength was incorrectly decremented");
			}
			
			Ok();
		}
	}
}
