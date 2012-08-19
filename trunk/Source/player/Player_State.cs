using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Player {
	class Player_State {
		public Int16 speed;		/* Current speed */

		public Int16 num_blows;		/* Number of blows x100 */
		public Int16 num_shots;		/* Number of shots */

		public byte ammo_mult;		/* Ammo multiplier */
		public byte ammo_tval;		/* Ammo variety */

		public Int16[] stat_add = new Int16[(int)Stat.Max];	/* Equipment stat bonuses */
		public Int16[] stat_ind = new Int16[(int)Stat.Max];	/* Indexes into stat tables */
		public Int16[] stat_use = new Int16[(int)Stat.Max];	/* Current modified stats */
		public Int16[] stat_top = new Int16[(int)Stat.Max];	/* Maximal modified stats */

		public Int16 dis_ac;		/* Known base ac */
		public Int16 ac;			/* Base ac */

		public Int16 dis_to_a;		/* Known bonus to ac */
		public Int16 to_a;			/* Bonus to ac */

		public Int16 to_h;			/* Bonus to hit */
		public Int16 dis_to_h;		/* Known bonus to hit */

		public Int16 to_d;			/* Bonus to dam */
		public Int16 dis_to_d;		/* Known bonus to dam */

		public Int16 see_infra;		/* Infravision range */

		public Int16[] skills = new Int16[(int)Skill.MAX];	/* Skills */

		public UInt32 noise;			/* Derived from stealth */

		public bool heavy_wield;	/* Heavy weapon */
		public bool heavy_shoot;	/* Heavy shooter */
		public bool icky_wield;	/* Icky weapon shooter */

		public Bitflag flags = new Bitflag(Object.Object_Flag.SIZE);	/* Status flags from race and items */

		public Player_State() {
		}

		public Player_State(Player_State state) {
			speed = state.speed;

			num_blows = state.num_blows;
			num_shots = state.num_shots;

			ammo_mult = state.ammo_mult;
			ammo_tval = state.ammo_tval;

			state.stat_add.CopyTo(stat_add, 0);
			state.stat_ind.CopyTo(stat_ind, 0);
			state.stat_use.CopyTo(stat_use, 0);
			state.stat_top.CopyTo(stat_top, 0);

			dis_ac = state.dis_ac;
			ac = state.ac;	

			dis_to_a = state.dis_to_a;
			to_a = state.to_a;	

			to_h = state.to_h;		
			dis_to_h = state.dis_to_h;	
			
			to_d = state.to_d;
			dis_to_d = state.dis_to_d;

			see_infra = state.see_infra;

			state.skills.CopyTo(skills, 0);

			noise = state.noise;

			heavy_wield = state.heavy_wield;	/* Heavy weapon */
			heavy_shoot = state.heavy_shoot;	/* Heavy shooter */
			icky_wield = state.icky_wield;	/* Icky weapon shooter */

			flags.copy(state.flags);
		}

		/*
		 * Computes current weight limit.
		 */
		public int weight_limit()
		{
			int i;

			/* Weight limit based only on strength */
			i = Player.adj_str_wgt[stat_ind[(int)Stat.Str]] * 100;

			/* Return the result */
			return (i);
		}
	}
}
