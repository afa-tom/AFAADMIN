using AFAADMIN.Common.DependencyInjection;
using AFAADMIN.Common.Exceptions;
using AFAADMIN.Database.Repositories;
using AFAADMIN.System.Application.Dtos;
using AFAADMIN.System.Domain.Entities;
using Mapster;
using SqlSugar;

namespace AFAADMIN.System.Application.Services.Impl;

public class DictService : IDictService, IScopedDependency
{
    private readonly IBaseRepository<SysDictType> _typeRepo;
    private readonly IBaseRepository<SysDictData> _dataRepo;
    private readonly ISqlSugarClient _db;

    public DictService(IBaseRepository<SysDictType> typeRepo,
        IBaseRepository<SysDictData> dataRepo, ISqlSugarClient db)
    {
        _typeRepo = typeRepo;
        _dataRepo = dataRepo;
        _db = db;
    }

    // ===== 字典类型 =====

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

        if (await _typeRepo.AnyAsync(t => t.DictCode == dto.DictCode && t.Id != dto.Id))
            throw new BusinessException("字典编码已存在");

        dto.Adapt(entity);
        return await _typeRepo.UpdateAsync(entity);
    }

    public async Task<bool> DeleteTypeAsync(long id)
    {
        // 级联删除字典数据
        await _dataRepo.DeleteAsync(d => d.DictTypeId == id);
        return await _typeRepo.SoftDeleteAsync(id);
    }

    // ===== 字典数据 =====

    public async Task<List<DictDataDto>> GetDataListByTypeIdAsync(long dictTypeId)
    {
        var list = await _dataRepo.GetListAsync(d => d.DictTypeId == dictTypeId);
        return list.OrderBy(d => d.Sort).Adapt<List<DictDataDto>>();
    }

    public async Task<List<DictDataDto>> GetDataListByCodeAsync(string dictCode)
    {
        var items = await _db.Queryable<SysDictData>()
            .LeftJoin<SysDictType>((d, t) => d.DictTypeId == t.Id)
            .Where((d, t) => t.DictCode == dictCode && d.IsDeleted == false && t.IsDeleted == false)
            .OrderBy((d, t) => d.Sort)
            .Select((d, t) => d)
            .ToListAsync();

        return items.Adapt<List<DictDataDto>>();
    }

    public async Task<long> CreateDataAsync(CreateDictDataDto dto)
    {
        var entity = dto.Adapt<SysDictData>();
        return await _dataRepo.InsertReturnIdAsync(entity);
    }

    public async Task<bool> UpdateDataAsync(UpdateDictDataDto dto)
    {
        var entity = await _dataRepo.GetByIdAsync(dto.Id);
        if (entity == null) throw new BusinessException("字典数据不存在");

        dto.Adapt(entity);
        return await _dataRepo.UpdateAsync(entity);
    }

    public async Task<bool> DeleteDataAsync(long id)
    {
        return await _dataRepo.SoftDeleteAsync(id);
    }
}
