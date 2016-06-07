namespace Logitech.Maths
{
    public class AffineFunction
    {
        #region Initialization

        public AffineFunction()
        {
            Slope = YIntercept = 0;
        }

        public AffineFunction(double slope, double yIntercept)
        {
            Slope = slope;
            YIntercept = yIntercept;
        }

        public AffineFunction(double ax, double ay, double slope)
        {
            Slope = slope;
            YIntercept = ay - slope*ax;
        }

        public AffineFunction(double ax, double ay, double bx, double by)
        {
            Slope = (by - ay)/(bx - ax);
            YIntercept = ay - Slope*ax;
        }

        #endregion

        #region Properties

        public double Slope { get; set; }
        public double YIntercept { get; set; }

        public double XIntercept
        {
            get { return X(0); }
            set { YIntercept = -Slope*value; }
        }

        #endregion

        #region Methods

        public double Y(double x)
        {
            return Slope*x + YIntercept;
        }

        public double X(double y)
        {
            return DoubleEx.IsZero(Slope) ? double.NaN : (y - YIntercept)/Slope;
        }

        public void TranslateTo(double ax, double ay)
        {
            YIntercept = ay - Slope*ax;
        }

        #endregion
    }
}