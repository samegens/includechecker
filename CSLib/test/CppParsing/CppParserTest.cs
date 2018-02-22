using System;
using NUnit.Framework;
using DevPal.CSLib.CppParsing;

namespace DevPal.CSLib.Tests.CppParsing
{
	[TestFixture]
	public class CppParserTest
	{
		[Test]
		public void TestRemoveIncludes()
		{
			CppParser parser = new CppParser();
			string contents = @"// #include ""hello.h""
#include ""hello.h""

class hello {
};
";
			parser.RemoveIncludes(ref contents);
			string expected_contents = @"// #include ""hello.h""


class hello {
};
";
			Assert.AreEqual(expected_contents, contents);
		}

		[Test]
		public void TestRemoveComments()
		{
			Assert.IsTrue(CheckRemoveComments("", ""));
			Assert.IsTrue(CheckRemoveComments("\r\n", "\r\n"));
			Assert.IsTrue(CheckRemoveComments("// this is comment", ""));
			Assert.IsTrue(CheckRemoveComments("hello world// this is comment", "hello world"));
			Assert.IsTrue(CheckRemoveComments("hello// this is comment\r\nworld", "hello\r\nworld"));
			Assert.IsTrue(CheckRemoveComments("hello world/", "hello world/"));
			Assert.IsTrue(CheckRemoveComments("\"hello // world\"", "\"hello // world\""));
			Assert.IsTrue(CheckRemoveComments("\"hello \\\" //world\"", "\"hello \\\" //world\""));
			Assert.IsTrue(CheckRemoveComments("'\"' // comment", "'\"' "));
			Assert.IsTrue(CheckRemoveComments("// /*", ""));

			Assert.IsTrue(CheckRemoveComments("hello /* comment */ world", "hello               world"));
			Assert.IsTrue(CheckRemoveComments("hello/*comment\r\n*/world", "hello         \r\n  world"));
			Assert.IsTrue(CheckRemoveComments("hello /* \" */ world", "hello         world"));
			Assert.IsTrue(CheckRemoveComments("hello /* // */ world", "hello          world"));
		}
		
		
		[Test]
		public void TestRemoveStrings()
		{
			Assert.IsTrue(CheckRemoveStrings("", ""));
			Assert.IsTrue(CheckRemoveStrings("\"\"", ""));
			Assert.IsTrue(CheckRemoveStrings("\"hello\"\"world\"", ""));
			Assert.IsTrue(CheckRemoveStrings("\"\\\"hello\\\"\"", ""));
			Assert.IsTrue(CheckRemoveStrings("\"\\\"\"hello", "hello"));	// "\""hello
			Assert.IsTrue(CheckRemoveStrings("\"\\\\\"hello", "hello"));	// "\\"hello
			Assert.IsTrue(CheckRemoveStrings("'\\\"'", ""));				// '\"'
		}


		private bool CheckRemoveComments(string inInput, string inExpectedOutput)
		{
			string contents = inInput;
			CppParser parser = new CppParser();
			parser.RemoveComments(ref contents);
			Assert.AreEqual(inExpectedOutput, contents);
			return (inExpectedOutput == contents);
		}
		
		
		private bool CheckRemoveStrings(string inInput, string inExpectedOutput)
		{
			string contents = inInput;
			CppParser parser = new CppParser();
			parser.RemoveStrings(ref contents);
			Assert.AreEqual(inExpectedOutput, contents);
			return (inExpectedOutput == contents);
		}
	}
}
