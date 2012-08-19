using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Pit_Profile {
		public Pit_Profile next;

		public int pit_idx; /* Index in pit_info */
		public string name;
		public int room_type; /* Is this a pit or a nest? */
		public int ave; /* Level where this pit is most common */
		public int rarity; /* How unusual this pit is */
		public int obj_rarity; /* How rare objects are in this pit */
		public Bitflag flags = new Bitflag(Monster.Monster_Flag.SIZE);         /* Required flags */
		public Bitflag spell_flags = new Bitflag(Monster.Monster_Flag.SIZE);  /* Required spell flags */
		public Bitflag forbidden_spell_flags = new Bitflag(Monster.Monster_Flag.SIZE); 
		public int n_bases;
		public Monster.Monster_Base[] Base = new Monster.Monster_Base[Monster.Monster_Flag.SIZE];
	}
}
