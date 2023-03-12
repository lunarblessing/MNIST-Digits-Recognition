using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworks
{
	public class Layer
	{
		Random Random { get; set; }
		IActivation Activation { get; set; }

		public int InputsCount { get; set; }
		public int OutputsCount { get; set; }
		public double[] Weights { get; set; }
		public double[] Biases { get; set; }
		public double[] LastOutputs { get; set; }
		public double[] Errors { get; set; }

		public Layer( int numNodesIn, int numNodesOut, Random rng, IActivation activation )
		{
			Random = rng;
			InputsCount = numNodesIn;
			OutputsCount = numNodesOut;
			Weights = new double[InputsCount * OutputsCount];
			Biases = new double[OutputsCount];
			LastOutputs = new double[OutputsCount];
			Errors = new double[OutputsCount];
			Activation = activation;
			InitializeWeights();
		}

		public virtual double[] Process( double[] inputs )
		{
			Parallel.For( 0, OutputsCount, ( i ) =>
			{
				double weightedInput = Biases[i];
				int startWeightIndex = i * InputsCount;
				for (int j = 0; j < InputsCount; j++)
				{
					weightedInput += Weights[startWeightIndex + j] * inputs[j];
				}
				LastOutputs[i] = Activation.Activate( weightedInput );
			} );
			return LastOutputs;
		}

		public int GetLastGuessNumber()
		{
			return Array.IndexOf( LastOutputs, LastOutputs.Max() );
		}

		public double GetWeight( int inputIndex, int outputIndex )
		{
			return Weights[outputIndex * InputsCount + inputIndex];
		}

		public double GetAverageError()
		{
			return Errors.Average();
		}

		public double CalculateTotalError( int correctLabel )
		{
			double sum = 0.0;
			for (int i = 0; i < OutputsCount; i++)
			{
				double expected = i == correctLabel ? 1.0 : 0.0;
				double error = (expected - LastOutputs[i]) * (expected - LastOutputs[i]);
				sum += error;
			}
			return 0.5 * sum;
		}

		public void CalculateOutputLayerDeltas( int correctLabel )
		{
			for (int i = 0; i < OutputsCount; i++)
			{
				double expected = i == correctLabel ? 1.0 : 0.0;
				Errors[i] = LastOutputs[i] * (1 - LastOutputs[i]) * (expected - LastOutputs[i]);
			}
		}

		public void CalculateHiddenLayerDeltas( Layer nextLayer )
		{
			Parallel.For( 0, OutputsCount, ( i ) =>
			{
				Errors[i] = 0.0;
				for (int j = 0; j < nextLayer.OutputsCount; j++)
				{
					Errors[i] += nextLayer.GetWeight( i, j ) * nextLayer.Errors[j];
				}
				Errors[i] *= LastOutputs[i] * (1 - LastOutputs[i]);
			} );
		}

		public void GradientWeightChange( double[] previousLayer, double rate )
		{
			Parallel.For( 0, OutputsCount, ( outputIndex ) =>
			{
				for (int inputIndex = 0; inputIndex < InputsCount; inputIndex++)
				{
					Weights[outputIndex * InputsCount + inputIndex] += rate * Errors[outputIndex]
						* previousLayer[inputIndex];
				}
				Biases[outputIndex] += rate * Errors[outputIndex];
			} );
		}

		void InitializeWeights()
		{
			for (int i = 0; i < Weights.Length; i++)
			{
				Weights[i] = Random.NextDouble() * 0.1 - 0.05;
			}
			for (int i = 0; i < Biases.Length; i++)
			{
				Biases[i] = Random.NextDouble() * 0.1 - 0.05;
			}
		}
	}
}
