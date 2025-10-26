// NOTE: Shader automatically converted from Godot Engine 4.4.1.stable.mono's StandardMaterial3D.

shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_disabled, unshaded;

uniform float progress : hint_range(0.0, 1.0, 0.001);

uniform vec4 albedo : source_color;
uniform sampler2D texture_albedo : source_color, filter_linear_mipmap, repeat_disable;
uniform sampler2D back_albedo : source_color, filter_linear_mipmap, repeat_disable;
uniform float alpha_scissor_threshold : hint_range(0.0, 1.0, 0.001);
uniform ivec2 albedo_texture_size;
uniform float point_size : hint_range(0.1, 128.0, 0.1);

uniform float roughness : hint_range(0.0, 1.0);
uniform sampler2D texture_metallic : hint_default_white, filter_linear_mipmap, repeat_enable;
uniform vec4 metallic_texture_channel;
uniform sampler2D texture_roughness : hint_roughness_r, filter_linear_mipmap, repeat_enable;

uniform float specular : hint_range(0.0, 1.0, 0.01);
uniform float metallic : hint_range(0.0, 1.0, 0.01);

uniform vec3 uv1_scale;
uniform vec3 uv1_offset;
uniform vec3 uv2_scale;
uniform vec3 uv2_offset;

void vertex()
{
	UV = UV * uv1_scale.xy + uv1_offset.xy;

	if(UV.x > progress)
	{
		float t = (1.0 - UV.x + progress + 1.0) * 50.0;// + progress * 20.0;
		float r = 0.0025;

		float yOffset = 0.03;
		VERTEX.y = yOffset + (r / (2.0 * 3.1415)) * t * (sin(t));

		float offset = -(1.563 * 0.5) + progress * 1.563 + 0.03;
		VERTEX.x = offset - (r / (2.0 * 3.1415)) * t * (cos(t));

		UV.x *= 1.02;

		//vec3 normalVector = VERTEX - vec3(offset, 0, VERTEX.z);
		//normalVector = normalize(normalVector);
		//NORMAL = normalVector;
		//NORMAL = (MODELVIEW_MATRIX  * vec4(normalVector, 0.0)).xyz;
		//NORMAL.xyz = vec3(0.0, 0.0, 1.0);
	}
}

void fragment()
{
	vec2 base_uv = UV;

	vec4 albedo_tex;
	if(FRONT_FACING)
	{
		albedo_tex = texture(texture_albedo, base_uv);
	}
	else
	{
		albedo_tex = texture(back_albedo, base_uv);
	}
	ALBEDO = albedo.rgb * albedo_tex.rgb;

	float metallic_tex = dot(texture(texture_metallic, base_uv), metallic_texture_channel);
	METALLIC = metallic_tex * metallic;
	SPECULAR = specular;

	vec4 roughness_texture_channel = vec4(1.0, 0.0, 0.0, 0.0);
	float roughness_tex = dot(texture(texture_roughness, base_uv), roughness_texture_channel);
	ROUGHNESS = roughness_tex * roughness;
	ALPHA *= albedo.a * albedo_tex.a;
	ALPHA_SCISSOR_THRESHOLD = alpha_scissor_threshold;
}
