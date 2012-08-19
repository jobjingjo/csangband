#ifndef INCLUDED_UI_MENU_H
#define INCLUDED_UI_MENU_H

/*
  Together, these classes define the constant properties of
  the various menu classes.

  A menu consists of:
   - menu_iter, which describes how to handle the type of "list" that's 
     being displayed as a menu
   - a menu_skin, which describes the layout of the menu on the screen.
   - various bits and bobs of other data (e.g. the actual list of entries)
 */
typedef struct menu_type menu_type;


/*** Menu API ***/

/**
 * Allocate and return a new, initialised, menu.
 */
menu_type *menu_new_action(menu_action *acts, size_t n);


/**
 * Given a predefined menu kind, return its iter functions.
 */
const menu_iter *menu_find_iter(menu_iter_id iter_id);


/**
 * Set menu private data and the number of menu items.
 *
 * Menu private data is then available from inside menu callbacks using
 * menu_priv().
 */
void menu_setpriv(menu_type *menu, int count, void *data);


/**
 * Return menu private data, set with menu_setpriv().
 */
void *menu_priv(menu_type *menu);


/*
 * Set a filter on what items a menu can display.
 *
 * Use this if your menu private data has 100 items, but you want to choose
 * which ones of those to display at any given time, e.g. in an inventory menu.
 * object_list[] should be an array of indexes to display, and n should be its
 * length.
 */
void menu_set_filter(menu_type *menu, const int object_list[], int n);


/**
 * Remove any filters set on a menu by menu_set_filer().
 */
void menu_release_filter(menu_type *menu);


/**
 * Ready a menu for display in the region specified.
 *
 * XXX not ready for dynamic resizing just yet
 */
bool menu_layout(menu_type *menu, const region *loc);


/**
 * Display a menu.
 * If reset_screen is true, it will reset the screen to the previously saved
 * state before displaying.
 */
void menu_refresh(menu_type *menu, bool reset_screen);


/**
 * Run a menu.
 *
 * 'notify' is a bitwise OR of ui_event_type events that you want to
 * menu_select to return to you if they're not handled inside the menu loop.
 * e.g. if you want to handle key events without specifying a menu_iter.handle
 * function, you can set notify to EVT_KBRD, and any non-navigation keyboard
 * events will stop the menu loop and return them to you.
 *
 * Some events are returned by default, and else are EVT_ESCAPE and EVT_SELECT.
 * 
 * Event types that can be returned:
 *   EVT_ESCAPE: no selection; go back (by default)
 *   EVT_SELECT: menu.cursor is the selected menu item (by default)
 *   EVT_MOVE:   the cursor has moved
 *   EVT_KBRD:   unhandled keyboard events
 *   EVT_MOUSE:  unhandled mouse events  
 *   EVT_RESIZE: resize events
 * 
 * XXX remove 'notify'
 *
 * If popup is true, the screen background is saved before starting the menu,
 * and restored before each redraw. This allows variably-sized information
 * at the bottom of the menu.
 */
ui_event menu_select(menu_type *menu, int notify, bool popup);


/* Interal menu stuff that cmd-know needs because it's quite horrible */
bool menu_handle_mouse(menu_type *menu, const ui_event *in, ui_event *out);
bool menu_handle_keypress(menu_type *menu, const ui_event *in, ui_event *out);


/*** Dynamic menu handling ***/

menu_type *menu_dynamic_new(void);
void menu_dynamic_add(menu_type *m, const char *text, int value);
size_t menu_dynamic_longest_entry(menu_type *m);
int menu_dynamic_select(menu_type *m);
void menu_dynamic_free(menu_type *m);

#endif /* INCLUDED_UI_MENU_H */
