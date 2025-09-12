shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_linear_mipmap;

uniform vec2 lens_center = vec2(0.5, 0.5); // in screen UV space
uniform float camera_zoom = 1.0;
uniform float lens_radius = 0.25;          // radius of the circular lens
uniform float lens_power = 1.6;            // <1 = magnify, >1 = shrink
uniform float edge_softness = 1.0;         // fraction of radius used for smoothing

void fragment()
{
	vec2 uv = SCREEN_UV;
    vec2 offset = uv - lens_center;

    // Aspect ratio correction
    vec2 screen_size = 1.0 / SCREEN_PIXEL_SIZE;
    float aspect = screen_size.x / screen_size.y;
    vec2 scaled_offset = vec2(offset.x * aspect, offset.y);

    float r = length(scaled_offset);
	float adjusted_radius = lens_radius * camera_zoom;
	
    if (r < adjusted_radius)
	{
        // Normalized radius [0..1]
        float normalized_r = r / adjusted_radius;

        // Warp curve
        float warped_r = pow(normalized_r, lens_power) * adjusted_radius;

        vec2 warped_offset = scaled_offset;
        if (r > 0.0)
		{
            warped_offset *= warped_r / r;
        }

        // Undo aspect ratio scaling
        warped_offset = vec2(warped_offset.x / aspect, warped_offset.y);

        // Blend warped UV with original UV based on softness
        float t = smoothstep(adjusted_radius, adjusted_radius - edge_softness * adjusted_radius, r);
        vec2 final_offset = mix(offset, warped_offset, t);

        uv = lens_center + final_offset;
    }

    COLOR = texture(screen_texture, uv);
}
