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
// File:	ConfigRegistry.cs
// Created:	Mon Jun 21 15:05:51 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Text;

namespace TownleyEnterprises.Config {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class provides an implementation of the Registry pattern
///   for system-wide configuration.
/// </summary>
/// <version>$Id: ConfigRegistry.cs,v 1.2 2004/06/23 14:45:44 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public sealed class ConfigRegistry
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to register a config supplier. 
	/// </summary>
	/// <param name="supplier">the supplier to register</param>
	//////////////////////////////////////////////////////////////
	
	public static void RegisterSupplier(IConfigSupplier supplier)
	{
		AppConfig ac = GetConfig(supplier.AppName);
		ac.RegisterConfigSupplier(supplier);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to retrieve a reference to the
	///   specified AppConfig instance.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public static AppConfig GetConfig(string name)
	{
		AppConfig ac = (AppConfig)_hash[name];
		if(ac == null)
		{
			ac = new AppConfig(name);
			_hash[name] = ac;
		}
	
		return ac;
	}

	// create a shared environment resolver
	internal static IConfigSupplier Environment = new EnvConfigSupplier();

	private static Hashtable	_hash = new Hashtable();

	// prevent instances
	private ConfigRegistry() {}
}

}
