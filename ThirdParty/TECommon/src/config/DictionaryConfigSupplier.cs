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
// File:	DictionaryConfigSupplier.cs
// Created:	Mon Jun 21 16:24:40 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Text;

namespace TownleyEnterprises.Config {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class is a Dictionary decorator which implements the
///   IConfigSupplier interface.
/// </summary>
/// <version>$Id: DictionaryConfigSupplier.cs,v 1.3 2004/06/23 14:47:01 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class DictionaryConfigSupplier: IConfigSupplier
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor takes the dictionary and gives it a name
	///   so that it can implement the interface.
	/// </summary>
	/// <param name="appName">the appname to be used by the
	/// supplier</param>
	/// <param name="d">the dictionary to decorate</param>
	//////////////////////////////////////////////////////////////
	
	public DictionaryConfigSupplier(string appName, IDictionary d)
	{
		_name = appName;
		_dict = d;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property provides the name used to access the
	///   settings.  Normally, it is set when the instance is
	///   created.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public virtual string AppName
	{
		get { return _name; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property returns the collection of keys in the
	///   supplier.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public virtual ICollection Keys
	{
		get { return _dict.Keys; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This indexer provides access to a configuration
	///   setting.  If the setting does not exist, a null is
	///   returned.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public virtual string this[string key]
	{
		get { return (string)_dict[key]; }
		set { _dict[key] = value; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property determines if the supplier is
	///   case-sensitive.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public virtual bool IsCaseSensitive
	{
		get { return true; }
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
	
	public virtual bool CanRead(string key)
	{
		return _dict.Contains(key);
	}
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to determine if the key can be
	///   written to this supplier.
	/// </summary>
	/// <param name="key">the key to check</param>
	/// <returns>true if the key can be written by this
	/// supplier</returns>
	//////////////////////////////////////////////////////////////
	
	public virtual bool CanWrite(string key)
	{
		return true;
	}
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method will cause the properties to be reloaded
	///   from their original source.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public virtual void Load()
	{
	}
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method will cause the properties to be saved
	///   to their original source.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public virtual void Save()
	{
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property provides a way for derived classes to
	///   refresh the underlying dictionary.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public virtual IDictionary Dictionary
	{
		get { return _dict; }
		set { _dict = value; }
	}

	public override string ToString()
	{
		StringBuilder buf = new StringBuilder();
		buf.Append(base.ToString());
		buf.Append("[");
		buf.Append(GetHashCode());
		buf.Append("]");

		return buf.ToString();
	}

	private readonly string		_name;

	// derived classes need to refresh the dictionary
	private IDictionary		_dict;
}

}
