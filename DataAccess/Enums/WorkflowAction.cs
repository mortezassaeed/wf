using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Enums;
public enum WorkflowAction
{
    Submit = 1,
    Approve = 2,
    Reject = 3,
    Cancel = 4,
    RequestMoreInfo = 5,
    ProvideInfo = 6,
    Revise = 7,
    Complete = 8
}