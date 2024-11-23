using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CoolectorAPI.Repositories;
using CoolectorAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using CoolectorAPI.DTO;

/**
 * In your ASP.NET Core project, you'll need to install the following package for generating and validating JWT tokens:
 * dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
 * dotnet add package System.IdentityModel.Tokens.Jwt
 * 
 */

namespace CoolectorAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController :ControllerBase
    {
        private readonly UserRepository _userRepository;

        // The secret key for generating JWT
        private readonly SymmetricSecurityKey _key;


        /// <summary>
        /// Constructor initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="repository">An instance of <see cref="UserRepository"/> for handling SQL queries related to users.</param>
        /// <param name="configuration">An instance of <see cref="IConfiguration"/> used to retrieve application settings, including the JWT secret key.</param>
        /// <exception cref="ArgumentNullException">Thrown when the JWT secret key is not provided in the configuration.</exception>
        /// <remarks>
        /// This constructor initializes the <see cref="UserController"/> with the necessary dependencies,
        /// including a user repository for SQL queries and a secret key for generating JWT tokens.
        /// </remarks>
        public UserController(UserRepository repository, IConfiguration configuration)
        {
            _userRepository = repository;

            // Fetch the secret key from configuration and store it in _key
            var secretKey = configuration["JwtSettings:SecretKey"];

            // Ensure the secret key is not null or empty
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentNullException(nameof(secretKey), "Secret key for JWT is not provided in the configuration.");
            }

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }
        // ==================== end of constructor ====================



        // GET: api/user/getAll
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers() 
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }
        // ==================== end of getAll GET call ====================



        /// <summary>
        /// POST: api/user
        /// Creates a New User
        /// 
        /// </summary>
        /// <param name="userDto"></param>
        /// Contains email and password for the user signin.
        /// 
        /// <returns></returns>
        [HttpPost("new")]
        public async Task<ActionResult<User>> PostUser(UserCreateDTO userDto)
        {
            var user = new User 
            {
                Email = userDto.Email,
                Password = userDto.Password,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Relation = userDto.Relation
            };

            await _userRepository.AddUserAsync(user);
            return Ok(user);
        }
        // ==================== end of POST single user call ====================


        /// <summary>
        /// Authenticates a user by verifying the provided email and password.
        /// If the credentials are valid, a JWT token is generated and returned.
        /// Otherwise, an Unauthorized response is sent.
        /// </summary>
        /// <param name="loginDto">The data transfer object containing the user's login credentials (email and password).</param>
        /// <returns>
        /// A 200 OK response with a JWT token if authentication is successful.
        /// A 401 Unauthorized response if the credentials are invalid.
        /// </returns>
        /// <remarks>
        /// This method is accessed via the following endpoint: 
        /// POST: api/user/login
        /// 
        /// It uses the <see cref="UserLoginDTO"/> object to capture the email and password submitted by the user.
        /// The <see cref="GenerateJwtToken"/> method is called to generate a JWT token for the authenticated user.
        /// </remarks>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
        {
            var userCode = await _userRepository.GetUserByCredentialsAsync(loginDto.Email, loginDto.Password);

            if (userCode == null)
            {
                // If no user was found or password is incorrect, return 401 Unauthorized
                return Unauthorized(new { message = "Invalid email or password from API." });
            }

            // If user is found, generate JWT token
            var token = GenerateJwtToken(userCode.Value);

            // If user is found, return 200 OK with token
            return Ok(new { token });
        }
        // ==================== end of login POST call ====================




        /// <summary>
        /// Helper method to generate a JWT Token for the specified user.
        /// </summary>
        /// <param name="user">The user object used to generate claims in the JWT.</param>
        /// <returns>A JWT token as a string.</returns>
        /// <remarks>
        /// The token includes only the user's ID as a claim and has an expiration time of 10 minutes.
        /// </remarks>
        private string GenerateJwtToken(int userCode)
        {
            var claims = new[]
            {
                // Include only the user's ID as a claim
                new Claim("code", userCode.ToString())
            };              

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "Coolector", 
                audience: "Coolector",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
             );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        // ==================== end of JWT token helper method ====================


    }
}
