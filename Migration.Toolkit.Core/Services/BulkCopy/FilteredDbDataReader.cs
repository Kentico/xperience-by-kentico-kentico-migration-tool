using System.Data.Common;

namespace Migration.Toolkit.Core.Services.BulkCopy;

public class FilteredDbDataReader: DataReaderProxyBase
{
    private readonly Func<DbDataReader, bool> _skipPredicate;

    public FilteredDbDataReader(DbDataReader innerReader, Func<DbDataReader, bool> skipPredicate) : base(innerReader)
    {
        _skipPredicate = skipPredicate;
    }

    public override bool Read()
    {
        while (true)
        {
            if (base.Read())
            {
                if (_skipPredicate(_innerReader))
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}