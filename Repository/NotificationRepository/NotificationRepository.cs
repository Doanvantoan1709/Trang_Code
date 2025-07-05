using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using BaseProject.Model.Entities;
using Repository.Common;


namespace BaseProject.Repository.NotificationRepository
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(DbContext context) : base(context)
        {
        }
    }
}
