#version 330 core
out vec4 FragColour;

in vec3 Position;
in vec3 Normal;
in vec2 TexCoords;

struct Material {
    sampler2D diffuseMap;
    sampler2D specularMap;

    float shininess;
};

struct DirLight {
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct PointLight {
    vec3 position;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct SpotLight {
    vec3 position;
    vec3 direction;

    float cutOff;
    float outerCutOff;

    float constant;
    float linear;
    float quadratic;
    
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

#define MAX_LIGHTS 64

uniform PointLight pointLights[MAX_LIGHTS];
uniform DirLight dirLights[MAX_LIGHTS];
uniform SpotLight spotLights[MAX_LIGHTS];

uniform int numPointLights;
uniform int numDirLights;
uniform int numSpotLights;

uniform Material material;
uniform sampler2D texture0;

uniform vec3 viewPos;

vec3 CalculateDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalculatePointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main() {
    vec3 viewDir = normalize(viewPos - Position);
    vec3 norm = normalize(Normal);
    
    vec3 result = vec3(0.0, 0.0, 0.0);

    for(int i=0; i<numDirLights; i++) {
        result += CalculateDirLight(dirLights[i], norm, viewDir);
    }

    for(int i=0; i<numPointLights; i++) {
        result += CalculatePointLight(pointLights[i], norm, Position, viewDir);
    }

    FragColour = vec4(result, 1.0);
}

vec3 CalculateDirLight(DirLight light, vec3 normal, vec3 viewDir) {
    vec3 lightDir = normalize(-light.direction);

    vec3 ambient = light.ambient * vec3(texture(material.diffuseMap, TexCoords));

    float diff = max(dot(normal, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuseMap, TexCoords));

    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular * spec * vec3(texture(material.specularMap, TexCoords));

    return (ambient + diffuse + specular);
}

vec3 CalculatePointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir) {
    vec3 lightDir = normalize(light.position - fragPos);
    
    vec3 ambient = light.ambient * vec3(texture(material.diffuseMap, TexCoords));

    float diff = max(dot(normal, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuseMap, TexCoords));

    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular * spec * vec3(texture(material.specularMap, TexCoords));

    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + 
                        light.linear * distance + 
                        light.quadratic * (distance * distance));

    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;

    return (ambient + diffuse + specular);
}
