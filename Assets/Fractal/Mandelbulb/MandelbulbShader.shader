Shader "Unlit/MandelbulbShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Fov("Fov", Float) = 1.1
        _AmbientLight("Ambient Light", Float) = 0.3
        _Color("Color", Color) = (1,1,1,1)
        _Gloss("Gloss", Float) = 1
        _NumRayMarchSteps("Ray steps", Int) = 20
        _NumIterations("Iterations", Int) = 8
        _Exponent("Exponent", Float) = 8
        _Position("Position", Vector) = ( 0, 0, 0 )
       
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"


            float3		_LightDir;
            float		_Exponent;
            int			_NumIterations;
            int			_NumRayMarchSteps;
            float		_Fov;
            float3      _Position;

            #include "utility.cginc"
            #include "distanceFunctions.cginc"

            struct VertexInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal: NORMAL;
            };

            struct VertexOutput
            {

                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal: TEXCOORD1;
                float3 worldPos: TEXCORD2;
                float3 rayDir: TEXCORD3;
                float3 rayOrigin: TEXCORD4;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Gloss;
            float _AmbientLight;
            

            VertexOutput vert (VertexInput v)
            {
                VertexOutput o;
                o.normal = v.normal;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.rayDir = o.worldPos - _WorldSpaceCameraPos.xyz;
                o.rayOrigin = _WorldSpaceCameraPos.xyz;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }


            float raymarch(float3 raySrc, float3 rayDirection, float4 pos)
            {
                const float minimumDistance = 0.0001;
                float3 p = raySrc - pos.xyz;
                bool hit = false;
                float distanceStepped = 0.0;
                int steps;

                for (steps = 0; steps < _NumRayMarchSteps; steps++)
                {
                    float d = DE(p);
                    distanceStepped += d;

                    if (d < minimumDistance) {
                        hit = true;
                        break;
                    }
                    else if (distanceStepped > distance( raySrc , _Position) * 2 ) {
                        break;
                    }

                    p += d * rayDirection;
                }

                float greyscale = 1 - (steps / (float)_NumRayMarchSteps);
                //if (greyscale < 0.1) {
                //	return float4(0, 0, 0, 0);
                //}
                if (!hit) {
                    return -1.0;
                }
                return distanceStepped;
            }

            // Find ray direction through the current pixel in camera space (LHS, camera looks down +Z)
            float3 get_eye_ray_through_pixel(float2 svpos)
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
                float zNearNorm = rcp(tan(_Fov / 2)); // tan(fov/2) = 1 / zn_norm

                return normalize(float3(pixelNormPos, zNearNorm)); // LHS
            }


            float4 mandelBrot(float2 uv) {
                float2 c = uv;
                float2 z;
                float iter;

                for (iter = 0; iter < 255; iter++) {
                    z = float2(z.x * z.x - z.y * z.y, 2 * z.x * z.y) + c;
                    if (length(z) > 2) break;
                }

                return float4((iter / 255).xxx, 1);
            }


            float sphIntersect(float3 ro, float3 rd, float4 pos)
            {
                float3 oc = ro - pos.xyz;
                float b = dot(oc, rd);
                float c = dot(oc, oc) - pos.w * pos.w;
                float h = b * b - c;
                if (h < 0.0) return -1.0;
                h = sqrt(h);
                return -b - h;
            }




            float4 frag(VertexOutput i, out float outDepth: SV_Depth) : SV_Target
            {

                float3 rayOrigin = i.rayOrigin;
                float3 rayDir = normalize(i.rayDir);
                float3 spherePos = unity_ObjectToWorld._m03_m13_m23;

                float rayHit = raymarch(rayOrigin, rayDir, float4(spherePos, 0.5));

                clip(rayHit);

                float3 worldPos = rayDir * rayHit + rayOrigin;
                float3 worldNormal = normalize(worldPos - spherePos);

                // basic lighting
                half3 worldLightDir = _WorldSpaceLightPos0.xyz;
                half ndotl = saturate(dot(worldNormal, worldLightDir));
                half3 lighting = _LightColor0 * ndotl;

                // ambient lighting
                half3 ambient = ShadeSH9(float4(worldNormal, 1));
                lighting += ambient;

                float4 clipPos = UnityWorldToClipPos(worldPos);
                outDepth = clipPos.z / clipPos.w;

                return half4(lighting, 1);




                //// Cam Position
                //float3 camPos = _WorldSpaceCameraPos.xyz;
                //float3 rayDir = camPos - i.worldPos;
                //float3 viewDir = normalize(rayDir);

                ////float3 fractalDir = get_eye_ray_through_pixel(viewDir);
                ////return float4(viewDir, 1);
                //float4 fractalColor = raymarch(camPos, viewDir);
                //return fractalColor;
                //
                //// return float4(i.worldPos, 0);
                //float3 normal = normalize(i.normal);

                //// Global Light
                //float3 lightColor = _LightColor0.rgb;
                //float3 lightDir = _WorldSpaceLightPos0.xyz;

                ////Direct Light
                //float lightFalloff = max(0, dot(lightDir,normal));
                //float3 directDiffuseLight = lightColor * lightFalloff;
                //

                //// Ambient Light
                //float3 ambientLight = lightColor * _AmbientLight;




                //// Direct specular Light
                //// Phong
                //float3 viewReflect = reflect(-viewDir,normal );
                //float3 specularFalloff = max(0, dot(viewReflect, lightDir));
                //specularFalloff = pow(specularFalloff, _Gloss);


                //float3 directSpecular = specularFalloff * lightColor;


                //
                //// Composit
                //float3 diffuseLight = ambientLight + directDiffuseLight;
                //float3 finalSurfaceColor = diffuseLight * _Color.rgb + directSpecular;



                //float3 color = finalSurfaceColor;



                //

                //

                //

                //UNITY_APPLY_FOG(i.fogCoord, color);

                //return float4(color, 1);

                //float3  csRay = get_eye_ray_through_pixel(i.clipSpacePos.xy);
                //float3 wsRay = mul(unity_CameraToWorld, float4(csRay, 0)); // column-major matrix, so post mult

                //float4 color = raymarch(_WorldSpaceCameraPos, csRay);
                //// apply fog
               
                //return color;
            }
            ENDCG
        }
    }
}
