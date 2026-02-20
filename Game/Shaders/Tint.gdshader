shader_type canvas_item;

instance uniform float tintFactor : hint_range(0.0, 1.0, 0.001);

varying vec4 modulate;

void vertex()
{
	modulate = COLOR;
}

void fragment()
{
	COLOR = texture(TEXTURE, UV);

	COLOR.rgb = mix(COLOR.rgb, modulate.rgb, tintFactor);
	COLOR.a *= modulate.a;
}
