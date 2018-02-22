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
// File:	Properties.cs
// Created:	Sun Jun 20 21:38:37 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Text;

using TownleyEnterprises.Common;
using TownleyEnterprises.IO;

namespace TownleyEnterprises.Config {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   <para>
///   This class represents an un-named set of configuration
///   properties.  Current examples in the wild of such things are the
///   system's environment variables and the J2SE properties
///   mechanism.
///   </para>
///   <para>
///   This class encapsulates all of these different storage mechanism
///   into a common set of functionality.
///   </para>
/// </summary>
/// <version>$Id: Properties.cs,v 1.3 2004/06/23 14:47:52 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class Properties
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   Creates an empty properties set with no values.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public Properties()
		: this(null, false, null)
	{
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   Creates a new properties object initialized with the
	///   values from another properties set.
	/// </summary>
	/// <param name="defaults">the default values</param>
	//////////////////////////////////////////////////////////////
	
	public Properties(Properties defaults)
		: this(defaults, false, null)
	{
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This constructor gives more control over the behavior of
	///   the properties regarding the resolution.
	/// </summary>
	/// <param name="defaults">the default values</param>
	/// <param name="ignoreCase">true if the properties are not
	/// case-sensitive</param>
	/// <param name="prefix">a prefix to append to each of the
	/// property values.  Prefixes are prepended to all property
	/// names and are separated by the <c>.</c> character.</param>
	//////////////////////////////////////////////////////////////
	
	public Properties(Properties defaults, 
				bool ignoreCase, string prefix)
	{
		if(defaults != null)
		{
			_hash = new Hashtable(defaults._hash);
		}
		else
		{
			_hash = new Hashtable();
		}

		_ignoreCase = ignoreCase;

		if(prefix != null)
			_prefix = prefix + ".";
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   Indexer to provide values as strings.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public virtual string this[string key]
	{
		get
		{
			string rkey = GetKeyName(key, false);
//Console.WriteLine("GetKeyName:  " + rkey);
			return (string)_hash[rkey];
		}
		set { _hash[GetKeyName(key, true)] = value; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property returns the keys of the properties stored
	///   in the instance.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public virtual ICollection Keys
	{
		get
		{
			if(_prefix == null || _ignoreCase == true)
				return _hash.Keys;

			return _keys.Keys;
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property provides access to the raw keys as
	///   initialized by whatever created the section.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public virtual ICollection RawKeys
	{
		get { return _hash.Keys; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property returns the values of the items in the
	///   instance.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public virtual ICollection Values
	{
		get { return _hash.Values; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method clears the contents of the instance.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public virtual void Clear()
	{
		_keys.Clear();
		_hash.Clear();
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to determine if an item exists in
	///   the properties instance.
	/// </summary>
	/// <param name="key">the key to check</param>
	/// <returns>true if the key exists; false otherwise</returns>
	//////////////////////////////////////////////////////////////

	public virtual bool Contains(string key)
	{
		return _hash.Contains(GetKeyName(key, false));
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property determines if the properties object is
	///   case-sensitive for property resolution.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public virtual bool IsCaseSensitive
	{
		get { return !_ignoreCase; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property retrieves the prefix applied to the
	///   properties.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public virtual string Prefix
	{
		get { return _prefix; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to output the entire section as a
	///   string.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public override string ToString()
	{
		return Serializer.DictToPropertiesString(_hash);
	}

	// FIXME:  this needs to be investigated again because i'm
	// sure it can be more efficient

	private string GetKeyName(string key, bool writing)
	{
		if(_prefix == null && !_ignoreCase)
			return key;

//Console.WriteLine("requested key name for key '{0}'; writing = {1}; prefix = '{2}'; _ignoreCase = {3}", key, writing, _prefix, _ignoreCase);

		if(!writing && _ignoreCase)
		{
			key = key.ToLower();
		}

//Console.WriteLine(Serializer.DictToPropertiesString(_keys));
//Console.WriteLine("\n");
//Console.WriteLine(Serializer.DictToPropertiesString(_hash));
		string s = (string)_keys[key];
		if(s == null && writing)
		{
			s = Paths.Suffix(key, _prefix);
			if(s == key && _prefix != null)
			{
				// special case during initialization
				s = _prefix + key;
			}
			
			if(_ignoreCase)
			{
				s = s.ToLower();
			}

//Console.WriteLine("adding alias '{0}' for '{1}'", s, key);
			_keys[s] = key;
			s = key;
		}
		else if(s == null && !writing)
		{
			s = key;
		}

//Console.WriteLine("GetKeyName:  returning '{0}'", s);
		return s;
	}

	private Hashtable		_keys = new Hashtable();
	private readonly Hashtable	_hash = null;
	private readonly bool		_ignoreCase = false;
	private readonly string		_prefix = null;
}

}
