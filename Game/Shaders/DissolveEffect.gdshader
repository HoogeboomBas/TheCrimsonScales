shader_type canvas_item;

uniform sampler2D dissolve_map: filter_nearest;

uniform vec4 burn_color : source_color = vec4(1.0, 0.35, 0.05, 1.0);
uniform float burn_width : hint_range(0.001, 0.5) = 0.08;

instance uniform float progress: hint_range(0.0, 1.0);

void fragment()
{
	vec4 color = texture(TEXTURE, UV);
	float dissolve_value = texture(dissolve_map, UV).r;

	// How far past the dissolve threshold we are
	float dissolve_amount = progress * (1.0 + burn_width + 0.02) - burn_width - dissolve_value;

	// Main alpha
	float alpha = 1.0 - smoothstep(0.0, 0.02, dissolve_amount);

	// Burn edge
	float burn = 1.0 - smoothstep(
		0.0,
		burn_width,
		abs(dissolve_amount)
	);

	// Apply burn color
	color.rgb = mix(
		color.rgb,
		burn_color.rgb,
		burn
	);

	// Make the burned area brighter
	color.rgb += burn_color.rgb * burn * 0.5;

	color.a *= alpha;

	COLOR = color;
}