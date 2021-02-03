## [3.3.3](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.3.2...3.3.3) (2021-02-03)


### Bug Fixes

* particle trails draw in wrong transform ([17ce81e](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/17ce81eb0eccb103c21fa553183df97429cf5c6f)), closes [#145](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/145)

## [3.3.2](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.3.1...3.3.2) (2021-02-01)


### Bug Fixes

* _cachedPosition defaults to localPosition ([c0aa89b](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/c0aa89bd6f7847723a4702b6ca70fa202e8a8304)), closes [#121](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/121)
* submeshes can't over 8 ([2a1f334](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/2a1f3345bacdecf38e8890781a181a1392224e35)), closes [#122](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/122)

## [3.3.1](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.3.0...3.3.1) (2021-02-01)


### Bug Fixes

* ignore material check and transform check ([d11cd0a](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/d11cd0a06d76a32b2a119387bddc34c703b9b497)), closes [#119](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/119)
* the trail is incorrect in SimulationSpace.Local ([9313489](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/9313489552b30f2e2b0b42a641f5e0502995b03d))

# [3.3.0](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.2.0...3.3.0) (2020-11-20)


### Bug Fixes

* the particles may disappear unintentionally ([2ec81da](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/2ec81da04877d63593dd863133b6da149dcd79e6)), closes [#117](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/117)


### Features

* ignore rendering of particle systems that do not have a SharedMaterial and TrailMaterial ([08c4aba](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/08c4aba8ab9b5a041d4350a72dae62d25530afca)), closes [#118](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/118)
* show/hide materials in inspector ([4b4aebf](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/4b4aebff8cdaff9acc696a1094e170e65631135f))
* shrink rendering by material ([46a7ddd](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/46a7dddd11c3e030192cd998ae1a79441f5e5c14)), closes [#113](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/113)

# [3.2.0](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.1.1...3.2.0) (2020-11-15)


### Features

* compatibility with other IMaterialModifier ([08273cb](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/08273cb0c340ccb4f35120dc804c37d758da9ce1)), closes [#115](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/115)

## [3.1.1](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.1.0...3.1.1) (2020-11-09)


### Bug Fixes

* error on build in Unity 2019.3.11-15 ([68669c7](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/68669c739676f2354db4913a0e2296ab1715ee1f)), closes [#114](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/114)
* null Reference when creating New Scene after Prefab was open in PrefabMode ([22bcecd](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/22bcecd0abd6ad651fcf066e5c9efe9a43fd217a)), closes [#111](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/111)

# [3.1.0](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.0.1...3.1.0) (2020-10-28)


### Bug Fixes

* compile error in 2018.2 ([82f81ef](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/82f81efd7e4ea06465e24f44f96d9726a1a60cc8))


### Features

* maskable option to ignore masking ([af5f7e9](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/af5f7e90b0570d5c7fcf045fd6b81036a060e493)), closes [#109](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/109)

## [3.0.1](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.0.0...3.0.1) (2020-10-28)


### Bug Fixes

* fix the sorting algorithm ([7acbf22](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/7acbf22b4de7c3b5251fbb720bb5b575946622f6))
* in rare cases, an IndexOutOfRangeException is thrown ([f7eac0a](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f7eac0a34f07767dc04e035f97179cc30935284f))

# [3.0.0](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/2.3.0...3.0.0) (2020-10-27)


### Bug Fixes

* IgnoreCanvasScaler may be enabled unintentionally ([d9f9244](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/d9f9244e49127bea405c3cb802b588c1eae00831))
* an error happens during loading scene in editor ([ab9d9aa](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/ab9d9aa7b3afcdbdda00004f7af3fd4827aaea54)), closes [#101](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/101)
* not working as expected in world simulation space ([683fcb4](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/683fcb4ecdf8bfa0994571f5d6c3dd2bc242ca2a)), closes [#98](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/98)
* if the package was installed via openupm, an unintended directory 'Samples' was included ([1913de5](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/1913de557743b9480f72c5378d13c284a4ac93f9))
* animatable properties not working ([5b8b0bd](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/5b8b0bd28b251a7ea6e0cfa0c4b69bd7f9c4d953)), closes [#95](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/95)
* combine Instances error ([878f812](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/878f81202ac29a8a20f174efa916da64eef99e8a)), closes [#91](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/91)
* in rare cases, the particle size is incorrect with camera-space mode ([90593ac](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/90593ac021ce19d164927e44804354535db047bb)), closes [#93](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/93)
* trails material uses sprite texture ([9e65ee7](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/9e65ee7345e16b5124e94d26f5749999c648f677)), closes [#92](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/92)
* ignore missing object on initialize ([8bd9b62](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/8bd9b621b9efcd242c410405d066494a1d53f9a3))
* not masked ([4ef5947](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/4ef5947baa325002aecd1ccbdc75056a6567f14b))
* in Unity 2018.2, PrefabStageUtility is not found ([0b6dcff](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/0b6dcff5d6356db497532daa0a26804852e8de24))
* removed UIParticle will be saved in prefab mode ([08e2d51](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/08e2d51c73a294d44974e7fba35e2477f04e6860))
* hide camera for baking ([30b4703](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/30b4703e2a1746efc4b7db154354f80fd0593b98))
* In ignore canvas scaler mode, Transform.localScale is zero ([cc71f2b](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/cc71f2bdac1a61fd5e5fc85d0a69589e05a0f79d)), closes [#89](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/89)
* In prefab mode, an error occurs ([a222f37](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/a222f3710b530c7fc9fab10f25bd28d820ffebe2)), closes [#88](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/88)
* the default value of IgnoreCanvasScaler is true ([966fae1](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/966fae1d22a98259ec5aff68b4603b7c21dfdfc9))
* build fails ([ac080a4](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/ac080a44e4d872bc3f784fc222cc74aac7e795e9)), closes [#85](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/85)
* if in the mask, rendering material will be destroyed ([0db40cf](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/0db40cf160b2a5a27c83ef15d648b2771a47b51a))
* baking camera settings for camera space ([436c5e4](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/436c5e47f75c3e167dcd77c188847e9d7d6ea68d))
* fix local simulation ([7add9de](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/7add9defb70be29ddbe536d854591c2e0d9e83fa))
* fix camera for baking mesh ([6395a4f](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/6395a4fa744a46551593185711d6545a94d8b456))
* The type or namespace name 'U2D' does not exist in the namespace 'UnityEditor.Experimental' in Unity 2019.3 or later ([930199e](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/930199e5e42920825b27d5bf3e2b2a4bda77fa14)), closes [#82](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/82)
* texture sheet animation module Sprite mode not working ([30d1d5d](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/30d1d5d3cc67234a8cd985e98f181aff2a8bd8ef)), closes [#79](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/79)
* An exception in the OnSceneGUI caused the scale of the transformation to change unintentionally ([75413e0](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/75413e0e2cff42a85b73b33e17e0bb6344ecc8f6))
* read-only properties in the inspector ([f012b23](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f012b238d97aad3fdc3107b1f9a197de869c43e6))
* Added CanvasRenderer as a required component ([a8950f6](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/a8950f65c817be04b0be222c9728c716fdd7c658))
* If sprite is null, a null exception is thrown ([50c6e98](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/50c6e980ca37dda1bece5252162fa05ca3472ee8))
* fix displayed version in readme ([c29bbdd](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/c29bbddf8ad9a251d5f472b77cf85b3d432bba71))
* abnormal Mesh Bounds with Particle Trails ([518a749](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/518a7497105a114a0f6b1782df0c35ba0aecfab2)), closes [#61](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/61)
* multiple UIParticleOverlayCamera in scene ([3f09395](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/3f093958b3353463d6c5bd29ef3338203d4e41d7)), closes [#73](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/73)
* add package keywords ([49d8f3f](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/49d8f3fe4c76cf6bd2cd5b6134ee23134532da8e))
* particles not visible if scale.z is 0 ([35718e0](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/35718e099acbb04fdadf131c7e4d2e6c3f4a1756)), closes [#64](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/64)
* remove unnecessary scripts ([0a43740](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/0a4374099dc3151e7f1a3a24a6ce6c39a968e163))
* workaround for [#70](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/70) ([4bbcc33](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/4bbcc334abb7cd6db2897fad0bda219d5ea73530))
* change the text in the inspector to make it more understandable. ([7ca0b6f](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/7ca0b6fa34c1168ef103992e1c69b68631b3bc60)), closes [#66](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/66)
* editor crashes when mesh is set to null when ParticleSystem.RenderMode=Mesh ([e5ebadd](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/e5ebadd84731716f82c63ac5ba3eb676720e3ad6)), closes [#69](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/69)
* getting massive errors on editor ([ef82fa8](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/ef82fa82a69894e643f0d257f99eb9177785f697)), closes [#67](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/67)
* heavy editor UI ([d3b470a](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/d3b470a2be3a21add9e126b357e46bdfaa6f16c8))
* remove a menu to add overlay camera ([f5d3b6e](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f5d3b6edb5687c4d465992ef5c3c0d54f7b36d74))
* rotating the particle rect may cause out of bounds ([3439842](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/3439842119f334b50910c0a983e561944cc792a2)), closes [#55](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/55)
* scale will be decrease on editor ([0c59846](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/0c59846f11438b12caad04ae5d5b544a28883ea6))
* UI darker than normal when change to linear color space ([db4d8a7](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/db4d8a742ca36c8dd2de6936b9cf2912c72d4b9f)), closes [#57](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/57)


### Features

* cache modified material ([6b397f3](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/6b397f39b89f40c4aae9c9f56706b3bc68a376be)), closes [#94](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/94)
* improve the material batching ([4be5666](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/4be56669fb764bf61c0246a6e56d18640053b565))
* un-limit on the number of mesh instances ([f133881](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f1338813ca85d305f334799b78154e03b0aff60c))
* refresh children ParticleSystem with a gameObjects as root ([8bae1d0](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/8bae1d08cc6f00e2b8d6f336aad92233891da1e4))
* add API to bind ParticleSystem object ([a77bbd3](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/a77bbd3a9a65d5fd1198bd8e580982ca8e07fca8))
* material batching ([8f703e6](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/8f703e6d2c0e8229ca14b25638dae5d91a5658c3))
* support AnimatableProperty for multiple materials ([062d988](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/062d9887fb8b096250ec3b43d9aa82637940a8bb))
* remove menu in inspector ([e7f8f51](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/e7f8f512122a01423de415b55e3190d62bda146a))
* add menu to create UIParticle ([2fa1843](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/2fa18431f0c8c4aeadfdd1cb98eeeef5ac6970a0))
* add play/pause/stop api ([f09a386](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f09a386bc59fbab8143f7f0b814c8684aea7f27c))
* support for changing rendering orders ([745d4a5](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/745d4a598846b3e77d1071433079fdd5140921a8))
* Support for child ParticleSystem rendering ([4ee90be](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/4ee90be17c68bf405f81f432615a3eebaa022366))
* UIParticle for trail is no longer needed ([466e43c](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/466e43cf931d211907419f804a90776a0d9f4906))
* add menu to create UIParticle ([14f1c78](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/14f1c782ff0f2b67d85d7c9ad0cf662da1dd1fc6))
* Combine baked meshes to improve performance ([633d058](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/633d058756fde776a7e5192a0f023adf6eb0ca7b))
* improve performance ([77c056a](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/77c056ad5f2918efe457883f3b0361f952600568))
* optimization for vertices transforms and adding node for trails ([e070e8d](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/e070e8d5ee205c25a1e3be5d3178821d4a8265d0)), closes [#75](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/75)
* option to ignoring canvas scaling ([fe85fed](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/fe85fed3c0ad2881578ff68722863d65dfa4db7a))
* support 3d scaling ([42a84bc](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/42a84bc5e130aed3cf5e57dcd6a9d8dc94deb641))
* support custom simulation space ([a83e647](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/a83e64761c008e88ff328a2609118806e97f19cf)), closes [#78](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/78)
* support for particle systems including trail only ([f389d39](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f389d39953c85b97482b12d1c8578ecaeddacd18)), closes [#61](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/61)
* add support for SpriteAtlas ([b31e325](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/b31e325bb1ef0856cb1ac4c4b0c4da0f1578b8ba))
* add menu to import sample ([b8b1827](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/b8b18273185769235101da01f5bbadbac188e387))


### BREAKING CHANGES

* The development branch name has been changed. Most cases are unaffected.
* The child UIParticle is no longer needed.
* The bake-task has changed significantly. It may look different from previous versions.
* update develop environment to Unity 2018.3. Unity 2018.2 will continue to be supported.

# [3.0.0-preview.38](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.37...v3.0.0-preview.38) (2020-10-04)


### Bug Fixes

* delete unused file in package ([2e69974](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/2e699749a8f0f620505621a13a628aa87f192875))
* material dirty on validate (on editor) ([fa34301](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/fa3430130cdffa3f934e926645958ad9f19edc5d))


### Features

* display material properties in inspector ([313c1fc](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/313c1fc159429034f84b2e7c30424158c43b71e9)), closes [#104](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/104)
* support 3D scaling ([a508c3b](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/a508c3bb86ad6694722868303385b20adc914134)), closes [#105](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/105)

# [3.0.0-preview.37](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.36...v3.0.0-preview.37) (2020-10-01)


### Bug Fixes

* fix menus ([5fa12b5](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/5fa12b5338a90764a3bf384dcd3911f2ab4eba61))

# [3.0.0-preview.36](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.35...v3.0.0-preview.36) (2020-09-28)


### Bug Fixes

* do not bake particle system to mesh when the alpha is zero ([1775713](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/1775713c2dbeef09ad3eb1f49b53cf44bf61d535)), closes [#102](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/102)
* in Unity 2018.x, sample import failed on Windows ([f5861b0](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f5861b0add1477987d6b9a3db26979fde50930ad))

# [3.0.0-preview.35](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.34...v3.0.0-preview.35) (2020-09-27)


### Bug Fixes

* an error happens during loading scene in editor ([ab9d9aa](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/ab9d9aa7b3afcdbdda00004f7af3fd4827aaea54)), closes [#101](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/101)

# [3.0.0-preview.34](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.33...v3.0.0-preview.34) (2020-09-15)


### Bug Fixes

* not working as expected in world simulation space ([683fcb4](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/683fcb4ecdf8bfa0994571f5d6c3dd2bc242ca2a)), closes [#98](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/98)

# [3.0.0-preview.33](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.32...v3.0.0-preview.33) (2020-09-14)


### Bug Fixes

* if the package was installed via openupm, an unintended directory 'Samples' was included ([1913de5](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/1913de557743b9480f72c5378d13c284a4ac93f9))

# [3.0.0-preview.32](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.31...v3.0.0-preview.32) (2020-09-14)


### Bug Fixes

* animatable properties not working ([5b8b0bd](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/5b8b0bd28b251a7ea6e0cfa0c4b69bd7f9c4d953)), closes [#95](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/95)

# [3.0.0-preview.31](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.30...v3.0.0-preview.31) (2020-09-02)


### Bug Fixes

* combine Instances error ([878f812](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/878f81202ac29a8a20f174efa916da64eef99e8a)), closes [#91](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/91)
* in rare cases, the particle size is incorrect with camera-space mode ([90593ac](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/90593ac021ce19d164927e44804354535db047bb)), closes [#93](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/93)
* trails material uses sprite texture ([9e65ee7](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/9e65ee7345e16b5124e94d26f5749999c648f677)), closes [#92](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/92)


### Features

* refresh children ParticleSystem with a gameObjects as root ([8bae1d0](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/8bae1d08cc6f00e2b8d6f336aad92233891da1e4))

# [3.0.0-preview.30](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.29...v3.0.0-preview.30) (2020-09-02)


### Bug Fixes

* ignore missing object on initialize ([8bd9b62](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/8bd9b621b9efcd242c410405d066494a1d53f9a3))


### Features

* add API to bind ParticleSystem object ([a77bbd3](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/a77bbd3a9a65d5fd1198bd8e580982ca8e07fca8))

# [3.0.0-preview.29](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.28...v3.0.0-preview.29) (2020-09-01)


### Features

* material batching ([8f703e6](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/8f703e6d2c0e8229ca14b25638dae5d91a5658c3))

# [3.0.0-preview.28](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.27...v3.0.0-preview.28) (2020-09-01)


### Features

* support AnimatableProperty for multiple materials ([062d988](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/062d9887fb8b096250ec3b43d9aa82637940a8bb))

# [3.0.0-preview.27](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.26...v3.0.0-preview.27) (2020-09-01)


### Bug Fixes

* not masked ([4ef5947](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/4ef5947baa325002aecd1ccbdc75056a6567f14b))

# [3.0.0-preview.26](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.25...v3.0.0-preview.26) (2020-09-01)


### Bug Fixes

* in Unity 2018.2, PrefabStageUtility is not found ([0b6dcff](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/0b6dcff5d6356db497532daa0a26804852e8de24))

# [3.0.0-preview.25](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.24...v3.0.0-preview.25) (2020-09-01)


### Bug Fixes

* removed UIParticle will be saved in prefab mode ([08e2d51](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/08e2d51c73a294d44974e7fba35e2477f04e6860))

# [3.0.0-preview.24](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.23...v3.0.0-preview.24) (2020-09-01)


### Bug Fixes

* hide camera for baking ([30b4703](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/30b4703e2a1746efc4b7db154354f80fd0593b98))
* In ignore canvas scaler mode, Transform.localScale is zero ([cc71f2b](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/cc71f2bdac1a61fd5e5fc85d0a69589e05a0f79d)), closes [#89](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/89)
* In prefab mode, an error occurs ([a222f37](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/a222f3710b530c7fc9fab10f25bd28d820ffebe2)), closes [#88](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/88)


### Features

* remove menu in inspector ([e7f8f51](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/e7f8f512122a01423de415b55e3190d62bda146a))

# [3.0.0-preview.23](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.22...v3.0.0-preview.23) (2020-08-31)


### Bug Fixes

* the default value of IgnoreCanvasScaler is true ([966fae1](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/966fae1d22a98259ec5aff68b4603b7c21dfdfc9))

# [3.0.0-preview.22](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.21...v3.0.0-preview.22) (2020-08-29)


### Bug Fixes

* build fails ([ac080a4](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/ac080a44e4d872bc3f784fc222cc74aac7e795e9)), closes [#85](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/85)

# [3.0.0-preview.21](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.20...v3.0.0-preview.21) (2020-08-28)


### Bug Fixes

* if in the mask, rendering material will be destroyed ([0db40cf](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/0db40cf160b2a5a27c83ef15d648b2771a47b51a))
* support animatable material property (again) ([cf6ca80](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/cf6ca80d1273bcf49e18d805260afa8e36e94617))

# [3.0.0-preview.20](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.19...v3.0.0-preview.20) (2020-08-28)


### Features

* automatically update ([96a868b](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/96a868b60a3f36d761d58b5082aa9d37666e63a3))

# [3.0.0-preview.19](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.18...v3.0.0-preview.19) (2020-08-28)


### Bug Fixes

* baking camera settings for camera space ([436c5e4](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/436c5e47f75c3e167dcd77c188847e9d7d6ea68d))
* fix local simulation ([7add9de](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/7add9defb70be29ddbe536d854591c2e0d9e83fa))


### Features

* add menu to create UIParticle ([2fa1843](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/2fa18431f0c8c4aeadfdd1cb98eeeef5ac6970a0))
* add play/pause/stop api ([f09a386](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f09a386bc59fbab8143f7f0b814c8684aea7f27c))
* support for changing rendering orders ([745d4a5](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/745d4a598846b3e77d1071433079fdd5140921a8))
* Support for child ParticleSystem rendering ([4ee90be](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/4ee90be17c68bf405f81f432615a3eebaa022366))
* UIParticle for trail is no longer needed ([466e43c](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/466e43cf931d211907419f804a90776a0d9f4906))


### BREAKING CHANGES

* The child UIParticle is no longer needed.

# [3.0.0-preview.18](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.17...v3.0.0-preview.18) (2020-08-19)


### Bug Fixes

* AsmdefEx is no longer required ([50e749c](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/50e749c183def5e97affa7e6ae9f3ceb69247825))
* fix camera for baking mesh ([6395a4f](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/6395a4fa744a46551593185711d6545a94d8b456))
* support .Net Framework 3.5 (again) ([23fcb06](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/23fcb06bf9169ee160ccd8adb2cc3aab1a30186a))


### Features

* 3.0.0 updater ([f99292b](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f99292b9a15c9c085efacc0330d6b848669fadfa))
* add menu to create UIParticle ([14f1c78](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/14f1c782ff0f2b67d85d7c9ad0cf662da1dd1fc6))
* Combine baked meshes to improve performance ([633d058](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/633d058756fde776a7e5192a0f023adf6eb0ca7b))
* improve performance ([77c056a](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/77c056ad5f2918efe457883f3b0361f952600568))
* optimization for vertices transforms and adding node for trails ([e070e8d](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/e070e8d5ee205c25a1e3be5d3178821d4a8265d0)), closes [#75](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/75)
* option to ignoring canvas scaling ([fe85fed](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/fe85fed3c0ad2881578ff68722863d65dfa4db7a))
* support 3d scaling ([42a84bc](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/42a84bc5e130aed3cf5e57dcd6a9d8dc94deb641))
* support custom simulation space ([a83e647](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/a83e64761c008e88ff328a2609118806e97f19cf)), closes [#78](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/78)
* support for particle systems including trail only ([f389d39](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f389d39953c85b97482b12d1c8578ecaeddacd18)), closes [#61](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/61)


### BREAKING CHANGES

* The bake-task has changed significantly. It may look different from previous versions.

# [3.0.0-preview.17](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.16...v3.0.0-preview.17) (2020-08-13)


### Bug Fixes

* The type or namespace name 'U2D' does not exist in the namespace 'UnityEditor.Experimental' in Unity 2019.3 or later ([930199e](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/930199e5e42920825b27d5bf3e2b2a4bda77fa14)), closes [#82](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/82)

# [3.0.0-preview.16](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.15...v3.0.0-preview.16) (2020-08-12)


### Bug Fixes

* texture sheet animation module Sprite mode not working ([30d1d5d](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/30d1d5d3cc67234a8cd985e98f181aff2a8bd8ef)), closes [#79](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/79)

# [3.0.0-preview.15](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.14...v3.0.0-preview.15) (2020-08-11)


### Bug Fixes

* An exception in the OnSceneGUI caused the scale of the transformation to change unintentionally ([75413e0](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/75413e0e2cff42a85b73b33e17e0bb6344ecc8f6))

# [3.0.0-preview.14](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.13...v3.0.0-preview.14) (2020-08-11)


### Bug Fixes

* read-only properties in the inspector ([f012b23](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f012b238d97aad3fdc3107b1f9a197de869c43e6))

# [3.0.0-preview.13](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.12...v3.0.0-preview.13) (2020-08-11)


### Bug Fixes

* Added CanvasRenderer as a required component ([a8950f6](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/a8950f65c817be04b0be222c9728c716fdd7c658))
* inspector is broken in Unity 2020.1 ([26c5395](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/26c5395a45ff00e99e46ee4aae85c51df6c3641f))


### Features

* update OSC to 1.0.0-preview.25 ([22e116e](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/22e116e11d3b8cf13b941e9a02a0ffce24e3e99f))

# [3.0.0-preview.12](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.11...v3.0.0-preview.12) (2020-08-11)


### Bug Fixes

* Profiler.BeginSample -> Profiler.EndSample if a canvas is disabled or a camera doesn't found ([4a0a5d1](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/4a0a5d13be68e38d5b2e225156740aed27c52d12))

# [3.0.0-preview.11](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.10...v3.0.0-preview.11) (2020-05-07)


### Bug Fixes

* If sprite is null, a null exception is thrown ([50c6e98](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/50c6e980ca37dda1bece5252162fa05ca3472ee8))

# [3.0.0-preview.10](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.9...v3.0.0-preview.10) (2020-04-30)


### Features

* add support for SpriteAtlas ([b31e325](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/b31e325bb1ef0856cb1ac4c4b0c4da0f1578b8ba))

# [3.0.0-preview.9](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.8...v3.0.0-preview.9) (2020-03-04)


### Bug Fixes

* fix displayed version in readme ([c29bbdd](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/c29bbddf8ad9a251d5f472b77cf85b3d432bba71))

# [3.0.0-preview.8](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.7...v3.0.0-preview.8) (2020-03-03)


### Bug Fixes

* abnormal Mesh Bounds with Particle Trails ([518a749](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/518a7497105a114a0f6b1782df0c35ba0aecfab2)), closes [#61](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/61)
* multiple UIParticleOverlayCamera in scene ([3f09395](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/3f093958b3353463d6c5bd29ef3338203d4e41d7)), closes [#73](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/73)

# [3.0.0-preview.7](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.6...v3.0.0-preview.7) (2020-03-02)


### Bug Fixes

* add package keywords ([49d8f3f](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/49d8f3fe4c76cf6bd2cd5b6134ee23134532da8e))
* fix sample path ([57ee210](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/57ee21005e114fdf186b5db55ca2b77b7b7c441a))

# [3.0.0-preview.6](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.5...v3.0.0-preview.6) (2020-02-21)


### Bug Fixes

* sample version ([ed18032](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/ed18032be43397debbd538cae258c226ebeeb2e9))

# [3.0.0-preview.5](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.4...v3.0.0-preview.5) (2020-02-21)


### Bug Fixes

* particles not visible if scale.z is 0 ([35718e0](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/35718e099acbb04fdadf131c7e4d2e6c3f4a1756)), closes [#64](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/64)

# [3.0.0-preview.4](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.3...v3.0.0-preview.4) (2020-02-18)


### Bug Fixes

* compile error in Unity 2019.1 or later ([28ca922](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/28ca922167afff3ef8341fa747968357d4487d1f)), closes [#70](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/70) [#71](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/71)

# [3.0.0-preview.3](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.2...v3.0.0-preview.3) (2020-02-17)


### Bug Fixes

* remove unnecessary scripts ([0a43740](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/0a4374099dc3151e7f1a3a24a6ce6c39a968e163))
* workaround for [#70](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/70) ([4bbcc33](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/4bbcc334abb7cd6db2897fad0bda219d5ea73530))

# [3.0.0-preview.2](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v3.0.0-preview.1...v3.0.0-preview.2) (2020-02-13)


### Bug Fixes

* compile error ([e2c5c7b](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/e2c5c7b05d1307877e2f37555d4845932d542930))

# [3.0.0-preview.1](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v2.3.0...v3.0.0-preview.1) (2020-02-12)


### Bug Fixes

* change the text in the inspector to make it more understandable. ([7ca0b6f](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/7ca0b6fa34c1168ef103992e1c69b68631b3bc60)), closes [#66](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/66)
* editor crashes when mesh is set to null when ParticleSystem.RenderMode=Mesh ([e5ebadd](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/e5ebadd84731716f82c63ac5ba3eb676720e3ad6)), closes [#69](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/69)
* getting massive errors on editor ([ef82fa8](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/ef82fa82a69894e643f0d257f99eb9177785f697)), closes [#67](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/67)
* heavy editor UI ([d3b470a](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/d3b470a2be3a21add9e126b357e46bdfaa6f16c8))
* remove a menu to add overlay camera ([f5d3b6e](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f5d3b6edb5687c4d465992ef5c3c0d54f7b36d74))
* rotating the particle rect may cause out of bounds ([3439842](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/3439842119f334b50910c0a983e561944cc792a2)), closes [#55](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/55)
* scale will be decrease on editor ([0c59846](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/0c59846f11438b12caad04ae5d5b544a28883ea6))
* UI darker than normal when change to linear color space ([db4d8a7](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/db4d8a742ca36c8dd2de6936b9cf2912c72d4b9f)), closes [#57](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/57)


### Build System

* update develop environment ([9fcf169](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/9fcf169cd3ced519611b2ede7f98ad4d678027c6))


### Features

* add menu to import sample ([b8b1827](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/b8b18273185769235101da01f5bbadbac188e387))
* add samples test ([287b5cc](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/287b5cc832f2899796227520bda4d11ad8e4fae9))
* **editor:** add osc package (portable mode) ([6c7f880](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/6c7f8804350112505949c5c296f9e0340877a3e8))


### BREAKING CHANGES

* update develop environment to Unity 2018.3.
Unity 2018.2 will continue to be supported.

# Changelog

## [v2.3.0](https://github.com/mob-sakai/ParticleEffectForUGUI/tree/v2.3.0) (2019-05-12)

[Full Changelog](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v2.2.1...v2.3.0)

World simulation bug due to transform movement have been fixed

**Fixed bugs:**

- World simulation bug due to transform movement [\#47](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/47)

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
