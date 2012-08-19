using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	/*
	* Timed effects
	*/
	//TMD
	public enum Timed_Effect : int
	{
		FAST = 0, SLOW, BLIND, PARALYZED, CONFUSED,
		AFRAID, IMAGE, POISONED, CUT, STUN, PROTEVIL,
		INVULN, HERO, SHERO, SHIELD, BLESSED, SINVIS,
		SINFRA, OPP_ACID, OPP_ELEC, OPP_FIRE, OPP_COLD,
		OPP_POIS, OPP_CONF, AMNESIA, TELEPATHY, STONESKIN,
		TERROR, SPRINT, BOLD,

		MAX
	};
}
