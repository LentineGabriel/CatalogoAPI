using CatagoloAPI.DTOs;
using CatagoloAPI.Models;
using CatagoloAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CatagoloAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    #region Props/Ctor
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _cfg;

    public AuthController(ITokenService tokenService , UserManager<ApplicationUser> userManager , RoleManager<IdentityRole> roleManager , IConfiguration cfg)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _cfg = cfg;
    }
    #endregion

    #region CreateRole and AddUserToRole
    [HttpPost]
    [Route("CriarPerfil")]
    [Authorize(Policy = "BossOnly")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var roleExist = await _roleManager.RoleExistsAsync(roleName);
        if(!roleExist)
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if(roleResult.Succeeded) return StatusCode(StatusCodes.Status200OK , new ResponseDTO { Status = "Success" , Message = "Role created successfully!" });
            else return StatusCode(StatusCodes.Status400BadRequest , new ResponseDTO { Status = "Error" , Message = "Role creation failed!" });
        }

        return StatusCode(StatusCodes.Status400BadRequest , new ResponseDTO { Status = "Error" , Message = "Role already exists!" });
    }

    [HttpPost]
    [Route("AdicionarUsuarioAoPerfil")]
    [Authorize(Policy = "BossOnly")]
    public async Task<IActionResult> AddUserToRole(string email , string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if(user != null)
        {
            var result = await _userManager.AddToRoleAsync(user , roleName);
            if(result.Succeeded) return StatusCode(StatusCodes.Status200OK , new ResponseDTO { Status = "Success" , Message = "User added to role successfully!" });
            else return StatusCode(StatusCodes.Status400BadRequest , new ResponseDTO { Status = "Error" , Message = "Adding user to role failed!" });
        }

        return BadRequest(new { Error = "Unable to find user." });
    }
    #endregion

    #region Login
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModelDTO loginModel)
    {
        var user = await _userManager.FindByNameAsync(loginModel.Username!);
        if(user is not null && await _userManager.CheckPasswordAsync(user , loginModel.Password!))
        {
            // user informations
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            foreach(var ur in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role , ur));
            }

            // token
            var token = _tokenService.GenerateAccessToken(authClaims , _cfg);
            var refreshToken = _tokenService.GenerateRefreshToken();
            _ = int.TryParse(_cfg["JWT:RefreshTokenValidityInMinutes"] , out int refreshTokenValidityInMinutes);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token) ,
                RefreshToken = refreshToken ,
                Expiration = token.ValidTo
            });
        }

        return Unauthorized();
    }
    #endregion

    #region Register
    [HttpPost]
    [Route("RegistrarUsuário")]
    public async Task<IActionResult> Register([FromBody] RegisterModelDTO registerModel)
    {
        var userExits = await _userManager.FindByNameAsync(registerModel.Username!);
        if(userExits is not null) return StatusCode(StatusCodes.Status500InternalServerError ,
                                  new ResponseDTO { Status = "Error" , Message = "User already exists!" });

        ApplicationUser user = new()
        {
            SecurityStamp = Guid.NewGuid().ToString() ,
            Email = registerModel.Email ,
            UserName = registerModel.Username
        };

        var result = await _userManager.CreateAsync(user , registerModel.Password!);
        if(!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError ,
                              new ResponseDTO { Status = "Error" , Message = "User creation failed." });

        return Ok(new ResponseDTO { Status = "Success" , Message = "User created successfully!" });
    }
    #endregion

    #region RefreshToken
    [HttpPost]
    [Route("RefreshToken")]
    public async Task<IActionResult> RefreshToken(TokenModelDTO tokenModel)
    {
        if(tokenModel is null) return BadRequest("Invalid client request");
        string? accessToken = tokenModel.AccessToken ?? throw new ArgumentNullException(nameof(tokenModel));
        string? refreshToken = tokenModel.RefreshToken ?? throw new ArgumentNullException(nameof(tokenModel));

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken! , _cfg);
        if(principal == null) return BadRequest("Invalid access token/refresh token");

        string username = principal.Identity!.Name!;
        var user = await _userManager.FindByNameAsync(username!);
        if(user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now) return BadRequest("Invalid access token/refresh token");

        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList() , _cfg);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken) ,
            refreshToken = newRefreshToken
        });
    }
    #endregion

    #region Revoke
    [HttpPost]
    [Route("RevogarToken/{username}")]
    [Authorize(Policy = "BossOnly")]
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if(user == null) return BadRequest("Invalid user name");

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }
    #endregion
}