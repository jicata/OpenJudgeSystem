﻿namespace OJS.Web.Areas.Contests.ViewModels.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ParticipantResultViewModel
    {
        public ParticipantResultViewModel()
        {
            this.ProblemResults = new List<ProblemResultPairViewModel>();
        }

        public string ParticipantUsername { get; set; }

        public string ParticipantFirstName { get; set; }

        public string ParticipantLastName { get; set; }

        public string ParticipantFullName => $"{this.ParticipantFirstName} {this.ParticipantLastName}".Trim();

        public IEnumerable<ProblemResultPairViewModel> ProblemResults { get; set; }

        public int Total
        {
            get
            {
                return this.ProblemResults.Where(x => x.ShowResult).Sum(x => x.BestSubmission?.Points ?? 0);
            }
        }

        public int AdminTotal
        {
            get
            {
                return this.ProblemResults.Sum(x => x.BestSubmission?.Points ?? 0);
            }
        }

        public double? GetContestTimeInSeconds(DateTime? contestStartTime)
        {
            if (contestStartTime.HasValue)
            {
                var lastSubmission = this.ProblemResults
                    .Where(x => x.ShowResult && x.BestSubmission != null)
                    .OrderByDescending(x => x.BestSubmission.CreatedOn)
                    .Select(x => x.BestSubmission)
                    .FirstOrDefault();

                if (lastSubmission != null)
                {
                    var lastSubmissionTime = lastSubmission.CreatedOn;
                    var contestTimeInSeconds = (lastSubmissionTime - contestStartTime.Value).TotalSeconds;
                    return contestTimeInSeconds;
                }
            }

            return null;
        }
    }
}
