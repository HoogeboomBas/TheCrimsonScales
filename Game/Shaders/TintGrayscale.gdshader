shader_type canvas_item;

instance uniform float grayscaleFactor = 0.0;

varying vec4 modulate;

void vertex()
{
	modulate = COLOR;
}

void fragment()
{
	COLOR = texture(TEXTURE, UV);

	vec3 grayscale = vec3(0.2126 * COLOR.r + 0.7152 * COLOR.g + 0.0722 * COLOR.b);

	COLOR.rgb = mix(COLOR.rgb, grayscale, grayscaleFactor);
	COLOR *= modulate;
}
