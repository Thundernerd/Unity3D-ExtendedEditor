#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TNRD {
	public class ExtendedAssets {
		private Dictionary<string, Texture2D> textures;
		[JsonProperty]
		private string path;

		public ExtendedAssets() {
			textures = new Dictionary<string, Texture2D>();
		}

		public ExtendedAssets( string path, ExtendedWindow window ) {
			if ( string.IsNullOrEmpty( path ) ) {
				// Figure the path out with reflection
				var t = window.Editor.GetType();
				var files = Directory.GetFiles( Application.dataPath, string.Format( "*{0}.cs", t.Name ), SearchOption.AllDirectories );
				if ( files.Length == 1 ) {
					var f = files[0];
					var fi = new FileInfo( f );
					this.path = Path.Combine( fi.DirectoryName, "Assets/" );
				}
			} else {
				this.path = path.ToLower().StartsWith( "assets" ) ? path : string.Format( "Assets/{0}", path );
			}

			textures = new Dictionary<string, Texture2D>();
		}

		public Texture2D this[string key] {
			get {
				if ( textures.ContainsKey( key ) ) {
					return textures[key];
				} else {
					return Load( key );
				}
			}
		}

		public Texture2D Load( string key ) {
			if ( textures.ContainsKey( key ) ) {
				return textures[key];
			}

			var path = Path.Combine( this.path, key + ".png" );
			if ( !File.Exists( path ) ) return null;

			var tex = new Texture2D( 1, 1 );
			tex.hideFlags = HideFlags.HideAndDontSave;

			var bytes = File.ReadAllBytes( path );
			tex.LoadImage( bytes );

			textures.Add( key, tex );
			return textures[key];
		}
	}
}
#endif