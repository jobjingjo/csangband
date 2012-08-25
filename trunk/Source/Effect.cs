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
		public int rating;
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
		//    return this.aim; //This is bulshit, making a public getter.
		//}
	}
}
