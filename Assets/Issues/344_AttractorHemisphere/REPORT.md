# Issue #344: Add hemisphere movement and a radius control

Issue: https://github.com/mob-sakai/ParticleEffectForUGUI/issues/344

## Status

Added `Movement.Hemisphere` and the `hemisphereRadius` property to
`UIParticleAttractor`. The new enum value is appended so existing serialized
`Linear`, `Smooth`, and `Sphere` values keep their numeric assignments. The
new movement follows a sinusoidal half-arch from the particle's position when
attraction begins to the destination. It stores that initial position per
ParticleSystem particle seed so the curve remains stable across updates. The
position along that curve is advanced from its previous progress toward the
current lifetime ratio, matching the convergence behavior of the Smooth
movement rather than moving linearly through the full curve duration.
`hemisphereRadius` controls the maximum offset from the direct path at the
midpoint, in particle simulation units. The Attractor's up axis determines the
curve direction; if it is parallel to the travel direction, a stable fallback
axis is used. The radius can be zero. The existing movement types and
destination-arrival radius are unchanged.

The path endpoint and destination test remain subject to the existing
`maxSpeed` setting. If the speed is too low, a particle may not reach the
destination before its lifetime expires. The movement step is compensated
for the curve angle so the particle's progress along the origin-to-destination
axis remains comparable to a straight path at the same `maxSpeed`.

## Reproduction and usage

Open `Issue344.unity` and enter Play mode. Emit particles, switch between
Linear and Hemisphere, and adjust Hemisphere Radius. With Hemisphere selected,
particles curve toward the Attractor; increasing the radius increases the
maximum offset from the direct path.

`destinationRadius` remains the radius around the destination that triggers
`OnAttracted`; `hemisphereRadius` controls the path shape and is a separate
setting.

## Regression checks

The PlayMode tests verify midpoint offset, start/end points, radius clamping,
continued movement along the same curve, low-speed compensation for the
longer curved path, curve orientation in World and Local simulation space,
and finite positions when the Attractor up axis is parallel to the path.
Existing movement enum values are preserved.

```sh
unity command --port 7801 test run --mode playmode --filter Coffee.UIParticle.Issue344.Tests.AttractorHemisphereTests --async-tests true --timeout 600
unity command --port 7801 test status --detail full
```

In Unity 6000.0.71f1, the #344 tests passed (5/5) and the existing #362
Attractor and simulation tests passed (3/3). Other Unity versions and player
builds have not been checked.
