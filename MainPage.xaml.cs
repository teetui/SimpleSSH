using Microsoft.Extensions.Logging.Abstractions;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SimpleSSH
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly Login login = new Login();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            login.Host = HostTextBox.Text;
            login.UserID = UserIDTextBox.Text;
            login.Password = PasswordBox.Password;

            string result;
            if (CommandTextBox.Text != string.Empty)
                result = login.Command(CommandTextBox.Text);
            else
                result = "No command specified.";

            // Create the message dialog and set its content
            var messageDialog = new MessageDialog(result);
            await messageDialog.ShowAsync();
        }

        private async void KeyFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add(".ppk");

            StorageFile keyFile = await openPicker.PickSingleFileAsync();
            if (keyFile != null)
            {
                // Application now has read/write access to the picked file
                KeyTextBox.Text = keyFile.Path;

                // copy keyfile to local folder,
                // because UWP file access permissions
                // don't seem to work in SSH.Net
                StorageFile newKeyFile = await keyFile.CopyAsync(ApplicationData.Current.LocalFolder, 
                    keyFile.Name, NameCollisionOption.ReplaceExisting);
                login.SetKeyFile(newKeyFile);
            }
        }
    }
}
