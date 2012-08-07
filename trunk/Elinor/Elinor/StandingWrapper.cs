namespace Elinor
{
    internal class StandingWrapper
    {
        private readonly string _name;
        internal double Standing { get; private set; }

        internal StandingWrapper(string name, double standing)
        {
            _name = name;
            Standing = standing;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
