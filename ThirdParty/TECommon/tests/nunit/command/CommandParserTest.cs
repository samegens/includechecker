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
// File:	CommandParserTest.cs
// Created:	Tue Jun  8 21:14:49 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using NUnit.Framework;

namespace TownleyEnterprises.Command {


//////////////////////////////////////////////////////////////////////
/// <summary>
///   Unit tests for the CommandParser class.  These tests mainly deal
///   with successfully parsing the options.  Manual verification of the
///   formatting of the usage and help output will still be necessary.
/// </summary>  
/// <version>$Id: CommandParserTest.cs,v 1.3 2004/06/15 20:28:49 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

[TestFixture]
public sealed class CommandParserTest: AbstractCommandListener
{
	[Test]
	public void testAllOptionsRegistered()
	{
		parser.AddCommandListener(this);
		string[] args = new string[] { "--one", "value",
					"--onlylong", "value3",
					"--three" };
		parser.Parse(args);

		Assert.IsTrue(opt1.Matched);
		Assert.IsTrue(opt2.Matched);
		Assert.IsTrue(opt3.Matched);
	}

	[Test]
	public void testOptionOneWithSpaces()
	{
		parser.AddCommandListener(this);
		string[] args = new string[] { "--one", "value" };
		parser.Parse(args);

		Assert.IsTrue(opt1.Matched);
		Assert.AreEqual("value", opt1.Arg);
		Assert.AreEqual("value", opt1.ArgValue);
	}

	[Test]
	public void testOptionOneWithEquals()
	{
		parser.AddCommandListener(this);
		string[] args = new string[] { "--one=value" };
		parser.Parse(args);

		Assert.IsTrue(opt1.Matched);
		Assert.AreEqual("value", opt1.Arg);
		Assert.AreEqual("value", opt1.ArgValue);
	}

	[Test]
	public void testOptionOneShort()
	{
		parser.AddCommandListener(this);
		string[] args = new string[] { "-1", "value" };
		parser.Parse(args);

		Assert.IsTrue(opt1.Matched);
		Assert.AreEqual("value", opt1.Arg);
		Assert.AreEqual("value", opt1.ArgValue);
	}

	[Test]
	public void testOptionNoneMatched()
	{
		parser.AddCommandListener(this);
		string[] args = new string[] { "one", "two", "three" };
		parser.Parse(args);

		Assert.IsFalse(opt1.Matched);
		Assert.IsFalse(opt2.Matched);
		Assert.IsFalse(opt3.Matched);
	}

	[Test]
	public void testOptionOneMissingArg()
	{
		parser.AddCommandListener(this);
		string[] args = new string[] { "--one" };
		parser.Parse(args);
		Assert.IsFalse(opt1.Matched);
	}

	[Test]
	public void testOnlyLongOption()
	{
		parser.AddCommandListener(this);
		string[] args = new string[] { "--onlylong", "value" };
		parser.Parse(args);
		
		Assert.IsTrue(opt2.Matched);
		Assert.AreEqual("value", opt2.Arg);
		Assert.AreEqual("value", opt2.ArgValue);
	}

	[Test]
	public void testOnlyLongOptionMissingArg()
	{
		parser.AddCommandListener(this);
		string[] args = new string[] { "--onlylong" };
		parser.Parse(args);
		
		Assert.IsFalse(opt2.Matched);
	}

//	[Test]
//	public void testConflictingOptions()
//	{
//		parser.AddCommandListener(new AbstractCommandListener() {
//			public string Description
//			{
//				get { return "test1"; }
//			}
//
//			public CommandOption[] Options
//			{
//				get { return options2; }
//			}
//		});
//		string[] args = new string[] { "--one", "two", "-t" };
//		parser.Parse(args);
//		Assert.IsFalse(opt1.Matched);
//		Assert.IsFalse(opt3.Matched);
//		Assert.IsTrue(opt4.Matched);
//		Assert.IsTrue(opt5.Matched);
//	}

	[Test]
	public void testUnregisterListener()
	{
		parser.AddCommandListener(this);
		string[] args = new string[] { "--one", "value",
					"--onlylong", "value3",
					"--three" };
		parser.Parse(args);

		Assert.IsTrue(opt1.Matched);
		Assert.IsTrue(opt2.Matched);
		Assert.IsTrue(opt3.Matched);
		
		// now unregister and try to parse
		parser.RemoveCommandListener(this);

		// manually reset our option's state
		opt1.Reset();
		opt2.Reset();
		opt3.Reset();

		// re-parse the argument list
		parser.Parse(args);

		Assert.IsFalse(opt1.Matched);
		Assert.IsFalse(opt2.Matched);
		Assert.IsFalse(opt3.Matched);
	}

	[Test]
	public void testAlternateSwitches()
	{
		CommandParser altp = new CommandParser("altp", null,
						'/', "^^");
		altp.AddCommandListener(this);
		string[] args = new string[] { "^^one", "value",
					"^^onlylong", "value3",
					"/t" };
		altp.Parse(args);

		Assert.IsTrue(opt1.Matched);
		Assert.AreEqual("value", opt1.Arg);
		Assert.AreEqual("value", opt1.ArgValue);
		Assert.IsTrue(opt2.Matched);
		Assert.AreEqual("value3", opt2.Arg);
		Assert.AreEqual("value3", opt2.ArgValue);
		Assert.IsTrue(opt3.Matched);
	}

	[Test]
	public void testSameSwitches()
	{
		try
		{
			CommandParser altp = new CommandParser("altp",
						null, '/', "/");
		}
		catch(SystemException e)
		{
			Assert.AreEqual("long switch must be at least 2 characters", e.Message);
		}
	}

	[Test]
	public void testLongLongSwitch()
	{
		CommandParser altp = new CommandParser("altp", null,
						'*', "*****");
		altp.AddCommandListener(this);
		string[] args = new string[] { "*****one", "value",
					"*****onlylong", "value3",
					"*t" };
		altp.Parse(args);

		Assert.IsTrue(opt1.Matched);
		Assert.AreEqual("value", opt1.Arg);
		Assert.AreEqual("value", opt1.ArgValue);
		Assert.IsTrue(opt2.Matched);
		Assert.AreEqual("value3", opt2.Arg);
		Assert.AreEqual("value3", opt2.ArgValue);
		Assert.IsTrue(opt3.Matched);
	}

	[Test]
	public void testOptionDefault()
	{
		parser.AddCommandListener(this);
		string[] args = new string[0];
		parser.Parse(args);

		Assert.IsFalse(opt6.Matched);
		Assert.AreEqual("yay", opt6.Arg);
		Assert.AreEqual("yay", opt6.ArgValue);
	}

	[Test]
	public void testEndOfArgsPosix()
	{
		parser.AddCommandListener(this);
		string[] args = new string[] { "--", "-1", "value", "-t", "--onlylong", "value" };
		parser.Parse(args);

		Assert.IsFalse(opt1.Matched);
		Assert.IsFalse(opt2.Matched);
		Assert.IsFalse(opt3.Matched);
		Assert.AreEqual(5, parser.UnhandledArguments.Length);
	}

	[Test]
	public void testEndOfArgsNone()
	{
		CommandParser altp = new CommandParser("altp", null,
						'-', "--", null);
		altp.AddCommandListener(this);
		
		string[] args = new string[] { "--", "-1", "value", "-t", "--onlylong", "value" };
		altp.Parse(args);
	
		// this tests causes the argument parsing to stop at the '--'
		Assert.IsFalse(opt1.Matched, "option 1 should not be matched");
		Assert.IsFalse(opt2.Matched, "option 2 should not be matched");
		Assert.IsFalse(opt3.Matched, "option 3 should not be matched");
	}

	[Test]
	public void testEndOfArgsCustom()
	{
		CommandParser altp = new CommandParser("altp", null,
						'-', "--", "***");
		altp.AddCommandListener(this);
		
		string[] args = new string[] { "***", "--", "-1", "value", "-t", "--onlylong", "value" };
		altp.Parse(args);
	
		Assert.IsFalse(opt1.Matched);
		Assert.IsFalse(opt2.Matched);
		Assert.IsFalse(opt3.Matched);
		Assert.AreEqual(6, altp.UnhandledArguments.Length);
	}

	[Test]
	public void testSingleJoinedOption()
	{
		parser.AddCommandListener(this);
		string[] args = new string[] { "-Dfoo=bar" };
		parser.Parse(args);
		
		Assert.IsTrue(joined.Matched);
		Assert.AreEqual("foo=bar", joined.Arg);
	}

//	[Test]
//	public void testHelp()
//	{
//		// NOTE:  this is a manual test
//		parser.AddCommandListener(this);
//		string[] args = new string[] { "--help" };
//		parser.Parse(args);
//	}
		
	public override string Description
	{
		get { return "CommandParserTest"; }
	}

	public override CommandOption[] Options
	{
		get { return options1; }
	}

	CommandParser parser = new CommandParser("progname");

	static CommandOption opt1 = new CommandOption("one", '1', true,
			"ARG", "Some argument value");
	static CommandOption opt2 = new CommandOption("onlylong", (char)0,
			true, "ARG", "option 2 text");
	static CommandOption opt3 = new CommandOption("three", 't', false,
			null, "some descriptive text");
	static CommandOption opt4 = new CommandOption("one", 'X', false,
			null, "descriptive text 4");
	static CommandOption opt5 = new CommandOption("ixx", 't', false,
			null, "descriptive text 5");
	static CommandOption opt6 = new CommandOption("default", (char)0,
			true, "ARG", "this option has a default value", "yay");
	static CommandOption joined = new JoinedCommandOption('D', false, "PROPERTY=VALUE", "set the system property", true);

	static CommandOption[] options1 = 
		new CommandOption[] { opt1, opt2, opt3, opt6, joined };
	static CommandOption[] options2 = 
		new CommandOption[] { opt1, opt2, opt3, opt4, opt5 };
}

}
