using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FilmFlow.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для RatingBarControl.xaml
    /// </summary>
    public partial class RatingBarControl : UserControl
    {
        public static readonly DependencyProperty RatingProperty = DependencyProperty.Register("Rating", typeof(double), typeof(RatingBarControl),
            new PropertyMetadata(new PropertyChangedCallback(OnRatingChanged)));
        public static readonly DependencyProperty MouseMoveProperty = DependencyProperty.Register("OnMouseMoveEvent", typeof(ICommand), typeof(RatingBarControl));
        public static readonly DependencyProperty MouseStateProperty = DependencyProperty.Register("OnMouseStateEvent", typeof(ICommand), typeof(RatingBarControl));
        public double Rating { get { return (double)GetValue(RatingProperty); } set { SetValue(RatingProperty, value); } }

        private static void OnRatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => UpdateRatingCommand?.Invoke();
        private static Action UpdateRatingCommand;
       
        public ICommand OnMouseMoveEvent { get { return (ICommand)GetValue(MouseMoveProperty); } set { SetValue(MouseMoveProperty, value); } }
        public ICommand OnMouseStateEvent { get { return (ICommand)GetValue(MouseStateProperty); } set { SetValue(MouseStateProperty, value); } }
        private void UpdateRatingValue()
        {
            Bar_1.Value = Bar_2.Value = Bar_3.Value = Bar_4.Value = Bar_5.Value = 0;
            switch (Rating)
            {
                case < 20:
                    Bar_1.Value = Rating*5;
                    break;
                case < 40:
                    Bar_1.Value = 100;
                    Bar_2.Value = (Rating - 20*1) * 5;
                    break;
                case < 60:
                    Bar_1.Value = Bar_2.Value = 100;
                    Bar_3.Value = (Rating - 20 * 2) * 5;
                    break;
                case < 80:
                    Bar_1.Value = Bar_2.Value = Bar_3.Value = 100;
                    Bar_4.Value = (Rating - 20 * 3) * 5;
                    break;
                case < 100:
                    Bar_1.Value = Bar_2.Value = Bar_3.Value = Bar_4.Value = 100;
                    Bar_5.Value = (Rating - 20 * 4) * 5;
                    break;
                default://5
                    Bar_1.Value = Bar_2.Value = Bar_3.Value = Bar_4.Value = Bar_5.Value = 100;
                    break;
            }
        }
        public RatingBarControl()
        {
            InitializeComponent();
            UpdateRatingCommand += UpdateRatingValue;
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMoveEvent?.Execute((float)((e.GetPosition(this).X / (this).ActualWidth) * 5));
            if (OnMouseMoveEvent != null)
                BarGrid.IsEnabled = true;
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnMouseStateEvent?.Execute(true);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            OnMouseStateEvent?.Execute(false);
        }
    }
}
