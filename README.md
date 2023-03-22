# AI: THE SOMNIUM FILES - nirvanA Initiative Fix (port to BepInEx6 BE)

This is a WIP BepInEx6 BE version of [AI: THE SOMNIUM FILES - nirvanA Initiative Fix](https://github.com/Lyall/AISomniumFiles2Fix), with debug menu fixes maybe potentially.

Fixes done:

- `BustShot` fake folder stuff to populate treeview.
- Hide `EventViewer`, because code for it is stripped completely and it overlays other windows.
- Restore partial functionality of `FlagViewer` by jankiest means possible.
- All `TopMenu` buttons work. Functionality should be the same as intended (except for action binds which don't work???).

## Configuration

Example config with defaults:

```ini
## Settings file was created by plugin aitsf2fix v1.0.0
## Plugin GUID: aitsf2fix

[Quality]

## Enable High Quality MSAA on all cameras
# Setting type: Boolean
# Default value: true
HighMSAA = true

## Force NoVSync setting
# Setting type: Boolean
# Default value: false
NoVSync = true

## Force this anisoLevel
# Setting type: Int32
# Default value: -1
ForceAniso = 16

[Resolution]

# Setting type: Int32
# Default value: 1920
X = 1366

# Setting type: Int32
# Default value: 1080
Y = 768

# Setting type: Boolean
# Default value: false
Override = false

# Setting type: Boolean
# Default value: false
Fullscreen = false

# Setting type: Boolean
# Default value: true
UIFix = true
```
