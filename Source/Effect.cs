using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Effect {
		public static List<Effect> list = new List<Effect>();

		private static int counter = 0;
		public Effect(string name, bool aim, int rating, string desc){
			this.name = name;
			this.aim = aim;
			this.rating = rating;
			this.desc = desc;
			this.value = counter++;

			list.Add(this);
		}

		public string name;
		public bool aim;
		public int rating; //aka power
		public string desc;
		public int value;
		/*
		 * "rating" is the power rating for an item activation, as a damage-per-blow
		 * equivalent (x2). These ratings are used in the calculation of the power (and
		 * therefore cost) of an item which has the effect as an activation, but NOT
		 * for other items (e.g. potions, scrolls). Hence the use of INIHIBIT_POWER.
		 */
		/*     name            aim?   rating	short description	*/
		public static Effect XXX = new Effect("XXX", false,  0,	null);
		public static Effect POISON = new Effect("POISON", false,  0,	"poisons you for 2d7+10 turns");
		public static Effect BLIND = new Effect("BLIND", false,  0,	"blinds you for 4d25+75 turns");
		public static Effect SCARE = new Effect("SCARE", false,  0,	"induces fear in you for 1d10+10 turns");
		public static Effect CONFUSE = new Effect("CONFUSE", false,  0,	"confuses you for 4d5+10 turns");
		public static Effect HALLUC = new Effect("HALLUC", false,  0,	"causes you to hallucinate");
		public static Effect PARALYZE = new Effect("PARALYZE", false,  0,	"induces paralysis for 1d5+5 turns");
		public static Effect SLOW = new Effect("SLOW", false,  0,	"slows you for 1d25+15 turns");
		public static Effect CURE_POISON = new Effect("CURE_POISON", false,  1,	"neutralizes poison");
		public static Effect CURE_BLINDNESS = new Effect("CURE_BLINDNESS", false,  4,	"cures blindness");
		public static Effect CURE_PARANOIA = new Effect("CURE_PARANOIA", false,  2,	"removes your fear");
		public static Effect CURE_CONFUSION = new Effect("CURE_CONFUSION", false,  4,	"cures confusion");
		public static Effect CURE_MIND = new Effect("CURE_MIND", false,  8,	"cures confusion and hallucination, removes fear and grants you temporary resistance to confusion");
		public static Effect CURE_BODY = new Effect("CURE_BODY", false,  7,	"heals cut damage, and cures stunning, poison and blindness");

		public static Effect CURE_LIGHT = new Effect("CURE_LIGHT", false,  3,	"heals 20 hitpoints, some cut damage, makes you a little less confused, and cures blindness");
		public static Effect CURE_SERIOUS = new Effect("CURE_SERIOUS", false,  6,	"heals 40 hitpoints, cut damage, and cures blindness and confusion");
		public static Effect CURE_CRITICAL = new Effect("CURE_CRITICAL", false,  9,	"heals 60 hitpoints, cut damage, and cures stunning, poisoning, blindness, and confusion");
		public static Effect CURE_FULL = new Effect("CURE_FULL", false, 12,	"heals you a really large amount (35% of max HP, minimum 300HP);, heals cut damage, and cures stunning, poisoning, blindness, and confusion");
		public static Effect CURE_FULL2 = new Effect("CURE_FULL2", false, 18,	"restores 1200 hit points, heals cut damage, and cures stunning, poisoning, blindness, and confusion");
		public static Effect CURE_NONORLYBIG = new Effect("CURE_NONORLYBIG", false, 21,	"restores 5000 hit points, restores experience and stats, heals cut damage, and cures stunning, poison, blindness, and confusion");
		public static Effect CURE_TEMP = new Effect("CURE_TEMP", false,  9,	"heals cut damage, and cures all stunning, poison, blindness and confusion");
		public static Effect HEAL1 = new Effect("HEAL1", false, 13,	"heals 500 hit points");
		public static Effect HEAL2 = new Effect("HEAL2", false, 16,	"heals 1000 hit points");
		public static Effect HEAL3 = new Effect("HEAL3", false, 14,	"heals 500 hit points, heals cut damage, and cures stunning");

		public static Effect GAIN_EXP = new Effect("GAIN_EXP", false, Object.Object.INHIBIT_POWER, "grants 100,000 experience points");
		public static Effect LOSE_EXP = new Effect("LOSE_EXP", false,  0,	"drains a quarter of your experience");
		public static Effect RESTORE_EXP = new Effect("RESTORE_EXP", false,  8,	"restores your experience");

		public static Effect RESTORE_MANA = new Effect("RESTORE_MANA", false, 20,	"restores your mana points to maximum");

		public static Effect GAIN_STR = new Effect("GAIN_STR", false, Object.Object.INHIBIT_POWER, "restores and increases your strength");
		public static Effect GAIN_INT = new Effect("GAIN_INT", false, Object.Object.INHIBIT_POWER, "restores and increases your intelligence");
		public static Effect GAIN_WIS = new Effect("GAIN_WIS", false, Object.Object.INHIBIT_POWER, "restores and increases your wisdom");
		public static Effect GAIN_DEX = new Effect("GAIN_DEX", false, Object.Object.INHIBIT_POWER, "restores and increases your dexterity");
		public static Effect GAIN_CON = new Effect("GAIN_CON", false, Object.Object.INHIBIT_POWER, "restores and increases your constitution");
		public static Effect GAIN_CHR = new Effect("GAIN_CHR", false, Object.Object.INHIBIT_POWER, "restores and increases your charisma");
		public static Effect GAIN_ALL = new Effect("GAIN_ALL", false, Object.Object.INHIBIT_POWER, "restores and increases all your stats");
		public static Effect BRAWN = new Effect("BRAWN", false, 30,	"raises your strength at the expense of a random attribute");
		public static Effect INTELLECT = new Effect("INTELLECT", false, 25,	"raises your intelligence at the expense of a random attribute");
		public static Effect CONTEMPLATION = new Effect("CONTEMPLATION", false, 25,	"raises your wisdom at the expense of a random attribute");
		public static Effect TOUGHNESS = new Effect("TOUGHNESS", false, 30,	"raises your constitution at the expense of a random attribute");
		public static Effect NIMBLENESS = new Effect("NIMBLENESS", false, 25,	"raises your dexterity at the expense of a random attribute");
		public static Effect PLEASING = new Effect("PLEASING", false,  5,	"raises your charisma at the expense of a random attribute");
		public static Effect LOSE_STR = new Effect("LOSE_STR", false,  0,	"reduces your strength with damage 5d5");
		public static Effect LOSE_INT = new Effect("LOSE_INT", false,  0,	"reduces your intelligence with damage 5d5");
		public static Effect LOSE_WIS = new Effect("LOSE_WIS", false,  0,	"reduces your wisdom with damage 5d5");
		public static Effect LOSE_DEX = new Effect("LOSE_DEX", false,  0,	"reduces your dexterity with damage 5d5");
		public static Effect LOSE_CON = new Effect("LOSE_CON", false,  0,	"reduces your constitution with damage 5d5");
		public static Effect LOSE_CHR = new Effect("LOSE_CHR", false,  0,	"reduces your intelligence with damage 5d5");
		public static Effect LOSE_CON2 = new Effect("LOSE_CON2", false,  0,	"reduces your constitution with damage 10d10");
		public static Effect RESTORE_STR = new Effect("RESTORE_STR", false, 10,	"restores your strength");
		public static Effect RESTORE_INT = new Effect("RESTORE_INT", false,  8,	"restores your intelligence");
		public static Effect RESTORE_WIS = new Effect("RESTORE_WIS", false,  8,	"restores your wisdom");
		public static Effect RESTORE_DEX = new Effect("RESTORE_DEX", false,  9,	"restores your dexterity");
		public static Effect RESTORE_CON = new Effect("RESTORE_CON", false, 10,	"restores your constitution");
		public static Effect RESTORE_CHR = new Effect("RESTORE_CHR", false,  4,	"restores your charisma");
		public static Effect RESTORE_ALL = new Effect("RESTORE_ALL", false, 15,	"restores all your stats");

		public static Effect RESTORE_ST_LEV = new Effect("RESTORE_ST_LEV", false, 17,	"restores all your stats and your experience points");

		public static Effect TMD_INFRA = new Effect("TMD_INFRA", false,  5,	"extends your infravision by 50 feet for 4d25+100 turns");
		public static Effect TMD_SINVIS = new Effect("TMD_SINVIS", false,  7,	"cures blindness and allows you to see invisible things for 2d6+12 turns");
		public static Effect TMD_ESP = new Effect("TMD_ESP", false, 10,	"cures blindness and gives you telepathy for 6d6+12 turns");

		public static Effect ENLIGHTENMENT = new Effect("ENLIGHTENMENT", false, 22,	"completely lights up and magically maps the level");
		public static Effect ENLIGHTENMENT2 = new Effect("ENLIGHTENMENT2", false, Object.Object.INHIBIT_POWER, "increases your intelligence and wisdom, detects and maps everything in the surrounding area, and identifies all items in your pack");

		public static Effect HERO = new Effect("HERO", false,  7,	"restores 10 hit points, removes fear and grants you resistance to fear and +12 to-hit for 1d25+25 turns");
		public static Effect SHERO = new Effect("SHERO", false,  9,	"restores 30 hit points, removes fear and grants you resistance to fear, +24 to-hit, and -10AC for 1d25+25 turns");

		public static Effect RESIST_ACID = new Effect("RESIST_ACID", false,  4,	"grants temporary resistance to acid for 1d10+10 turns");
		public static Effect RESIST_ELEC = new Effect("RESIST_ELEC", false,  4,	"grants temporary resistance to electricity for 1d10+10 turns");
		public static Effect RESIST_FIRE = new Effect("RESIST_FIRE", false,  4,	"grants temporary resistance to fire for 1d10+10 turns");
		public static Effect RESIST_COLD = new Effect("RESIST_COLD", false,  4,	"grants temporary resistance to cold for 1d10+10 turns");
		public static Effect RESIST_POIS = new Effect("RESIST_POIS", false,  4,	"grants temporary resistance to poison for 1d10+10 turns");
		public static Effect RESIST_ALL = new Effect("RESIST_ALL", false, 10,	"grants temporary resistance to acid, electricity, fire, cold and poison for 1d20+20 turns");

		public static Effect DETECT_TREASURE = new Effect("DETECT_TREASURE", false,  6,	"detects gold and objects nearby");
		public static Effect DETECT_TRAP = new Effect("DETECT_TRAP", false,  6,	"detects traps nearby");
		public static Effect DETECT_DOORSTAIR = new Effect("DETECT_DOORSTAIR", false,  6,	"detects doors and stairs nearby");
		public static Effect DETECT_INVIS = new Effect("DETECT_INVIS", false,  6,	"detects invisible creatures nearby");
		public static Effect DETECT_EVIL = new Effect("DETECT_EVIL", false,  6,	"detects evil creatures nearby");
		public static Effect DETECT_ALL = new Effect("DETECT_ALL", false, 10,	"detects treasure, traps, doors, stairs, and all creatures nearby");

		public static Effect ENCHANT_TOHIT = new Effect("ENCHANT_TOHIT", false, 15,	"attempts to magically enhance a weapon's to-hit bonus");
		public static Effect ENCHANT_TODAM = new Effect("ENCHANT_TODAM", false, 20,	"attempts to magically enhance a weapon's to-dam bonus");
		public static Effect ENCHANT_WEAPON = new Effect("ENCHANT_WEAPON", false, 22,	"attempts to magically enhance a weapon both to-hit and to-dam");
		public static Effect ENCHANT_ARMOR = new Effect("ENCHANT_ARMOR", false, 12,	"attempts to magically enhance a piece of armour");
		public static Effect ENCHANT_ARMOR2 = new Effect("ENCHANT_ARMOR2", false, 15,	"attempts to magically enhance a piece of armour with high chance of success");
		public static Effect RESTORE_ITEM = new Effect("RESTORE_ITEM", false, 10,	"restores an item after disenchantment or damage");
		public static Effect IDENTIFY = new Effect("IDENTIFY", false,  9,	"reveals to you the extent of an item's magical powers");
		public static Effect REMOVE_CURSE = new Effect("REMOVE_CURSE", false,  8,	"removes all ordinary curses from all equipped items");
		public static Effect REMOVE_CURSE2 = new Effect("REMOVE_CURSE2", false, 20,	"removes all curses from all equipped items");
		public static Effect LIGHT = new Effect("LIGHT", false,  4,	"lights up an area and inflicts 2d8 damage on light-sensitive creatures");
		public static Effect SUMMON_MON = new Effect("SUMMON_MON", false,  0,	"summons monsters at the current dungeon level");
		public static Effect SUMMON_UNDEAD = new Effect("SUMMON_UNDEAD", false,  0,	"summons undead monsters at the current dungeon level");
		public static Effect TELE_PHASE = new Effect("TELE_PHASE", false,  5,	"teleports you randomly up to 10 squares away");
		public static Effect TELE_LONG = new Effect("TELE_LONG", false,  6,	"teleports you randomly up to 100 squares away");
		public static Effect TELE_LEVEL = new Effect("TELE_LEVEL", false, 15,	"teleports you one level up or down");
		public static Effect CONFUSING = new Effect("CONFUSING", false,  8,	"causes your next attack upon a monster to confuse it");
		public static Effect MAPPING = new Effect("MAPPING", false, 10,	"maps the area around you");
		public static Effect RUNE = new Effect("RUNE", false, 20,	"inscribes a glyph of warding beneath you, which monsters cannot move onto");

		public static Effect ACQUIRE = new Effect("ACQUIRE", false, Object.Object.INHIBIT_POWER, "creates a good object nearby");
		public static Effect ACQUIRE2 = new Effect("ACQUIRE2", false, Object.Object.INHIBIT_POWER, "creates a few good items nearby");
		public static Effect ANNOY_MON = new Effect("ANNOY_MON", false,  0,	"awakens all nearby sleeping monsters and hastens all monsters within line of sight");
		public static Effect CREATE_TRAP = new Effect("CREATE_TRAP", false,  0,	"creates traps surrounding you");
		public static Effect DESTROY_TDOORS = new Effect("DESTROY_TDOORS", false,  6,	"destroys all traps and doors surrounding you");
		public static Effect RECHARGE = new Effect("RECHARGE", false, 11,	"tries to recharge a wand or staff, destroying the wand or staff on failure");
		public static Effect BANISHMENT = new Effect("BANISHMENT", false, 20,	"removes all non-unique monsters represented by a chosen symbol from the level, dealing you damage in the process");
		public static Effect DARKNESS = new Effect("DARKNESS", false,  0,	"darkens the nearby area and blinds you for 1d5+3 turns");
		public static Effect PROTEVIL = new Effect("PROTEVIL", false,  6,	"grants you protection from evil for 1d25 plus 3 times your character level turns");
		public static Effect SATISFY = new Effect("SATISFY", false,  7,	"magically renders you well-fed, curing any gastrointestinal problems");
		public static Effect CURSE_WEAPON = new Effect("CURSE_WEAPON", false,  0,	"curses your currently wielded melee weapon");
		public static Effect CURSE_ARMOR = new Effect("CURSE_ARMOR", false,  0,	"curses your currently worn body armor");
		public static Effect BLESSING = new Effect("BLESSING", false,  6,	"increases your AC and to-hit bonus for 1d12+6 turns");
		public static Effect BLESSING2 = new Effect("BLESSING2", false,  7,	"increases your AC and to-hit bonus for 1d24+12 turns");
		public static Effect BLESSING3 = new Effect("BLESSING3", false,  8,	"increases your AC and to-hit bonus for 1d48+24 turns");
		public static Effect RECALL = new Effect("RECALL", false, 15,	"returns you from the dungeon or takes you to the dungeon after a short delay");
		public static Effect DEEP_DESCENT = new Effect("DEEP_DESCENT", false, 19,	"teleports you two levels down");

		public static Effect EARTHQUAKES = new Effect("EARTHQUAKES", false,  5,	"causes an earthquake around you");
		public static Effect DESTRUCTION2 = new Effect("DESTRUCTION2", false, 12,	"destroys an area around you in the shape of a circle radius 15, and blinds you for 1d10+10 turns");

		public static Effect LOSHASTE = new Effect("LOSHASTE", false,  0,	"hastes all monsters within line of sight");
		public static Effect LOSSLOW = new Effect("LOSSLOW", false,  7,	"slows all non-unique monsters within line of sight");
		public static Effect LOSSLEEP = new Effect("LOSSLEEP", false,  8,	"sleeps all non-unique creatures within line of sight");
		public static Effect LOSCONF = new Effect("LOSCONF", false, 10,	"confuses all non-unique creatures within line of sight");
		public static Effect LOSKILL = new Effect("LOSKILL", false, 25,	"removes all non-unique monsters within 20 squares, dealing you damage in the process");
		public static Effect ILLUMINATION = new Effect("ILLUMINATION", false,  4,	"lights up the surrounding area, hurting light-sensitive creatures");
		public static Effect CLAIRVOYANCE = new Effect("CLAIRVOYANCE", false, 23,	"maps the entire level and detects nearby objects, traps, doors, and stairs");
		public static Effect PROBING = new Effect("PROBING", false,  8,	"gives you information on the health and abilities of monsters you can see");

		public static Effect HASTE = new Effect("HASTE", false, 10,	"hastens you for 2d10+20 turns");
		public static Effect HASTE1 = new Effect("HASTE1", false, 10,	"hastens you for d20+20 turns");
		public static Effect HASTE2 = new Effect("HASTE2", false, 13,	"hastens you for d75+75 turns");

		public static Effect DISPEL_EVIL = new Effect("DISPEL_EVIL", false, 12,	"deals five times your level's damage to all evil creatures that you can see");
		public static Effect DISPEL_EVIL60 = new Effect("DISPEL_EVIL60", false,  9,	"deals 60 damage to all evil creatures that you can see");
		public static Effect DISPEL_UNDEAD = new Effect("DISPEL_UNDEAD", false,  9,	"deals 60 damage to all undead creatures that you can see");
		public static Effect DISPEL_ALL = new Effect("DISPEL_ALL", false, 11,	"deals 120 damage to all creatures that you can see");

		public static Effect SLEEPII = new Effect("SLEEPII", false,  8,	"puts to sleep the monsters around you");
		public static Effect STAR_BALL = new Effect("STAR_BALL", false, 18,	"fires a ball of electricity in all directions, each one causing 150 damage");
		public static Effect RAGE_BLESS_RESIST = new Effect("RAGE_BLESS_RESIST", false, 21,	"bestows upon you berserk rage, bless, and resistance");
		public static Effect RESTORE_LIFE = new Effect("RESTORE_LIFE", false,  8,	"restores your experience to full");
		public static Effect REM_FEAR_POIS = new Effect("REM_FEAR_POIS", false,  3,	"cures you of fear and poison");
		public static Effect FIREBRAND = new Effect("FIREBRAND", false, 25,	"brands bolts with fire, in an unbalanced fashion");

		public static Effect FIRE_BOLT = new Effect("FIRE_BOLT", true,   5,	"creates a fire bolt with damage 9d8");
		public static Effect FIRE_BOLT2 = new Effect("FIRE_BOLT2", true,   7,	"creates a fire bolt with damage 12d8");
		public static Effect FIRE_BOLT3 = new Effect("FIRE_BOLT3", true,   9,	"creates a fire bolt with damage 16d8");
		public static Effect FIRE_BOLT72 = new Effect("FIRE_BOLT72", true,   9,	"creates a fire ball with damage 72");
		public static Effect FIRE_BALL = new Effect("FIRE_BALL", true,  11,	"creates a fire ball with damage 144");
		public static Effect FIRE_BALL2 = new Effect("FIRE_BALL2", true,  11,	"creates a large fire ball with damage 120");
		public static Effect FIRE_BALL200 = new Effect("FIRE_BALL200", true,  13,	"creates a large fire ball with damage 200");
		public static Effect COLD_BOLT = new Effect("COLD_BOLT", true,   4,	"creates a frost bolt with damage 6d8");
		public static Effect COLD_BOLT2 = new Effect("COLD_BOLT2", true,   7,	"creates a frost bolt with damage 12d8");
		public static Effect COLD_BALL2 = new Effect("COLD_BALL2", true,  13,	"creates a large frost ball with damage 200");
		public static Effect COLD_BALL50 = new Effect("COLD_BALL50", true,   8,	"creates a frost ball with damage 50");
		public static Effect COLD_BALL100 = new Effect("COLD_BALL100", true,  10,	"creates a frost ball with damage 100");
		public static Effect COLD_BALL160 = new Effect("COLD_BALL160", true,  12,	"creates a frost ball with damage 160");
		public static Effect ACID_BOLT = new Effect("ACID_BOLT", true,   4,	"creates an acid bolt with damage 5d8");
		public static Effect ACID_BOLT2 = new Effect("ACID_BOLT2", true,   6,	"creates an acid bolt with damage 10d8");
		public static Effect ACID_BOLT3 = new Effect("ACID_BOLT3", true,   7,	"creates an acid bolt with damage 12d8");
		public static Effect ACID_BALL = new Effect("ACID_BALL", true,  11,	"creates an acid ball with damage 125");
		public static Effect ELEC_BOLT = new Effect("ELEC_BOLT", true,   5,	"creates a lightning bolt (that always beams); with damage 6d6");
		public static Effect ELEC_BALL = new Effect("ELEC_BALL", true,   9,	"creates a lightning ball with damage 64");
		public static Effect ELEC_BALL2 = new Effect("ELEC_BALL2", true,  14,	"creates a large lightning ball with damage 250");

		public static Effect DRAIN_LIFE1 = new Effect("DRAIN_LIFE1", true,   9,	"drains up to 90 hit points of life from a target creature");
		public static Effect DRAIN_LIFE2 = new Effect("DRAIN_LIFE2", true,  10,	"drains up to 120 hit points of life from a target creature");
		public static Effect DRAIN_LIFE3 = new Effect("DRAIN_LIFE3", true,  11,	"drains up to 150 hit points of life from a target creature");
		public static Effect DRAIN_LIFE4 = new Effect("DRAIN_LIFE4", true,  12,	"drains up to 250 hit points of life from a target creature");
		public static Effect MISSILE = new Effect("MISSILE", true,   3,	"fires a magic missile with damage 3d4");
		public static Effect MANA_BOLT = new Effect("MANA_BOLT", true,   7,	"fires a mana bolt with damage 12d8");
		public static Effect BIZARRE = new Effect("BIZARRE", true,  20,	"does bizarre things");
		public static Effect ARROW = new Effect("ARROW", true,  11,	"fires a magical arrow with damage 150");
		public static Effect STINKING_CLOUD = new Effect("STINKING_CLOUD", true,   3,	"fires a stinking cloud with damage 12");
		public static Effect STONE_TO_MUD = new Effect("STONE_TO_MUD", true,   6,	"turns rock into mud");
		public static Effect TELE_OTHER = new Effect("TELE_OTHER", true,  11,	"teleports a target monster away");
		public static Effect CONFUSE2 = new Effect("CONFUSE2", true,   3,	"confuses a target monster");

		public static Effect MON_HEAL = new Effect("MON_HEAL", true,   0,	"heals a single monster 4d6 hit points");
		public static Effect MON_HASTE = new Effect("MON_HASTE", true,   0,	"hastes a single monster");
		public static Effect MON_SLOW = new Effect("MON_SLOW", true,   3,	"attempts to magically slow a single monster");
		public static Effect MON_CONFUSE = new Effect("MON_CONFUSE", true,   3,	"attempts to magically confuse a single monster");
		public static Effect MON_SLEEP = new Effect("MON_SLEEP", true,   3,	"attempts to induce magical sleep in a single monster");
		public static Effect MON_CLONE = new Effect("MON_CLONE", true,   0,	"hastes, heals, and magically duplicates a single monster");
		public static Effect MON_SCARE = new Effect("MON_SCARE", true,   3,	"attempts to induce magical fear in a single monster");

		public static Effect LIGHT_LINE = new Effect("LIGHT_LINE", true,   6,	"lights up part of the dungeon in a straight line");
		public static Effect DISARMING = new Effect("DISARMING", true,   7,	"destroys traps, unlocks doors and reveals all secret doors in a given direction");
		public static Effect TDOOR_DEST = new Effect("TDOOR_DEST", true,   5,	"destroys traps and doors");
		public static Effect POLYMORPH = new Effect("POLYMORPH", true,   7,	"polymorphs a monster into another kind of creature");

		public static Effect STARLIGHT = new Effect("STARLIGHT", false,  5,	"fires a line of light in all directions, each one causing light-sensitive creatures 6d8 damage");
		public static Effect STARLIGHT2 = new Effect("STARLIGHT2", false,  7,	"fires a line of light in all directions, each one causing 10d8 damage");
		public static Effect BERSERKER = new Effect("BERSERKER", false, 10,	"puts you in a berserker rage for d50+50 turns");

		public static Effect WONDER = new Effect("WONDER", true,   9,	"creates random and unpredictable effects");

		public static Effect WAND_BREATH = new Effect("WAND_BREATH", true,  12,	"shoots a large ball of one of the base elements for 120-200 damage");
		public static Effect STAFF_MAGI = new Effect("STAFF_MAGI", false, 20,	"restores both intelligence and manapoints to maximum");
		public static Effect STAFF_HOLY = new Effect("STAFF_HOLY", false, 12,	"inflicts damage on evil creatures you can see, cures 50 hit points, heals all temporary effects and grants you protection from evil");
		public static Effect DRINK_GOOD = new Effect("DRINK_GOOD", false,  0,	null);
		public static Effect DRINK_BREATH = new Effect("DRINK_BREATH", true,   8,	"causes you to breathe either cold or flames for 80 damage");
		public static Effect DRINK_SALT = new Effect("DRINK_SALT", false,  0,	"induces vomiting and paralysis for 4 turns, resulting in severe hunger but also curing poison");
		public static Effect DRINK_DEATH = new Effect("DRINK_DEATH", false,  0,	"inflicts 5000 points of damage");
		public static Effect DRINK_RUIN = new Effect("DRINK_RUIN", false,  0,	"inflicts 10d10 points of damage and decreases all your stats");
		public static Effect DRINK_DETONATE = new Effect("DRINK_DETONATE", false,  0,	"inflicts 50d20 points of damage, severe cuts, and stunning");
		public static Effect FOOD_GOOD = new Effect("FOOD_GOOD", false,  0,	null);
		public static Effect FOOD_WAYBREAD = new Effect("FOOD_WAYBREAD", false,  4,	"restores 4d8 hit points and neutralizes poison");
		public static Effect SHROOM_EMERGENCY = new Effect("SHROOM_EMERGENCY", false, 7,	"grants temporary resistance to fire and cold, cures 200HP, but also makes you hallucinate wildly");
		public static Effect SHROOM_TERROR = new Effect("SHROOM_TERROR", false, 5,	"speeds up you temporarily but also makes you mortally afraid");
		public static Effect SHROOM_STONE = new Effect("SHROOM_STONE", false, 5,	"turns your skin to stone briefly, which grants an extra 40AC but slows you down");
		public static Effect SHROOM_DEBILITY = new Effect("SHROOM_DEBILITY", false, 5,	"restores some mana but also drains either your strength or constitution");
		public static Effect SHROOM_SPRINTING = new Effect("SHROOM_SPRINTING", false, 5,	"hastes you for a while, but then makes you slower for a while afterward");
		public static Effect SHROOM_PURGING = new Effect("SHROOM_PURGING", false, 5,	"makes you very hungry but restores constitution and strength");
		public static Effect RING_ACID = new Effect("RING_ACID", true,  11,	"grants acid resistance for d20+20 turns and creates an acid ball of damage 70");
		public static Effect RING_FLAMES = new Effect("RING_FLAMES", true,  11,	"grants fire resistance for d20+20 turns and creates a fire ball of damage 80");
		public static Effect RING_ICE = new Effect("RING_ICE", true,  11,	"grants cold resistance for d20+20 turns and creates a cold ball of damage 75");
		public static Effect RING_LIGHTNING = new Effect("RING_LIGHTNING", true,  11,	"grants electricity resistance for d20+20 turns and creates a lightning ball of damage 85");

		public static Effect DRAGON_BLUE = new Effect("DRAGON_BLUE", true,   18,	"allows you to breathe lightning for 100 damage");
		public static Effect DRAGON_GREEN = new Effect("DRAGON_GREEN", true,   19,	"allows you to breathe poison gas for 150 damage");
		public static Effect DRAGON_RED = new Effect("DRAGON_RED", true,   20,	"allows you to breathe fire for 200 damage");
		public static Effect DRAGON_MULTIHUED = new Effect("DRAGON_MULTIHUED", true, 20,	"allows you to breathe the elements for 250 damage");
		public static Effect DRAGON_BRONZE = new Effect("DRAGON_BRONZE", true,   Object.Object.INHIBIT_POWER,	"allows you to breathe confusion for 120 damage");
		public static Effect DRAGON_GOLD = new Effect("DRAGON_GOLD", true,   19,	"allows you to breathe sound for 130 damage");
		public static Effect DRAGON_CHAOS = new Effect("DRAGON_CHAOS", true,   23,	"allows you to breathe chaos or disenchantment for 220 damage");
		public static Effect DRAGON_LAW = new Effect("DRAGON_LAW", true,   23,	"allows you to breathe sound/shards for 230 damage");
		public static Effect DRAGON_BALANCE = new Effect("DRAGON_BALANCE", true,   24,	"allows you to breathe balance for 250 damage");
		public static Effect DRAGON_SHINING = new Effect("DRAGON_SHINING", true,   21,	"allows you to breathe light or darkness for 200 damage");
		public static Effect DRAGON_POWER = new Effect("DRAGON_POWER", true,   25,	"allows you to breathe for 300 damage");

		public static Effect TRAP_DOOR = new Effect("TRAP_DOOR", false,  0,	"a trap door which drops you down a level");
		public static Effect TRAP_PIT = new Effect("TRAP_PIT", false,  0,	"a pit trap - the fall might hurt");
		public static Effect TRAP_PIT_SPIKES = new Effect("TRAP_PIT_SPIKES", false, 0,	 "a pit trap, with nasty spikes");
		public static Effect TRAP_PIT_POISON = new Effect("TRAP_PIT_POISON", false, 0,	"a pit trap, with poisoned spikes");
		public static Effect TRAP_RUNE_SUMMON = new Effect("TRAP_RUNE_SUMMON", false, 0,	"a rune which summons monsters");
		public static Effect TRAP_RUNE_TELEPORT = new Effect("TRAP_RUNE_TELEPORT", false, 0,	"a rune which teleports");
		public static Effect TRAP_SPOT_FIRE = new Effect("TRAP_SPOT_FIRE", false,  0,	"a magical fire trap");
		public static Effect TRAP_SPOT_ACID = new Effect("TRAP_SPOT_ACID", false,  0,	"a magical acid trap");
		public static Effect TRAP_DART_SLOW = new Effect("TRAP_DART_SLOW", false,  0,	"a dart which slows movements");
		public static Effect TRAP_DART_LOSE_STR = new Effect("TRAP_DART_LOSE_STR", false, 0,	"a dart which drains strength");
		public static Effect TRAP_DART_LOSE_DEX = new Effect("TRAP_DART_LOSE_DEX", false, 0,	"a dart which drains dexterity");
		public static Effect TRAP_DART_LOSE_CON = new Effect("TRAP_DART_LOSE_CON", false, 0,	"a dart which drains constitution");
		public static Effect TRAP_GAS_BLIND = new Effect("TRAP_GAS_BLIND", false,  0,	"blinding gas");
		public static Effect TRAP_GAS_CONFUSE = new Effect("TRAP_GAS_CONFUSE", false, 0,	"confusing gas");
		public static Effect TRAP_GAS_POISON = new Effect("TRAP_GAS_POISON", false, 0,	"poison gas");
		public static Effect TRAP_GAS_SLEEP = new Effect("TRAP_GAS_SLEEP", false,  0,	"soporific gas");
		static Effect MAX = new Effect("MAX", false, 0, "ERROR!!!");
		
		public int effect_power()
		{
			if (value < 1 || value > MAX.value)
				return 0;

			return value; //I am assuming this is power... TODO: make sure this is correct
			//return effects[effect].power;
		}

		///*
		// * Utility functions
		// */
		//public bool effect_aim()
		//{
		//    return this.aim; //This is bulshit, making the property public.
		//}

		/*
		 * Do an effect, given an object.
		 * Boost is the extent to which skill surpasses difficulty, used as % boost. It
		 * ranges from 0 to 138.
		 */
		public bool effect_do(out bool ident, bool aware, int dir, int beam, int boost)
		{
			int py = Misc.p_ptr.py;
			int px = Misc.p_ptr.px;
			int dam, chance, dur;

			ident = false;

			if (value < 1 || value > Effect.MAX.value)
			{
				Utilities.msg("Bad effect passed to do_effect().  Please report this bug.");
				return false;
			}

			if (this == POISON)
			{
				throw new NotImplementedException();
				//player_inc_timed(p_ptr, TMD_POISONED, damroll(2, 7) + 10, true, true);
				//*ident = true;
				//return true;
			}

			else if (this == BLIND)
			{
				throw new NotImplementedException();
				//player_inc_timed(p_ptr, TMD_BLIND, damroll(4, 25) + 75, true, true);
				//*ident = true;
				//return true;
			}

			else if (this == SCARE)
			{
				throw new NotImplementedException();
				//player_inc_timed(p_ptr, TMD_AFRAID, randint0(10) + 10, true, true);
				//*ident = true;
				//return true;
			}

			else if (this == CONFUSE)
			{
				throw new NotImplementedException();
				//player_inc_timed(p_ptr, TMD_CONFUSED, damroll(4, 5) + 10, true, true);
				//*ident = true;
				//return true;
			}

			else if (this == HALLUC)
			{
				throw new NotImplementedException();
				//player_inc_timed(p_ptr, TMD_IMAGE, randint0(250) + 250, true, true);
				//*ident = true;
				//return true;
			}

			else if (this == PARALYZE)
			{
				throw new NotImplementedException();
				//player_inc_timed(p_ptr, TMD_PARALYZED, randint0(5) + 5, true, true);
				//*ident = true;
				//return true;
			}

			else if (this == SLOW)
			{
				throw new NotImplementedException();
				//if (player_inc_timed(p_ptr, TMD_SLOW, randint1(25) + 15, true, true))
				//    *ident = true;
				//return true;
			}

			else if (this == CURE_POISON)
			{
				throw new NotImplementedException();
				//if (player_clear_timed(p_ptr, TMD_POISONED, true)) *ident = true;
				//return true;
			}

			else if (this == CURE_BLINDNESS)
			{
				throw new NotImplementedException();
				//if (player_clear_timed(p_ptr, TMD_BLIND, true)) *ident = true;
				//return true;
			}

			else if (this == CURE_PARANOIA)
			{
				throw new NotImplementedException();
				//if (player_clear_timed(p_ptr, TMD_AFRAID, true)) *ident = true;
				//return true;
			}

			else if (this == CURE_CONFUSION)
			{
				throw new NotImplementedException();
				//if (player_clear_timed(p_ptr, TMD_CONFUSED, true)) *ident = true;
				//return true;
			}

			else if (this == CURE_MIND)
			{
				throw new NotImplementedException();
				//if (player_clear_timed(p_ptr, TMD_CONFUSED, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_AFRAID, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_IMAGE, true)) *ident = true;
				//if (!of_has(p_ptr.state.flags, OF_RES_CONFU) &&
				//    player_inc_timed(p_ptr, TMD_OPP_CONF, damroll(4, 10), true, true))
				//        *ident = true;
				//return true;
			}

			else if (this == CURE_BODY)
			{
				throw new NotImplementedException();
				//if (player_clear_timed(p_ptr, TMD_STUN, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_CUT, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_POISONED, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_BLIND, true)) *ident = true;
				//return true;
			}


			else if (this == CURE_LIGHT)
			{
				throw new NotImplementedException();
				//if (hp_player(20)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_BLIND, true)) *ident = true;
				//if (player_dec_timed(p_ptr, TMD_CUT, 20, true)) *ident = true;
				//if (player_dec_timed(p_ptr, TMD_CONFUSED, 20, true)) *ident = true;
				//return true;
			}

			else if (this == CURE_SERIOUS)
			{
				throw new NotImplementedException();
				//if (hp_player(40)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_CUT, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_BLIND, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_CONFUSED, true)) *ident = true;
				//return true;
			}

			else if (this == CURE_CRITICAL)
			{
				throw new NotImplementedException();
				//if (hp_player(60)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_BLIND, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_CONFUSED, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_POISONED, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_STUN, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_CUT, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_AMNESIA, true)) *ident = true;
				//return true;
			}

			else if (this == CURE_FULL)
			{
				throw new NotImplementedException();
				//int amt = (p_ptr.mhp * 35) / 100;
				//if (amt < 300) amt = 300;

				//if (hp_player(amt)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_BLIND, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_CONFUSED, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_POISONED, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_STUN, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_CUT, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_AMNESIA, true)) *ident = true;
				//return true;
			}

			else if (this == CURE_FULL2)
			{
				throw new NotImplementedException();
				//if (hp_player(1200)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_BLIND, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_CONFUSED, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_POISONED, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_STUN, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_CUT, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_AMNESIA, true)) *ident = true;
				//return true;
			}

			else if (this == CURE_TEMP)
			{
				throw new NotImplementedException();
				//if (player_clear_timed(p_ptr, TMD_BLIND, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_POISONED, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_CONFUSED, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_STUN, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_CUT, true)) *ident = true;
				//return true;
			}

			else if (this == HEAL1)
			{
				throw new NotImplementedException();
				//if (hp_player(500)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_CUT, true)) *ident = true;
				//return true;
			}

			else if (this == HEAL2)
			{
				throw new NotImplementedException();
				//if (hp_player(1000)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_CUT, true)) *ident = true;
				//return true;
			}

			else if (this == HEAL3)
			{
				throw new NotImplementedException();
				//if (hp_player(500)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_STUN, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_CUT, true)) *ident = true;
				//return true;
			}

			else if (this == GAIN_EXP)
			{
				throw new NotImplementedException();
				//if (p_ptr.exp < PY_MAX_EXP)
				//{
				//    msg("You feel more experienced.");
				//    player_exp_gain(p_ptr, 100000L);
				//    *ident = true;
				//}
				//return true;
			}

			else if (this == LOSE_EXP)
			{
				throw new NotImplementedException();
				//if (!check_state(p_ptr, OF_HOLD_LIFE, p_ptr.state.flags) && (p_ptr.exp > 0))
				//{
				//    msg("You feel your memories fade.");
				//    player_exp_lose(p_ptr, p_ptr.exp / 4, false);
				//    *ident = true;
				//}
				//*ident = true;
				//wieldeds_notice_flag(p_ptr, OF_HOLD_LIFE);
				//return true;
			}

			else if (this == RESTORE_EXP)
			{
				throw new NotImplementedException();
				//if (restore_level()) *ident = true;
				//return true;
			}

			else if (this == RESTORE_MANA)
			{
				throw new NotImplementedException();
				//if (p_ptr.csp < p_ptr.msp)
				//{
				//    p_ptr.csp = p_ptr.msp;
				//    p_ptr.csp_frac = 0;
				//    msg("Your feel your head clear.");
				//    p_ptr.redraw |= (PR_MANA);
				//    *ident = true;
				//}
				//return true;
			}

			else if (this == GAIN_STR ||
					this == GAIN_INT ||
					this == GAIN_WIS ||
					this == GAIN_DEX ||
					this == GAIN_CON ||
					this == GAIN_CHR)
			{
				throw new NotImplementedException();
				//int stat = effect - EF_GAIN_STR;
				//if (do_inc_stat(stat)) *ident = true;
				//return true;
			}

			else if (this == GAIN_ALL)
			{
				throw new NotImplementedException();
				//if (do_inc_stat(A_STR)) *ident = true;
				//if (do_inc_stat(A_INT)) *ident = true;
				//if (do_inc_stat(A_WIS)) *ident = true;
				//if (do_inc_stat(A_DEX)) *ident = true;
				//if (do_inc_stat(A_CON)) *ident = true;
				//if (do_inc_stat(A_CHR)) *ident = true;
				//return true;
			}

			else if (this == BRAWN)
			{
				throw new NotImplementedException();
				///* Pick a random stat to decrease other than strength */
				//int stat = randint0(A_MAX-1) + 1;

				//if (do_dec_stat(stat, true))
				//{
				//    do_inc_stat(A_STR);
				//    *ident = true;
				//}

				//return true;
			}

			else if (this == INTELLECT)
			{
				throw new NotImplementedException();
				///* Pick a random stat to decrease other than intelligence */
				//int stat = randint0(A_MAX-1);
				//if (stat >= A_INT) stat++;

				//if (do_dec_stat(stat, true))
				//{
				//    do_inc_stat(A_INT);
				//    *ident = true;
				//}

				//return true;
			}

			else if (this == CONTEMPLATION)
			{
				throw new NotImplementedException();
				///* Pick a random stat to decrease other than wisdom */
				//int stat = randint0(A_MAX-1);
				//if (stat >= A_WIS) stat++;

				//if (do_dec_stat(stat, true))
				//{
				//    do_inc_stat(A_WIS);
				//    *ident = true;
				//}

				//return true;
			}

			else if (this == TOUGHNESS)
			{
				throw new NotImplementedException();
				///* Pick a random stat to decrease other than constitution */
				//int stat = randint0(A_MAX-1);
				//if (stat >= A_CON) stat++;

				//if (do_dec_stat(stat, true))
				//{
				//    do_inc_stat(A_CON);
				//    *ident = true;
				//}

				//return true;
			}

			else if (this == NIMBLENESS)
			{
				throw new NotImplementedException();
				///* Pick a random stat to decrease other than dexterity */
				//int stat = randint0(A_MAX-1);
				//if (stat >= A_DEX) stat++;

				//if (do_dec_stat(stat, true))
				//{
				//    do_inc_stat(A_DEX);
				//    *ident = true;
				//}

				//return true;
			}

			else if (this == PLEASING)
			{
				throw new NotImplementedException();
				///* Pick a random stat to decrease other than charisma */
				//int stat = randint0(A_MAX-1);

				//if (do_dec_stat(stat, true))
				//{
				//    do_inc_stat(A_CHR);
				//    *ident = true;
				//}

				//return true;
			}

			else if (this == LOSE_STR ||
					this == LOSE_INT ||
					this == LOSE_WIS ||
					this == LOSE_DEX ||
					this == LOSE_CON ||
					this == LOSE_CHR)
			{
				throw new NotImplementedException();
				//int stat = effect - EF_LOSE_STR;

				//take_hit(p_ptr, damroll(5, 5), "stat drain");
				//(void)do_dec_stat(stat, false);
				//*ident = true;

				//return true;
			}

			else if (this == LOSE_CON2)
			{
				throw new NotImplementedException();
				//take_hit(p_ptr, damroll(10, 10), "poisonous food");
				//(void)do_dec_stat(A_CON, false);
				//*ident = true;

				//return true;
			}

			else if (this == RESTORE_STR ||
					this == RESTORE_INT ||
					this == RESTORE_WIS ||
					this == RESTORE_DEX ||
					this == RESTORE_CON ||
					this == RESTORE_CHR)
			{
				throw new NotImplementedException();
				//int stat = effect - EF_RESTORE_STR;
				//if (do_res_stat(stat)) *ident = true;
				//return true;
			}

			else if (this == CURE_NONORLYBIG)
			{
				throw new NotImplementedException();
				//msg("You feel life flow through your body!");
				//restore_level();
				//(void)player_clear_timed(p_ptr, TMD_POISONED, true);
				//(void)player_clear_timed(p_ptr, TMD_BLIND, true);
				//(void)player_clear_timed(p_ptr, TMD_CONFUSED, true);
				//(void)player_clear_timed(p_ptr, TMD_IMAGE, true);
				//(void)player_clear_timed(p_ptr, TMD_STUN, true);
				//(void)player_clear_timed(p_ptr, TMD_CUT, true);
				//(void)player_clear_timed(p_ptr, TMD_AMNESIA, true);

				//if (do_res_stat(A_STR)) *ident = true;
				//if (do_res_stat(A_INT)) *ident = true;
				//if (do_res_stat(A_WIS)) *ident = true;
				//if (do_res_stat(A_DEX)) *ident = true;
				//if (do_res_stat(A_CON)) *ident = true;
				//if (do_res_stat(A_CHR)) *ident = true;

				///* Recalculate max. hitpoints */
				//update_stuff(p_ptr);

				//hp_player(5000);

				//*ident = true;
				//return true;
			}

			else if (this == RESTORE_ALL)
			{
				throw new NotImplementedException();
				///* Life, above, also gives these effects */
				//if (do_res_stat(A_STR)) *ident = true;
				//if (do_res_stat(A_INT)) *ident = true;
				//if (do_res_stat(A_WIS)) *ident = true;
				//if (do_res_stat(A_DEX)) *ident = true;
				//if (do_res_stat(A_CON)) *ident = true;
				//if (do_res_stat(A_CHR)) *ident = true;
				//return true;
			}

			else if (this == RESTORE_ST_LEV)
			{
				throw new NotImplementedException();
				//if (restore_level()) *ident = true;
				//if (do_res_stat(A_STR)) *ident = true;
				//if (do_res_stat(A_INT)) *ident = true;
				//if (do_res_stat(A_WIS)) *ident = true;
				//if (do_res_stat(A_DEX)) *ident = true;
				//if (do_res_stat(A_CON)) *ident = true;
				//if (do_res_stat(A_CHR)) *ident = true;
				//return true;
			}

			else if (this == TMD_INFRA)
			{
				throw new NotImplementedException();
				//if (player_inc_timed(p_ptr, TMD_SINFRA, 100 + damroll(4, 25), true, true))
				//    *ident = true;
				//return true;
			}

			else if (this == TMD_SINVIS)
			{
				throw new NotImplementedException();
				//if (player_clear_timed(p_ptr, TMD_BLIND, true)) *ident = true;
				//if (player_inc_timed(p_ptr, TMD_SINVIS, 12 + damroll(2, 6), true, true))
				//    *ident = true;
				//return true;
			}

			else if (this == TMD_ESP)
			{
				throw new NotImplementedException();
				//if (player_clear_timed(p_ptr, TMD_BLIND, true)) *ident = true;
				//if (player_inc_timed(p_ptr, TMD_TELEPATHY, 12 + damroll(6, 6), true, true))
				//    *ident = true;
				//return true;
			}


			else if (this == ENLIGHTENMENT)
			{
				throw new NotImplementedException();
				//msg("An image of your surroundings forms in your mind...");
				//wiz_light();
				//*ident = true;
				//return true;
			}


			else if (this == ENLIGHTENMENT2)
			{
				throw new NotImplementedException();
				//msg("You begin to feel more enlightened...");
				//message_flush();
				//wiz_light();
				//(void)do_inc_stat(A_INT);
				//(void)do_inc_stat(A_WIS);
				//(void)detect_traps(true);
				//(void)detect_doorstairs(true);
				//(void)detect_treasure(true);
				//identify_pack();
				//*ident = true;
				//return true;
			}

			else if (this == HERO)
			{
				throw new NotImplementedException();
				//dur = randint1(25) + 25;
				//if (hp_player(10)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_AFRAID, true)) *ident = true;
				//if (player_inc_timed(p_ptr, TMD_BOLD, dur, true, true)) *ident = true;
				//if (player_inc_timed(p_ptr, TMD_HERO, dur, true, true)) *ident = true;
				//return true;
			}

			else if (this == SHERO)
			{
				dur = Random.randint1(25) + 25;
				if (Spell.hp_player(30)) ident = true;
				if (Misc.p_ptr.clear_timed(Timed_Effect.AFRAID, true)) ident = true;
				if (Misc.p_ptr.inc_timed(Timed_Effect.BOLD, dur, true, true)) ident = true;
				if (Misc.p_ptr.inc_timed(Timed_Effect.SHERO, dur, true, true)) ident = true;
				return true;
			}


			else if (this == RESIST_ACID)
			{
				throw new NotImplementedException();
				//if (player_inc_timed(p_ptr, TMD_OPP_ACID, randint1(10) + 10, true, true))
				//    *ident = true;
				//return true;
			}

			else if (this == RESIST_ELEC)
			{
				throw new NotImplementedException();
				//if (player_inc_timed(p_ptr, TMD_OPP_ELEC, randint1(10) + 10, true, true))
				//    *ident = true;
				//return true;
			}

			else if (this == RESIST_FIRE)
			{
				throw new NotImplementedException();
				//if (player_inc_timed(p_ptr, TMD_OPP_FIRE, randint1(10) + 10, true, true))
				//    *ident = true;
				//return true;
			}

			else if (this == RESIST_COLD)
			{
				throw new NotImplementedException();
				//if (player_inc_timed(p_ptr, TMD_OPP_COLD, randint1(10) + 10, true, true))
				//    *ident = true;
				//return true;
			}

			else if (this == RESIST_POIS)
			{
				throw new NotImplementedException();
				//if (player_inc_timed(p_ptr, TMD_OPP_POIS, randint1(10) + 10, true, true))
				//    *ident = true;
				//return true;
			}

			else if (this == RESIST_ALL)
			{
				throw new NotImplementedException();
				//if (player_inc_timed(p_ptr, TMD_OPP_ACID, randint1(20) + 20, true, true))
				//    *ident = true;
				//if (player_inc_timed(p_ptr, TMD_OPP_ELEC, randint1(20) + 20, true, true))
				//    *ident = true;
				//if (player_inc_timed(p_ptr, TMD_OPP_FIRE, randint1(20) + 20, true, true))
				//    *ident = true;
				//if (player_inc_timed(p_ptr, TMD_OPP_COLD, randint1(20) + 20, true, true))
				//    *ident = true;
				//if (player_inc_timed(p_ptr, TMD_OPP_POIS, randint1(20) + 20, true, true))
				//    *ident = true;
				//return true;
			}

			else if (this == DETECT_TREASURE)
			{
				throw new NotImplementedException();
				//if (detect_treasure(aware)) *ident = true;
				//return true;
			}

			else if (this == DETECT_TRAP)
			{
				throw new NotImplementedException();
				//if (detect_traps(aware)) *ident = true;
				//return true;
			}

			else if (this == DETECT_DOORSTAIR)
			{
				throw new NotImplementedException();
				//if (detect_doorstairs(aware)) *ident = true;
				//return true;
			}

			else if (this == DETECT_INVIS)
			{
				throw new NotImplementedException();
				//if (detect_monsters_invis(aware)) *ident = true;
				//return true;
			}

			else if (this == DETECT_EVIL)
			{
				throw new NotImplementedException();
				//if (detect_monsters_evil(aware)) *ident = true;
				//return true;
			}

			else if (this == DETECT_ALL)
			{
				throw new NotImplementedException();
				//if (detect_all(aware)) *ident = true;
				//return true;
			}

			else if (this == ENCHANT_TOHIT)
			{
				throw new NotImplementedException();
				//*ident = true;
				//return enchant_spell(1, 0, 0);
			}

			else if (this == ENCHANT_TODAM)
			{
				throw new NotImplementedException();
				//*ident = true;
				//return enchant_spell(0, 1, 0);
			}

			else if (this == ENCHANT_WEAPON)
			{
				throw new NotImplementedException();
				//*ident = true;
				//return enchant_spell(randint1(3), randint1(3), 0);
			}

			else if (this == ENCHANT_ARMOR)
			{
				throw new NotImplementedException();
				//*ident = true;
				//return enchant_spell(0, 0, 1);
			}

			else if (this == ENCHANT_ARMOR2)
			{
				throw new NotImplementedException();
				//*ident = true;
				//return enchant_spell(0, 0, randint1(3) + 2);
			}

			else if (this == RESTORE_ITEM)
			{
				throw new NotImplementedException();
				//*ident = true;
				//return restore_item();
			}

			else if (this == IDENTIFY)
			{
				throw new NotImplementedException();
				//*ident = true;
				//if (!ident_spell()) return false;
				//return true;
			}

			else if (this == REMOVE_CURSE)
			{
				throw new NotImplementedException();
				//if (remove_curse())
				//{
				//    if (!p_ptr.timed[TMD_BLIND])
				//        msg("The air around your body glows blue for a moment...");
				//    else
				//        msg("You feel as if someone is watching over you.");

				//    *ident = true;
				//}
				//return true;
			}

			else if (this == REMOVE_CURSE2)
			{
				throw new NotImplementedException();
				//remove_all_curse();
				//*ident = true;
				//return true;
			}

			else if (this == LIGHT)
			{
				throw new NotImplementedException();
				//if (light_area(damroll(2, 8), 2)) *ident = true;
				//return true;
			}

			else if (this == SUMMON_MON)
			{
				throw new NotImplementedException();
				//int i;
				//sound(MSG_SUM_MONSTER);

				//for (i = 0; i < randint1(3); i++)
				//{
				//    if (summon_specific(py, px, p_ptr.depth, 0, 1))
				//        *ident = true;
				//}
				//return true;
			}

			else if (this == SUMMON_UNDEAD)
			{
				throw new NotImplementedException();
				//int i;
				//sound(MSG_SUM_UNDEAD);

				//for (i = 0; i < randint1(3); i++)
				//{
				//    if (summon_specific(py, px, p_ptr.depth,
				//        S_UNDEAD, 1))
				//        *ident = true;
				//}
				//return true;
			}

			else if (this == TELE_PHASE)
			{
				throw new NotImplementedException();
				//teleport_player(10);
				//*ident = true;
				//return true;
			}

			else if (this == TELE_LONG)
			{
				throw new NotImplementedException();
				//teleport_player(100);
				//*ident = true;
				//return true;
			}

			else if (this == TELE_LEVEL)
			{
				throw new NotImplementedException();
				//(void)teleport_player_level();
				//*ident = true;
				//return true;
			}

			else if (this == CONFUSING)
			{
				throw new NotImplementedException();
				//if (p_ptr.confusing == 0)
				//{
				//    msg("Your hands begin to glow.");
				//    p_ptr.confusing = true;
				//    *ident = true;
				//}
				//return true;
			}

			else if (this == MAPPING)
			{
				throw new NotImplementedException();
				//map_area();
				//*ident = true;
				//return true;
			}

			else if (this == RUNE)
			{
				throw new NotImplementedException();
				//warding_glyph();
				//*ident = true;
				//return true;
			}

			else if (this == ACQUIRE)
			{
				throw new NotImplementedException();
				//acquirement(py, px, p_ptr.depth, 1, true);
				//*ident = true;
				//return true;
			}

			else if (this == ACQUIRE2)
			{
				throw new NotImplementedException();
				//acquirement(py, px, p_ptr.depth, randint1(2) + 1,
				//    true);
				//*ident = true;
				//return true;
			}

			else if (this == ANNOY_MON)
			{
				throw new NotImplementedException();
				//msg("There is a high pitched humming noise.");
				//aggravate_monsters(0);
				//*ident = true;
				//return true;
			}

			else if (this == CREATE_TRAP)
			{
				throw new NotImplementedException();
				///* Hack -- no traps in the town */
				//if (p_ptr.depth == 0)
				//    return true;

				//trap_creation();
				//msg("You hear a low-pitched whistling sound.");
				//*ident = true;
				//return true;
			}

			else if (this == DESTROY_TDOORS)
			{
				throw new NotImplementedException();
				//if (destroy_doors_touch()) *ident = true;
				//return true;
			}

			else if (this == RECHARGE)
			{
				throw new NotImplementedException();
				//*ident = true;
				//if (!recharge(60)) return false;
				//return true;
			}

			else if (this == BANISHMENT)
			{
				throw new NotImplementedException();
				//*ident = true;
				//if (!banishment()) return false;
				//return true;
			}

			else if (this == DARKNESS)
			{
				throw new NotImplementedException();
				//if (!check_state(p_ptr, OF_RES_DARK, p_ptr.state.flags))
				//    (void)player_inc_timed(p_ptr, TMD_BLIND, 3 + randint1(5), true, true);
				//unlight_area(10, 3);
				//wieldeds_notice_flag(p_ptr, OF_RES_DARK);
				//*ident = true;
				//return true;
			}

			else if (this == PROTEVIL)
			{
				throw new NotImplementedException();
				//if (player_inc_timed(p_ptr, TMD_PROTEVIL, randint1(25) + 3 *
				//    p_ptr.lev, true, true)) *ident = true;
				//return true;
			}

			else if (this == SATISFY)
			{
				throw new NotImplementedException();
				//if (player_set_food(p_ptr, PY_FOOD_MAX - 1)) *ident = true;
				//return true;
			}

			else if (this == CURSE_WEAPON)
			{
				throw new NotImplementedException();
				//if (curse_weapon()) *ident = true;
				//return true;
			}

			else if (this == CURSE_ARMOR)
			{
				throw new NotImplementedException();
				//if (curse_armor()) *ident = true;
				//return true;
			}

			else if (this == BLESSING)
			{
				throw new NotImplementedException();
				//if (player_inc_timed(p_ptr, TMD_BLESSED, randint1(12) + 6, true, true))
				//    *ident = true;
				//return true;
			}

			else if (this == BLESSING2)
			{
				throw new NotImplementedException();
				//if (player_inc_timed(p_ptr, TMD_BLESSED, randint1(24) + 12, true, true))
				//    *ident = true;
				//return true;
			}

			else if (this == BLESSING3)
			{
				throw new NotImplementedException();
				//if (player_inc_timed(p_ptr, TMD_BLESSED, randint1(48) + 24, true, true))
				//    *ident = true;
				//return true;
			}

			else if (this == RECALL)
			{
				throw new NotImplementedException();
				//set_recall();
				//*ident = true;
				//return true;
			}

			else if (this == DEEP_DESCENT)
			{
				throw new NotImplementedException();
				//int i, target_depth = p_ptr.depth;
			
				///* Calculate target depth */
				//for (i = 2; i > 0; i--) {
				//    if (is_quest(target_depth)) break;
				//    if (target_depth >= MAX_DEPTH - 1) break;
				
				//    target_depth++;
				//}

				//if (target_depth > p_ptr.depth) {
				//    msgt(MSG_TPLEVEL, "You sink through the floor...");
				//    dungeon_change_level(target_depth);
				//    *ident = true;
				//    return true;
				//} else {
				//    msgt(MSG_TPLEVEL, "You sense a malevolent presence blocking passage to the levels below.");
				//    *ident = true;
				//    return false;
				//}
			}

			else if (this == LOSHASTE)
			{
				throw new NotImplementedException();
				//if (speed_monsters()) *ident = true;
				//return true;
			}

			else if (this == LOSSLEEP)
			{
				throw new NotImplementedException();
				//if (sleep_monsters(aware)) *ident = true;
				//return true;
			}

			else if (this == LOSSLOW)
			{
				throw new NotImplementedException();
				//if (slow_monsters()) *ident = true;
				//return true;
			}

			else if (this == LOSCONF)
			{
				throw new NotImplementedException();
				//if (confuse_monsters(aware)) *ident = true;
				//return true;
			}

			else if (this == LOSKILL)
			{
				throw new NotImplementedException();
				//(void)mass_banishment();
				//*ident = true;
				//return true;
			}

			else if (this == EARTHQUAKES)
			{
				throw new NotImplementedException();
				//earthquake(py, px, 10);
				//*ident = true;
				//return true;
			}

			else if (this == DESTRUCTION2)
			{
				throw new NotImplementedException();
				//destroy_area(py, px, 15, true);
				//*ident = true;
				//return true;
			}

			else if (this == ILLUMINATION)
			{
				throw new NotImplementedException();
				//if (light_area(damroll(2, 15), 3)) *ident = true;
				//return true;
			}

			else if (this == CLAIRVOYANCE)
			{
				throw new NotImplementedException();
				//*ident = true;
				//wiz_light();
				//(void)detect_traps(true);
				//(void)detect_doorstairs(true);
				//return true;
			}

			else if (this == PROBING)
			{
				throw new NotImplementedException();
				//*ident = probing();
				//return true;
			}

			else if (this == STONE_TO_MUD)
			{
				throw new NotImplementedException();
				//if (wall_to_mud(dir)) *ident = true;
				//return true;
			}

			else if (this == CONFUSE2)
			{
				throw new NotImplementedException();
				//*ident = true;
				//confuse_monster(dir, 20, aware);
				//return true;
			}

			else if (this == BIZARRE)
			{
				throw new NotImplementedException();
				//*ident = true;
				//ring_of_power(dir);
				//return true;
			}

			else if (this == STAR_BALL)
			{
				throw new NotImplementedException();
				//int i;
				//*ident = true;
				//for (i = 0; i < 8; i++) fire_ball(GF_ELEC, ddd[i],
				//    (150 * (100 + boost) / 100), 3);
				//return true;
			}

			else if (this == RAGE_BLESS_RESIST)
			{
				throw new NotImplementedException();
				//dur = randint1(50) + 50;
				//*ident = true;
				//(void)hp_player(30);
				//(void)player_clear_timed(p_ptr, TMD_AFRAID, true);
				//(void)player_inc_timed(p_ptr, TMD_BOLD, dur, true, true);
				//(void)player_inc_timed(p_ptr, TMD_SHERO, dur, true, true);
				//(void)player_inc_timed(p_ptr, TMD_BLESSED, randint1(50) + 50, true, true);
				//(void)player_inc_timed(p_ptr, TMD_OPP_ACID, randint1(50) + 50, true, true);
				//(void)player_inc_timed(p_ptr, TMD_OPP_ELEC, randint1(50) + 50, true, true);
				//(void)player_inc_timed(p_ptr, TMD_OPP_FIRE, randint1(50) + 50, true, true);
				//(void)player_inc_timed(p_ptr, TMD_OPP_COLD, randint1(50) + 50, true, true);
				//(void)player_inc_timed(p_ptr, TMD_OPP_POIS, randint1(50) + 50, true, true);
				//return true;
			}

			else if (this == SLEEPII)
			{
				throw new NotImplementedException();
				//*ident = true;
				//sleep_monsters_touch(aware);
				//return true;
			}

			else if (this == RESTORE_LIFE)
			{
				throw new NotImplementedException();
				//*ident = true;
				//restore_level();
				//return true;
			}

			else if (this == MISSILE)
			{
				throw new NotImplementedException();
				//*ident = true;
				//dam = damroll(3, 4) * (100 + boost) / 100;
				//fire_bolt_or_beam(beam, GF_MISSILE, dir, dam);
				//return true;
			}

			else if (this == DISPEL_EVIL)
			{
				throw new NotImplementedException();
				//*ident = true;
				//dam = p_ptr.lev * 5 * (100 + boost) / 100;
				//dispel_evil(dam);
				//return true;
			}

			else if (this == DISPEL_EVIL60)
			{
				throw new NotImplementedException();
				//dam = 60 * (100 + boost) / 100;
				//if (dispel_evil(dam)) *ident = true;
				//return true;
			}

			else if (this == DISPEL_UNDEAD)
			{
				throw new NotImplementedException();
				//dam = 60 * (100 + boost) / 100;
				//if (dispel_undead(dam)) *ident = true;
				//return true;
			}

			else if (this == DISPEL_ALL)
			{
				throw new NotImplementedException();
				//dam = 120 * (100 + boost) / 100;
				//if (dispel_monsters(dam)) *ident = true;
				//return true;
			}

			else if (this == HASTE)
			{
				throw new NotImplementedException();
				//if (!p_ptr.timed[TMD_FAST])
				//{
				//    if (player_set_timed(p_ptr, TMD_FAST, damroll(2, 10) + 20, true)) *ident = true;
				//}
				//else
				//{
				//    (void)player_inc_timed(p_ptr, TMD_FAST, 5, true, true);
				//}

				//return true;
			}

			else if (this == HASTE1)
			{
				throw new NotImplementedException();
				//if (!p_ptr.timed[TMD_FAST])
				//{
				//    if (player_set_timed(p_ptr, TMD_FAST, randint1(20) + 20, true)) *ident = true;
				//}
				//else
				//{
				//    (void)player_inc_timed(p_ptr, TMD_FAST, 5, true, true);
				//}

				//return true;
			}

			else if (this == HASTE2)
			{
				throw new NotImplementedException();
				//if (!p_ptr.timed[TMD_FAST])
				//{
				//    if (player_set_timed(p_ptr, TMD_FAST, randint1(75) + 75, true)) *ident = true;
				//}
				//else
				//{
				//    (void)player_inc_timed(p_ptr, TMD_FAST, 5, true, true);
				//}

				//return true;
			}


			else if (this == FIRE_BOLT)
			{
				throw new NotImplementedException();
				//*ident = true;
				//dam = damroll(9, 8) * (100 + boost) / 100;
				//fire_bolt(GF_FIRE, dir, dam);
				//return true;
			}

			else if (this == FIRE_BOLT2)
			{
				throw new NotImplementedException();
				//dam = damroll(12, 8) * (100 + boost) / 100;
				//fire_bolt_or_beam(beam, GF_FIRE, dir, dam);
				//*ident = true;
				//return true;
			}

			else if (this == FIRE_BOLT3)
			{
				throw new NotImplementedException();
				//dam = damroll(16, 8) * (100 + boost) / 100;
				//fire_bolt_or_beam(beam, GF_FIRE, dir, dam);
				//*ident = true;
				//return true;
			}

			else if (this == FIRE_BOLT72)
			{
				throw new NotImplementedException();
				//dam = 72 * (100 + boost) / 100;
				//*ident = true;
				//fire_ball(GF_FIRE, dir, dam, 2);
				//return true;
			}

			else if (this == FIRE_BALL)
			{
				throw new NotImplementedException();
				//dam = 144 * (100 + boost) / 100;
				//fire_ball(GF_FIRE, dir, dam, 2);
				//*ident = true;
				//return true;
			}

			else if (this == FIRE_BALL2)
			{
				throw new NotImplementedException();
				//dam = 120 * (100 + boost) / 100;
				//*ident = true;
				//fire_ball(GF_FIRE, dir, dam, 3);
				//return true;
			}

			else if (this == FIRE_BALL200)
			{
				throw new NotImplementedException();
				//dam = 200 * (100 + boost) / 100;
				//*ident = true;
				//fire_ball(GF_FIRE, dir, dam, 3);
				//return true;
			}

			else if (this == COLD_BOLT)
			{
				throw new NotImplementedException();
				//dam = damroll(6, 8) * (100 + boost) / 100;
				//*ident = true;
				//fire_bolt_or_beam(beam, GF_COLD, dir, dam);
				//return true;
			}

			else if (this == COLD_BOLT2)
			{
				throw new NotImplementedException();
				//dam = damroll(12, 8) * (100 + boost) / 100;
				//*ident = true;
				//fire_bolt(GF_COLD, dir, dam);
				//return true;
			}

			else if (this == COLD_BALL2)
			{
				throw new NotImplementedException();
				//dam = 200 * (100 + boost) / 100;
				//*ident = true;
				//fire_ball(GF_COLD, dir, dam, 3);
				//return true;
			}

			else if (this == COLD_BALL50)
			{
				throw new NotImplementedException();
				//dam = 50 * (100 + boost) / 100;
				//*ident = true;
				//fire_ball(GF_COLD, dir, dam, 2);
				//return true;
			}

			else if (this == COLD_BALL100)
			{
				throw new NotImplementedException();
				//dam = 100 * (100 + boost) / 100;
				//*ident = true;
				//fire_ball(GF_COLD, dir, dam, 2);
				//return true;
			}

			else if (this == COLD_BALL160)
			{
				throw new NotImplementedException();
				//dam = 160 * (100 + boost) / 100;
				//*ident = true;
				//fire_ball(GF_COLD, dir, dam, 3);
				//return true;
			}

			else if (this == ACID_BOLT)
			{
				throw new NotImplementedException();
				//dam = damroll(5, 8) * (100 + boost) / 100;
				//*ident = true;
				//fire_bolt(GF_ACID, dir, dam);
				//return true;
			}

			else if (this == ACID_BOLT2)
			{
				throw new NotImplementedException();
				//dam = damroll(10, 8) * (100 + boost) / 100;
				//fire_bolt_or_beam(beam, GF_ACID, dir, dam);
				//*ident = true;
				//return true;
			}

			else if (this == ACID_BOLT3)
			{
				throw new NotImplementedException();
				//dam = damroll(12, 8) * (100 + boost) / 100;
				//fire_bolt_or_beam(beam, GF_ACID, dir, dam);
				//*ident = true;
				//return true;
			}

			else if (this == ACID_BALL)
			{
				throw new NotImplementedException();
				//dam = 120 * (100 + boost) / 100;
				//fire_ball(GF_ACID, dir, dam, 2);
				//*ident = true;
				//return true;
			}

			else if (this == ELEC_BOLT)
			{
				throw new NotImplementedException();
				//dam = damroll(6, 6) * (100 + boost) / 100;
				//*ident = true;
				//fire_beam(GF_ELEC, dir, dam);
				//return true;
			}

			else if (this == ELEC_BALL)
			{
				throw new NotImplementedException();
				//dam = 64 * (100 + boost) / 100;
				//fire_ball(GF_ELEC, dir, dam, 2);
				//*ident = true;
				//return true;
			}

			else if (this == ELEC_BALL2)
			{
				throw new NotImplementedException();
				//dam = 250 * (100 + boost) / 100;
				//*ident = true;
				//fire_ball(GF_ELEC, dir, dam, 3);
				//return true;
			}


			else if (this == ARROW)
			{
				throw new NotImplementedException();
				//dam = 150 * (100 + boost) / 100;
				//*ident = true;
				//fire_bolt(GF_ARROW, dir, dam);
				//return true;
			}

			else if (this == REM_FEAR_POIS)
			{
				throw new NotImplementedException();
				//*ident = true;
				//(void)player_clear_timed(p_ptr, TMD_AFRAID, true);
				//(void)player_clear_timed(p_ptr, TMD_POISONED, true);
				//return true;
			}

			else if (this == STINKING_CLOUD)
			{
				throw new NotImplementedException();
				//dam = 12 * (100 + boost) / 100;
				//*ident = true;
				//fire_ball(GF_POIS, dir, dam, 3);
				//return true;
			}


			else if (this == DRAIN_LIFE1)
			{
				throw new NotImplementedException();
				//dam = 90 * (100 + boost) / 100;
				//if (drain_life(dir, dam)) *ident = true;
				//return true;
			}

			else if (this == DRAIN_LIFE2)
			{
				throw new NotImplementedException();
				//dam = 120 * (100 + boost) / 100;
				//if (drain_life(dir, dam)) *ident = true;
				//return true;
			}

			else if (this == DRAIN_LIFE3)
			{
				throw new NotImplementedException();
				//dam = 150 * (100 + boost) / 100;
				//if (drain_life(dir, dam)) *ident = true;
				//return true;
			}

			else if (this == DRAIN_LIFE4)
			{
				throw new NotImplementedException();
				//dam = 250 * (100 + boost) / 100;
				//if (drain_life(dir, dam)) *ident = true;
				//return true;
			}

			else if (this == FIREBRAND)
			{
				throw new NotImplementedException();
				//*ident = true;
				//if (!brand_bolts()) return false;
				//return true;
			}

			else if (this == MANA_BOLT)
			{
				throw new NotImplementedException();
				//dam = damroll(12, 8) * (100 + boost) / 100;
				//fire_bolt(GF_MANA, dir, dam);
				//*ident = true;
				//return true;
			}

			else if (this == MON_HEAL)
			{
				throw new NotImplementedException();
				//if (heal_monster(dir)) *ident = true;
				//return true;
			}

			else if (this == MON_HASTE)
			{
				throw new NotImplementedException();
				//if (speed_monster(dir)) *ident = true;
				//return true;
			}

			else if (this == MON_SLOW)
			{
				throw new NotImplementedException();
				//if (slow_monster(dir)) *ident = true;
				//return true;
			}

			else if (this == MON_CONFUSE)
			{
				throw new NotImplementedException();
				//if (confuse_monster(dir, 10, aware)) *ident = true;
				//return true;
			}

			else if (this == MON_SLEEP)
			{
				throw new NotImplementedException();
				//if (sleep_monster(dir, aware)) *ident = true;
				//return true;
			}

			else if (this == MON_CLONE)
			{
				throw new NotImplementedException();
				//if (clone_monster(dir)) *ident = true;
				//return true;
			}

			else if (this == MON_SCARE)
			{
				throw new NotImplementedException();
				//if (fear_monster(dir, 10, aware)) *ident = true;
				//return true;
			}

			else if (this == LIGHT_LINE)
			{
				throw new NotImplementedException();
				//msg("A line of shimmering blue light appears.");
				//light_line(dir);
				//*ident = true;
				//return true;
			}

			else if (this == TELE_OTHER)
			{
				throw new NotImplementedException();
				//if (teleport_monster(dir)) *ident = true;
				//return true;
			}

			else if (this == DISARMING)
			{
				throw new NotImplementedException();
				//if (disarm_trap(dir)) *ident = true;
				//return true;
			}

			else if (this == TDOOR_DEST)
			{
				throw new NotImplementedException();
				//if (destroy_door(dir)) *ident = true;
				//return true;
			}

			else if (this == POLYMORPH)
			{
				throw new NotImplementedException();
				//if (poly_monster(dir)) *ident = true;
				//return true;
			}

			else if (this == STARLIGHT)
			{
				throw new NotImplementedException();
				//int i;
				//if (!p_ptr.timed[TMD_BLIND])
				//    msg("Light shoots in all directions!");
				//for (i = 0; i < 8; i++) light_line(ddd[i]);
				//*ident = true;
				//return true;
			}

			else if (this == STARLIGHT2)
			{
				throw new NotImplementedException();
				//int k;
				//for (k = 0; k < 8; k++) strong_light_line(ddd[k]);
				//*ident = true;
				//return true;
			}

			else if (this == BERSERKER)
			{
				throw new NotImplementedException();
				//dur = randint1(50) + 50;
				//if (player_inc_timed(p_ptr, TMD_BOLD, dur, true, true)) *ident = true;
				//if (player_inc_timed(p_ptr, TMD_SHERO, dur, true, true)) *ident = true;
				//return true;
			}

			else if (this == WONDER)
			{
				throw new NotImplementedException();
				//if (effect_wonder(dir, randint1(100) + p_ptr.lev / 5,
				//    beam)) *ident = true;
				//return true;
			}

			else if (this == WAND_BREATH)
			{
				throw new NotImplementedException();
				///* table of random ball effects and their damages */
				//const int breath_types[] = {
				//    GF_ACID, 200,
				//    GF_ELEC, 160,
				//    GF_FIRE, 200,
				//    GF_COLD, 160,
				//    GF_POIS, 120
				//};
				///* pick a random (type, damage) tuple in the table */
				//int which = 2 * randint0(sizeof(breath_types) / (2 * sizeof(int)));
				//fire_ball(breath_types[which], dir, breath_types[which + 1], 3);
				//*ident = true;
				//return true;
			}

			else if (this == STAFF_MAGI)
			{
				throw new NotImplementedException();
				//if (do_res_stat(A_INT)) *ident = true;
				//if (p_ptr.csp < p_ptr.msp)
				//{
				//    p_ptr.csp = p_ptr.msp;
				//    p_ptr.csp_frac = 0;
				//    *ident = true;
				//    msg("Your feel your head clear.");
				//    p_ptr.redraw |= (PR_MANA);
				//}
				//return true;
			}

			else if (this == STAFF_HOLY)
			{
				throw new NotImplementedException();
				//dam = 120 * (100 + boost) / 100;
				//if (dispel_evil(dam)) *ident = true;
				//if (player_inc_timed(p_ptr, TMD_PROTEVIL, randint1(25) + 3 *
				//    p_ptr.lev, true, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_POISONED, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_AFRAID, true)) *ident = true;
				//if (hp_player(50)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_STUN, true)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_CUT, true)) *ident = true;
				//return true;
			}

			else if (this == DRINK_BREATH)
			{
				throw new NotImplementedException();
				//const int breath_types[] =
				//{
				//    GF_FIRE, 80,
				//    GF_COLD, 80,
				//};

				//int which = 2 * randint0(N_ELEMENTS(breath_types) / 2);
				//fire_ball(breath_types[which], dir, breath_types[which + 1], 2);
				//*ident = true;
				//return true;
			}

			else if (this == DRINK_GOOD)
			{
				throw new NotImplementedException();
				//msg("You feel less thirsty.");
				//*ident = true;
				//return true;
			}

			else if (this == DRINK_DEATH)
			{
				throw new NotImplementedException();
				//msg("A feeling of Death flows through your body.");
				//take_hit(p_ptr, 5000, "a potion of Death");
				//*ident = true;
				//return true;
			}

			else if (this == DRINK_RUIN)
			{
				throw new NotImplementedException();
				//msg("Your nerves and muscles feel weak and lifeless!");
				//take_hit(p_ptr, damroll(10, 10), "a potion of Ruination");
				//player_stat_dec(p_ptr, A_DEX, true);
				//player_stat_dec(p_ptr, A_WIS, true);
				//player_stat_dec(p_ptr, A_CON, true);
				//player_stat_dec(p_ptr, A_STR, true);
				//player_stat_dec(p_ptr, A_CHR, true);
				//player_stat_dec(p_ptr, A_INT, true);
				//*ident = true;
				//return true;
			}

			else if (this == DRINK_DETONATE)
			{
				throw new NotImplementedException();
				//msg("Massive explosions rupture your body!");
				//take_hit(p_ptr, damroll(50, 20), "a potion of Detonation");
				//(void)player_inc_timed(p_ptr, TMD_STUN, 75, true, true);
				//(void)player_inc_timed(p_ptr, TMD_CUT, 5000, true, true);
				//*ident = true;
				//return true;
			}

			else if (this == DRINK_SALT)
			{
				throw new NotImplementedException();
				//msg("The potion makes you vomit!");
				//player_set_food(p_ptr, PY_FOOD_STARVE - 1);
				//(void)player_clear_timed(p_ptr, TMD_POISONED, true);
				//(void)player_inc_timed(p_ptr, TMD_PARALYZED, 4, true, false);
				//*ident = true;
				//return true;
			}

			else if (this == FOOD_GOOD)
			{
				Utilities.msg("That tastes good.");
				ident = true;
				return true;
			}

			else if (this == FOOD_WAYBREAD)
			{
				throw new NotImplementedException();
				//msg("That tastes good.");
				//(void)player_clear_timed(p_ptr, TMD_POISONED, true);
				//(void)hp_player(damroll(4, 8));
				//*ident = true;
				//return true;
			}

			else if (this == SHROOM_EMERGENCY)
			{
				throw new NotImplementedException();
				//(void)player_set_timed(p_ptr, TMD_IMAGE, rand_spread(250, 50), true);
				//(void)player_set_timed(p_ptr, TMD_OPP_FIRE, rand_spread(30, 10), true);
				//(void)player_set_timed(p_ptr, TMD_OPP_COLD, rand_spread(30, 10), true);
				//(void)hp_player(200);
				//*ident = true;
				//return true;
			}

			else if (this == SHROOM_TERROR)
			{
				throw new NotImplementedException();
				//if (player_set_timed(p_ptr, TMD_TERROR, rand_spread(100, 20), true))
				//    *ident = true;
				//return true;
			}

			else if (this == SHROOM_STONE)
			{
				throw new NotImplementedException();
				//if (player_set_timed(p_ptr, TMD_STONESKIN, rand_spread(80, 20), true))
				//    *ident = true;
				//return true;
			}

			else if (this == SHROOM_DEBILITY)
			{
				throw new NotImplementedException();
				//int stat = one_in_(2) ? A_STR : A_CON;

				//if (p_ptr.csp < p_ptr.msp)
				//{
				//    p_ptr.csp = p_ptr.msp;
				//    p_ptr.csp_frac = 0;
				//    msg("Your feel your head clear.");
				//    p_ptr.redraw |= (PR_MANA);
				//    *ident = true;
				//}

				//(void)do_dec_stat(stat, false);

				//*ident = true;
				//return true;
			}

			else if (this == SHROOM_SPRINTING)
			{
				throw new NotImplementedException();
				//if (player_inc_timed(p_ptr, TMD_SPRINT, 100, true, true)) *ident = true;
				//return true;
			}

			else if (this == SHROOM_PURGING)
			{
				throw new NotImplementedException();
				//player_set_food(p_ptr, PY_FOOD_FAINT - 1);
				//if (do_res_stat(A_STR)) *ident = true;
				//if (do_res_stat(A_CON)) *ident = true;
				//if (player_clear_timed(p_ptr, TMD_POISONED, true)) *ident = true;
				//return true;
			}

			else if (this == RING_ACID)
			{
				throw new NotImplementedException();
				//dam = 70 * (100 + boost) / 100;
				//*ident = true;
				//fire_ball(GF_ACID, dir, dam, 2);
				//player_inc_timed(p_ptr, TMD_OPP_ACID, randint1(20) + 20, true, true);
				//return true;
			}

			else if (this == RING_FLAMES)
			{
				throw new NotImplementedException();
				//dam = 80 * (100 + boost) / 100;
				//*ident = true;
				//fire_ball(GF_FIRE, dir, dam, 2);
				//player_inc_timed(p_ptr, TMD_OPP_FIRE, randint1(20) + 20, true, true);
				//return true;
			}

			else if (this == RING_ICE)
			{
				throw new NotImplementedException();
				//dam = 75 * (100 + boost) / 100;
				//*ident = true;
				//fire_ball(GF_COLD, dir, dam, 2);
				//player_inc_timed(p_ptr, TMD_OPP_COLD, randint1(20) + 20, true, true);
				//return true;
			}

			else if (this == RING_LIGHTNING)
			{
				throw new NotImplementedException();
				//dam = 85 * (100 + boost) / 100;
				//*ident = true;
				//fire_ball(GF_ELEC, dir, dam, 2);
				//player_inc_timed(p_ptr, TMD_OPP_ELEC, randint1(20) + 20, true, true);
				//return true;
			}

			else if (this == DRAGON_BLUE)
			{
				throw new NotImplementedException();
				//dam = 100 * (100 + boost) / 100;
				//msgt(MSG_BR_ELEC, "You breathe lightning.");
				//fire_ball(GF_ELEC, dir, dam, 2);
				//return true;
			}

			else if (this == DRAGON_GREEN)
			{
				throw new NotImplementedException();
				//dam = 150 * (100 + boost) / 100;
				//msgt(MSG_BR_GAS, "You breathe poison gas.");
				//fire_ball(GF_POIS, dir, dam, 2);
				//return true;
			}

			else if (this == DRAGON_RED)
			{
				throw new NotImplementedException();
				//dam = 200 * (100 + boost) / 100;
				//msgt(MSG_BR_FIRE, "You breathe fire.");
				//fire_ball(GF_FIRE, dir, dam, 2);
				//return true;
			}

			else if (this == DRAGON_MULTIHUED)
			{
				throw new NotImplementedException();
				//class
				//{
				//    int msg_sound;
				//    const char *msg;
				//    int typ;
				//} mh[] =
				//{
				//    { MSG_BR_ELEC,  "lightning",  GF_ELEC },
				//    { MSG_BR_FROST, "frost",      GF_COLD },
				//    { MSG_BR_ACID,  "acid",       GF_ACID },
				//    { MSG_BR_GAS,   "poison gas", GF_POIS },
				//    { MSG_BR_FIRE,  "fire",       GF_FIRE }
				//};

				//int chance = randint0(5);
				//dam = 250 * (100 + boost) / 100;
				//msgt(mh[chance].msg_sound, "You breathe %s.", mh[chance].msg);
				//fire_ball(mh[chance].typ, dir, dam, 2);
				//return true;
			}

			else if (this == DRAGON_BRONZE)
			{
				throw new NotImplementedException();
				//dam = 120 * (100 + boost) / 100;
				//msgt(MSG_BR_CONF, "You breathe confusion.");
				//fire_ball(GF_CONFU, dir, dam, 2);
				//return true;
			}

			else if (this == DRAGON_GOLD)
			{
				throw new NotImplementedException();
				//dam = 130 * (100 + boost) / 100;
				//msgt(MSG_BR_SOUND, "You breathe sound.");
				//fire_ball(GF_SOUND, dir, dam, 2);
				//return true;
			}

			else if (this == DRAGON_CHAOS)
			{
				throw new NotImplementedException();
				//dam = 220 * (100 + boost) / 100;
				//chance = randint0(2);
				//msgt((chance == 1 ? MSG_BR_CHAOS : MSG_BR_DISEN),
				//        "You breathe %s.",
				//        ((chance == 1 ? "chaos" : "disenchantment")));
				//fire_ball((chance == 1 ? GF_CHAOS : GF_DISEN),
				//            dir, dam, 2);
				//return true;
			}

			else if (this == DRAGON_LAW)
			{
				throw new NotImplementedException();
				//dam = 230 * (100 + boost) / 100;
				//chance = randint0(2);
				//msgt((chance == 1 ? MSG_BR_SOUND : MSG_BR_SHARDS), "You breathe %s.",
				//            ((chance == 1 ? "sound" : "shards")));
				//fire_ball((chance == 1 ? GF_SOUND : GF_SHARD),
				//            dir, dam, 2);
				//return true;
			}

			else if (this == DRAGON_BALANCE)
			{
				throw new NotImplementedException();
				//dam = 250 * (100 + boost) / 100;
				//chance = randint0(4);
				//msg("You breathe %s.",
				//            ((chance == 1) ? "chaos" :
				//            ((chance == 2) ? "disenchantment" :
				//                ((chance == 3) ? "sound" : "shards"))));
				//fire_ball(((chance == 1) ? GF_CHAOS :
				//            ((chance == 2) ? GF_DISEN :
				//            ((chance == 3) ? GF_SOUND : GF_SHARD))),
				//            dir, dam, 2);
				//return true;
			}

			else if (this == DRAGON_SHINING)
			{
				throw new NotImplementedException();
				//dam = 200 * (100 + boost) / 100;
				//chance = randint0(2);
				//msgt((chance == 0 ? MSG_BR_LIGHT : MSG_BR_DARK), "You breathe %s.",
				//        ((chance == 0 ? "light" : "darkness")));
				//fire_ball((chance == 0 ? GF_LIGHT : GF_DARK), dir, dam,
				//    2);
				//return true;
			}

			else if (this == DRAGON_POWER)
			{
				throw new NotImplementedException();
				//dam = 300 * (100 + boost) / 100;
				//msgt(MSG_BR_ELEMENTS, "You breathe the elements.");
				//fire_ball(GF_MISSILE, dir, dam, 2);
				//return true;
			}

			else if (this == TRAP_DOOR)
			{
				throw new NotImplementedException();
				//msg("You fall through a trap door!");
				//if (check_state(p_ptr, OF_FEATHER, p_ptr.state.flags)) {
				//    msg("You float gently down to the next level.");
				//} else {
				//    take_hit(p_ptr, damroll(2, 8), "a trap");
				//}
				//wieldeds_notice_flag(p_ptr, OF_FEATHER);

				//dungeon_change_level(p_ptr.depth + 1);
				//return true;
			}

			else if (this == TRAP_PIT)
			{
				throw new NotImplementedException();
				//msg("You fall into a pit!");
				//if (check_state(p_ptr, OF_FEATHER, p_ptr.state.flags)) {
				//    msg("You float gently to the bottom of the pit.");
				//} else {
				//    take_hit(p_ptr, damroll(2, 6), "a trap");
				//}
				//wieldeds_notice_flag(p_ptr, OF_FEATHER);
				//return true;
			}

			else if (this == TRAP_PIT_SPIKES)
			{
				throw new NotImplementedException();
				//msg("You fall into a spiked pit!");

				//if (check_state(p_ptr, OF_FEATHER, p_ptr.state.flags)) {
				//    msg("You float gently to the floor of the pit.");
				//    msg("You carefully avoid touching the spikes.");
				//} else {
				//    int dam = damroll(2, 6);

				//    /* Extra spike damage */
				//    if (one_in_(2)) {
				//        msg("You are impaled!");
				//        dam *= 2;
				//        (void)player_inc_timed(p_ptr, TMD_CUT, randint1(dam), true, true);
				//    }

				//    take_hit(p_ptr, dam, "a trap");
				//}
				//wieldeds_notice_flag(p_ptr, OF_FEATHER);
				//return true;
			}

			else if (this == TRAP_PIT_POISON)
			{
				throw new NotImplementedException();
				//msg("You fall into a spiked pit!");

				//if (check_state(p_ptr, OF_FEATHER, p_ptr.state.flags)) {
				//    msg("You float gently to the floor of the pit.");
				//    msg("You carefully avoid touching the spikes.");
				//} else {
				//    int dam = damroll(2, 6);

				//    /* Extra spike damage */
				//    if (one_in_(2)) {
				//        msg("You are impaled on poisonous spikes!");
				//        (void)player_inc_timed(p_ptr, TMD_CUT, randint1(dam * 2), true, true);
				//        (void)player_inc_timed(p_ptr, TMD_POISONED, randint1(dam * 4), true, true);
				//    }

				//    take_hit(p_ptr, dam, "a trap");
				//}
				//wieldeds_notice_flag(p_ptr, OF_FEATHER);
				//return true;
			}

			else if (this == TRAP_RUNE_SUMMON)
			{
				throw new NotImplementedException();
				//int i;
				//int num = 2 + randint1(3);

				//msgt(MSG_SUM_MONSTER, "You are enveloped in a cloud of smoke!");

				///* Remove trap */
				//cave.info[py][px] &= ~(CAVE_MARK);
				//cave_set_feat(cave, py, px, FEAT_FLOOR);

				//for (i = 0; i < num; i++)
				//    (void)summon_specific(py, px, p_ptr.depth, 0, 1);

				//return true;
			}

			else if (this == TRAP_RUNE_TELEPORT)
			{
				Utilities.msg("You hit a teleport trap!");
				Spell.teleport_player(100);
				return true;
			}

			else if (this == TRAP_SPOT_FIRE)
			{
				throw new NotImplementedException();
				//int dam;

				//msg("You are enveloped in flames!");
				//dam = damroll(4, 6);
				//dam = adjust_dam(p_ptr, GF_FIRE, dam, RANDOMISE,
				//        check_for_resist(p_ptr, GF_FIRE, p_ptr.state.flags, true));
				//if (dam) {
				//    take_hit(p_ptr, dam, "a fire trap");
				//    inven_damage(p_ptr, GF_FIRE, MIN(dam * 5, 300));
				//}
				//return true;
			}

			else if (this == TRAP_SPOT_ACID)
			{
				throw new NotImplementedException();
				//int dam;

				//msg("You are splashed with acid!");
				//dam = damroll(4, 6);
				//dam = adjust_dam(p_ptr, GF_ACID, dam, RANDOMISE,
				//        check_for_resist(p_ptr, GF_ACID, p_ptr.state.flags, true));
				//if (dam) {
				//    take_hit(p_ptr, dam, "an acid trap");
				//    inven_damage(p_ptr, GF_ACID, MIN(dam * 5, 300));
				//}
				//return true;
			}

			else if (this == TRAP_DART_SLOW)
			{
				throw new NotImplementedException();
				//if (trap_check_hit(125)) {
				//    msg("A small dart hits you!");
				//    take_hit(p_ptr, damroll(1, 4), "a trap");
				//    (void)player_inc_timed(p_ptr, TMD_SLOW, randint0(20) + 20, true, false);
				//} else {
				//    msg("A small dart barely misses you.");
				//}
				//return true;
			}

			else if (this == TRAP_DART_LOSE_STR)
			{
				throw new NotImplementedException();
				//if (trap_check_hit(125)) {
				//    msg("A small dart hits you!");
				//    take_hit(p_ptr, damroll(1, 4), "a trap");
				//    (void)do_dec_stat(A_STR, false);
				//} else {
				//    msg("A small dart barely misses you.");
				//}
				//return true;
			}

			else if (this == TRAP_DART_LOSE_DEX)
			{
				throw new NotImplementedException();
				//if (trap_check_hit(125)) {
				//    msg("A small dart hits you!");
				//    take_hit(p_ptr, damroll(1, 4), "a trap");
				//    (void)do_dec_stat(A_DEX, false);
				//} else {
				//    msg("A small dart barely misses you.");
				//}
				//return true;
			}

			else if (this == TRAP_DART_LOSE_CON)
			{
				throw new NotImplementedException();
				//if (trap_check_hit(125)) {
				//    msg("A small dart hits you!");
				//    take_hit(p_ptr, damroll(1, 4), "a trap");
				//    (void)do_dec_stat(A_CON, false);
				//} else {
				//    msg("A small dart barely misses you.");
				//}
				//return true;
			}

			else if (this == TRAP_GAS_BLIND)
			{
				throw new NotImplementedException();
				//msg("You are surrounded by a black gas!");
				//(void)player_inc_timed(p_ptr, TMD_BLIND, randint0(50) + 25, true, true);
				//return true;
			}

			else if (this == TRAP_GAS_CONFUSE)
			{
				Utilities.msg("You are surrounded by a gas of scintillating colors!");
				Misc.p_ptr.inc_timed(Timed_Effect.CONFUSED, Random.randint0(20) + 10, true, true);
				return true;
			}

			else if (this == TRAP_GAS_POISON)
			{
				throw new NotImplementedException();
				//msg("You are surrounded by a pungent green gas!");
				//(void)player_inc_timed(p_ptr, TMD_POISONED, randint0(20) + 10, true, true);
				//return true;
			}

			else if (this == TRAP_GAS_SLEEP)
			{
				throw new NotImplementedException();
				//msg("You are surrounded by a strange white mist!");
				//(void)player_inc_timed(p_ptr, TMD_PARALYZED, randint0(10) + 5, true, true);
				//return true;
			}


			else if (this == XXX || this == MAX){
				throw new NotImplementedException();
				//break;
			}

			//we probably forgot to throw one earlier...
			throw new NotImplementedException();
			
			/* Not used */
			Utilities.msg("Effect not handled.");
			return false;
		}

		public bool obvious()
		{
			if (this == IDENTIFY)
				return true;

			return false;
		}
	}
}
