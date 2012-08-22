using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Object {
	partial class Object {
		/**
		 * Modes for object_desc().
		 */
		public enum Detail
		{
			BASE   = 0x00,   /*!< Only describe the base name */
			COMBAT = 0x01,   /*!< Also show combat bonuses */
			EXTRA  = 0x02,   /*!< Show charges/inscriptions/pvals */

			FULL   = COMBAT | EXTRA,
								   /*!< Show entire description */

			STORE  = 0x04,   /*!< This is an in-store description */
			PLURAL = 0x08,   /*!< Always pluralise */
			SINGULAR    = 0x10,    /*!< Always singular */
			SPOIL  = 0x20,    /*!< Display regardless of player knowledge */
			PREFIX = 0x40   /* */
		}

		//Not sure what this is? delegate possibly....
		//static size_t obj_desc_name_format(string buf, int max, int end, string fmt, string modstr, bool pluralise);

		/**
		 * Puts the object base kind's name into buf.
		 */
		void object_base_name(string buf, int max, int tval, bool plural)
		{
			throw new NotImplementedException();
			//object_base *kb = &kb_info[tval];
			//size_t end = 0;

			//end = obj_desc_name_format(buf, max, end, kb.name, null, plural);
		}


		/*
		 * Puts a very stripped-down version of an object's name into buf.
		 * If easy_know is true, then the IDed names are used, otherwise
		 * flavours, scroll names, etc will be used.
		 *
		 * Just truncates if the buffer isn't big enough.
		 */
		void object_kind_name(string buf, int max, Object_Kind kind, bool easy_know)
		{
			throw new NotImplementedException();
			///* If not aware, use flavor */
			//if (!easy_know && !kind.aware && kind.flavor)
			//{
			//    if (kind.tval == TV_FOOD && kind.sval > SV_FOOD_MIN_SHROOM)
			//    {
			//        strnfmt(buf, max, "%s Mushroom", kind.flavor.text);
			//    }
			//    else
			//    {
			//        /* Plain flavour (e.g. Copper) will do. */
			//        my_strcpy(buf, kind.flavor.text, max);
			//    }
			//}

			///* Use proper name (Healing, or whatever) */
			//else
			//{
			//    char *t;

			//    if (kind.tval == TV_FOOD && kind.sval > SV_FOOD_MIN_SHROOM)
			//    {
			//        my_strcpy(buf, "Mushroom of ", max);
			//        max -= strlen(buf);
			//        t = buf + strlen(buf);
			//    }
			//    else
			//    {
			//        t = buf;
			//    }

			//    /* Format remainder of the string */
			//    obj_desc_name_format(t, max, 0, kind.name, null, false);
			//}
		}



		static string obj_desc_get_modstr(Object_Kind kind)
		{
			switch (kind.tval)
			{
			    case TVal.TV_AMULET:
			    case TVal.TV_RING:
			    case TVal.TV_STAFF:
			    case TVal.TV_WAND:
			    case TVal.TV_ROD:
			    case TVal.TV_POTION:
			    case TVal.TV_FOOD:
			    case TVal.TV_SCROLL:
			        return kind.flavor != null ? kind.flavor.text : "";

			    case TVal.TV_MAGIC_BOOK:
			    case TVal.TV_PRAYER_BOOK:
			        return kind.Name;
			}

			return "";
		}

		static string obj_desc_get_basename(Object o_ptr, bool aware)
		{
			bool show_flavor = o_ptr.kind.flavor != null ? true : false;


			if ((o_ptr.ident & Object.IDENT_STORE) != 0) show_flavor = false;
			if (aware && !Option.show_flavors.value) show_flavor = false;



			/* Known artifacts get special treatment */
			if (o_ptr.artifact != null && aware)
			    return o_ptr.kind.Name;

			/* Analyze the object */
			switch (o_ptr.tval)
			{
			    case TVal.TV_SKELETON:
			    case TVal.TV_BOTTLE:
			    case TVal.TV_JUNK:
			    case TVal.TV_SPIKE:
			    case TVal.TV_FLASK:
			    case TVal.TV_CHEST:
			    case TVal.TV_SHOT:
			    case TVal.TV_BOLT:
			    case TVal.TV_ARROW:
			    case TVal.TV_BOW:
			    case TVal.TV_HAFTED:
			    case TVal.TV_POLEARM:
			    case TVal.TV_SWORD:
			    case TVal.TV_DIGGING:
			    case TVal.TV_BOOTS:
			    case TVal.TV_GLOVES:
			    case TVal.TV_CLOAK:
			    case TVal.TV_CROWN:
			    case TVal.TV_HELM:
			    case TVal.TV_SHIELD:
			    case TVal.TV_SOFT_ARMOR:
			    case TVal.TV_HARD_ARMOR:
			    case TVal.TV_DRAG_ARMOR:
			    case TVal.TV_LIGHT:
			        return o_ptr.kind.Name;

			    case TVal.TV_AMULET:
			        return (show_flavor ? "& # Amulet~" : "& Amulet~");

			    case TVal.TV_RING:
			        return (show_flavor ? "& # Ring~" : "& Ring~");

			    case TVal.TV_STAFF:
			        return (show_flavor ? "& # Sta|ff|ves|" : "& Sta|ff|ves|");

			    case TVal.TV_WAND:
			        return (show_flavor ? "& # Wand~" : "& Wand~");

			    case TVal.TV_ROD:
			        return (show_flavor ? "& # Rod~" : "& Rod~");

			    case TVal.TV_POTION:
			        return (show_flavor ? "& # Potion~" : "& Potion~");

			    case TVal.TV_SCROLL:
			        return (show_flavor ? "& Scroll~ titled #" : "& Scroll~");

			    case TVal.TV_MAGIC_BOOK:
			        return "& Book~ of Magic Spells #";

			    case TVal.TV_PRAYER_BOOK:
			        return "& Holy Book~ of Prayers #";

			    case TVal.TV_FOOD:
			        if (o_ptr.sval > SVal.SV_FOOD_MIN_SHROOM)
			            return (show_flavor ? "& # Mushroom~" : "& Mushroom~");
			        else
			            return o_ptr.kind.Name;
			}

			return "(nothing)";
		}


		static int obj_desc_name_prefix(ref string buf, int max, int end, Object o_ptr, bool known, string basename, string modstr)
		{
			if(o_ptr.number <= 0)
				buf = buf + "no more ";
			else if(o_ptr.number > 1)
				buf = buf + o_ptr.number.ToString() + " ";
			else if((o_ptr.name_is_visible() || known) && o_ptr.artifact != null)
				buf += "the ";

			else if(basename[0] == '&') {
				bool an = false;
				string lookahead = basename.Substring(1);

				while(lookahead[0] == ' ')
					lookahead = lookahead.Substring(1);

				if(lookahead[0] == '#') {
					if(modstr != null && "aeiouAEIOU".Contains(modstr[0]))
						an = true;
				} else if("aeiouAEIOU".Contains(lookahead[0])) {
					an = true;
				}

				if(an)
					buf = buf + "an ";
				else
					buf = buf + "a ";
			}

			return end;
		}



		/**
		 * Formats 'fmt' into 'buf', with the following formatting characters:
		 *
		 * '~' at the end of a word (e.g. "fridge~") will pluralise
		 *
		 * '|x|y|' will be output as 'x' if singular or 'y' if plural
		 *    (e.g. "kni|fe|ves|")
		 *
		 * '#' will be replaced with 'modstr' (which may contain the pluralising
		 * formats given above).
		 */
		static int obj_desc_name_format(ref string buf, int max, int end, string fmt, string modstr, bool pluralise)
		{
			char prev = '\0';
			/* Copy the string */
			while (fmt.Length > 0)
			{
			    if (fmt[0] == '&')
			    {
					while(fmt[0] == ' ' || fmt[0] == '&') {
						prev = fmt[0]; //we need to store it to remember that it isn't valid, don't remove it! :P
						fmt = fmt.Substring(1);
					}
			        continue;
			    }

			    /* Pluralizer (regular English plurals) */
			    else if (fmt[0] == '~')
			    {
			        //char prev = *(fmt - 1); //Thanks to this, I had to put "prev" assignments everywhere...

			        if (!pluralise)
			        {
						prev = fmt[0];
						fmt = fmt.Substring(1);
			            continue;
			        }

			        /* e.g. cutlass-e-s, torch-e-s, box-e-s */
					if(prev == 's' || prev == 'h' || prev == 'x')
						buf = buf + "es";
					else
						buf = buf + "s";
			    }

			    /* Special plurals */
			    else if (fmt[0] == '|')
			    {
					//TODO: if names are fucking wonky, breakpoint here and inspect the three strings below throughout this code block

			        /* e.g. kni|fe|ves|
			         *          ^  ^  ^ */
			        string singular = fmt.Substring(1);
			        string plural   = fmt.Substring(fmt.IndexOf('|'));
			        string endmark  = null;

			        if (plural.Length > 0)
			        {
						plural = plural.Substring(1);
			            endmark = plural.Substring(plural.IndexOf('|'));
			        }

					//Not sure what this is optimizing...
			        if (singular.Length == 0 || plural.Length == 0 || endmark.Length == 0) return end;

					//fuck it, I'm guessing on this next part. Originals are commented out.
					//For %.*s look at http://www.cplusplus.com/reference/clibrary/cstdio/printf/
					if(!pluralise)
						buf = buf + singular;
						//strnfcat(buf, max, &end, "%.*s", plural - singular - 1, singular);
					else
						buf = buf + plural; 
						//strnfcat(buf, max, &end, "%.*s", endmark - plural, plural);
					
					prev = fmt[0]; //screw it, close enough. This could cause problems though...
					fmt = endmark;
			    }

			    /* Add modstr, with pluralisation if relevant */
			    else if (fmt[0] == '#')
			    {
			        end = obj_desc_name_format(ref buf, max, end, modstr, null, pluralise);
			    }

			    else
					buf = buf + fmt[0];
			        //buf[end++] = *fmt;

				prev = fmt[0];
			    fmt = fmt.Substring(1);
			}

			//buf[end] = 0;

			return end;
		}


		/*
		 * Format object o_ptr's name into 'buf'.
		 */
		static int obj_desc_name(ref string buf, int max, int end, Object o_ptr, bool prefix, Detail mode, bool spoil)
		{
			bool known = o_ptr.is_known() || ((o_ptr.ident & Object.IDENT_STORE) != 0) || spoil;
			bool aware = o_ptr.flavor_is_aware() || ((o_ptr.ident & Object.IDENT_STORE) != 0) || spoil;

			string basename = obj_desc_get_basename(o_ptr, aware);
			string modstr = obj_desc_get_modstr(o_ptr.kind);

			if (aware && !o_ptr.kind.everseen && !spoil)
			    o_ptr.kind.everseen = true;

			if (prefix)
			    end = obj_desc_name_prefix(ref buf, max, end, o_ptr, known,
			            basename, modstr);

			/* Pluralize if (not forced singular) and
			 * (not a known/visible artifact) and
			 * (not one in stack or forced plural) */
			end = obj_desc_name_format(ref buf, max, end, basename, modstr,
			        ((mode & Detail.SINGULAR) == 0) &&
			        !(o_ptr.artifact == null && (o_ptr.name_is_visible() || known)) &&
			        (o_ptr.number != 1 || ((mode & Detail.PLURAL) != 0)));


			/** Append extra names of various kinds **/

			if((o_ptr.name_is_visible() || known) && o_ptr.artifact != null)
				buf = buf + " " + o_ptr.artifact.Name;

			else if((spoil && o_ptr.ego != null) || o_ptr.ego_is_visible())
				buf = buf + " " + o_ptr.ego.name;

			else if(aware && o_ptr.artifact == null && (o_ptr.kind.flavor != null || o_ptr.kind.tval == TVal.TV_SCROLL))
				buf = buf + " of " + o_ptr.kind.Name;

			return end;
		}

		/*
		 * Is o_ptr armor?
		 */
		static bool obj_desc_show_armor(Object o_ptr)
		{
			if (o_ptr.ac != 0) return true;

			switch (o_ptr.tval)
			{
			    case TVal.TV_BOOTS:
			    case TVal.TV_GLOVES:
			    case TVal.TV_CLOAK:
			    case TVal.TV_CROWN:
			    case TVal.TV_HELM:
			    case TVal.TV_SHIELD:
			    case TVal.TV_SOFT_ARMOR:
			    case TVal.TV_HARD_ARMOR:
			    case TVal.TV_DRAG_ARMOR:
			    {
			        return true;
			    }
			}

			return false;
		}

		static int obj_desc_chest(Object o_ptr, ref string buf, int max, int end)
		{
			throw new NotImplementedException();
			//bool known = object_is_known(o_ptr) || (o_ptr.ident & IDENT_STORE);

			//if (o_ptr.tval != TV_CHEST) return end;
			//if (!known) return end;

			///* May be "empty" */
			//if (!o_ptr.pval[DEFAULT_PVAL])
			//    strnfcat(buf, max, &end, " (empty)");

			///* May be "disarmed" */
			//else if (o_ptr.pval[DEFAULT_PVAL] < 0)
			//{
			//    if (chest_traps[0 - o_ptr.pval[DEFAULT_PVAL]])
			//        strnfcat(buf, max, &end, " (disarmed)");
			//    else
			//        strnfcat(buf, max, &end, " (unlocked)");
			//}

			///* Describe the traps, if any */
			//else
			//{
			//    /* Describe the traps */
			//    switch (chest_traps[o_ptr.pval[DEFAULT_PVAL]])
			//    {
			//        case 0:
			//            strnfcat(buf, max, &end, " (Locked)");
			//            break;

			//        case CHEST_LOSE_STR:
			//            strnfcat(buf, max, &end, " (Poison Needle)");
			//            break;

			//        case CHEST_LOSE_CON:
			//            strnfcat(buf, max, &end, " (Poison Needle)");
			//            break;

			//        case CHEST_POISON:
			//            strnfcat(buf, max, &end, " (Gas Trap)");
			//            break;

			//        case CHEST_PARALYZE:
			//            strnfcat(buf, max, &end, " (Gas Trap)");
			//            break;

			//        case CHEST_EXPLODE:
			//            strnfcat(buf, max, &end, " (Explosion Device)");
			//            break;

			//        case CHEST_SUMMON:
			//            strnfcat(buf, max, &end, " (Summoning Runes)");
			//            break;

			//        default:
			//            strnfcat(buf, max, &end, " (Multiple Traps)");
			//            break;
			//    }
			//}

			//return end;
		}

		static int obj_desc_combat(Object o_ptr, ref string buf, int max, int end, bool spoil)
		{
			Bitflag flags = new Bitflag(Object_Flag.SIZE);
			Bitflag flags_known = new Bitflag(Object_Flag.SIZE);

			o_ptr.object_flags(ref flags);
			o_ptr.object_flags_known(ref flags_known);

			if (flags.has(Object_Flag.SHOW_DICE.value))
			{
			    /* Only display the real damage dice if the combat stats are known */
				if(spoil || o_ptr.attack_plusses_are_visible())
					buf = buf + " (" + o_ptr.dd + "d" + o_ptr.ds + ")";
				else
					buf = buf + " (" + o_ptr.kind.dd + "d" + o_ptr.kind.ds + ")";
			}

			if (flags.has(Object_Flag.SHOW_MULT.value))
			{
			    /* Display shooting power as part of the multiplier */
			    if (flags.has(Object_Flag.MIGHT.value) && (spoil || o_ptr.object_flag_is_known(Object_Flag.MIGHT.value)))
					buf = buf + " (x" + (o_ptr.sval % 10) + o_ptr.pval[o_ptr.which_pval(Object_Flag.MIGHT.value)] + ")";
			    else
					buf = buf + " (x" + o_ptr.sval % 10 + ")";
			}

			/* Show weapon bonuses */
			if (spoil || o_ptr.attack_plusses_are_visible())
			{
			    if (flags.has(Object_Flag.SHOW_MODS.value) || o_ptr.to_d != 0 || o_ptr.to_h != 0)
			    {
			        /* Make an exception for body armor with only a to-hit penalty */
					if(o_ptr.to_h < 0 && o_ptr.to_d == 0 &&
						(o_ptr.tval == TVal.TV_SOFT_ARMOR ||
						 o_ptr.tval == TVal.TV_HARD_ARMOR ||
						 o_ptr.tval == TVal.TV_DRAG_ARMOR))
						buf = buf + " (" + (o_ptr.to_h > 0 ? "+" : "-") + o_ptr.to_h + ")";

					/* Otherwise, always use the full tuple */
					else
						buf = buf + " (" + (o_ptr.to_h > 0 ? "+" : "-") + o_ptr.to_h + "," +
							(o_ptr.to_d > 0 ? "+" : "-") + o_ptr.to_d + ")";
			    }
			}


			/* Show armor bonuses */
			if (spoil || o_ptr.defence_plusses_are_visible())
			{
			    if (obj_desc_show_armor(o_ptr))
					buf = buf + " [" + o_ptr.ac + "," + (o_ptr.to_a > 0?"+":"-") + o_ptr.to_a + "]";
			    else if (o_ptr.to_a != 0)
					buf = buf + " [" + (o_ptr.to_a > 0?"+":"-") + o_ptr.to_a + "]";
			}
			else if (obj_desc_show_armor(o_ptr))
			{
				buf = buf + " [" + (o_ptr.was_sensed() ? o_ptr.ac : o_ptr.kind.ac) + "]";
			}

			return end;
		}

		static int obj_desc_light(Object o_ptr, ref string buf, int max, int end)
		{
			Bitflag f = new Bitflag(Object_Flag.SIZE);

			o_ptr.object_flags(ref f);

			/* Fuelled light sources get number of remaining turns appended */
			if ((o_ptr.tval == TVal.TV_LIGHT) && !f.has(Object_Flag.NO_FUEL.value))
			    buf += String.Format(" ({0} turns)", o_ptr.timeout);

			return buf.Length;
		}

		static int obj_desc_pval(Object o_ptr, string buf, int max, int end, bool spoil)
		{
			throw new NotImplementedException();
			//bitflag f[OF_SIZE], f2[OF_SIZE];
			//int i;

			//object_flags(o_ptr, f);
			//create_mask(f2, false, OFT_PVAL, OFT_STAT, OFT_MAX);

			//if (!of_is_inter(f, f2)) return end;

			//strnfcat(buf, max, &end, " <");
			//for (i = 0; i < o_ptr.num_pvals; i++) {
			//    if (spoil || object_this_pval_is_visible(o_ptr, i)) {
			//        if (i > 0)
			//            strnfcat(buf, max, &end, ", ");
			//        strnfcat(buf, max, &end, "%+d", o_ptr.pval[i]);
			//    }
			//}

			//strnfcat(buf, max, &end, ">");

			//return end;
		}

		static int obj_desc_charges(Object o_ptr, ref string buf, int max, int end)
		{
			bool aware = o_ptr.flavor_is_aware() || ((o_ptr.ident & Object.IDENT_STORE) != 0);

			/* Wands and Staffs have charges */
			if (aware && (o_ptr.tval == TVal.TV_STAFF || o_ptr.tval == TVal.TV_WAND))
				buf = buf + " (" + o_ptr.pval[Misc.DEFAULT_PVAL] + " charge" + (o_ptr.pval[Misc.DEFAULT_PVAL] > 1 ? "s":"") + ")";

			/* Charging things */
			else if (o_ptr.timeout > 0)
			{
			    if (o_ptr.tval == TVal.TV_ROD && o_ptr.number > 1)
			    {
			        int power;
			        int time_base = Random.randcalc(o_ptr.kind.time, 0, aspect.MINIMISE);

			        if (time_base == 0) time_base = 1;

			        /*
			         * Find out how many rods are charging, by dividing
			         * current timeout by each rod's maximum timeout.
			         * Ensure that any remainder is rounded up.  Display
			         * very discharged stacks as merely fully discharged.
			         */
			        power = (o_ptr.timeout + (time_base - 1)) / time_base;
			        if (power > o_ptr.number) power = o_ptr.number;

			        /* Display prettily */
					buf = " (" + power + " charging)";
			    }

			    /* Artifacts, single rods */
			    else if (!(o_ptr.tval == TVal.TV_LIGHT && o_ptr.artifact == null))
			    {
					buf = buf + " (charging)";
			    }
			}

			return end;
		}

		static int obj_desc_inscrip(Object o_ptr, ref string buf, int max, int end)
		{
			string[] u = {"", "", "", ""};
			int n = 0;
			Object.obj_pseudo_t feel = o_ptr.pseudo();
			Bitflag flags_known = new Bitflag(Object_Flag.SIZE);
			Bitflag f2 = new Bitflag(Object_Flag.SIZE);

			o_ptr.object_flags_known(ref flags_known);

			/* Get inscription */
			if (o_ptr.note != null && o_ptr.note.value != null)
			    u[n++] = o_ptr.note.ToString();

			/* Use special inscription, if any */
			if (!o_ptr.is_known() && feel != 0)
			{
			    /* cannot tell excellent vs strange vs splendid until wield */
			    if (!o_ptr.was_worn() && o_ptr.ego != null)
			        u[n++] = "ego";
			    else
			        u[n++] = Misc.inscrip_text[(int)feel]; //I know that feel bro.
			}
			else if (((o_ptr.ident & Object.IDENT_EMPTY) != 0) && !o_ptr.is_known())
			    u[n++] = "empty";
			else if (!o_ptr.is_known() && o_ptr.was_worn())
			{
			    if (o_ptr.wield_slot() == Misc.INVEN_WIELD || o_ptr.wield_slot() == Misc.INVEN_BOW)
			        u[n++] = "wielded";
			    else u[n++] = "worn";
			}
			else if (!o_ptr.is_known() && o_ptr.was_fired())
			    u[n++] = "fired";
			else if (!o_ptr.flavor_is_aware() && o_ptr.flavor_was_tried())
			    u[n++] = "tried";

			/* Note curses */
			Object_Flag.create_mask(f2, false, Object_Flag.object_flag_type.CURSE);
			if (flags_known.is_inter(f2))
			    u[n++] = "cursed";

			/* Note squelch */
			if (Squelch.item_ok(o_ptr))
			    u[n++] = "squelch";

			if (n != 0)
			{
			    int i;
			    for (i = 0; i < n; i++)
			    {
					if(i == 0)
						buf = buf + " {";
					buf = buf + u[i];
			        if (i < n-1)
						buf += ", ";
			    }

				buf += "}";
			}

			return end;
		}


		/* Add "unseen" to the end of unaware items in stores */
		static int obj_desc_aware(Object o_ptr, ref string buf, int max, int end)
		{
			if(!o_ptr.flavor_is_aware())
				buf += " {unseen}";

			return end;
		}


		/**
		 * Describes item `o_ptr` into buffer `buf` of size `max`.
		 *
		 * ODESC_PREFIX prepends a 'the', 'a' or number
		 * ODESC_BASE results in a base description.
		 * ODESC_COMBAT will add to-hit, to-dam and AC info.
		 * ODESC_EXTRA will add pval/charge/inscription/squelch info.
		 * ODESC_PLURAL will pluralise regardless of the number in the stack.
		 * ODESC_STORE turns off squelch markers, for in-store display.
		 * ODESC_SPOIL treats the object as fully identified.
		 *
		 * Setting 'prefix' to true prepends a 'the', 'a' or the number in the stack,
		 * respectively.
		 *
		 * \returns The number of bytes used of the buffer.
		 */
		public string object_desc(Detail mode)
		{
			bool prefix = (int)(mode & Detail.PREFIX) != 0;
			bool spoil = (int)(mode & Detail.SPOIL) != 0;

			int end = 0, i = 0;

			/* Simple description for null item */
			if(tval == 0) {
				return "(nothing)";
			}

			bool known = is_known() || (ident & Object.IDENT_STORE) != 0 || spoil;

			/* We've seen it at least once now we're aware of it */
			if (known && ego != null && !spoil) ego.everseen = true;


			/*** Some things get really simple descriptions ***/

			if(tval == TVal.TV_GOLD) {
				return pval[Misc.DEFAULT_PVAL] + " gold pieces worth of " + kind.Name +  (Squelch.item_ok(this) ? " {squelch}" : "");
			}

			/** Construct the name **/

			/* Copy the base name to the buffer */
			string buf = "";
			end = obj_desc_name(ref buf, 80, end, this, prefix, mode, spoil);

			if ((mode & Detail.COMBAT) != 0)
			{
			    if (tval == TVal.TV_CHEST)
			        end = obj_desc_chest(this, ref buf, 80, end);
			    else if (tval == TVal.TV_LIGHT)
			        end = obj_desc_light(this, ref buf, 80, end);

			    end = obj_desc_combat(this, ref buf, 80, end, spoil);
			}

			if ((mode & Detail.EXTRA) != 0)
			{
			    for (i = 0; i < num_pvals; i++)
			        if (spoil || this_pval_is_visible(i)) {
			            end = obj_desc_pval(this, buf, 80, end, spoil);
			            break;
			        }

			    end = obj_desc_charges(this, ref buf, 80, end);

			    if ((mode & Detail.STORE) != 0)
			    {
			        end = obj_desc_aware(this, ref buf, 80, end);
			    }
			    else
			        end = obj_desc_inscrip(this, ref buf, 80, end);
			}

			return buf;
		}
	}
}
