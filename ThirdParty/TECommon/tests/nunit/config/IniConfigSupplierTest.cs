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
// File:	IniConfigSupplierTest.cs
// Created:	Wed Jun 23 09:35:03 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;
using NUnit.Framework;

using TownleyEnterprises.IO;

namespace TownleyEnterprises.Config {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   Tests for the IniConfigSupplier class.
/// </summary>  
/// <version>$Id: IniConfigSupplierTest.cs,v 1.2 2004/06/24 20:04:29 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

[TestFixture]
public sealed class IniConfigSupplierTest
{
	[SetUp]
	public void Init()
	{
		string s = Environment.GetEnvironmentVariable("TEST_DATA_DIR");
		config = new IniConfigSupplier("myapp",
			Path.Combine(s, "system.ini"), "windows");
	}

	[Test]
	public void VerifySimpleProperty()
	{
		Assert.AreEqual("mmdrv.dll", 
			config["Windows.Drivers.Wave"]);
	}
	
	[Test]
	public void VerifyRegistration()
	{
		AppConfig appconfig = new AppConfig("myapp");
		appconfig.RegisterConfigSupplier(config);

// FIXME:  bug in the appconfig class when resolving the mixed-case
// name...
//Serializer.WriteConfigAsProperties(Console.Out, appconfig);
		Assert.AreEqual(config.AppName, appconfig.AppName);
		Assert.AreEqual("mmdrv.dll", 
			appconfig["windows.drivers.wave"]);

	}

	private IniConfigSupplier config = null;
}

}
