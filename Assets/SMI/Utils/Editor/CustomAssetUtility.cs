/*
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org>
//*/

using UnityEngine;
using UnityEditor;
using System.IO;

namespace SMI.Utils.Editor
{
	public static class CustomAssetUtility
	{
		public static void CreateAsset< T >() where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance< T >();
	        
			string path = AssetDatabase.GetAssetPath( Selection.activeObject );

			if( path == "" )
				path = "Assets";
			else if( Path.GetExtension( path ) != "" )
				path = path.Replace( Path.GetFileName( AssetDatabase.GetAssetPath( Selection.activeObject ) ), "" );
	        
			string assetName		= typeof( T ).ToString();
			assetName				= "/New " + assetName.Substring( assetName.LastIndexOf( "." ) + 1 );
			string assetPathAndName	= AssetDatabase.GenerateUniqueAssetPath( path + assetName + ".asset" );
	        
			AssetDatabase.CreateAsset( asset, assetPathAndName );
			AssetDatabase.SaveAssets();

			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
		}
	}
}