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

		Term_Win next;

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
		static int copy(Term_Win f, int w, int h)
		{
			throw new NotImplementedException();
			//int x, y;

			///* Copy contents */
			//for (y = 0; y < h; y++)
			//{
			//    byte *f_aa = f.a[y];
			//    char *f_cc = f.c[y];

			//    byte *s_aa = s.a[y];
			//    char *s_cc = s.c[y];

			//    byte *f_taa = f.ta[y];
			//    char *f_tcc = f.tc[y];

			//    byte *s_taa = s.ta[y];
			//    char *s_tcc = s.tc[y];

			//    for (x = 0; x < w; x++)
			//    {
			//        *s_aa++ = *f_aa++;
			//        *s_cc++ = *f_cc++;

			//        *s_taa++ = *f_taa++;
			//        *s_tcc++ = *f_tcc++;
			//    }
			//}

			///* Copy cursor */
			//s.cx = f.cx;
			//s.cy = f.cy;
			//s.cu = f.cu;
			//s.cv = f.cv;

			///* Success */
			//return (0);
		}

		/*
		 * Nuke a term_win (see below)
		 */
		static int nuke()
		{
			throw new NotImplementedException();
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
			//return (0);
		}
	}
}
