﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Domain.Enums
{
    public enum AttendanceStatus
    {
        Present = 1,
        Absent = 2,
        Late = 3,
        HalfDay = 4,
        Holiday = 5,
        Leave = 6
    }
}
