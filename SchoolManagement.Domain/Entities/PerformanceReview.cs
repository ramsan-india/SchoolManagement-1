﻿using SchoolManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Domain.Entities
{
    public class PerformanceReview : BaseEntity
    {
        public Guid EmployeeId { get; private set; }
        public Guid ReviewerId { get; private set; }
        public DateTime ReviewPeriodStart { get; private set; }
        public DateTime ReviewPeriodEnd { get; private set; }
        public decimal OverallRating { get; private set; }
        public string Goals { get; private set; }
        public string Achievements { get; private set; }
        public string AreasOfImprovement { get; private set; }
        public string ReviewerComments { get; private set; }
        public string EmployeeComments { get; private set; }
        public ReviewStatus Status { get; private set; }

        // Navigation Properties
        public virtual Employee Employee { get; private set; }
        public virtual Employee Reviewer { get; private set; }

        private PerformanceReview() { }

        public PerformanceReview(Guid employeeId, Guid reviewerId, DateTime periodStart,
                               DateTime periodEnd)
        {
            EmployeeId = employeeId;
            ReviewerId = reviewerId;
            ReviewPeriodStart = periodStart;
            ReviewPeriodEnd = periodEnd;
            Status = ReviewStatus.Draft;
        }

        public void UpdateReview(decimal overallRating, string goals, string achievements,
                               string areasOfImprovement, string reviewerComments)
        {
            OverallRating = overallRating;
            Goals = goals;
            Achievements = achievements;
            AreasOfImprovement = areasOfImprovement;
            ReviewerComments = reviewerComments;
        }

        public void SubmitReview()
        {
            Status = ReviewStatus.Submitted;
        }

        public void AddEmployeeComments(string comments)
        {
            EmployeeComments = comments;
            Status = ReviewStatus.Acknowledged;
        }
    }
}
