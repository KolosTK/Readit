using System.ComponentModel.DataAnnotations;

namespace Readit.Models;

public enum ReadingStatus
{
    [Display(Name = "Not Started")]
    NotStarted,
    
    [Display(Name = "Reading")]
    Reading,
    
    [Display(Name = "Completed")]
    Completed,
    
    [Display(Name = "On Hold")]
    OnHold,
    
    [Display(Name = "Dropped")]
    Dropped,
    
    [Display(Name = "Planned")]
    Planned
}