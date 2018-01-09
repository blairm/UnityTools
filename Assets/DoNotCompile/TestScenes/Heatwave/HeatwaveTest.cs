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

using SMI.PostProcessEffects.Runtime;

public class HeatwaveTest : MonoBehaviour
{
    public GameObject	heatwavePrefab;

	public int			heatwaveCount	= 8;

	public void Awake()
	{
		heatwaveCount	= Mathf.NextPowerOfTwo( heatwaveCount );
		heatwaveList	= new HeatwaveImageEffect[ heatwaveCount ];

		for( int i = 0; i < heatwaveCount; ++i )
		{
			GameObject go		= ( GameObject ) Instantiate( heatwavePrefab );
			heatwaveList[ i ]	= go.GetComponent< HeatwaveImageEffect >();
			go.SetActive( false );
		}
	}

    public void Update()
    {
        if( Input.GetMouseButtonDown( 0 ) )
		{
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit hit;
			if( Physics.Raycast( ray, out hit, 1000 ) )
			{
				heatwaveList[ heatwaveIndex ].transform.position = hit.point;
				heatwaveList[ heatwaveIndex ].explode();
				heatwaveIndex = ++heatwaveIndex & ( heatwaveCount - 1 );
			}
		}
    }

	private HeatwaveImageEffect[]	heatwaveList;
	private int						heatwaveIndex	= 0;
}