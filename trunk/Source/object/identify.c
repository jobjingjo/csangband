/*
 * File: identify.c
 * Purpose: Object identification and knowledge routines
 *
 * Copyright (c) 1997 Ben Harrison, James E. Wilson, Robert A. Koeneke
 * Copyright (c) 2009 Brian Bull
 *
 * This work is free software; you can redistribute it and/or modify it
 * under the terms of either:
 *
 * a) the GNU General Public License as published by the Free Software
 *    Foundation, version 2, or
 *
 * b) the "Angband licence":
 *    This software may be copied and distributed for educational, research,
 *    and not for profit purposes provided that this copyright and statement
 *    are included in all such copies.  Other copyrights may also apply.
 */

#include "angband.h"
#include "cave.h"
#include "game-event.h"
#include "history.h"
#include "object/slays.h"
#include "object/tvalsval.h"
#include "object/pval.h"
#include "spells.h"
#include "squelch.h"

/*** Knowledge accessor functions ***/

/**
 * \returns whether the object is known to be an artifact
 */
bool object_is_known_artifact(const object_type *o_ptr)
{
	return (o_ptr.ident & IDENT_INDESTRUCT) ||
			(o_ptr.artifact && object_was_sensed(o_ptr));
}

/**
 * \returns whether the object is known to be cursed
 */
bool object_is_known_cursed(const object_type *o_ptr)
{
	bitflag f[OF_SIZE], f2[OF_SIZE];

	object_flags_known(o_ptr, f);

	/* Gather whatever curse flags there are to know */
	create_mask(f2, false, OFT_CURSE, OFT_MAX);

	return of_is_inter(f, f2);
}

/**
 * \returns whether the object is known to be blessed
 */
bool object_is_known_blessed(const object_type *o_ptr)
{
	bitflag f[OF_SIZE];

	object_flags_known(o_ptr, f);

	return (of_has(f, OF_BLESSED)) ? true : false;
}





/**
 * Mark an object's flavour as tried.
 *
 * \param o_ptr is the object whose flavour should be marked
 */
void object_flavor_tried(object_type *o_ptr)
{
	assert(o_ptr);
	assert(o_ptr.kind);

	o_ptr.kind.tried = true;
}

/**
 * Check whether an object has IDENT_KNOWN but should not
 */
bool object_is_not_known_consistently(const object_type *o_ptr)
{
	if (easy_know(o_ptr))
		return false;
	if (!(o_ptr.ident & IDENT_KNOWN))
		return true;
	if ((o_ptr.ident & IDENTS_SET_BY_IDENTIFY) != IDENTS_SET_BY_IDENTIFY)
		return true;
	if (o_ptr.ident & IDENT_EMPTY)
		return true;
	else if (o_ptr.artifact &&
			!(o_ptr.artifact.seen || o_ptr.artifact.everseen))
		return true;

	if (!of_is_full(o_ptr.known_flags))
		return true;

	return false;
}


/**
 * Notice that an object is indestructible.
 */
void object_notice_indestructible(object_type *o_ptr)
{
	if (object_add_ident_flags(o_ptr, IDENT_INDESTRUCT))
		object_check_for_ident(o_ptr);
}


/*
 * Notice the ego on an ego item.
 */
void object_notice_ego(object_type *o_ptr)
{
	bitflag learned_flags[OF_SIZE];
	bitflag xtra_flags[OF_SIZE];

	if (!o_ptr.ego)
		return;


	/* XXX Eddie print a message on notice ego if not already noticed? */
	/* XXX Eddie should we do something about everseen of egos here? */

	/* Learn ego flags */
	of_union(o_ptr.known_flags, o_ptr.ego.flags);

	/* Learn all flags except random abilities */
	of_setall(learned_flags);

	switch (o_ptr.ego.xtra)
	{
		case OBJECT_XTRA_TYPE_NONE:
			break;
		case OBJECT_XTRA_TYPE_SUSTAIN:
			create_mask(xtra_flags, false, OFT_SUST, OFT_MAX);
			of_diff(learned_flags, xtra_flags);
			break;
		case OBJECT_XTRA_TYPE_RESIST:
			create_mask(xtra_flags, false, OFT_HRES, OFT_MAX);
			of_diff(learned_flags, xtra_flags);
			break;
		case OBJECT_XTRA_TYPE_POWER:
			create_mask(xtra_flags, false, OFT_MISC, OFT_PROT, OFT_MAX);
			of_diff(learned_flags, xtra_flags);
			break;
		default:
			assert(0);
	}

	of_union(o_ptr.known_flags, learned_flags);

	if (object_add_ident_flags(o_ptr, IDENT_NAME))
	{
		/* if you know the ego, you know which it is of excellent or splendid */
		object_notice_sensing(o_ptr);

		object_check_for_ident(o_ptr);
	}
}


/*
 * Sense artifacts
 */
void object_sense_artifact(object_type *o_ptr)
{
	if (o_ptr.artifact)
		object_notice_sensing(o_ptr);
	else
		o_ptr.ident |= IDENT_NOTART;
}


/**
 * Notice the "effect" from activating an object.
 *
 * \param o_ptr is the object to become aware of
 */
void object_notice_effect(object_type *o_ptr)
{
	if (object_add_ident_flags(o_ptr, IDENT_EFFECT))
		object_check_for_ident(o_ptr);

	/* noticing an effect gains awareness */
	if (!object_flavor_is_aware(o_ptr))
		object_flavor_aware(o_ptr);
}


static void object_notice_defence_plusses(struct player *p, object_type *o_ptr)
{
	assert(o_ptr && o_ptr.kind);

	if (object_defence_plusses_are_visible(o_ptr))
		return;

	if (object_add_ident_flags(o_ptr, IDENT_DEFENCE))
		object_check_for_ident(o_ptr);

	if (o_ptr.ac || o_ptr.to_a)
	{
		char o_name[80];

		object_desc(o_name, sizeof(o_name), o_ptr, ODESC_BASE);
		msgt(MSG_PSEUDOID,
				"You know more about the %s you are wearing.",
				o_name);
	}

	p.update |= (PU_BONUS);
	event_signal(EVENT_INVENTORY);
	event_signal(EVENT_EQUIPMENT);
}


/**
 * Notice things which happen on defending.
 */
void object_notice_on_defend(struct player *p)
{
	int i;

	for (i = INVEN_WIELD; i < INVEN_TOTAL; i++)
		if (p.inventory[i].kind)
			object_notice_defence_plusses(p, &p.inventory[i]);

	event_signal(EVENT_INVENTORY);
	event_signal(EVENT_EQUIPMENT);
}


/*
 * Notice stuff when firing or throwing objects.
 *
 */
/* XXX Eddie perhaps some stuff from do_cmd_fire and do_cmd_throw should be moved here */
void object_notice_on_firing(object_type *o_ptr)
{
	if (object_add_ident_flags(o_ptr, IDENT_FIRED))
		object_check_for_ident(o_ptr);
}



/*
 * Determine whether a weapon or missile weapon is obviously {excellent} when
 * worn.
 *
 * XXX Eddie should messages be adhoc all over the place?  perhaps the main
 * loop should check for change in inventory/wieldeds and all messages be
 * printed from one place
 */
void object_notice_on_wield(object_type *o_ptr)
{
	bitflag f[OF_SIZE], f2[OF_SIZE], obvious_mask[OF_SIZE];
	bool obvious = false;

	create_mask(obvious_mask, true, OFID_WIELD, OFT_MAX);

	/* Save time of wield for later */
	object_last_wield = turn;

	/* Only deal with un-ID'd items */
	if (object_is_known(o_ptr)) return;

	/* Wear it */
	object_flavor_tried(o_ptr);
	if (object_add_ident_flags(o_ptr, IDENT_WORN))
		object_check_for_ident(o_ptr);

	/* CC: may wish to be more subtle about this once we have ego lights
	 * with multiple pvals */
	if (obj_is_light(o_ptr) && o_ptr.ego)
		object_notice_ego(o_ptr);

	if (object_flavor_is_aware(o_ptr) && easy_know(o_ptr))
	{
		object_notice_everything(o_ptr);
		return;
	}

	/* Automatically sense artifacts upon wield */
	object_sense_artifact(o_ptr);

	/* Note artifacts when found */
	if (o_ptr.artifact)
		history_add_artifact(o_ptr.artifact, object_is_known(o_ptr), true);

	/* special case FA, needed at least for mages wielding gloves */
	if (object_FA_would_be_obvious(o_ptr))
		of_on(obvious_mask, OF_FREE_ACT);

	/* Extract the flags */
	object_flags(o_ptr, f);

	/* Find obvious things (disregarding curses) - why do we remove the curses?? */
	create_mask(f2, false, OFT_CURSE, OFT_MAX);
	of_diff(obvious_mask, f2);
	if (of_is_inter(f, obvious_mask)) obvious = true;
	create_mask(obvious_mask, true, OFID_WIELD, OFT_MAX);

	/* Notice any obvious brands or slays */
	object_notice_slays(o_ptr, obvious_mask);

	/* Learn about obvious flags */
	of_union(o_ptr.known_flags, obvious_mask);

	/* XXX Eddie should these next NOT call object_check_for_ident due to worries about repairing? */

	/* XXX Eddie this is a small hack, but jewelry with anything noticeable really is obvious */
	/* XXX Eddie learn =soulkeeping vs =bodykeeping when notice sustain_str */
	if (object_is_jewelry(o_ptr))
	{
		/* Learn the flavor of jewelry with obvious flags */
		if (EASY_LEARN && obvious)
			object_flavor_aware(o_ptr);

		/* Learn all flags on any aware non-artifact jewelry */
		if (object_flavor_is_aware(o_ptr) && !o_ptr.artifact)
			object_know_all_flags(o_ptr);
	}

	object_check_for_ident(o_ptr);

	if (!obvious) return;

	/* XXX Eddie need to add stealth here, also need to assert/double-check everything is covered */
	/* CC: also need to add FA! */
	if (of_has(f, OF_STR))
		msg("You feel %s!", o_ptr.pval[which_pval(o_ptr,
			OF_STR)] > 0 ? "stronger" : "weaker");
	if (of_has(f, OF_INT))
		msg("You feel %s!", o_ptr.pval[which_pval(o_ptr,
			OF_INT)] > 0 ? "smarter" : "more stupid");
	if (of_has(f, OF_WIS))
		msg("You feel %s!", o_ptr.pval[which_pval(o_ptr,
			OF_WIS)] > 0 ? "wiser" : "more naive");
	if (of_has(f, OF_DEX))
		msg("You feel %s!", o_ptr.pval[which_pval(o_ptr,
			OF_DEX)] > 0 ? "more dextrous" : "clumsier");
	if (of_has(f, OF_CON))
		msg("You feel %s!", o_ptr.pval[which_pval(o_ptr,
			OF_CON)] > 0 ? "healthier" : "sicklier");
	if (of_has(f, OF_CHR))
		msg("You feel %s!", o_ptr.pval[which_pval(o_ptr,
			OF_CHR)] > 0 ? "cuter" : "uglier");
	if (of_has(f, OF_SPEED))
		msg("You feel strangely %s.", o_ptr.pval[which_pval(o_ptr,
			OF_SPEED)] > 0 ? "quick" : "sluggish");
	if (of_has(f, OF_BLOWS))
		msg("Your weapon %s in your hands.",
			o_ptr.pval[which_pval(o_ptr, OF_BLOWS)] > 0 ?
				"tingles" : "aches");
	if (of_has(f, OF_SHOTS))
		msg("Your bow %s in your hands.",
			o_ptr.pval[which_pval(o_ptr, OF_SHOTS)] > 0 ?
				"tingles" : "aches");
	if (of_has(f, OF_INFRA))
		msg("Your eyes tingle.");
	if (of_has(f, OF_LIGHT))
		msg("It glows!");
	if (of_has(f, OF_TELEPATHY))
		msg("Your mind feels strangely sharper!");

	/* WARNING -- masking f by obvious mask -- this should be at the end of this function */
	/* CC: I think this can safely go, but just in case ... */
/*	flags_mask(f, OF_SIZE, OF_OBVIOUS_MASK, FLAG_END); */

	/* Remember the flags */
	object_notice_sensing(o_ptr);

	/* XXX Eddie should we check_for_ident here? */
}








