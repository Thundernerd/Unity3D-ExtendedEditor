#if UNITY_EDITOR

namespace TNRD.Editor.Core {


	/// <summary>
	/// Settings that apply to ExtendedWindow
	/// </summary>
	public class ExtendedWindowSettings {

		/// <summary>
		/// Creats an instance of ExtendedWindowSettings
		/// </summary>
		public ExtendedWindowSettings() { }

		/// <summary>
		/// The path to the assets (Default is \"Assets/.../CurrentEditorFolder/Assets/\")
		/// </summary>
		public string AssetPath = "";

		/// <summary>
		/// Can the window be resized
		/// </summary>
		public bool AllowResize = false;

		/// <summary>
		/// Can the window be moved
		/// </summary>
		public bool AllowRepositioning = false;

		/// <summary>
		/// Draw the maximize and close buttons
		/// </summary>
		public bool DrawTitleBarButtons = false;

		/// <summary>
		/// Draw the toolbar
		/// </summary>
		public bool DrawToolbar = false;

		/// <summary>
		/// Blocks underlying windows from appearing
		/// </summary>
		public bool IsBlocking = true;

		/// <summary>
		/// Is fullscreen in the editor
		/// </summary>
		public bool IsFullscreen = true;

		/// <summary>
		/// Use a camera in the window
		/// </summary>
		public bool UseCamera = false;

		/// <summary>
		/// Enable OnSceneGUI for the window
		/// </summary>
		public bool UseOnSceneGUI = false;
	}
}
#endif