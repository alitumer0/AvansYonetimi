using AutoMapper;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.ViewModels;

namespace VarlikYönetimi.Mapping.Mapping
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            // User Mapping
            CreateMap<User, UserViewModel>().ReverseMap();
            CreateMap<User, UserCreateViewModel>().ReverseMap();
            CreateMap<User, UserEditViewModel>().ReverseMap();
            CreateMap<User, LoginViewModel>().ReverseMap();
            CreateMap<User, RegisterViewModel>().ReverseMap();

            // Role Mapping
            CreateMap<Role, RoleViewModel>().ReverseMap();
            CreateMap<Role, RoleCreateViewModel>().ReverseMap();
            CreateMap<Role, RoleEditViewModel>().ReverseMap();

            // Project Mapping
            CreateMap<Project, ProjectViewModel>().ReverseMap();
            CreateMap<Project, ProjectCreateViewModel>().ReverseMap();
            CreateMap<Project, ProjectEditViewModel>().ReverseMap();

            // AdvanceRequest Mapping
            CreateMap<AdvanceRequest, AdvanceRequestViewModel>().ReverseMap();
            CreateMap<AdvanceRequest, AdvanceRequestCreateViewModel>().ReverseMap();
            CreateMap<AdvanceRequest, AdvanceRequestEditViewModel>().ReverseMap();

            // ApprovalProcess Mapping
            CreateMap<ApprovalProcess, ApprovalProcessViewModel>().ReverseMap();
            CreateMap<ApprovalProcess, ApprovalProcessCreateViewModel>().ReverseMap();
            CreateMap<ApprovalProcess, ApprovalProcessEditViewModel>().ReverseMap();

            // Payment Mapping
            CreateMap<Payment, PaymentViewModel>().ReverseMap();
            CreateMap<Payment, PaymentCreateViewModel>().ReverseMap();
            CreateMap<Payment, PaymentEditViewModel>().ReverseMap();

            // Notification Mapping
            CreateMap<Notification, NotificationViewModel>().ReverseMap();
            CreateMap<Notification, NotificationCreateViewModel>().ReverseMap();
            CreateMap<Notification, NotificationEditViewModel>().ReverseMap();

            // ApprovalSettings Mapping
            CreateMap<ApprovalSettings, ApprovalSettingsViewModel>().ReverseMap();
            CreateMap<ApprovalSettings, ApprovalSettingsCreateViewModel>().ReverseMap();
            CreateMap<ApprovalSettings, ApprovalSettingsEditViewModel>().ReverseMap();

            // LegalAction Mapping
            CreateMap<LegalAction, LegalActionViewModel>().ReverseMap();
            CreateMap<LegalAction, LegalActionCreateViewModel>().ReverseMap();
            CreateMap<LegalAction, LegalActionEditViewModel>().ReverseMap();
        }
    }
}
