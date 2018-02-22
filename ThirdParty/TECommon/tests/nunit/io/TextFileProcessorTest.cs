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
// File:	TextFileProcessorTest.cs
// Created:	Thu Jun 10 07:59:42 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;
using NUnit.Framework;

namespace TownleyEnterprises.IO {


//////////////////////////////////////////////////////////////////////
/// <summary>
///   This file implements tests for the TextFileProcessor class from
///   the IO package.
/// </summary>  
/// <version>$Id: TextFileProcessorTest.cs,v 1.5 2004/06/24 10:37:26 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

[TestFixture]
public sealed class TextFileProcessorTest
{
	private class BasicLineProcessor: AbstractLineProcessor
	{
		public override void ProcessLine(string line)
		{
			// make sure we count the lines
			base.ProcessLine(line);

			lines.Add(line);
		}

		public ArrayList lines = new ArrayList();
	}

	[Test]
	public void VerifySimpleParseUTF8WithDefaultEncoding()
	{
		string dataDir = Environment.GetEnvironmentVariable("TEST_DATA_DIR");
		Assert.IsNotNull(dataDir,
				"TEST_DATA_DIR environment variable should be set");

		TextFileProcessor processor = new TextFileProcessor(
					Path.Combine(dataDir, "text-file-utf8.txt"));
		BasicLineProcessor blp = new BasicLineProcessor();
		processor.ProcessFile(blp);

		// now, we check the data
		Assert.AreEqual(4, blp.LineCount,
				"input file should contain 4 lines");
		Assert.AreEqual("\u20AC4,000.00", blp.lines[0],
				"should correctly process the Euro sign");
		Assert.AreEqual("Nestl\u00E9", blp.lines[1],
				"should correctly process non-8859-15 characters");
		Assert.AreEqual("ESPA\u00D1A", blp.lines[2],
				"should correctly process non-8859-15 characters");
		Assert.AreEqual("Pla\u00E7a", blp.lines[3],
				"should correctly process non-8859-15 characters");
	}
	
	[Test]
	public void VerifySimpleParseWithCP1252Encoding()
	{
		string dataDir = Environment.GetEnvironmentVariable("TEST_DATA_DIR");
		Assert.IsNotNull(dataDir,
				"TEST_DATA_DIR environment variable should be set");

		TextFileProcessor processor = new TextFileProcessor(
					Path.Combine(dataDir, "text-file-cp1252.txt"),
					"windows-1252");
		BasicLineProcessor blp = new BasicLineProcessor();
		processor.ProcessFile(blp);

		// now, we check the data
		Assert.AreEqual(4, blp.LineCount,
				"input file should contain 4 lines");
		Assert.AreEqual("\u20AC4,000.00", blp.lines[0],
				"should correctly process the Euro sign");
		Assert.AreEqual("Nestl\u00E9", blp.lines[1],
				"should correctly process non-8859-15 characters");
		Assert.AreEqual("ESPA\u00D1A", blp.lines[2],
				"should correctly process non-8859-15 characters");
		Assert.AreEqual("Pla\u00E7a", blp.lines[3],
				"should correctly process non-8859-15 characters");
	}
	
	// NOTE:  this test doesn't seem to work with .NET
//	[Test]
//	public void VerifySimpleParseWith885915Encoding()
//	{
//		string dataDir = Environment.GetEnvironmentVariable("TEST_DATA_DIR");
//		Assert.IsNotNull(dataDir,
//				"TEST_DATA_DIR environment variable should be set");
//
//		TextFileProcessor processor = new TextFileProcessor(
//					Path.Combine(dataDir, "text-file-cp1252.txt"),
//					"ISO8859-15");
//		BasicLineProcessor blp = new BasicLineProcessor();
//		processor.ProcessFile(blp);
//
//		// now, we check the data
//		string s;
//		Assert.AreEqual(4, blp.LineCount,
//				"input file should contain 4 lines");
//		Assert.AreEqual("\u20AC4,000.00", blp.lines[0],
//				"should correctly process the Euro sign");
//		Assert.AreEqual("Nestl\u00E9", blp.lines[1],
//				"should correctly process non-8859-15 characters");
//		Assert.AreEqual("ESPA\u00D1A", blp.lines[2],
//				"should correctly process non-8859-15 characters");
//		Assert.AreEqual("Pla\u00E7a", blp.lines[3],
//				"should correctly process non-8859-15 characters");
//	}
}

}
