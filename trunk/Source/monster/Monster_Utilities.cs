using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;

namespace CSAngband.Monster {
	partial class Monster {
		/*
		 * This function updates the monster record of the given monster
		 *
		 * This involves extracting the distance to the player (if requested),
		 * and then checking for visibility (natural, infravision, see-invis,
		 * telepathy), updating the monster visibility flag, redrawing (or
		 * erasing) the monster when its visibility changes, and taking note
		 * of any interesting monster flags (cold-blooded, invisible, etc).
		 *
		 * Note the new "mflag" field which encodes several monster state flags,
		 * including "view" for when the monster is currently in line of sight,
		 * and "mark" for when the monster is currently visible via detection.
		 *
		 * The only monster fields that are changed here are "cdis" (the
		 * distance from the player), "ml" (visible to the player), and
		 * "mflag" (to maintain the "MFLAG_VIEW" flag).
		 *
		 * Note the special "update_monsters()" function which can be used to
		 * call this function once for every monster.
		 *
		 * Note the "full" flag which requests that the "cdis" field be updated,
		 * this is only needed when the monster (or the player) has moved.
		 *
		 * Every time a monster moves, we must call this function for that
		 * monster, and update the distance, and the visibility.  Every time
		 * the player moves, we must call this function for every monster, and
		 * update the distance, and the visibility.  Whenever the player "state"
		 * changes in certain ways ("blindness", "infravision", "telepathy",
		 * and "see invisible"), we must call this function for every monster,
		 * and update the visibility.
		 *
		 * Routines that change the "illumination" of a grid must also call this
		 * function for any monster in that grid, since the "visibility" of some
		 * monsters may be based on the illumination of their grid.
		 *
		 * Note that this function is called once per monster every time the
		 * player moves.  When the player is running, this function is one
		 * of the primary bottlenecks, along with "update_view()" and the
		 * "process_monsters()" code, so efficiency is important.
		 *
		 * Note the optimized "inline" version of the "distance()" function.
		 *
		 * A monster is "visible" to the player if (1) it has been detected
		 * by the player, (2) it is close to the player and the player has
		 * telepathy, or (3) it is close to the player, and in line of sight
		 * of the player, and it is "illuminated" by some combination of
		 * infravision, torch light, or permanent light (invisible monsters
		 * are only affected by "light" if the player can see invisible).
		 *
		 * Monsters which are not on the current panel may be "visible" to
		 * the player, and their descriptions will include an "offscreen"
		 * reference.  Currently, offscreen monsters cannot be targeted
		 * or viewed directly, but old targets will remain set.  XXX XXX
		 *
		 * The player can choose to be disturbed by several things, including
		 * "OPT(disturb_move)" (monster which is viewable moves in some way), and
		 * "OPT(disturb_near)" (monster which is "easily" viewable moves in some
		 * way).  Note that "moves" includes "appears" and "disappears".
		 */
		public static void update_mon(int m_idx, bool full)
		{
			Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);

			Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			Monster_Lore l_ptr = Misc.l_list[m_ptr.r_idx];
			if(l_ptr == null) {
				l_ptr = Misc.l_list[m_ptr.r_idx] = new Monster_Lore();
			}

			int d;

			/* Current location */
			int fy = m_ptr.fy;
			int fx = m_ptr.fx;

			/* Seen at all */
			bool flag = false;

			/* Seen by vision */
			bool easy = false;


			/* Compute distance */
			if (full)
			{
			    int py = Misc.p_ptr.py;
			    int px = Misc.p_ptr.px;

			    /* Distance components */
			    int dy = (py > fy) ? (py - fy) : (fy - py);
			    int dx = (px > fx) ? (px - fx) : (fx - px);

			    /* Approximate distance */
			    d = (dy > dx) ? (dy + (dx>>1)) : (dx + (dy>>1));

			    /* Restrict distance */
			    if (d > 255) d = 255;

			    /* Save the distance */
			    m_ptr.cdis = (byte)d;
			}

			/* Extract distance */
			else
			{
			    /* Extract the distance */
			    d = m_ptr.cdis;
			}


			/* Detected */
			if ((m_ptr.mflag & (Monster_Flag.MFLAG_MARK)) != 0) flag = true;


			/* Nearby */
			if (d <= Misc.MAX_SIGHT)
			{
			    /* Basic telepathy */
			    if (Misc.p_ptr.check_state(Object_Flag.TELEPATHY, Misc.p_ptr.state.flags))
			    {
			        /* Empty mind, no telepathy */
			        if (r_ptr.flags.has(Monster_Flag.EMPTY_MIND.value))
			        {
			            /* Nothing! */
			        }

			        /* Weird mind, occasional telepathy */
			        else if (r_ptr.flags.has(Monster_Flag.WEIRD_MIND.value))
			        {
						throw new NotImplementedException();
						///* One in ten individuals are detectable */
						//if ((m_idx % 10) == 5)
						//{
						//    /* Detectable */
						//    flag = true;

						//    /* Check for LOS so that MFLAG_VIEW is set later */
						//    if (player_has_los_bold(fy, fx)) easy = true;
						//}
			        }

			        /* Normal mind, allow telepathy */
			        else
			        {
						throw new NotImplementedException();
						///* Detectable */
						//flag = true;

						///* Check for LOS to that MFLAG_VIEW is set later */
						//if (player_has_los_bold(fy, fx)) easy = true;
			        }
			    }

			    /* Normal line of sight and player is not blind */
			    if (Cave.player_has_los_bold(fy, fx) && Misc.p_ptr.timed[(int)Timed_Effect.BLIND] == 0)
			    {
			        /* Use "infravision" */
			        if (d <= Misc.p_ptr.state.see_infra)
			        {
			            /* Learn about warm/cold blood */
			            l_ptr.flags.on(Monster_Flag.COLD_BLOOD.value);

			            /* Handle "warm blooded" monsters */
			            if (!r_ptr.flags.has(Monster_Flag.COLD_BLOOD.value))
			            {
			                /* Easy to see */
			                easy = flag = true;
			            }
			        }

			        /* See if the monster is emitting light */
			        /*if (rf_has(r_ptr.flags, RF_HAS_LIGHT)) easy = flag = true;*/

			        /* Use "illumination" */
			        if (Cave.player_can_see_bold(fy, fx))
			        {
			            /* Learn it emits light */
			            l_ptr.flags.on(Monster_Flag.HAS_LIGHT.value);

			            /* Learn about invisibility */
			            l_ptr.flags.on(Monster_Flag.INVISIBLE.value);

			            /* Handle "invisible" monsters */
			            if (r_ptr.flags.has(Monster_Flag.INVISIBLE.value))
			            {
			                /* See invisible */
			                if (Misc.p_ptr.check_state(Object_Flag.SEE_INVIS, Misc.p_ptr.state.flags))
			                {
			                    /* Easy to see */
			                    easy = flag = true;
			                }
			            }

			            /* Handle "normal" monsters */
			            else
			            {
			                /* Easy to see */
			                easy = flag = true;
			            }
			        }
			    }
			}

			/* If a mimic looks like a squelched item, it's not seen */
			if (is_mimicking(m_idx)) {
				throw new NotImplementedException();
				//object_type *o_ptr = object_byid(m_ptr.mimicked_o_idx);
				//if (squelch_item_ok(o_ptr))
				//    easy = flag = false;
			}
	
			/* The monster is now visible */
			if (flag)
			{
				/* Learn about the monster's mind */
				if (Misc.p_ptr.check_state(Object_Flag.TELEPATHY, Misc.p_ptr.state.flags))
				{
				    l_ptr.flags.set(Monster_Flag.EMPTY_MIND.value, Monster_Flag.WEIRD_MIND.value, 
						Monster_Flag.SMART.value, Monster_Flag.STUPID.value);
				}

				/* It was previously unseen */
				if (!m_ptr.ml)
				{
				    /* Mark as visible */
				    m_ptr.ml = true;

				    /* Draw the monster */
				    Cave.cave_light_spot(Cave.cave, fy, fx);

				    /* Update health bar as needed */
				    if (Misc.p_ptr.health_who == m_idx) Misc.p_ptr.redraw |= (Misc.PR_HEALTH);

				    /* Hack -- Count "fresh" sightings */
				    if (l_ptr.sights < short.MaxValue) l_ptr.sights++;

				    /* Disturb on appearance */
				    if (Option.disturb_move.value) Cave.disturb(Misc.p_ptr, 1, 0);

				    /* Window stuff */
				    Misc.p_ptr.redraw |= Misc.PR_MONLIST;
				}
			}

			/* The monster is not visible */
			else
			{
			    /* It was previously seen */
			    if (m_ptr.ml)
			    {
					/* Treat mimics differently */
					if (m_ptr.mimicked_o_idx == 0 || Squelch.item_ok(Object.Object.byid(m_ptr.mimicked_o_idx)))
					{
					    /* Mark as not visible */
					    m_ptr.ml = false;

					    /* Erase the monster */
					    Cave.cave_light_spot(Cave.cave, fy, fx);

					    /* Update health bar as needed */
					    if (Misc.p_ptr.health_who == m_idx) Misc.p_ptr.redraw |= (Misc.PR_HEALTH);

					    /* Disturb on disappearance */
					    if (Option.disturb_move.value) Cave.disturb(Misc.p_ptr, 1, 0);

					    /* Window stuff */
					    Misc.p_ptr.redraw |= Misc.PR_MONLIST;
					}
			    }
			}


			/* The monster is now easily visible */
			if (easy)
			{
				/* Change */
				if ((m_ptr.mflag & (Monster_Flag.MFLAG_VIEW)) == 0)
				{
				    /* Mark as easily visible */
				    m_ptr.mflag |= (Monster_Flag.MFLAG_VIEW);

				    /* Disturb on appearance */
				    if (Option.disturb_near.value) Cave.disturb(Misc.p_ptr, 1, 0);

				    /* Re-draw monster window */
				    Misc.p_ptr.redraw |= Misc.PR_MONLIST;
				}
			}

			/* The monster is not easily visible */
			else
			{
				/* Change */
				if ((m_ptr.mflag & (Monster_Flag.MFLAG_VIEW)) != 0)
				{
				    /* Mark as not easily visible */
				    m_ptr.mflag &= ~(Monster_Flag.MFLAG_VIEW);

				    /* Disturb on disappearance */
				    if (Option.disturb_near.value){
						Cave.disturb(Misc.p_ptr, 1, 0);
					}

				    /* Re-draw monster list window */
				    Misc.p_ptr.redraw |= Misc.PR_MONLIST;
				}
			}
		}

		/*
		 * Make a monster carry an object
		 */
		public short carry(Object.Object j_ptr)
		{
			short o_idx;

			short this_o_idx, next_o_idx = 0;

			/* Scan objects already being held for combination */
			for (this_o_idx = hold_o_idx; this_o_idx != 0; this_o_idx = next_o_idx)
			{
				Object.Object o_ptr;

				/* Get the object */
				o_ptr = Object.Object.byid(this_o_idx);

				/* Get the next object */
				next_o_idx = o_ptr.next_o_idx;

				/* Check for combination */
				if (o_ptr.similar(j_ptr, Object.Object.object_stack_t.OSTACK_MONSTER))
				{
					/* Combine the items */
					o_ptr.absorb(j_ptr);

					/* Result */
					return (this_o_idx);
				}
			}


			/* Make an object */
			o_idx = Object.Object.o_pop();

			/* Success */
			if (o_idx != 0)
			{
				//Object.Object o_ptr;

				/* Get new object */
				//o_ptr = Object.Object.byid(o_idx);

				//Nick: Set new object:
				Object.Object.set_byid(o_idx, j_ptr);

				/* Copy object */
				//o_ptr = j_ptr;

				/* Forget mark */
				j_ptr.marked = 0; //false

				/* Forget location */
				j_ptr.iy = j_ptr.ix = 0;

				/* Link the object to the monster */
				j_ptr.held_m_idx = (short)midx;

				/* Link the object to the pile */
				j_ptr.next_o_idx = hold_o_idx;

				/* Link the monster to the object */
				hold_o_idx = o_idx;
			}

			/* Result */
			return (o_idx);
		}

		/*
		 * Returns true if the given monster is currently mimicking an item.
		 */
		public static bool is_mimicking(int m_idx)
		{
			Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);

			return (m_ptr.unaware && m_ptr.mimicked_o_idx != 0);
		}

		/*
		 * This function simply updates all the (non-dead) monsters (see above).
		 */
		public static void update_monsters(bool full)
		{
			int i;

			/* Update each (live) monster */
			for (i = 1; i < Cave.cave_monster_max(Cave.cave); i++)
			{
				Monster m_ptr = Cave.cave_monster(Cave.cave, i);

				/* Skip dead monsters */
				if (m_ptr.r_idx == 0) continue;

				/* Update the monster */
				update_mon(i, full);
			}
		}


		/*
		 * Swap the players/monsters (if any) at two locations XXX XXX XXX
		 */
		public static void monster_swap(int y1, int x1, int y2, int x2)
		{
			int m1, m2;

			Monster m_ptr;

			Monster_Race r_ptr;

			/* Monsters */
			m1 = Cave.cave.m_idx[y1][x1];
			m2 = Cave.cave.m_idx[y2][x2];


			/* Update grids */
			Cave.cave.m_idx[y1][x1] = (short)m2;
			Cave.cave.m_idx[y2][x2] = (short)m1;


			/* Monster 1 */
			if (m1 > 0)
			{
			    m_ptr = Cave.cave_monster(Cave.cave, m1);

			    /* Move monster */
			    m_ptr.fy = (byte)y2;
			    m_ptr.fx = (byte)x2;

			    /* Update monster */
			    update_mon(m1, true);

			    /* Radiate light? */
			    r_ptr = Misc.r_info[m_ptr.r_idx];
			    if (r_ptr.flags.has(Monster_Flag.HAS_LIGHT.value)) Misc.p_ptr.update |= Misc.PU_UPDATE_VIEW;

			    /* Redraw monster list */
			    Misc.p_ptr.redraw |= (Misc.PR_MONLIST);
			}

			/* Player 1 */
			else if (m1 < 0)
			{
			    /* Move player */
			    Misc.p_ptr.py = (byte)y2;
			    Misc.p_ptr.px = (byte)x2;

			    /* Update the trap detection status */
			    Misc.p_ptr.redraw |= (Misc.PR_DTRAP);

			    /* Update the panel */
			    Misc.p_ptr.update |= (Misc.PU_PANEL);

			    /* Update the visuals (and monster distances) */
			    Misc.p_ptr.update |= (Misc.PU_UPDATE_VIEW | Misc.PU_DISTANCE);

			    /* Update the flow */
			    Misc.p_ptr.update |= (Misc.PU_UPDATE_FLOW);

			    /* Redraw monster list */
			    Misc.p_ptr.redraw |= (Misc.PR_MONLIST);
			}

			/* Monster 2 */
			if (m2 > 0)
			{
			    m_ptr = Cave.cave_monster(Cave.cave, m2);

			    /* Move monster */
			    m_ptr.fy = (byte)y1;
			    m_ptr.fx = (byte)x1;

			    /* Update monster */
			    update_mon(m2, true);

			    /* Radiate light? */
			    r_ptr = Misc.r_info[m_ptr.r_idx];
			    if (r_ptr.flags.has(Monster_Flag.HAS_LIGHT.value)) Misc.p_ptr.update |= Misc.PU_UPDATE_VIEW;

			    /* Redraw monster list */
			    Misc.p_ptr.redraw |= (Misc.PR_MONLIST);
			}

			/* Player 2 */
			else if (m2 < 0)
			{
			    /* Move player */
			    Misc.p_ptr.py = (byte)y1;
			    Misc.p_ptr.px = (byte)x1;

			    /* Update the trap detection status */
			    Misc.p_ptr.redraw |= (Misc.PR_DTRAP);

			    /* Update the panel */
			    Misc.p_ptr.update |= (Misc.PU_PANEL);

			    /* Update the visuals (and monster distances) */
			    Misc.p_ptr.update |= (Misc.PU_UPDATE_VIEW | Misc.PU_DISTANCE);

			    /* Update the flow */
			    Misc.p_ptr.update |= (Misc.PU_UPDATE_FLOW);

			    /* Redraw monster list */
			    Misc.p_ptr.redraw |= (Misc.PR_MONLIST);
			}


			/* Redraw */
			Cave.cave_light_spot(Cave.cave, y1, x1);
			Cave.cave_light_spot(Cave.cave, y2, x2);
		}

		/*
		 * Let the given monster attempt to reproduce.
		 *
		 * Note that "reproduction" REQUIRES empty space.
		 */
		public static bool multiply_monster(int m_idx)
		{
			throw new NotImplementedException();
			//monster_type *m_ptr = cave_monster(cave, m_idx);

			//int i, y, x;

			//bool result = false;

			///* Try up to 18 times */
			//for (i = 0; i < 18; i++)
			//{
			//    int d = 1;

			//    /* Pick a location */
			//    scatter(&y, &x, m_ptr.fy, m_ptr.fx, d, 0);

			//    /* Require an "empty" floor grid */
			//    if (!cave_empty_bold(y, x)) continue;

			//    /* Create a new monster (awake, no groups) */
			//    result = place_new_monster(cave, y, x, m_ptr.r_idx, false, false,
			//        ORIGIN_DROP_BREED);

			//    /* Done */
			//    break;
			//}

			///* Result */
			//return (result);
		}

		/*
		 * Make player fully aware of the given mimic.
		 */
		public static void become_aware(int m_idx)
		{
			throw new NotImplementedException();
			//monster_type *m_ptr = cave_monster(cave, m_idx);
			//monster_race *r_ptr = &r_info[m_ptr.r_idx];
			//monster_lore *l_ptr = &l_list[m_ptr.r_idx];

			//if(m_ptr.unaware) {
			//    m_ptr.unaware = false;

			//    /* Learn about mimicry */
			//    if (rf_has(r_ptr.flags, RF_UNAWARE))
			//        rf_on(l_ptr.flags, RF_UNAWARE);

			//    /* Delete any false items */
			//    if (m_ptr.mimicked_o_idx > 0) {
			//        object_type *o_ptr = object_byid(m_ptr.mimicked_o_idx);
			//        char o_name[80];
			//        object_desc(o_name, sizeof(o_name), o_ptr, ODESC_FULL);

			//        /* Print a message */
			//        msg("The %s was really a monster!", o_name);

			//        /* Clear the mimicry */
			//        o_ptr.mimicking_m_idx = 0;
			//        delete_object_idx(m_ptr.mimicked_o_idx);
			//        m_ptr.mimicked_o_idx = 0;
			//    }
		
			//    /* Update monster and item lists */
			//    p_ptr.update |= (PU_UPDATE_VIEW | PU_MONSTERS);
			//    p_ptr.redraw |= (PR_MONLIST | PR_ITEMLIST);
			//}
		}

		/*
		 * Learn about an "observed" resistance or other player state property, or
		 * lack of it.
		 */
		public void update_smart_learn(Player.Player p, int what)
		{
			throw new NotImplementedException();
			//monster_race *r_ptr = &r_info[m.r_idx];

			///* Sanity check */
			//if (!what) return;

			///* anything a monster might learn, the player should learn */
			//wieldeds_notice_flag(p, what);

			///* Not allowed to learn */
			//if (!OPT(birth_ai_learn)) return;

			///* Too stupid to learn anything */
			//if (rf_has(r_ptr.flags, RF_STUPID)) return;

			///* Not intelligent, only learn sometimes */
			//if (!rf_has(r_ptr.flags, RF_SMART) && one_in_(2)) return;

			///* Analyze the knowledge; fail very rarely */
			//if (check_state(p, what, p.state.flags) && !one_in_(100))
			//    of_on(m.known_pflags, what);
			//else
			//    of_off(m.known_pflags, what);
		}

		/*
		 * Mega-hack - Fix plural names of monsters
		 *
		 * Taken from PernAngband via EY, modified to fit NPP monster list
		 *
		 * Note: It should handle all regular Angband monsters.
		 */
		public static string plural_aux(string name)
		{
			//just make it return the new string
			throw new NotImplementedException();
			//int name_len = strlen(name);

			//if (strstr(name, " of "))
			//{
			//    char *aider = strstr(name, " of ");
			//    char dummy[80];
			//    int i = 0;
			//    char *ctr = name;

			//    while (ctr < aider)
			//    {
			//        dummy[i] = *ctr;
			//        ctr++;
			//        i++;
			//    }

			//    if (dummy[i - 1] == 's')
			//    {
			//        strcpy (&(dummy[i]), "es");
			//        i++;
			//    }
			//    else
			//    {
			//        strcpy (&(dummy[i]), "s");
			//    }

			//    strcpy(&(dummy[i + 1]), aider);
			//    my_strcpy(name, dummy, max);
			//}
			//else if ((strstr(name, "coins")) || (strstr(name, "gems")))
			//{
			//    char dummy[80];
			//    strcpy (dummy, "Piles of c");
			//    my_strcat (dummy, &(name[1]), sizeof(dummy));
			//    my_strcpy (name, dummy, max);
			//    return;
			//}

			//else if (strstr(name, "Greater Servant of"))
			//{
			//    char dummy[80];
			//    strcpy (dummy, "Greater Servants of ");
			//    my_strcat (dummy, &(name[1]), sizeof(dummy));
			//    my_strcpy (name, dummy, max);
			//    return;
			//}
			//else if (strstr(name, "Lesser Servant of"))
			//{
			//    char dummy[80];
			//    strcpy (dummy, "Greater Servants of ");
			//    my_strcat (dummy, &(name[1]), sizeof(dummy));
			//    my_strcpy (name, dummy, max);
			//    return;
			//}
			//else if (strstr(name, "Servant of"))
			//{
			//    char dummy[80];
			//    strcpy (dummy, "Servants of ");
			//    my_strcat (dummy, &(name[1]), sizeof(dummy));
			//    my_strcpy (name, dummy, max);
			//    return;
			//}
			//else if (strstr(name, "Great Wyrm"))
			//{
			//    char dummy[80];
			//    strcpy (dummy, "Great Wyrms ");
			//    my_strcat (dummy, &(name[1]), sizeof(dummy));
			//    my_strcpy (name, dummy, max);
			//    return;
			//}
			//else if (strstr(name, "Spawn of"))
			//{
			//    char dummy[80];
			//    strcpy (dummy, "Spawn of ");
			//    my_strcat (dummy, &(name[1]), sizeof(dummy));
			//    my_strcpy (name, dummy, max);
			//    return;
			//}
			//else if (strstr(name, "Descendant of"))
			//{
			//    char dummy[80];
			//    strcpy (dummy, "Descendant of ");
			//    my_strcat (dummy, &(name[1]), sizeof(dummy));
			//    my_strcpy (name, dummy, max);
			//    return;
			//}
			//else if ((strstr(name, "Manes")) || (name[name_len-1] == 'u') || (strstr(name, "Yeti")) ||
			//    (streq(&(name[name_len-2]), "ua")) || (streq(&(name[name_len-3]), "nee")) ||
			//    (streq(&(name[name_len-4]), "idhe")))
			//{
			//    return;
			//}
			//else if (name[name_len-1] == 'y')
			//{
			//    strcpy(&(name[name_len - 1]), "ies");
			//}
			//else if (streq(&(name[name_len - 4]), "ouse"))
			//{
			//    strcpy (&(name[name_len - 4]), "ice");
			//}
			//else if (streq(&(name[name_len - 4]), "lung"))
			//{
			//    strcpy (&(name[name_len - 4]), "lungen");
			//}
			//else if (streq(&(name[name_len - 3]), "sus"))
			//{
			//    strcpy (&(name[name_len - 3]), "si");
			//}
			//else if (streq(&(name[name_len - 4]), "star"))
			//{
			//    strcpy (&(name[name_len - 4]), "stari");
			//}
			//else if (streq(&(name[name_len - 3]), "aia"))
			//{
			//    strcpy (&(name[name_len - 3]), "aiar");
			//}
			//else if (streq(&(name[name_len - 3]), "inu"))
			//{
			//    strcpy (&(name[name_len - 3]), "inur");
			//}
			//else if (streq(&(name[name_len - 5]), "culus"))
			//{
			//    strcpy (&(name[name_len - 5]), "culi");
			//}
			//else if (streq(&(name[name_len - 4]), "sman"))
			//{
			//    strcpy (&(name[name_len - 4]), "smen");
			//}
			//else if (streq(&(name[name_len - 4]), "lman"))
			//{
			//    strcpy (&(name[name_len - 4]), "lmen");
			//}
			//else if (streq(&(name[name_len - 2]), "ex"))
			//{
			//    strcpy (&(name[name_len - 2]), "ices");
			//}
			//else if ((name[name_len - 1] == 'f') && (!streq(&(name[name_len - 2]), "ff")))
			//{
			//    strcpy (&(name[name_len - 1]), "ves");
			//}
			//else if (((streq(&(name[name_len - 2]), "ch")) || (name[name_len - 1] == 's')) &&
			//        (!streq(&(name[name_len - 5]), "iarch")))
			//{
			//    strcpy (&(name[name_len]), "es");
			//}
			//else
			//{
			//    strcpy (&(name[name_len]), "s");
			//}
		}
	}
}
