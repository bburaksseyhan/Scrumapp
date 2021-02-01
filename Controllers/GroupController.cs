using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using scrum_ui.Models;
using scrum_ui.Repository;
using Newtonsoft.Json;
using scrum_ui.ViewModel;
using scrum_ui.Extension;

namespace scrum_ui.Controllers
{
    public class GroupController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGroupRepository _groupRepository;
        private readonly IParticipantRepository _participantRepository;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly ILogger<GroupController> _logger;
        public GroupController(IHttpContextAccessor httpContextAccessor,
                               IGroupRepository groupRepository,
                               IUserRepository userRepository,
                               IParticipantRepository participantRepository,
                               ILogger<GroupController> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _participantRepository = participantRepository;
            _logger = logger;
            _groupRepository = groupRepository;
        }


        public IActionResult Index()
        {
            var session = HttpContext.Session.GetComplexData<UserViewModel>("logged_user_data");
            if (session == null)
                return RedirectToAction("Index", "Login");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(Group group)
        {
            var session = HttpContext.Session.GetComplexData<UserViewModel>("logged_user_data");
            if (session == null)
                return RedirectToAction("Index", "Login");
    
            group.UserId = session.Id;
            group.Redirect = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            
            if (!ModelState.IsValid)
                return View("Index");

            var result = await _groupRepository.Create(group);

            if (result)
                return RedirectToAction("Index", "Home");

            return View("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int groupId)
        {
            var session = HttpContext.Session.GetComplexData<UserViewModel>("logged_user_data");
            if (session == null)
                return RedirectToAction("Index", "Login");

            var result = await _groupRepository.Delete(groupId);

            return Json(result);
        }

        public async Task<ActionResult> Close(int groupId)
        {
            var session = HttpContext.Session.GetComplexData<UserViewModel>("logged_user_data");
            if (session == null)
                return RedirectToAction("Index", "Login");

            var result = await _groupRepository.Close(groupId);

            return Json(result);
        }

        public async Task<ActionResult> Participant(string key)
        {
            var session = HttpContext.Session.GetComplexData<UserViewModel>("logged_user_data");
            if (session == null)
                return RedirectToAction("Index", "Login");

            var groupInfo = await _groupRepository.GetSingleGroupByKeyAsync(key);

            var participants = await _participantRepository.GetParticipants(groupInfo.Id);
            
            return View(new ParticipantViewModel(){ Participants = participants, Key = key, Redirect = groupInfo.Redirect});
        }
    }
}