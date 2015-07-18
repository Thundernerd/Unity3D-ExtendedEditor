#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Utilities {

	/// <summary>
	/// Functionality that could be considered useful
	/// </summary>
	public class Utils {
		
		/// <summary>
		/// Returns the InspectorWindow type to use as GetWindow's desiredDockNextTo parameter
		/// </summary>
        public static Type InspectorWindow {
			get {
				return Assembly.Load( "UnityEditor" ).GetType( "UnityEditor.InspectorWindow" );
			}
		}

		/// <summary>
		/// Returns the SceneView type to use as GetWindow's desiredDockNextTo parameter
		/// </summary>
        public static Type SceneView {
			get {
				return typeof(SceneView);
			}
		}

		/// <summary>
		/// Returns the GameView type to use as GetWindow's desiredDockNextTo parameter
		/// </summary>
        public static Type GameView {
			get {
				return Assembly.Load( "UnityEditor" ).GetType( "UnityEditor.GameView" );
			}
		}

		/// <summary>
		/// Returns the HierarchyWindow type to use as GetWindow's desiredDockNextTo parameter
		/// </summary>
        public static Type HierarchyWindow {
			get {
				return Assembly.Load( "UnityEditor" ).GetType( "UnityEditor.SceneHierarchyWindow" );
			}
		}

		/// <summary>
		/// Returns the ProjectBrowser type to use as GetWindow's desiredDockNextTo parameter
		/// </summary>
		public static Type ProjectBrowser {
			get {
				return Assembly.Load( "UnityEditor" ).GetType( "UnityEditor.ProjectBrowser" );
			}
		}

		/// <summary>
		/// Returns the AnimationWindow type to use as GetWindow's desiredDockNextTo parameter
		/// </summary>
		public static Type AnimationWindow {
			get {
				return Assembly.Load( "UnityEditor" ).GetType( "UnityEditor.AnimationWindow" );
			}
		}

		/// <summary>
		/// Returns the ProfilerWindow type to use as GetWindow's desiredDockNextTo parameter
		/// </summary>
		public static Type ProfilerWindow {
			get {
				return Assembly.Load( "UnityEditor" ).GetType( "UnityEditor.ProfilerWindow" );
			}
		}

		/// <summary>
		/// Returns the AudioMixerWindow type to use as GetWindow's desiredDockNextTo parameter
		/// </summary>
		public static Type AudioMixerWindow {
			get {
				return Assembly.Load( "UnityEditor" ).GetType( "UnityEditor.AudioMixerWindow" );
			}
		}

		/// <summary>
		/// Returns an array of file names that are formatted for ExtendedGUI.DropdownList
		/// </summary>
		/// <param name="directory">The directory to search for files</param>
		/// <param name="searchPattern">The search string</param>
		/// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or all subdirectories</param>
		public static string[] GetFiles( string directory, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly ) {
			directory = directory.Trim( '/', '\\' );
			directory += "/";

			var dirInfo = new DirectoryInfo( string.Format( "{0}/{1}", Application.dataPath, directory ) );
			var files = dirInfo.GetFiles( searchPattern, searchOption );
			var values = new string[files.Length];

			var dir = string.Format( "{0}/{1}", Application.dataPath, directory ).Replace( '/', '\\' );
			for ( int i = 0; i < files.Length; i++ ) {
				var value = files[i].FullName.Replace( dir, "" ).Replace( files[i].Extension, "" );
				values[i] = value.Replace( '\\', '/' );
			}

			return values;
		}

		/// <summary>
		/// Returns an array of directory names that are formatted for ExtendedGUI.DropdownList
		/// </summary>
		/// <param name="directory">The directory to search for files</param>
		/// <param name="searchPattern">The search string</param>
		/// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or all subdirectories</param>
		public static string[] GetDirectories( string directory, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly ) {
			directory = directory.Trim( '/', '\\' );
			directory += "/";

			var dirInfo = new DirectoryInfo( string.Format( "{0}/{1}", Application.dataPath, directory ) );
			var directories = dirInfo.GetDirectories( searchPattern, searchOption );
			var values = new string[directories.Length];

			var dir = string.Format( "{0}/{1}", Application.dataPath, directory ).Replace( '/', '\\' );
			for ( int i = 0; i < directories.Length; i++ ) {
				var value = directories[i].FullName.Replace( dir, "" ).Replace( ".prefab", "" );
				values[i] = value.Replace( '\\', '/' );
			}

			return values;
		}
	}
}
#endif