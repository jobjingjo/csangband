/*
 * File: mon-msg.c
 * Purpose: Monster message code.
 *
 * Copyright (c) 1997-2007 Ben Harrison, James E. Wilson, Robert A. Koeneke
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
#include "monster/mon-msg.h"
#include "monster/mon-util.h"

/*
 * The null-terminated array of string actions used to format stacked messages.
 * Singular and plural modifiers are encoded in the same string. Example:
 * "[is|are] hurt" is expanded to "is hurt" if you request the singular form.
 * The string is expanded to "are hurt" if the plural form is requested.
 * The singular and plural parts are optional. Example:
 * "rear[s] up in anger" only includes a modifier for the singular form.
 * Any of these strings can start with "~", in which case we consider that
 * string as a whole message, not as a part of a larger message. This
 * is useful to display Moria-like death messages.
 */
static const char *msg_repository[MAX_MON_MSG + 1] =
{
	/* Dummy action */
	"[is|are] hurt.",    		/* MON_MSG_NONE */

	/* From project_m */
	"die[s].",   				/* MON_MSG_DIE  */
	"[is|are] destroyed.",		/* MON_MSG_DESTROYED */
	"resist[s] a lot.",			/* MON_MSG_RESIST_A_LOT */
	"[is|are] hit hard.",		/* MON_MSG_HIT_HARD */
	"resist[s].",				/* MON_MSG_RESIST */
	"[is|are] immune.",			/* MON_MSG_IMMUNE */
	"resist[s] somewhat.",		/* MON_MSG_RESIST_SOMEWHAT */
	"[is|are] unaffected!",		/* MON_MSG_UNAFFECTED */
	"spawn[s]!",				/* MON_MSG_SPAWN */
	"look[s] healthier.",		/* MON_MSG_HEALTHIER */
	"fall[s] asleep!",			/* MON_MSG_FALL_ASLEEP */
	"wake[s] up.",				/* MON_MSG_WAKES_UP */
	"cringe[s] from the light!",/* MON_MSG_CRINGE_LIGHT */
	"shrivel[s] away in the light!",	/* MON_MSG_SHRIVEL_LIGHT */
	"lose[s] some skin!",		/* MON_MSG_LOSE_SKIN */
	"dissolve[s]!",				/* MON_MSG_DISSOLVE */
	"catch[es] fire!",			/* MON_MSG_CATCH_FIRE */
	"[is|are] badly frozen.", 	 /* MON_MSG_BADLY_FROZEN */
	"shudder[s].",				/* MON_MSG_SHUDDER */
	"change[s]!",				/* MON_MSG_CHANGE */
	"disappear[s]!",			/* MON_MSG_DISAPPEAR */
	"[is|are] even more stunned.",		/* MON_MSG_MORE_DAZED */
	"[is|are] stunned.",		/* MON_MSG_DAZED */
	"[is|are] no longer stunned.",	/* MON_MSG_NOT_DAZED */
	"look[s] more confused.",	/* MON_MSG_MORE_CONFUSED */
	"look[s] confused.",		/* MON_MSG_CONFUSED */
	"[is|are] no longer confused.",/* MON_MSG_NOT_CONFUSED */
	"look[s] more slowed.",		/* MON_MSG_MORE_SLOWED */
	"look[s] slowed.",			/* MON_MSG_SLOWED */
	"speed[s] up.",				/* MON_MSG_NOT_SLOWED */
	"look[s] even faster!",		/* MON_MSG_MORE_HASTED */
	"start[s|] moving faster.",	/* MON_MSG_HASTED */
	"slows down.",				/* MON_MSG_NOT_HASTED */
	"look[s] more terrified!",	/* MON_MSG_MORE_AFRAID */
	"flee[s] in terror!",		/* MON_MSG_FLEE_IN_TERROR */
	"[is|are] no longer afraid.",/* MON_MSG_NOT_AFRAID */
	"~You hear [a|several] scream[|s] of agony!",/* MON_MSG_MORIA_DEATH */
	"disintegrates!",		/* MON_MSG_DISENTEGRATES */
	"freez[es] and shatter[s]",  /* MON_MSG_FREEZE_SHATTER */
	"lose[s] some mana!",		/* MON_MSG_MANA_DRAIN */
	"looks briefly puzzled.",	/* MON_MSG_BRIEF_PUZZLE */
	"maintain[s] the same shape.", /* MON_MSG_MAINTAIN_SHAPE */

	/* From message_pain */
	"[is|are] unharmed.",		/* MON_MSG_UNHARMED  */
	
	/* Dummy messages for monster pain - we use edit file info instead. */
	"",							/* MON_MSG_95 */
	"",							/* MON_MSG_75 */
	"",							/* MON_MSG_50 */
	"",							/* MON_MSG_35 */
	"",							/* MON_MSG_20 */
	"",							/* MON_MSG_10 */
	"",							/* MON_MSG_0 */

	null						/* MAX_MON_MSG */
};


/*
 * Dump a message describing a monster's reaction to damage
 */
void message_pain(int m_idx, int dam)
{
	long oldhp, newhp, tmp;
	int percentage;

	monster_type *m_ptr = cave_monster(cave, m_idx);
	
	int msg_code = MON_MSG_UNHARMED;
	char m_name[80];

	/* Get the monster name */
	monster_desc(m_name, sizeof(m_name), m_ptr, 0);

	/* Notice non-damage */
	if (dam == 0)
	{
		add_monster_message(m_name, m_idx, msg_code, false);
		return;
	}

	/* Note -- subtle fix -CFT */
	newhp = (long)(m_ptr.hp);
	oldhp = newhp + (long)(dam);
	tmp = (newhp * 100L) / oldhp;
	percentage = (int)(tmp);
	
	if (percentage > 95)
	   msg_code = MON_MSG_95;
	else if (percentage > 75)
	   msg_code = MON_MSG_75;
	else if (percentage > 50)
	   msg_code = MON_MSG_50;
	else if (percentage > 35)
	   msg_code = MON_MSG_35;
	else if (percentage > 20)
	   msg_code = MON_MSG_20;
	else if (percentage > 10)
	   msg_code = MON_MSG_10;
	else
	   msg_code = MON_MSG_0;
	
   add_monster_message(m_name, m_idx, msg_code, false);
}

#define SINGULAR_MON   1
#define PLURAL_MON     2
           







