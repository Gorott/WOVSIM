using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WOVSIM.Persistence.EntityFramework;

namespace WOVSIM.Bot.Services;

public abstract class EntityService<TEntity, TKey> where TEntity : class
{
    protected readonly IDbContextFactory<WovsimDbContext> _contextFactory;

    protected EntityService(IDbContextFactory<WovsimDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    protected async Task<bool> UpdateAsync(
        Expression<Func<TEntity, bool>> predicate,
        Action<TEntity> updateAction,
        CancellationToken ct = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        await using var db = await _contextFactory.CreateDbContextAsync(ct);
        IQueryable<TEntity> q = db.Set<TEntity>();
        foreach (var inc in includes) q = q.Include(inc);

        var entity = await q.FirstOrDefaultAsync(predicate, ct);
        if (entity is null) return false;

        updateAction(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    
    protected async Task<TEntity> GetOrCreateAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<TEntity> createFactory,
        CancellationToken ct = default)
    {
        await using WovsimDbContext db = await _contextFactory.CreateDbContextAsync(ct);
        
        TEntity? entity = await db.Set<TEntity>().FirstOrDefaultAsync(predicate, ct);
        if (entity != null)
        {
            return entity;
        }
        
        TEntity newEntity = createFactory();
        db.Set<TEntity>().Add(newEntity);
        await db.SaveChangesAsync(ct);
        return newEntity;
    }
    
    protected async Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null,
        CancellationToken ct = default)
    {
        await using WovsimDbContext db = await _contextFactory.CreateDbContextAsync(ct);

        IQueryable<TEntity> query = db.Set<TEntity>();

        if (includes != null)
        {
            query = includes(query);
        }

        return await query.FirstOrDefaultAsync(predicate, ct);
    }

}