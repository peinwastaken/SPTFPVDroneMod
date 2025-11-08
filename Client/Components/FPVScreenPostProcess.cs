using UnityEngine;

namespace FPVDroneMod.Components
{
    [RequireComponent(typeof(Camera))]
    public class FPVScreenPostProcess : MonoBehaviour
    {
        public Material analogMat;
        public Material noiseMat;
        public Material blurMat;
        public Material scanMat;

        private RenderTexture _rt1;
        private RenderTexture _rt2;
        private RenderTexture _rt3;

        private void ReleaseRenderTextures()
        {
            if (_rt1 != null) { _rt1.Release(); _rt1 = null; }
            if (_rt2 != null) { _rt2.Release(); _rt2 = null; }
            if (_rt3 != null) { _rt3.Release(); _rt3 = null; }
        }

        private void OnDisable()
        {
            ReleaseRenderTextures();
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            int w = Screen.width;
            int h = Screen.height;
            if (_rt1 == null || _rt1.width != w || _rt1.height != h)
            {
                ReleaseRenderTextures();
                _rt1 = new RenderTexture(w, h, 0, src.format);
                _rt2 = new RenderTexture(w, h, 0, src.format);
                _rt3 = new RenderTexture(w, h, 0, src.format);
                _rt1.Create();
                _rt2.Create();
                _rt3.Create();
            }
            
            // noise
            Graphics.Blit(src, _rt1, noiseMat);
        
            // blur
            Graphics.Blit(_rt1, _rt2, blurMat);
        
            // scan
            Graphics.Blit(_rt2, _rt3, scanMat);

            // analog
            Graphics.Blit(_rt3, _rt1, analogMat);
            
            // flip and display
            Graphics.Blit(_rt1, dest, new Vector2(1, -1), new Vector2(0, 1));
        }

        private void OnDestroy()
        {
            ReleaseRenderTextures();
        }
    }
}
