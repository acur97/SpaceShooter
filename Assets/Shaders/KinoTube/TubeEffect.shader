// KinoTube - Old TV/video artifacts simulation https://github.com/keijiro/KinoTube
// Updated for Unity 6 by @acur97 https://github.com/acur97

Shader "Hidden/TubeEffect"
{
    HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        #define UNITY_PI 3.14159265359h

        // Declare several properties that will be used to control the behavior of the shader.        
        half _bleeding;
        int _bleedingSteps;
        half _fringing;

        int _BleedTaps;
        half _BleedDelta;
        half _FringeDelta;
        half _Scanline;
        half _ScalineScale;
        half _ScalineSpeed;

        // This function converts an RGB color to a YIQ color.
        // YIQ is a color space that separates the luminance (brightness) from the chrominance (color) information.
        half3 RGB2YIQ(half3 rgb)
        {
            // Saturate the RGB color to ensure it is within the valid range.
            rgb = saturate(rgb);

            // If the color space is not gamma, convert the RGB color to gamma space.
            // #ifndef UNITY_COLORSPACE_GAMMA
            //     rgb = LinearToGammaSpace(rgb);
            // #endif

            // Convert the RGB color to YIQ using a matrix multiplication.
            return mul(half3x3(0.299h,  0.587h,  0.114h,
                               0.596h, -0.274h, -0.322h,
                               0.211h, -0.523h,  0.313h), rgb);
        }

        // This function converts a YIQ color to an RGB color.
        half3 YIQ2RGB(half3 yiq)
        {
            // Convert the YIQ color to RGB using a matrix multiplication.
            half3 rgb = mul(half3x3(1,  0.956h,  0.621h,
                                    1, -0.272h, -0.647h,
                                    1, -1.106h,  1.703h), yiq);

            // Saturate the RGB color to ensure it is within the valid range.
            rgb = saturate(rgb);

            // If the color space is not gamma, convert the RGB color to linear space.
            // #ifndef UNITY_COLORSPACE_GAMMA
            //     rgb = GammaToLinearSpace(rgb);
            // #endif

            return rgb;
        }

        // This function samples the YIQ color at a given UV coordinate and offset.
        half3 SampleYIQ(half2 uv, half du)
        {
            // Offset the UV coordinate by the du value and
            // Sample the texture at the offset UV coordinate and convert the result to YIQ.
            return RGB2YIQ((half3)SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + half2(du, 0)).rgb);
        }

        half4 frag(Varyings input) : SV_Target
        {
            // Extract the UV coordinates from the input.
            half2 uv = input.texcoord;

            // Sample the YIQ color at the current UV coordinate.
            half3 yiq = SampleYIQ(uv, 0);

            #ifndef SHADER_API_GLES3
            // Setup values
            half bleedWidth = 0.04h * _bleeding; // width of bleeding
            half bleedStep = 2.5h / _bleedingSteps; // max interval of taps
            int bleedTaps = ceil(bleedWidth / bleedStep);
            half bleedDelta = bleedWidth / bleedTaps;
            
            // Perform bleeding effect by sampling the YIQ color at neighboring UV coordinates and adding them to the current YIQ color.
            for (int i = 0; i < bleedTaps; i++)
            {
                half3 sample = SampleYIQ(uv, -bleedDelta * i);
                yiq.y += sample.y;
                yiq.z += SampleYIQ(uv, +bleedDelta * i).z;
            }
            
            // Normalize the Y and Z components of the YIQ color
            yiq.yz /= (bleedTaps + 1);
            #endif
            
            // Perform fringing effect by sampling the YIQ color at neighboring UV coordinates and subtracting the samples.
            half fringeDelta = 0.0025h * _fringing; // width of fringing
            half y1 = SampleYIQ(uv, -fringeDelta).x;
            half y2 = SampleYIQ(uv, +fringeDelta).x;
            yiq.yz += (y2 - y1);
            
            // Perform scanline effect by generating a sine wave based on the Y coordinate of the UV and scaling it.
            uv.y += (half)_Time.x * _ScalineSpeed;
            half scan = lerp(1, (sin(uv.y * _ScalineScale * UNITY_PI) * 0.5h + 0.5h), _Scanline);

            // Convert the YIQ color to RGB and multiply it by the scanline effect
            return half4(YIQ2RGB(yiq * scan), 1);
        }
        
    ENDHLSL
    
    Properties
    {
        _bleeding ("Bleeding", Range(0.0, 1.0)) = 0.5
        _bleedingSteps ("Bleeding Quality", Range(0.0, 1920.0)) = 720
        _fringing ("Fringing", Range(0.0, 1.0)) = 0.25
        _Scanline ("Scanline", Range(0.0, 1.0)) = 0.0
        _ScalineScale ("Scanline Scale", Range(0.0, 1000.0)) = 500
        _ScalineSpeed ("Scanline Speed", Range(-1.0, 1.0)) = 0.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100
        ZWrite Off Cull Off

        Pass
        {
            Name "Tube Pass"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment frag
            
            ENDHLSL
        }
    }
}