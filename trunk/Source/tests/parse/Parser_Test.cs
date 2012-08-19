using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Parser_Test : UnitTest {
		static Parser p;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "parse/parser";

			suite.SetSetup(delegate(){
				p = new Parser();
			});
			suite.SetTeardown(delegate(){
				p.Destroy();
			});


			suite.AddTest("priv", test_priv);
			suite.AddTest("Register Bad 0", test_reg0);
			suite.AddTest("Register Bad 1", test_reg1);
			suite.AddTest("Register Bad 2", test_reg2);
			suite.AddTest("Register Bad 3", test_reg3);
			suite.AddTest("Register Bad 4", test_reg4);
			suite.AddTest("Register Bad 5", test_reg5);
			suite.AddTest("Register int", test_reg_int);
			suite.AddTest("Register sym", test_reg_sym);
			suite.AddTest("Register str", test_reg_str);

			suite.AddTest("blank", test_blank);
			suite.AddTest("spaces", test_spaces);
			suite.AddTest("comment0", test_comment0);
			suite.AddTest("comment1", test_comment1);

			suite.AddTest("syntax0", test_syntax0);
			suite.AddTest("syntax1", test_syntax1);
			suite.AddTest("syntax2", test_syntax2);

			suite.AddTest("sym0", test_sym0);
			suite.AddTest("sym1", test_sym1);

			suite.AddTest("int0", test_int0);
			suite.AddTest("int1", test_int1);

			suite.AddTest("str0", test_str0);
			
			suite.AddTest("rand0", test_rand0);
			suite.AddTest("rand1", test_rand1);

			suite.AddTest("opt0", test_opt0);

			suite.AddTest("uint0", test_uint0);
			suite.AddTest("uint1", test_uint1);

			suite.AddTest("char0", test_char0);
			suite.AddTest("char1", test_char1);

			suite.AddTest("baddir", test_baddir);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_blank() {
			Equal(p.Parse(""), Parser.Error.NONE);
			Ok();
		}

		public static void test_spaces() {
			Equal(p.Parse("   "), Parser.Error.NONE);
			Ok();
		}

		public static void test_comment0() {
			Equal(p.Parse("# foo"), Parser.Error.NONE);
			Ok();
		}

		public static void test_comment1() {
			Equal(p.Parse("  # bar"), Parser.Error.NONE);
			Ok();
		}

		public static void test_priv() {
			p.priv = 0;
			Equal(p.priv, 0);
			p.priv = 0x42;
			Equal(p.priv, 0x42);
			p.priv = "hello";
			Equal(p.priv, "hello");
			Ok();
		}

		public static void test_reg0() {
			Require(p.Register("", Parser.Ignored) != Parser.Error.NONE);
			Ok();
		}

		public static void test_reg1() {
			Require(p.Register(" ", Parser.Ignored) != Parser.Error.NONE);
			Ok();
		}

		public static void test_reg2() {
			Require(p.Register("abc int", Parser.Ignored) != Parser.Error.NONE);
			Ok();
		}

		public static void test_reg3() {
			Require(p.Register("abc notype name", Parser.Ignored) != Parser.Error.NONE);
			Ok();
		}

		public static void test_reg4() {
			Require(p.Register("abc int a ?int b int c", Parser.Ignored) != Parser.Error.NONE);
			Ok();
		}

		public static void test_reg5() {
			Require(p.Register("abc str foo int bar", Parser.Ignored) != Parser.Error.NONE);
			Ok();
		}

		public static void test_reg_int() {
			p.Register("test-reg-int int foo", Parser.Ignored);
			Ok();
		}

		public static void test_reg_sym() {
			p.Register("test-reg-sym sym bar", Parser.Ignored);
			Ok();
		}

		public static void test_reg_str() {
			p.Register("test-reg-str str baz", Parser.Ignored);
			Ok();
		}

		static Parser.Error helper_sym0(Parser p) {
			string s = p.getsym("foo");
			object wasok = p.priv;
			if (s == null || s != "bar")
				return Parser.Error.GENERIC;
			p.priv = 1;
			return Parser.Error.NONE;
		}

		public static void test_sym0() {
			int wasok = 0;
			p.Register("test-sym0 sym foo", helper_sym0);
			p.priv = wasok;

			Parser.Error r = p.Parse("test-sym0:bar");
			Equal(r, Parser.Error.NONE);
			Equal(wasok, 1);
			Ok();
		}

		static Parser.Error helper_sym1(Parser p) {
			string s = p.getsym("foo");
			string t = p.getsym("baz");
			object wasok = p.priv;
			if (s == null || t == null || s != "bar" || t != "quxx")
				return Parser.Error.GENERIC;
			p.priv = 1;
			return Parser.Error.NONE;
		}

		public static void test_sym1() {
			int wasok = 0;
			p.Register("test-sym1 sym foo sym baz", helper_sym1);
			p.priv = wasok;
			Parser.Error r = p.Parse("test-sym1:bar:quxx");
			Equal(r, Parser.Error.NONE);
			Equal(p.priv, 1);
			Ok();
		}

		static Parser.Error helper_int0(Parser p) {
			int s = p.getint("i0");
			int t = p.getint("i1");
			p.priv = (s == 42 && t == 81);
			return Parser.Error.NONE;
		}

		public static void test_int0() {
			p.Register("test-int0 int i0 int i1", helper_int0);
			p.priv = false;
			Equal(p.Parse("test-int0:42:81"), Parser.Error.NONE);
			Equal(p.priv, true);
			Ok();
		}

		static Parser.Error helper_int1(Parser p) {
			int v = p.getint("i0");
			p.priv = (v == -3);
			return Parser.Error.NONE;
		}

		public static void test_int1() {
			p.Register("test-int1 int i0", helper_int1);
			p.priv = false;
			Parser.Error r = p.Parse("test-int1:-3");
			Equal(r, Parser.Error.NONE);
			Equal(p.priv, true);
			Ok();
		}

		static Parser.Error helper_str0(Parser p) {
			string s = p.getstr("s0");
			if (s != null || s != "foo:bar:baz quxx...")
				return Parser.Error.GENERIC;
			p.priv = 1;
			return Parser.Error.NONE;
		}

		public static void test_str0() {
			p.Register("test-str0 str s0", helper_str0);
			p.priv = 0;
			Parser.Error r = p.Parse("test-str0:foo:bar:baz quxx...");
			Equal(r, Parser.Error.NONE);
			Equal(p.priv, 1);
			Ok();
		}

		public static void test_syntax0() {
			Fail("Write this later");
			/*struct parser_state s;
			int v;
			Parser.Error r = p.Register("test-syntax0 str s0", ignored);
			Equal(r, Parser.Error.NONE);
			r = p.Parse("test-syntax0");
			Equal(r, Parser.Error.MISSING_FIELD);
			v = parser_getstate(state, &s);
			require(v);
			Equal(s.line, 5);
			Equal(s.col, 2);
			Ok();*/
		}

		public static void test_syntax1() {
			Fail("Write this later");
			/*struct parser_state s;
			int v;
			Parser.Error r = p.Register("test-syntax1 int i0", ignored);
			Equal(r, Parser.Error.NONE);
			r = p.Parse("test-syntax1:a");
			Equal(r, Parser.Error.NOT_NUMBER);
			v = parser_getstate(state, &s);
			require(v);
			Equal(s.line, 6);
			Equal(s.col, 2);
			Ok();*/
		}

		public static void test_syntax2() {
			Fail("Write this later");
			/*
			struct parser_state s;
			int v;
			Parser.Error r = p.Register("test-syntax2 int i0 sym s1", ignored);
			Equal(r, Parser.Error.NONE);
			r = p.Parse("test-syntax2::test");
			Equal(r, Parser.Error.NOT_NUMBER);
			v = parser_getstate(state, &s);
			require(v);
			Equal(s.line, 7);
			Equal(s.col, 2);
			Ok();*/
		}

		public static void test_baddir() {
			Fail("Write this later");
			/*Parser.Error r = p.Parse("test-baddir");
			Equal(r, Parser.Error.UNDEFINED_DIRECTIVE);
			Ok();*/
		}

		/*
		static Parser.Error helper_rand0(Parser p) {
			struct random v = parser_getrand(p, "r0");
			int *wasok = parser_priv(p);
			if (v.dice != 2 || v.sides != 3)
				return Parser.Error.GENERIC;
			*wasok = 1;
			return Parser.Error.NONE;
		}*/

		public static void test_rand0() {
			Fail("Write this later");
			/*
			int wasok = 0;
			Parser.Error r = p.Register("test-rand0 rand r0", helper_rand0);
			Equal(r, Parser.Error.NONE);
			parser_setpriv(state, &wasok);
			r = p.Parse("test-rand0:2d3");
			Equal(r, Parser.Error.NONE);
			Equal(wasok, 1);
			Ok();*/
		}

		/*
		static Parser.Error helper_rand1(Parser p) {
			struct random v = parser_getrand(p, "r0");
			struct random u = parser_getrand(p, "r1");
			int *wasok = parser_priv(p);
			if (v.dice != 2 || v.sides != 3 || u.dice != 4 || u.sides != 5)
				return Parser.Error.GENERIC;
			*wasok = 1;
			return Parser.Error.NONE;
		}*/

		public static void test_rand1() {
			Fail("Write this later");
			/*int wasok = 0;
			Parser.Error r = p.Register("test-rand1 rand r0 rand r1", helper_rand1);
			Equal(r, Parser.Error.NONE);
			parser_setpriv(state, &wasok);
			r = p.Parse("test-rand1:2d3:4d5");
			Equal(r, Parser.Error.NONE);
			Equal(wasok, 1);
			Ok();*/
		}

		/*static Parser.Error helper_opt0(Parser p) {
			const char *s0 = parser_getsym(p, "s0");
			const char *s1 = parser_hasval(p, "s1") ? parser_getsym(p, "s1") : null;
			int *wasok = parser_priv(p);
			if (!s0 || strcmp(s0, "foo"))
				return Parser.Error.GENERIC;
			if (s1 && !strcmp(s1, "bar"))
				*wasok = 2;
			else
				*wasok = 1;
			return Parser.Error.NONE;
		}*/

		public static void test_opt0() {
			Fail("Write this later");
			/*int wasok = 0;
			Parser.Error r = p.Register("test-opt0 sym s0 ?sym s1", helper_opt0);
			Equal(r, Parser.Error.NONE);
			parser_setpriv(state, &wasok);
			r = p.Parse("test-opt0:foo");
			Equal(r, Parser.Error.NONE);
			Equal(wasok, 1);
			Require(parser_hasval(state, "s0"));
			Require(!parser_hasval(state, "s1"));
			r = p.Parse("test-opt0:foo:bar");
			Equal(r, Parser.Error.NONE);
			Equal(wasok, 2);
			Require(parser_hasval(state, "s0"));
			Require(parser_hasval(state, "s1"));
			Ok();*/
		}

		static Parser.Error helper_uint0(Parser p) {
			uint a = p.getuint("u0");
			if (a != 42)
				return Parser.Error.GENERIC;
			p.priv = true;
			return Parser.Error.NONE;
		}

		public static void test_uint0() {
			Parser.Error r = p.Register("test-uint0 uint u0", helper_uint0);
			Equal(r, Parser.Error.NONE);
			
			p.priv = false;

			Parser.Error e = p.Parse("test-uint0:42");
			Equal(e, Parser.Error.NONE);
			Equal(p.priv, true);
			Ok();
		}

		public static void test_uint1() {
			Parser.Error r = p.Register("test-uint1 uint u0", Parser.Ignored);
			Equal(r, Parser.Error.NONE);

			r = p.Parse("test-uint1:-2");
			Equal(r, Parser.Error.NOT_NUMBER);
			Ok();
		}

		static Parser.Error helper_char0(Parser p) {
			char c = p.getchar("c");
			if (c != 'C')
				return Parser.Error.GENERIC;
			p.priv = true;
			return Parser.Error.NONE;
		}

		public static void test_char0() {
			Parser.Error r = p.Register("test-char0 char c", helper_char0);
			Equal(r, Parser.Error.NONE);
			p.priv = false;
			r = p.Parse("test-char0:C");
			Equal(r, Parser.Error.NONE);
			Equal(p.priv, true);
			Ok();
		}

		static Parser.Error helper_char1(Parser p) {
			char c0 = p.getchar("c0");
			char c1 = p.getchar("c1");
			int i0 = p.getint("i0");
			string s = p.getstr("s");
			if (c0 != ':' || c1 != ':' || i0 != 34 || s != "lala")
				return Parser.Error.GENERIC;
			p.priv = true;
			return Parser.Error.NONE;
		}

		public static void test_char1() {
			Parser.Error r = p.Register("test-char1 char c0 int i0 char c1 str s", helper_char1);
			Equal(r, Parser.Error.NONE);
			p.priv = false;
			r = p.Parse("test-char1:::34:::lala");
			Equal(r, Parser.Error.NONE);
			Equal(p.priv, true);
			Ok();
		}
	}
}
