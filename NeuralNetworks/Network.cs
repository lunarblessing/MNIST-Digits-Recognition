using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NeuralNetworks
{
	public class Network
	{
		public static int ImageSize { get; private set; } = 28;
		public const int CategoriesCount = 10;

		public event EventHandler IterationsEnded;

		Layer[] Layers { get; set; }
		Random Random { get; set; }
		Image[] TrainingImages { get; set; }
		Image[] TestImages { get; set; }
		int ProcessedGuessesCount { get; set; }
		int CorrectGuessesCount { get; set; }
		IDataAndUIConnector UIConnector { get; set; }
		double LearningRate { get; set; }
		double AcceptableError { get; set; }
		Dictionary<Image, int> FinalIterationTraining { get; set; }
			= new Dictionary<Image, int>();
		Dictionary<Image, int> FinalIterationTest { get; set; }
			= new Dictionary<Image, int>();

		public Network(
			int middleLayerSize,
			IDataAndUIConnector uiConnector,
			Random random
			)
		{
			Random = random;
			UIConnector = uiConnector;
			SaveImages();
			InitLayers( middleLayerSize );
		}

		void InitLayers( int middleLayerSize )
		{
			if (middleLayerSize > 0)
			{
				Layers = new Layer[2];
				Layers[0] = new Layer( ImageSize * ImageSize, middleLayerSize, Random, new SigmoidFunction() );
				Layers[1] = new Layer( middleLayerSize, CategoriesCount, Random, new SigmoidFunction() );
			}
			else
			{
				Layers = new Layer[1];
				Layers[0] = new Layer( ImageSize * ImageSize, CategoriesCount, Random, new SigmoidFunction() );

			}
			return;
		}

		public void Start( int iterations, double learningRate, double acceptableError )
		{
			LearningRate = learningRate;
			AcceptableError = acceptableError;
			for (int iteration = 0; iteration < iterations; iteration++)
			{
				double trainingAverageError = 0.0;
				for (var i = 0; i < TrainingImages.Length; i++)
				{
					Image image = TrainingImages[i];
					double[] outputs = ProcessImage( image );
					if (iteration == iterations - 1)
					{
						var guess = Layers[^1].GetLastGuessNumber();
						FinalIterationTraining.Add( image, guess );
					}
					trainingAverageError += Math.Abs( Train( image ) );
				}
				trainingAverageError /= TrainingImages.Length;
				WriteStats( iteration, trainingAverageError, true );
				ResetStats();
				double testAverageError = 0.0;
				if (iteration == iterations - 1)
				{
					foreach (var image in TestImages)
					{
						var outputs = ProcessImage( image );
						testAverageError += Math.Abs( GetErrorOfLastProcess( image ) );
						if (iteration == iterations - 1)
						{
							var guess = Layers[^1].GetLastGuessNumber();
							FinalIterationTest.Add( image, guess );
						}
					}
					testAverageError /= TestImages.Length;
					WriteStats( iteration, testAverageError, false );
					ResetStats();
				}
			}
			IterationsEnded?.Invoke( this, EventArgs.Empty );
		}

		public Image GetTrainingImageByIndex( int index )
		{
			return TrainingImages[index];
		}

		public Image GetTestImageByIndex( int index )
		{
			return TestImages[index];
		}

		public double[] ProcessImage( Image image, bool saveStats = true )
		{
			Layers[0].Process( image.Data );
			for (int i = 1; i < Layers.Length; i++)
			{
				Layers[i].Process( Layers[i - 1].LastOutputs );
			}
			if (saveStats)
			{
				ProcessedGuessesCount++;
				int guess = Layers[^1].GetLastGuessNumber();
				if (guess == image.Label)
				{
					CorrectGuessesCount++;
				}
			}
			return Layers[^1].LastOutputs;
		}

		public double GetWeightInMiddleLayer( int inputIndex, int neuronIndex )
		{
			return Layers[0].GetWeight( inputIndex, neuronIndex );
		}

		public double GetWeightInOutputLayer( int inputIndex, int outputIndex )
		{
			return Layers[^1].GetWeight( inputIndex, outputIndex );
		}

		public (int inputIndex, int middleIndex) GetRandomInputConnection()
		{
			return (Random.Next( Layers[0].InputsCount ), Random.Next( Layers[0].OutputsCount ));
		}

		public (int middleIndex, int outputIndex) GetRandomOutputConnection()
		{
			return (Random.Next( Layers[^1].InputsCount ), Random.Next( Layers[^1].OutputsCount ));
		}

		public double[] GuessTestImage( Image image )
		{
			return ProcessImage( image, false );
		}

		public (Image image, double[] outputs, int guess)? GetRandomGuess( bool fromTraining, bool shouldBeIncorrect )
		{
			var dict = fromTraining ? FinalIterationTraining : FinalIterationTest;
			if (!shouldBeIncorrect)
			{
				int index = Random.Next( dict.Count );
				var kv = dict.ElementAt( index );
				var img = kv.Key;
				var outputs = ProcessImage( img, false );
				return (img, outputs, Layers[^1].GetLastGuessNumber());
			}
			else
			{
				var wrongGuesses = dict.Where( kv => kv.Value != kv.Key.Label );
				if (!wrongGuesses.Any())
				{
					return null;
				}
				wrongGuesses = wrongGuesses.OrderBy( kv => Random.Next() ).ToList();
				foreach (var kv in wrongGuesses)
				{
					var outputs = ProcessImage( kv.Key, false );
					if (Layers[^1].GetLastGuessNumber() == kv.Key.Label)
					{
						continue;
					}
					else
					{
						return (kv.Key, outputs, Layers[^1].GetLastGuessNumber());
					}
				}
				// All previously wrong guesses became correct
				return null;
			}
		}

		void SaveImages()
		{
			MNISTReader reader = new MNISTReader();
			TrainingImages = reader.ReadTrainingData();
			TestImages = reader.ReadTestData();
			ImageSize = reader.ImageSize;
		}

		double Train( Image image )
		{
			int result = Layers[^1].GetLastGuessNumber();
			var totalError = Layers[^1].CalculateTotalError( image.Label );
			if (totalError > AcceptableError)
			{
				Layers[^1].CalculateOutputLayerDeltas( image.Label );
				for (int i = Layers.Length - 2; i >= 0; i--)
				{
					Layers[i].CalculateHiddenLayerDeltas( Layers[i + 1] );
				}
				Layers[0].GradientWeightChange( image.Data, LearningRate );
				for (int i = 1; i < Layers.Length; i++)
				{
					Layers[i].GradientWeightChange( Layers[i - 1].LastOutputs, LearningRate );
				}
			}
			return totalError;
		}

		double GetErrorOfLastProcess( Image image )
		{
			return Layers[^1].CalculateTotalError( image.Label );
		}

		void WriteStats( int iteration, double avgError, bool isTraining )
		{
			if (isTraining)
			{
				UIConnector.WriteTrainingResults( iteration, (double)CorrectGuessesCount / ProcessedGuessesCount, avgError );
			}
			else
			{
				UIConnector.WriteTestResults( iteration, (double)CorrectGuessesCount / ProcessedGuessesCount, avgError );
			}

		}

		void ResetStats()
		{
			CorrectGuessesCount = 0;
			ProcessedGuessesCount = 0;
		}
	}
}
