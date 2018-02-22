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
// Name:	TraceWriter.cs
// Created:	Sat Jun 26 09:34:31 IST 2004
//
///////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace TownleyEnterprises.Trace {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This interface is implemented by classes which can write trace
///   information. It is a subset of the TextWriter interface.
/// </summary>
/// <version>$Id: TraceWriter.cs,v 1.1 2004/06/28 06:50:15 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public interface TraceWriter
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method writes the message to the trace writer
	///   without the trailing newline character.
	/// </summary>
	/// <param name="message">the message to write</param>
	//////////////////////////////////////////////////////////////

	void Write(object message);
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method writes the message to the trace writer
	///   including the trailing newline character.
	/// </summary>
	/// <param name="message">the message to write</param>
	//////////////////////////////////////////////////////////////

	void WriteLine(object message);

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property provides access to the underlying
	///   TextWriter instance.
	/// </summary>
	//////////////////////////////////////////////////////////////

	TextWriter TextWriter
	{
		get;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method flushes the trace writer.
	/// </summary>
	//////////////////////////////////////////////////////////////

	void Flush();
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method closes the trace writer.
	/// </summary>
	//////////////////////////////////////////////////////////////

	void Close();
}

}
