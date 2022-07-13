[BUFFERTYPE:BOX]
[VERTEX]
#version 460 core

layout(location = 0) in vec4 position;
uniform mat4 u_mvp = mat4(1.0);
out vec4 frag_color;
uniform vec4 u_color;
void main(void)
{
    gl_Position = u_mvp * position;
    frag_color = u_color;
}

[FRAGMENT]
#version 460 core
in vec4 frag_color;
out vec4 color;

void main(void)
{
    color = frag_color;
}