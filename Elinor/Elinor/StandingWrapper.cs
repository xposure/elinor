namespace Elinor
{
    class StandingWrapper
    {
        public string Name;
        public double Standing;

        public StandingWrapper(string name, double standing)
        {
            Name = name;
            Standing = standing;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
