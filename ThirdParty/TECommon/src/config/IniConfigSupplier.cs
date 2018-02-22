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
// File:	IniConfigSupplier.cs
// Created:	Tue Jun 22 22:21:39 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;
using System.Text;

using TownleyEnterprises.Common;
using TownleyEnterprises.IO;

namespace TownleyEnterprises.Config {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   <para>
///   This class provides an IConfigSupplier instance based on the
///   contents of an INI file.
///   </para>
///   <para>
///   Note:  there are loads of assumptions in this class.  The main
///   one is that given the following key:
///   <c>some.really.long.key</c>, the resolution of that will be
///   based on the following:
///   <ol>
///   <li>The prefix (if any) will be stripped</li>
///   <li>The value will be the last string following the last
///   "dot"</li>
///   <li>Everything else will be used as the section name.
///   </ol>
///   </para>
/// </summary>
/// <version>$Id: IniConfigSupplier.cs,v 1.2 2004/06/24 20:03:43 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class IniConfigSupplier: IConfigSupplier
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor reads the properties from the specified
	///   configuration location.
	/// </summary>
	/// <param name="appname">the application name</param>
	/// <param name="filename">the INI file to load</param>
	//////////////////////////////////////////////////////////////
	
	public IniConfigSupplier(string appname, string filename)
		: this(appname, filename, null)
	{
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor reads the properties from the specified
	///   configuration location.  All of the sections within the
	///   file will be prefixed with the given prefix.
	/// </summary>
	/// <param name="appname">the application name</param>
	/// <param name="filename">the INI file to load</param>
	/// <param name="prefix">the prefix to use</param>
	//////////////////////////////////////////////////////////////
	
	public IniConfigSupplier(string appname, string filename,
			string prefix)
	{
		_name = appname;
		_inifile = filename;
		if(prefix != null)
			_prefix = prefix + ".";

		_input = new IniFileProcessor(_inifile);
		Load();
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
		get { return _keys.Keys; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This indexer provides access to a configuration
	///   setting.  If the setting does not exist, a null is
	///   returned.
	/// </summary>
	/// <exception class="System.NotSupportedException">if the
	/// value cannot be set</exception>
	//////////////////////////////////////////////////////////////

	public virtual string this[string key]
	{
		// FIXME:  should really check if the file is writable
		// before allowing set

		get
		{
//Console.WriteLine("key:  " + key);
			string[] path = GetPathComponents(key.ToLower());
			ConfigSection section = _input[path[0]];
			if(section == null)
			{
//Console.WriteLine("section " + path[0] + " was not found.");
				return null;
			}

			return section[path[1]];
		}
		set
		{
			// FIXME:  this looks a bit dodgy, but I'm in a hurry...
			// need to add some unit tests
			string[] path = GetPathComponents(key.ToLower());
			string mkey = _prefix + path[0];
			if(!_keys.Contains(mkey))
			{
				_keys[mkey] = path[0];
			}

			_input[path[0]][path[1]] = value;
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property determines if the supplier is
	///   case-sensitive.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public virtual bool IsCaseSensitive
	{
		get { return false; }
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
		return _keys.Contains(key);
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
//		return !(File.GetAttributes(_propfile) &
//				FileAttributes.ReadOnly);

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
//Console.WriteLine("name:  " + _name);
//Console.WriteLine("inifile:  " + _inifile);
//Console.WriteLine("prefix:  " + _prefix);
		_keys = new Hashtable();
		_input.ProcessFile();

		// load the sections
		foreach(ConfigSection section in _input.Sections)
		{
			string s = (_prefix + section.Name).ToLower();
//Console.WriteLine("added alias '{0}' for '{1}'", s, section.Name);
			foreach(string s2 in section.Keys)
			{
				_keys[s + "." + s2] = s2;
			}
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method will save the properties to their original
	///   source.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public virtual void Save()
	{
//		// make a backup copy first
//		File.Move(_propfile, 
//			Paths.GetBackupFileName(_propfile, "~", false));
//
//		StreamWriter sw = new StreamWriter(_propfile);
//		sw.WriteLine("# Created {0}", DateTime.Now.ToString("u"));
//
//		Serializer.WriteConfigAsIni(sw, this);
//		sw.Close();
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

	// FIXME:  currently has a bug in support for section value names
	// including a '.'
	
	private string[] GetPathComponents(string name)
	{
		string key = Paths.Basename(name, ".");
		string section = Paths.Suffix(name, _prefix);

//Console.WriteLine("section:  '{0}'; key:  '{1}'", section, key);

		string[] pe = section.Split('.');
		if((_prefix != null && pe.Length >= 2)
				|| (_prefix == null && pe.Length >= 1))
		{
			section = section.Substring(0,
					section.Length - key.Length - 1);
		}
		else
		{
			section = key;
			key = "";
		}

//Console.WriteLine("section:  '{0}'; key:  '{1}'", section, key);
		return (new string[] { section, key });
	}

	private readonly string		_name;
	private IniFileProcessor	_input;
	private string			_inifile = null;
	private string			_prefix = null;
	private Hashtable		_keys;
}

}
