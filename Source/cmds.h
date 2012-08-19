#ifndef INCLUDED_CMDS_H
#define INCLUDED_CMDS_H

#include "game-cmd.h"

/* cmd-obj.c */
void do_cmd_uninscribe(cmd_code code, cmd_arg args[]);
void do_cmd_inscribe(cmd_code code, cmd_arg args[]);
void do_cmd_takeoff(cmd_code code, cmd_arg args[]);
void wield_item(object_type *o_ptr, int item, int slot);
void do_cmd_wield(cmd_code code, cmd_arg args[]);
void do_cmd_drop(cmd_code code, cmd_arg args[]);
void do_cmd_destroy(cmd_code code, cmd_arg args[]);
void do_cmd_use(cmd_code code, cmd_arg args[]);
void do_cmd_refill(cmd_code code, cmd_arg args[]);
void do_cmd_study_spell(cmd_code code, cmd_arg args[]);
void do_cmd_cast(cmd_code code, cmd_arg args[]);
void do_cmd_study_book(cmd_code code, cmd_arg args[]);

/* cmd1.c */
int do_autopickup(void);

/* cmd2.c */
void textui_cmd_rest(void);
void textui_cmd_suicide(void);

/* cmd3.c */
void textui_cmd_destroy(void);
void textui_cmd_toggle_ignore(void);

/* cmd4.c */
extern void display_feeling(bool obj_only);

/* attack.c */
extern void do_cmd_fire(cmd_code code, cmd_arg args[]);
extern void do_cmd_throw(cmd_code code, cmd_arg args[]);

/* Types of item use */
typedef enum
{
	USE_TIMEOUT,
	USE_CHARGE,
	USE_SINGLE
} use_type;

/* XXX */
extern int cmp_monsters(const void *a, const void *b);

/* ui-knowledge.c */
extern int big_pad(int col, int row, byte a, byte c);

#endif

