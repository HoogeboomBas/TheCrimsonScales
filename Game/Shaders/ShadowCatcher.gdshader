shader_type spatial;
uniform vec3 shadow_color: source_color; //Color of the shadow
uniform float shadow_opacity: hint_range(0, 1); //Opacity multiplier of the shadow
uniform float shadow_max_opacity: hint_range(0, 1); //Maximum opacity of overlapping shadows

void fragment() {
	// simple single color albedo.
	ALBEDO = shadow_color;
	// start with 0 alpha so we can add multiple light shadows together in light()
	ALPHA = 0.0;
}

void light() {
	// unshaded lighting
	DIFFUSE_LIGHT = ALBEDO;
	// add alpha based on attenuation, which includes if we're in shadow, cap at our max opacity
	ALPHA = min(ALPHA + shadow_opacity*(1.0 - ATTENUATION), shadow_max_opacity);
}