/*
 * File: main-sdl.c
 * Purpose: Angband SDL port 
 *
 * Copyright (c) 2007 Ben Harrison, Gregory Velichansky, Eric Stevens,
 * Leon Marrick, Iain McFall, and others
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
#include "buildid.h"
#include "cmds.h"
#include "files.h"

/*
 * Comments and suggestions are welcome. The UI probably needs some
 * adjustment, and I need comments from you.
 * perhaps also something like "Angband 3.0.8 by Andrew Sidwell and others;
 * SDL port by Iain McFall an others, please see the accompanying documentation
 * for credits" or something
 */

/*
 * This file helps Angband work with at least some computers capable of running
 * SDL, a set of simple and quite popular game development libraries that work
 * on many different operating systems, including Windows, most flavours of
 * UNIX or similar, and Mac OS X.  It requires a 32-bit (or higher) machine
 * capable of displaying at least 640x480 in 256 colors.  A Pentium or better
 * is strongly recommended (for speed reasons treated more fully below).
 *
 * To use this file, use an appropriate "Makefile" or "Project File", install
 * the required libraries (described below), make sure that "USE_SDL" is
 * defined somewhere, and obtain various extra files (described below).  If
 * you are new to all this, read "makefile.sdl".
 *
 * This port uses the following libraries:  SDL (v1.2+) and  SDL_ttf.
 * All are available as source code, pre-compiled libs for developers,
 * and libs (or dlls) for players from www.libsdl.org
 *
 *
 * Other files used by this port:
 * - The game must have a collection of bitmap .fon files in /lib/xtra/font.
 *
 * - If "USE_GRAPHICS" is defined, then it also needs some bitmapped (.bmp)
 *   graphics files in /lib/xtra/graf, such as "16x16.bmp" and "16x16m.bmp".
 *
 * - The "lib/pref/pref-sdl.prf" file contains keymaps, macro definitions,
 *   and/or color redefinitions.
 * - The "lib/pref/font-sdl.prf" contains attr/char mappings for use with the
 *   normal "*.fon" font files in the "lib/xtra/font/" directory.
 *
 *
 *
 * "Term" framework by Ben Harrison (benh@phial.com).
 *
 * Original Sangband SDL port and the "intrface" module by Leon Marrick
 * (www.runegold.org/sangband).
 *
 * Additional helpful ideas by:
 * 2001 Gregory Velichansky <hmaon@bumba.net>, creator of the first Angband SDL
 * port.
 * 2006 Eric Stevens <sdltome@gmail.com>, main author of the TOME SDL port.
 */

/*
 * Comments on using SDL with Angband:
 *
 * The good news:
 * - SDL is cross-platform.  Really.  No joke.  If this port doesn't work on
 *   your system, it probably isn't SDL's fault.
 * - SDL is relatively easy to use, allowing you you to cobble up feature-rich
 *   apps that look half-way decent without too much fuss and bother.  It's
 *   wonderful for prototyping.
 * - SDL does most of what *band developers are likely to need, plus a whole
 *   lot more we haven't realized we want yet.
 * - SDL is a cleanly written open-source API; it is much less painful to get
 *   the straight word then it is with most other libraries.  Also, the SDL
 *   community offers active discussion boards, solid documentation, and
 *   numerous code examples relating to almost any question you might have
 *   occasion to ask.
 *
 * The bad news:
 * - SDL can be tedious to install.  Each individual library is straightforward
 *   enough, but *band development work requires several, the number growing as
 *   you get more sophisticated.
 * - SDL (as a stand-alone lib, without the assistance of OpenGL) can be very
 *   sluggish if you aren't careful.  It is poor at detecting, let alone making
 *   fullest use of, available video hardware, which cripples speed.  So,
 *   getting half-way decent performance in a game like Angband takes some
 *   skill and vast amounts of effort.  Speed - the lack thereof - is this
 *   port's biggest problem.  More comments below.
 * - SDL is not a complete game development library (although the add-ons help
 *   tremendously).  Much-needed functionality - text output, blit stretching,
 *   and video refresh synching being three examples - were either missing
 *   altogether or covered by functions that proved too slow or delicate for
 *   production use.  I ended up having to spend at least as much time, and
 *   write at least as much low-level code, as I did using the Win32 API.
 * - SDL, like Allegro but to a lesser extent, is falling behind current tech-
 *   nology.  Development progresses, but obsolescence looms, especially on
 *   Windows machines.
 */

#ifdef USE_SDL

#include "main.h"
#include "SDL.h"
#include "SDL_ttf.h"
#include "SDL_image.h"

/* SDL flags used for the main window surface */
static Uint32 vflags = SDL_ANYFORMAT;

/* Current screen dimensions */
static int screen_w = 800;
static int screen_h = 600;

/* Fullscreen dimensions */
static int full_w;
static int full_h;

/* Want fullscreen? */
static bool fullscreen = false;

/* XXXXXXXXX */
static char *ANGBAND_DIR_USER_SDL;

/*
 * Used as 'system' font
 */
static const char *DEFAULT_FONT_FILE = "6x10x.fon";

#define MAX_FONTS 40
char *FontList[MAX_FONTS];
static int num_fonts = 0;


/*
 * A font structure
 * Note that the data is only valid for a surface with matching
 * values for pitch & bpp. If a surface is resized the data _must_ be
 * recalculated.
 */
typedef struct sdl_Font sdl_Font;
struct sdl_Font
{
	int width;					/* The dimensions of this font (in pixels)*/
	int height;
	
	char name[32];				/* The name of this font */
	
	Uint16 pitch;				/* Pitch of the surface this font is made for */
	Uint8 bpp;					/* Bytes per pixel of the surface */
	Uint8 something;			/* Padding */
	
	int *data;					/* The data */
};

static sdl_Font SystemFont;

#define NUM_GLYPHS 256

/*
 * Window information
 * Each window has its own surface and coordinates
 */
typedef struct term_window term_window;
struct term_window
{
	term term_data;
	
	SDL_Surface *surface;	/* The surface for this window */
#ifdef USE_GRAPHICS
	SDL_Surface *tiles;		/* The appropriately sized tiles for this window */
#endif	
	byte Term_idx;			/* Index of term that relates to this */
	
	int top;				/* Window Coordinates on the main screen */
	int left;
	
	int keys;				/* Size of keypress storage */
	
	sdl_Font font;			/* Font info */
	char *req_font;			/* Requested font */
	int rows;				/* Dimension in tiles */
	int cols;
	
	int border;				/* Border width */
	int title_height;		/* Height of title bar */
	
	int width;				/* Dimension in pixels == tile_wid * cols + 2 x border*/
	int height;
	
	int tile_wid;			/* Size in pixels of a char */
	int tile_hgt;
	
	bool visible;			/* Can we see this window? */
	
	SDL_Rect uRect;			/* The part that needs to be updated */
};




typedef struct mouse_info mouse_info;
struct mouse_info
{
	int left;			/* Is it pressed? */
	int right;
	
	int leftx;			/* _IF_ left button is pressed these */
	int lefty;			/* show where it was pressed */
	
	int rightx;
	int righty;
	
	int x;						/* Current position of mouse */
	int y;
	
};

#define WINDOW_DRAW (SDL_USEREVENT + 1)

typedef struct sdl_ButtonBank sdl_ButtonBank;
typedef struct sdl_Button sdl_Button;
typedef struct sdl_Window sdl_Window;

typedef void (*button_press_func)(sdl_Button *sender);
struct sdl_Button
{
	SDL_Rect pos;				/* Position & Size */
	bool selected;				/* Selected? */
	bool visible;				/* Visible? */
	button_press_func activate;	/* A function to call when pressed */
	sdl_ButtonBank *owner;		/* Which bank is this in? */
	char caption[50];			/* Text for this button */
	Uint32 unsel_colour;		/* Button unselected colour */
	Uint32 sel_colour;			/* Selected colour*/
	Uint32 cap_colour;			/* Caption colour */
	void *data;					/* Something */
	int tag;					/* Something */
	
};

struct sdl_ButtonBank
{
	sdl_Button *buttons;		/* A collection of buttons */
	bool *used;					/* What buttons are available? */
	sdl_Window *window;			/* The window that these buttons are on */
	bool need_update;
};

/*
 * Other 'windows' (basically a surface with a position and buttons on it)
 * Currently used for the top status bar and popup windows
 */
typedef void (*sdl_WindowCustomDraw)(sdl_Window *window);
struct sdl_Window
{
	int top;				/* Position on main window */
	int left;
	
	int width;				/* Dimensions */
	int height;
	
	bool visible;			/* Visible? */
	
	SDL_Surface *surface;	/* SDL surface info */
	
	sdl_ButtonBank buttons;		/* Buttons */
	
	sdl_Font font;			/* Font */
	
	SDL_Surface *owner;		/* Who shall I display on */
	
	sdl_WindowCustomDraw draw_extra; /* Stuff to draw on the surface */
	bool need_update;
};




/*
 * The main surface of the application
 */
static SDL_Surface *AppWin;

/*
 * The status bar
 */
static sdl_Window StatusBar;

/*
 * The Popup window
 */
static sdl_Window PopUp;
static bool popped;

/*
 * Term windows
 */
static term_window windows[ANGBAND_TERM_MAX];
static int Zorder[ANGBAND_TERM_MAX];

/* Keep track of the mouse status */
static mouse_info mouse;

/*
 * The number pad consists of 10 keys, each with an SDL identifier
 */
#define is_numpad(k) \
((k == SDLK_KP0) || (k == SDLK_KP1) || (k == SDLK_KP2) || (k == SDLK_KP3) || \
 (k == SDLK_KP4) || (k == SDLK_KP5) || (k == SDLK_KP6) || \
 (k == SDLK_KP7) || (k == SDLK_KP8) || (k == SDLK_KP9) || (k == SDLK_KP_ENTER))

static int SnapRange = 5;	/* Window snap range (pixels) */
static int StatusHeight;	/* The height in pixels of the status bar */
static int SelectedTerm;	/* Current selected Term */

static int AboutSelect;		/* About button */
static int TermSelect;		/* Term selector button */
static int FontSelect;		/* Font selector button */
static int VisibleSelect;	/* Hide/unhide window button*/
static int MoreSelect;		/* Other options button */
static int QuitSelect;		/* Quit button */

/* For saving the icon for the About Box */
static SDL_Surface *mratt = null;

/* Buttons on the 'More' panel */
static int MoreOK;			/* Accept changes */
static int MoreFullscreen;	/* Fullscreen toggle button */
static int MoreSnapPlus;	/* Increase snap range */
static int MoreSnapMinus;	/* Decrease snap range */

static bool Moving;				/* Moving a window */
static bool Sizing;				/* Sizing a window */
static SDL_Rect SizingSpot;		/* Rect to descibe the sizing area */
static bool Sizingshow = false;	/* Is the resize thingy displayed? */
static SDL_Rect SizingRect;		/* Rect to describe the current resize window */

#ifdef USE_GRAPHICS
typedef struct GfxInfo GfxInfo;
struct GfxInfo
{
	const char *name;				/* Name to show on button */
	const char *gfxfile;			/* The file with tiles */
	int width;				/* Width of a tile */
	int height;				/* Height of a tile */
	const char *pref;				/* Preference file to use */
	bool avail;				/* Are the appropriate files available? */
};

static SDL_Surface *GfxSurface = null;	/* A surface for the graphics */

#define GfxModes 5
static GfxInfo GfxDesc[GfxModes] =
{
	{"None", null, -1, -1, null, true},
	{"8x8", "8x8.png", 8, 8, "old", true},
	{"16x16", "16x16.png", 16, 16, "new", true},
	{"32x32", "32x32.png", 32, 32, "david", true},
	{"8x16", "8x16.png", 16, 16, "nomad", true},
	/* XXX (GRAPHICS_PSEUDO ???) */
};


static int MoreWidthPlus;	/* Increase tile width */
static int MoreWidthMinus;	/* Decrease tile width */
static int MoreHeightPlus;	/* Increase tile height */
static int MoreHeightMinus;	/* Decrease tile height */
static int GfxButtons[GfxModes];	/* Graphics mode buttons */
static int SelectedGfx;				/* Current selected gfx */
#endif

/*
 * The basic angband text colours in an sdl friendly form
 */
static u32b text_colours[MAX_COLORS];

u32b back_colour;		/* Background colour */


/*
 * Fill in an SDL_Rect structure.
 * Note it also returns the value adjusted
 */
static SDL_Rect *RECT(int x, int y, int w, int h, SDL_Rect *rect)
{	
	rect.x = x;
	rect.y = y;
	rect.w = w;
	rect.h = h;
	
	return rect;
}

/*
 * Is a point(x, y) in a rectangle?
 */
static bool point_in(SDL_Rect *rect, int x, int y)
{
	if (x < rect.x) return (false);
	if (y < rect.y) return (false);
	if (x >= rect.x + rect.w) return (false);
	if (y >= rect.y + rect.h) return (false);
	
	/* Must be inside */
	return (true);
}

/*
 * Draw an outline box
 * Given the top, left, width & height
 */
static void sdl_DrawBox(SDL_Surface *surface, SDL_Rect *rect, Uint32 colour, int width)
{
	SDL_Rect rc;
	int left = rect.x;
	int right = rect.x + rect.w - width;
	int top = rect.y;
	int bottom = rect.y + rect.h - width;
	
	/* Top left . Top Right */
	RECT(left, top, rect.w, width, &rc);
	SDL_FillRect(surface, &rc, colour);
	
	/* Bottom left . Bottom Right */
	RECT(left, bottom, rect.w, width, &rc);
	SDL_FillRect(surface, &rc, colour);
	
	/* Top left . Bottom left */
	RECT(left, top, width, rect.h, &rc);
	SDL_FillRect(surface, &rc, colour);
	
	/* Top right . Bottom right */
	RECT(right, top, width, rect.h, &rc);
	SDL_FillRect(surface, &rc, colour);
}

/*
 * Get the width and height of a given font file
 */
static errr sdl_CheckFont(const char *fontname, int *width, int *height)
{
	char buf[1024];
	
	TTF_Font *ttf_font;
	
	/* Build the path */
	path_build(buf, sizeof(buf), ANGBAND_DIR_XTRA_FONT, fontname);
	
	/* Attempt to load it */
	ttf_font = TTF_OpenFont(buf, 0);
	
	/* Bugger */
	if (!ttf_font) return (-1);
	
	/* Get the size */
	if (TTF_SizeText(ttf_font, "M", width, height)) return (-1);
	
	/* Finished with the font */
	TTF_CloseFont(ttf_font);
	
	return (0);
}

/* 
* The sdl_Font routines
 */

/*
 * Free any memory assigned by Create()
 */
static void sdl_FontFree(sdl_Font *font)
{
	/* The only memory reserved is the data */
	FREE(font.data);
}


/*
 * Create new font data with font fontname, optimizing the data
 * for the surface given
 */
static errr sdl_FontCreate(sdl_Font *font, const char *fontname, SDL_Surface *surface)
{
	char buf[1024];
	int i;
	
	TTF_Font *ttf_font;
	
	/* Our temporary glyph will be rendered in white */
	SDL_Color white = { 255, 255, 255, 0 };
	
	int *temp_array;
	
	/* The first pixel data is stored immediately after the character indexes */
	/* But we need an extra space to store the total pixel count */
	int num_pixels = NUM_GLYPHS + 1;
	
	/* Free any old data */
	if (font.data) sdl_FontFree(font);
	
	/* Build the path */
	path_build(buf, sizeof(buf), ANGBAND_DIR_XTRA_FONT, fontname);
	
	/* Attempt to load it */
	ttf_font = TTF_OpenFont(buf, 0);
	
	/* Bugger */
	if (!ttf_font) return (-1);
	
	/* Get the size */
	if (TTF_SizeText(ttf_font, "M", &font.width, &font.height)) return (-1);
	
	/* Fill in some of the font struct */
	my_strcpy(font.name, fontname, 30);
	font.pitch = surface.pitch;
	font.bpp = surface.format.BytesPerPixel;
	
	/* Make a very large temporary holding bin for pixel offset data  XXX */
	temp_array = C_ZNEW(NUM_GLYPHS * 1000, int);
	
	/* Render and encode all the characters */
	for (i = 0; i < NUM_GLYPHS; i++)
	{
		SDL_Surface *temp_glyph;
		int x, y, v;
		Uint8 *p;
		
		/* Render this glyph, in white, onto an 8-bit surface */
		temp_glyph = TTF_RenderGlyph_Solid(ttf_font, i, white);
		
		/* Remember the current pixel array location */
		temp_array[i] = num_pixels;
		
		/* If glyph does not exist, no pixels get plotted */
		if (!temp_glyph)
		{
			fprintf(stderr, "char:%d : %s\n", i, TTF_GetError());
			continue;
		}
		
		/* Store this glyph into our temporary array */
		for (x = 0; x < temp_glyph.w; x++)
		{
			for (y = 0; y < temp_glyph.h; y++)
			{
				/* Access this pixel.  "pitch" is the pixel width in bytes of an SDL surface */
				p = (Uint8 *)temp_glyph.pixels + (y * temp_glyph.pitch) + (x * temp_glyph.format.BytesPerPixel);
				
				/* Ignore pixels without color data */
				if (!*p) continue;
				
				/* Precalculate some of the math needed to access this pixel on screen */
				v = (y * font.pitch) + (x * font.bpp);
				
				/* And store it */
				temp_array[num_pixels++] = v;
			}
		}
		
		/* Free the temporary surface */
		SDL_FreeSurface(temp_glyph);
	}
	
	/* Calculate and store access position data for the final character */
	temp_array[NUM_GLYPHS] = num_pixels;
	
	/* Make a pixel access array for this font */
	font.data = C_ZNEW(num_pixels, int);
	
	/* Save the data gathered in our temporary array */
	for (i = 0; i < num_pixels; i++) font.data[i] = temp_array[i];
	
	/* Free the temporary pixel data */
	FREE(temp_array);
	
	/* Finished with the font */
	TTF_CloseFont(ttf_font);
	
	/* Success */
	return (0);	
}




/*
 * Draw some text onto a surface
 * The surface is first checked to see if it is compatable with
 * this font, if it isn't the the font will be 're-precalculated'
 *
 * You can, I suppose, use one font on many surfaces, but it is
 * definately not recommended. One font per surface is good enough.
 */
static errr sdl_FontDraw(sdl_Font *font, SDL_Surface *surface, Uint32 colour, int x, int y, int n , const char *s)
{
	Uint8 bpp = surface.format.BytesPerPixel;
	Uint16 pitch = surface.pitch;
	
	int i, j;
	
	int start, end;
	
	Uint8 *pd0;
	
	int *data;
	
	if ((bpp != font.bpp) || (pitch != font.pitch))
		sdl_FontCreate(font, font.name, surface);
	
	data = font.data;
	/* Lock the window surface (if necessary) */
	if (SDL_MUSTLOCK(surface))
	{
		if (SDL_LockSurface(surface) < 0) return (-1);
	}
	
	/* Point to the left-top corner of the screen pixel data for this tile */
	pd0 = (Uint8 *)surface.pixels + (y * pitch) + (x * bpp);
	
	/* Advance along the string.  Adjust pixel pointer one tile per character. */
	for (i = 0; i < n; i++, pd0 += (font.width * bpp))
	{
		/* Translate the character */
		byte c = *(s + i);
		
		/* Get the first and last pixel locator we need to plot */
		start = data[c];
		end   = data[c + 1];
		
		/* Handle all of the legal bit depths */
		switch (bpp)
		{
			/* 256 colors -- pixel data is 8-bit */
			case 1:
			{
				/*
				 * Use entries in the pixel location array to jump to and
				 * change pixels to our desired color.
				 */
				for (j = start; j < end; j++) *(Uint8*)(pd0 + data[j]) = (Uint8)colour;
				break;
			}
				
				/* 15 and 16-bit color (SDL handles the details) */
			case 2:
			{
				for (j = start; j < end; j++) *(Uint16*)(pd0 + data[j]) = (Uint16)colour;
				break;
			}
				
				/* 32-bit color is easy.  24-bit color needs more testing. */
			case 3:
			case 4:
			default:
			{
				for (j = start; j < end; j++) *(Uint32*)(pd0 + data[j]) = colour;
				break;
			}
		}
	}
	
	/* Unlock the surface */
	if (SDL_MUSTLOCK(surface)) SDL_UnlockSurface(surface);
	
	/* Success */
	return (0);
}



/*
 * Draw a button on the screen
 */
static void sdl_ButtonDraw(sdl_Button *button)
{
	SDL_Surface *surface = button.owner.window.surface;
	sdl_Font *font = &button.owner.window.font;
	
	Uint32 colour = button.selected ? button.sel_colour : button.unsel_colour;
	
	if (!button.visible) return;
	
	SDL_FillRect(surface, &button.pos, colour);
	
	if (strlen(button.caption))
	{
		size_t len = strlen(button.caption);

		unsigned max = button.pos.w / font.width;
		int n = MIN(len, max);
		int l = n * font.width / 2;
		int x = button.pos.x + ((button.pos.w) / 2) - l;
		
		sdl_FontDraw(font, surface, button.cap_colour,
					 x, button.pos.y + 1, n, button.caption);
	}
}

/*
 * Adjust the position of a button
 */
static void sdl_ButtonMove(sdl_Button *button, int x, int y)
{
	button.pos.x = x;
	button.pos.y = y;
	button.owner.need_update = true;
}

/*
 * Adjust the size of a button
 */
static void sdl_ButtonSize(sdl_Button *button, int w, int h)
{
	button.pos.w = w;
	button.pos.h = h;
	button.owner.need_update = true;
}

/*
 * Set the caption
 */
static void sdl_ButtonCaption(sdl_Button *button, const char *s)
{
	my_strcpy(button.caption, s, sizeof(button.caption));
	button.owner.need_update = true;
}

/*
 * Set the visibility of a button
 */
static void sdl_ButtonVisible(sdl_Button *button, bool visible)
{
	if (button.visible != visible)
	{
		button.visible = visible;
		
		button.owner.need_update = true;
	}
}


/* Maximum amount of buttons in a bank */
#define MAX_BUTTONS 40

/*
 * The button_bank package
 */
/*
 * Initialize it
 */
static void sdl_ButtonBankInit(sdl_ButtonBank *bank, sdl_Window *window)
{
	bank.window = window;
	bank.buttons = C_ZNEW(MAX_BUTTONS, sdl_Button);
	bank.used = C_ZNEW(MAX_BUTTONS, bool);
	bank.need_update = true;
}

/*
 * Clear the bank
 */
static void sdl_ButtonBankFree(sdl_ButtonBank *bank)
{
	FREE(bank.buttons);
	FREE(bank.used);
}

/*
 * Draw all the buttons on the screen
 */
static void sdl_ButtonBankDrawAll(sdl_ButtonBank *bank)
{
	int i;
	
	for (i = 0; i < MAX_BUTTONS; i++)
	{
		sdl_Button *button = &bank.buttons[i];
		
		if (!bank.used[i]) continue;
		if (!button.visible) continue;
		
		sdl_ButtonDraw(button);
	}
	bank.need_update = false;
}

/*
 * Get a new button index
 */
static int sdl_ButtonBankNew(sdl_ButtonBank *bank)
{
	int i = 0;
	sdl_Button *new_button;
	
	while (bank.used[i] && (i < MAX_BUTTONS)) i++;
	
	if (i == MAX_BUTTONS)
	{
		/* Bugger! */
		return (-1);
	}
	
	/* Get the button */
	new_button = &bank.buttons[i];
	
	/* Mark the button as used */
	bank.used[i] = true;
	
	/* Clear it */
	WIPE(new_button, sdl_Button);
	
	/* Mark it as mine */
	new_button.owner = bank;
	
	/* Default colours */
	new_button.unsel_colour = SDL_MapRGB(bank.window.surface.format, 160, 160, 60);
	new_button.sel_colour = SDL_MapRGB(bank.window.surface.format, 210, 210, 110);
	new_button.cap_colour = SDL_MapRGB(bank.window.surface.format, 0, 0, 0);
	
	/* Success */
	return (i);
}

/*
 * Retrieve button 'idx' or null
 */
static sdl_Button *sdl_ButtonBankGet(sdl_ButtonBank *bank, int idx)
{
	/* Check the index */
	if ((idx < 0) || (idx >= MAX_BUTTONS)) return (null);
	if (!bank.used[idx]) return (null);
	
	/* Return it */
	return &bank.buttons[idx];
}

#if 0
/*
 * Remove a Button by its index
 */
static void sdl_ButtonBankRemove(sdl_ButtonBank *bank, int idx)
{
	sdl_Button *button;
	
	/* Check the index */
	if ((idx < 0) || (idx >= MAX_BUTTONS)) return;
	if (!bank.used[idx]) return;
	
	/* Grab it */
	button = &bank.buttons[idx];
	
	/* Hide it */
	button.visible = false;
	
	/* Draw it */
	bank.need_update = true;
	
	/* Forget */
	bank.used[idx] = false;
}

#endif

/*
 * Examine and respond to mouse presses
 * Return if we 'handled' the click
 */
static bool sdl_ButtonBankMouseDown(sdl_ButtonBank *bank, int x, int y)
{
	int i;
	
	/* Check every button */
	for (i = 0; i < MAX_BUTTONS; i++)
	{
		sdl_Button *button = &bank.buttons[i];
		
		/* Discard some */
		if (!bank.used[i]) continue;
		if (!button.visible) continue;
		
		/* Check the coordinates */
		if (point_in(&button.pos, x, y))
		{
			button.selected = true;
			
			/* Draw it */
			bank.need_update = true;
			
			return (true);
		}
	}
	return (false);
}

/*
 * Respond to a mouse button release
 */
static bool sdl_ButtonBankMouseUp(sdl_ButtonBank *bank, int x, int y)
{
	int i;
	
	/* Check every button */
	for (i = 0; i < MAX_BUTTONS; i++)
	{
		sdl_Button *button = &bank.buttons[i];
		
		/* Discard some */
		if (!bank.used[i]) continue;
		if (!button.visible) continue;
		
		/* Check the coordinates */
		if (point_in(&button.pos, x, y))
		{
			/* Has this butoon been 'selected'? */
			if (button.selected)
			{
				/* Activate the button (usually) */
				if (button.activate) (*button.activate)(button);
				
				/* Now not selected */
				button.selected = false;
				
				/* Draw it */
				bank.need_update = true;
				
				return (true);
			}
		}
		else
		{
			/* This button was 'selected' but the release of the */
			/* mouse button was outside the area of this button */
			if (button.selected)
			{
				/* Now not selected */
				button.selected = false;
				
				/* Draw it */
				bank.need_update = true;
			}
		}
	}
	
	return (false);
}

/*
 * sdl_Window functions
 */
static void sdl_WindowFree(sdl_Window* window)
{
	if (window.surface)
	{
		SDL_FreeSurface(window.surface);
		sdl_ButtonBankFree(&window.buttons);
		sdl_FontFree(&window.font);
		WIPE(window, sdl_Window);
	}
}

/*
 * Initialize a window
 */
static void sdl_WindowInit(sdl_Window* window, int w, int h, SDL_Surface *owner, const char *fontname)
{
	sdl_WindowFree(window);
	window.owner = owner;
	window.width = w;
	window.height = h;
	window.surface = SDL_CreateRGBSurface(SDL_SWSURFACE, w, h,
										   owner.format.BitsPerPixel,
										   owner.format.Rmask, owner.format.Gmask,
										   owner.format.Bmask, owner.format.Amask);
	sdl_ButtonBankInit(&window.buttons, window);
	sdl_FontCreate(&window.font, fontname, window.surface);
	window.visible = true;
	window.need_update = true;
}


static void sdl_WindowBlit(sdl_Window* window)
{
	SDL_Rect rc;
	
	if (!window.visible) return;
	
	RECT(window.left, window.top, window.width, window.height, &rc);
	
	SDL_BlitSurface(window.surface, null, window.owner, &rc);
	SDL_UpdateRects(window.owner, 1, &rc);
}

static void sdl_WindowText(sdl_Window* window, Uint32 c, int x, int y, const char *s)
{
	sdl_FontDraw(&window.font, window.surface, c, x, y, strlen(s), s);
}

static void sdl_WindowUpdate(sdl_Window* window)
{
	if ((window.need_update || window.buttons.need_update) && (window.visible))
	{
		SDL_Event Event;
		
		SDL_FillRect(window.surface, null, back_colour);
		
		if (window.draw_extra) (*window.draw_extra)(window);
		
		sdl_ButtonBankDrawAll(&window.buttons);
		
		window.need_update = false;
		
		WIPE(&Event, SDL_Event);
		
		Event.type = WINDOW_DRAW;
		
		Event.user.data1 = (void*)window;
		
		SDL_PushEvent(&Event);
	}
}



static void term_windowFree(term_window* win)
{
	if (win.surface)
	{
		SDL_FreeSurface(win.surface);
		win.surface = null;
#ifdef USE_GRAPHICS
		/* Invalidate the gfx surface */
		if (win.tiles)
		{
			SDL_FreeSurface(win.tiles);
			win.tiles = null;
		}
#endif
		term_nuke(&win.term_data);
	}
	
	sdl_FontFree(&win.font);
}

static errr save_prefs(void);
static void hook_quit(const char *str)
{
	int i;
	
	save_prefs();
	
	string_free(ANGBAND_DIR_USER_SDL);
	
	/* Free the surfaces of the windows */
	for (i = 0; i < ANGBAND_TERM_MAX; i++)
	{
		term_windowFree(&windows[i]);
		string_free(windows[i].req_font);
	}
	
#ifdef USE_GRAPHICS
	/* Free the graphics surface */
	if (GfxSurface) SDL_FreeSurface(GfxSurface);
#endif
	/* Free the 'System font' */
	sdl_FontFree(&SystemFont);
	
	/* Free the statusbar window */
	sdl_WindowFree(&StatusBar);
	
	/* free the popup window */
	sdl_WindowFree(&PopUp);
	
	/* Free the main surface */
	SDL_FreeSurface(AppWin);
	
	/* Shut down the font library */
	TTF_Quit(); 
	
	/* Shut down SDL */
	SDL_Quit();
	
	for (i = 0; i < MAX_FONTS; i++)
		string_free(FontList[i]);
}

static void BringToTop(void)
{
	int i, idx;
	
	for (idx = 0; idx < ANGBAND_TERM_MAX; idx++)
	{
		if (Zorder[idx] == SelectedTerm) break;
	}
	
	if (idx == ANGBAND_TERM_MAX) return;
	
	for (i = idx; i < ANGBAND_TERM_MAX - 1; i++)
	{
		Zorder[i] = Zorder[i + 1];
	}
	
	Zorder[ANGBAND_TERM_MAX - 1] = SelectedTerm;
}

	
	
/*
 * Validate a file
 */
static void validate_file(const char *s)
{
	if (!file_exists(s))
		quit_fmt("Cannot find required file:\n%s", s);
}

/*
 * Find a window that is under the points x,y on
 * the main screen
 */

static int sdl_LocateWin(int x, int y)
{
	int i;
	
	for (i = ANGBAND_TERM_MAX - 1; i >= 0; i--)
	{
		term_window *win = &windows[Zorder[i]];
		SDL_Rect rc;
		
		if (!win.visible) continue;
		if (!point_in(RECT(win.left, win.top, win.width, win.height, &rc), x, y)) continue;
		
		return (Zorder[i]);
	}
	
	return (-1);
}

static void draw_statusbar(sdl_Window *window)
{
	char buf[128];
	term_window *win = &windows[SelectedTerm];
	int fw = window.font.width;
	int x = 1;
	sdl_Button *button;
	
	SDL_Rect rc;
	
	u32b c = SDL_MapRGB(window.surface.format, 160, 160, 60);

	RECT(0, StatusBar.height - 1, StatusBar.width, 1, &rc);
	SDL_FillRect(StatusBar.surface, &rc, c);
	
	button = sdl_ButtonBankGet(&StatusBar.buttons, AboutSelect);
	x += button.pos.w + 20;
	
	sdl_WindowText(&StatusBar, c, x, 1, "Term:");
	x += 5 * fw;
	
	button = sdl_ButtonBankGet(&StatusBar.buttons, TermSelect);
	button.pos.x = x;
	x += button.pos.w + 10;
	
	my_strcpy(buf, format("(%dx%d)", win.cols, win.rows), sizeof(buf));
	sdl_WindowText(&StatusBar, c, x, 1, buf);
	x += strlen(buf) * fw + 20;
	
	sdl_WindowText(&StatusBar, c, x, 1, "Visible:");
	x += 8 * fw;
	
	button = sdl_ButtonBankGet(&StatusBar.buttons, VisibleSelect);
	button.pos.x = x;
	x += button.pos.w + 20;
	
	button = sdl_ButtonBankGet(&StatusBar.buttons, FontSelect);
	if (button.visible) sdl_WindowText(&StatusBar, c, x, 1, "Font:");
	x += 5 * fw;
	
	
	button.pos.x = x;
	x += button.pos.w + 20;
	
	button = sdl_ButtonBankGet(&StatusBar.buttons, MoreSelect);
	button.pos.x = x;
	
	x += button.pos.w + 20;
	
	
}



static void sdl_BlitWin(term_window *win)
{
	SDL_Rect rc;
	
	if (!win.surface) return;
	if (!win.visible) return;
	if (win.uRect.x == -1) return;
	
	/* Select the area to be updated */
	RECT(win.left + win.uRect.x, win.top + win.uRect.y, win.uRect.w, win.uRect.h, &rc);
	
	SDL_BlitSurface(win.surface, &win.uRect, AppWin, &rc);
	SDL_UpdateRects(AppWin, 1, &rc);
	
	/* Mark the update as complete */
	win.uRect.x = -1;
}

static void sdl_BlitAll(void)
{
	SDL_Rect rc;
	sdl_Window *window = &StatusBar;
	int i;
	Uint32 colour = SDL_MapRGB(AppWin.format, 160, 160, 60);
	/* int32 ccolour = SDL_MapRGB(AppWin.format, 160, 40, 40); */
	SDL_FillRect(AppWin, null, back_colour);
	
	for (i = 0; i < ANGBAND_TERM_MAX; i++)
	{
		term_window *win = &windows[Zorder[i]];
		
		if (!win.surface) continue;
		if (!win.visible) continue;
		
		RECT(win.left, win.top, win.width, win.height, &rc);
		SDL_BlitSurface(win.surface, null, AppWin, &rc);
		
		if (Zorder[i] == SelectedTerm)
		{
			SizingSpot.w = 10;
			SizingSpot.h = 10;
			SizingSpot.x = win.left + win.width - 10;
			SizingSpot.y = win.top + win.height - 10;
			/* SDL_FillRect(AppWin, &SizingSpot, ccolour); */
			
			if (Sizing)
			{
				int width = 2;
				int grabsize = 10;
				rc = SizingRect;
				SizingSpot.w = grabsize;
				SizingSpot.h = grabsize;
				SizingSpot.x = SizingRect.x + SizingRect.w - grabsize;
				SizingSpot.y = SizingRect.y + SizingRect.h - grabsize;
				sdl_DrawBox(AppWin, &rc, colour, width);
			}
		}
	}
	
	RECT(window.left, window.top, window.width, window.height, &rc);
	
	SDL_BlitSurface(window.surface, null, AppWin, &rc);
	
	SDL_UpdateRect(AppWin, 0, 0, AppWin.w, AppWin.h);
	
	
}

static void RemovePopUp(void)
{
	PopUp.visible = false;
	popped = false;
	sdl_BlitAll();
}

static void QuitActivate(sdl_Button *sender)
{
	SDL_Event Event;
	
	Event.type = SDL_QUIT;
	
	SDL_PushEvent(&Event);
}

static void SetStatusButtons(void)
{
	term_window *win = &windows[SelectedTerm];
	sdl_Button *button = sdl_ButtonBankGet(&StatusBar.buttons, TermSelect);
	sdl_Button *fontbutton = sdl_ButtonBankGet(&StatusBar.buttons, FontSelect);
	sdl_Button *visbutton = sdl_ButtonBankGet(&StatusBar.buttons, VisibleSelect);
	
	sdl_ButtonCaption(button, angband_term_name[SelectedTerm]);
	
	if (!win.visible)
	{
		sdl_ButtonVisible(fontbutton, false);
		sdl_ButtonCaption(visbutton, "No");
	}
	else
	{
		sdl_ButtonVisible(fontbutton, true);
		sdl_ButtonCaption(fontbutton, win.font.name);
		sdl_ButtonCaption(visbutton, "Yes");
	}
}
	
static void TermFocus(int idx)
{
	if (SelectedTerm == idx) return;
	
	SelectedTerm = idx;
	
	BringToTop();
	
	SetStatusButtons();
	
	sdl_BlitAll();
}	

static void AboutDraw(sdl_Window *win)
{
	SDL_Rect rc;
	SDL_Rect icon;
	
	/* Wow - a different colour! */
	Uint32 colour = SDL_MapRGB(win.surface.format, 160, 60, 60);
	
	RECT(0, 0, win.width, win.height, &rc);
	
	/* Draw a nice box */
	SDL_FillRect(win.surface, &win.surface.clip_rect, SDL_MapRGB(win.surface.format, 255,255,255));
	sdl_DrawBox(win.surface, &win.surface.clip_rect, colour, 5);
	if (mratt) {
		RECT((win.width - mratt.w) / 2, 5, mratt.w, mratt.h, &icon);
		SDL_BlitSurface(mratt, null, win.surface, &icon);
	}
	sdl_WindowText(win, colour, 20, 150, format("You are playing %s", buildid));
	sdl_WindowText(win, colour, 20, 160, "See http://www.rephial.org");
}

static void AboutActivate(sdl_Button *sender)
{
	int width = 350;
	int height = 200;
	
	sdl_WindowInit(&PopUp, width, height, AppWin, StatusBar.font.name);
	PopUp.left = (AppWin.w / 2) - width / 2;
	PopUp.top = (AppWin.h / 2) - height / 2;
	PopUp.draw_extra = AboutDraw;

	popped = true;
}

static void SelectTerm(sdl_Button *sender)
{
	RemovePopUp();
	
	TermFocus(sender.tag);
}
	
static void TermActivate(sdl_Button *sender)
{
	int i, maxl = 0; 
	int width, height = ANGBAND_TERM_MAX * (StatusBar.font.height + 1);

	for (i = 0; i < ANGBAND_TERM_MAX; i++)
	{
		int l = strlen(angband_term_name[i]); 
		if (l > maxl) maxl = l;
	}
	
	width = maxl * StatusBar.font.width + 20;
	
	sdl_WindowInit(&PopUp, width, height, AppWin, StatusBar.font.name);
	PopUp.left = sender.pos.x;
	PopUp.top = sender.pos.y;
	
	for (i = 0; i < ANGBAND_TERM_MAX; i++)
	{
		int h = PopUp.font.height;
		int b = sdl_ButtonBankNew(&PopUp.buttons);
		sdl_Button *button = sdl_ButtonBankGet(&PopUp.buttons, b);
		sdl_ButtonSize(button, width - 2 , h);
		sdl_ButtonMove(button, 1, i * (h + 1));
		sdl_ButtonCaption(button, angband_term_name[i]);
		sdl_ButtonVisible(button, true);
		button.tag = i;
		button.activate = SelectTerm;
	}
	popped = true;
}

static void ResizeWin(term_window* win, int w, int h);
static void term_data_link_sdl(term_window *win);

static void VisibleActivate(sdl_Button *sender)
{
	term_window *window = &windows[SelectedTerm];
	
	if (SelectedTerm == 0) return;
	
	if (window.visible)
	{
		window.visible = false;
		term_windowFree(window);
		angband_term[SelectedTerm] = null;
		
	}
	else
	{
		window.visible = true;
		ResizeWin(window, window.width, window.height);
		
	}
	
	SetStatusButtons();
	sdl_BlitAll();
}

static void SelectFont(sdl_Button *sender)
{
	term_window *window = &windows[SelectedTerm];
	int w, h;
	
	sdl_FontFree(&window.font);
	string_free(window.req_font);

	window.req_font = string_make(sender.caption);
	
	sdl_CheckFont(window.req_font, &w, &h);
#ifdef USE_GRAPHICS	
	/* Invalidate the gfx surface */
	if (window.tiles)
	{
		SDL_FreeSurface(window.tiles);
		window.tiles = null;
	}
#endif
	
	ResizeWin(window, (w * window.cols) + (2 * window.border),
			  (h * window.rows) + window.border + window.title_height);
	
	SetStatusButtons();
	
	RemovePopUp();
	
}


static void FontActivate(sdl_Button *sender)
{
	int i, maxl = 0; 
	int width, height = num_fonts * (StatusBar.font.height + 1);

	for (i = 0; i < num_fonts; i++)
	{
		int l = strlen(FontList[i]); 
		if (l > maxl) maxl = l;
	}
	
	width = maxl * StatusBar.font.width + 20;
	
	sdl_WindowInit(&PopUp, width, height, AppWin, StatusBar.font.name);
	PopUp.left = sender.pos.x;
	PopUp.top = sender.pos.y;
	
	for (i = 0; i < num_fonts; i++)
	{
		int h = PopUp.font.height;
		int b = sdl_ButtonBankNew(&PopUp.buttons);
		sdl_Button *button = sdl_ButtonBankGet(&PopUp.buttons, b);
		sdl_ButtonSize(button, width - 2 , h);
		sdl_ButtonMove(button, 1, i * (h + 1));
		sdl_ButtonCaption(button, FontList[i]);
		sdl_ButtonVisible(button, true);
		button.activate = SelectFont;
	}
	popped = true;
}

#ifdef USE_GRAPHICS
static errr load_gfx(void);
static bool do_update = false;

static void SelectGfx(sdl_Button *sender)
{
	SelectedGfx = sender.tag;
}
#endif

static void AcceptChanges(sdl_Button *sender)
{
	sdl_Button *button;
	bool do_video_reset = false;
	
#ifdef USE_GRAPHICS
	if (use_graphics != SelectedGfx)
	{
		do_update = true;
		use_graphics = SelectedGfx;
	}
	
	if (use_graphics)
	{
		arg_graphics = true;
		load_gfx();
	}
	else
	{
		arg_graphics = false;
		tile_width = 1;
		tile_height = 1;
		reset_visuals(true);
	}

	/* Invalidate all the gfx surfaces */
	if (do_update)
	{
		int i;
		for (i = 0; i < ANGBAND_TERM_MAX; i++)
		{
			term_window *win = &windows[i];
			if (win.tiles)
			{
				SDL_FreeSurface(win.tiles);
				win.tiles = null;
			}
		}
	}	
	
#endif
	
	button = sdl_ButtonBankGet(&PopUp.buttons, MoreFullscreen);
	
	if (button.tag != fullscreen)
	{
		fullscreen = !fullscreen;
		
		do_video_reset = true;
	}
	
	
	SetStatusButtons();
	
	RemovePopUp();
	
	if (do_update)
	{
		if (character_dungeon) do_cmd_redraw();
	}
	
	if (do_video_reset)
	{
		SDL_Event Event;
		
		WIPE(&Event, SDL_Event);
		
		Event.type = SDL_VIDEORESIZE;
		Event.resize.w = screen_w;
		Event.resize.h = screen_h;
		
		SDL_PushEvent(&Event);
	}

	do_update = false;
	
}

static void FlipTag(sdl_Button *sender)
{
	if (sender.tag)
	{
		sender.tag = 0;
		sdl_ButtonCaption(sender, "Off");
	}
	else
	{
		sender.tag = 1;
		sdl_ButtonCaption(sender, "On");
	}
}

static void SnapChange(sdl_Button *sender)
{
	SnapRange += sender.tag;
	if (SnapRange < 0) SnapRange = 0;
	if (SnapRange > 20) SnapRange = 20;
	PopUp.need_update = true;
}

#ifdef USE_GRAPHICS
static void WidthChange(sdl_Button *sender)
{
	tile_width += sender.tag;
	if (tile_width < 1) tile_width = 1;
	if (tile_width > 6) tile_width = 6;
	do_update = true;
}

static void HeightChange(sdl_Button *sender)
{
	tile_height += sender.tag;
	if (tile_height < 1) tile_height = 1;
	if (tile_height > 3) tile_height = 3;
	do_update = true;
}
#endif

static void MoreDraw(sdl_Window *win)
{
	SDL_Rect rc;
	sdl_Button *button;
	int y = 20, i;
	
	/* Wow - a different colour! */
	Uint32 colour = SDL_MapRGB(win.surface.format, 160, 60, 60);
	
	RECT(0, 0, win.width, win.height, &rc);
	
	/* Draw a nice box */
	sdl_DrawBox(win.surface, &rc, colour, 5);
	
#ifdef USE_GRAPHICS
	
	button = sdl_ButtonBankGet(&win.buttons, MoreWidthMinus);
	if (SelectedGfx) {
		sdl_WindowText(win, colour, 20, y, format("Tile width is %d.", tile_width));
		sdl_ButtonMove(button, 200, y);
		sdl_ButtonVisible(button, true);
	} else {
		sdl_ButtonVisible(button, false);
	}

	button = sdl_ButtonBankGet(&win.buttons, MoreWidthPlus);
	if (SelectedGfx) {
		sdl_ButtonMove(button, 230, y);
		sdl_ButtonVisible(button, true);
		y += 20;
	} else {
		sdl_ButtonVisible(button, false);
	}

	button = sdl_ButtonBankGet(&win.buttons, MoreHeightMinus);
	if (SelectedGfx) {
		sdl_WindowText(win, colour, 20, y, format("Tile height is %d.", tile_height));
		sdl_ButtonMove(button, 200, y);
		sdl_ButtonVisible(button, true);
	} else {
		sdl_ButtonVisible(button, false);
	}

	button = sdl_ButtonBankGet(&win.buttons, MoreHeightPlus);
	if (SelectedGfx) {
		sdl_ButtonMove(button, 230, y);
		sdl_ButtonVisible(button, true);
		y += 20;
	} else {
		sdl_ButtonVisible(button, false);
	}

	sdl_WindowText(win, colour, 20, y, "Selected Graphics:");
	sdl_WindowText(win, SDL_MapRGB(win.surface.format, 210, 110, 110),
				   200, y, GfxDesc[SelectedGfx].name);

	y += 20;

	sdl_WindowText(win, colour, 20, y, "Available Graphics:");
	
	for (i = 0; i < GfxModes; i++)
	{
		if (!GfxDesc[i].avail) continue;
		button = sdl_ButtonBankGet(&win.buttons, GfxButtons[i]);
		sdl_ButtonMove(button, 200, y);
		y += 20;
	}
#endif	

	button = sdl_ButtonBankGet(&win.buttons, MoreFullscreen);
	sdl_WindowText(win, colour, 20, y, "Fullscreen is:");

	sdl_ButtonMove(button, 200, y);
	y+= 20;

	sdl_WindowText(win, colour, 20, y, format("Snap range is %d.", SnapRange));
	button = sdl_ButtonBankGet(&win.buttons, MoreSnapMinus);
	sdl_ButtonMove(button, 200, y);

	button = sdl_ButtonBankGet(&win.buttons, MoreSnapPlus);
	sdl_ButtonMove(button, 230, y);
}

static void MoreActivate(sdl_Button *sender)
{
	int width = 300;
	int height = 300;
	int i;
	sdl_Button *button;
	
	Uint32 ucolour;
	Uint32 scolour;
	
	sdl_WindowInit(&PopUp, width, height, AppWin, StatusBar.font.name);
	PopUp.left = (AppWin.w / 2) - width / 2;
	PopUp.top = (AppWin.h / 2) - height / 2;
	PopUp.draw_extra = MoreDraw;
	
	ucolour = SDL_MapRGB(PopUp.surface.format, 160, 60, 60);
	scolour = SDL_MapRGB(PopUp.surface.format, 210, 110, 110);
	
#ifdef USE_GRAPHICS
	MoreWidthPlus = sdl_ButtonBankNew(&PopUp.buttons);
	button = sdl_ButtonBankGet(&PopUp.buttons, MoreWidthPlus);
	
	button.unsel_colour = ucolour;
	button.sel_colour = scolour;
	sdl_ButtonSize(button, 20, PopUp.font.height + 2);
	sdl_ButtonCaption(button, "+");
	button.tag = 1;
	sdl_ButtonVisible(button, true);
	button.activate = WidthChange;
	
	MoreWidthMinus = sdl_ButtonBankNew(&PopUp.buttons);
	button = sdl_ButtonBankGet(&PopUp.buttons, MoreWidthMinus);
	
	button.unsel_colour = ucolour;
	button.sel_colour = scolour;
	sdl_ButtonSize(button, 20, PopUp.font.height + 2);
	sdl_ButtonCaption(button, "-");
	button.tag = -1;
	sdl_ButtonVisible(button, true);
	button.activate = WidthChange;
	
	MoreHeightPlus = sdl_ButtonBankNew(&PopUp.buttons);
	button = sdl_ButtonBankGet(&PopUp.buttons, MoreHeightPlus);
	
	button.unsel_colour = ucolour;
	button.sel_colour = scolour;
	sdl_ButtonSize(button, 20, PopUp.font.height + 2);
	sdl_ButtonCaption(button, "+");
	button.tag = 1;
	sdl_ButtonVisible(button, true);
	button.activate = HeightChange;
	
	MoreHeightMinus = sdl_ButtonBankNew(&PopUp.buttons);
	button = sdl_ButtonBankGet(&PopUp.buttons, MoreHeightMinus);
	
	button.unsel_colour = ucolour;
	button.sel_colour = scolour;
	sdl_ButtonSize(button, 20, PopUp.font.height + 2);
	sdl_ButtonCaption(button, "-");
	button.tag = -1;
	sdl_ButtonVisible(button, true);
	button.activate = HeightChange;
	
	SelectedGfx = use_graphics;
	
	for (i = 0; i < GfxModes; i++)
	{
		if (!GfxDesc[i].avail) continue;
		GfxButtons[i] = sdl_ButtonBankNew(&PopUp.buttons);
		button = sdl_ButtonBankGet(&PopUp.buttons, GfxButtons[i]);
		
		button.unsel_colour = ucolour;
		button.sel_colour = scolour;
		sdl_ButtonSize(button, 50 , PopUp.font.height + 2);
		sdl_ButtonVisible(button, true);
		sdl_ButtonCaption(button, GfxDesc[i].name);
		button.tag = i;
		button.activate = SelectGfx;
	}
#endif
	MoreFullscreen = sdl_ButtonBankNew(&PopUp.buttons);
	button = sdl_ButtonBankGet(&PopUp.buttons, MoreFullscreen);
	
	button.unsel_colour = ucolour;
	button.sel_colour = scolour;
	sdl_ButtonSize(button, 50 , PopUp.font.height + 2);
	sdl_ButtonVisible(button, true);
	sdl_ButtonCaption(button, fullscreen ? "On" : "Off");
	button.tag = fullscreen;
	button.activate = FlipTag;
	
	MoreSnapPlus = sdl_ButtonBankNew(&PopUp.buttons);
	button = sdl_ButtonBankGet(&PopUp.buttons, MoreSnapPlus);
	
	button.unsel_colour = ucolour;
	button.sel_colour = scolour;
	sdl_ButtonSize(button, 20, PopUp.font.height + 2);
	sdl_ButtonCaption(button, "+");
	button.tag = 1;
	sdl_ButtonVisible(button, true);
	button.activate = SnapChange;
	
	MoreSnapMinus = sdl_ButtonBankNew(&PopUp.buttons);
	button = sdl_ButtonBankGet(&PopUp.buttons, MoreSnapMinus);
	
	button.unsel_colour = ucolour;
	button.sel_colour = scolour;
	sdl_ButtonSize(button, 20, PopUp.font.height + 2);
	sdl_ButtonCaption(button, "-");
	button.tag = -1;
	sdl_ButtonVisible(button, true);
	button.activate = SnapChange;
	
	MoreOK = sdl_ButtonBankNew(&PopUp.buttons);
	button = sdl_ButtonBankGet(&PopUp.buttons, MoreOK);
	
	button.unsel_colour = ucolour;
	button.sel_colour = scolour;
	sdl_ButtonSize(button, 50 , PopUp.font.height + 2);
	sdl_ButtonVisible(button, true);
	sdl_ButtonCaption(button, "OK");
	sdl_ButtonMove(button, width / 2 - 25, height - 40);
	button.activate = AcceptChanges;
	
	popped = true;
}

static errr Term_xtra_sdl_clear(void);

/*
 * Make a window with size (x,y) pixels
 * Note: The actual size of the window may end up smaller.
 * This may be called when a window wants resizing,
 * is made visible, or the font has changed.
 * This function doesn't go in for heavy optimization, and doesn't need it-
 * it may initialize a few too many redraws or whatnot, but everything gets done!
 */
static void ResizeWin(term_window* win, int w, int h)
{
	/* Don't bother */
	if (!win.visible) return;
	
	win.border = 2;
	win.title_height = StatusHeight;
	
	/* No font - a new font is needed . get dimensions */
	if (!win.font.data)
	{
		/* Get font dimensions */
		sdl_CheckFont(win.req_font, &win.tile_wid, &win.tile_hgt);
		
		/* Oops */
		if (!win.tile_wid || !win.tile_hgt)
			quit(format("Unable to find font '%s'.\n"
						"Note that there are new extended font files ending in 'x' in %s.\n"
					    "Please check %s and edit if necessary.",
					    win.req_font, ANGBAND_DIR_XTRA_FONT, ANGBAND_DIR_USER_SDL));
	}
	
	/* Get the amount of columns & rows */
	win.cols = (w - (win.border * 2)) / win.tile_wid;
	win.rows = (h - win.border - win.title_height) / win.tile_hgt;
	
	/* Calculate the width & height */
	win.width = (win.cols * win.tile_wid) + (win.border * 2);
	win.height = (win.rows * win.tile_hgt) + win.border + win.title_height;
	
	/* Delete the old surface */
	if (win.surface) SDL_FreeSurface(win.surface);
	
	/* Create a new surface */
	win.surface = SDL_CreateRGBSurface(SDL_SWSURFACE, win.width, win.height,
										   AppWin.format.BitsPerPixel,
										   AppWin.format.Rmask, AppWin.format.Gmask,
										   AppWin.format.Bmask, AppWin.format.Amask);
	
	/* Fill it */
	SDL_FillRect(win.surface, null, SDL_MapRGB(AppWin.format, 160, 160, 60));
	
	/* Label it */
	sdl_FontDraw(&SystemFont, win.surface, back_colour, 1, 1,
				 strlen(angband_term_name[win.Term_idx]), angband_term_name[win.Term_idx]);
	
	/* Mark the whole window for redraw */
	RECT(0, 0, win.width, win.height, &win.uRect);
	
	/* Create the font if we need to */
	if (!win.font.data)
	{
		sdl_FontCreate(&win.font, win.req_font, win.surface);
	}
	
	/* This window was never visible before */
	if (!angband_term[win.Term_idx])
	{
		term *old = Term;
		
		/* Initialize the term data */
		term_data_link_sdl(win);
		
		/* Make it visible to angband */
		angband_term[win.Term_idx] = &win.term_data;
		
		/* Activate it */
		Term_activate((term*)&win.term_data);
		
		/* Redraw */
		Term_redraw();
		
		/* Restore */
		Term_activate(old);
	}
	/* Resize the term */
	else
	{
		term *old = Term;
		
		/* Activate it */
		Term_activate((term*)&win.term_data);
		
		/* Resize */
		Term_resize(win.cols, win.rows);
		
		/* Redraw */
		Term_redraw();
		
		/* Restore */
		Term_activate(old);
	}
	
	/* Calculate the hotspot */
	if (win.Term_idx == SelectedTerm)
	{
		SizingSpot.w = 10;
		SizingSpot.h = 10;
		SizingSpot.x = win.left + win.width - 10;
		SizingSpot.y = win.top + win.height - 10;
	}
	
	StatusBar.need_update = true;
	
	/* HACK - Redraw all windows */
	if (character_dungeon) do_cmd_redraw();
}


static errr load_prefs(void)
{
	char buf[1024];
	ang_file *fff;
	term_window *win;
	int i;
	
	/* Initialize the windows with crappy defaults! */
	for (i = 0; i < ANGBAND_TERM_MAX; i++)
	{
		win = &windows[i];
		
		/* Clear the data */
		WIPE(win, term_window);
		
		/* Who? */
		win.Term_idx = i;
		
		win.req_font = string_make(DEFAULT_FONT_FILE);
		
		if (i == 0)
		{
			win.top = StatusHeight;
			win.width = 600;
			win.height = 380;
			win.keys = 1024;
			win.visible = true;
		}
		else
		{
			win.top = 400 + (i * 10);
			win.left = (i - 1) * 10;
			win.width = 400;
			win.height = 200;
			win.keys = 32;
			
			win.visible = false;
		}
	}
	
	/* Build the path */
	path_build(buf, sizeof(buf), ANGBAND_DIR_USER, "sdlinit.txt");
	
	/* XXXXX */
	ANGBAND_DIR_USER_SDL = string_make(buf);
	
	/* Open the file */
	fff = file_open(buf, MODE_READ, -1);
	
	/* Check it */
	if (!fff) return (1);
	
	/* Process the file */
	while (file_getl(fff, buf, sizeof(buf)))
	{
		char *s;
		if (!buf[0]) continue;
		
		if (buf[0] == '#') continue;
		
		s = strchr(buf, '=');
		s++;
		while (!isalnum(*s)) s++;
		
		if (strstr(buf, "Resolution"))
		{
			screen_w = atoi(s);
			s = strchr(buf, 'x');
			screen_h = atoi(s + 1);
		}
		else if (strstr(buf, "Fullscreen"))
		{
			fullscreen = atoi(s);
		}
		else if (strstr(buf, "Graphics"))
		{
			use_graphics = atoi(s);
			if (use_graphics) arg_graphics = true;
		}
		else if (strstr(buf, "TileWidth"))
		{
			tile_width = atoi(s);
		}
		else if (strstr(buf, "TileHeight"))
		{
			tile_height = atoi(s);
		}
		else if (strstr(buf, "Bigtile"))
		{
			tile_width += atoi(s);
		}
                else if (strstr(buf, "Dbltile"))
                {
			tile_width += tile_width * atoi(s);
			tile_height += tile_height * atoi(s);
                }
                else if (strstr(buf, "Trptile"))
                {
			tile_width += 2 * tile_width * atoi(s);
			tile_height += 2 * tile_height * atoi(s);
                }
		else if (strstr(buf, "Window"))
		{
			win = &windows[atoi(s)];
		}
		else if (strstr(buf, "Visible"))
		{
			win.visible = atoi(s);
		}
		else if (strstr(buf, "Left"))
		{
			win.left = atoi(s);
		}
		else if (strstr(buf, "Top"))
		{
			win.top = atoi(s);
		}
		else if (strstr(buf, "Width"))
		{
			win.width = atoi(s);
		}
		else if (strstr(buf, "Height"))
		{
			win.height = atoi(s);
		}
		else if (strstr(buf, "Keys"))
		{
			win.keys = atoi(s);
		}
		else if (strstr(buf, "Font"))
		{
			win.req_font = string_make(s);
		}
		
		
	}
	
	if (screen_w < 640) screen_w = 640;
	if (screen_h < 480) screen_h = 480;
	
	file_close(fff);
	
	return (0);
}

static errr save_prefs(void)
{
	ang_file *fff;
	int i;

	/* Open the file */
	fff = file_open(ANGBAND_DIR_USER_SDL, MODE_WRITE, FTYPE_TEXT);

	/* Check it */
	if (!fff) return (1);

	file_putf(fff, "Resolution = %dx%d\n", screen_w, screen_h);
	file_putf(fff, "Fullscreen = %d\n", fullscreen);
	file_putf(fff, "Graphics = %d\n", use_graphics);
	file_putf(fff, "TileWidth = %d\n\n", tile_width);
        file_putf(fff, "TileHeight = %d\n\n", tile_height);
	
	for (i = 0; i < ANGBAND_TERM_MAX; i++)
	{
		term_window *win = &windows[i];
		
		file_putf(fff, "Window = %d\n", i);
		file_putf(fff, "Visible = %d\n", (int)win.visible);
		file_putf(fff, "Left = %d\n", win.left);
		file_putf(fff, "Top = %d\n", win.top);
		file_putf(fff, "Width = %d\n", win.width);
		file_putf(fff, "Height = %d\n", win.height);
		
		file_putf(fff, "Keys = %d\n", win.keys);
		
		file_putf(fff, "Font = %s\n\n", win.req_font);
	}	
	
	file_close(fff);
	
	/* Done */
	return (0);
}

static void set_update_rect(term_window *win, SDL_Rect *rc);

static void DrawSizeWidget(void)
{
	Uint32 colour = SDL_MapRGB(AppWin.format, 30, 160, 70);
	SDL_FillRect(AppWin, &SizingSpot, colour);
	SDL_UpdateRects(AppWin, 1, &SizingSpot);
}

static int Movingx;
static int Movingy;

/*
 * Is What within Range units of Origin
 */

#define closeto(Origin, What, Range) \
	((ABS((Origin) - (What))) < (Range))

/*
 * This function keeps the 'mouse' info up to date,
 * and reacts to mouse buttons appropriately.
 */
static void sdl_HandleMouseEvent(SDL_Event *event)
{
	term *old = Term;
	term_window *win;
	switch (event.type)
	{
		/* Mouse moved */
		case SDL_MOUSEMOTION:
		{
			mouse.x = event.motion.x;
			mouse.y = event.motion.y;
			win = &windows[SelectedTerm];
			
			/* We are moving a window */
			if (Moving)
			{
				int i;
				
				/* Move the window */
				win.left = (mouse.x - Movingx);
				win.top = (mouse.y - Movingy);
				
				/* Left bounds check */
				if (win.left < 0)
				{
					win.left = 0;
					Movingx = mouse.x;
				}
					
				/* Right bounds check */
				if ((win.left + win.width) > AppWin.w)
				{
					win.left = AppWin.w - win.width;
					Movingx = mouse.x - win.left;
				}
				
				/* Top bounds check */
				if (win.top < StatusHeight)
				{
					win.top = StatusHeight;
					Movingy = mouse.y - win.top;
				}
				
				/* Bottom bounds check */
				if ((win.top + win.height) > AppWin.h)
				{
					win.top = AppWin.h - win.height;
					Movingy = mouse.y - win.top;
				}
				
				for (i = 0; i < ANGBAND_TERM_MAX; i++)
				{
					term_window *snapper = &windows[i];
					
					/* Can't snap to self... */
					if (i == SelectedTerm) continue;
					
					/* Can't snap to the invisible */
					if (!snapper.visible) continue;
					
					/* Check the windows are across from each other */
					if ((snapper.top < win.top + win.height) && (win.top < snapper.top + snapper.height))
					{
						/* Lets try to the left... */
						if (closeto(win.left, snapper.left + snapper.width, SnapRange))
						{
							win.left = snapper.left + snapper.width;
							Movingx = mouse.x - win.left;
						}
						/* Maybe to the right */
						if (closeto(win.left + win.width, snapper.left, SnapRange))
						{
							win.left = snapper.left - win.width;
							Movingx = mouse.x - win.left;
						}
					}
					
					/* Check the windows are above/below each other */
					if ((snapper.left < win.left + win.width) && (win.left < snapper.left + snapper.width))
					{
						/* Lets try to the top... */
						if (closeto(win.top, snapper.top + snapper.height, SnapRange))
						{
							win.top = snapper.top + snapper.height;
							Movingy = mouse.y - win.top;
						}
						/* Maybe to the bottom */
						if (closeto(win.top + win.height, snapper.top, SnapRange))
						{
							win.top = snapper.top - win.height;
							Movingy = mouse.y - win.top;
						}
					}
					
				}
				
				/* Show on the screen */
				sdl_BlitAll();
			}
			
			/* A window is being re-sized */
			else if (Sizing)
			{
				/* Adjust the sizing rectangle */
				SizingRect.w = win.width - win.left + (mouse.x - Movingx);
				SizingRect.h = win.height - win.top + (mouse.y - Movingy);
				
				/* XXX - The main window can't be too small */
				if (SelectedTerm == 0)
				{
					int minwidth = (win.tile_wid * 80) + 2 * win.border;
					int minheight = (win.tile_hgt * 24) + win.border + win.title_height;
					if (SizingRect.w < minwidth)
						SizingRect.w = minwidth;
					if (SizingRect.h < minheight)
						SizingRect.h = minheight;
				}
				
				/* Show on the screen */				
				sdl_BlitAll();
			}
			else if (!popped)
			{
				/* Have a look for the corner stuff */
				if (point_in(&SizingSpot, mouse.x, mouse.y))
				{
					if (!Sizingshow)
					{
						/* Indicate the hotspot */
						Sizingshow = true;
						DrawSizeWidget();
					}
				}
				else if (Sizingshow)
				{
					SDL_Rect rc;
					Sizingshow = false;
					RECT(win.width - 10, win.height - 10, 10, 10, &rc);
					set_update_rect(win, &rc);
					sdl_BlitWin(win);
				}
			}
			break;
		}
			
		/* A button has been pressed */
		case SDL_MOUSEBUTTONDOWN:
		{
			
			sdl_Window *window;
			bool res;
			int idx = sdl_LocateWin(mouse.x, mouse.y);
			
			if (event.button.button == SDL_BUTTON_LEFT)
			{
				bool just_gained_focus = false;
				mouse.left = 1;
				mouse.leftx = event.button.x;
				mouse.lefty = event.button.y;
				
				/* Pop up window gets priority */
				if (popped) window = &PopUp; else window = &StatusBar;
				
				/* React to a button press */
				res = sdl_ButtonBankMouseDown(&window.buttons, mouse.x - window.left, mouse.y - window.top);
				
				/* If pop-up window active and no reaction then cancel the popup */
				if (popped && !res)
				{
					RemovePopUp();
					break;
				}
				
				/* Has this mouse press been handled */
				if (res) break;
				
				/* Is the mouse press in a term_window? */
				if (idx < 0) break;
				
				/* The 'focused' window has changed */
				if (idx != SelectedTerm)
				{
					TermFocus(idx);
					just_gained_focus = true;
				}
	
				/* A button press has happened on the focused term window */
				win = &windows[idx];
				
				/* Check for mouse press in the title bar */
				if (mouse.y < win.top + win.title_height)
				{
					/* Let's get moving */
					Moving = true;
					
					/* BringToTop(idx); */
					
					/* Remember where we started */
					Movingx = mouse.x - win.left;
					Movingy = mouse.y - win.top;
				}
				
				/* Check for the little hotspot in the botton right corner */
				else if (point_in(&SizingSpot, mouse.x, mouse.y))
				{
					/* Let's get sizing */
					Sizing = true;
					
					/* Create the sizing rectangle */
					RECT(win.left, win.top, win.width, win.height, &SizingRect);
					
					/* Remember where we started */
					Movingx = mouse.x - win.left;
					Movingy = mouse.y - win.top;
					
				}
				
				/* Signal a mouse press to angband (only if the window is already focused) */
				else if (!just_gained_focus)
				{
					if (win.visible)
					{
						/* Calculate the 'cell' coords */
						int x = (mouse.x - win.left - win.border) / win.tile_wid;
						int y = (mouse.y - win.top - win.title_height) / win.tile_hgt;
						
						/* Send the mousepress to the appropriate term */
						Term_activate(angband_term[idx]);
						Term_mousepress(x, y, 1);
						Term_activate(old);
					}
				}
			}
			
			/* Process right mouse button */
			else if (event.button.button == SDL_BUTTON_RIGHT)
			{
				mouse.right = 1;
				mouse.rightx = event.button.x;
				mouse.righty = event.button.y;
				
				/* Right-click always cancels the popup */
				if (popped)
				{
					popped = false;
				}
				/* See if it's inside a term_window */
				else if (idx != -1)
				{
					int x, y;
					win = &windows[idx];
					
					/* Calculate the 'cell' coords */
					x = (mouse.x - win.left - win.border) / win.tile_wid;
					y = (mouse.y - win.top - win.title_height) / win.tile_hgt;
					
					/* Bounds check */
					if ((x >= 0) && (y >= 0) && (x < win.cols) && (y < win.rows))
					{
						/* Send the mousepress to the appropriate term */
						Term_activate(angband_term[idx]);
						Term_mousepress(x, y, 2);
						Term_activate(old);
					}
				}
			}
			
			break;
		}
		case SDL_MOUSEBUTTONUP:
		{
			/* Handle release of left button */
			if (event.button.button == SDL_BUTTON_LEFT)
			{
				sdl_Window *window;
				bool res;
				mouse.left = 0;
				
				/* Pop up window gets priority */
				if (popped) window = &PopUp; else window = &StatusBar;
				
				/* React to a button release */
				res = sdl_ButtonBankMouseUp(&window.buttons, mouse.x - window.left, mouse.y - window.top);
				
				/* Cancel popup */
				if (popped && !res)
				{
					RemovePopUp();
				}
				
				/* Finish moving */
				if (Moving)
				{
					Moving = false;
					
					/* Update */
					sdl_BlitAll();
				}
				
				/* Finish sizing */
				if (Sizing)
				{
					/* Sort out the window */
					ResizeWin(&windows[SelectedTerm], SizingRect.w, SizingRect.h);
					Sizing = false;
					
					/* Update */
					sdl_BlitAll();
				}
			}
			else if (event.button.button == SDL_BUTTON_RIGHT)
			{
				mouse.right = 0;
			}
			break;
		}	
			
	}
}

/*
 * Handle keypresses.
 *
 * We treat left and right modifier keys as equivalent.
 * We ignore any key without a valid SDLK index.
 */
static void sdl_keypress(SDL_keysym keysym)
{
	u16b key_code = keysym.unicode;
	SDLKey key_sym = keysym.sym;

	int ch = 0;

	/* Store the value of various modifier keys */
	bool mc = (keysym.mod & KMOD_CTRL) > 0;
	bool ms = (keysym.mod & KMOD_SHIFT) > 0;
	bool ma = (keysym.mod & KMOD_ALT) > 0;
	bool mm = (keysym.mod & KMOD_META) > 0;
	bool kp = false;

	byte mods = (ma ? KC_MOD_ALT : 0) | (mm ? KC_MOD_META : 0);

	/* Ignore if main term is not initialized */
	if (!Term) return;

	/* Handle all other valid SDL keys */
	switch (key_sym) {
		/* keypad */
		case SDLK_KP0: ch = '0'; kp = true; break;
		case SDLK_KP1: ch = '1'; kp = true; break;
		case SDLK_KP2: ch = '2'; kp = true; break;
		case SDLK_KP3: ch = '3'; kp = true; break;
		case SDLK_KP4: ch = '4'; kp = true; break;
		case SDLK_KP5: ch = '5'; kp = true; break;
		case SDLK_KP6: ch = '6'; kp = true; break;
		case SDLK_KP7: ch = '7'; kp = true; break;
		case SDLK_KP8: ch = '8'; kp = true; break;
		case SDLK_KP9: ch = '9'; kp = true; break;
		case SDLK_KP_PERIOD: ch = '.'; kp = true; break;
		case SDLK_KP_DIVIDE: ch = '/'; kp = true; break;
		case SDLK_KP_MULTIPLY: ch = '*'; kp = true; break;
		case SDLK_KP_MINUS: ch = '-'; kp = true; break;
		case SDLK_KP_PLUS: ch = '+'; kp = true; break;
		case SDLK_KP_ENTER: ch = '\n'; kp = true; break;
		case SDLK_KP_EQUALS: ch = '='; kp = true; break;

		/* have have these to get consistent ctrl-shift behaviour */
		case SDLK_0: if (!ms || mc || ma) ch = '0'; break;
		case SDLK_1: if (!ms || mc || ma) ch = '1'; break;
		case SDLK_2: if (!ms || mc || ma) ch = '2'; break;
		case SDLK_3: if (!ms || mc || ma) ch = '3'; break;
		case SDLK_4: if (!ms || mc || ma) ch = '4'; break;
		case SDLK_5: if (!ms || mc || ma) ch = '5'; break;
		case SDLK_6: if (!ms || mc || ma) ch = '6'; break;
		case SDLK_7: if (!ms || mc || ma) ch = '7'; break;
		case SDLK_8: if (!ms || mc || ma) ch = '8'; break;
		case SDLK_9: if (!ms || mc || ma) ch = '9'; break;

		case SDLK_UP: ch = ARROW_UP; break;
		case SDLK_DOWN: ch = ARROW_DOWN; break;
		case SDLK_RIGHT: ch = ARROW_RIGHT; break;
		case SDLK_LEFT: ch = ARROW_LEFT; break;

		case SDLK_INSERT: ch = KC_INSERT; break;
		case SDLK_HOME: ch = KC_HOME; break;
		case SDLK_PAGEUP: ch = KC_PGUP; break;
		case SDLK_DELETE: ch = KC_DELETE; break;
		case SDLK_END: ch = KC_END; break;
		case SDLK_PAGEDOWN: ch = KC_PGDOWN; break;

		case SDLK_F1: ch = KC_F1; break;
		case SDLK_F2: ch = KC_F2; break;
		case SDLK_F3: ch = KC_F3; break;
		case SDLK_F4: ch = KC_F4; break;
		case SDLK_F5: ch = KC_F5; break;
		case SDLK_F6: ch = KC_F6; break;
		case SDLK_F7: ch = KC_F7; break;
		case SDLK_F8: ch = KC_F8; break;
		case SDLK_F9: ch = KC_F9; break;
		case SDLK_F10: ch = KC_F10; break;
		case SDLK_F11: ch = KC_F11; break;
		case SDLK_F12: ch = KC_F12; break;
		case SDLK_F13: ch = KC_F13; break;
		case SDLK_F14: ch = KC_F14; break;
		case SDLK_F15: ch = KC_F15; break;

		default: break;
	}

	if (ch) {
		if (kp) mods |= KC_MOD_KEYPAD;
		if (mc) mods |= KC_MOD_CONTROL;
		if (ms) mods |= KC_MOD_SHIFT;
		Term_keypress(ch, mods);
	} else if (key_code) {
		/* If the keycode is 7-bit ASCII (except numberpad) send
		 * directly to the game. */
		if (mc && (key_sym == SDLK_TAB || key_sym == SDLK_RETURN ||
				key_sym == SDLK_BACKSPACE ||
				MODS_INCLUDE_CONTROL(key_code)))
			mods |= KC_MOD_CONTROL;
		if (ms && MODS_INCLUDE_SHIFT(key_code)) mods |= KC_MOD_SHIFT;

		Term_keypress(key_code, mods);
	}
}


static void init_windows(void);
static void init_morewindows(void);
/*
 * Handle a single message sent to the application.
 *
 * Functions that are either called from a separate thread or which need to
 * create a separate thread (such as sounds) need to pass messages to this
 * function in order to execute most operations.  See the useage of
 * "SDL_USEREVENT".
 */
static errr sdl_HandleEvent(SDL_Event *event)
{
	
	/* Handle message */
	switch (event.type)
	{
		/* Keypresses */
		case SDL_KEYDOWN:
		{
			/* Handle keypress */
			sdl_keypress(event.key.keysym);
			
			break;
		}
		case SDL_MOUSEBUTTONDOWN:
		case SDL_MOUSEBUTTONUP:
		{
			/* Handle mouse stuff */
			sdl_HandleMouseEvent(event);
			break;
		}

		case SDL_MOUSEMOTION:
		{
                        int i;  
                        SDL_Event events[10];

			/*
                         * If there are a bundle of mouse movements pending,
                         * we'll just take every tenth one - this makes a
                         * simple approach to dragging practical, for instance.
			 */  
                        i = SDL_PeepEvents(events, 10, SDL_GETEVENT, SDL_EVENTMASK(SDL_MOUSEMOTION));
                        if (i > 0) 
                        {
                                *event = events[i - 1];
                        }

			/* Handle mouse stuff */
			sdl_HandleMouseEvent(event);
			break;
		}
			
		/* Shut down the game */
		/* XXX - check for stuck inside menu etc... */
		case SDL_QUIT:
		{
			/* We are playing a game with an active character */
			if (character_generated && inkey_flag)
			{
				/* Hack -- Forget messages */
				msg_flag = false;
				
				/* Save the game */
				save_game();
			}
			
			save_prefs();
			
			quit(null);
			break;
		}
			
		/* Resize the application */
		case SDL_VIDEORESIZE:
		{
			/* Free the surface */
			SDL_FreeSurface(AppWin);
			
			if (!fullscreen)
			{
				/* Make sure */
				vflags &= ~(SDL_FULLSCREEN);
				vflags |= SDL_RESIZABLE;
				
				screen_w = event.resize.w;
				screen_h = event.resize.h;
				
				if (screen_w < 640) screen_w = 640;
				if (screen_h < 480) screen_h = 480;
				
				/* Resize the application surface */
				AppWin = SDL_SetVideoMode(screen_w, screen_h, 0, vflags);
			}
			else
			{
				/* Make sure */
				vflags |= SDL_FULLSCREEN;
				vflags &= ~(SDL_RESIZABLE);
				
				AppWin = SDL_SetVideoMode(full_w, full_h, 0, vflags);
			}
			init_morewindows();
			init_windows();
			
			break;
		}
			
		case WINDOW_DRAW:
		{
			/* Redraw window that have asked */
			sdl_Window *window = (sdl_Window*)event.user.data1;
			sdl_WindowBlit(window);
			break;
		}
		default:
		{
			/* Do nothing */
			break;
		}
	}	
	sdl_WindowUpdate(&StatusBar);
	sdl_WindowUpdate(&PopUp);
	return (0);
}

/*
 * Update the redraw rect
 * A simple but effective way to keep track of what
 * parts of a window need to updated.
 * Any new areas that are updated before a blit are simply combined
 * into a new larger rectangle to encompass all changes.
 */
static void set_update_rect(term_window *win, SDL_Rect *rc)
{
	/* No outstanding update areas yet? */
	if (win.uRect.x == -1)
	{
		/* Simple copy */
		win.uRect = *rc;
	}
	else
	{
		/* Combine the old update area with the new */
		int x = MIN(win.uRect.x, rc.x);
		int y = MIN(win.uRect.y, rc.y);
		int x2 = MAX(win.uRect.x + win.uRect.w, rc.x + rc.w);
		int y2 = MAX(win.uRect.y + win.uRect.h, rc.y + rc.h);
		RECT(x, y, x2 - x, y2 - y, &win.uRect);
	}
}

/*
 * Clear a terminal window
 */
static errr Term_xtra_sdl_clear(void)
{
	term_window *win = (term_window*)(Term.data);
	SDL_Rect rc;
	
	/* Oops */
	if (!win.surface) return (1);
	
	/* Create the fill area */
	RECT(win.border, win.title_height, win.width - (2 * win.border),
		 win.height - win.border - win.title_height, &rc);
	
	
	/* Fill the rectangle */
	SDL_FillRect(win.surface, &rc, back_colour);
	
	/* Rectangle to update */
	set_update_rect(win, &rc);
	
	/* Success */
	return (0);
}

/*
 * Process at least one event
 */
static errr Term_xtra_sdl_event(int v)
{
	SDL_Event event;
	errr error = 0;
	
	/* Wait for an event */
	if (v)
	{
		/* Wait in 0.02s increments while updating animations every 0.2s */
		int i = 0;
		while (!SDL_PollEvent(&event))
		{
			if (i == 0) idle_update();
			usleep(20000);
			i = (i + 1) % 10;
		}

		/* Handle it */
		error = sdl_HandleEvent(&event);
	}
	
	/* Check for an event */
	else
	{
		/* Get a single pending event */
		if (SDL_PollEvent(&event))
		{
			/* Handle it */
			error = sdl_HandleEvent(&event);
		}
	}
	
	/* Note success or failure */
	return (error);
}

/*
 * Process all pending events
 */
static errr Term_xtra_sdl_flush(void)
{
	SDL_Event event;
	
	/* Get all pending events */
	while (SDL_PollEvent(&event))
	{
		/* Handle them (ignore errors) */
		(void)sdl_HandleEvent(&event);
	}
	
	/* Done */
	return (0);
}

/*
 * Delay for "x" milliseconds
 */
static errr Term_xtra_sdl_delay(int v)
{
	/* Sleep */
	if (v > 0)
	{
		Term_xtra_sdl_event(0);
		SDL_Delay(v);
	}
	
	/* Success */
	return (0);
}

static errr Term_bigcurs_sdl(int col, int row)
{
	term_window *win = (term_window*)(Term.data);
	
	Uint32 colour = text_colours[TERM_YELLOW];
	
	SDL_Rect rc;
	
	/* Make a rectangle */
	RECT(col * win.tile_wid, row * win.tile_hgt, win.tile_wid * tile_width, win.tile_hgt * tile_height, &rc);
	
	/* Translate it */
	rc.x += win.border;
	rc.y += win.title_height;
	
	/* Draw it */
	sdl_DrawBox(win.surface, &rc, colour, 1);
	
	/* Update area */
	set_update_rect(win, &rc);
	
	/* Success */
	return (0);
}

static errr Term_curs_sdl(int col, int row)
{
	term_window *win = (term_window*)(Term.data);
	
	Uint32 colour = text_colours[TERM_YELLOW];
	
	SDL_Rect rc;
	
	/* Make a rectangle */
	RECT(col * win.tile_wid, row * win.tile_hgt, win.tile_wid, win.tile_hgt, &rc);
	
	/* Translate it */
	rc.x += win.border;
	rc.y += win.title_height;
	
	/* Draw it */
	sdl_DrawBox(win.surface, &rc, colour, 1);
	
	/* Update area */
	set_update_rect(win, &rc);

	/* Success */
	return (0);
}

static errr Term_xtra_sdl(int n, int v)
{
	switch (n)
	{
		/* Process an event */
		case TERM_XTRA_EVENT:
		{
			return (Term_xtra_sdl_event(v));
		}
		/* Flush all events */
		case TERM_XTRA_FLUSH:
		{
			return (Term_xtra_sdl_flush());
		}
		/* Clear the screen */
		case TERM_XTRA_CLEAR:
		{
			return (Term_xtra_sdl_clear());
		}
		/* Show or hide the cursor */
		case TERM_XTRA_SHAPE:
		{
			int x, y;
			
			/* Obtain the cursor */
			(void)Term_locate(&x, &y);
			
			/* Show or hide the cursor */
			Term_curs_sdl(x, y);
			return (0);
		}
		case TERM_XTRA_FRESH:
		{
			/* Get the current window data */
			term_window *win = (term_window*)(Term.data);
			
			/* Blat it! */
			sdl_BlitWin(win);
			
			/* Done */
			return (0);
		}
		case TERM_XTRA_DELAY:
		{
			return (Term_xtra_sdl_delay(v));
		}
		case TERM_XTRA_REACT:
		{
			int i;
			/* Re-initialize the colours */
			for (i = 0; i < MAX_COLORS; i++)
			{
				text_colours[i] = SDL_MapRGB(AppWin.format, angband_color_table[i][1],
											 angband_color_table[i][2],
											 angband_color_table[i][3]);
			}
		}
	}
	
	return (1);
}



static errr Term_wipe_sdl(int col, int row, int n)
{
	term_window *win = (term_window*)(Term.data);
	
	SDL_Rect rc;
	
	/* Build the area to black out */
	rc.x = col * win.tile_wid;
	rc.y = row * win.tile_hgt;
	rc.w = win.tile_wid * n;
	rc.h = win.tile_hgt;
	
	/* Translate it */
	rc.x += win.border;
	rc.y += win.title_height;
	
	/* Wipe it */
	SDL_FillRect(win.surface, &rc, back_colour);
	
	/* Update */
	set_update_rect(win, &rc);
	
	return (0);
}

/*
 * Given a position in the ISO Latin-1 character set, return
 * the correct character on this system.
 */
 static byte Term_xchar_sdl(byte c)
{
 	/* The Sdl port uses the Latin-1 standard */
 	return (c);
}

/*
 * Draw some text to a window
 */
static errr Term_text_sdl(int col, int row, int n, byte a, const char *s)
{
	term_window *win = (term_window*)(Term.data);
	Uint32 colour = text_colours[a];
	int x = col * win.tile_wid;
	int y = row * win.tile_hgt;
	
	/* Translate */
	x += win.border;
	y += win.title_height;
	
	/* Not much point really... */
	if (!win.visible) return (0);
	
	/* Clear the way */
	Term_wipe_sdl(col, row, n);
	
	/* Draw it */
	return (sdl_FontDraw(&win.font, win.surface, colour, x, y, n, s));
}

#ifdef USE_GRAPHICS
/*
 * Do a 'stretched blit'
 * SDL has no support for stretching... What a bastard!
 * 
 */
static void sdl_StretchBlit(SDL_Surface *src, SDL_Rect *srcRect, SDL_Surface *dest, SDL_Rect *destRect)
{
	int x, y;
	int sx, sy, dx, dy;
	Uint8 *ps, *pd;
	
	for (y = 0; y < destRect.h; y++)
	{
		for (x = 0; x < destRect.w; x++)
		{
			/* Actual source coords */
			sx = (srcRect.w * x / (destRect.w)) + srcRect.x;
			sy = (srcRect.h * y / (destRect.h)) + srcRect.y;
			
			/* Find a source pixel */
			ps = (Uint8 *)src.pixels + (sx * src.format.BytesPerPixel) + (sy * src.pitch);
#if 0
			/* Do we need to draw it? */
			switch (src.format.BytesPerPixel)
			{
				case 1:
				{
					if (*ps == src.format.colorkey) continue;
					break;
				}
				case 2:
				{
					Uint16 *ps16 = (Uint16*) ps;
					if (*ps16 == src.format.colorkey) continue;
					break;
				}
				case 3:
				case 4:
				{
					Uint32 *ps32 = (Uint32*) ps;
					if (*ps32 == src.format.colorkey) continue;
					break;
				}
			}
#endif
					
			/* Actual destination pixel coords */
			dx = x + destRect.x;
			dy = y + destRect.y;
			
			/* Destination pixel */
			pd = (Uint8 *)dest.pixels + (dx * dest.format.BytesPerPixel) + (dy * dest.pitch);
			
			switch (dest.format.BytesPerPixel)
			{
				case 1:
				{
					*pd = *ps;
					break;
				}
				case 2:
				{
					Uint16 *ps16 = (Uint16*) ps;
					Uint16 *pd16 = (Uint16*) pd;
					*pd16 = *ps16;
					break;
				}
				case 3:
				case 4:
				{
					Uint32 *ps32 = (Uint32*) ps;
					Uint32 *pd32 = (Uint32*) pd;
					*pd32 = *ps32;
				}
			}
			
		}
	}
	
}

/*
 * Make the 'pre-stretched' tiles for this window
 * Assumes the tiles surface was freed elsewhere
 */
static errr sdl_BuildTileset(term_window *win)
{
	int x, y;
	int ta, td;
	int xx, yy;
	GfxInfo *info = &GfxDesc[use_graphics];
	if (!use_graphics) return (1);
	
	/* Calculate the number of tiles across & down*/
	ta = GfxSurface.w / info.width;
	td = GfxSurface.h / info.height;
	
	/* Calculate the size of the new surface */
	x = ta * win.tile_wid * tile_width;
	y = td * win.tile_hgt * tile_height;
	
	/* Make it */
	win.tiles = SDL_CreateRGBSurface(SDL_SWSURFACE, x, y,
			GfxSurface.format.BitsPerPixel,
			GfxSurface.format.Rmask, GfxSurface.format.Gmask,
			GfxSurface.format.Bmask, GfxSurface.format.Amask);
	
	/* Bugger */
	if (!win.tiles) return (1);
	
	/* For every tile... */
	for (xx = 0; xx < ta; xx++)
	{
		for (yy = 0; yy < td; yy++)
		{
			SDL_Rect src, dest;
                        int dwid = win.tile_wid * tile_width;
                        int dhgt = win.tile_hgt * tile_height;

			/* Source rectangle (on GfxSurface) */
			RECT(xx * info.width, yy * info.height, info.width, info.height, &src);

			/* Destination rectangle (win.tiles) */
			RECT(xx * dwid, yy * dhgt, dwid, dhgt, &dest);

			/* Do the stretch thing */
			sdl_StretchBlit(GfxSurface, &src, win.tiles, &dest);
		}
	}

	return (0);
}
#endif

/*
 * Put some gfx on the screen
 * XXX - This function _never_ seems to get called with n > 1 ?
 * This needs improvement...
 */
static errr Term_pict_sdl(int col, int row, int n, const byte *ap, const char *cp,
						  const byte *tap, const char *tcp)
{
	
#ifdef USE_GRAPHICS
	/* Get the right window */
	term_window *win = (term_window*)(Term.data);
	
	SDL_Rect rc, src;
	int i, j;
	
	/* First time a pict is requested we load the tileset in */
	if (!win.tiles)
	{
		sdl_BuildTileset(win);
		if (!win.tiles) return (1);
	}
	
	/* Make the destination rectangle */
	RECT(col * win.tile_wid, row * win.tile_hgt, win.tile_wid, win.tile_hgt, &rc);
	
	/* Translate it */
	rc.x += win.border;
	rc.y += win.title_height;
	
	/* Stretch for bigtile mode */
	rc.w *= tile_width;
	rc.h *= tile_height;
	
	/* Get the dimensions of the graphic surface */
	src.w = rc.w;
	src.h = rc.h;
	
	/* Clear the way */
	for (i = 0; i < tile_width; i++)
	        for (j = 0; j < tile_height; j++)
		        Term_wipe_sdl(col + i, row + j, n);
	
	/* Blit 'em! (it) */
	for (i = 0; i < n; i++)
	{
		/* Get the terrain tile */
		src.x = (tcp[i] & 0x7F) * src.w;
		src.y = (tap[i] & 0x7F) * src.h;
		
		SDL_BlitSurface(win.tiles, &src, win.surface, &rc);
		
		/* If foreground is the same as background, we're done */
		if ((tap[i] == ap[i]) && (tcp[i] == cp[i])) continue;
		
		/* Get the foreground tile */
		src.x = (cp[i] & 0x7F) * src.w;
		src.y = (ap[i] & 0x7F) * src.h;
		
		SDL_BlitSurface(win.tiles, &src, win.surface, &rc);
	}
	
	/* Update area */
	set_update_rect(win, &rc);
#endif
	
	return (0);
}

/*
 * Create and initialize the Term contined within this window.
 */
static void term_data_link_sdl(term_window *win)
{
	term *t = &win.term_data;
	
	/* Initialize the term */
	term_init(t, win.cols, win.rows, win.keys);
	
	t.higher_pict = true;
	
	/* Use a "software" cursor */
	t.soft_cursor = true;
	
	/* Erase with "white space" */
	t.attr_blank = TERM_WHITE;
	t.char_blank = ' ';
	
	/* Never refresh one row */
	t.never_frosh = true;
	
	/* Ignore the init/nuke hooks */
	
	/* Prepare the template hooks */
	t.xtra_hook = Term_xtra_sdl;
	t.curs_hook = Term_curs_sdl;
	t.bigcurs_hook = Term_bigcurs_sdl;
	t.wipe_hook = Term_wipe_sdl;
	t.text_hook = Term_text_sdl;
	t.pict_hook = Term_pict_sdl;
	t.xchar_hook = Term_xchar_sdl;
	
	/* Remember where we came from */
	t.data = win;
}

/*
 * Initialize the status bar:
 *  Populate it with some buttons
 *  Set the custom draw function for the bar
 */
static void init_morewindows(void)
{
	char buf[128];
	sdl_Button *button;
	int x;

	popped = false;
	
	/* Make sure */
	sdl_WindowFree(&PopUp);
	
	/* Initialize the status bar */
	sdl_WindowInit(&StatusBar, AppWin.w, StatusHeight, AppWin, DEFAULT_FONT_FILE);
	
	/* Cusom drawing function */
	StatusBar.draw_extra = draw_statusbar;
	
	AboutSelect = sdl_ButtonBankNew(&StatusBar.buttons);
	button = sdl_ButtonBankGet(&StatusBar.buttons, AboutSelect);
	
	my_strcpy(buf, format("%s %s", VERSION_NAME, VERSION_STRING), sizeof(buf));
	
	/* Initialize the 'about' button */
	sdl_ButtonSize(button, StatusBar.font.width * strlen(buf) + 5, StatusHeight - 2);
	sdl_ButtonMove(button, 1, 1);
	sdl_ButtonVisible(button, true);
	sdl_ButtonCaption(button, buf);
	button.activate = AboutActivate;
	
	
	/* New button */
	TermSelect = sdl_ButtonBankNew(&StatusBar.buttons);
	button = sdl_ButtonBankGet(&StatusBar.buttons, TermSelect);
	
	/* Initialize the 'term' button */
	sdl_ButtonSize(button, 60, StatusHeight - 2);
	x = 100 + (StatusBar.font.width * 5);
	sdl_ButtonMove(button, x, 1);
	sdl_ButtonVisible(button, true);
	button.activate = TermActivate;
	
	/* Another new button */
	VisibleSelect = sdl_ButtonBankNew(&StatusBar.buttons);
	button = sdl_ButtonBankGet(&StatusBar.buttons, VisibleSelect);
	
	/* Initialize the 'visible' button */
	sdl_ButtonSize(button, 60,  StatusHeight - 2);
	x = 200 + (StatusBar.font.width * 8);
	sdl_ButtonMove(button, x, 1);
	sdl_ButtonVisible(button, true);
	button.activate = VisibleActivate;
	
	/* Another new button */
	FontSelect = sdl_ButtonBankNew(&StatusBar.buttons);
	button = sdl_ButtonBankGet(&StatusBar.buttons, FontSelect);
	
	/* Initialize the 'font_select' button */
	sdl_ButtonSize(button, 60,  StatusHeight - 2);
	sdl_ButtonMove(button, 400, 1);
	button.activate = FontActivate;
	
	/* Another new button */
	MoreSelect = sdl_ButtonBankNew(&StatusBar.buttons);
	button = sdl_ButtonBankGet(&StatusBar.buttons, MoreSelect);
	
	/* Initialize the 'more' button */
	sdl_ButtonSize(button, 60,  StatusHeight - 2);
	sdl_ButtonMove(button, 400, 1);
	sdl_ButtonVisible(button, true);
	sdl_ButtonCaption(button, "Options");
	button.activate = MoreActivate;
	
	/* Another new button */
	QuitSelect = sdl_ButtonBankNew(&StatusBar.buttons);
	button = sdl_ButtonBankGet(&StatusBar.buttons, QuitSelect);
	
	/* Initialize the 'quit' button */
	sdl_ButtonSize(button, 60,  StatusHeight - 2);
	sdl_ButtonMove(button, AppWin.w - 61, 1);
	sdl_ButtonCaption(button, "Quit");
	button.activate = QuitActivate;
	sdl_ButtonVisible(button, true);
	
	SetStatusButtons();
	
	TermFocus(0);
}

#ifdef USE_GRAPHICS

/*
 * The new streamlined graphics loader.
 * Only uses colour keys.
 * Much more tolerant of different bit-planes
 */
static errr load_gfx(void)
{
	char buf[1024];
	const char *filename = GfxDesc[use_graphics].gfxfile;
	SDL_Surface *temp;

	/* This may be called when GRAPHICS_NONE is set */
	if (!filename) return (0);

	/* Free the old surface */
	if (GfxSurface) SDL_FreeSurface(GfxSurface);

	/* Find and load the file into a temporary surface */
	path_build(buf, sizeof(buf), ANGBAND_DIR_XTRA_GRAF, filename);
	temp = IMG_Load(buf);
	if (!temp) return (1);

	/* Change the surface type to the current video surface format */
	GfxSurface = SDL_DisplayFormatAlpha(temp);

	/* Make sure we know what pref file to use */
	ANGBAND_GRAF = GfxDesc[use_graphics].pref;

	/* Reset the graphics mapping for this tileset */
	if (character_dungeon) reset_visuals(true);

	/* All good */
	return (0);
}
#endif

/*
 * Initialize the graphics
 */
static void init_gfx(void)
{
#ifndef USE_GRAPHICS
	/* User requested gfx but USE_GRAPHICS not defined... */
	if (arg_graphics)
	{
		arg_graphics = false;
	}
	
	/* Make sure */
	use_graphics = GRAPHICS_NONE;
	tile_width = 1;
        tile_height = 1;
#else
	GfxInfo *info = &GfxDesc[use_graphics];
	int i;
	
	/* Check for existence of required files */
	for (i = 0; i < GfxModes; i++)
	{
		char path[1024];
		
		/* Check the graphic file */
		if (GfxDesc[i].gfxfile)
		{
			path_build(path, sizeof(path), ANGBAND_DIR_XTRA_GRAF, GfxDesc[i].gfxfile);

			if (!file_exists(path))
			{
				plog_fmt("Can't find file %s - graphics mode '%s' will be disabled.", path, GfxDesc[i].name);
				GfxDesc[i].avail = false;
			}
		}
	}
	
	/* Check availability (default to no graphics) */
	if (!info.avail)
	{
		use_graphics = GRAPHICS_NONE;
		arg_graphics = false;
		tile_width = 1;
		tile_height = 1;
	}
	
	/* Load the graphics stuff in */
	load_gfx();
#endif
}

/*
 * Create the windows
 * Called sometime after load_prefs()
 */
static void init_windows(void)
{
	int i;
	
	for (i = 0; i < ANGBAND_TERM_MAX; i++)
	{
		term_window *win = &windows[i];
		
		/* Only bother with visible windows */
		if (win.visible)
		{
			/* Don't crowd out the status bar... */
			if (win.top < StatusHeight) win.top = StatusHeight;
#ifdef USE_GRAPHICS			
			/* Invalidate the gfx surface */
			if (win.tiles)
			{
				SDL_FreeSurface(win.tiles);
				win.tiles = null;
			}
#endif
			/* This will set up the window correctly */
			ResizeWin(win, win.width, win.height);
		}
		else
		{
			/* Doesn't exist */
			angband_term[i] = null;
		}
		
		/* Term 0 is at the top */
		Zorder[i] = ANGBAND_TERM_MAX - i - 1;
	}
	
	/* Good to go... */
	Term_activate(term_screen);
}

/*
 * Set up some SDL stuff
 *
 */
static void init_sdl_local(void)
{
	const SDL_VideoInfo *VideoInfo;
	
	int i;
	int h, w;
	char path[1024];
	
	/* Get information about the video hardware */
	VideoInfo = SDL_GetVideoInfo();
	
	/* Require at least 256 colors */
	if (VideoInfo.vfmt.BitsPerPixel < 8) quit(format("This %s port requires lots of colors.", VERSION_NAME));
	
	full_w = VideoInfo.current_w;
	full_h = VideoInfo.current_h;
	
	/* Use a software surface - A tad inefficient, but stable... */
	vflags |= SDL_SWSURFACE;
	
	/* Set fullscreen flag */
	if (fullscreen) vflags |= SDL_FULLSCREEN;
	
	/* otherwise we make this surface resizable */
	else vflags |= SDL_RESIZABLE;
	
	/* Create the main window */
	AppWin = SDL_SetVideoMode(fullscreen ? full_w : screen_w,fullscreen ? full_h : screen_h, 0, vflags);
	
	/* Handle failure */
 	if (!AppWin) quit(format("Failed to create %dx%d window at %d bpp!",
							 screen_w, screen_h, VideoInfo.vfmt.BitsPerPixel));
	
	/* Set the window caption */
	SDL_WM_SetCaption(VERSION_NAME, null);
	
	/* Enable key repeating; use defaults */
	(void)SDL_EnableKeyRepeat(SDL_DEFAULT_REPEAT_DELAY, SDL_DEFAULT_REPEAT_INTERVAL);
	
	/* Enable Unicode (so we can read key codes) */
	(void)SDL_EnableUNICODE(1);
	
	/* Build a color for "black" that matches the pixel depth of this surface */
	back_colour = SDL_MapRGB(AppWin.format, angband_color_table[TERM_DARK][1],
						   angband_color_table[TERM_DARK][2],
						   angband_color_table[TERM_DARK][3]);
	
	
	
	/* Initialize the colours */
	for (i = 0; i < MAX_COLORS; i++)
	{
		text_colours[i] = SDL_MapRGB(AppWin.format, angband_color_table[i][1],
									 angband_color_table[i][2],
									 angband_color_table[i][3]);
	}
	
	/* Get the height of the status bar */
	sdl_CheckFont(DEFAULT_FONT_FILE, &w, &h);
	StatusHeight = h + 3;
	
	/* Font used for window titles */
	sdl_FontCreate(&SystemFont, DEFAULT_FONT_FILE, AppWin);

	/* Get the icon for display in the About box */
	path_build(path, sizeof(path), ANGBAND_DIR_XTRA_ICON, "att-128.png");
	if (file_exists(path))
		mratt = IMG_Load(path);
}

/**
 * Font sorting function
 *
 * Orders by width, then height, then face
 */
static int cmp_font(const void *f1, const void *f2)
{
	const char *font1 = *(const char **)f1;
	const char *font2 = *(const char **)f2;
	int height1, height2;
	int width1, width2;
	char face1[5], face2[5];

	sscanf(font1, "%dx%d%4s.", &width1, &height1, face1);
	sscanf(font2, "%dx%d%4s.", &width2, &height2, face2);

	if (width1 < width2)
		return -1;
	else if (width1 > width2)
		return 1;
	else {
		if (height1 < height2)
			return -1;
		else if (height1 > height2)
			return 1;
		else
			return strcmp(face1, face2);
	}
}

/** 
 * This function is now mis-named as paths are set correctly by init_stuff()
 * in main.c before init_sdl calls this. But it still does some other stuff.
 */ 
static void init_paths(void)
{
	int i;
	char path[1024];
	char buf[1024];
	ang_dir *dir;

	/* Build the filename */
	path_build(path, sizeof(path), ANGBAND_DIR_XTRA_FONT, DEFAULT_FONT_FILE);
	
	/* Hack -- Validate the basic font */
	validate_file(path);
	
	for (i = 0; i < MAX_FONTS; i++)
		FontList[i] = null;


	/** Scan for fonts **/

	/* Open the directory */
	dir = my_dopen(ANGBAND_DIR_XTRA_FONT);
	if (!dir) return;

	/* Read every font to the limit */
	while (my_dread(dir, buf, sizeof buf))
	{
		/* Check for file extension */
		if (suffix(buf, ".fon"))
			FontList[num_fonts++] = string_make(buf);

		/* Don't grow to long */
		if (num_fonts == MAX_FONTS) break;
	}

	sort(FontList, num_fonts, sizeof(FontList[0]), cmp_font);
	/* Done */
	my_dclose(dir);
}


const char help_sdl[] = "SDL frontend";
/*
 * The SDL port's "main()" function.
 */
int init_sdl(int argc, char *argv[])
{
	/* Initialize SDL:  Timer, video, and audio functions */
	if (SDL_Init(SDL_INIT_VIDEO) < 0)
	{
		fprintf(stderr, "Couldn't initialize SDL: %s\n",SDL_GetError());
		return (2);
	}
	
	/* Initialize the TTF library */
	if (TTF_Init() < 0)
	{
		fprintf(stderr, "Couldn't initialize TTF: %s\n",SDL_GetError());
		SDL_Quit();
		return (2);
	}

	/* Init some extra paths */
	init_paths();
	
	/* Load prefs */
	load_prefs();
	
	/* Get sdl going */
	init_sdl_local();
	
	/* Prepare the windows */
	init_windows();
	
	/* Prepare the gfx */
	init_gfx();
	
	/* Prepare some more windows(!) */
	init_morewindows();
	
	/* Activate  quit hook */
	quit_aux = hook_quit;

	/* Paranoia */
	return (0);
}

#endif /* USE_SDL */
