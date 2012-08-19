/*
 * File: mon-timed.c
 * Purpose: Monster timed effects.
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
#include "monster/mon-spell.h"
#include "monster/mon-timed.h"
#include "monster/mon-util.h"

/**
 * Increases the timed effect `ef_idx` by `timer`.
 *
 * Calculates the new timer, then passes that to mon_set_timed().
 * Note that each effect has a maximum number of turns it can be active for.
 * If this function would put an effect timer over that cap, it sets it for
 * that cap instead.
 *
 * Returns true if the monster's timer changed.
 */
bool mon_inc_timed(int m_idx, int ef_idx, int timer, u16b flag, bool id)
{
	monster_type *m_ptr;
	mon_timed_effect *effect;

	assert(ef_idx >= 0 && ef_idx < MON_TMD_MAX);
	effect = &effects[ef_idx];

	assert(m_idx > 0);
	m_ptr = cave_monster(cave, m_idx);

	/* For negative amounts, we use mon_dec_timed instead */
	assert(timer > 0);

	/* Make it last for a mimimum # of turns if it is a new effect */
	if ((!m_ptr.m_timed[ef_idx]) && (timer < 2)) timer = 2;

	/* New counter amount - prevent overflow */
	if (MAX_SHORT - timer < m_ptr.m_timed[ef_idx])
		timer = MAX_SHORT;
	else
		timer += m_ptr.m_timed[ef_idx];

	/* Reduce to max_timer if necessary*/
	if (timer > effect.max_timer)
		timer = effect.max_timer;

	return mon_set_timed(m_ptr, ef_idx, timer, flag, id);
}

