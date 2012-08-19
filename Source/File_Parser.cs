using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class File_Parser {
		public File_Parser(string name, init_func init, run_func run, run_func finish, cleanup_func cleanup) {
			this.name = name;
			this.init = init;
			this.run = run;
			this.finish = finish;
			this.cleanup = cleanup;
		}
		public string name;
		
		//Delegates
		public delegate Parser init_func();
		public init_func init;

		public delegate Parser.Error run_func(Parser p);
		public run_func run;
		public run_func finish;

		public delegate void cleanup_func();
		public cleanup_func cleanup;

		public Parser.Error run_parser() {
			Parser p = init();
			Parser.Error r;
			if (p == null) {
				return Parser.Error.GENERIC;
			}
			r = run(p);
			if (r != Parser.Error.NONE) {
				print_error(p);
				return r;
			}
			r = finish(p);
			if (r != Parser.Error.NONE)
				print_error(p);
			return r;
		}

		/* More angband-specific bits of the parser
		 * These require hooks into other parts of the code, and are a candidate for
		 * moving elsewhere.
		 */
		static void print_error(Parser p) {
			throw new NotImplementedException();
			//struct parser_state s;
			//parser_getstate(p, &s);
			//msg("Parse error in %s line %d column %d: %s: %s", fp.name,
			//           s.line, s.col, s.msg, parser_error_str[s.error]);
			//message_flush();
			//quit_fmt("Parse error in %s line %d column %d.", fp.name, s.line, s.col);
		}
	}
}
