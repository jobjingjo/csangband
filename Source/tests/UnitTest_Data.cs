using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class UnitTest_Data {
		public static Maxima Z_Info() {
			Maxima ret = new Maxima();
			ret.f_max = 2;
			ret.k_max = 2;
			ret.a_max = 2;
			ret.e_max = 2;
			ret.r_max = 2;
			ret.mp_max = 2;
			ret.s_max = 2;
			ret.pit_max = 2;
			ret.o_max = 2;
			ret.m_max = 2;
			return ret;
		}

		public static Player.Player_Sex Player_Sex() {
			Player.Player_Sex ps = new Player.Player_Sex("Test Sex", "Test Winner");
			return ps;
		}

		public static Object.Object_Base Sword_Base(){
			Object.Object_Base ob = new Object.Object_Base();
			ob.Name = "Test Sword";
			ob.tval = Object.TVal.TV_SWORD;
			ob.Next = null;
			ob.break_perc = 50;
			return ob;
		}
		
		public static Object.Artifact Artifact_Sword(){
			Object.Artifact a = new Object.Artifact();
			a.Name = "Test Artifact";
			a.Text = "A test artifact.";
			a.aidx = 0;
			a.Next = null;
			a.tval = Object.TVal.TV_SWORD;
			a.sval = (int)Object.SVal.sval_sword.SV_LONG_SWORD;
			a.to_a = 1;
			a.to_h = 2;
			a.to_d = 3;
			a.ac = 5;
			a.dd = 2;
			a.ds = 5;
			a.weight = 16;
			a.cost = 40;
			return a;
		}

		public static Object.Object_Kind Longsword(){
			Object.Object_Kind ok = new Object.Object_Kind();
			ok.Name = "Test Longsword";
			ok.Text = "A test longsword [0].";
			ok.Base = Sword_Base();
			ok.kidx = 0;
			ok.tval = Object.TVal.TV_SWORD;
			ok.sval = (int)Object.SVal.sval_sword.SV_LONG_SWORD;
			ok.pval = new random_value[]{
					new random_value(),
					new random_value(),
					new random_value()
			};

			ok.to_h = new random_value(1, 0, 0, 0);
			ok.to_d = new random_value(1, 0, 0, 0);
			ok.to_a = new random_value(2, 0, 0, 0);

			ok.dd = 4;
			ok.ds = 6;
			ok.weight = 16;

			ok.cost = 20;

			ok.d_attr = 0;
			ok.d_char = '|';

			ok.alloc_prob = 20;
			ok.alloc_min = 1;
			ok.alloc_max = 10;
			ok.level = 0;

			ok.effect = Effect.XXX; //That's default, right?
			ok.gen_mult_prob = 0;
			ok.flavor = null;
			return ok;
		}

		public static Object.Object_Kind Torch(){
			Object.Object_Kind t = new Object.Object_Kind();
			t.Name = "Test Torch";
			t.Text = "A test torch [1].";
			t.kidx = 1;
			t.tval = Object.TVal.TV_LIGHT;
			t.sval = Object.SVal.SV_LIGHT_TORCH;
			t.pval = new random_value[]{
					new random_value(5000, 0, 0, 0),
					new random_value(),
					new random_value(),
			};

			t.to_h = new random_value();
			t.to_d = new random_value();
			t.to_a = new random_value();

			t.dd = 1;
			t.ds = 1;
			t.weight = 10;

			t.cost = 1;

			t.d_attr = 0;
			t.d_char = '~';

			t.alloc_prob = 10;
			t.alloc_min = 1;
			t.alloc_max = 10;
			t.level = 0;

			t.effect = Effect.XXX;
			t.gen_mult_prob = 0;
			t.flavor = null;
			return t;
		}

		public static Object.Object_Kind Gold(){
			Object.Object_Kind g = new Object.Object_Kind();
			g.Name = "Test Gold";
			g.Text = "Test gold [2].";
			g.kidx = 2;
			g.tval = Object.TVal.TV_GOLD;
			g.sval = 0;
			g.pval = new random_value[]{
					new random_value(),
					new random_value(),
					new random_value(),
			};

			g.to_h = new random_value();
			g.to_d = new random_value();
			g.to_a = new random_value();

			g.dd = 1;
			g.ds = 1;
			g.weight = 1;

			g.cost = 0;

			g.d_attr = 0;
			g.d_char = '$';

			g.alloc_prob = 0;
			g.alloc_min = 0;
			g.alloc_max = 0;
			g.level = 0;

			g.effect = Effect.XXX;
			g.gen_mult_prob = 0;
			g.flavor = null;
			return g;
		}

		public static Player.Player_Race Race(){
			Player.Player_Race pr = new Player.Player_Race();
			pr.Name = "TestRace";
			pr.r_adj = new Int16[]{
				/*STR*/ +2,
				/*INT*/ -1,
				/*WIS*/ -2,
				/*DEX*/ +1,
				/*CON*/ +3,
				/*CHR*/ +0
			};

			pr.r_skills = new Int16[]{
				0,	//DISARM
				5,	//DEVICE
				10,	//SAVE
				-5,	//STEALTH
				-10,//SEARCH
				10,	//SEARCH_FREQUENCY
				0,	//TO_HIT_MELEE
				0,	//TO_HIT_BOW
				0,	//TO_HIT_THROW
				0	//DIGGING
			};

			pr.r_mhp = 10;
			pr.r_exp = 110;

			pr.b_age = 14;
			pr.m_age = 6;

			pr.m_b_ht = 72;
			pr.m_m_ht = 6;
			pr.f_b_ht = 66;
			pr.f_m_ht = 4;

			pr.m_b_wt = 180;
			pr.m_m_wt = 25;
			pr.f_b_wt = 150;
			pr.f_m_wt = 20;

			pr.infra = 40;

			pr.choice = 0xFF;

			pr.history = null;
			return pr;
		}

		public static Player.Start_Item Start_Torch(){
			Player.Start_Item si = new Player.Start_Item();
			si.kind = Torch();
			si.min = 3;
			si.max = 5;
			si.next = null;
			return si;
		}

		public static Player.Start_Item Start_Longsword(){
			Player.Start_Item ls = new Player.Start_Item();
			ls.kind = Longsword();
			ls.min = 1;
			ls.max = 1;
			ls.next = Start_Torch();
			return ls;
		}

		public static Player.Player_Class Class(){
			Player.Player_Class pc = new Player.Player_Class();
			pc.Name = "TestClass";
			pc.title = new string[]{
				"TestTitle0",
				"TestTitle1",
				"TestTitle2",
				"TestTitle3",
				"TestTitle4",
				"TestTitle5",
				"TestTitle6",
				"TestTitle7",
				"TestTitle8",
				"TestTitle9",
			};

			pc.c_adj = new Int16[]{
				+1, //str
				-2, //int
				+3, //wis
				+2, //dex
				-1, //con
				+0, //chr
			};

			pc.c_skills = new Int16[]{
				25, //SKILL_DISARM
				18, //SKILL_DEVICE
				18, //SKILL_SAVE
				1, //SKILL_STEALTH
				14, //SKILL_SEARCH
				2, //SKILL_SEARCH_FREQUENCY
				70, //SKILL_TO_HIT_MELEE
				55, //SKILL_TO_HIT_BOW
				55, //SKILL_TO_HIT_THROW
				0, //SKILL_DIGGING
			};

			pc.x_skills = new Int16[]{
				10,//SKILL_DISARM
				7, //SKILL_DEVICE
				10,//SKILL_SAVE
				0, //SKILL_STEALTH
				0, //SKILL_SEARCH
				0, //SKILL_SEARCH_FREQUENCY
				45,//SKILL_TO_HIT_MELEE
				45,//SKILL_TO_HIT_BOW
				45,//SKILL_TO_HIT_THROW
				0  //SKILL_DIGGING
			};

			pc.c_mhp = 9;
			pc.c_exp = 0;

			pc.max_attacks = 6;
			pc.min_weight = 30;
			pc.att_multiply = 5;

			pc.spell_book = 0;
			pc.spell_stat = 0;
			pc.spell_first = 0;
			pc.spell_weight = 0;

			pc.sense_base = 7000;
			pc.sense_div = 40;

			pc.start_items = Start_Longsword();
			return pc;
		}

		public static Monster.Monster_Base RB_Info(){
			Monster.Monster_Base mb = new Monster.Monster_Base();
			mb.Next = null;
			mb.Name = "townsfolk";
			mb.Text = "Townsfolk";
			mb.flags.data = new byte[]{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
			mb.spell_flags.data = new byte[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
			mb.d_char = (char)116; //This might be a "t"
			mb.pain = null;
			return mb;
		}

		public static Monster.Monster_Race R_Human(){
			Monster.Monster_Race mr = new Monster.Monster_Race();
			mr.Next = null;
			mr.ridx = 0;
			mr.Name = "Human";
			mr.Text = "A random test human";

			mr.Base = RB_Info();

			mr.avg_hp = 10;
			mr.ac = 12;
			mr.sleep = 0;
			mr.aaf = 20;
			mr.speed = 110;
			mr.mexp = 50;
			mr.power = 1;
			mr.scaled_power = 1;
			mr.highest_threat = 5;
			mr.freq_innate = 0;
			mr.freq_spell = 0;

			mr.blow = new Monster.Monster_Blow[]{
				new Monster.Monster_Blow(Monster.Monster_Blow.RBM.HIT, Monster.Monster_Blow.RBE.HURT, 3, 1),
				new Monster.Monster_Blow(),
				new Monster.Monster_Blow(),
				new Monster.Monster_Blow()
			};

			mr.level = 1;
			mr.rarity = 1;

			mr.d_attr = 0;
			mr.d_char = 't';

			mr.x_attr = 0;
			mr.x_char = 't';

			mr.max_num = 100;
			mr.cur_num = 0;

			mr.drops = null;
			return mr;
		}

		static Object.Object[] Test_Inven = new Object.Object[Misc.ALL_INVEN_TOTAL];

		public static Player.Player Player(){
			Player.Player p = new Player.Player();
			p.py = 1;
			p.px = 1;
			p.psex = 0;
			p.Sex = Player_Sex();
			p.Race = Race();
			p.Class = Class();
			p.hitdie = 10;
			p.expfact = 100;
			p.age = 12;
			p.ht = 40;
			p.wt = 80;
			p.sc = 100;
			p.au = 500;
			p.max_depth = 10;
			p.depth = 6;
			p.max_lev = 3;
			p.lev = 3;
			p.max_exp = 100;
			p.exp = 80;
			p.mhp = 20;
			p.chp = 14;
			p.msp = 12;
			p.csp = 11;
			p.stat_max = new short[]{
				14, //str
				8, //int
				10, //wis
				12, //dex
				14, //con
				12 //chr
			};
			p.stat_cur = new short[]{
				14, //str
				8, //int
				10, //wis
				11, //dex
				14, //con
				8 //chr
			};
			p.word_recall = 0;
			p.energy = 100;
			p.food = 5000;
			p.player_hp = new short[]{
				  5,  10,  15,  20,  25,  30,  35,  40,  45,  50,
				 55,  60,  65,  70,  75,  80,  85,  90,  95, 100,
				105, 110, 115, 120, 125, 130, 135, 140, 145, 150,
				155, 160, 165, 170, 175, 180, 185, 190, 195, 200,
				205, 210, 215, 220, 225, 230, 235, 240, 245, 250
			};
			p.history = "no history";
			p.is_dead = false;
			p.wizard = false;
			p.inventory = Test_Inven;
			return p;
		}
	}
}
