// Upgrade NOTE: commented out 'float4x4 _CameraToWorld', a built-in variable
// Upgrade NOTE: replaced '_CameraToWorld' with 'unity_CameraToWorld'

Shader "Custom/Mandelbulb" {
Properties {
	_MainTex("Texture", 2D) = "white" {}
	_Color("Color", Color) = (1,1,1,1)
}
SubShader {
		Tags { "RenderType" = "Opaque" }
Pass{
	//Blend One OneMinusSrcAlpha, One One
	//BlendOp Add
	
	Blend SrcAlpha OneMinusSrcAlpha
	CGPROGRAM
	#include "UnityCG.cginc"
	#pragma target 4.0
	#pragma vertex vert_img
	#pragma fragment frag
		
	#define black float4(0,0,0,1)
	#define green float4(0,1,0,1)	
	#define white float4(1,1,1,1)
	#define blue  float4(0,0,0.7,1)		
	#define red float4(1,0,0,1)	
	#define orange float4(1, 0.64, 0,1)
	#define yellow  float4(1,1,0,1)	
	#define seethrough float4(0,0,0,0)

	sampler2D _MainTex;
	float4 _MainTex_ST;
	fixed4 _Color;

	// built-in Unity shader variables (http://docs.unity3d.com/462/Documentation/Manual/SL-BuiltinValues.html)
	// these don't need to be declared. listing what's used in the shader for readability
	//float		_WorldSpaceCameraPos;
	//float4	_ScreenParams;

	// shader variables set by Unity if declared
	// float4x4	_CameraToWorld;

	// uniforms set in script
	//float4x4	_CameraToWorldMatrix;
	float3		_LightDir;
	float		_Exponent;
	int			_NumIterations;
	int			_NumRayMarchSteps;
	float		_Fov;

	#include "utility.cginc"
	#include "distanceFunctions.cginc"




	// Raymarch scene given an origin and direction using sphere tracing
	// Each step, we move by the closest distance to any object in the scene from the current ray point.
	// We stop when we're really close to an object (or) if we've exceeded the number of steps
	// Returns a greyscale color based on number of steps marched (white --> less, black --> more)
	float4
	raymarch(float3 rayo, float3 rayd) 
	{			
		const float minimumDistance = 0.0001;
		float3 p = rayo;
		bool hit = false;		
		float distanceStepped = 0.0;
		int steps;

		for(steps = 0; steps < _NumRayMarchSteps; steps++)
		{			
			float d = DE(p);
			distanceStepped += d;

			if (d < minimumDistance) {
				hit = true;
				break;
			}

			p += d * rayd;
		}			

		float greyscale = 1 - (steps/(float)_NumRayMarchSteps);
		//if (greyscale < 0.1) {
		//	return float4(0, 0, 0, 0);
		//}
		return float4(greyscale, greyscale, greyscale, 1-greyscale);			
	}

	// Find ray direction through the current pixel in camera space (LHS, camera looks down +Z)
	float3
	get_eye_ray_through_pixel(float2 svpos)
	{
		//// Get the ray direction for the current pixel in LHS (coz Unity uses LHS for all its game objects)		
		// get pixel coordinate in [-1, 1] normalized space
		float2 pixelNormPos = -1 + (2 * svpos.xy) / _ScreenParams;
		
		// account for aspect ratio
		pixelNormPos.x *= _ScreenParams.x / _ScreenParams.y;
		
		// DX has its origin at the top left, meaning Y goes downwards. Switch to Y upwards.
		pixelNormPos.y *= -1; 

		// get zNear in normalized coordinates using the vertical field of view
		// the camera looks down +Z, so it needs to be +ive
		float zNearNorm = rcp( tan(_Fov/2) ); // tan(fov/2) = 1 / zn_norm

		return normalize(float3(pixelNormPos, zNearNorm)); // LHS
	}


	float4 
	frag(v2f_img i) : COLOR
	{
		float3 csRay = get_eye_ray_through_pixel(i.pos);		
		return float4(csRay, 1); // visualize if ray direction was calculated correctly
		
		// The _CameraToWorld matrix we use below is not set in the C# script. 
		// It's one of the built-in variables set by Unity.
		
		// if you choose to pass the camera to world matrix, do NOT use camera.cameraToWorldMatrix
		// that one uses RHS (file:///C:/Program%20Files/Unity/Editor/Data/Documentation/en/ScriptReference/Camera-cameraToWorldMatrix.html)
		// Use camera.transform.localToWorldMatrix instead.		
		float3 wsRay = mul(unity_CameraToWorld, float4(csRay, 0)); // column-major matrix, so post mult

		float4 color = raymarch(_WorldSpaceCameraPos, wsRay);		
		color = tex2D(_MainTex, i.uv) * color * _Color;
		return color;
	}
	ENDCG
} // Pass

} // Subshader
	FallBack "Oculus/Unlit Transparent Color"
}
