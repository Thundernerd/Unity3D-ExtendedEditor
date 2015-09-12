using UnityEngine;
using TNRD.Editor.Core;

namespace TNRD.Editor.Controls {

    public class Image : ExtendedControl {

        private Texture2D image;
        private string imageToLoad = "";

        public ScaleMode ScaleMode;

        public Image() { }

        public Image( string name ) :
            this( name, ScaleMode.StretchToFill ) { }

        public Image( string name, ScaleMode scaleMode ) {
            imageToLoad = name;
            ScaleMode = scaleMode;
        }

        public Image( Texture2D texture )
            : this( texture, ScaleMode.StretchToFill ) { }

        public Image( Texture2D texture, ScaleMode scaleMode ) {
            image = texture;
            ScaleMode = scaleMode;
        }

        public override void OnInitialize() {
            base.OnInitialize();

            if ( image == null && !string.IsNullOrEmpty( imageToLoad ) ) {
                image = Window.Assets[imageToLoad];
            }

            if ( Size.x == 0 && Size.y == 0 ) {
                Size = new Vector2( 100, 100 );
            }
        }

        public override void OnGUI() {
            base.OnGUI();

            GUI.Box( Rectangle, "" );
            GUI.DrawTexture( Rectangle, image, ScaleMode );
        }

        public void Load( string name ) {
            image = Window.Assets[name];
        }

        public void Load( Texture2D texture ) {
            image = texture;
        }
    }
}
