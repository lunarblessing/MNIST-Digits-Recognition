using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Environment;

namespace NeuralNetworks
{
	public class MNISTReader
	{
		private static string TrainImages = Path.Combine( GetFolderPath( SpecialFolder.Desktop ), "train-images.idx3-ubyte" );
		private static string TrainLabels = Path.Combine( GetFolderPath( SpecialFolder.Desktop ), "train-labels.idx1-ubyte" );
		private static string TestImages = Path.Combine( GetFolderPath( SpecialFolder.Desktop ), "t10k-images.idx3-ubyte" );
		private static string TestLabels = Path.Combine( GetFolderPath( SpecialFolder.Desktop ), "t10k-labels.idx1-ubyte" );

		public int ImageSize { get; private set; }

		public Image[] ReadTrainingData()
		{
			return Read( TrainImages, TrainLabels ).ToArray();
		}

		public Image[] ReadTestData()
		{
			return Read( TestImages, TestLabels ).ToArray();
		}

		private int ReadBigInt32( BinaryReader br )
		{
			var bytes = br.ReadBytes( sizeof( int ) );
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse( bytes );
			}
			return BitConverter.ToInt32( bytes, 0 );
		}

		private IEnumerable<Image> Read( string imagesPath, string labelsPath)
		{
			BinaryReader labels = new BinaryReader( new FileStream( labelsPath, FileMode.Open ) );
			BinaryReader images = new BinaryReader( new FileStream( imagesPath, FileMode.Open ) );

			int magicNumber = ReadBigInt32( images );
			int numberOfImages = ReadBigInt32( images );
			if(numberOfImages > 2000)
			{
				numberOfImages = 2000;
			}
			int width = ReadBigInt32( images );
			int height = ReadBigInt32( images );

			int magicLabel = ReadBigInt32( labels );
			int numberOfLabels = ReadBigInt32( labels );

			ImageSize = width;

			for (int i = 0; i < numberOfImages; i++)
			{
				var doubleArray =
					images.ReadBytes( width * height )
					.Select( byteValue => byteValue / (double)byte.MaxValue )
					.ToArray();
				yield return new Image( labels.ReadByte(), doubleArray );
			}
		}
	}

	public class Image
	{
		public int Label { get; set; }
		public double[] Data { get; set; }

		public Image( int label, double[] data )
		{
			Label = label;
			Data = data;
		}
	}
}
