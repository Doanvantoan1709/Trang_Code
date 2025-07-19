using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseProject.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Common;

namespace BaseProject.Repository.EmailCheckRepository
{
    public class EmailCheckRepository : Repository<incoming_email>, IEmailCheckRepository
    {
        public EmailCheckRepository(DbContext context) : base(context)
        {
        }
    }
}
