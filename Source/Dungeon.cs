using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CSAngband.Monster;
using CSAngband.Object;

namespace CSAngband {
	class Dungeon {
		/*
		 * This is used when the user is idle to allow for simple animations.
		 * Currently the only thing it really does is animate shimmering monsters.
		 */
		public static void idle_update()
		{
			if (!Player.Player.character_dungeon) return;

			if (!Option.animate_flicker.value) return;

			throw new NotImplementedException();
			///* Animate and redraw if necessary */
			//do_animation();
			//redraw_stuff(Player.Player.instance);

			/* Refresh the main screen */
			//Term.fresh();
		}

		/*
		 * Actually play a game.
		 *
		 * This function is called from a variety of entry points, since both
		 * the standard "main.c" file, as well as several platform-specific
		 * "main-xxx.c" files, call this function to start a new game with a
		 * new savefile, start a new game with an existing savefile, or resume
		 * a saved game with an existing savefile.
		 *
		 * If the "new_game" parameter is true, and the savefile contains a
		 * living character, then that character will be killed, so that the
		 * player may start a new game with that savefile.  This is only used
		 * by the "-n" option in "main.c".
		 *
		 * If the savefile does not exist, cannot be loaded, or contains a dead
		 * character, then a new game will be started.
		 *
		 * Several platforms (Windows, Macintosh, Amiga) start brand new games
		 * with "savefile" and "oPlayer.Player.instance.base_name" both empty, and initialize
		 * them later based on the player name.  To prevent weirdness, we must
		 * initialize "oPlayer.Player.instance.base_name" to "PLAYER" if it is empty.
		 *
		 * Note that we load the RNG state from savefiles (2.8.0 or later) and
		 * so we only initialize it if we were unable to load it.  The loading
		 * code marks successful loading of the RNG state using the "Rand_quick"
		 * flag, which is a hack, but which optimizes loading of savefiles.
		 */
		public static void play_game()
		{
		    /* Initialize */
		    bool new_game = Init.init_angband();

		    /*** Do horrible, hacky things, to start the game off ***/

		    /* Hack -- Increase "icky" depth */
		    Misc.character_icky++;

		    /* Verify main term */
		    if (Misc.term_screen == null)
		        Utilities.quit("main window does not exist");

		    /* Make sure main term is active */
		    Misc.term_screen.activate();

		    /* Verify minimum size */
		    if ((Term.instance.hgt < 24) || (Term.instance.wid < 80))
		        Utilities.quit("main window is too small");

		    /* Hack -- Turn off the cursor */
		    Term.set_cursor(false);


		    /*** Try to load the savefile ***/

		    Player.Player.instance.is_dead = true;

		    if (Files.savefile.Length > 0 && File.Exists(Files.savefile)) {
		        if (!Savefile.load(Files.savefile))
		            Utilities.quit("broken savefile");

		        if (Player.Player.instance.is_dead && Misc.arg_wizard) {
		                Player.Player.instance.is_dead = false;
		                Player.Player.instance.chp = Player.Player.instance.mhp;
		                Player.Player.instance.noscore |= Misc.NOSCORE_WIZARD;
		        }
		    }

		    /* No living character loaded */
		    if (Player.Player.instance.is_dead)
		    {
		        /* Make new player */
		        new_game = true;

		        /* The dungeon is not ready */
		        Player.Player.character_dungeon = false;
		    }

		    /* Hack -- Default base_name */
			if(Player.Player_Other.instance.base_name.Length == 0)
				Player.Player_Other.instance.base_name = "PLAYER";


		    /* Init RNG */
		    if (Random.Rand_quick)
		    {
		        int seed;

		        /* Basic seed */
		        seed = DateTime.Now.Millisecond;

				//#ifdef SET_UID

				//    /* Mutate the seed on Unix machines */
				//    seed = ((seed >> 3) * (getpid() << 1));

				//#endif

		        /* Use the complex RNG */
		        Random.Rand_quick = false;

		        /* Seed the "complex" RNG */
		        Random.Rand_state_init(seed);
		    }

		    /* Roll new character */
		    if (new_game)
		    {
		        /* The dungeon is not ready */
		        Player.Player.character_dungeon = false;

		        /* Start in town */
		        Player.Player.instance.depth = 0;

		        /* Hack -- seed for flavors */
		        Misc.seed_flavor = (int)Random.randint0(0x10000000);

		        /* Hack -- seed for town layout */
		        Misc.seed_town = (int)Random.randint0(0x10000000);

		        /* Hack -- seed for random artifacts */
		        Misc.seed_randart = (int)Random.randint0(0x10000000);

		        /* Roll up a new character. Quickstart is allowed if ht_birth is set */
		        Birther.player_birth(Player.Player.instance.ht_birth != 0 ? true : false);

		        /* Randomize the artifacts if required */
		        if ((Option.birth_randarts.value) && (!Option.birth_keep_randarts.value || !Player.Player.instance.randarts)) {
		            Object.Randart.do_randart(Misc.seed_randart, true);
		            Player.Player.instance.randarts = true;
		        }
		    }

		    /* Initialize temporary fields sensibly */
		    Player.Player.instance.object_idx = Player.Player.instance.object_kind_idx = Misc.NO_OBJECT;
		    Player.Player.instance.monster_race_idx = 0;

		    /* Normal machine (process player name) */
		    if (Files.savefile.Length > 0)
		        Files.process_player_name(false);

		    /* Weird machine (process player name, pick savefile name) */
		    else
		        Files.process_player_name(true);

		    /* Stop the player being quite so dead */
		    Player.Player.instance.is_dead = false;

		    /* Flash a message */
		    Utilities.prt("Please wait...", 0, 0);

		    /* Allow big cursor */
		    Term.smlcurs = false;

		    /* Flush the message */
		    Term.fresh();

			/* Flavor the objects */
			Object.Flavor.init();


			///* Reset visuals */
			Init.reset_visuals(true);

			///* Tell the UI we've started. */
			Game_Event.signal(Game_Event.Event_Type.ENTER_GAME);

			/* Redraw stuff */
			Player.Player.instance.redraw |= (Misc.PR_INVEN | Misc.PR_EQUIP | Misc.PR_MONSTER | Misc.PR_MESSAGE);
			Player.Player.instance.redraw_stuff();


			/* Process some user pref files */
			process_some_user_pref_files();


			/* React to changes */
			Term.xtra(TERM_XTRA.REACT, 0);


			/* Generate a dungeon level if needed */
			Cave.cave = new Cave(); //Todo: figure out a better place to put this...
			if (!Player.Player.character_dungeon)
			    Cave.cave_generate(Cave.cave, Player.Player.instance);


			/* Character is now "complete" */
			Player.Player.character_generated = true;


			/* Hack -- Decrease "icky" depth */
			Misc.character_icky--;


			/* Start playing */
			Player.Player.instance.playing = true;

			/* Save not required yet. */
			Player.Player.instance.autosave = false;

			/* Hack -- Enforce "delayed death" */
			if (Player.Player.instance.chp < 0) Player.Player.instance.is_dead = true;

			/* Process */
			while (true)
			{
				/* Play ambient sound on change of level. */
				//play_ambient_sound(); //Enable sound later

				/* Process the level */
				dungeon(Cave.cave);

				/* Notice stuff */
				if (Player.Player.instance.notice != 0) Misc.p_ptr.notice_stuff();

				/* Update stuff */
				if (Player.Player.instance.update != 0) Misc.p_ptr.update_stuff();

				/* Redraw stuff */
				if (Player.Player.instance.redraw != 0) Misc.p_ptr.redraw_stuff();


				/* Cancel the target */
				Target.set_monster(0);

				/* Cancel the health bar */
				Cave.health_track(Player.Player.instance, 0);


				/* Forget the view */
				Cave.forget_view();


				/* Handle "quit and save" */
				if (!Player.Player.instance.playing && !Player.Player.instance.is_dead) break;


				/* XXX XXX XXX */
				Utilities.message_flush();

				/* Accidental Death */
				if (Player.Player.instance.playing && Player.Player.instance.is_dead) {
					throw new NotImplementedException();
					///* XXX-elly: this does not belong here. Refactor or
					// * remove. Very similar to do_cmd_wiz_cure_all(). */
					//if ((Player.Player.instance.wizard || OPT(cheat_live)) && !get_check("Die? ")) {
					//    /* Mark social class, reset age, if needed */
					//    if (Player.Player.instance.sc) Player.Player.instance.sc = Player.Player.instance.age = 0;

					//    /* Increase age */
					//    Player.Player.instance.age++;

					//    /* Mark savefile */
					//    Player.Player.instance.noscore |= NOSCORE_WIZARD;

					//    /* Message */
					//    msg("You invoke wizard mode and cheat death.");
					//    message_flush();

					//    /* Cheat death */
					//    Player.Player.instance.is_dead = false;

					//    /* Restore hit points */
					//    Player.Player.instance.chp = Player.Player.instance.mhp;
					//    Player.Player.instance.chp_frac = 0;

					//    /* Restore spell points */
					//    Player.Player.instance.csp = Player.Player.instance.msp;
					//    Player.Player.instance.csp_frac = 0;

					//    /* Hack -- Healing */
					//    Player.Player.instance.clear_timed(Timed_Effect.BLIND, true);
					//    Player.Player.instance.clear_timed(Timed_Effect.CONFUSED, true);
					//    Player.Player.instance.clear_timed(Timed_Effect.POISONED, true);
					//    Player.Player.instance.clear_timed(Timed_Effect.AFRAID, true);
					//    Player.Player.instance.clear_timed(Timed_Effect.PARALYZED, true);
					//    Player.Player.instance.clear_timed(Timed_Effect.IMAGE, true);
					//    Player.Player.instance.clear_timed(Timed_Effect.STUN, true);
					//    Player.Player.instance.clear_timed(Timed_Effect.CUT, true);

					//    /* Hack -- Prevent starvation */
					//    Player.Player.instance.set_food(Player.Player.instance, PY_FOOD_MAX - 1);

					//    /* Hack -- cancel recall */
					//    if (Player.Player.instance.word_recall)
					//    {
					//        /* Message */
					//        msg("A tension leaves the air around you...");
					//        message_flush();

					//        /* Hack -- Prevent recall */
					//        Player.Player.instance.word_recall = 0;
					//    }

					//    /* Note cause of death XXX XXX XXX */
					//    my_strcpy(Player.Player.instance.died_from, "Cheating death", sizeof(Player.Player.instance.died_from));

					//    /* New depth */
					//    Player.Player.instance.depth = 0;

					//    /* Leaving */
					//    Player.Player.instance.leaving = true;
					//}
				}

				/* Handle "death" */
				if (Player.Player.instance.is_dead) break;

				/* Make a new level */
				Cave.cave_generate(Cave.cave, Player.Player.instance);
			}

			throw new NotImplementedException();

			///* Disallow big cursor */
			//smlcurs = true;

			///* Tell the UI we're done with the game state */
			//event_signal(EVENT_LEAVE_GAME);

			///* Close stuff */
			//close_game();
		}

		/*
		 * Process some user pref files
		 */
		static void process_some_user_pref_files()
		{
			string buf;
			//char buf[1024];

			/* Process the "user.prf" file */
			Prefs.process_pref_file("user.prf", true, true);

			/* Get the "PLAYER.prf" filename */
			buf = Player.Player_Other.instance.base_name + ".prf";

			/* Process the "PLAYER.prf" file */
			Prefs.process_pref_file(buf, true, true);
		}

		/*
		 * Interact with the current dungeon level.
		 *
		 * This function will not exit until the level is completed,
		 * the user dies, or the game is terminated.
		 */
		static void dungeon(Cave c)
		{
			Monster.Monster m_ptr;
			int i;



			/* Hack -- enforce illegal panel */
			Term.instance.offset_y = Cave.DUNGEON_HGT;
			Term.instance.offset_x = Cave.DUNGEON_WID;


			/* Not leaving */
			Misc.p_ptr.leaving = false;


			/* Reset the "command" vars */
			Misc.p_ptr.command_arg = 0;


			/* Cancel the target */
			Target.set_monster(0);

			/* Cancel the health bar */
			Cave.health_track(Misc.p_ptr, 0);

			/* Disturb */
			Cave.disturb(Misc.p_ptr, 1, 0);


			/* Track maximum player level */
			if (Misc.p_ptr.max_lev < Misc.p_ptr.lev)
			{
				Misc.p_ptr.max_lev = Misc.p_ptr.lev;
			}


			/* Track maximum dungeon level */
			if (Misc.p_ptr.max_depth < Misc.p_ptr.depth)
			{
				Misc.p_ptr.max_depth = Misc.p_ptr.depth;
			}

			/* If autosave is pending, do it now. */
			if (Misc.p_ptr.autosave)
			{
				///* The borg runs so quickly that this is a bad idea. */
				//#ifndef ALLOW_BORG 
				//        save_game();
				//#endif
				Misc.p_ptr.autosave = false;
			}

			/* Choose panel */
			Xtra2.verify_panel();


			/* Flush messages */
			Utilities.message_flush();


			/* Hack -- Increase "xtra" depth */
			Misc.character_xtra++;


			/* Clear */
			Term.clear();


			/* Update stuff */
			Misc.p_ptr.update |= (Misc.PU_BONUS | Misc.PU_HP | Misc.PU_MANA | Misc.PU_SPELLS);

			/* Calculate torch radius */
			Misc.p_ptr.update |= (Misc.PU_TORCH);

			/* Update stuff */
			Misc.p_ptr.update_stuff();


			/* Fully update the visuals (and monster distances) */
			Misc.p_ptr.update |= (Misc.PU_FORGET_VIEW | Misc.PU_UPDATE_VIEW | Misc.PU_DISTANCE);

			/* Fully update the flow */
			Misc.p_ptr.update |= (Misc.PU_FORGET_FLOW | Misc.PU_UPDATE_FLOW);

			/* Redraw dungeon */
			Misc.p_ptr.redraw |= (uint)(Misc.PR_BASIC | Misc.PR_EXTRA | Misc.PR_MAP);

			/* Redraw "statusy" things */
			Misc.p_ptr.redraw |= (Misc.PR_INVEN | Misc.PR_EQUIP | Misc.PR_MONSTER | Misc.PR_MONLIST | Misc.PR_ITEMLIST);

			/* Update stuff */
			Misc.p_ptr.update_stuff();

			/* Redraw stuff */
			Misc.p_ptr.redraw_stuff();


			/* Hack -- Decrease "xtra" depth */
			Misc.character_xtra--;


			/* Update stuff */
			Misc.p_ptr.update |= (Misc.PU_BONUS | Misc.PU_HP | Misc.PU_MANA | Misc.PU_SPELLS);

			/* Combine / Reorder the pack */
			Misc.p_ptr.notice |= (uint)(Misc.PN_COMBINE | Misc.PN_REORDER | Misc.PN_SORT_QUIVER);

			/* Make basic mouse buttons */
			Button.button_add("[ESC]", (char)keycode_t.ESCAPE);
			Button.button_add("[Ret]", '\r');
			Button.button_add("[Spc]", ' ');
			Button.button_add("[Rpt]", 'n');
			Button.button_add("[Std]", ',');

			/* Redraw buttons */
			Misc.p_ptr.redraw |= (Misc.PR_BUTTONS);

			/* Notice stuff */
			Misc.p_ptr.notice_stuff();

			/* Update stuff */
			Misc.p_ptr.update_stuff();

			/* Redraw stuff */
			Misc.p_ptr.redraw_stuff();

			/* Refresh */
			Term.fresh();

			/* Handle delayed death */
			if (Misc.p_ptr.is_dead) return;

			/* Announce (or repeat) the feeling */
			//Nick: May not need this, looks like it just repeats the town feeling. 
			//Misc.p_ptr.update_stuff(); above already does this...
			if (Misc.p_ptr.depth == 0) Command.display_feeling(false); 

			/* Give player minimum energy to start a new level, but do not reduce higher value from savefile for level in progress */
			if (Misc.p_ptr.energy < Misc.INITIAL_DUNGEON_ENERGY)
				Misc.p_ptr.energy = Misc.INITIAL_DUNGEON_ENERGY;


			/*** Process this dungeon level ***/

			/* Main loop */
			while (true)
			{
				/* Hack -- Compact the monster list occasionally */
				if (Cave.cave_monster_count(Cave.cave) + 32 > Misc.z_info.m_max) Monster_Make.compact_monsters(64);

				/* Hack -- Compress the monster list occasionally */
				if (Cave.cave_monster_count(Cave.cave) + 32 < Cave.cave_monster_max(Cave.cave)) Monster_Make.compact_monsters(0);

				/* Hack -- Compact the object list occasionally */
				if(Misc.o_cnt + 32 > Misc.z_info.o_max) {
					throw new NotImplementedException();
					//compact_objects(64);
				}

				/* Hack -- Compress the object list occasionally */
				if(Misc.o_cnt + 32 < Misc.o_max) {
					throw new NotImplementedException();
					//compact_objects(0);
				}

				/* Can the player move? */
				while ((Misc.p_ptr.energy >= 100) && !Misc.p_ptr.leaving)
				{
					/* Do any necessary animations */
					do_animation();

					/* process monster with even more energy first */
					Monster.Monster.process_monsters(c, (byte)(Misc.p_ptr.energy + 1));

					/* if still alive */
					if (!Misc.p_ptr.leaving)
					{
					        /* Mega hack -redraw big graphics - sorry NRM */
					        if ((Term.tile_width > 1) || (Term.tile_height > 1)) 
					            Misc.p_ptr.redraw |= (Misc.PR_MAP);

					    /* Process the player */
					    process_player();
					}
				}

				/* Notice stuff */
				if (Misc.p_ptr.notice != 0) Misc.p_ptr.notice_stuff();

				/* Update stuff */
				if (Misc.p_ptr.update != 0) Misc.p_ptr.update_stuff();

				/* Redraw stuff */
				if (Misc.p_ptr.redraw != 0) Misc.p_ptr.redraw_stuff();

				/* Hack -- Highlight the player */
				Cave.move_cursor_relative(Misc.p_ptr.py, Misc.p_ptr.px);

				/* Handle "leaving" */
				if (Misc.p_ptr.leaving) break;


				/* Process all of the monsters */
				Monster.Monster.process_monsters(c, 100);

				/* Notice stuff */
				if (Misc.p_ptr.notice != 0) Misc.p_ptr.notice_stuff();

				/* Update stuff */
				if (Misc.p_ptr.update != 0) Misc.p_ptr.update_stuff();

				/* Redraw stuff */
				if (Misc.p_ptr.redraw != 0) Misc.p_ptr.redraw_stuff();

				/* Hack -- Highlight the player */
				Cave.move_cursor_relative(Misc.p_ptr.py, Misc.p_ptr.px);

				/* Handle "leaving" */
				if (Misc.p_ptr.leaving) break;


				/* Process the world */
				process_world(c);

				/* Notice stuff */
				if (Misc.p_ptr.notice != 0) Misc.p_ptr.notice_stuff();

				/* Update stuff */
				if (Misc.p_ptr.update != 0) Misc.p_ptr.update_stuff();

				/* Redraw stuff */
				if (Misc.p_ptr.redraw != 0) Misc.p_ptr.redraw_stuff();

				/* Hack -- Highlight the player */
				Cave.move_cursor_relative(Misc.p_ptr.py, Misc.p_ptr.px);

				/* Handle "leaving" */
				if (Misc.p_ptr.leaving) break;

				/*** Apply energy ***/

				/* Give the player some energy */
				Misc.p_ptr.energy += Misc.extract_energy[Misc.p_ptr.state.speed];

				/* Give energy to all monsters */
				for (i = Cave.cave_monster_max(Cave.cave) - 1; i >= 1; i--)
				{
					int mspeed;

					/* Access the monster */
					m_ptr = Cave.cave_monster(Cave.cave, i);

					/* Ignore "dead" monsters */
					if (m_ptr.r_idx == 0) continue;

					/* Calculate the net speed */
					mspeed = m_ptr.mspeed;
					if (m_ptr.m_timed[(int)Misc.MON_TMD.FAST] != 0)
						mspeed += 10;
					if (m_ptr.m_timed[(int)Misc.MON_TMD.SLOW] != 0)
						mspeed -= 10;

					/* Give this monster some energy */
					m_ptr.energy += Misc.extract_energy[mspeed];
				}

				/* Count game turns */
				Misc.turn++;
			}
		}

		/*
		 * Process the player
		 *
		 * Notice the annoying code to handle "pack overflow", which
		 * must come first just in case somebody manages to corrupt
		 * the savefiles by clever use of menu commands or something.
		 *
		 * Notice the annoying code to handle "monster memory" changes,
		 * which allows us to avoid having to update the window flags
		 * every time we change any internal monster memory field, and
		 * also reduces the number of times that the recall window must
		 * be redrawn.
		 *
		 * Note that the code to check for user abort during repeated commands
		 * and running and resting can be disabled entirely with an option, and
		 * even if not disabled, it will only check during every 128th game turn
		 * while resting, for efficiency.
		 */
		static void process_player()
		{
			int i;

			/*** Check for interrupts ***/

			/* Complete resting */
			if (Misc.p_ptr.resting < 0)
			{
			    /* Basic resting */
			    if (Misc.p_ptr.resting == (int)Misc.REST.ALL_POINTS)
			    {
			        /* Stop resting */
			        if ((Misc.p_ptr.chp == Misc.p_ptr.mhp) && (Misc.p_ptr.csp == Misc.p_ptr.msp))
			        {
			            Cave.disturb(Misc.p_ptr, 0, 0);
			        }
			    }

			    /* Complete resting */
			    else if (Misc.p_ptr.resting == (int)Misc.REST.COMPLETE)
			    {
			        /* Stop resting */
			        if ((Misc.p_ptr.chp == Misc.p_ptr.mhp) &&
			            (Misc.p_ptr.csp == Misc.p_ptr.msp) &&
			            Misc.p_ptr.timed[(int)Timed_Effect.BLIND] == 0 && Misc.p_ptr.timed[(int)Timed_Effect.CONFUSED] == 0 &&
			            Misc.p_ptr.timed[(int)Timed_Effect.POISONED] == 0 && Misc.p_ptr.timed[(int)Timed_Effect.AFRAID] == 0 &&
			            Misc.p_ptr.timed[(int)Timed_Effect.TERROR] == 0 &&
			            Misc.p_ptr.timed[(int)Timed_Effect.STUN] == 0 && Misc.p_ptr.timed[(int)Timed_Effect.CUT] == 0 &&
			            Misc.p_ptr.timed[(int)Timed_Effect.SLOW] == 0 && Misc.p_ptr.timed[(int)Timed_Effect.PARALYZED] == 0 &&
			            Misc.p_ptr.timed[(int)Timed_Effect.IMAGE] == 0 && Misc.p_ptr.word_recall == 0)
			        {
			            Cave.disturb(Misc.p_ptr, 0, 0);
			        }
			    }
		
			    /* Rest until HP or SP are filled */
			    else if (Misc.p_ptr.resting == (int)Misc.REST.SOME_POINTS)
			    {
			        /* Stop resting */
			        if ((Misc.p_ptr.chp == Misc.p_ptr.mhp) || (Misc.p_ptr.csp == Misc.p_ptr.msp))
			        {
			            Cave.disturb(Misc.p_ptr, 0, 0);
			        }
			    }
			}

			/* Check for "player abort" */
			if (Misc.p_ptr.running != 0 || Game_Command.get_nrepeats() > 0 || (Misc.p_ptr.resting != 0 && ((Misc.turn & 0x7F) == 0)))
			{
			    ui_event e;

				/* Do not wait */
				Utilities.inkey_scan = Misc.SCAN_INSTANT;

				/* Check for a key */
				e = Utilities.inkey_ex();
				if (e.type != ui_event_type.EVT_NONE) {
				    /* Flush and disturb */
				    Utilities.flush();
				    Cave.disturb(Misc.p_ptr, 0, 0);
				    Utilities.msg("Cancelled.");
				}
			}


			/*** Handle actual user input ***/

			/* Repeat until energy is reduced */
			do
			{
			    /* Notice stuff (if needed) */
			    if (Misc.p_ptr.notice != 0) Misc.p_ptr.notice_stuff();

			    /* Update stuff (if needed) */
			    if (Misc.p_ptr.update != 0) Misc.p_ptr.update_stuff();

			    /* Redraw stuff (if needed) */
			    if (Misc.p_ptr.redraw != 0) Misc.p_ptr.redraw_stuff();


			    /* Place the cursor on the player */
			    Cave.move_cursor_relative(Misc.p_ptr.py, Misc.p_ptr.px);

			    /* Refresh (optional) */
			    Term.fresh();

			    /* Hack -- Pack Overflow */
			    Object.Object.pack_overflow();

			    /* Assume free turn */
			    Misc.p_ptr.energy_use = 0;

			    /* Dwarves detect treasure */
			    if (Misc.p_ptr.player_has(Misc.PF.SEE_ORE.value))
			    {
			        /* Only if they are in good shape */
			        if (	Misc.p_ptr.timed[(int)Timed_Effect.IMAGE] == 0 &&
			                Misc.p_ptr.timed[(int)Timed_Effect.CONFUSED] == 0 &&
			                Misc.p_ptr.timed[(int)Timed_Effect.AMNESIA] == 0 &&
			                Misc.p_ptr.timed[(int)Timed_Effect.STUN] == 0 &&
			                Misc.p_ptr.timed[(int)Timed_Effect.PARALYZED] == 0 &&
			                Misc.p_ptr.timed[(int)Timed_Effect.TERROR] == 0 &&
			                Misc.p_ptr.timed[(int)Timed_Effect.AFRAID] == 0)
						throw new NotImplementedException();
			            //detect_close_buried_treasure();
			    }

			    /* Paralyzed or Knocked Out */
			    if ((Misc.p_ptr.timed[(int)Timed_Effect.PARALYZED] != 0) || (Misc.p_ptr.timed[(int)Timed_Effect.STUN] >= 100))
			    {
			        /* Take a turn */
			        Misc.p_ptr.energy_use = 100;
			    }

			    /* Picking up objects */
			    else if ((Misc.p_ptr.notice & Misc.PN_PICKUP) != 0)
			    {
					Misc.p_ptr.energy_use = (short)(Command.do_autopickup() * 10);
					if (Misc.p_ptr.energy_use > 100)
					    Misc.p_ptr.energy_use = 100;
					Misc.p_ptr.notice &= ~(Misc.PN_PICKUP);
			
					/* Appropriate time for the player to see objects */
					Game_Event.signal(Game_Event.Event_Type.SEEFLOOR);
			    }

			    /* Resting */
			    else if (Misc.p_ptr.resting != 0)
			    {
			        /* Timed rest */
			        if (Misc.p_ptr.resting > 0)
			        {
			            /* Reduce rest count */
			            Misc.p_ptr.resting--;

			            /* Redraw the state */
			            Misc.p_ptr.redraw |= (Misc.PR_STATE);
			        }

			        /* Take a turn */
			        Misc.p_ptr.energy_use = 100;

			        /* Increment the resting counter */
			        Misc.p_ptr.resting_turn++;
			    }

			    /* Running */
			    else if (Misc.p_ptr.running != 0)
			    {
			        /* Take a step */
			        Pathfind.run_step(0);
			    }

			    /* Repeated command */
			    else if (Game_Command.get_nrepeats() > 0)
			    {
					/* Hack -- Assume messages were seen */
					Term.msg_flag = false;

					/* Clear the top line */
					Utilities.prt("", 0, 0);

					/* Process the command */
					Game_Command.process_command(cmd_context.CMD_GAME, true);
			    }

			    /* Normal command */
			    else
			    {
					/* Check monster recall */
					process_player_aux();

					/* Place the cursor on the player */
					Cave.move_cursor_relative(Misc.p_ptr.py, Misc.p_ptr.px);

					/* Get and process a command */
					Game_Command.process_command(cmd_context.CMD_GAME, false);

					/* Mega hack - redraw if big graphics - sorry NRM */
					if ((Term.tile_width > 1) || (Term.tile_height > 1)) 
					        Misc.p_ptr.redraw |= (Misc.PR_MAP);
			    }


			    /*** Clean up ***/

			    /* Significant */
			    if (Misc.p_ptr.energy_use != 0)
			    {
			        /* Use some energy */
			        Misc.p_ptr.energy -= Misc.p_ptr.energy_use;

			        /* Increment the total energy counter */
			        Misc.p_ptr.total_energy += (uint)Misc.p_ptr.energy_use;

			        /* Hack -- constant hallucination */
			        if (Misc.p_ptr.timed[(int)Timed_Effect.IMAGE] != 0)
			        {
			            Misc.p_ptr.redraw |= (Misc.PR_MAP);
			        }

			        /* Shimmer multi-hued monsters */
			        for (i = 1; i < Cave.cave_monster_max(Cave.cave); i++)
			        {
			            Monster_Race race;
			            Monster.Monster mon = Cave.cave_monster(Cave.cave, i);
			            if (mon.r_idx == 0)
			                continue;
			            race = Misc.r_info[mon.r_idx];
			            if (race.flags.has(Monster_Flag.ATTR_MULTI.value))
			                continue;
			            Cave.cave_light_spot(Cave.cave, mon.fy, mon.fx);
			        }

			        /* Clear NICE flag, and show marked monsters */
			        for (i = 1; i < Cave.cave_monster_max(Cave.cave); i++)
			        {
			            Monster.Monster mon = Cave.cave_monster(Cave.cave, i);
			            mon.mflag &= ~Monster_Flag.MFLAG_NICE;
			            if ((mon.mflag & Monster_Flag.MFLAG_MARK) != 0) {
			                if ((mon.mflag & Monster_Flag.MFLAG_SHOW) == 0) {
			                    mon.mflag &= ~Monster_Flag.MFLAG_MARK;
			                    throw new NotImplementedException();
								//update_mon(i, false);
			                }
			            }
			        }
			    }

			    /* Clear SHOW flag */
			    for (i = 1; i < Cave.cave_monster_max(Cave.cave); i++)
			    {
			        Monster.Monster mon = Cave.cave_monster(Cave.cave, i);
			        mon.mflag &= ~Monster_Flag.MFLAG_SHOW;
			    }

			    /* HACK: This will redraw the itemlist too frequently, but I'm don't
			       know all the individual places it should go. */
			    Misc.p_ptr.redraw |= Misc.PR_ITEMLIST;
			}

			while (Misc.p_ptr.energy_use == 0 && !Misc.p_ptr.leaving);

			/* Notice stuff (if needed) */
			if (Misc.p_ptr.notice != 0) Misc.p_ptr.notice_stuff();
		}

		/*
		 * Handle certain things once every 10 game turns
		 */
		static void process_world(Cave c)
		{
			int i;

			int regen_amount;

			Object.Object o_ptr;

			/* Every 10 game turns */
			if ((Misc.turn % 10) != 0) return;


			/*** Check the Time ***/

			/* Play an ambient sound at regular intervals. */
			if ((Misc.turn % ((10L * Cave.TOWN_DAWN) / 4)) == 0)
			{
			    //play_ambient_sound(); TODO: enable sound
			}

			/*** Handle the "town" (stores and sunshine) ***/

			/* While in town */
			if (Misc.p_ptr.depth == 0)
			{
			    /* Hack -- Daybreak/Nighfall in town */
			    if ((Misc.turn % ((10L * Cave.TOWN_DAWN) / 2)) == 0)
			    {
			        bool dawn;

			        /* Check for dawn */
			        dawn = ((Misc.turn % (10L * Cave.TOWN_DAWN)) == 0);

			        /* Day breaks */
			        if (dawn)
			            Utilities.msg("The sun has risen.");

			        /* Night falls */
			        else
			            Utilities.msg("The sun has fallen.");

			        /* Illuminate */
			        Cave.cave_illuminate(c, dawn);
			    }
			}


			/* While in the dungeon */
			else
			{
			    /* Update the stores once a day (while in the dungeon).
			       The changes are not actually made until return to town,
			       to avoid giving details away in the knowledge menu. */
			    if ((Misc.turn % (10L * Store.TURNS)) == 0) Misc.daycount++;
			}


			/*** Process the monsters ***/

			/* Check for creature generation */
			if (Random.one_in_(Misc.MAX_M_ALLOC_CHANCE))
			{
			    /* Make a new monster */
			    Monster_Make.pick_and_place_distant_monster(Cave.cave, new Loc(Misc.p_ptr.px, Misc.p_ptr.py), 
					Misc.MAX_SIGHT + 5, false, Misc.p_ptr.depth);
			}

			/* Hack -- Check for creature regeneration */
			if ((Misc.turn % 100) == 0) regen_monsters();


			/*** Damage over Time ***/

			/* Take damage from poison */
			if (Misc.p_ptr.timed[(int)Timed_Effect.POISONED] != 0)
			{
			    /* Take damage */
			    Spell.take_hit(Misc.p_ptr, 1, "poison");
			}

			/* Take damage from cuts */
			if (Misc.p_ptr.timed[(int)Timed_Effect.CUT] != 0)
			{
			    /* Mortal wound or Deep Gash */
			    if (Misc.p_ptr.timed[(int)Timed_Effect.CUT] > 200)
			        i = 3;

			    /* Severe cut */
			    else if (Misc.p_ptr.timed[(int)Timed_Effect.CUT] > 100)
			        i = 2;

			    /* Other cuts */
			    else
			        i = 1;

			    /* Take damage */
			    Spell.take_hit(Misc.p_ptr, i, "a fatal wound");
			}


			/*** Check the Food, and Regenerate ***/

			/* Digest normally */
			if (Misc.p_ptr.food < Misc.PY_FOOD_MAX)
			{
			    /* Every 100 game turns */
			    if ((Misc.turn % 100) == 0)
			    {
			        /* Basic digestion rate based on speed */
			        i = Misc.extract_energy[Misc.p_ptr.state.speed] * 2;

			        /* Regeneration takes more food */
			        if (Misc.p_ptr.check_state(Object_Flag.REGEN, Misc.p_ptr.state.flags)) i += 30;

			        /* Slow digestion takes less food */
			        if (Misc.p_ptr.check_state(Object_Flag.SLOW_DIGEST, Misc.p_ptr.state.flags)) i -= 10;

			        /* Minimal digestion */
			        if (i < 1) i = 1;

			        /* Digest some food */
			        Misc.p_ptr.set_food(Misc.p_ptr.food - i);
			    }
			}

			/* Digest quickly when gorged */
			else
			{
			    /* Digest a lot of food */
			    Misc.p_ptr.set_food(Misc.p_ptr.food - 100);
			}

			/* Getting Faint */
			if (Misc.p_ptr.food < Misc.PY_FOOD_FAINT)
			{
			    /* Faint occasionally */
			    if (Misc.p_ptr.timed[(int)Timed_Effect.PARALYZED] == 0 && Random.one_in_(10))
			    {
			        /* Message */
			        Utilities.msg("You faint from the lack of food.");
			        Cave.disturb(Misc.p_ptr, 1, 0);

			        /* Faint (bypass free action) */
			        Misc.p_ptr.inc_timed(Timed_Effect.PARALYZED, 1 + Random.randint0(5), true, false);
			    }
			}


			/* Starve to death (slowly) */
			if (Misc.p_ptr.food < Misc.PY_FOOD_STARVE)
			{
			    /* Calculate damage */
			    i = (Misc.PY_FOOD_STARVE - Misc.p_ptr.food) / 10;

			    /* Take damage */
			    Spell.take_hit(Misc.p_ptr, i, "starvation");
			}

			/** Regenerate HP **/

			/* Default regeneration */
			if (Misc.p_ptr.food >= Misc.PY_FOOD_WEAK)
			    regen_amount = Misc.PY_REGEN_NORMAL;
			else if (Misc.p_ptr.food < Misc.PY_FOOD_STARVE)
			    regen_amount = 0;
			else if (Misc.p_ptr.food < Misc.PY_FOOD_FAINT)
			    regen_amount = Misc.PY_REGEN_FAINT;
			else /* if (p_ptr.food < PY_FOOD_WEAK) */
			    regen_amount = Misc.PY_REGEN_WEAK;

			/* Various things speed up regeneration */
			if (Misc.p_ptr.check_state(Object_Flag.REGEN, Misc.p_ptr.state.flags))
			    regen_amount *= 2;
			if (Misc.p_ptr.searching != 0 || Misc.p_ptr.resting != 0)
			    regen_amount *= 2;

			/* Some things slow it down */
			if (Misc.p_ptr.check_state(Object_Flag.IMPAIR_HP, Misc.p_ptr.state.flags))
			    regen_amount /= 2;

			/* Various things interfere with physical healing */
			if (Misc.p_ptr.timed[(int)Timed_Effect.PARALYZED] != 0) regen_amount = 0;
			if (Misc.p_ptr.timed[(int)Timed_Effect.POISONED] != 0) regen_amount = 0;
			if (Misc.p_ptr.timed[(int)Timed_Effect.STUN] != 0) regen_amount = 0;
			if (Misc.p_ptr.timed[(int)Timed_Effect.CUT] != 0) regen_amount = 0;

			/* Regenerate Hit Points if needed */
			if (Misc.p_ptr.chp < Misc.p_ptr.mhp)
			    regenhp(regen_amount);


			/** Regenerate SP **/

			/* Default regeneration */
			regen_amount = Misc.PY_REGEN_NORMAL;

			/* Various things speed up regeneration */
			if (Misc.p_ptr.check_state(Object_Flag.REGEN, Misc.p_ptr.state.flags))
			    regen_amount *= 2;
			if (Misc.p_ptr.searching != 0 || Misc.p_ptr.resting != 0)
			    regen_amount *= 2;

			/* Some things slow it down */
			if (Misc.p_ptr.check_state(Object_Flag.IMPAIR_MANA, Misc.p_ptr.state.flags))
			    regen_amount /= 2;

			/* Regenerate mana */
			if (Misc.p_ptr.csp < Misc.p_ptr.msp)
			    regenmana(regen_amount);



			/*** Timeout Various Things ***/

			decrease_timeouts();



			/*** Process Light ***/

			/* Check for light being wielded */
			o_ptr = Misc.p_ptr.inventory[Misc.INVEN_LIGHT];

			/* Burn some fuel in the current light */
			if (o_ptr.tval == TVal.TV_LIGHT)
			{
			    Bitflag f = new Bitflag(Object_Flag.SIZE);
			    bool burn_fuel = true;

			    /* Get the object flags */
				o_ptr.object_flags(ref f);

			    /* Turn off the wanton burning of light during the day in the town */
			    if (Misc.p_ptr.depth == 0 && ((Misc.turn % (10L * Cave.TOWN_DAWN)) < ((10L * Cave.TOWN_DAWN) / 2)))
			        burn_fuel = false;

			    /* If the light has the NO_FUEL flag, well... */
			    if (f.has(Object_Flag.NO_FUEL.value))
			        burn_fuel = false;

			    /* Use some fuel (except on artifacts, or during the day) */
			    if (burn_fuel && o_ptr.timeout > 0)
			    {
			        /* Decrease life-span */
			        o_ptr.timeout--;

			        /* Hack -- notice interesting fuel steps */
			        if ((o_ptr.timeout < 100) || ((o_ptr.timeout % 100) == 0))
			        {
			            /* Redraw stuff */
			            Misc.p_ptr.redraw |= (Misc.PR_EQUIP);
			        }

			        /* Hack -- Special treatment when blind */
			        if (Misc.p_ptr.timed[(int)Timed_Effect.BLIND] != 0)
			        {
			            /* Hack -- save some light for later */
			            if (o_ptr.timeout == 0) o_ptr.timeout++;
			        }

			        /* The light is now out */
			        else if (o_ptr.timeout == 0)
			        {
			            Cave.disturb(Misc.p_ptr, 0, 0);
			            Utilities.msg("Your light has gone out!");
			        }

			        /* The light is getting dim */
			        else if ((o_ptr.timeout < 100) && ((o_ptr.timeout % 10)) == 0)
			        {
			            Cave.disturb(Misc.p_ptr, 0, 0);
			            Utilities.msg("Your light is growing faint.");
			        }
			    }
			}

			/* Calculate torch radius */
			Misc.p_ptr.update |= (Misc.PU_TORCH);


			/*** Process Inventory ***/

			/* Handle experience draining */
			if (Misc.p_ptr.check_state(Object_Flag.DRAIN_EXP, Misc.p_ptr.state.flags))
			{
				throw new NotImplementedException();
				//if ((Misc.p_ptr.exp > 0) && Random.one_in_(10))
				//    player_exp_lose(Misc.p_ptr, 1, false);

				//wieldeds_notice_flag(Misc.p_ptr, Object_Flag.DRAIN_EXP);
			}

			/* Recharge activatable objects and rods */
			recharge_objects();

			/* Feel the inventory */
			Object.Object.sense_inventory();


			/*** Involuntary Movement ***/

			/* Random teleportation */
			if (Misc.p_ptr.check_state(Object_Flag.TELEPORT, Misc.p_ptr.state.flags) && Random.one_in_(100))
			{
				throw new NotImplementedException();
				//wieldeds_notice_flag(Misc.p_ptr, OF_TELEPORT);
				//teleport_player(40);
				//Cave.disturb(Misc.p_ptr, 0, 0);
			}

			/* Delayed Word-of-Recall */
			if (Misc.p_ptr.word_recall != 0)
			{
			    /* Count down towards recall */
			    Misc.p_ptr.word_recall--;

			    /* Activate the recall */
			    if (Misc.p_ptr.word_recall == 0)
			    {
			        /* Disturbing! */
			        Cave.disturb(Misc.p_ptr, 0, 0);

					throw new NotImplementedException();
					///* Determine the level */
					//if (Misc.p_ptr.depth)
					//{
					//    msgt(MSG_TPLEVEL, "You feel yourself yanked upwards!");
					//    dungeon_change_level(0);
					//}
					//else
					//{
					//    msgt(MSG_TPLEVEL, "You feel yourself yanked downwards!");

					//    /* New depth - back to max depth or 1, whichever is deeper */
					//    dungeon_change_level(Misc.p_ptr.max_depth < 1 ? 1: Misc.p_ptr.max_depth);
					//}
			    }
			}
		}

		/*
		 * Regenerate the monsters (once per 100 game turns)
		 *
		 * XXX XXX XXX Should probably be done during monster turns.
		 */
		static void regen_monsters()
		{
			int i, frac;

			/* Regenerate everyone */
			for (i = 1; i < Cave.cave_monster_max(Cave.cave); i++)
			{
			    /* Check the i'th monster */
			    Monster.Monster m_ptr = Cave.cave_monster(Cave.cave, i);
			    Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			    /* Skip dead monsters */
			    if (m_ptr.r_idx == 0) continue;

			    /* Allow regeneration (if needed) */
			    if (m_ptr.hp < m_ptr.maxhp)
			    {
			        /* Hack -- Base regeneration */
			        frac = m_ptr.maxhp / 100;

			        /* Hack -- Minimal regeneration rate */
			        if (frac == 0) frac = 1;

			        /* Hack -- Some monsters regenerate quickly */
			        if (r_ptr.flags.has(Monster_Flag.REGENERATE.value)) frac *= 2;

			        /* Hack -- Regenerate */
			        m_ptr.hp += (short)frac;

			        /* Do not over-regenerate */
			        if (m_ptr.hp > m_ptr.maxhp) m_ptr.hp = m_ptr.maxhp;

			        /* Redraw (later) if needed */
			        if (Misc.p_ptr.health_who == i) Misc.p_ptr.redraw |= (Misc.PR_HEALTH);
			    }
			}
		}

		/*
		 * This animates monsters and/or items as necessary.
		 */
		static void do_animation()
		{
			int i;

			for (i = 1; i < Cave.cave_monster_max(Cave.cave); i++)
			{
			    ConsoleColor attr;

			    Monster.Monster m_ptr = Cave.cave_monster(Cave.cave, i);
				if (m_ptr == null || m_ptr.ml)
			        continue;

			    Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			    if (r_ptr.flags.has(Monster_Flag.ATTR_MULTI.value))
			        attr = Utilities.num_to_attr(Random.randint1(16 - 1)); //BASIC_COLORS - 1, so, size of ConsoleColors...
			    else if (r_ptr.flags.has(Monster_Flag.ATTR_FLICKER.value))
			        attr = get_flicker(r_ptr.x_attr);
			    else
			        continue;

			    m_ptr.attr = attr;
			    Misc.p_ptr.redraw |= (Misc.PR_MAP | Misc.PR_MONLIST);
			}
			flicker++;
		}

		static byte flicker = 0;
		/*static byte[,] color_flicker = new byte[BASIC_COLORS,3] //Basic colors should be size of ConsoleColors...
		{
			{TERM_DARK, TERM_L_DARK, TERM_L_RED},
			{TERM_WHITE, TERM_L_WHITE, TERM_L_BLUE},
			{TERM_SLATE, TERM_WHITE, TERM_L_DARK},
			{TERM_ORANGE, TERM_YELLOW, TERM_L_RED},
			{TERM_RED, TERM_L_RED, TERM_L_PINK},
			{TERM_GREEN, TERM_L_GREEN, TERM_L_TEAL},
			{TERM_BLUE, TERM_L_BLUE, TERM_SLATE},
			{TERM_UMBER, TERM_L_UMBER, TERM_MUSTARD},
			{TERM_L_DARK, TERM_SLATE, TERM_L_VIOLET},
			{TERM_WHITE, TERM_SLATE, TERM_L_WHITE},
			{TERM_L_PURPLE, TERM_PURPLE, TERM_L_VIOLET},
			{TERM_YELLOW, TERM_L_YELLOW, TERM_MUSTARD},
			{TERM_L_RED, TERM_RED, TERM_L_PINK},
			{TERM_L_GREEN, TERM_L_TEAL, TERM_GREEN},
			{TERM_L_BLUE, TERM_DEEP_L_BLUE, TERM_BLUE_SLATE},
			{TERM_L_UMBER, TERM_UMBER, TERM_MUD},
			{TERM_PURPLE, TERM_VIOLET, TERM_MAGENTA},
			{TERM_VIOLET, TERM_L_VIOLET, TERM_MAGENTA},
			{TERM_TEAL, TERM_L_TEAL, TERM_L_GREEN},
			{TERM_MUD, TERM_YELLOW, TERM_UMBER},
			{TERM_L_YELLOW, TERM_WHITE, TERM_L_UMBER},
			{TERM_MAGENTA, TERM_L_PINK, TERM_L_RED},
			{TERM_L_TEAL, TERM_L_WHITE, TERM_TEAL},
			{TERM_L_VIOLET, TERM_L_PURPLE, TERM_VIOLET},
			{TERM_L_PINK, TERM_L_RED, TERM_L_WHITE},
			{TERM_MUSTARD, TERM_YELLOW, TERM_UMBER},
			{TERM_BLUE_SLATE, TERM_BLUE, TERM_SLATE},
			{TERM_DEEP_L_BLUE, TERM_L_BLUE, TERM_BLUE},
		};*/

		static ConsoleColor get_flicker(ConsoleColor a)
		{
			throw new NotImplementedException();
			//switch(flicker % 3)
			//{
			//    case 1: return color_flicker[a, 1];
			//    case 2: return color_flicker[a, 2];
			//}
			//return a;
		}

		/*
		 * Hack -- helper function for "process_player()"
		 *
		 * Check for changes in the "monster memory"
		 */
		static int old_monster_race_idx = 0;
		static Bitflag old_flags = new Bitflag(Monster_Flag.SIZE);
		static Bitflag old_spell_flags = new Bitflag(Monster_Spell_Flag.SIZE);

		static byte[] old_blows = new byte[Monster_Blow.MONSTER_BLOW_MAX];

		static byte	old_cast_innate = 0;
		static byte	old_cast_spell = 0;
		static void process_player_aux()
		{
			int i;
			bool changed = false;

			/* Tracking a monster */
			if (Misc.p_ptr.monster_race_idx != 0)
			{
				/* Get the monster lore */
				Monster_Lore l_ptr = Misc.l_list[Misc.p_ptr.monster_race_idx];

				for (i = 0; i < Monster_Blow.MONSTER_BLOW_MAX; i++)
				{
				    if (old_blows[i] != l_ptr.blows[i])
				    {
				        changed = true;
				        break;
				    }
				}

				/* Check for change of any kind */
				if (changed ||
				    (old_monster_race_idx != Misc.p_ptr.monster_race_idx) ||
				    !old_flags.is_equal(l_ptr.flags) ||
				    !old_spell_flags.is_equal(l_ptr.spell_flags) ||
				    (old_cast_innate != l_ptr.cast_innate) ||
				    (old_cast_spell != l_ptr.cast_spell))
				{
				    /* Memorize old race */
				    old_monster_race_idx = Misc.p_ptr.monster_race_idx;

				    /* Memorize flags */
				    old_flags.copy(l_ptr.flags);
				    old_spell_flags.copy(l_ptr.spell_flags);

				    /* Memorize blows */
				    //memmove(old_blows, l_ptr.blows, sizeof(byte)*MONSTER_BLOW_MAX);
					l_ptr.blows = old_blows;

				    /* Memorize castings */
				    old_cast_innate = l_ptr.cast_innate;
				    old_cast_spell = l_ptr.cast_spell;

				    /* Redraw stuff */
				    Misc.p_ptr.redraw |= (Misc.PR_MONSTER);
				    Misc.p_ptr.redraw_stuff();
				}
			}
		}

		/*
		 * Recharge activatable objects in the player's equipment
		 * and rods in the inventory and on the ground.
		 */
		static void recharge_objects()
		{
			int i;

			bool charged = false, discharged_stack;

			Object.Object o_ptr;

			/*** Recharge equipment ***/
			for (i = Misc.INVEN_WIELD; i < Misc.INVEN_TOTAL; i++)
			{
			    /* Get the object */
			    o_ptr = Misc.p_ptr.inventory[i];

			    /* Skip non-objects */
			    if (o_ptr.kind == null) continue;

			    /* Recharge activatable objects */
			    if (o_ptr.recharge_timeout())
			    {
			        charged = true;

			        /* Message if an item recharged */
			        recharged_notice(o_ptr, true);
			    }
			}

			/* Notice changes */
			if (charged)
			{
			    /* Window stuff */
			    Misc.p_ptr.redraw |= (Misc.PR_EQUIP);
			}

			charged = false;

			/*** Recharge the inventory ***/
			for (i = 0; i < Misc.INVEN_PACK; i++)
			{
			    o_ptr = Misc.p_ptr.inventory[i];

			    /* Skip non-objects */
			    if (o_ptr.kind == null) continue;

			    discharged_stack = (o_ptr.number_charging() == o_ptr.number) ? true : false;

			    /* Recharge rods, and update if any rods are recharged */
			    if (o_ptr.tval == TVal.TV_ROD && o_ptr.recharge_timeout())
			    {
			        charged = true;

			        /* Entire stack is recharged */
			        if (o_ptr.timeout == 0)
			            recharged_notice(o_ptr, true);

			        /* Previously exhausted stack has acquired a charge */
			        else if (discharged_stack)
			            recharged_notice(o_ptr, false);
			    }
			}

			/* Notice changes */
			if (charged)
			{
			    /* Combine pack */
			    Misc.p_ptr.notice |= (Misc.PN_COMBINE);

			    /* Redraw stuff */
			    Misc.p_ptr.redraw |= (Misc.PR_INVEN);
			}

			/*** Recharge the ground ***/
			for (i = 1; i < Misc.o_max; i++)
			{
			    /* Get the object */
			    o_ptr = Object.Object.byid((short)i);

			    /* Skip dead objects */
			    if (o_ptr == null || o_ptr.kind == null) continue;

			    /* Recharge rods on the ground */
			    if (o_ptr.tval == TVal.TV_ROD)
			        o_ptr.recharge_timeout();
			}
		}

		/*
		 * Regenerate hit points
		 */
		static void regenhp(int percent)
		{
			int new_chp, new_chp_frac;
			int old_chp;

			/* Save the old hitpoints */
			old_chp = Misc.p_ptr.chp;

			/* Extract the new hitpoints */
			new_chp = (int)(((long)Misc.p_ptr.mhp) * percent + Misc.PY_REGEN_HPBASE);
			Misc.p_ptr.chp += (short)(new_chp >> 16);   /* div 65536 */

			/* check for overflow */
			if ((Misc.p_ptr.chp < 0) && (old_chp > 0)) Misc.p_ptr.chp = short.MaxValue;
			new_chp_frac = (new_chp & 0xFFFF) + Misc.p_ptr.chp_frac;	/* mod 65536 */
			if (new_chp_frac >= 0x10000L)
			{
			    Misc.p_ptr.chp_frac = (ushort)(new_chp_frac - 0x10000L);
			    Misc.p_ptr.chp++;
			}
			else
			{
			    Misc.p_ptr.chp_frac = (ushort)new_chp_frac;
			}

			/* Fully healed */
			if (Misc.p_ptr.chp >= Misc.p_ptr.mhp)
			{
			    Misc.p_ptr.chp = Misc.p_ptr.mhp;
			    Misc.p_ptr.chp_frac = 0;
			}

			/* Notice changes */
			if (old_chp != Misc.p_ptr.chp)
			{
				/* Redraw */
				Misc.p_ptr.redraw |= (Misc.PR_HP);
				Object.Object.wieldeds_notice_flag(Misc.p_ptr, Object_Flag.REGEN.value);
				Object.Object.wieldeds_notice_flag(Misc.p_ptr, Object_Flag.IMPAIR_HP.value);
			}
		}


		/*
		 * Regenerate mana points
		 */
		static void regenmana(int percent)
		{
			throw new NotImplementedException();
			//s32b new_mana, new_mana_frac;
			//int old_csp;

			//old_csp = p_ptr.csp;
			//new_mana = ((long)p_ptr.msp) * percent + PY_REGEN_MNBASE;
			//p_ptr.csp += (s16b)(new_mana >> 16);	/* div 65536 */
			///* check for overflow */
			//if ((p_ptr.csp < 0) && (old_csp > 0))
			//{
			//    p_ptr.csp = MAX_SHORT;
			//}
			//new_mana_frac = (new_mana & 0xFFFF) + p_ptr.csp_frac;	/* mod 65536 */
			//if (new_mana_frac >= 0x10000L)
			//{
			//    p_ptr.csp_frac = (u16b)(new_mana_frac - 0x10000L);
			//    p_ptr.csp++;
			//}
			//else
			//{
			//    p_ptr.csp_frac = (u16b)new_mana_frac;
			//}

			///* Must set frac to zero even if equal */
			//if (p_ptr.csp >= p_ptr.msp)
			//{
			//    p_ptr.csp = p_ptr.msp;
			//    p_ptr.csp_frac = 0;
			//}

			///* Redraw mana */
			//if (old_csp != p_ptr.csp)
			//{
			//    /* Redraw */
			//    p_ptr.redraw |= (PR_MANA);
			//    wieldeds_notice_flag(p_ptr, OF_REGEN);
			//    wieldeds_notice_flag(p_ptr, OF_IMPAIR_MANA);
			//}
		}

		/*
		 * Helper for process_world -- decrement p_ptr.timed[] fields.
		 */
		static void decrease_timeouts()
		{
			int adjust = (Player.Player.adj_con_fix[Misc.p_ptr.state.stat_ind[(int)Stat.Con]] + 1);
			int i;

			/* Decrement all effects that can be done simply */
			for (i = 0; i < (int)Timed_Effect.MAX; i++)
			{
			    int decr = 1;
			    if (Misc.p_ptr.timed[i] == 0)
			        continue;

			    switch (i)
			    {
			        case (int)Timed_Effect.CUT:
			        {
			            /* Hack -- check for truly "mortal" wound */
			            decr = (Misc.p_ptr.timed[i] > 1000) ? 0 : adjust;
			            break;
			        }

			        case (int)Timed_Effect.POISONED:
			        case (int)Timed_Effect.STUN:
			        {
			            decr = adjust;
			            break;
			        }
			    }
			    /* Decrement the effect */
			    Misc.p_ptr.dec_timed((Timed_Effect)i, decr, false);
			}

			return;
		}

		/*
		 * If player has inscribed the object with "!!", let him know when it's
		 * recharged. -LM-
		 * Also inform player when first item of a stack has recharged. -HK-
		 * Notify all recharges w/o inscription if notify_recharge option set -WP-
		 */
		static void recharged_notice(Object.Object o_ptr, bool all)
		{
			throw new NotImplementedException();
			//char o_name[120];

			//const char *s;

			//bool notify = false;

			//if (OPT(notify_recharge))
			//{
			//    notify = true;
			//}
			//else if (o_ptr.note)
			//{
			//    /* Find a '!' */
			//    s = strchr(quark_str(o_ptr.note), '!');

			//    /* Process notification request */
			//    while (s)
			//    {
			//        /* Find another '!' */
			//        if (s[1] == '!')
			//        {
			//            notify = true;
			//            break;
			//        }

			//        /* Keep looking for '!'s */
			//        s = strchr(s + 1, '!');
			//    }
			//}

			//if (!notify) return;


			///* Describe (briefly) */
			//object_desc(o_name, sizeof(o_name), o_ptr, ODESC_BASE);

			///* Disturb the player */
			//disturb(p_ptr, 0, 0);

			///* Notify the player */
			//if (o_ptr.number > 1)
			//{
			//    if (all) msg("Your %s have recharged.", o_name);
			//    else msg("One of your %s has recharged.", o_name);
			//}

			///* Artifacts */
			//else if (o_ptr.artifact)
			//{
			//    msg("The %s has recharged.", o_name);
			//}

			///* Single, non-artifact items */
			//else msg("Your %s has recharged.", o_name);
		}

		/*
		 * Change dungeon level - e.g. by going up stairs or with WoR.
		 */
		public static void dungeon_change_level(int dlev)
		{
			/* New depth */
			Misc.p_ptr.depth = (short)dlev;

			/* If we're returning to town, update the store contents
			   according to how long we've been away */
			if (dlev == 0 && Misc.daycount != 0)
			{
			    if (Option.cheat_xtra.value) Utilities.msg("Updating Shops...");
			    while (Misc.daycount-- != 0)
			    {
			        int n;

			        /* Maintain each shop (except home) */
			        for (n = 0; n < (int)STORE.MAX_STORES; n++)
			        {
			            /* Skip the home */
			            if (n == (int)STORE.HOME) continue;

			            /* Maintain */
						Misc.stores[n].store_maint();
			        }

			        /* Sometimes, shuffle the shop-keepers */
			        if (Random.one_in_(Store.SHUFFLE))
			        {
			            /* Message */
			            if (Option.cheat_xtra.value) Utilities.msg("Shuffling a Shopkeeper...");

			            /* Pick a random shop (except home) */
			            while (true)
			            {
			                n = Random.randint0((int)STORE.MAX_STORES);
			                if (n != (int)STORE.HOME) break;
			            }

			            /* Shuffle it */
						Misc.stores[n].store_shuffle();
			        }
			    }
			    Misc.daycount = 0;
			    if (Option.cheat_xtra.value) Utilities.msg("Done.");
			}

			/* Leaving */
			Misc.p_ptr.leaving = true;

			/* Save the game when we arrive on the new level. */
			Misc.p_ptr.autosave = true;
		}
	}
}
