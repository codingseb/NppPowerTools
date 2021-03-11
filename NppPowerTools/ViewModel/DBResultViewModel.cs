using System.Collections;
using System.Collections.Generic;

namespace NppPowerTools
{
    public class DBResultViewModel: IEnumerable<object>
    {
        public List<string> ColumnsNames { get; set; }

        public List<dynamic> Results { get; set; }

        public dynamic this[int index]
        {
            get => Results[index];
            set => Results[index] = value;
        }

        public IEnumerator<dynamic> GetEnumerator()
        {
            return Results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Results.GetEnumerator();
        }
    }
}
