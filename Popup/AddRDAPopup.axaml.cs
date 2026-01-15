using Avalonia.Controls;
using RemoteHub.Classe;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace RemoteHub.Popup;

public partial class AddRDAPopup : Window
{
    private byte[]? _iconBytes = null;
    public AddRDAPopup()
    {
        InitializeComponent();
    }

    private async void ChooseIcon_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (this.StorageProvider is { } storageProvider)
        {
            var files = await storageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions
            {
                Title = "Selectionner une icône",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new Avalonia.Platform.Storage.FilePickerFileType("Images")
                    {
                        Patterns = new[] { "*.png", "*.jpg", "*.jpeg" }
                    }
                }
            });

            if (files.Count > 0)
            {
                var file = files[0];

                // Lecture du flux
                await using (var previewStream = await file.OpenReadAsync())
                {
                    IconPreview.Source = new Avalonia.Media.Imaging.Bitmap(previewStream);
                }

                // Lecture du flux pour la sauvegarde
                await using (var saveStream = await file.OpenReadAsync())
                {
                    using var ms = new MemoryStream();
                    await saveStream.CopyToAsync(ms);
                    _iconBytes = ms.ToArray();
                }
            }
        }
    }

    private void Add_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string rdaName = RDAName?.Text?.Trim() ?? "";
        string rdaUser = Name?.Text?.Trim() ?? "";
        string rdaPassword = Password?.Text?.Trim() ?? "";
        string rdaAddress = Address?.Text?.Trim() ?? "";
        string features;
        if (Son.IsChecked == true) { features = "Son"; } else { features = ""; }

        if (RDP.IsChecked == true)
        {
            string rdaSoftware = "";

            string iconBytes = ""; 
            var assembly = Assembly.GetExecutingAssembly(); 
            using Stream stream = assembly.GetManifestResourceStream("RemoteHub.Asset.rdp_icon.png"); 
            using MemoryStream ms = new MemoryStream(); stream.CopyTo(ms); 
            iconBytes = Convert.ToBase64String(ms.ToArray());

            if (string.IsNullOrWhiteSpace(rdaName) || string.IsNullOrEmpty(rdaUser) || string.IsNullOrEmpty(rdaPassword) || string.IsNullOrEmpty(rdaAddress))
            {
                new AlerteWindow("Les champs sont obligatoires.").ShowDialog(this);
                return;
            }
            else
            {
                RDAManager rdaManager = new RDAManager();
                rdaManager.AddRDA(rdaName, rdaAddress, rdaUser, rdaPassword, rdaSoftware, features, iconBytes); 
            }
        }
        else
        {
            string rdaSoftware = Software?.Text?.Trim() ?? "";
            string iconBytes = (_iconBytes != null && _iconBytes.Length > 0) ? Convert.ToBase64String(_iconBytes) : "";

            if (string.IsNullOrWhiteSpace(rdaName) || string.IsNullOrEmpty(rdaUser) || string.IsNullOrEmpty(rdaPassword) || string.IsNullOrEmpty(rdaAddress) || string.IsNullOrEmpty(iconBytes))
            {
                new AlerteWindow("Les champs sont obligatoires.").ShowDialog(this);
                return;
            }
            else
            {
                RDAManager rdaManager = new RDAManager();
                rdaManager.AddRDA(rdaName, rdaAddress, rdaUser, rdaPassword, rdaSoftware, features, iconBytes);
            }
        }
        this.Close(true);
    }

    private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close(false);
    }
}