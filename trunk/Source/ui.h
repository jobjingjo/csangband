/*
 * Copyright (c) 2007 Pete Mack and others
 * This code released under the Gnu Public License. See www.fsf.org
 * for current GPL license details. Addition permission granted to
 * incorporate modifications in all Angband variants as defined in the
 * Angband variants FAQ. See rec.games.roguelike.angband for FAQ.
 */



#ifndef INCLUDED_UI_H
#define INCLUDED_UI_H




/*** Text ***/

#include "z-textblock.h"
void textui_textblock_show(textblock *tb, region orig_area, const char *header);
void textui_textblock_place(textblock *tb, region orig_area, const char *header);


/*** Misc ***/

void window_make(int origin_x, int origin_y, int end_x, int end_y);


#endif /* INCLUDED_UI_H */
