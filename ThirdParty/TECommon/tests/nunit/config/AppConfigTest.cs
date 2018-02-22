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
// File:	AppConfigTest.cs
// Created:	Mon Jun 21 16:30:24 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;
using NUnit.Framework;

namespace TownleyEnterprises.Config {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   Tests for the AppConfig class.
/// </summary>  
/// <version>$Id: AppConfigTest.cs,v 1.2 2004/06/22 11:57:08 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

[TestFixture]
public sealed class AppConfigTest
{
	[SetUp]
	public void InitConfig()
	{
		config = new AppConfig("actest");
		ht1 = new Hashtable();
		ht1["key1"] = "val1";
		ht1["key2"] = "val2";
		ht1["key3"] = "val3";

		IConfigSupplier cs1 = 
			new DictionaryConfigSupplier("actest", ht1);
		config.RegisterConfigSupplier(cs1);

		ht2 = new Hashtable();
		ht2["key2"] = "override";
	
		IConfigSupplier cs2 = 
			new DictionaryConfigSupplier("actest", ht2);
		config.RegisterConfigSupplier(cs2);

		ht3 = new Hashtable();
		ht3["eggs"] = "green";
		ht3["ham"] = "true";
		ht3["key3"] = "true";
		
		IConfigSupplier cs3 = 
			new DictionaryConfigSupplier("actest", ht3);
		config.RegisterConfigSupplier(cs3);
		
//Console.WriteLine("cs1 hash code:  {0}", cs1.GetHashCode());
//Console.WriteLine("cs2 hash code:  {0}", cs2.GetHashCode());
	}

	[Test]
	public void VerifySimpleProperty()
	{
		Assert.AreEqual("val1", config["key1"]);
	}
	
	[Test]
	public void VerifyOverrideProperty()
	{
		Assert.AreEqual("override", config["key2"]);
	}

	[Test]
	public void VerifyOverrideProperty2ndLevel()
	{
		Assert.AreEqual("true", config["key3"]);
	}

	[Test]
	public void VerifyIgnoreSettings()
	{
		Hashtable ht = new Hashtable();
		ht["ht"] = "bogus";
		
		config.RegisterConfigSupplier(
			new DictionaryConfigSupplier("bogus", ht));
		
		Assert.IsNull(config["ht"], "value should be ignored because application name is not the same");
	}

	[Test]
	public void VerifyEmptySettings()
	{
		Hashtable ht = new Hashtable();
		ht["key1"] = null;
		ht["key2"] = null;
		ht["key3"] = null;
		
		config.RegisterConfigSupplier(
			new DictionaryConfigSupplier("actest", ht));
		
		Assert.IsNull(config["key1"], "value should override existing setting.");
		Assert.IsNull(config["key2"], "value should override existing setting.");
		Assert.IsNull(config["key3"], "value should override existing setting.");
	}

	[Test]
	public void VerifyBasicPropertySave()
	{
		config["key1"] = "new value";
		Assert.AreEqual("new value", ht1["key1"]);
	}

	[Test]
	public void VerifyOverridePropertySave()
	{
		config["key2"] = "new value";
		Assert.AreEqual("new value", ht2["key2"]);
	}

	[Test]
	public void VerifyWriteCapturingResolver()
	{
		AppConfig ac = new AppConfig("actest");
		Hashtable hash = new Hashtable();
		ac.TemplateResolver = new WriteCaptureConfigResolver(
			new DictionaryConfigSupplier("actest", hash));

		// cheat and use the already initialized values
		ac.RegisterConfigSupplier(config);

		ac["key1"] = "write capture1";
		ac["key2"] = "write capture2";
		Assert.AreEqual("write capture1", ac["key1"]);
		Assert.AreEqual("write capture2", ac["key2"]);
		Assert.AreEqual("write capture1", hash["key1"]);
		Assert.AreEqual("write capture2", hash["key2"]);
		Assert.AreEqual("val1", config["key1"]);
		Assert.AreEqual("override", config["key2"]);
		Assert.AreEqual("val1", ht1["key1"]);
		Assert.AreEqual("override", ht2["key2"]);
	}

	private Hashtable	ht1;
	private Hashtable	ht2;
	private Hashtable	ht3;
	private AppConfig	config;
}

}
