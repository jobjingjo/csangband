using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Source
{
    class Table
    {
        /*
         * Certain "screens" always use the main screen, including News, Birth,
         * Dungeon, Tomb-stone, High-scores, Macros, Colors, Visuals, Options.
         *
         * Later, special flags may allow sub-windows to "steal" stuff from the
         * main window, including File dump (help), File dump (artifacts, uniques),
         * Character screen, Small scale map, Previous Messages, Store screen, etc.
         */
        const char *window_flag_desc[32] =
        {
	        "Display inven/equip",
	        "Display equip/inven",
	        "Display player (basic)",
	        "Display player (extra)",
	        "Display player (compact)",
	        "Display map view",
	        "Display messages",
	        "Display overhead view",
	        "Display monster recall",
	        "Display object recall",
	        "Display monster list",
	        "Display status",
	        "Display item list",
	        null,
	        "Display borg messages",
	        "Display borg status",
	        null,
	        null,
	        null,
	        null,
	        null,
	        null,
	        null,
	        null,
	        null,
	        null,
	        null,
	        null,
	        null,
	        null,
	        null,
	        null
        };
    }
}
