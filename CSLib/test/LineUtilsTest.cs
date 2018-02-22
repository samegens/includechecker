using System;
using NUnit.Framework;
using DevPal.CSLib;
using System.Collections.Generic;

namespace DevPal.CSLib.Tests
{
	[TestFixture]
	public class LineUtilsTest
	{
		[Test]
		public void TestGetLineListEmptyString()
		{
			List<string> lines = LineUtil.GetLineList("");
			Assert.AreEqual(0, lines.Count);
		}


		[Test]
		public void TestGetLineListSingleLine()
		{
			List<string> lines = LineUtil.GetLineList("hello");
			Assert.AreEqual(1, lines.Count);
		}


		[Test]
		public void TestGetLineListSingleLineWithNewLine()
		{
			List<string> lines = LineUtil.GetLineList("hello\r\n");
			Assert.AreEqual(2, lines.Count);
		}


		[Test]
		public void TestGetLineListSingleLineWithWhitespaceBeforeNewLine()
		{
			List<string> lines = LineUtil.GetLineList("hello    \r\n");
			IEnumerator<string> iter = lines.GetEnumerator();
			iter.MoveNext();
			string line = (string)iter.Current;
			Assert.AreEqual("hello    ", line);
		}


		[Test]
		public void TestGetLineList()
		{
			List<string> lines = null;
			IEnumerator<string> enumerator = null;

			lines = LineUtil.GetLineList("");
			Assert.AreEqual(0, lines.Count);

			lines = LineUtil.GetLineList("hello\r\nworld\r\n");
			Assert.AreEqual(3, lines.Count);
			enumerator = lines.GetEnumerator();
			enumerator.MoveNext();
			Assert.AreEqual("hello", (string)enumerator.Current);
			enumerator.MoveNext();
			Assert.AreEqual("world", (string)enumerator.Current);
			enumerator.MoveNext();
			Assert.AreEqual("", (string)enumerator.Current);
		}

		[Test]
		public void TestGetEmptyListsDifference()
		{
			List<string> list1 = new List<string>();
			List<string> list2 = new List<string>();
			List<string> diff = LineUtil.GetLineListsDifference(list1, list2);
			Assert.IsNotNull(diff);
			Assert.AreEqual(0, diff.Count);
		}

		[Test]
		public void TestGetNonEmptyListsDifference()
		{
			List<string> list1 = new List<string>();
			list1.Add("hello");
			list1.Add("hi");
			List<string> list2 = new List<string>();
			list2.Add("hello");
			list2.Add("world");

			List<string> diff = LineUtil.GetLineListsDifference(list1, list2);
			Assert.IsNotNull(diff);
			Assert.AreEqual(1, diff.Count);

			IEnumerator<string> iter = diff.GetEnumerator();
			iter.MoveNext();
			Assert.AreEqual("world", (string) iter.Current);
		}
	}
}
