using System.Linq.Expressions;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common.MigrationProtocol;

namespace Migration.Tool.Common.Abstractions;

public abstract class EntityMapperBase<TSourceEntity, TTargetEntity>(ILogger logger, IPrimaryKeyMappingContext pkContext, IProtocol protocol) : IEntityMapper<TSourceEntity, TTargetEntity>
{
    protected readonly IProtocol Protocol = protocol;

    public virtual IModelMappingResult<TTargetEntity> Map(TSourceEntity? source, TTargetEntity? target)
    {
        var failures = new List<IModelMappingResult<TTargetEntity>>();
        var mappingHelper = new MappingHelper(pkContext, failures.Add);

        if (source is null)
        {
            logger.LogTrace("Source entity is not defined");
            return HandbookReferences.SourceEntityIsNull<TSourceEntity>().AsFailure<TTargetEntity>().Log(logger, Protocol);
        }

        bool newInstance = false;
        if (target is null)
        {
            logger.LogTrace("Null target supplied, creating new instance");
            target = CreateNewInstance(source, mappingHelper, failures.Add);
            if (target == null || Equals(target, default) || failures.Count > 0)
            {
                return new AggregatedResult<TTargetEntity>(failures).Log(logger, Protocol);
            }

            newInstance = true;
        }

        var mappedResult = MapInternal(source, target, newInstance, mappingHelper, failures.Add);

        return failures.Count > 0
            ? new AggregatedResult<TTargetEntity>(failures).Log(logger, Protocol)
            : new MapperResultSuccess<TTargetEntity>(mappedResult, newInstance).Log(logger, Protocol);
    }

    protected abstract TTargetEntity? CreateNewInstance(TSourceEntity source, MappingHelper mappingHelper, AddFailure addFailure);
    protected abstract TTargetEntity MapInternal(TSourceEntity source, TTargetEntity target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure);

    protected delegate void AddFailure(MapperResultFailure<TTargetEntity> failure);

    protected class MappingHelper(IPrimaryKeyMappingContext primaryKeyMappingContext, Action<IModelMappingResult<TTargetEntity>> addFailure)
    {
        public Guid Require(Guid? value, string valueName)
        {
            if (value is { } v && v != Guid.Empty)
            {
                return v;
            }

            var failure = HandbookReferences
                    .SourceValueIsRequired<TSourceEntity>(valueName)
                    .AsFailure<TTargetEntity>()
                ;

            addFailure(failure);
            return Guid.Empty;
        }

        public int Require(int? value, string valueName)
        {
            if (value is { } v)
            {
                return v;
            }

            var failure = HandbookReferences
                    .SourceValueIsRequired<TSourceEntity>(valueName)
                    .AsFailure<TTargetEntity>()
                ;

            addFailure(failure);
            return -1;
        }

        public bool Require(bool? value, string valueName)
        {
            if (value is { } v)
            {
                return v;
            }

            var failure = HandbookReferences
                    .SourceValueIsRequired<TSourceEntity>(valueName)
                    .AsFailure<TTargetEntity>()
                ;

            addFailure(failure);
            return false;
        }

        public DateTime Require(DateTime? value, string valueName)
        {
            if (value is { } v)
            {
                return v;
            }

            var failure = HandbookReferences
                    .SourceValueIsRequired<TSourceEntity>(valueName)
                    .AsFailure<TTargetEntity>()
                ;

            addFailure(failure);
            return DateTime.MinValue;
        }

        public bool TranslateIdAllowNulls<TKeyOwner>(Expression<Func<TKeyOwner, object>> keyNameSelector, int? sourceId, out int? translatedId)
        {
            if (sourceId == null)
            {
                translatedId = null;
                return true;
            }

            translatedId = primaryKeyMappingContext.MapFromSourceOrNull(keyNameSelector, sourceId);
            if (!translatedId.HasValue)
            {
                string memberName = keyNameSelector.GetMemberName();
                var failure = HandbookReferences
                        .MissingRequiredDependency<TKeyOwner>(memberName, sourceId)
                        .NeedsManualAction()
                        .AsFailure<TTargetEntity>()
                    ;

                addFailure(failure);
                return false;
            }

            return true;
        }

        public bool TryTranslateId<TKeyOwner>(Expression<Func<TKeyOwner, object>> keyNameSelector, int? sourceId, out int? translatedId)
        {
            translatedId = primaryKeyMappingContext.MapFromSourceOrNull(keyNameSelector, sourceId);
            if (sourceId.HasValue && !translatedId.HasValue)
            {
                return false;
            }

            return true;
        }

        public bool TranslateRequiredId<TKeyOwner>(Expression<Func<TKeyOwner, object>> keyNameSelector, int? sourceId, out int translatedId)
        {
            if (primaryKeyMappingContext.TryRequireMapFromSource(keyNameSelector, sourceId, out translatedId))
            {
                return true;
            }

            string memberName = keyNameSelector.GetMemberName();
            var failure = HandbookReferences
                    .MissingRequiredDependency<TKeyOwner>(memberName, sourceId)
                    .NeedsManualAction()
                    .AsFailure<TTargetEntity>()
                ;
            addFailure(failure);
            return false;
        }
    }
}
