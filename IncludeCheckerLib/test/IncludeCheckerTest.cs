using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace DevPal.IncludeChecker
{
	[TestFixture]
	public class IncludeCheckerTest
	{
		[SetUp]
		public void SetUp()
		{
			mConfig = new Config();
			mConfig.CtagsPath = TestUtils.sGetFullPath(@"IncludeChecker\ctags\ctags.exe");
		}
		
		
		[Test]
		public void TestGetNameVariationsWithPrefixes()
		{
			List<string> type_alias_prefixes = new List<string>();
			List<string> type_alias_suffixes = new List<string>();
			type_alias_prefixes.Add("p");
			type_alias_prefixes.Add("pp");
			type_alias_prefixes.Add("r");
			type_alias_prefixes.Add("rp");
			type_alias_prefixes.Add("pcp");
			type_alias_prefixes.Add("rcp");
			type_alias_prefixes.Add("c");
			type_alias_prefixes.Add("rc");
			type_alias_prefixes.Add("pc");
			type_alias_prefixes.Add("ppp");
			type_alias_prefixes.Add("ppc");
			type_alias_prefixes.Add("rpc");
			type_alias_prefixes.Add("pcpc");

			type_alias_prefixes.Add("a");
			type_alias_prefixes.Add("ra");
			type_alias_prefixes.Add("pa");
			type_alias_prefixes.Add("ca");
			type_alias_prefixes.Add("rca");
			type_alias_prefixes.Add("pca");
			type_alias_prefixes.Add("rcac");
			type_alias_prefixes.Add("pcac");

			type_alias_prefixes.Add("ap");
			type_alias_prefixes.Add("rap");
			type_alias_prefixes.Add("pap");
			type_alias_prefixes.Add("apc");
			type_alias_prefixes.Add("rapc");
			type_alias_prefixes.Add("papc");
			type_alias_prefixes.Add("cap");
			type_alias_prefixes.Add("rcap");
			type_alias_prefixes.Add("rcapc");
			type_alias_prefixes.Add("pcap");

			List<string> variations = IncludeChecker.GetNameVariations("RTTIReader", type_alias_prefixes, type_alias_suffixes);
			Assert.IsTrue(variations.Contains("RTTIReader"));
			Assert.IsTrue(variations.Contains("ppRTTIReader"));
			Assert.IsTrue(variations.Contains("rRTTIReader"));
			Assert.IsTrue(variations.Contains("rpRTTIReader"));
			Assert.IsTrue(variations.Contains("pcpRTTIReader"));
			Assert.IsTrue(variations.Contains("rcpRTTIReader"));
			Assert.IsTrue(variations.Contains("cRTTIReader"));
			Assert.IsTrue(variations.Contains("rcRTTIReader"));
			Assert.IsTrue(variations.Contains("pcRTTIReader"));
			Assert.IsTrue(variations.Contains("pppRTTIReader"));
			Assert.IsTrue(variations.Contains("ppcRTTIReader"));
			Assert.IsTrue(variations.Contains("rpcRTTIReader"));
			Assert.IsTrue(variations.Contains("pcpcRTTIReader"));
			Assert.IsTrue(variations.Contains("aRTTIReader"));
			Assert.IsTrue(variations.Contains("raRTTIReader"));
			Assert.IsTrue(variations.Contains("paRTTIReader"));
			Assert.IsTrue(variations.Contains("caRTTIReader"));
			Assert.IsTrue(variations.Contains("rcaRTTIReader"));
			Assert.IsTrue(variations.Contains("pcaRTTIReader"));
			Assert.IsTrue(variations.Contains("rcacRTTIReader"));
			Assert.IsTrue(variations.Contains("pcacRTTIReader"));
			Assert.IsTrue(variations.Contains("apRTTIReader"));
			Assert.IsTrue(variations.Contains("rapRTTIReader"));
			Assert.IsTrue(variations.Contains("papRTTIReader"));
			Assert.IsTrue(variations.Contains("apcRTTIReader"));
			Assert.IsTrue(variations.Contains("rapcRTTIReader"));
			Assert.IsTrue(variations.Contains("papcRTTIReader"));
			Assert.IsTrue(variations.Contains("capRTTIReader"));
			Assert.IsTrue(variations.Contains("rcapRTTIReader"));
			Assert.IsTrue(variations.Contains("rcapcRTTIReader"));
			Assert.IsTrue(variations.Contains("pcapRTTIReader"));
		}
		
		
		[Test]
		public void TestGetNameVariationsWithPrefixAndSuffix()
		{
			List<string> type_alias_prefixes = new List<string>();
			type_alias_prefixes.Add("p");
			type_alias_prefixes.Add("a");

			List<string> type_alias_suffixes = new List<string>();
			type_alias_suffixes.Add("Ref");
			type_alias_suffixes.Add("RefC");

			List<string> variations = IncludeChecker.GetNameVariations("RTTIReader", type_alias_prefixes, type_alias_suffixes);
			Assert.IsTrue(variations.Contains("pRTTIReader"));
			Assert.IsTrue(variations.Contains("aRTTIReader"));
			Assert.IsTrue(variations.Contains("RTTIReaderRef"));
			Assert.IsTrue(variations.Contains("RTTIReaderRefC"));
			Assert.IsTrue(variations.Contains("pRTTIReaderRef"));
			Assert.IsTrue(variations.Contains("pRTTIReaderRefC"));
			Assert.IsTrue(variations.Contains("aRTTIReaderRef"));
			Assert.IsTrue(variations.Contains("aRTTIReaderRefC"));
		}
		
		
		[Test]
		public void TestUsedHeaders()
		{
			IncludeChecker checker = new IncludeChecker(new TestOutput(), mConfig,
					TestUtils.sGetFullPath(@"IncludeCheckerLib\test\UsedHeaders\sourcefile.cpp"), "",
					0);
			checker.CheckIncludes();
			Assert.AreEqual(0, checker.NrWarnings);
			List<IncludeChecker.UnusedHeaderResult> results = checker.Results;
			Assert.AreEqual(0, results.Count);
		}
		
		
		[Test]
		public void TestTypeAliasPrefix()
		{
			mConfig.AddTypeAliasPrefix("ap");
			mConfig.AddTypeAliasPrefix("a");
			IncludeChecker checker = new IncludeChecker(new TestOutput(), mConfig, 
					TestUtils.sGetFullPath(@"IncludeCheckerLib\test\TypeAliasPrefix\sourcefile.cpp"), "",
					0);
			checker.CheckIncludes();
			Assert.AreEqual(0, checker.NrWarnings);
			List<IncludeChecker.UnusedHeaderResult> results = checker.Results;
			Assert.AreEqual(0, results.Count);
		}


		[Test]
		public void TestTypeAliasSuffix()
		{
			mConfig.AddTypeAliasPrefix("a");
			mConfig.AddTypeAliasSuffix("Ref");
			IncludeChecker checker = new IncludeChecker(new TestOutput(), mConfig,
					TestUtils.sGetFullPath(@"IncludeCheckerLib\test\TypeAliasSuffix\sourcefile.cpp"), "",
					0);
			checker.CheckIncludes();
			Assert.AreEqual(0, checker.NrWarnings);
			List<IncludeChecker.UnusedHeaderResult> results = checker.Results;
			Assert.AreEqual(0, results.Count);
		}
		
		
		[Test]
		public void TestIgnoreUnusedHeader()
		{
			mConfig.AddIgnoreHeaderInfo(new IncludeChecker.IgnoreHeaderInfo("sourcefile.cpp", "unused.h"));
			mConfig.AddIgnoreHeaderInfo(new IncludeChecker.IgnoreHeaderInfo(null, "unused2.h"));
			mConfig.AddIgnoreHeaderInfo(new IncludeChecker.IgnoreHeaderInfo("otherfile.cpp", "unused3.h"));
			IncludeChecker checker = new IncludeChecker(new TestOutput(), mConfig,
					TestUtils.sGetFullPath(@"IncludeCheckerLib\test\IgnoreUnusedHeaders\sourcefile.cpp"), "",
					0);
			checker.CheckIncludes();
			Assert.AreEqual(0, checker.NrWarnings);
			List<IncludeChecker.UnusedHeaderResult> results = checker.Results;
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(TestUtils.sGetFullPath(@"IncludeCheckerLib\test\IgnoreUnusedHeaders\sourcefile.cpp"), results[0].SourceFile);
			Assert.AreEqual(TestUtils.sGetFullPath(@"IncludeCheckerLib\test\IgnoreUnusedHeaders\unused3.h"), results[0].HeaderFile);
		}


		[Test]
		public void TestIgnorePathOnSource()
		{
			mConfig.AddIgnorePath(TestUtils.sGetFullPath(@"IncludeCheckerLib\test\IgnoreUnusedHeaders\sourcefile.cpp"));
			IncludeChecker checker = new IncludeChecker(new TestOutput(), mConfig,
					TestUtils.sGetFullPath(@"IncludeCheckerLib\test\IgnoreUnusedHeaders\sourcefile.cpp"), "",
					0);
			checker.CheckIncludes();
			Assert.AreEqual(0, checker.NrWarnings);
			List<IncludeChecker.UnusedHeaderResult> results = checker.Results;
			Assert.AreEqual(0, results.Count);
		}


		[Test]
		public void TestIgnorePathOnHeader()
		{
			mConfig.AddIgnorePath(TestUtils.sGetFullPath(@"IncludeCheckerLib\test\IgnoreUnusedHeaders\unused.h"));
			mConfig.AddIgnorePath(TestUtils.sGetFullPath(@"IncludeCheckerLib\test\IgnoreUnusedHeaders\unused2.h"));
			mConfig.AddIgnorePath(TestUtils.sGetFullPath(@"IncludeCheckerLib\test\IgnoreUnusedHeaders\unused3.h"));
			IncludeChecker checker = new IncludeChecker(new TestOutput(), mConfig,
					TestUtils.sGetFullPath(@"IncludeCheckerLib\test\IgnoreUnusedHeaders\sourcefile.cpp"), "",
					0);
			checker.CheckIncludes();
			Assert.AreEqual(0, checker.NrWarnings);
			List<IncludeChecker.UnusedHeaderResult> results = checker.Results;
			Assert.AreEqual(0, results.Count);
		}


		[Test]
		public void TestIgnorePathOnNonExistentHeaderAndSource()
		{
			mConfig.AddIgnoreHeaderInfo(new IncludeChecker.IgnoreHeaderInfo("sourcefile.cpp", "unknown.h"));
			IncludeChecker checker = new IncludeChecker(new TestOutput(), mConfig,
					TestUtils.sGetFullPath(@"IncludeCheckerLib\test\IgnoreNonExistentHeader\sourcefile.cpp"), "",
					0);
			checker.CheckIncludes();
			Assert.AreEqual(0, checker.NrWarnings);
			List<IncludeChecker.UnusedHeaderResult> results = checker.Results;
			Assert.AreEqual(0, results.Count);
		}


		[Test]
		public void TestUnusedHeaders()
		{
			IncludeChecker checker = new IncludeChecker(new TestOutput(), mConfig,
					TestUtils.sGetFullPath(@"IncludeCheckerLib\test\UnusedHeaders\sourcefile.cpp"), "",
					0);
			checker.CheckIncludes();
			Assert.AreEqual(0, checker.NrWarnings);
			List<IncludeChecker.UnusedHeaderResult> results = checker.Results;
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(TestUtils.sGetFullPath(@"IncludeCheckerLib\test\UnusedHeaders\sourcefile.cpp"), results[0].SourceFile);
			Assert.AreEqual(TestUtils.sGetFullPath(@"IncludeCheckerLib\test\UnusedHeaders\header.h"), results[0].HeaderFile);
		}
		
		
		[Test]
		public void TestUnusedHeaderAfterHeaderWithoutTags()
		{
			IncludeChecker checker = new IncludeChecker(new TestOutput(), mConfig,
					TestUtils.sGetFullPath(@"IncludeCheckerLib\test\UnusedHeaders\sourcefile2.cpp"), "",
					0);
			checker.CheckIncludes();
			Assert.AreEqual(0, checker.NrWarnings);
			List<IncludeChecker.UnusedHeaderResult> results = checker.Results;
			Assert.AreEqual(2, results.Count);
			Assert.AreEqual(TestUtils.sGetFullPath(@"IncludeCheckerLib\test\UnusedHeaders\sourcefile2.cpp"), results[0].SourceFile);
			Assert.AreEqual(TestUtils.sGetFullPath(@"IncludeCheckerLib\test\UnusedHeaders\onlyincludes.h"), results[0].HeaderFile);
			Assert.AreEqual(TestUtils.sGetFullPath(@"IncludeCheckerLib\test\UnusedHeaders\sourcefile2.cpp"), results[1].SourceFile);
			Assert.AreEqual(TestUtils.sGetFullPath(@"IncludeCheckerLib\test\UnusedHeaders\header.h"), results[1].HeaderFile);
		}


        [Test]
        public void TestInterfaceHeaderNotConfigured()
        {
            IncludeChecker checker = new IncludeChecker(new TestOutput(), mConfig,
                    TestUtils.sGetFullPath(@"IncludeCheckerLib\test\InterfaceHeaders\includeinterfaceheader.cpp"), "",
                    0);
            checker.CheckIncludes();
            List<IncludeChecker.UnusedHeaderResult> results = checker.Results;
            Assert.AreEqual(1, results.Count);
        }


        [Test]
        public void TestInterfaceHeaderConfigured()
        {
            mConfig.InterfaceHeaders.Add("interfaceheader.h");
            IncludeChecker checker = new IncludeChecker(new TestOutput(), mConfig,
                    TestUtils.sGetFullPath(@"IncludeCheckerLib\test\InterfaceHeaders\includeinterfaceheader.cpp"), "",
                    0);
            checker.CheckIncludes();
            List<IncludeChecker.UnusedHeaderResult> results = checker.Results;
            Assert.AreEqual(0, results.Count);
        }

		///////////////////////////////
		
		private class TestOutput: IncludeChecker.IOutput
		{
			public void WriteLine(string inString)
			{
			}
			
			
			public void WriteError(string inString)
			{
			}
		}
		

		private Config mConfig;
	}
}
