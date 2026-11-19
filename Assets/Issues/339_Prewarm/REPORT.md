# Issue #339: Prewarm on UI window activation

Issue: https://github.com/mob-sakai/ParticleEffectForUGUI/issues/339

## Status

Reproduced in Unity 6000.0.71f1 using the current package. No product fix
has been applied: a candidate that matched the native result also erased
particles emitted before the first update.

## Reproduction

Open `Issue339.unity` and enter Play mode. The comparison activates three
UIParticles and three native ParticleSystems, records particle counts after
the first rendered frame, and displays the results. Press **Deactivate and
reactivate** to repeat. Each system has Loop and Prewarm enabled, a start
lifetime of 5 seconds and a constant emission rate of 10 particles/second.
Durations are 1, 2.5 and 5 seconds. Native systems serve as a comparison.

Typical measured counts on both initial activation and reactivation:

| Duration | UIParticle | Native ParticleSystem |
|---|---:|---:|
| 1 | 11 | 50 |
| 2.5 | 26 | 50 |
| 5 | 50 | 50 |

These are controlled test conditions, not a reconstruction of the reporter's
project. The reproduction screenshot is saved in the session workspace at
`files/issue339-reproduction.png`; it is not part of this asset.

## Investigation

`UIParticleRenderer.Set` clears and pauses a prewarmed ParticleSystem.
On the next non-paused update, `UIParticleRenderer.Simulate` advances by
`main.duration` in addition to the elapsed frame time. With lifetime greater
than duration this produces fewer live particles than the native prewarm in
the tested configuration.

Unity's [Main module documentation](https://docs.unity3d.com/6000.0/Documentation/Manual/PartSysMainModule.html)
describes Prewarm as initialization after a full cycle and says it requires
Looping. The counts above are observations for this configuration, not a
general promise about Unity's warm-up duration or emission distributions.
Unity's exact behavior with bursts, variable lifetime, sub-emitters and other
versions remains unverified.

An experimental call to `ParticleSystem.Simulate(0, false, true, true)` at
the corrected emitter position made the native-count comparison pass, but
its restart cleared an explicitly emitted particle before the first update.
The existing implementation preserved that particle. The candidate was
removed rather than introducing that regression; a future fix must preserve
existing particles and account for simulation state, child systems and
renderer coordinate adjustment.

## Regression checks

In Unity 6000.0.71f1 the normal tests passed (2/2): Prewarm-off starts
without a full accumulated effect and a particle emitted before the first
update survives. The native-comparison test is marked `Explicit` and
currently fails for durations 1 and 2.5 on both activation and reactivation.
The same test passed with the rejected candidate, while the existing-particle
test failed.

```sh
unity command --port 7801 test run --mode playmode --filter Coffee.UIParticle.Issue339.Tests.PrewarmTests --async-tests true --timeout 600
unity command --port 7801 test status --detail full
unity command --port 7801 test run --mode playmode --filter Coffee.UIParticle.Issue339.Tests.PrewarmTests --include-explicit true --async-tests true --timeout 600
unity command --port 7801 test status --detail full
```

After a future fix, require all tests to pass, inspect the scene before and
after reactivation, and repeat with Loop/Prewarm off, manual Play, Pause,
variable lifetime, bursts, sub-emitters, zero time scale, World and Local
spaces and supported Unity versions. Re-run the #355 playback and #387
emission regression checks. Replace port 7801 if the Editor uses another.
