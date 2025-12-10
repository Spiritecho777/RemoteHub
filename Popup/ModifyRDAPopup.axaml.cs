using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RemoteHub.Classe;
using System;
using System.Diagnostics;
using System.IO;

namespace RemoteHub.Popup;

public partial class ModifyRDAPopup : Window
{
    private byte[]? _iconBytes = null;
    private RDAEntry oldEntry;
    public ModifyRDAPopup(RDAEntry rdaEntry)
    {
        InitializeComponent();
        oldEntry = rdaEntry;
        RDAName.Text = rdaEntry.Name;
        Name.Text = rdaEntry.Username;
        Password.Text = rdaEntry.Password;
        Address.Text = rdaEntry.Address;
        Software.Text = rdaEntry.Software;
        //Features.Text = rdaEntry.Features;
        IconPreview.Source = rdaEntry.IconBitmap;
        _iconBytes = Convert.FromBase64String(rdaEntry.Icon);
    }

    private void Advanced_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //var advancedPopup = new AdvancedRDAPopup();
        //advancedPopup.ShowDialog(this);
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

    private void Modify_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string rdaName = RDAName?.Text?.Trim() ?? "";
        string rdaUser = Name?.Text?.Trim() ?? "";
        string rdaPassword = Password?.Text?.Trim() ?? "";
        string rdaAddress = Address?.Text?.Trim() ?? "";
        string rdaSoftware = Software?.Text?.Trim() ?? "";
        string iconBytes = (_iconBytes != null && _iconBytes.Length > 0) ? Convert.ToBase64String(_iconBytes) : "";
        Debug.WriteLine("Icon Bytes Length: " + rdaPassword);
        if (string.IsNullOrWhiteSpace(rdaName) || string.IsNullOrEmpty(rdaUser) || string.IsNullOrEmpty(rdaPassword) || string.IsNullOrEmpty(rdaAddress) || string.IsNullOrEmpty(iconBytes))
        {
            new AlerteWindow("Les champs sont obligatoires.").ShowDialog(this);
            return;
        }
        else
        {
            RDAEntry newEntry = new RDAEntry()
            {
                Name = rdaName,
                Address = rdaAddress,
                Username = rdaUser,
                Password = rdaPassword,
                Software = rdaSoftware,
                Features = "",
                Icon = iconBytes

            };

            RDAManager rdaManager = new RDAManager();
            rdaManager.ModifyRDA(oldEntry,newEntry);
        }
        this.Close(true);
    }

    private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close(false);
    }
}