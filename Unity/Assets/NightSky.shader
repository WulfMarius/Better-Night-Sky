Shader "Custom/NightSky"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_TintColor ("Tint Color", Color) = (1, 1, 1, 1)
		_Scale ("Scale", Float) = 1
	}

	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "ForceNoShadowCasting"="true" "RenderType"="Transparent"}

		Cull Back 
		Lighting Off
		ZWrite On
		ColorMask RGBA
		Blend SrcAlpha One

        Pass {  
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
     
            #include "UnityCG.cginc"
     
            struct vertexInput {
                float4 position : POSITION;
                float2 texCoord : TEXCOORD0;
            };
     
            struct vertexOutput {
                float4 position : POSITION;
                float2 texCoord : TEXCOORD0;
            };
     
			sampler2D _MainTex;
            float4  _TintColor;
			float _Scale;
               
            vertexOutput vert (vertexInput input)
            {
                vertexOutput output;
                output.position = UnityObjectToClipPos(input.position);
				output.texCoord = input.texCoord;
                return output;
            }
     
            float4 frag (vertexOutput output) : COLOR
            {
                fixed4 texColor = tex2D(_MainTex, output.texCoord);
                return _Scale * texColor * _TintColor;
            }
            ENDCG
        }	
	}
}
