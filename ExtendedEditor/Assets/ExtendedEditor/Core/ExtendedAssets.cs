#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using TNRD.Json;
using UnityEngine;

namespace TNRD.Editor.Core {

    /// <summary>
    /// Asset manager for the Extended Editor
    /// </summary>
    public class ExtendedAssets {

        [JsonIgnore]
        private Dictionary<string, Texture2D> textures;

        [JsonProperty]
        /// <summary>
        /// The path where the assets are stored
        /// </summary>
        public string Path;

        /// <summary>
        /// Creates a new instance of ExtendedAssets
        /// </summary>
        public ExtendedAssets() {
            textures = new Dictionary<string, Texture2D>();
        }

        /// <summary>
        /// Creates a new instance of ExtendedAssets
        /// </summary>
        /// <param name="path">The path to the asset folder</param>
        public ExtendedAssets( string path ) {
            textures = new Dictionary<string, Texture2D>();
            Path = path;
        }

        public ExtendedAssets( ExtendedEditor editor ) {
            textures = new Dictionary<string, Texture2D>();
            var type = editor.GetType();
            var files = Directory.GetFiles( Application.dataPath, string.Format( "*{0}.cs", type.Name ), SearchOption.AllDirectories );
            if ( files.Length == 1 ) {
                var f = files[0];
                var fi = new FileInfo( f );
                Path = System.IO.Path.Combine( fi.DirectoryName, "Assets/" );
            }
        }

        /// <summary>
        /// Creates a new instance of ExtendedAssets
        /// </summary>
        /// <param name="path">The path to the asset folder</param>
        /// <param name="window">The window containing this asset manager</param>
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

        public Texture2D this[string key] {
            get {
                if ( textures.ContainsKey( key ) ) {
                    return textures[key];
                } else {
                    return Load( key );
                }
            }
        }

        /// <summary>
        /// Loads a texture from the asset path with the given key
        /// </summary>
        /// <param name="key">The name of the texture file without the extension</param>
        /// <returns>Texture2D or null if the texture file does not exist</returns>
        public Texture2D Load( string key ) {
            return Load( key, Path );
        }

        /// <summary>
        /// Loads a texture from the given location with the given key
        /// </summary>
        /// <param name="key">The name of the texture file without the extension</param>
        /// <param name="location">The location to look for the texture file</param>
        /// <returns>Texture2D or null if the texture file does not exist</returns>
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

        /// <summary>
        /// Converts a base 64 string into a texture and stores it in the local instance
        /// </summary>
        /// <param name="key">The key to store the texture with</param>
        /// <param name="b64">The base 64 string representing an image</param>
        /// <returns>Texture2D</returns>
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

        public void Destroy( ExtendedWindow window ) {
            foreach ( var item in textures ) {
                window.Editor.DestroyAsset( item.Value );
            }
        }
    }
}
#endif