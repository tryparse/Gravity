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

float4 AmbientColor;

struct PixelShaderInput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

bool IsTransparentPixel(float4 pixel)
{
	return pixel.a == 0.0
		|| (pixel.r == 0.0
		&& pixel.g == 0.0
		&& pixel.b == 0.0);
};

float4 PixelShaderLight(PixelShaderInput input) : COLOR
{ 
	float4 colorMap = tex2D(ColorMapSampler, input.TextureCoordinates);
	float4 lightMap = tex2D(LightMapSampler, input.TextureCoordinates);

	if (IsTransparentPixel(colorMap))
	{
		colorMap = lightMap + AmbientColor;
	}
	else 
	{
		colorMap.rgb *= (AmbientColor.rgb + lightMap.rgb);
	}

	return colorMap;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL PixelShaderLight();
	}
};