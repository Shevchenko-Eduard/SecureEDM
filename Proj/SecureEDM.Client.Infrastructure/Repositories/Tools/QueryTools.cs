using System.Linq.Expressions;
using SecureEDM.Common.Domain.Models.ValueObject;

namespace SecureEDM.Client.Infrastructure.Repositories.Tools;

public static class QueryTools
{
    public static void ExecuteIfNotNull<TValue, TEntity>(
        ref IQueryable<TEntity> query, TValue? value, 
        Func<TValue, IQueryable<TEntity>> action)
        where TValue : class
    {
        if (value is TValue valueNotNull) query = action(valueNotNull);
    }
    public static void ExecuteIfNotNull<TValue, TEntity>(
        ref IQueryable<TEntity> query, TValue? value, 
        Func<TValue, IQueryable<TEntity>> action)
        where TValue : struct
    {
        if (value is TValue valueNotNull) query = action(valueNotNull);
    }
    public static void ExecuteIfNotNull<TValue, TEntity>(
        ref IQueryable<TEntity> query, Range<TValue>? value,
        Func<TValue, IQueryable<TEntity>> actionTo,
        Func<TValue, IQueryable<TEntity>> actionFrom)
        where TValue : struct, IComparable<TValue>
    {
        if (value is Range<TValue> valueNotNull)
        {
            if (valueNotNull.To is TValue to)
            {
                query = actionTo(to);
            }
            if (valueNotNull.From is TValue from)
            {
                query = actionFrom(from);
            }
        }
    }
    public static void ExecuteWhereIfNotNull<TValue, TEntity>(
        ref IQueryable<TEntity> query, Range<TValue>? value,
        Func<TValue, Expression<Func<TEntity, bool>>> actionTo,
        Func<TValue, Expression<Func<TEntity, bool>>> actionFrom)
        where TValue : struct, IComparable<TValue>
    {
        if (value is Range<TValue> valueNotNull)
        {
            if (valueNotNull.To is TValue to)
            {
                query = query.Where(actionTo(to));
            }
            if (valueNotNull.From is TValue from)
            {
                query = query.Where(actionFrom(from));
            }
        }
    }
        public static void ExecuteWhereIfNotNull<TValue, TEntity>(
        ref IQueryable<TEntity> query, TValue? value, 
        Func<TValue, Expression<Func<TEntity, bool>>> action)
        where TValue : class
    {
        if (value is TValue valueNotNull) query = query.Where(action(valueNotNull));
    }
    public static void ExecuteWhereIfNotNull<TValue, TEntity>(
        ref IQueryable<TEntity> query, TValue? value, 
        Func<TValue, Expression<Func<TEntity, bool>>> action)
        where TValue : struct
    {
        if (value is TValue valueNotNull) query = query.Where(action(valueNotNull));
    }
}