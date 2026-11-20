# Issue #354: Emission over Distance jitter

Issue: https://github.com/mob-sakai/ParticleEffectForUGUI/issues/354

## Findings

The original report sets a particle effect's position directly to
`Mouse.current.position.ReadValue()`. That value is in screen pixels; it is not
a Canvas-local or world-space position. The correct conversion depends on the
target RectTransform's parent and Canvas render camera.
`Scene/UIParticlePointerFollower.cs` provides an adapter that accepts the
screen position and converts it to the target RectTransform's parent plane.
For a camera-space or AR Canvas, assign the rendering camera to the Canvas.

For `rateOverDistance`, move the `UIParticle` transform rather than moving its
ParticleSystem child. The package already contains special handling for this
simulation path; the PlayMode check below covers a moving UIParticle with
Local-space particles and Local-space trails.

Issue #354 also contains separate reports of scaled World-space distance
emission in the Editor and trail rendering after adding UIParticle. The
attached Trail prefab is from Unity 2022.3.59f1 and UI Particle 4.11.2; it has
`rateOverDistance = 0.5`, an enabled Local-space Trail module, and a
UIParticle scale of 1. Those reports were not independently reproduced across
the current Unity/package version matrix. Vuforia was not available for
testing. This issue remains open pending those reproductions.

## Regression checks

The PlayMode tests check screen-to-Canvas conversion for Overlay and
Screen Space - Camera canvases, and confirm distance-based emission and trail
mesh generation while moving a UIParticle transform. Unity script compilation
completed without errors, and all three Issue #354 PlayMode tests passed.

```sh
unity command --port 7801 script recompile --focus false
unity command --port 7801 test run --mode playmode --filter Coffee.UIParticle.Issue354.Tests --filter-type Assembly --async-tests true --timeout 600
unity command --port 7801 test status --detail full
```

Results: compilation completed with `failed: false`; the PlayMode run reported
3 passed, 0 failed, 0 skipped.
