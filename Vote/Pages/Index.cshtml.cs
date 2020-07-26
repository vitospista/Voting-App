using System;
using Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vote.Event;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Vote.Pages
{
    //[Authorize]
    public class IndexModel : PageModel
    {
        private string _optionA;
        private string _optionB;

        protected readonly IMessageQueue _messageQueue;
        protected readonly IConfiguration _configuration;
        protected readonly ILogger _logger;

        public IndexModel(IMessageQueue messageQueue, IConfiguration configuration, ILogger<IndexModel> logger)
        {
            _messageQueue = messageQueue;
            _configuration = configuration;
            _logger = logger;

            _optionA = _configuration.GetValue<string>("Voting:OptionA");
            _optionB = _configuration.GetValue<string>("Voting:OptionB");
        }

        public string OptionA { get; private set; }

        public string OptionB { get; private set; }

        [BindProperty]
        public string Vote { get; private set; }

        private string _voterId 
        {
            get { return TempData.Peek("VoterId") as string; }
            set { TempData["VoterId"] = value; }
        }

        public void OnGet()
        {
            OptionA = _optionA;
            OptionB = _optionB;
        }

        public IActionResult OnPostLogin(string provider)
        {
            return Challenge(provider);
        }

        public IActionResult OnPost(string vote)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToPage();
            }

            Vote = vote;
            OptionA = _optionA;
            OptionB = _optionB;
            if (_configuration.GetValue<bool>("MessageQueue:Enabled"))
            {
                PublishVote(vote);
            }
            return Page();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync();
            DeleteCookies();
            return RedirectToPage();
        }

        private void PublishVote(string vote)
        {
            if (string.IsNullOrEmpty(_voterId))
            {
                _voterId = Guid.NewGuid().ToString();
            }
            var message = new VoteCastEvent
            {
                VoterId = _voterId,
                Vote = vote
            };
           _messageQueue.Publish(message);
        }

        private void DeleteCookies()
        {
            foreach (var cookie in HttpContext.Request.Cookies)
            {
                Response.Cookies.Delete(cookie.Key);
            }
        }
    }
}
