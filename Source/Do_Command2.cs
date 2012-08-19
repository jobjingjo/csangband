using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;

namespace CSAngband {
	partial class Do_Command {
		/*
		 * Go up one level
		 */
		public static void go_up(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			///* Verify stairs */
			//if (cave.feat[p_ptr.py][p_ptr.px] != FEAT_LESS)
			//{
			//    msg("I see no up staircase here.");
			//    return;
			//}

			///* Ironman */
			//if (OPT(birth_ironman))
			//{
			//    msg("Nothing happens!");
			//    return;
			//}

			///* Hack -- take a turn */
			//p_ptr.energy_use = 100;

			///* Success */
			//msgt(MSG_STAIRS_UP, "You enter a maze of up staircases.");

			///* Create a way back */
			//p_ptr.create_up_stair = false;
			//p_ptr.create_down_stair = true;

			///* Change level */
			//dungeon_change_level(p_ptr.depth - 1);
		}


		/*
		 * Go down one level
		 */
		public static void go_down(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			///* Verify stairs */
			//if (cave.feat[p_ptr.py][p_ptr.px] != FEAT_MORE)
			//{
			//    msg("I see no down staircase here.");
			//    return;
			//}

			///* Hack -- take a turn */
			//p_ptr.energy_use = 100;

			///* Success */
			//msgt(MSG_STAIRS_DOWN, "You enter a maze of down staircases.");

			///* Create a way back */
			//p_ptr.create_up_stair = true;
			//p_ptr.create_down_stair = false;

			///* Change level */
			//dungeon_change_level(p_ptr.depth + 1);
		}



		/*
		 * Simple command to "search" for one turn
		 */
		public static void search(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			///* Only take a turn if attempted */
			//if (search(true))
			//    p_ptr.energy_use = 100;
		}


		/*
		 * Hack -- toggle search mode
		 */
		public static void toggle_search(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			///* Stop searching */
			//if (p_ptr.searching)
			//{
			//    /* Clear the searching flag */
			//    p_ptr.searching = false;

			//    /* Recalculate bonuses */
			//    p_ptr.update |= (PU_BONUS);

			//    /* Redraw the state */
			//    p_ptr.redraw |= (PR_STATE);
			//}

			///* Start searching */
			//else
			//{
			//    /* Set the searching flag */
			//    p_ptr.searching = true;

			//    /* Update stuff */
			//    p_ptr.update |= (PU_BONUS);

			//    /* Redraw stuff */
			//    p_ptr.redraw |= (PR_STATE | PR_SPEED);
			//}
		}

		/*
		 * Attempt to open the given chest at the given location
		 *
		 * Assume there is no monster blocking the destination
		 *
		 * Returns true if repeated commands may continue
		 */
		static bool open_chest(int y, int x, short o_idx) {
			throw new NotImplementedException();
			//int i, j;

			//bool flag = true;

			//bool more = false;

			//object_type *o_ptr = object_byid(o_idx);


			///* Attempt to unlock it */
			//if (o_ptr.pval[DEFAULT_PVAL] > 0)
			//{
			//    /* Assume locked, and thus not open */
			//    flag = false;

			//    /* Get the "disarm" factor */
			//    i = p_ptr.state.skills[SKILL_DISARM];

			//    /* Penalize some conditions */
			//    if (p_ptr.timed[TMD_BLIND] || no_light()) i = i / 10;
			//    if (p_ptr.timed[TMD_CONFUSED] || p_ptr.timed[TMD_IMAGE]) i = i / 10;

			//    /* Extract the difficulty */
			//    j = i - o_ptr.pval[DEFAULT_PVAL];

			//    /* Always have a small chance of success */
			//    if (j < 2) j = 2;

			//    /* Success -- May still have traps */
			//    if (randint0(100) < j)
			//    {
			//        msgt(MSG_LOCKPICK, "You have picked the lock.");
			//        player_exp_gain(p_ptr, 1);
			//        flag = true;
			//    }

			//    /* Failure -- Keep trying */
			//    else
			//    {
			//        /* We may continue repeating */
			//        more = true;
			//        flush();
			//        msgt(MSG_LOCKPICK_FAIL, "You failed to pick the lock.");
			//    }
			//}

			///* Allowed to open */
			//if (flag)
			//{
			//    /* Apply chest traps, if any */
			//    chest_trap(y, x, o_idx);

			//    /* Let the Chest drop items */
			//    chest_death(y, x, o_idx);

			//    /* Squelch chest if autosquelch calls for it */
			//    p_ptr.notice |= PN_SQUELCH;

			//    /* Redraw chest, to be on the safe side (it may have been squelched) */
			//    cave_light_spot(cave, y, x);
			//}

			///* Result */
			//return (more);
		}


		/*
		 * Attempt to disarm the chest at the given location
		 *
		 * Assume there is no monster blocking the destination
		 *
		 * Returns true if repeated commands may continue
		 */
		static bool disarm_chest(int y, int x, short o_idx) {
			throw new NotImplementedException();
			//int i, j;

			//bool more = false;

			//object_type *o_ptr = object_byid(o_idx);


			///* Get the "disarm" factor */
			//i = p_ptr.state.skills[SKILL_DISARM];

			///* Penalize some conditions */
			//if (p_ptr.timed[TMD_BLIND] || no_light()) i = i / 10;
			//if (p_ptr.timed[TMD_CONFUSED] || p_ptr.timed[TMD_IMAGE]) i = i / 10;

			///* Extract the difficulty */
			//j = i - o_ptr.pval[DEFAULT_PVAL];

			///* Always have a small chance of success */
			//if (j < 2) j = 2;

			///* Must find the trap first. */
			//if (!object_is_known(o_ptr))
			//{
			//    msg("I don't see any traps.");
			//}

			///* Already disarmed/unlocked */
			//else if (o_ptr.pval[DEFAULT_PVAL] <= 0)
			//{
			//    msg("The chest is not trapped.");
			//}

			///* No traps to find. */
			//else if (!chest_traps[o_ptr.pval[DEFAULT_PVAL]])
			//{
			//    msg("The chest is not trapped.");
			//}

			///* Success (get a lot of experience) */
			//else if (randint0(100) < j)
			//{
			//    msgt(MSG_DISARM, "You have disarmed the chest.");
			//    player_exp_gain(p_ptr, o_ptr.pval[DEFAULT_PVAL]);
			//    o_ptr.pval[DEFAULT_PVAL] = (0 - o_ptr.pval[DEFAULT_PVAL]);
			//}

			///* Failure -- Keep trying */
			//else if ((i > 5) && (randint1(i) > 5))
			//{
			//    /* We may keep trying */
			//    more = true;
			//    flush();
			//    msg("You failed to disarm the chest.");
			//}

			///* Failure -- Set off the trap */
			//else
			//{
			//    msg("You set off a trap!");
			//    chest_trap(y, x, o_idx);
			//}

			///* Result */
			//return (more);
		}


		/*
		 * Determine if a given grid may be "opened"
		 */
		static bool open_test(int y, int x) {
			throw new NotImplementedException();
			///* Must have knowledge */
			//if (!(cave.info[y][x] & (CAVE_MARK))) {
			//    msg("You see nothing there.");
			//    return false;
			//}

			//if (!cave_iscloseddoor(cave, y, x)) {
			//    msgt(MSG_NOTHING_TO_OPEN, "You see nothing there to open.");
			//    return false;
			//}

			///* Okay */
			//return (true);
		}


		/*
		 * Perform the basic "open" command on doors
		 *
		 * Assume there is no monster blocking the destination
		 *
		 * Returns true if repeated commands may continue
		 */
		static bool open_aux(int y, int x) {
			throw new NotImplementedException();
			//int i, j;

			//bool more = false;


			///* Verify legality */
			//if (!do_cmd_open_test(y, x)) return (false);


			///* Jammed door */
			//if (cave_isjammeddoor(cave, y, x))
			//{
			//    msg("The door appears to be stuck.");
			//}

			///* Locked door */
			//else if (cave_islockeddoor(cave, y, x))
			//{
			//    /* Disarm factor */
			//    i = p_ptr.state.skills[SKILL_DISARM];

			//    /* Penalize some conditions */
			//    if (p_ptr.timed[TMD_BLIND] || no_light()) i = i / 10;
			//    if (p_ptr.timed[TMD_CONFUSED] || p_ptr.timed[TMD_IMAGE]) i = i / 10;

			//    /* Extract the lock power */
			//    j = cave.feat[y][x] - FEAT_DOOR_HEAD;

			//    /* Extract the difficulty XXX XXX XXX */
			//    j = i - (j * 4);

			//    /* Always have a small chance of success */
			//    if (j < 2) j = 2;

			//    /* Success */
			//    if (randint0(100) < j)
			//    {
			//        /* Message */
			//        msgt(MSG_LOCKPICK, "You have picked the lock.");

			//        /* Open the door */
			//        cave_set_feat(cave, y, x, FEAT_OPEN);

			//        /* Update the visuals */
			//        p_ptr.update |= (PU_UPDATE_VIEW | PU_MONSTERS);

			//        /* Experience */
			//        /* Removed to avoid exploit by repeatedly locking and unlocking door */
			//        /* player_exp_gain(p_ptr, 1); */
			//    }

			//    /* Failure */
			//    else
			//    {
			//        flush();

			//        /* Message */
			//        msgt(MSG_LOCKPICK_FAIL, "You failed to pick the lock.");

			//        /* We may keep trying */
			//        more = true;
			//    }
			//}

			///* Closed door */
			//else
			//{
			//    /* Open the door */
			//    cave_set_feat(cave, y, x, FEAT_OPEN);

			//    /* Update the visuals */
			//    p_ptr.update |= (PU_UPDATE_VIEW | PU_MONSTERS);

			//    /* Sound */
			//    sound(MSG_OPENDOOR);
			//}

			///* Result */
			//return (more);
		}



		/*
		 * Open a closed/locked/jammed door or a closed/locked chest.
		 *
		 * Unlocking a locked door/chest is worth one experience point.
		 */
		public static void open(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int y, x, dir;

			//s16b o_idx;

			//bool more = false;

			//dir = args[0].direction;

			///* Get location */
			//y = p_ptr.py + ddy[dir];
			//x = p_ptr.px + ddx[dir];

			///* Check for chests */
			//o_idx = chest_check(y, x);


			///* Verify legality */
			//if (!o_idx && !do_cmd_open_test(y, x))
			//{
			//    /* Cancel repeat */
			//    disturb(p_ptr, 0, 0);
			//    return;
			//}

			///* Take a turn */
			//p_ptr.energy_use = 100;

			///* Apply confusion */
			//if (player_confuse_dir(p_ptr, &dir, false))
			//{
			//    /* Get location */
			//    y = p_ptr.py + ddy[dir];
			//    x = p_ptr.px + ddx[dir];

			//    /* Check for chest */
			//    o_idx = chest_check(y, x);
			//}


			///* Monster */
			//if (cave.m_idx[y][x] > 0)
			//{
			//    int m_idx = cave.m_idx[y][x];

			//    /* Mimics surprise the player */
			//    if (is_mimicking(m_idx)) {
			//        become_aware(m_idx);

			//        /* Mimic wakes up */
			//        mon_clear_timed(m_idx, MON_TMD_SLEEP, MON_TMD_FLG_NOMESSAGE, false);
			//    } else {
			//        /* Message */
			//        msg("There is a monster in the way!");

			//        /* Attack */
			//        py_attack(y, x);
			//    }
			//}

			///* Chest */
			//else if (o_idx)
			//{
			//    /* Open the chest */
			//    more = do_cmd_open_chest(y, x, o_idx);
			//}

			///* Door */
			//else
			//{
			//    /* Open the door */
			//    more = do_cmd_open_aux(y, x);
			//}

			///* Cancel repeat unless we may continue */
			//if (!more) disturb(p_ptr, 0, 0);
		}


		/*
		 * Determine if a given grid may be "closed"
		 */
		static bool close_test(int y, int x) {
			throw new NotImplementedException();
			///* Must have knowledge */
			//if (!(cave.info[y][x] & (CAVE_MARK)))
			//{
			//    /* Message */
			//    msg("You see nothing there.");

			//    /* Nope */
			//    return (false);
			//}

			///* Require open/broken door */
			//if ((cave.feat[y][x] != FEAT_OPEN) &&
			//    (cave.feat[y][x] != FEAT_BROKEN))
			//{
			//    /* Message */
			//    msg("You see nothing there to close.");

			//    /* Nope */
			//    return (false);
			//}

			///* Okay */
			//return (true);
		}


		/*
		 * Perform the basic "close" command
		 *
		 * Assume there is no monster blocking the destination
		 *
		 * Returns true if repeated commands may continue
		 */
		static bool close_aux(int y, int x) {
			throw new NotImplementedException();
			//bool more = false;

			///* Verify legality */
			//if (!do_cmd_close_test(y, x)) return (false);

			///* Broken door */
			//if (cave.feat[y][x] == FEAT_BROKEN)
			//{
			//    /* Message */
			//    msg("The door appears to be broken.");
			//}

			///* Open door */
			//else
			//{
			//    /* Close the door */
			//    cave_set_feat(cave, y, x, FEAT_DOOR_HEAD);

			//    /* Update the visuals */
			//    p_ptr.update |= (PU_UPDATE_VIEW | PU_MONSTERS);

			//    /* Sound */
			//    sound(MSG_SHUTDOOR);
			//}

			///* Result */
			//return (more);
		}


		/*
		 * Close an open door.
		 */
		public static void close(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int y, x, dir;

			//bool more = false;

			//dir = args[0].direction;

			///* Get location */
			//y = p_ptr.py + ddy[dir];
			//x = p_ptr.px + ddx[dir];

			///* Verify legality */
			//if (!do_cmd_close_test(y, x))
			//{
			//    /* Cancel repeat */
			//    disturb(p_ptr, 0, 0);
			//    return;
			//}

			///* Take a turn */
			//p_ptr.energy_use = 100;

			///* Apply confusion */
			//if (player_confuse_dir(p_ptr, &dir, false))
			//{
			//    /* Get location */
			//    y = p_ptr.py + ddy[dir];
			//    x = p_ptr.px + ddx[dir];
			//}


			///* Monster */
			//if (cave.m_idx[y][x] > 0)
			//{
			//    /* Message */
			//    msg("There is a monster in the way!");

			//    /* Attack */
			//    py_attack(y, x);
			//}

			///* Door */
			//else
			//{
			//    /* Close door */
			//    more = do_cmd_close_aux(y, x);
			//}

			///* Cancel repeat unless told not to */
			//if (!more) disturb(p_ptr, 0, 0);
		}


		/*
		 * Determine if a given grid may be "tunneled"
		 */
		static bool tunnel_test(int y, int x) {
			throw new NotImplementedException();
			///* Must have knowledge */
			//if (!(cave.info[y][x] & (CAVE_MARK)))
			//{
			//    /* Message */
			//    msg("You see nothing there.");

			//    /* Nope */
			//    return (false);
			//}

			///* Must be a wall/door/etc */
			//if (cave_floor_bold(y, x))
			//{
			//    /* Message */
			//    msg("You see nothing there to tunnel.");

			//    /* Nope */
			//    return (false);
			//}

			///* Okay */
			//return (true);
		}



		/*
		 * Perform the basic "tunnel" command
		 *
		 * Assumes that no monster is blocking the destination
		 *
		 * Uses "twall" (above) to do all "terrain feature changing".
		 *
		 * Returns true if repeated commands may continue
		 */
		static bool tunnel_aux(int y, int x) {
			throw new NotImplementedException();
			//bool more = false;


			///* Verify legality */
			//if (!do_cmd_tunnel_test(y, x)) return (false);


			///* Sound XXX XXX XXX */
			///* sound(MSG_DIG); */

			///* Titanium */
			//if (cave.feat[y][x] >= FEAT_PERM_EXTRA)
			//{
			//    msg("This seems to be permanent rock.");
			//}

			///* Granite */
			//else if (cave.feat[y][x] >= FEAT_WALL_EXTRA)
			//{
			//    /* Tunnel */
			//    if ((p_ptr.state.skills[SKILL_DIGGING] > 40 + randint0(1600)) && twall(y, x))
			//    {
			//        msg("You have finished the tunnel.");
			//    }

			//    /* Keep trying */
			//    else
			//    {
			//        /* We may continue tunelling */
			//        msg("You tunnel into the granite wall.");
			//        more = true;
			//    }
			//}

			///* Quartz / Magma */
			//else if (cave.feat[y][x] >= FEAT_MAGMA)
			//{
			//    bool okay = false;
			//    bool gold = false;
			//    bool hard = false;

			//    /* Found gold */
			//    if (cave.feat[y][x] >= FEAT_MAGMA_H)
			//    {
			//        gold = true;
			//    }

			//    /* Extract "quartz" flag XXX XXX XXX */
			//    if ((cave.feat[y][x] - FEAT_MAGMA) & 0x01)
			//    {
			//        hard = true;
			//    }

			//    /* Quartz */
			//    if (hard)
			//    {
			//        okay = (p_ptr.state.skills[SKILL_DIGGING] > 20 + randint0(800));
			//    }

			//    /* Magma */
			//    else
			//    {
			//        okay = (p_ptr.state.skills[SKILL_DIGGING] > 10 + randint0(400));
			//    }

			//    /* Success */
			//    if (okay && twall(y, x))
			//    {
			//        /* Found treasure */
			//        if (gold)
			//        {
			//            /* Place some gold */
			//            place_gold(cave, y, x, p_ptr.depth, ORIGIN_FLOOR);

			//            /* Message */
			//            msg("You have found something!");
			//        }

			//        /* Found nothing */
			//        else
			//        {
			//            /* Message */
			//            msg("You have finished the tunnel.");
			//        }
			//    }

			//    /* Failure (quartz) */
			//    else if (hard)
			//    {
			//        /* Message, continue digging */
			//        msg("You tunnel into the quartz vein.");
			//        more = true;
			//    }

			//    /* Failure (magma) */
			//    else
			//    {
			//        /* Message, continue digging */
			//        msg("You tunnel into the magma vein.");
			//        more = true;
			//    }
			//}

			///* Rubble */
			//else if (cave.feat[y][x] == FEAT_RUBBLE)
			//{
			//    /* Remove the rubble */
			//    if ((p_ptr.state.skills[SKILL_DIGGING] > randint0(200)) && twall(y, x))
			//    {
			//        /* Message */
			//        msg("You have removed the rubble.");

			//        /* Hack -- place an object */
			//        if (randint0(100) < 10)	{
			//            /* Create a simple object */
			//            place_object(cave, y, x, p_ptr.depth, false, false,
			//                ORIGIN_RUBBLE);

			//            /* Observe the new object */
			//            if (!squelch_item_ok(object_byid(cave.o_idx[y][x])) &&
			//                    player_can_see_bold(y, x))
			//                msg("You have found something!");
			//        }
			//    }

			//    else
			//    {
			//        /* Message, keep digging */
			//        msg("You dig in the rubble.");
			//        more = true;
			//    }
			//}

			///* Secret doors */
			//else if (cave.feat[y][x] >= FEAT_SECRET)
			//{
			//    /* Tunnel */
			//    if ((p_ptr.state.skills[SKILL_DIGGING] > 30 + randint0(1200)) && twall(y, x))
			//    {
			//        msg("You have finished the tunnel.");
			//    }

			//    /* Keep trying */
			//    else
			//    {
			//        /* We may continue tunelling */
			//        msg("You tunnel into the granite wall.");
			//        more = true;

			//        /* Occasional Search XXX XXX */
			//        if (randint0(100) < 25) search(false);
			//    }
			//}

			///* Doors */
			//else
			//{
			//    /* Tunnel */
			//    if ((p_ptr.state.skills[SKILL_DIGGING] > 30 + randint0(1200)) && twall(y, x))
			//    {
			//        msg("You have finished the tunnel.");
			//    }

			//    /* Keep trying */
			//    else
			//    {
			//        /* We may continue tunelling */
			//        msg("You tunnel into the door.");
			//        more = true;
			//    }
			//}

			///* Result */
			//return (more);
		}


		/*
		 * Tunnel through "walls" (including rubble and secret doors)
		 *
		 * Digging is very difficult without a "digger" weapon, but can be
		 * accomplished by strong players using heavy weapons.
		 */
		public static void tunnel(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int y, x, dir;
			//bool more = false;

			//dir = args[0].direction;

			///* Get location */
			//y = p_ptr.py + ddy[dir];
			//x = p_ptr.px + ddx[dir];


			///* Oops */
			//if (!do_cmd_tunnel_test(y, x))
			//{
			//    /* Cancel repeat */
			//    disturb(p_ptr, 0, 0);
			//    return;
			//}

			///* Take a turn */
			//p_ptr.energy_use = 100;

			///* Apply confusion */
			//if (player_confuse_dir(p_ptr, &dir, false))
			//{
			//    /* Get location */
			//    y = p_ptr.py + ddy[dir];
			//    x = p_ptr.px + ddx[dir];
			//}


			///* Monster */
			//if (cave.m_idx[y][x] > 0)
			//{
			//    /* Message */
			//    msg("There is a monster in the way!");

			//    /* Attack */
			//    py_attack(y, x);
			//}

			///* Walls */
			//else
			//{
			//    /* Tunnel through walls */
			//    more = do_cmd_tunnel_aux(y, x);
			//}

			///* Cancel repetition unless we can continue */
			//if (!more) disturb(p_ptr, 0, 0);
		}

		/*
		 * Determine if a given grid may be "disarmed"
		 */
		static bool disarm_test(int y, int x) {
			throw new NotImplementedException();
			///* Must have knowledge */
			//if (!(cave.info[y][x] & (CAVE_MARK))) {
			//    msg("You see nothing there.");
			//    return false;
			//}

			///* Look for a closed, unlocked door to lock */
			//if (cave.feat[y][x] == FEAT_DOOR_HEAD)	return true;

			///* Look for a trap */
			//if (!cave_isknowntrap(cave, y, x)) {
			//    msg("You see nothing there to disarm.");
			//    return false;
			//}

			///* Okay */
			//return true;
		}


		/*
		 * Perform the command "lock door"
		 *
		 * Assume there is no monster blocking the destination
		 *
		 * Returns true if repeated commands may continue
		 */
		static bool lock_door(int y, int x) {
			throw new NotImplementedException();
			//int i, j, power;
			//bool more = false;

			///* Verify legality */
			//if (!do_cmd_disarm_test(y, x)) return false;

			///* Get the "disarm" factor */
			//i = p_ptr.state.skills[SKILL_DISARM];

			///* Penalize some conditions */
			//if (p_ptr.timed[TMD_BLIND] || no_light())
			//    i = i / 10;
			//if (p_ptr.timed[TMD_CONFUSED] || p_ptr.timed[TMD_IMAGE])
			//    i = i / 10;

			///* Calculate lock "power" */
			//power = m_bonus(7, p_ptr.depth);

			///* Extract the difficulty */
			//j = i - power;

			///* Always have a small chance of success */
			//if (j < 2) j = 2;

			///* Success */
			//if (randint0(100) < j) {
			//    msg("You lock the door.");
			//    cave_set_feat(cave, y, x, FEAT_DOOR_HEAD + power);
			//}

			///* Failure -- Keep trying */
			//else if ((i > 5) && (randint1(i) > 5)) {
			//    flush();
			//    msg("You failed to lock the door.");

			//    /* We may keep trying */
			//    more = true;
			//}
			///* Failure */
			//else
			//    msg("You failed to lock the door.");

			///* Result */
			//return more;
		}


		/*
		 * Perform the basic "disarm" command
		 *
		 * Assume there is no monster blocking the destination
		 *
		 * Returns true if repeated commands may continue
		 */
		static bool disarm_aux(int y, int x) {
			throw new NotImplementedException();
			//int i, j, power;

			//const char *name;

			//bool more = false;


			///* Verify legality */
			//if (!do_cmd_disarm_test(y, x)) return (false);


			///* Get the trap name */
			//name = f_info[cave.feat[y][x]].name;

			///* Get the "disarm" factor */
			//i = p_ptr.state.skills[SKILL_DISARM];

			///* Penalize some conditions */
			//if (p_ptr.timed[TMD_BLIND] || no_light()) i = i / 10;
			//if (p_ptr.timed[TMD_CONFUSED] || p_ptr.timed[TMD_IMAGE]) i = i / 10;

			///* XXX XXX XXX Variable power? */

			///* Extract trap "power" */
			//power = 5;

			///* Extract the difficulty */
			//j = i - power;

			///* Always have a small chance of success */
			//if (j < 2) j = 2;

			///* Success */
			//if (randint0(100) < j)
			//{
			//    /* Message */
			//    msgt(MSG_DISARM, "You have disarmed the %s.", name);

			//    /* Reward */
			//    player_exp_gain(p_ptr, power);

			//    /* Forget the trap */
			//    cave.info[y][x] &= ~(CAVE_MARK);

			//    /* Remove the trap */
			//    cave_set_feat(cave, y, x, FEAT_FLOOR);
			//}

			///* Failure -- Keep trying */
			//else if ((i > 5) && (randint1(i) > 5))
			//{
			//    flush();

			//    /* Message */
			//    msg("You failed to disarm the %s.", name);

			//    /* We may keep trying */
			//    more = true;
			//}

			///* Failure -- Set off the trap */
			//else
			//{
			//    /* Message */
			//    msg("You set off the %s!", name);

			//    /* Hit the trap */
			//    hit_trap(y, x);
			//}

			///* Result */
			//return (more);
		}


		/*
		 * Disarms a trap, or a chest
		 */
		public static void disarm(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int y, x, dir;

			//s16b o_idx;

			//bool more = false;

			//dir = args[0].direction;

			///* Get location */
			//y = p_ptr.py + ddy[dir];
			//x = p_ptr.px + ddx[dir];

			///* Check for chests */
			//o_idx = chest_check(y, x);


			///* Verify legality */
			//if (!o_idx && !do_cmd_disarm_test(y, x))
			//{
			//    /* Cancel repeat */
			//    disturb(p_ptr, 0, 0);
			//    return;
			//}

			///* Take a turn */
			//p_ptr.energy_use = 100;

			///* Apply confusion */
			//if (player_confuse_dir(p_ptr, &dir, false))
			//{
			//    /* Get location */
			//    y = p_ptr.py + ddy[dir];
			//    x = p_ptr.px + ddx[dir];

			//    /* Check for chests */
			//    o_idx = chest_check(y, x);
			//}


			///* Monster */
			//if (cave.m_idx[y][x] > 0) {
			//    msg("There is a monster in the way!");
			//    py_attack(y, x);
			//}

			///* Chest */
			//else if (o_idx)
			//    more = do_cmd_disarm_chest(y, x, o_idx);

			///* Door to lock */
			//else if (cave.feat[y][x] == FEAT_DOOR_HEAD)
			//    more = do_cmd_lock_door(y, x);

			///* Disarm trap */
			//else
			//    more = do_cmd_disarm_aux(y, x);

			///* Cancel repeat unless told not to */
			//if (!more) disturb(p_ptr, 0, 0);
		}


		/*
		 * Determine if a given grid may be "bashed"
		 */
		static bool bash_test(int y, int x) {
			throw new NotImplementedException();
			///* Must have knowledge */
			//if (!(cave.info[y][x] & (CAVE_MARK))) {
			//    msg("You see nothing there.");
			//    return (false);
			//}

			//if (!cave_iscloseddoor(cave, y, x)) {
			//    msg("You see nothing there to bash.");
			//    return false;
			//}

			///* Okay */
			//return (true);
		}


		/*
		 * Perform the basic "bash" command
		 *
		 * Assume there is no monster blocking the destination
		 *
		 * Returns true if repeated commands may continue
		 */
		static bool bash_aux(int y, int x) {
			throw new NotImplementedException();
			//int bash, temp;

			//bool more = false;


			///* Verify legality */
			//if (!do_cmd_bash_test(y, x)) return (false);


			///* Message */
			//msg("You smash into the door!");

			///* Hack -- Bash power based on strength */
			///* (Ranges from 3 to 20 to 100 to 200) */
			//bash = adj_str_blow[p_ptr.state.stat_ind[A_STR]];

			///* Extract door power */
			//temp = ((cave.feat[y][x] - FEAT_DOOR_HEAD) & 0x07);

			///* Compare bash power to door power */
			//temp = (bash - (temp * 10));

			///* Hack -- always have a chance */
			//if (temp < 1) temp = 1;

			///* Hack -- attempt to bash down the door */
			//if (randint0(100) < temp)
			//{
			//    /* Break down the door */
			//    if (randint0(100) < 50)
			//    {
			//        cave_set_feat(cave, y, x, FEAT_BROKEN);
			//    }

			//    /* Open the door */
			//    else
			//    {
			//        cave_set_feat(cave, y, x, FEAT_OPEN);
			//    }

			//    msgt(MSG_OPENDOOR, "The door crashes open!");

			//    /* Update the visuals */
			//    p_ptr.update |= (PU_UPDATE_VIEW | PU_MONSTERS);
			//}

			///* Saving throw against stun */
			//else if (randint0(100) < adj_dex_safe[p_ptr.state.stat_ind[A_DEX]] +
			//         p_ptr.lev) {
			//    msg("The door holds firm.");

			//    /* Allow repeated bashing */
			//    more = true;
			//}

			///* Low dexterity has bad consequences */
			//else {
			//    msg("You are off-balance.");

			//    /* Lose balance ala stun */
			//    (void)player_inc_timed(p_ptr, TMD_STUN, 2 + randint0(2), true, false);
			//}

			///* Result */
			//return more;
		}


		/*
		 * Bash open a door, success based on character strength
		 *
		 * For a closed door, pval is positive if locked; negative if stuck.
		 *
		 * For an open door, pval is positive for a broken door.
		 *
		 * A closed door can be opened - harder if locked. Any door might be
		 * bashed open (and thereby broken). Bashing a door is (potentially)
		 * faster! You move into the door way. To open a stuck door, it must
		 * be bashed. A closed door can be jammed (see do_cmd_spike()).
		 *
		 * Creatures can also open or bash doors, see elsewhere.
		 */
		public static void bash(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int y, x, dir;
			//bool more = false;

			//dir = args[0].direction;

			///* Get location */
			//y = p_ptr.py + ddy[dir];
			//x = p_ptr.px + ddx[dir];


			///* Verify legality */
			//if (!do_cmd_bash_test(y, x))
			//{
			//    /* Cancel repeat */
			//    disturb(p_ptr, 0, 0);
			//    return;
			//}

			///* Take a turn */
			//p_ptr.energy_use = 100;

			///* Apply confusion */
			//if (player_confuse_dir(p_ptr, &dir, false))
			//{
			//    /* Get location */
			//    y = p_ptr.py + ddy[dir];
			//    x = p_ptr.px + ddx[dir];
			//}


			///* Monster */
			//if (cave.m_idx[y][x] > 0)
			//{
			//    /* Message */
			//    msg("There is a monster in the way!");

			//    /* Attack */
			//    py_attack(y, x);
			//}

			///* Door */
			//else
			//{
			//    /* Bash the door */
			//    more = do_cmd_bash_aux(y, x);
			//}

			///* Cancel repeat unless we may continue */
			//if (!more) disturb(p_ptr, 0, 0);
		}


		/*
		 * Manipulate an adjacent grid in some way
		 *
		 * Attack monsters, tunnel through walls, disarm traps, open doors.
		 *
		 * This command must always take energy, to prevent free detection
		 * of invisible monsters.
		 *
		 * The "semantics" of this command must be chosen before the player
		 * is confused, and it must be verified against the new grid.
		 */
		public static void alter_aux(int dir) {
			throw new NotImplementedException();
			//int y, x;
			//bool more = false;

			///* Get location */
			//y = p_ptr.py + ddy[dir];
			//x = p_ptr.px + ddx[dir];

			///* Take a turn */
			//p_ptr.energy_use = 100;

			///* Apply confusion */
			//if (player_confuse_dir(p_ptr, &dir, false)) {
			//    /* Get location */
			//    y = p_ptr.py + ddy[dir];
			//    x = p_ptr.px + ddx[dir];
			//}

			///* Attack monsters */
			//if (cave.m_idx[y][x] > 0)
			//    py_attack(y, x);

			///* Tunnel through walls and rubble */
			//else if (cave_isdiggable(cave, y, x))
			//    more = do_cmd_tunnel_aux(y, x);

			///* Open closed doors */
			//else if (cave_iscloseddoor(cave, y, x))
			//    more = do_cmd_open_aux(y, x);

			///* Disarm traps */
			//else if (cave_isknowntrap(cave, y, x))
			//    more = do_cmd_disarm_aux(y, x);

			///* Oops */
			//else
			//    msg("You spin around.");

			///* Cancel repetition unless we can continue */
			//if (!more) disturb(p_ptr, 0, 0);
		}

		public static void alter(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//do_cmd_alter_aux(args[0].direction);
		}



		/*
		 * Determine if a given grid may be "spiked"
		 */
		static bool spike_test(int y, int x) {
			throw new NotImplementedException();
			///* Must have knowledge */
			//if (!(cave.info[y][x] & (CAVE_MARK))) {
			//    msg("You see nothing there.");
			//    return false;
			//}

			///* Check if door is closed */
			//if (!cave_iscloseddoor(cave, y, x)) {
			//    msg("You see nothing there to spike.");
			//    return false;
			//}

			///* Check that the door is not fully spiked */
			//if (!(cave.feat[y][x] < FEAT_DOOR_TAIL)) {
			//    msg("You can't use more spikes on this door.");
			//    return false;
			//}

			///* Okay */
			//return true;
		}


		/*
		 * Jam a closed door with a spike
		 *
		 * This command may NOT be repeated
		 */
		public static void spike(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int y, x, dir, item = 0;

			//dir = args[0].direction;

			///* Get a spike */
			//if (!get_spike(&item))
			//{
			//    /* Message */
			//    msg("You have no spikes!");

			//    /* Done */
			//    return;
			//}

			///* Get location */
			//y = p_ptr.py + ddy[dir];
			//x = p_ptr.px + ddx[dir];


			///* Verify legality */
			//if (!do_cmd_spike_test(y, x)) return;


			///* Take a turn */
			//p_ptr.energy_use = 100;

			///* Apply confusion */
			//if (player_confuse_dir(p_ptr, &dir, false))
			//{
			//    /* Get location */
			//    y = p_ptr.py + ddy[dir];
			//    x = p_ptr.px + ddx[dir];
			//}


			///* Monster */
			//if (cave.m_idx[y][x] > 0)
			//{
			//    /* Message */
			//    msg("There is a monster in the way!");

			//    /* Attack */
			//    py_attack(y, x);
			//}

			///* Go for it */
			//else
			//{
			//    /* Verify legality */
			//    if (!do_cmd_spike_test(y, x)) return;

			//    /* Successful jamming */
			//    msg("You jam the door with a spike.");

			//    /* Convert "locked" to "stuck" XXX XXX XXX */
			//    if (cave.feat[y][x] < FEAT_DOOR_HEAD + 0x08)
			//        cave.feat[y][x] += 0x08;

			//    /* Add one spike to the door */
			//    if (cave.feat[y][x] < FEAT_DOOR_TAIL)
			//        cave.feat[y][x] += 0x01;

			//    /* Use up, and describe, a single spike, from the bottom */
			//    inven_item_increase(item, -1);
			//    inven_item_describe(item);
			//    inven_item_optimize(item);
			//}
		}


		/*
		 * Determine if a given grid may be "walked"
		 */
		static bool walk_test(int y, int x) {
			int m_idx = Cave.cave.m_idx[y][x];

			/* Allow attack on visible monsters if unafraid */
			if ((m_idx > 0) && (Cave.cave_monster(Cave.cave, m_idx).ml) && !Monster.Monster.is_mimicking(m_idx))
			{
			    /* Handle player fear */
			    if(Misc.p_ptr.check_state(Object_Flag.AFRAID, Misc.p_ptr.state.flags))
			    {
					throw new NotImplementedException();
					///* Extract monster name (or "it") */
					//char m_name[80];
					//const monster_type *m_ptr;

					//m_ptr = cave_monster(cave, m_idx);
					//monster_desc(m_name, sizeof(m_name), m_ptr, 0);

					///* Message */
					//msgt(MSG_AFRAID,
					//    "You are too afraid to attack %s!", m_name);

					///* Nope */
					//return (false);
			    }

			    return (true);
			}

			/* If we don't know the grid, allow attempts to walk into it */
			if ((Cave.cave.info[y][x] & Cave.CAVE_MARK) == 0)
			    return true;

			/* Require open space */
			if (!Cave.cave_floor_bold(y, x))
			{
			    /* Rubble */
			    if (Cave.cave.feat[y][x] == Cave.FEAT_RUBBLE)
			        Utilities.msgt(Message_Type.MSG_HITWALL, "There is a pile of rubble in the way!");

			    /* Door */
			    else if (Cave.cave.feat[y][x] < Cave.FEAT_SECRET)
			        return true;

			    /* Wall */
			    else
			        Utilities.msgt(Message_Type.MSG_HITWALL, "There is a wall in the way!");

			    /* Cancel repeat */
			    Cave.disturb(Misc.p_ptr, 0, 0);

			    /* Nope */
			    return (false);
			}

			/* Okay */
			return (true);
		}


		/*
		 * Walk in the given direction.
		 */
		public static void walk(Command_Code code, cmd_arg[] args) {
			int x, y;
			int dir = args[0].value;

			/* Apply confusion if necessary */
			Misc.p_ptr.confuse_dir(ref dir, false);

			/* Confused movements use energy no matter what */
			if (dir != args[0].value)	
			    Misc.p_ptr.energy_use = 100;

			/* Verify walkability */
			y = Misc.p_ptr.py + Misc.ddy[dir];
			x = Misc.p_ptr.px + Misc.ddx[dir];
			if (!Do_Command.walk_test(y, x))
			    return;

			Misc.p_ptr.energy_use = 100;

			Command.move_player(dir, true);
		}


		/*
		 * Walk into a trap.
		 */
		public static void jump(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int x, y;
			//int dir = args[0].direction;

			///* Apply confusion if necessary */
			//player_confuse_dir(p_ptr, &dir, false);

			///* Verify walkability */
			//y = p_ptr.py + ddy[dir];
			//x = p_ptr.px + ddx[dir];
			//if (!do_cmd_walk_test(y, x))
			//    return;

			//p_ptr.energy_use = 100;

			//move_player(dir, false);
		}


		/*
		 * Start running.
		 *
		 * Note that running while confused is not allowed.
		 */
		public static void run(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int x, y;
			//int dir = args[0].direction;

			//if (player_confuse_dir(p_ptr, &dir, true))
			//{
			//    return;
			//}

			///* Get location */
			//y = p_ptr.py + ddy[dir];
			//x = p_ptr.px + ddx[dir];
			//if (!do_cmd_walk_test(y, x))
			//    return;

			///* Start run */
			//run_step(dir);
		}


		/*
		 * Start running with pathfinder.
		 *
		 * Note that running while confused is not allowed.
		 */
		public static void pathfind(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			///* Hack XXX XXX XXX */
			//int dir = 5;
			//if (player_confuse_dir(p_ptr, &dir, true))
			//{
			//    return;
			//}

			//if (findpath(args[0].point.x, args[0].point.y))
			//{
			//    p_ptr.running = 1000;
			//    /* Calculate torch radius */
			//    p_ptr.update |= (PU_TORCH);
			//    p_ptr.running_withpathfind = true;
			//    run_step(0);
			//}
		}



		/*
		 * Stay still.  Search.  Enter stores.
		 * Pick up treasure if "pickup" is true.
		 */
		public static void hold(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			///* Take a turn */
			//p_ptr.energy_use = 100;

			///* Spontaneous Searching */
			//if ((p_ptr.state.skills[SKILL_SEARCH_FREQUENCY] >= 50) ||
			//    one_in_(50 - p_ptr.state.skills[SKILL_SEARCH_FREQUENCY]))
			//{
			//    search(false);
			//}

			///* Continuous Searching */
			//if (p_ptr.searching)
			//{
			//    search(false);
			//}

			///* Pick things up, not using extra energy */
			//do_autopickup();

			///* Hack -- enter a store if we are on one */
			//if ((cave.feat[p_ptr.py][p_ptr.px] >= FEAT_SHOP_HEAD) &&
			//    (cave.feat[p_ptr.py][p_ptr.px] <= FEAT_SHOP_TAIL))
			//{
			//    /* Disturb */
			//    disturb(p_ptr, 0, 0);

			//    cmd_insert(CMD_ENTER_STORE);

			//    /* Free turn XXX XXX XXX */
			//    p_ptr.energy_use = 0;
			//}
			//else
			//{
			//    event_signal(EVENT_SEEFLOOR);
			//}
		}



		/*
		 * Pick up objects on the floor beneath you.  -LM-
		 */
		public static void pickup(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int energy_cost;

			///* Pick up floor objects, forcing a menu for multiple objects. */
			//energy_cost = py_pickup(1) * 10;

			///* Charge this amount of energy. */
			//p_ptr.energy_use = energy_cost;
		}

		/*
		 * Pick up objects on the floor beneath you.  -LM-
		 */
		public static void autopickup(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//p_ptr.energy_use = do_autopickup() * 10;
		}


		/*
		 * Rest (restores hit points and mana and such)
		 */
		public static void rest(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			///* 
			// * A little sanity checking on the input - only the specified negative 
			// * values are valid. 
			// */
			//if ((args[0].choice < 0) &&
			//    ((args[0].choice != REST_COMPLETE) &&
			//     (args[0].choice != REST_ALL_POINTS) &&
			//     (args[0].choice != REST_SOME_POINTS))) 
			//{
			//    return;
			//}

			///* Save the rest code */
			//p_ptr.resting = args[0].choice;

			///* Truncate overlarge values */
			//if (p_ptr.resting > 9999) p_ptr.resting = 9999;

			///* Take a turn XXX XXX XXX (?) */
			//p_ptr.energy_use = 100;

			///* Cancel searching */
			//p_ptr.searching = false;

			///* Recalculate bonuses */
			//p_ptr.update |= (PU_BONUS);

			///* Redraw the state */
			//p_ptr.redraw |= (PR_STATE);

			///* Handle stuff */
			//handle_stuff(p_ptr);

			///* Refresh XXX XXX XXX */
			//Term_fresh();
		}


		/*
		 * Hack -- commit suicide
		 */
		public static void suicide(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			///* Commit suicide */
			//p_ptr.is_dead = true;

			///* Stop playing */
			//p_ptr.playing = false;

			///* Leaving */
			//p_ptr.leaving = true;

			///* Cause of death */
			//my_strcpy(p_ptr.died_from, "Quitting", sizeof(p_ptr.died_from));
		}

		public static void save_game(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//save_game();
		}

		/**
		 * Throw an object from the quiver, pack or floor.
		 */
		//had to rename to upper case, because throw is keyword...
		//in fact, all of these should be upper case!
		public static void Throw(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException(); //lol, close enough for now
			//int item = args[0].item;
			//int dir = args[1].direction;
			//int shots = 1;

			//object_type *o_ptr = object_from_item_idx(item);
			//int weight = MAX(o_ptr.weight, 10);
			//int str = adj_str_blow[p_ptr.state.stat_ind[A_STR]];
			//int range = MIN(((str + 20) * 10) / weight, 10);

			//ranged_attack attack = make_ranged_throw;

			///* Make sure the player isn't throwing wielded items */
			//if (item >= INVEN_WIELD && item < QUIVER_START) {
			//    msg("You have cannot throw wielded items.");
			//    return;
			//}

			///* Check the item being thrown is usable by the player. */
			//if (!item_is_available(item, null, (USE_EQUIP | USE_INVEN | USE_FLOOR))) {
			//    msg("That item is not within your reach.");
			//    return;
			//}

			//ranged_helper(item, dir, range, shots, attack);
		}

		/* Wield or wear an item */
		public static void wield(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//object_type *equip_o_ptr;
			//char o_name[80];

			//unsigned n;

			//int item = args[0].item;
			//int slot = args[1].number;
			//object_type *o_ptr = object_from_item_idx(item);

			//if (!item_is_available(item, null, USE_INVEN | USE_FLOOR))
			//{
			//    msg("You do not have that item to wield.");
			//    return;
			//}

			///* Check the slot */
			//if (!slot_can_wield_item(slot, o_ptr))
			//{
			//    msg("You cannot wield that item there.");
			//    return;
			//}

			//equip_o_ptr = &p_ptr.inventory[slot];

			///* If the slot is open, wield and be done */
			//if (!equip_o_ptr.kind)
			//{
			//    wield_item(o_ptr, item, slot);
			//    return;
			//}

			///* If the slot is in the quiver and objects can be combined */
			//if (obj_is_ammo(equip_o_ptr) && object_similar(equip_o_ptr, o_ptr,
			//    OSTACK_QUIVER))
			//{
			//    wield_item(o_ptr, item, slot);
			//    return;
			//}

			///* Prevent wielding into a cursed slot */
			//if (cursed_p(equip_o_ptr.flags))
			//{
			//    object_desc(o_name, sizeof(o_name), equip_o_ptr, ODESC_BASE);
			//    msg("The %s you are %s appears to be cursed.", o_name,
			//               describe_use(slot));
			//    return;
			//}

			///* "!t" checks for taking off */
			//n = check_for_inscrip(equip_o_ptr, "!t");
			//while (n--)
			//{
			//    /* Prompt */
			//    object_desc(o_name, sizeof(o_name), equip_o_ptr,
			//                ODESC_PREFIX | ODESC_FULL);

			//    /* Forget it */
			//    if (!get_check(format("Really take off %s? ", o_name))) return;
			//}

			//wield_item(o_ptr, item, slot);
		}

		/* Take off an item */
		public static void takeoff(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int item = args[0].item;

			//if (!item_is_available(item, null, USE_EQUIP))
			//{
			//    msg("You are not wielding that item.");
			//    return;
			//}

			//if (!obj_can_takeoff(object_from_item_idx(item)))
			//{
			//    msg("You cannot take off that item.");
			//    return;
			//}

			//(void)inven_takeoff(item, 255);
			//pack_overflow();
			//p_ptr.energy_use = 50;
		}

		/* Drop an item */
		public static void drop(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int item = args[0].item;
			//object_type* o_ptr = object_from_item_idx(item);
			//int amt = args[1].number;

			//if(!item_is_available(item, null, USE_INVEN | USE_EQUIP)) {
			//    msg("You do not have that item to drop it.");
			//    return;
			//}

			///* Hack -- Cannot remove cursed items */
			//if((item >= INVEN_WIELD) && cursed_p(o_ptr.flags)) {
			//    msg("Hmmm, it seems to be cursed.");
			//    return;
			//}

			//inven_drop(item, amt);
			//p_ptr.energy_use = 50;
		}

		/* Destroy an item */
		public static void destroy(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//object_type *o_ptr;
			//int item = args[0].item;

			//if (!item_is_available(item, null, USE_INVEN | USE_EQUIP | USE_FLOOR))
			//{
			//    msg("You do not have that item to ignore it.");
			//    return;
			//}

			//o_ptr = object_from_item_idx(item);

			//if ((item >= INVEN_WIELD) && cursed_p(o_ptr.flags)) {
			//    msg("You cannot ignore cursed items.");
			//} else {	
			//    char o_name[80];

			//    object_desc(o_name, sizeof o_name, o_ptr, ODESC_PREFIX | ODESC_FULL);
			//    msgt(MSG_DESTROY, "Ignoring %s.", o_name);

			//    o_ptr.ignore = true;
			//    p_ptr.notice |= PN_SQUELCH;
			//}
		}

		/*** Inscriptions ***/

		/* Remove inscription */
		public static void uninscribe(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//object_type* o_ptr = object_from_item_idx(args[0].item);

			//if(obj_has_inscrip(o_ptr))
			//    msg("Inscription removed.");

			//o_ptr.note = 0;

			//p_ptr.notice |= (PN_COMBINE | PN_SQUELCH | PN_SORT_QUIVER);
			//p_ptr.redraw |= (PR_INVEN | PR_EQUIP);
		}

		/* Add inscription */
		public static void inscribe(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//object_type *o_ptr = object_from_item_idx(args[0].item);

			//o_ptr.note = quark_add(args[1].string);

			//p_ptr.notice |= (PN_COMBINE | PN_SQUELCH | PN_SORT_QUIVER);
			//p_ptr.redraw |= (PR_INVEN | PR_EQUIP);
		}

		/*
		 * Use an object the right way.
		 *
		 * There may be a BIG problem with any "effect" that can cause "changes"
		 * to the inventory.  For example, a "scroll of recharging" can cause
		 * a wand/staff to "disappear", moving the inventory up.  Luckily, the
		 * scrolls all appear BEFORE the staffs/wands, so this is not a problem.
		 * But, for example, a "staff of recharging" could cause MAJOR problems.
		 * In such a case, it will be best to either (1) "postpone" the effect
		 * until the end of the function, or (2) "change" the effect, say, into
		 * giving a staff "negative" charges, or "turning a staff into a stick".
		 * It seems as though a "rod of recharging" might in fact cause problems.
		 * The basic problem is that the act of recharging (and destroying) an
		 * item causes the inducer of that action to "move", causing "o_ptr" to
		 * no longer point at the correct item, with horrifying results.
		 */
		public static void use(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int item = args[0].item;
			//object_type* o_ptr = object_from_item_idx(item);
			//int effect;
			//bool ident = false, used = false;
			//bool was_aware = object_flavor_is_aware(o_ptr);
			//int dir = 5;
			//int px = p_ptr.px, py = p_ptr.py;
			//int snd, boost, level;
			//use_type use;
			//int items_allowed = 0;

			///* Determine how this item is used. */
			//if(obj_is_rod(o_ptr)) {
			//    if(!obj_can_zap(o_ptr)) {
			//        msg("That rod is still charging.");
			//        return;
			//    }

			//    use = USE_TIMEOUT;
			//    snd = MSG_ZAP_ROD;
			//    items_allowed = USE_INVEN | USE_FLOOR;
			//} else if(obj_is_wand(o_ptr)) {
			//    if(!obj_has_charges(o_ptr)) {
			//        msg("That wand has no charges.");
			//        return;
			//    }

			//    use = USE_CHARGE;
			//    snd = MSG_ZAP_ROD;
			//    items_allowed = USE_INVEN | USE_FLOOR;
			//} else if(obj_is_staff(o_ptr)) {
			//    if(!obj_has_charges(o_ptr)) {
			//        msg("That staff has no charges.");
			//        return;
			//    }

			//    use = USE_CHARGE;
			//    snd = MSG_USE_STAFF;
			//    items_allowed = USE_INVEN | USE_FLOOR;
			//} else if(obj_is_food(o_ptr)) {
			//    use = USE_SINGLE;
			//    snd = MSG_EAT;
			//    items_allowed = USE_INVEN | USE_FLOOR;
			//} else if(obj_is_potion(o_ptr)) {
			//    use = USE_SINGLE;
			//    snd = MSG_QUAFF;
			//    items_allowed = USE_INVEN | USE_FLOOR;
			//} else if(obj_is_scroll(o_ptr)) {
			//    /* Check player can use scroll */
			//    if(!player_can_read())
			//        return;

			//    use = USE_SINGLE;
			//    snd = MSG_GENERIC;
			//    items_allowed = USE_INVEN | USE_FLOOR;
			//} else if(obj_is_activatable(o_ptr)) {
			//    if(!obj_can_activate(o_ptr)) {
			//        msg("That item is still charging.");
			//        return;
			//    }

			//    use = USE_TIMEOUT;
			//    snd = MSG_ACT_ARTIFACT;
			//    items_allowed = USE_EQUIP;
			//} else {
			//    msg("The item cannot be used at the moment");
			//}

			///* Check if item is within player's reach. */
			//if(items_allowed == 0 || !item_is_available(item, null, items_allowed)) {
			//    msg("You cannot use that item from its current location.");
			//    return;
			//}

			///* track the object used */
			//track_object(item);

			///* Figure out effect to use */
			//effect = object_effect(o_ptr);

			///* If the item requires a direction, get one (allow cancelling) */
			//if(obj_needs_aim(o_ptr))
			//    dir = args[1].direction;

			///* Check for use if necessary, and execute the effect */
			//if((use != USE_CHARGE && use != USE_TIMEOUT) || check_devices(o_ptr)) {
			//    int beam = beam_chance(o_ptr.tval);

			//    /* Special message for artifacts */
			//    if(o_ptr.artifact) {
			//        msgt(snd, "You activate it.");
			//        if(o_ptr.artifact.effect_msg)
			//            activation_message(o_ptr, o_ptr.artifact.effect_msg);
			//        level = o_ptr.artifact.level;
			//    } else {
			//        /* Make a noise! */
			//        sound(snd);
			//        level = o_ptr.kind.level;
			//    }

			//    /* A bit of a hack to make ID work better.
			//        -- Check for "obvious" effects beforehand. */
			//    if(effect_obvious(effect))
			//        object_flavor_aware(o_ptr);

			//    /* Boost damage effects if skill > difficulty */
			//    boost = MAX(p_ptr.state.skills[SKILL_DEVICE] - level, 0);

			//    /* Do effect */
			//    used = effect_do(effect, &ident, was_aware, dir, beam, boost);

			//    /* Quit if the item wasn't used and no knowledge was gained */
			//    if(!used && (was_aware || !ident))
			//        return;
			//}

			///* If the item is a null pointer or has been wiped, be done now */
			//if(!o_ptr || !o_ptr.kind)
			//    return;

			//if(ident)
			//    object_notice_effect(o_ptr);

			///* Food feeds the player */
			//if(o_ptr.tval == TV_FOOD || o_ptr.tval == TV_POTION)
			//    player_set_food(p_ptr, p_ptr.food + o_ptr.pval[DEFAULT_PVAL]);

			///* Use the turn */
			//p_ptr.energy_use = 100;

			///* Mark as tried and redisplay */
			//p_ptr.notice |= (PN_COMBINE | PN_REORDER);
			//p_ptr.redraw |= (PR_INVEN | PR_EQUIP | PR_OBJECT);

			///*
			// * If the player becomes aware of the item's function, then mark it as
			// * aware and reward the player with some experience.  Otherwise, mark
			// * it as "tried".
			// */
			//if(ident && !was_aware) {
			//    /* Object level */
			//    int lev = o_ptr.kind.level;

			//    object_flavor_aware(o_ptr);
			//    if(o_ptr.tval == TV_ROD)
			//        object_notice_everything(o_ptr);
			//    player_exp_gain(p_ptr, (lev + (p_ptr.lev / 2)) / p_ptr.lev);
			//    p_ptr.notice |= PN_SQUELCH;
			//} else if(used) {
			//    object_flavor_tried(o_ptr);
			//}

			///* If there are no more of the item left, then we're done. */
			//if(!o_ptr.number)
			//    return;

			///* Chargeables act differently to single-used items when not used up */
			//if(used && use == USE_CHARGE) {
			//    /* Use a single charge */
			//    o_ptr.pval[DEFAULT_PVAL]--;

			//    /* Describe charges */
			//    if(item >= 0)
			//        inven_item_charges(item);
			//    else
			//        floor_item_charges(0 - item);
			//} else if(used && use == USE_TIMEOUT) {
			//    /* Artifacts use their own special field */
			//    if(o_ptr.artifact)
			//        o_ptr.timeout = randcalc(o_ptr.artifact.time, 0, RANDOMISE);
			//    else
			//        o_ptr.timeout += randcalc(o_ptr.kind.time, 0, RANDOMISE);
			//} else if(used && use == USE_SINGLE) {
			//    /* Destroy a potion in the pack */
			//    if(item >= 0) {
			//        inven_item_increase(item, -1);
			//        inven_item_describe(item);
			//        inven_item_optimize(item);
			//    }

			//    /* Destroy a potion on the floor */
			//    else {
			//        floor_item_increase(0 - item, -1);
			//        floor_item_describe(0 - item);
			//        floor_item_optimize(0 - item);
			//    }
			//}

			///* Hack to make Glyph of Warding work properly */
			//if(cave.feat[py][px] == FEAT_GLYPH) {
			//    /* Push objects off the grid */
			//    if(cave.o_idx[py][px])
			//        push_object(py, px);
			//}


		}


		/*** Refuelling ***/
		static void refill_lamp(Object.Object j_ptr, Object.Object o_ptr, int item) {
			throw new NotImplementedException();
			///* Refuel */
			//j_ptr.timeout += o_ptr.timeout ? o_ptr.timeout : o_ptr.pval[DEFAULT_PVAL];

			///* Message */
			//msg("You fuel your lamp.");

			///* Comment */
			//if(j_ptr.timeout >= FUEL_LAMP) {
			//    j_ptr.timeout = FUEL_LAMP;
			//    msg("Your lamp is full.");
			//}

			///* Refilled from a lantern */
			//if(o_ptr.sval == SV_LIGHT_LANTERN) {
			//    /* Unstack if necessary */
			//    if(o_ptr.number > 1) {
			//        object_type* i_ptr;
			//        object_type object_type_body;

			//        /* Get local object */
			//        i_ptr = &object_type_body;

			//        /* Obtain a local object */
			//        object_copy(i_ptr, o_ptr);

			//        /* Modify quantity */
			//        i_ptr.number = 1;

			//        /* Remove fuel */
			//        i_ptr.timeout = 0;

			//        /* Unstack the used item */
			//        o_ptr.number--;
			//        p_ptr.total_weight -= i_ptr.weight;

			//        /* Carry or drop */
			//        if(item >= 0)
			//            item = inven_carry(p_ptr, i_ptr);
			//        else
			//            drop_near(cave, i_ptr, 0, p_ptr.py, p_ptr.px, false);
			//    }

			//    /* Empty a single lantern */
			//    else {
			//        /* No more fuel */
			//        o_ptr.timeout = 0;
			//    }

			//    /* Combine / Reorder the pack (later) */
			//    p_ptr.notice |= (PN_COMBINE | PN_REORDER);

			//    /* Redraw stuff */
			//    p_ptr.redraw |= (PR_INVEN);
			//}

			///* Refilled from a flask */
			//else {
			//    /* Decrease the item (from the pack) */
			//    if(item >= 0) {
			//        inven_item_increase(item, -1);
			//        inven_item_describe(item);
			//        inven_item_optimize(item);
			//    }

			//    /* Decrease the item (from the floor) */
			//    else {
			//        floor_item_increase(0 - item, -1);
			//        floor_item_describe(0 - item);
			//        floor_item_optimize(0 - item);
			//    }
			//}

			///* Recalculate torch */
			//p_ptr.update |= (PU_TORCH);

			///* Redraw stuff */
			//p_ptr.redraw |= (PR_EQUIP);
		}


		static void refuel_torch(Object.Object j_ptr, Object.Object o_ptr, int item) {
			throw new NotImplementedException();
			//bitflag f[OF_SIZE];
			//bitflag g[OF_SIZE];

			///* Refuel */
			//j_ptr.timeout += o_ptr.timeout + 5;

			///* Message */
			//msg("You combine the torches.");

			///* Transfer the LIGHT flag if refuelling from a torch with it to one
			//   without it */
			//object_flags(o_ptr, f);
			//object_flags(j_ptr, g);
			//if (of_has(f, OF_LIGHT) && !of_has(g, OF_LIGHT))
			//{
			//    of_on(j_ptr.flags, OF_LIGHT);
			//    if (!j_ptr.ego && o_ptr.ego)
			//        j_ptr.ego = o_ptr.ego;
			//    msg("Your torch shines further!");
			//}

			///* Over-fuel message */
			//if (j_ptr.timeout >= FUEL_TORCH)
			//{
			//    j_ptr.timeout = FUEL_TORCH;
			//    msg("Your torch is fully fueled.");
			//}

			///* Refuel message */
			//else
			//{
			//    msg("Your torch glows more brightly.");
			//}

			///* Decrease the item (from the pack) */
			//if (item >= 0)
			//{
			//    inven_item_increase(item, -1);
			//    inven_item_describe(item);
			//    inven_item_optimize(item);
			//}

			///* Decrease the item (from the floor) */
			//else
			//{
			//    floor_item_increase(0 - item, -1);
			//    floor_item_describe(0 - item);
			//    floor_item_optimize(0 - item);
			//}

			///* Recalculate torch */
			//p_ptr.update |= (PU_TORCH);

			///* Redraw stuff */
			//p_ptr.redraw |= (PR_EQUIP);
		}


		public static void refill(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//object_type *j_ptr = &p_ptr.inventory[INVEN_LIGHT];
			//bitflag f[OF_SIZE];

			//int item = args[0].item;
			//object_type *o_ptr = object_from_item_idx(item);

			//if (!item_is_available(item, null, USE_INVEN | USE_FLOOR))
			//{
			//    msg("You do not have that item to refill with it.");
			//    return;
			//}

			///* Check what we're wielding. */
			//object_flags(j_ptr, f);

			//if (j_ptr.tval != TV_LIGHT)
			//{
			//    msg("You are not wielding a light.");
			//    return;
			//}

			//else if (of_has(f, OF_NO_FUEL))
			//{
			//    msg("Your light cannot be refilled.");
			//    return;
			//}

			///* It's a lamp */
			//if (j_ptr.sval == SV_LIGHT_LANTERN)
			//    refill_lamp(j_ptr, o_ptr, item);

			///* It's a torch */
			//else if (j_ptr.sval == SV_LIGHT_TORCH)
			//    refuel_torch(j_ptr, o_ptr, item);

			//p_ptr.energy_use = 50;
		}


		/*
		 * Buy the item with the given index from the current store's inventory.
		 */
		public static void buy(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int item = args[0].item;
			//int amt = args[1].number;

			//object_type *o_ptr;	
			//object_type object_type_body;
			//object_type *i_ptr = &object_type_body;

			//char o_name[80];
			//int price, item_new;

			//struct store *store = current_store();

			//if (!store) {
			//    msg("You cannot purchase items when not in a store.");
			//    return;
			//}

			///* Get the actual object */
			//o_ptr = &store.stock[item];

			///* Get desired object */
			//object_copy_amt(i_ptr, o_ptr, amt);

			///* Ensure we have room */
			//if (!inven_carry_okay(i_ptr))
			//{
			//    msg("You cannot carry that many items.");
			//    return;
			//}

			///* Describe the object (fully) */
			//object_desc(o_name, sizeof(o_name), i_ptr, ODESC_PREFIX | ODESC_FULL);

			///* Extract the price for the entire stack */
			//price = price_item(i_ptr, false, i_ptr.number);

			//if (price > p_ptr.au)
			//{
			//    msg("You cannot afford that purchase.");
			//    return;
			//}

			///* Spend the money */
			//p_ptr.au -= price;

			///* Update the display */
			//store_flags |= STORE_GOLD_CHANGE;

			///* ID objects on buy */
			//object_notice_everything(i_ptr);

			///* Combine / Reorder the pack (later) */
			//p_ptr.notice |= (PN_COMBINE | PN_REORDER | PN_SORT_QUIVER | PN_SQUELCH);

			///* The object no longer belongs to the store */
			//i_ptr.ident &= ~(IDENT_STORE);

			///* Message */
			//if (one_in_(3)) msgt(MSG_STORE5, "%s", ONE_OF(comment_accept));
			//msg("You bought %s for %ld gold.", o_name, (long)price);

			///* Erase the inscription */
			//i_ptr.note = 0;

			///* Give it to the player */
			//item_new = inven_carry(p_ptr, i_ptr);

			///* Message */
			//object_desc(o_name, sizeof(o_name), &p_ptr.inventory[item_new],
			//            ODESC_PREFIX | ODESC_FULL);
			//msg("You have %s (%c).", o_name, index_to_label(item_new));

			///* Hack - Reduce the number of charges in the original stack */
			//if (o_ptr.tval == TV_WAND || o_ptr.tval == TV_STAFF)
			//{
			//    o_ptr.pval[DEFAULT_PVAL] -= i_ptr.pval[DEFAULT_PVAL];
			//}

			///* Handle stuff */
			//handle_stuff(p_ptr);

			///* Remove the bought objects from the store */
			//store_item_increase(store, item, -amt);
			//store_item_optimize(store, item);

			///* Store is empty */
			//if (store.stock_num == 0)
			//{
			//    int i;

			//    /* Shuffle */
			//    if (one_in_(STORE_SHUFFLE))
			//    {
			//        /* Message */
			//        msg("The shopkeeper retires.");

			//        /* Shuffle the store */
			//        store_shuffle(store);
			//        store_flags |= STORE_FRAME_CHANGE;
			//    }

			//    /* Maintain */
			//    else
			//    {
			//        /* Message */
			//        msg("The shopkeeper brings out some new stock.");
			//    }

			//    /* New inventory */
			//    for (i = 0; i < 10; ++i)
			//        store_maint(store);
			//}

			//event_signal(EVENT_INVENTORY);
			//event_signal(EVENT_EQUIPMENT);
		}

		/*
		 * Retrieve the item with the given index from the home's inventory.
		 */
		public static void retrieve(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int item = args[0].item;
			//int amt = args[1].number;

			//object_type *o_ptr;	
			//object_type picked_item;
			//char o_name[80];
			//int item_new;

			//struct store *store = current_store();

			//if (store.sidx != STORE_HOME) {
			//    msg("You are not currently at home.");
			//    return;
			//}

			///* Get the actual object */
			//o_ptr = &store.stock[item];

			///* Get desired object */
			//object_copy_amt(&picked_item, o_ptr, amt);

			///* Ensure we have room */
			//if (!inven_carry_okay(&picked_item))
			//{
			//    msg("You cannot carry that many items.");
			//    return;
			//}

			///* Distribute charges of wands, staves, or rods */
			//distribute_charges(o_ptr, &picked_item, amt);

			///* Give it to the player */
			//item_new = inven_carry(p_ptr, &picked_item);

			///* Describe just the result */
			//object_desc(o_name, sizeof(o_name), &p_ptr.inventory[item_new],
			//            ODESC_PREFIX | ODESC_FULL);

			///* Message */
			//msg("You have %s (%c).", o_name, index_to_label(item_new));

			///* Handle stuff */
			//handle_stuff(p_ptr);

			///* Remove the items from the home */
			//store_item_increase(store, item, -amt);
			//store_item_optimize(store, item);

			//event_signal(EVENT_INVENTORY);
			//event_signal(EVENT_EQUIPMENT);
		}

		/*
		 * Sell an item to the current store.
		 */
		public static void sell(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//    int item = args[0].item;
			//    int amt = args[1].number;
			//    object_type sold_item;
			//    struct store *store = current_store();
			//    int price, dummy, value;
			//    char o_name[120];

			//    /* Get the item */
			//    object_type *o_ptr = object_from_item_idx(item);

			//    /* Cannot remove cursed objects */
			//    if ((item >= INVEN_WIELD) && cursed_p(o_ptr.flags)) {
			//        msg("Hmmm, it seems to be cursed.");
			//        return;
			//    }

			//    /* Check we are somewhere we can sell the items. */
			//    if (!store) {
			//        msg("You cannot sell items when not in a store.");
			//        return;
			//    }

			//    /* Check the store wants the items being sold */
			//    if (!store_will_buy(store, o_ptr)) {
			//        msg("I do not wish to purchase this item.");
			//        return;
			//    }

			//    /* Get a copy of the object representing the number being sold */
			//    object_copy_amt(&sold_item, o_ptr, amt);

			//    /* Check if the store has space for the items */
			//    if (!store_check_num(store, &sold_item))
			//    {
			//        msg("I have not the room in my store to keep it.");
			//        return;
			//    }

			//    price = price_item(&sold_item, true, amt);

			//    /* Get some money */
			//    p_ptr.au += price;

			//    /* Update the display */
			//    store_flags |= STORE_GOLD_CHANGE;

			//    /* Update the auto-history if selling an artifact that was previously un-IDed. (Ouch!) */
			//    if (o_ptr.artifact)
			//        history_add_artifact(o_ptr.artifact, true, true);

			//    /* Combine / Reorder the pack (later) */
			//    p_ptr.notice |= (PN_COMBINE | PN_REORDER | PN_SORT_QUIVER);

			//    /* Redraw stuff */
			//    p_ptr.redraw |= (PR_INVEN | PR_EQUIP);

			//    /* Get the "apparent" value */
			//    dummy = object_value(&sold_item, amt, false);
			///*	msg("Dummy is %d", dummy); */

			//    /* Identify original object */
			//    object_notice_everything(o_ptr);

			//    /* Take a new copy of the now known-about object. */
			//    object_copy_amt(&sold_item, o_ptr, amt);

			//    /* The item belongs to the store now */
			//    sold_item.ident |= IDENT_STORE;

			//    /*
			//    * Hack -- Allocate charges between those wands, staves, or rods
			//    * sold and retained, unless all are being sold.
			//     */
			//    distribute_charges(o_ptr, &sold_item, amt);

			//    /* Get the "actual" value */
			//    value = object_value(&sold_item, amt, false);
			///*	msg("Value is %d", value); */

			//    /* Get the description all over again */
			//    object_desc(o_name, sizeof(o_name), &sold_item, ODESC_PREFIX | ODESC_FULL);

			//    /* Describe the result (in message buffer) */
			//    msg("You sold %s (%c) for %ld gold.",
			//        o_name, index_to_label(item), (long)price);

			//    /* Analyze the prices (and comment verbally) */
			//    purchase_analyze(price, value, dummy);

			//    /* Set squelch flag */
			//    p_ptr.notice |= PN_SQUELCH;

			//    /* Take the object from the player */
			//    inven_item_increase(item, -amt);
			//    inven_item_optimize(item);

			//    /* Handle stuff */
			//    handle_stuff(p_ptr);

			//    /* The store gets that (known) object */
			//    store_carry(store, &sold_item);

			//    event_signal(EVENT_INVENTORY);
			//    event_signal(EVENT_EQUIPMENT);
		}

		/*
		 * Stash an item in the home.
		 */
		public static void stash(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int item = args[0].item;
			//int amt = args[1].number;
			//object_type dropped_item;
			//object_type *o_ptr = object_from_item_idx(item);
			//struct store *store = current_store();
			//char o_name[120];

			///* Check we are somewhere we can stash items. */
			//if (store.sidx != STORE_HOME)
			//{
			//    msg("You are not in your home.");
			//    return;
			//}

			///* Cannot remove cursed objects */
			//if ((item >= INVEN_WIELD) && cursed_p(o_ptr.flags))
			//{
			//    msg("Hmmm, it seems to be cursed.");
			//    return;
			//}	

			///* Get a copy of the object representing the number being sold */
			//object_copy_amt(&dropped_item, o_ptr, amt);

			//if (!store_check_num(store, &dropped_item))
			//{
			//    msg("Your home is full.");
			//    return;
			//}

			///* Distribute charges of wands/staves/rods */
			//distribute_charges(o_ptr, &dropped_item, amt);

			///* Describe */
			//object_desc(o_name, sizeof(o_name), &dropped_item, ODESC_PREFIX | ODESC_FULL);

			///* Message */
			//msg("You drop %s (%c).", o_name, index_to_label(item));

			///* Take it from the players inventory */
			//inven_item_increase(item, -amt);
			//inven_item_optimize(item);

			///* Handle stuff */
			//handle_stuff(p_ptr);

			///* Let the home carry it */
			//home_carry(&dropped_item);

			//event_signal(EVENT_INVENTORY);
			//event_signal(EVENT_EQUIPMENT);
		}


		/*
		 * Display contents of a store from knowledge menu
		 *
		 * The only allowed actions are 'I' to inspect an item
		 */
		public static void store_knowledge() {
			throw new NotImplementedException();
			//menu_type menu;

			//screen_save();
			//clear_from(0);

			///* Init the menu structure */
			//menu_init(&menu, MN_SKIN_SCROLL, &store_menu);
			//menu_layout(&menu, &store_menu_region);

			///* Calculate the positions of things and redraw */
			//store_menu_set_selections(&menu, true);
			//store_flags = STORE_INIT_CHANGE;
			//store_display_recalc(&menu);
			//store_menu_recalc(&menu);
			//store_redraw();

			//menu_select(&menu, 0, false);

			///* Flush messages XXX XXX XXX */
			//message_flush();

			//screen_load();
		}




		/*
		 * Enter a store, and interact with it.
		 */
		public static void store(Command_Code code, cmd_arg[] args) {
			/* Take note of the store number from the terrain feature */
			Store store = Store.current_store();
			Menu_Type menu;

			throw new NotImplementedException();
			///* Verify that there is a store */
			//if (!store) {
			//    msg("You see no store here.");
			//    return;
			//}

			///* Check if we can enter the store */
			//if (OPT(birth_no_stores)) {
			//    msg("The doors are locked.");
			//    return;
			//}

			///* Shut down the normal game view - it won't be updated - and start
			//   up the store state. */
			//event_signal(EVENT_LEAVE_GAME);
			//event_signal(EVENT_ENTER_STORE);

			///* Forget the view */
			//forget_view();

			///* Reset the command variables */
			//p_ptr.command_arg = 0;



			///*** Display ***/

			///* Save current screen (ie. dungeon) */
			//screen_save();


			///*** Inventory display ***/

			///* Wipe the menu and set it up */
			//menu_init(&menu, MN_SKIN_SCROLL, &store_menu);
			//menu_layout(&menu, &store_menu_region);

			//store_menu_set_selections(&menu, false);
			//store_flags = STORE_INIT_CHANGE;
			//store_display_recalc(&menu);
			//store_menu_recalc(&menu);
			//store_redraw();

			///* Say a friendly hello. */
			//if (store.sidx != STORE_HOME)
			//    prt_welcome(store.owner);

			//msg_flag = false;
			//menu_select(&menu, 0, false);
			//msg_flag = false;

			///* Switch back to the normal game view. */
			//event_signal(EVENT_LEAVE_STORE);
			//event_signal(EVENT_ENTER_GAME);

			///* Take a turn */
			//p_ptr.energy_use = 100;


			///* Flush messages XXX XXX XXX */
			//message_flush();


			///* Load the screen */
			//screen_load();


			///* Update the visuals */
			//p_ptr.update |= (PU_UPDATE_VIEW | PU_MONSTERS);

			///* Redraw entire screen */
			//p_ptr.redraw |= (PR_BASIC | PR_EXTRA);

			///* Redraw map */
			//p_ptr.redraw |= (PR_MAP);
		}


		/*** Spell casting ***/

		/* Gain a specific spell, specified by spell number (for mages). */
		public static void study_spell(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int spell = args[0].choice;

			//int item_list[INVEN_TOTAL + MAX_FLOOR_STACK];
			//int item_num;
			//int i;

			///* Check the player can study at all atm */
			//if (!player_can_study())
			//    return;

			///* Check that the player can actually learn the nominated spell. */
			//item_tester_hook = obj_can_browse;
			//item_num = scan_items(item_list, N_ELEMENTS(item_list), (USE_INVEN | USE_FLOOR));

			///* Check through all available books */
			//for (i = 0; i < item_num; i++)
			//{
			//    if (spell_in_book(spell, item_list[i]))
			//    {
			//        if (spell_okay_to_study(spell))
			//        {
			//            /* Spell is in an available book, and player is capable. */
			//            spell_learn(spell);
			//            p_ptr.energy_use = 100;
			//        }
			//        else
			//        {
			//            /* Spell is present, but player incapable. */
			//            msg("You cannot learn that spell.");
			//        }

			//        return;
			//    }
			//}
		}

		/* Cast a spell from a book */
		public static void cast(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int spell = args[0].choice;
			//int dir = args[1].direction;

			//int item_list[INVEN_TOTAL + MAX_FLOOR_STACK];
			//int item_num;
			//int i;

			//const char *verb = ((p_ptr.class.spell_book == TV_MAGIC_BOOK) ? "cast" : "recite");
			//const char *noun = ((p_ptr.class.spell_book == TV_MAGIC_BOOK) ? "spell" : "prayer");

			///* Check the player can cast spells at all */
			//if (!player_can_cast())
			//    return;

			///* Check spell is in a book they can access */
			//item_tester_hook = obj_can_browse;
			//item_num = scan_items(item_list, N_ELEMENTS(item_list), (USE_INVEN | USE_FLOOR));

			///* Check through all available books */
			//for (i = 0; i < item_num; i++)
			//{
			//    if (spell_in_book(spell, item_list[i]))
			//    {
			//        if (spell_okay_to_cast(spell))
			//        {
			//            /* Get the spell */
			//            const magic_type *s_ptr = &p_ptr.class.spells.info[spell];

			//            /* Verify "dangerous" spells */
			//            if (s_ptr.smana > p_ptr.csp)
			//            {
			//                /* Warning */
			//                msg("You do not have enough mana to %s this %s.", verb, noun);

			//                /* Flush input */
			//                flush();

			//                /* Verify */
			//                if (!get_check("Attempt it anyway? ")) return;
			//            }

			//            /* Cast a spell */
			//            if (spell_cast(spell, dir))
			//                p_ptr.energy_use = 100;
			//        }
			//        else
			//        {
			//            /* Spell is present, but player incapable. */
			//            msg("You cannot %s that %s.", verb, noun);
			//        }

			//        return;
			//    }
			//}

		}


		/* Gain a random spell from the given book (for priests) */
		public static void study_book(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int book = args[0].item;
			//object_type *o_ptr = object_from_item_idx(book);

			//int spell = -1;
			//struct spell *sp;
			//int k = 0;

			//const char *p = ((p_ptr.class.spell_book == TV_MAGIC_BOOK) ? "spell" : "prayer");

			///* Check the player can study at all atm */
			//if (!player_can_study())
			//    return;

			///* Check that the player has access to the nominated spell book. */
			//if (!item_is_available(book, obj_can_browse, (USE_INVEN | USE_FLOOR)))
			//{
			//    msg("That item is not within your reach.");
			//    return;
			//}

			///* Extract spells */
			//for (sp = o_ptr.kind.spells; sp; sp = sp.next) {
			//    if (!spell_okay_to_study(sp.spell_index))
			//        continue;
			//    if ((++k > 1) && (randint0(k) != 0))
			//        continue;
			//    spell = sp.spell_index;
			//}

			//if (spell < 0)
			//{
			//    msg("You cannot learn any %ss in that book.", p);
			//}
			//else
			//{
			//    spell_learn(spell);
			//    p_ptr.energy_use = 100;	
			//}
		}

		/**
		 * Fire an object from the quiver, pack or floor at a target.
		 */
		public static void fire(Command_Code code, cmd_arg[] args) {
			throw new NotImplementedException();
			//int item = args[0].item;
			//int dir = args[1].direction;
			//int range = 6 + 2 * p_ptr.state.ammo_mult;
			//int shots = p_ptr.state.num_shots;

			//ranged_attack attack = make_ranged_shot;

			//object_type *j_ptr = &p_ptr.inventory[INVEN_BOW];
			//object_type *o_ptr = object_from_item_idx(item);

			///* Require a usable launcher */
			//if (!j_ptr.tval || !p_ptr.state.ammo_tval) {
			//    msg("You have nothing to fire with.");
			//    return;
			//}

			///* Check the item being fired is usable by the player. */
			//if (!item_is_available(item, null, USE_EQUIP | USE_INVEN | USE_FLOOR)) {
			//    msg("That item is not within your reach.");
			//    return;
			//}

			///* Check the ammo can be used with the launcher */
			//if (o_ptr.tval != p_ptr.state.ammo_tval) {
			//    msg("That ammo cannot be fired by your current weapon.");
			//    return;
			//}

			//ranged_helper(item, dir, range, shots, attack);
		}
	}
}
