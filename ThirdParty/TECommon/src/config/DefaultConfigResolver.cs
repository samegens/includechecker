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
// File:	DefaultConfigResolver.cs
// Created:	Mon Jun 21 15:08:33 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Text;

using TownleyEnterprises.Common;

namespace TownleyEnterprises.Config {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class is responsible for managing parent-child override
///   relationships between the registered config supplier.  It is a
///   decorator for a single reference to an IConfigSupplier instance
///   delegate.
/// </summary>
/// <version>$Id: DefaultConfigResolver.cs,v 1.2 2004/06/23 14:46:32 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class DefaultConfigResolver: IConfigResolver
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This constructor creates an empty instance which is used
	///   as a template.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public DefaultConfigResolver()
	{
		_debug = new DebugWriter(this);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor takes a reference to config supplier
	///   instance to decorate.
	/// </summary>
	/// <param name="key">the key being resolved</param>
	/// <param name="config">the supplier with the actual
	/// data</param>
	//////////////////////////////////////////////////////////////

	protected DefaultConfigResolver(string key, IConfigSupplier config)
	{
		_key = key;
		_config = config;
		_debug = new DebugWriter(this);
		_debug.WriteLine(
			string.Format("created resolver {0}:{1} = '{2}'",
				config, key, config[key]));
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   Property which allows manipulating the parent resolver.
	/// </summary>
	/// <param name="parent">the parent resovler</param>
	//////////////////////////////////////////////////////////////

	public virtual IConfigResolver Parent
	{
		get { return _parent; }
		set
		{
			_debug.WriteLine("new parent:  " + value);
			_parent = value;
		}
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
		get { return _config.AppName; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property returns the collection of keys in the
	///   supplier.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public virtual ICollection Keys
	{
		get { return _config.Keys; }
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
		get
		{
			string val = null;

			if(_parent != null)
			{
				val = _parent[key];

				_debug.WriteLine(string.Format("resolving from parent; {0} = '{1}'", key, val));
			}
			else
			{
				val = _config[key];
				_debug.WriteLine(string.Format("resolving from config; {0} = '{1}'", key, val));
			}

			return val;
		}

		set
		{
			if(_parent != null && _parent.CanWrite(key))
			{
				_debug.WriteLine(string.Format("saving to parent; {0} = '{1}'", key, value));
				_parent[key] = value;
			}
			else if(_config.CanWrite(key))
			{
				_debug.WriteLine(string.Format("saving to config; {0} = '{1}'", key, value));
				_config[key] = value;
			}
			else
			{
				throw new NotSupportedException(key);
			}
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
		get { return _config.IsCaseSensitive; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This factory method returns the appropriate resolver to
	///   the consumer.  The definition of "appropriate" is left
	///   to the implementation of the config resolver.
	/// </summary>
	/// <param name="key">the key to be tested</param>
	/// <param name="supplier">the potential new supplier</param>
	/// <param name="current">the current supplier</param>
	//////////////////////////////////////////////////////////////

	public virtual IConfigResolver GetResolver(string key,
				IConfigSupplier supplier,
				IConfigResolver current)
	{
		IConfigResolver cr = null;

		if(_debug.IsActive)
		{
			_debug.WriteLine("template:  getting resolver for key:  " + key);
			_debug.WriteLine("template:  supplier:  " + supplier);
			_debug.WriteLine("template:  current:  " + current);
		}

		// only do something if the supplier can read the key
		if(supplier.CanRead(key))
		{
			cr = new DefaultConfigResolver(key, supplier);
		}
		else
		{
			return current;
		}

		// if we get here, the supplier can read the key, so
		// it is just a matter of hooking up the relationships
		if(current != null)
		{
			current.Parent = cr;
			cr = current;
		}

		return cr;
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
		return _config.CanRead(key);
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
		return _config.CanWrite(key);
	}
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method will cause the properties to be reloaded
	///   from their original source.
	/// </summary>
	/// <exception class="System.IO.IOException">if the load
	/// operation fails</exception>
	//////////////////////////////////////////////////////////////
	
	public virtual void Load()
	{
		_config.Load();
	}
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method will cause the properties to be saved
	///   to their original source.
	/// </summary>
	/// <exception class="System.IO.IOException">if the save
	/// operation fails</exception>
	//////////////////////////////////////////////////////////////
	
	public virtual void Save()
	{
		_config.Save();
	}

	public override string ToString()
	{
		// this is debugging stuff
		//StringBuilder buf = new StringBuilder();
		//buf.Append(base.ToString());
		//buf.Append("[");
		//buf.Append(GetHashCode());
		//buf.Append("]");

		//return buf.ToString();
		
		return this[_key];
	}

	private readonly IConfigSupplier	_config;
	private IConfigResolver			_parent = null;
	private DebugWriter			_debug;
	private string				_key = null;
}

}
