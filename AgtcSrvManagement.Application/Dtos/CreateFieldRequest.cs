using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgtcSrvManagement.Application.Dtos;

public class CreateFieldRequest
{
    public string Name { get; set; }
    public string CropType { get; set; }
    public double Area { get; set; }
}
