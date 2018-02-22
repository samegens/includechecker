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
// File:	CommandParser.cs
// Created:	Wed May 19 09:04:48 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Text;

namespace TownleyEnterprises.Command {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class provides support for parsing command-line arguments.
/// </summary>
///
/// <version>$Id: CommandParser.cs,v 1.5 2004/06/18 15:30:45 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public sealed class CommandParser: ICommandListener
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This class is used to make life easier by mapping 3
	///   things at once.  If we match an argument, we
	///   automatically have all we need to call the appropriate
	///   listener.
	/// </summary>
	//////////////////////////////////////////////////////////////

	private class OptionHolder
	{
		public OptionHolder(CommandOption o, ICommandListener l)
		{
			option = o;
			listener = l;
		}

		internal readonly CommandOption		option;
		internal readonly ICommandListener	listener;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>Specify the autohelp default handler
	/// options</summary>

	// FIXME:  needs l10n support!!
	//////////////////////////////////////////////////////////////
	
	private static CommandOption[] ahopts = {
		new CommandOption("help", '?', false, null, "show this help message"),
		new CommandOption("usage", (char)0, false, null, "show brief usage message")
	};

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The default constructor initializes the parser with the
	///   standard '-' and '--' switches for the short and long
	///   options.  To use a different switch, the alternate
	///   constructor may be used instead.
	/// </summary>
	///
	/// <param name="appName">the name of the application</param>
	//////////////////////////////////////////////////////////////

	public CommandParser(string appName)
		: this(appName, null)
	{
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This version of the constructor alows a description for
	///   the unhandled arguments to be supplied to the parser.
	///   Primarily this is intended for use by the autohelp
	///   feature.
	/// </summary>
	///
	/// <param name="appName">the name of the application</param>
	/// <param name="appHelp">the help for the additional
	/// arguments which may be supplied to the application</param>
	//////////////////////////////////////////////////////////////

	public CommandParser(string appName, string argHelp)
		: this(appName, argHelp, '-', "--", "--")
	{
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This version of the constructor allows the client to
	///   specify the switch characters to be used for the short
	///   and long options.
	/// </summary>
	///
	/// <param name="appName">the name of the application</param>
	/// <param name="appHelp">the help for the additional
	/// arguments which may be supplied to the application</param>
	/// <param name="sSwitch">the single character option
	/// switch</param>
	/// <param name="lSwitch">lSwitch the long option
	/// switch</param>
	///
	/// <exception cref="SystemException">if a single
	/// character is used for the long switch</exception>
	//////////////////////////////////////////////////////////////

	public CommandParser(string appName, string argHelp,
				char sSwitch, string lSwitch)
		: this(appName, argHelp, sSwitch, lSwitch, "--")
	{
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This version of the constructor allows the client to
	///   specify the switch characters to be used for the short
	///   and long options.  It also allows the specification of
	///   the string to mark the end of the argument list.  By
	///   default, this string is <c>--</c> which conforms
	///   to the POSIX standard.
	/// </summary>
	///
	/// <param name="appName">the name of the application</param>
	/// <param name="appHelp">the help for the additional
	/// arguments which may be supplied to the application</param>
	/// <param name="sSwitch">the single character option
	/// switch</param>
	/// <param name="lSwitch">lSwitch the long
	/// option switch</param>
	/// <param name="endOfArgsMarker">the string marking the end
	/// of the argument list (may be null).  Anything after this
	/// string is treated as a leftover argument.</param>
	///
	/// <exception cref="SystemException">if a single
	/// character is used for the long switch</exception>
	//////////////////////////////////////////////////////////////

	public CommandParser(string appName, string argHelp,
				char sSwitch, string lSwitch,
				string endOfArgsMarker)
	{
		_appname = appName;
		_arghelp = argHelp;
		_sswitch = sSwitch;
		_lswitch = lSwitch;
		_eoargs = endOfArgsMarker;

		if(_lswitch.Length == 1)
		{
			throw new SystemException("long switch must be at least 2 characters");
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method tells the parser to automatically handle
	///   command lines with the help character.  Optionally, the
	///   help or usage can be printed when no arguments are
	///   specified.  By default autohelp is enabled and zero
	///   arguments are allowed.
	/// </summary>
	///
	/// <param name="autohelp">true to use autohelp; false to
	/// disable</param>
	/// <param name="allowZeroArgs">true to allow commands to have
	/// no arguments; false to require at least one
	/// argument</param>
	//////////////////////////////////////////////////////////////

	public void EnableAutohelp(bool autohelp, bool allowZeroArgs)
	{
		_autohelp = autohelp;
		_zeroarg = allowZeroArgs;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to register a new command listener
	///   with the parser.
	/// </summary>
	///
	/// <param name="listener">the ICommandListener
	/// instance</param>
	//////////////////////////////////////////////////////////////

	public void AddCommandListener(ICommandListener listener)
	{
		// prevent adding the same listener more than once
		if(_listeners.Contains(listener))
			return;

		CommandOption[] opts = listener.Options;

		for(int i = 0; i < opts.Length; ++i)
		{
			AddOption(opts[i], listener);
		}

		_listeners.Add(listener);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to unregister a command listener with
	///   the parser.
	/// </summary>
	///
	/// <param name="listener">the ICommandListener
	/// instance</param>
	//////////////////////////////////////////////////////////////

	public void RemoveCommandListener(ICommandListener listener)
	{
		CommandOption[] opts = listener.Options;

		for(int i = 0; i < opts.Length; ++i)
		{
			RemoveOption(opts[i]);
		}

		_listeners.Remove(listener);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This is the main parsing function that should be called to
	///   trigger the parsing of the command-line arguments
	///   registered with the parser.
	/// </summary>
	///
	/// <param name="args">the command-line arguments to
	/// parse</param>
	//////////////////////////////////////////////////////////////

	public void Parse(string[] args)
	{
		if(args.Length == 0 && !_zeroarg)
		{
			Usage();
			return;
		}
		
		if(_autohelp)
		{
			AddCommandListener(this);
		}
		else
		{
			RemoveCommandListener(this);
		}

		// reset all the options (fix for multiple parse bug)
		ResetOptions();

		bool copyargs = false;
		_leftovers = new ArrayList();
		
		for(int i = 0; i < args.Length; ++i)
		{
			string	s = args[i];

			// executive decision:  if the argument is
			// empty, it's silently ignored

			if(s == null || s.Length == 0)
				continue;

			if(s.Equals(_eoargs))
			{
				copyargs = true;
				continue;
			}

			if(copyargs)
			{
				_leftovers.Add(s);
				continue;
			}

			// take care of the normal processing
			i = ProcessArg(i, args);
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method allows the client of the argument parser to
	///   retrieve any unhandled arguments in the argument list.
	///   The main use of this method is to get options such as
	///   file names from the command line.
	/// </summary>
	///
	/// <returns>an array of string objects or a zero-length
	/// array if none were present</returns>
	//////////////////////////////////////////////////////////////

	public string[] UnhandledArguments
	{
		get
		{
			if(_leftovers == null)
				return new string[0];

			return (string[])_leftovers.ToArray(typeof(string));
		}
	}

	/// OptionListener interface
	
	public void OptionMatched(CommandOption opt, string arg)
	{
		switch(opt.ShortName)
		{
			case '?':
				Help();
				System.Environment.Exit(0);
				break;
			case (char)0:
				Usage();
				System.Environment.Exit(0);
				break;
		}
	}

	public CommandOption[] Options
	{
		get { return ahopts; }
	}

	public string Description
	{
		get { return "Help options"; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method prints the automatically generated help
	///   messages for the registered options.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public void Help()
	{
		if (_preamble != null)
		{
			PrintWrappedText(_preamble, ' ', 79, 0);
			Console.WriteLine("");
		}

		Console.Write("Usage:  " + _appname + " [OPTION...]");
		if(_arghelp != null && _arghelp.Length != 0)
		{
			Console.Write(" " + _arghelp);
		}

		foreach(ICommandListener l in _listeners)
		{
			Console.WriteLine("\n" + l.Description + ":");

			PrintOptionsHelp(l.Options);
		}

		if(_postamble != null)
		{
			Console.WriteLine("");
			PrintWrappedText(_postamble, ' ', 79, 0);
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to print the usage summary
	///   information.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public void Usage()
	{
		StringBuilder buf = new StringBuilder("Usage:  ");
		buf.Append(_appname);

		foreach(ICommandListener l in _listeners)
		{
			CommandOption[] opts = l.Options;
			for(int i = 0; i < opts.Length; ++i)
			{
				char sn = opts[i].ShortName;
				string ln = opts[i].LongName;
				bool show = opts[i].ShowArgInHelp;
				string hlp = opts[i].Help;

				if(!show)
				{
					continue;
				}

				buf.Append(" [");
				if(sn != (char)0)
				{
					buf.Append(_sswitch);
					buf.Append(sn);
					if(ln != null)
						buf.Append("|");
				}
				if(ln != null)
				{
					if(opts[i] is PosixCommandOption)
						buf.Append(_sswitch);
					else
						buf.Append(_lswitch);
					buf.Append(ln);
				}

				if(opts[i].ExpectsArgument)
				{
					if((sn != (char)0 &&
						!(opts[i] is JoinedCommandOption))
						|| opts[i] is PosixCommandOption)
						buf.Append(" ");
					else if(sn == (char)0 &&
						!(opts[i] is JoinedCommandOption))
						buf.Append("=");

					if(hlp != null)
					{
						buf.Append(hlp);
					}
					else
					{
						buf.Append("<arg>");
					}
				}
				
				buf.Append("]");
			}
		}
		
		if(_arghelp != null && _arghelp.Length != 0)
		{
			// ok, this is cheating a little for when it
			// wraps based on the ] being in col 72...
			buf.Append(" ");
			buf.Append(_arghelp);
		}

		// now, we split the lines
		PrintWrappedText(buf.ToString(), ']', 72, 8);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to configure the command parser to
	///   exit with the specified return code when it encounters
	///   arguments with missing required parameters.
	/// </summary>
	///
	/// <param name="val">toggles the behavior</param>
	/// <param name="status">the exit status to pass to
	/// Exit()</param>
	//////////////////////////////////////////////////////////////

	public void SetExitOnMissingArg(bool val, int status)
	{
		_exitmissing = val;
		_exitstatus = status;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to set optional text which can be
	///   printed before and after the command option descriptions.
	/// </summary>
	///
	/// <param name="preamble">the text to be printed before the
	/// option descriptions</param>
	/// <param name="postamble">the text to be printed after the
	/// option descriptions</param>
	//////////////////////////////////////////////////////////////

	public void SetExtraHelpText(string preamble, string postamble)
	{
		_preamble = preamble;
		_postamble = postamble;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is an easy way to add a new command option to
	///   the appropriate places.
	/// </summary>
	///
	/// <param name="opt">the CommandOption to add</param>
	/// <param name="l">the ICommandListener to notify</param>
	//////////////////////////////////////////////////////////////

	private void AddOption(CommandOption opt, ICommandListener l)
	{
		OptionHolder holder = new OptionHolder(opt, l);

		// this fix is necessary because .NET seems to throw an
		// exception if the key to a hashtable find is null
		string lname = opt.LongName == null ? "" : opt.LongName;
		char c = opt.ShortName;

		// sanity check for existing options
		OptionHolder lobj = (OptionHolder)_longOpts[lname];
		OptionHolder sobj = (OptionHolder)_shortOpts[c];
		if(lobj != null || sobj != null)
		{
			string desc = null;
			Console.Error.Write("warning:  overriding option");
			Console.Error.Write(" '");
			if(lobj != null)
			{
				Console.Error.Write(lname);
				desc = lobj.listener.Description;
			}
			else if(sobj != null)
			{
				Console.Error.Write(c);
				desc = sobj.listener.Description;
			}
			Console.Error.Write("' from '");
			Console.Error.Write(desc);
			Console.Error.Write("' by '");
			Console.Error.Write(l.Description);
			Console.Error.WriteLine("'.");
		}

		// set up the maps
		_longOpts[lname] = holder;

		if(c != (char)0)
		{
			_shortOpts[c] = holder;
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method controls what happens when a missing argument
	///   for an option is encountered.
	/// </summary>
	///
	/// <param name="val">the OptionHolder</param>
	//////////////////////////////////////////////////////////////

	private void HandleMissingArg(OptionHolder val)
	{
		string hlp = val.option.Help;
		if(hlp == null || hlp.Length == 0)
		{
			hlp = "<arg>";
		}

		string name = val.option.LongName;
		if(name == null || name.Length == 0)
		{
			StringBuilder buf = new StringBuilder();
			buf.Append(_sswitch);
			buf.Append(val.option.ShortName);
			name = buf.ToString();
		}
		else
		{
			StringBuilder buf = new StringBuilder(_lswitch);
			buf.Append(name);
			name = buf.ToString();
		}

		string msg = "error:  option " + name + " requires parameter '" + hlp + "'.";
		if(_exitmissing)
		{
			Console.Error.Write(msg);
			Console.Error.WriteLine("  Exiting.");
			Usage();
			System.Environment.Exit(_exitstatus);
		}
		else
		{
			Console.Error.Write(msg);
			Console.Error.WriteLine("  Ignored.");
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is responsible for printing the options block
	///   for a given command listener.
	/// </summary>
	///
	/// <param name="opts">the command options</param>
	//////////////////////////////////////////////////////////////

	private void PrintOptionsHelp(CommandOption[] opts)
	{
		for(int i = 0; i < opts.Length; ++i)
		{
			StringBuilder buf = new StringBuilder("  ");
			char sn = opts[i].ShortName;
			string ln = opts[i].LongName;
			bool show = opts[i].ShowArgInHelp;
			Object ad = opts[i].ArgumentDefault;
			string hlp = opts[i].Help;
			string desc = opts[i].Description;
			Object val = opts[i].ArgValue;

			if(!show)
			{
				continue;
			}

			if(sn != (char)0)
			{
				buf.Append(_sswitch);
				buf.Append(sn);

				if(ln != null)
				{
					buf.Append(", ");
				}
			}
			if(ln != null)
			{
				if(opts[i] is PosixCommandOption)
					buf.Append(_sswitch);
				else
					buf.Append(_lswitch);
				buf.Append(ln);
			}

			if(opts[i].ExpectsArgument)
			{
				if(ln != null)
				{
					if(opts[i] is PosixCommandOption)
						buf.Append(" ");
					else
						buf.Append("=");
				}
				else if(!(opts[i] is JoinedCommandOption))
				{
					buf.Append(" ");
				}
				if(hlp != null)
				{
					buf.Append(hlp);
				}
				else
				{
					buf.Append("<arg>");
				}
			}

			if(buf.Length >= SWITCH_LENGTH)
			{
				buf.Append(" ");
			}

			for(int j = buf.Length; j < SWITCH_LENGTH; ++j)
			{
				buf.Append(" ");
			}

			buf.Append(desc);

			if(ad != null)
			{
				buf.Append(" (default: ");
				
				if(val is string)
					buf.Append("\"");
				else if(val is char)
					buf.Append("'");
				
				buf.Append(ad);
				
				if(val is string)
					buf.Append("\"");
				else if(val is char)
					buf.Append("'");
				
				buf.Append(")");
			}

			PrintWrappedText(buf.ToString(), ' ',
					79, SWITCH_LENGTH);
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method handles the multi-line formatting of the
	///   indicated text based on the cut character, and prefix
	///   indent.
	/// </summary>
	///
	/// <param name="text">the text to wrap</param>
	/// <param name="cchar">the character at which wrapping should
	/// take place (if necessary)</param>
	/// <param name="width">the width at which wrapping should
	/// take place</param>
	/// <param name="indent">the number of spaces to indent the
	/// text</param>
	//////////////////////////////////////////////////////////////

	private void PrintWrappedText(string text, char cchar, 
				int width, int indent)
	{
		// check if we have a newline
		int nl = text.IndexOf('\n');
		if(nl != -1)
		{
			int start = 0;
			while(nl != -1)
			{
				string sstr = text.Substring(start, nl);
				PrintWrappedText(sstr, cchar,
						width, indent);
				start = nl+1;
				int x = sstr.IndexOf('\n');
				if(x == -1)
				{
					PrintWrappedText(text.Substring(start),
						cchar, width, indent);
					return;
				}
				
				nl += x;
			}
		}

		string line = text;
		int lwidth = width;
		while(line.Length > lwidth)
		{
			string t = null;
			int cut = lwidth;
			char c = line[cut];
			if(c != cchar)
			{
				int ocut = cut;
				cut = line.LastIndexOf(cchar, cut);
				if(cut > lwidth || cut == -1)
				{
					cut = line.LastIndexOf(' ', ocut);
					if(cut == -1)
					{
						// then we can't wrap
						// correctly, so just
						// bail and chop at
						// the edge
						cut = lwidth - 1;
					}
				}
				t = line.Substring(0, cut + 1);
			}
			else if(c == cchar && Char.IsWhiteSpace(c))
			{
				// we don't want the cchar
				t = line.Substring(0, cut);
			}
			else
			{
				// we need to keep the cchar
				t = line.Substring(0, ++cut);
			}

			Console.WriteLine(t);
			line = line.Substring(cut + 1).Trim();
			for(int xx = 0; xx < indent; ++xx)
			{
				Console.Write(" ");
			}
			lwidth = width - indent;
		}
		Console.WriteLine(line);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to unregister a command option from
	///   the appropriate places.
	/// </summary>
	///
	/// <param name="opt">the CommandOption to delete</param>
	//////////////////////////////////////////////////////////////

	private void RemoveOption(CommandOption opt)
	{
		// again, required to avoid the .NET null argument
		// exception
		if(opt.LongName != null)
		{
			_longOpts.Remove(opt.LongName);
		}

		_shortOpts.Remove(opt.ShortName);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to reset the option state prior to
	///   parsing.  It is necessary to ensure that each time the
	///   parse is performed, the correct results are returned.
	/// </summary>
	//////////////////////////////////////////////////////////////

	private void ResetOptions()
	{
		foreach(OptionHolder holder in _longOpts.Values)
		{
			holder.option.Reset();
		}

		foreach(OptionHolder holder in _shortOpts.Values)
		{
			holder.option.Reset();
		}
	}

	private int ProcessArg(int argc, string[] args)
	{
		OptionHolder val = null;
		string	s = args[argc];

		if(s == null || s.Length == 0)
			return --argc;

		char c0 = s[0];
		int slen = s.Length;
		int idx = s.IndexOf("=");

		if((_sswitch == c0) && (slen > 1)
				&& !(s.StartsWith(_lswitch)))
		{
			// we have one of the following:
			//
			// 1. a switch
			// 2. a posix option
			// 3. a set of combined options
			
			if(slen == 2)
			{
				val = (OptionHolder)_shortOpts[s[1]];
			}
			else if(slen > 2)
			{
				val = (OptionHolder)_longOpts[s];

				if(val == null)
				{
					// must be combined switches
					return ExpandSwitches(s.Substring(1),
							argc, args);
				}
			}
		}
		else if(s.StartsWith(_lswitch))
		{
			// must be a long option
			string key;
			if(idx != -1)
			{
				key = s.Substring(_lswitch.Length, idx-2);
			}
			else
			{
				key = s.Substring(_lswitch.Length);
			}
			val = (OptionHolder)_longOpts[key];
		}
		else
		{
			_leftovers.Add(s);
			return argc;
		}

		// if we get here should have a value
		if(val == null)
		{
			Console.Error.WriteLine("error:  unknown option specified (" + s + ")");
			Usage();
			System.Environment.Exit(_exitstatus);
			return args.Length;
		}

		string arg = null;

		// handle the option
		if(val.option.ExpectsArgument)
		{
			// check to make sure that there's no
			// '=' sign.
			if(idx != -1)
			{
				arg = s.Substring(idx + 1);
				if(arg.Length == 0)
				{
					HandleMissingArg(val);
				}
			}
			else
			{
				if(++argc < args.Length)
				{
					arg = args[argc];
				}
				else
				{
					HandleMissingArg(val);
					return ++argc;
				}
			}
		}

		// give the option a chance to do what it
		// wants
		val.option.OptionMatched(arg);
		
		// notify the listeners
		val.listener.OptionMatched(val.option, arg);

		return argc;
	}

	private int ExpandSwitches(string sw, int argc, string[] args)
	{
		OptionHolder oh = null;
		char ch = (char)0;
		string arg = null;

		for(int i = 0; i < sw.Length; ++i)
		{
			ch = sw[i];
			oh = (OptionHolder)_shortOpts[ch];
			if(oh == null)
			{
				Console.Error.WriteLine("error:  unknown option '" + ch + "' specified.");
				System.Environment.Exit(_exitstatus);
				return args.Length;
			}

			if(oh.option is JoinedCommandOption)
			{
				if(i == 0)
				{
					arg = sw.Substring(1);
					oh.option.OptionMatched(arg);
					oh.listener.OptionMatched(oh.option, arg);
					break;
				}
				else
				{
					Console.Error.WriteLine("error:  unknown option '" + ch + "' specified.");
					System.Environment.Exit(_exitstatus);
					return args.Length;
				}
			}
			else
			{
				if(oh.option.ExpectsArgument
						&& (i == sw.Length - 1))
				{
					if(++argc < args.Length)
					{
						arg = args[argc];
					}
					else
					{
						HandleMissingArg(oh);
					}
				}
				else if(oh.option.ExpectsArgument)
				{
					Console.Error.WriteLine("error:  invalid option combination '" + sw + "'");
					System.Environment.Exit(_exitstatus);
					return args.Length;
				}
			}

			// match the option
			oh.option.OptionMatched(arg);
			oh.listener.OptionMatched(oh.option, arg);
		}		

		return argc;
	}

	/** the name of our application */
	private string		_appname;

	/** the help text for the unhandled arguments */
	private string		_arghelp;

	/** our map for the long options */
	private Hashtable	_longOpts = new Hashtable();

	/** our map of the short options */
	private Hashtable	_shortOpts = new Hashtable();

	/** our registered listeners */
	private ArrayList	_listeners = new ArrayList();

	/** indicate if we should handle autohelp */
	private bool		_autohelp = true;

	/** indicate if allow no arguments */
	private bool		_zeroarg = true;

	/** the short switch */
	private char		_sswitch;

	/** the long switch */
	private string		_lswitch;

	/** string to signal end of the argument list */
	private string		_eoargs;

	/** the unhandled arguments */
	private ArrayList	_leftovers;

	/** controls if we exit on missing arguments */
	private bool		_exitmissing;

	/** the exit code to use if exit on missing arguments */
	private int		_exitstatus;

	/** the preamble to print before the options */
	private string		_preamble = null;

	/** the postamble to print */
	private string		_postamble = null;

	/** the maximum width of the switch part */
	private const int	SWITCH_LENGTH = 35;
}

}
