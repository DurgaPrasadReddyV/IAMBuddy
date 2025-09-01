using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD_Provisioning_Demo.Steps.Events
{
    public class MailServiceEvents
    {
        public static readonly string SimpleMessageMailSent = nameof(SimpleMessageMailSent);
        public static readonly string ApprovalsMailSent = nameof(ApprovalsMailSent);
    }
}
