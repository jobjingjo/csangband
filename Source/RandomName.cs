using System;

namespace CSAngband {
	static class RandomName {
		/*
		 * The different types of name randname.c can generate
		 * which is also the number of sections in names.txt
		 */
		public enum randname_type
		{
			RANDNAME_TOLKIEN = 1, //was previously 1
			RANDNAME_SCROLL,
 
			/* End of type marker - not a valid name type */
			RANDNAME_NUM_TYPES 
		}

		/* Markers for the start and end of words. */
		const int S_WORD = 26;
		const int E_WORD = S_WORD;
		const int TOTAL = 27;

		//this was typedef'd too
		//ushort[,,] name_probs = new ushort[S_WORD+1, S_WORD+1, TOTAL+1];

		/*
		 * Array[RANDNAME_NUM_TYPES][num_names] of random names
		 */
		//const char ***
		public static string[][] name_sections;

		/*
		 * This function builds probability tables from a list of purely alphabetical
		 * lower-case words, and puts them into the supplied name_probs object.
		 * The array of names should have a null entry at the end of the list.
		 * It relies on the ASCII character set (through use of A2I).
		 */
		//learn was const char **
		static void build_prob(ushort[,,] probs, string[] learn)
		{
			int c_prev, c_cur, c_next;
			string ch;
			int i;

			/* Build raw frequencies */
			for (i = 0; learn[i] != null; i++)
			{
			    c_prev = c_cur = S_WORD;
			    ch = learn[i];

			    /* Iterate over the next word */
				for (int j = 0; j < ch.Length; j++)
			    //while (*ch != '\0')
			    {
					char curr = ch[j];
			        c_next = Basic.A2I(Char.ToLower(curr));

			        probs[c_prev, c_cur, c_next]++;
			        probs[c_prev, c_cur, TOTAL]++;
                        
			        /* Step on */
			        c_prev = c_cur;
			        c_cur = c_next;
			        //ch++; //j++
			    }

			    probs[c_prev, c_cur, E_WORD]++;
			    probs[c_prev, c_cur, TOTAL]++;
			}
		}

		/*
		 * Use W. Sheldon Simms' random name generator algorithm (Markov Chain stylee).
		 * 
		 * Generate a random word using the probability tables we built earlier.  
		 * Relies on the A2I and I2A macros (and so the ASCII character set) and 
		 * is_a_vowel (so the basic 5 English vowels).
		 */
		//sections was const char ***
		static ushort[,,] lprobs = new ushort[S_WORD+1, S_WORD+1, TOTAL+1];
		static randname_type cached_type = randname_type.RANDNAME_NUM_TYPES;
		public static int randname_make(randname_type name_type, int min, int max, ref string word_buf, int buflen, string [][]sections)
		{
			int lnum = 0;
			bool found_word = false;

			Misc.assert(name_type > 0 && name_type < randname_type.RANDNAME_NUM_TYPES);

			/* To allow for a terminating character */
			Misc.assert(buflen > max);

			/* We cache one set of probabilities, only regenerate when
			   the type changes.  It's as good a way as any for now.
			   Frankly, we could probably regenerate every time. */
			if (cached_type != name_type)
			{
				string[] wordlist = null;
			    //const char **wordlist = null;

			    wordlist = sections[(int)name_type];

			    build_prob(lprobs, wordlist);

			    cached_type = name_type;
			}
        
			/* Generate the actual word wanted. */
			while (!found_word)
			{
				string cp = word_buf;
			    //char *cp = word_buf;
			    int c_prev = S_WORD;
			    int c_cur = S_WORD;
			    int tries = 0;
			    bool contains_vowel = false;
			    lnum = 0;

			    /* We start the word again if we run out of space or have
			       had to have 10 goes to find a word that satisfies the
			       minimal conditions. */
			    while (tries < 10 && lnum <= max && !found_word)
			    {
			        /* Pick the next letter based on a simple weighting
			          of which letters can follow the previous two */
			        int r;
			        int c_next = 0;

			        Misc.assert(c_prev >= 0 && c_prev <= S_WORD);
			        Misc.assert(c_cur >= 0 && c_cur <= S_WORD);

			        r = (int)Random.randint0(lprobs[c_prev, c_cur, TOTAL]);

			        while (r >= lprobs[c_prev, c_cur, c_next])
			        {
			            r -= lprobs[c_prev, c_cur, c_next];
			            c_next++;
			        }

			        Misc.assert(c_next <= E_WORD);
			        Misc.assert(c_next >= 0);
            
			        if (c_next == E_WORD)
			        {
			            /* If we've reached the end, we check if we've
			               met the simple conditions, otherwise have
			               another go at choosing a letter for this
			               position. */
			            if (lnum >= min && contains_vowel)
			            {
			                //cp = "";// = '\0'; We don't need null terminator anymore...
							word_buf = cp;
			                found_word = true;
			            }
			            else
			            {
			                tries++;
			            }
			        }
			        else
			        {
			            /* Add the letter to the word and move on. */
						char curr = Basic.I2A(c_next);
						cp += curr;
			            //*cp = I2A(c_next);

			            if (is_a_vowel(curr))
			                contains_vowel = true;

			            //cp++;//this was handled by cp += curr...
			            lnum++;
			            Misc.assert(c_next <= S_WORD);
			            Misc.assert(c_next >= 0);
			            c_prev = c_cur;
			            c_cur = c_next;
			        }
			    }
			}

			return lnum;
		}


		static bool is_a_vowel(char ch)
		{
			switch (ch)
			{
				case 'a':
				case 'e':
				case 'i':
				case 'o':
				case 'u':
					 return (true);
			}

			return (false);
		}

		/*public static void init() {
			name_sections = new string[(int)randname_type.RANDNAME_NUM_TYPES][];

		}*/

		public static void Main(string[] args)
		{
			int i;
			string name = ""; //char name[256];

			Random.Rand_value = (uint)DateTime.Now.Millisecond;

			for (i = 0; i < 20; i++)
			{
				randname_make(randname_type.RANDNAME_TOLKIEN, 5, 9, ref name, 256, name_sections);
				name = Char.ToUpper(name[0]) + name.Substring(1);
				Console.Out.Write(name + "\n");
			}

			Console.In.ReadLine();
		}
	}
}
