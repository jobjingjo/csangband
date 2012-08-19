using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Target {
		/*** File-wide variables ***/

		/* Is the target set? */
		static bool target_set;

		/* Current monster being tracked, or 0 */
		static ushort target_who;

		/* Target location */
		static short target_x, target_y;

		/*
		 * Determine is a monster makes a reasonable target
		 *
		 * The concept of "targetting" was stolen from "Morgul" (?)
		 *
		 * The player can target any location, or any "target-able" monster.
		 *
		 * Currently, a monster is "target_able" if it is visible, and if
		 * the player can hit it with a projection, and the player is not
		 * hallucinating.  This allows use of "use closest target" macros.
		 *
		 * Future versions may restrict the ability to target "trappers"
		 * and "mimics", but the semantics is a little bit weird.
		 */
		public static bool able(int m_idx)
		{
			throw new NotImplementedException();
			//int py = p_ptr.py;
			//int px = p_ptr.px;

			//monster_type *m_ptr;

			///* No monster */
			//if (m_idx <= 0) return (false);

			///* Get monster */
			//m_ptr = cave_monster(cave, m_idx);

			///* Monster must be alive */
			//if (!m_ptr.r_idx) return (false);

			///* Monster must be visible */
			//if (!m_ptr.ml) return (false);
	
			///* Player must be aware this is a monster */
			//if (m_ptr.unaware) return (false);

			///* Monster must be projectable */
			//if (!projectable(py, px, m_ptr.fy, m_ptr.fx, PROJECT_NONE))
			//    return (false);

			///* Hack -- no targeting hallucinations */
			//if (p_ptr.timed[TMD_IMAGE]) return (false);

			///* Assume okay */
			//return (true);
		}

		/*
		 * Set the target to a monster (or nobody)
		 */
		public static void set_monster(int m_idx)
		{
			/* Acceptable target */
			if ((m_idx > 0) && able(m_idx))
			{
				throw new NotImplementedException();
				//monster_type *m_ptr = cave_monster(cave, m_idx);

				///* Save target info */
				//target_set = true;
				//target_who = m_idx;
				//target_y = m_ptr.fy;
				//target_x = m_ptr.fx;
			}

			/* Clear target */
			else
			{
			    /* Reset target info */
			    target_set = false;
			    target_who = 0;
			    target_y = 0;
			    target_x = 0;
			}
		}

		/**
		 * Returns the currently targeted monster index.
		 */
		public static short get_monster()
		{
			return (short)target_who;
		}


	}
}
