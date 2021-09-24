using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;

namespace VirtualBank.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class UtilityPaymentController : ControllerBase
    {
        private readonly IUtilityPaymentService _utilityPaymentService;
        private readonly UserManager<AppUser> _userManager;

        public UtilityPaymentController(IUtilityPaymentService utilityPaymentService,
                                        UserManager<AppUser> userManager)
        {
            _utilityPaymentService = utilityPaymentService;
            _userManager = userManager;
        }

    }
}