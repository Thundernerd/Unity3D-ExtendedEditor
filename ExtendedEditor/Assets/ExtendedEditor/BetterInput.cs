using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class BetterInput {

	private static Dictionary<KeyCode, State<bool>> keyStates = new Dictionary<KeyCode, State<bool>>();
	private static Dictionary<EMouseButton, State<bool>> mouseStates = new Dictionary<EMouseButton, State<bool>>();
	private static bool hookedToEditor = false;

	private static Vector2 mousePosition = new Vector2();
	public static Vector2 MousePosition { get { return mousePosition; } }
	private static Vector2 mouseDelta = new Vector2();
	public static Vector2 MouseDelta { get { return mouseDelta; } }

	public static void Initialize() {
		if ( !hookedToEditor ) {
			EditorApplication.update += Update;
			hookedToEditor = true;
		}
	}

	public static void Destroy() {
		if ( hookedToEditor ) {
			EditorApplication.update -= Update;
			hookedToEditor = false;
		}
	}

	public static void Update() {
		var kCopy = new Dictionary<KeyCode, State<bool>>( keyStates );
		foreach ( var item in kCopy ) {
			var value = kCopy[item.Key];
			value.Update();
			keyStates[item.Key] = value;
		}

		var mCopy = new Dictionary<EMouseButton, State<bool>>( mouseStates );
		foreach ( var item in mCopy ) {
			var value = mCopy[item.Key];
			value.Update();
			mouseStates[item.Key] = value;
		}
	}

	public static void OnGUI() {
		mouseDelta.Set( 0, 0 );

		var e = Event.current;

		if ( e != null ) {
			switch ( e.type ) {
				case EventType.KeyDown:
					if ( e.modifiers == EventModifiers.Shift ) {
						if ( !keyStates.ContainsKey( KeyCode.LeftShift ) ) {
							keyStates.Add( KeyCode.LeftShift, new State<bool>() );
						}
						if ( !keyStates.ContainsKey( KeyCode.RightShift ) ) {
							keyStates.Add( KeyCode.RightShift, new State<bool>() );
						}

						var kds = keyStates[KeyCode.LeftShift];
						kds.Current = true;
						keyStates[KeyCode.LeftShift] = kds;
						kds = keyStates[KeyCode.RightShift];
						kds.Current = true;
						keyStates[KeyCode.RightShift] = kds;
					}

					if ( !keyStates.ContainsKey( e.keyCode ) ) {
						keyStates.Add( e.keyCode, new State<bool>() );
					}
					var kd = keyStates[e.keyCode];
					kd.Current = true;
					keyStates[e.keyCode] = kd;
					break;
				case EventType.KeyUp:
					if ( e.modifiers == EventModifiers.Shift ) {
						if ( !keyStates.ContainsKey( KeyCode.LeftShift ) ) {
							keyStates.Add( KeyCode.LeftShift, new State<bool>() );
						}
						if ( !keyStates.ContainsKey( KeyCode.RightShift ) ) {
							keyStates.Add( KeyCode.RightShift, new State<bool>() );
						}

						var kus = keyStates[KeyCode.LeftShift];
						kus.Current = false;
						keyStates[KeyCode.LeftShift] = kus;
						kus = keyStates[KeyCode.RightShift];
						kus.Current = true;
						keyStates[KeyCode.RightShift] = kus;
					}

					if ( !keyStates.ContainsKey( e.keyCode ) ) {
						keyStates.Add( e.keyCode, new State<bool>() );
					}
					var ku = keyStates[e.keyCode];
					ku.Current = false;
					keyStates[e.keyCode] = ku;
					break;
				case EventType.MouseDown:
					var mbd = (EMouseButton)e.button;
					if ( !mouseStates.ContainsKey( mbd ) ) {
						mouseStates.Add( mbd, new State<bool>() );
					}
					var md = mouseStates[mbd];
					md.Current = true;
					mouseStates[mbd] = md;
					mousePosition = e.mousePosition;
					break;
				case EventType.MouseUp:
					var mdu = (EMouseButton)e.button;
					if ( !mouseStates.ContainsKey( mdu ) ) {
						mouseStates.Add( mdu, new State<bool>() );
					}
					var mu = mouseStates[mdu];
					mu.Current = false;
					mouseStates[mdu] = mu;
					break;
				case EventType.MouseMove:
				case EventType.MouseDrag:
					mousePosition = e.mousePosition;
					mouseDelta = e.delta;
					break;
				default:
					break;
			}
		}
	}

	public static bool ButtonPressed( EMouseButton button ) {
		if ( !mouseStates.ContainsKey( button ) ) return false;
		return mouseStates[button].IsPressed();
	}
	public static bool ButtonReleased( EMouseButton button ) {
		if ( !mouseStates.ContainsKey( button ) ) return false;
		return mouseStates[button].IsReleased();
	}
	public static bool ButtonDown( EMouseButton button ) {
		if ( !mouseStates.ContainsKey( button ) ) return false;
		return mouseStates[button].IsDown();
	}
	public static bool ButtonUp( EMouseButton button ) {
		if ( !mouseStates.ContainsKey( button ) ) return false;
		return mouseStates[button].IsUp();
	}

	public static bool KeyPressed( KeyCode key ) {
		if ( !keyStates.ContainsKey( key ) ) return false;
		return keyStates[key].IsPressed();
	}
	public static bool KeyReleased( KeyCode key ) {
		if ( !keyStates.ContainsKey( key ) ) return false;
		return keyStates[key].IsReleased();
	}
	public static bool KeyDown( KeyCode key ) {
		if ( !keyStates.ContainsKey( key ) ) return false;
		return keyStates[key].IsDown();
	}
	public static bool KeyUp( KeyCode key ) {
		if ( !keyStates.ContainsKey( key ) ) return false;
		return keyStates[key].IsUp(); ;
	}
}
