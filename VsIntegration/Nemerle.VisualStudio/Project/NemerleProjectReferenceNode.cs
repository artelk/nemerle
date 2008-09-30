﻿using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Project;
using System.IO;

namespace Nemerle.VisualStudio.Project
{
	[CLSCompliant(false), ComVisible(true)]
	public class NemerleProjectReferenceNode : ProjectReferenceNode
	{
		#region ctors

		/// <summary>
		/// Constructor for the ReferenceNode. It is called when the project is reloaded, when the project element representing the refernce exists. 
		/// </summary>
		public NemerleProjectReferenceNode(ProjectNode root, ProjectElement element)
			: base(root, element)
		{
		}

		/// <summary>
		/// constructor for the NemerleProjectReferenceNode
		/// </summary>
		public NemerleProjectReferenceNode(ProjectNode root, string referencedProjectName, string projectPath, string projectReference)
			: base(root, referencedProjectName, projectPath, projectReference)
		{
		}

		#endregion

		/// <summary>
		/// Gets the full path to the assembly generated by this project.
		/// </summary>
		public new string ReferencedProjectOutputPath
		{
			get
			{
				// Make sure that the referenced project implements the automation object.
				if (null == this.ReferencedProjectObject)
					return null;

				// Get the configuration manager from the project.
				EnvDTE.ConfigurationManager confManager = this.ReferencedProjectObject.ConfigurationManager;
				if (null == confManager)
					return null;

				// Get the active configuration.
				EnvDTE.Configuration config = confManager.ActiveConfiguration;
				if (null == config)
					return null;

				//Debug.WriteLine("config.Properties.Count = " + config.Properties.Count);
				//foreach (EnvDTE.Property prop in config.Properties)
				//	Debug.WriteLine(prop.Name + " = " + prop.Value);

				// Get the output path for the current configuration.
				EnvDTE.Property outputPathProperty = config.Properties.Item("OutputPath");
				if (null == outputPathProperty)
					return null;
				
				string outputPath = outputPathProperty.Value.ToString();

				// Ususally the output path is relative to the project path, but it is possible
				// to set it as an absolute path. If it is not absolute, then evaluate its value
				// based on the project directory.
				if (!Path.IsPathRooted(outputPath))
				{
					string projectDir = Path.GetDirectoryName(this.Url);
					outputPath = Path.Combine(projectDir, outputPath);
				}

				// Now get the name of the assembly from the project.
				// Some project system throw if the property does not exist. We expect an ArgumentException.
				EnvDTE.Property assemblyNameProperty = null;
				try
				{
					assemblyNameProperty = this.ReferencedProjectObject.Properties.Item("OutputFileName");
				}
				catch (ArgumentException)
				{
				}

				if (null == assemblyNameProperty)
					return null;

				// build the full path adding the name of the assembly to the output path.
				outputPath = System.IO.Path.Combine(outputPath, assemblyNameProperty.Value.ToString());

				return outputPath;
			}
		}
	}
}
