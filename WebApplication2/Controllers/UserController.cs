using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Helpers;
using WebApplication2.Models.DTOs;
using WebApplication2.Models.Entities;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes ="Bearer")]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IRepository _repository;
        private readonly IPhotoManager _photoManager;
        private readonly IAuthService _authService;

        public UserController(ILogger<UserController> logger, UserManager<AppUser> userManager,
            IMapper mapper, IRepository repository, IPhotoManager photoManager,
            IAuthService authService)
        {
            _logger = logger;
            _userManager = userManager;
            _mapper = mapper;
            _repository = repository;
            _photoManager = photoManager;
            _authService = authService;
        }

        [HttpGet()]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if(user == null)
                {
                    return NotFound($"No record found for user with id: {id}");
                }

                var userToReturn = _mapper.Map<UserToReturnDto>(user);
                
                return Ok(userToReturn);

            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("all")]
        [Authorize(Roles ="admin")]
        public IActionResult GetAll(int page, int perPage)
        {
            

            try
            {
                var users = _userManager.Users;

                var usersToReturnList = new List<UserToReturnDto>();
                foreach (var user in users)
                {
                    usersToReturnList.Add(_mapper.Map<UserToReturnDto>(user));
                }

                var paginated = UtitlityMethods.Paginate<UserToReturnDto>(usersToReturnList, page, perPage);

                return Ok(paginated);

            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserToAddDto model)
        {
            try
            {
                // Check if email already exists
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                    return BadRequest("Email already exists!");

                var appUser = _mapper.Map<AppUser>(model);
                var addResult = await _userManager.CreateAsync(appUser, model.Password);
                if (!addResult.Succeeded)
                {
                    var errList = "";
                    foreach(var err in addResult.Errors)
                    {
                        errList += err.Description + ",\n";
                    }
                    return BadRequest(errList);
                }

                // add role to user
                await _userManager.AddToRoleAsync(appUser, "regular");
                return Ok($"User added with Id: {appUser.Id}");

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut()]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateDto model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return BadRequest($"No record found for user with id: {id}");

                var vResult = await _authService.ValidateLoggedInUser(User, user.Id);
                if (vResult["Code"] == "400")
                    return Unauthorized(vResult["Message"]);

                // update the appUser object with the userToUpdate dto using automapper
                _mapper.Map<UserUpdateDto, AppUser>(model, user);

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errList = "";
                    foreach (var err in updateResult.Errors)
                    {
                        errList += err.Description + ",\n";
                    }
                    return BadRequest(errList);
                }

                return Ok(_mapper.Map<UserToReturnDto>(user));

            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return BadRequest($"No record found for user with id: {id}");

                var delResult = await _userManager.DeleteAsync(user);
                if (!delResult.Succeeded)
                {
                    var errList = "";
                    foreach (var err in delResult.Errors)
                    {
                        errList += err.Description + ",\n";
                    }
                    return BadRequest(errList);
                }

                return Ok("Deleted successfully");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        public async Task<IActionResult> AddPhoto(string id, [FromForm] UserDtoToUploadPhoto model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return BadRequest($"No record found for user with id: {id}");

                var vResult = await _authService.ValidateLoggedInUser(User, user.Id);
                if (vResult["Code"] == "400")
                    return Unauthorized(vResult["Message"]);

                var uploadResult = await _photoManager.UploadImage(model.Photo);
                if (!uploadResult.IsSuccess)
                {
                    return BadRequest(uploadResult.Message);
                }

                // update photo details on user in db
                user.PhotoUrl = uploadResult.PhotoUrl;
                user.PublicId = uploadResult.PublicId;
                await _userManager.UpdateAsync(user);

                return Ok(_mapper.Map<UserToReturnDto>(user));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete-photo")]
        public async Task<IActionResult> DeletePhoto(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return BadRequest($"No record found for user with id: {id}");

                var deleteResult = await _photoManager.DeleteImage(user.PublicId);
                if (!deleteResult)
                {
                    return BadRequest("Failed to delete photo");
                }

                user.PhotoUrl = null;
                user.PublicId = null;
                await _userManager.UpdateAsync(user);

                return Ok("Photo deleted successfully");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("search")]
        [Authorize(Roles = "regular, admin")]
        public IActionResult GetAll(int page, int perPage, string searchTerm)
        {
            try
            {
                var users = _userManager.Users;

                var searchResult = users.Where(x => x.Email == searchTerm ||
                                               x.UserName == searchTerm ||
                                               x.Id == searchTerm).ToList();

                var usersToReturnList = new List<UserToReturnDto>();
                var paginated = UtitlityMethods.Paginate<AppUser>(searchResult, page, perPage);
                foreach (var user in paginated)
                {
                    usersToReturnList.Add(_mapper.Map<UserToReturnDto>(user));
                }


                return Ok(paginated);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
// Http verbs
/*
    - GET
    - POST
    - PATCH
    - PUT
    - DELETE
 */
// Http return methods
/*
    - Ok            - 200
    - BadRequest    - 400
    - Forbidden     - 403
    - NotFound      - 404
    - Internal Server Error - 500
    - Unauthorized  - 401
    - Created       - 201
    - NoContent     - 204
 */