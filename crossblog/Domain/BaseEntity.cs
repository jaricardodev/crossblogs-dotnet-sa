using System;

namespace crossblog.Domain
{
    public abstract class BaseEntity
    {
        public int Id  { get; set; }

        public DateTime Created_At { get; set; }

        public DateTime Updated_At { get; set; }
    }
}