/*
 * File: signals.c
 * Purpose: Handle various OS signals
 *
 * Copyright (c) 1997 Ben Harrison
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
#include "files.h"
#include "savefile.h"

#ifndef WINDOWS

#include <signal.h>

#ifdef SET_UID
# include <sys/types.h>
#endif




#endif	/* !WINDOWS */

