using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace DevPal.IncludeChecker
{
	[TestFixture]
	public class CtagsParserTest
	{
        [SetUp]
        public void SetUp()
        {
            mCtagsPath = TestUtils.sGetFullPath(@"IncludeChecker\ctags\ctags.exe");
        }


		[Test]
		public void TestLines()
		{
			Assert.IsTrue(CheckTag("RTTIReader       class        25 RTTIReader.h     class RTTIReader", CtagsParser.Tag.EType.EClass, "RTTIReader"));
			Assert.IsTrue(CheckTag("RTTIToTypeInfo   typedef      88 RTTIReader.h     typedef HashMap<HashedPtr<cRTTI>, pReaderTypeInfo> RTTIToTypeInfo;", CtagsParser.Tag.EType.ETypedef, "RTTIToTypeInfo"));
			Assert.IsTrue(CheckTag("ReaderTypeInfo   struct       77 RTTIReader.h     struct ReaderTypeInfo", CtagsParser.Tag.EType.EStruct, "ReaderTypeInfo"));
			Assert.IsTrue(CheckTag("__PIGS_RTTI_READER_H__ macro         3 RTTIReader.h     #define __PIGS_RTTI_READER_H__", CtagsParser.Tag.EType.EMacro, "__PIGS_RTTI_READER_H__"));
			Assert.IsTrue(CheckTag("RESULT_UNEXPECTED_END enumerator   48 RTTIReader.h     RESULT_UNEXPECTED_END", CtagsParser.Tag.EType.EEnumerator, "RESULT_UNEXPECTED_END"));
			Assert.IsTrue(CheckTag("EResult          enum         29 RTTIReader.h     enum EResult", CtagsParser.Tag.EType.EEnum, "EResult"));
			Assert.IsTrue(CheckTag("gSort            function     23 Sort.h           void gSort(taTYPE* inBegin, taTYPE* inEnd)", CtagsParser.Tag.EType.EFunction, "gSort"));
		}


		[Test]
		public void TestUnknownType()
		{
			CtagsParser parser = new CtagsParser("");
			CtagsParser.Tag tag = parser.GetTag("mLastError       justmadethisup       65 RTTIReader.h     EResult mLastError; ///< Last reported error");
			Assert.AreEqual(CtagsParser.Tag.EType.EUnknown, tag.GetTagType());
		}


		[Test]
		public void TestGetTags()
		{
			string ctags_output = @"EResult          enum         29 RTTIReader.h     enum EResult
GetLastError     function     59 RTTIReader.h     inline EResult GetLastError() { return mLastError; } ///< Get last result code
RESULT_VERSION_MISMATCH enumerator   45 RTTIReader.h     RESULT_VERSION_MISMATCH,
RTTIReader       class        25 RTTIReader.h     class RTTIReader
RTTIToTypeInfo   typedef      88 RTTIReader.h     typedef HashMap<HashedPtr<cRTTI>, pReaderTypeInfo> RTTIToTypeInfo;
ReaderTypeInfo   struct       77 RTTIReader.h     struct ReaderTypeInfo
StringToTypeInfo typedef      87 RTTIReader.h     typedef HashMap<string, pReaderTypeInfo> StringToTypeInfo;
__PIGS_RTTI_READER_H__ macro         3 RTTIReader.h     #define __PIGS_RTTI_READER_H__
mAttrs           member       81 RTTIReader.h     aRTTIAttrInfo mAttrs; ///< Attributes of type
mConstructor     member       83 RTTIReader.h     RTTIConstructorFunc mConstructor; ///< Constructor function
mDestructor      member       84 RTTIReader.h     RTTIDestructorFunc mDestructor; ///< Destructor function
mFactory         member       70 RTTIReader.h     pcRTTIFactory mFactory; ///< Factory we're using
mIndex           member       79 RTTIReader.h     int mIndex; ///< Index into mAllTypesArray
mLastError       member       65 RTTIReader.h     EResult mLastError; ///< Last reported error
mObjects         member       68 RTTIReader.h     pTypedPtrArray mObjects; ///< Object list to be saved
mReadMsgHandler  member       85 RTTIReader.h     RTTIMessageHandlerInfo mReadMsgHandler; ///< Handler for <mReadMsgType> message
mReadMsgType     member       71 RTTIReader.h     rcRTTI mReadMsgType; ///< Type of message to handle custom read functionality
mSize            member       82 RTTIReader.h     int mSize; ///< Size of atom
mStream          member       69 RTTIReader.h     pStream mStream; ///< Stream we're writing to
mType            member       80 RTTIReader.h     pcRTTI mType; ///< Type
mTypeInfoArray   member       91 RTTIReader.h     apReaderTypeInfo mTypeInfoArray; ///< Information about all types in file (in array form, for indexing)
mTypeInfoByName  member       89 RTTIReader.h     StringToTypeInfo mTypeInfoByName; ///< Information about all types in file
mTypeInfoByRTTI  member       90 RTTIReader.h     RTTIToTypeInfo mTypeInfoByRTTI; ///< Information about all types in file
gToString        prototype   150 StringTools.h    bool gToString(rcInt64 inData, rString outString);
gRTTIBinaryVersion externvar    11 RTTIVersion.h    extern pcTChar gRTTIBinaryVersion;
";
			CtagsParser parser = new CtagsParser("");
            List<CtagsParser.Tag> tags = parser.ParseCtagsOutput(ctags_output);
			Assert.AreEqual(25, tags.Count);
			Assert.IsTrue(tags.Contains(new CtagsParser.Tag(CtagsParser.Tag.EType.EFunction, "GetLastError")));
			Assert.IsTrue(tags.Contains(new CtagsParser.Tag(CtagsParser.Tag.EType.EEnum, "EResult")));
			Assert.IsTrue(tags.Contains(new CtagsParser.Tag(CtagsParser.Tag.EType.EEnumerator, "RESULT_VERSION_MISMATCH")));
			Assert.IsTrue(tags.Contains(new CtagsParser.Tag(CtagsParser.Tag.EType.EClass, "RTTIReader")));
			Assert.IsTrue(tags.Contains(new CtagsParser.Tag(CtagsParser.Tag.EType.ETypedef, "RTTIToTypeInfo")));
			Assert.IsTrue(tags.Contains(new CtagsParser.Tag(CtagsParser.Tag.EType.EStruct, "ReaderTypeInfo")));
			Assert.IsTrue(tags.Contains(new CtagsParser.Tag(CtagsParser.Tag.EType.ETypedef, "StringToTypeInfo")));
			Assert.IsTrue(tags.Contains(new CtagsParser.Tag(CtagsParser.Tag.EType.EMacro, "__PIGS_RTTI_READER_H__")));
			Assert.IsTrue(tags.Contains(new CtagsParser.Tag(CtagsParser.Tag.EType.EPrototype, "gToString")));
			Assert.IsTrue(tags.Contains(new CtagsParser.Tag(CtagsParser.Tag.EType.EExternVar, "gRTTIBinaryVersion")));
		}

		
		[Test]
		public void TestGetTagsFromFileWithMissingCtagsExe()
		{
			CtagsParser parser = new CtagsParser(@"d:\non\existing\path\to\ctags.exe");
			string errors = "";
            List<CtagsParser.Tag> input_tags = parser.GetTagsFromFile(TestUtils.sGetFullPath(@"IncludeCheckerLib\test\TestFile.h"), ref errors);
			Assert.IsTrue(errors.StartsWith("Error running ctags: "));
		}
		

		[Test]
		public void TestGetTagsFromFile()
		{
			CtagsParser parser = new CtagsParser(mCtagsPath);
			string errors = "";
            List<CtagsParser.Tag> input_tags = parser.GetTagsFromFile(TestUtils.sGetFullPath(@"IncludeCheckerLib\test\TestFile.h"), ref errors);
			Assert.AreEqual("", errors);
			
			// convert to generic
			List<CtagsParser.Tag> tags = new List<CtagsParser.Tag>();
			foreach (CtagsParser.Tag tag in input_tags)
				tags.Add(tag);
			int index = 0;
			Assert.AreEqual("ClassEnum", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EEnum, tags[index++].GetTagType());

			Assert.AreEqual("ClassEnumValue1", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EEnumerator, tags[index++].GetTagType());

			Assert.AreEqual("ClassEnumValue2", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EEnumerator, tags[index++].GetTagType());

			Assert.AreEqual("ClassFunc", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EPrototype, tags[index++].GetTagType());

			Assert.AreEqual("DEFINE_FUNCTION", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EMacro, tags[index++].GetTagType());

			Assert.AreEqual("DEFINE_IN_CLASS", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EMacro, tags[index++].GetTagType());

			Assert.AreEqual("DEFINE_IN_NAMESPACE", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EMacro, tags[index++].GetTagType());

			Assert.AreEqual("DEFINE_MULTILINE_FUNCTION", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EMacro, tags[index++].GetTagType());

			Assert.AreEqual("DEFINE_VALUE", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EMacro, tags[index++].GetTagType());

			Assert.AreEqual("EnumValue1", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EEnumerator, tags[index++].GetTagType());

			Assert.AreEqual("EnumValue2", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EEnumerator, tags[index++].GetTagType());

			Assert.AreEqual("ExternFunc", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EPrototype, tags[index++].GetTagType());

			Assert.AreEqual("ExternVar", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EExternVar, tags[index++].GetTagType());

			Assert.AreEqual("GlobalClass", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EClass, tags[index++].GetTagType());

			Assert.AreEqual("GlobalClass", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EPrototype, tags[index++].GetTagType());

			Assert.AreEqual("GlobalEnum", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EEnum, tags[index++].GetTagType());

			Assert.AreEqual("GlobalNamespace", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.ENamespace, tags[index++].GetTagType());

			Assert.AreEqual("GlobalVar", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EVar, tags[index++].GetTagType());

			Assert.AreEqual("IntFunc", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EPrototype, tags[index++].GetTagType());

			Assert.AreEqual("IntTypedef", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.ETypedef, tags[index++].GetTagType());

			Assert.AreEqual("JUST_DEFINE", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EMacro, tags[index++].GetTagType());

			Assert.AreEqual("VoidFunc", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EPrototype, tags[index++].GetTagType());

			Assert.AreEqual("VoidFuncInNamespace", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EPrototype, tags[index++].GetTagType());

			Assert.AreEqual("VolatileVar", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EVar, tags[index++].GetTagType());

			Assert.AreEqual("mClassVariable", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EMember, tags[index++].GetTagType());

			Assert.AreEqual("mStaticClassVariable", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EMember, tags[index++].GetTagType());

			Assert.AreEqual("~GlobalClass", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EPrototype, tags[index++].GetTagType());
		}


		[Test]
		public void TestGetTagsFromFileWithSpacesInFilename()
		{
			CtagsParser parser = new CtagsParser(mCtagsPath);
			string errors = "";
            List<CtagsParser.Tag> input_tags = parser.GetTagsFromFile(TestUtils.sGetFullPath(@"IncludeCheckerLib\test\Test File With Spaces.h"), ref errors);
			Assert.AreEqual("", errors);
			Assert.IsTrue(input_tags.Count > 0);
		}
		
		
		[Test]
		public void TestGetTagsFromComplicatedHeader()
		{
			CtagsParser parser = new CtagsParser(mCtagsPath);
			string errors = "";
            List<CtagsParser.Tag> input_tags = parser.GetTagsFromFile(TestUtils.sGetFullPath(@"IncludeCheckerLib\test\ComplicatedHeader.h"), ref errors);
			Assert.AreEqual("", errors);
			Assert.IsTrue(input_tags.Count > 0);

			// convert to generic
			List<CtagsParser.Tag> tags = new List<CtagsParser.Tag>();
			foreach (CtagsParser.Tag tag in input_tags)
				tags.Add(tag);
			int index = 0;

			Assert.AreEqual("memset", tags[index].GetName());
			Assert.AreEqual(CtagsParser.Tag.EType.EPrototype, tags[index++].GetTagType());
		}


        private bool CheckTag(string inCtagsLine, CtagsParser.Tag.EType inExpectedType, string inExpectedName)
        {
            bool result = true;
            CtagsParser parser = new CtagsParser("");
            CtagsParser.Tag tag = parser.GetTag(inCtagsLine);
            result = result && (inExpectedType == tag.GetTagType());
            result = result && (inExpectedName == tag.GetName());
            return result;
        }


        private string mCtagsPath = null;
    }
}
