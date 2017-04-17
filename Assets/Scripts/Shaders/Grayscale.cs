using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	/*
	 * Grayscale is a part of the UnityStandardAssets library. By default it does not support fading, it is either on or off.
	 * The following changes have been made, to add fading functionality:
	 * public float effectAmount = 1; added
	 * material.SetFloat("_EffectAmount", effectAmount); added
	 * 
	 * GrayscaleEffect is a shader file, used by Unity. The following changes were made to it, to add fading functionality:
	 * "_EffectAmount ("Effect Amount", Range (0,1)) = 1.0" added
	 * "uniform float _EffectAmount;" added
	 * return output; changed to return lerp(original, output, _EffectAmount);
	 */ 
    [ExecuteInEditMode]
    [AddComponentMenu("Image Effects/Color Adjustments/Grayscale")]
    public class Grayscale : ImageEffectBase {
        public Texture  textureRamp;
        public float    rampOffset;
		public float effectAmount = 1;

        // Called by camera to apply image effect
        void OnRenderImage (RenderTexture source, RenderTexture destination) {
            material.SetTexture("_RampTex", textureRamp);
            material.SetFloat("_RampOffset", rampOffset);
			material.SetFloat("_EffectAmount", effectAmount);
            Graphics.Blit (source, destination, material);
        }
    }
}
