using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Term_Win {
		/*
		 * A term_win is a "window" for a Term
		 *
		 *	- Cursor Useless/Visible codes
		 *	- Cursor Location (see "Useless")
		 *
		 *	- Array[h] -- Access to the attribute array
		 *	- Array[h] -- Access to the character array
		 *
		 *	- Array[h*w] -- Attribute array
		 *	- Array[h*w] -- Character array
		 *
		 *	- next screen saved
		 *	- hook to be called on screen size change
		 *
		 * Note that the attr/char pair at (x,y) is a[y][x]/c[y][x]
		 * and that the row of attr/chars at (0,y) is a[y]/c[y]
		 */
		public bool cu, cv;
		public byte cx, cy;

		//all below arrays were pointers
		//alright, so all the 1D arrays are just concatinations of the 2D arrays... let's just use properties
		//note: they were linked in init, so this just emulates functionality... but slower.
		//update: properties suck, so we are just going to disable the big array and try to work around it.
		public ConsoleColor[][] a;
		public char[][] c;

		//byte[] va; //disabled, thanks to no pointers in C#
		//char[] vc;

		public ConsoleColor[][] ta;
		public char[][] tc;

		//byte[] vta;
		//char[] vtc;

		public Term_Win next;

		/*
		 * Initialize a "term_win" (using the given window size)
		 */
		public int init(int w, int h)
		{
			/* Make the window access arrays */
			a = new ConsoleColor[h][];
			c = new char[h][];

			/* Make the window content arrays */
			//va = new byte[h * w];
			//vc = new char[h * w];

			/* Make the terrain access arrays */
			ta = new ConsoleColor[h][];
			tc = new char[h][];

			/* Make the terrain content arrays */
			//vta = new byte[h * w];
			//vtc = new char[h * w];

			/* Prepare the window access arrays */
			for (int y = 0; y < h; y++)
			{
			    /*a[y] = va + (w * y);
			    c[y] = vc + (w * y);

			    ta[y] = vta + (w * y);
			    tc[y] = vtc + (w * y);*/

				a[y] = new ConsoleColor[w];
			    c[y] = new char[w];

			    ta[y] = new ConsoleColor[w];
			    tc[y] = new char[w];
			}

			/* Success */
			return (0);
		}


		/*
		 * Copy a "term_win" from another
		 */
		public int copy(Term_Win f, int w, int h)
		{
			int x, y;

			/* Copy contents */
			for (y = 0; y < h; y++)
			{
			    ConsoleColor[] f_aa = f.a[y];
			    char[] f_cc = f.c[y];

			    ConsoleColor[] s_aa = a[y];
			    char[] s_cc = c[y];

			    ConsoleColor[] f_taa = f.ta[y];
			    char[] f_tcc = f.tc[y];

			    ConsoleColor[] s_taa = ta[y];
			    char[] s_tcc = tc[y];

			    for (x = 0; x < w; x++)
			    {
					s_aa[x] = f_aa[x];
					s_cc[x] = f_cc[x];

					s_taa[x] = f_taa[x];
					s_tcc[x] = f_tcc[x];
			    }
			}

			/* Copy cursor */
			cx = f.cx;
			cy = f.cy;
			cu = f.cu;
			cv = f.cv;

			/* Success */
			return (0);
		}

		/*
		 * Nuke a term_win (see below)
		 */
		public int nuke()
		{
			///* Free the window access arrays */
			//FREE(s.a);
			//FREE(s.c);

			///* Free the window content arrays */
			//FREE(s.va);
			//FREE(s.vc);

			///* Free the terrain access arrays */
			//FREE(s.ta);
			//FREE(s.tc);

			///* Free the terrain content arrays */
			//FREE(s.vta);
			//FREE(s.vtc);

			///* Success */
			return (0);
		}
	}
}
