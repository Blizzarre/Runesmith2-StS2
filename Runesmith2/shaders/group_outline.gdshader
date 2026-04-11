shader_type canvas_item;

uniform vec3 line_colour: source_color = vec3(1.0);
uniform int line_thickness: hint_range(0, 10) = 1;
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

void fragment() {
	vec2 size = SCREEN_PIXEL_SIZE * float(line_thickness);
	float line_alpha = 1.0;

	vec4 colour = textureLod(screen_texture, SCREEN_UV, 2.0);
	vec4 outline = vec4(line_colour, min(line_alpha, 1.0));
	COLOR *= mix(outline, colour, colour.a);
}