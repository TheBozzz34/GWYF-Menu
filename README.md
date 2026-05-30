# GWYF Menu

This repository is now set up as a **BepInEx plugin** so players can install the mod without manually patching `Assembly-CSharp.dll` in dnSpy.

## Player install (no binary patching)
1. Install BepInEx 5 for your game.
2. Download `GWYF.Menu.dll` from the releases for this repo.
3. Copy `GWYF.Menu.dll` into:
   - `<GameFolder>/BepInEx/plugins/GWYF.Menu/`
4. Launch the game.
5. Press `F8` to toggle the menu.

The plugin hooks `AllPlayersTriggerZone.OnStartClient` via Harmony and creates `__DebugWindow` automatically.

## Build from source
You need the game's Unity managed assemblies available.

### Windows (PowerShell)
```powershell
$env:GAME_MANAGED_DIR = "C:\Path\To\Game\Game_Data\Managed"
$env:BEPINEX_CORE_DIR = "C:\Path\To\Game\BepInEx\core"
dotnet build .\src\GWYF.Menu.Plugin\GWYF.Menu.Plugin.csproj -c Release
```

### Any OS
```bash
GAME_MANAGED_DIR="/path/to/Game_Data/Managed" \
BEPINEX_CORE_DIR="/path/to/Game/BepInEx/core" \
dotnet build /tmp/workspace/TheBozzz34/GWYF-Menu/src/GWYF.Menu.Plugin/GWYF.Menu.Plugin.csproj -c Release
```

Output DLL:
`src/GWYF.Menu.Plugin/bin/Release/net472/GWYF.Menu.dll`
