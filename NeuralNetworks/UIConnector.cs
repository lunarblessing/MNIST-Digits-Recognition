using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworks
{
	public class UIConnector : IDataAndUIConnector
	{

		const int RandomWeights = 4;

		NetworkControl Control { get; set; }
		Network Network { get; set; }
		(int inputIndex, int outputIndex)[] RandomConnections { get; set; }

		public UIConnector( NetworkControl control )
		{
			Control = control;
			control.SetConnector( this );
		}

		public void StartNetwork(
			int neuronsCount,
			Random random,
			double learningRate,
			double acceptableError,
			int iterations
			)
		{
			Network = new Network( neuronsCount, this, random );
			Network.IterationsEnded += OnNetworkIterationsEnded;
			RandomConnections = new (int, int)[RandomWeights];
			RandomConnections[0] = Network.GetRandomInputConnection();
			RandomConnections[1] = Network.GetRandomInputConnection();
			RandomConnections[2] = Network.GetRandomOutputConnection();
			RandomConnections[3] = Network.GetRandomOutputConnection();
			Task.Factory.StartNew( () =>
			{
				Network.Start( iterations, learningRate, acceptableError );
			} );
		}

		private void OnNetworkIterationsEnded( object? sender, EventArgs e )
		{
			Control.OnIterationsEnded();
		}

		public void WriteTrainingResults( int iteration, double percentOfCorrect, double averageError )
		{
			double[] weights = new double[RandomWeights];
			for (int i = 0; i < RandomWeights / 2; i++)
			{
				weights[i] = Network.GetWeightInMiddleLayer
					( RandomConnections[i].inputIndex, RandomConnections[i].outputIndex );
			}
			for (int i = RandomWeights / 2; i < RandomWeights; i++)
			{
				weights[i] = Network.GetWeightInOutputLayer
					( RandomConnections[i].inputIndex, RandomConnections[i].outputIndex );
			}
			Control.Dispatcher.BeginInvoke( new Action( () =>
				Control.WriteIterationResults( true, iteration, percentOfCorrect, averageError, weights ) ) );
		}

		public void WriteTestResults( int iteration, double percentOfCorrect, double averageError )
		{
			Control.Dispatcher.BeginInvoke( new Action( () =>
				Control.WriteIterationResults( false, iteration, percentOfCorrect, averageError ) ) );
		}

		public (Image image, double[] outputs, int guess)? GetRandomImageGuess( bool fromTrainingSet )
		{
			return Network.GetRandomGuess( fromTrainingSet, false );
		}

		public (Image image, double[] outputs, int guess)? GetRandomIncorrectImageGuess( bool fromTrainingSet )
		{
			return Network.GetRandomGuess( fromTrainingSet, true );
		}

		public double GetWeightInHiddenLayer( int inputIndex, int outputIndex )
		{
			return Network.GetWeightInMiddleLayer( inputIndex, outputIndex );
		}

		public double GetWeightInOutputLayer( int inputIndex, int outputIndex )
		{
			return Network.GetWeightInOutputLayer( inputIndex, outputIndex );
		}

		public (int inputIndex, int outputIndex) GetRandomHiddenConnection()
		{
			return Network.GetRandomInputConnection();
		}

		public (int inputIndex, int outputIndex) GetRandomOutputConnection()
		{
			return Network.GetRandomOutputConnection();
		}
	}
}
