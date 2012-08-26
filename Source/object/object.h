#ifndef INCLUDED_OBJECT_H
#define INCLUDED_OBJECT_H

#include "z-rand.h"
#include "z-file.h"
#include "z-textblock.h"
#include "z-quark.h"
#include "z-bitflag.h"
#include "game-cmd.h"
#include "cave.h"

struct player;

/*** Constants ***/

#define ORIGIN_SIZE FLAG_SIZE(ORIGIN_MAX)
#define ORIGIN_BYTES 4 /* savefile bytes - room for 32 origin types */


/**
 * Modes for object_info()
 */
typedef enum
{
	OINFO_NONE   = 0x00, /* No options */
	OINFO_TERSE  = 0x01, /* Keep descriptions brief, e.g. for dumps */
	OINFO_SUBJ   = 0x02, /* Describe object from the character's POV */
	OINFO_FULL   = 0x04, /* Treat object as if fully IDd */
	OINFO_DUMMY  = 0x08, /* Object does not exist (e.g. knowledge menu) */
	OINFO_EGO    = 0x10, /* Describe ego random powers */
} oinfo_detail_t;


#define sign(x) ((x) > 0 ? 1 : ((x) < 0 ? -1 : 0))


/*** Functions ***/

/* identify.c */
extern s32b object_last_wield;

bool object_is_known(const object_type *o_ptr);
bool object_is_known_artifact(const object_type *o_ptr);
bool object_is_known_cursed(const object_type *o_ptr);
bool object_is_known_blessed(const object_type *o_ptr);
bool object_is_known_not_artifact(const object_type *o_ptr);
bool object_is_not_known_consistently(const object_type *o_ptr);
bool object_was_sensed(const object_type *o_ptr);=
bool object_ego_is_visible(const object_type *o_ptr);
bool object_attack_plusses_are_visible(const object_type *o_ptr);
bool object_defence_plusses_are_visible(const object_type *o_ptr);
bool object_flag_is_known(const object_type *o_ptr, int flag);
bool object_high_resist_is_possible(const object_type *o_ptr);
void object_flavor_tried(object_type *o_ptr);
void object_notice_everything(object_type *o_ptr);
void object_notice_indestructible(object_type *o_ptr);
void object_notice_sensing(object_type *o_ptr);
void object_sense_artifact(object_type *o_ptr);
bool object_notice_flags(object_type *o_ptr, bitflag flags[OF_SIZE]);
bool object_notice_curses(object_type *o_ptr);
void object_notice_on_defend(struct player *p);
void object_notice_on_wield(object_type *o_ptr);
void object_notice_on_firing(object_type *o_ptr);
void wieldeds_notice_flag(struct player *p, int flag);
void wieldeds_notice_on_attack(void);
void object_repair_knowledge(object_type *o_ptr);
obj_pseudo_t object_pseudo(const object_type *o_ptr);
void sense_inventory(void);
bool easy_know(const object_type *o_ptr);
bool object_check_for_ident(object_type *o_ptr);
bool object_name_is_visible(const object_type *o_ptr);
void object_know_all_flags(object_type *o_ptr);

/* obj-desc.c */
void object_base_name(char *buf, size_t max, int tval, bool plural);

/* obj-info.c */
textblock *object_info(const object_type *o_ptr, oinfo_detail_t mode);
textblock *object_info_ego(struct ego_item *ego);
void object_info_spoil(ang_file *f, const object_type *o_ptr, int wrap);
void object_info_chardump(ang_file *f, const object_type *o_ptr, int indent, int wrap);

/* obj-make.c */
void free_obj_alloc(void);
bool init_obj_alloc(void);
object_kind *get_obj_num(int level, bool good);
s16b apply_magic(object_type *o_ptr, int lev, bool okay, bool good, bool great);
bool make_object(struct cave *c, object_type *j_ptr, int lev, bool good, bool great, s32b *value);
void make_gold(object_type *j_ptr, int lev, int coin_type);
void copy_artifact_data(object_type *o_ptr, const artifact_type *a_ptr);
void ego_apply_magic(object_type *o_ptr, int level);
void ego_min_pvals(object_type *o_ptr);

/* obj-ui.c */
void show_inven(olist_detail_t mode);
void show_equip(olist_detail_t mode);
void show_floor(const int *floor_list, int floor_num, olist_detail_t mode);
bool verify_item(const char *prompt, int item);
bool get_item(int *cp, const char *pmt, const char *str, cmd_code cmd, int mode);

/* obj-util.c */
struct object_kind *objkind_get(int tval, int sval);
struct object_kind *objkind_byid(int kidx);
void flavor_init(void);
void reset_visuals(bool load_prefs);
void object_flags(const object_type *o_ptr, bitflag flags[OF_SIZE]);
void object_flags_known(const object_type *o_ptr, bitflag flags[OF_SIZE]);
char index_to_label(int i);
s16b label_to_inven(int c);
s16b label_to_equip(int c);
bool slot_can_wield_item(int slot, const object_type *o_ptr);
const char *mention_use(int slot);
bool item_tester_okay(const object_type *o_ptr);
int scan_floor(int *items, int max_size, int y, int x, int mode);
void excise_object_idx(int o_idx);
void delete_object_idx(int o_idx);
void delete_object(int y, int x);
void compact_objects(int size);
void wipe_o_list(struct cave *c);
s16b o_pop(void);
object_type *get_first_object(int y, int x);
object_type *get_next_object(const object_type *o_ptr);
bool is_blessed(const object_type *o_ptr);
s32b object_value(const object_type *o_ptr, int qty, int verbose);
s32b object_value_real(const object_type *o_ptr, int qty, int verbose,
    bool known);
void object_wipe(object_type *o_ptr);
void object_copy(object_type *o_ptr, const object_type *j_ptr);
void object_copy_amt(object_type *dst, object_type *src, int amt);
void object_split(struct object *dest, struct object *src, int amt);
s16b floor_carry(struct cave *c, int y, int x, object_type *j_ptr);
void drop_near(struct cave *c, object_type *j_ptr, int chance, int y, int x,
	bool verbose);
void push_object(int y, int x);
void acquirement(int y1, int x1, int level, int num, bool great);
void inven_item_charges(int item);
void inven_item_describe(int item);
void inven_item_optimize(int item);
void floor_item_charges(int item);
void floor_item_describe(int item);
void floor_item_increase(int item, int num);
void floor_item_optimize(int item);
bool inven_carry_okay(const object_type *o_ptr);
bool inven_stack_okay(const object_type *o_ptr);
s16b inven_takeoff(int item, int amt);
void inven_drop(int item, int amt);
void combine_pack(void);
void reorder_pack(void);
void open_quiver_slot(int slot);
int get_use_device_chance(const object_type *o_ptr);
void distribute_charges(object_type *o_ptr, object_type *q_ptr, int amt);
void reduce_charges(object_type *o_ptr, int amt);
int number_charging(const object_type *o_ptr);
bool recharge_timeout(object_type *o_ptr);
unsigned check_for_inscrip(const object_type *o_ptr, const char *inscrip);
object_kind *lookup_kind(int tval, int sval);
int lookup_name(int tval, const char *name);
int lookup_artifact_name(const char *name);
int lookup_sval(int tval, const char *name);
int tval_find_idx(const char *name);
const char *tval_find_name(int tval);
bool obj_is_staff(const object_type *o_ptr);
bool obj_is_wand(const object_type *o_ptr);
bool obj_is_rod(const object_type *o_ptr);
bool obj_is_potion(const object_type *o_ptr);
bool obj_is_scroll(const object_type *o_ptr);
bool obj_is_food(const object_type *o_ptr);
bool obj_is_light(const object_type *o_ptr);
bool obj_is_ring(const object_type *o_ptr);
bool obj_has_charges(const object_type *o_ptr);
bool obj_can_zap(const object_type *o_ptr);
bool obj_is_activatable(const object_type *o_ptr);
bool obj_can_activate(const object_type *o_ptr);
bool obj_can_refill(const object_type *o_ptr);
bool obj_can_browse(const object_type *o_ptr);
bool obj_can_cast_from(const object_type *o_ptr);
bool obj_can_study(const object_type *o_ptr);
bool obj_can_takeoff(const object_type *o_ptr);
bool obj_can_wear(const object_type *o_ptr);
bool obj_can_fire(const object_type *o_ptr);
bool obj_has_inscrip(const object_type *o_ptr);
object_type *object_from_item_idx(int item);
bool get_item_okay(int item);
int scan_items(int *item_list, size_t item_list_max, int mode);
bool item_is_available(int item, bool (*tester)(const object_type *), int mode);
extern void display_itemlist(void);
extern void display_object_idx_recall(s16b o_idx);
extern void display_object_kind_recall(s16b k_idx);

bool pack_is_full(void);
bool pack_is_overfull(void);
void pack_overflow(void);

extern struct object *object_byid(s16b oidx);
extern void objects_init(void);
extern void objects_destroy(void);

/* obj-power.c and randart.c */
char *artifact_gen_name(struct artifact *a, const char ***wordlist);

#endif /* !INCLUDED_OBJECT_H */
