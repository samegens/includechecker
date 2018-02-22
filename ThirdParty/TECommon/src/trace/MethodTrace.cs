//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2001-2004, Andrew S. Townley
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
// Name:	MethodTrace.cs
// Created:	Sun Jun 27 12:38:25 IST 2004
//
///////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Text;
using System.Threading;

using TownleyEnterprises.Common;

namespace TownleyEnterprises.Trace {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class augments the trace core with a way to track the name
///   of the currently traced method.  This class maintains a
///   thread-local call stack.
/// </summary>
/// <version>$Id: MethodTrace.cs,v 1.1 2004/06/28 06:50:15 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class MethodTrace: TraceCore
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor initializes the trace core instance as
	///   well as specifies the default value at which the method
	///   information should be printed.
	/// </summary>
	/// <param name="name">the name to print to the trace log</param>
	/// <param name="maturity">the maturity multiplier</param>
	/// <param name="methodTraceLevel">the level when the method
	/// information is printed</param>
	//////////////////////////////////////////////////////////////
	
	protected MethodTrace(string name, int maturity,
				int methodTraceLevel)
		:base(name, maturity)
	{
		_mt = methodTraceLevel;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor initializes the trace core instance.
	/// </summary>
	/// <param name="name">the name to print to the trace log</param>
	/// <param name="maturity">the maturity multiplier</param>
	/// information is printed</param>
	//////////////////////////////////////////////////////////////
	
	protected MethodTrace(string name, int maturity)
		: this(name, maturity, 1)
	{
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method should be called at the beginning of each
	///   method to be traced with the method name.
	/// </summary>
	/// <param name="name">the name of the method without the
	/// '()'</param>
	//////////////////////////////////////////////////////////////
	
	public virtual void MethodStart(string name)
	{
		object[] arr = { name };
		WriteLine(_mt, TRACE_START_FMT, arr);
		_method.Push(name);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This version of MethodStart prints all of the arguments
	///   together with the parameter names.
	/// </summary>
	/// <param name="name">the name of the method without the
	/// '()'</param>
	/// <param name="pnames">the paramater names</param>
	/// <param name="prams">the paramater values</param>
	/// <exception class="System.ArgumentExeption">if the length
	/// of the parameter names does not match the length of the
	/// parameters</exception>
	//////////////////////////////////////////////////////////////

	public virtual void MethodStart(string name, string[] pnames,
			params object[] prams)
	{
		if(pnames.Length != prams.Length)
		{
			throw new ArgumentException("Argument length doesn't match parameter name length.");
		}

		_method.Push(name);
	
		// FIXME:  need to come up with a common place to do
		// this sort of thing...
		StringBuilder buf = new StringBuilder(name);
		buf.Append("(");
		for(int i = 0; i < pnames.Length; ++i)
		{
			buf.Append(pnames[i]);
			buf.Append(" = ");
			if(prams[i] is String)
			{
				buf.Append("'");
				buf.Append(prams[i]);
				buf.Append("'");
			}
			else
			{
				buf.Append(prams[i]);
			}
			if(i < pnames.Length - 1)
				buf.Append(", ");
		}
		buf.Append(")");

		WriteLine(_mt, buf.ToString());
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This version of MethodStart prints all of the arguments
	///   without the parameter names.
	/// </summary>
	/// <param name="name">the name of the method without the
	/// '()'</param>
	/// <param name="prams">the paramater values</param>
	//////////////////////////////////////////////////////////////

	public virtual void MethodStart(string name, params object[] list)
	{
		_method.Push(name);
		
		StringBuilder buf = new StringBuilder(name);
		buf.Append("(");
		for(int i = 0; i < list.Length; ++i)
		{
			if(list[i] is string)
			{
				buf.Append("'");
				buf.Append(list[i]);
				buf.Append("'");
			}
			else
			{
				buf.Append(list[i]);
			}
			if(i < list.Length - 1)
				buf.Append(", ");
		}
		buf.Append(")");

		WriteLine(_mt, buf.ToString());
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property is used to retrieve the current method
	///   scope.  If the scope is invalid, the "invalid scope"
	///   message is returned instead of the method name.
	/// </summary>
	/// <exception class="InvalidOperationException">if the method
	/// scope is corrupted</exception>
	//////////////////////////////////////////////////////////////

	private string CurrentMethod
	{
		get
		{
			string m;
			try
			{
				m = (string)_method.Peek();
			}
			catch(InvalidOperationException e)
			{
				throw new InvalidOperationException("MethodTrace eror:  no method scope (empty stack).  Probable programming error (missing throw for traced exception?)");
			}

			return m;
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method should be called to specify that the method
	///   is about to return with no value.
	/// </summary>
	/// <exception class="InvalidOperationException">if the method
	/// scope is corrupted</exception>
	//////////////////////////////////////////////////////////////

	public virtual void MethodReturn()
	{
		object[] arr = { CurrentMethod };
		WriteLine(_mt, TRACE_RETURN_FMT, arr);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method should be called to specify that the method
	///   is about to return with the indicated value.
	/// </summary>
	/// <param name="arg">the return value</param>
	/// <exception class="InvalidOperationException">if the method
	/// scope is corrupted</exception>
	//////////////////////////////////////////////////////////////

	public virtual bool MethodReturn(bool arg)
	{
		object[] arr = { CurrentMethod, arg };
		WriteLine(_mt, TRACE_RETURN_GEN_FMT, arr);

		return arg;
	}

	public virtual byte MethodReturn(byte arg)
	{
		object[] arr = { CurrentMethod, arg };
		WriteLine(_mt, TRACE_RETURN_GEN_FMT, arr);

		return arg;
	}

	public virtual char MethodReturn(char arg)
	{
		object[] arr = { CurrentMethod, arg };
		WriteLine(_mt, TRACE_RETURN_GEN_FMT, arr);

		return arg;
	}

	public virtual decimal MethodReturn(decimal arg)
	{
		object[] arr = { CurrentMethod, arg };
		WriteLine(_mt, TRACE_RETURN_GEN_FMT, arr);
		return arg;
	}

	public virtual double MethodReturn(double arg)
	{
		object[] arr = { CurrentMethod, arg };
		WriteLine(_mt, TRACE_RETURN_GEN_FMT, arr);
		return arg;
	}

	public virtual float MethodReturn(float arg)
	{
		object[] arr = { CurrentMethod, arg };
		WriteLine(_mt, TRACE_RETURN_GEN_FMT, arr);
		return arg;
	}

	public virtual int MethodReturn(int arg)
	{
		object[] arr = { CurrentMethod, arg };
		WriteLine(_mt, TRACE_RETURN_GEN_FMT, arr);
		return arg;
	}

	public virtual long MethodReturn(long arg)
	{
		object[] arr = { CurrentMethod, arg };
		WriteLine(_mt, TRACE_RETURN_GEN_FMT, arr);
		return arg;
	}

	public virtual object MethodReturn(object arg)
	{
		object[] arr = { CurrentMethod, arg == null ? "null" : arg.ToString() };
		WriteLine(_mt, TRACE_RETURN_GEN_FMT, arr);
		return arg;
	}

//	public virtual sbyte MethodReturn(sbyte arg)
//	{
//		object[] arr = { CurrentMethod, arg };
//		WriteLine(_mt, TRACE_RETURN_GEN_FMT, arr);
//		return arg;
//	}

	public virtual short MethodReturn(short arg)
	{
		object[] arr = { CurrentMethod, arg };
		WriteLine(_mt, TRACE_RETURN_GEN_FMT, arr);
		return arg;
	}

	public virtual string MethodReturn(string arg)
	{
		object[] arr = { CurrentMethod, arg == null ? "null" : arg };
		if(arg != null)
			WriteLine(_mt, TRACE_RETURN_STR_FMT, arr);
		else
			WriteLine(_mt, TRACE_RETURN_GEN_FMT, arr);

		return arg;
	}

//	public virtual uint MethodReturn(uint arg)
//	{
//		object[] arr = { CurrentMethod, arg };
//		WriteLine(_mt, TRACE_RETURN_GEN_FMT, arr);
//		return arg;
//	}
//
//	public virtual ulong MethodReturn(ulong arg)
//	{
//		object[] arr = { CurrentMethod, arg };
//		WriteLine(_mt, TRACE_RETURN_GEN_FMT, arr);
//		return arg;
//	}

	public virtual Traceable MethodReturn(Traceable arg)
	{
		object[] arr = { CurrentMethod, arg == null ? "null" : arg.TraceString };
		WriteLine(_mt, TRACE_RETURN_GEN_FMT, arr);
		return arg;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to specify that the current method
	///   is about to throw an exception.
	/// </summary>
	/// <param name="ex">the exception to be thrown</param>
	/// <param name="print">if true, print the stack trace of the
	/// exception automatically</param>
	/// <returns>the exception which will be thrown</returns>
	/// <exception class="InvalidOperationException">if the method
	/// scope is corrupted</exception>
	//////////////////////////////////////////////////////////////

	public virtual Exception MethodThrow(Exception ex, bool print)
	{
		object[] arr = { CurrentMethod, ex.GetType().FullName, ex.Message };
		WriteLine(_mt, TRACE_THROW_FMT, arr);

		if(print)
		{
			WriteLine(_mt, ex.StackTrace);
		}
		
		return ex;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to pop the current method off the
	///   stack.
	/// </summary>
	/// <returns>the method name</returns>
	/// <exception class="InvalidOperationException">if the method
	/// scope is corrupted</exception>
	//////////////////////////////////////////////////////////////

	protected string PopCurrentMethod()
	{
		string m;
		try
		{
			m = (string)_method.Pop();
		}
		catch(InvalidOperationException e)
		{
			throw new InvalidOperationException("MethodTrace eror:  no method scope (empty stack).  Probable programming error (missing throw for traced exception?)");
		}

		return m;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This class is used to keep a separate stack for each
	///   thread which accesses the instance.
	/// </summary>
	//////////////////////////////////////////////////////////////

	private class ThreadStack: ThreadLocal
	{
		public ThreadStack()
		{
			base.Set(new Stack());
		}

		public bool Empty()
		{
			Stack s = GetStack();
			lock(s)
			{
				return (s.Count == 0);
			}
		}

		public object Peek()
		{
			Stack s = GetStack();
			lock(s)
			{
				return s.Peek();
			}
		}

		public object Pop()
		{
			Stack s = GetStack();
			lock(s)
			{
				return s.Pop();
			}
		}

		public void Push(object item)
		{
			Stack s = GetStack();
			lock(s)
			{
				s.Push(item);
			}
		}

		private Stack GetStack()
		{
			Stack s = (Stack)base.Get();
			if(s == null)
				s = _defaultStack;

			return s;
		}
	}

	/** This is the message format used for method start */
	public static readonly string TRACE_START_FMT = "{0}()";

	/** This is the message format used for methods that return no values */
	public static readonly string TRACE_RETURN_FMT = "{0}() returning.";

	/**
	 * This is the message format used for methods returning object values
	 */

	public static readonly string TRACE_RETURN_GEN_FMT = "{0}() returning:  {1}";

	/**
	 * This is the message format used for methods returning
	 * string values
	 */

	public static readonly string TRACE_RETURN_STR_FMT = "{0}() returning:  \"{1}\"";

	/**
	 * This is the message format for method arguments that are
	 * strings
	 */

	public static readonly string TRACE_ARG_STR_FMT = "\t{0} = \"{1}\"";

	/**
	 * This is the message format for method other method
	 * arguments
	 */

	public static readonly string TRACE_ARG_GEN_FMT = "\t{0} = {1}";

	/** This is the message format for throwing exceptions */
	public static readonly string TRACE_THROW_FMT = "{0}() throwing exception:  {1}: {2}";
	
	/** the method call stack for the class being traced*/
	private ThreadStack		_method = new ThreadStack();

	/** the default stack if we're calling cross-thread methods */
	private static Stack		_defaultStack = new Stack();

	/** the value at which the method information should be
	 * printed (default == 1) */
	private int			_mt;
}

}
