using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworks
{
	public interface IActivation
	{
		double Activate( double weightedInput );
	}

	public class StepFunction : IActivation
	{
		public double Activate( double weightedInput )
		{
			return weightedInput > 0 ? 1.0 : -1.0;
		}
	}

	public class SigmoidFunction : IActivation
	{
		public double Activate( double weightedInput )
		{
			return 1 / (1.0 + Math.Exp( -weightedInput ));
		}
	}

	public class LinearFunction : IActivation
	{
		public double Activate( double weightedInput )
		{
			return weightedInput;
		}
	}
}
