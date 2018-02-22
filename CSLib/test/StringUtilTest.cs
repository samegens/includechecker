using System;
using NUnit.Framework;
using DevPal.CSLib;

namespace DevPal.CSLib.Tests
{
	[TestFixture]
	public class StringUtilTest
	{
		[Test]
		public void TestWildCardCompare()
		{
			Assert.IsTrue(StringUtil.WildCardCompare("", ""));
			Assert.IsTrue(StringUtil.WildCardCompare("*", ""));
			Assert.IsTrue(StringUtil.WildCardCompare("?", " "));
			Assert.IsFalse(StringUtil.WildCardCompare("?", ""));
			Assert.IsTrue(StringUtil.WildCardCompare("*.obj", "hello.obj"));
			Assert.IsFalse(StringUtil.WildCardCompare("*.obj", "hello.object"));
			Assert.IsTrue(StringUtil.WildCardCompare(",*", ",backup"));
			Assert.IsFalse(StringUtil.WildCardCompare(",*", ".,backup"));
			Assert.IsTrue(StringUtil.WildCardCompare("hello", "hello"));
			Assert.IsFalse(StringUtil.WildCardCompare("hello", "hello "));
			Assert.IsFalse(StringUtil.WildCardCompare("hello", " hello"));
		}

		[Test]
		public void TestParseBeginOfStringAsIntEmptyString()
		{
			Assert.AreEqual(-1, StringUtil.ParseBeginOfStringAsInt(""));
		}

		[Test]
		public void TestParseBeginOfStringAsIntNonIntString()
		{
			Assert.AreEqual(-1, StringUtil.ParseBeginOfStringAsInt("hello world"));
		}

		[Test]
		public void TestParseBeginOfStringAsIntAllDigits()
		{
			Assert.AreEqual(0, StringUtil.ParseBeginOfStringAsInt("0"));
			Assert.AreEqual(1, StringUtil.ParseBeginOfStringAsInt("1"));
			Assert.AreEqual(2, StringUtil.ParseBeginOfStringAsInt("2"));
			Assert.AreEqual(3, StringUtil.ParseBeginOfStringAsInt("3"));
			Assert.AreEqual(4, StringUtil.ParseBeginOfStringAsInt("4"));
			Assert.AreEqual(5, StringUtil.ParseBeginOfStringAsInt("5"));
			Assert.AreEqual(6, StringUtil.ParseBeginOfStringAsInt("6"));
			Assert.AreEqual(7, StringUtil.ParseBeginOfStringAsInt("7"));
			Assert.AreEqual(8, StringUtil.ParseBeginOfStringAsInt("8"));
			Assert.AreEqual(9, StringUtil.ParseBeginOfStringAsInt("9"));
		}

		[Test]
		public void TestParseBeginOfStringAsIntDigitsAndRubbish()
		{
			Assert.AreEqual(1024, StringUtil.ParseBeginOfStringAsInt("1024helloworld"));
		}

		[Test]
		public void TestEndsWithChar()
		{
			Assert.AreEqual(false, "".EndsWith('a'));
			Assert.AreEqual(false, "ab".EndsWith('a'));
			Assert.AreEqual(true, "ab".EndsWith('b'));
		}
	}
}
