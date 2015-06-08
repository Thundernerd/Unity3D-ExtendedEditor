#if UNITY_EDITOR
namespace TNRD {
	public class ExtendedWindowSettings {

		/// <summary>
		/// Default is Assets/.../CurrentEditorFolder/Assets/
		/// </summary>
		public string AssetPath = "";

		public bool DrawToolbar = false;

		public bool IsBlocking = true;
		public bool IsFullscreen = true;

		public bool UseCamera = false;
		public bool UseOnSceneGUI = false;
	}
}
#endif