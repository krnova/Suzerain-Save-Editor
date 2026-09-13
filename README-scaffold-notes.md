# Android port scaffold — apply-by-hand notes

This zip mirrors the target repo structure. Copy each file into place at the
matching path (overwriting the blank/moved placeholders from your earlier
`touch`/`git mv` pass). It does NOT include your ViewModels/ folder content —
that moved verbatim via `git mv` already and needs nothing but a namespace
edit (see below).

## Required manual fixups this scaffold could NOT do for you

1. **`SuzerainSaveEditor.UI/ViewModels/*.cs`** — after your `git mv`, every
   file in here still says `namespace SuzerainSaveEditor.App.ViewModels;`.
   Bulk-replace with `namespace SuzerainSaveEditor.UI.ViewModels;` across the
   whole folder, and fix any `using SuzerainSaveEditor.App.*` lines the same
   way. I have not seen these files' contents, so I could not verify there's
   nothing else in them coupled to the old namespace/App project.

2. **`SuzerainSaveEditor.UI/App.axaml`** — I have never seen this file's
   content. Two things to check once you paste it here or open it yourself:
   - Its `x:Class` attribute almost certainly still reads
     `SuzerainSaveEditor.App.App` — must become `SuzerainSaveEditor.UI.App`
     to match the moved `App.axaml.cs`.
   - If it declares `<FluentTheme/>` or references icon resources
     (`IconSave`, `IconFolderOpen`, `IconUndo`, `IconRedo`, `IconSearch`,
     `IconClock`, `IconCheck`, `IconError`) as merged dictionaries, those
     need to stay reachable from here — `MainView.axaml` (now in this same
     project) references them via `{StaticResource ...}`, so if they were
     previously merged in at the `Application` level they'll still resolve
     fine post-move. If they lived somewhere under the old `App` project's
     `Assets/` instead, that reference will break and needs relocating too.

3. **`SuzerainSaveEditor.App/App.axaml` and `.csproj` `Assets\**` glob** —
   `Assets/app-icon.ico` and `avalonia-logo.ico` should stay in the desktop
   project (Windows icon, `ApplicationIcon` property references it), but
   confirm nothing in the moved `App.axaml` needs them by relative path.

4. **Tests project** — `SuzerainSaveEditor.Tests` presumably references
   `SuzerainSaveEditor.App.ViewModels.*` types directly (its
   `ViewModelTests.cs` files). Those now live in `SuzerainSaveEditor.UI`.
   You'll need to add a `ProjectReference` to `SuzerainSaveEditor.UI.csproj`
   in `SuzerainSaveEditor.Tests.csproj` and fix the `using` statements. I
   haven't seen that csproj or those test files, so I can't do this part for
   you blind.

5. **Solution file** — `SuzerainSaveEditor.slnx` needs the two new projects
   (`SuzerainSaveEditor.UI`, `SuzerainSaveEditor.Android`) added so they show
   up in tooling / get built by `dotnet build` at the solution level. Exact
   syntax depends on your `.slnx` schema version — I haven't seen the file.

6. **`AndroidChangeSummaryView` / `AndroidUnsavedChangesView` styling** —
   deliberately minimal/unstyled placeholders, not a visual match for the
   desktop dialogs. Revisit once you can actually see them on-device.

## Things I changed from what I described in chat, worth knowing

- `DragDropHelper` / `DroppedFile` are now `public` (were `internal`) —
  required because they're called from `SuzerainSaveEditor.App`'s
  code-behind, which is now a different assembly than the one that defines
  them. Added `InternalsVisibleTo` entries in `SuzerainSaveEditor.UI.csproj`
  as a belt-and-suspenders measure in case anything else internal turns out
  to need cross-assembly visibility once you actually compile this.
- `IPlatformHost.cs` needed a `using SuzerainSaveEditor.UI.Services;` I'd
  missed when first describing it in chat — added.
- Android's `AndroidManifest.xml` deliberately declares NO storage
  permissions — see the inline comment for why (scoped storage +
  Storage Access Framework via `FileDialogService`).
- `android:minSdkVersion` set to 21 and `SupportedOSPlatformVersion` to 21 —
  confirmed via search that .NET 10's Android minimum is still API 21 (the
  bump to 24 for the CoreCLR runtime migration lands in .NET 11, not this
  release).
- `TargetFramework` for the Android project is the bare `net10.0-android`
  (no `34.0`/`36.0` suffix) so it tracks the workload's current default
  rather than going stale — confirmed via search that the current default
  is API 36.0 as of this writing, but that's exactly the kind of number
  that moves, which is the point of not hardcoding it.

## Not yet done, flagged earlier as follow-up work

- No Android implementation of `ISavePathProvider`/`IAppDataPathProvider` —
  these still use whatever platform-detection logic they had before (I
  haven't seen those files either). On Android these likely need to resolve
  to `Context.GetExternalFilesDir(null)` / `Context.FilesDir` instead of a
  Windows `%LOCALAPPDATA%`-style path. Worth checking before assuming
  "recent files" / backups work correctly on-device.
