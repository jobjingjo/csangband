using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CSAngband {
	class Prefs {
		/**
		 * Private data for pref file parsing.
		 */
		class prefs_data
		{
			public bool bypass;
			public keypress[] keymap_buffer = new keypress[Keymap.ACTION_MAX];
			public bool user;
			public bool[] loaded_window_flag = new bool[Misc.ANGBAND_TERM_MAX];
			public uint[] window_flags = new uint[Misc.ANGBAND_TERM_MAX];
		};

		/*
		 * Process the user pref file with the given name.
		 * "quiet" means "don't complain about not finding the file.
		 *
		 * 'user' should be true if the pref file loaded is user-specific and not
		 * a game default.
		 *
		 * Returns true if everything worked OK, false otherwise
		 */
		public static bool process_pref_file(string name, bool quiet, bool user)
		{
			//Nick: Rewritten from scratch, since io is so different from C
			string path = Misc.path_build(Misc.ANGBAND_DIR_PREF, name);

			if(!File.Exists(path)) {
			    path = Misc.path_build(Misc.ANGBAND_DIR_USER, name);
			    if(!File.Exists(path)){
			        if (!quiet){
			            Utilities.msg("Can not open " + path + ".");
			        }
			        return false;
			    }
			}

			FileStream f = null;
			try {
			    f = File.OpenRead(path);
			} catch {
			    if (!quiet){
			        Utilities.msg("Can not open " + path + ".");
			    }
			    return false;
			} finally {
				if (f != null){
					f.Close(); //Just wanted to see if we could open it before proceeding...
				}
			}

						
			Parser p = init_parse_prefs(user);
			Parser.Error e = p.parse_file(path, false);

			finish_parse_prefs(p);
			p.Destroy();

			return e == Parser.Error.NONE;

			//Below I wrote until I realized that there exists a Parser.ParseFile...
			//StreamReader sr = new StreamReader(f);
			

			//while (!sr.EndOfStream)
			//{
			//    line_no++;

			//    e = p.Parse(line);
			//    if (e != Parser.Error.NONE)
			//    {
			//        print_error(buf, p);
			//        break;
			//    }
			//}
			//finish_parse_prefs(p);

			//f.Close();
			//p.Destroy();

			///* Result */
			//return e == Parser.Error.NONE;


			//======================Original below for comparison
			//char buf[1024];

			//ang_file *f;
			//Parser p;
			//int e = 0;

			//int line_no = 0;

			///* Build the filename */
			//path_build(buf, sizeof(buf), ANGBAND_DIR_PREF, name);
			//if (!file_exists(buf))
			//    path_build(buf, sizeof(buf), ANGBAND_DIR_USER, name);

			//f = file_open(buf, MODE_READ, -1);
			//if (!f)
			//{
			//    if (!quiet)
			//        msg("Cannot open '%s'.", buf);
			//}
			//else
			//{
			//    char line[1024];

			//    p = init_parse_prefs(user);
			//    while (file_getl(f, line, sizeof line))
			//    {
			//        line_no++;

			//        e = parser_parse(p, line);
			//        if (e != Parser.Error.NONE)
			//        {
			//            print_error(buf, p);
			//            break;
			//        }
			//    }
			//    finish_parse_prefs(p);

			//    file_close(f);
			//    mem_free(p.priv);
			//    parser_destroy(p);
			//}

			///* Result */
			//return e == Parser.Error.NONE;
		}

		/*
		 * Helper function for "process_pref_file()"
		 *
		 * Input:
		 *   v: output buffer array
		 *   f: final character
		 *
		 * Output:
		 *   result
		 */
		static string process_pref_file_expr(ref string sp, ref char fp)
		{
			char f = ' ';

			/* Initial */
			//we might actually be refering the input directly...
			string s = sp;

			/* Skip spaces */
			while (Char.IsWhiteSpace(s[0])){
				s = s.Substring(1);
			}

			/* Save start */
			string b = s;

			/* Default */
			string v = "?o?o?"; //WHAT IS THIS?!?!

			/* Analyze */
			if (s[0] == '[')
			{
				//string p;

				/* Skip [ */
				s = s.Substring(1);

				/* First */
				string t = process_pref_file_expr(ref s, ref f);

				/* Oops */
				if (t == null)
				{
					/* Nothing */ 
				}

				/* Function: IOR */
				else if (t == "IOR")
				{
					v = "0";
					while (s.Length > 0 && (f != ']'))
					{
						t = process_pref_file_expr(ref s, ref f);
						if (t.Length > 0 && t != "0") v = "1";
					}
				}

				/* Function: AND */
				else if (t == "AND")
				{
					v = "1";
					while (s.Length > 0 && (f != ']'))
					{
						t = process_pref_file_expr(ref s, ref f);
						if (t.Length > 0 && t == "0") v = "0";
					}
				}

				/* Function: NOT */
				else if (t == "NOT")
				{
					v = "1";
					while (s.Length > 0 && (f != ']'))
					{
						t = process_pref_file_expr(ref s, ref f);
						if (t.Length > 0 && t != "0") v = "0";
					}
				}

				/* Function: EQU */
				else if (t == "EQU")
				{
					v = "1";
					if (s.Length > 0 && (f != ']'))
					{
						t = process_pref_file_expr(ref s, ref f);
					}
					while (s.Length > 0 && (f != ']'))
					{
						string p = t;
						t = process_pref_file_expr(ref s, ref f);
						if (t.Length > 0 && !(p == t)) v = "0";
					}
				}

				/* Function: LEQ */
				else if (t == "LEQ")
				{
					v = "1";
					if (s.Length > 0 && (f != ']'))
					{
						t = process_pref_file_expr(ref s, ref f);
					}
					while (s.Length > 0 && (f != ']'))
					{
						throw new NotImplementedException();
						//p = t;
						//t = process_pref_file_expr(ref s, ref f);
						//if (t.Length > 0 && (strcmp(p, t) >= 0)) v = "0";
					}
				}

				/* Function: GEQ */
				else if (t == "GEQ")
				{
					v = "1";
					if (s.Length > 0 && (f != ']'))
					{
						t = process_pref_file_expr(ref s, ref f);
					}
					while (s.Length > 0 && (f != ']'))
					{
						throw new NotImplementedException();
						//p = t;
						//t = process_pref_file_expr(ref s, ref f);
						//if (t.Length > 0 && (strcmp(p, t) <= 0)) v = "0";
					}
				}

				/* Oops */
				else
				{
					while (s.Length > 0 && (f != ']'))
					{
						t = process_pref_file_expr(ref s, ref f);
					}
				}

				/* Verify ending */
				if (f != ']') v = "?x?x?";

				/* Extract final and Terminate */
				if (s.Length == 0){
					f = '\0';
				} else {
					f = s[0];
					s = s.Substring(1);
				}
				//Orginal:
				//if ((f = s[0]) != '\0') *s++ = '\0';
			}

			/* Other */
			else
			{
				/* Accept all printables except spaces and brackets */
				//while (isprint((unsigned char)*s) && !strchr(" []", *s)) ++s;
				//I am just forgetting the "isprint" thing entirely...
				while (s.Length > 0 && !" []".Contains(s[0])){
					s = s.Substring(1);
				}

				/* Extract final and Terminate */
				//if ((f = *s) != '\0') *s++ = '\0';
				if (s.Length == 0){
					f = '\0';
				} else {
					f = s[0];
					s = s.Substring(1);
				}

				/* Variable */
				if (b[0] == '$')
				{
					b = b.Substring(1);
					if (b == "SYS")
						v = Misc.ANGBAND_SYS;
					else if (b == "GRAF")
						v = Misc.ANGBAND_GRAF;
					else if (b == "RACE")
						v = Player.Player.instance.Race.Name;
					else if (b == "CLASS")
						v = Player.Player.instance.Class.Name;
					else if (b == "PLAYER")
						v = Player.Player_Other.instance.base_name;
				}

				/* Constant */
				else
				{
					v = b;
				}
			}

			/* Save */
			fp = f;
			sp = s;

			return v;
		}

		static Parser.Error parse_prefs_expr(Parser p)
		{
			prefs_data d = p.priv as prefs_data;

			string v;
			string str;
			string expr;
			char f = '\0'; //Nick: No default originally, I am just guessing here...

			Misc.assert(d != null);

			/* XXX this can be avoided with a rewrite of process_pref_file_expr */
			str = expr = p.getstr("expr");

			/* Parse the expr */
			v = process_pref_file_expr(ref expr, ref f);

			/* Set flag */
			d.bypass = (v == "0");

			return Parser.Error.NONE;
		}

		static Parser.Error parse_prefs_k(Parser p)
		{
			throw new NotImplementedException();
			//int tvi, svi;
			//object_kind *kind;

			//prefs_data d = p.priv as prefs_data;
			//Misc.assert(d != null);
			//if (d.bypass) return Parser.Error.NONE;

			//tvi = tval_find_idx(p.getsym("tval"));
			//if (tvi < 0)
			//    return Parser.Error.UNRECOGNISED_TVAL;

			//svi = lookup_sval(tvi, p.getsym("sval"));
			//if (svi < 0)
			//    return Parser.Error.UNRECOGNISED_SVAL;

			//kind = lookup_kind(tvi, svi);
			//if (!kind)
			//    return Parser.Error.UNRECOGNISED_SVAL;

			//kind.x_attr = (byte)p.getint("attr");
			//kind.x_char = (char)p.getint("char");

			//return Parser.Error.NONE;
		}

		static Parser.Error parse_prefs_r(Parser p)
		{
			throw new NotImplementedException();
			//int idx;
			//monster_race *monster;

			//prefs_data d = p.priv as prefs_data;
			//Misc.assert(d != null);
			//if (d.bypass) return Parser.Error.NONE;

			//idx = parser_getuint(p, "idx");
			//if (idx >= z_info.r_max)
			//    return Parser.Error.OUT_OF_BOUNDS;

			//monster = &r_info[idx];
			//monster.x_attr = (byte)p.getint("attr");
			//monster.x_char = (char)p.getint("char");

			//return Parser.Error.NONE;
		}

		static Parser.Error parse_prefs_f(Parser p)
		{
			int idx;
			Feature feature;

			string lighting;
			int light_idx;

			prefs_data d = p.priv as prefs_data;
			Misc.assert(d != null);
			if (d.bypass) return Parser.Error.NONE;

			idx = (int)p.getuint("idx");
			if (idx >= Misc.z_info.f_max)
			    return Parser.Error.OUT_OF_BOUNDS;

			lighting = p.getsym("lighting");
			if (lighting == "bright")
			    light_idx = (int)Grid_Data.grid_light_level.FEAT_LIGHTING_BRIGHT;
			else if (lighting == "lit")
			    light_idx = (int)Grid_Data.grid_light_level.FEAT_LIGHTING_LIT;
			else if (lighting == "dark")
			    light_idx = (int)Grid_Data.grid_light_level.FEAT_LIGHTING_DARK;
			else if (lighting == "all")
			    light_idx = (int)Grid_Data.grid_light_level.FEAT_LIGHTING_MAX;
			else
			    return Parser.Error.GENERIC; /* xxx fixme */

			if (light_idx < (int)Grid_Data.grid_light_level.FEAT_LIGHTING_MAX)
			{
			    feature = Misc.f_info[idx];
			    feature.x_attr[light_idx] = (ConsoleColor)p.getint("attr");
			    feature.x_char[light_idx] = (char)p.getint("char");
			}
			else
			{
			    for (light_idx = 0; light_idx < (int)Grid_Data.grid_light_level.FEAT_LIGHTING_MAX; light_idx++)
			    {
			        feature = Misc.f_info[idx];
			        feature.x_attr[light_idx] = (ConsoleColor)p.getint("attr");
			        feature.x_char[light_idx] = (char)p.getint("char");
			    }
			}

			return Parser.Error.NONE;
		}

		static Parser.Error parse_prefs_gf(Parser p)
		{
			throw new NotImplementedException();
			//bool types[GF_MAX] = { 0 };
			//string direction;
			//int motion;

			//string s, *t;

			//size_t i;

			//prefs_data d = p.priv as prefs_data;
			//Misc.assert(d != null);
			//if (d.bypass) return Parser.Error.NONE;

			///* Parse the type, which is a | seperated list of GF_ constants */
			//s = string_make(p.getsym("type"));
			//t = strtok(s, "| ");
			//while (t) {
			//    if (streq(t, "*")) {
			//        memset(types, true, sizeof types);
			//    } else {
			//        int idx = gf_name_to_idx(t);
			//        if (idx == -1)
			//            return Parser.Error.INVALID_VALUE;

			//        types[idx] = true;
			//    }

			//    t = strtok(null, "| ");
			//}

			//string_free(s);

			//direction = p.getsym("direction");
			//if (streq(direction, "static"))
			//    motion = BOLT_NO_MOTION;
			//else if (streq(direction, "0"))
			//    motion = BOLT_0;
			//else if (streq(direction, "45"))
			//    motion = BOLT_45;
			//else if (streq(direction, "90"))
			//    motion = BOLT_90;
			//else if (streq(direction, "135"))
			//    motion = BOLT_135;
			//else
			//    return Parser.Error.INVALID_VALUE;

			//for (i = 0; i < GF_MAX; i++) {
			//    if (!types[i]) continue;

			//    gf_to_attr[i][motion] = (byte)parser_getuint(p, "attr");
			//    gf_to_char[i][motion] = (char)parser_getuint(p, "char");
			//}

			//return Parser.Error.NONE;
		}

		static Parser.Error parse_prefs_l(Parser p)
		{
			throw new NotImplementedException();
			//unsigned int idx;
			//struct flavor *flavor;

			//prefs_data d = p.priv as prefs_data;
			//Misc.assert(d != null);
			//if (d.bypass) return Parser.Error.NONE;

			//idx = parser_getuint(p, "idx");
			//for (flavor = flavors; flavor; flavor = flavor.next)
			//    if (flavor.fidx == idx)
			//        break;

			//if (flavor) {
			//    flavor.x_attr = (byte)p.getint("attr");
			//    flavor.x_char = (char)p.getint("char");
			//}

			//return Parser.Error.NONE;
		}

		static Parser.Error parse_prefs_e(Parser p)
		{
			throw new NotImplementedException();
			//int tvi, a;

			//prefs_data d = p.priv as prefs_data;
			//Misc.assert(d != null);
			//if (d.bypass) return Parser.Error.NONE;

			//tvi = tval_find_idx(p.getsym("tval"));
			//if (tvi < 0 || tvi >= (long)N_ELEMENTS(tval_to_attr))
			//    return Parser.Error.UNRECOGNISED_TVAL;

			//a = p.getint("attr");
			//if (a) tval_to_attr[tvi] = (byte) a;

			//return Parser.Error.NONE;
		}

		static Parser.Error parse_prefs_q(Parser p)
		{
			throw new NotImplementedException();
			//prefs_data d = p.priv as prefs_data;
			//Misc.assert(d != null);
			//if (d.bypass) return Parser.Error.NONE;

			//if (parser_hasval(p, "sval") && parser_hasval(p, "flag"))
			//{
			//    object_kind *kind;
			//    int tvi, svi;

			//    tvi = tval_find_idx(p.getsym("n"));
			//    if (tvi < 0)
			//        return Parser.Error.UNRECOGNISED_TVAL;
	
			//    svi = lookup_sval(tvi, p.getsym("sval"));
			//    if (svi < 0)
			//        return Parser.Error.UNRECOGNISED_SVAL;

			//    kind = lookup_kind(tvi, svi);
			//    if (!kind)
			//        return Parser.Error.UNRECOGNISED_SVAL;

			//    kind.squelch = p.getint("flag");
			//}
			//else
			//{
			//    int idx = p.getint("idx");
			//    int level = p.getint("n");

			//    squelch_level[idx] = level;
			//}

			//return Parser.Error.NONE;
		}

		static Parser.Error parse_prefs_b(Parser p)
		{
			throw new NotImplementedException();
			//int idx;

			//prefs_data d = p.priv as prefs_data;
			//Misc.assert(d != null);
			//if (d.bypass) return Parser.Error.NONE;

			//idx = parser_getuint(p, "idx");
			//if (idx > z_info.k_max)
			//    return Parser.Error.OUT_OF_BOUNDS;

			//add_autoinscription(idx, p.getstr("text"));

			//return Parser.Error.NONE;
		}

		static Parser.Error parse_prefs_a(Parser p)
		{
			string act;

			prefs_data d = p.priv as prefs_data;
			Misc.assert(d != null);
			if (d.bypass) return Parser.Error.NONE;

			act = p.getstr("act");
			ui_event[] evts = UIEvent.keypress_from_text(act);

			d.keymap_buffer = new keypress[evts.Count()];
			for(int i = 0; i < evts.Count(); i++) {
				d.keymap_buffer[i] = evts[i].key;
			}

			return Parser.Error.NONE;
		}

		static Parser.Error parse_prefs_c(Parser p)
		{
			int mode;

			prefs_data d = p.priv as prefs_data;
			Misc.assert(d != null);
			if (d.bypass) return Parser.Error.NONE;

			mode = p.getint("mode");
			if (mode < 0 || mode >= (int)Keymap.Mode.MAX)
			    return Parser.Error.OUT_OF_BOUNDS;

			ui_event[] temp = UIEvent.keypress_from_text(p.getstr("key"));

			if (temp[0].type != ui_event_type.EVT_KBRD || temp.Length > 1)
			    return Parser.Error.FIELD_TOO_LONG;

			Keymap.add(mode, temp[0].key, d.keymap_buffer, d.user);

			return Parser.Error.NONE;
		}

		static Parser.Error parse_prefs_m(Parser p)
		{
			Message_Type type;
			string attr;

			prefs_data d = p.priv as prefs_data;
			Misc.assert(d != null);
			if (d.bypass) return Parser.Error.NONE;

			type = (Message_Type)p.getint("type");
			attr = p.getsym("attr");

			ConsoleColor a = Utilities.color_text_to_attr(attr);

			if (a < 0)
			    return Parser.Error.INVALID_COLOR;

			Message.color_define(type, a);

			return Parser.Error.NONE;
		}

		static Parser.Error parse_prefs_v(Parser p)
		{
			throw new NotImplementedException();
			//int idx;

			//prefs_data d = p.priv as prefs_data;
			//Misc.assert(d != null);
			//if (d.bypass) return Parser.Error.NONE;

			//idx = parser_getuint(p, "idx");
			//if (idx > MAX_COLORS)
			//    return Parser.Error.OUT_OF_BOUNDS;

			//angband_color_table[idx][0] = p.getint("k");
			//angband_color_table[idx][1] = p.getint("r");
			//angband_color_table[idx][2] = p.getint("g");
			//angband_color_table[idx][3] = p.getint("b");

			//return Parser.Error.NONE;
		}

		static Parser.Error parse_prefs_w(Parser p)
		{
			throw new NotImplementedException();
			//int window;
			//size_t flag;

			//prefs_data d = p.priv as prefs_data;
			//Misc.assert(d != null);
			//if (d.bypass) return Parser.Error.NONE;

			//window = p.getint("window");
			//if (window <= 0 || window >= ANGBAND_TERM_MAX)
			//    return Parser.Error.OUT_OF_BOUNDS;

			//flag = parser_getuint(p, "flag");
			//if (flag >= N_ELEMENTS(window_flag_desc))
			//    return Parser.Error.OUT_OF_BOUNDS;

			//if (window_flag_desc[flag])
			//{
			//    int value = parser_getuint(p, "value");
			//    if (value)
			//        d.window_flags[window] |= (1L << flag);
			//    else
			//        d.window_flags[window] &= ~(1L << flag);
			//}

			//d.loaded_window_flag[window] = true;

			//return Parser.Error.NONE;
		}

		static Parser.Error parse_prefs_x(Parser p)
		{
			throw new NotImplementedException();
			//prefs_data d = p.priv as prefs_data;
			//Misc.assert(d != null);
			//if (d.bypass) return Parser.Error.NONE;

			///* XXX check for valid option */
			//option_set(p.getstr("option"), false);

			//return Parser.Error.NONE;
		}

		static Parser.Error parse_prefs_y(Parser p)
		{
			throw new NotImplementedException();
			//prefs_data d = p.priv as prefs_data;
			//Misc.assert(d != null);
			//if (d.bypass) return Parser.Error.NONE;

			//option_set(p.getstr("option"), true);

			//return Parser.Error.NONE;
		}


		static Parser init_parse_prefs(bool user)
		{
			Parser p = new Parser();
			prefs_data pd = new prefs_data();
			int i;

			p.priv = pd;
			pd.user = user;
			for (i = 0; i < Misc.ANGBAND_TERM_MAX; i++) {
			    pd.loaded_window_flag[i] = false;
			}

			p.Register("% str file", parse_prefs_load);
			p.Register("? str expr", parse_prefs_expr);
			p.Register("K sym tval sym sval int attr int char", parse_prefs_k);
			p.Register("R uint idx int attr int char", parse_prefs_r);
			p.Register("F uint idx sym lighting int attr int char", parse_prefs_f);
			p.Register("GF sym type sym direction uint attr uint char", parse_prefs_gf);
			p.Register("L uint idx int attr int char", parse_prefs_l);
			p.Register("E sym tval int attr", parse_prefs_e);
			p.Register("Q sym idx sym n ?sym sval ?sym flag", parse_prefs_q);
			    /* XXX should be split into two kinds of line */
			p.Register("B uint idx str text", parse_prefs_b);
			    /* XXX idx should be {tval,sval} pair! */
			p.Register("A str act", parse_prefs_a);
			p.Register("C int mode str key", parse_prefs_c);
			p.Register("M int type sym attr", parse_prefs_m);
			p.Register("V uint idx int k int r int g int b", parse_prefs_v);
			p.Register("W int window uint flag uint value", parse_prefs_w);
			p.Register("X str option", parse_prefs_x);
			p.Register("Y str option", parse_prefs_y);

			return p;
		}

		static Parser.Error finish_parse_prefs(Parser p)
		{
			prefs_data d = p.priv as prefs_data;
			int i;

			/* Update sub-windows based on the newly read-in prefs.
			 *
			 * The op_ptr.window_flag[] array cannot be updated directly during
			 * parsing since the changes between the existing flags and the new
			 * are used to set/unset the event handlers that update the windows.
			 *
			 * Build a complete set to pass to subwindows_set_flags() by loading
			 * any that weren't read in by the parser from the existing set.
			 */
			for (i = 0; i < Misc.ANGBAND_TERM_MAX; i++) {
			    if (!d.loaded_window_flag[i])
			        d.window_flags[i] = Player.Player_Other.instance.window_flag[i];
			}
			Misc.subwindows_set_flags(d.window_flags, Misc.ANGBAND_TERM_MAX);

			return Parser.Error.NONE;
		}

		/**
		 * Load another file.
		 */
		static Parser.Error parse_prefs_load(Parser p)
		{
			prefs_data d = p.priv as prefs_data;
			string file;

			Misc.assert(d != null);
			if (d.bypass) return Parser.Error.NONE;

			file = p.getstr("file");
			process_pref_file(file, true, d.user);

			return Parser.Error.NONE;
		}
	}
}
