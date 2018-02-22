using System;
using System.Collections.Generic;
using System.IO;

namespace DevPal.CSLib
{
	/// <summary>
	/// FileUtil contains static utility function to work with files and directories.
	/// </summary>
	public static class FileUtil
	{
        /// <summary>
        /// Read all text from a file, return as single string
        /// </summary>
		public static string ReadTextFromFile(string inPath)
		{
			List<string> lines = new List<string>();
			StreamReader sr = new StreamReader(inPath);
			string contents = sr.ReadToEnd();
			sr.Close();
			return contents;
		}


        /// <summary>
        /// Read all text from a file, return list of lines
        /// </summary>
		public static List<string> ReadLinesFromFile(string inPath)
		{
			string contents = ReadTextFromFile(inPath);
			return LineUtil.GetLineList(contents);
		}


        /// <summary>
        /// Write text to a file
        /// </summary>
		public static void WriteTextToFile(string inPath, string inText)
		{
			StreamWriter sw = new StreamWriter(inPath);
			sw.Write(inText);
			sw.Close();
		}


        /// <summary>
        /// Write a list of lines as a single text to a file
        /// </summary>
        public static void WriteLinesToFile(string inPath, List<string> lines)
		{
			StreamWriter sw = new StreamWriter(inPath);
			foreach (string line in lines)
			{
				sw.WriteLine(line);
			}
			sw.Close();
		}


        /// <summary>
		/// Retrieves all files residing in a directory tree.
		/// </summary>
		public static List<string> GetFilesRecursively(string inPath)
		{
			List<string> files = new List<string>();
			if (Directory.Exists(inPath)) 
			{
				DirectoryInfo di = new DirectoryInfo(inPath);
				AddFilesRecursively(di, ref files);
			}
			return files;
		}


        /// <summary>
        /// Recursive helper of GetFilesRecursively
        /// </summary>
        private static void AddFilesRecursively(DirectoryInfo inDirInfo, ref List<string> ioFiles)
		{
			FileSystemInfo[] files = inDirInfo.GetFiles();
			foreach (FileSystemInfo file in files)
			{
				ioFiles.Add(file.FullName);
			}

			FileSystemInfo[] dirs = inDirInfo.GetDirectories();
			foreach (DirectoryInfo dir in dirs)
			{
				AddFilesRecursively(dir, ref ioFiles);
			}
		}


        /// <summary>
		/// Removes a directory recursively. Does nothing if the directory doesn't exist.
		/// </summary>
		public static void DeleteDirectory(string inPath)
		{
			try
			{
				Directory.Delete(inPath, true);
			}
			catch (DirectoryNotFoundException)
			{
				// ignore
			}
		}

		
        /// <summary>
		/// Deletes a list of files.
		/// </summary>
		/// <param name="outErrors">Empty when no errors occurred, error description when errors did occur.</param>
		/// <returns>true when all files were successfully deleted.
		/// If a file doesn't exist, no error is given for that file.
		/// </returns>
		public static bool DeleteFiles(List<string> inFileNames, ref string outErrors)
		{
			bool ret_val = true;
			foreach (string filename in inFileNames)
			{
				try
				{
					File.Delete(filename);
				}
				catch (Exception e)
				{
					outErrors += "Could not delete file " + filename + ": " + e.Message + "\r\n";
					ret_val = false;
				}
			}

			return ret_val;
		}
	}
}
