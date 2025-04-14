#version 330 core
out vec4 OutColour;

in vec2 FragUV;
in vec4 FragColour;

uniform sampler2D Texture;

void main() {
    OutColour = FragColour * texture(Texture, FragUV);
}