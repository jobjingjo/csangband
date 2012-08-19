using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Button {
		/*** Constants ***/

		/**
		 * Maximum number of mouse buttons
		 */
		public const int MAX_MOUSE_BUTTONS = 20;

		/**
		 * Maximum length of a mouse button label
		 */
		public const int MAX_MOUSE_LABEL = 10;


		/*** Types ***/

		/**
		 * Mouse button structure
		 */
		class button_mouse
		{
			public string label; //length was MAX_MOUSE_LABEL /*!< Label on the button */
			public int left;     /*!< Column containing the left edge of the button */
			public int right;    /*!< Column containing the right edge of the button */
			public char key;     /*!< Keypress corresponding to the button */
		}



		/*** Variables ***/

		static button_mouse[] button_mse;
		static button_mouse[] button_backup;

		static int button_start;
		static int button_length;
		static int button_num;


		/*
		 * Hooks for making and unmaking buttons
		 */
		/** Function hook types **/

		/** Function prototype for the UI to provide to create native buttons */
		public delegate int button_add_f(string s, char c);
		static button_add_f button_add_hook;

		/** Function prototype for the UI to provide to remove native buttons */
		public delegate int button_kill_f(char c);
		static button_kill_f button_kill_hook;


		/*** Code ***/

		/*
		 * The mousebutton code. Buttons should be created when neccessary and
		 * destroyed when no longer necessary.  By default, buttons occupy the section
		 * of the bottom line between the status display and the location display
		 * in normal screen mode, and the bottom line after any prompt in alternate
		 * screen mode.
		 *
		 * Individual ports may (and preferably will) handle this differently using
		 * button_add_gui and button_kill_gui.
		 */

		/*
		 * Add a button
		 */
		public static int add_text(string label, char keypress)
		{
			Player.Player p_ptr = Player.Player.instance;
			int length = label.Length;

			/* Check the label length */
			if (length > MAX_MOUSE_LABEL)
			{
			    Utilities.bell("Label too long - button abandoned!");
			    return 0;
			}

			/* Check we haven't already got a button for this keypress */
			for (int i = 0; i < button_num; i++)
			    if (button_mse[i].key == keypress)
			        return 0;

			/* Make the button */
			button_length += length;
			button_mse[button_num].label = label;
			button_mse[button_num].left = button_length - length + 1;
			button_mse[button_num].right = button_length;
			button_mse[button_num++].key = keypress;

			/* Redraw */
			p_ptr.redraw |= (Misc.PR_BUTTONS);

			/* Return the size of the button */
			return (length);
		}

		/*
		 * Add a button
		 */
		public static int button_add(string label, char keypress)
		{
			if (button_add_hook == null)
			    return 0;
			else
			    return button_add_hook(label, keypress);
		}

		/*
		 * Make a backup of the current buttons
		 */
		public static void button_backup_all()
		{
			throw new NotImplementedException();
			///* Check we haven't already done this */
			//if (button_backup[0].key) return;

			///* Straight memory copy */
			//(void)C_COPY(button_backup, button_mse, MAX_MOUSE_BUTTONS, button_mouse);
		}


		/*
		 * Restore the buttons from backup
		 */
		public static void button_restore()
		{
			throw new NotImplementedException();
			//int i = 0;

			///* Remove the current lot */
			//button_kill_all();

			///* Get all the previous buttons, copy them back */
			//while (button_backup[i].key)
			//{
			//    /* Add them all back, forget the backups */
			//    button_add(button_backup[i].label, button_backup[i].key);
			//    button_backup[i].key = '\0';
			//    i++;
			//}
		}


		/*
		 * Remove a button
		 */
		public static int kill_text(char keypress)
		{
			Player.Player p_ptr = Player.Player.instance;

			int i, j, length;

			/* Find the button */
			for (i = 0; i < button_num; i++)
			    if (button_mse[i].key == keypress) break;

			/* No such button */
			if (i == button_num)
			{
			    return 0;
			}

			/* Find the length */
			length = button_mse[i].right - button_mse[i].left + 1;
			button_length -= length;

			/* Move each button up one */
			for (j = i; j < button_num - 1; j++)
			{
			    button_mse[j] = button_mse[j + 1];

			    /* Adjust length */
			    button_mse[j].left -= length;
			    button_mse[j].right -= length;
			}

			/* Wipe the data */
			button_mse[button_num].label = "";
			button_mse[button_num].left = 0;
			button_mse[button_num].right = 0;
			button_mse[button_num--].key = (char)0;

			/* Redraw */
			p_ptr.redraw |= (Misc.PR_BUTTONS);
			p_ptr.redraw_stuff();

			/* Return the size of the button */
			return (length);
		}

		/*
		 * Kill a button
		 */
		public static int button_kill(char keypress)
		{
			throw new NotImplementedException();
			//if (!button_kill_hook) return 0;
			//else
			//    return (*button_kill_hook) (keypress);
		}

		/*
		 * Kill all buttons
		 */
		public static void button_kill_all()
		{
			int i;

			/* Paranoia */
			if (button_kill_hook == null) return;

			/* One by one */
			for (i = button_num - 1; i >= 0; i--)
			    button_kill_hook(button_mse[i].key);
		}


		/*
		 * Initialise buttons.
		 */
		public static void Init(button_add_f add, button_kill_f kill)
		{
		    /* Prepare mouse button arrays */
			button_mse = new button_mouse[MAX_MOUSE_BUTTONS];
			button_backup = new button_mouse[MAX_MOUSE_BUTTONS];

			for(int i = 0; i < button_mse.Length; i++) {
				button_mse[i] = new button_mouse();
				button_backup[i] = new button_mouse();
			}

		    /* Initialise the hooks */
		    button_add_hook = add;
		    button_kill_hook = kill;
		}

		/*
		 * Dispose of the button memory
		 */
		public static void button_free()
		{
			throw new NotImplementedException();
			//FREE(button_mse);
			//FREE(button_backup);
		}

		/**
		 * Return the character represented by a button at screen position (x, y),
		 * or 0.
		 */
		public static char button_get_key(int x, int y)
		{
			throw new NotImplementedException();
			//int i;

			//for (i = 0; i < button_num; i++)
			//{
			//    if ((y == Term.hgt - 1) &&
			//        (x >= button_start + button_mse[i].left) &&
			//        (x <= button_start + button_mse[i].right))
			//    {
			//        return button_mse[i].key;
			//    }
			//}

			//return 0;
		}

		/**
		 * Print the current button list at the specified `row` and `col`umn.
		 */
		public static int button_print(int row, int col)
		{
			throw new NotImplementedException();
			//int j;

			//button_start = col;

			//for (j = 0; j < button_num; j++)
			//    c_put_str(TERM_SLATE, button_mse[j].label, row, col + button_mse[j].left);

			//return button_length;
		}
	}
}
