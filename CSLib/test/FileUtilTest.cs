using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;


namespace DevPal.CSLib.Tests
{
	[TestFixture]
	public class FileUtilTest
	{
		[Test]
		public void TestGetFilesRecursively()
		{
            string tempDir = CreateTempDirectory();

			Directory.CreateDirectory(Path.Combine(tempDir, "testdir"));
			Directory.CreateDirectory(Path.Combine(tempDir, "testdir\\subdir"));
			Directory.CreateDirectory(Path.Combine(tempDir, "testdir\\subdir2"));
			Assert.IsTrue(Directory.Exists(Path.Combine(tempDir, "testdir\\subdir2")));

			// test directory tree without files
			List<string> files = FileUtil.GetFilesRecursively(Path.Combine(tempDir, ".\\testdir"));
			Assert.AreEqual(0, files.Count);

			// now add files
			CreateFile(Path.Combine(tempDir, "testdir\\file1.txt"));
			CreateFile(Path.Combine(tempDir, "testdir\\subdir\\file2.txt"));
			CreateFile(Path.Combine(tempDir, "testdir\\subdir\\file3.txt"));
			CreateFile(Path.Combine(tempDir, "testdir\\subdir2\\file4.txt"));
			CreateFile(Path.Combine(tempDir, "testdir\\subdir2\\file5.txt"));
			CreateFile(Path.Combine(tempDir, "testdir\\subdir2\\file6.txt"));

			files = FileUtil.GetFilesRecursively(Path.Combine(tempDir, "testdir"));
			Assert.AreEqual(6, files.Count);

			Directory.Delete(tempDir, true);
		}

		[Test]
		public void TestReadLinesFromEmptyFile()
		{
			string path = ".\\testfile.txt";
			try
			{
				File.Delete(path);
			}
			catch (FileNotFoundException)
			{
				// just ignore
			}

			StreamWriter sw = new StreamWriter(path);
			sw.Close();
			sw = null;

			Assert.IsTrue(File.Exists(path));

			List<string> lines = FileUtil.ReadLinesFromFile(path);
			Assert.IsNotNull(lines);
			Assert.AreEqual(0, lines.Count);

			File.Delete(".\\testfile.txt");
		}

		[Test]
		public void TestReadLinesFromNonEmptyFile()
		{
			string path = ".\\testfile.txt";
			try
			{
				File.Delete(path);
			}
			catch (FileNotFoundException)
			{
				// just ignore
			}

			StreamWriter sw = new StreamWriter(path);
			sw.WriteLine("hello");
			sw.WriteLine("world");
			sw.Close();

			Assert.IsTrue(File.Exists(path));

			List<string> lines = FileUtil.ReadLinesFromFile(path);
			Assert.IsNotNull(lines);
			Assert.AreEqual(3, lines.Count);
			IEnumerator<string> iter = lines.GetEnumerator();
			iter.MoveNext();
			Assert.AreEqual("hello", (string) iter.Current);
			iter.MoveNext();
			Assert.AreEqual("world", (string) iter.Current);
			iter.MoveNext();
			Assert.AreEqual("", (string) iter.Current);

			File.Delete(path);
		}

		[Test]
		public void TestWriteLinesToFile()
		{
			string path = ".\\testfile.txt";
			try
			{
				File.Delete(path);
			}
			catch (FileNotFoundException)
			{
				// just ignore
			}

			List<string> lines = new List<string>();
			lines.Add("hello");
			lines.Add("world");

			FileUtil.WriteLinesToFile(path, lines);

			StreamReader sr = new StreamReader(path);
			string contents = sr.ReadToEnd();
			sr.Close();
			Assert.AreEqual("hello\r\nworld\r\n", contents);

			File.Delete(path);
		}

		[Test]
		public void TestReadAndWriteText()
		{
			string test_string = "hello\r\nworld\r\nHow are you?\r\n";
			const string path = ".\\test.txt";
			FileUtil.WriteTextToFile(path, test_string);
			string read_string = FileUtil.ReadTextFromFile(path);
			Assert.AreEqual(test_string, read_string);

			File.Delete(path);
		}

		[Test]
		public void TestDeleteDirectory()
		{
			Directory.CreateDirectory(".\\testdir");
			Directory.CreateDirectory(".\\testdir\\subdir");
			FileUtil.DeleteDirectory(".\\testdir");
			Assert.IsFalse(Directory.Exists(".\\testdir"));
			FileUtil.DeleteDirectory(".\\testdir");	// no exception
		}

		[Test]
		public void TestDeleteFilesEmptyList()
		{
			List<string> deleteFiles = new List<string>();
			string errors = "";
			Assert.IsTrue(FileUtil.DeleteFiles(deleteFiles, ref errors));
		}

		[Test]
		public void TestDeleteFiles1File()
		{
			Directory.CreateDirectory(".\\testdir");
			CreateFile(".\\testdir\\testfile");
			List<string> deleteFiles = new List<string>();
			string errors = "";
			deleteFiles.Add(".\\testdir\\testfile");
			Assert.IsTrue(FileUtil.DeleteFiles(deleteFiles, ref errors));
			FileUtil.DeleteDirectory(".\\testdir");
		}

		[Test]
		public void TestDeleteFiles2Files()
		{
			Directory.CreateDirectory(".\\testdir");
			CreateFile(".\\testdir\\testfile");
			CreateFile(".\\testdir\\testfile2");
			List<string> deleteFiles = new List<string>();
			string errors = "";
			deleteFiles.Add(".\\testdir\\testfile");
			deleteFiles.Add(".\\testdir\\testfile2");
			Assert.IsTrue(FileUtil.DeleteFiles(deleteFiles, ref errors));
			FileUtil.DeleteDirectory(".\\testdir");
		}

		[Test]
		public void TestDeleteFilesNonExistingFiles()
		{
			Directory.CreateDirectory(".\\testdir");
			List<string> deleteFiles = new List<string>();
			deleteFiles.Add(".\\testdir\\testfile");
			deleteFiles.Add(".\\testdir\\testfile2");
			string errors = "";
			Assert.IsTrue(FileUtil.DeleteFiles(deleteFiles, ref errors));
			FileUtil.DeleteDirectory(".\\testdir");
		}

		//TODO: file that's open for write
		//TODO: file that is a directory

		//////////////////////// private helpers //////////////////////////////////////

        private string CreateTempDirectory()
        {
            var tempDir = "\\temp\\";
            string randomChars = "abcdefghijklmnopqrstuvwxyz0123456789";
            const int nrCharsToGenerate = 4;
            var rnd = new Random();
            for (int i = 0; i < nrCharsToGenerate; i++)
            {
                tempDir = tempDir + randomChars[rnd.Next(0, randomChars.Length)];
            }
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            Directory.CreateDirectory(tempDir);
            return tempDir;
        }


        private void CreateFile(string path)
		{
			StreamWriter sw = new StreamWriter(path);
            sw.Write("generated test file, you can safely delete me");
			sw.Close();
		}
	}
}
