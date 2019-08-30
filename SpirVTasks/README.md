<h1 align="center">
  SpirVTasks MSBuild add-on
  <br>  
<p align="center">
  <a href="https://www.nuget.org/packages/SpirVTasks">
    <img src="https://buildstats.info/nuget/SpirVTasks">
  </a>
  <a href="https://www.paypal.me/GrandTetraSoftware">
    <img src="https://img.shields.io/badge/Donate-PayPal-green.svg">
  </a>  
</p>
</h1>

**SpirVTasks** package add **SpirV** compilation support to msbuild project. Error and warning
are transmited to the **IDE**.

Add glsl files to your project with the **`<GLSLShader>`** tag.

```xml
<ItemGroup>    
  <GLSLShader Include="shaders\*.frag;shaders\*.vert;shaders\*.comp;shaders\*.geom" />
</ItemGroup> 
```

Resulting `.spv` files will be embedded with the resource ID = ProjectName.file.ext.spv.
You can override the resource name by adding a **`<LogicalName>`** element.

```xml
<ItemGroup>    
  <GLSLShader Include="shaders\skybox.vert">
	  <LogicalName>NewName.vert.spv</LogicalName>
  </GLSLShader>
</ItemGroup> 
```
**glslc** executable is searched with the help of the **VULKAN_SDK** or **PATH** environments variables.
You may also point to glslc with the **`<SpirVglslcPath>`** property (>= 0.1.7), but if the property is set.

```xml
<PropertyGroup>
  <SpirVglslcPath>bin\glslc.exe</SpirVglslcPath>
</PropertyGroup>
```

I've added an **include** mechanism for glsl, file are searched from the location of the current parsed file,
then in the **`<SpirVAdditionalIncludeDirectories>`** directories property.

```xml
<PropertyGroup>
  <SpirVAdditionalIncludeDirectories>$(MSBuildThisFileDirectory)common\</SpirVAdditionalIncludeDirectories>
</PropertyGroup>
```

```glsl
#include <preamble.inc>

layout (location = 0) in vec3 inColor;
layout (location = 0) out vec4 outFragColor;

void main() 
{
    outFragColor = vec4(inColor, 1.0);
}
```

TODO:

- Error source file and line with included files.