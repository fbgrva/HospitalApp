using HospitalApp.DBContextHospital;
using HospitalApp.Entities;
using HospitalApp.Services.Abstracts;
using HospitalApp.Services.Concretes;
using HospitalApp.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection();

serviceCollection.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer("Data Source=AUCE;Initial Catalog=HospitalApp;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;TrustServerCertificate=True"));

var emailService = new EmailService(
    smtpServer: "smtp.gmail.com",
    smtpPort: 587,
    smtpUsername: "fatimebagirovaa27@gmail.com",
    smtpPassword: "qptd gkyg nhmo duhr"
);

var otpService = new OTPService();

serviceCollection.AddSingleton(emailService);
serviceCollection.AddSingleton(otpService);

serviceCollection.AddScoped<IUserService, UserService>();
serviceCollection.AddScoped<IPatientService, PatientService>();
serviceCollection.AddScoped<IDoctorService, DoctorService>();
serviceCollection.AddScoped<IDepartmentService, DepartmentService>();
serviceCollection.AddScoped<IAppointmentService, AppointmentService>();
serviceCollection.AddScoped<IMedicalRecordService, MedicalRecordService>();

var serviceProvider = serviceCollection.BuildServiceProvider();

try
{
    using (var scope = serviceProvider.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
        context.Database.Migrate();
    }
}
catch (Exception ex)
{
    return;
}

var userService = serviceProvider.GetService<IUserService>();
var patientService = serviceProvider.GetService<IPatientService>();
var doctorService = serviceProvider.GetService<IDoctorService>();
var departmentService = serviceProvider.GetService<IDepartmentService>();
var appointmentService = serviceProvider.GetService<IAppointmentService>();
var medicalRecordService = serviceProvider.GetService<IMedicalRecordService>();

Console.WriteLine("====================================");
Console.WriteLine("   HOSPITAL MANAGEMENT SYSTEM ");
Console.WriteLine("====================================");

User currentUser = null;

while (currentUser == null)
{
    Console.WriteLine("1. Qeydiyyat");
    Console.WriteLine("2. Girish");
    Console.WriteLine("3. Chixish");
    Console.Write("Seciminiz: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            currentUser = await RegisterUser(userService);
            break;
        case "2":
            currentUser = await LoginUser(userService);
            break;
        case "3":
            Console.WriteLine("Sistemden chixilir...");
            return;
        default:
            Console.WriteLine("Yanlish secim!");
            break;
    }
}

Console.WriteLine($"\n Xosh geldiniz, {currentUser.Name} {currentUser.Surname}!");

while (true)
{
    Console.WriteLine("\nESAS MENYU:");
    Console.WriteLine("1. Istifadechi Emeliyyatlari");
    Console.WriteLine("2. Hekim Emeliyyatlari");
    Console.WriteLine("3. Shobe Emeliyyatlari");
    Console.WriteLine("4. Randevu Emeliyyatlari");
    Console.WriteLine("5. Tibbi Qeyd Emeliyyatlari");
    Console.WriteLine("6. Chixish");
    Console.Write("Seciminiz: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            await UserOperationsMenu(userService, patientService, currentUser);
            break;
        case "2":
            await DoctorOperationsMenu(doctorService, departmentService, userService);
            break;
        case "3":
            await DepartmentOperationsMenu(departmentService);
            break;
        case "4":
            await AppointmentOperationsMenu(appointmentService, patientService, doctorService);
            break;
        case "5":
            await MedicalRecordOperationsMenu(medicalRecordService, patientService, doctorService);
            break;
        case "6":
            Console.WriteLine("Sistemden chixilir...");
            return;
        default:
            Console.WriteLine("Yanlish secim!");
            break;
    }
}

async Task UserOperationsMenu(IUserService userService, IPatientService patientService, User user)
{
    while (true)
    {
        Console.WriteLine("\n ISTIFADECHI EMELIYYATLARI:");
        Console.WriteLine("1. Xeste Qeydiyyati");
        Console.WriteLine("2. Shifreni deyish");
        Console.WriteLine("3. Profil melumatlari");
        Console.WriteLine("4. Geri");
        Console.Write("Seciminiz: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await RegisterPatient(patientService, userService);
                break;
            case "2":
                await ChangePassword(userService, user.Id);
                break;
            case "3":
                ShowUserProfile(user);
                break;
            case "4":
                return;
            default:
                Console.WriteLine("Yanlish secim!");
                break;
        }
    }
}

async Task DoctorOperationsMenu(IDoctorService doctorService, IDepartmentService departmentService, IUserService userService)
{
    while (true)
    {
        Console.WriteLine("\n HEKIM EMELIYYATLARI:");
        Console.WriteLine("1. Butun Hekimler");
        Console.WriteLine("2. Hekim Elave Et");
        Console.WriteLine("3. Hekim Sil");
        Console.WriteLine("4. Geri");
        Console.Write("Seciminiz: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await GetAllDoctors(doctorService);
                break;
            case "2":
                await AddDoctor(doctorService, departmentService, userService);
                break;
            case "3":
                await DeleteDoctor(doctorService);
                break;
            case "4":
                return;
            default:
                Console.WriteLine("Yanlish secim!");
                break;
        }
    }
}

async Task DepartmentOperationsMenu(IDepartmentService departmentService)
{
    while (true)
    {
        Console.WriteLine("\n SHOBE EMELIYYATLARI:");
        Console.WriteLine("1. Butun Shobeler");
        Console.WriteLine("2. Shobe Elave Et");
        Console.WriteLine("3. Shobe Sil");
        Console.WriteLine("4. Geri");
        Console.Write("Seciminiz: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await GetAllDepartments(departmentService);
                break;
            case "2":
                await AddDepartment(departmentService);
                break;
            case "3":
                await DeleteDepartment(departmentService);
                break;
            case "4":
                return;
            default:
                Console.WriteLine("Yanlish secim!");
                break;
        }
    }
}

async Task AppointmentOperationsMenu(IAppointmentService appointmentService, IPatientService patientService, IDoctorService doctorService)
{
    while (true)
    {
        Console.WriteLine("\n RANDEVU EMELIYYATLARI:");
        Console.WriteLine("1. Butun Randevular");
        Console.WriteLine("2. Randevu Elave Et");
        Console.WriteLine("3. Randevu Sil");
        Console.WriteLine("4. Geri");
        Console.Write("Seciminiz: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await GetAllAppointments(appointmentService);
                break;
            case "2":
                await AddAppointment(appointmentService, patientService, doctorService);
                break;
            case "3":
                await DeleteAppointment(appointmentService);
                break;
            case "4":
                return;
            default:
                Console.WriteLine("Yanlish secim!");
                break;
        }
    }
}

async Task MedicalRecordOperationsMenu(IMedicalRecordService medicalRecordService, IPatientService patientService, IDoctorService doctorService)
{
    while (true)
    {
        Console.WriteLine("\n TIBBI QEYD EMELIYYATLARI:");
        Console.WriteLine("1. Butun Tibbi Qeydler");
        Console.WriteLine("2. Tibbi Qeyd Elave Et");
        Console.WriteLine("3. Tibbi Qeyd Sil");
        Console.WriteLine("4. Geri");
        Console.Write("Seciminiz: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await GetAllMedicalRecords(medicalRecordService);
                break;
            case "2":
                await AddMedicalRecord(medicalRecordService, patientService, doctorService);
                break;
            case "3":
                await DeleteMedicalRecord(medicalRecordService);
                break;
            case "4":
                return;
            default:
                Console.WriteLine("Yanlish secim!");
                break;
        }
    }
}


async Task<User> RegisterUser(IUserService userService)
{
    try
    {
        Console.WriteLine("\n=== YENI ISTIFADECHI QEYDIYYATI ===");

        Console.Write("Ad: ");
        var name = Console.ReadLine();

        Console.Write("Soyad: ");
        var surname = Console.ReadLine();

        Console.Write("Istifadechi adi: ");
        var username = Console.ReadLine();

        Console.Write("Email: ");
        var email = Console.ReadLine();

        Console.Write("Shifre: ");
        var password = Console.ReadLine();

        var user = new User
        {
            Name = name,
            Surname = surname,
            Username = username,
            Email = email
        };

        var registeredUser = userService.Register(user, password);

        Console.WriteLine(" Qeydiyyat ugurla tamamlandi!");
        Console.WriteLine("OTP kodu: " + registeredUser.OTP);

        Console.Write("OTP Kodunu daxil edin: ");
        var otp = Console.ReadLine();

        if (userService.VerifyEmail(email, otp))
        {
            Console.WriteLine(" Email ugurla tesdiqlendi!");
            return registeredUser;
        }
        else
        {
            Console.WriteLine(" Yanlish OTP kodu!");
            return null;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Xeta: {ex.Message}");
        return null;
    }
}

async Task<User> LoginUser(IUserService userService)
{
    try
    {
        Console.WriteLine("\n=== SISTEME GIRISH ===");

        Console.Write("Istifadechi adi: ");
        var username = Console.ReadLine();

        Console.Write("Shifre: ");
        var password = Console.ReadLine();

        var user = userService.Login(username, password);

        Console.WriteLine($" Ugurla girish edildi!");
        return user;
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Xeta: {ex.Message}");
        return null;
    }
}

async Task ChangePassword(IUserService userService, int userId)
{
    try
    {
        Console.WriteLine("\n=== SHIFRE YENILEME ===");

        Console.Write("Kohne shifre: ");
        var oldPassword = Console.ReadLine();

        Console.Write("Yeni shifre: ");
        var newPassword = Console.ReadLine();

        if (userService.ChangePassword(userId, oldPassword, newPassword))
        {
            Console.WriteLine(" Shifre ugurla deyishdirildi!");
        }
        else
        {
            Console.WriteLine(" Shifre deyishdirilmedi!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Xeta: {ex.Message}");
    }
}

void ShowUserProfile(User user)
{
    Console.WriteLine("\n=== PROFIL MELUMLARI ===");
    Console.WriteLine($"Ad: {user.Name}");
    Console.WriteLine($"Soyad: {user.Surname}");
    Console.WriteLine($"Istifadechi adi: {user.Username}");
    Console.WriteLine($"Email: {user.Email}");
}

async Task RegisterPatient(IPatientService patientService, IUserService userService)
{
    try
    {
        Console.WriteLine("\n=== XESTE QEYDIYYATI ===");

        var users = userService.GetAll();
        Console.WriteLine("\nMövcud Istifadechiler:");
        foreach (var user in users)
        {
            Console.WriteLine($"ID: {user.Id}, Ad: {user.Name} {user.Surname}");
        }

        Console.Write("Istifadechi ID: ");
        int userId = int.Parse(Console.ReadLine());

        Console.Write("Unvan: ");
        var address = Console.ReadLine();

        Console.Write("Telefon: ");
        var phone = Console.ReadLine();

        Console.Write("Dogum tarixi (yyyy-mm-dd): ");
        var dob = DateTime.Parse(Console.ReadLine());

        Console.Write("Cinsiyyet: ");
        var gender = Console.ReadLine();

        var patient = new Patient
        {
            UserId = userId,
            Address = address,
            PhoneNumber = phone,
            DateOfBirth = dob,
            Gender = gender
        };

        var result = patientService.Add(patient);
        Console.WriteLine($" Xeste ugurla qeydiyyatdan kechdi! ID: {result.Id}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Xeta: {ex.Message}");
    }
}

async Task AddDoctor(IDoctorService doctorService, IDepartmentService departmentService, IUserService userService)
{
    try
    {
        Console.WriteLine("\n=== YENI HEKIM ===");

        var users = userService.GetAll();
        Console.WriteLine("\nMövcud Istifadechiler:");
        foreach (var user in users)
        {
            Console.WriteLine($"ID: {user.Id}, Ad: {user.Name} {user.Surname}");
        }

        Console.Write("Istifadechi ID: ");
        int userId = int.Parse(Console.ReadLine());

        var departments = departmentService.GetAll();
        Console.WriteLine("\nMövcud Shobeler:");
        foreach (var dept in departments)
        {
            Console.WriteLine($"ID: {dept.Id}, Ad: {dept.Name}");
        }

        Console.Write("Shobe ID: ");
        int departmentId = int.Parse(Console.ReadLine());

        Console.Write("Ixtisas: ");
        var specialization = Console.ReadLine();

        Console.Write("Tecrube (il): ");
        int experience = int.Parse(Console.ReadLine());

        var doctor = new Doctor
        {
            UserId = userId,
            Specialization = specialization,
            Experience = experience,
            DepartmentId = departmentId
        };

        var result = doctorService.Add(doctor);
        Console.WriteLine($" Hekim ugurla elave edildi! ID: {result.Id}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Xeta: {ex.Message}");
    }
}

async Task AddDepartment(IDepartmentService departmentService)
{
    try
    {
        Console.WriteLine("\n=== YENI SHOBE ===");

        Console.Write("Shobe adi: ");
        var name = Console.ReadLine();

        Console.Write("Tesvir: ");
        var description = Console.ReadLine();

        var department = new Department
        {
            Name = name,
            Description = description
        };

        var result = departmentService.Add(department);
        Console.WriteLine($" Shobe ugurla elave edildi! ID: {result.Id}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Xeta: {ex.Message}");
    }
}

async Task AddAppointment(IAppointmentService appointmentService, IPatientService patientService, IDoctorService doctorService)
{
    try
    {
        Console.WriteLine("\n=== YENI RANDEVU ===");

        var patients = patientService.GetAll();
        Console.WriteLine("\nMövcud Xesteler:");
        foreach (var patient in patients)
        {
            Console.WriteLine($"ID: {patient.Id}, Ad: {patient.User.Name} {patient.User.Surname}");
        }

        Console.Write("Xeste ID: ");
        int patientId = int.Parse(Console.ReadLine());

        var doctors = doctorService.GetAll();
        Console.WriteLine("\nMövcud Hekimler:");
        foreach (var doctor in doctors)
        {
            Console.WriteLine($"ID: {doctor.Id}, Ad: {doctor.User.Name} {doctor.User.Surname}");
        }

        Console.Write("Hekim ID: ");
        int doctorId = int.Parse(Console.ReadLine());

        Console.Write("Randevu tarixi (yyyy-mm-dd HH:mm): ");
        var appointmentDate = DateTime.Parse(Console.ReadLine());

        var appointment = new Appointment
        {
            PatientId = patientId,
            DoctorId = doctorId,
            AppointmentDate = appointmentDate,
            Status = "Scheduled"
        };

        var result = appointmentService.Add(appointment);
        Console.WriteLine($" Randevu ugurla elave edildi! ID: {result.Id}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Xeta: {ex.Message}");
    }
}

async Task AddMedicalRecord(IMedicalRecordService medicalRecordService, IPatientService patientService, IDoctorService doctorService)
{
    try
    {
        Console.WriteLine("\n=== YENI TIBBI QEYD ===");

        var patients = patientService.GetAll();
        Console.WriteLine("\nMövcud Xesteler:");
        foreach (var patient in patients)
        {
            Console.WriteLine($"ID: {patient.Id}, Ad: {patient.User.Name} {patient.User.Surname}");
        }

        Console.Write("Xeste ID: ");
        int patientId = int.Parse(Console.ReadLine());

        var doctors = doctorService.GetAll();
        Console.WriteLine("\nMövcud Hekimler:");
        foreach (var doctor in doctors)
        {
            Console.WriteLine($"ID: {doctor.Id}, Ad: {doctor.User.Name} {doctor.User.Surname}");
        }

        Console.Write("Hekim ID: ");
        int doctorId = int.Parse(Console.ReadLine());

        Console.Write("Diaqnoz: ");
        var diagnosis = Console.ReadLine();

        Console.Write("Mualice: ");
        var treatment = Console.ReadLine();

        var medicalRecord = new MedicalRecord
        {
            PatientId = patientId,
            DoctorId = doctorId,
            Diagnosis = diagnosis,
            Treatment = treatment,
            RecordDate = DateTime.Now
        };

        var result = medicalRecordService.Add(medicalRecord);
        Console.WriteLine($" Tibbi qeyd ugurla elave edildi! ID: {result.Id}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Xeta: {ex.Message}");
    }
}

async Task GetAllDoctors(IDoctorService doctorService)
{
    try
    {
        var doctors = doctorService.GetAll();
        Console.WriteLine("\n=== BUTUN HEKIMLER ===");
        foreach (var doctor in doctors)
        {
            Console.WriteLine($"ID: {doctor.Id}, Ad: {doctor.User.Name} {doctor.User.Surname}, Ixtisas: {doctor.Specialization}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Xeta: {ex.Message}");
    }
}

async Task GetAllDepartments(IDepartmentService departmentService)
{
    try
    {
        var departments = departmentService.GetAll();
        Console.WriteLine("\n=== BUTUN SHOBELER ===");
        foreach (var department in departments)
        {
            Console.WriteLine($"ID: {department.Id}, Ad: {department.Name}, Tesvir: {department.Description}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Xeta: {ex.Message}");
    }
}

async Task GetAllAppointments(IAppointmentService appointmentService)
{
    try
    {
        var appointments = appointmentService.GetAll();
        Console.WriteLine("\n=== BUTUN RANDEVULAR ===");
        foreach (var appointment in appointments)
        {
            Console.WriteLine($"ID: {appointment.Id}, Xeste ID: {appointment.PatientId}, Hekim ID: {appointment.DoctorId}, Tarix: {appointment.AppointmentDate}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Xeta: {ex.Message}");
    }
}

async Task GetAllMedicalRecords(IMedicalRecordService medicalRecordService)
{
    try
    {
        var records = medicalRecordService.GetAll();
        Console.WriteLine("\n=== BUTUN TIBBI QEYDLER ===");
        foreach (var record in records)
        {
            Console.WriteLine($"ID: {record.Id}, Xeste ID: {record.PatientId}, Hekim ID: {record.DoctorId}, Diaqnoz: {record.Diagnosis}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Xeta: {ex.Message}");
    }
}

async Task DeleteDoctor(IDoctorService doctorService)
{
    try
    {
        Console.Write("Silmek istediyiniz hekim ID: ");
        int id = int.Parse(Console.ReadLine());
        doctorService.Delete(id);
        Console.WriteLine(" Hekim ugurla silindi!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Xeta: {ex.Message}");
    }
}

async Task DeleteDepartment(IDepartmentService departmentService)
{
    try
    {
        Console.Write("Silmek istediyiniz shobe ID: ");
        int id = int.Parse(Console.ReadLine());
        departmentService.Delete(id);
        Console.WriteLine(" Shobe ugurla silindi!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Xeta: {ex.Message}");
    }
}

async Task DeleteAppointment(IAppointmentService appointmentService)
{
    try
    {
        Console.Write("Silmek istediyiniz randevu ID: ");
        int id = int.Parse(Console.ReadLine());
        appointmentService.Delete(id);
        Console.WriteLine(" Randevu ugurla silindi!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Xeta: {ex.Message}");
    }
}

async Task DeleteMedicalRecord(IMedicalRecordService medicalRecordService)
{
    try
    {
        Console.Write("Silmek istediyiniz tibbi qeyd ID: ");
        int id = int.Parse(Console.ReadLine());
        medicalRecordService.Delete(id);
        Console.WriteLine(" Tibbi qeyd ugurla silindi!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Xeta: {ex.Message}");
    }
}