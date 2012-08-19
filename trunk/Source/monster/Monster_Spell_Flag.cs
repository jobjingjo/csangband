using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Monster {
	class Monster_Spell_Flag {
		public static List<Monster_Spell_Flag> list = new List<Monster_Spell_Flag>();

		public int value;              /* Numerical index (RSF_FOO) */

		private static int counter = 0;
		public Monster_Spell_Flag(string name, mon_spell_type type, string desc, int cap, int div, GF gf,
			/*Message_Type msgt,*/ bool save, int hit, string verb, random_value base_dam, random_value rlev_dam, string blind_verb){

			value = counter++;

			this.name = name;
			this.type = type;
			this.desc = desc;
			this.cap = cap;
			this.div = div;
			this.gf = gf;
			//this.msgt = msgt;
			this.save = save;
			this.hit = hit;
			this.verb = verb;
			this.base_dam = base_dam;
			this.rlev_dam = rlev_dam;
			this.blind_verb = blind_verb;

			list.Add(this);
		}

		public string name;			//Might need this... right?
		public mon_spell_type type;    /* Type bitflag */
		public string desc;			/* Verbal description */
		public int cap;                /* Damage cap */
		public int div;                /* Damage divisor (monhp / this) */
		public GF gf;                 /* Flag for projection type (GF.FOO) */
		//public Message_Type msgt;               /* Flag for message colouring */
		public bool save;              /* Does this attack allow a saving throw? */
		public int hit;                /* To-hit level for the attack */
		public string verb;			/* Description of the attack */
		public random_value base_dam;  /* Base damage for the attack */
		public random_value rlev_dam;  /* Monster-level-dependent damage */
		public string blind_verb;		/* Description of the attack if unseen */

		/* Fields:
		 * name - spell name
		 * type - spell type
		 * desc - textual description
		 * cap - damage cap
		 * div - damage divisor (monster hp / this)
		 * gf - projection type for project() functions
		 * msgt - flag for message colouring
		 * save - does this spell allow a saving throw?
		 * hit - to-hit chance (100 = always, 0 = never)
		 * verb - description of attack
		 * base_dam - raw damage (random_value struct)
		 * rlev_dam - monster-level-dependent damage (random_value)
		 * blind_verb - description of unseen attacks
		 */
		
		/* 	name		type		desc			  cap	div	gf			msgt			save	hit		verb					base_dam		rlev_dam		blind_verb */
		public static Monster_Spell_Flag NONE =		new Monster_Spell_Flag("NONE" ,		(mon_spell_type)0,			null,			    0,	0, 	GF.NONE,							false,	0,		null,					new random_value(0,0,0,0),	new random_value(0,0,0,0),	null);
		public static Monster_Spell_Flag SHRIEK =	new Monster_Spell_Flag("SHRIEK" ,	mon_spell_type.RST_ANNOY,	"shriek",		    0,	0, 	GF.NONE,					false,	100,	"makes a high-pitched",	new random_value(0,0,0,0),	new random_value(0,0,0,0),	"shrieks");
		public static Monster_Spell_Flag XXX1 =		new Monster_Spell_Flag("XXX1" ,		(mon_spell_type)0,			null,				0,	0,	GF.NONE,							false,	0,		null,					new random_value(0,0,0,0),	new random_value(0,0,0,0),	null);
		public static Monster_Spell_Flag XXX2 =		new Monster_Spell_Flag("XXX2" ,		(mon_spell_type)0,			null,				0,	0,	GF.NONE,							false,	0,		null,					new random_value(0,0,0,0),	new random_value(0,0,0,0),	null);
		public static Monster_Spell_Flag XXX3 =		new Monster_Spell_Flag("XXX3" ,		(mon_spell_type)0,			null,				0,	0,	GF.NONE,							false,	0,		null,					new random_value(0,0,0,0),	new random_value(0,0,0,0),	null);
		public static Monster_Spell_Flag ARROW_1 =	new Monster_Spell_Flag("ARROW_1" ,	mon_spell_type.RST_BOLT,	"arrow",			0,	0,	GF.ARROW,			false,	40,		"fires an",				new random_value(0,1,6,0),	new random_value(0,0,0,0),	"makes a strange noise");
		public static Monster_Spell_Flag ARROW_2 =	new Monster_Spell_Flag("ARROW_2" ,	mon_spell_type.RST_BOLT,	"arrow",			0,	0,	GF.ARROW,					false,	40,		"fires an",				new random_value(0,3,6,0),	new random_value(0,0,0,0),	"makes a strange noise");
		public static Monster_Spell_Flag ARROW_3 =	new Monster_Spell_Flag("ARROW_3" ,	mon_spell_type.RST_BOLT,	"missile",			0,	0,	GF.ARROW,					false,	50,		"fires a",				new random_value(0,5,6,0),	new random_value(0,0,0,0),	"makes a strange noise");
		public static Monster_Spell_Flag ARROW_4 =	new Monster_Spell_Flag("ARROW_4" ,	mon_spell_type.RST_BOLT,	"missile",			0,	0,	GF.ARROW,					false,	50,		"fires a",				new random_value(0,7,6,0),	new random_value(0,0,0,0),	"makes a strange noise");
		public static Monster_Spell_Flag BR_ACID =	new Monster_Spell_Flag("BR_ACID" ,	mon_spell_type.RST_BREATH,	"acid",			 1600,	3, 	GF.ACID,		false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"hisses");
		public static Monster_Spell_Flag BR_ELEC =	new Monster_Spell_Flag("BR_ELEC" ,	mon_spell_type.RST_BREATH,	"lightning",	 1600,	3,	GF.ELEC,		false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"crackles");
		public static Monster_Spell_Flag BR_FIRE =	new Monster_Spell_Flag("BR_FIRE" ,	mon_spell_type.RST_BREATH,	"fire",			 1600,	3,	GF.FIRE,	false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"roars");
		public static Monster_Spell_Flag BR_COLD =	new Monster_Spell_Flag("BR_COLD" ,	mon_spell_type.RST_BREATH,	"frost",		 1600,	3,	GF.COLD,	false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"wooshes");
		public static Monster_Spell_Flag BR_POIS =	new Monster_Spell_Flag("BR_POIS" ,	mon_spell_type.RST_BREATH,	"poison",		  800,	3,	GF.POIS,		false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"retches");
		public static Monster_Spell_Flag BR_NETH =	new Monster_Spell_Flag("BR_NETH" ,	mon_spell_type.RST_BREATH,	"nether",		  550,	6,	GF.NETHER,		false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"groans");
		public static Monster_Spell_Flag BR_LIGHT = new Monster_Spell_Flag("BR_LIGHT" ,	mon_spell_type.RST_BREATH,	"light",		  400,	6,	GF.LIGHT,		false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"breathes");
		public static Monster_Spell_Flag BR_DARK =	new Monster_Spell_Flag("BR_DARK" ,	mon_spell_type.RST_BREATH,	"darkness",		  400,	6,	GF.DARK,		false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"breathes");
		public static Monster_Spell_Flag BR_CONF =	new Monster_Spell_Flag("BR_CONF" ,	mon_spell_type.RST_BREATH,	"confusion",		0,	0,	GF.CONFU,	false,	0,		"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	null);
		public static Monster_Spell_Flag BR_SOUN =	new Monster_Spell_Flag("BR_SOUN" ,	mon_spell_type.RST_BREATH,	"sound",		  500,	6, 	GF.SOUND,	false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"breathes");
		public static Monster_Spell_Flag BR_CHAO =	new Monster_Spell_Flag("BR_CHAO" ,	mon_spell_type.RST_BREATH,	"chaos", 		  500,	6,	GF.CHAOS,		false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"breathes");
		public static Monster_Spell_Flag BR_DISE =	new Monster_Spell_Flag("BR_DISE" ,	mon_spell_type.RST_BREATH,	"disenchantment", 500,	6,	GF.DISEN,		false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"breathes");
		public static Monster_Spell_Flag BR_NEXU =	new Monster_Spell_Flag("BR_NEXU" ,	mon_spell_type.RST_BREATH,	"nexus",	 	  400,	6,	GF.NEXUS,		false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"breathes");
		public static Monster_Spell_Flag BR_TIME =	new Monster_Spell_Flag("BR_TIME" ,	mon_spell_type.RST_BREATH,	"time",		 	  150,	3,	GF.TIME,		false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"breathes");
		public static Monster_Spell_Flag BR_INER =	new Monster_Spell_Flag("BR_INER" ,	mon_spell_type.RST_BREATH,	"inertia",	 	  200,	6,	GF.INERTIA,	false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"breathes");
		public static Monster_Spell_Flag BR_GRAV =	new Monster_Spell_Flag("BR_GRAV" ,	mon_spell_type.RST_BREATH,	"gravity",	 	  200,	3,	GF.GRAVITY,		false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"breathes");
		public static Monster_Spell_Flag BR_SHAR =	new Monster_Spell_Flag("BR_SHAR" ,	mon_spell_type.RST_BREATH,	"shards",	 	  500,	6,	GF.SHARD,		false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"breathes");
		public static Monster_Spell_Flag BR_PLAS =	new Monster_Spell_Flag("BR_PLAS" ,	mon_spell_type.RST_BREATH,	"hellfire",	 	  150,	6,	GF.PLASMA,		false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"breathes");
		public static Monster_Spell_Flag BR_WALL =	new Monster_Spell_Flag("BR_WALL" ,	mon_spell_type.RST_BREATH,	"force",	 	  200,	6,	GF.FORCE,		false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"breathes");
		public static Monster_Spell_Flag BR_MANA =	new Monster_Spell_Flag("BR_MANA" ,	mon_spell_type.RST_BREATH,	"raw magic",	 1600,	3,	GF.MANA,					false,	100,	"breathes",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"breathes");
		public static Monster_Spell_Flag XXX4 =		new Monster_Spell_Flag("XXX4" ,		0,							null,				0,	0,	GF.NONE,				false,	0,		null,					new random_value(0,0,0,0),	new random_value(0,0,0,0),	null);
		public static Monster_Spell_Flag XXX5 =		new Monster_Spell_Flag("XXX5" ,		0,							null,				0,	0,	GF.NONE,					false,	0,		null,					new random_value(0,0,0,0),	new random_value(0,0,0,0),	null);
		public static Monster_Spell_Flag XXX6 =		new Monster_Spell_Flag("XXX6" ,		0,							null,				0,	0,	GF.NONE,				false,	0,		null,					new random_value(0,0,0,0),	new random_value(0,0,0,0),	null);
		public static Monster_Spell_Flag BOULDER =	new Monster_Spell_Flag("BOULDER" ,	mon_spell_type.RST_BOLT,	"boulder",	   		0,	0,	GF.ARROW,				false,	60,		"hurls a",				new random_value(0,1,12,0),	new random_value(0,14,12,1),	"grunts with exertion");
		public static Monster_Spell_Flag BA_ACID =	new Monster_Spell_Flag("BA_ACID" ,	mon_spell_type.RST_BALL,	"acid",				0,	0,	GF.ACID,				false,	100,	"casts a ball of",		new random_value(15,0,0,0),	new random_value(0,1,300,0),	"mumbles");
		public static Monster_Spell_Flag BA_ELEC =	new Monster_Spell_Flag("BA_ELEC" ,	mon_spell_type.RST_BALL,	"lightning",		0,	0,	GF.ELEC,				false,	100,	"casts a ball of",		new random_value(8,0,0,0),	new random_value(0,1,150,0),	"mumbles");
		public static Monster_Spell_Flag BA_FIRE =	new Monster_Spell_Flag("BA_FIRE" ,	mon_spell_type.RST_BALL,	"fire",				0,	0,	GF.FIRE,				false,	100,	"casts a ball of",		new random_value(10,0,0,0),	new random_value(0,1,350,0),	"mumbles");
		public static Monster_Spell_Flag BA_COLD =	new Monster_Spell_Flag("BA_COLD" ,	mon_spell_type.RST_BALL,	"frost",			0,	0,	GF.COLD,				false,	100,	"casts a ball of",		new random_value(10,0,0,0),	new random_value(0,1,150,0),	"mumbles");
		public static Monster_Spell_Flag BA_POIS =	new Monster_Spell_Flag("BA_POIS" ,	mon_spell_type.RST_BALL,	"poison",			0,	0,	GF.POIS,			false,	100,	"creates a cloud of",	new random_value(0,12,2,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag BA_NETH =	new Monster_Spell_Flag("BA_NETH" ,	mon_spell_type.RST_BALL,	"nether",			0,	0,	GF.NETHER,				false,	100,	"casts a ball of",		new random_value(50,10,10,0),	new random_value(100,0,0,0),	"mumbles");
		public static Monster_Spell_Flag BA_WATE =	new Monster_Spell_Flag("BA_WATE" ,	mon_spell_type.RST_BALL,	"water",	   		0,	0,	GF.WATER,				false,	100,	"creates a whirlpool of",new random_value(50,0,0,0),	new random_value(0,1,250,0),	"gurgles");
		public static Monster_Spell_Flag BA_MANA =	new Monster_Spell_Flag("BA_MANA" ,	mon_spell_type.RST_BALL,	"raw magic",		0,	0,	GF.MANA,				false,	100,	"invokes a storm of", 	new random_value(0,10,10,0),	new random_value(500,0,0,0),	"screams loudly");
		public static Monster_Spell_Flag BA_DARK =	new Monster_Spell_Flag("BA_DARK" ,	mon_spell_type.RST_BALL,	"darkness",			0,	0,	GF.DARK,				false,	100,	"invokes a storm of",	new random_value(0,10,10,0),	new random_value(500,0,0,0),	"mumbles loudly");
		public static Monster_Spell_Flag DRAIN_MANA=new Monster_Spell_Flag("DRAIN_MANA" ,	mon_spell_type.RST_ANNOY,"mana away",		0,	0,	GF.NONE,					false,	100,	"drains your",			new random_value(0,0,0,0),	new random_value(0,0,0,0),	"moans");
		public static Monster_Spell_Flag MIND_BLAST=new Monster_Spell_Flag("MIND_BLAST" ,	mon_spell_type.RST_ATTACK | mon_spell_type.RST_ANNOY,
																													  "psionic energy",	0,	0,	GF.NONE,					true,	100,	"gazes at you with",	new random_value(0,8,8,0),	new random_value(0,0,0,0),	"focuses on your mind");
		public static Monster_Spell_Flag BRAIN_SMASH=new Monster_Spell_Flag("BRAIN_SMASH" ,mon_spell_type.RST_ATTACK | mon_spell_type.RST_ANNOY,
																													"psionic energy",	0,	0,	GF.NONE,					true,	100,	"smashes you with",		new random_value(0,12,15,0),	new random_value(0,0,0,0),	"focuses on your mind");
		public static Monster_Spell_Flag CAUSE_1 =	new Monster_Spell_Flag("CAUSE_1" ,	mon_spell_type.RST_ATTACK,	"curses",	   		0,	0,	GF.NONE,						true,	100,	"points at you and",	new random_value(0,3,8,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag CAUSE_2 =	new Monster_Spell_Flag("CAUSE_2" ,	mon_spell_type.RST_ATTACK,	"curses horribly",	0,	0,	GF.NONE,						true,	100,	"points at you and",	new random_value(0,8,8,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag CAUSE_3 =	new Monster_Spell_Flag("CAUSE_3" ,	mon_spell_type.RST_ATTACK,	"incants terribly",	0,	0,	GF.NONE,						true,	100,	"points at you and",	new random_value(0,10,15,0),	new random_value(0,0,0,0),	"mumbles loudly");
		public static Monster_Spell_Flag CAUSE_4 =	new Monster_Spell_Flag("CAUSE_4" ,	mon_spell_type.RST_ATTACK,	"screams the word 'DIE!'",0,0,GF.NONE,					true,	100,	"points at you and",	new random_value(0,15,15,0),	new random_value(0,0,0,0),	"screams the word 'DIE!'");
		public static Monster_Spell_Flag BO_ACID =	new Monster_Spell_Flag("BO_ACID" ,	mon_spell_type.RST_BOLT,	"acid",				0,	0,	GF.ACID,				false,	100,	"casts a bolt of",		new random_value(0,7,8,0),	new random_value(33,0,0,0),	"mumbles");
		public static Monster_Spell_Flag BO_ELEC =	new Monster_Spell_Flag("BO_ELEC" ,	mon_spell_type.RST_BOLT,	"lightning",		0,	0,	GF.ELEC,					false,	100,	"casts a bolt of",		new random_value(0,4,8,0),	new random_value(33,0,0,0),	"mumbles");
		public static Monster_Spell_Flag BO_FIRE =	new Monster_Spell_Flag("BO_FIRE" ,	mon_spell_type.RST_BOLT,	"fire",				0,	0,	GF.FIRE,			false,	100,	"casts a bolt of",		new random_value(0,9,8,0),	new random_value(33,0,0,0),	"mumbles");
		public static Monster_Spell_Flag BO_COLD =	new Monster_Spell_Flag("BO_COLD" ,	mon_spell_type.RST_BOLT,	"frost",			0,	0,	GF.COLD,				false,	100,	"casts a bolt of",		new random_value(0,6,8,0),	new random_value(33,0,0,0),	"mumbles");
		public static Monster_Spell_Flag BO_POIS =	new Monster_Spell_Flag("BO_POIS" ,	mon_spell_type.RST_BOLT,	"poison",			0,	0,	GF.POIS,				false,	100,	"spews a stream of",	new random_value(0,9,8,0),	new random_value(33,0,0,0),	"retches");
		public static Monster_Spell_Flag BO_NETH =	new Monster_Spell_Flag("BO_NETH" ,	mon_spell_type.RST_BOLT,	"nether",			0,	0,	GF.NETHER,			false,	100,	"casts a bolt of",		new random_value(30,5,5,0),	new random_value(150,0,0,0),	"mumbles");
		public static Monster_Spell_Flag BO_WATE = new Monster_Spell_Flag("BO_WATE" ,	mon_spell_type.RST_BOLT,	"water",			0,	0,	GF.WATER,				false,	100,	"fires a jet of",		new random_value(0,10,10,0),	new random_value(100,0,0,0),	"gurgles");
		public static Monster_Spell_Flag BO_MANA = new Monster_Spell_Flag("BO_MANA" ,	mon_spell_type.RST_BOLT,	"raw magic",		0,	0,	GF.MANA,				false,	100,	"casts a bolt of",		new random_value(50,0,0,0),	new random_value(0,1,350,0),	"screams");
		public static Monster_Spell_Flag BO_PLAS = new Monster_Spell_Flag("BO_PLAS" ,	mon_spell_type.RST_BOLT,	"hellfire",			0,	0,	GF.PLASMA,				false,	100,	"casts a bolt of",		new random_value(10,8,7,0),	new random_value(100,0,0,0),	"screams");
		public static Monster_Spell_Flag BO_ICEE = new Monster_Spell_Flag("BO_ICEE" ,	mon_spell_type.RST_BOLT,	"ice",		   		0,	0,	GF.ICE,					false,	100,	"shoots a spear of",	new random_value(0,6,6,0),	new random_value(100,0,0,0),	"mumbles");
		public static Monster_Spell_Flag MISSILE = new Monster_Spell_Flag("MISSILE" ,	mon_spell_type.RST_BOLT,	"magic missile",	0,	0,	GF.MISSILE,				false,	100,	"fires a",				new random_value(0,2,6,0),	new random_value(33,0,0,0),	"mumbles");
		public static Monster_Spell_Flag SCARE = new Monster_Spell_Flag("SCARE" ,      mon_spell_type.RST_ANNOY,	"scary horrors",	0,	0,	GF.NONE,			true,	100,	"conjures up",			new random_value(0,0,0,0),	new random_value(0,0,0,0),	"makes scary noises");
		public static Monster_Spell_Flag BLIND = new Monster_Spell_Flag("BLIND" ,      mon_spell_type.RST_ANNOY,	"blinding flash",	0,	0,	GF.NONE,						true,	100,	"sets off a",			new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag CONF = new Monster_Spell_Flag("CONF" ,       mon_spell_type.RST_ANNOY,	"weird things",			0,	0,	GF.NONE,						true,	100,	"conjures up",			new random_value(0,0,0,0),	new random_value(0,0,0,0),	"messes with your mind");
		public static Monster_Spell_Flag SLOW = new Monster_Spell_Flag("SLOW" ,       mon_spell_type.RST_ANNOY | mon_spell_type.RST_HASTE,"move slowly",0,0,GF.NONE,						true,	100,	"tries to make you",	new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag HOLD = new Monster_Spell_Flag("HOLD" ,       mon_spell_type.RST_ANNOY | mon_spell_type.RST_HASTE,"stop moving",0,0,GF.NONE,						true,	100,	"tries to make you",	new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag HASTE = new Monster_Spell_Flag("HASTE" ,      mon_spell_type.RST_HASTE,	"moving faster",	0,	0,	GF.NONE,						false,	100,	"starts",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag XXX7 = new Monster_Spell_Flag("XXX7" ,		0,								null,				0,	0,	GF.NONE,						false,	0,		null,					new random_value(0,0,0,0),	new random_value(0,0,0,0),	null);
		public static Monster_Spell_Flag HEAL = new Monster_Spell_Flag("HEAL" ,       mon_spell_type.RST_HEAL,		"closing wounds",	0,	0,	GF.NONE,						false,	100,	"magically starts",		new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag XXX8 = new Monster_Spell_Flag("XXX8" ,		0,								null,				0,	0,	GF.NONE,						false,	0,		null,					new random_value(0,0,0,0),	new random_value(0,0,0,0),	null);
		public static Monster_Spell_Flag BLINK = new Monster_Spell_Flag("BLINK" ,      mon_spell_type.RST_TACTIC | mon_spell_type.RST_ESCAPE,"phase door",0,0,GF.NONE,						false,	100,	"casts a",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag TPORT = new Monster_Spell_Flag("TPORT" ,      mon_spell_type.RST_ESCAPE,	"teleport away",	0,	0,	GF.NONE,						false,	100,	"tries to",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"makes a soft 'pop'");
		public static Monster_Spell_Flag XXX9 = new Monster_Spell_Flag("XXX9" ,		0,			null,				0,	0,	GF.NONE,					false,	0,		null,					new random_value(0,0,0,0),	new random_value(0,0,0,0),	null);
		public static Monster_Spell_Flag XXX10 = new Monster_Spell_Flag("XXX10" ,		0,			null,				0,	0,	GF.NONE,						false,	0,		null,					new random_value(0,0,0,0),	new random_value(0,0,0,0),	null);
		public static Monster_Spell_Flag TELE_TO = new Monster_Spell_Flag("TELE_TO" ,    mon_spell_type.RST_ANNOY,	"come hither",		0,	0,	GF.NONE,				false,	100,	"commands you to",		new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag TELE_AWAY = new Monster_Spell_Flag("TELE_AWAY" ,  mon_spell_type.RST_ESCAPE,	"go away",			0,	0,	GF.NONE,		false,	100,	"commands you to",		new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag TELE_LEVEL = new Monster_Spell_Flag("TELE_LEVEL" , mon_spell_type.RST_ESCAPE,	"go far away",		0,	0,	GF.NONE,				true,	100,	"commands you to",		new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag XXX11 = new Monster_Spell_Flag("XXX11" ,		0,			null,				0,	0,	GF.NONE,						false,	0,		null,					new random_value(0,0,0,0),	new random_value(0,0,0,0),	null);
		public static Monster_Spell_Flag DARKNESS = new Monster_Spell_Flag("DARKNESS" ,   mon_spell_type.RST_ANNOY,	"darkness",			0,	0,	GF.DARK_WEAK,			false,	100,	"surrounds you in",		new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag TRAPS = new Monster_Spell_Flag("TRAPS" ,      mon_spell_type.RST_ANNOY,	"traps",			0,	0,	GF.NONE,		false,	100,	"cackles evilly about",	new random_value(0,0,0,0),	new random_value(0,0,0,0),	"cackles evilly");
		public static Monster_Spell_Flag FORGET = new Monster_Spell_Flag("FORGET" ,     mon_spell_type.RST_ANNOY,	"forget things",	0,	0,	GF.NONE,						true,	100,	"tries to make you",	new random_value(0,0,0,0),	new random_value(0,0,0,0),	"messes with your mind");
		public static Monster_Spell_Flag XXX12 = new Monster_Spell_Flag("XXX12" ,		0,			null,				0,	0,	GF.NONE,					false,	0,		null,					new random_value(0,0,0,0),	new random_value(0,0,0,0),	null);
		public static Monster_Spell_Flag S_KIN = new Monster_Spell_Flag("S_KIN" ,      mon_spell_type.RST_SUMMON,	"its kin",			0,	0,	GF.NONE,			false,	100,	"summons",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag S_HI_DEMON = new Monster_Spell_Flag("S_HI_DEMON" , mon_spell_type.RST_SUMMON,	"major demons",		0,	0,	GF.NONE,			false,	100,	"summons",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag S_MONSTER = new Monster_Spell_Flag("S_MONSTER" ,  mon_spell_type.RST_SUMMON,	"a companion",		0,	0,	GF.NONE,			false,	100,	"summons",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag S_MONSTERS = new Monster_Spell_Flag("S_MONSTERS" , mon_spell_type.RST_SUMMON,	"some friends",		0,	0,	GF.NONE,			false,	100,	"summons",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag S_ANIMAL = new Monster_Spell_Flag("S_ANIMAL" ,   mon_spell_type.RST_SUMMON,	"animals",			0,	0,	GF.NONE,				false,	100,	"summons",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag S_SPIDER = new Monster_Spell_Flag("S_SPIDER" ,   mon_spell_type.RST_SUMMON,	"spiders",			0,	0,	GF.NONE,				false,	100,	"summons",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag S_HOUND = new Monster_Spell_Flag("S_HOUND" ,    mon_spell_type.RST_SUMMON,	"hounds",			0,	0,	GF.NONE,			false,	100,	"summons",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag S_HYDRA = new Monster_Spell_Flag("S_HYDRA" ,    mon_spell_type.RST_SUMMON,	"hydrae",			0,	0,	GF.NONE,				false,	100,	"summons",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag S_ANGEL = new Monster_Spell_Flag("S_ANGEL" ,    mon_spell_type.RST_SUMMON,	"an angel",			0,	0,	GF.NONE,			false,	100,	"summons",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag S_DEMON = new Monster_Spell_Flag("S_DEMON" ,    mon_spell_type.RST_SUMMON,	"a demon",			0,	0,	GF.NONE,		false,	100,	"summons",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag S_UNDEAD = new Monster_Spell_Flag("S_UNDEAD" ,   mon_spell_type.RST_SUMMON,	"the undead",		0,	0,	GF.NONE,			false,	100,	"summons",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag S_DRAGON = new Monster_Spell_Flag("S_DRAGON" ,   mon_spell_type.RST_SUMMON,	"a dragon",			0,	0,	GF.NONE,			false,	100,	"summons",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag S_HI_UNDEAD = new Monster_Spell_Flag("S_HI_UNDEAD" ,mon_spell_type.RST_SUMMON,	"fiends of darkness",0,	0,	GF.NONE,			false,100,	"summons",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag S_HI_DRAGON = new Monster_Spell_Flag("S_HI_DRAGON" ,mon_spell_type.RST_SUMMON,	"ancient dragons",	0,	0,	GF.NONE,	false,100,	"summons",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag S_WRAITH = new Monster_Spell_Flag("S_WRAITH" ,   mon_spell_type.RST_SUMMON,	"ringwraiths",		0,	0,	GF.NONE,				false,	100,	"summons",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag S_UNIQUE = new Monster_Spell_Flag("S_UNIQUE" ,   mon_spell_type.RST_SUMMON,	"his servants",		0,	0,	GF.NONE,			false,	100,	"summons",				new random_value(0,0,0,0),	new random_value(0,0,0,0),	"mumbles");
		public static Monster_Spell_Flag MAX = new Monster_Spell_Flag("MAX" ,		0,			null,			    0,	0, 	GF.NONE,					false,	0,		null,					new random_value(0,0,0,0),	new random_value(0,0,0,0),	null);

		/* Flags for non-timed spell effects
		 * - include legal restrictions for "summon_specific()"
		 * (see src/timed.h for timed effect flags) */
		public enum spell_effect_flag {
			S_INV_DAM,
			S_TELEPORT,
			S_TELE_TO,
			S_TELE_LEV,
			S_TELE_SELF,
			S_DRAIN_LIFE,
			S_DRAIN_STAT,
			S_SWAP_STAT,
			S_DRAIN_ALL,
			S_DISEN,
			S_ANIMAL = 11,
			S_SPIDER = 12,
			S_HOUND = 13,
			S_HYDRA = 14,
			S_ANGEL = 15,
			S_DEMON = 16,
			S_UNDEAD = 17,
			S_DRAGON = 18,
			S_HI_DEMON = 26,
			S_HI_UNDEAD = 27,
			S_HI_DRAGON = 28,
			S_WRAITH = 31,
			S_UNIQUE = 32,
			S_KIN = 33,
			S_MONSTER = 41,
			S_MONSTERS = 42,
			S_DRAIN_MANA,
			S_HEAL,
			S_BLINK,
			S_DARKEN,
			S_TRAPS,
			S_AGGRAVATE,

			S_MAX
		};

		/* Spell type bitflags */
		public enum mon_spell_type {
			RST_BOLT    = 0x001,
			RST_BALL    = 0x002,
			RST_BREATH  = 0x004,
			RST_ATTACK  = 0x008,    /* Direct (non-projectable) attacks */
			RST_ANNOY   = 0x010,    /* Irritant spells, usually non-fatal */
			RST_HASTE   = 0x020,    /* Relative speed advantage */
			RST_HEAL    = 0x040,
			RST_TACTIC  = 0x080,    /* Get a better position */
			RST_ESCAPE  = 0x100,
			RST_SUMMON  = 0x200
		};

		/* Minimum flag which can fail */
		public static int MIN_NONINNATE_SPELL   = (Bitflag.FLAG_START + 32);

		public static int SIZE{
			get {
				return Bitflag.FLAG_SIZE((int)Monster_Spell_Flag.MAX.value);
			}
		}




		//SOME of these may belong in Monster_Spell_Effect_Flag.cs...

		/**
		 * Determine the damage of a spell attack which ignores monster hp
		 * (i.e. bolts and balls, including arrows/boulders/storms/etc.)
		 *
		 * \param spell is the attack type
		 * \param rlev is the monster level of the attacker
		 * \param aspect is the damage calc required (min, avg, max, random)
		 */
		//dam_aspect was type "aspect"
		static int nonhp_dam(int spell, int rlev, int dam_aspect)
		{
			throw new NotImplementedException();
			//const struct mon_spell *rs_ptr = &mon_spell_table[spell];
			//int dam;

			///* base damage is X + YdZ (m_bonus is not used) */
			//dam = randcalc(rs_ptr.base_dam, 0, dam_aspect);

			///* rlev-dependent damage (m_bonus is used as a switch) */
			//dam += (rlev * rs_ptr.rlev_dam.base / 100);

			//if (rs_ptr.rlev_dam.m_bonus == 1) /* then rlev affects dice */
			//    dam += damcalc(MIN(1, rs_ptr.rlev_dam.dice * rlev / 100), 
			//            rs_ptr.rlev_dam.sides, dam_aspect);
			//else /* rlev affects sides */
			//    dam += damcalc(rs_ptr.rlev_dam.dice, rs_ptr.rlev_dam.sides *
			//            rlev / 100, dam_aspect);

			//return dam;
		}

		/**
		 * Determine the damage of a monster attack which depends on its hp
		 *
		 * \param spell is the attack type
		 * \param hp is the monster's hp
		 */
		static int hp_dam(int spell, int hp)
		{
			throw new NotImplementedException();
			//const struct mon_spell *rs_ptr = &mon_spell_table[spell];
			//int dam;

			///* Damage is based on monster's current hp */
			//dam = hp / rs_ptr.div;

			///* Check for maximum damage */
			//if (dam > rs_ptr.cap)
			//    dam = rs_ptr.cap;

			//return dam;
		}

		/**
		 * Drain stats at random
		 *
		 * \param num is the number of points to drain
		 * \param sustain is whether sustains will prevent draining
		 * \param perma is whether the drains are permanent
		 */
		static void drain_stats(int num, bool sustain, bool perma)
		{
			throw new NotImplementedException();
			//int i, k = 0;
			//const char *act = null;

			//for (i = 0; i < num; i++) {
			//    switch (randint1(6)) {
			//        case 1: k = A_STR; act = "strong"; break;
			//        case 2: k = A_INT; act = "bright"; break;
			//        case 3: k = A_WIS; act = "wise"; break;
			//        case 4: k = A_DEX; act = "agile"; break;
			//        case 5: k = A_CON; act = "hale"; break;
			//        case 6: k = A_CHR; act = "beautiful"; break;
			//    }

			//    if (sustain)
			//        do_dec_stat(k, perma);
			//    else {
			//        msg("You're not as %s as you used to be...", act);
			//        player_stat_dec(p_ptr, k, perma);
			//    }
			//}

			//return;
		}

		/**
		 * Swap a random pair of stats
		 */
		static void swap_stats()
		{
			throw new NotImplementedException();
			//int max1, cur1, max2, cur2, ii, jj;

			//msg("Your body starts to scramble...");

			///* Pick a pair of stats */
			//ii = randint0(A_MAX);
			//for (jj = ii; jj == ii; jj = randint0(A_MAX)) /* loop */;

			//max1 = p_ptr.stat_max[ii];
			//cur1 = p_ptr.stat_cur[ii];
			//max2 = p_ptr.stat_max[jj];
			//cur2 = p_ptr.stat_cur[jj];

			//p_ptr.stat_max[ii] = max2;
			//p_ptr.stat_cur[ii] = cur2;
			//p_ptr.stat_max[jj] = max1;
			//p_ptr.stat_cur[jj] = cur1;

			//p_ptr.update |= (PU_BONUS);

			//return;
		}

		/**
		 * Drain mana from the player, healing the caster.
		 *
		 * \param m_idx is the monster casting
		 * \param rlev is its level
		 * \param seen is whether @ can see it
		 */
		static void drain_mana(int m_idx, int rlev, bool seen)
		{
			throw new NotImplementedException();
			//monster_type *m_ptr = cave_monster(cave, m_idx);
			//int r1;
			//char m_name[80];

			///* Get the monster name (or "it") */
			//monster_desc(m_name, sizeof(m_name), m_ptr, 0x00);

			//if (!p_ptr.csp) {
			//    msg("The draining fails.");
			//    if (OPT(birth_ai_learn) && !(m_ptr.smart & SM_IMM_MANA)) {
			//        msg("%^s notes that you have no mana!", m_name);
			//        m_ptr.smart |= SM_IMM_MANA;
			//    }
			//    return;
			//}

			///* Attack power */
			//r1 = (randint1(rlev) / 2) + 1;

			///* Full drain */
			//if (r1 >= p_ptr.csp) {
			//    r1 = p_ptr.csp;
			//    p_ptr.csp = 0;
			//    p_ptr.csp_frac = 0;
			//}
			///* Partial drain */
			//else
			//    p_ptr.csp -= r1;

			///* Redraw mana */
			//p_ptr.redraw |= PR_MANA;

			///* Heal the monster */
			//if (m_ptr.hp < m_ptr.maxhp) {
			//    m_ptr.hp += (6 * r1);
			//    if (m_ptr.hp > m_ptr.maxhp)
			//        m_ptr.hp = m_ptr.maxhp;

			//    /* Redraw (later) if needed */
			//    if (p_ptr.health_who == m_idx)
			//        p_ptr.redraw |= (PR_HEALTH);

			//    /* Special message */
			//    if (seen)
			//        msg("%^s appears healthier.", m_name);
			//}
		}

		/**
		 * Monster self-healing.
		 *
		 * \param m_idx is the monster casting
		 * \param rlev is its level
		 * \param seen is whether @ can see it
		 */
		static void heal_self(int m_idx, int rlev, bool seen)
		{
			throw new NotImplementedException();
			//monster_type *m_ptr = cave_monster(cave, m_idx);
			//char m_name[80], m_poss[80];

			///* Get the monster name (or "it") */
			//monster_desc(m_name, sizeof(m_name), m_ptr, 0x00);

			///* Get the monster possessive ("his"/"her"/"its") */
			//monster_desc(m_poss, sizeof(m_poss), m_ptr, MDESC_PRO2 | MDESC_POSS);

			///* Heal some */
			//m_ptr.hp += (rlev * 6);

			///* Fully healed */
			//if (m_ptr.hp >= m_ptr.maxhp) {
			//    m_ptr.hp = m_ptr.maxhp;

			//    if (seen)
			//        msg("%^s looks REALLY healthy!", m_name);
			//    else
			//        msg("%^s sounds REALLY healthy!", m_name);
			//} else if (seen) /* Partially healed */
			//    msg("%^s looks healthier.", m_name);
			//else
			//    msg("%^s sounds healthier.", m_name);

			///* Redraw (later) if needed */
			//if (p_ptr.health_who == m_idx) p_ptr.redraw |= (PR_HEALTH);

			///* Cancel fear */
			//if (m_ptr.m_timed[MON_TMD_FEAR]) {
			//    mon_clear_timed(m_idx, MON_TMD_FEAR, MON_TMD_FLG_NOMESSAGE, false);
			//    msg("%^s recovers %s courage.", m_name, m_poss);
			//}
		}

		/**
		 * Apply side effects from a spell attack to the player
		 *
		 * \param spell is the attack type
		 * \param dam is the amount of damage caused by the attack
		 * \param m_idx is the attacking monster
		 * \param rlev is its level
		 * \param seen is whether @ can see it
		 */
		static void do_side_effects(int spell, int dam, int m_idx, bool seen)
		{
			throw new NotImplementedException();
			//monster_type *m_ptr = cave_monster(cave, m_idx);
			//monster_race *r_ptr = &r_info[m_ptr.r_idx];

			//const struct spell_effect *re_ptr;
			//const struct mon_spell *rs_ptr = &mon_spell_table[spell];

			//int i, choice[99], dur = 0, j = 0, count = 0;
			//s32b d = 0;

			//bool sustain = false, perma = false, chosen[RSE_MAX] = { 0 };

			///* Extract the monster level */
			//int rlev = ((r_ptr.level >= 1) ? r_ptr.level : 1);

			///* First we note all the effects we'll be doing. */
			//for (re_ptr = spell_effect_table; re_ptr.index < RSE_MAX; re_ptr++) {
			//    if ((re_ptr.method && (re_ptr.method == rs_ptr.index)) ||
			//            (re_ptr.gf && (re_ptr.gf == rs_ptr.gf))) {

			//        /* If we have a choice of effects, we create a cum freq table */
			//        if (re_ptr.chance) {
			//            for (i = j; i < (j + re_ptr.chance); i++)
			//                choice[i] = re_ptr.index;
			//            j = i;
			//        }
			//        else
			//            chosen[re_ptr.index] = true;
			//    }
			//}

			///* If we have built a cum freq table, choose an effect from it */
			//if (j)
			//    chosen[choice[randint0(j)]] = true;

			///* Now we cycle through again to activate the chosen effects */
			//for (re_ptr = spell_effect_table; re_ptr.index < RSE_MAX; re_ptr++) {
			//    if (chosen[re_ptr.index]) {

			//        /*
			//         * Check for resistance - there are three possibilities:
			//         * 1. Immunity to the attack type if side_immune is true
			//         * 2. Resistance to the attack type if it affords no immunity
			//         * 3. Resistance to the specific side-effect
			//         *
			//         * TODO - add interesting messages to the RSE_ and GF. tables
			//         * to replace the generic ones below. (See #1376)
			//         */
			//        if (re_ptr.res_flag)
			//            update_smart_learn(m_ptr, p_ptr, re_ptr.res_flag);

			//        if ((rs_ptr.gf && check_side_immune(rs_ptr.gf)) ||
			//                check_state(p_ptr, re_ptr.res_flag, p_ptr.state.flags)) {
			//            msg("You resist the effect!");
			//            continue;
			//        }

			//        /* Allow saving throw if available */
			//        if (re_ptr.save &&
			//                randint0(100) < p_ptr.state.skills[SKILL_SAVE]) {
			//            msg("You avoid the effect!");
			//            continue;
			//        }

			//        /* Implement the effect */
			//        if (re_ptr.timed) {

			//            /* Calculate base duration (m_bonus is not used) */
			//            dur = randcalc(re_ptr.base, 0, RANDOMISE);

			//            /* Calculate the damage-dependent duration (m_bonus is
			//             * used as a cap) */
			//            dur += damcalc(re_ptr.dam.dice, re_ptr.dam.sides *
			//                    dam / 100, RANDOMISE);

			//            if (re_ptr.dam.m_bonus && (dur > re_ptr.dam.m_bonus))
			//                dur = re_ptr.dam.m_bonus;

			//            /* Apply the effect - we have already checked for resistance */
			//            (void)player_inc_timed(p_ptr, re_ptr.flag, dur, true, false);

			//        } else {
			//            switch (re_ptr.flag) {
			//                case S_INV_DAM:
			//                    if (dam > 0)
			//                        inven_damage(p_ptr, re_ptr.gf, MIN(dam *
			//                            randcalc(re_ptr.dam, 0, RANDOMISE), 300));
			//                    break;

			//                case S_TELEPORT: /* m_bonus is used as a clev filter */
			//                    if (!re_ptr.dam.m_bonus || 
			//                            randint1(re_ptr.dam.m_bonus) > p_ptr.lev)
			//                        teleport_player(randcalc(re_ptr.base, 0,
			//                            RANDOMISE));
			//                    break;

			//                case S_TELE_TO:
			//                    teleport_player_to(m_ptr.fy, m_ptr.fx);
			//                    break;

			//                case S_TELE_LEV:
			//                    teleport_player_level();
			//                    break;

			//                case S_TELE_SELF:
			//                    teleport_away(m_ptr, randcalc(re_ptr.base, 0,
			//                        RANDOMISE));
			//                    break;

			//                case S_DRAIN_LIFE:
			//                    d = re_ptr.base.base + (p_ptr.exp *
			//                        re_ptr.base.sides / 100) * MON_DRAIN_LIFE;

			//                    msg("You feel your life force draining away!");
			//                    player_exp_lose(p_ptr, d, false);
			//                    break;

			//                case S_DRAIN_STAT: /* m_bonus is used as a flag */
			//                    if (re_ptr.dam.m_bonus > 0)
			//                        sustain = true;

			//                    if (abs(re_ptr.dam.m_bonus) > 1)
			//                        perma = true;

			//                    drain_stats(randcalc(re_ptr.base, 0, RANDOMISE),
			//                        sustain, perma);
			//                    break;

			//                case S_SWAP_STAT:
			//                    swap_stats();
			//                    break;

			//                case S_DRAIN_ALL:
			//                    msg("You're not as powerful as you used to be...");

			//                    for (i = 0; i < A_MAX; i++)
			//                        player_stat_dec(p_ptr, i, false);
			//                    break;

			//                case S_DISEN:
			//                    (void)apply_disenchant(0);
			//                    break;

			//                case S_DRAIN_MANA:
			//                    drain_mana(m_idx, rlev, seen);
			//                    break;

			//                case S_HEAL:
			//                    heal_self(m_idx, rlev, seen);
			//                    break;

			//                case S_DARKEN:
			//                    (void)unlight_area(0, 3);
			//                    break;

			//                case S_TRAPS:
			//                    (void)trap_creation();
			//                    break;

			//                case S_AGGRAVATE:
			//                    aggravate_monsters(m_idx);
			//                    break;

			//                case S_KIN:
			//                    summon_kin_type = r_ptr.d_char;
			//                case S_MONSTER:	case S_MONSTERS:
			//                case S_SPIDER: case S_HOUND: case S_HYDRA: case S_ANGEL:
			//                case S_ANIMAL:
			//                case S_DEMON: case S_HI_DEMON:
			//                case S_UNDEAD: case S_HI_UNDEAD: case S_WRAITH:
			//                case S_DRAGON: case S_HI_DRAGON:
			//                case S_UNIQUE:
			//                    for (i = 0; i < re_ptr.base.base; i++)
			//                        count += summon_specific(m_ptr.fy, m_ptr.fx,
			//                            rlev, re_ptr.flag, 0);

			//                    for (i = 0; i < re_ptr.dam.base; i++)
			//                        count += summon_specific(m_ptr.fy, m_ptr.fx,
			//                            rlev, S_HI_UNDEAD, 0);

			//                    if (count && p_ptr.timed[TMD_BLIND])
			//                        msgt(rs_ptr.msgt, "You hear %s appear nearby.",
			//                            (count > 1 ? "many things" : "something"));

			//                default:
			//                    break;
			//            }		
			//        }
			//    }
			//}
			//return;
		}

		/**
		 * Calculate the damage of a monster spell.
		 *
		 * \param spell is the spell in question.
		 * \param hp is the hp of the casting monster.
		 * \param rlev is the level of the casting monster.
		 * \param dam_aspect is the damage calc we want (min, max, avg, random).
		 */
		static int mon_spell_dam(int spell, int hp, int rlev, aspect dam_aspect)
		{
			throw new NotImplementedException();
			//const struct mon_spell *rs_ptr = &mon_spell_table[spell];

			//if (rs_ptr.div)
			//    return hp_dam(spell, hp);
			//else
			//    return nonhp_dam(spell, rlev, dam_aspect);
		}


		/**
		 * Process a monster spell 
		 *
		 * \param spell is the monster spell flag (RSF_FOO)
		 * \param m_idx is the attacking monster
		 * \param seen is whether the player can see the monster at this moment
		 */
		void do_mon_spell(int spell, int m_idx, bool seen)
		{
			throw new NotImplementedException();
			//const struct mon_spell *rs_ptr = &mon_spell_table[spell];

			//monster_type *m_ptr = cave_monster(cave, m_idx);
			//monster_race *r_ptr = &r_info[m_ptr.r_idx];

			//char m_name[80], ddesc[80];

			//bool hits = false;

			//int dam = 0, flag = 0, rad = 0;

			///* Extract the monster level */
			//int rlev = ((r_ptr.level >= 1) ? r_ptr.level : 1);

			///* Get the monster name (or "it") */
			//monster_desc(m_name, sizeof(m_name), m_ptr, 0x00);

			///* See if it hits */
			//if (rs_ptr.hit == 100) 
			//    hits = true;
			//else if (rs_ptr.hit == 0)
			//    hits = false;
			//else
			//    hits = check_hit(p_ptr, rs_ptr.hit, rlev);

			///* Tell the player what's going on */
			//disturb(p_ptr, 1,0);

			//if (!seen)
			//    msg("Something %s.", rs_ptr.blind_verb);
			//else if (!hits) {
			//    msg("%^s %s %s, but misses.", m_name, rs_ptr.verb,	rs_ptr.desc);
			//    return;
			//} else if (rs_ptr.msgt)
			//    msgt(rs_ptr.msgt, "%^s %s %s.", m_name, rs_ptr.verb, rs_ptr.desc);
			//else 
			//    msg("%^s %s %s.", m_name, rs_ptr.verb, rs_ptr.desc);


			///* Try a saving throw if available */
			//if (rs_ptr.save && randint0(100) < p_ptr.state.skills[SKILL_SAVE]) {
			//    msg("You avoid the effects!");
			//    return;
			//}

			///* Calculate the damage */
			//dam = mon_spell_dam(spell, m_ptr.hp, rlev, RANDOMISE);

			///* Get the "died from" name in case this attack kills @ */
			//monster_desc(ddesc, sizeof(ddesc), m_ptr, MDESC_SHOW | MDESC_IND2);

			///* Display the attack, adjust for resists and apply effects */
			//if (rs_ptr.type & RST_BOLT)
			//    flag = PROJECT_STOP | PROJECT_KILL;
			//else if (rs_ptr.type & (RST_BALL | RST_BREATH)) {
			//    flag = PROJECT_GRID | PROJECT_ITEM | PROJECT_KILL;
			//    rad = rf_has(r_ptr.flags, RF_POWERFUL) ? 3 : 2;
			//}

			//if (rs_ptr.gf) {
			//    (void)project(m_idx, rad, p_ptr.py, p_ptr.px, dam, rs_ptr.gf, flag);
			//    monster_learn_resists(m_ptr, p_ptr, rs_ptr.gf);
			//}
			//else /* Note that non-projectable attacks are unresistable */
			//    take_hit(p_ptr, dam, ddesc);

			//do_side_effects(spell, dam, m_idx, seen);

			//return;
		}

		/**
		 * Test a spell bitflag for a type of spell.
		 * Returns true if any desired type is among the flagset
		 *
		 * \param f is the set of spell flags we're testing
		 * \param type is the spell type(s) we're looking for
		 */
		bool test_spells(Bitflag f, mon_spell_type type)
		{
			throw new NotImplementedException();
			//const struct mon_spell *rs_ptr;

			//for (rs_ptr = mon_spell_table; rs_ptr.index < RSF_MAX; rs_ptr++)
			//    if (rsf_has(f, rs_ptr.index) && (rs_ptr.type & type))
			//        return true;

			//return false;
		}

		/**
		 * Set a spell bitflag to allow only a specific set of spell types.
		 *
		 * \param f is the set of spell flags we're pruning
		 * \param type is the spell type(s) we're allowing
		 */
		void set_spells(Bitflag f, mon_spell_type type)
		{
			throw new NotImplementedException();
			//const struct mon_spell *rs_ptr;

			//for (rs_ptr = mon_spell_table; rs_ptr.index < RSF_MAX; rs_ptr++)
			//    if (rsf_has(f, rs_ptr.index) && !(rs_ptr.type & type))
			//        rsf_off(f, rs_ptr.index);

			//return;
		}

		/**
		 * Turn off spells with a side effect or a gf_type that is resisted by
		 * something in flags, subject to intelligence and chance.
		 *
		 * \param spells is the set of spells we're pruning
		 * \param flags is the set of flags we're testing
		 * \param r_ptr is the monster type we're operating on
		 */
		void unset_spells(Bitflag spells, Bitflag flags, Monster_Race r_ptr)
		{
			throw new NotImplementedException();
			//const struct mon_spell *rs_ptr;
			//const struct spell_effect *re_ptr;

			///* First we test the gf (projectable) spells */
			//for (rs_ptr = mon_spell_table; rs_ptr.index < RSF_MAX; rs_ptr++)
			//    if (rs_ptr.gf && randint0(100) < check_for_resist(p_ptr, rs_ptr.gf, flags,
			//            false) * (rf_has(r_ptr.flags, RF_SMART) ? 2 : 1) * 25)
			//        rsf_off(spells, rs_ptr.index);

			///* ... then we test the non-gf side effects */
			//for (re_ptr = spell_effect_table; re_ptr.index < RSE_MAX; re_ptr++)
			//    if (re_ptr.method && re_ptr.res_flag && (rf_has(r_ptr.flags,
			//            RF_SMART) || !one_in_(3)) && of_has(flags, re_ptr.res_flag))
			//        rsf_off(spells, re_ptr.method);
		}

		/**
		 * Calculate a monster's maximum spell power.
		 *
		 * \param r_ptr is the monster we're studying
		 * \param resist is the degree of resistance we're assuming to any
		 *   attack type (-1 = vulnerable ... 3 = immune)
		 */
		public static int best_spell_power(Monster_Race r_ptr, int resist)
		{
			//const struct mon_spell *rs_ptr;
			//const struct spell_effect *re_ptr;
			
			int dam = 0, best_dam = 0; 

			/* Extract the monster level */
			int rlev = ((r_ptr.level >= 1) ? r_ptr.level : 1);

			//for (Monster_Spell_Flag rs_ptr = mon_spell_table; rs_ptr.index < RSF_MAX; rs_ptr++) {
			foreach (Monster_Spell_Flag rs_ptr in list){
			    if (r_ptr.spell_flags.has(rs_ptr.value)) {

			        /* Get the maximum basic damage output of the spell (could be 0) */
			        dam = mon_spell_dam(rs_ptr.value, Monster_Make.mon_hp(r_ptr, aspect.MAXIMISE), rlev,
			            aspect.MAXIMISE);

			        /* For all attack forms the player can save against, damage
			         * is halved */
			        if (rs_ptr.save)
			            dam /= 2;

			        /* Adjust the real damage by the assumed resistance (if it is a
			         * resistable type) */
			        if (rs_ptr.gf != GF.NONE)
			            dam = Spell.adjust_dam(Player.Player.instance, rs_ptr.gf, dam, aspect.MAXIMISE, resist);

			        /* Add the power ratings assigned to the various possible spell
			         * effects (which is crucial for non-damaging spells) */
			        foreach(Monster_Spell_Effect_Flag re_ptr in Monster_Spell_Effect_Flag.list){
					//for (re_ptr = spell_effect_table; re_ptr.index < RSE_MAX; re_ptr++) {
			            if ((re_ptr.method != null && (re_ptr.method.value == rs_ptr.value)) ||
			                    (re_ptr.gf != null && (re_ptr.gf == rs_ptr.gf))) {

			                /* First we adjust the real damage if necessary */
			                if (re_ptr.power.dice != 0)
			                    dam = (dam * re_ptr.power.dice) / 100;

			                /* Then we add any flat rating for this effect */
			                dam += re_ptr.power.Base;

			                /* Then we add any rlev-dependent rating */
			                if (re_ptr.power.m_bonus < 0)
			                    dam += re_ptr.power.sides / (rlev + 1);
			                else if (re_ptr.power.m_bonus > 0)
			                    dam += (re_ptr.power.sides * rlev) / 100;
			            }
			        }

			        /* Update the best_dam tracker */
			        if (dam > best_dam)
			            best_dam = dam;
			    }
			}

			return best_dam;
		}
	}
}
