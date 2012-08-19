using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Test {
		public Test(string Name, TestFunction TestFunc) {
			this.Name = Name;
			this.Function += TestFunc;
		}

		public string Name;

		public delegate void TestFunction();
		public TestFunction Function;
	}
}
