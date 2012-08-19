using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Suite {
		public String Name;

		public List<Test> Tests = new List<Test>();

		public void AddTest(string Name, Test.TestFunction Test) {
			Tests.Add(new Test(Name, Test));
		}

		public delegate void RedTape();
		public RedTape Setup;
		public RedTape Teardown;

		public void NoSetup() {
			this.Setup = delegate() {
			};
		}

		public void NoTeardown() {
			this.Teardown = delegate() {
			};
		}

		public void SetSetup(RedTape SetupFunction) {
			this.Setup = SetupFunction;
		}

		public void SetTeardown(RedTape TeardownFunction) {
			this.Teardown = TeardownFunction;
		}
	}
}
