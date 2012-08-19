using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class UnitTest_Utils {
		/*
		 * Call this function to simulate init_stuff() and populate the *_info arrays
		 */
		public static void read_edit_files() {
			//All these have max length of 512 in original?
			string configpath = Config.DEFAULT_CONFIG_PATH;
			string libpath = Config.DEFAULT_LIB_PATH;
			string datapath = Config.DEFAULT_DATA_PATH;

			//We definitely end with Path_Sep (the '/')
			/*if (!suffix(configpath, PATH_SEP))
				my_strcat(configpath, PATH_SEP, sizeof(configpath));
			if (!suffix(libpath, PATH_SEP))
				my_strcat(libpath, PATH_SEP, sizeof(libpath));
			if (!suffix(datapath, PATH_SEP))
				my_strcat(datapath, PATH_SEP, sizeof(datapath));*/


			Init.file_paths(configpath, libpath, datapath);
			Init.arrays();
		}
	}
}
