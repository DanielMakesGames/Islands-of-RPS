Shader "Colr/Master Shader" {

	Properties {
		_MainTex("Texture", 2D) = "white" {}
		
		_TopColor ("Top Color", Color) = (0, 1, 0, 0)
		_BottomColor ("Bottom Color", Color) = (0, 0.5, 0.5, 0)

		_FrontTopColor ("Front Top Color", Color) = (1, 0, 0, 0)
		_FrontBottomColor ("Front Bottom Color", Color) = (1, 0, 0, 0)

		_BackTopColor ("Back Top Color", Color) = (0.5, 0.5, 0, 0)
		_BackBottomColor ("Back Bottom Color", Color) = (0.5, 0.5, 0, 0)

		_RightTopColor ("Right Top Color", Color) = (0, 0, 1, 0)
		_RightBottomColor ("Right Bottom Color", Color) = (0, 0, 1, 0)

		_LeftTopColor ("Left Top Color", Color) = (0.5, 0, 0.5, 0)
		_LeftBottomColor ("Left Bottom Color", Color) = (0.5, 0, 0.5, 0)

		_GradientYStartPos ("Gradient start Y", Float) = 0
		_GradientHeight("Gradient Height", Float) = 10

		_LightTint ("Light Color", Color) = (1, 1, 1, 0)

		_AmbientColor ("Ambient Color", Color) = (0.5, 0.5, 0.5, 0.0)
		_AmbientPower ("Ambient Power", Range(0, 2.0)) = 0.0

		_LightmapColor ("Shadow Color", Color) = (0, 0, 0, 0)
		_LightmapPower ("Shadow Power", Range(0, 5.0)) = 0.5

		_Rotation ("Rotation", Range(0, 6.283185)) = 0.0
		_Offset ("Rotation offset", Vector) = (0, 0, 0)
	}

	SubShader {
		Tags { "QUEUE"="Geometry" "RenderType"="Opaque" }
		LOD 200

		Pass {
		Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Geometry" "RenderType"="Opaque" }
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fog
            #pragma multi_compile_fwdbase
            #pragma shader_feature LIGHTMAP_COLR_ON
            #pragma shader_feature FRONT_GRADIENT_ON
            #pragma shader_feature FRONT_SOLID_ON
            #pragma shader_feature BACK_GRADIENT_ON
            #pragma shader_feature BACK_SOLID_ON
            #pragma shader_feature LEFT_GRADIENT_ON
            #pragma shader_feature LEFT_SOLID_ON
            #pragma shader_feature RIGHT_GRADIENT_ON
            #pragma shader_feature RIGHT_SOLID_ON
            #pragma shader_feature FOG_BOTTOM
            #pragma shader_feature INDEPENDENT_SIDES

			#pragma vertex vert
			#pragma fragment frag

			#define PI_HALF 1.57079632679

			#include "UnityCG.cginc"
            #include "AutoLight.cginc"

			static const half3 VEC3_ONE = half3(1, 1, 1);
			static const half4 VEC4_ONE = half4(1, 1, 1, 1);
			static const half4 VEC4_ZERO = half4(0, 0, 0, 0);
            static const half3 FRONT = half3(0, 0, 1);
            static const half3 BACK = half3(0, 0, -1);
            static const half3 RIGHT = half3(1, 0, 0);
            static const half3 LEFT = half3(-1, 0, 1);
            static const half3 UP = half3(0, 1,0);
            static const half3 DOWN = half3(0, -1, 0);

			uniform float _Rotation;
			uniform float3 _Offset;

			uniform half3 _FrontTopColor;
			uniform half3 _FrontBottomColor;
			uniform half3 _BackTopColor;
			uniform half3 _BackBottomColor;

			uniform half3 _RightTopColor;
			uniform half3 _RightBottomColor;

			uniform half3 _LeftTopColor;
			uniform half3 _LeftBottomColor;
			uniform half3 _TopColor;
			uniform half3 _BottomColor;

			uniform float _GradientYStartPos;
			uniform float _GradientHeight;

			uniform half3 _AmbientColor;
			uniform half _AmbientPower;

			uniform half3 _LightTint;

			uniform half _LightmapPower;
			uniform half3 _LightmapColor;

			uniform fixed _ShadowPower;

			struct appdata {
				float4 vertex : POSITION;
				half3 normal : NORMAL;
				half3 color : COLOR;
				float4 uv : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
			};

			struct v2f {
				float4 pos : POSITION;

				float2 lightmap_uv : TEXCOORD0;
				float3 lighting : TEXCOORD1;

				float3 color : TEXCOORD2;

            	UNITY_FOG_COORDS(3)
            	LIGHTING_COORDS(4, 5)

				float2 uv : TEXCOORD6;
			};

		    // These are prepopulated by Unity
		    // sampler2D unity_Lightmap;
		    // float4 unity_LightmapST;

		    sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v) {
				v2f o;

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.pos = UnityObjectToClipPos(v.vertex);

				half3 normal = normalize(mul(unity_ObjectToWorld, half4(v.normal, 0))).xyz;

				o.color = v.color;
				
                half dirFront = 1 - acos(saturate(dot(FRONT, FRONT * normal))) / PI_HALF;
				half dirBack =  1 - acos(saturate(dot(BACK, FRONT * normal))) / PI_HALF;
				half dirRight = 1 - acos(saturate(dot(RIGHT, RIGHT * normal))) / PI_HALF;
				half dirLeft =  1 - acos(saturate(dot(LEFT, RIGHT * normal))) / PI_HALF;
				half dirUp =    1 - acos(saturate(dot(UP, UP * normal))) / PI_HALF;
				half dirDown =  1 - acos(saturate(dot(DOWN, UP * normal))) / PI_HALF;

   				float4 posGrad = mul(unity_ObjectToWorld, v.vertex);
				float posGradRotated = (posGrad.x - _Offset.x) * sin(_Rotation) + 
									   (posGrad.y - _Offset.y) * cos(_Rotation);
				half blendFractor = saturate((posGradRotated - _GradientYStartPos) / _GradientHeight);

                #ifndef FRONT_GRADIENT_ON
                    _FrontBottomColor = _BottomColor;
                    _FrontTopColor = _TopColor;
                #endif
                #ifndef BACK_GRADIENT_ON
                    _BackBottomColor = _BottomColor;
                    _BackTopColor = _TopColor;
                #endif
                #ifndef LEFT_GRADIENT_ON
                    _LeftBottomColor = _BottomColor;
                    _LeftTopColor = _TopColor;
                #endif
                #ifndef RIGHT_GRADIENT_ON
                    _RightBottomColor = _BottomColor;
                    _RightTopColor = _TopColor;
                #endif

				half3 colorFront = lerp(_FrontBottomColor, _FrontTopColor, blendFractor);
				half3 colorBack = lerp(_BackBottomColor, _BackTopColor, blendFractor);
				half3 colorRight = lerp(_RightBottomColor, _RightTopColor, blendFractor);
				half3 colorLeft = lerp(_LeftBottomColor, _LeftTopColor, blendFractor);
                
                #if FOG_BOTTOM
                    // Remove directions with blendFractor.
                    dirFront =  lerp(1, dirFront, blendFractor);
                    dirBack =   lerp(1, dirBack, blendFractor);
                    dirRight =  lerp(1, dirRight, blendFractor);
                    dirLeft =   lerp(1, dirLeft, blendFractor);
                    dirUp =     lerp(1, dirUp, blendFractor);
                    dirDown =   lerp(1, dirDown, blendFractor);
                #endif
                
                half3 finalFront = normal.z > 0? (colorFront * dirFront) : (colorBack * dirBack);
                half3 finalRight = normal.x > 0? (colorRight * dirRight) : (colorLeft * dirLeft);
                half3 finalTop = normal.y > 0? (_TopColor * dirUp) : (_BottomColor * dirDown);
                
                #if !INDEPENDENT_SIDES
                    o.lighting = finalFront + finalRight + finalTop;
                #else
                    if (abs(normal.x) > abs(normal.y) && abs(normal.x) > abs(normal.z))
                    {
                        o.lighting = finalRight;
                    }
                    else if (abs(normal.y) > abs(normal.z))
                    {
                        o.lighting = finalTop;
                    }
                    else
                    {
                        o.lighting = finalFront;
                    }
                #endif
                
				#if LIGHTMAP_COLR_ON
					o.lightmap_uv = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#else
					o.lightmap_uv = float2(0, 0);
				#endif

				// Transfer realtime shadows
				TRANSFER_SHADOW(o);

				UNITY_TRANSFER_FOG(o, o.pos);

				o.lighting += _AmbientColor * _AmbientPower;
				
				return o;
			}

			fixed4 frag(v2f v) : COLOR {
				half4 result = VEC4_ZERO;
				half4 lightColor = float4(_LightTint * v.lighting, 1);

                #if LIGHTMAP_COLR_ON && LIGHTMAP_ON
                    half4 lmColor = UNITY_SAMPLE_TEX2D(unity_Lightmap, v.lightmap_uv) * SHADOW_ATTENUATION(v);
                    half4 dlm = half4(DecodeLightmap(lmColor), 0);
                    half4 lmPower = lerp(VEC4_ONE, dlm, _LightmapPower);
                    result = lerp(half4(_LightmapColor, 0), VEC4_ONE, lmPower) * lightColor;
                #else
                    result = lightColor * SHADOW_ATTENUATION(v);
                #endif

				result.rgb *= v.color * tex2D(_MainTex, v.uv);

                UNITY_APPLY_FOG(v.fogCoord, result);

			  	return result;
			}
			ENDCG
		}
	}

	FallBack "VertexLit"
    CustomEditor "MasterShaderEditor"
}
