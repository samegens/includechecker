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
// File:	Serializer.cs
// Created:	Tue Jun 22 22:41:30 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;
using System.Text;

using TownleyEnterprises.Config;

namespace TownleyEnterprises.IO {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This utility class provides various ways of serializing common
///   data structures to recurring serialized forms.
/// </summary>
/// <version>$Id: Serializer.cs,v 1.1 2004/06/23 14:39:11 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public sealed class Serializer
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to serialize the IDictionary to a
	///   format which more-or-less follows the key=value form
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public static string DictToPropertiesString(IDictionary dict)
	{
		StringBuilder buf = new StringBuilder();

		foreach(string key in dict.Keys)
		{
			buf.Append(key);
			buf.Append("=");
			buf.Append(dict[key]);
			buf.Append("\n");
		}

		return buf.ToString();
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to serialize the IConfigSupplier to a
	///   format which more-or-less follows the key=value form
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public static string ConfigToPropertiesString(IConfigSupplier dict)
	{
		StringBuilder buf = new StringBuilder();

		foreach(string key in dict.Keys)
		{
			buf.Append(key);
			buf.Append("=");
			buf.Append(dict[key]);
			buf.Append("\n");
		}

		return buf.ToString();
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method writes the dictionary in property format to
	///   the specified TextWriter.
	/// </summary>
	/// <param name="writer">the writer to use</param>
	/// <param name="dict">the dictionary to write</param>
	/// <exception class="System.IO.IOException">if the write
	/// fails</exception>
	//////////////////////////////////////////////////////////////
	
	public static void WriteDictAsProperties(TextWriter writer,
				IDictionary dict)
	{
		writer.Write(DictToPropertiesString(dict));
	}
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method writes the config supplier in property
	///   format to the specified TextWriter.
	/// </summary>
	/// <param name="writer">the writer to use</param>
	/// <param name="config">the supplier to write</param>
	/// <exception class="System.IO.IOException">if the write
	/// fails</exception>
	//////////////////////////////////////////////////////////////
	
	public static void WriteConfigAsProperties(TextWriter writer,
				IConfigSupplier dict)
	{
		writer.Write(ConfigToPropertiesString(dict));
	}
	
	// prevent instances
	private Serializer() {}
}

}
