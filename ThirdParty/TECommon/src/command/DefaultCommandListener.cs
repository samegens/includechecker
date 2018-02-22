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
// File:	DefaultCommandListener.cs
// Created:	Mon Jul 19 17:35:58 GMTDT 2004
//
//////////////////////////////////////////////////////////////////////

namespace TownleyEnterprises.Command {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This abstract class is intended to easily support the
///   implementation of command-line argument handler classes
///   by providing an empty optionMatched method.
/// </summary>
/// <version>$Id: DefaultCommandListener.cs,v 1.1 2004/07/19 16:45:23 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class DefaultCommandListener: AbstractCommandListener
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property is used to retrieve the
	///   description of the command listener's options
	///   when printing the help message.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public override string Description
	{
		get { return _description; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property is used to retrieve the arguments
	///   to be handled by the listener.
	/// </summary>
	///
	/// <returns>an array of CommandOption
	/// arguments</returns>
	//////////////////////////////////////////////////////////////

	public override CommandOption[] Options
	{
		get { return _options; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor can be used to initialize the values for
	///   the listener.
	/// </summary>
	///
	/// <param name="desc">the description displayed by the
	/// listener</param>
	/// <param name="opts">the options for the listener</param>
	//////////////////////////////////////////////////////////////

	public DefaultCommandListener(string desc, CommandOption[] opts)
	{
		_description = desc;
		_options = opts;
	}

	private readonly string			_description;
	private readonly CommandOption[]	_options;
}
}
