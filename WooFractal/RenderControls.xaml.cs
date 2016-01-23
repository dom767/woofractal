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
            wooSlider1.Set(_Parent._DistanceMinimum, 5, false, this);
            wooSlider2.Set(_Parent._DistanceIterations, 1024, false, this);
            wooSlider3.Set(_Parent._StepSize, 1.0, false, this);
        }

        public void GUIUpdate()
        {
            _Parent._DistanceMinimum = wooSlider1.GetSliderValue();
            _Parent._DistanceIterations = wooSlider2.GetSliderValue();
            _Parent._StepSize = wooSlider3.GetSliderValue();

            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }
        
    }
}
