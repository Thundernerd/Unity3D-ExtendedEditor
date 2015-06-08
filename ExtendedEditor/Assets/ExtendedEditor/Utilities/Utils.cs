#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;

namespace TNRD {
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
	}
}
#endif