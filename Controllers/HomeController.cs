using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using scrum_ui.Repository;
using System.Threading.Tasks;
using scrum_ui.Models;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;
using System.Collections.Generic;
using scrum_ui.ViewModel;
using scrum_ui.Extension;

namespace scrum_ui.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly IGroupRepository _groupRepository;
        private readonly IParticipantRepository _participantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IHubContext<ScrumHub> _hubContext;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        public HomeController(IHttpContextAccessor httpContextAccessor,
                              IHubContext<ScrumHub> scrumHub,
                              IGroupRepository groupRepository,
                              IUserRepository userRepository,
                              IParticipantRepository participantRepository,
                              ILogger<HomeController> logger,
                              IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _participantRepository = participantRepository;
            _userRepository = userRepository;
            _hubContext = scrumHub;
            _groupRepository = groupRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var session = HttpContext.Session.GetComplexData<UserViewModel>("logged_user_data");

            if (session == null)
                return RedirectToAction("Index", "Login");

            if (session.GroupId != null)
            { //group id yoksa dışarıdan geliyor demektir direct oturuma yönlendir.
                var data = await _groupRepository.GetSingleGroup(session.GroupId.Value);
                
                var query = $"{data.Redirect}/home/session?key={data.Key}&email={session.Email}&owner=false";

                return Redirect(query);
            }

            //bu session kullanıcısına ait oturum var mı varsa ownerdır
            var groups = await _groupRepository.GetGroupAsync(session.Id);
        
            var groupViewModels = _mapper.Map<List<GroupViewModel>>(groups);

            groupViewModels.ForEach(x => x.Email = session.Email);
                
            return View(groupViewModels);
        }

        //only participant leader come to that action
        public async Task<ActionResult> Session(string key, string email, bool owner)
        {
            var userIsExist = await _userRepository.GetUser(email);//yoksa kayıt olmadı demek
            if (userIsExist == null)
                    return RedirectToAction("Index", "Register");

            var session = HttpContext.Session.GetComplexData<UserViewModel>("logged_user_data");
            if (session == null)
                return RedirectToAction("Index", "Login");

            var group = await _groupRepository.GetSingleGroupByKeyAsync(key);
            var sessionDto = new Session();
            sessionDto.Email = session.Email;
            sessionDto.Key = key;
            sessionDto.Group.Id = group.Id;
            
            var isOwner = await _groupRepository.GetGroupAsync(session.Id); 
        
            if(isOwner.Count > 0){
                if(isOwner[0].Key.Equals(key))
                {
                    sessionDto.isOwner = owner;
                }
            }else
            {
                sessionDto.isOwner = owner;
            }
            sessionDto.Group = await _groupRepository.GetSingleGroupByKeyAsync(sessionDto.Key);
            var participant = await _participantRepository.GetParticipant(session.Email);
            sessionDto.Image = participant == null ? "/img/sheriff.png" : participant.Image;

            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(email) && !owner)
                SendData(key, email, false, sessionDto.Image);

            return View(sessionDto);
        }

        [HubMethodName("NotifyService")]
        private async void SendData(string key, string email, bool isOwner, string image) => await _hubContext.Clients.All.SendCoreAsync("ReceiveMessage", new object[] { key, email, isOwner, image });
    }
}