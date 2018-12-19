﻿Shader "Instanced/Trees/Crossboard" 
{
	Properties
	{
		_TrunkColor("Color Trunk", Color) = (1,1,1,1)
		_TrunkColorModify("Trunk variance", Color) = (0.8,0.7,0.1,1)
		_LeafColor("Color leafs", Color) = (0,0,0,1)
		_LeafColorModify("Leaf variance", Color) = (0,0,0,1)
		_RandomColor("Random Color Amount", Range(0,1)) = 0.7

		_Mask("Leaf mask", 2D) = "white" {}
		_MainTex("Texture", 2D) = "white" {}

		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.6

		_Offset("Placment Offset", Vector) = (0,0.3,0,0)
		/*_RandomWidth("Random Width Amount", Range(0,2)) = 1
		_RandomHeight("Random Height Amount", Range(0,2)) = 1*/

		[Toggle(FILL_WITH_RED)]
		_UseLod("Use Lod", Float) = 1
		_LodFarFade("Far FadeOut Distance", Float) = 100
		_LodFar("Far LodOut Distance", Float) = 200
		_LodNearFade("Near FadeOut Distance", Float) = 10
		_LodNear("Near LodOut Distance", Float) = 5

		// TODO: move to a computeShader which send back a splatmap with the wind info.
		_TreeAmplitude("Wind Amplitude", Float) = 0.001
		_WindSpeed("Wind Speed", Float) = 100
		_WindStength("Wind Strength", Float) = 20
	}
	SubShader
	{
		Cull Off
		LOD 200
		//Offset 10, -10

		Pass
		{
			ZWrite On
			ColorMask 0
		}

		Tags
		{
			"Queue" = "Transparent-1" "RenderType" = "Transparent" "LightMode" = "ForwardBase"
		}
		Pass 
		{
			ZWrite Off // don't write to depth buffer 
			// in order not to occlude other objects
			Blend SrcAlpha OneMinusSrcAlpha // multiplicative blending 
			// for attenuation by the fragment's alpha

			CGPROGRAM
			#define PI 3.1415926535897932384626433832795
			#include "UnityCG.cginc" 
			#include "Lighting.cginc"

			// shadow helper functions and macros
			#include "AutoLight.cginc"

			//#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight alpha:fade
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom

			#pragma target 4.0

			#include "Crossboard.cginc"

			ENDCG
		}
		Tags
		{
			"Queue" = "Transparent" "RenderType" = "Transparent" "LightMode" = "ForwardBase"
		}
		Pass 
		{
			ZWrite On // write to depth buffer to add color
			Blend SrcAlpha OneMinusSrcAlpha // additive blending to add colors

			CGPROGRAM
			#define PI 3.1415926535897932384626433832795
			#include "UnityCG.cginc" 
			#include "Lighting.cginc"

			// shadow helper functions and macros
			#include "AutoLight.cginc"

			//#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight alpha:fade
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom

			#pragma target 4.0
			#include "Crossboard.cginc"	

			ENDCG
		}
	}
}