﻿#version 330 core

uniform sampler2D samp;

in vec2 fTexCoords;

out vec4 FragColor;

void main() {
    FragColor = texture(samp, fTexCoords);
}