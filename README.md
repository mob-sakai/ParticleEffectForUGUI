Particle Effect For UGUI (UI Particle)
===

This plugin provide a component to render particle effect for uGUI in Unity 2018.2 or later.  
The particle rendering is maskable and sortable, without Camera, RenderTexture or Canvas.  

![](https://img.shields.io/badge/Editor-2018.2+-4796F2?logo=unity)
[![](https://img.shields.io/npm/v/com.coffee.ui-particle?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.coffee.ui-particle/)
[![](https://img.shields.io/github/v/release/mob-sakai/ParticleEffectForUGUI?include_prereleases&sort=semver)](https://github.com/mob-sakai/ParticleEffectForUGUI/releases)
[![](https://img.shields.io/github/v/release/mob-sakai/ParticleEffectForUGUI?include_prereleases&sort=semver&filter=3.%2A&label=release(3.x))](https://github.com/mob-sakai/ParticleEffectForUGUI/releases)
[![](https://img.shields.io/github/release-date/mob-sakai/ParticleEffectForUGUI.svg)](https://github.com/mob-sakai/ParticleEffectForUGUI/releases)  [![](https://img.shields.io/github/license/mob-sakai/ParticleEffectForUGUI.svg)](https://github.com/mob-sakai/ParticleEffectForUGUI/blob/master/LICENSE.txt)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-orange.svg)](http://makeapullrequest.com)


<< [Description](#Description) | [Demo](#demo) | [Installation](#installation) | [Usage](#usage) | [Development Note](#development-note) | [Change log](https://github.com/mob-sakai/ParticleEffectForUGUI/blob/upm/CHANGELOG.md) >>



<br><br><br><br>

## Description

![](https://user-images.githubusercontent.com/12690315/41771577-8da4b968-7650-11e8-9524-cd162c422d9d.gif)

This plugin uses new APIs `MeshBake/MashTrailBake` (added with Unity 2018.2) to render particles by CanvasRenderer.
You can mask and sort particles for uGUI without Camera, RenderTexture, Canvas.

Compares this "Baking mesh" approach with the conventional approach:  
[Performance test results](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/193#issuecomment-1160028374)

|Approach|Good|Bad|Screenshot|
|-|-|-|-|
|Baking mesh<br>**\(UIParticle\)**|Rendered as is.<br>Maskable.<br>Sortable.<br>Less objects.|[Not support `TEXCOORD*.zw` components for custom vertex stream](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/191#issuecomment-1043409186)|<img src="https://user-images.githubusercontent.com/12690315/41765089-0302b9a2-763e-11e8-88b3-b6ffa306bbb0.gif" width="500px">|
|Do nothing|Rendered as is.|**Looks like a glitch.**<br>Not maskable.<br>Not sortable.|<img src="https://user-images.githubusercontent.com/12690315/41765090-0329828a-763e-11e8-8d8a-f1d269ea3bc7.gif" width="500px">|
|Convert particle to UIVertex<br>[\(UIParticleSystem\)](https://forum.unity.com/threads/free-script-particle-systems-in-ui-screen-space-overlay.406862/)|Maskable.<br>Sortable.<br>Less objects.|**Adjustment is difficult.**<br>Requires UI shaders.<br>Difficult to adjust scale.<br>Force hierarchy scalling.<br>Simulation results are incorrect.<br>Trail, rotation of transform, time scaling are not supported.<br>Generate heavy GC every frame.|<img src="https://user-images.githubusercontent.com/12690315/41765088-02deb9c6-763e-11e8-98d0-9e0c1766ef39.gif" width="500px">|
|Use Canvas to sort|Rendered as is.<br>Sortable.|**You must to manage sorting orders.**<br>Not maskable.<br>More batches.|<img src="https://user-images.githubusercontent.com/12690315/41765087-02b866ea-763e-11e8-8c33-081c9ad852f8.gif" width="500px">|
|Use RenderTexture|Maskable.<br>Sortable.|**Requires Camera and RenderTexture.**<br>Difficult to adjust position and size.<br>Quality depends on the RenderTexture's setting.|<img src="https://user-images.githubusercontent.com/12690315/41765085-0291b3e2-763e-11e8-827b-72e5ee9bc556.gif" width="500px">|

|Approach|FPS on Editor|FPS on iPhone6|FPS on Xperia XZ|
|--|--|--|--|
|Particle System|43|57|22|
|UIParticleSystem|4|3|0 (unmeasurable)|
|Sorting By Canvas|43|44|18|
|UIParticle|17|12|4|
|UIParticle with MeshSharing|44|45|30|

<br><br>

#### Features

* Easy to use: the package is out-of-the-box
* Sort particle effects and UI by sibling index
* No Camera, RenderTexture or Canvas are required
* Masking by Mask or RectMask2D
* Support Trail module
* Support CanvasGroup alpha
* No allocations
* Support overlay, camera space and world space
* Support Universal Render Pipeline (URP) and High Definition Render Pipeline (HDRP)
* Support disabling `Enter Play Mode Options > Reload Domain`
* Support changing material property with AnimationClip (AnimatableProperty)  
![](https://user-images.githubusercontent.com/12690315/53286323-2d94a980-37b0-11e9-8afb-c4a207805ff2.gif)
* [4.0.0+] Support 8+ materials
* [4.0.0+] Correct world space particle position when changing window size for standalone platforms (Windows, MacOSX and Linux)
* [4.0.0+] Adaptive scaling for UI
* [4.0.0+] Mesh sharing group to improve performance  
![](https://user-images.githubusercontent.com/12690315/174311048-c882df81-6c34-4eba-b0aa-5645457692f1.gif)
* [4.0.0+] Particle attractor component  
![](https://user-images.githubusercontent.com/12690315/174311027-462929a4-13f0-4ec4-86ea-9c832f2eecf1.gif)
* [4.1.0+] Relative/Absolute particle position mode  
![](https://user-images.githubusercontent.com/12690315/175751579-5a2357e8-2ecf-4afd-83c8-66e9771bde39.gif)


<br><br><br><br>

## Demo

* [WebGL Demo](https://mob-sakai.github.io/demos/UIParticle_Demo/index.html)  
> ![](https://user-images.githubusercontent.com/12690315/174311768-1843a5f2-f776-491b-aaa8-2a131a8b6a16.gif)
* [WebGL Demo (Cartoon FX & War FX)](https://mob-sakai.github.io/Demos/ParticleEffectForUGUI_CFX)
  * [Cartoon FX Free][CFX] & [War FX][WFX] (by [Jean Moreno (JMO)][JMO]) with UIParticle
> ![](https://user-images.githubusercontent.com/12690315/91664766-3e07ac00-eb2c-11ea-978b-ef723be80619.gif)

[CFX]: https://assetstore.unity.com/packages/vfx/particles/cartoon-fx-free-109565
[WFX]: https://assetstore.unity.com/packages/vfx/particles/war-fx-5669
[JMO]: https://assetstore.unity.com/publishers/1669


<br><br><br><br>

## Installation

### Requirement

Unity 2018.2 or later

### Install via OpenUPM

This package is available on [OpenUPM](https://openupm.com).
It's recommended to install it via [openupm-cli](https://github.com/openupm/openupm-cli).
```
openupm add com.coffee.ui-particle
```

### Install via Package Manager Window (using Git URL)

1. Select `Window/Package Manager` menu to open `Package Manager` window.
2. Click `+` and `Install package from git URL...`  
![](https://github.com/mob-sakai/ParticleEffectForUGUI/assets/12690315/57317aa4-e55c-4568-b2aa-2ee2a78ddc02)
3. Input `https://github.com/mob-sakai/ParticleEffectForUGUI.git` and click `Install`  
![](https://github.com/mob-sakai/ParticleEffectForUGUI/assets/12690315/72cc38b5-cb32-4c85-8209-c85f7bb931ea)


Or, use [UpmGitExtension](https://github.com/mob-sakai/UpmGitExtension) to install and update the package.

### Install via manifest.json (using Git URL)

1. Open `Packages/manifest.json` in your project.
2. Add this line below `"dependencies": {`
  * `"com.coffee.ui-particle": "https://github.com/mob-sakai/ParticleEffectForUGUI.git",`  
  ![](https://github.com/mob-sakai/ParticleEffectForUGUI/assets/12690315/fe1cad9c-8d09-4e17-b6f5-9eac0d267921)
3. To update the package, change suffix `#{version}`.
  * `"com.coffee.ui-particle": "https://github.com/mob-sakai/ParticleEffectForUGUI.git#4.2.0",`

### Install as an embed package

1. Download a source code zip file from [Releases](https://github.com/mob-sakai/ParticleEffectForUGUI/releases) page
2. Extract it and place it under `Packages` directory in your project.  
![](https://github.com/mob-sakai/ParticleEffectForUGUI/assets/12690315/4c3d3439-5bb0-4e87-a917-ebe328ef89a8)


<br><br><br><br>

## How to play demo

### For Unity 2019.1 or later

1. Open `Package Manager` window
2. Select `UI Particle` package in package list
3. Click `Import Sample` button  
![demo](https://user-images.githubusercontent.com/12690315/95017806-83bd1480-0696-11eb-8c24-c56f45ab1ac2.png)
4. The demo project is imported into `Assets/Samples/UI Particle/{version}/Demo`
5. Open `UIParticle_Demo` scene and play it

### For Unity 2018.4 or earlier

1. Select `Assets/Samples/UI Particle Demo` from menu
2. The demo project is imported into `Assets/Samples/UI Particle/{version}/Demo`
3. Open `UIParticle_Demo` scene and play it

### About `Cartoon FX & War Fx Demo`

* It requires free assets ([Cartoon FX Free][CFX] & [War FX][WFX])
  * by [Jean Moreno (JMO)][JMO]



<br><br><br><br>

## Usage

### UIParticle component

`UIParticle` controls the ParticleSystems that is attached to its own game objects and child game objects.

![](https://github.com/mob-sakai/ParticleEffectForUGUI/assets/12690315/3559df45-63e7-4c4c-9233-f455779efa29)

| Properties | Description |
| -- | -- |
| Maskable | Does this graphic allow masking. |
| Scale | Scale the rendering.<br>When the `3D` toggle is enabled, 3D scale (x,y,z) is supported. |
| Animatable Properties | If you want update material properties (e.g. `_MainTex_ST`, `_Color`) in AnimationClip, use this to mark the changes. |
| Mesh Sharing | Particle simulation results are shared within the same group.<br>A large number of the same effects can be displayed with a small load.<br>When the `Random` toggle is enabled, it will be grouped randomaly. |
| Position Mode | **Absolute:** Emit from the world position of the `ParticleSystem`.<br>**Relative:** Emit from the scaled position of the `ParticleSystem`. |
| Auto Scaling | Transform.lossyScale (=world scale) will be set to `(1, 1, 1)` on update.<br>It prevents the root-Canvas scale from affecting the hierarchy-scaled `ParticleSystem`. |
| Rendering Order | The ParticleSystem list to be rendered.<br>You can change the order and the materials. |

NOTE: Press `Refresh` button to reconstruct rendering order based on children ParticleSystem's sorting order and z position.

<br><br>

### Basically usage

1. Select `Game Object/UI/ParticleSystem` to create UIParticle with a ParticleSystem.  
![particle](https://user-images.githubusercontent.com/12690315/95007361-cad0e880-0649-11eb-8835-f145d62c5977.png)
2. Adjust the ParticleSystem as you like.  
![particle1](https://user-images.githubusercontent.com/12690315/95007359-ca385200-0649-11eb-8383-627c9750bda8.png)

<br><br>

### With your existing ParticleSystem prefab

1. Select `Game Object/UI/ParticleSystem (Empty)` to create UIParticle.  
![empty](https://user-images.githubusercontent.com/12690315/95007362-cb697f00-0649-11eb-8a09-29b0a13791e4.png)
2. Drag & drop your ParticleSystem prefab on UIParticle.  
![particle3](https://user-images.githubusercontent.com/12690315/95007356-c6a4cb00-0649-11eb-9316-562f4bce3f31.png)

<br><br>

### With `Mask` or `RectMask2D` component

If you want to mask particles, set a stencil supported shader (such as `UI/UIAdditive`) to material for ParticleSystem.
If you use some custom shaders, see [How to make a custom shader to support Mask/RectMask2D component](#how-to-make-a-custom-shader-to-support-maskrectmask2d-component) section.

![](https://user-images.githubusercontent.com/12690315/95017591-3b512700-0695-11eb-864e-04166ea1809a.png)


<br><br>

### Script usage

```cs
// Instant ParticleSystem prefab with UIParticle on runtime.
var go = GameObject.Instantiate(prefab);
var uiParticle = go.AddComponent<UIParticle>();

// Control by ParticleSystem.
particleSystem.Play();
particleSystem.Emit(10);

// Control by UIParticle.
uiParticle.Play();
uiParticle.Stop();
```

<br><br>

### UIParticleAttractor component

`UIParticleAttractor` attracts particles generated by the specified ParticleSystem.

![](https://github.com/mob-sakai/ParticleEffectForUGUI/assets/12690315/ea6ae0ed-f9a8-437c-8baa-47526303391e)  
![](https://user-images.githubusercontent.com/12690315/174311027-462929a4-13f0-4ec4-86ea-9c832f2eecf1.gif)

| Properties | Description |
| -- | -- |
| Particle System | Attracts particles generated by the specified particle system. |
| Distination Radius | Once the particle is within the radius, the particle lifetime will become 0 and `OnAttracted` will be called. |
| Delay Rate | Delay to start attracting.<br>It is a percentage of the particle's start lifetime. |
| Max Speed | Maximum speed of attracting.<br> If this value is too small, attracting may not be completed by the end of the lifetime and `OnAttracted` may not be called. |
| Movement | Attracting movement type. (Linear, Smooth, Sphere) |
| Update Mode | **Normal:** Update with scaled delta time.<br>**Unscaled Time:** Update with unscaled delta time. |
| OnAttracted | An event called when attracting is complete (per particle). |

<br><br><br><br>

## Development Note

### FAQ: Why are my particle effects not displayed correctly?

If `ParticleSystem` alone displays particles correctly but `UIParticle` does not, please check the following points:

* [Shader Limitation](https://github.com/mob-sakai/ParticleEffectForUGUI#shader-limitation)
  * Most cases can be solved by using `UI/Additive` or `UI/Default`.
* Particles are masked
  * `UIParticle` is maskable.
  * Set `Mask` or `RectMask2D` component properly.
* Particles are too small
  * If particles are small enough, they will not appear on the screen.
  * Increase the `Scale` value.
  * If you don't want to change the apparent size depending on the resolution, try `Auto Scaling` option.
* Particles are too many
  * No more than 65535 vertices can be displayed (for mesh combination limitations).
  * Please set `Emission` module and `Max Particles` of ParticleSystem properly.
* Particles are emitted off-screen.
  * When `Position Mode = Relative`, particles are emitted from scaled position of the ParticleSystem, not from the screen point of the ParticleSystem.
  * Place the ParticleSystem in the proper position or try `Position Mode = Absolute`.
* Attaching `UIParticle` to the same object as `ParticleSystem`
  * `Transform.localScale` will be overridden by `Auto Scaling` option.
  * It is recommended to place `ParticleSystem` under `UIParticle`.


<br>

### Shader Limitation

The use of UI shaders is recommended.

- If you need a simple Additive shader, use the `UI/Additive` shader instead.
- If you need a simple alpha-blend shader, use the `UI/Default` shader instead.
- If your custom shader does not work properly with UIParticle, consider creating a custom UI shader.

#### Built-in shaders are not supported

UIParticle does not support all built-in shaders except for `UI/Default`.  
If their use is detected, an error is displayed in the inspector.  
Use UI shaders instead.

#### UV.zw components will be discarded

UIParticleRenderer renders the particles based on UIVertex.  
Therefore, only xy components is available for each UV in the shader. (zw components will be discarded).  
So unfortunately UIParticles will not work well with some shaders.  

#### Custom vertex streams

When using custom vertex streams, you can fill zw components with "unnecessary" data.  
https://github.com/mob-sakai/ParticleEffectForUGUI/issues/191


<br>

### Overheads

UIParticle has some overheads and the batching depends on uGUI.  
When improving performance, keep the following in mind:
- If you are displaying a large number of the same effect, consider `Mesh Sharing` feature in [UIParticle Component](#uiparticle-component).
  - If you don't like the uniform output, consider `Random Group` feature.  
![](https://user-images.githubusercontent.com/12690315/174311048-c882df81-6c34-4eba-b0aa-5645457692f1.gif)
- If you are using multiple materials, you will have more draw calls.
  - Consider single material, atlasing the sprites, and using `Sprite` mode in the `Texture Sheet Animation` module in ParticleSystem.

### How to make a custom shader to support Mask/RectMask2D component

<details>
<summary>Shader tips</summary>

```ShaderLab
Shader "Your/Custom/Shader"
{
    Properties
    {
        // ...
        // #### required for Mask ####
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            // ...
        }

        // #### required for Mask ####
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]
        // ...

        Pass
        {
            // ...
            // #### required for RectMask2D ####
            #include "UnityUI.cginc"
            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            float4 _ClipRect;

            // #### required for Mask ####
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                // ...
            };

            struct v2f
            {
                // ...
                // #### required for RectMask2D ####
                float4 worldPosition    : TEXCOORD1;
            };
            
            v2f vert(appdata_t v)
            {
                v2f OUT;
                // ...
                // #### required for RectMask2D ####
                OUT.worldPosition = v.vertex;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // ...
                // #### required for RectMask2D ####
                #ifdef UNITY_UI_CLIP_RECT
                    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                // #### required for Mask ####
                #ifdef UNITY_UI_ALPHACLIP
                    clip (color.a - 0.001);
                #endif

                return color;
            }
            ENDCG
        }
    }
}
```
</details>


<br><br><br><br>

## Contributing

### Issues

Issues are very valuable to this project.

- Ideas are a valuable source of contributions others can make
- Problems show where this project is lacking
- With a question you show where contributors can improve the user experience

### Pull Requests

Pull requests are, a great way to get your ideas into this repository.  
See [CONTRIBUTING.md](/../../blob/develop/CONTRIBUTING.md).

### Support

This is an open source project that I am developing in my spare time.  
If you like it, please support me.  
With your support, I can spend more time on development. :)

[![](https://user-images.githubusercontent.com/12690315/50731629-3b18b480-11ad-11e9-8fad-4b13f27969c1.png)](https://www.patreon.com/join/mob_sakai?)  
[![](https://user-images.githubusercontent.com/12690315/66942881-03686280-f085-11e9-9586-fc0b6011029f.png)](https://github.com/users/mob-sakai/sponsorship)



<br><br><br><br>

## License

* MIT



## Author

* ![](https://user-images.githubusercontent.com/12690315/96986908-434a0b80-155d-11eb-8275-85138ab90afa.png) [mob-sakai](https://github.com/mob-sakai) [![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai) ![GitHub followers](https://img.shields.io/github/followers/mob-sakai?style=social)



## See Also

* GitHub page : https://github.com/mob-sakai/ParticleEffectForUGUI
* Releases : https://github.com/mob-sakai/ParticleEffectForUGUI/releases
* Issue tracker : https://github.com/mob-sakai/ParticleEffectForUGUI/issues
* Change log : https://github.com/mob-sakai/ParticleEffectForUGUI/blob/upm/CHANGELOG.md
