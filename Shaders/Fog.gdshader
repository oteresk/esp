shader_type canvas_item;

uniform sampler2D noise_texture: repeat_enable,filter_nearest;

// fog density
uniform float density: hint_range(0.0, 1.0) = 1;
// fog speed
uniform vec2 speed = vec2(0.02 , 0.01);

void fragment() {
	// make the fog slowly move
	vec2 uv=UV + speed * TIME;
	
	// sample noise texture
float noise = texture(noise_texture,uv).r;
// convert noise
float fog = clamp(noise * 2.0 - 1.0, 0.0, 1.0);
// apply fog effect
COLOR.a*=fog * density;

}