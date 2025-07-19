using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseProject.Model.Entities;
using BaseProject.Repository.EmailCheckRepository;
using BaseProject.Service.Common;
using BaseProject.Service.EmailCheckService.Dto;
using BaseProject.Service.EmailCheckService.ViewModels;
using Repository.Common;
using Service.Common;

namespace BaseProject.Service.EmailCheckService
{
    public class EmailCheckService : Service<incoming_email>, IEmailCheckService
    {
        private readonly IEmailCheckRepository _emailCheckRepository;

        public EmailCheckService(IEmailCheckRepository emailCheckRepository) : base(emailCheckRepository)
        {
            _emailCheckRepository = emailCheckRepository;
        }

        public async Task<PagedList<EmailCheckDto>> GetData(EmailCheckSearch search)
        {
            var query = from q in GetQueryable()
                        select new EmailCheckDto()
                        {
                            category = q.category,
                            content = q.content,
                            created_at = q.created_at,
                            from_email = q.from_email,
                            id = q.id,
                            received_time = q.received_time,
                            suspicious_indices = q.suspicious_indices,
                            title = q.title,
                            to_email = q.to_email
                        };

       

            if (query != null)
            {

                if (!string.IsNullOrEmpty(search.from_email))
                    query = query.Where(x => x.from_email.ToLower().Contains(search.from_email.ToLower()));

                if (!string.IsNullOrEmpty(search.content))
                    query = query.Where(x => x.content.ToLower().Contains(search.content.ToLower()));
                if (!string.IsNullOrEmpty(search.from_email))
                    query = query.Where(x => x.from_email.ToLower().Contains(search.from_email.ToLower()));
                if (!string.IsNullOrEmpty(search.suspicious_indices))
                    query = query.Where(x => x.suspicious_indices.ToLower().Contains(search.suspicious_indices.ToLower()));
                if (!string.IsNullOrEmpty(search.title))
                    query = query.Where(x => x.title.ToLower().Contains(search.title.ToLower()));
                if (!string.IsNullOrEmpty(search.category))
                    query = query.Where(x => x.category.ToLower().Contains(search.category.ToLower()));

            }
            query = query.OrderByDescending(x => x.created_at);

            var result = await PagedList<EmailCheckDto>.CreateAsync(query, search);
            return result;

        }

        public Task<incoming_email> GetDto(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
