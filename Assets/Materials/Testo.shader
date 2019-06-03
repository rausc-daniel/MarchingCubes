Shader "Unlit/Testo"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_StartColor("Start Color", Color) = (1,0,0,0)
		_EndColor("End Color", Color) = (0,0,1,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
		Cull off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _StartColor;
			float4 _EndColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = v.normal;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				col *= acos(dot(i.normal, float4(0, 1, 0, 0))) / (3.14159265359 * 2);
                return col;
            }
            ENDCG
        }
    }
}
