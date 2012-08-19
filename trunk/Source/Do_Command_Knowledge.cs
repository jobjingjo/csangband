using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	partial class Do_Command {
		/*
		 * Display known artifacts
		 */
		public static void knowledge_artifacts(string name, int row)
		{
			throw new NotImplementedException();
			///* HACK -- should be TV_MAX */
			//group_funcs obj_f = {TV_GOLD, false, kind_name, a_cmp_tval, art2gid, 0};
			//member_funcs art_f = {display_artifact, desc_art_fake, 0, 0, recall_prompt, 0, 0};

			//int *artifacts;
			//int a_count = 0;

			//artifacts = C_ZNEW(z_info.a_max, int);

			///* Collect valid artifacts */
			//a_count = collect_known_artifacts(artifacts, z_info.a_max);

			//display_knowledge("artifacts", artifacts, a_count, obj_f, art_f, null);
			//FREE(artifacts);
		}

		/*
		 * Display known ego_items
		 */
		public static void knowledge_ego_items(string name, int row)
		{
			throw new NotImplementedException();
			//group_funcs obj_f =
			//    {TV_GOLD, false, ego_grp_name, e_cmp_tval, default_group, 0};

			//member_funcs ego_f = {display_ego_item, desc_ego_fake, 0, 0, recall_prompt, 0, 0};

			//int *egoitems;
			//int e_count = 0;
			//int i, j;

			///* HACK: currently no more than 3 tvals for one ego type */
			//egoitems = C_ZNEW(z_info.e_max * EGO_TVALS_MAX, int);
			//default_join = C_ZNEW(z_info.e_max * EGO_TVALS_MAX, join_t);

			//for (i = 0; i < z_info.e_max; i++)
			//{
			//    if (e_info[i].everseen || OPT(cheat_xtra))
			//    {
			//        for (j = 0; j < EGO_TVALS_MAX && e_info[i].tval[j]; j++)
			//        {
			//            int gid = obj_group_order[e_info[i].tval[j]];

			//            /* Ignore duplicate gids */
			//            if (j > 0 && gid == default_join[e_count - 1].gid) continue;

			//            egoitems[e_count] = e_count;
			//            default_join[e_count].oid = i;
			//            default_join[e_count++].gid = gid;
			//        }
			//    }
			//}

			//display_knowledge("ego items", egoitems, e_count, obj_f, ego_f, null);

			//FREE(default_join);
			//FREE(egoitems);
		}

		/*
		 * Interact with feature visuals.
		 */
		public static void knowledge_features(string name, int row)
		{
			throw new NotImplementedException();
			//group_funcs fkind_f = {N_ELEMENTS(feature_group_text), false,
			//                        fkind_name, f_cmp_fkind, feat_order, 0};

			//member_funcs feat_f = {display_feature, feat_lore, f_xchar, f_xattr, feat_prompt, f_xtra_act, 0};

			//int *features;
			//int f_count = 0;
			//int i;

			//features = C_ZNEW(z_info.f_max, int);

			//for (i = 0; i < z_info.f_max; i++)
			//{
			//    /* Ignore non-features and mimics */
			//    if (f_info[i].name == 0 || f_info[i].mimic != i)
			//        continue;

			//    features[f_count++] = i; /* Currently no filter for features */
			//}

			//display_knowledge("features", features, f_count, fkind_f, feat_f,
			//    "                    Sym");
			//FREE(features);
		}

		public static void knowledge_store(string name, int row)
		{
			throw new NotImplementedException();
			//store_knowledge = row - 5;
			//do_cmd_store_knowledge();
			//store_knowledge = STORE_NONE;
		}

		public static void knowledge_scores(string name, int row)
		{
			throw new NotImplementedException();
			//show_scores();
		}

		public static void knowledge_history(string name, int row)
		{
			throw new NotImplementedException();
			//history_display();
		}

		/*
		 * Display known monsters.
		 */
		public static void knowledge_monsters(string name, int row)
		{
			throw new NotImplementedException();
			//group_funcs r_funcs = {N_ELEMENTS(monster_group), false, race_name,
			//                        m_cmp_race, default_group, mon_summary};

			//member_funcs m_funcs = {display_monster, mon_lore, m_xchar, m_xattr, recall_prompt, 0, 0};

			//int *monsters;
			//int m_count = 0;
			//int i;
			//size_t j;

			//for (i = 0; i < z_info.r_max; i++)
			//{
			//    monster_race *r_ptr = &r_info[i];
			//    if (!OPT(cheat_know) && !l_list[i].sights) continue;
			//    if (!r_ptr.name) continue;

			//    if (rf_has(r_ptr.flags, RF_UNIQUE)) m_count++;

			//    for (j = 1; j < N_ELEMENTS(monster_group) - 1; j++)
			//    {
			//        const char *pat = monster_group[j].chars;
			//        if (strchr(pat, r_ptr.d_char)) m_count++;
			//    }
			//}

			//default_join = C_ZNEW(m_count, join_t);
			//monsters = C_ZNEW(m_count, int);

			//m_count = 0;
			//for (i = 0; i < z_info.r_max; i++)
			//{
			//    monster_race *r_ptr = &r_info[i];
			//    if (!OPT(cheat_know) && !l_list[i].sights) continue;
			//    if (!r_ptr.name) continue;

			//    for (j = 0; j < N_ELEMENTS(monster_group)-1; j++)
			//    {
			//        const char *pat = monster_group[j].chars;
			//        if (j == 0 && !rf_has(r_ptr.flags, RF_UNIQUE))
			//            continue;
			//        else if (j > 0 && !strchr(pat, r_ptr.d_char))
			//            continue;

			//        monsters[m_count] = m_count;
			//        default_join[m_count].oid = i;
			//        default_join[m_count++].gid = j;
			//    }
			//}

			//display_knowledge("monsters", monsters, m_count, r_funcs, m_funcs,
			//        "                   Sym  Kills");
			//FREE(default_join);
			//FREE(monsters);
		}
	}
}
