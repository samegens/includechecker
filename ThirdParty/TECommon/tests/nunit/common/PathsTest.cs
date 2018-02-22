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
// File:	PathsTest.cs
// Created:	Sat Jun 19 13:12:38 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using NUnit.Framework;

namespace TownleyEnterprises.Common {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   Unit tests for the path class.
/// </summary>  
/// <version>$Id: PathsTest.cs,v 1.2 2004/06/23 14:51:34 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

[TestFixture]
public sealed class PathsTest
{
	[Test]
	public void testBasenameFileName()
	{
		Assert.AreEqual("file.txt",
			Paths.Basename("/some/path/to/file.txt", "/"));
	}
	
	[Test]
	public void testBasenameFileNameStripSuffix()
	{
		Assert.AreEqual("file",
			Paths.Basename("/some/path/to/file.txt",
				"/", ".txt"));
	}
	
	[Test]
	public void testDirname()
	{
		Assert.AreEqual("/some/path/to",
			Paths.Dirname("/some/path/to/file.txt"));
	}
	
	[Test]
	public void testDirnameNo()
	{
		Assert.AreEqual(".", Paths.Dirname("file.txt"));
	}
	
	[Test]
	public void testBasenameNull()
	{
		Assert.IsNull(Paths.Basename(null, "foo"));
	}
	
	[Test]
	public void testBasenameNo()
	{
		Assert.AreEqual("one", Paths.Basename("one", "/"));
	}
	
	[Test]
	public void testClassname()
	{
		Assert.AreEqual("PathsTest",
				Paths.Classname(GetType().FullName));
	}
	
	[Test]
	public void testSuffix()
	{
		Assert.AreEqual(".Common.PathsTest",
				Paths.Suffix(GetType().FullName,
				"TownleyEnterprises"));
	}
	
	[Test]
	public void testSuffixNotPresent()
	{
		Assert.AreEqual("TownleyEnterprises.Common.PathsTest",
				Paths.Suffix(GetType().FullName,
				"ZZZZ"));
	}
}
}

