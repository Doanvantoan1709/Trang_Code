using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseProject.Model.Entities;
using Repository.Common;

namespace BaseProject.Repository.EmailCheckRepository
{
    public interface IEmailCheckRepository :IRepository<incoming_email>
    {

    }
}
