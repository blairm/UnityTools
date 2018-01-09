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
using System.Collections.Generic;

namespace SMI.Utils.Editor
{
	public class ReplaceWithPrefab : ScriptableObject
	{
		[ MenuItem( "SMI/Utils/Replace with Prefab", true ) ]
		public static bool validate()
		{
			GameObject prefab = new List< Object >( Selection.GetFiltered( typeof( GameObject ), SelectionMode.Assets ) ).Find( x => { return AssetDatabase.IsMainAsset( x ); } ) as GameObject;

			Object[] sceneTransform = Selection.GetFiltered( typeof( Transform ), SelectionMode.ExcludePrefab );
			
			if( prefab != null && sceneTransform.Length > 0 )
				return true;
			
			return false;
		}

		[ MenuItem( "SMI/Utils/Replace with Prefab" ) ]
		public static void replace()
		{
			GameObject prefab = new List< Object >( Selection.GetFiltered( typeof( GameObject ), SelectionMode.Assets ) ).Find( x => { return AssetDatabase.IsMainAsset( x ); } ) as GameObject;
			
			if( prefab == null )
				return;
			
			Object[] sceneTransform = Selection.GetFiltered( typeof( Transform ), SelectionMode.ExcludePrefab );

			foreach( Transform t in sceneTransform )
			{
				GameObject instantiatedPrefab				= PrefabUtility.InstantiatePrefab( prefab ) as GameObject;
				Undo.RegisterCreatedObjectUndo( instantiatedPrefab, t.name );
				instantiatedPrefab.transform.parent			= t.parent;
				instantiatedPrefab.transform.localPosition	= t.localPosition;
				instantiatedPrefab.transform.localRotation	= t.localRotation;
				instantiatedPrefab.transform.localScale		= t.localScale;
				Undo.DestroyObjectImmediate( t.gameObject );
			}
		}
	}
}