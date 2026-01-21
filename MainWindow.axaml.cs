using Avalonia.Controls;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.VisualTree;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using RemoteHub.Classe;
using RemoteHub.Popup;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

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
                var popup = new ModifyRDAPopup(rdaEntry);
                popup.Closed += (_, _) => UpdateRDAList();
                popup.Show();
            }
        }

        private void OpenRDA_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is RDAEntry rdaEntry)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string tempPath = Path.Combine(Path.GetTempPath(), $"{rdaEntry.Name}.rdp");

                    var sb = new StringBuilder(); 
                    sb.AppendLine($"full address:s:{rdaEntry.Address}"); 
                    sb.AppendLine($"username:s:{rdaEntry.Username}"); 
                    sb.AppendLine("desktopscalefactor:i:100"); 
                    sb.AppendLine("compression:i:1"); 
                    sb.AppendLine("keyboardhook:i:2"); 
                    sb.AppendLine("connection type:i:7"); 
                    sb.AppendLine("networkautodetect:i:1"); 
                    sb.AppendLine("server port:i:3389"); 
                    sb.AppendLine("authentication level:i:2"); 
                    sb.AppendLine("promptcredentialonce:i:0"); 
                    sb.AppendLine("prompt for credentials:i:0"); 
                    sb.AppendLine("negotiate security layer:i:1"); 
                    sb.AppendLine("enablecredsspsupport:i:1"); 
                    sb.AppendLine("clipboard flags:i:51"); 
                    sb.AppendLine("enablerdsaadauth:i:0"); 
                    sb.AppendLine("drivestoredirect:s:C:\\;");
                
                    // Ajout conditionnel des features 
                
                    // RemoteApp
                    if (!string.IsNullOrWhiteSpace(rdaEntry.Software))
                    { 
                        sb.AppendLine("remoteapplicationmode:i:1");
                        sb.AppendLine($"remoteapplicationprogram:s:{rdaEntry.Software}"); 
                    } 
                
                    // Audio
                    if (rdaEntry.Features.Contains("Son")) 
                    { 
                        sb.AppendLine("audiocapturemode:i:1"); 
                        sb.AppendLine("audiomode:i:0"); 
                        sb.AppendLine("audioqualitymode:i:2"); 
                    }

                    File.WriteAllText(tempPath, sb.ToString());

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
                    string args = "";

                    args = $"/u:{user} /p:\"{rdaEntry.Password}\" /v:{rdaEntry.Address} /dynamic-resolution /cert:ignore";

                    if (rdaEntry.Features.Contains("Son")) { args += " /sound:sys:pulse /microphone:sys:pulse"; }
                    if (rdaEntry.Software != "") { args += $" /app:\"{rdaEntry.Software}\""; }

                    var Proc = Process.Start(new ProcessStartInfo
                    {
                        FileName = "xfreerdp",
                        Arguments = args,
                        UseShellExecute = false
                    });
                    Console.WriteLine(Proc.ToString());
                    //Proc.WaitForExit(); ;
                    //File.Delete(tempPath);
                }
            }
        }

        private async void About_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var infoVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            string version = infoVersion?.Split('+')[0];
            var box = MessageBoxManager.GetMessageBoxStandard("Version", "Version: " + version, ButtonEnum.Ok); 
            await box.ShowAsync();
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