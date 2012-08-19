using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;

namespace CSAngband {
	class Grid_Data {
		public enum grid_light_level
		{
			FEAT_LIGHTING_BRIGHT = 0,
			FEAT_LIGHTING_LIT,
			FEAT_LIGHTING_DARK,
			FEAT_LIGHTING_MAX
		};

		public uint m_idx;		/* Monster index */
		public uint f_idx;		/* Feature index */
		public Object_Kind first_kind;	/* The "kind" of the first item on the grid */
		public bool multiple_objects;	/* Is there more than one item there? */

		public grid_light_level lighting; /* Light level */
		public bool in_view; /* true when the player can currently see the grid. */
		public bool is_player;
		public bool hallucinate;
		public bool trapborder;
	}
}
