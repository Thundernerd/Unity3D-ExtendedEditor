#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using TNRD.Json;
using UnityEngine;

namespace TNRD.Editor.Core {

	[DocsDescription("Asset manager for the Extended Editor")]
	public class ExtendedAssets {

		[JsonIgnore]
		private Dictionary<string, Texture2D> textures;
		[JsonProperty]
		[DocsDescription("The path where the assets are stored")]
		public string Path;

		[DocsDescription("Creates a new instance of ExtendedAssets")]
		public ExtendedAssets() {
			textures = new Dictionary<string, Texture2D>();
		}

		[DocsDescription("Creates a new instance of ExtendedAssets")]
		[DocsParameter("path", "The path to the asset folder")]
		public ExtendedAssets( string path ) {
			textures = new Dictionary<string, Texture2D>();
			Path = path;
		}

		[DocsDescription("Creates a new instance of ExtendedAssets")]
		[DocsParameter("path", "The path to the asset folder")]
		[DocsParameter("window", "The window containing this asset manager")]
		public ExtendedAssets( string path, ExtendedWindow window ) {
			if ( string.IsNullOrEmpty( path ) ) {
				var type = window.Editor.GetType();
				var files = Directory.GetFiles( Application.dataPath, string.Format( "*{0}.cs", type.Name ), SearchOption.AllDirectories );
				if ( files.Length == 1 ) {
					var f = files[0];
					var fi = new FileInfo( f );
					this.Path = System.IO.Path.Combine( fi.DirectoryName, "Assets/" );
				}
			} else {
				this.Path = path.ToLower().StartsWith( "assets" ) ? path : string.Format( "Assets/{0}", path );
			}

			textures = new Dictionary<string, Texture2D>();
		}

		[DocsIgnore]
		public Texture2D this[string key] {
			get {
				if ( textures.ContainsKey( key ) ) {
					return textures[key];
				} else {
					return Load( key );
				}
			}
		}

		[DocsDescription("Loads a texture from the asset path with the given key")]
		[DocsParameter("key", "The name of the texture file without the extension")]
		[DocsReturnValue("Texture2D or null if the texture file does not exist")]
        public Texture2D Load( string key ) {
			return Load( key, Path );
		}

		[DocsDescription("Loads a texture from the given location with the given key")]
		[DocsParameter("key", "The name of the texture file without the extension")]
		[DocsParameter("location", "The location to look for the texture file")]
		[DocsReturnValue("Texture2D or null if the texture file does not exist")]
		public Texture2D Load( string key, string location ) {
			if ( textures.ContainsKey( key ) ) {
				return textures[key];
			}

			var path = System.IO.Path.Combine( location, key + ".png" );
			if ( !File.Exists( path ) ) return null;

			var tex = new Texture2D( 1, 1 );
			tex.hideFlags = HideFlags.HideAndDontSave;

			var bytes = File.ReadAllBytes( path );
			tex.LoadImage( bytes );

			textures.Add( key, tex );
			return textures[key];
		}

		[DocsDescription("Converts a base 64 string into a texture and stores it in the local instance")]
		[DocsParameter("key", "The key to store the texture with")]
		[DocsParameter("b64", "The base 64 string representing an image")]
		[DocsReturnValue("Texture2D")]
		public Texture2D FromBase64( string key, string b64 ) {
			if ( textures.ContainsKey( key ) ) {
				return textures[key];
			}

			var tex = new Texture2D( 1, 1 );
			tex.hideFlags = HideFlags.HideAndDontSave;

			var bytes = System.Convert.FromBase64String( b64 );
			tex.LoadImage( bytes );

			textures.Add( key, tex );
			return textures[key];
		}
		
		[DocsDescription("Destroys all the loaded texture assets")]
		public void Destroy( ExtendedWindow window ) {
			foreach ( var item in textures ) {
				window.Editor.DestroyAsset( item.Value );
			}
		}
	}
}
#endif