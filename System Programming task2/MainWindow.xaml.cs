using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace System_Programming_task2;
public partial class MainWindow : Window, INotifyPropertyChanged
{
    private string copyAddress;
    private string destinationAddress;
    private long fileSize;
    private long progress = 0;
    private int percentage;
    private bool cancel = false;
    public int Percentage { get => percentage; set { percentage = value; OnPropertyChanged(); } }
    public long ProgressValue { get => progress; set { progress = value; OnPropertyChanged(); } }
    public long FileSize { get => fileSize; set { fileSize = value; OnPropertyChanged(); } }
    public string CopyAddress { get => copyAddress; set { copyAddress = value; OnPropertyChanged(); } }
    public string DestinationAddress { get => destinationAddress; set { destinationAddress = value; OnPropertyChanged(); } }
    public MainWindow()
    {
        InitializeComponent();
        ProgressValue = 0;
        FileSize = 100;
        DataContext = this;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        if (button is null) return;

        OpenFileDialog openFileDialog = new OpenFileDialog();

        openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
        string selectedFilePath = "";
        if (openFileDialog.ShowDialog() == true)
            selectedFilePath = openFileDialog.FileName;
        if (button.Name == "From")
            CopyAddress = selectedFilePath;
        else if (button.Name == "To")
            DestinationAddress = selectedFilePath;
        ProgressValue = 0;
        Percentage = 0;
    }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(CopyAddress))
        {
            MessageBox.Show("Copy address is not correct", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (string.IsNullOrEmpty(DestinationAddress))
        {
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            DestinationAddress = $"{directory}/{Guid.NewGuid()}.txt";
        }

        if (File.Exists(DestinationAddress))
            File.WriteAllText(DestinationAddress, string.Empty);
        else
            using (File.Create(DestinationAddress)) { }

        var text = File.ReadAllText(CopyAddress);
        FileSize = text.Length;
        ProgressValue = 0;
        new Thread(() =>
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (cancel) break;
                File.AppendAllText(DestinationAddress, text[i].ToString());
                ProgressValue = i + 1;
                Percentage = (int)(ProgressValue * 100 / FileSize);
                Thread.Sleep(50);
            }
        }).Start();
        cancel = false;

    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        cancel = true;
    }
}