# Issue #378: Add target-list clearing and smooth movement modes

Issue: https://github.com/mob-sakai/ParticleEffectForUGUI/issues/378

## Status

Added `ClearParticleSystems()` to clear registered target ParticleSystems and
the Attractor's cached particle and movement state. Added `Movement.InSmooth`
and `Movement.OutSmooth` without changing the existing enum values or the
existing `Movement.Smooth` behavior.

The new modes use a stable particle origin and a quadratic easing curve:
`InSmooth` uses `t * t` to accelerate at the beginning, while `OutSmooth` uses
`1 - (1 - t) * (1 - t)` to decelerate at the end. Both remain subject to `maxSpeed`.

## Regression checks

The PlayMode tests verify the easing profiles across updates, cleared target
registration and movement state, and preservation of existing movement enum
values.

Open `Issue378.unity` to compare all movement modes, adjust the Attractor speed,
emit or clear particles, and register, remove, or clear the target list.

```sh
unity command --port 7801 test run --mode playmode --filter Coffee.UIParticle.Issue378.Tests.AttractorSmoothTests --async-tests true --timeout 600
unity command --port 7801 test status --detail full
```

The #378 tests passed (3/3). The Attractor regression filter covering #321,
#344, #362, and #378 also passed (14/14):

```sh
unity command --port 7801 test run --mode playmode --filter Attractor --filter-type TestName --async-tests true --timeout 600
unity command --port 7801 test status --detail full
```
