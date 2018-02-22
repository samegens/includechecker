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
// File:	RepeatableCommandOption.java
// Created:	Wed May 19 08:09:38 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;

namespace TownleyEnterprises.Command {

/// <summary>
///   This class provides the basic support for repeatable command line
///   arguments.  This class can be used to allow a given argument to be
///   supplied more than once on the command line.
/// </summary>
///
/// <version>$Id: RepeatableCommandOption.cs,v 1.2 2004/06/15 16:57:44 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>

public class RepeatableCommandOption: CommandOption
{
	/// <summary>
	///   The constructor takes almost all of the parent class's
	///   arguments, but the assumption is that if it was a regular
	///   command option, it wouldn't be necessary to recognize it
	///   more than once.  Therefore, this class assumes that there
	///   will be an argument each time it is matched by the command
	///   parser.
	/// </summary>
	///
	/// <param name="longName">the long name to be checked</param>
	/// <param name="shortName">the short, single character
	/// name</param>
	/// <param name="argHelp">the help string for the
	/// argument</param>
	/// <param name="argDesc">the long description of what the
	/// argument does</param>

	public RepeatableCommandOption(string longName, char shortName,
				string argHelp, string argDesc)
		: base(longName, shortName, true, argHelp, argDesc, true, null)
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

	public RepeatableCommandOption(string longName, char shortName,
				string argHelp, string argDesc,
				bool show, Object def)
		: base(longName, shortName, true, argHelp, argDesc, show, def)
	{
	}

	/// <summary>
	///   This method calls the parent class's version to ensure
	///   that the option behaves consistently with the rest of
	///   the system, but it collects each of the arguments so
	///   that they may be retrieved via the {@link getArgs}
	///   method.
	/// </summary>
	///
	/// <param name="arg">the argument (if expected)</param>

	public override void OptionMatched(string arg)
	{
		base.OptionMatched(arg);
		AddArg(arg);
	}

	/// <summary>
	///   This method returns the arguments which have been
	///   matched by this instance.
	/// </summary>
	///
	/// <returns>a copy of the matched arguments</returns>

	public virtual IList<string> GetArgs()
	{
		return _list.AsReadOnly();
	}

	/// <summary>
	///   This method empties the list of all matched arguments in
	///   addition to any operations performed by the parent
	///   class.
	/// </summary>

	public override void Reset()
	{
		base.Reset();
		_list.Clear();
	}

	/// <summary>
	///   This method is provided so that any derived classes can
	///   append arguments to the list.
	/// </summary>
	///
	/// <param name="arg">the argument to be added</param>

	protected virtual void AddArg(string arg)
	{
		_list.Add(arg);
	}

	private List<string>	_list = new List<string>();
}

}
