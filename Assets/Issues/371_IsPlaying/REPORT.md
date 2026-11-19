# Issue #371: IsPlaying information

Issue: https://github.com/mob-sakai/ParticleEffectForUGUI/issues/371

## Status

Added `UIParticle.isPlaying`, a read-only property for the state of this
component's registered ParticleSystems. In Unity 6000.0.71f1, the reported
problem was reproduced: after `UIParticle.Play()`, particles were present
while the controlled `ParticleSystem.isPlaying` was false because UIParticle
manually simulates it.

The new property returns true if the component is active, enabled and not
paused, and at least one registered, active ParticleSystem has live particles
or is still playing according to Unity's `IsAlive(false)` or `isPlaying`.
Null and destroyed entries are ignored. It includes start delay and remaining
particles after emission ends. `Pause`, `Stop`, and `Clear` make it false;
`Resume` allows it to become true again if a system remains alive.

This is a playback-state query, not a visibility test. A looping system can
remain alive after `StopEmission()` because disabling emission does not stop
the simulation. A zero time scale, missing Canvas, clipping, or other rendering
conditions do not necessarily make the property false. Only registered systems
are considered, not unregistered descendants. With Mesh Sharing, a Replica
reports its own registered systems rather than the shared Primary's state.
The behavior of direct calls to individual ParticleSystems follows their
current Unity state; it does not change `UIParticle.isPaused`.

## Reproduction

Open `Issue371.unity` and enter Play mode. Press **Play** and compare
`UIParticle.isPlaying`, `ParticleSystem.isPlaying`, and the live particle
count. Try **Pause**, **Resume**, **Stop emission**, **Stop**, and **Clear**.
The effect is non-looping, so its status returns to false after it finishes.
The sample is an independent reproduction rather than the reporter's project.

## Regression checks

The PlayMode tests cover manual simulation, start delay, manual emission,
pause/resume, stop/clear, non-looping completion, multiple systems, inactive
and null entries, unregistered children, disabled components, and a replica's
local state.

```sh
unity command --port 7801 test run --mode playmode --filter Coffee.UIParticle.Issue371.Tests.IsPlayingTests --async-tests true --timeout 600
unity command --port 7801 test status --detail full
```

In Unity 6000.0.71f1, the #371 tests passed (7/7), the #355 playback test
passed (1/1), and the normal #387 emission tests passed (11/11). Other Unity
versions and player builds have not been checked.
