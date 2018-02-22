using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using TownleyEnterprises.Command;

using DevPal.CSLib;
using DevPal.CSLib.CppParsing;
using Microsoft.Win32;
using System.Xml;


namespace DevPal.IncludeChecker
{
    /// <summary>
    /// Main application, implementing the command line version of IncludeChecker.
    /// </summary>
	class CommandLineIncludeChecker: AbstractCommandListener, IncludeChecker.IOutput
	{
        /// <summary>
        /// Application entry point
        /// </summary>
		static void Main(string[] inCommandLineArguments)
		{
			Console.WriteLine(IncludeChecker.GetApplicationHeader());

			CommandLineIncludeChecker checker = new CommandLineIncludeChecker();
			if (checker.Check(inCommandLineArguments))
				Environment.Exit(0);
			else
				Environment.Exit(1);
		}


		/// <summary>
		/// Run the check with all command line arguments.
		/// </summary>
		public bool Check(string[] inCommandLine)
		{
			ParseCommandLine(inCommandLine);

			IncludeChecker checker = new IncludeChecker(this, mConfig, mStartFiles, mStartDir, mNrWarnings);
			checker.CheckIncludes();
			
			if (!string.IsNullOrEmpty(mXmlOutputPath))
			{
				List<IncludeChecker.UnusedHeaderResult> results = checker.Results;
				WriteResultsToXml(results, mXmlOutputPath);
			}

			return checker.Results.Count == 0;
		}
		
		
		/// <summary>
		/// Write the list of unused headers to an XML file.
		/// </summary>
		private void WriteResultsToXml(List<IncludeChecker.UnusedHeaderResult> inResults, string inPath)
		{
			XmlDocument doc = new XmlDocument();
			XmlElement root = doc.CreateElement("unused_headers");
			doc.AppendChild(root);
			
			foreach (IncludeChecker.UnusedHeaderResult result in inResults)
			{
				XmlElement result_element = doc.CreateElement("unused_header");

				XmlElement source_path_element = doc.CreateElement("source");
				source_path_element.AppendChild(doc.CreateTextNode(result.SourceFile));
				result_element.AppendChild(source_path_element);
				
				XmlElement source_line_element = doc.CreateElement("line");
				source_line_element.AppendChild(doc.CreateTextNode(result.SourceLine.ToString()));
				result_element.AppendChild(source_line_element);
				
				XmlElement header_element = doc.CreateElement("header");
				header_element.AppendChild(doc.CreateTextNode(result.HeaderFile));
				result_element.AppendChild(header_element);
				
				root.AppendChild(result_element);
			}

			doc.Save(inPath);
		}


		////////////////////// Command line parsing //////////////////////////

		static CommandOption sConfigFileOption = new CommandOption("configuration", 'c', true, "<file>", "specify configuration file (optional)");
		static CommandOption sVersionOption = new CommandOption("version", 'v', false, "", "show version information");
		static CommandOption sVerboseOption = new CommandOption("verbose", 'V', false, "verbose", "show more output");
		static CommandOption sDirectoryOption = new CommandOption("directory", 'D', true, "<directory>", "specify directory to check (recursively)", null);
		static CommandOption sXmlOutputOption = new CommandOption("xmloutput", 'x', true, "<file>", "specify file for xml output", null);
		static RepeatableCommandOption sIncludeOption = new RepeatableCommandOption("includepath", 'I', "<directory>", "specify include path, multiple include paths can be given", true, null);
		static RepeatableCommandOption sExcludeOption = new RepeatableCommandOption("excludepath", 'E', "<file or directory>", "specify header file or directory to ignore, multiple exclude paths can be given", true, null);
        static RepeatableCommandOption sInterfaceHeaderOption = new RepeatableCommandOption("interfaceheader", 'i', "<file>", "mark header file as interface header, multiple interface headers can be specified", true, null);
        static RepeatableCommandOption sTypeAliasPrefixOption = new RepeatableCommandOption("type_alias_prefix", 't', "<string>", "specify a prefix for type aliases", true, null);
		static RepeatableCommandOption sTypeAliasSuffixOption = new RepeatableCommandOption("type_alias_suffix", 'T', "<string>", "specify a suffix for type aliases", true, null);
		static CommandOption[] sCommandLineOptions = new CommandOption[] {
			sConfigFileOption,
			sVersionOption,
			sVerboseOption,
			sDirectoryOption,
			sXmlOutputOption,
			sIncludeOption,
			sExcludeOption,
            sInterfaceHeaderOption,
			sTypeAliasPrefixOption,
			sTypeAliasSuffixOption
		};


		/// <summary>
		/// Description of command line options.
		/// </summary>
		public override string Description
		{
			get { return "Options"; }
		}

		
		/// <summary>
		/// Command line options.
		/// </summary>
		public override CommandOption[] Options
		{
			get { return sCommandLineOptions; }
		}
 
 
		/// <summary>
		/// Parse the command line.
		/// </summary>
		public void ParseCommandLine(string[] inCommandLine)
		{
			CommandParser parser = new CommandParser("IncludeChecker", "[<file> ...]");
			parser.AddCommandListener(this);
			parser.SetExitOnMissingArg(true, 1);
			parser.SetExtraHelpText("", @"Example:
IncludeChecker -I c:\dev\foo\include -I c:\dev\bar\include -t r -t cr
-T Ptr -T Ref test.cpp

When using the --directory or -D option, it's possible to specify a wildcard:
IncludeChecker -D include\*.h -I c:\dev\foo\include

Sample configuration file:

<devpal>
  <includechecker>
    <settings>
      <verbose/>
      <include_path>c:\dev\foo</include_path>
      <include_path>c:\dev\bar</include_path>
      <exclude_path>c:\dev\foo\include\precompiled.h</exclude_path>
      <exclude_path>c:\dev\bar\include\precompiled.h</exclude_path>
      <type_alias_prefix>r</type_alias_prefix>
      <type_alias_prefix>cr</type_alias_prefix>
      <type_alias_suffix>Ptr</type_alias_suffix>
      <type_alias_suffix>Ref</type_alias_suffix>
    </settings>
  </includechecker>
</devpal>");
			if (inCommandLine.Length == 0)
			{
				parser.Help();
				System.Environment.Exit(-1);
			}
			parser.Parse(inCommandLine);
			
			if (sVersionOption.Matched)
			{
				Console.WriteLine("version " + IncludeChecker.Version);
				System.Environment.Exit(0);
			}

			if (sConfigFileOption.Matched)
			{
				string config_path = (string)sConfigFileOption.ArgValue;
				ReadConfigFile(config_path);
			}

			if (sVerboseOption.Matched)
			{
				// Command line overrides config file
				mConfig.Verbose = true;
			}

			if (parser.UnhandledArguments.Length == 0 && !sDirectoryOption.Matched)
			{
				WriteError("Error: no file or directory specified.");
				System.Environment.Exit(-1);
			}

			if (sDirectoryOption.Matched)
			{
				mStartDir = (string)sDirectoryOption.ArgValue;
				Verbose("Directory to check: " + mStartDir);
			}
			
			if (sXmlOutputOption.Matched)
			{
				mXmlOutputPath = (string)sXmlOutputOption.ArgValue;
				Verbose("Writing XML output to: " + mXmlOutputPath);
			}

			if (sIncludeOption.Matched)
			{
				foreach (string path in sIncludeOption.GetArgs())
				{
					if (!System.IO.Directory.Exists(path))
					{
						WriteLine("Warning: include directory " + path + " does not exist!");
						++mNrWarnings;
					}
					else
					{
						mConfig.AddIncludePath(path);
						Verbose("Command line: added include path " + path);
					}
				}
			}

			if (sExcludeOption.Matched)
			{
				foreach (string path in sExcludeOption.GetArgs())
				{
					if (File.Exists(path) || Directory.Exists(path))
					{
						mConfig.AddIgnorePath(path);
						Verbose("Command line: added exclude path " + path);
					}
					else
					{
						WriteError("Error: exclude path " + path + " does not exist!");
						System.Environment.Exit(-1);
					}
				}
			}

            if (sInterfaceHeaderOption.Matched)
            {
                foreach (string path in sInterfaceHeaderOption.GetArgs())
                {
                    mConfig.InterfaceHeaders.Add(path);
                    Verbose("Command line: added interface header " + path);
                }
            }
			
			if (sTypeAliasPrefixOption.Matched)
			{
				foreach (string prefix in sTypeAliasPrefixOption.GetArgs())
				{
					mConfig.AddTypeAliasPrefix(prefix);
					Verbose("Command line: added type alias prefix '" + prefix + "'");
				}
			}

			if (sTypeAliasSuffixOption.Matched)
			{
				foreach (string suffix in sTypeAliasSuffixOption.GetArgs())
				{
					mConfig.AddTypeAliasSuffix(suffix);
					Verbose("Command line: added type alias suffix '" + suffix + "'");
				}
			}

			if (string.IsNullOrEmpty(mConfig.CtagsPath))
				mConfig.CtagsPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ctags\ctags.exe";
			Verbose("    ctags path is " + mConfig.CtagsPath);

			if (parser.UnhandledArguments.Length > 0)
			{
				foreach (string path in parser.UnhandledArguments)
				{
					if (File.Exists(path))
					{
						Verbose("File to check: " + path);
						mStartFiles.Add(path);
					}
					else
					{
						string error = "Error: " + path + " is not a file.";
						if (Directory.Exists(path))
							error += " Please use --directory or -D (this will allow wildcards).";
						WriteError(error);
						System.Environment.Exit(-1);
					}
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		// IncludeChecker.IOutput implementation

		public void WriteLine(string inOutput)
		{
			System.Console.WriteLine(inOutput);
		}


		public void WriteError(string inError)
		{
			System.Console.Error.WriteLine(inError);
		}

		//////////////////////////////////////////////////////////////////////////////
		// Helpers

		/// <summary>
		/// Read the configuration file.
		/// </summary>
		private void ReadConfigFile(string inPath)
		{
			if (!File.Exists(inPath))
			{
				WriteError("Error: configuration file " + inPath + " does not exist.");
				System.Environment.Exit(-1);
			}

			Verbose("Reading configuration from file " + inPath);
			string config_contents = FileUtil.ReadTextFromFile(inPath);
			string base_path = Path.GetDirectoryName(inPath);
			if (!base_path.EndsWith(Path.DirectorySeparatorChar))
				base_path += Path.DirectorySeparatorChar;
			string error = "";
			if (!mConfig.Parse(config_contents, base_path, ref error))
			{
				WriteError("Error: could not read configuration file " + inPath + ": " + error);
				System.Environment.Exit(-1);
			}

			foreach (string path in mConfig.IncludePaths)
			{
				if (!System.IO.Directory.Exists(path))
				{
					WriteLine("Warning: include directory " + path + " does not exist!");
					++mNrWarnings;
				}
				else
				{
					Verbose("    added include path " + path);
				}
			}

			foreach (string path in mConfig.ExludePaths)
			{
				if (File.Exists(path) || Directory.Exists(path))
				{
					Verbose("    added exclude path " + path);
				}
				//SAM: I think the exclude doesn't have to exist. For example when encountering a hard-coded include like /usr/include/malloc.h
				// it probably doesn't exist on Windows, but we want to be able to ignore it.
				//else
				//{
				//    WriteError("Error: exclude path " + path + " does not exist!");
				//    System.Environment.Exit(-1);
				//}
			}

			foreach (string prefix in mConfig.TypeAliasPrefixes)
			{
				Verbose("    added type alias prefix '" + prefix + "'");
			}

			if (mConfig.IgnoreHeaderInfos != null)
			{
				foreach (IncludeChecker.IgnoreHeaderInfo ignore_header in mConfig.IgnoreHeaderInfos)
				{
					string header = "<empty>";
					if (!string.IsNullOrEmpty(ignore_header.Header))
						header = ignore_header.Header;

					string source = "<none>";
					if (!string.IsNullOrEmpty(ignore_header.Source))
						source = ignore_header.Source;

					Verbose("    ignoring header " + header + " from source " + source);
				}
			}
		}


		private void Verbose(string inOutput)
		{
			if (mConfig.Verbose)
				WriteLine(inOutput);
		}


		//////////////////////////////////////////////////////////////////////////////
		
		private Config mConfig = new Config();
		private string mStartDir = string.Empty;
		private string mXmlOutputPath = string.Empty;
		private List<string> mStartFiles = new List<string>();
		private int mNrWarnings = 0;
	}
}
