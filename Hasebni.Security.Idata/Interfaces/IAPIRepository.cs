using Hasebni.Base;
using Hasebni.Model.Main;
using Hasebni.Model.Security;
using Hasebni.Security.Dto.User;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System.Threading.Tasks;

namespace Hasebni.Security.Idata.Interfaces
{
    public interface IAPIRepository
    {
        Task<OperationResult<UserInfoDtoResponse>> RegisterUser(RegisterUserDto userDto);

        Task<OperationResult<bool>> IsUserNameNotUsed(string username);
        Task<OperationResult<bool>> IsEmailNotUsed(string email);

        Task<OperationResult<LoginDtoResponse>> Login(LoginDto loginDto);

        Task<OperationResult<UpdateUserDto>> UpdateUser(UpdateUserDto updateUser);

        Task<OperationResult<bool>> DeleteUser(string email);

        Task<OperationResult<ProfileDto>> GetUsers();

        Task<OperationResult<bool>> ConfirmEmail(ConfirmEmailDto confirmEmailDto);
        Task<OperationResult<UserInfoDtoResponse>> GetUserInfoById(int id);
        Task<OperationResult<UserInfoDtoResponse>> GetUserInfoByEmail(string email);
        Task<OperationResult<string>> ChangePassword(ChangePasswordDto changePasswordDto);
        Task<OperationResult<bool>> ForgetPassword(string Email);
        
        Task<OperationResult<bool>> ResetPassword(ResetPasswordDto model);

        Task<OperationResult<ProfileDto>> SearchByUserName(string userName);


    }
}
