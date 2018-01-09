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
	public class ToggleActiveChild : EditorWindow
	{
		[ MenuItem( "SMI/Game Objects/Toggle Active Child" ) ]
		public static void InitWindow()
		{
			EditorWindow.GetWindow< ToggleActiveChild >();
		}

		public void OnGUI()
		{
			if( children == null )
				return;
			
			GUILayout.Space( 10 );
			GUILayout.Label( "Children" );
			
			for( int i = 0; i < children.Count; ++i )
			{
				if( children[ i ] == null )
					continue;
				
				bool initialActive	= children[ i ].activeSelf;
				bool active			= EditorGUILayout.Toggle( children[ i ].name, initialActive );
				
				if( active != initialActive )
				{
					if( !active && initialActive )
					{
						children[ i ].SetActive( false );
					}
					else if( active && !initialActive )
					{
						ActivateOnly( i );
					}
				}
			}
			
			GUILayout.Space( 5 );
			
			if( GUILayout.Button( "Previous" ) )
				ActivatePrev();
			
			if( GUILayout.Button( "Next" ) )
				ActivateNext();
		}


		private List< GameObject > children;
		
		private void OnEnable()
		{
			ObtainChildren();
		}
		
		private void OnFocus()
		{
			ObtainChildren();
		}
		
		private void ObtainChildren()
		{
			GameObject numberPanelsParent = Selection.activeGameObject;

			if(numberPanelsParent == null)
				return;
			
			children = new List< GameObject >();

			foreach( Transform child in numberPanelsParent.transform )
			{
				child.gameObject.SetActive( false );
				children.Add( child.gameObject );
			}

			if( children.Count > 0 )
				children[ 0 ].SetActive( true );
		}

		private void ActivateNext()
		{
			if( children == null )
				return;
			
			int activeIndex	= children.FindLastIndex( x => x.activeSelf );
			int newIndex	= ( activeIndex + children.Count + 1 ) % children.Count;
			ActivateOnly( newIndex );
		}

		private void ActivatePrev()
		{
			if( children == null )
				return;
			
			int activeIndex	= children.FindIndex( x => x.activeSelf );
			int newIndex	= ( activeIndex + children.Count - 1 ) % children.Count;
			ActivateOnly( newIndex );
		}

		private void ActivateOnly( int index )
		{
			if( index > -1 && index < children.Count && children[ index ] != null )
			{
				children.ForEach( x => { if( x != null ) x.SetActive( false ); } );
				children[ index ].SetActive( true );
			}
		}
	}
}