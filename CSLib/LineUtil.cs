using System;
using System.Collections.Generic;


namespace DevPal.CSLib
{
	/// <summary>
	/// LineUtil contains static utility functions that process text lines.
	/// </summary>
	public static class LineUtil
	{
		/// <summary>
		/// Turns text containing multiple lines in a list of separate lines.
		/// Trailing CR/LF is removed from each line.
		/// </summary>
		public static List<string> GetLineList(string inText)
		{
			if (string.IsNullOrEmpty(inText))
				return new List<string>();

			string[] lines = inText.Split('\n');
			List<string> line_list = new List<string>();
			for (int i = 0; i < lines.Length; ++i)
				line_list.Add(lines[i].TrimEnd('\r', '\n'));

			return line_list;
		}


        /// <summary>
		/// Returns a list of lines that are not in inList1, but are in inList2.
		/// </summary>
		public static List<string> GetLineListsDifference(List<string> inList1, List<string> inList2)
		{
			if (inList1.Count == 0)
				return inList2;

			List<string> sorted_list1 = new List<string>(inList1);
			sorted_list1.Sort();
			List<string> sorted_list2 = new List<string>(inList2);
			sorted_list2.Sort();

			List<string> difference = new List<string>();
			IEnumerator<string> iter2 = sorted_list2.GetEnumerator();
			IEnumerator<string> iter1 = sorted_list1.GetEnumerator();
			iter1.MoveNext();
			string line1 = iter1.Current;

			// Walk through the second list, and try to find matches in the first list.
			while (iter2.MoveNext()) 
			{
				string line2 = iter2.Current;
				while (line1.CompareTo(line2) < 0)
				{
					if (iter1.MoveNext())
					{
						line1 = iter1.Current;
					}
					else
					{
						line1 = string.Empty;
						break;
					}
				}
				if (line1.CompareTo(line2) != 0)
				{
					difference.Add(line2);
				}
			}

			return difference;
		}
	}
}
