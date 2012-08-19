#ifndef INCLUDED_TYPES_H
#define INCLUDED_TYPES_H

/*
 * This file contains various defined types used by the game.
 *
 * TODO: Most of these should be elsewhere, in their own header files.
 * For example, the object structs should be in object.h.
 *
 * Be careful when creating data structures; most of these are designed
 * to be serialised to file, so be careful to use exact-size data types
 * (like u32b and s32b) and not just "int"s.
 */

#include "z-term.h"

/**** Available Types ****/

/** An array of 256 bytes */
typedef byte byte_256[256];

/** An array of DUNGEON_WID bytes */
typedef byte byte_wid[DUNGEON_WID];

/** An array of DUNGEON_WID s16b's */
typedef s16b s16b_wid[DUNGEON_WID];



/**** Available Structs ****/


/*
 * A game color.
 */
struct color_type
{
	char index_char;            /* Character index:  'r' = red, etc. */
	char name[32];              /* Color name */
	byte color_translate[MAX_ATTR];       /* Index for various in-game translations */
};s


#endif /* !INCLUDED_TYPES_H */
