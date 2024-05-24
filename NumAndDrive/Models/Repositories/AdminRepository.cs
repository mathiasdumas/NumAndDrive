using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace NumAndDrive.Models.Repositories
{
	public class AdminRepository : IAdminRepository
	{
        public async Task UploadUsersFromCSVFile(Admin admin, UserManager<User> userManager)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../wwwroot/data/");
            string filePath = path + admin.File.FileName;

            CreateTemporaryFolder(path);

            CreateTemporaryFile(filePath, admin);

            List<User> usersToAdd = ReadCSVFile(filePath);

            DeleteTemporaryFolder(path);

            await UploadUsers(usersToAdd, userManager);
        }

        public void CreateTemporaryFolder(string path)
		{
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public void DeleteTemporaryFolder(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        } 

        public void CreateTemporaryFile(string path, Admin admin)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                admin.File.CopyTo(stream);
            }
        }

        public List<User> ReadCSVFile(string path)
        {
            StreamReader streamReader = new StreamReader(path);
            string line;
            string[] values;
            line = streamReader.ReadLine(); // Skip the first line
            List<User> usersToAdd = new List<User>();
            while (!streamReader.EndOfStream)
            {
                line = streamReader.ReadLine();
                values = line.Split(';');
                string lastName = values[0];
                string firstName = values[1];
                string profilePicPath = values[2];
                string mail = values[3];
                string phoneNumber = values[4];
                int departmentId = int.Parse(values[5]);
                int userTypeId = int.Parse(values[6]);
                int statusId = int.Parse(values[7]);

                User user = new User()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    ArchiveDate = null,
                    ProfilePicturePath = profilePicPath,
                    Email = mail,
                    PhoneNumber = phoneNumber,
                    DepartmentId = departmentId,
                    UserTypeId = userTypeId,
                    StatusId = statusId,
                    AccessFailedCount = 0,
                    LockoutEnabled = false,
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    EmailConfirmed = false,
                    UserName = mail
                };
                usersToAdd.Add(user);
            }
            streamReader.Close();
            return usersToAdd;
        }

        public async 
        Task
UploadUsers(List<User> users, UserManager<User> userManager)
        {
            foreach (User user in users)
            {
                await userManager.CreateAsync(user, PasswordGenerator());
            }
        }

        public string PasswordGenerator()
        {
            int length = 10;
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";
            var random = new Random();
            string password = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }
    }
}

