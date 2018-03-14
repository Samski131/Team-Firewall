using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent (typeof(Camera))]
    [AddComponentMenu ("Image Effects/Rendering/Global Fog")]
    class GlobalFog : PostEffectsBase
	{
        //Obsolete, Removed
		//[Tooltip("Apply distance-based fog?")]
        private bool  distanceFog = false;
		[Tooltip("Exclude far plane pixels from distance-based fog? (Skybox or clear color)")]
		public bool  excludeFarPixels = true;
		[Tooltip("Distance fog is based on radial distance from camera when checked")]
		public bool  useRadialDistance = false;
		[Tooltip("Apply height-based fog?")]
		public bool  heightFog = true;
		[Tooltip("Fog top Y coordinate")]
        public float height = 40.0f;
        [Range(0.00001f, 0.5f)]
        public float heightDensity = 1.0f;
		[Tooltip("Push fog away from the camera by this amount")]
        public float startDistance = 0.0f;

        public Shader fogShader = null;
        private Material fogMaterial = null;

		public GameObject light;
        public bool fogChanging = false;
        public int fogDirection = -1;

        void Update()
        {
            //If the fog density has reached the desired amount, stop changing the fog
            if (heightDensity < 0.00001f)
            {
                heightDensity = 0.00001f;
                fogChanging = false;
            }
            else if (heightDensity > 0.5f)
            {
                heightDensity = 0.5f;
                fogChanging = false;
            }


			if(Input.GetButtonDown("Light Increase"))
			{
				
				light.GetComponent<Light>().intensity += 0.1f;
				Debug.Log("Increasing");

			}

			if(Input.GetButtonDown("Light Decrease"))
			{

				light.GetComponent<Light>().intensity -= 0.1f;
				Debug.Log("Decreasing");
			}

            //If the fog is meant to be changing, alter the values of the density based on the direction
            if (fogChanging == true)
            {
                if (fogDirection == 1)
                {
                    //Decreasing
                    heightDensity = (heightDensity * 1.05f) + 0.01f;
                }
                else
                {
                    //Increasing
                    heightDensity = (heightDensity / 1.05f) - 0.01f;
                }
            }


            //Get the button inputs for the density changing
            if (Input.GetButtonDown("Fog Density Increase") && !fogChanging)
            {
                //Increasing
                fogChanging = true;
                fogDirection = 1;

            }
            else if (Input.GetButtonDown("Fog Density Decrease") && !fogChanging)
            {
                //Decreasing
                fogChanging = true;
                fogDirection = 0;
            }


        }

        public override bool CheckResources ()
		{
            CheckSupport (true);

            fogMaterial = CheckShaderAndCreateMaterial (fogShader, fogMaterial);

            if (!isSupported)
                ReportAutoDisable ();
            return isSupported;
        }

        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CheckResources() == false || (!distanceFog && !heightFog))
            {
                Graphics.Blit(source, destination);
                return;
            }

            Camera cam = GetComponent<Camera>();
            Transform camtr = cam.transform;

            Vector3[] frustumCorners = new Vector3[4];
            cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, cam.stereoActiveEye, frustumCorners);
            var bottomLeft = camtr.TransformVector(frustumCorners[0]);
            var topLeft = camtr.TransformVector(frustumCorners[1]);
            var topRight = camtr.TransformVector(frustumCorners[2]);
            var bottomRight = camtr.TransformVector(frustumCorners[3]);

            Matrix4x4 frustumCornersArray = Matrix4x4.identity;
            frustumCornersArray.SetRow(0, bottomLeft);
            frustumCornersArray.SetRow(1, bottomRight);
            frustumCornersArray.SetRow(2, topLeft);
            frustumCornersArray.SetRow(3, topRight);

            var camPos = camtr.position;
            float FdotC = camPos.y - height;
            float paramK = (FdotC <= 0.0f ? 1.0f : 0.0f);
            float excludeDepth = (excludeFarPixels ? 1.0f : 2.0f);
            fogMaterial.SetMatrix("_FrustumCornersWS", frustumCornersArray);
            fogMaterial.SetVector("_CameraWS", camPos);
            fogMaterial.SetVector("_HeightParams", new Vector4(height, FdotC, paramK, heightDensity * 0.5f));
            fogMaterial.SetVector("_DistanceParams", new Vector4(-Mathf.Max(startDistance, 0.0f), excludeDepth, 0, 0));

            var sceneMode = RenderSettings.fogMode;
            var sceneDensity = RenderSettings.fogDensity;
            var sceneStart = RenderSettings.fogStartDistance;
            var sceneEnd = RenderSettings.fogEndDistance;
            Vector4 sceneParams;
            bool linear = (sceneMode == FogMode.Linear);
            float diff = linear ? sceneEnd - sceneStart : 0.0f;
            float invDiff = Mathf.Abs(diff) > 0.0001f ? 1.0f / diff : 0.0f;
            sceneParams.x = sceneDensity * 1.2011224087f; // density / sqrt(ln(2)), used by Exp2 fog mode
            sceneParams.y = sceneDensity * 1.4426950408f; // density / ln(2), used by Exp fog mode
            sceneParams.z = linear ? -invDiff : 0.0f;
            sceneParams.w = linear ? sceneEnd * invDiff : 0.0f;
            fogMaterial.SetVector("_SceneFogParams", sceneParams);
            fogMaterial.SetVector("_SceneFogMode", new Vector4((int)sceneMode, useRadialDistance ? 1 : 0, 0, 0));

            int pass = 0;
            if (distanceFog && heightFog)
                pass = 0; // distance + height
            else if (distanceFog)
                pass = 1; // distance only
            else
                pass = 2; // height only
            Graphics.Blit(source, destination, fogMaterial, pass);
        }
    }
}
