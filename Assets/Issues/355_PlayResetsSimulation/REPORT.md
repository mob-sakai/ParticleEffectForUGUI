# Issue #355: `UIParticle.Play()` resets active particles

## Summary

Changing a card highlight's color and calling `UIParticle.Play()` while the effect was already running cleared its existing particles. This interrupted the transition between highlight colors.

Issue: https://github.com/mob-sakai/ParticleEffectForUGUI/issues/355

## Cause and fix

`UIParticle.Play()` called `ParticleSystem.Simulate(0, false, true)` for every registered system. The `restart` argument reset the simulation even when it was already running.

`Play()` now restarts a system only when the `UIParticle` is paused or the individual system is stopped. This preserves particles on repeated calls while still initializing a stopped system on its first `Play()` and restarting after `Pause()` or `Stop()`. `Resume()` continues a paused simulation without restarting it.

An intermediate change skipped initialization whenever `isPaused` was false. Since `isPaused` is initially false even for a stopped system, the reproduction scene exposed a regression in which the first `Play()` produced no particles. The stopped-system check addresses that case.

## Reproduction scene

Open `Issue355Highlight.unity` in Unity and enter Play mode. The scene changes the card highlight color every three seconds, calling `Play()` after each change. The **Change color + Play()** button triggers the same operation manually.

The on-screen result compares the particle count and the multiset of particle random seeds immediately before and after each call. A transition passes only when at least one particle exists and all existing particles are retained.

## Verification

- Environment: Unity 6000.0.71f1.
- Before the fix, the regression test observed the particle count drop from 9 to 0 after a repeated `Play()`.
- After the fix, the scene completed 21 color transitions with no failures; the last transition retained all 240 particles.
- The PlayMode regression test passed (1 passed, 0 failed), covering initial playback, repeated `Play()`, `Pause()`/`Resume()`, and restart after `Pause()` and `Stop()`.

Run the test with the Unity Editor connected on port 7800:

```sh
unity command --port 7800 test run --mode playmode --filter PlayDoesNotResetAnAlreadyRunningSimulation --filter-type TestName --async-tests true --timeout 600
unity command --port 7800 test status
```

The scene and test are retained as reproducible evidence; screenshots are intentionally excluded from the commit.
