using System.IO;
using System.Text;
using TNRD.Editor.Core;
using UnityEditor;
using UnityEngine;

public class DocsGenerator : MonoBehaviour {

	// Use this for initialization
	[MenuItem("TNRD/Create ObjectDocs")]
	private static void CreateDocs() {
		var t = typeof(ExtendedEditor);
		var types = t.Assembly.GetTypes();
		foreach ( var item in types ) {
			if ( item.Namespace == null ) continue;
			if ( item.Namespace.StartsWith( "TNRD" ) ) {

				var builder = new StringBuilder();

				builder.Append(
@"<!DOCTYPE html>
<html lang=""en"" class=""no-js"">
    <head>
        <title>Extended Editor Documentation</title>
        <meta name=""msapplication-TileColor"" content=""#222c37"" />
        <script type=""text/javascript"" src=""js/jquery.jsu""></script>
        <script type=""text/javascript"" src=""js/toc.jsu""></script>
        <script type=""text/javascript"" src=""js/core.jsu""></script>
        <link href=""http://fonts.googleapis.com/css?family=Open+Sans:400,700,400italic"" rel=""stylesheet"" type=""text/css"" />
        <link rel=""stylesheet"" type=""text/css"" href=""css/core.cssu"" />
    </head>
    <body>
        <div class=""header-wrapper"">
            <div id=""header"" class=""header"">
                <div class=""content"">
                    <div class=""more"">
                        <div class=""filler""></div>
                    </div>
                </div>
            </div>
            <div class=""toolbar"">
                <div class=""content clear""></div>
            </div>
        </div>
        <div id=""master-wrapper"" class=""master-wrapper clear"">
            <div id=""sidebar"" class=""sidebar"">
                <div class=""sidebar-wrap"">
                    <div class=""content"">
                        <div class=""sidebar-menu"">
                            <div class=""toc"">
                                <h2>Scripting API</h2>
                            </div>
                        </div>
                        <p>
                            <br/>
                        </p>
                    </div>
                </div>
            </div>
            <div id=""content-wrap"" class=""content-wrap"">
                <div class=""content-block"">
					<div class=""content"">" );

				builder.AppendFormat(
@"<div class=""section"">
	<div class=""mb20 clear"" id="""">
		<h1 class=""heading inherit"">{0}</h1>
		<div class=""clear""></div>
		<p class=""cl mb0 left mr10"">class in {1} </p>
        <div class=""clear""></div>
	</div>
	<div class=""subsection"">
		<h2>Description</h2>
		<p></p>
	</div>", item.Name, item.Namespace );


				#region Variables
				builder.Append(
@"<div clas=""subsection"">
	<h2>Variables</h2>
	<table class=""list"">
		<tbody>" );

				var variables = item.GetFields( System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly );
				foreach ( var variable in variables ) {
					if ( variable.IsPrivate ) continue;
					//<a href=""{0}-{1}.html"">{1}</a>
					builder.AppendFormat(
@"<tr>
	<td class=""lbl"">
		<a href=""#"">{1}</a>
	</td>
	<td class=""desc"">...</td>
</tr>", item.Name, variable.Name );
				}

				builder.Append(
@"		</tbody>
	</table>
</div>" );
				#endregion

				#region Constructors
				builder.Append(
@"<div clas=""subsection"">
	<h2>Constructors</h2>
	<table class=""list"">
		<tbody>" );

				var constructors = item.GetConstructors( System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly );
				for ( int i = 0; i < constructors.Length; i++ ) {
					if ( constructors[i].IsPrivate ) continue;
					//<a href=""{0}-ctor-{1}.html"">{0}</a>
					builder.AppendFormat(
@"<tr>
	<td class=""lbl"">
		<a href=""#"">{0}</a>
	</td>
	<td class=""desc"">...</td>
</tr>", item.Name, i );
				}

				builder.Append(
@"		</tbody>
	</table>
</div>" );

				#endregion

				#region Public Functions
				builder.Append(
@"<div clas=""subsection"">
	<h2>Public functions</h2>
	<table class=""list"">
		<tbody>" );

				var pubFunctions = item.GetMethods( System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly );
				foreach ( var function in pubFunctions ) {
					if ( function.IsPrivate ) continue;
					//<a href=""{0}-{1}.html"">{1}</a>
					builder.AppendFormat(
@"<tr>
	<td class=""lbl"">
		<a href=""#"">{1}</a>
	</td>
	<td class=""desc"">...</td>
</tr>", item.Name, function.Name );
				}

				builder.Append(
@"		</tbody>
	</table>
</div>" );
				#endregion

				#region Static Functions
				builder.Append(
@"<div clas=""subsection"">
	<h2>Static functions</h2>
	<table class=""list"">
		<tbody>" );

				var statFunctions = item.GetMethods( System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public );
				foreach ( var function in statFunctions ) {
					//<a href=""{0}-{1}.html"">{1}</a>
					builder.AppendFormat(
@"<tr>
	<td class=""lbl"">
		<a href=""#"">{1}</a>
	</td>
	<td class=""desc"">...</td>
</tr>", item.Name, function.Name );
				}

				builder.Append(
@"		</tbody>
	</table>
</div>" );
				#endregion

				builder.Append(
@"                </div>
                <div class=""footer-wrapper"">
                    <div class=""footer clear""></div>
                </div>
            </div>
			</div>
        </div>
    </div>
</div>
</body>
</html>" );

				//Debug.Log( builder.ToString() );
				File.WriteAllText( string.Format( "{0}.{1}.html", item.Namespace, item.Name ).Replace( "`1", "" ), builder.ToString() );
				//var s = "{";
				//s += string.Format( "\n\t \"link\":\"{1}\", \n\t \"title\":\"{0}\", \n\t \"children\":[] \n", item.Name, item.Namespace );
				//s += "},";
				//Debug.Log( s );
			}
		}
	}

	// Update is called once per frame
	void Update() {

	}
}
