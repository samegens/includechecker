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
// File:	DelimitedCommandOption.cs
// Created:	Wed May 19 07:47:45 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;

namespace TownleyEnterprises.Command {

/// <summary>
///   This class provides support for multi-valued options which are
///   specified using delmited values.  Examples of options of this form
///   are:
///   <example>
/// <code>
/// -X one,two,three
/// --value=1,2,3,4
/// --eggs green
///	</code>
///   </example>
///   <para>
///   NOTE:  due to limitations in the .NET framework for parsing
///   strings, delimiters of more than one character will produce
///   unexpected results when used with this implementation.
///   </para>
/// </summary>
///
/// <version>$Id: DelimitedCommandOption.cs,v 1.3 2004/06/18 15:30:45 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>

public class DelimitedCommandOption: RepeatableCommandOption
{
	/// <summary>
	///   The constructor takes almost all of the parent class's
	///   arguments, but the assumption is that if it was a regular
	///   command option, it wouldn't be necessary to recognize it
	///   more than once.  Therefore, this class assumes that there
	///   will be an argument each time it is matched by the command
	///   parser.  The default delmiter is the comma (','). 
	/// </summary>
	///
	/// <param name="longName">the long name to be checked</param>
	/// <param name="shortName">the short, single character
	/// name</param>
	/// <param name="argHelp">the help string for the
	/// argument</param>
	/// <param name="argDesc">the long description of what the
	/// argument does</param>

	public DelimitedCommandOption(string longName, char shortName,
				string argHelp, string argDesc)
		: this(longName, shortName, argHelp, argDesc, 
				true, null, ",")
	{
	}

	/// <summary>
	///   This version of the constructor allows specifying if the
	///   argument is to be shown to the user and if the argument
	///   has a default value if it is not specified on the
	///   command line.
	/// </summary>
	/// 
	/// <param name="longName">the long name to be checked</param>
	/// <param name="shortName">the short, single character
	/// name</param>
	/// <param name="argHelp">the help string for the
	/// argument</param>
	/// <param name="argDesc">the long description of what the
	/// argument does</param>
	/// <param name="show">true if should be shown in
	/// autohelp</param>
	/// <param name="def">the default value of the argument if it
	/// is not specified.</param>
	/// <param name="delim">the delimiter to be used to seperate
	/// the option values.</param>

	public DelimitedCommandOption(string longName, char shortName,
				string argHelp, string argDesc,
				bool show, Object def, string delim)
		: base(longName, shortName, argHelp, argDesc, show, def)
	{
		_delim = delim;
	}

	/// <summary>
	///   This method will parse the argument as it's added and
	///   ensure that all of the values get added to the parent
	///   class's list.
	/// </summary>
	///
	/// <param name="arg">the argument to be added</param>

	protected override void AddArg(string arg)
	{
		string[] tokens = arg.Split(_delim.ToCharArray());
		foreach (string s in tokens)
		{
			base.AddArg(s);
		}
	}

	private readonly string	_delim;
}

}
