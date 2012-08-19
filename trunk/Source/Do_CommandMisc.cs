using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	partial class Do_Command {
		/*
		 * Toggle wizard mode
		 */
		public static void wizard()
		{
			throw new NotImplementedException();
			///* Verify first time */
			//if (!(p_ptr.noscore & NOSCORE_WIZARD))
			//{
			//    /* Mention effects */
			//    msg("You are about to enter 'wizard' mode for the very first time!");
			//    msg("This is a form of cheating, and your game will not be scored!");
			//    message_flush();

			//    /* Verify request */
			//    if (!get_check("Are you sure you want to enter wizard mode? "))
			//        return;

			//    /* Mark savefile */
			//    p_ptr.noscore |= NOSCORE_WIZARD;
			//}

			///* Toggle mode */
			//if (p_ptr.wizard)
			//{
			//    p_ptr.wizard = false;
			//    msg("Wizard mode off.");
			//}
			//else
			//{
			//    p_ptr.wizard = true;
			//    msg("Wizard mode on.");
			//}

			///* Update monsters */
			//p_ptr.update |= (PU_MONSTERS);

			///* Redraw "title" */
			//p_ptr.redraw |= (PR_TITLE);
		}

		/*
		 * Verify use of "debug" mode
		 */
		//Was in #define ALLOW_DEBUG
		public static void try_debug()
		{
			throw new NotImplementedException();
			///* Ask first time */
			//if (!(p_ptr.noscore & NOSCORE_DEBUG))
			//{
			//    /* Mention effects */
			//    msg("You are about to use the dangerous, unsupported, debug commands!");
			//    msg("Your machine may crash, and your savefile may become corrupted!");
			//    message_flush();

			//    /* Verify request */
			//    if (!get_check("Are you sure you want to use the debug commands? "))
			//        return;

			//    /* Mark savefile */
			//    p_ptr.noscore |= NOSCORE_DEBUG;
			//}

			///* Okay */
			//do_cmd_debug();
		}


		/*
		 * Verify use of "borg" mode
		 */
		//Was in #define BORG
		public static void try_borg()
		{
			throw new NotImplementedException();
			///* Ask first time */
			//if (!(p_ptr.noscore & NOSCORE_BORG))
			//{
			//    /* Mention effects */
			//    msg("You are about to use the dangerous, unsupported, borg commands!");
			//    msg("Your machine may crash, and your savefile may become corrupted!");
			//    message_flush();

			//    /* Verify request */
			//    if (!get_check("Are you sure you want to use the borg commands? "))
			//        return;

			//    /* Mark savefile */
			//    p_ptr.noscore |= NOSCORE_BORG;
			//}

			///* Okay */
			//do_cmd_borg();
		}


		/*
		 * Quit the game.
		 */
		//ARGS was cmd_arg
		public static void quit(Command_Code code, object[] args)
		{
			throw new NotImplementedException();
			///* Stop playing */
			//p_ptr.playing = false;

			///* Leaving */
			//p_ptr.leaving = true;
		}


		/*
		 * Display the options and redraw afterward.
		 */
		public static void xxx_options()
		{
			throw new NotImplementedException();
			//do_cmd_options();
			//do_cmd_redraw();
		}


		/*
		 * Display the main-screen monster list.
		 */
		public static void monlist()
		{
			throw new NotImplementedException();
			///* Save the screen and display the list */
			//screen_save();
			//display_monlist();

			///* Wait */
			//anykey();

			///* Return */
			//screen_load();
		}


		/*
		 * Display the main-screen item list.
		 */
		public static void itemlist()
		{
			throw new NotImplementedException();
			///* Save the screen and display the list */
			//screen_save();
			//display_itemlist();

			///* Wait */
			//anykey();

			///* Return */
			//screen_load();
		}


		/*
		 * Invoked when the command isn't recognised.
		 */
		public static void unknown()
		{
			Utilities.prt("Type '?' for help.", 0, 0);
		}

		/*
		 * Display a "small-scale" map of the dungeon.
		 *
		 * Note that the "player" is always displayed on the map.
		 */
		public static void view_map()
		{
			throw new NotImplementedException();
			//int cy, cx;
			//const char *prompt = "Hit any key to continue";
	
			///* Save screen */
			//screen_save();

			///* Note */
			//prt("Please wait...", 0, 0);

			///* Flush */
			//Term_fresh();

			///* Clear the screen */
			//Term_clear();

			///* Display the map */
			//display_map(&cy, &cx);

			///* Show the prompt */
			//put_str(prompt, Term.hgt - 1, Term.wid / 2 - strlen(prompt) / 2);

			///* Highlight the player */
			//Term_gotoxy(cx, cy);

			///* Get any key */
			//(void)anykey();

			///* Load screen */
			//screen_load();
		}
	}
}
