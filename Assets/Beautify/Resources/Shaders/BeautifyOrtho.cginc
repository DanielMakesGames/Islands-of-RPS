#ifndef __BEAUTIFY_ORTHO
#define __BEAUTIFY_ORTHO

// Fix for Oculus GO & Stereo Rendering
#if SHADER_API_MOBILE && (defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED))
    //#define BEAUTIFY_GET_DEPTH_01(x) saturate(Linear01Depth(x))
    //#define BEAUTIFY_GET_DEPTH_EYE(x) clamp(LinearEyeDepth(x), 0, _ProjectionParams.z)
    
    #define BEAUTIFY_DEPTH_LOD(tex,uv) UNITY_SAMPLE_TEX2DARRAY_LOD(tex, float3((uv).xy, 0), (uv).w).r
    #define BEAUTIFY_DEPTH(tex,uv) UNITY_SAMPLE_TEX2DARRAY(tex, float3((uv).x, (uv).y, 0)).r
#else
    #define BEAUTIFY_DEPTH_LOD(tex,uv) SAMPLE_DEPTH_TEXTURE_LOD(tex, uv)
    #define BEAUTIFY_DEPTH(tex,uv) SAMPLE_DEPTH_TEXTURE(tex, uv)

    //#define BEAUTIFY_GET_DEPTH_01(x) Linear01Depth(x)
    //#define BEAUTIFY_GET_DEPTH_EYE(x) LinearEyeDepth(x)
#endif

#if defined(BEAUTIFY_ORTHO)
    #if UNITY_REVERSED_Z
  		#define Linear01Depth(x) (1.0-(x))
       	#define LinearEyeDepth(x) ((1.0-(x)) * _ProjectionParams.z)
    #else
	   	#define Linear01Depth(x) (x)
   		#define LinearEyeDepth(x) ((x) * _ProjectionParams.z)
    #endif
#endif

#endif // __BEAUTIFY_ORTHO