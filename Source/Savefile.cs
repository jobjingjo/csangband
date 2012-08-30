using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CSAngband {
	class Savefile {
		/**
		 * The savefile code.
		 *
		 * Savefiles since ~3.1 have used a block-based system.  Each savefile
		 * consists of an 8-byte header, the first four bytes of which mark this
		 * as a savefile, the second four bytes provide a variant ID.
		 *
		 * After that, each block has the format:
		 * - 16-byte string giving the type of block
		 * - 4-byte block version
		 * - 4-byte block size
		 * - 4-byte block checksum
		 * ... data ...
		 * padding so that block is a multiple of 4 bytes
		 *
		 * The savefile deosn't contain the version number of that game that saved it;
		 * versioning is left at the individual block level.  The current code
		 * keeps a list of savefile blocks to save in savers[] below, along with
		 * their current versions.
		 *
		 * For each block type and version, there is a loading function to load that
		 * type/version combination.  For example, there may be a loader for v1
		 * and v2 of the RNG block; these must be different functions.  It has been
		 * done this way since it allows easier maintenance; after each release, you
		 * need simply remove old loaders and you will not have to disentangle
		 * lots of code with "if (version > 3)" and its like everywhere.
		 *
		 * Savefile loading and saving is done by keeping the current block in
		 * memory, which is accessed using the wr_* and rd_* functions.  This is
		 * then written out, whole, to disk, with the appropriate header.
		 *
		 *
		 * So, if you want to make a savefile compat-breaking change, then there are
		 * a few things you should do:
		 *
		 * - increment the version in 'savers' below
		 * - add a loading function that accepts the new version (in addition to
		 *   the previous loading function) to 'loaders'
		 * - and watch the magic happen.
		 *
		 *
		 * TODO:
		 * - wr_ and rd_ should be passed a buffer to work with, rather than using
		 *   the rd_ and wr_ functions with a universal buffer
		 * - 
		 */


		/** Magic bits at beginning of savefile */
		static byte[] savefile_magic = { 83, 97, 118, 101 };
		static string savefile_name = "VNLA";

		/* Buffer bits */
		static byte[] buffer;
		static uint buffer_size;
		static uint buffer_pos;
		static uint buffer_check;

		const int BUFFER_INITIAL_SIZE	 =	1024;
		const int BUFFER_BLOCK_INCREMENT =	1024;

		/** Savefile saving functions */
		class Saver {
			//char name[16];

			public Saver(string a, save_func b, uint c){
				name = a;
				save = b;
				version = c;
			}

			public string name;
			public delegate void save_func();
			public save_func save;
			public uint version;	
		} 
		
		static Saver[] savers = {
			new Saver( "rng", Save.wr_randomizer, 1 ),
			new Saver( "options", Save.wr_options, 2 ),
			new Saver( "messages", Save.wr_messages, 1 ),
			new Saver( "monster memory", Save.wr_monster_memory, 2 ),
			new Saver( "object memory", Save.wr_object_memory, 1 ),
			new Saver( "quests", Save.wr_quests, 1 ),
			new Saver( "artifacts", Save.wr_artifacts, 2 ),
			new Saver( "player", Save.wr_player, 2 ),
			new Saver( "squelch", Save.wr_squelch, 1 ),
			new Saver( "misc", Save.wr_misc, 2 ),
			new Saver( "player hp", Save.wr_player_hp, 1 ),
			new Saver( "player spells", Save.wr_player_spells, 1 ),
			new Saver( "randarts", Save.wr_randarts, 2 ),
			new Saver( "inventory", Save.wr_inventory, 4 ),
			new Saver( "stores", Save.wr_stores, 4 ),
			new Saver( "dungeon", Save.wr_dungeon, 1 ),
			new Saver( "objects", Save.wr_objects, 4 ),
			new Saver( "monsters", Save.wr_monsters, 6 ),
			new Saver( "ghost", Save.wr_ghost, 1 ),
			new Saver( "history", Save.wr_history, 1 )
		};

		/** Savefile loading functions */
		class Loader {
			public Loader(string a, load_func b, uint c){
				name = a;
				load = b;
				version = c;
			}
			public string name;
			public delegate int load_func();
			public load_func load;
			public uint version;
		} 
		
		Loader[] loaders = {
			new Loader( "rng", Load.rd_randomizer, 1 ),
			new Loader( "options", Load.rd_options_1, 1 ),
			new Loader( "options", Load.rd_options_2, 2 ),
			new Loader( "messages", Load.rd_messages, 1 ),
			new Loader( "monster memory", Load.rd_monster_memory_1, 1 ),
			new Loader( "monster memory", Load.rd_monster_memory_2, 2 ),
			new Loader( "object memory", Load.rd_object_memory, 1 ),
			new Loader( "quests", Load.rd_quests, 1 ),
			new Loader( "artifacts", Load.rd_artifacts, 2 ),
			new Loader( "player", Load.rd_player, 2 ),
			new Loader( "squelch", Load.rd_squelch, 1 ),
			new Loader( "misc", Load.rd_misc, 1 ),
			new Loader( "misc", Load.rd_misc_2, 2),
			new Loader( "player hp", Load.rd_player_hp, 1 ),
			new Loader( "player spells", Load.rd_player_spells, 1 ),
			new Loader( "randarts", Load.rd_randarts_1, 1 ),
			new Loader( "randarts", Load.rd_randarts_2, 2 ),
			new Loader( "inventory", Load.rd_inventory_1, 1 ),
			new Loader( "inventory", Load.rd_inventory_2, 2 ),
			new Loader( "inventory", Load.rd_inventory_3, 3 ),
			new Loader( "inventory", Load.rd_inventory_4, 4 ),	
			new Loader( "stores", Load.rd_stores_1, 1 ),
			new Loader( "stores", Load.rd_stores_2, 2 ),
			new Loader( "stores", Load.rd_stores_3, 3 ),
			new Loader( "stores", Load.rd_stores_4, 4 ),	
			new Loader( "dungeon", Load.rd_dungeon, 1 ),
			new Loader( "objects", Load.rd_objects_1, 1 ),
			new Loader( "objects", Load.rd_objects_2, 2 ),
			new Loader( "objects", Load.rd_objects_3, 3 ),
			new Loader( "objects", Load.rd_objects_4, 4 ),
			new Loader( "monsters", Load.rd_monsters_1, 1 ),
			new Loader( "monsters", Load.rd_monsters_2, 2 ),
			new Loader( "monsters", Load.rd_monsters_3, 3 ),
			new Loader( "monsters", Load.rd_monsters_4, 4 ),
			new Loader( "monsters", Load.rd_monsters_5, 5 ),
			new Loader( "monsters", Load.rd_monsters_6, 6 ),
			new Loader( "ghost", Load.rd_ghost, 1 ),
			new Loader( "history", Load.rd_history, 1 ),
		};


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

		const int SAVEFILE_HEAD_SIZE	=	28;

		/*** Savefile saving functions ***/

		static bool try_save(StreamWriter file)
		{
		    byte[] savefile_head = new byte[SAVEFILE_HEAD_SIZE];
		    int i, pos = 0;

		    /* Start off the buffer */
		    buffer = new byte[BUFFER_INITIAL_SIZE];
		    buffer_size = BUFFER_INITIAL_SIZE;

		    for (i = 0; i < savers.Length; i++)
		    {
		        buffer_pos = 0;
		        buffer_check = 0;

		        savers[i].save();

		        /* 16-byte block name */
				//pos = my_strcpy((char *)savefile_head, savers[i].name,sizeof savefile_head);
				pos = savers[i].name.Length; //my_strcpy returns length of source.
				for (int n = 0; n < savers[i].name.Length; n++){
					savefile_head[n] = (byte)savers[i].name[n];
				}
				

		        while (pos < 16)
		            savefile_head[pos++] = 0;

				uint[] temp_vals = new uint[]{
					savers[i].version,
					buffer_pos,
					buffer_check
				};

				foreach (uint v in temp_vals){
					savefile_head[pos++] = (byte)(v & 0xFF);
					savefile_head[pos++] = (byte)((v >> 8) & 0xFF);
					savefile_head[pos++] = (byte)((v >> 16) & 0xFF);
					savefile_head[pos++] = (byte)((v >> 24) & 0xFF);
				}

				Misc.assert(pos == SAVEFILE_HEAD_SIZE);

				file.Write(savefile_head);
				file.Write(buffer);

				//file_write(file, (char *)savefile_head, SAVEFILE_HEAD_SIZE);
				//file_write(file, (char *)buffer, buffer_pos);

				/* pad to 4 byte multiples */
				if((buffer_pos % 4) != 0) {
					for(uint x = buffer_pos % 4; x != 0; x--) {
						file.Write((byte)'x');
					}
				}
		    }

			buffer = null;
		    //mem_free(buffer);

		    return true;
		}


		/*
		 * Attempt to save the player in a savefile
		 */
		public static bool savefile_save(string path)
		{
			//ang_file *file;
			int count = 0;
			string new_savefile;//[1024];
			string old_savefile;//[1024];

			/* New savefile */
			old_savefile = String.Format("{0}{1}.old", path, Random.Rand_simple(1000000));
			while (File.Exists(old_savefile) && (count++ < 100)) {
				old_savefile = String.Format("{0}{1}{2}.old", path, Random.Rand_simple(1000000),count);
			}
			count = 0;
			/* Make sure that the savefile doesn't already exist */
			/*safe_setuid_grab();
			file_delete(new_savefile);
			file_delete(old_savefile);
			safe_setuid_drop();*/

			/* Open the savefile */

			//safe_setuid_grab();
			new_savefile = String.Format("{0}{1}.new", path, Random.Rand_simple(1000000));
			while (File.Exists(new_savefile) && (count++ < 100)) {
				new_savefile = String.Format("{0}{1}{2}.new", path, Random.Rand_simple(1000000),count);
			}
			FileStream fs = File.Open(new_savefile, FileMode.Create, FileAccess.Write);
			StreamWriter file = new StreamWriter(fs);
			//safe_setuid_drop();

			if (file != null)
			{
				file.Write(savefile_magic);
				file.Write(savefile_name);

				Player.Player.character_saved = try_save(file);

				fs.Close();
			}

			throw new NotImplementedException();

			//if (Player.Player.character_saved)
			//{
			//    bool err = false;

			//    //safe_setuid_grab();

			//    if (file_exists(savefile) && !file_move(savefile, old_savefile))
			//        err = true;

			//    if (!err)
			//    {
			//        if (!file_move(new_savefile, savefile))
			//            err = true;

			//        if (err)
			//            file_move(old_savefile, savefile);
			//        else
			//            file_delete(old_savefile);
			//    } 

			//    //safe_setuid_drop();

			//    return err ? false : true;
			//}

			///* Delete temp file if the save failed */
			//if (file)
			//{
			//    /* file is no longer valid, but it still points to a non zero
			//     * value if the file was created above */
			//    safe_setuid_grab();
			//    file_delete(new_savefile);
			//    safe_setuid_drop();
			//}
			//return false;
		}
	}
}
