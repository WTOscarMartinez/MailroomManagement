namespace MailroomManagement;

public class Package{
    public int ID {get; set;} 
    public string OwnerName {get; set;} = string.Empty;

    public string PostalAgency {get; set;} = string.Empty;

    public DateTime DeliveryDate {get; set;} 

    public string status {get; set;} = string.Empty;

}