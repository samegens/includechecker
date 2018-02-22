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
// File:	ConfigSection.cs
// Created:	Tue Jun 15 17:58:47 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Text;

using TownleyEnterprises.IO;

namespace TownleyEnterprises.Config {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class represents a section in the "classic" Windows Profile
///   or INI file but also represents more generalized named groups of
///   configuration settings.  Sections are named sets of key/value
///   pairs which are essentially the same as named Hashtables (which
///   is how this class is currently implemented).
/// </summary>
/// <version>$Id: ConfigSection.cs,v 1.1 2004/06/23 08:13:02 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class ConfigSection: Properties
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor provides the name for the section.  The
	///   section and key values will be completely
	///   case-insensitive.
	/// </summary>
	/// <param name="name">the name of the section</param>
	//////////////////////////////////////////////////////////////
	
	public ConfigSection(string name)
		: this(name, null, true, null)
	{
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This constructor provides initializers for the name and
	///   the property resolution.
	/// </summary>
	/// <param name="name">the name of the section</param>
	/// <param name="ignoreCase">true if the properties are not
	/// case-sensitive</param>
	//////////////////////////////////////////////////////////////
	
	public ConfigSection(string name, bool ignoreCase)
		: this(name, null, ignoreCase, null)
	{
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This constructor fully initializes the instance.
	/// </summary>
	/// <param name="name">the name of the section</param>
	/// <param name="defaults">the default values</param>
	/// <param name="ignoreCase">true if the properties are not
	/// case-sensitive</param>
	/// <param name="prefix">a prefix to append to each of the
	/// property values.  Prefixes are prepended to all property
	/// names and are separated by the <c>.</c> character.</param>
	//////////////////////////////////////////////////////////////
	
	public ConfigSection(string name, Properties defaults,
				bool ignoreCase, string prefix)
		: base(defaults, ignoreCase, prefix)
	{
		_name = name;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property provides access to the section's name.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public string Name
	{
		get { return _name; }
		set { _name = value; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to output the entire section as a
	///   string.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public override string ToString()
	{
		StringBuilder buf = new StringBuilder("[");
		buf.Append(_name);
		buf.Append("]");
		buf.Append("\n");

		buf.Append(base.ToString());
		
		return buf.ToString();
	}

	private string		_name = "";
}

}
