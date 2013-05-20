uniform extern texture ScreenTexture;	

static const float PI = 3.14159265f;

sampler ScreenS = sampler_state
{
	Texture = <ScreenTexture>;	
};

float4 PixelShader(float2 texCoord: TEXCOORD0) : COLOR
{
	float2 sinOffset = float2 ( 0, sin ( 10 * texCoord.x )/50 );
	float4 color = tex2D(ScreenS, texCoord + sinOffset);
		
	return color;
}

technique
{
	pass P0
	{
		PixelShader = compile ps_2_0 PixelShader();
	}
}