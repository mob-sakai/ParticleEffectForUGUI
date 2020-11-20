Particle Effect For UGUI (UI Particle)
===

**:warning: NOTE: Do not use [the obsolete tags and branches](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/108) to reference the package. They will be removed in near future. :warning:**

This plugin provide a component to render particle effect for uGUI in Unity 2018.2 or later.  
The particle rendering is maskable and sortable, without Camera, RenderTexture or Canvas.

[![](https://img.shields.io/npm/v/com.coffee.ui-particle?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.coffee.ui-particle/)
[![](https://img.shields.io/github/v/release/mob-sakai/ParticleEffectForUGUI?include_prereleases)](https://github.com/mob-sakai/ParticleEffectForUGUI/releases)
[![](https://img.shields.io/github/release-date/mob-sakai/ParticleEffectForUGUI.svg)](https://github.com/mob-sakai/ParticleEffectForUGUI/releases)  [![](https://img.shields.io/github/license/mob-sakai/ParticleEffectForUGUI.svg)](https://github.com/mob-sakai/ParticleEffectForUGUI/blob/master/LICENSE.txt)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-orange.svg)](http://makeapullrequest.com)  
![](https://img.shields.io/badge/Unity%202018.2+-supported-blue.svg)
![](https://img.shields.io/badge/Unity%202019.x-supported-blue.svg)
![](https://img.shields.io/badge/Unity%202020.x-supported-blue.svg)  
![](https://img.shields.io/badge/Universal%20Rendering%20Pipeline-supported-blue.svg)


<< [Description](#Description) | [Demo](#demo) | [Installation](#installation) | [Usage](#usage) | [Development Note](#development-note) | [Change log](https://github.com/mob-sakai/ParticleEffectForUGUI/blob/upm/CHANGELOG.md) >>



<br><br><br><br>

## Description

![](https://user-images.githubusercontent.com/12690315/41771577-8da4b968-7650-11e8-9524-cd162c422d9d.gif)

This plugin uses new APIs `MeshBake/MashTrailBake` (added with Unity 2018.2) to render particles by CanvasRenderer.
You can mask and sort particles for uGUI without Camera, RenderTexture, Canvas.

Compares this "Baking mesh" approach with the conventional approach:  
(This scene is included in the package.)

|Approach|Good|Bad|Screenshot|
|-|-|-|-|
|Baking mesh<br>**\(UIParticle\)**|Rendered as is.<br>Maskable.<br>Sortable.<br>Less objects.|**Requires Unity 2018.2 or later.**<br>Requires UI shaders to use Mask.|<img src="https://user-images.githubusercontent.com/12690315/41765089-0302b9a2-763e-11e8-88b3-b6ffa306bbb0.gif" width="500px">|
|Do nothing|Rendered as is.|**Looks like a glitch.**<br>Not maskable.<br>Not sortable.|<img src="https://user-images.githubusercontent.com/12690315/41765090-0329828a-763e-11e8-8d8a-f1d269ea3bc7.gif" width="500px">|
|Convert particle to UIVertex<br>[\(UIParticleSystem\)](https://forum.unity.com/threads/free-script-particle-systems-in-ui-screen-space-overlay.406862/)|Maskable.<br>Sortable.<br>Less objects.|**Adjustment is difficult.**<br>Requires UI shaders.<br>Difficult to adjust scale.<br>Force hierarchy scalling.<br>Simulation results are incorrect.<br>Trail, rotation of transform, time scaling are not supported.<br>Generate heavy GC every frame.|<img src="https://user-images.githubusercontent.com/12690315/41765088-02deb9c6-763e-11e8-98d0-9e0c1766ef39.gif" width="500px">|
|Use Canvas to sort|Rendered as is.<br>Sortable.|**You must to manage sorting orders.**<br>Not maskable.<br>More batches.|<img src="https://user-images.githubusercontent.com/12690315/41765087-02b866ea-763e-11e8-8c33-081c9ad852f8.gif" width="500px">|
|Use RenderTexture|Maskable.<br>Sortable.|**Requires Camera and RenderTexture.**<br>Difficult to adjust position and size.<br>Quality depends on the RenderTexture's setting.|<img src="https://user-images.githubusercontent.com/12690315/41765085-0291b3e2-763e-11e8-827b-72e5ee9bc556.gif" width="500px">|


#### Features

* Easy to use: the package is out-of-the-box
* Sort particle effects with UI
* No Camera, RenderTexture or Canvas are required
* Masking with Mask or RectMask2D
* Support Trail module
* Change alpha with CanvasGroup
* No heavy allocation every frame
* Support overlay, camera space and world space
* Support changing material property with AnimationClip (AnimatableProperty)  
![](https://user-images.githubusercontent.com/12690315/53286323-2d94a980-37b0-11e9-8afb-c4a207805ff2.gif)



<br><br><br><br>

## Demo

* [WebGL Demo](https://mob-sakai.github.io/Demos/ParticleEffectForUGUI)
* [WebGL Demo (Cartoon FX & War FX)](https://mob-sakai.github.io/Demos/ParticleEffectForUGUI_CFX)
  * [Cartoon FX Free][CFX] & [War FX][WFX] (by [Jean Moreno (JMO)][JMO]) with UIParticle

[CFX]: https://assetstore.unity.com/packages/vfx/particles/cartoon-fx-free-109565
[WFX]: https://assetstore.unity.com/packages/vfx/particles/war-fx-5669
[JMO]: https://assetstore.unity.com/publishers/1669

![](https://user-images.githubusercontent.com/12690315/91664766-3e07ac00-eb2c-11ea-978b-ef723be80619.gif)



<br><br><br><br>

## Installation

### Requirement

![](https://img.shields.io/badge/Unity%202018.2+-supported-blue.svg)
![](https://img.shields.io/badge/Unity%202019.x-supported-blue.svg)
![](https://img.shields.io/badge/Unity%202020.x-supported-blue.svg)  
![](https://img.shields.io/badge/Universal%20Rendering%20Pipeline-supported-blue.svg)

### Using OpenUPM

This package is available on [OpenUPM](https://openupm.com).  
You can install it via [openupm-cli](https://github.com/openupm/openupm-cli).
```
openupm add com.coffee.ui-particle
```

### Using Git

Find the manifest.json file in the Packages folder of your project and add a line to `dependencies` field.

* `"com.coffee.ui-particle": "https://github.com/mob-sakai/ParticleEffectForUGUI.git"`

To update the package, change suffix `#{version}` to the target version.

* `"com.coffee.ui-particle": "https://github.com/mob-sakai/ParticleEffectForUGUI.git#3.3.0",`

Or, use [UpmGitExtension](https://github.com/mob-sakai/UpmGitExtension) to install and update the package.

### For Unity 2018.2

Unity 2018.2 supports embedded packages.

1. Download a source code zip file from [Releases](https://github.com/mob-sakai/ParticleEffectForUGUI/releases) page
2. Extract it
3. Import it under `Packages` directory in your Unity project



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

![](https://user-images.githubusercontent.com/12690315/99765566-af129a80-2b42-11eb-88f6-661182d0b619.png)

| Properties | Description |
| -- | -- |
| Maskable | Does this graphic allow masking. |
| Ignore Canvas Scale | Ignore the scale of the root canvas.<br>This prevents it from displaying small even in hierarchy scaling mode of `ParticleSystem`. |
| Scale | Scale the rendering.<br>When the `3D` toggle is enabled, 3D scale (x,y,z) is supported. |
| Animatable Properties | If you want update material properties (e.g. `_MainTex_ST`, `_Color`) in AnimationClip, use this to mark the changes. |
| Shrink By Material | Shrink rendering by material.<br>Performance will be improved, but in some cases the rendering is not correct. |
| Rendering Order | The ParticleSystems to be rendered.<br>You can change the rendering order and the materials. |

NOTE: Press `Refresh` button to reconstruct rendering order based on children ParticleSystem's sorting order and z position.

<br><br>

### Basically usage

1. Select `Game Object/UI/ParticleSystem` to create UIParticle with a ParticleSystem.  
![particle](https://user-images.githubusercontent.com/12690315/95007361-cad0e880-0649-11eb-8835-f145d62c5977.png)
2. Adjust the ParticleSystem as you like.  
![particle1](https://user-images.githubusercontent.com/12690315/95007359-ca385200-0649-11eb-8383-627c9750bda8.png)

<br><br>

### With your ParticleSystem prefab

1. Select `Game Object/UI/ParticleSystem (Empty)` to create UIParticle.  
![empty](https://user-images.githubusercontent.com/12690315/95007362-cb697f00-0649-11eb-8a09-29b0a13791e4.png)
2. Drag & drop your ParticleSystem prefab on UIParticle.  
![particle3](https://user-images.githubusercontent.com/12690315/95007356-c6a4cb00-0649-11eb-9316-562f4bce3f31.png)

<br><br>

### With `Mask` or `MaskRect2D` component

If you want to mask particles, set a stencil supported shader (such as `UI/UIAdditive`) to material for ParticleSystem.

![](https://user-images.githubusercontent.com/12690315/95017591-3b512700-0695-11eb-864e-04166ea1809a.png)


<br><br>

### Script usage

```cs
// Instant ParticleSystem prefab with UIParticle on runtime.
var go = GameObject.Instantiate(prefab);
var uiParticle = go.AddComponent<UIParticle>();

// Play/Stop the controled ParticleSystems.
uiParticle.Play();
uiParticle.Stop();
```

<br><br><br><br>

## Development Note

### Animatable material property

![](https://user-images.githubusercontent.com/12690315/53286323-2d94a980-37b0-11e9-8afb-c4a207805ff2.gif)

Animation clips can change the material properties of the Renderer, such as ParticleSystemRenderer.  
It uses MaterialPropertyBlock so it does not create new material instances.  
Using material properties, you can change UV animation, scale and color etc.

Well, there is a component called CanvasRenderer.  
It is used by all Graphic components for UI (Text, Image, Raw Image, etc.) including UIParticle.  
However, It is **NOT** a Renderer.  
Therefore, in UIParticle, changing ParticleSystemRenderer's MaterialPropertyBlock by animation clip is ignored.

To prevent this, Use "Animatable Material Property".  
"Animatable Material Property" gets the necessary properties from ParticleSystemRenderer's MaterialPropertyBlock and sets them to the CanvasRenderer's material. 



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
