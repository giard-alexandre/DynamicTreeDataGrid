using System;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

using SampleApp.ViewModels;

namespace SampleApp.Views;

public partial class MainWindow : Window {
    private static readonly FilePickerFileType JsonFileType = new("JSON") {
        Patterns = ["*.json"], AppleUniformTypeIdentifiers = ["public.json"], MimeTypes = ["application/json"]
    };

    public MainWindow() { InitializeComponent(); }

    private void Click_SaveToFile(object? sender, RoutedEventArgs e) {
        if (DataContext is not MainWindowViewModel vm) {
            return;
        }

        Dispatcher.UIThread.Invoke(async () => {
            var file = await this.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions {
                Title = "Select Save File",
                DefaultExtension = "json",
                FileTypeChoices = [JsonFileType],
                ShowOverwritePrompt = true,
            });

            if (file is null) {
                return;
            }

            await using var stream = await file.OpenWriteAsync();
            await vm.StoreStateAsync(stream);
        });
    }

    private void Click_LoadFromFile(object? sender, RoutedEventArgs e) {
        if (DataContext is not MainWindowViewModel vm) {
            return;
        }

        Dispatcher.UIThread.Invoke(async () => {
            var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
                Title = "Select File to Load", AllowMultiple = false, FileTypeFilter = [JsonFileType],
            });

            if (files.Count is 0 or > 1) {
                Console.WriteLine("A single file must be selected!");
                return;
            }

            await using var stream = await files[0].OpenReadAsync();
            await vm.LoadStateAsync(stream);
        });
    }
}
