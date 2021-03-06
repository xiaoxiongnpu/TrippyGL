﻿#version 330 core

uniform sampler2D distortMap;
uniform sampler2D normalsMap;

uniform sampler2D reflectSamp;
uniform sampler2D refractSamp;
uniform sampler2D depthSamp;

uniform float nearPlane;
uniform float farPlane;

uniform vec3 sunlightDir;
uniform vec3 sunlightColor;

uniform vec3 cameraPos;
uniform float nearFog;
uniform float farFog;

uniform vec2 distortOffset;

in vec3 fPosition;
in vec4 clipSpace;
in vec2 distortCoords;
in vec3 toCameraVector;
in float waterDepth;
in float aboveWater;

out vec4 FragColor;

const float waveStrenght = 0.02;
const float shineDamper = 64.0;
const float reflectivity = 0.7;

float depthToDistance(in float depth) {
	return 2.0 * nearPlane * farPlane / (farPlane + nearPlane - (2.0 * depth - 1.0) * (farPlane - nearPlane));
}

vec3 calcSpecularReflection(in vec2 distortedTexCoords, in vec3 toCamVec) {
	vec4 normalColor = texture(normalsMap, distortedTexCoords);
	vec3 waterNormal = normalize(vec3(normalColor.r * 2.0 - 1.0, normalColor.b, normalColor.g * 2.0 - 1.0));

	vec3 reflectedLight = aboveWater > 0 ? reflect(sunlightDir, waterNormal) : refract(sunlightDir, -waterNormal, 0.9);
	float specular = max(dot(reflectedLight, toCamVec), 0.0);
	specular = pow(specular, shineDamper);
	return sunlightColor * (specular * reflectivity);
}

void main() {
	float depthyness = clamp((waterDepth-1.0) / 12.0, 0, 1);
	vec2 fragCoords = (clipSpace.xy / clipSpace.w) * 0.5 + 0.5;

	vec2 distortedTexCoords = texture(distortMap, vec2(distortCoords.x + distortOffset.x, distortCoords.y)).xy * 0.1;
	distortedTexCoords = distortCoords + vec2(distortedTexCoords.x, distortedTexCoords.y + distortOffset.y);
	vec2 distort = (texture(distortMap, distortedTexCoords).xy * 2.0 - 1.0) * waveStrenght * depthyness;

	vec4 reflectColor = texture(reflectSamp, vec2(fragCoords.x, 1.0 - fragCoords.y) + distort);
	vec4 refractColor = texture(refractSamp, fragCoords + distort);
	float camWaterDepth = depthToDistance(texture(depthSamp, fragCoords + distort).x) - depthToDistance(gl_FragCoord.z);
	
	vec3 toCamVec = normalize(toCameraVector);

	float refractFactor = max(pow(abs(toCamVec.y), 1.1) - (aboveWater*0.5+0.5) * camWaterDepth/92.0, 0);
	FragColor = mix(reflectColor, refractColor, refractFactor);

	FragColor.xyz += calcSpecularReflection(distortedTexCoords, toCamVec) * depthyness;
    FragColor.a = 1.0 - max((distance(cameraPos, fPosition) - nearFog) / (farFog - nearFog), 0.0);
}