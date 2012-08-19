using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Monster {
	class Eval {
		public static int tot_mon_power;

		static long blow_effect(Monster_Blow.RBE effect, int atk_dam, int rlev) {
			/*other bad effects - minor*/
			if(effect == Monster_Blow.RBE.EAT_GOLD ||
				effect == Monster_Blow.RBE.EAT_ITEM ||
				effect == Monster_Blow.RBE.EAT_FOOD ||
				effect == Monster_Blow.RBE.EAT_LIGHT ||
				effect == Monster_Blow.RBE.LOSE_CHR) {
				
				atk_dam += 5;

			/*other bad effects - poison / disease */
			} else if(effect == Monster_Blow.RBE.POISON) {

				atk_dam *= 5;
			    atk_dam /= 4;
			    atk_dam += rlev;

			/*other bad effects - elements / sustains*/
			} else if (	effect ==  Monster_Blow.RBE.TERRIFY ||
						effect == Monster_Blow.RBE.ELEC ||
						effect == Monster_Blow.RBE.COLD ||
						effect == Monster_Blow.RBE.FIRE){
			    
				atk_dam += 10;
			    
			/*other bad effects - elements / major*/
			} else if (	effect == Monster_Blow.RBE.ACID ||
						effect == Monster_Blow.RBE.BLIND ||
						effect == Monster_Blow.RBE.CONFUSE ||
						effect == Monster_Blow.RBE.LOSE_STR ||
						effect == Monster_Blow.RBE.LOSE_INT ||
						effect == Monster_Blow.RBE.LOSE_WIS ||
						effect == Monster_Blow.RBE.LOSE_DEX ||
						effect == Monster_Blow.RBE.HALLU){

			    atk_dam += 20;

			/*other bad effects - major*/
			} else if (effect ==  Monster_Blow.RBE.UN_BONUS ||
						effect ==  Monster_Blow.RBE.UN_POWER ||
						effect ==  Monster_Blow.RBE.LOSE_CON){
			        atk_dam += 30;
			
			/*other bad effects - major*/
			} else if (effect ==  Monster_Blow.RBE.PARALYZE ||
			    effect ==  Monster_Blow.RBE.LOSE_ALL)
			    {
			        atk_dam += 40;
			        
			    } else if (
			    /* Experience draining attacks */
			    effect ==  Monster_Blow.RBE.EXP_10 ||
			    effect ==  Monster_Blow.RBE.EXP_20)
			    {
			        /* change inspired by Eddie because exp is infinite */
			        atk_dam += 5;
			    } else if (
			    effect ==  Monster_Blow.RBE.EXP_40 ||
			    effect ==  Monster_Blow.RBE.EXP_80)
			    {
			        /* as above */
			        atk_dam += 10;
			    }
			    /*Earthquakes*/
			    else if (effect ==  Monster_Blow.RBE.SHATTER)
			    {
			        atk_dam += 300;
			    }

			return atk_dam;
		}

		static long max_dam(Monster_Race r_ptr) {
			int rlev, i;
			int melee_dam = 0, atk_dam = 0, spell_dam = 0;
			int dam = 1;

			/* Extract the monster level, force 1 for town monsters */
			rlev = ((r_ptr.level >= 1) ? r_ptr.level : 1);

			/* Assume single resist for the elemental attacks */
			spell_dam = Monster_Spell_Flag.best_spell_power(r_ptr, 1);

			/* Hack - Apply over 10 rounds */
			spell_dam *= 10;

			/* Scale for frequency and availability of mana / ammo */
			if(spell_dam != 0) {
				int freq = r_ptr.freq_spell;

				/* Hack -- always get 1 shot */
				if(freq < 10)
					freq = 10;

				/* Adjust for frequency */
				spell_dam = spell_dam * freq / 100;
			}

			/* Check attacks */
			for(i = 0; i < 4; i++) {
				/* Extract the attack infomation */
				Monster_Blow.RBE effect = r_ptr.blow[i].effect;
				Monster_Blow.RBM method = r_ptr.blow[i].method;
				int d_dice = r_ptr.blow[i].d_dice;
				int d_side = r_ptr.blow[i].d_side;

				/* Hack -- no more attacks */
				if(method == null)
					continue;

				/* Assume maximum damage*/
				atk_dam = (int)blow_effect(effect, d_dice * d_side, r_ptr.level);

				/*stun definitely most dangerous*/
				if(method == Monster_Blow.RBM.PUNCH || method == Monster_Blow.RBM.KICK ||
				   method == Monster_Blow.RBM.BUTT || method == Monster_Blow.RBM.CRUSH) {
					atk_dam *= 4;
					atk_dam /= 3;
				} else if(method == Monster_Blow.RBM.CLAW || method == Monster_Blow.RBM.BITE) {
					atk_dam *= 7;
					atk_dam /= 5;
				}


				/* Normal melee attack */
				if(!r_ptr.flags.has(Monster_Flag.NEVER_BLOW.value)) {
					/* Keep a running total */
					melee_dam += atk_dam;
				}
			}

			/* 
			 * Apply damage over 10 rounds. We assume that the monster has to make contact first.
			 * Hack - speed has more impact on melee as has to stay in contact with player.
			 * Hack - this is except for pass wall and kill wall monsters which can always get to the player.
			 * Hack - use different values for huge monsters as they strike out to range 2.
			 */
			if(r_ptr.flags.test(Monster_Flag.SIZE, Monster_Flag.KILL_WALL.value, Monster_Flag.PASS_WALL.value))
				melee_dam *= 10;
			else {
				melee_dam = melee_dam * 3 + melee_dam * Misc.extract_energy[r_ptr.speed + (r_ptr.spell_flags.has(Monster_Spell_Flag.HASTE.value) ? 5 : 0)] / 7;
			}

			/*
			 * Scale based on attack accuracy. We make a massive number of assumptions here and just use monster level.
			 */
			melee_dam = melee_dam * Math.Min(45 + rlev * 3, 95) / 100;

			/* Hack -- Monsters that multiply ignore the following reductions */
			if(!r_ptr.flags.has(Monster_Flag.MULTIPLY.value)) {
				/*Reduce damamge potential for monsters that move randomly */
				if(r_ptr.flags.test(Monster_Flag.SIZE, Monster_Flag.RAND_25.value, Monster_Flag.RAND_50.value)) {
					int reduce = 100;

					if(r_ptr.flags.has(Monster_Flag.RAND_25.value))
						reduce -= 25;
					if(r_ptr.flags.has(Monster_Flag.RAND_50.value))
						reduce -= 50;

					/*even moving randomly one in 8 times will hit the player*/
					reduce += (100 - reduce) / 8;

					/* adjust the melee damage*/
					melee_dam = (melee_dam * reduce) / 100;
				}

				/*monsters who can't move aren't nearly as much of a combat threat*/
				if(r_ptr.flags.has(Monster_Flag.NEVER_MOVE.value)) {
					if(r_ptr.spell_flags.has(Monster_Spell_Flag.TELE_TO.value) ||
						r_ptr.spell_flags.has(Monster_Spell_Flag.BLINK.value)) {
						/* Scale for frequency */
						melee_dam = melee_dam / 5 + 4 * melee_dam * r_ptr.freq_spell / 500;

						/* Incorporate spell failure chance */
						if(!r_ptr.flags.has(Monster_Flag.STUPID.value))
							melee_dam = melee_dam / 5 + 4 * melee_dam * Math.Min(75 + (rlev + 3) / 4, 100) / 500;
					} else if(r_ptr.flags.has(Monster_Flag.INVISIBLE.value))
						melee_dam /= 3;
					else
						melee_dam /= 5;
				}
			}

			/* But keep at a minimum */
			if(melee_dam < 1)
				melee_dam = 1;

			/*
			 * Combine spell and melee damage
			 */
			dam = (spell_dam + melee_dam);

			r_ptr.highest_threat = (short)dam;
			r_ptr.spell_dam = spell_dam;	/*AMF:DEBUG*/
			r_ptr.melee_dam = melee_dam;	/*AMF:DEBUG*/

			/*
			 * Adjust for speed.  Monster at speed 120 will do double damage,
			 * monster at speed 100 will do half, etc.  Bonus for monsters who can haste self.
			 */
			dam = (dam * Misc.extract_energy[r_ptr.speed + (r_ptr.spell_flags.has(Monster_Spell_Flag.HASTE.value) ? 5 : 0)]) / 10;

			/*
			 * Adjust threat for speed -- multipliers are more threatening.
			 */
			if(r_ptr.flags.has(Monster_Flag.MULTIPLY.value))
				r_ptr.highest_threat = (short)((r_ptr.highest_threat * Misc.extract_energy[r_ptr.speed + (r_ptr.spell_flags.has(Monster_Spell_Flag.HASTE.value) ? 5 : 0)]) / 5);

			/*
			 * Adjust threat for friends.
			 */
			if(r_ptr.flags.has(Monster_Flag.FRIENDS.value))
				r_ptr.highest_threat *= 2;
			else if(r_ptr.flags.has(Monster_Flag.FRIEND.value))
				r_ptr.highest_threat = (short)(r_ptr.highest_threat * 3 / 2);

			/*but deep in a minimum*/
			if(dam < 1)
				dam = 1;

			/* We're done */
			return (dam);
		}

		static long hp_adjust(Monster_Race r_ptr) {
			long hp;
			int resists = 1;
			int hide_bonus = 0;

			/* Get the monster base hitpoints */
			hp = r_ptr.avg_hp;

			/* Never moves with no ranged attacks - high hit points count for less */
			if (r_ptr.flags.has(Monster_Flag.NEVER_MOVE.value) && !(r_ptr.freq_innate != 0 || r_ptr.freq_spell != 0))
			{
			    hp /= 2;
			    if (hp < 1) hp = 1;
			}

			/* Just assume healers have more staying power */
			if (r_ptr.spell_flags.has(Monster_Spell_Flag.HEAL.value)) hp = (hp * 6) / 5;

			/* Miscellaneous improvements */
			if (r_ptr.flags.has(Monster_Flag.REGENERATE.value)) {hp *= 10; hp /= 9;}
			if (r_ptr.flags.has(Monster_Flag.PASS_WALL.value)) {hp *= 3; hp /= 2;}

			/* Calculate hide bonus */
			if (r_ptr.flags.has(Monster_Flag.EMPTY_MIND.value)) hide_bonus += 2;
			else
			{
			    if (r_ptr.flags.has(Monster_Flag.COLD_BLOOD.value)) hide_bonus += 1;
			    if (r_ptr.flags.has(Monster_Flag.WEIRD_MIND.value)) hide_bonus += 1;
			}

			/* Invisibility */
			if (r_ptr.flags.has(Monster_Flag.INVISIBLE.value))
			{
			    hp = (hp * (r_ptr.level + hide_bonus + 1)) / Math.Max(1, (int)r_ptr.level);
			}

			/* Monsters that can teleport are a hassle, and can easily run away */
			if (r_ptr.spell_flags.test(Monster_Spell_Flag.SIZE, Monster_Spell_Flag.TPORT.value, Monster_Spell_Flag.TELE_AWAY.value,
			    Monster_Spell_Flag.TELE_LEVEL.value))
			    hp = (hp * 6) / 5;

			/*
			 * Monsters that multiply are tougher to kill
			 */
			if (r_ptr.flags.has(Monster_Flag.MULTIPLY.value)) hp *= 2;

			/* Monsters with resistances are harder to kill.
			   Therefore effective slays / brands against them are worth more. */
			if (r_ptr.flags.has(Monster_Flag.IM_ACID.value)) resists += 2;
			if (r_ptr.flags.has(Monster_Flag.IM_FIRE.value)) resists += 2;
			if (r_ptr.flags.has(Monster_Flag.IM_COLD.value)) resists += 2;
			if (r_ptr.flags.has(Monster_Flag.IM_ELEC.value)) resists += 2;
			if (r_ptr.flags.has(Monster_Flag.IM_POIS.value)) resists += 2;

			/* Bonus for multiple basic resists and weapon resists */
			if (resists >= 12) resists *= 6;
			else if (resists >= 10) resists *= 4;
			else if (resists >= 8) resists *= 3;
			else if (resists >= 6) resists *= 2;

			/* If quite resistant, reduce resists by defense holes */
			if (resists >= 6)
			{
			    if (r_ptr.flags.has(Monster_Flag.HURT_ROCK.value)) resists -= 1;
			    if (r_ptr.flags.has(Monster_Flag.HURT_LIGHT.value)) resists -= 1;
			    if (!r_ptr.flags.has(Monster_Flag.NO_SLEEP.value)) resists -= 3;
			    if (!r_ptr.flags.has(Monster_Flag.NO_FEAR.value)) resists -= 2;
			    if (!r_ptr.flags.has(Monster_Flag.NO_CONF.value)) resists -= 2;
			    if (!r_ptr.flags.has(Monster_Flag.NO_STUN.value)) resists -= 1;

			    if (resists < 5) resists = 5;
			}

			/* If quite resistant, bonus for high resists */
			if (resists >= 3)
			{
			    if (r_ptr.flags.has(Monster_Flag.IM_WATER.value)) resists += 1;
			    if (r_ptr.flags.has(Monster_Flag.RES_NETH.value)) resists += 1;
			    if (r_ptr.flags.has(Monster_Flag.RES_NEXUS.value)) resists += 1;
			    if (r_ptr.flags.has(Monster_Flag.RES_DISE.value)) resists += 1;
			}

			/* Scale resists */
			resists = resists * 25;

			/* Monster resistances */
			if (resists < (r_ptr.ac + resists) / 3)
			{
			    hp += (hp * resists) / (150 + r_ptr.level); 	
			}
			else
			{
			    hp += (hp * (r_ptr.ac + resists) / 3) / (150 + r_ptr.level); 			
			}

			/*boundry control*/
			if (hp < 1) hp = 1;

			return (hp);
		}

		public static int r_power(Monster_Race[] races) {
			//God damn it C...
			/*
			ang_file *mon_fp;
			char buf[1024];*/
			bool dump = false;

			/* Allocate space for power */
			long[] power = new long[Misc.z_info.r_max];
			long[] tot_hp = new long[Misc.MAX_DEPTH];
			long[] tot_dam = new long[Misc.MAX_DEPTH];
			long[] mon_count = new long[Misc.MAX_DEPTH];

			for(int iteration = 0; iteration < 3; iteration++) {

				/* Reset the sum of all monster power values */
				tot_mon_power = 0;

				/* Make sure all arrays start at zero */
				for(int i = 0; i < Misc.MAX_DEPTH; i++) {
					tot_hp[i] = 0;
					tot_dam[i] = 0;
					mon_count[i] = 0;
				}

				/* Go through r_info and evaluate power ratings & flows. */
				for(int i = 0; i < Misc.z_info.r_max; i++) {

					/* Point at the "info" */
					Monster_Race r_ptr = races[i];
					if(r_ptr == null)
						continue;

					/* Set the current level */
					byte lvl = r_ptr.level;

					/* Maximum damage this monster can do in 10 game turns */
					long dam = max_dam(r_ptr);

					/* Adjust hit points based on resistances */
					long hp = hp_adjust(r_ptr);

					/* Hack -- set exp */
					if(lvl == 0)
						r_ptr.mexp = 0;
					else {
						/* Compute depths of non-unique monsters */
						if(r_ptr.flags.has(Monster_Flag.UNIQUE.value)) {
							long mexp = (hp * dam) / 25;
							long threat = r_ptr.highest_threat;

							/* Compute level algorithmically */
							int j;
							for(j = 1; (mexp > j + 4) || (threat > j + 5);
								mexp -= j * j, threat -= (j + 4), j++)
								;

							/* Set level */
							lvl = (byte)Math.Min((j > 250 ? 90 + (j - 250) / 20 : /* Level 90+ */
									(j > 130 ? 70 + (j - 130) / 6 :	/* Level 70+ */
									(j > 40 ? 40 + (j - 40) / 3 :	/* Level 40+ */
									j))), 99);

							/* Set level */
							if(Misc.arg_rebalance)
								r_ptr.level = lvl;
						}

						if(Misc.arg_rebalance) {
							/* Hack -- for Ungoliant */
							if(hp > 10000)
								r_ptr.mexp = (int)((hp / 25) * (dam / lvl));
							else
								r_ptr.mexp = (int)((hp * dam) / (lvl * 25));

							/* Round to 2 significant figures */
							if(r_ptr.mexp > 100) {
								if(r_ptr.mexp < 1000) {
									r_ptr.mexp = (r_ptr.mexp + 5) / 10;
									r_ptr.mexp *= 10;
								} else if(r_ptr.mexp < 10000) {
									r_ptr.mexp = (r_ptr.mexp + 50) / 100;
									r_ptr.mexp *= 100;
								} else if(r_ptr.mexp < 100000) {
									r_ptr.mexp = (r_ptr.mexp + 500) / 1000;
									r_ptr.mexp *= 1000;
								} else if(r_ptr.mexp < 1000000) {
									r_ptr.mexp = (r_ptr.mexp + 5000) / 10000;
									r_ptr.mexp *= 10000;
								} else if(r_ptr.mexp < 10000000) {
									r_ptr.mexp = (r_ptr.mexp + 50000) / 100000;
									r_ptr.mexp *= 100000;
								}
							}
						}
					}

					/* If we're rebalancing, this is a nop, if not, we restore the orig value */
					lvl = r_ptr.level;
					if((lvl != 0) && (r_ptr.mexp < 1L))
						r_ptr.mexp = 1;

					/*
					 * Hack - We have to use an adjustment factor to prevent overflow.
					 * Try to scale evenly across all levels instead of scaling by level.
					 */
					hp /= 2;
					if(hp < 1)
						hp = 1;
					r_ptr.hp = hp;		/*AMF:DEBUG*/

					/* Define the power rating */
					power[i] = hp * dam;

					/* Adjust for group monsters.  Average in-level group size is 5 */
					if(!r_ptr.flags.has(Monster_Flag.UNIQUE.value)) {
						if(r_ptr.flags.has(Monster_Flag.FRIEND.value))
							power[i] *= 2;
						else if(r_ptr.flags.has(Monster_Flag.FRIENDS.value))
							power[i] *= 5;
					}

					/* Adjust for escorts */
					if(r_ptr.flags.has(Monster_Flag.ESCORTS.value))
						power[i] *= 3;
					if(r_ptr.flags.has(Monster_Flag.ESCORT.value) && !r_ptr.flags.has(Monster_Flag.ESCORTS.value))
						power[i] *= 2;

					/* Adjust for multiplying monsters. This is modified by the speed,
					 * as fast multipliers are much worse than slow ones. We also adjust for
					 * ability to bypass walls or doors.
					 */
					if(r_ptr.flags.has(Monster_Flag.MULTIPLY.value)) {
						if(r_ptr.flags.test(Monster_Flag.SIZE, Monster_Flag.KILL_WALL.value, Monster_Flag.PASS_WALL.value))
							power[i] = Math.Max(power[i], power[i] * Misc.extract_energy[r_ptr.speed
										+ (r_ptr.spell_flags.has(Monster_Spell_Flag.HASTE.value) ? 5 : 0)]);
						else if(r_ptr.flags.test(Monster_Flag.SIZE, Monster_Flag.OPEN_DOOR.value, Monster_Flag.BASH_DOOR.value))
							power[i] = Math.Max(power[i], power[i] * Misc.extract_energy[r_ptr.speed
										+ (r_ptr.spell_flags.has(Monster_Spell_Flag.HASTE.value) ? 5 : 0)] * 3 / 2);
						else
							power[i] = Math.Max(power[i], power[i] * Misc.extract_energy[r_ptr.speed
										+ (r_ptr.spell_flags.has(Monster_Spell_Flag.HASTE.value) ? 5 : 0)] / 2);
					}

					/*
					 * Update the running totals - these will be used as divisors later
					 * Total HP / dam / count for everything up to the current level
					 */
					for(int j = lvl; j < (lvl == 0 ? lvl + 1 : Misc.MAX_DEPTH); j++) {
						int count = 10;

						/* Uniques don't count towards monster power on the level. */
						if(r_ptr.flags.has(Monster_Flag.UNIQUE.value))
							continue;

						/* Specifically placed monsters don't count towards monster power
						 * on the level. */
						if(!(r_ptr.rarity != 0))
							continue;

						/* Hack -- provide adjustment factor to prevent overflow */
						if((j == 90) && (r_ptr.level < 90)) {
							hp /= 10;
							dam /= 10;
						}

						if((j == 65) && (r_ptr.level < 65)) {
							hp /= 10;
							dam /= 10;
						}

						if((j == 40) && (r_ptr.level < 40)) {
							hp /= 10;
							dam /= 10;
						}

						/*
						 * Hack - if it's a group monster or multiplying monster, add several to the count
						 * so that the averages don't get thrown off
						 */

						if(r_ptr.flags.has(Monster_Flag.FRIEND.value))
							count = 20;
						else if(r_ptr.flags.has(Monster_Flag.FRIENDS.value))
							count = 50;

						if(r_ptr.flags.has(Monster_Flag.MULTIPLY.value)) {
							if(r_ptr.flags.test(Monster_Flag.SIZE, Monster_Flag.KILL_WALL.value, Monster_Flag.PASS_WALL.value))
								count = Math.Max(1, (int)Misc.extract_energy[r_ptr.speed
									+ (r_ptr.spell_flags.has(Monster_Spell_Flag.HASTE.value) ? 5 : 0)]) * count;
							else if(r_ptr.flags.test(Monster_Flag.SIZE, Monster_Flag.OPEN_DOOR.value, Monster_Flag.BASH_DOOR.value))
								count = Math.Max(1, Misc.extract_energy[r_ptr.speed
									+ (r_ptr.spell_flags.has(Monster_Spell_Flag.HASTE.value) ? 5 : 0)] * 3 / 2) * count;
							else
								count = Math.Max(1, Misc.extract_energy[r_ptr.speed
									+ (r_ptr.spell_flags.has(Monster_Spell_Flag.HASTE.value) ? 5 : 0)] / 2) * count;
						}

						/* Very rare monsters count less towards total monster power on the
						 * level. */
						if(r_ptr.rarity > count) {
							hp = hp * count / r_ptr.rarity;
							dam = dam * count / r_ptr.rarity;

							count = r_ptr.rarity;
						}

						tot_hp[j] += hp;
						tot_dam[j] += dam;

						mon_count[j] += count / r_ptr.rarity;
					}

				}

				/* Apply divisors now */
				for(int i = 0; i < Misc.z_info.r_max; i++) {
					int new_power;

					/* Point at the "info" */
					Monster_Race r_ptr = races[i];
					if(r_ptr == null)
						continue;

					/* Extract level */
					byte lvl = r_ptr.level;

					/* Paranoia */
					if(tot_hp[lvl] != 0 && tot_dam[lvl] != 0) {

						/* Divide by average HP and av damage for all in-level monsters */
						/* Note we have factored in the above 'adjustment factor' */
						long av_hp = tot_hp[lvl] * 10 / mon_count[lvl];
						long av_dam = tot_dam[lvl] * 10 / mon_count[lvl];

						/* Assign monster power */
						r_ptr.power = power[i];

						/* Justifiable paranoia - avoid divide by zero errors */
						if(av_hp > 0)
							power[i] = power[i] / av_hp;
						if(av_dam > 0)
							power[i] = power[i] / av_dam;

						/* Assign monster scaled power */
						r_ptr.scaled_power = power[i];

						/* Never less than 1 */
						if(r_ptr.power < 1)
							r_ptr.power = 1;

						/* Get power */
						new_power = (int)r_ptr.power;

						/* Compute rarity algorithmically */
						int j;
						for(j = 1; new_power > j; new_power -= j * j, j++)
							;

						/* Set rarity */
						if(Misc.arg_rebalance)
							r_ptr.rarity = (byte)j;
					}
				}

			}
			/* Determine total monster power */
			for(int i = 0; i < Misc.z_info.r_max; i++) {
				Monster_Race r = Misc.r_info[i];
				if(r == null)
					continue;

				tot_mon_power += (int)r.scaled_power;
			}

			/*	msg("Tot mon power is %d", tot_mon_power); */

			if(dump) {
				/* dump the power details */
				throw new NotImplementedException();
				//path_build(buf, sizeof(buf), ANGBAND_DIR_USER, "mon_power.txt");
				//mon_fp = file_open(buf, MODE_WRITE, FTYPE_TEXT);

				//file_putf(mon_fp, "ridx|level|rarity|d_char|name|pwr|scaled|melee|spell|hp\n");

				//for (i = 0; i < z_info.r_max; i++) {
				//    r_ptr = &r_info[i];	

				//    /* Don't print anything for nonexistent monsters */
				//    if (!r_ptr.name) continue;

				//    file_putf(mon_fp, "%d|%d|%d|%c|%s|%d|%d|%d|%d|%d\n", r_ptr.ridx,
				//        r_ptr.level, r_ptr.rarity, r_ptr.d_char, r_ptr.name,
				//        r_ptr.power, r_ptr.scaled_power, r_ptr.melee_dam,
				//        r_ptr.spell_dam, r_ptr.hp);
				//}

				//file_close(mon_fp);
			}

			/* Free power array */
			//FREE(power);

			/* Success */
			return 0;
		}
	}
}
