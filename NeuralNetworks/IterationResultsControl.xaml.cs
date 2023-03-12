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

namespace NeuralNetworks
{

	public partial class IterationResultsControl : UserControl
	{
		public IterationResultsControl()
		{
			InitializeComponent();
			iterationText.Text = "# итерации";
			correctText.Text = "% верных";
			errorText.Text = "Средняя ошибка";
			w1Text.Text = "вес 1";
			w2Text.Text = "вес 2";
			w3Text.Text = "вес 3";
			w4Text.Text = "вес 4";
		}

		public IterationResultsControl( bool isTraining, int iteration, double correct, double error, double[]? weights )
		{
			InitializeComponent();
			iterationText.Text = isTraining ? iteration.ToString() : "Тест";
			correctText.Text = correct.ToString( "P1" );
			errorText.Text = error.ToString( "f3" );
			if (weights == null || weights.Length < 4)
			{
				return;
			}
			w1Text.Text = weights?[0].ToString( "f3" );
			w2Text.Text = weights?[1].ToString( "f3" );
			w3Text.Text = weights?[2].ToString( "f3" );
			w4Text.Text = weights?[3].ToString( "f3" );
		}
	}
}
