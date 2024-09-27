Shader "Custom/OceanFloor"
{
    Properties
    {
        _ShipPosition ("Ship Position", Vector) = (0, 0, 0, 0)  // Position of the ship
        _Radius ("Radius", Float) = 5.0                         // Radius of lighting influence
        _FalloffFactor ("Falloff Factor", Float) = 2.0          // Control the softness of the light roll-off
        _EmissionColor ("Emission Color", Color) = (1, 1, 1, 1) // Color for lit areas
        _MainTex ("Base Texture", 2D) = "white" {}              // Main texture for the seabed
        _NormalMap ("Normal Map", 2D) = "bump" {}               // Normal map for surface details
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1) // Specular color for reflections
        _Glossiness ("Glossiness", Range(0.0, 1.0)) = 0.5       // Glossiness for specular reflections
        _LightColor ("Light Color", Color) = (1, 1, 1, 1)       // Color of the directional light
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            float4 _ShipPosition;
            float _Radius;
            float _FalloffFactor;
            float4 _EmissionColor;
            float _Glossiness;
            float4 _SpecularColor;
            float4 _LightColor;

            sampler2D _MainTex;
            sampler2D _NormalMap;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // Convert object space to world space
                o.worldNormal = mul((float3x3)unity_ObjectToWorld, v.normal); // Convert normals to world space
                o.uv = v.uv; // Pass UVs for texture sampling
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Sample the normal map and transform it from tangent space to world space
                half3 normalTangent = UnpackNormal(tex2D(_NormalMap, i.uv));
                half3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, normalTangent));

                // Sample the base texture
                half3 baseColor = tex2D(_MainTex, i.uv).rgb;

                // Simple Lambertian diffuse lighting
                half3 lightDir = normalize(_WorldSpaceLightPos0.xyz); // Direction of the main light
                half diffuse = max(dot(worldNormal, lightDir), 0.0);  // Lambert's cosine law

                // Add a basic specular highlight
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                half3 reflectDir = reflect(-lightDir, worldNormal);
                half specular = pow(max(dot(viewDir, reflectDir), 0.0), _Glossiness * 128.0);

                // Calculate distance from the ship to the current fragment (seabed pixel)
                float distanceToShip = distance(i.worldPos, _ShipPosition.xyz);

                // Normalize the distance into a 0-1 range using the radius and apply soft falloff
                float intensity = pow(saturate(1.0 - distanceToShip / _Radius), _FalloffFactor);

                // Combine emission, diffuse, and specular terms
                half3 finalColor = baseColor * diffuse + _EmissionColor.rgb * intensity;
                finalColor += specular * _SpecularColor.rgb; // Add specular reflection

                return half4(finalColor, 1.0); // Output the final color
            }
            ENDCG
        }
    }
}
