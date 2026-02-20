shader_type canvas_item;
// Shader to round corners of a canvas. The 'radius_scale' is multiplied by
// minimum(width, height)/2.0 to calculate the radius of the corners.
//
// Instructions:
// 1) The node that uses this shader must have signals 'tree_entered' and
// 'item_rect_changed' connected to a callable with the next code:
// material.set_shader_parameter("width", size.x)
// material.set_shader_parameter("height", size.y)
//
// Known issues:
// 1) If used on 'TextureRect', take care of 'expand_mode' and 'stretch_mode',
// because image corners might be outside node rectangle and therefore clipped.
// Corners are rounded, but they are outside node's rectangle.

uniform float radius_scale: hint_range(0.0, 1.0, 0.01) = 0.1;
uniform bool rounded_corner_top_left = true;
uniform bool rounded_corner_top_right = true;
uniform bool rounded_corner_bottom_left = true;
uniform bool rounded_corner_bottom_right = true;

instance uniform vec4 atlas_region; // x, y, width, height in pixels
instance uniform vec2 atlas_size;   // width, height of the full atlas in pixels

varying vec4 modulate;

void vertex()
{
	modulate = COLOR;
}

void fragment() {
	// Convert pixel coordinates to normalized UV for the full atlas
    vec2 full_atlas_uv = UV * atlas_size;

    // Calculate normalized UV for the sub-texture (0 to 1 range)
    vec2 normalized_uv = (full_atlas_uv - atlas_region.xy) / atlas_region.zw;

	float width = atlas_region.z;
	float height = atlas_region.w;

	vec4 image = texture(TEXTURE, UV);
	vec2 pos = vec2(normalized_uv.x*width, normalized_uv.y*height);
	float radius = min(width, height)*radius_scale/2.0;
	float dist;
	// Top left corner
	if (rounded_corner_top_left) {
		dist = length(pos - vec2(radius));
		if (dist > radius && pos.x < radius && pos.y < radius) {
			image.a = 0.0;
		}
	}
	// Top right corner
	if (rounded_corner_top_right) {
		dist = length(pos - vec2(width-radius, radius));
		if (dist > radius && pos.x > width-radius && pos.y < radius) {
			image.a = 0.0;
		}
	}
	// Bottom left corner
	if (rounded_corner_bottom_left) {
		dist = length(pos - vec2(radius, height-radius));
		if (dist > radius && pos.x < radius && pos.y > height-radius) {
			image.a = 0.0;
		}
	}
	// Bottom right corner
	if (rounded_corner_bottom_right) {
		dist = length(pos - vec2(width-radius, height-radius));
		if (dist > radius && pos.x > width-radius && pos.y > height-radius) {
			image.a = 0.0;
		}
	}
	COLOR = image * modulate;
}
