using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Enums
{
    /// <summary>
    /// Same role set as Identity.Role lookup seeds / TeamMemberSearch TargetRoles:
    /// Programmer, Designer, QA, DevOps, PM, Data Analyst, Robotics Developer.
    /// </summary>
    public enum PrimaryRole
    {
        Programmer,
        Designer,
        QA,
        DevOps,
        PM,
        DataAnalyst,
        RoboticsDeveloper
    }
}
