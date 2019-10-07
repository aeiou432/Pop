using UnityEngine;
using System.Collections;

public class GaussianBlur : PostEffectsBase {

	public Shader gaussianBlurShader;
	private Material gaussianBlurMaterial = null;

	public Material material {  
		get {
			gaussianBlurMaterial = CheckShaderAndCreateMaterial(gaussianBlurShader, gaussianBlurMaterial);
			return gaussianBlurMaterial;
		}  
	}

	// Blur iterations - larger number means more blur.
	[Range(0, 8)]
	public int iterations = 3;
	
	// Blur spread for each iteration - larger value means more blur
	[Range(0, 3.0f)]
	public float blurSpread = 0.6f;
	
	[Range(1, 8)]
	public int downSample = 2;

    [Range(0.0f, 2.0f)]
    public float strength = 1;
	void OnRenderImage (RenderTexture src, RenderTexture dest) {
		if (material != null) {
			int rtW = src.width/downSample;
			int rtH = src.height/downSample;

			RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
			buffer0.filterMode = FilterMode.Bilinear;
            float blureSize = blurSpread * Screen.width / 600;

            Graphics.Blit(src, buffer0);

			for (int i = 0; i < iterations; i++) {
				material.SetFloat("_BlurSize", 1.0f + blureSize);
                material.SetFloat("_BlurStrength", strength);
                RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

				// Render the vertical pass
				Graphics.Blit(buffer0, buffer1, material, 0);

				RenderTexture.ReleaseTemporary(buffer0);
				buffer0 = buffer1;
            }

			Graphics.Blit(buffer0, dest);
			RenderTexture.ReleaseTemporary(buffer0);
		} else {
			Graphics.Blit(src, dest);
		}
	}
}
