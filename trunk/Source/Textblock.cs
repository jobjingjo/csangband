using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband {
	class Textblock {
		public string Text;
		List<ConsoleColor> attrs;

		public Textblock(){
			Text = "";
			attrs = new List<ConsoleColor>();
		}

		public ConsoleColor[] Attributes {
			get {
				return attrs.ToArray();
			}
		}

		/**
		 * Add text to a text block, formatted.
		 */
		public void append(string fmt, params object[] vals)
		{
			vappend_c(ConsoleColor.White, fmt, vals);
		}

		/**
		 * Add coloured text to a text block, formatted.
		 */
		public void append_c(ConsoleColor attr, string fmt, params object[] vals)
		{
			vappend_c(attr, fmt, vals);
		}

		void vappend_c(ConsoleColor attr, string fmt, params object[] vp)
		{
			string ToAppend = String.Format(fmt, vp);

			for(int i = 0; i < ToAppend.Length; i++) {
				attrs.Add(attr);
			}

			Text += ToAppend;
		}

		/*
		private void new_line(size_t **line_starts, size_t **line_lengths,
				size_t *n_lines, size_t *cur_line,
				size_t start, size_t len)
		{
			if (*cur_line == *n_lines) {
				// this number is not arbitrary: it's the height of a "standard" term
				// HACK: This should not be hard coded, it should be in a class somewhere
				(*n_lines) += 24;

				*line_starts = mem_realloc(*line_starts,
						*n_lines * sizeof **line_starts);
				*line_lengths = mem_realloc(*line_lengths,
						*n_lines * sizeof **line_lengths);
			}

			(*line_starts)[*cur_line] = start;
			(*line_lengths)[*cur_line] = len;

			(*cur_line)++;
		}
		*/

		/**
		 * Given a certain width, split a textblock into wrapped lines of text.
		 *
		 * \returns Number of lines in output.
		 */
		/*
		size_t textblock_calculate_lines(textblock *tb, size_t **line_starts, size_t **line_lengths, size_t width)
		{
			const char *text = tb.text;

			size_t cur_line = 0, n_lines = 0;

			size_t len = strlen(text);
			size_t text_offset;

			size_t line_start = 0, line_length = 0;
			size_t word_start = 0, word_length = 0;

			assert(width > 0);

			for (text_offset = 0; text_offset < len; text_offset++) {
				if (text[text_offset] == '\n') {
					new_line(line_starts, line_lengths, &n_lines, &cur_line,
							line_start, line_length);

					line_start = text_offset + 1;
					line_length = 0;
				} else if (text[text_offset] == ' ') {
					line_length++;

					word_start = line_length;
					word_length = 0;
				} else {
					line_length++;
					word_length++;
				}

				// special case: if we have a very long word, just slice it
				if (word_length == width) {
					new_line(line_starts, line_lengths, &n_lines, &cur_line,
							line_start, line_length);

					line_start += line_length;
					line_length = 0;
				}

				// normal wrapping: wrap text at last word
				if (line_length == width) {
					size_t last_word_offset = word_start;
					while (text[line_start + last_word_offset] != ' ')
						last_word_offset--;

					new_line(line_starts, line_lengths, &n_lines, &cur_line,
							line_start, last_word_offset);

					line_start += word_start;
					line_length = word_length;
				}
			}

			return cur_line;
		}*/

		/**
		 * Output a textblock to file.
		 */
		/*
		public void textblock_to_file(textblock *tb, ang_file *f, int indent, int wrap_at)
		{
			size_t *line_starts = null;
			size_t *line_lengths = null;

			size_t n_lines, i;

			int width = wrap_at - indent;
			assert(width > 0);

			n_lines = textblock_calculate_lines(tb, &line_starts, &line_lengths, width);

			for (i = 0; i < n_lines; i++) {
				file_putf(f, "%*c%.*s\n",
						indent, ' ',
						line_lengths[i], tb.text + line_starts[i]);
			}
		}*/
	}
}
