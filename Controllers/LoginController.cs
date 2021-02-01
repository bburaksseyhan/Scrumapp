using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using scrum_ui.ViewModel;
using scrum_ui.Repository;
using scrum_ui.Extension;
using AutoMapper;

namespace scrum_ui.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IParticipantRepository _participantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly ILogger<LoginController> _logger;
        private readonly IMapper _mapper;
        public LoginController(IUserRepository userRepository,
                                IParticipantRepository participantRepository,
                               IMapper mapper,
                               IHttpContextAccessor httpContextAccessor,
                               ILogger<LoginController> logger)
        {
            _userRepository = userRepository;
            _participantRepository = participantRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public IActionResult Index(bool IsRedirect)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginModel login)
        {
            var user = await _userRepository.GetUser(login.Email);
            
            if (!ModelState.IsValid && user == null)
                return View("Index");
            
            var userviewModel = _mapper.Map<UserViewModel>(user);

            bool verified = BCrypt.Net.BCrypt.Verify(login.Password, userviewModel.Password);
            if(!verified)
                return View("Index");

            userviewModel.IsVerified = verified;
            
            var participant = await _participantRepository.GetParticipant(login.Email);

            userviewModel.GroupId = participant == null ? null : participant.GroupId;
            
            HttpContext.Session.SetComplexData("logged_user_data", userviewModel);  

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("logged_user_data");

            return RedirectToAction("Index", "Login");
        }
    }
}