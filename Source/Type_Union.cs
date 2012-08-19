using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	enum type_val {
		NONE = 0,
		INT,
		FLOAT,
		CHAR,
		STRING
	}

	class Type_Union {
		public Type_Union() {
			t = type_val.NONE;
		}

		public Type_Union(string str) {
			t = type_val.STRING;
			s = str;
		}

		public Type_Union(int val) {
			t = type_val.INT;
			i = val;
		}

		public Type_Union(float val) {
			t = type_val.FLOAT;
			f = val;
		}

		public type_val t;

		public float f;
		public int i;
		public char c;
		public string s;

		public object value {
			get {
				if (t == type_val.STRING) {
					return s;
				} else if (t == type_val.INT) {
					return i;
				} else if(t == type_val.FLOAT) {
					return f;
				} else if(t == type_val.CHAR) {
					return c;
				}

				return null;
			}
		}
	}
}
