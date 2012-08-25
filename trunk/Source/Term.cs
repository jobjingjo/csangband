using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	///**** Available Constants ****/

	///*
	// * Definitions for the "actions" of "Term_xtra()"
	// *
	// * These values may be used as the first parameter of "Term_xtra()",
	// * with the second parameter depending on the "action" itself.  Many
	// * of the actions shown below are optional on at least one platform.
	// *
	// * The "TERM_XTRA_EVENT" action uses "v" to "wait" for an event
	// * The "TERM_XTRA_SHAPE" action uses "v" to "show" the cursor
	// * The "TERM_XTRA_FROSH" action uses "v" for the index of the row
	// * The "TERM_XTRA_ALIVE" action uses "v" to "activate" (or "close")
	// * The "TERM_XTRA_LEVEL" action uses "v" to "resume" (or "suspend")
	// * The "TERM_XTRA_DELAY" action uses "v" as a "millisecond" value
	// *
	// * The other actions do not need a "v" code, so "zero" is used.
	// */
	public enum TERM_XTRA {
		EVENT = 1,    /* Process some pending events */
		FLUSH = 2,    /* Flush all pending events */
		CLEAR = 3,    /* Clear the entire window */
		SHAPE = 4,    /* Set cursor shape (optional) */
		FROSH = 5,    /* Flush one row (optional) */
		FRESH = 6,    /* Flush all rows (optional) */
		NOISE = 7,    /* Make a noise (optional) */
		BORED = 9,    /* Handle stuff when bored (optional) */
		REACT = 10,    /* React to global changes (optional) */
		ALIVE = 11,    /* Change the "hard" level (optional) */
		LEVEL = 12,    /* Change the "soft" level (optional) */
		DELAY = 13    /* Delay some milliseconds (optional) */
	}


	///*** Color constants ***/


	///*
	// * Angband "attributes" (with symbols, and base (R,G,B) codes)
	// *
	// * The "(R,G,B)" codes are given in "fourths" of the "maximal" value,
	// * and should "gamma corrected" on most (non-Macintosh) machines.
	// */
	//#define TERM_DARK     0  /* d */    /* 0 0 0 */
	//#define TERM_WHITE    1  /* w */    /* 4 4 4 */
	//#define TERM_SLATE    2  /* s */    /* 2 2 2 */
	//#define TERM_ORANGE   3  /* o */    /* 4 2 0 */
	//#define TERM_RED      4  /* r */    /* 3 0 0 */
	//#define TERM_GREEN    5  /* g */    /* 0 2 1 */
	//#define TERM_BLUE     6  /* b */    /* 0 0 4 */
	//#define TERM_UMBER    7  /* u */    /* 2 1 0 */
	//#define TERM_L_DARK   8  /* D */    /* 1 1 1 */
	//#define TERM_L_WHITE  9  /* W */    /* 3 3 3 */
	//#define TERM_L_PURPLE 10 /* P */    /* ? ? ? */
	//#define TERM_YELLOW   11 /* y */    /* 4 4 0 */
	//#define TERM_L_RED    12 /* R */    /* 4 0 0 */
	//#define TERM_L_GREEN  13 /* G */    /* 0 4 0 */
	//#define TERM_L_BLUE   14 /* B */    /* 0 4 4 */
	//#define TERM_L_UMBER  15 /* U */    /* 3 2 1 */

	//#define TERM_PURPLE      16    /* p */
	//#define TERM_VIOLET      17    /* v */
	//#define TERM_TEAL        18    /* t */
	//#define TERM_MUD         19    /* m */
	//#define TERM_L_YELLOW    20    /* Y */
	//#define TERM_MAGENTA     21    /* i */
	//#define TERM_L_TEAL      22    /* T */
	//#define TERM_L_VIOLET    23    /* V */
	//#define TERM_L_PINK      24    /* I */
	//#define TERM_MUSTARD     25    /* M */
	//#define TERM_BLUE_SLATE  26    /* z */
	//#define TERM_DEEP_L_BLUE 27    /* Z */

	///* The following allow color 'translations' to support environments with a limited color depth
	// * as well as translate colours to alternates for e.g. menu highlighting. */

	//#define ATTR_FULL        0    /* full color translation */
	//#define ATTR_MONO        1    /* mono color translation */
	//#define ATTR_VGA         2    /* 16 color translation */
	//#define ATTR_BLIND       3    /* "Blind" color translation */
	//#define ATTR_LIGHT       4    /* "Torchlit" color translation */
	//#define ATTR_DARK        5    /* "Dark" color translation */
	//#define ATTR_HIGH        6    /* "Highlight" color translation */
	//#define ATTR_METAL       7    /* "Metallic" color translation */
	//#define ATTR_MISC        8    /* "Miscellaneous" color translation - see misc_to_attr */

	//#define MAX_ATTR        9

	///*
	// * Maximum number of colours, and number of "basic" Angband colours
	// */ 
	//#define MAX_COLORS        256
	//#define BASIC_COLORS    28

	class Term {
		public static bool msg_flag = false; //TODO: Determine if this is a good default value
		public static int message_column = 0; //TODO: Default 0? Maybe.

		public object user; //was void* // *	- Extra "user" info (used by application)

		public object data; //was void* // *	- Extra "data" info (used by implementation)

		public bool user_flag;	// *	- Flag "user_flag"
		// *	  An extra "user" flag (used by application)

		public bool data_flag;	// *	- Flag "data_flag"
		// *	  An extra "data" flag (used by implementation)

		public bool active_flag;// *	- Flag "active_flag"
		// *	  This "term" is "active"

		// *	- Flag "mapped_flag"
		// *	  This "term" is "mapped"
		public bool mapped_flag;

		// *	- Flag "total_erase"
		// *	  This "term" should be fully erased
		public bool total_erase;

		// *	- Flag "fixed_shape"
		// *	  This "term" is not allowed to resize
		public bool fixed_shape;

		// *	- Flag "icky_corner"
		// *	  This "term" has an "icky" corner grid
		public bool icky_corner;

		// *	- Flag "soft_cursor"
		// *	  This "term" uses a "software" cursor
		public bool soft_cursor;

		// *	- Flag "always_pict"
		// *	  Use the "Term_pict()" routine for all text
		public bool always_pict;

		// *	- Flag "higher_pict"
		// *	  Use the "Term_pict()" routine for special text
		public bool higher_pict;

		// *	- Flag "always_text"
		// *	  Use the "Term_text()" routine for invisible text
		public bool always_text;

		// *	- Flag "unused_flag"
		// *	  Reserved for future use
		public bool unused_flag;

		// *	- Flag "never_bored"
		// *	  Never call the "TERM_XTRA_BORED" action
		public bool never_bored;

		// *	- Flag "never_frosh"
		// *	  Never call the "TERM_XTRA_FROSH" action
		public bool never_frosh;

		// *	- Value "attr_blank"
		// *	  Use this "attr" value for "blank" grids
		public ConsoleColor attr_blank;

		// *	- Value "char_blank"
		// *	  Use this "char" value for "blank" grids
		public char char_blank;

		// *	- Ignore this pointer

		// *	- Keypress Queue -- various data
		// *	- Keypress Queue -- pending keys
		ui_event[] key_queue;

		UInt16 key_head;
		UInt16 key_tail;
		UInt16 key_xtra;
		UInt16 key_size;


		// *	- Window Width (max 255)
		public byte wid;

		// *	- Window Height (max 255)
		public byte hgt;

		// *	- Minimum modified row
		public byte y1;
		// *	- Maximum modified row
		public byte y2;

		// *	- Minimum modified column (per row)
		public byte[] x1;
		// *	- Maximum modified column (per row)
		public byte[] x2;

		///* Offsets used by the map subwindows */
		public byte offset_x;
		public byte offset_y;

		//term_win* for next 4
		// *	- Displayed screen image
		Term_Win old;
		// *	- Requested screen image
		public Term_Win scr;

		// *	- Temporary screen image
		Term_Win tmp;
		// *	- Memorized screen image
		Term_Win mem;

		///* Number of times saved */
		byte saved;



		// *	- Hook for init-ing the term
		// *	- Hook for nuke-ing the term
		// *
		// *	- Hook for extra actions
		// *
		// *	- Hook for placing the cursor
		// *
		// *	- Hook for drawing some blank spaces
		// *
		// *	- Hook for drawing a string of chars using an attr
		// *
		// *	- Hook for drawing a sequence of special attr/char pairs
		public delegate void term_hook_func(Term t);
		public term_hook_func init_hook;
		public term_hook_func nuke_hook;

		public delegate int nv_func(TERM_XTRA n, int v);
		public delegate int xy_func(int x, int y);
		public delegate int xyn_func(int x, int y, int n);
		public delegate int text_func(int x, int y, int n, ConsoleColor a, char[] s);
		public delegate int pict_func(int x, int y, int n, ConsoleColor[] a, char[] s, ConsoleColor[] tap, char[] tcp);
		public delegate char xchar_func(char c);
		public nv_func xtra_hook;
		public xy_func curs_hook;
		public xy_func bigcurs_hook;
		public xyn_func wipe_hook;
		public text_func text_hook;
		public pict_func pict_hook;
		public xchar_func xchar_hook;


		///* sketchy key logging pt. 1 */
		static int KEYLOG_SIZE = 8;
		static int log_i;
		static int log_size;
		static keypress[] keylog = new keypress[KEYLOG_SIZE];


		///**** Available Variables ****/

		public static Term instance;

		public static bool panel_contains(uint y, uint x) {
			uint hgt;
			uint wid;
			if (Term.instance == null)
			    return true;
			hgt = (uint)Misc.SCREEN_HGT;
			wid = (uint)Misc.SCREEN_WID;
			return (y - instance.offset_y) < hgt && (x - instance.offset_x) < wid;
		}

		/*
		 * This file provides a generic, efficient, terminal window package,
		 * which can be used not only on standard terminal environments such
		 * as dumb terminals connected to a Unix box, but also in more modern
		 * "graphic" environments, such as the Macintosh or Unix/X11.
		 *
		 * Each "window" works like a standard "dumb terminal", that is, it
		 * can display a two dimensional array of grids containing colored
		 * textual symbols, plus an optional cursor, and it can be used to
		 * get keypress events from the user.
		 *
		 * In fact, this package can simply be used, if desired, to support
		 * programs which will look the same on a dumb terminal as they do
		 * on a graphic platform such as the Macintosh.
		 *
		 * This package was designed to help port the game "Angband" to a wide
		 * variety of different platforms.  Angband, like many other games in
		 * the "rogue-like" heirarchy, requires, at the minimum, the ability
		 * to display "colored textual symbols" in a standard 80x24 "window",
		 * such as that provided by most dumb terminals, and many old personal
		 * computers, and to check for "keypresses" from the user.  The major
		 * concerns were thus portability and efficiency, so Angband could be
		 * easily ported to many different systems, with minimal effort, and
		 * yet would run quickly on each of these systems, no matter what kind
		 * of underlying hardware/software support was being used.
		 *
		 * It is important to understand the differences between the older
		 * "dumb terminals" and the newer "graphic interface" machines, since
		 * this package was designed to work with both types of systems.
		 *
		 * New machines:
		 *   waiting for a keypress is complex
		 *   checking for a keypress is often cheap
		 *   changing "colors" may be expensive
		 *   the "color" of a "blank" is rarely important
		 *   moving the "cursor" is relatively cheap
		 *   use a "software" cursor (only moves when requested)
		 *   drawing characters normally will not erase old ones
		 *   drawing a character on the cursor often erases it
		 *   may have fast routines for "clear a region"
		 *   the bottom right corner is usually not special
		 *
		 * Old machines:
		 *   waiting for a keypress is simple
		 *   checking for a keypress is often expensive
		 *   changing "colors" is usually cheap
		 *   the "color" of a "blank" may be important
		 *   moving the "cursor" may be expensive
		 *   use a "hardware" cursor (moves during screen updates)
		 *   drawing new symbols automatically erases old ones
		 *   characters may only be drawn at the cursor location
		 *   drawing a character on the cursor will move the cursor
		 *   may have fast routines for "clear entire window"
		 *   may have fast routines for "clear to end of line"
		 *   the bottom right corner is often dangerous
		 *
		 *
		 * This package provides support for multiple windows, each of an
		 * arbitrary size (up to 255x255), each with its own set of flags,
		 * and its own hooks to handle several low-level procedures which
		 * differ from platform to platform.  Then the main program simply
		 * creates one or more "term" structures, setting the various flags
		 * and hooks in a manner appropriate for the current platform, and
		 * then it can use the various "term" structures without worrying
		 * about the underlying platform.
		 *
		 *
		 * This package allows each "grid" in each window to hold an attr/char
		 * pair, with each ranging from 0 to 255, and makes very few assumptions
		 * about the meaning of any attr/char values.  Normally, we assume that
		 * "attr 0" is "black", with the semantics that "black" text should be
		 * sent to "Term_wipe()" instead of "Term_text()", but this sematics is
		 * modified if either the "always_pict" or the "always_text" flags are
		 * set.  We assume that "char 0" is "dangerous", since placing such a
		 * "char" in the middle of a string "terminates" the string, and usually
		 * we prevent its use.
		 *
		 * Finally, we use a special attr/char pair, defaulting to "attr 0" and
		 * "char 32", also known as "black space", when we "erase" or "clear"
		 * any window, but this pair can be redefined to any pair, including
		 * the standard "white space", or the bizarre "emptiness" ("attr 0"
		 * and "char 0"), as long as various obscure restrictions are met.
		 *
		 *
		 * This package provides several functions which allow a program to
		 * interact with the "term" structures.  Most of the functions allow
		 * the program to "request" certain changes to the current "term",
		 * such as moving the cursor, drawing an attr/char pair, erasing a
		 * region of grids, hiding the cursor, etc.  Then there is a special
		 * function which causes all of the "pending" requests to be performed
		 * in an efficient manner.  There is another set of functions which
		 * allow the program to query the "requested state" of the current
		 * "term", such as asking for the cursor location, or what attr/char
		 * is at a given location, etc.  There is another set of functions
		 * dealing with "keypress" events, which allows the program to ask if
		 * the user has pressed any keys, or to forget any keys the user pressed.
		 * There is a pair of functions to allow this package to memorize the
		 * contents of the current "term", and to restore these contents at
		 * a later time.  There is a special function which allows the program
		 * to specify which "term" structure should be the "current" one.  At
		 * the lowest level, there is a set of functions which allow a new
		 * "term" to be initialized or destroyed, and which allow this package,
		 * or a program, to access the special "hooks" defined for the current
		 * "term", and a set of functions which those "hooks" can use to inform
		 * this package of the results of certain occurances, for example, one
		 * such function allows this package to learn about user keypresses,
		 * detected by one of the special "hooks".
		 *
		 * We provide, among other things, the functions "Term_keypress()"
		 * to "react" to keypress events, and "Term_redraw()" to redraw the
		 * entire window, plus "Term_resize()" to note a new size.
		 *
		 *
		 * Note that the current "term" contains two "window images".  One of
		 * these images represents the "requested" contents of the "term", and
		 * the other represents the "actual" contents of the "term", at the time
		 * of the last performance of pending requests.  This package uses these
		 * two images to determine the "minimal" amount of work needed to make
		 * the "actual" contents of the "term" match the "requested" contents of
		 * the "term".  This method is not perfect, but it often reduces the
		 * amount of work needed to perform the pending requests, which thus
		 * increases the speed of the program itself.  This package promises
		 * that the requested changes will appear to occur either "all at once"
		 * or in a "top to bottom" order.  In addition, a "cursor" is maintained,
		 * and this cursor is updated along with the actual window contents.
		 *
		 * Currently, the "Term_fresh()" routine attempts to perform the "minimum"
		 * number of physical updates, in terms of total "work" done by the hooks
		 * Term_wipe(), Term_text(), and Term_pict(), making use of the fact that
		 * adjacent characters of the same color can both be drawn together using
		 * the "Term_text()" hook, and that "black" text can often be sent to the
		 * "Term_wipe()" hook instead of the "Term_text()" hook, and if something
		 * is already displayed in a window, then it is not necessary to display
		 * it again.  Unfortunately, this may induce slightly non-optimal results
		 * in some cases, in particular, those in which, say, a string of ten
		 * characters needs to be written, but the fifth character has already
		 * been displayed.  Currently, this will cause the "Term_text()" routine
		 * to be called once for each half of the string, instead of once for the
		 * whole string, which, on some machines, may be non-optimal behavior.
		 *
		 * The new formalism includes a "displayed" screen image (old) which
		 * is actually seen by the user, a "requested" screen image (scr)
		 * which is being prepared for display, a "memorized" screen image
		 * (mem) which is used to save and restore screen images, and a
		 * "temporary" screen image (tmp) which is currently unused.
		 *
		 *
		 * Several "flags" are available in each "term" to allow the underlying
		 * visual system (which initializes the "term" structure) to "optimize"
		 * the performance of this package for the given system, or to request
		 * certain behavior which is helpful/required for the given system.
		 *
		 * The "soft_cursor" flag indicates the use of a "soft" cursor, which
		 * only moves when explicitly requested,and which is "erased" when
		 * any characters are drawn on top of it.  This flag is used for all
		 * "graphic" systems which handle the cursor by "drawing" it.
		 *
		 * The "icky_corner" flag indicates that the bottom right "corner"
		 * of the windows are "icky", and "printing" anything there may
		 * induce "messy" behavior, such as "scrolling".  This flag is used
		 * for most old "dumb terminal" systems.
		 *
		 *
		 * The "term" structure contains the following function "hooks":
		 *
		 *   instance.init_hook = Init the term
		 *   instance.nuke_hook = Nuke the term
		 *   instance.xtra_hook = Perform extra actions
		 *   instance.curs_hook = Draw (or Move) the cursor
		 *   instance.bigcurs_hook = Draw (or Move) the big cursor (bigtile mode)
		 *   instance.wipe_hook = Draw some blank spaces
		 *   instance.text_hook = Draw some text in the window
		 *   instance.pict_hook = Draw some attr/chars in the window
		 *
		 * The "instance.xtra_hook" hook provides a variety of different functions,
		 * based on the first parameter (which should be taken from the various
		 * TERM_XTRA_* defines) and the second parameter (which may make sense
		 * only for some first parameters).  It is available to the program via
		 * the "Term_xtra()" function, though some first parameters are only
		 * "legal" when called from inside this package.
		 *
		 * The "instance.curs_hook" hook provides this package with a simple way
		 * to "move" or "draw" the cursor to the grid "x,y", depending on the
		 * setting of the "soft_cursor" flag.  Note that the cursor is never
		 * redrawn if "nothing" has happened to the screen (even temporarily).
		 * This hook is required.
		 *
		 * The "instance.wipe_hook" hook provides this package with a simple way
		 * to "erase", starting at "x,y", the next "n" grids.  This hook assumes
		 * that the input is valid.  This hook is required, unless the setting
		 * of the "always_pict" or "always_text" flags makes it optional.
		 *
		 * The "instance.text_hook" hook provides this package with a simple way
		 * to "draw", starting at "x,y", the "n" chars contained in "cp", using
		 * the attr "a".  This hook assumes that the input is valid, and that
		 * "n" is between 1 and 256 inclusive, but it should NOT assume that
		 * the contents of "cp" are null-terminated.  This hook is required,
		 * unless the setting of the "always_pict" flag makes it optional.
		 *
		 * The "instance.pict_hook" hook provides this package with a simple way
		 * to "draw", starting at "x,y", the "n" attr/char pairs contained in
		 * the arrays "ap" and "cp".  This hook assumes that the input is valid,
		 * and that "n" is between 1 and 256 inclusive, but it should NOT assume
		 * that the contents of "cp" are null-terminated.  This hook is optional,
		 * unless the setting of the "always_pict" or "higher_pict" flags make
		 * it required.  Note that recently, this hook was changed from taking
		 * a byte "a" and a char "c" to taking a length "n", an array of bytes
		 * "ap" and an array of chars "cp".  Old implementations of this hook
		 * should now iterate over all "n" attr/char pairs.
		 * The two new arrays "tap" and "tcp" can contain the attr/char pairs
		 * of the terrain below the values in "ap" and "cp".  These values can
		 * be used to implement transparency when using graphics by drawing
		 * the terrain values as a background and the "ap", "cp" values in
		 * the foreground.
		 *
		 * The game "Angband" uses a set of files called "main-xxx.c", for
		 * various "xxx" suffixes.  Most of these contain a function called
		 * "init_xxx()", that will prepare the underlying visual system for
		 * use with Angband, and then create one or more "term" structures,
		 * using flags and hooks appropriate to the given platform, so that
		 * the "main()" function can call one (or more) of the "init_xxx()"
		 * functions, as appropriate, to prepare the required "term" structs
		 * (one for each desired sub-window), and these "init_xxx()" functions
		 * are called from a centralized "main()" function in "main.c".  Other
		 * "main-xxx.c" systems contain their own "main()" function which, in
		 * addition to doing everything needed to initialize the actual program,
		 * also does everything that the normal "init_xxx()" functions would do.
		 *
		 * The game "Angband" defines, in addition to "attr 0", all of the
		 * attr codes from 1 to 15, using definitions in "defines.h", and
		 * thus the "main-xxx.c" files used by Angband must handle these
		 * attr values correctly.  Also, they must handle all other attr
		 * values, though they may do so in any way they wish, for example,
		 * by always taking every attr code mod 16.  Many of the "main-xxx.c"
		 * files use "white space" ("attr 1" / "char 32") to "erase" or "clear"
		 * any window, for efficiency.
		 *
		 * See "main-xxx.c" for a simple skeleton file which can be used to
		 * create a "visual system" for a new platform when porting Angband.
		 */






		///*
		// * The current "term"
		// */
		//term *Term = null;

		///* grumbles */
		//int log_i = 0;
		//int log_size = 0;

		///*** External hooks ***/


		/*
		 * Execute the "instance.xtra_hook" hook, if available (see above).
		 */
		public static int xtra(TERM_XTRA n, int v) {
			/* Verify the hook */
			if(instance.xtra_hook == null)
				return (-1);

			/* Call the hook */
			return (instance.xtra_hook(n, v));
		}



		///*** Fake hooks ***/


		/*
		 * Hack -- fake hook for "Term_curs()" (see above)
		 */
		public static int curs_hack(int x, int y) {
			/* Compiler silliness */
			if(x != 0 || y != 0)
				return (-2);

			/* Oops */
			return (-1);
		}

		/*
		 * Hack -- fake hook for "Term_wipe()" (see above)
		 */
		public static int wipe_hack(int x, int y, int n) {
			/* Compiler silliness */
			if(x != 0 || y != 0 || n != 0)
				return (-2);

			/* Oops */
			return (-1);
		}

		/*
		 * Hack -- fake hook for "Term_text()" (see above)
		 */
		public static int text_hack(int x, int y, int n, ConsoleColor a, char[] cp) {
			/* Compiler silliness */
			if(x != 0 || y != 0 || n != 0 || a != 0 || cp != null)
				return (-2);

			/* Oops */
			return (-1);
		}


		/*
		 * Hack -- fake hook for "Term_pict()" (see above)
		 */
		public static int pict_hack(int x, int y, int n, ConsoleColor[] ap, char[] cp, ConsoleColor[] tap, char[] tcp) {
			/* Compiler silliness */
			if(x != 0 || y != 0 || n != 0 || ap != null || cp != null || tap != null || tcp != null)
				return (-2);

			/* Oops */
			return (-1);
		}


		///*** Efficient routines ***/


		/*
		 * Mentally draw an attr/char at a given location
		 *
		 * Assumes given location and values are valid.
		 */
		public void queue_char(int x, int y, ConsoleColor a, char c, ConsoleColor ta, char tc) {
			Term t = this;
			ConsoleColor[] scr_aa = t.scr.a[y];
			char[] scr_cc = t.scr.c[y];

			ConsoleColor oa = scr_aa[x];
			char oc = scr_cc[x];

			ConsoleColor[] scr_taa = t.scr.ta[y];
			char[] scr_tcc = t.scr.tc[y];

			ConsoleColor ota = scr_taa[x];
			char otc = scr_tcc[x];

			/* Don't change if the terrain value is 0 */
			if(ta == 0)
				ta = ota;
			if(tc == 0)
				tc = otc;

			/* Hack -- Ignore non-changes */
			if((oa == a) && (oc == c) && (ota == ta) && (otc == tc))
				return;

			/* Save the "literal" information */
			scr_aa[x] = a;
			scr_cc[x] = c;

			scr_taa[x] = ta;
			scr_tcc[x] = tc;

			/* Check for new min/max row info */
			if(y < t.y1)
				t.y1 = (byte)y;
			if(y > t.y2)
				t.y2 = (byte)y;

			/* Check for new min/max col info for this row */
			if(x < t.x1[y])
				t.x1[y] = (byte)x;
			if(x > t.x2[y])
				t.x2[y] = (byte)x;
		}

		/* Queue a large-sized tile */

		public void big_queue_char(int x, int y, ConsoleColor a, char c, ConsoleColor a1, char c1) {
			int hor, vert;

			/* Avoid warning */
			//(void)c;

			/* No tall skinny tiles */
			if(tile_width > 1) {
				/* Horizontal first */
				for(hor = 0; hor <= tile_width; hor++) {
					/* Queue dummy character */
					if(hor != 0) {
						if(((int)a & 0x80) != 0)
							queue_char(x + hor, y, ConsoleColor.DarkGray, (char)255, ConsoleColor.White, '\0'); //Was 255, 0 for color
						else
							queue_char(x + hor, y, ConsoleColor.White, ' ', a1, c1);
					}

					/* Now vertical */
					for(vert = 1; vert <= tile_height; vert++) {
						/* Queue dummy character */
						if(((int)a & 0x80) != 0)
							throw new NotImplementedException();
							//Term_queue_char(t, x + hor, y + vert, 255, -1, 0, 0);
						else
							queue_char(x + hor, y + vert, ConsoleColor.White, ' ', a1, c1);
					}
				}
			} else {
				/* Only vertical */
				for(vert = 1; vert <= tile_height; vert++) {
					/* Queue dummy character */
					if(((int)a & 0x80) != 0)
						throw new NotImplementedException();
						//Term_queue_char(t, x, y + vert, 255, -1, 0, 0);
					else
						queue_char(x, y + vert, ConsoleColor.White, ' ', a1, c1);
				}
			}
		}

		/*
		 * Mentally draw some attr/chars at a given location
		 *
		 * Assumes that (x,y) is a valid location, that the first "n" characters
		 * of the string "s" are all valid (non-zero), and that (x+n-1,y) is also
		 * a valid location, so the first "n" characters of "s" can all be added
		 * starting at (x,y) without causing any illegal operations.
		 */
		static void queue_chars(int x, int y, int n, ConsoleColor a, string s) {
			int x1 = -1, x2 = -1;

			ConsoleColor[] scr_aa = instance.scr.a[y];
			char[] scr_cc = instance.scr.c[y];

			ConsoleColor[] scr_taa = instance.scr.ta[y];
			char[] scr_tcc = instance.scr.tc[y];

			/* Queue the attr/chars */
			int cnt = 0;
			for(; n != 0; cnt++, x++, n--) {
				char c = s[cnt];

				ConsoleColor oa = scr_aa[x];
				char oc = scr_cc[x];

				ConsoleColor ota = scr_taa[x];
				char otc = scr_tcc[x];

				/* Hack -- Ignore non-changes */
				if((oa == a) && (oc == c) && (ota == 0) && (otc == 0))
					continue;

				/* Save the "literal" information */
				scr_aa[x] = a;
				scr_cc[x] = c;//xchar_trans(*s);

				scr_taa[x] = 0;
				scr_tcc[x] = (char)0;

				/* Note the "range" of window updates */
				if(x1 < 0)
					x1 = x;
				x2 = x;
			}

			/* Expand the "change area" as needed */
			if(x1 >= 0) {
				/* Check for new min/max row info */
				if(y < instance.y1)
					instance.y1 = (byte)y;
				if(y > instance.y2)
					instance.y2 = (byte)y;

				/* Check for new min/max col info in this row */
				if(x1 < instance.x1[y])
					instance.x1[y] = (byte)x1;
				if(x2 > instance.x2[y])
					instance.x2[y] = (byte)x2;
			}
		}



		///*** Refresh routines ***/


		/*
		 * Flush a row of the current window (see "Term_fresh")
		 *
		 * Display text using "Term_pict()"
		 */
		static void fresh_row_pict(int y, int x1, int x2) {
			throw new NotImplementedException();
			//int x;

			//byte *old_aa = instance.old.a[y];
			//char *old_cc = instance.old.c[y];

			//byte *scr_aa = instance.scr.a[y];
			//char *scr_cc = instance.scr.c[y];

			//byte *old_taa = instance.old.ta[y];
			//char *old_tcc = instance.old.tc[y];

			//byte *scr_taa = instance.scr.ta[y];
			//char *scr_tcc = instance.scr.tc[y];

			//byte ota;
			//char otc;

			//byte nta;
			//char ntc;

			///* Pending length */
			//int fn = 0;

			///* Pending start */
			//int fx = 0;

			//byte oa;
			//char oc;

			//byte na;
			//char nc;

			///* Scan "modified" columns */
			//for (x = x1; x <= x2; x++)
			//{
			//    /* See what is currently here */
			//    oa = old_aa[x];
			//    oc = old_cc[x];

			//    /* See what is desired there */
			//    na = scr_aa[x];
			//    nc = scr_cc[x];

			//    ota = old_taa[x];
			//    otc = old_tcc[x];

			//    nta = scr_taa[x];
			//    ntc = scr_tcc[x];

			//    /* Handle unchanged grids */
			//    if ((na == oa) && (nc == oc) && (nta == ota) && (ntc == otc))
			//    {
			//        /* Flush */
			//        if (fn)
			//        {
			//            /* Draw pending attr/char pairs */
			//            (void)((*instance.pict_hook)(fx, y, fn, &scr_aa[fx], &scr_cc[fx],
			//                          &scr_taa[fx], &scr_tcc[fx]));

			//            /* Forget */
			//            fn = 0;
			//        }

			//        /* Skip */
			//        continue;
			//    }

			//    /* Save new contents */
			//    old_aa[x] = na;
			//    old_cc[x] = nc;

			//    old_taa[x] = nta;
			//    old_tcc[x] = ntc;

			//    /* Restart and Advance */
			//    if (fn++ == 0) fx = x;
			//}

			///* Flush */
			//if (fn)
			//{
			//    /* Draw pending attr/char pairs */
			//    (void)((*instance.pict_hook)(fx, y, fn, &scr_aa[fx], &scr_cc[fx],
			//                  &scr_taa[fx], &scr_tcc[fx]));
			//}
		}


		static char[] get_subarray(char[] from, int startat, int length) {
			char[] ret = new char[length];

			for(int i = 0; i < length; i++) {
				ret[i] = from[i + startat];
			}

			return ret;
		}


		/*
		 * Flush a row of the current window (see "Term_fresh")
		 *
		 * Display text using "Term_text()" and "Term_wipe()",
		 * but use "Term_pict()" for high-bit attr/char pairs
		 */
		static void fresh_row_both(int y, int x1, int x2) {
			int x;

			ConsoleColor[] old_aa = instance.old.a[y];
			char[] old_cc = instance.old.c[y];
			ConsoleColor[] scr_aa = instance.scr.a[y];
			char[] scr_cc = instance.scr.c[y];

			ConsoleColor[] old_taa = instance.old.ta[y];
			char[] old_tcc = instance.old.tc[y];
			ConsoleColor[] scr_taa = instance.scr.ta[y];
			char[] scr_tcc = instance.scr.tc[y];

			ConsoleColor ota;
			char otc;
			ConsoleColor nta;
			char ntc;

			/* The "always_text" flag */
			bool always_text = instance.always_text;

			/* Pending length */
			int fn = 0;

			/* Pending start */
			int fx = 0;

			/* Pending attr */
			ConsoleColor fa = instance.attr_blank;

			ConsoleColor oa;
			char oc;

			ConsoleColor na;
			char nc;

			/* Scan "modified" columns */
			for(x = x1; x <= x2; x++) {
				/* See what is currently here */
				oa = old_aa[x];
				oc = old_cc[x];

				/* See what is desired there */
				na = scr_aa[x];
				nc = scr_cc[x];

				ota = old_taa[x];
				otc = old_tcc[x];

				nta = scr_taa[x];
				ntc = scr_tcc[x];

				/* Handle unchanged grids */
				if((na == oa) && (nc == oc) && (nta == ota) && (ntc == otc)) {
					/* Flush */
					if(fn != 0) {
						/* Draw pending chars (normal) */
						if(fa != 0 || always_text) {
							instance.text_hook(fx, y, fn, fa, get_subarray(scr_cc, fx, fn));
						}

						/* Draw pending chars (black) */
						else {
							instance.wipe_hook(fx, y, fn);
						}

						/* Forget */
						fn = 0;
					}

					/* Skip */
					continue;
				}

				/* Save new contents */
				old_aa[x] = na;
				old_cc[x] = nc;
				old_taa[x] = nta;
				old_tcc[x] = ntc;

				/* Handle high-bit attr/chars */
				if(((int)na & 0x80) != 0 && (nc & 0x80) != 0) {
					/* Flush */
					if(fn != 0) {
						/* Draw pending chars (normal) */
						if(fa != 0 || always_text) {
							instance.text_hook(fx, y, fn, fa, get_subarray(scr_cc, fx, fn));
						}

						/* Draw pending chars (black) */
						else {
							instance.wipe_hook(fx, y, fn);
						}

						/* Forget */
						fn = 0;
					}

					/* 2nd byte of bigtile */
					if(((int)na == 255) && (nc == (char)255))
						continue;

					/* Hack -- Draw the special attr/char pair */
					instance.pict_hook(x, y, 1, new ConsoleColor[] { na }, new char[] { nc }, new ConsoleColor[] { nta }, new char[] { ntc });

					/* Skip */
					continue;
				}

				/* Notice new color */
				if(fa != na) {
					/* Flush */
					if(fn != 0) {
						/* Draw the pending chars */
						if(fa != 0 || always_text) {
							instance.text_hook(fx, y, fn, fa, get_subarray(scr_cc, fx, fn));
						}

						/* Hack -- Erase "leading" spaces */
						else {
							instance.wipe_hook(fx, y, fn);
						}

						/* Forget */
						fn = 0;
					}

					/* Save the new color */
					fa = na;
				}

				/* Restart and Advance */
				if(fn++ == 0)
					fx = x;
			}

			/* Flush */
			if(fn != 0) {
				/* Draw pending chars (normal) */
				if(fa != 0 || always_text) {
					instance.text_hook(fx, y, fn, fa, get_subarray(scr_cc, fx, fn));
				}

				/* Draw pending chars (black) */
				else {
					instance.wipe_hook(fx, y, fn);
				}
			}
		}


		/*
		 * Flush a row of the current window (see "Term_fresh")
		 *
		 * Display text using "Term_text()" and "Term_wipe()"
		 */
		static void fresh_row_text(int y, int x1, int x2) {
			throw new NotImplementedException();
			//int x;

			//byte *old_aa = instance.old.a[y];
			//char *old_cc = instance.old.c[y];

			//byte *scr_aa = instance.scr.a[y];
			//char *scr_cc = instance.scr.c[y];

			///* The "always_text" flag */
			//int always_text = instance.always_text;

			///* Pending length */
			//int fn = 0;

			///* Pending start */
			//int fx = 0;

			///* Pending attr */
			//byte fa = instance.attr_blank;

			//byte oa;
			//char oc;

			//byte na;
			//char nc;


			///* Scan "modified" columns */
			//for (x = x1; x <= x2; x++)
			//{
			//    /* See what is currently here */
			//    oa = old_aa[x];
			//    oc = old_cc[x];

			//    /* See what is desired there */
			//    na = scr_aa[x];
			//    nc = scr_cc[x];

			//    /* Handle unchanged grids */
			//    if ((na == oa) && (nc == oc))
			//    {
			//        /* Flush */
			//        if (fn)
			//        {
			//            /* Draw pending chars (normal) */
			//            if (fa || always_text)
			//            {
			//                (void)((*instance.text_hook)(fx, y, fn, fa, &scr_cc[fx]));
			//            }

			//            /* Draw pending chars (black) */
			//            else
			//            {
			//                (void)((*instance.wipe_hook)(fx, y, fn));
			//            }

			//            /* Forget */
			//            fn = 0;
			//        }

			//        /* Skip */
			//        continue;
			//    }

			//    /* Save new contents */
			//    old_aa[x] = na;
			//    old_cc[x] = nc;

			//    /* Notice new color */
			//    if (fa != na)
			//    {
			//        /* Flush */
			//        if (fn)
			//        {
			//            /* Draw the pending chars */
			//            if (fa || always_text)
			//            {
			//                (void)((*instance.text_hook)(fx, y, fn, fa, &scr_cc[fx]));
			//            }

			//            /* Hack -- Erase "leading" spaces */
			//            else
			//            {
			//                (void)((*instance.wipe_hook)(fx, y, fn));
			//            }

			//            /* Forget */
			//            fn = 0;
			//        }

			//        /* Save the new color */
			//        fa = na;
			//    }

			//    /* Restart and Advance */
			//    if (fn++ == 0) fx = x;
			//}

			///* Flush */
			//if (fn)
			//{
			//    /* Draw pending chars (normal) */
			//    if (fa || always_text)
			//    {
			//        (void)((*instance.text_hook)(fx, y, fn, fa, &scr_cc[fx]));
			//    }

			//    /* Draw pending chars (black) */
			//    else
			//    {
			//        (void)((*instance.wipe_hook)(fx, y, fn));
			//    }
			//}
		}

		public static byte tile_width = 1;            /* Tile width in units of font width */
		public static byte tile_height = 1;           /* Tile height in units of font height */

		/* Helper variables for large cursor */
		static bool bigcurs = false;
		public static bool smlcurs = true;


		/*
		 * Actually perform all requested changes to the window
		 *
		 * If absolutely nothing has changed, not even temporarily, or if the
		 * current "Term" is not mapped, then this function will return 1 and
		 * do absolutely nothing.
		 *
		 * Note that when "soft_cursor" is true, we erase the cursor (if needed)
		 * whenever anything has changed, and redraw it (if needed) after all of
		 * the screen updates are complete.  This will induce a small amount of
		 * "cursor flicker" but only when the screen has been updated.  If the
		 * screen is updated and then restored, you may still get this flicker.
		 *
		 * When "soft_cursor" is not true, we make the cursor invisible before
		 * doing anything else if it is supposed to be invisible by the time we
		 * are done, and we make it visible after moving it to its final location
		 * after all of the screen updates are complete.
		 *
		 * Note that "Term_xtra(TERM_XTRA_CLEAR,0)" must erase the entire screen,
		 * including the cursor, if needed, and may place the cursor anywhere.
		 *
		 * Note that "Term_xtra(TERM_XTRA_FROSH,y)" will be always be called
		 * after any row "y" has been "flushed", unless the "instance.never_frosh"
		 * flag is set, and "Term_xtra(TERM_XTRA_FRESH,0)" will be called after
		 * all of the rows have been "flushed".
		 *
		 * Note the use of three different functions to handle the actual flush,
		 * based on the settings of the "instance.always_pict" and "instance.higher_pict"
		 * flags (see below).
		 *
		 * The three helper functions (above) work by collecting similar adjacent
		 * grids into stripes, and then sending each stripe to "instance.pict_hook",
		 * "instance.text_hook", or "instance.wipe_hook", based on the settings of the
		 * "instance.always_pict" and "instance.higher_pict" flags, which select which
		 * of the helper functions to call to flush each row.
		 *
		 * The helper functions currently "skip" any grids which already contain
		 * the desired contents.  This may or may not be the best method, especially
		 * when the desired content fits nicely into the current stripe.  For example,
		 * it might be better to go ahead and queue them while allowed, but keep a
		 * count of the "trailing skipables", then, when time to flush, or when a
		 * "non skippable" is found, force a flush if there are too many skippables.
		 *
		 * Perhaps an "initialization" stage, where the "text" (and "attr")
		 * buffers are "filled" with information, converting "blanks" into
		 * a convenient representation, and marking "skips" with "zero chars",
		 * and then some "processing" is done to determine which chars to skip.
		 *
		 * Currently, the helper functions are optimal for systems which prefer
		 * to "print a char + move a char + print a char" to "print three chars",
		 * and for applications that do a lot of "detailed" color printing.
		 *
		 * In the two "queue" functions, total "non-changes" are "pre-skipped".
		 * The helper functions must also handle situations in which the contents
		 * of a grid are changed, but then changed back to the original value,
		 * and situations in which two grids in the same row are changed, but
		 * the grids between them are unchanged.
		 *
		 * If the "instance.always_pict" flag is set, then "Term_fresh_row_pict()"
		 * will be used instead of "Term_fresh_row_text()".  This allows all the
		 * modified grids to be collected into stripes of attr/char pairs, which
		 * are then sent to the "instance.pict_hook" hook, which can draw these pairs
		 * in whatever way it would like.
		 *
		 * If the "instance.higher_pict" flag is set, then "Term_fresh_row_both()"
		 * will be used instead of "Term_fresh_row_text()".  This allows all the
		 * "special" attr/char pairs (in which both the attr and char have the
		 * high-bit set) to be sent (one pair at a time) to the "instance.pict_hook"
		 * hook, which can draw these pairs in whatever way it would like.
		 *
		 * Normally, the "Term_wipe()" function is used only to display "blanks"
		 * that were induced by "Term_clear()" or "Term_erase()", and then only
		 * if the "attr_blank" and "char_blank" fields have not been redefined
		 * to use "white space" instead of the default "black space".  Actually,
		 * the "Term_wipe()" function is used to display all "black" text, such
		 * as the default "spaces" created by "Term_clear()" and "Term_erase()".
		 *
		 * Note that the "instance.always_text" flag will disable the use of the
		 * "Term_wipe()" function hook entirely, and force all text, even text
		 * drawn in the color "black", to be explicitly drawn.  This is useful
		 * for machines which implement "Term_wipe()" by just drawing spaces.
		 *
		 * Note that the "instance.always_pict" flag will disable the use of the
		 * "Term_wipe()" function entirely, and force everything, even text
		 * drawn in the attr "black", to be explicitly drawn.
		 *
		 * Note that if no "black" text is ever drawn, and if "attr_blank" is
		 * not "zero", then the "Term_wipe" hook will never be used, even if
		 * the "instance.always_text" flag is not set.
		 *
		 * This function does nothing unless the "Term" is "mapped", which allows
		 * certain systems to optimize the handling of "closed" windows.
		 *
		 * On systems with a "soft" cursor, we must explicitly erase the cursor
		 * before flushing the output, if needed, to prevent a "jumpy" refresh.
		 * The actual method for this is horrible, but there is very little that
		 * we can do to simplify it efficiently.  XXX XXX XXX
		 *
		 * On systems with a "hard" cursor, we will "hide" the cursor before
		 * flushing the output, if needed, to avoid a "flickery" refresh.  It
		 * would be nice to *always* hide the cursor during the refresh, but
		 * this might be expensive (and/or ugly) on some machines.
		 *
		 * The "instance.icky_corner" flag is used to avoid calling "Term_wipe()"
		 * or "Term_pict()" or "Term_text()" on the bottom right corner of the
		 * window, which might induce "scrolling" or other nasty stuff on old
		 * dumb terminals.  This flag is handled very efficiently.  We assume
		 * that the "Term_curs()" call will prevent placing the cursor in the
		 * corner, if needed, though I doubt such placement is ever a problem.
		 * Currently, the use of "instance.icky_corner" and "instance.soft_cursor"
		 * together may result in undefined behavior.
		 */
		public static int fresh() {
			int x, y;

			int w = instance.wid;
			int h = instance.hgt;

			int y1 = instance.y1;
			int y2 = instance.y2;

			Term_Win old = instance.old;
			Term_Win scr = instance.scr;


			/* Do nothing unless "mapped" */
			if(!instance.mapped_flag)
				return (1);


			/* Trivial Refresh */
			if((y1 > y2) &&
				(scr.cu == old.cu) &&
				(scr.cv == old.cv) &&
				(scr.cx == old.cx) &&
				(scr.cy == old.cy) &&
				!(instance.total_erase)) {
				/* Nothing */
				return (1);
			}


			/* Paranoia -- use "fake" hooks to prevent core dumps */
			if(instance.curs_hook == null)
				instance.curs_hook = Term.curs_hack;
			if(instance.bigcurs_hook == null)
				instance.bigcurs_hook = instance.curs_hook;
			if(instance.wipe_hook == null)
				instance.wipe_hook = Term.wipe_hack;
			if(instance.text_hook == null)
				instance.text_hook = Term.text_hack;
			if(instance.pict_hook == null)
				instance.pict_hook = Term.pict_hack;


			/* Handle "total erase" */
			if(instance.total_erase) {
				ConsoleColor na = instance.attr_blank;
				char nc = instance.char_blank;

				/* Physically erase the entire window */
				Term.xtra(TERM_XTRA.CLEAR, 0);

				/* Hack -- clear all "cursor" data */
				old.cv = old.cu = false;
				old.cx = old.cy = 0;

				/* Wipe each row */
				for(y = 0; y < h; y++) {
					//ConsoleColor[] aa = old.a[y];
					//char[] cc = old.c[y];
					//ConsoleColor[] taa = old.ta[y];
					//char[] tcc = old.tc[y];

					/* Wipe each column */
					for(x = 0; x < w; x++) {
						/* Wipe each grid */
						old.a[y][x] = na;
						old.c[y][x] = nc;
						old.ta[y][x] = na;
						old.tc[y][x] = nc;
						//*aa++ = na;
						//*cc++ = nc;

						//*taa++ = na;
						//*tcc++ = nc;
					}
				}

				/* Redraw every row */
				y1 = 0;
				y2 = h - 1;
				instance.y1 = (byte)y1;
				instance.y2 = (byte)y2;

				/* Redraw every column */
				for(y = 0; y < h; y++) {
					instance.x1[y] = 0;
					instance.x2[y] = (byte)(w - 1);
				}

				/* Forget "total erase" */
				instance.total_erase = false;
			}


			/* Cursor update -- Erase old Cursor */
			if(instance.soft_cursor) {
				/* Cursor was visible */
				if(!old.cu && old.cv) {
					int tx = old.cx;
					int ty = old.cy;

					ConsoleColor[] old_aa = old.a[ty];
					char[] old_cc = old.c[ty];

					//Gotta wrap these for pict_hook
					ConsoleColor[] oa = new ConsoleColor[] { old_aa[tx] };
					char[] oc = new char[] { old_cc[tx] };

					ConsoleColor[] old_taa = old.ta[ty];
					char[] old_tcc = old.tc[ty];

					//Wrap these too
					ConsoleColor[] ota = new ConsoleColor[] { old_taa[tx] };
					char[] otc = new char[] { old_tcc[tx] };


					/* Hack -- use "Term_pict()" always */
					if(instance.always_pict) {
						instance.pict_hook(tx, ty, 1, oa, oc, ota, otc);
					}

					/* Hack -- use "Term_pict()" sometimes */
					else if(instance.higher_pict && ((byte)oa[0] & 0x80) != 0 && (oc[0] & 0x80) != 0) {
						instance.pict_hook(tx, ty, 1, oa, oc, ota, otc);
					}

					/* Hack -- restore the actual character */
					else if(oa[0] != 0 || instance.always_text) {
						instance.text_hook(tx, ty, 1, oa[0], oc);
					}

					/* Hack -- erase the grid */
					else {
						instance.wipe_hook(tx, ty, 1);
					}
				}
			}

			/* Cursor Update -- Erase old Cursor */
			else {
				/* Cursor will be invisible */
				if(scr.cu || !scr.cv) {
					/* Make the cursor invisible */
					Term.xtra(TERM_XTRA.SHAPE, 0);
				}
			}


			/* Something to update */
			if(y1 <= y2) {
				/* Handle "icky corner" */
				if(instance.icky_corner) {
					/* Avoid the corner */
					if(y2 >= h - 1) {
						/* Avoid the corner */
						if(instance.x2[h - 1] > w - 2) {
							/* Avoid the corner */
							instance.x2[h - 1] = (byte)(w - 2);
						}
					}
				}


				/* Scan the "modified" rows */
				for(y = y1; y <= y2; ++y) {
					int x1 = instance.x1[y];
					int x2 = instance.x2[y];

					/* Flush each "modified" row */
					if(x1 <= x2) {
						/* Always use "Term_pict()" */
						if(instance.always_pict) {
							/* Flush the row */
							fresh_row_pict(y, x1, x2);
						}

						/* Sometimes use "Term_pict()" */
						else if(instance.higher_pict) {
							/* Flush the row */
							fresh_row_both(y, x1, x2);
						}

						/* Never use "Term_pict()" */
						else {
							/* Flush the row */
							fresh_row_text(y, x1, x2);
						}

						/* This row is all done */
						instance.x1[y] = (byte)w;
						instance.x2[y] = 0;

						/* Hack -- Flush that row (if allowed) */
						if(!instance.never_frosh)
							Term.xtra(TERM_XTRA.FROSH, y);
					}
				}

				/* No rows are invalid */
				instance.y1 = (byte)h;
				instance.y2 = 0;
			}


			/* Cursor update -- Show new Cursor */
			if(instance.soft_cursor) {
				/* Draw the cursor */
				if(!scr.cu && scr.cv) {
					if((((tile_width > 1) || (tile_height > 1)) &&
						 (!smlcurs) && (instance.saved == 0) && (scr.cy > 0))
						|| bigcurs) {
						/* Double width cursor for the Bigtile mode */
						instance.bigcurs_hook(scr.cx, scr.cy);
					} else {
						/* Call the cursor display routine */
						instance.curs_hook(scr.cx, scr.cy);
					}
				}
			}

			/* Cursor Update -- Show new Cursor */
			else {
				/* The cursor is useless, hide it */
				if(scr.cu) {
					/* Paranoia -- Put the cursor NEAR where it belongs */
					instance.curs_hook(w - 1, scr.cy);

					/* Make the cursor invisible */
					/* Term_xtra(TERM_XTRA_SHAPE, 0); */
				}

				/* The cursor is invisible, hide it */
				else if(!scr.cv) {
					/* Paranoia -- Put the cursor where it belongs */
					instance.curs_hook(scr.cx, scr.cy);

					/* Make the cursor invisible */
					/* Term_xtra(TERM_XTRA_SHAPE, 0); */
				}

				/* The cursor is visible, display it correctly */
				else {
					/* Put the cursor where it belongs */
					instance.curs_hook(scr.cx, scr.cy);

					/* Make the cursor visible */
					Term.xtra(TERM_XTRA.SHAPE, 1);
				}
			}


			/* Save the "cursor state" */
			old.cu = scr.cu;
			old.cv = scr.cv;
			old.cx = scr.cx;
			old.cy = scr.cy;


			/* Actually flush the output */
			Term.xtra(TERM_XTRA.FRESH, 0);


			/* Success */
			return (0);
		}



		///*** Output routines ***/


		/*
		 * Set the cursor visibility
		 */
		public static int set_cursor(bool v) {
			/* Already done */
			if(instance.scr.cv == v)
				return (1);

			/* Change */
			instance.scr.cv = v;

			/* Success */
			return (0);
		}


		/*
		 * Place the cursor at a given location
		 *
		 * Note -- "illegal" requests do not move the cursor.
		 */
		public static int gotoxy(int x, int y) {
			int w = instance.wid;
			int h = instance.hgt;

			/* Verify */
			if((x < 0) || (x >= w))
				return (-1);
			if((y < 0) || (y >= h))
				return (-1);

			/* Remember the cursor */
			instance.scr.cx = (byte)x;
			instance.scr.cy = (byte)y;

			/* The cursor is not useless */
			instance.scr.cu = false;

			/* Success */
			return (0);
		}


		///*
		// * At a given location, place an attr/char
		// * Do not change the cursor position
		// * No visual changes until "Term_fresh()".
		// */
		//errr Term_draw(int x, int y, byte a, char c)
		//{
		//    int w = instance.wid;
		//    int h = instance.hgt;

		//    /* Verify location */
		//    if ((x < 0) || (x >= w)) return (-1);
		//    if ((y < 0) || (y >= h)) return (-1);

		//    /* Paranoia -- illegal char */
		//    if (!c) return (-2);

		//    /* Queue it for later */
		//    Term_queue_char(Term, x, y, a, c, 0, 0);

		//    /* Success */
		//    return (0);
		//}


		/*
		 * Using the given attr, add the given char at the cursor.
		 *
		 * We return "-2" if the character is "illegal". XXX XXX
		 *
		 * We return "-1" if the cursor is currently unusable.
		 *
		 * We queue the given attr/char for display at the current
		 * cursor location, and advance the cursor to the right,
		 * marking it as unuable and returning "1" if it leaves
		 * the screen, and otherwise returning "0".
		 *
		 * So when this function, or the following one, return a
		 * positive value, future calls to either function will
		 * return negative ones.
		 */
		public static int addch(ConsoleColor a, char c) {
			int w = instance.wid;

			/* Handle "unusable" cursor */
			if(instance.scr.cu)
				return (-1);

			/* Paranoia -- no illegal chars */
			//if (!c) return (-2); //Nick: meh

			/* Queue the given character for display */
			instance.queue_char(instance.scr.cx, instance.scr.cy, a, c, 0, '\0');

			/* Advance the cursor */
			instance.scr.cx++;

			/* Success */
			if(instance.scr.cx < w)
				return (0);

			/* Note "Useless" cursor */
			instance.scr.cu = true;

			/* Note "Useless" cursor */
			return (1);
		}


		/*
		 * At the current location, using an attr, add a string
		 *
		 * We also take a length "n", using negative values to imply
		 * the largest possible value, and then we use the minimum of
		 * this length and the "actual" length of the string as the
		 * actual number of characters to attempt to display, never
		 * displaying more characters than will actually fit, since
		 * we do NOT attempt to "wrap" the cursor at the screen edge.
		 *
		 * We return "-1" if the cursor is currently unusable.
		 * We return "N" if we were "only" able to write "N" chars,
		 * even if all of the given characters fit on the screen,
		 * and mark the cursor as unusable for future attempts.
		 *
		 * So when this function, or the preceding one, return a
		 * positive value, future calls to either function will
		 * return negative ones.
		 */
		public static int addstr(int n, ConsoleColor a, string buf) {
			int k;

			int w = instance.wid;

			int res = 0;

			string s;

			/* Copy to a rewriteable string */
			s = buf;

			/* Translate it to 7-bit ASCII or system-specific format */
			//xstr_trans(s, LATIN1); I don't think this is needed...

			/* Handle "unusable" cursor */
			if(instance.scr.cu)
				return (-1);

			/* Obtain maximal length */
			k = (n < 0) ? (w + 1) : n;

			/* Obtain the usable string length */
			for(n = 0; (n < k) && n < s.Length; n++) /* loop */
				;

			/* React to reaching the edge of the screen */
			if(instance.scr.cx + n >= w)
				res = n = w - instance.scr.cx;

			/* Queue the first "n" characters for display */
			queue_chars(instance.scr.cx, instance.scr.cy, n, a, s);

			/* Advance the cursor */
			instance.scr.cx += (byte)n;

			/* Hack -- Notice "Useless" cursor */
			if(res != 0)
				instance.scr.cu = true;

			/* Success (usually) */
			return (res);
		}


		/*
		 * Move to a location and, using an attr, add a char
		 */
		public static int putch(int x, int y, ConsoleColor a, char c) {
			int res;

			/* Move first */
			if((res = Term.gotoxy(x, y)) != 0)
				return (res);

			/* Then add the char */
			if((res = Term.addch(a, c)) != 0)
				return (res);

			/* Success */
			return (0);
		}


		///*
		// * Move to a location and, using an attr, add a big tile
		// */
		//void Term_big_putch(int x, int y, byte a, char c)
		//{
		//    int hor, vert;

		//    /* Avoid warning */
		//    (void)c;

		//    /* No tall skinny tiles */
		//    if (tile_width > 1)
		//    {
		//        /* Horizontal first */
		//        for (hor = 0; hor <= tile_width; hor++)
		//        {
		//            /* Queue dummy character */
		//            if (hor != 0)
		//            {
		//                if (a & 0x80)
		//                    Term_putch(x + hor, y, 255, -1);
		//                else
		//                    Term_putch(x + hor, y, TERM_WHITE, ' ');
		//            }

		//            /* Now vertical */
		//            for (vert = 1; vert <= tile_height; vert++)
		//            {
		//                /* Queue dummy character */
		//                if (a & 0x80)
		//                    Term_putch(x + hor, y + vert, 255, -1);
		//                else
		//                    Term_putch(x + hor, y + vert, TERM_WHITE, ' ');
		//            }
		//        }
		//    }
		//    else
		//    {
		//        /* Only vertical */
		//        for (vert = 1; vert <= tile_height; vert++)
		//        {
		//            /* Queue dummy character */
		//            if (a & 0x80)
		//                Term_putch(x, y + vert, 255, -1);
		//            else
		//                Term_putch(x, y + vert, TERM_WHITE, ' ');
		//        }
		//    }
		//}


		/*
		 * Move to a location and, using an attr, add a string
		 */
		public static int putstr(int x, int y, int n, ConsoleColor a, string s) {
			int res;

			if(instance == null)
				return 0;

			/* Move first */
			if((res = gotoxy(x, y)) != 0)
				return (res);

			/* Then add the string */
			if((res = addstr(n, a, s)) != 0)
				return (res);

			/* Success */
			return (0);
		}



		/*
		 * Place cursor at (x,y), and clear the next "n" chars
		 */
		public static int erase(int x, int y, int n) {
			int i;

			int w = instance.wid;
			/* int h = instance.hgt; */

			int x1 = -1;
			int x2 = -1;

			ConsoleColor na = instance.attr_blank;
			char nc = instance.char_blank;

			ConsoleColor[] scr_aa;
			char[] scr_cc;

			ConsoleColor[] scr_taa;
			char[] scr_tcc;

			/* Place cursor */
			if(Term.gotoxy(x, y) != 0)
				return (-1);

			/* Force legal size */
			if(x + n > w)
				n = w - x;

			/* Fast access */
			scr_aa = instance.scr.a[y];
			scr_cc = instance.scr.c[y];

			scr_taa = instance.scr.ta[y];
			scr_tcc = instance.scr.tc[y];

			if((n > 0) && (scr_cc[x] == 255) && (scr_aa[x] == (ConsoleColor)255)) {
				x--;
				n++;
			}

			/* Scan every column */
			for(i = 0; i < n; i++, x++) {
				ConsoleColor oa = scr_aa[x];
				char oc = scr_cc[x];

				/* Hack -- Ignore "non-changes" */
				if((oa == na) && (oc == nc))
					continue;

				/* Save the "literal" information */
				scr_aa[x] = na;
				scr_cc[x] = nc;

				scr_taa[x] = 0;
				scr_tcc[x] = (char)0;

				/* Track minimum changed column */
				if(x1 < 0)
					x1 = x;

				/* Track maximum changed column */
				x2 = x;
			}

			/* Expand the "change area" as needed */
			if(x1 >= 0) {
				/* Check for new min/max row info */
				if(y < instance.y1)
					instance.y1 = (byte)y;
				if(y > instance.y2)
					instance.y2 = (byte)y;

				/* Check for new min/max col info in this row */
				if(x1 < instance.x1[y])
					instance.x1[y] = (byte)x1;
				if(x2 > instance.x2[y])
					instance.x2[y] = (byte)x2;
			}

			/* Success */
			return (0);
		}


		/*
		 * Clear the entire window, and move to the top left corner
		 *
		 * Note the use of the special "total_erase" code
		 */
		public static int clear() {
			int x, y;

			int w = instance.wid;
			int h = instance.hgt;

			ConsoleColor na = instance.attr_blank;
			char nc = instance.char_blank;

			/* Cursor usable */
			instance.scr.cu = false;

			/* Cursor to the top left */
			instance.scr.cx = instance.scr.cy = 0;

			/* Wipe each row */
			for(y = 0; y < h; y++) {
				ConsoleColor[] scr_aa = instance.scr.a[y];
				char[] scr_cc = instance.scr.c[y];
				ConsoleColor[] scr_taa = instance.scr.ta[y];
				char[] scr_tcc = instance.scr.tc[y];

				/* Wipe each column */
				for(x = 0; x < w; x++) {
					scr_aa[x] = na;
					scr_cc[x] = nc;

					scr_taa[x] = Utilities.num_to_attr(0);
					scr_tcc[x] = '\0';
				}

				/* This row has changed */
				instance.x1[y] = 0;
				instance.x2[y] = (byte)(w - 1);
			}

			/* Every row has changed */
			instance.y1 = 0;
			instance.y2 = (byte)(h - 1);

			/* Force "total erase" */
			instance.total_erase = true;

			/* Success */
			return (0);
		}





		/*
		 * Redraw (and refresh) the whole window.
		 */
		public static int redraw()
		{
		    /* Force "total erase" */
		    instance.total_erase = true;

		    /* Hack -- Refresh */
		    fresh();

		    /* Success */
		    return (0);
		}


		/*
		 * Redraw part of a window.
		 */
		public static int redraw_section(int x1, int y1, int x2, int y2)
		{
		    int i, j;
			char[] c_ptr; //char*

		    /* Bounds checking */
		    if (y2 >= instance.hgt) y2 = instance.hgt - 1;
		    if (x2 >= instance.wid) x2 = instance.wid - 1;
		    if (y1 < 0) y1 = 0;
		    if (x1 < 0) x1 = 0;


		    /* Set y limits */
		    instance.y1 = (byte)y1;
		    instance.y2 = (byte)y2;

		    /* Set the x limits */
		    for (i = instance.y1; i <= instance.y2; i++)
		    {
		        if ((x1 > 0) && (instance.old.a[i][x1] == (ConsoleColor)255))
		            x1--;

		        instance.x1[i] = (byte)x1;
		        instance.x2[i] = (byte)x2;

		        c_ptr = instance.old.c[i];

		        /* Clear the section so it is redrawn */
		        for (j = x1; j <= x2; j++)
		        {
		            /* Hack - set the old character to "none" */
		            c_ptr[j] = (char)0;
		        }
		    }

		    /* Hack -- Refresh */
		    fresh();

		    /* Success */
		    return (0);
		}





		///*** Access routines ***/
		/*
		 * Extract the cursor visibility
		 */
		public static bool get_cursor() {
			/* Extract visibility */
			return instance.scr.cv;
		}


		/*
		 * Extract the current window size
		 */
		public static int get_size(out int w, out int h) {
			w = instance != null ? instance.wid : 80;
			h = instance != null ? instance.hgt : 24;
			return 0;
		}


		/*
		 * Extract the current cursor location
		 */
		public static int locate(out int x, out int y) {
			/* Access the cursor */
			x = instance.scr.cx;
			y = instance.scr.cy;

			/* Warn about "useless" cursor */
			if(instance.scr.cu)
				return (1);

			/* Success */
			return (0);
		}


		/*
		 * At a given location, determine the "current" attr and char
		 * Note that this refers to what will be on the window after the
		 * next call to "Term_fresh()".  It may or may not already be there.
		 */
		public static int what(int x, int y, ref ConsoleColor a, ref char c) {
			int w = instance.wid;
			int h = instance.hgt;

			/* Verify location */
			if((x < 0) || (x >= w))
				return (-1);
			if((y < 0) || (y >= h))
				return (-1);

			/* Direct access */
			a = instance.scr.a[y][x];
			c = instance.scr.c[y][x];

			/* Success */
			return (0);
		}



		///*** Input routines ***/


		/*
		 * Flush and forget the input
		 */
		public static int flush() {
			if(instance == null)
				return 0;

			/* Hack -- Flush all events */
			xtra(TERM_XTRA.FLUSH, 0);

			/* Forget all keypresses */
			instance.key_head = instance.key_tail = 0;

			/* Success */
			return (0);
		}


		/* sketchy keylogging pt. 2 */
		static void log_keypress(ui_event e) {
			if(e.type != ui_event_type.EVT_KBRD)
				return;
			if(e.key.code == 0)
				return;

			keylog[log_i] = e.key;
			if(log_size < KEYLOG_SIZE)
				log_size++;
			log_i = (log_i + 1) % KEYLOG_SIZE;
		}


		/*
		 * Add a keypress to the "queue"
		 */
		public static int keypress(keycode_t k, byte mods) {
			/* Hack -- Refuse to enqueue non-keys */
			if(k == keycode_t.KC_NONE)
				return (-1);

			/* Store the char, advance the queue */
			instance.key_queue[instance.key_head].type = ui_event_type.EVT_KBRD;
			instance.key_queue[instance.key_head].key.code = k;
			instance.key_queue[instance.key_head].key.mods = mods;
			instance.key_head++;

			/* Circular queue, handle wrap */
			if(instance.key_head == instance.key_size)
				instance.key_head = 0;

			/* Success (unless overflow) */
			if(instance.key_head != instance.key_tail)
				return (0);

			/* Problem */
			return (1);
		}

		///*
		// * Add a mouse event to the "queue"
		// */
		//errr Term_mousepress(int x, int y, char button)
		//{
		//  /* Store the char, advance the queue */
		//  instance.key_queue[instance.key_head].type = EVT_MOUSE;
		//  instance.key_queue[instance.key_head].mouse.x = x;
		//  instance.key_queue[instance.key_head].mouse.y = y;
		//  instance.key_queue[instance.key_head].mouse.button = button;
		//  instance.key_head++;

		//  /* Circular queue, handle wrap */
		//  if (instance.key_head == instance.key_size) instance.key_head = 0;

		//  /* Success (unless overflow) */
		//  if (instance.key_head != instance.key_tail) return (0);

		//#if 0
		//  /* Hack -- Forget the oldest key */
		//  if (++instance.key_tail == instance.key_size) instance.key_tail = 0;
		//#endif

		//  /* Problem */
		//  return (1);
		//}


		///*
		// * Add a keypress to the FRONT of the "queue"
		// */
		//errr Term_key_push(int k)
		//{
		//    ui_event ke;

		//    if (!k) return (-1);

		//    ke.type = EVT_KBRD;
		//    ke.key.code = k;
		//    ke.key.mods = 0;

		//    return Term_event_push(&ke);
		//}

		public static int event_push(ui_event ke)
		{
		    /* Hack -- Refuse to enqueue non-keys */
		    if (ke == null) return (-1);

		    /* Hack -- Overflow may induce circular queue */
		    if (instance.key_tail == 0) instance.key_tail = instance.key_size;

		    /* Back up, Store the char */
		    /* Store the char, advance the queue */
		    instance.key_queue[--instance.key_tail] = ke;

		    /* Success (unless overflow) */
		    if (instance.key_head != instance.key_tail) return (0);

			//#if 0
			//    /* Hack -- Forget the oldest key */
			//    if (++instance.key_tail == instance.key_size) instance.key_tail = 0;
			//#endif

		    /* Problem */
		    return (1);
		}





		/*
		 * Check for a pending keypress on the key queue.
		 *
		 * Store the keypress, if any, in "ch", and return "0".
		 * Otherwise store "zero" in "ch", and return "1".
		 *
		 * Wait for a keypress if "wait" is true.
		 *
		 * Remove the keypress if "take" is true.
		 */
		public static int inkey(out ui_event ch, bool wait, bool take) {
			/* Assume no key */
			ch = new ui_event();

			/* Hack -- get bored */
			if(!instance.never_bored) {
				/* Process random events */
				Term.xtra(TERM_XTRA.BORED, 0);
			}

			/* Wait */
			if(wait) {
				/* Process pending events while necessary */
				while(instance.key_head == instance.key_tail) {
					/* Process events (wait for one) */
					Term.xtra(TERM_XTRA.EVENT, 1);
				}
			}

			/* Do not Wait */
			else {
				/* Process pending events if necessary */
				if(instance.key_head == instance.key_tail) {
					/* Process events (do not wait) */
					Term.xtra(TERM_XTRA.EVENT, 0);
				}
			}

			/* No keys are ready */
			if(instance.key_head == instance.key_tail)
				return (1);

			/* Extract the next keypress */
			ch = instance.key_queue[instance.key_tail];

			/* sketchy key loggin */
			log_keypress(ch);

			/* If requested, advance the queue, wrap around if necessary */
			if(take && (++instance.key_tail == instance.key_size))
				instance.key_tail = 0;

			/* Success */
			return (0);
		}



		///*** Extra routines ***/

		/*
		 * Save the "requested" screen into the "memorized" screen
		 *
		 * Every "Term_save()" should match exactly one "Term_load()"
		 */
		public static int save()
		{
		    int w = instance.wid;
		    int h = instance.hgt;

			/* Allocate window */
		    Term_Win mem = new Term_Win();
			
		    /* Initialize window */
		    mem.init(w, h);

		    /* Grab */
			mem.copy(instance.scr, w, h);

		    /* Front of the queue */
		    mem.next = instance.mem;
		    instance.mem = mem;

		    /* One more saved */
		    instance.saved++;

		    /* Success */
		    return (0);
		}


		/*
		 * Restore the "requested" contents (see above).
		 *
		 * Every "Term_save()" should match exactly one "Term_load()"
		 */
		public static int load()
		{
		    int y;

		    int w = instance.wid;
		    int h = instance.hgt;

		    Term_Win tmp;

		    /* Pop off window from the list */
		    if (instance.mem != null)
		    {
		        /* Save pointer to old mem */
		        tmp = instance.mem;

		        /* Forget it */
		        instance.mem = instance.mem.next;

		        /* Load */
				instance.scr.copy(tmp, w, h);

		        /* Free the old window */
				tmp.nuke();

		        /* Kill it */
		        //FREE(tmp);
		    }

		    /* Assume change */
		    for (y = 0; y < h; y++)
		    {
		        /* Assume change */
		        instance.x1[y] = 0;
		        instance.x2[y] = (byte)(w - 1);
		    }

		    /* Assume change */
		    instance.y1 = 0;
		    instance.y2 = (byte)(h - 1);

		    /* One less saved */
		    instance.saved--;

		    /* Success */
		    return (0);
		}



		///*
		// * React to a new physical window size.
		// */
		//errr Term_resize(int w, int h)
		//{
		//    int i;

		//    int wid, hgt;

		//    byte *hold_x1;
		//    byte *hold_x2;

		//    term_win *hold_old;
		//    term_win *hold_scr;
		//    term_win *hold_mem;
		//    term_win *hold_tmp;

		//    ui_event evt = EVENT_EMPTY;
		//    evt.type = EVT_RESIZE;


		//    /* Resizing is forbidden */
		//    if (instance.fixed_shape) return (-1);


		//    /* Ignore illegal changes */
		//    if ((w < 1) || (h < 1)) return (-1);


		//    /* Ignore non-changes */
		//    if ((instance.wid == w) && (instance.hgt == h)) return (1);


		//    /* Minimum dimensions */
		//    wid = MIN(instance.wid, w);
		//    hgt = MIN(instance.hgt, h);

		//    /* Save scanners */
		//    hold_x1 = instance.x1;
		//    hold_x2 = instance.x2;

		//    /* Save old window */
		//    hold_old = instance.old;

		//    /* Save old window */
		//    hold_scr = instance.scr;

		//    /* Save old window */
		//    hold_mem = instance.mem;

		//    /* Save old window */
		//    hold_tmp = instance.tmp;

		//    /* Create new scanners */
		//    instance.x1 = C_ZNEW(h, byte);
		//    instance.x2 = C_ZNEW(h, byte);

		//    /* Create new window */
		//    instance.old = ZNEW(term_win);

		//    /* Initialize new window */
		//    term_win_init(instance.old, w, h);

		//    /* Save the contents */
		//    term_win_copy(instance.old, hold_old, wid, hgt);

		//    /* Create new window */
		//    instance.scr = ZNEW(term_win);

		//    /* Initialize new window */
		//    term_win_init(instance.scr, w, h);

		//    /* Save the contents */
		//    term_win_copy(instance.scr, hold_scr, wid, hgt);

		//    /* If needed */
		//    if (hold_mem)
		//    {
		//        /* Create new window */
		//        instance.mem = ZNEW(term_win);

		//        /* Initialize new window */
		//        term_win_init(instance.mem, w, h);

		//        /* Save the contents */
		//        term_win_copy(instance.mem, hold_mem, wid, hgt);
		//    }

		//    /* If needed */
		//    if (hold_tmp)
		//    {
		//        /* Create new window */
		//        instance.tmp = ZNEW(term_win);

		//        /* Initialize new window */
		//        term_win_init(instance.tmp, w, h);

		//        /* Save the contents */
		//        term_win_copy(instance.tmp, hold_tmp, wid, hgt);
		//    }

		//    /* Free some arrays */
		//    FREE(hold_x1);
		//    FREE(hold_x2);

		//    /* Nuke */
		//    term_win_nuke(hold_old);

		//    /* Kill */
		//    FREE(hold_old);

		//    /* Illegal cursor */
		//    if (instance.old.cx >= w) instance.old.cu = 1;
		//    if (instance.old.cy >= h) instance.old.cu = 1;

		//    /* Nuke */
		//    term_win_nuke(hold_scr);

		//    /* Kill */
		//    FREE(hold_scr);

		//    /* Illegal cursor */
		//    if (instance.scr.cx >= w) instance.scr.cu = 1;
		//    if (instance.scr.cy >= h) instance.scr.cu = 1;

		//    /* If needed */
		//    if (hold_mem)
		//    {
		//        /* Nuke */
		//        term_win_nuke(hold_mem);

		//        /* Kill */
		//        FREE(hold_mem);

		//        /* Illegal cursor */
		//        if (instance.mem.cx >= w) instance.mem.cu = 1;
		//        if (instance.mem.cy >= h) instance.mem.cu = 1;
		//    }

		//    /* If needed */
		//    if (hold_tmp)
		//    {
		//        /* Nuke */
		//        term_win_nuke(hold_tmp);

		//        /* Kill */
		//        FREE(hold_tmp);

		//        /* Illegal cursor */
		//        if (instance.tmp.cx >= w) instance.tmp.cu = 1;
		//        if (instance.tmp.cy >= h) instance.tmp.cu = 1;
		//    }

		//    /* Save new size */
		//    instance.wid = w;
		//    instance.hgt = h;

		//    /* Force "total erase" */
		//    instance.total_erase = true;

		//    /* Assume change */
		//    for (i = 0; i < h; i++)
		//    {
		//        /* Assume change */
		//        instance.x1[i] = 0;
		//        instance.x2[i] = w - 1;
		//    }

		//    /* Assume change */
		//    instance.y1 = 0;
		//    instance.y2 = h - 1;

		//    /* Push a resize event onto the stack */
		//    Term_event_push(&evt);

		//    /* Success */
		//    return (0);
		//}



		///*
		// * Activate a new Term (and deactivate the current Term)
		// *
		// * This function is extremely important, and also somewhat bizarre.
		// * It is the only function that should "modify" the value of "Term".
		// *
		// * To "create" a valid "term", one should do "term_init(t)", then
		// * set the various flags and hooks, and then do "Term_activate(t)".
		// */
		//used to return errr
		public int activate() {
			/* Hack -- already done */
			if(instance == this)
				return (1);

			/* Deactivate the old Term */
			if(instance != null)
				Term.xtra(TERM_XTRA.LEVEL, 0);

			/* Hack -- Call the special "init" hook */
			if(!active_flag) {
				/* Call the "init" hook */
				if(init_hook != null) {
					init_hook(this);
				}

				/* Remember */
				active_flag = true;

				/* Assume mapped */
				mapped_flag = true;
			}

			/* Remember the Term */
			instance = this;

			/* Activate the new Term */
			if(instance != null)
				Term.xtra(TERM_XTRA.LEVEL, 1);

			/* Success */
			return (0);
		}



		/*
		 * Nuke a term
		 */
		public int nuke() {
			throw new NotImplementedException();
			///* Hack -- Call the special "nuke" hook */
			//if (t.active_flag)
			//{
			//    /* Call the "nuke" hook */
			//    if (t.nuke_hook) (*t.nuke_hook)(t);

			//    /* Remember */
			//    t.active_flag = false;

			//    /* Assume not mapped */
			//    t.mapped_flag = false;
			//}


			///* Nuke "displayed" */
			//term_win_nuke(t.old);

			///* Kill "displayed" */
			//FREE(t.old);

			///* Nuke "requested" */
			//term_win_nuke(t.scr);

			///* Kill "requested" */
			//FREE(t.scr);

			///* If needed */
			//if (t.mem)
			//{
			//    /* Nuke "memorized" */
			//    term_win_nuke(t.mem);

			//    /* Kill "memorized" */
			//    FREE(t.mem);
			//}

			///* If needed */
			//if (t.tmp)
			//{
			//    /* Nuke "temporary" */
			//    term_win_nuke(t.tmp);

			//    /* Kill "temporary" */
			//    FREE(t.tmp);
			//}

			///* Free some arrays */
			//FREE(t.x1);
			//FREE(t.x2);

			///* Free the input queue */
			//FREE(t.key_queue);

			///* Success */
			//return (0);
		}


		/*
		 * Initialize a term, using a window of the given size.
		 * Also prepare the "input queue" for "k" keypresses
		 * By default, the cursor starts out "invisible"
		 * By default, we "erase" using "black spaces"
		 */
		public int init(int w, int h, int k) {
			/* Wipe it */
			//(void)WIPE(t, term);
			//let's just assume we're good...

			/* Prepare the input queue */
			key_head = key_tail = 0;

			/* Determine the input queue size */
			key_size = (ushort)k;

			/* Allocate the input queue */
			key_queue = new ui_event[key_size];

			/* Save the size */
			wid = (byte)w;
			hgt = (byte)h;

			/* Allocate change arrays */
			x1 = new byte[h];
			x2 = new byte[h];


			/* Allocate "displayed" */
			old = new Term_Win();

			/* Initialize "displayed" */
			old.init(w, h);


			/* Allocate "requested" */
			scr = new Term_Win();

			/* Initialize "requested" */
			scr.init(w, h);


			/* Assume change */
			for(int y = 0; y < h; y++) {
				/* Assume change */
				x1[y] = 0;
				x2[y] = (byte)(w - 1);
			}

			/* Assume change */
			y1 = 0;
			y2 = (byte)(h - 1);

			/* Force "total erase" */
			total_erase = true;


			/* Default "blank" */
			attr_blank = 0;
			char_blank = ' ';

			/* No saves yet */
			saved = 0;

			for(int i = 0; i < key_queue.Length; i++) {
				key_queue[i] = new ui_event();
				key_queue[i].key = new keypress();
			}
			/* Success */
			return (0);
		}
	}
}
