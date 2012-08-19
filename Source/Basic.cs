using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Basic { //h-basic.h
		/*** Some hackish character manipulation ***/
		/*
		 * Note that all "index" values must be "lowercase letters", while
		 * all "digits" must be "digits".
		 */
		public static int A2I(char X){ //Auto detect capital in this bitch, hellz yeah
			char t = Char.ToLower(X);
			return t - 'a';
			//if(X >= 'A') {
			//    return X - 'A';
			//}
			//return ((X) - 'a');
		}
		public static char I2A(int X){
			return (char)(((char)X) + 'a');
		}
		public static int D2I(char X){
			return ((X) - '0');
		}
		public static char I2D(int X){
			return (char)((X) + '0');
		}
	}
}
