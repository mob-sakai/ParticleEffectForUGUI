ParticleEffectForUGUI
===

This plugin provide a component to render particle effect for uGUI in Unity 2018.2+.  
The particle rendering is maskable and sortable, without Camera, RenderTexture or Canvas.

[![](https://img.shields.io/github/release/mob-sakai/ParticleEffectForUGUI.svg?label=latest%20version)](https://github.com/mob-sakai/ParticleEffectForUGUI/releases)
[![](https://img.shields.io/github/release-date/mob-sakai/ParticleEffectForUGUI.svg)](https://github.com/mob-sakai/ParticleEffectForUGUI/releases)  
![](https://img.shields.io/badge/requirement-Unity%202018.2%2B-green.svg)
[![](https://img.shields.io/github/license/mob-sakai/ParticleEffectForUGUI.svg)](https://github.com/mob-sakai/ParticleEffectForUGUI/blob/upm/LICENSE.md)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-orange.svg)](http://makeapullrequest.com)
[![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai)



<< [Description](#Description) | [WebGL Demo](#demo) | [Download](https://github.com/mob-sakai/ParticleEffectForUGUI/releases) | [Usage](#usage) | [Development Note](#development-note) | [Change log](https://github.com/mob-sakai/ParticleEffectForUGUI/blob/upm/CHANGELOG.md) >>

### What's new? Please see [See changelog ![](https://img.shields.io/github/release-date/mob-sakai/ParticleEffectForUGUI.svg?label=last%20updated)](https://github.com/mob-sakai/ParticleEffectForUGUI/blob/upm/CHANGELOG.md)
### Do you want to receive notifications for new releases? [Watch this repo ![](https://img.shields.io/github/watchers/mob-sakai/ParticleEffectForUGUI.svg?style=social&label=Watch)](https://github.com/mob-sakai/ParticleEffectForUGUI/subscription)
### Support me on Patreon! [![become_a_patron](https://user-images.githubusercontent.com/12690315/50731629-3b18b480-11ad-11e9-8fad-4b13f27969c1.png)](https://www.patreon.com/join/2343451?)



<br><br><br><br>
## Description

![](https://user-images.githubusercontent.com/12690315/41771577-8da4b968-7650-11e8-9524-cd162c422d9d.gif)

This plugin uses new APIs `MeshBake/MashTrailBake` (added with Unity 2018.2) to render particles by CanvasRenderer.
You can mask and sort particles for uGUI without Camera, RenderTexture, Canvas.

Compares this "Baking mesh" approach with the conventional approach:  
(This scene is included in the package.)

|Approach|Good|Bad|Screenshot|
|-|-|-|-|
|Baking mesh<br>**\(UIParticle\)**|Rendered as is.<br>Maskable.<br>Sortable.<br>Less objects.|**Requires Unity 2018.2+.**<br>Requires UI shaders to use Mask.|<img src="https://user-images.githubusercontent.com/12690315/41765089-0302b9a2-763e-11e8-88b3-b6ffa306bbb0.gif" width="500px">|
|Do nothing|Rendered as is.|**Looks like a glitch.**<br>Not maskable.<br>Not sortable.|<img src="https://user-images.githubusercontent.com/12690315/41765090-0329828a-763e-11e8-8d8a-f1d269ea3bc7.gif" width="500px">|
|Convert particle to UIVertex<br>[\(UIParticleSystem\)](https://forum.unity.com/threads/free-script-particle-systems-in-ui-screen-space-overlay.406862/)|Maskable.<br>Sortable.<br>Less objects.|**Adjustment is difficult.**<br>Requires UI shaders.<br>Difficult to adjust scale.<br>Force hierarchy scalling.<br>Simulation results are incorrect.<br>Trail, rotation of transform, time scaling are not supported.<br>Generate heavy GC every frame.|<img src="https://user-images.githubusercontent.com/12690315/41765088-02deb9c6-763e-11e8-98d0-9e0c1766ef39.gif" width="500px">|
|Use Canvas to sort|Rendered as is.<br>Sortable.|**You must to manage sorting orders.**<br>Not maskable.<br>More batches.|<img src="https://user-images.githubusercontent.com/12690315/41765087-02b866ea-763e-11e8-8c33-081c9ad852f8.gif" width="500px">|
|Use RenderTexture|Maskable.<br>Sortable.|**Requires Camera and RenderTexture.**<br>Difficult to adjust position and size.<br>Quality depends on the RenderTexture's setting.|<img src="https://user-images.githubusercontent.com/12690315/41765085-0291b3e2-763e-11e8-827b-72e5ee9bc556.gif" width="500px">|


#### Features

* Sort particle effects with uGUI
* No Camera, RenderTexture or Canvas are required
* Masking with Mask or RectMask2D
* Easy to use
* Support Trail module
* Change alpha with CanvasGroup
* Scaling independent of Transform
* No heavy allocation every frame
* All ParticleSystem.ScalingModes and all Canvas.RenderModes are supported. They look almost the same in all modes.
![](https://user-images.githubusercontent.com/12690315/49866926-6c22f500-fe4c-11e8-8393-d5a546e9e2d3.gif)
* Scaled gizmo  
![](https://user-images.githubusercontent.com/12690315/50343861-f31e4e80-056b-11e9-8f60-8bd0a8ff7adb.gif)
* Animatable material property  
![](https://user-images.githubusercontent.com/12690315/53286323-2d94a980-37b0-11e9-8afb-c4a207805ff2.gif)



<br><br><br><br>
## Demo

[WebGL Demo](http://mob-sakai.github.io/ParticleEffectForUGUI)



<br><br><br><br>
## Install

#### Using UnityPackageManager (for Unity 2018.3+)

Find the manifest.json file in the Packages folder of your project and edit it to look like this:
```js
{
  "dependencies": {
    "com.coffee.ui-particle": "https://github.com/mob-sakai/ParticleEffectForUGUI.git#2.2.1",
    ...
  },
}
```
To update the package, change `#{version}` to the target version.  
Or, use [UpmGitExtension](https://github.com/mob-sakai/UpmGitExtension).

#### Using .unitypackage file (for Unity 2018.2+)

Download `*.unitypackage` from [Releases](https://github.com/mob-sakai/ParticleEffectForUGUI/releases) and import the package into your Unity project.  
Select `Assets > Import Package > Custom Package` from the menu.  
![](https://user-images.githubusercontent.com/12690315/46570979-edbb5a00-c9a7-11e8-845d-c5ee279effec.png)



<br><br><br><br>
## How to play demo

* Import `UIParticle_Demo.unitypackage` into your project.  
* The demo unitypackage exists in `Assets/Assets/Coffee/UIExtensions/UIParticle` or `Packages/UI Particle`.  
* Open UIParticle_Demo scene and play it.



<br><br><br><br>
## Usage

1. Add your particle effect to canvas.
2. (Option) If you want to mask particles, set a UI shader such as "UI/UIAdditive" to material for ParticleSystem.  
![](https://user-images.githubusercontent.com/12690315/42674022-134e3a40-86a9-11e8-8f44-a110d2f14185.gif)
3. Add `UIParticle` component to root particle system of your effect from `Add Component` in inspector.  
![](https://user-images.githubusercontent.com/12690315/41772125-5aca69c8-7652-11e8-8442-21f6015069a1.png)
4. If your effect consists of multiple ParticleSystems, click "Fix".  
![](https://user-images.githubusercontent.com/12690315/49148942-1c243880-f34c-11e8-9cf5-d871d65c4dbe.png)
5. Adjust the Scale property to change the size of the effect.  
![](https://user-images.githubusercontent.com/12690315/49148937-19c1de80-f34c-11e8-87fc-138192777540.gif)
6.  Enjoy!


##### Requirement

* Unity 2018.2+ (Tested in Unity 2018.2.0f2)
* No other SDK are required




<br><br><br><br>
## Development Note

#### Animatable material property

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
## License

* MIT
* © UTJ/UCL



## Author

[mob-sakai](https://github.com/mob-sakai)
[![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai)  
[![become_a_patron](https://user-images.githubusercontent.com/12690315/50731629-3b18b480-11ad-11e9-8fad-4b13f27969c1.png)](https://www.patreon.com/join/2343451?)



## See Also

* GitHub page : https://github.com/mob-sakai/ParticleEffectForUGUI
* Releases : https://github.com/mob-sakai/ParticleEffectForUGUI/releases
* Issue tracker : https://github.com/mob-sakai/ParticleEffectForUGUI/issues
* Current project : https://github.com/mob-sakai/ParticleEffectForUGUI/projects/1
* Change log : https://github.com/mob-sakai/ParticleEffectForUGUI/blob/upm/CHANGELOG.md
