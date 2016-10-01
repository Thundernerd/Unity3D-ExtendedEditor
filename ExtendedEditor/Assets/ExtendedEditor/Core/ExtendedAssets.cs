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

        [IgnoreSerialization]
        private Dictionary<string, Texture2D> textures;
        [IgnoreSerialization]
        private Dictionary<string, string> texts;

        private string[] resources;

        public string Path;

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

        private string GetPath( string key, string ext ) {
#if IS_LIBRARY
            var separator = ".";
#else
            var separator = "/";
#endif

            if ( !ext.StartsWith( "." ) ) {
                ext = "." + ext;
            }

            if ( EditorGUIUtility.isProSkin ) {
                var v = resources.Where( r => r.EndsWith( string.Format( "pro{0}{1}{2}", separator, key, ext ) ) ).FirstOrDefault();
                if ( v != null ) {
                    return v;
                }
            }

            return resources.Where( r => !r.Contains( "pro" + separator ) && r.EndsWith( key + ext ) ).FirstOrDefault();
        }

        public Texture2D Texture( string key ) {
            return Texture( key, "png" );
        }

        public Texture2D Texture( string key, string ext ) {
            if ( textures.ContainsKey( key ) ) {
                return textures[key];
            }

            var path = GetPath( key, ext );
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
            return Text( key, "txt" );
        }

        public string Text( string key, string ext ) {
            if ( texts.ContainsKey( key ) ) {
                return texts[key];
            }

            var path = GetPath( key, ext );
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
            var path = GetPath( key, ext );
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
    }
}
#endif