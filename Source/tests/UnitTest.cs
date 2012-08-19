using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class UnitTest {
		public static void Ok() {
			throw new UnitTest_Main.Success();
		}

		public static void Fail(string Message) {
			throw new UnitTest_Main.Failure(Message);
		}

		public static void Equal<T>(T X, T Y, string Message = null) {
			Require(X.Equals(Y), Message);
		}

		public static void Require(bool assert, string Message = null) {
			if(!assert) {
				throw new UnitTest_Main.Failure(Message);
			}
		}
	}
}
