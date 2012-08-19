using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Quark_Test : UnitTest {
		static CSAngband.Player.Player p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "quark";

			suite.SetSetup(delegate() {
				Quark.Init();
			});

			suite.SetTeardown(delegate(){
				Quark.Free();
			});


			suite.AddTest("Allocate", test_alloc);
			suite.AddTest("No-Duplicates", test_dedup);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_alloc() {
			Quark q1 = Quark.Add("0-foo");
			Quark q2 = Quark.Add("0-bar");
			Quark q3 = Quark.Add("0-baz");

			if(q1 == null){
				Fail("Failed to allocate Quark");	
			}

			if(q2 == null) {
				Fail("Failed to allocate Quark");
			}

			if(q3 == null) {
				Fail("Failed to allocate Quark");
			}


			if(q1.ToString() != "0-foo") {
				Fail("Failed to allocate Quark");
			}

			if(q2.ToString() != "0-bar") {
				Fail("Failed to allocate Quark");
			}

			if(q3.ToString() != "0-baz") {
				Fail("Failed to allocate Quark");
			}

			Ok();
		}

		public static void test_dedup() {
			Quark q1 = Quark.Add("1-foo");
			Quark q2 = Quark.Add("1-foo");
			Quark q3 = Quark.Add("1-bar");

			if(q1 == null) {
				Fail("Failed to initialize quark");
			}
			if(q2 == null) {
				Fail("Failed to initialize quark");
			}
			if(q3 == null) {
				Fail("Failed to initialize quark");
			}

			if(q1 != q2) {
				Fail("Allocated an extra quark");
			}

			if(q1.ToString() != q2.ToString()) {
				Fail("Quark strings do not match");
			}

			if(q1 == q3) {
				Fail("Incorrectly matched Quarks");
			}
			if(q1.ToString() == q3.ToString()) {
				Fail("Quarks strings match when they shouldn't");
			}

			Ok();
		}
	}
}
