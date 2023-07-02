using System.Collections.Generic;

namespace GA
{
    public class Individual<T>
    {
        public IList<T> Genes;
        public float? Fitness { get; set; }

        public Individual(IEnumerable<T> genes)
        {
            Genes = new List<T>(genes);
        }
    }
}
