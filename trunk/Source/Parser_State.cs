using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Parser_State {
		Parser.Error error;
		uint line;
		uint col;
		string msg;
	}
}
