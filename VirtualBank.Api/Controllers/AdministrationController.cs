using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.RoleApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.RolesApiResponses;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Entities;

namespace VirtualBank.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class AdministrationController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }


        // GET api/v1/administration/roles/list
        /// <summary>
        /// list all roles
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet(ApiRoutes.Administration.ListRoles)]
        public IActionResult ListRoles()
        {
            var apiRsponse = new ApiResponse();

            var roles = _roleManager.Roles;

            if (roles.ToList().Count == 0)
            {
                return Ok(apiRsponse);
            }

            return Ok(roles);
        }


        // POST api/v1/administration/roles
        /// <summary>
        /// Create a role
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost(ApiRoutes.Administration.CreateRole)]
        public async Task<IActionResult> CreateRole([FromQuery] string roleName, CancellationToken cancellationToken )
        {
            var apiRsponse = new ApiResponse();

            if (await CheckRoleExists(roleName))
            {
                apiRsponse.AddError(ExceptionCreator.CreateBadRequestError($"Role name '{roleName}' already exists"));

                return BadRequest(apiRsponse);
            }

            IdentityRole identityRole = new IdentityRole
            {
                Name = roleName
            };

            IdentityResult result = await _roleManager.CreateAsync(identityRole);

            if (result.Succeeded)
            {
                return Ok(apiRsponse);
            }

            foreach(IdentityError error in result.Errors)
            {
                apiRsponse.AddError(ExceptionCreator.CreateUnexpectedError(Convert.ToInt32(error.Code), error.Description));
            }

            return BadRequest(apiRsponse);
        }


        // GET api/v1/administration/roles/users
        /// <summary>
        /// Get user in role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet(ApiRoutes.Administration.GetUsersInRoles)]
        public async Task<IActionResult> GetUsersInRole([FromQuery] string roleId, CancellationToken cancellationToken)
        {
            var apiResponse = new ApiResponse();

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                apiResponse.AddError(ExceptionCreator.CreateNotFoundError(nameof(role), $"Role id: {roleId} is not found"));
                return NotFound(apiResponse);
            }

            var userRoleList = new List<UserRoleResponse>();

            foreach (var user in _userManager.Users)
            {
                var userRole = new UserRoleResponse()
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRole.HasRole = true;
                }
                else
                {
                    userRole.HasRole = false;
                }

                userRoleList.Add(userRole);
            }

            return Ok(userRoleList);
        }


        // POST api/v1/administration/roles/users
        /// <summary>
        /// Get user in role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost(ApiRoutes.Administration.EditUsersInRoles)]
        public async Task<IActionResult> EditUsersInRole([FromQuery] string roleId, EditUserRoleRequest request, CancellationToken cancellationToken)
        {
            var apiResponse = new ApiResponse();

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                apiResponse.AddError(ExceptionCreator.CreateNotFoundError(nameof(role), $"Role id: {roleId} is not found"));
                return NotFound(apiResponse);
            }

            var userRoleList = new List<UserRoleResponse>();

            foreach (var user in _userManager.Users)
            {
                var userRole = new UserRoleResponse()
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRole.HasRole = true;
                }
                else
                {
                    userRole.HasRole = false;
                }

                userRoleList.Add(userRole);
            }

            return Ok(userRoleList);
        }


        #region Private helper methods
        /// <summary>
        /// Check if the passed email is already registered 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private async Task<bool> CheckRoleExists(string roleName)
        {
            if (await _roleManager.FindByNameAsync(roleName) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
    }
}
