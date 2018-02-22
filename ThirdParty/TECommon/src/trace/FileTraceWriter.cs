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
// Name:	FileTraceWriter.cs
// Created:	Sun Jun 27 12:15:05 IST 2004
//
///////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace TownleyEnterprises.Trace {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class provides an implementation of the TraceWriter
///   interface in terms of a system file.  Multiple threads accessing
///   this writer are synchronized on the underlying stream.
/// </summary>
/// <version>$Id: FileTraceWriter.cs,v 1.1 2004/06/28 06:50:15 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class FileTraceWriter: TraceWriter
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor takes the name of the file and the
	///   append mode for the output stream.
	/// </summary>
	/// <param name="filename">the name of the file</param>
	/// <param name="append">true to append to an existing file</param>
	/// <exception class="System.IO.IOException">if there are any
	/// problems creating the file</exception>
	//////////////////////////////////////////////////////////////
	
	public FileTraceWriter(String filename, bool append)
	{
		_stream = new StreamWriter(filename, append);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method writes the message to the trace output, but
	///   does not include the newline.
	/// </summary>
	/// <param name="message">the message to write</param>
	//////////////////////////////////////////////////////////////
	
	public void Write(object message)
	{
		lock(_stream)
		{
			_stream.Write(message);
			_stream.Flush();
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method writes a line to the trace output.
	/// </summary>
	/// <param name="message">the message to write</param>
	//////////////////////////////////////////////////////////////
	
	public void WriteLine(object message)
	{
		lock(_stream)
		{
			_stream.WriteLine(message);
			_stream.Flush();
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property provides access to the underlying
	///   TextWriter instance.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public TextWriter TextWriter
	{
		get { return _stream; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method flushes the trace writer.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public void Flush()
	{
		lock(_stream)
		{
			_stream.Flush();
		}
	}
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method closes the trace writer.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public void Close()
	{
		lock(_stream)
		{
			_stream.Flush();
			_stream.Close();
		}
	}

	private StreamWriter _stream = null;
}

}
