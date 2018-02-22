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
// File:	DelimitedLineProcessor.cs
// Created:	Thu Jun 10 07:39:09 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;

namespace TownleyEnterprises.IO {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class provides a mechanism for easily parsing delimited
///   text files.
/// </summary>
/// <version>$Id: DelimitedLineProcessor.cs,v 1.3 2004/06/15 17:23:45 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public abstract class DelimitedLineProcessor: AbstractLineProcessor
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor initializes the procesor with the
	///   delimiter to be used.
	/// </summary>
	/// <param name="delim">the delimiter</param>
	//////////////////////////////////////////////////////////////
	
	protected DelimitedLineProcessor(string delim)
	{
		_delim = delim;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method gets called for each line of text.  It breaks
	///   the line into delimited strings automatically, so this
	///   method should not need to be overridden.
	/// </summary>
	/// <param name="line">the line of the input file</param>
	/// <exception class="Exception">if an error occurs processing
	/// the file</exception>
	//////////////////////////////////////////////////////////////
	
	public override void ProcessLine(string line)
	{
		base.ProcessLine(line);
		if(line == null || line.Length == 0)
			return;

		ArrayList list = new ArrayList();
		int idx = 0;
		int lidx = -1;

		// beak up the list
		while(idx != -1)
		{
			string tok;

			idx = line.IndexOf(_delim, lidx+1);

			// does the line start with the delimiter?
			if(idx == 0)
			{
				tok = "";
			}
			else
			{
				int sidx = lidx+1;

				if(idx != -1)
				{
					tok = line.Substring(sidx, idx - sidx);
				}
				else
				{
					tok = line.Substring(lidx+1);
				}
			}

			list.Add(tok);
			lidx = idx;
		}

		// call the abstract method
		ProcessItems(list);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method should be overridden by concrete subclasses
	///   to provide processing of the individual line items.
	/// </summary>
	/// <param name="list">the list of tokens in the input line</param>
	//////////////////////////////////////////////////////////////
	
	public abstract void ProcessItems(IList list);
	
	private readonly string _delim;
}

}
