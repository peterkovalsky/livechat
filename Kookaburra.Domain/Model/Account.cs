using Kookaburra.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kookaburra.Domain.Model
{
    public class Account
    {
        public int Id { get; set; }

        [StringLength(500)]
        public string Website { get; set; }

        public bool IsTrial { get; set; }

        public DateTime SignUpDate { get; set; }

        public int TrialPeriodDays { get; set; }

        [StringLength(50)]
        public string Key { get; set; }

        public virtual ICollection<Operator> Operators { get; set; }

        public virtual ICollection<Visitor> Visitors { get; set; }


        [NotMapped]
        public DateTime TrialExpiryDate
        {
            get
            {
                return SignUpDate.AddDays(TrialPeriodDays);
            }
        }

        [NotMapped]
        public int TrialDaysLeft
        {
            get
            {
                if (!IsTrial) return 0;

                if (TrialExpiryDate >= DateTime.UtcNow)
                {
                    return (TrialExpiryDate - DateTime.UtcNow).Days;
                }

                return 0;
            }
        }

        [NotMapped]
        public AccountStatusType AccountStatus
        {
            get
            {
                // trial
                if (IsTrial)
                {
                    if (TrialExpiryDate >= DateTime.UtcNow)
                    {
                        return AccountStatusType.Trial;
                    }
                    else
                    {
                        return AccountStatusType.TrialExpired;
                    }
                }
                else // paid
                {
                    return AccountStatusType.Paid;
                }
            }
        }
    }
}