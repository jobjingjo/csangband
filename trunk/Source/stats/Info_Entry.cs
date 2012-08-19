using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Entries for spell/activation descriptions
 */
namespace CSAngband {
	class Info_Entry {
		UInt16 index;   /* Effect index */
		bool aim;       /* Whether the effect requires aiming */
		UInt16 power;   /* Power rating for obj-power.c */
		string desc;    /* Effect description */
	}
}
