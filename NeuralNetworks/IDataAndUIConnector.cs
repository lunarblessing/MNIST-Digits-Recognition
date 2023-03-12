using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworks
{
	public interface IDataAndUIConnector
	{
		public void WriteTrainingResults( int iteration, double percentOfCorrect, double averageError );

		public void WriteTestResults( int iteration, double percentOfCorrect, double averageError );

		public (Image image, double[] outputs, int guess)? GetRandomImageGuess( bool fromTrainingSet );

		public (Image image, double[] outputs, int guess)? GetRandomIncorrectImageGuess( bool fromTrainingSet );

		public double GetWeightInHiddenLayer( int inputIndex, int outputIndex );

		public double GetWeightInOutputLayer( int inputIndex, int outputIndex );

		public (int inputIndex, int outputIndex) GetRandomHiddenConnection();

		public (int inputIndex, int outputIndex) GetRandomOutputConnection();
	}
}
