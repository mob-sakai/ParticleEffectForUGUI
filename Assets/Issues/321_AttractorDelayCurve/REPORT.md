# Issue #321: Additions to Particle Attractor

Issue: https://github.com/mob-sakai/ParticleEffectForUGUI/issues/321

## Status

Added `UIParticleAttractor.delayRate` as a `ParticleSystem.MinMaxCurve`.
The Inspector and runtime property support Constant, Two Constants, Curve,
and Two Curves. Each particle receives a stable normalized sample derived
from its `randomSeed`, which prevents the selected delay from changing
between updates. The evaluated value is multiplied by the particle's start
lifetime and clamped to the existing 0–0.95 delay range.

Existing serialized `float` delay values are upgraded to a Constant curve.
The existing `delay` property remains available and reads or writes a constant
value, so current scripts continue to compile.

## Reproduction

Open `Issue321.unity` and enter Play mode. Change Delay Rate in the
`UIParticleAttractor` Inspector to a range or curves, then press
**Emit particles**. Particles begin moving toward the target at different
times while retaining the selected delay for their lifetime.

## Regression checks

The PlayMode tests cover legacy float compatibility, clamping, a constant
curve, two constants, one curve, and two curves. The two-particle cases verify
that different random seeds select different start delays deterministically.

```sh
unity command --port 7801 test run --mode playmode --filter Coffee.UIParticle.Issue321.Tests.AttractorDelayCurveTests --async-tests true --timeout 600
unity command --port 7801 test status --detail full
```

In Unity 6000.0.71f1, the #321 tests passed (4/4), the #344 Hemisphere
tests passed (4/4), and the #362 Attractor and simulation tests passed (3/3).
The reproduction scene emitted 20 particles with a 0.05–0.85 Two Constants
delay range. Other Unity versions and player builds have not been checked.
