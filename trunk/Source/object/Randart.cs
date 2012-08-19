using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Object {
	class Randart {
		/*
		 * Randomize the artifacts
		 *
		 * The full flag toggles between just randomizing the names and
		 * complete randomization of the artifacts.
		 */
		public static int do_randart(int randart_seed, bool full)
		{
			throw new NotImplementedException();
			//errr err;

			///* Prepare to use the Angband "simple" RNG. */
			//Rand_value = randart_seed;
			//Rand_quick = true;

			///* Only do all the following if full randomization requested */
			//if (full)
			//{
			//    /* Allocate the various "original powers" arrays */
			//    base_power = C_ZNEW(z_info.a_max, s32b);
			//    base_item_level = C_ZNEW(z_info.a_max, byte);
			//    base_item_prob = C_ZNEW(z_info.a_max, byte);
			//    base_art_alloc = C_ZNEW(z_info.a_max, byte);
			//    baseprobs = C_ZNEW(z_info.k_max, s16b);
			//    base_freq = C_ZNEW(z_info.k_max, s16b);

			//    /* Open the log file for writing */
			//    if (verbose)
			//    {
			//        char buf[1024];
			//        path_build(buf, sizeof(buf), ANGBAND_DIR_USER,
			//            "randart.log");
			//        log_file = file_open(buf, MODE_WRITE, FTYPE_TEXT);
			//        if (!log_file)
			//        {
			//            msg("Error - can't open randart.log for writing.");
			//            exit(1);
			//        }
			//    }

			//    /* Store the original power ratings */
			//    store_base_power();

			//    /* Determine the generation probabilities */
			//    parse_frequencies();
			//}

			///* Generate the random artifact (names) */
			//err = do_randart_aux(full);

			///* Only do all the following if full randomization requested */
			//if (full)
			//{
			//    /* Just for fun, look at the frequencies on the finished items */
			//    /* Remove this prior to release */
			//    store_base_power();
			//    parse_frequencies();

			//    /* Close the log file */
			//    if (verbose)
			//    {
			//        if (!file_close(log_file))
			//        {
			//            msg("Error - can't close randart.log file.");
			//            exit(1);
			//        }
			//    }

			//    /* Free the "original powers" arrays */
			//    FREE(base_power);
			//    FREE(base_item_level);
			//    FREE(base_item_prob);
			//    FREE(base_art_alloc);
			//    FREE(baseprobs);
			//    FREE(base_freq);
			//}

			///* When done, resume use of the Angband "complex" RNG. */
			//Rand_quick = false;

			//return (err);
		}
	}
}
