using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Vault {
		
		/*
		 * Information about "vault generation"
		 */
		public Vault next;
		public uint vidx;
		public string name;
		public string text;

		public byte typ;			/* Vault type */

		public byte rat;			/* Vault rating */

		public byte hgt;			/* Vault height */
		public byte wid;			/* Vault width */

	}
}
