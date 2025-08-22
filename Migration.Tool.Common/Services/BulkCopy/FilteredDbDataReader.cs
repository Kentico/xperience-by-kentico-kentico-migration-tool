using System.Data;

namespace Migration.Tool.Common.Services.BulkCopy;

public class FilteredDbDataReader<TReader>(IDataReader innerReader, Func<IDataReader, bool> includePredicate) : DataReaderProxyBase(innerReader)
    where TReader : IDataReader
{
    public int TotalItems { get; private set; }
    public int TotalNonFiltered { get; private set; }

    public override bool Read()
    {
        while (true)
        {
            if (base.Read())
            {
                TotalItems++;
                if (!includePredicate(InnerReader))
                {
                    continue;
                }

                TotalNonFiltered++;
                return true;
            }

            return false;
        }
    }
}
