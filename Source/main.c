/*
 * File: main.c
 * Purpose: Core game initialisation for UNIX (and other) machines
 *
 * Copyright (c) 1997 Ben Harrison, and others
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
#include "init.h"

/*
 * Some machines have a "main()" function in their "main-xxx.c" file,
 * all the others use this file for their "main()" function.
 */

#if defined(WIN32_CONSOLE_MODE) || !defined(WINDOWS) || defined(USE_SDL)

#include "main.h"
#include "textui.h"
#include "init.h"

/*
 * List of the available modules in the order they are tried.
 */
static const struct module modules[] =
{
#ifdef USE_GTK
	{ "gtk", help_gtk, init_gtk },
#endif /* USE_GTK */

#ifdef USE_X11
	{ "x11", help_x11, init_x11 },
#endif /* USE_X11 */

#ifdef USE_SDL
	{ "sdl", help_sdl, init_sdl },
#endif /* USE_SDL */

#ifdef USE_GCU
	{ "gcu", help_gcu, init_gcu },
#endif /* USE_GCU */

#ifdef USE_TEST
	{ "test", help_test, init_test },
#endif /* !USE_TEST */

#ifdef USE_STATS
	{ "stats", help_stats, init_stats },
#endif /* USE_STATS */
};

static int init_sound_dummy(int argc, char *argv[]) {
	return 0;
}

/*
 * List of sound modules in the order they should be tried.
 */
static const struct module sound_modules[] =
{
#ifdef SOUND_SDL
	{ "sdl", "SDL_mixer sound module", init_sound_sdl },
#endif /* SOUND_SDL */

	{ "none", "No sound", init_sound_dummy },
};


/*
 * SDL needs a look-in
 */
#ifdef USE_SDL
# include "SDL.h"
#endif


/*
 * Initialize and verify the file paths, and the score file.
 *
 * Use the ANGBAND_PATH environment var if possible, else use
 * DEFAULT_PATH, and in either case, branch off appropriately.
 *
 * First, we'll look for the ANGBAND_PATH environment variable,
 * and then look for the files in there.  If that doesn't work,
 * we'll try the DEFAULT_PATH constants.  So be sure that one of
 * these two things works...
 *
 * We must ensure that the path ends with "PATH_SEP" if needed,
 * since the "init_file_paths()" function will simply append the
 * relevant "sub-directory names" to the given path.
 *
 * Make sure that the path doesn't overflow the buffer.  We have
 * to leave enough space for the path separator, directory, and
 * filenames.
 */
static void init_stuff(void)
{
	char configpath[512];
	char libpath[512];
	char datapath[512];

	/* Use the angband_path, or a default */
	my_strcpy(configpath, DEFAULT_CONFIG_PATH, sizeof(configpath));
	my_strcpy(libpath, DEFAULT_LIB_PATH, sizeof(libpath));
	my_strcpy(datapath, DEFAULT_DATA_PATH, sizeof(datapath));

	/* Make sure they're terminated */
	configpath[511] = '\0';
	libpath[511] = '\0';
	datapath[511] = '\0';

	/* Hack -- Add a path separator (only if needed) */
	if (!suffix(configpath, PATH_SEP)) my_strcat(configpath, PATH_SEP, sizeof(configpath));
	if (!suffix(libpath, PATH_SEP)) my_strcat(libpath, PATH_SEP, sizeof(libpath));
	if (!suffix(datapath, PATH_SEP)) my_strcat(datapath, PATH_SEP, sizeof(datapath));

	/* Initialize */
	init_file_paths(configpath, libpath, datapath);
}



/*
 * Handle a "-d<what>=<path>" option
 *
 * The "<what>" can be any string starting with the same letter as the
 * name of a subdirectory of the "lib" folder (i.e. "i" or "info").
 *
 * The "<path>" can be any legal path for the given system, and should
 * not end in any special path separator (i.e. "/tmp" or "~/.ang-info").
 */
static void change_path(const char *info)
{
	if (!info || !info[0])
		quit_fmt("Try '-d<path>'.", info);

	string_free(ANGBAND_DIR_USER);
	ANGBAND_DIR_USER = string_make(info);
}




#ifdef SET_UID

/*
 * Find a default user name from the system.
 */
static void user_name(char *buf, size_t len, int id)
{
	struct passwd *pw = getpwuid(id);

	/* Default to PLAYER */
	if (!pw)
	{
		my_strcpy(buf, "PLAYER", len);
		return;
	}

	/* Capitalise and copy */
	strnfmt(buf, len, "%^s", pw.pw_name);
}

#endif /* SET_UID */

static void debug_opt(const char *arg) {
	if (streq(arg, "mem-poison-alloc"))
		mem_flags |= MEM_POISON_ALLOC;
	else if (streq(arg, "mem-poison-free"))
		mem_flags |= MEM_POISON_FREE;
	else {
		puts("Debug flags:");
		puts("  mem-poison-alloc: Poison all memory allocations");
		puts("   mem-poison-free: Poison all freed memory");
		exit(0);
	}
}

#endif
