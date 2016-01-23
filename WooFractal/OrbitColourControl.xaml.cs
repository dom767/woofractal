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
    /// Interaction logic for OrbitColourControl.xaml
    /// </summary>
    public partial class OrbitColourControl : UserControl, IGUIUpdateable
    {
        OrbitColours _OrbitColours;
        IGUIUpdateable _GUIUpdateable;

        public OrbitColourControl()
        {
            InitializeComponent();
        }

        public void SetOrbitColours(OrbitColours orbitColours, IGUIUpdateable guiUpdateable)
        {
            _OrbitColours = orbitColours;
            _GUIUpdateable = guiUpdateable;

            SetupGUI();
        }

        public OrbitColours GetOrbitColours()
        {
            return _OrbitColours;
        }

        public void SetupGUI()
        {
            materialControl1.SetMaterial(_OrbitColours._StartColour, this);
            materialControl2.SetMaterial(_OrbitColours._EndColour, this);
            wooSlider1.Set(_OrbitColours._Multiplier, 10, false, this);
        }

        public void GUIUpdate()
        {
            _OrbitColours._StartColour = materialControl1.GetMaterial();
            _OrbitColours._EndColour = materialControl2.GetMaterial();
            _OrbitColours._Multiplier = wooSlider1.GetSliderValue();
            _GUIUpdateable.GUIUpdate();
        }
    }
}
