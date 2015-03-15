#if UNITY_EDITOR
using UnityEngine;
using System;

public class BetterEditorEventArgs : EventArgs {
	public readonly EMouseButton button;
	public readonly Vector2 delta;
	public readonly KeyInfo info;
	public readonly KeyCode key;
	public readonly string[] paths;
	public readonly Vector2 position;

	public BetterEditorEventArgs( EMouseButton? button = null, Vector2? delta = null, KeyInfo info = null, KeyCode? key = null, string[] paths = null, Vector2? position = null )
		: base() {
		this.button = button == null ? EMouseButton.None : button.Value;
		this.delta = delta == null ? Vector2.zero : delta.Value;
		this.info = info;
		this.key = key == null ? KeyCode.None : key.Value;
		this.paths = paths;
		this.position = position == null ? Vector2.zero : position.Value;
	}
}
#endif