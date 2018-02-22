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
// Name:	TraceCore.cs
// Created:	Sun Jun 27 13:47:23 IST 2004
//
///////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;
using System.Threading;

using TownleyEnterprises.Common;
using TownleyEnterprises.Config;

namespace TownleyEnterprises.Trace {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   <para>
///   This is the base trace functionality.  More documentation will
///   be done later.
///   </para>
///   <para>
///   The maturity level of the class to be traced allows
///   author-defined filtering of the trace messages for classes which
///   are more mature (and assumed to work).  The maturity level is a
///   multiplier of the trace level for individual trace methods.
///   <para>
/// </summary>
/// <version>$Id: TraceCore.cs,v 1.4 2004/07/19 16:48:44 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

public class TraceCore
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   The constructor restricts creation of direct instance of
	///   the class except by subclasses.  For each instance
	///   created, it is automatically added to the map of
	///   configured instances so that finer control of the trace
	///   output can be provided.
	/// </summary>
	/// <param name="name">the identifier to be printed in the
	/// trace logs</param>
	/// <param name="maturity">the relative maturity of this class
	/// (see the TraceCore documentation for a complete
	/// explaination)</param>
	//////////////////////////////////////////////////////////////

	protected TraceCore(string name, int maturity)
	{
		lock(_tracers.SyncRoot)
		{
			_tracers[name] = this;
		}
		
		_className = name;
		_cnhash = _className.GetHashCode();
		_maturity = maturity;

		LoadInstanceSettings(name, _props);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is a generic method that is used by all of
	///   the other, simpler methods.  If the current trace level
	///   value is grate or equal to the maturity level * 10 plus
	///   the threshold level, the message will be printed.
	/// </summary>
	/// <param name="threshold">the threshold at which the message
	/// should be traced</param>
	/// <param name="fmt">the message format string</param>
	/// <param name="args">the format arguments</param>
	//////////////////////////////////////////////////////////////

	public virtual void WriteLine(int threshold, string fmt, 
				params object[] args)
	{
		if(!WillTrace(threshold))
			return;

		if(_showts)
		{
			DateTime d = DateTime.Now;
			_ts.Write(d.ToString(_tsfmt) + " ");
		}

		// only print the thread name if we're supposed to...
		String threadName = Thread.CurrentThread.Name;
		if(_showthreadalways || threadName != null) 
		{
			_ts.Write(string.Format(TRACE_CLASS_THREAD_FMT,
				new Object[] { _className, 
					_cnhash, threadName }));
		}
		else
		{
			_ts.Write(string.Format(TRACE_CLASS_FMT,
				new Object[] { _className, _cnhash }));
		}

		if(args != null)
			_ts.WriteLine(string.Format(fmt, args));
		else
			_ts.WriteLine(fmt);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This is a short-hand way to call the WrieLine method if
	///   no arguments are needed.
	/// </summary>
	/// <param name="threshold">the threshold at which the message
	/// should be traced</param>
	/// <param name="fmt">the message</param>
	//////////////////////////////////////////////////////////////

	public virtual void WriteLine(int threshold, string fmt)
	{
		WriteLine(threshold, fmt, null);
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   <para>
	///   This method can be used to determine if trace messages
	///   will be printed for a certain threshold value.  This is
	///   especially useful at higher thresholds when dumping the
	///   contents of data structures.  To dump the contents of an
	///   array to the trace log, the following mechanism should
	///   be used:
	///   </para>
	///   <example>
	///   <code>
	///   if(WillTrace(10))
	///   {
	///   	for(int i = 0; i < arr.Length; ++i)
	///   	{
	///   		_trace.WriteLine(10, "arr[" + i + "] = " + arr[i]);
	///	}
	///   }
	///   </code>
	///   </example>
	/// </summary>
	/// <param name="threshold">the threshold to check</param>
	//////////////////////////////////////////////////////////////

	public virtual bool WillTrace(int threshold)
	{
		int thresh = (_maturity * 10) + threshold;

		if(_ts == null)
			return false;

		// allow the instance to override
		if((_iTraceLevel != -1 && _iTraceLevel >= thresh) ||
			(_iTraceLevel == -1 && _traceLevel >= thresh))
		{
			return true;
		}

		return false;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property accesses the current global trace level for
	///   all instances.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public static int GlobalTraceLevel
	{
		get { return _traceLevel; }
		set { _traceLevel = value; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method returns the trace level for the specified
	///   name.
	/// </summary>
	/// <param name="name">the trace name</param>
	/// <returns>the trace level</returns>
	//////////////////////////////////////////////////////////////

	public static int GetTraceLevel(string name)
	{
		TraceCore tc = null;

		lock(_tracers.SyncRoot)
		{
			tc = (TraceCore)_tracers[name];
		}

		if(tc != null)
			return tc.TraceLevel;
	
		return 0;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property accesses the current trace level for
	///   this instance.
	/// </summary>
	//////////////////////////////////////////////////////////////

	public virtual int TraceLevel
	{
		get { return _iTraceLevel; }
		set { _iTraceLevel = value; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method sets the trace level for the specified
	///   name.
	/// </summary>
	/// <param name="name">the trace name</param>
	/// <param name="level">the trace level</param>
	/// <returns>the trace level</returns>
	//////////////////////////////////////////////////////////////

	public static void SetTraceLevel(string name, int level)
	{
		TraceCore tc = null;

		lock(_tracers)
		{
			tc = (TraceCore)_tracers[name];
		}

		if(tc != null)
			tc.TraceLevel = level;
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property retrieves the TextWriter
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public static TextWriter TextWriter
	{
		get { return _ts.TextWriter; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property controls if the timestamp will be printed
	///   in the trace log.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public static bool ShowTimestamp
	{
		get { return _showts; }
		set { _showts = value; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property controls if the thread name will be printed
	///   in the trace log.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public static bool ShowThreadNameAlways
	{
		get { return _showthreadalways; }
		set { _showthreadalways = value; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This property controls the format of the timestamp.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public static string TimestampFormat
	{
		get { lock(_tsfmt) { return _tsfmt; } }
		set { lock(_tsfmt) { _tsfmt = value; } }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method returns the trace file name.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	public static string GetTraceFile()
	{
		lock(_traceFileName) { return _traceFileName; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to set the global trace file name
	///   for all objects.
	/// </summary>
	/// <param name="filename">the file name; if the name is "" or
	/// null, the Console.Error stream will be used</param>
	/// <param name="append">true to append to an existing
	/// file</param>
	//////////////////////////////////////////////////////////////
	
	public static void SetTraceFile(string filename, bool append)
	{
		lock(_ts)
		{
			if(_ts != null)
			{
				_ts.Close();
			}

			if("" == filename || filename == null)
			{
				_ts = SystemTraceWriter.GetInstance();
				return;
			}

			try
			{
				// make sure we expand any environment variables
				filename = Environment.ExpandEnvironmentVariables(filename);
				_ts = new FileTraceWriter(filename, append);
				_traceFileName = filename;
			}
			catch(Exception e)
			{
				_traceFileName = "";
				Console.Error.WriteLine(e);
				_ts = SystemTraceWriter.GetInstance();
			}
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to load all of the confiuration
	///   settings from the specified IConfigSupplier.
	/// </summary>
	/// <param name="props">the supplier</param>
	//////////////////////////////////////////////////////////////
	
	public static void LoadSettings(IConfigSupplier props)
	{
		lock(_props)
		{
			if(!_props.Equals(props))
			{
				foreach(string key in props.Keys)
				{
					_props[key] = props[key];
				}
			}
		}

		// FIXME:  need to eliminate the get bool check
		// cut-n-paste!!!
		string s = props["errortrace.tracefile"];
		if(s != null)
		{
			string	a = props["errortrace.append"];
			bool append = true;

			if(a != null)
			{
				char c = a.ToLower()[0];
				if(c != 'y' && c != 't')
					append = false;
			}
			SetTraceFile(s, append);
		}

		s = props["errortrace.showts"];
		if(s != null)
		{
			char c = s.ToLower()[0];
			if(c != 'y' && c != 't')
				ShowTimestamp = false;
		}
		s = props["errortrace.showthreadalways"];
		if(s != null)
		{
			char c = s.ToLower()[0];
			if(c != 'y' && c != 't')
				ShowThreadNameAlways = false;
		}
		s = props["errortrace.tsfmt"];
		if(s != null)
		{
			TimestampFormat = s;
		}

		s = props["errortrace.tracelevel"];
		if(s != null)
		{
			GlobalTraceLevel = int.Parse(s);
		}

		// take care of the instances

		foreach(string key in _tracers.Keys)
		{
			TraceCore tc = (TraceCore)_tracers[key];
			tc.LoadInstanceSettings(key, props);
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is used to allow the instance to load the
	///   settings once it has been created.  Each instance can
	///   have its own trace level independently controlled.
	/// </summary>
	/// <param name="props">the supplier</param>
	//////////////////////////////////////////////////////////////
	
	public void LoadInstanceSettings(string name, IConfigSupplier props)
	{
		String pname = "errortrace." + name + ".tracelevel";
		String s = props[pname];

		if(s == null)
		{
			pname = string.Concat(name, ".tracelevel");
			s = props[pname];
		}

		if(s != null)
		{
				TraceLevel = int.Parse(s);
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method is responsible for handling the bootstap
	///   configuration process for the ErrorTrace system.
	/// </summary>
	/// <param name="props">the supplier</param>
	//////////////////////////////////////////////////////////////
	
	private static void Initialize()
	{
		if(_init)
			return;

		try
		{
			IConfigSupplier ics = new PropertiesConfigSupplier("errortrace");
//Console.WriteLine(TownleyEnterprises.IO.Serializer.ConfigToPropertiesString(ics));
			ConfigRegistry.RegisterSupplier(ics);
		}
		catch(IOException)
		{
			// valid to not have the properties file
		}

		_props = ConfigRegistry.GetConfig("errortrace");
		LoadSettings(_props);

		// FIXME:  not really sure how to do this at the
		// moment...
		
//		try
//		{
//			// load in reverse order
//			InputStream is = ErrorTrace.class.getResourceAsStream("/errortrace.properties");
//			if(is != null)
//			{
//				_props.load(is);
//			}
//
//			// check for the errortrace.properties in the system
//			String s = System.getProperty("errortrace.properties");
//			if(s != null)
//			{
//				is = new FileInputStream(s);
//				_props.load(is);
//			}
//
//			// load from the command-line
//			_props.putAll(System.getProperties());
//
//			// take care of setting them all
//			loadSettings(_props);
//		}
//		catch(IOException e)
//		{
//			e.printStackTrace();
//		}
	}

	/** the default date format string */
//	private static readonly string		TRACE_DATE_FMT = "yyyy-MM-dd HH:mm:ss.SSS z";
	private static readonly string		TRACE_DATE_FMT = "yyyy-MM-dd HH:mm:ss.fff 'GMT'z";

	// FIXME:  figure out how to get the formatting I have in the
	// Java Edition...
	/** format string for the thread */
//	private static readonly string		TRACE_CLASS_THREAD_FMT = "{0}[{1,number,#}:{2}] ";
	private static readonly string		TRACE_CLASS_THREAD_FMT = "{0}[{1}:{2}] ";

	/** format string for the thread */
//	private static readonly string		TRACE_CLASS_FMT = "{0}[{1,number,#}] ";
	private static readonly string		TRACE_CLASS_FMT = "{0}[{1}] ";

	/** this is the name of the trace file */
	private static string			_traceFileName = "";

	/** the trace stream for the trace messages */
	private static TraceWriter		_ts = SystemTraceWriter.GetInstance();

	/** the trace level for all classes */
	private static int			_traceLevel;

	/** the date formatter we're going to use */
	private static string			_tsfmt = TRACE_DATE_FMT;

	/** our static properties */
	private static IConfigSupplier		_props;

	/** if we're supposed to show the date */
	private static bool			_showts = true;

	/** if we're supposed to always show the thread */
	private static bool			_showthreadalways = false;

	/** if we've been initialized */
	private static bool			_init = false;

	/** this map keeps track of all the trace instances */
	private static Hashtable		_tracers = new Hashtable();
	
	/** the maturity level for the class being traced */
	private int				_maturity;

	/** the name of the class being traced */
	private string				_className;

	/** save the hash code for the class name string */
	private readonly int			_cnhash;

	/** the instance trace level for this instance */
	private int				_iTraceLevel = -1;

	static TraceCore()
	{
		Initialize();
	}
}

}
