using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CSAngband.Object {
	class Object_Flag {
		/*** Constants ***/
		/* The object flag types */
		public enum object_flag_type {
			PVAL = 1,	/* pval-related but not to a stat */
			STAT,		/* affects a stat */
			SUST,		/* sustains a stat */
			SLAY,		/* a "normal" creature-type slay */
			BRAND,		/* a brand against monsters lacking the resist */
			KILL,		/* a powerful creature-type slay */
			VULN,		/* lowers resistance to an element */
			IMM,		/* offers immunity to an element */
			LRES,		/* a "base" elemental resistance */
			HRES,		/* a "high" elemental resistance */
			IGNORE,		/* object ignores an element */
			HATES,		/* object can be destroyed by element */
			PROT,		/* protection from an effect */
			MISC,		/* a good property, suitable for ego items */
			LIGHT,		/* applicable only to light sources */
			MELEE,		/* applicable only to melee weapons */
			CURSE,		/* a "sticky" curse */
			BAD,		/* an undesirable flag that isn't a curse */
			INT,		/* an internal flag, not shown in the game */
			
			MAX
		};

		/* How object flags are IDd */
		public enum object_flag_id {
			NONE = 0,		/* never shown */
			NORMAL,		/* normal ID on use */
			TIMED,			/* obvious after time */
			WIELD			/* obvious on wield */
		};

		public static List<Object_Flag> list = new List<Object_Flag>();

		private static int counter = 0;
		public Object_Flag(string name, bool pval, Timed_Effect timed, object_flag_id id, object_flag_type type,
							int power, int p_m, int wpn, int bow, int ring, int amu, int light, int body, int cloak,
							int shield, int hat, int gloves, int boots, string message) {
		
			this.value = counter++;
			this.name = name;
			this.pval = pval;
			this.timed = timed;
			this.id = id;
			this.type = type;
			this.power = power;
			this.p_m = p_m;
			this.weapon = wpn;
			this.bow = bow;
			this.ring = ring;
			this.amulet = amu;
			this.light = light;
			this.body = body;
			this.cloak = cloak;
			this.shield = shield;
			this.hat = hat;
			this.gloves = gloves;
			this.boots = boots;
			this.message = message;
			
			list.Add(this);
		}

		public int value;
		public string name;
		public bool pval;
		public Timed_Effect timed;
		public object_flag_id id;
		public object_flag_type type;
		public int power;
		public int p_m;
		public int weapon;
		public int bow;
		public int ring;
		public int amulet;
		public int light;
		public int body;
		public int cloak;
		public int shield;
		public int hat;
		public int gloves;
		public int boots;
		public string message;

		/*
		 * File: src/object/list-object-flags.h
		 * Purpose: object flags for all objects
		 *
		 * Changing flag order will break savefiles. There is a hard-coded limit of
		 * 256 flags, due to 32 bytes of storage for item flags in the savefile. Flags
		 * below start from 0 on line 21, so a flag's sequence number is its line
		 * number minus 21.
		 * 
		 * index: the flag number
		 * pval: is it a quantitative flag? false means it's just on/off
		 * timed: what is the corresponding Timed_Effect. flag
		 * id: when the flag is IDd
		 * type: what type of flag is it?
		 * power: base power rating for the flag (0 means it is unused or derived)
		 * pval_mult: weight of this flag relative to other pval flags
		 * wpn/bow/ring/amu/light/body/cloak/shield/hat/gloves/boots: power multiplier for this slot
		 * message: what is printed when the flag is IDd (but see also identify.c and list-slays.h)
		 */
		/* index       	pval	timed			id			type		power	p_m	wpn	bow	ring	amu	light	body	cloak	shield	hat	gloves	boots	message */
		public static Object_Flag NONE = new Object_Flag("NONE" ,        false,	0,				0,			0,			0,		0,	0,	0,	0,		0,	0,		0,		0,		0,		0,	0,		0,		"");
		public static Object_Flag STR = new Object_Flag("STR" ,         true,	0,				object_flag_id.WIELD,	object_flag_type.STAT,	9,		13,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"");
		public static Object_Flag INT = new Object_Flag("INT" ,         true,	0,				object_flag_id.WIELD,	object_flag_type.STAT,	5,		10,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"");
		public static Object_Flag WIS = new Object_Flag("WIS" ,         true,	0,				object_flag_id.WIELD,	object_flag_type.STAT,	5,		10,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"");
		public static Object_Flag DEX = new Object_Flag("DEX" ,         true,	0,				object_flag_id.WIELD,	object_flag_type.STAT,	8,		10,	1,	1,	1,		1,	1,		1,		1,		1,		1,	2,		1,		"");
		public static Object_Flag CON = new Object_Flag("CON" ,         true,	0,				object_flag_id.WIELD,	object_flag_type.STAT,	12,		15,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"");
		public static Object_Flag CHR = new Object_Flag("CHR" ,         true,	0,				object_flag_id.WIELD,	object_flag_type.STAT,	2,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"");
		public static Object_Flag XXX1 = new Object_Flag("XXX1" ,        false,	0,				0,			0,			0,		0,	0,	0,	0,		0,	0,		0,		0,		0,		0,	0,		0,		"");
		public static Object_Flag XXX2 = new Object_Flag("XXX2" ,        false,	0,				0,			0,			0,		0,	0,	0,	0,		0,	0,		0,		0,		0,		0,	0,		0,		"");
		public static Object_Flag STEALTH = new Object_Flag("STEALTH" ,     true,	0,				object_flag_id.WIELD,	object_flag_type.PVAL,	8,		12,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag SEARCH = new Object_Flag("SEARCH" ,      true,	0,				object_flag_id.WIELD,	object_flag_type.PVAL,	2,		5,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag INFRA = new Object_Flag("INFRA" ,       true,	Timed_Effect.SINFRA,		object_flag_id.WIELD,	object_flag_type.PVAL,	4,		8,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"");
		public static Object_Flag TUNNEL = new Object_Flag("TUNNEL" ,      true,	0,				object_flag_id.WIELD,	object_flag_type.PVAL,	3,		8,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"");
		public static Object_Flag SPEED = new Object_Flag("SPEED" ,       true,	Timed_Effect.FAST,		object_flag_id.WIELD,	object_flag_type.PVAL,	20,		6,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"");
		public static Object_Flag BLOWS = new Object_Flag("BLOWS" ,       true,	0,				object_flag_id.WIELD,	object_flag_type.PVAL,	0,		50,	1,	0,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag SHOTS = new Object_Flag("SHOTS" ,       true,	0,				object_flag_id.WIELD,	object_flag_type.PVAL,	0,		50,	0,	1,	4,		4,	4,		4,		4,		4,		4,	4,		4,		"");
		public static Object_Flag MIGHT = new Object_Flag("MIGHT" ,       true,	0,				object_flag_id.WIELD,	object_flag_type.PVAL,	0,		30,	0,	1,	0,		0,	0,		0,		0,		0,		0,	0,		0,		"");
		public static Object_Flag SLAY_ANIMAL = new Object_Flag("SLAY_ANIMAL" , false,	0,				object_flag_id.NORMAL,object_flag_type.SLAY,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag SLAY_EVIL = new Object_Flag("SLAY_EVIL" ,   false,	0,				object_flag_id.NORMAL,object_flag_type.SLAY,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag SLAY_UNDEAD = new Object_Flag("SLAY_UNDEAD" , false,	0,				object_flag_id.NORMAL,object_flag_type.SLAY,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag SLAY_DEMON = new Object_Flag("SLAY_DEMON" ,  false,	0,				object_flag_id.NORMAL,object_flag_type.SLAY,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag SLAY_ORC = new Object_Flag("SLAY_ORC" ,    false,	0,				object_flag_id.NORMAL,object_flag_type.SLAY,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag SLAY_TROLL = new Object_Flag("SLAY_TROLL" ,  false,	0,				object_flag_id.NORMAL,object_flag_type.SLAY,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag SLAY_GIANT = new Object_Flag("SLAY_GIANT" ,  false,	0,				object_flag_id.NORMAL,object_flag_type.SLAY,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag SLAY_DRAGON = new Object_Flag("SLAY_DRAGON" , false,	0,				object_flag_id.NORMAL,object_flag_type.SLAY,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag KILL_DRAGON = new Object_Flag("KILL_DRAGON" , false,	0,				object_flag_id.NORMAL,object_flag_type.KILL,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag KILL_DEMON = new Object_Flag("KILL_DEMON" ,  false,	0,				object_flag_id.NORMAL,object_flag_type.KILL,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag KILL_UNDEAD = new Object_Flag("KILL_UNDEAD" , false,	0,				object_flag_id.NORMAL,object_flag_type.KILL,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag BRAND_POIS = new Object_Flag("BRAND_POIS" ,  false,	0,				object_flag_id.WIELD,	object_flag_type.BRAND,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag BRAND_ACID = new Object_Flag("BRAND_ACID" ,  false,	0,				object_flag_id.WIELD,	object_flag_type.BRAND,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag BRAND_ELEC = new Object_Flag("BRAND_ELEC" ,  false,	0,				object_flag_id.WIELD,	object_flag_type.BRAND,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag BRAND_FIRE = new Object_Flag("BRAND_FIRE" ,  false,	0,				object_flag_id.WIELD,	object_flag_type.BRAND,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag BRAND_COLD = new Object_Flag("BRAND_COLD" ,  false,	0,				object_flag_id.WIELD, object_flag_type.BRAND,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag SUST_STR = new Object_Flag("SUST_STR" ,    false,	0,				object_flag_id.NORMAL,object_flag_type.SUST,	9,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag SUST_INT = new Object_Flag("SUST_INT" ,    false,	0,				object_flag_id.NORMAL,object_flag_type.SUST,	4,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag SUST_WIS = new Object_Flag("SUST_WIS" ,    false,	0,				object_flag_id.NORMAL,object_flag_type.SUST,	4,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag SUST_DEX = new Object_Flag("SUST_DEX" ,    false,	0,				object_flag_id.NORMAL,object_flag_type.SUST,	7,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag SUST_CON = new Object_Flag("SUST_CON" ,    false, 	0,				object_flag_id.NORMAL,object_flag_type.SUST,	8,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag SUST_CHR = new Object_Flag("SUST_CHR" ,    false,	0,				object_flag_id.NORMAL,object_flag_type.SUST,	1,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag VULN_ACID = new Object_Flag("VULN_ACID" ,   false, 	0,				object_flag_id.NORMAL,object_flag_type.VULN,	-6,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag VULN_ELEC = new Object_Flag("VULN_ELEC" ,   false,	0,				object_flag_id.NORMAL,object_flag_type.VULN,	-6,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag VULN_FIRE = new Object_Flag("VULN_FIRE" ,   false,	0,				object_flag_id.NORMAL,object_flag_type.VULN,	-6,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag VULN_COLD = new Object_Flag("VULN_COLD" ,   false,	0,				object_flag_id.NORMAL,object_flag_type.VULN,	-6,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag XXX3 = new Object_Flag("XXX3" ,        false,	0,				0,			0,			0,		0,	0,	0,	0,		0,	0,		0,		0,		0,		0,	0,		0,		"Your %s glows.");
		public static Object_Flag XXX4 = new Object_Flag("XXX4" ,        false,	0,				0,			0,			0,		0,	0,	0,	0,		0,	0,		0,		0,		0,		0,	0,		0,		"Your %s glows.");
		public static Object_Flag IM_ACID = new Object_Flag("IM_ACID" ,     false, 	0,				object_flag_id.NORMAL,object_flag_type.IMM,	38,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag IM_ELEC = new Object_Flag("IM_ELEC" ,     false,	0,				object_flag_id.NORMAL,object_flag_type.IMM,	35,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag IM_FIRE = new Object_Flag("IM_FIRE" ,     false,	0,				object_flag_id.NORMAL,object_flag_type.IMM,	40,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag IM_COLD = new Object_Flag("IM_COLD" ,     false,	0,				object_flag_id.NORMAL,object_flag_type.IMM,	37,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag RES_ACID = new Object_Flag("RES_ACID" ,    false,	Timed_Effect.OPP_ACID,	object_flag_id.NORMAL,object_flag_type.LRES,	5,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag RES_ELEC = new Object_Flag("RES_ELEC" ,    false,	Timed_Effect.OPP_ELEC,	object_flag_id.NORMAL,object_flag_type.LRES,	6,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag RES_FIRE = new Object_Flag("RES_FIRE" ,    false,	Timed_Effect.OPP_FIRE,	object_flag_id.NORMAL,object_flag_type.LRES,	6,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag RES_COLD = new Object_Flag("RES_COLD" ,    false,	Timed_Effect.OPP_COLD,	object_flag_id.NORMAL,object_flag_type.LRES,	6,		0,	1,	1,	1,		1,	1,		1,		1,		1,		1,	1,		1,		"Your %s glows.");
		public static Object_Flag RES_POIS = new Object_Flag("RES_POIS" ,    false,	Timed_Effect.OPP_POIS,	object_flag_id.NORMAL,object_flag_type.HRES,	28,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"Your %s glows.");
		public static Object_Flag RES_FEAR = new Object_Flag("RES_FEAR" ,    false,	Timed_Effect.BOLD,		object_flag_id.NORMAL,object_flag_type.PROT,	6,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"Your %s glows.");
		public static Object_Flag RES_LIGHT = new Object_Flag("RES_LIGHT" ,   false,	0,				object_flag_id.NORMAL,object_flag_type.HRES,	6,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"Your %s glows.");
		public static Object_Flag RES_DARK = new Object_Flag("RES_DARK" ,    false,	0,				object_flag_id.NORMAL,object_flag_type.HRES,	16,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"Your %s glows.");
		public static Object_Flag RES_BLIND = new Object_Flag("RES_BLIND" ,   false,	0,				object_flag_id.NORMAL,object_flag_type.PROT,	16,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"Your %s glows.");
		public static Object_Flag RES_CONFU = new Object_Flag("RES_CONFU" ,   false,	Timed_Effect.OPP_CONF,	object_flag_id.NORMAL,object_flag_type.PROT,	24,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"Your %s glows.");
		public static Object_Flag RES_SOUND = new Object_Flag("RES_SOUND" ,   false,	0,				object_flag_id.NORMAL,object_flag_type.HRES,	14,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"Your %s glows.");
		public static Object_Flag RES_SHARD = new Object_Flag("RES_SHARD" ,   false,	0,				object_flag_id.NORMAL,object_flag_type.HRES,	8,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"Your %s glows.");
		public static Object_Flag RES_NEXUS = new Object_Flag("RES_NEXUS" ,   false,	0,				object_flag_id.NORMAL,object_flag_type.HRES,	15,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"Your %s glows.");
		public static Object_Flag RES_NETHR = new Object_Flag("RES_NETHR" ,   false,	0,				object_flag_id.NORMAL,object_flag_type.HRES,	20,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"Your %s glows.");
		public static Object_Flag RES_CHAOS = new Object_Flag("RES_CHAOS" ,   false,	0,				object_flag_id.NORMAL,object_flag_type.HRES,	20,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"Your %s glows.");
		public static Object_Flag RES_DISEN = new Object_Flag("RES_DISEN" ,   false, 	0,				object_flag_id.NORMAL,object_flag_type.HRES,	20,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"Your %s glows.");
		public static Object_Flag SLOW_DIGEST = new Object_Flag("SLOW_DIGEST" , false,	0,				object_flag_id.TIMED,	object_flag_type.MISC,	2,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"You feel your %s slow your metabolism.");
		public static Object_Flag FEATHER = new Object_Flag("FEATHER" ,     false,	0,				object_flag_id.NORMAL,object_flag_type.MISC,	1,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"Your %s slows your fall.");
		public static Object_Flag LIGHT = new Object_Flag("LIGHT" ,       false,	0,				object_flag_id.WIELD,	object_flag_type.MISC,	3,		0,	1,  1,  1,      1,  10,     1,      1,      1,      1,  1,      1,		"");
		public static Object_Flag REGEN = new Object_Flag("REGEN" ,       false,	0,				object_flag_id.TIMED,	object_flag_type.MISC,	5,		0,	1,  1,  2,      2,  2,      2,      2,      2,      2,  2,      2,		"You feel your %s speed up your recovery.");
		public static Object_Flag TELEPATHY = new Object_Flag("TELEPATHY" ,   false,	Timed_Effect.TELEPATHY,	object_flag_id.WIELD,	object_flag_type.MISC,	35,		0,	1,	1,	2,		2,	2,		2,		2,		2,		2,	2,		2,		"");
		public static Object_Flag SEE_INVIS = new Object_Flag("SEE_INVIS" ,   false,	Timed_Effect.SINVIS,		object_flag_id.WIELD,	object_flag_type.MISC,	6,		0,	1,  1,  2,      2,  2,      2,      2,      2,      2,  2,      2,		"");
		public static Object_Flag FREE_ACT = new Object_Flag("FREE_ACT" ,    false,	0,				object_flag_id.NORMAL,object_flag_type.MISC,	8,		0,	1,  1,  2,      2,  2,      2,      2,      2,      2,  5,      2,		"Your %s glows.");
		public static Object_Flag HOLD_LIFE = new Object_Flag("HOLD_LIFE" ,   false,	0,				object_flag_id.NORMAL,object_flag_type.MISC,	5,		0,	1,  1,  2,      2,  2,      2,      2,      2,      2,  2,      2,		"Your %s glows.");
		public static Object_Flag NO_FUEL = new Object_Flag("NO_FUEL" ,     false,	0,				object_flag_id.WIELD,	object_flag_type.LIGHT,	5,		0,	0,	0,	0,		0,	1,		0,		0,		0,		0,	0,		0,		"");
		public static Object_Flag IMPAIR_HP = new Object_Flag("IMPAIR_HP" ,   false,	0,				object_flag_id.TIMED,	object_flag_type.BAD,	-9,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"You feel your %s slow your recovery.");
		public static Object_Flag IMPAIR_MANA = new Object_Flag("IMPAIR_MANA" , false,	0,				object_flag_id.TIMED,	object_flag_type.BAD,	-9,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"You feel your %s slow your mana recovery.");
		public static Object_Flag AFRAID = new Object_Flag("AFRAID" ,      false,	Timed_Effect.AFRAID,		object_flag_id.WIELD,	object_flag_type.BAD,	-20,	0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"");
		public static Object_Flag IMPACT = new Object_Flag("IMPACT" ,      false,	0,				object_flag_id.NORMAL,object_flag_type.MELEE,	10,		0,	1,	0,	0,		0,	0,		0,		0,		0,		0,	0,		0,		"Your %s causes an earthquake!");
		public static Object_Flag TELEPORT = new Object_Flag("TELEPORT" ,    false,	0,				object_flag_id.NORMAL,object_flag_type.BAD,	-20,	0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"Your %s teleports you.");
		public static Object_Flag AGGRAVATE = new Object_Flag("AGGRAVATE" ,   false,	0,				object_flag_id.TIMED,	object_flag_type.BAD,	-20,	0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"You feel your %s aggravate things around you.");
		public static Object_Flag DRAIN_EXP = new Object_Flag("DRAIN_EXP" ,   false,	0,				object_flag_id.TIMED,	object_flag_type.BAD,	-5,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"You feel your %s drain your life.");
		public static Object_Flag IGNORE_ACID = new Object_Flag("IGNORE_ACID" , false,	0,				object_flag_id.NORMAL,object_flag_type.IGNORE,	3,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"");
		public static Object_Flag IGNORE_ELEC = new Object_Flag("IGNORE_ELEC" , false,	0,				object_flag_id.NORMAL,object_flag_type.IGNORE,	1,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"");
		public static Object_Flag IGNORE_FIRE = new Object_Flag("IGNORE_FIRE" , false,	0,				object_flag_id.NORMAL,object_flag_type.IGNORE,	3,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"");
		public static Object_Flag IGNORE_COLD = new Object_Flag("IGNORE_COLD" , false,	0,				object_flag_id.NORMAL,object_flag_type.IGNORE,	1,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"");
		public static Object_Flag RES_STUN = new Object_Flag("RES_STUN" ,    false,	0,				object_flag_id.NORMAL,object_flag_type.PROT,	12,		0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"Your %s glows.");
		public static Object_Flag XXX5 = new Object_Flag("XXX5" ,        false,	0,				0,			0,			0,		0,	0,	0,	0,		0,	0,		0,		0,		0,		0,	0,		0,		"");
		public static Object_Flag BLESSED = new Object_Flag("BLESSED" ,     false,	0,				object_flag_id.WIELD,	object_flag_type.MELEE,	1,		0,	1,	0,	0,		0,	0,		0,		0,		0,		0,	0,		0,		"");
		public static Object_Flag XXX6 = new Object_Flag("XXX6" ,        false,	0,				0,			0,			0,		0,	0,	0,	0,		0,	0,		0,		0,		0,		0,	0,		0,		"");
		public static Object_Flag INSTA_ART = new Object_Flag("INSTA_ART" ,   false,	0,				object_flag_id.NONE,	object_flag_type.INT,	0,      0,	0,  0,  0,      0,  0,      0,      0,      0,      0,  0,      0,		"");
		public static Object_Flag EASY_KNOW = new Object_Flag("EASY_KNOW" ,   false,	0,				object_flag_id.NONE,	object_flag_type.INT,	0,      0,	0,  0,  0,      0,  0,      0,      0,      0,      0,  0,      0,		"");
		public static Object_Flag HIDE_TYPE = new Object_Flag("HIDE_TYPE" ,   false,	0,				object_flag_id.NONE,	object_flag_type.INT,	0,      0,	0,  0,  0,      0,  0,      0,      0,      0,      0,  0,      0,		"");
		public static Object_Flag SHOW_MODS = new Object_Flag("SHOW_MODS" ,   false,	0,				object_flag_id.NONE,	object_flag_type.INT,	0,      0,	0,  0,  0,      0,  0,      0,      0,      0,      0,  0,      0,		"");
		public static Object_Flag XXX7 = new Object_Flag("XXX7" ,        false,	0,				0,			0,			0,      0,	0,  0,  0,      0,  0,      0,      0,      0,      0,  0,      0,		"");
		public static Object_Flag LIGHT_CURSE = new Object_Flag("LIGHT_CURSE" , false,	0,				object_flag_id.WIELD,	object_flag_type.CURSE,	-5,     0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"");
		public static Object_Flag HEAVY_CURSE = new Object_Flag("HEAVY_CURSE" , false,	0,				object_flag_id.WIELD,	object_flag_type.CURSE,	-15,    0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"");
		public static Object_Flag PERMA_CURSE = new Object_Flag("PERMA_CURSE" , false,	0,				object_flag_id.WIELD,	object_flag_type.CURSE,	-25,    0,	1,  1,  1,      1,  1,      1,      1,      1,      1,  1,      1,		"");
		public static Object_Flag HATES_ACID = new Object_Flag("HATES_ACID" ,  false,	0,				object_flag_id.NONE,	object_flag_type.HATES,	0,      0,	0,  0,  0,      0,  0,      0,      0,      0,      0,  0,      0,		"");
		public static Object_Flag HATES_ELEC = new Object_Flag("HATES_ELEC" ,  false,	0,				object_flag_id.NONE,	object_flag_type.HATES,	0,      0,	0,  0,  0,      0,  0,      0,      0,      0,      0,  0,      0,		"");
		public static Object_Flag HATES_FIRE = new Object_Flag("HATES_FIRE" ,  false,	0,				object_flag_id.NONE,	object_flag_type.HATES,	0,      0,	0,  0,  0,      0,  0,      0,      0,      0,      0,  0,      0,		"");
		public static Object_Flag HATES_COLD = new Object_Flag("HATES_COLD" ,  false,	0,				object_flag_id.NONE,	object_flag_type.HATES,	0,      0,	0,  0,  0,      0,  0,      0,      0,      0,      0,  0,      0,		"");
		public static Object_Flag SPELLS_OK = new Object_Flag("SPELLS_OK" ,   false, 	0,				object_flag_id.NONE,	object_flag_type.INT,	0,      0,	0,  0,  0,      0,  0,      0,      0,      0,      0,  0,      0,		"");
		public static Object_Flag SHOW_DICE = new Object_Flag("SHOW_DICE" ,   false,	0,				object_flag_id.NONE,	object_flag_type.INT,	0,      0,	0,  0,  0,      0,  0,      0,      0,      0,      0,  0,      0,		"");
		public static Object_Flag SHOW_MULT = new Object_Flag("SHOW_MULT" ,	false,	0,				object_flag_id.NONE,	object_flag_type.INT,	0,      0,	0,  0,  0,      0,  0,      0,      0,      0,      0,  0,      0,		"");
		public static Object_Flag BRAND_ICKY = new Object_Flag("BRAND_ICKY" ,  false,	0,				object_flag_id.WIELD,	object_flag_type.BRAND,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag BRAND_FIZZ = new Object_Flag("BRAND_FIZZ" ,  false,	0,				object_flag_id.WIELD,	object_flag_type.BRAND,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag BRAND_BUZZ = new Object_Flag("BRAND_BUZZ" ,  false,	0,				object_flag_id.WIELD,	object_flag_type.BRAND,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag BRAND_WARM = new Object_Flag("BRAND_WARM" ,  false,	0,				object_flag_id.WIELD,	object_flag_type.BRAND,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag BRAND_COOL = new Object_Flag("BRAND_COOL" ,  false,	0,				object_flag_id.WIELD, object_flag_type.BRAND,	0,		0,	1,	3,	3,		3,	3,		3,		3,		3,		3,	3,		3,		"");
		public static Object_Flag MAX = new Object_Flag("MAX" ,			false,	0,				0,			0,			0,      0,	0,  0,  0,      0,  0,      0,      0,      0,      0,  0,      0,		"");


		public static int SIZE{
			get {
				return Bitflag.FLAG_SIZE(MAX.value);
			}
		}
		public static int BYTES = 32;  /* savefile bytes, i.e. 256 flags */

		/* Hack -- special "xtra" object flag info (type) */
		/* Can get rid of these now we have OFT_ flags */
		/* No - because "POWER" uses two types of OFTs, so cannot get rid of these
		 * until ego_item.txt has an X: line with a variable number of OFTs - that's
		 * basically waiting for a rewrite of ego generation 
		 * -- or we could change OFTs to a bitflag */
		public const int OBJECT_XTRA_TYPE_NONE    = 0;
		public const int OBJECT_XTRA_TYPE_SUSTAIN = 1;
		public const int OBJECT_XTRA_TYPE_RESIST  = 2;
		public const int OBJECT_XTRA_TYPE_POWER = 3;

		/**
		 * Create a "mask" of flags of a specific type or ID threshold.
		 *
		 * \param f is the flag array we're filling
		 * \param id is whether we're masking by ID level
		 * \param ... is the list of flags or ID types we're looking for
		 *
		 * N.B. OFT_MAX must be the last item in the ... list
		 */
		public static void create_mask(Bitflag f, bool id, params object[] vals)
		{
			f.wipe();

			/* Process each type in the va_args */
			for (int i = 0; i < vals.Length; i++) {
				int value = (int)vals[i];
				foreach(Object_Flag of in list) {
					if ((id && of.value == value) || (!id && of.type == (object_flag_type)vals[i]))
			            f.on(of.value);
				}
			    /*for (Object_Flag of_ptr = object_flag_table; of_ptr.index < OF_MAX; of_ptr++)
			        if ((id && of_ptr.id == i) || (!id && of_ptr.type == i))
			            of_on(f, of_ptr.index);*/
			}

			return;
		}

		/**
		 * Print a message when an object flag is identified by use.
		 *
		 * \param flag is the flag being noticed
		 * \param name is the object name 
		 */
		public static void flag_message(int flag, string name)
		{
			Object_Flag of_ptr = Object_Flag.list[flag];

			if (of_ptr.message != null && of_ptr.message != "")
			    Utilities.msg(of_ptr.message, name);

			return;
		}

		/**
		 * Determine whether a flagset includes any curse flags.
		 */
		public static bool cursed_p(Bitflag f)
		{
			Bitflag f2 = new Bitflag(Object_Flag.SIZE);

			f2.wipe();
			create_mask(f2, false, object_flag_type.CURSE);

			return f.is_inter(f2);
		}

		/**
		 * Log the names of a flagset to a file.
		 *
		 * \param f is the set of flags we are logging.
		 * \param log_file is the file to which we are logging the names.
		 */
		//log_file was type ang_file... I don't think we need that anymore...
		public static void log_flags(Bitflag f, FileStream log_file)
		{
			throw new NotImplementedException();
			//int i;

			//file_putf(log_file, "Object flags are:\n");
			//for (i = 0; i < OF_MAX; i++)
			//    if (of_has(f, i))
			//        file_putf(log_file, "%s\n", flag_names[i]);
		}

		/**
		 * Log the name of a flag to a file.
		 *
		 * \param flag is the flag to log.
		 * \param log_file is ... oh come on how obvious does it need to be?
		 */
		public static string flag_name(int flag)
		{
			throw new NotImplementedException();
			//return flag_names[flag];
		}

		/**
		 * Get the slot multiplier for a flag's power rating
		 *
		 * \param flag is the flag in question.
		 * \param slot is the wield_slot it's in.
		 */
		public static int slot_mult(int flag, int slot)
		{
			Object_Flag of_ptr = list[flag];

			switch (slot) {
			    case Misc.INVEN_WIELD: 	return of_ptr.weapon;
			    case Misc.INVEN_BOW:	return of_ptr.bow;
			    case Misc.INVEN_LEFT:
			    case Misc.INVEN_RIGHT:	return of_ptr.ring;
			    case Misc.INVEN_NECK:	return of_ptr.amulet;
			    case Misc.INVEN_LIGHT:	return of_ptr.light;
			    case Misc.INVEN_BODY:	return of_ptr.body;
			    case Misc.INVEN_OUTER:	return of_ptr.cloak;
			    case Misc.INVEN_ARM:	return of_ptr.shield;
			    case Misc.INVEN_HEAD:	return of_ptr.hat;
			    case Misc.INVEN_HANDS:	return of_ptr.gloves;
			    case Misc.INVEN_FEET:	return of_ptr.boots;
			    default: 			return 1;
			}
		}

		/**
		 * Return the base power rating for a flag.
		 */
		public static int flag_power(int flag)
		{
			Object_Flag of_ptr = list[flag];

			return of_ptr.power;
		}

		/**
		 * Ascertain whether a flag is granular (pval-based) or binary.
		 */
		public static bool flag_uses_pval(int flag)
		{
			Object_Flag of_ptr = list[flag];

			return of_ptr.pval;
		}

		/**
		 * Return the OFT_ type of a flag.
		 */
		public static int obj_flag_type(int flag)
		{
			Object_Flag of_ptr = list[flag];

			return (int)of_ptr.type;
		}

		/**
		 * Return the pval weighting of a flag. (Some pvals are more important than
		 * others.)
		 */
		public static int pval_mult(int flag)
		{
			Object_Flag of_ptr = list[flag];

			return of_ptr.p_m;//pval_mult; <--Nick: Assuming p_m = pval_mult...
		}

	}
}
