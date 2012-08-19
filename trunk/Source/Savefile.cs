using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Savefile {
		/**
		 * Load a savefile.
		 */
		public static bool load(string path)
		{
			throw new NotImplementedException();
			//byte head[8];
			//bool ok = true;

			//ang_file *f = file_open(path, MODE_READ, -1);
			//if (f) {
			//    if (file_read(f, (char *) &head, 8) == 8 &&
			//            memcmp(&head[0], savefile_magic, 4) == 0 &&
			//            memcmp(&head[4], savefile_name, 4) == 0) {
			//        if (!try_load(f)) {
			//            ok = false;
			//            note("Failed loading savefile.");
			//        }
			//    } else {
			//        ok = false;
			//        note("Savefile is corrupted -- incorrect file header.");
			//    }

			//    file_close(f);
			//} else {
			//    ok = false;
			//    note("Couldn't open savefile.");
			//}

			//return ok;
		}
	}
}
