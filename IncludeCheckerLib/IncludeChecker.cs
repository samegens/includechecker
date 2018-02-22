using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

using DevPal.CSLib;
using DevPal.CSLib.CppParsing;
using System.Reflection;

namespace DevPal.IncludeChecker
{
	public class IncludeChecker
	{
		private const string kApplicationHeader = "IncludeChecker %VERSION% - %COPYRIGHT%";
		
		
        /// <summary>
        /// Get the header to display when the application is started.
        /// </summary>
		public static string GetApplicationHeader()
		{
			string header = kApplicationHeader.Replace("%VERSION%", Version);
			header = header.Replace("%COPYRIGHT%", AssemblyCopyright);
			return header;
		}


		/// <summary>
		/// Interface used for immediate output.
		/// </summary>
		public interface IOutput
		{
			/// <summary>
			/// Write a string to standard output.
			/// </summary>
			/// <param name="inString"></param>
			void WriteLine(string inString);
			
			/// <summary>
			/// Write a string to standard error.
			/// </summary>
			/// <param name="inString"></param>
			void WriteError(string inString);
		}
		
		
		/// <summary>
		/// UnusedHeaderResult stores the result of a single unused header file.
		/// </summary>
		public class UnusedHeaderResult
		{
            /// <summary>
            /// Constructor.
            /// </summary>
			public UnusedHeaderResult(string inSourceFile, int inSourceLine, string inHeaderFile)
			{
				mSourceFile = inSourceFile;
				mSourceLine = inSourceLine;
				mHeaderFile = inHeaderFile;
			}
			
			
            /// <summary>
            /// Full path to the source file.
            /// </summary>
			public string SourceFile
			{
				get { return mSourceFile; }
			}
			
			
            /// <summary>
            /// Line in the source file.
            /// </summary>
			public int SourceLine
			{
				get { return mSourceLine; }
			}
			
			
            /// <summary>
            /// Full path to the included file.
            /// </summary>
			public string HeaderFile
			{
				get { return mHeaderFile; }
			}
			
			
			private string mSourceFile;
			private int mSourceLine;
			private string mHeaderFile;
		}
		
		
        /// <summary>
        /// Info to specify a header to ignore.
        /// </summary>
        /// <remarks>
        /// The source file can be empty, which means that the header file can be ignored always.
        /// </remarks>
		public class IgnoreHeaderInfo
		{
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="inSourceFile">If inSourceFile is null, inHeaderFile will always be ignored.</param>
			public IgnoreHeaderInfo(string inSourceFile, string inHeaderFile)
			{
				mSourceFile = inSourceFile;
				mHeaderFile = inHeaderFile;
			}
			
			
            /// <summary>
            /// Source file.
            /// </summary>
			public string Source
			{
				get { return mSourceFile; }
			}
			
			
            /// <summary>
            /// Header file.
            /// </summary>
			public string Header
			{
				get { return mHeaderFile; }
			}
			
			
			private string mSourceFile;
			private string mHeaderFile;
		}


        /// <summary>
        /// Constructor used to check a single source file.
        /// </summary>
		public IncludeChecker(IOutput inOutput, Config inConfig, string inStartFile, string inStartDir, int inNrWarnings) :
            this(inOutput, inConfig, new List<string>() { inStartFile}, inStartDir, inNrWarnings)
		{
		}


        /// <summary>
        /// Constructor used to check multiple source files.
        /// </summary>
		public IncludeChecker(IOutput inOutput, Config inConfig, List<string> inStartFiles, string inStartDir, int inNrWarnings)
		{
			mOutput = inOutput;
			mConfig = inConfig;
			mStartFiles = inStartFiles;
			mStartDir = inStartDir;
			mNrWarnings = inNrWarnings;
		}
		
		
        /// <summary>
        /// Perform the actual checking.
        /// </summary>
		public void CheckIncludes()
		{
			List<string> files = GetFiles();
			foreach (string filepath in files)
				CheckFile(filepath);	// Don't check return value, we'll process as much files as possible.

			WriteLine(mNrUnusedHeaders + " unused headers, " + mNrWarnings + " warnings");
		}
		
		
        /// <summary>
        /// Results of the check.
        /// </summary>
		public List<UnusedHeaderResult> Results
		{
			get
			{
				return mResults;
			}
		}
		
		
        /// <summary>
        /// Number of warnings.
        /// </summary>
		public int NrWarnings
		{
			get
			{
				return mNrWarnings;
			}
		}


        /// <summary>
        /// Get all name variations with the given prefixes and suffixes.
        /// </summary>
        /// <remarks>
        /// Only made public for testing.
        /// </remarks>
		public static List<string> GetNameVariations(string inName, List<string> inTypeAliasPrefixes, List<string> inTypeAliasSuffixes)
		{
			List<string> variations = new List<string>();

			variations.Add(inName);

			foreach (string prefix in inTypeAliasPrefixes)
				variations.Add(prefix + inName);

			foreach (string suffix in inTypeAliasSuffixes)
				variations.Add(inName + suffix);

			foreach (string prefix in inTypeAliasPrefixes)
			{
				foreach (string suffix in inTypeAliasSuffixes)
					variations.Add(prefix + inName + suffix);
			}

			return variations;
		}


		/// <summary>
		/// Check a single file for unused includes.
		/// </summary>
		/// <param name="inFilePath">Full path to the file to check.</param>
		/// <returns>Returns false if there was an error, true otherwise.</returns>
		private bool CheckFile(string inFilePath)
		{
			WriteLine("Processing file " + inFilePath);
			if (IsSourceFileIgnored(inFilePath))
			{
				Verbose("  ignored because of exclude path");
				return true;
			}
			
			if (IsInterfaceHeader(inFilePath))
			{
				Verbose("  ignored because of interface header");
				return true;
			}
			
			if (!System.IO.File.Exists(inFilePath))
			{
				WriteLine("Warning: file " + inFilePath + " doesn't exist!");
				++mNrWarnings;
				return true;
			}
			string contents = FileUtil.ReadTextFromFile(inFilePath);
			StripContents(ref contents);

			foreach (Include include in GetIncludesFromFile(inFilePath))
			{
				Verbose("  found include " + include.Path);
				// Don't check .inl files or .cpp files, we assume that those don't define any symbols, only use them.
				string lowered_path = include.Path.ToLower();
				if (!lowered_path.EndsWith(".inl") && !lowered_path.EndsWith(".cpp"))
				{
					if (File.Exists(include.Path))
					{
						if (!CheckIfHeaderIsUsed(inFilePath, contents, include))
							return false;
					}
                    else if (!IsHeaderFileIgnored(inFilePath, include.Path))
                    {
                        WriteLine(inFilePath + "(" + include.Line + "): Warning: header " + include.Path + " not found");
                        ++mNrWarnings;
                    }
                    else
                    {
                        Verbose("    file does not exist");
                    }
				}
				else
				{
					Verbose("    skipping (ignoring .inl and .cpp)");
				}
			}
			
			return true;
		}
		
		
		/// <summary>
		/// Get the list of includes from the file specified by inFilePath.
		/// </summary>
		/// <param name="inFilePath"></param>
		/// <returns></returns>
		private List<Include> GetIncludesFromFile(string inFilePath)
		{
			IncludeFinder include_finder = new IncludeFinder(mConfig.IncludePaths);
			return include_finder.GetIncludes(inFilePath);
		}


		/// <summary>
		/// Checks if inFileContents uses include file inInclude.
		/// </summary>
		/// <returns>Returns false if there was an error, true otherwise.</returns>
		private bool CheckIfHeaderIsUsed(string inFilePath, string inFileContents, Include inInclude)
		{
			if (IsHeaderFileIgnored(inFilePath, inInclude.Path))
			{
				Verbose("    ignored");
				return true;
			}

            string errors = "";
            List<CtagsParser.Tag> tags = GetTagsFromInclude(inInclude, ref errors);
			if (tags == null)
			{
				mOutput.WriteError(errors);
				return false;
			}
			foreach (CtagsParser.Tag tag in tags)
			{
				Verbose("    tag " + tag.GetTagType().ToString() + " " + tag.GetName());
				if (FileContentsContainTag(inFileContents, tag))
				{
					Verbose("    found tag " + tag.GetName() + " from header " + inInclude.Path);
					return true;
				}
			}
			
			// Extra heuristic: use the basename of the include. There are wrapper includes that include platform dependent
			// headers. We won't pick that up until we add those tags to this include.
			string header_basename = Path.GetFileNameWithoutExtension(inInclude.Path);
			if (FileContentsContainString(inFileContents, header_basename))
			{
				Verbose("    found string " + header_basename + " from header " + inInclude.Path);
				return true;
			}
			
			Verbose("    no tags found");
			WriteLine(inFilePath + "(" + inInclude.Line + "): nothing declared in " + inInclude.Path + " seems to be used.");
			mResults.Add(new UnusedHeaderResult(inFilePath, inInclude.Line, inInclude.Path));
			++mNrUnusedHeaders;
			return true;
		}
		
		
		/// <summary>
		/// Get the tags for inInclude.
        /// </summary>
        /// <remarks>
		/// Caches the results so each header needs only to be scanned once.
		/// For normal headers only the tags from the header will be parsed. For interface headers all tags from
		/// the complete include tree will be retrieved.
        /// </remarks>
		private List<CtagsParser.Tag> GetTagsFromInclude(Include inInclude, ref string outErrors)
		{
			List<CtagsParser.Tag> tags = GetTagsFromCache(inInclude);
			if (tags != null)
				return tags;
				
			if (IsInterfaceHeader(inInclude.Path))
				tags = GetTagsFromHeaderRecursively(inInclude, ref outErrors);
			else
				tags = GetTagsFromHeaderOnly(inInclude, ref outErrors);
			
			if (tags != null)
				AddTagsToCache(inInclude, tags);

			return tags;
		}
		
		
		/// <summary>
		/// Is inInclude an interface header?
		/// </summary>
		private bool IsInterfaceHeader(string inPath)
		{
			foreach (string interface_header in mConfig.InterfaceHeaders)
			{
				if (inPath.EndsWith(interface_header, StringComparison.InvariantCultureIgnoreCase))
					return true;
			}
			
			return false;
		}
		
		
		/// <summary>
		/// Get all tags from a header only.
		/// </summary>
		/// <returns>null on error, a list of tags otherwise</returns>
		private List<CtagsParser.Tag> GetTagsFromHeaderOnly(Include inInclude, ref string outErrors)
		{
			CtagsParser parser = new CtagsParser(mConfig.CtagsPath);
			return parser.GetTagsFromFile(inInclude.Path, ref outErrors);
		}
		
		
		/// <summary>
		/// Get tags from inInclude, but also from all files it includes.
		/// </summary>
		private List<CtagsParser.Tag> GetTagsFromHeaderRecursively(Include inInclude, ref string outErrors)
		{
			List<CtagsParser.Tag> tags = GetTagsFromHeaderOnly(inInclude, ref outErrors);
			if (tags == null)
				return tags;
				
			// Make sure recursion stops when we encounter this file again. The list is not complete yet, but it will be when
			// we finish the recursion.
			AddTagsToCache(inInclude, tags);

			var tag_set = new HashSet<CtagsParser.Tag>();
			foreach (CtagsParser.Tag tag in tags)
				tag_set.Add(tag);
				
			List<Include> includes = GetIncludesFromFile(inInclude.Path);
			foreach (Include include in includes)
			{
				tags = GetTagsFromInclude(include, ref outErrors);
				foreach (CtagsParser.Tag tag in tags)
					tag_set.Add(tag);
			}
			
			return new List<CtagsParser.Tag>(tag_set);
		}


		/// <summary>
		/// Remove includes, comments and strings from ioContents.
		/// </summary>
		/// <param name="ioContents"></param>
		private void StripContents(ref string ioContents)
		{
			CppParser parser = new CppParser();
			//TODO: do the removes in one pass
			parser.RemoveIncludes(ref ioContents);
			parser.RemoveComments(ref ioContents);
			parser.RemoveStrings(ref ioContents);
		}


        /// <summary>
        /// Do we have to ignore the source file inFilePath?
        /// </summary>
		private bool IsSourceFileIgnored(string inFilePath)
		{
			foreach (string filepath in mConfig.ExludePaths)
			{
				if (inFilePath.StartsWith(filepath, StringComparison.InvariantCultureIgnoreCase))
					return true;
			}
			return false;
		}
		
		
        /// <summary>
        /// Do we have to ignore the combination of inSourcePath and inHeaderPath?
        /// </summary>
		private bool IsHeaderFileIgnored(string inSourcePath, string inHeaderPath)
		{
			foreach (string filepath in mConfig.ExludePaths)
			{
                if (inSourcePath.StartsWith(filepath, StringComparison.InvariantCultureIgnoreCase))
					return true;
                if (inHeaderPath.StartsWith(filepath, StringComparison.InvariantCultureIgnoreCase))
					return true;
			}

			if (mConfig.IgnoreHeaderInfos != null)
			{
				foreach (IgnoreHeaderInfo ignore_header in mConfig.IgnoreHeaderInfos)
				{
					// The file is ignored if:
					// - source is not specified and the header paths match
					// - source paths match and header paths match
					if (inHeaderPath.EndsWith(ignore_header.Header, StringComparison.InvariantCultureIgnoreCase))
					{
						if (string.IsNullOrEmpty(ignore_header.Source) || inSourcePath.EndsWith(ignore_header.Source, StringComparison.InvariantCultureIgnoreCase))
							return true;
					}
				}
			}

            return false;
		}


        /// <summary>
        /// Check if inContents contain the given tag.
        /// </summary>
		private bool FileContentsContainTag(string inContents, CtagsParser.Tag inTag)
		{
			if (FileContentsContainString(inContents, inTag.GetName()))
				return true;
			if (inTag.GetTagType() == CtagsParser.Tag.EType.EStruct
					|| inTag.GetTagType() == CtagsParser.Tag.EType.EClass)
			{
				List<string> variations = GetNameVariations(inTag.GetName(), mConfig.TypeAliasPrefixes, mConfig.TypeAliasSuffixes);
				foreach (string name in variations)
				{
					if (FileContentsContainString(inContents, name))
						return true;
				}
			}

			return false;
		}


        /// <summary>
        /// Check if inContents contain the given string.
        /// </summary>
		private bool FileContentsContainString(string inContents, string inString)
		{
			string regex = @"\b" + inString + @"\b";
			Match m = Regex.Match(inContents, regex);
			return m.Success;
		}


        /// <summary>
        /// Check if inContents contain the given string, but search for it while ignoring case.
        /// </summary>
        private bool FileContentsContainStringCaseInsensitive(string inContents, string inString)
		{
			string regex = @"\b" + inString + @"\b";
			Match m = Regex.Match(inContents, regex, RegexOptions.IgnoreCase);
			return m.Success;
		}


		private List<string> GetFiles()
		{
			List<string> list;
			if (!string.IsNullOrEmpty(mStartDir))
				list = GetFilesFromDir(mStartDir);
			else
				list = mStartFiles;

			return list;
		}


		private List<string> GetFilesFromDir(string inDir)
		{
			string[] extensions = new string[] {
				".cpp",
				".cxx",
				".c",
				".cc",
				".h",
				".hpp",
				".inl"
			};

			if (mStartDir.Contains("*"))
			{
				int pos = mStartDir.IndexOf("*");
				string extension = mStartDir.Substring(pos + 1);
				extensions = new string[] { extension };

				mStartDir = mStartDir.Substring(0, pos);
			}

			List<string> all_files = FileUtil.GetFilesRecursively(mStartDir);
			List<string> filtered_files = new List<string>();
			foreach (string filepath in all_files)
			{
				bool has_matching_extension = false;
				foreach (string extension in extensions)
				{
					if (filepath.EndsWith(extension))
					{
						has_matching_extension = true;
						break;
					}
				}
				if (has_matching_extension)
					filtered_files.Add(filepath);
			}
			return filtered_files;
		}


		private void Verbose(string inOutput)
		{
			if (mConfig.Verbose)
				WriteLine(inOutput);
		}


		private void WriteLine(string inOutput)
		{
			mOutput.WriteLine(inOutput);
		}


		private void WriteError(string inError)
		{
			mOutput.WriteLine(inError);
		}
		
		
		/// <summary>
		/// Add tags for inInclude to the cache.
		/// </summary>
		private void AddTagsToCache(Include inInclude, List<CtagsParser.Tag> inTags)
		{
			mTagCache[inInclude.Path.ToLowerInvariant()] = inTags;
		}
		
		
		/// <summary>
		/// Get tags from cache.
		/// </summary>
		/// <returns>null if the cache has no entry for the specified include.</returns>
		private List<CtagsParser.Tag> GetTagsFromCache(Include inInclude)
		{
			List<CtagsParser.Tag> tags;
			if (mTagCache.TryGetValue(inInclude.Path.ToLowerInvariant(), out tags))
				return tags;
			else
				return null;
		}


        /// <summary>
        /// Version string.
        /// </summary>
		public static string Version
		{
			get
			{
				string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
#if BETA
				version += " Beta";
#endif
				return version;
			}
		}


        /// <summary>
        /// Assembly copyright string.
        /// </summary>
		private static string AssemblyCopyright
		{
			get
			{
				// Get all Copyright attributes on this assembly
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				// If there aren't any Copyright attributes, return an empty string
				if (attributes.Length == 0)
					return string.Empty;
				// If there is a Copyright attribute, return its value
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}



		private IOutput mOutput;

		private Config mConfig = null;
		private string mStartDir = string.Empty;
		private List<string> mStartFiles = new List<string>();
		private Dictionary<string, List<CtagsParser.Tag>> mTagCache = new Dictionary<string, List<CtagsParser.Tag>>();

		private int mNrUnusedHeaders = 0;
		private int mNrWarnings = 0;
		private List<UnusedHeaderResult> mResults = new List<UnusedHeaderResult>();
	}
}
