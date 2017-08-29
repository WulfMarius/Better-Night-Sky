Shader "Custom/Moon"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
		_TintColor ("Tint Color", Color) = (0.2, 0.3, 1 ,1)
	}

	SubShader
	{
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		Cull Off
		Lighting Off
		ZWrite On
		ColorMask A
		Pass
		{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
     
            #include "UnityCG.cginc"
     
            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
     
            struct v2f {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
     
            sampler2D _MaskTex;
     
            uniform float4 _MainTex_ST;
               
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
     
            fixed4 frag (v2f i) : COLOR
            {
                fixed4 texColor = tex2D(_MaskTex, i.texcoord);
				clip(texColor - 0.5);
				return texColor;
            }
            ENDCG
		}

		Cull Off
		Lighting Off
		ZWrite On
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha
        Pass {  
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
     
            #include "UnityCG.cginc"
     
            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
     
            struct v2f {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
     
            sampler2D _MainTex;
            fixed4 _TintColor;
     
            uniform float4 _MainTex_ST;
               
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
     
            fixed4 frag (v2f i) : COLOR
            {
                fixed4 texColor = tex2D(_MainTex, i.texcoord);
				clip(texColor - 0.1);
				return texColor * _TintColor;
            }
            ENDCG
        }	
	}
}
