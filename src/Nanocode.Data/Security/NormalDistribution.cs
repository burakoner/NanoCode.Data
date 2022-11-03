using System;
using System.Text;

namespace Nanocode.Data.Security
{
    public class NormalDistribution
    {
        private readonly Random r = new Random();

        public double RandomNormal(double min, double max, int tightness)
        {
            var total = 0.0;
            for (var i = 1; i <= tightness; i++)
            {
                total += this.r.NextDouble();
            }
            return ((total / tightness) * (max - min)) + min;
        }

        public double RandomNormalDist(double min, double max, int tightness, double exp)
        {
            var total = 0.0;
            for (var i = 1; i <= tightness; i++)
            {
                total += Math.Pow(this.r.NextDouble(), exp);
            }

            return ((total / tightness) * (max - min)) + min;
        }

        public double RandomBiasedPow(double min, double max, int tightness, double peak)
        {
            // Calculate skewed normal distribution, skewed by Math.Pow(...), specifiying where in the range the peak is
            // NOTE: This peak will yield unreliable results in the top 20% and bottom 20% of the range.
            //       To peak at extreme ends of the range, consider using a different bias function

            var total = 0.0;
            var scaledPeak = peak / (max - min) + min;

            if (scaledPeak < 0.2 || scaledPeak > 0.8)
            {
                throw new Exception("Peak cannot be in bottom 20% or top 20% of range.");
            }

            var exp = this.GetExp(scaledPeak);

            for (var i = 1; i <= tightness; i++)
            {
                // Bias the random number to one side or another, but keep in the range of 0 - 1
                // The exp parameter controls how far to bias the peak from normal distribution
                total += this.BiasPow(this.r.NextDouble(), exp);
            }

            return ((total / tightness) * (max - min)) + min;
        }
        public double GetExp(double peak) =>
            // Get the exponent necessary for BiasPow(...) to result in the desired peak 
            // Based on empirical trials, and curve fit to a cubic equation, using WolframAlpha
            -12.7588 * Math.Pow(peak, 3) + 27.3205 * Math.Pow(peak, 2) - 21.2365 * peak + 6.31735;

        public double BiasPow(double input, double exp) => Math.Pow(input, exp);
    }
}
