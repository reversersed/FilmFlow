using FilmFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace FilmFlow.MainWindow.NavigationViews.SettingsView
{
    public class SettingsViewModel : ViewModelBase
    {
        //Private properties
        private Visibility _settingsApplyingTextConfirmation = Visibility.Collapsed;
        private string _currentSelectedLanguage;

        //Public properties
        public Visibility SettingsApplyingTitleConfirmation { get { return _settingsApplyingTextConfirmation; } set { _settingsApplyingTextConfirmation = value; OnPropertyChanged(nameof(SettingsApplyingTitleConfirmation)); } }
        public string[] LanguageList { get; set; } = new string[] { "Русский", "English" };
        public string CurrentSelectedLanguage { get { return _currentSelectedLanguage; } set { _currentSelectedLanguage = value; OnPropertyChanged(nameof(CurrentSelectedLanguage)); } }

        //Commands
        public ICommand ApplyChanges { get; }
        public ICommand RestartApplication { get; }
        public SettingsViewModel()
        {
            ApplyChanges = new ViewModelCommand(ApplyChangesCommand);
            RestartApplication = new ViewModelCommand(RestartApplicationCommand);

            switch(FilmFlow.Properties.Settings.Default.Language)
            {
                case "ru-RU":
                    CurrentSelectedLanguage = LanguageList[0];
                    break;
                case "en-US":
                    CurrentSelectedLanguage = LanguageList[1];
                    break;
            }
        }

        private void RestartApplicationCommand(object obj)
        {
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
        }

        private void ApplyChangesCommand(object obj)
        {
            //Language
            switch (CurrentSelectedLanguage)
            {
                case "Русский":
                    FilmFlow.Properties.Settings.Default.Language = "ru-RU";
                    break;
                case "English":
                    FilmFlow.Properties.Settings.Default.Language = "en-US";
                    break;
            }
            FilmFlow.Properties.Settings.Default.Save();
            //
            SettingsApplyingTitleConfirmation = Visibility.Visible;
        }
    }
}
