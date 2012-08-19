using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * This file provides a pseudo-random number generator.
 *
 * This code provides both a "quick" random number generator (4 bytes of
 * state), and a "complex" random number generator (128 + 4 bytes of state).
 *
 * The complex RNG (used for most game entropy) is provided by the WELL102a
 * algorithm, used with permission. See below for copyright information
 * about the WELL implementation.
 *
 * To use of the "simple" RNG, activate it via "Rand_quick = true" and
 * "Rand_value = seed". After that it will be automatically used instead of
 * the "complex" RNG. When you are done, you can de-activate it via
 * "Rand_quick = false". You can also choose a new seed.
 */

/* begin WELL RNG
 * *************************************************************************
 * Copyright:  Francois Panneton and Pierre L'Ecuyer, University of Montreal
 *             Makoto Matsumoto, Hiroshima University                       
 * *************************************************************************
 * Code was modified slightly by Erik Osheim to work on unsigned integers.  
 */


namespace CSAngband {
	/**
	 * A struct representing a strategy for making a dice roll.
	 *
	 * The result will be base + XdY + BONUS, where m_bonus is used in a
	 * tricky way to determine BONUS.
	 */
	class random_value {
		public random_value() {
			Base = 0;
			dice = 0;
			sides = 0;
			m_bonus = 0;
		}
		public random_value(int Base, int dice, int sides, int m_bonus) {
			this.Base = Base;
			this.dice = dice;
			this.sides = sides;
			this.m_bonus = m_bonus;
		}
		public int Base;
		public int dice;
		public int sides;
		public int m_bonus;
	};
	/* Random aspects used by damcalc, m_bonus_calc, and ranvals */
	public enum aspect{
		MINIMISE,
		AVERAGE,
		MAXIMISE,
		EXTREMIFY,
		RANDOMISE
	};

	class Random {
		private static System.Random random = new System.Random();

		private static bool isfixed = false;
		private static int fixedval = 0;

		/**
		 * The number of 32-bit integers worth of seed state.
		 */
		public const int RAND_DEG = 32;

		/**
		 * Generates a random signed long integer X where "0 <= X < M" holds.
		 *
		 * The integer X falls along a uniform distribution.
		 */
		public static int randint0(int M){
			return Rand_div(M);
		}

		/**
		 * Generates a random signed long integer X where "1 <= X <= M" holds.
		 *
		 * The integer X falls along a uniform distribution.
		 */
		public static int randint1(int M) {
			return Rand_div(M) + 1;
		}

		/**
		 * Generate a random signed long integer X where "A - D <= X <= A + D" holds.
		 * Note that "rand_spread(A, D)" == "rand_range(A - D, A + D)"
		 *
		 * The integer X falls along a uniform distribution.
		 */
		public static int rand_spread(int A, int D) {
			return (int)((A) + (randint0(1 + (D) + (D))) - (D));
		}

		/**
		 * Return true one time in `x`.
		 */
		public static bool one_in_(int x) {
			return randint0(x) == 0;
		}

		/**
		 * Whether we are currently using the "quick" method or not.
		 */
		public static bool Rand_quick;

		/**
		 * The state used by the "quick" RNG. AKA Seed
		 */
		public static UInt32 Rand_value;

		/**
		 * The state used by the "complex" RNG.
		 */
		public static UInt32 state_i;
		public static UInt32[] STATE = new UInt32[RAND_DEG];
		public static UInt32 z0;
		public static UInt32 z1;
		public static UInt32 z2;


		/**
		 * Initialise the RNG state with the given seed.
		 */
		public static void Rand_state_init(int seed) {
			random = new System.Random(seed);
		}

		/**
		 * Generates a random unsigned long integer X where "0 <= X < M" holds.
		 *
		 * The integer X falls along a uniform distribution.
		 */
		public static int Rand_div(int m) {
			//TODO Make sure this works like it does in the original.
			if(isfixed) {
				return (fixedval * 1000 * (m - 1)) / (100 * 1000);
			}
			return random.Next(m);
		}

		/**
		 * Generate a signed random integer within `stand` standard deviations of
		 * `mean`, following a normal distribution.
		 */
		public static Int16 Rand_normal(int mean, int stand) {
			//throw new NotImplementedException();
			//What we are actually going to do is return a number equal to mean +- stand
			return (short)(mean + random.Next(stand * 2) - stand);
		}

		/**
		 * Generate a semi-random number from 0 to m-1, in a way that doesn't affect
		 * gameplay.  This is intended for use by external program parts like the
		 * main-*.c files.
		 */
		public static UInt32 Rand_simple(UInt32 m) {
			throw new NotImplementedException();
			//return 0;
		}

		/**
		 * Emulate a number `num` of dice rolls of dice with `sides` sides.
		 */
		public static int damroll(int num, int sides) {
			int i;
			int sum = 0;

			if (sides <= 0) return 0;

			for (i = 0; i < num; i++)
				sum += randint1(sides);
			return sum;
		}

		/**
		 * Calculation helper function for damroll
		 */
		public static int damcalc(int num, int sides, aspect dam_aspect) {
			switch (dam_aspect) {
				case aspect.MAXIMISE:
				case aspect.EXTREMIFY: return num * sides;

				case aspect.RANDOMISE: return damroll(num, sides);

				case aspect.MINIMISE: return num;

				case aspect.AVERAGE: return num * (sides + 1) / 2;
			}

			return 0;
		}

		/**
		 * Generates a random signed long integer X where "A <= X <= B"
		 * Note that "rand_range(0, N-1)" == "randint0(N)".
		 *
		 * The integer X falls along a uniform distribution.
		 */
		public static int rand_range(int A, int B) {
			return random.Next(B - A) + A;
		}

		/**
		 * Help determine an "enchantment bonus" for an object.
		 *
		 * To avoid floating point but still provide a smooth distribution of bonuses,
		 * we simply round the results of division in such a way as to "average" the
		 * correct floating point value.
		 *
		 * This function has been changed.  It uses "Rand_normal()" to choose values
		 * from a normal distribution, whose mean moves from zero towards the max as
		 * the level increases, and whose standard deviation is equal to 1/4 of the
		 * max, and whose values are forced to lie between zero and the max, inclusive.
		 *
		 * Since the "level" rarely passes 100 before Morgoth is dead, it is very
		 * rare to get the "full" enchantment on an object, even a deep levels.
		 *
		 * It is always possible (albeit unlikely) to get the "full" enchantment.
		 *
		 * A sample distribution of values from "m_bonus(10, N)" is shown below:
		 *
		 *   N       0     1     2     3     4     5     6     7     8     9    10
		 * ---    ----  ----  ----  ----  ----  ----  ----  ----  ----  ----  ----
		 *   0   66.37 13.01  9.73  5.47  2.89  1.31  0.72  0.26  0.12  0.09  0.03
		 *   8   46.85 24.66 12.13  8.13  4.20  2.30  1.05  0.36  0.19  0.08  0.05
		 *  16   30.12 27.62 18.52 10.52  6.34  3.52  1.95  0.90  0.31  0.15  0.05
		 *  24   22.44 15.62 30.14 12.92  8.55  5.30  2.39  1.63  0.62  0.28  0.11
		 *  32   16.23 11.43 23.01 22.31 11.19  7.18  4.46  2.13  1.20  0.45  0.41
		 *  40   10.76  8.91 12.80 29.51 16.00  9.69  5.90  3.43  1.47  0.88  0.65
		 *  48    7.28  6.81 10.51 18.27 27.57 11.76  7.85  4.99  2.80  1.22  0.94
		 *  56    4.41  4.73  8.52 11.96 24.94 19.78 11.06  7.18  3.68  1.96  1.78
		 *  64    2.81  3.07  5.65  9.17 13.01 31.57 13.70  9.30  6.04  3.04  2.64
		 *  72    1.87  1.99  3.68  7.15 10.56 20.24 25.78 12.17  7.52  4.42  4.62
		 *  80    1.02  1.23  2.78  4.75  8.37 12.04 27.61 18.07 10.28  6.52  7.33
		 *  88    0.70  0.57  1.56  3.12  6.34 10.06 15.76 30.46 12.58  8.47 10.38
		 *  96    0.27  0.60  1.25  2.28  4.30  7.60 10.77 22.52 22.51 11.37 16.53
		 * 104    0.22  0.42  0.77  1.36  2.62  5.33  8.93 13.05 29.54 15.23 22.53
		 * 112    0.15  0.20  0.56  0.87  2.00  3.83  6.86 10.06 17.89 27.31 30.27
		 * 120    0.03  0.11  0.31  0.46  1.31  2.48  4.60  7.78 11.67 25.53 45.72
		 * 128    0.02  0.01  0.13  0.33  0.83  1.41  3.24  6.17  9.57 14.22 64.07
		 */
		public static short m_bonus(int max, int level) {
			int val = Random.Rand_normal(level, max / 4); //It boils down to this...

			if(val > max)
				val = max;
			if(val < 0)
				val = 0;
			return (short)val;
			//int bonus, stand, value;

			///* Make sure level is reasonable */
			//if (level >= MAX_DEPTH) level = MAX_DEPTH - 1;

			///* The bonus approaches max as level approaches MAX_DEPTH */
			//bonus = simulate_division(max * level, MAX_DEPTH);

			///* The standard deviation is 1/4 of the max */
			//stand = simulate_division(max, 4);

			///* Choose a value */
			//value = Rand_normal(bonus, stand);

			///* Return, enforcing the min and max values */
			//if (value < 0)
			//    return 0;
			//else if (value > max)
			//    return max;
			//else
			//    return value;
		}

		/**
		 * Calculation helper function for m_bonus.
		 */
		public static int m_bonus_calc(int max, int level, aspect bonus_aspect) {
			switch (bonus_aspect) {
				case aspect.EXTREMIFY:
				case aspect.MAXIMISE:  return max;

				case aspect.RANDOMISE: return m_bonus(max, level);

				case aspect.MINIMISE:  return 0;

				case aspect.AVERAGE:   return max * level / Misc.MAX_DEPTH;
			}

			return 0;
		}

		/**
		 * Calculation helper function for random_value structs.
		 */
		public static int randcalc(random_value v, int level, aspect rand_aspect) {
			if (rand_aspect == aspect.EXTREMIFY) {
				int min = randcalc(v, level, aspect.MINIMISE);
				int max = randcalc(v, level, aspect.MAXIMISE);
				return Math.Abs(min) > Math.Abs(max) ? min : max;
			} else {
				int dmg   = damcalc(v.dice, v.sides, rand_aspect);
				int bonus = m_bonus_calc(v.m_bonus, level, rand_aspect);
				return v.Base + dmg + bonus;
			}
		}

		/**
		 * Test to see if a value is within a random_value's range.
		 */
		public static bool randcalc_valid(random_value v, int test) {
			throw new NotImplementedException();
			//return false;
		}

		/**
		 * Test to see if a random_value actually varies.
		 */
		public static bool randcalc_varies(random_value v) {
			throw new NotImplementedException();
			//return false;
		}

		public static void fix(int val) {
			isfixed = true;
			fixedval = val;
		}
	}
}
