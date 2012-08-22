#ifndef INCLUDED_EXTERNS_H
#define INCLUDED_EXTERNS_H

#include "monster/constants.h"
#include "monster/monster.h"
#include "object/object.h"
#include "player/types.h"
#include "store.h"
#include "types.h"
#include "x-char.h"
#include "z-file.h"
#include "z-msg.h"
#include "spells.h"

/* This file was automatically generated. It is now obsolete (it was never a
 * good idea to begin with; you should include only what you use instead of
 * including everything everywhere) and is being slowly destroyed. Do not add
 * new entries to this file.
 */

#ifdef ALLOW_BORG
/* Screensaver variables for the borg.  apw */
extern bool screensaver;
#endif /* ALLOW_BORG */

/* tables.c */
extern const s32b player_exp[PY_MAX_LEVEL];
extern const byte chest_traps[64];
extern const char *stat_names[A_MAX];
extern const char *window_flag_desc[32];
extern const byte char_tables[256][CHAR_TABLE_SLOTS];

/* variable.c */
extern const char *copyright;
extern int arg_graphics;
extern bool arg_graphics_nice;
extern bool character_existed;
extern bool character_saved;
extern s16b character_xtra;
extern bool use_graphics_nice;
extern char savefile[1024];
extern term *angband_term[ANGBAND_TERM_MAX];
extern char angband_term_name[ANGBAND_TERM_MAX][16];
extern byte angband_color_table[MAX_COLORS][4];
extern color_type color_table[MAX_COLORS];
extern const char *angband_sound_name[MSG_MAX];
extern monster_lore *l_list;
extern quest *q_list;
extern alloc_entry *alloc_ego_table;
extern s16b alloc_race_size;
extern alloc_entry *alloc_race_table;
extern byte gf_to_attr[GF_MAX][BOLT_MAX];
extern char gf_to_char[GF_MAX][BOLT_MAX];

extern byte item_tester_tval;
extern bool (*get_mon_num_hook)(int r_idx);
extern ang_file *text_out_file;
extern int text_out_indent;
extern int text_out_pad;
extern bool use_transparency;

/* util.c */
extern struct keypress *inkey_next;

/* cmd1.c */
extern bool search(bool verbose);
extern byte py_pickup(int pickup);
extern void move_player(int dir, bool disarm);

/* cmd2.c */
int count_feats(int *y, int *x, bool (*test)(struct cave *cave, int y, int x), bool under);
int count_chests(int *y, int *x, bool trapped);
int coords_to_dir(int y, int x);

/* death.c */
void death_screen(void);

/* dungeon.c */
extern int value_check_aux1(const object_type *o_ptr);
extern void idle_update(void);

/* melee2.c */
extern bool make_attack_spell(int m_idx);

/* pathfind.c */
extern bool findpath(int y, int x);
extern void run_step(int dir);

/* score.c */
extern void enter_score(time_t *death_time);
extern void show_scores(void);
extern void predict_score(void);


/* signals.c */
extern void signals_ignore_tstp(void);
extern void signals_handle_tstp(void);
extern void signals_init(void);

/* store.c */
void do_cmd_store_knowledge(void);

/* util.c */
extern int roman_to_int(const char *roman);
extern int int_to_roman(int n, char *roman, size_t bufsize);
extern void flush_fail(void);
extern struct keypress inkey(void);
extern ui_event inkey_ex(void);
extern void sound(int val);
extern void msg(const char *fmt, ...);
extern void msgt(unsigned int type, const char *fmt, ...);
extern void c_put_str(byte attr, const char *str, int row, int col);
extern void put_str(const char *str, int row, int col);
extern void c_prt(byte attr, const char *str, int row, int col);
extern void text_out_to_file(byte attr, const char *str);
extern void clear_from(int row);
extern bool askfor_aux_keypress(char *buf, size_t buflen, size_t *curs, size_t *len, struct keypress keypress, bool firsttime);
extern bool askfor_aux(char *buf, size_t len, bool keypress_h(char *, size_t, size_t *, size_t *, struct keypress, bool));
extern bool get_string(const char *prompt, char *buf, size_t len);
extern s16b get_quantity(const char *prompt, int max);
extern char get_char(const char *prompt, const char *options, size_t len, char fallback);
extern bool (*get_file)(const char *suggested_name, char *path, size_t len);
extern bool get_com(const char *prompt, struct keypress *command);
extern bool get_com_ex(const char *prompt, ui_event *command);
extern void grid_data_as_text(grid_data *g, byte *ap, char *cp, byte *tap, char *tcp);
extern void pause_line(struct term *term);
extern bool is_a_vowel(int ch);
extern int color_char_to_attr(char c);
extern int color_text_to_attr(const char *name);
extern const char *attr_to_text(byte a);

#ifdef SUPPORT_GAMMA
extern void build_gamma_table(int gamma);
extern byte gamma_table[256];
#endif /* SUPPORT_GAMMA */

/* x-char.c */
extern void xstr_trans(char *str, int encoding);
extern char xchar_trans(byte c);

/* xtra2.c */
bool adjust_panel(int y, int x);
bool change_panel(int dir);
void center_panel(void);
int motion_dir(int y1, int x1, int y2, int x2);

/* xtra3.c */
byte monster_health_attr(void);
void toggle_inven_equip(void);

/* wiz-spoil.c */
bool make_fake_artifact(object_type *o_ptr, struct artifact *artifact);



/* borg.h */
#ifdef ALLOW_BORG
extern void do_cmd_borg(void);
#endif /* ALLOW_BORG */


#endif /* !INCLUDED_EXTERNS_H */

