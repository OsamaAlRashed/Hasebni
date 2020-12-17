using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hasebni.API.Pages;
using Hasebni.API.ViewModels;
using Hasebni.Base;
using Hasebni.Model.Security;
using Hasebni.Security.Dto.User;
using Hasebni.Security.Idata.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hasebni.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly IAPIRepository APIRepository;

        public AccountController(IAPIRepository APIRepository)
        {
            this.APIRepository = APIRepository;
        }

        //Account/RegisterUser
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserDto userDto)
        {
            var result = await APIRepository.RegisterUser(userDto);

            //var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account",
            //    new { result.Result.Token , email = result.Result.Email}, Request.Scheme);
           // var message = new Message(new string[] { result.Result.Email }, "Confirmation email link", confirmationLink, null);
           // await emailSender.SendEmailAsync(message);

            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
                case OperationResultTypes.Failed:
                    return new JsonResult("Failed") { StatusCode = 404 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }

        //Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await APIRepository.Login(loginDto);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
                case OperationResultTypes.Failed:
                    return new JsonResult("Failed") { StatusCode = 404 };
                case OperationResultTypes.NotExist:
                    return new JsonResult("NotExist") { StatusCode = 204 };
                case OperationResultTypes.Forbidden:
                    return new JsonResult("Forbidden") { StatusCode = 403 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }

        //Account/IsUserNotNameUsed
        [HttpGet]
        public async Task<IActionResult> IsUserNotNameUsed(string username)
        {
            var result = await APIRepository.IsUserNameNotUsed(username);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
                case OperationResultTypes.Exist:
                    return new JsonResult(result.Result) { StatusCode = 209 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }

        //Account/IsEmailNotUsed
        [HttpGet]
        public async Task<IActionResult> IsEmailNotUsed(string email)
        {
            var result = await APIRepository.IsEmailNotUsed(email);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
                case OperationResultTypes.Exist:
                    return new JsonResult(result.Result) { StatusCode = 209 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }

        //Account/UpdateUser
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UpdateUserDto updateUserDto)
        {
            var result = await APIRepository.UpdateUser(updateUserDto);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
                case OperationResultTypes.Failed:
                    return new JsonResult("Failed") { StatusCode = 404 };
                case OperationResultTypes.NotExist:
                    return new JsonResult("NotExist") { StatusCode = 204 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }

        //Account/DeleteUser
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var result = await APIRepository.DeleteUser(email);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
                case OperationResultTypes.Failed:
                    return new JsonResult("Failed") { StatusCode = 404 };
                case OperationResultTypes.NotExist:
                    return new JsonResult("NotExist") { StatusCode = 204 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }

        //Account/ConfirmEmail
        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto confirmEmailDto)
        {
            var result = await APIRepository.ConfirmEmail(confirmEmailDto);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
                case OperationResultTypes.Failed:
                    return new JsonResult("Failed") { StatusCode = 404 };
                case OperationResultTypes.NotExist:
                    return new JsonResult("NotExist") { StatusCode = 204 };
                case OperationResultTypes.Forbidden:
                    return new JsonResult("Forbidden") { StatusCode = 401 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }

        //Account/ChangePassword
        [HttpPut]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var result = await APIRepository.ChangePassword(changePasswordDto);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
                case OperationResultTypes.Failed:
                    return new JsonResult("Failed") { StatusCode = 404 };
                case OperationResultTypes.NotExist:
                    return new JsonResult("NotExist") { StatusCode = 204 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }

        [HttpGet]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var result = await APIRepository.ForgetPassword(email);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
                case OperationResultTypes.Failed:
                    return new JsonResult("Failed") { StatusCode = 404 };
                case OperationResultTypes.NotExist:
                    return new JsonResult("NotExist") { StatusCode = 204 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromForm]ResetPasswordViewModel model)
        {
            ResetPasswordDto Dto = new ResetPasswordDto
            {
                Email = model.Email,
                Token = model.Token,
                NewPassword = model.NewPassword,
                ConfirmPassword = model.ConfirmPassword
            };
            var result = await APIRepository.ResetPassword(Dto);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Failed, Try again ...") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult("Done, Login again ...") { StatusCode = 200 };
                case OperationResultTypes.Failed:
                    return new JsonResult("Failed, Try again ...") { StatusCode = 404 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }


    }
}