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
// Name:	TraceFileOption.cs
// Created:	Mon Jul 19 17:03:27 GMTDT 2004
//
///////////////////////////////////////////////////////////////////////

using System;
using TownleyEnterprises.Command;

namespace TownleyEnterprises.Trace {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This is a pre-fab command option for manipulating the trace
///   file of a system.
/// </summary>
/// <remarks>
///   This option allows setting either the global trace output file
///   via the <c>--tracefile=FILE</c> syntax.  If the file already
///   exists, the new output is appended to the end of the file.
/// </remarks>
/// <version>$Id: TraceFileOption.cs,v 1.1 2004/07/19 16:44:18 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class TraceFileOption: CommandOption
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor takes care of setting all of the base
	///   class parameters.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public TraceFileOption()
		: base("tracefile", (char)0, true, 
			"FILE", "sets file name for the trace output")
	{
	}

	public override void OptionMatched(string arg)
	{
		base.OptionMatched(arg);

		// handle the option
		TraceCore.SetTraceFile(arg, true);
	}
}

}
