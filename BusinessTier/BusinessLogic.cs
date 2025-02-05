namespace MailroomManagement;

using System.Data;
using MySql.Data.MySqlClient;
class BusinessLogic
{
   
    static void Main(string[] args)
    {
        bool _continue = true;
        StaffLogin staffLogin;
        GuiTier appGUI = new GuiTier();
        DataTier database = new DataTier();

        // start GUI
        staffLogin = appGUI.Login();

       
        if (database.LoginCheck(staffLogin)){

            while(_continue){
                int option  = appGUI.Dashboard(staffLogin);
                switch(option)
                {
                    //Search Residents
                    case 1:
                        appGUI.SearchResidentUI();
                        break;
                    //Package Pickup
                    case 2:
                        DataTable tablePackage = database.CheckPackage();
                        if(tablePackage != null)
                            appGUI.DisplayPackagesUI(tablePackage);
                        break;
                    //Retrieve Records
                    case 3:
                        appGUI.DisplayRecords();
                        break;
                    case 4:
                        _continue = false;
                        Console.WriteLine("Log out, Goodbye.");
                        break;
                    // default: wrong input
                    default:
                        Console.WriteLine("Wrong Input");
                        break;
                }

            }
        }
        else{
                Console.WriteLine("Login Failed, Goodbye.");
        }        
    }    
}

