using BlueWASIdnty.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlueWASIdnty.Server.Controllers
{
    public class UserController : Controller
    {

        UserManager<ApplicationUser> _UserManager;

        RoleManager<IdentityRole> _RoleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _UserManager = userManager;
            _RoleManager = roleManager; 
            
        }

        // Property used to add or edit the currently selected user

        //ApplicationUser UserData = new ApplicationUser();

        // Tracks the selected role for the currently selected user

        string SelectedUserRole { get; set; } = "User";

        // Collection to display the existing users

        List<ApplicationUser> ColUsers = new List<ApplicationUser>();

        // Options to display in the roles dropdown when editing a user

        List<string> Options = new List<string>() { "User", "Admin" };

        // To hold any possible errors

        string strError = "";


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("User/GetUsers")]

        public List<ApplicationUser> GetUsers()
        {

            // clear any error messages

            strError = "";

            // Collection to hold users

            ColUsers = new List<ApplicationUser>();

            // get users from _UserManager

            var user = _UserManager.Users.Select(x => new ApplicationUser

            {

                Id = x.Id,

                UserName = x.UserName,

                Email = x.Email,

                PasswordHash = "*****"

            });

            foreach (var item in user)

            {

                ColUsers.Add(item);

            }

            return ColUsers;
        }

        [HttpPost("User/SaveUser")]
        async Task SaveUser(ApplicationUser UserData)

        {
            try

            {

                // Is this an existing user?

                if (UserData.Id != "")

                {

                    // Get the user

                    var user = await _UserManager.FindByIdAsync(UserData.Id);

                    // Update Email

                    user.Email = UserData.Email;

                    // Update the user

                    await _UserManager.UpdateAsync(user);

                    // Only update password if the current value

                    // is not the default value

                    if (UserData.PasswordHash != "*****" && string.IsNullOrWhiteSpace(UserData.PasswordHash))

                    {

                        var resetToken =

                            await _UserManager.GeneratePasswordResetTokenAsync(user);

                        var passworduser =

                            await _UserManager.ResetPasswordAsync(

                                user,

                                resetToken,

                                UserData.PasswordHash);

                        if (!passworduser.Succeeded)

                        {

                            if (passworduser.Errors.FirstOrDefault() != null)

                            {

                                strError =

                                    passworduser

                                    .Errors

                                    .FirstOrDefault()

                                    .Description;

                            }

                            else

                            {

                                strError = "Pasword error";

                            }

                            // Keep the popup opened

                            return;

                        }

                    }

                    // Handle Roles

                    // Is user in ALready in role?

                    var UserResult = await _UserManager.IsInRoleAsync(user, SelectedUserRole);


                    if (!UserResult)

                    {

                        // Put in selected role

                        await _UserManager.AddToRoleAsync(user, SelectedUserRole);

                        // Remove user from other role

                        await _UserManager.RemoveFromRolesAsync(user, Options.Where(x => x != SelectedUserRole));

                    }


                }

                else

                {

                    // Insert new user

                    var NewUser =

                        new ApplicationUser

                        {

                            UserName = UserData.UserName,

                            Email = UserData.Email

                        };

                    var CreateResult =

                        await _UserManager

                        .CreateAsync(NewUser, UserData.PasswordHash);

                    if (!CreateResult.Succeeded)

                    {

                        if (CreateResult

                            .Errors

                            .FirstOrDefault() != null)

                        {

                            strError =

                                CreateResult

                                .Errors

                                .FirstOrDefault()

                                .Description;

                        }

                        else

                        {

                            strError = "Create error";

                        }

                        // Keep the popup opened

                        return;

                    }

                    else

                    {

                        // Handle Roles

                        if (SelectedUserRole != null)

                        {

                            // Put admin in Administrator role

                            await _UserManager

                                .AddToRoleAsync(NewUser, SelectedUserRole);

                        }

                    }

                }

              

            }

            catch (Exception ex)

            {

                strError = ex.GetBaseException().Message;

            }

        }


       [HttpPost("user/EditUser")]
        async Task EditUser(ApplicationUser UserData)

        {

            // Set the selected user

            // as the current user

            
            // Get the user

            var user = await _UserManager.FindByIdAsync(UserData.Id);

            if (user != null)

            {

                // Is user in administrator role?

                var UserResult =

                    await _UserManager

                    .IsInRoleAsync(user, "Admin");

                if (UserResult)

                {

                    SelectedUserRole = "Admin";

                }

                else

                {

                    SelectedUserRole = "User";

                }

            }



        }

        async Task DeleteUser(ApplicationUser UserData)

        {

            

            // Get the user

            var user = await _UserManager.FindByIdAsync(UserData.Id);

            if (user != null)

            {

                // Delete the user

                await _UserManager.DeleteAsync(user);

            }

            

        }

    }
}
