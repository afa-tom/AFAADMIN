using System.Linq.Expressions;
using AFAADMIN.Common.Models;
using AFAADMIN.Database.Entities;
using SqlSugar;

namespace AFAADMIN.Database.Repositories;

/// <summary>
/// 泛型仓储接口
/// </summary>
public interface IBaseRepository<T> where T : BaseEntity, new()
{
    ISqlSugarClient DbContext { get; }

    Task<T?> GetByIdAsync(long id);
    Task<T?> GetFirstAsync(Expression<Func<T, bool>> predicate);
    Task<List<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null);
    Task<PageResult<T>> GetPageAsync(PageRequest page, Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null, bool isAsc = false);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task<long> InsertReturnIdAsync(T entity);
    Task<bool> InsertAsync(T entity);
    Task<bool> InsertRangeAsync(List<T> entities);
    Task<bool> UpdateAsync(T entity);
    Task<bool> UpdateColumnsAsync(T entity, Expression<Func<T, object>> columns);
    Task<bool> DeleteAsync(long id);
    Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate);
    Task<bool> SoftDeleteAsync(long id);
    Task<bool> SoftDeleteAsync(Expression<Func<T, bool>> predicate);
}

/// <summary>
/// 泛型仓储基类实现
/// </summary>
public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity, new()
{
    protected readonly ISqlSugarClient _db;

    public ISqlSugarClient DbContext => _db;

    public BaseRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 根据主键获取实体
    /// </summary>
    public async Task<T?> GetByIdAsync(long id)
    {
        return await _db.Queryable<T>().InSingleAsync(id);
    }

    /// <summary>
    /// 根据条件获取第一条
    /// </summary>
    public async Task<T?> GetFirstAsync(Expression<Func<T, bool>> predicate)
    {
        return await _db.Queryable<T>().FirstAsync(predicate);
    }

    /// <summary>
    /// 根据条件获取列表
    /// </summary>
    public async Task<List<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null)
    {
        var query = _db.Queryable<T>();
        if (predicate != null)
            query = query.Where(predicate);
        return await query.ToListAsync();
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    public async Task<PageResult<T>> GetPageAsync(PageRequest page,
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null,
        bool isAsc = false)
    {
        RefAsync<int> totalCount = 0;
        var query = _db.Queryable<T>();

        if (predicate != null)
            query = query.Where(predicate);

        if (orderBy != null)
            query = isAsc ? query.OrderBy(orderBy) : query.OrderBy(orderBy, OrderByType.Desc);
        else
            query = query.OrderBy(t => t.CreateTime, OrderByType.Desc);

        var items = await query.ToPageListAsync(page.PageIndex, page.PageSize, totalCount);

        return new PageResult<T>
        {
            PageIndex = page.PageIndex,
            PageSize = page.PageSize,
            TotalCount = totalCount,
            Items = items
        };
    }

    /// <summary>
    /// 是否存在
    /// </summary>
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _db.Queryable<T>().AnyAsync(predicate);
    }

    /// <summary>
    /// 计数
    /// </summary>
    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        var query = _db.Queryable<T>();
        if (predicate != null)
            query = query.Where(predicate);
        return await query.CountAsync();
    }

    /// <summary>
    /// 插入并返回主键
    /// </summary>
    public async Task<long> InsertReturnIdAsync(T entity)
    {
        return await _db.Insertable(entity).ExecuteReturnSnowflakeIdAsync();
    }

    /// <summary>
    /// 插入单条
    /// </summary>
    public async Task<bool> InsertAsync(T entity)
    {
        return await _db.Insertable(entity).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 批量插入
    /// </summary>
    public async Task<bool> InsertRangeAsync(List<T> entities)
    {
        return await _db.Insertable(entities).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 更新整个实体
    /// </summary>
    public async Task<bool> UpdateAsync(T entity)
    {
        return await _db.Updateable(entity)
            .IgnoreColumns(it => new { it.CreateTime, it.CreateBy })
            .ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 按列更新
    /// </summary>
    public async Task<bool> UpdateColumnsAsync(T entity, Expression<Func<T, object>> columns)
    {
        return await _db.Updateable(entity)
            .UpdateColumns(columns)
            .ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 物理删除
    /// </summary>
    public async Task<bool> DeleteAsync(long id)
    {
        return await _db.Deleteable<T>().In(id).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 按条件物理删除
    /// </summary>
    public async Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        return await _db.Deleteable<T>().Where(predicate).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 软删除（标记 IsDeleted = true）
    /// </summary>
    public async Task<bool> SoftDeleteAsync(long id)
    {
        return await _db.Updateable<T>()
            .SetColumns(it => it.IsDeleted == true)
            .SetColumns(it => it.UpdateTime == DateTime.Now)
            .Where(it => it.Id == id)
            .ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 按条件软删除
    /// </summary>
    public async Task<bool> SoftDeleteAsync(Expression<Func<T, bool>> predicate)
    {
        return await _db.Updateable<T>()
            .SetColumns(it => it.IsDeleted == true)
            .SetColumns(it => it.UpdateTime == DateTime.Now)
            .Where(predicate)
            .ExecuteCommandAsync() > 0;
    }
}
