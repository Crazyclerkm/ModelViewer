#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec2 TexCoords;

uniform mat4 worldFromObject;
uniform mat4 viewFromWorld;
uniform mat4 projectionFromView;

void main() {
    TexCoords = aTexCoords;    
    gl_Position = projectionFromView * viewFromWorld * worldFromObject * vec4(aPos, 1.0);
}