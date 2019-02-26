# Changelog

## [v2.2.1](https://github.com/mob-sakai/ParticleEffectForUGUI/tree/v2.2.1) (2019-02-26)

[Full Changelog](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v2.2.0...v2.2.1)

**Fixed bugs:**

- v2.2.0 has 2 warnings [\#44](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/44)
- Disable ParticleSystemRenderer on reset [\#45](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/45)

## [v2.2.0](https://github.com/mob-sakai/ParticleEffectForUGUI/tree/v2.2.0) (2019-02-23)

[Full Changelog](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v2.1.1...v2.2.0)

**Implemented enhancements:**

- Display warning when material does not support Mask [\#43](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/43)
- Support changing material property by AnimationClip [\#42](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/42)

**Fixed bugs:**

- UV Animation is not work. [\#41](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/41)

## [v2.1.1](https://github.com/mob-sakai/ParticleEffectForUGUI/tree/v2.1.1) (2019-02-15)

[Full Changelog](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v2.1.0...v2.1.1)

## [v2.1.0](https://github.com/mob-sakai/ParticleEffectForUGUI/tree/v2.1.0) (2019-02-07)

[Full Changelog](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v2.0.0...v2.1.0)

World simulation bug is fixed.  
![](https://user-images.githubusercontent.com/12690315/52386223-71a56000-2ac8-11e9-9cdb-93175d24febe.gif)

**Fixed bugs:**

- When moving the transform in world simulation mode, particles don't behave as expected [\#37](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/37)

## [v2.0.0](https://github.com/mob-sakai/ParticleEffectForUGUI/tree/v2.0.0) (2019-01-17)

[Full Changelog](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v1.3.3...v2.0.0)

**Install UIParticle with Unity Package Manager!**

Find the manifest.json file in the Packages folder of your project and edit it to look like this:
```js
{
  "dependencies": {
    "com.coffee.ui-particle": "https://github.com/mob-sakai/ParticleEffectForUGUI.git#2.0.0",
    ...
  },
}
```
To update the package, change `#2.0.0` to the target version.

**Implemented enhancements:**

- Integrate with UnityPackageManager [\#30](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/30)

## [v1.3.3](https://github.com/mob-sakai/ParticleEffectForUGUI/tree/v1.3.3) (2019-01-16)

[Full Changelog](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v1.3.2...v1.3.3)

**Fixed bugs:**

- On prefab edit mode, unnecessary UIParticleOverlayCamera is generated in the scene [\#35](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/35)
- With 'Screen Space - Camera' render mode, sorting is incorrect [\#34](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/34)

## [v1.3.2](https://github.com/mob-sakai/ParticleEffectForUGUI/tree/v1.3.2) (2018-12-29)

[Full Changelog](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v1.3.1...v1.3.2)

**Implemented enhancements:**

- Use shared material during play mode [\#29](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/29)

**Fixed bugs:**

- Particle not showing on Android, while on editor it works  [\#31](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/31)

## [v1.3.1](https://github.com/mob-sakai/ParticleEffectForUGUI/tree/v1.3.1) (2018-12-24)

[Full Changelog](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v1.3.0...v1.3.1)

**Fixed bugs:**

- OnSceneGUI has errors in Unity 2018.2 [\#27](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/27)
- Demo scene crashes in Unity 2018.2 [\#26](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/26)

## [v1.3.0](https://github.com/mob-sakai/ParticleEffectForUGUI/tree/v1.3.0) (2018-12-21)

[Full Changelog](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v1.2.1...v1.3.0)

With Gizmo you can control the scaled Shape.  
![](https://user-images.githubusercontent.com/12690315/50343861-f31e4e80-056b-11e9-8f60-8bd0a8ff7adb.gif)

**Fixed bugs:**

- In overlay, particle size is too small [\#23](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/23)
- UIParticle.Scale does not affect the gizmo of shape module [\#21](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/21)

## [v1.2.1](https://github.com/mob-sakai/ParticleEffectForUGUI/tree/v1.2.1) (2018-12-13)

[Full Changelog](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v1.2.0...v1.2.1)

**Fixed bugs:**

- Rect mask 2D doesn't work on WebGL [\#20](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/20)

## [v1.2.0](https://github.com/mob-sakai/ParticleEffectForUGUI/tree/v1.2.0) (2018-12-13)

[Full Changelog](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v1.1.0...v1.2.0)

New scaling system solves the particle effect scaling problem in most cases.
* All ParticleSystem.ScalingModes are supported
* All Canvas.RenderModes are supported
* They look almost the same in all modes

New scaling system scales particle effect well even if you change the following parameters:
* Camera.FieldOfView
* CanvasScaler.MatchWidthOrHeight
* Canvas.PlaneDistance

![](https://user-images.githubusercontent.com/12690315/49866926-6c22f500-fe4c-11e8-8393-d5a546e9e2d3.gif)

**NOTE: If upgrading from v1.1.0, readjust the UIParticle.Scale property.**

**Implemented enhancements:**

- New scaling system [\#18](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/18)

**Fixed bugs:**

- Rect mask 2D doesn't work [\#17](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/17)
- Using prefab view will cause a lot of errors [\#16](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/16)
- Canvas.scaleFactor not take into account [\#15](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/15)

## [v1.1.0](https://github.com/mob-sakai/ParticleEffectForUGUI/tree/v1.1.0) (2018-11-28)

[Full Changelog](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v1.0.0...v1.1.0)

Easily to use, easily to set up.

* Adjust the Scale property to change the size of the effect.  
![](https://user-images.githubusercontent.com/12690315/49148937-19c1de80-f34c-11e8-87fc-138192777540.gif)
* If your effect consists of multiple ParticleSystems, you can quickly set up UIParticles by clicking "Fix".
![](https://user-images.githubusercontent.com/12690315/49148942-1c243880-f34c-11e8-9cf5-d871d65c4dbe.png)

**Implemented enhancements:**

- Easy setup in editor [\#11](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/11)
- Add a scale property independent of transform [\#10](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/10)

**Fixed bugs:**

- Raycast blocking is unnecessary [\#12](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/12)

## [v1.0.0](https://github.com/mob-sakai/ParticleEffectForUGUI/tree/v1.0.0) (2018-07-13)

[Full Changelog](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v0.1.0...v1.0.0)

Let's use particle for your UI!  
UIParticle is use easy.  
The particle rendering is maskable and sortable, without Camera, RenderTexture or Canvas.  
![](https://user-images.githubusercontent.com/12690315/41771577-8da4b968-7650-11e8-9524-cd162c422d9d.gif)

**Implemented enhancements:**

- Supports sprites [\#5](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/5)
- Use Canvas.willRenderCanvases event instead of LateUpdate [\#4](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/4)

## [v0.1.0](https://github.com/mob-sakai/ParticleEffectForUGUI/tree/v0.1.0) (2018-06-22)

[Full Changelog](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/6b89c14a5144e290e55d041bc0ad03756a113ae0...v0.1.0)

![](https://user-images.githubusercontent.com/12690315/41771577-8da4b968-7650-11e8-9524-cd162c422d9d.gif)

This plugin uses new APIs `MeshBake/MashTrailBake` (added with Unity 2018.2) to render particles by CanvasRenderer.  
You can mask and sort particles for uGUI without Camera, RenderTexture, Canvas.

**Implemented enhancements:**

- Bake particle mesh to render by CanvasRenderer [\#1](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/1)



\* *This Changelog was automatically generated by [github_changelog_generator](https://github.com/github-changelog-generator/github-changelog-generator)*
