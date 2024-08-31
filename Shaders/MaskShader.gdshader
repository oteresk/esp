shader_type canvas_item;

uniform sampler2D mask;
uniform float alpha;

void fragment() {
	COLOR = texture(TEXTURE, UV);
	vec2 muv=UV;
	muv.y=1.0-muv.y;
	COLOR.a=texture(mask,muv).r*alpha;
	
	
}
