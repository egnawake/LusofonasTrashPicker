using System.Collections.Generic;
using System.Linq;

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

        public override string ToString()
        {
            return string.Join(',', Genes);
        }
    }
}
