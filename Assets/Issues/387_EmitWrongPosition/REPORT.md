# Issue #387: Particle emission at the wrong position

Issue: https://github.com/mob-sakai/ParticleEffectForUGUI/issues/387

## Status

The world-space position error was reproduced in Unity 6000.0.71f1 with UI
Particle 4.14.2. An experimental `UIParticle.Emit(int)` passed the limited
PlayMode checks described below. It does not fix existing direct calls to
`ParticleSystem.Emit()`, and is not yet verified as a general fix. The
reporter's exact particle-system settings are unknown, so the reproduction
is not necessarily identical to their setup.

## Reproduction

`Issue387.unity` contains an overlay Canvas and a UIParticle with scale 10.
Its child ParticleSystem has World simulation space, Play On Awake disabled,
no shape or initial speed, and a single burst at time zero. Its long particle
lifetime makes the emitted particle available for inspection.

In Play mode, call `ParticleSystem.Play()` on the child and inspect the
generated UIParticleRenderer's CanvasRenderer mesh. Its world-space bounds
center should equal the emitter's position, but was approximately ten times
the emitter's position in the reproduction. Stop and clear the system before
repeating with `UIParticle.Play()`; the burst then appears at the emitter.

For direct emission, stop and clear the system, disable its Emission module,
and call `ParticleSystem.Emit(1)`. The mesh center is similarly misplaced,
including when `UIParticle.Play()` was called before `Emit(1)`. Changing the
simulation space to Local makes the direct emission appear at the emitter.

For the reproduction's overlay Canvas, uniform scale, and disabled Shape
module, using `ParticleSystem.EmitParams.position` set to the emitter's world
position divided by the UIParticle scale also placed the particle at the
emitter. This is a limited workaround, not a general formula for child
offsets, custom Canvas scales, shape sampling, or nonuniform transforms.

The reproduction used an emitter at approximately `(772.5, 490, 0)`.
World-space particles emitted directly had a mesh center around
`(7725, 4900, 0)`. With the burst started by `UIParticle.Play()`, the mesh
center was `(772.5, 490, 0)`. In Local space, direct emission also matched the
emitter position.

## Investigation

During its simulation update, `UIParticleRenderer` temporarily scales the
world-space emitter position down before simulating, then restores the
transform. Mesh generation subsequently applies the UIParticle scale. A
particle emitted directly outside that simulation update is created using the
unscaled emitter position, and is subsequently scaled during rendering.

The prototype emits the requested count from each registered ParticleSystem,
including any registered sub-emitters, temporarily moving its transform to
the same scaled position used for normal simulation and restoring the
transform afterward. It updates automatic transform scaling before emission.
Zero emits nothing; negative counts throw `ArgumentOutOfRangeException`.
It does not intercept calls to `ParticleSystem.Emit()`.

Additional PlayMode checks in Unity 6000.0.71f1 found:

- With a stationary World-space birth sub-emitter, `Emit(1)` immediately
  created one particle in each registered system, then the parent particle
  triggered a second sub-emitter particle. Both meshes appeared at the emitter
  in this configuration. Other sub-emitter events, spaces and offsets have not
  been checked.
- In a Primary/Replica pair, emitting on the Primary produced a mesh on both.
  Calling `Emit(1)` on the Replica emitted a particle in its own system but
  did not add one to the Primary or change the shared displayed effect.
- A Custom space referencing a separate sibling Transform passed. When the
  Custom Transform was a child of the emitter, the displayed particle was
  approximately 9,034 units from the expected emitter position. A time-zero
  burst started by `UIParticle.Play()` had the same error. Custom simulation
  space is unsupported regardless of the reference Transform; its tests are
  retained only as optional observations, not supported regressions.
- A moving particle with a Local-space trail rendered both meshes close
  together. With World-space particles and a World-space trail, the meshes
  were approximately 9,034 units apart. A `UIParticle.Play()` burst with the
  same setup also showed the discrepancy.
- A moving World-space emitter retained its previously emitted particle, and
  the tested scaled Canvas worked with all three auto-scaling modes.
- Calling `Emit(1)` while a registered ParticleSystem GameObject was inactive
  did not create or queue a particle. After reactivation, a new `Emit(1)`
  produced a visible particle.

The matching `Play()` failures suggest the two rendering problems are not
specific to the new `Emit()` entry point; they have not been independently
verified on a version without the prototype. Other inactive-object cases, mesh
sharing modes, trail combinations, Custom-space transforms and Canvas
configurations remain unverified.

This distinction explains the measured results, but automatically identifying
particles emitted by arbitrary external calls without changing existing
particle behavior requires further design. In particular, changing the
ParticleSystem transform permanently or rewriting all newly observed particles
could also affect normal emission, sub-emitters, trails, and existing world-space
particles. `UIParticle.Play()` is a workaround for the tested burst, not for
arbitrary direct `Emit()` calls.

## Regression checks

After a future fix, run the reproduction above and verify the mesh center is
within a small tolerance of the emitter in both World and Local simulation
spaces. Check direct emission and a time-zero burst separately, using both
playback methods. Repeat with a non-origin emitter, nonuniform scaling, moving
emitters, existing particles, sub-emitters, and trails. Also run the #355
regression checklist in `Assets/Issues/355_PlayResetsSimulation/REPORT.md`.

The PlayMode tests cover the limited position-override workaround, Local-space
direct emission, and the time-zero burst started by `UIParticle.Play()`. For
the prototype they check World and Local spaces with nonuniform scale and an
offset child, preservation of existing particles and emitter transforms,
shape sampling, and emission from two registered systems before the first
frame. They also check zero and negative counts. They do not assert that
unmodified World-space `ParticleSystem.Emit()` is fixed. Run them with the
target Editor port:

```sh
unity command --port 7801 test run --mode playmode --filter Coffee.UIParticle.Issue387.Tests.EmissionPositionTests --async-tests true --timeout 600
unity command --port 7801 test status
```

In Unity 6000.0.71f1, the 11 non-explicit #387 tests passed. Three unsupported
Custom-space checks and two World-trail checks are marked `Explicit` so they
do not break ordinary test runs. Run them deliberately; the last complete run
produced **16 total, 12 passed, 4 failed**. The Custom-space failures are
accepted because this simulation space is unsupported; the World-trail
discrepancies remain open:

```sh
unity command --port 7801 test run --mode playmode --filter Coffee.UIParticle.Issue387.Tests.EmissionPositionTests --include-explicit true --async-tests true --timeout 600
unity command --port 7801 test status --detail full
```

The #355 `PlayDoesNotResetAnAlreadyRunningSimulation` PlayMode test passed
after the initial prototype; its scene's visual checklist was not repeated.
Replace the port if the Editor uses a different one.
