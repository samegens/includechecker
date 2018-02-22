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
// File:	CommandOption.cs
// Created:	Tue May 18 12:23:41 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TownleyEnterprises.Command {

/// <summary>
///   This class provides support for defining command-line
///   arguments.
/// </summary>
/// <version>$Id: CommandOption.cs,v 1.4 2004/07/19 16:51:26 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>

public class CommandOption
{
	/// <summary>
	///   The class is fully initialized by the constructor and each
	///   argument is immutable once it has been set.
	/// </summary>
	///
	/// <param name="longName">the long name to be checked</param>
	/// <param name="shortName">the short, single character
	/// name</param>
	/// <param name="hasArg">true if this option expects an
	/// argument; false if it is a switch</param>
	/// <param name="argHelp">the help string for the
	/// argument</param>
	/// <param name="argDesc">the long description of what the
	/// argument does</param>

	public CommandOption(string longName, char shortName,
				bool hasArg, string argHelp,
				string argDesc)
		: this(longName, shortName, hasArg, argHelp,
		  argDesc, true, null)
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
	/// <param name="hasArg">true if this option expects an
	/// argument; false if it is a switch</param>
	/// <param name="argHelp">the help string for the
	/// argument</param>
	/// <param name="argDesc">the long description of what the
	/// argument does</param>
	/// <param name="show">true if should be shown in
	/// autohelp</param>
	/// <param name="def">the default value for the option</param>

	public CommandOption(string longName, char shortName,
				bool hasArg, string argHelp,
				string argDesc, bool show, 
				Object def)
	{
		_longName = longName;
		_shortName = shortName;
		_help = argHelp;
		_desc = argDesc;
		_show = show;
		_hasarg = hasArg;
		_default = def;
	}

	/// <summary>
	///   This version of the constructor allows specifying if the
	///   argument has a default value.  It will always be shown
	///   in the autohelp.
	/// </summary>
	///
	/// <param name="longName">the long name to be checked</param>
	/// <param name="shortName">the short, single character
	/// name</param>
	/// <param name="hasArg">true if this option expects an
	/// argument; false if it is a switch</param>
	/// <param name="argHelp">the help string for the
	/// argument</param>
	/// <param name="argDesc">the long description of what the
	/// argument does</param>
	/// <param name="def">the default value for the option</param>

	public CommandOption(string longName, char shortName,
				bool hasArg, string argHelp,
				string argDesc, Object def)
		: this(longName, shortName, hasArg, argHelp, argDesc,
		true, def)
	{
	}

	public virtual string LongName
	{
		get { return _longName; }
	}

	public virtual char ShortName
	{
		get { return _shortName; }
	}

	public virtual bool ExpectsArgument
	{
		get { return _hasarg; }
	}

	public virtual string Help
	{
		get { return _help; }
	}

	public virtual string Description
	{
		get { return _desc; }
	}

	public virtual bool ShowArgInHelp
	{
		get { return _show; }
	}

	public virtual Object ArgumentDefault
	{
		get { return _default; }
	}

	public override int GetHashCode()
	{
		StringBuilder buf = new StringBuilder(_longName);
		buf.Append(_shortName);
		buf.Append(_help);
		buf.Append(_desc);

		return buf.ToString().GetHashCode();
	}

	/// <summary>
	///   This method is used to retrieve the argument (if any)
	///   which was given to the option.  If no argument was
	///   specified and the option has a default value, the
	///   default value will be returned instead.
	/// </summary>
	///
	/// <returns>the argument or null if no argument was
	/// specified</returns>

	public virtual string Arg
	{
		get
		{
			if(_arg == null && _default != null)
				return _default.ToString();

			return _arg;
		}
	}

	/// <summary>
	///   Indicates if this option has been matched by the command
	///   parser or not.
	/// </summary>
	///
	/// <returns>true if matched; false otherwise</returns>

	public virtual bool Matched
	{
		get { return _matched; }
	}

	/// <summary>
	/// <para>This method is called by the command parser to indicate
	/// that the option has been matched.</para>
	/// <para>This method may be overridden by derived classes to
	/// provide object-oriented command-line argument handling.
	/// The default implementation simply sets the value returned
	/// by getMatched to <c>true</c> and stores the
	/// argument.</para>
	/// </summary>
	///
	/// <param name="arg">the argument (if expected)</param>

	public virtual void OptionMatched(string arg)
	{
		_matched = true;
		_arg = arg;
	}

	/// <summary>
	///   This method is used to support multiple parses by a
	///   CommandParser instance using different sets of
	///   arguments.  Derived classes should override this method
	///   to reset any state stored using the optionMatched method. 
	/// </summary>

	public virtual void Reset()
	{
		_matched = false;
		_arg = null;
	}

	/// <summary>
	///   This property is used to allow derived classes to
	///   provide any type massaging necessary to return a usable
	///   value of the argument to the client code.
	/// </summary>
	/// 
	/// <returns>the appropriate type of argument</returns>
	
	public virtual Object ArgValue
	{
		get { return Arg; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to split compound options into their
	///   individual parts.  Compound options are given as key=val
	///   on the command line.
	/// </summary>
	/// <param name="str">the string containing the option</param>
	/// <return>a 2 element array of strings for the key and
	/// value</return>
	//////////////////////////////////////////////////////////////

	public static string[] ParseOption(string str)
	{
		string[] rez = new string[2];
		
		int cut = str.IndexOf("=");
		if(cut == -1)
			return rez;

		rez[0] = str.Substring(0, cut);
		rez[1] = str.Substring(cut + 1);

		return rez;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to break up a list of strings
	///   containing compound options and use them to initialize a
	///   hashtable.
	/// </summary>
	/// <param name="list">the list of strings</param>
	/// <return>hashtable containing the keys and values</return>
	//////////////////////////////////////////////////////////////

	public static Hashtable ParseOptions(List<string> list)
	{
		Hashtable opts = new Hashtable();
		foreach(string s in list)
		{
			string[] ray = ParseOption(s);
			opts[ray[0]] = ray[1];
		}

		return opts;
	}

	private readonly bool	_hasarg;
	private readonly bool	_show;
	private readonly char	_shortName;
	private readonly Object	_default;
	private readonly string	_desc;
	private readonly string	_help;
	private readonly string	_longName;

	private string		_arg = null;
	private bool		_matched = false;
}

}
