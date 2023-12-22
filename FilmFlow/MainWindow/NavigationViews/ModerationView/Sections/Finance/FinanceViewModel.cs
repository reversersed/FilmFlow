using FilmFlow.Models;
using FilmFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FilmFlow.MainWindow.NavigationViews.ModerationView.Sections.Finance
{
    public class FinanceViewModel : ViewModelBase
    {
        //Private properties
        private bool _allTimeMode {  get; set; }
        private DateOnly StartDate {  get; set; }
        private DateOnly EndDate {  get; set; }
        private string _startDateString = string.Empty;
        private string _endDateString = string.Empty;

        //Public properties
        public bool AllTimeMode { get { return _allTimeMode; } set { _allTimeMode = value; OnPropertyChanged(nameof(AllTimeMode)); } }
        public string StartDateString { get { return _startDateString; } set {
                int date;
                string trimDate = value.Replace(".", "");
                if ((trimDate != null && trimDate.Length > 0) && (!Int32.TryParse(trimDate, out date) || date < 0))
                    return;
                if (trimDate.Length == 6)
                {
                    DateOnly correctDate;
                    if (!DateOnly.TryParse("01." + value, out correctDate) || correctDate.Year < 1800 || correctDate.Year > DateTime.UtcNow.Year + 1)
                        return;
                    StartDate = correctDate;
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
                    DateOnly correctDate;
                    if (!DateOnly.TryParse("01."+value, out correctDate) || correctDate.Year < 1800 || correctDate.Year > DateTime.UtcNow.Year + 1 || (StartDate.Year > 1 && StartDate > correctDate))
                        return;
                    EndDate = correctDate;
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

        //Commands
        public ICommand GetReport { get; }

        //Repositories
        IUserRepository userRepository;

        //Methods
        public FinanceViewModel()
        {
            userRepository = new UserRepository();

            GetReport = new ViewModelCommand(LoadReport, x => AllTimeMode ? true : (StartDate.Year > 1 && StartDate <= EndDate));
        }

        private void LoadReport(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
