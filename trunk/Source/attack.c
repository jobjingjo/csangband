/*
 * File: attack.c
 * Purpose: Attacking (both throwing and melee) code
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

#include "attack.h"
#include "cave.h"
#include "cmds.h"
#include "monster/mon-make.h"
#include "monster/mon-msg.h"
#include "monster/mon-util.h"
#include "monster/mon-timed.h"
#include "monster/monster.h"
#include "object/slays.h"
#include "object/tvalsval.h"
#include "spells.h"
#include "target.h"

/**
 * Helper function used with ranged_helper by do_cmd_throw.
 */
static struct attack_result make_ranged_throw(object_type *o_ptr, int y, int x) {
	struct attack_result result = {false, 0, 0, "hit"};

	monster_type *m_ptr = cave_monster(cave, cave.m_idx[y][x]);
	monster_race *r_ptr = &r_info[m_ptr.r_idx];

	int bonus = p_ptr.state.to_h + o_ptr.to_h;
	int chance = p_ptr.state.skills[SKILL_TO_HIT_THROW] + bonus * BTH_PLUS_ADJ;
	int chance2 = chance - distance(p_ptr.py, p_ptr.px, y, x);

	int multiplier = 1;
	const struct slay *best_s_ptr = null;

	/* If we missed then we're done */
	if (!test_hit(chance2, r_ptr.ac, m_ptr.ml)) return result;

	result.success = true;

	improve_attack_modifier(o_ptr, m_ptr, &best_s_ptr, true, false);

	/* If we have a slay, modify the multiplier appropriately */
	if (best_s_ptr != null) {
		result.hit_verb = best_s_ptr.range_verb;
		multiplier += best_s_ptr.mult;
	}

	/* Apply damage: multiplier, slays, criticals, bonuses */
	result.dmg = damroll(o_ptr.dd, o_ptr.ds);
	result.dmg += o_ptr.to_d;
	result.dmg *= multiplier;
	result.dmg = critical_norm(o_ptr.weight, o_ptr.to_h, result.dmg, &result.msg_type);

	return result;
}
