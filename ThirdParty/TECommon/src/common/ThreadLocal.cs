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
// File:	ThreadLocal.cs
// Created:	Sun Jun 27 13:28:12 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Threading;

namespace TownleyEnterprises.Common {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   <para>
///   This class provides functionality similar to the J2SE
///   ThreadLocal class so that individual classes needing this
///   functionality don't have to mess with the slots themselves.
///   </para>
///   <para>
///   This class allocates an unnamed slot in the constructor and the
///   Get and Set methods are simply wrappers to the Thread.GetData
///   and Thread.SetData methods.
///   </para>
/// </summary>
/// <version>$Id: ThreadLocal.cs,v 1.1 2004/06/28 07:05:17 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class ThreadLocal
{
	public ThreadLocal()
	{
		_slot = Thread.AllocateDataSlot();
	}

	public void Set(object data)
	{
		Thread.SetData(_slot, data);
	}

	public object Get()
	{
		return Thread.GetData(_slot);
	}

	private readonly LocalDataStoreSlot	_slot;
}

}
