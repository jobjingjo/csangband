/*
 * File: mon-timed.h
 * Purpose: Structures and functions for monster timed effects.
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

#ifndef MONSTER_TIMED_H
#define MONSTER_TIMED_H

#include "angband.h"

/** Constants **/

/** Structures **/

/** Variables **/


/** Functions **/
bool mon_inc_timed(int m_idx, int ef_idx, int timer, u16b flag, bool id);
bool mon_dec_timed(int m_idx, int ef_idx, int timer, u16b flag, bool id);
bool mon_clear_timed(int m_idx, int ef_idx, u16b flag, bool id);


#endif /* MONSTER_TIMED_H */
