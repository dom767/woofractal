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
    /// <summary>
    /// Interaction logic for RenderControls.xaml
    /// </summary>
    public partial class RenderControls : UserControl, IGUIUpdateable
    {

        RenderOptions _Parent;
        public RenderControls(RenderOptions parent)
        {
            InitializeComponent();

            _Parent = parent;

            RenderSliders();
        }

        public void RenderSliders()
        {
            wooSlider1.Set(_Parent._DistanceMinimum, 0, 5, this);
            wooSlider2.Set(_Parent._DistanceIterations, 1, 1024, this);
            wooSlider3.Set(_Parent._StepSize, 0.01, 1.0, this);
            wooSlider4.Set(_Parent._DistanceExtents, 0.5, 4.0, this);
            wooSlider5.Set(_Parent._FractalIterationCount, 1, 250, this);
            wooSlider6.Set(_Parent._ColourIterationCount, 1, 250, this);
        }

        public void GUIUpdate()
        {
            _Parent._DistanceMinimum = wooSlider1.GetSliderValue();
            _Parent._DistanceIterations = wooSlider2.GetSliderValue();
            _Parent._StepSize = wooSlider3.GetSliderValue();
            _Parent._DistanceExtents = wooSlider4.GetSliderValue();
            _Parent._FractalIterationCount = (int)(wooSlider5.GetSliderValue() + 0.5);
            _Parent._ColourIterationCount = (int)(wooSlider6.GetSliderValue() + 0.5);

            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }
        
    }
}
