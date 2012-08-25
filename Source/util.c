/*
 * File: util.c
 * Purpose: Gamma correction, some high-level UI functions, inkey()
 *
 * Copyright (c) 1997 Ben Harrison, James E. Wilson, Robert A. Koeneke
 *
 * This work is free software; you can redistribute it and/or modify it
 * under the terms of either:
 *
 * a) the GNU General Public License as published by the Free Software
 *    Foundation, version 2, or
 *
 * b) the "Angband licence":
 *    This software may be copied and distributed for educational, research,
 *    and not for profit purposes provided that this copyright and statement
 *    are included in all such copies.  Other copyrights may also apply.
 */

#include "angband.h"
#include "button.h"
#include "cmds.h"
#include "game-event.h"
#include "randname.h"

#ifdef ALLOW_BORG

/*
 * Mega-Hack -- special "inkey_hack" hook.  XXX XXX XXX
 *
 * This special function hook allows the "Borg" (see elsewhere) to take
 * control of the "inkey()" function, and substitute in fake keypresses.
 */
struct keypress (*inkey_hack)(int flush_first) = null;

#endif /* ALLOW_BORG */

/*
 * Hack -- Make a (relevant?) sound
 */
void sound(int val)
{
	/* No sound */
	if (!OPT(use_sound) || !sound_hook) return;

	sound_hook(val);
}


/*
 * Write text to the given file and apply line-wrapping.
 *
 * Hook function for text_out(). Make sure that text_out_file points
 * to an open text-file.
 *
 * Long lines will be wrapped at text_out_wrap, or at column 75 if that
 * is not set; or at a newline character.  Note that punctuation can
 * sometimes be placed one column beyond the wrap limit.
 *
 * You must be careful to end all file output with a newline character
 * to "flush" the stored line position.
 */
void text_out_to_file(byte a, const char *str)
{
	const char *s;
	char buf[1024];

	/* Current position on the line */
	static int pos = 0;

	/* Wrap width */
	int wrap = (text_out_wrap ? text_out_wrap : 75);

	/* We use either ascii or system-specific encoding */
 	int encoding = OPT(xchars_to_file) ? SYSTEM_SPECIFIC : ASCII;

	/* Unused parameter */
	(void)a;

	/* Copy to a rewriteable string */
 	my_strcpy(buf, str, 1024);

 	/* Translate it to 7-bit ASCII or system-specific format */
 	xstr_trans(buf, encoding);

	/* Current location within "buf" */
 	s = buf;

	/* Process the string */
	while (*s)
	{
		char ch;
		int n = 0;
		int len = wrap - pos;
		int l_space = -1;

		/* If we are at the start of the line... */
		if (pos == 0)
		{
			int i;

			/* Output the indent */
			for (i = 0; i < text_out_indent; i++)
			{
				file_writec(text_out_file, ' ');
				pos++;
			}
		}

		/* Find length of line up to next newline or end-of-string */
		while ((n < len) && !((s[n] == '\n') || (s[n] == '\0')))
		{
			/* Mark the most recent space in the string */
			if (s[n] == ' ') l_space = n;

			/* Increment */
			n++;
		}

		/* If we have encountered no spaces */
		if ((l_space == -1) && (n == len))
		{
			/* If we are at the start of a new line */
			if (pos == text_out_indent)
			{
				len = n;
			}
			/* HACK - Output punctuation at the end of the line */
			else if ((s[0] == ' ') || (s[0] == ',') || (s[0] == '.'))
			{
				len = 1;
			}
			else
			{
				/* Begin a new line */
				file_writec(text_out_file, '\n');

				/* Reset */
				pos = 0;

				continue;
			}
		}
		else
		{
			/* Wrap at the newline */
			if ((s[n] == '\n') || (s[n] == '\0')) len = n;

			/* Wrap at the last space */
			else len = l_space;
		}

		/* Write that line to file */
		for (n = 0; n < len; n++)
		{
			/* Ensure the character is printable */
			ch = (my_isprint((unsigned char) s[n]) ? s[n] : ' ');

			/* Write out the character */
			file_writec(text_out_file, ch);

			/* Increment */
			pos++;
		}

		/* Move 's' past the stuff we've written */
		s += len;

		/* If we are at the end of the string, end */
		if (*s == '\0') return;

		/* Skip newlines */
		if (*s == '\n') s++;

		/* Begin a new line */
		file_writec(text_out_file, '\n');

		/* Reset */
		pos = 0;

		/* Skip whitespace */
		while (*s == ' ') s++;
	}

	/* We are done */
	return;
}

/*
 * Given a "formatted" chunk of text (i.e. one including tags like {red}{/})
 * in 'source', with starting point 'init', this finds the next section of
 * text and any tag that goes with it, return true if it finds something to 
 * print.
 * 
 * If it returns true, then it also fills 'text' with a pointer to the start
 * of the next printable section of text, and 'len' with the length of that 
 * text, and 'end' with a pointer to the start of the next section.  This
 * may differ from "text + len" because of the presence of tags.  If a tag
 * applies to the section of text, it returns a pointer to the start of that
 * tag in 'tag' and the length in 'taglen'.  Otherwise, 'tag' is filled with
 * null.
 *
 * See text_out_e for an example of its use.
 */
static bool next_section(const char *source, size_t init, const char **text, size_t *len, const char **tag, size_t *taglen, const char **end)
{
	const char *next;	

	*tag = null;
	*text = source + init;
	if (*text[0] == '\0') return false;

	next = strchr(*text, '{');
	while (next)
	{
		const char *s = next + 1;

		while (*s && isalpha((unsigned char) *s)) s++;

		/* Woo!  valid opening tag thing */
		if (*s == '}')
		{
			const char *close = strstr(s, "{/}");

			/* There's a closing thing, so it's valid. */
			if (close)
			{
				/* If this tag is at the start of the fragment */
				if (next == *text)
				{
					*tag = *text + 1;
					*taglen = s - *text - 1;
					*text = s + 1;
					*len = close - *text;
					*end = close + 3;
					return true;
				}
				/* Otherwise return the chunk up to this */
				else
				{
					*len = next - *text;
					*end = *text + *len;
					return true;
				}
			}
			/* No closing thing, therefore all one lump of text. */
			else
			{
				*len = strlen(*text);
				*end = *text + *len;
				return true;
			}
		}
		/* End of the string, that's fine. */
		else if (*s == '\0')
		{
				*len = strlen(*text);
				*end = *text + *len;
				return true;
		}
		/* An invalid tag, skip it. */
		else
		{
			next = next + 1;
		}

		next = strchr(next, '{');
	}

	/* Default to the rest of the string */
	*len = strlen(*text);
	*end = *text + *len;

	return true;
}

/*
 * Prompt for a string from the user.
 *
 * The "prompt" should take the form "Prompt: ".
 *
 * See "askfor_aux" for some notes about "buf" and "len", and about
 * the return value of this function.
 */
bool get_string(const char *prompt, char *buf, size_t len)
{
	bool res;

	/* Paranoia XXX XXX XXX */
	message_flush();

	/* Display prompt */
	prt(prompt, 0, 0);

	/* Ask the user for a string */
	res = askfor_aux(buf, len, null);

	/* Translate it to 8-bit (Latin-1) */
 	xstr_trans(buf, LATIN1);

	/* Clear prompt */
	prt("", 0, 0);

	/* Result */
	return (res);
}

/* TODO: refactor get_check() in terms of get_char() */
/*
 * Ask the user to respond with a character. Options is a constant string,
 * e.g. "yns"; len is the length of the constant string, and fallback should
 * be the default answer if the user hits escape or an invalid key.
 *
 * Example: get_char("Study? ", "yns", 3, 'n')
 *     This prompts "Study? [yns]" and defaults to 'n'.
 *
 */
char get_char(const char *prompt, const char *options, size_t len, char fallback)
{
	size_t i;
	struct keypress key;
	char button[4], buf[80];
	bool repeat = false;
  
	/* Paranoia XXX XXX XXX */
	message_flush();

	/* Hack -- Build a "useful" prompt */
	strnfmt(buf, 78, "%.70s[%s] ", prompt, options);

	/* Hack - kill the repeat button */
	if (button_kill('n')) repeat = true;
	
	/* Make some buttons */
	for (i = 0; i < len; i++)
	{
		strnfmt(button, 4, "[%c]", options[i]);
		button_add(button, options[i]);
	}
	redraw_stuff(p_ptr);
  
	/* Prompt for it */
	prt(buf, 0, 0);

	/* Get an acceptable answer */
	key = inkey();

	/* Lowercase answer if necessary */
	if (key.code >= 'A' && key.code <= 'Z') key.code += 32;

	/* See if key is in our options string */
	if (!strchr(options, (char)key.code))
		key.code = fallback;

	/* Kill the buttons */
	for (i = 0; i < len; i++) button_kill(options[i]);

	/* Hack - restore the repeat button */
	if (repeat) button_add("[Rpt]", 'n');
	redraw_stuff(p_ptr);
  
	/* Erase the prompt */
	prt("", 0, 0);

	/* Success */
	return key.code;
}


/**
 * Text-native way of getting a filename.
 */
static bool get_file_text(const char *suggested_name, char *path, size_t len)
{
	char buf[160];

	/* Get filename */
	my_strcpy(buf, suggested_name, sizeof buf);
	if (!get_string("File name: ", buf, sizeof buf)) return false;

	/* Make sure it's actually a filename */
	if (buf[0] == '\0' || buf[0] == ' ') return false;

	/* Build the path */
	path_build(path, len, ANGBAND_DIR_USER, buf);

	/* Check if it already exists */
	if (file_exists(buf))
	{
		char buf2[160];
		strnfmt(buf2, sizeof(buf2), "Replace existing file %s?", buf);

		if (get_check(buf2) == false)
			return false;
	}

	return true;
}




/**
 * Get a pathname to save a file to, given the suggested name.  Returns the
 * result in "path".
 */
bool (*get_file)(const char *suggested_name, char *path, size_t len) = get_file_text;




/*
 * Prompts for a keypress
 *
 * The "prompt" should take the form "Command: "
 *
 * Returns true unless the character is "Escape"
 */
bool get_com(const char *prompt, struct keypress *command)
{
	ui_event ke;
	bool result;

	result = get_com_ex(prompt, &ke);
	*command = ke.key;

	return result;
}


bool get_com_ex(const char *prompt, ui_event *command)
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
	*command = ke;

	/* Done */
	if (ke.type == EVT_KBRD && ke.key.code == ESCAPE)
		return false;
	return true;
}


/*
 * Check a char for "vowel-hood"
 */
bool is_a_vowel(int ch)
{
	switch (tolower((unsigned char) ch))
	{
		case 'a':
		case 'e':
		case 'i':
		case 'o':
		case 'u':
		{
			return (true);
		}
	}

	return (false);
}


/*
 * Accept a color index character; if legal, return the color.  -LM-
 *
 * Unlike Sangband, we don't translate these colours here.
 */
/* XXX: having color_{char,text}_to_attr() separately is moronic. */
int color_char_to_attr(char c)
{
	int a;

	/* Is negative -- spit it right back out */
	if (c < 0) return (c);

	/* Is a space or '\0' -- return black */
	if (c == '\0' || c == ' ') return (TERM_DARK);

	/* Search the color table */
	for (a = 0; a < BASIC_COLORS; a++)
	{
		/* Look for the index */
		if (color_table[a].index_char == c) break;
	}

	/* If we don't find the color, we assume white */
	if (a == BASIC_COLORS) return (TERM_WHITE);

	/* Return the color */
	return (a);
}


/*
 * Extract a textual representation of an attribute
 */
const char *attr_to_text(byte a)
{
	if (a < BASIC_COLORS)
		return (color_table[a].name);
	else
		return ("Icky");
}


#ifdef SUPPORT_GAMMA

/*
 * XXX XXX XXX Important note about "colors" XXX XXX XXX
 *
 * The "TERM_*" color definitions list the "composition" of each
 * "Angband color" in terms of "quarters" of each of the three color
 * components (Red, Green, Blue), for example, TERM_UMBER is defined
 * as 2/4 Red, 1/4 Green, 0/4 Blue.
 *
 * These values are NOT gamma-corrected.  On most machines (with the
 * Macintosh being an important exception), you must "gamma-correct"
 * the given values, that is, "correct for the intrinsic non-linearity
 * of the phosphor", by converting the given intensity levels based
 * on the "gamma" of the target screen, which is usually 1.7 (or 1.5).
 *
 * The actual formula for conversion is unknown to me at this time,
 * but you can use the table below for the most common gamma values.
 *
 * So, on most machines, simply convert the values based on the "gamma"
 * of the target screen, which is usually in the range 1.5 to 1.7, and
 * usually is closest to 1.7.  The converted value for each of the five
 * different "quarter" values is given below:
 *
 *  Given     Gamma 1.0       Gamma 1.5       Gamma 1.7     Hex 1.7
 *  -----       ----            ----            ----          ---
 *   0/4        0.00            0.00            0.00          #00
 *   1/4        0.25            0.27            0.28          #47
 *   2/4        0.50            0.55            0.56          #8f
 *   3/4        0.75            0.82            0.84          #d7
 *   4/4        1.00            1.00            1.00          #ff
 */

/* Table of gamma values */
byte gamma_table[256];

/* Table of ln(x / 256) * 256 for x going from 0 . 255 */
static const s16b gamma_helper[256] =
{
	0, -1420, -1242, -1138, -1065, -1007, -961, -921, -887, -857, -830,
	-806, -783, -762, -744, -726, -710, -694, -679, -666, -652, -640,
	-628, -617, -606, -596, -586, -576, -567, -577, -549, -541, -532,
	-525, -517, -509, -502, -495, -488, -482, -475, -469, -463, -457,
	-451, -455, -439, -434, -429, -423, -418, -413, -408, -403, -398,
	-394, -389, -385, -380, -376, -371, -367, -363, -359, -355, -351,
	-347, -343, -339, -336, -332, -328, -325, -321, -318, -314, -311,
	-308, -304, -301, -298, -295, -291, -288, -285, -282, -279, -276,
	-273, -271, -268, -265, -262, -259, -257, -254, -251, -248, -246,
	-243, -241, -238, -236, -233, -231, -228, -226, -223, -221, -219,
	-216, -214, -212, -209, -207, -205, -203, -200, -198, -196, -194,
	-192, -190, -188, -186, -184, -182, -180, -178, -176, -174, -172,
	-170, -168, -166, -164, -162, -160, -158, -156, -155, -153, -151,
	-149, -147, -146, -144, -142, -140, -139, -137, -135, -134, -132,
	-130, -128, -127, -125, -124, -122, -120, -119, -117, -116, -114,
	-112, -111, -109, -108, -106, -105, -103, -102, -100, -99, -97, -96,
	-95, -93, -92, -90, -89, -87, -86, -85, -83, -82, -80, -79, -78,
	-76, -75, -74, -72, -71, -70, -68, -67, -66, -65, -63, -62, -61,
	-59, -58, -57, -56, -54, -53, -52, -51, -50, -48, -47, -46, -45,
	-44, -42, -41, -40, -39, -38, -37, -35, -34, -33, -32, -31, -30,
	-29, -27, -26, -25, -24, -23, -22, -21, -20, -19, -18, -17, -16,
	-14, -13, -12, -11, -10, -9, -8, -7, -6, -5, -4, -3, -2, -1
};


/*
 * Build the gamma table so that floating point isn't needed.
 *
 * Note gamma goes from 0.256.  The old value of 100 is now 128.
 */
void build_gamma_table(int gamma)
{
	int i, n;

	/*
	 * value is the current sum.
	 * diff is the new term to add to the series.
	 */
	long value, diff;

	/* Hack - convergence is bad in these cases. */
	gamma_table[0] = 0;
	gamma_table[255] = 255;

	for (i = 1; i < 255; i++)
	{
		/*
		 * Initialise the Taylor series
		 *
		 * value and diff have been scaled by 256
		 */
		n = 1;
		value = 256L * 256L;
		diff = ((long)gamma_helper[i]) * (gamma - 256);

		while (diff)
		{
			value += diff;
			n++;

			/*
			 * Use the following identiy to calculate the gamma table.
			 * exp(x) = 1 + x + x^2/2 + x^3/(2*3) + x^4/(2*3*4) +...
			 *
			 * n is the current term number.
			 *
			 * The gamma_helper array contains a table of
			 * ln(x/256) * 256
			 * This is used because a^b = exp(b*ln(a))
			 *
			 * In this case:
			 * a is i / 256
			 * b is gamma.
			 *
			 * Note that everything is scaled by 256 for accuracy,
			 * plus another factor of 256 for the final result to
			 * be from 0-255.  Thus gamma_helper[] * gamma must be
			 * divided by 256*256 each itteration, to get back to
			 * the original power series.
			 */
			diff = (((diff / 256) * gamma_helper[i]) * (gamma - 256)) / (256 * n);
		}

		/*
		 * Store the value in the table so that the
		 * floating point pow function isn't needed.
		 */
		gamma_table[i] = ((long)(value / 256) * i) / 256;
	}
}

#endif /* SUPPORT_GAMMA */
