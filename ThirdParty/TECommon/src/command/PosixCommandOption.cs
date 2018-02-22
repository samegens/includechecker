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
// File:	PosixCommandOption.cs
// Created:	Wed May 19 08:56:12 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;

namespace TownleyEnterprises.Command {

/// <summary>
///   This class provides support for POSIX-compliant command options.
/// </summary>
///
/// <version>$Id: PosixCommandOption.cs,v 1.2 2004/06/15 16:57:44 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>

public class PosixCommandOption: CommandOption
{
	/// <summary>
	///   POSIX command options can't have both and long forms
	///   (we're not counting abbreviations at the moment).  This
	///   method creates the option using the indicated switch and
	///   values.
	/// </summary>
	///
	/// <param name="name">the name to be checked</param>
	/// <param name="hasArg">true if this option expects an
	/// argument; false if it is a switch</param>
	/// <param name="argHelp">the help string for the
	/// argument</param>
	/// <param name="argDesc">argDesc the long description of what
	/// the argument does</param>

	public PosixCommandOption(string name, bool hasArg,
				string argHelp, string argDesc)
		: base(null, (char)0, hasArg, argHelp, argDesc, true, null)
	{
		if(name.Length == 1)
		{
			_switch = name[0];
			_name = null;
		}
		else
		{
			_switch = (char)0;
			_name = name;
		}
	}

	public override string LongName
	{
		get { return _name; }
	}

	public override char ShortName
	{
		get { return _switch; }
	}

	private readonly string	_name;
	private readonly char	_switch;
}

}
