using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Monster {
	/*
	 * Monster blow structure
	 *
	 *	- Method (RBM_*)
	 *	- Effect (RBE_*)
	 *	- Damage Dice
	 *	- Damage Sides
	 */
	class Monster_Blow {
		public const int MONSTER_BLOW_MAX = 4;

		/*
		 * Monster blow methods
		 */
		public class RBM
		{
			public static List<RBM> list = new List<RBM>();
			public string name;
			public string text;
			public int value;
			public bool do_cut;
			public bool do_stun;
			public string action;
			public Message_Type sound_msg;

			public RBM(string name, string text, int value, bool docut, bool dostun, Message_Type sound_msg, string action){
				this.name = name;
				this.text = text;
				this.value = value;
				this.do_cut = docut;
				this.do_stun = dostun;
				this.action = action;
				this.sound_msg = sound_msg;

				list.Add(this);
			}
			public static RBM NONE =	new RBM("NONE", "",					0,	false, false, Message_Type.MSG_GENERIC, "");
			public static RBM HIT =		new RBM("HIT", "hit",				1,	true, true,	Message_Type.MSG_MON_HIT, "hits you.");
			public static RBM TOUCH=	new RBM("TOUCH", "touch",			2,	false, false, Message_Type.MSG_MON_TOUCH,"touches you.");
			public static RBM PUNCH=	new RBM("PUNCH", "punch",			3,	false, true, Message_Type.MSG_MON_PUNCH, "punches you.");
			public static RBM KICK=		new RBM("KICK", "kick",				4,	false, true, Message_Type.MSG_MON_KICK, "kicks you.");
			public static RBM CLAW=		new RBM("CLAW", "claw",				5,	true, false, Message_Type.MSG_MON_CLAW, "claws you.");
			public static RBM BITE=		new RBM("BITE", "bite",				6,	true, false, Message_Type.MSG_MON_BITE, "bites you.");
			public static RBM STING=	new RBM("STING", "sting",			7,	false, false, Message_Type.MSG_MON_STING, "stings you.");
			public static RBM BUTT=		new RBM("BUTT", "butt",				8,	false, true, Message_Type.MSG_MON_BUTT, "butts you.");
			public static RBM CRUSH=	new RBM("CRUSH", "crush",			9,	false, true, Message_Type.MSG_MON_CRUSH, "crushes you.");
			public static RBM ENGULF=	new RBM("ENGULF", "engulf",			10,	false, false, Message_Type.MSG_MON_ENGULF, "engulfs you.");
			public static RBM CRAWL=	new RBM("CRAWL", "crawl on you",		11,	false, false, Message_Type.MSG_MON_CRAWL, "crawls on you.");
			public static RBM DROOL=	new RBM("DROOL", "drool on you",		12,	false, false, Message_Type.MSG_MON_DROOL, "drools on you.");
			public static RBM SPIT=		new RBM("SPIT", "spit",				13,	false, false, Message_Type.MSG_MON_SPIT, "spits on you.");
			public static RBM GAZE=		new RBM("GAZE", "gaze",				14,	false, false, Message_Type.MSG_MON_GAZE, "gazes at you.");
			public static RBM WAIL=		new RBM("WAIL", "wail",				15,	false, false, Message_Type.MSG_MON_WAIL, "wails at you.");
			public static RBM SPORE=	new RBM("SPORE", "release spores",	16,	false, false, Message_Type.MSG_MON_SPORE, "releases spores at you.");
			public static RBM BEG=		new RBM("BEG", "beg",				17,	false, false, Message_Type.MSG_MON_BEG, "begs you for money.");
			public static RBM INSULT=	new RBM("INSULT", "insult",			18,	false, false, Message_Type.MSG_MON_INSULT, "");
			public static RBM MOAN=		new RBM("MOAN", "moan",				19,	false, false, Message_Type.MSG_MON_MOAN, "");
			public static RBM MAX =		new RBM("MAX", "ERROR: RBM.MAX SHOULD NEVER BE SHOWN TO THE PLAYER", 20, true, true, Message_Type.MSG_MAX, "ERROR");
		};

		/*
		 * Monster blow effects
		 */
		public class RBE
		{
			public static List<RBE> list = new List<RBE>();
			public string name;
			public string text;
			public int value;
			public int power;

			public RBE(string name, string text, int value, int power){
				this.name = name;
				this.text = text;
				this.value = value;
				this.power = power;

				list.Add(this);
			}

			public static RBE NONE=      new RBE("NONE", "",					0,	0);
			public static RBE HURT=      new RBE("HURT", "attack",				1,	40);
			public static RBE POISON=    new RBE("POISON", "poison",				2,	20);
			public static RBE UN_BONUS=  new RBE("UN_BONUS", "disenchant",			3,	10);
			public static RBE UN_POWER=  new RBE("UN_POWER", "drain charges",		4,	10);
			public static RBE EAT_GOLD=  new RBE("EAT_GOLD", "steal gold",			5,	0);
			public static RBE EAT_ITEM=  new RBE("EAT_ITEM", "steal items",			6,	0);
			public static RBE EAT_FOOD=  new RBE("EAT_FOOD", "eat your food",		7,	0);
			public static RBE EAT_LIGHT= new RBE("EAT_LIGHT", "absorb light",		8,	0);
			public static RBE ACID=      new RBE("ACID", "shoot acid",			9,	20);
			public static RBE ELEC=      new RBE("ELEC", "electrify",			10,	40);
			public static RBE FIRE=      new RBE("FIRE", "burn",				11,	40);
			public static RBE COLD=      new RBE("COLD", "freeze",				12,	40);
			public static RBE BLIND=     new RBE("BLIND", "blind",				13,	0);
			public static RBE CONFUSE=   new RBE("CONFUSE", "confuse",				14,	20);
			public static RBE TERRIFY=   new RBE("TERRIFY", "terrify",				15,	0);
			public static RBE PARALYZE=  new RBE("PARALYZE", "paralyze",			16,	0);
			public static RBE LOSE_STR=  new RBE("LOSE_STR", "reduce strength",		17,	0);
			public static RBE LOSE_INT=  new RBE("LOSE_INT", "reduce intelligence", 18,	0);
			public static RBE LOSE_WIS=  new RBE("LOSE_WIS", "reduce wisdom",		19,	0);
			public static RBE LOSE_DEX=  new RBE("LOSE_DEX", "reduce dexterity",	20,	0);
			public static RBE LOSE_CON=  new RBE("LOSE_CON", "reduce constitution", 21,	0);
			public static RBE LOSE_CHR=  new RBE("LOSE_CHR", "reduce charisma",		22,	0);
			public static RBE LOSE_ALL=  new RBE("LOSE_ALL", "reduce all stats",	23,	0);
			public static RBE SHATTER=   new RBE("SHATTER", "shatter",				24,	60);
			public static RBE EXP_10=    new RBE("EXP_10", "lower experience",	25,	20);
			public static RBE EXP_20=    new RBE("EXP_20", "lower experience",	26,	20);
			public static RBE EXP_40=    new RBE("EXP_40", "lower experience",	27,	20);
			public static RBE EXP_80=    new RBE("EXP_80", "lower experience",	28,	20);
			public static RBE HALLU=     new RBE("HALLU", "cause hallucinations",29,	0);
			public static RBE MAX = new RBE("MAX", "ERROR: RBE.MAX SHOULD NEVER BE SHOWN TO THE PLAYER", 30, 99);
		};


		public Monster_Blow() {
			method = RBM.NONE;
			effect = RBE.NONE;
			d_dice = 0;
			d_side = 0;
		}
		public Monster_Blow(RBM method, RBE effect, byte d_dice, byte d_side) {
			this.method = method;
			this.effect = effect;
			this.d_dice = d_dice;
			this.d_side = d_side;
		}
		public RBM method;
		public RBE effect;
		public byte d_dice;
		public byte d_side;
	}
}
