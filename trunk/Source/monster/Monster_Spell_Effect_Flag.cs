using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;

namespace CSAngband.Monster {
	class Monster_Spell_Effect_Flag {
		public static List<Monster_Spell_Effect_Flag> list = new List<Monster_Spell_Effect_Flag>();

		public Monster_Spell_Effect_Flag(int index, Monster_Spell_Flag method, GF gf, bool timed, Timed_Effect flag,
						random_value Base, random_value dam, int chance, bool save, Object_Flag res_flag, random_value power) {

			this.index = index;
			this.method = method;
			this.gf = gf;
			this.timed = timed;
			this.flag = flag;
			this.Base = Base;
			this.dam = dam;
			this.chance = chance;
			this.save = save;
			this.res_flag = res_flag;
			this.power = power;
		}

		public int index;             /* Numerical index (RAE_#) */
		public Monster_Spell_Flag method;/* What Monster_Spell_Flag. attack has this effect */
		public GF gf;                 /* What GF. type has this effect */
		public bool timed;             /* true if timed, false if permanent */
		public Timed_Effect flag;               /* Effect flag */
		public random_value Base;      /* The base duration or impact */
		public random_value dam;       /* Damage-dependent duration or impact */
		public int chance;             /* Chance of this effect if >1 available */
		public bool save;              /* Does this effect allow a saving throw? */
		public Object_Flag res_flag;   /* Resistance to this specific effect */
		public random_value power;		/* Power rating of effect */

		/* Fields:
		 * - numerical index (RSE_this)
		 * - which Monster_Spell_Flag. attack method has this effect
		 * - which GF. type has this side effect
		 * - is this effect temporary?
		 * - flag for the effect
		 * - basic duration (or impact if permanent)
		 * - damage-dependent duration (or impact)
		 * - chance of this effect arising from this attack (0 means always)
		 * - does the player get a save against this effect? (after any save against
		 *   the original attack)
		 * - what object flag resists this effect? (in addition to gf_ptr.resist etc.)
		 * - power rating of this side effect encoded as a random_value struct:
		 * 		base = flat power rating
		 * 		dice = percentage adjustment to damage output
		 *		sides = rlev-dependent power
		 *		m_bonus: +ve means power is proportional to rlev, negative means inverse
		 */

		/*																			   index method							gf			timed	effect															base						dam		 					chance	save 	res_flag 		power */
		public static Monster_Spell_Effect_Flag RSE_0 = new Monster_Spell_Effect_Flag(	0 ,	Monster_Spell_Flag.NONE,		GF.NONE,	false,	(Timed_Effect)0,												new random_value(0,0,0,0),	new random_value(0,0,0,0),	0,		false, 	Object_Flag.NONE,				new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_1 = new Monster_Spell_Effect_Flag(	1 ,	Monster_Spell_Flag.NONE,		GF.POIS,	true,	Timed_Effect.POISONED,											new random_value(10,0,0,0),	new random_value(0,1,100,0),0,		false,	Object_Flag.RES_POIS,	new random_value(0,125,100,1));
		public static Monster_Spell_Effect_Flag RSE_2 = new Monster_Spell_Effect_Flag(	2 ,	Monster_Spell_Flag.NONE,		GF.LIGHT,	true,	Timed_Effect.BLIND,												new random_value(2,1,5,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_BLIND,	new random_value(10,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_3 = new Monster_Spell_Effect_Flag(	3 ,	Monster_Spell_Flag.NONE,		GF.DARK,	true,	Timed_Effect.BLIND,												new random_value(2,1,5,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_BLIND,	new random_value(10,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_4 = new Monster_Spell_Effect_Flag(	4 ,	Monster_Spell_Flag.NONE,		GF.SOUND,	true,	Timed_Effect.STUN,												new random_value(5,0,0,0),	new random_value(0,1,33,35),0,		false,	Object_Flag.RES_STUN,	new random_value(20,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_5 = new Monster_Spell_Effect_Flag(	5 ,	Monster_Spell_Flag.NONE,		GF.SHARD,	true,	Timed_Effect.CUT,												new random_value(0,0,0,0),	new random_value(0,1,100,0),0,		false,	Object_Flag.NONE,				new random_value(5,125,0,0));
		public static Monster_Spell_Effect_Flag RSE_6 = new Monster_Spell_Effect_Flag(	6 ,	Monster_Spell_Flag.NONE,		GF.CHAOS,	true,	Timed_Effect.IMAGE,												new random_value(0,1,10,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_CHAOS,	new random_value(0,0,2000,-1));
		public static Monster_Spell_Effect_Flag RSE_7 = new Monster_Spell_Effect_Flag(	7 ,	Monster_Spell_Flag.NONE,		GF.CHAOS,	true,	Timed_Effect.CONFUSED,											new random_value(9,1,20,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_CONFU,	new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_8 = new Monster_Spell_Effect_Flag(	8 ,	Monster_Spell_Flag.NONE,		GF.INERTIA,	true,	Timed_Effect.SLOW,												new random_value(3,1,4,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(30,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_9 = new Monster_Spell_Effect_Flag(	9 ,	Monster_Spell_Flag.NONE,		GF.GRAVITY,	true,	Timed_Effect.STUN,												new random_value(5,0,0,0),	new random_value(0,1,33,35),0,		false,	Object_Flag.RES_STUN,	new random_value(30,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_10 = new Monster_Spell_Effect_Flag(10 ,	Monster_Spell_Flag.BRAIN_SMASH, GF.GRAVITY,	true,	Timed_Effect.SLOW,												new random_value(3,1,4,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_11 = new Monster_Spell_Effect_Flag(11 ,	Monster_Spell_Flag.NONE,		GF.FORCE,	true,	Timed_Effect.STUN,												new random_value(0,1,20,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_STUN,	new random_value(30,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_12 = new Monster_Spell_Effect_Flag(12 ,	Monster_Spell_Flag.NONE,		GF.PLASMA,	true,	Timed_Effect.STUN,												new random_value(5,0,0,0),	new random_value(0,1,75,35),0,		false,	Object_Flag.RES_STUN,	new random_value(30,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_13 = new Monster_Spell_Effect_Flag(13 ,	Monster_Spell_Flag.NONE,		GF.WATER,	true,	Timed_Effect.CONFUSED,											new random_value(5,1,5,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_CONFU,	new random_value(20,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_14 = new Monster_Spell_Effect_Flag(14 ,	Monster_Spell_Flag.NONE,		GF.WATER,	true,	Timed_Effect.STUN,												new random_value(0,1,40,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_STUN,	new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_15 = new Monster_Spell_Effect_Flag(15 ,	Monster_Spell_Flag.NONE,		GF.ICE,		true,	Timed_Effect.CUT,												new random_value(0,5,8,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_SHARD,	new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_16 = new Monster_Spell_Effect_Flag(16 ,	Monster_Spell_Flag.NONE,		GF.ICE,		true,	Timed_Effect.STUN,												new random_value(0,1,15,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_STUN,	new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_17 = new Monster_Spell_Effect_Flag(17 ,	Monster_Spell_Flag.CAUSE_4,		GF.NONE,	true,	Timed_Effect.CUT,												new random_value(0,10,10,0),new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_18 = new Monster_Spell_Effect_Flag(18 ,	Monster_Spell_Flag.MIND_BLAST,	GF.NONE,	true,	Timed_Effect.CONFUSED,											new random_value(3,1,4,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_CONFU,	new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_19 = new Monster_Spell_Effect_Flag(19 ,	Monster_Spell_Flag.NONE,		GF.ACID,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_INV_DAM,	new random_value(0,0,0,0),	new random_value(5,0,0,0),	0,		false,	Object_Flag.IM_ACID,		new random_value(20,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_20 = new Monster_Spell_Effect_Flag(20 ,	Monster_Spell_Flag.NONE,		GF.ELEC,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_INV_DAM,	new random_value(0,0,0,0),	new random_value(5,0,0,0),	0,		false,	Object_Flag.IM_ELEC,		new random_value(10,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_21 = new Monster_Spell_Effect_Flag(21 ,	Monster_Spell_Flag.NONE,		GF.FIRE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_INV_DAM,	new random_value(0,0,0,0),	new random_value(5,0,0,0),	0,		false,	Object_Flag.IM_FIRE,		new random_value(10,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_22 = new Monster_Spell_Effect_Flag(22 ,	Monster_Spell_Flag.NONE,		GF.COLD,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_INV_DAM,	new random_value(0,0,0,0),	new random_value(5,0,0,0),	0,		false,	Object_Flag.IM_COLD,		new random_value(10,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_23 = new Monster_Spell_Effect_Flag(23 ,	Monster_Spell_Flag.NONE,		GF.NEXUS,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_TELEPORT,	new random_value(200,0,0,0),new random_value(0,0,0,0),	3,		false,	Object_Flag.RES_NEXUS,	new random_value(20,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_24 = new Monster_Spell_Effect_Flag(24 ,	Monster_Spell_Flag.NONE,		GF.NEXUS,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_TELE_TO,	new random_value(0,0,0,0),	new random_value(0,0,0,0),	2,		false,	Object_Flag.RES_NEXUS,	new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_25 = new Monster_Spell_Effect_Flag(25 ,	Monster_Spell_Flag.NONE,		GF.NEXUS,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_TELE_LEV,	new random_value(1,0,0,0),	new random_value(0,0,0,0),	1,		true,	Object_Flag.RES_NEXUS,	new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_26 = new Monster_Spell_Effect_Flag(26 ,	Monster_Spell_Flag.NONE,		GF.NEXUS,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_SWAP_STAT,	new random_value(1,0,0,0),	new random_value(0,0,0,0),	1,		true,	Object_Flag.RES_NEXUS,	new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_27 = new Monster_Spell_Effect_Flag(27 ,	Monster_Spell_Flag.NONE,		GF.NETHER,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_DRAIN_LIFE,new random_value(200,0,1,0),new random_value(0,0,0,0),	0,		false,	Object_Flag.HOLD_LIFE,	new random_value(0,0,2000,-1));
		public static Monster_Spell_Effect_Flag RSE_28 = new Monster_Spell_Effect_Flag(28 ,	Monster_Spell_Flag.NONE,		GF.CHAOS,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_DRAIN_LIFE,new random_value(5000,0,1,0),new random_value(0,0,0,0),	0,		false,	Object_Flag.HOLD_LIFE,	new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_29 = new Monster_Spell_Effect_Flag(29 ,	Monster_Spell_Flag.NONE,		GF.DISEN,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_DISEN,		new random_value(1,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_DISEN,	new random_value(50,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_30 = new Monster_Spell_Effect_Flag(30 ,	Monster_Spell_Flag.NONE,		GF.GRAVITY,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_TELEPORT,	new random_value(5,0,0,0),	new random_value(0,0,0,127),0,		false,	Object_Flag.NONE,				new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_31 = new Monster_Spell_Effect_Flag(31 ,	Monster_Spell_Flag.NONE,		GF.TIME,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_DRAIN_LIFE,new random_value(100,0,1,0),new random_value(0,0,0,0),	5,		false,	Object_Flag.NONE,				new random_value(0,0,2000,-1));
		public static Monster_Spell_Effect_Flag RSE_32 = new Monster_Spell_Effect_Flag(32 ,	Monster_Spell_Flag.NONE,		GF.TIME,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_DRAIN_STAT,new random_value(2,0,0,0),	new random_value(0,0,0,0),	4,		false,	Object_Flag.NONE,				new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_33 = new Monster_Spell_Effect_Flag(33 ,	Monster_Spell_Flag.NONE,		GF.TIME,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_DRAIN_ALL,	new random_value(1,0,0,0),	new random_value(0,0,0,0),	1,		false,	Object_Flag.NONE,				new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_34 = new Monster_Spell_Effect_Flag(34 ,	Monster_Spell_Flag.BRAIN_SMASH,	GF.NONE,	true,	Timed_Effect.BLIND,												new random_value(7,1,8,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_BLIND,	new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_35 = new Monster_Spell_Effect_Flag(35 ,	Monster_Spell_Flag.BRAIN_SMASH,	GF.NONE,	true,	Timed_Effect.CONFUSED,											new random_value(3,1,4,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_CONFU,	new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_36 = new Monster_Spell_Effect_Flag(36 ,	Monster_Spell_Flag.BRAIN_SMASH,	GF.NONE,	true,	Timed_Effect.PARALYZED,											new random_value(3,1,4,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.FREE_ACT,	new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_37 = new Monster_Spell_Effect_Flag(37 ,	Monster_Spell_Flag.NONE,		GF.ICE,		false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_INV_DAM,	new random_value(0,0,0,0),	new random_value(5,0,0,0),	0,		false,	Object_Flag.IM_COLD,		new random_value(0,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_38 = new Monster_Spell_Effect_Flag(38 ,	Monster_Spell_Flag.NONE,		GF.MANA,	false,	(Timed_Effect)0,												new random_value(0,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(100,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_39 = new Monster_Spell_Effect_Flag(39 ,	Monster_Spell_Flag.DRAIN_MANA,	GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_DRAIN_MANA,new random_value(0,0,0,0),	new random_value(0,1,50,0),	0,		false,	Object_Flag.NONE,				new random_value(5,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_40 = new Monster_Spell_Effect_Flag(40 ,	Monster_Spell_Flag.SCARE,		GF.NONE,	true,	Timed_Effect.AFRAID,											new random_value(3,1,4,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_FEAR,	new random_value(5,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_41 = new Monster_Spell_Effect_Flag(41 ,	Monster_Spell_Flag.CONF,		GF.NONE,	true,	Timed_Effect.CONFUSED,											new random_value(3,1,4,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_CONFU,	new random_value(10,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_42 = new Monster_Spell_Effect_Flag(42 ,	Monster_Spell_Flag.BLIND,		GF.NONE,	true,	Timed_Effect.BLIND,												new random_value(11,1,4,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_BLIND,	new random_value(10,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_43 = new Monster_Spell_Effect_Flag(43 ,	Monster_Spell_Flag.SLOW,		GF.NONE,	true,	Timed_Effect.SLOW,												new random_value(3,1,4,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.FREE_ACT,	new random_value(15,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_44 = new Monster_Spell_Effect_Flag(44 ,	Monster_Spell_Flag.HOLD,		GF.NONE,	true,	Timed_Effect.PARALYZED,											new random_value(3,1,4,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.FREE_ACT,	new random_value(25,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_45 = new Monster_Spell_Effect_Flag(45 ,	Monster_Spell_Flag.HASTE,		GF.NONE,	true,	Timed_Effect.FAST,												new random_value(50,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(70,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_46 = new Monster_Spell_Effect_Flag(46 , Monster_Spell_Flag.HEAL,		GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_HEAL,		new random_value(0,0,0,0),	new random_value(6,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(30,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_47 = new Monster_Spell_Effect_Flag(47 ,	Monster_Spell_Flag.BLINK,		GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_TELE_SELF,	new random_value(10,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(15,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_48 = new Monster_Spell_Effect_Flag(48 ,	Monster_Spell_Flag.TELE_TO,		GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_TELE_TO,	new random_value(0,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(25,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_49 = new Monster_Spell_Effect_Flag(49 ,	Monster_Spell_Flag.TELE_AWAY,	GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_TELEPORT,	new random_value(100,0,0,0),new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(25,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_50 = new Monster_Spell_Effect_Flag(50 ,	Monster_Spell_Flag.TELE_LEVEL,	GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_TELE_LEV,	new random_value(1,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.RES_NEXUS,	new random_value(40,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_51 = new Monster_Spell_Effect_Flag(51 ,	Monster_Spell_Flag.TPORT,		GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_TELE_SELF,	new random_value((Misc.MAX_SIGHT * 2 + 5),0,0,0), new random_value(0,0,0,0), 0, false, Object_Flag.NONE,		new random_value(15,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_52 = new Monster_Spell_Effect_Flag(52 ,	Monster_Spell_Flag.DARKNESS,	GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_DARKEN,	new random_value(3,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(5,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_53 = new Monster_Spell_Effect_Flag(53 ,	Monster_Spell_Flag.TRAPS,		GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_TRAPS,		new random_value(0,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(10,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_54 = new Monster_Spell_Effect_Flag(54 ,	Monster_Spell_Flag.FORGET,		GF.NONE,	true,	Timed_Effect.AMNESIA,											new random_value(3,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(25,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_55 = new Monster_Spell_Effect_Flag(55 ,	Monster_Spell_Flag.SHRIEK,		GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_AGGRAVATE,	new random_value(0,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(0,0,150,1));
		public static Monster_Spell_Effect_Flag RSE_56 = new Monster_Spell_Effect_Flag(56 ,	Monster_Spell_Flag.S_KIN,		GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_KIN,		new random_value(6,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(0,0,200,1));
		public static Monster_Spell_Effect_Flag RSE_57 = new Monster_Spell_Effect_Flag(57 ,	Monster_Spell_Flag.S_MONSTER,	GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_MONSTER,	new random_value(1,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(40,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_58 = new Monster_Spell_Effect_Flag(58 ,	Monster_Spell_Flag.S_MONSTERS,	GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_MONSTERS,	new random_value(8,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(80,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_59 = new Monster_Spell_Effect_Flag(59 ,	Monster_Spell_Flag.S_ANIMAL,	GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_ANIMAL,	new random_value(6,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(30,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_60 = new Monster_Spell_Effect_Flag(60 ,	Monster_Spell_Flag.S_SPIDER,	GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_SPIDER,	new random_value(6,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(20,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_61 = new Monster_Spell_Effect_Flag(61 ,	Monster_Spell_Flag.S_HOUND,		GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_HOUND,		new random_value(6,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(100,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_62 = new Monster_Spell_Effect_Flag(62 ,	Monster_Spell_Flag.S_HYDRA,		GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_HYDRA,		new random_value(6,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(150,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_63 = new Monster_Spell_Effect_Flag(63 ,	Monster_Spell_Flag.S_ANGEL,		GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_ANGEL,		new random_value(1,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(150,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_64 = new Monster_Spell_Effect_Flag(64 ,	Monster_Spell_Flag.S_DEMON,		GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_DEMON,		new random_value(1,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(0,0,150,1));
		public static Monster_Spell_Effect_Flag RSE_65 = new Monster_Spell_Effect_Flag(65 ,	Monster_Spell_Flag.S_UNDEAD,	GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_UNDEAD,	new random_value(1,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(0,0,150,1));
		public static Monster_Spell_Effect_Flag RSE_66 = new Monster_Spell_Effect_Flag(66 ,	Monster_Spell_Flag.S_DRAGON,	GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_DRAGON,	new random_value(1,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(0,0,150,1));
		public static Monster_Spell_Effect_Flag RSE_67 = new Monster_Spell_Effect_Flag(67 ,	Monster_Spell_Flag.S_HI_DEMON,	GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_HI_DEMON,	new random_value(8,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(250,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_68 = new Monster_Spell_Effect_Flag(68 ,	Monster_Spell_Flag.S_HI_UNDEAD,	GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_HI_UNDEAD,	new random_value(8,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(400,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_69 = new Monster_Spell_Effect_Flag(69 ,	Monster_Spell_Flag.S_HI_DRAGON,	GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_HI_DRAGON,	new random_value(8,0,0,0),	new random_value(0,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(400,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_70 = new Monster_Spell_Effect_Flag(70 ,	Monster_Spell_Flag.S_WRAITH,	GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_WRAITH,	new random_value(8,0,0,0),	new random_value(8,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(450,0,0,0));
		public static Monster_Spell_Effect_Flag RSE_71 = new Monster_Spell_Effect_Flag(71 ,	Monster_Spell_Flag.S_UNIQUE,	GF.NONE,	false,	(Timed_Effect)Monster_Spell_Flag.spell_effect_flag.S_UNIQUE,	new random_value(8,0,0,0),	new random_value(8,0,0,0),	0,		false,	Object_Flag.NONE,				new random_value(500,0,0,0));
		public static Monster_Spell_Effect_Flag MAX = new Monster_Spell_Effect_Flag(   72 , Monster_Spell_Flag.NONE,		GF.NONE,	false,	(Timed_Effect)0,												new random_value(0,0,0,0),	new random_value(0,0,0,0),	0,		false, 	Object_Flag.NONE,				new random_value(0,0,0,0));
	}
}
