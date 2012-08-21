/*
 * File: dungeon.c
 * Purpose: The game core bits, shared across platforms.
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
#include "cave.h"
#include "cmds.h"
#include "files.h"
#include "game-event.h"
#include "generate.h"
#include "init.h"
#include "monster/monster.h"
#include "monster/mon-make.h"
#include "monster/mon-spell.h"
#include "monster/mon-util.h"
#include "object/tvalsval.h"
#include "prefs.h"
#include "savefile.h"
#include "spells.h"
#include "target.h"


static void play_ambient_sound(void)
{
	/* Town sound */
	if (p_ptr.depth == 0) 
	{
		/* Hack - is it daytime or nighttime? */
		if (turn % (10L * TOWN_DAWN) < TOWN_DAWN / 2)
		{
			/* It's day. */
			sound(MSG_AMBIENT_DAY);
		} 
		else 
		{
			/* It's night. */
			sound(MSG_AMBIENT_NITE);
		}
		
	}

	/* Dungeon level 1-20 */
	else if (p_ptr.depth <= 20) 
	{
		sound(MSG_AMBIENT_DNG1);
	}

	/* Dungeon level 21-40 */
	else if (p_ptr.depth <= 40) 
	{
		sound(MSG_AMBIENT_DNG2);
	}

	/* Dungeon level 41-60 */
	else if (p_ptr.depth <= 60) 
	{
		sound(MSG_AMBIENT_DNG3);
	}

	/* Dungeon level 61-80 */
	else if (p_ptr.depth <= 80)
	{
		sound(MSG_AMBIENT_DNG4);
	}

	/* Dungeon level 80- */
	else
	{
		sound(MSG_AMBIENT_DNG5);
	}
}





