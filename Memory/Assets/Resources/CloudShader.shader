Shader "Custom/CloudIdeaGlobal"
{
  Properties
  {
    _MainTex      ("Cloud RGB",        2D)    = "white" {}
    _EnableLight  ("Enable Light",     Range(0,1)) = 1
    _Intensity    ("Glow Intensity",   Range(0,5)) = 1
    _BlurRadius   ("Edge Blur Radius", Float)     = 2
  }
  SubShader
  {
    Tags { "RenderType"="Transparent" "Queue"="Transparent" }
    LOD 100
    Blend SrcAlpha OneMinusSrcAlpha

    Pass
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      sampler2D _MainTex;
      float4   _MainTex_TexelSize;
      float    _EnableLight, _Intensity, _BlurRadius;

      // THESE come from C# via Shader.SetGlobal*
      float3   _IdeaPosGlobal;
      float4   _IdeaColorGlobal; // rgb*intensity in .rgb, alpha unused
      float    _IdeaRangeGlobal;

      struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
      struct v2f
      {
        float2 uv      : TEXCOORD0;
        float4 pos     : SV_POSITION;
        float3 worldP  : TEXCOORD1;
      };

      v2f vert(appdata v)
      {
        v2f o;
        o.pos    = UnityObjectToClipPos(v.vertex);
        o.uv     = v.uv;
        o.worldP = mul(unity_ObjectToWorld, v.vertex).xyz;
        return o;
      }

      fixed4 frag(v2f i) : SV_Target
      {
        fixed3 rgb    = tex2D(_MainTex, i.uv).rgb;
        float bright  = dot(rgb, float3(0.3333,0.3333,0.3333));

        // blur‐based edge distance
        int   r      = max(1, (int)_BlurRadius);
        float2 ts    = _MainTex_TexelSize.xy;
        float sum    = 0; int cnt = 0;
        for(int y=-r; y<=r; y++)
          for(int x=-r; x<=r; x++)
          {
            float2 uv2 = i.uv + float2(x,y)*ts;
            sum += dot(tex2D(_MainTex, uv2).rgb, float3(0.3333,0.3333,0.3333));
            cnt++;
          }
        float avg     = sum/cnt;
        float edgeDist= saturate(1 - avg);

        // core mask
        float mask    = bright * edgeDist * _EnableLight * _Intensity;

        // idea‐light attenuation
        float d       = distance(i.worldP, _IdeaPosGlobal);
        float atten   = saturate(1 - d/_IdeaRangeGlobal);

        // final glow
        fixed3 glow   = rgb + mask * atten * _IdeaColorGlobal.rgb;
        return fixed4(saturate(glow), bright);
      }
      ENDCG
    }
  }
}
