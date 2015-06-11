if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace TNRD.Editor {
	public class ListControl : ExtendedControl {

		public class ListEventArgs : EventArgs {

			public readonly int Index;
			public readonly string Item;

			public ListEventArgs() : base() { }

			public ListEventArgs( int index, string item ) : this() {
				Index = index;
				Item = item;
			}
		}

		public event EventHandler<ListEventArgs> OnSelectedItemChanged;
		public event EventHandler<ListEventArgs> OnItemDoubleClick;

		new public Rect Rectangle {
			get {
				return new Rect( Position.x, Position.y, Size.x, Size.y );
			}
		}

		private string[] items;

		private int index = -1;

		private bool scrollable = true;
		private bool searchable = false;
		private string searchText = "";

		private Vector2 doubleClickPosition;
		private Vector2 scrollPosition;

		private Color highlightColor = new Color( 0.243f, 0.372f, 0.588f );
		private Color alternateColor = new Color( 0.267f, 0.267f, 0.267f, 0.75f );

		private ListControl() { }
		public ListControl( Vector2 position, Vector2 size, string[] items, bool scrollable = true, bool searchable = false ) {
			Position = position;
			Size = size;
			this.items = items;
			this.scrollable = scrollable;
			this.searchable = searchable;
		}

		public ListControl( string[] items ) : this( items, true ) { }
		public ListControl( string[] items, bool scrollable ) {
			this.items = items;
			this.scrollable = scrollable;
		}

		public override void OnInitialize() {
			base.OnInitialize();
		}

		public override void OnGUI() {
			if ( items.Length == 0 ) return;
			base.OnGUI();

			var itemsToProcess = new List<string>( items );
			var listRect = Rectangle;
			var viewRect = Rectangle;
			var boxRect = new Rect( Rectangle.x - 1, Rectangle.y - 1, Rectangle.width + 2, Rectangle.height + 2 );
			var lineHeight = GUI.skin.label.CalcSize( new GUIContent( items[0] ) ).y;
			var mouseDown = Input.ButtonPressed( EMouseButton.Left );
			var mousePos = Input.MousePosition;
			var controlID = GetControlID( FocusType.Passive );

			GUI.Box( boxRect, "", EditorStyles.helpBox );

			if ( searchable ) {
				searchText = ExtendedGUI.ToolbarSearchFieldWithBackground( listRect, searchText ).ToLower();

				for ( int i = itemsToProcess.Count - 1; i >= 0; i-- ) {
					if ( !itemsToProcess[i].ToLower().Contains( searchText ) ) {
						itemsToProcess.RemoveAt( i );
					}
				}

				listRect.y += 17.5f;
				listRect.height -= 17.5f;
			}

			if ( scrollable ) {
				var h = lineHeight * itemsToProcess.Count;
				if ( h > viewRect.height ) {
					viewRect.height = h;
					viewRect.width -= 15f;
				}

				if ( searchable ) {
					viewRect.y += 17.5f;
					viewRect.height -= 17.5f;
				}
			}

			if ( mouseDown ) {
				index = -1;

				if ( new Rect( listRect.x, listRect.y, viewRect.width, listRect.height ).Contains( mousePos ) ) {
					GUIUtility.hotControl = controlID;
					GUIUtility.keyboardControl = 0;
				}
			}

			scrollPosition = GUI.BeginScrollView( listRect, scrollPosition, viewRect, false, false );

			for ( int i = 0; i < itemsToProcess.Count; i++ ) {
				var r = new Rect( viewRect.x, viewRect.y + ( i * lineHeight ), viewRect.width, lineHeight );

				if ( i % 2 == 0 ) {
					EditorGUI.DrawRect( r, alternateColor );
				}

				if ( mouseDown ) {
					if ( listRect.Contains( mousePos ) ) {
						if ( r.Contains( mousePos + scrollPosition ) ) {
							index = i;

							if ( OnSelectedItemChanged != null ) {
								OnSelectedItemChanged.Invoke( this, new ListEventArgs( i, itemsToProcess[i] ) );
							}

							GUIUtility.keyboardControl = 0;
						}
					}
				} else {
					GUI.Label( r, itemsToProcess[i] );
				}

				if ( index == i ) {
					EditorGUI.DrawRect( r, highlightColor );
					GUI.Label( r, itemsToProcess[i] );
				} else {
					GUI.Label( r, itemsToProcess[i] );
				}

				if ( Input.IsDoubleClick && Input.Button == EMouseButton.Left ) {
					if ( listRect.Contains( mousePos ) ) {
						if ( r.Contains( mousePos + scrollPosition ) ) {
							if ( OnItemDoubleClick != null ) {
								OnItemDoubleClick.Invoke( this, new ListEventArgs( i, itemsToProcess[i] ) );
							}
						}
					}
				}
			}

			GUI.EndScrollView();

			if ( Input.ButtonUp( EMouseButton.Left ) && GUIUtility.hotControl == controlID ) {
				GUIUtility.hotControl = 0;
			}
		}

		public void UpdateItems( string[] items ) {
			this.items = items;
		}
	}
}
#endif