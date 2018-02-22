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
// File:	AbstractLineProcessor.cs
// Created:	Wed Jun  9 20:10:18 IST 2004
//
//////////////////////////////////////////////////////////////////////

namespace TownleyEnterprises.IO {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class provides a default implementation of the
///   ILineProcessor interface which keeps track of the number of
///   lines processed.
/// </summary>
/// <version>$Id: AbstractLineProcessor.cs,v 1.2 2004/06/15 17:22:08 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public abstract class AbstractLineProcessor: ILineProcessor
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method gets called for each line processed.
	/// </summary>
	/// <param name="line">the line of the input file</param>
	/// <exception class="Exception">if an error occurs processing
	/// the file</exception>
	//////////////////////////////////////////////////////////////
	
	public virtual void ProcessLine(string line)
	{
		++_lines;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method gets called to reset any internal state
	///   maintained by the instance prior to starting any input.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public virtual void Reset()
	{
		_lines = 0;
	}
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method returns the number of lines processed since
	///   the last reset.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public int LineCount
	{
		get { return _lines; }
	}

	private int _lines = 0;
}

}
