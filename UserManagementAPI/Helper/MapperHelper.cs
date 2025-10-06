using Riok.Mapperly.Abstractions;
using UserManagementAPI.Data;
using UserManagementAPI.Models;

namespace UserManagementAPI.Helper;

[Mapper]
public static partial class MapperHelper
{
    public static partial IQueryable<User> AsUserDtoQueryable(this IQueryable<TUser> q);
    public static partial User ToUserDto(this TUser q);
    public static partial TUser FromUserDto(this User q);


}
