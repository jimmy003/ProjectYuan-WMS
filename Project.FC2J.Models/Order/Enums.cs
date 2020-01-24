using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.Models.Order
{
    public enum OrderStatusEnum
    {
        NEW=0,SUBMITTED=1,VALIDATED=2,CANCELLED=3,DELIVERED=4,DELIVERED_WITH_RETURNS=5,RETURNEDALL=6,PAID=7,PARTIAL=8,BADACCOUNTS=9
    }

    public enum PoStatusEnum
    {
        NEW = 0, SUBMITTED = 1, ACKNOWLEDGED = 2, DELIVERED = 3, DELIVERED_BUT_NOT_ALL = 4
    }
}
