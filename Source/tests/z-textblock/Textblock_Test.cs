using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Textblock_Test : UnitTest {
		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "z-textblock/textblock";

			suite.NoSetup();
			suite.NoTeardown();


			suite.AddTest("Constructor", test_alloc);
			suite.AddTest("Append", test_append);
			suite.AddTest("Colour", test_colour);
			suite.AddTest("Length/Content", test_length);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_alloc() {
			Textblock tb = new Textblock();
			Require(tb != null);
			Ok();
		}

		public static void test_append() {
			Textblock tb = new Textblock();

			Require(tb.Text == "");

			tb.append("Hello");
			Require(tb.Text == "Hello");

			tb.append("{0}", 20);
			Require(tb.Text == "Hello20");

			Ok();
		}

		public static void test_colour() {
			Textblock tb = new Textblock();

			string text = "two";

			//These were all TERM_L_GREEN
			ConsoleColor[] attrs = new ConsoleColor[] { ConsoleColor.Green, ConsoleColor.Green, ConsoleColor.Green };	

			tb.append_c(ConsoleColor.Green, text);

			for(int i = 0; i < attrs.Length; i++) {
				Require(tb.Attributes[i] == attrs[i]);
			}

			Ok();
		}

		public static void test_length() {
			Textblock tb = new Textblock();

			string text = "1234567";

			/* Add it 32 times to make sure that appending definitely works */
			for (int i = 0; i < 32; i++) {
				tb.append(text);
			}

			/* Now make sure it's all right */
			for (int i = 0; i < 32; i++) {
				int n = text.Length;
				int offset = i * n;

				//Original
	 			//Require(!memcmp(tb_text + offset, text, n));
				Require(tb.Text.Substring(offset, n) == text);
			}

			Ok();
		}
	}
}
