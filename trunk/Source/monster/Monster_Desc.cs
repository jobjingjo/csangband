using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Monster;
using RBM = CSAngband.Monster.Monster_Blow.RBM;
using RBE = CSAngband.Monster.Monster_Blow.RBE;

namespace CSAngband.Monster {
	partial class Monster {
		/*
		 * Bit flags for the "monster_desc" function
		 */
		public enum Desc{
			OBJE	=	0x01	/* Objective (or Reflexive) */,
			POSS	=	0x02	/* Possessive (or Reflexive) */,
			IND1	=	0x04	/* Indefinites for hidden monsters */,
			IND2	=	0x08	/* Indefinites for visible monsters */,
			PRO1	=	0x10	/* Pronominalize hidden monsters */,
			PRO2	=	0x20	/* Pronominalize visible monsters */,
			HIDE	=	0x40	/* Assume the monster is hidden */,
			SHOW	=	0x80	/* Assume the monster is visible */
		}

		/*
		 * Build a string describing a monster in some way.
		 *
		 * We can correctly describe monsters based on their visibility.
		 * We can force all monsters to be treated as visible or invisible.
		 * We can build nominatives, objectives, possessives, or reflexives.
		 * We can selectively pronominalize hidden, visible, or all monsters.
		 * We can use definite or indefinite descriptions for hidden monsters.
		 * We can use definite or indefinite descriptions for visible monsters.
		 *
		 * Pronominalization involves the gender whenever possible and allowed,
		 * so that by cleverly requesting pronominalization / visibility, you
		 * can get messages like "You hit someone.  She screams in agony!".
		 *
		 * Reflexives are acquired by requesting Objective plus Possessive.
		 *
		 * I am assuming that no monster name is more than 65 characters long,
		 * so that "char desc[80];" is sufficiently large for any result, even
		 * when the "offscreen" notation is added.
		 *
		 * Note that the "possessive" for certain unique monsters will look
		 * really silly, as in "Morgoth, King of Darkness's".  We should
		 * perhaps add a flag to "remove" any "descriptives" in the name.
		 *
		 * Note that "offscreen" monsters will get a special "(offscreen)"
		 * notation in their name if they are visible but offscreen.  This
		 * may look silly with possessives, as in "the rat's (offscreen)".
		 * Perhaps the "offscreen" descriptor should be abbreviated.
		 *
		 * Mode Flags:
		 *   0x01 -. Objective (or Reflexive)
		 *   0x02 -. Possessive (or Reflexive)
		 *   0x04 -. Use indefinites for hidden monsters ("something")
		 *   0x08 -. Use indefinites for visible monsters ("a kobold")
		 *   0x10 -. Pronominalize hidden monsters
		 *   0x20 -. Pronominalize visible monsters
		 *   0x40 -. Assume the monster is hidden
		 *   0x80 -. Assume the monster is visible
		 *
		 * Useful Modes:
		 *   0x00 -. Full nominative name ("the kobold") or "it"
		 *   0x04 -. Full nominative name ("the kobold") or "something"
		 *   0x80 -. Banishment resistance name ("the kobold")
		 *   0x88 -. Killing name ("a kobold")
		 *   0x22 -. Possessive, genderized if visable ("his") or "its"
		 *   0x23 -. Reflexive, genderized if visable ("himself") or "itself"
		 */
		public string monster_desc(Desc in_mode)
		{
			string res = "ERROR IN MONSTER_DESC";
			int mode = (int)in_mode;

			Monster_Race r_ptr = Misc.r_info[r_idx];

			string name = r_ptr.Name;

			/* Can we "see" it (forced, or not hidden + visible) */
			bool seen = (((mode & (0x80)) != 0) || (((mode & (0x40)) == 0) && ml));

			/* Sexed Pronouns (seen and forced, or unseen and allowed) */
			bool pron = ((seen && ((mode & (0x20)) != 0)) || (!seen && ((mode & (0x10)) != 0)));


			/* First, try using pronouns, or describing hidden monsters */
			if (!seen || pron)
			{
				/* an encoding of the monster "sex" */
				int kind = 0x00;

				/* Extract the gender (if applicable) */
				if (r_ptr.flags.has(Monster_Flag.FEMALE.value)) kind = 0x20;
				else if (r_ptr.flags.has(Monster_Flag.MALE.value)) kind = 0x10;

				/* Ignore the gender (if desired) */
				if (!pron) kind = 0x00;


				/* Assume simple result */
				res = "it";

				/* Brute force: split on the possibilities */
				switch (kind + (mode & 0x07))
				{
					/* Neuter, or unknown */
					case 0x00: res = "it"; break;
					case 0x01: res = "it"; break;
					case 0x02: res = "its"; break;
					case 0x03: res = "itself"; break;
					case 0x04: res = "something"; break;
					case 0x05: res = "something"; break;
					case 0x06: res = "something's"; break;
					case 0x07: res = "itself"; break;

					/* Male (assume human if vague) */
					case 0x10: res = "he"; break;
					case 0x11: res = "him"; break;
					case 0x12: res = "his"; break;
					case 0x13: res = "himself"; break;
					case 0x14: res = "someone"; break;
					case 0x15: res = "someone"; break;
					case 0x16: res = "someone's"; break;
					case 0x17: res = "himself"; break;

					/* Female (assume human if vague) */
					case 0x20: res = "she"; break;
					case 0x21: res = "her"; break;
					case 0x22: res = "her"; break;
					case 0x23: res = "herself"; break;
					case 0x24: res = "someone"; break;
					case 0x25: res = "someone"; break;
					case 0x26: res = "someone's"; break;
					case 0x27: res = "herself"; break;
				}
			}


			/* Handle visible monsters, "reflexive" request */
			else if ((mode & 0x02) != 0 && (mode & 0x01) != 0)
			{
				/* The monster is visible, so use its gender */
				if (r_ptr.flags.has(Monster_Flag.FEMALE.value)) res = "herself";
				else if (r_ptr.flags.has(Monster_Flag.MALE.value)) res = "himself";
				else res = "itself";
			}


			/* Handle all other visible monster requests */
			else
			{
				/* It could be a Unique */
				if (r_ptr.flags.has(Monster_Flag.UNIQUE.value))
				{
					res = name;
				}

				/* It could be an indefinite monster */
				else if ((mode & 0x08) != 0)
				{
					/* XXX Check plurality for "some" */

					/* Indefinite monsters need an indefinite article */
					res = "aeiouAEIOU".Contains(name[0]) ? "an" : "a";
					res += name;
				}

				/* It could be a normal, definite, monster */
				else
				{
					/* Definite monsters need a definite article */
					res = "the " + name;
				}

				/* Handle the Possessive as a special afterthought */
				if ((mode & 0x02) != 0)
				{
					/* XXX Check for trailing "s" */

					/* Simply append "apostrophe" and "s" */
					res += "'s";
				}

				/* Mention "offscreen" monsters XXX XXX */
				if (!Term.panel_contains(fy, fx))
				{
					/* Append special notation */
					res += " (offscreen)";
				}
			}

			/* Return the result */
			return res;
		}
	}
}
