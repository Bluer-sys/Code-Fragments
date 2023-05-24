Shader "Game/PulseCircle"
{
	Properties
	{
		[Header(Color)]
		_Color1 ("Color 1", Color)					= (1,1,1,1)
		_Color2 ("Color 2", Color)					= (1,1,1,1)
		
		[Header(Radius)]
		_Radius1("Radius 1", Range(0,2))			= 1
		_Radius2("Radius 2", Range(0,2))			= 1
		
		[Header(Thickness)]
		_Thick1("Thickness 1", Range(0,3))			= 1
		_Thick2("Thickness 2", Range(0,3))			= 1
		
		[Header(Pulsation)]
		_PulseFrequency ("Frequency", Range(0,3))	= 1
		_PulseOffset ("Offset", Range(0,3))			= 1
		_PulseMul ("Multiplier", Range(0,2))		= 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
		LOD 200
		
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM

		#pragma surface surf Standard  vertex:vert alpha:fade fullforwardshadows
		#pragma target 3.0

		
		struct Input
		{
			float4 color : COLOR;
			float3 localPos;
		};

		// Circle
		float	_Radius1;
		float	_Radius2;
		fixed3	_Color1;
		fixed3	_Color2;
		float	_Thick1;
		float	_Thick2;

		// Pulse
		fixed	_PulseFrequency;
		fixed	_PulseOffset;
		fixed	_PulseMul;

		float cycleFunc(float value)
		{
			return pow(value % 3, 2) / 2; 
		}
		
		void vert(inout appdata_full v, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);

			data.localPos	= v.vertex;
        }
		
		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c		= IN.color;

			// Pulsation
			float r1		= _Radius1 + (cycleFunc(_Time.z * _PulseFrequency) + _PulseOffset) * _PulseMul;
			float r2		= _Radius2 + (cycleFunc(_Time.z * _PulseFrequency) + _PulseOffset) * _PulseMul;
			
			// Circle 1
			bool inCircle1	= step(length(IN.localPos), r1) && step(r1 - _Thick1, length(IN.localPos));
			c.rgb			= inCircle1 ? _Color1 : c.rgb;
			
			// Circle 2
			bool inCircle2	= step(length(IN.localPos), r2) && step(r2 - _Thick2, length(IN.localPos));
			c.rgb			= inCircle2 ? _Color2 : c.rgb;

			// Alpha
			c.a				= inCircle2 || inCircle1 ?  1 : 0;
			
			o.Albedo		= c.rgb;
			o.Alpha			= c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

