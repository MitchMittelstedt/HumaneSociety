using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////
   
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation)
            {
                case "delete":
                    RemoveEmployee(employee);
                    break;
                case "update":
                    UpdateEmployee(employee);
                    break;
                case "read":
                    GetEmployeeByID(employee.EmployeeId);
                    break;
                case "create":
                    AddEmployee(employee);
                    break;
                default:
                    break;
            }
        }

        internal static void AddEmployee(Employee employee)  //create
        {
            db.Employees.InsertOnSubmit(employee);
            db.SubmitChanges();
        }

        internal static void GetEmployeeByID(int id)  //read
        {
            db.Employees.Where(c => c.EmployeeId == id).Single();
        }

        internal static void UpdateEmployee(Employee employee)  //update
        {
            Employee employeeFromDb = db.Employees.Where(c => c.EmployeeId == employee.EmployeeId).Single();
            employeeFromDb.FirstName = employee.FirstName;
            employeeFromDb.LastName = employee.LastName;
            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;
            employeeFromDb.EmployeeNumber = employee.EmployeeNumber;
            employeeFromDb.Email = employee.Email;

        }

        internal static void RemoveEmployee(Employee employee)  //delete
        {
            db.Employees.DeleteOnSubmit(employee);
        }

        //add, read, 

        // TODO: Animal CRUD Operations


        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
        }

        internal static Animal GetAnimalByID(int id)
        {
            Animal animal = db.Animals.Where(c => c.AnimalId == id).Single();
            return animal;

        }

        internal static void UpdateAnimal(Animal animal, Dictionary<int, string> updates)//
        {
            Animal animalFromDb = db.Animals.Where(c => c.AnimalId == animal.AnimalId).Single();

            var update = from entry in updates
                         select entry;

            foreach (var entry in updates)
            {
                switch (entry.Key)
                {
                    case 1:
                        animalFromDb.CategoryId = db.Categories.Where(c => c.Name == entry.Value).Select(p => p.CategoryId).Single();
                        break;
                    case 2:
                        animalFromDb.Name = entry.Value;
                        break;
                    case 3:
                        animalFromDb.Age = int.Parse(entry.Value);
                        break;
                    case 4:
                        animalFromDb.Demeanor = entry.Value;
                        break;
                    case 5:
                        animalFromDb.KidFriendly = bool.Parse(entry.Value);
                        break;
                    case 6:
                        animalFromDb.PetFriendly = bool.Parse(entry.Value);
                        break;
                    case 7:
                        animalFromDb.Weight = int.Parse(entry.Value);
                        break;
                    case 8:
                        animalFromDb.AnimalId = int.Parse(entry.Value);
                        break;
                    default:
                        break;
                }
            }
            db.SubmitChanges();
        }                                          

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
        }

        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> animalsToSearch = db.Animals;
            animalsToSearch = animalsToSearch.Where(a => updates.Values.Equals(db.Categories.Where(c => c.Name == updates.Values.ElementAt(0)).Select(p => p.CategoryId).Single()) && updates.Values.Equals(a.Name) && updates.Values.Equals(a.Weight.ToString()) && updates.Values.Equals(a.Age.ToString()) && updates.Values.Equals(a.Demeanor) && updates.Values.Equals(a.KidFriendly.ToString()) && updates.Values.Equals(a.PetFriendly.ToString()) && updates.Values.Equals(a.Weight.ToString()) && updates.Values.Equals(a.AnimalId.ToString())).AsQueryable();
            return animalsToSearch;
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            var categoryId = db.Categories.Where(c => c.Name == categoryName).Select(c => c.CategoryId).Single();
            return categoryId;
        }
        
        internal static Room GetRoom(int animalId)
        {
            Room room = db.Rooms.Where(r => r.AnimalId == animalId).Single();
            return room;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            var dietPlanId = db.DietPlans.Where(d => d.Name == dietPlanName).Select(n => n.DietPlanId).Single();
            return dietPlanId;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoption = new Adoption(); //why Adoption not Adoptions ask
            adoption.ClientId = client.ClientId;
            adoption.AnimalId = animal.AnimalId;
            adoption.PaymentCollected = true;
            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            IQueryable <Adoption> pendingAdoptions = db.Adoptions;
            return pendingAdoptions;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            var adoptionToUpdate = db.Adoptions.Where(a => a.AnimalId == adoption.AnimalId).Single();
            if (isAdopted == true)
            {
                adoptionToUpdate.ApprovalStatus = "Adopted";
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            var adoptionToRemove = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).Single();
            db.Adoptions.DeleteOnSubmit(adoptionToRemove);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            var animalShots = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId);
            return animalShots;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}