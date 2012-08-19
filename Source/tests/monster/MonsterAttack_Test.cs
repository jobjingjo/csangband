using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Monster;
using RBM = CSAngband.Monster.Monster_Blow.RBM;
using RBE = CSAngband.Monster.Monster_Blow.RBE;

namespace CSAngband.Tests {
	class MonsterAttack_Test : UnitTest {
		static Monster.Monster m;

		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "monster/attack";

			suite.SetSetup(delegate() {
				Monster.Monster_Race r = UnitTest_Data.R_Human();

				m = new Monster.Monster();
				m.Race = r;
				m.r_idx = (short)r.ridx;
				Misc.r_info = new Monster.Monster_Race[] {r};

				Player.Player.instance = null;
				Random.fix(100);
			});
			suite.NoTeardown();


			suite.AddTest("blows", test_blows);
			suite.AddTest("effects", test_effects);

			UnitTest_Main.AddSuite(suite);
		}

		static int mdam(Monster.Monster m)
		{
			return m.Race.blow[0].d_dice;
		}

		static int take1(Player.Player p, Monster.Monster m, RBM blow, RBE eff)
		{
			int oldv, newv;
			m.Race.blow[0].effect = eff;
			m.Race.blow[0].method = blow;
			p.chp = p.mhp;
			oldv = p.chp;
			m.make_attack_normal(p);
			newv = p.chp;
			p.chp = p.mhp;
			return oldv - newv;
		}

		public static void test_blows() {
			Player.Player p = UnitTest_Data.Player();
			int delta;

			m.Race.flags.set(Monster_Flag.NEVER_BLOW.value);
			delta = take1(p, m, RBM.HIT, RBE.HURT);
			m.Race.flags.clear(Monster_Flag.NEVER_BLOW.value);
			
			Require(delta == 0);

			delta = take1(p, m, RBM.HIT, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.TOUCH, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.PUNCH, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.KICK, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.CLAW, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.BITE, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.STING, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.BUTT, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.CRUSH, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.ENGULF, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.CRAWL, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.DROOL, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.SPIT, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.GAZE, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.WAIL, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.SPORE, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.BEG, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.INSULT, RBE.HURT);
			Require(delta == mdam(m));
			delta = take1(p, m, RBM.MOAN, RBE.HURT);
			Require(delta == mdam(m));

			Ok();
		}

		public static void test_effects() {
			Player.Player p = UnitTest_Data.Player();
			int delta;

			Require(p.timed[(int)Timed_Effect.POISONED] == 0);
			delta = take1(p, m, RBM.HIT, RBE.POISON);
			Require(p.timed[(int)Timed_Effect.POISONED] != 0);

			delta = take1(p, m, RBM.HIT, RBE.ACID);
			Require(delta > 0);
			delta = take1(p, m, RBM.HIT, RBE.ELEC);
			Require(delta > 0);
			delta = take1(p, m, RBM.HIT, RBE.FIRE);
			Require(delta > 0);
			delta = take1(p, m, RBM.HIT, RBE.COLD);
			Require(delta > 0);

			Require(p.timed[(int)Timed_Effect.BLIND] == 0);
			delta = take1(p, m, RBM.HIT, RBE.BLIND);
			Require(p.timed[(int)Timed_Effect.BLIND] != 0);

			Ok();
		}
	}
}
