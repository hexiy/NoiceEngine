[VERTEX]
#version 460 core

layout(location = 0) in vec4 position;

layout(location = 1) in vec4 aTexCoord;

out vec4 texCoord;
out vec4 frag_color;
uniform mat4 u_mvp = mat4(1.0);

void main(void)
{

    texCoord = aTexCoord;

    gl_Position = u_mvp * position;
}

[FRAGMENT]
#version 460 core
in vec4 texCoord;
uniform sampler2D textureObject;
uniform vec4 u_color;
uniform vec2 u_resolution;
uniform vec2 u_offset;
uniform vec2 u_scale;
layout(location = 0) out vec4 color;

void main(void)
{
    vec2 pixelSize = vec2(1.0 / u_resolution.x, 1.0 / u_resolution.y);
    vec4 texColor = texture(textureObject, vec2(texCoord.x + pixelSize.x * u_offset.x, texCoord.y + pixelSize.y * u_offset.y) * (pixelSize * u_scale)) * u_color;
    if (texColor.a < 0.1)
    {
	  discard;
    }
    else
    {
	  color = texColor;
    }
}