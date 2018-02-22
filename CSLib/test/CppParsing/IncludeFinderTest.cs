using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace DevPal.CSLib.CppParsing
{
	public class TestIncludeFinder: IncludeFinder
	{
		public TestIncludeFinder(List<string> inIncludePaths) :
			base(inIncludePaths)
		{
		}


		public override List<Include> GetIncludes(string inFilePath)
		{
			List<Include> includes = new List<Include>();

			switch (inFilePath.ToLower())
			{
				case @"d:\dev\core\a.h":
					includes.Add(new Include(@"d:\dev\Core\b.h", 1, false));
					includes.Add(new Include(@"d:\dev\Core\c.h", 2, false));
					break;
				case @"d:\dev\core\b.h":
					includes.Add(new Include(@"d:\dev\Core\d.h", 1, false));
					break;
				case @"d:\dev\core\c.h":
					includes.Add(new Include(@"d:\dev\Core\e.h", 1, false));
					break;
				case @"d:\dev\core\d.h":
				case @"d:\dev\core\e.h":
					break;

				case @"d:\dev\core\cyclic\a.h":
					includes.Add(new Include(@"d:\dev\Core\cyclic\b.h", 1, false));
					break;
				case @"d:\dev\core\cyclic\b.h":
					includes.Add(new Include(@"d:\dev\Core\cyclic\a.h", 1, false));
					break;

				default:
					Assert.Fail();
					break;
			}
			
			return includes;
		}

	}

	[TestFixture]
	public class IncludeFinderTest
	{
		private TestIncludeFinder mTestIncludeFinder;

		[OneTimeSetUp]
		public void SetupFixture()
		{
			List<string> include_paths = new List<string>();
			include_paths.Add(@"d:\dev\Core");
			include_paths.Add(@"d:\dev\Pigs");
			mTestIncludeFinder = new TestIncludeFinder(include_paths);
		}


		[Test]
		public void TestIsInclude()
		{
			Assert.IsTrue(IncludeFinder.IsInclude("#include <header.h>"));
			Assert.IsTrue(IncludeFinder.IsInclude(" #include \"header.h\""));
			Assert.IsTrue(IncludeFinder.IsInclude("\t#include \"header.h\""));
			Assert.IsFalse(IncludeFinder.IsInclude(""));
			Assert.IsFalse(IncludeFinder.IsInclude("include <header.h>"));
		}


		[Test]
		public void TestGetIncludePath()
		{
			Assert.AreEqual("header.h", IncludeFinder.GetIncludePath("#include <header.h>"));
			Assert.AreEqual("header.h", IncludeFinder.GetIncludePath("#include \"header.h\""));
		}


		[Test]
		public void TestGetNativePath()
		{
			Assert.AreEqual(@"Core\CCore", IncludeFinder.GetNativePath(@"Core\CCore"));
		}


		[Test]
		public void TestIsSystemInclude()
		{
			Assert.IsTrue(IncludeFinder.IsSystemInclude("#include <header.h>"));
			Assert.IsFalse(IncludeFinder.IsSystemInclude("#include \"header.h\""));
		}


		[Test]
		public void TestGetPossibleHeaderFilesSystemHeader()
		{
			List<string> include_paths = new List<string>();
			include_paths.Add(@"d:\dev\main\Code\Core");
			include_paths.Add(@"d:\dev\main\Code\Pigs");
			List<string> possible_headers = IncludeFinder.GetPossibleHeaderFiles(include_paths, 
					@"d:\dev\main\Code\Pigs\Test\header.h", new Include("somedir/otherheader.h", 1, true));
			Assert.AreEqual(2, possible_headers.Count);
			Assert.AreEqual(@"d:\dev\main\Code\Core\somedir\otherheader.h", possible_headers[0]);
			Assert.AreEqual(@"d:\dev\main\Code\Pigs\somedir\otherheader.h", possible_headers[1]);
		}

	
		[Test]
		public void TestGetPossibleHeaderFilesLocalHeader()
		{
			List<string> include_paths = new List<string>();
			include_paths.Add(@"d:\dev\main\Code\Core");
			include_paths.Add(@"d:\dev\main\Code\Pigs");
			List<string> possible_headers = IncludeFinder.GetPossibleHeaderFiles(include_paths, 
				@"d:\dev\main\Code\Pigs\Test\header.h", new Include("somedir/otherheader.h", 1, false));
			Assert.AreEqual(3, possible_headers.Count);
			Assert.AreEqual(@"d:\dev\main\Code\Pigs\Test\somedir\otherheader.h", possible_headers[0]);
			Assert.AreEqual(@"d:\dev\main\Code\Core\somedir\otherheader.h", possible_headers[1]);
			Assert.AreEqual(@"d:\dev\main\Code\Pigs\somedir\otherheader.h", possible_headers[2]);
		}


		[Test]
		public void TestGetIncludesRecursively()
		{
			List<string> all_includes = mTestIncludeFinder.GetIncludesRecursively(@"d:\dev\Core\a.h");
			Assert.AreEqual(4, all_includes.Count);
			Assert.IsTrue(all_includes.Contains(@"d:\dev\Core\b.h"));
			Assert.IsTrue(all_includes.Contains(@"d:\dev\Core\c.h"));
			Assert.IsTrue(all_includes.Contains(@"d:\dev\Core\d.h"));
			Assert.IsTrue(all_includes.Contains(@"d:\dev\Core\e.h"));
		}


		[Test]
		public void TestGetIncludesRecursivelyCyclic()
		{
			List<string> all_includes = mTestIncludeFinder.GetIncludesRecursively(@"d:\dev\Core\cyclic\a.h");
			Assert.AreEqual(1, all_includes.Count);
			Assert.IsTrue(all_includes.Contains(@"d:\dev\Core\cyclic\b.h"));
		}


		[Test]
		public void TestGetRawIncludes()
		{
			IncludeFinder finder = new IncludeFinder(null);
			string source = @"// #include <blah/blah.h>

#include <lib/include.h>

#include ""syslib/sysinclude.h""

";
			List<Include> includes = finder.GetRawIncludes(source);
			Assert.AreEqual("lib/include.h", includes[0].Path);
			Assert.AreEqual(3, includes[0].Line);
			Assert.AreEqual("syslib/sysinclude.h", includes[1].Path);
			Assert.AreEqual(5, includes[1].Line);
		}
	}
}
