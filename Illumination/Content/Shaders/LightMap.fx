#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D ShadowMapTexture;
sampler2D ShadowMapSampler
{
	Texture = <ShadowMapTexture>;
};

float4 LightColor;
float2 LightScreenPosition;
float LightRadius;

struct PixelShaderInput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 PixelShaderLight(PixelShaderInput input) : COLOR
{ 
	float4 result = float4(0,0,0,0);
	float4 shadowMap = tex2D(ShadowMapSampler, input.TextureCoordinates);
	float distanceFromLight = distance(LightScreenPosition, input.TextureCoordinates);
	float4 lightPixel = float4(0,0,0,0);

	if (distanceFromLight <= LightRadius)
	{
		float intensity = 1.1 - (distanceFromLight / LightRadius);
		lightPixel.rgb = LightColor.rgb * intensity;
		lightPixel.a = intensity;
	}

	result.rgb = shadowMap.rgb * lightPixel.rgb;
	result.a = lightPixel.a;

	return result;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL PixelShaderLight();
	}
};