using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Quark {
		private static List<Quark> quarks;
		public string value;

		public static void Init(){
			quarks = new List<Quark>();
		}

		public static void Free() {
			foreach (Quark q in quarks){
				q.value = null;
			}
			quarks = null;
		}

		public static Quark Add(string Value){
			foreach (Quark q in quarks){
				if(q.ToString() == Value) {
					return q;
				}
			}

			Quark ret = new Quark();
			ret.value = Value;

			quarks.Add(ret);

			return ret;
		}

		public override string ToString() {
			return value;
		}
	}
}
