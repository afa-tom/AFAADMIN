using AFAADMIN.System.Application.Dtos;
using FluentValidation;

namespace AFAADMIN.System.Application.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(64).WithMessage("用户名最长64个字符");
        RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(6).WithMessage("密码至少6个字符");
        RuleFor(x => x.Status).InclusiveBetween(0, 1).WithMessage("状态值无效");
    }
}

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("用户ID无效");
        RuleFor(x => x.Status).InclusiveBetween(0, 1).WithMessage("状态值无效");
    }
}

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("用户名不能为空");
        RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
    }
}

public class CreateRoleValidator : AbstractValidator<CreateRoleDto>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.RoleName).NotEmpty().WithMessage("角色名不能为空")
            .MaximumLength(64).WithMessage("角色名最长64个字符");
        RuleFor(x => x.RoleCode).NotEmpty().WithMessage("角色编码不能为空")
            .MaximumLength(64).WithMessage("角色编码最长64个字符");
    }
}

public class CreateMenuValidator : AbstractValidator<CreateMenuDto>
{
    public CreateMenuValidator()
    {
        RuleFor(x => x.MenuName).NotEmpty().WithMessage("菜单名称不能为空");
        RuleFor(x => x.MenuType).InclusiveBetween(1, 3).WithMessage("菜单类型无效（1=目录 2=菜单 3=按钮）");
    }
}

public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("用户ID无效");
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage("新密码不能为空")
            .MinimumLength(6).WithMessage("密码至少6个字符");
    }
}
