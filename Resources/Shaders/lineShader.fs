#version 330 core
out vec4 FragColor;

uniform vec3 lineColour;;

void main() {
    FragColor = vec4(lineColour, 1.0); // Bright red
}