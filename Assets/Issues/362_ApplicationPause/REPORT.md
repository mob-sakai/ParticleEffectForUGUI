# Issue #362: Particle effects after application suspension

Issue: https://github.com/mob-sakai/ParticleEffectForUGUI/issues/362

## Status

In Unity 6000.0.71f1 Editor, a deliberately blocked frame reproduced an
unbounded jump in `UIParticleAttractor` using Unscaled Time. The existing
`235560544e1bc21e65445c44727c920e6aca61b4` change already caps the UI
ParticleSystem's unscaled simulation delta but did not cap the attractor's
unscaled movement. The attractor now uses the same `Time.maximumDeltaTime`
limit. Actual application suspension and Android native UI have not been
verified; the long-frame reproduction is a proxy, not proof that every
reported mobile symptom is fixed.

## Reproduction and fix

Open `Issue362.unity` and enter Play mode. Press **Simulate 1 second stall**.
The demonstration intentionally blocks the main thread for one second, then
displays the measured unscaled delta, maximum delta, particle lifetime
advance, and attractor displacement. It uses a non-looping World-space
ParticleSystem with unscaled time and an unscaled linear attractor.

Before the fix, the PlayMode test observed attractor displacement of about
1.016 units in a long frame, exceeding the configured bound of 0.13 units.
The particle simulation remained capped. After the fix, with a maximum
delta of 0.1 seconds, both the attractor and the simulation remain within
their respective limits. Normal scaled attractor movement is unchanged.

In the demonstration, a measured unscaled delta of 1.010 seconds with a
maximum of 0.333 seconds produced a 0.333-second particle lifetime advance
and 0.200 units of attractor displacement. Screenshots are saved in the
session workspace, not in this asset.

## Regression checks

Run the following PlayMode tests in Unity Editor:

```sh
unity command --port 7801 test run --mode playmode --filter Coffee.UIParticle.Issue362.Tests.ApplicationPauseTests --async-tests true --timeout 600
unity command --port 7801 test status --detail full
```

All three tests must pass. They check a long frame with unscaled attractor
movement, the existing particle simulation cap, and that scaled and unscaled
attractor modes behave differently with `Time.timeScale` set to zero.
They restore altered global time settings afterward. Replace the port if the
Editor uses another one.

For a complete platform check, reproduce the reporter's non-looping,
approximately one-second duration and five-second lifetime effect on an
Android device, with and without an attractor and with unscaled time. Open
native UI and background/restore the application while particles are alive;
confirm that particles remain visible and resume without an unexpected jump.
Repeat the Editor shortcut and iOS checks if relevant. Check configured
`Time.maximumDeltaTime` and `UIParticle.timeScaleMultiplier`: the latter is
applied after the particle simulation cap. The reporter's exact project and
device setup remain unavailable. No Android device was attached during this
investigation.
