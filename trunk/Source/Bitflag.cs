using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Bitflag {
		private int size;
		public byte[] data;

		public Bitflag(int size){
			this.size = size;
			data = new byte[size];

			for(int i = 0; i < data.Length; i++) {
				data[i] = 0;
			}
		}

		public byte this[int i]{
			get {
				return data[i];
			}
			set {
				data[i] = value;
			}
		}

		/* The basic datatype of bitflags */
		public const int FLAG_WIDTH = (sizeof(byte)*8);//the *8 is to get # of bits

		/* Enum flag value of the first valid flag in a set
		 * Enums must be manually padded with the number of dummy elements
		 */
		public const int FLAG_START = 1;

		/* Sentinel value indicates no more flags present for va-arg functions */
		//shouldn't need this since C# is magic
		//THIS JUST IN!!! this now represents a "null" flag... since no flag should ever be flag 0
		//Original comment is still a lie, we don't use it for that
		//In fact, I've renamed this "FLAG_END" to be "FLAG_null" to better represent it's actual use
		public const int FLAG_null  = (FLAG_START - 1);

		/* The array size necessary to hold "n" flags */
		public static int FLAG_SIZE(int n){
			return (((n) + FLAG_WIDTH - 1) / FLAG_WIDTH);
		}

		/* The highest flag value plus one in an array of size "n" */
		public static int FLAG_MAX(int n){
			return (int)((n) * FLAG_WIDTH + FLAG_START);
		}

		/* Convert a sequential flag enum value to its array index */
		public static int FLAG_OFFSET(int id){
			return (((id) - FLAG_START) / FLAG_WIDTH);
		}

		/* Convert a sequential flag enum value to its binary flag value. */
		public static byte FLAG_BINARY(int id){
			return (byte)(1 << ((id) - FLAG_START) % FLAG_WIDTH);
		}


		//All below functions had flags and size. flags was a byte* and size was a size_t

		/**
		 * Sets multiple bitflags in a bitfield.
		 *
		 * The flags specified in `...` are set in `flags`. The bitfield size is
		 * supplied in `size`. true is returned when changes were made, false
		 * otherwise.
		 *
		 * WARNING: FLAG_END must be the final argument in the `...` list. <- LIES
		 */
		public bool set(params int[] values)
		{
			bool delta = false;

			/* Process each flag in the va-args */
			for (int i = 0; i < values.Length; i++)
			{
				int f = values[i];

				int flag_offset = FLAG_OFFSET(f);
				byte flag_binary = FLAG_BINARY(f);

				Misc.assert(flag_offset < size);

				/* !flag_has() */
				if ((data[flag_offset] & flag_binary) == 0) delta = true;

				/* flag_on() */
				data[flag_offset] |= flag_binary;
			}

			return delta;
		}

		/**
		 * Tests if a flag is "on" in a bitflag set.
		 *
		 * true is returned when `flag` is on in `flags`, and false otherwise.
		 * The flagset size is supplied in `size`.
		 */
		public bool has(int flag)
		{
			int flag_offset = FLAG_OFFSET(flag);
			byte flag_binary = FLAG_BINARY(flag);

			//if (flag == FLAG_END) return false;

			Misc.assert(flag_offset < size);

			return ((data[flag_offset] & flag_binary) != 0);
		}

		//fi and fl were const char*
		bool flag_has_dbg(int flag, string fi, string fl)
		{
			throw new NotImplementedException();
			/*
			const size_t flag_offset = FLAG_OFFSET(flag);
			const int flag_binary = FLAG_BINARY(flag);

			if (flag == FLAG_END) return false;

			if (flag_offset >= size)
			{
				quit_fmt("Error in flag_has(%s, %s): FlagID[%d] Size[%u] FlagOff[%u] FlagBV[%d]\n",
						 fi, fl, flag, (unsigned int) size, (unsigned int) flag_offset, flag_binary);
			}

			assert(flag_offset < size);

			if (flags[flag_offset] & flag_binary) return true;

			return false;*/
		}


		/**
		 * Interates over the flags which are "on" in a bitflag set.
		 *
		 * Returns the next on flag in `flags`, starting from (and including)
		 * `flag`. FLAG_END will be returned when the end of the flag set is reached.
		 * Iteration will start at the beginning of the flag set when `flag` is
		 * FLAG_END. The bitfield size is supplied in `size`.
		 */
		public int next(int flag)
		{
			int max_flags = FLAG_MAX(size);
			int f, flag_offset, flag_binary;

			for (f = flag; f < max_flags; f++)
			{
				flag_offset = FLAG_OFFSET(f);
				flag_binary = FLAG_BINARY(f);

				if ((data[flag_offset] & flag_binary) != 0) return f;
			}

			return FLAG_null;
		}


		/**
		 * Tests a bitfield for emptiness.
		 *
		 * true is returned when no flags are set in `flags`, and false otherwise.
		 * The bitfield size is supplied in `size`.
		 */
		public bool is_empty()
		{
			for (int i = 0; i < size; i++)
				if (data[i] > 0) return false;

			return true;
		}


		/**
		 * Tests a bitfield for fullness.
		 *
		 * true is returned when all flags are set in `flags`, and false otherwise.
		 * The bitfield size is supplied in `size`.
		 */
		public bool is_full()
		{
			throw new NotImplementedException();
			/*
			size_t i;

			for (i = 0; i < size; i++)
				if (flags[i] != (bitflag) -1) return false;

			return true;*/
		}


		/**
		 * Tests two bitfields for intersection.
		 *
		 * true is returned when any flag is set in both `flags1` and `flags2`, and
		 * false otherwise. The size of the bitfields is supplied in `size`.
		 */
		public bool is_inter(Bitflag flags2)
		{
			for (int i = 0; i < size; i++)
				if ((data[i] & flags2.data[i]) != 0) return true;

			return false;
		}


		/**
		 * Test if one bitfield is a subset of another.
		 *
		 * true is returned when every set flag in `flags2` is also set in `flags1`,
		 * and false otherwise. The size of the bitfields is supplied in `size`.
		 */
		bool flag_is_subset(Bitflag flags1, Bitflag flags2)
		{
			throw new NotImplementedException();
			/*
			size_t i;

			for (i = 0; i < size; i++)
				if (~flags1[i] & flags2[i]) return false;

			return true;*/
		}


		/**
		 * Tests two bitfields for equality.
		 *
		 * true is returned when the flags set in `flags1` and `flags2` are identical,
		 * and false otherwise. the size of the bitfields is supplied in `size`.
		 */
		public bool is_equal(Bitflag flags)
		{
			for(int i = 0; i < data.Length; i++) {
				if(data[i] != flags.data[i]) {
					return false;
				}
			}
			return true;
		}


		/**
		 * Sets one bitflag in a bitfield.
		 *
		 * The bitflag identified by `flag` is set in `flags`. The bitfield size is
		 * supplied in `size`.  true is returned when changes were made, false
		 * otherwise.
		 */
		public bool on(int flag)
		{
			int flag_offset = FLAG_OFFSET(flag);
			byte flag_binary = FLAG_BINARY(flag);

			Misc.assert(flag_offset < size);

			if ((data[flag_offset] & flag_binary) == 1) return false;

			data[flag_offset] |= flag_binary;

			return true;
		}

		//fi and fl were const char *
		bool flag_on_dbg(int flag, string fi, string fl)
		{
			throw new NotImplementedException();
			/*const size_t flag_offset = FLAG_OFFSET(flag);
			const int flag_binary = FLAG_BINARY(flag);

			if (flag_offset >= size)
			{
				quit_fmt("Error in flag_on(%s, %s): FlagID[%d] Size[%u] FlagOff[%u] FlagBV[%d]\n",
						 fi, fl, flag, (unsigned int) size, (unsigned int) flag_offset, flag_binary);
			}

			assert(flag_offset < size);

			if (flags[flag_offset] & flag_binary) return false;

			flags[flag_offset] |= flag_binary;

			return true;*/
		}


		/**
		 * Clears one flag in a bitfield.
		 *
		 * The bitflag identified by `flag` is cleared in `flags`. The bitfield size
		 * is supplied in `size`.  true is returned when changes were made, false
		 * otherwise.
		 */
		public bool off(int flag)
		{
			int flag_offset = FLAG_OFFSET(flag);
			int flag_binary = FLAG_BINARY(flag);

			Misc.assert(flag_offset < size);

			if ((data[flag_offset] & flag_binary) == 0) return false;

			data[flag_offset] &= (byte)~flag_binary;

			return true;
		}


		/**
		 * Clears all flags in a bitfield.
		 *
		 * All flags in `flags` are cleared. The bitfield size is supplied in `size`.
		 */
		public void wipe()
		{
			for(int i = 0; i < size; i++) {
				data[i] = 0;
			}
		}


		/**
		 * Sets all flags in a bitfield.
		 *
		 * All flags in `flags` are set. The bitfield size is supplied in `size`.
		 */
		public void setall()
		{
			for(int i = 0; i < data.Length; i++) {
				data[i] = 255;
			}
		}


		/**
		 * Negates all flags in a bitfield.
		 *
		 * All flags in `flags` are toggled. The bitfield size is supplied in `size`.
		 */
		void flag_negate()
		{
			throw new NotImplementedException();
			/*size_t i;
	
			for (i = 0; i < size; i++)
				flags[i] = ~flags[i];*/
		}


		/**
		 * Copies one bitfield into another.
		 *
		 * All flags in `flags2` are copied into `flags1`. The size of the bitfields is
		 * supplied in `size`.
		 */
		public void copy(Bitflag flags)
		{
			flags.data.CopyTo(data, 0);
		}


		/**
		 * Computes the union of two bitfields.
		 *
		 * For every set flag in `flags2`, the corresponding flag is set in `flags1`.
		 * The size of the bitfields is supplied in `size`. true is returned when
		 * changes were made, and false otherwise.
		 */
		public bool union(Bitflag flags)
		{
			bool delta = false;

			for (int i = 0; i < size; i++)
			{
				/* !flag_is_subset() */
				//There was no != originally...
				if ((~data[i] & flags.data[i]) != 0) delta = true;

				data[i] |= flags.data[i];
			}

			return delta;
		}


		/**
		 * Computes the union of one bitfield and the complement of another.
		 *
		 * For every unset flag in `flags2`, the corresponding flag is set in `flags1`.
		 * The size of the bitfields is supplied in `size`. true is returned when
		 * changes were made, and false otherwise.
		 */
		bool flag_comp_union(Bitflag flags1, Bitflag flags2)
		{
			throw new NotImplementedException();
			/*size_t i;
			bool delta = false;

			for (i = 0; i < size; i++)
			{*/
				/* no equivalent fn */
				/*if (!(~flags1[i] & ~flags2[i])) delta = true;

				flags1[i] |= ~flags2[i];
			}

			return delta;*/
		}


		/**
		 * Computes the intersection of two bitfields.
		 *
		 * For every unset flag in `flags2`, the corresponding flag is cleared in
		 * `flags1`. The size of the bitfields is supplied in `size`. true is returned
		 * when changes were made, and false otherwise.
		 */
		//flags1 = this, flags2 = flags
		public bool inter(Bitflag flags)
		{
			bool delta = false;

			for (int i = 0; i < size; i++)
			{
				/* !flag_is_equal() */
				if (!(data[i] == flags.data[i])) delta = true;

				data[i] &= flags.data[i];
			}

			return delta;
		}


		/**
		 * Computes the difference of two bitfields.
		 *
		 * For every set flag in `flags2`, the corresponding flag is cleared in
		 * `flags1`. The size of the bitfields is supplied in `size`. true is returned
		 * when changes were made, and false otherwise.
		 */
		public bool diff(Bitflag flags2)
		{
			int i;
			bool delta = false;

			for (i = 0; i < size; i++)
			{
				/* flag_is_inter() */
				if ((data[i] & flags2.data[i]) != 0) delta = true;

				data[i] &= (byte)~flags2.data[i];
			}

			return delta;
		}





		/**
		 * Tests if any of multiple bitflags are set in a bitfield.
		 *
		 * true is returned if any of the flags specified in `...` are set in `flags`,
		 * false otherwise. The bitfield size is supplied in `size`.
		 *
		 * WARNING: FLAG_END must be the final argument in the `...` list.
		 */
		public bool test(params int[] values)
		{
			foreach(int val in values) {
				if(has(val)) {
					return true;
				}
			}

			return false;
			///*size_t flag_offset;
			//int flag_binary;
			//int f;
			//va_list args;
			//bool delta = false;*/

			//va_start(args, size);

			///* Process each flag in the va-args */
			//for (f = va_arg(args, int); f != FLAG_END; f = va_arg(args, int))
			//{
			//    flag_offset = FLAG_OFFSET(f);
			//    flag_binary = FLAG_BINARY(f);

			//    assert(flag_offset < size);

			//    /* flag_has() */
			//    if (flags[flag_offset] & flag_binary)
			//    {
			//        delta = true;
			//        break;
			//    }
			//}

			//va_end(args);
	
			//return delta;
		}


		/**
		 * Tests if all of the multiple bitflags are set in a bitfield.
		 *
		 * true is returned if all of the flags specified in `...` are set in `flags`,
		 * false otherwise. The bitfield size is supplied in `size`. 
		 *
		 * WARNING: FLAG_END must be the final argument in the `...` list.
		 */
		public bool test_all(params int[] values)
		{

			int flag_offset;
			int flag_binary;
			bool delta = true;

			/* Process each flag in the va-args */
			for (int i = 0; i < values.Length; i++)
			{
				int f = values[i];
			    flag_offset = FLAG_OFFSET(f);
			    flag_binary = FLAG_BINARY(f);

			    Misc.assert(flag_offset < size);

			    /* !flag_has() */
			    if ((data[flag_offset] & flag_binary) == 0)
			    {
			        delta = false;
			        break;
			    }
			}
	
			return delta;



			//size_t flag_offset;
			//int flag_binary;
			//int f;
			//va_list args;
			//bool delta = true;

			//va_start(args, size);

			///* Process each flag in the va-args */
			//for (f = va_arg(args, int); f != FLAG_END; f = va_arg(args, int))
			//{
			//    flag_offset = FLAG_OFFSET(f);
			//    flag_binary = FLAG_BINARY(f);

			//    assert(flag_offset < size);

			//    /* !flag_has() */
			//    if (!(flags[flag_offset] & flag_binary))
			//    {
			//        delta = false;
			//        break;
			//    }
			//}

			//va_end(args);
	
			//return delta;
		}


		/**
		 * Clears multiple bitflags in a bitfield.
		 *
		 * The flags specified in `...` are cleared in `flags`. The bitfield size is
		 * supplied in `size`. true is returned when changes were made, false
		 * otherwise.
		 *
		 * WARNING: FLAG_END must be the final argument in the `...` list.
		 */
		public bool clear(params int[] values)
		{
			throw new NotImplementedException();
			/*size_t flag_offset;
			int flag_binary;
			int f;
			va_list args;
			bool delta = false;

			va_start(args, size);
*/
			/* Process each flag in the va-args */
			/*for (f = va_arg(args, int); f != FLAG_END; f = va_arg(args, int))
			{
				flag_offset = FLAG_OFFSET(f);
				flag_binary = FLAG_BINARY(f);

				assert(flag_offset < size);
*/
				/* flag_has() */
				//if (flags[flag_offset] & flag_binary) delta = true;

				/* flag_off() */
				//flags[flag_offset] &= ~flag_binary;
			/*}

			va_end(args);

			return delta;*/
		}


		


		/**
		 * Wipes a bitfield, and then sets multiple bitflags.
		 *
		 * The flags specified in `...` are set in `flags`, while all other flags are
		 * cleared. The bitfield size is supplied in `size`.
		 *
		 * WARNING: FLAG_END must be the final argument in the `...` list.
		 */
		void flags_init(params int[] values)
		{
			throw new NotImplementedException();
			/*int f;
			va_list args;

			flag_wipe(flags, size);

			va_start(args, size);
*/
			/* Process each flag in the va-args */
			/*for (f = va_arg(args, int); f != FLAG_END; f = va_arg(args, int))
				flag_on(flags, size, f);

			va_end(args);*/
		}


		/**
		 * Computes the intersection of a bitfield and multiple bitflags.
		 *
		 * The flags not specified in `...` are cleared in `flags`. The bitfeild size
		 * is supplied in `size`. true is returned when changes were made, false
		 * otherwise.
		 *
		 * WARNING: FLAG_END must be the final argument in the `...` list.
		 */
		bool flags_mask(params int[] values)
		{
			throw new NotImplementedException();
			/*int f;
			va_list args;
			bool delta = false;

			bitflag *mask;
*/
			/* Build the mask */
		/*	mask = C_ZNEW(size, bitflag);

			va_start(args, size);
*/
			/* Process each flag in the va-args */
			/*for (f = va_arg(args, int); f != FLAG_END; f = va_arg(args, int))
				flag_on(mask, size, f);

			va_end(args);

			delta = flag_inter(flags, mask, size);
*/
			/* Free the mask */
/*
			FREE(mask);

			return delta;*/
		}
	}
}
