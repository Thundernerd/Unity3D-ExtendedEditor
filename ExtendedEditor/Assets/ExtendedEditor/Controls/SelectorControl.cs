#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TNRD {
	public class SelectorControl : ExtendedControl {

		[JsonIgnore]
		public List<SelectableControl> SelectedControls = new List<SelectableControl>();

		private Vector2 start;
		private Vector2 end;

		private bool startedDrag = false;
		private bool dragControls = false;

		public SelectorControl() { }

		new public Rect Rectangle {
			get {
				return new Rect( Position.x, Position.y, Size.x, Size.y );
			}
		}

		public override void OnGUI() {
			var id = GetControlID( FocusType.Passive );
			if ( GUIUtility.hotControl != 0 && GUIUtility.hotControl != id ) return;
			if ( Input.KeyDown( KeyCode.LeftAlt ) || Input.KeyDown( KeyCode.RightAlt ) ) return;

			if ( ( Input.KeyDown( KeyCode.LeftShift ) || Input.KeyDown( KeyCode.RightShift ) ) && Input.ButtonReleased( EMouseButton.Left ) ) {
				var controls = Window.GetControlsSlow<SelectableControl>();
				foreach ( var item in controls ) {
					if ( item.Contains( Input.MousePosition ) ) {
						item.OnSelect();
						SelectedControls.Add( item );
						return;
					}
				}
			}

			if ( Input.ButtonDown( EMouseButton.Left ) ) {
				if ( SelectedControls.Count < 2 ) {
					var controls = Window.GetControlsSlow<SelectableControl>();
					var newControls = new List<SelectableControl>();

					for ( int i = 0; i < controls.Count; i++ ) {
						if ( controls[i].Contains( Input.MousePosition ) ) {
							newControls.Add( controls[i] );
						}
					}

					if ( newControls.Count == 1 ) {
						for ( int i = 0; i < SelectedControls.Count; i++ ) {
							SelectedControls[i].OnDeselect();
						}

						SelectedControls.Clear();
						SelectedControls.Add( newControls[0] );
						newControls[0].OnSelect();
					}
				}
			}

			if ( Input.Type == EventType.MouseDrag && Input.ButtonDown( EMouseButton.Left ) ) {
				var controls = Window.GetControlsSlow<SelectableControl>();

				var delta = Input.MouseDelta;
				if ( Window.Settings.UseCamera ) {
					delta = Window.ScaleMatrix.inverse.MultiplyVector( delta );
				}

				if ( !startedDrag && SelectedControls.Count > 0 ) {
					for ( int i = 0; i < SelectedControls.Count; i++ ) {
						if ( !dragControls ) {
							if ( SelectedControls[i].Contains( Input.MousePosition ) ) {
								GUIUtility.hotControl = id;
								GUIUtility.keyboardControl = 0;
								
								dragControls = true;
								i = -1;
							}
						} else {
							SelectedControls[i].Move( delta );
						}
					}
				}

				if ( dragControls ) return;

				if ( !startedDrag ) {
					GUIUtility.hotControl = id;
					GUIUtility.keyboardControl = 0;

					start = Input.MousePosition;
					startedDrag = true;
				}

				end = Input.MousePosition;

				var minx = Mathf.Min( start.x, end.x );
				var maxx = Mathf.Max( start.x, end.x );
				var miny = Mathf.Min( start.y, end.y );
				var maxy = Mathf.Max( start.y, end.y );

				Position.Set( minx, miny );
				Size.Set( maxx - minx, maxy - miny );

				foreach ( var item in controls ) {
					if ( Rectangle.Contains( item.Center() ) ) {
						if ( !item.IsSelected ) {
							item.OnSelect();
							SelectedControls.Add( item );
						}
					} else {
						if ( item.IsSelected ) {
							item.OnDeselect();
							SelectedControls.Remove( item );
						}
					}
				}

			} else if ( Input.IsDoubleClick && Input.Button == EMouseButton.Left ) {
				foreach ( var item in SelectedControls ) {
					item.OnDeselect();
				}

				SelectedControls.Clear();
			} else if ( Input.ButtonReleased( EMouseButton.Left ) ) {
				startedDrag = false;
				dragControls = false;

				if ( GUIUtility.hotControl == id ) {
					GUIUtility.hotControl = 0;
				}
			}

			if ( startedDrag ) {
				GUI.Box( Rectangle, "" );
			}
		}
	}
}
#endif