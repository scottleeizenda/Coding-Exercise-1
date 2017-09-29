using System;

namespace IzendaCMS.DataModel.Models
{
    public interface IModificationHistory
    {
        DateTime DateModified { get; set; }
        DateTime DateCreated { get; set; }
    }
}
