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

	public partial class Menu : UserControl
	{
		public Menu()
		{
			InitializeComponent();
		}

		private void OnStartButtonClick( object sender, RoutedEventArgs e )
		{
			int neuronsCount;
			if (!int.TryParse( NeuronsCountTB.Text, out neuronsCount ))
			{
				neuronsCount = 0;
			}
			double learningRate;
			if (!double.TryParse( LearningRateTB.Text, out learningRate ))
			{
				learningRate = 0.15;
			}
			int randomSeed;
			if (!int.TryParse( RandomSeedTB.Text, out randomSeed ))
			{
				randomSeed = (int)DateTime.Now.Ticks;
			}
			double error;
			if (!double.TryParse( errorTB.Text, out error ))
			{
				error = 0.01;
			}
			int iterations;
			if (!int.TryParse( iterationsTB.Text, out iterations ))
			{
				iterations = 10;
			}
			Random random = new Random( randomSeed );
			NetworkControl control = new NetworkControl();
			UIConnector connector = new UIConnector( control );
			connector.StartNetwork(
				neuronsCount: neuronsCount,
				random: random,
				learningRate: learningRate,
				acceptableError: error,
				iterations: iterations
				);
			Content = control;
		}
	}
}
