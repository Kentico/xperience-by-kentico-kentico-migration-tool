namespace Migration.Toolkit.Common.Services.BulkCopy;

using System.Data;

public class FilteredDbDataReader<TReader>(IDataReader innerReader, Func<IDataReader, bool> includePredicate) : DataReaderProxyBase(innerReader)
    where TReader : IDataReader
{
    public int TotalItems { get; private set; } = 0;
    public int TotalNonFiltered { get; private set; } = 0;

    public override bool Read()
    {
        while (true)
        {
            if (base.Read())
            {
                TotalItems++;
                if (!includePredicate(_innerReader))
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