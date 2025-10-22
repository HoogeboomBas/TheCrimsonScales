shader_type spatial;
render_mode blend_mix, depth_draw_opaque, unshaded;
//render_mode blend_mix, depth_draw_never, depth_test_disabled, unshaded;

//uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_linear_mipmap;
uniform sampler2D screen_texture : source_color;
uniform sampler2D height_map : source_color;
uniform vec4 fresnel_color : source_color;
uniform float fresnel_amount = 1;
uniform float distortion = 1;
uniform float animation_speed = 1;
uniform float alpha = 1;

float fresnel(float amount, vec3 normal, vec3 view)
{
	return pow((1.0 - clamp(dot(normalize(normal), normalize(view)), 0.0, 1.0 )), amount);
}

vec2 parallax_mapping_uv_offset_1_step(float height, float amplitude, vec3 view_dir_tangent)
{
    height = height * amplitude - amplitude / 2.0;
    vec3 vector = view_dir_tangent;
    vector.y += 0.42;
    return height * (vector.xz / vector.y);
}

vec2 parallax_mapping_uv(float height, float amplitude, vec2 uv, vec3 tangent, vec3 normal, vec3 binormal, vec3 view)
{
	float depth = amplitude / 10.0;
	mat3 tangent_matrix = mat3(tangent, normal, -binormal); // VIEW TO TANGENT SPACE
	vec3 view_tangent = transpose(tangent_matrix) * view;
	float parallaxHeight = height;//texture(height, uv).r;
    vec2 parallaxOffset = parallax_mapping_uv_offset_1_step(parallaxHeight, depth, view_tangent);
    return parallaxOffset + uv;
}

void fragment()
{
	vec2 n = NORMAL.rg;
	float aspect = VIEWPORT_SIZE.x / VIEWPORT_SIZE.y;
	vec2 scaled_offset = vec2(n.x / aspect, n.y);
	scaled_offset /= abs(VERTEX.z);

	vec2 heightmap_uv = UV + vec2(0, TIME * animation_speed);
	vec2 parallaxUV = parallax_mapping_uv(texture(height_map, heightmap_uv).r, 2.0, UV, TANGENT, NORMAL, BINORMAL, VIEW);

	// final distortion
	vec2 final_uv = parallaxUV + scaled_offset * distortion;

	ALBEDO = texture(screen_texture, final_uv).rgb;

	float basic_fresnel = fresnel(fresnel_amount, NORMAL, VIEW);
	ALBEDO = mix(ALBEDO, fresnel_color.rgb, basic_fresnel);
	ALBEDO += basic_fresnel * 0.2;

	ALPHA = mix(alpha, 1.0, basic_fresnel);
}
