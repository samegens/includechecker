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
// File:	PropertyFileProcessor.java
// Created:	Sun Jun 20 22:05:55 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using TownleyEnterprises.Config;

namespace TownleyEnterprises.IO {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class provides an extension of the TextFileProcessor class
///   which is useful in parsing J2SE properties files which contain
///   strings.  Once the file has been parsed, the properties can be
///   retrieved and used as desired.
/// </summary>
/// <version>$Id: PropertyFileProcessor.cs,v 1.2 2004/06/23 14:49:04 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class PropertyFileProcessor: TextFileProcessor
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This private class extends the AbstractLineProcessor to
	///   handle the parsing of the properties file.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	private class FileReader: AbstractLineProcessor
	{
		public FileReader(bool ignoreCase, string prefix)
		{
			_properties = new Properties(null,
					ignoreCase, prefix);
		}

		public override void ProcessLine(string line)
		{
			base.ProcessLine(line);
			
			// ignore comments
			if(line.Length == 0 || line.StartsWith("#"))
			{
				return;
			}
			
			ParseValue(line);
		}

		public Properties Properties
		{
			get { return _properties; }
		}

		private void ParseValue(string line)
		{
			int idx = line.IndexOf("=");
			if(idx != -1)
			{
				string sval = line.Substring(idx + 1);
				_properties[line.Substring(0, idx).Trim()] = sval.Trim();
			}
		}

		private Properties		_properties;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor initializes the processor, but does not
	///   actually process the file.
	/// </summary>
	/// <param name="name">the file to process</param>
	//////////////////////////////////////////////////////////////

	public PropertyFileProcessor(string name)
		: this(name, false, null)
	{
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor initializes the processor, but does not
	///   actually process the file.
	/// </summary>
	/// <param name="name">the file to process</param>
	/// <param name="ignoreCase">true if the properties are not
	/// case-sensitive</param>
	/// <param name="prefix">the prefix to use when building the
	/// property entries</param>
	//////////////////////////////////////////////////////////////

	public PropertyFileProcessor(string name,
				bool ignoreCase, string prefix)
		: base(name)
	{
		_ignoreCase = ignoreCase;
		_prefix = prefix;
		_ir = new FileReader(ignoreCase, prefix);
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
		_ir = new FileReader(_ignoreCase, _prefix);
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
	///   Retrieves the properties processed by this instance.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public Properties Properties
	{
		get { return _ir.Properties; }
	}

	private bool		_ignoreCase = false;
	private string		_prefix = null;
	private FileReader	_ir = null;
}

}
