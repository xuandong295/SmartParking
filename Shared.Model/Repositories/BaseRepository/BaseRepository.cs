using Microsoft.EntityFrameworkCore;
using Shared.Model.Entities.EF;
using Shared.Model.Entities.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.Model.Repositories.BaseRepository.IBaseRepository;

namespace Shared.Model.Repositories.BaseRepository
{
    public class BaseRepository<T> : IBaseRepository
    {
        public DataContext DataContext;
        //public ILoggerFactory DataContext;
        public BaseRepository(DataContext dataContext)
        {
            DataContext = dataContext;
        }
       
    }
}
