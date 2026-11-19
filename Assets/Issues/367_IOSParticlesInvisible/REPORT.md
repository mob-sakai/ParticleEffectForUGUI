# Issue #367: UI particles invisible in iOS builds

Issue: https://github.com/mob-sakai/ParticleEffectForUGUI/issues/367

## Status

The original iOS failure has not been reproduced and its cause has not been
established. No package fix has been made; physical-device verification and
the reporter's minimal project are still needed.

## Reproduction scene

`Issue367.unity` contains a screen-space overlay Canvas, a UIParticle, and a
looping child ParticleSystem using the package's `UI/Additive` material. It is
a minimal approximation of the reported setup, not the reporter's project.

In Unity 6000.0.71f1, entering Play mode displayed particles in the Editor;
one runtime inspection found 36 particles. The Editor capture is stored outside
the issue directory at `Screenshots/issue367-editor.png`.

The original report used Unity 6000.0.20f1 and UI Particle 4.11.2. The current
checkout and reproduction scene are not identical to that environment. A later
Android report describes particles disappearing after a short time; it has not
been established that the two reports share a cause.

## iOS simulator verification

Unity 6000.0.71f1, UI Particle 4.14.2, built-in render pipeline, development
build, IL2CPP, Metal, Xcode 26.2, iPhone 16 Pro simulator with iOS 18.1.
`UI/Additive` was temporarily added to Always Included Shaders for the build.

Unity generated an Xcode project and `xcodebuild` compiled it successfully.
After relaunching the app, screenshots taken approximately 20 and 80 seconds
later show visible particles. An earlier screenshot taken shortly after the
first launch was black, so wait for the app to finish starting before judging.
The screenshots are retained in the session workspace, not the repository.
The console reported an unsupported ASTC texture format and fallback
decompression, but no particle-related error was observed.

The prior iOS SDK setting (`DeviceSDK`) and Always Included Shaders list were
restored. No physical iOS or Android device was connected. The simulator uses
a different graphics environment from an iOS device, and a successful result
does not establish that the original physical-device issue is fixed.

## Android follow-up

The later Android report supplies a separate Unity package containing a scene,
textures, a material, and a Timeline asset. It has not been imported or
executed: no Android device or configured Android emulator was found. Importing
it directly would also place assets outside the issue directory. Its symptom
and cause remain unverified and should not be conflated with the iOS report.

## Regression procedure

1. Open `Issue367.unity` and enter Play mode. Confirm that particles continuously
   appear in the center of the Game view.
2. Build the scene for iOS with `UI/Additive` in Always Included Shaders. Record
   the Unity version, package version, iOS SDK, device or simulator, graphics
   API, and build result. Restore any project settings changed for the build.
3. Run the build and compare the display with the Editor. A regression check
   passes only when particles remain visible throughout the observation period.
   Record a screenshot and the duration. A simulator result alone does not
   prove that the original physical-device issue is fixed.
4. If playback or rendering code changes, also run the #355 regression checklist
   in `Assets/Issues/355_PlayResetsSimulation/REPORT.md`.
