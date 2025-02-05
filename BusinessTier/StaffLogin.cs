using System.Data;

namespace MailroomManagement;

public class StaffLogin{
    public string StaffUsername {get; set;} = string.Empty;
    public string StaffPassword {get; set;} = string.Empty;

    public Resident SearchResident(string name){
        DataTier residentDatabase = new DataTier();
        return residentDatabase.CheckForResident(name);
    }
    public void AddToPending(Package package){
        DataTier packageDatabase = new DataTier();
        package.status = "Pending";
        packageDatabase.AddPackage(package);
    }
    public void AddToUnknown(Package package){
        DataTier packageDatabase = new DataTier();
        package.status = "Unknown";
        packageDatabase.AddPackage(package);
    }
    public void PackagePickup(int id){
        DataTier database = new DataTier();
        database.UpdatePackageStatus(id);
    }
}