//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WhiskyClub.DataAccessLayer.Entities
{
    using System;
    using System.Collections.Generic;
    
    internal partial class Event
    {
    	public int EventId { get; set; }
    	public int HostId { get; set; }
    	public string Description { get; set; }
    	public System.DateTime HostedDate { get; set; }
    	public byte[] Version { get; set; }
    	public System.DateTime InsertedDate { get; set; }
    	public System.DateTime UpdatedDate { get; set; }
    
    	public virtual Host Host { get; set; }
    }
}
