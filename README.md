This is a faithful recreation of the classic Atari game "Kaboom!" written in C#. It was originally built on Microsoft XNA and has since been ported to [MonoGame](https://monogame.net/) (DesktopGL).

## Playing

Download `Kaboom.exe` and run it. It's a single self-contained file, so there's nothing else to install.

## Building

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download) or newer.

Run from source:

```
dotnet run --project Kaboom
```

Publish the single-file exe (output goes to `publish/Kaboom.exe`):

```
dotnet publish Kaboom -c Release -r win-x64 -o publish
```

Game assets in `KaboomContent/` are embedded into the exe at build time.
