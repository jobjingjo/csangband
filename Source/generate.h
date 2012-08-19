/* generate.h - dungeon generation interface */

#ifndef GENERATE_H
#define GENERATE_H

void ensure_connectedness(struct cave *c);

void place_object(struct cave *c, int y, int x, int level, bool good,
	bool great, byte origin);
void place_gold(struct cave *c, int y, int x, int level, byte origin);
void place_secret_door(struct cave *c, int y, int x);
void place_closed_door(struct cave *c, int y, int x);
void place_random_door(struct cave *c, int y, int x);

extern struct vault *random_vault(int typ);





#endif /* !GENERATE_H */
