using UnityEngine;
using UnityEditor;

[ CustomEditor( typeof( CustomAsset ) ) ]
public class CustomComponentEditor : Editor
{
	public void OnEnable()
	{
		_target = target as CustomAsset;
	}
	
	override public void OnInspectorGUI()
	{
		_target.text = EditorGUILayout.TextField( "customAsset", _target.text, EditorStyles.textField );
		
        if( GUI.changed )
            EditorUtility.SetDirty( target );
	}
	
	
	private CustomAsset _target;
}