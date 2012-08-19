using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Monster {
	class Monster_Base {
		public Monster_Base Next;

		public string Name;
		public string Text;

		public Bitflag flags = new Bitflag(Monster_Flag.SIZE);         /* Flags */
		public Bitflag spell_flags = new Bitflag(Monster_Spell_Flag.SIZE);  /* Spell flags */
	
		public char d_char;			/* Default monster character */

		public Monster_Pain pain;		/* Pain messages */

		/**
		 * Return whether the given base matches any of the names given.
		 *
		 * Accepts a variable-length list of name strings. The list must end with null.
		 */
		public bool match_monster_bases(params string[] value)
		{
			throw new NotImplementedException();
			/*
			bool ok = false;
			va_list vp;
			char *name;

			va_start(vp, base);
			while (!ok && ((name = va_arg(vp, char *)) != null))
				ok = base == lookup_monster_base(name);
			va_end(vp);

			return ok;*/
		}
	}
}
