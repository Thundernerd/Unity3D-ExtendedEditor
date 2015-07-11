#if UNITY_EDITOR

namespace TNRD.Editor.Core {
	/// <summary>
	/// Settings that apply to a single window
	/// </summary>
	public class ExtendedWindowSettings {

		/// <summary>
		/// Default is Assets/.../CurrentEditorFolder/Assets/
		/// </summary>
		public string AssetPath = "";

		public bool AllowResize = true;

		public bool AllowRepositioning = true;

		public bool DrawTitleBarButtons = true;

		public bool DrawToolbar = false;

		public bool IsBlocking = true;

		public bool IsFullscreen = true;

		public bool UseCamera = false;

		public bool UseOnSceneGUI = false;
	}
}
#endif