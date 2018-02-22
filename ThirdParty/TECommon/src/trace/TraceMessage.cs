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
// Name:	TraceMessage.cs
// Created:	Sat Jun 26 13:47:14 IST 2004
//
///////////////////////////////////////////////////////////////////////

using System;
using System.Text;

namespace TownleyEnterprises.Trace {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class encapsulates a message which is to be written to the
///   trace log.  It is mainly useful when you want to collect a set
///   of data and then write it all at once rather than as individual
///   lines.  The main difference between this and using a normal
///   StringBuilder is that this class is aware of the Traceable
///   interface and will use TraceString if it is available.
/// </summary>
/// <version>$Id: TraceMessage.cs,v 1.1 2004/06/28 06:50:15 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class TraceMessage 
{
	public TraceMessage() : this(null)
	{
	}

	public TraceMessage(string message)
	{
		if(message != null)
			_buf = new StringBuilder(message);
		else
			_buf = new StringBuilder();
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method allows adding traceable contents via
	///   TraceString instead of ToString.
	/// </summary>
	/// <param name="traceable">the traceable instance</param>
	/// <returns>the StringBuilder reference</returns>
	//////////////////////////////////////////////////////////////
	
	public StringBuilder Append(Traceable traceable)
	{
		return _buf.Append(traceable.TraceString);
	}

	public override string ToString()
	{
		return _buf.ToString();
	}

	// implementation of decorator methods so we can be used as
	// StringBuilder

	public StringBuilder Append(bool arg) { return _buf.Append(arg); }
	public StringBuilder Append(byte arg) { return _buf.Append(arg); }
	public StringBuilder Append(char arg) { return _buf.Append(arg); }
	public StringBuilder Append(char arg, int i) { return _buf.Append(arg, i); }
	public StringBuilder Append(char[] arg) { return _buf.Append(arg); }
	public StringBuilder Append(char[] arg, int offset, int len) { return _buf.Append(arg, offset, len); }
	public StringBuilder Append(decimal arg) { return _buf.Append(arg); }
	public StringBuilder Append(double arg) { return _buf.Append(arg); }
	public StringBuilder Append(float arg) { return _buf.Append(arg); }
	public StringBuilder Append(int arg) { return _buf.Append(arg); }
	public StringBuilder Append(long arg) { return _buf.Append(arg); }
	public StringBuilder Append(object arg) { return _buf.Append(arg); }
	public StringBuilder Append(short arg) { return _buf.Append(arg); }
	public StringBuilder Append(string arg) { return _buf.Append(arg); }
	public StringBuilder Append(string arg, int offset, int len) { return _buf.Append(arg, offset, len); }
	

	// FIXME:  decide if we want to break CLS-compliance with
	// these...
//	public StringBuilder Append(sbyte arg) { return _buf.Append(arg); }
//	public StringBuilder Append(uint arg) { return _buf.Append(arg); }
//	public StringBuilder Append(ulong arg) { return _buf.Append(arg); }
//	public StringBuilder Append(ushort arg) { return _buf.Append(arg); }

	private readonly StringBuilder _buf;
}

}
