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
// File:	IConfigResolver.cs
// Created:	Tue Jun 22 07:52:28 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Text;

namespace TownleyEnterprises.Config {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This interface provides support for suppling new configuration
///   resolver strategies.
/// </summary>
/// <version>$Id: IConfigResolver.cs,v 1.1 2004/06/22 11:59:54 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public interface IConfigResolver: IConfigSupplier
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   Property which allows manipulating the parent resolver.
	/// </summary>
	/// <param name="parent">the parent resovler</param>
	//////////////////////////////////////////////////////////////

	IConfigResolver Parent
	{
		get;
		set;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This factory method returns the appropriate resolver to
	///   the consumer.  The definition of "appropriate" is left
	///   to the implementation of the config resolver.
	/// </summary>
	/// <param name="key">the key to be tested</param>
	/// <param name="supplier">the potential new supplier</param>
	/// <param name="current">the current supplier</param>
	//////////////////////////////////////////////////////////////

	IConfigResolver GetResolver(string key,
				IConfigSupplier supplier,
				IConfigResolver current);
}

}
