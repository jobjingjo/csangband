/*
 * File: game-cmd.c
 * Purpose: Handles the queueing of game commands.
 *
 * Copyright (c) 2008-9 Antony Sidwell
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
#include "cmds.h"
#include "game-cmd.h"
#include "object/object.h"
#include "object/tvalsval.h"
#include "spells.h"
#include "target.h"

errr (*cmd_get_hook)(cmd_context c, bool wait);

void cmd_set_arg_string(game_command *cmd, int n, const char *str)
{
	int idx = cmd_idx(cmd.command);

	assert(n <= CMD_MAX_ARGS);
	assert(game_cmds[idx].arg_type[n] & arg_STRING);

	cmd.arg[n].string = string_make(str);
	cmd.arg_type[n] = arg_STRING;
	cmd.arg_present[n] = true;
}

void cmd_set_arg_target(game_command *cmd, int n, int target)
{
	int idx = cmd_idx(cmd.command);

	assert(n <= CMD_MAX_ARGS);
	assert(game_cmds[idx].arg_type[n] & arg_TARGET);

	cmd.arg[n].direction = target;
	cmd.arg_type[n] = arg_TARGET;
	cmd.arg_present[n] = true;
}

void cmd_set_arg_point(game_command *cmd, int n, int x, int y)
{
	int idx = cmd_idx(cmd.command);

	assert(n <= CMD_MAX_ARGS);
	assert(game_cmds[idx].arg_type[n] & arg_POINT);

	cmd.arg[n].point.x = x;
	cmd.arg[n].point.y = y;
	cmd.arg_type[n] = arg_POINT;
	cmd.arg_present[n] = true;
}



