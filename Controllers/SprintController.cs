using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using scrum_ui.Extension;
using scrum_ui.Models;
using scrum_ui.Repository;
using scrum_ui.ViewModel;

namespace scrum_ui.Controllers
{
    public class SprintController : Controller
    {
        private IHubContext<ScrumHub> _hubContext;
        private readonly ITaskRepository _taskRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IParticipantRepository _participantRepository;
        private readonly ISprintLogRepository _sprintLogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly IMapper _mapper;
        private readonly ILogger<SprintController> _logger;
        public SprintController(IHubContext<ScrumHub> hubContext,
                                IMapper mapper,
                                IGroupRepository groupRepository,
                                IParticipantRepository participantRepository,
                                ISprintLogRepository sprintLogRepository,
                                IHttpContextAccessor httpContextAccessor,
                                ITaskRepository taskRepository,
                                ILogger<SprintController> logger)
        {
            _hubContext = hubContext;
            _taskRepository = taskRepository;
            _participantRepository = participantRepository;
            _sprintLogRepository = sprintLogRepository;
            _groupRepository = groupRepository;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto data)
        {
            var session = HttpContext.Session.GetComplexData<UserViewModel>("logged_user_data");
            if (session == null)
                return RedirectToAction("Index", "Login");

            var task = _mapper.Map<scrum_ui.Models.Task>(data);
            task.UserId = session.Id;

            await _taskRepository.CreateTask(task);

            var tasks = await _taskRepository.GetTasksByGroupId(task.GroupId);

            int totalScore = 0;
            foreach (var item in tasks)
            {
                if (item.Score != null)
                    totalScore += item.Score.Value;
            }

            return Json(new { Result = new { Task = task, TotalScore = totalScore, Count = tasks.Count } });
        }

        [HttpPost]
        public IActionResult ActivateButtons(bool visible, bool owner, string task)
        {
            var session = HttpContext.Session.GetComplexData<UserViewModel>("logged_user_data");
            if (session == null)
                return RedirectToAction("Index", "Login");

            SendData(visible, owner, task: task, false);

            return Json(visible);
        }

        //This methods request key then get all task who depens on request key
        [HttpGet]
        public async Task<IActionResult> GetTasks(string key)
        {
            var session = HttpContext.Session.GetComplexData<UserViewModel>("logged_user_data");
            if (session == null)
                return RedirectToAction("Index", "Login");

            var group = await _groupRepository.GetSingleGroupByKeyAsync(key);

            if (group == null)
                return Json("return no group result");

            var tasks = await _taskRepository.GetTasksByGroupId(group.Id);

            int totalScore = 0;
            foreach (var item in tasks)
            {
                if (item.Score.HasValue)
                    totalScore += item.Score.Value;
            }

            return Json(new { Result = new { Tasks = tasks, TotalScore = totalScore, Count = tasks.Count } });
        }

        [HttpPost]
        public async Task<IActionResult> TaskRemove(int id)
        {
            var session = HttpContext.Session.GetComplexData<UserViewModel>("logged_user_data");
            if (session == null)
                return RedirectToAction("Index", "Login");

            var task = await _taskRepository.GetTaskById(id);

            var result = await _taskRepository.RemoveTask(id);

            var tasks = await _taskRepository.GetTasksByGroupId(task.GroupId);

            int totalScore = 0;
            foreach (var item in tasks)
            {
                if (item.Score.HasValue)
                    totalScore += item.Score.Value;
            }

            SendData(false, false, "", false);

            return Json(new { Result = new { Tasks = tasks, TotalScore = totalScore, Count = tasks.Count } });
        }

        [HttpPost]
        public async Task<IActionResult> TaskScoreUpdate(int id, int score)
        {
            var session = HttpContext.Session.GetComplexData<UserViewModel>("logged_user_data");
            if (session == null)
                return RedirectToAction("Index", "Login");

            var result = await _taskRepository.UpdateTaskScore(id, score);

            var task = await _taskRepository.GetTaskById(id);

            var tasks = await _taskRepository.GetTasksByGroupId(task.GroupId);

            int totalScore = 0;
            foreach (var item in tasks)
            {
                if (item.Score.HasValue)
                    totalScore += item.Score.Value;
            }

            SendData(false, false, "", true);


            return Json(new { Result = new { Tasks = tasks, TotalScore = totalScore, Count = tasks.Count } });
        }

        [HttpPost]
        public async Task<IActionResult> NotifyMemberScore(int score, string key, string email, string task)
        {
            var session = HttpContext.Session.GetComplexData<UserViewModel>("logged_user_data");
            if (session == null)
                return RedirectToAction("Index", "Login");

            SendData(score, key, email);

            await _sprintLogRepository.Create(new SprintLog()
            {
                ParticipantName = session.Email,
                Task = task,
                GroupId = session.GroupId.Value,
                Score = score,
            });

            return Json(true);
        }


        [HttpPost]
        public async  Task<IActionResult> CloseSprint(string groupId){

            var result = await _sprintLogRepository.GetAllLogsByGroupId(2);

            return Json(result);
        }

        [HubMethodName("NotifyService")]
        private async void SendData(bool visible, bool owner, string task, bool isUpdate) => await _hubContext.Clients.All.SendCoreAsync("ChangesStateFibonacciButtons", new object[] { visible, owner, task, isUpdate });

        [HubMethodName("NotifyService")]
        private async void SendData(int score, string key, string email) => await _hubContext.Clients.All.SendCoreAsync("ReceiveFromMemberMessage", new object[] { score, key, email });
    }
}