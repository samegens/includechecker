using System;
using System.IO;


namespace DevPal.CSLib
{
	/// <summary>
	/// Contains useful utility functions on strings.
	/// </summary>
	public static class StringUtil
	{
		// http://www.codeproject.com/string/wildcmp.asp
		public static bool WildCardCompare(string wild, string str)
		{
			int cp=0, mp=0;
	
			int i=0;
			int j=0;
			while (i < str.Length && j < wild.Length && wild[j] != '*')
			{
				if ((wild[j] != str[i]) && (wild[j] != '?')) 
				{
					return false;
				}
				i++;
				j++;
			}
		
			while (i<str.Length) 
			{
				if (j<wild.Length && wild[j] == '*') 
				{
					if ((j++)>=wild.Length) 
					{
						return true;
					}
					mp = j;
					cp = i+1;
				} 
				else if (j<wild.Length && (wild[j] == str[i] || wild[j] == '?')) 
				{
					j++;
					i++;
				} 
				else 
				{
					j = mp;
					i = cp++;
				}
			}
		
			while (j < wild.Length && wild[j] == '*')
			{
				j++;
			}
			return j>=wild.Length;
		}


        /// <summary>
        /// Interprets the start of inString as a positive integer.
        /// </summary>
        /// <returns>Returns -1 if the string could not be parsed.</returns>
		public static int ParseBeginOfStringAsInt(string inString)
		{
			int pos = 0;
			while (pos < inString.Length && inString[pos] >= '0' && inString[pos] <= '9')
			{
				pos++;
			}

			if (pos > 0)
			{
				return int.Parse(inString.Substring(0, pos));
			}

			return -1;
		}


		/// <summary>
		/// Checks if the string ends with inChar
		/// </summary>
		public static bool EndsWith(this string inString, char inChar)
		{
			if (inString.Length == 0)
				return false;

			return inString[inString.Length - 1] == inChar;
		}
	}
}
