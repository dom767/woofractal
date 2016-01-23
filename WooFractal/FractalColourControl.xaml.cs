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
    /// Interaction logic for FractalColourControl.xaml
    /// </summary>
    public partial class FractalColourControl : UserControl, IGUIUpdateable
    {
        FractalColours _FractalColours;

        public FractalColourControl(FractalColours fractalColours)
        {
            InitializeComponent();

            _FractalColours = fractalColours;
            checkBox1.IsChecked = _FractalColours._XOrbitEnabled;
            orbitColourControl1.SetOrbitColours(_FractalColours._OrbitColoursX, this);
            checkBox2.IsChecked = _FractalColours._YOrbitEnabled;
            orbitColourControl2.SetOrbitColours(_FractalColours._OrbitColoursY, this);
            checkBox3.IsChecked = _FractalColours._ZOrbitEnabled;
            orbitColourControl3.SetOrbitColours(_FractalColours._OrbitColoursZ, this);
        }

        public void GUIUpdate()
        {
            _FractalColours._OrbitColoursX = orbitColourControl1.GetOrbitColours();
            _FractalColours._OrbitColoursY = orbitColourControl2.GetOrbitColours();
            _FractalColours._OrbitColoursZ = orbitColourControl3.GetOrbitColours();

            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }
    }
}
