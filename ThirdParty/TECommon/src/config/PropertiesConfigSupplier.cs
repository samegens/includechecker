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
// File:	PropertiesConfigSupplier.cs
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
///   This class decorates a Properties instance as an IConfigSupplier
///   implementation.
/// </summary>
/// <version>$Id: PropertiesConfigSupplier.cs,v 1.2 2004/06/28 06:51:43 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class PropertiesConfigSupplier: IConfigSupplier
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This constructor determines the path name automatically
	///   based on the underlying system defaults.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public PropertiesConfigSupplier(string appname)
		: this(appname, null)
	{
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor reads the properties from the specified
	///   configuration location.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public PropertiesConfigSupplier(string appname, string path)
	{
		string pname = String.Concat(appname, ".properties");
		if(path != null)
		{
			_propfile = Path.Combine(path, pname);
		}
		else
		{
			_propfile = pname;
		}
		_name = appname;
		_input = new PropertyFileProcessor(_propfile);
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
		get { return _props.Keys; }
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

		get { return _props[key]; }
		set { _props[key] = value; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property determines if the supplier is
	///   case-sensitive.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public virtual bool IsCaseSensitive
	{
		get { return _props.IsCaseSensitive; }
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
		return _props.Contains(key);
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
		_input.ProcessFile();
		_props = _input.Properties;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method will save the properties to their original
	///   source.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public virtual void Save()
	{
		// make a backup copy first
		File.Move(_propfile, 
			Paths.GetBackupFileName(_propfile, "~", false));

		StreamWriter sw = new StreamWriter(_propfile);
		sw.WriteLine("# Created {0}", DateTime.Now.ToString("u"));

		Serializer.WriteConfigAsProperties(sw, this);
		sw.Close();
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
	private string			_propfile = null;
	private PropertyFileProcessor	_input;
	private Properties		_props;
}

}
