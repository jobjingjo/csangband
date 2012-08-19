using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	static class Attack {
		class attack_result {
			bool success;
			int dmg;
			UInt32 msg_type;
			string hit_verb;
		};

		//public static int breakage_chance(const object_type *o_ptr, bool hit_target);
		public static bool test_hit(int chance, int ac, int vis) {
			return false;
		}
		public static void py_attack(int y, int x) {
		}

		/**
		 * ranged_attack is a function pointer, used to execute a kind of attack.
		 *
		 * This allows us to abstract details of throwing, shooting, etc. out while
		 * keeping the core projectile tracking, monster cleanup, and display code
		 * in common.
		 */
		//Make this a delegate
		//public static typedef struct attack_result (*ranged_attack) (object_type *o_ptr, int y, int x);
	}
}
