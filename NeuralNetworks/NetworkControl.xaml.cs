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

	public partial class NetworkControl : UserControl
	{

		bool iterationRunning = true;
		IDataAndUIConnector connector;

		public NetworkControl()
		{
			InitializeComponent();
			iterationPanel.Children.Add( new IterationResultsControl() );
		}

		public void SetConnector( IDataAndUIConnector connector )
		{
			this.connector = connector;
		}

		public void WriteIterationResults( bool training, int iteration, double correctPercent, double averageError, double[]? weights = null )
		{
			iterationPanel.Children.Add( new IterationResultsControl( training, iteration, correctPercent, averageError, weights ) );
		}

		public void OnIterationsEnded()
		{
			iterationRunning = false;
		}

		void SetImage( (Image image, double[] outputs, int guess)? imageData )
		{
			if (imageData == null)
			{
				return;
			}
			var data = imageData.Value;
			outputsPanel.Children.Clear();
			for (int i = 0; i < data.outputs.Length; i++)
			{
				var block = new TextBlock();
				block.Text = $"{i}: {data.outputs[i]:f3}";
				block.Margin = new Thickness( 2 );
				outputsPanel.Children.Add( block );
			}
			labelText.Text = $"Правильный: {data.image.Label}";
			guessText.Text = $"Ответ: {data.guess}";
			if (data.image.Label == data.guess)
			{
				guessText.Foreground = Brushes.Green;
			}
			else
			{
				guessText.Foreground = Brushes.Red;
			}
			imageControl.Source = MakeBitmap( data.image.Data );
		}

		WriteableBitmap MakeBitmap( double[] data )
		{
			const int PixelsPerOriginal = 5;
			var bitmap = new WriteableBitmap(
				Network.ImageSize * PixelsPerOriginal,
				Network.ImageSize * PixelsPerOriginal,
				96,
				96,
				PixelFormats.Gray8,
				null );
			try
			{
				bitmap.Lock();
				for (int y = 0; y < Network.ImageSize; y++)
				{
					for (int x = 0; x < Network.ImageSize; x++)
					{
						Int32Rect area = new Int32Rect( x * PixelsPerOriginal, y * PixelsPerOriginal,
							PixelsPerOriginal, PixelsPerOriginal );
						byte[] pixels = new byte[PixelsPerOriginal * PixelsPerOriginal];
						Array.Fill( pixels, (byte)(data[y * Network.ImageSize + x] * byte.MaxValue) );
						bitmap.WritePixels( area, pixels, PixelsPerOriginal, 0 );
					}
				}
			}
			finally
			{
				bitmap.Unlock();
			}
			return bitmap;
		}

		private void RandomTrainingImageClick( object sender, RoutedEventArgs e )
		{
			if (iterationRunning)
			{
				return;
			}
			var imgData = connector.GetRandomImageGuess( true );
			SetImage( imgData );
		}

		private void RandomTestImageClick( object sender, RoutedEventArgs e )
		{
			if (iterationRunning)
			{
				return;
			}
			var imgData = connector.GetRandomImageGuess( false );
			SetImage( imgData );
		}

		private void WrongTrainingImageClick( object sender, RoutedEventArgs e )
		{
			if (iterationRunning)
			{
				return;
			}
			var imgData = connector.GetRandomIncorrectImageGuess( true );
			SetImage( imgData );
		}

		private void WrongTestImageClick( object sender, RoutedEventArgs e )
		{
			if (iterationRunning)
			{
				return;
			}
			var imgData = connector.GetRandomIncorrectImageGuess( false );
			SetImage( imgData );
		}
	}
}
