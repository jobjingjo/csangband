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
 * Determine damage for critical hits from shooting.
 *
 * Factor in item weight, total plusses, and player level.
 */
static int critical_shot(int weight, int plus, int dam, u32b *msg_type) {
	int chance = weight + (p_ptr.state.to_h + plus) * 4 + p_ptr.lev * 2;
	int power = weight + randint1(500);

	if (randint1(5000) > chance) {
		*msg_type = MSG_SHOOT_HIT;
		return dam;

	} else if (power < 500) {
		*msg_type = MSG_HIT_GOOD;
		return 2 * dam + 5;

	} else if (power < 1000) {
		*msg_type = MSG_HIT_GREAT;
		return 2 * dam + 10;

	} else {
		*msg_type = MSG_HIT_SUPERB;
		return 3 * dam + 15;
	}
}

/**
 * This is a helper function used by do_cmd_throw and do_cmd_fire.
 *
 * It abstracts out the projectile path, display code, identify and clean up
 * logic, while using the 'attack' parameter to do work particular to each
 * kind of attack.
 */
static void ranged_helper(int item, int dir, int range, int shots, ranged_attack attack) {
	/* Get the ammo */
	object_type *o_ptr = object_from_item_idx(item);

	int i, j;
	byte missile_attr = object_attr(o_ptr);
	char missile_char = object_char(o_ptr);

	object_type object_type_body;
	object_type *i_ptr = &object_type_body;

	char o_name[80];

	int path_n;
	u16b path_g[256];

	int msec = op_ptr.delay_factor;

	/* Start at the player */
	int x = p_ptr.px;
	int y = p_ptr.py;

	/* Predict the "target" location */
	s16b ty = y + 99 * ddy[dir];
	s16b tx = x + 99 * ddx[dir];

	bool hit_target = false;

	/* Check for target validity */
	if ((dir == 5) && target_okay()) {
		int taim;
		char msg[80];
		target_get(&tx, &ty);
		taim = distance(y, x, ty, tx);
		if (taim > range) {
			sprintf (msg, "Target out of range by %d squares. Fire anyway? ",
				taim - range);
			if (!get_check(msg)) return;
		}
	}

	/* Sound */
	sound(MSG_SHOOT);

	object_notice_on_firing(o_ptr);

	/* Describe the object */
	object_desc(o_name, sizeof(o_name), o_ptr, ODESC_FULL | ODESC_SINGULAR);

	/* Actually "fire" the object -- Take a partial turn */
	p_ptr.energy_use = (100 / shots);

	/* Calculate the path */
	path_n = project_path(path_g, range, y, x, ty, tx, 0);

	/* Hack -- Handle stuff */
	handle_stuff(p_ptr);

	/* Start at the player */
	x = p_ptr.px;
	y = p_ptr.py;

	/* Project along the path */
	for (i = 0; i < path_n; ++i) {
		int ny = GRID_Y(path_g[i]);
		int nx = GRID_X(path_g[i]);

		/* Hack -- Stop before hitting walls */
		if (!cave_floor_bold(ny, nx)) break;

		/* Advance */
		x = nx;
		y = ny;

		/* Only do visuals if the player can "see" the missile */
		if (player_can_see_bold(y, x)) {
			print_rel(missile_char, missile_attr, y, x);
			move_cursor_relative(y, x);

			Term_fresh();
			if (p_ptr.redraw) redraw_stuff(p_ptr);

			Term_xtra(TERM_XTRA_DELAY, msec);
			cave_light_spot(cave, y, x);

			Term_fresh();
			if (p_ptr.redraw) redraw_stuff(p_ptr);
		} else {
			/* Delay anyway for consistency */
			Term_xtra(TERM_XTRA_DELAY, msec);
		}

		/* Handle monster */
		if (cave.m_idx[y][x] > 0) break;
	}

	/* Try the attack on the monster at (x, y) if any */
	if (cave.m_idx[y][x] > 0) {
		monster_type *m_ptr = cave_monster(cave, cave.m_idx[y][x]);
		monster_race *r_ptr = &r_info[m_ptr.r_idx];
		int visible = m_ptr.ml;

		bool fear = false;
		char m_name[80];
		const char *note_dies = monster_is_unusual(r_ptr) ? " is destroyed." : " dies.";

		struct attack_result result = attack(o_ptr, y, x);
		int dmg = result.dmg;
		u32b msg_type = result.msg_type;
		const char *hit_verb = result.hit_verb;

		if (result.success) {
			hit_target = true;

			/* Get "the monster" or "it" */
			monster_desc(m_name, sizeof(m_name), m_ptr, 0);
		
			object_notice_attack_plusses(o_ptr);
		
			/* No negative damage; change verb if no damage done */
			if (dmg <= 0) {
				dmg = 0;
				hit_verb = "fail to harm";
			}
		
			if (!visible) {
				/* Invisible monster */
				msgt(MSG_SHOOT_HIT, "The %s finds a mark.", o_name);
			} else {
				/* Visible monster */
				if (msg_type == MSG_SHOOT_HIT)
					msgt(MSG_SHOOT_HIT, "The %s %s %s.", o_name, hit_verb, m_name);
				else if (msg_type == MSG_HIT_GOOD) {
					msgt(MSG_HIT_GOOD, "The %s %s %s. %s", o_name, hit_verb, m_name, "It was a good hit!");
				} else if (msg_type == MSG_HIT_GREAT) {
					msgt(MSG_HIT_GREAT, "The %s %s %s. %s", o_name, hit_verb, m_name,
						 "It was a great hit!");
				} else if (msg_type == MSG_HIT_SUPERB) {
					msgt(MSG_HIT_SUPERB, "The %s %s %s. %s", o_name, hit_verb, m_name,
						 "It was a superb hit!");
				}
		
				/* Track this monster */
				if (m_ptr.ml) monster_race_track(m_ptr.r_idx);
				if (m_ptr.ml) health_track(p_ptr, cave.m_idx[y][x]);
			}
		
			/* Complex message */
			if (p_ptr.wizard)
				msg("You do %d (out of %d) damage.", dmg, m_ptr.hp);
		
			/* Hit the monster, check for death */
			if (!mon_take_hit(cave.m_idx[y][x], dmg, &fear, note_dies)) {
				message_pain(cave.m_idx[y][x], dmg);
				if (fear && m_ptr.ml)
					add_monster_message(m_name, cave.m_idx[y][x], MON_MSG_FLEE_IN_TERROR, true);
			}
		}
	}

	/* Obtain a local object */
	object_copy(i_ptr, o_ptr);
	object_split(i_ptr, o_ptr, 1);

	/* See if the ammunition broke or not */
	j = breakage_chance(i_ptr, hit_target);

	/* Drop (or break) near that location */
	drop_near(cave, i_ptr, j, y, x, true);

	if (item >= 0) {
		/* The ammo is from the inventory */
		inven_item_increase(item, -1);
		inven_item_describe(item);
		inven_item_optimize(item);
	} else {
		/* The ammo is from the floor */
		floor_item_increase(0 - item, -1);
		floor_item_optimize(0 - item);
	}
}





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
