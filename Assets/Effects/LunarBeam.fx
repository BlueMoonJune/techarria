sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float2 uTargetPosition;
float uSaturation;
float uRotation;
float myParam;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

float4x4 colors =
{
    254, 0, 0, 254,
    158, 242, 174, 126,
    35, 170, 238, 229,
    255, 255, 255, 255
};

const float PI = 2 * acos(0);

const bool pixelate = true;
const float pixelScale = 2.0;

const float thickness = 16.0;
const float indent = 3.0;
const float waveAmplitude = 1.5;
const float waveFrequency = 2.0;
const float waveSize = 60.0;
const float beamLength = 80;

// This is a shader. You are on your own with shaders. Compile shaders in an XNB project.

float4 Beam(float2 coords : TEXCOORD0) : COLOR0
{
    float2 fragCoord = coords * uImageSize0;
    
    float2 p;
    if (pixelate)
    {
        p = round(fragCoord / pixelScale) * pixelScale;
    }
    else
    {
        p = fragCoord;
    }

    float2 p1 = float2(thickness, uImageSize0.y / 2.0);
    float2 p2 = float2(beamLength + thickness, uImageSize0.y / 2.0);

    float t = myParam + (p.x + p.y) / waveSize;
    // Time varying pixel color
    float dist = length(p - p1);
    dist = min(dist, length(p - p2));
    if (dot(p2 - p1, p - p1) > 0.0 && dot(p1 - p2, p - p2) > 0.0)
    {
        float2 a = p - p1;
        float2 b = normalize(p2 - p1);
        dist = min(dist, length(a - dot(a, b) * b) + indent + asin(sin(t * waveFrequency)) * waveAmplitude);
    }
    
    float4 edgeCol = lerp(colors[int(t) % 4]/255, colors[int(t + 1.0) % 4]/255, sin(t % 1.0) * PI / 2.0);
    
    // Output to screen
    if (dist < thickness)
    {
        return lerp(float4(1, 1, 1, 1), edgeCol, pow(dist / thickness, 5.0));
    }
    else
    {
        return float4(0, 0, 0, 0);
    }
}

float4 UV(float2 coords : TEXCOORD0) : COLOR0
{
    return float4(coords, 0, 1);

}

technique Technique1
{
	pass ExampleDyePass
	{
		PixelShader = compile ps_3_0 Beam();
	}
}