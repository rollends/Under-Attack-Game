uniform extern texture ScreenTexture;	

static const float PI = 3.14159265f;
float time;

sampler ScreenS = sampler_state
{
	Texture = <ScreenTexture>;	
};

float4 PixelShader(float2 texCoord: TEXCOORD0) : COLOR
{
	float4 color = tex2D(ScreenS, texCoord) + float4 ( sin((2 * PI * texCoord.y - PI + time/2*PI)/4)/4, sin((2 * PI * texCoord.y - PI + time/2*PI)/4)/4, 0, 0);
		
	return color;
}

technique
{
	pass P0
	{
		PixelShader = compile ps_2_0 PixelShader();
	}
}
