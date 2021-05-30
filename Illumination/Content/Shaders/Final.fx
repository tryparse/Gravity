#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D ColorMapTexture;
sampler2D ColorMapSampler
{
	Texture = <ColorMapTexture>;
};

Texture2D LightMapTexture;
sampler2D LightMapSampler
{
	Texture = <LightMapTexture>;
};

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

bool IsTransparentPixel(float4 pixel)
{
	return pixel.a == 0.0
		&& pixel.r == 0.0
		&& pixel.g == 0.0
		&& pixel.b == 0.0;
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
	float4 colorMap = tex2D(ColorMapSampler, input.TexCoords);
	float4 lightMap = tex2D(LightMapSampler, input.TexCoords);

	// if (IsTransparentPixel(lightMap))
	// {
	// 	// colorMap.rgb *= AmbientColor.rgb;
	// }
	// else 
	// {
		colorMap.rgb *= (lightMap.rgb);
	// }

	return colorMap;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderLight();
	}
};