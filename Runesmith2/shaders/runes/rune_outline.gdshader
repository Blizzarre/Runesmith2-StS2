shader_type canvas_item;

uniform float circle_r : hint_range(0.0, 100.0, 0.1) = 10.0;
uniform float circle_thickness : hint_range(0.0, 10.0, 0.1) = 1.0;
uniform float feather : hint_range(0.0, 0.1, 0.001) = 0.005;
uniform vec4 color : source_color = vec4(1.0);
uniform vec2 circle1;
uniform vec2 circle2;
uniform float control_scale;

varying vec2 tex_size;

void vertex() {
	VERTEX += (UV * 2.0 - 1.0) * circle_r * control_scale;
}

float circle(vec2 position, float radius)
{
	return smoothstep(radius, radius + feather, length(position));
}

void fragment() {
	vec2 uv = UV;
	uv -= vec2(0.5);
	uv *= (1.0 + TEXTURE_PIXEL_SIZE * circle_r * 2.0);
	uv += vec2(0.5);
	vec4 sprite_color = texture(TEXTURE, uv);
	
	COLOR.a = max((1.0 - circle(uv - circle1 * TEXTURE_PIXEL_SIZE, circle_r * TEXTURE_PIXEL_SIZE.x)), (1.0 - circle(uv - circle2 * TEXTURE_PIXEL_SIZE, circle_r * TEXTURE_PIXEL_SIZE.x)));;
	COLOR.a = max(sprite_color.a, COLOR.a);
	COLOR.a *= (circle(uv - circle1 * TEXTURE_PIXEL_SIZE, (circle_r - circle_thickness) * TEXTURE_PIXEL_SIZE.x));
	COLOR.a *= (circle(uv - circle2 * TEXTURE_PIXEL_SIZE, (circle_r - circle_thickness) * TEXTURE_PIXEL_SIZE.x));
	COLOR.a *= color.a;
	COLOR.rgb = color.rgb;
}

