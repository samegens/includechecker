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
// File:	DebugWriter.cs
// Created:	Tue Jun 22 09:36:32 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace TownleyEnterprises.Common {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   <para>
///   This class is provided solely for the implementation of the
///   te-common classes so there is no external dependencies.  To
///   include real debug tracing, please use the
///   TownleyEnterprises.Trace API instead.
///   </para>
///   <para>NOTE:  this class is intentionally not documented</para>
/// </summary>
/// <version>$Id: DebugWriter.cs,v 1.1 2004/06/22 11:58:24 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public sealed class DebugWriter
{
	public DebugWriter(object o)
	{
		_writer = Console.Error;
		_hashcode = o.GetHashCode();
		_name = o.GetType().FullName;
		string sev = _name + ".debug";
		sev = sev.Replace(".", "_");
		string s = Environment.GetEnvironmentVariable(sev);

		if(s != null && "true" == s)
			_active = true;
		else
			_active = false;
	}

	public bool IsActive
	{
		get { return _active; }
	}

	public void WriteLine(string s)
	{
		if(!_active)
			return;

		_writer.Write(_name);
		_writer.Write("[");
		_writer.Write(_hashcode);
		_writer.Write("] ");
		_writer.WriteLine(s);
	}

	private TextWriter	_writer = null;
	private int		_hashcode;
	private bool		_active = false;
	private string		_name;
}

}
