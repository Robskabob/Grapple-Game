Shader "Unlit/Outline"
{
    Properties
    {
        Vector1_1E2ADAD("mult", Float) = 1.1
        Color_CF041842("Color", Color) = (0.5660378, 1, 0.5831137, 0.7686275)
    }
        SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent+0"
        }

        Pass
        {
            Name "Pass"
            Tags
            {
            // LightMode: <None>
        }

        // Render State
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Front
        ZTest LEqual
        ZWrite Off
        // ColorMask: <None>


        HLSLPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        // Pragmas
        #pragma prefer_hlslcc gles
        #pragma exclude_renderers d3d11_9x
        #pragma target 2.0
        #pragma multi_compile_fog
        #pragma multi_compile_instancing

        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _AlphaClip 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        #define SHADERPASS_UNLIT

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"

        // --------------------------------------------------
        // Graph

        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float Vector1_1E2ADAD;
        float4 Color_CF041842;
        CBUFFER_END

            // Graph Functions

            void Unity_Comparison_Greater_float(float A, float B, out float Out)
            {
                Out = A > B ? 1 : 0;
            }

            void Unity_Negate_float(float In, out float Out)
            {
                Out = -1 * In;
            }

            void Unity_Branch_float(float Predicate, float True, float False, out float Out)
            {
                Out = Predicate ? True : False;
            }

            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }

            struct Bindings_SAdd_74515b17eb537a543b8fe720278da7c7
            {
            };

            void SG_SAdd_74515b17eb537a543b8fe720278da7c7(float Vector1_45D94C47, float Vector1_DDE64ED8, float Vector1_79A6082A, Bindings_SAdd_74515b17eb537a543b8fe720278da7c7 IN, out float New_0)
            {
                float _Property_861CD8BA_Out_0 = Vector1_79A6082A;
                float _Comparison_652A3DFB_Out_2;
                Unity_Comparison_Greater_float(_Property_861CD8BA_Out_0, 0, _Comparison_652A3DFB_Out_2);
                float _Property_422FA54B_Out_0 = Vector1_45D94C47;
                float _Negate_F654064A_Out_1;
                Unity_Negate_float(_Property_422FA54B_Out_0, _Negate_F654064A_Out_1);
                float _Branch_38A7C1EC_Out_3;
                Unity_Branch_float(_Comparison_652A3DFB_Out_2, _Property_422FA54B_Out_0, _Negate_F654064A_Out_1, _Branch_38A7C1EC_Out_3);
                float _Property_83C22573_Out_0 = Vector1_DDE64ED8;
                float _Add_A5F1579F_Out_2;
                Unity_Add_float(_Branch_38A7C1EC_Out_3, _Property_83C22573_Out_0, _Add_A5F1579F_Out_2);
                New_0 = _Add_A5F1579F_Out_2;
            }

            void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
            {
                RGBA = float4(R, G, B, A);
                RGB = float3(R, G, B);
                RG = float2(R, G);
            }

            // Graph Vertex
            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 WorldSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 WorldSpaceTangent;
                float3 ObjectSpaceBiTangent;
                float3 WorldSpaceBiTangent;
                float3 ObjectSpacePosition;
                float3 WorldSpacePosition;
            };

            struct VertexDescription
            {
                float3 VertexPosition;
                float3 VertexNormal;
                float3 VertexTangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                float _Property_7ECBCDB4_Out_0 = Vector1_1E2ADAD;
                float _Split_77C3E200_R_1 = IN.WorldSpacePosition[0];
                float _Split_77C3E200_G_2 = IN.WorldSpacePosition[1];
                float _Split_77C3E200_B_3 = IN.WorldSpacePosition[2];
                float _Split_77C3E200_A_4 = 0;
                float _Split_3034B6F8_R_1 = IN.ObjectSpacePosition[0];
                float _Split_3034B6F8_G_2 = IN.ObjectSpacePosition[1];
                float _Split_3034B6F8_B_3 = IN.ObjectSpacePosition[2];
                float _Split_3034B6F8_A_4 = 0;
                Bindings_SAdd_74515b17eb537a543b8fe720278da7c7 _SAdd_69F519B8;
                float _SAdd_69F519B8_New_0;
                SG_SAdd_74515b17eb537a543b8fe720278da7c7(_Property_7ECBCDB4_Out_0, _Split_77C3E200_R_1, _Split_3034B6F8_R_1, _SAdd_69F519B8, _SAdd_69F519B8_New_0);
                Bindings_SAdd_74515b17eb537a543b8fe720278da7c7 _SAdd_CD38EF6D;
                float _SAdd_CD38EF6D_New_0;
                SG_SAdd_74515b17eb537a543b8fe720278da7c7(_Property_7ECBCDB4_Out_0, _Split_77C3E200_G_2, _Split_3034B6F8_G_2, _SAdd_CD38EF6D, _SAdd_CD38EF6D_New_0);
                Bindings_SAdd_74515b17eb537a543b8fe720278da7c7 _SAdd_B7A9A7A0;
                float _SAdd_B7A9A7A0_New_0;
                SG_SAdd_74515b17eb537a543b8fe720278da7c7(_Property_7ECBCDB4_Out_0, _Split_77C3E200_B_3, _Split_3034B6F8_B_3, _SAdd_B7A9A7A0, _SAdd_B7A9A7A0_New_0);
                float4 _Combine_657E7B70_RGBA_4;
                float3 _Combine_657E7B70_RGB_5;
                float2 _Combine_657E7B70_RG_6;
                Unity_Combine_float(_SAdd_69F519B8_New_0, _SAdd_CD38EF6D_New_0, _SAdd_B7A9A7A0_New_0, 0, _Combine_657E7B70_RGBA_4, _Combine_657E7B70_RGB_5, _Combine_657E7B70_RG_6);
                float3 _Transform_7302BB02_Out_1 = TransformWorldToObject(_Combine_657E7B70_RGB_5.xyz);
                description.VertexPosition = _Transform_7302BB02_Out_1;
                description.VertexNormal = IN.ObjectSpaceNormal;
                description.VertexTangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
            };

            struct SurfaceDescription
            {
                float3 Color;
                float Alpha;
                float AlphaClipThreshold;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                surface.Color = IsGammaSpace() ? float3(0.7353569, 0.7353569, 0.7353569) : SRGBToLinear(float3(0.7353569, 0.7353569, 0.7353569));
                surface.Alpha = 1;
                surface.AlphaClipThreshold = 0.5;
                return surface;
            }

            // --------------------------------------------------
            // Structs and Packing

            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output = (PackedVaryings)0;
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }

            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.WorldSpaceNormal = TransformObjectToWorldNormal(input.normalOS);
                output.ObjectSpaceTangent = input.tangentOS;
                output.WorldSpaceTangent = TransformObjectToWorldDir(input.tangentOS.xyz);
                output.ObjectSpaceBiTangent = normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
                output.WorldSpaceBiTangent = TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
                output.ObjectSpacePosition = input.positionOS;
                output.WorldSpacePosition = TransformObjectToWorld(input.positionOS);

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

                // Render State
                Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                Cull Back
                ZTest LEqual
                ZWrite On
                // ColorMask: <None>


                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                // Debug
                // <None>

                // --------------------------------------------------
                // Pass

                // Pragmas
                #pragma prefer_hlslcc gles
                #pragma exclude_renderers d3d11_9x
                #pragma target 2.0
                #pragma multi_compile_instancing

                // Keywords
                #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
                // GraphKeywords: <None>

                // Defines
                #define _SURFACE_TYPE_TRANSPARENT 1
                #define _AlphaClip 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define FEATURES_GRAPH_VERTEX
                #define SHADERPASS_SHADOWCASTER

                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"

                // --------------------------------------------------
                // Graph

                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float Vector1_1E2ADAD;
                float4 Color_CF041842;
                CBUFFER_END

                    // Graph Functions

                    void Unity_Comparison_Greater_float(float A, float B, out float Out)
                    {
                        Out = A > B ? 1 : 0;
                    }

                    void Unity_Negate_float(float In, out float Out)
                    {
                        Out = -1 * In;
                    }

                    void Unity_Branch_float(float Predicate, float True, float False, out float Out)
                    {
                        Out = Predicate ? True : False;
                    }

                    void Unity_Add_float(float A, float B, out float Out)
                    {
                        Out = A + B;
                    }

                    struct Bindings_SAdd_74515b17eb537a543b8fe720278da7c7
                    {
                    };

                    void SG_SAdd_74515b17eb537a543b8fe720278da7c7(float Vector1_45D94C47, float Vector1_DDE64ED8, float Vector1_79A6082A, Bindings_SAdd_74515b17eb537a543b8fe720278da7c7 IN, out float New_0)
                    {
                        float _Property_861CD8BA_Out_0 = Vector1_79A6082A;
                        float _Comparison_652A3DFB_Out_2;
                        Unity_Comparison_Greater_float(_Property_861CD8BA_Out_0, 0, _Comparison_652A3DFB_Out_2);
                        float _Property_422FA54B_Out_0 = Vector1_45D94C47;
                        float _Negate_F654064A_Out_1;
                        Unity_Negate_float(_Property_422FA54B_Out_0, _Negate_F654064A_Out_1);
                        float _Branch_38A7C1EC_Out_3;
                        Unity_Branch_float(_Comparison_652A3DFB_Out_2, _Property_422FA54B_Out_0, _Negate_F654064A_Out_1, _Branch_38A7C1EC_Out_3);
                        float _Property_83C22573_Out_0 = Vector1_DDE64ED8;
                        float _Add_A5F1579F_Out_2;
                        Unity_Add_float(_Branch_38A7C1EC_Out_3, _Property_83C22573_Out_0, _Add_A5F1579F_Out_2);
                        New_0 = _Add_A5F1579F_Out_2;
                    }

                    void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
                    {
                        RGBA = float4(R, G, B, A);
                        RGB = float3(R, G, B);
                        RG = float2(R, G);
                    }

                    // Graph Vertex
                    struct VertexDescriptionInputs
                    {
                        float3 ObjectSpaceNormal;
                        float3 WorldSpaceNormal;
                        float3 ObjectSpaceTangent;
                        float3 WorldSpaceTangent;
                        float3 ObjectSpaceBiTangent;
                        float3 WorldSpaceBiTangent;
                        float3 ObjectSpacePosition;
                        float3 WorldSpacePosition;
                    };

                    struct VertexDescription
                    {
                        float3 VertexPosition;
                        float3 VertexNormal;
                        float3 VertexTangent;
                    };

                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                    {
                        VertexDescription description = (VertexDescription)0;
                        float _Property_7ECBCDB4_Out_0 = Vector1_1E2ADAD;
                        float _Split_77C3E200_R_1 = IN.WorldSpacePosition[0];
                        float _Split_77C3E200_G_2 = IN.WorldSpacePosition[1];
                        float _Split_77C3E200_B_3 = IN.WorldSpacePosition[2];
                        float _Split_77C3E200_A_4 = 0;
                        float _Split_3034B6F8_R_1 = IN.ObjectSpacePosition[0];
                        float _Split_3034B6F8_G_2 = IN.ObjectSpacePosition[1];
                        float _Split_3034B6F8_B_3 = IN.ObjectSpacePosition[2];
                        float _Split_3034B6F8_A_4 = 0;
                        Bindings_SAdd_74515b17eb537a543b8fe720278da7c7 _SAdd_69F519B8;
                        float _SAdd_69F519B8_New_0;
                        SG_SAdd_74515b17eb537a543b8fe720278da7c7(_Property_7ECBCDB4_Out_0, _Split_77C3E200_R_1, _Split_3034B6F8_R_1, _SAdd_69F519B8, _SAdd_69F519B8_New_0);
                        Bindings_SAdd_74515b17eb537a543b8fe720278da7c7 _SAdd_CD38EF6D;
                        float _SAdd_CD38EF6D_New_0;
                        SG_SAdd_74515b17eb537a543b8fe720278da7c7(_Property_7ECBCDB4_Out_0, _Split_77C3E200_G_2, _Split_3034B6F8_G_2, _SAdd_CD38EF6D, _SAdd_CD38EF6D_New_0);
                        Bindings_SAdd_74515b17eb537a543b8fe720278da7c7 _SAdd_B7A9A7A0;
                        float _SAdd_B7A9A7A0_New_0;
                        SG_SAdd_74515b17eb537a543b8fe720278da7c7(_Property_7ECBCDB4_Out_0, _Split_77C3E200_B_3, _Split_3034B6F8_B_3, _SAdd_B7A9A7A0, _SAdd_B7A9A7A0_New_0);
                        float4 _Combine_657E7B70_RGBA_4;
                        float3 _Combine_657E7B70_RGB_5;
                        float2 _Combine_657E7B70_RG_6;
                        Unity_Combine_float(_SAdd_69F519B8_New_0, _SAdd_CD38EF6D_New_0, _SAdd_B7A9A7A0_New_0, 0, _Combine_657E7B70_RGBA_4, _Combine_657E7B70_RGB_5, _Combine_657E7B70_RG_6);
                        float3 _Transform_7302BB02_Out_1 = TransformWorldToObject(_Combine_657E7B70_RGB_5.xyz);
                        description.VertexPosition = _Transform_7302BB02_Out_1;
                        description.VertexNormal = IN.ObjectSpaceNormal;
                        description.VertexTangent = IN.ObjectSpaceTangent;
                        return description;
                    }

                    // Graph Pixel
                    struct SurfaceDescriptionInputs
                    {
                    };

                    struct SurfaceDescription
                    {
                        float Alpha;
                        float AlphaClipThreshold;
                    };

                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                    {
                        SurfaceDescription surface = (SurfaceDescription)0;
                        surface.Alpha = 1;
                        surface.AlphaClipThreshold = 0.5;
                        return surface;
                    }

                    // --------------------------------------------------
                    // Structs and Packing

                    // Generated Type: Attributes
                    struct Attributes
                    {
                        float3 positionOS : POSITION;
                        float3 normalOS : NORMAL;
                        float4 tangentOS : TANGENT;
                        #if UNITY_ANY_INSTANCING_ENABLED
                        uint instanceID : INSTANCEID_SEMANTIC;
                        #endif
                    };

                    // Generated Type: Varyings
                    struct Varyings
                    {
                        float4 positionCS : SV_POSITION;
                        #if UNITY_ANY_INSTANCING_ENABLED
                        uint instanceID : CUSTOM_INSTANCE_ID;
                        #endif
                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                        uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                        #endif
                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                        uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                        #endif
                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                        #endif
                    };

                    // Generated Type: PackedVaryings
                    struct PackedVaryings
                    {
                        float4 positionCS : SV_POSITION;
                        #if UNITY_ANY_INSTANCING_ENABLED
                        uint instanceID : CUSTOM_INSTANCE_ID;
                        #endif
                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                        uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                        #endif
                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                        uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                        #endif
                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                        #endif
                    };

                    // Packed Type: Varyings
                    PackedVaryings PackVaryings(Varyings input)
                    {
                        PackedVaryings output = (PackedVaryings)0;
                        output.positionCS = input.positionCS;
                        #if UNITY_ANY_INSTANCING_ENABLED
                        output.instanceID = input.instanceID;
                        #endif
                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                        #endif
                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                        #endif
                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                        output.cullFace = input.cullFace;
                        #endif
                        return output;
                    }

                    // Unpacked Type: Varyings
                    Varyings UnpackVaryings(PackedVaryings input)
                    {
                        Varyings output = (Varyings)0;
                        output.positionCS = input.positionCS;
                        #if UNITY_ANY_INSTANCING_ENABLED
                        output.instanceID = input.instanceID;
                        #endif
                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                        #endif
                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                        #endif
                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                        output.cullFace = input.cullFace;
                        #endif
                        return output;
                    }

                    // --------------------------------------------------
                    // Build Graph Inputs

                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                    {
                        VertexDescriptionInputs output;
                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                        output.ObjectSpaceNormal = input.normalOS;
                        output.WorldSpaceNormal = TransformObjectToWorldNormal(input.normalOS);
                        output.ObjectSpaceTangent = input.tangentOS;
                        output.WorldSpaceTangent = TransformObjectToWorldDir(input.tangentOS.xyz);
                        output.ObjectSpaceBiTangent = normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
                        output.WorldSpaceBiTangent = TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
                        output.ObjectSpacePosition = input.positionOS;
                        output.WorldSpacePosition = TransformObjectToWorld(input.positionOS);

                        return output;
                    }

                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                    {
                        SurfaceDescriptionInputs output;
                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                    #else
                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                    #endif
                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                        return output;
                    }


                    // --------------------------------------------------
                    // Main

                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

                    ENDHLSL
                }

                Pass
                {
                    Name "DepthOnly"
                    Tags
                    {
                        "LightMode" = "DepthOnly"
                    }

                        // Render State
                        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                        Cull Back
                        ZTest LEqual
                        ZWrite On
                        ColorMask 0


                        HLSLPROGRAM
                        #pragma vertex vert
                        #pragma fragment frag

                        // Debug
                        // <None>

                        // --------------------------------------------------
                        // Pass

                        // Pragmas
                        #pragma prefer_hlslcc gles
                        #pragma exclude_renderers d3d11_9x
                        #pragma target 2.0
                        #pragma multi_compile_instancing

                        // Keywords
                        // PassKeywords: <None>
                        // GraphKeywords: <None>

                        // Defines
                        #define _SURFACE_TYPE_TRANSPARENT 1
                        #define _AlphaClip 1
                        #define ATTRIBUTES_NEED_NORMAL
                        #define ATTRIBUTES_NEED_TANGENT
                        #define FEATURES_GRAPH_VERTEX
                        #define SHADERPASS_DEPTHONLY

                        // Includes
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                        #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"

                        // --------------------------------------------------
                        // Graph

                        // Graph Properties
                        CBUFFER_START(UnityPerMaterial)
                        float Vector1_1E2ADAD;
                        float4 Color_CF041842;
                        CBUFFER_END

                            // Graph Functions

                            void Unity_Comparison_Greater_float(float A, float B, out float Out)
                            {
                                Out = A > B ? 1 : 0;
                            }

                            void Unity_Negate_float(float In, out float Out)
                            {
                                Out = -1 * In;
                            }

                            void Unity_Branch_float(float Predicate, float True, float False, out float Out)
                            {
                                Out = Predicate ? True : False;
                            }

                            void Unity_Add_float(float A, float B, out float Out)
                            {
                                Out = A + B;
                            }

                            struct Bindings_SAdd_74515b17eb537a543b8fe720278da7c7
                            {
                            };

                            void SG_SAdd_74515b17eb537a543b8fe720278da7c7(float Vector1_45D94C47, float Vector1_DDE64ED8, float Vector1_79A6082A, Bindings_SAdd_74515b17eb537a543b8fe720278da7c7 IN, out float New_0)
                            {
                                float _Property_861CD8BA_Out_0 = Vector1_79A6082A;
                                float _Comparison_652A3DFB_Out_2;
                                Unity_Comparison_Greater_float(_Property_861CD8BA_Out_0, 0, _Comparison_652A3DFB_Out_2);
                                float _Property_422FA54B_Out_0 = Vector1_45D94C47;
                                float _Negate_F654064A_Out_1;
                                Unity_Negate_float(_Property_422FA54B_Out_0, _Negate_F654064A_Out_1);
                                float _Branch_38A7C1EC_Out_3;
                                Unity_Branch_float(_Comparison_652A3DFB_Out_2, _Property_422FA54B_Out_0, _Negate_F654064A_Out_1, _Branch_38A7C1EC_Out_3);
                                float _Property_83C22573_Out_0 = Vector1_DDE64ED8;
                                float _Add_A5F1579F_Out_2;
                                Unity_Add_float(_Branch_38A7C1EC_Out_3, _Property_83C22573_Out_0, _Add_A5F1579F_Out_2);
                                New_0 = _Add_A5F1579F_Out_2;
                            }

                            void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
                            {
                                RGBA = float4(R, G, B, A);
                                RGB = float3(R, G, B);
                                RG = float2(R, G);
                            }

                            // Graph Vertex
                            struct VertexDescriptionInputs
                            {
                                float3 ObjectSpaceNormal;
                                float3 WorldSpaceNormal;
                                float3 ObjectSpaceTangent;
                                float3 WorldSpaceTangent;
                                float3 ObjectSpaceBiTangent;
                                float3 WorldSpaceBiTangent;
                                float3 ObjectSpacePosition;
                                float3 WorldSpacePosition;
                            };

                            struct VertexDescription
                            {
                                float3 VertexPosition;
                                float3 VertexNormal;
                                float3 VertexTangent;
                            };

                            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                            {
                                VertexDescription description = (VertexDescription)0;
                                float _Property_7ECBCDB4_Out_0 = Vector1_1E2ADAD;
                                float _Split_77C3E200_R_1 = IN.WorldSpacePosition[0];
                                float _Split_77C3E200_G_2 = IN.WorldSpacePosition[1];
                                float _Split_77C3E200_B_3 = IN.WorldSpacePosition[2];
                                float _Split_77C3E200_A_4 = 0;
                                float _Split_3034B6F8_R_1 = IN.ObjectSpacePosition[0];
                                float _Split_3034B6F8_G_2 = IN.ObjectSpacePosition[1];
                                float _Split_3034B6F8_B_3 = IN.ObjectSpacePosition[2];
                                float _Split_3034B6F8_A_4 = 0;
                                Bindings_SAdd_74515b17eb537a543b8fe720278da7c7 _SAdd_69F519B8;
                                float _SAdd_69F519B8_New_0;
                                SG_SAdd_74515b17eb537a543b8fe720278da7c7(_Property_7ECBCDB4_Out_0, _Split_77C3E200_R_1, _Split_3034B6F8_R_1, _SAdd_69F519B8, _SAdd_69F519B8_New_0);
                                Bindings_SAdd_74515b17eb537a543b8fe720278da7c7 _SAdd_CD38EF6D;
                                float _SAdd_CD38EF6D_New_0;
                                SG_SAdd_74515b17eb537a543b8fe720278da7c7(_Property_7ECBCDB4_Out_0, _Split_77C3E200_G_2, _Split_3034B6F8_G_2, _SAdd_CD38EF6D, _SAdd_CD38EF6D_New_0);
                                Bindings_SAdd_74515b17eb537a543b8fe720278da7c7 _SAdd_B7A9A7A0;
                                float _SAdd_B7A9A7A0_New_0;
                                SG_SAdd_74515b17eb537a543b8fe720278da7c7(_Property_7ECBCDB4_Out_0, _Split_77C3E200_B_3, _Split_3034B6F8_B_3, _SAdd_B7A9A7A0, _SAdd_B7A9A7A0_New_0);
                                float4 _Combine_657E7B70_RGBA_4;
                                float3 _Combine_657E7B70_RGB_5;
                                float2 _Combine_657E7B70_RG_6;
                                Unity_Combine_float(_SAdd_69F519B8_New_0, _SAdd_CD38EF6D_New_0, _SAdd_B7A9A7A0_New_0, 0, _Combine_657E7B70_RGBA_4, _Combine_657E7B70_RGB_5, _Combine_657E7B70_RG_6);
                                float3 _Transform_7302BB02_Out_1 = TransformWorldToObject(_Combine_657E7B70_RGB_5.xyz);
                                description.VertexPosition = _Transform_7302BB02_Out_1;
                                description.VertexNormal = IN.ObjectSpaceNormal;
                                description.VertexTangent = IN.ObjectSpaceTangent;
                                return description;
                            }

                            // Graph Pixel
                            struct SurfaceDescriptionInputs
                            {
                            };

                            struct SurfaceDescription
                            {
                                float Alpha;
                                float AlphaClipThreshold;
                            };

                            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                            {
                                SurfaceDescription surface = (SurfaceDescription)0;
                                surface.Alpha = 1;
                                surface.AlphaClipThreshold = 0.5;
                                return surface;
                            }

                            // --------------------------------------------------
                            // Structs and Packing

                            // Generated Type: Attributes
                            struct Attributes
                            {
                                float3 positionOS : POSITION;
                                float3 normalOS : NORMAL;
                                float4 tangentOS : TANGENT;
                                #if UNITY_ANY_INSTANCING_ENABLED
                                uint instanceID : INSTANCEID_SEMANTIC;
                                #endif
                            };

                            // Generated Type: Varyings
                            struct Varyings
                            {
                                float4 positionCS : SV_POSITION;
                                #if UNITY_ANY_INSTANCING_ENABLED
                                uint instanceID : CUSTOM_INSTANCE_ID;
                                #endif
                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                #endif
                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                #endif
                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                #endif
                            };

                            // Generated Type: PackedVaryings
                            struct PackedVaryings
                            {
                                float4 positionCS : SV_POSITION;
                                #if UNITY_ANY_INSTANCING_ENABLED
                                uint instanceID : CUSTOM_INSTANCE_ID;
                                #endif
                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                #endif
                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                #endif
                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                #endif
                            };

                            // Packed Type: Varyings
                            PackedVaryings PackVaryings(Varyings input)
                            {
                                PackedVaryings output = (PackedVaryings)0;
                                output.positionCS = input.positionCS;
                                #if UNITY_ANY_INSTANCING_ENABLED
                                output.instanceID = input.instanceID;
                                #endif
                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                #endif
                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                #endif
                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                output.cullFace = input.cullFace;
                                #endif
                                return output;
                            }

                            // Unpacked Type: Varyings
                            Varyings UnpackVaryings(PackedVaryings input)
                            {
                                Varyings output = (Varyings)0;
                                output.positionCS = input.positionCS;
                                #if UNITY_ANY_INSTANCING_ENABLED
                                output.instanceID = input.instanceID;
                                #endif
                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                #endif
                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                #endif
                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                output.cullFace = input.cullFace;
                                #endif
                                return output;
                            }

                            // --------------------------------------------------
                            // Build Graph Inputs

                            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                            {
                                VertexDescriptionInputs output;
                                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                output.ObjectSpaceNormal = input.normalOS;
                                output.WorldSpaceNormal = TransformObjectToWorldNormal(input.normalOS);
                                output.ObjectSpaceTangent = input.tangentOS;
                                output.WorldSpaceTangent = TransformObjectToWorldDir(input.tangentOS.xyz);
                                output.ObjectSpaceBiTangent = normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
                                output.WorldSpaceBiTangent = TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
                                output.ObjectSpacePosition = input.positionOS;
                                output.WorldSpacePosition = TransformObjectToWorld(input.positionOS);

                                return output;
                            }

                            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                            {
                                SurfaceDescriptionInputs output;
                                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                            #else
                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                            #endif
                            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                return output;
                            }


                            // --------------------------------------------------
                            // Main

                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

                            ENDHLSL
                        }

    }
        FallBack "Hidden/Shader Graph/FallbackError"
}
