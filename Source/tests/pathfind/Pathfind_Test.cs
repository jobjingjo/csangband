using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	class Pathfind_Test : UnitTest {
		public static void Register() {
			Suite suite = new Suite();
			suite.Name = "pathfind/pathfind";

			suite.NoSetup();
			suite.NoTeardown();


			suite.AddTest("dir-to", test_dir_to);

			UnitTest_Main.AddSuite(suite);
		}

		public static void test_dir_to() {
			Equal(Pathfind.direction_to(new Loc(0, 0), new Loc(0, 1)), Direction.N);
			Equal(Pathfind.direction_to(new Loc(0, 0), new Loc(1, 0)), Direction.E);
			Equal(Pathfind.direction_to(new Loc(0, 0), new Loc(1, 1)), Direction.NE);
			Equal(Pathfind.direction_to(new Loc(0, 0), new Loc(0, -1)), Direction.S);
			Equal(Pathfind.direction_to(new Loc(0, 0), new Loc(-1, 0)), Direction.W);
			Equal(Pathfind.direction_to(new Loc(0, 0), new Loc(-1, -1)), Direction.SW);
			Equal(Pathfind.direction_to(new Loc(0, 0), new Loc(-1, 1)), Direction.NW);
			Equal(Pathfind.direction_to(new Loc(0, 0), new Loc(1, -1)), Direction.SE);
			Equal(Pathfind.direction_to(new Loc(0, 0), new Loc(0, 0)), Direction.NONE);

			Equal(Pathfind.direction_to(new Loc(0, 0), new Loc(1, 10)), Direction.N);
			Equal(Pathfind.direction_to(new Loc(0, 0), new Loc(8, 10)), Direction.NE);
			Equal(Pathfind.direction_to(new Loc(0, 0), new Loc(12, 4)), Direction.E);
			Ok();
		}
	}
}
