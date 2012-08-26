using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;

namespace CSAngband.Monster {
	/*
	 * Monster information, for a specific monster.
	 *
	 * Note: fy, fx constrain dungeon size to 256x256
	 *
	 * The "hold_o_idx" field points to the first object of a stack
	 * of objects (if any) being carried by the monster (see above).
	 */
	partial class Monster {
		//TODO make this a property that looks at/changes r_idx
		public Monster_Race Race;
		public Int16 r_idx;			/* Monster race index */
		public int midx;

		public byte fy;			/* Y location on map */
		public byte fx;			/* X location on map */

		public Int16 hp;			/* Current Hit points */
		public Int16 maxhp;			/* Max Hit points */

		public Int16[] m_timed = new Int16[(int)Misc.MON_TMD.MAX]; /* Timed monster status effects */

		public byte mspeed;		/* Monster "speed" */
		public byte energy;		/* Monster "energy" */

		public byte cdis;			/* Current dis from player */

		public short mflag;			/* Extra monster flags */

		public bool ml;			/* Monster is "visible" */
		public bool unaware;		/* Player doesn't know this is a monster */
	
		public Int16 mimicked_o_idx; /* Object this monster is mimicking */

		public Int16 hold_o_idx;	/* Object being held (if any) */

		public ConsoleColor attr;  		/* attr last used for drawing monster */

		public UInt32 smart;			/* Field for "adult_ai_learn" */

		public Bitflag known_pflags = new Bitflag(Object.Object_Flag.SIZE); /* Known player flags */


		//Was in Monster.utilities
		/**
		 * Return the monster base matching the given name.
		 */
		public static Monster_Base lookup_monster_base(string name)
		{
			/* Look for it */
			for (Monster_Base Base = Misc.rb_info; Base != null; Base = Base.Next) {
				if(name == Base.Name) {
					return Base;
				}
			}

			return null;
		}

		internal void WIPE() {
			r_idx = 0;
		}

		internal void COPY(Monster n_ptr) {
			throw new NotImplementedException();
		}

		/*
		 * Process all the "live" monsters, once per game turn.
		 *
		 * During each game turn, we scan through the list of all the "live" monsters,
		 * (backwards, so we can excise any "freshly dead" monsters), energizing each
		 * monster, and allowing fully energized monsters to move, attack, pass, etc.
		 *
		 * Note that monsters can never move in the monster array (except when the
		 * "compact_monsters()" function is called by "dungeon()" or "save_player()").
		 *
		 * This function is responsible for at least half of the processor time
		 * on a normal system with a "normal" amount of monsters and a player doing
		 * normal things.
		 *
		 * When the player is resting, virtually 90% of the processor time is spent
		 * in this function, and its children, "process_monster()" and "make_move()".
		 *
		 * Most of the rest of the time is spent in "update_view()" and "light_spot()",
		 * especially when the player is running.
		 *
		 * Note the special "MFLAG_NICE" flag, which prevents "nasty" monsters from
		 * using any of their spell attacks until the player gets a turn.
		 */
		public static void process_monsters(Cave c, byte minimum_energy)
		{
			int i;

			Monster m_ptr;
			Monster_Race r_ptr;

			/* Process the monsters (backwards) */
			for (i = Cave.cave_monster_max(c) - 1; i >= 1; i--)
			{
				/* Handle "leaving" */
				if (Misc.p_ptr.leaving) break;


				/* Get the monster */
				m_ptr = Cave.cave_monster(Cave.cave, i);


				/* Ignore "dead" monsters */
				if (m_ptr.r_idx == 0) continue;


				/* Not enough energy to move */
				if (m_ptr.energy < minimum_energy) continue;

				/* Use up "some" energy */
				m_ptr.energy -= 100;


				/* Heal monster? XXX XXX XXX */


				/* Get the race */
				r_ptr = Misc.r_info[m_ptr.r_idx];

				/*
				 * Process the monster if the monster either:
				 * - can "sense" the player
				 * - is hurt
				 * - can "see" the player (checked backwards)
				 * - can "smell" the player from far away (flow)
				 */
				if ((m_ptr.cdis <= r_ptr.aaf) || (m_ptr.hp < m_ptr.maxhp) || 
					Cave.player_has_los_bold(m_ptr.fy, m_ptr.fx) || monster_can_flow(c, i))
				{
					/* Process the monster */
					process_monster(c, i);
				}
			}
		}

		static bool monster_can_flow(Cave c, int m_idx)
		{
			Monster m_ptr;
			Monster_Race r_ptr;
			int fy, fx;

			Misc.assert(c != null);

			m_ptr = Cave.cave_monster(Cave.cave, m_idx);
			r_ptr = Misc.r_info[m_ptr.r_idx];
			fy = m_ptr.fy;
			fx = m_ptr.fx;

			/* Check the flow (normal aaf is about 20) */
			if ((c.when[fy][fx] == c.when[Misc.p_ptr.py][Misc.p_ptr.px]) &&
				(c.cost[fy][fx] < Misc.MONSTER_FLOW_DEPTH) &&
				(c.cost[fy][fx] < r_ptr.aaf))
				return true;
			return false;
		}

		/*
		 * Process a monster
		 *
		 * In several cases, we directly update the monster lore
		 *
		 * Note that a monster is only allowed to "reproduce" if there
		 * are a limited number of "reproducing" monsters on the current
		 * level.  This should prevent the level from being "swamped" by
		 * reproducing monsters.  It also allows a large mass of mice to
		 * prevent a louse from multiplying, but this is a small price to
		 * pay for a simple multiplication method.
		 *
		 * XXX Monster fear is slightly odd, in particular, monsters will
		 * fixate on opening a door even if they cannot open it.  Actually,
		 * the same thing happens to normal monsters when they hit a door
		 *
		 * In addition, monsters which *cannot* open or bash down a door
		 * will still stand there trying to open it...  XXX XXX XXX
		 *
		 * Technically, need to check for monster in the way combined
		 * with that monster being in a wall (or door?) XXX
		 */
		public static void process_monster(Cave c, int m_idx)
		{
			Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);
			Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];
			Monster_Lore l_ptr = Misc.l_list[m_ptr.r_idx];

			int i, oy, ox, ny, nx;

			int[] mm = new int[5];

			bool woke_up = false;
			bool stagger;

			bool do_turn;
			bool do_move;
			bool do_view;

			bool did_open_door;
			bool did_bash_door;


			/* Handle "sleep" */
			if (m_ptr.m_timed[(int)Misc.MON_TMD.SLEEP] != 0)
			{
				uint notice;

				/* Aggravation */
				if (Misc.p_ptr.check_state(Object_Flag.AGGRAVATE, Misc.p_ptr.state.flags))
				{
				    /* Wake the monster */
				    mon_clear_timed(m_idx, (int)Misc.MON_TMD.SLEEP, Misc.MON_TMD_FLG_NOTIFY, false);

				    /* Notice the "waking up" */
				    if (m_ptr.ml && !m_ptr.unaware)
				    {
				        //char m_name[80];
						string m_name;

				        /* Get the monster name */
						m_name = m_ptr.monster_desc(0);
				        //monster_desc(m_name, sizeof(m_name), m_ptr, 0);

				        /* Dump a message */
				        Utilities.msg("%^s wakes up.", m_name);

				        /* Hack -- Update the health bar */
				        if (Misc.p_ptr.health_who == m_idx) Misc.p_ptr.redraw |= (Misc.PR_HEALTH);
				    }

				    /* Efficiency XXX XXX */
				    return;
				}

				/* Anti-stealth */
				notice = (uint)Random.randint0(1024);

				/* Hack -- See if monster "notices" player */
				if ((notice * notice * notice) <= Misc.p_ptr.state.noise)
				{
				    int d = 1;

				    /* Wake up faster near the player */
				    if (m_ptr.cdis < 50) d = (100 / m_ptr.cdis);

				    /* Still asleep */
				    if (m_ptr.m_timed[(int)Misc.MON_TMD.SLEEP] > d)
				    {
				        /* Monster wakes up "a little bit" */
				        mon_dec_timed(m_idx, (int)Misc.MON_TMD.SLEEP, d , (ushort)Misc.MON_TMD_FLG_NOMESSAGE, false);

				        /* Notice the "not waking up" */
				        if (m_ptr.ml && !m_ptr.unaware)
				        {
				            /* Hack -- Count the ignores */
				            if (l_ptr.ignore < Byte.MaxValue)
				            {
				                l_ptr.ignore++;
				            }
				        }
				    }

				    /* Just woke up */
				    else
				    {
				        /* Reset sleep counter */
				        woke_up = mon_clear_timed(m_idx, (int)Misc.MON_TMD.SLEEP, Misc.MON_TMD_FLG_NOMESSAGE, false);

				        /* Notice the "waking up" */
				        if (m_ptr.ml && !m_ptr.unaware)
				        {
				            //char m_name[80];
							string m_name;

				            /* Get the monster name */
				            m_name = m_ptr.monster_desc(0);

				            /* Dump a message */
				            //Utilities.msg("%^s wakes up.", m_name);
							Utilities.msg(m_name + " wakes up.");

				            /* Hack -- Update the health bar */
				            if (Misc.p_ptr.health_who == m_idx) Misc.p_ptr.redraw |= (Misc.PR_HEALTH);

				            /* Hack -- Count the wakings */
				            if (l_ptr.wake < byte.MaxValue)
				            {
				                l_ptr.wake++;
				            }
				        }
				    }
				}

				/* Still sleeping */
				if (m_ptr.m_timed[(int)Misc.MON_TMD.SLEEP] != 0) return;
			}

			/* If the monster just woke up, then it doesn't act */
			if (woke_up) return;

			if (m_ptr.m_timed[(int)Misc.MON_TMD.FAST] != 0)
			    mon_dec_timed(m_idx, Misc.MON_TMD.FAST, 1, 0, false);

			if (m_ptr.m_timed[(int)Misc.MON_TMD.SLOW] != 0)
			    mon_dec_timed(m_idx, Misc.MON_TMD.SLOW, 1, 0, false);

			if (m_ptr.m_timed[(int)Misc.MON_TMD.STUN] != 0)
			{
				throw new NotImplementedException();
				//int d = 1;

				///* Make a "saving throw" against stun */
				//if (randint0(5000) <= r_ptr.level * r_ptr.level)
				//{
				//    /* Recover fully */
				//    d = m_ptr.m_timed[MON_TMD_STUN];
				//}

				///* Hack -- Recover from stun */
				//if (m_ptr.m_timed[MON_TMD_STUN] > d)
				//    mon_dec_timed(m_idx, MON_TMD_STUN, 1, MON_TMD_FLG_NOMESSAGE, false);
				//else
				//    mon_clear_timed(m_idx, MON_TMD_STUN, MON_TMD_FLG_NOTIFY, false);

				///* Still stunned */
				//if (m_ptr.m_timed[MON_TMD_STUN]) return;
			}

			if (m_ptr.m_timed[(int)Misc.MON_TMD.CONF] != 0)
			{
				throw new NotImplementedException();
				//int d = randint1(r_ptr.level / 10 + 1);

				///* Still confused */
				//if (m_ptr.m_timed[MON_TMD_CONF] > d)
				//    mon_dec_timed(m_idx, MON_TMD_CONF, d , MON_TMD_FLG_NOMESSAGE,
				//        false);
				//else
				//    mon_clear_timed(m_idx, MON_TMD_CONF, MON_TMD_FLG_NOTIFY, false);
			}

			if (m_ptr.m_timed[(int)Misc.MON_TMD.FEAR] != 0)
			{
				/* Amount of "boldness" */
				int d = Random.randint1(r_ptr.level / 10 + 1);

				if (m_ptr.m_timed[(int)Misc.MON_TMD.FEAR] > d)
				    mon_dec_timed(m_idx, Misc.MON_TMD.FEAR, d, Misc.MON_TMD_FLG_NOMESSAGE, false);
				else
				    mon_clear_timed(m_idx, Misc.MON_TMD.FEAR, Misc.MON_TMD_FLG_NOTIFY, false);
			}


			/* Get the origin */
			oy = m_ptr.fy;
			ox = m_ptr.fx;


			/* Attempt to "mutiply" (all monsters are allowed an attempt for lore
			 * purposes, even non-breeders)
			 */
			if (Misc.num_repro < Misc.MAX_REPRO)
			{
				int k, y, x;

				/* Count the adjacent monsters */
				for (k = 0, y = oy - 1; y <= oy + 1; y++)
				{
				    for (x = ox - 1; x <= ox + 1; x++)
				    {
				        /* Count monsters */
				        if (Cave.cave.m_idx[y][x] > 0) k++;
				    }
				}

				/* Multiply slower in crowded areas */
				if ((k < 4) && (k == 0 || Random.one_in_(k * Misc.MON_MULT_ADJ)))
				{
				    /* Successful breeding attempt, learn about that now */
				    if (m_ptr.ml) l_ptr.flags.on(Monster_Flag.MULTIPLY.value);
			
				    /* Try to multiply (only breeders allowed) */
				    if (r_ptr.flags.has(Monster_Flag.MULTIPLY.value) && multiply_monster(m_idx))
				    {
				        /* Make a sound */
				        if (m_ptr.ml)
				        {
				            //sound(MSG_MULTIPLY); //TODO: enable sound
				        }

				        /* Multiplying takes energy */
				        return;
				    }
				}
			}

			/* Mimics lie in wait */
			if (is_mimicking(m_idx)) return;
	
			/* Attempt to cast a spell */
			if (make_attack_spell(m_idx)) return;


			/* Reset */
			stagger = false;

			/* Confused */
			if (m_ptr.m_timed[(int)Misc.MON_TMD.CONF] != 0)
			{
			    /* Stagger */
			    stagger = true;
			}

			/* Random movement - always attempt for lore purposes */
			else
			{
			    int roll = Random.randint0(100);
		
			    /* Random movement (25%) */
			    if (roll < 25)
			    {
			        /* Learn about small random movement */
			        if (m_ptr.ml) l_ptr.flags.on(Monster_Flag.RAND_25.value);

			        /* Stagger */			
			        if (r_ptr.flags.test(Monster_Flag.RAND_25.value, Monster_Flag.RAND_50.value)) stagger = true;
			    }
		
			    /* Random movement (50%) */
			    else if (roll < 50)
			    {
			        /* Learn about medium random movement */
			        if (m_ptr.ml) l_ptr.flags.on(Monster_Flag.RAND_50.value);

			        /* Stagger */			
			        if (r_ptr.flags.has(Monster_Flag.RAND_50.value)) stagger = true;
			    }
		
			    /* Random movement (75%) */
			    else if (roll < 75)
			    {
			        /* Stagger */			
			        if (r_ptr.flags.test_all(Monster_Flag.RAND_25.value, Monster_Flag.RAND_50.value))
			        {
			            stagger = true;
			        }
			    }
			}

			/* Normal movement */
			if (!stagger)
			{
			    /* Logical moves, may do nothing */
			    if (!get_moves(Cave.cave, m_idx, mm)) return;
			}


			/* Assume nothing */
			do_turn = false;
			do_move = false;
			do_view = false;

			/* Assume nothing */
			did_open_door = false;
			did_bash_door = false;


			/* Process moves */
			for (i = 0; i < 5; i++)
			{
			    /* Get the direction (or stagger) */
			    int d = (stagger ? Misc.ddd[Random.randint0(8)] : mm[i]);

			    /* Get the destination */
			    ny = oy + Misc.ddy[d];
			    nx = ox + Misc.ddx[d];


			    /* Floor is open? */
			    if (Cave.cave_floor_bold(ny, nx))
			    {
			        /* Go ahead and move */
			        do_move = true;
			    }

			    /* Permanent wall in the way */
			    else if (Cave.cave.feat[ny][nx] >= Cave.FEAT_PERM_EXTRA)
			    {
			        /* Nothing */
			    }

			    /* Normal wall, door, or secret door in the way */
			    else
			    {
			        /* There's some kind of feature in the way, so learn about
			         * kill-wall and pass-wall now
			         */
			        if (m_ptr.ml)
			        {
			            l_ptr.flags.on(Monster_Flag.PASS_WALL.value);
			            l_ptr.flags.on(Monster_Flag.KILL_WALL.value);
			        }

			        /* Monster moves through walls (and doors) */
			        if (r_ptr.flags.has(Monster_Flag.PASS_WALL.value))
			        {
			            /* Pass through walls/doors/rubble */
			            do_move = true;
			        }

			        /* Monster destroys walls (and doors) */
			        else if (r_ptr.flags.has(Monster_Flag.KILL_WALL.value))
			        {
			            /* Eat through walls/doors/rubble */
			            do_move = true;

			            /* Forget the wall */
			            Cave.cave.info[ny][nx] &= ~(Cave.CAVE_MARK);

			            /* Notice */
			            Cave.cave_set_feat(c, ny, nx, Cave.FEAT_FLOOR);

			            /* Note changes to viewable region */
			            if (Cave.player_has_los_bold(ny, nx)) do_view = true;
			        }

			        /* Handle doors and secret doors */
			        else if (((Cave.cave.feat[ny][nx] >= Cave.FEAT_DOOR_HEAD) &&
			                     (Cave.cave.feat[ny][nx] <= Cave.FEAT_DOOR_TAIL)) ||
			                    (Cave.cave.feat[ny][nx] == Cave.FEAT_SECRET))
			        {
			            bool may_bash = true;

			            /* Take a turn */
			            do_turn = true;
				
			            /* Learn about door abilities */
			            if (m_ptr.ml)
			            {
			                l_ptr.flags.on(Monster_Flag.OPEN_DOOR.value);
			                l_ptr.flags.on(Monster_Flag.BASH_DOOR.value);
			            }

			            /* Creature can open doors. */
			            if (r_ptr.flags.has(Monster_Flag.OPEN_DOOR.value))
			            {
			                /* Closed doors and secret doors */
			                if ((Cave.cave.feat[ny][nx] == Cave.FEAT_DOOR_HEAD) ||
			                         (Cave.cave.feat[ny][nx] == Cave.FEAT_SECRET)) {
			                    /* The door is open */
			                    did_open_door = true;

			                    /* Do not bash the door */
			                    may_bash = false;
			                }

			                /* Locked doors (not jammed) */
			                else if (Cave.cave.feat[ny][nx] < Cave.FEAT_DOOR_HEAD + 0x08) {
			                    int k;

			                    /* Door power */
			                    k = ((Cave.cave.feat[ny][nx] - Cave.FEAT_DOOR_HEAD) & 0x07);

			                    /* Try to unlock it */
			                    if (Random.randint0(m_ptr.hp / 10) > k) {
			                        Utilities.msg("Something fiddles with a lock.");

			                        /* Reduce the power of the door by one */
			                        Cave.cave_set_feat(c, ny, nx, Cave.cave.feat[ny][nx] - 1);

			                        /* Do not bash the door */
			                        may_bash = false;
			                    }
			                }
			            }

			            /* Stuck doors -- attempt to bash them down if allowed */
			            if (may_bash && r_ptr.flags.has(Monster_Flag.BASH_DOOR.value))
			            {
			                int k;

			                /* Door power */
			                k = ((Cave.cave.feat[ny][nx] - Cave.FEAT_DOOR_HEAD) & 0x07);

			                /* Attempt to bash */
			                if (Random.randint0(m_ptr.hp / 10) > k) {
			                    Utilities.msg("Something slams against a door.");

			                    /* Reduce the power of the door by one */
			                    Cave.cave_set_feat(c, ny, nx, Cave.cave.feat[ny][nx] - 1);

			                    /* If the door is no longer jammed */
			                    if (Cave.cave.feat[ny][nx] < Cave.FEAT_DOOR_HEAD + 0x09)	{
			                        Utilities.msg("You hear a door burst open!");

			                        /* Disturb (sometimes) */
			                        Cave.disturb(Misc.p_ptr, 0, 0);

			                        /* The door was bashed open */
			                        did_bash_door = true;

			                        /* Hack -- fall into doorway */
			                        do_move = true;
			                    }
			                }
			            }
			        }

			        /* Deal with doors in the way */
			        if (did_open_door || did_bash_door)
			        {
			            /* Break down the door */
			            if (did_bash_door && (Random.randint0(100) < 50))
			            {
			                Cave.cave_set_feat(c, ny, nx, Cave.FEAT_BROKEN);
			            }

			            /* Open the door */
			            else
			            {
			                Cave.cave_set_feat(c, ny, nx, Cave.FEAT_OPEN);
			            }

			            /* Handle viewable doors */
			            if (Cave.player_has_los_bold(ny, nx)) do_view = true;
			        }
			    }


			    /* Hack -- check for Glyph of Warding */
			    if (do_move && (Cave.cave.feat[ny][nx] == Cave.FEAT_GLYPH))
			    {
			        /* Assume no move allowed */
			        do_move = false;

			        /* Break the ward */
			        if (Random.randint1(Misc.BREAK_GLYPH) < r_ptr.level)
			        {
			            /* Describe observable breakage */
			            if ((Cave.cave.info[ny][nx] & (Cave.CAVE_MARK)) != 0)
			            {
			                Utilities.msg("The rune of protection is broken!");
			            }

			            /* Forget the rune */
			            Cave.cave.info[ny][nx] &= ~Cave.CAVE_MARK;

			            /* Break the rune */
			            Cave.cave_set_feat(c, ny, nx, Cave.FEAT_FLOOR);

			            /* Allow movement */
			            do_move = true;
			        }
			    }


			    /* The player is in the way. */
			    if (do_move && (Cave.cave.m_idx[ny][nx] < 0))
			    {
			        /* Learn about if the monster attacks */
			        if (m_ptr.ml) l_ptr.flags.on(Monster_Flag.NEVER_BLOW.value);

			        /* Some monsters never attack */
			        if (r_ptr.flags.has(Monster_Flag.NEVER_BLOW.value))
			        {
			            /* Do not move */
			            do_move = false;
			        }
			
			        /* Otherwise, attack the player */
			        else
			        {
			            /* Do the attack */
			            m_ptr.make_attack_normal(Misc.p_ptr);

			            /* Do not move */
			            do_move = false;

			            /* Took a turn */
			            do_turn = true;
			        }
			    }


			    /* Some monsters never move */
			    if (do_move && r_ptr.flags.has(Monster_Flag.NEVER_MOVE.value))
			    {
			        /* Learn about lack of movement */
			        if (m_ptr.ml) l_ptr.flags.on(Monster_Flag.NEVER_MOVE.value);

			        /* Do not move */
			        do_move = false;
			    }


			    /* A monster is in the way */
			    if (do_move && (Cave.cave.m_idx[ny][nx] > 0))
			    {
			        Monster n_ptr = Cave.cave_monster(Cave.cave, Cave.cave.m_idx[ny][nx]);

			        /* Kill weaker monsters */
			        bool kill_ok = r_ptr.flags.has(Monster_Flag.KILL_BODY.value);

			        /* Move weaker monsters if they can swap places */
			        /* (not in a wall) */
			        bool move_ok = (r_ptr.flags.has(Monster_Flag.MOVE_BODY.value) &&
			                       Cave.cave_floor_bold(m_ptr.fy, m_ptr.fx));

			        /* Assume no movement */
			        do_move = false;

					if (compare_monsters(m_ptr, n_ptr) > 0)
					{
					    /* Learn about pushing and shoving */
					    if (m_ptr.ml)
					    {
					        l_ptr.flags.on(Monster_Flag.KILL_BODY.value);
					        l_ptr.flags.on(Monster_Flag.MOVE_BODY.value);
					    }

					    if (kill_ok || move_ok)
					    {
					        /* Get the names of the monsters involved */
					        //char m_name[80];
					        //char n_name[80];
					        string m_name;
					        string n_name;
					        m_name = m_ptr.monster_desc(Desc.IND1);
					        n_name = n_ptr.monster_desc(Desc.IND1);

					        /* Allow movement */
					        do_move = true;

					        /* Monster ate another monster */
					        if (kill_ok)
					        {
					            /* Note if visible */
					            if (m_ptr.ml && (m_ptr.mflag & (Monster_Flag.MFLAG_VIEW)) != 0)
					            {
									string name = Char.ToUpper(m_name[0]) + m_name.Substring(1);
					                Utilities.msg("{0} tramples over {1}.", name, n_name);
					            }

								throw new NotImplementedException();
					            //delete_monster(ny, nx);
					        }
					        else
					        {
					            /* Note if visible */
					            if (m_ptr.ml && (m_ptr.mflag & (Monster_Flag.MFLAG_VIEW)) != 0)
					            {
									string name = Char.ToUpper(m_name[0]) + m_name.Substring(1);
					                Utilities.msg("{0} pushes past {1}.", name, n_name);
					            }
					        }
					    }
					}
			    }

			    /* Creature has been allowed move */
			    if (do_move)
			    {
			        short this_o_idx, next_o_idx = 0;
			
			        /* Learn about no lack of movement */
			        if (m_ptr.ml) l_ptr.flags.on(Monster_Flag.NEVER_MOVE.value);

			        /* Take a turn */
			        do_turn = true;

			        /* Move the monster */
			        monster_swap(oy, ox, ny, nx);

			        /* Possible disturb */
			        if (m_ptr.ml && (Option.disturb_move.value || 
								(((m_ptr.mflag & (Monster_Flag.MFLAG_VIEW)) != 0) && Option.disturb_near.value)))
			        {
			            /* Disturb */
			            Cave.disturb(Misc.p_ptr, 0, 0);
			        }


			        /* Scan all objects in the grid */
			        for (this_o_idx = Cave.cave.o_idx[ny][nx]; this_o_idx != 0; this_o_idx = next_o_idx)
			        {
			            Object.Object o_ptr;

			            /* Get the object */
			            o_ptr = Object.Object.byid(this_o_idx);
						if(o_ptr == null)
							continue;

			            /* Get the next object */
			            next_o_idx = o_ptr.next_o_idx;

			            /* Skip gold */
			            if (o_ptr.tval == TVal.TV_GOLD) continue;
				
			            /* Learn about item pickup behavior */
			            if (m_ptr.ml)
			            {
			                l_ptr.flags.on(Monster_Flag.TAKE_ITEM.value);
			                l_ptr.flags.on(Monster_Flag.KILL_ITEM.value);
			            }

			            /* Take or Kill objects on the floor */
			            if (r_ptr.flags.has(Monster_Flag.TAKE_ITEM.value) || r_ptr.flags.has(Monster_Flag.KILL_ITEM.value))
			            {
			                Bitflag obj_flags = new Bitflag(Object_Flag.SIZE);
			                Bitflag mon_flags = new Bitflag(Monster_Flag.SIZE);

			                //char m_name[80];
			                //char o_name[80];
							string m_name;
							string o_name = "";

			                mon_flags.wipe();

			                /* Extract some flags */
			                o_ptr.object_flags(ref obj_flags);

			                /* Get the object name */
			                o_name = o_ptr.object_desc(Object.Object.Detail.PREFIX | Object.Object.Detail.FULL);

			                /* Get the monster name */
			                m_name = m_ptr.monster_desc(Desc.IND1);

			                /* React to objects that hurt the monster */
							throw new NotImplementedException();
							//react_to_slay(obj_flags, mon_flags);

							///* The object cannot be picked up by the monster */
							//if (o_ptr.artifact || rf_is_inter(r_ptr.flags, mon_flags))
							//{
							//    /* Only give a message for "take_item" */
							//    if (rf_has(r_ptr.flags, RF_TAKE_ITEM))
							//    {
							//        /* Describe observable situations */
							//        if (m_ptr.ml && Cave.player_has_los_bold(ny, nx) && !squelch_item_ok(o_ptr))
							//        {
							//            /* Dump a message */
							//            msg("%^s tries to pick up %s, but fails.",
							//                       m_name, o_name);
							//        }
							//    }
							//}

							///* Pick up the item */
							//else if (rf_has(r_ptr.flags, RF_TAKE_ITEM))
							//{
							//    Object.Object i_ptr;
							//    //object_type object_type_body;

							//    /* Describe observable situations */
							//    if (player_has_los_bold(ny, nx) && !squelch_item_ok(o_ptr))
							//    {
							//        /* Dump a message */
							//        msg("%^s picks up %s.", m_name, o_name);
							//    }

							//    /* Get local object */
							//    i_ptr = &object_type_body;

							//    /* Obtain local object */
							//    object_copy(i_ptr, o_ptr);

							//    /* Delete the object */
							//    delete_object_idx(this_o_idx);

							//    /* Carry the object */
							//    monster_carry(m_ptr, i_ptr);
							//}

							///* Destroy the item */
							//else
							//{
							//    /* Describe observable situations */
							//    if (player_has_los_bold(ny, nx) && !squelch_item_ok(o_ptr))
							//    {
							//        /* Dump a message */
							//        msgt(MSG_DESTROY, "%^s crushes %s.", m_name, o_name);
							//    }

							//    /* Delete the object */
							//    delete_object_idx(this_o_idx);
							//}
			            }
			        }
			    }

			    /* Stop when done */
			    if (do_turn) break;
			}


			/* If we haven't done anything, try casting a spell again */
			if (Option.birth_ai_smart.value && !do_turn && !do_move)
			{
			    /* Cast spell */
			    if (make_attack_spell(m_idx)) return;
			}

			if (r_ptr.flags.has(Monster_Flag.HAS_LIGHT.value)) do_view = true;

			/* Notice changes in view */
			if (do_view)
			{
			    /* Update the visuals */
			    Misc.p_ptr.update |= (Misc.PU_UPDATE_VIEW | Misc.PU_MONSTERS);

			    /* Fully update the flow XXX XXX XXX */
			    Misc.p_ptr.update |= (Misc.PU_FORGET_FLOW | Misc.PU_UPDATE_FLOW);
			}


			/* Hack -- get "bold" if out of options */
			if (!do_turn && !do_move && m_ptr.m_timed[(int)Misc.MON_TMD.FEAR] != 0)
			{
			    mon_clear_timed(m_idx, Misc.MON_TMD.FEAR, Misc.MON_TMD_FLG_NOTIFY, false);
			}

			/* If we see an unaware monster do something, become aware of it */
			if(do_turn && m_ptr.unaware) {
				throw new NotImplementedException();
				//become_aware(m_idx);
			}

		}

		/*
		 * Hack -- compare the "strength" of two monsters XXX XXX XXX
		 */
		static int compare_monsters(Monster m_ptr, Monster n_ptr)
		{
			Monster_Race r_ptr;

			int mexp1, mexp2;

			/* Race 1 */
			r_ptr = Misc.r_info[m_ptr.r_idx];

			/* Extract mexp */
			mexp1 = r_ptr.mexp;

			/* Race 2 */
			r_ptr = Misc.r_info[n_ptr.r_idx];

			/* Extract mexp */
			mexp2 = r_ptr.mexp;

			/* Compare */
			if (mexp1 < mexp2) return (-1);
			if (mexp1 > mexp2) return (1);

			/* Assume equal */
			return (0);
		}

		/*
		 * Choose "logical" directions for monster movement
		 *
		 * We store the directions in a special "mm" array
		 */
		static bool get_moves(Cave c, int m_idx, int[] mm)
		{
			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);
			Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			int y, ay, x, ax;

			int move_val = 0;

			int y2 = py;
			int x2 = px;

			bool done = false;

			/* Flow towards the player */
			get_moves_aux(c, m_idx, ref y2, ref x2);

			/* Extract the "pseudo-direction" */
			y = m_ptr.fy - y2;
			x = m_ptr.fx - x2;



			/* Normal animal packs try to get the player out of corridors. */
			if (Option.birth_ai_packs.value && r_ptr.flags.has(Monster_Flag.FRIENDS.value) && 
				r_ptr.flags.has(Monster_Flag.ANIMAL.value) &&
				!r_ptr.flags.test(Monster_Flag.PASS_WALL.value, Monster_Flag.KILL_WALL.value))
			{
				throw new NotImplementedException();
				//int i, open = 0;

				///* Count empty grids next to player */
				//for (i = 0; i < 8; i++)
				//{
				//    /* Check grid around the player for room interior (room walls count)
				//       or other empty space */
				//    if ((cave.feat[py + ddy_ddd[i]][px + ddx_ddd[i]] <= FEAT_MORE) ||
				//        (cave.info[py + ddy_ddd[i]][px + ddx_ddd[i]] & (CAVE_ROOM)))
				//    {
				//        /* One more open grid */
				//        open++;
				//    }
				//}

				///* Not in an empty space and strong player */
				//if ((open < 7) && (p_ptr.chp > p_ptr.mhp / 2))
				//{
				//    /* Find hiding place */
				//    if (find_hiding(m_idx, &y, &x)) done = true;
				//}
			}


			/* Apply fear */
			if (!done && mon_will_run(m_idx))
			{
				/* Try to find safe place */
				if (!(Option.birth_ai_smart.value && find_safety(c, m_idx, out y, out x)))
				{
				    /* This is not a very "smart" method XXX XXX */
				    y = (-y);
				    x = (-x);
				}

				else
				{
				    /* Adjust movement */
				    get_fear_moves_aux(c, m_idx, ref y, ref x);
				}

				done = true;
			}


			/* Monster groups try to surround the player */
			if (!done && Option.birth_ai_packs.value && r_ptr.flags.has(Monster_Flag.FRIENDS.value))
			{
				throw new NotImplementedException();
				//int i;

				///* If we are not already adjacent */
				//if (m_ptr.cdis > 1)
				//{
				//    /* Find an empty square near the player to fill */
				//    int tmp = randint0(8);
				//    for (i = 0; i < 8; i++)
				//    {
				//        /* Pick squares near player (pseudo-randomly) */
				//        y2 = py + ddy_ddd[(tmp + i) & 7];
				//        x2 = px + ddx_ddd[(tmp + i) & 7];
				
				//        /* Ignore filled grids */
				//        if (!cave_empty_bold(y2, x2)) continue;
				
				//        /* Try to fill this hole */
				//        break;
				//    }
				//}
				///* Extract the new "pseudo-direction" */
				//y = m_ptr.fy - y2;
				//x = m_ptr.fx - x2;
			}


			/* Check for no move */
			if (x == 0 && y == 0) return (false);

			/* Extract the "absolute distances" */
			ax = Math.Abs(x);
			ay = Math.Abs(y);

			/* Do something weird */
			if (y < 0) move_val += 8;
			if (x > 0) move_val += 4;

			/* Prevent the diamond maneuvre */
			if (ay > (ax << 1))
			{
			    move_val++;
			    move_val++;
			}
			else if (ax > (ay << 1))
			{
			    move_val++;
			}

			/* Analyze */
			switch (move_val)
			{
			    case 0:
			    {
			        mm[0] = 9;
			        if (ay > ax)
			        {
			            mm[1] = 8;
			            mm[2] = 6;
			            mm[3] = 7;
			            mm[4] = 3;
			        }
			        else
			        {
			            mm[1] = 6;
			            mm[2] = 8;
			            mm[3] = 3;
			            mm[4] = 7;
			        }
			        break;
			    }

			    case 1:
			    case 9:
			    {
			        mm[0] = 6;
			        if (y < 0)
			        {
			            mm[1] = 3;
			            mm[2] = 9;
			            mm[3] = 2;
			            mm[4] = 8;
			        }
			        else
			        {
			            mm[1] = 9;
			            mm[2] = 3;
			            mm[3] = 8;
			            mm[4] = 2;
			        }
			        break;
			    }

			    case 2:
			    case 6:
			    {
			        mm[0] = 8;
			        if (x < 0)
			        {
			            mm[1] = 9;
			            mm[2] = 7;
			            mm[3] = 6;
			            mm[4] = 4;
			        }
			        else
			        {
			            mm[1] = 7;
			            mm[2] = 9;
			            mm[3] = 4;
			            mm[4] = 6;
			        }
			        break;
			    }

			    case 4:
			    {
			        mm[0] = 7;
			        if (ay > ax)
			        {
			            mm[1] = 8;
			            mm[2] = 4;
			            mm[3] = 9;
			            mm[4] = 1;
			        }
			        else
			        {
			            mm[1] = 4;
			            mm[2] = 8;
			            mm[3] = 1;
			            mm[4] = 9;
			        }
			        break;
			    }

			    case 5:
			    case 13:
			    {
			        mm[0] = 4;
			        if (y < 0)
			        {
			            mm[1] = 1;
			            mm[2] = 7;
			            mm[3] = 2;
			            mm[4] = 8;
			        }
			        else
			        {
			            mm[1] = 7;
			            mm[2] = 1;
			            mm[3] = 8;
			            mm[4] = 2;
			        }
			        break;
			    }

			    case 8:
			    {
			        mm[0] = 3;
			        if (ay > ax)
			        {
			            mm[1] = 2;
			            mm[2] = 6;
			            mm[3] = 1;
			            mm[4] = 9;
			        }
			        else
			        {
			            mm[1] = 6;
			            mm[2] = 2;
			            mm[3] = 9;
			            mm[4] = 1;
			        }
			        break;
			    }

			    case 10:
			    case 14:
			    {
			        mm[0] = 2;
			        if (x < 0)
			        {
			            mm[1] = 3;
			            mm[2] = 1;
			            mm[3] = 6;
			            mm[4] = 4;
			        }
			        else
			        {
			            mm[1] = 1;
			            mm[2] = 3;
			            mm[3] = 4;
			            mm[4] = 6;
			        }
			        break;
			    }

			    default: /* case 12: */
			    {
			        mm[0] = 1;
			        if (ay > ax)
			        {
			            mm[1] = 2;
			            mm[2] = 4;
			            mm[3] = 3;
			            mm[4] = 7;
			        }
			        else
			        {
			            mm[1] = 4;
			            mm[2] = 2;
			            mm[3] = 7;
			            mm[4] = 3;
			        }
			        break;
			    }
			}

			/* Want to move */
			return (true);
		}

		/*
		 * Returns whether a given monster will try to run from the player.
		 *
		 * Monsters will attempt to avoid very powerful players.  See below.
		 *
		 * Because this function is called so often, little details are important
		 * for efficiency.  Like not using "mod" or "div" when possible.  And
		 * attempting to check the conditions in an optimal order.  Note that
		 * "(x << 2) == (x * 4)" if "x" has enough bits to hold the result.
		 *
		 * Note that this function is responsible for about one to five percent
		 * of the processor use in normal conditions...
		 */
		static bool mon_will_run(int m_idx)
		{
			Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);

			Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			ushort p_lev, m_lev;
			ushort p_chp, p_mhp;
			ushort m_chp, m_mhp;
			uint p_val, m_val;

			/* Keep monsters from running too far away */
			if (m_ptr.cdis > Misc.MAX_SIGHT + 5) return (false);

			/* All "afraid" monsters will run away */
			if (m_ptr.m_timed[(int)Misc.MON_TMD.FEAR] != 0) return (true);

			/* Nearby monsters will not become terrified */
			if (m_ptr.cdis <= 5) return (false);

			/* Examine player power (level) */
			p_lev = (ushort)Misc.p_ptr.lev;

			/* Examine monster power (level plus morale) */
			m_lev = (ushort)(r_ptr.level + (m_idx & 0x08) + 25);

			/* Optimize extreme cases below */
			if (m_lev > p_lev + 4) return (false);
			if (m_lev + 4 <= p_lev) return (true);

			/* Examine player health */
			p_chp = (ushort)Misc.p_ptr.chp;
			p_mhp = (ushort)Misc.p_ptr.mhp;

			/* Examine monster health */
			m_chp = (ushort)m_ptr.hp;
			m_mhp = (ushort)m_ptr.maxhp;

			/* Prepare to optimize the calculation */
			p_val = (ushort)((p_lev * p_mhp) + (p_chp << 2));	/* div p_mhp */
			m_val = (ushort)((m_lev * m_mhp) + (m_chp << 2));	/* div m_mhp */

			/* Strong players scare strong monsters */
			if (p_val * m_mhp > m_val * p_mhp) return (true);

			/* Assume no terror */
			return (false);
		}

		/*
		 * Choose the "best" direction for "flowing"
		 *
		 * Note that ghosts and rock-eaters are never allowed to "flow",
		 * since they should move directly towards the player.
		 *
		 * Prefer "non-diagonal" directions, but twiddle them a little
		 * to angle slightly towards the player's actual location.
		 *
		 * Allow very perceptive monsters to track old "spoor" left by
		 * previous locations occupied by the player.  This will tend
		 * to have monsters end up either near the player or on a grid
		 * recently occupied by the player (and left via "teleport").
		 *
		 * Note that if "smell" is turned on, all monsters get vicious.
		 *
		 * Also note that teleporting away from a location will cause
		 * the monsters who were chasing you to converge on that location
		 * as long as you are still near enough to "annoy" them without
		 * being close enough to chase directly.  I have no idea what will
		 * happen if you combine "smell" with low "aaf" values.
		 */
		static bool get_moves_aux(Cave c, int m_idx, ref int yp, ref int xp)
		{
			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			int i, y, x, y1, x1;

			int when = 0;
			int cost = 999;

			Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);
			Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			/* Monster can go through rocks */
			if (r_ptr.flags.test(Monster_Flag.PASS_WALL.value, Monster_Flag.KILL_WALL.value)) return (false);

			/* Monster location */
			y1 = m_ptr.fy;
			x1 = m_ptr.fx;

			/* The player is not currently near the monster grid */
			if (c.when[y1][x1] < c.when[py][px])
			{
				/* The player has never been near the monster grid */
				if (c.when[y1][x1] == 0) return (false);

				/* The monster is not allowed to track the player */
				if (!Option.birth_ai_smell.value) return (false);
			}

			/* Monster is too far away to notice the player */
			if (c.cost[y1][x1] > Misc.MONSTER_FLOW_DEPTH) return (false);
			if (c.cost[y1][x1] > r_ptr.aaf) return (false);

			/* Hack -- Player can see us, run towards him */
			if (Cave.player_has_los_bold(y1, x1)) return (false);

			/* Check nearby grids, diagonals first */
			for (i = 7; i >= 0; i--)
			{
				/* Get the location */
				y = y1 + Misc.ddy_ddd[i];
				x = x1 + Misc.ddx_ddd[i];

				/* Ignore illegal locations */
				if (c.when[y][x] == 0) continue;

				/* Ignore ancient locations */
				if (c.when[y][x] < when) continue;

				/* Ignore distant locations */
				if (c.cost[y][x] > cost) continue;

				/* Save the cost and time */
				when = c.when[y][x];
				cost = c.cost[y][x];

				/* Hack -- Save the "twiddled" location */
				yp = py + 16 * Misc.ddy_ddd[i];
				xp = px + 16 * Misc.ddx_ddd[i];
			}

			/* No legal move (?) */
			if (when == 0) return (false);

			/* Success */
			return (true);
		}

		/*
		 * Creatures can cast spells, shoot missiles, and breathe.
		 *
		 * Returns "true" if a spell (or whatever) was (successfully) cast.
		 *
		 * XXX XXX XXX This function could use some work, but remember to
		 * keep it as optimized as possible, while retaining generic code.
		 *
		 * Verify the various "blind-ness" checks in the code.
		 *
		 * XXX XXX XXX Note that several effects should really not be "seen"
		 * if the player is blind.
		 *
		 * Perhaps monsters should breathe at locations *near* the player,
		 * since this would allow them to inflict "partial" damage.
		 *
		 * Perhaps smart monsters should decline to use "bolt" spells if
		 * there is a monster in the way, unless they wish to kill it.
		 *
		 * It will not be possible to "correctly" handle the case in which a
		 * monster attempts to attack a location which is thought to contain
		 * the player, but which in fact is nowhere near the player, since this
		 * might induce all sorts of messages about the attack itself, and about
		 * the effects of the attack, which the player might or might not be in
		 * a position to observe.  Thus, for simplicity, it is probably best to
		 * only allow "faulty" attacks by a monster if one of the important grids
		 * (probably the initial or final grid) is in fact in view of the player.
		 * It may be necessary to actually prevent spell attacks except when the
		 * monster actually has line of sight to the player.  Note that a monster
		 * could be left in a bizarre situation after the player ducked behind a
		 * pillar and then teleported away, for example.
		 *
		 * Note that this function attempts to optimize the use of spells for the
		 * cases in which the monster has no spells, or has spells but cannot use
		 * them, or has spells but they will have no "useful" effect.  Note that
		 * this function has been an efficiency bottleneck in the past.
		 *
		 * Note the special "MFLAG_NICE" flag, which prevents a monster from using
		 * any spell attacks until the player has had a single chance to move.
		 */
		static bool make_attack_spell(int m_idx)
		{
			int chance, thrown_spell, rlev, failrate;

			Bitflag f = new Bitflag(Monster_Spell_Flag.SIZE);

			Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);
			Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];
			Monster_Lore l_ptr = Misc.l_list[m_ptr.r_idx];

			//char m_name[80], m_poss[80], ddesc[80];
			string m_name, m_poss, ddesc;

			/* Player position */
			int px = Misc.p_ptr.px;
			int py = Misc.p_ptr.py;

			/* Extract the blind-ness */
			bool blind = (Misc.p_ptr.timed[(int)Timed_Effect.BLIND] != 0 ? true : false);

			/* Extract the "see-able-ness" */
			bool seen = (!blind && m_ptr.ml);

			/* Assume "normal" target */
			bool normal = true;

			/* Handle "leaving" */
			if (Misc.p_ptr.leaving) return false;

			/* Cannot cast spells when confused */
			if (m_ptr.m_timed[(int)Misc.MON_TMD.CONF] != 0) return (false);

			/* Cannot cast spells when nice */
			if ((m_ptr.mflag & Monster_Flag.MFLAG_NICE) != 0) return false;

			/* Hack -- Extract the spell probability */
			chance = (r_ptr.freq_innate + r_ptr.freq_spell) / 2;

			/* Not allowed to cast spells */
			if (chance == 0) return false;

			/* Only do spells occasionally */
			if (Random.randint0(100) >= chance) return false;

			/* Hack -- require projectable player */
			if (normal)
			{
			    /* Check range */
			    if (m_ptr.cdis > Misc.MAX_RANGE) return false;

			    /* Check path */
				if (!Cave.projectable(m_ptr.fy, m_ptr.fx, py, px, Spell.PROJECT_NONE))
				    return false;
			}

			/* Extract the monster level */
			rlev = ((r_ptr.level >= 1) ? r_ptr.level : 1);

			/* Extract the racial spell flags */
			f.copy(r_ptr.spell_flags);

			/* Allow "desperate" spells */
			if (r_ptr.flags.has(Monster_Flag.SMART.value) &&
			    m_ptr.hp < m_ptr.maxhp / 10 &&
			    Random.randint0(100) < 50){
				throw new NotImplementedException();

				///* Require intelligent spells */
				//set_spells(f, RST_HASTE | RST_ANNOY | RST_ESCAPE | RST_HEAL | RST_TACTIC | RST_SUMMON);
			}

			throw new NotImplementedException();

			///* Remove the "ineffective" spells */
			//remove_bad_spells(m_idx, f);

			///* Check whether summons and bolts are worth it. */
			//if (!rf_has(r_ptr.flags, RF_STUPID))
			//{
			//    /* Check for a clean bolt shot */
			//    if (test_spells(f, RST_BOLT) &&
			//        !clean_shot(m_ptr.fy, m_ptr.fx, py, px))

			//        /* Remove spells that will only hurt friends */
			//        set_spells(f, ~RST_BOLT);

			//    /* Check for a possible summon */
			//    if (!(summon_possible(m_ptr.fy, m_ptr.fx)))

			//        /* Remove summoning spells */
			//        set_spells(f, ~RST_SUMMON);
			//}

			///* No spells left */
			//if (rsf_is_empty(f)) return false;

			///* Get the monster name (or "it") */
			//monster_desc(m_name, sizeof(m_name), m_ptr, 0x00);

			///* Get the monster possessive ("his"/"her"/"its") */
			//monster_desc(m_poss, sizeof(m_poss), m_ptr, MDESC_PRO2 | MDESC_POSS);

			///* Get the "died from" name */
			//monster_desc(ddesc, sizeof(ddesc), m_ptr, MDESC_SHOW | MDESC_IND2);

			///* Choose a spell to cast */
			//thrown_spell = choose_attack_spell(m_idx, f);

			///* Abort if no spell was chosen */
			//if (!thrown_spell) return false;

			///* If we see an unaware monster try to cast a spell, become aware of it */
			//if (m_ptr.unaware)
			//    become_aware(m_idx);

			///* Calculate spell failure rate */
			//failrate = 25 - (rlev + 3) / 4;
			//if (m_ptr.m_timed[MON_TMD_FEAR])
			//    failrate += 20;

			///* Stupid monsters will never fail (for jellies and such) */
			//if (OPT(birth_ai_smart) || rf_has(r_ptr.flags, RF_STUPID))
			//    failrate = 0;

			///* Check for spell failure (innate attacks never fail) */
			//if ((thrown_spell >= MIN_NONINNATE_SPELL) && (randint0(100) < failrate))
			//{
			//    /* Message */
			//    msg("%^s tries to cast a spell, but fails.", m_name);

			//    return true;
			//}

			///* Cast the spell. */
			//disturb(p_ptr, 1, 0);

			///* Special case RSF_HASTE until TMD_* and MON_TMD_* are rationalised */
			//if (thrown_spell == RSF_HASTE) {
			//    if (blind)
			//        msg("%^s mumbles.", m_name);
			//    else
			//        msg("%^s concentrates on %s body.", m_name, m_poss);

			//    (void)mon_inc_timed(m_idx, MON_TMD_FAST, 50, 0, false);
			//} else
			//    do_mon_spell(thrown_spell, m_idx, seen);

			///* Remember what the monster did to us */
			//if (seen) {
			//    rsf_on(l_ptr.spell_flags, thrown_spell);

			//    /* Innate spell */
			//    if (thrown_spell < MIN_NONINNATE_SPELL) {
			//        if (l_ptr.cast_innate < MAX_UCHAR)
			//            l_ptr.cast_innate++;
			//    } else {
			//    /* Bolt or Ball, or Special spell */
			//        if (l_ptr.cast_spell < MAX_UCHAR)
			//            l_ptr.cast_spell++;
			//    }
			//}
			///* Always take note of monsters that kill you */
			//if (p_ptr.is_dead && (l_ptr.deaths < MAX_SHORT)) {
			//    l_ptr.deaths++;
			//}

			///* A spell was cast */
			//return true;
		}

		/*
		 * Critical blow.  All hits that do 95% of total possible damage,
		 * and which also do at least 20 damage, or, sometimes, N damage.
		 * This is used only to determine "cuts" and "stuns".
		 */
		public static int monster_critical(int dice, int sides, int dam)
		{
			int max = 0;
			int total = dice * sides;

			/* Must do at least 95% of perfect */
			if (dam < total * 19 / 20) return (0);

			/* Weak blows rarely work */
			if ((dam < 20) && (Random.randint0(100) >= dam)) return (0);

			/* Perfect damage */
			if (dam == total) max++;

			/* Super-charge */
			if (dam >= 20)
			{
				while (Random.randint0(100) < 2) max++;
			}

			/* Critical damage */
			if (dam > 45) return (6 + max);
			if (dam > 33) return (5 + max);
			if (dam > 25) return (4 + max);
			if (dam > 18) return (3 + max);
			if (dam > 11) return (2 + max);
			return (1 + max);
		}


		/*
		 * Hack -- Precompute a bunch of calls to distance() in find_safety() and
		 * find_hiding().
		 *
		 * The pair of arrays dist_offsets_y[n] and dist_offsets_x[n] contain the
		 * offsets of all the locations with a distance of n from a central point,
		 * with an offset of (0,0) indicating no more offsets at this distance.
		 *
		 * This is, of course, fairly unreadable, but it eliminates multiple loops
		 * from the previous version.
		 *
		 * It is probably better to replace these arrays with code to compute
		 * the relevant arrays, even if the storage is pre-allocated in hard
		 * coded sizes.  At the very least, code should be included which is
		 * able to generate and dump these arrays (ala "los()").  XXX XXX XXX
		 *
		 * Also, the storage needs could be reduced by using char.  XXX XXX XXX
		 *
		 * These arrays could be combined into two big arrays, using sub-arrays
		 * to hold the offsets and lengths of each portion of the sub-arrays, and
		 * this could perhaps also be used somehow in the "look" code.  XXX XXX XXX
		 */
		//Nick: ohh My GOD this is horrible. Remove this IMMEDIATELY and just fucking call distance() @_@
		//XXX XXX XXX MAJOR HACK DA FUQ BBQ TODO FIX THIS FIRST ABOVE ALL ELSE

		static int[] d_off_y_0 =
		{ 0 };

		static int[] d_off_x_0 =
		{ 0 };


		static int[] d_off_y_1 =
		{ -1, -1, -1, 0, 0, 1, 1, 1, 0 };

		static int[] d_off_x_1 =
		{ -1, 0, 1, -1, 1, -1, 0, 1, 0 };


		static int[] d_off_y_2 =
		{ -1, -1, -2, -2, -2, 0, 0, 1, 1, 2, 2, 2, 0 };

		static int[] d_off_x_2 =
		{ -2, 2, -1, 0, 1, -2, 2, -2, 2, -1, 0, 1, 0 };


		static int[] d_off_y_3 =
		{ -1, -1, -2, -2, -3, -3, -3, 0, 0, 1, 1, 2, 2,
		  3, 3, 3, 0 };

		static int[] d_off_x_3 =
		{ -3, 3, -2, 2, -1, 0, 1, -3, 3, -3, 3, -2, 2,
		  -1, 0, 1, 0 };


		static int[] d_off_y_4 =
		{ -1, -1, -2, -2, -3, -3, -3, -3, -4, -4, -4, 0,
		  0, 1, 1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 0 };

		static int[] d_off_x_4 =
		{ -4, 4, -3, 3, -2, -3, 2, 3, -1, 0, 1, -4, 4,
		  -4, 4, -3, 3, -2, -3, 2, 3, -1, 0, 1, 0 };


		static int[] d_off_y_5 =
		{ -1, -1, -2, -2, -3, -3, -4, -4, -4, -4, -5, -5,
		  -5, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 4, 4, 5, 5,
		  5, 0 };

		static int[] d_off_x_5 =
		{ -5, 5, -4, 4, -4, 4, -2, -3, 2, 3, -1, 0, 1,
		  -5, 5, -5, 5, -4, 4, -4, 4, -2, -3, 2, 3, -1,
		  0, 1, 0 };


		static int[] d_off_y_6 =
		{ -1, -1, -2, -2, -3, -3, -4, -4, -5, -5, -5, -5,
		  -6, -6, -6, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5,
		  5, 5, 6, 6, 6, 0 };

		static int[] d_off_x_6 =
		{ -6, 6, -5, 5, -5, 5, -4, 4, -2, -3, 2, 3, -1,
		  0, 1, -6, 6, -6, 6, -5, 5, -5, 5, -4, 4, -2,
		  -3, 2, 3, -1, 0, 1, 0 };


		static int[] d_off_y_7 =
		{ -1, -1, -2, -2, -3, -3, -4, -4, -5, -5, -5, -5,
		  -6, -6, -6, -6, -7, -7, -7, 0, 0, 1, 1, 2, 2, 3,
		  3, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 7, 0 };

		static int[] d_off_x_7 =
		{ -7, 7, -6, 6, -6, 6, -5, 5, -4, -5, 4, 5, -2,
		  -3, 2, 3, -1, 0, 1, -7, 7, -7, 7, -6, 6, -6,
		  6, -5, 5, -4, -5, 4, 5, -2, -3, 2, 3, -1, 0,
		  1, 0 };


		static int[] d_off_y_8 =
		{ -1, -1, -2, -2, -3, -3, -4, -4, -5, -5, -6, -6,
		  -6, -6, -7, -7, -7, -7, -8, -8, -8, 0, 0, 1, 1,
		  2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7,
		  8, 8, 8, 0 };

		static int[] d_off_x_8 =
		{ -8, 8, -7, 7, -7, 7, -6, 6, -6, 6, -4, -5, 4,
		  5, -2, -3, 2, 3, -1, 0, 1, -8, 8, -8, 8, -7,
		  7, -7, 7, -6, 6, -6, 6, -4, -5, 4, 5, -2, -3,
		  2, 3, -1, 0, 1, 0 };


		static int[] d_off_y_9 =
		{ -1, -1, -2, -2, -3, -3, -4, -4, -5, -5, -6, -6,
		  -7, -7, -7, -7, -8, -8, -8, -8, -9, -9, -9, 0,
		  0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 7,
		  7, 8, 8, 8, 8, 9, 9, 9, 0 };

		static int[] d_off_x_9 =
		{ -9, 9, -8, 8, -8, 8, -7, 7, -7, 7, -6, 6, -4,
		  -5, 4, 5, -2, -3, 2, 3, -1, 0, 1, -9, 9, -9,
		  9, -8, 8, -8, 8, -7, 7, -7, 7, -6, 6, -4, -5,
		  4, 5, -2, -3, 2, 3, -1, 0, 1, 0 };


		static int[][] dist_offsets_y =
		{
			d_off_y_0, d_off_y_1, d_off_y_2, d_off_y_3, d_off_y_4,
			d_off_y_5, d_off_y_6, d_off_y_7, d_off_y_8, d_off_y_9
		};

		static int[][] dist_offsets_x =
		{
			d_off_x_0, d_off_x_1, d_off_x_2, d_off_x_3, d_off_x_4,
			d_off_x_5, d_off_x_6, d_off_x_7, d_off_x_8, d_off_x_9
		};


		/*
		 * Choose a "safe" location near a monster for it to run toward.
		 *
		 * A location is "safe" if it can be reached quickly and the player
		 * is not able to fire into it (it isn't a "clean shot").  So, this will
		 * cause monsters to "duck" behind walls.  Hopefully, monsters will also
		 * try to run towards corridor openings if they are in a room.
		 *
		 * This function may take lots of CPU time if lots of monsters are fleeing.
		 *
		 * Return true if a safe location is available.
		 */
		public static bool find_safety(Cave c, int m_idx, out int yp, out int xp)
		{
			yp = 0;
			xp = 0;

			Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);

			int fy = m_ptr.fy;
			int fx = m_ptr.fx;

			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;

			int i, y, x, dy, dx, d, dis;
			int gy = 0, gx = 0, gdis = 0;

			int[] y_offsets;
			int[] x_offsets;

			/* Start with adjacent locations, spread further */
			for (d = 1; d < 10; d++)
			{
				/* Get the lists of points with a distance d from (fx, fy) */
				y_offsets = dist_offsets_y[d];
				x_offsets = dist_offsets_x[d];

				/* Check the locations */
				for (i = 0, dx = x_offsets[0], dy = y_offsets[0];
					 dx != 0 || dy != 0;
					 i++, dx = x_offsets[i], dy = y_offsets[i])
				{
					y = fy + dy;
					x = fx + dx;

					/* Skip illegal locations */
					if (!Cave.cave.in_bounds_fully(y, x)) continue;

					/* Skip locations in a wall */
					if (!Cave.cave_floor_bold(y, x)) continue;

					/* Ignore grids very far from the player */
					if (c.when[y][x] < c.when[py][px]) continue;

					/* Ignore too-distant grids */
					if (c.cost[y][x] > c.cost[fy][fx] + 2 * d) continue;

					/* Check for absence of shot (more or less) */
					if (!Cave.player_has_los_bold(y,x))
					{
						/* Calculate distance from player */
						dis = Cave.distance(y, x, py, px);

						/* Remember if further than previous */
						if (dis > gdis)
						{
							gy = y;
							gx = x;
							gdis = dis;
						}
					}
				}

				/* Check for success */
				if (gdis > 0)
				{
					/* Good location */
					yp = fy - gy;
					xp = fx - gx;

					/* Found safe place */
					return (true);
				}
			}

			/* No safe place */
			return (false);
		}

		/*
		 * Provide a location to flee to, but give the player a wide berth.
		 *
		 * A monster may wish to flee to a location that is behind the player,
		 * but instead of heading directly for it, the monster should "swerve"
		 * around the player so that he has a smaller chance of getting hit.
		 */
		static bool get_fear_moves_aux(Cave c, int m_idx, ref int yp, ref int xp)
		{
			int y, x, y1, x1, fy, fx, py, px, gy = 0, gx = 0;
			int when = 0, score = -1;
			int i;

			Monster m_ptr = Cave.cave_monster(Cave.cave, m_idx);
			Monster_Race r_ptr = Misc.r_info[m_ptr.r_idx];

			/* Player location */
			py = Misc.p_ptr.py;
			px = Misc.p_ptr.px;

			/* Monster location */
			fy = m_ptr.fy;
			fx = m_ptr.fx;

			/* Desired destination */
			y1 = fy - yp;
			x1 = fx - xp;

			/* The player is not currently near the monster grid */
			if (c.when[fy][fx] < c.when[py][px])
			{
				/* No reason to attempt flowing */
				return (false);
			}

			/* Monster is too far away to use flow information */
			if (c.cost[fy][fx] > Misc.MONSTER_FLOW_DEPTH) return (false);
			if (c.cost[fy][fx] > r_ptr.aaf) return (false);

			/* Check nearby grids, diagonals first */
			for (i = 7; i >= 0; i--)
			{
				int dis, s;

				/* Get the location */
				y = fy + Misc.ddy_ddd[i];
				x = fx + Misc.ddx_ddd[i];

				/* Ignore illegal locations */
				if (c.when[y][x] == 0) continue;

				/* Ignore ancient locations */
				if (c.when[y][x] < when) continue;

				/* Calculate distance of this grid from our destination */
				dis = Cave.distance(y, x, y1, x1);

				/* Score this grid */
				s = 5000 / (dis + 3) - 500 / (c.cost[y][x] + 1);

				/* No negative scores */
				if (s < 0) s = 0;

				/* Ignore lower scores */
				if (s < score) continue;

				/* Save the score and time */
				when = c.when[y][x];
				score = s;

				/* Save the location */
				gy = y;
				gx = x;
			}

			/* No legal move (?) */
			if (when == 0) return (false);

			/* Find deltas */
			yp = fy - gy;
			xp = fx - gx;

			/* Success */
			return (true);
		}

		/**
		 * Update monster knowledge of player resists.
		 *
		 * \param m_idx is the monster who is learning
		 * \param type is the GF_ type to which it's learning about the player's
		 *    resistance (or lack of)
		 */
		void monster_learn_resists(Player.Player p, GF type)
		{
			//GF gf_ptr = &gf_table[type];

			update_smart_learn(p, type.resist);
			update_smart_learn(p, type.immunity);
			update_smart_learn(p, type.vuln);

			return;
		}

	}
}
