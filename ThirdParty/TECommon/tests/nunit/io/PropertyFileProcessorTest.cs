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
// File:	PropertyFileProcessorTest.cs
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
///   This file implements tests for the PropertyFileProcessor class
///   from the IO package.
/// </summary>  
/// <version>$Id: PropertyFileProcessorTest.cs,v 1.2 2004/06/23 14:50:27 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

[TestFixture]
public sealed class PropertyFileProcessorTest
{
	[SetUp]
	public void ParseFile()
	{
		string dataDir = Environment.GetEnvironmentVariable("TEST_DATA_DIR");
		Assert.IsNotNull(dataDir,
			"TEST_DATA_DIR environment variable should be set");
		
		PropertyFileProcessor pfp = new PropertyFileProcessor(
			Path.Combine(dataDir, "test.properties"));
		pfp.ProcessFile();
		props = pfp.Properties;
	}

	[Test]
	public void VerifySimpleProperty()
	{
		Assert.AreEqual("value", props["simple.property"]);
	}
	
	[Test]
	public void VerifyStripLeadingValueSpaces()
	{
		Assert.AreEqual("value", props["simple.property.lvs"]);
	}

	[Test]
	public void VerifyStripTrailingValueSpaces()
	{
		Assert.AreEqual("value", props["simple.property.tvs"]);
	}

	[Test]
	public void VerifyStripLeadingKeySpaces()
	{
		Assert.AreEqual("value", props["simple.property.lks"]);
	}

	[Test]
	public void VerifyStripTrailingKeySpaces()
	{
		Assert.AreEqual("value", props["simple.property.tks"]);
	}

	[Test]
	public void VerifyPrefix()
	{
		string dataDir = Environment.GetEnvironmentVariable("TEST_DATA_DIR");
		
		PropertyFileProcessor pfp = new PropertyFileProcessor(
			Path.Combine(dataDir, "test.properties"),
			false, "bogon");
		pfp.ProcessFile();
		props = pfp.Properties;
		Assert.AreEqual("value", props["bogon.simple.property.tks"]);
	}

	private Properties	props;
}

}
