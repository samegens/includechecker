using System.Text;
using System.Collections.Generic;


namespace DevPal.CSLib.CppParsing
{
    /// <summary>
    /// Simple C++ parser.
    /// </summary>
	public class CppParser
	{
        /// <summary>
        /// Remove #include statements from C++ source.
        /// </summary>
        /// <param name="ioContents"></param>
		public void RemoveIncludes(ref string ioContents)
		{
			List<string> lines = LineUtil.GetLineList(ioContents);
			StringBuilder output = new StringBuilder();
			bool add_newline = false;
			foreach (string line in lines)
			{
				if (add_newline)
					output.Append("\r\n");
				add_newline = true;

				string trimmed_line = line.Trim();
				if (!IncludeFinder.IsInclude(line))
					output.Append(line);
			}
			ioContents = output.ToString();
		}


        /// <summary>
        /// Remove C and C++ style comments from C++ source.
        /// </summary>
		public void RemoveComments(ref string ioContents)
		{
			int i = 0;
			int length = ioContents.Length;
			StringBuilder new_contents = new StringBuilder();
			while (i < length)
			{
				if (i + 1 < length
					&& ioContents[i] == '/' && ioContents[i+1] == '/')
				{
					SkipCppComment(ioContents, ref i);
				}
				else if (i + 1 < length
					&& ioContents[i] == '/' && ioContents[i+1] == '*')
				{
					new_contents.Append(SkipCComment(ioContents, ref i));
				}
				else if (ioContents[i] == '"')
				{
					new_contents.Append(ReadString(ioContents, ref i));
				}
				else if (ioContents[i] == '\'')
				{
					new_contents.Append(ReadChar(ioContents, ref i));
				}
				else
				{
					new_contents.Append(ioContents[i]);
					i++;
				}
			}
			ioContents = new_contents.ToString();
		}
		
		
        /// <summary>
        /// Remove string literals from C++ source.
        /// </summary>
		public void RemoveStrings(ref string ioContents)
		{
			int i = 0;
			int length = ioContents.Length;
			StringBuilder new_contents = new StringBuilder();
			while (i < length)
			{
				if (i < length && ioContents[i] == '"')
				{
					++i;
					SkipStringLiteral(ioContents, ref i);
				}
				else if (i < length && ioContents[i] == '\'')
				{
					++i;
					SkipCharLiteral(ioContents, ref i);
				}
				else
				{
					new_contents.Append(ioContents[i]);
					i++;
				}
			}
			ioContents = new_contents.ToString();
		}
		
		
        /// <summary>
        /// Set the index to the first character after a string literal.
        /// </summary>
		private void SkipStringLiteral(string inContents, ref int ioIndex)
		{
			int length = inContents.Length;
			while (ioIndex < length)
			{
				if (inContents[ioIndex] == '\\')
				{
					// Skip this and next character
					ioIndex += 2;
				}
				else if (inContents[ioIndex] == '"')
				{
					++ioIndex;
					break;
				}
				else
				{
					++ioIndex;
				}
			}
		}


        /// <summary>
        /// Set the index to the first character after a character literal.
        /// </summary>
		private void SkipCharLiteral(string inContents, ref int ioIndex)
		{
			int length = inContents.Length;
			while (ioIndex < length)
			{
				if (inContents[ioIndex] == '\\')
				{
					// Skip this and next character
					ioIndex += 2;
				}
				else if (inContents[ioIndex] == '\'')
				{
					++ioIndex;
					break;
				}
				else
				{
					++ioIndex;
				}
			}
		}


        /// <summary>
        /// Set the index to the first character after a C++ style comment.
        /// </summary>
		private void SkipCppComment(string inContents, ref int ioIndex)
		{
			int length = inContents.Length;
			while (ioIndex < length
					&& inContents[ioIndex] != '\r'
					&& inContents[ioIndex] != '\n')
				ioIndex++;
		}


        /// <summary>
        /// Set the index to the first character after the C-style comment.
        /// </summary>
        /// <returns>
        /// Returns the comment replaced by spaces, to keep locations of other source intact.
        /// </returns>
		private string SkipCComment(string inContents, ref int ioIndex)
		{
			string spaces = "";
			int length = inContents.Length;
			while (ioIndex + 1 < length
				&& (inContents[ioIndex] != '*' || inContents[ioIndex + 1] != '/'))
			{
				if (inContents[ioIndex] == '\r' || inContents[ioIndex] == '\n')
					spaces += inContents[ioIndex];
				else
					spaces += ' ';
				ioIndex++;
			}

			// Skip C comment end marker if present.
			if (ioIndex + 1 < length
				&& inContents[ioIndex] == '*'
				&& inContents[ioIndex + 1] == '/')
			{
				ioIndex += 2;
				spaces += "  ";
			}

			return spaces;
		}


		private string ReadString(string inContents, ref int ioIndex)
		{
			string read_string = "\"";
			int length = inContents.Length;
			// Skip first ".
			ioIndex++;
			while (ioIndex < length)
			{
				if (inContents[ioIndex] == '\"')
				{
					read_string += '\"';
					ioIndex++;
					return read_string;
				}
				else if (inContents[ioIndex] == '\\')
				{
					read_string += ReadEscapeSequence(inContents, ref ioIndex);
				}
				else
				{
					read_string += inContents[ioIndex];
					ioIndex++;
				}
			}
			// EOF in string, return string read so far.
			return read_string;
		}


		private string ReadChar(string inContents, ref int ioIndex)
		{
			string read_string = "'";
			ioIndex++;	// Skip first '.
			int length = inContents.Length;
			while (ioIndex < length)
			{
				if (inContents[ioIndex] == '\'')
				{
					read_string += '\'';
					ioIndex++;
					return read_string;
				}
				else if (inContents[ioIndex] == '\\')
				{
					read_string += ReadEscapeSequence(inContents, ref ioIndex);
				}
				else
				{
					read_string += inContents[ioIndex];
					ioIndex++;
				}
			}
			// EOF in char, return string read so far.
			return read_string;
		}


		// Pre: inContents[ioIndex] == '\\'
		// Post: ioIndex points to the char after the last char of the sequence.
		private string ReadEscapeSequence(string inContents, ref int ioIndex)
		{
			if (ioIndex + 1 < inContents.Length)
			{
				string sequence = "\\" + inContents[ioIndex + 1];
				ioIndex += 2;
				return sequence;
			}
			else
			{
				ioIndex++;
				return "\\";
			}
		}
	}
}
