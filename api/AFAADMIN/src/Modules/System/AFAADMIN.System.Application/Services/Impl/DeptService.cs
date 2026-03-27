using AFAADMIN.Common.DependencyInjection;
using AFAADMIN.Common.Exceptions;
using AFAADMIN.Database.Repositories;
using AFAADMIN.System.Application.Dtos;
using AFAADMIN.System.Domain.Entities;
using Mapster;

namespace AFAADMIN.System.Application.Services.Impl;

public class DeptService : IDeptService, IScopedDependency
{
    private readonly IBaseRepository<SysDept> _deptRepo;

    public DeptService(IBaseRepository<SysDept> deptRepo)
    {
        _deptRepo = deptRepo;
    }

    public async Task<List<DeptDto>> GetTreeAsync()
    {
        var all = await _deptRepo.GetListAsync();
        var dtos = all.Adapt<List<DeptDto>>();
        return BuildTree(dtos, 0);
    }

    public async Task<DeptDto?> GetByIdAsync(long id)
    {
        var dept = await _deptRepo.GetByIdAsync(id);
        return dept?.Adapt<DeptDto>();
    }

    public async Task<long> CreateAsync(CreateDeptDto dto)
    {
        var dept = dto.Adapt<SysDept>();
        return await _deptRepo.InsertReturnIdAsync(dept);
    }

    public async Task<bool> UpdateAsync(UpdateDeptDto dto)
    {
        var dept = await _deptRepo.GetByIdAsync(dto.Id);
        if (dept == null) throw new BusinessException("部门不存在");

        if (dto.ParentId == dto.Id)
            throw new BusinessException("父级部门不能选择自身");

        dto.Adapt(dept);
        return await _deptRepo.UpdateAsync(dept);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        if (await _deptRepo.AnyAsync(d => d.ParentId == id))
            throw new BusinessException("请先删除子部门");
        return await _deptRepo.SoftDeleteAsync(id);
    }

    private static List<DeptDto> BuildTree(List<DeptDto> all, long parentId)
    {
        return all
            .Where(d => d.ParentId == parentId)
            .OrderBy(d => d.Sort)
            .Select(d =>
            {
                d.Children = BuildTree(all, d.Id);
                return d;
            })
            .ToList();
    }
}
