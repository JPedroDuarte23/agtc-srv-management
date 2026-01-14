using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace AgtcSrvManagement.Domain.Models;

public class Property
{
    public Guid Id { get; set; }
    public string Name { get; set; }       
    public string Location { get; set; }   
    public double TotalArea { get; set; }
    public Guid OwnerId { get; set; }
    public List<Field> Fields { get; set; } = new();
}
