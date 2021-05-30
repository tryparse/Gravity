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
float2 LightWorldPosition;
float LightRadius;
float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
	float4 Position: SV_POSITION;
    float2 TexCoords : TEXCOORD0;
};

struct PixelShaderInput
{
    float4 Position: SV_POSITION;
	float4 WorldPosition: POSITION1;
    float2 TexCoords : TEXCOORD0;
};

PixelShaderInput VertexShaderFunction(VertexShaderInput input)
{
	PixelShaderInput result;

	float4 worldPosition = mul(input.Position, World);

	result.Position = mul(worldPosition, Projection);
	result.WorldPosition = worldPosition;
	result.TexCoords = input.TexCoords;

	return result;
}

float4 PixelShaderLight(PixelShaderInput input) : COLOR
{ 
	float4 result = float4(0,0,0,1);
	float4 shadowMap = tex2D(ShadowMapSampler, input.TexCoords);
	float distanceFromLight = distance(LightWorldPosition, input.WorldPosition.xy);
	float4 lightPixel = float4(0,0,0,0);
	// float4 ambient = float4(0.5, 0.5, 0.5, 0.5);

	if (distanceFromLight <= LightRadius)
	{
		float intensity = 1.1 - (distanceFromLight / LightRadius);
		lightPixel.rgb = LightColor.rgb * intensity;
		lightPixel.a = intensity;
	}

	result.rgb = shadowMap.rgb * lightPixel.rgb;
	result.a = lightPixel.a;
	// if (input.TexCoords.x > 0.5)
	// {
	// 	return float4(1,0,0,1);
	// }
	// else
	// {
	// 	return float4(0,1,0,1);
	// }

	return result;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderLight();
	}
};