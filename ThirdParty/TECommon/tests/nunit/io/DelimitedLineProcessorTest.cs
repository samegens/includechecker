//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2004, Andrew S. Townley
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
// 
//     * Redistributions of source code must retain the above
//     copyright notice, this list of conditions and the following
//     disclaimer.
// 
//     * Redistributions in binary form must reproduce the above
//     copyright notice, this list of conditions and the following
//     disclaimer in the documentation and/or other materials provided
//     with the distribution.
// 
//     * Neither the names Andrew Townley and Townley Enterprises,
//     Inc. nor the names of its contributors may be used to endorse
//     or promote products derived from this software without specific
//     prior written permission.  
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
// FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
// COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
// OF THE POSSIBILITY OF SUCH DAMAGE.
//
// File:	DelimitedLineProcessorTest.cs
// Created:	Thu Jun 10 09:10:41 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;
using NUnit.Framework;

namespace TownleyEnterprises.IO {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This file implements tests for the DelimitedLineProcessor class
///   from the IO package.
/// </summary>  
/// <version>$Id: DelimitedLineProcessorTest.cs,v 1.2 2004/06/15 20:28:49 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

[TestFixture]
public sealed class DelimitedLineProcessorTest
{
	private class Processor: DelimitedLineProcessor
	{
		public Processor(string del) : base(del) {}

		public override void ProcessItems(IList list)
		{
			linelist.Add(list);
		}
	}

	[SetUp]
	public void ParseFile()
	{
		string dataDir = Environment.GetEnvironmentVariable("TEST_DATA_DIR");
		Assert.IsNotNull(dataDir,
				"TEST_DATA_DIR environment variable should be set");
		
		TextFileProcessor processor = new TextFileProcessor(
					Path.Combine(dataDir, "delimited-file.txt"));
		processor.ProcessFile(new Processor("|"));
	}

	[Test]
	public void VerifiyEmptyItemOne()
	{
		ArrayList line = (ArrayList)linelist[0];

		Assert.AreEqual(7, line.Count,
			"should parse right number of items");
		Assert.AreEqual("", line[0]);
		Assert.AreEqual("this", line[1]);
		Assert.AreEqual("line", line[2]);
		Assert.AreEqual("has", line[3]);
		Assert.AreEqual("no", line[4]);
		Assert.AreEqual("item", line[5]);
		Assert.AreEqual("one", line[6]);
	}

	[Test]
	public void VerifiyNormalParse()
	{
		ArrayList line = (ArrayList)linelist[1];
		
		Assert.AreEqual(5, line.Count,
			"should parse right number of items");
		Assert.AreEqual("This", line[0]);
		Assert.AreEqual("is", line[1]);
		Assert.AreEqual("the", line[2]);
		Assert.AreEqual("first", line[3]);
		Assert.AreEqual("line", line[4]);
	}

	[Test]
	public void VerifiyEmptyLastToken()
	{
		ArrayList line = (ArrayList)linelist[3];
		
		Assert.AreEqual(7, line.Count,
			"should parse right number of items");
		Assert.AreEqual("this", line[0]);
		Assert.AreEqual("line", line[1]);
		Assert.AreEqual("has", line[2]);
		Assert.AreEqual("no", line[3]);
		Assert.AreEqual("last", line[4]);
		Assert.AreEqual("token", line[5]);
		Assert.AreEqual("", line[6]);
	}

	[Test]
	public void VerifiyMissingToken()
	{
		ArrayList line = (ArrayList)linelist[4];
		
		Assert.AreEqual(7, line.Count,
			"should parse right number of items");
		Assert.AreEqual("this", line[0]);
		Assert.AreEqual("", line[1]);
		Assert.AreEqual("line", line[2]);
		Assert.AreEqual("is", line[3]);
		Assert.AreEqual("missing", line[4]);
		Assert.AreEqual("a", line[5]);
		Assert.AreEqual("token", line[6]);
	}

	static ArrayList linelist = new ArrayList();
}

}
