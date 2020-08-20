using System;
using System.Collections.Generic;
using System.Text;

namespace Dinner.Base.Model
{
    public interface IEntity<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
    }
}
