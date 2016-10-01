#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if IS_LIBRARY
using System.Reflection;
#endif
using TNRD.Editor.Serialization;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor {

    public class ExtendedAssets {

        public enum EAssetType {
            Text,
            Texture
        }

        [IgnoreSerialization]
        private Dictionary<string, Texture2D> textures;
        [IgnoreSerialization]
        private Dictionary<string, string> texts;

        private string[] resources;

        public string Path;

        public Texture2D this[string key] {
            get {
                if ( textures.ContainsKey( key ) ) {
                    return textures[key];
                } else {
                    return Texture( key );
                }
            }
        }

        public ExtendedAssets() {
            textures = new Dictionary<string, Texture2D>();
            texts = new Dictionary<string, string>();
        }

        public void Initialize( string path ) {
#if IS_LIBRARY
            resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
#else
            if ( !string.IsNullOrEmpty( path ) ) {
                Path = path;
            } else {
                var stack = new System.Diagnostics.StackTrace( true );
                if ( stack.FrameCount > 0 ) {
                    var frame = stack.GetFrame( stack.FrameCount - 1 );
                    var fname = System.IO.Path.GetFileName( frame.GetFileName() );

                    Path = frame.GetFileName().Replace( '\\', '/' );
                    Path = Path.Replace( fname, "Assets/" );
                }
            }

            resources = Directory.GetFiles( Path, "*", SearchOption.AllDirectories )
                .Where( f => !f.EndsWith( ".meta" ) )
                .Select( f => f.Replace( "\\", "/" ) )
                .ToArray();
#endif
        }

        private string GetExtension( EAssetType type ) {
            switch ( type ) {
                case EAssetType.Text:
                    return ".txt";
                case EAssetType.Texture:
                    return ".png";
            }

            return "";
        }

        private string GetPath( string key, EAssetType type ) {
#if IS_LIBRARY
            var separator = ".";
#else
            var separator = "/";
#endif

            if ( EditorGUIUtility.isProSkin ) {
                var v = resources.Where( r => r.EndsWith( string.Format( "pro{0}{1}{2}", separator, key, GetExtension( type ) ) ) ).FirstOrDefault();
                if ( v != null ) {
                    return v;
                }
            }

            return resources.Where( r => !r.Contains( "pro" + separator ) && r.EndsWith( key + GetExtension( type ) ) ).FirstOrDefault();
        }

        public Texture2D Texture( string key ) {
            if ( textures.ContainsKey( key ) ) {
                return textures[key];
            }

            var path = GetPath( key, EAssetType.Texture );
            if ( string.IsNullOrEmpty( path ) )
                return null;

            var tex = new Texture2D( 1, 1 );
            tex.hideFlags = HideFlags.HideAndDontSave;

#if IS_LIBRARY
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream( path );
            byte[] bytes;
            using ( var ms = new MemoryStream() ) {
                var buffer = new byte[4096];
                var count = 0;
                while ( ( count = stream.Read( buffer, 0, buffer.Length ) ) != 0 ) {
                    ms.Write( buffer, 0, count );
                }
                bytes = ms.ToArray();
            }
            tex.LoadImage( bytes );
#else
            var bytes = File.ReadAllBytes( path );
            tex.LoadImage( bytes );
#endif

            textures.Add( key, tex );
            return textures[key];
        }

        public string Text( string key ) {
            if ( texts.ContainsKey( key ) ) {
                return texts[key];
            }

            var path = GetPath( key, EAssetType.Text );
            if ( string.IsNullOrEmpty( path ) )
                return "";

#if IS_LIBRARY
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream( path );
            var data = "";
            using ( var reader = new StreamReader( stream ) ) {
                data = reader.ReadToEnd();
            }
            return data;
#else
            var data = File.ReadAllText( path );
            return data;
#endif
        }

        public byte[] Blob( string key, string ext ) {
#if IS_LIBRARY
            var separator = ".";
#else
            var separator = "/";
#endif

            var path = "";

            if ( EditorGUIUtility.isProSkin ) {
                path = resources.Where( r => r.EndsWith( string.Format( "pro{0}{1}{2}", separator, key, ext ) ) ).FirstOrDefault();
            }

            if ( string.IsNullOrEmpty( path ) ) {
                path = resources.Where( r => !r.Contains( "pro" + separator ) && r.EndsWith( key + ext ) ).FirstOrDefault();
            }

            if ( string.IsNullOrEmpty( path ) )
                return null;

#if IS_LIBRARY
            var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream( path );
            var bytes = new byte[stream.Length];
            stream.Read( bytes, 0, (int)stream.Length );
            return bytes;
#else
            var bytes = File.ReadAllBytes( path );
            return bytes;
#endif
        }

        public Texture2D B64( string key, string b64 ) {
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

        public bool HasKey( string key ) {
            return textures.ContainsKey( key );
        }
    }
}
#endif