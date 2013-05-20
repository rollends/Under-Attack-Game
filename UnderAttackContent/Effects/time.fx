//uniform extern texture ScreenTexture;	

static const float PI = 3.14159265f;
float time = 0;

sampler TextureSampler : register(s0);

float4 ApplyTimeEffect(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    // Look up the texture color.
    float4 tex = tex2D(TextureSampler, texCoord);
    float4 delta = float4 ( sin((2 * PI * texCoord.y - PI + time/2*PI)/4)/4, sin((2 * PI * texCoord.y- PI + time/2*PI)/4)/4, 0, 0);

    tex.rgba = clamp(delta.rgba + tex.rgba,float4(0,0,0,0),float4(1,1,1,1));
   
    return tex;
}

technique
{
	pass P0
	{
		PixelShader = compile ps_2_0 ApplyTimeEffect();
	}
}
