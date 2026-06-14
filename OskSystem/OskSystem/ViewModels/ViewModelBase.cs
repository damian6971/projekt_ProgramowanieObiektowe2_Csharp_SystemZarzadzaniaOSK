// Wszystkie ViewModele dziedziczą stąd, żeby ekran odświeżał się po zmianie danych.
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OskSystem.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
