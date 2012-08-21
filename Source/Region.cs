using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	/* ================== GEOMETRY ====================== */
	/* Defines a rectangle on the screen that is bound to a Panel or subpanel */
	class Region {
		public Region() {
			col = 0;
			row = 0;
			width = 0;
			page_rows = 0;
		}

		public Region(int col, int row, int width, int page_rows){
			this.col = col;
			this.row = row;
			this.width = width;
			this.page_rows = page_rows;
		}

		public Region(Region copy){
			this.col = copy.col;
			this.row = copy.row;
			this.width = copy.width;
			this.page_rows = copy.page_rows;
		}

		public int col;	/* x-coordinate of upper right corner */
		public int row;	/* y-coord of upper right coordinate */
		public int width;	/* width of display area. 1 - use system default. */
				/* non-positive - rel to right of screen */
		public int page_rows;	/* non-positive value is relative to the bottom of the screen */

		/* Region that defines the full screen */
		public static Region SCREEN_REGION = new Region(0, 0, 0, 0);

		/* Erase the contents of a region */
		public void erase()
		{
			Region calc = calculate();
			int i = 0;

			for (i = 0; i < calc.page_rows; i++)
				Term.erase(calc.col, calc.row + i, calc.width);
		}

		/* Erase the contents of a region + 1 char each way */
		public void erase_bordered()
		{
			Region calc = calculate();
			int i = 0;

			calc.col = Math.Max(calc.col - 1, 0);
			calc.row = Math.Max(calc.row - 1, 0);
			calc.width += 2;
			calc.page_rows += 2;

			for (i = 0; i < calc.page_rows; i++)
				Term.erase(calc.col, calc.row + i, calc.width);
		}

		/* Given a region with relative values, make them absolute */
		public Region calculate()
		{
			int w, h;
			Term.get_size(out w, out h);

			Region loc = new Region(this);
			if (loc.col < 0)
				loc.col += w;
			if (loc.row < 0)
				loc.row += h;
			if (loc.width <= 0)
				loc.width += w - loc.col;
			if (loc.page_rows <= 0)
				loc.page_rows += h - loc.row;

			return loc;
		}

		/* Check whether a (mouse) event is inside a region */
		public bool region_inside(ui_event key)
		{
			throw new NotImplementedException();
			////TODO make sure this is right... I don't think it is... I need to ensure the mouse.x/y is correct
			//if ((col > key.mouse.x) || (col + width <= key.mouse.x))
			//    return false;

			//if ((row > key.mouse.y) || (row + page_rows <= key.mouse.y))
			//    return false;

			//return true;
		}
	}
}
