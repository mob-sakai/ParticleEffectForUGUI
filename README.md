# <img alt="UIParticleIcon" src="https://github.com/mob-sakai/ParticleEffectForUGUI/assets/12690315/d76e105e-a840-4f61-a1f6-8cf311c0812d" width="26"/> Particle Effect For UGUI (UI Particle) <!-- omit in toc -->

[![](https://img.shields.io/npm/v/com.coffee.ui-particle?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.coffee.ui-particle/)
[![](https://img.shields.io/github/v/release/mob-sakai/ParticleEffectForUGUI)](https://github.com/mob-sakai/ParticleEffectForUGUI/releases)
[![](https://img.shields.io/github/license/mob-sakai/ParticleEffectForUGUI.svg)](https://github.com/mob-sakai/ParticleEffectForUGUI/blob/main/LICENSE.md)  
![](https://img.shields.io/badge/Unity-2018.2+-57b9d3.svg?style=flat&logo=unity)
![](https://img.shields.io/badge/uGUI_2.0_Ready-57b9d3.svg?style=flat)
![](https://img.shields.io/badge/UPR%2FHDPR_Ready-57b9d3.svg?style=flat)  
![](https://github.com/mob-sakai/ParticleEffectForUGUI/actions/workflows/test.yml/badge.svg?branch=develop)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-orange.svg)](http://makeapullrequest.com)
[![](https://img.shields.io/github/watchers/mob-sakai/ParticleEffectForUGUI.svg?style=social&label=Watch)](https://github.com/mob-sakai/ParticleEffectForUGUI/subscription)
[![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai)

<< [📝 Description](#-description-) | [📌 Key Features](#-key-features) | [🎮 Demo](#-demo) | [⚙ Installation](#-installation) | [🚀 Usage](#-usage) | [🛠 Development Note](#-development-note) | [🤝 Contributing](#-contributing) >>

## 📝 Description <!-- omit in toc -->

![](https://user-images.githubusercontent.com/12690315/41771577-8da4b968-7650-11e8-9524-cd162c422d9d.gif)

This package uses the new APIs `MeshBake/MeshTrailBake` (introduced in Unity 2018.2) to render particles through `CanvasRenderer`.  
You can render, mask, and sort your `ParticleSystems` for UI without the need for an additional `Camera`, `RenderTexture`, or `Canvas`.

- [📌 Key Features](#-key-features)
- [🎮 Demo](#-demo)
- [⚙ Installation](#-installation)
    - [Install via OpenUPM](#install-via-openupm)
    - [Install via UPM (with Package Manager UI)](#install-via-upm-with-package-manager-ui)
    - [Install via UPM (Manually)](#install-via-upm-manually)
    - [Install as Embedded Package](#install-as-embedded-package)
- [🚀 Usage](#-usage)
  - [Component: UIParticle](#component-uiparticle)
  - [Basic Usage](#basic-usage)
  - [Usage with Your Existing ParticleSystem Prefab](#usage-with-your-existing-particlesystem-prefab)
  - [Usage with `Mask` or `RectMask2D` Component](#usage-with-mask-or-rectmask2d-component)
  - [Usage with Script](#usage-with-script)
  - [Component: UIParticleAttractor](#component-uiparticleattractor)
  - [Project Settings](#project-settings)
- [🛠 Development Note](#-development-note)
  - [Compares the Baking mesh approach with the conventional approach](#compares-the-baking-mesh-approach-with-the-conventional-approach)
    - [Performance test results](#performance-test-results)
  - [🔍 FAQ: Why Are My UIParticles Not Displayed Correctly?](#-faq-why-are-my-uiparticles-not-displayed-correctly)
  - [Shader Limitation](#shader-limitation)
    - [Built-in shaders are not supported](#built-in-shaders-are-not-supported)
    - [(Unity 2018 or 2019) UV.zw components will be discarded](#unity-2018-or-2019-uvzw-components-will-be-discarded)
    - [(Unity 2018 or 2019) Custom vertex streams](#unity-2018-or-2019-custom-vertex-streams)
  - [Overheads](#overheads)
  - [How to Make a Custom Shader to Support `Mask` and `RectMask2D` Component](#how-to-make-a-custom-shader-to-support-mask-and-rectmask2d-component)
- [🤝 Contributing](#-contributing)
  - [Issues](#issues)
  - [Pull Requests](#pull-requests)
  - [Support](#support)
- [License](#license)
- [Author](#author)
- [See Also](#see-also)

<br><br>

## 📌 Key Features

* **Easy to use:** The package is ready to use out of the box.
* **Sortable:** Sort particle effects and other UI elements by sibling index.
* **Maskable:** Supports `Mask` or `RectMask2D`.
* **No extra components required:** No need for an additional `Camera`, `RenderTexture`, or `Canvas`.
* **Trail module support:** Fully supports the Trail module.
* **CanvasGroup alpha support:** Integrates with `CanvasGroup` alpha.
* **No allocations:** Efficiently renders particles without allocations.
* **Any canvas render mode support:** Works with overlay, camera space, and world space.
* **Any Render pipeline support:** Compatible with Universal Render Pipeline (URP) and High Definition Render Pipeline (HDRP).
* **Disabling domain reload support:** Supports disabling `Enter Play Mode Options > Reload Domain`.
* **Animatable material properties:** Supports changing material properties with AnimationClip (AnimatableProperty).  
  ![AnimatableProperty.gif](https://user-images.githubusercontent.com/12690315/53286323-2d94a980-37b0-11e9-8afb-c4a207805ff2.gif)
* **Multiple materials:** Supports 8+ materials.
* **Correct positioning:** Adjusts world space particle positions correctly when changing window size for standalone platforms (Windows, MacOSX, and Linux).
* **Adaptive scaling:** Provides adaptive scaling for UI (AutoScalingMode).
* **Performance optimization:** Mesh sharing group to improve performance.  
  <img alt="MeshSharing.gif" src="https://user-images.githubusercontent.com/12690315/174311048-c882df81-6c34-4eba-b0aa-5645457692f1.gif" width="450"/>
* **Particle attractor:** Includes a particle attractor component.  
  <img alt="ParticleAttractor.gif" src="https://user-images.githubusercontent.com/12690315/174311027-462929a4-13f0-4ec4-86ea-9c832f2eecf1.gif" width="450"/>
* **Emission position mode:** Supports relative/absolute particle emission position modes.  
  <img alt="AbsolutePosition.gif" src="https://user-images.githubusercontent.com/12690315/175751579-5a2357e8-2ecf-4afd-83c8-66e9771bde39.gif" width="450"/>
* **Custom view size:** Fixes min/max particle size mismatch.  
  ![CustomViewSize.gif](https://github.com/mob-sakai/ParticleEffectForUGUI/assets/12690315/dd929959-1a37-420b-b13d-e849022b9c9d)

<br><br>

## 🎮 Demo

* [WebGL Demo](https://mob-sakai.github.io/demos/UIParticle_Demo/index.html)

> ![](https://user-images.githubusercontent.com/12690315/174311768-1843a5f2-f776-491b-aaa8-2a131a8b6a16.gif)

* [WebGL Demo (Cartoon FX & War FX)](https://mob-sakai.github.io/Demos/ParticleEffectForUGUI_CFX)
    * [Cartoon FX Free][CFX] & [War FX][WFX] (by [Jean Moreno (JMO)][JMO]) with UIParticle

> ![](https://user-images.githubusercontent.com/12690315/91664766-3e07ac00-eb2c-11ea-978b-ef723be80619.gif)

[CFX]: https://assetstore.unity.com/packages/vfx/particles/cartoon-fx-free-109565

[WFX]: https://assetstore.unity.com/packages/vfx/particles/war-fx-5669

[JMO]: https://assetstore.unity.com/publishers/1669

<br><br>

## ⚙ Installation

_This package requires **Unity 2018.3 or later**._

#### Install via OpenUPM

- This package is available on [OpenUPM](https://openupm.com) package registry.
- This is the preferred method of installation, as you can easily receive updates as they're released.
- If you have [openupm-cli](https://github.com/openupm/openupm-cli) installed, then run the following command in your project's directory:
  ```
  openupm add com.coffee.ui-particle
  ```
- To update the package, use Package Manager UI (`Window > Package Manager`) or run the following command with `@{version}`:
  ```
  openupm add com.coffee.ui-particle@4.9.0
  ```

#### Install via UPM (with Package Manager UI)

- Click `Window > Package Manager` to open Package Manager UI.
- Click `+ > Add package from git URL...` and input the repository URL: `https://github.com/mob-sakai/ParticleEffectForUGUI.git`  
  ![](https://github.com/user-attachments/assets/f88f47ad-c606-44bd-9e86-ee3f72eac548)
- To update the package, change suffix `#{version}` to the target version.
  - e.g. `https://github.com/mob-sakai/ParticleEffectForUGUI.git#4.9.0`

#### Install via UPM (Manually)

- Open the `Packages/manifest.json` file in your project. Then add this package somewhere in the `dependencies` block:
  ```json
  {
    "dependencies": {
      "com.coffee.ui-particle": "https://github.com/mob-sakai/ParticleEffectForUGUI.git",
      ...
    }
  }
  ```

- To update the package, change suffix `#{version}` to the target version.
  - e.g. `"com.coffee.ui-particle": "https://github.com/mob-sakai/ParticleEffectForUGUI.git#4.9.0",`

#### Install as Embedded Package

1. Download a source code zip file from [Releases](https://github.com/mob-sakai/ParticleEffectForUGUI.git/releases) and extract it.
2. Place it in your project's `Packages` directory.  
   ![](https://github.com/mob-sakai/mob-sakai/assets/12690315/0b7484b4-5fca-43b0-a9ef-e5dbd99bcdb4)
- If you want to fix bugs or add features, install it as an embedded package.
- To update the package, you need to re-download it and replace the contents.

<br><br>

## 🚀 Usage

### Component: UIParticle

`UIParticle` controls the ParticleSystems that are attached to its own game objects and child game objects.

![](https://github.com/mob-sakai/ParticleEffectForUGUI/assets/12690315/1cf5753b-33fc-4cef-91c3-413c515a954f)

- **Maskable**: Does this graphic allow maskable.
- **Scale**: Scale the rendering particles. When the `3D` toggle is enabled, 3D scale (x, y, z) is supported.
- **Animatable Properties**: If you want to update material properties (e.g., `_MainTex_ST`, `_Color`) in AnimationClip,
  use this to mark as animatable.
- **Mesh Sharing**: Particle simulation results are shared within the same group. A large number of the same effects can
  be displayed with a small load. When the `Random` toggle is enabled, it will be grouped randomly.
  - **None:** Disable mesh sharing.
  - **Auto:** Automatically select Primary/Replica.
  - **Primary:** Provides particle simulation results to the same group.
  - **Primary Simulator:** Primary, but do not render the particle (simulation only).
  - **Replica:** Render simulation results provided by the primary.
- **Position Mode**: Emission position mode.
  - **Absolute:** The particles will be emitted from the world position.
  - **Relative:** The particles will be emitted from the scaled position.
- **Auto Scaling Mode**: How to automatically adjust when the Canvas scale is changed by the screen size or reference resolution.
  - **None:** Do nothing.
  - **Transform:** Transform.lossyScale (=world scale) will be set to (1, 1, 1).
  - **UIParticle:** UIParticle.scale will be adjusted.
- **Use Custom View:** Use this if the particles are not displayed correctly due to min/max particle size.
  - **Custom view size:** Change the bake view size.
- **Rendering Order**: The ParticleSystem list to be rendered. You can change the order and the materials.

**NOTE:** Press the `Refresh` button to reconstruct the rendering order based on children ParticleSystem's sorting order
and z-position.

<br><br>

### Basic Usage

1. Select `GameObject/UI/ParticleSystem` to create UIParticle with a ParticleSystem.
   ![particle](https://user-images.githubusercontent.com/12690315/95007361-cad0e880-0649-11eb-8835-f145d62c5977.png)
2. Adjust the ParticleSystem as you like.
   ![particle1](https://user-images.githubusercontent.com/12690315/95007359-ca385200-0649-11eb-8383-627c9750bda8.png)

<br>

### Usage with Your Existing ParticleSystem Prefab

1. Select `GameObject/UI/ParticleSystem (Empty)` to create UIParticle.
   ![empty](https://user-images.githubusercontent.com/12690315/95007362-cb697f00-0649-11eb-8a09-29b0a13791e4.png)
2. Drag and drop your ParticleSystem prefab onto UIParticle.
   ![particle3](https://user-images.githubusercontent.com/12690315/95007356-c6a4cb00-0649-11eb-9316-562f4bce3f31.png)

<br>

### Usage with `Mask` or `RectMask2D` Component

If you want to mask particles, set a stencil-supported shader (such as `UI/UIAdditive`) to the material for
ParticleSystem.
If you use some custom shaders, see
the [How to Make a Custom Shader to Support Mask/RectMask2D Component](#how-to-make-a-custom-shader-to-support-maskrectmask2d-component)
section.

![](https://user-images.githubusercontent.com/12690315/95017591-3b512700-0695-11eb-864e-04166ea1809a.png)

<br><br>

### Usage with Script

```cs
// Instantiate ParticleSystem prefab with UIParticle on runtime.
var go = GameObject.Instantiate(prefab);
var uiParticle = go.AddComponent<UIParticle>();
uiParticle.scale = 100;

// Control by ParticleSystem.
particleSystem.Play();
particleSystem.Emit(10);

// Control by UIParticle.
uiParticle.Play();
uiParticle.Stop();
```

<br><br>

### Component: UIParticleAttractor

`UIParticleAttractor` attracts particles generated by the specified ParticleSystem.

![](https://github.com/mob-sakai/ParticleEffectForUGUI/assets/12690315/5c20ad73-4b9a-4f38-9cdc-119df5cce077)
![](https://user-images.githubusercontent.com/12690315/174311027-462929a4-13f0-4ec4-86ea-9c832f2eecf1.gif)

- **Particle Systems**: Attracts particles generated by the specified ParticleSystems.
- **Destination Radius**: Once the particle is within the radius, the particle lifetime will become 0, and `OnAttracted`
  will be called.
- **Delay Rate**: Delay to start attracting. It is a percentage of the particle's start lifetime.
- **Max Speed**: Maximum speed of attracting. If this value is too small, attracting may not be completed by the end of
  the lifetime, and `OnAttracted` may not be called.
- **Movement**: Attracting movement type. (`Linear`, `Smooth`, `Sphere`)
- **Update Mode**: Update mode.
    - **Normal:** Update with scaled delta time.
    - **Unscaled Time:** Update with unscaled delta time.
- **OnAttracted**: An event called when attracting is complete (per particle).

<br><br>

### Project Settings

![](https://github.com/user-attachments/assets/befc7f34-fb47-4006-831a-eba79fda11ca)

- Click `Edit > Project Settings` to open the Project Settings window and then select `UI > UI Particle` category.

<br><br>

## 🛠 Development Note

### Compares the Baking mesh approach with the conventional approach

- **Baking mesh approach (=UIParticle)**  
  ![](https://user-images.githubusercontent.com/12690315/41765089-0302b9a2-763e-11e8-88b3-b6ffa306bbb0.gif)
    - ✅ Rendered as is.
    - ✅ Maskable.
    - ✅ Sortable.
    - ✅ Less objects.

- **Do nothing (=Plain ParticleSystem)**  
  ![](https://user-images.githubusercontent.com/12690315/41765090-0329828a-763e-11e8-8d8a-f1d269ea3bc7.gif)
    - ✅ Rendered as is.
    - ❌ Looks like a glitch.
    - ❌ Not maskable.
    - ❌ Not sortable.

- **Convert particle to UIVertex (=[UIParticleSystem][UIParticleSystem])**  
  ![](https://user-images.githubusercontent.com/12690315/41765088-02deb9c6-763e-11e8-98d0-9e0c1766ef39.gif)
    - ✅ Maskable.
    - ✅ Sortable.
    - ❌ Adjustment is difficult.
    - ❌ Requires UI shaders.
    - ❌ Difficult to adjust scale.
    - ❌ Force hierarchy scalling.
    - ❌ Simulation results are incorrect.
    - ❌ Trail, rotation of transform, time scaling are not supported.
    - ❌ Generate heavy GC every frame.

- **Use Canvas to sort (Sorting By Canvas )**  
  ![](https://user-images.githubusercontent.com/12690315/41765087-02b866ea-763e-11e8-8c33-081c9ad852f8.gif)
    - ✅ Rendered as is.
    - ✅ Sortable.
    - ❌ You must to manage sorting orders.
    - ❌ Not maskable.
    - ❌ More batches.
    - ❌ Requires Canvas.

- **Use RenderTexture**  
  ![](https://user-images.githubusercontent.com/12690315/41765085-0291b3e2-763e-11e8-827b-72e5ee9bc556.gif)
    - ✅ Maskable.
    - ✅ Sortable.
    - ❌ Requires Camera and RenderTexture.
    - ❌ Difficult to adjust position and size.
    - ❌ Quality depends on the RenderTexture's setting.

[UIParticleSystem]: https://forum.unity.com/threads/free-script-particle-systems-in-ui-screen-space-overlay.406862/

#### [Performance test results](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/193#issuecomment-1160028374)

| Approach                    | FPS on Editor | FPS on iPhone6 | FPS on Xperia XZ |
|-----------------------------|---------------|----------------|------------------|
| Particle System             | 43            | 57             | 22               |
| UIParticleSystem            | 4             | 3              | 0 (unmeasurable) |
| Sorting By Canvas           | 43            | 44             | 18               |
| UIParticle                  | 17            | 12             | 4                |
| UIParticle with MeshSharing | 44            | 45             | 30               |

### 🔍 FAQ: Why Are My UIParticles Not Displayed Correctly?

If `ParticleSystem` alone displays particles correctly but `UIParticle` does not, please check the following points:

- [Shader Limitation](#shader-limitation)
    - `UIParticle` does not support all built-in shaders except for `UI/Default`.
    - Most cases can be solved by using `UI/Additive` or `UI/Default`.
- Particles are not masked
    - `UIParticle` is maskable.
    - Set `Mask` or `RectMask2D` component properly.
    - [Use maskable/clipable shader](#how-to-make-a-custom-shader-to-support-maskrectmask2d-component) (such
      as `UI/Additive` or `UI/Default`)
- Particles are too small
    - If particles are small enough, they will not appear on the screen.
    - Increase the `Scale` value.
    - If you don't want to change the apparent size depending on the resolution, try the `Auto Scaling` option.
- Particles are too many
    - No more than 65535 vertices can be displayed (for mesh combination limitations).
    - Please set `Emission` module and `Max Particles` of ParticleSystem properly.
- Particles are emitted off-screen.
    - When `Position Mode = Relative`, particles are emitted from the scaled position of the ParticleSystem, not from
      the screen point of the ParticleSystem.
    - Place the ParticleSystem in the proper position or try `Position Mode = Absolute`.
- Attaching `UIParticle` to the same object as `ParticleSystem`
    - `Transform.localScale` will be overridden by the `Auto Scaling` option.
    - It is recommended to place `ParticleSystem` under `UIParticle`.
- If `Transform.localScale` contains 0, rendering will be skipped.
- Displayed particles are in the correct position but too large/too small
    - Adjust `ParticleSystem.renderer.Min/MaxParticleSize`.

<br>

### Shader Limitation

The use of UI shaders is recommended.

- If you need a simple Additive shader, use the `UI/Additive` shader instead.
- If you need a simple alpha-blend shader, use the `UI/Default` shader instead.
- If your custom shader does not work properly with UIParticle, consider creating a custom UI shader.

#### Built-in shaders are not supported

`UIParticle` does not support all built-in shaders except for `UI/Default`.  
If their use is detected, an error is displayed in the inspector.  
Use UI shaders instead.

#### (Unity 2018 or 2019) UV.zw components will be discarded

UIParticleRenderer renders the particles based on UIVertex.  
Therefore, only the xy components are available for each UV in the shader. (zw components will be discarded).  
So unfortunately, UIParticles will not work well with some shaders.

#### (Unity 2018 or 2019) Custom vertex streams

When using custom vertex streams, you can fill zw components with "unnecessary" data.  
Refer to [this issue](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/191) for more information.

<br>

### Overheads

UIParticle has some overheads, and the batching depends on uGUI.  
When improving performance, keep the following in mind:

- If you are displaying a large number of the same effect, consider the `Mesh Sharing` feature in
  the [UIParticle Component](#uiparticle-component).
    - If you don't like the uniform output, consider the `Random Group` feature.  
      ![](https://user-images.githubusercontent.com/12690315/174311048-c882df81-6c34-4eba-b0aa-5645457692f1.gif)
- If you are using multiple materials, you will have more draw calls.
    - Consider a single material, atlasing the sprites, and using `Sprite` mode in the `Texture Sheet Animation` module
      in the ParticleSystem.

### How to Make a Custom Shader to Support `Mask` and `RectMask2D` Component

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

<br><br>

## 🤝 Contributing

### Issues

Issues are incredibly valuable to this project:

- Ideas provide a valuable source of contributions that others can make.
- Problems help identify areas where this project needs improvement.
- Questions indicate where contributors can enhance the user experience.

### Pull Requests

Pull requests offer a fantastic way to contribute your ideas to this repository.  
Please refer to [CONTRIBUTING.md](https://github.com/mob-sakai/ParticleEffectForUGUI/blob/main/CONTRIBUTING.md)
and [develop branch](https://github.com/mob-sakai/ParticleEffectForUGUI/tree/develop) for guidelines.

### Support

This is an open-source project developed during my spare time.  
If you appreciate it, consider supporting me.  
Your support allows me to dedicate more time to development. 😊

[![](https://user-images.githubusercontent.com/12690315/50731629-3b18b480-11ad-11e9-8fad-4b13f27969c1.png)](https://www.patreon.com/join/2343451?)  
[![](https://user-images.githubusercontent.com/12690315/66942881-03686280-f085-11e9-9586-fc0b6011029f.png)](https://github.com/users/mob-sakai/sponsorship)

<br><br>

## License

* MIT

## Author

* ![](https://user-images.githubusercontent.com/12690315/96986908-434a0b80-155d-11eb-8275-85138ab90afa.png) [mob-sakai](https://github.com/mob-sakai) [![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai) ![GitHub followers](https://img.shields.io/github/followers/mob-sakai?style=social)

## See Also

* GitHub page : https://github.com/mob-sakai/ParticleEffectForUGUI
* Releases : https://github.com/mob-sakai/ParticleEffectForUGUI/releases
* Issue tracker : https://github.com/mob-sakai/ParticleEffectForUGUI/issues
* Change log : https://github.com/mob-sakai/ParticleEffectForUGUI/blob/main/CHANGELOG.md
