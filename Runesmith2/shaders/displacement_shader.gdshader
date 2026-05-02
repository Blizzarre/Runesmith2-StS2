shader_type canvas_item;

uniform bool shaking       = false;
uniform float amplitude    = 1.0;
uniform float noise_scale_x  = 1.0;
uniform float noise_speed_x  = 10.0;
uniform float noise_scale_y  = 1.0;
uniform float noise_speed_y  = 10.0;
uniform sampler2D noise: filter_nearest, repeat_enable;

varying vec4 modulate_color;

void vertex() {
	modulate_color = COLOR;
}

void fragment() {
	if (shaking) {
		vec2 uv = UV;

		// Generate two noise values to construct a random direction.
		float nx = texture(noise, uv * noise_scale_x + vec2(TIME * noise_speed_x, 0.0)).r;
		float ny = texture(noise, uv * noise_scale_y + vec2(0.0, TIME * noise_speed_y)).g;
		// Map [0,1] to [-1,1]
		vec2 rand_dir = normalize(vec2(nx * 2.0 - 1.0, ny * 2.0 - 1.0));

		// Use another noise value to determine the jitter amplitude sign.
		float n = texture(noise, (uv + vec2(TIME * noise_speed_x, TIME * noise_speed_y)) * vec2(noise_scale_x, noise_scale_y)).b;
		float off = (n * 2.0 - 1.0) * amplitude;

		// Offset along a random direction
		vec2 uv2 = uv + rand_dir * off * TEXTURE_PIXEL_SIZE;
		COLOR = texture(TEXTURE, uv2);
		COLOR *= modulate_color;
	}
}
