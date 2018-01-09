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

using UnityEditor;
using UnityEngine;

namespace SMI.PlanetTextureGenerator.Editor
{
	public class PlanetPreviewWindow : EditorWindow
	{
		public const int	labelGap	= 20;
		public const int	rowGap		= 25;

		static public void ShowWindow()
		{
			EditorWindow.GetWindow< PlanetPreviewWindow >();
		}

		public void updatePreview()
		{
			if( Selection.activeObject != null )
			{
				if( Selection.activeObject is PlanetAsset && Selection.activeObject != _target )
					_target = Selection.activeObject as PlanetAsset;
			}

			if( textureGenerator == null )
				textureGenerator = new PlanetTextureGenerator();

			textureGenerator._target = _target;
			textureGenerator.updatePreview();
		}

		public void OnGUI()				{ if( textureGenerator != null ) showPreviewGUI(); }
		public void OnSelectionChange()	{ updateTargetIfChanged(); }
		public void OnInspectorUpdate()	{ Repaint(); }
	
	
		private PlanetAsset				_target;
		private PlanetTextureGenerator	textureGenerator;

		private Vector2					scrollPosition;

		private void showPreviewGUI()
		{
			GUILayout.Space( 5 );

			scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, GUILayout.MaxWidth( 537 ), GUILayout.MaxHeight( 470 ) );

			Rect paramRect = EditorGUILayout.BeginVertical( GUILayout.Width( 522 ), GUILayout.Height( 455 ) );
			{
				//row 0
				paramRect = EditorGUILayout.BeginHorizontal();
				{
					paramRect = EditorGUILayout.BeginVertical( GUILayout.MaxWidth( PlanetTextureGenerator.previewWidth + 10 ), GUILayout.MaxHeight( PlanetTextureGenerator.previewHeight ) );
					{
						GUILayout.Label( "Diffuse", EditorStyles.label );
						paramRect.y		+= labelGap;
						paramRect.width	= PlanetTextureGenerator.previewWidth;
						EditorGUI.DrawPreviewTexture( paramRect, textureGenerator.diffusePreview );
					}
					EditorGUILayout.EndVertical();

					paramRect = EditorGUILayout.BeginVertical( GUILayout.MaxWidth( PlanetTextureGenerator.previewWidth ), GUILayout.MaxHeight( PlanetTextureGenerator.previewHeight ) );
					{
						GUILayout.Label( "Specular", EditorStyles.label );
						paramRect.y += labelGap;
						EditorGUI.DrawTextureAlpha( paramRect, textureGenerator.diffusePreview );
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();


				GUILayout.Space( rowGap );

				//row 1
				paramRect = EditorGUILayout.BeginHorizontal();
				{
					paramRect = EditorGUILayout.BeginVertical( GUILayout.MaxWidth( PlanetTextureGenerator.previewWidth + 10 ), GUILayout.MaxHeight( PlanetTextureGenerator.previewHeight ) );
					{
						GUILayout.Label( "Normal", EditorStyles.label );
						paramRect.y		+= labelGap;
						paramRect.width	= PlanetTextureGenerator.previewWidth;
						EditorGUI.DrawPreviewTexture( paramRect, textureGenerator.normalPreview );
					}
					EditorGUILayout.EndVertical();
				
					paramRect = EditorGUILayout.BeginVertical( GUILayout.MaxWidth( PlanetTextureGenerator.previewWidth ), GUILayout.MaxHeight( PlanetTextureGenerator.previewHeight ) );
					{
						GUILayout.Label( "Height", EditorStyles.label );
						paramRect.y += labelGap;
						EditorGUI.DrawTextureAlpha( paramRect, textureGenerator.normalPreview );
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();


				GUILayout.Space( rowGap );
			
				//row 2
				paramRect = EditorGUILayout.BeginHorizontal();
				{
					paramRect = EditorGUILayout.BeginVertical( GUILayout.MaxWidth( PlanetTextureGenerator.previewWidth + 10 ), GUILayout.MaxHeight( PlanetTextureGenerator.previewHeight ) );
					{
						GUILayout.Label( "Illumination", EditorStyles.label );
						paramRect.y		+= labelGap;
						paramRect.width	= PlanetTextureGenerator.previewWidth;
						EditorGUI.DrawPreviewTexture( paramRect, textureGenerator.illuminationPreview );
					}
					EditorGUILayout.EndVertical();
				
					paramRect = EditorGUILayout.BeginVertical( GUILayout.MaxWidth( PlanetTextureGenerator.previewWidth ), GUILayout.MaxHeight( PlanetTextureGenerator.previewHeight ) );
					{
						GUILayout.Label( "Cloud", EditorStyles.label );
						paramRect.y += labelGap;
						EditorGUI.DrawPreviewTexture( paramRect, textureGenerator.cloudPreview );
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndScrollView();
		}

		private void updateTargetIfChanged()
		{
			if( Selection.activeObject != null )
			{
				if( Selection.activeObject is PlanetAsset && Selection.activeObject != _target )
					updatePreview();
			}
		}
	}
}