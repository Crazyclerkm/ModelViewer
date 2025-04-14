#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aUV;
layout (location = 2) in vec4 aFragColour;

out vec2 FragUV;
out vec4 FragColour;

uniform mat4 projection;

void main() {
    FragUV = aUV;
    FragColour = aFragColour;    
    gl_Position = projection * vec4(aPos, 0.0, 1.0);
}