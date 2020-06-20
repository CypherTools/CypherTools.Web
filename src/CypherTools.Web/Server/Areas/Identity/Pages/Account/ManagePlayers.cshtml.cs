using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using CypherTools.Web.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using DynamicData;

namespace CypherTools.Web.Server.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Administrator")]
    public class ManagePlayersModel : PageModel
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public ManagePlayersModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public IList<ApplicationUser> Players { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            var users = await _userManager.GetUsersInRoleAsync("Player");

            Players = users;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                foreach (var player in Players)
                {
                    if (player.DiscordID.ToUpper() == "DELETE")
                    {
                        var users = await _userManager.GetUsersInRoleAsync("Player");

                        var usertodelete = users.FirstOrDefault(x => x.Email == player.Email);

                        var result = await _userManager.DeleteAsync(usertodelete);
                    }
                    else
                    {
                        var users = await _userManager.GetUsersInRoleAsync("Player");

                        var usertoupdate = users.FirstOrDefault(x => x.Email == player.Email);

                        usertoupdate.DiscordID = player.DiscordID;

                        var result = await _userManager.UpdateAsync(usertoupdate);
                    }
                }

                Players.RemoveMany(Players.Where(x => x.DiscordID.ToUpper() == "DELETE"));
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
