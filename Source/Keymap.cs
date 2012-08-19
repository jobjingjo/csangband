using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CSAngband {
	/**
	 * Keymap implementation.
	 *
	 * Keymaps are defined in pref files and map onto the internal game keyset,
	 * which is roughly what you get if you have roguelike keys turned off.
	 *
	 * We store keymaps by pairing triggers with actions; the trigger is a single
	 * keypress and the action is stored as a string of keypresses, terminated
	 * with a keypress with type == EVT_NONE.
	 *
	 * XXX We should note when we read in keymaps that are "official game" keymaps
	 * and ones which are user-defined.  Then we can avoid writing out official
	 * game ones and messing up everyone's pref files with a load of junk.
	 */
	class Keymap {
		/** Maximum number of keypresses a trigger can map to. **/
		public const int ACTION_MAX = 20;
		
		/** Keymap modes. */
		public enum Mode{
			ORIG = 0,
			ROGUE,

			MAX
		};

		public keypress key;
		public keypress[] actions;
		public bool user;		/* User-defined keymap */
		public Keymap next;

		/**
		 * List of keymaps.
		 */
		static Keymap[] keymaps = new Keymap[(int)Mode.MAX];

		/**
		 * Add a keymap to the mappings table.
		 */
		public static void add(int keymap, keypress trigger, keypress[] actions, bool user)
		{
			Keymap k = new Keymap();
			Misc.assert(keymap >= 0 && keymap < (int)Mode.MAX);

			remove(keymap, trigger);

			k.key = trigger;
			k.actions = make(actions);
			k.user = user;

			k.next = keymaps[keymap];
			keymaps[keymap] = k;

			return;
		}

		/**
		 * Find a keymap, given a keypress.
		 */
		public static keypress[] find(int keymap, keypress kc)
		{
			Keymap k;
			Misc.assert(keymap >= 0 && keymap < (int)Keymap.Mode.MAX);
			for (k = keymaps[keymap]; k != null; k = k.next) {
			    if (k.key.code == kc.code && k.key.mods == kc.mods)
			        return k.actions;
			}

			return null;
		}


		/**
		 * Duplicate a given keypress string and return the duplicate.
		 */
		public static keypress[] make(keypress[] actions)
		{
			keypress[] newk;
			int n = actions.Length;

			/* Make room for the terminator */
			//No terminator, we are smart...
			//n += 1;

			newk = new keypress[n];
			for (int i = 0; i < n; i++){
				newk[i] = actions[i];
			}

			return newk;
		}



		/**
		 * Remove a keymap.  Return true if one was removed.
		 */
		public static bool remove(int keymap, keypress trigger)
		{
			Keymap k;
			Keymap prev = null;
			Misc.assert(keymap >= 0 && keymap < (int)Mode.MAX);

			for (k = keymaps[keymap]; k != null; k = k.next) {
			    if (k.key.code == trigger.code && k.key.mods == trigger.mods) {
			        if (prev != null)
			            prev.next = k.next;
			        else
			            keymaps[keymap] = k.next;
			        return true;
			    }

			    prev = k;
			}

			return false;
		}


		/**
		 * Forget and free all keymaps.
		 */
		public static void free()
		{
			throw new NotImplementedException();
			//size_t i;
			//Keymap k;
			//for (i = 0; i < N_ELEMENTS(keymaps); i++) {
			//    k = keymaps[i];
			//    while (k) {
			//        Keymap next = k.next;
			//        mem_free(k.actions);
			//        mem_free(k);
			//        k = next;
			//    }
			//}
		}


		/*
		 * Append active keymaps to a given file.
		 */
		public static void dump(FileStream fff)
		{
			throw new NotImplementedException();
			//int mode;
			//Keymap k;

			//if (OPT(rogue_like_commands))
			//    mode = KEYMAP_MODE_ROGUE;
			//else
			//    mode = KEYMAP_MODE_ORIG;

			//for (k = keymaps[mode]; k; k = k.next) {
			//    char buf[1024];
			//    keypress key[2] = { { 0 }, { 0 } };

			//    if (!k.user) continue;

			//    /* Encode the action */
			//    keypress_to_text(buf, sizeof(buf), k.actions, false);
			//    file_putf(fff, "A:%s\n", buf);

			//    /* Convert the key into a string */
			//    key[0] = k.key;
			//    keypress_to_text(buf, sizeof(buf), key, true);
			//    file_putf(fff, "C:%d:%s\n", mode, buf);

			//    file_putf(fff, "\n");
			//}
		}
	}
}
