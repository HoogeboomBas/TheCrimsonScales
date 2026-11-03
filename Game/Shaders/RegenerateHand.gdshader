shader_type canvas_item;

uniform sampler2D lines : source_color;

uniform float progress : hint_range(0.0, 1.0, 0.001);

void vertex()
{
}

void fragment()
{
	COLOR = texture(TEXTURE, UV);

	float lineV = -1.0 + progress;
	COLOR.a *= 1.0 - texture(lines, UV + vec2(0.0, lineV)).a;
}
