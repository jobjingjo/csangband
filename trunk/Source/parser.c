/*
 * File: parser.c
 * Purpose: Info file parser.
 *
 * Copyright (c) 2011 Elly <elly+angband@leptoquark.net>
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

/**
 * A parser has a list of hooks (which are run across new lines given to
 * parser_parse()) and a list of the set of named values for the current line.
 * Each hook has a list of specs, which are essentially named formal parameters;
 * when we run a particular hook across a line, each spec in the hook is
 * assigned a value.
 */

#include "externs.h"
#include "parser.h"
#include "z-file.h"
#include "z-form.h"
#include "z-util.h"
#include "z-virt.h"


const char *parser_error_str[PARSE_ERROR_MAX] = {
	"(none)",
	"generic error",
	"invalid flag",
	"invalid item number",
	"invalid spell frequency",
	"invalid value",
	"invalid colour",
	"invalid effect",
	"invalid option",
	"missing field",
	"missing record header",
	"field too long",
	"non-sequential records",
	"not a number",
	"not random",
	"obsolete file",
	"out of bounds",
	"out of memory",
	"too few entries",
	"too many entries",
	"undefined directive",
	"unrecognized blow",
	"unrecognized tval",
	"unrecognized sval",
	"vault too big",
	"internal error",
};

struct parser_spec {
	struct parser_spec *next;
	int type;
	const char *name;
};

struct parser_value {
	struct parser_spec spec;
	union {
		char cval;
		int ival;
		unsigned int uval;
		char *sval;
		random_value rval;
	} u;
};

struct parser_hook {
	struct parser_hook *next;
	enum parser_error (*func)(struct parser *p);
	char *dir;
	struct parser_spec *fhead;
	struct parser_spec *ftail;
};

static bool parse_random(const char *str, random_value *bonus) {
	bool negative = false;

	char buffer[50];
	int i = 0, b, dn, ds, mb;
	
	const char end_chr = '|';
	char eov;

	/* Entire value may be negated */
	if (str[0] == '-')
	{
		negative = true;
		i++;
	}

	/* Make a working copy of the string */
	my_strcpy(buffer, &str[i], N_ELEMENTS(buffer) - 2);

	/* Check for invalid negative numbers */
	if (null != strstr(buffer, "-"))
		return false;

	/*
	 * Add a sentinal value at the end of the string.
	 * Used by scanf to make sure there's no text after the final conversion.
	 */
	buffer[strlen(buffer) + 1] = '\0';
	buffer[strlen(buffer)] = end_chr;

	/* Scan the value, apply defaults for unspecified components */
	if (5 == sscanf(buffer, "%d+%dd%dM%d%c", &b, &dn, &ds, &mb, &eov) && eov == end_chr)
	{
		/* No defaults */
	}
	else if (4 == sscanf(buffer, "%d+d%dM%d%c", &b, &ds, &mb, &eov) && eov == end_chr)
	{
		dn = 1;
	}
	else if (3 == sscanf(buffer, "%d+M%d%c", &b, &mb, &eov) && eov == end_chr)
	{
		dn = 0; ds = 0;
	}
	else if (4 == sscanf(buffer, "%d+%dd%d%c", &b, &dn, &ds, &eov) && eov == end_chr)
	{
		mb = 0;
	}
	else if (3 == sscanf(buffer, "%d+d%d%c", &b, &ds, &eov) && eov == end_chr)
	{
		dn = 1; mb = 0;
	}
	else if (4 == sscanf(buffer, "%dd%dM%d%c", &dn, &ds, &mb, &eov) && eov == end_chr)
	{
		b = 0;
	}
	else if (3 == sscanf(buffer, "d%dM%d%c", &ds, &mb, &eov) && eov == end_chr)
	{
		b = 0; dn = 1;
	}
	else if (2 == sscanf(buffer, "M%d%c", &mb, &eov) && eov == end_chr)
	{
		b = 0; dn = 0; ds = 0;
	}
	else if (3 == sscanf(buffer, "%dd%d%c", &dn, &ds, &eov) && eov == end_chr)
	{
		b = 0; mb = 0;
	}
	else if (2 == sscanf(buffer, "d%d%c", &ds, &eov) && eov == end_chr)
	{
		b = 0; dn = 1; mb = 0;
	}
	else if (2 == sscanf(buffer, "%d%c", &b, &eov) && eov == end_chr)
	{
		dn = 0; ds = 0; mb = 0;
	}
	else
	{
		return false;
	}

	/* Assign the values */
	bonus.base = b;
	bonus.dice = dn;
	bonus.sides = ds;
	bonus.m_bonus = mb;

	/*
	 * Handle negation (the random components are always positive, so the base
	 * must be adjusted as necessary).
	 */
	if (negative)
	{
		bonus.base *= -1;
		bonus.base -= bonus.m_bonus;
		bonus.base -= bonus.dice * (bonus.sides + 1);
	}

	return true;
}

int parser_getstate(struct parser *p, struct parser_state *s) {
	s.error = p.error;
	s.line = p.lineno;
	s.col = p.colno;
	s.msg = p.errmsg;
	return s.error != PARSE_ERROR_NONE;
}

void parser_setstate(struct parser *p, unsigned int col, const char *msg) {
	p.colno = col;
	my_strcpy(p.errmsg, msg, sizeof(p.errmsg));
}

void cleanup_parser(struct file_parser *fp)
{
	fp.cleanup();
}

errr remove_flag(bitflag *flags, const size_t size, const char **flag_table, const char *flag_name) {
	int flag = lookup_flag(flag_table, flag_name);

	if (flag == FLAG_END) return PARSE_ERROR_INVALID_FLAG;

	flag_off(flags, size, flag);

	return 0;
}
