namespace Engine;

public static class ShaderCache
{
	public static Shader spriteRendererShader;
	public static Shader boxRendererShader;
	public static Shader renderTextureShader;
	public static Shader renderTexturePostProcessShader;
	public static Shader renderTextureBloomShader;

	public static int shaderInUse = -1;


	public static void CreateShaders()
	{
		CreateSpriteRendererShader();
		CreateBoxRendererShader();
		CreateRenderTextureShader();
		CreateRenderTexturePostProcessShader();
		CreateRenderTextureBloomShader();
	}
	private static void CreateRenderTextureShader()
	{
		string vertexShader = @"#version 460 core

layout(location = 0) in vec4 position;

layout(location = 1) in vec4 aTexCoord;

out vec4 texCoord;
out vec4 frag_color;
uniform mat4 u_mvp = mat4(1.0);
void main(void)
{
    texCoord = aTexCoord;

    gl_Position = u_mvp * position;
}";

		string fragmentShader = @"#version 450 core
in vec4 texCoord;
uniform sampler2D textureObject;
layout(location = 0) out vec4 color;
uniform vec2 u_resolution = vec2(1000,500);

void main(void)
{
vec4 texColor =texture(textureObject, vec2(texCoord.x, texCoord.y));
 color = texColor;
}";

		renderTextureShader = new Shader(vertexShader, fragmentShader);

		renderTextureShader.Load();
	}
	private static void CreateRenderTextureBloomShader()
	{
		string vertexShader = @"#version 460 core

layout(location = 0) in vec4 position;

layout(location = 1) in vec4 aTexCoord;

out vec4 texCoord;
out vec4 frag_color;
uniform mat4 u_mvp = mat4(1.0);

void main(void)
{
    texCoord = aTexCoord;

    gl_Position = u_mvp * position;
}";

		string fragmentShader = @"#version 450 core
in vec4 texCoord;
uniform sampler2D textureObject;
layout(location = 0) out vec4 color;
uniform vec2 u_resolution = vec2(1000,500);

void main(void)
{
vec4 texColor =texture(textureObject, vec2(texCoord.x, texCoord.y));

//vec2 relativePosition = texCoord.xy/vec2(1,1) - 0.5;
//float len= length(relativePosition);
//float vignette = smoothstep(.9,.2,len);
//texColor.rgb=mix(texColor.rgb,texColor.rgb*vignette, .7);

//texColor.a=0.7;


// bloom

float bloom= 0;
vec4 bloomColor= vec4(1,1,1,1);
float bloomStrength = 5;

for(int x=-50;x<50;x++)
{
for(int y=-50;y<50;y++)
{
vec4 pixelColor =texture(textureObject, vec2(texCoord.x+(1.0/u_resolution.x)*x, texCoord.y+(1.0/u_resolution.y)*y));

if(pixelColor.x > 0.2 || pixelColor.y >0.2 || pixelColor.z > 0.2 && pixelColor.w > 0){
bloom+= length(pixelColor)*0.00001*bloomStrength;

bloomColor= (bloomColor + pixelColor);

}
}
}
if(bloom >0){
texColor =vec4(normalize(bloomColor).rgb,1)*bloom;
//texColor = normalize(bloomColor)*bloom;
texColor.a = 1f;
 color = texColor;
}

}";

		renderTextureBloomShader = new Shader(vertexShader, fragmentShader);

		renderTextureBloomShader.Load();
	}
	private static void CreateRenderTexturePostProcessShader()
	{
		string vertexShader = @"#version 460 core

layout(location = 0) in vec4 position;

layout(location = 1) in vec4 aTexCoord;

out vec4 texCoord;
out vec4 frag_color;
uniform mat4 u_mvp = mat4(1.0);
void main(void)
{
    texCoord = aTexCoord;

    gl_Position = u_mvp * position;
}";

		string fragmentShader = @"#version 450 core
in vec4 texCoord;
uniform sampler2D textureObject;
layout(location = 0) out vec4 color;
uniform vec2 u_resolution = vec2(1000,500);

void main(void)
{
vec4 texColor =texture(textureObject, vec2(texCoord.x, texCoord.y));

vec2 relativePosition = texCoord.xy/vec2(1,1) - 0.5;
float len= length(relativePosition);
float vignette = smoothstep(.9,.2,len);
texColor.rgb=mix(texColor.rgb,texColor.rgb*vignette, .7);

texColor.a=1;

 color = texColor;
}";

		renderTexturePostProcessShader = new Shader(vertexShader, fragmentShader);

		renderTexturePostProcessShader.Load();
	}
	private static void CreateSpriteRendererShader()
	{
		string vertexShader = @"#version 460 core

layout(location = 0) in vec4 position;

layout(location = 1) in vec4 aTexCoord;

out vec4 texCoord;
out vec4 frag_color;
uniform mat4 u_mvp = mat4(1.0);

void main(void)
{

    texCoord = aTexCoord;

    gl_Position = u_mvp * position;
}";

		string fragmentShader = @"#version 450 core
in vec4 texCoord;
uniform sampler2D textureObject;
uniform vec4 u_color;
layout(location = 0) out vec4 color;

void main(void)
{
vec4 texColor =texture(textureObject, vec2(texCoord.x, texCoord.y)) * u_color;
if(texColor.a < 0.1){
        discard;
}
else{
 color = texColor;
}
}";

		spriteRendererShader = new Shader(vertexShader, fragmentShader);

		spriteRendererShader.Load();
	}
	private static void CreateBoxRendererShader()
	{
		string vertexShader = @"#version 460 core

layout(location = 0) in vec4 position;
uniform mat4 u_mvp = mat4(1.0);
out vec4 frag_color;
uniform vec4 u_color;
void main(void)
{
gl_Position = u_mvp * position;
frag_color = u_color;
}";

		string fragmentShader = @"#version 460 core
in vec4 frag_color;
out vec4 color;

void main(void)
{
color = frag_color;
}";

		boxRendererShader = new Shader(vertexShader, fragmentShader);
		boxRendererShader.Load();
	}
	public static void UseShader(Shader shader)
	{
		if (shader.ProgramID == shaderInUse) { return; }
		shaderInUse = shader.ProgramID;
		GL.UseProgram(shader.ProgramID);
	}
}
