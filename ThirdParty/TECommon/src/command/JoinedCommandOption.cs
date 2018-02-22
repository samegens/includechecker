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
// File:	JoinedCommandOption.cs
// Created:	Wed May 19 08:49:38 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;

namespace TownleyEnterprises.Command {

/// <summary>
///   This class provides support for "joined" command options.  Some common examples of these are:
///   <ul>
///   <li>From gcc:
///   <ul>
///   <li><c><strong>-m[VALUE]</strong></c> options for
///   machine-dependent behavior</li>
///   <li><c><strong>-W[WARNING]</strong></c> options for warning
///   levels</li>
///   </ul>
///   </li>
///   <li>From java:
///   <ul>
///   <li><c><strong>-D[PROPERTY=VALUE]</strong></c> options to
///   set system properties</li>
///   <li><c><strong>-X[OPTION]</strong></c> non-standard
///   extension options</li>
///   </ul>
///   </li>
///   </ul>
///   <para>
///   Typically speaking, the value of the joined option must immediately
///   follow the option's short name or switch.  However, this class
///   supports behavior to allow the value to also be specified as the
///   next argument in the argument list.
///   </para>
/// </summary>
///
/// <version>$Id: JoinedCommandOption.cs,v 1.3 2004/06/18 15:30:45 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>

public class JoinedCommandOption: RepeatableCommandOption
{
	/// <summary>
	///   The constructor specifies only the arguments which make
	///   sense for this type of option.  These options do not
	///   support long names.
	/// </summary>
	///
	/// <param name="shortName">this is the switch for the
	/// option</param>
	/// <param name="canSplit">true if this option can be split
	/// from the value; false means that the value must be joined
	/// tothe switch</param>
	/// <param name="argHelp">the help string for the
	/// argument</param>
	/// <param name="argDesc">the long description of what the
	/// argument does</param>
	/// <param name="show">true if should be shown in
	/// autohelp</param>

	public JoinedCommandOption(char shortName, bool canSplit,
					string argHelp, string argDesc,
					bool show)
		: base(null, shortName, argHelp, argDesc, show, null)
	{
		_cansplit = canSplit;
	}

	/// <summary>
	///   This method is used to tell the parser if the option value
	///   can follow the switch or it must be part of the switch.
	/// </summary>
	///
	/// <returns>true if the argument must be joined; false
	/// otherwise</returns>

	public virtual bool ArgCanSplit
	{
		get { return _cansplit; }
	}

	/** track if the argument value can be split from the switch */
	private readonly bool	_cansplit;
}

}
