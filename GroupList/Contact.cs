using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GroupList;

/// <summary>
/// The Contact class is adapted from the ListView and GridView Microsoft sample and was used here as an excellent source
/// of sample data that we can put into an alphabetically-grouped SemanticZoom control.  This is a great sample to learn
/// the basics of ListView and GridView controls.
/// https://docs.microsoft.com/en-us/windows/uwp/controls-and-patterns/listview-and-gridview
/// </summary>
namespace GroupList.Model
{
    public class Contact
    {
        private static Random random = new Random();

        #region Properties
        private string initials;
        public string Initials
        {
            get
            {
                if (initials == string.Empty && FirstName != string.Empty && LastName != string.Empty)
                {
                    initials = FirstName[0].ToString() + LastName[0].ToString();
                }
                return initials;
            }
        }
        private string name;
        public string Name
        {
            get
            {
                if (name == string.Empty && FirstName != string.Empty && LastName != string.Empty)
                {
                    name = FirstName + " " + LastName;
                }
                return name;
            }
        }
        private string lastName;
        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                lastName = value;
                initials = string.Empty; // force to recalculate the value 
                name = string.Empty; // force to recalculate the value 
            }
        }
        private string firstName;
        public string FirstName
        {
            get
            {
                return firstName;
            }
            set
            {
                firstName = value;
                initials = string.Empty; // force to recalculate the value 
                name = string.Empty; // force to recalculate the value 
            }
        }
        public string Position { get; set; }
        public string PhoneNumber { get; set; }
        public string Biography { get; set; }
        #endregion

        public Contact()
        {
            // default values for each property.
            initials = string.Empty;
            name = string.Empty;
            LastName = string.Empty;
            FirstName = string.Empty;
            Position = string.Empty;
            PhoneNumber = string.Empty;
            Biography = string.Empty;
        }
     
        #region Public Methods
		/// <summary>
		/// Gets a new contact based on randomly-generated data.
		/// </summary>
		/// <returns>A single randomly-generated Contact object.<returns>
        public static Contact GetNewContact()
        {
            return new Contact()
            {
                FirstName = GenerateFirstName(),
                LastName = GenerateLastName(),
                Biography = GetBiography(),
                PhoneNumber = GeneratePhoneNumber(),
                Position = GeneratePosition(),
            };
        }

		/// <summary>
		/// Randomly generates an ObservableCollection of Contact objects for sample use.
		/// </summary>
		/// <param name="numberOfContacts"></param>
		/// <returns>Returns an ObservableCollection of randomly-generated contacts.</returns>
		public static ObservableCollection<Contact> GetContacts(int numberOfContacts)
        {
            ObservableCollection<Contact> contacts = new ObservableCollection<Contact>();

            for (int i = 0; i < numberOfContacts; i++)
            {
                contacts.Add(GetNewContact());
            }
            return contacts;
        }
		
		/// <summary>
		/// Calls GetContacts(int) to generate a list of randomly constructed Contact objects and groups
		/// them into an ObservableCollection of GroupInfoList objects for binding in the ZoomedInView and
		/// ZoomedOutView of the SemanticZoom control.  This function is not called in this demo, but is provided
		/// as an example.  It will only generate GroupInfoList objects with Contact members and will not make any Empty
		/// GroupInfoList objects.  Call this if you don't want to have Empty GroupInfoList members (maybe...remember that the Contact
		/// objects are generated randomly so there might be a Contact for every Key/letter), and then your ZoomedOutView
		/// will only have GroupInfoList objects representing those Keys (letters) with Contacts in them.  
		/// </summary>
		/// <param name="numberOfContacts"></param>
		/// <returns>An ObservableCollection of GroupInfoList objects containing Contact objects.</returns>
        public static ObservableCollection<GroupInfoList> GetContactsGrouped(int numberOfContacts)
        {
            ObservableCollection<GroupInfoList> groups = new ObservableCollection<GroupInfoList>();

			// This Linq expression calls GetContacts for a bunch of randomly-generated Contact objects,
			// groups them by the first letter of the LastName of each Contact, orders them alphabetically, and makes an 
			// anonymous type object containing the GroupName (key) and the list of items (Contacts).
            var query = from item in GetContacts(numberOfContacts)
                        group item by item.LastName[0] into g
                        orderby g.Key
                        select new { GroupName = g.Key, Items = g };

			// This iterates through the query results and makes a GroupInfoList object for each
			// group of Contacts represented by a letter.  
            foreach (var g in query)
			{ 
                GroupInfoList info = new GroupInfoList();

				// set the Key property of the GroupInfoList to the GroupName of the anonymous type object 
				// generated in the Linq query above
                info.Key = g.GroupName.ToString(); 
				
				// iterate through all the Contact items in the Linq query and add them
				// to the GroupInfoList object derived from List<T>
                foreach (var item in g.Items)
                {
                    info.Add(item);
                }

				// add this GroupInfoList to the ObservableCollection<GroupInfoList> that we will return
                groups.Add(info);
            }

            return groups;
        }

		/// <summary>
		/// Like GetContactsGrouped, this generates an ObservableCollection of GroupInfoLists, but does it a bit
		/// differently. For Contact groups that have no Contact data members, this will generate an empty GroupInfoList
		/// object representing the letter (or number), and allow you to display a ZoomedOutView just like the Contacts list
		/// on Windows Phone, where empty groups with no Contacts are grayed out and not selectable.
		/// </summary>
		/// <param name="numberOfContacts"></param>
		/// <returns>An ObservableCollection of GroupInfoList objects containing Contact objects.</returns>
		public static ObservableCollection<GroupInfoList> GetContactsGroupedAllAlpha(int numberOfContacts)
        {
            ObservableCollection<GroupInfoList> groups = new ObservableCollection<GroupInfoList>();

			// get an ObservableCollection of Contact objects (it could also be a List<Contact>).
            var generatedContacts = GetContacts(numberOfContacts);

			// the letters/numbers representing our GroupInfoList Keys 
            var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToList();

			// This Linq expression creates an IEnumerable collection of anonymous types containing the letter/number
			// Key of the group, and its associated Contact objects by iterating through the List of letters/numbers
			// and getting the Contact objects where the LastName property starts with that letter/number, ordering them
			// by LastName.
            var groupByAlpha = from letter in letters
                               select new
                               {
                                   Key = letter.ToString(),

                                   query = from item in generatedContacts
                                           where item.LastName.StartsWith(letter.ToString(), StringComparison.CurrentCultureIgnoreCase)
                                           orderby item.LastName
                                           select item
                               };

			// Now, we create the GroupInfoList objects and add them to our returned ObservableCollection.

			// iterate through the IEnumerable 'groupByAlpha' created above
            foreach (var g in groupByAlpha)
            {
				// make a new GroupInfoList object
                GroupInfoList info = new GroupInfoList();

				// assign its key (letter/number)
                info.Key = g.Key;

				// iterate through all the Contact objects for that Key and add them to the GroupInfoList.  If the
				// Key has no Contacts, then none will be added and that GroupInfoList will be empty.
                foreach(var item in g.query)
                {
                    info.Add(item);
                }

				// add the GroupInfoList to the ObservableCollection to be returned
                groups.Add(info);
            }

            return groups;
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion

		// this stuff generates the Contact objects from the hard-coded data below.  Thanks, Microsoft.
        #region Helpers
        private static string GeneratePosition()
        {
            List<string> positions = new List<string>() { "Program Manager", "Developer", "Product Manager", "Evangelist" };
            return positions[random.Next(0, positions.Count)];
        }
        private static string GetBiography()
        {
            List<string> biographies = new List<string>()
            {
                @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer id facilisis lectus. Cras nec convallis ante, quis pulvinar tellus. Integer dictum accumsan pulvinar. Pellentesque eget enim sodales sapien vestibulum consequat.",
                @"Maecenas eu sapien ac urna aliquam dictum.",
                @"Nullam eget mattis metus. Donec pharetra, tellus in mattis tincidunt, magna ipsum gravida nibh, vitae lobortis ante odio vel quam.",
                @"Quisque accumsan pretium ligula in faucibus. Mauris sollicitudin augue vitae lorem cursus condimentum quis ac mauris. Pellentesque quis turpis non nunc pretium sagittis. Nulla facilisi. Maecenas eu lectus ante. Proin eleifend vel lectus non tincidunt. Fusce condimentum luctus nisi, in elementum ante tincidunt nec.",
                @"Aenean in nisl at elit venenatis blandit ut vitae lectus. Praesent in sollicitudin nunc. Pellentesque justo augue, pretium at sem lacinia, scelerisque semper erat. Ut cursus tortor at metus lacinia dapibus.",
                @"Ut consequat magna luctus justo egestas vehicula. Integer pharetra risus libero, et posuere justo mattis et.",
                @"Proin malesuada, libero vitae aliquam venenatis, diam est faucibus felis, vitae efficitur erat nunc non mauris. Suspendisse at sodales erat.",
                @"Aenean vulputate, turpis non tincidunt ornare, metus est sagittis erat, id lobortis orci odio eget quam. Suspendisse ex purus, lobortis quis suscipit a, volutpat vitae turpis.",
                @"Duis facilisis, quam ut laoreet commodo, elit ex aliquet massa, non varius tellus lectus et nunc. Donec vitae risus ut ante pretium semper. Phasellus consectetur volutpat orci, eu dapibus turpis. Fusce varius sapien eu mattis pharetra.",
                @"Nam vulputate eu erat ornare blandit. Proin eget lacinia erat. Praesent nisl lectus, pretium eget leo et, dapibus dapibus velit. Integer at bibendum mi, et fringilla sem."
            };
            return biographies[random.Next(0, biographies.Count)];
        }

        private static string GeneratePhoneNumber()
        {
            return string.Format("{0:(###)} {1:###}-{2:####}", random.Next(100, 999), random.Next(100, 999), random.Next(1000, 9999));
        }
        private static string GenerateFirstName()
        {
            List<string> names = new List<string>() { "Lilly", "Mukhtar", "Sophie", "Femke", "Abdul-Rafi'", "Chirag-ud-D...", "Mariana", "Aarif", "Sara", "Ibadah", "Fakhr", "Ilene", "Sardar", "Hanna", "Julie", "Iain", "Natalia", "Henrik", "Rasa", "Quentin", "Gadi", "Pernille", "Ishtar", "Jimme", "Justina", "Lale", "Elize", "Rand", "Roshanara", "Rajab", "Bijou", "Marcus", "Marcus", "Alima", "Francisco", "Thaqib", "Andreas", "Mariana", "Amalie", "Rodney", "Dena", "Fadl", "Ammar", "Anna", "Nasreen", "Reem", "Tomas", "Filipa", "Frank", "Bari'ah", "Parvaiz", "Jibran", "Tomas", "Elli", "Carlos", "Diego", "Henrik", "Aruna", "Vahid", "Eliana", "Roxane", "Amanda", "Ingrid", "Wander", "Malika", "Basim", "Eisa", "Alina", "Andreas", "Deeba", "Diya", "Parveen", "Bakr", "Celine", "Bakr", "Marcus", "Daniel", "Mathea", "Edmee", "Hedda", "Maria", "Maja", "Alhasan", "Alina", "Hedda", "Victor", "Aaftab", "Guilherme", "Maria", "Kai", "Sabien", "Abdel", "Fadl", "Bahaar", "Vasco", "Jibran", "Parsa", "Catalina", "Fouad", "Colette" };
            return names[random.Next(0, names.Count)];
        }
        private static string GenerateLastName()
        {
            List<string> lastnames = new List<string>() { "Carlson", "Attia", "Quint", "Hollenberg", "Khoury", "Araujo", "Hakimi", "Seegers", "Abadi", "al", "Krommenhoek", "Siavashi", "Kvistad", "Sjo", "Vanderslik", "Fernandes", "Dehli", "Sheibani", "Laamers", "Batlouni", "Lyngvær", "Oveisi", "Veenhuizen", "Gardenier", "Siavashi", "Mutlu", "Karzai", "Mousavi", "Natsheh", "Seegers", "Nevland", "Lægreid", "Bishara", "Cunha", "Hotaki", "Kyvik", "Cardoso", "Pilskog", "Pennekamp", "Nuijten", "Bettar", "Borsboom", "Skistad", "Asef", "Sayegh", "Sousa", "Medeiros", "Kregel", "Shamoun", "Behzadi", "Kuzbari", "Ferreira", "Van", "Barros", "Fernandes", "Formo", "Nolet", "Shahrestaani", "Correla", "Amiri", "Sousa", "Fretheim", "Van", "Hamade", "Baba", "Mustafa", "Bishara", "Formo", "Hemmati", "Nader", "Hatami", "Natsheh", "Langen", "Maloof", "Berger", "Ostrem", "Bardsen", "Kramer", "Bekken", "Salcedo", "Holter", "Nader", "Bettar", "Georgsen", "Cunha", "Zardooz", "Araujo", "Batalha", "Antunes", "Vanderhoorn", "Nader", "Abadi", "Siavashi", "Montes", "Sherzai", "Vanderschans", "Neves", "Sarraf", "Kuiters" };
            return lastnames[random.Next(0, lastnames.Count)];
        }
        #endregion
    }
}
