#ifndef INCLUDED_GAME_CMD_H
#define INCLUDED_GAME_CMD_H



/**
 * Returns the top command on the queue.
 */
game_command *cmd_get_top(void);

/* Inserts a command in the queue to be carried out. */
errr cmd_insert_s(game_command *cmd);

/* 
 * Convenience functions.
 * Insert a command with params in the queue to be carried out.
 */
errr cmd_insert_repeated(cmd_code c, int nrepeats);
errr cmd_insert(cmd_code c);

/**
 * Set the args of a command.
 */
void cmd_set_arg_choice(game_command *cmd, int n, int choice);
void cmd_set_arg_string(game_command *cmd, int n, const char *str);
void cmd_set_arg_direction(game_command *cmd, int n, int dir);
void cmd_set_arg_target(game_command *cmd, int n, int target);
void cmd_set_arg_point(game_command *cmd, int n, int x, int y);
void cmd_set_arg_item(game_command *cmd, int n, int item);
void cmd_set_arg_number(game_command *cmd, int n, int num);

/* 
 * Gets the next command from the queue, optionally waiting to allow
 * the UI time to process user input, etc. if wait is true 
 */
errr cmd_get(cmd_context c, game_command **cmd, bool wait);

/* Called by the game engine to get the player's next action. */
void process_command(cmd_context c, bool no_request);

/* Remove any pending repeats from the current command. */
void cmd_cancel_repeat(void);

/* Update the number of repeats pending for the current command. */
void cmd_set_repeat(int nrepeats);

/*
 * Call to disallow the current command from being repeated with the
 * "Repeat last command" command.
 */
void cmd_disable_repeat(void);

#endif
