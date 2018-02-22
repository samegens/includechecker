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
// File:	AppConfig.cs
// Created:	Mon Jun 14 08:45:41 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;

namespace TownleyEnterprises.Config {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class is used to group all of the settings for a given
///   application into a common place.
/// </summary>
/// <version>$Id: AppConfig.cs,v 1.4 2004/06/24 20:03:03 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public sealed class AppConfig: IConfigSupplier
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor takes the name of the application
	///   managed by these settings.
	/// </summary>
	/// <param name="name">the application name</param>
	//////////////////////////////////////////////////////////////
	
	public AppConfig(string name)
	{
		_name = name;
		RegisterConfigSupplier(ConfigRegistry.Environment);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to register a configuration supplier
	///   for this application.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public void RegisterConfigSupplier(IConfigSupplier supplier)
	{
		if(supplier.AppName != null
				&& _name != supplier.AppName)
		{
//Console.WriteLine("supplier name:  '{0}'", supplier.AppName);
//Console.WriteLine("my name:        '{0}'", _name);
//Console.WriteLine("skipped");
			// don't do anything if the names don't match
			return;
		}

		foreach(string key in supplier.Keys)
		{
			_hash[key] = _template.GetResolver(key,
					supplier,
					(IConfigResolver)_hash[key]);
		}

		_suppliers.Add(supplier);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to register a configuration supplier
	///   for this application.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public void UnregisterConfigSupplier(IConfigSupplier supplier)
	{
		// FIXME:  figure out how to implement this cleanly
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property allows manipulation of the template
	///   strategy for configuration resolution.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public IConfigResolver TemplateResolver
	{
		get { return _template; }
		set { _template = value; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property provides the name used to access the
	///   settings.  Normally, it is set when the instance is
	///   created.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public string AppName
	{
		get { return _name; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property returns the collection of keys in the
	///   supplier.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public ICollection Keys
	{
		get { return _hash.Keys; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This indexer provides access to a configuration
	///   setting.  If the setting does not exist, a null is
	///   returned.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public string this[string key]
	{
		get
		{
			IConfigResolver cr = (IConfigResolver)_hash[key];
			if(cr != null)
				return cr[key];
			
			return null;
		}

		set
		{
			IConfigResolver cr = (IConfigResolver)_hash[key];
			if(cr != null)
				cr[key] = value;
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property determines if the supplier is
	///   case-sensitive.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public bool IsCaseSensitive
	{
		get { return !_ignoreCase; }
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
	
	public bool CanRead(string key)
	{
		return _hash.Contains(key);
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
	
	public bool CanWrite(string key)
	{
		IConfigSupplier cs = (IConfigSupplier)_hash[key];
		if(cs != null)
			return cs.CanWrite(key);

		return false;
	}
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method will cause the properties to be reloaded
	///   from their original source.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public void Load()
	{
		foreach(IConfigSupplier supplier in _suppliers)
		{
			supplier.Load();
		}
	}
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method will cause the properties to be saved
	///   to their original source.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public void Save()
	{
		foreach(IConfigSupplier supplier in _suppliers)
		{
			supplier.Save();
		}
	}

	private Hashtable	_hash = new Hashtable();
	private ArrayList	_suppliers = new ArrayList();
	private string		_name;
	private bool		_ignoreCase = true;
	private IConfigResolver	_template = new DefaultConfigResolver();
}

}
