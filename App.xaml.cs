using FilmFlow.Login;
using FilmFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;

namespace FilmFlow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void ApplicationStart(object sender, EventArgs e)
        {

            ResourceDictionary languageDictionary = new ResourceDictionary();
            if (FilmFlow.Properties.Settings.Default.Language.Length < 2)
            {
                FilmFlow.Properties.Settings.Default.Language = CultureInfo.CurrentCulture.Name;
                FilmFlow.Properties.Settings.Default.Save();
            }
            languageDictionary.Source = new Uri("..\\Lang\\Lang."+FilmFlow.Properties.Settings.Default.Language+ ".xaml",UriKind.Relative);

            App.Current.Resources.MergedDictionaries.Add(languageDictionary);
            
            LoginView loginView = new LoginView();
            LoginViewModel viewModel = loginView.DataContext as LoginViewModel;
            viewModel.showRegistrationWindow = ShowRegistration;

            loginView.Show();
        }
        private void ShowRegistration(object? obj)
        {
            throw new NotImplementedException();
        }
    }
}
