using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;

namespace CSAngband {
	partial class Do_Command {
		/*
		 * Check to see if the player can use a rod/wand/staff/activatable object.
		 */
		static int check_devices(Object.Object o_ptr)
		{
			throw new NotImplementedException();
			//int fail;
			//const char *action;
			//const char *what = null;

			///* Get the right string */
			//switch (o_ptr.tval)
			//{
			//    case TV_ROD:   action = "zap the rod";   break;
			//    case TV_WAND:  action = "use the wand";  what = "wand";  break;
			//    case TV_STAFF: action = "use the staff"; what = "staff"; break;
			//    default:       action = "activate it";  break;
			//}

			///* Figure out how hard the item is to use */
			//fail = get_use_device_chance(o_ptr);

			///* Roll for usage */
			//if (randint1(1000) < fail)
			//{
			//    flush();
			//    msg("You failed to %s properly.", action);
			//    return false;
			//}

			///* Notice empty staffs */
			//if (what && o_ptr.pval[DEFAULT_PVAL] <= 0)
			//{
			//    flush();
			//    msg("The %s has no charges left.", what);
			//    o_ptr.ident |= (IDENT_EMPTY);
			//    return false;
			//}

			//return true;
		}


		/*
		 * Return the chance of an effect beaming, given a tval.
		 */
		static int beam_chance(int tval)
		{
			switch (tval)
			{
				case TVal.TV_WAND: return 20;
				case TVal.TV_ROD:  return 10;
			}

			return 0;
		}
	}
}
