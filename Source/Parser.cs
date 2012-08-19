using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace CSAngband {
	class Parser {
		Error error;
		UInt32 lineno;
		UInt32 colno;
		string errmsg; //Max = 1024

		//This might replace parser_specin parser_hook
		public class Hook_Spec{
			public string name;
			public PARSE_T type;
		}

		public delegate Error ParseFunc(Parser p);
		public class Parser_Hook {
			public ParseFunc func; //This might replace the parser_hook below
			public string dir; //No idea what "dir" stands for.
			public List<Hook_Spec> specs = new List<Hook_Spec>();
		}

		private List<Parser_Hook> hooks = new List<Parser_Hook>();

		//TODO: Enable these
		/*struct parser_hook *hooks;
		struct parser_value *fhead;
		struct parser_value *ftail;*/

		public class Parser_Value {
			public Hook_Spec spec;
			public object value;
		}
		private List<Parser_Value> values = new List<Parser_Value>();
		
		public object priv;

		public void Destroy(){
			//NOTHING TO SEE HERE
			//ARTIFACTS OF C's LACK OF GARBAGE COLLECTION

			//ONE DAY, SOMEONE SHOULD destroy THIS FUNCTION

			/*struct parser_hook *h;
			parser_freeold(p);
			while (p.hooks)
			{
				h = p.hooks.next;
				clean_specs(p.hooks);
				mem_free(p.hooks);
				p.hooks = h;
			}
			mem_free(p);*/
		}

		/* This is a bit long and should probably be refactored a bit. */
		public Error Parse(string line) {
			/*struct parser_hook *h;
			struct parser_spec *s;
			struct parser_value *v;
			char *sp = null;*/

			//assert(p); //lol, C is so silly
			Misc.assert(line != null);

			freeold();

			lineno++;
			colno = 1;

			values.Clear();

			// Ignore empty lines and comments.
			line = line.Trim();
			if (line.Length == 0 || line[0] == '#'){
				return Error.NONE;
			}

			string[] tok = line.Split(new char[]{':'});
			int tat = 0;

			if (tok.Length == 0) {
				error = Error.MISSING_FIELD;
				return error;
			}

			Parser_Hook h = findhook(tok[tat++]);
			if (h == null) {
				error = Error.UNDEFINED_DIRECTIVE;
				return error;
			}

			//Imma just try this from scratch
			//We wish to match each token in order
			foreach(Hook_Spec s in h.specs) {
				if(tat >= tok.Length) {
					if((s.type & PARSE_T.OPT) != 0) {
						break; //We're cool, it was optional anyways
					} else {
						return Error.GENERIC;
					}
				}
				string val = tok[tat++];
				Parser_Value v = new Parser_Value();

				PARSE_T type = s.type & ~PARSE_T.OPT;

				if(type == PARSE_T.INT) {
					int t;
					if(!Int32.TryParse(val, out t)) {
						return Error.INVALID_VALUE;
					}
					v.value = t;
				} else if(type == PARSE_T.UINT) {
					uint t;
					if(!UInt32.TryParse(val, out t)) {
						return Error.INVALID_VALUE;
					}
					v.value = t;
				} else if(type == PARSE_T.CHAR) {
					char t;
					if(!Char.TryParse(val, out t)) {
						return Error.INVALID_VALUE;
					}
					v.value = t;
				} else if(type == PARSE_T.RAND) {
					random_value t = new random_value();

					Regex r = new Regex("[0-9]+");
					MatchCollection matches = r.Matches(val, 0);

					//This first boolean quickly became hacky. Todo: build better regex!
					bool has_dice_number = val.Contains("d") && val[0] != 'd' && val[1] != 'd'; //begining with a d is cheating
					bool has_Base_modifies = val.Contains("M");

					int mat = 0;

					t.dice = 1;
					if(has_dice_number) {
						t.dice = int.Parse(matches[mat++].Value);
					}

					t.sides = int.Parse(matches[mat++].Value);

					t.Base = 0;
					if(has_Base_modifies) {
						t.Base = int.Parse(matches[mat++].Value);
					}

					v.value = t;
				} else if(type == PARSE_T.SYM) {
					v.value = val; //Straight up
				} else if (type == PARSE_T.STR){
					string temp = val;
					while(tat < tok.Length) {
						temp += tok[tat++];
						if(tat < tok.Length) {
							temp += ":";
						}
					}
					v.value = temp;
				} else {
					throw new NotImplementedException();
					//We probably got a type I have not written yet
				}

				v.spec = s;
				values.Add(v);
			}

			return h.func(this);
		}

		Parser_Hook findhook(string dir) {
			foreach(Parser_Hook h in hooks) {
				if(h.dir == dir) {
					return h;
				}
			}
			return null;
		}

		void freeold() {
			//this is pretty much the same thing
			values = new List<Parser_Value>();
			/*
			struct parser_value *v;
			while (p.fhead)
			{
				int t = p.fhead.spec.type & ~PARSE_T_OPT;
				v = (struct parser_value *)p.fhead.spec.next;
				if (t == PARSE_T_SYM || t == PARSE_T_STR)
					mem_free(p.fhead.u.sval);
				mem_free(p.fhead);
				p.fhead = v;
			}*/
		}

		/** Registers a parser hook.
		 *
		 * Hooks have the following format:
		 *   <fmt>  ::= <name> [<type> <name>]* [?<type> <name>]*
		 *   <type> ::= int | str | sym | rand | char
		 * The first <name> is called the directive for this hook. Any other hooks with
		 * the same directive are superseded by this hook. It is an error for a
		 * mandatory field to follow an optional field. It is an error for any field to
		 * follow a field of type `str`, since `str` fields are not delimited and will
		 * consume the entire rest of the line.
		 */
		public Error Register(string fmt, ParseFunc pf) {
			//Misc.assert(p); //if p (self) was null, we wouldn't be here
			Misc.assert(fmt != null);
			Misc.assert(pf != null);

			Parser_Hook h = new Parser_Hook();
			h.func = pf;

			/*h.next = p.hooks;
			h.func = func;*/
			Error r = parse_specs(h, fmt);
			if (r != Error.NONE)
			{
				return r;
			}

			hooks.Add(h);
			//p.hooks = h;
			//mem_free(cfmt);
			return Error.NONE;
		}

		static Error parse_specs(Parser_Hook h, string fmt) {
			Misc.assert(h != null);
			Misc.assert(fmt != null);

			string[] split = fmt.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
			if (split.Length == 0)
				return Error.GENERIC;

			h.dir = split[0];

			int i = 1;
			while(i < split.Length)
			{
				/* Lack of a type is legal; that means we're at the end of the
				 * line. */
				string stype = split[i++];

				/* Lack of a name, on the other hand... */
				if(i >= split.Length) {
					clean_specs(h);
					return Error.GENERIC;
				}
				string name = split[i++];

				/* Grab a type, check to see if we have a mandatory type
				 * following an optional type. */
				PARSE_T type = parse_type(stype);
				if (type == PARSE_T.NONE)
				{
					clean_specs(h);
					return Error.GENERIC;
				}
				if ((type & PARSE_T.OPT) == 0 && h.specs.Count > 0 && (h.specs.Last().type & PARSE_T.OPT) != 0)
				{
					clean_specs(h);
					return Error.GENERIC;
				}
				if (h.specs.Count > 0 && ((h.specs.Last().type & ~PARSE_T.OPT) == PARSE_T.STR))
				{
					clean_specs(h);
					return Error.GENERIC;
				}

				/* Save this spec. */
				Hook_Spec hs = new Hook_Spec();
				hs.type = type;
				hs.name = name;
				h.specs.Add(hs);
			}

			return Error.NONE;
		}

		
		static PARSE_T parse_type(string s) {
			PARSE_T rv = 0;
			if(s[0] == '?') {
				rv |= PARSE_T.OPT;
				s = s.Substring(1); //Skip forward a character
			}
			if(s == "int")
				return PARSE_T.INT | rv;
			if(s == "sym")
				return PARSE_T.SYM | rv;
			if(s == "str")
				return PARSE_T.STR | rv;
			if(s == "rand")
				return PARSE_T.RAND | rv;
			if(s == "uint")
				return PARSE_T.UINT | rv;
			if(s == "char")
				return PARSE_T.CHAR | rv;
			return PARSE_T.NONE;
		}

		private static void clean_specs(Parser_Hook h) {
			h.dir = "";
			h.specs = new List<Hook_Spec>();
			h.func = null;
		}

		public static Error Ignored(Parser p) {
			return Error.NONE;
		}

		/** Returns the integer named `name`. This symbol must exist. */
		public int getint(string name) {
			Parser_Value v = getval(name);
			Misc.assert((v.spec.type & ~PARSE_T.OPT) == PARSE_T.INT);
			return (int)v.value;
		}

		public uint getuint(string name) {
			Parser_Value v = getval(name);
			Misc.assert((v.spec.type & ~PARSE_T.OPT) == PARSE_T.UINT);
			return (uint)v.value;
		}

		public char getchar(string name) {
			Parser_Value v = getval(name);
			Misc.assert((v.spec.type & ~PARSE_T.OPT) == PARSE_T.CHAR);
			return (char)v.value;
		}

		/** Returns the symbol named `name`. This symbol must exist. */
		public string getsym(string name) {
			Parser_Value v = getval(name);
			Misc.assert((v.spec.type & ~PARSE_T.OPT) == PARSE_T.SYM);
			return (string)v.value;
		}

		/** Returns the string named `name`. This symbol must exist. */
		public string getstr(string name) {
			Parser_Value v = getval(name);
			Misc.assert((v.spec.type & ~PARSE_T.OPT) == PARSE_T.STR);
			return (string)v.value;
		}

		public random_value getrand(string name) {
			Parser_Value v = getval(name);
			Misc.assert((v.spec.type & ~PARSE_T.OPT) == PARSE_T.RAND);
			return (random_value)v.value;
		}

		private Parser_Value getval(string name) {
			foreach (Parser_Value v in values){
				if (v.spec.name == name){
					return v;
				}
			}
			Misc.assert(false);
			return null;
		}

		/* The basic file parsing function */
		public Error parse_file(string filename, bool build_path = true) {
			string path = "";

			if(build_path) {
				path = Misc.path_build(Misc.ANGBAND_DIR_EDIT, filename + ".txt");
			} else {
				path = filename;
			}
			
			FileStream fs = File.OpenRead(path);

			StreamReader sr = new StreamReader(fs);
			while(!sr.EndOfStream) {
				Parse(sr.ReadLine());
			}

			fs.Close();
			
			return Error.NONE;
		}

		public bool hasval(string name) {
			//Nick's implementation
			foreach(Parser_Value p in values) {
				if(p.spec.name == name) {
					return true;
				}
			}
			return false;
			//struct parser_value *v;
			//for (v = p.fhead; v; v = (struct parser_value *)v.spec.next)
			//{
			//    if (!strcmp(v.spec.name, name))
			//        return true;
			//}
			//return false;
		}

		public enum PARSE_T {
			NONE = 0,
			INT = 2,
			SYM = 4,
			STR = 6,
			RAND = 8,
			UINT = 10,
			CHAR = 12,
			OPT = 0x00000001
		};

		public enum Error {
			NONE = 0,
			GENERIC,
			INVALID_FLAG,
			INVALID_ITEM_NUMBER,
			INVALID_SPELL_FREQ,
			INVALID_VALUE,
			INVALID_COLOR,
			INVALID_EFFECT,
			INVALID_OPTION,
			MISSING_FIELD,
			MISSING_RECORD_HEADER,
			FIELD_TOO_LONG,
			NON_SEQUENTIAL_RECORDS,
			NOT_NUMBER,
			NOT_RANDOM,
			OBSOLETE_FILE,
			OUT_OF_BOUNDS,
			OUT_OF_MEMORY,
			TOO_FEW_ENTRIES,
			TOO_MANY_ENTRIES,
			UNDEFINED_DIRECTIVE,
			UNRECOGNISED_BLOW,
			UNRECOGNISED_TVAL,
			UNRECOGNISED_SVAL,
			VAULT_TOO_BIG,
			INTERNAL,

			MAX
		};
	}
}
