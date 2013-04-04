/*
 * File: savefile.c
 * Purpose: Savefile loading and saving main routines
 *
 * Copyright (c) 2009 Andi Sidwell <andi@takkaria.org>
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
#include <errno.h>
#include "angband.h"
#include "savefile.h"








/** Utility **/


/*
 * Hack -- Show information on the screen, one line at a time.
 *
 * Avoid the top two lines, to avoid interference with "note()".
 */
void note(const char *message)
{
	static int y = 2;

	/* Draw the message */
	prt(message, y, 0);
	pause_line(Term);

	/* Advance one line (wrap if needed) */
	if (++y >= 24) y = 2;

	/* Flush it */
	Term_fresh();
}




/** Base put/get **/

static void sf_put(byte v)
{
	assert(buffer != null);
	assert(buffer_size > 0);

	if (buffer_size == buffer_pos)
	{
		buffer_size += BUFFER_BLOCK_INCREMENT;
		buffer = mem_realloc(buffer, buffer_size);
	}

	assert(buffer_pos < buffer_size);

	buffer[buffer_pos++] = v;
	buffer_check += v;
}

static byte sf_get(void)
{
	assert(buffer != null);
	assert(buffer_size > 0);
	assert(buffer_pos < buffer_size);

	buffer_check += buffer[buffer_pos];

	return buffer[buffer_pos++];
}


/* accessor */

void wr_byte(byte v)
{
	sf_put(v);
}

void wr_u16b(u16b v)
{
	sf_put((byte)(v & 0xFF));
	sf_put((byte)((v >> 8) & 0xFF));
}

void wr_s16b(s16b v)
{
	wr_u16b((u16b)v);
}

void wr_u32b(u32b v)
{
	sf_put((byte)(v & 0xFF));
	sf_put((byte)((v >> 8) & 0xFF));
	sf_put((byte)((v >> 16) & 0xFF));
	sf_put((byte)((v >> 24) & 0xFF));
}

void wr_s32b(s32b v)
{
	wr_u32b((u32b)v);
}

void wr_string(const char *str)
{
	while (*str)
	{
		wr_byte(*str);
		str++;
	}
	wr_byte(*str);
}


void rd_byte(byte *ip)
{
	*ip = sf_get();
}

void rd_u16b(u16b *ip)
{
	(*ip) = sf_get();
	(*ip) |= ((u16b)(sf_get()) << 8);
}

void rd_s16b(s16b *ip)
{
	rd_u16b((u16b*)ip);
}

void rd_u32b(u32b *ip)
{
	(*ip) = sf_get();
	(*ip) |= ((u32b)(sf_get()) << 8);
	(*ip) |= ((u32b)(sf_get()) << 16);
	(*ip) |= ((u32b)(sf_get()) << 24);
}

void rd_s32b(s32b *ip)
{
	rd_u32b((u32b*)ip);
}

void rd_string(char *str, int max)
{
	byte tmp8u;
	int i = 0;

	do
	{
		rd_byte(&tmp8u);

		if (i < max) str[i] = tmp8u;
		if (!tmp8u) break;
	} while (++i);

	str[max - 1] = '\0';
}

void strip_bytes(int n)
{
	byte tmp8u;
	while (n--) rd_byte(&tmp8u);
}







/*** Savefiel loading functions ***/

static bool try_load(ang_file *f)
{
	byte savefile_head[SAVEFILE_HEAD_SIZE];
	u32b block_version, block_size;
	char *block_name;

	while (true)
	{
		size_t i;
		int (*loader)(void) = null;

		/* Load in the next header */
		size_t size = file_read(f, (char *)savefile_head, SAVEFILE_HEAD_SIZE);
		if (!size)
			break;

		if (size != SAVEFILE_HEAD_SIZE || savefile_head[15] != 0) {
			note("Savefile is corrupted -- block header mangled.");
			return false;
		}

#define RECONSTRUCT_U32B(from) \
		((u32b) savefile_head[from]) | \
		((u32b) savefile_head[from+1] << 8) | \
		((u32b) savefile_head[from+2] << 16) | \
		((u32b) savefile_head[from+3] << 24);

		block_name = (char *) savefile_head;
		block_version = RECONSTRUCT_U32B(16);
		block_size = RECONSTRUCT_U32B(20);

		/* pad to 4 bytes */
		if (block_size % 4)
			block_size += 4 - (block_size % 4);

		/* Find the right loader */
		for (i = 0; i < N_ELEMENTS(loaders); i++) {
			if (streq(block_name, loaders[i].name) &&
					block_version == loaders[i].version) {
				loader = loaders[i].load;
			}
		}

		if (!loader) {
			/* No loader found */
			note("Savefile too old.  Try importing it into an older Angband first.");
			return false;
		}

		/* Allocate space for the buffer */
		buffer = mem_alloc(block_size);
		buffer_pos = 0;
		buffer_check = 0;

		buffer_size = file_read(f, (char *) buffer, block_size);
		if (buffer_size != block_size) {
			note("Savefile is corrupted -- not enough bytes.");
			mem_free(buffer);
			return false;
		}

		/* Try loading */
		if (loader() != 0) {
			note("Savefile is corrupted.");
			mem_free(buffer);
			return false;
		}

		mem_free(buffer);
	}

	/* Still alive */
	if (p_ptr.chp >= 0)
	{
		/* Reset cause of death */
		my_strcpy(p_ptr.died_from, "(alive and well)", sizeof(p_ptr.died_from));
	}

	return true;
}
