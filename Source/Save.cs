using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Monster;
using CSAngband.Object;
using CSAngband.Player;

namespace CSAngband {
	class Save {
        private static void wr_string(string val)
        {
            foreach (char c in val.ToCharArray())
            {
                wr_byte((byte)c);
            }
        }

        private static void wr_u16b(UInt16 val)
        {
			Savefile.BufferAddBytes(BitConverter.GetBytes(val));
        }

        private static void wr_s16b(Int16 val)
        {
			Savefile.BufferAddBytes(BitConverter.GetBytes(val));
        }

        private static void wr_u32b(UInt32 val)
        {
			Savefile.BufferAddBytes(BitConverter.GetBytes(val));
        }

		private static void wr_s32b(Int32 val)
        {
			Savefile.BufferAddBytes(BitConverter.GetBytes(val));
        }

        public static void wr_byte(byte val)
        {
			Savefile.BufferAddBytes(new byte[] {val});
        }

		public static void wr_byte(bool val) {
			Savefile.BufferAddBytes(new byte[] { (byte)(val ? 0 : 1) });
		}

		/*
		 * Write an "item" record
		 */
		static void wr_item(Object.Object o_ptr)
		{
			int i, j;

			wr_u16b(0xffff);
			wr_byte(Savefile.ITEM_VERSION);

			wr_s16b(0);

			/* Location */
			wr_byte(o_ptr.iy);
			wr_byte(o_ptr.ix);

			wr_byte(o_ptr.tval);
			wr_byte(o_ptr.sval);

			for (i = 0; i < Misc.MAX_PVALS; i++) {
				wr_s16b(o_ptr.pval[i]);
			}
			wr_byte(o_ptr.num_pvals);

			wr_byte(0);

			wr_byte(o_ptr.number);
			wr_s16b(o_ptr.weight);

			if (o_ptr.artifact != null) wr_byte((byte)o_ptr.artifact.aidx);
			else wr_byte(0);

			if (o_ptr.ego != null) wr_byte((byte)o_ptr.ego.eidx);
			else wr_byte(0);

			wr_s16b(o_ptr.timeout);

			wr_s16b(o_ptr.to_h);
			wr_s16b(o_ptr.to_d);
			wr_s16b(o_ptr.to_a);
			wr_s16b(o_ptr.ac);
			wr_byte(o_ptr.dd);
			wr_byte(o_ptr.ds);

			wr_u16b((ushort)o_ptr.ident);

			wr_byte(o_ptr.marked);

			wr_byte((byte)o_ptr.origin);
			wr_byte(o_ptr.origin_depth);
			wr_u16b((ushort)o_ptr.origin_xtra);

			for (i = 0; i < Object.Object_Flag.BYTES && i < Object.Object_Flag.SIZE; i++)
				wr_byte(o_ptr.flags[i]);
			if (i < Object.Object_Flag.BYTES) Savefile.pad_bytes(Object.Object_Flag.BYTES - i);

			for (i = 0; i < Object.Object_Flag.BYTES && i < Object.Object_Flag.SIZE; i++)
				wr_byte(o_ptr.known_flags[i]);
			if (i < Object.Object_Flag.BYTES) Savefile.pad_bytes(Object.Object_Flag.BYTES - i);

			for (j = 0; j < Misc.MAX_PVALS; j++) {
				for (i = 0; i < Object.Object_Flag.BYTES && i < Object.Object_Flag.SIZE; i++)
					wr_byte(o_ptr.pval_flags[j][i]);
				if (i < Object.Object_Flag.BYTES) Savefile.pad_bytes(Object.Object_Flag.BYTES - i);
			}

			/* Held by monster index */
			wr_s16b(o_ptr.held_m_idx);

			wr_s16b(o_ptr.mimicking_m_idx);

			/* Save the inscription (if any) */
			if (o_ptr.note != null) {
				wr_string(o_ptr.note.ToString());
			} else {
				wr_string("");
			}
		}


		/*
		 * Write RNG state
		 *
		 * There were originally 64 bytes of randomizer saved. Now we only need
		 * 32 + 5 bytes saved, so we'll write an extra 27 bytes at the end which won't
		 * be used.
		 */
		public static void wr_randomizer()
		{
			int i;

			/* current value for the simple RNG */
			wr_u32b(Random.Rand_value);

			/* state index */
			wr_u32b(Random.state_i);

            /* RNG variables */
            wr_u32b(Random.z0);
            wr_u32b(Random.z1);
            wr_u32b(Random.z2);

            /* RNG state */
            for (i = 0; i < Random.RAND_DEG; i++)
                wr_u32b(Random.STATE[i]);

            /* null padding */
            for (i = 0; i < 59 - Random.RAND_DEG; i++)
                wr_u32b(0);
		}


		/*
		 * Write the "options"
		 */
		public static void wr_options()
		{
		    int i, k;

		    uint[] window_flag = new uint[Misc.ANGBAND_TERM_MAX];
            uint[] window_mask = new uint[Misc.ANGBAND_TERM_MAX];


		    /* Special Options */
            wr_byte(Player.Player_Other.instance.delay_factor);
		    wr_byte(Player.Player_Other.instance.hitpoint_warn);
		    wr_u16b(Misc.lazymove_delay);

		    /* Normal options */
            for (i = 0; i < Option.MAX; i++)
            {
                if (Option.options[i] == null) continue;

                string name = Option.options[i].name;
		        
                if (name == null) continue;

		        wr_string(name);
		        wr_byte((byte)(Option.options[i].value?1:0));
		   }

		    /* Sentinel */
		    wr_byte(0);

		    /*** Window options ***/

		    /* Reset */
		    for (i = 0; i < Misc.ANGBAND_TERM_MAX; i++)
		    {
		        /* Flags */
                window_flag[i] = Player.Player_Other.instance.window_flag[i];

		        /* Mask */
		        window_mask[i] = 0;

		        /* Build the mask */
		        for (k = 0; k < 32; k++)
		        {
		            /* Set mask */
		            if (Table.window_flag_desc[k] != null)
		            {
		                window_mask[i] |= (uint)(1 << k);
		            }
		        }
		    }

		    /* Dump the flags */
		    for (i = 0; i < Misc.ANGBAND_TERM_MAX; i++) wr_u32b(window_flag[i]);

		    /* Dump the masks */
		    for (i = 0; i < Misc.ANGBAND_TERM_MAX; i++) wr_u32b(window_mask[i]);
		}


		public static void wr_messages()
		{
			UInt16 num = Message.messages_num();
			if (num > 80) num = 80;
			wr_u16b(num);

			/* Dump the messages (oldest first!) */
			for (int i = (num - 1); i >= 0; i--) {
				wr_string(Message.str((ushort)i));
				wr_u16b((ushort)Message.type((ushort)i));
			}
		}


		public static void wr_monster_memory()
		{
			int i;
			int r_idx;

			wr_u16b(Misc.z_info.r_max);
			for (r_idx = 0; r_idx < Misc.z_info.r_max; r_idx++) {
				Monster_Race r_ptr = Misc.r_info[r_idx];
				Monster_Lore l_ptr = Misc.l_list[r_idx];

				if (r_ptr == null) {
					r_ptr = new Monster_Race();
				}

				if (l_ptr == null) {
					l_ptr = new Monster_Lore();
				}

				/* Count sights/deaths/kills */
				wr_s16b(l_ptr.sights);
				wr_s16b(l_ptr.deaths);
				wr_s16b(l_ptr.pkills);
				wr_s16b(l_ptr.tkills);

				/* Count wakes and ignores */
				wr_byte(l_ptr.wake);
				wr_byte(l_ptr.ignore);

				/* Count drops */
				wr_byte(l_ptr.drop_gold);
				wr_byte(l_ptr.drop_item);

				/* Count spells */
				wr_byte(l_ptr.cast_innate);
				wr_byte(l_ptr.cast_spell);

				/* Count blows of each type */
				for (i = 0; i < Monster_Blow.MONSTER_BLOW_MAX; i++)
					wr_byte(l_ptr.blows[i]);

				/* Memorize flags */
				for (i = 0; i < Monster_Flag.BYTES && i < Monster_Flag.SIZE; i++)
					wr_byte(l_ptr.flags.data[i]);
				if (i < Monster_Flag.BYTES) Savefile.pad_bytes(Monster_Flag.BYTES - i);

				for (i = 0; i < Monster_Flag.BYTES && i < Monster_Spell_Flag.SIZE; i++)
					wr_byte(l_ptr.spell_flags[i]);
				if (i < Monster_Flag.BYTES) Savefile.pad_bytes(Monster_Flag.BYTES - i);

				/* Monster limit per level */
				wr_byte(r_ptr.max_num);

				/* XXX */
				wr_byte(0);
				wr_byte(0);
				wr_byte(0);
			}
		}


		public static void wr_object_memory()
		{
			wr_u16b(Misc.z_info.k_max);
			for (int k_idx = 0; k_idx < Misc.z_info.k_max; k_idx++) {
				byte tmp8u = 0;
				Object_Kind k_ptr = Misc.k_info[k_idx];
				if (k_ptr == null) {
					k_ptr = new Object_Kind();
				}

				if (k_ptr.aware) tmp8u |= 0x01;
				if (k_ptr.tried) tmp8u |= 0x02;
				if (Squelch.kind_is_squelched_aware(k_ptr)) tmp8u |= 0x04;
				if (k_ptr.everseen) tmp8u |= 0x08;
				if (Squelch.kind_is_squelched_unaware(k_ptr)) tmp8u |= 0x10;

				wr_byte(tmp8u);
			}
		}


		public static void wr_quests()
		{
			int i;
			ushort tmp16u;

			/* Hack -- Dump the quests */
			tmp16u = Misc.MAX_Q_IDX;
			wr_u16b(tmp16u);
			for (i = 0; i < tmp16u; i++) {
				Quest q = Misc.q_list[i];
				if (q == null) {
					q = new Quest();
				}
				wr_byte(q.level);
				wr_byte(0);
				wr_byte(0);
				wr_byte(0);
			}
		}


		public static void wr_artifacts()
		{
			int i;
			ushort tmp16u;

			/* Hack -- Dump the artifacts */
			tmp16u = Misc.z_info.a_max;
			wr_u16b(tmp16u);
			for (i = 0; i < tmp16u; i++) {
				Artifact a_ptr = Misc.a_info[i];
				if (a_ptr == null) {
					a_ptr = new Artifact();
				}
				wr_byte(a_ptr.created);
				wr_byte(a_ptr.seen);
				wr_byte(a_ptr.everseen);
				wr_byte(0);
			}
		}


		public static void wr_player()
		{
			int i;

			wr_string(Player_Other.instance.full_name);

			wr_string(Misc.p_ptr.died_from);

			wr_string(Misc.p_ptr.history);

			/* Race/Class/Gender/Spells */
			wr_byte((byte)Misc.p_ptr.Race.ridx);
			wr_byte((byte)Misc.p_ptr.Class.cidx);
			wr_byte(Misc.p_ptr.psex);
			wr_byte(Player_Other.instance.name_suffix);

			wr_byte(Misc.p_ptr.hitdie);
			wr_byte(Misc.p_ptr.expfact);

			wr_s16b(Misc.p_ptr.age);
			wr_s16b(Misc.p_ptr.ht);
			wr_s16b(Misc.p_ptr.wt);

			/* Dump the stats (maximum and current and birth) */
			for (i = 0; i < (int)Stat.Max; ++i) wr_s16b(Misc.p_ptr.stat_max[i]);
			for (i = 0; i < (int)Stat.Max; ++i) wr_s16b(Misc.p_ptr.stat_cur[i]);
			for (i = 0; i < (int)Stat.Max; ++i) wr_s16b(Misc.p_ptr.stat_birth[i]);

			wr_s16b(Misc.p_ptr.ht_birth);
			wr_s16b(Misc.p_ptr.wt_birth);
			wr_s16b(Misc.p_ptr.sc_birth);
			wr_u32b((uint)Misc.p_ptr.au_birth);

			/* Padding */
			wr_u32b(0);

			wr_u32b((uint)Misc.p_ptr.au);


			wr_u32b((uint)Misc.p_ptr.max_exp);
			wr_u32b((uint)Misc.p_ptr.exp);
			wr_u16b(Misc.p_ptr.exp_frac);
			wr_s16b(Misc.p_ptr.lev);

			wr_s16b(Misc.p_ptr.mhp);
			wr_s16b(Misc.p_ptr.chp);
			wr_u16b(Misc.p_ptr.chp_frac);

			wr_s16b(Misc.p_ptr.msp);
			wr_s16b(Misc.p_ptr.csp);
			wr_u16b(Misc.p_ptr.csp_frac);

			/* Max Player and Dungeon Levels */
			wr_s16b(Misc.p_ptr.max_lev);
			wr_s16b(Misc.p_ptr.max_depth);

			/* More info */
			wr_s16b(0);	/* oops */
			wr_s16b(0);	/* oops */
			wr_s16b(0);	/* oops */
			wr_s16b(0);	/* oops */
			wr_s16b(Misc.p_ptr.sc);
			wr_s16b(0);	/* oops */

			wr_s16b(Misc.p_ptr.food);
			wr_s16b(Misc.p_ptr.energy);
			wr_s16b(Misc.p_ptr.word_recall);
			wr_s16b(Misc.p_ptr.state.see_infra);
			wr_byte(Misc.p_ptr.confusing);
			wr_byte(Misc.p_ptr.searching);

			/* Find the number of timed effects */
			wr_byte((byte)Timed_Effect.MAX);

			/* Read all the effects, in a loop */
			for (i = 0; i < (int)Timed_Effect.MAX; i++)
			    wr_s16b(Misc.p_ptr.timed[i]);

			/* Total energy used so far */
			wr_u32b(Misc.p_ptr.total_energy);
			/* # of turns spent resting */
			wr_u32b(Misc.p_ptr.resting_turn);

			/* Future use */
			for (i = 0; i < 8; i++) wr_u32b(0);
		}


		public static void wr_squelch()
		{
			int i, n;

			/* Write number of squelch bytes */
			wr_byte((byte)Squelch.squelch_size);
			for (i = 0; i < Squelch.squelch_size; i++)
				wr_byte(Squelch.squelch_level[i]);

			/* Write ego-item squelch bits */
			wr_u16b(Misc.z_info.e_max);
			for (i = 0; i < Misc.z_info.e_max; i++) {
				byte flags = 0;

				/* Figure out and write the everseen flag */
				if (Misc.e_info[i] != null && Misc.e_info[i].everseen) flags |= 0x02;
				wr_byte(flags);
			}

			n = 0;
			for (i = 0; i < Misc.z_info.k_max; i++)
				if (Misc.k_info[i] != null && Misc.k_info[i].note != null)
					n++;

			/* Write the current number of auto-inscriptions */
			wr_u16b((ushort)n);

			/* Write the autoinscriptions array */
			for (i = 0; i < Misc.z_info.k_max; i++) {
				if (Misc.k_info[i] == null || Misc.k_info[i].note == null)
					continue;
				wr_s16b((short)i);
				wr_string(Misc.k_info[i].note.ToString());
			}

			return;
		}


		public static void wr_misc() {
			/* XXX Old random artifact version, remove after 3.3 */
			wr_u32b(63);

			/* Random artifact seed */
			wr_u32b((uint)Misc.seed_randart);


			/* XXX Ignore some flags */
			wr_u32b(0);
			wr_u32b(0);
			wr_u32b(0);


			/* Write the "object seeds" */
			wr_u32b((uint)Misc.seed_flavor);
			wr_u32b((uint)Misc.seed_town);


			/* Special stuff */
			wr_u16b(Misc.p_ptr.panic_save);
			wr_u16b(Misc.p_ptr.total_winner);
			wr_u16b(Misc.p_ptr.noscore);


			/* Write death */
			wr_byte(Misc.p_ptr.is_dead);

			/* Write feeling */
			wr_byte(Cave.cave.feeling);
			wr_u16b(Cave.cave.feeling_squares);
			wr_s32b(Cave.cave.created_at);

			/* Current turn */
			wr_s32b(Misc.turn);
		}


		public static void wr_player_hp()
		{
			int i;

			wr_u16b(Misc.PY_MAX_LEVEL);
			for (i = 0; i < Misc.PY_MAX_LEVEL; i++)
				wr_s16b(Misc.p_ptr.player_hp[i]);
		}


		public static void wr_player_spells() {
			int i;

			wr_u16b(Misc.PY_MAX_SPELLS);

			for (i = 0; i < Misc.PY_MAX_SPELLS; i++)
				wr_byte((byte)Misc.p_ptr.spell_flags[i]);

			for (i = 0; i < Misc.PY_MAX_SPELLS; i++)
				wr_byte(Misc.p_ptr.spell_order[i]);
		}


		/*
		 * Dump the random artifacts
		 */
		public static void wr_randarts()
		{
			int i, j, k;

			if (!Option.birth_randarts.value)
			    return;

			wr_u16b(Misc.z_info.a_max);

			for (i = 0; i < Misc.z_info.a_max; i++)
			{
			    Artifact a_ptr = Misc.a_info[i];

			    wr_byte(a_ptr.tval);
			    wr_byte(a_ptr.sval);
			    for (j = 0; j < Misc.MAX_PVALS; j++)
			        wr_s16b(a_ptr.pval[j]);
			    wr_byte(a_ptr.num_pvals);

			    wr_s16b(a_ptr.to_h);
			    wr_s16b(a_ptr.to_d);
			    wr_s16b(a_ptr.to_a);
			    wr_s16b(a_ptr.ac);

			    wr_byte(a_ptr.dd);
			    wr_byte(a_ptr.ds);

			    wr_s16b(a_ptr.weight);

			    wr_s32b(a_ptr.cost);

			    for (j = 0; j < Object_Flag.BYTES && j < Object_Flag.SIZE; j++)
			        wr_byte(a_ptr.flags[j]);
			    if (j < Object_Flag.BYTES) Savefile.pad_bytes(Object_Flag.BYTES - j);

			    for (k = 0; k < Misc.MAX_PVALS; k++) {
			        for (j = 0; j < Object_Flag.BYTES && j < Object_Flag.SIZE; j++)
			            wr_byte(a_ptr.pval_flags[k][j]);
			        if (j < Object_Flag.BYTES) Savefile.pad_bytes(Object_Flag.BYTES - j);
			    }

			    wr_byte(a_ptr.level);
			    wr_byte(a_ptr.rarity);
			    wr_byte(a_ptr.alloc_prob);
			    wr_byte(a_ptr.alloc_min);
			    wr_byte(a_ptr.alloc_max);

			    wr_u16b((ushort)a_ptr.effect.value);
			    wr_u16b((ushort)a_ptr.time.Base);
			    wr_u16b((ushort)a_ptr.time.dice);
			    wr_u16b((ushort)a_ptr.time.sides);
			}
		}


		public static void wr_inventory()
		{
			int i;

			/* Write the inventory */
			for (i = 0; i < Misc.ALL_INVEN_TOTAL; i++) {
				Object.Object o_ptr = Misc.p_ptr.inventory[i];

				/* Skip non-objects */
				if (o_ptr.kind == null) continue;

				/* Dump index */
				wr_u16b((ushort)i);

				/* Dump object */
				wr_item(o_ptr);
			}

			/* Add a sentinel */
			wr_u16b(0xFFFF);
		}


		public static void wr_stores()
		{
			int i;

			wr_u16b((ushort)STORE.MAX_STORES);
			for (i = 0; i < (ushort)STORE.MAX_STORES; i++)
			{
			    Store st_ptr = Misc.stores[i];
			    int j;

			    /* XXX Old values */
			    wr_u32b(0);
			    wr_s16b(0);

			    /* Save the current owner */
			    wr_byte((byte)st_ptr.owner.oidx);

			    /* Save the stock size */
			    wr_byte(st_ptr.stock_num);

			    /* XXX Old values */
			    wr_s16b(0);
			    wr_s16b(0);

			    /* Save the stock */
			    for (j = 0; j < st_ptr.stock_num; j++)
			        wr_item(st_ptr.stock[j]);
			}
		}



		/*
		 * The cave grid flags that get saved in the savefile
		 */
		const int IMPORTANT_FLAGS = (Cave.CAVE_MARK | Cave.CAVE_GLOW | Cave.CAVE_ICKY | Cave.CAVE_ROOM);


		/*
		 * Write the current dungeon
		 */
		public static void wr_dungeon()
		{
			int y, x;

			byte tmp8u;

			byte count;
			byte prev_char;


			if (Misc.p_ptr.is_dead)
			    return;

			/*** Basic info ***/

			/* Dungeon specific info follows */
			wr_u16b((ushort)Misc.p_ptr.depth);
			wr_u16b(Misc.daycount);
			wr_u16b((ushort)Misc.p_ptr.py);
			wr_u16b((ushort)Misc.p_ptr.px);
			wr_u16b((ushort)Cave.cave.height);
			wr_u16b((ushort)Cave.cave.width);
			wr_u16b(0);
			wr_u16b(0);


			/*** Simple "Run-Length-Encoding" of cave ***/

			/* Note that this will induce two wasted bytes */
			count = 0;
			prev_char = 0;

			/* Dump the cave */
			for (y = 0; y < Cave.DUNGEON_HGT; y++)
			{
				for (x = 0; x < Cave.DUNGEON_WID; x++)
			    {
			        /* Extract the important cave.info flags */
					tmp8u = (byte)(Cave.cave.info[y][x] & (IMPORTANT_FLAGS));

			        /* If the run is broken, or too full, flush it */
			        if ((tmp8u != prev_char) || (count == byte.MaxValue))
			        {
			            wr_byte((byte)count);
			            wr_byte((byte)prev_char);
			            prev_char = tmp8u;
			            count = 1;
			        }

			        /* Continue the run */
			        else
			        {
			            count++;
			        }
			    }
			}

			/* Flush the data (if any) */
			if (count != 0)
			{
			    wr_byte((byte)count);
			    wr_byte((byte)prev_char);
			}

			/** Now dump the cave.info2[][] stuff **/

			/* Note that this will induce two wasted bytes */
			count = 0;
			prev_char = 0;

			/* Dump the cave */
			for (y = 0; y < Cave.DUNGEON_HGT; y++)
			{
				for (x = 0; x < Cave.DUNGEON_WID; x++)
			    {
			        /* Keep all the information from info2 */
					tmp8u = (byte)Cave.cave.info2[y][x];

			        /* If the run is broken, or too full, flush it */
			        if ((tmp8u != prev_char) || (count == byte.MaxValue))
			        {
			            wr_byte((byte)count);
			            wr_byte((byte)prev_char);
			            prev_char = tmp8u;
			            count = 1;
			        }

			        /* Continue the run */
			        else
			        {
			            count++;
			        }
			    }
			}

			/* Flush the data (if any) */
			if (count != 0)
			{
			    wr_byte((byte)count);
			    wr_byte((byte)prev_char);
			}


			/*** Simple "Run-Length-Encoding" of cave ***/

			/* Note that this will induce two wasted bytes */
			count = 0;
			prev_char = 0;

			/* Dump the cave */
			for (y = 0; y < Cave.DUNGEON_HGT; y++)
			{
				for (x = 0; x < Cave.DUNGEON_WID; x++)
			    {
			        /* Extract a byte */
					tmp8u = Cave.cave.feat[y][x];

			        /* If the run is broken, or too full, flush it */
			        if ((tmp8u != prev_char) || (count == byte.MaxValue))
			        {
			            wr_byte((byte)count);
			            wr_byte((byte)prev_char);
			            prev_char = tmp8u;
			            count = 1;
			        }

			        /* Continue the run */
			        else
			        {
			            count++;
			        }
			    }
			}

			/* Flush the data (if any) */
			if (count != 0)
			{
			    wr_byte((byte)count);
			    wr_byte((byte)prev_char);
			}


			/*** Compact ***/

			/* Compact the objects */
			Object.Object.compact_objects(0);

			/* Compact the monsters */
			Monster_Make.compact_monsters(0);
		}


		public static void wr_objects()
		{
			int i;

			if (Misc.p_ptr.is_dead)
			    return;
	
			/* Total objects */
			wr_u16b((ushort)Misc.o_max);

			/* Dump the objects */
			for (i = 1; i < Misc.o_max; i++)
			{
			    Object.Object o_ptr = Object.Object.byid((short)i);

			    /* Dump it */
			    wr_item(o_ptr);
			}
		}


		public static void wr_monsters()
		{
			int i;
			int j;

			if (Misc.p_ptr.is_dead)
			    return;

			/* Total monsters */
			wr_u16b((ushort)Cave.cave_monster_max(Cave.cave));

			/* Dump the monsters */
			for (i = 1; i < Cave.cave_monster_max(Cave.cave); i++) {
			    byte unaware = 0;
	
				Monster.Monster m_ptr = Cave.cave_monster(Cave.cave, i);

			    wr_s16b(m_ptr.r_idx);
			    wr_byte(m_ptr.fy);
			    wr_byte(m_ptr.fx);
			    wr_s16b(m_ptr.hp);
			    wr_s16b(m_ptr.maxhp);
			    wr_byte(m_ptr.mspeed);
			    wr_byte(m_ptr.energy);
			    wr_byte((byte)Misc.MON_TMD.MAX);

			    for (j = 0; j < (byte)Misc.MON_TMD.MAX; j++)
			        wr_s16b(m_ptr.m_timed[j]);

			    if (m_ptr.unaware) unaware |= 0x01;
			    wr_byte(unaware);

			    for (j = 0; j < Object.Object_Flag.BYTES && j < Object.Object_Flag.SIZE; j++)
			        wr_byte(m_ptr.known_pflags[j]);
				if (j < Object.Object_Flag.BYTES) Savefile.pad_bytes(Object.Object_Flag.BYTES - j);
		
			    wr_byte(0);
			}
		}


		public static void wr_ghost()
		{
			int i;

			if (Misc.p_ptr.is_dead)
			    return;

			/* XXX */
	
			/* Name */
			wr_string("Broken Ghost");
	
			/* Hack -- stupid data */
			for (i = 0; i < 60; i++) wr_byte(0);
		}


		public static void wr_history()
		{
			int i;
			uint tmp32u = (uint)History.history_get_num();

			wr_u32b(tmp32u);
			for (i = 0; i < tmp32u; i++)
			{
			    wr_u16b(History.history_list[i].type);
			    wr_s32b(History.history_list[i].turn);
			    wr_s16b(History.history_list[i].dlev);
			    wr_s16b(History.history_list[i].clev);
			    wr_byte(History.history_list[i].a_idx);
			    wr_string(History.history_list[i].evt);
			}
		}
	}
}
