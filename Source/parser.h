/** Init file parser library
 *
 * The basic structure of the parser is as follows: there is a table of hooks
 * which are run when a directive matching their format is encountered. When the
 * hook is called, all the arguments it declares in its format have been parsed
 * out and can be accessed with parser_get*(). See the unit tests for examples.
 */
#ifndef PARSER_H
#define PARSER_H

#include "h-basic.h"
#include "z-bitflag.h"
#include "z-rand.h"

struct parser;

extern const char *parser_error_str[PARSE_ERROR_MAX];

/** Sets parser's private data.
 *
 * This is commonly used to store context for stateful parsing.
 */
extern void parser_setpriv(struct parser *p, void *v);

/** Returns whether the parser has a value named `name`.
 *
 * Used to test for presence of optional values.
 */
extern bool parser_hasval(struct parser *p, const char *name);


/** Returns the unsigned integer named `name`. This symbol must exist. */
extern unsigned int parser_getuint(struct parser *p, const char *name);

/** Returns the random value named `name`. This symbol must exist. */
extern struct random parser_getrand(struct parser *p, const char *name);

/** Returns the character named `name`. This symbol must exist. */
extern char parser_getchar(struct parser *p, const char *name);

/** Fills the provided struct with the parser's state, if any. Returns true if
 * the parser is in an error state, and false otherwise.
 */
extern int parser_getstate(struct parser *p, struct parser_state *s);

/** Sets the parser's detailed error description and field number. */
extern void parser_setstate(struct parser *p, unsigned int col, const char *msg);

void cleanup_parser(struct file_parser *fp);
int lookup_flag(const char **flag_table, const char *flag_name);
errr grab_flag(bitflag *flags, const size_t size, const char **flag_table, const char *flag_name);
errr remove_flag(bitflag *flags, const size_t size, const char **flag_table, const char *flag_name);

#endif /* !PARSER_H */
