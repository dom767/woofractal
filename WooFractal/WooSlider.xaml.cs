using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WooFractal
{
    public interface IGUIUpdateable
    {
        void GUIUpdate();
    }
    /// <summary>
    /// Interaction logic for WooSlider.xaml
    /// </summary>
    public partial class WooSlider : UserControl
    {
        public WooSlider()
        {
            InitializeComponent();
        }

        double _Value;
        double _Range;
        bool _Signed;
        IGUIUpdateable _GUIUpdateTarget;

        public void Set(double value, double range, bool signed, IGUIUpdateable guiTarget)
        {
            _Value = value;
            _Range = range;
            _Signed = signed;
            _GUIUpdateTarget = guiTarget;
        }

        public double GetSliderValue()
        {
            if (_Value > -0.000000000001 && _Value < 0.00000000000001)
                return 0.0;
            else
                return _Value;
        }

        bool _ValueDrag;
        Point _LastPos;

        private void rectangle1_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_ValueDrag)
            {
                System.Console.WriteLine("wspmlbup");
                Mouse.Capture(null);
                Point _NewPos = e.GetPosition(this);
                double delta = _NewPos.X - _LastPos.X;
                _Value += (_Signed ? 2 : 1) * _Range * delta / this.ActualWidth;
                ValueUpdated(true);
                _ValueDrag = false;
            }
        }

        private void ValueUpdated(bool updateGUI)
        {
            grid.Width = this.ActualWidth;
            rectangle1.Width = this.ActualWidth;

            if (_Signed)
            {
                double controlMid = this.ActualWidth * 0.5;
                double controlWidth = this.ActualWidth * 0.5;

                if (_Value < 0)
                {
                    Thickness marg = rectangle2.Margin;
                    marg.Left = controlMid + _Value * controlWidth / _Range;
                    rectangle2.Margin = marg;
                    rectangle2.Width = -_Value * controlWidth / _Range;
                }
                else
                {
                    Thickness marg = rectangle2.Margin;
                    marg.Left = controlMid;
                    rectangle2.Margin = marg;
                    rectangle2.Width = _Value * controlWidth / _Range;
                }
            }
            else
            {
                Thickness marg = rectangle2.Margin;
                marg.Left = 0;
                rectangle2.Margin = marg;
                rectangle2.Width = _Value * this.ActualWidth / _Range;
            }

            if (updateGUI)
            {
                _GUIUpdateTarget.GUIUpdate();
            }
        }

        private void rectangle1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(sender as System.Windows.IInputElement);
            _ValueDrag = true;
            _LastPos = e.GetPosition(this);

            if (_Signed)
            {
                _Value = (_LastPos.X - (this.ActualWidth * 0.5)) * _Range * 2 / this.ActualWidth;
            }
            else
            {
                _Value = _Range * _LastPos.X / this.ActualWidth;
            }

            ValueUpdated(true);
        }

        private void rectangle1_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_ValueDrag)
            {
                Point _NewPos = e.GetPosition(this);
                double delta = _NewPos.X - _LastPos.X;
                _Value += (_Signed ? 2 : 1) * _Range * delta / this.ActualWidth;
                _LastPos = _NewPos;
                ValueUpdated(true);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ValueUpdated(false);
        }
    }
}
