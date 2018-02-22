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
// File:	Paths.cs
// Created:	Sat Jun 19 12:59:40 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace TownleyEnterprises.Common {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class provides methods equivalent to the UNIX commands
///   <c>dirname</c> and <c>basename</c> using arbitrary path
///   separators.  This class provides similar, but more flexible
///   operations to those found in the System.IO.Path class.
/// </summary>
/// <version>$Id: Paths.cs,v 1.3 2004/06/23 14:41:21 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public sealed class Paths
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to strip the path name and optional
	///   suffix from a given delimited string representing some
	///   kind of path.  It is useful for dealing with package
	///   names as well as regular filesystem paths.
	/// </summary>
	/// <param name="path">the path string</param>
	/// <param name="pd">the path delimiter</param>
	/// <param name="suffix">the suffix to remove</param>
	/// <returns>the base path</returns>
	//////////////////////////////////////////////////////////////

	public static string Basename(string path, string pd, 
					string suffix)
	{
		if(path == null || pd == null)
		{
			return null;
		}

		int idx = path.LastIndexOf(pd);
		if(idx < 0)
		{
			return path;
		}

		string pc = path.Substring(idx+1);

		if(suffix != null)
		{
			// strip the suffix
			if(suffix.Length < pc.Length)
				pc = pc.Substring(0, 
					pc.Length - suffix.Length);
		}

		return pc;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This version of the basename command exposes just the
	///   path and the delimiter.  Suffix stripping is not
	///   supported.
	/// </summary>
	/// <param name="path">the path string</param>
	/// <param name="pd">the path delimiter</param>
	/// <returns>the base path</returns>
	//////////////////////////////////////////////////////////////

	public static string Basename(string path, string pd)
	{
		return Basename(path, pd, null);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to remove the namespace name from a
	///   fully-qualified class name.
	/// </summary>
	/// <param name="name">the full class name</param>
	/// <returns>the class name</returns>
	//////////////////////////////////////////////////////////////

	public static string Classname(string name)
	{
		return Basename(name, ".", null);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method returns the path portion of the delimited
	///   string similar to the UNIX dirname command, but for
	///   arbitrary delimiters.
	/// </summary>
	/// <param name="path">the path string</param>
	/// <param name="pd">the path delimiter</param>
	/// <returns>the path portion of the name</returns>
	//////////////////////////////////////////////////////////////

	public static string Pathname(string path, string pd)
	{
		if(path == null || pd == null)
		{
			return null;
		}

		int idx = path.LastIndexOf(pd);
		if(idx < 0)
			return "";

		return path.Substring(0, idx);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method provides the exact functionality of the UNIX
	///   dirname command.  All of the non-directory suffix is
	///   stripped from the path, delmited by the <c>/</c>
	///   character.
	/// </summary>
	/// <param name="path">the path string</param>
	/// <returns>the directory name or <c>.</c> if the path does
	/// not have a directory</returns>
	//////////////////////////////////////////////////////////////

	public static string Dirname(string path)
	{
		string s = Pathname(path, "/");
		if(s.Length == 0)
		{
			return ".";
		}

		return s;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method provides the suffix of a path without the
	///   given prefix.  This is useful for trying to strip known
	///   portions of a path to find the relative parts.
	/// </summary>
	/// <param name="path">the path string</param>
	/// <param name="prefix">the path prefix</param>
	/// <returns>the path without the given prefix</returns>
	//////////////////////////////////////////////////////////////

	public static string Suffix(string path, string prefix)
	{
		if(prefix != null && path.StartsWith(prefix))
			return path.Substring(prefix.Length);

		return path;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to create an appropriate backup file
	///   name with the specified suffix.
	/// </summary>
	/// <param name="name">the file name</param>
	/// <param name="suffix">the suffix to use.  If the file with
	/// the suffix already exists, the value of the
	/// <c>overwrite</c> parameter controls the behavior.</param>
	/// <param name="overwrite">true to overwrite the existing
	/// backup files; false to append a number until a unique
	/// backup file name has been found.</param>
	/// <returns>the backup file name</returns>
	//////////////////////////////////////////////////////////////

	public static string GetBackupFileName(string name,
				string suffix, bool overwrite)
	{
		string	backup = name + suffix;
		string 	s = backup;
		int	i = 0;
	
		if(!overwrite)
		{
			while(File.Exists(s))
			{
				// figure out a name that doesn't exist
				s = backup + (++i);
			}
		}

		return s;
	}

	// prevent instantiation
	private Paths() {}
}

}
