using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    public class Event
{
   [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string date { get; set; }
    public string description { get; set; }
    public string lang { get; set; }
    public string category1 { get; set; }
    public string category2 { get; set; }
    public string granularity { get; set; }
}
}