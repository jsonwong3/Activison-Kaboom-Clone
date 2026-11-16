using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Kaboom
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            PreloadNativeLibraries();

            using (Kaboom game = new Kaboom())
            {
                game.Run();
            }
        }

        /// <summary>
        /// In a single-file build the native libraries (SDL2, OpenAL) are extracted to a temp
        /// folder, but MonoGame only looks next to the exe. Loading them up front from the
        /// runtime's native search directories lets MonoGame find the already-loaded modules.
        /// </summary>
        static void PreloadNativeLibraries()
        {
            string dirs = AppContext.GetData("NATIVE_DLL_SEARCH_DIRECTORIES") as string;
            if (string.IsNullOrEmpty(dirs)) return;

            foreach (string lib in new[] { "SDL2.dll", "soft_oal.dll" })
            {
                foreach (string dir in dirs.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
                {
                    string path = Path.Combine(dir, lib);
                    if (File.Exists(path) && NativeLibrary.TryLoad(path, out _)) break;
                }
            }
        }
    }
}
