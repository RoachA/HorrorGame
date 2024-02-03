// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PostProcessing/FogVolume"
{

    Properties
    {
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		_FogColor("FogColor", Color) = (0,0,0,0)
		_DistanceFade("Distance Fade", Vector) = (0,1,0,0)
		_FogTexture("Fog Texture", 2D) = "white" {}
		_PanTime("Pan Time", Float) = 0
		_NoiseDefinition("Noise Definition", Vector) = (0,0,0,0)
		_ParallaxOffset("Parallax Offset", Float) = 1
		_SurfaceScale("Surface Scale", Float) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_LitColor("Lit Color", Color) = (0,0,0,0)

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }

    SubShader
    {
		LOD 0

		
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "UniversalMaterialType"="Lit" "Queue"="Transparent" "ShaderGraphShader"="true" }

		Cull Back
		Blend SrcAlpha One, SrcAlpha One
		ZTest LEqual
		ZWrite Off
		Offset 0 , 0
		ColorMask RGBA
		

		HLSLINCLUDE
		#pragma target 2.0
		#pragma prefer_hlslcc gles
		#pragma only_renderers d3d11 glcore gles gles3 metal // ensure rendering platforms toggle list is visible
		ENDHLSL

		
        Pass
        {
			
            Name "Sprite Lit"
            Tags { "LightMode"="Universal2D" }

            HLSLPROGRAM

			#define ASE_SRP_VERSION 120109
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define VARYINGS_NEED_SCREENPOSITION

            #define SHADERPASS SHADERPASS_SPRITELIT

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_TANGENT
			#define ASE_NEEDS_VERT_NORMAL


			struct VertexInput
			{
				float3 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float3 positionWS : TEXCOORD1;
				float4 color : TEXCOORD2;
				float4 screenPosition : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

            struct SurfaceDescription
			{
				float3 BaseColor;
				float Alpha;
			};

			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"

			half4 _RendererColor;

			sampler2D _TextureSample0;
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _FogTexture;
			CBUFFER_START( UnityPerMaterial )
			float4 _FogColor;
			float4 _LitColor;
			float2 _DistanceFade;
			float2 _NoiseDefinition;
			float _PanTime;
			float _ParallaxOffset;
			float _SurfaceScale;
			CBUFFER_END


			inline float2 ParallaxOffset( half h, half height, half3 viewDir )
			{
				h = h * height - height/2.0;
				float3 v = normalize( viewDir );
				v.z += 0.42;
				return h* (v.xy / v.z);
			}
			

			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord4 = screenPos;
				float3 ase_worldTangent = TransformObjectToWorldDir(v.tangent.xyz);
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.normal);
				float ase_vertexTangentSign = v.tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_worldPos = TransformObjectToWorld( (v.vertex).xyz );
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_tanViewDir =  tanToWorld0 * ase_worldViewDir.x + tanToWorld1 * ase_worldViewDir.y  + tanToWorld2 * ase_worldViewDir.z;
				ase_tanViewDir = SafeNormalize( ase_tanViewDir );
				float2 paralaxOffset87 = ParallaxOffset( 1 , _ParallaxOffset , ase_tanViewDir );
				float2 temp_cast_0 = (( 0.5 * ( 1.0 - _SurfaceScale ) )).xx;
				float2 texCoord78 = v.uv0.xy * temp_cast_0 + float2( 0,0 );
				float2 vertexToFrag94 = ( paralaxOffset87 + texCoord78 );
				o.ase_texcoord5.xy = vertexToFrag94;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord5.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);

				o.positionCS = vertexInput.positionCS;
				o.positionWS.xyz =  vertexInput.positionWS;
				o.texCoord0.xyzw =  v.uv0;
				o.color.xyzw =  v.color;
				o.screenPosition.xyzw =  vertexInput.positionNDC;

				return o;
			}

			half4 frag( VertexOutput IN   ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				float4 fog_color_107 = _FogColor;
				float4 tex2DNode104 = tex2D( _TextureSample0, IN.screenPosition.xy );
				float4 lerpResult106 = lerp( fog_color_107 , _LitColor , tex2DNode104);
				
				float smoothstepResult41 = smoothstep( _DistanceFade.x , _DistanceFade.y , distance( _WorldSpaceCameraPos , IN.positionWS ));
				float eyeDepth55 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( IN.screenPosition.xy ),_ZBufferParams);
				float4 screenPos = IN.ase_texcoord4;
				float4 temp_cast_2 = (_NoiseDefinition.x).xxxx;
				float4 temp_cast_3 = (_NoiseDefinition.y).xxxx;
				float mulTime84 = _TimeParameters.x * _PanTime;
				float2 vertexToFrag94 = IN.ase_texcoord5.xy;
				float2 temp_output_2_0_g3 = vertexToFrag94;
				float2 temp_cast_4 = (-3.0).xx;
				float2 temp_output_11_0_g3 = ( temp_output_2_0_g3 - temp_cast_4 );
				float dotResult12_g3 = dot( temp_output_11_0_g3 , temp_output_11_0_g3 );
				float2 temp_cast_5 = (0.0).xx;
				float2 panner82 = ( mulTime84 * float2( 1,0 ) + ( temp_output_2_0_g3 + ( temp_output_11_0_g3 * ( dotResult12_g3 * dotResult12_g3 * temp_cast_5 ) ) + float2( 0,0 ) ));
				float2 texCoord76 = IN.texCoord0.xy * float2( 3,3 ) + float2( 0,0 );
				float2 panner81 = ( mulTime84 * float2( -1,0 ) + texCoord76);
				float4 lightMask113 = tex2DNode104;
				float4 smoothstepResult85 = smoothstep( temp_cast_2 , temp_cast_3 , saturate( ( tex2D( _FogTexture, panner82 ).b + ( tex2D( _FogTexture, panner81 ).g + lightMask113 ) ) ));
				float lerpResult67 = lerp( 0.0 , ( saturate( smoothstepResult41 ) * saturate( ( _FogColor.a * ( eyeDepth55 - screenPos.w ) ) ) ) , smoothstepResult85.r);
				
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;
				surfaceDescription.BaseColor = lerpResult106.rgb;
				surfaceDescription.Alpha = lerpResult67;

				half4 color = half4(surfaceDescription.BaseColor, surfaceDescription.Alpha);

				#if defined(DEBUG_DISPLAY)
				SurfaceData2D surfaceData;
				InitializeSurfaceData(color.rgb, color.a, surfaceData);
				InputData2D inputData;
				InitializeInputData(IN.positionWS.xy, half2(IN.texCoord0.xy), inputData);
				half4 debugColor = 0;

				SETUP_DEBUG_DATA_2D(inputData, IN.positionWS);

				if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
				{
					return debugColor;
				}
				#endif

				color *= IN.color * _RendererColor;
				return color;
			}

            ENDHLSL
        }

		
        Pass
        {
			
            Name "Sprite Normal"
            Tags { "LightMode"="NormalsRendering" }

            HLSLPROGRAM

			#define ASE_SRP_VERSION 120109
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS

            #define SHADERPASS SHADERPASS_SPRITENORMAL

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_TANGENT
			#define ASE_NEEDS_VERT_NORMAL


			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _FogTexture;
			sampler2D _TextureSample0;
			CBUFFER_START( UnityPerMaterial )
			float4 _FogColor;
			float4 _LitColor;
			float2 _DistanceFade;
			float2 _NoiseDefinition;
			float _PanTime;
			float _ParallaxOffset;
			float _SurfaceScale;
			CBUFFER_END


			struct VertexInput
			{
				float3 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float3 normalWS : TEXCOORD0;
				float4 tangentWS : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

            struct SurfaceDescription
			{
				float3 NormalTS;
				float Alpha;
			};

			inline float2 ParallaxOffset( half h, half height, half3 viewDir )
			{
				h = h * height - height/2.0;
				float3 v = normalize( viewDir );
				v.z += 0.42;
				return h* (v.xy / v.z);
			}
			

			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_worldPos = TransformObjectToWorld( (v.vertex).xyz );
				o.ase_texcoord2.xyz = ase_worldPos;
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord3 = screenPos;
				float3 ase_worldTangent = TransformObjectToWorldDir(v.tangent.xyz);
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.normal);
				float ase_vertexTangentSign = v.tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_tanViewDir =  tanToWorld0 * ase_worldViewDir.x + tanToWorld1 * ase_worldViewDir.y  + tanToWorld2 * ase_worldViewDir.z;
				ase_tanViewDir = SafeNormalize( ase_tanViewDir );
				float2 paralaxOffset87 = ParallaxOffset( 1 , _ParallaxOffset , ase_tanViewDir );
				float2 temp_cast_0 = (( 0.5 * ( 1.0 - _SurfaceScale ) )).xx;
				float2 texCoord78 = v.ase_texcoord.xy * temp_cast_0 + float2( 0,0 );
				float2 vertexToFrag94 = ( paralaxOffset87 + texCoord78 );
				o.ase_texcoord4.xy = vertexToFrag94;
				
				o.ase_texcoord4.zw = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;


				float3 positionWS = TransformObjectToWorld(v.vertex);
				float4 tangentWS = float4(TransformObjectToWorldDir(v.tangent.xyz), v.tangent.w);

				o.positionCS = TransformWorldToHClip(positionWS);
				o.normalWS.xyz =  -GetViewForwardDir();
				o.tangentWS.xyzw =  tangentWS;
				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				float3 ase_worldPos = IN.ase_texcoord2.xyz;
				float smoothstepResult41 = smoothstep( _DistanceFade.x , _DistanceFade.y , distance( _WorldSpaceCameraPos , ase_worldPos ));
				float4 screenPos = IN.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float eyeDepth55 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float4 temp_cast_0 = (_NoiseDefinition.x).xxxx;
				float4 temp_cast_1 = (_NoiseDefinition.y).xxxx;
				float mulTime84 = _TimeParameters.x * _PanTime;
				float2 vertexToFrag94 = IN.ase_texcoord4.xy;
				float2 temp_output_2_0_g3 = vertexToFrag94;
				float2 temp_cast_2 = (-3.0).xx;
				float2 temp_output_11_0_g3 = ( temp_output_2_0_g3 - temp_cast_2 );
				float dotResult12_g3 = dot( temp_output_11_0_g3 , temp_output_11_0_g3 );
				float2 temp_cast_3 = (0.0).xx;
				float2 panner82 = ( mulTime84 * float2( 1,0 ) + ( temp_output_2_0_g3 + ( temp_output_11_0_g3 * ( dotResult12_g3 * dotResult12_g3 * temp_cast_3 ) ) + float2( 0,0 ) ));
				float2 texCoord76 = IN.ase_texcoord4.zw * float2( 3,3 ) + float2( 0,0 );
				float2 panner81 = ( mulTime84 * float2( -1,0 ) + texCoord76);
				float4 tex2DNode104 = tex2D( _TextureSample0, ase_screenPosNorm.xy );
				float4 lightMask113 = tex2DNode104;
				float4 smoothstepResult85 = smoothstep( temp_cast_0 , temp_cast_1 , saturate( ( tex2D( _FogTexture, panner82 ).b + ( tex2D( _FogTexture, panner81 ).g + lightMask113 ) ) ));
				float lerpResult67 = lerp( 0.0 , ( saturate( smoothstepResult41 ) * saturate( ( _FogColor.a * ( eyeDepth55 - screenPos.w ) ) ) ) , smoothstepResult85.r);
				
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;
				surfaceDescription.NormalTS = float3(0.0f, 0.0f, 1.0f);
				surfaceDescription.Alpha = lerpResult67;

				half crossSign = (IN.tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
				half3 bitangent = crossSign * cross(IN.normalWS.xyz, IN.tangentWS.xyz);
				half4 color = half4(1.0,1.0,1.0, surfaceDescription.Alpha);

				return NormalsRenderingShared(color, surfaceDescription.NormalTS, IN.tangentWS.xyz, bitangent, IN.normalWS);
			}

            ENDHLSL
        }

		
        Pass
        {
			
            Name "SceneSelectionPass"
            Tags { "LightMode"="SceneSelectionPass" }

            Cull Off
			Blend Off
			ZTest LEqual
			ZWrite On

            HLSLPROGRAM

			#define ASE_SRP_VERSION 120109
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT

            #define SHADERPASS SHADERPASS_DEPTHONLY
	        #define SCENESELECTIONPASS 1


            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_TANGENT
			#define ASE_NEEDS_VERT_NORMAL


			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _FogTexture;
			sampler2D _TextureSample0;
			CBUFFER_START( UnityPerMaterial )
			float4 _FogColor;
			float4 _LitColor;
			float2 _DistanceFade;
			float2 _NoiseDefinition;
			float _PanTime;
			float _ParallaxOffset;
			float _SurfaceScale;
			CBUFFER_END


            struct VertexInput
			{
				float3 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};


			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

            int _ObjectId;
            int _PassValue;

            struct SurfaceDescription
			{
				float Alpha;
			};

			inline float2 ParallaxOffset( half h, half height, half3 viewDir )
			{
				h = h * height - height/2.0;
				float3 v = normalize( viewDir );
				v.z += 0.42;
				return h* (v.xy / v.z);
			}
			

			VertexOutput vert( VertexInput v )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_worldPos = TransformObjectToWorld( (v.vertex).xyz );
				o.ase_texcoord.xyz = ase_worldPos;
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord1 = screenPos;
				float3 ase_worldTangent = TransformObjectToWorldDir(v.tangent.xyz);
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.normal);
				float ase_vertexTangentSign = v.tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_tanViewDir =  tanToWorld0 * ase_worldViewDir.x + tanToWorld1 * ase_worldViewDir.y  + tanToWorld2 * ase_worldViewDir.z;
				ase_tanViewDir = SafeNormalize( ase_tanViewDir );
				float2 paralaxOffset87 = ParallaxOffset( 1 , _ParallaxOffset , ase_tanViewDir );
				float2 temp_cast_0 = (( 0.5 * ( 1.0 - _SurfaceScale ) )).xx;
				float2 texCoord78 = v.ase_texcoord.xy * temp_cast_0 + float2( 0,0 );
				float2 vertexToFrag94 = ( paralaxOffset87 + texCoord78 );
				o.ase_texcoord2.xy = vertexToFrag94;
				
				o.ase_texcoord2.zw = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif


				float3 positionWS = TransformObjectToWorld(v.vertex);
				o.positionCS = TransformWorldToHClip(positionWS);
				return o;
			}

			half4 frag( VertexOutput IN ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				float3 ase_worldPos = IN.ase_texcoord.xyz;
				float smoothstepResult41 = smoothstep( _DistanceFade.x , _DistanceFade.y , distance( _WorldSpaceCameraPos , ase_worldPos ));
				float4 screenPos = IN.ase_texcoord1;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float eyeDepth55 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float4 temp_cast_0 = (_NoiseDefinition.x).xxxx;
				float4 temp_cast_1 = (_NoiseDefinition.y).xxxx;
				float mulTime84 = _TimeParameters.x * _PanTime;
				float2 vertexToFrag94 = IN.ase_texcoord2.xy;
				float2 temp_output_2_0_g3 = vertexToFrag94;
				float2 temp_cast_2 = (-3.0).xx;
				float2 temp_output_11_0_g3 = ( temp_output_2_0_g3 - temp_cast_2 );
				float dotResult12_g3 = dot( temp_output_11_0_g3 , temp_output_11_0_g3 );
				float2 temp_cast_3 = (0.0).xx;
				float2 panner82 = ( mulTime84 * float2( 1,0 ) + ( temp_output_2_0_g3 + ( temp_output_11_0_g3 * ( dotResult12_g3 * dotResult12_g3 * temp_cast_3 ) ) + float2( 0,0 ) ));
				float2 texCoord76 = IN.ase_texcoord2.zw * float2( 3,3 ) + float2( 0,0 );
				float2 panner81 = ( mulTime84 * float2( -1,0 ) + texCoord76);
				float4 tex2DNode104 = tex2D( _TextureSample0, ase_screenPosNorm.xy );
				float4 lightMask113 = tex2DNode104;
				float4 smoothstepResult85 = smoothstep( temp_cast_0 , temp_cast_1 , saturate( ( tex2D( _FogTexture, panner82 ).b + ( tex2D( _FogTexture, panner81 ).g + lightMask113 ) ) ));
				float lerpResult67 = lerp( 0.0 , ( saturate( smoothstepResult41 ) * saturate( ( _FogColor.a * ( eyeDepth55 - screenPos.w ) ) ) ) , smoothstepResult85.r);
				
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;
				surfaceDescription.Alpha = lerpResult67;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}

            ENDHLSL
        }

		
        Pass
        {
			
            Name "ScenePickingPass"
            Tags { "LightMode"="Picking" }

            Cull Off
			Blend Off
			ZTest LEqual
			ZWrite On


            HLSLPROGRAM

			#define ASE_SRP_VERSION 120109
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT

            #define SHADERPASS SHADERPASS_DEPTHONLY
			#define SCENEPICKINGPASS 1

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        	#define ASE_NEEDS_VERT_TANGENT
        	#define ASE_NEEDS_VERT_NORMAL


			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _FogTexture;
			sampler2D _TextureSample0;
			CBUFFER_START( UnityPerMaterial )
			float4 _FogColor;
			float4 _LitColor;
			float2 _DistanceFade;
			float2 _NoiseDefinition;
			float _PanTime;
			float _ParallaxOffset;
			float _SurfaceScale;
			CBUFFER_END


            struct VertexInput
			{
				float3 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

            float4 _SelectionID;

            struct SurfaceDescription
			{
				float Alpha;
			};

   			inline float2 ParallaxOffset( half h, half height, half3 viewDir )
   			{
   				h = h * height - height/2.0;
   				float3 v = normalize( viewDir );
   				v.z += 0.42;
   				return h* (v.xy / v.z);
   			}
   			

			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_worldPos = TransformObjectToWorld( (v.vertex).xyz );
				o.ase_texcoord.xyz = ase_worldPos;
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord1 = screenPos;
				float3 ase_worldTangent = TransformObjectToWorldDir(v.tangent.xyz);
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.normal);
				float ase_vertexTangentSign = v.tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_tanViewDir =  tanToWorld0 * ase_worldViewDir.x + tanToWorld1 * ase_worldViewDir.y  + tanToWorld2 * ase_worldViewDir.z;
				ase_tanViewDir = SafeNormalize( ase_tanViewDir );
				float2 paralaxOffset87 = ParallaxOffset( 1 , _ParallaxOffset , ase_tanViewDir );
				float2 temp_cast_0 = (( 0.5 * ( 1.0 - _SurfaceScale ) )).xx;
				float2 texCoord78 = v.ase_texcoord.xy * temp_cast_0 + float2( 0,0 );
				float2 vertexToFrag94 = ( paralaxOffset87 + texCoord78 );
				o.ase_texcoord2.xy = vertexToFrag94;
				
				o.ase_texcoord2.zw = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				float3 positionWS = TransformObjectToWorld(v.vertex);
				o.positionCS = TransformWorldToHClip(positionWS);

				return o;
			}

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				float3 ase_worldPos = IN.ase_texcoord.xyz;
				float smoothstepResult41 = smoothstep( _DistanceFade.x , _DistanceFade.y , distance( _WorldSpaceCameraPos , ase_worldPos ));
				float4 screenPos = IN.ase_texcoord1;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float eyeDepth55 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float4 temp_cast_0 = (_NoiseDefinition.x).xxxx;
				float4 temp_cast_1 = (_NoiseDefinition.y).xxxx;
				float mulTime84 = _TimeParameters.x * _PanTime;
				float2 vertexToFrag94 = IN.ase_texcoord2.xy;
				float2 temp_output_2_0_g3 = vertexToFrag94;
				float2 temp_cast_2 = (-3.0).xx;
				float2 temp_output_11_0_g3 = ( temp_output_2_0_g3 - temp_cast_2 );
				float dotResult12_g3 = dot( temp_output_11_0_g3 , temp_output_11_0_g3 );
				float2 temp_cast_3 = (0.0).xx;
				float2 panner82 = ( mulTime84 * float2( 1,0 ) + ( temp_output_2_0_g3 + ( temp_output_11_0_g3 * ( dotResult12_g3 * dotResult12_g3 * temp_cast_3 ) ) + float2( 0,0 ) ));
				float2 texCoord76 = IN.ase_texcoord2.zw * float2( 3,3 ) + float2( 0,0 );
				float2 panner81 = ( mulTime84 * float2( -1,0 ) + texCoord76);
				float4 tex2DNode104 = tex2D( _TextureSample0, ase_screenPosNorm.xy );
				float4 lightMask113 = tex2DNode104;
				float4 smoothstepResult85 = smoothstep( temp_cast_0 , temp_cast_1 , saturate( ( tex2D( _FogTexture, panner82 ).b + ( tex2D( _FogTexture, panner81 ).g + lightMask113 ) ) ));
				float lerpResult67 = lerp( 0.0 , ( saturate( smoothstepResult41 ) * saturate( ( _FogColor.a * ( eyeDepth55 - screenPos.w ) ) ) ) , smoothstepResult85.r);
				
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;
				surfaceDescription.Alpha = lerpResult67;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = _SelectionID;
				return outColor;
			}


            ENDHLSL
        }

		
        Pass
        {
			
            Name "Sprite Forward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM

			#define ASE_SRP_VERSION 120109
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR

            #define SHADERPASS SHADERPASS_SPRITEFORWARD

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_TANGENT
			#define ASE_NEEDS_VERT_NORMAL


			sampler2D _TextureSample0;
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _FogTexture;
			CBUFFER_START( UnityPerMaterial )
			float4 _FogColor;
			float4 _LitColor;
			float2 _DistanceFade;
			float2 _NoiseDefinition;
			float _PanTime;
			float _ParallaxOffset;
			float _SurfaceScale;
			CBUFFER_END


            struct VertexInput
			{
				float3 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};


			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float3 positionWS : TEXCOORD1;
				float4 color : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

            struct SurfaceDescription
			{
				float3 BaseColor;
				float Alpha;
				float3 NormalTS;
			};

			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"

			inline float2 ParallaxOffset( half h, half height, half3 viewDir )
			{
				h = h * height - height/2.0;
				float3 v = normalize( viewDir );
				v.z += 0.42;
				return h* (v.xy / v.z);
			}
			

			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);


				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord3 = screenPos;
				
				float3 ase_worldTangent = TransformObjectToWorldDir(v.tangent.xyz);
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.normal);
				float ase_vertexTangentSign = v.tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_worldPos = TransformObjectToWorld( (v.vertex).xyz );
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_tanViewDir =  tanToWorld0 * ase_worldViewDir.x + tanToWorld1 * ase_worldViewDir.y  + tanToWorld2 * ase_worldViewDir.z;
				ase_tanViewDir = SafeNormalize( ase_tanViewDir );
				float2 paralaxOffset87 = ParallaxOffset( 1 , _ParallaxOffset , ase_tanViewDir );
				float2 temp_cast_0 = (( 0.5 * ( 1.0 - _SurfaceScale ) )).xx;
				float2 texCoord78 = v.uv0.xy * temp_cast_0 + float2( 0,0 );
				float2 vertexToFrag94 = ( paralaxOffset87 + texCoord78 );
				o.ase_texcoord4.xy = vertexToFrag94;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord4.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;


				float3 positionWS = TransformObjectToWorld(v.vertex);

				o.positionCS = TransformWorldToHClip(positionWS);
				o.positionWS.xyz =  positionWS;
				o.texCoord0.xyzw =  v.uv0;
				o.color.xyzw =  v.color;

				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				float4 fog_color_107 = _FogColor;
				float4 screenPos = IN.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float4 tex2DNode104 = tex2D( _TextureSample0, ase_screenPosNorm.xy );
				float4 lerpResult106 = lerp( fog_color_107 , _LitColor , tex2DNode104);
				
				float smoothstepResult41 = smoothstep( _DistanceFade.x , _DistanceFade.y , distance( _WorldSpaceCameraPos , IN.positionWS ));
				float eyeDepth55 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float4 temp_cast_2 = (_NoiseDefinition.x).xxxx;
				float4 temp_cast_3 = (_NoiseDefinition.y).xxxx;
				float mulTime84 = _TimeParameters.x * _PanTime;
				float2 vertexToFrag94 = IN.ase_texcoord4.xy;
				float2 temp_output_2_0_g3 = vertexToFrag94;
				float2 temp_cast_4 = (-3.0).xx;
				float2 temp_output_11_0_g3 = ( temp_output_2_0_g3 - temp_cast_4 );
				float dotResult12_g3 = dot( temp_output_11_0_g3 , temp_output_11_0_g3 );
				float2 temp_cast_5 = (0.0).xx;
				float2 panner82 = ( mulTime84 * float2( 1,0 ) + ( temp_output_2_0_g3 + ( temp_output_11_0_g3 * ( dotResult12_g3 * dotResult12_g3 * temp_cast_5 ) ) + float2( 0,0 ) ));
				float2 texCoord76 = IN.texCoord0.xy * float2( 3,3 ) + float2( 0,0 );
				float2 panner81 = ( mulTime84 * float2( -1,0 ) + texCoord76);
				float4 lightMask113 = tex2DNode104;
				float4 smoothstepResult85 = smoothstep( temp_cast_2 , temp_cast_3 , saturate( ( tex2D( _FogTexture, panner82 ).b + ( tex2D( _FogTexture, panner81 ).g + lightMask113 ) ) ));
				float lerpResult67 = lerp( 0.0 , ( saturate( smoothstepResult41 ) * saturate( ( _FogColor.a * ( eyeDepth55 - screenPos.w ) ) ) ) , smoothstepResult85.r);
				
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;
				surfaceDescription.BaseColor = lerpResult106.rgb;
				surfaceDescription.NormalTS = float3(0.0f, 0.0f, 1.0f);
				surfaceDescription.Alpha = lerpResult67;


				half4 color = half4(surfaceDescription.BaseColor, surfaceDescription.Alpha);

				#if defined(DEBUG_DISPLAY)
				SurfaceData2D surfaceData;
				InitializeSurfaceData(color.rgb, color.a, surfaceData);
				InputData2D inputData;
				InitializeInputData(IN.positionWS.xy, half2(IN.texCoord0.xy), inputData);
				half4 debugColor = 0;

				SETUP_DEBUG_DATA_2D(inputData, IN.positionWS);

				if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
				{
					return debugColor;
				}
				#endif

				color *= IN.color;
				return color;
			}


            ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;25;920,-167;Float;False;False;-1;2;UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI;0;17;New Amplify Shader;ece0159bad6633944bf6b818f4dd296c;True;Sprite Lit;0;0;Sprite Lit;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;UniversalMaterialType=Lit;Queue=Transparent=Queue=0;ShaderGraphShader=true;True;0;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;26;920,-167;Float;False;False;-1;2;UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI;0;17;New Amplify Shader;ece0159bad6633944bf6b818f4dd296c;True;Sprite Normal;0;1;Sprite Normal;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;UniversalMaterialType=Lit;Queue=Transparent=Queue=0;ShaderGraphShader=true;True;0;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=NormalsRendering;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;27;920,-167;Float;False;False;-1;2;UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI;0;17;New Amplify Shader;ece0159bad6633944bf6b818f4dd296c;True;SceneSelectionPass;0;2;SceneSelectionPass;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;UniversalMaterialType=Lit;Queue=Transparent=Queue=0;ShaderGraphShader=true;True;0;True;12;all;0;False;True;0;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;28;920,-167;Float;False;False;-1;2;UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI;0;17;New Amplify Shader;ece0159bad6633944bf6b818f4dd296c;True;ScenePickingPass;0;3;ScenePickingPass;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;UniversalMaterialType=Lit;Queue=Transparent=Queue=0;ShaderGraphShader=true;True;0;True;12;all;0;False;True;0;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;1293.437,-130.2528;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;38;616.7024,-777.312;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;36;549.7024,-934.3118;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DistanceOpNode;35;844.7025,-844.312;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;41;1154.702,-642.312;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;42;889.7024,-588.312;Inherit;False;Property;_DistanceFade;Distance Fade;1;0;Create;True;0;0;0;False;0;False;0,1;3.69,-4.61;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SaturateNode;40;1462.193,-422.0361;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;1730.097,-286.6019;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;64;1252.598,-1306.202;Inherit;True;Property;_FogTexture;Fog Texture;2;0;Create;True;0;0;0;False;0;False;-1;1af1adb124539d540ac51142e293e869;1af1adb124539d540ac51142e293e869;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;84;885.274,-1117.901;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;83;747.274,-1181.901;Inherit;False;Property;_PanTime;Pan Time;3;0;Create;True;0;0;0;False;0;False;0;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;67;2290.494,-450.7766;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;91;251.8639,-1403.21;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;88;391.3745,-1915.018;Inherit;False;Property;_ParallaxOffset;Parallax Offset;5;0;Create;True;0;0;0;False;0;False;1;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;93;912.0032,-1635.603;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;89;432.3745,-1757.918;Inherit;False;Tangent;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ParallaxOffsetHlpNode;87;672.3745,-1838.018;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;85;2195.104,-815.0839;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;77;1748.18,-1436.071;Inherit;True;Property;_FogTexture1;Fog Texture;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;64;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexToFragmentNode;94;1131.303,-1653.103;Inherit;False;False;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;96;1470.511,-1741.64;Inherit;True;Spherize;-1;;3;1488bb72d8899174ba0601b595d32b07;0;4;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;98;1187.846,-1545.864;Inherit;False;Constant;_Float1;Float 1;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;1191.046,-1770.264;Inherit;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;0;False;0;False;-3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;56;671.7209,-46.84778;Float;False;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;101;994.223,-42.43927;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;103;1543.947,-183.7805;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;14;1042.754,-345.9165;Inherit;False;Property;_FogColor;FogColor;0;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.3711285,0.464346,0.5660378,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;107;1305.338,-305.9901;Inherit;False;fog color ;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;29;3363.49,-348.6543;Float;False;True;-1;2;UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI;0;14;PostProcessing/FogVolume;ece0159bad6633944bf6b818f4dd296c;True;Sprite Forward;0;4;Sprite Forward;6;True;True;8;5;False;;1;False;;8;5;False;;1;False;;False;False;False;False;False;False;False;False;False;False;False;True;True;0;False;;True;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;UniversalMaterialType=Lit;Queue=Transparent=Queue=0;ShaderGraphShader=true;True;0;True;5;d3d11;glcore;gles;gles3;metal;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=UniversalForward;False;False;0;;0;0;Standard;2;Vertex Position;1;0;Debug Display;0;0;0;5;True;True;True;True;True;False;;False;0
Node;AmplifyShaderEditor.GetLocalVarNode;108;2566.084,-328.6287;Inherit;False;107;fog color ;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenDepthNode;55;654.3428,-170.6352;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;113;2981.945,125.1406;Inherit;False;lightMask;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;80;2073.879,-961.7914;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;79;1897.88,-1131.791;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;86;1889.851,-781.7031;Inherit;False;Property;_NoiseDefinition;Noise Definition;4;0;Create;True;0;0;0;False;0;False;0,0;1.55,-0.55;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;76;742.2974,-1336.002;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;3,3;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;78;676.1796,-1511.591;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;5,5;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;90;-66.78729,-1497.464;Inherit;False;Property;_SurfaceScale;Surface Scale;6;0;Create;True;0;0;0;False;0;False;0;-15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;501.864,-1480.21;Inherit;False;2;2;0;FLOAT;0.5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;81;1010.274,-1250.901;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;82;1539.674,-1455.501;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;114;1680.328,-1108.879;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;115;1211.36,-1013.483;Inherit;False;113;lightMask;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;109;2480.376,-180.7524;Inherit;False;Property;_LitColor;Lit Color;8;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;106;2977.831,-213.8499;Inherit;True;3;0;COLOR;1,1,1,0;False;1;COLOR;0.7830189,0.7423906,0.7423906,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;104;2475.348,54.18268;Inherit;True;Property;_TextureSample0;Texture Sample 0;7;0;Create;True;0;0;0;False;0;False;-1;None;6051bfd509da20f4fa5fef77af17380a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenPosInputsNode;105;2186.919,75.57148;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;61;0;14;4
WireConnection;61;1;101;0
WireConnection;35;0;36;0
WireConnection;35;1;38;0
WireConnection;41;0;35;0
WireConnection;41;1;42;1
WireConnection;41;2;42;2
WireConnection;40;0;41;0
WireConnection;63;0;40;0
WireConnection;63;1;103;0
WireConnection;64;1;81;0
WireConnection;84;0;83;0
WireConnection;67;1;63;0
WireConnection;67;2;85;0
WireConnection;91;0;90;0
WireConnection;93;0;87;0
WireConnection;93;1;78;0
WireConnection;87;1;88;0
WireConnection;87;2;89;0
WireConnection;85;0;80;0
WireConnection;85;1;86;1
WireConnection;85;2;86;2
WireConnection;77;1;82;0
WireConnection;94;0;93;0
WireConnection;96;2;94;0
WireConnection;96;3;97;0
WireConnection;96;4;98;0
WireConnection;101;0;55;0
WireConnection;101;1;56;4
WireConnection;103;0;61;0
WireConnection;107;0;14;0
WireConnection;29;0;106;0
WireConnection;29;2;67;0
WireConnection;113;0;104;0
WireConnection;80;0;79;0
WireConnection;79;0;77;3
WireConnection;79;1;114;0
WireConnection;78;0;92;0
WireConnection;92;1;91;0
WireConnection;81;0;76;0
WireConnection;81;1;84;0
WireConnection;82;0;96;0
WireConnection;82;1;84;0
WireConnection;114;0;64;2
WireConnection;114;1;115;0
WireConnection;106;0;108;0
WireConnection;106;1;109;0
WireConnection;106;2;104;0
WireConnection;104;1;105;0
ASEEND*/
//CHKSM=0F66D976DF602627105A93A1BF89981CAF25230F