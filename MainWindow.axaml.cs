using Avalonia.Controls;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.VisualTree;
using RemoteHub.Classe;
using RemoteHub.Popup;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;

namespace RemoteHub
{
    public partial class MainWindow : Window
    {
        internal ObservableCollection<RDAEntry> RDAEntries { get; } = new();
        internal RDAManager rdaManager = new RDAManager();

        public MainWindow()
        {
            InitializeComponent();
            ListRDA.LayoutUpdated += (_, _) => ListRDA_Loaded();

            UpdateRDAList();
        }

        private void UpdateRDAList()
        {
            ListRDA.Items.Clear();

            if (rdaManager.FileExists()) { 
                foreach (var entry in rdaManager.GetAllRDA())
                {
                    ListRDA.Items.Add(entry);
                }
            }
        }

        private void ListRDA_Loaded()
        {
            for (int i = 0; i < ListRDA.ItemCount; i++)
            {
                var container = ListRDA.ItemContainerGenerator.ContainerFromIndex(i);
                if (container is ListBoxItem listBoxItem)
                {
                    var buttons = listBoxItem.GetVisualDescendants().OfType<Button>().ToList();
                    if (buttons.Count == 1)
                    {
                        buttons[0].Click -= OpenRDA_Click;    // Open button

                        buttons[0].Click += OpenRDA_Click;    // Open button
                    }
                }
            }
        }

        private void AddRDA_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                var popup = new AddRDAPopup();
                popup.Closed += (_, _) => UpdateRDAList();
                popup.Show();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing AddSitePopup: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void Popup_Closed(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #region Interactions RDA
        private void ModifyRDA_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is MenuItem menu && menu.Tag is RDAEntry rdaEntry)
            {
                //OpenModifyRDAWindow(rdaEntry);
            }
        }

        private void OpenRDA_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is RDAEntry rdaEntry)
            {
                //penRDAConnection(rdaEntOry);
                string tempPath = Path.Combine(Path.GetTempPath(), $"{rdaEntry.Name}.rdp");
                string rdpContent = $@"
                full address:s:{rdaEntry.Address}
                remoteapplicationmode:i:1
                remoteapplicationprogram:s:{rdaEntry.Software}
                username:s:{rdaEntry.Username}
                desktopscalefactor:i:100
                compression:i:1
                keyboardhook:i:2
                connection type:i:7
                networkautodetect:i:1
                server port:i:3389
                authentication level:i:2
                promptcredentialonce:i:0
                prompt for credentials:i:0
                negotiate security layer:i:1
                enablecredsspsupport:i:1
                clipboard flags:i:51
                enablerdsaadauth:i:0
                drivestoredirect:s:C:\;
                ";
                File.WriteAllText(tempPath, rdpContent);

                //intégré les features

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string target = $"TERMSRV/{rdaEntry.Address}";
                    
                    CredentialManager.SaveCredential(target, rdaEntry.Username, rdaEntry.Password);
                    var Proc = Process.Start(new ProcessStartInfo
                    {
                        FileName = "mstsc.exe",
                        Arguments = $"\"{tempPath}\"",
                        UseShellExecute = true
                    });

                    Proc?.WaitForExit();

                    CredentialManager.DeleteCredential(target);
                    File.Delete(tempPath);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    var parts = rdaEntry.Username.Split('\\');
                    string domain = parts[0];
                    string user = parts[1];

                    var Proc = Process.Start(new ProcessStartInfo
                    {
                        FileName = "xfreerdp",
                        Arguments = $"/u:{user} /p:\"{rdaEntry.Password}\" /v:{rdaEntry.Address} /app:\"{rdaEntry.Software}\"",
                        UseShellExecute = true
                    });

                    Proc.WaitForExit(); ;
                    File.Delete(tempPath);
                }
            }
        }

        private void DeleteRDA_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is MenuItem menu && menu.Tag is RDAEntry rdaEntry)
            {
                rdaManager.DeleteRDA(rdaEntry);
                UpdateRDAList();
            }
            UpdateRDAList();
        }
        #endregion
    }
}