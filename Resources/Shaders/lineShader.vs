#version 330 core
layout (location = 0) in vec3 aPos;

uniform mat4 worldFromObject;
uniform mat4 viewFromWorld;
uniform mat4 projectionFromView;

void main() {
    gl_Position = projectionFromView * viewFromWorld * worldFromObject * vec4(aPos, 1.0);
}