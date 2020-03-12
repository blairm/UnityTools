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