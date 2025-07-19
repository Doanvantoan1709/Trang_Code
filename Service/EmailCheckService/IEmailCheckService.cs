using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseProject.Model.Entities;
using BaseProject.Service.Common;
using BaseProject.Service.EmailCheckService.Dto;
using BaseProject.Service.EmailCheckService.ViewModels;
using Service.Common;

namespace BaseProject.Service.EmailCheckService
{
    public interface IEmailCheckService :IService<incoming_email>
    {
        Task<PagedList<EmailCheckDto>> GetData(EmailCheckSearch search);
        Task<incoming_email> GetDto(Guid id);

    }
}
