# Issue #373: Particles expire early during scripted emission

Issue: https://github.com/mob-sakai/ParticleEffectForUGUI/issues/373

## Status

Reproduced in Unity 6000.0.71f1 Editor with the current package. No product
fix has been applied. Pausing the controlled ParticleSystem immediately after
each direct `Emit()` avoided the discrepancy in the tested setup, but this
is a limited workaround, not a general solution.

## Reproduction

The reporter's attached project emits particles individually with
`ParticleSystem.Emit(EmitParams, 1)`, then reads and writes particles each
frame and records the last time each seed is observed. The reproduction here
independently creates a similar Local-space setup and a native ParticleSystem
for comparison; it does not include the reporter's assets.

Open `Issue373.unity`, enter Play mode and click **Run comparison**. With the
pause toggle disabled, the first UI particle can disappear earlier than its
native counterpart, while a later particle is closer to its expected lifetime.
One Editor run measured the first particle at 0.686 seconds for UI versus
1.081 seconds for native, and the last at 0.981 versus 0.967 seconds. These
are last-observed times, subject to frame timing; they are not exact death
timestamps.

Enable **Pause UI ParticleSystem immediately after each Emit** and rerun. In
the tested setup the UI and native lifetimes should differ by less than
0.1 seconds. This explicitly pauses the ParticleSystem itself, not
`UIParticle.Pause()`, and depends on UIParticle continuing its own manual
simulation. The workaround has not been validated for sub-emitters, trails,
other playback control patterns, or platforms.

## Investigation

The test observed `isPlaying == true` after each direct `Emit()` in the
reproduction, while `UIParticleRenderer` also calls `ParticleSystem.Simulate`
during its render update. Pausing immediately after each `Emit()` removed the
measured lifetime discrepancy. This supports an interaction between automatic
and manual simulation, but the precise engine update ordering has not been
independently traced in every scenario. The reporter's original project was
inspected, but was not run as a separate project.

An additional Editor Play-mode comparison emitted 100 particles using
`UIParticle.Emit(1)` instead of direct `Emit(EmitParams, 1)`. It also showed
shortened lifetimes: the first UI and native particles were last observed at
0.672 and 0.982 seconds, respectively, and the maximum difference was 0.311
seconds. The controlled system was playing after all 100 emissions. This
comparison identified particles by their generated random seeds and read and
wrote them each frame. `UIParticle.Emit(int)` does not accept `EmitParams`,
so it is not a direct replacement for the reporter's API usage.

Automatically intercepting direct `ParticleSystem.Emit()` is not possible
through the current UIParticle API. A general fix requires verifying Unity's
update ordering and preserving existing playback, automatic emission,
sub-emitter and pause semantics; changing those globally without that
verification risks regressions.

## Regression checks

Run the normal PlayMode test:

```sh
unity command --port 7801 test run --mode playmode --filter Coffee.UIParticle.Issue373.Tests.ParticleLifetimeTests --async-tests true --timeout 600
unity command --port 7801 test status --detail full
```

In Unity 6000.0.71f1 the workaround test passed (1/1). The direct-emission
comparison is marked `Explicit` because the issue remains unfixed. To run
both tests:

```sh
unity command --port 7801 test run --mode playmode --filter Coffee.UIParticle.Issue373.Tests.ParticleLifetimeTests --include-explicit true --async-tests true --timeout 600
unity command --port 7801 test status --detail full
```

The last run produced one pass and one expected failure: the maximum
difference from native was 0.312 seconds. After a fix, both tests should
pass with less than 0.1 seconds maximum difference for the 100 measured
particles. Repeat under different frame rates, Local and World simulation,
automatic emission, `UIParticle.Emit()` (including a reproducible automated
test), sub-emitters, trails, Pause/Play,
mobile devices, and the reporter's original configuration. Re-run the #355,
#362 and #387 regression tests. Screenshots are kept in the session workspace
and not committed as assets.
