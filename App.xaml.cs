using FilmFlow.Login;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
            string? languageName = getCurrentLanguage();
            languageDictionary.Source = new Uri("..\\Lang\\Lang."+(languageName == null ? "en-US" : languageName).ToString()+ ".xaml",UriKind.Relative);

            App.Current.Resources.MergedDictionaries.Add(languageDictionary);
            
            LoginView loginView = new LoginView();
            loginView.Show();
        }
        private string? getCurrentLanguage()
        {
            var config = new XmlDocument();
            try
            {
                config.Load("config.xml");
            }
            catch
            {
                new XDocument(new XElement("config", new XElement("language", CultureInfo.CurrentCulture.Name))).Save("config.xml");
                config.Load("config.xml");
            }
            return config.DocumentElement?.SelectSingleNode("/config/language")?.InnerText;
        }
    }
}
