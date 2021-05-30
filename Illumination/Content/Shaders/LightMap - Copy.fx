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

struct VertexShaderOutput
{
    float4 Position: SV_POSITION;
    float2 PosWorld: POSITION1;
    float2 TexCoords : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);

	output.Position = mul(viewPosition, Projection);
	output.PosWorld = worldPosition.xy;
    output.TexCoords = input.TexCoords;

    return output;
}

float4 PixelShaderLight(VertexShaderOutput input) : COLOR
{ 
	float4 result = float4(0,0,0,0);
	float4 shadowMap = tex2D(ShadowMapSampler, input.TexCoords);
	float distanceFromLight = distance(LightWorldPosition, input.PosWorld);
	float4 lightPixel = float4(0,0,0,0);
	float4 ambient = float4(0.5, 0.5, 0.5, 0.5);

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
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderLight();
	}
};