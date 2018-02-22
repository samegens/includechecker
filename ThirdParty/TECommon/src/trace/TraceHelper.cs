//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2001-2004, Andrew S. Townley
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
// Name:	TraceHelper.cs
// Created:	Sat Jun 26 17:15:54 IST 2004
//
///////////////////////////////////////////////////////////////////////

using System;
using System.Collections;

namespace TownleyEnterprises.Trace {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class provides trace representations of various standard
///   data structures.
/// </summary>
/// <version>$Id: TraceHelper.cs,v 1.1 2004/06/28 06:50:15 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public sealed class TraceHelper
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method generates a trace representation of a
	///   dictionary.
	/// </summary>
	/// <param name="m">the dictionary</param>
	/// <returns>a object representing the contents of the
	/// dictionary</returns>
	//////////////////////////////////////////////////////////////

	public static object Trace(IDictionary m)
	{
		return Trace(m, DEFAULT_SEP);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This overloaded method takes the object separator
	///   string.
	/// </summary>
	/// <param name="m">the dictionary</param>
	/// <returns>a object representing the contents of the
	/// dictionary</returns>
	//////////////////////////////////////////////////////////////

	public static object Trace(IDictionary m, String sep)
	{
		TraceMessage buf = new TraceMessage("{ ");

		int i = 0;
		int count = m.Keys.Count;
		foreach(object key in m.Keys)
		{
			if(key is string)
			{
				buf.Append("'");
				buf.Append(key);
				buf.Append("'");
			}
			else
			{
				buf.Append(key);
			}
			buf.Append(": ");

			object val = m[key];
			if(val is string)
			{
				buf.Append("'");
				buf.Append(val);
				buf.Append("'");
			}
			else
			{
				buf.Append(val);
			}

			if(++i < count)
				buf.Append(sep);
		}
		buf.Append(" }");

		return buf;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   Generates a representation of the list using the trace
	///   string rather than ToString for all list objects.
	/// </summary>
	/// <param name="l">the list</param>
	/// <returns>a object representing the contents of the
	/// list</returns>
	//////////////////////////////////////////////////////////////

	public static object Trace(IList l)
	{
		return Trace(l, DEFAULT_SEP);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   Generates a representation of the list using the trace
	///   string rather than ToString for all list objects.  Each
	///   element in the list will be separated by the specified
	///   separator string.
	/// </summary>
	/// <param name="l">the list</param>
	/// <param name="sep">the object separator</param>
	/// <returns>a object representing the contents of the
	/// list</returns>
	//////////////////////////////////////////////////////////////

	public static object Trace(IList l, string sep)
	{
		TraceMessage msg = new TraceMessage("[");

		int i = 0;
		int count = l.Count;
		foreach(object elem in l)
		{
			msg.Append(elem);
			if(++i < count)
			{
				msg.Append(sep);
			}
		}

		msg.Append("]");
		return msg;
	}

	private static readonly string DEFAULT_SEP = ", ";
}

}
