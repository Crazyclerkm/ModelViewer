#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec2 TexCoords;

uniform mat4 world_from_object;
uniform mat4 view_from_world;
uniform mat4 projection_from_view;

void main() {
    TexCoords = aTexCoords;    
    gl_Position = projection_from_view * view_from_world * world_from_object * vec4(aPos, 1.0);
}