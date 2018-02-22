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
// File:	WriteCaptureConfigResolver.cs
// Created:	Tue Jun 22 12:32:16 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Text;

using TownleyEnterprises.Common;

namespace TownleyEnterprises.Config {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class is used to install an IConfigResolver which will
///   capture all of the writes to the configuration settings,
///   regardless where the values came from.  Read operations work
///   normally unless the value has been changed.  In this case, the
///   new value is returned.
/// </summary>
/// <version>$Id: WriteCaptureConfigResolver.cs,v 1.1 2004/06/22 12:01:51 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class WriteCaptureConfigResolver: DefaultConfigResolver
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This constructor creates an empty instance which is used
	///   as a template.
	/// </summary>
	/// <param name="config">the supplier to use for
	/// writing</param>
	//////////////////////////////////////////////////////////////

	public WriteCaptureConfigResolver(IConfigSupplier write)
	{
		_write = write;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor takes a reference to config supplier
	///   instance to decorate.
	/// </summary>
	/// <param name="key">the key being resolved</param>
	/// <param name="config">the supplier with the actual
	/// data</param>
	/// <param name="write">parameter is used to propagate the
	/// output supplier</param>
	//////////////////////////////////////////////////////////////

	protected WriteCaptureConfigResolver(string key, 
				IConfigSupplier config,
				IConfigSupplier write)
		: base(key, config)
	{
		_write = write;
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

	public override string this[string key]
	{
		get
		{
			if(_changed)
				return _write[key];

			return base[key];
		}

		set
		{
			_changed = true;
			_write[key] = value;
		}
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

	public override IConfigResolver GetResolver(string key,
				IConfigSupplier supplier,
				IConfigResolver current)
	{
		// unfortunately, this is pretty much cut-n-paste code
		// reuse from the base class... :(

		IConfigResolver cr = null;

		// only do something if the supplier can read the key
		if(supplier.CanRead(key))
		{
			cr = new WriteCaptureConfigResolver(key, 
					supplier, _write);
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
	///   This method is used to determine if the key can be
	///   written to this supplier.
	/// </summary>
	/// <param name="key">the key to check</param>
	/// <returns>true if the key can be written by this
	/// supplier</returns>
	//////////////////////////////////////////////////////////////
	
	public override bool CanWrite(string key)
	{
		return true;
	}
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method will cause the properties to be saved
	///   to their original source.
	/// </summary>
	/// <exception class="System.IO.IOException">if the save
	/// operation fails</exception>
	//////////////////////////////////////////////////////////////
	
	public override void Save()
	{
		_write.Save();
	}

	private readonly IConfigSupplier	_write = null;
	private bool				_changed = false;
}

}
