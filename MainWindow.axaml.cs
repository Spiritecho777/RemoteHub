using RemoteHub.Classe;
using RemoteHub.Popup;
using Avalonia.Controls;
using Avalonia.VisualTree;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Diagnostics;

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
                    if (buttons.Count == 3)
                    {
                        buttons[0].Click -= OpenRDA_Click;    // Open button
                        buttons[1].Click -= ModifyRDA_Click;   // Modify button
                        buttons[2].Click -= DeleteRDA_Click; // Delete button

                        buttons[0].Click += OpenRDA_Click;    // Open button
                        buttons[1].Click += ModifyRDA_Click;   // Modify button
                        buttons[2].Click += DeleteRDA_Click; // Delete button
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
            if (sender is Button button && button.DataContext is RDAEntry rdaEntry)
            {
                //OpenModifyRDAWindow(rdaEntry);
            }
        }

        private void OpenRDA_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is RDAEntry rdaEntry)
            {
                //OpenRDAConnection(rdaEntry);
            }
        }

        private void DeleteRDA_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is RDAEntry rdaEntry)
            {
                //DeleteRDAEntry(rdaEntry);
            }
        }
        #endregion
    }
}