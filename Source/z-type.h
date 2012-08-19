#ifndef INCLUDED_ZTYPE_H
#define INCLUDED_ZTYPE_H

#include "h-basic.h"

typedef struct data_panel data_panel;

extern type_union i2u(int i);
extern type_union f2u(float f);
extern type_union c2u(char c);
extern type_union s2u(const char *s);

static const type_union END = { T_END, { 0 } };


/*
 * A set of points that can be constructed to apply a set of changes to
 */
struct point_set {
	int n;
	int allocated;
	struct loc *pts;
};

struct point_set *point_set_new(int initial_size);
void point_set_dispose(struct point_set *ps);
void add_to_point_set(struct point_set *ps, int y, int x);
int point_set_size(struct point_set *ps);

#endif /* !INCLUDED_ZTYPE_H */
