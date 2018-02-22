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
// File:	IniFileProcessorTest.cs
// Created:	Tue Jun 15 22:01:46 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using TownleyEnterprises.Config;

namespace TownleyEnterprises.IO {


//////////////////////////////////////////////////////////////////////
/// <summary>
///   This file implements tests for the IniFileProcessor class from
///   the IO package.
/// </summary>  
/// <version>$Id: IniFileProcessorTest.cs,v 1.9 2004/06/24 10:32:59 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

[TestFixture]
public sealed class IniFileProcessorTest
{
	[SetUp]
	public void ParseFile()
	{
		string dataDir = Environment.GetEnvironmentVariable("TEST_DATA_DIR");
		Assert.IsNotNull(dataDir,
			"TEST_DATA_DIR environment variable should be set");
		
		processor = new IniFileProcessor(
			Path.Combine(dataDir, "browscap.ini"));
		processor.ShowWarnings = false;
		processor.ProcessFile();
	}

	[Test]
	public void VerifySimpleSection()
	{
		Assert.AreEqual(2279, processor.Sections.Count,
			"there should be 2279 sections");
		Assert.AreEqual(12806, processor.LineCount,
			"there should be 12806 lines");

		// check the Galeon 1.0 section
		ConfigSection section = processor["Galeon 1.0"];
		Assert.IsNotNull(section,
			"Galeon 1.0 should be in the file.");

		Assert.AreEqual("Galeon 1.0", section.Name);
		Assert.AreEqual("Galeon", section["browser"]);
		Assert.AreEqual("1", section["version"]);
		Assert.AreEqual("False", section["netclr"]);
	}
	
	[Test]
	public void VerifyCaseInsensitiveKeys()
	{
		ConfigSection section = processor["galeon 1.0"];
		Assert.AreEqual("Galeon", section["bRoWsEr"]);
	}

	[Test]
	public void VerifyDiscardDoubleQuotes()
	{
		ConfigSection section = processor["nunit"];
		Assert.AreEqual("double quote test", section["dqt"]);
	}

	[Test]
	public void VerifyDiscardSingleQuotes()
	{
		ConfigSection section = processor["nunit"];
		Assert.AreEqual("single quote test", section["sqt"]);
	}

	[Test]
	public void VerifyKeepInternalQuotes()
	{
		ConfigSection section = processor["nunit"];
		Assert.AreEqual("his mind was \"blown\" by the song",
			section["iqt"]);
	}

	[Test]
	public void VerifyKeepLeadQuotes()
	{
		ConfigSection section = processor["nunit"];
		Assert.AreEqual("\"there's a quote here",
			section["lqt"]);
	}

	[Test]
	public void VerifyKeepEndQuotes()
	{
		ConfigSection section = processor["nunit"];
		Assert.AreEqual("there's a quote at the end'",
			section["tqt"]);
	}

	[Test]
	public void VerifyStripLeadingValueSpaces()
	{
		ConfigSection section = processor["nunit"];
		Assert.AreEqual("value", section["lvs"]);
	}

	[Test]
	public void VerifyStripTrailingValueSpaces()
	{
		ConfigSection section = processor["nunit"];
		Assert.AreEqual("value", section["tvs"]);
	}

	[Test]
	public void VerifyStripLeadingKeySpaces()
	{
		ConfigSection section = processor["nunit"];
		Assert.AreEqual("value", section["lks"]);
	}

	[Test]
	public void VerifyStripTrailingKeySpaces()
	{
		ConfigSection section = processor["nunit"];
		Assert.AreEqual("value", section["tks"]);
	}

	[Test]
	public void VerifyParseEmptyValue()
	{
		ConfigSection section = processor["nunit"];
		Assert.AreEqual("", section["emptyvalue"]);
	}

	[Test]
	public void VerifyParseEmptyKey()
	{
		ConfigSection section = processor["nunit"];
		Assert.AreEqual("emptykey", section[""]);
	}

	[Test]
	public void VerifyMissingKeyIsNull()
	{
		ConfigSection section = processor["nunit"];
		Assert.IsNull(section["missing"],
			"missing key should return a null");
	}

	[Test]
	public void VerifyWindirResolution()
	{
		string windir = Environment.GetEnvironmentVariable("WINDIR");
		Assert.IsNotNull(windir,
			"WINDIR environment variable should be set (if on UNIX, should be set to TEST_DATA_DIR)");
		IniFileProcessor ini = new IniFileProcessor("system.ini");
		ini.ShowWarnings = false;
		ini.ProcessFile();

		ConfigSection section = ini["drivers"];
		Assert.AreEqual("mmdrv.dll", section["wave"]);
	}

	private IniFileProcessor	processor;
}

}
