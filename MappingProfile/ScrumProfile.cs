using AutoMapper;
using scrum_ui.ViewModel;
using scrum_ui.Models;

namespace scrum_ui.MappingProfile
{
    public class ScrumProfile : Profile
    {
        public ScrumProfile()
        {
            CreateMap<RegisterModel, User>()
                    .ForMember(x=>x.Email, x=>x.MapFrom(y=>y.Email))
                    .ForMember(x=>x.Password, x=>x.MapFrom(y=>y.Password))
                    .ForMember(x=>x.RePassword, x=>x.MapFrom(y=>y.RePassword))
                    .ForMember(x=>x.CreatedDate, x=>x.MapFrom(y=>y.CreateDate));

            CreateMap<Group, GroupViewModel>();

            CreateMap<CreateTaskDto, Task>()
                    .ForPath(x=>x.Name, x => x.MapFrom(y=>y.Task))
                    .ForPath(x=>x.GroupId, x => x.MapFrom(y=>y.GroupId));

            CreateMap<User,UserViewModel>()
                    .ForPath(x => x.Id , x => x.MapFrom(y => y.Id))
                    .ForPath(x => x.Email , x => x.MapFrom(y => y.Email))
                    .ForPath(x => x.IsVerified, x => x.MapFrom(y => y.IsVerified));
        }
    }
}