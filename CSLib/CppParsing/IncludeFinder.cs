using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace DevPal.CSLib.CppParsing
{
    /// <summary>
    /// Represents a single #include statement.
    /// </summary>
	public class Include
	{
        /// <summary>
        /// Constructor.
        /// </summary>
		public Include(string inPath, int inLine, bool inIsSystemInclude)
		{
			mPath = inPath;
			mLine = inLine;
			mIsSystemInclude = inIsSystemInclude;
		}


        /// <summary>
        /// Path of the include, can be a full path (when a file was found), or the path as specified in the #include statement.
        /// </summary>
		public string Path
		{
			get { return mPath; }
			set { mPath = value; }
		}


        /// <summary>
        /// Line number of the #include statement.
        /// </summary>
		public int Line
		{
			get { return mLine; }
		}


        /// <summary>
        /// Is the #include a system include? (<...> instead of "...")
        /// </summary>
		public bool IsSystemInclude
		{
			get { return mIsSystemInclude; }
		}
		

		private string mPath;
		private int mLine;
		private bool mIsSystemInclude;
	}


    /// <summary>
    /// IncludeFinder tries to resolve includes to an absolute path with a list of include directories.
    /// </summary>
	public class IncludeFinder
	{
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inIncludePaths">Full directory paths to use when resolving include paths.</param>
        public IncludeFinder(List<string> inIncludePaths)
        {
            mIncludePaths = inIncludePaths;
        }


        /// <summary>
        /// Returns the paths where a #include might possibly point to.
        /// </summary>
        /// <remarks>Pre: inIncludeLine is a valid include line, ie IsInclude is true.</remarks>
        public static List<string> GetPossibleHeaderFiles(List<string> inIncludePaths, string inHeaderPath, Include inInclude)
        {
            List<string> paths = new List<string>();
            string relative_path = GetNativePath(inInclude.Path);
            if (!inInclude.IsSystemInclude)
            {
                // Add path relative to header file.
                string path;
                int last_slash_pos = inHeaderPath.LastIndexOf("\\");
                if (last_slash_pos >= 0)
                {
                    path = inHeaderPath.Substring(0, last_slash_pos + 1);
                    path += relative_path;
                    paths.Add(path);
                }
            }

            foreach (string include_path in inIncludePaths)
            {
                string path = include_path;
                if (!path.EndsWith("\\"))
                {
                    path += "\\";
                }
                path += relative_path;
                paths.Add(path);
            }

            return paths;
        }


        /// <summary>
        /// Is inLine an #include statement?
        /// </summary>
        public static bool IsInclude(string inLine)
		{
			string stripped = inLine.Trim();
			return stripped.StartsWith("#include");
		}

        
        /// <summary>
        /// Get the path of an #include statement.
        /// </summary>
        /// <remarks>Pre: inLine is a valid include line, ie IsInclude is true.</remarks>
		public static string GetIncludePath(string inLine)
		{
			int first_pos = inLine.IndexOf("\"");
			int last_pos = -1;
			if (first_pos > 0) 
			{
				last_pos = inLine.IndexOf("\"", first_pos + 1);
			}
			else 
			{
				first_pos = inLine.IndexOf("<");
				if (first_pos > 0) 
				{
					last_pos = inLine.IndexOf(">", first_pos + 1);
				}
			}

			if (first_pos > 0 && last_pos > first_pos) 
			{
				return inLine.Substring(first_pos + 1, last_pos - first_pos - 1);
			}
			else 
			{
				return "";
			}
		}


        /// <summary>
        /// Return the native variant of the include path.
        /// </summary>
		public static string GetNativePath(string inIncludePath)
		{
			return inIncludePath.Replace('/', Path.DirectorySeparatorChar);
		}


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Pre: inIncludeLine is a valid include line, ie IsInclude is true.</remarks>
		public static bool IsSystemInclude(string inIncludeLine)
		{
			return (inIncludeLine.IndexOf("<") > 0 && inIncludeLine.IndexOf(">") > 0);
		}


        /// <summary>
        /// Returns a list of includes as they appear in the source.
        /// </summary>
        /// <remarks>Include path is not translated.</remarks>
		public List<Include> GetRawIncludes(string inSource)
		{
			List<Include> includes = new List<Include>();
			StringReader reader = new StringReader(inSource);
			string line = null;
			int line_number = 1;
			while (true)
			{
				line = reader.ReadLine();
				if (line == null)
					break;

				if (IsInclude(line))
				{
					bool is_system_include = IsSystemInclude(line);
					Include include = new Include(GetIncludePath(line), line_number, is_system_include);
					includes.Add(include);
				}
				++line_number;
			}
			
			return includes;
		}


        /// <summary>
        /// Returns the list of include files inFilePath includes.
        /// </summary>
        /// <remarks>The full path is returned if possible, else the path of the include is
        /// returned as is. Does NOT retrieve the includes recursively.
        /// </remarks>
		public virtual List<Include> GetIncludes(string inFilePath)
		{
			// If we couldn't map the header file to a full path, we just return
			// an empty list.
			if (!System.IO.File.Exists(inFilePath)) 
				return new List<Include>();

			string file_contents = FileUtil.ReadTextFromFile(inFilePath);
			List<Include> includes = GetRawIncludes(file_contents);
			foreach (Include include in includes)
			{
				List<string> possible_header_files = GetPossibleHeaderFiles(mIncludePaths, inFilePath, include);
				foreach (string header_file in possible_header_files)
				{
					if (System.IO.File.Exists(header_file))
					{
						include.Path = header_file;
						break;
					}
				}
			}
			return includes;
		}


        /// <summary>
        /// Get all includes in the include tree of inFilePath
        /// </summary>
		public List<string> GetIncludesRecursively(string inFilePath)
		{
			List<string> visited_header_files = new List<string>();
			return GetIncludesRecursively(inFilePath, visited_header_files);
		}


        /// <summary>
        /// Recursive implementation helper of GetIncludesRecursively
        /// </summary>
		private List<string> GetIncludesRecursively(string inFilePath, List<string> ioVisitedHeaderFiles)
		{
			ioVisitedHeaderFiles.Add(inFilePath);
			List<Include> local_includes = GetIncludes(inFilePath);
			HashSet<string> recursive_includes = new HashSet<string>();
			foreach (Include include in local_includes) 
			{
				if (!ioVisitedHeaderFiles.Contains(include.Path)) 
				{
					recursive_includes.Add(include.Path);
					HashSet<string> more_includes = null;
					if (mHeaderFileToIncludeListMap.ContainsKey(include.Path)) 
					{
						more_includes = mHeaderFileToIncludeListMap[include.Path];
					}
					else 
					{
						more_includes = new HashSet<string>(GetIncludesRecursively(include.Path, ioVisitedHeaderFiles));
						mHeaderFileToIncludeListMap.Add(include.Path, more_includes);
					}
					recursive_includes.UnionWith(more_includes);
				}
			}
			
			// Transform set into list.
			List<string> include_list = new List<string>();
			foreach (string include_path in recursive_includes)
				include_list.Add(include_path);
			
			return include_list;
		}


        private List<string> mIncludePaths;
        private Dictionary<string, HashSet<string>> mHeaderFileToIncludeListMap = new Dictionary<string, HashSet<string>>();	// Contains per header file a set of header files recursively included by that header file.
	}
}
