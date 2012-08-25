using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSAngband.Object;
using CSAngband.Monster;
using CSAngband.Player;

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

		/* State flags */
		public const int STORE_GOLD_CHANGE      =0x01;
		public const int STORE_FRAME_CHANGE     =0x02;

		public const int STORE_SHOW_HELP        =0x04;

		/* Compound flag for the initial display of a store */
		public const int STORE_INIT_CHANGE = (STORE_FRAME_CHANGE | STORE_GOLD_CHANGE);


		/* Flags for the display */
		public static short store_flags;

		public static Region store_menu_region = new Region( 1, 4, -1, -2 );
		public static Menu_Type.menu_iter store_menu = new Menu_Type.menu_iter(
			null,
			null,
			store_display_entry,
			store_menu_handle,
			null
		);

		public static Menu_Type.menu_iter store_know_menu = new Menu_Type.menu_iter(
			null,
			null,
			store_display_entry,
			null,
			null
		);

		public static void Init() {
			Store store_list;

			store_list = parse_stores();
			store_list = add_builtin_stores(store_list);
			parse_owners(store_list);
			Misc.stores = flatten_stores(store_list);
		}

		/* Easy names for the elements of the 'scr_places' arrays. */
		enum LOC
		{
			PRICE = 0,
			OWNER,
			HEADER,
			MORE,
			HELP_CLEAR,
			HELP_PROMPT,
			AU,
			WEIGHT,

			MAX
		};

		/* Places for the various things displayed onscreen */
		static int[] scr_places_x = new int[(int)LOC.MAX];
		static int[] scr_places_y = new int[(int)LOC.MAX];

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
					break;
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
		public void store_shuffle()
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
		public void store_maint()
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

		/*
		 * Redisplay a single store entry
		 */
		static void store_display_entry(Menu_Type menu, int oid, bool cursor, int row, int col, int width)
		{
			Object.Object o_ptr;
			int x;
			Object.Object.Detail desc = Object.Object.Detail.PREFIX;

			//char o_name[80];
			//char out_val[160];
			string o_name;
			string out_val;
			ConsoleColor colour;

			Store store = current_store();

			Misc.assert(store != null);

			/* Get the object */
			o_ptr = store.stock[oid];

			/* Describe the object - preserving insriptions in the home */
			if (store.sidx == STORE.HOME) desc = Object.Object.Detail.FULL;
			else desc = Object.Object.Detail.FULL | Object.Object.Detail.STORE;
			o_name = o_ptr.object_desc(Object.Object.Detail.PREFIX | desc);

			/* Display the object */
			Utilities.c_put_str(Misc.tval_to_attr[o_ptr.tval & 0x7F], o_name, row, col);

			/* Show weights */
			colour = Menu_Type.curs_attrs[(int)Menu_Type.CURS.KNOWN][cursor?1:0];
			//out_val = String.Format("%3d.%d lb", o_ptr.weight / 10, o_ptr.weight % 10);
			out_val = String.Format("{0}.{1} lb", o_ptr.weight / 10, o_ptr.weight % 10);
			Utilities.c_put_str(colour, out_val, row, scr_places_x[(int)LOC.WEIGHT]);

			/* Describe an object (fully) in a store */
			if (store.sidx != STORE.HOME)
			{
			    /* Extract the "minimum" price */
			    x = price_item(o_ptr, false, 1);

			    /* Make sure the player can afford it */
			    if ((int) Misc.p_ptr.au < (int) x)
			        colour = Menu_Type.curs_attrs[(int)Menu_Type.CURS.UNKNOWN][cursor?1:0];

			    /* Actually draw the price */
			    if (((o_ptr.tval == TVal.TV_WAND) || (o_ptr.tval == TVal.TV_STAFF)) && (o_ptr.number > 1))
			        //strnfmt(out_val, sizeof out_val, "%9ld avg", (long)x);
					out_val = String.Format("{0} avg", x);
			    else
			        //strnfmt(out_val, sizeof out_val, "%9ld    ", (long)x);
					out_val = String.Format("{0}    ", (long)x);

			    Utilities.c_put_str(colour, out_val, row, scr_places_x[(int)LOC.PRICE]);
			}
		}

		/*
		 * Determine the price of an object (qty one) in a store.
		 *
		 *  store_buying == true  means the shop is buying, player selling
		 *               == false means the shop is selling, player buying
		 *
		 * This function takes into account the player's charisma, but
		 * never lets a shop-keeper lose money in a transaction.
		 *
		 * The "greed" value should exceed 100 when the player is "buying" the
		 * object, and should be less than 100 when the player is "selling" it.
		 *
		 * Hack -- the black market always charges twice as much as it should.
		 */
		public static int price_item(Object.Object o_ptr, bool store_buying, int qty)
		{
			int adjust;
			int price;
			Store store = current_store();
			Owner ot_ptr;

			if (store == null) return 0;

			ot_ptr = store.owner;


			/* Get the value of the stack of wands, or a single item */
			if ((o_ptr.tval == TVal.TV_WAND) || (o_ptr.tval == TVal.TV_STAFF))
			    price = o_ptr.value(qty, false);
			else
			    price = o_ptr.value(1, false);

			/* Worthless items */
			if (price <= 0) return (0);


			/* Add in the charisma factor */
			if (store.sidx == STORE.B_MARKET)
			    adjust = 150;
			else
			    adjust = Player.Player.adj_chr_gold[Misc.p_ptr.state.stat_ind[(int)Stat.Chr]];


			/* Shop is buying */
			if (store_buying)
			{
			    /* Set the factor */
			    adjust = 100 + (100 - adjust);
			    if (adjust > 100) adjust = 100;

			    /* Shops now pay 2/3 of true value */
			    price = price * 2 / 3;

			    /* Black market sucks */
			    if (store.sidx == STORE.B_MARKET)
			        price = price / 2;

			    /* Check for no_selling option */
			    if (Option.birth_no_selling.value) return (0);
			}

			/* Shop is selling */
			else
			{
			    /* Fix the factor */
			    if (adjust < 100) adjust = 100;

			    /* Black market sucks */
			    if (store.sidx == STORE.B_MARKET)
			        price = price * 2;
			}

			/* Compute the final price (with rounding) */
			price = (int)((price * adjust + 50L) / 100);

			/* Now convert price to total price for non-wands */
			if (!(o_ptr.tval == TVal.TV_WAND) && !(o_ptr.tval == TVal.TV_STAFF))
			    price *= qty;

			/* Now limit the price to the purse limit */
			if (store_buying && (price > ot_ptr.max_cost * qty))
			    price = ot_ptr.max_cost * qty;

			/* Note -- Never become "free" */
			if (price <= 0L) return (qty);

			/* Return the price */
			return (price);
		}

		/*
		 *
		 */
		static bool store_menu_handle(Menu_Type m, ui_event mevent, int oid)
		{
			bool processed = true;

			if (mevent.type == ui_event_type.EVT_SELECT)
			{
			    /* Nothing for now, except "handle" the event */
			    return true;
			    /* In future, maybe we want a display a list of what you can do. */
			}
			else if (mevent.type == ui_event_type.EVT_KBRD)
			{
				bool storechange = false;

				switch ((char)mevent.key.code) {
				    case 's':
				    case 'd': storechange = store_sell(); break;
				    case 'p':
				    case 'g': storechange = store_purchase(oid); break;
				    case 'l':
				    case 'x': store_examine(oid); break;

				    case '?': {
				        /* Toggle help */
				        if ((store_flags & STORE_SHOW_HELP) != 0)
				            store_flags &= ~(STORE_SHOW_HELP);
				        else
				            store_flags |= STORE_SHOW_HELP;

				        /* Redisplay */
				        store_flags |= STORE_INIT_CHANGE;
				        break;
				    }

				    case '=': {
						Do_Command.options();
				        store_menu_set_selections(m, false);
				        break;
				    }

				    default:
				        processed = store_process_command_key(mevent.key);
						break;
				}

				if ((char)mevent.key.code == UIEvent.KTRL('R')) {
					/* XXX redraw functionality should be another menu_iter handler */
					Term.clear();
					store_flags |= (STORE_FRAME_CHANGE | STORE_GOLD_CHANGE);
				}

				/* Let the game handle any core commands (equipping, etc) */
				Game_Command.process_command(cmd_context.CMD_STORE, true);

				if (storechange)
				    store_menu_recalc(m);

				if (processed) {
					Game_Event.signal(Game_Event.Event_Type.INVENTORY);
					Game_Event.signal(Game_Event.Event_Type.EQUIPMENT);
				}

				/* Notice and handle stuff */
				Misc.p_ptr.notice_stuff();
				Misc.p_ptr.handle_stuff();

				/* Display the store */
				store_display_recalc(m);
				store_menu_recalc(m);
				store_redraw();

				return processed;
			}

			return false;
		}

		/*
		 * Process a command in a store
		 *
		 * Note that we must allow the use of a few "special" commands in the stores
		 * which are not allowed in the dungeon, and we must disable some commands
		 * which are allowed in the dungeon but not in the stores, to prevent chaos.
		 */
		static bool store_process_command_key(keypress kp)
		{
			throw new NotImplementedException();
			//int cmd = 0;

			///* Process the keycode */
			//switch (kp.code) {
			//    case 'T': /* roguelike */
			//    case 't': cmd = CMD_TAKEOFF; break;

			//    case KTRL('D'): /* roguelike */
			//    case 'k': textui_cmd_destroy(); break;

			//    case 'P': /* roguelike */
			//    case 'b': textui_spell_browse(); break;

			//    case '~': textui_browse_knowledge(); break;
			//    case 'I': textui_obj_examine(); break;
			//    case 'w': cmd = CMD_WIELD; break;
			//    case '{': cmd = CMD_INSCRIBE; break;
			//    case '}': cmd = CMD_UNINSCRIBE; break;

			//    case 'e': do_cmd_equip(); break;
			//    case 'i': do_cmd_inven(); break;
			//    case KTRL('E'): toggle_inven_equip(); break;
			//    case 'C': do_cmd_change_name(); break;
			//    case KTRL('P'): do_cmd_messages(); break;
			//    case ')': do_cmd_save_screen(); break;

			//    default: return false;
			//}

			//if (cmd)
			//    cmd_insert_repeated(cmd, 0);

			//return true;
		}

		/*
		 * Sell an object, or drop if it we're in the home.
		 */
		static bool store_sell()
		{
			throw new NotImplementedException();
			//int amt;
			//int item;
			//int get_mode = USE_EQUIP | USE_INVEN | USE_FLOOR;

			//object_type *o_ptr;
			//object_type object_type_body;
			//object_type *i_ptr = &object_type_body;

			//char o_name[120];


			//const char *reject = "You have nothing that I want. ";
			//const char *prompt = "Sell which item? ";

			//struct store *store = current_store();

			//if (!store) {
			//    msg("You cannot sell items when not in a store.");
			//    return false;
			//}

			///* Clear all current messages */
			//msg_flag = false;
			//prt("", 0, 0);

			//if (store.sidx == STORE_HOME) {
			//    prompt = "Drop which item? ";
			//} else {
			//    item_tester_hook = store_will_buy_tester;
			//    get_mode |= SHOW_PRICES;
			//}

			///* Get an item */
			//p_ptr.command_wrk = USE_INVEN;

			//if (!get_item(&item, prompt, reject, CMD_DROP, get_mode))
			//    return false;

			///* Get the item */
			//o_ptr = object_from_item_idx(item);

			///* Hack -- Cannot remove cursed objects */
			//if ((item >= INVEN_WIELD) && cursed_p(o_ptr.flags))
			//{
			//    /* Oops */
			//    msg("Hmmm, it seems to be cursed.");

			//    /* Nope */
			//    return false;
			//}

			///* Get a quantity */
			//amt = get_quantity(null, o_ptr.number);

			///* Allow user abort */
			//if (amt <= 0) return false;

			///* Get a copy of the object representing the number being sold */
			//object_copy_amt(i_ptr, object_from_item_idx(item), amt);

			//if (!store_check_num(store, i_ptr))
			//{
			//    if (store.sidx == STORE_HOME)
			//        msg("Your home is full.");

			//    else
			//        msg("I have not the room in my store to keep it.");

			//    return false;
			//}

			///* Get a full description */
			//object_desc(o_name, sizeof(o_name), i_ptr, ODESC_PREFIX | ODESC_FULL);

			///* Real store */
			//if (store.sidx != STORE_HOME)
			//{
			//    /* Extract the value of the items */
			//    u32b price = price_item(i_ptr, true, amt);

			//    screen_save();

			//    /* Show price */
			//    prt(format("Price: %d", price), 1, 0);

			//    /* Confirm sale */
			//    if (!store_get_check(format("Sell %s? [ESC, any other key to accept]", o_name)))
			//    {
			//        screen_load();
			//        return false;
			//    }

			//    screen_load();

			//    cmd_insert(CMD_SELL);
			//    cmd_set_arg_item(cmd_get_top(), 0, item);
			//    cmd_set_arg_number(cmd_get_top(), 1, amt);
			//}

			///* Player is at home */
			//else
			//{
			//    cmd_insert(CMD_STASH);
			//    cmd_set_arg_item(cmd_get_top(), 0, item);
			//    cmd_set_arg_number(cmd_get_top(), 1, amt);
			//}

			//return true;
		}

		/*
		 * Buy an object from a store
		 */
		static bool store_purchase(int item)
		{
			int amt, num;
			int price;

			Object.Object o_ptr;

			//object_type object_type_body;
			Object.Object i_ptr = new Object.Object();

			//char o_name[80];
			string o_name;

			Store store = current_store();

			if (store == null) {
			    Utilities.msg("You cannot purchase items when not in a store.");
			    return false;
			}

			/* Get the actual object */
			o_ptr = store.stock[item];
			if (item < 0) return false;

			/* Clear all current messages */
			Term.msg_flag = false;
			Utilities.prt("", 0, 0);

			if (store.sidx == STORE.HOME) {
			    amt = o_ptr.number;
			} else {
			    /* Price of one */
			    price = price_item(o_ptr, false, 1);

			    /* Check if the player can afford any at all */
			    if ((uint)Misc.p_ptr.au < (uint)price)
			    {
			        /* Tell the user */
			        Utilities.msg("You do not have enough gold for this item.");

			        /* Abort now */
			        return false;
			    }

			    /* Work out how many the player can afford */
			    amt = Misc.p_ptr.au / price;
			    if (amt > o_ptr.number) amt = o_ptr.number;
		
			    /* Double check for wands/staves */
			    if ((Misc.p_ptr.au >= price_item(o_ptr, false, amt+1)) && (amt < o_ptr.number))
			        amt++;

			}

			/* Find the number of this item in the inventory */
			if (!o_ptr.flavor_is_aware())
			    num = 0;
			else
			    num = find_inven(o_ptr);

			o_name = String.Format("{0} how many{1}? (max {2}) ",
			        (store.sidx == STORE.HOME) ? "Take" : "Buy",
			        num != 0 ? String.Format(" (you have {0})", num) : "", amt);

			/* Get a quantity */
			amt = Utilities.get_quantity(o_name, amt);

			/* Allow user abort */
			if (amt <= 0) return false;

			/* Get desired object */
			i_ptr.copy_amt(o_ptr, amt);

			/* Ensure we have room */
			if (!i_ptr.inven_carry_okay())
			{
			    Utilities.msg("You cannot carry that many items.");
			    return false;
			}

			/* Describe the object (fully) */
			o_name = i_ptr.object_desc(Object.Object.Detail.PREFIX | Object.Object.Detail.FULL);

			/* Attempt to buy it */
			if (store.sidx != STORE.HOME)
			{
			    bool response;

			    /* Extract the price for the entire stack */
			    price = price_item(i_ptr, false, i_ptr.number);

			    Utilities.screen_save();

			    /* Show price */
			    Utilities.prt(String.Format("Price: {0}", price), 1, 0);

			    /* Confirm purchase */
			    response = store_get_check(String.Format("Buy {0}? [ESC, any other key to accept]", o_name));
			    Utilities.screen_load();

			    /* Negative response, so give up */
			    if (!response) return false;

				Game_Command.insert(Command_Code.BUY);
				Game_Command.get_top().set_arg_choice(0, item);
				Game_Command.get_top().set_arg_number(1, amt);
			}

			/* Home is much easier */
			else
			{
				Game_Command.insert(Command_Code.RETRIEVE);
				Game_Command.get_top().set_arg_choice(0, item);
				Game_Command.get_top().set_arg_number(1, amt);
			}

			/* Not kicked out */
			return true;
		}

		static bool store_get_check(string prompt)
		{
			keypress ch;

			/* Prompt for it */
			Utilities.prt(prompt, 0, 0);

			/* Get an answer */
			ch = Utilities.inkey();

			/* Erase the prompt */
			Utilities.prt("", 0, 0);

			if (ch.code == keycode_t.ESCAPE) return (false);
			if ("Nn".Contains((char)ch.code)) return (false);

			/* Success */
			return (true);
		}

		/*
		 * Return the quantity of a given item in the pack (include quiver).
		 */
		static int find_inven(Object.Object o_ptr)
		{
			int i, j;
			int num = 0;

			/* Similar slot? */
			for (j = 0; j < Misc.QUIVER_END; j++)
			{
			    Object.Object j_ptr = Misc.p_ptr.inventory[j];

			    /* Check only the inventory and the quiver */
			    if (j >= Misc.INVEN_WIELD && j < Misc.QUIVER_START) continue;

			    /* Require identical object types */
			    if (o_ptr.kind != j_ptr.kind) continue;

			    /* Analyze the items */
			    switch (o_ptr.tval)
			    {
			        /* Chests */
			        case TVal.TV_CHEST:
			        {
			            /* Never okay */
			            return 0;
			        }

			        /* Food and Potions and Scrolls */
			        case TVal.TV_FOOD:
			        case TVal.TV_POTION:
			        case TVal.TV_SCROLL:
			        {
			            /* Assume okay */
			            break;
			        }

			        /* Staves and Wands */
			        case TVal.TV_STAFF:
			        case TVal.TV_WAND:
			        {
			            /* Assume okay */
			            break;
			        }

			        /* Rods */
			        case TVal.TV_ROD:
			        {
			            /* Assume okay */
			            break;
			        }

			        /* Weapons and Armor */
			        case TVal.TV_BOW:
			        case TVal.TV_DIGGING:
			        case TVal.TV_HAFTED:
			        case TVal.TV_POLEARM:
			        case TVal.TV_SWORD:
			        case TVal.TV_BOOTS:
			        case TVal.TV_GLOVES:
			        case TVal.TV_HELM:
			        case TVal.TV_CROWN:
			        case TVal.TV_SHIELD:
			        case TVal.TV_CLOAK:
			        case TVal.TV_SOFT_ARMOR:
			        case TVal.TV_HARD_ARMOR:
			        case TVal.TV_DRAG_ARMOR:
			        /* Fall through */

			        /* Rings, Amulets, Lights */
			        case TVal.TV_RING:
			        case TVal.TV_AMULET:
			        case TVal.TV_LIGHT:
			        {
			            /* Require both items to be known */
			            if (!o_ptr.is_known() || !j_ptr.is_known()) continue;

			            /* Fall through */
						goto fucking_fallthroughs;
			        }

			        /* Missiles */
			        case TVal.TV_BOLT:
			        case TVal.TV_ARROW:
			        case TVal.TV_SHOT:
			        fucking_fallthroughs:
					{
			            /* Require identical knowledge of both items */
			            if (o_ptr.is_known() != j_ptr.is_known()) continue;

			            /* Require identical "bonuses" */
			            if (o_ptr.to_h != j_ptr.to_h) continue;
			            if (o_ptr.to_d != j_ptr.to_d) continue;
			            if (o_ptr.to_a != j_ptr.to_a) continue;

			            /* Require identical "pval" codes */
			            for (i = 0; i < Misc.MAX_PVALS; i++)
			                if (o_ptr.pval[i] != j_ptr.pval[i])
			                    continue;

			            if (o_ptr.num_pvals != j_ptr.num_pvals)
			                continue;

			            /* Require identical "artifact" names */
			            if (o_ptr.artifact != j_ptr.artifact) continue;

			            /* Require identical "ego-item" names */
			            if (o_ptr.ego != j_ptr.ego) continue;

			            /* Lights must have same amount of fuel */
			            else if (o_ptr.timeout != j_ptr.timeout && o_ptr.tval == TVal.TV_LIGHT)
			                continue;

			            /* Require identical "values" */
			            if (o_ptr.ac != j_ptr.ac) continue;
			            if (o_ptr.dd != j_ptr.dd) continue;
			            if (o_ptr.ds != j_ptr.ds) continue;

			            /* Probably okay */
			            break;
			        }

			        /* Various */
			        default:
			        {
			            /* Require knowledge */
			            if (!o_ptr.is_known() || !j_ptr.is_known()) continue;

			            /* Probably okay */
			            break;
			        }
			    }


			    /* Different flags */
			    if (!o_ptr.flags.is_equal(j_ptr.flags))
			        continue;

			    /* They match, so add up */
			    num += j_ptr.number;
			}

			return num;
		}

		/*
		 * Examine an item in a store
		 */
		static void store_examine(int item)
		{
			throw new NotImplementedException();
			//struct store *store = current_store();
			//object_type *o_ptr;

			//char header[120];

			//textblock *tb;
			//region area = { 0, 0, 0, 0 };

			//if (item < 0) return;

			///* Get the actual object */
			//o_ptr = &store.stock[item];

			///* Show full info in most stores, but normal info in player home */
			//tb = object_info(o_ptr, (store.sidx != STORE_HOME) ? OINFO_FULL : OINFO_NONE);
			//object_desc(header, sizeof(header), o_ptr, ODESC_PREFIX | ODESC_FULL);

			//textui_textblock_show(tb, area, header);
			//textblock_free(tb);

			///* Hack -- Browse book, then prompt for a command */
			//if (o_ptr.tval == p_ptr.class.spell_book)
			//    textui_book_browse(o_ptr);
		}

		public static void store_menu_set_selections(Menu_Type menu, bool knowledge_menu)
		{
			if (knowledge_menu)
			{
			    if (Option.rogue_like_commands.value)
			    {
			        /* These two can't intersect! */
			        menu.cmd_keys = "?Ieilx";
			        menu.selections = "abcdfghjkmnopqrstuvwyz134567";
			    }
			    /* Original */
			    else
			    {
			        /* These two can't intersect! */
			        menu.cmd_keys = "?Ieil";
			        menu.selections = "abcdfghjkmnopqrstuvwxyz13456";
			    }
			}
			else
			{
			    /* Roguelike */
			    if (Option.rogue_like_commands.value)
			    {
			        /* These two can't intersect! */
			        menu.cmd_keys = "\x04\x05\x10?={}~CEIPTdegilpswx"; /* \x10 = ^p , \x04 = ^D, \x05 = ^E */
			        menu.selections = "abcfmnoqrtuvyz13456790ABDFGH";
			    }

			    /* Original */
			    else
			    {
			        /* These two can't intersect! */
			        menu.cmd_keys = "\x05\x010?={}~CEIbdegiklpstwx"; /* \x05 = ^E, \x10 = ^p */
			        menu.selections = "acfhjmnoqruvyz13456790ABDFGH";
			    }
			}
		}

		/*
		 * This function sets up screen locations based on the current term size.
		 *
		 * Current screen layout:
		 *  line 0: reserved for messages
		 *  line 1: shopkeeper and their purse / item buying price
		 *  line 2: empty
		 *  line 3: table headers
		 *
		 *  line 4: Start of items
		 *
		 * If help is turned off, then the rest of the display goes as:
		 *
		 *  line (height - 4): end of items
		 *  line (height - 3): "more" prompt
		 *  line (height - 2): empty
		 *  line (height - 1): Help prompt and remaining gold
		 *
		 * If help is turned on, then the rest of the display goes as:
		 *
		 *  line (height - 7): end of items
		 *  line (height - 6): "more" prompt
		 *  line (height - 4): gold remaining
		 *  line (height - 3): command help 
		 */
		public static void store_display_recalc(Menu_Type m)
		{
			int wid, hgt;
			Region loc;

			Store store = current_store();

			Term.get_size(out wid, out hgt);

			/* Clip the width at a maximum of 104 (enough room for an 80-char item name) */
			if (wid > 104) wid = 104;

			/* Clip the text_out function at two smaller than the screen width */
			Misc.text_out_wrap = wid - 2;


			/* X co-ords first */
			scr_places_x[(int)LOC.PRICE] = wid - 14;
			scr_places_x[(int)LOC.AU] = wid - 26;
			scr_places_x[(int)LOC.OWNER] = wid - 2;
			scr_places_x[(int)LOC.WEIGHT] = wid - 14;

			/* Add space for for prices */
			if (store.sidx != STORE.HOME)
			    scr_places_x[(int)LOC.WEIGHT] -= 10;

			/* Then Y */
			scr_places_y[(int)LOC.OWNER] = 1;
			scr_places_y[(int)LOC.HEADER] = 3;

			/* If we are displaying help, make the height smaller */
			if ((store_flags & (STORE_SHOW_HELP)) != 0)
			    hgt -= 3;

			scr_places_y[(int)LOC.MORE] = hgt - 3;
			scr_places_y[(int)LOC.AU] = hgt - 1;

			loc = m.boundary;

			/* If we're displaying the help, then put it with a line of padding */
			if ((store_flags & (STORE_SHOW_HELP)) != 0)
			{
			    scr_places_y[(int)LOC.HELP_CLEAR] = hgt - 1;
			    scr_places_y[(int)LOC.HELP_PROMPT] = hgt;
			    loc.page_rows = -5;
			}
			else
			{
			    scr_places_y[(int)LOC.HELP_CLEAR] = hgt - 2;
			    scr_places_y[(int)LOC.HELP_PROMPT] = hgt - 1;
			    loc.page_rows = -2;
			}

			m.layout(loc);
		}

		public static void store_menu_recalc(Menu_Type m)
		{
			Store store = current_store();
			m.priv(store.stock_num, store.stock);
		}

		/*
		 * Decides what parts of the store display to redraw.  Called on terminal
		 * resizings and the redraw command.
		 */
		public static void store_redraw()
		{
			if ((store_flags & (STORE_FRAME_CHANGE)) != 0)
			{
			    store_display_frame();

			    if ((store_flags & STORE_SHOW_HELP) != 0)
			        store_display_help();
			    else
			        Utilities.prt("Press '?' for help.", scr_places_y[(int)LOC.HELP_PROMPT], 1);

			    store_flags &= ~(STORE_FRAME_CHANGE);
			}

			if ((store_flags & (STORE_GOLD_CHANGE)) != 0)
			{
			    /*Utilities.prt(String.Format("Gold Remaining: %9ld", (long)Misc.p_ptr.au),
			        scr_places_y[(int)LOC.AU], scr_places_x[(int)LOC.AU]);*/
				Utilities.prt(String.Format("Gold Remaining: {0}", (long)Misc.p_ptr.au),
			        scr_places_y[(int)LOC.AU], scr_places_x[(int)LOC.AU]);
			    store_flags &= ~(STORE_GOLD_CHANGE);
			}
		}

		/*
		 * Display store (after clearing screen)
		 */
		static void store_display_frame()
		{
			//char buf[80];
			string buf;
			Store store = current_store();
			Owner ot_ptr = store.owner;

			/* Clear screen */
			Term.clear();

			/* The "Home" is special */
			if (store.sidx == STORE.HOME)
			{
			    /* Put the owner name */
			    Utilities.put_str("Your Home", scr_places_y[(int)LOC.OWNER], 1);

			    /* Label the object descriptions */
			    Utilities.put_str("Home Inventory", scr_places_y[(int)LOC.HEADER], 1);

			    /* Show weight header */
			    Utilities.put_str("Weight", scr_places_y[(int)LOC.HEADER], scr_places_x[(int)LOC.WEIGHT] + 2);
			}

			/* Normal stores */
			else
			{
			    string store_name = Misc.f_info[Cave.FEAT_SHOP_HEAD + (int)store.sidx].name;
			    string owner_name = ot_ptr.name;

			    /* Put the owner name */
			    Utilities.put_str(owner_name, scr_places_y[(int)LOC.OWNER], 1);

			    /* Show the max price in the store (above prices) */
			    //buf = String.Format("{0} (%ld)", store_name, (long)(ot_ptr.max_cost));
				buf = String.Format("{0} ({1})", store_name, (long)(ot_ptr.max_cost));
			    Utilities.prt(buf, scr_places_y[(int)LOC.OWNER], scr_places_x[(int)LOC.OWNER] - buf.Length);

			    /* Label the object descriptions */
			    Utilities.put_str("Store Inventory", scr_places_y[(int)LOC.HEADER], 1);

			    /* Showing weight label */
			    Utilities.put_str("Weight", scr_places_y[(int)LOC.HEADER], scr_places_x[(int)LOC.WEIGHT] + 2);

			    /* Label the asking price (in stores) */
			    Utilities.put_str("Price", scr_places_y[(int)LOC.HEADER], scr_places_x[(int)LOC.PRICE] + 4);
			}
		}


		/*
		 * Display help.
		 */
		static void store_display_help()
		{
			throw new NotImplementedException();
			//int help_loc = scr_places_y[LOC_HELP_PROMPT];
			//struct store *store = current_store();
			//bool is_home = (store.sidx == STORE_HOME) ? true : false;

			///* Clear */
			//clear_from(scr_places_y[LOC_HELP_CLEAR]);

			///* Prepare help hooks */
			//text_out_hook = text_out_to_screen;
			//text_out_indent = 1;
			//Term_gotoxy(1, help_loc);

			//text_out("Use the ");
			//text_out_c(TERM_L_GREEN, "movement keys");
			//text_out(" to navigate, or ");
			//text_out_c(TERM_L_GREEN, "Space");
			//text_out(" to advance to the next page. '");

			//if (OPT(rogue_like_commands))
			//    text_out_c(TERM_L_GREEN, "x");
			//else
			//    text_out_c(TERM_L_GREEN, "l");

			//text_out("' examines");
			//if (store_knowledge == STORE_NONE)
			//{
			//    text_out(" and '");
			//    text_out_c(TERM_L_GREEN, "p");

			//    if (is_home) text_out("' picks up");
			//    else text_out("' purchases");
			//}
			//text_out(" the selected item. '");

			//if (store_knowledge == STORE_NONE)
			//{
			//    text_out_c(TERM_L_GREEN, "d");
			//    if (is_home) text_out("' drops");
			//    else text_out("' sells");
			//}
			//else
			//{
			//    text_out_c(TERM_L_GREEN, "I");
			//    text_out("' inspects");
			//}
			//text_out(" an item from your inventory. ");

			//text_out_c(TERM_L_GREEN, "ESC");
			//if (store_knowledge == STORE_NONE)
			//{
			//    text_out(" exits the building.");
			//}
			//else
			//{
			//    text_out(" exits this screen.");
			//}

			//text_out_indent = 0;
		}


		//Nick: The first 3 were commented out.
		static string[] comment_hint = new string[]
		{
			/*"{0} tells you soberly: \"{0}\".",
			"({0}) There's a saying round here, \"{0}\".",
			"{0} offers to tell you a secret next time you're about."*/
			"\"{0}\""
		};

		/*
		 * Shopkeeper welcome messages.
		 *
		 * The shopkeeper's name must come first, then the character's name.
		 */
		static string[] comment_welcome =
		{
			"",
			"{0} nods to you.",
			"{0} says hello.",
			"{0}: \"See anything you like, adventurer?\"",
			"{0}: \"How may I help you, {1}?\"",
			"{0}: \"Welcome back, {1}.\"",
			"{0}: \"A pleasure to see you again, {1}.\"",
			"{0}: \"How may I be of assistance, good {1}?\"",
			"{0}: \"You do honour to my humble store, noble {1}.\"",
			"{0}: \"Me and my family are entirely at your service, glorious {1}.\""
		};


		/*
		 * The greeting a shopkeeper gives the character says a lot about his
		 * general attitude.
		 *
		 * Taken and modified from Sangband 1.0.
		 */
		public static void prt_welcome(Owner ot_ptr)
		{
			//char short_name[20];
			string short_name;
			string owner_name = ot_ptr.name;

			int j;

			if (Random.one_in_(2))
			    return;

			///* Extract the first name of the store owner (stop before the first space) */
			//for (j = 0; owner_name[j] && owner_name[j] != ' '; j++)
			//    short_name[j] = owner_name[j];

			///* Truncate the name */
			//short_name[j] = '\0';

			short_name = owner_name.Split(' ')[0];

			if (Random.one_in_(3)) {
			    int i = Random.randint0(comment_hint.Length);
			    Utilities.msg(comment_hint[i], (object)Xtra3.random_hint());
			} else if (Misc.p_ptr.lev > 5) {
			    string player_name;

			    /* We go from level 1 - 50  */
			    int i = ((int)Misc.p_ptr.lev - 1) / 5;
			    i = Math.Min(i, comment_welcome.Length - 1);

			    /* Get a title for the character */
			    if ((i % 2) != 0 && Random.randint0(2) != 0) player_name = Misc.p_ptr.Class.title[(Misc.p_ptr.lev - 1) / 5];
			    else if (Random.randint0(2) != 0)       player_name = Player_Other.instance.full_name;
			    else                        player_name = (Misc.p_ptr.psex == Misc.SEX_MALE ? "sir" : "lady");

			    /* Balthazar says "Welcome" */
			    Utilities.prt(String.Format(comment_welcome[i], short_name, player_name), 0, 0);
			}
		}

	}
}
