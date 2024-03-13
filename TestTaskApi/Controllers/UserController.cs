using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestTaskApi.Database;
using TestTaskApi.DTOs;
using TestTaskApi.Helper;
using TestTaskApi.Interface;

namespace TestTaskApi.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userService;
        private readonly DatabaseContext context;
        private readonly JwtHelper _jwtHelper;
        public UserController(IUserRepository userService, DatabaseContext context, JwtHelper jwtHelper)
        {
            this.context = context;
            _jwtHelper = jwtHelper;

        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User model)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(model.ProfilePicture);

                var projectRootDirectory = Directory.GetCurrentDirectory();
                var targetDirectory = Path.Combine(projectRootDirectory, "wwwroot/Public/Images");

                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }


                var uniqueFileName = Guid.NewGuid().ToString() + ".png"; // You can adjust the extension


                var filePath = Path.Combine(targetDirectory, uniqueFileName);



                System.IO.File.WriteAllBytes(filePath, imageBytes);





                // Save the file to the server

                // Create a URL for the image
                var imageUrl = Path.Combine("Public/Images", uniqueFileName);

                // Update the user's profileImagePath with the file path
                var urlPath = imageUrl.Replace(Path.DirectorySeparatorChar, '/');

                var baseUrl = $"{HttpContext.Request.Scheme}:/{HttpContext.Request.Host}";

                // Create the complete URL
                var imageUrl1 = baseUrl + "/" + imageUrl;
                var mainimageUrl = imageUrl1.Replace("\\", "/");
                //var filePath1 = Path.Combine(baseUrl, imageUrl);
                string? FileUrl = mainimageUrl.ToString();

                model.ProfilePicture = FileUrl;
                Hasher hs = new Hasher();
                string haspassword = hs.HashPassword(model.Password);
                model.Password = haspassword;
                context.Users.Add(model);
                await context.SaveChangesAsync();



                return Ok(new ApiResponse<object>(true, null, "User Resgister Sucessfully", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ApiResponse<object>(false, null, ex.Message, 400));
            }
        }

        [HttpPut]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            try
            {
                var existingUser = await context.Users.FindAsync(id);

                if (existingUser == null)
                {
                    return NotFound(new ApiResponse<object>(false, null, "User not found", 404));
                }
                Hasher hs = new Hasher();
                string haspassword = hs.HashPassword(updatedUser.Password);



                byte[] imageBytes = Convert.FromBase64String(updatedUser.ProfilePicture);

                var projectRootDirectory = Directory.GetCurrentDirectory();
                var targetDirectory = Path.Combine(projectRootDirectory, "wwwroot/Public/Images");

                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }


                var uniqueFileName = Guid.NewGuid().ToString() + ".png"; // You can adjust the extension


                var filePath = Path.Combine(targetDirectory, uniqueFileName);



                System.IO.File.WriteAllBytes(filePath, imageBytes);





                // Save the file to the server

                // Create a URL for the image
                var imageUrl = Path.Combine("Public/Images", uniqueFileName);

                // Update the user's profileImagePath with the file path
                var urlPath = imageUrl.Replace(Path.DirectorySeparatorChar, '/');

                var baseUrl = $"{HttpContext.Request.Scheme}:/{HttpContext.Request.Host}";

                // Create the complete URL
                var imageUrl1 = baseUrl + "/" + imageUrl;
                var mainimageUrl = imageUrl1.Replace("\\", "/");
                //var filePath1 = Path.Combine(baseUrl, imageUrl);
                string? FileUrl = mainimageUrl.ToString();



                // Update properties of the existing user
                existingUser.UserName = updatedUser.UserName;
                existingUser.Address = updatedUser.Address;
                existingUser.EmailId = updatedUser.EmailId;
                existingUser.Password = haspassword;
                existingUser.ProfilePicture = FileUrl;
                existingUser.Dob = updatedUser.Dob;
                // ... (update other properties)

                // Save changes to the database
                await context.SaveChangesAsync();

                return Ok(new ApiResponse<object>(true, null, "User updated successfully", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(false, null, ex.Message, 500));
            }
        }
        [HttpGet]
        [Route("getall")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var allUsers = context.Users.ToList();

                return Ok(new ApiResponse<IEnumerable<User>>(true, allUsers, "All users retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(false, null, ex.Message, 500));
            }
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var userToDelete = await context.Users.FindAsync(id);

                if (userToDelete == null)
                {
                    return NotFound(new ApiResponse<object>(false, null, "User not found", 404));
                }

                // Remove user from the context
                context.Users.Remove(userToDelete);

                // Save changes to the database
                await context.SaveChangesAsync();

                return Ok(new ApiResponse<object>(true, null, "User deleted successfully", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(false, null, ex.Message, 500));
            }
        }

        [HttpGet]
        [Route("get/{id}")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                var user = context.Users.Find(id);

                if (user == null)
                {
                    return NotFound(new ApiResponse<object>(false, null, "User not found", 404));
                }

                return Ok(new ApiResponse<User>(true, user, "User retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(false, null, ex.Message, 500));
            }
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInModel signInModel)
        {
            try
            {
                // Find the user by username
                var user = await context.Users.SingleOrDefaultAsync(u => u.UserName == signInModel.UserName);

                if (user == null)
                {
                    return NotFound(new ApiResponse<object>(false, null, "User not found", 404));
                }


                Hasher hs = new Hasher();
                string haspassword = hs.HashPassword(signInModel.Password);
                // Verify the password
                if (haspassword!=user.Password)
                {
                    return BadRequest(new ApiResponse<object>(false, null, "Invalid password", 400));
                }
                
                var token1 = _jwtHelper.GenerateJwtToken(user);
                

                // TODO: Generate a token for authentication if using JWT or other authentication mechanisms

                return Ok(new ApiResponse<object>(true, token1, "Sign-in successful", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(false, null, ex.Message, 500));
            }
        }
    }
}
