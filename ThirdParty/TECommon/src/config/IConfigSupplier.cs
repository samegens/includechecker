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
// File:	IConfigSupplier.cs
// Created:	Mon Jun 14 08:45:41 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;

namespace TownleyEnterprises.Config {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This interface is used to provide a common abstraction to
///   accessing application configuration properties.
/// </summary>
/// <version>$Id: IConfigSupplier.cs,v 1.2 2004/06/22 12:04:13 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public interface IConfigSupplier
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property provides the name used to access the
	///   settings.  Normally, it is set when the instance is
	///   created.
	/// </summary>
	//////////////////////////////////////////////////////////////

	string AppName
	{
		get;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property returns the collection of keys in the
	///   supplier.
	/// </summary>
	//////////////////////////////////////////////////////////////

	ICollection Keys
	{
		get;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This indexer provides access to a configuration
	///   setting.  If the setting does not exist, a null is
	///   returned.
	/// </summary>
	//////////////////////////////////////////////////////////////

	string this[string key]
	{
		get;
		set;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property determines if the supplier is
	///   case-sensitive.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	bool IsCaseSensitive
	{
		get;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method checks to see if the supplier provides the
	///   specified key.
	/// </summary>
	/// <param name="key">the key to check</param>
	/// <returns>true if the key is provided by this
	/// supplier</returns>
	//////////////////////////////////////////////////////////////
	
	bool CanRead(string key);
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to determine if the key can be
	///   written to this supplier.
	/// </summary>
	/// <param name="key">the key to check</param>
	/// <returns>true if the key can be written by this
	/// supplier</returns>
	//////////////////////////////////////////////////////////////
	
	bool CanWrite(string key);
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method will cause the properties to be reloaded
	///   from their original source.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	void Load();
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method will cause the properties to be saved
	///   to their original source.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	void Save();
}

}
