#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0 // This was ps_4_0_level_9_1, do that for DirectX

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = &lt;SpriteTexture&gt;;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

// I don't understand the stuff before here
float4 MainPS(VertexShaderOutput input) : COLOR
{
	// Applying our cool effect. What it does is: when drawing pixel X:Y, instead of taking the
	// pixel from texture position X:Y, take it from (X+Y*0.2:Y) to create a slanted effected
	float2 tex2; // I am using a temp var because I don't know if we can/should modify input.TC
	tex2[0] = input.TextureCoordinates[0] - input.TextureCoordinates[1] * 0.2f;
	tex2[1] = input.TextureCoordinates[1];

    return tex2D(SpriteTextureSampler,tex2) * input.Color;
}
// Here comes the rest of the things I don't understand
technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};