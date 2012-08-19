using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;
using CSAngband.Monster;
namespace CSAngband {
	class GF {

		int value;
		private static int counter = 0;
		public GF(string name, string desc, Object_Flag resist, int num, random_value denom, Timed_Effect opp,
				Object_Flag immunity, bool side_immune, Object_Flag vuln, Monster_Flag mon_res, Monster_Flag mon_vuln,
				Object_Flag obj_hates, Object_Flag obj_imm){

			value = counter++;
			this.name = name;
			this.desc = desc;
			this.resist = resist;
			this.num = num;
			this.denom = denom;
			this.opp = opp;
			this.immunity = immunity;
			this.side_immune = side_immune;
			this.vuln = vuln;
			this.mon_res = mon_res;
			this.mon_vuln = mon_vuln;
			this.obj_hates = obj_hates;
			this.obj_imm = obj_imm;
		}

		public string name;
		public string desc;
		public Object_Flag resist;
		public int num;
		public random_value denom;
		public Timed_Effect opp;
		public Object_Flag immunity;
		public bool side_immune;
		public Object_Flag vuln;
		public Monster_Flag mon_res;
		public Monster_Flag mon_vuln;
		public Object_Flag obj_hates;
		public Object_Flag obj_imm;

		/* Fields:
		 * name - type index (GF_THIS) 
		 * desc - text description of attack if blind
		 * resist - object flag for resistance
		 * num - numerator for resistance
		 * denom - denominator for resistance (random_value)
		 * opp - timed flag for temporary resistance ("opposition")
		 * immunity - object flag for total immunity
		 * side_immune - whether immunity protects from *all* side-effects
		 * vuln - object flag for vulnerability
		 * mon_res - monster flag for resistance
		 * mon_vuln - monster flag for vulnerability
		 * obj_hates - object flag for object vulnerability
		 * obj_imm - object flag for object immunity
		 */

		/*										name  			desc				resist					num			denom						opp						immunity				side_im		vuln					mon_resist				mon_vuln				obj_hates				obj_imm */
		public static GF NONE  =		new GF("NONE",			"ERROR",			Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF ARROW =		new GF("ARROW" ,		"something sharp",	Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF MISSILE =		new GF("MISSILE" ,		"something",		Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF MANA =			new GF("MANA" ,			"something",		Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF HOLY_ORB =		new GF("HOLY_ORB" ,		"something",		Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF LIGHT_WEAK =	new GF("LIGHT_WEAK" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.HURT_LIGHT,Object_Flag.NONE,		Object_Flag.NONE);
		public static GF DARK_WEAK =	new GF("DARK_WEAK" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF WATER =		new GF("WATER" ,		"water",			Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.IM_WATER,	Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF PLASMA =		new GF("PLASMA" ,		"something",		Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.RES_PLAS,	Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF METEOR =		new GF("METEOR" ,		"something",		Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF ICE =			new GF("ICE" ,			"something sharp",	Object_Flag.RES_COLD,   1,			new random_value(3,0,0,0),  Timed_Effect.OPP_COLD,  Object_Flag.IM_COLD,	false,		Object_Flag.VULN_COLD,  Monster_Flag.IM_COLD,	Monster_Flag.HURT_COLD,	Object_Flag.HATES_COLD,	Object_Flag.IGNORE_COLD);
		public static GF GRAVITY =		new GF("GRAVITY" ,		"something strange",Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF INERTIA =		new GF("INERTIA" ,		"something strange",Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF FORCE =		new GF("FORCE" ,		"something hard",	Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF TIME =			new GF("TIME" ,			"something strange",Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF ACID =			new GF("ACID" ,			"acid",				Object_Flag.RES_ACID,   1,			new random_value(3,0,0,0),  Timed_Effect.OPP_ACID,  Object_Flag.IM_ACID,	true,		Object_Flag.VULN_ACID,  Monster_Flag.IM_ACID,	Monster_Flag.NONE,		Object_Flag.HATES_ACID,	Object_Flag.IGNORE_ACID);
		public static GF ELEC =			new GF("ELEC" ,			"lightning",		Object_Flag.RES_ELEC,   1,			new random_value(3,0,0,0),  Timed_Effect.OPP_ELEC,  Object_Flag.IM_ELEC,	true,		Object_Flag.VULN_ELEC,  Monster_Flag.IM_ELEC,	Monster_Flag.NONE,		Object_Flag.HATES_ELEC,	Object_Flag.IGNORE_ELEC);
		public static GF FIRE =			new GF("FIRE" ,			"fire",				Object_Flag.RES_FIRE,   1,			new random_value(3,0,0,0),  Timed_Effect.OPP_FIRE,  Object_Flag.IM_FIRE,	true,		Object_Flag.VULN_FIRE,  Monster_Flag.IM_FIRE,	Monster_Flag.HURT_FIRE,	Object_Flag.HATES_FIRE,	Object_Flag.IGNORE_FIRE);
		public static GF COLD =			new GF("COLD" ,			"cold",				Object_Flag.RES_COLD,   1,			new random_value(3,0,0,0),  Timed_Effect.OPP_COLD,  Object_Flag.IM_COLD,	true,		Object_Flag.VULN_COLD,  Monster_Flag.IM_COLD,	Monster_Flag.HURT_COLD,	Object_Flag.HATES_COLD,	Object_Flag.IGNORE_COLD);
		public static GF POIS =			new GF("POIS" ,			"poison",			Object_Flag.RES_POIS,   1,			new random_value(3,0,0,0),  Timed_Effect.OPP_POIS,  Object_Flag.NONE,       true,		Object_Flag.NONE,       Monster_Flag.IM_POIS,	Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF LIGHT =		new GF("LIGHT" ,		"something",		Object_Flag.RES_LIGHT,  4,			new random_value(6,1,6,0),  Timed_Effect.FAST,      Object_Flag.NONE,       true,		Object_Flag.NONE,       Monster_Flag.NONE,      Monster_Flag.HURT_LIGHT,Object_Flag.NONE,		Object_Flag.NONE);
		public static GF DARK =			new GF("DARK" ,			"something",		Object_Flag.RES_DARK,   4,			new random_value(6,1,6,0),  Timed_Effect.FAST,      Object_Flag.NONE,       true,		Object_Flag.NONE,       Monster_Flag.NONE,      Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF CONFU =		new GF("CONFU" ,		"something",		Object_Flag.RES_CONFU,	6,			new random_value(6,1,6,0),	Timed_Effect.OPP_CONF,	Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF SOUND =		new GF("SOUND" ,		"noise",			Object_Flag.RES_SOUND,  5,			new random_value(6,1,6,0),  Timed_Effect.FAST,      Object_Flag.NONE,       true,		Object_Flag.NONE,       Monster_Flag.NONE,      Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF SHARD =		new GF("SHARD" ,		"something sharp",	Object_Flag.RES_SHARD,  6,			new random_value(6,1,6,0),  Timed_Effect.FAST,      Object_Flag.NONE,       true,		Object_Flag.NONE,       Monster_Flag.NONE,      Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF NEXUS =		new GF("NEXUS" ,		"something strange",Object_Flag.RES_NEXUS,  6,			new random_value(6,1,6,0),  Timed_Effect.FAST,      Object_Flag.NONE,       true,		Object_Flag.NONE,       Monster_Flag.RES_NEXUS,	Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF NETHER =		new GF("NETHER" ,		"something cold",	Object_Flag.RES_NETHR,  6,			new random_value(6,1,6,0),  Timed_Effect.FAST,      Object_Flag.NONE,       true,		Object_Flag.NONE,       Monster_Flag.RES_NETH,	Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF CHAOS =		new GF("CHAOS" ,		"something strange",Object_Flag.RES_CHAOS,	6,			new random_value(6,1,6,0), 	Timed_Effect.FAST,      Object_Flag.NONE,       true,		Object_Flag.NONE,       Monster_Flag.NONE,      Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF DISEN =		new GF("DISEN" ,		"something strange",Object_Flag.RES_DISEN,  6,			new random_value(6,1,6,0),  Timed_Effect.FAST,      Object_Flag.NONE,       true,		Object_Flag.NONE,       Monster_Flag.RES_DISE,	Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF KILL_WALL =	new GF("KILL_WALL" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF KILL_DOOR =	new GF("KILL_DOOR" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF KILL_TRAP =	new GF("KILL_TRAP" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF MAKE_WALL =	new GF("MAKE_WALL" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF MAKE_DOOR =	new GF("MAKE_DOOR" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF MAKE_TRAP =	new GF("MAKE_TRAP" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF AWAY_UNDEAD =	new GF("AWAY_UNDEAD" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF AWAY_EVIL =	new GF("AWAY_EVIL" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF AWAY_ALL =		new GF("AWAY_ALL" ,		null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF TURN_UNDEAD =	new GF("TURN_UNDEAD" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF TURN_EVIL =	new GF("TURN_EVIL" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF TURN_ALL =		new GF("TURN_ALL" ,		null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF DISP_UNDEAD =	new GF("DISP_UNDEAD" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF DISP_EVIL =	new GF("DISP_EVIL" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF DISP_ALL =		new GF("DISP_ALL" ,		null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF OLD_CLONE =	new GF("OLD_CLONE" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF OLD_POLY =		new GF("OLD_POLY" ,		null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF OLD_HEAL =		new GF("OLD_HEAL" ,		null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF OLD_SPEED =	new GF("OLD_SPEED" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF OLD_SLOW =		new GF("OLD_SLOW" ,		null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF OLD_CONF =		new GF("OLD_CONF" ,		null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF OLD_SLEEP =	new GF("OLD_SLEEP" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF OLD_DRAIN =	new GF("OLD_DRAIN" ,	null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		true,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
		public static GF MAX =			new GF("MAX" ,			null,				Object_Flag.NONE,		0,			new random_value(0,0,0,0),	Timed_Effect.FAST,		Object_Flag.NONE,		false,		Object_Flag.NONE,		Monster_Flag.NONE,		Monster_Flag.NONE,		Object_Flag.NONE,		Object_Flag.NONE);
	}
}
