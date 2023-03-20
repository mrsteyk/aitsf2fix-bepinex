# AI: THE SOMNIUM FILES - nirvanA Initiative Fix (port to BepInEx6 BE)

This is a WIP BepInEx6 BE version of [AI: THE SOMNIUM FILES - nirvanA Initiative Fix](https://github.com/Lyall/AISomniumFiles2Fix), with debug menu fixes maybe potentially.

Fixes done:

- `BustShot` fake folder stuff to populate treeview.
- Hide `EventViewer`, because code for it is stripped completely and it overlays other windows.
- Restore partial functionality of `FlagViewer` by jankiest means possible.
- Most `TopMenu` buttons work.

## Configuration

```ini
## Settings file was created by plugin aitsf2fix v1.0.0
## Plugin GUID: aitsf2fix

[Quality]

## Enable High Quality MSAA on all cameras
# Setting type: Boolean
# Default value: true
HighMSAA = true

[Resolution]

# Setting type: Int32
# Default value: Main Display Width
X = 1920

# Setting type: Int32
# Default value: Main Display Height
Y = 1080

# Setting type: Boolean
# Default value: false
Override = false

# Setting type: Boolean
# Default value: false
Fullscreen = false

# Setting type: Boolean
# Default value: true
UIFix = true # doesn't do anything as of rn
```
