using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgtcSrvManagement.Domain.Models;

public class Field
{
    public Guid FieldId { get; set; }
    public string Name { get; set; }
    public string CropType { get; set; }
    public double Area { get; set; }
}
