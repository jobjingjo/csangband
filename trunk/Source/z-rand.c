#define M1 3
#define M2 24
#define M3 10

#define MAT0POS(t, v) (v ^ (v >> t))
#define MAT0NEG(t, v) (v ^ (v << (-(t))))
#define Identity(v) (v)

u32b state_i = 0;
u32b STATE[RAND_DEG] = {0, 0, 0, 0, 0, 0, 0, 0,
						0, 0, 0, 0, 0, 0, 0, 0,
						0, 0, 0, 0, 0, 0, 0, 0,
						0, 0, 0, 0, 0, 0, 0, 0};
u32b z0, z1, z2;

#define V0    STATE[state_i]
#define VM1   STATE[(state_i + M1) & 0x0000001fU]
#define VM2   STATE[(state_i + M2) & 0x0000001fU]
#define VM3   STATE[(state_i + M3) & 0x0000001fU]
#define VRm1  STATE[(state_i + 31) & 0x0000001fU]
#define newV0 STATE[(state_i + 31) & 0x0000001fU]
#define newV1 STATE[state_i]

static u32b WELLRNG1024a (void){
	z0      = VRm1;
	z1      = Identity(V0) ^ MAT0POS (8, VM1);
	z2      = MAT0NEG (-19, VM2) ^ MAT0NEG(-14,VM3);
	newV1   = z1 ^ z2; 
	newV0   = MAT0NEG (-11,z0) ^ MAT0NEG(-7,z1) ^ MAT0NEG(-13,z2);
	state_i = (state_i + 31) & 0x0000001fU;
	return STATE[state_i];
}
/* end WELL RNG */

/*
 * Simple RNG, implemented with a linear congruent algorithm.
 */
#define LCRNG(X) ((X) * 1103515245 + 12345)


/**
 * Whether to use the simple RNG or not.
 */
bool Rand_quick = true;

/**
 * The current "seed" of the simple RNG.
 */
u32b Rand_value;

static bool rand_fixed = false;
static u32b rand_fixval = 0;

/**
 * Initialize the complex RNG using a new seed.
 */
void Rand_state_init(u32b seed) {
	int i, j;

	/* Seed the table */
	STATE[0] = seed;

	/* Propagate the seed */
	for (i = 1; i < RAND_DEG; i++)
		STATE[i] = LCRNG(STATE[i - 1]);

	/* Cycle the table ten times per degree */
	for (i = 0; i < RAND_DEG * 10; i++) {
		/* Acquire the next index */
		j = (state_i + 1) % RAND_DEG;

		/* Update the table, extract an entry */
		STATE[j] += STATE[state_i];

		/* Advance the index */
		state_i = j;
	}
}


/**
 * Extract a "random" number from 0 to m - 1, via division.
 *
 * This method selects "random" 28-bit numbers, and then uses division to drop
 * those numbers into "m" different partitions, plus a small non-partition to
 * reduce bias, taking as the final value the first "good" partition that a
 * number falls into.
 *
 * This method has no bias, and is much less affected by patterns in the "low"
 * bits of the underlying RNG's. However, it is potentially non-terminating.
 */
u32b Rand_div(u32b m) {
	u32b r, n;

	/* Division by zero will result if m is larger than 0x10000000 */
	assert(m <= 0x10000000);

	/* Hack -- simple case */
	if (m <= 1) return (0);

	if (rand_fixed)
		return (rand_fixval * 1000 * (m - 1)) / (100 * 1000);

	/* Partition size */
	n = (0x10000000 / m);

	if (Rand_quick) {
		/* Use a simple RNG */
		/* Wait for it */
		while (1) {
			/* Cycle the generator */
			r = (Rand_value = LCRNG(Rand_value));

			/* Mutate a 28-bit "random" number */
			r = ((r >> 4) & 0x0FFFFFFF) / n;

			/* Done */
			if (r < m) break;
		}
	} else {
		/* Use a complex RNG */
		while (1) {
			/* Get the next pseudorandom number */
			r = WELLRNG1024a();

			/* Mutate a 28-bit "random" number */
			r = ((r >> 4) & 0x0FFFFFFF) / n;

			/* Done */
			if (r < m) break;
		}
	}

	/* Use the value */
	return (r);
}


/**
 * The number of entries in the "Rand_normal_table"
 */
#define RANDNOR_NUM	256

/**
 * The standard deviation of the "Rand_normal_table"
 */
#define RANDNOR_STD	64

/**
 * The normal distribution table for the "Rand_normal()" function (below)
 */
static s16b Rand_normal_table[RANDNOR_NUM] = {
	206,   613,   1022,  1430,  1838,  2245,  2652,  3058,
	3463,  3867,  4271,  4673,  5075,  5475,  5874,  6271,
	6667,  7061,  7454,  7845,  8234,  8621,  9006,  9389,
	9770,  10148, 10524, 10898, 11269,	11638,	12004,	12367,
	12727, 13085, 13440, 13792, 14140,	14486,	14828,	15168,
	15504, 15836, 16166, 16492, 16814,	17133,	17449,	17761,
	18069, 18374, 18675, 18972, 19266,	19556,	19842,	20124,
	20403, 20678, 20949, 21216, 21479,	21738,	21994,	22245,

	22493, 22737, 22977, 23213, 23446,	23674,	23899,	24120,
	24336, 24550, 24759, 24965, 25166,	25365,	25559,	25750,
	25937, 26120, 26300, 26476, 26649,	26818,	26983,	27146,
	27304, 27460, 27612, 27760, 27906,	28048,	28187,	28323,
	28455, 28585, 28711, 28835, 28955,	29073,	29188,	29299,
	29409, 29515, 29619, 29720, 29818,	29914,	30007,	30098,
	30186, 30272, 30356, 30437, 30516,	30593,	30668,	30740,
	30810, 30879, 30945, 31010, 31072,	31133,	31192,	31249,

	31304, 31358, 31410, 31460, 31509,	31556,	31601,	31646,
	31688, 31730, 31770, 31808, 31846,	31882,	31917,	31950,
	31983, 32014, 32044, 32074, 32102,	32129,	32155,	32180,
	32205, 32228, 32251, 32273, 32294,	32314,	32333,	32352,
	32370, 32387, 32404, 32420, 32435,	32450,	32464,	32477,
	32490, 32503, 32515, 32526, 32537,	32548,	32558,	32568,
	32577, 32586, 32595, 32603, 32611,	32618,	32625,	32632,
	32639, 32645, 32651, 32657, 32662,	32667,	32672,	32677,

	32682, 32686, 32690, 32694, 32698,	32702,	32705,	32708,
	32711, 32714, 32717, 32720, 32722,	32725,	32727,	32729,
	32731, 32733, 32735, 32737, 32739,	32740,	32742,	32743,
	32745, 32746, 32747, 32748, 32749,	32750,	32751,	32752,
	32753, 32754, 32755, 32756, 32757,	32757,	32758,	32758,
	32759, 32760, 32760, 32761, 32761,	32761,	32762,	32762,
	32763, 32763, 32763, 32764, 32764,	32764,	32764,	32765,
	32765, 32765, 32765, 32766, 32766, 32766, 32766, 32767,
};


/**
 * Generate a random integer number of NORMAL distribution
 *
 * The table above is used to generate a psuedo-normal distribution, in a
 * manner which is much faster than calling a transcendental function to
 * calculate a true normal distribution.
 *
 * Basically, entry 64 * N in the table above represents the number of times
 * out of 32767 that a random variable with normal distribution will fall
 * within N standard deviations of the mean.  That is, about 68 percent of the
 * time for N=1 and 95 percent of the time for N=2.
 *
 * The table above contains a "faked" final entry which allows us to pretend
 * that all values in a normal distribution are strictly less than four
 * standard deviations away from the mean.  This results in "conservative"
 * distribution of approximately 1/32768 values.
 *
 * Note that the binary search takes up to 16 quick iterations.
 */
s16b Rand_normal(int mean, int stand) {
	s16b tmp, offset;

	// foo
	s16b low = 0;
	s16b high = RANDNOR_NUM;

	/* Paranoia */
	if (stand < 1) return (mean);

	/* Roll for probability */
	tmp = (s16b)randint0(32768);

	/* Binary Search */
	while (low < high) {
		int mid = (low + high) >> 1;

		/* Move right if forced */
		if (Rand_normal_table[mid] < tmp) {
			low = mid + 1;
		} else {
			high = mid;
		}
	}

	/* Convert the index into an offset */
	offset = (long)stand * (long)low / RANDNOR_STD;

	/* One half should be negative */
	if (one_in_(2)) return (mean - offset);

	/* One half should be positive */
	return (mean + offset);
}


/**
 * Generates a random signed long integer X where `A` <= X <= `B`.
 * The integer X falls along a uniform distribution.
 *
 * Note that "rand_range(0, N-1)" == "randint0(N)".
 */
int rand_range(int A, int B) {
	if (A == B) return A;
	assert(A < B);

	return A + (s32b)Rand_div(1 + B - A);
}


/**
 * Perform division, possibly rounding up or down depending on the size of the
 * remainder and chance.
 */
static int simulate_division(int dividend, int divisor) {
	int quotient  = dividend / divisor;
	int remainder = dividend % divisor;
	if (randint0(divisor) < remainder) quotient++;
	return quotient;
}

/**
 * Test to see if a value is within a random_value's range
 */
bool randcalc_valid(random_value v, int test) {
	if (test < randcalc(v, 0, MINIMISE))
		return false;
	else if (test > randcalc(v, 0, MAXIMISE))
		return false;
	else
		return true;
}

/**
 * Test to see if a random_value actually varies
 */
bool randcalc_varies(random_value v) {
	return randcalc(v, 0, MINIMISE) != randcalc(v, 0, MAXIMISE);
}

/*
 * Another simple RNG that does not use any of the above state
 * (so can be used without disturbing the game's RNG state)
 */
int getpid(void);
u32b Rand_simple(u32b m) {
	static time_t seed;
	time_t v;
	v = time(null);
	seed = LCRNG(seed) + ((v << 16) ^ v ^ getpid());
	return (seed%m);
}
