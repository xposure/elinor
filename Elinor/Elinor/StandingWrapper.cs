namespace Elinor
{
    internal class StandingWrapper
    {
        private readonly string _name;

        internal StandingWrapper(string name, double standing)
        {
            _name = name;
            Standing = standing;
        }

        internal double Standing { get; private set; }

        public override string ToString()
        {
            return _name;
        }
    }
}