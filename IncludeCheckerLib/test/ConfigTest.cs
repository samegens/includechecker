using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace DevPal.IncludeChecker
{
	[TestFixture]
	public class ConfigTest
	{
		[Test]
		public void TestParseValidSettings()
		{
			string config_string = @"<devpal>
	<includechecker>
		<settings>
			<ctags_path>d:\SAM\projects\IncludeAnalyzer\IncludeChecker\ctags\ctags.exe</ctags_path>
			<include_path>z:\dev\main\Code\PIGS</include_path>
			<include_path>Core</include_path>
			<exclude_path>z:\dev\main\Code\ThirdParty</exclude_path>
			<exclude_path>c:\Program Files\Microsoft Visual Studio 8\VC\include</exclude_path>
			<type_alias_prefix>r</type_alias_prefix>
			<type_alias_prefix>rc</type_alias_prefix>
			<type_alias_prefix>rca</type_alias_prefix>
			<type_alias_suffix>Ref</type_alias_suffix>
			<type_alias_suffix>RefC</type_alias_suffix>
			<ignore_header>
				<source>file1.cpp</source>
				<header>header1.h</header>
			</ignore_header>
			<ignore_header>
				<source>file2.cpp</source>
				<header>header2.h</header>
			</ignore_header>
			<ignore_header>
				<header>z:\dev\main\Code\PIGS\PCore\NamespaceOn.h</header>
			</ignore_header>
			<ignore_header>
				<header>z:\dev\main\Code\PIGS\PCore\NamespaceOff.h</header>
			</ignore_header>
			<ignore_header>
				<header>z:\dev\main\Code\PIGS\PCore\PCore.h</header>
			</ignore_header>
			<verbose/>
		</settings>
	</includechecker>
</devpal>
";
			Config config = new Config();
			string error = "";
			Assert.IsTrue(config.Parse(config_string, @"z:\dev\main\Code\", ref error));
			Assert.AreEqual("", error);
			
			Assert.AreEqual(@"d:\SAM\projects\IncludeAnalyzer\IncludeChecker\ctags\ctags.exe", config.CtagsPath);
			
			List<string> include_paths = config.IncludePaths;
			Assert.AreEqual(@"z:\dev\main\Code\PIGS", include_paths[0]);
			Assert.AreEqual(@"z:\dev\main\Code\Core", include_paths[1]);
			
			List<string> exclude_paths = config.ExludePaths;
			Assert.AreEqual(@"z:\dev\main\Code\ThirdParty", exclude_paths[0]);
			Assert.AreEqual(@"c:\Program Files\Microsoft Visual Studio 8\VC\include", exclude_paths[1]);
			
			List<string> type_alias_prefixes = config.TypeAliasPrefixes;
			Assert.AreEqual(@"r", type_alias_prefixes[0]);
			Assert.AreEqual(@"rc", type_alias_prefixes[1]);
			Assert.AreEqual(@"rca", type_alias_prefixes[2]);

			List<string> type_alias_suffixes = config.TypeAliasSuffixes;
			Assert.AreEqual(@"Ref", type_alias_suffixes[0]);
			Assert.AreEqual(@"RefC", type_alias_suffixes[1]);

			List<IncludeChecker.IgnoreHeaderInfo> ignore_infos = config.IgnoreHeaderInfos;
			Assert.AreEqual("file1.cpp", ignore_infos[0].Source);
			Assert.AreEqual("header1.h", ignore_infos[0].Header);
			Assert.AreEqual("file2.cpp", ignore_infos[1].Source);
			Assert.AreEqual("header2.h", ignore_infos[1].Header);
			Assert.IsNull(ignore_infos[2].Source);
			Assert.AreEqual(@"z:\dev\main\Code\PIGS\PCore\NamespaceOn.h", ignore_infos[2].Header);
			Assert.IsNull(ignore_infos[3].Source);
			Assert.AreEqual(@"z:\dev\main\Code\PIGS\PCore\NamespaceOff.h", ignore_infos[3].Header);
			Assert.IsNull(ignore_infos[4].Source);
			Assert.AreEqual(@"z:\dev\main\Code\PIGS\PCore\PCore.h", ignore_infos[4].Header);
			
			Assert.IsTrue(config.Verbose);
		}


		[Test]
		public void TestParseEmptySettings()
		{
			string config_string = @"<devpal>
	<includeusagechecker>
		<settings>
		</settings>
	</includeusagechecker>
</devpal>
";
			Config config = new Config();
			string error = "";
			Assert.IsTrue(config.Parse(config_string, string.Empty, ref error));
			Assert.AreEqual("", error);

			List<string> include_paths = config.IncludePaths;
			Assert.AreEqual(0, include_paths.Count);

			List<string> exclude_paths = config.ExludePaths;
			Assert.AreEqual(0, exclude_paths.Count);
			
			List<string> type_alias_prefixes = config.TypeAliasPrefixes;
			Assert.AreEqual(0, type_alias_prefixes.Count);

			Assert.IsFalse(config.Verbose);
		}


		[Test]
		public void TestParseInvalidSettings()
		{
			string config_string = @"<devpal>
	<includeusagechecker>
		<settings>
			<includepath>z:\dev\main\Code\PIGS</includepath>
			<includepath>z:\dev\main\Code\Core</includepath>
			<ignorepath>z:\dev\main\Code\PIGS\PCore\NamespaceOn.h
";
			Config config = new Config();
			string error = string.Empty;
			Assert.IsFalse(config.Parse(config_string, string.Empty, ref error));
			Assert.AreNotEqual("", error);
		}


		//TODO: test valid xml but not valid according to xsd

	}
}
