using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Z_Info_Test : UnitTest {
		static Parser p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "parse/z-info";

			suite.SetSetup(delegate() {
				p = Init.init_parse_z();
			});
			suite.SetTeardown(delegate() {
				p.Destroy();
			});


			suite.AddTest("negative", test_negative);
			suite.AddTest("badmax", test_badmax);
			suite.AddTest("fmax", test_f_max);
			suite.AddTest("kmax", test_k_max);
			suite.AddTest("amax", test_a_max);
			suite.AddTest("emax", test_e_max);
			suite.AddTest("rmax", test_r_max);
			suite.AddTest("smax", test_s_max);
			suite.AddTest("omax", test_o_max);
			suite.AddTest("mmax", test_m_max);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_negative() {
			Equal(p.Parse("M:F:-1") , Parser.Error.INVALID_VALUE);
			Ok();
		}

		public static void test_badmax() {
			Equal(p.Parse("M:D:1"), Parser.Error.UNDEFINED_DIRECTIVE);
			Ok();
		}

		
		public static void test_f_max() {
			Fail("Write Later");
			/*maxima *m = parser_priv(s);
			char buf[64];
			errr r;
			snprintf(buf, sizeof(buf), "M:%c:%d", 'F', __LINE__);
			r = parser_parse(s, buf);
			eq(m.f_max, __LINE__);
			eq(r, 0);
			ok;*/
		}

		public static void test_k_max() {
			Fail("Write Later");
			/*maxima *m = parser_priv(s);
			char buf[64];
			errr r;
			snprintf(buf, sizeof(buf), "M:%c:%d", 'K', __LINE__);
			r = parser_parse(s, buf);
			eq(m.k_max, __LINE__);
			eq(r, 0);
			ok;*/
		}

		public static void test_a_max() {
			Fail("Write Later");
			/*maxima *m = parser_priv(s);
			char buf[64];
			errr r;
			snprintf(buf, sizeof(buf), "M:%c:%d", 'A', __LINE__);
			r = parser_parse(s, buf);
			eq(m.a_max, __LINE__);
			eq(r, 0);
			ok;*/
		}

		public static void test_e_max() {
			Fail("Write Later");
			/*maxima *m = parser_priv(s);
			char buf[64];
			errr r;
			snprintf(buf, sizeof(buf), "M:%c:%d", 'E', __LINE__);
			r = parser_parse(s, buf);
			eq(m.e_max, __LINE__);
			eq(r, 0);
			ok;*/
		}

		public static void test_r_max() {
			Fail("Write Later");
			/*maxima *m = parser_priv(s);
			char buf[64];
			errr r;
			snprintf(buf, sizeof(buf), "M:%c:%d", 'R', __LINE__);
			r = parser_parse(s, buf);
			eq(m.r_max, __LINE__);
			eq(r, 0);
			ok;*/
		}

		public static void test_s_max() {
			Fail("Write Later");
			/*maxima *m = parser_priv(s);
			char buf[64];
			errr r;
			snprintf(buf, sizeof(buf), "M:%c:%d", 'S', __LINE__);
			r = parser_parse(s, buf);
			eq(m.s_max, __LINE__);
			eq(r, 0);
			ok;*/
		}

		public static void test_o_max() {
			Fail("Write Later");
			/*maxima *m = parser_priv(s);
			char buf[64];
			errr r;
			snprintf(buf, sizeof(buf), "M:%c:%d", 'O', __LINE__);
			r = parser_parse(s, buf);
			eq(m.o_max, __LINE__);
			eq(r, 0);
			ok;*/
		}

		public static void test_m_max() {
			Fail("Write Later");
			/*maxima *m = parser_priv(s);
			char buf[64];
			errr r;
			snprintf(buf, sizeof(buf), "M:%c:%d", 'M', __LINE__);
			r = parser_parse(s, buf);
			eq(m.REPLACE_ME, __LINE__);
			eq(r, 0);
			ok;*/
		}
	}
}
