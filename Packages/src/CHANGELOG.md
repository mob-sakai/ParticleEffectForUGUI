## [4.11.2](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.11.1...v4.11.2) (2025-03-15)


### Bug Fixes

* IL2CPP build fails on older versions of Unity ([0da6525](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/0da652520cd165b43de7404c0b0ab1fbcf9349d1))
* NRE on enable ([0cff50e](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/0cff50ef696aa53fb7c46a9a737b7cf3a05b7b9b)), closes [#359](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/359)

## [4.11.1](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.11.0...v4.11.1) (2025-02-21)


### Bug Fixes

* component icons will no longer be displayed in the scene view ([6dfbdae](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/6dfbdae38d3822ab9c2c6f0e4ca1ca32ee98a239))

# [4.11.0](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.10.7...v4.11.0) (2025-02-21)


### Features

* add 'TimeScaleMultiplier' option ([925af0b](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/925af0b6046f65f23a778f67cefa8ff9cbedb513))

## [4.10.7](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.10.6...v4.10.7) (2025-01-14)


### Bug Fixes

* editor crashed on exit play mode (editor, windows) ([47ee45c](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/47ee45cbbe651a8f87ca2b8a3948f8b88db8211e)), closes [#351](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/351)

## [4.10.6](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.10.5...v4.10.6) (2025-01-03)


### Bug Fixes

* sub-emitter particles may not render correctly in certain scenarios ([8276684](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/8276684c3b1646f0490ed64557547ba15281664a)), closes [#348](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/348)
* sub-emitter's `inherit velocity` module doubles at runtime ([67de3d1](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/67de3d1bd3e16dc9b564625cb990c53d75769506)), closes [#349](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/349)

## [4.10.5](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.10.4...v4.10.5) (2024-12-23)


### Bug Fixes

* '3D' scale toggle in the inspector does not keep on reload ([934f4b8](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/934f4b8f1c61f8ff20228d0ebcea9f636a3758ed)), closes [#346](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/346)

## [4.10.4](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.10.3...v4.10.4) (2024-12-19)


### Bug Fixes

* rendering issues when playing with opening a prefab stage ([95235a9](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/95235a929b82cf681365ed6eba837d857f83e3d2)), closes [#345](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/345)

## [4.10.3](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.10.2...v4.10.3) (2024-11-20)


### Bug Fixes

* if not configured as a preloaded asset, the project settings asset will be regenerated ([abe0948](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/abe09485f65dd4efd18e74675e752e0213bdf3be)), closes [#342](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/342)

## [4.10.2](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.10.1...v4.10.2) (2024-11-01)


### Bug Fixes

* trail incorrect offset ([afe00a1](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/afe00a1dde80eb1c0a7bb668b75f4c3733d3fa43)), closes [#335](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/335)

## [4.10.1](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.10.0...v4.10.1) (2024-09-29)


### Bug Fixes

* mainTex will be ignored ([2ee69d0](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/2ee69d04245fabce185f67dc9bd68c870e556564))

# [4.10.0](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.9.1...v4.10.0) (2024-09-29)


### Bug Fixes

* component icon is not set ([5ff6ec8](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/5ff6ec815a174de5d3f16d424f1204c60912a8d8))


### Features

* add project settings ([1ce4e31](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/1ce4e31a9681bf1a201d2723c8d97e07ecc16592))

## [4.9.1](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.9.0...v4.9.1) (2024-08-07)


### Bug Fixes

* ParticleSystem trails gain offset on parent canvas change ([2a1cd50](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/2a1cd502b452b5b56edf8bcfe91adf99d1bb5147)), closes [#323](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/323)

# [4.9.0](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.8.1...v4.9.0) (2024-07-18)


### Features

* ParticleAttractor supports multiple ParticleSystems ([3834780](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/3834780fdb43443fe6e1ef89df54d26a24d62a91))

## [4.8.1](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.8.0...v4.8.1) (2024-06-27)


### Bug Fixes

* remove debug code ([669deb4](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/669deb41d4ac589d9db93b29bc8e95383e7f28a5))

# [4.8.0](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.7.2...v4.8.0) (2024-06-27)


### Bug Fixes

* generated baking-camera object remains in the prefab or scene (again) ([de35cba](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/de35cba34c6312c1405ed522e9927c620c78e72d))
* SetParticleSystemInstance/Prefab APIs destroy generated objects ([ae3f3a8](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/ae3f3a8e62cc733420354d237ab765ac777127c8))


### Features

* add 'custom view' option. ([a703c29](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/a703c2921ca08c2280d0c8fde01e4c0b33b5c69e))
* remove overlay window (editor) ([8358170](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/835817049f4fcf00dd2bf98dbada14f041ad3544))
* restore `Transform.localScale` when setting `autoScalingMode` to something other than `Transform` (again) ([88a970d](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/88a970d93a2b69cf011d86bd1807569e90538e0e))
* the rendering order list in inspector is now more compact ([be90172](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/be901724e064befacf617f4940b0331e1d31e1ca))

## [4.7.2](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.7.1...v4.7.2) (2024-06-21)


### Bug Fixes

* generated baking-camera object remains in the prefab or scene ([0bb8438](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/0bb843830197d8c1252232928becc211c0ada08d))

## [4.7.1](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.7.0...v4.7.1) (2024-06-20)


### Bug Fixes

* despite not using the size module, particles become smaller based on their z position ([a8ed6e6](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/a8ed6e68584e1d9e45ed852eefcc03979ea7e0e1)), closes [#316](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/316)

# [4.7.0](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.6.8...v4.7.0) (2024-06-19)


### Bug Fixes

* `UIParticle.transform.localScale` does not scale particles ([1d40e24](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/1d40e24c742741e97f03c55468ccb1e505341133)), closes [#313](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/313)
* UIParticle is scaled by canvas size even when `AutoScalingMode.None` and `ScalingMode.Local` ([54a4b1c](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/54a4b1cdfd06400c7be89c1ee704bb42a659c7c2)), closes [#313](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/313)
* UIParticle is scaled incorrectly with nested canvases ([f26920f](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f26920f9825547222a4afbb31cc5dc5a002c3e9b)), closes [#313](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/313)


### Features

* reset previous position on start play for world space simulation ([3880484](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/3880484ce5190c42fc79c81d0b69e3fbeda09dd0)), closes [#303](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/303)
* restore `Transform.localScale` when setting `autoScalingMode` to something other than `Transform` ([5505247](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/5505247a94a929ff89635fde512a9b95691e0043))

## [4.6.8](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.6.7...v4.6.8) (2024-06-14)


### Bug Fixes

* 'Resource ID out of range in GetResource' error in overlay rendering mode ([05286ce](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/05286cedfd17b1a0cb90a5e918513644f47cd831)), closes [#308](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/308)

## [4.6.7](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.6.6...v4.6.7) (2024-05-24)


### Bug Fixes

* the ParticleSystem's localPosition drifts at certain scales due to floating-point precision issues ([e924eb4](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/e924eb45968a112347471cabaeabc274e4c37ce4)), closes [#299](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/299) [#312](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/312)

## [4.6.6](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.6.5...v4.6.6) (2024-05-23)


### Bug Fixes

* fix release workflow ([30b0076](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/30b00762f6da166c043587798b1552f27b4cc604))

## [4.6.5](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.6.4...v4.6.5) (2024-05-23)


### Bug Fixes

* update workflows (for preview and v4) ([3eab097](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/3eab0979b9b85919b804442ab05735b7120eade5))

## [4.6.4](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.6.3...v4.6.4) (2024-05-22)


### Bug Fixes

* assertion failed on expression: 'ps->array_size()' ([1b5c359](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/1b5c359058289895caf5f245fe09abb643bc38eb)), closes [#278](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/278)
* lost Material.mainTexture when using AnimatableProperties ([ea04352](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/ea043524c0b00f67cba26a1f9ea537ee4a56d6ff)), closes [#311](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/311)
* remove unnecessary code ([c37c014](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/c37c01486499773e3d7e8296c95bb4c3fae94abb))

## [4.6.3](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.6.2...v4.6.3) (2024-04-04)


### Bug Fixes

* if only Trail Material is used, it will not be displayed ([2eff411](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/2eff411bd97eb4e6947d29a02b85b414bfdaee3a)), closes [#294](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/294)
* if the UIParticle parents do not have Canvas, an exception is thrown in OnEnable ([e82c833](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/e82c833d04b819f103984931ba29a3616ef50908)), closes [#300](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/300)
* particle size too small due to auto scaling ([2ec3748](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/2ec374833614d64406e7c3207ca5fe234a749dcb)), closes [#295](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/295)

## [4.6.2](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.6.1...v4.6.2) (2024-02-01)


### Bug Fixes

* fix compile error in Unity 2021.1 or older ([fcae60b](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/fcae60bf29079bac07463bd3a86f8644151d72ba))
* fix demos ([ad20d12](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/ad20d128a2ad033d9f30b98f0a0dab6091f5aa19))
* fix warning ([7fd4a8e](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/7fd4a8e343ce587dffa9db5ff186061b3ebb38a6))

## [4.6.1](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.6.0...v4.6.1) (2024-01-26)


### Bug Fixes

* unintended scaling occurs when `AutoScalingMode=UIParticle` and `ScalingMode=Local` ([1627de1](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/1627de10eb1e742a015603ae9939071665a5bd89)), closes [#292](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/292)

# [4.6.0](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.5.2...v4.6.0) (2024-01-26)


### Bug Fixes

* fix abnormal mesh bounds error ([772bf50](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/772bf50d168982bd401c30e58172e0a60fbafe46)), closes [#213](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/213)
* fix warning ([93d3919](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/93d3919b6fb6ac186b3e99f8baaef9a044f583f2))
* fix warning ([8a78ec1](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/8a78ec13ad2aad9138a22b67c332871e064a38cc))


### Features

* "[generated]" GameObjects on the hierarchy is disturbing ([7c42421](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/7c4242150b591daf64390588afa27efa27368af3)), closes [#288](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/288)
* add explicit dependencies ([9a0187a](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/9a0187a72a35d378ff7be965bfcb7475f423fe0f))
* add icon ([0c1022c](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/0c1022c6224394713f62b41e5e4ef0c289610ae1))
* remove samples ([f53a7fa](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f53a7faed3ee73ac22d745a778284e818624b510))

# [4.6.0-preview.1](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/v4.5.2...v4.6.0-preview.1) (2024-01-24)


### Bug Fixes

* fix abnormal mesh bounds error ([772bf50](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/772bf50d168982bd401c30e58172e0a60fbafe46)), closes [#213](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/213)
* fix warning ([93d3919](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/93d3919b6fb6ac186b3e99f8baaef9a044f583f2))
* fix warning ([8a78ec1](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/8a78ec13ad2aad9138a22b67c332871e064a38cc))


### Features

* "[generated]" GameObjects on the hierarchy is disturbing ([7c42421](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/7c4242150b591daf64390588afa27efa27368af3)), closes [#288](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/288)
* add explicit dependencies ([9a0187a](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/9a0187a72a35d378ff7be965bfcb7475f423fe0f))
* add icon ([0c1022c](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/0c1022c6224394713f62b41e5e4ef0c289610ae1))
* remove samples ([f53a7fa](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f53a7faed3ee73ac22d745a778284e818624b510))

## [4.5.2](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.5.1...4.5.2) (2024-01-18)


### Bug Fixes

* (editor) sometimes crashes when entering play mode ([b80c3e6](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/b80c3e6c9fdd2a8fb72ff233edb85df2e3dbba3d))

## [4.5.1](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.5.0...4.5.1) (2023-12-23)


### Bug Fixes

* fix material for mesh sharing group ([6126af9](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/6126af9f376dd4c100a1b9d19d9499bdef7d5566))
* the changes made to the material used by the ParticleSystem are not immediately reflected ([3184ba9](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/3184ba94ae08264223c0c71443ad70acc1a1ccb2)), closes [#280](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/280)

# [4.5.0](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.4.0...4.5.0) (2023-12-23)


### Bug Fixes

* incorrect rendering of world-space simulated particles while animating scale ([ac58475](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/ac584755393d87bda2e80d9653370b7e4c68912f)), closes [#285](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/285)
* remove obsolete warning ([5d5eb34](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/5d5eb34590b7cefb0e4ac26c0441e104176ce522))


### Features

* Automatically generated objects are no longer editable (NotEditable). ([5607dc4](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/5607dc4eed0c086b4651941953df6c7d535712e0))
* support IMeshModifier ([5c3232f](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/5c3232faf3d2cfad1e3e1a9349b8346c7982a608)), closes [#282](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/282)

# [4.4.0](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.3.0...4.4.0) (2023-11-08)


### Features

* support 'Active Apply Color Space' for linear color space ([45c56bb](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/45c56bbd850202365751ea019baf5131b2eb9fbe))

# [4.3.0](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.2.2...4.3.0) (2023-11-08)


### Features

* added 'autoScalingMode (None, Transform.localScale, UIParticle.scale)' instead of 'autoScaling' ([107f901](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/107f901fe3232223322681edc4bf908642474298))
* reset transform.localScale on upgrading v3.x to v4.x ([c710787](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/c710787b5ba496cf73e7eb43458bb3958139baa9)), closes [#277](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/277)

## [4.2.2](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.2.1...4.2.2) (2023-10-25)


### Bug Fixes

* il2cpp code stripping bug ([73f6dad](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/73f6dad0f33641a76ddd05ffc6812ced3f8a276d)), closes [#269](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/269)

## [4.2.1](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.2.0...4.2.1) (2023-08-18)


### Bug Fixes

* autoScaling and PositionMode may be locked ([3f2f12d](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/3f2f12d2cf7541118c02830ec9fdea8183357487))

# [4.2.0](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.1.7...4.2.0) (2023-08-18)


### Bug Fixes

* assertion 'ps->array_size()' in UpdateMesh() when using trails of type ribbon ([f75fcce](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f75fcce0dae0bc166bd01d36a150aded1fd721f3)), closes [#241](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/241)
* built-in shaders are no longer supported ([c2119c1](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/c2119c171a1262431eac7fea6bf3125db2bcaaca)), closes [#233](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/233) [#257](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/257)
* crash occurs when too many vertices are rendered ([723a04d](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/723a04d0cfd156715a3c92b6d6bd75fdc1862c28))
* error: SerializedObject target has been destroyed ([e930516](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/e93051603e121732c92bcd89ded48087c2b0d7fb)), closes [#267](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/267)
* excessive particle emitted on move ParticleSystem for local space simulation and emission over distance ([2fe0bde](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/2fe0bde422f9769dfedaf6b053ea07f773646679)), closes [#265](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/265)
* fix typos ([52f2ef1](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/52f2ef1f2471a2e1c29fca96255c04b222d9c848))
* generated GameObject will be named '[generated] *' ([9b2e5c1](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/9b2e5c1d1024e091de6d18a4578cd18b43563e48))
* inactive ParticleSystems are removed from the list on refresh ([4851a18](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/4851a1880eef9f385dd9db644ea7e544f95da4fc))
* mesh sharing not working ([8b4ca1a](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/8b4ca1add5c409601e840253e1c0dbcdbf536da8)), closes [#236](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/236)
* nullptr exceptions when using nested UIParticle components in hierarchy ([e67e948](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/e67e9482e2cb840b16e2cfe76e04f7423fcbd3a3)), closes [#246](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/246)
* nullReferenceException after copy-n-paste ([425aad0](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/425aad0cbab475635c72bee84ecbf3f2acedccc2)), closes [#258](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/258)
* remove unnecessary per-frame allocation. ([e92b514](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/e92b514624cc362e53ddeae5ade20fa732f94c7c))
* scaling ParticleSystem puts prewarmed particles in wrong location ([fb31db4](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/fb31db47f2debb3aadbdc4d1b88d0efd9c4ad7bd)), closes [#235](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/235)
* sub-emitters option is not work in editor playing ([b308b26](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/b308b2683372662bb834b6f6d23ea3435a68d1b3)), closes [#231](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/231)
* the camera under UIParticle will be assigned as _orthoCamera ([c42f8c8](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/c42f8c8ab0ff033689349a81e02a4808e071a8a2))
* UIParticleAttractor attracts the particles at wrong position when in RelativeMode ([68d9925](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/68d9925a16237df3c7b07b4781172cbd03425421)), closes [#262](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/262)


### Features

* 'AbsoluteMode' option is renamed to 'PositionMode' ([67eff61](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/67eff610736344ba0122163ff5ee63b25c43f555))
* 'AutoScaling' option will be imported from 'IgnoreCanvasScale' (for v3.x) ([4103041](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/410304125f3f25f3f543c7bc01dcc661eab00609))
* add 'AutoScaling' option for UIParticle ([35325c8](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/35325c88996fa6aea19a6dd1395c05884e1f84ae))
* add 'UpdateMode' option for UIParticleAttractor ([903f702](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/903f702d7be38228841a5a693e3afdceb4a59d9f)), closes [#250](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/250)
* add particle system getter and setter for attractor ([a4bcf93](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/a4bcf93022d2729f3d2a74a2cac4f52e68641b18)), closes [#253](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/253)
* add public properties for UIParticleAttractor ([392ab6d](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/392ab6dd76c36e815320d3a50744d19faa631260)), closes [#253](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/253)
* add Start/StopEmission API for UIParticle ([e499836](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/e4998365c9825fa385e0a317768ce073a1f15b48)), closes [#240](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/240)

## [4.1.7](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.1.6...4.1.7) (2022-08-30)


### Bug Fixes

* the annoying empty black scene overlay box shown even when nothing is selected ([bdeeabb](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/bdeeabbbe140b0ba80fac7ac477874c2467d3a16))

## [4.1.6](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.1.5...4.1.6) (2022-08-10)


### Bug Fixes

* fix abnormal mesh bounds error ([f60d6df](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f60d6dfe6030ac89527a4265e414e9a0a20d56db)), closes [#213](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/213) [#218](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/218)

## [4.1.5](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.1.4...4.1.5) (2022-08-10)


### Bug Fixes

* fix culling for RectMask2D ([9e2dbe7](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/9e2dbe7758eb28a4f6a7c11113d9169847880f96)), closes [#220](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/220)

## [4.1.4](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.1.3...4.1.4) (2022-07-01)


### Bug Fixes

* add `Enabled` toggle in overlay window ([f97e619](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/f97e6195e62b5acfa8f3e97bfe3bc4a7dcadf38a))
* if `m_Particles` contains null, an error will occur ([550d0c4](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/550d0c43be35cd07e390ffd5749557c89fee0332)), closes [#214](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/214)
* ParticleSystem reordering and refreshing in inspector does not work for prefab asset ([7eb4112](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/7eb41124db06ea794db76788b35ce82a0af2c402))
* refresh button does not works in prefab edit mode ([c1538a8](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/c1538a83998608a30dc90944b05f8b75e165cf05)), closes [#214](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/214)
* when `UIParticlrRenderer` destroy manually, an error will occur ([a11d2d0](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/a11d2d01ce5f67e3f430bcb0bfdee1ad9abf7cfe))

## [4.1.3](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.1.2...4.1.3) (2022-06-28)


### Bug Fixes

* error on drag prefab to scene ([fa2f867](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/fa2f867bcaff437bb9420da1abcef970cdb09ade)), closes [#211](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/211)

## [4.1.2](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.1.1...4.1.2) (2022-06-27)


### Bug Fixes

* error on editor ([8034228](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/80342287137c07d58a7492875a401d80cb134073)), closes [#210](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/210)
* incorrect position of world space trail particles ([fb7f308](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/fb7f308f092db8a1512383857b80110cd626ecf9)), closes [#209](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/209)

## [4.1.1](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.1.0...4.1.1) (2022-06-25)


### Bug Fixes

* add absolute mode toggle to overlay window ([48d1994](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/48d1994f5f8751b707b6ef7695b552df731bece9))

# [4.1.0](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.0.1...4.1.0) (2022-06-25)


### Features

* add relative/absolute particle position mode ([1879ac8](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/1879ac8c538778e386e68cfc989a6f4f974043ca)), closes [#205](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/205)

## [4.0.1](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/4.0.0...4.0.1) (2022-06-24)


### Bug Fixes

* overlays do not exist in Unity 2019.2-2021.1 ([cd8e037](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/cd8e0372b63bb6feaaf053518013a641bc7e65ac)), closes [#207](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/207) [#208](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/208)

# [4.0.0](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.3.12...4.0.0) (2022-06-21)


### Bug Fixes

* correct world space particle position when changing screen size ([c6644a2](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/c6644a213263375c7a35b5082ef4b71cc58964e6))
* keep properly canvas batches ([d8e96e6](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/d8e96e69a62dff7a451eaed32c7a814e7e62dbb9))


### Features

* adaptive scaling for UI ([aa0d56f](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/aa0d56f9faa05e9679d4b476bcf135eafb1b8af9))
* add overlay window for UIParticle ([7b21c50](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/7b21c500ef78103b605fdca71051d2357b09602f))
* add particle attractor component ([386170c](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/386170cbf68ebf59d4510fe0a45cf83925ec9ba4))
* display warning in inspector if using 'TEXCOORD*.zw' components as custom vertex stream ([59221d5](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/59221d58217a440b77d504e6428bf99f10246260))
* mesh sharing group ([9afeebf](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/9afeebf67212cdf4d3ac9e9a3b78a7ced5c7ecfe))
* random mesh sharing group ([4fa43ed](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/4fa43eda4bc70c9c827c4fad9d5ae1327bfbc322))
* support 8+ materials ([b76bf5a](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/b76bf5a5ad378c3c4b16bcf08d21337757557101)), closes [#122](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/122) [#152](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/152) [#186](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/186)


### BREAKING CHANGES

* If you update to v4, you may be required to adjust your UIParticle.scale.

# [4.0.0-preview.1](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.3.12...4.0.0-preview.1) (2022-06-18)


### Bug Fixes

* correct world space particle position when changing screen size ([c6644a2](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/c6644a213263375c7a35b5082ef4b71cc58964e6))
* keep properly canvas batches ([d8e96e6](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/d8e96e69a62dff7a451eaed32c7a814e7e62dbb9))


### Features

* adaptive scaling for UI ([aa0d56f](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/aa0d56f9faa05e9679d4b476bcf135eafb1b8af9))
* add overlay window for UIParticle ([7b21c50](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/7b21c500ef78103b605fdca71051d2357b09602f))
* add particle attractor component ([386170c](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/386170cbf68ebf59d4510fe0a45cf83925ec9ba4))
* mesh sharing group ([9afeebf](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/9afeebf67212cdf4d3ac9e9a3b78a7ced5c7ecfe))
* random mesh sharing group ([4fa43ed](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/4fa43eda4bc70c9c827c4fad9d5ae1327bfbc322))
* support 8+ materials ([b76bf5a](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/b76bf5a5ad378c3c4b16bcf08d21337757557101)), closes [#122](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/122) [#152](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/152) [#186](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/186)


### BREAKING CHANGES

* If you update to v4, you may be required to adjust your UIParticle.scale.

## [3.3.12](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.3.11...3.3.12) (2022-06-10)


### Bug Fixes

* always display materials in inspector ([a10042d](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/a10042d989dea18ff010bdbe970aa434e2bdf117))
* UNITY_UI_ALPHACLIP in UIAdditive shader is not working ([e817e8d](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/e817e8d3c75188f3243243855b135bd840699199))

## [3.3.11](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.3.10...3.3.11) (2022-06-10)


### Bug Fixes

* sorting by layer does not work properly ([ccc09e6](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/ccc09e6aca2fa3d7bc887e6c733e66706e40ae0f))
* when using linear color space, the particle colors are not output correctly ([11c3a7b](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/11c3a7b37415d78e1b8ba3988a6e043c9f1861e0)), closes [#203](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/203)

## [3.3.10](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.3.9...3.3.10) (2022-02-17)


### Bug Fixes

* annoying warning for [ExecuteInEditMode] ([b6b2c72](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/b6b2c72b198566e2880a22831c937eff7e9eff28)), closes [#180](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/180)
* PrefabStageUtility class is not experimental after 2021.2 ([0fd5d7a](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/0fd5d7affe707fa9e92abd6e192bf400dfb1a80a))

## [3.3.9](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.3.8...3.3.9) (2021-08-02)


### Bug Fixes

* fix for warning CS0618 ([61760d9](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/61760d940cdd4baacaa196ac1631a0a1a40b7204))

## [3.3.8](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.3.7...3.3.8) (2021-06-08)


### Bug Fixes

* improve performance ([e352d15](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/e352d153cef8c1efb2792e35010d7eed1e31a040))

## [3.3.7](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.3.6...3.3.7) (2021-06-02)


### Bug Fixes

* Refresh() will be called multiple times in the same frame, due to external assets ([0b9d80d](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/0b9d80da939580c72ca1471081d7a034edc985d4))

## [3.3.6](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.3.5...3.3.6) (2021-05-11)


### Bug Fixes

* In rare cases, the generated camera (for baking) will not be deactivated ([12c748a](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/12c748a8cd4adfd2dc5f085cec77050431f261a4))
* remove from "Add Component" menu ([476c402](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/476c4027ff5f70fb9b4c026dd5fc59bf5a876227))

## [3.3.5](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.3.4...3.3.5) (2021-02-28)


### Bug Fixes

* fix cached position for pre-warmed particles ([e3f42d7](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/e3f42d747a7fd973b5813cc72a9444943a6c3ad0))
* ParticleSystem creates particles in wrong position during pre-warm ([b93e0e4](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/b93e0e4701c7011176eeec5c109dda7f4ea632e0)), closes [#147](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/147)

## [3.3.4](https://github.com/mob-sakai/ParticleEffectForUGUI/compare/3.3.3...3.3.4) (2021-02-22)


### Bug Fixes

* Multiselecting sets all scales to the same value ([13223b2](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/13223b2d747609cf88b424ad590bda7f857b387d))
* support sub emitter with 'PlayOnAwake' ([d5ce78a](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/d5ce78ae5acf2740ba7fdc6cde9f197c4e165484))
* The maximum material count is 8 ([3bb5241](https://github.com/mob-sakai/ParticleEffectForUGUI/commit/3bb52412751360409747192150188ae904f2c3d3))

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
