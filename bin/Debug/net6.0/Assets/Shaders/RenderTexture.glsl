[BUFFERTYPE:RENDERTEXTURE]
[VERTEX]
#version 460 core

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 aTexCoord;

uniform mat4 u_mvp = mat4(1.0);

out vec4 texCoord;
out vec4 frag_color;

void main(void)
{
    texCoord = aTexCoord;

    gl_Position = u_mvp * position;
}

[FRAGMENT]
#version 460 core
in vec4 texCoord;
uniform sampler2D textureObject;
layout(location = 0) out vec4 color;
uniform vec2 u_resolution;

void main(void)
{
    vec4 texColor = texture(textureObject, vec2(texCoord.x, texCoord.y));
    color = texColor;
}