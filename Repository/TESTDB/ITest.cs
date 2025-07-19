using BaseProject.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Repository.TESTDB
{
    public interface ITest
    {
        Task<List<Incoming_emails>> GetAllAsync();
    }
}
