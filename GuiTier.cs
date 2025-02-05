using System.Data;
using Google.Protobuf;
using MailroomManagement;
using MySql.Data.MySqlClient;
class GuiTier{
    StaffLogin staffLogin = new StaffLogin();
    Package package = new Package();
    Resident resident = new Resident();
    DataTier database = new DataTier();

    // print login page
    public StaffLogin Login(){
        Console.WriteLine("------Welcome to Mailroom Management System------");
        Console.WriteLine("Please input Staff username): ");
        staffLogin.StaffUsername = Console.ReadLine()!;
        Console.WriteLine("Please input password: ");
        staffLogin.StaffPassword = Console.ReadLine()!;
        return staffLogin;
    }
    // print Options after user logs in successfully
    public int Dashboard(StaffLogin staffLogin){
        Console.WriteLine("---------------Dashboard-------------------");
        Console.WriteLine($"Hello: {staffLogin.StaffUsername}");
        Console.WriteLine("Please select an option to continue:");
        Console.WriteLine("1. Search Resident");
        Console.WriteLine("2. Package Pickup");
        Console.WriteLine("3. Retrieve Record");
        Console.WriteLine("4. Log Out");
        int option = Convert.ToInt16(Console.ReadLine());
        return option;
    }

    // Search for resident
    public void SearchResidentUI(){
        resident = null!;
        Console.WriteLine("------Search------");
        Console.WriteLine("Please enter package owner: ");
        string searchName = Console.ReadLine()!;
        resident = staffLogin.SearchResident(searchName);
        if( resident != null ){
            package.OwnerName = resident.full_name;
            Console.WriteLine("Found owner. Enter Postal Agency(e.g., FedEx, USPS, UPS).:");
            package.PostalAgency = Console.ReadLine()!;
            package.DeliveryDate = DateTime.Now;
            staffLogin.AddToPending(package);
        }else{
            Console.WriteLine("No owner found. Add to UNKNOWN area (Y/N).");
            while(true){
                string ans = Console.ReadLine()!.ToLower();
                if("y" == ans){
                    Console.WriteLine("Enter owner's name:");
                    package.OwnerName = Console.ReadLine()!;
                    Console.WriteLine("Enter Postal Agency(e.g., FedEx, USPS, UPS).:");
                    package.PostalAgency = Console.ReadLine()!;
                    package.DeliveryDate = DateTime.Now;
                    staffLogin.AddToUnknown(package);
                    break;
                }else if("n" == ans){
                    break;
                }
            }
        }
    }

    // show packages returned from database
    public void DisplayPackagesUI(DataTable tablePackage){
        List<int> pendingPackagesIDs = new();

        Console.WriteLine("---------------Packages-------------------");
        foreach(DataRow row in tablePackage.Rows){
            if(row["_status"].ToString() == "Pending"){
                Console.WriteLine($"Package ID: {row["id"]} \t Owner Name: {row["owner_name"]} \t Dalivery Date:{row["delivery_date"]} \t Status: {row["_status"]}");
                pendingPackagesIDs.Add(Convert.ToInt32(row["id"]));
            }
        }
        Console.WriteLine("Enter package ID for pickup:");
        // Declare ans as a nullable integer to store the input value
        int ans;
        
        // Check if interger
        if (int.TryParse(Console.ReadLine()!, out ans))
        {
            // Check if ans is a pending package ID
            if (pendingPackagesIDs.Contains(ans))
            {
                Console.WriteLine("To Confirm Package Pickup, type \"confirm\":");
                if (Console.ReadLine()!.ToLower() == "confirm")
                {
                    staffLogin.PackagePickup(ans);
                    foreach(DataRow row in tablePackage.Rows){
                        if(Convert.ToInt32(row["id"]) == ans){
                            Resident pickupResident = database.CheckForResident(row["owner_name"].ToString()!);
                            EmailSender.SendEmail(
                                senderEmail: "noreply@buffteks.org",           // Sender's email address
                                password: "cidm4360fall2024@*",                // Sender's email password
                                toEmail: $"{pickupResident.email}",        // Recipient's email address
                                subject: "Package Pickup Notification"         // Subject of the email
                            );
                        }
                    }
                    Console.WriteLine("Pickup Confirmed. Back to menu.");
                }
                else
                {
                    Console.WriteLine("Back to menu.");
                }
            }
            else
            {
                Console.WriteLine("No pending package matches ID.");
            }
        }
        else
        {
            // Handle invalid input
            Console.WriteLine("Invalid package ID entered.");
        }
    }

    // show resident package records returned from database
    public void DisplayRecords(){
        resident = null!;

        //Get unit input
        Console.WriteLine("Enter an unit number:");
        string input = Console.ReadLine()!;
            
        //Try to parse the input into a valid integer
        if (!int.TryParse(input, out int unit))
        {
                Console.WriteLine("unit is not a valid number.");
        }

        //Get user input for name
        Console.WriteLine("Enter Resident Full Name:");
        string name = Console.ReadLine()!;
        
        //Check if name is not null or empty)
        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Input is not a name.");
        }         
        
        resident = database.CheckRecords(name, unit);

        DataTable tableRecords = database.CheckPackage();

        if(resident != null){
            Console.WriteLine("---------------Package History-------------------");
            foreach(DataRow row in tableRecords.Rows){
                if(row["owner_name"].ToString() == resident.full_name){
                    Console.WriteLine($"Package ID: {row["id"]} \t Full Name: {resident.full_name} \t Unit number: {resident.unit_number} \t Dalivery Date:{row["delivery_date"]} \t Status: {row["_status"]}");
                }
            }
        }
    }
}