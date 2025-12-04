using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace RemoteHub
{
    public partial class App : Application
    {
        private static Mutex? mutex;
        private static bool mutexAcquired = false;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            bool isNewInstance = AcquireCrossPlatformMutex("RemoteHub_Mutex");

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();

                desktop.Exit += OnAppExit;
                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            }

            base.OnFrameworkInitializationCompleted();
        }

        #region Manipulation faite suite a la fermeture - crash du logiciel
        // Méthode appelée lorsque l'application se termine proprement
        private void OnAppExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            DeleteFile();
            ReleaseMutex();
        }

        // Méthode appelée lorsque le processus est tué ou termine
        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            DeleteFile();
            ReleaseMutex();
        }

        private void ReleaseMutex()
        {
            if (mutexAcquired && mutex is not null)
            {
                try { mutex.ReleaseMutex(); } catch { }
            }
        }

        private void DeleteFile()
        {
            string appDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RemoteHubData");
            string filePath = System.IO.Path.Combine(appDirectory, "RDA_decrypted.dat");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            string filePath2 = System.IO.Path.Combine(appDirectory, "RDA.dat");

            if (File.Exists(filePath2))
            {
                long fileSize = new FileInfo(filePath2).Length;

                if (fileSize == 0 && File.Exists(filePath2))
                {
                    File.Delete(filePath2);
                }
            }
        }

        //Mutex cross-plateforme
        private bool AcquireCrossPlatformMutex(string mutexName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                mutex = new Mutex(true, mutexName, out bool isNewInstance);
                mutexAcquired = isNewInstance;
                return isNewInstance;
            }
            else
            {
                string lockPath = Path.Combine(Path.GetTempPath(), mutexName + ".lock");

                try
                {
                    if (File.Exists(lockPath))
                    {
                        // Vérifier si le processus existe toujours
                        try
                        {
                            string pidStr = File.ReadAllText(lockPath);
                            if (int.TryParse(pidStr, out int pid))
                            {
                                Process.GetProcessById(pid);
                                // Le processus existe, on ne peut pas acquérir le mutex
                                return false;
                            }
                        }
                        catch
                        {
                            // Le processus n'existe plus, supprimer le fichier
                            File.Delete(lockPath);
                        }
                    }

                    File.WriteAllText(lockPath, Process.GetCurrentProcess().Id.ToString());
                    mutexAcquired = true;

                    AppDomain.CurrentDomain.ProcessExit += (_, _) =>
                    {
                        try { File.Delete(lockPath); } catch { }
                    };
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        #endregion
    }
}