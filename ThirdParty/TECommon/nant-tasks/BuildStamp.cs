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
// File:	BuildStamp.cs
// Created:	Fri Jun 18 08:23:31 IST 2004
//
//////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Text;
using System.Xml;

using TownleyEnterprises.Config;
using TownleyEnterprises.IO;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using NAnt.Core.Util;

namespace TownleyEnterprises.BuildTasks.NAnt {

//////////////////////////////////////////////////////////////////////
/// <summary>
///   This class is responsible for performing build counting and
///   subsequent substitution of input files to create output files.
///   While this task may be performed using build-system magic, it is
///   highly dependent on the build system being used.  For this
///   reason, I'm finally implementing this functionality directly
///   (again).
/// </summary>
/// <remarks>
///   <para>
///   I originally wrote something along these lines to support what I
///   was doing at Informix.  That was a very simple C program which
///   parsed some files and then wrote a fixed file.  Later work with
///   GNU autoconf introduced me to the idea of <c>.in</c> files which
///   would be processed to generate the "actual" desired output.
///   </para>
///   <para>
///   For the Java Edition, I cooked up something using the built-in
///   Ant tasks, but it isn't as clean as I would like due to some of
///   the limitations with the way Ant handles properties along with
///   needing more than one file to track things.  Since I'm using
///   NAnt as my main .NET build tool and it doesn't have all of the
///   required tasks (along with wanting to fix some of the issues
///   with the Ant-centric solution), I wrote this class to address
///   these issues.
///   </para>
///   <h3>
///   How it works
///   </h3>
///   <para>
///   Essential to the use of this class is an understanding of some
///   basic concepts used to describe a project and version.  These
///   are:
///   <list type="bullet">
///   <item>
///   <term>Project Name</term>
///   <description>
///   The name of the project which will be used during token
///   substitution.  Normally, this name is not localized.
///   </description>
///   </item>
///   <item>
///   <term>Major Version</term>
///   <description>
///   This is an integer which normally represents the overall release
///   family of a software component.  Normally, these represent the
///   overall maturity of the system, e.g. a major version of 2 is
///   theoretically more mature than a version of 1.
///   </description>
///   </item>
///   </list>
///   </para>
/// </remarks>
/// <version>$Id: BuildStamp.cs,v 1.3 2004/06/21 09:42:07 atownley Exp $</version>
/// <author><a href="mailto:adz1092@netscape.net">Andrew S. Townley</a></author>
//////////////////////////////////////////////////////////////////////

[TaskName("buildstamp")]
public class BuildStamp : Task
{
	//////////////////////////////////////////////////////////////
	/// <summary>
	///   Specify the name of the file containing all of the
	///   project's version information.
	/// </summary>
	//////////////////////////////////////////////////////////////

	[TaskAttribute("versioninfo", Required=true)]

	public FileInfo VersionInfo
	{
		get { return _countfile; }
		set { _countfile = value; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   Determines if the count should be incremented.  The
	///   default is to create developer builds, so this value is
	///   false.
	/// </summary>
	//////////////////////////////////////////////////////////////

	[TaskAttribute("count")]
	[BooleanValidator()]

	public bool Count
	{
		get { return _countbuild; }
		set { _countbuild = value; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   Provides an alternative string to label developer builds
	///   other than DEVELOPER.
	/// </summary>
	//////////////////////////////////////////////////////////////

	[TaskAttribute("devname")]

	public string DevName
	{
		get { return _devname; }
		set { _devname = value; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This is a hack to work around the missing property file
	///   support.  This causes the task execution to not be
	///   performed
	/// </summary>
	//////////////////////////////////////////////////////////////

	[TaskAttribute("import")]
	[BooleanValidator()]

	public bool Import
	{
		get { return _import; }
		set { _import = value; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This reverses the stamp process so that any files which
	///   would be created during the stamp process are deleted.
	///   The default value is false.
	/// </summary>
	//////////////////////////////////////////////////////////////

	[TaskAttribute("clean")]
	[BooleanValidator()]

	public bool Clean
	{
		get { return _clean; }
		set { _clean = value; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This value does a "dry run" of the process and only
	///   echos what would happen during the task execution rather
	///   than actually performing the task.
	/// </summary>
	//////////////////////////////////////////////////////////////

	[TaskAttribute("dryrun")]
	[BooleanValidator()]

	public bool DryRun
	{
		get { return _dryrun; }
		set { _dryrun = value; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   Specify the fileset of files to stamp.
	/// </summary>
	//////////////////////////////////////////////////////////////

	[BuildElement("fileset")]

	public FileSet StampFileSet
	{
		get { return _fileset; }
		set { _fileset = value; }
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   Checks to ensure the configuration parameters make
	///   sense.
	/// </summary>
	/// <param name="taskNode">the node used to initialize the
	/// task</param>
	//////////////////////////////////////////////////////////////

	protected override void InitializeTask(XmlNode taskNode)
	{
		if(!_countfile.Exists)
		{
			throw new BuildException(string.Format("error:  unable to load version information from file '{0}'", _countfile.FullName));
		}

		// figure out what the build count should be
		PropertyFileProcessor pfp = new PropertyFileProcessor(_countfile.FullName);
		pfp.ProcessFile();
		_props = pfp.Properties;

		if(_props.Contains("build.version.project"))
		{
			_name = _props["build.version.project"];
		}
		else
		{
			_name = Project.ProjectName;
		}

		_major = Int32.Parse(_props["build.version.major"]);
		_minor = Int32.Parse(_props["build.version.minor"]);
		_release = _props["build.version.release"];
		_irelease = ParseReleaseNumber(_release);
		_count = Int32.Parse(_props["build.version.count"]);
		_datestr = _date.ToString("yyyy-MM-dd hh:mm:ss 'GMT'z");

		if(_countbuild)
		{
			++_count;
			_bldcount = _count.ToString();
			_props["build.version.count"] = _bldcount;
			
			if(!_dryrun)
			{
				StreamWriter sw = new 
					StreamWriter(_countfile.FullName);
				sw.Write(_props);
				sw.Close();
			}
		}
		else if(_import)
		{
			_bldcount = _count.ToString();
		}
		else
		{
			_bldcount = _devname;
		}

		// set the properties in the build
		Properties["build.version.project"] = _name;
		Properties["build.version.major"] = _major.ToString();
		Properties["build.version.minor"] = _minor.ToString();
		Properties["build.version.release"] = _release;
		Properties["build.version.release-prefix"] = _irelease.ToString();
		Properties["build.version.count"] = _bldcount;
		Properties["build.version.date"] = _datestr;

		Log(Level.Verbose, "{0}Major version:    {1}", LogPrefix, _major);
		Log(Level.Verbose, "{0}Minor version:    {1}", LogPrefix, _minor);
		Log(Level.Verbose, "{0}Release version:  {1}", LogPrefix, _release);
		Log(Level.Verbose, "{0}Build number:     {1}", LogPrefix, _bldcount);
		Log(Level.Verbose, "{0}Build date:       {1}", LogPrefix, _datestr);
		Log(Level.Verbose, "");
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   Actually executes the task based on the attribute
	///   values.
	/// </summary>
	//////////////////////////////////////////////////////////////

	protected override void ExecuteTask()
	{
		if(_fileset == null && _import == true)
		{
			// hack to only set the properties
			return;
		}

		// normal task processing
		if(_fileset.BaseDirectory == null)
			_fileset.BaseDirectory = new DirectoryInfo(Project.BaseDirectory);

		// take care of processing the fileset
		DirectoryInfo basedir = _fileset.BaseDirectory;
		foreach(string file in _fileset.FileNames)
		{
			StampFile(file);
		}
	}

	protected void StampFile(string file)
	{
		FileInfo src = new FileInfo(file);
		string target = Path.Combine(Path.GetDirectoryName(file),
				Path.GetFileNameWithoutExtension(file));

		if(!src.Exists)
			return;


		if(!_dryrun && !_clean)
		{
			Log(Level.Verbose, "{0}Stamping file {1}", LogPrefix, target);
			FilterProcessor fp = new FilterProcessor(this, target);
			TextFileProcessor processor = new TextFileProcessor(file);
			processor.ProcessFile(fp);
			fp._target.Close();
		}
		else if(!_dryrun && _clean)
		{
			Log(Level.Verbose, "{0}Removing file {1}", LogPrefix, target);
			File.Delete(target);
		}
	}

	//////////////////////////////////////////////////////////////
	/// <summary>
	///   This method parses out the numeric prefix of the release
	///   string and returns that to the caller.
	/// </summary>
	//////////////////////////////////////////////////////////////
	
	private int ParseReleaseNumber(string rel)
	{
		StringBuilder buf = new StringBuilder();
		for(int i = 0; i < rel.Length; ++i)
		{
			if(Char.IsDigit(rel[i]))
			{
				buf.Append(rel[i]);
			}
			else
			{
				break;
			}
		}

		return Int32.Parse(buf.ToString());
	}

	private class FilterProcessor: AbstractLineProcessor
	{
		public FilterProcessor(BuildStamp stamp, string target)
		{
			_target = new StreamWriter(target);
			_stamp = stamp;
		}

		public override void ProcessLine(string line)
		{
			string nl = line.Replace("@build.version.project@", _stamp._name);
			nl = nl.Replace("@build.version.major@", _stamp._major.ToString());
			nl = nl.Replace("@build.version.minor@", _stamp._minor.ToString());
			nl = nl.Replace("@build.version.release@", _stamp._release);
			nl = nl.Replace("@build.version.release-prefix@", _stamp._irelease.ToString());
			nl = nl.Replace("@build.version.count@", _stamp._bldcount);
			nl = nl.Replace("@build.version.date@", _stamp._datestr);

			_target.Write(nl);
			_target.Write("\n");
		}

		public BuildStamp	_stamp;
		public StreamWriter	_target;
	}

	internal string		_name;
	internal int		_major;
	internal int		_minor;
	internal int		_irelease;
	internal string		_release;
	internal FileInfo	_countfile;
	internal bool		_countbuild	= false;
	internal int		_count		= 0;
	internal string		_bldcount;
	internal string		_devname	= "DEVELOPER";
	internal bool		_clean		= false;
	internal bool		_dryrun		= false;
	internal bool		_import		= false;
	internal string		_suffix		= ".in";
	internal FileSet	_fileset;
	internal Properties	_props;
	internal DateTime	_date		= DateTime.Now;
	internal string		_datestr;
}

}
