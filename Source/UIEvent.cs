using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CSAngband {
	/**
	 * The various UI events that can occur.
	 */
	enum ui_event_type
	{
		EVT_NONE	= 0x0000,

		/* Basic events */
		EVT_KBRD	= 0x0001,	/* Keypress */
		EVT_MOUSE	= 0x0002,	/* Mousepress */
		EVT_RESIZE	= 0x0004,	/* Display resize */

		EVT_BUTTON	= 0x0008,	/* Button press */

		/* 'Abstract' events */
		EVT_ESCAPE	= 0x0010,	/* Get out of this menu */
		EVT_MOVE	= 0x0020,	/* Menu movement */
		EVT_SELECT	= 0x0040	/* Menu selection */
	};

	/**
	* Type capable of holding any input key we might want to use.
	*/
	enum keycode_t : byte {
		KC_NONE        = 0x00, /*Default, no key */
		/**
		 * Key modifiers.
		 */
		KC_MOD_CONTROL = 0x01,
		KC_MOD_SHIFT   = 0x02,
		KC_MOD_ALT     = 0x04,
		KC_MOD_META    = 0x08,
		KC_MOD_KEYPAD  = 0x10,
		/**
		 * Keyset mappings for various keys.
		 */
		ESCAPE       = 0x1B,
		KC_RETURN    = 0x0A, /* ASCII \n */
		KC_ENTER     = 0x0D, /* ASCII \r */
		KC_TAB       = 0x09, /* ASCII \t */
		KC_DELETE    = 0x7F,
		KC_BACKSPACE = 0x08, /* ASCII \h */

		ARROW_DOWN   = 0x80,
		ARROW_LEFT   = 0x81,
		ARROW_RIGHT  = 0x82,
		ARROW_UP     = 0x83,
		
		KC_F1        = 0x84,
		KC_F2        = 0x85,
		KC_F3        = 0x86,
		KC_F4        = 0x87,
		KC_F5        = 0x88,
		KC_F6        = 0x89,
		KC_F7        = 0x8A,
		KC_F8        = 0x8B,
		KC_F9        = 0x8C,
		KC_F10       = 0x8D,
		KC_F11       = 0x8E,
		KC_F12       = 0x8F,
		KC_F13       = 0x90,
		KC_F14       = 0x91,
		KC_F15       = 0x92,

		KC_HELP      = 0x93,
		KC_HOME      = 0x94,
		KC_PGUP      = 0x95,
		KC_END       = 0x96,
		KC_PGDOWN    = 0x97,
		KC_INSERT    = 0x98,
		KC_PAUSE     = 0x99,
		KC_BREAK     = 0x9a,
		KC_BEGIN     = 0x9b
		/* we have up until 0x9F before we start edging into displayable Unicode */
		/* then we could move into private use area 1, 0xE000 onwards */
	}

	/**
	* Union type to hold information about any given event.
	*/
	class ui_event {
		public ui_event(){
			type = ui_event_type.EVT_NONE;
			key = null;
			mouse = null;
		}
		public ui_event_type type;
		public keypress key;
		public mouseclick mouse;
	}

	/**
	* Struct holding all relevant info for keypresses.
	*/
	class keypress {
		public keypress(){
			code = keycode_t.KC_NONE;
			mods = 0;
		}
		public keycode_t code;
		public byte mods;
	};

	/**
	* Struct holding all relevant info for mouse clicks.
	*/
	class mouseclick {
		public mouseclick(){
			x = 0;
			y = 0;
			button = 0;
		}
		public byte x;
		public byte y;
		public byte button;
	};

	static class UIEvent {
		/**
		 * The game assumes that in certain cases, the effect of a modifer key will
		 * be encoded in the keycode itself (e.g. 'A' is shift-'a').  In these cases
		 * (specified below), a keypress' 'mods' value should not encode them also.
		 *
		 * If the character has come from the keypad:
		 *   Include all mods
		 * Else if the character is in the range 0x01-0x1F, and the keypress was
		 * from a key that without modifiers would be in the range 0x40-0x5F:
		 *   CONTROL is encoded in the keycode, and should not be in mods
		 * Else if the character is in the range 0x21-0x2F, 0x3A-0x60 or 0x7B-0x7E:
		 *   SHIFT is often used to produce these should not be encoded in mods
		 *
		 * (All ranges are inclusive.)
		 *
		 * You can use these macros for part of the above conditions.
		 */
		public static bool MODS_INCLUDE_CONTROL(keycode_t key){
			byte v = (byte)key;
			return (((v) >= 0x01 && (v) <= 0x1F) ? false : true);
		}

		public static bool MODS_INCLUDE_SHIFT(keycode_t key){
			byte v = (byte)key;
			return ((((v) >= 0x21 && (v) <= 0x2F) || 
					((v) >= 0x3A && (v) <= 0x60) || 
					((v) >= 0x7B && (v) <= 0x7E)) ? false : true);
		}

		/**
		 * If keycode you're trying to apply control to is between 0x40-0x5F
		 * inclusive, then you should take 0x40 from the keycode and leave
		 * KC_MOD_CONTROL unset.  Otherwise, leave the keycode alone and set
		 * KC_MOD_CONTROL in mods.
		 *
		 * This macro returns true in the former case and false in the latter.
		 */
		public static bool ENCODE_KTRL(keycode_t key){
			byte v = (byte)key;
			return (((v) >= 0x40 && (v) <= 0x5F) ? true : false);
		}

		/**
		 * Given a character X, turn it into a control character.
		 */
		public static char KTRL(char X) {
			return (char)(((byte)X) & 0x1F);
		}

		/**
		 * Given a control character X, turn it into its uppercase ASCII equivalent.
		 */
		public static keycode_t UN_KTRL(keycode_t X){
			return (X + 64);
		}

		/* Analogous to isdigit() etc in ctypes */
		public static bool isarrow(keycode_t c){
			return ((c >= keycode_t.ARROW_DOWN) && (c <= keycode_t.ARROW_UP));
		}

		/**
		 * Map keycodes to their textual equivalent.
		 */
		class mapping {
			public mapping(keycode_t key, string str){
				code = key;
				desc = str;
			}
			public keycode_t code;
			public string desc;
		} 
		
		static mapping[] mappings;
		
		static UIEvent(){
			mappings = new mapping[]{
				new mapping(keycode_t.ESCAPE, "Escape"),
				new mapping(keycode_t.KC_RETURN, "Return"),
				new mapping(keycode_t.KC_ENTER, "Enter"),
				new mapping(keycode_t.KC_TAB, "Tab"),
				new mapping(keycode_t.KC_DELETE, "Delete"),
				new mapping(keycode_t.KC_BACKSPACE, "Backspace"),
				new mapping(keycode_t.ARROW_DOWN, "Down"),
				new mapping(keycode_t.ARROW_LEFT, "Left"),
				new mapping(keycode_t.ARROW_RIGHT, "Right"),
				new mapping(keycode_t.ARROW_UP, "Up"),
				new mapping(keycode_t.KC_F1, "F1"),
				new mapping(keycode_t.KC_F2, "F2"),
				new mapping(keycode_t.KC_F3, "F3"),
				new mapping(keycode_t.KC_F4, "F4"),
				new mapping(keycode_t.KC_F5, "F5"),
				new mapping(keycode_t.KC_F6, "F6"),
				new mapping(keycode_t.KC_F7, "F7"),
				new mapping(keycode_t.KC_F8, "F8"),
				new mapping(keycode_t.KC_F9, "F9"),
				new mapping(keycode_t.KC_F10, "F10"),
				new mapping(keycode_t.KC_F11, "F11"),
				new mapping(keycode_t.KC_F12, "F12"),
				new mapping(keycode_t.KC_F13, "F13"),
				new mapping(keycode_t.KC_F14, "F14"),
				new mapping(keycode_t.KC_F15, "F15"),
				new mapping(keycode_t.KC_HELP, "Help"),
				new mapping(keycode_t.KC_HOME, "Home"),
				new mapping(keycode_t.KC_PGUP, "PageUp"),
				new mapping(keycode_t.KC_END, "End"),
				new mapping(keycode_t.KC_PGDOWN, "PageDown"),
				new mapping(keycode_t.KC_INSERT, "Insert"),
				new mapping(keycode_t.KC_PAUSE, "Pause"),
				new mapping(keycode_t.KC_BREAK, "Break"),
				new mapping(keycode_t.KC_BEGIN, "Begin")
			};
		}
		
		/*** Functions ***/

		/** Given a string (and that string's length), return the corresponding keycode */ /*Given a string, try and find it in "mappings".*/
		public static keycode_t keycode_find_code(string str)
		{
			for (int i = 0; i < mappings.Length; i++) {
				if (str.StartsWith(mappings[i].desc)){
					return mappings[i].code;
				}
			}
			return 0;
		}

		/** Given a keycode, return its description */
		public static string keycode_find_desc(keycode_t kc)
		{
			for (int i = 0; i < mappings.Length; i++) {
				if (mappings[i].code == kc){
					return mappings[i].desc;
				}
			}
			return null;
		}

		/*
		 * Convert a hexidecimal-digit into a decimal
		 */
		static int dehex(byte c)
		{
			try {
				return Int32.Parse(((char)c).ToString(), System.Globalization.NumberStyles.HexNumber);
			} 
			catch {
				return 0;
			}
		}

		/** Convert a string of keypresses into their textual representation */
		public static string keypress_to_text(ui_event[] src, bool expand_backslash){
			string buf = "";

			foreach (ui_event evt in src){
				if (evt.type != ui_event_type.EVT_KBRD) break;

				keycode_t i = evt.key.code;
				byte mods = evt.key.mods;
				string desc = keycode_find_desc(i);

				/* un-ktrl control characters if they don't have a description */
				/* this is so that Tab (^I) doesn't get turned into ^I but gets
				 * displayed as [Tab] */
				if ((byte)i < 0x20 && desc == null){
					mods |= (byte)keycode_t.KC_MOD_CONTROL;
					i = UN_KTRL(i);
				}

				if (mods != 0) {
					if (((mods & (byte)keycode_t.KC_MOD_CONTROL) != 0) && (mods & ~(byte)keycode_t.KC_MOD_CONTROL) == 0) {
						buf += "^";
					} else {
						buf += "{";
						if((mods & (byte)keycode_t.KC_MOD_CONTROL) != 0) buf += "^";
						if((mods & (byte)keycode_t.KC_MOD_SHIFT) != 0) buf += "S";
						if((mods & (byte)keycode_t.KC_MOD_ALT) != 0) buf += "A";
						if((mods & (byte)keycode_t.KC_MOD_META) != 0) buf += "M";
						if((mods & (byte)keycode_t.KC_MOD_KEYPAD) != 0) buf += "K";
						buf += "}";
					}
				}

				if (desc != null) {
					buf += "[" + desc + "]";
				} else {
					switch ((char)i) {
						case '\a': buf += "\a"; break;
						case '\\': {
							if (expand_backslash)
								buf += "\\\\";
							else
								buf += "\\";
							break;
						}
						case '^': buf += "\\^"; break;
						case '[': buf += "\\["; break;
						default: {
							if ((char)i < 127)
								buf += (char)i;
							else
								buf += "\\x" + i.ToString("X2");
							break;
						}
					}
				}
			}

			return buf;
		}

		static void STORE(List<ui_event> key, int pos, byte mod, keycode_t code){
			if ((mod & (byte)keycode_t.KC_MOD_CONTROL) != 0 && ENCODE_KTRL(code)) {
				mod = (byte)(mod & (~((byte)keycode_t.KC_MOD_CONTROL)));
				code = (keycode_t)KTRL((char)code);
			}

			//key[pos] = new ui_event();
			key[pos].key = new keypress();
			key[pos].key.mods = mod;
			key[pos].key.code = code;
		}

		/** Covert a textual representation of keypresses into actual keypresses */
		public static ui_event[] keypress_from_text(string str)
		{
			List<ui_event> buf = new List<ui_event>();
			
			int bufat = 0;
			int cur = 0;
			byte mods = 0;

			/* Analyze the "ascii" string */
			while (cur < str.Length)
			{
				buf.Add(new ui_event());
				buf[bufat].type = ui_event_type.EVT_KBRD;
				buf[bufat].key = new keypress();

				if (str[cur] == '\\')
				{
					cur++;
					if (cur == str.Length) break;

					switch (str[cur]) {
						/* Hex-mode */
						case 'x': {
							try {
								int v1 = Int32.Parse(str[++cur].ToString(), System.Globalization.NumberStyles.HexNumber) * 16;
								int v2 = Int32.Parse(str[++cur].ToString(), System.Globalization.NumberStyles.HexNumber);
								STORE(buf, bufat++, mods, (keycode_t)(v1+v2));
							}
							catch {
								STORE(buf, bufat++, mods, (keycode_t)'?');
							}
							break;
						}

						case '\a': STORE(buf, bufat++, mods, (keycode_t)'\a'); break;
						case '\\': STORE(buf, bufat++, mods, (keycode_t)'\\'); break;
						case '^': STORE(buf, bufat++, mods, (keycode_t)'^'); break;
						case '[': STORE(buf, bufat++, mods, (keycode_t)'['); break;
						default: STORE(buf, bufat++, mods, (keycode_t)str[cur]); break;
					}

					mods = 0;

					/* Skip the final char */
					cur++;
				} else if (str[cur] == '[') {
					/* parse non-ascii keycodes */
					int end;
					keycode_t kc;

					if (++cur == str.Length) return buf.ToArray();

					end = str.Substring(cur, str.Length - cur).IndexOf(']');
					if (end == -1) return buf.ToArray();

					kc = keycode_find_code(str);
					if (kc == 0) return buf.ToArray();

					STORE(buf, bufat++, mods, kc);
					mods = 0;
					cur += end + 1;
				} else if (str[cur] == '{') {
					cur++;
					/* Specify modifier for next character */
					if (cur == str.Length || str.Substring(cur, str.Length - cur).IndexOf('}') == -1)
						return buf.ToArray();

					/* analyze modifier chars */
					while (str[cur] != '}') {
						switch (str[cur]) {
							case '^': mods |= (byte)keycode_t.KC_MOD_CONTROL; break;
							case 'S': mods |= (byte)keycode_t.KC_MOD_SHIFT; break;
							case 'A': mods |= (byte)keycode_t.KC_MOD_ALT; break;
							case 'M': mods |= (byte)keycode_t.KC_MOD_META; break;
							case 'K': mods |= (byte)keycode_t.KC_MOD_KEYPAD; break;
							default:
								return buf.ToArray();
						}

						cur++;
					}

					/* skip ending bracket */
					cur++;
				} else if (str[cur] == '^') {
					mods |= (byte)keycode_t.KC_MOD_CONTROL;
					cur++;
				} else {
					/* everything else */
					STORE(buf, bufat++, mods, (keycode_t)str[cur++]);
					mods = 0;
				}
			}

			//buf[bufat] = new ui_event();

			return buf.ToArray();
		}
	}
}
