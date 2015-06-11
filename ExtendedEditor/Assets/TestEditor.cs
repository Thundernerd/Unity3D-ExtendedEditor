using TNRD.Editor;
using UnityEditor;

public class TestEditor : ExtendedEditor {

	[MenuItem("TNRD/Test")]
	static void A() {
		GetWindow<TestEditor>();
	}

	protected override void OnInitialize() {
		base.OnInitialize();

		RepaintOnUpdate = true;
		AddWindow( new TestWindow() );
	}
}
