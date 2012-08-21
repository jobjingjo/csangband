using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;
using CSAngband.Monster;

namespace CSAngband {
	/* List of store indices */
	public enum STORE {
		NONE = -1,
		GENERAL = 0,
		ARMOR = 1,
		WEAPON = 2,
		TEMPLE = 3,
		ALCHEMY = 4,
		MAGIC = 5,
		B_MARKET = 6,
		HOME = 7,
		MAX_STORES = 8
	};

	class Owner {
		public uint oidx;
		public Owner next;
		public string name;
		public int max_cost;
	};

	class Store {
		public Store() {
		}

		public Store(STORE idx) {
			sidx = idx;
			stock = new Object.Object[INVEN_MAX];
			stock_size = (short)INVEN_MAX;
		}
		public Store next;
		public Owner owners;
		public Owner owner;
		public STORE sidx;

		public byte stock_num;			/* Stock -- Number of entries */
		public short stock_size;		/* Stock -- Total Size of Array */
		public Object.Object[] stock;		/* Stock -- Actual stock items */

		public uint table_num;     /* Table -- Number of entries */
		public uint table_size;    /* Table -- Total Size of Array */
		public Object_Kind[] table;        /* Table -- Legal item kinds */



		public static int INVEN_MAX = 24;    /* Max number of discrete objs in inven */
		public static int TURNS = 1000; /* Number of turns between turnovers */
		public static int SHUFFLE = 25;    /* 1/Chance (per day) of an owner changing */
		public static int MIN_KEEP = 6;       /* Min slots to "always" keep full (>0) */
		public static int MAX_KEEP = 18;      /* Max slots to "always" keep full (<STORE_INVEN_MAX) */

		/* Some local constants */
		const int STORE_TURNOVER = 9;       /* Normal shop turnover, per day */
		const int STORE_OBJ_LEVEL= 5;       /* Magic Level for normal stores */

		public static void Init() {
			Store store_list;

			store_list = parse_stores();
			store_list = add_builtin_stores(store_list);
			parse_owners(store_list);
			Misc.stores = flatten_stores(store_list);
		}

		static Store parse_stores() {
			Parser p = store_parser_new();
			Store stores;
			/* XXX ignored */
			p.parse_file("store");
			stores = p.priv as Store;
			p.Destroy();
			return stores;
		}

		public static Parser store_parser_new() {
			Parser p = new Parser();
			p.priv = null;
			p.Register("S uint index uint slots", parse_s);
			p.Register("I uint slots sym tval sym sval", parse_i);
			return p;
		}

		static Parser.Error parse_s(Parser p) {
			Store h = p.priv as Store;
			Store s;
			uint idx = p.getuint("index") - 1;
			uint slots = p.getuint("slots");

			if(idx < (uint)STORE.ARMOR || idx > (uint)STORE.MAGIC)
				return Parser.Error.OUT_OF_BOUNDS;

			s = new Store((STORE)(p.getuint("index") - 1));
			s.table = new Object_Kind[slots];
			s.table_size = slots;
			s.next = h;
			p.priv = s;
			return Parser.Error.NONE;
		}

		static Parser.Error parse_i(Parser p) {
			Store s = p.priv as Store;
			uint slots = p.getuint("slots");
			int tval = TVal.find_idx(p.getsym("tval"));
			int sval = SVal.lookup_sval(tval, p.getsym("sval"));
			Object_Kind kind = Object_Kind.lookup_kind(tval, sval);

			if(kind == null)
				return Parser.Error.UNRECOGNISED_SVAL;

			if(s.table_num + slots > s.table_size)
				return Parser.Error.TOO_MANY_ENTRIES;
			while(slots-- != 0) {
				s.table[s.table_num++] = kind;
			}
			/* XXX: get rid of this table_size/table_num/indexing thing. It's
			 * stupid. Dynamically allocate. */
			return Parser.Error.NONE;
		}

		static Store add_builtin_stores(Store stores) {
			Store s0, s1, s2;

			s0 = new Store(STORE.GENERAL);
			s1 = new Store(STORE.B_MARKET);
			s2 = new Store(STORE.HOME);

			s0.next = stores;
			s1.next = s0;
			s2.next = s1;

			return s2;
		}

		static void parse_owners(Store stores) {
			Parser p = store_owner_parser_new(stores);
			p.parse_file("shop_own");
			p.Destroy();
		}

		static Store[] flatten_stores(Store store_list) {
			Store s;
			Store[] stores = new Store[(int)STORE.MAX_STORES];

			for(s = store_list; s != null; s = s.next) {
				/* XXX bounds-check */
				stores[(int)s.sidx] = s;
			}

			return stores;
		}

		class owner_parser_state {
			public Store stores;
			public Store cur;
		};

		static Parser.Error parse_own_n(Parser p) {
			owner_parser_state s = p.priv as owner_parser_state;
			uint index = p.getuint("index");
			Store st;

			for (st = s.stores; st != null ; st = st.next) {
				if (st.sidx == (STORE)index) {
					s.cur = st;
				}
			}

			return st != null ? Parser.Error.NONE : Parser.Error.OUT_OF_BOUNDS;
		}

		static Parser.Error parse_own_s(Parser p) {
			owner_parser_state s = p.priv as owner_parser_state;
			uint maxcost = p.getuint("maxcost");
			string name = p.getstr("name");
			Owner o;

			if (s.cur == null)
				return Parser.Error.MISSING_RECORD_HEADER;

			o = new Owner();
			o.oidx = (s.cur.owners != null ? s.cur.owners.oidx + 1 : 0);
			o.next = s.cur.owners;
			o.name = name;
			o.max_cost = (int)maxcost;
			s.cur.owners = o;
			return Parser.Error.NONE;
		}

		static Parser store_owner_parser_new(Store stores) {
			Parser p = new Parser();
			owner_parser_state s = new owner_parser_state();
			s.stores = stores;
			s.cur = null;
			p.priv = s;
			p.Register("V sym version", Parser.Ignored);
			p.Register("N uint index", parse_own_n);
			p.Register("S uint maxcost str name", parse_own_s);
			return p;
		}

		public static void reset() {
			Store s;

			for (int i = 0; i < (int)STORE.MAX_STORES; i++) {
			    s = Misc.stores[i];
			    s.stock_num = 0;
			    s.store_shuffle();
				for(int j = 0; j < s.stock_size; j++)
					s.stock[j] = new Object.Object();
			        //object_wipe(&s.stock[j]);
			    if (i == (int)STORE.HOME)
			        continue;
			    for (int j = 0; j < 10; j++)
			        s.store_maint();
			}
		}

		/*
		 * Shuffle one of the stores.
		 */
		void store_shuffle()
		{
			Owner o = owner;

			while (o == owner)
				o = choose_owner();

			owner = o;
		}

		Owner choose_owner() {
			Owner o;
			uint n = 0;

			for (o = owners; o != null; o = o.next) {
				n++;
			}

			n = (uint)Random.randint0((int)n);
			return store_ownerbyidx(n);
		}

		Owner store_ownerbyidx(uint idx) {
			Owner o;
			for (o = owners; o != null; o = o.next) {
				if (o.oidx == idx)
					return o;
			}

			throw new Exception("YOU SHOULD HAVE NOT REACHED THIS POINT!!!");
		}

		/*
		 * Maintain the inventory at the stores.
		 */
		void store_maint()
		{
			int j;
			uint stock;
			int restock_attempts = 100000;

			/* Ignore home */
			if (sidx == STORE.HOME)
				return;

			/* General Store gets special treatment */
			if (sidx == STORE.GENERAL) {
				/* Sell off 30% of the inventory */
				prune(300);
				create_staples();
				return;
			}

			/* Prune the black market */
			if (sidx == STORE.B_MARKET)
			{
				/* Destroy crappy black market items */
				for (j = stock_num - 1; j >= 0; j--)
				{
					Object.Object o_ptr = this.stock[j];

					/* Destroy crappy items */
					if (!black_market_ok(o_ptr))
					{
						/* Destroy the object */
						item_increase(j, 0 - o_ptr.number);
						item_optimize(j);
					}
				}
			}

			/*** "Sell" various items */

			/* Sell a few items */
			stock = stock_num;
			stock -= (uint)Random.randint1((int)STORE_TURNOVER);

			/* Keep stock between specified min and max slots */
			if (stock > MAX_KEEP) stock = (uint)MAX_KEEP;
			if (stock < MIN_KEEP) stock = (uint)MIN_KEEP;

			/* Destroy objects until only "j" slots are left */
			while (stock_num > stock) delete_random();


			/*** "Buy in" various items */

			/* Buy a few items */
			stock = stock_num;
			stock += (uint)Random.randint1((int)STORE_TURNOVER);

			/* Keep stock between specified min and max slots */
			if (stock > MAX_KEEP) stock = (uint)MAX_KEEP;
			if (stock < MIN_KEEP) stock = (uint)MIN_KEEP;

			/* For the rest, we just choose items randomlyish */
			/* The (huge) restock_attempts will only go to zero (otherwise
			 * infinite loop) if stores don't have enough items they can stock! */
			while (stock_num < stock && (--restock_attempts) != 0)
				create_random();

			if (restock_attempts == 0)
				Utilities.quit("Unable to (re-)stock store " + sidx + ". Please report this bug");
		}

		/*
		 * This makes sure that the black market doesn't stock any object that other
		 * stores have, unless it is an ego-item or has various bonuses.
		 *
		 * Based on a suggestion by Lee Vogt <lvogt@cig.mcel.mot.com>.
		 */
		static bool black_market_ok(Object.Object o_ptr)
		{
			/* Ego items are always fine */
			if (o_ptr.ego != null) return (true);

			/* Good items are normally fine */
			if (o_ptr.to_a > 2) return (true);
			if (o_ptr.to_h > 1) return (true);
			if (o_ptr.to_d > 2) return (true);


			/* No cheap items */
			if (o_ptr.value(1, false) < 10) return (false);

			/* Check the other stores */
			for (int i = 0; i < (int)STORE.MAX_STORES; i++)
			{
				/* Skip home and black market */
				if (i == (int)STORE.B_MARKET || i == (int)STORE.HOME)
					continue;

				/* Check every object in the store */
				for (int j = 0; j < Misc.stores[i].stock_num; j++)
				{
					Object.Object j_ptr = Misc.stores[i].stock[j];

					/* Compare object kinds */
					if (o_ptr.kind == j_ptr.kind)
						return (false);
				}
			}

			/* Otherwise fine */
			return (true);
		}

		/*
		 * Delete a percentage of a store's inventory
		 */
		void prune(int chance_in_1000)
		{
			int i;

			for (i = 0; i < stock_num; i++) {
				if (Random.randint0(1000) < chance_in_1000)
					delete_index(i);
			}
		}

		/*
		 * Staple definitions.
		 */
		enum create_mode { MAKE_SINGLE, MAKE_NORMAL, MAKE_MAX };

		struct staple_type
		{
			public staple_type(int a, int b, create_mode c){
				tval = a;
				sval = b;
				mode = c;
			}
			public int tval, sval;
			public create_mode mode;
		} 
		
		static staple_type[] staples = new staple_type[]
		{
			new staple_type( TVal.TV_FOOD, (int)SVal.sval_food.SV_FOOD_RATION, create_mode.MAKE_NORMAL ),
			new staple_type( TVal.TV_LIGHT, (int)SVal.SV_LIGHT_TORCH, create_mode.MAKE_NORMAL ),
			new staple_type( TVal.TV_SCROLL, (int)SVal.SV_SCROLL_WORD_OF_RECALL, create_mode.MAKE_NORMAL ),
			new staple_type( TVal.TV_SCROLL, (int)SVal.SV_SCROLL_PHASE_DOOR, create_mode.MAKE_NORMAL ),
			new staple_type( TVal.TV_FLASK, 0, create_mode.MAKE_NORMAL ),
			new staple_type( TVal.TV_SPIKE, 0, create_mode.MAKE_NORMAL ),
			new staple_type( TVal.TV_SHOT, (int)SVal.SV_AMMO_NORMAL, create_mode.MAKE_MAX ),
			new staple_type( TVal.TV_ARROW, (int)SVal.SV_AMMO_NORMAL, create_mode.MAKE_MAX ),
			new staple_type( TVal.TV_BOLT, (int)SVal.SV_AMMO_NORMAL, create_mode.MAKE_MAX ),
			new staple_type( TVal.TV_DIGGING, (int)SVal.sval_digging.SV_SHOVEL, create_mode.MAKE_SINGLE ),
			new staple_type( TVal.TV_DIGGING, (int)SVal.sval_digging.SV_PICK, create_mode.MAKE_SINGLE ),
			new staple_type( TVal.TV_CLOAK, (int)SVal.sval_cloak.SV_CLOAK, create_mode.MAKE_SINGLE )
		};

		/*
		 * Create all staple items.
		 */
		static void create_staples()
		{
			Store store = Misc.stores[(int)STORE.GENERAL];

			//Nick: I wrote this, it doesn't solve the underlying problem...
			///* Double check the stock numbers */
			//store.stock_num = 0;
			//for(int i = 0; i < INVEN_MAX; i++) {
			//    if(store.stock[i].number != 0)
			//        store.stock_num++;
			//}

			/* Make sure there's enough room for staples */
			while (store.stock_num >= INVEN_MAX - staples.Length)
				store.delete_random();

			/* Iterate through staples */
			for (int i = 0; i < staples.Length; i++)
			{
				staple_type staple = staples[i];

				/* Create the staple and combine it into the store inventory */
				int idx = store.create_item(Object_Kind.lookup_kind(staple.tval, staple.sval));
				Object.Object o_ptr = store.stock[idx];

				Misc.assert(o_ptr != null);

				/* Tweak the quantities */
				switch (staple.mode)
				{
					case create_mode.MAKE_SINGLE:
						o_ptr.number = 1;
						break;

					case create_mode.MAKE_NORMAL:
						mass_produce(o_ptr);
						break;

					case create_mode.MAKE_MAX:
						o_ptr.number = 99;
						break;
				}
			}
		}

		/*
		 * Increase, by a 'num', the number of an item 'item' in store 'st'.
		 * This can result in zero items.
		 */
		void item_increase(int item, int num)
		{
			int cnt;
			Object.Object o_ptr;

			/* Get the object */
			o_ptr = stock[item];

			/* Verify the number */
			cnt = o_ptr.number + num;
			if (cnt > 255) cnt = 255;
			else if (cnt < 0) cnt = 0;

			/* Save the new number */
			o_ptr.number = (byte)cnt;
		}

		/*
		 * Remove a slot if it is empty, in store 'st'.
		 */
		void item_optimize(int item)
		{
			int j;
			Object.Object o_ptr;

			/* Get the object */
			o_ptr = stock[item];

			/* Must exist */
			if (o_ptr.kind == null) return;

			/* Must have no items */
			if (o_ptr.number != 0) return;

			/* One less object */
			stock_num--;

			/* Slide everyone */
			for (j = item; j < stock_num; j++)
			{
				stock[j] = stock[j + 1];
			}

			/* Nuke the final slot */
			stock[j] = new Object.Object();
			//object_wipe(&store.stock[j]);
		}


		/*
		 * Delete a random object from store 'st', or, if it is a stack, perhaps only
		 * partially delete it.
		 *
		 * This function is used when store maintainance occurs, and is designed to
		 * imitate non-PC purchasers making purchases from the store.
		 */
		void delete_random()
		{
			int what;

			if (stock_num <= 0) return;

			/* Pick a random slot */
			what = (int)Random.randint0(stock_num);

			delete_index(what);
		}

		/*
		 * Creates a random object and gives it to store 'st'
		 */
		bool create_random()
		{
			int tries, level;

			Object.Object i_ptr;
			//Object.Object object_type_body;

			int min_level, max_level;

			/* Decide min/max levels */
			if (sidx == STORE.B_MARKET) {
				min_level = Misc.p_ptr.max_depth + 5;
				max_level = Misc.p_ptr.max_depth + 20;
			} else {
				min_level = 1;
				max_level = STORE_OBJ_LEVEL + Math.Max(Misc.p_ptr.max_depth - 20, 0);
			}

			if (min_level > 55) min_level = 55;
			if (max_level > 70) max_level = 70;

			/* Consider up to six items */
			for (tries = 0; tries < 6; tries++)
			{
				Object_Kind kind;

				/* Work out the level for objects to be generated at */
				level = Random.rand_range(min_level, max_level);


				/* Black Markets have a random object, of a given level */
				if (sidx == STORE.B_MARKET)
					kind = Object.Object.get_obj_num(level, false);
				else
					kind = get_choice();


				/*** Pre-generation filters ***/

				/* No chests in stores XXX */
				if (kind.tval == TVal.TV_CHEST) continue;


				/*** Generate the item ***/

				/* Get local object */
				i_ptr = new Object.Object();
				//i_ptr = object_type_body;

				/* Create a new object of the chosen kind */
				i_ptr.prep(kind, level, aspect.RANDOMISE);

				/* Apply some "low-level" magic (no artifacts) */
				i_ptr.apply_magic(level, false, false, false);

				/* Reject if item is 'damaged' (i.e. negative mods) */
				switch (i_ptr.tval)
				{
					case TVal.TV_DIGGING:
					case TVal.TV_HAFTED:
					case TVal.TV_POLEARM:
					case TVal.TV_SWORD:
					case TVal.TV_BOW:
					case TVal.TV_SHOT:
					case TVal.TV_ARROW:
					case TVal.TV_BOLT:
					{
						if ((i_ptr.to_h < 0) || (i_ptr.to_d < 0)) 
							continue;
						if (i_ptr.to_a < 0) continue;
						break;
					}

					case TVal.TV_DRAG_ARMOR:
					case TVal.TV_HARD_ARMOR:
					case TVal.TV_SOFT_ARMOR:
					case TVal.TV_SHIELD:
					case TVal.TV_HELM:
					case TVal.TV_CROWN:
					case TVal.TV_CLOAK:
					case TVal.TV_GLOVES:
					case TVal.TV_BOOTS:
					{
						if (i_ptr.to_a < 0) continue;
						break;
					}
				}

				/* The object is "known" and belongs to a store */
				i_ptr.ident |= Object.Object.IDENT_STORE;
				i_ptr.origin = Origin.STORE;


				/*** Post-generation filters ***/

				/* Black markets have expensive tastes */
				if ((sidx == STORE.B_MARKET) && !black_market_ok(i_ptr))
					continue;

				/* No "worthless" items */
				if (i_ptr.value(1, false) < 1) continue;

				/* Mass produce and/or apply discount */
				mass_produce(i_ptr);

				/* Attempt to carry the object */
				carry(i_ptr);

				/* Definitely done */
				return true;
			}

			return false;
		}

		/*
		 * Delete an object from store 'st', or, if it is a stack, perhaps only
		 * partially delete it.
		 */
		void delete_index(int what)
		{
			int num;
			Object.Object o_ptr;

			/* Paranoia */
			if (stock_num <= 0) return;

			/* Get the object */
			o_ptr = stock[what];

			/* Determine how many objects are in the slot */
			num = o_ptr.number;

			/* Deal with stacks */
			if (num > 1)
			{
				/* Special behaviour for arrows, bolts &tc. */
				switch (o_ptr.tval)
				{
					case TVal.TV_SPIKE:
					case TVal.TV_SHOT:
					case TVal.TV_ARROW:
					case TVal.TV_BOLT:
					{
						/* 50% of the time, destroy the entire stack */
						if (Random.randint0(100) < 50 || num < 10)
							num = o_ptr.number;

						/* 50% of the time, reduce the size to a multiple of 5 */
						else
							num = (int)(Random.randint1((int)(num / 5)) * 5 + (num % 5));

						break;
					}

					default:
					{
						/* 50% of the time, destroy a single object */
						if (Random.randint0(100) < 50) num = 1;

						/* 25% of the time, destroy half the objects */
						else if (Random.randint0(100) < 50) num = (num + 1) / 2;
				
						/* 25% of the time, destroy all objects */
						else num = o_ptr.number;

						/* Hack -- decrement the total charges of staves and wands. */
						if (o_ptr.tval == TVal.TV_STAFF || o_ptr.tval == TVal.TV_WAND)
						{
							o_ptr.pval[Misc.DEFAULT_PVAL] -= (short)(num * o_ptr.pval[Misc.DEFAULT_PVAL] / o_ptr.number);
						}
						break;
					}
				}

			}

			if (o_ptr.artifact != null)
				History.lose_artifact(o_ptr.artifact);

			/* Delete the item */
			item_increase(what, -num);
			item_optimize(what);
		}

		/*
		 * Helper function: create an item with the given tval,sval pair, add it to the
		 * store st.  Return the slot in the inventory.
		 */
		int create_item(Object_Kind kind)
		{
			Object.Object obj = new Object.Object();

			/* Create a new object of the chosen kind */
			obj.prep(kind, 0, aspect.RANDOMISE);

			/* Item belongs to a store */
			obj.ident |= Object.Object.IDENT_STORE;
			obj.origin = Origin.STORE;

			/* Attempt to carry the object */
			return carry(obj);
		}

		/*
		 * Add an object to a real stores inventory.
		 *
		 * If the object is "worthless", it is thrown away (except in the home).
		 *
		 * If the object cannot be combined with an object already in the inventory,
		 * make a new slot for it, and calculate its "per item" price.  Note that
		 * this price will be negative, since the price will not be "fixed" yet.
		 * Adding an object to a "fixed" price stack will not change the fixed price.
		 *
		 * In all cases, return the slot (or -1) where the object was placed
		 */
		int carry(Object.Object o_ptr)
		{
			int slot;
			int value, j_value;
			Object.Object j_ptr;

			Object_Kind kind = o_ptr.kind;

			/* Evaluate the object */
			value = o_ptr.value(1, false);

			/* Cursed/Worthless items "disappear" when sold */
			if (value <= 0) return (-1);

			/* Erase the inscription & pseudo-ID bit */
			o_ptr.note = null;

			/* Some item types require maintenance */
			switch (o_ptr.tval)
			{
			    /* Refuel lights to the standard amount */
			    case TVal.TV_LIGHT:
			    {
			        Bitflag f = new Bitflag(Object_Flag.SIZE);
			        o_ptr.object_flags(ref f);

			        if (!f.has(Object_Flag.NO_FUEL.value))
			        {
			            if (o_ptr.sval == SVal.SV_LIGHT_TORCH)
			                o_ptr.timeout = Misc.DEFAULT_TORCH;

			            else if (o_ptr.sval == SVal.SV_LIGHT_LANTERN)
			                o_ptr.timeout = Misc.DEFAULT_LAMP;
			        }

			        break;
			    }

			    /* Recharge rods */
			    case TVal.TV_ROD:
			    {
			        o_ptr.timeout = 0;
			        break;
			    }

			    /* Possibly recharge wands and staves */
			    case TVal.TV_STAFF:
			    case TVal.TV_WAND:
			    {
			        bool recharge = false;

			        /* Recharge without fail if the store normally carries that type */
			        for (int i = 0; i < table_num; i++)
			        {
			            if (table[i] == o_ptr.kind)
			                recharge = true;
			        }

			        if (recharge)
			        {
			            int charges = 0;

			            /* Calculate the recharged number of charges */
			            for (int i = 0; i < o_ptr.number; i++)
			                charges += Random.randcalc(kind.charge, 0, aspect.RANDOMISE);

			            /* Use recharged value only if greater */
			            if (charges > o_ptr.pval[Misc.DEFAULT_PVAL])
			                o_ptr.pval[Misc.DEFAULT_PVAL] = (short)charges;
			        }

			        break;
			    }
			}

			/* Check each existing object (try to combine) */
			for (slot = 0; slot < stock_num; slot++)
			{
			    /* Get the existing object */
			    j_ptr = stock[slot];

			    /* Can the existing items be incremented? */
			    if (j_ptr.similar(o_ptr, Object.Object.object_stack_t.OSTACK_STORE))
			    {
			        /* Absorb (some of) the object */
			        store_object_absorb(ref j_ptr, ref o_ptr);

			        /* All done */
			        return (slot);
			    }
			}

			/* No space? */
			if (stock_num >= stock_size) {
			    return (-1);
			}

			/* Check existing slots to see if we must "slide" */
			for (slot = 0; slot < stock_num; slot++)
			{
			    /* Get that object */
			    j_ptr = stock[slot];

			    /* Objects sort by decreasing type */
			    if (o_ptr.tval > j_ptr.tval) break;
			    if (o_ptr.tval < j_ptr.tval) continue;

			    /* Objects sort by increasing sval */
			    if (o_ptr.sval < j_ptr.sval) break;
			    if (o_ptr.sval > j_ptr.sval) continue;

			    /* Evaluate that slot */
			    j_value = j_ptr.value(1, false);

			    /* Objects sort by decreasing value */
			    if (value > j_value) break;
			    if (value < j_value) continue;
			}

			/* Slide the others up */
			for (int i = stock_num; i > slot; i--)
			{
				stock[i] = stock[i - 1];
			    /* Hack -- slide the objects */
			    //object_copy(&store.stock[i], &store.stock[i-1]);
			}

			/* More stuff now */
			stock_num++;

			/* Hack -- Insert the new object */
			stock[slot] = o_ptr;
			//object_copy(&store.stock[slot], o_ptr);

			/* Return the location */
			return (slot);
		}

		/*
		 * Allow a store object to absorb another object
		 */
		static void store_object_absorb(ref Object.Object o_ptr, ref Object.Object j_ptr)
		{
			int total = o_ptr.number + j_ptr.number;

			/* Combine quantity, lose excess items */
			o_ptr.number = (byte)((total > 99) ? 99 : total);

			/* Hack -- if rods are stacking, add the charging timeouts */
			if (o_ptr.tval == TVal.TV_ROD)
				o_ptr.timeout += j_ptr.timeout;

			/* Hack -- if wands/staves are stacking, combine the charges */
			if ((o_ptr.tval == TVal.TV_WAND) || (o_ptr.tval == TVal.TV_STAFF))
			{
				o_ptr.pval[Misc.DEFAULT_PVAL] += j_ptr.pval[Misc.DEFAULT_PVAL];
			}

			if ((o_ptr.origin != j_ptr.origin) ||
				(o_ptr.origin_depth != j_ptr.origin_depth) ||
				(o_ptr.origin_xtra != j_ptr.origin_xtra))
			{
				int act = 2;

				if ((o_ptr.origin == Origin.DROP) && (o_ptr.origin == j_ptr.origin))
				{
					Monster_Race r_ptr = Misc.r_info[o_ptr.origin_xtra];
					Monster_Race s_ptr = Misc.r_info[j_ptr.origin_xtra];

					bool r_uniq = r_ptr.flags.has(Monster_Flag.UNIQUE.value) ? true : false;
					bool s_uniq = s_ptr.flags.has(Monster_Flag.UNIQUE.value) ? true : false;

					if (r_uniq && !s_uniq) act = 0;
					else if (s_uniq && !r_uniq) act = 1;
					else act = 2;
				}

				switch (act)
				{
					/* Overwrite with j_ptr */
					case 1:
					{
						o_ptr.origin = j_ptr.origin;
						o_ptr.origin_depth = j_ptr.origin_depth;
						o_ptr.origin_xtra = j_ptr.origin_xtra;
						o_ptr.origin = Origin.MIXED;
						break;//No break before. Origin mixed was copied.
					}

					/* Set as "mixed" */
					case 2:
					{
						o_ptr.origin = Origin.MIXED;
						break;
					}
				}
			}
		}


		/*
		 * Some cheap objects should be created in piles.
		 */
		static void mass_produce(Object.Object o_ptr)
		{
			int size = 1;
			int cost = o_ptr.value(1, false);

			/* Analyze the type */
			switch (o_ptr.tval)
			{
			    /* Food, Flasks, and Lights */
			    case TVal.TV_FOOD:
			    case TVal.TV_FLASK:
			    case TVal.TV_LIGHT:
			    {
			        if (cost <= 5L) size += mass_roll(3, 5);
			        if (cost <= 20L) size += mass_roll(3, 5);
			        break;
			    }

			    case TVal.TV_POTION:
			    case TVal.TV_SCROLL:
			    {
			        if (cost <= 60L) size += mass_roll(3, 5);
			        if (cost <= 240L) size += mass_roll(1, 5);
			        break;
			    }

			    case TVal.TV_MAGIC_BOOK:
			    case TVal.TV_PRAYER_BOOK:
			    {
			        if (cost <= 50L) size += mass_roll(2, 3);
			        if (cost <= 500L) size += mass_roll(1, 3);
			        break;
			    }

			    case TVal.TV_SOFT_ARMOR:
			    case TVal.TV_HARD_ARMOR:
			    case TVal.TV_SHIELD:
			    case TVal.TV_GLOVES:
			    case TVal.TV_BOOTS:
			    case TVal.TV_CLOAK:
			    case TVal.TV_HELM:
			    case TVal.TV_CROWN:
			    case TVal.TV_SWORD:
			    case TVal.TV_POLEARM:
			    case TVal.TV_HAFTED:
			    case TVal.TV_DIGGING:
			    case TVal.TV_BOW:
			    {
			        if (o_ptr.ego != null) break;
			        if (cost <= 10L) size += mass_roll(3, 5);
			        if (cost <= 100L) size += mass_roll(3, 5);
			        break;
			    }

			    case TVal.TV_SPIKE:
			    case TVal.TV_SHOT:
			    case TVal.TV_ARROW:
			    case TVal.TV_BOLT:
			    {
			        if (cost <= 5L)
			            size = (int)Random.randint1(3) * 20;         /* 20-60 in 20s */
			        else if (cost > 5L && cost <= 50L)
			            size = (int)Random.randint1(4) * 10;         /* 10-40 in 10s */
			        else if (cost > 50 && cost <= 500L)
			            size = (int)Random.randint1(4) * 5;          /* 5-20 in 5s */
			        else
			            size = 1;

			        break;
			    }
			}


			/* Save the total pile size */
			o_ptr.number = (byte)size;
		}

		/*
		 * Special "mass production" computation.
		 */
		static int mass_roll(int times, int max)
		{
			int i, t = 0;

			Misc.assert(max > 1);

			for (i = 0; i < times; i++)
				t += Random.randint0(max);

			return (t);
		}

		/*
		 * Get a choice from the store allocation table, in tables.c
		 */
		Object_Kind get_choice()
		{
			/* Choose a random entry from the store's table */
			int r = Random.randint0((int)table_num);

			/* Return it */
			return table[r];
		}

		/* Get the current store or null if there isn't one */
		public static Store current_store()
		{
			STORE n = STORE.NONE;

			/* If we're displaying store knowledge whilst not in a store,
			 * override the value returned
			 */
			if (Misc.store_knowledge != STORE.NONE)
			    n = Misc.store_knowledge;

			else if ((Cave.cave.feat[Misc.p_ptr.py][Misc.p_ptr.px] >= Cave.FEAT_SHOP_HEAD) &&
			        (Cave.cave.feat[Misc.p_ptr.py][Misc.p_ptr.px] <= Cave.FEAT_SHOP_TAIL))
			    n = (STORE)(Cave.cave.feat[Misc.p_ptr.py][Misc.p_ptr.px] - Cave.FEAT_SHOP_HEAD);

			if (n != STORE.NONE)
			    return Misc.stores[(int)n];
			else
			    return null;
		}

	}
}
