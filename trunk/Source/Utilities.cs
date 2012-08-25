using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	static class Utilities {
		/**
		 * The name of the program.
		 */
		public static string argv0 = null;

		/*
		 * Mega-Hack -- special "inkey_next" pointer.  XXX XXX XXX
		 *
		 * This special pointer allows a sequence of keys to be "inserted" into
		 * the stream of keys returned by "inkey()".  This key sequence cannot be
		 * bypassed by the Borg.  We use it to implement keymaps.
		 */
		public static List<keypress> inkey_next = new List<keypress>(); //might be an array/list

		class color_chart_info {
			public color_chart_info(char a, string b, string wat, ConsoleColor c) {
				sym = a;
				name = b;
				shortname = wat;
				color = c;
			}

			public char sym;
			public string name;
			public string shortname;
			public ConsoleColor color;
		}

		static color_chart_info[] color_chart = new color_chart_info[] {
			new color_chart_info('d', "Black", "black", ConsoleColor.Black),
			new color_chart_info('w', "White", "white", ConsoleColor.White),
			new color_chart_info('s', "Slate", "slate", ConsoleColor.Gray),
			new color_chart_info('o', "Orange", "orange", ConsoleColor.DarkYellow),
			new color_chart_info('r', "Red", "red", ConsoleColor.Red),
			new color_chart_info('g', "Green", "green", ConsoleColor.DarkGreen),
			new color_chart_info('b', "Blue", "blue", ConsoleColor.Blue),
			new color_chart_info('u', "Umber", "umber", ConsoleColor.DarkYellow),
			new color_chart_info('D', "Light Dark", "lightdark", ConsoleColor.DarkGray),
			new color_chart_info('W', "Light Slate", "lightslate", ConsoleColor.White),
			new color_chart_info('P', "Light Purple", "lightpurple", ConsoleColor.Magenta),
			new color_chart_info('y', "Yellow", "yellow", ConsoleColor.Yellow),
			new color_chart_info('R', "Light Red", "lightred", ConsoleColor.Red),
			new color_chart_info('G', "Light Green", "lightgreen", ConsoleColor.Green),
			new color_chart_info('B', "Light Blue", "lightblue", ConsoleColor.Cyan),
			new color_chart_info('U', "Light Umber", "lightumber", ConsoleColor.Yellow),
			new color_chart_info('p', "Purple", "purple", ConsoleColor.DarkMagenta),
			new color_chart_info('v', "Violet", "violet", ConsoleColor.DarkMagenta),
			new color_chart_info('t', "Teal", "teal", ConsoleColor.DarkCyan),
			new color_chart_info('m', "Mud", "mud", ConsoleColor.DarkYellow),
			new color_chart_info('Y', "Light Yellow", "lightyellow", ConsoleColor.Yellow),
			new color_chart_info('i', "Magenta-Pink", "magentapink", ConsoleColor.Magenta),
			new color_chart_info('T', "Light Teal", "lightteal", ConsoleColor.Cyan),
			new color_chart_info('V', "Light Violet", "lightviolet", ConsoleColor.Magenta),
			new color_chart_info('I', "Light Pink", "lightpink", ConsoleColor.Red),
			new color_chart_info('M', "Mustard", "mustard", ConsoleColor.DarkYellow),
			new color_chart_info('z', "Blue Slate", "blueslate", ConsoleColor.Cyan),
			new color_chart_info('Z', "Deep Light Blue", "deeplightblue", ConsoleColor.Cyan)
		};

		/*
		 * Converts a string to a terminal color byte.
		 */
		public static ConsoleColor color_text_to_attr(string name)
		{
			foreach(color_chart_info i in color_chart){
				if(name.Length == 1) {
					if(name[0] == i.sym) {
						return i.color;
					}
				} else {
					if(name == i.name || name.ToLower() == i.name.ToLower() || name == i.shortname) {
						return i.color;
					}
				}
			}

			int k;
			if(Char.IsDigit(name[0]) && int.TryParse(name, out k)) {
				return color_chart[k].color;
			}

			/* Default to white */
			return ConsoleColor.White;
		}

		public static ConsoleColor num_to_attr(int num) {
			return color_chart[num].color;
		}

		/**** Available Functions ****/

		/**
		 * Case insensitive comparison between two strings
		 */
		public static int my_stricmp(string s1, string s2){
			return s1.ToUpper().CompareTo(s2.ToUpper());
		}

		/**
		 * Case insensitive comparison between two strings, up to n characters long.
		 */
		public static int my_strnicmp(string a, string b, int n){
			return my_stricmp(a.ToUpper().Substring(0, n), b.ToUpper().Substring(0, n));
		}

		/**
		 * Case-insensitive strstr
		 */
		public static string my_stristr(string str, string pattern){
			if (str.Contains(pattern)){
				return pattern;
			}

			return null;
		}

		/**
		 * Copy up to 'bufsize'-1 characters from 'src' to 'buf' and null-terminate
		 * the result.  The 'buf' and 'src' strings may not overlap.
		 *
		 * Returns: strlen(src).  This makes checking for truncation
		 * easy.  Example:
		 *   if (my_strcpy(buf, src, sizeof(buf)) >= sizeof(buf)) ...;
		 *
		 * This function should be equivalent to the strlcpy() function in BSD.
		 */
		public static int my_strcpy(ref string buf, string src, int bufsize){
			buf = src.Substring(0, bufsize);
			return buf.Length;
		}

		/**
		 * Try to append a string to an existing null-terminated string, never writing
		 * more characters into the buffer than indicated by 'bufsize', and
		 * null-terminating the buffer.  The 'buf' and 'src' strings may not overlap.
		 *
		 * my_strcat() returns strlen(buf) + strlen(src).  This makes checking for
		 * truncation easy.  Example:
		 *   if (my_strcat(buf, src, sizeof(buf)) >= sizeof(buf)) ...;
		 *
		 * This function should be equivalent to the strlcat() function in BSD.
		 */
		public static int my_strcat(ref string buf, string src, int bufsize){
			buf = buf + src.Substring(0, bufsize);
			return buf.Length;
		}

		/*
		 * Determine if string "a" is equal to string "b"
		 */
		public static bool streq(string a, string b)
		{
			return a.Equals(b);
		}

		/*
		 * Determine if string "t" is a prefix of string "s"
		 */
		public static bool prefix(string s, string t){
			return s.StartsWith(t);
		}

		/*
		 * Determine if string "t" is a prefix of string "s" - case insensitive.
		 */
		public static bool prefix_i(string s, string t){
			return s.ToUpper().StartsWith(t.ToUpper());
		}

		/*
		 * Determine if string "t" is a suffix of string "s"
		 */
		public static bool suffix(string s, string t){
			return s.EndsWith(t);
		}

		public delegate void str_func(string s);

		//Redefinable "plog" action
		public static str_func plog_aux = null;

		/* Print an error message */
		public static void plog(string str){
			if (plog_aux != null){
				plog_aux(str);
			} else {
				Console.Error.WriteLine(((argv0 != null) ? argv0 : "?") + ": " + str);
			}
		}

		//Redefinable "quit" action
		public static str_func quit_aux = null;

		/*
		 * Exit (ala "exit()").  If 'str' is null, do "exit(EXIT_SUCCESS)".
		 * Otherwise, plog() 'str' and exit with an error code of -1.
		 * But always use 'quit_aux', if set, before anything else.
		 */
		public static void quit(string str = null){
			/* Attempt to use the aux function */
			if (quit_aux != null){
				quit_aux(str);
			}

			/* Success */
			if (str == null){
				System.Environment.Exit(0);
				//(void)(exit(EXIT_SUCCESS));
			}

			/* Send the string to plog() */
			plog(str);

			/* Failure */
			System.Environment.Exit(1);
			//exit(EXIT_FAILURE);
		}


		/* Sorting functions */
		public static void sort<T>(T[] ToSort, Comparison<T> SortBy)
		{
			Array.Sort(ToSort, SortBy);
		}


		/* Arithmetic mean of the first 'size' entries of the array 'nums' */
		public static int mean(int[] nums, int size)
		{
			int i, total = 0;

			for(i = 0; i < size; i++) total += nums[i];

			return total / size;
		}

		
		/* Variance of the first 'size' entries of the array 'nums'  */
		public static int variance(int[] nums, int size)
		{
			int i, avg, total = 0;

			avg = mean(nums, size);

			for(i = 0; i < size; i++)
			{
				int delta = nums[i] - avg;
				total += delta * delta;
			}

			return total / size;
		}

		/*
		 * Hack -- flush
		 */
		static void msg_flush(int x)
		{
			//throw new NotImplementedException();
			ConsoleColor a = ConsoleColor.Cyan; //Was L_BLUE

			/* Pause for response */
			Term.putstr(x, 0, -1, a, "-more-");

			if (!Option.auto_more.value)
			    anykey();

			/* Clear the line */
			Term.erase(0, 0, 255);
		}


		/*
		 * Output a message to the top line of the screen.
		 *
		 * Break long messages into multiple pieces (40-72 chars).
		 *
		 * Allow multiple short messages to "share" the top line.
		 *
		 * Prompt the user to make sure he has a chance to read them.
		 *
		 * These messages are memorized for later reference (see above).
		 *
		 * We could do a "Term_fresh()" to provide "flicker" if needed.
		 *
		 * The global "msg_flag" variable can be cleared to tell us to "erase" any
		 * "pending" messages still on the screen, instead of using "msg_flush()".
		 * This should only be done when the user is known to have read the message.
		 *
		 * We must be very careful about using the "msg("%s", )" functions without
		 * explicitly calling the special "msg("%s", null)" function, since this may
		 * result in the loss of information if the screen is cleared, or if anything
		 * is displayed on the top line.
		 *
		 * Hack -- Note that "msg("%s", null)" will clear the top line even if no
		 * messages are pending.
		 */
		static void msg_print_aux(Message_Type type, string msg)
		{
			int n;

			if (Term.instance == null)
			    return;

			/* Obtain the size */
			int w, h;
			Term.get_size(out w, out h);

			/* Hack -- Reset */
			if (!Term.msg_flag) Term.message_column = 0;

			/* Message Length */
			n = (msg != null ? msg.Length : 0);

			/* Hack -- flush when requested or needed */
			if ((Term.message_column != 0) && (msg == null || ((Term.message_column + n) > (w - 8))))
			{
			    /* Flush */
			    msg_flush(Term.message_column);

			    /* Forget it */
			    Term.msg_flag = false;

			    /* Reset */
			    Term.message_column = 0;
			}


			/* No message */
			if (msg == null) return;

			/* Paranoia */
			if (n > 1000) return;


			/* Memorize the message (if legal) */
			if (Player.Player.character_generated && !(Player.Player.instance.is_dead))
			    Message.add(msg, type);

			/* Window stuff */
			Player.Player.instance.redraw |= (uint)(Misc.PR_MESSAGE);

			/* Copy it */
			string buf = msg;

			/* Analyze the buffer */
			string t = buf;

			/* Get the color of the message */
			ConsoleColor color = Message.type_color(type);

			/* Split message */
			//Nick I reweote the below commented out bit of code. This is my interpretation of what it does
			//Note, I think it just splits the mesage based on spaces and tries to fit is all
			int charat = 0;
			while (n > w - 1)
			{
			    /* Default split */
			    int split = w - 8;

			    /* Find the rightmost split point */
			    for (int check = (w / 2); check < w - 8; check++)
			        if (t[check] == ' ') split = check;

			    /* Save the split character */
			    //oops = t[split];

			    /* Split the message */
				t = buf.Substring(0, split);

			    /* Display part of the message */
			    Term.putstr(0, 0, split, color, t);

			    /* Flush it */
			    msg_flush(split + 1);

			    /* Prepare to recurse on the rest of "buf" */
			    charat += split; n -= split;
			}
			///* Split message */
			//while (n > w - 1)
			//{
			//    char oops;

			//    /* Default split */
			//    int split = w - 8;

			//    /* Find the rightmost split point */
			//    for (int check = (w / 2); check < w - 8; check++)
			//        if (t[check] == ' ') split = check;

			//    /* Save the split character */
			//    oops = t[split];

			//    /* Split the message */
			//    t[split] = '\0';

			//    /* Display part of the message */
			//    Term.putstr(0, 0, split, color, t);

			//    /* Flush it */
			//    msg_flush(split + 1);

			//    /* Restore the split character */
			//    t[split] = oops;

			//    /* Insert a space */
			//    t[--split] = ' ';

			//    /* Prepare to recurse on the rest of "buf" */
			//    t += split; n -= split;
			//}

			/* Display the tail of the message */
			Term.putstr(Term.message_column, 0, n, color, t);

			/* Remember the message */
			Term.msg_flag = true;

			/* Remember the position */
			Term.message_column += n + 1;

			/* Send refresh event */
			Game_Event.signal(Game_Event.Event_Type.MESSAGE);
		}

		/*
		 * Display a formatted message, using "vstrnfmt()" and "msg("%s", )".
		 */
		public static void msg(string fmt, params object[] vals)
		{
			string buf = String.Format(fmt, vals);

			/* Display */
			msg_print_aux(Message_Type.MSG_GENERIC, buf);
		}

		public static void msgt(Message_Type type, string fmt, params object[] vals)
		{
			string buf; //[1024];
			buf = String.Format(fmt, vals);
			//sound(type); //Sound is disabled for now... TODO: Enabe sound.
			msg_print_aux(type, buf);
		}

		/*
		 * Get a keypress or mouse click from the user.
		 */
		public static void anykey()
		{
			ui_event ke = new ui_event();
  
			/* Only accept a keypress or mouse click */
			while (ke.type != ui_event_type.EVT_MOUSE && ke.type != ui_event_type.EVT_KBRD)
				ke = inkey_ex();
		}

		public static bool inkey_xtra;		/* See the "inkey()" function */
		public static uint inkey_scan;		/* See the "inkey()" function */
		public static bool inkey_flag;		/* See the "inkey()" function */
		public static short signal_count;		/* Hack -- Count interrupts */
		/*
		 * Get a keypress from the user.
		 *
		 * This function recognizes a few "global parameters".  These are variables
		 * which, if set to true before calling this function, will have an effect
		 * on this function, and which are always reset to false by this function
		 * before this function returns.  Thus they function just like normal
		 * parameters, except that most calls to this function can ignore them.
		 *
		 * If "inkey_xtra" is true, then all pending keypresses will be flushed.
		 * This is set by flush(), which doesn't actually flush anything itself
		 * but uses that flag to trigger delayed flushing.
		 *
		 * If "inkey_scan" is true, then we will immediately return "zero" if no
		 * keypress is available, instead of waiting for a keypress.
		 *
		 * If "inkey_flag" is true, then we are waiting for a command in the main
		 * map interface, and we shouldn't show a cursor.
		 *
		 * If we are waiting for a keypress, and no keypress is ready, then we will
		 * refresh (once) the window which was active when this function was called.
		 *
		 * Note that "back-quote" is automatically converted into "escape" for
		 * convenience on machines with no "escape" key.
		 *
		 * If "angband_term[0]" is not active, we will make it active during this
		 * function, so that the various "main-xxx.c" files can assume that input
		 * is only requested (via "Term_inkey()") when "angband_term[0]" is active.
		 *
		 * Mega-Hack -- This function is used as the entry point for clearing the
		 * "signal_count" variable, and of the "character_saved" variable.
		 *
		 * Mega-Hack -- Note the use of "inkey_hack" to allow the "Borg" to steal
		 * control of the keyboard from the user.
		 */
		public static ui_event inkey_ex()
		{
			ui_event kk;
			ui_event ke = new ui_event();

			bool done = false;

			Term old = Term.instance;

			/* Delayed flush */
			if (inkey_xtra) {
				Term.flush();
				inkey_next = null;
				inkey_xtra = false;
			}

			/* Hack -- Use the "inkey_next" pointer */
			if (inkey_next != null && inkey_next.Count > 0 && inkey_next[0] != null && inkey_next[0].code != keycode_t.KC_NONE)
			{
				/* Get next character, and advance */
				ke.key = inkey_next.First();
				inkey_next.RemoveAt(0);

				ke.type = ui_event_type.EVT_KBRD;

				/* Cancel the various "global parameters" */
				inkey_flag = false;
				inkey_scan = 0;

				/* Accept result */
				return (ke);
			}

			/* Forget pointer */
			inkey_next = null;

			//#ifdef ALLOW_BORG

			//    /* Mega-Hack -- Use the special hook */
			//    if (inkey_hack && ((ke.key = (*inkey_hack)(inkey_xtra)) != 0))
			//    {
			//        /* Cancel the various "global parameters" */
			//        inkey_flag = false;
			//        inkey_scan = 0;
			//        ke.type = EVT_KBRD;

			//        /* Accept result */
			//        return (ke);
			//    }

			//#endif /* ALLOW_BORG */


			/* Get the cursor state */
			bool cursor_state = Term.get_cursor();

			/* Show the cursor if waiting, except sometimes in "command" mode */
			if (inkey_scan == 0 && (!inkey_flag || (Misc.character_icky != 0)))
				Term.set_cursor(true);


			/* Hack -- Activate main screen */
			Misc.term_screen.activate();


			/* Get a key */
			while (ke.type == ui_event_type.EVT_NONE)
			{
				/* Hack -- Handle "inkey_scan == SCAN_INSTANT */
				if (inkey_scan == Misc.SCAN_INSTANT &&
						(0 != Term.inkey(out kk, false, false)))
					break;


				/* Hack -- Flush output once when no key ready */
				if (!done && (0 != Term.inkey(out kk, false, false)))
				{
					/* Hack -- activate proper term */
					old.activate();

					/* Flush output */
					Term.fresh();

					/* Hack -- activate main screen */
					Misc.term_screen.activate();

					/* Mega-Hack -- reset saved flag */
					Player.Player.character_saved = false;

					/* Mega-Hack -- reset signal counter */
					signal_count = 0;

					/* Only once */
					done = true;
				}


				/* Get a key (see above) */
				ke = inkey_aux((int)inkey_scan);

				/* Handle mouse buttons */
				if ((ke.type == ui_event_type.EVT_MOUSE) && (Option.mouse_buttons.value))
				{
					/* Check to see if we've hit a button */
					/* Assuming text buttons here for now - this would have to
					 * change for GUI buttons */
					char key = Button.button_get_key(ke.mouse.x, ke.mouse.y);

					if (key != 0)
					{
						/* Rewrite the event */
						/* XXXmacro button implementation needs updating */
						ke.type = ui_event_type.EVT_BUTTON;
						ke.key.code = (keycode_t)key;
						ke.key.mods = 0;

						/* Done */
						break;
					}
				}

				/* Treat back-quote as escape */
				if ((char)ke.key.code == '`')
					ke.key.code = keycode_t.ESCAPE;
			}


			/* Hack -- restore the term */
			old.activate();


			/* Restore the cursor */
			Term.set_cursor(cursor_state);


			/* Cancel the various "global parameters" */
			inkey_flag = false;
			inkey_scan = 0;

			/* Return the keypress */
			return (ke);
		}

		/*
		 * Helper function called only from "inkey()"
		 */
		static ui_event inkey_aux(int scan_cutoff)
		{
			int w = 0;	

			ui_event ke;
	
			/* Wait for a keypress */
			if (scan_cutoff == Misc.SCAN_OFF)
			{
			    Term.inkey(out ke, true, true);
			}
			else
			{
			    w = 0;

			    /* Wait only as long as macro activation would wait*/
			    while (Term.inkey(out ke, false, true) != 0)
			    {
			        /* Increase "wait" */
			        w++;

			        /* Excessive delay */
			        if (w >= scan_cutoff)
			        {
			            ui_event empty = new ui_event();
			            return empty;
			        }

			        /* Delay */
			        Term.xtra(TERM_XTRA.DELAY, 10);
			    }
			}

			return (ke);
		}

		/*
		 * Display a string on the screen using an attribute.
		 *
		 * At the given location, using the given attribute, if allowed,
		 * add the given string.  Do not clear the line.
		 */
		public static void c_put_str(ConsoleColor attr, string str, int row, int col)
		{
			/* Position cursor, Dump the attr/text */
			Term.putstr(col, row, -1, attr, str);
		}


		/*
		 * As above, but in "white"
		 */
		public static void put_str(string str, int row, int col)
		{
			/* Spawn */
			Term.putstr(col, row, -1, ConsoleColor.White, str);
		}



		/*
		 * Display a string on the screen using an attribute, and clear
		 * to the end of the line.
		 */
		public static void c_prt(ConsoleColor attr, string str, int row, int col)
		{
			/* Clear line, position cursor */
			Term.erase(col, row, 255);

			/* Dump the attr/text */
			Term.addstr(-1, attr, str);
		}


		/*
		 * As above, but in "white"
		 */
		public static void prt(string str, int row, int col)
		{
			/* Spawn */
			c_prt(ConsoleColor.White, str, row, col);
		}

		/*
		 * Pause for user response
		 *
		 * This function is stupid.  XXX XXX XXX
		 */
		public static void pause_line(Term term)
		{
			prt("", term.hgt - 1, 0);
			put_str("[Press any key to continue]", term.hgt - 1, 23);
			anykey();
			prt("", term.hgt - 1, 0);
		}

		//Couldn't find actual, so I made the below two funcs
		public delegate void atexit_func();
		public static atexit_func on_exit_funcs;
		public static void atexit(atexit_func ef){
			on_exit_funcs += ef;
		}

		public static void onexit() {
			on_exit_funcs();
		}

		/*
		 * Output text to the screen or to a file depending on the
		 * selected hook.  Takes strings with "embedded formatting",
		 * such that something within {red}{/} will be printed in red.
		 *
		 * Note that such formatting will be treated as a "breakpoint"
		 * for the printing, so if used within words may lead to part of the
		 * word being moved to the next line.
		 */
		public static void text_out_e(string fmt, params object[] args)
		{

			//Formatted String
			//string output = fmt;//String.Format(fmt, args);
			if(args.Length > 0) {
				fmt = String.Format(fmt, args);
			}

			string prt = "";
			ConsoleColor current_color = ConsoleColor.White;
			int arg_len = 0;
			int arg_at = 0;

			for(string parse = fmt; parse.Length > 0; parse = parse.Substring(1)) {
				char next = parse[0];

				if(next != '{') {
					prt += next;
				} else {
					if(prt.Length > 0) {
						//Flush what we have...
						object[] new_args = new object[arg_len];
						for(int i = arg_at; i < arg_at + arg_len; i++) {
							new_args[i - arg_at] = args[i];
						}
						arg_at += arg_len;
						arg_len = 0;
						prt = string.Format(prt, new_args);
						Misc.text_out_hook(current_color, prt);
						prt = "";
					}

					parse = parse.Substring(1);

					if(parse[0] == '/') {
						current_color = ConsoleColor.White;
						parse = parse.Substring(1);
					} else if (char.IsDigit(parse[0])) {
						//That means a C# style argument
						prt += "{" + parse[0];
						arg_len += 1;
					} else {
						int endbrace = parse.IndexOf('}');
						string color = parse.Substring(0, endbrace);
						parse = parse.Substring(endbrace); //Skip over to the end brace.

						current_color = color_text_to_attr(color);
					}
				}
			}
			if(prt.Length > 0) {
				//Flush what we have...
				object[] new_args = new object[arg_len];
				for(int i = arg_at; i < arg_at + arg_len; i++) {
					new_args[i - arg_at] = args[i];
				}
				arg_at += arg_len;
				arg_len = 0;
				prt = string.Format(prt, new_args);
				Misc.text_out_hook(current_color, prt);
				prt = "";
			}
			//throw new NotImplementedException();

			//TODO make it look at the embedded formatting!!!

			//char buf[1024];
			//char smallbuf[1024];
			//va_list vp;

			//const char *start, *next, *text, *tag;
			//size_t textlen, taglen = 0;

			///* Begin the Varargs Stuff */
			//va_start(vp, fmt);

			///* Do the va_arg fmt to the buffer */
			//(void)vstrnfmt(buf, sizeof(buf), fmt, vp);

			///* End the Varargs Stuff */
			//va_end(vp);

			//start = buf;
			//while (next_section(start, 0, &text, &textlen, &tag, &taglen, &next))
			//{
			//    int a = -1;

			//    memcpy(smallbuf, text, textlen);
			//    smallbuf[textlen] = 0;

			//    if (tag)
			//    {
			//        char tagbuffer[11];

			//        /* Colour names are less than 11 characters long. */
			//        assert(taglen < 11);

			//        memcpy(tagbuffer, tag, taglen);
			//        tagbuffer[taglen] = '\0';

			//        a = color_text_to_attr(tagbuffer);
			//    }
		
			//    if (a == -1) 
			//        a = TERM_WHITE;

			//    /* Output now */
			//    text_out_hook(a, smallbuf);

			//    start = next;
			//}
		}

		/*
		 * Output text to the screen or to a file depending on the selected
		 * text_out hook.
		 */
		public static void text_out(string fmt, params object[] args)
		{
			string s = String.Format(fmt, args);
			Misc.text_out_hook(ConsoleColor.White, s);
		}


		/*
		 * Output text to the screen (in color) or to a file depending on the
		 * selected hook.
		 */
		public static void text_out_c(ConsoleColor a, string fmt, params object[] args)
		{
			throw new NotImplementedException();
			//char buf[1024];
			//va_list vp;

			///* Begin the Varargs Stuff */
			//va_start(vp, fmt);

			///* Do the va_arg fmt to the buffer */
			//(void)vstrnfmt(buf, sizeof(buf), fmt, vp);

			///* End the Varargs Stuff */
			//va_end(vp);

			///* Output now */
			//text_out_hook(a, buf);
		}

		/*
		 * Print some (colored) text to the screen at the current cursor position,
		 * automatically "wrapping" existing text (at spaces) when necessary to
		 * avoid placing any text into the last column, and clearing every line
		 * before placing any text in that line.  Also, allow "newline" to force
		 * a "wrap" to the next line.  Advance the cursor as needed so sequential
		 * calls to this function will work correctly.
		 *
		 * Once this function has been called, the cursor should not be moved
		 * until all the related "text_out()" calls to the window are complete.
		 *
		 * This function will correctly handle any width up to the maximum legal
		 * value of 256, though it works best for a standard 80 character width.
		 */
		public static void text_out_to_screen(ConsoleColor a, string str)
		{
			/*int x, y;

			int wid, h;

			int wrap;

			const char *s;
			char buf[1024];*/

			/* We use either ascii or system-specific encoding */
			//int encoding = (Option.xchars_to_file.value) ? SYSTEM_SPECIFIC : ASCII; //we should be good on encoding

			/* Obtain the size */
			int wid, h;
			Term.get_size(out wid, out h);

			/* Obtain the cursor */
			int x, y;
			Term.locate(out x, out y);

			/* Copy to a rewriteable string */
			//my_strcpy(buf, str, 1024);
	
			/* Translate it to 7-bit ASCII or system-specific format */
			//xstr_trans(buf, encoding);
	
			/* Use special wrapping boundary? */
			int wrap;
			if ((Misc.text_out_wrap > 0) && (Misc.text_out_wrap < wid))
			    wrap = Misc.text_out_wrap;
			else
			    wrap = wid;

			/* Process the string */
			for (string s = str; s.Length > 0; s = s.Substring(1))
			{
			    //char ch = '\0';

			    /* Force wrap */
			    if (s[0] == '\n')
			    {
			        /* Wrap */
			        x = Misc.text_out_indent;
			        y++;

			        /* Clear line, move cursor */
			        Term.erase(x, y, 255);

			        x += Misc.text_out_pad;
			        Term.gotoxy(x, y);

			        continue;
			    }

			    /* Clean up the char */
			    //ch = (my_isprint((unsigned char)*s) ? *s : ' ');

			    /* Wrap words as needed */
			    if ((x >= wrap - 1) && (s[0] != ' '))
			    {
			        int i, n = 0;

			        ConsoleColor[] av = new ConsoleColor[256];
			        char[] cv = new char[256];

			        /* Wrap word */
			        if (x < wrap)
			        {
			            /* Scan existing text */
			            for (i = wrap - 2; i >= 0; i--)
			            {
			                /* Grab existing attr/char */
			                Term.what(i, y, ref av[i], ref cv[i]);

			                /* Break on space */
			                if (cv[i] == ' ') break;

			                /* Track current word */
			                n = i;
			            }
			        }

			        /* Special case */
			        if (n == 0) n = wrap;

			        /* Clear line */
			        Term.erase(n, y, 255);

			        /* Wrap */
			        x = Misc.text_out_indent;
			        y++;

			        /* Clear line, move cursor */
			        Term.erase(x, y, 255);

			        x += Misc.text_out_pad;
			        Term.gotoxy(x, y);

			        /* Wrap the word (if any) */
			        for (i = n; i < wrap - 1; i++)
			        {
			            /* Dump */
			            Term.addch(av[i], cv[i]);

			            /* Advance (no wrap) */
			            if (++x > wrap) x = wrap;
			        }
			    }

			    /* Dump */
			    Term.addch(a, s[0]);

			    /* Advance */
			    if (++x > wrap) x = wrap;
			}
		}



		/*
		 * Find the start of a possible Roman numerals suffix by going back from the
		 * end of the string to a space, then checking that all the remaining chars
		 * are valid Roman numerals.
		 * 
		 * Return the start position, or null if there isn't a valid suffix. 
		 */
		public static string find_roman_suffix_start(string buf)
		{
			if(buf == null)
				return null;
			int start = buf.LastIndexOf(' ');
			string p;
	
			if (start != 0)
			{
			    start++;
			    p = buf.Substring(start);
			    while (p.Length > 0)
			    {
			        if (p[0] != 'I' && p[0] != 'V' && p[0] != 'X' && p[0] != 'L' &&
			            p[0] != 'C' && p[0] != 'D' && p[0] != 'M')
			        {
			            start = 0;
			            break;
			        }
			        p.Substring(1);			    
			    }
			}
			if(start == 0) {
				return null;
			}
			return buf.Substring(start);
		}

		/*----- Roman numeral functions  ------*/

		/*
		 * Converts an arabic numeral (int) to a roman numeral (char *).
		 *
		 * An arabic numeral is accepted in parameter `n`, and the corresponding
		 * upper-case roman numeral is placed in the parameter `roman`.  The
		 * length of the buffer must be passed in the `bufsize` parameter.  When
		 * there is insufficient room in the buffer, or a roman numeral does not
		 * exist (e.g. non-positive integers) a value of 0 is returned and the
		 * `roman` buffer will be the empty string.  On success, a value of 1 is
		 * returned and the zero-terminated roman numeral is placed in the
		 * parameter `roman`.
		 */
		public static int int_to_roman(int n, string roman)
		{
			throw new NotImplementedException();
			///* Roman symbols */
			//char roman_symbol_labels[13][3] =
			//    {"M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX",
			//     "V", "IV", "I"};
			//int  roman_symbol_values[13] =
			//    {1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1};

			///* Clear the roman numeral buffer */
			//roman[0] = '\0';

			///* Roman numerals have no zero or negative numbers */
			//if (n < 1)
			//    return 0;

			///* Build the roman numeral in the buffer */
			//while (n > 0)
			//{
			//    int i = 0;

			//    /* Find the largest possible roman symbol */
			//    while (n < roman_symbol_values[i])
			//        i++;

			//    /* No room in buffer, so abort */
			//    if (strlen(roman) + strlen(roman_symbol_labels[i]) + 1
			//        > bufsize)
			//        break;

			//    /* Add the roman symbol to the buffer */
			//    my_strcat(roman, roman_symbol_labels[i], bufsize);

			//    /* Decrease the value of the arabic numeral */
			//    n -= roman_symbol_values[i];
			//}

			///* Ran out of space and aborted */
			//if (n > 0)
			//{
			//    /* Clean up and return */
			//    roman[0] = '\0';

			//    return 0;
			//}

			//return 1;
		}


		/*
		 * Converts a roman numeral (char *) to an arabic numeral (int).
		 *
		 * The null-terminated roman numeral is accepted in the `roman`
		 * parameter and the corresponding integer arabic numeral is returned.
		 * Only upper-case values are considered. When the `roman` parameter
		 * is empty or does not resemble a roman numeral, a value of -1 is
		 * returned.
		 *
		 * XXX This function will parse certain non-sense strings as roman
		 *     numerals, such as IVXCCCVIII
		 */
		public static int roman_to_int(string roman)
		{
			throw new NotImplementedException();
			//size_t i;
			//int n = 0;
			//char *p;

			//char roman_token_chr1[] = "MDCLXVI";
			//const char *roman_token_chr2[] = {0, 0, "DM", 0, "LC", 0, "VX"};

			//int roman_token_vals[7][3] = {{1000},
			//                              {500},
			//                              {100, 400, 900},
			//                              {50},
			//                              {10, 40, 90},
			//                              {5},
			//                              {1, 4, 9}};

			//if (strlen(roman) == 0)
			//    return -1;

			///* Check each character for a roman token, and look ahead to the
			//   character after this one to check for subtraction */
			//for (i = 0; i < strlen(roman); i++)
			//{
			//    char c1, c2;
			//    int c1i, c2i;

			//    /* Get the first and second chars of the next roman token */
			//    c1 = roman[i];
			//    c2 = roman[i + 1];

			//    /* Find the index for the first character */
			//    p = strchr(roman_token_chr1, c1);
			//    if (p)
			//    {
			//        c1i = p - roman_token_chr1;
			//    } else {
			//        return -1;
			//    }

			//    /* Find the index for the second character */
			//    c2i = 0;
			//    if (roman_token_chr2[c1i] && c2)
			//    {
			//        p = strchr(roman_token_chr2[c1i], c2);
			//        if (p)
			//        {
			//            c2i = (p - roman_token_chr2[c1i]) + 1;
			//            /* Two-digit token, so skip a char on the next pass */
			//            i++;
			//        }
			//    }

			//    /* Increase the arabic numeral */
			//    n += roman_token_vals[c1i][c2i];
			//}

			//return n;
		}

		/*
		 * Get a "keypress" from the user.
		 */
		public static keypress inkey()
		{
			ui_event ke = new ui_event();

			/* Only accept a keypress */
			while (ke.type != ui_event_type.EVT_ESCAPE && ke.type != ui_event_type.EVT_KBRD)
				ke = inkey_ex();

			/* Paranoia */
			if (ke.type == ui_event_type.EVT_ESCAPE) {
				ke.type = ui_event_type.EVT_KBRD;
				ke.key.code = keycode_t.ESCAPE;
				ke.key.mods = 0;
			}

			return ke.key;
		}

		/*
		 * Extract a direction (or zero) from a character
		 */
		public static int target_dir(keypress ch)
		{
			int d = 0;

			/* Already a direction? */
			if (Char.IsDigit((char)ch.code)) {
				d = Basic.D2I((char)ch.code);
			} else if (UIEvent.isarrow(ch.code)) {
				switch (ch.code) {
					case keycode_t.ARROW_DOWN:  d = 2; break;
					case keycode_t.ARROW_LEFT:  d = 4; break;
					case keycode_t.ARROW_RIGHT: d = 6; break;
					case keycode_t.ARROW_UP:    d = 8; break;
				}
			} else {
				int mode;
				keypress[] act;

				if (Option.rogue_like_commands.value)
					mode = (int)Keymap.Mode.ROGUE;
				else
					mode = (int)Keymap.Mode.ORIG;

				/* XXX see if this key has a digit in the keymap we can use */
				act = Keymap.find(mode, ch);
				if (act != null) {
					
					for (int i = 0; i < act.Length; i++) {
						keypress cur = act[i];
						if(cur == null)
							continue;
						//cur.type == ui_event_type.EVT_KBRD && 
						if (Char.IsDigit((char)cur.code)){
							d = Basic.D2I((char)cur.code);
						}
					}
				}
			}

			/* Paranoia */
			if (d == 5) d = 0;

			/* Return direction */
			return (d);
		}

		/*
		 * Clear part of the screen
		 */
		public static void clear_from(int row)
		{
			int y;

			/* Erase requested rows */
			for (y = row; y < Term.instance.hgt; y++)
			{
				/* Erase part of the screen */
				Term.erase(0, y, 255);
			}
		}

		/*
		 * Print the queued messages.
		 */
		public static void message_flush()
		{
			/* Hack -- Reset */
			if (!Term.msg_flag) Term.message_column = 0;

			/* Flush when needed */
			if (Term.message_column != 0)
			{
				/* Print pending messages */
				if (Term.instance != null)
					msg_flush(Term.message_column);

				/* Forget it */
				Term.msg_flag = false;

				/* Reset */
				Term.message_column = 0;
			}
		}

		/*
		 * The default "keypress handling function" for askfor_aux, this takes the
		 * given keypress, input buffer, length, etc, and does the appropriate action
		 * for each keypress, such as moving the cursor left or inserting a character.
		 *
		 * It should return true when editing of the buffer is "complete" (e.g. on
		 * the press of RETURN).
		 */
		static bool askfor_aux_keypress(ref string buf, ref int curs, ref int len, keypress key, bool firsttime)
		{
			switch (key.code)
			{
			    case keycode_t.ESCAPE:
			    {
			        curs = 0;
			        return true;
			    }
		
			    case (keycode_t)'\n':
			    case (keycode_t)'\r':
			    {
			        curs = len;
			        return true;
			    }
		
			    case keycode_t.ARROW_LEFT:
			    {
			        if (firsttime) curs = 0;
			        if (curs > 0) curs--;
			        break;
			    }
		
			    case keycode_t.ARROW_RIGHT:
			    {
			        if (firsttime) curs = len - 1;
			        if (curs < len) curs++;
			        break;
			    }
		
			    case (keycode_t)0x7F:
			    case (keycode_t)'\b':
			    {
			        /* If this is the first time round, backspace means "delete all" */
			        if (firsttime)
			        {
			            buf = "";
			            curs = 0;
			            len = 0;

			            break;
			        }

			        /* Refuse to backspace into oblivion */
			        if (curs == 0) break;

			        /* Move the string from k to nul along to the left by 1 */
					string left = buf.Substring(0, curs - 1);
					string right = "";
					if(curs + 1 < buf.Length) {
						right = buf.Substring(curs + 1, len - curs);
					}
					buf = left + right;
			        //memmove(&buf[*curs - 1], &buf[*curs], *len - *curs);

			        /* Decrement */
			        curs--;
			        len--;

			        /* Terminate */
					buf = buf.Substring(0, len);

			        break;
			    }
		
			    default:
			    {
			        bool atnull = (buf.Length == curs);

			        /*if (!my_isprint((unsigned char)keypress.code))
			        {
			            bell("Illegal edit key!");
			            break;
			        }*/ //Good enough... right?

			        /* Clear the buffer if this is the first time round */
			        if (firsttime)
			        {
						buf = "";
			            curs = 0;
			            len = 0;
			            atnull = true;
			        }

			        if (atnull)
			        {
			            /* Make sure we have enough room for a new character */
			            //if ((curs + 1) >= buflen) break;
						//We're always good!
			        }
			        else
			        {
			            /* Make sure we have enough room to add a new character */
			            //if ((*len + 1) >= buflen) break;

			            /* Move the rest of the buffer along to make room */
			            //memmove(&buf[*curs+1], &buf[*curs], *len - *curs);
			        }

			        /* Insert the character */
					buf = buf + (char)key.code;
					curs++;
					len++;

			        /* Terminate */
			        //buf[*len] = '\0'; //meh

			        break;
			    }
			}

			/* By default, we aren't done. */
			return false;
		}


		/*
		 * Get some input at the cursor location.
		 *
		 * The buffer is assumed to have been initialized to a default string.
		 * Note that this string is often "empty" (see below).
		 *
		 * The default buffer is displayed in yellow until cleared, which happens
		 * on the first keypress, unless that keypress is Return.
		 *
		 * Normal chars clear the default and append the char.
		 * Backspace clears the default or deletes the final char.
		 * Return accepts the current buffer contents and returns true.
		 * Escape clears the buffer and the window and returns false.
		 *
		 * Note that 'len' refers to the size of the buffer.  The maximum length
		 * of the input is 'len-1'.
		 *
		 * 'keypress_h' is a pointer to a function to handle keypresses, altering
		 * the input buffer, cursor position and suchlike as required.  See
		 * 'askfor_aux_keypress' (the default handler if you supply null for
		 * 'keypress_h') for an example.
		 */
		public delegate bool keypress_func(ref string s, ref int i, ref int j, keypress k, bool tf);
		public static bool askfor_aux(ref string buf, int max_len, keypress_func keypress_h)
		{
			int k = 0;		/* Cursor position */
			int nul = 0;		/* Position of the null byte in the string */

			keypress ch = new keypress();

			bool done = false;
			bool firsttime = true;

			if (keypress_h == null)
			{
			    keypress_h = askfor_aux_keypress;
			}

			/* Locate the cursor */
			int x, y;
			Term.locate(out x, out y);


			/* Paranoia */
			if ((x < 0) || (x >= 80)) x = 0;


			/* Restrict the length */
			if (x + buf.Length > 80) buf = buf.Substring(0, 80 - x);

			/* Truncate the default entry */
			//Nick: Just did this above.
			//if(buf.Length > len) {
			//    buf = buf.Substring(0, len);
			//}

			/* Get the position of the null byte */
			nul = buf.Length;

			/* Display the default answer */
			Term.erase(x, y, max_len);
			Term.putstr(x, y, -1, ConsoleColor.Yellow, buf);

			/* Process input */
			while (!done)
			{
			    /* Place cursor */
			    Term.gotoxy(x + k, y);

			    /* Get a key */
			    ch = inkey();

			    /* Let the keypress handler deal with the keypress */
			    done = keypress_h(ref buf, ref k, ref nul, ch, firsttime);

			    /* Update the entry */
			    Term.erase(x, y, max_len);
			    Term.putstr(x, y, -1, ConsoleColor.White, buf);

			    /* Not the first time round anymore */
			    firsttime = false;
			}

			/* Done */
			return (ch.code != keycode_t.ESCAPE);
		}

		/*
		 * A "keypress" handling function for askfor_aux, that handles the special
		 * case of '*' for a new random "name" and passes any other "keypress"
		 * through to the default "editing" handler.
		 */
		static bool get_name_keypress(ref string buf, ref int curs, ref int len, keypress key, bool firsttime)
		{
			bool result;

			switch ((char)key.code)
			{
			    case '*':
			    {
					throw new NotImplementedException();
					//*len = randname_make(RANDNAME_TOLKIEN, 4, 8, buf, buflen, name_sections);
					//buf[0] = toupper((unsigned char) buf[0]);
					//*curs = 0;
					//result = false;
			        //break;
			    }

			    default:
			    {
			        result = askfor_aux_keypress(ref buf, ref curs, ref len, key, firsttime);
			        break;
			    }
			}

			return result;
		}

		/*
		 * Gets a name for the character, reacting to name changes.
		 *
		 * If sf is true, we change the savefile name depending on the character name.
		 *
		 * What a horrible name for a global function.  XXX XXX XXX
		 */
		public static bool get_name(ref string buf, int len)
		{
			bool res;

			/* Paranoia XXX XXX XXX */
			message_flush();

			/* Display prompt */
			prt("Enter a name for your character (* for a random name): ", 0, 0);

			/* Save the player name */
			buf = Player.Player_Other.instance.full_name;

			/* Ask the user for a string */
			res = askfor_aux(ref buf, len, get_name_keypress);

			/* Clear prompt */
			prt("", 0, 0);

			/* Revert to the old name if the player doesn't pick a new one. */
			if (!res)
			{
				buf = Player.Player_Other.instance.full_name;
			}

			return res;
		}

		/*
		 * Flush the screen, make a noise
		 */
		public static void bell(string reason)
		{
			/* Mega-Hack -- Flush the output */
			Term.fresh();

			/* Hack -- memorize the reason if possible */
			if (Player.Player.character_generated && reason != null)
			{
				Message.add(reason, Message_Type.MSG_BELL);

			    /* Window stuff */
			    Misc.p_ptr.redraw |= (Misc.PR_MESSAGE);
			    Misc.p_ptr.redraw_stuff();
			}

			/* Flush the input (later!) */
			flush();
		}
		/*
		 * Flush all pending input.
		 *
		 * Actually, remember the flush, using the "inkey_xtra" flag, and in the
		 * next call to "inkey()", perform the actual flushing, for efficiency,
		 * and correctness of the "inkey()" function.
		 */
		public static void flush()
		{
			/* Do it later */
			inkey_xtra = true;
		}

		/*
		 * Verify something with the user
		 *
		 * The "prompt" should take the form "Query? "
		 *
		 * Note that "[y/n]" is appended to the prompt.
		 */
		public static bool get_check(string prompt)
		{
			keypress ke;

			//char buf[80];
			string buf;

			bool repeat = false;
  
			/* Paranoia XXX XXX XXX */
			message_flush();

			/* Hack -- Build a "useful" prompt */
			buf = prompt + "[y/n]";
			//strnfmt(buf, 78, "%.70s[y/n] ", prompt);

			/* Hack - kill the repeat button */
			if (Button.button_kill('n') != 0) repeat = true;
	
			/* Make some buttons */
			Button.button_add("[y]", 'y');
			Button.button_add("[n]", 'n');
			Misc.p_ptr.redraw_stuff();
  
			/* Prompt for it */
			prt(buf, 0, 0);
			ke = inkey();

			/* Kill the buttons */
			Button.button_kill('y');
			Button.button_kill('n');

			/* Hack - restore the repeat button */
			if (repeat) Button.button_add("[Rpt]", 'n');
			Misc.p_ptr.redraw_stuff();
  
			/* Erase the prompt */
			prt("", 0, 0);

			/* Normal negation */
			if ((ke.code != (keycode_t)'Y') && (ke.code != (keycode_t)'y')) return (false);

			/* Success */
			return (true);
		}

		/*
		 * Save the screen, and increase the "icky" depth.
		 *
		 * This function must match exactly one call to "screen_load()".
		 */
		public static void screen_save()
		{
			/* Hack -- Flush messages */
			message_flush();

			/* Save the screen (if legal) */
			Term.save();

			/* Increase "icky" depth */
			Misc.character_icky++;
		}


		/*
		 * Load the screen, and decrease the "icky" depth.
		 *
		 * This function must match exactly one call to "screen_save()".
		 */
		public static void screen_load()
		{
			/* Hack -- Flush messages */
			message_flush();

			/* Load the screen (if legal) */
			Term.load();

			/* Decrease "icky" depth */
			Misc.character_icky--;

			/* Mega hack -redraw big graphics - sorry NRM */
			if (Misc.character_icky == 0 && (Term.tile_width > 1 || Term.tile_height > 1))
			    Term.redraw();
		}

		/*
		 * A Hengband-like 'window' function, that draws a surround box in ASCII art.
		 */
		public static void window_make(int origin_x, int origin_y, int end_x, int end_y)
		{
			int n;
			Region to_clear = new Region();

			to_clear.col = origin_x;
			to_clear.row = origin_y;
			to_clear.width = end_x - origin_x;
			to_clear.page_rows = end_y - origin_y;

			to_clear.erase();

			Term.putch(origin_x, origin_y, ConsoleColor.White, '+');
			Term.putch(end_x, origin_y, ConsoleColor.White, '+');
			Term.putch(origin_x, end_y, ConsoleColor.White, '+');
			Term.putch(end_x, end_y, ConsoleColor.White, '+');

			for (n = 1; n < (end_x - origin_x); n++)
			{
			    Term.putch(origin_x + n, origin_y, ConsoleColor.White, '-');
			    Term.putch(origin_x + n, end_y, ConsoleColor.White, '-');
			}

			for (n = 1; n < (end_y - origin_y); n++)
			{
			    Term.putch(origin_x, origin_y + n, ConsoleColor.White, '|');
			    Term.putch(end_x, origin_y + n, ConsoleColor.White, '|');
			}
		}

		/*
		 * Request a "quantity" from the user
		 *
		 * Allow "p_ptr.command_arg" to specify a quantity
		 */
		public static short get_quantity(string prompt, int max)
		{
			int amt = 1;


			/* Use "command_arg" */
			if (Misc.p_ptr.command_arg != 0)
			{
			    /* Extract a number */
			    amt = Misc.p_ptr.command_arg;

			    /* Clear "command_arg" */
			    Misc.p_ptr.command_arg = 0;
			}

			/* Prompt if needed */
			else if ((max != 1))
			{
			    //char tmp[80];
				string tmp;
			    //char buf[80];
				string buf;

			    /* Build a prompt if needed */
			    if (prompt == null)
			    {
			        /* Build a prompt */
			        //strnfmt(tmp, sizeof(tmp), "Quantity (0-%d, *=all): ", max);
					tmp = String.Format("Quantity (0-{0}, *=all): ", max);

			        /* Use that prompt */
			        prompt = tmp;
			    }

			    /* Build the default */
				buf = String.Format("{0}", amt);

			    /* Ask for a quantity */
			    if (!get_string(prompt, ref buf, 7)) return (0);

				/* A star or letter means "all" */
			    if ((buf[0] == '*') || Char.IsLetter(buf[0])) 
					amt = max; 
				else 
					/* Extract a number */
					amt = int.Parse(buf);
			}

			/* Enforce the maximum */
			if (amt > max) amt = max;

			/* Enforce the minimum */
			if (amt < 0) amt = 0;

			/* Return the result */
			return (short)(amt);
		}

		/*
		 * Prompt for a string from the user.
		 *
		 * The "prompt" should take the form "Prompt: ".
		 *
		 * See "askfor_aux" for some notes about "buf" and "len", and about
		 * the return value of this function.
		 */
		public static bool get_string(string prompt, ref string buf, int max_length)
		{
			bool res;

			/* Paranoia XXX XXX XXX */
			message_flush();

			/* Display prompt */
			prt(prompt, 0, 0);

			/* Ask the user for a string */
			res = askfor_aux(ref buf, max_length, null);

			/* Translate it to 8-bit (Latin-1) */
			//Nick: uhh.... let's ignore this...
			//xstr_trans(buf, LATIN1);

			/* Clear prompt */
			prt("", 0, 0);

			/* Result */
			return (res);
		}

		public static bool get_com_ex(string prompt, out ui_event command)
		{
			ui_event ke;

			/* Paranoia XXX XXX XXX */
			message_flush();

			/* Display a prompt */
			prt(prompt, 0, 0);

			/* Get a key */
			ke = inkey_ex();

			/* Clear the prompt */
			prt("", 0, 0);

			/* Save the command */
			command = ke;

			/* Done */
			if (ke.type == ui_event_type.EVT_KBRD && ke.key.code == keycode_t.ESCAPE)
				return false;
			return true;
		}

	}
}
