using System;
using System.Diagnostics;
using System.ComponentModel;
using DevPal.CSLib;
using System.Collections.Generic;

namespace DevPal.IncludeChecker
{
    /// <summary>
    /// Parses the output of ctags.
    /// </summary>
	public class CtagsParser
	{
        /// <summary>
        /// Represents a tag as found by ctags.
        /// </summary>
		public class Tag
		{
            /// <summary>
            /// Type of the tag.
            /// </summary>
			public enum EType {
				EUnknown,
				EClass,
				EEnum,
				EEnumerator,
				EMacro,
				EMember,
				ENamespace,
				ETypedef,
				EStruct,
				EFunction,
				EPrototype,
				EExternVar,
				EVar
			};


            /// <summary>
            /// Constructor.
            /// </summary>
            public Tag(EType inType, string inName)
			{
				mType = inType;
				mName = inName;
			}


            /// <summary>
            /// Equality check.
            /// </summary>
			public override bool Equals(object obj)
			{
				if (obj == null || !(obj is Tag))
					return false;
				Tag rhs = (Tag)obj;
				return (rhs.mName == mName) && (rhs.mType == mType);
			}


            /// <summary>
            /// Get hash code.
            /// </summary>
			public override int GetHashCode()
			{
				return mName.GetHashCode() * 37 + mType.GetHashCode();
			}


            /// <summary>
            /// Get type of the tag.
            /// </summary>
			public EType GetTagType()
			{
				return mType;
			}


            /// <summary>
            /// Get name of the tag.
            /// </summary>
			public string GetName()
			{
				return mName;
			}


            private EType mType = EType.EUnknown;
            private string mName = "";
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public CtagsParser(string inCtagsPath)
		{
			mCtagsPath = inCtagsPath;
		}


        /// <summary>
        /// Get the tag information from one ctags output line.
        /// </summary>
		public Tag GetTag(string inCtagsLine)
		{
			string[] parts = inCtagsLine.Split(' ');
			string ctags_type = "";
			for (int i = 1; i < parts.Length; ++i)
			{
				if (parts[i].Length > 0) 
				{
					ctags_type = parts[i];
					break;
				}
			}
			return new Tag(GetTagTypeFromString(ctags_type), parts[0]);
		}


        /// <summary>
        /// Parse all tags from the ctags output.
        /// </summary>
		public List<Tag> ParseCtagsOutput(string inCtagsOutput)
		{
			List<Tag> tags = new List<Tag>();
			List<string> lines = LineUtil.GetLineList(inCtagsOutput);
			foreach (string line in lines)
			{
				Tag tag = GetTag(line);
				if (tag.GetTagType() != Tag.EType.EUnknown)
					tags.Add(tag);
			}
			return tags;
		}


		/// <summary>
		/// Runs ctags on inFilePath and returns a list with tags that were found.
		/// </summary>
		/// <param name="outError">Error string when the tags could not be generated.</param>
		/// <returns>A list containing all tags found, null if there was an error.</returns>
		public List<Tag> GetTagsFromFile(string inFilePath, ref string outError)
		{
			Process process = new Process();
			try
			{
				process.StartInfo.FileName = mCtagsPath;
				// Use --c++kinds=+px to also get function prototypes and external variable declarations.
				// Use -x to send simplified output to stdout.
				process.StartInfo.Arguments = @"--c++-kinds=+tpx -x -I __out_bcount_full_opt+ --language-force=c++ """ + inFilePath + @"""";
				//TODO: pass -I arguments to this function.
				process.StartInfo.Verb = "";
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.UseShellExecute = false;
				process.Start();
				string output = process.StandardOutput.ReadToEnd();
				string errors = process.StandardError.ReadToEnd();
				process.WaitForExit();
				if (process.ExitCode == 0)
				{
					return ParseCtagsOutput(output);
				}
			}
			catch (Win32Exception e)
			{
				outError = "Error running ctags: " + e.Message;
				return null;
			}

			return null;
		}

		//////////////////////////// private helpers //////////////////////////

        /// <summary>
        /// Parse a tag type from a ctags type string.
        /// </summary>
		private Tag.EType GetTagTypeFromString(string inCtagsType)
		{
			switch (inCtagsType)
			{
				case "class":		return Tag.EType.EClass;
				case "typedef":		return Tag.EType.ETypedef;
				case "struct":		return Tag.EType.EStruct;
				case "macro":		return Tag.EType.EMacro;
				case "enum":		return Tag.EType.EEnum;
				case "enumerator":	return Tag.EType.EEnumerator;
				case "function":	return Tag.EType.EFunction;
				case "member":		return Tag.EType.EMember;
				case "namespace":	return Tag.EType.ENamespace;
				case "prototype":	return Tag.EType.EPrototype;
				case "externvar":	return Tag.EType.EExternVar;
				case "variable":	return Tag.EType.EVar;
				default:			return Tag.EType.EUnknown;
			}
		}

        private string mCtagsPath = "";
    }
}
