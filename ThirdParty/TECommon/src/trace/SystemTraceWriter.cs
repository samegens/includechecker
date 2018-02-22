//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2003-2004, Andrew S. Townley
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
// Name:	SystemTraceWriter.java
// Created:	Sun Jun 27 13:39:40 IST 2004
//
///////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace TownleyEnterprises.Trace {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class provides a TraceWriter implementation which is based
///   on Console.Error and is the default trace stream used unless
///   otherwise configured.
/// </summary>
/// <version>$Id: SystemTraceWriter.cs,v 1.1 2004/06/28 06:50:15 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class SystemTraceWriter: TraceWriter
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This class is actually a singleton, so we don't want
	///   instances.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	private SystemTraceWriter()
	{
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This is the singleton's factory method for creating or
	///   obtaining a reference.
	/// </summary>
	/// <returns>a reference to the shared trace writer
	/// instance</returns>
	//////////////////////////////////////////////////////////////
	
	public static TraceWriter GetInstance()
	{
		if(_instance == null)
		{
			_instance = new SystemTraceWriter();
		}

		return _instance;
	}

	public void Write(object message)
	{
		lock(Console.Error)
		{
			Console.Error.Write(message);
		}
	}

	public void WriteLine(Object message)
	{
		lock(Console.Error)
		{
			Console.Error.WriteLine(message);
		}
	}

	public TextWriter TextWriter
	{
		get { return Console.Error; }
	}

	public void Flush()
	{
		lock(Console.Error)
		{
			Console.Error.Flush();
		}
	}

	public void Close()
	{
	}

	/** our singleton instance */
	private static TraceWriter _instance = null;
}

}
