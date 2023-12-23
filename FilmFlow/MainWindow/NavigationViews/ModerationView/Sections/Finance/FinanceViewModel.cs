using FilmFlow.Models;
using FilmFlow.ViewModels;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace FilmFlow.MainWindow.NavigationViews.ModerationView.Sections.Finance
{
    public class FinanceViewModel : ViewModelBase
    {
        //Private properties
        private bool _allTimeMode = true;
        private DateOnly StartDate;
        private DateOnly EndDate;
        private string _startDateString = string.Empty;
        private string _endDateString = string.Empty;
        private int _totalSubscriptions = 0;
        private int _totalSubCost = 0;
        private int _totalReplenishments = 0;
        private int _totalReplValue = 0;
        private GenreModel _popularGenre;
        private Visibility _reportVisibility = Visibility.Collapsed;
        private ObservableCollection<GenreModel> _genreReport;
        private SeriesCollection _reportSeries;

        //Public properties
        public SeriesCollection ReportSeries { get { return _reportSeries; } set { _reportSeries = value; OnPropertyChanged(nameof(ReportSeries)); } }
        public bool AllTimeMode { get { return _allTimeMode; } set { _allTimeMode = value; OnPropertyChanged(nameof(AllTimeMode)); } }
        public string StartDateString { get { return _startDateString; } set {
                int date;
                string trimDate = value.Replace(".", "");
                if ((trimDate != null && trimDate.Length > 0) && (!Int32.TryParse(trimDate, out date) || date < 0))
                    return;
                if (trimDate.Length == 6)
                {
                    try
                    {
                        DateOnly correctDate;
                        if (!DateOnly.TryParse("01." + value, out correctDate) || correctDate.Year < 1800 || correctDate.Year > DateTime.UtcNow.Year + 1)
                            return;
                        StartDate = correctDate;
                    }catch(Exception) { return; }
                }
                else
                    StartDate = DateOnly.MinValue;
                if (trimDate.Length > 2)
                    _startDateString = trimDate.Insert(2, ".");
                else
                    _startDateString = trimDate;
                OnPropertyChanged(nameof(StartDateString));
            } 
        }
        public string EndDateString { get { return _endDateString; } set {
                int date;
                string trimDate = value.Replace(".", "");
                if ((trimDate != null && trimDate.Length > 0) && (!Int32.TryParse(trimDate, out date) || date < 0))
                    return;
                if (trimDate.Length == 6)
                {
                    try
                    {
                        DateOnly correctDate;
                        if (!DateOnly.TryParse(DateTime.DaysInMonth(Int32.Parse(value.Split('.')[1]), Int32.Parse(value.Split('.')[0])).ToString() + "." + value,
                            out correctDate) || correctDate.Year < 1800 || correctDate.Year > DateTime.UtcNow.Year + 1 || (StartDate.Year > 1 && StartDate > correctDate))
                            return;
                        EndDate = correctDate;
                    }
                    catch (Exception) { return; }
                }
                else
                    EndDate = DateOnly.MinValue;
                if (trimDate.Length > 2)
                    _endDateString = trimDate.Insert(2, ".");
                else
                    _endDateString = trimDate;
                OnPropertyChanged(nameof(EndDateString));
            } 
        }
        public ObservableCollection<GenreModel> GenreReport { get { return _genreReport; } set { _genreReport = value; OnPropertyChanged(nameof(GenreReport)); } }
        public int TotalSubscriptions { get { return _totalSubscriptions; } set { _totalSubscriptions = value; OnPropertyChanged(nameof(TotalSubscriptions)); OnPropertyChanged(nameof(AverageSubCost)); } }
        public int TotalSubCost { get { return _totalSubCost; } set { _totalSubCost = value; OnPropertyChanged(nameof(TotalSubCost)); OnPropertyChanged(nameof(AverageSubCost)); } }
        public int TotalReplenishments { get { return _totalReplenishments; } set { _totalReplenishments = value; OnPropertyChanged(nameof(TotalReplenishments)); OnPropertyChanged(nameof(AverageReplenishment)); } }
        public int TotalReplValue { get { return _totalReplValue; } set { _totalReplValue = value; OnPropertyChanged(nameof(TotalReplValue)); OnPropertyChanged(nameof(AverageReplenishment)); } }
        public int AverageReplenishment { get { return TotalReplenishments == 0 ? 0 : (int)Math.Round((double)TotalReplValue/(double)TotalReplenishments); } }
        public int AverageSubCost { get { return TotalSubscriptions == 0 ? 0 : (int)Math.Round((double)TotalSubCost/(double)TotalSubscriptions); } }
        public GenreModel PopularGenre { get { return _popularGenre; } set { _popularGenre = value; OnPropertyChanged(nameof(PopularGenre)); } }
        public Visibility ReportVisibility { get { return _reportVisibility; } set { _reportVisibility = value; OnPropertyChanged(nameof(ReportVisibility)); } }

        //Commands
        public ICommand GetReport { get; }
        public ICommand DownloadReport { get; }

        //Repositories
        IUserRepository userRepository;

        //Methods
        public FinanceViewModel()
        {
            userRepository = new UserRepository();

            GetReport = new ViewModelCommand(LoadReport, x => AllTimeMode ? true : (StartDate.Year > 1 && StartDate <= EndDate));
            DownloadReport = new ViewModelCommand(Download, x => ReportVisibility == Visibility.Visible);

            EndDateString = DateTime.UtcNow.ToUniversalTime().ToString("MM.yyyy");
            StartDateString = DateTime.UtcNow.ToUniversalTime().AddYears(-1).ToString("MM.yyyy");

            ReportVisibility = Visibility.Collapsed;
        }

        private void Download(object obj)
        {
            using FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult key = dialog.ShowDialog();
            if(key == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                using(StreamWriter writer = new StreamWriter(dialog.SelectedPath+"/report.csv"))
                {
                    writer.WriteLine((System.Windows.Application.Current.FindResource("TotalRepl") as string)+";"+TotalReplenishments);
                    writer.WriteLine((System.Windows.Application.Current.FindResource("SumRepl") as string)+";"+TotalReplValue+ "₽");
                    writer.WriteLine((System.Windows.Application.Current.FindResource("AverageRepl") as string)+";"+AverageReplenishment+"₽");
                    writer.WriteLine();
                    writer.WriteLine((System.Windows.Application.Current.FindResource("TotalSubs") as string)+";"+TotalSubscriptions);
                    writer.WriteLine((System.Windows.Application.Current.FindResource("SumSubs") as string)+";"+TotalSubCost+ "₽");
                    writer.WriteLine((System.Windows.Application.Current.FindResource("AverageSubs") as string)+";"+AverageSubCost+ "₽");
                    writer.WriteLine((System.Windows.Application.Current.FindResource("TopGenre") as string)+";"+PopularGenre.Name);

                    writer.WriteLine();
                    writer.WriteLine(System.Windows.Application.Current.FindResource("GenreRateTitle") as string);
                    writer.WriteLine(System.Windows.Application.Current.FindResource("GenreReport") as string+";"+System.Windows.Application.Current.FindResource("RateGenre") as string);
                    foreach(var i in GenreReport)
                        writer.WriteLine(i.Name+";"+i.Id+"%");
                }
            }
        }

        private void LoadReport(object obj)
        {
            TotalSubscriptions = userRepository.GetTotalSubscriptions(AllTimeMode ? DateOnly.MinValue : StartDate, AllTimeMode ? DateOnly.MaxValue : EndDate);
            TotalSubCost = userRepository.GetTotalSubscriptionCost(AllTimeMode ? DateOnly.MinValue : StartDate, AllTimeMode ? DateOnly.MaxValue : EndDate);
            PopularGenre = userRepository.GetPopularSubscriptionGenre(AllTimeMode ? DateOnly.MinValue : StartDate, AllTimeMode ? DateOnly.MaxValue : EndDate);

            TotalReplenishments = userRepository.GetTotalReplenishments(AllTimeMode ? DateOnly.MinValue : StartDate, AllTimeMode ? DateOnly.MaxValue : EndDate);
            TotalReplValue = userRepository.GetTotalReplenishmentValue(AllTimeMode ? DateOnly.MinValue : StartDate, AllTimeMode ? DateOnly.MaxValue : EndDate);
             
            GenreReport = new ObservableCollection<GenreModel>(userRepository.GetReportList(AllTimeMode ? DateOnly.MinValue : StartDate, AllTimeMode ? DateOnly.MaxValue : EndDate).OrderByDescending(i => i.Id));

            int toOther = 0;
            int maxOther = 30;
            ReportSeries = new SeriesCollection();
            foreach(var i in GenreReport.OrderBy(i => i.Id))
            {
                if(toOther < maxOther && i.Id < maxOther)
                    toOther += i.Id;
                else
                    ReportSeries.Add(new PieSeries{ Title = i.Name, Values = new ChartValues<int> { i.Id } });
            }
            if(toOther > 0)
                ReportSeries.Add(new PieSeries { Title = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? "Другое" : "Other", Values = new ChartValues<int> { toOther } });

            ReportVisibility = Visibility.Visible;
            return;
        }
    }
}
