# Open Issue Triage and Difficulty Assessment

- **Triage date:** 2026-11-19
- **Last updated:** 2026-11-19
- **Scope:** Open issues in `mob-sakai/ParticleEffectForUGUI`, excluding pull requests
- **Issues reviewed:** 44
- **Purpose:** Estimate resolution difficulty from the reports and identify possible next steps for each issue.
- **Note:** #270 and #324 are FAQ or existing-information references and require no further work.

> Difficulty estimates are initial assessments based on issue descriptions and comments, not proof that the cause or reproduction has been confirmed. Issue status and code can change; check the current issue, comments, and affected versions before starting work. **Fixed** means a fix has been implemented in this repository; it does not mean the GitHub issue has been closed.

## Status Legend

| Mark | Status |
|---|---|
| ✅ | Fixed and verified; the GitHub issue may still be open. |
| 🔄 | In progress. |
| ❌ | Attempted but not fixed; record the reason and any next steps. |
| ⏸️ | Blocked or awaiting information; record the blocker. |
| ⏭️ | No action required. |

Unmarked issues have no recorded work status. Difficulty and the need for more information do not, by themselves, indicate that work was attempted or blocked.

## Difficulty Criteria

| Category | Criteria |
|---|---|
| **Easy** | Likely answerable with existing documentation or a small API or behavior change. |
| **Medium** | Requires reproduction or specification decisions in Unity and changes or tests involving multiple components or runtime states. |
| **Hard** | Likely requires rendering or simulation design changes, broad refactoring, testing across environments, or investigation of a Unity engine issue. |
| **More information needed** | Reproduction steps, environment, or expected behavior are insufficient to establish the cause and effort. Gather information first. |

## Issue Resolution Workflow

Use this document as the starting point for selecting and tracking work. Check the latest GitHub issue, comments, and state before starting, then record status and results here and update the classification if needed.

1. Create `Assets/Issues/{issue_number}_{issue_summary}/` for each issue. Use a short filename-friendly summary, replacing spaces and similar characters with `_`.
2. Use Unity MCP to inspect the relevant scenes, GameObjects, and components, and capture the state before reproduction. Capture reproduction and post-fix states as needed and save the evidence in the issue directory.
3. Keep reproduction scenes, scripts, and tests in the issue-specific directory. Minimize the reproduction where possible and do not mix assets or configurations from other issues.
4. Confirm the reproduction conditions before fixing the issue and run its regression checks. Use Unity Test Runner or logs when screenshots alone cannot establish the result.
5. Review issue-specific reproduction and test assets separately from the permanent product fix. Decide whether to retain those assets based on their testing and maintenance value.
6. Create an English report in the issue directory covering reproduction conditions, the fix, regression test instructions, explicit pass/fail criteria, and the environment and results. For later related changes, consult existing reports and reproduction assets to check potentially affected behavior; record why any relevant check could not be run.
7. Mark the issue with the appropriate status emoji in its table row, and record checks performed, blockers, and related commits or pull requests.

## Easy

| Issue | Assessment |
|---|---|
| ✅ [#355 Play() resets the simulation](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/355) | **Fixed** in commit `f9cb393`; the GitHub issue remains open. Repeated `Play()` now preserves existing particles, with a regression test. See the [report](Assets/Issues/355_PlayResetsSimulation/REPORT.md). |
| 🔄 [#387 Using Emit() emits on wrong position](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/387) | Experimental `UIParticle.Emit()` works in tested cases but not for World-space trails or a Custom space under the emitter; the same cases also fail with `UIParticle.Play()` bursts. Direct `ParticleSystem.Emit()` remains unfixed. See the [report](Assets/Issues/387_EmitWrongPosition/REPORT.md). |

## No Action Required (FAQ or Existing Information)

These FAQ/question issues do not require individual fixes or issue work. They remain included in the triage.

| Issue | Disposition |
|---|---|
| ⏭️ [#270 FAQ: Why Are My UIParticles Not Displayed Correctly?](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/270) | Information is already organized as an FAQ; no further action is required. |
| ⏭️ [#324 How to use with “CartoonFxRemasterFree”](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/324) | Treat as covered by the FAQ and existing shader limitations; no individual fix is required. |

## Medium

| Issue | Assessment |
|---|---|
| ⏭️ [#293 Is custom simulation space broken?](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/293) | Custom simulation space is not supported; use Local or World instead. |
| [#302 Size of the Shape Gizmo](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/302) | Check Shape Gizmo display across Canvas modes, separating Scene View display transforms from simulation transforms. |
| [#321 Additions to Particle Attractor](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/321) | Add constant, range, and curve options to the Attractor's Delay Rate, similar to Particle System. Design serialization and compatibility with existing values. |
| [#331 Transparency issue with Shader Graph shaders](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/331) | Shader Graph Scene Picking/Selection passes may affect baking. Reproduce with fixed URP and Shader Graph versions and consider shader selection for baking. |
| ❌ [#333 Mesh sharing across different scales](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/333) | Reproduced with differing parent scales; the Replica uses the Primary's baked size. A compatibility-safe fix remains unresolved. See the [report](Assets/Issues/333_MeshSharingScale/REPORT.md). |
| ❌ [#339 Prewarm doesn't work](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/339) | Reproduced fewer prewarmed particles than native when lifetime exceeds duration. A candidate native restart cleared existing particles, so a safe fix remains unresolved. See the [report](Assets/Issues/339_Prewarm/REPORT.md). |
| [#340 Support for Images in Shape module](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/340) | Feature request to emit from a uGUI Image shape. Define supported shapes and sampling for Sprite, Fill, masks, etc., and narrow the scope. |
| [#344 Particle Attractor: hemisphere and radius options](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/344) | Additional Attractor movement modes require decisions on coordinate space, radius, and interactions with existing modes, plus behavior tests. |
| [#354 Emission over Distance is jittery](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/354) | Separate update timing, Canvas coordinate conversion, and external factors such as Vuforia when investigating jitter from per-frame input position updates. |
| [#362 Particles break on application pause](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/362) | Check delta time, unscaled time, and Attractor behavior after application suspension, including time caps and simulation state on each platform. |
| [#365 Scene View zoom changes particle visuals](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/365) | Reproduce changes between Scene/Game View and zoom levels, focusing on the Editor Bake Camera, View Size, and Min/Max Particle Size. |
| [#366 Renderer not updated after SpriteAtlasManager late binding](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/366) | A proposed implementation is in the comments. Check subscription lifetime and the scope of material/texture updates before applying it. |
| [#371 IsPlaying information](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/371) | Decide whether the API means emitting or particles remaining alive, and define aggregation across multiple systems and behavior when paused. The API may be small, but its contract and state-based tests are needed; consider it after #387 and #378. |
| [#372 UI particle scale and Shape Gizmo do not match](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/372) | Align Editor Gizmo transforms with runtime UI rendering transforms and verify across Canvas configurations. |
| [#373 Particle lifetime is shortened unexpectedly](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/373) | Inspect the attached reproduction project and distinguish time updates from simulation or Attractor changes to `remainingLifetime`. |
| [#378 Particle Attractor: clear list / InSmooth and OutSmooth](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/378) | A target-list clearing API is small, but movement modes require specifications and tests. Separate the easy API from the extension; consider the list-clearing API after checking #387. |
| [#395 Parent scale changes particle position](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/395) | Reproduce World Simulation and Trail position changes when scaling a parent away from the origin; inspect transforms and interpolation state before and after scaling. |
| [#413 Particle position error with animation](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/413) | Investigate a transient position jump when reusing a UI object and starting an animation. Check when previous-frame position and scale caches are reset in the attached project. |

## Hard

| Issue | Assessment |
|---|---|
| [#224 Support MeshRenderer](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/224) | Supporting renderers other than ParticleSystem requires a common renderer abstraction and UI baking design, not just a small fix. |
| [#281 Support bakable renderer](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/281) | Mesh, SkinnedMesh, Line, and Trail renderers have different baking APIs and update methods; a common abstraction and renderer-specific verification are needed. |
| [#283 Improve performance](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/283) | Broad performance work includes reducing delegates and reconsidering `MaskableGraphic` inheritance. Profile and verify compatibility incrementally. |
| [#284 Update Editor](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/284) | Editor overhaul includes Inspector restructuring, compact display, and generated-object visibility. Split it into multiple features. |
| [#301 UIGeometryJob crash](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/301) | A SIGSEGV in Unity's internal UI job requires distinguishing plugin and engine causes, native crash analysis, and testing across versions. Compare with #234, #352, and #415 only as separate crashes; record thread/native stacks and Unity, OS, and package versions. |
| [#358 Optimize this plugin](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/358) | Obtain profiler data and a reproduction scene for high load with many UIParticles; measure bottlenecks before choosing changes. |
| [#364 `ps->array_size()` assertion persists](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/364) | Investigate conditions and possible Unity engine origin of the internal ParticleSystem assertion across Unity versions, including any relationship to #278. Similar to #369, but maintain separate reproduction steps without assuming a shared cause. |
| [#369 `ps->array_size()` assertion with Timeline Sub Emitters](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/369) | Separate Timeline evaluation order from particle-array access in the internal assertion involving Timeline preview and sub-emitters; test Unity versions separately. Similar to #364, but maintain separate reproduction steps without assuming a shared cause. |
| [#377 Pause mesh update/bake when off-screen](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/377) | Requires renderer culling, simulation pause/resume, Mask/Canvas visibility, and a design for elapsed time and shared renderers while invisible. |
| [#415 iOS Metal crash in BakeMesh on Unity 6000.6.8f1](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/415) | Reproduce the native Metal buffer crash on Unity 6000.6.8f1 and package 4.14.2, check known Unity issues, and verify workarounds. Do not assume a managed null-reference fix resolves it. Compare with #234, #301, and #352 only as separate crashes; record thread/native stacks and Unity, OS, and package versions. |

## More Information Needed

| Issue | Missing information and next steps |
|---|---|
| [#234 Android OOM / SIGTRAP](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/234) | Not reproduced on test devices; the Crashlytics stack is insufficient. Obtain affected devices, complete logs, memory trends, and a minimal reproduction. For #234, #301, #352, and #415, record thread/native stacks and Unity, OS, and package versions separately; do not assume a shared cause. |
| [#320 New drawcall, cannot be batched](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/320) | Frame Debugger images alone cannot identify whether Material, Texture, Stencil, or draw order split the batch. Inspect the draw state and Canvas settings. |
| [#327 Random Toggle gets enabled](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/327) | Reported when setting GroupId by script and disabling while hidden. Obtain a reproduction project, configuration code, and Inspector values before and after. |
| [#337 Special effects position adaptation](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/337) | Reported Windows/macOS resolution differences need Canvas, Camera, CanvasScaler, and scale settings from both environments and a minimal reproduction. |
| [#352 Crash on Android](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/352) | An Android IL2CPP GC-related crash log is insufficient. Obtain a complete symbolicated log, device and version details, and a reproduction scene. Compare with #234, #301, and #415 only as separate crashes; record thread/native stacks and Unity, OS, and package versions. |
| [#361 Cartoon FX shaders fail on Android](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/361) | Results differ between OpenGLES3 devices after migration from Built-in to URP. Inspect Shader Graph, variants, Graphics API, stripping, and the affected shader. |
| ⏸️ [#367 Unity 6 iOS particles invisible](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/367) | The minimal scene displays particles in Unity 6000.0.71f1 Editor and an iOS simulator. The original device issue and its cause remain unverified; a physical device and reporter's project are needed. See `Assets/Issues/367_IOSParticlesInvisible/REPORT.md`. |
| [#370 Use Custom View breaks Screen Space - Camera](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/370) | Camera rotation is suspected, but the root cause is unconfirmed without a minimal project. Check Canvas/Camera transforms and the expected bake view. Potentially related to #398; create a shared reproduction covering both configurations. |
| [#384 Trail particles do not display correctly in UI](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/384) | Trail particles disappear after disabling and re-enabling the whole prefab. Obtain a minimal reproduction with hierarchy, trail settings, and reactivation order. |
| [#396 not displayed when Inactive → Active](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/396) | Reported to depend on the order in which child ParticleSystems and the parent UIParticle are disabled; a comment also mentions prewarm. Run the attached project on the current version. |
| [#398 BakeCamera is oriented in world space](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/398) | May relate to #370, but requires reproduction data for Canvas Camera, parent transforms, and Custom View on a World Space Canvas. Create a shared reproduction covering both configurations. |
| [#409 Piloto VFX broken quads in Game View](https://github.com/mob-sakai/ParticleEffectForUGUI/issues/409) | Renderer reportedly becomes disabled in Game View but works in Scene View. Check the prefab, shader, runtime renderer state, and a minimal reproduction. |
