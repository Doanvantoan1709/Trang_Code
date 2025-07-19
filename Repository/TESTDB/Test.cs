using BaseProject.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Repository.TESTDB
{
    public class Test : ITest
    {
        private readonly BaseProjectContext _dbContext;

        public Test(BaseProjectContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Incoming_emails>> GetAllAsync()
        {
            return await _dbContext.incoming_emails
                .Take(20)
                .ToListAsync();
        }
    }
}
