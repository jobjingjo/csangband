/* target.h - target interface */

#ifndef TARGET_H
#define TARGET_H

void target_set_monster(int m_idx);
void target_set_location(int y, int x);
bool target_set_interactive(int mode, int x, int y);
void target_get(s16b *col, s16b *row);
s16b target_get_monster(void);

#endif /* !TARGET_H */
