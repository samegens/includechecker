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
// File:	TextFileProcessor.java
// Created:	Wed Jun  9 20:14:11 IST 2004
//
//////////////////////////////////////////////////////////////////////

namespace TownleyEnterprises.IO {

using System;
using System.IO;
using System.Text;

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class provides a pluggable mechanism for processing line-based
///   text files with a given character encoding.  The class breaks up
///   the input file into lines of text.  The actual processing of the
///   line is provided by providing an instance of ILineProcessor
///   apropriate for the task.
/// </summary>
/// <version>$Id: TextFileProcessor.cs,v 1.5 2004/06/24 10:32:59 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class TextFileProcessor
{
	
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor initializes the input file using the
	///   default encoding.
	/// </summary>
	/// <param name="name">the file to process</param>
	//////////////////////////////////////////////////////////////

	public TextFileProcessor(string name)
		: this(name, null)
	{
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor initializes the input file using the
	///   specified character encoding.
	/// </summary>
	/// <param name="name">the name of the file to process</param>
	/// <param name="encodding">the encoding of the file to process</param>
	/// <exception class="System.NotSupportedException">if the
	/// encoding isn't supported on this platform</exception>
	//////////////////////////////////////////////////////////////

	public TextFileProcessor(string name, string encoding)
	{
		_filename = name;
		_encoding = encoding;
		if(encoding != null)
		{
			Encoding.GetEncoding(_encoding);
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to process the file.  For each line,
	///   the ILineProcessor.ProcessLine method is called.
	/// </summary>
	/// <param name="lp">the line processor</param>
	/// <exception class="System.IO.IOException">if there is an
	/// error reading the file</exception>
	//////////////////////////////////////////////////////////////

	public void ProcessFile(ILineProcessor lp)
	{
		StreamReader	reader = null;

		if(!File.Exists(_filename))
		{
			throw new FileNotFoundException(_filename);
		}

		try
		{
			if(_encoding != null)
			{
				reader = new StreamReader(_filename, 
					Encoding.GetEncoding(_encoding));
			}
			else
			{
				reader = new StreamReader(_filename);
			}

			lp.Reset();
			string line = reader.ReadLine();
			while(line != null)
			{
				lp.ProcessLine(line);
				line = reader.ReadLine();
			}
		}
		finally
		{
			try
			{
				if(reader != null)
				{
					reader.Close();
				}
			}
			catch(IOException)
			{
				// don't care
			}
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property is used by derived classes to modify the
	///   filename after object creation, if necessary.
	/// </summary>
	//////////////////////////////////////////////////////////////

	protected string FileName
	{
		get { return _filename; }
		set { _filename = value; }
	}
	
	private readonly string	_encoding;

	// can't be read-only as derivied classes might need to change
	// it post object construction
	private string	_filename;
}

}
