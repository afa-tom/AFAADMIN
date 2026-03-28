using AFAADMIN.Common.Cache;
using AFAADMIN.Common.DependencyInjection;
using AFAADMIN.Common.Exceptions;
using AFAADMIN.Database.Repositories;
using AFAADMIN.EventBus;
using AFAADMIN.System.Application.Dtos;
using AFAADMIN.System.Domain.Entities;
using AFAADMIN.System.Domain.Events;
using Mapster;
using SqlSugar;
using ICacheService = AFAADMIN.Common.Cache.ICacheService;

namespace AFAADMIN.System.Application.Services.Impl;

public class DictService : IDictService, IScopedDependency
{
    private readonly IBaseRepository<SysDictType> _typeRepo;
    private readonly IBaseRepository<SysDictData> _dataRepo;
    private readonly ISqlSugarClient _db;
    private readonly ICacheService _cache;
    private readonly IEventPublisher _eventPublisher;

    public DictService(IBaseRepository<SysDictType> typeRepo,
        IBaseRepository<SysDictData> dataRepo, ISqlSugarClient db,
        ICacheService cache, IEventPublisher eventPublisher)
    {
        _typeRepo = typeRepo;
        _dataRepo = dataRepo;
        _db = db;
        _cache = cache;
        _eventPublisher = eventPublisher;
    }

    public async Task<List<DictTypeDto>> GetTypeListAsync()
    {
        var list = await _typeRepo.GetListAsync();
        return list.OrderBy(t => t.CreateTime).Adapt<List<DictTypeDto>>();
    }

    public async Task<long> CreateTypeAsync(CreateDictTypeDto dto)
    {
        if (await _typeRepo.AnyAsync(t => t.DictCode == dto.DictCode))
            throw new BusinessException("字典编码已存在");

        var entity = dto.Adapt<SysDictType>();
        return await _typeRepo.InsertReturnIdAsync(entity);
    }

    public async Task<bool> UpdateTypeAsync(UpdateDictTypeDto dto)
    {
        var entity = await _typeRepo.GetByIdAsync(dto.Id);
        if (entity == null) throw new BusinessException("字典类型不存在");

        var oldCode = entity.DictCode;
        if (await _typeRepo.AnyAsync(t => t.DictCode == dto.DictCode && t.Id != dto.Id))
            throw new BusinessException("字典编码已存在");

        dto.Adapt(entity);
        var result = await _typeRepo.UpdateAsync(entity);

        // 清除缓存
        await _eventPublisher.PublishAsync(new DictDataChangedEvent { DictCode = oldCode });
        if (oldCode != dto.DictCode)
            await _eventPublisher.PublishAsync(new DictDataChangedEvent { DictCode = dto.DictCode });

        return result;
    }

    public async Task<bool> DeleteTypeAsync(long id)
    {
        var entity = await _typeRepo.GetByIdAsync(id);
        await _dataRepo.DeleteAsync(d => d.DictTypeId == id);
        var result = await _typeRepo.SoftDeleteAsync(id);

        if (entity != null)
            await _eventPublisher.PublishAsync(new DictDataChangedEvent { DictCode = entity.DictCode });

        return result;
    }

    public async Task<List<DictDataDto>> GetDataListByTypeIdAsync(long dictTypeId)
    {
        var list = await _dataRepo.GetListAsync(d => d.DictTypeId == dictTypeId);
        return list.OrderBy(d => d.Sort).Adapt<List<DictDataDto>>();
    }

    public async Task<List<DictDataDto>> GetDataListByCodeAsync(string dictCode)
    {
        // 带缓存
        var cached = await _cache.GetAsync<List<DictDataDto>>(CacheKeys.DictData(dictCode));
        if (cached != null) return cached;

        var items = await _db.Queryable<SysDictData>()
            .LeftJoin<SysDictType>((d, t) => d.DictTypeId == t.Id)
            .Where((d, t) => t.DictCode == dictCode && d.IsDeleted == false && t.IsDeleted == false)
            .OrderBy((d, t) => d.Sort)
            .Select((d, t) => d)
            .ToListAsync();

        var result = items.Adapt<List<DictDataDto>>();
        await _cache.SetAsync(CacheKeys.DictData(dictCode), result, TimeSpan.FromHours(2));
        return result;
    }

    public async Task<long> CreateDataAsync(CreateDictDataDto dto)
    {
        var entity = dto.Adapt<SysDictData>();
        var id = await _dataRepo.InsertReturnIdAsync(entity);

        // 清除关联字典类型的缓存
        var dictType = await _typeRepo.GetByIdAsync(dto.DictTypeId);
        if (dictType != null)
            await _eventPublisher.PublishAsync(new DictDataChangedEvent { DictCode = dictType.DictCode });

        return id;
    }

    public async Task<bool> UpdateDataAsync(UpdateDictDataDto dto)
    {
        var entity = await _dataRepo.GetByIdAsync(dto.Id);
        if (entity == null) throw new BusinessException("字典数据不存在");

        dto.Adapt(entity);
        var result = await _dataRepo.UpdateAsync(entity);

        var dictType = await _typeRepo.GetByIdAsync(dto.DictTypeId);
        if (dictType != null)
            await _eventPublisher.PublishAsync(new DictDataChangedEvent { DictCode = dictType.DictCode });

        return result;
    }

    public async Task<bool> DeleteDataAsync(long id)
    {
        var entity = await _dataRepo.GetByIdAsync(id);
        var result = await _dataRepo.SoftDeleteAsync(id);

        if (entity != null)
        {
            var dictType = await _typeRepo.GetByIdAsync(entity.DictTypeId);
            if (dictType != null)
                await _eventPublisher.PublishAsync(new DictDataChangedEvent { DictCode = dictType.DictCode });
        }

        return result;
    }
}
