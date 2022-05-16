using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Core.Configuration;

namespace Migration.Toolkit.Core.Abstractions;

public abstract class SynchronizerBase<TSource, TTarget, TKey> where TTarget : class where TSource : class //: ISynchronizer<TSource,TTarget>
{
    protected readonly ILogger Logger;
    private readonly IEntityMapper<TSource, TTarget> _mapper;
    // private readonly IDataEqualityComparer<TSource, TTarget> _comparer;
    private readonly EntityConfigurations _entityConfigurations;

    public SynchronizerBase(ILogger logger, IEntityMapper<TSource, TTarget> mapper,// IDataEqualityComparer<TSource, TTarget> comparer,
        EntityConfigurations entityConfigurations)
    {
        Logger = logger;
        _mapper = mapper;
        // _comparer = comparer;
        _entityConfigurations = entityConfigurations;
    }

    // TODO tk: 2022-05-08 filter by SiteId

    protected abstract IEnumerable<TSource> GetSourceEnumerable();
    protected abstract IEnumerable<TTarget> GetTargetEnumerable();
    protected abstract IEnumerable<TKey> GetAllKeysEnumerable();
    // protected abstract bool CheckKey(TKey key, TSource source);
    // protected abstract bool CheckKey(TKey key, TTarget target);

    protected abstract TKey? SelectKey(TSource? source);
    protected abstract TKey? SelectKey(TTarget? target);

    // TODO tk: 2022-05-08 ensure storage of new PK to context for PK rewrite & consult with dependancy tree
    protected abstract void Insert(TTarget item);
    protected abstract void Update(TTarget item);
    protected abstract void Delete(TTarget item);

    protected abstract string Print(TSource item);
    protected abstract string Print(TTarget item);

    public async Task StartAsync()
    {
        var sourceEnumerable = GetSourceEnumerable();
        var targetEnumerable = GetTargetEnumerable();
        var allKeys = GetAllKeysEnumerable();

        var alignerNg = EnumerableHelper.CreateAligner(
            sourceEnumerable.GetEnumerator(),
            targetEnumerable.GetEnumerator(),
            allKeys.GetEnumerator(),
            SelectKey,
            SelectKey,
            true
        );
        
        var tableName = ReflectionHelper<TTarget>.GetFirstAttributeOrNull<TableAttribute>()?.Name;
        var configuration = _entityConfigurations.GetEntityConfiguration(tableName);

        // TODO tk: 2022-05-06 create buffer for bulk updates / inserts

        foreach (var result in alignerNg)
        {
            switch (result)
            {
                case SimpleAlignFatalNoMatch<TSource, TTarget, TKey>(_, _, var key, var errorDescription):
                    // skip
                    Logger.LogError("Failed to match {key} with error {errorDescription}", key, errorDescription);
                    break;
                case SimpleAlignResultMatch<TSource, TTarget, TKey>(var source, var target, var key):
                {
                    // update
                    var mapped = MapTargetFromSource(source, target);
                    if (mapped != null)
                    {
                        Update(mapped);
                        Logger.LogInformation("Updating {item}", mapped);
                    }
                    else
                    {
                        // TODO tk: 2022-05-08 log                        
                    }

                    break;
                }
                case SimpleAlignResultOnlyA<TSource, TTarget, TKey>(var source, var key):
                {
                    // insert
                    var mapped = MapTargetFromSource(source, null);
                    if (mapped != null)
                    {
                        Insert(mapped); // update
                        Logger.LogInformation("Inserting {item}", mapped);
                    }
                    else
                    {
                        // TODO tk: 2022-05-08 log
                    }

                    break;
                }
                case SimpleAlignResultOnlyB<TSource, TTarget, TKey>(var target, var key):
                    if (configuration.DeleteTargetIfSourceNotExists)
                    {
                        Delete(target);
                        Logger.LogInformation("Deleting {item}", target);
                    }
                    else
                    {
                        Logger.LogInformation("Deleting of {item} is DISABLED", target);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result));
            }
        }

        // using var aligner = EnumeratorAlignerNg<TSource, TTarget, TKey, ModelMappingResult<TTarget>>
        //     .Create(
        //         sourceEnumerable.GetEnumerator(),
        //         targetEnumerable.GetEnumerator(),
        //         allKeys.GetEnumerator(),
        //         (a, b) => _mapper.Map(a, b),
        //         _comparer,
        //         CheckKey,
        //         CheckKey,
        //         SelectKey,
        //         SelectKey,
        //         true
        //     );
        //
        //
        //
        // while (aligner.MoveNext())
        // {
        //     var current = aligner.Current;
        //
        //     switch (current)
        //     {
        //         case AlignResultMatchSame<TSource, TTarget, TKey, ModelMappingResult<TTarget>?>(_, var b, _, _):
        //             Logger.LogTrace("Updating NOT {item} (item data is equal)", b);
        //             break;
        //         case AlignResultOnlyA<TSource, TTarget, TKey, ModelMappingResult<TTarget>?>(var a, var key, var result)
        //             when result! is { Success: true }:
        //             // insert
        //             Insert(result.Item);
        //             Logger.LogInformation("Inserting {item}", result.Item);
        //             break;
        //         case AlignResultOnlyB<TSource, TTarget, TKey, ModelMappingResult<TTarget>?>(var target, _, _):
        //             if (configuration.DeleteTargetIfSourceNotExists)
        //             {
        //                 Delete(target);
        //                 Logger.LogInformation("Deleting {item}", target);
        //             }
        //             else
        //             {
        //                 Logger.LogInformation("Not deleting {item}", target);
        //             }
        //
        //             break;
        //         case AlignResultMatchMapped<TSource, TTarget, TKey, ModelMappingResult<TTarget>?>(var cmsSite, _, _, var result)
        //             when result! is { Success: true }:
        //             switch (result)
        //             {
        //                 case null:
        //                     // skip
        //                     Logger.LogWarning("Result of mapping was not created");
        //                     break;
        //                 case ModelMappingFailed<TTarget>(var message):
        //                     // skip
        //                     Logger.LogWarning(message);
        //                     break;
        //                 case ModelMappingFailedKeyMismatch<TTarget>(_, _, var message, _):
        //                     // skip
        //                     Logger.LogWarning(message);
        //                     break;
        //                 case ModelMappingFailedSourceNotDefined<TTarget>(_, _, var message, _):
        //                     // skip
        //                     Logger.LogWarning(message);
        //                     break;
        //                 case ModelMappingSuccess<TTarget>(var mappingResult, var newInstance):
        //                     if (newInstance)
        //                     {
        //                         // insert
        //                         Insert(result.Item!);
        //                         Logger.LogInformation("Inserting {item}", cmsSite);
        //                     }
        //                     else
        //                     {
        //                         // update
        //                         Update(mappingResult!);
        //                         Logger.LogInformation("Updating {item}", cmsSite);
        //                     }
        //
        //                     break;
        //                 default:
        //                     throw new ArgumentOutOfRangeException(nameof(result));
        //             }
        //
        //             break;
        //         case AlignFatalNoMatch<TSource, TTarget, TKey, ModelMappingResult<TTarget>?>(_, _, var key, var errorDescription):
        //             // skip
        //             Logger.LogError("Failed to match {key} with error {errorDescription}", key, errorDescription);
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException(nameof(current));
        //     }
        // }
    }

    private TTarget? MapTargetFromSource(TSource source, TTarget? target)
    {
        var mappingResult = _mapper.Map(source, target);
        switch (mappingResult)
        {
            // TODO tk: 2022-05-08 more info for error message (logger scope?)
            case ModelMappingFailed<TTarget>(var message):
                Logger.LogError(message);
                break;
            case ModelMappingFailedKeyMismatch<TTarget>(_, _, var message, _):
                Logger.LogError(message);
                break;
            case ModelMappingFailedSourceNotDefined<TTarget>(_, _, var message, _):
                Logger.LogError(message);
                break;
            case ModelMappingSuccess<TTarget>(var mappingResultItem, _):
                return mappingResultItem;
            default:
                throw new ArgumentOutOfRangeException(nameof(mappingResult));
        }

        return default;
    }
}