using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using scrum_ui.ViewModel;
using scrum_ui.Models;
using scrum_ui.Repository;

namespace scrum_ui.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RegisterController> _logger;
        private readonly IMapper _mapper;
        public RegisterController(ILogger<RegisterController> logger,
                                  IUserRepository userRepository,
                                  IMapper mapper)
        {
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterModel register)
        {
            if (!ModelState.IsValid)
                return View("Index");

            var isExist = await _userRepository.GetUser(register.Email);
            if(isExist != null)
                return View("Index");

            register.Password = BCrypt.Net.BCrypt.HashPassword(register.Password);
            register.RePassword = BCrypt.Net.BCrypt.HashPassword(register.RePassword);
            
            var result = await _userRepository.Register(_mapper.Map<User>(register));

            if (result)
                return RedirectToAction("Index", "Login");

            return View("Index");
        }
    }
}