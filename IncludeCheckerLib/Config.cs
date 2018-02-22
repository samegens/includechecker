using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using DevPal.CSLib;

namespace DevPal.IncludeChecker
{
    /// <summary>
    /// IncludeChecker configuration.
    /// </summary>
	public class Config
	{
		/// <summary>
		/// Load the configuration from a file.
		/// </summary>
		public bool LoadFromFile(string inFilePath, ref string outError)
		{
			if (!File.Exists(inFilePath))
			{
				outError = "Error: file " + inFilePath + " doesn't exist.";
				return false;
			}

			string config_contents = FileUtil.ReadTextFromFile(inFilePath);
			string base_path = Path.GetDirectoryName(inFilePath);
			if (!base_path.EndsWith(Path.DirectorySeparatorChar))
				base_path += Path.DirectorySeparatorChar;
			return Parse(config_contents, base_path, ref outError);
		}
		
		
        /// <summary>
        /// Parse the configuration from a string.
        /// </summary>
		public bool Parse(string inConfigString, string inBasePath, ref string outError)
		{
			mIncludePaths = new List<string>();
			mIgnorePaths = new List<string>();
			mTypeAliasPrefixes = new List<string>();
			mIgnoreHeaderInfos = new List<IncludeChecker.IgnoreHeaderInfo>();
			mVerbose = false;
			
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(inConfigString);
				XPathNavigator nav = doc.CreateNavigator();

				XPathNodeIterator iter = nav.Select("/devpal/includechecker/settings/ctags_path");
				foreach (XPathNavigator current in iter)
					mCtagsPath = current.Value;

				iter = nav.Select("/devpal/includechecker/settings/include_path");
				AddPathsFromIteratorToList(iter, mIncludePaths, inBasePath);

				iter = nav.Select("/devpal/includechecker/settings/interface_header");
				AddPathsFromIteratorToList(iter, mInterfaceHeaders, inBasePath);

				iter = nav.Select("/devpal/includechecker/settings/exclude_path");
				AddPathsFromIteratorToList(iter, mIgnorePaths, inBasePath);

				iter = nav.Select("/devpal/includechecker/settings/type_alias_prefix");
				foreach (XPathNavigator current in iter)
					mTypeAliasPrefixes.Add(current.Value);

				iter = nav.Select("/devpal/includechecker/settings/type_alias_suffix");
				foreach (XPathNavigator current in iter)
					mTypeAliasSuffixes.Add(current.Value);

				iter = nav.Select("/devpal/includechecker/settings/verbose");
				mVerbose = (iter.Count > 0);
				
				iter = nav.Select("/devpal/includechecker/settings/ignore_header");
				foreach (XPathNavigator current in iter)
				{
					XPathNodeIterator source = current.SelectChildren("source", "");
					string source_string = null;
					if (source.MoveNext())
						source_string = source.Current.Value;
					
					XPathNodeIterator header = current.SelectChildren("header", "");
					if (!header.MoveNext())
					{
						outError = "Missing <header> in ignore_header: " + current.InnerXml;
						return false;
					}
					string header_string = header.Current.Value;
					
					IncludeChecker.IgnoreHeaderInfo info = new IncludeChecker.IgnoreHeaderInfo(source_string, header_string);
					mIgnoreHeaderInfos.Add(info);
				}

				return true;
			}
			catch (XmlException e)
			{
				outError = e.ToString();
				return false;
			}
		}


		public void AddPathsFromIteratorToList(XPathNodeIterator inNodeIterator, List<string> ioList, string inBasePath)
		{
			foreach (XPathNavigator current in inNodeIterator)
			{
				string path = current.Value;
				if (!Path.IsPathRooted(path))
					path = inBasePath + path;
				ioList.Add(path);
			}
		}
		
		
        /// <summary>
        /// Add a type alias prefix. (for example "rc" for const &)
        /// </summary>
		public void AddTypeAliasPrefix(string inTypeAliasPrefix)
		{
			mTypeAliasPrefixes.Add(inTypeAliasPrefix);
		}


        /// <summary>
        /// Add a type alias suffix. (for example "Ref" for refcounted wrapper declaration)
        /// </summary>
        /// <param name="inTypeAliasSuffix"></param>
		public void AddTypeAliasSuffix(string inTypeAliasSuffix)
		{
			mTypeAliasSuffixes.Add(inTypeAliasSuffix);
		}


        /// <summary>
        /// Add a header to be ignored.
        /// </summary>
		public void AddIgnoreHeaderInfo(IncludeChecker.IgnoreHeaderInfo inIgnoreHeaderInfo)
		{
			mIgnoreHeaderInfos.Add(inIgnoreHeaderInfo);
		}


        /// <summary>
        /// Add an include directory.
        /// </summary>
		public void AddIncludePath(string inIncludePath)
		{
			mIncludePaths.Add(inIncludePath);
		}


        /// <summary>
        /// Add a path to ignore.
        /// </summary>
        /// <param name="inIgnorePath"></param>
		public void AddIgnorePath(string inIgnorePath)
		{
			mIgnorePaths.Add(inIgnorePath);
		}


        /// <summary>
        /// List of include directories.
        /// </summary>
		public List<string> IncludePaths
		{
			get { return mIncludePaths; }
			set { mIncludePaths = value; }
		}
		
		
        /// <summary>
        /// List of paths to ignore
        /// </summary>
		public List<string> ExludePaths
		{
			get { return mIgnorePaths; }
			set { mIgnorePaths = value; }
		}
		
		
        /// <summary>
        /// List fo type alias prefixes.
        /// </summary>
		public List<string> TypeAliasPrefixes
		{
			get { return mTypeAliasPrefixes; }
			set { mTypeAliasPrefixes = value; }
		}


        /// <summary>
        /// List of type alias suffixes.
        /// </summary>
		public List<string> TypeAliasSuffixes
		{
			get { return mTypeAliasSuffixes; }
			set { mTypeAliasSuffixes = value; }
		}


        /// <summary>
        /// Verbose mode, if on IncludeChecker will output extra information.
        /// </summary>
		public bool Verbose
		{
			get { return mVerbose; }
			set { mVerbose = value; }
		}
		
		
        /// <summary>
        /// Path to the ctags executable.
        /// </summary>
		public string CtagsPath
		{
			get { return mCtagsPath; }
			set { mCtagsPath = value; }
		}
		
		
        /// <summary>
        /// Headers to ignore.
        /// </summary>
		public List<IncludeChecker.IgnoreHeaderInfo> IgnoreHeaderInfos
		{
			get { return mIgnoreHeaderInfos; }
			set { mIgnoreHeaderInfos = value; }
		}
		
		
        /// <summary>
        /// Headers that are to be processed as interface headers (which means that they don't actually declare anything, but include other headers that do).
        /// </summary>
		public List<string> InterfaceHeaders
		{
			get { return mInterfaceHeaders; }
		}


		private bool mVerbose;
		private string mCtagsPath = "";
		private List<string> mIncludePaths = new List<string>();
		private List<string> mIgnorePaths = new List<string>();
		private List<string> mInterfaceHeaders = new List<string>();
		private List<string> mTypeAliasPrefixes = new List<string>();
		private List<string> mTypeAliasSuffixes = new List<string>();
		private List<IncludeChecker.IgnoreHeaderInfo> mIgnoreHeaderInfos = new List<IncludeChecker.IgnoreHeaderInfo>();
	}
}
