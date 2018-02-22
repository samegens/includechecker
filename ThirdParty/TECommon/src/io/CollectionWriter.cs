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
// File:	CollectionWriter.cs
// Created:	Sat Jun 19 11:25:01 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;
using System.Text;

namespace TownleyEnterprises.IO {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class allows collections to be written to streams more
///   easily based on sensible ToString implementations of the
///   elements in the collections.
/// </summary>
/// <version>$Id: CollectionWriter.cs,v 1.2 2004/06/22 12:06:35 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class CollectionWriter: StreamWriter
{
	public CollectionWriter(Stream stream)
		: base(stream) {}
	
	public CollectionWriter(string s)
		:base(s) {}
	
	public CollectionWriter(Stream s, Encoding e)
		:base(s, e) {}
	
	public CollectionWriter(string s, bool b)
		:base(s, b) {}
	
	public CollectionWriter(Stream s, Encoding e, int x)
		:base(s, e, x) {}
	
	public CollectionWriter(string s, bool b, Encoding e)
		:base(s, b, e) {}
	
	public CollectionWriter(string s, bool b, Encoding e, int x)
		:base(s, b, e, x) {}
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This overloaded method specifically handles collections.
	/// </summary>
	/// <param name="c">the collection to write</param>
	/// <exception class="IOException">if an error occurs during
	/// the write</exception>
	//////////////////////////////////////////////////////////////
	
	public void Write(ICollection c)
	{
		Write(c, _delim);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This overloaded method specifically handles collections
	///   and allows delimiters to be specified all at the same
	///   time.
	/// </summary>
	/// <param name="c">the collection to write</param>
	/// <param name="d">the delimiter to use</param>
	/// <exception class="IOException">if an error occurs during
	/// the write</exception>
	//////////////////////////////////////////////////////////////
	
	public void Write(ICollection c, string d)
	{
		int i = 0;
		int count = c.Count;

		foreach(object o in c)
		{
			Write(o.ToString());
			if(d != null && ++i < count)
				Write(d);
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property sets the delimiter to print after each
	///   object in the collection.  The default is nothing.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public string Delimiter
	{
		get { return _delim; }
		set { _delim = value; }
	}

	private string _delim = "";
}

}
