using Items;
using System;

namespace Utils
{
    /// <summary>
    /// This is the representation of a quest in our actual game
    /// </summary>
    [Serializable]
    public class ConfiguredQuest
    {

        private Fraction.SimplifiedFraction _solution;
        private int _workingFraction;
		private int _questConfigurationID;
        private int _preferedFractionRepresentation;

        public Fraction.SimplifiedFraction solution { get { return _solution; } }
        public int workingFraction { get { return _workingFraction; } }
		public int questConfigurationID { get { return _questConfigurationID; } }
        public int PreferedFractionRepresentation { get { return _preferedFractionRepresentation; } }

		public ConfiguredQuest(int numerator, int denominator ,int workingFraction, int questConfID, int preferedFracIden)
        {
            this._solution = new Fraction.SimplifiedFraction(numerator, denominator, -1);
            this._workingFraction = workingFraction;
			this._questConfigurationID = questConfID;
            this._preferedFractionRepresentation = preferedFracIden;
        }

        public override string ToString()
        {
            return "Quest Solution: " + _solution.top + "/" + _solution.down + "\nWorking Fraction: "
                + _workingFraction + "\n";
        }
    }
}
