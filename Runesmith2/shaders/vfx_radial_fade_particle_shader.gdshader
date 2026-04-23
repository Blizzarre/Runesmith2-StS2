shader_type canvas_item;

uniform sampler2D lut : repeat_disable;

uniform vec2 pivot_offset = vec2(0.0, 0.0);

uniform sampler2D inner_edge_curve;
uniform sampler2D outer_edge_curve;
uniform float smoothness = 0.25;

varying float lifetime;
varying vec4 vertex_color;

void vertex() {
	vertex_color = COLOR;
	lifetime = INSTANCE_CUSTOM.y/INSTANCE_CUSTOM.w;
	
	VERTEX += pivot_offset * (1.0/TEXTURE_PIXEL_SIZE);
}

void fragment() {
	float dist = distance(vec2(0.5), UV);
	vec4 texture_color = texture(TEXTURE, UV);
	
	vec4 lut_color = texture(lut, texture_color.rr);
	
	float inner_edge_curve_val = texture(inner_edge_curve, vec2(lifetime)).r;
	float inner_edge_val = smoothstep(inner_edge_curve_val, inner_edge_curve_val + smoothness, dist);
	
	float outer_edge_curve_val = texture(outer_edge_curve, vec2(lifetime)).r;
	float outer_edge_val = smoothstep(outer_edge_curve_val, outer_edge_curve_val + smoothness, dist);
	
	float alpha = inner_edge_val - outer_edge_val;
	
	COLOR = vec4(lut_color.rgb, alpha * texture_color.a) * vertex_color;
}
