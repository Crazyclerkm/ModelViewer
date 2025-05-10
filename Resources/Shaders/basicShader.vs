#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec3 Position;
out vec3 Normal;
out vec2 TexCoords;

uniform mat4 worldFromObject;
uniform mat4 viewFromWorld;
uniform mat4 projectionFromView;

void main() {
    Position = vec3(worldFromObject * vec4(aPosition, 1.0));
    TexCoords = aTexCoords;
    Normal = aNormal;
    
    gl_Position = projectionFromView * viewFromWorld * worldFromObject * vec4(aPosition, 1.0);
}