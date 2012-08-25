using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;
using CSAngband.Monster;
using CSAngband.Player;

namespace CSAngband {
	partial class Init {
		/* Parsing functions for limits.txt */

#region Limits Parser

		public static Parser.Error parse_z(Parser p) {
			Maxima z;
			string label;
			UInt16 value;

			z = (Maxima)p.priv;
			label = p.getsym("label");
			value = (UInt16)p.getint("value");

			if (value < 0)
				return Parser.Error.INVALID_VALUE;

			if (label == "F")
				z.f_max = value;
			else if (label == "K")
				z.k_max = value;
			else if (label == "A")
				z.a_max = value;
			else if (label == "E")
				z.e_max = value;
			else if(label == "R") {
				z.r_max = value;
				//HACK! This is literally the earliest we can instatiate it, so that later on, we can do it again @_@
				Misc.l_list = new Monster_Lore[value];
			} else if(label == "P")
				z.mp_max = value;
			else if(label == "S")
				z.s_max = value;
			else if(label == "O")
				z.o_max = value;
			else if(label == "M")
				z.m_max = value;
			else if(label == "I")
				z.pit_max = value;
			else
				return Parser.Error.UNDEFINED_DIRECTIVE;

			return 0;
		}

		public static Parser init_parse_z() {
			Maxima z = new Maxima();
			Parser p = new Parser();

			p.priv = z;
			p.Register("V sym version", Parser.Ignored);
			p.Register("M sym label int value", parse_z);
			return p;
		}

		public static Parser.Error run_parse_z(Parser p) {
			return p.parse_file("limits");
		}

		public static Parser.Error finish_parse_z(Parser p) {
			Misc.z_info = (Maxima)p.priv;
			p.Destroy();
			return 0;
		}

		static void cleanup_z()
		{
			Misc.z_info = null;
		}

		static File_Parser z_parser = new File_Parser("limits", init_parse_z, run_parse_z, finish_parse_z, cleanup_z);

#endregion

#region Terrain/Feature Parser

		public static Parser init_parse_f() {
			Parser p = new Parser();
			p.priv = null;
			p.Register("V sym version", Parser.Ignored);
			p.Register("N uint index str name", parse_f_n);
			p.Register("G char glyph sym color", parse_f_g);
			p.Register("M uint index", parse_f_m);
			p.Register("P uint priority", parse_f_p);
			p.Register("F ?str flags", parse_f_f);
			p.Register("X int locked int jammed int shopnum int dig", parse_f_x);
			p.Register("E str effect", parse_f_e);
			return p;
		}

		public static Parser.Error run_parse_f(Parser p) {
			return p.parse_file("terrain");
		}

		public static Parser.Error finish_parse_f(Parser p) {
			Feature f, n;

			Misc.f_info = new Feature[Misc.z_info.f_max];
			for (f = (Feature)p.priv; f != null ; f = f.next) {
				if (f.fidx >= Misc.z_info.f_max)
					continue;
				Misc.f_info[f.fidx] = f;
			}

			//f = (Feature)p.priv;
			//while (f != null) {
			//    n = f.next;
			//    //free(f);
			//    f = n;
			//}

			p.Destroy();
			return 0;
		}

		static void cleanup_f() {
			int idx;
			for (idx = 0; idx < Misc.z_info.f_max; idx++) {
				Misc.f_info[idx].name = "";
			}
			Misc.f_info = null;
		}

		public static File_Parser f_parser = new File_Parser("terrain", init_parse_f, run_parse_f, finish_parse_f, cleanup_f);

		/* Parsing functions for terrain.txt */
		public static Parser.Error parse_f_n(Parser p) {
			int idx = (int)p.getuint("index");
			string name = p.getstr("name");
			Feature h = (Feature)p.priv;

			Feature f = new Feature();
			f.next = h;
			f.fidx = idx;
			f.mimic = (byte)idx;
			f.name = name;
			p.priv = f;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_f_g(Parser p) {
			char glyph = p.getchar("glyph");
			string color = p.getsym("color");
			ConsoleColor attr = 0;
			Feature f = (Feature)p.priv;

			if (f == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			f.d_char = glyph;
			attr = Utilities.color_text_to_attr(color);
			if (attr < 0)
			    return Parser.Error.INVALID_COLOR;
			f.d_attr = attr;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_f_m(Parser p) {
			uint idx = p.getuint("index");
			Feature f = p.priv as Feature;

			if (f == null)
				return Parser.Error.MISSING_RECORD_HEADER;
			f.mimic = (byte)idx;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_f_p(Parser p) {
			uint priority = p.getuint("priority");
			Feature f = p.priv as Feature;

			if (f == null)
				return Parser.Error.MISSING_RECORD_HEADER;
			f.priority = (byte)priority;
			return Parser.Error.NONE;
		}

		//Flags was a pointer
		private static Parser.Error grab_one_flag(out UInt32 flags, UInt32 in_flags, string[] names, string what)
		{
			flags = in_flags;
			/* Check flags */
			for (int i = 0; i < 32 && names.Length > i; i++)
			{
			    if (what == names[i])
			    {
			        flags |= (UInt32)(1L << i);
			        return (0);
			    }
			}

			return (Parser.Error)(-1);
		}

		public static Parser.Error parse_f_f(Parser p) {
			Feature f = p.priv as Feature;

			if (f == null) return Parser.Error.MISSING_RECORD_HEADER;

			if (!p.hasval("flags"))
			    return Parser.Error.NONE;
			string flags = p.getstr("flags");

			//Nick's implementation, need to test this!
			string[] tokens = flags.Split(new string[] { " ", "|" }, StringSplitOptions.RemoveEmptyEntries);

			foreach(string s in tokens) {
				if(grab_one_flag(out f.flags, f.flags, Misc.f_info_flags, s) != Parser.Error.NONE) {
					return Parser.Error.INVALID_FLAG;
				}
			}

			//old C way of doing it
			/*s = strtok(flags, " |");
			while (s) {
			    if (grab_one_flag(&f.flags, Misc.f_info_flags, s)) {
			        mem_free(s);
			        return Parser.Error.INVALID_FLAG;
			    }
			    s = strtok(null, " |");
			}*/

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_f_x(Parser p) {
			Feature f = p.priv as Feature;

			if (f == null)
				return Parser.Error.MISSING_RECORD_HEADER;
			f.locked = (byte)p.getint("locked");
			f.jammed = (byte)p.getint("jammed");
			f.shopnum = (byte)p.getint("shopnum");
			f.dig = (byte)p.getint("dig");
			return Parser.Error.NONE;
		}

		static Effect grab_one_effect(string what) {
			foreach(Effect e in Effect.list) {
				if(e.name == what) {
					return e;
				}
			}

			/* Oops */
			Utilities.msg("Unknown effect '" + what + "'");

			/* Error */
			return null;
		}

		public static Parser.Error parse_f_e(Parser p) {
			Feature f = p.priv as Feature;

			if (f == null) return Parser.Error.MISSING_RECORD_HEADER;
			f.effect = grab_one_effect(p.getstr("effect"));
			if (f.effect == null) return Parser.Error.INVALID_EFFECT;
			return Parser.Error.NONE;
		}
#endregion

#region Object Base Parser
		/* Parsing functions for object_base.txt */
		class kb_parsedata {
			public kb_parsedata(){

			}
			public Object.Object_Base defaults;
			public Object.Object_Base kb; //A head node basically...
		}

		public static Parser init_parse_kb() {
			Parser p = new Parser();

			kb_parsedata d = new kb_parsedata();
			d.defaults = new Object.Object_Base();

			p.priv = d;

			p.Register("V sym version", Parser.Ignored);
			p.Register("D sym label int value", parse_kb_d);
			p.Register("N sym tval str name", parse_kb_n);
			p.Register("B int breakage", parse_kb_b);
			p.Register("F str flags", parse_kb_f);
			return p;
		}

		public static Parser.Error run_parse_kb(Parser p) {
			return p.parse_file("object_base");
		}

		public static Parser.Error finish_parse_kb(Parser p) {
			kb_parsedata d = p.priv as kb_parsedata;

			Misc.assert(d != null);

			Misc.kb_info = new Object.Object_Base[Object.TVal.TV_MAX];

			for (Object.Object_Base kb = d.kb; kb != null; kb = kb.Next) {
			    if (kb.tval >= Object.TVal.TV_MAX)
			        continue;
				Misc.kb_info[kb.tval] = kb;
			}

			p.Destroy();
			return Parser.Error.NONE;
		}

		static void cleanup_kb()
		{
			throw new NotImplementedException();
			//int idx;
			//for (idx = 0; idx < Object.TVal.TV_MAX; idx++)
			//{
			//    string_free(kb_info[idx].name);
			//}
			//mem_free(kb_info);
		}

		public static File_Parser kb_parser = new File_Parser(
			"object_base",
			init_parse_kb,
			run_parse_kb,
			finish_parse_kb,
			cleanup_kb
		);


		public static Parser.Error parse_kb_d(Parser p) {
			string label;
			int value;

			kb_parsedata d = p.priv as kb_parsedata;
			Misc.assert(d != null);

			label = p.getsym("label");
			value = p.getint("value");

			if (label == "B")
				d.defaults.break_perc = value;
			else
				return Parser.Error.UNDEFINED_DIRECTIVE;

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_kb_n(Parser p) {
			kb_parsedata d = p.priv as kb_parsedata;
			Misc.assert(d != null);

			Object.Object_Base kb = new Object.Object_Base();
			kb.Copy(d.defaults);

			kb.Next = d.kb;
			d.kb = kb;

			kb.tval = Object.TVal.find_idx(p.getsym("tval"));
			if (kb.tval == -1)
			    return Parser.Error.UNRECOGNISED_TVAL;

			kb.Name = p.getstr("name");

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_kb_b(Parser p) {
			Object.Object_Base kb;

			kb_parsedata d = p.priv as kb_parsedata;
			Misc.assert(d != null);

			kb = d.kb;
			Misc.assert(kb != null);

			kb.break_perc = p.getint("breakage");

			return Parser.Error.NONE;
		}

		static int lookup_flag(List<Object.Object_Flag> flag_table, string flag_name) {
			int i = 0;

			while ( i < flag_table.Count && flag_table[i].name != flag_name)
				i++;

			/* End of table reached without match */
			if (i == flag_table.Count) i = -1; //Error!

			return i;
		}

		static Parser.Error grab_flag(Bitflag flags, List<Object.Object_Flag> flag_table, string flag_name) {
			int flag = lookup_flag(flag_table, flag_name);

			if (flag == -1) return Parser.Error.INVALID_FLAG;

			flags.on(flag);

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_kb_f(Parser p) {
			Object.Object_Base kb;

			kb_parsedata d = p.priv as kb_parsedata;
			Misc.assert(d != null);

			kb = d.kb;
			Misc.assert(kb != null);

			string s = p.getstr("flags");

			//What I believe the below is doing
			string[] t = s.Split(new string[] { " ", "|" }, StringSplitOptions.RemoveEmptyEntries);
			string temp;
			foreach(string token in t) {
			    temp = token;
			    if(grab_flag(kb.flags, Object.Object_Flag.list, token) != Parser.Error.NONE) {
					return Parser.Error.INVALID_FLAG;
			    }
			}

			return Parser.Error.NONE;

			/* DA FUQ?
			string t = strtok(s, " |");
			while (t) {
			    if (grab_flag(kb.flags, OF_SIZE, k_info_flags, t))
			        break;
			    t = strtok(null, " |");
			}
			mem_free(s);

			return t ? Parser.Error.INVALID_FLAG : Parser.Error.NONE;*/
		}


#endregion

#region Object Parser

		public static Parser init_parse_k() {
			Parser p = new Parser();
			p.priv = null;
			p.Register("V sym version", Parser.Ignored);
			p.Register("N int index str name", parse_k_n);
			p.Register("G sym char sym color", parse_k_g);
			p.Register("I sym tval int sval", parse_k_i);
			p.Register("W int level int extra int weight int cost", parse_k_w);
			p.Register("A int common str minmax", parse_k_a);
			p.Register("P int ac rand hd rand to-h rand to-d rand to-a", parse_k_p);
			p.Register("C rand charges", parse_k_c);
			p.Register("M int prob rand stack", parse_k_m);
			p.Register("F str flags", parse_k_f);
			p.Register("E sym name ?rand time", parse_k_e);
			p.Register("L rand pval ?str flags", parse_k_l);
			p.Register("D str text", parse_k_d);
			return p;
		}

		public static Parser.Error run_parse_k(Parser p) {
			return p.parse_file("object");
		}

		public static Parser.Error finish_parse_k(Parser p) {
			Misc.k_info = new Object_Kind[Misc.z_info.k_max];
			for (Object_Kind k = p.priv as Object_Kind; k != null; k = k.next) {
			    if (k.kidx >= Misc.z_info.k_max)
			        continue;
				Misc.k_info[k.kidx] = k;
			}

			Misc.objkinds = p.priv as Object_Kind;
			p.Destroy();
			return 0;
		}

		static void cleanup_k()
		{
			//int idx;
			//for (idx = 0; idx < Misc.z_info.k_max; idx++) {
			//    string_free(k_info[idx].name);
			//    mem_free(k_info[idx].text);
			//}
			//mem_free(k_info);
		}

		public static File_Parser k_parser = new File_Parser(
			"object",
			init_parse_k,
			run_parse_k,
			finish_parse_k,
			cleanup_k
		);


		/* Parsing functions for object.txt */

		public static Parser.Error parse_k_n(Parser p) {
			int idx = p.getint("index");
			string name = p.getstr("name");
			Object_Kind h = p.priv as Object_Kind;
			Object_Kind k = new Object_Kind();

			k.next = h;
			p.priv = k;
			k.kidx = (uint)idx;
			k.Name = name;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_k_g(Parser p) {
			Object_Kind k = p.priv as Object_Kind;
			Misc.assert(k != null);
			
			string sym = p.getsym("char");
			string color = p.getsym("color");
			

			k.d_char = sym[0];
			k.d_attr = Utilities.color_text_to_attr(color);

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_k_i(Parser p) {
			Object_Kind k = p.priv as Object_Kind;
			Misc.assert(k != null);

			int tval = TVal.find_idx(p.getsym("tval"));
			if (tval < 0)
			    return Parser.Error.UNRECOGNISED_TVAL;

			k.tval = (byte)tval;
			k.sval = (byte)p.getint("sval");

			k.Base = Misc.kb_info[k.tval];

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_k_w(Parser p) {
			Object_Kind k = p.priv as Object_Kind;
			Misc.assert(k != null);

			k.level = (byte)p.getint("level");
			k.weight = (byte)p.getint("weight");
			k.cost = p.getint("cost");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_k_a(Parser p) {
			Object_Kind k = p.priv as Object_Kind;
			string tmp = p.getstr("minmax");
			Misc.assert(k != null);

			k.alloc_prob = (byte)p.getint("common");

			int amin, amax;

			string[] tokens = tmp.Split(new string[] { " to " }, StringSplitOptions.RemoveEmptyEntries);

			if(tokens.Length != 2)
				return Parser.Error.GENERIC;

			if(!int.TryParse(tokens[0], out amin) || !int.TryParse(tokens[1], out amax))
				return Parser.Error.GENERIC;

			k.alloc_min = (byte)amin;
			k.alloc_max = (byte)amax;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_k_p(Parser p) {
			Object_Kind k = p.priv as Object_Kind;
			Misc.assert(k != null);
			
			random_value hd = p.getrand("hd");
			
			k.ac = (byte)p.getint("ac");
			k.dd = (byte)hd.dice;
			k.ds = (byte)hd.sides;
			k.to_h = p.getrand("to-h");
			k.to_d = p.getrand("to-d");
			k.to_a = p.getrand("to-a");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_k_c(Parser p) {
			Object_Kind k = p.priv as Object_Kind;
			Misc.assert(k != null);

			k.charge = p.getrand("charges");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_k_m(Parser p) {
			Object_Kind k = p.priv as Object_Kind;
			Misc.assert(k != null);

			k.gen_mult_prob = (byte)p.getint("prob");
			k.stack_size = (random_value)p.getrand("stack");
			return Parser.Error.NONE;
		}

		static Parser.Error grab_flag_helper(string s, Bitflag flags){
			string[] t = s.Split(new string[] { " ", "|" }, StringSplitOptions.RemoveEmptyEntries);
			string temp;
			foreach(string token in t) {
			    temp = token;
			    if(grab_flag(flags, Object_Flag.list, token) != Parser.Error.NONE) {
					return Parser.Error.INVALID_FLAG;
			    }
			}

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_k_f(Parser p) {
			Object_Kind k = p.priv as Object_Kind;
			Misc.assert(k != null);

			string s = p.getstr("flags");

			return grab_flag_helper(s, k.flags);			

			/*string t = strtok(s, " |");
			while (t) {
			    if (grab_flag(k.flags, OF_SIZE, k_info_flags, t))
			        break;
			    t = strtok(null, " |");
			}
			mem_free(s);
			return t ? Parser.Error.INVALID_FLAG : Parser.Error.NONE;*/
		}

		public static Parser.Error parse_k_e(Parser p) {
			Object_Kind k = p.priv as Object_Kind;
			Misc.assert(k != null);

			k.effect = grab_one_effect(p.getsym("name"));
			if (p.hasval("time"))
			    k.time = p.getrand("time");
			if (k.effect == null)
			    return Parser.Error.GENERIC;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_k_d(Parser p) {
			Object_Kind k = p.priv as Object_Kind;
			Misc.assert(k != null);
			k.Text = k.Text + p.getstr("text");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_k_l(Parser p) {
			Object_Kind k = p.priv as Object_Kind;
			Misc.assert(k != null);

			k.pval[k.num_pvals] = p.getrand("pval");

			if (!p.hasval("flags")) {
			    k.num_pvals++;
			    return Parser.Error.NONE;
			}

			string s = p.getstr("flags");
			//return grab_flag_helper(s, k.pval_flags[k.num_pvals]);
			
			string[] t = s.Split(new string[] { " ", "|" }, StringSplitOptions.RemoveEmptyEntries);
			string temp;
			foreach(string token in t) {
			    temp = token;
			    if(grab_flag(k.flags, Object_Flag.list, token) != Parser.Error.NONE ||
					grab_flag(k.pval_flags[k.num_pvals], Object_Flag.list, token) != Parser.Error.NONE) {
					
					return Parser.Error.INVALID_FLAG;
			    }
			}

			k.num_pvals++;
			if (k.num_pvals > Misc.MAX_PVALS)
			    return Parser.Error.TOO_MANY_ENTRIES;

			return Parser.Error.NONE;
			
			
			
			//t = strtok(s, " |");

			//while (t) {
			//    if (grab_flag(k.flags, OF_SIZE, k_info_flags, t) ||
			//        grab_flag(k.pval_flags[k.num_pvals], OF_SIZE, k_info_flags, t))
			//        break;

			//    t = strtok(null, " |");
			//}

			//k.num_pvals++;
			//if (k.num_pvals > MAX_PVALS)
			//    return Parser.Error.TOO_MANY_ENTRIES;

			//mem_free(s);
			//return t ? Parser.Error.INVALID_FLAG : Parser.Error.NONE;
		}


#endregion

#region Ego Item Parser

		public static Parser init_parse_e() {
			Parser p = new Parser();
			p.priv = null;
			p.Register("V sym version", Parser.Ignored);
			p.Register("N int index str name", parse_e_n);
			p.Register("W int level int rarity int pad int cost", parse_e_w);
			p.Register("X int rating int xtra", parse_e_x);
			p.Register("A int common str minmax", parse_e_a);
			p.Register("T sym tval int min-sval int max-sval", parse_e_t);
			p.Register("C rand th rand td rand ta", parse_e_c);
			p.Register("M int th int td int ta", parse_e_m);
			p.Register("F ?str flags", parse_e_f);
			p.Register("L rand pval int min str flags", parse_e_l);
			p.Register("D str text", parse_e_d);
			return p;
		}

		public static Parser.Error run_parse_e(Parser p) {
			return p.parse_file("ego_item");
		}

		public static Parser.Error finish_parse_e(Parser p) {
			Ego_Item e;

			Misc.e_info = new Ego_Item[Misc.z_info.e_max];
			for (e = p.priv as Ego_Item; e != null; e = e.next) {
			    if (e.eidx >= Misc.z_info.e_max)
			        continue;
				Misc.e_info[e.eidx] =  e;
			}

			Slay.create_slay_cache(Misc.e_info);

			p.Destroy();
			return 0;
		}

		static void cleanup_e()
		{
			//int idx;
			//for (idx = 0; idx < Misc.z_info.e_max; idx++) {
			//    string_free(e_info[idx].name);
			//    mem_free(e_info[idx].text);
			//}
			//mem_free(e_info);
			//free_slay_cache();
		}

		public static File_Parser e_parser = new File_Parser(
			"ego_item",
			init_parse_e,
			run_parse_e,
			finish_parse_e,
			cleanup_e
		);



		/* Parsing functions for ego-item.txt */
		public static Parser.Error parse_e_n(Parser p) {
			int idx = p.getint("index");
			string name = p.getstr("name");
			Ego_Item h = p.priv as Ego_Item;

			Ego_Item e = new Ego_Item();
			e.next = h;
			p.priv = e;
			e.eidx = (byte)idx;
			e.name = name;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_e_w(Parser p) {
			int level = p.getint("level");
			int rarity = p.getint("rarity");
			int cost = p.getint("cost");
			Ego_Item e = p.priv as Ego_Item;

			if (e == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			e.level = (byte)level;
			e.rarity = (byte)rarity;
			e.cost = cost;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_e_x(Parser p) {
			int rating = p.getint("rating");
			int xtra = p.getint("xtra");
			Ego_Item e = p.priv as Ego_Item;

			if (e == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			e.rating = (byte)rating;
			e.xtra = (byte)xtra;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_e_a(Parser p) {
			Ego_Item e = p.priv as Ego_Item;
			string tmp = p.getstr("minmax");
			int amin, amax;

			e.alloc_prob = (byte)p.getint("common");

			string[] tokens = tmp.Split(new string[] { " to " }, StringSplitOptions.RemoveEmptyEntries);

			if(tokens.Length != 2)
				return Parser.Error.GENERIC;

			if(!int.TryParse(tokens[0], out amin) || !int.TryParse(tokens[1], out amax))
				return Parser.Error.GENERIC;

			if (amin > 255 || amax > 255 || amin < 0 || amax < 0)
			    return Parser.Error.OUT_OF_BOUNDS;

			e.alloc_min = (byte)amin;
			e.alloc_max = (byte)amax;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_e_t(Parser p) {
			int i;
			int tval;
			int min_sval, max_sval;

			Ego_Item e = p.priv as Ego_Item;
			if (e == null)
			    return Parser.Error.MISSING_RECORD_HEADER;

			tval = TVal.find_idx(p.getsym("tval"));
			if (tval < 0)
			    return Parser.Error.UNRECOGNISED_TVAL;

			min_sval = p.getint("min-sval");
			max_sval = p.getint("max-sval");

			i = e.tval_at++;
			if(i < e.tval.Length) {
				e.tval[i] = (byte)tval;
				e.min_sval[i] = (byte)min_sval;
				e.max_sval[i] = (byte)max_sval;
			}

			if (i == Misc.EGO_TVALS_MAX)
			    return Parser.Error.GENERIC;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_e_c(Parser p) {
			random_value th = p.getrand("th");
			random_value td = p.getrand("td");
			random_value ta = p.getrand("ta");
			Ego_Item e = p.priv as Ego_Item;

			if (e == null)
			    return Parser.Error.MISSING_RECORD_HEADER;

			e.to_h = th;
			e.to_d = td;
			e.to_a = ta;

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_e_m(Parser p) {
			int th = p.getint("th");
			int td = p.getint("td");
			int ta = p.getint("ta");
			Ego_Item e = p.priv as Ego_Item;

			if (e == null)
			    return Parser.Error.MISSING_RECORD_HEADER;

			e.min_to_h = (byte)th;
			e.min_to_d = (byte)td;
			e.min_to_a = (byte)ta;

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_e_f(Parser p) {
			Ego_Item e = p.priv as Ego_Item;
			if (e == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			if (!p.hasval("flags"))
			    return Parser.Error.NONE;

			string s = p.getstr("flags");

			return grab_flag_helper(s, e.flags);

			/*
			t = strtok(s, " |");
			while (t) {
			    if (grab_flag(e.flags, OF_SIZE, k_info_flags,t))
			        break;
			    t = strtok(null, " |");
			}
			mem_free(s);
			return t ? Parser.Error.INVALID_FLAG : Parser.Error.NONE;*/
		}

		//This has a single special use
		static Parser.Error grab_flag_helper2(string s, Ego_Item e){
			string[] t = s.Split(new string[] { " ", "|" }, StringSplitOptions.RemoveEmptyEntries);
			string temp;
			foreach(string token in t) {
			    temp = token;
			    if(grab_flag(e.flags, Object_Flag.list, token) == Parser.Error.NONE ||
					grab_flag(e.pval_flags[e.num_pvals], Object_Flag.list, token) == Parser.Error.NONE) {
						break;
			    }
			}

			e.num_pvals++;
			if (e.num_pvals > Misc.MAX_PVALS)
			    return Parser.Error.TOO_MANY_ENTRIES;

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_e_l(Parser p) {
			Ego_Item e = p.priv as Ego_Item;

			if (e == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			if (!p.hasval("flags"))
			    return Parser.Error.MISSING_FIELD;

			e.pval[e.num_pvals] = p.getrand("pval");
			e.min_pval[e.num_pvals] = (byte)p.getint("min");

			string s = p.getstr("flags");

			return grab_flag_helper2(s, e);
			/*t = strtok(s, " |");

			while (t) {
			    if (grab_flag(e.flags, OF_SIZE, k_info_flags, t) ||
			        grab_flag(e.pval_flags[e.num_pvals], OF_SIZE, k_info_flags, t))
			        break;

			    t = strtok(null, " |");
			}

			e.num_pvals++;
			if (e.num_pvals > MAX_PVALS)
			    return Parser.Error.TOO_MANY_ENTRIES;

			mem_free(s);
			return t ? Parser.Error.INVALID_FLAG : Parser.Error.NONE;*/
		}

		public static Parser.Error parse_e_d(Parser p) {
			Ego_Item e = p.priv as Ego_Item;

			if (e == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			e.text = e.text + p.getstr("text");
			return Parser.Error.NONE;
		}



#endregion

#region Artifact Parser
			
		public static Parser init_parse_a() {
			Parser p = new Parser();
			p.priv = null;
			p.Register("V sym version", Parser.Ignored);
			p.Register("N int index str name", parse_a_n);
			p.Register("I sym tval sym sval", parse_a_i);
			p.Register("W int level int rarity int weight int cost", parse_a_w);
			p.Register("A int common str minmax", parse_a_a);
			p.Register("P int ac rand hd int to-h int to-d int to-a", parse_a_p);
			p.Register("F ?str flags", parse_a_f);
			p.Register("E sym name rand time", parse_a_e);
			p.Register("M str text", parse_a_m);
			p.Register("L int pval str flags", parse_a_l);
			p.Register("D str text", parse_a_d);
			return p;
		}

		public static Parser.Error run_parse_a(Parser p) {
			return p.parse_file("artifact");
		}

		public static Parser.Error finish_parse_a(Parser p) {
			Misc.a_info = new Artifact[Misc.z_info.a_max];
			for (Artifact a = p.priv as Artifact; a != null; a = a.Next) {
			    if (a.aidx >= Misc.z_info.a_max)
			        continue;

				Misc.a_info[a.aidx] = a;
			}

			//Old C cleanup stuff
			/*a = p.priv as Artifact;
			while (a) {
			    n = a.next;
			    mem_free(a);
			    a = n;
			}*/

			p.Destroy();
			return 0;
		}

		static void cleanup_a()
		{
			//int idx;
			//for (idx = 0; idx < Misc.z_info.a_max; idx++) {
			//    string_free(a_info[idx].name);
			//    mem_free(a_info[idx].effect_msg);
			//    mem_free(a_info[idx].text);
			//}
			//mem_free(a_info);
		}

		public static File_Parser a_parser = new File_Parser(
			"artifact",
			init_parse_a,
			run_parse_a,
			finish_parse_a,
			cleanup_a
		);


		/* Parsing functions for artifact.txt */
		public static Parser.Error parse_a_n(Parser p) {
			Bitflag f = new Bitflag(Object_Flag.SIZE);
			int idx = p.getint("index");
			string name = p.getstr("name");
			Artifact h = p.priv as Artifact;

			Artifact a = new Artifact();
			a.Next = h;
			p.priv = a;
			a.aidx = (byte)idx;
			a.Name = name;

			/* Ignore all elements */
			Object_Flag.create_mask(f, false, Object_Flag.object_flag_type.IGNORE);
			a.flags.union(f);

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_a_i(Parser p) {
			Artifact a = p.priv as Artifact;
			Misc.assert(a != null);
			
			int tval = TVal.find_idx(p.getsym("tval"));
			if (tval < 0)
			    return Parser.Error.UNRECOGNISED_TVAL;
			a.tval = (byte)tval;

			int sval = SVal.lookup_sval(a.tval, p.getsym("sval"));
			if (sval < 0)
			    return Parser.Error.UNRECOGNISED_SVAL;
			a.sval = (byte)sval;

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_a_w(Parser p) {
			Artifact a = p.priv as Artifact;
			Misc.assert(a != null);

			a.level = (byte)p.getint("level");
			a.rarity = (byte)p.getint("rarity");
			a.weight = (byte)p.getint("weight");
			a.cost = p.getint("cost");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_a_a(Parser p) {
			Artifact a = p.priv as Artifact;
			string tmp = p.getstr("minmax");
			int amin, amax;
			Misc.assert(a != null);

			a.alloc_prob = (byte)p.getint("common");

			string[] tokens = tmp.Split(new string[] { " to " }, StringSplitOptions.RemoveEmptyEntries);
			if(tokens.Length != 2)
				return Parser.Error.GENERIC;
			if(!int.TryParse(tokens[0], out amin) || !int.TryParse(tokens[1], out amax))
				return Parser.Error.GENERIC;


			if (amin > 255 || amax > 255 || amin < 0 || amax < 0)
			    return Parser.Error.OUT_OF_BOUNDS;

			a.alloc_min = (byte)amin;
			a.alloc_max = (byte)amax;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_a_p(Parser p) {
			Artifact a = p.priv as Artifact;
			random_value hd = p.getrand("hd");
			Misc.assert(a != null);

			a.ac = (short)p.getint("ac");
			a.dd = (byte)hd.dice;
			a.ds = (byte)hd.sides;
			a.to_h = (short)p.getint("to-h");
			a.to_d = (short)p.getint("to-d");
			a.to_a = (short)p.getint("to-a");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_a_f(Parser p) {
			Artifact a = p.priv as Artifact;
			Misc.assert(a != null);

			if (!p.hasval("flags"))
			    return Parser.Error.NONE;
			string s = p.getstr("flags");

			return grab_flag_helper(s, a.flags);

			/*t = strtok(s, " |");
			while (t) {
			    if (grab_flag(a.flags, OF_SIZE, k_info_flags, t))
			        break;
			    t = strtok(null, " |");
			}
			mem_free(s);
			return t ? Parser.Error.INVALID_FLAG : Parser.Error.NONE;*/
		}

		public static Parser.Error parse_a_e(Parser p) {
			Artifact a = p.priv as Artifact;
			Misc.assert(a != null);

			a.effect = grab_one_effect(p.getsym("name"));
			a.time = p.getrand("time");
			if (a.effect == null)
			    return Parser.Error.GENERIC;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_a_m(Parser p) {
			Artifact a = p.priv as Artifact;
			Misc.assert(a != null);

			a.effect_msg = a.effect_msg + p.getstr("text");
			return Parser.Error.NONE;
		}

		//This has a single special use
		static Parser.Error grab_flag_helper3(string s, Artifact e){
			string[] t = s.Split(new string[] { " ","|" }, StringSplitOptions.RemoveEmptyEntries);
			string temp;
			foreach(string token in t) {
			    temp = token;
			    if(grab_flag(e.flags, Object_Flag.list, token) == Parser.Error.NONE ||
					grab_flag(e.pval_flags[e.num_pvals], Object_Flag.list, token) == Parser.Error.NONE) {
						break;
			    }
			}

			e.num_pvals++;
			if (e.num_pvals > Misc.MAX_PVALS)
			    return Parser.Error.TOO_MANY_ENTRIES;

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_a_l(Parser p) {
			Artifact a = p.priv as Artifact;
			string s; 
			Misc.assert(a != null);

			a.pval[a.num_pvals] = (short)p.getint("pval");

			if (!p.hasval("flags"))
			    return Parser.Error.MISSING_FIELD;

			s = p.getstr("flags");
			return grab_flag_helper3(s, a);
			//t = strtok(s, " |");

			//while (t) {
			//    if (grab_flag(a.flags, OF_SIZE, k_info_flags, t) ||
			//        grab_flag(a.pval_flags[a.num_pvals], OF_SIZE, k_info_flags, t))
			//        break;

			//    t = strtok(null, " |");
			//}

			//a.num_pvals++;
			//if (a.num_pvals > MAX_PVALS)
			//    return Parser.Error.TOO_MANY_ENTRIES;

			//mem_free(s);
			//return t ? Parser.Error.INVALID_FLAG : Parser.Error.NONE;
		}

		public static Parser.Error parse_a_d(Parser p) {
			Artifact a = p.priv as Artifact;
			Misc.assert(a != null);

			a.Text = a.Text + p.getstr("text");
			return Parser.Error.NONE;
		}

#endregion

#region Monster Pain Parser

		public static Parser init_parse_mp() {
			Parser p = new Parser();
			p.priv = null;

			p.Register("N uint index", parse_mp_n);
			p.Register("M str message", parse_mp_m);
			return p;
		}

		public static Parser.Error run_parse_mp(Parser p) {
			return p.parse_file("pain");
		}

		public static Parser.Error finish_parse_mp(Parser p) {
			Misc.pain_messages = new Monster_Pain[Misc.z_info.mp_max];
			for (Monster_Pain mp = p.priv as Monster_Pain; mp != null; mp = mp.Next) {
			    if (mp.pain_idx >= Misc.z_info.mp_max)
			        continue;
				Misc.pain_messages[mp.pain_idx] = mp;
			}
	
			//C Cleanup
			/*mp = p.priv;
			while (mp) {
			    n = mp.next;
			    mem_free(mp);
			    mp = n;
			}*/
	
			p.Destroy();
			return 0;
		}

		static void cleanup_mp()
		{
			//int idx, i;
			//for (idx = 0; idx < Misc.z_info.mp_max; idx++) {
			//    for (i = 0; i < 7; i++) {
			//        string_free((string )pain_messages[idx].messages[i]);
			//    }
			//}
			//mem_free(pain_messages);
		}

		public static File_Parser mp_parser = new File_Parser(
			"pain messages",
			init_parse_mp,
			run_parse_mp,
			finish_parse_mp,
			cleanup_mp
		);


		/* Initialise monster pain messages */
		public static Parser.Error parse_mp_n(Parser p) {
			Monster_Pain h = p.priv as Monster_Pain;
			Monster_Pain mp = new Monster_Pain();
			mp.Next = h;
			mp.pain_idx = (int)p.getuint("index");
			p.priv = mp;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_mp_m(Parser p) {
			Monster_Pain mp = p.priv as Monster_Pain;
			int i;

			if (mp == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			for (i = 0; i < 7; i++)
			    if (mp.Messages[i] == null)
			        break;
			if (i == 7)
			    return Parser.Error.TOO_MANY_ENTRIES;
			mp.Messages[i] = p.getstr("message");
			return Parser.Error.NONE;
		}



#endregion

#region Monster Base Parser

		public static Parser init_parse_rb() {
			Parser p = new Parser();
			p.priv = null;

			p.Register("V sym version", Parser.Ignored);
			p.Register("N str name", parse_rb_n);
			p.Register("G char glyph", parse_rb_g);
			p.Register("M uint pain", parse_rb_m);
			p.Register("F ?str flags", parse_rb_f);
			p.Register("S ?str spells", parse_rb_s);
			p.Register("D str desc", parse_rb_d);
			return p;
		}

		public static Parser.Error run_parse_rb(Parser p) {
			return p.parse_file("monster_base");
		}

		public static Parser.Error finish_parse_rb(Parser p) {
			Misc.rb_info = p.priv as Monster_Base;
			p.Destroy();
			return 0;
		}

		static void cleanup_rb()
		{
			//Monster_Base rb, *next;

			//rb = rb_info;
			//while (rb) {
			//    next = rb.next;
			//    string_free(rb.text);
			//    string_free(rb.name);
			//    mem_free(rb);
			//    rb = next;
			//}
		}

		public static File_Parser rb_parser = new File_Parser(
			"monster_base",
			init_parse_rb,
			run_parse_rb,
			finish_parse_rb,
			cleanup_rb
		);

		/* Parsing functions for monster_base.txt */

		public static Parser.Error parse_rb_n(Parser p) {
			Monster_Base h = p.priv as Monster_Base;
			Monster_Base rb =new Monster_Base();
			rb.Next = h;
			rb.Name = p.getstr("name");
			p.priv = rb;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_rb_g(Parser p) {
			Monster_Base rb = p.priv as Monster_Base;

			if (rb == null)
			    return Parser.Error.MISSING_RECORD_HEADER;

			rb.d_char = p.getchar("glyph");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_rb_m(Parser p) {
			Monster_Base rb = p.priv as Monster_Base;
			int pain_idx;

			if (rb == null)
			    return Parser.Error.MISSING_RECORD_HEADER;

			pain_idx = (int)p.getuint("pain");
			if (pain_idx >= Misc.z_info.mp_max)
			    /* XXX need a real error code for this */
			    return Parser.Error.GENERIC;

			rb.pain = Misc.pain_messages[pain_idx];

			return Parser.Error.NONE;
		}

		//string r_info_flags[] =
		//{
		//    #define RF(a, b) #a,
		//    #include "monster/list-mon-flags.h"
		//    #undef RF
		//    null
		//};

		static int lookup_flag_mf(List<Monster_Flag> flag_table, string flag_name) {
			int i = 0;

			while ( i < flag_table.Count && flag_table[i].desc != flag_name)
				i++;

			/* End of table reached without match */
			if (i == flag_table.Count) i = -1; //Error!

			return i;
		}

		static Parser.Error grab_flag_mf(Bitflag flags, List<Monster_Flag> flag_table, string flag_name) {
			int flag = lookup_flag_mf(flag_table, flag_name);

			if (flag == -1) return Parser.Error.INVALID_FLAG;

			flags.on(flag);

			return Parser.Error.NONE;
		}

		static Parser.Error grab_flag_helper_mf(string s, Bitflag flags){
			string[] t = s.Split(new string[] { " ","|" }, StringSplitOptions.RemoveEmptyEntries);
			string temp;
			foreach(string token in t) {
			    temp = token;
			    if(grab_flag_mf(flags, Monster_Flag.list, token) != Parser.Error.NONE) {
					return Parser.Error.INVALID_FLAG;
			    }
			}

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_rb_f(Parser p) {
			Monster_Base rb = p.priv as Monster_Base;

			if (rb == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			if (!p.hasval("flags"))
			    return Parser.Error.NONE;
			string flags = p.getstr("flags");

			return grab_flag_helper_mf(flags, rb.flags);

			//s = strtok(flags, " |");
			//while (s) {
			//    if (grab_flag(rb.flags, RF_SIZE, r_info_flags, s)) {
			//        mem_free(flags);
			//        return Parser.Error.INVALID_FLAG;
			//    }
			//    s = strtok(null, " |");
			//}

			//mem_free(flags);
			//return Parser.Error.NONE;
		}

		//string r_info_spell_flags[] =
		//{
		//    #define RSF(a, b, c, d, e, f, g, h, i, j, k, l, m) #a,
		//    #include "monster/list-mon-spells.h"
		//    #undef RSF
		//    null
		//};

		static int lookup_flag_msf(List<Monster_Spell_Flag> flag_table, string flag_name) {
			int i = 0;

			while ( i < flag_table.Count && flag_table[i].name != flag_name)
				i++;

			/* End of table reached without match */
			if (i == flag_table.Count) i = -1; //Error!

			return i;
		}

		static Parser.Error grab_flag_msf(Bitflag flags, List<Monster_Spell_Flag> flag_table, string flag_name) {
			int flag = lookup_flag_msf(flag_table, flag_name);

			if (flag == -1) return Parser.Error.INVALID_FLAG;

			flags.on(flag);

			return Parser.Error.NONE;
		}

		static Parser.Error grab_flag_helper_msf(string s, Bitflag flags){
			string[] t = s.Split(new string[] { " ","|" }, StringSplitOptions.RemoveEmptyEntries);
			string temp;
			foreach(string token in t) {
			    temp = token;
			    if(grab_flag_msf(flags, Monster_Spell_Flag.list, token) != Parser.Error.NONE) {
					return Parser.Error.INVALID_FLAG;
			    }
			}

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_rb_s(Parser p) {
			Monster_Base rb = p.priv as Monster_Base;

			if (rb == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			if (!p.hasval("spells"))
			    return Parser.Error.NONE;
			string flags = p.getstr("spells");

			return grab_flag_helper_msf(flags, rb.spell_flags);

			//s = strtok(flags, " |");
			//while (s) {
			//    if (grab_flag(rb.spell_flags, RSF_SIZE, r_info_spell_flags, s)) {
			//        mem_free(flags);
			//        return Parser.Error.INVALID_FLAG;
			//    }
			//    s = strtok(null, " |");
			//}

			//mem_free(flags);
			//return Parser.Error.NONE;
		}


		public static Parser.Error parse_rb_d(Parser p) {
			Monster_Base rb = p.priv as Monster_Base;

			if (rb == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			rb.Text = rb.Text + p.getstr("desc");
			return Parser.Error.NONE;
		}


#endregion

#region Monster Parser

		public static Parser init_parse_r() {
			Parser p = new Parser();
			p.priv = null;

			p.Register("V sym version", Parser.Ignored);
			p.Register("N uint index str name", parse_r_n);
			p.Register("T sym base", parse_r_t);
			p.Register("G char glyph", parse_r_g);
			p.Register("C sym color", parse_r_c);
			p.Register("I int speed int hp int aaf int ac int sleep", parse_r_i);
			p.Register("W int level int rarity int power int mexp", parse_r_w);
			p.Register("B sym method ?sym effect ?rand damage", parse_r_b);
			p.Register("F ?str flags", parse_r_f);
			p.Register("-F ?str flags", parse_r_mf);
			p.Register("D str desc", parse_r_d);
			p.Register("S str spells", parse_r_s);
			p.Register("drop sym tval sym sval uint chance uint min uint max", parse_r_drop);
			p.Register("drop-artifact str name", parse_r_drop_artifact);
			p.Register("mimic sym tval sym sval", parse_r_mimic);
			return p;
		}

		public static Parser.Error run_parse_r(Parser p) {
			return p.parse_file("monster");
		}

		public static Parser.Error finish_parse_r(Parser p) {
			Misc.r_info = new Monster_Race[Misc.z_info.r_max];
			for (Monster_Race r = p.priv as Monster_Race; r != null; r = r.Next) {
			    if (r.ridx >= Misc.z_info.r_max)
			        continue;
				Misc.r_info[r.ridx] = r;
			}
			Eval.r_power(Misc.r_info);

			p.Destroy();
			return 0;
		}

		static void cleanup_r()
		{
			//int ridx;

			//for (ridx = 0; ridx < Misc.z_info.r_max; ridx++) {
			//    Monster_Race r = &r_info[ridx];
			//    struct monster_drop *d, *dn;
			//    struct monster_mimic *m, *mn;

			//    d = r.drops;
			//    while (d) {
			//        dn = d.next;
			//        mem_free(d);
			//        d = dn;
			//    }
			//    m = r.mimic_kinds;
			//    while (m) {
			//        mn = m.next;
			//        mem_free(m);
			//        m = mn;
			//    }
			//    string_free(r.text);
			//    string_free(r.name);
			//}

			//mem_free(r_info);
		}

		public static File_Parser r_parser = new File_Parser(
			"monster",
			init_parse_r,
			run_parse_r,
			finish_parse_r,
			cleanup_r
		);


		/* Parsing functions for monster.txt */
		public static Parser.Error parse_r_n(Parser p) {
			Monster_Race h = p.priv as Monster_Race;
			Monster_Race r = new Monster_Race();
			r.Next = h;
			r.ridx = p.getuint("index");
			r.Name = p.getstr("name");
			p.priv = r;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_r_t(Parser p) {
			Monster_Race r = p.priv as Monster_Race;

			r.Base = Monster.Monster.lookup_monster_base(p.getsym("base"));
			if (r.Base == null)
			    /* Todo: make new error for this */
			    return Parser.Error.UNRECOGNISED_TVAL;

			/* The template sets the default display character */
			r.d_char = r.Base.d_char;

			/* Give the monster its default flags */
			r.flags.union(r.Base.flags);

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_r_g(Parser p) {
			Monster_Race r = p.priv as Monster_Race;

			/* If the display character is specified, it overrides any template */
			r.d_char = p.getchar("glyph");

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_r_c(Parser p) {
			Monster_Race r = p.priv as Monster_Race;
			string color;
			ConsoleColor attr;

			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			
			color = p.getsym("color");
			attr = Utilities.color_text_to_attr(color);
			if (attr < 0)
			    return Parser.Error.INVALID_COLOR;
			r.d_attr = attr;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_r_i(Parser p) {
			Monster_Race r = p.priv as Monster_Race;

			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			r.speed = (byte)p.getint("speed");
			r.avg_hp = (ushort)p.getint("hp");
			r.aaf = (byte)p.getint("aaf");
			r.ac = (short)p.getint("ac");
			r.sleep = (short)p.getint("sleep");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_r_w(Parser p) {
			Monster_Race r = p.priv as Monster_Race;

			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			r.level = (byte)p.getint("level");
			r.rarity = (byte)p.getint("rarity");
			r.power = (long)p.getint("power");
			r.mexp = p.getint("mexp");
			return Parser.Error.NONE;
		}


		static Monster_Blow.RBM find_blow_method(string name) {
			foreach(Monster_Blow.RBM i in Monster_Blow.RBM.list) {
				if(i.name == name) {
					return i;
				}
			}
			return null;
		}

		static Monster_Blow.RBE find_blow_effect(string name) {
			foreach(Monster_Blow.RBE i in Monster_Blow.RBE.list) {
				if(i.name == name) {
					return i;
				}
			}
			return null;
		}

		public static Parser.Error parse_r_b(Parser p) {
			Monster_Race r = p.priv as Monster_Race;
			int i;
			random_value dam;

			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			for (i = 0; i < Monster_Blow.MONSTER_BLOW_MAX; i++)
			    //used to check .method
				if (r.blow[i] == null)
			        break;
			if (i == Monster_Blow.MONSTER_BLOW_MAX)
			    return Parser.Error.TOO_MANY_ENTRIES;

			r.blow[i] = new Monster_Blow();
			r.blow[i].method = find_blow_method(p.getsym("method"));
			if (r.blow[i].method == null)
			    return Parser.Error.UNRECOGNISED_BLOW;
			if (p.hasval("effect")) {
			    r.blow[i].effect = find_blow_effect(p.getsym("effect"));
			    if (r.blow[i].effect == null)
			        return Parser.Error.INVALID_EFFECT;
			}
			if (p.hasval("damage")) {
			    dam = p.getrand("damage");
			    r.blow[i].d_dice = (byte)dam.dice;
			    r.blow[i].d_side = (byte)dam.sides;
			}


			return Parser.Error.NONE;
		}

		public static Parser.Error parse_r_f(Parser p) {
			Monster_Race r = p.priv as Monster_Race;
			string flags;

			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			if (!p.hasval("flags"))
			    return Parser.Error.NONE;

			flags = p.getstr("flags");

			return grab_flag_helper_mf(flags, r.flags);

			/*s = strtok(flags, " |");
			while (s) {
			    if (grab_flag(r.flags, RF_SIZE, r_info_flags, s)) {
			        mem_free(flags);
			        return Parser.Error.INVALID_FLAG;
			    }
			    s = strtok(null, " |");
			}

			mem_free(flags);
			return Parser.Error.NONE;*/
		}

		public static Parser.Error parse_r_mf(Parser p) {
			Monster_Race r = p.priv as Monster_Race;
			string flags;

			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			if (!p.hasval("flags"))
			    return Parser.Error.NONE;
			flags = p.getstr("flags");

			return grab_flag_helper_mf(flags, r.flags);
			/*
			s = strtok(flags, " |");
			while (s) {
			    if (remove_flag(r.flags, RF_SIZE, r_info_flags, s)) {
			        mem_free(flags);
			        return Parser.Error.INVALID_FLAG;
			    }
			    s = strtok(null, " |");
			}

			mem_free(flags);
			return Parser.Error.NONE;*/
		}

		public static Parser.Error parse_r_d(Parser p) {
			Monster_Race r = p.priv as Monster_Race;

			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			r.Text = r.Text + p.getstr("desc");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_r_s(Parser p) {
			Monster_Race r = p.priv as Monster_Race;
			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			
			Parser.Error ret = Parser.Error.NONE;

			int pct = 0;

			string flags = p.getstr("spells");
			string[] tokens = flags.Split(new string[]{" ","|"}, StringSplitOptions.RemoveEmptyEntries);
			int tat = 0;

			while (tat < tokens.Length) {
				string s = tokens[tat++];
				if(s.StartsWith("1_IN_") && int.TryParse(s.Substring(5), out pct)) {
					if (pct < 1 || pct > 100) {
			            ret = Parser.Error.INVALID_SPELL_FREQ;
			            break;
			        }
			        r.freq_spell = (byte)(100 / pct);
			        r.freq_innate = r.freq_spell;
				} else {
			        if (grab_flag_msf(r.spell_flags, Monster_Spell_Flag.list, s) != Parser.Error.NONE) {
			            ret = Parser.Error.INVALID_FLAG;
			            break;
			        }
			    }
			}

			/*string s = strtok(flags, " |");
			while (s) {
			    if (1 == sscanf(s, "1_IN_%d", &pct)) {
			        if (pct < 1 || pct > 100) {
			            ret = Parser.Error.INVALID_SPELL_FREQ;
			            break;
			        }
			        r.freq_spell = 100 / pct;
			        r.freq_innate = r.freq_spell;
			    } else {
			        if (grab_flag(r.spell_flags, RSF_SIZE, r_info_spell_flags, s)) {
			            ret = Parser.Error.INVALID_FLAG;
			            break;
			        }
			    }
			    s = strtok(null, " |");
			}*/

			/* Add the "base monster" flags to the monster */
			if (r.Base != null)
			    r.spell_flags.union(r.Base.spell_flags);

			//mem_free(flags);
			return ret;
		}

		public static Parser.Error parse_r_drop(Parser p) {
			throw new NotImplementedException();
			//Monster_Race r = p.priv;
			//struct monster_drop *d;
			//int tval, sval;

			//if (!r)
			//    return Parser.Error.MISSING_RECORD_HEADER;
			//tval = tval_find_idx(p.getsym("tval"));
			//if (tval < 0)
			//    return Parser.Error.UNRECOGNISED_TVAL;
			//sval = lookup_sval(tval, p.getsym("sval"));
			//if (sval < 0)
			//    return Parser.Error.UNRECOGNISED_SVAL;

			//if (p.getuint("min") > 99 || p.getuint("max") > 99)
			//    return Parser.Error.INVALID_ITEM_NUMBER;

			//d = mem_zalloc(sizeof *d);
			//d.kind = objkind_get(tval, sval);
			//d.percent_chance = p.getuint("chance");
			//d.min = p.getuint("min");
			//d.max = p.getuint("max");
			//d.next = r.drops;
			//r.drops = d;
			//return Parser.Error.NONE;
		}

		public static Parser.Error parse_r_drop_artifact(Parser p) {
			Monster_Race r = p.priv as Monster_Race;

			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			int art = Artifact.lookup_artifact_name(p.getstr("name"));
			if (art < 0)
			    return Parser.Error.GENERIC;
			Artifact a = Misc.a_info[art];

			Monster_Drop d = new Monster_Drop();
			d.artifact = a;
			d.min = 1;
			d.max = 1;
			d.percent_chance = 100;
			d.Next = r.drops;
			r.drops = d;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_r_mimic(Parser p) {
			Monster_Race r = p.priv as Monster_Race;
			int tval, sval;
			Object_Kind kind;

			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			tval = TVal.find_idx(p.getsym("tval"));
			if (tval < 0)
			    return Parser.Error.UNRECOGNISED_TVAL;
			sval = SVal.lookup_sval(tval, p.getsym("sval"));
			if (sval < 0)
			    return Parser.Error.UNRECOGNISED_SVAL;

			kind = Object_Kind.objkind_get(tval, sval);
			if (kind == null)
			    return Parser.Error.GENERIC;
			Monster_Mimic m = new Monster_Mimic();
			m.kind = kind;
			m.Next = r.mimic_kinds;
			r.mimic_kinds = m;
			return Parser.Error.NONE;
		}



#endregion

#region Pit Parser
		public static Parser init_parse_pit() {
			Parser p = new Parser();
			p.priv = null;

			p.Register("N uint index str name", parse_pit_n);
			p.Register("R uint type", parse_pit_r);
			p.Register("A uint rarity uint level", parse_pit_a);
			p.Register("O uint obj_rarity", parse_pit_o);
			p.Register("T sym base", parse_pit_t);
			p.Register("F ?str flags", parse_pit_f);
			p.Register("S ?str spells", parse_pit_s);
			p.Register("s ?str spells", parse_pit_s2);
			return p;
		}

		public static Parser.Error run_parse_pit(Parser p) {
			return p.parse_file("pit");
		}
 
		public static Parser.Error finish_parse_pit(Parser p) {
			Pit_Profile pit;
		
			Misc.pit_info = new Pit_Profile[Misc.z_info.pit_max];
			for (pit = p.priv as Pit_Profile; pit != null; pit = pit.next) {
			    if (pit.pit_idx >= Misc.z_info.pit_max)
			        continue;
				Misc.pit_info[pit.pit_idx] = pit;
			}
	
			//pit = p.priv;
			//while (pit) {
			//    n = pit.next;
			//    mem_free(pit);
			//    pit = n;
			//}
	
			p.Destroy();
			return 0;
		}

		static void cleanup_pits()
		{
			//int idx;
			//for (idx = 0; idx < Misc.z_info.pit_max; idx++) {
			//    string_free((string )pit_info[idx].name);
			//}
			//mem_free(pit_info);
		}

		public static File_Parser pit_parser = new File_Parser(
			"pits",
			init_parse_pit,
			run_parse_pit,
			finish_parse_pit,
			cleanup_pits
		);



		/*
		 * Initialize monster pits
		 */

		public static Parser.Error parse_pit_n(Parser p) {
			Pit_Profile h = p.priv as Pit_Profile;
			Pit_Profile pit = new Pit_Profile();
			pit.next = h;
			pit.pit_idx = (int)p.getuint("index");
			pit.name = p.getstr("name");	
			p.priv = pit;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_pit_r(Parser p) {
			Pit_Profile pit = p.priv as Pit_Profile;

			if (pit == null)
			    return Parser.Error.MISSING_RECORD_HEADER;

			pit.room_type = (int)p.getuint("type");
			return Parser.Error.NONE;
		}
		public static Parser.Error parse_pit_a(Parser p) {
			Pit_Profile pit = p.priv as Pit_Profile;

			if (pit == null)
			    return Parser.Error.MISSING_RECORD_HEADER;

			pit.rarity = (int)p.getuint("rarity");
			pit.ave = (int)p.getuint("level");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_pit_o(Parser p) {
			Pit_Profile pit = p.priv as Pit_Profile;

			if (pit == null)
			    return Parser.Error.MISSING_RECORD_HEADER;

			pit.obj_rarity = (int)p.getuint("obj_rarity");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_pit_t(Parser p) {
			Pit_Profile pit = p.priv as Pit_Profile;
			Monster_Base Base = Monster.Monster.lookup_monster_base(p.getsym("base"));

			if (pit == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			else if (pit.n_bases == Misc.MAX_RVALS)
			    return Parser.Error.TOO_MANY_ENTRIES;
			else if (Base == null)
			    return Parser.Error.UNRECOGNISED_TVAL;
			else {
			    pit.Base[pit.n_bases++] = Base;
			    return Parser.Error.NONE;		
			}
		}

		public static Parser.Error parse_pit_f(Parser p) {
			Pit_Profile pit = p.priv as Pit_Profile;
			string flags;

			if (pit == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			if (!p.hasval("flags"))
			    return Parser.Error.NONE;
			flags = p.getstr("flags");

			return grab_flag_helper_mf(flags, pit.flags);
			/*
			s = strtok(flags, " |");
			while (s) {
			    if (grab_flag(pit.flags, RF_SIZE, r_info_flags, s)) {
			        mem_free(flags);
			        return Parser.Error.INVALID_FLAG;
			    }
			    s = strtok(null, " |");
			}
	
			mem_free(flags);
			return Parser.Error.NONE;*/
		}

		public static Parser.Error parse_pit_s(Parser p) {
			Pit_Profile pit = p.priv as Pit_Profile;
			string flags;

			if (pit == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			if (!p.hasval("spells"))
			    return Parser.Error.NONE;
			flags = p.getstr("spells");

			return grab_flag_helper_msf(flags, pit.spell_flags);
			/*
			s = strtok(flags, " |");
			while (s) {
			    if (grab_flag(pit.spell_flags, RSF_SIZE, r_info_spell_flags, s)) {
			        mem_free(flags);
			        return Parser.Error.INVALID_FLAG;
			    }
			    s = strtok(null, " |");
			}
	
			mem_free(flags);
			return Parser.Error.NONE;*/
		}

		public static Parser.Error parse_pit_s2(Parser p) {
			Pit_Profile pit = p.priv as Pit_Profile;
			string flags;

			if (pit == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			if (!p.hasval("spells"))
			    return Parser.Error.NONE;
			flags = p.getstr("spells");

			return grab_flag_helper_msf(flags, pit.forbidden_spell_flags);
			/*
			s = strtok(flags, " |");
			while (s) {
			    if (grab_flag(pit.forbidden_spell_flags, RSF_SIZE, r_info_spell_flags, s)) {
			        mem_free(flags);
			        return Parser.Error.INVALID_FLAG;
			    }
			    s = strtok(null, " |");
			}
	
			mem_free(flags);
			return Parser.Error.NONE;*/
		}




#endregion

#region Vault Parser
		public static Parser init_parse_v() {
			Parser p = new Parser();
			p.priv = null;
			p.Register("V sym version", Parser.Ignored);
			p.Register("N uint index str name", parse_v_n);
			p.Register("X uint type int rating uint height uint width", parse_v_x);
			p.Register("D str text", parse_v_d);
			return p;
		}

		public static Parser.Error run_parse_v(Parser p) {
			return p.parse_file("vault");
		}

		public static Parser.Error finish_parse_v(Parser p) {
			Misc.vaults = p.priv as Vault;
			p.Destroy();
			return 0;
		}

		static void cleanup_v()
		{
			//Vault v, *next;
			//for (v = vaults; v; v = next) {
			//    next = v.next;
			//    mem_free(v.name);
			//    mem_free(v.text);
			//    mem_free(v);
			//}
		}

		public static File_Parser v_parser = new File_Parser(
			"vault",
			init_parse_v,
			run_parse_v,
			finish_parse_v,
			cleanup_v
		);


		/* Parsing functions for vault.txt */
		public static Parser.Error parse_v_n(Parser p) {
			Vault h = p.priv as Vault;
			Vault v = new Vault();

			v.vidx = p.getuint("index");
			v.name = p.getstr("name");
			v.next = h;
			p.priv = v;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_v_x(Parser p) {
			Vault v = p.priv as Vault;

			if (v == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			v.typ = (byte)p.getuint("type");
			v.rat = (byte)p.getint("rating");
			v.hgt = (byte)p.getuint("height");
			v.wid = (byte)p.getuint("width");

			/* XXX: huh? These checks were in the original code and I have no idea
			 * why. */
			if (v.typ == 6 && (v.wid > 33 || v.hgt > 22))
			    return Parser.Error.VAULT_TOO_BIG;
			if (v.typ == 7 && (v.wid > 66 || v.hgt > 44))
			    return Parser.Error.VAULT_TOO_BIG;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_v_d(Parser p) {
			Vault v = p.priv as Vault;

			if (v == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			v.text = v.text + p.getstr("text");
			return Parser.Error.NONE;
		}



#endregion

#region History Parser


		public static Parser init_parse_h() {
			Parser p = new Parser();
			p.priv = null;
			p.Register("V sym version", Parser.Ignored);
			p.Register("N uint chart int next int roll int bonus", parse_h_n);
			p.Register("D str text", parse_h_d);
			return p;
		}

		public static Parser.Error run_parse_h(Parser p) {
			return p.parse_file("p_hist");
		}

		public static Parser.Error finish_parse_h(Parser p) {
			History_Chart c;
			History_Entry e, prev, next;
			Misc.histories = p.priv as History_Chart;

			/* Go fix up the entry successor pointers. We can't compute them at
			 * load-time since we may not have seen the successor history yet. Also,
			 * we need to put the entries in the right order; the parser actually
			 * stores them backwards, which is not desirable.
			 */
			for (c = Misc.histories; c != null; c = c.next) {
			    e = c.entries;
			    prev = null;
			    while (e != null) {
			        next = e.next;
			        e.next = prev;
			        prev = e;
			        e = next;
			    }
			    c.entries = prev;
			    for (e = c.entries; e != null; e = e.next) {
			        if (e.isucc == 0)
			            continue;
			        e.succ = findchart(Misc.histories, (uint)e.isucc);
			        if (e.succ == null) {
			            return Parser.Error.GENERIC;
			        }
			    }
			}

			p.Destroy();
			return 0;
		}

		static void cleanup_h()
		{
			//History_Chart c, *next_c;
			//History_Entry e, *next_e;

			//c = histories;
			//while (c) {
			//    next_c = c.next;
			//    e = c.entries;
			//    while (e) {
			//        next_e = e.next;
			//        mem_free(e.text);
			//        mem_free(e);
			//        e = next_e;
			//    }
			//    mem_free(c);
			//    c = next_c;
			//}
		}

		public static File_Parser h_parser = new File_Parser(
			"p_hist",
			init_parse_h,
			run_parse_h,
			finish_parse_h,
			cleanup_h
		);

		static History_Chart findchart(History_Chart hs, uint idx) {
			for (; hs != null; hs = hs.next)
				if (hs.idx == idx)
					break;
			return hs;
		}


		/* Parsing functions for p_hist.txt */
		public static Parser.Error parse_h_n(Parser p) {
			History_Chart oc = p.priv as History_Chart;
			History_Chart c;
			History_Entry e = new History_Entry();
			uint idx = p.getuint("chart");
	
			if ((c = findchart(oc, idx)) == null) {
			    c = new History_Chart();
			    c.next = oc;
			    c.idx = idx;
			    p.priv = c;
			}

			e.isucc = p.getint("next");
			e.roll = p.getint("roll");
			e.bonus = p.getint("bonus");

			e.next = c.entries;
			c.entries = e;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_h_d(Parser p) {
			History_Chart h = p.priv as History_Chart;

			if (h == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			Misc.assert(h.entries != null);
			h.entries.text = h.entries.text + p.getstr("text");
			return Parser.Error.NONE;
		}


#endregion

#region Player Race Parser

		public static Parser init_parse_p() {
			Parser p = new Parser();
			p.priv = null;
			p.Register("V sym version", Parser.Ignored);
			p.Register("N uint index str name", parse_p_n);
			p.Register("S int str int int int wis int dex int con int chr", parse_p_s);
			p.Register("R int dis int dev int sav int stl int srh int fos int thm int thb int throw int dig", parse_p_r);
			p.Register("X int mhp int exp int infra", parse_p_x);
			p.Register("I uint hist int b-age int m-age", parse_p_i);
			p.Register("H int mbht int mmht int fbht int fmht", parse_p_h);
			p.Register("W int mbwt int mmwt int fbwt int fmwt", parse_p_w);
			p.Register("F ?str flags", parse_p_f);
			p.Register("Y ?str flags", parse_p_y);
			p.Register("C ?str classes", parse_p_c);
			return p;
		}

		public static Parser.Error run_parse_p(Parser p) {
			return p.parse_file("p_race");
		}

		public static Parser.Error finish_parse_p(Parser p) {
			Misc.races = p.priv as Player_Race;
			p.Destroy();
			return 0;
		}

		static void cleanup_p()
		{
			//Player_Race p = races;
			//Player_Race next;

			//while (p) {
			//    next = p.next;
			//    string_free((string )p.name);
			//    mem_free(p);
			//    p = next;
			//}
		}

		public static File_Parser p_parser = new File_Parser(
			"p_race",
			init_parse_p,
			run_parse_p,
			finish_parse_p,
			cleanup_p
		);



		/* Parsing functions for prace.txt */
		public static Parser.Error parse_p_n(Parser p) {
			Player_Race h = p.priv as Player_Race;
			Player_Race r = new Player_Race();

			r.Next = h;
			r.ridx = p.getuint("index");
			r.Name = p.getstr("name");
			p.priv = r;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_p_s(Parser p) {
			Player_Race r = p.priv as Player_Race;
			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			r.r_adj[(int)Stat.Str] = (short)p.getint("str");
			r.r_adj[(int)Stat.Dex] = (short)p.getint("dex");
			r.r_adj[(int)Stat.Con] = (short)p.getint("con");
			r.r_adj[(int)Stat.Int] = (short)p.getint("int");
			r.r_adj[(int)Stat.Wis] = (short)p.getint("wis");
			r.r_adj[(int)Stat.Chr] = (short)p.getint("chr");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_p_r(Parser p) {
			Player_Race r = p.priv as Player_Race;
			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			r.r_skills[(int)Skill.DISARM] = (short)p.getint("dis");
			r.r_skills[(int)Skill.DEVICE] = (short)p.getint("dev");
			r.r_skills[(int)Skill.SAVE] = (short)p.getint("sav");
			r.r_skills[(int)Skill.STEALTH] = (short)p.getint("stl");
			r.r_skills[(int)Skill.SEARCH] = (short)p.getint("srh");
			r.r_skills[(int)Skill.SEARCH_FREQUENCY] = (short)p.getint("fos");
			r.r_skills[(int)Skill.TO_HIT_MELEE] = (short)p.getint("thm");
			r.r_skills[(int)Skill.TO_HIT_BOW] = (short)p.getint("thb");
			r.r_skills[(int)Skill.TO_HIT_THROW] = (short)p.getint("throw");
			r.r_skills[(int)Skill.DIGGING] = (short)p.getint("dig");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_p_x(Parser p) {
			Player_Race r = p.priv as Player_Race;
			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			r.r_mhp = (byte)p.getint("mhp");
			r.r_exp = (byte)p.getint("exp");
			r.infra = (byte)p.getint("infra");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_p_i(Parser p) {
			Player_Race r = p.priv as Player_Race;
			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			r.history = findchart(Misc.histories, p.getuint("hist"));
			r.b_age = (byte)p.getint("b-age");
			r.m_age = (byte)p.getint("m-age");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_p_h(Parser p) {
			Player_Race r = p.priv as Player_Race;
			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			r.m_b_ht = (byte)p.getint("mbht");
			r.m_m_ht = (byte)p.getint("mmht");
			r.f_b_ht = (byte)p.getint("fbht");
			r.f_m_ht = (byte)p.getint("fmht");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_p_w(Parser p) {
			Player_Race r = p.priv as Player_Race;
			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			r.m_b_wt = (byte)p.getint("mbwt");
			r.m_m_wt = (byte)p.getint("mmwt");
			r.f_b_wt = (byte)p.getint("fbwt");
			r.f_m_wt = (byte)p.getint("fmwt");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_p_f(Parser p) {
			Player_Race r = p.priv as Player_Race;
			string flags;

			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			if (!p.hasval("flags"))
			    return Parser.Error.NONE;
			flags = p.getstr("flags");

			return grab_flag_helper(flags, r.flags);

			//s = strtok(flags, " |");
			//while (s) {
			//    if (grab_flag(r.flags, OF_SIZE, k_info_flags, s))
			//        break;
			//    s = strtok(null, " |");
			//}
			//mem_free(flags);
			//return s ? Parser.Error.INVALID_FLAG : Parser.Error.NONE;
		}

		static int lookup_flag5(List<Misc.PF> flag_table, string flag_name) {
			int i = 0;

			while ( i < flag_table.Count && flag_table[i].name != flag_name)
				i++;

			/* End of table reached without match */
			if (i == flag_table.Count) i = -1; //Error!

			return i;
		}

		static Parser.Error grab_flag5(Bitflag flags, List<Misc.PF> flag_table, string flag_name) {
			int flag = lookup_flag5(flag_table, flag_name);

			if (flag == -1) return Parser.Error.INVALID_FLAG;

			flags.on(flag);

			return Parser.Error.NONE;
		}

		static Parser.Error grab_flag_helper5(string s, Bitflag flags){
			string[] t = s.Split(new string[] { " ", "|" }, StringSplitOptions.RemoveEmptyEntries);
			string temp;
			foreach(string token in t) {
			    temp = token;
			    if(grab_flag5(flags, Misc.PF.list, token) != Parser.Error.NONE) {
					return Parser.Error.INVALID_FLAG;
			    }
			}

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_p_y(Parser p) {
			Player_Race r = p.priv as Player_Race;
			string flags;

			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			if (!p.hasval("flags"))
			    return Parser.Error.NONE;
			flags = p.getstr("flags");

			return grab_flag_helper5(flags, r.pflags);

			//s = strtok(flags, " |");
			//while (s) {
			//    if (grab_flag(r.pflags, PF_SIZE, player_info_flags, s))
			//        break;
			//    s = strtok(null, " |");
			//}
			//mem_free(flags);
			//return s ? Parser.Error.INVALID_FLAG : Parser.Error.NONE;
		}

		public static Parser.Error parse_p_c(Parser p) {
			Player_Race r = p.priv as Player_Race;
			string classes;

			if (r == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			if (!p.hasval("classes"))
			    return Parser.Error.NONE;
			classes = p.getstr("classes");

			string[] tokens = classes.Split(new string[]{" ", "|"}, StringSplitOptions.RemoveEmptyEntries);

			foreach(string token in tokens) {
				r.choice |= (byte)(1 << int.Parse(token));
			}

			return Parser.Error.NONE;
		}


#endregion

#region Player Class Parser

		public static Parser init_parse_c() {
			Parser p = new Parser();
			p.priv = null;
			p.Register("V sym version", Parser.Ignored);
			p.Register("N uint index str name", parse_c_n);
			p.Register("S int str int int int wis int dex int con int chr", parse_c_s);
			p.Register("C int dis int dev int sav int stl int srh int fos int thm int thb int throw int dig", parse_c_c);
			p.Register("X int dis int dev int sav int stl int srh int fos int thm int thb int throw int dig", parse_c_x);
			p.Register("I int mhp int exp int sense-base int sense-div", parse_c_i);
			p.Register("A int max-attacks int min-weight int att-multiply", parse_c_a);
			p.Register("M uint book uint stat uint first uint weight", parse_c_m);
			p.Register("B uint spell int level int mana int fail int exp", parse_c_b);
			p.Register("T str title", parse_c_t);
			p.Register("E sym tval sym sval uint min uint max", parse_c_e);
			p.Register("F ?str flags", parse_c_f);
			return p;
		}

		public static Parser.Error run_parse_c(Parser p) {
			return p.parse_file("p_class");
		}

		public static Parser.Error finish_parse_c(Parser p) {
			Misc.classes = p.priv as Player_Class;
			p.Destroy();
			return 0;
		}

		static void cleanup_c()
		{
			//Player_Class c = classes;
			//Player_Class next;
			//Start_Item item, *item_next;
			//int i;

			//while (c) {
			//    next = c.next;
			//    item = c.start_items;
			//    while(item) {
			//        item_next = item.next;
			//        mem_free(item);
			//        item = item_next;
			//    }
			//    for (i = 0; i < PY_MAX_LEVEL / 5; i++) {
			//        string_free((string )c.title[i]);
			//    }
			//    mem_free((string )c.name);
			//    mem_free(c);
			//    c = next;
			//}
		}

		public static File_Parser c_parser = new File_Parser(
			"p_class",
			init_parse_c,
			run_parse_c,
			finish_parse_c,
			cleanup_c
		);


		/* Parsing functions for pclass.txt */
		public static Parser.Error parse_c_n(Parser p) {
			Player_Class h = p.priv as Player_Class;
			Player_Class c = new Player_Class();
			c.cidx = p.getuint("index");
			c.Name = p.getstr("name");
			c.next = h;
			p.priv = c;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_c_s(Parser p){
			Player_Class c = p.priv as Player_Class;

			if (c == null)
			    return Parser.Error.MISSING_RECORD_HEADER;

			c.c_adj[(int)Stat.Str] = (short)p.getint("str");
			c.c_adj[(int)Stat.Int] = (short)p.getint("int");
			c.c_adj[(int)Stat.Wis] = (short)p.getint("wis");
			c.c_adj[(int)Stat.Dex] = (short)p.getint("dex");
			c.c_adj[(int)Stat.Con] = (short)p.getint("con");
			c.c_adj[(int)Stat.Chr] = (short)p.getint("chr");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_c_c(Parser p) {
			Player_Class c = p.priv as Player_Class;

			if (c == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			c.c_skills[(int)Skill.DISARM] = (short)p.getint("dis");
			c.c_skills[(int)Skill.DEVICE] = (short)p.getint("dev");
			c.c_skills[(int)Skill.SAVE] = (short)p.getint("sav");
			c.c_skills[(int)Skill.STEALTH] = (short)p.getint("stl");
			c.c_skills[(int)Skill.SEARCH] = (short)p.getint("srh");
			c.c_skills[(int)Skill.SEARCH_FREQUENCY] = (short)p.getint("fos");
			c.c_skills[(int)Skill.TO_HIT_MELEE] = (short)p.getint("thm");
			c.c_skills[(int)Skill.TO_HIT_BOW] = (short)p.getint("thb");
			c.c_skills[(int)Skill.TO_HIT_THROW] = (short)p.getint("throw");
			c.c_skills[(int)Skill.DIGGING] = (short)p.getint("dig");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_c_x(Parser p) {
			Player_Class c = p.priv as Player_Class;

			if (c == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			c.x_skills[(int)Skill.DISARM] = (short)p.getint("dis");
			c.x_skills[(int)Skill.DEVICE] = (short)p.getint("dev");
			c.x_skills[(int)Skill.SAVE] = (short)p.getint("sav");
			c.x_skills[(int)Skill.STEALTH] = (short)p.getint("stl");
			c.x_skills[(int)Skill.SEARCH] = (short)p.getint("srh");
			c.x_skills[(int)Skill.SEARCH_FREQUENCY] = (short)p.getint("fos");
			c.x_skills[(int)Skill.TO_HIT_MELEE] = (short)p.getint("thm");
			c.x_skills[(int)Skill.TO_HIT_BOW] = (short)p.getint("thb");
			c.x_skills[(int)Skill.TO_HIT_THROW] = (short)p.getint("throw");
			c.x_skills[(int)Skill.DIGGING] = (short)p.getint("dig");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_c_i(Parser p) {
			Player_Class c = p.priv as Player_Class;

			if (c == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			c.c_mhp = (short)p.getint("mhp");
			c.c_exp = (short)p.getint("exp");
			c.sense_base = (uint)p.getint("sense-base");
			c.sense_div = (ushort)p.getint("sense-div");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_c_a(Parser p) {
			Player_Class c = p.priv as Player_Class;

			if (c == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			c.max_attacks = (ushort)p.getint("max-attacks");
			c.min_weight = (ushort)p.getint("min-weight");
			c.att_multiply = (ushort)p.getint("att-multiply");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_c_m(Parser p) {
			Player_Class c = p.priv as Player_Class;

			if (c == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			c.spell_book = (byte)p.getuint("book");
			c.spell_stat = (ushort)p.getuint("stat");
			c.spell_first = (ushort)p.getuint("first");
			c.spell_weight = (ushort)p.getuint("weight");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_c_b(Parser p) {
			Player_Class c = p.priv as Player_Class;
			uint spell;

			if (c == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			spell = p.getuint("spell");
			if (spell >= Misc.PY_MAX_SPELLS)
			    return Parser.Error.OUT_OF_BOUNDS;
			c.spells.info[spell] = new Magic_Type();
			c.spells.info[spell].slevel = (byte)p.getint("level");
			c.spells.info[spell].smana = (byte)p.getint("mana");
			c.spells.info[spell].sfail = (byte)p.getint("fail");
			c.spells.info[spell].sexp = (byte)p.getint("exp");
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_c_t(Parser p) {
			Player_Class c = p.priv as Player_Class;
			int i;

			if (c == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			for (i = 0; i < Misc.PY_MAX_LEVEL / 5; i++) {
			    if (c.title[i] == null) {
			        c.title[i] = p.getstr("title");
			        break;
			    }
			}

			if (i >= Misc.PY_MAX_LEVEL / 5)
			    return Parser.Error.TOO_MANY_ENTRIES;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_c_e(Parser p) {
			Player_Class c = p.priv as Player_Class;
			Start_Item si;
			int tval, sval;

			if (c == null)
			    return Parser.Error.MISSING_RECORD_HEADER;

			tval = TVal.find_idx(p.getsym("tval"));
			if (tval < 0)
			    return Parser.Error.UNRECOGNISED_TVAL;

			sval = SVal.lookup_sval(tval, p.getsym("sval"));
			if (sval < 0)
			    return Parser.Error.UNRECOGNISED_SVAL;

			si = new Start_Item();
			si.kind = Object_Kind.objkind_get(tval, sval);
			si.min = (byte)p.getuint("min");
			si.max = (byte)p.getuint("max");

			if (si.min > 99 || si.max > 99) {
			    //mem_free(si.kind);
			    return Parser.Error.INVALID_ITEM_NUMBER;
			}

			si.next = c.start_items;
			c.start_items = si;

			return Parser.Error.NONE;
		}

		public static Parser.Error parse_c_f(Parser p) {
			Player_Class c = p.priv as Player_Class;
			string flags;

			if (c == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			if (!p.hasval("flags"))
			    return Parser.Error.NONE;
			flags = p.getstr("flags");

			return grab_flag_helper5(flags, c.pflags);

			/*s = strtok(flags, " |");
			while (s) {
			    if (grab_flag(c.pflags, PF_SIZE, player_info_flags, s))
			        break;
			    s = strtok(null, " |");
			}

			mem_free(flags);
			return s ? Parser.Error.INVALID_FLAG : Parser.Error.NONE;*/
		}



#endregion

#region Flavor Parser
		public static Parser init_parse_flavor() {
			Parser p = new Parser();
			p.priv = null;
			p.Register("V sym version", Parser.Ignored);
			p.Register("N uint index sym tval ?sym sval", parse_flavor_n);
			p.Register("G char glyph sym attr", parse_flavor_g);
			p.Register("D str desc", parse_flavor_d);
			return p;
		}

		public static Parser.Error run_parse_flavor(Parser p) {
			return p.parse_file("flavor");
		}

		public static Parser.Error finish_parse_flavor(Parser p) {
			Misc.flavors = p.priv as Flavor;
			p.Destroy();
			return 0;
		}

		static void cleanup_flavor()
		{
			//Flavor f, *next;

			//f = flavors;
			//while(f) {
			//    next = f.next;
			//    /* Hack - scrolls get randomly-generated names */
			//    if (f.tval != TV_SCROLL)
			//        mem_free(f.text);
			//    mem_free(f);
			//    f = next;
			//}
		}

		public static File_Parser flavor_parser = new File_Parser(
			"flavor",
			init_parse_flavor,
			run_parse_flavor,
			finish_parse_flavor,
			cleanup_flavor
		);



		/* Parsing functions for flavor.txt */
		public static Parser.Error parse_flavor_n(Parser p) {
			Flavor h = p.priv as Flavor;
			Flavor f = new Flavor();

			f.next = h;
			f.fidx = p.getuint("index");
			f.tval = (byte)TVal.find_idx(p.getsym("tval"));
			/* Misc.assert(f.tval); */
			if (p.hasval("sval"))
			    f.sval = (byte)SVal.lookup_sval(f.tval, p.getsym("sval"));
			else
			    f.sval = SVal.SV_UNKNOWN;
			p.priv = f;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_flavor_g(Parser p) {
			Flavor f = p.priv as Flavor;
			ConsoleColor d_attr;
			string attr;

			if (f == null)
			    return Parser.Error.MISSING_RECORD_HEADER;

			f.d_char = p.getchar("glyph");
			attr = p.getsym("attr");
			d_attr = Utilities.color_text_to_attr(attr);

			if (d_attr < 0)
			    return Parser.Error.GENERIC;
			f.d_attr = d_attr;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_flavor_d(Parser p) {
			Flavor f = p.priv as Flavor;

			if (f == null)
			    return Parser.Error.MISSING_RECORD_HEADER;
			f.text = f.text + p.getstr("desc");
			return Parser.Error.NONE;
		}



#endregion

#region Spells Parser
		public static Parser init_parse_s() {
			Parser p = new Parser();
			p.priv = null;
			p.Register("V sym version", Parser.Ignored);
			p.Register("N uint index str name", parse_s_n);
			p.Register("I uint tval uint sval uint snum", parse_s_i);
			p.Register("D str desc", parse_s_d);
			return p;
		}

		public static Parser.Error run_parse_s(Parser p) {
			return p.parse_file("spell");
		}

		public static Parser.Error finish_parse_s(Parser p) {
			Spell s;
			Object_Kind k;

			Misc.s_info = new Spell[Misc.z_info.s_max];
			for (s = p.priv as Spell; s != null; s = s.next) {
			    if (s.sidx >= Misc.z_info.s_max)
			        continue;
			    Misc.s_info[s.sidx] = s;
			    k = Object_Kind.objkind_get(s.tval, s.sval);
			    if (k != null) {
			        Misc.s_info[s.sidx].next = k.spells;
			        k.spells = Misc.s_info[s.sidx];
			    }
			}

			p.Destroy();
			return 0;
		}

		static void cleanup_s()
		{
			//int idx;
			//for (idx = 0; idx < Misc.z_info.s_max; idx++) {
			//    string_free(s_info[idx].name);
			//    mem_free(s_info[idx].text);
			//}
			//mem_free(s_info);
		}

		static File_Parser s_parser = new File_Parser(
			"spell",
			init_parse_s,
			run_parse_s,
			finish_parse_s,
			cleanup_s
		);


		/* Parsing functions for spell.txt */
		public static Parser.Error parse_s_n(Parser p) {
			Spell s = new Spell();
			s.next = p.priv as Spell;
			s.sidx = p.getuint("index");
			s.name = p.getstr("name");
			p.priv = s;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_s_i(Parser p) {
			Spell s = p.priv as Spell;

			if (s == null)
			    return Parser.Error.MISSING_RECORD_HEADER;

			s.tval = (byte)p.getuint("tval");
			s.sval = (byte)p.getuint("sval");
			s.snum = (byte)p.getuint("snum");

			/* XXX elly: postprocess instead? */
			s.realm = (byte)(s.tval - TVal.TV_MAGIC_BOOK);
			s.spell_index = (byte)(s.sidx - (s.realm * Misc.PY_MAX_SPELLS));
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_s_d(Parser p) {
			Spell s = p.priv as Spell;

			if (s == null)
			    return Parser.Error.MISSING_RECORD_HEADER;

			s.text = s.text + p.getstr("desc");
			return Parser.Error.NONE;
		}



#endregion

#region Hints Parser
		public static Parser init_parse_hints() {
			Parser p = new Parser();
			p.Register("H str text", parse_hint);
			return p;
		}

		public static Parser.Error run_parse_hints(Parser p) {
			return p.parse_file("hints");
		}

		public static Parser.Error finish_parse_hints(Parser p) {
			Misc.hints = p.priv as Hint;
			p.Destroy();
			return 0;
		}

		static void cleanup_hints()
		{
			//Hint h, *next;

			//h = hints;
			//while(h) {
			//    next = h.next;
			//    string_free(h.hint);
			//    mem_free(h);
			//    h = next;
			//}
		}

		static File_Parser hints_parser = new File_Parser(
			"hints",
			init_parse_hints,
			run_parse_hints,
			finish_parse_hints,
			cleanup_hints
		);


		/* Initialise hints */
		public static Parser.Error parse_hint(Parser p) {
			Hint h = p.priv as Hint;
			Hint newh = new Hint();

			newh.hint = p.getstr("text");
			newh.next = h;

			p.priv = newh;
			return Parser.Error.NONE;
		}


#endregion

#region Names Parser
		public static Parser init_parse_names() {
			Parser p = new Parser();
			names_parse n = new names_parse();
			n.section = 0;
			p.priv = n;
			p.Register("N int section", parse_names_n);
			p.Register("D str name", parse_names_d);
			return p;
		}

		public static Parser.Error run_parse_names(Parser p) {
			return p.parse_file("names");
		}

		public static Parser.Error finish_parse_names(Parser p) {
			int i;
			uint j;
			names_parse n = p.priv as names_parse;
			name nm;
			RandomName.name_sections = new string[(int)RandomName.randname_type.RANDNAME_NUM_TYPES][];
			for (i = 0; i < (int)RandomName.randname_type.RANDNAME_NUM_TYPES; i++) {
			    RandomName.name_sections[i] = new string[(n.nnames[i] + 1)];
			    for (nm = n.names[i], j = 0; nm != null && j < n.nnames[i]; nm = nm.next, j++) {
			        RandomName.name_sections[i][j] = nm.str;
			    }
			    RandomName.name_sections[i][n.nnames[i]] = null;
				//while(n.names[i]) {
				//    nm = n.names[i].next;
				//    mem_free(n.names[i]);
				//    n.names[i] = nm;
				//}
			}
			//mem_free(n);
			p.Destroy();
			return 0;
		}

		static void cleanup_names()
		{
			//int i, j;
			//for (i = 0; i < RANDNAME_NUM_TYPES; i++) {
			//    for (j = 0; name_sections[i][j]; j++) {
			//        string_free((string )name_sections[i][j]);
			//    }
			//    mem_free(name_sections[i]);
			//}
			//mem_free(name_sections);
		}

		public static File_Parser names_parser = new File_Parser(
			"names",
			init_parse_names,
			run_parse_names,
			finish_parse_names,
			cleanup_names
		);


		/* Parsing functions for names.txt (random name fragments) */
		class name {
		    public name next;
		    public string str;
		};

		class names_parse {
		    public uint section;
		    public uint[] nnames = new uint[(int)RandomName.randname_type.RANDNAME_NUM_TYPES];
		    public name[] names = new name[(int)RandomName.randname_type.RANDNAME_NUM_TYPES];
		};

		public static Parser.Error parse_names_n(Parser p) {
			uint section = (uint)p.getint("section");
			names_parse s = p.priv as names_parse;
			if (s.section >= (int)RandomName.randname_type.RANDNAME_NUM_TYPES)
			    return Parser.Error.GENERIC;
			s.section = section;
			return Parser.Error.NONE;
		}

		public static Parser.Error parse_names_d(Parser p) {
			string name = p.getstr("name");
			names_parse s = p.priv as names_parse;
			name ns = new name();

			if(s.section >= s.nnames.Length) {
				return Parser.Error.OUT_OF_BOUNDS;
			}

			s.nnames[s.section]++;
			ns.next = s.names[s.section];
			ns.str = name;
			s.names[s.section] = ns;
			return Parser.Error.NONE;
		}


#endregion
	}
}
