using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevPal.IncludeChecker
{
    static class TestUtils
    {
        /// <summary>
        /// Get the full path given a relative path to a test asset. 
        /// </summary>
        /// <param name="inRelativePath">Path relative to the IncludeChecker root directory.</param>
        public static string sGetFullPath(string inRelativePath)
        {
            if (System.IO.File.Exists(inRelativePath))
            {
                return inRelativePath;
            }

            if (string.IsNullOrEmpty(sTestRootPath))
            {
                // Assume that the executing assembly is located at IncludeCheckerLib\bin\Release\IncludeCheckerLib.dll
                string assembly_directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().FullName);
                sTestRootPath = System.IO.Path.GetFullPath(assembly_directory + @"..\..\..") + @"\";
            }

            string full_path = sTestRootPath + inRelativePath;
            if (System.IO.File.Exists(full_path))
            {
                return full_path;
            }

            throw new Exception(@"Could not find test file " + inRelativePath + ", test root is " + sTestRootPath + @". Is the assembly being tested run from IncludeCheckerLib\bin\Release?");
        }


        private static string sTestRootPath = null;
    }
}
