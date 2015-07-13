#if UNITY_EDITOR

namespace TNRD.Editor.Core {

	[DocsDescription("Settings that apply to ExtendedWindow")]
	public class ExtendedWindowSettings {

		[DocsDescription("Creats an instance of ExtendedWindowSettings")]
		public ExtendedWindowSettings() { }

		[DocsDescription("The path to the assets (Default is \"Assets/.../CurrentEditorFolder/Assets/\")")]
		public string AssetPath = "";

		[DocsDescription("Can the window be resized")]
		public bool AllowResize = false;

		[DocsDescription("Can the window be moved")]
		public bool AllowRepositioning = false;

		[DocsDescription("Draw the maximize and close buttons")]
		public bool DrawTitleBarButtons = false;

		[DocsDescription("Draw the toolbar")]
		public bool DrawToolbar = false;

		[DocsDescription("Blocks underlying windows from appearing")]
		public bool IsBlocking = true;

		[DocsDescription("Is fullscreen in the editor")]
		public bool IsFullscreen = true;

		[DocsDescription("Use a camera in the window")]
		public bool UseCamera = false;

		[DocsDescription("Enable OnSceneGUI for the window")]
		public bool UseOnSceneGUI = false;
	}
}
#endif