using Hasebni.Base.ApiValidation;
using Hasebni.Model.Security;
using Hasebni.Security.Dto.User;
using Hasebni.Security.Idata.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Hasebni.Base;
using Hasebni.SqlServer.DataBase;
using Hasebni.Model.Main;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Globalization;
using Hasebni.SharedKernal.ExtensionMethod;
using MailKit.Net.Smtp;
using MimeKit;
using Hasebni.Model.Setting;
using Microsoft.AspNetCore.WebUtilities;

namespace Hasebni.Security.Data.Repositories
{
    public class APIRepository : HasebniRepository, IAPIRepository
    {
        #region props and ctor

        private readonly UserManager<HUser> userManager;
        private readonly SignInManager<HUser> signInManager;
        private readonly IConfiguration configuration;
        //private readonly IEmailSender emailSender;
        private readonly IPasswordHasher<HUser> passwordHasher;
        

        public APIRepository(HasebniDbContext context,
            UserManager<HUser> userManager,
            SignInManager<HUser> signInManager,
            IConfiguration configuration,
           // IEmailSender emailSender,
            IPasswordHasher<HUser> passwordHasher ) : base(context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
           // this.emailSender = emailSender;
            this.passwordHasher = passwordHasher;
        }
        
        #endregion

        #region Methods

        public async Task<OperationResult<UserInfoDtoResponse>> RegisterUser(RegisterUserDto userDto)
        {
            OperationResult<UserInfoDtoResponse> operation = new OperationResult<UserInfoDtoResponse>();
            using (var Transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    HUser user = new HUser
                    {
                        UserName = userDto.UserName,
                        Email = userDto.Email
                    };
                    var result = await userManager.CreateAsync(user, userDto.Password);
                    if (result == IdentityResult.Success)
                    {
                        await Context.SaveChangesAsync();

                        //convert datatime
                        DateTime date = userDto.BirthDate.FixFormatDate();

                        //token to confirm email
                        var token = ExtensionMethods.GetSixNumberToken();
                        Profile profile = new Profile
                        {
                            Avatar = userDto.Avatar,
                            BirthDate = date,
                            FirstName = userDto.FirstName,
                            Gender = userDto.Gender,
                            LastName = userDto.LastName,
                            HUserFk = user.Id,
                            Token = token
                            //Members = Context.Members.SelectMany()

                        };
                        Context.Profiles.Add(profile);

                        if (await SendToken(userDto, token))
                        {
                            await Context.SaveChangesAsync();
                            Transaction.Commit();
                            operation.OperationResultType = OperationResultTypes.Success;
                        }
                        else
                        {
                            operation.OperationResultType = OperationResultTypes.Failed;
                            operation.Result = null;
                            Transaction.Rollback();
                        }

                        operation.Result = new UserInfoDtoResponse
                        {
                            Avatar = userDto.Avatar,
                            BirthDate = userDto.BirthDate,
                            Email = userDto.Email,
                            FirstName = userDto.FirstName,
                            Gender = userDto.Gender,
                            LastName = userDto.LastName,
                            UserId = profile.Id,
                            UserName = userDto.UserName
                        };
                    }
                    else
                    {
                        operation.OperationResultType = OperationResultTypes.Failed;
                        operation.Result = null;
                        Transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    operation.Exception = ex;
                    operation.OperationResultType = OperationResultTypes.Exception;
                    Transaction.Rollback();
                }
            }
            return operation;
        }

        public async Task<OperationResult<bool>> IsEmailNotUsed(string email)
        {
            OperationResult<bool> operation = new OperationResult<bool>();
            try
            {
                var result = await userManager.FindByEmailAsync(email);

                if (result == null)
                {
                    operation.Result = true;
                    operation.OperationResultType = OperationResultTypes.Success;
                }
                else
                {
                    operation.Result = false;
                    operation.OperationResultType = OperationResultTypes.Exist;
                }


            }
            catch (Exception ex)
            {
                operation.Exception = ex;
                operation.OperationResultType = OperationResultTypes.Exception;
            }

            return operation;
        }

        public async Task<OperationResult<bool>> IsUserNameNotUsed(string username)
        {
            OperationResult<bool> operation = new OperationResult<bool>();
            try
            {
                var result = await userManager.FindByNameAsync(username);

                if (result == null)
                {
                    operation.Result = true;
                    operation.OperationResultType = OperationResultTypes.Success;
                }
                else
                {
                    operation.Result = false;
                    operation.OperationResultType = OperationResultTypes.Exist;
                }

            }
            catch (Exception ex)
            {
                operation.Exception = ex;
                operation.OperationResultType = OperationResultTypes.Exception;
            }

            return operation;
        }

        public async Task<OperationResult<LoginDtoResponse>> Login(LoginDto loginDto)
        {
            OperationResult<LoginDtoResponse> operation = new OperationResult<LoginDtoResponse>();
            try
            {
                var user = await userManager.FindByEmailAsync(loginDto.Email);

                if (user is null)
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                    operation.Result = null;
                }
                else
                {
                    var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                 //   await signInManager.Get
                    if (result == SignInResult.Success)
                    {
                        operation.OperationResultType = OperationResultTypes.Success;
                        var data = await GetUserInfoByEmail(loginDto.Email);
                        var dataonly = data.Result;
                        operation.Result = new LoginDtoResponse
                        {
                            UserData = new UserInfoDtoResponse
                            {
                                Avatar = dataonly.Avatar,
                                BirthDate = dataonly.BirthDate,
                                Email = dataonly.Email,
                                FirstName = dataonly.FirstName,
                                Gender = dataonly.Gender,
                                LastName = dataonly.LastName,
                                UserId = dataonly.UserId,
                                UserName = dataonly.UserName,
                            },
                            Token = GenerateJwtToken(dataonly.Email)
                        };
                        var deviceToken = await Context.Devices
                            .SingleOrDefaultAsync(device => device.ProfileId == user.Id);
                        if (deviceToken is null)
                        {
                            var profile = Context.Profiles
                                .Where(p => (!p.DateDeleted.HasValue) && (p.HUserFk == user.Id))
                                .SingleOrDefault();
                            Device device = new Device()
                            {
                                ProfileId = profile.Id,
                                DeviceToken = loginDto.DeviceToken
                            };
                            Context.Devices.Add(device);

                            await Context.SaveChangesAsync();
                        }
                        else
                        {
                            if (deviceToken.DeviceToken != loginDto.DeviceToken)
                            {
                                deviceToken.DeviceToken = loginDto.DeviceToken;
                                Context.Devices.Update(deviceToken);
                                await Context.SaveChangesAsync();
                            }
                        }
                    }
                    
                    else if(result == SignInResult.NotAllowed)
                    {
                        operation.OperationResultType = OperationResultTypes.Forbidden;
                        operation.Result = null;
                    }
                    else
                    {
                        operation.OperationResultType = OperationResultTypes.Failed;
                        operation.Result = null;
                    }
                }
            }
            catch (Exception ex)
            {
                operation.Exception = ex;
                operation.OperationResultType = OperationResultTypes.Exception;
            }
            return operation;
        }

        private string GenerateJwtToken(string email)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role,""),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtOptions:Key"]));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["JwtOptions:Issuer"],
                audience: configuration["JwtOptions:audience"],
                claims: claims,
            //    expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<OperationResult<ProfileDto>> GetUsers()
        {
            OperationResult<ProfileDto> operation = new OperationResult<ProfileDto>();
            try
            {
                var users = await Context.Profiles
                .Where(u => !u.DateDeleted.HasValue)
                .Include(u => u.HUser)
                .Select(u => new ProfileDto
                {
                    Firstname = u.FirstName,
                    LastName = u.LastName,
                    Email = u.HUser.Email,
                    Username = u.HUser.UserName,
                    Gender = u.Gender,
                    Id = u.Id
                    
                }).ToListAsync();

                operation.OperationResultType = OperationResultTypes.Success;
                operation.IEnumerableResult = users;

            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<UpdateUserDto>> UpdateUser(UpdateUserDto updateUserDto)
        {
            OperationResult<UpdateUserDto> operation = new OperationResult<UpdateUserDto>();
            try
            {
                var profile = await Context.Profiles
                    .Where(p => p.Id == updateUserDto.Id)
                    .SingleOrDefaultAsync();

                if (profile != null)
                {
                    profile.Avatar = updateUserDto.Avatar;
                    profile.BirthDate = updateUserDto.BirthDate.FixFormatDate();
                    profile.FirstName = updateUserDto.Firstname;
                    profile.LastName = updateUserDto.LastName;
                    profile.Gender = updateUserDto.Gender;
                    Context.Profiles.Update(profile);
                    await Context.SaveChangesAsync();
                    operation.OperationResultType = OperationResultTypes.Success;
                    operation.Result = updateUserDto;
                }
                else
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                    operation.Result = null;
                }
            }
            catch (Exception ex)
            {
                operation.Exception = ex;
                operation.OperationResultType = OperationResultTypes.Exception;
            }
            return operation;
        }

        public async Task<OperationResult<bool>> DeleteUser(string email)
        {
            OperationResult<bool> operation = new OperationResult<bool>();
            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                    operation.Result = false;
                }
                else
                {
                    var result = await userManager.DeleteAsync(user);
                    if(result == IdentityResult.Success)
                    {
                        operation.OperationResultType = OperationResultTypes.Success;
                        operation.Result = true;
                    }
                    else
                    {
                        operation.OperationResultType = OperationResultTypes.Failed;
                        operation.Result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                operation.Exception = ex;
                operation.OperationResultType = OperationResultTypes.Exception;
            }
            return operation;
        }

        public async Task<OperationResult<bool>> ConfirmEmail(ConfirmEmailDto confirmEmailDto)
        {
            OperationResult<bool> operation = new OperationResult<bool>();
            try
            {
                if (confirmEmailDto.UserId == 0 || confirmEmailDto.Token == null)
                {
                    operation.OperationResultType = OperationResultTypes.Failed;
                    operation.Result = false;
                }

                // var user = await userManager.FindByIdAsync(userId.ToString());
                var user = await Context.Profiles.Where(p => (p.Id == confirmEmailDto.UserId) && !(p.DateDeleted.HasValue)).SingleOrDefaultAsync();
                if (user == null)
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                    operation.Result = false;
                }
                var user2 = await userManager.FindByIdAsync(user.HUserFk.ToString());
                if(confirmEmailDto.Token == user.Token)
                {
                    user2.EmailConfirmed = true;
                    await Context.SaveChangesAsync();
                    operation.OperationResultType = OperationResultTypes.Success;
                    operation.Result = true;
                }
                else
                {
                    operation.OperationResultType = OperationResultTypes.Forbidden;
                    operation.Result = false;
                }
                 
                    
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<UserInfoDtoResponse>> GetUserInfoById(int id)
        {
            OperationResult<UserInfoDtoResponse> operation = new OperationResult<UserInfoDtoResponse>();
            try
            {
                var data =  await Context.Profiles
                    .Include(p=> p.HUser)
                    .Where(p => (!p.DateDeleted.HasValue) && (p.Id == id))
                    .Select(p => new UserInfoDtoResponse
                    {
                        Avatar = p.Avatar,
                        BirthDate = p.BirthDate.ToString(),
                        Email = p.HUser.Email,
                        FirstName = p.FirstName,
                        Gender = p.Gender,
                        LastName = p.LastName,
                        UserId = p.Id,
                        UserName = p.HUser.UserName
                    }).SingleOrDefaultAsync();
                if(data != null)
                {
                    operation.OperationResultType = OperationResultTypes.Success;
                    operation.Result = data;
                }
                else
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                    operation.Result = null;
                }
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<UserInfoDtoResponse>> GetUserInfoByEmail(string email)
        {
            OperationResult<UserInfoDtoResponse> operation = new OperationResult<UserInfoDtoResponse>();
            try
            {
                var data = await Context.Profiles
                    .Include(p => p.HUser)
                    .Where(p => (!p.DateDeleted.HasValue) && (p.HUser.Email == email))
                    .Select(p => new UserInfoDtoResponse
                    {
                        Avatar = p.Avatar,
                        BirthDate = p.BirthDate.ToString(),
                        Email = p.HUser.Email,
                        FirstName = p.FirstName,
                        Gender = p.Gender,
                        LastName = p.LastName,
                        UserId = p.Id,
                        UserName = p.HUser.UserName
                    }).SingleOrDefaultAsync();
                if (data != null)
                {
                    operation.OperationResultType = OperationResultTypes.Success;
                    operation.Result = data;
                }
                else
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                    operation.Result = null;
                }
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        //for confirm email
        private async Task<bool> SendToken(RegisterUserDto userDto, string token)
        {
            try
            {
                //instantiate a new MimeMessage
                var message = new MimeMessage();
                //Setting the To e-mail address
                message.From.Add(new MailboxAddress("Hasebni Application", configuration["EmailSender:Email"]));
                //Setting the From e-mail address
                message.To.Add(new MailboxAddress(userDto.FirstName + " " + userDto.LastName, userDto.Email));
                //E-mail subject 
                message.Subject = "Subject";
                //E-mail message body
                message.Body = new TextPart("plain")
                {
                    Text = @$"مرحبا .. شكرا لك على اشتراكك في تطبيقنا 
                          إن رمز تأكيد حسابك المؤلف من 6 أرقام هو :
                             {token}
                           انسخه وضعه في الحقل المخصص لتأكيد الحساب وابقه سريا للحفاظ على أمان حسابك."
                    //Text = "مرحبا ، شكرا لك على اشتراكك في تطبيقنا." +
                    //$"إن رمز تأكيد حسابك المؤلف من 6 رموز هو : ({token})"
                    //+ "ضعه في الحقل المخصص لتأكيد الحساب. "
                    //+ "ابقه سريا للحفاظ على أمان حسابك"
                    //Text = $"Hello , my name is osama ,Your Authentcation token is {token}"
                };

                //Configure the e-mail
                using (var emailClient = new SmtpClient())
                {
                    emailClient.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    //emailClient.Connect("smtp.yandex.ru", 465, MailKit.Security.SecureSocketOptions.Auto);
                    // await emailClient.AuthenticateAsync("osamaAlrashed@yandex.com", "0956057886");
                    await emailClient.AuthenticateAsync(configuration["EmailSender:Email"], configuration["EmailSender:Password"]);
                    emailClient.Send(message);
                    emailClient.Disconnect(true);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        private async Task<bool> SendResetPasswordToken(RegisterUserDto userDto, string token)
        {
            try
            {
                //instantiate a new MimeMessage
                var message = new MimeMessage();
                //Setting the To e-mail address
                message.From.Add(new MailboxAddress("Hasebni Application", configuration["EmailSender:Email"]));
                //Setting the From e-mail address
                message.To.Add(new MailboxAddress(userDto.FirstName + " " + userDto.LastName, userDto.Email));
                //E-mail subject 
                message.Subject = "Subject";
                //E-mail message body
                var encodedtoken = Encoding.UTF8.GetBytes(token);
                var validToken = WebEncoders.Base64UrlEncode(encodedtoken);
                string url = $"{configuration["AppUrl"]}ResetPassword?email={userDto.Email}&token={validToken}";
                message.Body = new TextPart("plain")
                {
                    Text = $@"اتبع الرابط لإعادة تعيين كلمة السر:"
                           +  $"{url}"                                  
                };
                //Configure the e-mail
                using (var emailClient = new SmtpClient())
                {
                    emailClient.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    //emailClient.Connect("smtp.yandex.ru", 465, MailKit.Security.SecureSocketOptions.Auto);
                    // await emailClient.AuthenticateAsync("osamaAlrashed@yandex.com", "0956057886");
                    await emailClient.AuthenticateAsync(configuration["EmailSender:Email"], configuration["EmailSender:Password"]);
                    emailClient.Send(message);
                    emailClient.Disconnect(true);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<OperationResult<string>> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            OperationResult<string> operation = new OperationResult<string>();
            try
            {
                var user = Context.Users
                    .Where(u => u.Id == (Context.Profiles
                                        .Where(p => p.Id == changePasswordDto.Id)
                                        .SingleOrDefault().HUserFk))
                    .SingleOrDefault();
                if(user != null)
                {
                    var result = await userManager.ChangePasswordAsync(user, changePasswordDto.Password, changePasswordDto.NewPassword);
                    if(result == IdentityResult.Success)
                    {
                        await Context.SaveChangesAsync();
                        operation.OperationResultType = OperationResultTypes.Success;
                        operation.Result = changePasswordDto.NewPassword;
                    }
                    else
                    {
                        operation.OperationResultType = OperationResultTypes.Failed;
                    }
                }
                else
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                }
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<bool>> ForgetPassword(string email)
        {
            OperationResult<bool> operation = new OperationResult<bool>();
            try
            {
                var user = await userManager.FindByEmailAsync(email);
                var userDto = new RegisterUserDto
                {
                    Email = user.Email
                };
                if(user != null)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    if(await SendResetPasswordToken(userDto, token))
                    {
                        operation.OperationResultType = OperationResultTypes.Success;
                        operation.Result = true;
                    }
                    else
                    {
                        operation.OperationResultType = OperationResultTypes.Failed;
                        operation.Result = false;
                    }
                    //  var x = userManager.ResetPasswordAsync(user,token,newpass)
                }
                else
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                    operation.Result = false;
                }
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<ProfileDto>> SearchByUserName(string userName)
        {
            OperationResult<ProfileDto> operation = new OperationResult<ProfileDto>();
            try
            {
                var results = await Context.Profiles.Include(p => p.HUser)
                    .Where(p => (!p.DateDeleted.HasValue) && (p.HUser.UserName.Contains(userName)))
                    .ToListAsync();
                operation.OperationResultType = OperationResultTypes.Success;
                operation.IEnumerableResult = results.Select(r=> new ProfileDto { 
                    Firstname = r.FirstName,
                    LastName = r.LastName,
                    Username = r.HUser.UserName,
                    Avatar = r.Avatar
                });
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<bool>> ResetPassword(ResetPasswordDto model)
        {
            OperationResult<bool> operation = new OperationResult<bool>();
            try
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if(user == null)
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                    operation.Result = false;
                }
                else
                {
                    var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
                    var token = Encoding.UTF8.GetString(decodedToken);

                    var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    if(result == IdentityResult.Success)
                    {
                        operation.OperationResultType = OperationResultTypes.Success;
                        operation.Result = true;
                    }
                    else
                    {
                        operation.OperationResultType = OperationResultTypes.Failed;
                        operation.Result = false;
                    }
                }
            }
            catch (Exception ex) 
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }




        #endregion
        //private SendResetPassLink()
    }
}