using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace XamarinPclToggle
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private readonly string _basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Reference Assemblies\Microsoft\Framework\.NETPortable");
        private string _message;

        public MainViewModel()
        {
            DisableXamarinPclCommand = new RelayCommand<object>(DisableXamarinPcl, CanDisableXamarinPcl);
            EnableXamarinPclCommand = new RelayCommand<object>(EnableXamarinPcl, CanEnableXamarinPcl);
        }

        public ICommand DisableXamarinPclCommand { get; private set; }


        public ICommand EnableXamarinPclCommand { get; private set; }

        private bool CanDisableXamarinPcl(object notUsed)
        {
            return Directory.EnumerateFileSystemEntries(_basePath, "Xamarin.*.xml", SearchOption.AllDirectories).Any();
        }

        private async void DisableXamarinPcl(object notUsed)
        {
            Message = "";

            await Task.Run(() =>
                           {
                               foreach (var file in Directory.EnumerateFileSystemEntries(_basePath, "Xamarin.*.xml", SearchOption.AllDirectories))
                               {
                                   Trace.WriteLine(string.Format("Renaming {0} to {1}", file, file + ".disabled"));
                                   File.Move(file, file + ".disabled");
                               }
                           });

            CommandManager.InvalidateRequerySuggested();
            Message = "Done Disabling";
        }


        private bool CanEnableXamarinPcl(object notUsed)
        {
            return Directory.EnumerateFileSystemEntries(_basePath, "Xamarin.*.xml.disabled", SearchOption.AllDirectories).Any();
        }

        private async void EnableXamarinPcl(object notUsed)
        {
            Message = "";

            await Task.Run(() =>
                           {
                               foreach (var file in Directory.EnumerateFileSystemEntries(_basePath, "Xamarin.*.xml.disabled", SearchOption.AllDirectories))
                               {
                                   Trace.WriteLine(string.Format("Renaming {0} to {1}", file, file.Substring(0, file.Length - 9)));
                                   File.Move(file, file.Substring(0, file.Length - 9));
                               }
                           });

            CommandManager.InvalidateRequerySuggested();
            Message = "Done Enabling";
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}