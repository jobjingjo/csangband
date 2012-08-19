/*
 * File: monster.h
 * Purpose: structures and functions for monsters
 *
 * Copyright (c) 2007 Andi Sidwell
 * Copyright (c) 2010 Chris Carr
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
#ifndef MONSTER_MONSTER_H
#define MONSTER_MONSTER_H

#include "defines.h"
#include "h-basic.h"
#include "z-bitflag.h"
#include "z-rand.h"
#include "cave.h"
#include "player/types.h"

/** Constants **/

/* Flags for the monster timed functions */
#define MON_TMD_FLG_NOTIFY		0x01 /* Give notification */
#define MON_TMD_MON_SOURCE		0x02 /* Monster is causing the damage */
#define MON_TMD_FLG_NOMESSAGE	0x04 /* Never show a message */
#define MON_TMD_FLG_NOFAIL		0x08 /* Never fail */


/*** Functions ***/

/* melee2.c */
extern bool check_hit(struct player *p, int power, int level);
extern void process_monsters(struct cave *c, byte min_energy);
int mon_hp(const struct monster_race *r_ptr, aspect hp_aspect);

#ifdef TEST
extern bool (*testfn_make_attack_normal)(struct monster *m, struct player *p);
#endif /* !TEST */

#endif /* !MONSTER_MONSTER_H */
