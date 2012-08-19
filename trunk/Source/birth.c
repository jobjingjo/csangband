/*
 * File: birth.c
 * Purpose: Character creation
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
#include "cmds.h"
#include "files.h"
#include "game-event.h"
#include "game-cmd.h"
#include "history.h"
#include "object/inventory.h"
#include "object/tvalsval.h"
#include "object/object.h"
#include "squelch.h"
#include "ui-menu.h"