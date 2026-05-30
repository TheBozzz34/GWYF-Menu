# GWYF Menu

## Player install
1. Install BepInEx 5 for your game.
2. Download `GWYF.Menu.dll` from the releases for this repo.
3. Copy `GWYF.Menu.dll` into:
   - `<GameFolder>/BepInEx/plugins/GWYFMenu/`
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

dotnet build ./src/GWYF.Menu.Plugin/GWYF.Menu.Plugin.csproj -c Release
```

Output DLL:
`src/GWYF.Menu.Plugin/bin/Release/net472/GWYF.Menu.dll`
