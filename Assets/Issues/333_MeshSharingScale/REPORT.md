# Issue #333: Mesh sharing with different parent scales

Issue: https://github.com/mob-sakai/ParticleEffectForUGUI/issues/333

## Status

Reproduced in Unity 6000.0.71f1 using the current package with
`AutoScalingMode.Transform`. No product fix has been applied. The expected
behavior used for this investigation is that a replica has the same displayed
size as the same ParticleSystem running independently under the replica's
parent. A fix can change existing shared effects, so compatibility and the
intended behavior for other configurations must be settled before release.

## Reproduction

Open `Issue333.unity` and enter Play mode. Three single-particle effects appear:

1. A Primary with parent scale 1.
2. Its Replica with parent scale 2.
3. An independent reference with parent scale 2.

The Replica should have the same size as the independent reference, but is
the same size as the Primary. The scene uses an Overlay Canvas, World
simulation space, `AutoScalingMode.Transform`, identical particle settings
and no particle motion. A reproduction screenshot was saved in the session
workspace as `files/issue333-reproduction.png`; it is not part of the asset.

## Investigation

In Transform auto-scaling mode, `UpdateTransformScale()` cancels the scale of
each UIParticle's parent. `UIParticleRenderer` bakes a mesh using the Primary's
scale and passes that same mesh to the other renderers in the group. The
Replica therefore displays the Primary's baked size rather than the size
produced by its own parent scale.

The old local `fix-333` branch contains commit `67c7e81`, which proposes
adjusting the UIParticle transform based on the Primary-to-Replica parent
scale ratio. That commit also assigns the ratio back to `Vector3.one` before
using it, so its current code does not perform the proposed correction.
Removing that assignment alone would change the driven UIParticle transform,
potentially affecting existing layouts and effects. The issue's existing
comment also notes the compatibility risk; this is not a validated fix.
The branch was not available via the remote API at the time of this check.

## Regression checks

The PlayMode test compares the world-space mesh dimensions of a Replica with
an independently simulated reference. It checks matching scales and records
the following currently failing cases as explicit tests:

| Primary parent | Replica parent | Replica size | Reference size |
|---|---|---|---|
| `(1, 1, 1)` | `(2, 2, 2)` | `(10, 10)` | `(20, 20)` |
| `(2, 2, 2)` | `(1, 1, 1)` | `(20, 20)` | `(10, 10)` |
| `(1, 1, 1)` | changes from `(1, 1, 1)` to `(2, 3, 1)` | `(10, 10)` | `(20, 30)` |

Dimensions are measured in world units, with a tolerance of 0.1. On Unity
6000.0.71f1 the normal run passed one test; the explicit run reported one
pass and three failures. Those failures document the unfixed issue, and should
pass after an approved fix.

```sh
unity command --port 7801 test run --mode playmode --filter Coffee.UIParticle.Issue333.Tests.MeshSharingScaleTests --async-tests true --timeout 600
unity command --port 7801 test status --detail full
unity command --port 7801 test run --mode playmode --filter Coffee.UIParticle.Issue333.Tests.MeshSharingScaleTests --include-explicit true --async-tests true --timeout 600
unity command --port 7801 test status --detail full
```

After a fix, confirm all four tests pass and inspect the scene: the Replica
and independent reference should match. Also check unchanged same-scale
behavior, Primary/Primary Simulator/Auto selection and handoff, runtime
parent scale changes, nonuniform, zero and negative scales, rotated
hierarchies, Canvas scaling, masks, trails and all auto-scaling modes. Run
the #387 emission and #355 playback regression checks as appropriate.
The #387 non-explicit PlayMode tests passed (11/11), and the #355 playback
PlayMode test passed (1/1) during this investigation. The #355 scene's visual
checklist was not repeated because no playback or rendering code was changed.
