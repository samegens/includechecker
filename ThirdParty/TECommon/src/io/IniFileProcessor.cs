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
// File:	IniFileProcessor.java
// Created:	Tue Jun 15 17:54:26 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;

using TownleyEnterprises.Config;

namespace TownleyEnterprises.IO {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class provides an extension of the TextFileProcessor class
///   which is useful in parsing Windows Profiles (INI files).  Once
///   the file has been parsed, the sections can be retrieved for
///   further manipulation.
/// </summary>
/// <version>$Id: IniFileProcessor.cs,v 1.10 2004/06/24 10:28:29 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class IniFileProcessor: TextFileProcessor
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This private class extends the AbstractLineProcessor to
	///   handle the parsing of the INI file.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	private class IniReader: AbstractLineProcessor
	{
		public override void ProcessLine(string line)
		{
			base.ProcessLine(line);
			
			// ignore comments
			if(line.Length == 0 || line.StartsWith("#")
					|| line.StartsWith("'")
					|| line.StartsWith(";"))
			{
				return;
			}
			
			string tline = line.Trim();
			if(tline.IndexOf("[") != 0)
			{
				try
				{
					ParseValue(tline);
				}
				catch(Exception e)
				{
					if(_warnings)
					{
						Console.Error.WriteLine("warning:  unable to parse line {0}.  Ignored.", LineCount);
						Console.Error.WriteLine("cause was:");
						Console.Error.WriteLine(e);
					}
				}
			}
			else
			{
				_cs = ParseSection(tline);
				
				string key = _cs.Name.ToLower();
				if(_sections.Contains(key))
				{
					if(_warnings)
					{
						Console.Error.WriteLine("warning:  overriding previous section definition for section '" + _cs.Name + "'");
					}
					_dupSections.Add(_sections[key]);
				}

				_sections[key] = _cs;
			}
		}

		public ICollection Sections
		{
			get { return _sections.Values; }
		}

		public ICollection DuplicateSections
		{
			get { return _dupSections; }
		}

		public ConfigSection this[string key]
		{
			get { return (ConfigSection)_sections[key.ToLower()]; }
		}

		public bool Warnings
		{
			get { return _warnings; }
			set { _warnings = value; }
		}

		private ConfigSection ParseSection(string line)
		{
			ConfigSection section = null;
			int idx = line.IndexOf("[");
			int idx2 = line.LastIndexOf("]");

			if(idx != -1 && idx2 != -1 && idx2 > idx)
			{
				section = new ConfigSection(
					line.Substring(idx + 1, 
						idx2 - idx - 1), true);
			}

			return section;
		}

		// assumption:  line is already trimmed by the caller
		private void ParseValue(string line)
		{
			int idx = line.IndexOf("=");
			
			if(idx != -1)
			{
				string sval;
				string key = line.Substring(0, idx).Trim();
				
				if(idx == line.Length - 1)
				{
					sval = "";
				}
				else
				{
					// still necessary to trim for leading WS
					sval = line.Substring(idx + 1).Trim();
				
					// strip any quotes & whitespace
					if((sval[sval.Length - 1] == '\"' &&
							sval[0] == '\"') ||
							(sval[sval.Length - 1] == '\'' &&
							sval[0] == '\''))
					{
						sval = sval.Substring(1,
							sval.Length - 2);
					}
				}

				_cs[key] = sval;
			}
		}

		private Hashtable	_sections = new Hashtable();
		private ConfigSection	_cs = null;
		private ArrayList	_dupSections = new ArrayList();
		private bool		_warnings = true;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor initializes the processor, but does not
	///   actually process the file.
	/// </summary>
	/// <param name="name">the file to process</param>
	//////////////////////////////////////////////////////////////

	public IniFileProcessor(string name) : base(name)
	{
		// compatibility with the Win32 API profile functions
		string windir = Environment.GetEnvironmentVariable("WINDIR");
		if(windir != null && "" != windir && !File.Exists(name))
		{
			string s = Path.Combine(windir, name);
			if(File.Exists(s))
				FileName = s;
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is called to actually process the file.
	///   Once processed, the sections can be retrieved via the
	///   Sections property.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public void ProcessFile()
	{
		ProcessFile(_ir);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method clears the contents of the file which has
	///   been processed.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public void Reset()
	{
		_ir = new IniReader();
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property is used to retrieve the parsed section
	///   objects.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public ICollection Sections
	{
		get { return (_ir == null) ? null : _ir.Sections; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property is used to retrieve any sections which
	///   were defined more than once.  Any section in this
	///   collection exists in the file, but it's values are not
	///   used because sections later in the file override
	///   sections defined earlier.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public ICollection DuplicateSections
	{
		get { return (_ir == null) ? null
				: _ir.DuplicateSections; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property is used to retrieve the number of lines in
	///   the input file.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public int LineCount
	{
		get { return _ir.LineCount; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property provides an indexer for retrieving a
	///   specific section in the file.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public ConfigSection this[string key]
	{
		get { return (_ir == null) ? null: _ir[key]; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property is used to control if warning messages are
	///   echoed to Console.Error.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public bool ShowWarnings
	{
		get { return (_ir == null) ? true : _ir.Warnings; }
		set
		{
			if(_ir != null)
				_ir.Warnings = value;
		}
	}

	private IniReader	_ir = new IniReader();
}

}
