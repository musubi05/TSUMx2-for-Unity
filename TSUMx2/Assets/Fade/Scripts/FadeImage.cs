/*
 The MIT License (MIT)

Copyright (c) 2013 yamamura tatsuhiko

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeImage : UnityEngine.UI.Graphic , IFade
{
	[SerializeField]
	private Texture maskTexture = null;

	[SerializeField, Range (0, 1)]
	private float cutoutRange;

	public float Range {
		get {
			return cutoutRange;
		}
		set {
			cutoutRange = value;
			UpdateMaskCutout (cutoutRange);
		}
	}

    private Image _inheritingFlickerImage = null;

    protected override void Awake() {
        base.Awake();

        var imageObject = this.transform.Find("Image");
        if(imageObject == null) {
            return;
        }
        _inheritingFlickerImage = this.transform.Find("Image").gameObject.GetComponent<Image>();
       
        // Enable inheriting flicker image
        _inheritingFlickerImage.gameObject.SetActive(true);
        _inheritingFlickerImage.color = new Color(
            this.color.r,
            this.color.g,
            this.color.b,
            cutoutRange
            );
    }

    protected override void Start ()
	{
		base.Start ();
		UpdateMaskTexture (maskTexture);
        if(_inheritingFlickerImage != null) {
            _inheritingFlickerImage.gameObject.SetActive(false);
        }
	}

    private void UpdateMaskCutout (float range)
	{
		enabled = true;
		material.SetFloat ("_Range", 1 - range);

		if (range <= 0) {
			this.enabled = false;
		}
	}

	public void UpdateMaskTexture (Texture texture)
	{
		material.SetTexture ("_MaskTex", texture);
		material.SetColor ("_Color", color);
	}

	#if UNITY_EDITOR
	protected override void OnValidate ()
	{
		base.OnValidate ();
		UpdateMaskCutout (Range);
		UpdateMaskTexture (maskTexture);
	}
	#endif
}
