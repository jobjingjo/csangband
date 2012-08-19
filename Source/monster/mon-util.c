/*
 * File: mon-util.c
 * Purpose: Monster manipulation utilities.
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
#include "monster/mon-make.h"
#include "monster/mon-msg.h"
#include "monster/mon-spell.h"
#include "monster/mon-util.h"
#include "squelch.h"


/*
 * Return the r_idx of the monster with the given name.
 * If no monster has the exact name given, returns the r_idx
 * of the first monster having the given name as a prefix.
 */
int lookup_monster(const char *name)
{
	int i;
	int r_idx = -1;

	/* Look for it */
	for (i = 1; i < z_info.r_max; i++)
	{
		monster_race *r_ptr = &r_info[i];

		/* Test for equality */
		if (r_ptr.name && streq(name, r_ptr.name))
			return i;

		/* Test for close matches */
		if (r_ptr.name && my_stristr(r_ptr.name, name) && r_idx == -1)
			r_idx = i;
	} 

	/* Return our best match */
	return r_idx;
}

/*
 * Mega-hack - Fix plural names of monsters
 *
 * Taken from PernAngband via EY, modified to fit NPP monster list
 *
 * Note: It should handle all regular Angband monsters.
 */
void plural_aux(char *name, size_t max)
{
	int name_len = strlen(name);

	if (strstr(name, " of "))
	{
		char *aider = strstr(name, " of ");
		char dummy[80];
		int i = 0;
		char *ctr = name;

		while (ctr < aider)
		{
			dummy[i] = *ctr;
			ctr++;
			i++;
		}

		if (dummy[i - 1] == 's')
		{
			strcpy (&(dummy[i]), "es");
			i++;
		}
		else
		{
			strcpy (&(dummy[i]), "s");
		}

		strcpy(&(dummy[i + 1]), aider);
		my_strcpy(name, dummy, max);
	}
	else if ((strstr(name, "coins")) || (strstr(name, "gems")))
	{
		char dummy[80];
		strcpy (dummy, "Piles of c");
		my_strcat (dummy, &(name[1]), sizeof(dummy));
		my_strcpy (name, dummy, max);
		return;
	}

	else if (strstr(name, "Greater Servant of"))
	{
		char dummy[80];
		strcpy (dummy, "Greater Servants of ");
		my_strcat (dummy, &(name[1]), sizeof(dummy));
		my_strcpy (name, dummy, max);
		return;
	}
	else if (strstr(name, "Lesser Servant of"))
	{
		char dummy[80];
		strcpy (dummy, "Greater Servants of ");
		my_strcat (dummy, &(name[1]), sizeof(dummy));
		my_strcpy (name, dummy, max);
		return;
	}
	else if (strstr(name, "Servant of"))
	{
		char dummy[80];
		strcpy (dummy, "Servants of ");
		my_strcat (dummy, &(name[1]), sizeof(dummy));
		my_strcpy (name, dummy, max);
		return;
	}
	else if (strstr(name, "Great Wyrm"))
	{
		char dummy[80];
		strcpy (dummy, "Great Wyrms ");
		my_strcat (dummy, &(name[1]), sizeof(dummy));
		my_strcpy (name, dummy, max);
		return;
	}
	else if (strstr(name, "Spawn of"))
	{
		char dummy[80];
		strcpy (dummy, "Spawn of ");
		my_strcat (dummy, &(name[1]), sizeof(dummy));
		my_strcpy (name, dummy, max);
		return;
	}
	else if (strstr(name, "Descendant of"))
	{
		char dummy[80];
		strcpy (dummy, "Descendant of ");
		my_strcat (dummy, &(name[1]), sizeof(dummy));
		my_strcpy (name, dummy, max);
		return;
	}
	else if ((strstr(name, "Manes")) || (name[name_len-1] == 'u') || (strstr(name, "Yeti")) ||
		(streq(&(name[name_len-2]), "ua")) || (streq(&(name[name_len-3]), "nee")) ||
		(streq(&(name[name_len-4]), "idhe")))
	{
		return;
	}
	else if (name[name_len-1] == 'y')
	{
		strcpy(&(name[name_len - 1]), "ies");
	}
	else if (streq(&(name[name_len - 4]), "ouse"))
	{
		strcpy (&(name[name_len - 4]), "ice");
	}
	else if (streq(&(name[name_len - 4]), "lung"))
	{
		strcpy (&(name[name_len - 4]), "lungen");
	}
	else if (streq(&(name[name_len - 3]), "sus"))
	{
		strcpy (&(name[name_len - 3]), "si");
	}
	else if (streq(&(name[name_len - 4]), "star"))
	{
		strcpy (&(name[name_len - 4]), "stari");
	}
	else if (streq(&(name[name_len - 3]), "aia"))
	{
		strcpy (&(name[name_len - 3]), "aiar");
	}
	else if (streq(&(name[name_len - 3]), "inu"))
	{
		strcpy (&(name[name_len - 3]), "inur");
	}
	else if (streq(&(name[name_len - 5]), "culus"))
	{
		strcpy (&(name[name_len - 5]), "culi");
	}
	else if (streq(&(name[name_len - 4]), "sman"))
	{
		strcpy (&(name[name_len - 4]), "smen");
	}
	else if (streq(&(name[name_len - 4]), "lman"))
	{
		strcpy (&(name[name_len - 4]), "lmen");
	}
	else if (streq(&(name[name_len - 2]), "ex"))
	{
		strcpy (&(name[name_len - 2]), "ices");
	}
	else if ((name[name_len - 1] == 'f') && (!streq(&(name[name_len - 2]), "ff")))
	{
		strcpy (&(name[name_len - 1]), "ves");
	}
	else if (((streq(&(name[name_len - 2]), "ch")) || (name[name_len - 1] == 's')) &&
			(!streq(&(name[name_len - 5]), "iarch")))
	{
		strcpy (&(name[name_len]), "es");
	}
	else
	{
		strcpy (&(name[name_len]), "s");
	}
}



/*
 * Helper function for display monlist.  Prints the number of creatures, followed
 * by either a singular or plural version of the race name as appropriate.
 */
static void get_mon_name(char *output_name, size_t max, int r_idx, int in_los)
{
	/* Get monster race and name */
	monster_race *r_ptr = &r_info[r_idx];

	char race_name[80];

	my_strcpy(race_name, r_ptr.name, sizeof(race_name));

	/* Unique names don't have a number */
	if (rf_has(r_ptr.flags, RF_UNIQUE))
	{
		my_strcpy(output_name, "[U] ", max);
	}

	/* Normal races*/
	else
	{
		my_strcpy(output_name, format("%3d ", in_los), max);

		/* Make it plural, if needed. */
		if (in_los > 1)
		{
			plural_aux(race_name, sizeof(race_name));
		}
	}

	/* Mix the quantity and the header. */
	my_strcat(output_name, race_name, max);
}


/*
 * Display visible monsters in a window
 */
void display_monlist(void)
{
	int ii;
	size_t i, j, k;
	int max;
	int line = 1, x = 0;
	int cur_x;
	unsigned total_count = 0, disp_count = 0, type_count = 0, los_count = 0;

	byte attr;

	char m_name[80];
	char buf[80];

	monster_type *m_ptr;
	monster_race *r_ptr;
	monster_race *r2_ptr;

	monster_vis *list;

	u16b *order;

	bool in_term = (Term != angband_term[0]);

	/* Hallucination is weird */
	if (p_ptr.timed[TMD_IMAGE])
	{
		if (in_term)
			clear_from(0);
		Term_gotoxy(0, 0);
		text_out_to_screen(TERM_ORANGE,
			"Your hallucinations are too wild to see things clearly.");

		return;
	}


	/* Clear the term if in a subwindow, set x otherwise */
	if (in_term)
	{
		clear_from(0);
		max = Term.hgt - 1;
	}
	else
	{
		x = 13;
		max = Term.hgt - 2;
	}

	/* Allocate the primary array */
	list = C_ZNEW(z_info.r_max, monster_vis);

	/* Scan the list of monsters on the level */
	for (ii = 1; ii < cave_monster_max(cave); ii++)
	{
		monster_vis *v;

		m_ptr = cave_monster(cave, ii);
		r_ptr = &r_info[m_ptr.r_idx];

		/* Only consider visible, aware monsters */
		if (!m_ptr.ml || m_ptr.unaware) continue;

		/* Take a pointer to this monster visibility entry */
		v = &list[m_ptr.r_idx];

		/* Note each monster type and save its display attr (color) */
		if (!v.count) type_count++;
		if (!v.attr) v.attr = m_ptr.attr ? m_ptr.attr : r_ptr.x_attr;
		
		/* Check for LOS
		 * Hack - we should use (m_ptr.mflag & (MFLAG_VIEW)) here,
		 * but this does not catch monsters detected by ESP which are
		 * targetable, so we cheat and use projectable() instead 
		 */
		if (projectable(p_ptr.py, p_ptr.px, m_ptr.fy, m_ptr.fx,
			PROJECT_NONE))
		{
			/* Increment the total number of in-LOS monsters */
			los_count++;

			/* Increment the LOS count for this monster type */
			v.los++;
			
			/* Check if asleep and increment accordingly */
			if (m_ptr.m_timed[MON_TMD_SLEEP]) v.los_asleep++;
		}
		/* Not in LOS so increment if asleep */
		else if (m_ptr.m_timed[MON_TMD_SLEEP]) v.asleep++;

		/* Bump the count for this race, and the total count */
		v.count++;
		total_count++;
	}

	/* Note no visible monsters at all */
	if (!total_count)
	{
		/* Clear display and print note */
		c_prt(TERM_SLATE, "You see no monsters.", 0, 0);
		if (!in_term)
		    Term_addstr(-1, TERM_WHITE, "  (Press any key to continue.)");

		/* Free up memory */
		FREE(list);

		/* Done */
		return;
	}

	/* Allocate the secondary array */
	order = C_ZNEW(type_count, u16b);

	/* Sort, because we cannot rely on monster.txt being ordered */

	/* Populate the ordered array, starting at 1 to ignore @ */
	for (i = 1; i < z_info.r_max; i++)
	{
		/* No monsters of this race are visible */
		if (!list[i].count) continue;

		/* Get the monster info */
		r_ptr = &r_info[i];

		/* Fit this monster into the sorted array */
		for (j = 0; j < type_count; j++)
		{
			/* If we get to the end of the list, put this one in */
			if (!order[j])
			{
				order[j] = i;
				break;
			}

			/* Get the monster info for comparison */
			r2_ptr = &r_info[order[j]];

			/* Monsters are sorted by depth */
			/* Monsters of same depth are sorted by power */
			if ((r_ptr.level > r2_ptr.level) ||
				((r_ptr.level == r2_ptr.level) &&
				(r_ptr.power > r2_ptr.power)))
			{
				/* Move weaker monsters down the array */
				for (k = type_count - 1; k > j; k--)
				{
					order[k] = order[k - 1];
				}

				/* Put current monster in the right place */
				order[j] = i;
				break;
			}
		}
	}

	/* Message for monsters in LOS - even if there are none */
	if (!los_count) prt(format("You can see no monsters."), 0, 0);
	else prt(format("You can see %d monster%s", los_count, (los_count == 1
		? ":" : "s:")), 0, 0);

	/* Print out in-LOS monsters in descending order */
	for (i = 0; (i < type_count) && (line < max); i++)
	{
		/* Skip if there are none of these in LOS */
		if (!list[order[i]].los) continue;

		/* Reset position */
		cur_x = x;

		/* Note that these have been displayed */
		disp_count += list[order[i]].los;

		/* Get monster race and name */
		r_ptr = &r_info[order[i]];

		/* Get monster race and name */
		get_mon_name(m_name, sizeof(m_name), order[i], list[order[i]].los);

		/* Display uniques in a special colour */
		if (rf_has(r_ptr.flags, RF_UNIQUE))
			attr = TERM_VIOLET;
		else if (r_ptr.level > p_ptr.depth)
			attr = TERM_RED;
		else
			attr = TERM_WHITE;

		/* Build the monster name */
		if (list[order[i]].los == 1)
			strnfmt(buf, sizeof(buf), (list[order[i]].los_asleep ==
			1 ? "%s (asleep) " : "%s "), m_name);
		else strnfmt(buf, sizeof(buf), (list[order[i]].los_asleep > 0 ?
			"%s (%d asleep) " : "%s"), m_name, list[order[i]].los_asleep);

		/* Display the pict */
		if ((tile_width == 1) && (tile_height == 1))
		{
		        Term_putch(cur_x++, line, list[order[i]].attr, r_ptr.x_char);
			Term_putch(cur_x++, line, TERM_WHITE, ' ');
		}

		/* Print and bump line counter */
		c_prt(attr, buf, line, cur_x);
		line++;

		/* Page wrap */
		if (!in_term && (line == max) && disp_count != total_count)
		{
			prt("-- more --", line, x);
			anykey();

			/* Clear the screen */
			for (line = 1; line <= max; line++)
				prt("", line, 0);

			/* Reprint Message */
			prt(format("You can see %d monster%s",
				los_count, (los_count > 0 ? (los_count == 1 ?
				":" : "s:") : "s.")), 0, 0);

			/* Reset */
			line = 1;
		}
	}

	/* Message for monsters outside LOS, if there are any */
	if (total_count > los_count)
	{
		/* Leave a blank line */
		line++;
		
		prt(format("You are aware of %d %smonster%s", 
		(total_count - los_count), (los_count > 0 ? "other " : ""), 
		((total_count - los_count) == 1 ? ":" : "s:")), line++, 0);
	}

	/* Print out non-LOS monsters in descending order */
	for (i = 0; (i < type_count) && (line < max); i++)
	{
		int out_of_los = list[order[i]].count - list[order[i]].los;

		/* Skip if there are none of these out of LOS */
		if (list[order[i]].count == list[order[i]].los) continue;

		/* Reset position */
		cur_x = x;

		/* Note that these have been displayed */
		disp_count += out_of_los;

		/* Get monster race and name */
		r_ptr = &r_info[order[i]];
		get_mon_name(m_name, sizeof(m_name), order[i], out_of_los);

		/* Display uniques in a special colour */
		if (rf_has(r_ptr.flags, RF_UNIQUE))
			attr = TERM_VIOLET;
		else if (r_ptr.level > p_ptr.depth)
			attr = TERM_RED;
		else
			attr = TERM_WHITE;

		/* Build the monster name */
		if (out_of_los == 1)
			strnfmt(buf, sizeof(buf), (list[order[i]].asleep ==
			1 ? "%s (asleep) " : "%s "), m_name);
		else strnfmt(buf, sizeof(buf), (list[order[i]].asleep > 0 ? 
			"%s (%d asleep) " : "%s"), m_name,
			list[order[i]].asleep);

		/* Display the pict */
		if ((tile_width == 1) && (tile_height == 1))
		{
		        Term_putch(cur_x++, line, list[order[i]].attr, r_ptr.x_char);
			Term_putch(cur_x++, line, TERM_WHITE, ' ');
		}

		/* Print and bump line counter */
		c_prt(attr, buf, line, cur_x);
		line++;

		/* Page wrap */
		if (!in_term && (line == max) && disp_count != total_count)
		{
			prt("-- more --", line, x);
			anykey();

			/* Clear the screen */
			for (line = 1; line <= max; line++)
				prt("", line, 0);

			/* Reprint Message */
			prt(format("You are aware of %d %smonster%s",
				(total_count - los_count), (los_count > 0 ?
				"other " : ""), ((total_count - los_count) > 0
				? ((total_count - los_count) == 1 ? ":" : "s:")
				: "s.")), 0, 0);

			/* Reset */
			line = 1;
		}
	}


	/* Print "and others" message if we've run out of space */
	if (disp_count != total_count)
	{
		strnfmt(buf, sizeof buf, "  ...and %d others.", total_count - disp_count);
		c_prt(TERM_WHITE, buf, line, x);
	}

	/* Otherwise clear a line at the end, for main-term display */
	else
	{
		prt("", line, x);
	}

	if (!in_term)
		Term_addstr(-1, TERM_WHITE, "  (Press any key to continue.)");

	/* Free the arrays */
	FREE(list);
	FREE(order);
}

/*
 * Hack -- the "type" of the current "summon specific"
 */
static int summon_specific_type = 0;


/*
 * Hack -- help decide if a monster race is "okay" to summon
 */
static bool summon_specific_okay(int r_idx)
{
	monster_race *r_ptr = &r_info[r_idx];
	bitflag *flags = r_ptr.flags;
	struct monster_base *base = r_ptr.base;

	bool unique = rf_has(r_ptr.flags, RF_UNIQUE);
	bool scary = flags_test(flags, RF_SIZE, RF_UNIQUE, RF_FRIEND, RF_FRIENDS, RF_ESCORT, RF_ESCORTS, FLAG_END);

	/* Check our requirements */
	switch (summon_specific_type)
	{
		case S_ANIMAL: return !unique && rf_has(flags, RF_ANIMAL);
		case S_SPIDER: return !unique && match_monster_bases(base, "spider", null);
		case S_HOUND: return !unique && match_monster_bases(base, "canine", "zephyr hound", null);
		case S_HYDRA: return !unique && match_monster_bases(base, "hydra", null);
		case S_ANGEL: return !scary && match_monster_bases(base, "angel", null);
		case S_DEMON: return !scary && rf_has(flags, RF_DEMON);
		case S_UNDEAD: return !scary && rf_has(flags, RF_UNDEAD);
		case S_DRAGON: return !scary && rf_has(flags, RF_DRAGON);
		case S_KIN: return !unique && r_ptr.d_char == summon_kin_type;
		case S_HI_UNDEAD: return match_monster_bases(base, "lich", "vampire", "wraith", null);
		case S_HI_DRAGON: return match_monster_bases(base, "ancient dragon", null);
		case S_HI_DEMON: return match_monster_bases(base, "major demon", null);
		case S_WRAITH: return unique && match_monster_bases(base, "wraith", null);
		case S_UNIQUE: return unique;
		case S_MONSTER: return !scary;
		case S_MONSTERS: return !unique;

		default: return true;
	}
}


/*
 * Place a monster (of the specified "type") near the given
 * location.  Return true iff a monster was actually summoned.
 *
 * We will attempt to place the monster up to 10 times before giving up.
 *
 * Note: S_UNIQUE and S_WRAITH (XXX) will summon Uniques
 * Note: S_HI_UNDEAD and S_HI_DRAGON may summon Uniques
 * Note: None of the other summon codes will ever summon Uniques.
 *
 * This function has been changed.  We now take the "monster level"
 * of the summoning monster as a parameter, and use that, along with
 * the current dungeon level, to help determine the level of the
 * desired monster.  Note that this is an upper bound, and also
 * tends to "prefer" monsters of that level.  Currently, we use
 * the average of the dungeon and monster levels, and then add
 * five to allow slight increases in monster power.
 *
 * Note that we use the new "monster allocation table" creation code
 * to restrict the "get_mon_num()" function to the set of "legal"
 * monsters, making this function much faster and more reliable.
 *
 * Note that this function may not succeed, though this is very rare.
 */
bool summon_specific(int y1, int x1, int lev, int type, int delay)
{
	int i, x = 0, y = 0, r_idx;


	/* Look for a location */
	for (i = 0; i < 20; ++i)
	{
		/* Pick a distance */
		int d = (i / 15) + 1;

		/* Pick a location */
		scatter(&y, &x, y1, x1, d, 0);

		/* Require "empty" floor grid */
		if (!cave_empty_bold(y, x)) continue;

		/* Hack -- no summon on glyph of warding */
		if (cave.feat[y][x] == FEAT_GLYPH) continue;

		/* Okay */
		break;
	}

	/* Failure */
	if (i == 20) return (false);


	/* Save the "summon" type */
	summon_specific_type = type;


	/* Require "okay" monsters */
	get_mon_num_hook = summon_specific_okay;

	/* Prepare allocation table */
	get_mon_num_prep();


	/* Pick a monster, using the level calculation */
	r_idx = get_mon_num((p_ptr.depth + lev) / 2 + 5);


	/* Remove restriction */
	get_mon_num_hook = null;

	/* Prepare allocation table */
	get_mon_num_prep();


	/* Handle failure */
	if (!r_idx) return (false);

	/* Attempt to place the monster (awake, allow groups) */
	if (!place_new_monster(cave, y, x, r_idx, false, true, ORIGIN_DROP_SUMMON))
		return (false);

	/* If delay, try to let the player act before the summoned monsters. */
	/* NOTE: should really be -100, but energy is currently 0-255. */
	if (delay)
		cave_monster(cave, cave.m_idx[y][x]).energy = 0;

	/* Success */
	return (true);
}


/*
 * Make player fully aware of the given mimic.
 */
void become_aware(int m_idx)
{
	monster_type *m_ptr = cave_monster(cave, m_idx);
	monster_race *r_ptr = &r_info[m_ptr.r_idx];
	monster_lore *l_ptr = &l_list[m_ptr.r_idx];

	if(m_ptr.unaware) {
		m_ptr.unaware = false;

		/* Learn about mimicry */
		if (rf_has(r_ptr.flags, RF_UNAWARE))
			rf_on(l_ptr.flags, RF_UNAWARE);

		/* Delete any false items */
		if (m_ptr.mimicked_o_idx > 0) {
			object_type *o_ptr = object_byid(m_ptr.mimicked_o_idx);
			char o_name[80];
			object_desc(o_name, sizeof(o_name), o_ptr, ODESC_FULL);

			/* Print a message */
			msg("The %s was really a monster!", o_name);

			/* Clear the mimicry */
			o_ptr.mimicking_m_idx = 0;
			delete_object_idx(m_ptr.mimicked_o_idx);
			m_ptr.mimicked_o_idx = 0;
		}
		
		/* Update monster and item lists */
		p_ptr.update |= (PU_UPDATE_VIEW | PU_MONSTERS);
		p_ptr.redraw |= (PR_MONLIST | PR_ITEMLIST);
	}
}




/*
 * Learn about an "observed" resistance or other player state property, or
 * lack of it.
 */
void update_smart_learn(struct monster *m, struct player *p, int what)
{
	monster_race *r_ptr = &r_info[m.r_idx];

	/* Sanity check */
	if (!what) return;

	/* anything a monster might learn, the player should learn */
	wieldeds_notice_flag(p, what);

	/* Not allowed to learn */
	if (!OPT(birth_ai_learn)) return;

	/* Too stupid to learn anything */
	if (rf_has(r_ptr.flags, RF_STUPID)) return;

	/* Not intelligent, only learn sometimes */
	if (!rf_has(r_ptr.flags, RF_SMART) && one_in_(2)) return;

	/* Analyze the knowledge; fail very rarely */
	if (check_state(p, what, p.state.flags) && !one_in_(100))
		of_on(m.known_pflags, what);
	else
		of_off(m.known_pflags, what);
}


