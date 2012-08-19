using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Command_List {
		/*
		 * A categorised list of all the command lists.
		 */
		public string name;
		public Command_Info[] list;

		public Command_List(string name, Command_Info[] list) {
			this.name = name;
			this.list = list;
		}

		public static Command_List[] all = new Command_List[]
		{
			new Command_List("Items",           Command_Info.cmd_item),
			new Command_List("Action commands", Command_Info.cmd_action),
			new Command_List("Manage items",    Command_Info.cmd_item_manage),
			new Command_List("Information",     Command_Info.cmd_info),
			new Command_List("Utility",         Command_Info.cmd_util),
			new Command_List("Hidden",          Command_Info.cmd_hidden)
		};
	}
}
